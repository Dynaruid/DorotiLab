#if MACOS
using AppKit;
using Doroti.Host.Maui;
using Foundation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DorotiTestbedApp.MacOS;

// Opt-in product probe. This runs the actual widget/scene/Metal path before inspecting NSControls.
internal static class PlatformViewEvidence
{
    internal static async Task CaptureAsync()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        try
        {
            if (Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "interleaved")
            {
                await CaptureInterleavedAsync(path);
                return;
            }
            var dispatcher = new AppKitPlatformViewDispatcher();
            ProductPlatformViewResult? result = null;
            for (var attempt = 0; attempt < 300 && result is null; attempt++)
            {
                await Task.Delay(100);
                await dispatcher.InvokeAsync(() =>
                {
                    var controls = Microsoft.Maui.Controls.Application.Current!.Windows
                        .Select(window => window.Handler?.PlatformView).OfType<NSWindow>()
                        .SelectMany(window => Descendants(window.ContentView))
                        .OfType<NSControl>().Where(control => control.Identifier?.StartsWith("doroti-platform-view-", StringComparison.Ordinal) == true)
                        .ToArray();
                    if (controls.Length != 2 || controls.Any(control => control.Superview?.Hidden != false))
                        return ValueTask.CompletedTask;
                    var button = controls.OfType<NSButton>().Single();
                    var editor = controls.OfType<NSTextField>().Single();
                    button.PerformClick(button);
                    if (button.Title != "Native clicks: 1") throw new InvalidOperationException("Product button activation was not delivered once.");
                    if (!editor.Window!.MakeFirstResponder(editor)) throw new InvalidOperationException("Product editor rejected focus.");
                    var fieldEditor = editor.CurrentEditor as NSTextView ?? throw new InvalidOperationException("No product field editor.");
                    fieldEditor.InsertText(new NSString("product-input"), new NSRange(NSRange.NotFound, 0));
                    if (!editor.StringValue.Contains("product-input")) throw new InvalidOperationException("Product editor lost inserted text.");
                    var bounds = controls.Select(control => control.Superview!.Frame).ToArray();
                    if (bounds.Any(rect => rect.Width != 240 || rect.Height != 64)) throw new InvalidOperationException("Scene/native layout size mismatch.");
                    result = new(true, controls.Select(control => control.Identifier!).ToArray(),
                        bounds.Select(rect => new double[] { rect.X, rect.Y, rect.Width, rect.Height }).ToArray(),
                        (double)editor.Window.BackingScaleFactor, button.Title, editor.StringValue,
                        (int)editor.Window.WindowNumber, Environment.GetEnvironmentVariable("DOROTI_MACOS_GRAPHITE") == "0" ? "Ganesh-Metal" : "Graphite-Metal");
                    return ValueTask.CompletedTask;
                });
            }
            if (result is null) throw new TimeoutException("The product widget/Metal path did not attach both native controls.");
            File.WriteAllText(path, JsonSerializer.Serialize(result, PlatformViewEvidenceJsonContext.Default.ProductPlatformViewResult));
            // Leave the window running for screenshot capture and explicit process cleanup by the validator.
        }
        catch (Exception error) { File.WriteAllText(path + ".exception.txt", error.ToString()); }
    }

    private static async Task CaptureInterleavedAsync(string path)
    {
        var dispatcher = new AppKitPlatformViewDispatcher();
        NSControl[] controls = [];
        for (var attempt = 0; attempt < 300 && controls.Length != 2; attempt++)
        {
            await Task.Delay(100);
            await dispatcher.InvokeAsync(() =>
            {
                controls = Microsoft.Maui.Controls.Application.Current!.Windows
                    .Select(window => window.Handler?.PlatformView).OfType<NSWindow>()
                    .SelectMany(window => Descendants(window.ContentView)).OfType<NSControl>()
                    .Where(control => control.Identifier?.StartsWith("doroti-platform-view-", StringComparison.Ordinal) == true &&
                        control.Superview?.Hidden == false).ToArray();
                return ValueTask.CompletedTask;
            });
        }
        if (controls.Length != 2) throw new TimeoutException("Interleaved product controls did not attach.");
        var editor = controls.OfType<NSTextField>().Single();
        var button = controls.OfType<NSButton>().Single();
        await dispatcher.InvokeAsync(() =>
        {
            editor.StringValue = "preserved-native-state";
            return ValueTask.CompletedTask;
        });
        var previous = -1;
        for (var attempt = 0; attempt < 1200; attempt++)
        {
            await Task.Delay(100);
            if (!File.Exists(path + ".stage") || !int.TryParse(File.ReadAllText(path + ".stage"), out var stage) || stage == previous) continue;
            await dispatcher.InvokeAsync(() =>
            {
                (PlatformViewFixtureProbe.SetStage ?? throw new InvalidOperationException("Fixture stage controller unavailable."))(stage);
                return ValueTask.CompletedTask;
            });
            await Task.Delay(600);
            var clicksBefore = PlatformViewFixtureProbe.ForegroundClicks;
            InterleavedStageResult? stageResult = null;
            await dispatcher.InvokeAsync(() =>
            {
                if (editor.StringValue != "preserved-native-state" || editor.Superview?.Superview is null || button.Superview?.Superview is null)
                    throw new InvalidOperationException("Native identity/state was lost while changing paint order.");
                var overlay = button.Superview.Superview;
                var origin = button.Superview.Frame.Location;
                var point = new CoreGraphics.CGPoint(origin.X + 230 - 20, origin.Y + 110 - 20);
                var hit = overlay.HitTest(overlay.ConvertPointToView(point, overlay.Superview));
                var shouldShield = stage is 1 or 2 or 5 or 6 or 9;
                if (shouldShield ? hit is not DorotiMacOSMetalView : hit != editor)
                    throw new InvalidOperationException($"Stage {stage} hit target mismatch: {hit?.GetType().Name}");
                if (shouldShield)
                {
                    var location = overlay.ConvertPointToView(point, null!);
                    var timestamp = (double)System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;
                    using var down = NSEvent.MouseEvent(NSEventType.LeftMouseDown, location, 0, timestamp,
                        editor.Window!.WindowNumber, null!, 0, 1, 1)!;
                    using var up = NSEvent.MouseEvent(NSEventType.LeftMouseUp, location, 0, timestamp + .01,
                        editor.Window.WindowNumber, null!, 0, 1, 0)!;
                    hit!.MouseDown(down);
                    hit.MouseUp(up);
                }
                var slots = overlay.Subviews.Where(view => view.GetType().Name == "AppKitPlatformRasterSurface" && !view.Hidden).ToArray();
                if (stage == 5 && slots.Length < 3) throw new InvalidOperationException("Missing intermediate/foreground Metal surfaces.");
                stageResult = new InterleavedStageResult(true, stage, controls.Select(control => control.Identifier!).ToArray(),
                    editor.StringValue, hit!.GetType().Name, slots.Select(view => (double)view.Layer!.ZPosition).ToArray(),
                    (int)editor.Window!.WindowNumber, (double)editor.Window.BackingScaleFactor,
                    new double[] { origin.X - 20, origin.Y - 20 },
                    new double[] { overlay.ConvertPointToView(CoreGraphics.CGPoint.Empty, null!).X,
                        editor.Window.Frame.Height - overlay.ConvertPointToView(CoreGraphics.CGPoint.Empty, null!).Y });
                return ValueTask.CompletedTask;
            });
            await Task.Delay(200);
            var clicks = PlatformViewFixtureProbe.ForegroundClicks - clicksBefore;
            if (clicks != (stage is 1 or 2 or 5 or 6 or 9 ? 1 : 0))
                throw new InvalidOperationException($"Stage {stage}: shield delivered {clicks} foreground taps.");
            File.WriteAllText(path + $".stage-{stage}.json", JsonSerializer.Serialize(stageResult! with { ForegroundTaps = clicks },
                PlatformViewEvidenceJsonContext.Default.InterleavedStageResult));
            previous = stage;
        }
    }

    private static IEnumerable<NSView> Descendants(NSView? root)
    {
        if (root is null) yield break;
        yield return root;
        foreach (var child in root.Subviews)
            foreach (var descendant in Descendants(child)) yield return descendant;
    }
}

internal sealed record ProductPlatformViewResult(bool Passed, string[] Identifiers, double[][] Bounds,
    double BackingScale, string ButtonTitle, string EditorText, int WindowNumber, string Renderer);

internal sealed record InterleavedStageResult(bool Passed, int Stage, string[] Identifiers, string EditorText,
    string HitTarget, double[] RasterOrders, int WindowNumber, double BackingScale, double[] SceneOrigin, double[] OverlayOrigin,
    int ForegroundTaps = 0);

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ProductPlatformViewResult))]
[JsonSerializable(typeof(InterleavedStageResult))]
internal sealed partial class PlatformViewEvidenceJsonContext : JsonSerializerContext;
#endif
