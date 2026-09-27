using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurAdaptiveValidation
{
    internal static void Run(SkiaSceneRenderer renderer, VulkanFixture fixture, bool graphite)
    {
        const int size = 128;
        var info = new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
        foreach (
            var name in new[]
            {
                "vertical",
                "reverse",
                "horizontal",
                "diagonal",
                "dpi2",
                "constant-low",
                "constant-high",
            }
        )
        {
            var start = name == "horizontal" ? new Offset(32, 0) : new Offset(0, 32);
            var end = name == "horizontal" ? new Offset(96, 0) : new Offset(0, 96);
            if (name == "diagonal")
                (start, end) = (new Offset(32, 32), new Offset(96, 96));
            if (name == "reverse")
                (start, end) = (end, start);
            var dpr = name == "dpi2" ? 2d : 1d;
            start = new Offset(start.dx / dpr, start.dy / dpr);
            end = new Offset(end.dx / dpr, end.dy / dpr);
            var sigma0 =
                name == "constant-low" ? 1
                : name == "constant-high" ? 12
                : 0;
            var sigma1 = name == "constant-low" ? 1 : 12;
            byte[] Render(
                double scale,
                bool adaptive,
                VariableBlurKernel kernel = VariableBlurKernel.gaussian
            )
            {
                using var surface = fixture.CreateSurface(info);
                surface.Canvas.Clear(SKColors.Transparent);
                using var paint = new SKPaint();
                for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                {
                    // Pixel-width strokes expose detail loss. Varying alpha catches
                    // accidental SrcOver instead of weighted replacement.
                    paint.Color =
                        (x + y) % 3 == 0
                            ? new SKColor(240, 80, 32, 128)
                            : new SKColor(32, 160, 240);
                    surface.Canvas.DrawRect(x, y, 1, 1, paint);
                }
                var builder = new SceneBuilder(1);
                builder.pushTransform([dpr, 0, 0, 0, 0, dpr, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1]);
                builder.pushBackdropFilter(
                    ImageFilter.variableBlur(
                        start,
                        end,
                        startSigma: sigma0 / dpr,
                        endSigma: sigma1 / dpr,
                        resolutionScale: scale,
                        adaptiveResolution: adaptive,
                        kernel: kernel
                    ),
                    Doroti.Ui.BlendMode.src
                );
                builder.pop();
                builder.pop();
                using var scene = builder.build();
                renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, size, size);
                if (graphite)
                    return fixture.ReadGraphite(surface, surface).Actual;
                fixture.Complete();
                using var bitmap = new SKBitmap(info);
                if (!surface.ReadPixels(info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
                    throw new Exception("Adaptive blur readback failed");
                return bitmap.Bytes;
            }
            var full = Render(1, false);
            var half = Render(.5, false);
            var quarter = Render(.25, false);
            var actual = Render(.25, true);
            var fast = Render(.25, true, VariableBlurKernel.fastGaussian);
            var fastError = 0d;
            var fastSharpError = 0;
            var max = 0d;
            var sharpError = 0;
            var sharpPixels = 0;
            var fixedSharpError = 0d;
            var dx = (end.dx - start.dx) * dpr;
            var dy = (end.dy - start.dy) * dpr;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var t = Math.Clamp(
                    ((x + .5 - start.dx * dpr) * dx + (y + .5 - start.dy * dpr) * dy)
                        / (dx * dx + dy * dy),
                    0,
                    1
                );
                var sigma = sigma0 + (sigma1 - sigma0) * t;
                var halfWeight = Math.Clamp((sigma - 2) / 2, 0, 1);
                var quarterWeight = Math.Clamp((sigma - 4) / 4, 0, 1);
                for (var channel = 0; channel < 4; channel++)
                {
                    var p = (y * size + x) * 4 + channel;
                    var expected =
                        full[p] * (1 - halfWeight)
                        + half[p] * (halfWeight - quarterWeight)
                        + quarter[p] * quarterWeight;
                    max = Math.Max(max, Math.Abs(actual[p] - expected));
                    fastError += Math.Abs(fast[p] - actual[p]);
                    if (sigma <= 2)
                    {
                        sharpError = Math.Max(sharpError, Math.Abs(actual[p] - full[p]));
                        fastSharpError = Math.Max(fastSharpError, Math.Abs(fast[p] - full[p]));
                        fixedSharpError += Math.Abs(quarter[p] - full[p]);
                        sharpPixels++;
                    }
                }
            }
            if (max > 2 || sharpError > 1)
                throw new Exception(
                    $"adaptive/{name}: blend error={max:F3}, sharp error={sharpError}"
                );
            if (name == "vertical" && fixedSharpError / sharpPixels < 5)
                throw new Exception("Sharp-detail fixture did not expose fixed-resolution loss");
            if (fastSharpError > 1 || fastError / actual.Length > 8)
                throw new Exception(
                    $"fastGaussian/{name}: sharp error={fastSharpError}, MAE={fastError / actual.Length:F3}"
                );
            Console.WriteLine(
                $"PASS adaptive/{name}: transition error={max:F3}, sharp error={sharpError}, fixed sharp MAE={(sharpPixels == 0 ? 0 : fixedSharpError / sharpPixels):F3}"
            );
            Console.WriteLine(
                $"PASS fastGaussian/{name}: sharp error={fastSharpError}, MAE={fastError / actual.Length:F3}"
            );
        }
    }
}
