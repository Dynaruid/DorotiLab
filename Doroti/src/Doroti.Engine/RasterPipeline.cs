using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Rendering;

namespace Doroti.Engine;

public sealed class SurfaceSession : IDisposable
{
    private readonly object _gate = new();
    private readonly Func<IRenderSurface>? _recreate;
    private IRenderSurface _surface;
    private readonly Func<IRenderSurface>? _create;
    private SurfaceGeneration _physicalGeneration;
    private bool _disposed;

    public SurfaceSession(IRenderSurface surface, Func<IRenderSurface>? recreate = null)
    {
        _surface = surface ?? throw new ArgumentNullException(nameof(surface));
        _recreate = recreate;
        _physicalGeneration = surface.Generation;
        Generation = surface.Generation;
        SurfaceCreatedThreadId = Environment.CurrentManagedThreadId;
    }

    public SurfaceSession(Func<IRenderSurface> create)
    {
        _create = create ?? throw new ArgumentNullException(nameof(create));
        _recreate = create;
        _surface = null!;
        Generation = new(1);
    }

    public SurfaceGeneration Generation { get; private set; }

    public int RecoveryCount { get; private set; }

    public int SurfaceCreatedThreadId { get; private set; }

    public int SurfaceDisposedThreadId { get; private set; }

    public bool TryBeginFrame(
        SurfaceGeneration expectedGeneration,
        out ISurfaceFrame? frame,
        out string? diagnostic)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            EnsureSurface();
            frame = _surface.BeginFrame();
            if (frame.Generation != _physicalGeneration)
            {
                _physicalGeneration = frame.Generation;
                Generation = Generation.Next();
                frame.Dispose();
                frame = null;
                diagnostic = "The surface changed generation before rasterization.";
                return false;
            }
            if (expectedGeneration != Generation)
            {
                frame.Dispose();
                frame = null;
                diagnostic = $"Frame generation {expectedGeneration.Value} is stale; active generation is {Generation.Value}.";
                return false;
            }
            diagnostic = null;
            return true;
        }
    }

    public bool TryRecover(Exception failure, out string diagnostic)
    {
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            EnsureSurface();
            if (_surface.Generation != _physicalGeneration)
            {
                _physicalGeneration = _surface.Generation;
                Generation = Generation.Next();
                RecoveryCount++;
                diagnostic = $"Surface recovered itself at generation {Generation.Value} after {failure.GetType().Name}.";
                return true;
            }
            if (_recreate is null)
            {
                diagnostic = $"Surface recovery is unavailable after {failure.GetType().Name}: {failure.Message}";
                return false;
            }
            try
            {
                _surface.Dispose();
                _surface = _recreate();
                _physicalGeneration = _surface.Generation;
                Generation = Generation.Next();
                RecoveryCount++;
                diagnostic = $"Surface recovered at generation {Generation.Value} after {failure.GetType().Name}.";
                return true;
            }
            catch (Exception recoveryFailure)
            {
                diagnostic = $"Surface recovery failed: {recoveryFailure.GetType().Name}: {recoveryFailure.Message}";
                return false;
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (_surface is not null)
            {
                _surface.Dispose();
                SurfaceDisposedThreadId = Environment.CurrentManagedThreadId;
            }
        }
    }

    private void EnsureSurface()
    {
        if (_surface is not null)
        {
            return;
        }
        _surface = _create!();
        _physicalGeneration = _surface.Generation;
        SurfaceCreatedThreadId = Environment.CurrentManagedThreadId;
    }
}

public sealed record FrameTiming(
    FrameId FrameId,
    TimeSpan Commit,
    TimeSpan Preroll,
    TimeSpan Raster,
    TimeSpan Present,
    long AllocatedBytes,
    int DisplayListBytes);

public sealed record FramePixelReadback(FrameId FrameId, Doroti.Graphics.Size PixelSize, int RowBytes, byte[] Bgra8888Pixels);

