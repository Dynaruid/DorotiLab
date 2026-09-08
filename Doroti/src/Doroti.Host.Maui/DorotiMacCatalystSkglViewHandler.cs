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
        private double _lastLayoutScale;
        private bool _drawingLayout;

        public DorotiMacCatalystMetalView()
        {
            // MTKView normally resizes its drawable after UIKit has committed
            // the new bounds. During Catalyst live resize Core Animation then
            // stretches the previous drawable for a frame or two. Own the
            // drawable size here so bounds and backing pixels change together.
            AutoResizeDrawable = false;
            // SKMetalView presents through MTLCommandBuffer.PresentDrawable.
            // Transaction presentation instead requires WaitUntilScheduled
            // followed by drawable.Present, which that delegate does not use.
            // Enabling it here leaves rendered interaction frames off screen.
            PresentsWithTransaction = false;
            Layer.ContentsGravity = CALayer.GravityTopLeft;
            Layer.MasksToBounds = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var size = Bounds.Size;
            var scale = (double)(ContentScaleFactor > 0
                ? ContentScaleFactor
                : Window?.Screen.Scale ?? UIScreen.MainScreen.Scale);
            if (_drawingLayout || Window is null || size.Width <= 0 || size.Height <= 0 ||
                (size.Equals(_lastLayoutSize) && scale.Equals(_lastLayoutScale))) return;

            _lastLayoutSize = size;
            _lastLayoutScale = scale;
            try
            {
                _drawingLayout = true;
                var drawableSize = new CGSize(
                    Math.Max(1, Math.Round(size.Width * scale)),
                    Math.Max(1, Math.Round(size.Height * scale)));

                // Update backing geometry without implicit layer animations,
                // then draw immediately. SKMetalView owns GPU presentation.
                CATransaction.Begin();
                try
                {
                    CATransaction.DisableActions = true;
                    // UIView.ContentMode.Redraw may restore resize gravity after
                    // construction, so pin it again in the actual resize callback.
                    Layer.ContentsGravity = CALayer.GravityTopLeft;
                    DrawableSize = drawableSize;
                    Layer.ContentsScale = (System.Runtime.InteropServices.NFloat)scale;

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
