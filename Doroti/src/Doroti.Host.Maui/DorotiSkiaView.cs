#if IOS && !MACCATALYST
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace Doroti.Host.Maui;

/// <summary>
/// Doroti's MAUI surface contract for Metal and Graphite. Window changes are
/// forwarded directly, without SKGLView's reflection-based string binding.
/// Native surfaces and touch delivery remain owned by the existing handlers.
/// </summary>
public class DorotiSkiaView : View, ISKGLView
{
    public static readonly BindableProperty HasRenderLoopProperty =
        BindableProperty.Create(nameof(HasRenderLoop), typeof(bool), typeof(DorotiSkiaView), false);
    public static readonly BindableProperty EnableTouchEventsProperty =
        BindableProperty.Create(nameof(EnableTouchEvents), typeof(bool), typeof(DorotiSkiaView), false);
    public static readonly BindableProperty IgnorePixelScalingProperty =
        BindableProperty.Create(nameof(IgnorePixelScaling), typeof(bool), typeof(DorotiSkiaView), false);

    public bool HasRenderLoop { get => (bool)GetValue(HasRenderLoopProperty); set => SetValue(HasRenderLoopProperty, value); }
    public bool EnableTouchEvents { get => (bool)GetValue(EnableTouchEventsProperty); set => SetValue(EnableTouchEventsProperty, value); }
    public bool IgnorePixelScaling { get => (bool)GetValue(IgnorePixelScalingProperty); set => SetValue(IgnorePixelScalingProperty, value); }
    public SKSize CanvasSize { get; private set; }
    public GRContext? GRContext { get; private set; }
    public event EventHandler<SKPaintGLSurfaceEventArgs>? PaintSurface;
    public event EventHandler<SKTouchEventArgs>? Touch;

    public void InvalidateSurface() => Handler?.Invoke(nameof(ISKGLView.InvalidateSurface));
    bool ISKGLView.HasRenderLoop => HasRenderLoop && Window is not null;
    void ISKGLView.OnCanvasSizeChanged(SKSizeI size) => CanvasSize = size;
    void ISKGLView.OnGRContextChanged(GRContext? context) => GRContext = context;
    void ISKGLView.OnPaintSurface(SKPaintGLSurfaceEventArgs args) => PaintSurface?.Invoke(this, args);
    void ISKGLView.OnTouch(SKTouchEventArgs args) => Touch?.Invoke(this, args);

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Window)) Handler?.UpdateValue(nameof(HasRenderLoop));
    }
}
#endif
