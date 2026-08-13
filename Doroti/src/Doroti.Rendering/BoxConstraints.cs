using Doroti.Graphics;

namespace Doroti.Rendering;

public readonly record struct EdgeInsets(double Left, double Top, double Right, double Bottom)
{
    public static EdgeInsets Zero { get; } = new(0, 0, 0, 0);

    public EdgeInsets(double all)
        : this(all, all, all, all)
    {
    }

    public double Horizontal => Left + Right;

    public double Vertical => Top + Bottom;

    public bool IsFiniteAndNonNegative =>
        double.IsFinite(Left) && Left >= 0 &&
        double.IsFinite(Top) && Top >= 0 &&
        double.IsFinite(Right) && Right >= 0 &&
        double.IsFinite(Bottom) && Bottom >= 0;
}

/// <summary>Backend-neutral box constraints with Flutter-compatible normalization rules.</summary>
public readonly record struct BoxConstraints
{
    public BoxConstraints(
        double minWidth = 0,
        double maxWidth = double.PositiveInfinity,
        double minHeight = 0,
        double maxHeight = double.PositiveInfinity)
    {
        if (!double.IsFinite(minWidth) || minWidth < 0 ||
            !double.IsFinite(minHeight) || minHeight < 0 ||
            double.IsNaN(maxWidth) || double.IsNegativeInfinity(maxWidth) || maxWidth < minWidth ||
            double.IsNaN(maxHeight) || double.IsNegativeInfinity(maxHeight) || maxHeight < minHeight)
        {
            throw new ArgumentException(
                $"Constraints must be normalized and non-negative: width={minWidth}..{maxWidth}, height={minHeight}..{maxHeight}.");
        }

        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinHeight = minHeight;
        MaxHeight = maxHeight;
    }

    public double MinWidth { get; }

    public double MaxWidth { get; }

    public double MinHeight { get; }

    public double MaxHeight { get; }

    public bool HasBoundedWidth => double.IsFinite(MaxWidth);

    public bool HasBoundedHeight => double.IsFinite(MaxHeight);

    public bool HasTightWidth => MinWidth >= MaxWidth;

    public bool HasTightHeight => MinHeight >= MaxHeight;

    public bool IsTight => HasTightWidth && HasTightHeight;

    public Size Smallest => new(MinWidth, MinHeight);

    public Size Biggest => new(MaxWidth, MaxHeight);

    public static BoxConstraints Tight(Size size)
    {
        ValidateFiniteSize(size, nameof(size));
        return new(size.Width, size.Width, size.Height, size.Height);
    }

    public static BoxConstraints TightFor(double? width = null, double? height = null)
    {
        ValidateOptionalExtent(width, nameof(width));
        ValidateOptionalExtent(height, nameof(height));
        return new(width ?? 0, width ?? double.PositiveInfinity, height ?? 0, height ?? double.PositiveInfinity);
    }

    public static BoxConstraints Loose(Size size)
    {
        ValidateFiniteSize(size, nameof(size));
        return new(0, size.Width, 0, size.Height);
    }

    public Size Constrain(Size size)
    {
        if (double.IsNaN(size.Width) || double.IsNaN(size.Height))
        {
            throw new ArgumentException("A constrained size cannot contain NaN.", nameof(size));
        }

        return new(
            Math.Clamp(size.Width, MinWidth, MaxWidth),
            Math.Clamp(size.Height, MinHeight, MaxHeight));
    }

    public double ConstrainWidth(double width = double.PositiveInfinity) => Math.Clamp(width, MinWidth, MaxWidth);

    public double ConstrainHeight(double height = double.PositiveInfinity) => Math.Clamp(height, MinHeight, MaxHeight);

    public bool IsSatisfiedBy(Size size) =>
        size.IsFinite && size.Width >= MinWidth && size.Width <= MaxWidth && size.Height >= MinHeight && size.Height <= MaxHeight;

    public BoxConstraints Loosen() => new(0, MaxWidth, 0, MaxHeight);

    public BoxConstraints Deflate(EdgeInsets insets)
    {
        if (!insets.IsFiniteAndNonNegative)
        {
            throw new ArgumentException("Insets must be finite and non-negative.", nameof(insets));
        }

        var horizontal = insets.Horizontal;
        var vertical = insets.Vertical;
        return new(
            Math.Max(0, MinWidth - horizontal),
            Math.Max(0, MaxWidth - horizontal),
            Math.Max(0, MinHeight - vertical),
            Math.Max(0, MaxHeight - vertical));
    }

    public BoxConstraints Enforce(BoxConstraints other) => new(
        Math.Clamp(MinWidth, other.MinWidth, other.MaxWidth),
        Math.Clamp(MaxWidth, other.MinWidth, other.MaxWidth),
        Math.Clamp(MinHeight, other.MinHeight, other.MaxHeight),
        Math.Clamp(MaxHeight, other.MinHeight, other.MaxHeight));

    private static void ValidateFiniteSize(Size size, string parameterName)
    {
        if (!size.IsFinite || size.Width < 0 || size.Height < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Size must be finite and non-negative.");
        }
    }

    private static void ValidateOptionalExtent(double? extent, string parameterName)
    {
        if (extent is { } value && (!double.IsFinite(value) || value < 0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "A tight extent must be finite and non-negative.");
        }
    }
}
