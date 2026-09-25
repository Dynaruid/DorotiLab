using Doroti.Desktop;
using Doroti.Ui;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using BackdropMode = Doroti.Desktop.WindowBackdropMode;

namespace Doroti.Host.Qt;

internal static class QtDesktopWindowPolicy
{
    internal static WindowEvaluation Evaluate(WindowOptions options, WindowOptions? current)
    {
        options.Validate();
        static WindowEvaluation No(string reason) => new(WindowSupport.Unsupported, reason);
        if (options.StartupVisibility != WindowStartupVisibility.PlatformDefault)
            return No("Qt Quick currently requires PlatformDefault startup; hidden first-frame readiness is unsupported.");
        if (current is null && options.PresentationState == WindowPresentationState.Minimized)
            return No("Qt Desktop cannot prepare its first frame while initially minimized.");
        if (options.Position is not null || options.Centered)
            return No("Qt Desktop does not expose global physical-pixel placement or centering.");
        if (options.AlwaysOnTop || options.SkipTaskbar)
            return No("Qt Desktop does not guarantee topmost or taskbar policy across QPA/compositors.");
        foreach (var size in new[] { options.Size, options.MinimumSize, options.MaximumSize })
            if (size is not null && !ValidSize(size))
                return No("Qt window sizes must be integral logical units in [1, 16777215].");
        var appearance = options.Appearance;
        if (appearance.TitleBar != new WindowTitleBarOptions())
            return No("Qt Desktop currently supports Normal/System/Standard native chrome only.");
        if (appearance.MacOSBackdrop is not null || appearance.ThemeSource != WindowThemeSource.System)
            return No("Qt Desktop currently supports the system theme without platform overrides.");
        if (appearance.Backdrop.Mode is not (BackdropMode.System or BackdropMode.Solid)
            || appearance.Backdrop.AcrylicKind != Doroti.Desktop.WindowAcrylicKind.Default
            || appearance.Backdrop.TintColor is not null || appearance.Backdrop.TintOpacity is not null
            || appearance.Backdrop.LuminosityOpacity is not null)
            return No("Qt Desktop currently supports opaque System/Solid backgrounds; legacy compositor blur remains separate.");
        if (appearance.BackgroundColor.alpha != 255
            || (appearance.DarkBackgroundColor is { } dark && dark.alpha != 255))
            return No("Qt Desktop System/Solid backgrounds must be opaque.");
        if (current is not null && appearance != current.Appearance)
            return new(WindowSupport.RequiresRecreation,
                "Qt Desktop appearance is fixed at creation; runtime material/theme changes are not implemented.");
        return WindowEvaluation.Supported;
    }

    private static bool ValidSize(Size size) => size.width <= 16777215 && size.height <= 16777215
        && size.width == Math.Floor(size.width) && size.height == Math.Floor(size.height);
}
