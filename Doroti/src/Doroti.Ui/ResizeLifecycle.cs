using System.Text.Json.Serialization;

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
    double DeviceScaleX,
    double DeviceScaleY,
    long TimestampMicroseconds)
{
    [JsonConstructor]
    public DorotiResizeEpoch(
        long generation,
        double logicalWidth,
        double logicalHeight,
        int physicalWidth,
        int physicalHeight,
        double devicePixelRatio,
        long timestampMicroseconds)
        : this(generation, logicalWidth, logicalHeight, physicalWidth, physicalHeight,
            devicePixelRatio, devicePixelRatio, timestampMicroseconds) { }

    public double DevicePixelRatio => DeviceScaleX;
    public bool HasUniformDeviceScale =>
        Math.Abs(DeviceScaleX - DeviceScaleY) <= double.Epsilon;

    public bool HasDrawableSize =>
        LogicalWidth > 0 && LogicalHeight > 0 && PhysicalWidth > 0 && PhysicalHeight > 0;
}

/// <summary>
/// One immutable host viewport publication. Framework metrics and the native
/// resize target must be copied from the same publication instead of sampled
/// independently while a frame is being built.
/// </summary>
public sealed record DorotiViewEpoch(
    ulong ViewId,
    long ResizeTargetGeneration,
    long MetricsGeneration,
    double LogicalWidth,
    double LogicalHeight,
    int PhysicalWidth,
    int PhysicalHeight,
    double DeviceScaleX,
    double DeviceScaleY,
    long TimestampMicroseconds)
{
    public double DevicePixelRatio => DeviceScaleX;
    public bool HasUniformDeviceScale =>
        Math.Abs(DeviceScaleX - DeviceScaleY) <= double.Epsilon;
    public bool HasDrawableSize =>
        LogicalWidth > 0 && LogicalHeight > 0 && PhysicalWidth > 0 && PhysicalHeight > 0;
}

