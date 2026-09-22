using System.Reflection;
using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

if (!OperatingSystem.IsMacOS())
    throw new PlatformNotSupportedException("This GPU contract uses Metal on macOS.");
var device = Native.MTLCreateSystemDefaultDevice();
var queue = Native.Send(device, Native.Selector("newCommandQueue"));
try
{
    using var context =
        GRContext.CreateMetal(
            new GRMtlBackendContext { DeviceHandle = device, QueueHandle = queue }
        ) ?? throw new Exception("Metal context unavailable");
    using var cached = new SkiaSceneRenderer(
        1,
        new Host(),
        null,
        null,
        "budget",
        "budget",
        "budget",
        pictureRasterCachePixels: 64
    );
    using var direct = new SkiaSceneRenderer(
        1,
        new Host(),
        null,
        null,
        "direct",
        "direct",
        "direct",
        enablePictureRasterCache: false
    );
    using var actual = SKSurface.Create(context, true, new SKImageInfo(64, 64));
    using var expected = SKSurface.Create(context, true, new SKImageInfo(64, 64));
    var pictures = new List<Picture>();
    var builder = new SceneBuilder(1);
    for (var i = 0; i < 16; i++)
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawRect(
            Rect.fromLTWH(0, 0, 4, 4),
            new Paint { color = new Color(0xff000000L + i * 10000) }
        );
        var picture = recorder.endRecording();
        pictures.Add(picture);
        builder.addPicture(
            new Offset(i % 4 * 8, i / 4 * 8),
            picture,
            Rect.fromLTWH(0, 0, 4, 4),
            isComplexHint: true
        );
    }
    using var scene = builder.build();
    // The platform-segment entry point is normally inside Paint's frame scope.
    // Invoke that scope directly here to isolate native cache lifetime from the framework.
    var begin = typeof(SkiaSceneRenderer).GetMethod(
        "BeginPictureRasterFrame",
        BindingFlags.NonPublic | BindingFlags.Instance
    )!;
    for (var frame = 0; frame < 12; frame++)
    {
        begin.Invoke(cached, null);
        actual.Canvas.Clear(SKColors.Transparent);
        cached.DrawPlatformRasterSegment(actual.Canvas, scene.Commands, 64, 64);
        expected.Canvas.Clear(SKColors.Transparent);
        direct.DrawPlatformRasterSegment(expected.Canvas, scene.Commands, 64, 64);
        context.Flush(submit: true, synchronous: true);
        using var a = new SKBitmap(new SKImageInfo(64, 64));
        using var b = new SKBitmap(new SKImageInfo(64, 64));
        if (
            !actual.ReadPixels(a.Info, a.GetPixels(), a.RowBytes, 0, 0)
            || !expected.ReadPixels(b.Info, b.GetPixels(), b.RowBytes, 0, 0)
        )
            throw new Exception("GPU readback failed");
        if (!a.Pixels.SequenceEqual(b.Pixels))
            throw new Exception("Cache admission/eviction changed GPU pixels");
        if (cached.CaptureCacheMemory().RasterRgba8Bytes > 256)
            throw new Exception("Raster exceeded reserved budget");
    }
    var work = cached.Diagnostics.Work!;
    if (work.PromotionCount < 1 || cached.Diagnostics.PictureRasterCacheHits < 1)
        throw new Exception("GPU cache path was not exercised");
    Console.WriteLine(
        $"PASS native Metal budget, active-frame retention, replay pixels: promotions={work.PromotionCount}, bytes={cached.CaptureCacheMemory().RasterRgba8Bytes}"
    );
    foreach (var picture in pictures)
        picture.Dispose();
}
finally
{
    Native.Send(queue, Native.Selector("release"));
    Native.Send(device, Native.Selector("release"));
}

static class Native
{
    [DllImport("/System/Library/Frameworks/Metal.framework/Metal")]
    internal static extern nint MTLCreateSystemDefaultDevice();

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "sel_registerName")]
    internal static extern nint Selector(string name);

    [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
    internal static extern nint Send(nint receiver, nint selector);
}

sealed class Host : ISkiaSceneRendererHost
{
    public int Invalidations;
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 8, 8, 8, 8, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 8, 8, 8, 8, 1, 1);
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add { }
        remove { }
    }
    public event Action<long, TimeSpan>? InputReceived
    {
        add { }
        remove { }
    }
    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    public void UpdateSemantics(SemanticsUpdate update) { }

    public void ClearSemantics() { }

    public void RequestInvalidate() => Invalidations++;
}
