namespace Doroti.Graphics;

/// <summary>An integer physical-pixel rectangle produced from logical edges.</summary>
public readonly record struct PixelRect(int Left, int Top, int Right, int Bottom)
{
    public int Width => checked(Right - Left);

    public int Height => checked(Bottom - Top);

    public Size Size => new(Width, Height);
}

/// <summary>
/// The sole logical-to-physical extent policy. Leading edges round down and trailing edges
/// round up so adjacent logical coverage cannot leave a gap or clip the outer edge.
/// </summary>
public static class PixelExtentPolicy
{
    public static PixelRect ToPixelRect(Rect logicalRect, double scaleFactor)
    {
        if (!logicalRect.IsFinite)
        {
            throw new ArgumentException("Logical pixel bounds must be finite.", nameof(logicalRect));
        }
        ValidateScale(scaleFactor);
        return new(
            Floor(logicalRect.Left * scaleFactor),
            Floor(logicalRect.Top * scaleFactor),
            Ceiling(logicalRect.Right * scaleFactor),
            Ceiling(logicalRect.Bottom * scaleFactor));
    }

    public static Size ToPixelSize(Size logicalSize, double scaleFactor)
    {
        if (!logicalSize.IsFinite || logicalSize.Width < 0 || logicalSize.Height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize), "Logical size must be finite and non-negative.");
        }
        return ToPixelRect(Rect.FromLeftTopWidthHeight(0, 0, logicalSize.Width, logicalSize.Height), scaleFactor).Size;
    }

    public static Size ToLogicalSize(Size pixelSize, double scaleFactor)
    {
        if (!pixelSize.IsFinite || pixelSize.Width < 0 || pixelSize.Height < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pixelSize), "Pixel size must be finite and non-negative.");
        }
        ValidateScale(scaleFactor);
        return new(pixelSize.Width / scaleFactor, pixelSize.Height / scaleFactor);
    }

    public static Offset ToPhysicalPoint(Offset logicalPoint, double scaleFactor)
    {
        if (!logicalPoint.IsFinite)
        {
            throw new ArgumentException("Logical point must be finite.", nameof(logicalPoint));
        }
        ValidateScale(scaleFactor);
        return new(logicalPoint.X * scaleFactor, logicalPoint.Y * scaleFactor);
    }

    private static int Floor(double value) => checked((int)Math.Floor(value));

    private static int Ceiling(double value) => checked((int)Math.Ceiling(value));

    private static void ValidateScale(double scaleFactor)
    {
        if (!double.IsFinite(scaleFactor) || scaleFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleFactor), "Scale factor must be finite and positive.");
        }
    }
}
