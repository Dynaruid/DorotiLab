using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Semantics;
using Doroti.Framework.Services;
using Doroti.Runtime;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using TextStyle = Doroti.Ui.TextStyle;

internal static class ProductParagraphProbe
{
    internal static async Task<(string Receipt, byte[] Png)> RunAsync(Func<string, byte[]> fontBytes)
    {
        using var fonts = new SkiaFallbackFontCollection("Arabic");
        foreach (var family in new[] { "Arabic", "Devanagari", "Thai", "Emoji", "Lao", "Khmer", "Burmese" })
            fonts.Register(fontBytes(family), family);
        var host = new Host();
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "probe", "probe", "probe", fallbackFonts: fonts);
        CheckSemantics(renderer, host);
        var invoke = DorotiUiInvocation.Managed("Skia paragraph regression");
        using var mixed = Layout("abc سلام xyz", "Arabic", "ar");
        var firstArabic = Glyph(mixed, 4);
        var lastArabic = Glyph(mixed, 7);
        Require(firstArabic.writingDirection == TextDirection.rtl
            && firstArabic.graphemeClusterLayoutBounds.left > lastArabic.graphemeClusterLayoutBounds.left,
            "An RTL run within an LTR paragraph places the earlier Arabic cluster on the right.");
        var bounds = firstArabic.graphemeClusterLayoutBounds;
        var hit = mixed.getPositionForOffset(new Offset(bounds.right - bounds.width / 4, bounds.center.dy));
        Require(hit.offset == firstArabic.graphemeClusterCodeUnitRange.start, "Clicking the leading RTL edge returns its logical start.");
        var selection = mixed.getBoxesForRange(4, 8);
        Require(selection.Count > 0 && selection.All(box => box.direction == TextDirection.rtl)
            && selection.Min(box => box.left) == lastArabic.graphemeClusterLayoutBounds.left
            && selection.Max(box => box.right) == firstArabic.graphemeClusterLayoutBounds.right,
            "The selected Arabic word shares glyph geometry.");

        using var indic = Layout("कि", "Devanagari", "hi");
        Require(Glyph(indic, 1).graphemeClusterCodeUnitRange is { start: 0, end: 2 },
            "The Indic vowel and base use one UTF-16 editing cluster.");
        using var emoji = Layout("👩‍💻🙂", "Arabic", "en");
        Require(Glyph(emoji, 3).graphemeClusterCodeUnitRange is { start: 0, end: 5 }
            && Glyph(emoji, 6).graphemeClusterCodeUnitRange is { start: 5, end: 7 },
            "A fallback font handles each entire emoji grapheme.");

        using var thaiWord = Layout("ภาษาไทย", "Thai", "th");
        using var thai = Layout("ภาษาไทยภาษาไทย", "Thai", "th", thaiWord.maxIntrinsicWidth + 1);
        Require(thai.getWordBoundary(new TextPosition(2)) is { start: 0, end: 4 }
            && thai.getWordBoundary(new TextPosition(8)) is { start: 7, end: 11 }
            && thai.getLineBoundary(new TextPosition(0)) is { start: 0, end: 7 }
            && thai.numberOfLines == 2,
            $"Thai word selection and wrapping use dictionary boundaries. " +
            $"word2={thai.getWordBoundary(new TextPosition(2))}, word8={thai.getWordBoundary(new TextPosition(8))}, " +
            $"firstLine={thai.getLineBoundary(new TextPosition(0))}, lines={thai.numberOfLines}");
        var line = thai.computeLineMetrics()[0];
        var softWrap = thai.getPositionForOffset(new Offset(line.width + 10, line.baseline - line.ascent / 2));
        Require(softWrap.offset == 7 && softWrap.affinity == TextAffinity.upstream,
            "The trailing edge of the wrapped line preserves upstream affinity.");
        _ = thai.getBoxesForRange(0, thai.text.Length);
        var afterSelection = thai.getPositionForOffset(new Offset(line.width + 10, line.baseline - line.ascent / 2));
        Require(afterSelection == softWrap, "A line hit agrees with the full selection geometry.");
        Require(thai.getLineBoundary(new TextPosition(7)) is { start: 7, end: 14 },
            "Downstream affinity resolves the same offset on the following line.");

