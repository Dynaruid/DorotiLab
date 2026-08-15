// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/circle_border.dart
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

namespace Doroti.Generated.Framework.Painting;

public class CircleBorder : OutlinedBorder
{
    public virtual double eccentricity { get; private set; } = default!;

    public CircleBorder(BorderSide side = default!, double eccentricity = 0.0) : base(side: side ?? BorderSide.none)
    {
        this.eccentricity = eccentricity;
        System.Diagnostics.Debug.Assert((eccentricity >= 0.0));
        System.Diagnostics.Debug.Assert((eccentricity <= 1.0));
    }

    public override ShapeBorder scale(double t) => new CircleBorder(side: side.scale(t), eccentricity: this.eccentricity);
    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if ((a is CircleBorder))
        {
            CircleBorder a__as2194 = (CircleBorder)a;
            return new CircleBorder(side: BorderSide.lerp(((CircleBorder)a__as2194).side, side, t), eccentricity: Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((CircleBorder)((CircleBorder)a__as2194)).eccentricity, this.eccentricity, t)), 0.0, 1.0));
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if ((b is CircleBorder))
        {
            CircleBorder b__as2507 = (CircleBorder)b;
            return new CircleBorder(side: BorderSide.lerp(side, ((CircleBorder)b__as2507).side, t), eccentricity: Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.eccentricity, ((CircleBorder)((CircleBorder)b__as2507)).eccentricity, t)), 0.0, 1.0));
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(_adjustRect(rect).deflate(((BorderSide)side).strokeInset));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(_adjustRect(rect));
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.Rect adjustedRect__3136 = _adjustRect(rect);
        return global::Doroti.Ui.RRect.fromRectAndRadius(adjustedRect__3136, global::Doroti.Ui.Radius.elliptical((adjustedRect__3136.width / 2.0), (adjustedRect__3136.height / 2.0))).contains(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        if ((this.eccentricity == 0.0))
        {
            canvas.drawCircle(rect.center, (rect.shortestSide / 2.0), paint);
        }
        else
        {
            canvas.drawOval(_adjustRect(rect), paint);
        }
    }

    public override bool preferPaintInterior => true;
    public override CircleBorder copyWith(BorderSide? side = null, BorderRadiusGeometry? borderRadius = null, double? eccentricity = null, LinearBorderEdge? start = null, LinearBorderEdge? end = null, LinearBorderEdge? top = null, LinearBorderEdge? bottom = null, double? circularity = null, double? rectilinearity = null, double? points = null, double? innerRadiusRatio = null, double? pointRounding = null, double? valleyRounding = null, double? rotation = null, double? squash = null)
    {
        return new CircleBorder(side: (side ?? this.side), eccentricity: (eccentricity ?? this.eccentricity));
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
                    if ((this.eccentricity == 0.0))
                    {
                        canvas.drawCircle(rect.center, (((rect.shortestSide + ((BorderSide)side).strokeOffset)) / 2L), side.toPaint());
                    }
                    else
                    {
                        global::Doroti.Ui.Rect borderRect__4262 = _adjustRect(rect);
                        canvas.drawOval(borderRect__4262.inflate((((BorderSide)side).strokeOffset / 2L)), side.toPaint());
                    }
                    break;
                }
        }
    }

    internal virtual global::Doroti.Ui.Rect _adjustRect(Rect rect)
    {
        if (((this.eccentricity == 0.0) || (rect.width == rect.height)))
        {
            return global::Doroti.Ui.Rect.fromCircle(center: rect.center, radius: (rect.shortestSide / 2.0));
        }
        if ((rect.width < rect.height))
        {
            double delta__4638 = ((((1.0 - this.eccentricity)) * ((rect.height - rect.width))) / 2.0);
            return global::Doroti.Ui.Rect.fromLTRB(rect.left, (rect.top + delta__4638), rect.right, (rect.bottom - delta__4638));
        }
        else
        {
            double delta__4825 = ((((1.0 - this.eccentricity)) * ((rect.width - rect.height))) / 2.0);
            return global::Doroti.Ui.Rect.fromLTRB((rect.left + delta__4825), rect.top, (rect.right - delta__4825), rect.bottom);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as CircleBorder;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is CircleBorder) && (object.Equals(((CircleBorder)__other).side, side))) && (((CircleBorder)((CircleBorder)__other)).eccentricity == this.eccentricity));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(side, this.eccentricity);
    public override string ToString()
    {
        if ((this.eccentricity != 0.0))
        {
            return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CircleBorder"))}({side}, eccentricity: {this.eccentricity})";
        }
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CircleBorder"))}({side})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

