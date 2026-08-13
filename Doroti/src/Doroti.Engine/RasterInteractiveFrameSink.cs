using Doroti.Composition;
using Doroti.Rendering;

namespace Doroti.Engine;

/// <summary>Commits interactive widget frames to a selected Doroti render surface.</summary>
public sealed class RasterInteractiveFrameSink : IAsyncInteractiveFrameSink, IInteractiveImageHost, IDisposable
{
    private readonly ResourceRegistry _resources = new();
    private readonly ImageCache _images;
    private readonly SceneCommitter _committer;
    private readonly SurfaceSession _surface;
    private readonly RasterCompositor _compositor;
    private bool _disposed;

    public RasterInteractiveFrameSink(IRenderSurface surface, FrameTraceRecorder? trace = null)
    {
        ArgumentNullException.ThrowIfNull(surface);
        FrameTrace = trace ?? new();
        _images = new(_resources);
        _committer = new(_resources);
        _surface = new(surface);
        _compositor = new(_surface, FrameTrace);
    }

    public FrameTraceRecorder FrameTrace { get; }

    public IReadOnlyList<FrameTiming> Timings => _compositor.Timings;

    public int QueueDepth => _compositor.QueueDepth;

    public int QueueHighWatermark => _compositor.QueueHighWatermark;

    public int SupersededFrameCount => _compositor.SupersededFrameCount;

    public SurfaceGeneration SurfaceGeneration => _surface.Generation;

    public int RecoveryCount => _surface.RecoveryCount;

    public ImageCache Images => _images;

    public Task<FramePixelReadback> CaptureNextFrameAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _compositor.CaptureNextFrameAsync();
    }

    public FrameAckStatus Present(FrameId frameId, RenderPipelineFrame frame)
        => PresentAsync(frameId, frame).AsTask().GetAwaiter().GetResult().Status;

    public ValueTask<FrameAckResult> PresentAsync(
        FrameId frameId,
        RenderPipelineFrame frame,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(frame);
        cancellationToken.ThrowIfCancellationRequested();
        var scene = _committer.Commit(
            frameId,
            _surface.Generation,
            frame.Configuration.SurfaceGeneration,
            frame.Configuration.PixelSize,
            frame.RootLayer);
        var submission = _compositor.SubmitAsync(scene, cancellationToken);
        return AwaitAckAsync(scene, submission);
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

    private static async ValueTask<FrameAckResult> AwaitAckAsync(CommittedScene scene, ValueTask submission)
    {
        try
        {
            await submission.ConfigureAwait(false);
            return await scene.Ack.Completion.ConfigureAwait(false);
        }
        finally
        {
            scene.Dispose();
        }
    }
}
