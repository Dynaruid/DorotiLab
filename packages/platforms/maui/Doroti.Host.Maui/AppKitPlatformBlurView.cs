#if MACOS
using AppKit;
using CoreGraphics;
using CoreImage;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Public Core Image backdrop filters over live AppKit/Metal siblings.
/// The effect has no material tint and never changes its opacity to simulate radius.</summary>
internal sealed class AppKitPlatformBlurView : NSView
{
    internal static PlatformEffectSupport Support { get; } =
        new(
            true,
            1,
            64,
            true,
            "AppKit Core Image supports one isotropic backdrop, logical sigma 0–64 and saturation 0–2."
        );
    internal PlatformEffectStyle? AppliedStyle { get; private set; }
    private double _sigma = double.NaN,
        _saturation = double.NaN;

    internal AppKitPlatformBlurView()
    {
        Identifier = "doroti-platform-effect";
        WantsLayer = true;
        LayerUsesCoreImageFilters = true;
        Layer!.MasksToBounds = true;
        Layer.Opaque = false;
        AlphaValue = 1;
    }

    internal static void Validate(PlatformBackdropSegment effect)
    {
        effect.Style?.Validate();
        if (effect.SigmaX != effect.SigmaY || effect.SigmaX is < 0 or > 64)
        {
            throw new NotSupportedException(
                "AppKit Core Image requires isotropic sigma in [0,64] logical points."
            );
        }

        if (NSWorkspace.SharedWorkspace.AccessibilityDisplayShouldReduceTransparency)
        {
            throw new NotSupportedException(
                "Reduce Transparency is enabled; select SolidTint explicitly."
            );
        }
    }

    internal void SetEffect(PlatformBackdropSegment effect)
    {
        AppKitPlatformViewDispatcher.VerifyThread();
        var sigma = effect.SigmaX;
        var saturation = effect.Style?.Saturation ?? 1;
        if (_sigma != sigma || _saturation != saturation)
        {
            // Replace the complete filter array. Core Animation copies filters on
            // assignment; mutating an already-attached CIFilter would not update it.
            using var blur = new CIGaussianBlur { Radius = (float)sigma };
            using var color = new CIColorControls
            {
                Saturation = (float)saturation,
                Brightness = 0,
                Contrast = 1,
            };
            BackgroundFilters =
                sigma == 0
                    ? saturation == 1
                        ? []
                        : [color]
                    : saturation == 1
                        ? [blur]
                        : [blur, color];
            _sigma = sigma;
            _saturation = saturation;
        }
        AppliedStyle = effect.Style;
        Hidden = (sigma == 0 && saturation == 1) || Frame.IsEmpty;
    }

    internal void Deactivate()
    {
        // Background filters can continue affecting composited siblings even when
        // AppKit hides the empty view. Remove the filters before its paint slot is
        // reused by the sharp foreground, then invalidate the parameter cache.
        BackgroundFilters = [];
        _sigma = _saturation = double.NaN;
        AppliedStyle = null;
        Hidden = true;
    }

    public override bool IsOpaque => false;

    public override NSView? HitTest(CGPoint point) => null;

    public override bool AcceptsFirstResponder() => false;

    public override bool AccessibilityElement
    {
        get => false;
        set { }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BackgroundFilters = [];
        }

        base.Dispose(disposing);
    }
}
#endif
