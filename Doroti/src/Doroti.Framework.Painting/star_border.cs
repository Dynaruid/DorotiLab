// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/star_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class Star_borderLibrary
{
    internal static double _kRadToDeg = 180L / Dart_mathLibrary.pi;
}

public static partial class Star_borderLibrary
{
    internal static double _kDegToRad = Dart_mathLibrary.pi / 180L;
}

public class StarBorder : OutlinedBorder
{
    public virtual double points { get; private set; } = default!;
    internal virtual double? _innerRadiusRatio { get; private set; }
    public virtual double pointRounding { get; private set; } = default!;
    public virtual double valleyRounding { get; private set; } = default!;
    internal virtual double _rotationRadians { get; private set; } = default!;
    public virtual double squash { get; private set; } = default!;

    public StarBorder(
        BorderSide side = default!,
        double points = 5,
        double innerRadiusRatio = 0.4,
        double pointRounding = 0,
        double valleyRounding = 0,
        double rotation = 0,
        double squash = 0
    )
        : base(side: side ?? BorderSide.none)
    {
        this.points = points;
        this.pointRounding = pointRounding;
        this.valleyRounding = valleyRounding;
        this.squash = squash;
        _rotationRadians = (rotation) * Star_borderLibrary._kDegToRad;
        _innerRadiusRatio = (innerRadiusRatio);
        System.Diagnostics.Debug.Assert(squash >= 0L);
        System.Diagnostics.Debug.Assert(squash <= 1L);
        System.Diagnostics.Debug.Assert(pointRounding >= 0L);
        System.Diagnostics.Debug.Assert(pointRounding <= 1L);
        System.Diagnostics.Debug.Assert(valleyRounding >= 0L);
        System.Diagnostics.Debug.Assert(valleyRounding <= 1L);
        System.Diagnostics.Debug.Assert(valleyRounding + pointRounding <= 1L);
        System.Diagnostics.Debug.Assert(innerRadiusRatio >= 0L);
        System.Diagnostics.Debug.Assert(innerRadiusRatio <= 1L);
        System.Diagnostics.Debug.Assert(points >= 2L);
    }

    public static StarBorder CreatePolygon(
        BorderSide side = default!,
        double sides = 5,
        double pointRounding = 0,
        double rotation = 0,
        double squash = 0
    )
    {
        var __instance = new StarBorder(
            side,
            default!,
            default!,
            pointRounding,
            default!,
            rotation,
            squash
        );
        __instance.pointRounding = pointRounding;
        __instance.squash = squash;
        __instance.points = sides;
        __instance.valleyRounding = 0;
        __instance._rotationRadians = (rotation) * Star_borderLibrary._kDegToRad;
        __instance._innerRadiusRatio = null;
        return __instance;
    }

    public virtual double innerRadiusRatio
    {
        get { return _innerRadiusRatio ?? Dart_mathLibrary.cos(Dart_mathLibrary.pi / points); }
    }
    public virtual double rotation => _rotationRadians * Star_borderLibrary._kRadToDeg;