        // These offsets also agree with ICU's word and line iterators.
        foreach (var (family, locale, first, second) in new[]
        {
            ("Lao", "lo", "ດີ", "ຂອບໃຈ"),
            ("Khmer", "km", "តើ", "លោក"),
            ("Burmese", "my", "လူ", "တိုင်း"),
        })
        {
            using var firstWord = Layout(first, family, locale);
            using var secondWord = Layout(second, family, locale);
            using var combined = Layout(first + second, family, locale,
                Math.Max(firstWord.maxIntrinsicWidth, secondWord.maxIntrinsicWidth) + 1);
            var left = combined.getWordBoundary(new TextPosition(0));
            var right = combined.getWordBoundary(new TextPosition(first.Length));
            var lineEnd = combined.getLineBoundary(new TextPosition(0)).end;
            Require(left.start == 0 && left.end == first.Length && right.start == first.Length
                && right.end == combined.text.Length && combined.numberOfLines == 2 && lineEnd == first.Length,
                $"{family} selection and wrapping split at {first.Length}. Words {left}, {right}, first line ends {lineEnd}.");
            var lowerBoxes = combined.getBoxesForRange(right.start, right.end);
            Require(combined.getBoxesForRange(left.start, left.end).Count > 0
                && lowerBoxes.Count > 0 && lowerBoxes.All(box => box.top > 0),
                $"{family} word ranges correspond to the placed lines.");
            var wordPixels = await PaintSample(combined);
            var secondLine = checked((int)Math.Floor(lowerBoxes[0].top + 4)) * 200 * 4;
            Require(wordPixels.AsSpan(0, secondLine).IndexOfAnyExcept((byte)0) >= 0
                && wordPixels.AsSpan(secondLine).IndexOfAnyExcept((byte)0) >= 0,
                $"{family} paints on both lines.");
        }

        using var blank = Layout("abc\r\n\r\nxyz", "Arabic", "en");
        var blankLine = blank.computeLineMetrics()[1];
        Require(blank.getPositionForOffset(new Offset(100, blankLine.baseline)).offset == 5,
            "A click on an empty line before CRLF places the caret before its newline.");

        using var elided = renderer.Layout(new ParagraphRequest("long Arabic سلام and Latin text", 95,
            "Arabic", 24, MaxLines: 1, Locale: new Locale("ar"), Ellipsis: "…"), invoke);
        Require(elided.didExceedMaxLines && elided.numberOfLines == 1 && elided.getGlyphInfoAt(elided.text.Length - 1) is null,
            "Ellipsis keeps truncated text outside selectable geometry.");
        using var ellipsisOnly = renderer.Layout(new ParagraphRequest("abcd", 0, "Arabic", 24, MaxLines: 1, Ellipsis: "…"), invoke);
        var ellipsisHit = ellipsisOnly.getPositionForOffset(Offset.zero);
        Require(ellipsisHit is { offset: 0, affinity: TextAffinity.upstream },
            $"An ellipsis without source glyphs preserves the line-end affinity. Found {ellipsisHit}.");

        var reflowRequest = new ParagraphRequest("abc سلام 12 भाषा\r\nภาษาไทยภาษาไทย Z  ", 650, "Arabic", 24);
        foreach (var (align, direction) in new[] { (TextAlign.left, TextDirection.ltr), (TextAlign.center, TextDirection.ltr),
            (TextAlign.start, TextDirection.rtl), (TextAlign.justify, TextDirection.ltr), (TextAlign.end, TextDirection.rtl) })
        {
            var styled = reflowRequest with { TextAlign = align, TextDirection = direction };
            using var resizable = renderer.Layout(styled, invoke);
            foreach (var nextWidth in new[] { 400.0, 400.25, 45.0, 45.25, 100.0, 100.0, 550.0, double.PositiveInfinity })
            {
                resizable.layout(new ParagraphConstraints(nextWidth));
                using var fresh = renderer.Layout(styled with { Width = nextWidth }, invoke);
                RequireMatchingGeometry(resizable, fresh);
            }
        }

        const string indexedText = "  Price 12.00 Add to cart  Price 24.00 Add one more.   ";
        var indexedRequest = new ParagraphRequest(indexedText, double.PositiveInfinity, "Arabic", 20);
        using var indexed = renderer.Layout(indexedRequest, invoke);
        var minWidth = indexed.minIntrinsicWidth;
        var fullWidth = indexed.maxIntrinsicWidth;
        foreach (var nextWidth in new[] { Math.BitDecrement(fullWidth), fullWidth, Math.BitIncrement(fullWidth),
            minWidth, Math.BitDecrement(minWidth), 0, 128, double.PositiveInfinity })
        {
            indexed.layout(new ParagraphConstraints(nextWidth));
            // With an explicit maximum above the text length, the ellipsis
            // selects the general line breaker without truncating the text.
            using var general = renderer.Layout(indexedRequest with { Width = nextWidth, MaxLines = indexedText.Length + 1,
                Ellipsis = "…" }, invoke);
            RequireMatchingGeometry(indexed, general);
        }

