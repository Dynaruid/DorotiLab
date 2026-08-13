namespace Doroti.Graphics;

public enum PathFillRule
{
    NonZero,
    EvenOdd,
}

/// <summary>Immutable polygonal path used by backend-neutral display lists.</summary>
public sealed class PathGeometry
{
    private readonly IReadOnlyList<Offset> _points;

    public PathGeometry(IEnumerable<Offset> points, bool isClosed = true, PathFillRule fillRule = PathFillRule.NonZero)
    {
        ArgumentNullException.ThrowIfNull(points);
        var copy = points.ToArray();
        if (copy.Length < 2)
        {
            throw new ArgumentException("A path requires at least two points.", nameof(points));
        }
        if (copy.Any(point => !point.IsFinite))
        {
            throw new ArgumentException("Path points must be finite.", nameof(points));
        }

        _points = Array.AsReadOnly(copy);
        IsClosed = isClosed;
        FillRule = fillRule;
        Bounds = new(
            copy.Min(point => point.X),
            copy.Min(point => point.Y),
            copy.Max(point => point.X),
            copy.Max(point => point.Y));
    }

    public IReadOnlyList<Offset> Points => _points;

    public bool IsClosed { get; }

    public PathFillRule FillRule { get; }

    public Rect Bounds { get; }
}
