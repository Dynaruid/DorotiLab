// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/beveled_rectangle_border.dart
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

public class BeveledRectangleBorder : OutlinedBorder
{
    public virtual BorderRadiusGeometry borderRadius { get; private set; } = default!;

    public BeveledRectangleBorder(BorderSide side = default!, BorderRadiusGeometry borderRadius = default!) : base(side: side ?? BorderSide.none)
    {
        BorderRadiusGeometry __borderRadius = borderRadius ?? BorderRadius.zero;
        this.borderRadius = __borderRadius;
    }

    public override ShapeBorder scale(double t)
    {
        return new BeveledRectangleBorder(side: side.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is BeveledRectangleBorder))
        {
            BeveledRectangleBorder a__as1688 = (BeveledRectangleBorder)a;
            return new BeveledRectangleBorder(side: BorderSide.lerp(((BeveledRectangleBorder)a__as1688).side, side, t), borderRadius: BorderRadiusGeometry.lerp(((BeveledRectangleBorder)((BeveledRectangleBorder)a__as1688)).borderRadius, this.borderRadius, t)!);
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is BeveledRectangleBorder))
        {
            BeveledRectangleBorder b__as2010 = (BeveledRectangleBorder)b;
            return new BeveledRectangleBorder(side: BorderSide.lerp(side, ((BeveledRectangleBorder)b__as2010).side, t), borderRadius: BorderRadiusGeometry.lerp(this.borderRadius, ((BeveledRectangleBorder)((BeveledRectangleBorder)b__as2010)).borderRadius, t)!);
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override BeveledRectangleBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new BeveledRectangleBorder(side: (side ?? this.side), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Path _getPath(RRect rrect)
    {
        var centerLeft__2648 = new global::Doroti.Flutter.Ui.Offset(rrect.left, rrect.center.dy);
        var centerRight__2708 = new global::Doroti.Flutter.Ui.Offset(rrect.right, rrect.center.dy);
        var centerTop__2770 = new global::Doroti.Flutter.Ui.Offset(rrect.center.dx, rrect.top);
        var centerBottom__2828 = new global::Doroti.Flutter.Ui.Offset(rrect.center.dx, rrect.bottom);
        double tlRadiusX__2900 = Math.Max(0.0, rrect.tlRadiusX);
        double tlRadiusY__2961 = Math.Max(0.0, rrect.tlRadiusY);
        double trRadiusX__3022 = Math.Max(0.0, rrect.trRadiusX);
        double trRadiusY__3083 = Math.Max(0.0, rrect.trRadiusY);
        double blRadiusX__3144 = Math.Max(0.0, rrect.blRadiusX);
        double blRadiusY__3205 = Math.Max(0.0, rrect.blRadiusY);
        double brRadiusX__3266 = Math.Max(0.0, rrect.brRadiusX);
        double brRadiusY__3327 = Math.Max(0.0, rrect.brRadiusY);
        var vertices__3382 = new List<global::Doroti.Flutter.Ui.Offset> { new global::Doroti.Flutter.Ui.Offset(rrect.left, Math.Min(centerLeft__2648.dy, (rrect.top + tlRadiusY__2961))), new global::Doroti.Flutter.Ui.Offset(Math.Min(centerTop__2770.dx, (rrect.left + tlRadiusX__2900)), rrect.top), new global::Doroti.Flutter.Ui.Offset(Math.Max(centerTop__2770.dx, (rrect.right - trRadiusX__3022)), rrect.top), new global::Doroti.Flutter.Ui.Offset(rrect.right, Math.Min(centerRight__2708.dy, (rrect.top + trRadiusY__3083))), new global::Doroti.Flutter.Ui.Offset(rrect.right, Math.Max(centerRight__2708.dy, (rrect.bottom - brRadiusY__3327))), new global::Doroti.Flutter.Ui.Offset(Math.Max(centerBottom__2828.dx, (rrect.right - brRadiusX__3266)), rrect.bottom), new global::Doroti.Flutter.Ui.Offset(Math.Min(centerBottom__2828.dx, (rrect.left + blRadiusX__3144)), rrect.bottom), new global::Doroti.Flutter.Ui.Offset(rrect.left, Math.Max(centerLeft__2648.dy, (rrect.bottom - blRadiusY__3205))) };
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Flutter.Ui.Path();
    __cascade.addPolygon(vertices__3382, true);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(this.borderRadius.resolve(textDirection).toRRect(rect).deflate(((BorderSide)side).strokeInset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return _getPath(this.borderRadius.resolve(textDirection).toRRect(rect));
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
                    global::Doroti.Flutter.Ui.RRect borderRect__4658 = this.borderRadius.resolve(textDirection).toRRect(rect);
                    global::Doroti.Flutter.Ui.RRect adjustedRect__4742 = borderRect__4658.inflate(((BorderSide)side).strokeOutset);
                    global::Doroti.Flutter.Ui.Path path__4815 = ((Func<Path>)(() =>
{
    var __cascade = _getPath(adjustedRect__4742);
    __cascade.addPath(getInnerPath(rect, textDirection: textDirection), Offset.zero);
    return __cascade;
}))();
                    canvas.drawPath(path__4815, side.toPaint());
                    break;
                }
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as BeveledRectangleBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is BeveledRectangleBorder) && (object.Equals(((BeveledRectangleBorder)__other).side, side))) && (object.Equals(((BeveledRectangleBorder)((BeveledRectangleBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.borderRadius);
    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "BeveledRectangleBorder"))}({side}, {this.borderRadius})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

