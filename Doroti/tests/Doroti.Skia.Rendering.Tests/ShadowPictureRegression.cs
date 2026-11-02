using System.Collections;
using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using UiPath = Doroti.Ui.Path;

internal static class ShadowPictureRegression
{
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    internal static void Run()
    {
        CheckRasterPromotionPolicy();
        using var renderer = new SkiaSceneRenderer(1, new Host(), null, null, "test", "test", "test");
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", Members)!
            .CreateDelegate<Action<SKCanvas, IReadOnlyList<PathCommand>>>(renderer);
        var retained = typeof(SkiaSceneRenderer).GetMethod("DrawRetainedPicture", Members)!;
        var scaleField = typeof(SkiaSceneRenderer).GetField("_shadowDeviceScale", Members)!;
        var cache = (IDictionary)typeof(SkiaSceneRenderer).GetField("_pictureCommandCache", Members)!.GetValue(renderer)!;
        var countField = typeof(SkiaSceneRenderer).GetField("_pictureCommandRecordings", Members)!;
        var cases = 0;
        foreach (var transparent in new[] { false, true })
        {
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            var bounds = Rect.fromLTWH(20, 30, 100, 55);
            var rounded = RRect.fromRectAndRadius(bounds, Radius.circular(12));
            var path = new UiPath(); path.addRRect(rounded);
            canvas.save(); canvas.translate(3.25, 5.125);
            canvas.drawShadow(path, new Color(0xD0243566), 9, transparent);
            canvas.drawRRect(rounded, new Paint { color = new Color(0xA0315879) });
            using var paragraph = new Paragraph("cache 한글 😀", 100, 16);
            paragraph.layout(new ParagraphConstraints(100));
            canvas.drawParagraph(paragraph, new Offset(25, 48));
            canvas.restore();
            using var picture = recorder.endRecording();
            var id = transparent ? 102L : 101L;
            var payload = Activator.CreateInstance(typeof(Scene).Assembly.GetType("Doroti.Ui.ScenePicturePayload")!,
                Members, null, [id, Offset.zero, picture.Commands, bounds.shift(new Offset(3.25, 5.125)), false, false], null)!;
            SKPicture? previousPicture = null;
            foreach (var scale in new[] { 1f, 2.75f, 3f })
            {
                scaleField.SetValue(renderer, (double)scale);
                var linear = SKMatrix.CreateScale(scale, scale);
                Warm(linear);
                if (previousPicture is not null) Check(previousPicture.Handle == IntPtr.Zero, "changed scale retires the previous native picture");
                previousPicture = CachedPicture(id);
                foreach (var (x, y) in new[] { (0f, 0f), (4.25f, 12.375f), (3.2f, -5.1f) })
                {
                    var matrix = linear; matrix.TransX = x; matrix.TransY = y;
                    Compare(matrix); cases++;
                    Check(ReferenceEquals(previousPicture, CachedPicture(id)), "fractional translation reuses the same vector recording");
                }
            }
            var skew = new SKMatrix(2.75f, .25f, 4.125f, .125f, 2.5f, 12.375f, 0, 0, 1);
            Warm(skew); Compare(skew); cases++;
            var perspective = skew; perspective.Persp0 = .0001f;
            var recordings = (long)countField.GetValue(renderer)!;
            Compare(perspective); cases++;
            Check((long)countField.GetValue(renderer)! == recordings, "perspective retains direct drawing");
            Warm(skew); previousPicture = CachedPicture(id);
            scaleField.SetValue(renderer, 1.25);
            Warm(skew); Compare(skew); cases++;
            Check(previousPicture.Handle == IntPtr.Zero, "shadow device scale invalidates independently of canvas scale");

            void Warm(SKMatrix matrix)
            {
                using var one = Render(matrix, true);
                using var two = Render(matrix, true);
                Check(CachedPicture(id).Handle != IntPtr.Zero, "shadow picture reaches the real native command cache");
            }
            void Compare(SKMatrix matrix)
            {
                using var expected = Render(matrix, false);
                using var actual = Render(matrix, true);
                Check(expected.GetPixelSpan().SequenceEqual(actual.GetPixelSpan()), $"exact shadow/text pixels under {matrix}");
            }
            SKBitmap Render(SKMatrix matrix, bool cached)
            {
                var bitmap = new SKBitmap(500, 400, SKColorType.Rgba8888, SKAlphaType.Premul);
                using var target = new SKCanvas(bitmap);
                target.Clear(SKColors.White);
                target.ClipRect(new SKRect(0, 0, 350, 310));
                target.SetMatrix(matrix);
                var saveCount = target.SaveCount;
                if (cached) retained.Invoke(renderer, [target, payload]);
                else draw(target, picture.Commands);
                Check(target.SaveCount == saveCount && target.TotalMatrix == matrix, "replay preserves destination matrix and save stack");
                return bitmap;
            }
        }
        Check((long)countField.GetValue(renderer)! > 0, "shadow pictures are recorded rather than replayed in managed code");
        var live = cache.Values.Cast<object>().Select(entry => (SKPicture?)entry.GetType().GetField("Picture", Members)!.GetValue(entry))
            .OfType<SKPicture>().ToArray();
        renderer.RegisterFontAsync(File.ReadAllBytes("samples/DorotiTestbedApp/assets/fonts/Roboto-regular.ttf"), "late-font").GetAwaiter().GetResult();
        Check(cache.Count == 0 && live.All(item => item.Handle == IntPtr.Zero), "font registration retires retained glyph recordings");
        Console.WriteLine($"PASS: {cases} exact shadow/text picture comparisons, fractional translation reuse, affine/DPR invalidation, perspective fallback and font retirement");

        SKPicture CachedPicture(long id) => (SKPicture)cache[id]!.GetType().GetField("Picture", Members)!.GetValue(cache[id])!;
    }
    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
    private static void CheckRasterPromotionPolicy()
    {
        using var native = new SkiaSceneRenderer(1, new Host(), null, null, "test", "test", "test");
        using var browser = new SkiaSceneRenderer(2, new Host(), null, null, "test", "test", "test")
            { DeferPictureRasterPromotionsDuringScroll = true };
        var trace = new DorotiFrameTrace();
        var clock = TimeSpan.FromSeconds(1);
        typeof(DorotiFrameTrace).GetProperty("ActivityClock", Members)!.SetValue(trace, (Func<TimeSpan>)(() => clock));
        native.AttachFrameworkTrace(trace); browser.AttachFrameworkTrace(trace);
        var begin = typeof(SkiaSceneRenderer).GetMethod("BeginPictureRasterFrame", Members)!;
        var deferred = typeof(SkiaSceneRenderer).GetField("_deferPictureRasterPromotions", Members)!;
        trace.RecordScroll(DorotiFramePhase.scrollStart, 1, 7, 0, null, "DragScrollActivity");
        begin.Invoke(native, null); begin.Invoke(browser, null);
        Check(!(bool)deferred.GetValue(native)! && (bool)deferred.GetValue(browser)!, "only opting-in hosts defer new raster promotions");
        trace.RecordScroll(DorotiFramePhase.scrollUpdate, 1, 7, 15.25, 15.25, "BallisticScrollActivity");
        trace.RecordScroll(DorotiFramePhase.scrollEnd, 1, 7, 15.25, null, "IdleScrollActivity");
        begin.Invoke(browser, null);
        Check((bool)deferred.GetValue(browser)!, "the quiet interval covers the final wheel/ballistic updates");
        clock += TimeSpan.FromSeconds(1);
        begin.Invoke(browser, null);
        Check(!(bool)deferred.GetValue(browser)!, "cache promotion resumes when scrolling settles");
        Console.WriteLine("PASS: Web raster promotion opt-in, unchanged default host policy, scroll/quiet interval and settled resumption");
    }
    private sealed class Host : ISkiaSceneRendererHost
    {
        public long InputSequence => 0;
        public long SurfaceGeneration => 1;
        public DorotiViewEpoch ViewEpoch => null!;
        public DorotiResizeEpoch ResizeTarget => null!;
        public PlatformConfiguration Configuration { get; } = new([], Brightness.light, false, false);
        public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
        public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
        public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
        public void UpdateSemantics(SemanticsUpdate update) { }
        public void ClearSemantics() { }
        public void RequestInvalidate() { }
    }
}
