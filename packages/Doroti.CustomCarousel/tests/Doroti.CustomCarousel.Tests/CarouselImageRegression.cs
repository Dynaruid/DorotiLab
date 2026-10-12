using Doroti.Framework.Painting;
using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Ui;
using DorotiCarouselApp;
using System.Text.Json;
using Path = System.IO.Path;

internal static class CarouselImageRegression
{
    public static void Run(string output)
    {
        var source = new MemoryImage(new Uint8List(new byte[] { 1 }));
        var coverKey = new CarouselImageKey(source, 192, 256, BoxFit.cover);
        var cover = new CarouselImageProvider(coverKey);
        var size = cover.TargetSize(450, 1350);
        Require(size.width == 192 && size.height == 576, "Cover must preserve cropped pixels and the source aspect ratio.");
        size = new CarouselImageProvider(coverKey with { Fit = BoxFit.contain }).TargetSize(450, 1350);
        Require(size.width == 86 && size.height == 256, "Contain must preserve the full source aspect ratio.");
        size = cover.TargetSize(16, 8);
        Require(size.width == 16 && size.height == 8, "Small originals must not be upscaled.");
        Require(CarouselImageProvider.Bucket(240) == CarouselImageProvider.Bucket(241), "Small size changes must reuse the decode bucket.");
        Require(CarouselImageProvider.Bucket(480) >= 480, "DPR 2 must retain physical pixel density.");
        Require(cover.Equals(new CarouselImageProvider(coverKey)), "Equivalent providers must reuse cached pixels.");
        Require(!cover.Equals(new CarouselImageProvider(coverKey with { Width = 384 })), "DPR/size changes must invalidate the decode key.");

        Directory.CreateDirectory(output);
        var receipts = new List<object>();
        long previousBytes = 0;
        foreach (var dpr in new[] { 1, 2 })
        {
            using var tester = new WidgetTester(new Size(560, 850), dpr);
            tester.pumpWidget(new CarouselGallery());
            int pending = 0; long bytes = 0, count = 0;
            for (var frame = 0; frame < 30; frame++)
            {
                Thread.Sleep(20);
                tester.pump(TimeSpan.FromMilliseconds(16));
                tester.View.DispatchPlatformEvent(() =>
                {
                    var cache = PaintingBinding.instance.imageCache;
                    pending = checked((int)cache.pendingImageCount);
                    bytes = cache.currentSizeBytes;
                    count = cache.currentSize;
                });
                if (pending == 0 && count == 26) break;
            }
            Require(pending == 0 && count == 26, $"All 26 home images must decode at DPR {dpr}, pending={pending}, count={count}.");
            Require(bytes < (dpr == 1 ? 32L : 70L) * 1024 * 1024, $"Home must stay below the original 94MiB working set, got {bytes}.");
            Require(bytes > previousBytes, "DPR 2 must use higher-resolution images than DPR 1.");
            previousBytes = bytes;
            tester.WritePng(Path.Combine(output, $"home-dpr-{dpr}.png"));
            receipts.Add(new { devicePixelRatio = dpr, decodedImages = count, decodedBytes = bytes });
        }
        File.WriteAllText(Path.Combine(output, "image-cache.json"), JsonSerializer.Serialize(receipts, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"PASS: cover/contain aspect, source limits, stable cache keys, and 26 home images at DPR 1/2. Receipts: {output}");
    }

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
