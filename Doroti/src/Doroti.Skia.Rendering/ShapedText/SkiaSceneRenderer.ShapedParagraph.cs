using Doroti.Skia.Rendering.Unicode;
using Doroti.Ui;
using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Buffer = HarfBuzzSharp.Buffer;
using UiColor = Doroti.Ui.Color;
using UiDirection = Doroti.Ui.TextDirection;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private const int MaxPreparedParagraphs = 16;
    private const int MaxPreparedParagraphWeight = 16_384;
    private const int MaxReusableShapeLength = 4_096;
    private const int ShapingUnitsPerEm = 512;
    private readonly LinkedList<ShapedParagraphLayout> _preparedParagraphs = new();
    private long _preparedParagraphWeight;
    private Buffer? _shapingBuffer;

    private Paragraph CreateShapedParagraph(ParagraphRequest request)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var runs = NormalizeTextRuns(request)
            .Select(run => new ParagraphTextRun(run.Text, run.Style.SnapshotForPainting())).ToArray();
        var direction = request.TextDirection ?? UiDirection.ltr;
        var layout = new ShapedParagraphLayout(this, request, runs, direction);
        var paragraph = new Paragraph(request, runs, layout)
        {
            LayoutTextAlign = request.TextAlign ?? TextAlign.start,
            LayoutTextDirection = direction,
        };
        try
        {
            if (!request.DeferLayout)
                paragraph.layout(new ParagraphConstraints(request.Width));
            return paragraph;
        }
        catch
        {
            paragraph.Dispose();
            throw;
        }
    }

    private (double Width, ParagraphHostLayoutSnapshot Snapshot) LayoutShaped(
        ShapedParagraphLayout layout, double availableWidth)
    {
        var started = DorotiFrameClock.Now;
        lock (_paintGate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            ObjectDisposedException.ThrowIf(layout.Disposed, layout);
            var fontGeneration = _fontGeneration;
            var analysis = layout.Analysis;
            // Remove the lease from the idle cache while it is in use. Only
            // the active call can release these fonts during placement.
            var prepared = TakePreparedParagraph(layout) ?? PrepareParagraph(layout.Request, layout.Runs, analysis);
            try
            {
                var widths = prepared.Widths;
                var placedWidth = double.IsFinite(availableWidth) ? availableWidth : widths.Maximum;
                var snapshot = prepared.Placement;
                if (snapshot is null || (availableWidth != prepared.AvailableWidth
                    && (snapshot.DidExceedMaxLines || availableWidth < widths.UnwrappedAdvance
                        || prepared.AvailableWidth < widths.UnwrappedAdvance
                        || ResolveLineAlignment(layout.Request.TextAlign, analysis.BaseLevel) != TextAlign.left)))
                {
                    snapshot = PlaceParagraph(layout.Request, layout.Runs, analysis, prepared.Cells,
                        availableWidth, placedWidth, prepared.EmptyMetrics, widths);
                }
                prepared.Placement = snapshot;
                prepared.AvailableWidth = availableWidth;
                _paragraphCount++;
                _paragraphMicroseconds += (DorotiFrameClock.Now - started).Ticks / 10;
                return (placedWidth, snapshot);
            }
            finally
            {
                if (_disposed || layout.Disposed || fontGeneration != _fontGeneration
                    || prepared.Weight > MaxPreparedParagraphWeight || prepared.Placement is null
                    || layout.Prepared is not null)
                    prepared.Dispose();
                else
                    RetainPreparedParagraph(layout, prepared);
            }
        }
    }

    private PreparedParagraph PrepareParagraph(ParagraphRequest request, ParagraphTextRun[] runs,
        ParagraphTextAnalysis analysis)
    {
        var fonts = new ShapingFonts();
        try
        {
            var cells = ShapeParagraph(request, runs, analysis.Clusters, fonts, out var emptyMetrics);
            var widths = MeasureIntrinsicWidths(cells, analysis.Breaks);
            return new(cells, fonts, widths, emptyMetrics, request.Text.Length + widths.GlyphCount);
        }
        catch
        {
            fonts.Dispose();
            throw;
        }
    }

    private void RetainPreparedParagraph(ShapedParagraphLayout layout, PreparedParagraph prepared)
    {
        while (_preparedParagraphs.Count >= MaxPreparedParagraphs
            || _preparedParagraphWeight + prepared.Weight > MaxPreparedParagraphWeight)
        {
            TakePreparedParagraph(_preparedParagraphs.First!.Value)!.Dispose();
        }
        layout.Prepared = prepared;
        _preparedParagraphs.AddLast(layout.CacheNode ??= new(layout));
        _preparedParagraphWeight += prepared.Weight;
    }

    private PreparedParagraph? TakePreparedParagraph(ShapedParagraphLayout layout)
    {
        var prepared = layout.Prepared;
        if (prepared is null) return null;
        _preparedParagraphs.Remove(layout.CacheNode!);
        layout.Prepared = null;
        _preparedParagraphWeight -= prepared.Weight;
        return prepared;
    }

    private void ClearPreparedParagraphs()
    {
        while (_preparedParagraphs.First is { } oldest)
            TakePreparedParagraph(oldest.Value)!.Dispose();
    }

    private List<ShapedCell> ShapeParagraph(ParagraphRequest request, ParagraphTextRun[] runs,
        ParagraphTextAnalysis.Cluster[] clusters, ShapingFonts fonts, out VerticalMetrics emptyMetrics)
    {
        var cells = new List<ShapedCell>(clusters.Length);
        var clusterIndex = 0;
        var runEnd = 0;
        emptyMetrics = default;
        if (runs.Length == 0)
            runs = [new ParagraphTextRun(request.Text, new TextStyle(fontSize: request.FontSize, fontFamily: request.FontFamily))];

        foreach (var run in runs)
        {
            runEnd += run.Text.Length;
            var style = run.Style;
            var color = ToColor(style.foreground?.color ?? style.color ?? request.Color ?? new UiColor(0xff000000));
            var resources = GetTextRenderResources(style.fontFamily ?? request.FontFamily,
                (float)(style.fontSize ?? request.FontSize), color, style);
            var language = (style.locale ?? request.Locale)?.toLanguageTag() ?? "und";
            using var shapingLanguage = new Language(language);
            var features = CreateShapingFeatures(style.fontFeatures);
            if (emptyMetrics.Height == 0)
                emptyMetrics = MeasureHeight(resources.PrimaryFont.Font, style, request.Height);

            while (clusterIndex < clusters.Length && clusters[clusterIndex].Start < runEnd)
            {
                var first = clusters[clusterIndex];
                if (first.IsHardBreak)
                {
                    cells.Add(new(first.Start, first.End, first.Level, true, true, false, 0,
                        MeasureHeight(resources.PrimaryFont.Font, style, request.Height), null, 0, 0, 0));
                    clusterIndex++;
                    continue;
                }

                var firstFont = Resolve(first);
                var next = clusterIndex + 1;
                while (next < clusters.Length)
                {
                    var candidate = clusters[next];
                    if (candidate.Start >= runEnd || candidate.IsHardBreak || candidate.Level != first.Level
                        || candidate.Script != first.Script || !ReferenceEquals(firstFont, Resolve(candidate)))
                        break;
                    next++;
                }
                ShapeRun(request.Text, first.Start, clusters[next - 1].End, first.Level, shapingLanguage, features,
                    firstFont, fonts.Get(firstFont), color, style, request.Height, cells);
                clusterIndex = next;
            }

            TextRenderResources.TextFontResource Resolve(ParagraphTextAnalysis.Cluster cluster) =>
                resources.ResolveClusterFont(request.Text.AsSpan(cluster.Start, cluster.End - cluster.Start), language);
        }
        return cells;
    }

    private static Feature[] CreateShapingFeatures(IReadOnlyList<FontFeature>? features)
    {
        if (features is not { Count: > 0 }) return [];
        var result = new Feature[features.Count];
        for (var i = 0; i < result.Length; i++)
        {
            var (tag, value) = features[i];
            if (tag.Length != 4 || tag.AsSpan().IndexOfAnyExceptInRange(' ', '~') >= 0 || value is < 0 or > uint.MaxValue)
                throw new ArgumentException("OpenType features require a four-character ASCII tag and an unsigned 32-bit value.", nameof(features));
            result[i] = new(new Tag(tag[0], tag[1], tag[2], tag[3]), (uint)value);
        }
        return result;
    }

    private static Font CreateShapingFont(SKTypeface typeface)
    {
        using var blob = typeface.OpenStream(out var index).ToHarfBuzzBlob();
        using var face = new Face(blob, index) { Index = index, UnitsPerEm = typeface.UnitsPerEm };
        var variations = typeface.VariationDesignPosition;
        Span<Variation> axes = variations.Length <= 16 ? stackalloc Variation[variations.Length] : new Variation[variations.Length];
        for (var i = 0; i < axes.Length; i++)
            axes[i] = new() { Tag = (uint)variations[i].Axis, Value = variations[i].Value };

        var font = new Font(face);
        try
        {
            font.SetScale(ShapingUnitsPerEm, ShapingUnitsPerEm);
            font.SetFunctionsOpenType();
            if (!axes.IsEmpty) font.SetVariations(axes);
            return font;
        }
        catch
        {
            font.Dispose();
            throw;
        }
    }

    private void ShapeRun(string text, int start, int end, sbyte level, Language language, Feature[] features,
        TextRenderResources.TextFontResource face, SKFont retainedFont, SKColor color, TextStyle style,
        double? paragraphHeight, List<ShapedCell> output)
    {
        using var largeBuffer = end - start > MaxReusableShapeLength ? new Buffer() : null;
        var buffer = largeBuffer ?? (_shapingBuffer ??= new());
        buffer.ClearContents();
        buffer.AddUtf16(text.AsSpan(), start, end - start);
        buffer.Direction = (level & 1) == 0 ? Direction.LeftToRight : Direction.RightToLeft;
        buffer.Language = language;
        buffer.GuessSegmentProperties();
        face.ShapingFont.Shape(buffer, features);
        var glyphInfo = buffer.GetGlyphInfoSpan();
        var positions = buffer.GetGlyphPositionSpan();
        var glyphs = new ushort[glyphInfo.Length];
        var points = new SKPoint[glyphInfo.Length];
        var run = new ShapedRun(retainedFont, color, glyphs, points);
        var ascent = MeasureHeight(face.Font, style, paragraphHeight);
        var scaleY = face.Font.Size / ShapingUnitsPerEm;
        var scaleX = scaleY * face.Font.ScaleX;
        var firstCell = output.Count;
        var x = 0f;
        var y = 0f;
        for (var i = 0; i < glyphs.Length; )
        {
            var cluster = glyphInfo[i].Cluster;
            var origin = x;
            var j = i;
            var rawAdvance = 0L;
            do
            {
                var position = positions[j];
                glyphs[j] = checked((ushort)glyphInfo[j].Codepoint);
                points[j] = new(x + position.XOffset * scaleX, y - position.YOffset * scaleY);
                x += position.XAdvance * scaleX;
                y += position.YAdvance * scaleY;
                rawAdvance += position.XAdvance;
                j++;
            }
            while (j < glyphs.Length && glyphInfo[j].Cluster == cluster);

            var offset = checked((int)cluster);
            output.Add(new(offset, offset, level, false, false, false, rawAdvance * (double)scaleX, ascent,
                run, i, j - i, origin));
            i = j;
        }
        if ((level & 1) != 0) output.Reverse(firstCell, output.Count - firstCell);
        if (firstCell == output.Count || output[firstCell].Start > start)
            output.Insert(firstCell, new(start, start, level, false, false, false, 0, ascent, null, 0, 0, 0));

        for (var i = firstCell; i < output.Count; i++)
        {
            var cell = output[i];
            var cellEnd = i + 1 == output.Count ? end : output[i + 1].Start;
            if (cell.Start < start || cellEnd > end || cellEnd <= cell.Start)
                throw new InvalidDataException("HarfBuzz returned unordered or out-of-range UTF-16 clusters.");

            var space = text[cell.Start] == ' ';
            var reset = space;
            if (text[cell.Start] is < ' ' or > '~')
            {
                var first = Codepoint.ReadAt(text, cell.Start, out _);
                space = first.LineBreakClass == LineBreakClass.Space;
                reset = first.BiDiClass is BidiClass.WhiteSpace or BidiClass.SegmentSeparator
                    or BidiClass.BoundaryNeutral or BidiClass.LeftToRightIsolate or BidiClass.RightToLeftIsolate
                    or BidiClass.FirstStrongIsolate or BidiClass.PopDirectionalIsolate;
            }
            var spacing = cell.Advance > 0 ? (style.letterSpacing ?? 0) + (space ? style.wordSpacing ?? 0 : 0) : 0;
            output[i] = cell with { End = cellEnd, IsSpace = space, ResetLevel = reset,
                Advance = Math.Max(0, cell.Advance + spacing) };
        }
    }

    private static VerticalMetrics MeasureHeight(SKFont font, TextStyle style, double? paragraphHeight)
    {
        var metrics = font.Metrics;
        var ascent = Math.Max(0, -metrics.Ascent);
        var descent = Math.Max(0, metrics.Descent);
        var height = ascent + descent;
        var requested = Math.Max(0, style.height is { } multiplier
            ? font.Size * multiplier
            : paragraphHeight ?? height);
        var above = style.leadingDistribution == TextLeadingDistribution.even
            ? (requested - height) / 2
            : (requested - height) * ascent / Math.Max(1, height);
        var effectiveAscent = Math.Clamp(ascent + above, 0, requested);
        return new(effectiveAscent, requested - effectiveAscent);
    }

    private readonly record struct VerticalMetrics(double Ascent, double Descent)
    {
        internal double Height => Ascent + Descent;
    }

    private sealed class ShapedParagraphLayout(SkiaSceneRenderer renderer, ParagraphRequest request,
        ParagraphTextRun[] runs, UiDirection direction) : IParagraphHostLayout
    {
        private ParagraphTextAnalysis? _analysis;
        internal ParagraphRequest Request { get; } = request;
        internal ParagraphTextRun[] Runs { get; } = runs;
        internal ParagraphTextAnalysis Analysis => _analysis ??= new(Request.Text, direction);
        internal LinkedListNode<ShapedParagraphLayout>? CacheNode { get; set; }
        internal PreparedParagraph? Prepared { get; set; }
        internal bool Disposed { get; private set; }

        public (double Width, ParagraphHostLayoutSnapshot Snapshot) Layout(double availableWidth) =>
            renderer.LayoutShaped(this, availableWidth);

        public void Dispose()
        {
            lock (renderer._paintGate)
            {
                Disposed = true;
                renderer.TakePreparedParagraph(this)?.Dispose();
                _analysis = null;
            }
        }
    }

    private sealed class PreparedParagraph(List<ShapedCell> cells, ShapingFonts fonts,
        IntrinsicWidths widths, VerticalMetrics emptyMetrics, long weight) : IDisposable
    {
        internal List<ShapedCell> Cells { get; } = cells;
        internal IntrinsicWidths Widths { get; } = widths;
        internal VerticalMetrics EmptyMetrics { get; } = emptyMetrics;
        internal long Weight { get; } = weight;
        internal ParagraphHostLayoutSnapshot? Placement { get; set; }
        internal double AvailableWidth { get; set; }

        public void Dispose() => fonts.Dispose();
    }

    private sealed record ShapedRun(SKFont Font, SKColor Color, ushort[] Glyphs, SKPoint[] Positions);

    private readonly record struct ShapedCell(int Start, int End, sbyte Level, bool HardBreak, bool ResetLevel,
        bool IsSpace, double Advance, VerticalMetrics Metrics, ShapedRun? Run, int GlyphStart, int GlyphCount, float Origin);

    private sealed class ShapingFonts : IDisposable
    {
        private readonly Dictionary<TextRenderResources.TextFontResource, SKFont> _fonts = [];

        internal SKFont Get(TextRenderResources.TextFontResource resource)
        {
            if (_fonts.TryGetValue(resource, out var retained))
                return retained;
            var font = resource.Font;
            retained = new SKFont(resource.Typeface, font.Size, font.ScaleX, font.SkewX)
            {
                Embolden = font.Embolden,
                Edging = font.Edging,
                Hinting = font.Hinting,
                Subpixel = font.Subpixel,
            };
            _fonts.Add(resource, retained);
            return retained;
        }

        public void Dispose()
        {
            foreach (var font in _fonts.Values) font.Dispose();
            _fonts.Clear();
        }
    }

    private sealed record ShapedParagraphPaint(IReadOnlyList<ShapedPaintRun> Runs)
    {
        internal void Draw(SKCanvas canvas, Offset offset)
        {
            if (Runs.Count == 0) return;
            using var paint = new SKPaint { IsAntialias = true };
            foreach (var run in Runs)
            {
                paint.Color = run.Color;
                canvas.DrawText(run.Blob, (float)offset.dx, (float)offset.dy, paint);
            }
        }
    }

    // The immutable native blob owns its typeface and positioned data. The
    // public Paragraph and every recorded Picture share this managed owner.
    // Skia releases it after the last retained snapshot is collected.
    private sealed record ShapedPaintRun(SKTextBlob Blob, SKColor Color);
}