/// <summary>Identity captured once at the beginning of a framework frame.</summary>
public sealed record DorotiSceneBuildToken(
    DorotiViewEpoch ViewEpoch,
    long FrameworkFrameNumber,
    int RootPhysicalWidth,
    int RootPhysicalHeight)
{
    public bool HasRootPhysicalSize => RootPhysicalWidth > 0 && RootPhysicalHeight > 0;

    public DorotiSceneBuildToken WithRootPhysicalSize(int width, int height) =>
        this with { RootPhysicalWidth = width, RootPhysicalHeight = height };
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
    double DeviceScaleX,
    double DeviceScaleY,
    int RootPhysicalWidth,
    int RootPhysicalHeight,
    long FrameworkFrameNumber,
    long SceneSequence)
{
    public double DevicePixelRatio => DeviceScaleX;
    public bool HasDrawableSize =>
        LogicalWidth > 0 && LogicalHeight > 0 && PhysicalWidth > 0 && PhysicalHeight > 0;

    public static DorotiFrameDescriptor FromBuildToken(
        DorotiSceneBuildToken token,
        long sceneSequence)
    {
        ArgumentNullException.ThrowIfNull(token);
        var epoch = token.ViewEpoch;
        return new(
            epoch.ViewId,
            epoch.ResizeTargetGeneration,
            epoch.MetricsGeneration,
            epoch.LogicalWidth,
            epoch.LogicalHeight,
            epoch.PhysicalWidth,
            epoch.PhysicalHeight,
            epoch.DeviceScaleX,
            epoch.DeviceScaleY,
            token.RootPhysicalWidth,
            token.RootPhysicalHeight,
            token.FrameworkFrameNumber,
            sceneSequence);
    }

    public int CompareAdmissionTo(DorotiFrameDescriptor other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var target = ResizeTargetGeneration.CompareTo(other.ResizeTargetGeneration);
        if (target != 0) return target;
        var metrics = MetricsGeneration.CompareTo(other.MetricsGeneration);
        if (metrics != 0) return metrics;
        return SceneSequence.CompareTo(other.SceneSequence);
    }

    public DorotiFrameMatchResult MatchExact(
        DorotiViewEpoch current,
        DorotiResizeEpoch target,
        int surfaceWidth,
        int surfaceHeight,
        double surfaceScaleX,
        double surfaceScaleY)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(target);

        if (!current.HasUniformDeviceScale || !target.HasUniformDeviceScale ||
            Math.Abs(surfaceScaleX - surfaceScaleY) > double.Epsilon)
            return DorotiFrameMatchResult.Mismatch(
                DorotiFrameMismatch.nonUniformDeviceScale,
                $"scene={DeviceScaleX}x{DeviceScaleY}; current={current.DeviceScaleX}x{current.DeviceScaleY}; surface={surfaceScaleX}x{surfaceScaleY}");
        if (ViewId != current.ViewId)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.viewId,
                $"scene={ViewId}; current={current.ViewId}");
        if (ResizeTargetGeneration != current.ResizeTargetGeneration ||
            ResizeTargetGeneration != target.Generation)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.resizeTargetGeneration,
                $"scene={ResizeTargetGeneration}; current={current.ResizeTargetGeneration}; target={target.Generation}");
        if (MetricsGeneration != current.MetricsGeneration)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.metricsGeneration,
                $"scene={MetricsGeneration}; current={current.MetricsGeneration}");
        if (LogicalWidth != current.LogicalWidth || LogicalHeight != current.LogicalHeight ||
            LogicalWidth != target.LogicalWidth || LogicalHeight != target.LogicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.logicalSize,
                $"scene={LogicalWidth}x{LogicalHeight}; current={current.LogicalWidth}x{current.LogicalHeight}; target={target.LogicalWidth}x{target.LogicalHeight}");
        if (PhysicalWidth != current.PhysicalWidth || PhysicalHeight != current.PhysicalHeight ||
            PhysicalWidth != target.PhysicalWidth || PhysicalHeight != target.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.physicalSize,
                $"scene={PhysicalWidth}x{PhysicalHeight}; current={current.PhysicalWidth}x{current.PhysicalHeight}; target={target.PhysicalWidth}x{target.PhysicalHeight}");
        if (DeviceScaleX != current.DeviceScaleX || DeviceScaleY != current.DeviceScaleY ||
            DeviceScaleX != target.DeviceScaleX || DeviceScaleY != target.DeviceScaleY)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.deviceScale,
                $"scene={DeviceScaleX}x{DeviceScaleY}; current={current.DeviceScaleX}x{current.DeviceScaleY}; target={target.DeviceScaleX}x{target.DeviceScaleY}");
        if (RootPhysicalWidth != current.PhysicalWidth || RootPhysicalHeight != current.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.rootPhysicalSize,
                $"root={RootPhysicalWidth}x{RootPhysicalHeight}; current={current.PhysicalWidth}x{current.PhysicalHeight}");
        if (surfaceWidth != target.PhysicalWidth || surfaceHeight != target.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.surfacePhysicalSize,
                $"surface={surfaceWidth}x{surfaceHeight}; target={target.PhysicalWidth}x{target.PhysicalHeight}");
        if (surfaceScaleX != target.DeviceScaleX || surfaceScaleY != target.DeviceScaleY)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.surfaceDeviceScale,
                $"surface={surfaceScaleX}x{surfaceScaleY}; target={target.DeviceScaleX}x{target.DeviceScaleY}");
        return DorotiFrameMatchResult.Exact;
    }

    /// <summary>
    /// Native final gate after the renderer has already matched the framework
    /// metrics generation. This intentionally does not invent a metrics value
    /// at the surface boundary.
    /// </summary>
    public DorotiFrameMatchResult MatchTargetAndSurface(
        DorotiResizeEpoch target,
        int surfaceWidth,
        int surfaceHeight,
        double surfaceScaleX,
        double surfaceScaleY)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (!target.HasUniformDeviceScale ||
            Math.Abs(surfaceScaleX - surfaceScaleY) > double.Epsilon)
            return DorotiFrameMatchResult.Mismatch(
                DorotiFrameMismatch.nonUniformDeviceScale,
                $"scene={DeviceScaleX}x{DeviceScaleY}; target={target.DeviceScaleX}x{target.DeviceScaleY}; surface={surfaceScaleX}x{surfaceScaleY}");
        if (ResizeTargetGeneration != target.Generation)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.resizeTargetGeneration,
                $"scene={ResizeTargetGeneration}; target={target.Generation}");
        if (LogicalWidth != target.LogicalWidth || LogicalHeight != target.LogicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.logicalSize,
                $"scene={LogicalWidth}x{LogicalHeight}; target={target.LogicalWidth}x{target.LogicalHeight}");
        if (PhysicalWidth != target.PhysicalWidth || PhysicalHeight != target.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.physicalSize,
                $"scene={PhysicalWidth}x{PhysicalHeight}; target={target.PhysicalWidth}x{target.PhysicalHeight}");
        if (DeviceScaleX != target.DeviceScaleX || DeviceScaleY != target.DeviceScaleY)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.deviceScale,
                $"scene={DeviceScaleX}x{DeviceScaleY}; target={target.DeviceScaleX}x{target.DeviceScaleY}");
        if (RootPhysicalWidth != target.PhysicalWidth || RootPhysicalHeight != target.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.rootPhysicalSize,
                $"root={RootPhysicalWidth}x{RootPhysicalHeight}; target={target.PhysicalWidth}x{target.PhysicalHeight}");
        if (surfaceWidth != target.PhysicalWidth || surfaceHeight != target.PhysicalHeight)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.surfacePhysicalSize,
                $"surface={surfaceWidth}x{surfaceHeight}; target={target.PhysicalWidth}x{target.PhysicalHeight}");
        if (surfaceScaleX != target.DeviceScaleX || surfaceScaleY != target.DeviceScaleY)
            return DorotiFrameMatchResult.Mismatch(DorotiFrameMismatch.surfaceDeviceScale,
                $"surface={surfaceScaleX}x{surfaceScaleY}; target={target.DeviceScaleX}x{target.DeviceScaleY}");
        return DorotiFrameMatchResult.Exact;
    }
}

