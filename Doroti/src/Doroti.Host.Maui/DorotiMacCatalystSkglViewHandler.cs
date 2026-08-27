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
            // A Catalyst resize driven from the left or bottom also moves the
            // native window origin. Presenting Metal independently from Core
            // Animation lets the drawable and the window geometry land in
            // adjacent commits, which appears as a one-frame positional shake.
            PresentsWithTransaction = true;
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

                // Commit the new backing size and the Metal presentation with
                // the same Core Animation transaction. This is important for
                // the left and bottom edges, where AppKit changes the window
                // origin as well as its size.
                CATransaction.Begin();
                try
                {
                    CATransaction.DisableActions = true;
                    // UIView.ContentMode.Redraw may restore resize gravity after
                    // construction, so pin it again in the actual resize callback.
                    Layer.ContentsGravity = CALayer.GravityTopLeft;
                    DrawableSize = drawableSize;
                    Layer.ContentsScale = scale;

                    // MTKView.Draw invokes the existing SkiaSharp delegate with
                    // the drawable that exactly matches the current bounds.
                    Draw();
                }
                finally
                {
                    CATransaction.Commit();
                }
            }
            finally
            {
                _drawingLayout = false;
            }
        }
    }
}
#endif
