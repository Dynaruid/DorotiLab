using System.Diagnostics;
using System.Runtime.InteropServices;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal enum FlutterWindowsResizeState
{
    ResizeStarted,
    FrameGenerated,
    SurfaceReady,
    Presented,
    Done,
    TimedOut,
    Failed,
    Superseded,
    Suspended,
}

internal enum FlutterWindowsResizeTerminal
{
    Done,
    TimedOut,
    Failed,
    Superseded,
    Suspended,
}

/// <summary>
/// The only work source the platform resize wait is allowed to poll.  An
/// implementation must execute one already-queued engine task without
/// entering a native window-loop or waiting for a dispatcher callback.
/// </summary>
internal interface IFlutterWindowsEngineTaskRunner
{
    void PostEngineTask(Action task);

    bool TryRunOneTask();
}

/// <summary>
/// Schedules exact frame work onto the raster owner.  The returned operation
/// must complete only after the raster side has either reported a terminal
/// result or deliberately rejected the request.  It is intentionally not a
/// platform dispatcher abstraction.
/// </summary>
internal interface IFlutterWindowsResizeRaster
{
    ValueTask<FlutterWindowsResizeRasterPresentationResult> RenderExactAsync(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame,
        CancellationToken cancellationToken = default);
}

internal delegate FlutterWindowsResizeFrame? FlutterWindowsResizeFrameGenerator(
    FlutterWindowsResizeRequest request);

internal sealed record FlutterWindowsResizeHandshakeOptions(
    TimeSpan MaximumEngineTaskPollDuration,
    TimeSpan MaximumRasterUnblockWaitDuration)
{
    internal static readonly FlutterWindowsResizeHandshakeOptions Default = new(
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(100));

    internal void Validate()
    {
        if (MaximumEngineTaskPollDuration <= TimeSpan.Zero ||
            MaximumEngineTaskPollDuration > TimeSpan.FromMilliseconds(100))
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaximumEngineTaskPollDuration),
                "The Flutter platform resize handshake is bounded to at most 100ms.");
        }
        if (MaximumRasterUnblockWaitDuration <= TimeSpan.Zero ||
            MaximumRasterUnblockWaitDuration > TimeSpan.FromMilliseconds(100))
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaximumRasterUnblockWaitDuration),
                "The raster post-unblock ordering wait is bounded to at most 100ms.");
        }
    }
}

/// <summary>
/// An immutable copy of one F3 metrics publication.  It deliberately keeps
/// the original <see cref="WindowsViewMetrics"/> reference so an equal-looking
/// later observation cannot be relabelled as this resize generation.
/// </summary>
internal sealed record FlutterWindowsResizeRequest(
    WindowsViewMetrics Metrics,
    long ResizeGeneration,
    int PhysicalWidth,
    int PhysicalHeight,
    long StartedTimestampMicroseconds)
{
    internal bool HasDrawableSize =>
        Metrics.HasDrawableSize && PhysicalWidth > 0 && PhysicalHeight > 0;
}

/// <summary>
/// The framework frame built from one immutable resize request.  The raster
/// boundary compares all identity and extent fields before it touches EGL.
/// </summary>
internal sealed record FlutterWindowsResizeFrame(
    WindowsViewMetrics Metrics,
    DorotiFrameDescriptor Descriptor,
    long ResizeGeneration,
    int PhysicalWidth,
    int PhysicalHeight,
    SKColor ClearColor)
{
    internal static FlutterWindowsResizeFrame CreateExact(
        WindowsViewMetrics metrics,
        long frameworkFrameNumber,
        long sceneSequence,
        SKColor clearColor)
    {
        ArgumentNullException.ThrowIfNull(metrics);
        return new FlutterWindowsResizeFrame(
            metrics,
            metrics.CreateFrameDescriptor(frameworkFrameNumber, sceneSequence),
            metrics.ResizeGeneration,
            metrics.PhysicalWidth,
            metrics.PhysicalHeight,
            clearColor);
    }
}

internal sealed record FlutterWindowsResizePollResult(
    long ResizeGeneration,
    FlutterWindowsResizeTerminal Terminal,
    TimeSpan Elapsed,
    bool ChildRectReobservedAfterTimeout,
    bool LatestRedrawRequestedAfterTimeout);

internal sealed record FlutterWindowsResizeDwmFlushResult(
    bool ExecutedAfterPlatformUnblock,
    bool DonePublishedBeforeDwmFlush,
    int HResult,
    string? Failure);

internal sealed record FlutterWindowsResizeRasterPresentationResult(
    bool Accepted,
    bool SurfaceReady,
    bool Presented,
    bool DwmFlushAfterPlatformUnblock,
    bool DonePublishedBeforeDwmFlush,
    int DwmFlushHResult,
    string? Failure);

