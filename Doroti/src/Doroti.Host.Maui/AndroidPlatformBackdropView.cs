#if ANDROID
using System.Runtime.Versioning;
using Android.Content;
using Android.Graphics;
using Android.Views;
using SkiaSharp;
using NativeView = Android.Views.View;

namespace Doroti.Host.Maui;

// Some raster exists only to supply the blur; its normal View display list is empty.
internal interface IAndroidPlatformBackdropSource
{
    void DrawBackdropSource(Canvas canvas);
}

/// <summary>Samples preceding live View render nodes, applies a GPU blur, and clips
/// the result to this overlay. Native controls remain attached and interactive.</summary>
[SupportedOSPlatform("android31.0")]
internal sealed class AndroidPlatformBackdropView : NativeView
{
    private readonly RenderNode _node = new("Doroti native backdrop");
    private RenderEffect? _effect;
    private NativeView[] _sources = [];
    private SKRectI _sampleBounds;
    private float _sigmaX, _sigmaY, _saturation;

    internal AndroidPlatformBackdropView(Context context) : base(context)
    {
        ImportantForAccessibility = ImportantForAccessibility.No;
        SetWillNotDraw(false);
        _node.SetClipToBounds(true);
    }

    internal void UpdateFrame(SKRectI sampleBounds, NativeView[] sources, float sigmaX, float sigmaY, float saturation)
    {
        _sampleBounds = sampleBounds; _sources = sources;
        _node.SetPosition(0, 0, sampleBounds.Width, sampleBounds.Height);
        if (_effect is null || sigmaX != _sigmaX || sigmaY != _sigmaY || saturation != _saturation)
        {
            var red = .2126f * (1 - saturation);
            var green = .7152f * (1 - saturation);
            var blue = .0722f * (1 - saturation);
            using var matrix = new ColorMatrix([
                red + saturation, green, blue, 0, 0,
                red, green + saturation, blue, 0, 0,
                red, green, blue + saturation, 0, 0,
                0, 0, 0, 1, 0]);
            using var filter = new ColorMatrixColorFilter(matrix);
            RenderEffect effect;
            // RenderEffect applies AOSP Blur::convertRadiusToSigma(radius) =
            // radius * 0.57735 + 0.5. Our contract is already a device-pixel sigma.
            // Invert that mapping; sub-half-pixel kernels reduce to identity.
            var radiusX = Math.Max(0, (sigmaX - .5f) / .57735f);
            var radiusY = Math.Max(0, (sigmaY - .5f) / .57735f);
            if (radiusX > 0 || radiusY > 0)
            {
                using var blur = RenderEffect.CreateBlurEffect(radiusX, radiusY,
                    Shader.TileMode.Clamp ?? throw new InvalidOperationException("Clamp tile mode unavailable."))
                    ?? throw new InvalidOperationException("Android rejected the backdrop blur.");
                // Match Skia's color(blur(source)); clipping amplified channels
                // before blurring produces a different result at colored edges.
                effect = RenderEffect.CreateColorFilterEffect(filter, blur)
                    ?? throw new InvalidOperationException("Android rejected the backdrop color filter.");
            }
            else effect = RenderEffect.CreateColorFilterEffect(filter)
                ?? throw new InvalidOperationException("Android rejected the backdrop color filter.");
            _node.SetRenderEffect(effect);
            _effect?.Dispose(); _effect = effect; _sigmaX = sigmaX; _sigmaY = sigmaY; _saturation = saturation;
        }
        Invalidate();
    }

    protected override void OnDraw(Canvas canvas)
    {
        if (_sampleBounds.IsEmpty || _effect is null) return;
        if (!canvas.IsHardwareAccelerated)
            throw new PlatformNotSupportedException("Native backdrop requires a hardware Android Canvas.");
        var recording = _node.BeginRecording(_sampleBounds.Width, _sampleBounds.Height);
        try
        {
            foreach (var source in _sources)
            {
                if (source.Handle == 0 || source.Parent != Parent || source.Visibility != ViewStates.Visible) continue;
                recording.Save();
                recording.Translate(source.Left - _sampleBounds.Left, source.Top - _sampleBounds.Top);
                if (source is IAndroidPlatformBackdropSource raster) raster.DrawBackdropSource(recording);
                else source.Draw(recording);
                recording.Restore();
            }
        }
        finally { _node.EndRecording(); }
        canvas.Save();
        canvas.ClipRect(0, 0, Width, Height);
        canvas.Translate(_sampleBounds.Left - Left, _sampleBounds.Top - Top);
        canvas.DrawRenderNode(_node);
        canvas.Restore();
        // WebView animations can change without a Doroti scene revision. Resample while visible.
        if (IsShown) PostInvalidateOnAnimation();
    }

    public override bool OnTouchEvent(MotionEvent? e) => false;

    protected override void Dispose(bool disposing)
    {
        if (disposing) { _sources = []; _node.DiscardDisplayList(); _node.Dispose(); _effect?.Dispose(); _effect = null; }
        base.Dispose(disposing);
    }
}
#endif
