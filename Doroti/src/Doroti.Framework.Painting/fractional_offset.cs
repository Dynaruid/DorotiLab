// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/fractional_offset.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Painting;

public class FractionalOffset : Alignment
{
    public static FractionalOffset topLeft = new FractionalOffset(0.0, 0.0);
    public static FractionalOffset topCenter = new FractionalOffset(0.5, 0.0);
    public static FractionalOffset topRight = new FractionalOffset(1.0, 0.0);
    public static FractionalOffset centerLeft = new FractionalOffset(0.0, 0.5);
    public static FractionalOffset center = new FractionalOffset(0.5, 0.5);
    public static FractionalOffset centerRight = new FractionalOffset(1.0, 0.5);
    public static FractionalOffset bottomLeft = new FractionalOffset(0.0, 1.0);
    public static FractionalOffset bottomCenter = new FractionalOffset(0.5, 1.0);
    public static FractionalOffset bottomRight = new FractionalOffset(1.0, 1.0);

    public FractionalOffset(double dx, double dy) : base(((dx * 2.0) - 1.0), ((dy * 2.0) - 1.0))
    {
    }

    public static FractionalOffset CreateFromOffsetAndSize(Offset offset, Size size)
    {
        return new FractionalOffset((offset.dx / size.width), (offset.dy / size.height));
    }

    public static FractionalOffset CreateFromOffsetAndRect(Offset offset, Rect rect)
    {
        return FractionalOffset.CreateFromOffsetAndSize((offset - rect.topLeft), rect.size);
    }

    public virtual double dx => (((x + 1.0)) / 2.0);
    public virtual double dy => (((y + 1.0)) / 2.0);
    public override Alignment op_Subtract(Alignment other)
    {
        if ((other is not FractionalOffset))
        {
            return (base.op_Subtract(other));
        }
        return new FractionalOffset((this.dx - ((FractionalOffset)((FractionalOffset)other)).dx), (this.dy - ((FractionalOffset)((FractionalOffset)other)).dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Add(Alignment other)
    {
        if ((other is not FractionalOffset))
        {
            return (base.op_Add(other));
        }
        return new FractionalOffset((this.dx + ((FractionalOffset)((FractionalOffset)other)).dx), (this.dy + ((FractionalOffset)((FractionalOffset)other)).dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FractionalOffset op_Subtract()
    {
        return new FractionalOffset(-this.dx, -this.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FractionalOffset op_Multiply(double other)
    {
        return new FractionalOffset((this.dx * other), (this.dy * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FractionalOffset op_Divide(double other)
    {
        return new FractionalOffset((this.dx / other), (this.dy / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FractionalOffset ___(double other)
    {
        return new FractionalOffset(((checked((long)(this.dx / other)))).toDouble(), ((checked((long)(this.dy / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override FractionalOffset __(double other)
    {
        return new FractionalOffset((this.dx % other), (this.dy % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static FractionalOffset? lerp(FractionalOffset? a, FractionalOffset? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return new FractionalOffset(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.5, b!.dx, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.5, ((FractionalOffset)b).dy, t)));
        }
        if ((b is null))
        {
            return new FractionalOffset(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((FractionalOffset)a).dx, 0.5, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((FractionalOffset)a).dy, 0.5, t)));
        }
        return new FractionalOffset(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((FractionalOffset)a).dx, ((FractionalOffset)b).dx, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((FractionalOffset)a).dy, ((FractionalOffset)b).dy, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"FractionalOffset({this.dx.toStringAsFixed(1L)}, " + $"{this.dy.toStringAsFixed(1L)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

