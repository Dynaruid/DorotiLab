namespace Doroti.Graphics;

public readonly record struct Size(double Width, double Height)
{
    public static Size Zero { get; } = new(0, 0);

    public bool IsFinite => double.IsFinite(Width) && double.IsFinite(Height);

    public bool IsEmpty => Width <= 0 || Height <= 0;
}

public readonly record struct Offset(double X, double Y)
{
    public static Offset Zero { get; } = new(0, 0);

    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Y);

    public static Offset operator +(Offset left, Offset right) => new(left.X + right.X, left.Y + right.Y);

    public static Offset operator -(Offset left, Offset right) => new(left.X - right.X, left.Y - right.Y);
}

public readonly record struct Rect(double Left, double Top, double Right, double Bottom)
{
    public static Rect Zero { get; } = new(0, 0, 0, 0);

    public double Width => Right - Left;

    public double Height => Bottom - Top;

    public Size Size => new(Width, Height);

    public bool IsFinite =>
        double.IsFinite(Left) && double.IsFinite(Top) && double.IsFinite(Right) && double.IsFinite(Bottom);

    public bool IsEmpty => Left >= Right || Top >= Bottom;

    public static Rect FromLeftTopWidthHeight(double left, double top, double width, double height) =>
        new(left, top, left + width, top + height);

    public bool Contains(Offset point) =>
        point.X >= Left && point.X < Right && point.Y >= Top && point.Y < Bottom;

    public Rect Intersect(Rect other) => new(
        Math.Max(Left, other.Left),
        Math.Max(Top, other.Top),
        Math.Min(Right, other.Right),
        Math.Min(Bottom, other.Bottom));

    public Rect ExpandToInclude(Rect other)
    {
        if (IsEmpty)
        {
            return other;
        }
        if (other.IsEmpty)
        {
            return this;
        }
        return new(
            Math.Min(Left, other.Left),
            Math.Min(Top, other.Top),
            Math.Max(Right, other.Right),
            Math.Max(Bottom, other.Bottom));
    }
}