public sealed class RasterCompositor : ICompositor, IAsyncDisposable
{
    private readonly SurfaceSession _surfaceSession;
    private readonly FrameMailbox<CommittedScene> _mailbox;
    private readonly FrameTraceRecorder _trace;
    private readonly Task _rasterLoop;
    private readonly RasterCache _cache = new();
    private readonly List<FrameTiming> _timings = [];
    private readonly object _readbackGate = new();
    private TaskCompletionSource<FramePixelReadback>? _pendingReadback;
    private int _stopping;

    public RasterCompositor(SurfaceSession surfaceSession, FrameTraceRecorder? trace = null)
    {
        _surfaceSession = surfaceSession ?? throw new ArgumentNullException(nameof(surfaceSession));
        _trace = trace ?? new();
        _mailbox = new(CompleteSceneFromMailbox);
        _rasterLoop = Task.Factory.StartNew(
            RasterLoop,
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    public int QueueDepth => _mailbox.Depth;

    public int QueueHighWatermark => _mailbox.HighWatermark;

    public int SupersededFrameCount => _mailbox.SupersededCount;

    public int RasterThreadId { get; private set; }

    public IReadOnlyList<FrameTiming> Timings
    {
        get
        {
            lock (_timings)
            {
                return _timings.ToArray();
            }
        }
    }

    public Task<FramePixelReadback> CaptureNextFrameAsync()
    {
        lock (_readbackGate)
        {
            if (Volatile.Read(ref _stopping) != 0)
            {
                return Task.FromException<FramePixelReadback>(new InvalidOperationException("Compositor is shutting down."));
            }
            if (_pendingReadback is not null)
            {
                return Task.FromException<FramePixelReadback>(new InvalidOperationException("A frame readback is already pending."));
            }
            _pendingReadback = new(TaskCreationOptions.RunContinuationsAsynchronously);
            return _pendingReadback.Task;
        }
    }

    public ValueTask SubmitAsync(ICommittedScene scene, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(scene);
        cancellationToken.ThrowIfCancellationRequested();
        if (scene is not CommittedScene committed)
        {
            throw new ArgumentException("RasterCompositor accepts only scenes created by SceneCommitter.", nameof(scene));
        }
        _trace.Record("commit", committed, _mailbox.Depth, committed.Snapshot.Resources);
        _trace.Record("resource-retain", committed, _mailbox.Depth, committed.Snapshot.Resources);
        if (Volatile.Read(ref _stopping) != 0)
        {
            _trace.Record("enqueue", committed, _mailbox.Depth, committed.Snapshot.Resources);
            CompleteScene(
                committed,
                FrameAckStatus.Cancelled,
                "Compositor is shutting down.",
                FrameFaultKind.Cancelled);
            return ValueTask.FromException(new InvalidOperationException("Compositor is shutting down."));
        }
        _mailbox.TrySubmit(committed);
        _trace.Record("enqueue", committed, _mailbox.Depth, committed.Snapshot.Resources);
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _stopping, 1) != 0)
        {
            await _rasterLoop.ConfigureAwait(false);
            return;
        }
        _trace.Record("shutdown-begin", null, _mailbox.Depth, []);
        _mailbox.Shutdown();
        await _rasterLoop.ConfigureAwait(false);
        _mailbox.Dispose();
        CancelPendingReadback();
        _trace.Record("shutdown-complete", null, 0, []);
    }

    private void RasterLoop()
    {
        RasterThreadId = Environment.CurrentManagedThreadId;
        _cache.BindToCurrentThread();
        try
        {
            while (true)
            {
                var scene = _mailbox.Take();
                if (scene is null)
                {
                    return;
                }
                _trace.Record("dequeue", scene, _mailbox.Depth, scene.Snapshot.Resources);
                try
                {
                    Raster(scene);
                }
                finally
                {
                    _mailbox.CompleteInFlight();
                }
            }
        }
        finally
        {
            _surfaceSession.Dispose();
        }
    }

