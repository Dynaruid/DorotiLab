using CoreGraphics;
using Doroti.Host.Maui;
using Foundation;
using SkiaSharp;
using UIKit;

namespace DorotiTestbedApp.iOS;

// Opt-in measurement of PUBLIC UIKit blur against an sRGB Skia Gaussian reference.
// Captures the whole UIWindow as required by UIVisualEffectView's snapshot contract.
internal static class UIKitBlurCalibration
{
    internal static async Task RunAsync()
    {
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_BLUR_CALIBRATION") != "1")
            return;
        await Task.Delay(2000);
        var dispatcher = new UIKitPlatformViewDispatcher();
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Path.GetFileName(
                Environment.GetEnvironmentVariable("DOROTI_UIKIT_BLUR_CALIBRATION_NAME")
                    ?? "blur-calibration"
            )
        );
        Directory.CreateDirectory(directory);
        UIView? panel = null;
        UIImageView? source = null;
        UIKitPlatformBlurView? effect = null;
        UIImage? original = null;
        var idle = false;
        try
        {
            UIWindow? window = null;
            var scale = 3.0;
            await OnUi(() =>
            {
                window = Microsoft
                    .Maui.Controls.Application.Current!.Windows.Select(w => w.Handler?.PlatformView)
                    .OfType<UIWindow>()
                    .First();
                scale = (double)window.Screen.Scale;
                idle = UIApplication.SharedApplication.IdleTimerDisabled;
                UIApplication.SharedApplication.IdleTimerDisabled = true;
                panel = new UIView(window.Bounds)
                {
                    BackgroundColor = UIColor.White,
                    OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light,
                };
                window.AddSubview(panel);
                original = MakeImage(scale, 0);
                source = new UIImageView(original)
                {
                    Frame = new CGRect(20, 150, 320, 300),
                    ContentMode = UIViewContentMode.ScaleToFill,
                };
                panel.AddSubview(source);
                effect = new UIKitPlatformBlurView
                {
                    Frame = source.Frame,
                    UserInteractionEnabled = false,
                };
                panel.AddSubview(effect);
                File.WriteAllText(
                    Path.Combine(directory, "geometry.txt"),
                    $"scale={scale}\nleft=20\ntop=150\nwidth=320\nheight=300\n"
                );
            });
            await Task.Delay(300);
            await Capture("source");
            foreach (var sigma in new[] { 2.0, 4.0, 6.0, 8.0, 12.0, 16.0 })
            {
                UIImage? reference = null;
                await OnUi(() =>
                {
                    reference = MakeImage(scale, sigma);
                    source!.Image = reference;
                });
                await Task.Delay(100);
                await Capture($"reference-{sigma:0}");
                await OnUi(() =>
                {
                    source!.Image = original;
                    reference!.Dispose();
                });
            }
            // Exercise the production adapter, including scrubbing back from full
            // intensity and resetting to zero (no material or residual tint).
            foreach (
                var (name, strength) in new[]
                {
                    ("UIKit", .15),
                    ("UIKit", .25),
                    ("UIKit", .375),
                    ("UIKit", .75),
                    ("UIKit", 1.0),
                    ("UIKitDecreasing", .375),
                    ("UIKitReset", 0.0),
                }
            )
            foreach (var theme in new[] { UIUserInterfaceStyle.Light, UIUserInterfaceStyle.Dark })
            {
                await OnUi(() =>
                {
                    panel!.OverrideUserInterfaceStyle = theme;
                    effect!.SetSigma(strength * 16);
                    panel.LayoutIfNeeded();
                    if (Math.Abs(effect.AppliedIntensity - strength * 16 / 30) > .0001)
                        throw new InvalidOperationException("Animator intensity was not retained");
                    if (strength == 0 && effect.Effect is not null)
                        throw new InvalidOperationException("Zero strength retained the material");
                });
                await Task.Delay(150);
                await Capture(FormattableString.Invariant($"{name}-{strength:0.000}-{theme}"));
            }
            File.WriteAllText(
                Path.Combine(directory, "result.txt"),
                "PASS capture complete; image analysis is separate"
            );

            Task OnUi(Action action) =>
                dispatcher
                    .InvokeAsync(() =>
                    {
                        action();
                        return ValueTask.CompletedTask;
                    })
                    .AsTask();
            Task Capture(string name) =>
                OnUi(() =>
                {
                    using var renderer = new UIGraphicsImageRenderer(window!.Bounds.Size);
                    using var screenshot = renderer.CreateImage(_ =>
                        window.DrawViewHierarchy(window.Bounds, true)
                    );
                    using var png = screenshot.AsPNG();
                    File.WriteAllBytes(Path.Combine(directory, name + ".png"), png!.ToArray());
                });
        }
        catch (Exception error)
        {
            File.WriteAllText(Path.Combine(directory, "error.txt"), error.ToString());
        }
        finally
        {
            await dispatcher.InvokeAsync(() =>
            {
                effect?.RemoveFromSuperview();
                effect?.Dispose();
                source?.RemoveFromSuperview();
                source?.Dispose();
                original?.Dispose();
                panel?.RemoveFromSuperview();
                panel?.Dispose();
                UIApplication.SharedApplication.IdleTimerDisabled = idle;
                return ValueTask.CompletedTask;
            });
        }
    }

    private static UIImage MakeImage(double scale, double sigma)
    {
        var width = (int)(320 * scale);
        var height = (int)(300 * scale);
        using var colorSpace = SKColorSpace.CreateSrgb();
        using var surface = SKSurface.Create(
            new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace)
        );
        var canvas = surface.Canvas;
        canvas.Scale((float)scale);
        using var paint = new SKPaint { IsAntialias = false };
        paint.Color = new SKColor(32, 32, 32);
        canvas.DrawRect(0, 0, 160, 100, paint);
        paint.Color = new SKColor(224, 224, 224);
        canvas.DrawRect(160, 0, 160, 100, paint);
        var colors = new[]
        {
            new SKColor(180, 30, 30),
            new SKColor(30, 150, 30),
            new SKColor(30, 80, 180),
            new SKColor(128, 128, 128),
        };
        for (var i = 0; i < colors.Length; i++)
        {
            paint.Color = colors[i];
            canvas.DrawRect(i * 80, 100, 80, 100, paint);
        }
        for (var y = 200; y < 300; y += 20)
        for (var x = 0; x < 320; x += 20)
        {
            paint.Color =
                (x / 20 + y / 20) % 2 == 0 ? new SKColor(224, 224, 224) : new SKColor(32, 32, 32);
            canvas.DrawRect(x, y, 20, 20, paint);
        }
        using var source = surface.Snapshot();
        using var output = SKSurface.Create(
            new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul, colorSpace)
        );
        using var filter =
            sigma > 0
                ? SKImageFilter.CreateBlur(
                    (float)(sigma * scale),
                    (float)(sigma * scale),
                    SKShaderTileMode.Clamp
                )
                : null;
        using var blur = new SKPaint { ImageFilter = filter };
        output.Canvas.DrawImage(source, 0, 0, new SKSamplingOptions(SKFilterMode.Nearest), blur);
        using var image = output.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var bytes = NSData.FromArray(data.ToArray());
        return UIImage.LoadFromData(bytes, (nfloat)scale)!;
    }
}
