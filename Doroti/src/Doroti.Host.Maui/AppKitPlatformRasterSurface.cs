#if MACOS
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Skia.Rendering;
using Metal;
using SkiaSharp;

namespace Doroti.Host.Maui;

/// <summary>A reusable transparent raster slot. All slots submit to the owning MTKView's queue.</summary>
internal sealed class AppKitPlatformRasterSurface : NSView
{
    private readonly CAMetalLayer _metal;
    private readonly CGColorSpace _colorSpace =
        CGColorSpace.CreateSrgb()
        ?? throw new InvalidOperationException("sRGB color space is unavailable.");
    private readonly SkiaGraphiteSession? _graphite;
    private readonly GRContext? _ganesh;
    private int _leases;
    private bool _retired;
    private SkiaPlatformRasterContent.Slice? _displayed;
    private SkiaPlatformRasterContent.CacheScope _displayedScope;

    internal AppKitPlatformRasterSurface(
        IMTLDevice device,
        IMTLCommandQueue queue,
        GRContext? ganesh
    )
    {
        _ganesh = ganesh;
        _metal = new CAMetalLayer
        {
            Device = device,
            PixelFormat = MTLPixelFormat.BGRA8Unorm,
            FramebufferOnly = false,
            Opaque = false,
            PresentsWithTransaction = true,
            ColorSpace = _colorSpace,
        };
        Layer = _metal;
        WantsLayer = true;
        Hidden = true;
        if (DorotiMacOSMetalView.UseGraphite)
        {
            _graphite = SkiaGraphiteSession.CreateMetal(device.Handle, queue.Handle, 1);
        }
    }

    public override bool IsOpaque => false;

    public override NSView? HitTest(CGPoint point) => null;

    internal RasterFrame Prepare(
        SkiaSceneRenderer renderer,
        PlatformRasterSegment segment,
        int width,
        int height,
        PlatformCompositionToken token
    )
    {
        ObjectDisposedException.ThrowIf(_retired, this);
        var bounds = SkiaPlatformRasterContent.Coverage(segment.Commands, width, height);
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            bounds = new SKRectI(0, 0, 1, 1);
        }

        var slice = new SkiaPlatformRasterContent.Slice(segment.Commands, bounds);
        var scope = SkiaPlatformRasterContent.CacheScope.From(
            token,
            width,
            height,
            renderer.PlatformBackgroundColor
        );
        if (
            !Hidden
            && Superview is not null
            && _displayed is { } previous
            && SkiaPlatformRasterContent.CanReuse(_displayedScope, previous, scope, slice)
        )
        {
            _leases++;
            return new RasterFrame(this, null, segment.PaintOrder, slice, scope);
        }
        _displayed = null; // A failed resize/present must not authorize reuse of stale backing.
        _metal.DrawableSize = new CGSize(bounds.Width, bounds.Height);
        _metal.ContentsScale = (nfloat)token.DeviceScaleX;
        var drawable =
            _metal.NextDrawable()
            ?? throw new InvalidOperationException("No AppKit segment drawable available.");
        var frame = new RasterFrame(this, drawable, segment.PaintOrder, slice, scope);
        _leases++;
        try
        {
            if (_graphite is not null)
            {
                frame.Graphite = _graphite.BeginMetalFrame(
                    bounds.Width,
                    bounds.Height,
                    drawable.Texture.Handle
                );
            }
            else
            {
                frame.Target = new GRBackendRenderTarget(
                    bounds.Width,
                    bounds.Height,
                    new GRMtlTextureInfo(drawable.Texture)
                );
                frame.Ganesh =
                    SKSurface.Create(
                        _ganesh,
                        frame.Target,
                        GRSurfaceOrigin.TopLeft,
                        SKColorType.Bgra8888
                    )
                    ?? throw new InvalidOperationException(
                        "Could not wrap AppKit segment drawable."
                    );
            }
            var canvas = frame.Surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            canvas.Save();
            canvas.Translate(-bounds.Left, -bounds.Top);
            renderer.DrawPlatformRasterSegment(canvas, segment.Commands, width, height);
            canvas.Restore();
            return frame;
        }
        catch
        {
            frame.Dispose();
            throw;
        }
    }

    internal void Retire()
    {
        if (_retired)
        {
            return;
        }

        _retired = true;
        RemoveFromSuperview();
        _graphite?.StopAcceptingFrames();
        if (_leases == 0)
        {
            Release();
        }
    }

    private void Release()
    {
        _graphite?.Dispose();
        _metal.ColorSpace = null;
        _metal.Dispose();
        _colorSpace.Dispose();
        Dispose();
    }

    internal sealed class RasterFrame(
        AppKitPlatformRasterSurface slot,
        ICAMetalDrawable? drawable,
        int order,
        SkiaPlatformRasterContent.Slice slice,
        SkiaPlatformRasterContent.CacheScope scope
    ) : IDisposable
    {
        internal SkiaGraphiteSession.Frame? Graphite;
        internal SKSurface? Ganesh;
        internal GRBackendRenderTarget? Target;
        internal SKSurface Surface => Graphite?.Surface ?? Ganesh!;
        internal AppKitPlatformRasterSurface Slot => slot;
        internal int PaintOrder => order;
        internal CGRect Bounds =>
            new(
                slice.Bounds.Left / scope.ScaleX,
                slice.Bounds.Top / scope.ScaleY,
                slice.Bounds.Width / scope.ScaleX,
                slice.Bounds.Height / scope.ScaleY
            );
        internal bool Submitted { get; private set; }
        private bool _disposed;

        internal void Submit()
        {
            if (drawable is null)
            {
                return; // The layer keeps the previously presented image.
            }
            // Even a failed submission attempt requires a queue retirement marker.
            Submitted = true;
            if (Graphite is not null)
            {
                Graphite.Submit();
            }
            else
            {
                Ganesh!.Canvas.Flush();
                Ganesh.Flush();
                slot._ganesh!.Flush(submit: true, synchronous: false);
            }
        }

        internal void Present()
        {
            drawable?.Present();
            // Publishing cache metadata during Prepare/Submit would allow a
            // rejected frame to masquerade as the currently displayed image.
            slot._displayed = slice;
            slot._displayedScope = scope;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (Graphite is not null)
            {
                if (Submitted)
                {
                    Graphite.CompleteGpuWork();
                }
                else
                {
                    Graphite.CancelRecording();
                }
            }
            Ganesh?.Dispose();
            Target?.Dispose();
            drawable?.Dispose();
            slot._leases--;
            if (slot._retired && slot._leases == 0)
            {
                slot.Release();
            }
        }
    }
}
#endif
