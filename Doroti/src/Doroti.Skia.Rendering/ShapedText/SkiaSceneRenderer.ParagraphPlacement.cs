using System.Runtime.InteropServices;
using Doroti.Ui;
using SkiaSharp;
using UiDirection = Doroti.Ui.TextDirection;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaSceneRenderer
{
    private ParagraphHostLayoutSnapshot PlaceParagraph(ParagraphRequest request, ParagraphTextRun[] runs,
        ParagraphTextAnalysis analysis, List<ShapedCell> cells, double availableWidth, double width,
        VerticalMetrics emptyMetrics, IntrinsicWidths widths)
    {
        var lineRanges = BreakLines(cells, analysis.Breaks, availableWidth);
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
        var geometry = new ParagraphHostGraphemeSnapshot[lineCount == 0 ? 0 : lineRanges[lineCount - 1].End];
        var geometryCount = 0;
        var paint = new List<ShapedPaintRun>();
        using var blobBuilder = new SKTextBlobBuilder();
        using var suffixFonts = truncated && request.Ellipsis is { Length: > 0 } ? new ShapingFonts() : null;
        var top = 0.0;
        var longest = 0.0;
        for (var i = 0; i < lineCount; i++)
        {
            var range = lineRanges[i];
            var start = range.Start < cells.Count ? cells[range.Start].Start : request.Text.Length;
            var last = truncated && i == lineCount - 1;
            ReadOnlySpan<ShapedCell> lineCells = CollectionsMarshal.AsSpan(cells)[range.Start..range.End];
            if (last && request.Ellipsis is { Length: > 0 } ellipsis)
            {
                var elided = cells.GetRange(range.Start, range.End - range.Start);
                AppendEllipsis(request, runs, analysis.BaseLevel, elided, start, availableWidth, ellipsis, suffixFonts!);
                lineCells = CollectionsMarshal.AsSpan(elided);
            }

            var isSoftWrap = !last && !range.HardBreak && i + 1 < lineRanges.Count;
            var line = PlaceLine(request, analysis.BaseLevel, lineCells, start, top, width,
                i, isSoftWrap, emptyMetrics, geometry.AsSpan(geometryCount), blobBuilder, paint, out var added);
            lines[i] = line;
            geometryCount += added;
            top += line.Height;
            longest = Math.Max(longest, line.Width);
        }
        var maximumAscent = lines.Length == 0 ? emptyMetrics.Ascent : lines[0].Ascent;
        var firstHeight = lines.Length == 0 ? emptyMetrics.Height : lines[0].Height;
        if (request.Text.Length == 0) top = emptyMetrics.Height;
        return new ParagraphHostLayoutSnapshot(top, maximumAscent, firstHeight,
            widths.Minimum, widths.Maximum, longest, truncated, lines, geometry.AsMemory(0, geometryCount), analysis.WordBoundaries,
            new ShapedParagraphPaint(paint.AsReadOnly()));
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
        foreach (var cell in cells)
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

    private static List<LineRange> BreakLines(List<ShapedCell> cells, ReadOnlySpan<bool> breaks,
        double width)
    {
        var ranges = new List<LineRange>();
        for (var start = 0; start < cells.Count; )
        {
            var cursor = start;
            var lastBreak = start;
            var advance = 0.0;
            var hard = false;
            var emergency = false;
            for (; cursor < cells.Count; cursor++)
            {
                var cell = cells[cursor];
                if (cell.HardBreak)
                {
                    hard = true;
                    cursor++;
                    break;
                }
                advance += cell.Advance;
                if (advance > width && !cell.IsSpace)
                {
                    emergency = lastBreak == start;
                    cursor = emergency ? Math.Max(start + 1, cursor) : lastBreak;
                    break;
                }
                if (breaks[cell.End])
                    lastBreak = cursor + 1;
            }
            ranges.Add(new(start, cursor, hard, emergency));
            start = cursor;
        }
        if (ranges.Count != 0 && ranges[^1].HardBreak)
            ranges.Add(new(cells.Count, cells.Count, false, false));
        return ranges;
    }

    private void AppendEllipsis(ParagraphRequest request, ParagraphTextRun[] runs, sbyte level,
        List<ShapedCell> cells, int start, double width, string ellipsis, ShapingFonts fonts)
    {
        var direction = (level & 1) == 0 ? UiDirection.ltr : UiDirection.rtl;
        var style = runs.Length == 0
            ? new TextStyle(fontFamily: request.FontFamily, fontSize: request.FontSize)
            : StyleAt(runs, Math.Max(start, cells.Count == 0 ? start : cells[^1].Start));
        var suffix = ShapeParagraph(request with { Text = ellipsis }, [new ParagraphTextRun(ellipsis, style)],
            new ParagraphTextAnalysis(ellipsis, direction).Clusters, fonts, out _);
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

    private static ParagraphHostLineSnapshot PlaceLine(ParagraphRequest request, sbyte baseLevel,
        ReadOnlySpan<ShapedCell> cells, int start, double top, double width, int index, bool isSoftWrap,
        VerticalMetrics emptyMetrics, Span<ParagraphHostGraphemeSnapshot> geometry, SKTextBlobBuilder blobBuilder,
        List<ShapedPaintRun> paint, out int geometryCount)
    {
        var ascent = 0.0;
        var descent = 0.0;
        if (cells.Length == 0)
        {
            ascent = emptyMetrics.Ascent;
            descent = emptyMetrics.Descent;
        }
        Span<sbyte> levels = cells.Length <= 256 ? stackalloc sbyte[cells.Length] : new sbyte[cells.Length];
        for (var i = 0; i < cells.Length; i++)
        {
            var cell = cells[i];
            ascent = Math.Max(ascent, cell.Metrics.Ascent);
            descent = Math.Max(descent, cell.Metrics.Descent);
            levels[i] = cell.Level;
        }
        var visibleEnd = cells.Length;
        while (visibleEnd > 0 && cells[visibleEnd - 1].ResetLevel)
        {
            levels[--visibleEnd] = baseLevel;
        }

        var lineWidth = 0.0;
        var spaces = 0;
        for (var i = 0; i < visibleEnd; i++)
        {
            lineWidth += cells[i].Advance;
            if (cells[i].IsSpace) spaces++;
        }
        var justify = request.TextAlign == TextAlign.justify && isSoftWrap && spaces > 0 && width > lineWidth;
        var expansion = justify ? (width - lineWidth) / spaces : 0;
        if (justify) lineWidth = width;
        var left = ResolveLineAlignment(request.TextAlign, baseLevel) switch
        {
            TextAlign.center => (width - lineWidth) / 2,
            TextAlign.right => width - lineWidth,
            _ => 0,
        };

        Span<int> order = cells.Length <= 256 ? stackalloc int[cells.Length] : new int[cells.Length];
        ParagraphTextAnalysis.ReorderVisual(order, levels);
        Span<double> positions = cells.Length <= 256 ? stackalloc double[cells.Length] : new double[cells.Length];
        var pen = left;
        // Spaces discarded from the line width can still be selected. For
        // RTL their boxes extend to the left of the aligned content.
        for (var i = visibleEnd; i < cells.Length; i++)
            if ((levels[i] & 1) != 0) pen -= cells[i].Advance;

        var bottom = top + ascent + descent;
        geometryCount = 0;
        foreach (var logical in order)
        {
            var cell = cells[logical];
            var advance = cell.Advance + (logical < visibleEnd && cell.IsSpace ? expansion : 0);
            positions[logical] = pen;
            if (cell.Start < cell.End)
            {
                var direction = (levels[logical] & 1) == 0 ? UiDirection.ltr : UiDirection.rtl;
                // Only an ellipsis adds synthetic zero-length cells, all at
                // the end of the logical range.
                geometry[logical] = new(cell.Start, cell.End, pen, top, pen + advance, bottom,
                    top, bottom, direction, index);
                geometryCount++;
            }
            pen += advance;
        }
        var baseline = top + ascent;
        RecordGlyphs(cells, order, positions, (float)baseline, blobBuilder, paint);
        var last = cells.Length == 0 ? default : cells[^1];
        var end = cells.Length == 0 ? start : last.HardBreak ? last.Start : last.End;
        return new(start, end, last.HardBreak, ascent, descent, ascent + descent, lineWidth, left, baseline);
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
        Span<ushort> glyphBuffer = stackalloc ushort[128];
        Span<SKPoint> pointBuffer = stackalloc SKPoint[128];
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
            var buffer = builder.AllocatePositionedRun(run.Font, count);
            var glyphs = count <= glyphBuffer.Length ? glyphBuffer[..count] : new ushort[count];
            var points = count <= pointBuffer.Length ? pointBuffer[..count] : new SKPoint[count];
            var output = 0;
            for (var i = start; i < end; i++)
            {
                var logical = order[i];
                var cell = cells[logical];
                var translation = (float)positions[logical] - cell.Origin;
                for (var g = cell.GlyphStart; g < cell.GlyphStart + cell.GlyphCount; g++, output++)
                {
                    glyphs[output] = run.Glyphs[g];
                    var point = run.Positions[g];
                    points[output] = new(point.X + translation, point.Y + baseline);
                }
            }
            buffer.SetGlyphs(glyphs);
            buffer.SetPositions(points);
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

    private readonly record struct IntrinsicWidths(double Minimum, double Maximum, double UnwrappedAdvance, long GlyphCount);
}
