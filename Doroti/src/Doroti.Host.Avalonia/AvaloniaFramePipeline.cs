using System.Diagnostics;
using Doroti.Composition;
using Doroti.Core;
using Doroti.Engine;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Host.Avalonia;

public sealed record AvaloniaFramePipelineSnapshot(
    SurfaceGeneration SurfaceGeneration,
    string SurfaceBackend,
    string SurfaceDiagnostic,
    int UiThreadId,
    int RasterThreadId,
    int SurfaceCreatedThreadId,
    int SurfaceDisposedThreadId,
    int QueueDepth,
    int QueueHighWatermark,
    int SupersededFrames,
    int RegisteredResources,
    int ActiveResourceLeases,
    AvaloniaGpuResourceSnapshot GpuResources,
    AvaloniaFrameUploadSnapshot UploadResources);

public sealed record AvaloniaGpuResourceSnapshot(
    int ActiveContexts,
    long ContextsCreated,
    long ContextsReleased,
    int ActiveFrames,
    long FramesCreated,
    long FramesReleased)
{
    public bool IsBalanced =>
        ActiveContexts == 0 &&
        ActiveFrames == 0 &&
        ContextsCreated == ContextsReleased &&
        FramesCreated == FramesReleased;
}

public sealed record AvaloniaFrameUploadSnapshot(
    int ActiveBitmaps,
    long BitmapsCreated,
    long BitmapsReleased,
    long FramesStaged,
    long FramesImported,
    long InvalidationsCoalesced,
    int StagingThreadId,
    int ImportThreadId,
    long PendingArrayAllocationBytes,
    long StagingCopyBytes,
    long BitmapUploadCopyBytes)
{
    public bool IsBalanced =>
        ActiveBitmaps == 0 &&
        BitmapsCreated == BitmapsReleased;
}

/// <summary>
/// Owns the Doroti frame mailbox, surface generation and resource leases for one Avalonia window.
/// Avalonia types remain behind the host boundary.
/// </summary>
public interface IAvaloniaFramePipeline
{
    AvaloniaFramePipelineSnapshot Snapshot { get; }

    ImageCache Images { get; }

    IReadOnlyList<FrameTiming> Timings { get; }

    ResourceId RegisterImage(int width, int height, ReadOnlySpan<byte> bgra8888Pixels);

    bool RemoveResource(ResourceId resource);

    Task<FrameAckResult> PresentAsync(DisplayList displayList, CancellationToken cancellationToken = default);

    Task<FrameAckResult> PresentAsync(Layer rootLayer, CancellationToken cancellationToken = default);

    Task<FrameAckResult> PresentAsync(FrameId frameId, Layer rootLayer, CancellationToken cancellationToken = default);

    Task<AvaloniaPixelReadback> CaptureNextFrameAsync();

    void WriteFrameTrace(string path);
}

/// <summary>Target-only controls used to prove stale, present-failure and bounded-mailbox behavior.</summary>
public interface IAvaloniaFrameTestController
{
    void PauseNextPresent();

    bool WaitForPausedPresent(TimeSpan timeout);

    void ResumePresent();

    void FailNextPresent();

    void StaleNextPresent();
}

internal sealed class AvaloniaFramePipeline : IAvaloniaFramePipeline, IDisposable
{
    private readonly ResourceRegistry _resources = new();
    private readonly ImageCache _images;
    private readonly SceneCommitter _committer;
    private readonly SurfaceSession _surface;
    private readonly RasterCompositor _compositor;
    private readonly FrameTraceRecorder _trace = new();
    private readonly AvaloniaDisplayListControl _target;
    private readonly int _uiThreadId;
    private long _nextFrameId;
    private bool _disposed;

    internal AvaloniaFramePipeline(AvaloniaDisplayListControl target)
    {
        _target = target;
        _uiThreadId = Environment.CurrentManagedThreadId;
        _images = new(_resources);
        _committer = new(_resources);
        _surface = new(() => new ManagedBgraRenderSurface(target));
        _compositor = new(_surface, _trace);
    }

    public ImageCache Images => _images;

    public IReadOnlyList<FrameTiming> Timings => _compositor.Timings;

    public AvaloniaFramePipelineSnapshot Snapshot
    {
        get
        {
            return new(
                _surface.Generation,
                "ManagedBgra8888",
                "Backend-neutral managed BGRA8888 raster; official Avalonia owns host GPU composition.",
                _uiThreadId,
                _compositor.RasterThreadId,
                _surface.SurfaceCreatedThreadId,
                _surface.SurfaceDisposedThreadId,
                _compositor.QueueDepth,
                _compositor.QueueHighWatermark,
                _compositor.SupersededFrameCount,
                _resources.RegisteredCount,
                _resources.ActiveLeaseCount,
                new(0, 0, 0, 0, 0, 0),
                _target.UploadSnapshot);
        }
    }

