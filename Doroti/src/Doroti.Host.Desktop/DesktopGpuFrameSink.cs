using Doroti.Backends.Skia;
using Doroti.Composition;
using Doroti.Engine;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;
using Doroti.Shell.Core;

namespace Doroti.Host.Desktop;

public readonly record struct DesktopGpuResourceSnapshot(
    int ActiveContexts,
    long ContextsCreated,
    long ContextsReleased,
    int ActiveFrames,
    long FramesCreated,
    long FramesReleased);

/// <summary>
/// A2 desktop composition root: strict direct GPU selection plus asynchronous engine mailbox.
/// Backend and vendor renderer types stay inside the host implementation.
/// </summary>
public sealed class DesktopGpuFrameSink : IAsyncInteractiveFrameSink, IInteractiveImageHost, IDisposable
{
    private readonly SurfaceCreationResult _selection;
    private readonly RasterInteractiveFrameSink _inner;
    private readonly string _backendIdentity;
    private bool _disposed;

    public DesktopGpuFrameSink(IWindow window, FrameTraceRecorder? trace = null)
    {
        ArgumentNullException.ThrowIfNull(window);
        _selection = SkiaSurfaceFactory.CreateHardware(window);
        _inner = new(_selection.Surface, trace);
        _backendIdentity = window.TryGetFeature<IShellGraphicsService>(out var graphics) && graphics is not null
            ? graphics.BackendIdentity
            : "skia-opengl-gpu";
    }

    public ImageCache Images => _inner.Images;

    public string BackendIdentity => _backendIdentity;

    public string Diagnostic => _selection.Diagnostic;

    public bool SoftwareFallbackUsed => _selection.Backend is not SurfaceBackendKind.OpenGlGpu;

    public int QueueDepth => _inner.QueueDepth;

    public int QueueHighWatermark => _inner.QueueHighWatermark;

    public int SupersededFrameCount => _inner.SupersededFrameCount;

    public SurfaceGeneration SurfaceGeneration => _inner.SurfaceGeneration;

    public int RecoveryCount => _inner.RecoveryCount;

    public IReadOnlyList<FrameTiming> Timings => _inner.Timings;

    public DesktopGpuResourceSnapshot Resources
    {
        get
        {
            var value = _selection.ResourceDiagnostics.Snapshot;
            return new(
                value.ActiveSkiaContexts,
                value.SkiaContextsCreated,
                value.SkiaContextsReleased,
                value.ActiveFrames,
                value.FramesCreated,
                value.FramesReleased);
        }
    }

    public Task<FramePixelReadback> CaptureNextFrameAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _inner.CaptureNextFrameAsync();
    }

    public FrameAckStatus Present(FrameId frameId, RenderPipelineFrame frame) => _inner.Present(frameId, frame);

    public ValueTask<FrameAckResult> PresentAsync(
        FrameId frameId,
        RenderPipelineFrame frame,
        CancellationToken cancellationToken = default) =>
        _inner.PresentAsync(frameId, frame, cancellationToken);

    public void FailNextFrameForValidation()
    {
        if (_selection.Surface is not IGpuSurfaceRecoveryDiagnostics recovery)
        {
            throw new NotSupportedException("The selected GPU surface has no recovery diagnostic hook.");
        }
        recovery.FailNextFrame();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _inner.Dispose();
    }

    /// <summary>Runs a current-machine GPU/software readback comparison without promoting software to product fallback.</summary>
    public static double MeasureVisualTolerance(IWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        return Task.Factory.StartNew(() =>
        {
            var hardware = SkiaSurfaceFactory.CreateHardware(window);
            var software = SkiaSurfaceFactory.Create(window, SurfaceBackendPreference.SkiaSoftware);
            try
            {
                var gpu = RenderProbe(hardware.Surface);
                var cpu = RenderProbe(software.Surface);
                if (gpu.Length != cpu.Length)
                {
                    throw new InvalidDataException("A2 GPU/software visual probes produced different buffer lengths.");
                }
                return gpu.Zip(cpu, static (left, right) => Math.Abs(left - right)).Average();
            }
            finally
            {
                hardware.Surface.Dispose();
                software.Surface.Dispose();
            }
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).GetAwaiter().GetResult();
    }

    private static byte[] RenderProbe(IRenderSurface surface)
    {
        using var frame = surface.BeginFrame();
        frame.Clear(Color.FromArgb(255, 13, 21, 38));
        frame.Canvas.DrawRect(new(18, 16, 180, 90), new(Color.FromArgb(255, 38, 100, 139)));
        frame.Canvas.DrawText("Doroti A2 한글 ffi", new(24, 62), 20, new(Color.FromArgb(255, 255, 255, 255)));
        var width = checked((int)frame.PixelSize.Width);
        var height = checked((int)frame.PixelSize.Height);
        var pixels = new byte[checked(width * height * 4)];
        if (frame is not IPixelReadableSurfaceFrame readable || !readable.TryReadPixels(pixels, width * 4))
        {
            throw new NotSupportedException("A2 visual probe requires GPU and software readback.");
        }
        frame.Present();
        return pixels;
    }
}
