using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Skia.Rendering;
using SkiaSharp;
using Doroti.Framework.Painting;

internal static class ImageSizingContracts
{
    internal static async Task Verify()
    {
        using (var alive = new Doroti.Ui.Image(1, 2, 2))
        {
            var placeholder = new Doroti.Framework.Widgets.RawImage().createRenderObject(null!);
            placeholder.dispose();
            var valid = new Doroti.Framework.Widgets.RawImage(image: alive).createRenderObject(null!);
            valid.dispose();
            alive.Dispose();
            try
            {
                new Doroti.Framework.Widgets.RawImage(image: alive).createRenderObject(null!);
                throw new Exception("RawImage accepted a disposed image");
            }
            catch (AssertionError) { }
            catch (ObjectDisposedException) { }
        }
        using var environment = new ImageFixtureEnvironment();
        // This raster-only host has no frame pump. Give asynchronous provider
        // continuations an explicit offscreen scheduler, as a real host must.
        using var scheduler = DartAsyncRuntime.enterMicrotaskScheduler(callback => { _ = Task.Run(callback); });
        using var bitmap = new SKBitmap(4, 8);
        bitmap.Erase(new SKColor(200, 100, 50, 128));
        using var png = bitmap.Encode(SKEncodedImageFormat.Png, 100);
        var bytes = png.ToArray();
        var provider = new ResizeImage(new MemoryImage(new Uint8List(bytes)), width: 2, height: 2, policy: ResizeImagePolicy.fit);
        var key = await provider.obtainKey(ImageConfiguration.empty).asTask().WaitAsync(TimeSpan.FromSeconds(5));
        var fit = new TaskCompletionSource();
        var stream = provider.loadImage(key, (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)((buffer, getSize) =>
        {
            var size = getSize!(4, 8);
            Require(size.width == 1 && size.height == 2, "portrait ResizeImage.fit preserves ratio");
            fit.SetResult();
            return Dart_uiLibrary.instantiateImageCodecWithSize(buffer, getSize);
        }));
        await fit.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var delivered = new TaskCompletionSource();
        ImageStreamListener listener = null!;
        listener = new ImageStreamListener((info, _) =>
        {
            Require(info.image.width == 1 && info.image.height == 2, "fit provider delivers resized pixels");
            info.dispose();
            stream.removeListener(listener);
            delivered.SetResult();
        }, onError: (error, _) => delivered.TrySetException((Exception)error));
        stream.addListener(listener);
        await delivered.Task.WaitAsync(TimeSpan.FromSeconds(5));
        var asynchronous = new DeferredProvider();
        var resizedProvider = new ResizeImage(asynchronous, width: 2);
        var pendingKey = resizedProvider.obtainKey(ImageConfiguration.empty);
        asynchronous.Key.SetResult("ready");
        await pendingKey.asTask().WaitAsync(TimeSpan.FromSeconds(5));
        asynchronous = new DeferredProvider();
        pendingKey = new ResizeImage(asynchronous, width: 2).obtainKey(ImageConfiguration.empty);
        asynchronous.Key.SetException(new InvalidOperationException("key-error"));
        try { await pendingKey.asTask().WaitAsync(TimeSpan.FromSeconds(5)); throw new Exception("Key error was swallowed"); }
        catch (InvalidOperationException error) when (error.Message == "key-error") { }
        foreach (var item in new (long? Width, long? Height, bool Upscale, int W, int H)[]
        {
            (null, null, false, 4, 8), (2, null, false, 2, 4),
            (null, 2, false, 1, 2), (2, 3, false, 2, 3),
            (8, null, false, 4, 8), (null, 16, false, 4, 8),
            (8, null, true, 8, 16), (null, 16, true, 8, 16),
        })
        {
            using var buffer = await ImmutableBuffer.fromUint8List(new Uint8List(bytes));
            using var codec = await Dart_uiLibrary.instantiateImageCodecFromBuffer(buffer, item.Width, item.Height, item.Upscale);
            var frame = await codec.getNextFrame();
            using var image = frame.image;
            Require(image.width == item.W && image.height == item.H, $"codec dimensions {item}: {image.width}x{image.height}");
            await CheckPixels(image);
        }
        var fallback = new RasterFallback(environment.Renderer);
        using (var image = await ((IImageHostCapability)fallback).DecodeSizedAsync(bytes, (_, _) => new(2, null), false, DartUiInvocation.Managed("sizing-fallback")))
        {
            Require(image.width == 2 && image.height == 4, "default host fallback dimensions");
            await CheckPixels(image);
            Require(fallback.Original!.debugDisposed, "fallback releases original storage after rasterization");
        }
        try
        {
            await ((IImageHostCapability)fallback).DecodeSizedAsync(bytes, (_, _) => throw new InvalidOperationException("callback"), false, DartUiInvocation.Managed("sizing-failure"));
            throw new Exception("Target callback failure was swallowed");
        }
        catch (InvalidOperationException error) when (error.Message == "callback")
        { Require(fallback.Original!.debugDisposed, "callback failure releases decoded storage"); }

        using var source = typeof(MaterialSample.SampleImageDemo).Assembly.GetManifestResourceStream("MaterialSample.image.webp")!;
        using var encoded = new MemoryStream();
        source.CopyTo(encoded);
        using var webpData = SKData.CreateCopy(encoded.ToArray());
        using var webpCodec = SKCodec.Create(webpData);
        var nativeSize = webpCodec.GetScaledDimensions(1024f / 3648);
        Require(nativeSize.Width == 1024 && nativeSize.Height == 1536, "WebP codec scales during decode, before output bitmap allocation");
        using var sized = await environment.Renderer.DecodeSizedAsync(encoded.ToArray(), (width, height) =>
        {
            Require(width == 3648 && height == 5472, "callback receives intrinsic WebP dimensions");
            return new TargetImageSize(1024, null);
        }, false, DartUiInvocation.Managed("sizing-webp"));
        Require(sized.width == 1024 && sized.height == 1536, "sample WebP decodes at display size");
        Require((await sized.toByteData())!.asMemory().Length == 1024 * 1536 * 4, "display pixel storage size");
        Console.WriteLine("image sizing: codec aspect/exact/clamp/upscale, alpha pixels, fallback disposal, 1024x1536 WebP PASS");
    }

