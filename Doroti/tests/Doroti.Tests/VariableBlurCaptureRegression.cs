using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurCaptureRegression
{
    // Execute each production SkSL level on raster surfaces. This checks capture
    // scale/origin and kernel pixels; it does not exercise Graphite snapshots or
    // the production Adaptive mask/surface-pool pipeline.
    public static void Run()
    {
        FrameworkShaderLoader.RegisterResourceOwner(typeof(SkiaSceneRenderer).Assembly);
        var program = FrameworkShaderLoader.LoadEmbeddedProgram("rendering.variable-blur");
        using var effect = SKRuntimeEffect.CreateShader(program.source, out var shaderError)
            ?? throw new Exception(shaderError);
        var cases = 0;
        var maxError = 0;
        foreach (var (width, height) in new[] { (101, 240), (240, 101) })
        foreach (var scale in new[] { 1d, .5, .25 })
        foreach (var kernel in new[] { VariableBlurKernel.gaussian, VariableBlurKernel.fastGaussian })
        foreach (var tile in new[] { TileMode.clamp, TileMode.decal })
        foreach (var reverse in new[] { false, true })
        {
            using var source = SKSurface.Create(new SKImageInfo(width, height));
            source.Canvas.Clear(SKColors.Transparent);
            using var paint = new SKPaint();
            for (var y = 0; y < height; y += 3)
            for (var x = 0; x < width; x += 3)
            {
                paint.Color = new SKColor((byte)((x * 37 + y * 11) % 256),
                    (byte)((x * 13 + y * 29) % 256), (byte)((x * 7 + y * 43) % 256),
                    (byte)(80 + (x + y) % 176));
                source.Canvas.DrawRect(x, y, 3, 3, paint);
            }
            using var input = source.Snapshot();
            var vertical = width == 101;
            var visible = vertical ? new SKRect(0, 89.25f, width, 141.75f)
                : new SKRect(89.25f, 0, 141.75f, height);
            var settings = new VariableBlurSettings(new(0, 0),
                vertical ? new(0, 80) : new(80, 0), reverse ? 0 : 4,
                reverse ? 4 : 0, 32, .25, false, kernel);
            // DPR 3 and fractional motion exercise translated gradient coordinates.
            var matrix = SKMatrix.Concat(SKMatrix.CreateTranslation(.375f, -.625f),
                SKMatrix.CreateScale(3, 3));
            var capture = SkiaSceneRenderer.VariableBlurCaptureBounds(visible, settings,
                tile, matrix, width, height);
            if (capture == new SKRectI(0, 0, width, height))
                throw new Exception("Capture comparison did not exercise a partial domain.");
            SKImage Blur(SKImage image, SKMatrix transform)
            {
                var w = (int)Math.Ceiling(image.Width * scale);
                var h = (int)Math.Ceiling(image.Height * scale);
                var sx = (float)w / image.Width;
                var sy = (float)h / image.Height;
                using var reducedSurface = SKSurface.Create(new SKImageInfo(w, h));
                reducedSurface.Canvas.DrawImage(image, SKRect.Create(w, h),
                    new SKSamplingOptions(SKFilterMode.Linear));
                using var reduced = reducedSurface.Snapshot();
                var workingMatrix = SKMatrix.Concat(SKMatrix.CreateScale(sx, sy), transform);
                workingMatrix.TryInvert(out var inverse);
                var dx = settings.End.dx - settings.Start.dx;
                var dy = settings.End.dy - settings.Start.dy;
                var length = Math.Sqrt(dx * dx + dy * dy);
                var ux = dx / length;
                var uy = dy / length;
                SKImage Pass(SKImage sourceImage, double ax, double ay)
                {
                    using var child = sourceImage.ToShader(
                        tile == TileMode.clamp ? SKShaderTileMode.Clamp : SKShaderTileMode.Decal,
                        tile == TileMode.clamp ? SKShaderTileMode.Clamp : SKShaderTileMode.Decal,
                        new SKSamplingOptions(SKFilterMode.Linear));
                    using var uniforms = new SKRuntimeEffectUniforms(effect)
                    {
                        ["size"] = new float[] { w, h },
                        ["ramp"] = new float[] {
                            (float)((ux * inverse.ScaleX + uy * inverse.SkewY) / length),
                            (float)((ux * inverse.SkewX + uy * inverse.ScaleY) / length),
                            (float)((ux * inverse.TransX + uy * inverse.TransY) / length) },
                        ["sigmas"] = new float[] { (float)settings.StartSigma, (float)settings.EndSigma },
                        ["axis"] = new float[] {
                            (float)(workingMatrix.ScaleX * ax + workingMatrix.SkewX * ay),
                            (float)(workingMatrix.SkewY * ax + workingMatrix.ScaleY * ay) },
                        ["samples"] = 32f,
                        ["fastKernel"] = kernel == VariableBlurKernel.fastGaussian ? 1f : 0f,
                    };
                    using var children = new SKRuntimeEffectChildren(effect) { ["inputImage"] = child };
                    using var shader = effect.ToShader(uniforms, children);
                    using var output = SKSurface.Create(new SKImageInfo(w, h));
                    using var shaderPaint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
                    output.Canvas.DrawRect(SKRect.Create(w, h), shaderPaint);
                    return output.Snapshot();
                }
                using var first = Pass(reduced, ux, uy);
                return Pass(first, -uy, ux);
            }
            using var full = Blur(input, matrix);
            using var croppedInput = source.Snapshot(capture);
            using var cropped = Blur(croppedInput, SKMatrix.Concat(
                SKMatrix.CreateTranslation(-capture.Left, -capture.Top), matrix));
            SKBitmap Restore(SKImage image, SKRect destination)
            {
                var bitmap = new SKBitmap(width, height);
                using var canvas = new SKCanvas(bitmap);
                canvas.Clear(SKColors.Transparent);
                canvas.DrawImage(image, destination, new SKSamplingOptions(SKFilterMode.Linear));
                return bitmap;
            }
            using var expected = Restore(full, SKRect.Create(width, height));
            using var actual = Restore(cropped, capture);
            for (var y = (int)Math.Ceiling(visible.Top); y < Math.Floor(visible.Bottom); y++)
            for (var x = (int)Math.Ceiling(visible.Left); x < Math.Floor(visible.Right); x++)
            {
                var a = expected.GetPixel(x, y);
                var b = actual.GetPixel(x, y);
                var error = new[] { Math.Abs(a.Red - b.Red), Math.Abs(a.Green - b.Green),
                    Math.Abs(a.Blue - b.Blue), Math.Abs(a.Alpha - b.Alpha) }.Max();
                maxError = Math.Max(maxError, error);
                if (error > 2) throw new Exception($"Capture pixel mismatch at {x},{y}: {a}/{b}; " +
                    $"scale={scale}, kernel={kernel}, tile={tile}, reverse={reverse}");
            }
            cases++;
        }
        Console.WriteLine($"PASS: per-axis capture vs full-domain raster shader pixels ({cases} cases, max channel error {maxError}/255).");
    }
}
