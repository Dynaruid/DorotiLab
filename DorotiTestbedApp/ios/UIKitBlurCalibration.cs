using CoreGraphics;
using Doroti.Host.Maui;
using Foundation;
using SkiaSharp;
using UIKit;
using ObjCRuntime;

namespace DorotiTestbedApp.iOS;

// Opt-in measurement of PUBLIC UIKit blur against an sRGB Skia Gaussian reference.
// Captures the whole UIWindow as required by UIVisualEffectView's snapshot contract.
internal static class UIKitBlurCalibration
{
    internal static async Task RunAsync()
    {
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_BLUR_CALIBRATION") != "1") return;
        await Task.Delay(2000);
        var dispatcher = new UIKitPlatformViewDispatcher();
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "blur-calibration");
        Directory.CreateDirectory(directory);
        UIView? panel = null;
        var idle = false;
        try
        {
            UIWindow? window = null;
            UIImageView? source = null;
            UIVisualEffectView? effect = null;
            UIImage? original = null;
            var scale = 3.0;
            await OnUi(() =>
            {
                window = Microsoft.Maui.Controls.Application.Current!.Windows.Select(w => w.Handler?.PlatformView).OfType<UIWindow>().First();
                scale = (double)window.Screen.Scale;
                idle = UIApplication.SharedApplication.IdleTimerDisabled;
                UIApplication.SharedApplication.IdleTimerDisabled = true;
                panel = new UIView(window.Bounds) { BackgroundColor = UIColor.White, OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light };
                window.AddSubview(panel);
                original = MakeImage(scale, 0);
                source = new UIImageView(original) { Frame = new CGRect(20, 150, 320, 300), ContentMode = UIViewContentMode.ScaleToFill };
                panel.AddSubview(source);
                effect = new UIVisualEffectView { Frame = source.Frame, UserInteractionEnabled = false };
                panel.AddSubview(effect);
                File.WriteAllText(Path.Combine(directory, "geometry.txt"), $"scale={scale}\nleft=20\ntop=150\nwidth=320\nheight=300\n");
            });
            await Task.Delay(300);
            await Capture("source");
            foreach (var sigma in new[] { 2.0, 4.0, 6.0, 8.0, 12.0, 16.0 })
            {
                UIImage? reference = null;
                await OnUi(() => { reference = MakeImage(scale, sigma); source!.Image = reference; });
                await Task.Delay(100);
                await Capture($"reference-{sigma:0}");
                await OnUi(() => { source!.Image = original; reference!.Dispose(); });
            }
            if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_GAUSSIAN_CALIBRATION") == "1")
            {
                await OnUi(() =>
                {
                    using var invalid = new UIVisualEffectView();
                    if (UIKitGaussianFilter.Apply(invalid, 6) is null)
                        throw new InvalidOperationException("Invalid structure was accepted");
                    using var plain = new NSObject();
                    using var missingKey = new NSString("doroti_intentionally_missing_blur_probe_key");
                    var caught = false;
                    try { plain.ValueForKey(missingKey); }
                    catch (ObjCException) { caught = true; }
                    if (!caught) throw new InvalidOperationException("Objective-C exception was not marshaled");
                    File.WriteAllText(Path.Combine(directory, "exception-probe.txt"), "PASS missing structure rejected; Objective-C KVC exception caught in C#");
                    using var blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.Light);
                    effect!.Effect = blur;
                });
                foreach (var sigma in new[] { 4.0, 6.0, 12.0, 16.0 })
                foreach (var theme in new[] { UIUserInterfaceStyle.Light, UIUserInterfaceStyle.Dark })
                {
                    await OnUi(() =>
                    {
                        panel!.OverrideUserInterfaceStyle = theme;
                        panel.LayoutIfNeeded();
                        if (UIKitGaussianFilter.Apply(effect!, sigma) is { } reason)
                            throw new NotSupportedException(reason);
                    });
                    await Task.Delay(150);
                    await Capture($"Gaussian-{sigma:0.00}-{theme}");
                }
            }
            else
            foreach (var style in new[] { UIBlurEffectStyle.Light, UIBlurEffectStyle.ExtraLight, UIBlurEffectStyle.SystemUltraThinMaterialLight, UIBlurEffectStyle.SystemMaterial })
            foreach (var fraction in style == UIBlurEffectStyle.SystemMaterial ? new[] { 1.0 } : new[] { .15, .25, .4, .6, 1.0 })
            {
                UIViewPropertyAnimator? animator = null;
                UIBlurEffect? blur = null;
                await OnUi(() =>
                {
                    effect!.Effect = null;
                    blur = UIBlurEffect.FromStyle(style);
                    animator = new UIViewPropertyAnimator(1, UIViewAnimationCurve.Linear, () => effect.Effect = blur)
                    { ScrubsLinearly = true, PausesOnCompletion = true };
                    animator.StartAnimation(); animator.PauseAnimation(); animator.FractionComplete = (nfloat)fraction;
                });
                foreach (var theme in new[] { UIUserInterfaceStyle.Light, UIUserInterfaceStyle.Dark })
                {
                    await OnUi(() => panel!.OverrideUserInterfaceStyle = theme);
                    await Task.Delay(150);
                    await Capture($"{style}-{fraction:0.00}-{theme}");
                }
                await OnUi(() => { animator!.StopAnimation(true); animator.Dispose(); effect!.Effect = null; blur!.Dispose(); });
            }
            await OnUi(() => { effect!.RemoveFromSuperview(); effect.Dispose(); source!.RemoveFromSuperview(); source.Dispose(); original!.Dispose(); });
            File.WriteAllText(Path.Combine(directory, "result.txt"), "PASS capture complete; image analysis is separate");

