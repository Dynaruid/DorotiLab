// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/star_border.dart
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

public static partial class Star_borderLibrary
{
    internal static double _kRadToDeg = (180L / Dart_mathLibrary.pi);
}

public static partial class Star_borderLibrary
{
    internal static double _kDegToRad = (Dart_mathLibrary.pi / 180L);
}

public class StarBorder : OutlinedBorder
{
    public virtual double points { get; private set; } = default!;
    internal virtual double? _innerRadiusRatio { get; private set; }
    public virtual double pointRounding { get; private set; } = default!;
    public virtual double valleyRounding { get; private set; } = default!;
    internal virtual double _rotationRadians { get; private set; } = default!;
    public virtual double squash { get; private set; } = default!;

    public StarBorder(BorderSide side = default!, double points = 5, double innerRadiusRatio = 0.4, double pointRounding = 0, double valleyRounding = 0, double rotation = 0, double squash = 0) : base(side: side ?? BorderSide.none)
    {
        this.points = points;
        this.pointRounding = pointRounding;
        this.valleyRounding = valleyRounding;
        this.squash = squash;
        this._rotationRadians = (DartRuntimePrimitives.RequireValue(rotation) * Star_borderLibrary._kDegToRad);
        this._innerRadiusRatio = DartRuntimePrimitives.RequireValue(innerRadiusRatio);
        System.Diagnostics.Debug.Assert((squash >= 0L));
        System.Diagnostics.Debug.Assert((squash <= 1L));
        System.Diagnostics.Debug.Assert((pointRounding >= 0L));
        System.Diagnostics.Debug.Assert((pointRounding <= 1L));
        System.Diagnostics.Debug.Assert((valleyRounding >= 0L));
        System.Diagnostics.Debug.Assert((valleyRounding <= 1L));
        System.Diagnostics.Debug.Assert((((valleyRounding + pointRounding)) <= 1L));
        System.Diagnostics.Debug.Assert((innerRadiusRatio >= 0L));
        System.Diagnostics.Debug.Assert((innerRadiusRatio <= 1L));
        System.Diagnostics.Debug.Assert((points >= 2L));
    }

    public static StarBorder CreatePolygon(BorderSide side = default!, double sides = 5, double pointRounding = 0, double rotation = 0, double squash = 0)
    {
        var __instance = new StarBorder(default!, default!, default!, default!, default!, default!, default!);
        __instance.pointRounding = pointRounding;
        __instance.squash = squash;
        __instance.points = sides;
        __instance.valleyRounding = 0;
        __instance._rotationRadians = (DartRuntimePrimitives.RequireValue(rotation) * Star_borderLibrary._kDegToRad);
        __instance._innerRadiusRatio = null;
        return __instance;
    }

