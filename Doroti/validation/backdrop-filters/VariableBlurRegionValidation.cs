using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurRegionValidation
{
    // Compare bounded execution to full-frame execution followed by a native
    // fractional clip. Odd sizes exercise independent X/Y downsample rounding;
    // the sheared gradient exercises the first pass's second-axis halo.
    internal static void Run(SkiaSceneRenderer renderer, VulkanFixture fixture, bool graphite)
    {
        const int width = 127,
            height = 95;
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var clip = new SKRect(0, 7.25f, 83.75f, 69.5f);
        foreach (var scale in new[] { 1.0, 0.5, 0.25, 0.3, 0.125 })
        foreach (
            var tile in new[] { TileMode.clamp, TileMode.decal, TileMode.mirror, TileMode.repeated }
        )
        {
            using var full = fixture.CreateSurface(info);
            // Windows retains a larger GPU allocation when the viewport shrinks.
            // Only the viewport may participate in backdrop sampling/resizing.
            using var actual = fixture.CreateSurface(
                info.WithSize(width * 2 + 17, height * 2 + 19)
            );
            actual.Canvas.ClipRect(SKRect.Create(width, height));
            using var expected = fixture.CreateSurface(info);
            void Background(SKCanvas canvas)
            {
                canvas.Clear(SKColors.Transparent);
                using var paint = new SKPaint();
                for (var y = 0; y < height; y += 7)
                for (var x = 0; x < width; x += 9)
                {
                    paint.Color =
                        (x / 9 + y / 7) % 2 == 0
                            ? new SKColor(224, 96, 32)
                            : new SKColor(64, 128, 192, 128);
                    canvas.DrawRect(x, y, 9, 7, paint);
                }
            }
            Scene Build(bool bounded)
            {
                var builder = new SceneBuilder(1);
                if (bounded)
                    builder.pushClipRect(
                        new Rect(clip.Left, clip.Top, clip.Right, clip.Bottom),
                        clipBehavior: Clip.antiAlias
                    );
                builder.pushTransform([1.2, .2, 0, 0, .35, .9, 0, 0, 0, 0, 1, 0, 3, -2, 0, 1]);
                builder.pushBackdropFilter(
                    ImageFilter.variableBlur(
                        new Offset(0, 0),
                        new Offset(100, 70),
                        startSigma: 2,
                        endSigma: 12,
                        tileMode: tile,
                        resolutionScale: scale
                    ),
                    Doroti.Ui.BlendMode.src
                );
                builder.pop();
                builder.pop();
                if (bounded)
                    builder.pop();
                return builder.build();
            }
            using var fullScene = Build(false);
            using var clippedScene = Build(true);
            Background(full.Canvas);
            renderer.DrawPlatformRasterSegment(full.Canvas, fullScene.Commands, width, height);
            using var fullImage = full.Snapshot();
            Background(expected.Canvas);
            expected.Canvas.Save();
            expected.Canvas.ClipRect(clip, SKClipOperation.Intersect, true);
            using var replace = new SKPaint { BlendMode = SKBlendMode.Src };
            expected.Canvas.DrawImage(fullImage, 0, 0, SKSamplingOptions.Default, replace);
            expected.Canvas.Restore();
            Background(actual.Canvas);
            renderer.DrawPlatformRasterSegment(actual.Canvas, clippedScene.Commands, width, height);
            byte[] a,
                e;
            if (graphite)
                (e, a) = fixture.ReadGraphite(expected, actual);
            else
            {
                fixture.Complete();
                byte[] Read(SKSurface surface)
                {
                    using var bitmap = new SKBitmap(info);
                    if (!surface.ReadPixels(info, bitmap.GetPixels(), bitmap.RowBytes, 0, 0))
                        throw new Exception("Region readback failed");
                    return bitmap.Bytes;
                }
                a = Read(actual);
                e = Read(expected);
            }
            var max = a.Zip(e, (x, y) => Math.Abs(x - y)).Max();
            if (max > 1)
                throw new Exception(
                    $"Variable blur region scale={scale} tile={tile}: max error {max}"
                );
            Console.WriteLine(
                $"PASS variable/odd-region scale={scale} tile={tile}: max error {max}"
            );
        }
    }
}
