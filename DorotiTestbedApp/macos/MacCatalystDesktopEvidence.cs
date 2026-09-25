#if MACCATALYST
using System.Text.Json;
using Doroti.Host.Maui;
using UIKit;

namespace DorotiTestbedApp.MacCatalyst;

internal static class MacCatalystDesktopEvidence
{
    internal static async Task RunAsync()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_CATALYST_DESKTOP_PROBE");
        if (string.IsNullOrEmpty(path))
            return;
        try
        {
            using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            while (!File.Exists(path))
            {
                File.WriteAllText(
                    path + ".boot.json",
                    JsonSerializer.Serialize(
                        new
                        {
                            scenes = UIApplication
                                .SharedApplication.ConnectedScenes.OfType<UIWindowScene>()
                                .Select(s => new
                                {
                                    name = s.Session.Configuration.Name,
                                    sceneDelegate = s.Delegate?.GetType().FullName,
                                    systemFrame = s.EffectiveGeometry.SystemFrame.ToString(),
                                    state = s.ActivationState.ToString(),
                                })
                                .ToArray(),
                            windows = Microsoft
                                .Maui.Controls.Application.Current?.Windows.Select(w => new
                                {
                                    w.Title,
                                    handler = w.Handler?.GetType().FullName,
                                    native = (
                                        w.Handler?.PlatformView as UIWindow
                                    )?.Bounds.ToString(),
                                    hidden = (w.Handler?.PlatformView as UIWindow)?.Hidden,
                                    loaded = (
                                        (w.Page as ContentPage)?.Content as DorotiMauiSurface
                                    )?.IsLoaded,
                                })
                                .ToArray(),
                        }
                    )
                );
                await Task.Delay(100, deadline.Token);
            }
            var native = Microsoft
                .Maui.Controls.Application.Current!.Windows.Select(w => w.Handler?.PlatformView)
                .OfType<UIWindow>()
                .Single();
            var scene = native.WindowScene!;
            using var states = JsonDocument.Parse(File.ReadAllText(path));
            Check(
                !states.RootElement.GetProperty("canCancelNativeClose").GetBoolean(),
                "native close capability"
            );
            Check(
                states.RootElement.GetProperty("appearance").GetProperty("dark").GetString()
                    == "Applied",
                "theme apply"
            );
            Check(
                states.RootElement.GetProperty("appearance").GetProperty("reset").GetString()
                    == "Applied",
                "theme reset"
            );
            var surface = Views(native).OfType<DorotiUIKitGraphiteView>().Single();
            Check(
                Math.Abs(surface.Bounds.Width - 500) < 1
                    && Math.Abs(surface.Bounds.Height - 450) < 1,
                "native render view size " + surface.Bounds
            );
            Check(scene.Title == "Doroti Catalyst Desktop Probe", "scene title");
            Check(
                scene.SizeRestrictions!.MinimumSize.Width == 320
                    && scene.SizeRestrictions.MinimumSize.Height == 280,
                "minimum size"
            );
            Check(
                scene.SizeRestrictions.MaximumSize.Width == 900
                    && scene.SizeRestrictions.MaximumSize.Height == 700,
                "maximum size"
            );
            File.WriteAllText(
                path + ".native.json",
                JsonSerializer.Serialize(
                    new
                    {
                        result = "PASS",
                        scene.Title,
                        size = new[]
                        {
                            (double)surface.Bounds.Width,
                            (double)surface.Bounds.Height,
                        },
                        minimum = new[]
                        {
                            (double)scene.SizeRestrictions.MinimumSize.Width,
                            (double)scene.SizeRestrictions.MinimumSize.Height,
                        },
                        scale = (double)native.Screen.Scale,
                        native.Hidden,
                    },
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
            if (Environment.GetEnvironmentVariable("DOROTI_CATALYST_NATIVE_CLOSE") == "1")
                UIApplication.SharedApplication.RequestSceneSessionDestruction(
                    scene.Session,
                    null,
                    error => File.WriteAllText(path + ".error", error.ToString())
                );
        }
        catch (Exception error)
        {
            File.WriteAllText(path + ".error", error.ToString());
        }
    }

    private static IEnumerable<UIView> Views(UIView root)
    {
        yield return root;
        foreach (var child in root.Subviews)
        foreach (var view in Views(child))
            yield return view;
    }

    private static void Check(bool ok, string message)
    {
        if (!ok)
            throw new InvalidOperationException("Catalyst native assertion: " + message);
    }
}
#endif