internal sealed record FlutterWindowsResizeTransactionSnapshot(
    long ResizeGeneration,
    int PhysicalWidth,
    int PhysicalHeight,
    FlutterWindowsResizeState State,
    FlutterWindowsResizeTerminal? Terminal,
    IReadOnlyList<FlutterWindowsResizeState> StateHistory,
    bool PlatformUnblocked,
    bool ChildRectReobservedAfterTimeout,
    bool LatestRedrawRequestedAfterTimeout,
    bool DwmFlushAfterPlatformUnblock,
    bool DonePublishedBeforeDwmFlush,
    string? TerminalDetail);

internal sealed record FlutterWindowsResizeHandshakeSnapshot(
    int PlatformManagedThreadId,
    uint PlatformNativeThreadId,
    int MaximumPollMilliseconds,
    bool EngineTaskRunnerPollOnly,
    long MetricsDeliveredCount,
    long EngineTaskRunnerPollCount,
    long ArbitraryNestedWin32MessageDispatchCount,
    long FrameGeneratedCount,
    long SurfaceReadyCount,
    long PresentedCount,
    long DoneCount,
    long TimedOutCount,
    long FailedCount,
    long SupersededCount,
    long SuspendedCount,
    long ExactGenerationExtentMismatchFrameCount,
    long ExactGenerationExtentMismatchPresentCount,
    long ChildRectReobservedAfterTimeoutCount,
    long LatestRedrawRequestedAfterTimeoutCount,
    long DwmFlushAfterPlatformUnblockCount,
    long DwmFlushBeforeDoneCount,
    long DwmFlushFailureCount,
    long DwmFlushUnblockWaitTimeoutCount,
    long TerminalDuplicateCount,
    long TerminalMissingCount,
    bool AllTerminalsExactlyOnce,
    IReadOnlyList<FlutterWindowsResizeTransactionSnapshot> Transactions);

/// <summary>
/// Flutter-style, platform-owned resize ledger.  F3 publishes the immutable
/// target, an engine task produces an exact frame, and only F4's raster owner
/// may create the corresponding surface and perform the real swap.
/// </summary>
internal sealed class FlutterWindowsResizeHandshake : IDisposable
{
    private readonly object _gate = new();
    private readonly FlutterWindowsViewMetricsCoordinator _metricsCoordinator;
    private readonly IFlutterWindowsEngineTaskRunner _engineTaskRunner;
    private readonly IFlutterWindowsResizeRaster _raster;
    private readonly FlutterWindowsResizeFrameGenerator _frameGenerator;
    private readonly Action<WindowsViewMetrics> _requestLatestRedraw;
    private readonly FlutterWindowsResizeHandshakeOptions _options;
    private readonly int _platformManagedThreadId;
    private readonly uint _platformNativeThreadId;
    private readonly Dictionary<long, Transaction> _transactions = [];
    private Transaction? _active;
    private WindowsViewMetrics? _deferredMetrics;
    private bool _platformPollInProgress;
    private bool _processingDeferredMetrics;
    private bool _subscribed;
    private bool _disposed;
    private long _metricsDeliveredCount;
    private long _engineTaskRunnerPollCount;
    private long _frameGeneratedCount;
    private long _surfaceReadyCount;
    private long _presentedCount;
    private long _doneCount;
    private long _timedOutCount;
    private long _failedCount;
    private long _supersededCount;
    private long _suspendedCount;
    private long _exactGenerationExtentMismatchFrameCount;
    private long _exactGenerationExtentMismatchPresentCount;
    private long _childRectReobservedAfterTimeoutCount;
    private long _latestRedrawRequestedAfterTimeoutCount;
    private long _dwmFlushAfterPlatformUnblockCount;
    private long _dwmFlushBeforeDoneCount;
    private long _dwmFlushFailureCount;
    private long _dwmFlushUnblockWaitTimeoutCount;
    private long _terminalDuplicateCount;

