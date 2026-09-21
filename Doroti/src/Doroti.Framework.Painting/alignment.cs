// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/alignment.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    protected AlignmentGeometry() { }

    public static AlignmentGeometry CreateXy(double x, double y) => new Alignment(x, y);

    public static AlignmentGeometry CreateDirectional(double start, double y) =>
        new AlignmentDirectional(start, y);

    internal abstract double _x { get; }
    internal abstract double _start { get; }
    internal abstract double _y { get; }

    public virtual AlignmentGeometry add(AlignmentGeometry other)
    {
        return new _MixedAlignment__alignment(_x + other._x, _start + other._start, _y + other._y);
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
        if (a is null)
        {
            return b!.op_Multiply(t);
        }
        if (b is null)
        {
            return a.op_Multiply(1.0 - t);
        }
        if ((a is Alignment) && (b is Alignment))
        {
            Alignment a__as7939 = (Alignment)a;
            Alignment b__as7957 = (Alignment)b;
            return Alignment.lerp(a__as7939, b__as7957, t);
        }
        if ((a is AlignmentDirectional) && (b is AlignmentDirectional))
        {
            AlignmentDirectional a__as8027 = (AlignmentDirectional)a;
            AlignmentDirectional b__as8056 = (AlignmentDirectional)b;
            return AlignmentDirectional.lerp(a__as8027, b__as8056, t);
        }
        return new _MixedAlignment__alignment(
            (
                Dart_uiLibrary.lerpDouble(a._x, b._x, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            (
                Dart_uiLibrary.lerpDouble(a._start, b._start, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            (
                Dart_uiLibrary.lerpDouble(a._y, b._y, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Alignment resolve(TextDirection? direction);

    public override string ToString()
    {
        if (_start == 0.0)
        {
            return Alignment._stringify(_x, _y);
        }
        if (_x == 0.0)
        {
            return AlignmentDirectional._stringify(_start, _y);
        }
        return $"{Alignment._stringify(_x, _y)} + {AlignmentDirectional._stringify(_start, 0.0)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as AlignmentGeometry;
        if (__other is null)
        {
            return false;
        }

        return (__other is AlignmentGeometry)
            && (__other._x == _x)
            && (__other._start == _start)
            && (__other._y == _y);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(_x, _start, _y);
}

public class Alignment : AlignmentGeometry
{
    public virtual double x { get; private set; } = default!;
    public virtual double y { get; private set; } = default!;
    public static new Alignment topLeft = new Alignment(-1.0, -1.0);
    public static new Alignment topCenter = new Alignment(0.0, -1.0);
    public static new Alignment topRight = new Alignment(1.0, -1.0);
    public static new Alignment centerLeft = new Alignment(-1.0, 0.0);
    public static new Alignment center = new Alignment(0.0, 0.0);
    public static new Alignment centerRight = new Alignment(1.0, 0.0);
    public static new Alignment bottomLeft = new Alignment(-1.0, 1.0);
    public static new Alignment bottomCenter = new Alignment(0.0, 1.0);
    public static new Alignment bottomRight = new Alignment(1.0, 1.0);

    public Alignment(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    internal override double _x => x;
    internal override double _start => 0.0;
    internal override double _y => y;

    public override AlignmentGeometry add(AlignmentGeometry other)
    {
        if (other is Alignment)
        {
            Alignment other__as12977 = (Alignment)other;
            return op_Add(other__as12977);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Alignment op_Subtract(Alignment other)
    {
        return new Alignment(x - other.x, y - other.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Alignment op_Add(Alignment other)
    {
        return new Alignment(x + other.x, y + other.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Subtract()
    {
        return new Alignment(-x, -y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Multiply(double other)
    {
        return new Alignment(x * other, y * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment op_Divide(double other)
    {
        return new Alignment(x / other, y / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment ___(double other)
    {
        return new Alignment(
            checked((long)(x / other)).toDouble(),
            checked((long)(y / other)).toDouble()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment __(double other)
    {
        return new Alignment(x % other, y % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset alongOffset(Offset other)
    {
        double centerX = other.dx / 2.0;
        double centerY = other.dy / 2.0;
        return new Offset(centerX + (x * centerX), centerY + (y * centerY));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset alongSize(Size other)
    {
        double centerX = other.width / 2.0;
        double centerY = other.height / 2.0;
        return new Offset(centerX + (x * centerX), centerY + (y * centerY));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Offset withinRect(Rect rect)
    {
        double halfWidth = rect.width / 2.0;
        double halfHeight = rect.height / 2.0;
        return new Offset(
            rect.left + halfWidth + (x * halfWidth),
            rect.top + halfHeight + (y * halfHeight)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect inscribe(Size size, Rect rect)
    {
        double halfWidthDelta = (rect.width - size.width) / 2.0;
        double halfHeightDelta = (rect.height - size.height) / 2.0;
        return Rect.fromLTWH(
            rect.left + halfWidthDelta + (x * halfWidthDelta),
            rect.top + halfHeightDelta + (y * halfHeightDelta),
            size.width,
            size.height
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Alignment? lerp(Alignment? a, Alignment? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return new Alignment(
                (
                    Dart_uiLibrary.lerpDouble(0.0, b!.x, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                (
                    Dart_uiLibrary.lerpDouble(0.0, b.y, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        if (b is null)
        {
            return new Alignment(
                (
                    Dart_uiLibrary.lerpDouble(a.x, 0.0, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                (
                    Dart_uiLibrary.lerpDouble(a.y, 0.0, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        return new Alignment(
            (
                Dart_uiLibrary.lerpDouble(a.x, b.x, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            (
                Dart_uiLibrary.lerpDouble(a.y, b.y, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction) => this;

    internal static string _stringify(double x, double y)
    {
        return (x, y) switch
        {
            (-1.0, -1.0) => "Alignment.topLeft",
            (0.0, -1.0) => "Alignment.topCenter",
            (1.0, -1.0) => "Alignment.topRight",
            (-1.0, 0.0) => "Alignment.centerLeft",
            (0.0, 0.0) => "Alignment.center",
            (1.0, 0.0) => "Alignment.centerRight",
            (-1.0, 1.0) => "Alignment.bottomLeft",
            (0.0, 1.0) => "Alignment.bottomCenter",
            (1.0, 1.0) => "Alignment.bottomRight",
            _ => $"Alignment({x.toStringAsFixed(1L)}, {y.toStringAsFixed(1L)})",
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => _stringify(x, y);
}

public class AlignmentDirectional : AlignmentGeometry
{
    public virtual double start { get; private set; } = default!;
    public virtual double y { get; private set; } = default!;
    public static new AlignmentDirectional topStart = new AlignmentDirectional(-1.0, -1.0);
    public static new AlignmentDirectional topCenter = new AlignmentDirectional(0.0, -1.0);
    public static new AlignmentDirectional topEnd = new AlignmentDirectional(1.0, -1.0);
    public static new AlignmentDirectional centerStart = new AlignmentDirectional(-1.0, 0.0);
    public static new AlignmentDirectional center = new AlignmentDirectional(0.0, 0.0);
    public static new AlignmentDirectional centerEnd = new AlignmentDirectional(1.0, 0.0);
    public static new AlignmentDirectional bottomStart = new AlignmentDirectional(-1.0, 1.0);
    public static new AlignmentDirectional bottomCenter = new AlignmentDirectional(0.0, 1.0);
    public static new AlignmentDirectional bottomEnd = new AlignmentDirectional(1.0, 1.0);

    public AlignmentDirectional(double start, double y)
    {
        this.start = start;
        this.y = y;
    }

    internal override double _x => 0.0;
    internal override double _start => start;
    internal override double _y => y;

    public override AlignmentGeometry add(AlignmentGeometry other)
    {
        if (other is AlignmentDirectional)
        {
            AlignmentDirectional other__as20364 = (AlignmentDirectional)other;
            return op_Add(other__as20364);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AlignmentDirectional op_Subtract(AlignmentDirectional other)
    {
        return new AlignmentDirectional(start - other.start, y - other.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual AlignmentDirectional op_Add(AlignmentDirectional other)
    {
        return new AlignmentDirectional(start + other.start, y + other.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Subtract()
    {
        return new AlignmentDirectional(-start, -y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Multiply(double other)
    {
        return new AlignmentDirectional(start * other, y * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional op_Divide(double other)
    {
        return new AlignmentDirectional(start / other, y / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional ___(double other)
    {
        return new AlignmentDirectional(
            checked((long)(start / other)).toDouble(),
            checked((long)(y / other)).toDouble()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AlignmentDirectional __(double other)
    {
        return new AlignmentDirectional(start % other, y % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AlignmentDirectional? lerp(
        AlignmentDirectional? a,
        AlignmentDirectional? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return new AlignmentDirectional(
                (
                    Dart_uiLibrary.lerpDouble(0.0, b!.start, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                (
                    Dart_uiLibrary.lerpDouble(0.0, b.y, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        if (b is null)
        {
            return new AlignmentDirectional(
                (
                    Dart_uiLibrary.lerpDouble(a.start, 0.0, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                (
                    Dart_uiLibrary.lerpDouble(a.y, 0.0, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        return new AlignmentDirectional(
            (
                Dart_uiLibrary.lerpDouble(a.start, b.start, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            (
                Dart_uiLibrary.lerpDouble(a.y, b.y, t)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckCanResolveTextDirection(
                direction,
                $"{typeof(AlignmentDirectional)}"
            )
        );
        return (
            direction
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        ) switch
        {
            TextDirection.rtl => new Alignment(-start, y),
            TextDirection.ltr => new Alignment(start, y),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string _stringify(double start, double y)
    {
        return (start, y) switch
        {
            (-1.0, -1.0) => "AlignmentDirectional.topStart",
            (0.0, -1.0) => "AlignmentDirectional.topCenter",
            (1.0, -1.0) => "AlignmentDirectional.topEnd",
            (-1.0, 0.0) => "AlignmentDirectional.centerStart",
            (0.0, 0.0) => "AlignmentDirectional.center",
            (1.0, 0.0) => "AlignmentDirectional.centerEnd",
            (-1.0, 1.0) => "AlignmentDirectional.bottomStart",
            (0.0, 1.0) => "AlignmentDirectional.bottomCenter",
            (1.0, 1.0) => "AlignmentDirectional.bottomEnd",
            _ => $"AlignmentDirectional({start.toStringAsFixed(1L)}, {y.toStringAsFixed(1L)})",
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => _stringify(start, y);
}

internal class _MixedAlignment__alignment : AlignmentGeometry
{
    private double __field__x = default!;
    internal override double _x
    {
        get => __field__x;
    }
    private double __field__start = default!;
    internal override double _start
    {
        get => __field__start;
    }
    private double __field__y = default!;
    internal override double _y
    {
        get => __field__y;
    }

    internal _MixedAlignment__alignment(double _x, double _start, double _y)
    {
        __field__x = _x;
        __field__start = _start;
        __field__y = _y;
    }

    public override _MixedAlignment__alignment op_Subtract()
    {
        return new _MixedAlignment__alignment(-_x, -_start, -_y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment op_Multiply(double other)
    {
        return new _MixedAlignment__alignment(_x * other, _start * other, _y * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment op_Divide(double other)
    {
        return new _MixedAlignment__alignment(_x / other, _start / other, _y / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment ___(double other)
    {
        return new _MixedAlignment__alignment(
            checked((long)(_x / other)).toDouble(),
            checked((long)(_start / other)).toDouble(),
            checked((long)(_y / other)).toDouble()
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedAlignment__alignment __(double other)
    {
        return new _MixedAlignment__alignment(_x % other, _start % other, _y % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Alignment resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckCanResolveTextDirection(
                direction,
                $"{typeof(_MixedAlignment__alignment)}"
            )
        );
        return (
            direction
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        ) switch
        {
            TextDirection.rtl => new Alignment(_x - _start, _y),
            TextDirection.ltr => new Alignment(_x + _start, _y),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
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
        System.Diagnostics.Debug.Assert((y >= -1.0) && (y <= 1.0));
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "TextAlignVertical")}(y: {y})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
