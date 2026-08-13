// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/stadium_border.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public class StadiumBorder : OutlinedBorder
{
    public StadiumBorder(BorderSide side = default!) : base(side: side ?? BorderSide.none)
    {
    }

    public override ShapeBorder scale(double t) => new StadiumBorder(side: side.scale(t));
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is StadiumBorder))
        {
            StadiumBorder a__as1170 = (StadiumBorder)a;
            return new StadiumBorder(side: BorderSide.lerp(((StadiumBorder)a__as1170).side, side, t));
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as1274 = (CircleBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(((CircleBorder)a__as1274).side, side, t), circularity: (1.0 - t), eccentricity: ((CircleBorder)((CircleBorder)a__as1274)).eccentricity);
        }
        if ((a is RoundedRectangleBorder))
        {
            RoundedRectangleBorder a__as1471 = (RoundedRectangleBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(((RoundedRectangleBorder)a__as1471).side, side, t), borderRadius: ((RoundedRectangleBorder)((RoundedRectangleBorder)a__as1471)).borderRadius, rectilinearity: (1.0 - t));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is StadiumBorder))
        {
            StadiumBorder b__as1791 = (StadiumBorder)b;
            return new StadiumBorder(side: BorderSide.lerp(side, ((StadiumBorder)b__as1791).side, t));
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as1895 = (CircleBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, ((CircleBorder)b__as1895).side, t), circularity: t, eccentricity: ((CircleBorder)((CircleBorder)b__as1895)).eccentricity);
        }
        if ((b is RoundedRectangleBorder))
        {
            RoundedRectangleBorder b__as2086 = (RoundedRectangleBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, ((RoundedRectangleBorder)b__as2086).side, t), borderRadius: ((RoundedRectangleBorder)((RoundedRectangleBorder)b__as2086)).borderRadius, rectilinearity: t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override StadiumBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new StadiumBorder(side: (side ?? this.side));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        var radius__2530 = global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2.0));
        var borderRect__2591 = global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect, radius__2530);
        global::Doroti.Flutter.Ui.RRect adjustedRect__2659 = borderRect__2591.deflate(((BorderSide)side).strokeInset);
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(adjustedRect__2659);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        var radius__2847 = global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2.0));
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect, radius__2847));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        var radius__3070 = global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2.0));
        return global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect, radius__3070).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        var radius__3311 = global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2.0));
        canvas.drawRRect(global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect, radius__3311), paint);
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
                    var radius__3687 = global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2L));
                    var borderRect__3750 = global::Doroti.Flutter.Ui.RRect.fromRectAndRadius(rect, radius__3687);
                    canvas.drawRRect(borderRect__3750.inflate((((BorderSide)side).strokeOffset / 2L)), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as StadiumBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is StadiumBorder) && (object.Equals(((StadiumBorder)__other).side, side)));
    }

    public override int GetHashCode() => side.GetHashCode();
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "StadiumBorder"))}({side})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StadiumToCircleBorder__stadium_border : OutlinedBorder
{
    public virtual double circularity { get; private set; } = default!;
    public virtual double eccentricity { get; private set; } = default!;

    internal _StadiumToCircleBorder__stadium_border(BorderSide side = default!, double circularity = 0.0, double eccentricity = default!) : base(side: side ?? BorderSide.none)
    {
        this.circularity = circularity;
        this.eccentricity = eccentricity;
    }

    public override ShapeBorder scale(double t)
    {
        return new _StadiumToCircleBorder__stadium_border(side: side.scale(t), circularity: t, eccentricity: this.eccentricity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is StadiumBorder))
        {
            StadiumBorder a__as4725 = (StadiumBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(((StadiumBorder)a__as4725).side, side, t), circularity: (this.circularity * t), eccentricity: this.eccentricity);
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as4929 = (CircleBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(((CircleBorder)a__as4929).side, side, t), circularity: (this.circularity + (((1.0 - this.circularity)) * ((1.0 - t)))), eccentricity: ((CircleBorder)((CircleBorder)a__as4929)).eccentricity);
        }
        if ((a is _StadiumToCircleBorder__stadium_border))
        {
            _StadiumToCircleBorder__stadium_border a__as5164 = (_StadiumToCircleBorder__stadium_border)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(((_StadiumToCircleBorder__stadium_border)a__as5164).side, side, t), circularity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((_StadiumToCircleBorder__stadium_border)((_StadiumToCircleBorder__stadium_border)a__as5164)).circularity, this.circularity, t)), eccentricity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((_StadiumToCircleBorder__stadium_border)((_StadiumToCircleBorder__stadium_border)a__as5164)).eccentricity, this.eccentricity, t)));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is StadiumBorder))
        {
            StadiumBorder b__as5542 = (StadiumBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, ((StadiumBorder)b__as5542).side, t), circularity: (this.circularity * ((1.0 - t))), eccentricity: this.eccentricity);
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as5754 = (CircleBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, ((CircleBorder)b__as5754).side, t), circularity: (this.circularity + (((1.0 - this.circularity)) * t)), eccentricity: ((CircleBorder)((CircleBorder)b__as5754)).eccentricity);
        }
        if ((b is _StadiumToCircleBorder__stadium_border))
        {
            _StadiumToCircleBorder__stadium_border b__as5981 = (_StadiumToCircleBorder__stadium_border)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, ((_StadiumToCircleBorder__stadium_border)b__as5981).side, t), circularity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.circularity, ((_StadiumToCircleBorder__stadium_border)((_StadiumToCircleBorder__stadium_border)b__as5981)).circularity, t)), eccentricity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.eccentricity, ((_StadiumToCircleBorder__stadium_border)((_StadiumToCircleBorder__stadium_border)b__as5981)).eccentricity, t)));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Rect _adjustRect(Rect rect)
    {
        if (((this.circularity == 0.0) || (rect.width == rect.height)))
        {
            return rect;
        }
        if ((rect.width < rect.height))
        {
            double partialDelta__6458 = (((rect.height - rect.width)) / 2L);
            double delta__6524 = ((this.circularity * partialDelta__6458) * ((1.0 - this.eccentricity)));
            return global::Doroti.Flutter.Ui.Rect.fromLTRB(rect.left, (rect.top + delta__6524), rect.right, (rect.bottom - delta__6524));
        }
        else
        {
            double partialDelta__6705 = (((rect.width - rect.height)) / 2L);
            double delta__6771 = ((this.circularity * partialDelta__6705) * ((1.0 - this.eccentricity)));
            return global::Doroti.Flutter.Ui.Rect.fromLTRB((rect.left + delta__6771), rect.top, (rect.right - delta__6771), rect.bottom);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadius _adjustBorderRadius(Rect rect)
    {
        var circleRadius__6989 = BorderRadius.CreateCircular((rect.shortestSide / 2L));
        if ((this.eccentricity != 0.0))
        {
            if ((rect.width < rect.height))
            {
                return BorderRadius.lerp(circleRadius__6989, BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.elliptical((rect.width / 2L), ((((0.5 + (this.eccentricity / 2L))) * rect.height) / 2L))), DartRuntimePrimitives.RequireValue(this.circularity))!;
            }
            else
            {
                return BorderRadius.lerp(circleRadius__6989, BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.elliptical(((((0.5 + (this.eccentricity / 2L))) * rect.width) / 2L), (rect.height / 2L))), DartRuntimePrimitives.RequireValue(this.circularity))!;
            }
        }
        return circleRadius__6989;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(_adjustBorderRadius(rect).toRRect(_adjustRect(rect)).deflate(((BorderSide)side).strokeInset));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(_adjustBorderRadius(rect).toRRect(_adjustRect(rect)));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        return _adjustBorderRadius(rect).toRRect(_adjustRect(rect)).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRRect(_adjustBorderRadius(rect).toRRect(_adjustRect(rect)), paint);
    }

    public override bool preferPaintInterior => true;
    public override _StadiumToCircleBorder__stadium_border copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new _StadiumToCircleBorder__stadium_border(side: (side ?? this.side), circularity: (circularity ?? this.circularity), eccentricity: (eccentricity ?? this.eccentricity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

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
                    global::Doroti.Flutter.Ui.RRect borderRect__8917 = _adjustBorderRadius(rect).toRRect(_adjustRect(rect));
                    canvas.drawRRect(borderRect__8917.inflate((((BorderSide)side).strokeOffset / 2L)), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _StadiumToCircleBorder__stadium_border;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _StadiumToCircleBorder__stadium_border) && (object.Equals(((_StadiumToCircleBorder__stadium_border)__other).side, side))) && (((_StadiumToCircleBorder__stadium_border)((_StadiumToCircleBorder__stadium_border)__other)).circularity == this.circularity));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.circularity);
    public override string ToString()
    {
        if ((this.eccentricity != 0.0))
        {
            return $"StadiumBorder({side}, {((this.circularity * 100L)).toStringAsFixed(1L)}% of the way to being a CircleBorder that is {((this.eccentricity * 100L)).toStringAsFixed(1L)}% oval)";
        }
        return $"StadiumBorder({side}, {((this.circularity * 100L)).toStringAsFixed(1L)}% of the way to being a CircleBorder)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _StadiumToRoundedRectangleBorder__stadium_border : OutlinedBorder
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;
    public virtual double rectilinearity { get; private set; } = default!;

    internal _StadiumToRoundedRectangleBorder__stadium_border(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!, double rectilinearity = 0.0) : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
        this.rectilinearity = rectilinearity;
    }

    public override ShapeBorder scale(double t)
    {
        return new _StadiumToRoundedRectangleBorder__stadium_border(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)), rectilinearity: t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is StadiumBorder))
        {
            StadiumBorder a__as10360 = (StadiumBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(((StadiumBorder)a__as10360).side, side, t), borderRadius: this.borderRadius, rectilinearity: (this.rectilinearity * t));
        }
        if ((a is RoundedRectangleBorder))
        {
            RoundedRectangleBorder a__as10580 = (RoundedRectangleBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(((RoundedRectangleBorder)a__as10580).side, side, t), borderRadius: this.borderRadius, rectilinearity: (this.rectilinearity + (((1.0 - this.rectilinearity)) * ((1.0 - t)))));
        }
        if ((a is _StadiumToRoundedRectangleBorder__stadium_border))
        {
            _StadiumToRoundedRectangleBorder__stadium_border a__as10842 = (_StadiumToRoundedRectangleBorder__stadium_border)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(((_StadiumToRoundedRectangleBorder__stadium_border)a__as10842).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)a__as10842)).borderRadius, this.borderRadius, t)!, rectilinearity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)a__as10842)).rectilinearity, this.rectilinearity, t)));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is StadiumBorder))
        {
            StadiumBorder b__as11261 = (StadiumBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, ((StadiumBorder)b__as11261).side, t), borderRadius: this.borderRadius, rectilinearity: (this.rectilinearity * ((1.0 - t))));
        }
        if ((b is RoundedRectangleBorder))
        {
            RoundedRectangleBorder b__as11489 = (RoundedRectangleBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, ((RoundedRectangleBorder)b__as11489).side, t), borderRadius: this.borderRadius, rectilinearity: (this.rectilinearity + (((1.0 - this.rectilinearity)) * t)));
        }
        if ((b is _StadiumToRoundedRectangleBorder__stadium_border))
        {
            _StadiumToRoundedRectangleBorder__stadium_border b__as11743 = (_StadiumToRoundedRectangleBorder__stadium_border)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, ((_StadiumToRoundedRectangleBorder__stadium_border)b__as11743).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)b__as11743)).borderRadius, t)!, rectilinearity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.rectilinearity, ((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)b__as11743)).rectilinearity, t)));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadiusGeometry _adjustBorderRadius(Rect rect)
    {
        return BorderRadiusGeometry.lerp(this.borderRadius, BorderRadius.CreateAll(global::Doroti.Flutter.Ui.Radius.circular((rect.shortestSide / 2.0))), (1.0 - this.rectilinearity))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        global::Doroti.Flutter.Ui.RRect borderRect__12404 = _adjustBorderRadius(rect).resolve(textDirection).toRRect(rect);
        global::Doroti.Flutter.Ui.RRect adjustedRect__12497 = borderRect__12404.deflate(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((BorderSide)side).width, 0L, ((BorderSide)side).strokeAlign)));
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(adjustedRect__12497);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addRRect(_adjustBorderRadius(rect).resolve(textDirection).toRRect(rect));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        BorderRadius adjustedBorderRadius__12916 = _adjustBorderRadius(rect).resolve(textDirection);
        if ((object.Equals(adjustedBorderRadius__12916, BorderRadius.zero)))
        {
            return rect.contains(position);
        }
        return adjustedBorderRadius__12916.toRRect(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        BorderRadiusGeometry adjustedBorderRadius__13294 = _adjustBorderRadius(rect);
        if ((object.Equals(adjustedBorderRadius__13294, BorderRadius.zero)))
        {
            canvas.drawRect(rect, paint);
        }
        else
        {
            canvas.drawRRect(adjustedBorderRadius__13294.resolve(textDirection).toRRect(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;
    public override _StadiumToRoundedRectangleBorder__stadium_border copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new _StadiumToRoundedRectangleBorder__stadium_border(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius), rectilinearity: (rectilinearity ?? this.rectilinearity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

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
                    BorderRadiusGeometry adjustedBorderRadius__14179 = _adjustBorderRadius(rect);
                    global::Doroti.Flutter.Ui.RRect borderRect__14249 = adjustedBorderRadius__14179.resolve(textDirection).toRRect(rect);
                    canvas.drawRRect(borderRect__14249.inflate((((BorderSide)side).strokeOffset / 2L)), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _StadiumToRoundedRectangleBorder__stadium_border;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is _StadiumToRoundedRectangleBorder__stadium_border) && (object.Equals(((_StadiumToRoundedRectangleBorder__stadium_border)__other).side, side))) && (object.Equals(((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)__other)).borderRadius, this.borderRadius))) && (((_StadiumToRoundedRectangleBorder__stadium_border)((_StadiumToRoundedRectangleBorder__stadium_border)__other)).rectilinearity == this.rectilinearity));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius, this.rectilinearity);
    public override string ToString()
    {
        return $"StadiumBorder({side}, {this.borderRadius}, " + $"{((this.rectilinearity * 100L)).toStringAsFixed(1L)}% of the way to being a " + "RoundedRectangleBorder)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