public enum DorotiFrameMismatch
{
    none,
    missingBuildToken,
    viewId,
    resizeTargetGeneration,
    metricsGeneration,
    logicalSize,
    physicalSize,
    deviceScale,
    rootPhysicalSize,
    surfacePhysicalSize,
    surfaceDeviceScale,
    nonUniformDeviceScale,
}

public sealed record DorotiFrameMatchResult(
    DorotiFrameMismatch MismatchCode,
    string? Detail = null)
{
    public static DorotiFrameMatchResult Exact { get; } = new(DorotiFrameMismatch.none);
    public bool IsExact => MismatchCode == DorotiFrameMismatch.none;

    public static DorotiFrameMatchResult Mismatch(DorotiFrameMismatch code, string detail)
    {
        if (code == DorotiFrameMismatch.none) throw new ArgumentOutOfRangeException(nameof(code));
        return new(code, detail);
    }
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
        => Publish(logicalWidth, logicalHeight, devicePixelRatio, devicePixelRatio,
            timestampMicroseconds);

    public DorotiResizeEpoch Publish(
        double logicalWidth,
        double logicalHeight,
        double deviceScaleX,
        double deviceScaleY,
        long? timestampMicroseconds = null)
    {
        if (!double.IsFinite(logicalWidth) || logicalWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(logicalWidth));
        if (!double.IsFinite(logicalHeight) || logicalHeight < 0)
            throw new ArgumentOutOfRangeException(nameof(logicalHeight));
        if (!double.IsFinite(deviceScaleX) || deviceScaleX <= 0)
            throw new ArgumentOutOfRangeException(nameof(deviceScaleX));
        if (!double.IsFinite(deviceScaleY) || deviceScaleY <= 0)
            throw new ArgumentOutOfRangeException(nameof(deviceScaleY));

        var physicalWidth = logicalWidth <= 0 ? 0 : checked((int)Math.Round(logicalWidth * deviceScaleX));
        var physicalHeight = logicalHeight <= 0 ? 0 : checked((int)Math.Round(logicalHeight * deviceScaleY));
        lock (_gate)
        {
            if (_latest is { } current &&
                current.LogicalWidth == logicalWidth &&
                current.LogicalHeight == logicalHeight &&
                current.PhysicalWidth == physicalWidth &&
                current.PhysicalHeight == physicalHeight &&
                current.DeviceScaleX == deviceScaleX &&
                current.DeviceScaleY == deviceScaleY)
                return current;

            _latest = new(
                checked(++_generation),
                logicalWidth,
                logicalHeight,
                physicalWidth,
                physicalHeight,
                deviceScaleX,
                deviceScaleY,
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