    internal FlutterWindowsResizeHandshake(
        FlutterWindowsViewMetricsCoordinator metricsCoordinator,
        IFlutterWindowsEngineTaskRunner engineTaskRunner,
        IFlutterWindowsResizeRaster raster,
        FlutterWindowsResizeFrameGenerator frameGenerator,
        Action<WindowsViewMetrics>? requestLatestRedraw = null,
        FlutterWindowsResizeHandshakeOptions? options = null,
        bool subscribeToMetricsPublished = true)
    {
        _metricsCoordinator = metricsCoordinator ?? throw new ArgumentNullException(nameof(metricsCoordinator));
        _engineTaskRunner = engineTaskRunner ?? throw new ArgumentNullException(nameof(engineTaskRunner));
        _raster = raster ?? throw new ArgumentNullException(nameof(raster));
        _frameGenerator = frameGenerator ?? throw new ArgumentNullException(nameof(frameGenerator));
        _requestLatestRedraw = requestLatestRedraw ?? (_ => { });
        _options = options ?? FlutterWindowsResizeHandshakeOptions.Default;
        _options.Validate();
        _platformManagedThreadId = Environment.CurrentManagedThreadId;
        _platformNativeThreadId = NativeMethods.GetCurrentThreadId();
        if (subscribeToMetricsPublished)
        {
            _metricsCoordinator.MetricsPublished += HandleMetricsPublished;
            _subscribed = true;
        }
    }

    /// <summary>
    /// Starts one generation, posts its immutable metrics to the engine task
    /// runner, then performs Flutter's bounded engine-only poll.  No native
    /// window-loop work is executed inside this wait.
    /// </summary>
    internal FlutterWindowsResizePollResult BeginResizeAndPoll(WindowsViewMetrics metrics)
    {
        EnsurePlatformThread();
        var request = BeginResize(metrics);
        return PollResize(request);
    }

    /// <summary>
    /// Separating start from poll is useful for explicit supersede/failure
    /// tests.  Production callers should normally use <see cref="BeginResizeAndPoll"/>.
    /// </summary>
    internal FlutterWindowsResizeRequest BeginResize(WindowsViewMetrics metrics)
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(metrics);
        if (!ReferenceEquals(metrics, _metricsCoordinator.Current) ||
            !_metricsCoordinator.TryAdmitMetrics(metrics))
        {
            throw new InvalidOperationException(
                "The resize handshake accepts only the current immutable F3 WindowsViewMetrics publication.");
        }

