#if WINDOWS
using Microsoft.Maui;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Handlers;
using SkiaSharp.Views.Windows;
using Windows.Foundation;

namespace Doroti.Host.Maui;

/// <summary>
/// Public SkiaSharp handler extension which preserves the stock handler's
/// connection, touch, CanvasSize and GRContext behavior while replacing only
/// the native panel factory and the private Maui panel's scaling mapper.
/// </summary>
public sealed class DorotiWindowsSkiaViewHandler : SKGLViewHandler
{
    private static readonly PropertyMapper<ISKGLView, SKGLViewHandler> Mapper =
        new(SKGLViewMapper)
        {
            [nameof(ISKGLView.IgnorePixelScaling)] = MapDorotiIgnorePixelScaling,
        };

    public DorotiWindowsSkiaViewHandler()
        : base(Mapper, SKGLViewCommandMapper)
    {
    }

    protected override SKSwapChainPanel CreatePlatformView() => new DorotiWindowsSwapChainPanel();

    private static void MapDorotiIgnorePixelScaling(SKGLViewHandler handler, ISKGLView view)
    {
        if (handler.PlatformView is not DorotiWindowsSwapChainPanel panel) return;
        panel.IgnorePixelScaling = view.IgnorePixelScaling;
        panel.Invalidate();
    }
}

internal readonly record struct DorotiWindowsPreSwap(
    TimeSpan Timestamp,
    int SurfaceWidth,
    int SurfaceHeight);

/// <summary>
/// Stock SKSwapChainPanel rendering plus a managed boundary immediately after
/// Skia flushes and immediately before AngleSwapChainPanel performs the final
/// eglSwapBuffers call. The private leading resize swap remains unobserved and
/// must be treated separately in trace analysis.
/// </summary>
public sealed class DorotiWindowsSwapChainPanel : SKSwapChainPanel
{
    internal event Action<DorotiWindowsPreSwap>? BeforeFinalSwap;
    internal event Action? ContextDestroying;

    public DorotiWindowsSwapChainPanel()
    {
        DrawInBackground = false;
        EnableRenderLoop = false;
    }

    internal bool IgnorePixelScaling { get; set; }

    protected override void OnPaintSurface(SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs e)
    {
        if (IgnorePixelScaling)
        {
            var density = (float)ContentsScale;
            var userVisibleSize = new SKSizeI(
                (int)(e.Info.Width / density),
                (int)(e.Info.Height / density));
            var canvas = e.Surface.Canvas;
            canvas.Scale(density);
            canvas.Save();
            e = new SkiaSharp.Views.Windows.SKPaintGLSurfaceEventArgs(
                e.Surface,
                e.BackendRenderTarget,
                e.Origin,
                e.Info.WithSize(userVisibleSize),
                e.Info);
        }
        base.OnPaintSurface(e);
    }

    protected override void OnRenderFrame(Windows.Foundation.Rect rect)
    {
        base.OnRenderFrame(rect);
        BeforeFinalSwap?.Invoke(new(
            Doroti.Ui.DorotiFrameClock.Now,
            Math.Max(0, checked((int)rect.Width)),
            Math.Max(0, checked((int)rect.Height))));
    }

    protected override void OnDestroyingContext()
    {
        ContextDestroying?.Invoke();
        base.OnDestroyingContext();
    }
}
#endif
