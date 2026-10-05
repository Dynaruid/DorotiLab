#if MACOS || DOROTI_APPKIT_CONTRACTS
using Doroti.Desktop;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using BackdropMode = Doroti.Desktop.WindowBackdropMode;

namespace Doroti.Host.Maui;

internal static class AppKitDesktopWindowPolicy
{
    internal static WindowEvaluation Evaluate(WindowOptions options, WindowOptions? current)
    {
        options.Validate();
        if (options.StartupVisibility == WindowStartupVisibility.PlatformDefault)
            return new(WindowSupport.Unsupported, "This adapter owns first visibility; use Manual or WhenReady.");
        var a = options.Appearance;
        var b = a.MacOSBackdrop ?? a.Backdrop;
        if (options.SkipTaskbar)
            return new(
                WindowSupport.Unsupported,
                "AppKit Dock visibility belongs to the application, not an individual window."
            );
        if (options.Position is not null)
            return new(
                WindowSupport.Unsupported,
                "Global physical-pixel position is not mapped across AppKit screens; use Centered."
            );
        if (
            a.TitleBar.Frame != WindowFrame.Standard
            || a.TitleBar.Buttons != WindowCaptionButtonMode.Native
            || a.TitleBar.Style != WindowTitleBarStyle.Normal
        )
            return new(
                WindowSupport.Unsupported,
                "Custom/hidden chrome requires separate native input and accessibility qualification."
            );
        if (a.ThemeSource == WindowThemeSource.App)
            return new(
                WindowSupport.Unsupported,
                "Select System or Explicit; the app theme bridge is not connected."
            );
        if (
            b.LuminosityOpacity is not null
            || (
                b.Mode != BackdropMode.LiquidGlass
                && (b.TintColor is not null || b.TintOpacity is not null)
            )
        )
            return new(
                WindowSupport.Unsupported,
                "AppKit Acrylic does not expose Windows tint/luminosity controls; Liquid Glass supports tint only."
            );
        if (b.TintOpacity is not null && b.TintColor is null)
            return new(
                WindowSupport.Unsupported,
                "Liquid Glass tint opacity requires a tint color."
            );
        if (b.Mode != BackdropMode.Acrylic && b.AcrylicKind != Desktop.WindowAcrylicKind.Default)
            return new(WindowSupport.Unsupported, "AcrylicKind requires Acrylic mode.");
        if (
            a.TitleBar.Background == WindowTitleBarBackground.Backdrop
            && b.Mode is not (BackdropMode.Acrylic or BackdropMode.LiquidGlass)
        )
            return new(
                WindowSupport.Unsupported,
                "Backdrop titlebar requires Acrylic or Liquid Glass."
            );
        if (
            a.TitleBar.BackgroundColor is not null
            && a.TitleBar.Background != WindowTitleBarBackground.Solid
        )
            return new(WindowSupport.Unsupported, "Titlebar color requires Solid background.");
        if (
            current is not null
            && (
                current.Appearance.BackgroundColor != a.BackgroundColor
                || current.Appearance.DarkBackgroundColor != a.DarkBackgroundColor
            )
        )
            return new(
                WindowSupport.RequiresRecreation,
                "Changing the renderer base color requires recreation."
            );
        return WindowEvaluation.Supported;
    }

    internal static WindowEffectiveAppearance Resolve(
        Appearance appearance,
        bool supportsGlass,
        bool reduceTransparency
    )
    {
        var material = appearance.MacOSBackdrop ?? appearance.Backdrop;
        var mode = material.Mode;
        string? detail = null;
        var policy = false;
        if (mode == BackdropMode.LiquidGlass && !supportsGlass)
        {
            mode =
                material.Fallback == WindowBackdropFallback.Solid
                    ? BackdropMode.Solid
                    : BackdropMode.Transparent;
            detail = "Liquid Glass requires macOS 26; selected fallback applied.";
        }
        if (mode is BackdropMode.Acrylic or BackdropMode.LiquidGlass && reduceTransparency)
        {
            mode = BackdropMode.Solid;
            policy = true;
            detail = "Reduce Transparency is enabled.";
        }
        return new(
            appearance with
            {
                Backdrop = material with { Mode = mode },
                MacOSBackdrop = null,
            },
            policy,
            detail
        );
    }
}
#endif
