#if MACOS
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Metal;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>A reusable transparent raster slot. All slots submit to the owning MTKView's queue.</summary>
internal sealed class AppKitPlatformRasterSurface : NSView
{
    private readonly CAMetalLayer _metal;
    private readonly SkiaGraphiteSession? _graphite;
    private readonly GRContext? _ganesh;
    private int _leases;
    private bool _retired;

    internal AppKitPlatformRasterSurface(IMTLDevice device, IMTLCommandQueue queue, GRContext? ganesh)
    {
        _ganesh = ganesh;
        _metal = new CAMetalLayer
        {
            Device = device, PixelFormat = MTLPixelFormat.BGRA8Unorm, FramebufferOnly = false,
            Opaque = false, PresentsWithTransaction = true,
        };
        Layer = _metal;
        WantsLayer = true;
        Hidden = true;
        if (DorotiMacOSMetalView.UseGraphite)
            _graphite = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, 1);
    }

    public override bool IsOpaque => false;
    public override NSView? HitTest(CGPoint point) => null;

    internal RasterFrame Prepare(SkiaSceneRenderer renderer, PlatformRasterSegment segment, int width, int height, double scale)
    {
        ObjectDisposedException.ThrowIf(_retired, this);
        _metal.DrawableSize = new CGSize(width, height);
        _metal.ContentsScale = (System.Runtime.InteropServices.NFloat)scale;
        var drawable = _metal.NextDrawable() ?? throw new InvalidOperationException("No AppKit segment drawable available.");
        var frame = new RasterFrame(this, drawable, segment.PaintOrder);
        _leases++;
        try
        {
            if (_graphite is not null)
                frame.Graphite = _graphite.BeginMetalFrame(width, height, drawable.Texture.Handle);
            else
            {
                frame.Target = new GRBackendRenderTarget(width, height, new GRMtlTextureInfo(drawable.Texture));
                frame.Ganesh = SKSurface.Create(_ganesh, frame.Target, GRSurfaceOrigin.TopLeft, SKColorType.Bgra8888)
                    ?? throw new InvalidOperationException("Could not wrap AppKit segment drawable.");
            }
            var canvas = frame.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            renderer.DrawPlatformRasterSegment(canvas, segment.Commands, width, height);
            return frame;
        }
        catch { frame.Dispose(); throw; }
    }

    internal void Retire()
    {
        if (_retired) return;
        _retired = true;
        RemoveFromSuperview();
        _graphite?.StopAcceptingFrames();
        if (_leases == 0) Release();
    }

    private void Release() { _graphite?.Dispose(); _metal.Dispose(); Dispose(); }

    internal sealed class RasterFrame(AppKitPlatformRasterSurface slot, ICAMetalDrawable drawable, int order) : IDisposable
    {
        internal SkiaGraphiteSession.Frame? Graphite;
        internal SKSurface? Ganesh;
        internal GRBackendRenderTarget? Target;
        internal SKSurface Surface => Graphite?.Surface ?? Ganesh!;
        internal AppKitPlatformRasterSurface Slot => slot;
        internal int PaintOrder => order;
        internal bool Submitted { get; private set; }
        private bool _disposed;

        internal void Submit()
        {
            // Even a failed submission attempt requires a queue retirement marker.
            Submitted = true;
            if (Graphite is not null) Graphite.Submit();
            else
            {
                Ganesh!.Canvas.Flush();
                Ganesh.Flush();
                slot._ganesh!.Flush(submit: true, synchronous: false);
            }
        }

        internal void Present() => drawable.Present();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (Graphite is not null)
            {
                if (Submitted) Graphite.CompleteGpuWork();
                else Graphite.CancelRecording();
            }
            Ganesh?.Dispose();
            Target?.Dispose();
            drawable.Dispose();
            slot._leases--;
            if (slot._retired && slot._leases == 0) slot.Release();
        }
    }
}
#endif
