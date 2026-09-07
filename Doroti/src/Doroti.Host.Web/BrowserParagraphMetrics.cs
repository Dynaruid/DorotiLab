using Doroti.Ui;

namespace Doroti.Host.Web;

// Local UI-JS/.NET boundary only; this is not the cross-worker DisplayList.
// All geometry stays f64 and the existing UI/Raster metrics hash is unchanged.
internal static class BrowserParagraphMetrics
{
    internal static ParagraphHostLayoutSnapshot Decode(double[] values, int textLength, bool unconstrained)
    {
        if (values.Length < 16 || values[0] != 0x4454504d || values[1] != 1)
            throw new InvalidDataException("CanvasKit paragraph metrics header is invalid.");
        foreach (var value in values)
            if (!double.IsFinite(value)) throw new InvalidDataException("CanvasKit paragraph metrics must be finite.");
        int Count(int index) => Integer(values[index], int.MaxValue);
        var advancesCount = Count(12); var lineCount = Count(13); var glyphCount = Count(14); var unresolvedCount = Count(15);
        if (advancesCount != textLength ||
            16L + advancesCount + 9L * lineCount + 9L * glyphCount + unresolvedCount != values.Length)
            throw new InvalidDataException("CanvasKit paragraph metrics table lengths do not match.");
        if (unresolvedCount != 0)
            throw new InvalidDataException("CanvasKit text layout has unresolved codepoints.");
        for (var i = 2; i <= 8; i++)
            if (values[i] < 0) throw new InvalidDataException("CanvasKit paragraph dimensions must be nonnegative.");
        var exceeded = Flag(values[9]);
        static uint HashPart(double value) => value >= 0 && value <= uint.MaxValue && value == Math.Truncate(value)
            ? (uint)value : throw new InvalidDataException("CanvasKit paragraph metrics hash is invalid.");
        var hash = HashPart(values[10]) | ((ulong)HashPart(values[11]) << 32);
        var offset = 16;
        var advances = new double[advancesCount];
        for (var i = 0; i < advances.Length; i++)
        {
            var advance = values[offset++];
            if (advance < 0) throw new InvalidDataException("CanvasKit glyph advance must be nonnegative.");
            advances[i] = advance;
        }
        var lines = new ParagraphHostLineSnapshot[lineCount];
        for (var i = 0; i < lines.Length; i++, offset += 9)
        {
            var start = Integer(values[offset], textLength); var end = Integer(values[offset + 1], textLength);
            if (end < start) throw new InvalidDataException("CanvasKit paragraph line range is invalid.");
            lines[i] = new(start, end, Flag(values[offset + 2]), values[offset + 3], values[offset + 4],
                values[offset + 5], values[offset + 6], values[offset + 7], values[offset + 8]);
        }
        var glyphs = new ParagraphHostGraphemeSnapshot[glyphCount];
        for (var i = 0; i < glyphs.Length; i++, offset += 9)
        {
            var start = Integer(values[offset], textLength); var end = Integer(values[offset + 1], textLength);
            if (end <= start) throw new InvalidDataException("CanvasKit paragraph grapheme range is invalid.");
            glyphs[i] = new(start, end, values[offset + 2], values[offset + 3], values[offset + 4],
                values[offset + 5], values[offset + 6], values[offset + 7], Flag(values[offset + 8]) ? TextDirection.rtl : TextDirection.ltr);
        }
        return new(values[2], values[3], values[4], values[5], values[6], unconstrained ? values[2] : values[7],
            values[8], exceeded, hash, advances, lines, glyphs);
    }

    private static bool Flag(double value) => value switch
    {
        0 => false, 1 => true, _ => throw new InvalidDataException("CanvasKit paragraph metrics flag is invalid."),
    };
    private static int Integer(double value, int maximum) => value >= 0 && value <= maximum && value == Math.Truncate(value)
        ? (int)value : throw new InvalidDataException("CanvasKit paragraph metrics index is invalid.");
}

// Two most recently used widths per immutable paragraph. No global text/host
// retention. A font generation change clears both entries before lookup.
internal sealed class BrowserParagraphLayoutCache
{
    private (double Width, ParagraphHostLayoutSnapshot Value)? _latest, _previous;
    private long _fontGeneration = -1;
    internal ParagraphHostLayoutSnapshot GetOrCreate(double width, long fontGeneration,
        Func<ParagraphHostLayoutSnapshot> create)
    {
        if (_fontGeneration != fontGeneration)
        { _latest = _previous = null; _fontGeneration = fontGeneration; }
        if (_latest is { } latest && latest.Width == width) return latest.Value;
        if (_previous is { } previous && previous.Width == width)
        { _previous = _latest; _latest = previous; return previous.Value; }
        var value = create();
        // Large editable documents are measured normally, without retaining a
        // second set of unbounded glyph tables in this optional width cache.
        if ((long)value.CodeUnitAdvances.Count + 9L * (value.Lines.Count + value.Graphemes.Count) <= 65536)
        { _previous = _latest; _latest = (width, value); }
        return value;
    }
}
