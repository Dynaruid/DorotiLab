using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurKawaseRegression
{
    public static void Run()
    {
        FrameworkShaderLoader.RegisterResourceOwner(typeof(SkiaSceneRenderer).Assembly);
        using var kawase = Load("rendering.kawase-blur");
        using var interval = Load("rendering.variable-blur-stage");
        const int width = 2048, height = 128;
        SKImage Pass(SKImage source, int w, int h, bool up)
        {
            using var child = source.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp,
                new SKSamplingOptions(SKFilterMode.Linear));
            using var uniforms = new SKRuntimeEffectUniforms(kawase)
            {
                ["size"] = new float[] { w, h },
                ["ratio"] = new float[] { (float)source.Width / w, (float)source.Height / h },
                ["upsample"] = up ? 1f : 0f,
            };
            using var children = new SKRuntimeEffectChildren(kawase) { ["inputImage"] = child };
            using var shader = kawase.ToShader(uniforms, children);
            using var surface = SKSurface.Create(new SKImageInfo(w, h, SKColorType.RgbaF32, SKAlphaType.Premul));
            using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
            surface.Canvas.DrawRect(SKRect.Create(w, h), paint);
            return surface.Snapshot();
        }
        foreach (var depth in Enumerable.Range(2, 6))
        {
            var measured = new List<double>();
            foreach (var phase in new[] { 0, 1, (1 << depth) / 2, (1 << depth) - 1 }.Distinct())
            {
                var center = width / 2 + phase;
                using var source = SKSurface.Create(new SKImageInfo(width, height, SKColorType.RgbaF32, SKAlphaType.Premul));
                source.Canvas.Clear(SKColors.Transparent);
                using (var white = new SKPaint { Color = SKColors.White })
                    source.Canvas.DrawRect(center, 0, 1, height, white);
                var downs = new List<SKImage> { source.Snapshot() };
                SKImage? reconstructed = null;
                try
                {
                    for (var level = 1; level <= depth; level++)
                        downs.Add(Pass(downs[^1], width >> level, height >> level, false));
                    for (var level = depth - 1; level >= (depth == 2 ? 1 : 2); level--)
                    {
                        var next = Pass(reconstructed ?? downs[^1], width >> level, height >> level, true);
                        reconstructed?.Dispose();
                        reconstructed = next;
                    }
                    using var bitmap = new SKBitmap(new SKImageInfo(width, 1, SKColorType.RgbaF32, SKAlphaType.Premul));
                    using (var canvas = new SKCanvas(bitmap))
                        canvas.DrawImage(reconstructed!, SKRect.Create(width, 1), new SKSamplingOptions(SKFilterMode.Linear));
                    var rgba = new float[width * 4];
                    Marshal.Copy(bitmap.GetPixels(), rgba, 0, rgba.Length);
                    var response = Enumerable.Range(0, width).Select(x => (double)rgba[x * 4]).ToArray();
                    var energy = response.Sum();
                    var centroid = response.Select((v, x) => v * x).Sum() / energy;
                    var variance = response.Select((v, x) => v * (x - centroid) * (x - centroid)).Sum() / energy;
                    var sigma = Math.Sqrt(variance);
                    measured.Add(sigma);
                    if (Math.Abs(energy - 1) > .015 || Math.Abs(centroid - center) > .15)
                        throw new Exception($"Kawase impulse energy/phase mismatch: depth={depth}, phase={phase}, energy={energy:F4}, centroid={centroid-center:F4}.");
                    if (Math.Abs(sigma / SkiaSceneRenderer.VariableBlurKawaseSigma(depth) - 1) > .08)
                        throw new Exception($"Kawase sigma calibration mismatch: depth={depth}, phase={phase}, measured={sigma:F4}.");
                    // Widely spaced fixed taps create isolated repeated impulses.
                    for (var x = center + (int)Math.Ceiling(sigma); x < width - 1; x++)
                        if (response[x + 1] > response[x] + .03 * response.Max())
                            throw new Exception($"Kawase repeated contour at depth={depth}, x={x}.");
                }
                finally
                {
                    reconstructed?.Dispose();
                    foreach (var downImage in downs) downImage.Dispose();
                }
            }
            Console.WriteLine($"Kawase depth={depth}: impulse sigma {measured.Min():F4}..{measured.Max():F4}, calibrated={SkiaSceneRenderer.VariableBlurKawaseSigma(depth):F4}");
        }

        // Exercise the actual interval shader at endpoints and exact boundaries,
        // including alpha. One half-open interval must own every output pixel.
        using var constant = SKSurface.Create(new SKImageInfo(32, 1));
        constant.Canvas.Clear(new SKColor(80, 140, 200, 128));
        using var image = constant.Snapshot();
        using var imageShader = image.ToShader();
        var bounds = new[] { 2f, (float)SkiaSceneRenderer.VariableBlurKawaseSigma(2),
                (float)SkiaSceneRenderer.VariableBlurKawaseSigma(3), (float)SkiaSceneRenderer.VariableBlurKawaseSigma(4) };
        foreach (var reverse in new[] { false, true })
        foreach (var constantSigma in new[] { float.NaN, 0, 2, bounds[1], bounds[2], bounds[3] })
        {
            using var result = SKSurface.Create(new SKImageInfo(32, 1));
            for (var band = 0; band < 3; band++)
            {
                using var uniforms = new SKRuntimeEffectUniforms(interval)
                {
                    ["size"] = new float[] { 32, 1 },
                    ["ramp"] = new float[] { 1f / 31, 0, -.5f / 31 },
                    ["sigmas"] = float.IsNaN(constantSigma)
                        ? new float[] { reverse ? bounds[^1] : 0, reverse ? 0 : bounds[^1] }
                        : new float[] { constantSigma, constantSigma },
                    ["interval"] = new float[] { bounds[band], bounds[band + 1] },
                    ["lowerRatio"] = new float[] { 1, 1 }, ["upperRatio"] = new float[] { 1, 1 },
                    ["lastInterval"] = band == 2 ? 1f : 0f,
                    ["firstInterval"] = band == 0 ? 1f : 0f,
                    ["lowerOffset"] = new float[] { 0, 0 },
                };
                using var children = new SKRuntimeEffectChildren(interval)
                    { ["inputImage"] = imageShader, ["upperImage"] = imageShader };
                using var shader = interval.ToShader(uniforms, children);
                using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Plus };
                result.Canvas.DrawRect(SKRect.Create(32, 1), paint);
            }
            using var output = result.Snapshot();
            using var bitmap = SKBitmap.FromImage(output);
            for (var x = 0; x < 32; x++)
                if (Math.Abs(bitmap.GetPixel(x, 0).Alpha - 128) > 1)
                    throw new Exception("Variable Kawase intervals lost or duplicated premultiplied coverage.");
        }
        var settings = new VariableBlurSettings(new(0, 0), new(0, 100), 20, 0, 32, .25, true, VariableBlurKernel.dualKawase);
        foreach (var tile in new[] { TileMode.decal, TileMode.repeated, TileMode.mirror })
            if (SkiaSceneRenderer.VariableBlurKawaseGeometry(settings, tile, SKMatrix.Identity, out _, out _, out _))
                throw new Exception("Unsupported Kawase tile mode did not fall back.");
        foreach (var transform in new[] { SKMatrix.CreateScale(2, 1), SKMatrix.CreateSkew(.1f, 0) })
            if (SkiaSceneRenderer.VariableBlurKawaseGeometry(settings, TileMode.clamp, transform, out _, out _, out _))
                throw new Exception("Unsupported Kawase transform did not fall back.");
        var coarseCoordinates = settings with { Start = new(1e9, 0), End = new(1e9 + 64, 0), StartSigma = 124 };
        if (SkiaSceneRenderer.VariableBlurKawaseGeometry(coarseCoordinates, TileMode.clamp, SKMatrix.Identity,
            out _, out _, out var coordinateReason) || coordinateReason != "kawase-coordinate-fallback")
            throw new Exception("Kawase detail ramp collapsed at shader precision without a fallback.");
        Console.WriteLine("PASS: production Dual Kawase impulse calibration, energy, phase, contours, interval alpha and explicit fallbacks (raster; not GPU validation).");
    }

    private static SKRuntimeEffect Load(string id) => SKRuntimeEffect.CreateShader(
        FrameworkShaderLoader.LoadEmbeddedProgram(id).source, out var error) ?? throw new Exception(error);
}