        using var evicted = renderer.Layout(reflowRequest, invoke);
        var retained = new List<Paragraph>();
        try
        {
            for (var i = 0; i < 17; i++) retained.Add(Layout($"Cache pressure {i}", "Arabic", "en"));
            evicted.layout(new ParagraphConstraints(95));
            using var fresh = renderer.Layout(reflowRequest with { Width = 95 }, invoke);
            RequireMatchingGeometry(evicted, fresh);
        }
        finally
        {
            foreach (var item in retained) item.Dispose();
        }

        fonts.Register(fontBytes("Arabic"), "Replaceable");
        using var replaceable = Layout("123", "Replaceable", "en", 200);
        var originalWidth = replaceable.maxIntrinsicWidth;
        var deferredRecorder = new PictureRecorder();
        new Canvas(deferredRecorder).drawParagraph(replaceable, new Offset(4, 4));
        using var deferredPicture = deferredRecorder.endRecording();

        using var tabular = LayoutStyled("١٢٣", "ar", new TextStyle(fontFeatures: [FontFeature.enable("tnum")]));
        using var proportional = LayoutStyled("١٢٣", "ar", new TextStyle(fontFeatures: [FontFeature.enable("pnum")]));
        Require(Math.Abs(tabular.maxIntrinsicWidth - proportional.maxIntrinsicWidth) > 0.5,
            "The selected Arabic numeral feature changes the glyph advances.");
        using var regular = Layout("ABC 123", "Arabic", "en");
        using var condensed = LayoutStyled("ABC 123", "en", new TextStyle(fontVariations: [new FontVariation("wdth", 62.5)]));
        Require(condensed.maxIntrinsicWidth < regular.maxIntrinsicWidth - 0.5,
            "The width axis affects the placed advances.");
        await RequireDifferentPaint(tabular, proportional);
        await RequireDifferentPaint(regular, condensed);

