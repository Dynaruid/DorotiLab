using System.Text.Json;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Runtime;
using Doroti.Ui;
using Material = Doroti.Framework.Material;

namespace Doroti.Validation;

// Linked into a validation-only Web build as well as the native standalone fixture.
public static partial class ImagePipelineValidation
{


    public static async Task VerifyPixels()
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawRect(new Rect(0, 0, 1, 1), new Paint { color = new Color(0xffff0000) });
        canvas.drawRect(new Rect(1, 0, 2, 1), new Paint { color = new Color(0x8000ff00) });
        using var picture = recorder.endRecording();
        using var image = await picture.toImage(3, 1);
        var raw = await image.toByteData();
        Require(raw!.asMemory().Span.SequenceEqual(new byte[] { 255, 0, 0, 255, 0, 128, 0, 128, 0, 0, 0, 0 }), "premultiplied RGBA pixels");
        var straight = await image.toByteData(ImageByteFormat.rawStraightRgba);
        Require(straight!.asMemory().Span.SequenceEqual(new byte[] { 255, 0, 0, 255, 0, 255, 0, 128, 0, 0, 0, 0 }), "straight RGBA pixels");
        var pending = image.toByteData();
        using var clone = image.clone();
        image.Dispose();
        Require((await pending)!.lengthInBytes == 12, "read owns storage through original disposal");
        Require((await clone.toByteData())!.lengthInBytes == 12, "clone outlives original");
        await Throws<ObjectDisposedException>(async () => { await image.toByteData(); });
        var png = await clone.toByteData(ImageByteFormat.png);
        using var decoded = await View().DecodeImageAsync(png!.asMemory(), DartUiInvocation.Managed("image-validation"));
        Require((await decoded.toByteData())!.asMemory().Span.SequenceEqual(raw.asMemory().Span), "PNG round trip");
        await Throws<ArgumentOutOfRangeException>(async () => { await picture.toImage(0, 1); });
        await Throws<ArgumentOutOfRangeException>(async () => { await clone.toByteData((ImageByteFormat)99); });
        picture.Dispose();
        await Throws<ObjectDisposedException>(async () => { await picture.toImage(1, 1); });
        await VerifyStreams(clone);
    }

    private static async Task VerifyStreams(Image image)
    {
        // A ready single-frame codec must deliver even when getNextFrame completes synchronously.
        var stream = new MultiFrameImageStreamCompleter(new SynchronousFuture<Codec>(
            new Codec([new FrameInfo(image.clone(), Duration.zero)])), 1);
        var delivered = new TaskCompletionSource();
        ImageStreamListener listener = null!;
        listener = new ImageStreamListener((info, _) =>
        {
            info.dispose();
            stream.removeListener(listener);
            delivered.TrySetResult();
        });
        stream.addListener(listener);
        await delivered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var codec = new TaskCompletionSource<Codec>();
        var failed = new MultiFrameImageStreamCompleter(Future<Codec>.fromTask(codec.Task), 1);
        var failure = new TaskCompletionSource<object>();
        var ephemeral = new TaskCompletionSource<object>();
        var errorListener = new ImageStreamListener((info, _) => info.dispose(), onError: (error, _) => failure.TrySetResult(error));
        failed.addListener(errorListener);
        failed.addEphemeralErrorListener((error, _) => ephemeral.TrySetResult(error));
        var expected = new InvalidDataException("fixture decode failed");
        codec.SetException(expected);
        Require(ReferenceEquals(await failure.Task.WaitAsync(TimeSpan.FromSeconds(5)), expected), "codec error reaches listener");
        Require(ReferenceEquals(await ephemeral.Task.WaitAsync(TimeSpan.FromSeconds(5)), expected), "codec error reaches ephemeral listener");
        failed.removeListener(errorListener);
    }

    public static async Task VerifyProviderFailure()
    {
        try
        {
            await Material.ColorScheme.fromImageProvider(new MemoryImage(new Uint8List(new byte[] { 1, 2, 3, 4 })));
        }
        catch (Exception exception) when (exception is not TimeoutException)
        {
            // Invalid encoded data must fail through the stream, before the 30-second safety timeout.
            return;
        }
        throw new InvalidOperationException("Corrupt MemoryImage did not report a decode failure.");
    }

    public static Dictionary<string, string> PaletteFixtures()
    {
        var fixtures = new Dictionary<string, long[]>
        {
            ["white"] = Enumerable.Repeat(0xffffffffL, 112 * 112).ToArray(),
            ["near-colors"] = Enumerable.Range(0, 64).Select(index => 0xff808080L + ((index % 4) << 16) + ((index / 4 % 4) << 8) + index / 16).ToArray(),
            ["alpha"] = [0x00ff0000, 0x80ff0000, 0xffff0000, 0xffff0000, 0xff00ff00, 0xff0000ff],
            ["transparent"] = [0, 0],
        };
        return fixtures.ToDictionary(entry => entry.Key, entry =>
        {
            var pixels = entry.Value;
            var rgba = pixels.SelectMany(pixel => new[] { (byte)(pixel >> 16), (byte)(pixel >> 8), (byte)pixel, (byte)(pixel >> 24) }).ToArray();
            var colors = MaterialImageColorRuntime.Quantize(pixels);
            var seed = MaterialImageColorRuntime.Score(colors)[0];
            var schemes = new Dictionary<string, Dictionary<string, long>>();
            foreach (var brightness in new[] { Brightness.light, Brightness.dark })
            {
                var scheme = Material.ColorScheme.CreateFromSeed(new Color(seed), brightness: brightness);
                schemes[brightness.ToString()] = typeof(Material.ColorScheme).GetProperties().Where(property => property.PropertyType == typeof(Color))
                    .ToDictionary(property => property.Name, property => (long)((Color)property.GetValue(scheme)!).value);
            }
            return JsonSerializer.Serialize(new { rgba = Convert.ToBase64String(rgba), seed, colors, schemes });
        });
    }

    public static async Task<string> Inspect(byte[] encoded, bool realProvider = false, string? networkUrl = null)
    {
        using var image = await View().DecodeImageAsync(encoded, DartUiInvocation.Managed("image-validation"));
        var scale = Math.Min(1.0, 112.0 / Math.Max(image.width, image.height));
        var width = image.width * scale;
        var height = image.height * scale;
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        Decoration_imageLibrary.paintImage(canvas: canvas, rect: new Rect(0, 0, width, height), image: image, filterQuality: FilterQuality.none);
        using var picture = recorder.endRecording();
        using var scaled = await picture.toImage(Math.Max(1, (int)width), Math.Max(1, (int)height));
        var bytes = (await scaled.toByteData())!.asMemory().ToArray();
        var colors = MaterialImageColorRuntime.Quantize(MaterialImageColorRuntime.ArgbFromRgba(bytes));
        var seed = MaterialImageColorRuntime.Score(colors)[0];
        var schemes = new Dictionary<string, Dictionary<string, long>>();
        foreach (var brightness in new[] { Brightness.light, Brightness.dark })
        {
            dynamic provider = networkUrl is not null ? (object)new NetworkImageIo(networkUrl)
                : realProvider ? (object)new MemoryImage(new Uint8List(encoded)) : new ImmediateImageProvider(image);
            var actual = await Material.ColorScheme.fromImageProvider(provider, brightness: brightness);
            var expected = Material.ColorScheme.CreateFromSeed(seedColor: new Color(seed), brightness: brightness);
            var roles = typeof(Material.ColorScheme).GetProperties().Where(property => property.PropertyType == typeof(Color))
                .ToDictionary(property => property.Name, property => (long)((Color)property.GetValue(actual)!).value);
            foreach (var property in typeof(Material.ColorScheme).GetProperties().Where(property => property.PropertyType == typeof(Color)))
                Require(roles[property.Name] == ((Color)property.GetValue(expected)!).value, $"image scheme role {brightness}/{property.Name}");
            schemes[brightness.ToString()] = roles;
        }
        return JsonSerializer.Serialize(new { width = image.width, height = image.height,
            scaledWidth = scaled.width, scaledHeight = scaled.height, rgba = Convert.ToBase64String(bytes),
            seed, colors, schemes });
    }

    private static DorotiView View() => PlatformDispatcher.instance.views.First();
    private static void Require(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException($"Image contract failed: {name}");
    }
    private static async Task Throws<T>(Func<Task> action) where T : Exception
    {
        try { await action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}");
    }
}

public sealed class ImmediateImageProvider(Image image)
{
    public ImageStream resolve(ImageConfiguration configuration) => new ImmediateStream(image);
    private sealed class ImmediateStream(Image image) : ImageStream
    {
        public override void addListener(ImageStreamListener listener) => listener.onImage(new ImageInfo(image.clone()), true);
        public override void removeListener(ImageStreamListener listener) { }
    }
}
