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
    failed,
    lifecycle,
    metrics,
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
    long QueueLatencyMicroseconds = 0);

/// <summary>
/// Small ring buffer deliberately shared by framework dispatch and host raster.
/// It records metadata only, never retains a scene, resource, or callback.
/// </summary>
public sealed class DorotiFrameTrace
{
    private const int Capacity = 256;
    private readonly object _gate = new();
    private readonly Queue<DorotiFrameTraceEntry> _entries = new();
    private long _nextSequence;
    private long _lastTimestampMicroseconds;

    public void Record(
        DorotiFramePhase phase,
        ulong viewId,
        TimeSpan timestamp,
        long inputSequence = 0,
        long sceneSequence = 0,
        long surfaceGeneration = 0,
        string? reason = null,
        TimeSpan? queueLatency = null)
    {
        lock (_gate)
        {
            var timestampMicroseconds = Math.Max(_lastTimestampMicroseconds, timestamp.Ticks / 10);
            _lastTimestampMicroseconds = timestampMicroseconds;
            _entries.Enqueue(new(
                ++_nextSequence,
                timestampMicroseconds,
                phase,
                viewId,
                inputSequence,
                sceneSequence,
                surfaceGeneration,
                reason,
                Math.Max(0, queueLatency?.Ticks / 10 ?? 0)));
            while (_entries.Count > Capacity) _entries.Dequeue();
        }
    }

    public IReadOnlyList<DorotiFrameTraceEntry> Snapshot()
    {
        lock (_gate) return _entries.ToArray();
    }
}
