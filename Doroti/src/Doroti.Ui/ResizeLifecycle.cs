namespace Doroti.Ui;

/// <summary>
/// Immutable size snapshot used to correlate a native resize target with the
/// framework, raster, and native submission work produced for that target.
/// Generations advance only when size, DPR, or the backing context changes.
/// </summary>
public sealed record DorotiResizeEpoch(
    long Generation,
    double LogicalWidth,
    double LogicalHeight,
    int PhysicalWidth,
    int PhysicalHeight,
    double DevicePixelRatio,
    long TimestampMicroseconds);

/// <summary>One platform resize event in the common Doroti trace schema.</summary>
public sealed record DorotiResizeTraceEntry(
    long Sequence,
    long TimestampMicroseconds,
    string Phase,
    DorotiResizeEpoch Epoch,
    int ThreadId,
    string Source,
    long DurationMicroseconds = 0,
    int RafId = 0,
    int BackingWidth = 0,
    int BackingHeight = 0,
    int SurfaceWidth = 0,
    int SurfaceHeight = 0,
    string? Terminal = null,
    string? Detail = null);

/// <summary>
/// Bounded metadata-only trace. It deliberately owns no scene, GPU resource,
/// callback, or platform object and is safe to snapshot from evidence writers.
/// </summary>
public sealed class DorotiResizeTrace
{
    private const int Capacity = 4096;
    private readonly object _gate = new();
    private readonly Queue<DorotiResizeTraceEntry> _entries = new();
    private long _nextSequence;
    private long _lastTimestampMicroseconds;

    public void Record(
        string phase,
        DorotiResizeEpoch epoch,
        string source,
        TimeSpan? duration = null,
        int rafId = 0,
        int backingWidth = 0,
        int backingHeight = 0,
        int surfaceWidth = 0,
        int surfaceHeight = 0,
        string? terminal = null,
        string? detail = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phase);
        ArgumentNullException.ThrowIfNull(epoch);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        lock (_gate)
        {
            var timestamp = Math.Max(_lastTimestampMicroseconds, DorotiFrameClock.Now.Ticks / 10);
            _lastTimestampMicroseconds = timestamp;
            _entries.Enqueue(new(
                ++_nextSequence,
                timestamp,
                phase,
                epoch,
                Environment.CurrentManagedThreadId,
                source,
                Math.Max(0, duration?.Ticks / 10 ?? 0),
                rafId,
                backingWidth,
                backingHeight,
                surfaceWidth,
                surfaceHeight,
                terminal,
                detail));
            while (_entries.Count > Capacity) _entries.Dequeue();
        }
    }

    public IReadOnlyList<DorotiResizeTraceEntry> Snapshot()
    {
        lock (_gate) return _entries.ToArray();
    }
}
