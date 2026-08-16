// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/edge_insets.dart
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
            return ((((((this._left >= 0.0) && (this._right >= 0.0)) && (this._start >= 0.0)) && (this._end >= 0.0)) && (this._top >= 0.0)) && (this._bottom >= 0.0));
            return default!;
        }
    }
    public virtual double horizontal => (((this._left + this._right) + this._start) + this._end);
    public virtual double vertical => (this._top + this._bottom);
    public virtual double along(Axis axis)
    {
        return (axis switch { Axis.horizontal => this.horizontal, Axis.vertical => this.vertical, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size collapsedSize => new global::Doroti.Ui.Size(this.horizontal, this.vertical);
    public virtual EdgeInsetsGeometry flipped => new _MixedEdgeInsets__edge_insets(this._right, this._left, this._end, this._start, this._bottom, this._top);
    public virtual global::Doroti.Ui.Size inflateSize(Size size)
    {
        return new global::Doroti.Ui.Size((size.width + this.horizontal), (size.height + this.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Size deflateSize(Size size)
    {
        return new global::Doroti.Ui.Size((size.width - this.horizontal), (size.height - this.vertical));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        return new _MixedEdgeInsets__edge_insets((this._left - ((EdgeInsetsGeometry)other)._left), (this._right - ((EdgeInsetsGeometry)other)._right), (this._start - ((EdgeInsetsGeometry)other)._start), (this._end - ((EdgeInsetsGeometry)other)._end), (this._top - ((EdgeInsetsGeometry)other)._top), (this._bottom - ((EdgeInsetsGeometry)other)._bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        return new _MixedEdgeInsets__edge_insets((this._left + ((EdgeInsetsGeometry)other)._left), (this._right + ((EdgeInsetsGeometry)other)._right), (this._start + ((EdgeInsetsGeometry)other)._start), (this._end + ((EdgeInsetsGeometry)other)._end), (this._top + ((EdgeInsetsGeometry)other)._top), (this._bottom + ((EdgeInsetsGeometry)other)._bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsGeometry clamp(EdgeInsetsGeometry min, EdgeInsetsGeometry max)
    {
        return new _MixedEdgeInsets__edge_insets(Dart_uiLibrary.clampDouble(this._left, ((EdgeInsetsGeometry)min)._left, ((EdgeInsetsGeometry)max)._left), Dart_uiLibrary.clampDouble(this._right, ((EdgeInsetsGeometry)min)._right, ((EdgeInsetsGeometry)max)._right), Dart_uiLibrary.clampDouble(this._start, ((EdgeInsetsGeometry)min)._start, ((EdgeInsetsGeometry)max)._start), Dart_uiLibrary.clampDouble(this._end, ((EdgeInsetsGeometry)min)._end, ((EdgeInsetsGeometry)max)._end), Dart_uiLibrary.clampDouble(this._top, ((EdgeInsetsGeometry)min)._top, ((EdgeInsetsGeometry)max)._top), Dart_uiLibrary.clampDouble(this._bottom, ((EdgeInsetsGeometry)min)._bottom, ((EdgeInsetsGeometry)max)._bottom));
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
        if ((a is null))
        {
            return (b!.op_Multiply(t));
        }
        if ((b is null))
        {
            return (a.op_Multiply(((1.0 - t))));
        }
        if (((a is EdgeInsets) && (b is EdgeInsets)))
        {
            EdgeInsets a__as10466 = (EdgeInsets)a;
            EdgeInsets b__as10485 = (EdgeInsets)b;
            return EdgeInsets.lerp(((EdgeInsets)a__as10466), ((EdgeInsets)b__as10485), t);
        }
        if (((a is EdgeInsetsDirectional) && (b is EdgeInsetsDirectional)))
        {
            EdgeInsetsDirectional a__as10557 = (EdgeInsetsDirectional)a;
            EdgeInsetsDirectional b__as10587 = (EdgeInsetsDirectional)b;
            return EdgeInsetsDirectional.lerp(((EdgeInsetsDirectional)a__as10557), ((EdgeInsetsDirectional)b__as10587), t);
        }
        return new _MixedEdgeInsets__edge_insets(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._left, ((EdgeInsetsGeometry)b)._left, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._right, ((EdgeInsetsGeometry)b)._right, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._start, ((EdgeInsetsGeometry)b)._start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._end, ((EdgeInsetsGeometry)b)._end, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._top, ((EdgeInsetsGeometry)b)._top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsGeometry)a)._bottom, ((EdgeInsetsGeometry)b)._bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract EdgeInsets resolve(TextDirection? direction);
    public override string ToString()
    {
        if (((this._start == 0.0) && (this._end == 0.0)))
        {
            if (((((this._left == 0.0) && (this._right == 0.0)) && (this._top == 0.0)) && (this._bottom == 0.0)))
            {
                return "EdgeInsets.zero";
            }
            if ((((this._left == this._right) && (this._right == this._top)) && (this._top == this._bottom)))
            {
                return $"EdgeInsets.all({this._left.toStringAsFixed(1L)})";
            }
            return $"EdgeInsets({this._left.toStringAsFixed(1L)}, " + $"{this._top.toStringAsFixed(1L)}, " + $"{this._right.toStringAsFixed(1L)}, " + $"{this._bottom.toStringAsFixed(1L)})";
        }
        if (((this._left == 0.0) && (this._right == 0.0)))
        {
            return $"EdgeInsetsDirectional({this._start.toStringAsFixed(1L)}, " + $"{this._top.toStringAsFixed(1L)}, " + $"{this._end.toStringAsFixed(1L)}, " + $"{this._bottom.toStringAsFixed(1L)})";
        }
        return $"EdgeInsets({this._left.toStringAsFixed(1L)}, " + $"{this._top.toStringAsFixed(1L)}, " + $"{this._right.toStringAsFixed(1L)}, " + $"{this._bottom.toStringAsFixed(1L)})" + " + " + $"EdgeInsetsDirectional({this._start.toStringAsFixed(1L)}, " + "0.0, " + $"{this._end.toStringAsFixed(1L)}, " + "0.0)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as EdgeInsetsGeometry;
        if (__other is null) return false;
        return (((((((__other is EdgeInsetsGeometry) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._left == this._left)) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._right == this._right)) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._start == this._start)) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._end == this._end)) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._top == this._top)) && (((EdgeInsetsGeometry)((EdgeInsetsGeometry)__other))._bottom == this._bottom));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this._left, this._right, this._start, this._end, this._top, this._bottom);
}

public class EdgeInsets : EdgeInsetsGeometry
{
    public static EdgeInsets zero = EdgeInsets.CreateOnly();
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

    public static EdgeInsets CreateAll(double value)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = value;
        __instance.top = value;
        __instance.right = value;
        __instance.bottom = value;
        return __instance;
    }

    public static EdgeInsets CreateOnly(double left = 0.0, double top = 0.0, double right = 0.0, double bottom = 0.0)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = left;
        __instance.top = top;
        __instance.right = right;
        __instance.bottom = bottom;
        return __instance;
    }

    public static EdgeInsets CreateSymmetric(double vertical = 0.0, double horizontal = 0.0)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = horizontal;
        __instance.top = vertical;
        __instance.right = horizontal;
        __instance.bottom = vertical;
        return __instance;
    }

    public static EdgeInsets CreateFromViewPadding(ViewPadding padding, double devicePixelRatio)
    {
        var __instance = new EdgeInsets(default!, default!, default!, default!);
        __instance.left = (padding.left / devicePixelRatio);
        __instance.top = (padding.top / devicePixelRatio);
        __instance.right = (padding.right / devicePixelRatio);
        __instance.bottom = (padding.bottom / devicePixelRatio);
        return __instance;
    }

    public static EdgeInsets CreateFromWindowPadding(ViewPadding padding, double devicePixelRatio)
        => EdgeInsets.CreateFromViewPadding(padding, devicePixelRatio);

    internal override double _left => this.left;
    internal override double _top => this.top;
    internal override double _right => this.right;
    internal override double _bottom => this.bottom;
    internal override double _start => 0.0;
    internal override double _end => 0.0;
    public virtual global::Doroti.Ui.Offset topLeft => new global::Doroti.Ui.Offset(this.left, this.top);
    public virtual global::Doroti.Ui.Offset topRight => new global::Doroti.Ui.Offset(-this.right, this.top);
    public virtual global::Doroti.Ui.Offset bottomLeft => new global::Doroti.Ui.Offset(this.left, -this.bottom);
    public virtual global::Doroti.Ui.Offset bottomRight => new global::Doroti.Ui.Offset(-this.right, -this.bottom);
    public override EdgeInsets flipped => new EdgeInsets(this.right, this.bottom, this.left, this.top);
    public virtual global::Doroti.Ui.Rect inflateRect(Rect rect)
    {
        return global::Doroti.Ui.Rect.fromLTRB((rect.left - this.left), (rect.top - this.top), (rect.right + this.right), (rect.bottom + this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect deflateRect(Rect rect)
    {
        return global::Doroti.Ui.Rect.fromLTRB((rect.left + this.left), (rect.top + this.top), (rect.right - this.right), (rect.bottom - this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.RRect inflateRRect(RRect rect)
    {
        return global::Doroti.Ui.RRect.fromLTRBAndCorners((rect.left - this.left), (rect.top - this.top), (rect.right + this.right), (rect.bottom + this.bottom), topLeft: ((rect.tlRadius + global::Doroti.Ui.Radius.elliptical(this.left, this.top))).clamp(minimum: Radius.zero), topRight: ((rect.trRadius + global::Doroti.Ui.Radius.elliptical(this.right, this.top))).clamp(minimum: Radius.zero), bottomRight: ((rect.brRadius + global::Doroti.Ui.Radius.elliptical(this.right, this.bottom))).clamp(minimum: Radius.zero), bottomLeft: ((rect.blRadius + global::Doroti.Ui.Radius.elliptical(this.left, this.bottom))).clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.RRect deflateRRect(RRect rect)
    {
        return global::Doroti.Ui.RRect.fromLTRBAndCorners((rect.left + this.left), (rect.top + this.top), (rect.right - this.right), (rect.bottom - this.bottom), topLeft: ((rect.tlRadius - global::Doroti.Ui.Radius.elliptical(this.left, this.top))).clamp(minimum: Radius.zero), topRight: ((rect.trRadius - global::Doroti.Ui.Radius.elliptical(this.right, this.top))).clamp(minimum: Radius.zero), bottomRight: ((rect.brRadius - global::Doroti.Ui.Radius.elliptical(this.right, this.bottom))).clamp(minimum: Radius.zero), bottomLeft: ((rect.blRadius - global::Doroti.Ui.Radius.elliptical(this.left, this.bottom))).clamp(minimum: Radius.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        if ((other is EdgeInsets))
        {
            EdgeInsets other__as22836 = (EdgeInsets)other;
            return (this.op_Subtract(((EdgeInsets)other__as22836)));
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        if ((other is EdgeInsets))
        {
            EdgeInsets other__as23004 = (EdgeInsets)other;
            return (this.op_Add(((EdgeInsets)other__as23004)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry clamp(EdgeInsetsGeometry min, EdgeInsetsGeometry max)
    {
        return new EdgeInsets(Dart_uiLibrary.clampDouble(this._left, ((EdgeInsetsGeometry)min)._left, ((EdgeInsetsGeometry)max)._left), Dart_uiLibrary.clampDouble(this._top, ((EdgeInsetsGeometry)min)._top, ((EdgeInsetsGeometry)max)._top), Dart_uiLibrary.clampDouble(this._right, ((EdgeInsetsGeometry)min)._right, ((EdgeInsetsGeometry)max)._right), Dart_uiLibrary.clampDouble(this._bottom, ((EdgeInsetsGeometry)min)._bottom, ((EdgeInsetsGeometry)max)._bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsets op_Subtract(EdgeInsets other)
    {
        return new EdgeInsets((this.left - ((EdgeInsets)other).left), (this.top - ((EdgeInsets)other).top), (this.right - ((EdgeInsets)other).right), (this.bottom - ((EdgeInsets)other).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsets op_Add(EdgeInsets other)
    {
        return new EdgeInsets((this.left + ((EdgeInsets)other).left), (this.top + ((EdgeInsets)other).top), (this.right + ((EdgeInsets)other).right), (this.bottom + ((EdgeInsets)other).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Subtract()
    {
        return new EdgeInsets(-this.left, -this.top, -this.right, -this.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Multiply(double other)
    {
        return new EdgeInsets((this.left * other), (this.top * other), (this.right * other), (this.bottom * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets op_Divide(double other)
    {
        return new EdgeInsets((this.left / other), (this.top / other), (this.right / other), (this.bottom / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets ___(double other)
    {
        return new EdgeInsets(((checked((long)(this.left / other)))).toDouble(), ((checked((long)(this.top / other)))).toDouble(), ((checked((long)(this.right / other)))).toDouble(), ((checked((long)(this.bottom / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets __(double other)
    {
        return new EdgeInsets((this.left % other), (this.top % other), (this.right % other), (this.bottom % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static EdgeInsets? lerp(EdgeInsets? a, EdgeInsets? b, double t)
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
        return new EdgeInsets(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsets)a).left, ((EdgeInsets)b).left, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsets)a).top, ((EdgeInsets)b).top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsets)a).right, ((EdgeInsets)b).right, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsets)a).bottom, ((EdgeInsets)b).bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction) => this;
    public virtual EdgeInsets copyWith(double? left = null, double? top = null, double? right = null, double? bottom = null)
    {
        return EdgeInsets.CreateOnly(left: (left ?? this.left), top: (top ?? this.top), right: (right ?? this.right), bottom: (bottom ?? this.bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EdgeInsetsDirectional : EdgeInsetsGeometry
{
    public static EdgeInsetsDirectional zero = EdgeInsetsDirectional.CreateOnly();
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

    public static EdgeInsetsDirectional CreateOnly(double start = 0.0, double top = 0.0, double end = 0.0, double bottom = 0.0)
    {
        var __instance = new EdgeInsetsDirectional(default!, default!, default!, default!);
        __instance.start = start;
        __instance.top = top;
        __instance.end = end;
        __instance.bottom = bottom;
        return __instance;
    }

    public static EdgeInsetsDirectional CreateSymmetric(double horizontal = 0.0, double vertical = 0.0)
    {
        var __instance = new EdgeInsetsDirectional(default!, default!, default!, default!);
        __instance.start = horizontal;
        __instance.end = horizontal;
        __instance.top = vertical;
        __instance.bottom = vertical;
        return __instance;
    }

    public static EdgeInsetsDirectional CreateAll(double value)
    {
        var __instance = new EdgeInsetsDirectional(default!, default!, default!, default!);
        __instance.start = value;
        __instance.top = value;
        __instance.end = value;
        __instance.bottom = value;
        return __instance;
    }

    internal override double _start => this.start;
    internal override double _top => this.top;
    internal override double _end => this.end;
    internal override double _bottom => this.bottom;
    internal override double _left => 0.0;
    internal override double _right => 0.0;
    public override bool isNonNegative => ((((this.start >= 0.0) && (this.top >= 0.0)) && (this.end >= 0.0)) && (this.bottom >= 0.0));
    public override EdgeInsetsDirectional flipped => new EdgeInsetsDirectional(this.end, this.bottom, this.start, this.top);
    public override EdgeInsetsGeometry subtract(EdgeInsetsGeometry other)
    {
        if ((other is EdgeInsetsDirectional))
        {
            EdgeInsetsDirectional other__as29924 = (EdgeInsetsDirectional)other;
            return (this.op_Subtract(((EdgeInsetsDirectional)other__as29924)));
        }
        return base.subtract(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry add(EdgeInsetsGeometry other)
    {
        if ((other is EdgeInsetsDirectional))
        {
            EdgeInsetsDirectional other__as30103 = (EdgeInsetsDirectional)other;
            return (this.op_Add(((EdgeInsetsDirectional)other__as30103)));
        }
        return base.add(other);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional op_Subtract(EdgeInsetsDirectional other)
    {
        return new EdgeInsetsDirectional((this.start - ((EdgeInsetsDirectional)other).start), (this.top - ((EdgeInsetsDirectional)other).top), (this.end - ((EdgeInsetsDirectional)other).end), (this.bottom - ((EdgeInsetsDirectional)other).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional op_Add(EdgeInsetsDirectional other)
    {
        return new EdgeInsetsDirectional((this.start + ((EdgeInsetsDirectional)other).start), (this.top + ((EdgeInsetsDirectional)other).top), (this.end + ((EdgeInsetsDirectional)other).end), (this.bottom + ((EdgeInsetsDirectional)other).bottom));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Subtract()
    {
        return new EdgeInsetsDirectional(-this.start, -this.top, -this.end, -this.bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Multiply(double other)
    {
        return new EdgeInsetsDirectional((this.start * other), (this.top * other), (this.end * other), (this.bottom * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional op_Divide(double other)
    {
        return new EdgeInsetsDirectional((this.start / other), (this.top / other), (this.end / other), (this.bottom / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional ___(double other)
    {
        return new EdgeInsetsDirectional(((checked((long)(this.start / other)))).toDouble(), ((checked((long)(this.top / other)))).toDouble(), ((checked((long)(this.end / other)))).toDouble(), ((checked((long)(this.bottom / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsDirectional __(double other)
    {
        return new EdgeInsetsDirectional((this.start % other), (this.top % other), (this.end % other), (this.bottom % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static EdgeInsetsDirectional? lerp(EdgeInsetsDirectional? a, EdgeInsetsDirectional? b, double t)
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
        return new EdgeInsetsDirectional(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsDirectional)a).start, ((EdgeInsetsDirectional)b).start, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsDirectional)a).top, ((EdgeInsetsDirectional)b).top, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsDirectional)a).end, ((EdgeInsetsDirectional)b).end, t)), DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((EdgeInsetsDirectional)a).bottom, ((EdgeInsetsDirectional)b).bottom, t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(EdgeInsetsDirectional)}"));
        return (DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new EdgeInsets(this.end, this.top, this.start, this.bottom), TextDirection.ltr => new EdgeInsets(this.start, this.top, this.end, this.bottom), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual EdgeInsetsDirectional copyWith(double? start = null, double? top = null, double? end = null, double? bottom = null)
    {
        return EdgeInsetsDirectional.CreateOnly(start: (start ?? this.start), top: (top ?? this.top), end: (end ?? this.end), bottom: (bottom ?? this.bottom));
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
        this.__field__left = _left;
        this.__field__right = _right;
        this.__field__start = _start;
        this.__field__end = _end;
        this.__field__top = _top;
        this.__field__bottom = _bottom;
    }

    public override bool isNonNegative
    {
        get
        {
            return ((((((this._left >= 0.0) && (this._right >= 0.0)) && (this._start >= 0.0)) && (this._end >= 0.0)) && (this._top >= 0.0)) && (this._bottom >= 0.0));
            return default!;
        }
    }
    public override _MixedEdgeInsets__edge_insets op_Subtract()
    {
        return new _MixedEdgeInsets__edge_insets(-this._left, -this._right, -this._start, -this._end, -this._top, -this._bottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets op_Multiply(double other)
    {
        return new _MixedEdgeInsets__edge_insets((this._left * other), (this._right * other), (this._start * other), (this._end * other), (this._top * other), (this._bottom * other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets op_Divide(double other)
    {
        return new _MixedEdgeInsets__edge_insets((this._left / other), (this._right / other), (this._start / other), (this._end / other), (this._top / other), (this._bottom / other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets ___(double other)
    {
        return new _MixedEdgeInsets__edge_insets(((checked((long)(this._left / other)))).toDouble(), ((checked((long)(this._right / other)))).toDouble(), ((checked((long)(this._start / other)))).toDouble(), ((checked((long)(this._end / other)))).toDouble(), ((checked((long)(this._top / other)))).toDouble(), ((checked((long)(this._bottom / other)))).toDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _MixedEdgeInsets__edge_insets __(double other)
    {
        return new _MixedEdgeInsets__edge_insets((this._left % other), (this._right % other), (this._start % other), (this._end % other), (this._top % other), (this._bottom % other));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsets resolve(TextDirection? direction)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Painting.DebugLibrary.debugCheckCanResolveTextDirection(direction, $"{typeof(_MixedEdgeInsets__edge_insets)}"));
        return (DartRuntimePrimitives.RequireValue(direction) switch { TextDirection.rtl => new EdgeInsets((this._end + this._left), this._top, (this._start + this._right), this._bottom), TextDirection.ltr => new EdgeInsets((this._start + this._left), this._top, (this._end + this._right), this._bottom), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

