namespace Doroti.Ui;

/// <summary>
/// The stages a scroll input must traverse before it can be accepted as a
/// presented scroll result. The trace is diagnostic metadata only; it never
/// owns input packets, widgets, scenes, or native resources.
/// </summary>
public enum DorotiScrollTracePhase
{
    nativeInput,
    pointerData,
    hitTest,
    gesture,
    activity,
    viewport,
    layout,
    paint,
    retainedLayer,
    raster,
    present,
    scrollbar,
    semantics,
    failed,
}

public sealed record DorotiScrollTraceEntry(
    long Sequence,
    long InputSequence,
    long TimestampMicroseconds,
    DorotiScrollTracePhase Phase,
    ulong ViewId,
    string? Detail = null);

/// <summary>
/// Bounded, causally keyed scroll diagnostics. Consumers must supply the
/// input sequence returned by <see cref="Begin"/>; this prevents unrelated
/// frames from being attributed to an earlier scroll packet.
/// </summary>
public sealed class DorotiScrollTrace
{
    private const int Capacity = 512;
    private readonly object _gate = new();
    private readonly Queue<DorotiScrollTraceEntry> _entries = new();
    private long _nextSequence;
    private long _nextInputSequence;
    private long _lastTimestampMicroseconds;

    public long Begin(ulong viewId, DorotiScrollTracePhase phase, string? detail = null)
    {
        if (phase is not (DorotiScrollTracePhase.nativeInput or DorotiScrollTracePhase.pointerData))
            throw new ArgumentOutOfRangeException(nameof(phase), "A scroll trace must start at native input or PointerData.");

        lock (_gate)
        {
            var inputSequence = ++_nextInputSequence;
            Add(inputSequence, viewId, phase, detail);
            return inputSequence;
        }
    }

    public void Record(long inputSequence, ulong viewId, DorotiScrollTracePhase phase, string? detail = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(inputSequence);
        lock (_gate) Add(inputSequence, viewId, phase, detail);
    }

    public IReadOnlyList<DorotiScrollTraceEntry> Snapshot(long? inputSequence = null)
    {
        lock (_gate)
        {
            return inputSequence is null
                ? _entries.ToArray()
                : _entries.Where(entry => entry.InputSequence == inputSequence.Value).ToArray();
        }
    }

    private void Add(long inputSequence, ulong viewId, DorotiScrollTracePhase phase, string? detail)
    {
        var timestampMicroseconds = Math.Max(_lastTimestampMicroseconds, DorotiFrameClock.Now.Ticks / 10);
        _lastTimestampMicroseconds = timestampMicroseconds;
        _entries.Enqueue(new(++_nextSequence, inputSequence, timestampMicroseconds, phase, viewId, detail));
        while (_entries.Count > Capacity) _entries.Dequeue();
    }
}
