// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/edge_insets.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public abstract class EdgeInsetsGeometry
{
    public static EdgeInsetsGeometry zero = EdgeInsets.zero;
    public static EdgeInsetsGeometry infinity = new _MixedEdgeInsets__edge_insets(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);

    protected EdgeInsetsGeometry()
    {
    }

    public static EdgeInsetsGeometry CreateAll(double value)
        => EdgeInsets.CreateAll(value);

    public static EdgeInsetsGeometry CreateOnly(double left = default!, double right = default!, double top = default!, double bottom = default!)
        => EdgeInsets.CreateOnly(left, right, top, bottom);

    public static EdgeInsetsGeometry CreateDirectional(double start = default!, double end = default!, double top = default!, double bottom = default!)
        => EdgeInsetsDirectional.CreateOnly(start, end, top, bottom);

    public static EdgeInsetsGeometry CreateSymmetric(double vertical = default!, double horizontal = default!)
        => EdgeInsets.CreateSymmetric(vertical, horizontal);

    public static EdgeInsetsGeometry CreateFromLTRB(double left, double top, double right, double bottom)
        => new EdgeInsets(left, top, right, bottom);

    public static EdgeInsetsGeometry CreateFromViewPadding(ViewPadding padding, double devicePixelRatio)
        => EdgeInsets.CreateFromViewPadding(padding, devicePixelRatio);

    public static EdgeInsetsGeometry CreateFromSTEB(double start, double top, double end, double bottom)
        => new EdgeInsetsDirectional(start, top, end, bottom);

    internal abstract double _bottom { get; }
    internal abstract double _end { get; }
    internal abstract double _left { get; }
    internal abstract double _right { get; }
    internal abstract double _start { get; }
    internal abstract double _top { get; }
    public virtual bool isNonNegative
    {
        get
        {
            return (_left >= 0.0) && (_right >= 0.0) && (_start >= 0.0) && (_end >= 0.0) && (_top >= 0.0) && (_bottom >= 0.0);
        }
    }
    public virtual double horizontal => _left + _right + _start + _end;
    public virtual double vertical => _top + _bottom;
    public virtual double along(Axis axis)
    {
        return axis switch { Axis.horizontal => horizontal, Axis.vertical => vertical, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size collapsedSize => new Size(horizontal, vertical);
    public virtual EdgeInsetsGeometry flipped => new _MixedEdgeInsets__edge_insets(_right, _left, _end, _start, _bottom, _top);
    public virtual Size inflateSize(Size size)
    {
        return new Size(size.width + horizontal, size.height + vertical);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size deflateSize(Size size)
    {
        return new Size(size.width - horizontal, size.height - vertical);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        return new _MixedEdgeInsets__edge_insets(_left - other._left, _right - other._right, _start - other._start, _end - other._end, _top - other._top, _bottom - other._bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        return new _MixedEdgeInsets__edge_insets(_left + other._left, _right + other._right, _start + other._start, _end + other._end, _top + other._top, _bottom + other._bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry clamp(EdgeInsetsGeometry min, EdgeInsetsGeometry max)
    {
        return new _MixedEdgeInsets__edge_insets(Dart_uiLibrary.clampDouble(_left, min._left, max._left), Dart_uiLibrary.clampDouble(_right, min._right, max._right), Dart_uiLibrary.clampDouble(_start, min._start, max._start), Dart_uiLibrary.clampDouble(_end, min._end, max._end), Dart_uiLibrary.clampDouble(_top, min._top, max._top), Dart_uiLibrary.clampDouble(_bottom, min._bottom, max._bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract EdgeInsetsGeometry op_Subtract();
    public abstract EdgeInsetsGeometry op_Multiply(double other);
    public abstract EdgeInsetsGeometry op_Divide(double other);
    public abstract EdgeInsetsGeometry ___(double other);
    public abstract EdgeInsetsGeometry __(double other);
    public static EdgeInsetsGeometry? lerp(EdgeInsetsGeometry? a, EdgeInsetsGeometry? b, double t)
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
        if ((a is EdgeInsets) && (b is EdgeInsets))
        {
            EdgeInsets a__as10466 = (EdgeInsets)a;
            EdgeInsets b__as10485 = (EdgeInsets)b;
            return EdgeInsets.lerp(a__as10466, b__as10485, t);
        }
        if ((a is EdgeInsetsDirectional) && (b is EdgeInsetsDirectional))
        {
            EdgeInsetsDirectional a__as10557 = (EdgeInsetsDirectional)a;
            EdgeInsetsDirectional b__as10587 = (EdgeInsetsDirectional)b;
            return EdgeInsetsDirectional.lerp(a__as10557, b__as10587, t);
        }
        return new _MixedEdgeInsets__edge_insets(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._left, b._left, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._right, b._right, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._start, b._start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._end, b._end, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._top, b._top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a._bottom, b._bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract EdgeInsets resolve(TextDirection? direction);
    public override string ToString()
    {
        if ((_start == 0.0) && (_end == 0.0))
        {
            if ((_left == 0.0) && (_right == 0.0) && (_top == 0.0) && (_bottom == 0.0))
            {
                return "EdgeInsets.zero";
            }
            if ((_left == _right) && (_right == _top) && (_top == _bottom))
            {
                return $"EdgeInsets.all({_left.toStringAsFixed(1L)})";
            }
            return $"EdgeInsets({_left.toStringAsFixed(1L)}, " + $"{_top.toStringAsFixed(1L)}, " + $"{_right.toStringAsFixed(1L)}, " + $"{_bottom.toStringAsFixed(1L)})";
        }
        if ((_left == 0.0) && (_right == 0.0))
        {
            return $"EdgeInsetsDirectional({_start.toStringAsFixed(1L)}, " + $"{_top.toStringAsFixed(1L)}, " + $"{_end.toStringAsFixed(1L)}, " + $"{_bottom.toStringAsFixed(1L)})";
        }
        return $"EdgeInsets({_left.toStringAsFixed(1L)}, " + $"{_top.toStringAsFixed(1L)}, " + $"{_right.toStringAsFixed(1L)}, " + $"{_bottom.toStringAsFixed(1L)})" + " + " + $"EdgeInsetsDirectional({_start.toStringAsFixed(1L)}, " + "0.0, " + $"{_end.toStringAsFixed(1L)}, " + "0.0)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as EdgeInsetsGeometry;
        if (__other is null) return false;
        return (__other is EdgeInsetsGeometry) && (__other._left == _left) && (__other._right == _right) && (__other._start == _start) && (__other._end == _end) && (__other._top == _top) && (__other._bottom == _bottom);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(_left, _right, _start, _end, _top, _bottom);
}

public class EdgeInsets : EdgeInsetsGeometry
{
    public new static EdgeInsets zero = CreateOnly();
    public virtual double left { get; private set; } = default!;
    public virtual double top { get; private set; } = default!;
    public virtual double right { get; private set; } = default!;
    public virtual double bottom { get; private set; } = default!;

    public EdgeInsets(double left, double top, double right, double bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public new static EdgeInsets CreateAll(double value)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = value;
        __instance.top = value;
        __instance.right = value;
        __instance.bottom = value;
        return __instance;
    }

    public new static EdgeInsets CreateOnly(double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0)
    {
        var __instance = new EdgeInsets(left, top, right, bottom);
        __instance.left = left;
        __instance.top = top;
        __instance.right = right;
        __instance.bottom = bottom;
        return __instance;
    }

    public new static EdgeInsets CreateSymmetric(double vertical = 0.0, double horizontal = 0.0)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = horizontal;
        __instance.top = vertical;
        __instance.right = horizontal;
        __instance.bottom = vertical;
        return __instance;
    }

    public new static EdgeInsets CreateFromViewPadding(ViewPadding padding, double devicePixelRatio)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = padding.left / devicePixelRatio;
        __instance.top = padding.top / devicePixelRatio;
        __instance.right = padding.right / devicePixelRatio;
        __instance.bottom = padding.bottom / devicePixelRatio;
        return __instance;
    }

    public static EdgeInsets CreateFromWindowPadding(ViewPadding padding, double devicePixelRatio)
        => CreateFromViewPadding(padding, devicePixelRatio);

    internal override double _left => left;
    internal override double _top => top;
    internal override double _right => right;
    internal override double _bottom => bottom;
    internal override double _start => 0.0;
    internal override double _end => 0.0;
    public virtual Offset topLeft => new Offset(left, top);
    public virtual Offset topRight => new Offset(-right, top);
    public virtual Offset bottomLeft => new Offset(left, -bottom);
    public virtual Offset bottomRight => new Offset(-right, -bottom);
    public override EdgeInsets flipped => new EdgeInsets(right, bottom, left, top);
    public virtual Rect inflateRect(Rect rect)
    {
        return Rect.fromLTRB(rect.left - left, rect.top - top, rect.right + right, rect.bottom + bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect deflateRect(Rect rect)
    {
        return Rect.fromLTRB(rect.left + left, rect.top + top, rect.right - right, rect.bottom - bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RRect inflateRRect(RRect rect)
    {
        return RRect.fromLTRBAndCorners(rect.left - left, rect.top - top, rect.right + right, rect.bottom + bottom, topLeft: (rect.tlRadius + Radius.elliptical(left, top)).clamp(minimum: Radius.zero), topRight: (rect.trRadius + Radius.elliptical(right, top)).clamp(minimum: Radius.zero), bottomRight: (rect.brRadius + Radius.elliptical(right, bottom)).clamp(minimum: Radius.zero), bottomLeft: (rect.blRadius + Radius.elliptical(left, bottom)).clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RRect deflateRRect(RRect rect)
    {
        return RRect.fromLTRBAndCorners(rect.left + left, rect.top + top, rect.right - right, rect.bottom - bottom, topLeft: (rect.tlRadius - Radius.elliptical(left, top)).clamp(minimum: Radius.zero), topRight: (rect.trRadius - Radius.elliptical(right, top)).clamp(minimum: Radius.zero), bottomRight: (rect.brRadius - Radius.elliptical(right, bottom)).clamp(minimum: Radius.zero), bottomLeft: (rect.blRadius - Radius.elliptical(left, bottom)).clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        if (other is EdgeInsets)
        {
            EdgeInsets other__as22836 = (EdgeInsets)other;
            return op_Subtract(other__as22836);
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        if (other is EdgeInsets)
        {
            EdgeInsets other__as23004 = (EdgeInsets)other;
            return op_Add(other__as23004);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry clamp(EdgeInsetsGeometry min, EdgeInsetsGeometry max)
    {
        return new EdgeInsets(Dart_uiLibrary.clampDouble(_left, min._left, max._left), Dart_uiLibrary.clampDouble(_top, min._top, max._top), Dart_uiLibrary.clampDouble(_right, min._right, max._right), Dart_uiLibrary.clampDouble(_bottom, min._bottom, max._bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsets op_Subtract(EdgeInsets other)
    {
        return new EdgeInsets(left - other.left, top - other.top, right - other.right, bottom - other.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsets op_Add(EdgeInsets other)
    {
        return new EdgeInsets(left + other.left, top + other.top, right + other.right, bottom + other.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Subtract()
    {
        return new EdgeInsets(-left, -top, -right, -bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Multiply(double other)
    {
        return new EdgeInsets(left * other, top * other, right * other, bottom * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Divide(double other)
    {
        return new EdgeInsets(left / other, top / other, right / other, bottom / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets ___(double other)
    {
        return new EdgeInsets(checked((long)(left / other)).toDouble(), checked((long)(top / other)).toDouble(), checked((long)(right / other)).toDouble(), checked((long)(bottom / other)).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets __(double other)
    {
        return new EdgeInsets(left % other, top % other, right % other, bottom % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static EdgeInsets? lerp(EdgeInsets? a, EdgeInsets? b, double t)
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
        return new EdgeInsets(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.left, b.left, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.top, b.top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.right, b.right, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.bottom, b.bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction) => this;
    public virtual EdgeInsets copyWith(double? left = null, double? top = null, double? right = null, double? bottom = null)
    {
        return CreateOnly(left: left ?? this.left, top: top ?? this.top, right: right ?? this.right, bottom: bottom ?? this.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EdgeInsetsDirectional : EdgeInsetsGeometry
{
    public new static EdgeInsetsDirectional zero = CreateOnly();
    public virtual double start { get; private set; } = default!;
    public virtual double top { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;
    public virtual double bottom { get; private set; } = default!;

    public EdgeInsetsDirectional(double start, double top, double end, double bottom)
    {
        this.start = start;
        this.top = top;
        this.end = end;
        this.bottom = bottom;
    }

    public new static EdgeInsetsDirectional CreateOnly(double start = 0.0, double top = 0.0, double end = 0.0, double bottom = 0.0)
    {
        var __instance = new EdgeInsetsDirectional(start, top, end, bottom);
        __instance.start = start;
        __instance.top = top;
        __instance.end = end;
        __instance.bottom = bottom;
        return __instance;
    }

    public new static EdgeInsetsDirectional CreateSymmetric(double horizontal = 0.0, double vertical = 0.0)
    {
        var __instance = new EdgeInsetsDirectional(default!, default!, default!, default!);
        __instance.start = horizontal;
        __instance.end = horizontal;
        __instance.top = vertical;
        __instance.bottom = vertical;
        return __instance;
    }

    public new static EdgeInsetsDirectional CreateAll(double value)
    {
        var __instance = new EdgeInsetsDirectional(default!, default!, default!, default!);
        __instance.start = value;
        __instance.top = value;
        __instance.end = value;
        __instance.bottom = value;
        return __instance;
    }

    internal override double _start => start;
    internal override double _top => top;
    internal override double _end => end;
    internal override double _bottom => bottom;
    internal override double _left => 0.0;
    internal override double _right => 0.0;
    public override bool isNonNegative => (start >= 0.0) && (top >= 0.0) && (end >= 0.0) && (bottom >= 0.0);
    public override EdgeInsetsDirectional flipped => new EdgeInsetsDirectional(end, bottom, start, top);
    public override EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        if (other is EdgeInsetsDirectional)
        {
            EdgeInsetsDirectional other__as29924 = (EdgeInsetsDirectional)other;
            return op_Subtract(other__as29924);
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        if (other is EdgeInsetsDirectional)
        {
            EdgeInsetsDirectional other__as30103 = (EdgeInsetsDirectional)other;
            return op_Add(other__as30103);
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional op_Subtract(EdgeInsetsDirectional other)
    {
        return new EdgeInsetsDirectional(start - other.start, top - other.top, end - other.end, bottom - other.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional op_Add(EdgeInsetsDirectional other)
    {
        return new EdgeInsetsDirectional(start + other.start, top + other.top, end + other.end, bottom + other.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Subtract()
    {
        return new EdgeInsetsDirectional(-start, -top, -end, -bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Multiply(double other)
    {
        return new EdgeInsetsDirectional(start * other, top * other, end * other, bottom * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Divide(double other)
    {
        return new EdgeInsetsDirectional(start / other, top / other, end / other, bottom / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional ___(double other)
    {
        return new EdgeInsetsDirectional(checked((long)(start / other)).toDouble(), checked((long)(top / other)).toDouble(), checked((long)(end / other)).toDouble(), checked((long)(bottom / other)).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional __(double other)
    {
        return new EdgeInsetsDirectional(start % other, top % other, end % other, bottom % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static EdgeInsetsDirectional? lerp(EdgeInsetsDirectional? a, EdgeInsetsDirectional? b, double t)
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
        return new EdgeInsetsDirectional(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.start, b.start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.top, b.top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.end, b.end, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a.bottom, b.bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(EdgeInsetsDirectional)}"));
        return DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new EdgeInsets(end, top, start, bottom), TextDirection.ltr => new EdgeInsets(start, top, end, bottom), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional copyWith(double? start = null, double? top = null, double? end = null, double? bottom = null)
    {
        return CreateOnly(start: start ?? this.start, top: top ?? this.top, end: end ?? this.end, bottom: bottom ?? this.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MixedEdgeInsets__edge_insets : EdgeInsetsGeometry
{
    private double __field__left = default!;
    internal override double _left { get => __field__left; }
    private double __field__right = default!;
    internal override double _right { get => __field__right; }
    private double __field__start = default!;
    internal override double _start { get => __field__start; }
    private double __field__end = default!;
    internal override double _end { get => __field__end; }
    private double __field__top = default!;
    internal override double _top { get => __field__top; }
    private double __field__bottom = default!;
    internal override double _bottom { get => __field__bottom; }

    internal _MixedEdgeInsets__edge_insets(double _left, double _right, double _start, double _end, double _top, double _bottom)
    {
        __field__left = _left;
        __field__right = _right;
        __field__start = _start;
        __field__end = _end;
        __field__top = _top;
        __field__bottom = _bottom;
    }

    public override bool isNonNegative
    {
        get
        {
            return (_left >= 0.0) && (_right >= 0.0) && (_start >= 0.0) && (_end >= 0.0) && (_top >= 0.0) && (_bottom >= 0.0);
        }
    }
    public override _MixedEdgeInsets__edge_insets op_Subtract()
    {
        return new _MixedEdgeInsets__edge_insets(-_left, -_right, -_start, -_end, -_top, -_bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets op_Multiply(double other)
    {
        return new _MixedEdgeInsets__edge_insets(_left * other, _right * other, _start * other, _end * other, _top * other, _bottom * other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets op_Divide(double other)
    {
        return new _MixedEdgeInsets__edge_insets(_left / other, _right / other, _start / other, _end / other, _top / other, _bottom / other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets ___(double other)
    {
        return new _MixedEdgeInsets__edge_insets(checked((long)(_left / other)).toDouble(), checked((long)(_right / other)).toDouble(), checked((long)(_start / other)).toDouble(), checked((long)(_end / other)).toDouble(), checked((long)(_top / other)).toDouble(), checked((long)(_bottom / other)).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets __(double other)
    {
        return new _MixedEdgeInsets__edge_insets(_left % other, _right % other, _start % other, _end % other, _top % other, _bottom % other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(_MixedEdgeInsets__edge_insets)}"));
        return DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new EdgeInsets(_end + _left, _top, _start + _right, _bottom), TextDirection.ltr => new EdgeInsets(_start + _left, _top, _end + _right, _bottom), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

