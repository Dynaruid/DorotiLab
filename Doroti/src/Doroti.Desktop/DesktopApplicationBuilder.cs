using System.Runtime.CompilerServices;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Desktop;

public interface IDorotiDesktopApplicationStartup
{
    void Configure(DesktopApplicationBuilder desktop);
}

public sealed class DesktopApplicationBuilder
{
    internal DesktopApplicationBuilder(DorotiApplicationDescriptor application) =>
        DefaultMainWindow = DesktopApplication.FromViewConfiguration(application).MainWindow;

    /// <summary>Native window defaults supplied by the application launch configuration.</summary>
    public WindowCreateOptions DefaultMainWindow { get; }
    private WindowCreateOptions? _main;
    public WindowLifetimePolicy LifetimePolicy { get; set; } =
        WindowLifetimePolicy.OnLastWindowClosed;

    public void UseMainWindow(WindowCreateOptions options)
    {
        if (_main is not null)
            throw new InvalidOperationException(
                "UseMainWindow has already been called; window options have one owner."
            );
        ArgumentNullException.ThrowIfNull(options);
        options.Options.Validate();
        _main = options;
    }

    internal DesktopApplicationDefinition Build() =>
        new(
            _main
                ?? throw new InvalidOperationException("Desktop startup must call UseMainWindow."),
            LifetimePolicy
        );
}

public sealed record DesktopApplicationDefinition(
    WindowCreateOptions MainWindow,
    WindowLifetimePolicy LifetimePolicy
);

/// <summary>Registration is descriptor-scoped; common Hosting/Ui assemblies do not reference Desktop.</summary>
public static class DesktopApplication
{
    private static readonly ConditionalWeakTable<
        DorotiApplicationDescriptor,
        DesktopApplicationDefinition
    > Definitions = new();

    public static DorotiApplicationDescriptor Configure<TStartup>(
        DorotiApplicationDescriptor application
    )
        where TStartup : IDorotiDesktopApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(application);
        var builder = new DesktopApplicationBuilder(application);
        new TStartup().Configure(builder);
        var definition = builder.Build();
        // The companion selects native options; the application keeps its single root factory.
        var descriptor = application with
        {
            ViewConfiguration = ToViewConfiguration(
                definition.MainWindow.Options,
                definition.LifetimePolicy
            ) with { Navigation = application.ViewConfiguration.Navigation },
        };
        Definitions.Add(descriptor, definition);
        return descriptor;
    }

    public static bool TryGetDefinition(
        DorotiApplicationDescriptor descriptor,
        out DesktopApplicationDefinition? definition
    ) => Definitions.TryGetValue(descriptor, out definition);

    public static DesktopApplicationDefinition FromViewConfiguration(DorotiApplicationDescriptor descriptor)
    {
        var view = descriptor.ViewConfiguration;
        var appearance = view.ResolveAppearance();
        var material = appearance.ResolveBackdrop(false);
        return new(
            new WindowCreateOptions
            {
                Options = new WindowOptions
                {
                    Title = view.title,
                    Size = view.logicalSize,
                    Appearance = new WindowAppearanceOptions
                    {
                        BackgroundColor = view.backgroundColor ?? new Color(0xffffffff),
                        DarkBackgroundColor = view.darkBackgroundColor,
                        ThemeSource =
                            material.theme == Ui.WindowBackdropTheme.system
                                ? WindowThemeSource.System
                                : WindowThemeSource.Explicit,
                        Theme =
                            material.theme == Ui.WindowBackdropTheme.dark
                                ? WindowTheme.Dark
                                : WindowTheme.Light,
                        Backdrop = ToWindowBackdrop(material),
                        MacOSBackdrop = appearance.macOSBackdrop is { } mac ? ToWindowBackdrop(mac) : null,
                        TitleBar = new()
                        {
                            Background =
                                appearance.titlebarStyle == Ui.WindowTitlebarStyle.solid
                                    ? WindowTitleBarBackground.Solid
                                    : WindowTitleBarBackground.Backdrop,
                        },
                    },
                },
            },
            view.terminateAfterLastWindowClosed
                ? WindowLifetimePolicy.OnLastWindowClosed
                : WindowLifetimePolicy.Explicit
        );
    }

    private static WindowBackdropOptions ToWindowBackdrop(Ui.WindowBackdropOptions value) =>
        new()
        {
            Mode = value.mode switch
            {
                Ui.WindowBackdropMode.acrylic or Ui.WindowBackdropMode.experimentalAcrylic =>
                    WindowBackdropMode.Acrylic,
                Ui.WindowBackdropMode.liquidGlass => WindowBackdropMode.LiquidGlass,
                Ui.WindowBackdropMode.transparent => WindowBackdropMode.Transparent,
                Ui.WindowBackdropMode.solid => WindowBackdropMode.Solid,
                _ => WindowBackdropMode.System,
            },
            Fallback =
                value.fallback == Ui.WindowBackdropFallback.solid
                    ? WindowBackdropFallback.Solid
                    : WindowBackdropFallback.Transparent,
            AcrylicKind = (WindowAcrylicKind)value.acrylicKind,
            TintColor = value.tintColor,
            TintOpacity = value.tintOpacity,
            LuminosityOpacity = value.luminosityOpacity,
        };

    public static Ui.WindowBackdropOptions ToPlatformBackdrop(
        WindowAppearanceOptions appearance,
        bool isMacOS = false
    )
    {
        var material = (isMacOS ? appearance.MacOSBackdrop : null) ?? appearance.Backdrop;
        return new(
            material.Mode switch
            {
                WindowBackdropMode.Acrylic => Ui.WindowBackdropMode.acrylic,
                WindowBackdropMode.LiquidGlass => Ui.WindowBackdropMode.liquidGlass,
                WindowBackdropMode.Transparent => Ui.WindowBackdropMode.transparent,
                WindowBackdropMode.Solid => Ui.WindowBackdropMode.solid,
                _ => Ui.WindowBackdropMode.system,
            },
            material.Fallback == WindowBackdropFallback.Solid
                ? Ui.WindowBackdropFallback.solid
                : Ui.WindowBackdropFallback.transparent,
            (Ui.WindowAcrylicKind)material.AcrylicKind,
            appearance.ThemeSource == WindowThemeSource.System ? Ui.WindowBackdropTheme.system
                : appearance.Theme == WindowTheme.Dark ? Ui.WindowBackdropTheme.dark
                : Ui.WindowBackdropTheme.light,
            material.TintColor,
            material.TintOpacity,
            material.LuminosityOpacity
        );
    }

    public static DorotiViewConfiguration ToViewConfiguration(
        WindowOptions options,
        WindowLifetimePolicy lifetimePolicy
    ) =>
        new(
            options.Title,
            options.Size,
            options.Appearance.BackgroundColor,
            darkBackgroundColor: options.Appearance.DarkBackgroundColor,
            terminateAfterLastWindowClosed: lifetimePolicy
                == WindowLifetimePolicy.OnLastWindowClosed,
            appearance: new Ui.WindowAppearanceOptions(
                ToPlatformBackdrop(options.Appearance),
                options.Appearance.TitleBar.Background == WindowTitleBarBackground.Solid
                    ? Ui.WindowTitlebarStyle.solid
                    : Ui.WindowTitlebarStyle.unified,
                options.Appearance.MacOSBackdrop is null ? null : ToPlatformBackdrop(options.Appearance, true)
            )
        );
}
