using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Work kind is an admission rule, not a rendering style.  A resize frame
/// owns the next exact metrics target; ordinary animation work is latest-only
/// and can never overtake that target.
/// </summary>
internal enum FlutterWindowsFrameKind
{
    Ordinary,
    Resize,
}

internal enum FlutterWindowsFrameRunDisposition
{
    NoWork,
    SchedulingStopped,
    ResizeInFlight,
    OrdinaryInFlight,
    StaleRejected,
    CallbackFailed,
    Dispatched,
}

internal enum FlutterWindowsRasterAdmissionFailure
{
    None,
    Disposed,
    CrossView,
    SchedulingStopped,
    NotActive,
    StaleMetrics,
    PendingResize,
    WrongExtent,
}

/// <summary>
/// Per-view scheduler boundary used by the Flutter-style host.  It preserves
/// a single bounded pending slot, makes resize admission explicit, and keeps
/// raster completion separate from platform-thread callback dispatch.
/// </summary>
internal interface IFlutterWindowsFrameScheduler :
    IFrameHostCapability,
    IExactFrameHostCapability
{
    event Action<WindowsViewMetrics>? LatestMetricsFrameRequested;

    FlutterWindowsFrameScheduleResult ScheduleOrdinary(
        DorotiViewEpoch expectedEpoch,
        FlutterWindowsScheduledFrameCallback callback);

    FlutterWindowsFrameScheduleResult ScheduleResize(
        WindowsViewMetrics expectedMetrics,
        FlutterWindowsScheduledFrameCallback callback);

    FlutterWindowsFrameRunResult TryRunOneFrame();

    bool TryAdmitRaster(
        FlutterWindowsFrameTicket ticket,
        out FlutterWindowsRasterAdmissionFailure failure);

    bool ReportSwap(
        FlutterWindowsFrameTicket ticket,
        FlutterWindowsAngleEglPresentResult present,
        TimeSpan swapTimestamp);

    bool ReportSkiaReceipt(SkiaFrameReceipt receipt);

    void PublishMetrics(WindowsViewMetrics metrics);

    void SetHidden(bool hidden);

    void SetMinimized(bool minimized);

    void SetSuspended(bool suspended);

    FlutterWindowsFrameSchedulerSnapshot Snapshot { get; }
}

/// <summary>
/// One host-owned causal frame token.  <see cref="Metrics"/> is the original
/// immutable F3 publication; code must not substitute an equal-looking later
/// publication to make a stale scene appear current.
/// </summary>
internal sealed record FlutterWindowsFrameTicket(
    long CausalFrameId,
    FlutterWindowsFrameKind Kind,
    WindowsViewMetrics Metrics,
    DorotiViewEpoch ExpectedEpoch,
    long ScheduleOrder,
    TimeSpan ScheduledAt)
{
    internal ulong ViewId => Metrics.ViewId;
}

internal delegate void FlutterWindowsScheduledFrameCallback(
    FlutterWindowsFrameTicket ticket,
    FlutterWindowsVsyncSample vsync);

internal sealed record FlutterWindowsFrameScheduleResult(
    FlutterWindowsFrameTicket Ticket,
    bool Accepted,
    bool ReplacedLatest,
    bool RejectedAsStale);

internal sealed record FlutterWindowsFrameRunResult(
    FlutterWindowsFrameRunDisposition Disposition,
    FlutterWindowsFrameTicket? Ticket,
    FlutterWindowsVsyncSample? Vsync)
{
    internal bool Dispatched => Disposition == FlutterWindowsFrameRunDisposition.Dispatched;
}

/// <summary>
/// Counters are per view.  The F6 fixture aggregates separate scheduler
/// instances for a multi-window assertion instead of sharing a global queue.
/// A present counter records only real admitted swap/receipt pairs; rejected
/// raster proposals are intentionally kept separate.
/// </summary>
internal sealed record FlutterWindowsFrameSchedulerSnapshot(
    ulong ViewId,
    WindowsViewMetrics CurrentMetrics,
    bool IsHidden,
    bool IsMinimized,
    bool IsSuspended,
    int QueueCapacity,
    int QueueDepth,
    int MaxObservedQueueDepth,
    bool HasPendingResize,
    bool HasPendingOrdinary,
    bool HasResizeInFlight,
    bool HasOrdinaryInFlight,
    long CallbackCount,
    long ResizeCallbackCount,
    long OrdinaryCallbackCount,
    long RasterAdmissionCount,
    long SwapCount,
    long PresentedFrameCount,
    long AnimationStarvationCount,
    long ResizeStarvationCount,
    long FrameQueueOverflowCount,
    long StaleOrWrongSizePresentCount,
    long RejectedStaleOrWrongSizeRasterCount,
    long PendingResizeOrdinaryRejectedCount,
    long CausalGapCount,
    long OrdinaryResumeCount,
    long HiddenStopCount,
    long MinimizedStopCount,
    long SuspendedStopCount,
    long RestoredLatestMetricsCount,
    long DroppedStaleCallbackCount,
    long ResizeMergedCount,
    long OrdinaryMergedCount,
    long CrossViewLeakCount,
    long LastCausalFrameId,
    TimeSpan? LastCallbackTimestamp,
    TimeSpan? LastSwapTimestamp,
    bool Disposed)
{
    internal bool QueueIsBounded =>
        QueueDepth <= QueueCapacity && MaxObservedQueueDepth <= QueueCapacity;
}

