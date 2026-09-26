using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using BlendMode = Doroti.Ui.BlendMode;

var graphite = args.Contains("--graphite");
var gpu = args.Contains("--gpu") || graphite;
using var fixture = gpu ? new VulkanFixture(graphite) : null;
using var renderer = new SkiaSceneRenderer(
    1,
    new Host(),
    null,
    null,
    "backdrop-contract",
    DorotiSkiaRuntimeEffects.WindowsVulkanBackend,
    "backdrop-contract",
    enablePictureRasterCache: false
);
var swap = new double[] { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
var shader = FragmentProgram
    .fromSource(
        """
        uniform float2 size;
        uniform shader inputImage;
        half4 main(float2 p) {
            return inputImage.eval(p).bgra;
        }
        """,
        "backdrop-swap"
    )
    .fragmentShader();
var shaderFilter = new ImageFilter(shader);
var cases = new List<(string Name, ImageFilter Filter, Func<SKImageFilter> Native)>
{
    ("blur", new ImageFilter(2, 3), () => SKImageFilter.CreateBlur(2, 3, SKShaderTileMode.Clamp)),
    ("dilate", ImageFilter.dilate(3, 2), () => SKImageFilter.CreateDilate(3f, 2f)),
    ("erode", ImageFilter.erode(2, 3), () => SKImageFilter.CreateErode(2f, 3f)),
    (
        "saturation-zero",
        ColorFilter.saturation(0),
        () =>
            NativeColor(
                new float[]
                {
                    .2126f,
                    .7152f,
                    .0722f,
                    0,
                    0,
                    .2126f,
                    .7152f,
                    .0722f,
                    0,
                    0,
                    .2126f,
                    .7152f,
                    .0722f,
                    0,
                    0,
                    0,
                    0,
                    0,
                    1,
                    0,
                }
            )
    ),
    (
        "saturation-one",
        ColorFilter.saturation(1),
        () =>
            NativeColor(new float[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 })
    ),
    (
        "color-mode",
        ColorFilter.mode(new Color(0xff9182d3), BlendMode.modulate),
        () =>
        {
            using var c = SKColorFilter.CreateBlendMode(
                new SKColor(0xff9182d3),
                SKBlendMode.Modulate
            );
            return SKImageFilter.CreateColorFilter(c);
        }
    ),
    (
        "linear-gamma",
        ColorFilter.linearToSrgbGamma(),
        () =>
        {
            using var c = SKColorFilter.CreateLinearToSrgbGamma();
            return SKImageFilter.CreateColorFilter(c);
        }
    ),
    (
        "srgb-gamma",
        ColorFilter.srgbToLinearGamma(),
        () =>
        {
            using var c = SKColorFilter.CreateSrgbToLinearGamma();
            return SKImageFilter.CreateColorFilter(c);
        }
    ),
    (
        "morph-compose",
        new ImageFilter(ImageFilter.erode(1, 2), ImageFilter.dilate(3, 1)),
        () =>
        {
            using var outer = SKImageFilter.CreateErode(1f, 2f);
            using var inner = SKImageFilter.CreateDilate(3f, 1f);
            return SKImageFilter.CreateCompose(outer, inner);
        }
    ),
};
if (gpu)
{
    var mirror = FragmentProgram
        .fromSource(
            """
            uniform float2 size;
            uniform shader inputImage;
            half4 main(float2 p) { return inputImage.eval(float2(size.x - p.x, p.y)); }
            """,
            "backdrop-mirror"
        )
        .fragmentShader();
    cases.Add(
        (
            "shader-mirror",
            new ImageFilter(mirror),
            () =>
                SKImageFilter.CreateMatrix(
                    new SKMatrix(-1, 0, 64, 0, 1, 0, 0, 0, 1),
                    SKSamplingOptions.Default,
                    null
                )
        )
    );
    cases.Add(("shader", shaderFilter, () => NativeColor(swap.Select(v => (float)v).ToArray())));
    cases.Add(
        (
            "shader-inner",
            new ImageFilter(ImageFilter.dilate(2, 1), shaderFilter),
            () =>
            {
                using var outer = SKImageFilter.CreateDilate(2f, 1f);
                using var inner = NativeColor(swap.Select(v => (float)v).ToArray());
                return SKImageFilter.CreateCompose(outer, inner);
            }
        )
    );
    cases.Add(
        (
            "shader-outer",
            new ImageFilter(shaderFilter, ImageFilter.erode(1, 2)),
            () =>
            {
                using var outer = NativeColor(swap.Select(v => (float)v).ToArray());
                using var inner = SKImageFilter.CreateErode(1f, 2f);
                return SKImageFilter.CreateCompose(outer, inner);
            }
        )
    );
    cases.Add(
        (
            "shader-color-compose",
            new ImageFilter(ColorFilter.matrix(swap), shaderFilter),
            () =>
                NativeColor(
                    new float[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 }
                )
        )
    );
    cases.Add(
        (
            "shader-blur",
            new ImageFilter(shaderFilter, new ImageFilter(2, 3)),
            () =>
            {
                using var outer = NativeColor(swap.Select(v => (float)v).ToArray());
                using var inner = SKImageFilter.CreateBlur(2, 3, SKShaderTileMode.Clamp);
                return SKImageFilter.CreateCompose(outer, inner);
            }
        )
    );
    cases.Add(
        (
            "shader-shader",
            new ImageFilter(shaderFilter, shaderFilter),
            () =>
                NativeColor(
                    new float[] { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0 }
                )
        )
    );
}
using var picture = MakePicture();
var passes = 0;
foreach (var (name, filter, native) in cases)
{
    foreach (
        var mode in new[]
        {
            "backdrop",
            "image",
            "nested",
            "retained",
            "scaled",
            "src",
            "rounded",
            "foreground",
            "color-layer",
            "nested-backdrop",
        }
    )
    {
        // This oracle mirrors the full current-layer GPU input in device pixels.
        if (name == "shader-mirror" && mode is "image" or "scaled")
            continue;
        if (mode == "nested-backdrop" && name != "shader-mirror")
            continue;
        var builder = new SceneBuilder(1);
        if (mode == "scaled")
            builder.pushTransform(
                new double[] { 1.25, 0, 0, 0, 0, 1.25, 0, 0, 0, 0, 1, 0, 2, 3, 0, 1 }
            );
        if (mode == "nested")
            builder.pushOpacity(151);
        if (mode == "color-layer")
            builder.pushColorFilter(ColorFilter.matrix(swap));
        if (mode != "image")
            builder.addPicture(Offset.zero, picture);
        if (mode == "rounded")
            builder.pushClipRRect(RRect.fromRectXY(new Rect(8, 9, 53, 54), 9, 9));
        else
            builder.pushClipRect(new Rect(8, 9, 53, 54), clipBehavior: Clip.hardEdge);
        if (mode == "nested-backdrop")
            builder.pushBackdropFilter(new ImageFilter());
        EngineLayer retained =
            mode == "image"
                ? builder.pushImageFilter(filter, bounds: new Rect(0, 0, 64, 64))
                : builder.pushBackdropFilter(
                    filter,
                    mode == "src" ? BlendMode.src : BlendMode.srcOver
                );
        if (mode is "image" or "foreground")
            builder.addPicture(Offset.zero, picture);
        builder.pop();
        if (mode == "nested-backdrop")
            builder.pop();
        builder.pop();
        if (mode is "nested" or "scaled" or "color-layer")
            builder.pop();
        using var scene = builder.build();
        using var nf = native();
        using var expected = Surface();
        var canvas = expected.Canvas;
        canvas.Clear(new SKColor(0x70472d1b));
        canvas.Save();
        if (mode == "scaled")
        {
            canvas.Translate(2, 3);
            canvas.Scale(1.25f);
        }
        if (mode == "nested")
        {
            using var opacity = new SKPaint { Color = SKColors.White.WithAlpha(151) };
            canvas.SaveLayer(opacity);
        }
        if (mode == "color-layer")
        {
            using var c = SKColorFilter.CreateColorMatrix(swap.Select(v => (float)v).ToArray());
            using var p = new SKPaint { ColorFilter = c };
            canvas.SaveLayer(p);
        }
        if (mode != "image")
            DrawNative(canvas);
        canvas.Save();
        if (mode == "rounded")
        {
            using var pb = new SKPathBuilder();
            pb.AddRoundRect(new SKRect(8, 9, 53, 54), 9, 9);
            using var p = pb.Detach();
            canvas.ClipPath(p, SKClipOperation.Intersect, true);
        }
        else
            canvas.ClipRect(new SKRect(8, 9, 53, 54));
        if (mode == "nested-backdrop")
        {
            using var identity = SKImageFilter.CreateBlur(0, 0, SKShaderTileMode.Clamp);
            canvas.SaveLayer(new SKCanvasSaveLayerRec { Backdrop = identity });
        }
        if (mode == "image")
        {
            using var p = new SKPaint { ImageFilter = nf };
            canvas.SaveLayer(p);
            DrawNative(canvas);
        }
        else
        {
            using var p = new SKPaint
            {
                BlendMode = mode == "src" ? SKBlendMode.Src : SKBlendMode.SrcOver,
            };
            canvas.SaveLayer(new SKCanvasSaveLayerRec { Backdrop = nf, Paint = p });
            if (mode == "foreground")
                DrawNative(canvas);
        }
        canvas.Restore();
        if (mode == "nested-backdrop")
            canvas.Restore();
        canvas.Restore();
        if (mode is "nested" or "color-layer")
            canvas.Restore();
        canvas.Restore();
        using var actual = Surface();
        actual.Canvas.Clear(new SKColor(0x70472d1b));
        renderer.DrawPlatformRasterSegment(actual.Canvas, scene.Commands, 64, 64);
        Verify($"{name}/{mode}", expected, actual);
        if (mode == "retained")
        {
            var rb = new SceneBuilder(1);
            rb.addPicture(Offset.zero, picture);
            rb.pushClipRect(new Rect(8, 9, 53, 54), clipBehavior: Clip.hardEdge);
            rb.addRetained(retained);
            rb.pop();
            using var rs = rb.build();
            actual.Canvas.Clear(new SKColor(0x70472d1b));
            renderer.DrawPlatformRasterSegment(actual.Canvas, rs.Commands, 64, 64);
            Verify($"{name}/retained-replay", expected, actual);
        }
    }
}
foreach (var invalid in new[] { -1d, double.NaN, double.PositiveInfinity })
{
    try
    {
        ImageFilter.dilate(invalid);
        throw new Exception("Invalid radius accepted");
    }
    catch (ArgumentOutOfRangeException) { }
}
Console.WriteLine(
    $"PASS: {passes} {(graphite ? "Graphite Vulkan GPU" : gpu ? "Ganesh Vulkan GPU" : "CPU")} native pixel comparisons; radius contracts"
);

SKSurface Surface() =>
    fixture is null
        ? SKSurface.Create(new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul))
        : fixture.CreateSurface(new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul));
