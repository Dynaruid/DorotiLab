// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/input_border.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public abstract class InputBorder : global::Doroti.Generated.Framework.Painting.ShapeBorder
{
    public static InputBorder none = ((InputBorder)(object?)new _NoInputBorder__input_border());
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide borderSide { get; private set; } = default!;

    protected InputBorder(global::Doroti.Generated.Framework.Painting.BorderSide borderSide = default!)
    {
        global::Doroti.Generated.Framework.Painting.BorderSide __borderSide = borderSide ?? global::Doroti.Generated.Framework.Painting.BorderSide.none;
        this.borderSide = __borderSide;
    }

    public abstract InputBorder copyWith(global::Doroti.Generated.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null);
    public abstract bool isOutline { get; }
    public abstract void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null);
}

internal class _NoInputBorder__input_border : InputBorder
{
    internal _NoInputBorder__input_border() : base(borderSide: global::Doroti.Generated.Framework.Painting.BorderSide.none)
    {
    }

    public override _NoInputBorder__input_border copyWith(global::Doroti.Generated.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null) => new _NoInputBorder__input_border();
    public override bool isOutline => false;
    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry dimensions => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Generated.Framework.Painting.EdgeInsets.zero);
    public override _NoInputBorder__input_border scale(double t) => new _NoInputBorder__input_border();
    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRect(rect);
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRect(rect);
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRect(rect, paint);
    }

    public override bool preferPaintInterior => true;
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null)
    {
    }

}

public class UnderlineInputBorder : InputBorder
{
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public UnderlineInputBorder(global::Doroti.Generated.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!) : base(borderSide: borderSide ?? new global::Doroti.Generated.Framework.Painting.BorderSide())
    {
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.BorderRadius>(borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateOnly(topLeft: Radius.circular(4.0), topRight: Radius.circular(4.0)));
        this.borderRadius = __borderRadius;
    }

