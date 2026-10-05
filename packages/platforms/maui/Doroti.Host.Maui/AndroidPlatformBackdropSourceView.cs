#if ANDROID
using System.Runtime.Versioning;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace Doroti.Host.Maui;

/// <summary>Keeps the native child's normal display list available to the backdrop.
/// Sampling must not call WebView.Draw again with the moving blur viewport.</summary>
[SupportedOSPlatform("android31.0")]
internal sealed class AndroidPlatformBackdropSourceView(Context context)
    : FrameLayout(context),
        IAndroidPlatformBackdropSource
{
    private readonly RenderNode _content = new("Doroti native backdrop source");
    private bool _sampled;

    internal void SetBackdropSampling(bool sampled)
    {
        if (_sampled == sampled)
        {
            return;
        }

        _sampled = sampled;
        // A shared display list alone can still invoke the WebView GPU functor
        // twice with different viewport constraints. Reuse one composited image
        // for both destinations while blur is active, without CPU readback.
        _content.SetUseCompositingLayer(sampled, null);
        Invalidate();
    }

    protected override void DispatchDraw(Canvas canvas)
    {
        if (!_sampled || !canvas.IsHardwareAccelerated)
        {
            base.DispatchDraw(canvas);
            return;
        }
        _content.SetPosition(0, 0, Width, Height);
        var recording = _content.BeginRecording(Width, Height);
        try
        {
            base.DispatchDraw(recording);
        }
        finally
        {
            _content.EndRecording();
        }
        canvas.DrawRenderNode(_content);
    }

    public void DrawBackdropSource(Canvas canvas)
    {
        if (_content.HasDisplayList)
        {
            canvas.DrawRenderNode(_content);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _content.DiscardDisplayList();
            _content.Dispose();
        }
        base.Dispose(disposing);
    }
}
#endif
