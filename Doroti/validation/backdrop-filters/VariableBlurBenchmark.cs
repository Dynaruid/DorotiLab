using System.Diagnostics;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class VariableBlurBenchmark
{
    internal static void Run(
        SkiaSceneRenderer renderer,
        VulkanFixture fixture,
        bool fullResolutionOnly
    )
    {
        Console.WriteLine(
            "OFFSCREEN FILTER BENCHMARK — NOT FPS; excludes framework, window presentation and host frame-pool lifecycle."
        );
        foreach (var (width, height) in new[] { (2560, 1600) })
        foreach (var fraction in new[] { 1.0, 0.25 })
        foreach (
            var (scale, adaptive, kernel) in fullResolutionOnly
                ? new[] { (1.0, true, VariableBlurKernel.gaussian) }
                : new[]
                {
                    (1.0, true, VariableBlurKernel.gaussian),
                    (0.5, true, VariableBlurKernel.gaussian),
                    (0.25, true, VariableBlurKernel.gaussian),
                    (0.25, false, VariableBlurKernel.gaussian),
                    (0.25, true, VariableBlurKernel.fastGaussian),
                }
        )
        {
            using var surface = fixture.CreateSurface(
                new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul)
            );
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            for (var y = 0; y < height; y += 16)
            for (var x = 0; x < width; x += 16)
                canvas.drawRect(
                    new Rect(x, y, x + 16, y + 16),
                    new Paint
                    {
                        color = new Color((x / 16 + y / 16) % 2 == 0 ? 0xffe06020u : 0xff4080c0u),
                        isAntiAlias = false,
                    }
                );
            using var picture = recorder.endRecording();
            var backgroundBuilder = new SceneBuilder(1);
            backgroundBuilder.addPicture(Offset.zero, picture);
            using var backgroundScene = backgroundBuilder.build();
            renderer.DrawPlatformRasterSegment(
                surface.Canvas,
                backgroundScene.Commands,
                width,
                height
            );
            using var background = surface.Snapshot();
            fixture.Complete();
            var builder = new SceneBuilder(1);
            builder.pushClipRect(
                new Rect(0, 0, width, height * fraction),
                clipBehavior: Clip.hardEdge
            );
            builder.pushBackdropFilter(
                ImageFilter.variableBlur(
                    Offset.zero,
                    new Offset(0, height * fraction),
                    endSigma: 20,
                    resolutionScale: scale,
                    adaptiveResolution: adaptive,
                    kernel: kernel
                )
            );
            builder.pop();
            builder.pop();
            using var scene = builder.build();
            var times = new List<double>();
            var recordTimes = new List<double>();
            for (var frame = 0; frame < 15; frame++)
            {
                var timer = Stopwatch.StartNew();
                surface.Canvas.DrawImage(background, 0, 0, SKSamplingOptions.Default);
                renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, width, height);
                var record = timer.Elapsed.TotalMilliseconds;
                fixture.Complete();
                if (frame >= 3)
                {
                    times.Add(timer.Elapsed.TotalMilliseconds);
                    recordTimes.Add(record);
                }
            }
            times.Sort();
            recordTimes.Sort();
            Console.WriteLine(
                $"BENCH {width}x{height} coverage={fraction:P0} scale={scale} adaptive={adaptive} kernel={kernel} recordMedian={recordTimes[6]:F3}ms completeMedian={times[6]:F3}ms p95={times[^1]:F3}ms (12 frames, 3 warmups; includes GPU wait, excludes readback/presentation)"
            );
            if (width == 640 && fraction == 1 && fixture.Context is not null)
            {
                using var pixels = new SKBitmap(
                    new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul)
                );
                if (!surface.ReadPixels(pixels.Info, pixels.GetPixels(), pixels.RowBytes, 0, 0))
                    throw new Exception("Benchmark readback failed");
                using var image = SKImage.FromBitmap(pixels);
                using var png = image.Encode(SKEncodedImageFormat.Png, 100);
                var directory = "Doroti/artifacts/variable-blur-performance";
                Directory.CreateDirectory(directory);
                using var file = File.Create(
                    System.IO.Path.Combine(directory, $"scale-{scale}-adaptive-{adaptive}.png")
                );
                png.SaveTo(file);
            }
        }
    }
}
