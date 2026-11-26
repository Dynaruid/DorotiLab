using System.Globalization;

namespace Doroti.Ui;

public sealed record SemanticsTextRun(int Start, int End, Rect Bounds, int LineStart, int LineEnd,
    TextDirection Direction = TextDirection.ltr);

/// <summary>UTF-16 ranges from actual paragraph layout; no node-rectangle approximation.</summary>
public sealed class SemanticsTextGeometry
{
    private readonly int[] _characterBoundaries;

    public string Text { get; }
    public long Revision { get; }
    public IReadOnlyList<SemanticsTextRun> Runs { get; }
    public IReadOnlyList<int> CharacterBoundaries { get; }

    public SemanticsTextGeometry(string text, long revision, IEnumerable<SemanticsTextRun> runs)
        : this(text, revision, DefaultBoundaries(text), runs.ToArray())
    { }

    // Both arrays are owned by the geometry and can be shared with projections.
    internal SemanticsTextGeometry(string text, long revision, int[] boundaries, SemanticsTextRun[] runs)
    {
        if (runs.Any(r => r.Start < 0 || r.End < r.Start || r.End > text.Length || !r.Bounds.IsFinite
            || r.LineStart < 0 || r.LineEnd < r.LineStart || r.LineEnd > text.Length))
            throw new ArgumentException("Invalid layout text range.", nameof(runs));
        Text = text; Revision = revision;
        Runs = Array.AsReadOnly(runs);
        _characterBoundaries = boundaries;
        CharacterBoundaries = Array.AsReadOnly(boundaries);
    }

    private static int[] DefaultBoundaries(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return [.. StringInfo.ParseCombiningCharacters(text), text.Length];
    }

    public int Snap(int offset, bool forward = false)
    {
        offset = Math.Clamp(offset, 0, Text.Length);
        var index = Array.BinarySearch(_characterBoundaries, offset);
        return index >= 0 ? offset : _characterBoundaries[forward ? ~index : ~index - 1];
    }

    public IReadOnlyList<Rect> Bounds(int start, int end) => Runs.Where(r => r.End > start && r.Start < end && !r.Bounds.isEmpty)
        .GroupBy(r => (r.LineStart, r.LineEnd)).Select(line => line.Select(r => r.Bounds)
            .Aggregate((a, b) => a.expandToInclude(b))).ToArray();

    public int OffsetAtPoint(Offset point)
    {
        foreach (var run in Runs)
            if (run.Bounds.contains(point))
            {
                var trailing = point.dx >= run.Bounds.center.dx;
                if (run.Direction == TextDirection.rtl) trailing = !trailing;
                return trailing ? run.End : run.Start;
            }
        return Text.Length == 0 ? 0 : -1;
    }

    public SemanticsTextGeometry Transform(Func<Rect, Rect> transform, Rect? clip = null) => new(Text, Revision, _characterBoundaries,
        Runs.Select(run => run with { Bounds = clip is { } value ? transform(run.Bounds).intersect(value) : transform(run.Bounds) }).ToArray());

    public bool ContentEquals(SemanticsTextGeometry? other) => ReferenceEquals(this, other)
        || other is not null && Text == other.Text && Revision == other.Revision
            && _characterBoundaries.AsSpan().SequenceEqual(other._characterBoundaries) && Runs.SequenceEqual(other.Runs);

    public SemanticsTextGeometryWire ToWire() => new(Text, Revision, CharacterBoundaries.ToArray(),
        Runs.Select(r => new SemanticsTextRunWire(r.Start, r.End, r.Bounds.left, r.Bounds.top, r.Bounds.right, r.Bounds.bottom,
            r.LineStart, r.LineEnd, r.Direction == TextDirection.rtl)).ToArray());
}

public sealed record SemanticsTextGeometryWire(string text, long revision, int[] boundaries, SemanticsTextRunWire[] runs);
public sealed record SemanticsTextRunWire(int start, int end, double left, double top, double right, double bottom,
    int lineStart, int lineEnd, bool rtl);
