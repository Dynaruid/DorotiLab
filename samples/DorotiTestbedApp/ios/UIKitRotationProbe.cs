using CoreAnimation;
using Doroti.Host.Maui;
using Foundation;
using MetalKit;
using System.Text.Json;
using System.Text.Json.Serialization;
using UIKit;

namespace DorotiTestbedApp.iOS;

internal sealed record UIKitViewportUpdate(double Time, double TargetTime, double Width, double Height,
    double FrameInterval, double PreferredFrameRate);
internal sealed record UIKitRotationSample(double Time, double Width, double Height,
    double DrawableWidth, double DrawableHeight, double BoundsWidth, double PresentationWidth,
    double SafeTop, bool Animating, double Scale);
internal sealed record UIKitRotationResult(string Orientation, int DistinctWidths,
    double MeanPhaseError, double MaxPhaseError, List<string> Animations,
    List<UIKitViewportUpdate> Updates, List<UIKitRotationSample> Samples);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(List<UIKitRotationResult>))]
[JsonSerializable(typeof(List<UIKitRotationSample>))]
internal partial class UIKitRotationJsonContext : JsonSerializerContext;

// Opt-in native regression: request real scene rotations and verify that the
// renderer consumes intermediate viewports, then settles at exact pixels.
internal static class UIKitRotationProbe
{
    internal static async Task RunAsync(string output)
    {
        if (!Path.IsPathRooted(output))
            output = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                output
            );
        var results = new List<UIKitRotationResult>();
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                CheckNativeTiming();
                if (!OperatingSystem.IsIOSVersionAtLeast(16))
                    throw new PlatformNotSupportedException(
                        "Scene rotation probe requires iOS 16 or later."
                    );
                UIWindowScene? scene = null;
                MTKView? view = null;
                // A cold simulator can remain on its launch screen for several
                // seconds. Wait for an attached, rendered foreground surface.
                for (var attempt = 0; attempt < 300 && view is null; attempt++)
                {
                    scene = UIApplication
                        .SharedApplication.ConnectedScenes.OfType<UIWindowScene>()
                        .FirstOrDefault(s =>
                            s.ActivationState == UISceneActivationState.ForegroundActive
                        );
                    var window = scene?.Windows.FirstOrDefault(w => w.IsKeyWindow);
                    if (window is not null)
                        view = Descendants(window)
                            .OfType<MTKView>()
                            .FirstOrDefault(v =>
                                v is IUIKitAnimatedViewport animated
                                && animated.AnimatedViewport.Size.Width > 0
                            );
                    if (view is null)
                        await Task.Delay(100);
                }
                if (scene is null || view is null)
                    throw new TimeoutException("No rendered foreground viewport for rotation.");
                await Task.Delay(2000);
                var viewport = ((IUIKitAnimatedViewport)view).AnimatedViewport;
                var surface = VisualDescendants(
                        Microsoft.Maui.Controls.Application.Current!.Windows[0]
                    )
                    .OfType<DorotiMauiSurface>()
                    .First();
                var screenScale = view.Window!.Screen.Scale;
                foreach (
                    var orientation in new[]
                    {
                        UIInterfaceOrientationMask.LandscapeLeft,
                        UIInterfaceOrientationMask.Portrait,
                    }
                )
                {
                    var initial = view.DrawableSize;
                    var samples = new List<UIKitRotationSample>();
                    var widths = new HashSet<double>();
                    var phaseErrors = new List<double>();
                    var animations = new List<string>();
                    var updates = new List<UIKitViewportUpdate>();
                    var collecting = true;
                    void RecordViewport() =>
                        updates.Add(
                            new UIKitViewportUpdate(CAAnimation.CurrentMediaTime(),
                                viewport.FrameTargetTimestamp, (double)viewport.Size.Width,
                                (double)viewport.Size.Height, viewport.FrameInterval,
                                viewport.PreferredFrameRate)
                        );
                    viewport.Changed += RecordViewport;
                    void RecordSample()
                    {
                        if (!collecting) return;
                        if (
                            animations.Count == 0
                            && view.Layer.AnimationKeys is { Length: > 0 } keys
                        )
                            foreach (var key in keys)
                            {
                                var animation = view.Layer.AnimationForKey(key);
                                animations.Add(
                                    key
                                        + ": "
                                        + animation?.Description
                                        + $" begin={animation?.BeginTime} duration={animation?.Duration}"
                                        + (
                                            animation is CABasicAnimation basic
                                                ? $" from={basic.From} to={basic.To} additive={basic.Additive} curve={basic.TimingFunction}"
                                                : ""
                                        )
                                );
                            }
                        var rendered = surface.GeometrySnapshot!;
                        widths.Add(rendered.PixelWidth);
                        var presentationWidth = (double)(
                            view.Layer.PresentationLayer?.Bounds.Width ?? view.Bounds.Width
                        );
                        if (viewport.IsAnimating)
                            phaseErrors.Add(
                                Math.Abs(
                                    rendered.PixelWidth / (double)screenScale - presentationWidth
                                )
                            );
                        samples.Add(
                            new UIKitRotationSample(CAAnimation.CurrentMediaTime(),
                                rendered.PixelWidth, rendered.PixelHeight,
                                (double)view.DrawableSize.Width, (double)view.DrawableSize.Height,
                                (double)view.Bounds.Width, presentationWidth,
                                (double)viewport.SafeAreaInsets.Top, viewport.IsAnimating,
                                (double)view.ContentScaleFactor)
                        );
                    }
                    // UIKit can invoke this observer before the renderer's
                    // display-link callback. Sample after all pulse callbacks
                    // so a previous raster is not compared to the new geometry.
                    using var display = CADisplayLink.Create(() =>
                        UIApplication.SharedApplication.BeginInvokeOnMainThread(RecordSample));
                    display.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
                    string? error = null;
                    using var preferences = new UIWindowSceneGeometryPreferencesIOS(orientation);
                    try
                    {
                        scene.RequestGeometryUpdate(
                            preferences,
                            e => error = e.LocalizedDescription
                        );
                        for (var attempt = 0; attempt < 100; attempt++)
                        {
                            await Task.Delay(100);
                            var portrait = view.Bounds.Height > view.Bounds.Width;
                            if (
                                error is not null
                                || (
                                    portrait == (orientation == UIInterfaceOrientationMask.Portrait)
                                    && !viewport.IsAnimating
                                    && !initial.Equals(view.DrawableSize)
                                )
                            )
                                break;
                        }
                        // Include the last presentation and any late safe-area callback.
                        await Task.Delay(250);
                    }
                    finally
                    {
                        collecting = false;
                        display.Invalidate();
                        viewport.Changed -= RecordViewport;
                    }
                    if (error is not null)
                        throw new InvalidOperationException(error);
                    var final = view.DrawableSize;
                    if (initial.Equals(final) || widths.Count < 4)
                        throw new InvalidOperationException(
                            $"Rotation skipped intermediate layouts: {widths.Count} widths. Samples: {JsonSerializer.Serialize(samples, UIKitRotationJsonContext.Default.ListUIKitRotationSample)}"
                        );
                    if (
                        final.Width != Math.Round(view.Bounds.Width * screenScale)
                        || final.Height != Math.Round(view.Bounds.Height * screenScale)
                    )
                        throw new InvalidOperationException(
                            "Rotation did not settle at the exact drawable size."
                        );
                    if (!viewport.SafeAreaInsets.Equals(view.SafeAreaInsets))
                        throw new InvalidOperationException("Safe-area insets did not settle.");
                    if (viewport.IsAnimating)
                        throw new InvalidOperationException("Rotation display link did not stop.");
                    if (surface.Diagnostics!.Frame.Failed != 0)
                        throw new InvalidOperationException("A rotation frame failed.");
                    var meanPhaseError =
                        phaseErrors.Count > 0 ? phaseErrors.Average() : double.PositiveInfinity;
                    var maxPhaseError =
                        phaseErrors.Count > 0 ? phaseErrors.Max() : double.PositiveInfinity;
                    results.Add(
                        new UIKitRotationResult(orientation.ToString(), widths.Count,
                            meanPhaseError, maxPhaseError, animations, updates, samples)
                    );
                    if (
                        Environment.GetEnvironmentVariable("DOROTI_UIKIT_ROTATION_ASSERT_SYNC")
                        == "1"
                    )
                    {
                        var span = Math.Abs(final.Width - initial.Width) / (double)screenScale;
                        if (meanPhaseError > span * .05 || maxPhaseError > span * .10)
                            throw new InvalidOperationException(
                                $"Rotation phase mismatch: mean={meanPhaseError:F2}pt, max={maxPhaseError:F2}pt, span={span:F2}pt."
                            );
                    }
                }
            });
            File.WriteAllText(output, JsonSerializer.Serialize(results, UIKitRotationJsonContext.Default.ListUIKitRotationResult));
        }
        catch (Exception error)
        {
            if (results.Count > 0)
                File.WriteAllText(output + ".partial.json", JsonSerializer.Serialize(results, UIKitRotationJsonContext.Default.ListUIKitRotationResult));
            File.WriteAllText(output + ".error", error.ToString());
        }
    }

    private static void CheckNativeTiming()
    {
        using var layer = new CALayer { Bounds = new CoreGraphics.CGRect(0, 0, 844, 390) };
        foreach (var additive in new[] { false, true })
        {
            using var animation = CABasicAnimation.FromKeyPath("bounds.size");
            animation.From = NSValue.FromCGSize(additive ? new(-454, 454) : new(390, 844));
            animation.To = NSValue.FromCGSize(additive ? CoreGraphics.CGSize.Empty : new(844, 390));
            animation.Additive = additive;
            animation.BeginTime = CAAnimation.CurrentMediaTime() + 1;
            animation.Duration = .3;
            animation.TimingFunction = CAMediaTimingFunction.FromName(
                CAMediaTimingFunction.EaseInEaseOut
            );
            layer.AddAnimation(animation, "bounds.size");
            var midpoint = UIKitBoundsAnimation.Sample(layer, animation.BeginTime + .15);
            if (
                midpoint is not { } size
                || Math.Abs(size.Width - 617) > .01
                || Math.Abs(size.Height - 617) > .01
            )
                throw new InvalidOperationException(
                    "Native bounds timing/additive sampling failed."
                );
            layer.RemoveAllAnimations();
        }
    }

    private static IEnumerable<UIView> Descendants(UIView view)
    {
        yield return view;
        foreach (var child in view.Subviews)
        foreach (var descendant in Descendants(child))
            yield return descendant;
    }

    private static IEnumerable<Microsoft.Maui.IVisualTreeElement> VisualDescendants(
        Microsoft.Maui.IVisualTreeElement element
    )
    {
        yield return element;
        foreach (var child in element.GetVisualChildren())
        foreach (var descendant in VisualDescendants(child))
            yield return descendant;
    }
}
