// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/fractional_offset.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class FractionalOffset : Alignment
{
    public static new FractionalOffset topLeft = new FractionalOffset(0.0, 0.0);
    public static new FractionalOffset topCenter = new FractionalOffset(0.5, 0.0);
    public static new FractionalOffset topRight = new FractionalOffset(1.0, 0.0);
    public static new FractionalOffset centerLeft = new FractionalOffset(0.0, 0.5);
    public static new FractionalOffset center = new FractionalOffset(0.5, 0.5);
    public static new FractionalOffset centerRight = new FractionalOffset(1.0, 0.5);
    public static new FractionalOffset bottomLeft = new FractionalOffset(0.0, 1.0);
    public static new FractionalOffset bottomCenter = new FractionalOffset(0.5, 1.0);
    public static new FractionalOffset bottomRight = new FractionalOffset(1.0, 1.0);

    public FractionalOffset(double dx, double dy)
        : base((dx * 2.0) - 1.0, (dy * 2.0) - 1.0) { }

    public static FractionalOffset CreateFromOffsetAndSize(Offset offset, Size size)
    {
        return new FractionalOffset(offset.dx / size.width, offset.dy / size.height);
    }

    public static FractionalOffset CreateFromOffsetAndRect(Offset offset, Rect rect)
    {
        return CreateFromOffsetAndSize(offset - rect.topLeft, rect.size);
    }

    public virtual double dx => (x + 1.0) / 2.0;
    public virtual double dy => (y + 1.0) / 2.0;

    public override Alignment op_Subtract(Alignment other)
    {
        if (other is not FractionalOffset)
        {
            return base.op_Subtract(other);
        }
        return new FractionalOffset(
            dx - ((FractionalOffset)other).dx,
            dy - ((FractionalOffset)other).dy
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Alignment op_Add(Alignment other)
    {
        if (other is not FractionalOffset)
        {
            return base.op_Add(other);
        }
        return new FractionalOffset(
            dx + ((FractionalOffset)other).dx,
            dy + ((FractionalOffset)other).dy
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FractionalOffset op_Subtract()
    {
        return new FractionalOffset(-dx, -dy);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FractionalOffset op_Multiply(double other)
    {
        return new FractionalOffset(dx * other, dy * other);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FractionalOffset op_Divide(double other)
    {
        return new FractionalOffset(dx / other, dy / other);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FractionalOffset ___(double other)
    {
        return new FractionalOffset(
            checked((long)(dx / other)).toDouble(),
            checked((long)(dy / other)).toDouble()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override FractionalOffset __(double other)
    {
        return new FractionalOffset(dx % other, dy % other);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static FractionalOffset? lerp(FractionalOffset? a, FractionalOffset? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return new FractionalOffset(
                (
                    DorotiUiLibrary.lerpDouble(0.5, b!.dx, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                (
                    DorotiUiLibrary.lerpDouble(0.5, b.dy, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        if (b is null)
        {
            return new FractionalOffset(
                (
                    DorotiUiLibrary.lerpDouble(a.dx, 0.5, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                (
                    DorotiUiLibrary.lerpDouble(a.dy, 0.5, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        return new FractionalOffset(
            (
                DorotiUiLibrary.lerpDouble(a.dx, b.dx, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ),
            (
                DorotiUiLibrary.lerpDouble(a.dy, b.dy, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString()
    {
        return $"FractionalOffset({dx.toStringAsFixed(1L)}, " + $"{dy.toStringAsFixed(1L)})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