        Transaction transaction;
        var postEngineTask = false;
        lock (_gate)
        {
            if (_transactions.TryGetValue(metrics.ResizeGeneration, out var existing))
            {
                if (!ReferenceEquals(existing.Request.Metrics, metrics))
                {
                    throw new InvalidOperationException(
                        "A resize generation cannot be rebound to a different immutable metrics object.");
                }
                return existing.Request;
            }

            if (_active is not null && _active.Terminal is null)
                _ = CompleteTerminalLocked(_active, FlutterWindowsResizeTerminal.Superseded,
                    "A newer immutable metrics generation replaced this resize target.");

            var request = new FlutterWindowsResizeRequest(
                metrics,
                metrics.ResizeGeneration,
                metrics.PhysicalWidth,
                metrics.PhysicalHeight,
                DorotiFrameClock.Now.Ticks / 10);
            transaction = new Transaction(request);
            _transactions.Add(request.ResizeGeneration, transaction);
            _active = transaction;
            Interlocked.Increment(ref _metricsDeliveredCount);
            if (!request.HasDrawableSize)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Suspended,
                    "A zero-sized child-client target is suspended and cannot create a surface.");
            }
            else
            {
                postEngineTask = true;
            }
        }

        if (postEngineTask)
        {
            try
            {
                _engineTaskRunner.PostEngineTask(() => DeliverMetricsToEngine(transaction.Request));
            }
            catch (Exception exception)
            {
                _ = ReportFailed(transaction.Request,
                    $"The engine task runner rejected immutable metrics delivery: {exception.GetType().Name}.");
            }
        }
        return transaction.Request;
    }

    /// <summary>
    /// Polls only <see cref="IFlutterWindowsEngineTaskRunner.TryRunOneTask"/>
    /// until the request reaches one terminal state or the strict 100ms bound
    /// expires.  The platform thread deliberately does not run native window
    /// callbacks from this method.
    /// </summary>
    internal FlutterWindowsResizePollResult PollResize(FlutterWindowsResizeRequest request)
    {
        EnsurePlatformThread();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(request);
        var transaction = GetTransaction(request);
        lock (_gate)
        {
            if (_platformPollInProgress)
                throw new InvalidOperationException("Only one platform resize poll may run at a time.");
            _platformPollInProgress = true;
        }

        var started = Stopwatch.GetTimestamp();
        try
        {
            while (true)
            {
                if (TryFinishPlatformPoll(transaction, started, out var completed))
                    return completed;

                if (Stopwatch.GetElapsedTime(started) >= _options.MaximumEngineTaskPollDuration)
                {
                    CompleteTimeoutAndRequestLatestRedraw(transaction);
                    if (TryFinishPlatformPoll(transaction, started, out completed))
                        return completed;
                    throw new InvalidOperationException("Timed-out resize transaction did not receive a terminal state.");
                }

                try
                {
                    var ranTask = _engineTaskRunner.TryRunOneTask();
                    Interlocked.Increment(ref _engineTaskRunnerPollCount);
                    if (!ranTask) Thread.Yield();
                }
                catch (Exception exception)
                {
                    _ = ReportFailed(request,
                        $"Engine task polling failed: {exception.GetType().Name}.");
                }
            }
        }
        finally
        {
            ProcessDeferredMetricsAfterPoll();
        }
    }

    /// <summary>
    /// Framework-side admission.  This runs through the engine task runner on
    /// the platform thread and rejects any frame that was not built from the
    /// same immutable metrics generation and exact child-client extent.
    /// </summary>
    internal bool TryReportFrameGenerated(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame)
    {
        EnsurePlatformThread();
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(frame);
        if (HasTerminal(request)) return false;
        if (!IsExactFrameForRequest(request, frame))
        {
            Interlocked.Increment(ref _exactGenerationExtentMismatchFrameCount);
            _ = ReportFailed(request, "FrameGenerated rejected a non-exact generation or extent.");
            return false;
        }
        if (!_metricsCoordinator.TryAdmitFrame(frame.Descriptor, out var match) || !match.IsExact)
        {
            Interlocked.Increment(ref _exactGenerationExtentMismatchFrameCount);
            _ = ReportFailed(request,
                $"FrameGenerated rejected F3 frame admission: {match.MismatchCode}.");
            return false;
        }

        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            if (transaction.Terminal is not null) return false;
            if (transaction.State != FlutterWindowsResizeState.ResizeStarted)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "FrameGenerated arrived outside ResizeStarted.");
                return false;
            }
            TransitionLocked(transaction, FlutterWindowsResizeState.FrameGenerated);
            Interlocked.Increment(ref _frameGeneratedCount);
            return true;
        }
    }

    /// <summary>
    /// Raster admission happens before F4 receives a surface request.  A stale
    /// or wrong-size frame is terminally rejected rather than relabelled.
    /// </summary>
    internal bool TryAcceptRasterFrame(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(frame);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            if (transaction.Terminal is not null) return false;
            if (!IsExactFrameForRequest(request, frame))
            {
                Interlocked.Increment(ref _exactGenerationExtentMismatchPresentCount);
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "Raster rejected a frame whose exact generation or extent diverged from its resize request.");
                return false;
            }
            if (transaction.State != FlutterWindowsResizeState.FrameGenerated)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "Raster received a frame before FrameGenerated.");
                return false;
            }
            return true;
        }
    }

    internal bool TryReportSurfaceReady(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame,
        FlutterWindowsAngleEglSurfaceUpdateResult surface)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(surface);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            if (transaction.Terminal is not null) return false;
            if (!IsExactFrameForRequest(request, frame) ||
                surface.PhysicalWidth != request.PhysicalWidth ||
                surface.PhysicalHeight != request.PhysicalHeight ||
                surface.SurfaceGeneration <= 0 ||
                !request.HasDrawableSize)
            {
                Interlocked.Increment(ref _exactGenerationExtentMismatchPresentCount);
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "SurfaceReady rejected a non-exact F4 surface extent or generation.");
                return false;
            }
            if (transaction.State != FlutterWindowsResizeState.FrameGenerated)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "SurfaceReady arrived outside FrameGenerated.");
                return false;
            }
            TransitionLocked(transaction, FlutterWindowsResizeState.SurfaceReady);
            Interlocked.Increment(ref _surfaceReadyCount);
            return true;
        }
    }

    internal bool TryReportPresented(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame,
        FlutterWindowsAngleEglPresentResult present)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(frame);
        ArgumentNullException.ThrowIfNull(present);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            if (transaction.Terminal is not null) return false;
            if (!IsExactFrameForRequest(request, frame) ||
                !present.SuccessfulSwap ||
                present.PhysicalWidth != request.PhysicalWidth ||
                present.PhysicalHeight != request.PhysicalHeight ||
                present.SurfaceGeneration <= 0)
            {
                Interlocked.Increment(ref _exactGenerationExtentMismatchPresentCount);
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "Presented rejected a non-exact F4 successful swap.");
                return false;
            }
            if (transaction.State != FlutterWindowsResizeState.SurfaceReady)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed,
                    "Presented arrived outside SurfaceReady.");
                return false;
            }
            TransitionLocked(transaction, FlutterWindowsResizeState.Presented);
            Interlocked.Increment(ref _presentedCount);
            return CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Done,
                "A real F4 eglSwapBuffers completed for the exact resize target.");
        }
    }

    internal bool ReportFailed(FlutterWindowsResizeRequest request, string detail)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            return CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Failed, detail);
        }
    }

    internal bool ReportSuperseded(FlutterWindowsResizeRequest request, string detail)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            return CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Superseded, detail);
        }
    }

    internal bool ReportSuspended(FlutterWindowsResizeRequest request, string detail)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(detail);
        lock (_gate)
        {
            var transaction = GetTransactionLocked(request);
            return CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Suspended, detail);
        }
    }

    /// <summary>
    /// Called by the raster owner only after a successful exact swap has
    /// published Done.  It waits for the platform poll to leave its bounded
    /// loop, then executes the DwmFlush ordering point on that same raster
    /// thread.  It never flushes before Done/platform-unblocked ordering.
    /// </summary>
    internal FlutterWindowsResizeDwmFlushResult DwmFlushAfterPlatformUnblock(
        FlutterWindowsResizeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Transaction transaction;
        lock (_gate)
        {
            transaction = GetTransactionLocked(request);
            if (transaction.Terminal != FlutterWindowsResizeTerminal.Done)
            {
                Interlocked.Increment(ref _dwmFlushBeforeDoneCount);
                transaction.DwmFlushBeforeDone = true;
                return new(false, false, 0,
                    "DwmFlush was refused before the exact swap published Done.");
            }
        }

        if (!transaction.PlatformUnblockedSignal.Wait(_options.MaximumRasterUnblockWaitDuration))
        {
            Interlocked.Increment(ref _dwmFlushUnblockWaitTimeoutCount);
            return new(false, true, 0,
                "DwmFlush did not run because the platform did not unblock within the bounded wait.");
        }

        lock (_gate)
        {
            if (!transaction.PlatformUnblocked || transaction.Terminal != FlutterWindowsResizeTerminal.Done)
            {
                Interlocked.Increment(ref _dwmFlushBeforeDoneCount);
                transaction.DwmFlushBeforeDone = true;
                return new(false, false, 0,
                    "DwmFlush was refused because Done/platform-unblocked ordering was not retained.");
            }
        }

        var result = NativeMethods.DwmFlush();
        if (result < 0)
        {
            Interlocked.Increment(ref _dwmFlushFailureCount);
            return new(false, true, result, $"DwmFlush failed with HRESULT 0x{result:x8}.");
        }

        lock (_gate)
        {
            transaction.DwmFlushAfterPlatformUnblock = true;
            transaction.DonePublishedBeforeDwmFlush = true;
            Interlocked.Increment(ref _dwmFlushAfterPlatformUnblockCount);
        }
        return new(true, true, result, null);
    }

    internal FlutterWindowsResizeHandshakeSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                var transactions = _transactions.Values
                    .OrderBy(transaction => transaction.Request.ResizeGeneration)
                    .Select(transaction => transaction.ToSnapshot())
                    .ToArray();
                var terminalMissing = transactions.LongCount(transaction => transaction.Terminal is null);
                return new FlutterWindowsResizeHandshakeSnapshot(
                    _platformManagedThreadId,
                    _platformNativeThreadId,
                    checked((int)_options.MaximumEngineTaskPollDuration.TotalMilliseconds),
                    EngineTaskRunnerPollOnly: true,
                    Interlocked.Read(ref _metricsDeliveredCount),
                    Interlocked.Read(ref _engineTaskRunnerPollCount),
                    ArbitraryNestedWin32MessageDispatchCount: 0,
                    Interlocked.Read(ref _frameGeneratedCount),
                    Interlocked.Read(ref _surfaceReadyCount),
                    Interlocked.Read(ref _presentedCount),
                    Interlocked.Read(ref _doneCount),
                    Interlocked.Read(ref _timedOutCount),
                    Interlocked.Read(ref _failedCount),
                    Interlocked.Read(ref _supersededCount),
                    Interlocked.Read(ref _suspendedCount),
                    Interlocked.Read(ref _exactGenerationExtentMismatchFrameCount),
                    Interlocked.Read(ref _exactGenerationExtentMismatchPresentCount),
                    Interlocked.Read(ref _childRectReobservedAfterTimeoutCount),
                    Interlocked.Read(ref _latestRedrawRequestedAfterTimeoutCount),
                    Interlocked.Read(ref _dwmFlushAfterPlatformUnblockCount),
                    Interlocked.Read(ref _dwmFlushBeforeDoneCount),
                    Interlocked.Read(ref _dwmFlushFailureCount),
                    Interlocked.Read(ref _dwmFlushUnblockWaitTimeoutCount),
                    Interlocked.Read(ref _terminalDuplicateCount),
                    terminalMissing,
                    terminalMissing == 0 && Interlocked.Read(ref _terminalDuplicateCount) == 0,
                    transactions);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        EnsurePlatformThread();
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            if (_subscribed)
            {
                _metricsCoordinator.MetricsPublished -= HandleMetricsPublished;
                _subscribed = false;
            }
            foreach (var transaction in _transactions.Values.Where(transaction => transaction.Terminal is null))
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.Superseded,
                    "The platform resize owner was disposed before this generation completed.");
            }
        }
    }

    private void HandleMetricsPublished(WindowsViewMetrics metrics)
    {
        EnsurePlatformThread();
        if (_disposed) return;
        lock (_gate)
        {
            if (_platformPollInProgress)
            {
                if (_deferredMetrics is null ||
                    metrics.ResizeGeneration >= _deferredMetrics.ResizeGeneration)
                {
                    _deferredMetrics = metrics;
                }
                return;
            }
        }
        _ = BeginResizeAndPoll(metrics);
    }

    private void DeliverMetricsToEngine(FlutterWindowsResizeRequest request)
    {
        try
        {
            if (HasTerminal(request)) return;
            var frame = _frameGenerator(request);
            if (frame is null)
            {
                _ = ReportFailed(request, "The framework produced no frame for delivered immutable metrics.");
                return;
            }
            if (!TryReportFrameGenerated(request, frame)) return;

            var presentation = _raster.RenderExactAsync(request, frame);
            if (presentation.IsCompletedSuccessfully)
            {
                var completed = presentation.Result;
                if (!completed.Presented && !HasTerminal(request))
                {
                    _ = ReportFailed(request,
                        completed.Failure ?? "Raster returned without an exact successful presentation.");
                }
            }
            else
            {
                _ = ObserveRasterCompletionAsync(presentation, request);
            }
        }
        catch (Exception exception)
        {
            _ = ReportFailed(request,
                $"Framework/raster handoff failed: {exception.GetType().Name}.");
        }
    }

    private async Task ObserveRasterCompletionAsync(
        ValueTask<FlutterWindowsResizeRasterPresentationResult> presentation,
        FlutterWindowsResizeRequest request)
    {
        try
        {
            var completed = await presentation.ConfigureAwait(false);
            if (!completed.Presented && !HasTerminal(request))
            {
                _ = ReportFailed(request,
                    completed.Failure ?? "Raster completed without an exact successful presentation.");
            }
        }
        catch (Exception exception)
        {
            _ = ReportFailed(request, $"Raster completion failed: {exception.GetType().Name}.");
        }
    }

    private bool HasTerminal(FlutterWindowsResizeRequest request)
    {
        lock (_gate)
        {
            return GetTransactionLocked(request).Terminal is not null;
        }
    }

    private void CompleteTimeoutAndRequestLatestRedraw(Transaction transaction)
    {
        lock (_gate)
        {
            if (transaction.Terminal is null)
            {
                _ = CompleteTerminalLocked(transaction, FlutterWindowsResizeTerminal.TimedOut,
                    "The engine-only 100ms platform poll expired before an exact present completed.");
            }
        }

        WindowsViewMetrics current;
        try
        {
            current = _metricsCoordinator.ObserveChildMetrics();
            lock (_gate)
            {
                transaction.ChildRectReobservedAfterTimeout = true;
                Interlocked.Increment(ref _childRectReobservedAfterTimeoutCount);
            }
            RequestLatestRedraw(current);
            lock (_gate)
            {
                transaction.LatestRedrawRequestedAfterTimeout = true;
                Interlocked.Increment(ref _latestRedrawRequestedAfterTimeoutCount);
            }
        }
        catch (Exception exception)
        {
            lock (_gate)
            {
                transaction.TimeoutRecoveryFailure = exception.GetType().Name;
            }
        }
    }

    private void RequestLatestRedraw(WindowsViewMetrics metrics)
    {
        _requestLatestRedraw(metrics);
    }

    private bool TryFinishPlatformPoll(
        Transaction transaction,
        long startedTimestamp,
        out FlutterWindowsResizePollResult result)
    {
        lock (_gate)
        {
            if (transaction.Terminal is null)
            {
                result = default!;
                return false;
            }
            if (!transaction.PlatformUnblocked)
            {
                transaction.PlatformUnblocked = true;
                transaction.PlatformUnblockedSignal.Set();
            }
            result = new FlutterWindowsResizePollResult(
                transaction.Request.ResizeGeneration,
                transaction.Terminal.Value,
                Stopwatch.GetElapsedTime(startedTimestamp),
                transaction.ChildRectReobservedAfterTimeout,
                transaction.LatestRedrawRequestedAfterTimeout);
            return true;
        }
    }

    private void ProcessDeferredMetricsAfterPoll()
    {
        WindowsViewMetrics? deferred = null;
        lock (_gate)
        {
            _platformPollInProgress = false;
            if (!_processingDeferredMetrics && _deferredMetrics is not null)
            {
                deferred = _deferredMetrics;
                _deferredMetrics = null;
                _processingDeferredMetrics = true;
            }
        }
        if (deferred is null) return;
        try
        {
            _ = BeginResizeAndPoll(deferred);
        }
        finally
        {
            lock (_gate) _processingDeferredMetrics = false;
        }
    }

    private Transaction GetTransaction(FlutterWindowsResizeRequest request)
    {
        lock (_gate) return GetTransactionLocked(request);
    }

    private Transaction GetTransactionLocked(FlutterWindowsResizeRequest request)
    {
        if (!_transactions.TryGetValue(request.ResizeGeneration, out var transaction) ||
            !ReferenceEquals(transaction.Request, request))
        {
            throw new InvalidOperationException(
                "The resize request does not belong to this immutable transaction ledger.");
        }
        return transaction;
    }

    private static bool IsExactFrameForRequest(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame)
    {
        var descriptor = frame.Descriptor;
        return ReferenceEquals(frame.Metrics, request.Metrics) &&
               frame.ResizeGeneration == request.ResizeGeneration &&
               frame.PhysicalWidth == request.PhysicalWidth &&
               frame.PhysicalHeight == request.PhysicalHeight &&
               descriptor.ViewId == request.Metrics.ViewId &&
               descriptor.ResizeTargetGeneration == request.ResizeGeneration &&
               descriptor.MetricsGeneration == request.ResizeGeneration &&
               descriptor.PhysicalWidth == request.PhysicalWidth &&
               descriptor.PhysicalHeight == request.PhysicalHeight &&
               descriptor.RootPhysicalWidth == request.PhysicalWidth &&
               descriptor.RootPhysicalHeight == request.PhysicalHeight &&
               descriptor.DevicePixelRatio == request.Metrics.DevicePixelRatio;
    }

    private void TransitionLocked(Transaction transaction, FlutterWindowsResizeState state)
    {
        transaction.State = state;
        transaction.StateHistory.Add(state);
    }

    private bool CompleteTerminalLocked(
        Transaction transaction,
        FlutterWindowsResizeTerminal terminal,
        string detail)
    {
        if (transaction.Terminal is not null)
        {
            Interlocked.Increment(ref _terminalDuplicateCount);
            return false;
        }
        transaction.Terminal = terminal;
        transaction.TerminalDetail = detail;
        TransitionLocked(transaction, terminal switch
        {
            FlutterWindowsResizeTerminal.Done => FlutterWindowsResizeState.Done,
            FlutterWindowsResizeTerminal.TimedOut => FlutterWindowsResizeState.TimedOut,
            FlutterWindowsResizeTerminal.Failed => FlutterWindowsResizeState.Failed,
            FlutterWindowsResizeTerminal.Superseded => FlutterWindowsResizeState.Superseded,
            FlutterWindowsResizeTerminal.Suspended => FlutterWindowsResizeState.Suspended,
            _ => throw new ArgumentOutOfRangeException(nameof(terminal)),
        });
        switch (terminal)
        {
            case FlutterWindowsResizeTerminal.Done:
                Interlocked.Increment(ref _doneCount);
                break;
            case FlutterWindowsResizeTerminal.TimedOut:
                Interlocked.Increment(ref _timedOutCount);
                break;
            case FlutterWindowsResizeTerminal.Failed:
                Interlocked.Increment(ref _failedCount);
                break;
            case FlutterWindowsResizeTerminal.Superseded:
                Interlocked.Increment(ref _supersededCount);
                break;
            case FlutterWindowsResizeTerminal.Suspended:
                Interlocked.Increment(ref _suspendedCount);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(terminal));
        }
        return true;
    }

    private void EnsurePlatformThread()
    {
        if (Environment.CurrentManagedThreadId != _platformManagedThreadId ||
            NativeMethods.GetCurrentThreadId() != _platformNativeThreadId)
        {
            throw new InvalidOperationException(
                "The Flutter bounded resize platform handshake must remain on its owning platform thread.");
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private sealed class Transaction
    {
        internal Transaction(FlutterWindowsResizeRequest request)
        {
            Request = request;
            State = FlutterWindowsResizeState.ResizeStarted;
            StateHistory.Add(FlutterWindowsResizeState.ResizeStarted);
        }

        internal FlutterWindowsResizeRequest Request { get; }

        internal FlutterWindowsResizeState State { get; set; }

        internal FlutterWindowsResizeTerminal? Terminal { get; set; }

        internal List<FlutterWindowsResizeState> StateHistory { get; } = [];

        internal ManualResetEventSlim PlatformUnblockedSignal { get; } = new(false);

        internal bool PlatformUnblocked { get; set; }

        internal bool ChildRectReobservedAfterTimeout { get; set; }

        internal bool LatestRedrawRequestedAfterTimeout { get; set; }

        internal bool DwmFlushAfterPlatformUnblock { get; set; }

        internal bool DonePublishedBeforeDwmFlush { get; set; }

        internal bool DwmFlushBeforeDone { get; set; }

        internal string? TerminalDetail { get; set; }

        internal string? TimeoutRecoveryFailure { get; set; }

        internal FlutterWindowsResizeTransactionSnapshot ToSnapshot() => new(
            Request.ResizeGeneration,
            Request.PhysicalWidth,
            Request.PhysicalHeight,
            State,
            Terminal,
            [.. StateHistory],
            PlatformUnblocked,
            ChildRectReobservedAfterTimeout,
            LatestRedrawRequestedAfterTimeout,
            DwmFlushAfterPlatformUnblock,
            DonePublishedBeforeDwmFlush,
            TerminalDetail ?? TimeoutRecoveryFailure);
    }

    private static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentThreadId();

        [DllImport("dwmapi.dll")]
        internal static extern int DwmFlush();
    }
}