void Verify(string name, SKSurface expected, SKSurface actual)
{
    if (graphite)
    {
        var (e, a) = fixture!.ReadGraphite(expected, actual);
        var difference = e.Zip(a).Count(p => p.First != p.Second);
        if (difference != 0)
            throw new Exception($"{name}: {difference} differing Graphite channels");
        if (actual.Canvas.SaveCount != 1)
            throw new Exception("Unbalanced saves");
        passes++;
        Console.WriteLine($"PASS {name}");
        return;
    }
    fixture?.Context?.Flush(submit: true, synchronous: true);
    using var ei = expected.Snapshot();
    using var ai = actual.Snapshot();
    using var eb = SKBitmap.FromImage(ei);
    using var ab = SKBitmap.FromImage(ai);
    var different = eb.Pixels.Zip(ab.Pixels).Count(p => p.First != p.Second);
    if (different != 0)
    {
        Directory.CreateDirectory("Doroti/artifacts/backdrop-filters");
        using var e = ei.Encode(SKEncodedImageFormat.Png, 100);
        using var a = ai.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes("Doroti/artifacts/backdrop-filters/expected.png", e.ToArray());
        File.WriteAllBytes("Doroti/artifacts/backdrop-filters/actual.png", a.ToArray());
        throw new Exception($"{name}: {different} differing pixels");
    }
    if (actual.Canvas.SaveCount != 1)
        throw new Exception("Unbalanced saves");
    passes++;
    Console.WriteLine($"PASS {name}");
}
static SKImageFilter NativeColor(float[] matrix)
{
    using var cf = SKColorFilter.CreateColorMatrix(matrix);
    return SKImageFilter.CreateColorFilter(cf);
}
static Picture MakePicture()
{
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder);
    canvas.drawRect(
        new Rect(4, 5, 35, 43),
        new Paint { color = new Color(0xff926437), isAntiAlias = false }
    );
    canvas.drawRect(
        new Rect(21, 18, 59, 58),
        new Paint { color = new Color(0x8040a070), isAntiAlias = false }
    );
    return recorder.endRecording();
}
static void DrawNative(SKCanvas canvas)
{
    using var paint = new SKPaint { Color = new SKColor(0xff926437) };
    canvas.DrawRect(new SKRect(4, 5, 35, 43), paint);
    paint.Color = new SKColor(0x8040a070);
    canvas.DrawRect(new SKRect(21, 18, 59, 58), paint);
}

sealed class Host : ISkiaSceneRendererHost
{
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 64, 64, 64, 64, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 64, 64, 64, 64, 1, 1);
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

    public void RequestInvalidate() { }
}