    public override ShapeBorder scale(double t)
    {
        return new StarBorder(
            points: points,
            side: side.scale(t),
            rotation: rotation,
            innerRadiusRatio: innerRadiusRatio,
            pointRounding: pointRounding,
            valleyRounding: valleyRounding,
            squash: squash
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ShapeBorder? _twoPhaseLerp(
        double t,
        double split,
        Func<double, ShapeBorder?> first,
        Func<double, ShapeBorder?> second
    )
    {
        if (t < split)
        {
            return first(t * (1L / split));
        }
        else
        {
            t = 1L / (1.0 - split) * (t - split);
            return second(t);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (t == 0L)
        {
            return a;
        }
        if (t == 1.0)
        {
            return this;
        }
        if (a is StarBorder)
        {
            StarBorder a__as7561 = (StarBorder)a;
            return new StarBorder(
                side: BorderSide.lerp(a__as7561.side, side, t),
                points: (
                    Dart_uiLibrary.lerpDouble(a__as7561.points, points, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                rotation: (
                    Dart_uiLibrary.lerpDouble(a__as7561._rotationRadians, _rotationRadians, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) * Star_borderLibrary._kRadToDeg,
                innerRadiusRatio: (
                    Dart_uiLibrary.lerpDouble(a__as7561.innerRadiusRatio, innerRadiusRatio, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                pointRounding: (
                    Dart_uiLibrary.lerpDouble(a__as7561.pointRounding, pointRounding, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                valleyRounding: (
                    Dart_uiLibrary.lerpDouble(a__as7561.valleyRounding, valleyRounding, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                squash: (
                    Dart_uiLibrary.lerpDouble(a__as7561.squash, squash, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        if (a is CircleBorder)
        {
            CircleBorder a__as8105 = (CircleBorder)a;
            if (points >= 2.5)
            {
                double lerpedPoints = (
                    Dart_uiLibrary.lerpDouble(points.round(), points, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                return new StarBorder(
                    side: BorderSide.lerp(a__as8105.side, side, t),
                    points: lerpedPoints,
                    squash: (
                        Dart_uiLibrary.lerpDouble(a__as8105.eccentricity, squash, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    rotation: rotation,
                    innerRadiusRatio: (
                        Dart_uiLibrary.lerpDouble(
                            Dart_mathLibrary.cos(Dart_mathLibrary.pi / lerpedPoints),
                            innerRadiusRatio,
                            t
                        )
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    pointRounding: (
                        Dart_uiLibrary.lerpDouble(1.0, pointRounding, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    valleyRounding: (
                        Dart_uiLibrary.lerpDouble(0.0, valleyRounding, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
            }
            else
            {
                double lerpedPointsLocal = (
                    Dart_uiLibrary.lerpDouble(points, 2L, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                return new StarBorder(
                    side: BorderSide.lerp(a__as8105.side, side, t),
                    points: lerpedPointsLocal,
                    squash: (
                        Dart_uiLibrary.lerpDouble(a__as8105.eccentricity, squash, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    rotation: rotation,
                    innerRadiusRatio: (
                        Dart_uiLibrary.lerpDouble(1L, innerRadiusRatio, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    pointRounding: (
                        Dart_uiLibrary.lerpDouble(0.5, pointRounding, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    valleyRounding: (
                        Dart_uiLibrary.lerpDouble(0.5, valleyRounding, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
            }
        }
        if (a is StadiumBorder)
        {
            StadiumBorder a__as9329 = (StadiumBorder)a;
            BorderSide lerpedSide = BorderSide.lerp(a__as9329.side, side, t);
            return _twoPhaseLerp(
                t,
                0.5,
                (t) => a__as9329.lerpTo(new CircleBorder(side: lerpedSide), t),
                (t) => lerpFrom(new CircleBorder(side: lerpedSide), t)
            );
        }
        if (a is RoundedRectangleBorder)
        {
            RoundedRectangleBorder a__as9704 = (RoundedRectangleBorder)a;
            BorderSide lerpedSideLocal = BorderSide.lerp(a__as9704.side, side, t);
            return _twoPhaseLerp(
                t,
                1L / 3L,
                (t) =>
                {
                    return new StadiumBorder(side: lerpedSideLocal).lerpFrom(a__as9704, t);
                },
                (t) =>
                {
                    return _twoPhaseLerp(
                        t,
                        0.5,
                        (t) =>
                            new StadiumBorder(side: lerpedSideLocal).lerpTo(
                                new CircleBorder(side: lerpedSideLocal),
                                t
                            ),
                        (t) => lerpFrom(new CircleBorder(side: lerpedSideLocal), t)
                    );
                }
            );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (t == 0L)
        {
            return this;
        }
        if (t == 1.0)
        {
            return b;
        }
        if (b is StarBorder)
        {
            StarBorder b__as10562 = (StarBorder)b;
            return new StarBorder(
                side: BorderSide.lerp(side, b__as10562.side, t),
                points: (
                    Dart_uiLibrary.lerpDouble(points, b__as10562.points, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                rotation: (
                    Dart_uiLibrary.lerpDouble(_rotationRadians, b__as10562._rotationRadians, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) * Star_borderLibrary._kRadToDeg,
                innerRadiusRatio: (
                    Dart_uiLibrary.lerpDouble(innerRadiusRatio, b__as10562.innerRadiusRatio, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                pointRounding: (
                    Dart_uiLibrary.lerpDouble(pointRounding, b__as10562.pointRounding, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                valleyRounding: (
                    Dart_uiLibrary.lerpDouble(valleyRounding, b__as10562.valleyRounding, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                squash: (
                    Dart_uiLibrary.lerpDouble(squash, b__as10562.squash, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        }
        if (b is CircleBorder)
        {
            CircleBorder b__as11105 = (CircleBorder)b;
            if (points >= 2.5)
            {
                double lerpedPoints = (
                    Dart_uiLibrary.lerpDouble(points, points.round(), t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                return new StarBorder(
                    side: BorderSide.lerp(side, b__as11105.side, t),
                    points: lerpedPoints,
                    squash: (
                        Dart_uiLibrary.lerpDouble(squash, b__as11105.eccentricity, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    rotation: rotation,
                    innerRadiusRatio: (
                        Dart_uiLibrary.lerpDouble(
                            innerRadiusRatio,
                            Dart_mathLibrary.cos(Dart_mathLibrary.pi / lerpedPoints),
                            t
                        )
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    pointRounding: (
                        Dart_uiLibrary.lerpDouble(pointRounding, 1.0, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    valleyRounding: (
                        Dart_uiLibrary.lerpDouble(valleyRounding, 0.0, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
            }
            else
            {
                double lerpedPointsLocal = (
                    Dart_uiLibrary.lerpDouble(points, 2L, t)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                return new StarBorder(
                    side: BorderSide.lerp(side, b__as11105.side, t),
                    points: lerpedPointsLocal,
                    squash: (
                        Dart_uiLibrary.lerpDouble(squash, b__as11105.eccentricity, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    rotation: rotation,
                    innerRadiusRatio: (
                        Dart_uiLibrary.lerpDouble(innerRadiusRatio, 1L, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    pointRounding: (
                        Dart_uiLibrary.lerpDouble(pointRounding, 0.5, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ),
                    valleyRounding: (
                        Dart_uiLibrary.lerpDouble(valleyRounding, 0.5, t)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
            }
        }
        if (b is StadiumBorder)
        {
            StadiumBorder b__as12324 = (StadiumBorder)b;
            BorderSide lerpedSide = BorderSide.lerp(side, b__as12324.side, t);
            return _twoPhaseLerp(
                t,
                0.5,
                (t) => lerpTo(new CircleBorder(side: lerpedSide), t),
                (t) => b__as12324.lerpFrom(new CircleBorder(side: lerpedSide), t)
            );
        }
        if (b is RoundedRectangleBorder)
        {
            RoundedRectangleBorder b__as12677 = (RoundedRectangleBorder)b;
            BorderSide lerpedSideLocal = BorderSide.lerp(side, b__as12677.side, t);
            return _twoPhaseLerp(
                t,
                2L / 3L,
                (t) =>
                {
                    return _twoPhaseLerp(
                        t,
                        0.5,
                        (t) => lerpTo(new CircleBorder(side: lerpedSideLocal), t),
                        (t) =>
                            new StadiumBorder(side: lerpedSideLocal).lerpFrom(
                                new CircleBorder(side: lerpedSideLocal),
                                t
                            )
                    );
                },
                (t) =>
                {
                    return new StadiumBorder(side: lerpedSideLocal).lerpTo(b__as12677, t);
                }
            );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override StarBorder copyWith(
        BorderSide? side = null,
        BorderRadiusGeometry? borderRadius = null,
        double? eccentricity = null,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null,
        double? circularity = null,
        double? rectilinearity = null,
        double? points = null,
        double? innerRadiusRatio = null,
        double? pointRounding = null,
        double? valleyRounding = null,
        double? rotation = null,
        double? squash = null
    )
    {
        return new StarBorder(
            side: side ?? this.side,
            points: points ?? this.points,
            rotation: rotation ?? this.rotation,
            innerRadiusRatio: innerRadiusRatio ?? this.innerRadiusRatio,
            pointRounding: pointRounding ?? this.pointRounding,
            valleyRounding: valleyRounding ?? this.valleyRounding,
            squash: squash ?? this.squash
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        Rect adjustedRect = rect.deflate(side.strokeInset);
        return new _StarGenerator__star_border(
            points: points,
            rotation: _rotationRadians,
            innerRadiusRatio: innerRadiusRatio,
            pointRounding: pointRounding,
            valleyRounding: valleyRounding,
            squash: squash
        ).generate(adjustedRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return new _StarGenerator__star_border(
            points: points,
            rotation: _rotationRadians,
            innerRadiusRatio: innerRadiusRatio,
            pointRounding: pointRounding,
            valleyRounding: valleyRounding,
            squash: squash
        ).generate(rect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        switch (side.style)
        {
            case BorderStyle.none:
            {
                break;
            }
            case BorderStyle.solid:
            {
                Rect adjustedRect = rect.inflate(side.strokeOffset / 2L);
                Path path = new _StarGenerator__star_border(
                    points: points,
                    rotation: _rotationRadians,
                    innerRadiusRatio: innerRadiusRatio,
                    pointRounding: pointRounding,
                    valleyRounding: valleyRounding,
                    squash: squash
                ).generate(adjustedRect);
                canvas.drawPath(path, side.toPaint());
                break;
            }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as StarBorder;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is StarBorder)
            && Equals(__other.side, side)
            && (__other.points == points)
            && (__other._innerRadiusRatio == _innerRadiusRatio)
            && (__other.pointRounding == pointRounding)
            && (__other.valleyRounding == valleyRounding)
            && (__other._rotationRadians == _rotationRadians)
            && (__other.squash == squash);
    }

    public override int GetHashCode() => side.GetHashCode();

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "StarBorder")}({side}, points: {points}, innerRadiusRatio: {innerRadiusRatio})";
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

    internal _PointInfo__star_border(
        Offset valley,
        Offset point,
        Offset valleyArc1,
        Offset pointArc1,
        Offset valleyArc2,
        Offset pointArc2
    )
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

    internal _StarGenerator__star_border(
        double points,
        double innerRadiusRatio,
        double pointRounding,
        double valleyRounding,
        double rotation,
        double squash
    )
    {
        this.points = points;
        this.innerRadiusRatio = innerRadiusRatio;
        this.pointRounding = pointRounding;
        this.valleyRounding = valleyRounding;
        this.rotation = rotation;
        this.squash = squash;
        System.Diagnostics.Debug.Assert(points > 1L);
        System.Diagnostics.Debug.Assert(innerRadiusRatio <= 1L);
        System.Diagnostics.Debug.Assert(innerRadiusRatio >= 0L);
        System.Diagnostics.Debug.Assert(squash >= 0L);
        System.Diagnostics.Debug.Assert(squash <= 1L);
        System.Diagnostics.Debug.Assert(pointRounding >= 0L);
        System.Diagnostics.Debug.Assert(pointRounding <= 1L);
        System.Diagnostics.Debug.Assert(valleyRounding >= 0L);
        System.Diagnostics.Debug.Assert(valleyRounding <= 1L);
        System.Diagnostics.Debug.Assert((pointRounding + valleyRounding) <= 1L);
    }

    public virtual Path generate(Rect rect)
    {
        double radiusLocal = rect.shortestSide / 2L;
        Offset centerLocal = rect.center;
        var minInnerRadiusRatio = 0.002;
        double mappedInnerRadiusRatio =
            (innerRadiusRatio * (1.0 - minInnerRadiusRatio)) + minInnerRadiusRatio;
        var points = new List<_PointInfo__star_border>();
        double maxDiameter =
            2.0
            * _generatePoints(
                pointList: points,
                center: centerLocal,
                radius: radiusLocal,
                innerRadius: radiusLocal * mappedInnerRadiusRatio
            );
        var path = new Path();
        _drawPoints(path, points);
        var scale = new Offset(rect.width / maxDiameter, rect.height / maxDiameter);
        if (rect.shortestSide == rect.width)
        {
            scale = new Offset(scale.dx, (squash * scale.dy) + ((1L - squash) * scale.dx));
        }
        else
        {
            scale = new Offset((squash * scale.dx) + ((1L - squash) * scale.dy), scale.dy);
        }
        var squashMatrix = Matrix4.translationValues(rect.center.dx, rect.center.dy, 0);
        squashMatrix.multiply(Matrix4.diagonal3Values(scale.dx, scale.dy, 1));
        squashMatrix.multiply(Matrix4.rotationZ(rotation));
        squashMatrix.multiply(Matrix4.translationValues(-rect.center.dx, -rect.center.dy, 0));
        return path.transform(squashMatrix.storage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _generatePoints(
        List<_PointInfo__star_border> pointList,
        Offset center,
        double radius,
        double innerRadius
    )
    {
        double step = Dart_mathLibrary.pi / points;
        double angle = (-Dart_mathLibrary.pi / 2L) - step;
        var valleyLocal = new Offset(
            center.dx + (Dart_mathLibrary.cos(angle) * innerRadius),
            center.dy + (Dart_mathLibrary.sin(angle) * innerRadius)
        );
        Offset getCurveMidpoint(Offset a, Offset b, Offset c, Offset a1, Offset c1)
        {
            double angleLocal = _getAngle(a, b, c);
            double w = _getWeight(angleLocal) / 2L;
            return ((a1 / 4) + (b * w) + (c1 / 4)) / (0.5 + w);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double addPoint(
            double pointAngle,
            double pointStep,
            double pointRadius,
            double pointInnerRadius
        )
        {
            pointAngle += pointStep;
            var pointLocal = new Offset(
                center.dx + (Dart_mathLibrary.cos(pointAngle) * pointRadius),
                center.dy + (Dart_mathLibrary.sin(pointAngle) * pointRadius)
            );
            pointAngle += pointStep;
            var nextValley = new Offset(
                center.dx + (Dart_mathLibrary.cos(pointAngle) * pointInnerRadius),
                center.dy + (Dart_mathLibrary.sin(pointAngle) * pointInnerRadius)
            );
            Offset valleyArc1Local = valleyLocal + ((pointLocal - valleyLocal) * valleyRounding);
            Offset pointArc1Local = pointLocal + ((valleyLocal - pointLocal) * pointRounding);
            Offset pointArc2Local = pointLocal + ((nextValley - pointLocal) * pointRounding);
            Offset valleyArc2Local = nextValley + ((pointLocal - nextValley) * valleyRounding);
            pointList.Add(
                new _PointInfo__star_border(
                    valley: valleyLocal,
                    point: pointLocal,
                    valleyArc1: valleyArc1Local,
                    pointArc1: pointArc1Local,
                    pointArc2: pointArc2Local,
                    valleyArc2: valleyArc2Local
                )
            );
            valleyLocal = nextValley;
            return pointAngle;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double remainder = points - points.truncateToDouble();
        bool hasIntegerSides = remainder < 0.000001;
        double wholeSides = points - (hasIntegerSides ? 0L : 1L);
        for (var i = 0L; i < wholeSides; i += 1L)
        {
            angle = addPoint(angle, step, radius, innerRadius);
        }
        double valleyRadius = 0;
        double pointRadiusLocal = 0;
        _PointInfo__star_border thisPoint = pointList[(int)0L];
        _PointInfo__star_border nextPoint = pointList[(int)1L];
        Offset pointMidpoint = getCurveMidpoint(
            thisPoint.valley,
            thisPoint.point,
            nextPoint.valley,
            thisPoint.pointArc1,
            thisPoint.pointArc2
        );
        Offset valleyMidpoint = getCurveMidpoint(
            thisPoint.point,
            nextPoint.valley,
            nextPoint.point,
            thisPoint.valleyArc2,
            nextPoint.valleyArc1
        );
        valleyRadius = (valleyMidpoint - center).distance;
        pointRadiusLocal = (pointMidpoint - center).distance;
        if (!hasIntegerSides)
        {
            double effectiveInnerRadius = Math.Max(valleyRadius, innerRadius);
            double endingRadius =
                effectiveInnerRadius + (remainder * (radius - effectiveInnerRadius));
            addPoint(angle, step * remainder, endingRadius, innerRadius);
        }
        return Dart_uiLibrary.clampDouble(
            Math.Max(valleyRadius, pointRadiusLocal),
            double.Epsilon,
            double.MaxValue
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _drawPoints(Path path, List<_PointInfo__star_border> points)
    {
        Offset startingPoint = points.First().pointArc1;
        path.moveTo(startingPoint.dx, startingPoint.dy);
        double pointAngle = _getAngle(
            points[(int)0L].valley,
            points[(int)0L].point,
            points[(int)1L].valley
        );
        double pointWeight = _getWeight(pointAngle);
        double valleyAngle = _getAngle(
            points[(int)1L].point,
            points[(int)1L].valley,
            points[(int)0L].point
        );
        double valleyWeight = _getWeight(valleyAngle);
        for (var i = 0L; i < checked(points.Count); i += 1L)
        {
            _PointInfo__star_border pointLocal = points[(int)i];
            _PointInfo__star_border nextPoint = points[(int)((i + 1L) % checked(points.Count))];
            path.lineTo(pointLocal.pointArc1.dx, pointLocal.pointArc1.dy);
            if ((pointAngle != 180L) && (pointAngle != 0L))
            {
                path.conicTo(
                    pointLocal.point.dx,
                    pointLocal.point.dy,
                    pointLocal.pointArc2.dx,
                    pointLocal.pointArc2.dy,
                    pointWeight
                );
            }
            else
            {
                path.lineTo(pointLocal.pointArc2.dx, pointLocal.pointArc2.dy);
            }
            path.lineTo(pointLocal.valleyArc2.dx, pointLocal.valleyArc2.dy);
            if ((valleyAngle != 180L) && (valleyAngle != 0L))
            {
                path.conicTo(
                    nextPoint.valley.dx,
                    nextPoint.valley.dy,
                    nextPoint.valleyArc1.dx,
                    nextPoint.valleyArc1.dy,
                    valleyWeight
                );
            }
            else
            {
                path.lineTo(nextPoint.valleyArc1.dx, nextPoint.valleyArc1.dy);
            }
        }
        path.close();
    }

    internal virtual double _getWeight(double angle)
    {
        return Dart_mathLibrary.cos(angle / 2L % (Dart_mathLibrary.pi / 2L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getAngle(Offset a, Offset b, Offset c)
    {
        if (Equals(a, c) || Equals(b, c) || Equals(b, a))
        {
            return 0;
        }
        Offset u = a - b;
        Offset v = c - b;
        double dot = (u.dx * v.dx) + (u.dy * v.dy);
        double m1 = (b.dx == a.dx) ? double.PositiveInfinity : (-u.dy / -u.dx);
        double m2 = (b.dx == c.dx) ? double.PositiveInfinity : (-v.dy / -v.dx);
        double angle = Dart_mathLibrary.atan2(m1 - m2, 1L + (m1 * m2)).abs();
        if (dot < 0L)
        {
            angle += Dart_mathLibrary.pi;
        }
        return angle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
