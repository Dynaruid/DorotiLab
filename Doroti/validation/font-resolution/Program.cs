using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using Path = System.IO.Path;

var fontRoot = await FontTestAssets.RestoreAsync();
using var fonts = new SkiaFallbackFontCollection("Roboto");
foreach (var asset in new[] { "Roboto-regular.ttf", "Roboto-medium.ttf", "Roboto-bold.ttf",
    "NotoSansKR-Regular.otf", "NotoSansKR-Bold.otf" })
    fonts.Register(File.ReadAllBytes(Path.Combine(fontRoot, asset)));
fonts.Register(File.ReadAllBytes(Path.Combine(fontRoot, "NanumGothic-Regular.ttf")), "ExplicitKorean");
using var renderer = new SkiaSceneRenderer(1, new Host(), null, null, "font-test", "font-test", "font-test",
    fallbackFonts: fonts);

Paragraph Layout(string text, string? family = null, FontWeight? weight = null,
    string[]? fallback = null, FontStyle? slant = null) => renderer.Layout(
        new ParagraphRequest(text, 900, family, 24, TextRuns:
            [new ParagraphTextRun(text, new TextStyle(fontFamily: family, fontSize: 24,
                fontWeight: weight, fontFamilyFallback: fallback, fontStyle: slant))]), new("font-test"));

var latin = "iiii WWWW Roboto 123";
var defaultText = Layout(latin);
var explicitText = Layout(latin, "Roboto");
Check(Raster(defaultText).SequenceEqual(Raster(explicitText)), "host default uses Roboto pixels and metrics");
Check(Raster(Layout(latin, "DefinitelyMissingFont")).SequenceEqual(Raster(explicitText)),
    "unknown family reaches host default");
Check(Layout("iiii").maxIntrinsicWidth < Layout("WWWW").maxIntrinsicWidth / 2,
    "default is proportional, not the wasm emergency monospace face");
foreach (var (weight, asset) in new[] { (FontWeight.w400, "Roboto-regular.ttf"),
    (FontWeight.w500, "Roboto-medium.ttf"), (FontWeight.w700, "Roboto-bold.ttf") })
{
    using var face = SKTypeface.FromFile(Path.Combine(fontRoot, asset));
    using var font = new SKFont(face, 24);
    var paragraph = Layout(latin, weight: weight);
    Check(Math.Abs(paragraph.maxIntrinsicWidth - font.GetGlyphWidths(latin).Sum()) < .01,
        $"requested weight {face.FontWeight} measures the correct face");
}
var korean = "한글 가나다 테스트";
var regular = Raster(Layout(korean, "Roboto"));
var bold = Raster(Layout(korean, "Roboto", FontWeight.w700));
Check(!regular.SequenceEqual(bold), "Korean fallback retains requested bold weight");
foreach (var (weight, asset) in new[] { (FontWeight.w400, "NotoSansKR-Regular.otf"),
    (FontWeight.w700, "NotoSansKR-Bold.otf") })
{
    // Exclude spaces: those correctly use the primary Roboto font.
    using var face = SKTypeface.FromFile(Path.Combine(fontRoot, asset));
    using var font = new SKFont(face, 24);
    var text = "한글가나다";
    Check(font.ContainsGlyphs(text), "Noto covers Korean without tofu");
    Check(Math.Abs(Layout(text, "Roboto", weight).maxIntrinsicWidth - font.GetGlyphWidths(text).Sum()) < .01,
        "Korean fallback measures the requested Noto face");
}
var explicitFallback = Layout("한글가나다", "Roboto", fallback: ["ExplicitKorean"]);
Check(Math.Abs(explicitFallback.maxIntrinsicWidth - Layout("한글가나다", "ExplicitKorean").maxIntrinsicWidth) < .01,
    "explicit per-glyph fallback precedes global Noto");
Check(Raster(Layout(korean, "ExplicitKorean")).SequenceEqual(Raster(Layout(korean, "NanumGothic"))),
    "registration alias preserves access by intrinsic family name");
Check(!Raster(Layout(latin, "Roboto", slant: FontStyle.italic)).SequenceEqual(Raster(explicitText)),
    "missing italic face gets synthetic italic");
using (var native = new SkiaSceneRenderer(2, new Host(), null, null, "native", "native", "native"))
{
    Doroti.Skia.Fonts.NativeDefaultFonts.Register(native);
    Check(native.CaptureCacheMemory().FontGeneration == 3, "native bundle registers all three embedded faces offline");
    Paragraph Native(string? family) => native.Layout(new ParagraphRequest(latin, 900, family, 24), new("native"));
    Check(Math.Abs(Native("Roboto").maxIntrinsicWidth - Layout(latin, "Roboto").maxIntrinsicWidth) < .01,
        "native bundled Roboto matches the pinned font measurements");
    if (OperatingSystem.IsWindows())
        Check(Math.Abs(Native("CupertinoSystemText").maxIntrinsicWidth - Native("Segoe UI").maxIntrinsicWidth) < .01
            && Math.Abs(Native(null).maxIntrinsicWidth - Native("Segoe UI").maxIntrinsicWidth) < .01,
            "Windows default and Cupertino alias resolve to Segoe UI");
}
await renderer.RegisterFontAsync(File.ReadAllBytes(Path.Combine(fontRoot, "Roboto-bold.ttf")), "LateFont");
Check(renderer.CaptureCacheMemory().TextEntries == 0, "font registration invalidates stale resolution");
var output = Path.GetFullPath("Doroti/artifacts/font-resolution");
Directory.CreateDirectory(output);
File.WriteAllBytes(Path.Combine(output, "regular.png"), regular);
File.WriteAllBytes(Path.Combine(output, "bold.png"), bold);
Console.WriteLine("PASS font defaults, 3 weights, Korean fallback, aliases, synthetic italic, native defaults and cache invalidation");

byte[] Raster(Paragraph paragraph)
{
    using var surface = SKSurface.Create(new SKImageInfo(1000, 100));
    surface.Canvas.Clear(SKColors.White);
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder);
    canvas.drawParagraph(paragraph, new Offset(10, 10));
    using var picture = recorder.endRecording();
    var builder = new SceneBuilder(1);
    builder.addPicture(Offset.zero, picture);
    using var scene = builder.build();
    renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, 1000, 100);
    using var image = surface.Snapshot();
    using var data = image.Encode();
    return data.ToArray();
}
static void Check(bool ok, string message)
{
    if (!ok) throw new Exception(message);
    Console.WriteLine($"PASS {message}");
}

sealed class Host : ISkiaSceneRendererHost
{
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 1000, 100, 1000, 100, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 1000, 100, 1000, 100, 1, 1);
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
    public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public void UpdateSemantics(SemanticsUpdate update) { }
    public void ClearSemantics() { }
    public void RequestInvalidate() { }
}
