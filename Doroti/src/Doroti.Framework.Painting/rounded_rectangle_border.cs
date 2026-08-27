// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/rounded_rectangle_border.dart
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

public interface _RRectLikeBorder__rounded_rectangle_border
{
    public BorderRadiusGeometry borderRadius { get; }
}

public class RoundedRectangleBorder : OutlinedBorder, _RRectLikeBorder__rounded_rectangle_border
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;

    public RoundedRectangleBorder(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!) : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
    }

    public override ShapeBorder scale(double t)
    {
        return new RoundedRectangleBorder(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is RoundedRectangleBorder))
        {
            RoundedRectangleBorder a__as1656 = (RoundedRectangleBorder)a;
            return new RoundedRectangleBorder(side: BorderSide.lerp(((RoundedRectangleBorder)a__as1656).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((RoundedRectangleBorder)((RoundedRectangleBorder)a__as1656)).borderRadius, this.borderRadius, t)!);
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as1878 = (CircleBorder)a;
            return new _RoundedRectangleToCircleBorder__rounded_rectangle_border(side: BorderSide.lerp(((CircleBorder)a__as1878).side, side, t), borderRadius: this.borderRadius, circularity: (1.0 - t), eccentricity: ((CircleBorder)((CircleBorder)a__as1878)).eccentricity);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is RoundedRectangleBorder))
        {
            RoundedRectangleBorder b__as2220 = (RoundedRectangleBorder)b;
            return new RoundedRectangleBorder(side: BorderSide.lerp(side, ((RoundedRectangleBorder)b__as2220).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((RoundedRectangleBorder)((RoundedRectangleBorder)b__as2220)).borderRadius, t)!);
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as2442 = (CircleBorder)b;
            return new _RoundedRectangleToCircleBorder__rounded_rectangle_border(side: BorderSide.lerp(side, ((CircleBorder)b__as2442).side, t), borderRadius: this.borderRadius, circularity: t, eccentricity: ((CircleBorder)((CircleBorder)b__as2442)).eccentricity);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RoundedRectangleBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new RoundedRectangleBorder(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.RRect borderRect = this.borderRadius.resolve(textDirection).toRRect(rect);
        global::Doroti.Ui.RRect adjustedRect = borderRect.deflate(((BorderSide)side).strokeInset);
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(adjustedRect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(this.borderRadius.resolve(textDirection).toRRect(rect));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        BorderRadius resolvedBorderRadius = this.borderRadius.resolve(textDirection);
        if ((object.Equals(resolvedBorderRadius, BorderRadius.zero)))
        {
            return rect.contains(position);
        }
        return resolvedBorderRadius.toRRect(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        if ((object.Equals(this.borderRadius, BorderRadius.zero)))
        {
            canvas.drawRect(rect, paint);
        }
        else
        {
            canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (((BorderSide)side).style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    if ((((BorderSide)side).width == 0.0))
                    {
                        canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), side.toPaint());
                    }
                    else
                    {
                        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((BorderSide)side).color;
    return __cascade;
}))();
                        global::Doroti.Ui.RRect borderRect = this.borderRadius.resolve(textDirection).toRRect(rect);
                        global::Doroti.Ui.RRect inner = borderRect.deflate(((BorderSide)side).strokeInset);
                        global::Doroti.Ui.RRect outer = borderRect.inflate(((BorderSide)side).strokeOutset);
                        canvas.drawDRRect(outer, inner, paintLocal);
                    }
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as RoundedRectangleBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is RoundedRectangleBorder) && (object.Equals(((RoundedRectangleBorder)__other).side, side))) && (object.Equals(((RoundedRectangleBorder)((RoundedRectangleBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius);
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RoundedRectangleBorder"))}({side}, {this.borderRadius})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RoundedRectangleToCircleBorder__rounded_rectangle_border : _ShapeToCircleBorder__rounded_rectangle_border<RoundedRectangleBorder>
{
    internal _RoundedRectangleToCircleBorder__rounded_rectangle_border(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!, double circularity = default!, double eccentricity = default!) : base(side: side ?? BorderSide.none, borderRadius: borderRadius ?? BorderRadius.zero, circularity: circularity, eccentricity: eccentricity)
    {
    }

    public override void drawShape(Canvas canvas, Rect rect, BorderRadius radius, Paint paint, double? inflation = null)
    {
        global::Doroti.Ui.RRect rrect = radius.toRRect(rect);
        if ((inflation is not null))
        {
            double inflation__value5719 = DartRuntimePrimitives.RequireValue(inflation);
            rrect = rrect.inflate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inflation__value5719)));
        }
        canvas.drawRRect(rrect, paint);
    }

    public override Path buildPath(Rect rect, BorderRadius radius, double? inflation = null)
    {
        global::Doroti.Ui.RRect rrect = radius.toRRect(rect);
        if ((inflation is not null))
        {
            double inflation__value5959 = DartRuntimePrimitives.RequireValue(inflation);
            rrect = rrect.inflate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inflation__value5959)));
        }
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(rrect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool containsOuterShape(Rect rect, BorderRadius radius, Offset position)
    {
        return radius.toRRect(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _RoundedRectangleToCircleBorder__rounded_rectangle_border copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new _RoundedRectangleToCircleBorder__rounded_rectangle_border(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius), circularity: (circularity ?? this.circularity), eccentricity: (eccentricity ?? this.eccentricity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundedSuperellipseBorder : OutlinedBorder, _RRectLikeBorder__rounded_rectangle_border
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;

    public RoundedSuperellipseBorder(BorderSide side = default!, BorderRadiusGeometry? borderRadius = null) : base(side: side ?? BorderSide.none)
    {
        this.borderRadius = (borderRadius ?? BorderRadius.zero);
    }

    public override ShapeBorder scale(double t)
    {
        return new RoundedSuperellipseBorder(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is RoundedSuperellipseBorder))
        {
            RoundedSuperellipseBorder a__as8206 = (RoundedSuperellipseBorder)a;
            return new RoundedSuperellipseBorder(side: BorderSide.lerp(((RoundedSuperellipseBorder)a__as8206).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((RoundedSuperellipseBorder)((RoundedSuperellipseBorder)a__as8206)).borderRadius, this.borderRadius, t));
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as8433 = (CircleBorder)a;
            return new _RoundedSuperellipseToCircleBorder__rounded_rectangle_border(side: BorderSide.lerp(((CircleBorder)a__as8433).side, side, t), borderRadius: this.borderRadius, circularity: (1.0 - t), eccentricity: ((CircleBorder)((CircleBorder)a__as8433)).eccentricity);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is RoundedSuperellipseBorder))
        {
            RoundedSuperellipseBorder b__as8778 = (RoundedSuperellipseBorder)b;
            return new RoundedSuperellipseBorder(side: BorderSide.lerp(side, ((RoundedSuperellipseBorder)b__as8778).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((RoundedSuperellipseBorder)((RoundedSuperellipseBorder)b__as8778)).borderRadius, t));
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as9005 = (CircleBorder)b;
            return new _RoundedSuperellipseToCircleBorder__rounded_rectangle_border(side: BorderSide.lerp(side, ((CircleBorder)b__as9005).side, t), borderRadius: this.borderRadius, circularity: t, eccentricity: ((CircleBorder)((CircleBorder)b__as9005)).eccentricity);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RoundedSuperellipseBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new RoundedSuperellipseBorder(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        if ((object.Equals(this.borderRadius, BorderRadius.zero)))
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect.deflate(((BorderSide)side).strokeInset));
    return __cascade;
}))();
        }
        else
        {
            global::Doroti.Ui.RSuperellipse borderRect = this.borderRadius.resolve(textDirection).toRSuperellipse(rect);
            global::Doroti.Ui.RSuperellipse adjustedRect = borderRect.deflate(((BorderSide)side).strokeInset);
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRSuperellipse(adjustedRect);
    return __cascade;
}))();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        if ((object.Equals(this.borderRadius, BorderRadius.zero)))
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        }
        else
        {
            return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRSuperellipse(this.borderRadius.resolve(textDirection).toRSuperellipse(rect));
    return __cascade;
}))();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        BorderRadius resolvedBorderRadius = this.borderRadius.resolve(textDirection);
        if ((object.Equals(resolvedBorderRadius, BorderRadius.zero)))
        {
            return rect.contains(position);
        }
        return resolvedBorderRadius.toRSuperellipse(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        if ((object.Equals(this.borderRadius, BorderRadius.zero)))
        {
            canvas.drawRect(rect, paint);
        }
        else
        {
            canvas.drawRSuperellipse(this.borderRadius.resolve(textDirection).toRSuperellipse(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (((BorderSide)side).style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    double strokeOffset = (((((BorderSide)side).strokeOutset - ((BorderSide)side).strokeInset)) / 2L);
                    if ((object.Equals(this.borderRadius, BorderRadius.zero)))
                    {
                        global::Doroti.Ui.Rect @base = rect.inflate(strokeOffset);
                        canvas.drawRect(@base, side.toPaint());
                    }
                    else
                    {
                        global::Doroti.Ui.RSuperellipse baseLocal = this.borderRadius.resolve(textDirection).toRSuperellipse(rect).inflate(strokeOffset);
                        canvas.drawRSuperellipse(baseLocal, side.toPaint());
                    }
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as RoundedSuperellipseBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is RoundedSuperellipseBorder) && (object.Equals(((RoundedSuperellipseBorder)__other).side, side))) && (object.Equals(((RoundedSuperellipseBorder)((RoundedSuperellipseBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius);
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RoundedSuperellipseBorder"))}({side}, {this.borderRadius})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RoundedSuperellipseToCircleBorder__rounded_rectangle_border : _ShapeToCircleBorder__rounded_rectangle_border<RoundedSuperellipseBorder>
{
    internal _RoundedSuperellipseToCircleBorder__rounded_rectangle_border(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!, double circularity = default!, double eccentricity = default!) : base(side: side ?? BorderSide.none, borderRadius: borderRadius ?? BorderRadius.zero, circularity: circularity, eccentricity: eccentricity)
    {
    }

    public override void drawShape(Canvas canvas, Rect rect, BorderRadius radius, Paint paint, double? inflation = null)
    {
        global::Doroti.Ui.RSuperellipse rsuperellipse = radius.toRSuperellipse(rect);
        if ((inflation is not null))
        {
            double inflation__value12640 = DartRuntimePrimitives.RequireValue(inflation);
            rsuperellipse = rsuperellipse.inflate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inflation__value12640)));
        }
        canvas.drawRSuperellipse(rsuperellipse, paint);
    }

    public override Path buildPath(Rect rect, BorderRadius radius, double? inflation = null)
    {
        global::Doroti.Ui.RSuperellipse rsuperellipse = radius.toRSuperellipse(rect);
        if ((inflation is not null))
        {
            double inflation__value12936 = DartRuntimePrimitives.RequireValue(inflation);
            rsuperellipse = rsuperellipse.inflate(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(inflation__value12936)));
        }
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRSuperellipse(rsuperellipse);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool containsOuterShape(Rect rect, BorderRadius radius, Offset position)
    {
        return radius.toRSuperellipse(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _RoundedSuperellipseToCircleBorder__rounded_rectangle_border copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new _RoundedSuperellipseToCircleBorder__rounded_rectangle_border(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius), circularity: (circularity ?? this.circularity), eccentricity: (eccentricity ?? this.eccentricity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal abstract class _ShapeToCircleBorder__rounded_rectangle_border<T> : OutlinedBorder where T : _RRectLikeBorder__rounded_rectangle_border
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual double circularity { get; private set; } = default!;
    public virtual double eccentricity { get; private set; } = default!;

    internal _ShapeToCircleBorder__rounded_rectangle_border(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!, double circularity = default!, double eccentricity = default!) : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.circularity = circularity;
        this.eccentricity = eccentricity;
    }

    public abstract void drawShape(Canvas canvas, Rect rect, BorderRadius radius, Paint paint, double? inflation = null);
    public abstract global::Doroti.Ui.Path buildPath(Rect rect, BorderRadius radius, double? inflation = null);
    public abstract bool containsOuterShape(Rect rect, BorderRadius radius, Offset position);
    public override ShapeBorder scale(double t)
    {
        return copyWith(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)), circularity: t, eccentricity: this.eccentricity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is T))
        {
            T a__as14531 = (T)(object)a;
            return copyWith(side: BorderSide.lerp(((OutlinedBorder)(object)a__as14531).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((_RRectLikeBorder__rounded_rectangle_border)(object)a__as14531).borderRadius, this.borderRadius, t), circularity: (this.circularity * t), eccentricity: this.eccentricity);
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as14791 = (CircleBorder)a;
            return copyWith(side: BorderSide.lerp(((CircleBorder)a__as14791).side, side, t), borderRadius: this.borderRadius, circularity: (this.circularity + (((1.0 - this.circularity)) * ((1.0 - t)))), eccentricity: ((CircleBorder)((CircleBorder)a__as14791)).eccentricity);
        }
        if ((a is _ShapeToCircleBorder__rounded_rectangle_border<T>))
        {
            _ShapeToCircleBorder__rounded_rectangle_border<T> a__as15048 = (_ShapeToCircleBorder__rounded_rectangle_border<T>)a;
            return copyWith(side: BorderSide.lerp(((_ShapeToCircleBorder__rounded_rectangle_border<T>)a__as15048).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)a__as15048)).borderRadius, this.borderRadius, t), circularity: Dart_uiLibrary.lerpDouble(((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)a__as15048)).circularity, this.circularity, t), eccentricity: this.eccentricity);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is T))
        {
            T b__as15459 = (T)(object)b;
            return copyWith(side: BorderSide.lerp(side, ((OutlinedBorder)(object)b__as15459).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((_RRectLikeBorder__rounded_rectangle_border)(object)b__as15459).borderRadius, t), circularity: (this.circularity * ((1.0 - t))), eccentricity: this.eccentricity);
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as15727 = (CircleBorder)b;
            return copyWith(side: BorderSide.lerp(side, ((CircleBorder)b__as15727).side, t), borderRadius: this.borderRadius, circularity: (this.circularity + (((1.0 - this.circularity)) * t)), eccentricity: ((CircleBorder)((CircleBorder)b__as15727)).eccentricity);
        }
        if ((b is _ShapeToCircleBorder__rounded_rectangle_border<T>))
        {
            _ShapeToCircleBorder__rounded_rectangle_border<T> b__as15976 = (_ShapeToCircleBorder__rounded_rectangle_border<T>)b;
            return copyWith(side: BorderSide.lerp(side, ((_ShapeToCircleBorder__rounded_rectangle_border<T>)b__as15976).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)b__as15976)).borderRadius, t), circularity: Dart_uiLibrary.lerpDouble(this.circularity, ((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)b__as15976)).circularity, t), eccentricity: this.eccentricity);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _adjustRect(Rect rect)
    {
        if (((this.circularity == 0.0) || (rect.width == rect.height)))
        {
            return rect;
        }
        if ((rect.width < rect.height))
        {
            double partialDelta = (((rect.height - rect.width)) / 2L);
            double delta = ((this.circularity * partialDelta) * ((1.0 - this.eccentricity)));
            return global::Doroti.Ui.Rect.fromLTRB(rect.left, (rect.top + delta), rect.right, (rect.bottom - delta));
        }
        else
        {
            double partialDeltaLocal = (((rect.width - rect.height)) / 2L);
            double deltaLocal = ((this.circularity * partialDeltaLocal) * ((1.0 - this.eccentricity)));
            return global::Doroti.Ui.Rect.fromLTRB((rect.left + deltaLocal), rect.top, (rect.right - deltaLocal), rect.bottom);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadius _adjustBorderRadius(Rect rect, TextDirection? textDirection)
    {
        BorderRadius resolvedRadius = this.borderRadius.resolve(textDirection);
        if ((this.circularity == 0.0))
        {
            return resolvedRadius;
        }
        if ((this.eccentricity != 0.0))
        {
            if ((rect.width < rect.height))
            {
                return BorderRadius.lerp(resolvedRadius, BorderRadius.CreateAll(global::Doroti.Ui.Radius.elliptical((rect.width / 2L), ((((0.5 + (this.eccentricity / 2L))) * rect.height) / 2L))), DartRuntimePrimitives.RequireValue(this.circularity))!;
            }
            else
            {
                return BorderRadius.lerp(resolvedRadius, BorderRadius.CreateAll(global::Doroti.Ui.Radius.elliptical(((((0.5 + (this.eccentricity / 2L))) * rect.width) / 2L), (rect.height / 2L))), DartRuntimePrimitives.RequireValue(this.circularity))!;
            }
        }
        return BorderRadius.lerp(resolvedRadius, BorderRadius.CreateCircular((rect.shortestSide / 2L)), DartRuntimePrimitives.RequireValue(this.circularity))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return buildPath(_adjustRect(rect), _adjustBorderRadius(rect, textDirection), -DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BorderSide)side).width, 0L, ((BorderSide)side).strokeAlign)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return buildPath(_adjustRect(rect), _adjustBorderRadius(rect, textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.Rect adjustedRect = _adjustRect(rect);
        BorderRadius adjustedBorderRadius = _adjustBorderRadius(rect, textDirection);
        if ((object.Equals(adjustedBorderRadius, BorderRadius.zero)))
        {
            return adjustedRect.contains(position);
        }
        return containsOuterShape(adjustedRect, adjustedBorderRadius, position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        BorderRadius adjustedBorderRadius = _adjustBorderRadius(rect, textDirection);
        if ((object.Equals(adjustedBorderRadius, BorderRadius.zero)))
        {
            canvas.drawRect(_adjustRect(rect), paint);
        }
        else
        {
            drawShape(canvas, _adjustRect(rect), adjustedBorderRadius, paint);
        }
    }

    public override bool preferPaintInterior => true;
    public abstract override _ShapeToCircleBorder__rounded_rectangle_border<T> copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null);
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (((BorderSide)side).style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    drawShape(canvas, _adjustRect(rect), _adjustBorderRadius(rect, textDirection), side.toPaint(), (((BorderSide)side).strokeOffset / 2L));
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ShapeToCircleBorder__rounded_rectangle_border<T>;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is _ShapeToCircleBorder__rounded_rectangle_border<T>) && (object.Equals(((_ShapeToCircleBorder__rounded_rectangle_border<T>)__other).side, side))) && (object.Equals(((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)__other)).borderRadius, this.borderRadius))) && (((_ShapeToCircleBorder__rounded_rectangle_border<T>)((_ShapeToCircleBorder__rounded_rectangle_border<T>)__other)).circularity == this.circularity));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius, this.circularity);
    public override string ToString()
    {
        if ((this.eccentricity != 0.0))
        {
            return $"{typeof(T)}({side}, {this.borderRadius}, {((this.circularity * 100L)).toStringAsFixed(1L)}% of the way to being a CircleBorder that is {((this.eccentricity * 100L)).toStringAsFixed(1L)}% oval)";
        }
        return $"{typeof(T)}({side}, {this.borderRadius}, {((this.circularity * 100L)).toStringAsFixed(1L)}% of the way to being a CircleBorder)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

