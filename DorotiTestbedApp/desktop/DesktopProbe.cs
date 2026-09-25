using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

internal static class DesktopProbe
{
    internal static async Task RunAsync(
        DesktopWindowContext context,
        WindowState beforeShow,
        string path,
        CancellationToken ct
    )
    {
        var window = context.Window;
        var states = new Dictionary<string, object>
        {
            ["beforeShow"] = Snapshot(beforeShow),
            ["shown"] = Snapshot(window.State),
        };
        await window.SetTitleAsync("Doroti Desktop Probe", ct);
        states["resize"] = Snapshot(await window.SetSizeAsync(new Size(500, 650), ct));
        await window.SetAlwaysOnTopAsync(true, ct);
        await window.SetAlwaysOnTopAsync(false, ct);
        if (OperatingSystem.IsMacOS())
        {
            try
            {
                await window.SetSkipTaskbarAsync(true, ct);
                throw new InvalidOperationException("AppKit accepted per-window Dock hiding.");
            }
            catch (NotSupportedException)
            {
                states["skipTaskbarRejected"] = true;
            }
        }
        else
            await window.SetSkipTaskbarAsync(true, ct);
        await window.SetSkipTaskbarAsync(false, ct);
        await window.SetResizableAsync(false, ct);
        await window.SetResizableAsync(true, ct);
        states["maximized"] = Snapshot(await window.MaximizeAsync(ct));
        states["restored"] = Snapshot(await window.RestoreAsync(ct));
        states["minimized"] = Snapshot(await window.MinimizeAsync(ct));
        await window.RestoreAsync(ct);
        states["fullscreen"] = Snapshot(await window.SetFullScreenAsync(true, ct));
        states["windowed"] = Snapshot(await window.SetFullScreenAsync(false, ct));
        await window.HideAsync(ct);
        states["hidden"] = Snapshot(window.State);
        await window.ShowAsync(ct);
        await window.FocusAsync(ct);
        await window.SetSizeAsync(new Size(450, 800), ct);
        var appearance = window.State.RequestedAppearance;
        var apply = await window.ApplyAppearanceAsync(
            appearance with
            {
                ThemeSource = WindowThemeSource.Explicit,
                Theme = WindowTheme.Dark,
                Backdrop =
                    !OperatingSystem.IsMacOS()
                    && appearance.Backdrop.Mode == Doroti.Desktop.WindowBackdropMode.Acrylic
                        ? appearance.Backdrop with
                        {
                            TintOpacity = 0.4,
                            LuminosityOpacity = 0.6,
                        }
                        : appearance.Backdrop,
            },
            ct
        );
        var reset = await window.ApplyAppearanceAsync(appearance, ct);
        states["appearance"] = new
        {
            changed = apply.Status.ToString(),
            reset = reset.Status.ToString(),
        };
        if (OperatingSystem.IsMacOS())
        {
            var modes = new Dictionary<string, object>();
            foreach (
                var mode in new[]
                {
                    Doroti.Desktop.WindowBackdropMode.Solid,
                    Doroti.Desktop.WindowBackdropMode.Transparent,
                    Doroti.Desktop.WindowBackdropMode.Acrylic,
                    Doroti.Desktop.WindowBackdropMode.LiquidGlass,
                }
            )
            {
                var result = await window.ApplyAppearanceAsync(
                    appearance with
                    {
                        MacOSBackdrop = null,
                        Backdrop = new() { Mode = mode },
                        TitleBar = new() { Background = WindowTitleBarBackground.System },
                    },
                    ct
                );
                modes[mode.ToString()] = new
                {
                    status = result.Status.ToString(),
                    effective = result.State.EffectiveAppearance.Appearance.Backdrop.Mode.ToString(),
                };
            }
            await window.ApplyAppearanceAsync(appearance, ct);
            states["materialTransitions"] = modes;
        }
        states["final"] = Snapshot(window.State);
        var closes = 0;
        var closeSubscription = window.RegisterClosing(
            (_, _) =>
            {
                closes++;
                File.WriteAllText(path + ".close", closes.ToString());
                return Task.FromResult(
                    closes == 1 ? WindowCloseDecision.Cancel : WindowCloseDecision.Allow
                );
            }
        );
        context.Windows.WindowClosed += Closed;
        void Closed(DorotiWindowController closed)
        {
            if (closed.Id != window.Id)
                return;
            File.WriteAllText(
                path + ".closed",
                JsonSerializer.Serialize(
                    new
                    {
                        closed.State.Closed,
                        remaining = context.Windows.GetWindows().Count,
                        lifetime = context.Windows.LifetimePolicy.ToString(),
                    }
                )
            );
            closeSubscription.Dispose();
            context.Windows.WindowClosed -= Closed;
        }
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true })
        );
    }

    private static object Snapshot(WindowState state) =>
        new
        {
            size = new[] { state.ClientSize.width, state.ClientSize.height },
            state.Scale,
            state.Visible,
            state.Focused,
            presentation = state.PresentationState.ToString(),
            state.Closed,
        };
}
