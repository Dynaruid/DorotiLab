#if MACCATALYST
using CoreAnimation;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using SkiaSharp.Views.Maui.Handlers;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>
/// Catalyst SKMetalView that consumes a bounds change inside the same UIKit
/// layout callback instead of leaving the prior drawable stretched until the
/// next display-link pulse.
/// </summary>
public sealed class DorotiMacCatalystSkglViewHandler : SKGLViewHandler
{
    protected override SKMetalView CreatePlatformView() => new DorotiMacCatalystMetalView
    {
        BackgroundColor = UIColor.Clear,
        Opaque = false,
        ContentMode = UIViewContentMode.Redraw,
    };

    private sealed class DorotiMacCatalystMetalView : SKMetalView
    {
        private CGSize _lastLayoutSize;
        private bool _drawingLayout;

        public DorotiMacCatalystMetalView()
        {
            // MTKView normally resizes its drawable after UIKit has committed
            // the new bounds. During Catalyst live resize Core Animation then
            // stretches the previous drawable for a frame or two. Own the
            // drawable size here so bounds and backing pixels change together.
            AutoResizeDrawable = false;
            Layer.ContentsGravity = CALayer.GravityTopLeft;
            Layer.MasksToBounds = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var size = Bounds.Size;
            if (_drawingLayout || Window is null || size.Width <= 0 || size.Height <= 0 ||
                size.Equals(_lastLayoutSize)) return;

            _lastLayoutSize = size;
            try
            {
                _drawingLayout = true;
                var scale = ContentScaleFactor > 0
                    ? ContentScaleFactor
                    : UIScreen.MainScreen.Scale;
                var drawableSize = new CGSize(
                    Math.Max(1, Math.Round(size.Width * scale)),
                    Math.Max(1, Math.Round(size.Height * scale)));

                // Do not let Core Animation interpolate either the Metal
                // backing size or the contents placement during live resize.
                CATransaction.Begin();
                CATransaction.DisableActions = true;
                // UIView.ContentMode.Redraw may restore resize gravity after
                // construction, so pin it again in the actual resize callback.
                Layer.ContentsGravity = CALayer.GravityTopLeft;
                DrawableSize = drawableSize;
                Layer.ContentsScale = scale;
                CATransaction.Commit();

                // MTKView.Draw invokes the existing SkiaSharp delegate with
                // the drawable that exactly matches the current bounds.
                Draw();
            }
            finally
            {
                _drawingLayout = false;
            }
        }
    }
}
#endif
