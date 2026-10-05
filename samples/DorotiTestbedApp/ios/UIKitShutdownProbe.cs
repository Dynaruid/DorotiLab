using Doroti.Host.Maui;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DorotiTestbedApp.iOS;

internal sealed record UIKitShutdownProbeResult(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("sharedStopCompletion")] bool SharedStopCompletion,
    [property: JsonPropertyName("viewDisposed")] bool ViewDisposed,
    [property: JsonPropertyName("gpuRetiredBeforeCompletion")] bool GpuRetiredBeforeCompletion,
    [property: JsonPropertyName("before")] MauiHostDiagnostics Before,
    [property: JsonPropertyName("after")] MauiSurfaceSnapshot After,
    [property: JsonPropertyName("processTermination")] string ProcessTermination,
    [property: JsonPropertyName("physicalInput")] string PhysicalInput);

[JsonSerializable(typeof(UIKitShutdownProbeResult))]
internal sealed partial class UIKitShutdownProbeJsonContext : JsonSerializerContext;

internal static class UIKitShutdownProbe
{
    internal static async Task RunAsync(string output)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var app = (DorotiMauiApplication)Microsoft.Maui.Controls.Application.Current!;
                DorotiMauiSurface? surface = null;
                for (var attempt = 0; attempt < 300; attempt++)
                {
                    surface = app.Windows.Count == 0 ? null : Descendants(app.Windows[0]).OfType<DorotiMauiSurface>().FirstOrDefault();
                    if (surface?.Diagnostics?.Frame.Presented > 0) break;
                    await Task.Delay(100);
                }
                if (surface?.Diagnostics?.Frame.Presented is not > 0)
                    throw new TimeoutException("No rendered view before application shutdown.");
                var before = surface.Diagnostics;
                var native = Descendants(surface).OfType<DorotiGraphiteView>().Single().Handler?.PlatformView as DorotiUIKitGraphiteView;
                if (native is null) throw new InvalidOperationException("Shutdown probe requires the Graphite UIKit surface.");
                var stop = app.StopAsync();
                if (!ReferenceEquals(stop, app.StopAsync()))
                    throw new InvalidOperationException("Concurrent shutdown did not join one completion.");
                await stop.WaitAsync(TimeSpan.FromSeconds(40));
                if (surface.Diagnostics is not null || !native.RetireAsync().IsCompletedSuccessfully)
                    throw new InvalidOperationException("Shutdown retained a live framework/GPU view.");
                var after = native.CaptureSnapshot(new MauiSurfaceSnapshot(0, 0, 1, 0, 0, 0, "probe", "probe"));
                if (after.MetalAllocatedBytes is not null || after.NativeFramePipeline?.PendingGpuFrames != 0)
                    throw new InvalidOperationException("Native resources were not retired before shutdown completed.");
                File.WriteAllText(output, JsonSerializer.Serialize(
                    new UIKitShutdownProbeResult("PASS", true, true, true, before!, after,
                        "OS-owned; tested separately", "notVerified"),
                    UIKitShutdownProbeJsonContext.Default.UIKitShutdownProbeResult));
            });
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
