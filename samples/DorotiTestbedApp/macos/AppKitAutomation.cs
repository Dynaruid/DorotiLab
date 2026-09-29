#if MACOS
using AppKit;
using Doroti.Host.Maui;
using System.Text.Json;

namespace DorotiTestbedApp.MacOS;

/// <summary>Opt-in testbed control of this process's windows, never other applications.</summary>
internal static class AppKitAutomation
{
    internal static async Task RunAsync(string directory)
    {
        var request = Path.Combine(directory, "request.json");
        var response = Path.Combine(directory, "response.json");
        while (true)
        {
            await Task.Delay(50);
            if (!File.Exists(request)) continue;
            try
            {
                using var json = JsonDocument.Parse(File.ReadAllText(request));
                File.Delete(request);
                var op = json.RootElement.GetProperty("op").GetString();
                await AppKitUi.Invoke(() =>
                {
                    List<NSWindow> native = [];
                    NSApplication.SharedApplication.EnumerateWindows(NSWindowListOptions.OrderedFrontToBack,
                        (NSWindow window, ref bool stop) => native.Add(window));
                    var windows = native.Where(w => w.ContentView is { } view && Descendants(view).OfType<DorotiMacOSMetalView>().Any()).ToArray();
                    var target = json.RootElement.TryGetProperty("id", out var id)
                        ? windows.FirstOrDefault(w => (long)w.WindowNumber == id.GetInt64())
                        : windows.FirstOrDefault(w => w.IsKeyWindow) ?? windows.FirstOrDefault();
                    if (op == "exit") NSApplication.SharedApplication.Terminate(NSApplication.SharedApplication);
                    if (op == "focus" && target is not null)
                    {
                        NSApplication.SharedApplication.Activate();
                        target.MakeKeyAndOrderFront(NSApplication.SharedApplication);
                    }
                    if (op == "close")
                    {
                        target?.PerformClose(NSApplication.SharedApplication);
                        // Close can synchronously retire the device. Never inspect its old views.
                        File.WriteAllText(response, "[]");
                        return;
                    }
                    else if (op == "resize" && target is not null)
                        target.SetContentSize(new CoreGraphics.CGSize(json.RootElement.GetProperty("width").GetDouble(), json.RootElement.GetProperty("height").GetDouble()));
                    var data = windows.Select(w => new
                    {
                        id = (long)w.WindowNumber, w.Title, w.IsVisible, w.IsKeyWindow,
                        width = (double)w.ContentLayoutRect.Width, height = (double)w.ContentLayoutRect.Height,
                        scale = (double)w.BackingScaleFactor,
                        views = Descendants(w.ContentView!).OfType<DorotiMacOSMetalView>().Select(view =>
                            view.CaptureSnapshot(new MauiSurfaceSnapshot(0, 0, 1, 0, 0, 0, "probe", "probe"))).ToArray(),
                    }).ToArray();
                    File.WriteAllText(response + ".tmp", JsonSerializer.Serialize(data));
                    File.Move(response + ".tmp", response, true);
                });
            }
            catch (Exception error) { File.WriteAllText(response + ".error", error.ToString()); }
        }
    }
    private static IEnumerable<NSView> Descendants(NSView view)
    {
        yield return view;
        foreach (var child in view.Subviews)
            foreach (var descendant in Descendants(child)) yield return descendant;
    }
}
#endif
