using System.Text.Json;
using Doroti.Desktop;
using Doroti.Ui;

namespace DorotiTestbedApp.Desktop;

/// <summary>UIKit scene defaults are explicit; this sample does not inherit AppKit materials.</summary>
public sealed class MacCatalystDesktopStartup : IDorotiDesktopApplicationStartup
{
    public void Configure(DesktopApplicationBuilder desktop)
    {
        desktop.LifetimePolicy = WindowLifetimePolicy.Explicit;
        desktop.UseMainWindow(
            desktop.LegacyMainWindow with
            {
                Options = new WindowOptions
                {
                    Title = "Doroti Catalyst Desktop",
                    Size = new Size(600, 500),
                    MinimumSize = new Size(350, 300),
                    StartupVisibility = WindowStartupVisibility.PlatformDefault,
                    Appearance = new() { BackgroundColor = new Color(0xffffffff) },
                },
                OnCreated = async (context, ct) =>
                {
                    await context.Window.WaitUntilReadyToShowAsync(ct);
                    if (
                        Environment.GetEnvironmentVariable("DOROTI_CATALYST_DESKTOP_PROBE") is
                        { Length: > 0 } path
                    )
                        await ProbeAsync(context, path, ct);
                },
            }
        );
    }

    private static async Task ProbeAsync(
        DesktopWindowContext context,
        string path,
        CancellationToken ct
    )
    {
        var window = context.Window;
        var states = new Dictionary<string, object>();
        object Snapshot() =>
            new
            {
                size = new[] { window.State.ClientSize.width, window.State.ClientSize.height },
                window.State.Scale,
                window.State.Visible,
                window.State.Focused,
            };
        states["initial"] = Snapshot();
        await window.SetTitleAsync("Doroti Catalyst Desktop Probe", ct);
        await window.SetSizeAsync(new Size(550, 475), ct);
        states["intermediate"] = Snapshot();
        await window.SetSizeAsync(new Size(500, 450), ct);
        states["resized"] = Snapshot();
        await window.SetResizableAsync(false, ct);
        await window.SetSizeAsync(new Size(520, 460), ct);
        states["fixedResize"] = Snapshot();
        await window.SetResizableAsync(true, ct);
        await window.SetSizeAsync(new Size(500, 450), ct);
        await window.SetMinimumSizeAsync(new Size(320, 280), ct);
        await window.SetMaximumSizeAsync(new Size(900, 700), ct);
        await window.ShowAsync(ct);
        var appearance = window.State.RequestedAppearance;
        var dark = await window.ApplyAppearanceAsync(
            appearance with
            {
                ThemeSource = WindowThemeSource.Explicit,
                Theme = WindowTheme.Dark,
            },
            ct
        );
        var reset = await window.ApplyAppearanceAsync(appearance, ct);
        states["appearance"] = new
        {
            dark = dark.Status.ToString(),
            reset = reset.Status.ToString(),
        };
        states["canCancelNativeClose"] = window.Capabilities.CanCancelNativeClose;
        var unsupported = new List<string>();
        async Task Rejected(string name, Func<Task> action)
        {
            try
            {
                await action();
                throw new InvalidOperationException(name + " unexpectedly succeeded");
            }
            catch (NotSupportedException)
            {
                unsupported.Add(name);
            }
        }
        await Rejected(
            "SkipTaskbar",
            async () =>
            {
                await window.SetSkipTaskbarAsync(true, ct);
            }
        );
        await Rejected(
            "Hide",
            async () =>
            {
                await window.HideAsync(ct);
            }
        );
        await Rejected(
            "FullScreen",
            async () =>
            {
                await window.SetFullScreenAsync(true, ct);
            }
        );
        var acrylic = await window.ApplyAppearanceAsync(
            appearance with
            {
                Backdrop = new() { Mode = Doroti.Desktop.WindowBackdropMode.Acrylic },
            },
            ct
        );
        if (acrylic.Status != WindowApplyStatus.Rejected)
            throw new InvalidOperationException("Acrylic unexpectedly succeeded");
        states["rejected"] = unsupported;
        states["acrylicRejected"] = true;
        states["final"] = Snapshot();
        var closes = 0;
        var subscription = window.RegisterClosing(
            (_, _) =>
            {
                File.WriteAllText(path + ".close", (++closes).ToString());
                return Task.FromResult(
                    closes == 1 ? WindowCloseDecision.Cancel : WindowCloseDecision.Allow
                );
            }
        );
        context.Windows.WindowClosed += Closed;
        void Closed(DorotiWindowController closed)
        {
            File.WriteAllText(
                path + ".closed",
                JsonSerializer.Serialize(
                    new
                    {
                        closed.State.Closed,
                        remaining = context.Windows.GetWindows().Count,
                        closingCallbacks = closes,
                    }
                )
            );
            context.Windows.WindowClosed -= Closed;
            subscription.Dispose();
        }
        File.WriteAllText(
            path,
            JsonSerializer.Serialize(states, new JsonSerializerOptions { WriteIndented = true })
        );
        while (!File.Exists(path + ".native.json"))
            await Task.Delay(50, ct);
        if (Environment.GetEnvironmentVariable("DOROTI_CATALYST_NATIVE_CLOSE") == "1")
            return;
        File.WriteAllText(path + ".stage", "before first close");
        if (await window.CloseAsync(ct))
            throw new InvalidOperationException("First API close was not canceled");
        File.WriteAllText(path + ".stage", "before second close");
        // The accepted close cancels the startup token as part of the window lifetime.
        await window.CloseAsync();
        File.WriteAllText(path + ".stage", "after second close");
    }
}
