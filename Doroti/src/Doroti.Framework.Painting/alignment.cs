// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/alignment.dart
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

public abstract class AlignmentGeometry
{
    public static AlignmentGeometry topLeft = Alignment.topLeft;
    public static AlignmentGeometry topCenter = Alignment.topCenter;
    public static AlignmentGeometry topRight = Alignment.topRight;
    public static AlignmentGeometry topStart = AlignmentDirectional.topStart;
    public static AlignmentGeometry topEnd = AlignmentDirectional.topEnd;
    public static AlignmentGeometry centerLeft = Alignment.centerLeft;
    public static AlignmentGeometry center = Alignment.center;
    public static AlignmentGeometry centerRight = Alignment.centerRight;
    public static AlignmentGeometry centerStart = AlignmentDirectional.centerStart;
    public static AlignmentGeometry centerEnd = AlignmentDirectional.centerEnd;
    public static AlignmentGeometry bottomLeft = Alignment.bottomLeft;
    public static AlignmentGeometry bottomCenter = Alignment.bottomCenter;
    public static AlignmentGeometry bottomRight = Alignment.bottomRight;
    public static AlignmentGeometry bottomStart = AlignmentDirectional.bottomStart;
    public static AlignmentGeometry bottomEnd = AlignmentDirectional.bottomEnd;

    protected AlignmentGeometry()
    {
    }

    public static AlignmentGeometry CreateXy(double x, double y)
        => new Alignment(x, y);

    public static AlignmentGeometry CreateDirectional(double start, double y)
        => new AlignmentDirectional(start, y);

    internal abstract double _x { get; }
    internal abstract double _start { get; }
    internal abstract double _y { get; }
    public virtual AlignmentGeometry add(AlignmentGeometry other)
    {
        return new _MixedAlignment__alignment((this._x + ((AlignmentGeometry)other)._x), (this._start + ((AlignmentGeometry)other)._start), (this._y + ((AlignmentGeometry)other)._y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract AlignmentGeometry op_Subtract();
    public abstract AlignmentGeometry op_Multiply(double other);
    public abstract AlignmentGeometry op_Divide(double other);
    public abstract AlignmentGeometry ___(double other);
    public abstract AlignmentGeometry __(double other);
    public static AlignmentGeometry? lerp(AlignmentGeometry? a, AlignmentGeometry? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return (b!.op_Multiply(t));
        }
        if ((b is null))
        {
            return (a.op_Multiply(((1.0 - t))));
        }
        if (((a is Alignment) && (b is Alignment)))
        {
            Alignment a__as7939 = (Alignment)a;
            Alignment b__as7957 = (Alignment)b;
            return Alignment.lerp(((Alignment)a__as7939), ((Alignment)b__as7957), t);
        }
        if (((a is AlignmentDirectional) && (b is AlignmentDirectional)))
        {
            AlignmentDirectional a__as8027 = (AlignmentDirectional)a;
            AlignmentDirectional b__as8056 = (AlignmentDirectional)b;
            return AlignmentDirectional.lerp(((AlignmentDirectional)a__as8027), ((AlignmentDirectional)b__as8056), t);
        }
        return new _MixedAlignment__alignment(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentGeometry)a)._x, ((AlignmentGeometry)b)._x, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentGeometry)a)._start, ((AlignmentGeometry)b)._start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentGeometry)a)._y, ((AlignmentGeometry)b)._y, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Alignment resolve(TextDirection? direction);
    public override string ToString()
    {
        if ((this._start == 0.0))
        {
            return Alignment._stringify(this._x, this._y);
        }
        if ((this._x == 0.0))
        {
            return AlignmentDirectional._stringify(this._start, this._y);
        }
        return $"{Alignment._stringify(this._x, this._y)} + {AlignmentDirectional._stringify(this._start, 0.0)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AlignmentGeometry;
        if (__other is null) return false;
        return ((((__other is AlignmentGeometry) && (((AlignmentGeometry)((AlignmentGeometry)__other))._x == this._x)) && (((AlignmentGeometry)((AlignmentGeometry)__other))._start == this._start)) && (((AlignmentGeometry)((AlignmentGeometry)__other))._y == this._y));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this._x, this._start, this._y);
}

public class Alignment : AlignmentGeometry
{
    public virtual double x { get; private set; } = default!;
    public virtual double y { get; private set; } = default!;
    public static Alignment topLeft = new Alignment(-1.0, -1.0);
    public static Alignment topCenter = new Alignment(0.0, -1.0);
    public static Alignment topRight = new Alignment(1.0, -1.0);
    public static Alignment centerLeft = new Alignment(-1.0, 0.0);
    public static Alignment center = new Alignment(0.0, 0.0);
    public static Alignment centerRight = new Alignment(1.0, 0.0);
    public static Alignment bottomLeft = new Alignment(-1.0, 1.0);
    public static Alignment bottomCenter = new Alignment(0.0, 1.0);
    public static Alignment bottomRight = new Alignment(1.0, 1.0);

