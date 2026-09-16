#if ANDROID
using System.Runtime.Versioning;
using Android.Content;
using Android.Graphics;
using Android.Views;
using SkiaSharp;
using NativeView = Android.Views.View;

namespace Doroti.Host.Maui;

/// <summary>Samples preceding live View render nodes, applies a GPU blur, and clips
/// the result to this overlay. Native controls remain attached and interactive.</summary>
[SupportedOSPlatform("android31.0")]
internal sealed class AndroidPlatformBackdropView : NativeView
{
    private readonly RenderNode _node = new("Doroti native backdrop");
    private RenderEffect? _effect;
    private NativeView[] _sources = [];
    private SKRectI _sampleBounds;
    private float _sigmaX, _sigmaY;

    internal AndroidPlatformBackdropView(Context context) : base(context)
    {
        ImportantForAccessibility = ImportantForAccessibility.No;
        SetWillNotDraw(false);
        _node.SetClipToBounds(true);
    }

    internal void UpdateFrame(SKRectI sampleBounds, NativeView[] sources, float sigmaX, float sigmaY)
    {
        _sampleBounds = sampleBounds; _sources = sources;
        _node.SetPosition(0, 0, sampleBounds.Width, sampleBounds.Height);
        if (_effect is null || sigmaX != _sigmaX || sigmaY != _sigmaY)
        {
            var effect = RenderEffect.CreateBlurEffect(sigmaX, sigmaY, Shader.TileMode.Clamp!)!;
            _node.SetRenderEffect(effect);
            _effect?.Dispose(); _effect = effect; _sigmaX = sigmaX; _sigmaY = sigmaY;
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
                source.Draw(recording);
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