    public override bool isOutline => false;
    public override UnderlineInputBorder copyWith(global::Doroti.Generated.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null)
    {
        return new UnderlineInputBorder(borderSide: (borderSide ?? this.borderSide), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width));
            return default!;
        }
    }
    public override UnderlineInputBorder scale(double t)
    {
        return new UnderlineInputBorder(borderSide: this.borderSide.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRect(global::Doroti.Flutter.Ui.Rect.fromLTWH(rect.left, rect.top, rect.width, Math.Max(0.0, (rect.height - ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width))));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRRect(this.borderRadius.resolve(textDirection).toRRect(rect));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;
    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Generated.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is UnderlineInputBorder))
        {
            UnderlineInputBorder a__as7313 = (UnderlineInputBorder)a;
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new UnderlineInputBorder(borderSide: BorderSide.lerp(((UnderlineInputBorder)a__as7313).borderSide, this.borderSide, t), borderRadius: BorderRadius.lerp(((UnderlineInputBorder)((UnderlineInputBorder)a__as7313)).borderRadius, this.borderRadius, t)!));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Generated.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is UnderlineInputBorder))
        {
            UnderlineInputBorder b__as7641 = (UnderlineInputBorder)b;
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new UnderlineInputBorder(borderSide: BorderSide.lerp(this.borderSide, ((UnderlineInputBorder)b__as7641).borderSide, t), borderRadius: BorderRadius.lerp(this.borderRadius, ((UnderlineInputBorder)((UnderlineInputBorder)b__as7641)).borderRadius, t)!));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null)
    {
        if ((object.Equals(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).style, global::Doroti.Generated.Framework.Painting.BorderStyle.none)))
        {
            return;
        }
        if (((!object.Equals(((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft, Radius.zero)) || (!object.Equals(((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomRight, Radius.zero))))
        {
            var updatedBorderRadius__8547 = new global::Doroti.Generated.Framework.Painting.BorderRadius(bottomLeft: ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.clamp(maximum: global::Doroti.Flutter.Ui.Radius.circular((rect.height / 2L))), bottomRight: ((global::Doroti.Generated.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.clamp(maximum: global::Doroti.Flutter.Ui.Radius.circular((rect.height / 2L))));
            BoxBorder.paintNonUniformBorder(canvas, rect, textDirection: textDirection, borderRadius: updatedBorderRadius__8547, bottom: this.borderSide.copyWith(strokeAlign: global::Doroti.Generated.Framework.Painting.BorderSide.strokeAlignInside), color: ((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).color);
        }
        else
        {
            var alignInsideOffset__9085 = new global::Doroti.Flutter.Ui.Offset(0, (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2L));
            canvas.drawLine((rect.bottomLeft - alignInsideOffset__9085), (rect.bottomRight - alignInsideOffset__9085), this.borderSide.toPaint());
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as UnderlineInputBorder;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is UnderlineInputBorder) && (object.Equals(((UnderlineInputBorder)__other).borderSide, this.borderSide))) && (object.Equals(((UnderlineInputBorder)((UnderlineInputBorder)__other)).borderRadius, this.borderRadius)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.borderSide, this.borderRadius));
}

public class OutlineInputBorder : InputBorder
{
    public virtual double gapPadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public OutlineInputBorder(global::Doroti.Generated.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius = default!, double gapPadding = 4.0) : base(borderSide: borderSide ?? new global::Doroti.Generated.Framework.Painting.BorderSide())
    {
        global::Doroti.Generated.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(Radius.circular(4.0));
        this.borderRadius = __borderRadius;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert((gapPadding >= 0.0));
    }

    internal static bool _cornersAreCircular(global::Doroti.Generated.Framework.Painting.BorderRadius borderRadius)
    {
        return ((((((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topLeft.x == ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topLeft.y) && (((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomLeft.x == ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomLeft.y)) && (((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topRight.x == ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).topRight.y)) && (((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomRight.x == ((global::Doroti.Generated.Framework.Painting.BorderRadius)borderRadius).bottomRight.y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isOutline => true;
    public override OutlineInputBorder copyWith(global::Doroti.Generated.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null)
    {
        return new OutlineInputBorder(borderSide: (borderSide ?? this.borderSide), borderRadius: (borderRadius ?? this.borderRadius), gapPadding: (gapPadding ?? this.gapPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).strokeInset));
            return default!;
        }
    }
    public override OutlineInputBorder scale(double t)
    {
        return new OutlineInputBorder(borderSide: this.borderSide.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)), gapPadding: (this.gapPadding * t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Generated.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is OutlineInputBorder))
        {
            OutlineInputBorder a__as13586 = (OutlineInputBorder)a;
            OutlineInputBorder outline__13644 = ((OutlineInputBorder)a__as13586);
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new OutlineInputBorder(borderRadius: BorderRadius.lerp(((OutlineInputBorder)outline__13644).borderRadius, this.borderRadius, t)!, borderSide: BorderSide.lerp(outline__13644.borderSide, this.borderSide, t), gapPadding: DartRuntimePrimitives.RequireValue(((OutlineInputBorder)outline__13644).gapPadding)));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Generated.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is OutlineInputBorder))
        {
            OutlineInputBorder b__as14006 = (OutlineInputBorder)b;
            OutlineInputBorder outline__14064 = ((OutlineInputBorder)b__as14006);
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new OutlineInputBorder(borderRadius: BorderRadius.lerp(this.borderRadius, ((OutlineInputBorder)outline__14064).borderRadius, t)!, borderSide: BorderSide.lerp(this.borderSide, outline__14064.borderSide, t), gapPadding: DartRuntimePrimitives.RequireValue(((OutlineInputBorder)outline__14064).gapPadding)));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRRect(this.borderRadius.resolve(textDirection).toRRect(rect).deflate(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).strokeInset));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRRect(this.borderRadius.resolve(textDirection).toRRect(rect));
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;
    internal virtual global::Doroti.Flutter.Ui.Path _gapBorderPath(Canvas canvas, RRect center, double outerWidth, double start, double extent)
    {
        global::Doroti.Flutter.Ui.RRect scaledRRect__15263 = ((global::Doroti.Flutter.Ui.RRect)(object?)center.scaleRadii());
        var tlCorner__15309 = global::Doroti.Flutter.Ui.Rect.fromLTWH(scaledRRect__15263.left, scaledRRect__15263.top, (scaledRRect__15263.tlRadiusX * 2.0), (scaledRRect__15263.tlRadiusY * 2.0));
        var trCorner__15469 = global::Doroti.Flutter.Ui.Rect.fromLTWH((scaledRRect__15263.right - (scaledRRect__15263.trRadiusX * 2.0)), scaledRRect__15263.top, (scaledRRect__15263.trRadiusX * 2.0), (scaledRRect__15263.trRadiusY * 2.0));
        var brCorner__15660 = global::Doroti.Flutter.Ui.Rect.fromLTWH((scaledRRect__15263.right - (scaledRRect__15263.brRadiusX * 2.0)), (scaledRRect__15263.bottom - (scaledRRect__15263.brRadiusY * 2.0)), (scaledRRect__15263.brRadiusX * 2.0), (scaledRRect__15263.brRadiusY * 2.0));
        var blCorner__15884 = global::Doroti.Flutter.Ui.Rect.fromLTWH(scaledRRect__15263.left, (scaledRRect__15263.bottom - (scaledRRect__15263.blRadiusY * 2.0)), (scaledRRect__15263.blRadiusX * 2.0), (scaledRRect__15263.blRadiusY * 2.0));
        double cornerArcSweep__16222 = (Dart_mathLibrary.pi / 2.0);
        var path__16264 = new global::Doroti.Flutter.Ui.Path();
        if ((!object.Equals(scaledRRect__15263.tlRadius, Radius.zero)))
        {
            double tlCornerArcSweep__16369 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.acos(Dart_uiLibrary.clampDouble((1L - (start / scaledRRect__15263.tlRadiusX)), 0.0, 1.0));
            path__16264.addArc(tlCorner__15309, Dart_mathLibrary.pi, tlCornerArcSweep__16369);
        }
        else
        {
            path__16264.moveTo((scaledRRect__15263.left + (((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).strokeOffset / 2L)), scaledRRect__15263.top);
        }
        if ((start > scaledRRect__15263.tlRadiusX))
        {
            path__16264.lineTo(start, scaledRRect__15263.top);
        }
        double trCornerArcStart__17045 = (((3L * Dart_mathLibrary.pi)) / 2.0);
        var trCornerArcSweep__17095 = cornerArcSweep__16222;
        if (((start + extent) < (outerWidth - scaledRRect__15263.trRadiusX)))
        {
            path__16264.moveTo((start + extent), scaledRRect__15263.top);
            path__16264.lineTo((scaledRRect__15263.right - scaledRRect__15263.trRadiusX), scaledRRect__15263.top);
            if ((!object.Equals(scaledRRect__15263.trRadius, Radius.zero)))
            {
                path__16264.addArc(trCorner__15469, trCornerArcStart__17045, trCornerArcSweep__17095);
            }
        }
        else
        {
            if (((start + extent) < outerWidth))
            {
                double dx__17513 = (outerWidth - ((start + extent)));
                double sweep__17568 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.asin(Dart_uiLibrary.clampDouble((1L - (dx__17513 / scaledRRect__15263.trRadiusX)), 0.0, 1.0));
                path__16264.addArc(trCorner__15469, (trCornerArcStart__17045 + sweep__17568), (trCornerArcSweep__17095 - sweep__17568));
            }
        }
        if ((!object.Equals(scaledRRect__15263.brRadius, Radius.zero)))
        {
            path__16264.moveTo(scaledRRect__15263.right, (scaledRRect__15263.top + scaledRRect__15263.trRadiusY));
        }
        path__16264.lineTo(scaledRRect__15263.right, (scaledRRect__15263.bottom - scaledRRect__15263.brRadiusY));
        if ((!object.Equals(scaledRRect__15263.brRadius, Radius.zero)))
        {
            path__16264.addArc(brCorner__15660, 0.0, cornerArcSweep__16222);
        }
        path__16264.lineTo((scaledRRect__15263.left + scaledRRect__15263.blRadiusX), scaledRRect__15263.bottom);
        if ((!object.Equals(scaledRRect__15263.blRadius, Radius.zero)))
        {
            path__16264.addArc(blCorner__15884, (Dart_mathLibrary.pi / 2.0), cornerArcSweep__16222);
        }
        path__16264.lineTo(scaledRRect__15263.left, (scaledRRect__15263.top + scaledRRect__15263.tlRadiusY));
        return ((global::Doroti.Flutter.Ui.Path)(object?)path__16264);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null)
    {
        DartRuntimePrimitives.Assert(() => ((gapPercentage >= 0.0) && (gapPercentage <= 1.0)));
        DartRuntimePrimitives.Assert(() => OutlineInputBorder._cornersAreCircular(this.borderRadius));
        global::Doroti.Flutter.Ui.Paint paint__19224 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
        global::Doroti.Flutter.Ui.RRect outer__19270 = ((global::Doroti.Flutter.Ui.RRect)(object?)this.borderRadius.toRRect(rect));
        global::Doroti.Flutter.Ui.RRect center__19322 = ((global::Doroti.Flutter.Ui.RRect)(object?)outer__19270.inflate((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).strokeOffset / 2L)));
        if ((((gapStart is null) || (gapExtent <= 0.0)) || (gapPercentage == 0.0)))
        {
            canvas.drawRRect(center__19322, paint__19224);
        }
        else
        {
            double extent__19518 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, (gapExtent + (this.gapPadding * 2.0)), gapPercentage));
            double start__19609 = (DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => ((DartRuntimePrimitives.RequireValue(gapStart) + this.gapPadding) - extent__19518), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(gapStart) - this.gapPadding), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Flutter.Ui.Path path__19782 = ((global::Doroti.Flutter.Ui.Path)(object?)_gapBorderPath(canvas, center__19322, outer__19270.width, Math.Max(0.0, start__19609), extent__19518));
            canvas.drawPath(path__19782, paint__19224);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as OutlineInputBorder;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is OutlineInputBorder) && (object.Equals(((OutlineInputBorder)__other).borderSide, this.borderSide))) && (object.Equals(((OutlineInputBorder)((OutlineInputBorder)__other)).borderRadius, this.borderRadius))) && (((OutlineInputBorder)((OutlineInputBorder)__other)).gapPadding == this.gapPadding));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.borderSide, this.borderRadius, this.gapPadding));
}

public class ShapedInputBorder : InputBorder
{
    public virtual double gapPadding { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder shape { get; private set; } = default!;

    public ShapedInputBorder(global::Doroti.Generated.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Generated.Framework.Painting.ShapeBorder shape = default!, double gapPadding = 4.0) : base(borderSide: borderSide ?? new global::Doroti.Generated.Framework.Painting.BorderSide())
    {
        this.shape = shape;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert((gapPadding >= 0.0));
    }

    public override bool isOutline => true;
    public override ShapedInputBorder copyWith(global::Doroti.Generated.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null)
    {
        return new ShapedInputBorder(borderSide: (borderSide ?? this.borderSide), shape: (shape ?? this.shape), gapPadding: (gapPadding ?? this.gapPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width));
            return default!;
        }
    }
    public override ShapedInputBorder scale(double t)
    {
        return new ShapedInputBorder(borderSide: this.borderSide.scale(t), shape: this.shape.scale(t), gapPadding: (this.gapPadding * t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Generated.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is ShapedInputBorder))
        {
            ShapedInputBorder a__as23771 = (ShapedInputBorder)a;
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new ShapedInputBorder(borderSide: BorderSide.lerp(((ShapedInputBorder)a__as23771).borderSide, this.borderSide, t), shape: ShapeBorder.lerp(((ShapedInputBorder)((ShapedInputBorder)a__as23771)).shape, this.shape, t)!, gapPadding: DartRuntimePrimitives.RequireValue(((ShapedInputBorder)((ShapedInputBorder)a__as23771)).gapPadding)));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Generated.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is ShapedInputBorder))
        {
            ShapedInputBorder b__as24105 = (ShapedInputBorder)b;
            return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)new ShapedInputBorder(borderSide: BorderSide.lerp(this.borderSide, ((ShapedInputBorder)b__as24105).borderSide, t), shape: ShapeBorder.lerp(this.shape, ((ShapedInputBorder)((ShapedInputBorder)b__as24105)).shape, t)!, gapPadding: DartRuntimePrimitives.RequireValue(((ShapedInputBorder)((ShapedInputBorder)b__as24105)).gapPadding)));
        }
        return ((global::Doroti.Generated.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Path)(object?)this.shape.getInnerPath(rect.deflate(((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width), textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Path)(object?)this.shape.getOuterPath(rect, textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        if (((global::Doroti.Generated.Framework.Painting.ShapeBorder)this.shape).preferPaintInterior)
        {
            this.shape.paintInterior(canvas, rect, paint, textDirection: textDirection);
        }
        else
        {
            canvas.drawPath(this.shape.getOuterPath(rect, textDirection: textDirection), paint);
        }
    }

    public override bool preferPaintInterior => ((global::Doroti.Generated.Framework.Painting.ShapeBorder)this.shape).preferPaintInterior;
    internal virtual global::Doroti.Flutter.Ui.Path _gapBorderPath(Rect rect, double start, double extent, TextDirection? textDirection = null)
    {
        global::Doroti.Flutter.Ui.Path outerPath__25345 = ((global::Doroti.Flutter.Ui.Path)(object?)this.shape.getOuterPath(rect, textDirection: textDirection));
        if (((start <= 0L) && (extent <= 0L)))
        {
            return ((global::Doroti.Flutter.Ui.Path)(object?)outerPath__25345);
        }
        var gapLeft__25693 = start;
        double gapRight__25727 = (start + extent);
        var gapRect__25931 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addRect(global::Doroti.Flutter.Ui.Rect.fromLTRB(Dart_uiLibrary.clampDouble(gapLeft__25693, rect.left, rect.right), (rect.top - 1.0), Dart_uiLibrary.clampDouble(gapRight__25727, rect.left, rect.right), (rect.top + 1.0)));
            return __cascade;        }))();
        return ((global::Doroti.Flutter.Ui.Path)(object?)Dart_uiLibrary.Path.combine(PathOperation.difference, outerPath__25345, gapRect__25931));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Generated.Framework.Painting.BoxShape shape = global::Doroti.Generated.Framework.Painting.BoxShape.rectangle, global::Doroti.Generated.Framework.Painting.BorderRadius? borderRadius = null)
    {
        DartRuntimePrimitives.Assert(() => ((gapPercentage >= 0.0) && (gapPercentage <= 1.0)));
        global::Doroti.Flutter.Ui.Paint paint__27016 = ((global::Doroti.Flutter.Ui.Paint)(object?)this.borderSide.toPaint());
        global::Doroti.Flutter.Ui.Rect deflatedRect__27061 = ((global::Doroti.Flutter.Ui.Rect)(object?)rect.deflate((((global::Doroti.Generated.Framework.Painting.BorderSide)this.borderSide).width / 2.0)));
        if ((((gapStart is null) || (gapExtent <= 0.0)) || (gapPercentage == 0.0)))
        {
            if ((this.shape is global::Doroti.Generated.Framework.Painting.OutlinedBorder))
            {
                global::Doroti.Generated.Framework.Painting.OutlinedBorder shape__as27236 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(shape);
                var outlinedShape__27277 = ((global::Doroti.Generated.Framework.Painting.OutlinedBorder?)(object?)this.shape)!;
                global::Doroti.Generated.Framework.Painting.OutlinedBorder shapedBorder__27394 = ((global::Doroti.Generated.Framework.Painting.OutlinedBorder)(object?)outlinedShape__27277.copyWith(side: this.borderSide));
                shapedBorder__27394.paint(canvas, deflatedRect__27061, textDirection: textDirection);
            }
            else
            {
                canvas.drawPath(this.shape.getOuterPath(deflatedRect__27061, textDirection: textDirection), paint__27016);
            }
        }
        else
        {
            double extent__27682 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, (gapExtent + (this.gapPadding * 2.0)), gapPercentage));
            double start__27773 = (DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => ((DartRuntimePrimitives.RequireValue(gapStart) + this.gapPadding) - extent__27682), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(gapStart) - this.gapPadding), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Flutter.Ui.Path path__27946 = ((global::Doroti.Flutter.Ui.Path)(object?)_gapBorderPath(deflatedRect__27061, Math.Max(0.0, start__27773), extent__27682, textDirection: DartRuntimePrimitives.RequireValue(textDirection)));
            canvas.drawPath(path__27946, paint__27016);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as ShapedInputBorder;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is ShapedInputBorder) && (object.Equals(((ShapedInputBorder)__other).borderSide, this.borderSide))) && (object.Equals(((ShapedInputBorder)((ShapedInputBorder)__other)).shape, this.shape))) && (((ShapedInputBorder)((ShapedInputBorder)__other)).gapPadding == this.gapPadding));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.borderSide, this.shape, this.gapPadding));
}
