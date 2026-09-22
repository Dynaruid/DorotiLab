using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

var host = new Host();
using var renderer = new SkiaSceneRenderer(
    1,
    host,
    null,
    null,
    "memory-contract",
    "memory-contract",
    "memory-contract"
);
var request = new ParagraphRequest(
    "Latin 한글 😀 fallback",
    500,
    "Arial",
    18,
    Color: new Color(0xff123456)
);
var original = renderer.Layout(request, new("memory"));
var pixels = Raster(original);
var checkpoints = new List<SkiaCacheMemoryDiagnostics>();
for (var pass = 0; pass < 4; pass++)
{
    for (var i = 0; i < 1024; i++)
    {
        var style = new TextStyle(
            fontSize: 12 + i % 19,
            color: new Color(0xff000000L + i),
            fontWeight: i % 2 == 0 ? FontWeight.w400 : FontWeight.w700,
            letterSpacing: i % 3 * .2,
            wordSpacing: i % 2 * .1,
            fontFamilyFallback: new[] { "Arial", "Apple SD Gothic Neo" }
        );
        var paragraph = renderer.Layout(
            request with
            {
                TextRuns = new[] { new ParagraphTextRun(request.Text, style) },
            },
            new("memory")
        );
        if (i % 64 == 0)
            Raster(paragraph);
    }
    var memory = renderer.CaptureCacheMemory();
    Check(memory.TextEntries == memory.TextEntryLimit, "cache converges to entry limit");
    Check(memory.TextEvictions > 0, "evictions occurred");
    checkpoints.Add(memory);
}
var recreated = renderer.Layout(request, new("memory"));
Check(
    original.width == recreated.width && original.height == recreated.height,
    "measurements survive eviction"
);
Check(pixels.SequenceEqual(Raster(recreated)), "fractional DPR raster survives eviction");

// Original paragraphs and recorded pictures remain usable after resource eviction.
Check(pixels.SequenceEqual(Raster(original)), "retained paragraph survives eviction");
var hits = renderer.CaptureCacheMemory().TextHits;
renderer.Layout(request, new("memory"));
Check(renderer.CaptureCacheMemory().TextHits > hits, "same style reuses entry");
using var pictureRecorder = new SKPictureRecorder();
var recordingCanvas = pictureRecorder.BeginRecording(new SKRect(0, 0, 800, 100));
Draw(recordingCanvas, original);
using var recorded = pictureRecorder.EndRecording();
var fontBytes = File.ReadAllBytes(
    System.IO.Path.GetFullPath("DorotiTestbedApp/assets/fonts/Roboto-regular.ttf")
);
await renderer.RegisterFontAsync(fontBytes, "MemoryFont");
Check(renderer.CaptureCacheMemory().TextEntries == 0, "registration clears old resolution");
Check(renderer.CaptureCacheMemory().FontGeneration == 1, "registration advances generation");
using var replaySurface = SKSurface.Create(new SKImageInfo(800, 100));
replaySurface.Canvas.Clear(SKColors.Transparent);
replaySurface.Canvas.DrawPicture(recorded);
replaySurface.Canvas.Flush();
using var replayImage = replaySurface.Snapshot();
using var replayData = replayImage.Encode();
Check(
    replayData.ToArray().SequenceEqual(pixels),
    "SKPicture keeps glyphs after font resources dispose"
);
renderer.Dispose();
renderer.Dispose();
Check(renderer.CaptureCacheMemory().TextEntries == 0, "idempotent disposal releases cache");
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(checkpoints));
Console.WriteLine("PASS native cache lifetime, measurement and pixel contracts");

byte[] Raster(Paragraph paragraph)
{
    using var surface = SKSurface.Create(new SKImageInfo(800, 100));
    surface.Canvas.Clear(SKColors.Transparent);
    Draw(surface.Canvas, paragraph);
    surface.Canvas.Flush();
    using var image = surface.Snapshot();
    using var data = image.Encode();
    return data.ToArray();
}
void Draw(SKCanvas target, Paragraph paragraph)
{
    var recorder = new PictureRecorder();
    var canvas = new Canvas(recorder);
    canvas.drawParagraph(paragraph, new Offset(.25, .5));
    using var picture = recorder.endRecording();
    var builder = new SceneBuilder(1);
    builder.addPicture(Offset.zero, picture);
    using var scene = builder.build();
    target.Save();
    target.Scale(1.25f);
    renderer.DrawPlatformRasterSegment(target, scene.Commands, 800, 100);
    target.Restore();
}
static void Check(bool ok, string message)
{
    if (!ok)
        throw new Exception(message);
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