            Task OnUi(Action action) => dispatcher.InvokeAsync(() => { action(); return ValueTask.CompletedTask; }).AsTask();
            Task Capture(string name) => OnUi(() =>
            {
                using var renderer = new UIGraphicsImageRenderer(window!.Bounds.Size);
                using var screenshot = renderer.CreateImage(_ => window.DrawViewHierarchy(window.Bounds, true));
                using var png = screenshot.AsPNG();
                File.WriteAllBytes(Path.Combine(directory, name + ".png"), png!.ToArray());
            });
        }
        catch (Exception error) { File.WriteAllText(Path.Combine(directory, "error.txt"), error.ToString()); }
        finally
        {
            await dispatcher.InvokeAsync(() =>
            {
                panel?.RemoveFromSuperview(); panel?.Dispose();
                UIApplication.SharedApplication.IdleTimerDisabled = idle;
                return ValueTask.CompletedTask;
            });
        }
    }

    private static UIImage MakeImage(double scale, double sigma)
    {
        var width = (int)(320 * scale); var height = (int)(300 * scale);
        using var colorSpace = SKColorSpace.CreateSrgb();
        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace));
        var canvas = surface.Canvas;
        canvas.Scale((float)scale);
        using var paint = new SKPaint { IsAntialias = false };
        paint.Color = new SKColor(32, 32, 32); canvas.DrawRect(0, 0, 160, 100, paint);
        paint.Color = new SKColor(224, 224, 224); canvas.DrawRect(160, 0, 160, 100, paint);
        var colors = new[] { new SKColor(180, 30, 30), new SKColor(30, 150, 30), new SKColor(30, 80, 180), new SKColor(128, 128, 128) };
        for (var i = 0; i < colors.Length; i++) { paint.Color = colors[i]; canvas.DrawRect(i * 80, 100, 80, 100, paint); }
        for (var y = 200; y < 300; y += 20)
        for (var x = 0; x < 320; x += 20)
        { paint.Color = (x / 20 + y / 20) % 2 == 0 ? new SKColor(224, 224, 224) : new SKColor(32, 32, 32); canvas.DrawRect(x, y, 20, 20, paint); }
        using var source = surface.Snapshot();
        using var output = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace));
        using var filter = sigma > 0 ? SKImageFilter.CreateBlur((float)(sigma * scale), (float)(sigma * scale), SKShaderTileMode.Clamp) : null;
        using var blur = new SKPaint { ImageFilter = filter };
        output.Canvas.DrawImage(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest), blur);
        using var image = output.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var bytes = NSData.FromArray(data.ToArray());
        return UIImage.LoadFromData(bytes, (nfloat)scale)!;
    }
}
