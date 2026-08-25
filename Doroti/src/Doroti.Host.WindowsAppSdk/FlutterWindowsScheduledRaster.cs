using System.Collections.Concurrent;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// A raster task runner owns one MTA thread.  Posting must never execute the
/// callback on the caller (normally platform/scheduler) thread.
/// </summary>
internal interface IFlutterWindowsRasterTaskRunner : IDisposable
{
    void Post(Action action);

    ValueTask<T> RunAsync<T>(Func<T> operation, CancellationToken cancellationToken = default);
}

internal sealed record FlutterWindowsDedicatedRasterTaskRunnerSnapshot(
    int ManagedThreadId,
    ApartmentState ApartmentState,
    int QueueDepth,
    int MaxObservedQueueDepth,
    long PostedCount,
    long ExecutedCount,
    bool Disposed);

/// <summary>
/// Small, self-contained dedicated MTA task runner used by the F6 fixture and
/// future Flutter-style host.  Surface creation, Skia paint, and EGL swap all
/// use this single thread; the platform-side scheduler only posts work.
/// </summary>
internal sealed class FlutterWindowsDedicatedRasterTaskRunner : IFlutterWindowsRasterTaskRunner
{
    private readonly BlockingCollection<IRasterWorkItem> _queue = new();
    private readonly Thread _thread;
    private int _managedThreadId;
    private int _queueDepth;
    private int _maxObservedQueueDepth;
    private long _postedCount;
    private long _executedCount;
    private int _disposed;

    internal FlutterWindowsDedicatedRasterTaskRunner(string? name = null)
    {
        _thread = new(Run)
        {
            IsBackground = true,
            Name = name ?? "Doroti Flutter Windows scheduled raster",
        };
        _thread.SetApartmentState(ApartmentState.MTA);
        _thread.Start();
    }

    public void Post(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Enqueue(new ActionRasterWorkItem(action));
    }

    public ValueTask<T> RunAsync<T>(Func<T> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (cancellationToken.IsCancellationRequested)
            return ValueTask.FromCanceled<T>(cancellationToken);
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            Enqueue(new ResultRasterWorkItem<T>(operation, completion, cancellationToken));
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }
        return new(completion.Task);
    }

    internal ValueTask DrainAsync(CancellationToken cancellationToken = default) =>
        new(RunAsync(static () => true, cancellationToken).AsTask());

    internal FlutterWindowsDedicatedRasterTaskRunnerSnapshot Snapshot => new(
        Volatile.Read(ref _managedThreadId),
        _thread.GetApartmentState(),
        Volatile.Read(ref _queueDepth),
        Volatile.Read(ref _maxObservedQueueDepth),
        Interlocked.Read(ref _postedCount),
        Interlocked.Read(ref _executedCount),
        Volatile.Read(ref _disposed) != 0);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _queue.CompleteAdding();
        if (Thread.CurrentThread != _thread)
            _ = _thread.Join(TimeSpan.FromSeconds(5));
        _queue.Dispose();
    }

    private void Enqueue(IRasterWorkItem item)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        // Claim queue depth before publishing the item. The dedicated thread
        // can consume immediately after Add, so incrementing afterward would
        // let it decrement zero and produce a transient negative depth.
        var depth = Interlocked.Increment(ref _queueDepth);
        UpdateMaxQueueDepth(depth);
        try
        {
            _queue.Add(item);
        }
        catch (InvalidOperationException exception)
        {
            Interlocked.Decrement(ref _queueDepth);
            throw new ObjectDisposedException(nameof(FlutterWindowsDedicatedRasterTaskRunner), exception);
        }
        catch
        {
            Interlocked.Decrement(ref _queueDepth);
            throw;
        }
        Interlocked.Increment(ref _postedCount);
    }

    private void Run()
    {
        Volatile.Write(ref _managedThreadId, Environment.CurrentManagedThreadId);
        foreach (var item in _queue.GetConsumingEnumerable())
        {
            Interlocked.Decrement(ref _queueDepth);
            try
            {
                item.Execute();
            }
            finally
            {
                Interlocked.Increment(ref _executedCount);
            }
        }
    }

    private void UpdateMaxQueueDepth(int depth)
    {
        while (true)
        {
            var current = Volatile.Read(ref _maxObservedQueueDepth);
            if (depth <= current) return;
            if (Interlocked.CompareExchange(ref _maxObservedQueueDepth, depth, current) == current)
                return;
        }
    }

    private interface IRasterWorkItem
    {
        void Execute();
    }

    private sealed class ActionRasterWorkItem(Action action) : IRasterWorkItem
    {
        public void Execute() => action();
    }

    private sealed class ResultRasterWorkItem<T>(
        Func<T> operation,
        TaskCompletionSource<T> completion,
        CancellationToken cancellationToken) : IRasterWorkItem
    {
        public void Execute()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                completion.TrySetCanceled(cancellationToken);
                return;
            }
            try
            {
                completion.TrySetResult(operation());
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        }
    }
}

