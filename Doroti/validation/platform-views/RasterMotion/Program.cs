using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

using var picture = Picture(0xff336699);
using var changed = Picture(0xff663399);
var start = Slice(picture, 20, 30);
Check("integer translation retains immutable foreground", Same(start, Slice(picture, 70, 90)));
Check("fractional translation redraws", !Same(start, Slice(picture, 70.25, 90)));
Check("new picture redraws", !Same(start, Slice(changed, 70, 90)));
Check("live picture redraws", !Same(start, Slice(picture, 70, 90, changing: true)));
Check("changed viewport clipping redraws", !Same(start, Slice(picture, 70, 90, viewportRight: 100)));
Check("antialiased scene clip redraws", !Same(start, Slice(picture, 70, 90, clip: Clip.antiAlias)));
Check("group opacity redraws", !Same(start, Slice(picture, 70, 90, opacity: true)));
Check("resized bitmap redraws", !Same(start, Slice(picture, 70, 90) with { Bounds = new SKRectI(70, 90, 151, 150) }));
Check("stationary background cannot follow moving crop", !Same(
    start, start with { Bounds = new SKRectI(70, 90, 150, 150) }));
var layered = new SceneBuilder();
layered.pushClipRect(Rect.fromLTWH(20, 30, 80, 60), Clip.hardEdge);
layered.addPicture(new Offset(20, 30), picture);
layered.addPicture(new Offset(20, 30), changed);
layered.pop();
using var layeredScene = layered.build();
Check("coincident small foreground pictures share one bitmap",
    SkiaPlatformRasterContent.Split(layeredScene.Commands, 1000, 1000).Count == 1);
var emptyRecorder = new PictureRecorder();
var emptyCanvas = new Canvas(emptyRecorder);
emptyCanvas.save(); emptyCanvas.translate(20, 30); emptyCanvas.clipRect(Rect.fromLTWH(0, 0, 80, 60)); emptyCanvas.restore();
using var emptyPicture = emptyRecorder.endRecording();
var emptyBuilder = new SceneBuilder(); emptyBuilder.addPicture(Offset.zero, emptyPicture);
using var emptyScene = emptyBuilder.build();
Check("state-only picture has no pixels", !SkiaPlatformRasterContent.HasDrawing(emptyScene.Commands));
Check("state-only picture allocates no bitmap", SkiaPlatformRasterContent.Split(emptyScene.Commands, 1000, 1000).Count == 0);
var effectRecorder = new PictureRecorder();
var effectCanvas = new Canvas(effectRecorder);
effectCanvas.saveLayer(null, new Paint { blendMode = BlendMode.src }); effectCanvas.restore();
using var effectPicture = effectRecorder.endRecording();
var effectBuilder = new SceneBuilder(); effectBuilder.addPicture(Offset.zero, effectPicture);
using var effectScene = effectBuilder.build();
Check("empty layer with destructive blend is retained", SkiaPlatformRasterContent.HasDrawing(effectScene.Commands));
var sample = new SKRectI(0, 0, 100, 80);
var pieces = SkiaPlatformRasterContent.SubtractOpaque(sample, new(10, 20, 90, 70));
Check("opaque native interior preserves all kernel edge strips", pieces.Count == 4 &&
    pieces.Sum(r => r.Width * r.Height) == 100 * 80 - 80 * 50 &&
    pieces.All(r => r.Left >= 0 && r.Top >= 0 && r.Right <= 100 && r.Bottom <= 80));
Check("opaque native coverage eliminates hidden background capture",
    SkiaPlatformRasterContent.SubtractOpaque(sample, new(-1, -1, 101, 81)).Count == 0);
Check("disjoint native content preserves full background capture",
    SkiaPlatformRasterContent.SubtractOpaque(sample, new(110, 0, 200, 80)).SequenceEqual([sample]));
Check("kernel pieces do not overlap", pieces.SelectMany((a, i) => pieces.Skip(i + 1).Select(b =>
    Math.Min(a.Right, b.Right) <= Math.Max(a.Left, b.Left) || Math.Min(a.Bottom, b.Bottom) <= Math.Max(a.Top, b.Top))).All(v => v));
