using System.Diagnostics;

namespace Doroti.Ui;

/// <summary>
/// The common, monotonic time domain for native input, vsync, framework work,
/// and terminal rendering.  Host wall clocks must not be used for frame data:
/// they can jump while a device is suspended or its clock is corrected.
/// </summary>
public static class DorotiFrameClock
{
    private static readonly long Origin = Stopwatch.GetTimestamp();

    public static TimeSpan Now => Stopwatch.GetElapsedTime(Origin);

    public static TimeSpan ClampForward(TimeSpan candidate, TimeSpan previous) =>
        candidate < previous ? previous : candidate;
}

public enum DorotiFramePhase
{
    input,
    scheduleFrame,
    beginFrame,
    transientCallbacks,
    midFrameMicrotasks,
    persistentCallbacks,
    postFrameCallbacks,
    build,
    layout,
    paint,
    sceneBuild,
    drawFrame,
    sceneSubmitted,
    raster,
    present,
    replay,
    superseded,
    dropped,
    failed,
    lifecycle,
    metrics,
    scrollStart,
    scrollUpdate,
    scrollEnd,
    animationStart,
    animationEnd,
    rasterEnd,
    semanticsBuild,
    semanticsBuildEnd,
    semanticsApply,
    semanticsApplyEnd,
    semanticsDeferred,
}

/// <summary>One causally ordered item in a bounded per-view frame trace.</summary>
public sealed record DorotiFrameTraceEntry(
    long Sequence,
    long TimestampMicroseconds,
    DorotiFramePhase Phase,
    ulong ViewId,
    long InputSequence = 0,
    long SceneSequence = 0,
    long SurfaceGeneration = 0,
    string? Reason = null,
    long QueueLatencyMicroseconds = 0,
    long ScrollPositionId = 0,
    double? ScrollOffset = null,
    double? ScrollDelta = null,
    string? ScrollActivity = null,
    double? ScrollMinExtent = null,
    double? ScrollMaxExtent = null,
    long TickerId = 0,
    string? TickerLabel = null);

/// <summary>
/// Small ring buffer deliberately shared by framework dispatch and host raster.
/// It records metadata only, never retains a scene, resource, or callback.
/// </summary>
public sealed class DorotiFrameTrace
{
    // A 256-entry trace retained less than two seconds on a high-refresh-rate
    // scroll because each frame contributes several framework/host phases. It
    // routinely evicted the input and scroll-start entries before the terminal
    // present could be inspected. Keep enough metadata for a complete gesture.
    private const int Capacity = 8192;
    private readonly object _gate = new();
    private readonly Queue<DorotiFrameTraceEntry> _entries = new();
    private long _nextSequence;
    private long _lastTimestampMicroseconds;
    private long _lastInputSequence;
    private readonly HashSet<long> _activeScrollPositions = [];

    public bool HasActiveScrollActivity
    {
        get
        {
            lock (_gate) return _activeScrollPositions.Count != 0;
        }
    }

    public void Record(
        DorotiFramePhase phase,
        ulong viewId,
        TimeSpan timestamp,
        long inputSequence = 0,
        long sceneSequence = 0,
        long surfaceGeneration = 0,
        string? reason = null,
        TimeSpan? queueLatency = null,
        long scrollPositionId = 0,
        double? scrollOffset = null,
        double? scrollDelta = null,
        string? scrollActivity = null,
        double? scrollMinExtent = null,
        double? scrollMaxExtent = null,
        long tickerId = 0,
        string? tickerLabel = null)
    {
        lock (_gate)
        {
            var timestampMicroseconds = Math.Max(_lastTimestampMicroseconds, timestamp.Ticks / 10);
            _lastTimestampMicroseconds = timestampMicroseconds;
            if (phase == DorotiFramePhase.input && inputSequence > 0)
                _lastInputSequence = inputSequence;
            _entries.Enqueue(new(
                ++_nextSequence,
                timestampMicroseconds,
                phase,
                viewId,
                inputSequence,
                sceneSequence,
                surfaceGeneration,
                reason,
                Math.Max(0, queueLatency?.Ticks / 10 ?? 0),
                scrollPositionId,
                scrollOffset,
                scrollDelta,
                scrollActivity,
                scrollMinExtent,
                scrollMaxExtent,
                tickerId,
                tickerLabel));
            while (_entries.Count > Capacity) _entries.Dequeue();
        }
    }

    public void RecordScroll(
        DorotiFramePhase phase,
        ulong viewId,
        long scrollPositionId,
        double scrollOffset,
        double? scrollDelta,
        string scrollActivity,
        double? scrollMinExtent = null,
        double? scrollMaxExtent = null)
    {
        if (phase is not (DorotiFramePhase.scrollStart or
            DorotiFramePhase.scrollUpdate or DorotiFramePhase.scrollEnd))
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "A scroll phase is required.");

        long inputSequence;
        lock (_gate)
        {
            inputSequence = _lastInputSequence;
            if (phase == DorotiFramePhase.scrollStart) _activeScrollPositions.Add(scrollPositionId);
            else if (phase == DorotiFramePhase.scrollEnd) _activeScrollPositions.Remove(scrollPositionId);
        }
        Record(phase, viewId, DorotiFrameClock.Now, inputSequence,
            scrollPositionId: scrollPositionId, scrollOffset: scrollOffset,
            scrollDelta: scrollDelta, scrollActivity: scrollActivity,
            scrollMinExtent: scrollMinExtent, scrollMaxExtent: scrollMaxExtent);
    }

    public void RecordTicker(
        DorotiFramePhase phase,
        long tickerId,
        string tickerLabel)
    {
        if (phase is not (DorotiFramePhase.animationStart or DorotiFramePhase.animationEnd))
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "An animation phase is required.");

        long inputSequence;
        lock (_gate) inputSequence = _lastInputSequence;
        Record(phase, 0, DorotiFrameClock.Now, inputSequence,
            tickerId: tickerId, tickerLabel: tickerLabel);
    }

    public IReadOnlyList<DorotiFrameTraceEntry> Snapshot()
    {
        lock (_gate) return _entries.ToArray();
    }
}
