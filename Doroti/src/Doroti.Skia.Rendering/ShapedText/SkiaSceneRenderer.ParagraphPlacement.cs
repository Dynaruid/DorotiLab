using System.Runtime.InteropServices;
using Doroti.Ui;
using SkiaSharp;
using UiDirection = Doroti.Ui.TextDirection;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private ParagraphHostLayoutSnapshot PlaceParagraph(ParagraphRequest request, ParagraphTextRun[] runs,
        PreparedParagraph prepared, double availableWidth, double width, out LineBreakWidths breakWidths)
    {
        var analysis = prepared.Analysis;
        var cells = prepared.Cells;
        var widths = prepared.Widths;
        var emptyMetrics = prepared.EmptyMetrics;
        var advances = prepared.LineAdvances;
        var lineRanges = advances is not null && availableWidth >= widths.Minimum
            ? advances.BreakLines(availableWidth, out breakWidths)
            : BreakLines(CollectionsMarshal.AsSpan(cells), analysis.Breaks, availableWidth, out breakWidths);
        var limit = request.MaxLines ?? lineRanges.Count;
        if (limit < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Paragraph max lines must be nonnegative.");
        if (request.MaxLines is null && !string.IsNullOrEmpty(request.Ellipsis))
        {
            var firstEmergencyBreak = lineRanges.FindIndex(line => line.EmergencyBreak);
            if (firstEmergencyBreak >= 0) limit = firstEmergencyBreak + 1;
        }

        var lineCount = (int)Math.Min(limit, lineRanges.Count);
        var truncated = lineCount < lineRanges.Count;
        var lines = new ParagraphHostLineSnapshot[lineCount];
        var placements = new PlacedLine[lineCount];
        var geometryCount = 0;
        var top = 0.0;
        var longest = 0.0;
        for (var i = 0; i < lineCount; i++)
        {
            var range = lineRanges[i];
            var start = range.Start < cells.Count ? cells[range.Start].Start : request.Text.Length;
            var last = truncated && i == lineCount - 1;
            var source = cells;
            var first = range.Start;
            var count = range.End - range.Start;
            if (last && request.Ellipsis is { Length: > 0 } ellipsis)
            {
                source = cells.GetRange(first, count);
                AppendEllipsis(request, runs, analysis.BaseLevel, source, start, availableWidth, ellipsis);
                first = 0;
                count = source.Count;
            }

            var isSoftWrap = !last && !range.HardBreak && i + 1 < lineRanges.Count;
            var line = advances is not null
                ? advances.PlaceLine(cells, range, top, out placements[i])
                : PlaceLine(request.TextAlign, analysis.BaseLevel, source, first, count, start,
                    top, width, isSoftWrap, emptyMetrics, out placements[i]);
            lines[i] = line;
            geometryCount += placements[i].GeometryCount;
            top += line.Height;
            longest = Math.Max(longest, line.Width);
        }
        var maximumAscent = lines.Length == 0 ? emptyMetrics.Ascent : lines[0].Ascent;
        var firstHeight = lines.Length == 0 ? emptyMetrics.Height : lines[0].Height;
        if (request.Text.Length == 0) top = emptyMetrics.Height;
        var geometry = new PlacedGraphemes(placements, geometryCount);
        return new ParagraphHostLayoutSnapshot(top, maximumAscent, firstHeight,
            widths.Minimum, widths.Maximum, longest, truncated, lines,
            geometry.Build, geometry.GetLine, request.Text.Length, analysis.WordBoundaries,
            new ShapedParagraphPaint(placements));
    }

    private static IntrinsicWidths MeasureIntrinsicWidths(List<ShapedCell> cells, ReadOnlySpan<bool> breaks)
    {
        var lineWidth = 0.0;
        var wordWidth = 0.0;
        var trailingSpace = 0.0;
        var minimum = 0.0;
        var maximum = 0.0;
        var unwrappedAdvance = 0.0;
        var glyphCount = 0L;
        foreach (ref readonly var cell in CollectionsMarshal.AsSpan(cells))
        {
            glyphCount += cell.GlyphCount;
            lineWidth += cell.Advance;
            wordWidth += cell.Advance;
            if (!cell.HardBreak)
                trailingSpace = cell.IsSpace ? trailingSpace + cell.Advance : 0;
            if (cell.HardBreak || breaks[cell.End])
            {
                minimum = Math.Max(minimum, wordWidth - trailingSpace);
                wordWidth = 0;
            }
            if (cell.HardBreak)
            {
                maximum = Math.Max(maximum, lineWidth - trailingSpace);
                unwrappedAdvance = Math.Max(unwrappedAdvance, lineWidth);
                lineWidth = 0;
                trailingSpace = 0;
            }
        }

        return new(Math.Max(minimum, wordWidth - trailingSpace), Math.Max(maximum, lineWidth - trailingSpace),
            Math.Max(unwrappedAdvance, lineWidth), glyphCount);
    }

    private static List<LineRange> BreakLines(ReadOnlySpan<ShapedCell> cells, ReadOnlySpan<bool> breaks,
        double width, out LineBreakWidths breakWidths)
    {
        var ranges = new List<LineRange>();
        var minimum = 0.0;
        var maximum = double.PositiveInfinity;
        for (var start = 0; start < cells.Length; )
        {
            var cursor = start;
            var lastBreak = start;
            var advance = 0.0;
            var lastFitting = 0.0;
            var hard = false;
            var emergency = false;
            for (; cursor < cells.Length; cursor++)
            {
                ref readonly var cell = ref cells[cursor];
                if (cell.HardBreak)
                {
                    hard = true;
                    cursor++;
                    break;
                }
                advance += cell.Advance;
                if (!cell.IsSpace)
                {
                    if (advance > width)
                    {
                        maximum = Math.Min(maximum, advance);
                        emergency = lastBreak == start;
                        cursor = emergency ? Math.Max(start + 1, cursor) : lastBreak;
                        break;
                    }
                    lastFitting = advance;
                }
                if (breaks[cell.End])
                    lastBreak = cursor + 1;
            }
            minimum = Math.Max(minimum, lastFitting);
            ranges.Add(new(start, cursor, hard, emergency));
            start = cursor;
        }
        if (ranges.Count != 0 && ranges[^1].HardBreak)
            ranges.Add(new(cells.Length, cells.Length, false, false));
        breakWidths = new(minimum, maximum);
        return ranges;
    }

    private void AppendEllipsis(ParagraphRequest request, ParagraphTextRun[] runs, sbyte level,
        List<ShapedCell> cells, int start, double width, string ellipsis)
    {
        var direction = (level & 1) == 0 ? UiDirection.ltr : UiDirection.rtl;
        var style = runs.Length == 0
            ? new TextStyle(fontFamily: request.FontFamily, fontSize: request.FontSize)
            : StyleAt(runs, Math.Max(start, cells.Count == 0 ? start : cells[^1].Start));
        var suffix = ShapeParagraph(request with { Text = ellipsis }, [new ParagraphTextRun(ellipsis, style)],
            new ParagraphTextAnalysis(ellipsis, direction).Clusters, out _);
        while (cells.Count != 0 && (cells[^1].IsSpace || cells[^1].HardBreak))
            cells.RemoveAt(cells.Count - 1);
        var advance = cells.Sum(cell => cell.Advance) + suffix.Sum(cell => cell.Advance);
        while (cells.Count != 0 && advance > width)
        {
            advance -= cells[^1].Advance;
            cells.RemoveAt(cells.Count - 1);
        }
        var offset = cells.Count == 0 ? start : cells[^1].End;
        foreach (var cell in suffix)
            cells.Add(cell with { Start = offset, End = offset });
    }

    private static TextStyle StyleAt(ParagraphTextRun[] runs, int offset)
    {
        foreach (var run in runs)
        {
            if (offset < run.Text.Length) return run.Style;
            offset -= run.Text.Length;
        }
        return runs[^1].Style;
    }

    private static ParagraphHostLineSnapshot PlaceLine(TextAlign? alignment, sbyte baseLevel,
        List<ShapedCell> source, int first, int count, int start, double top, double width,
        bool isSoftWrap, VerticalMetrics emptyMetrics, out PlacedLine placement)
    {
        ReadOnlySpan<ShapedCell> cells = CollectionsMarshal.AsSpan(source).Slice(first, count);
        var ascent = cells.IsEmpty ? emptyMetrics.Ascent : 0;
        var descent = cells.IsEmpty ? emptyMetrics.Descent : 0;
        var visibleEnd = count;
        while (visibleEnd > 0 && cells[visibleEnd - 1].ResetLevel) visibleEnd--;
        var lineWidth = 0.0;
        var spaces = 0;
        for (var i = 0; i < count; i++)
        {
            ref readonly var cell = ref cells[i];
            ascent = Math.Max(ascent, cell.Metrics.Ascent);
            descent = Math.Max(descent, cell.Metrics.Descent);
            if (i >= visibleEnd) continue;
            lineWidth += cell.Advance;
            if (cell.IsSpace) spaces++;
        }
        var justify = alignment == TextAlign.justify && isSoftWrap && spaces > 0 && width > lineWidth;
        var expansion = justify ? (width - lineWidth) / spaces : 0;
        if (justify) lineWidth = width;
        var left = ResolveLineAlignment(alignment, baseLevel) switch
        {
            TextAlign.center => (width - lineWidth) / 2,
            TextAlign.right => width - lineWidth,
            _ => 0,
        };
        var baseline = top + ascent;
        var last = cells.IsEmpty ? default : cells[^1];
        var end = cells.IsEmpty ? start : last.HardBreak ? last.Start : last.End;
        var geometryCount = count;
        // Ellipsis cells paint at the truncation offset without adding selectable text.
        while (geometryCount > 0 && cells[geometryCount - 1].Start == cells[geometryCount - 1].End) geometryCount--;
        placement = new(source, first, count, visibleEnd, geometryCount, baseLevel,
            expansion, left, top, baseline, baseline + descent);
        return new(start, end, last.HardBreak, ascent, descent, ascent + descent, lineWidth, left, baseline);
    }

    private sealed class PlacedGraphemes(PlacedLine[] lines, int count)
    {
        private sealed record HitLine(int Index, ParagraphHostGraphemeSnapshot[] Graphemes);
        private HitLine? _lastHit;

        internal ReadOnlyMemory<ParagraphHostGraphemeSnapshot> GetLine(int index)
        {
            if (Volatile.Read(ref _lastHit) is { } cached && cached.Index == index) return cached.Graphemes;
            ref readonly var line = ref lines[index];
            if (line.GeometryCount == 0) return ReadOnlyMemory<ParagraphHostGraphemeSnapshot>.Empty;
            var graphemes = new ParagraphHostGraphemeSnapshot[line.GeometryCount];
            PlaceLineContent(line, index, graphemes, null, null);
            Volatile.Write(ref _lastHit, new(index, graphemes));
            return graphemes;
        }

        internal ReadOnlyMemory<ParagraphHostGraphemeSnapshot> Build()
        {
            if (count == 0) return ReadOnlyMemory<ParagraphHostGraphemeSnapshot>.Empty;
            var graphemes = new ParagraphHostGraphemeSnapshot[count];
            var cached = Volatile.Read(ref _lastHit);
            var offset = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                ref readonly var line = ref lines[i];
                if (line.GeometryCount == 0) continue;
                var destination = graphemes.AsSpan(offset, line.GeometryCount);
                if (cached?.Index == i) cached.Graphemes.AsSpan().CopyTo(destination);
                else PlaceLineContent(line, i, destination, null, null);
                offset += destination.Length;
            }
            return graphemes;
        }
    }

    private static List<ShapedPaintRun> BuildParagraphPaint(PlacedLine[] lines)
    {
        var paint = new List<ShapedPaintRun>();
        using var builder = new SKTextBlobBuilder();
        foreach (ref readonly var line in lines.AsSpan())
            PlaceLineContent(line, 0, [], builder, paint);
        return paint;
    }

    private static void PlaceLineContent(in PlacedLine line, int index, Span<ParagraphHostGraphemeSnapshot> geometry,
        SKTextBlobBuilder? builder, List<ShapedPaintRun>? paint)
    {
        var cells = line.Cells;
        Span<sbyte> levels = cells.Length <= 256 ? stackalloc sbyte[cells.Length] : new sbyte[cells.Length];
        for (var i = 0; i < line.VisibleEnd; i++) levels[i] = cells[i].Level;
        levels[line.VisibleEnd..].Fill(line.BaseLevel);
        Span<int> order = cells.Length <= 256 ? stackalloc int[cells.Length] : new int[cells.Length];
        ParagraphTextAnalysis.ReorderVisual(order, levels);
        Span<double> positions = cells.Length <= 256 ? stackalloc double[cells.Length] : new double[cells.Length];
        var pen = line.Left;
        // Spaces discarded from the line width can still be selected. For
        // RTL their boxes extend to the left of the aligned content.
        if ((line.BaseLevel & 1) != 0)
            for (var i = line.VisibleEnd; i < cells.Length; i++) pen -= cells[i].Advance;

        foreach (var logical in order)
        {
            ref readonly var cell = ref cells[logical];
            positions[logical] = pen;
            pen += cell.Advance + (logical < line.VisibleEnd && cell.IsSpace ? line.Expansion : 0);
        }
        if (builder is not null)
        {
            RecordGlyphs(cells, order, positions, (float)line.Baseline, builder, paint!);
            return;
        }
        for (var i = 0; i < geometry.Length; i++)
        {
            ref readonly var cell = ref cells[i];
            var advance = cell.Advance + (i < line.VisibleEnd && cell.IsSpace ? line.Expansion : 0);
            var left = positions[i];
            var direction = (levels[i] & 1) == 0 ? UiDirection.ltr : UiDirection.rtl;
            geometry[i] = new(cell.Start, cell.End, left, line.Top, left + advance, line.Bottom,
                line.Top, line.Bottom, direction, index);
        }
    }

    private static TextAlign ResolveLineAlignment(TextAlign? align, sbyte baseLevel) => align switch
    {
        TextAlign.center => TextAlign.center,
        TextAlign.right => TextAlign.right,
        TextAlign.end when (baseLevel & 1) == 0 => TextAlign.right,
        null or TextAlign.start or TextAlign.justify when (baseLevel & 1) != 0 => TextAlign.right,
        _ => TextAlign.left,
    };

    private static void RecordGlyphs(ReadOnlySpan<ShapedCell> cells, ReadOnlySpan<int> order,
        ReadOnlySpan<double> positions, float baseline, SKTextBlobBuilder builder, List<ShapedPaintRun> paint)
    {
        SKColor? color = null;
        for (var start = 0; start < order.Length; )
        {
            var run = cells[order[start]].Run;
            if (run is null)
            {
                start++;
                continue;
            }

            var end = start + 1;
            var count = cells[order[start]].GlyphCount;
            while (end < order.Length && ReferenceEquals(cells[order[end]].Run, run))
            {
                count += cells[order[end]].GlyphCount;
                end++;
            }
            if (color is { } previousColor && previousColor != run.Color)
                FinishBlob(previousColor);
            color = run.Color;
            var buffer = builder.AllocateRawPositionedRun(run.Font, count);
            var glyphs = buffer.Glyphs;
            var points = buffer.Positions;
            var output = 0;
            for (var i = start; i < end; i++)
            {
                var logical = order[i];
                ref readonly var cell = ref cells[logical];
                var translation = (float)positions[logical] - cell.Origin;
                for (var g = cell.GlyphStart; g < cell.GlyphStart + cell.GlyphCount; g++, output++)
                {
                    glyphs[output] = run.Glyphs[g];
                    var point = run.Positions[g];
                    points[output] = new(point.X + translation, point.Y + baseline);
                }
            }
            start = end;
        }
        if (color is { } finalColor)
            FinishBlob(finalColor);

        void FinishBlob(SKColor value)
        {
            var blob = builder.Build() ?? throw new InvalidOperationException("Skia could not record a shaped glyph run.");
            paint.Add(new(blob, value));
        }
    }

    private readonly record struct LineRange(int Start, int End, bool HardBreak, bool EmergencyBreak);
    private readonly record struct AdvancePoint(double Advance, int LastBreak, int VisibleEnd, int NextNonSpace);

    // Uniform left-aligned lines need no per-cell metric scan on every width change.
    private sealed class LineAdvanceIndex(AdvancePoint[] points, VerticalMetrics metrics)
    {
        private const double UnitsPerPixel = 1L << 32;
        private const double LargestExactSum = 1L << 53;

        internal static LineAdvanceIndex? Create(ReadOnlySpan<ShapedCell> cells, ReadOnlySpan<bool> breaks)
        {
            if (cells.Length < 32 || cells.Length > MaxPreparedParagraphWeight) return null;

            var metrics = cells[0].Metrics;
            var points = new AdvancePoint[cells.Length + 1];
            var total = 0.0;
            var lastBreak = 0;
            var visibleEnd = 0;
            for (var i = 0; i < cells.Length; i++)
            {
                ref readonly var cell = ref cells[i];
                var advance = cell.Advance * UnitsPerPixel;
                // Prefix differences and the original per-line sums must be exactly equal.
                if (cell.HardBreak || cell.Level != 0 || cell.ResetLevel != cell.IsSpace || cell.Metrics != metrics
                    || advance < 0 || advance != Math.Truncate(advance) || advance > LargestExactSum - total)
                    return null;

                total += advance;
                if (breaks[cell.End]) lastBreak = i + 1;
                if (!cell.IsSpace) visibleEnd = i + 1;
                points[i + 1] = new(total / UnitsPerPixel, lastBreak, visibleEnd, cells.Length);
            }
            var nextNonSpace = cells.Length;
            for (var i = cells.Length - 1; i >= 0; i--)
            {
                if (!cells[i].IsSpace) nextNonSpace = i;
                points[i] = points[i] with { NextNonSpace = nextNonSpace };
            }
            return new(points, metrics);
        }

        internal List<LineRange> BreakLines(double width, out LineBreakWidths breakWidths)
        {
            var ranges = new List<LineRange>(8);
            var count = points.Length - 1;
            var minimum = 0.0;
            var maximum = double.PositiveInfinity;
            for (var start = 0; start < count; )
            {
                // Find the first non-space cell that would overflow. Spaces
                // can hang beyond the width until the next non-space cell.
                var rejected = count;
                if (Width(start, count) > width)
                {
                    var low = start + 1;
                    var high = count;
                    while (low < high)
                    {
                        var middle = low + (high - low) / 2;
                        if (Width(start, middle) > width) high = middle;
                        else low = middle + 1;
                    }
                    rejected = points[low - 1].NextNonSpace;
                }

                var fit = points[rejected];
                minimum = Math.Max(minimum, Width(start, Math.Max(start, fit.VisibleEnd)));
                if (rejected == count)
                {
                    ranges.Add(new(start, count, false, false));
                    break;
                }
                maximum = Math.Min(maximum, Width(start, rejected + 1));
                var emergency = fit.LastBreak <= start;
                var end = emergency ? Math.Max(start + 1, rejected) : fit.LastBreak;
                ranges.Add(new(start, end, false, emergency));
                start = end;
            }
            breakWidths = new(minimum, maximum);
            return ranges;
        }

        internal ParagraphHostLineSnapshot PlaceLine(List<ShapedCell> cells, LineRange range, double top,
            out PlacedLine placement)
        {
            var visibleEnd = Math.Max(range.Start, points[range.End].VisibleEnd);
            var count = range.End - range.Start;
            var baseline = top + metrics.Ascent;
            placement = new(cells, range.Start, count, visibleEnd - range.Start, count, 0,
                0, 0, top, baseline, baseline + metrics.Descent);
            return new(cells[range.Start].Start, cells[range.End - 1].End, false, metrics.Ascent, metrics.Descent,
                metrics.Height, Width(range.Start, visibleEnd), 0, baseline);
        }

        private double Width(int start, int end) => points[end].Advance - points[start].Advance;
    }

    private readonly record struct PlacedLine(List<ShapedCell> Source, int First, int Count, int VisibleEnd,
        int GeometryCount, sbyte BaseLevel, double Expansion, double Left, double Top, double Baseline, double Bottom)
    {
        internal ReadOnlySpan<ShapedCell> Cells => CollectionsMarshal.AsSpan(Source).Slice(First, Count);
    }

    // Every accepted non-space cell still fits, and every first rejected cell
    // still overflows. That preserves the same breaks, including fallback wraps.
    private readonly record struct LineBreakWidths(double Minimum, double Maximum)
    {
        internal bool Contains(double width) =>
            width >= Minimum && (width < Maximum || double.IsPositiveInfinity(Maximum));

        internal LineBreakWidths AtOrAbove(double minimum) => new(Math.Max(Minimum, minimum), Maximum);
    }

    private readonly record struct IntrinsicWidths(double Minimum, double Maximum, double UnwrappedAdvance, long GlyphCount);
}