/// <summary>
/// One causal callback-to-present trace. The four timestamps are captured by
/// their respective owners: scheduler callback, dedicated raster entry, real
/// F4 swap, and the Skia terminal receipt.
/// </summary>
internal sealed record FlutterWindowsScheduledRasterCausalTrace(
    long CausalFrameId,
    ulong ViewId,
    long ResizeGeneration,
    int PhysicalWidth,
    int PhysicalHeight,
    TimeSpan CallbackTimestamp,
    TimeSpan RasterTimestamp,
    TimeSpan? SwapTimestamp,
    TimeSpan? PresentedTimestamp,
    bool ExactMetrics,
    bool Presented);

internal sealed record FlutterWindowsScheduledRasterResult(
    long CausalFrameId,
    bool Accepted,
    bool Rasterized,
    bool Swapped,
    bool Presented,
    FlutterWindowsRasterAdmissionFailure AdmissionFailure,
    string? Failure,
    FlutterWindowsScheduledRasterCausalTrace? CausalTrace = null);

internal sealed record FlutterWindowsScheduledRasterSnapshot(
    ulong ViewId,
    long QueuedRasterCount,
    long RasterCount,
    long RejectedRasterCount,
    long SwapCount,
    long PresentedReceiptCount,
    long CausalReceiptMismatchCount,
    long CallbackPostedCount,
    long FailureCount,
    FlutterWindowsScheduledRasterCausalTrace? LastCausalTrace,
    bool Disposed);

/// <summary>
/// Presentation boundary shared by the exact ANGLE fixture and the product
/// compositor front. The scheduler owns metrics/causal admission; the
/// implementation owns its raster-thread-affine native surface and present.
/// </summary>
internal interface IFlutterWindowsScheduledSurface : IDisposable
{
    FlutterWindowsAngleEglSurfaceUpdateResult UpdateForMetrics(WindowsViewMetrics targetMetrics);

    FlutterWindowsAngleEglPresentResult RenderAndSwap(
        WindowsViewMetrics targetMetrics,
        Action<SKSurface> paint,
        Action? beforeSwap = null);
}

/// <summary>
/// Joins F6 scheduler tickets to the F4 exact surface.  Its callback is safe
/// to run from the platform scheduler because it only posts to the dedicated
/// raster runner.  The raster thread rechecks immutable metrics before it
/// creates a surface, paints under the causal ID, makes a real F4 swap, then
/// consumes the Skia receipt emitted by <see cref="SkiaSceneRenderer"/>.
/// </summary>
internal sealed class FlutterWindowsScheduledRaster : IDisposable
{
    private readonly FlutterWindowsFrameScheduler _scheduler;
    private readonly IFlutterWindowsScheduledSurface _windowSurface;
    private readonly SkiaSceneRenderer _renderer;
    private readonly IFlutterWindowsRasterTaskRunner _rasterTaskRunner;
    private readonly FlutterWindowsResizeTrace? _resizeTrace;
    private readonly ConcurrentDictionary<long, CausalRasterState> _causalStates = [];
    private long _queuedRasterCount;
    private long _rasterCount;
    private long _rejectedRasterCount;
    private long _swapCount;
    private long _presentedReceiptCount;
    private long _causalReceiptMismatchCount;
    private long _callbackPostedCount;
    private long _failureCount;
    private FlutterWindowsScheduledRasterCausalTrace? _lastCausalTrace;
    private int _disposed;

