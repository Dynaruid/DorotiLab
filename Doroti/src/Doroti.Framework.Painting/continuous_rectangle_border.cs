// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/continuous_rectangle_border.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class ContinuousRectangleBorder : OutlinedBorder
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;

    public ContinuousRectangleBorder(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!) : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
    }

    public override EdgeInsetsGeometry dimensions => EdgeInsets.CreateAll(((BorderSide)side).width);
    public override ShapeBorder scale(double t)
    {
        return new ContinuousRectangleBorder(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is ContinuousRectangleBorder))
        {
            ContinuousRectangleBorder a__as1701 = (ContinuousRectangleBorder)a;
            return new ContinuousRectangleBorder(side: BorderSide.lerp(((ContinuousRectangleBorder)a__as1701).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((ContinuousRectangleBorder)((ContinuousRectangleBorder)a__as1701)).borderRadius, this.borderRadius, t)!);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is ContinuousRectangleBorder))
        {
            ContinuousRectangleBorder b__as2029 = (ContinuousRectangleBorder)b;
            return new ContinuousRectangleBorder(side: BorderSide.lerp(side, ((ContinuousRectangleBorder)b__as2029).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((ContinuousRectangleBorder)((ContinuousRectangleBorder)b__as2029)).borderRadius, t)!);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _clampToShortest(RRect rrect, double value)
    {
        return ((value > rrect.shortestSide) ? rrect.shortestSide : value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Path _getPath(RRect rrect)
    {
        double leftLocal = rrect.left;
        double rightLocal = rrect.right;
        double topLocal = rrect.top;
        double bottomLocal = rrect.bottom;
        double tlRadiusXLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.tlRadiusX));
        double tlRadiusYLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.tlRadiusY));
        double trRadiusXLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.trRadiusX));
        double trRadiusYLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.trRadiusY));
        double blRadiusXLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.blRadiusX));
        double blRadiusYLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.blRadiusY));
        double brRadiusXLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.brRadiusX));
        double brRadiusYLocal = Math.Max(0.0, _clampToShortest(rrect, rrect.brRadiusY));
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo(leftLocal, (topLocal + tlRadiusXLocal));
    __cascade.cubicTo(leftLocal, topLocal, leftLocal, topLocal, (leftLocal + tlRadiusYLocal), topLocal);
    __cascade.lineTo((rightLocal - trRadiusXLocal), topLocal);
    __cascade.cubicTo(rightLocal, topLocal, rightLocal, topLocal, rightLocal, (topLocal + trRadiusYLocal));
    __cascade.lineTo(rightLocal, (bottomLocal - brRadiusXLocal));
    __cascade.cubicTo(rightLocal, bottomLocal, rightLocal, bottomLocal, (rightLocal - brRadiusYLocal), bottomLocal);
    __cascade.lineTo((leftLocal + blRadiusXLocal), bottomLocal);
    __cascade.cubicTo(leftLocal, bottomLocal, leftLocal, bottomLocal, leftLocal, (bottomLocal - blRadiusYLocal));
    __cascade.close();
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(this.borderRadius.resolve(textDirection).toRRect(rect).deflate(((BorderSide)side).width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(this.borderRadius.resolve(textDirection).toRRect(rect));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ContinuousRectangleBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new ContinuousRectangleBorder(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, BoxShape shape = BoxShape.rectangle, BorderRadius? borderRadius = null)
    {
        if (rect.isEmpty)
        {
            return;
        }
        switch (((BorderSide)side).style)
        {
            case BorderStyle.none:
                {
                    break;
                }
            case BorderStyle.solid:
                {
                    canvas.drawPath(getOuterPath(rect, textDirection: textDirection), side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as ContinuousRectangleBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is ContinuousRectangleBorder) && (object.Equals(((ContinuousRectangleBorder)__other).side, side))) && (object.Equals(((ContinuousRectangleBorder)((ContinuousRectangleBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius);
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ContinuousRectangleBorder"))}({side}, {this.borderRadius})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