    public virtual double innerRadiusRatio
    {
        get
        {
            return (this._innerRadiusRatio ?? global::Doroti.Runtime.Dart_mathLibrary.cos((Dart_mathLibrary.pi / this.points)));
            return default!;
        }
    }
    public virtual double rotation => (this._rotationRadians * Star_borderLibrary._kRadToDeg);
    public override ShapeBorder scale(double t)
    {
        return new StarBorder(points: this.points, side: side.scale(t), rotation: this.rotation, innerRadiusRatio: this.innerRadiusRatio, pointRounding: this.pointRounding, valleyRounding: this.valleyRounding, squash: this.squash);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ShapeBorder? _twoPhaseLerp(double t, double split, Func<double, ShapeBorder?> first, Func<double, ShapeBorder?> second)
    {
        if ((t < split))
        {
            return first((t * ((1L / split))));
        }
        else
        {
            t = (((1L / ((1.0 - split)))) * ((t - split)));
            return second(t);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((t == 0L))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return this;
        }
        if ((a is StarBorder))
        {
            StarBorder a__as7561 = (StarBorder)a;
            return new StarBorder(side: BorderSide.lerp(((StarBorder)a__as7561).side, side, t), points: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561)).points, this.points, t)), rotation: (DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561))._rotationRadians, this._rotationRadians, t)) * Star_borderLibrary._kRadToDeg), innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561)).innerRadiusRatio, this.innerRadiusRatio, t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561)).pointRounding, this.pointRounding, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561)).valleyRounding, this.valleyRounding, t)), squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((StarBorder)((StarBorder)a__as7561)).squash, this.squash, t)));
        }
        if ((a is CircleBorder))
        {
            CircleBorder a__as8105 = (CircleBorder)a;
            if ((this.points >= 2.5))
            {
                double lerpedPoints = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.points.round(), this.points, t));
                return new StarBorder(side: BorderSide.lerp(((CircleBorder)a__as8105).side, side, t), points: lerpedPoints, squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CircleBorder)((CircleBorder)a__as8105)).eccentricity, this.squash, t)), rotation: this.rotation, innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(global::Doroti.Runtime.Dart_mathLibrary.cos((Dart_mathLibrary.pi / lerpedPoints)), this.innerRadiusRatio, t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1.0, this.pointRounding, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, this.valleyRounding, t)));
            }
            else
            {
                double lerpedPointsLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.points, 2L, t));
                return new StarBorder(side: BorderSide.lerp(((CircleBorder)a__as8105).side, side, t), points: lerpedPointsLocal, squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CircleBorder)((CircleBorder)a__as8105)).eccentricity, this.squash, t)), rotation: this.rotation, innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(1L, this.innerRadiusRatio, t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.5, this.pointRounding, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.5, this.valleyRounding, t)));
            }
        }
        if ((a is StadiumBorder))
        {
            StadiumBorder a__as9329 = (StadiumBorder)a;
            BorderSide lerpedSide = BorderSide.lerp(((StadiumBorder)a__as9329).side, side, t);
            return _twoPhaseLerp(t, 0.5, ((Func<double, ShapeBorder?>)((t) => ((StadiumBorder)a__as9329).lerpTo(new CircleBorder(side: lerpedSide), t))), ((Func<double, ShapeBorder?>)((t) => lerpFrom(new CircleBorder(side: lerpedSide), t))));
        }
        if ((a is RoundedRectangleBorder))
        {
            RoundedRectangleBorder a__as9704 = (RoundedRectangleBorder)a;
            BorderSide lerpedSideLocal = BorderSide.lerp(((RoundedRectangleBorder)a__as9704).side, side, t);
            return _twoPhaseLerp(t, (1L / 3L), ((Func<double, ShapeBorder?>)((t) =>
            {
                return new StadiumBorder(side: lerpedSideLocal).lerpFrom(((RoundedRectangleBorder)a__as9704), t);
                return default;
            })), ((Func<double, ShapeBorder?>)((t) =>
            {
                return _twoPhaseLerp(t, 0.5, ((Func<double, ShapeBorder?>)((t) => new StadiumBorder(side: lerpedSideLocal).lerpTo(new CircleBorder(side: lerpedSideLocal), t))), ((Func<double, ShapeBorder?>)((t) => lerpFrom(new CircleBorder(side: lerpedSideLocal), t))));
                return default;
            })));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((t == 0L))
        {
            return this;
        }
        if ((t == 1.0))
        {
            return b;
        }
        if ((b is StarBorder))
        {
            StarBorder b__as10562 = (StarBorder)b;
            return new StarBorder(side: BorderSide.lerp(side, ((StarBorder)b__as10562).side, t), points: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.points, ((StarBorder)((StarBorder)b__as10562)).points, t)), rotation: (DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this._rotationRadians, ((StarBorder)((StarBorder)b__as10562))._rotationRadians, t)) * Star_borderLibrary._kRadToDeg), innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.innerRadiusRatio, ((StarBorder)((StarBorder)b__as10562)).innerRadiusRatio, t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.pointRounding, ((StarBorder)((StarBorder)b__as10562)).pointRounding, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.valleyRounding, ((StarBorder)((StarBorder)b__as10562)).valleyRounding, t)), squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.squash, ((StarBorder)((StarBorder)b__as10562)).squash, t)));
        }
        if ((b is CircleBorder))
        {
            CircleBorder b__as11105 = (CircleBorder)b;
            if ((this.points >= 2.5))
            {
                double lerpedPoints = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.points, this.points.round(), t));
                return new StarBorder(side: BorderSide.lerp(side, ((CircleBorder)b__as11105).side, t), points: lerpedPoints, squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.squash, ((CircleBorder)((CircleBorder)b__as11105)).eccentricity, t)), rotation: this.rotation, innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.innerRadiusRatio, global::Doroti.Runtime.Dart_mathLibrary.cos((Dart_mathLibrary.pi / lerpedPoints)), t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.pointRounding, 1.0, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.valleyRounding, 0.0, t)));
            }
            else
            {
                double lerpedPointsLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.points, 2L, t));
                return new StarBorder(side: BorderSide.lerp(side, ((CircleBorder)b__as11105).side, t), points: lerpedPointsLocal, squash: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.squash, ((CircleBorder)((CircleBorder)b__as11105)).eccentricity, t)), rotation: this.rotation, innerRadiusRatio: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.innerRadiusRatio, 1L, t)), pointRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.pointRounding, 0.5, t)), valleyRounding: DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.valleyRounding, 0.5, t)));
            }
        }
        if ((b is StadiumBorder))
        {
            StadiumBorder b__as12324 = (StadiumBorder)b;
            BorderSide lerpedSide = BorderSide.lerp(side, ((StadiumBorder)b__as12324).side, t);
            return _twoPhaseLerp(t, 0.5, ((Func<double, ShapeBorder?>)((t) => lerpTo(new CircleBorder(side: lerpedSide), t))), ((Func<double, ShapeBorder?>)((t) => ((StadiumBorder)b__as12324).lerpFrom(new CircleBorder(side: lerpedSide), t))));
        }
        if ((b is RoundedRectangleBorder))
        {
            RoundedRectangleBorder b__as12677 = (RoundedRectangleBorder)b;
            BorderSide lerpedSideLocal = BorderSide.lerp(side, ((RoundedRectangleBorder)b__as12677).side, t);
            return _twoPhaseLerp(t, (2L / 3L), ((Func<double, ShapeBorder?>)((t) =>
            {
                return _twoPhaseLerp(t, 0.5, ((Func<double, ShapeBorder?>)((t) => lerpTo(new CircleBorder(side: lerpedSideLocal), t))), ((Func<double, ShapeBorder?>)((t) => new StadiumBorder(side: lerpedSideLocal).lerpFrom(new CircleBorder(side: lerpedSideLocal), t))));
                return default;
            })), ((Func<double, ShapeBorder?>)((t) =>
            {
                return new StadiumBorder(side: lerpedSideLocal).lerpTo(((RoundedRectangleBorder)b__as12677), t);
                return default;
            })));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override StarBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new StarBorder(side: (side ?? this.side), points: (points ?? this.points), rotation: (rotation ?? this.rotation), innerRadiusRatio: (innerRadiusRatio ?? this.innerRadiusRatio), pointRounding: (pointRounding ?? this.pointRounding), valleyRounding: (valleyRounding ?? this.valleyRounding), squash: (squash ?? this.squash));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.Rect adjustedRect = rect.deflate(((BorderSide)side).strokeInset);
        return new _StarGenerator__star_border(points: this.points, rotation: this._rotationRadians, innerRadiusRatio: this.innerRadiusRatio, pointRounding: this.pointRounding, valleyRounding: this.valleyRounding, squash: this.squash).generate(adjustedRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return new _StarGenerator__star_border(points: this.points, rotation: this._rotationRadians, innerRadiusRatio: this.innerRadiusRatio, pointRounding: this.pointRounding, valleyRounding: this.valleyRounding, squash: this.squash).generate(rect);
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
                    global::Doroti.Ui.Rect adjustedRect = rect.inflate((((BorderSide)side).strokeOffset / 2L));
                    global::Doroti.Ui.Path path = new _StarGenerator__star_border(points: this.points, rotation: this._rotationRadians, innerRadiusRatio: this.innerRadiusRatio, pointRounding: this.pointRounding, valleyRounding: this.valleyRounding, squash: this.squash).generate(adjustedRect);
                    canvas.drawPath(path, side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as StarBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is StarBorder) && (object.Equals(((StarBorder)__other).side, side))) && (((StarBorder)((StarBorder)__other)).points == this.points)) && (((StarBorder)((StarBorder)__other))._innerRadiusRatio == this._innerRadiusRatio)) && (((StarBorder)((StarBorder)__other)).pointRounding == this.pointRounding)) && (((StarBorder)((StarBorder)__other)).valleyRounding == this.valleyRounding)) && (((StarBorder)((StarBorder)__other))._rotationRadians == this._rotationRadians)) && (((StarBorder)((StarBorder)__other)).squash == this.squash));
    }

    public override int GetHashCode() => side.GetHashCode();
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "StarBorder"))}({side}, points: {this.points}, innerRadiusRatio: {this.innerRadiusRatio})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PointInfo__star_border
{
    public virtual Offset valley { get; set; } = default!;
    public virtual Offset point { get; set; } = default!;
    public virtual Offset valleyArc1 { get; set; } = default!;
    public virtual Offset pointArc1 { get; set; } = default!;
    public virtual Offset pointArc2 { get; set; } = default!;
    public virtual Offset valleyArc2 { get; set; } = default!;

    internal _PointInfo__star_border(Offset valley, Offset point, Offset valleyArc1, Offset pointArc1, Offset valleyArc2, Offset pointArc2)
    {
        this.valley = valley;
        this.point = point;
        this.valleyArc1 = valleyArc1;
        this.pointArc1 = pointArc1;
        this.valleyArc2 = valleyArc2;
        this.pointArc2 = pointArc2;
    }

}