    internal FlutterWindowsScheduledRaster(
        FlutterWindowsFrameScheduler scheduler,
        IFlutterWindowsScheduledSurface windowSurface,
        SkiaSceneRenderer renderer,
        IFlutterWindowsRasterTaskRunner rasterTaskRunner,
        FlutterWindowsResizeTrace? resizeTrace = null)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _windowSurface = windowSurface ?? throw new ArgumentNullException(nameof(windowSurface));
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _rasterTaskRunner = rasterTaskRunner ?? throw new ArgumentNullException(nameof(rasterTaskRunner));
        _resizeTrace = resizeTrace;
        _renderer.FrameReceipt += HandleFrameReceipt;
    }

    /// <summary>
    /// Raised after a scheduled raster reaches a terminal causal observation.
    /// The event reports timestamps from the real scheduler/raster/swap/Skia
    /// owners rather than a fixture-side approximation.
    /// </summary>
    internal event Action<FlutterWindowsScheduledRasterCausalTrace>? CausalTraceCompleted;

    /// <summary>
    /// Returns the callback passed to <see cref="FlutterWindowsFrameScheduler"/>.
    /// It posts asynchronously and therefore never calls F4 on the platform
    /// thread that ran <c>TryRunOneFrame</c>.
    /// </summary>
    internal FlutterWindowsScheduledFrameCallback CreateFrameCallback() => QueueRender;

    internal void QueueRender(FlutterWindowsFrameTicket ticket, FlutterWindowsVsyncSample vsync)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ThrowIfDisposed();
        var callbackTimestamp = DorotiFrameClock.Now;
        Interlocked.Increment(ref _callbackPostedCount);
        Interlocked.Increment(ref _queuedRasterCount);
        try
        {
            _rasterTaskRunner.Post(() => _ = RenderExactCore(ticket, vsync, callbackTimestamp));
        }
        catch (Exception exception)
        {
            Interlocked.Increment(ref _failureCount);
            _scheduler.ReportFrameFailure(ticket, $"raster post failed: {exception.GetType().Name}");
        }
    }

    internal ValueTask<FlutterWindowsScheduledRasterResult> RenderExactAsync(
        FlutterWindowsFrameTicket ticket,
        FlutterWindowsVsyncSample vsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ThrowIfDisposed();
        var callbackTimestamp = DorotiFrameClock.Now;
        Interlocked.Increment(ref _queuedRasterCount);
        return _rasterTaskRunner.RunAsync(
            () => RenderExactCore(ticket, vsync, callbackTimestamp),
            cancellationToken);
    }

    internal FlutterWindowsScheduledRasterSnapshot Snapshot => new(
        _scheduler.Snapshot.ViewId,
        Interlocked.Read(ref _queuedRasterCount),
        Interlocked.Read(ref _rasterCount),
        Interlocked.Read(ref _rejectedRasterCount),
        Interlocked.Read(ref _swapCount),
        Interlocked.Read(ref _presentedReceiptCount),
        Interlocked.Read(ref _causalReceiptMismatchCount),
        Interlocked.Read(ref _callbackPostedCount),
        Interlocked.Read(ref _failureCount),
        Volatile.Read(ref _lastCausalTrace),
        Volatile.Read(ref _disposed) != 0);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _renderer.FrameReceipt -= HandleFrameReceipt;
    }

    private FlutterWindowsScheduledRasterResult RenderExactCore(
        FlutterWindowsFrameTicket ticket,
        FlutterWindowsVsyncSample vsync,
        TimeSpan callbackTimestamp)
    {
        Interlocked.Increment(ref _rasterCount);
        var causalState = new CausalRasterState(
            ticket.CausalFrameId,
            ticket.ViewId,
            ticket.Metrics,
            callbackTimestamp,
            DorotiFrameClock.Now);
        _causalStates[ticket.CausalFrameId] = causalState;
        _resizeTrace?.Record("rasterStarted", ticket.Metrics, ticket.CausalFrameId,
            $"kind={ticket.Kind}");
        if (!_scheduler.TryAdmitRaster(ticket, out var admissionFailure))
        {
            Interlocked.Increment(ref _rejectedRasterCount);
            _scheduler.ReportFrameSuperseded(ticket, $"F6 raster rejected: {admissionFailure}");
            var trace = CompleteCausalTrace(causalState, exactMetrics: false, presented: false);
            return new(ticket.CausalFrameId, false, false, false, false, admissionFailure, null, trace);
        }
        causalState.ExactMetrics = true;

        SkiaPaintResult? paint = null;
        try
        {
            _windowSurface.UpdateForMetrics(ticket.Metrics);
            var present = _windowSurface.RenderAndSwap(ticket.Metrics, surface =>
            {
                if (!_scheduler.TryAdmitRaster(ticket, out var callbackAdmissionFailure))
                    throw new RasterAdmissionException(callbackAdmissionFailure);

                paint = _renderer.Paint(
                    surface,
                    ticket.Metrics.PhysicalWidth,
                    ticket.Metrics.PhysicalHeight,
                    ticket.Metrics.ToResizeEpoch(),
                    ticket.CausalFrameId);
                if (!paint.Value.ShouldPresent || paint.Value.Completion is not { })
                    throw new RasterAdmissionException(FlutterWindowsRasterAdmissionFailure.StaleMetrics);
                _resizeTrace?.Record("rasterPrepared", ticket.Metrics, ticket.CausalFrameId);
            }, () => _resizeTrace?.Record("swapStarted", ticket.Metrics, ticket.CausalFrameId));

            causalState.SwapTimestamp = DorotiFrameClock.Now;
            _resizeTrace?.Record("swapCompleted", ticket.Metrics, ticket.CausalFrameId,
                $"surfaceGeneration={present.SurfaceGeneration}");
            if (!_scheduler.ReportSwap(ticket, present, causalState.SwapTimestamp.Value))
            {
                Interlocked.Increment(ref _rejectedRasterCount);
                if (paint?.Completion is { } rejectedCompletion)
                    _renderer.SupersedePaint(rejectedCompletion,
                        "F6 scheduler rejected the F4 swap receipt");
                else
                    _scheduler.ReportFrameSuperseded(ticket, "F6 swap receipt was rejected");
                var trace = CompleteCausalTrace(causalState, exactMetrics: false, presented: false);
                return new(ticket.CausalFrameId, true, true, false, false,
                    FlutterWindowsRasterAdmissionFailure.StaleMetrics,
                    "F6 scheduler rejected the F4 swap receipt.", trace);
            }

            Interlocked.Increment(ref _swapCount);
            var completion = paint?.Completion ?? throw new InvalidOperationException(
                "An admitted F6 raster swap requires a Skia paint completion.");
            _renderer.CompletePaint(completion);
            var completedTrace = CompleteCausalTrace(
                causalState,
                exactMetrics: causalState.ExactMetrics && causalState.ReceiptAccepted,
                presented: causalState.Presented);
            return new(ticket.CausalFrameId, true, true, true, completedTrace.Presented,
                FlutterWindowsRasterAdmissionFailure.None,
                vsync.IsDwmCompositionTiming ? null : "deterministic timing source",
                completedTrace);
        }
        catch (RasterAdmissionException exception)
        {
            Interlocked.Increment(ref _rejectedRasterCount);
            if (paint?.Completion is { } completion)
                _renderer.SupersedePaint(completion,
                    $"F6 raster admission changed to {exception.Failure}");
            _scheduler.ReportFrameSuperseded(ticket,
                $"F6 raster admission changed to {exception.Failure}");
            var trace = CompleteCausalTrace(causalState, exactMetrics: false, presented: false);
            return new(ticket.CausalFrameId, false, paint is not null, false, false,
                exception.Failure,
                "The exact scheduler admission changed before F4 swap.", trace);
        }
        catch (Exception exception)
        {
            Interlocked.Increment(ref _failureCount);
            if (paint?.Completion is { } completion)
                _renderer.FailPaint(completion, $"F6 scheduled raster failed: {exception.GetType().Name}");
            _scheduler.ReportFrameFailure(ticket,
                $"F6 scheduled raster failed: {exception.GetType().Name}");
            var trace = CompleteCausalTrace(causalState, exactMetrics: false, presented: false);
            return new(ticket.CausalFrameId, false, paint is not null, false, false,
                FlutterWindowsRasterAdmissionFailure.None,
                exception.Message, trace);
        }
    }

    private void HandleFrameReceipt(SkiaFrameReceipt receipt)
    {
        if (!receipt.HasCausalFrameId) return;
        var accepted = _scheduler.ReportSkiaReceipt(receipt);
        if (_causalStates.TryGetValue(receipt.CausalFrameId, out var causalState))
        {
            causalState.ReceiptTimestamp = receipt.Timestamp;
            causalState.ReceiptAccepted = accepted;
            causalState.Presented = accepted &&
                receipt.Terminal is DorotiFrameTerminal.presented or DorotiFrameTerminal.submitted;
        }
        if (accepted && receipt.Terminal is DorotiFrameTerminal.presented or DorotiFrameTerminal.submitted)
            Interlocked.Increment(ref _presentedReceiptCount);
        else if (!accepted)
            Interlocked.Increment(ref _causalReceiptMismatchCount);
    }

    private FlutterWindowsScheduledRasterCausalTrace CompleteCausalTrace(
        CausalRasterState state,
        bool exactMetrics,
        bool presented)
    {
        var trace = new FlutterWindowsScheduledRasterCausalTrace(
            state.CausalFrameId,
            state.ViewId,
            state.Metrics.ResizeGeneration,
            state.Metrics.PhysicalWidth,
            state.Metrics.PhysicalHeight,
            state.CallbackTimestamp,
            state.RasterTimestamp,
            state.SwapTimestamp,
            state.ReceiptTimestamp,
            exactMetrics,
            presented);
        _ = _causalStates.TryRemove(state.CausalFrameId, out _);
        Volatile.Write(ref _lastCausalTrace, trace);
        _resizeTrace?.Record(
            presented ? "presented" : "frameTerminal",
            state.Metrics,
            state.CausalFrameId,
            $"exact={exactMetrics};presented={presented}",
            captureGeometry: presented);
        CausalTraceCompleted?.Invoke(trace);
        return trace;
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(
        Volatile.Read(ref _disposed) != 0,
        this);

    private sealed class RasterAdmissionException(
        FlutterWindowsRasterAdmissionFailure failure) : Exception
    {
        internal FlutterWindowsRasterAdmissionFailure Failure { get; } = failure;
    }

    private sealed class CausalRasterState(
        long causalFrameId,
        ulong viewId,
        WindowsViewMetrics metrics,
        TimeSpan callbackTimestamp,
        TimeSpan rasterTimestamp)
    {
        internal long CausalFrameId { get; } = causalFrameId;
        internal ulong ViewId { get; } = viewId;
        internal WindowsViewMetrics Metrics { get; } = metrics;
        internal TimeSpan CallbackTimestamp { get; } = callbackTimestamp;
        internal TimeSpan RasterTimestamp { get; } = rasterTimestamp;
        internal TimeSpan? SwapTimestamp { get; set; }
        internal TimeSpan? ReceiptTimestamp { get; set; }
        internal bool ExactMetrics { get; set; }
        internal bool ReceiptAccepted { get; set; }
        internal bool Presented { get; set; }
    }
}
