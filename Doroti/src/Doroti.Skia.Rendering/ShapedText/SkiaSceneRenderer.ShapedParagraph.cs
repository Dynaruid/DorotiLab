using System.Runtime.InteropServices;
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
    private readonly Dictionary<object, LinkedListNode<PreparedParagraph>> _preparedByKey = [];
    private readonly LinkedList<PreparedParagraph> _preparedParagraphs = new();
    private long _preparedParagraphWeight;
    private Buffer? _shapingBuffer;

    private Paragraph CreateShapedParagraph(ParagraphRequest request)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var originalRuns = NormalizeTextRuns(request);
        var runs = originalRuns.Count == 0 ? [] : new ParagraphTextRun[originalRuns.Count];
        for (var i = 0; i < runs.Length; i++)
        {
            var run = originalRuns[i];
            runs[i] = new(run.Text, run.Style.SnapshotForPainting());
        }
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
            var key = layout.CacheKey;
            var last = _preparedParagraphs.Last;
            var cached = ReferenceEquals(last?.Value.Key, key) ? last : _preparedByKey.GetValueOrDefault(key);
            if (cached is not null && cached != last)
            {
                _preparedParagraphs.Remove(cached);
                _preparedParagraphs.AddLast(cached);
            }
            var prepared = cached?.Value ?? PrepareParagraph(layout);
            var analysis = prepared.Analysis;
            var widths = prepared.Widths;
            var placedWidth = double.IsFinite(availableWidth) ? availableWidth : widths.Maximum;
            var snapshot = prepared.Placement;
            if (snapshot is null || (availableWidth != prepared.AvailableWidth
                && !prepared.ReusableWidths.Contains(availableWidth)))
            {
                snapshot = PlaceParagraph(layout.Request, layout.Runs, prepared, availableWidth, placedWidth, out var breakWidths);
                prepared.ReusableWidths = !snapshot.DidExceedMaxLines
                    && ResolveLineAlignment(layout.Request.TextAlign, analysis.BaseLevel) == TextAlign.left
                    ? layout.Request.TextAlign == TextAlign.justify
                        ? breakWidths.AtOrAbove(widths.UnwrappedAdvance)
                        : breakWidths
                    : default;
            }
            prepared.Placement = snapshot;
            prepared.AvailableWidth = availableWidth;
            if (cached is null && !_disposed && !layout.Disposed && fontGeneration == _fontGeneration
                && prepared.Weight <= MaxPreparedParagraphWeight && !_preparedByKey.ContainsKey(prepared.Key))
                RetainPreparedParagraph(prepared);
            _paragraphCount++;
            _paragraphMicroseconds += (DorotiFrameClock.Now - started).Ticks / 10;
            return (placedWidth, snapshot);
        }
    }

    private PreparedParagraph PrepareParagraph(ShapedParagraphLayout layout)
    {
        var analysis = layout.Analysis;
        var request = layout.Request;
        var cells = ShapeParagraph(request, layout.Runs, analysis.Clusters, out var emptyMetrics);
        var widths = MeasureIntrinsicWidths(cells, analysis.Breaks);
        var lineAdvances = layout.Runs.Length <= 1 && analysis.BaseLevel == 0
            && (request.TextAlign is null or TextAlign.start or TextAlign.left)
            && string.IsNullOrEmpty(request.Ellipsis) && request.Text.AsSpan().IndexOfAnyExceptInRange(' ', '~') < 0
            ? LineAdvanceIndex.Create(CollectionsMarshal.AsSpan(cells), analysis.Breaks)
            : null;
        var weight = request.Text.Length + widths.GlyphCount + (request.Ellipsis?.Length ?? 0);
        return new(layout.CacheKey, analysis, cells, widths, emptyMetrics, lineAdvances, weight);
    }

    private void RetainPreparedParagraph(PreparedParagraph prepared)
    {
        while (_preparedParagraphs.Count >= MaxPreparedParagraphs
            || _preparedParagraphWeight + prepared.Weight > MaxPreparedParagraphWeight)
        {
            RemovePreparedParagraph(_preparedParagraphs.First!.Value.Key);
        }
        _preparedByKey.Add(prepared.Key, _preparedParagraphs.AddLast(prepared));
        _preparedParagraphWeight += prepared.Weight;
    }

    private void RemovePreparedParagraph(object key)
    {
        if (!_preparedByKey.Remove(key, out var node)) return;
        _preparedParagraphs.Remove(node);
        _preparedParagraphWeight -= node.Value.Weight;
    }

    private void ClearPreparedParagraphs()
    {
        _preparedByKey.Clear();
        _preparedParagraphs.Clear();
        _preparedParagraphWeight = 0;
    }

    private List<ShapedCell> ShapeParagraph(ParagraphRequest request, ParagraphTextRun[] runs,
        ParagraphTextAnalysis.Cluster[] clusters, out VerticalMetrics emptyMetrics)
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
            var color = ToParagraphColor(ParagraphColor(request, style));
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
                    firstFont, color, style, request.Height, cells);
                clusterIndex = next;
            }

            TextRenderResources.TextFontResource Resolve(ParagraphTextAnalysis.Cluster cluster) =>
                resources.ResolveClusterFont(request.Text.AsSpan(cluster.Start, cluster.End - cluster.Start), language);
        }
        return cells;
    }

    private static UiColor? ParagraphColor(ParagraphRequest request, TextStyle? style) =>
        style?.foreground?.color ?? style?.color ?? request.Color;

    private static SKColor ToParagraphColor(UiColor? color) => color is null ? SKColors.Black : ToColor(color);

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
        TextRenderResources.TextFontResource face, SKColor color, TextStyle style,
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
        var run = new ShapedRun(face.RecordingFont, color, glyphs, points);
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
        private readonly SharedParagraphKey? _sharedKey = SharedParagraphKey.Create(request, runs, direction);
        internal ParagraphRequest Request { get; } = request;
        internal ParagraphTextRun[] Runs { get; } = runs;
        internal ParagraphTextAnalysis Analysis => _analysis ??= new(Request.Text, direction);
        internal object CacheKey => (object?)_sharedKey ?? this;
        internal bool Disposed { get; private set; }

        public (double Width, ParagraphHostLayoutSnapshot Snapshot) Layout(double availableWidth) =>
            renderer.LayoutShaped(this, availableWidth);

        public void Dispose()
        {
            lock (renderer._paintGate)
            {
                Disposed = true;
                if (_sharedKey is null) renderer.RemovePreparedParagraph(this);
                _analysis = null;
            }
        }
    }

    // Uniform text can share shaping and immutable placement across Paragraph handles.
    // Multi-run and list-valued styles use the same bounded cache by handle identity.
    private sealed record SharedParagraphKey(string Text, string? Family, double FontSize, SKColor Color,
        Locale? Locale, int Weight, FontStyle? Style, double? LetterSpacing, double? WordSpacing,
        double? HeightMultiplier, double? ParagraphHeight, TextLeadingDistribution? Leading,
        UiDirection Direction, TextAlign? Alignment, long? MaxLines, string? Ellipsis)
    {
        internal static SharedParagraphKey? Create(ParagraphRequest request, ParagraphTextRun[] runs, UiDirection direction)
        {
            if (runs.Length > 1 || request.Text.Length > MaxPreparedParagraphWeight) return null;
            var style = runs.Length == 0 ? null : runs[0].Style;
            if (style is { fontFamilyFallback.Count: > 0 } or { fontFeatures.Count: > 0 } or { fontVariations.Count: > 0 })
                return null;
            var color = ParagraphColor(request, style);
            // Color.value is virtual. Leave custom colors to the normal layout path.
            if (color is not null && color.GetType() != typeof(UiColor)) return null;

            return new(request.Text, style?.fontFamily ?? request.FontFamily, style?.fontSize ?? request.FontSize,
                ToParagraphColor(color),
                style?.locale ?? request.Locale, style?.fontWeight?.value ?? 400, style?.fontStyle,
                style?.letterSpacing, style?.wordSpacing, style?.height, request.Height, style?.leadingDistribution,
                direction, request.TextAlign, request.MaxLines, request.Ellipsis);
        }
    }

    private sealed class PreparedParagraph(object key, ParagraphTextAnalysis analysis, List<ShapedCell> cells,
        IntrinsicWidths widths, VerticalMetrics emptyMetrics, LineAdvanceIndex? lineAdvances, long weight)
    {
        internal object Key { get; } = key;
        internal ParagraphTextAnalysis Analysis { get; } = analysis;
        internal List<ShapedCell> Cells { get; } = cells;
        internal IntrinsicWidths Widths { get; } = widths;
        internal VerticalMetrics EmptyMetrics { get; } = emptyMetrics;
        internal LineAdvanceIndex? LineAdvances { get; } = lineAdvances;
        internal long Weight { get; } = weight;
        internal ParagraphHostLayoutSnapshot? Placement { get; set; }
        internal double AvailableWidth { get; set; }
        internal LineBreakWidths ReusableWidths { get; set; }
    }

    private sealed record ShapedRun(SKFont Font, SKColor Color, ushort[] Glyphs, SKPoint[] Positions);

    private readonly record struct ShapedCell(int Start, int End, sbyte Level, bool HardBreak, bool ResetLevel,
        bool IsSpace, double Advance, VerticalMetrics Metrics, ShapedRun? Run, int GlyphStart, int GlyphCount, float Origin);

    private sealed class ShapedParagraphPaint(PlacedLine[] placements)
    {
        private readonly Lazy<List<ShapedPaintRun>> _runs = new(() => BuildParagraphPaint(placements));

        internal void Draw(SKCanvas canvas, Offset offset)
        {
            var runs = _runs.Value;
            if (runs.Count == 0) return;
            using var paint = new SKPaint { IsAntialias = true };
            foreach (var run in runs)
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
