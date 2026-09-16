// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/stadium_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class StadiumBorder : OutlinedBorder
{
    public StadiumBorder(BorderSide side = default!) : base(side: side ?? BorderSide.none)
    {
    }

    public override ShapeBorder scale(double t) => new StadiumBorder(side: side.scale(t));
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is StadiumBorder)
        {
            StadiumBorder a__as1170 = (StadiumBorder)a;
            return new StadiumBorder(side: BorderSide.lerp(a__as1170.side, side, t));
        }
        if (a is CircleBorder)
        {
            CircleBorder a__as1274 = (CircleBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(a__as1274.side, side, t), circularity: 1.0 - t, eccentricity: a__as1274.eccentricity);
        }
        if (a is RoundedRectangleBorder)
        {
            RoundedRectangleBorder a__as1471 = (RoundedRectangleBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(a__as1471.side, side, t), borderRadius: a__as1471.borderRadius, rectilinearity: 1.0 - t);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is StadiumBorder)
        {
            StadiumBorder b__as1791 = (StadiumBorder)b;
            return new StadiumBorder(side: BorderSide.lerp(side, b__as1791.side, t));
        }
        if (b is CircleBorder)
        {
            CircleBorder b__as1895 = (CircleBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, b__as1895.side, t), circularity: t, eccentricity: b__as1895.eccentricity);
        }
        if (b is RoundedRectangleBorder)
        {
            RoundedRectangleBorder b__as2086 = (RoundedRectangleBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, b__as2086.side, t), borderRadius: b__as2086.borderRadius, rectilinearity: t);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override StadiumBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new StadiumBorder(side: side ?? this.side);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        var radius = Radius.circular(rect.shortestSide / 2.0);
        var borderRect = RRect.fromRectAndRadius(rect, radius);
        RRect adjustedRect = borderRect.deflate(side.strokeInset);
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addRRect(adjustedRect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        var radius = Radius.circular(rect.shortestSide / 2.0);
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addRRect(RRect.fromRectAndRadius(rect, radius));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        var radius = Radius.circular(rect.shortestSide / 2.0);
        return RRect.fromRectAndRadius(rect, radius).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        var radius = Radius.circular(rect.shortestSide / 2.0);
        canvas.drawRRect(RRect.fromRectAndRadius(rect, radius), paint);
    }

    public override bool preferPaintInterior => true;
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (side.style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    var radius = Radius.circular(rect.shortestSide / 2L);
                    var borderRect = RRect.fromRectAndRadius(rect, radius);
                    canvas.drawRRect(borderRect.inflate(side.strokeOffset / 2L), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as StadiumBorder;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is StadiumBorder) && Equals(__other.side, side);
    }

    public override int GetHashCode() => side.GetHashCode();
    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "StadiumBorder")}({side})";
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
        return new _StadiumToCircleBorder__stadium_border(side: side.scale(t), circularity: t, eccentricity: eccentricity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is StadiumBorder)
        {
            StadiumBorder a__as4725 = (StadiumBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(a__as4725.side, side, t), circularity: circularity * t, eccentricity: eccentricity);
        }
        if (a is CircleBorder)
        {
            CircleBorder a__as4929 = (CircleBorder)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(a__as4929.side, side, t), circularity: circularity + ((1.0 - circularity) * (1.0 - t)), eccentricity: a__as4929.eccentricity);
        }
        if (a is _StadiumToCircleBorder__stadium_border)
        {
            _StadiumToCircleBorder__stadium_border a__as5164 = (_StadiumToCircleBorder__stadium_border)a;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(a__as5164.side, side, t), circularity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a__as5164.circularity, circularity, t)), eccentricity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a__as5164.eccentricity, eccentricity, t)));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is StadiumBorder)
        {
            StadiumBorder b__as5542 = (StadiumBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, b__as5542.side, t), circularity: circularity * (1.0 - t), eccentricity: eccentricity);
        }
        if (b is CircleBorder)
        {
            CircleBorder b__as5754 = (CircleBorder)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, b__as5754.side, t), circularity: circularity + ((1.0 - circularity) * t), eccentricity: b__as5754.eccentricity);
        }
        if (b is _StadiumToCircleBorder__stadium_border)
        {
            _StadiumToCircleBorder__stadium_border b__as5981 = (_StadiumToCircleBorder__stadium_border)b;
            return new _StadiumToCircleBorder__stadium_border(side: BorderSide.lerp(side, b__as5981.side, t), circularity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(circularity, b__as5981.circularity, t)), eccentricity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(eccentricity, b__as5981.eccentricity, t)));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Rect _adjustRect(Rect rect)
    {
        if ((circularity == 0.0) || (rect.width == rect.height))
        {
            return rect;
        }
        if (rect.width < rect.height)
        {
            double partialDelta = (rect.height - rect.width) / 2L;
            double delta = circularity * partialDelta * (1.0 - eccentricity);
            return Rect.fromLTRB(rect.left, rect.top + delta, rect.right, rect.bottom - delta);
        }
        else
        {
            double partialDeltaLocal = (rect.width - rect.height) / 2L;
            double deltaLocal = circularity * partialDeltaLocal * (1.0 - eccentricity);
            return Rect.fromLTRB(rect.left + deltaLocal, rect.top, rect.right - deltaLocal, rect.bottom);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadius _adjustBorderRadius(Rect rect)
    {
        var circleRadius = BorderRadius.CreateCircular(rect.shortestSide / 2L);
        if (eccentricity != 0.0)
        {
            if (rect.width < rect.height)
            {
                return BorderRadius.lerp(circleRadius, BorderRadius.CreateAll(Radius.elliptical(rect.width / 2L, (0.5 + (eccentricity / 2L)) * rect.height / 2L)), DartRuntimePrimitives.RequireValue(circularity))!;
            }
            else
            {
                return BorderRadius.lerp(circleRadius, BorderRadius.CreateAll(Radius.elliptical((0.5 + (eccentricity / 2L)) * rect.width / 2L, rect.height / 2L)), DartRuntimePrimitives.RequireValue(circularity))!;
            }
        }
        return circleRadius;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addRRect(_adjustBorderRadius(rect).toRRect(_adjustRect(rect)).deflate(side.strokeInset));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
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
        return new _StadiumToCircleBorder__stadium_border(side: side ?? this.side, circularity: circularity ?? this.circularity, eccentricity: eccentricity ?? this.eccentricity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (side.style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    RRect borderRect = _adjustBorderRadius(rect).toRRect(_adjustRect(rect));
                    canvas.drawRRect(borderRect.inflate(side.strokeOffset / 2L), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _StadiumToCircleBorder__stadium_border;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _StadiumToCircleBorder__stadium_border) && Equals(__other.side, side) && (__other.circularity == circularity);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, circularity);
    public override string ToString()
    {
        if (eccentricity != 0.0)
        {
            return $"StadiumBorder({side}, {(circularity * 100L).toStringAsFixed(1L)}% of the way to being a CircleBorder that is {(eccentricity * 100L).toStringAsFixed(1L)}% oval)";
        }
        return $"StadiumBorder({side}, {(circularity * 100L).toStringAsFixed(1L)}% of the way to being a CircleBorder)";
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
        return new _StadiumToRoundedRectangleBorder__stadium_border(side: side.scale(t), borderRadius: borderRadius.op_Multiply(t), rectilinearity: t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is StadiumBorder)
        {
            StadiumBorder a__as10360 = (StadiumBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(a__as10360.side, side, t), borderRadius: borderRadius, rectilinearity: rectilinearity * t);
        }
        if (a is RoundedRectangleBorder)
        {
            RoundedRectangleBorder a__as10580 = (RoundedRectangleBorder)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(a__as10580.side, side, t), borderRadius: borderRadius, rectilinearity: rectilinearity + ((1.0 - rectilinearity) * (1.0 - t)));
        }
        if (a is _StadiumToRoundedRectangleBorder__stadium_border)
        {
            _StadiumToRoundedRectangleBorder__stadium_border a__as10842 = (_StadiumToRoundedRectangleBorder__stadium_border)a;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(a__as10842.side, side, t), borderRadius: BorderRadiusGeometry.lerp(a__as10842.borderRadius, borderRadius, t)!, rectilinearity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(a__as10842.rectilinearity, rectilinearity, t)));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is StadiumBorder)
        {
            StadiumBorder b__as11261 = (StadiumBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, b__as11261.side, t), borderRadius: borderRadius, rectilinearity: rectilinearity * (1.0 - t));
        }
        if (b is RoundedRectangleBorder)
        {
            RoundedRectangleBorder b__as11489 = (RoundedRectangleBorder)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, b__as11489.side, t), borderRadius: borderRadius, rectilinearity: rectilinearity + ((1.0 - rectilinearity) * t));
        }
        if (b is _StadiumToRoundedRectangleBorder__stadium_border)
        {
            _StadiumToRoundedRectangleBorder__stadium_border b__as11743 = (_StadiumToRoundedRectangleBorder__stadium_border)b;
            return new _StadiumToRoundedRectangleBorder__stadium_border(side: BorderSide.lerp(side, b__as11743.side, t), borderRadius: BorderRadiusGeometry.lerp(borderRadius, b__as11743.borderRadius, t)!, rectilinearity: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(rectilinearity, b__as11743.rectilinearity, t)));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadiusGeometry _adjustBorderRadius(Rect rect)
    {
        return BorderRadiusGeometry.lerp(borderRadius, BorderRadius.CreateAll(Radius.circular(rect.shortestSide / 2.0)), 1.0 - rectilinearity)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        RRect borderRect = _adjustBorderRadius(rect).resolve(textDirection).toRRect(rect);
        RRect adjustedRect = borderRect.deflate(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(side.width, 0L, side.strokeAlign)));
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addRRect(adjustedRect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addRRect(_adjustBorderRadius(rect).resolve(textDirection).toRRect(rect));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        BorderRadius adjustedBorderRadius = _adjustBorderRadius(rect).resolve(textDirection);
        if (Equals(adjustedBorderRadius, BorderRadius.zero))
        {
            return rect.contains(position);
        }
        return adjustedBorderRadius.toRRect(rect).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        BorderRadiusGeometry adjustedBorderRadius = _adjustBorderRadius(rect);
        if (Equals(adjustedBorderRadius, BorderRadius.zero))
        {
            canvas.drawRect(rect, paint);
        }
        else
        {
            canvas.drawRRect(adjustedBorderRadius.resolve(textDirection).toRRect(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;
    public override _StadiumToRoundedRectangleBorder__stadium_border copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new _StadiumToRoundedRectangleBorder__stadium_border(side: side ?? this.side, borderRadius: borderRadius ?? this.borderRadius, rectilinearity: rectilinearity ?? this.rectilinearity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        switch (side.style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    BorderRadiusGeometry adjustedBorderRadius = _adjustBorderRadius(rect);
                    RRect borderRect = adjustedBorderRadius.resolve(textDirection).toRRect(rect);
                    canvas.drawRRect(borderRect.inflate(side.strokeOffset / 2L), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as _StadiumToRoundedRectangleBorder__stadium_border;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _StadiumToRoundedRectangleBorder__stadium_border) && Equals(__other.side, side) && Equals(__other.borderRadius, borderRadius) && (__other.rectilinearity == rectilinearity);
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, borderRadius, rectilinearity);
    public override string ToString()
    {
        return $"StadiumBorder({side}, {borderRadius}, " + $"{(rectilinearity * 100L).toStringAsFixed(1L)}% of the way to being a " + "RoundedRectangleBorder)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