var scope = SkiaPlatformRasterContent.CacheScope.From(new(1, 2, 3, 4, 2, 2), 1000, 800, SKColors.Transparent, 9);
Check("same resource domain permits translated reuse", SkiaPlatformRasterContent.CanReuse(scope, start, scope, Slice(picture, 70, 90)));
Check("HWND path can explicitly forbid translated reuse", !SkiaPlatformRasterContent.CanReuse(scope, start, scope, Slice(picture, 70, 90), allowTranslation: false));
Check("new frame number does not invalidate pixels", scope == SkiaPlatformRasterContent.CacheScope.From(new(1, 2, 99, 4, 2, 2), 1000, 800, SKColors.Transparent, 9));
foreach (var changedScope in new[] {
    scope with { ViewId = 2 }, scope with { ViewEpoch = 3 }, scope with { SurfaceGeneration = 5 },
    scope with { Width = 999 }, scope with { Height = 799 }, scope with { ScaleX = 3 },
    scope with { ScaleY = 3 }, scope with { Background = SKColors.Red }, scope with { ResourceGeneration = 10 } })
    Check("changed owner/epoch/extent/scale/background/resource invalidates reuse",
        !SkiaPlatformRasterContent.CanReuse(scope, start, changedScope, start));
Check("coverage retains enforced clip and pixel halo",
    SkiaPlatformRasterContent.Coverage(layeredScene.Commands, 1000, 1000) == new SKRectI(18, 28, 102, 92));
Check("empty content has no coverage", SkiaPlatformRasterContent.Coverage(emptyScene.Commands, 1000, 1000).IsEmpty);
// A moving panel and stationary navigation can share a planner raster segment.
// Its union is not reusable, but each enforced-clip slice still is.
var navigation = Slice(changed, 0, 400);
var combinedStart = start.Commands.Concat(navigation.Commands).ToArray();
var combinedEnd = Slice(picture, 70, 90).Commands.Concat(navigation.Commands).ToArray();
var splitStart = SkiaPlatformRasterContent.Split(combinedStart, 1000, 800);
var splitEnd = SkiaPlatformRasterContent.Split(combinedEnd, 1000, 800);
Check("panel and stationary navigation remain separate slices", splitStart.Count == 2 && splitEnd.Count == 2);
Check("moving panel and stationary navigation independently reuse pixels",
    splitStart.Zip(splitEnd).All(pair => SkiaPlatformRasterContent.CanReuse(scope, pair.First, scope, pair.Second)));
Check("combined moving and stationary content cannot reuse one bitmap",
    !SkiaPlatformRasterContent.CanReuse(scope, new(combinedStart, new(0, 0, 1000, 800)),
        scope, new(combinedEnd, new(0, 0, 1000, 800))));
Console.WriteLine("PASS 34 platform raster motion and cross-platform cache checks");

static bool Same(SkiaPlatformRasterContent.Slice a, SkiaPlatformRasterContent.Slice b) =>
    SkiaPlatformRasterContent.EquivalentTranslation(a, b);
static void Check(string name, bool pass)
{
    if (!pass) throw new InvalidOperationException(name);
    Console.WriteLine("PASS " + name);
}
static Picture Picture(uint color)
{
    var recorder = new PictureRecorder();
    new Canvas(recorder).drawRect(Rect.fromLTWH(0, 0, 80, 60), new Paint { color = new Color(color) });
    return recorder.endRecording();
}
static SkiaPlatformRasterContent.Slice Slice(Picture picture, double x, double y,
    bool changing = false, double viewportRight = 500, Clip clip = Clip.hardEdge, bool opacity = false)
{
    var builder = new SceneBuilder();
    builder.pushClipRect(Rect.fromLTWH(0, 0, viewportRight, 500), Clip.hardEdge);
    builder.pushOffset(x, y);
    builder.pushClipRect(Rect.fromLTWH(0, 0, 80, 60), clip);
    if (opacity) builder.pushOpacity(128);
    builder.addPicture(Offset.zero, picture, willChangeHint: changing);
    if (opacity) builder.pop();
    builder.pop(); builder.pop(); builder.pop();
    using var scene = builder.build();
    return new(scene.Commands.ToArray(), new((int)x, (int)y, (int)x + 80, (int)y + 60));
}