    public Alignment(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    internal override double _x => this.x;
    internal override double _start => 0.0;
    internal override double _y => this.y;
    public override AlignmentGeometry add(AlignmentGeometry other)
    {
        if ((other is Alignment))
        {
            Alignment other__as12977 = (Alignment)other;
            return (this.op_Add(((Alignment)other__as12977)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Alignment op_Subtract(Alignment other)
    {
        return new Alignment((this.x - ((Alignment)other).x), (this.y - ((Alignment)other).y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Alignment op_Add(Alignment other)
    {
        return new Alignment((this.x + ((Alignment)other).x), (this.y + ((Alignment)other).y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Subtract()
    {
        return new Alignment(-this.x, -this.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Multiply(double other)
    {
        return new Alignment((this.x * other), (this.y * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Divide(double other)
    {
        return new Alignment((this.x / other), (this.y / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment ___(double other)
    {
        return new Alignment(((checked((long)(this.x / other)))).toDouble(), ((checked((long)(this.y / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment __(double other)
    {
        return new Alignment((this.x % other), (this.y % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset alongOffset(Offset other)
    {
        double centerX__14331 = (other.dx / 2.0);
        double centerY__14374 = (other.dy / 2.0);
        return new global::Doroti.Ui.Offset((centerX__14331 + (this.x * centerX__14331)), (centerY__14374 + (this.y * centerY__14374)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset alongSize(Size other)
    {
        double centerX__14590 = (other.width / 2.0);
        double centerY__14636 = (other.height / 2.0);
        return new global::Doroti.Ui.Offset((centerX__14590 + (this.x * centerX__14590)), (centerY__14636 + (this.y * centerY__14636)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Offset withinRect(Rect rect)
    {
        double halfWidth__14855 = (rect.width / 2.0);
        double halfHeight__14902 = (rect.height / 2.0);
        return new global::Doroti.Ui.Offset(((rect.left + halfWidth__14855) + (this.x * halfWidth__14855)), ((rect.top + halfHeight__14902) + (this.y * halfHeight__14902)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect inscribe(Size size, Rect rect)
    {
        double halfWidthDelta__15367 = (((rect.width - size.width)) / 2.0);
        double halfHeightDelta__15434 = (((rect.height - size.height)) / 2.0);
        return global::Doroti.Ui.Rect.fromLTWH(((rect.left + halfWidthDelta__15367) + (this.x * halfWidthDelta__15367)), ((rect.top + halfHeightDelta__15434) + (this.y * halfHeightDelta__15434)), size.width, size.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Alignment? lerp(Alignment? a, Alignment? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return new Alignment(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, b!.x, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, ((Alignment)b).y, t)));
        }
        if ((b is null))
        {
            return new Alignment(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((Alignment)a).x, 0.0, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((Alignment)a).y, 0.0, t)));
        }
        return new Alignment(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((Alignment)a).x, ((Alignment)b).x, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((Alignment)a).y, ((Alignment)b).y, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction) => this;
    internal static string _stringify(double x, double y)
    {
        return ((x, y) switch { (-1.0, -1.0) => "Alignment.topLeft", (0.0, -1.0) => "Alignment.topCenter", (1.0, -1.0) => "Alignment.topRight", (-1.0, 0.0) => "Alignment.centerLeft", (0.0, 0.0) => "Alignment.center", (1.0, 0.0) => "Alignment.centerRight", (-1.0, 1.0) => "Alignment.bottomLeft", (0.0, 1.0) => "Alignment.bottomCenter", (1.0, 1.0) => "Alignment.bottomRight", _ => $"Alignment({x.toStringAsFixed(1L)}, {y.toStringAsFixed(1L)})" });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => _stringify(this.x, this.y);
}

public class AlignmentDirectional : AlignmentGeometry
{
    public virtual double start { get; private set; } = default!;
    public virtual double y { get; private set; } = default!;
    public static AlignmentDirectional topStart = new AlignmentDirectional(-1.0, -1.0);
    public static AlignmentDirectional topCenter = new AlignmentDirectional(0.0, -1.0);
    public static AlignmentDirectional topEnd = new AlignmentDirectional(1.0, -1.0);
    public static AlignmentDirectional centerStart = new AlignmentDirectional(-1.0, 0.0);
    public static AlignmentDirectional center = new AlignmentDirectional(0.0, 0.0);
    public static AlignmentDirectional centerEnd = new AlignmentDirectional(1.0, 0.0);
    public static AlignmentDirectional bottomStart = new AlignmentDirectional(-1.0, 1.0);
    public static AlignmentDirectional bottomCenter = new AlignmentDirectional(0.0, 1.0);
    public static AlignmentDirectional bottomEnd = new AlignmentDirectional(1.0, 1.0);

    public AlignmentDirectional(double start, double y)
    {
        this.start = start;
        this.y = y;
    }

    internal override double _x => 0.0;
    internal override double _start => this.start;
    internal override double _y => this.y;
    public override AlignmentGeometry add(AlignmentGeometry other)
    {
        if ((other is AlignmentDirectional))
        {
            AlignmentDirectional other__as20364 = (AlignmentDirectional)other;
            return (this.op_Add(((AlignmentDirectional)other__as20364)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AlignmentDirectional op_Subtract(AlignmentDirectional other)
    {
        return new AlignmentDirectional((this.start - ((AlignmentDirectional)other).start), (this.y - ((AlignmentDirectional)other).y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AlignmentDirectional op_Add(AlignmentDirectional other)
    {
        return new AlignmentDirectional((this.start + ((AlignmentDirectional)other).start), (this.y + ((AlignmentDirectional)other).y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Subtract()
    {
        return new AlignmentDirectional(-this.start, -this.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Multiply(double other)
    {
        return new AlignmentDirectional((this.start * other), (this.y * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Divide(double other)
    {
        return new AlignmentDirectional((this.start / other), (this.y / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional ___(double other)
    {
        return new AlignmentDirectional(((checked((long)(this.start / other)))).toDouble(), ((checked((long)(this.y / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional __(double other)
    {
        return new AlignmentDirectional((this.start % other), (this.y % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AlignmentDirectional? lerp(AlignmentDirectional? a, AlignmentDirectional? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return new AlignmentDirectional(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, b!.start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, ((AlignmentDirectional)b).y, t)));
        }
        if ((b is null))
        {
            return new AlignmentDirectional(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentDirectional)a).start, 0.0, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentDirectional)a).y, 0.0, t)));
        }
        return new AlignmentDirectional(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentDirectional)a).start, ((AlignmentDirectional)b).start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((AlignmentDirectional)a).y, ((AlignmentDirectional)b).y, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(AlignmentDirectional)}"));
        return (DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new Alignment(-this.start, this.y), TextDirection.ltr => new Alignment(this.start, this.y), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _stringify(double start, double y)
    {
        return ((start, y) switch { (-1.0, -1.0) => "AlignmentDirectional.topStart", (0.0, -1.0) => "AlignmentDirectional.topCenter", (1.0, -1.0) => "AlignmentDirectional.topEnd", (-1.0, 0.0) => "AlignmentDirectional.centerStart", (0.0, 0.0) => "AlignmentDirectional.center", (1.0, 0.0) => "AlignmentDirectional.centerEnd", (-1.0, 1.0) => "AlignmentDirectional.bottomStart", (0.0, 1.0) => "AlignmentDirectional.bottomCenter", (1.0, 1.0) => "AlignmentDirectional.bottomEnd", _ => $"AlignmentDirectional({start.toStringAsFixed(1L)}, {y.toStringAsFixed(1L)})" });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => _stringify(this.start, this.y);
}

internal class _MixedAlignment__alignment : AlignmentGeometry
{
    private double __field__x = default!;
    internal override double _x { get => __field__x; }
    private double __field__start = default!;
    internal override double _start { get => __field__start; }
    private double __field__y = default!;
    internal override double _y { get => __field__y; }

    internal _MixedAlignment__alignment(double _x, double _start, double _y)
    {
        this.__field__x = _x;
        this.__field__start = _start;
        this.__field__y = _y;
    }

    public override _MixedAlignment__alignment op_Subtract()
    {
        return new _MixedAlignment__alignment(-this._x, -this._start, -this._y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment op_Multiply(double other)
    {
        return new _MixedAlignment__alignment((this._x * other), (this._start * other), (this._y * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment op_Divide(double other)
    {
        return new _MixedAlignment__alignment((this._x / other), (this._start / other), (this._y / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment ___(double other)
    {
        return new _MixedAlignment__alignment(((checked((long)(this._x / other)))).toDouble(), ((checked((long)(this._start / other)))).toDouble(), ((checked((long)(this._y / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment __(double other)
    {
        return new _MixedAlignment__alignment((this._x % other), (this._start % other), (this._y % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(_MixedAlignment__alignment)}"));
        return (DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new Alignment((this._x - this._start), this._y), TextDirection.ltr => new Alignment((this._x + this._start), this._y), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TextAlignVertical
{
    public virtual double y { get; private set; } = default!;
    public static TextAlignVertical top = new TextAlignVertical(y: -1.0);
    public static TextAlignVertical center = new TextAlignVertical(y: 0.0);
    public static TextAlignVertical bottom = new TextAlignVertical(y: 1.0);

    public TextAlignVertical(double y)
    {
        this.y = y;
        System.Diagnostics.Debug.Assert(((y >= -1.0) && (y <= 1.0)));
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextAlignVertical"))}(y: {this.y})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

