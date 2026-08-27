#if MACCATALYST
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
                // MTKView.Draw invokes the existing SkiaSharp delegate now.
                // If UIKit has not produced the resized drawable yet, the
                // delegate's normal SetNeedsDisplay path remains armed.
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
