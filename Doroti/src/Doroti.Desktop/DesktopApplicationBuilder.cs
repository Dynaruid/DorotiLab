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
        LegacyMainWindow = DesktopApplication.FromLegacy(application).MainWindow;

    /// <summary>Explicit migration input; reuse its content factory without attaching a second root.</summary>
    public WindowCreateOptions LegacyMainWindow { get; }
    private WindowCreateOptions? _main;
    public WindowLifetimePolicy LifetimePolicy { get; set; } =
        WindowLifetimePolicy.OnLastWindowClosed;

    public void UseMainWindow(WindowCreateOptions options)
    {
        if (_main is not null)
            throw new InvalidOperationException(
                "UseMainWindow has already been called; options/content have one owner."
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
        if (application.LaunchContext.Target is not ("Windows" or "macOS" or "Linux"))
            throw new PlatformNotSupportedException(
                "Desktop startup requires a native desktop runner."
            );
        var builder = new DesktopApplicationBuilder(application);
        new TStartup().Configure(builder);
        var definition = builder.Build();
        // The common app describes Web/mobile. The companion owns desktop options and content.
        var descriptor = application with
        {
            ViewConfiguration = ToViewConfiguration(
                definition.MainWindow.Options,
                definition.LifetimePolicy
            ),
            // A host must explicitly consume the desktop definition and install its
            // content. Reflection/manual bootstrap cannot silently fall back to the common app.
            EntrypointFactory = () =>
                throw new PlatformNotSupportedException(
                    "This host did not attach the registered desktop window adapter."
                ),
        };
        Definitions.Add(descriptor, definition);
        return descriptor;
    }

    public static bool TryGetDefinition(
        DorotiApplicationDescriptor descriptor,
        out DesktopApplicationDefinition? definition
    ) => Definitions.TryGetValue(descriptor, out definition);

    public static DesktopApplicationDefinition FromLegacy(DorotiApplicationDescriptor descriptor)
    {
        var view = descriptor.ViewConfiguration;
        var legacy = view.ResolveAppearance();
        var material = legacy.ResolveBackdrop(false);
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
                        Backdrop = FromLegacy(material),
                        MacOSBackdrop = legacy.macOSBackdrop is { } mac ? FromLegacy(mac) : null,
                        TitleBar = new()
                        {
                            Background =
                                legacy.titlebarStyle == Ui.WindowTitlebarStyle.solid
                                    ? WindowTitleBarBackground.Solid
                                    : WindowTitleBarBackground.Backdrop,
                        },
                    },
                },
                Content = WindowContent.FromEntrypoint(descriptor.EntrypointFactory),
            },
            view.terminateAfterLastWindowClosed
                ? WindowLifetimePolicy.OnLastWindowClosed
                : WindowLifetimePolicy.Explicit
        );
    }

    private static WindowBackdropOptions FromLegacy(Ui.WindowBackdropOptions value) =>
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

    public static Ui.WindowBackdropOptions ToLegacy(
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
                ToLegacy(options.Appearance),
                options.Appearance.TitleBar.Background == WindowTitleBarBackground.Solid
                    ? Ui.WindowTitlebarStyle.solid
                    : Ui.WindowTitlebarStyle.unified,
                options.Appearance.MacOSBackdrop is null ? null : ToLegacy(options.Appearance, true)
            )
        );
}
