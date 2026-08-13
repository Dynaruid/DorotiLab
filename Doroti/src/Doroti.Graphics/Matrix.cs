namespace Doroti.Graphics;

/// <summary>Immutable 4x4 column-vector transform in row-major storage order.</summary>
public readonly record struct Matrix(
    double M11,
    double M12,
    double M13,
    double M14,
    double M21,
    double M22,
    double M23,
    double M24,
    double M31,
    double M32,
    double M33,
    double M34,
    double M41,
    double M42,
    double M43,
    double M44)
{
    public static Matrix Identity { get; } = new(
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1);

    public bool IsFinite =>
        double.IsFinite(M11) && double.IsFinite(M12) && double.IsFinite(M13) && double.IsFinite(M14) &&
        double.IsFinite(M21) && double.IsFinite(M22) && double.IsFinite(M23) && double.IsFinite(M24) &&
        double.IsFinite(M31) && double.IsFinite(M32) && double.IsFinite(M33) && double.IsFinite(M34) &&
        double.IsFinite(M41) && double.IsFinite(M42) && double.IsFinite(M43) && double.IsFinite(M44);

    public static Matrix CreateTranslation(double x, double y) => Identity with { M14 = x, M24 = y };

    public static Matrix CreateScale(double x, double y) => Identity with { M11 = x, M22 = y };

    public Offset Transform(Offset point)
    {
        var x = (M11 * point.X) + (M12 * point.Y) + M14;
        var y = (M21 * point.X) + (M22 * point.Y) + M24;
        var w = (M41 * point.X) + (M42 * point.Y) + M44;
        return w is 0 or 1 ? new(x, y) : new(x / w, y / w);
    }

    public Rect TransformBounds(Rect rect)
    {
        if (!rect.IsFinite)
        {
            throw new ArgumentException("Rectangle must be finite.", nameof(rect));
        }

        var points = new[]
        {
            Transform(new(rect.Left, rect.Top)),
            Transform(new(rect.Right, rect.Top)),
            Transform(new(rect.Right, rect.Bottom)),
            Transform(new(rect.Left, rect.Bottom)),
        };
        return new(
            points.Min(point => point.X),
            points.Min(point => point.Y),
            points.Max(point => point.X),
            points.Max(point => point.Y));
    }

    public bool TryInvert(out Matrix inverse)
    {
        var values = new[,]
        {
            { M11, M12, M13, M14, 1d, 0d, 0d, 0d },
            { M21, M22, M23, M24, 0d, 1d, 0d, 0d },
            { M31, M32, M33, M34, 0d, 0d, 1d, 0d },
            { M41, M42, M43, M44, 0d, 0d, 0d, 1d },
        };
        for (var column = 0; column < 4; column++)
        {
            var pivot = column;
            for (var row = column + 1; row < 4; row++)
            {
                if (Math.Abs(values[row, column]) > Math.Abs(values[pivot, column]))
                {
                    pivot = row;
                }
            }
            if (Math.Abs(values[pivot, column]) <= double.Epsilon)
            {
                inverse = default;
                return false;
            }
            if (pivot != column)
            {
                for (var item = 0; item < 8; item++)
                {
                    (values[column, item], values[pivot, item]) = (values[pivot, item], values[column, item]);
                }
            }
            var divisor = values[column, column];
            for (var item = 0; item < 8; item++)
            {
                values[column, item] /= divisor;
            }
            for (var row = 0; row < 4; row++)
            {
                if (row == column)
                {
                    continue;
                }
                var factor = values[row, column];
                for (var item = 0; item < 8; item++)
                {
                    values[row, item] -= factor * values[column, item];
                }
            }
        }

        inverse = new(
            values[0, 4], values[0, 5], values[0, 6], values[0, 7],
            values[1, 4], values[1, 5], values[1, 6], values[1, 7],
            values[2, 4], values[2, 5], values[2, 6], values[2, 7],
            values[3, 4], values[3, 5], values[3, 6], values[3, 7]);
        return inverse.IsFinite;
    }

    public static Matrix operator *(Matrix left, Matrix right) => new(
        (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41),
        (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42),
        (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43),
        (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44),
        (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41),
        (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42),
        (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43),
        (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44),
        (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41),
        (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42),
        (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43),
        (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44),
        (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41),
        (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42),
        (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43),
        (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44));
}