    private static async Task CheckPixels(Doroti.Ui.Image image)
    {
        var pixels = (await image.toByteData())!.asMemory().ToArray();
        for (var offset = 0; offset < pixels.Length; offset += 4)
            Require(Math.Abs(pixels[offset] - 100) <= 1 && Math.Abs(pixels[offset + 1] - 50) <= 1 &&
                Math.Abs(pixels[offset + 2] - 25) <= 1 && pixels[offset + 3] == 128, "premultiplied alpha pixels survive decoding and scaling");
    }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class DeferredProvider : ImageProvider<string>
    {
        internal readonly TaskCompletionSource<string> Key = new();
        public override Future<string> obtainKey(ImageConfiguration configuration) => Future<string>.fromTask(Key.Task);
    }
    private sealed class RasterFallback(SkiaSceneRenderer renderer) : IImageHostCapability
    {
        internal Doroti.Ui.Image? Original;
        public async ValueTask<Doroti.Ui.Image> DecodeAsync(ReadOnlyMemory<byte> bytes, DartUiInvocation invocation, CancellationToken cancellationToken = default)
            => Original = await renderer.DecodeAsync(bytes, invocation, cancellationToken);
        public ValueTask<Doroti.Ui.Image> RasterizeAsync(Picture picture, int width, int height, DartUiInvocation invocation, CancellationToken cancellationToken = default)
            => renderer.RasterizeAsync(picture, width, height, invocation, cancellationToken);
    }
}