    public ResourceId RegisterImage(int width, int height, ReadOnlySpan<byte> bgra8888Pixels)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _resources.RegisterImage(width, height, bgra8888Pixels);
    }

    public bool RemoveResource(ResourceId resource)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _resources.Remove(resource);
    }

    public async Task<FrameAckResult> PresentAsync(DisplayList displayList, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(displayList);
        return await PresentAsync(new PictureLayer(Offset.Zero, displayList), cancellationToken).ConfigureAwait(false);
    }

    public async Task<FrameAckResult> PresentAsync(Layer rootLayer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(rootLayer);
        cancellationToken.ThrowIfCancellationRequested();
        var frameId = new FrameId(checked((ulong)Interlocked.Increment(ref _nextFrameId)));
        return await PresentAsync(frameId, rootLayer, cancellationToken).ConfigureAwait(false);
    }

    public async Task<FrameAckResult> PresentAsync(FrameId frameId, Layer rootLayer, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(rootLayer);
        cancellationToken.ThrowIfCancellationRequested();
        var observed = Volatile.Read(ref _nextFrameId);
        if (frameId.Value > checked((ulong)observed))
        {
            Interlocked.Exchange(ref _nextFrameId, checked((long)frameId.Value));
        }
        using var scene = _committer.Commit(frameId, _surface.Generation, rootLayer);
        await _compositor.SubmitAsync(scene, cancellationToken).ConfigureAwait(false);
        return await scene.Ack.Completion.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<AvaloniaPixelReadback> CaptureNextFrameAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var readback = await _compositor.CaptureNextFrameAsync().ConfigureAwait(false);
        return new(readback.PixelSize, readback.RowBytes, readback.Bgra8888Pixels);
    }

    public void WriteFrameTrace(string path)
    {
        _trace.Write(path);
        _ = FrameTraceRecorder.Replay(path);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _committer.Shutdown();
        _compositor.DisposeAsync().AsTask().GetAwaiter().GetResult();
        _images.Dispose();
        _resources.Dispose();
    }
}

/// <summary>Adapts the Doroti interactive widget pipeline to an Avalonia window's H2 frame pipeline.</summary>
public sealed class AvaloniaInteractiveFrameSink : IInteractiveFrameSink, IInteractiveImageHost
{
    private readonly IAvaloniaFramePipeline _pipeline;

    public AvaloniaInteractiveFrameSink(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (!window.TryGetFeature<IAvaloniaFramePipeline>(out var pipeline) || pipeline is null)
        {
            throw new NotSupportedException("The window does not expose the Avalonia frame pipeline.");
        }
        _pipeline = pipeline;
    }

    public ImageCache Images => _pipeline.Images;

    public long SynchronousUiWaitCount { get; private set; }

    public FrameAckStatus Present(FrameId frameId, RenderPipelineFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        SynchronousUiWaitCount++;
        return _pipeline.PresentAsync(frameId, frame.RootLayer).GetAwaiter().GetResult().Status;
    }
}

internal sealed class AvaloniaFrameDispatcher(
    AvaloniaHostDiagnostics diagnostics,
    WindowId window,
    Func<WindowMetrics> metrics) : IFrameDispatcher
{
    private readonly object _gate = new();
    private readonly Queue<Action<TimeSpan>> _callbacks = [];
    private readonly long _origin = Stopwatch.GetTimestamp();
    private bool _scheduled;

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var post = false;
        lock (_gate)
        {
            _callbacks.Enqueue(callback);
            if (!_scheduled)
            {
                _scheduled = true;
                post = true;
            }
        }
        diagnostics.Record("frame-clock-requested", window, metrics());
        if (post)
        {
            global::Avalonia.Threading.Dispatcher.UIThread.Post(Drain, global::Avalonia.Threading.DispatcherPriority.Render);
        }
    }

    private void Drain()
    {
        AvaloniaWindowBackend.RequireUiThread();
        Action<TimeSpan>[] callbacks;
        lock (_gate)
        {
            callbacks = _callbacks.ToArray();
            _callbacks.Clear();
            _scheduled = false;
        }
        var timestamp = Stopwatch.GetElapsedTime(_origin);
        diagnostics.Record("frame-clock-tick", window, metrics(), $"callbacks={callbacks.Length}");
        foreach (var callback in callbacks)
        {
            callback(timestamp);
        }
    }
}
