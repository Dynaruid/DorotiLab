#if MACOS
using AppKit;
using Doroti.Desktop;
using Doroti.Host.Maui;
using Foundation;
using System.Text.Json;

namespace DorotiTestbedApp.MacOS;

/// <summary>Opt-in native assertions for the Desktop adapter, separate from renderer evidence.</summary>
internal static class MacOSDesktopWindowEvidence
{
    internal static async Task RunAsync()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_DESKTOP_PROBE");
        if (string.IsNullOrEmpty(path))
            return;
        try
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            while (!File.Exists(path))
                await Task.Delay(50, deadline.Token);
            var native = Microsoft
                .Maui.Controls.Application.Current!.Windows.Select(w => w.Handler?.PlatformView)
                .OfType<NSWindow>()
                .Single(w => w.Title == "Doroti Desktop Probe");
            using var managed = JsonDocument.Parse(File.ReadAllText(path));
            var root = managed.RootElement;
            var manual = Environment.GetEnvironmentVariable("DOROTI_DESKTOP_SAMPLE") != "legacy";
            if (manual)
                Check(
                    !root.GetProperty("beforeShow").GetProperty("Visible").GetBoolean(),
                    "hidden readiness"
                );
            Check(
                root.GetProperty("resize").GetProperty("size")[0].GetDouble() == 500,
                "client width"
            );
            Check(
                root.GetProperty("resize").GetProperty("size")[1].GetDouble() == 650,
                "client height"
            );
            Check(
                root.GetProperty("fullscreen").GetProperty("presentation").GetString()
                    == "FullScreen",
                "full screen completion"
            );
            Check(
                root.GetProperty("windowed").GetProperty("presentation").GetString()
                    != "FullScreen",
                "full screen exit"
            );
            Check(
                root.GetProperty("minimized").GetProperty("presentation").GetString()
                    == "Minimized",
                "minimize completion"
            );
            Check(!root.GetProperty("hidden").GetProperty("Visible").GetBoolean(), "hide");
            Check(
                root.GetProperty("appearance").GetProperty("changed").GetString() == "Applied",
                "appearance apply"
            );
            Check(
                root.GetProperty("appearance").GetProperty("reset").GetString() == "Applied",
                "appearance reset"
            );
            Check(
                native.ContentLayoutRect.Width == 450 && native.ContentLayoutRect.Height == 800,
                "native client size"
            );
            if (manual)
                Check(
                    native.ContentMinSize.Width == 350
                        && native.ContentMinSize.Height
                            - (native.ContentView!.Bounds.Height - native.ContentLayoutRect.Height)
                            == 500,
                    "native minimum size"
                );
            if (root.TryGetProperty("materialTransitions", out var transitions))
                foreach (var transition in transitions.EnumerateObject())
                    Check(
                        transition.Value.GetProperty("status").GetString() == "Applied",
                        "material " + transition.Name
                    );
            var quit = Environment.GetEnvironmentVariable("DOROTI_DESKTOP_PROBE_QUIT") == "1";
            if (quit)
                NSApplication.SharedApplication.Terminate(null);
            else
                native.PerformClose(null);
            while (!File.Exists(path + ".close"))
                await Task.Delay(50, deadline.Token);
            await Task.Delay(100, deadline.Token);
            Check(
                File.ReadAllText(path + ".close") == "1" && native.IsVisible,
                "native close canceled"
            );
            File.WriteAllText(
                path + ".native.json",
                JsonSerializer.Serialize(
                    new
                    {
                        result = "PASS",
                        closeKind = quit ? "application-quit" : "window-close",
                        windowNumber = (long)native.WindowNumber,
                        client = new[]
                        {
                            (double)native.ContentLayoutRect.Width,
                            (double)native.ContentLayoutRect.Height,
                        },
                        minimum = new[]
                        {
                            (double)native.ContentMinSize.Width,
                            (double)native.ContentMinSize.Height,
                        },
                        scale = (double)native.BackingScaleFactor,
                    },
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
            if (Environment.GetEnvironmentVariable("DOROTI_DESKTOP_CAPTURE") == "1")
            {
                File.WriteAllText(path + ".capture", ((long)native.WindowNumber).ToString());
                while (File.Exists(path + ".capture"))
                    await Task.Delay(50, deadline.Token);
            }
            // Parent probe also requires a second close decision and normal process exit.
            if (quit)
                NSApplication.SharedApplication.Terminate(null);
            else
                native.PerformClose(null);
            if (
                !quit
                && Environment.GetEnvironmentVariable("DOROTI_DESKTOP_LIFETIME") == "Explicit"
            )
            {
                while (!File.Exists(path + ".closed"))
                    await Task.Delay(50, deadline.Token);
                await Task.Delay(100, deadline.Token);
                Check(
                    !native.IsVisible,
                    "Explicit closes the window and keeps the application running"
                );
                File.WriteAllText(path + ".explicit", "PASS");
                NSApplication.SharedApplication.Terminate(null);
            }
        }
        catch (Exception error)
        {
            File.WriteAllText(path + ".native-error.txt", error.ToString());
        }
    }

    private static void Check(bool condition, string name)
    {
        if (!condition)
            throw new InvalidOperationException("AppKit Desktop assertion failed: " + name);
    }
}
#endif