/// <summary>
/// Host-only, per-view scheduler. It stores one latest pending callback:
/// a resize replaces or rejects ordinary work, and an ordinary callback is
/// requested again after that resize presents. Scheduling is passive: a host
/// or fixture calls <see cref="TryRunOneFrame"/> at its chosen event-loop
/// boundary; no timer, nested Win32 dispatch, or shared global queue is
/// created here.
/// </summary>
internal sealed class FlutterWindowsFrameScheduler :
    IFlutterWindowsFrameScheduler,
    IDisposable
{
    private const int QueueCapacity = 1;

    private readonly object _gate = new();
    private readonly IFlutterWindowsVsyncSource _vsyncSource;
    private FlutterWindowsViewMetricsCoordinator? _metricsCoordinator;
    private WindowsViewMetrics _currentMetrics;
    private WindowsViewMetrics? _proposedMetrics;
    private ScheduledFrame? _pendingResize;
    private ScheduledFrame? _pendingOrdinary;
    private ScheduledFrame? _activeResize;
    private ScheduledFrame? _activeOrdinary;
    private bool _hidden;
    private bool _minimized;
    private bool _manualSuspended;
    private bool _metricsSuspended;
    private bool _awaitingOrdinaryResume;
    private bool _disposed;
    private long _nextCausalFrameId;
    private long _nextScheduleOrder;
    private int _maxObservedQueueDepth;
    private long _callbackCount;
    private long _resizeCallbackCount;
    private long _ordinaryCallbackCount;
    private long _rasterAdmissionCount;
    private long _swapCount;
    private long _presentedFrameCount;
    // These counters are incremented only by a future policy that detects a
    // missed deadline. Coalescing and resize priority are not starvation.
    private long _animationStarvationCount = 0;
    private long _resizeStarvationCount = 0;
    private long _frameQueueOverflowCount;
    private long _staleOrWrongSizePresentCount;
    private long _rejectedStaleOrWrongSizeRasterCount;
    private long _pendingResizeOrdinaryRejectedCount;
    private long _causalGapCount;
    private long _ordinaryResumeCount;
    private long _hiddenStopCount;
    private long _minimizedStopCount;
    private long _suspendedStopCount;
    private long _restoredLatestMetricsCount;
    private long _droppedStaleCallbackCount;
    private long _resizeMergedCount;
    private long _ordinaryMergedCount;
    private long _crossViewLeakCount;
    private long _lastCausalFrameId;
    private long _rasterAdmittedCausalFrameId;
    private TimeSpan? _lastCallbackTimestamp;
    private TimeSpan? _lastSwapTimestamp;

    internal FlutterWindowsFrameScheduler(
        WindowsViewMetrics initialMetrics,
        IFlutterWindowsVsyncSource vsyncSource)
    {
        ArgumentNullException.ThrowIfNull(initialMetrics);
        _currentMetrics = initialMetrics;
        _metricsSuspended = !initialMetrics.HasDrawableSize;
        _vsyncSource = vsyncSource ?? throw new ArgumentNullException(nameof(vsyncSource));
    }

    internal FlutterWindowsFrameScheduler(
        FlutterWindowsViewMetricsCoordinator metricsCoordinator,
        IFlutterWindowsVsyncSource vsyncSource)
        : this(metricsCoordinator?.Current ?? throw new ArgumentNullException(nameof(metricsCoordinator)), vsyncSource)
    {
        _metricsCoordinator = metricsCoordinator;
        _metricsCoordinator.MetricsPublished += HandleMetricsPublished;
    }

    /// <summary>
    /// Creates the bounded per-view scheduler with the deterministic timing
    /// source used by the F6 cadence contract (60/120/144/165Hz).  It is a
    /// fixture helper only; production timing is supplied by
    /// <see cref="FlutterWindowsDwmVsyncSource"/>.
    /// </summary>
    internal static FlutterWindowsFrameScheduler CreateDeterministicForValidation(
        WindowsViewMetrics initialMetrics,
        double refreshRateHz) => new(
        initialMetrics,
        new FlutterWindowsDeterministicVsyncSource(refreshRateHz));

    /// <summary>
    /// Raised when a stopped scheduler becomes eligible again.  The host uses
    /// the supplied latest F3 publication to request a new framework frame;
    /// an old callback is never relabelled to a new epoch.
    /// </summary>
    public event Action<WindowsViewMetrics>? LatestMetricsFrameRequested;

    /// <summary>
    /// Optional framework capability.  Existing hosts use the legacy
    /// IFrameHostCapability path, while this host keeps the caller's exact
    /// epoch through queue admission and callback dispatch.
    /// </summary>
    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        DorotiViewEpoch expectedEpoch;
        lock (_gate)
        {
            ThrowIfDisposed();
            expectedEpoch = _currentMetrics.ToViewEpoch();
        }
        ScheduleFrame(expectedEpoch, callback);
    }

    public void ScheduleFrame(DorotiViewEpoch expectedEpoch, Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        _ = ScheduleOrdinary(expectedEpoch, (_, vsync) => callback(vsync.Timestamp));
    }

    public FlutterWindowsFrameScheduleResult ScheduleOrdinary(
        DorotiViewEpoch expectedEpoch,
        FlutterWindowsScheduledFrameCallback callback) =>
        ScheduleOrdinary(expectedEpoch, callback, canReplaceBeforeDispatch: false);

    internal FlutterWindowsFrameScheduleResult ScheduleOrdinary(
        DorotiViewEpoch expectedEpoch,
        FlutterWindowsScheduledFrameCallback callback,
        bool canReplaceBeforeDispatch)
    {
        ArgumentNullException.ThrowIfNull(expectedEpoch);
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            ThrowIfDisposed();
            var ticket = CreateTicketNoLock(FlutterWindowsFrameKind.Ordinary, _currentMetrics, expectedEpoch);
            if (expectedEpoch.ViewId != _currentMetrics.ViewId)
            {
                _crossViewLeakCount++;
                return new(ticket, Accepted: false, ReplacedLatest: false, RejectedAsStale: true);
            }
            if (_pendingResize is not null)
            {
                // The single pending slot belongs to the exact resize.  The
                // framework will request the next ordinary animation callback
                // after that resize is presented; retaining this one would
                // create a second queue entry and risk a wrong-size present.
                _pendingResizeOrdinaryRejectedCount++;
                return new(ticket, Accepted: false, ReplacedLatest: false, RejectedAsStale: false);
            }

            // An active resize is no longer in the pending queue. Retain one
            // latest ordinary continuation so a framework scheduler that has
            // already marked its request as pending is guaranteed a callback.
            // A newer metrics publication may replace this not-yet-started
            // continuation with an exact resize ticket.
            var replaced = _pendingOrdinary is not null;
            if (replaced) _ordinaryMergedCount++;
            _pendingOrdinary = new ScheduledFrame(ticket, callback, canReplaceBeforeDispatch);
            UpdateQueueDepthNoLock();
            return new(ticket, Accepted: true, ReplacedLatest: replaced, RejectedAsStale: false);
        }
    }

    public FlutterWindowsFrameScheduleResult ScheduleResize(
        WindowsViewMetrics expectedMetrics,
        FlutterWindowsScheduledFrameCallback callback) =>
        ScheduleResize(expectedMetrics, callback, canReplaceBeforeDispatch: false);

    internal FlutterWindowsFrameScheduleResult ScheduleResize(
        WindowsViewMetrics expectedMetrics,
        FlutterWindowsScheduledFrameCallback callback,
        bool canReplaceBeforeDispatch)
    {
        ArgumentNullException.ThrowIfNull(expectedMetrics);
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            ThrowIfDisposed();
            var ticket = CreateTicketNoLock(
                FlutterWindowsFrameKind.Resize,
                expectedMetrics,
                expectedMetrics.ToViewEpoch());
            if (expectedMetrics.ViewId != _currentMetrics.ViewId)
            {
                _crossViewLeakCount++;
                return new(ticket, Accepted: false, ReplacedLatest: false, RejectedAsStale: true);
            }

            var highestQueuedGeneration = Math.Max(
                _pendingResize?.Ticket.Metrics.ResizeGeneration ?? long.MinValue,
                _activeResize?.Ticket.Metrics.ResizeGeneration ?? long.MinValue);
            if (expectedMetrics.ResizeGeneration < highestQueuedGeneration)
            {
                _droppedStaleCallbackCount++;
                return new(ticket, Accepted: false, ReplacedLatest: false, RejectedAsStale: true);
            }

            var replaced = _pendingResize is not null;
            if (replaced) _resizeMergedCount++;
            if (_pendingOrdinary is not null)
            {
                _pendingOrdinary = null;
                _ordinaryMergedCount++;
                _pendingResizeOrdinaryRejectedCount++;
                replaced = true;
            }
            if (_activeOrdinary is { } activeOrdinary)
            {
                // The resize revokes an ordinary callback that has not
                // reached F4 yet. Any late raster attempt will fail the
                // active-ticket admission check instead of presenting stale
                // ordinary content over the new exact target.
                ClearActiveNoLock(activeOrdinary.Ticket);
                _droppedStaleCallbackCount++;
                _pendingResizeOrdinaryRejectedCount++;
            }
            if (_activeResize is { } active &&
                expectedMetrics.ResizeGeneration > active.Ticket.Metrics.ResizeGeneration &&
                !ReferenceEquals(active.Ticket.Metrics, _currentMetrics))
            {
                // A newer immutable resize revokes admission for the older
                // raster attempt.  It does not block the platform thread.
                ClearActiveNoLock(active.Ticket);
                _resizeMergedCount++;
            }
            _pendingResize = new ScheduledFrame(ticket, callback, canReplaceBeforeDispatch);
            UpdateQueueDepthNoLock();
            return new(ticket, Accepted: true, ReplacedLatest: replaced, RejectedAsStale: false);
        }
    }

    /// <summary>
    /// Takes one eligible frame at a host event-loop boundary.  A resize wins
    /// over ordinary work; any resize currently rastering also prevents an
    /// ordinary frame from running or presenting at an obsolete extent.
    /// </summary>
    public FlutterWindowsFrameRunResult TryRunOneFrame()
    {
        ScheduledFrame? scheduled = null;
        WindowsViewMetrics? latestRequest = null;
        FlutterWindowsFrameRunResult? staleResult = null;
        lock (_gate)
        {
            if (_disposed)
            {
                return new(FlutterWindowsFrameRunDisposition.SchedulingStopped, null, null);
            }
            if (IsSchedulingStoppedNoLock())
            {
                return new(FlutterWindowsFrameRunDisposition.SchedulingStopped, null, null);
            }

            if (_pendingResize is not null)
            {
                scheduled = _pendingResize;
                _pendingResize = null;
                _activeResize = scheduled;
            }
            else if (_activeResize is not null)
            {
                return new(FlutterWindowsFrameRunDisposition.ResizeInFlight, null, null);
            }
            else if (_activeOrdinary is not null)
            {
                return new(FlutterWindowsFrameRunDisposition.OrdinaryInFlight, null, null);
            }
            else if (_pendingOrdinary is not null)
            {
                scheduled = _pendingOrdinary;
                _pendingOrdinary = null;
                _activeOrdinary = scheduled;
            }
            else
            {
                return new(FlutterWindowsFrameRunDisposition.NoWork, null, null);
            }

            UpdateQueueDepthNoLock();
            if (!HasExactRasterMetricsNoLock(scheduled.Ticket))
            {
                ClearActiveNoLock(scheduled.Ticket);
                _droppedStaleCallbackCount++;
                latestRequest = RequestLatestMetricsNoLock(restored: false);
                staleResult = new FlutterWindowsFrameRunResult(
                    FlutterWindowsFrameRunDisposition.StaleRejected,
                    scheduled.Ticket,
                    null);
            }
        }

        if (staleResult is not null)
        {
            if (latestRequest is not null) LatestMetricsFrameRequested?.Invoke(latestRequest);
            return staleResult;
        }

        FlutterWindowsVsyncSample vsync;
        try
        {
            vsync = _vsyncSource.SampleNext(scheduled!.Ticket.ViewId);
        }
        catch
        {
            ReportFrameFailure(scheduled!.Ticket, "vsync timing sample failed");
            return new(FlutterWindowsFrameRunDisposition.CallbackFailed, scheduled.Ticket, null);
        }

        lock (_gate)
        {
            if (_disposed || !IsActiveNoLock(scheduled!.Ticket))
                return new(FlutterWindowsFrameRunDisposition.StaleRejected, scheduled.Ticket, vsync);
            _callbackCount++;
            if (scheduled.Ticket.Kind == FlutterWindowsFrameKind.Resize)
                _resizeCallbackCount++;
            else
                _ordinaryCallbackCount++;
            _lastCausalFrameId = scheduled.Ticket.CausalFrameId;
            // DWM's qpcVBlank is boot-relative; the causal trace uses the
            // common DorotiFrameClock epoch at callback execution while the
            // Vsync sample remains available for cadence measurement.
            _lastCallbackTimestamp = DorotiFrameClock.Now;
        }

        try
        {
            scheduled!.Callback(scheduled.Ticket, vsync);
            return new(FlutterWindowsFrameRunDisposition.Dispatched, scheduled.Ticket, vsync);
        }
        catch
        {
            ReportFrameFailure(scheduled!.Ticket, "scheduled frame callback failed");
            return new(FlutterWindowsFrameRunDisposition.CallbackFailed, scheduled.Ticket, vsync);
        }
    }

    /// <summary>
    /// Final pre-raster admission.  It is intentionally separate from the
    /// callback dequeue so a raster bridge can reject a superseded ticket
    /// immediately before it changes an EGL surface or asks F4 to swap.
    /// </summary>
    public bool TryAdmitRaster(
        FlutterWindowsFrameTicket ticket,
        out FlutterWindowsRasterAdmissionFailure failure)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        lock (_gate)
        {
            if (_disposed)
            {
                failure = FlutterWindowsRasterAdmissionFailure.Disposed;
                return false;
            }
            if (ticket.ViewId != _currentMetrics.ViewId)
            {
                _crossViewLeakCount++;
                failure = FlutterWindowsRasterAdmissionFailure.CrossView;
                return false;
            }
            if (IsSchedulingStoppedNoLock())
            {
                failure = FlutterWindowsRasterAdmissionFailure.SchedulingStopped;
                return false;
            }
            if (!IsActiveNoLock(ticket))
            {
                failure = FlutterWindowsRasterAdmissionFailure.NotActive;
                return false;
            }
            if (ticket.Kind == FlutterWindowsFrameKind.Ordinary &&
                (_pendingResize is not null || _activeResize is not null))
            {
                _pendingResizeOrdinaryRejectedCount++;
                failure = FlutterWindowsRasterAdmissionFailure.PendingResize;
                return false;
            }
            if (!HasExactRasterMetricsNoLock(ticket))
            {
                _rejectedStaleOrWrongSizeRasterCount++;
                failure = FlutterWindowsRasterAdmissionFailure.StaleMetrics;
                return false;
            }
            if (!ticket.Metrics.HasDrawableSize)
            {
                _rejectedStaleOrWrongSizeRasterCount++;
                failure = FlutterWindowsRasterAdmissionFailure.WrongExtent;
                return false;
            }
            if (_rasterAdmittedCausalFrameId != ticket.CausalFrameId)
            {
                _rasterAdmittedCausalFrameId = ticket.CausalFrameId;
                _rasterAdmissionCount++;
            }
            failure = FlutterWindowsRasterAdmissionFailure.None;
            return true;
        }
    }

    /// <summary>
    /// Records a successful real F4 swap before the renderer completes its
    /// paint.  The later Skia receipt must carry this ticket's causal ID and
    /// exact descriptor before the scheduler counts a presented frame.
    /// </summary>
    public bool ReportSwap(
        FlutterWindowsFrameTicket ticket,
        FlutterWindowsAngleEglPresentResult present,
        TimeSpan swapTimestamp)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentNullException.ThrowIfNull(present);
        lock (_gate)
        {
            if (!IsActiveNoLock(ticket))
            {
                _causalGapCount++;
                return false;
            }
            if (!HasExactCurrentMetricsNoLock(ticket) ||
                present.PhysicalWidth != ticket.Metrics.PhysicalWidth ||
                present.PhysicalHeight != ticket.Metrics.PhysicalHeight ||
                !present.SuccessfulSwap)
            {
                // This is an observed bad present, not a rejected proposal.
                _staleOrWrongSizePresentCount++;
                ClearActiveNoLock(ticket);
                return false;
            }
            if (ticket.Kind == FlutterWindowsFrameKind.Ordinary &&
                (_pendingResize is not null || _activeResize is not null))
            {
                _staleOrWrongSizePresentCount++;
                ClearActiveNoLock(ticket);
                return false;
            }
            _swapCount++;
            _lastSwapTimestamp = swapTimestamp;
            return true;
        }
    }

    /// <summary>
    /// Consumes the Skia receipt emitted after the real F4 swap.  A receipt
    /// without a matching callback/swap/descriptor is a causal gap and is not
    /// counted as presentation.
    /// </summary>
    public bool ReportSkiaReceipt(SkiaFrameReceipt receipt)
    {
        ScheduledFrame? active;
        lock (_gate)
        {
            active = FindActiveNoLock(receipt.CausalFrameId);
            if (active is null || receipt.CausalFrameId <= 0)
            {
                _causalGapCount++;
                return false;
            }
            if (!HasExactCurrentMetricsNoLock(active.Ticket) ||
                !DescriptorMatchesTicket(receipt.Descriptor, active.Ticket))
            {
                _causalGapCount++;
                ClearActiveNoLock(active.Ticket);
                return false;
            }
            if (_lastSwapTimestamp is null || _lastCausalFrameId != receipt.CausalFrameId)
            {
                _causalGapCount++;
                ClearActiveNoLock(active.Ticket);
                return false;
            }

            if (receipt.Terminal is DorotiFrameTerminal.presented or DorotiFrameTerminal.submitted)
            {
                _presentedFrameCount++;
                if (active.Ticket.Kind == FlutterWindowsFrameKind.Resize)
                {
                    // The next ordinary animation callback proves cadence
                    // returned after the exact resize chain closed.
                    _awaitingOrdinaryResume = true;
                }
                else if (_awaitingOrdinaryResume)
                {
                    _awaitingOrdinaryResume = false;
                    _ordinaryResumeCount++;
                }
            }
            ClearActiveNoLock(active.Ticket);
            return true;
        }
    }

    internal void ReportFrameFailure(FlutterWindowsFrameTicket ticket, string reason)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        lock (_gate)
        {
            if (IsActiveNoLock(ticket)) ClearActiveNoLock(ticket);
        }
    }

    internal void ReportFrameSuperseded(FlutterWindowsFrameTicket ticket, string reason)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        lock (_gate)
        {
            if (IsActiveNoLock(ticket)) ClearActiveNoLock(ticket);
        }
    }

    /// <summary>Feeds an immutable F3 publication to this per-view scheduler.</summary>
    public void PublishMetrics(WindowsViewMetrics metrics)
    {
        ArgumentNullException.ThrowIfNull(metrics);
        WindowsViewMetrics? latestRequest;
        lock (_gate)
        {
            ThrowIfDisposed();
            if (metrics.ViewId != _currentMetrics.ViewId)
            {
                _crossViewLeakCount++;
                throw new InvalidOperationException(
                    "A Flutter Windows frame scheduler cannot accept metrics for another view.");
            }
            var wasStopped = IsSchedulingStoppedNoLock();
            var wasMetricsSuspended = _metricsSuspended;
            _currentMetrics = metrics;
            _proposedMetrics = null;
            _metricsSuspended = !metrics.HasDrawableSize;
            if (_metricsSuspended && !wasMetricsSuspended) _suspendedStopCount++;
            ReplacePendingWithLatestMetricsNoLock(metrics);
            DropActiveForNewerMetricsNoLock(metrics);
            latestRequest = !IsSchedulingStoppedNoLock() && wasStopped
                ? RequestLatestMetricsNoLock(restored: true)
                : null;
        }
        if (latestRequest is not null) LatestMetricsFrameRequested?.Invoke(latestRequest);
    }

    /// <summary>
    /// Publishes a future WINDOWPOS child extent for non-visible raster
    /// preparation. It can replace pending proposal work, but it does not
    /// revoke an active frame for the still-current admitted child extent.
    /// ReportSwap continues to require <see cref="_currentMetrics"/> exactly.
    /// </summary>
    internal void PublishProposedMetrics(WindowsViewMetrics metrics)
    {
        ArgumentNullException.ThrowIfNull(metrics);
        lock (_gate)
        {
            ThrowIfDisposed();
            if (metrics.ViewId != _currentMetrics.ViewId)
            {
                _crossViewLeakCount++;
                throw new InvalidOperationException(
                    "A Flutter Windows frame scheduler cannot prepare metrics for another view.");
            }
            _proposedMetrics = metrics;
            ReplacePendingWithLatestMetricsNoLock(metrics);
            if (_activeResize is { } active &&
                !ReferenceEquals(active.Ticket.Metrics, _currentMetrics) &&
                !ReferenceEquals(active.Ticket.Metrics, metrics))
            {
                ClearActiveNoLock(active.Ticket);
                _droppedStaleCallbackCount++;
            }
        }
    }

    public void SetHidden(bool hidden)
    {
        WindowsViewMetrics? latestRequest;
        lock (_gate)
        {
            ThrowIfDisposed();
            var wasStopped = IsSchedulingStoppedNoLock();
            if (hidden && !_hidden) _hiddenStopCount++;
            _hidden = hidden;
            latestRequest = !IsSchedulingStoppedNoLock() && wasStopped
                ? RequestLatestMetricsNoLock(restored: true)
                : null;
        }
        if (latestRequest is not null) LatestMetricsFrameRequested?.Invoke(latestRequest);
    }

    public void SetMinimized(bool minimized)
    {
        WindowsViewMetrics? latestRequest;
        lock (_gate)
        {
            ThrowIfDisposed();
            var wasStopped = IsSchedulingStoppedNoLock();
            if (minimized && !_minimized) _minimizedStopCount++;
            _minimized = minimized;
            latestRequest = !IsSchedulingStoppedNoLock() && wasStopped
                ? RequestLatestMetricsNoLock(restored: true)
                : null;
        }
        if (latestRequest is not null) LatestMetricsFrameRequested?.Invoke(latestRequest);
    }

    public void SetSuspended(bool suspended)
    {
        WindowsViewMetrics? latestRequest;
        lock (_gate)
        {
            ThrowIfDisposed();
            var wasStopped = IsSchedulingStoppedNoLock();
            if (suspended && !_manualSuspended) _suspendedStopCount++;
            _manualSuspended = suspended;
            latestRequest = !IsSchedulingStoppedNoLock() && wasStopped
                ? RequestLatestMetricsNoLock(restored: true)
                : null;
        }
        if (latestRequest is not null) LatestMetricsFrameRequested?.Invoke(latestRequest);
    }

    public FlutterWindowsFrameSchedulerSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(
                    _currentMetrics.ViewId,
                    _currentMetrics,
                    _hidden,
                    _minimized,
                    _manualSuspended || _metricsSuspended,
                    QueueCapacity,
                    QueueDepthNoLock(),
                    _maxObservedQueueDepth,
                    _pendingResize is not null,
                    _pendingOrdinary is not null,
                    _activeResize is not null,
                    _activeOrdinary is not null,
                    _callbackCount,
                    _resizeCallbackCount,
                    _ordinaryCallbackCount,
                    _rasterAdmissionCount,
                    _swapCount,
                    _presentedFrameCount,
                    _animationStarvationCount,
                    _resizeStarvationCount,
                    _frameQueueOverflowCount,
                    _staleOrWrongSizePresentCount,
                    _rejectedStaleOrWrongSizeRasterCount,
                    _pendingResizeOrdinaryRejectedCount,
                    _causalGapCount,
                    _ordinaryResumeCount,
                    _hiddenStopCount,
                    _minimizedStopCount,
                    _suspendedStopCount,
                    _restoredLatestMetricsCount,
                    _droppedStaleCallbackCount,
                    _resizeMergedCount,
                    _ordinaryMergedCount,
                    _crossViewLeakCount,
                    _lastCausalFrameId,
                    _lastCallbackTimestamp,
                    _lastSwapTimestamp,
                    _disposed);
            }
        }
    }

    public void Dispose()
    {
        FlutterWindowsViewMetricsCoordinator? coordinator;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _pendingResize = null;
            _pendingOrdinary = null;
            _activeResize = null;
            _activeOrdinary = null;
            coordinator = _metricsCoordinator;
            _metricsCoordinator = null;
        }
        if (coordinator is not null) coordinator.MetricsPublished -= HandleMetricsPublished;
        _vsyncSource.Dispose();
    }

    private void HandleMetricsPublished(WindowsViewMetrics metrics) => PublishMetrics(metrics);

    private FlutterWindowsFrameTicket CreateTicketNoLock(
        FlutterWindowsFrameKind kind,
        WindowsViewMetrics metrics,
        DorotiViewEpoch expectedEpoch) => new(
        checked(++_nextCausalFrameId),
        kind,
        metrics,
        expectedEpoch,
        checked(++_nextScheduleOrder),
        DorotiFrameClock.Now);

    private bool IsSchedulingStoppedNoLock() =>
        _hidden || _minimized || _manualSuspended || _metricsSuspended || !_currentMetrics.HasDrawableSize;

    private bool HasExactCurrentMetricsNoLock(FlutterWindowsFrameTicket ticket) =>
        ticket.ViewId == _currentMetrics.ViewId &&
        ReferenceEquals(ticket.Metrics, _currentMetrics) &&
        ticket.ExpectedEpoch == _currentMetrics.ToViewEpoch() &&
        ticket.Metrics.HasDrawableSize;

    private bool HasExactRasterMetricsNoLock(FlutterWindowsFrameTicket ticket) =>
        HasExactCurrentMetricsNoLock(ticket) ||
        (_proposedMetrics is { } proposed &&
         ticket.ViewId == proposed.ViewId &&
         ReferenceEquals(ticket.Metrics, proposed) &&
         ticket.ExpectedEpoch == proposed.ToViewEpoch() &&
         ticket.Metrics.HasDrawableSize);

    private bool IsActiveNoLock(FlutterWindowsFrameTicket ticket) =>
        (_activeResize?.Ticket.CausalFrameId == ticket.CausalFrameId &&
         ReferenceEquals(_activeResize.Ticket.Metrics, ticket.Metrics)) ||
        (_activeOrdinary?.Ticket.CausalFrameId == ticket.CausalFrameId &&
         ReferenceEquals(_activeOrdinary.Ticket.Metrics, ticket.Metrics));

    private ScheduledFrame? FindActiveNoLock(long causalFrameId)
    {
        if (_activeResize?.Ticket.CausalFrameId == causalFrameId) return _activeResize;
        if (_activeOrdinary?.Ticket.CausalFrameId == causalFrameId) return _activeOrdinary;
        return null;
    }

    private void ClearActiveNoLock(FlutterWindowsFrameTicket ticket)
    {
        if (_activeResize?.Ticket.CausalFrameId == ticket.CausalFrameId)
            _activeResize = null;
        if (_activeOrdinary?.Ticket.CausalFrameId == ticket.CausalFrameId)
            _activeOrdinary = null;
        if (_rasterAdmittedCausalFrameId == ticket.CausalFrameId)
            _rasterAdmittedCausalFrameId = 0;
    }

    private static bool DescriptorMatchesTicket(
        DorotiFrameDescriptor descriptor,
        FlutterWindowsFrameTicket ticket) =>
        descriptor.MatchExact(
            ticket.ExpectedEpoch,
            ticket.Metrics.ToResizeEpoch(),
            ticket.Metrics.PhysicalWidth,
            ticket.Metrics.PhysicalHeight,
            ticket.Metrics.DevicePixelRatio,
            ticket.Metrics.DevicePixelRatio).IsExact;

    private void DropActiveForNewerMetricsNoLock(WindowsViewMetrics metrics)
    {
        if (_activeResize is { } resize && !ReferenceEquals(resize.Ticket.Metrics, metrics))
        {
            ClearActiveNoLock(resize.Ticket);
            _droppedStaleCallbackCount++;
        }
        if (_activeOrdinary is { } ordinary && !ReferenceEquals(ordinary.Ticket.Metrics, metrics))
        {
            ClearActiveNoLock(ordinary.Ticket);
            _droppedStaleCallbackCount++;
        }
    }

    /// <summary>
    /// A framework callback that has not started does not own an immutable
    /// scene yet. Product hosts may therefore replace that pending request with
    /// the latest resize target while preserving a queue depth of one. Once a
    /// callback becomes active, the ordinary stale-admission rules apply.
    /// </summary>
    private void ReplacePendingWithLatestMetricsNoLock(WindowsViewMetrics metrics)
    {
        if (!metrics.HasDrawableSize) return;

        if (_pendingResize is { CanReplaceBeforeDispatch: true } pendingResize &&
            !ReferenceEquals(pendingResize.Ticket.Metrics, metrics))
        {
            _pendingResize = pendingResize with
            {
                Ticket = CreateTicketNoLock(
                    FlutterWindowsFrameKind.Resize,
                    metrics,
                    metrics.ToViewEpoch()),
            };
            _resizeMergedCount++;
        }

        if (_pendingOrdinary is { CanReplaceBeforeDispatch: true } pendingOrdinary &&
            !ReferenceEquals(pendingOrdinary.Ticket.Metrics, metrics))
        {
            _pendingOrdinary = null;
            _pendingResize = new ScheduledFrame(
                CreateTicketNoLock(
                    FlutterWindowsFrameKind.Resize,
                    metrics,
                    metrics.ToViewEpoch()),
                pendingOrdinary.Callback,
                CanReplaceBeforeDispatch: true);
            _ordinaryMergedCount++;
            _resizeMergedCount++;
        }
        UpdateQueueDepthNoLock();
    }

    private WindowsViewMetrics? RequestLatestMetricsNoLock(bool restored)
    {
        if (IsSchedulingStoppedNoLock()) return null;
        if (restored)
        {
            _restoredLatestMetricsCount++;
            _awaitingOrdinaryResume = true;
        }
        return _currentMetrics;
    }

    private void UpdateQueueDepthNoLock()
    {
        var depth = QueueDepthNoLock();
        if (depth > QueueCapacity) _frameQueueOverflowCount++;
        _maxObservedQueueDepth = Math.Max(_maxObservedQueueDepth, depth);
    }

    private int QueueDepthNoLock() =>
        (_pendingResize is null ? 0 : 1) + (_pendingOrdinary is null ? 0 : 1);

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private sealed record ScheduledFrame(
        FlutterWindowsFrameTicket Ticket,
        FlutterWindowsScheduledFrameCallback Callback,
        bool CanReplaceBeforeDispatch);
}
