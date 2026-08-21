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
    long TimestampMicroseconds)
{
    public bool HasDrawableSize =>
        LogicalWidth > 0 && LogicalHeight > 0 && PhysicalWidth > 0 && PhysicalHeight > 0;
}

/// <summary>
/// Immutable identity of a framework scene.  A target generation identifies
/// an OS/DOM size request; metrics identifies the framework notification that
/// built the scene.  Surface generations are deliberately absent because a
/// surface is committed only after an exact frame has been accepted.
/// </summary>
public sealed record DorotiFrameDescriptor(
    ulong ViewId,
    long ResizeTargetGeneration,
    long MetricsGeneration,
    double LogicalWidth,
    double LogicalHeight,
    int PhysicalWidth,
    int PhysicalHeight,
    double DevicePixelRatio,
    long SceneSequence)
{
    public bool HasDrawableSize =>
        LogicalWidth > 0 && LogicalHeight > 0 && PhysicalWidth > 0 && PhysicalHeight > 0;

    public bool IsExactFor(DorotiResizeEpoch target) =>
        ResizeTargetGeneration == target.Generation &&
        PhysicalWidth == target.PhysicalWidth &&
        PhysicalHeight == target.PhysicalHeight &&
        Math.Abs(DevicePixelRatio - target.DevicePixelRatio) <= double.Epsilon;
}

public enum DorotiFrameTerminal
{
    presented,
    submitted,
    superseded,
    dropped,
    failed,
}

public enum DorotiResizePipelineState
{
    idle,
    targetPending,
    frameBuilding,
    exactFrameReady,
    surfaceCommit,
    presenting,
    presented,
}

/// <summary>
/// Platform-neutral target publisher. It owns target generation only; native
/// surface owners remain solely responsible for surface generation.
/// </summary>
public sealed class DorotiResizeTargetCoordinator
{
    private readonly object _gate = new();
    private long _generation;
    private DorotiResizeEpoch? _latest;

    public DorotiResizeEpoch? Latest
    {
        get { lock (_gate) return _latest; }
    }

    public DorotiResizeEpoch Publish(
        double logicalWidth,
        double logicalHeight,
        double devicePixelRatio,
        long? timestampMicroseconds = null)
    {
        if (!double.IsFinite(logicalWidth) || logicalWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(logicalWidth));
        if (!double.IsFinite(logicalHeight) || logicalHeight < 0)
            throw new ArgumentOutOfRangeException(nameof(logicalHeight));
        if (!double.IsFinite(devicePixelRatio) || devicePixelRatio <= 0)
            throw new ArgumentOutOfRangeException(nameof(devicePixelRatio));

        var physicalWidth = logicalWidth <= 0 ? 0 : checked((int)Math.Round(logicalWidth * devicePixelRatio));
        var physicalHeight = logicalHeight <= 0 ? 0 : checked((int)Math.Round(logicalHeight * devicePixelRatio));
        lock (_gate)
        {
            if (_latest is { } current &&
                current.LogicalWidth == logicalWidth &&
                current.LogicalHeight == logicalHeight &&
                current.PhysicalWidth == physicalWidth &&
                current.PhysicalHeight == physicalHeight &&
                current.DevicePixelRatio == devicePixelRatio)
                return current;

            _latest = new(
                checked(++_generation),
                logicalWidth,
                logicalHeight,
                physicalWidth,
                physicalHeight,
                devicePixelRatio,
                timestampMicroseconds ?? DorotiFrameClock.Now.Ticks / 10);
            return _latest;
        }
    }
}

/// <summary>
/// One-in-flight plus one-latest mailbox. Replaced pending work is completed
/// immediately as superseded so producers can never wait indefinitely.
/// </summary>
public sealed class DorotiLatestFrameMailbox<T> where T : class
{
    private readonly object _gate = new();
    private T? _current;
    private T? _latest;

    public int Depth
    {
        get { lock (_gate) return (_current is null ? 0 : 1) + (_latest is null ? 0 : 1); }
    }

    public T? Offer(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        lock (_gate)
        {
            if (_current is null)
            {
                _current = value;
                return null;
            }
            var replaced = _latest;
            _latest = value;
            return replaced;
        }
    }

    public T? Current
    {
        get { lock (_gate) return _current; }
    }

    public T? CompleteCurrent()
    {
        lock (_gate)
        {
            var completed = _current;
            _current = _latest;
            _latest = null;
            return completed;
        }
    }

    public IReadOnlyList<T> Drain()
    {
        lock (_gate)
        {
            var values = new List<T>(2);
            if (_current is not null) values.Add(_current);
            if (_latest is not null) values.Add(_latest);
            _current = null;
            _latest = null;
            return values;
        }
    }
}

/// <summary>Tracks the exactly-once terminal contract for generated frames.</summary>
public sealed class DorotiFrameTerminalLedger
{
    private readonly object _gate = new();
    private readonly HashSet<long> _registered = [];
    private readonly Dictionary<long, DorotiFrameTerminal> _terminals = [];

    public void Register(long sceneSequence)
    {
        if (sceneSequence <= 0) throw new ArgumentOutOfRangeException(nameof(sceneSequence));
        lock (_gate)
        {
            if (!_registered.Add(sceneSequence))
                throw new InvalidOperationException($"Scene {sceneSequence} was registered more than once.");
        }
    }

    public bool TryComplete(long sceneSequence, DorotiFrameTerminal terminal)
    {
        lock (_gate)
        {
            if (!_registered.Contains(sceneSequence))
                throw new InvalidOperationException($"Scene {sceneSequence} was not registered.");
            if (_terminals.ContainsKey(sceneSequence)) return false;
            _terminals.Add(sceneSequence, terminal);
            return true;
        }
    }

    public IReadOnlyDictionary<long, DorotiFrameTerminal> Snapshot()
    {
        lock (_gate) return new Dictionary<long, DorotiFrameTerminal>(_terminals);
    }

    public IReadOnlyList<long> Unterminated()
    {
        lock (_gate) return _registered.Where(sequence => !_terminals.ContainsKey(sequence)).ToArray();
    }
}

/// <summary>One platform resize event in the common Doroti trace schema.</summary>
public sealed record DorotiResizeTraceEntry(
    long Sequence,
    long TimestampMicroseconds,
    string Phase,
    DorotiResizeEpoch Epoch,
    int ThreadId,
    string Source,
    long PerformanceCounter,
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
    // Ten seconds of high-refresh resize evidence can exceed 4,096 phase
    // entries. Keep the complete diagnostic window so target counts and
    // cross-process QPC correlation are not biased toward the trace tail.
    private const int Capacity = 16384;
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
                System.Diagnostics.Stopwatch.GetTimestamp(),
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
