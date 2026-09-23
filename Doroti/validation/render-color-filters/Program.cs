using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

using var renderer = new SkiaSceneRenderer(
    1, new Host(), null, null, "color-filter-contract", "skia-raster", "color-filter-contract",
    enablePictureRasterCache: false
);
var failures = new List<string>();
var matrix = new double[]
{
    0, 0, 1, 0, 0,
    1, 0, 0, 0, 0,
    0, 1, 0, 0, 0,
    0, 0, 0, .5, 0,
};
var cases = new (string Name, ColorFilter Filter, Func<SKColorFilter> Native)[]
{
    ("mode", ColorFilter.mode(new Color(0xff2479c3), BlendMode.srcIn),
        () => SKColorFilter.CreateBlendMode(new SKColor(0xff2479c3), SKBlendMode.SrcIn)),
    ("matrix-alpha", ColorFilter.matrix(matrix),
        () => SKColorFilter.CreateColorMatrix(matrix.Select(value => (float)value).ToArray())),
    ("linear-to-srgb", ColorFilter.linearToSrgbGamma(), SKColorFilter.CreateLinearToSrgbGamma),
    ("srgb-to-linear", ColorFilter.srgbToLinearGamma(), SKColorFilter.CreateSrgbToLinearGamma),
};
foreach (var (name, filter, native) in cases)
{
    foreach (var target in new[] { "paint", "saveLayer", "scene" })
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder, new Rect(0, 0, 64, 64));
        if (target == "saveLayer") canvas.saveLayer(null, new Paint { colorFilter = filter });
        DrawUi(canvas, target == "paint" ? filter : null);
        if (target == "saveLayer") canvas.restore();
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(1);
        ColorFilterEngineLayer? retained = null;
        if (target == "scene")
        {
            builder.pushOffset(3, 2);
            builder.pushOpacity(191);
            retained = builder.pushColorFilter(filter);
        }
        builder.addPicture(Offset.zero, picture, new Rect(0, 0, 64, 64));
        if (target == "scene")
        {
            builder.pop();
            builder.pop();
            builder.pop();
        }
        using var scene = builder.build();
        using var nativeFilter = native();
        var expected = Raster(skia =>
        {
            skia.Save();
            if (target == "scene")
            {
                skia.Translate(3, 2);
                using var opacity = new SKPaint { Color = SKColors.White.WithAlpha(191) };
                skia.SaveLayer(opacity);
            }
            if (target != "paint")
            {
                using var layerPaint = new SKPaint { ColorFilter = nativeFilter };
                skia.SaveLayer(layerPaint);
            }
            DrawNative(skia, target == "paint" ? nativeFilter : null);
            if (target != "paint") skia.Restore();
            if (target == "scene") skia.Restore();
            skia.Restore();
            DrawSibling(skia);
        });
        // The same commands exercise direct replay, SKPicture recording and cache hit.
        var passes = target == "paint" ? 3 : 1;
        var before = renderer.Diagnostics.Work!;
        for (var pass = 0; pass < passes; pass++)
        {
            Verify($"{name}/{target}/{pass}", expected, Render(scene));
        }
        if (target == "paint")
        {
            var after = renderer.Diagnostics.Work!;
            if (after.CommandRecordings <= before.CommandRecordings || after.CommandCacheHits <= before.CommandCacheHits)
            {
                failures.Add($"{name}: SKPicture recording/cache hit was not exercised");
            }
        }
        if (retained is not null)
        {
            var retainedBuilder = new SceneBuilder(1);
            retainedBuilder.pushOffset(3, 2);
            retainedBuilder.pushOpacity(191);
            retainedBuilder.addRetained(retained);
            retainedBuilder.pop();
            retainedBuilder.pop();
            using var retainedScene = retainedBuilder.build();
            Verify($"{name}/retained", expected, Render(retainedScene));
        }
    }
}
if (failures.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, failures));
Console.WriteLine("PASS: 24 native pixel comparisons (paint, saveLayer, nested scene, retained and command cache)");

SKColor[] Render(Scene scene) => Raster(canvas =>
{
    var saveCount = canvas.SaveCount;
    renderer.DrawPlatformRasterSegment(canvas, scene.Commands, 64, 64);
    if (canvas.SaveCount != saveCount) throw new InvalidOperationException("Unbalanced canvas saves");
    DrawSibling(canvas);
});

void Verify(string name, SKColor[] expected, SKColor[] actual)
{
    var different = expected.Zip(actual).Count(pair => pair.First != pair.Second);
    if (different > 0) failures.Add($"{name}: {different} differing pixels");
    Console.WriteLine($"{(different == 0 ? "PASS" : "FAIL")}: {name} ({different} differing pixels)");
}

static SKColor[] Raster(Action<SKCanvas> draw)
{
    using var surface = SKSurface.Create(new SKImageInfo(64, 64));
    surface.Canvas.Clear(new SKColor(0xff352719));
    draw(surface.Canvas);
    surface.Canvas.Flush();
    using var image = surface.Snapshot();
    using var bitmap = SKBitmap.FromImage(image);
    return bitmap.Pixels;
}

static void DrawUi(Canvas canvas, ColorFilter? filter)
{
    canvas.drawRect(new Rect(5, 6, 37, 39), new Paint { color = new Color(0xff926437), colorFilter = filter });
    canvas.drawRect(new Rect(18, 21, 49, 51), new Paint { color = new Color(0x8040a070), colorFilter = filter });
}

static void DrawNative(SKCanvas canvas, SKColorFilter? filter)
{
    using var paint = new SKPaint { Color = new SKColor(0xff926437), ColorFilter = filter, IsAntialias = true };
    canvas.DrawRect(new SKRect(5, 6, 37, 39), paint);
    paint.Color = new SKColor(0x8040a070);
    canvas.DrawRect(new SKRect(18, 21, 49, 51), paint);
}

static void DrawSibling(SKCanvas canvas)
{
    using var paint = new SKPaint { Color = SKColors.Yellow };
    canvas.DrawRect(new SKRect(52, 2, 62, 12), paint);
}

sealed class Host : ISkiaSceneRendererHost
{
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 64, 64, 64, 64, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 64, 64, 64, 64, 1, 1);
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
    public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public void UpdateSemantics(SemanticsUpdate update) { }
    public void ClearSemantics() { }
    public void RequestInvalidate() { }
}
