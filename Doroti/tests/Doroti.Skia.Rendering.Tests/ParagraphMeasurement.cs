using System.Diagnostics;
using System.Reflection;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

internal static class ParagraphMeasurement
{
    internal static void Run(string fontDirectory)
    {
#if DOROTI_SHAPED_PARAGRAPHS
        Console.WriteLine("HarfBuzz paragraph profile");
#else
        Console.WriteLine("Advance-based paragraph profile");
#endif
        using var fonts = new SkiaFallbackFontCollection("Arabic");
        foreach (var family in new[] { "Arabic", "Devanagari", "Thai", "Emoji" })
            fonts.Register(File.ReadAllBytes(System.IO.Path.Combine(fontDirectory, family + ".ttf")), family);
        using var renderer = new SkiaSceneRenderer(1, new ShadowPictureRegression.Host(), null, null,
            "probe", "probe", "probe", fallbackFonts: fonts);
        var invoke = DorotiUiInvocation.Managed("CPU paragraph measurement");
        var draw = typeof(SkiaSceneRenderer).GetMethod("DrawPicture", BindingFlags.Instance | BindingFlags.NonPublic)!
            .CreateDelegate<Action<SKCanvas, IReadOnlyList<PathCommand>>>(renderer);
        using var bitmap = new SKBitmap(640, 300);
        using var raster = new SKCanvas(bitmap);

        var requests = new (string Name, Func<int, string> Text, double Width, string Locale)[]
        {
            ("short-en", number => $"Add {number} items to cart", 600, "en"),
            ("wrap-en", number => string.Join(' ', Enumerable.Repeat($"Price 24.00 Add to cart Item {number}", 9)),
                480, "en"),
            ("wrap-mixed", number => string.Join(' ', Enumerable.Repeat($"abc {number} سلام X कि 👩‍💻 ภาษาไทยภาษาไทย", 7)),
                480, "ar"),
        };
        foreach (var (name, text, width, locale) in requests)
        {
            var request = new ParagraphRequest(text(3), width, "Arabic", 20, Locale: new Locale(locale));
            var edited = Enumerable.Range(1, 30).Select(index => request with { Text = text(index) }).ToArray();
            using var layout = renderer.Layout(request, invoke);
            var recorder = new PictureRecorder();
            var canvas = new Canvas(recorder);
            canvas.drawParagraph(layout, Offset.zero);
            using var picture = recorder.endRecording();
            Measure(name, "layout-repeat", _ => { using var result = renderer.Layout(request, invoke); });
            Measure(name, "layout-text-change", index => { using var result = renderer.Layout(edited[index], invoke); });
            Measure(name, "layout-resize", Resize);
            Measure(name, "resize-and-hit-test", index =>
            {
                Resize(index);
                layout.getPositionForOffset(new Offset(width / 2, layout.height / 2));
            });
            Measure(name, "resize-and-paint", index =>
            {
                Resize(index);
                var frame = new PictureRecorder();
                new Canvas(frame).drawParagraph(layout, Offset.zero);
                using var resizedPicture = frame.endRecording();
                draw(raster, resizedPicture.Commands);
            });
            Measure(name, "paint", _ => draw(raster, picture.Commands));

            void Resize(int index) => layout.layout(new ParagraphConstraints(width - 80 + index * 4));
        }
    }

    private static void Measure(string name, string phase, Action<int> operation)
    {
        var durations = new double[25];
        const int warmup = 5;
        for (var i = 0; i < warmup; i++) operation(i);
        var baseline = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < durations.Length; i++)
        {
            var start = Stopwatch.GetTimestamp();
            operation(i + warmup);
            durations[i] = Stopwatch.GetElapsedTime(start).TotalMicroseconds;
        }
        var bytes = (GC.GetAllocatedBytesForCurrentThread() - baseline) / durations.Length;
        Array.Sort(durations);
        Console.WriteLine(FormattableString.Invariant(
            $"{name} {phase}: p50={durations[12]:F1} us p95={durations[23]:F1} us managed={bytes} bytes"));
    }
}
