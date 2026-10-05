using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurKernelRegression
{
    // Execute the embedded production SkSL on Skia's raster runtime-effect
    // backend. This isolates the kernel from Adaptive masks and GPU scheduling.
    public static void Run()
    {
        RenderingShaderAssets.Register();
        var program = FrameworkShaderLoader.LoadEmbeddedProgram("rendering.variable-blur");
        using var effect =
            SKRuntimeEffect.CreateShader(program.source, out var error)
            ?? throw new Exception(error);
        const int width = 257,
            height = 3,
            center = width / 2;
        using var source = SKSurface.Create(new SKImageInfo(width, height));
        source.Canvas.Clear(SKColors.Black);
        using (var paint = new SKPaint { Color = SKColors.White })
            source.Canvas.DrawRect(center, 0, 1, height, paint);
        using var input = source.Snapshot();
        using var child = input.ToShader(
            SKShaderTileMode.Clamp,
            SKShaderTileMode.Clamp,
            new SKSamplingOptions(SKFilterMode.Linear)
        );

        float[] Render(float sigma, float fast, float scale = 1)
        {
            using var uniforms = new SKRuntimeEffectUniforms(effect)
            {
                ["size"] = new float[] { width, height },
                ["ramp"] = new float[] { 0, 0, 0 },
                ["sigmas"] = new float[] { sigma, sigma },
                ["axis"] = new float[] { scale, 0 },
                ["samples"] = 32f,
                ["fastKernel"] = fast,
            };
            using var children = new SKRuntimeEffectChildren(effect) { ["inputImage"] = child };
            using var shader = effect.ToShader(uniforms, children);
            using var bitmap = new SKBitmap(
                new SKImageInfo(width, height, SKColorType.RgbaF32, SKAlphaType.Premul)
            );
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint { Shader = shader, BlendMode = SKBlendMode.Src };
            canvas.DrawRect(0, 0, width, height, paint);
            var pixels = new float[width * height * 4];
            Marshal.Copy(bitmap.GetPixels(), pixels, 0, pixels.Length);
            return Enumerable.Range(0, width).Select(x => pixels[(width + x) * 4]).ToArray();
        }

        foreach (var sigma in new[] { 0f, 1f, 2f })
        {
            var fast = Render(sigma, 1);
            var gaussian = Render(sigma, 0);
            if (fast.Zip(gaussian).Any(pair => Math.Abs(pair.First - pair.Second) > .0001))
                throw new Exception($"Fast blur changed the sharp/weak kernel at sigma {sigma}.");
        }
        // At DPR 3 and 1/4 working resolution, the sample's sigma 20/32 means
        // working sigma 15/24. A seven-point dilated filter leaves bright copies.
        foreach (
            var (sigma, scale) in new[]
            {
                (2.01f, 1f),
                (2.5f, 1f),
                (2.99f, 1f),
                (3f, 1f),
                (4f, 1f),
                (8f, 1f),
                (20f, .75f),
                (32f, .75f),
            }
        )
        {
            var actual = Render(sigma, 1, scale);
            var workingSigma = sigma * scale;
            var expected = Enumerable
                .Range(0, width)
                .Select(x =>
                {
                    var distance = x - center;
                    return Math.Abs(distance) <= 3 * workingSigma
                        ? Math.Exp(-.5 * distance * distance / (workingSigma * workingSigma))
                        : 0;
                })
                .ToArray();
            var total = expected.Sum();
            for (var x = 0; x < width; x++)
                expected[x] /= total;
            var difference = actual.Zip(expected).Sum(pair => Math.Abs(pair.First - pair.Second));
            var tolerance = workingSigma >= 3 ? .02 : .08;
            if (actual.Max() > 1.5 * expected[center] || difference > tolerance)
                throw new Exception(
                    $"Fast blur repeats the impulse at working sigma {workingSigma}: peak={actual.Max():F5}, Gaussian peak={expected[center]:F5}, L1={difference:F5}."
                );
            if (Math.Abs(actual.Sum() - 1) > .03)
                throw new Exception($"Fast blur lost impulse energy at sigma {workingSigma}.");
            Console.WriteLine(
                $"Fast blur working sigma={workingSigma:F2}: peak={actual.Max():F5}, Gaussian peak={expected[center]:F5}, L1={difference:F5}"
            );
        }
        Console.WriteLine(
            "PASS: production Variable Blur SkSL impulse response, DPR/working scale and sharp-end preservation (raster runtime effect; not GPU validation)."
        );
    }
}