    private void Raster(CommittedScene scene)
    {
        if (scene.SurfaceGeneration != _surfaceSession.Generation)
        {
            CompleteScene(scene, FrameAckStatus.Stale, "Frame generation was stale before BeginFrame.", FrameFaultKind.Stale);
            return;
        }

        ISurfaceFrame? frame = null;
        var stage = RasterStage.BeginFrame;
        try
        {
            if (!_surfaceSession.TryBeginFrame(scene.SurfaceGeneration, out frame, out var staleDiagnostic))
            {
                CompleteScene(scene, FrameAckStatus.Stale, staleDiagnostic, FrameFaultKind.Stale);
                return;
            }
            var activeFrame = frame ?? throw new InvalidOperationException("Surface session returned no active frame.");
            if (scene.ExpectedPixelSize != Size.Zero && activeFrame.PixelSize != scene.ExpectedPixelSize)
            {
                CompleteScene(
                    scene,
                    FrameAckStatus.Stale,
                    $"Frame pixel extent {scene.ExpectedPixelSize.Width}x{scene.ExpectedPixelSize.Height} is stale; active extent is {activeFrame.PixelSize.Width}x{activeFrame.PixelSize.Height}.",
                    FrameFaultKind.Stale);
                return;
            }
            if (scene.MetricsGeneration != 0 &&
                (activeFrame is not IMetricsBoundSurfaceFrame metricsFrame || metricsFrame.MetricsGeneration != scene.MetricsGeneration))
            {
                var activeGeneration = activeFrame is IMetricsBoundSurfaceFrame bound ? bound.MetricsGeneration : 0;
                CompleteScene(
                    scene,
                    FrameAckStatus.Stale,
                    $"Frame metrics generation {scene.MetricsGeneration} is stale; active metrics generation is {activeGeneration}.",
                    FrameFaultKind.Stale);
                return;
            }

            var allocationStart = GC.GetAllocatedBytesForCurrentThread();
            stage = RasterStage.Preroll;
            var prerollStart = Stopwatch.GetTimestamp();
            _cache.Observe(scene.Snapshot, scene.SurfaceGeneration);
            var preroll = Stopwatch.GetElapsedTime(prerollStart);

            activeFrame.Clear(Doroti.Graphics.Color.Transparent);
            stage = RasterStage.Raster;
            _trace.Record("raster", scene, _mailbox.Depth, scene.Snapshot.Resources);
            var rasterStart = Stopwatch.GetTimestamp();
            scene.Snapshot.Rasterize(activeFrame.Canvas, scene.Resources);
            var raster = Stopwatch.GetElapsedTime(rasterStart);
            CompleteReadback(scene.FrameId, activeFrame);

            stage = RasterStage.Present;
            var presentStart = Stopwatch.GetTimestamp();
            activeFrame.Present();
            _trace.Record("present", scene, _mailbox.Depth, scene.Snapshot.Resources);
            var present = Stopwatch.GetElapsedTime(presentStart);
            var allocated = GC.GetAllocatedBytesForCurrentThread() - allocationStart;
            lock (_timings)
            {
                _timings.Add(new(scene.FrameId, scene.CommitDuration, preroll, raster, present, allocated, scene.Snapshot.DisplayListBytes));
            }
            CompleteScene(scene, FrameAckStatus.Presented, null, null);
        }
        catch (SurfaceStaleFrameException exception)
        {
            CompleteScene(scene, FrameAckStatus.Stale, exception.Message, FrameFaultKind.Stale);
        }
        catch (Exception exception)
        {
            if (stage is RasterStage.Raster or RasterStage.Preroll && exception is not SurfaceDeviceLostException)
            {
                CompleteScene(
                    scene,
                    FrameAckStatus.Failed,
                    $"Render programming fault during {stage}: {exception.GetType().Name}: {exception.Message}",
                    FrameFaultKind.Programming);
            }
            else
            {
                var recovered = _surfaceSession.TryRecover(exception, out var recoveryDiagnostic);
                var diagnostic = recovered
                    ? $"Surface failure during {stage}; the surface recovered. {recoveryDiagnostic}"
                    : $"Backend failure during {stage}; rendering terminated for this surface. {recoveryDiagnostic}";
                CompleteScene(
                    scene,
                    FrameAckStatus.Failed,
                    diagnostic,
                    recovered ? FrameFaultKind.RecoverableSurfaceLoss : FrameFaultKind.FatalBackend);
            }
        }
        finally
        {
            frame?.Dispose();
        }
    }

