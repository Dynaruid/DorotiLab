using Doroti.Skia.Rendering;
using SkiaSharp;

internal static class ScaledImageDecodeRegression
{
    public static void Run()
    {
        foreach (var (width, height) in new[] { (800, 800), (801, 800), (901, 1350) })
        {
            using var bitmap = new SKBitmap(width, height);
            bitmap.Erase(new SKColor(40, 120, 200));
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
            foreach (var target in new[] { 192, 333, 552 })
            {
                using var codec = SKCodec.Create(data);
                var size = SkiaSceneRenderer.BrowserDecodeDimensions(codec.EncodedFormat, width, height, target, target);
                if (size.Width < target || size.Height < target || size.Width > width || size.Height > height)
                    throw new Exception($"JPEG decode must cover the target without upscaling: {size}.");
                if (size.Width == width && size.Height == height && target == 192)
                    throw new Exception("Small JPEG decode fell back to intrinsic size.");
                using var decoded = new SKBitmap(new SKImageInfo(size.Width, size.Height, SKColorType.Rgba8888, SKAlphaType.Premul));
                var result = codec.GetPixels(decoded.Info, decoded.GetPixels());
                if (result != SKCodecResult.Success)
                    throw new Exception($"JPEG {width}x{height} -> {size} must decode directly, got {result}.");
                var pixel = decoded.GetPixel(size.Width / 2, size.Height / 2);
                if (Math.Abs(pixel.Blue - 200) > 4 || Math.Abs(pixel.Green - 120) > 4)
                    throw new Exception("Scaled JPEG decode lost image content.");
            }
        }
        var png = SkiaSceneRenderer.BrowserDecodeDimensions(SKEncodedImageFormat.Png, 1400, 1400, 320, 320);
        if (png != new SKSizeI(1400, 1400)) throw new Exception("PNG must use its supported intrinsic decode before resizing.");
        var full = SkiaSceneRenderer.BrowserDecodeDimensions(SKEncodedImageFormat.Jpeg, 801, 800, 1600, 1600);
        if (full != new SKSizeI(801, 800)) throw new Exception("Decode dimensions must not upscale the source.");
        Console.WriteLine("PASS: browser JPEG scale selection decodes nine target/odd-dimension cases directly, preserves pixels and avoids upscaling.");
    }
}
