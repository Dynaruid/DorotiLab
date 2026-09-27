using Doroti.Desktop;
using Doroti.Ui;
using Appearance = Doroti.Desktop.WindowAppearanceOptions;
using Backdrop = Doroti.Desktop.WindowBackdropOptions;
using BackdropMode = Doroti.Desktop.WindowBackdropMode;

namespace DorotiTestbedApp.Desktop;

public sealed class DesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        var mode = Environment.GetEnvironmentVariable("DOROTI_DESKTOP_SAMPLE") ?? "legacy";
        var legacy = desktop.LegacyMainWindow;
        if (Environment.GetEnvironmentVariable("DOROTI_DESKTOP_LIFETIME") == "Explicit")
            desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
        var options =
            mode == "legacy"
                ? legacy.Options
                : new WindowOptions
                {
                    Title = "Doroti Desktop · Sudoku",
                    Size = new Size(450, 800),
                    MinimumSize = new Size(350, 500),
                    Centered = true,
                    StartupVisibility = WindowStartupVisibility.Manual,
                    Appearance = new Appearance
                    {
                        BackgroundColor = new Color(mode == "acrylic" ? 0x00000000 : 0xffffffff),
                        Backdrop = new Backdrop
                        {
                            Mode = mode == "acrylic" ? BackdropMode.Acrylic : BackdropMode.System,
                            AcrylicKind =
                                mode == "acrylic"
                                    ? Doroti.Desktop.WindowAcrylicKind.Thin
                                    : Doroti.Desktop.WindowAcrylicKind.Default,
                        },
                        TitleBar = new()
                        {
                            Background =
                                mode == "acrylic"
                                    ? WindowTitleBarBackground.Backdrop
                                    : WindowTitleBarBackground.System,
                        },
                    },
                };
        if (
            options.Appearance.Backdrop.Mode != BackdropMode.Acrylic
            && options.Appearance.TitleBar.Background == WindowTitleBarBackground.Backdrop
        )
            options = options with
            {
                Appearance = options.Appearance with
                {
                    TitleBar = options.Appearance.TitleBar with
                    {
                        Background = WindowTitleBarBackground.System,
                    },
                },
            };
        if (
            Enum.TryParse<WindowTitleBarBackground>(
                Environment.GetEnvironmentVariable("DOROTI_DESKTOP_CAPTION"),
                out var caption
            )
        )
            options = options with
            {
                Appearance = options.Appearance with
                {
                    TitleBar = options.Appearance.TitleBar with { Background = caption },
                },
            };
        desktop.UseMainWindow(
            legacy with
            {
                Options = options,
                OnCreated = async (context, cancellationToken) =>
                {
                    await context.Window.EnsureInitializedAsync(cancellationToken);
                    await context.Window.WaitUntilReadyToShowAsync(cancellationToken);
                    var beforeShow = context.Window.State;
                    // The WhenReady auto-show and this hook run concurrently. Explicitly
                    // await the idempotent show before asking the native host for focus.
                    await context.Window.ShowAsync(cancellationToken);
                    await context.Window.FocusAsync(cancellationToken);
                    if (
                        Environment.GetEnvironmentVariable("DOROTI_DESKTOP_PROBE") is
                        { Length: > 0 } probe
                    )
                        await DesktopProbe.RunAsync(context, beforeShow, probe, cancellationToken);
                },
            }
        );
    }
}