    private void CompleteReadback(FrameId frameId, ISurfaceFrame frame)
    {
        TaskCompletionSource<FramePixelReadback>? pending;
        lock (_readbackGate)
        {
            pending = _pendingReadback;
            _pendingReadback = null;
        }
        if (pending is null)
        {
            return;
        }
        if (frame is not IPixelReadableSurfaceFrame readable)
        {
            pending.TrySetException(new NotSupportedException("The active render surface does not expose pixel readback."));
            return;
        }
        try
        {
            var width = checked((int)frame.PixelSize.Width);
            var height = checked((int)frame.PixelSize.Height);
            var rowBytes = checked(width * 4);
            var pixels = GC.AllocateUninitializedArray<byte>(checked(rowBytes * height));
            if (!readable.TryReadPixels(pixels, rowBytes))
            {
                pending.TrySetException(new InvalidOperationException("The active render surface rejected pixel readback."));
                return;
            }
            pending.TrySetResult(new(frameId, frame.PixelSize, rowBytes, pixels));
        }
        catch (Exception exception)
        {
            pending.TrySetException(exception);
        }
    }

    private void CancelPendingReadback()
    {
        TaskCompletionSource<FramePixelReadback>? pending;
        lock (_readbackGate)
        {
            pending = _pendingReadback;
            _pendingReadback = null;
        }
        pending?.TrySetCanceled();
    }

    private bool CompleteSceneFromMailbox(CommittedScene scene, FrameAckStatus status, string? diagnostic) =>
        CompleteScene(scene, status, diagnostic, status switch
        {
            FrameAckStatus.Superseded => FrameFaultKind.Superseded,
            FrameAckStatus.Cancelled => FrameFaultKind.Cancelled,
            FrameAckStatus.Stale => FrameFaultKind.Stale,
            _ => null,
        });

    private bool CompleteScene(
        CommittedScene scene,
        FrameAckStatus status,
        string? diagnostic,
        FrameFaultKind? faultKind)
    {
        var completed = scene.Complete(status, diagnostic, faultKind);
        if (completed)
        {
            _trace.Record("resource-release", scene, _mailbox.Depth, scene.Snapshot.Resources, status, diagnostic, faultKind);
            _trace.Record("ack", scene, _mailbox.Depth, scene.Snapshot.Resources, status, diagnostic, faultKind);
        }
        return completed;
    }

    private enum RasterStage
    {
        BeginFrame,
        Preroll,
        Raster,
        Present,
    }
}

internal sealed class RasterCache
{
    private readonly HashSet<int> _keys = [];
    private int _threadId;
    private SurfaceGeneration _generation;

    internal int Hits { get; private set; }

    internal int Misses { get; private set; }

    internal void BindToCurrentThread() => _threadId = Environment.CurrentManagedThreadId;

    internal void Observe(LayerTreeSnapshot snapshot, SurfaceGeneration generation)
    {
        AssertThread();
        if (_generation != generation)
        {
            _keys.Clear();
            _generation = generation;
        }
        if (_keys.Add(RuntimeHelpers.GetHashCode(snapshot)))
        {
            Misses++;
        }
        else
        {
            Hits++;
        }
    }

    private void AssertThread()
    {
        if (_threadId != Environment.CurrentManagedThreadId)
        {
            throw new InvalidOperationException("Raster cache was accessed outside its raster thread.");
        }
    }
}

public sealed record FrameTraceEvent(
    long Sequence,
    string Kind,
    long TimestampTicks,
    int ThreadId,
    ulong? FrameId,
    ulong? SurfaceGeneration,
    int QueueDepth,
    ulong[] Resources,
    FrameAckStatus? AckStatus,
    string? Diagnostic,
    FrameFaultKind? FaultKind);

public sealed record FrameTraceDocument(string SchemaVersion, FrameTraceEnvironment Environment, FrameTraceEvent[] Events);

