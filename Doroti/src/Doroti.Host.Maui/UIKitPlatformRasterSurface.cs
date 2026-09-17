#if IOS && !MACCATALYST
using UIKit;
using CoreAnimation;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Metal;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>A reusable transparent raster slot. All slots submit to the owning MTKView's queue.</summary>
internal sealed class UIKitPlatformRasterSurface : UIView
{
    private readonly CAMetalLayer _metal;
    private readonly SkiaGraphiteSession? _graphite;
    private int _leases;
    private bool _retired;

    internal UIKitPlatformRasterSurface(IMTLDevice device, IMTLCommandQueue queue)
    {
        _metal = new CAMetalLayer
        {
            Device = device, PixelFormat = MTLPixelFormat.BGRA8Unorm, FramebufferOnly = false,
            Opaque = false, PresentsWithTransaction = true,
        };
        Layer.AddSublayer(_metal);
        Opaque = false;
        UserInteractionEnabled = false;
        Hidden = true;
        _graphite = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, 1);
    }

    public override void LayoutSubviews() { base.LayoutSubviews(); _metal.Frame = Bounds; }
    public override UIView? HitTest(CGPoint point, UIEvent? evt) => null;

    internal RasterFrame Prepare(SkiaSceneRenderer renderer, PlatformRasterSegment segment, int width, int height, double scale)
    {
        ObjectDisposedException.ThrowIf(_retired, this);
        _metal.DrawableSize = new CGSize(width, height);
        _metal.ContentsScale = (nfloat)scale;
        var drawable = _metal.NextDrawable() ?? throw new InvalidOperationException("No UIKit segment drawable available.");
        var frame = new RasterFrame(this, drawable, segment.PaintOrder);
        _leases++;
        try
        {
            frame.Graphite = _graphite!.BeginMetalFrame(width, height, drawable.Texture.Handle);
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

    internal sealed class RasterFrame(UIKitPlatformRasterSurface slot, ICAMetalDrawable drawable, int order) : IDisposable
    {
        internal SkiaGraphiteSession.Frame? Graphite;
        internal SKSurface Surface => Graphite!.Surface;
        internal UIKitPlatformRasterSurface Slot => slot;
        internal int PaintOrder => order;
        internal bool Submitted { get; private set; }
        private bool _disposed;

        internal void Submit()
        {
            // Even a failed submission attempt requires a queue retirement marker.
            Submitted = true;
            Graphite!.Submit();
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
            drawable.Dispose();
            slot._leases--;
            if (slot._retired && slot._leases == 0) slot.Release();
        }
    }
}
#endif