internal class _StarGenerator__star_border
{
    public virtual double points { get; private set; } = default!;
    public virtual double innerRadiusRatio { get; private set; } = default!;
    public virtual double pointRounding { get; private set; } = default!;
    public virtual double valleyRounding { get; private set; } = default!;
    public virtual double rotation { get; private set; } = default!;
    public virtual double squash { get; private set; } = default!;

    internal _StarGenerator__star_border(double points, double innerRadiusRatio, double pointRounding, double valleyRounding, double rotation, double squash)
    {
        this.points = points;
        this.innerRadiusRatio = innerRadiusRatio;
        this.pointRounding = pointRounding;
        this.valleyRounding = valleyRounding;
        this.rotation = rotation;
        this.squash = squash;
        System.Diagnostics.Debug.Assert((points > 1L));
        System.Diagnostics.Debug.Assert((innerRadiusRatio <= 1L));
        System.Diagnostics.Debug.Assert((innerRadiusRatio >= 0L));
        System.Diagnostics.Debug.Assert((squash >= 0L));
        System.Diagnostics.Debug.Assert((squash <= 1L));
        System.Diagnostics.Debug.Assert((pointRounding >= 0L));
        System.Diagnostics.Debug.Assert((pointRounding <= 1L));
        System.Diagnostics.Debug.Assert((valleyRounding >= 0L));
        System.Diagnostics.Debug.Assert((valleyRounding <= 1L));
        System.Diagnostics.Debug.Assert(((pointRounding + valleyRounding) <= 1L));
    }