public sealed record FrameTraceEnvironment(string OperatingSystem, string Framework, int ProcessorCount);

public sealed class FrameTraceRecorder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };
    private readonly object _gate = new();
    private readonly List<FrameTraceEvent> _events = [];
    private long _sequence;

    public IReadOnlyList<FrameTraceEvent> Events
    {
        get
        {
            lock (_gate)
            {
                return _events.ToArray();
            }
        }
    }

    internal void Record(
        string kind,
        CommittedScene? scene,
        int queueDepth,
        IEnumerable<ResourceId> resources,
        FrameAckStatus? status = null,
        string? diagnostic = null,
        FrameFaultKind? faultKind = null)
    {
        lock (_gate)
        {
            _events.Add(new(
                ++_sequence,
                kind,
                Stopwatch.GetTimestamp(),
                Environment.CurrentManagedThreadId,
                scene?.FrameId.Value,
                scene?.SurfaceGeneration.Value,
                queueDepth,
                resources.Select(resource => resource.Value).ToArray(),
                status,
                diagnostic,
                faultKind));
        }
    }

    public void Write(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var document = new FrameTraceDocument(
            "doroti.frame-trace/v1",
            new(Environment.OSVersion.VersionString, Environment.Version.ToString(), Environment.ProcessorCount),
            Events.ToArray());
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(document, JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n");
    }

    public static FrameTraceDocument Replay(string path)
    {
        var document = JsonSerializer.Deserialize<FrameTraceDocument>(File.ReadAllText(path), JsonOptions)
            ?? throw new InvalidDataException("Trace document is empty.");
        if (document.SchemaVersion != "doroti.frame-trace/v1")
        {
            throw new InvalidDataException($"Unsupported trace schema {document.SchemaVersion}.");
        }
        var sequences = document.Events.Select(item => item.Sequence).ToArray();
        if (!sequences.SequenceEqual(Enumerable.Range(1, sequences.Length).Select(value => (long)value)))
        {
            throw new InvalidDataException("Trace sequence is not contiguous.");
        }
        foreach (var frame in document.Events.Where(item => item.FrameId.HasValue).GroupBy(item => item.FrameId))
        {
            if (frame.Count(item => item.Kind == "ack") != 1)
            {
                throw new InvalidDataException($"Frame {frame.Key} does not have exactly one terminal ACK.");
            }
            if (frame.Count(item => item.Kind == "resource-retain") != 1 || frame.Count(item => item.Kind == "resource-release") != 1)
            {
                throw new InvalidDataException($"Frame {frame.Key} does not have exactly one retain/release pair.");
            }
            var retained = frame.Single(item => item.Kind == "resource-retain").Resources.Order().ToArray();
            var released = frame.Single(item => item.Kind == "resource-release").Resources.Order().ToArray();
            if (!retained.SequenceEqual(released))
            {
                throw new InvalidDataException($"Frame {frame.Key} retained and released different resources.");
            }
            var kinds = frame.Select(item => item.Kind).ToArray();
            RequireOrder(frame.Key, kinds, "commit", "resource-retain", "enqueue");
            RequireOrder(frame.Key, kinds, "resource-release", "ack");
            var ack = frame.Single(item => item.Kind == "ack");
            if (ack.AckStatus is FrameAckStatus.Presented)
            {
                RequireOrder(frame.Key, kinds, "dequeue", "raster", "present", "resource-release", "ack");
            }
        }
        return document;
    }

    private static void RequireOrder(ulong? frameId, IReadOnlyList<string> kinds, params string[] expected)
    {
        var previous = -1;
        foreach (var kind in expected)
        {
            var index = -1;
            for (var candidate = previous + 1; candidate < kinds.Count; candidate++)
            {
                if (kinds[candidate] == kind)
                {
                    index = candidate;
                    break;
                }
            }
            if (index < 0)
            {
                throw new InvalidDataException($"Frame {frameId} is missing ordered transition {kind}.");
            }
            previous = index;
        }
    }
}