        const string cachedText = "A repeated label";
        var foreground = new Paint { color = new Color(0xffff0000) };
        var cacheRequest = new ParagraphRequest(cachedText, 200, "Arabic", 24, Locale: new Locale("en"),
            TextRuns: [new ParagraphTextRun(cachedText, new TextStyle(foreground: foreground))]);
        using var red = renderer.Layout(cacheRequest, invoke);
        using var narrowerRed = renderer.Layout(cacheRequest with { Width = 60 }, invoke);
        red.layout(new ParagraphConstraints(60));
        RequireMatchingGeometry(red, narrowerRed);
        foreground.color = new Color(0xff0000ff);
        using var blue = renderer.Layout(cacheRequest with { Width = 60 }, invoke);
        RequireMatchingGeometry(red, blue);
        await RequireDifferentPaint(red, blue);

        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder);
        canvas.drawParagraph(mixed, new Offset(10, 10));
        canvas.drawParagraph(indic, new Offset(10, 48));
        canvas.drawParagraph(emoji, new Offset(10, 86));
        canvas.drawParagraph(thai, new Offset(10, 124));
        using var picture = recorder.endRecording();
        var pixels = await Render(picture, ImageByteFormat.rawRgba, 300, 210);
        foreach (var (top, height) in new[] { (10, 38), (48, 38), (86, 38), (124, 55) })
        {
            var area = pixels.AsSpan(top * 300 * 4, height * 300 * 4);
            Require(area.IndexOfAnyExcept((byte)0) >= 0, $"Retained text paints within row {top}.");
        }
        mixed.layout(new ParagraphConstraints(45));
        mixed.Dispose();
        await renderer.RegisterFontAsync(fontBytes("Devanagari"), "Replaceable");
        replaceable.layout(new ParagraphConstraints(200));
        using var replacement = Layout("123", "Replaceable", "en", 200);
        Require(replaceable.maxIntrinsicWidth != originalWidth, "Replacing a font changes a retained paragraph's next layout.");
        RequireMatchingGeometry(replaceable, replacement);
        var replay = await Render(picture, ImageByteFormat.rawRgba, 300, 210);
        Require(pixels.AsSpan().SequenceEqual(replay), "Relayout, disposal, and font cache invalidation preserve the recorded raster.");
        replaceable.Dispose();
        using var originalFont = Layout("123", "Arabic", "en", 200);
        var originalFontPixels = await PaintSample(originalFont);
        var deferredPixels = await Render(deferredPicture, ImageByteFormat.rawRgba, 200, 128);
        Require(originalFontPixels.AsSpan().SequenceEqual(deferredPixels),
            "A picture first painted after font replacement and disposal still uses its original placed glyphs.");

        var png = await Render(picture, ImageByteFormat.png, 300, 210);
        return ("Product paragraph | mixed RTL, Indic and emoji UTF-16, Thai/Lao/Khmer/Burmese word/wrap, caret affinity, semantics, ellipsis, reflow, font replacement, OpenType, retained paint", png);

        Paragraph Layout(string text, string family, string locale, double width = double.PositiveInfinity) =>
            renderer.Layout(new ParagraphRequest(text, width, family, 24, Locale: new Locale(locale)), invoke);

        Paragraph LayoutStyled(string text, string locale, TextStyle style) =>
            renderer.Layout(new ParagraphRequest(text, double.PositiveInfinity, "Arabic", 24, Locale: new Locale(locale),
                TextRuns: [new ParagraphTextRun(text, style)]), invoke);

        async Task RequireDifferentPaint(Paragraph first, Paragraph second)
        {
            var left = await PaintSample(first);
            var right = await PaintSample(second);
            Require(left.AsSpan().IndexOfAnyExcept((byte)0) >= 0 && right.AsSpan().IndexOfAnyExcept((byte)0) >= 0
                && !left.AsSpan().SequenceEqual(right), "Changing the OpenType style changes the retained raster.");
        }

        async Task<byte[]> PaintSample(Paragraph paragraph)
        {
            var sample = new PictureRecorder();
            new Canvas(sample).drawParagraph(paragraph, new Offset(4, 4));
            using var recorded = sample.endRecording();
            return await Render(recorded, ImageByteFormat.rawRgba, 200, 128);
        }

        async Task<byte[]> Render(Picture recorded, ImageByteFormat format, int width, int height)
        {
            using var image = await renderer.RasterizeAsync(recorded, width, height, invoke);
            var bytes = await image.toByteData(format) ?? throw new InvalidOperationException("Missing Skia bitmap.");
            return bytes.asMemory().ToArray();
        }
    }

    private static void CheckSemantics(SkiaSceneRenderer renderer, Host host)
    {
        using var clock = DorotiExecutionContext.EnterTimeProvider(TimeProvider.System);
        using var dispatcher = new PlatformDispatcher();
        var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("paragraph-probe")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, new ParagraphLayout(renderer)));
        dispatcher.RegisterView(2, new DorotiViewCapabilities("paragraph-probe-second-view")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new Host(2)));
        using var scope = view.EnterInvocationScope();
        const string text = "A لله\r\nकि 👩‍💻";
        var paragraph = Render(text);
        try
        {
            paragraph.layout(new BoxConstraints(maxWidth: 300));
            var geometry = Capture(paragraph);
            Require(geometry.CharacterBoundaries.SequenceEqual([0, 1, 2, 5, 7, 9, 10, 15]),
                $"Semantics keeps the Arabic ligature, CRLF, Indic syllable and ZWJ emoji together. "
                + $"Boundaries [{string.Join(", ", geometry.CharacterBoundaries)}] for {geometry.Text.Length} units.");
            var ligature = geometry.Runs.Single(run => run.Start == 2);
            var box = paragraph.getBoxesForSelection(new TextSelection(baseOffset: 2, extentOffset: 5)).Single();
            Require(ligature.End == 5 && ligature.Direction == TextDirection.rtl && ligature.Bounds == box.toRect(),
                "The Arabic ligature exposes one RTL range with the selection rectangle.");
            var point = new Offset(ligature.Bounds.right - ligature.Bounds.width / 4, ligature.Bounds.center.dy);
            Require(geometry.OffsetAtPoint(point) == 2 && paragraph.getPositionForOffset(point).offset == 2,
                "Semantics and caret hit testing agree on the RTL leading edge.");
            Require(geometry.Snap(3) == 2 && geometry.Snap(3, forward: true) == 5,
                "Snapping an offset inside the ligature returns its placed boundaries.");
            var shift = new Offset(3, 5);
            var wire = geometry.Transform(rect => rect.shift(shift)).ToWire();
            Require(wire.boundaries.SequenceEqual(geometry.CharacterBoundaries)
                && wire.runs.Single(run => run.start == 2) is { end: 5, rtl: true } projected
                && projected.left == ligature.Bounds.left + shift.dx,
                "Projection and native wire serialization preserve the cluster.");

            paragraph.layout(new BoxConstraints(maxWidth: 45));
            var relayout = Capture(paragraph);
            Require(relayout.Revision != geometry.Revision
                && relayout.CharacterBoundaries.SequenceEqual(geometry.CharacterBoundaries)
                && geometry.Runs.Single(run => run.Start == 2) == ligature,
                "Wrapping recalculates placement without changing the earlier snapshot.");
        }
        finally { paragraph.dispose(); }

        var elided = Render("ABC long text 👩‍💻", maxLines: 1, overflow: TextOverflow.ellipsis);
        try
        {
            elided.layout(new BoxConstraints(maxWidth: 65));
            var geometry = Capture(elided);
            var emojiStart = geometry.Text.IndexOf("👩", StringComparison.Ordinal);
            Require(geometry.CharacterBoundaries[^1] == geometry.Text.Length
                && geometry.Snap(emojiStart + 1) == emojiStart
                && geometry.Runs.All(run => run.End <= emojiStart),
                "Elided text keeps logical boundaries without exposing rectangles for hidden clusters.");
        }
        finally { elided.dispose(); }

        static RenderParagraph Render(string text, long? maxLines = null, TextOverflow overflow = TextOverflow.clip) =>
            new(new TextSpan(text: text, style: new Doroti.Framework.Painting.TextStyle(fontFamily: "Arabic", fontSize: 24,
                fontFeatures: [FontFeature.enable("dlig")])),
                textAlign: TextAlign.center, textDirection: TextDirection.ltr, locale: new Locale("ar"),
                maxLines: maxLines, overflow: overflow);

        static SemanticsTextGeometry Capture(RenderParagraph paragraph)
        {
            var config = new SemanticsConfiguration();
            paragraph.describeSemanticsConfiguration(config);
            return config.hostTextGeometry ?? throw new InvalidOperationException("RenderParagraph did not publish text geometry.");
        }
    }

    private static void RequireMatchingGeometry(Paragraph actual, Paragraph fresh)
    {
        Require(actual.width == fresh.width && actual.height == fresh.height && actual.longestLine == fresh.longestLine
            && actual.didExceedMaxLines == fresh.didExceedMaxLines
            && actual.computeLineMetrics().SequenceEqual(fresh.computeLineMetrics())
            && actual.getBoxesForRange(0, actual.text.Length).SequenceEqual(fresh.getBoxesForRange(0, fresh.text.Length)),
            $"Reflow matches the fresh line and selection geometry at width {actual.width}.");
        for (var index = 0; index < actual.text.Length; index++)
            Require(Cluster(actual.getGlyphInfoAt(index)) == Cluster(fresh.getGlyphInfoAt(index)),
                $"Reflow matches fresh cluster geometry at UTF-16 offset {index}.");

        static (long Start, long End, Rect Bounds, TextDirection Direction)? Cluster(GlyphInfo? glyph) => glyph is null
            ? null
            : (glyph.graphemeClusterCodeUnitRange.start, glyph.graphemeClusterCodeUnitRange.end,
                glyph.graphemeClusterLayoutBounds, glyph.writingDirection);
    }

    private static GlyphInfo Glyph(Paragraph paragraph, int offset) =>
        paragraph.getGlyphInfoAt(offset) ?? throw new InvalidOperationException($"Missing glyph info at {offset}.");

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class ParagraphLayout(SkiaSceneRenderer renderer) : IParagraphHostCapability
    {
        public Paragraph Layout(ParagraphRequest request, DorotiUiInvocation invocation) => renderer.Layout(request, invocation);
    }

    private sealed class Host(ulong id = 1) : ISkiaSceneRendererHost, IViewHostCapability
    {
        public long InputSequence => 0;
        public long SurfaceGeneration => 1;
        public DorotiViewEpoch ViewEpoch { get; } = new(id, 1, 1, 320, 240, 320, 240, 1, 1, 0);
        public DorotiResizeEpoch ResizeTarget => null!;
        public ViewMetrics Metrics { get; } = new(new Size(320, 240), 1, new(), new(), new(), AppLifecycleState.resumed, 1, 1);
        public PlatformConfiguration Configuration { get; } = new([], Brightness.light, false, false);
        public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
        public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
        public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
        public event Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public void UpdateSemantics(SemanticsUpdate update) { }
        public void ClearSemantics() { }
        public void RequestInvalidate() { }
        public void Show() { }
        public void Resize(Size logicalSize) { }
        public void Close() { }
        public void Dispose() { }
    }
}
