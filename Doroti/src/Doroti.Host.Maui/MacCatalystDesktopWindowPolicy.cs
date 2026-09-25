#if MACCATALYST || DOROTI_APPKIT_CONTRACTS
using Doroti.Desktop;

namespace Doroti.Host.Maui;

/// <summary>Public UIKit scene controls only; AppKit behavior is not inferred from the OS.</summary>
internal static class MacCatalystDesktopWindowPolicy
{
    internal static WindowEvaluation Evaluate(WindowOptions options, WindowOptions? current)
    {
        options.Validate();
        if (options.StartupVisibility != WindowStartupVisibility.PlatformDefault)
            return Reject(
                "Catalyst owns scene visibility at launch. Select PlatformDefault; hidden first-frame preparation is not supported."
            );
        if (options.Position is not null || options.Centered)
            return Reject(
                "Global physical-pixel placement/centering is not mapped by the Catalyst adapter."
            );
        if (options.AlwaysOnTop || options.SkipTaskbar)
            return Reject(
                "UIKit does not expose per-scene desktop topmost or Dock visibility controls."
            );
        if (options.PresentationState != WindowPresentationState.Normal)
            return Reject(
                "Initial minimized/maximized/full-screen state is owned by the Catalyst scene system."
            );
        var a = options.Appearance;
        // MacOSBackdrop is an AppKit override, not a UIKit material request.
        if (a.MacOSBackdrop is not null)
            return Reject(
                "MacOSBackdrop is AppKit-specific. Select a Catalyst System or Solid backdrop explicitly."
            );
        if (
            a.Backdrop.Mode
            is not (Desktop.WindowBackdropMode.System or Desktop.WindowBackdropMode.Solid)
        )
            return Reject(
                "Catalyst desktop Acrylic/LiquidGlass/transparent composition is not implemented."
            );
        if (
            a.Backdrop.TintColor is not null
            || a.Backdrop.TintOpacity is not null
            || a.Backdrop.LuminosityOpacity is not null
            || a.Backdrop.AcrylicKind != Desktop.WindowAcrylicKind.Default
        )
            return Reject(
                "Catalyst System/Solid backdrops do not have tint or luminosity controls."
            );
        if (a.BackgroundColor.a != 1 || a.DarkBackgroundColor is { a: not 1 })
            return Reject("This Catalyst adapter requires an opaque renderer background.");
        if (
            a.TitleBar.Style != WindowTitleBarStyle.Normal
            || a.TitleBar.Frame != WindowFrame.Standard
            || a.TitleBar.Buttons != WindowCaptionButtonMode.Native
            || a.TitleBar.Background != WindowTitleBarBackground.System
            || a.TitleBar.BackgroundColor is not null
        )
            return Reject(
                "Only the native System titlebar and traffic lights are supported on Catalyst."
            );
        if (a.ThemeSource == WindowThemeSource.App)
            return Reject("Select System or Explicit; the app theme bridge is not connected.");
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

    private static WindowEvaluation Reject(string reason) => new(WindowSupport.Unsupported, reason);
}
#endif