/// <summary>
/// Concrete F4 bridge.  Construct and call this only on the dedicated raster
/// thread: F4 itself enforces that affinity for surface recreation and swap.
/// It reports the state ledger before and after the real F4 operations and
/// performs the post-unblock DwmFlush ordering point on that same thread.
/// </summary>
internal sealed class FlutterWindowsResizeRasterPresenter : IFlutterWindowsResizeRaster
{
    private readonly FlutterWindowsAngleEglWindowSurface _windowSurface;
    private readonly FlutterWindowsResizeHandshake _handshake;

    internal FlutterWindowsResizeRasterPresenter(
        FlutterWindowsAngleEglWindowSurface windowSurface,
        FlutterWindowsResizeHandshake handshake)
    {
        _windowSurface = windowSurface ?? throw new ArgumentNullException(nameof(windowSurface));
        _handshake = handshake ?? throw new ArgumentNullException(nameof(handshake));
    }

    public ValueTask<FlutterWindowsResizeRasterPresentationResult> RenderExactAsync(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _ = _handshake.ReportFailed(request, "Raster work was cancelled before an exact surface could be presented.");
            return ValueTask.FromResult(new FlutterWindowsResizeRasterPresentationResult(
                false, false, false, false, false, 0, "Raster work was cancelled."));
        }
        return ValueTask.FromResult(RenderExactCore(request, frame));
    }

    private FlutterWindowsResizeRasterPresentationResult RenderExactCore(
        FlutterWindowsResizeRequest request,
        FlutterWindowsResizeFrame frame)
    {
        if (!_handshake.TryAcceptRasterFrame(request, frame))
        {
            return new(false, false, false, false, false, 0,
                "Raster rejected a stale, non-exact, or terminal resize frame.");
        }

        try
        {
            var surface = _windowSurface.UpdateForMetrics(request.Metrics);
            if (!_handshake.TryReportSurfaceReady(request, frame, surface))
            {
                return new(true, false, false, false, false, 0,
                    "SurfaceReady was rejected by the exact resize ledger.");
            }

            var present = _windowSurface.RenderAndSwap(request.Metrics, frame.ClearColor);
            if (!_handshake.TryReportPresented(request, frame, present))
            {
                return new(true, true, false, false, false, 0,
                    "The exact F4 swap did not complete the resize ledger.");
            }

            var ordering = _handshake.DwmFlushAfterPlatformUnblock(request);
            return new(
                Accepted: true,
                SurfaceReady: true,
                Presented: true,
                DwmFlushAfterPlatformUnblock: ordering.ExecutedAfterPlatformUnblock,
                DonePublishedBeforeDwmFlush: ordering.DonePublishedBeforeDwmFlush,
                DwmFlushHResult: ordering.HResult,
                Failure: ordering.Failure);
        }
        catch (Exception exception)
        {
            _ = _handshake.ReportFailed(request,
                $"F4 raster surface/present failed: {exception.GetType().Name}.");
            return new(false, false, false, false, false, 0, exception.Message);
        }
    }
}