    public virtual global::Doroti.Ui.Path generate(Rect rect)
    {
        double radiusLocal = (rect.shortestSide / 2L);
        global::Doroti.Ui.Offset centerLocal = rect.center;
        var minInnerRadiusRatio = 0.002;
        double mappedInnerRadiusRatio = (((this.innerRadiusRatio * ((1.0 - minInnerRadiusRatio)))) + minInnerRadiusRatio);
        var points = new List<_PointInfo__star_border>();
        double maxDiameter = (2.0 * _generatePoints(pointList: points, center: centerLocal, radius: radiusLocal, innerRadius: (radiusLocal * mappedInnerRadiusRatio)));
        var path = new global::Doroti.Ui.Path();
        _drawPoints(path, points);
        var scale = new global::Doroti.Ui.Offset((rect.width / maxDiameter), (rect.height / maxDiameter));
        if ((rect.shortestSide == rect.width))
        {
            scale = new global::Doroti.Ui.Offset(scale.dx, ((this.squash * scale.dy) + (((1L - this.squash)) * scale.dx)));
        }
        else
        {
            scale = new global::Doroti.Ui.Offset(((this.squash * scale.dx) + (((1L - this.squash)) * scale.dy)), scale.dy);
        }
        var squashMatrix = Matrix4.translationValues(rect.center.dx, rect.center.dy, 0);
        squashMatrix.multiply(Matrix4.diagonal3Values(scale.dx, scale.dy, 1));
        squashMatrix.multiply(Matrix4.rotationZ(this.rotation));
        squashMatrix.multiply(Matrix4.translationValues(-rect.center.dx, -rect.center.dy, 0));
        return path.transform(squashMatrix.storage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _generatePoints(List<_PointInfo__star_border> pointList, Offset center, double radius, double innerRadius)
    {
        double step = (Dart_mathLibrary.pi / this.points);
        double angle = ((-Dart_mathLibrary.pi / 2L) - step);
        var valleyLocal = new global::Doroti.Ui.Offset((center.dx + (global::Doroti.Runtime.Dart_mathLibrary.cos(angle) * innerRadius)), (center.dy + (global::Doroti.Runtime.Dart_mathLibrary.sin(angle) * innerRadius)));
        Offset getCurveMidpoint(Offset a, Offset b, Offset c, Offset a1, Offset c1)
        {
            double angleLocal = _getAngle(a, b, c);
            double w = (_getWeight(angleLocal) / 2L);
            return (((((a1 / 4) + (b * w)) + (c1 / 4))) / ((0.5 + w)));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double addPoint(double pointAngle, double pointStep, double pointRadius, double pointInnerRadius)
        {
            pointAngle += pointStep;
            var pointLocal = new global::Doroti.Ui.Offset((center.dx + (global::Doroti.Runtime.Dart_mathLibrary.cos(pointAngle) * pointRadius)), (center.dy + (global::Doroti.Runtime.Dart_mathLibrary.sin(pointAngle) * pointRadius)));
            pointAngle += pointStep;
            var nextValley = new global::Doroti.Ui.Offset((center.dx + (global::Doroti.Runtime.Dart_mathLibrary.cos(pointAngle) * pointInnerRadius)), (center.dy + (global::Doroti.Runtime.Dart_mathLibrary.sin(pointAngle) * pointInnerRadius)));
            global::Doroti.Ui.Offset valleyArc1Local = (valleyLocal + (((pointLocal - valleyLocal)) * this.valleyRounding));
            global::Doroti.Ui.Offset pointArc1Local = (pointLocal + (((valleyLocal - pointLocal)) * this.pointRounding));
            global::Doroti.Ui.Offset pointArc2Local = (pointLocal + (((nextValley - pointLocal)) * this.pointRounding));
            global::Doroti.Ui.Offset valleyArc2Local = (nextValley + (((pointLocal - nextValley)) * this.valleyRounding));
            pointList.Add(new _PointInfo__star_border(valley: valleyLocal, point: pointLocal, valleyArc1: valleyArc1Local, pointArc1: pointArc1Local, pointArc2: pointArc2Local, valleyArc2: valleyArc2Local));
            valleyLocal = nextValley;
            return pointAngle;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double remainder = (this.points - this.points.truncateToDouble());
        bool hasIntegerSides = (remainder < 0.000001);
        double wholeSides = (this.points - ((hasIntegerSides ? 0L : 1L)));
        for (var i = 0L; (i < wholeSides); i += 1L)
        {
            angle = addPoint(angle, step, radius, innerRadius);
        }
        double valleyRadius = 0;
        double pointRadiusLocal = 0;
        _PointInfo__star_border thisPoint = pointList[(int)(0L)];
        _PointInfo__star_border nextPoint = pointList[(int)(1L)];
        global::Doroti.Ui.Offset pointMidpoint = getCurveMidpoint(((_PointInfo__star_border)thisPoint).valley, ((_PointInfo__star_border)thisPoint).point, ((_PointInfo__star_border)nextPoint).valley, ((_PointInfo__star_border)thisPoint).pointArc1, ((_PointInfo__star_border)thisPoint).pointArc2);
        global::Doroti.Ui.Offset valleyMidpoint = getCurveMidpoint(((_PointInfo__star_border)thisPoint).point, ((_PointInfo__star_border)nextPoint).valley, ((_PointInfo__star_border)nextPoint).point, ((_PointInfo__star_border)thisPoint).valleyArc2, ((_PointInfo__star_border)nextPoint).valleyArc1);
        valleyRadius = ((valleyMidpoint - center)).distance;
        pointRadiusLocal = ((pointMidpoint - center)).distance;
        if (!hasIntegerSides)
        {
            double effectiveInnerRadius = Math.Max(valleyRadius, innerRadius);
            double endingRadius = (effectiveInnerRadius + (remainder * ((radius - effectiveInnerRadius))));
            addPoint(angle, (step * remainder), endingRadius, innerRadius);
        }
        return Dart_uiLibrary.clampDouble(Math.Max(valleyRadius, pointRadiusLocal), double.Epsilon, double.MaxValue);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawPoints(Path path, List<_PointInfo__star_border> points)
    {
        global::Doroti.Ui.Offset startingPoint = points.First().pointArc1;
        path.moveTo(startingPoint.dx, startingPoint.dy);
        double pointAngle = _getAngle(points[(int)(0L)].valley, points[(int)(0L)].point, points[(int)(1L)].valley);
        double pointWeight = _getWeight(pointAngle);
        double valleyAngle = _getAngle(points[(int)(1L)].point, points[(int)(1L)].valley, points[(int)(0L)].point);
        double valleyWeight = _getWeight(valleyAngle);
        for (var i = 0L; (i < checked((long)(points.Count))); i += 1L)
        {
            _PointInfo__star_border pointLocal = points[(int)(i)];
            _PointInfo__star_border nextPoint = points[(int)((((i + 1L)) % checked((long)(points.Count))))];
            path.lineTo(((_PointInfo__star_border)pointLocal).pointArc1.dx, ((_PointInfo__star_border)pointLocal).pointArc1.dy);
            if (((pointAngle != 180L) && (pointAngle != 0L)))
            {
                path.conicTo(((_PointInfo__star_border)pointLocal).point.dx, ((_PointInfo__star_border)pointLocal).point.dy, ((_PointInfo__star_border)pointLocal).pointArc2.dx, ((_PointInfo__star_border)pointLocal).pointArc2.dy, pointWeight);
            }
            else
            {
                path.lineTo(((_PointInfo__star_border)pointLocal).pointArc2.dx, ((_PointInfo__star_border)pointLocal).pointArc2.dy);
            }
            path.lineTo(((_PointInfo__star_border)pointLocal).valleyArc2.dx, ((_PointInfo__star_border)pointLocal).valleyArc2.dy);
            if (((valleyAngle != 180L) && (valleyAngle != 0L)))
            {
                path.conicTo(((_PointInfo__star_border)nextPoint).valley.dx, ((_PointInfo__star_border)nextPoint).valley.dy, ((_PointInfo__star_border)nextPoint).valleyArc1.dx, ((_PointInfo__star_border)nextPoint).valleyArc1.dy, valleyWeight);
            }
            else
            {
                path.lineTo(((_PointInfo__star_border)nextPoint).valleyArc1.dx, ((_PointInfo__star_border)nextPoint).valleyArc1.dy);
            }
        }
        path.close();
    }

    internal virtual double _getWeight(double angle)
    {
        return global::Doroti.Runtime.Dart_mathLibrary.cos((((angle / 2L)) % ((Dart_mathLibrary.pi / 2L))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getAngle(Offset a, Offset b, Offset c)
    {
        if ((((object.Equals(a, c)) || (object.Equals(b, c))) || (object.Equals(b, a))))
        {
            return 0;
        }
        global::Doroti.Ui.Offset u = (a - b);
        global::Doroti.Ui.Offset v = (c - b);
        double dot = ((u.dx * v.dx) + (u.dy * v.dy));
        double m1 = ((b.dx == a.dx) ? double.PositiveInfinity : (-u.dy / -u.dx));
        double m2 = ((b.dx == c.dx) ? double.PositiveInfinity : (-v.dy / -v.dx));
        double angle = global::Doroti.Runtime.Dart_mathLibrary.atan2((m1 - m2), (1L + (m1 * m2))).abs();
        if ((dot < 0L))
        {
            angle += Dart_mathLibrary.pi;
        }
        return angle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

