using System.Text.Json;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MauiSampleApp;

public sealed class DorotiSkiaView : SKGLView
{
    private int _frames;
    private int _initialWidth;
    private int _initialHeight;
    private long _surfaceGeneration;
    private GRContext? _context;

    public DorotiSkiaView()
    {
        HasRenderLoop = false;
        EnableTouchEvents = true;
        PaintSurface += PaintGpuSurface;
        HandlerChanged += (_, _) => InvalidateSurface();
        SizeChanged += (_, _) => InvalidateSurface();
    }

    private void PaintGpuSurface(object? sender, SKPaintGLSurfaceEventArgs e)
    {
        if (e.Surface is null || GRContext is null)
        {
            throw new InvalidOperationException("Strict MAUI mode requires a GPU-backed SKSurface and GRContext.");
        }

        if (!ReferenceEquals(_context, GRContext))
        {
            _context = GRContext;
            _surfaceGeneration++;
        }

        var frame = Interlocked.Increment(ref _frames);
        if (frame == 1)
        {
            _initialWidth = e.BackendRenderTarget.Width;
            _initialHeight = e.BackendRenderTarget.Height;
        }

        e.Surface.Canvas.Clear(new SKColor(29, 27, 32));
        using var paint = new SKPaint { Color = new SKColor(208, 188, 255), IsAntialias = true };
        e.Surface.Canvas.DrawCircle(e.BackendRenderTarget.Width / 2f, e.BackendRenderTarget.Height / 2f,
            MathF.Min(e.BackendRenderTarget.Width, e.BackendRenderTarget.Height) / 4f, paint);
        e.Surface.Canvas.Flush();

        WriteEvidence(frame, e);
        if (frame < 3)
        {
            Dispatcher.Dispatch(InvalidateSurface);
        }
    }

    private void WriteEvidence(int frame, SKPaintGLSurfaceEventArgs e)
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_MAUI_FEASIBILITY_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var evidence = new
        {
            schema = "doroti-maui-feasibility/v1",
            targetFramework = AppContext.TargetFrameworkName,
            mauiVersion = "10.0.90",
            markupToolkitVersion = "8.0.0",
            skiaSharpVersion = "3.119.4",
            nativeViewType = Handler?.PlatformView?.GetType().FullName,
#if WINDOWS
            graphicsBackend = "winui3/SKSwapChainPanel/ANGLE-DirectX-Skia",
#elif MACCATALYST
            graphicsBackend = "UIKit-MacCatalyst/SKMetalView/Metal-Skia",
#endif
            gpuContext = GRContext is not null,
            gpuSurface = e.Surface is not null,
            width = e.BackendRenderTarget.Width,
            height = e.BackendRenderTarget.Height,
            initialWidth = _initialWidth,
            initialHeight = _initialHeight,
            resizeObserved = _initialWidth != e.BackendRenderTarget.Width || _initialHeight != e.BackendRenderTarget.Height,
            density = DeviceDisplay.Current.MainDisplayInfo.Density,
            surfaceGeneration = _surfaceGeneration,
            frames = frame,
            renderLoop = HasRenderLoop,
        };
        File.WriteAllText(path, JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }));
        var autoQuitFrames = int.TryParse(
            Environment.GetEnvironmentVariable("DOROTI_MAUI_FEASIBILITY_AUTO_QUIT_FRAMES"),
            out var configuredFrames)
            ? configuredFrames
            : 3;
        if (autoQuitFrames > 0 && frame >= autoQuitFrames)
        {
            Dispatcher.Dispatch(() => Application.Current?.Quit());
        }
    }
}
