using Doroti.Host.Maui;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DorotiTestbedApp.Android;

internal sealed record AndroidShutdownProbeResult(string Status, bool SharedStopCompletion,
    bool ViewDisposed, bool GpuRetiredBeforeCompletion, MauiHostDiagnostics Before,
    MauiSurfaceSnapshot After, string PhysicalInput);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(AndroidShutdownProbeResult))]
internal sealed partial class AndroidShutdownProbeJsonContext : JsonSerializerContext;

internal static class AndroidShutdownProbe
{
    internal static async Task RunAsync(MainActivity activity)
    {
        var output = System.IO.Path.Combine(activity.ExternalCacheDir!.AbsolutePath!, "shutdown-probe.json");
        try
        {
            var app = (DorotiMauiApplication)Microsoft.Maui.Controls.Application.Current!;
            DorotiMauiSurface? surface = null;
            for (var attempt = 0; attempt < 300; attempt++)
            {
                surface = app.Windows.FirstOrDefault() is { } window
                    ? Descendants(window).OfType<DorotiMauiSurface>().FirstOrDefault() : null;
                if (surface?.Diagnostics?.Frame.Presented > 0) break;
                await Task.Delay(100);
            }
            if (surface?.Diagnostics?.Frame.Presented is not > 0)
                throw new TimeoutException("No rendered Android view before shutdown.");
            var before = surface.Diagnostics;
            var native = (Descendants(surface).OfType<DorotiGraphiteView>().Single().Handler?.PlatformView as DorotiAndroidViewContainer)?.Surface
                ?? throw new InvalidOperationException("Shutdown requires the Android Vulkan surface.");
            var stop = app.StopAsync();
            if (!ReferenceEquals(stop, app.StopAsync()))
                throw new InvalidOperationException("Concurrent Stop did not join one completion.");
            await stop.WaitAsync(TimeSpan.FromSeconds(40));
            if (surface.Diagnostics is not null || !native.RetireAsync().IsCompletedSuccessfully)
                throw new InvalidOperationException("Stop completed before view/GPU retirement.");
            var after = native.CaptureSnapshot(new MauiSurfaceSnapshot(0, 0, 1, 0, 0, 0, "probe", "probe"));
            if (after.NativeFramePipeline?.PendingGpuFrames != 0)
                throw new InvalidOperationException("Stop retained pending GPU frames.");
            File.WriteAllText(output, JsonSerializer.Serialize(new AndroidShutdownProbeResult(
                "PASS", true, true, true, before!, after, "notVerified"),
                AndroidShutdownProbeJsonContext.Default.AndroidShutdownProbeResult));
        }
        catch (Exception error) { File.WriteAllText(output + ".error", error.ToString()); }
    }

    private static IEnumerable<Element> Descendants(Element element)
    {
        yield return element;
        if (element is IVisualTreeElement tree)
            foreach (var child in tree.GetVisualChildren().OfType<Element>())
                foreach (var descendant in Descendants(child)) yield return descendant;
    }
}
