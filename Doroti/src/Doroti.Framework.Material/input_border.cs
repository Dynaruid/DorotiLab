// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_border.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public abstract class InputBorder : global::Doroti.Framework.Painting.ShapeBorder
{
    public static InputBorder none = ((InputBorder)(object?)new _NoInputBorder__input_border());
    public virtual global::Doroti.Framework.Painting.BorderSide borderSide { get; private set; } = default!;

    protected InputBorder(global::Doroti.Framework.Painting.BorderSide borderSide = default!)
    {
        global::Doroti.Framework.Painting.BorderSide __borderSide = borderSide ?? global::Doroti.Framework.Painting.BorderSide.none;
        this.borderSide = __borderSide;
    }

    public abstract InputBorder copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null);
    public abstract bool isOutline { get; }
    public abstract void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null);
}

internal class _NoInputBorder__input_border : InputBorder
{
    internal _NoInputBorder__input_border() : base(borderSide: global::Doroti.Framework.Painting.BorderSide.none)
    {
    }

    public override _NoInputBorder__input_border copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null) => new _NoInputBorder__input_border();
    public override bool isOutline => false;
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry dimensions => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.zero);
    public override _NoInputBorder__input_border scale(double t) => new _NoInputBorder__input_border();
    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(rect);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRect(rect, paint);
    }

    public override bool preferPaintInterior => true;
    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null)
    {
    }

}

public class UnderlineInputBorder : InputBorder
{
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public UnderlineInputBorder(global::Doroti.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!) : base(borderSide: borderSide ?? new global::Doroti.Framework.Painting.BorderSide())
    {
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.BorderRadius>(borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.CreateOnly(topLeft: Radius.circular(4.0), topRight: Radius.circular(4.0)));
        this.borderRadius = __borderRadius;
    }

    public override bool isOutline => false;
    public override UnderlineInputBorder copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new UnderlineInputBorder(borderSide: (borderSide ?? this.borderSide), borderRadius: (borderRadius ?? this.borderRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width));
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
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(global::Doroti.Ui.Rect.fromLTWH(rect.left, rect.top, rect.width, Math.Max(0.0, (rect.height - ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width))));
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

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;
    public override global::Doroti.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is UnderlineInputBorder))
        {
            UnderlineInputBorder a__as7313 = (UnderlineInputBorder)a;
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new UnderlineInputBorder(borderSide: BorderSide.lerp(((UnderlineInputBorder)a__as7313).borderSide, this.borderSide, t), borderRadius: BorderRadius.lerp(((UnderlineInputBorder)((UnderlineInputBorder)a__as7313)).borderRadius, this.borderRadius, t)!));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is UnderlineInputBorder))
        {
            UnderlineInputBorder b__as7641 = (UnderlineInputBorder)b;
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new UnderlineInputBorder(borderSide: BorderSide.lerp(this.borderSide, ((UnderlineInputBorder)b__as7641).borderSide, t), borderRadius: BorderRadius.lerp(this.borderRadius, ((UnderlineInputBorder)((UnderlineInputBorder)b__as7641)).borderRadius, t)!));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null)
    {
        if ((object.Equals(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).style, global::Doroti.Framework.Painting.BorderStyle.none)))
        {
            return;
        }
        if (((!object.Equals(((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft, Radius.zero)) || (!object.Equals(((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomRight, Radius.zero))))
        {
            var updatedBorderRadius = new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomLeft.clamp(maximum: global::Doroti.Ui.Radius.circular((rect.height / 2L))), bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)this.borderRadius).bottomRight.clamp(maximum: global::Doroti.Ui.Radius.circular((rect.height / 2L))));
            BoxBorder.paintNonUniformBorder(canvas, rect, textDirection: textDirection, borderRadius: updatedBorderRadius, bottom: this.borderSide.copyWith(strokeAlign: global::Doroti.Framework.Painting.BorderSide.strokeAlignInside), color: ((global::Doroti.Framework.Painting.BorderSide)this.borderSide).color);
        }
        else
        {
            var alignInsideOffset = new global::Doroti.Ui.Offset(0, (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2L));
            canvas.drawLine((rect.bottomLeft - alignInsideOffset), (rect.bottomRight - alignInsideOffset), this.borderSide.toPaint());
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
    public virtual global::Doroti.Framework.Painting.BorderRadius borderRadius { get; private set; } = default!;

    public OutlineInputBorder(global::Doroti.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Framework.Painting.BorderRadius borderRadius = default!, double gapPadding = 4.0) : base(borderSide: borderSide ?? new global::Doroti.Framework.Painting.BorderSide())
    {
        global::Doroti.Framework.Painting.BorderRadius __borderRadius = borderRadius ?? global::Doroti.Framework.Painting.BorderRadius.CreateAll(Radius.circular(4.0));
        this.borderRadius = __borderRadius;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert((gapPadding >= 0.0));
    }

    internal static bool _cornersAreCircular(global::Doroti.Framework.Painting.BorderRadius borderRadius)
    {
        return ((((((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topLeft.x == ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topLeft.y) && (((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomLeft.x == ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomLeft.y)) && (((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topRight.x == ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).topRight.y)) && (((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomRight.x == ((global::Doroti.Framework.Painting.BorderRadius)borderRadius).bottomRight.y));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isOutline => true;
    public override OutlineInputBorder copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new OutlineInputBorder(borderSide: (borderSide ?? this.borderSide), borderRadius: (borderRadius ?? this.borderRadius), gapPadding: (gapPadding ?? this.gapPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateAll(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).strokeInset));
            return default!;
        }
    }
    public override OutlineInputBorder scale(double t)
    {
        return new OutlineInputBorder(borderSide: this.borderSide.scale(t), borderRadius: (this.borderRadius.op_Multiply(t)), gapPadding: (this.gapPadding * t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is OutlineInputBorder))
        {
            OutlineInputBorder a__as13586 = (OutlineInputBorder)a;
            OutlineInputBorder outline = ((OutlineInputBorder)a__as13586);
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new OutlineInputBorder(borderRadius: BorderRadius.lerp(((OutlineInputBorder)outline).borderRadius, this.borderRadius, t)!, borderSide: BorderSide.lerp(outline.borderSide, this.borderSide, t), gapPadding: DartRuntimePrimitives.RequireValue(((OutlineInputBorder)outline).gapPadding)));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is OutlineInputBorder))
        {
            OutlineInputBorder b__as14006 = (OutlineInputBorder)b;
            OutlineInputBorder outline = ((OutlineInputBorder)b__as14006);
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new OutlineInputBorder(borderRadius: BorderRadius.lerp(this.borderRadius, ((OutlineInputBorder)outline).borderRadius, t)!, borderSide: BorderSide.lerp(this.borderSide, outline.borderSide, t), gapPadding: DartRuntimePrimitives.RequireValue(((OutlineInputBorder)outline).gapPadding)));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRRect(this.borderRadius.resolve(textDirection).toRRect(rect).deflate(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).strokeInset));
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

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        canvas.drawRRect(this.borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;
    internal virtual global::Doroti.Ui.Path _gapBorderPath(Canvas canvas, RRect center, double outerWidth, double start, double extent)
    {
        global::Doroti.Ui.RRect scaledRRect = ((global::Doroti.Ui.RRect)(object?)center.scaleRadii());
        var tlCorner = global::Doroti.Ui.Rect.fromLTWH(scaledRRect.left, scaledRRect.top, (scaledRRect.tlRadiusX * 2.0), (scaledRRect.tlRadiusY * 2.0));
        var trCorner = global::Doroti.Ui.Rect.fromLTWH((scaledRRect.right - (scaledRRect.trRadiusX * 2.0)), scaledRRect.top, (scaledRRect.trRadiusX * 2.0), (scaledRRect.trRadiusY * 2.0));
        var brCorner = global::Doroti.Ui.Rect.fromLTWH((scaledRRect.right - (scaledRRect.brRadiusX * 2.0)), (scaledRRect.bottom - (scaledRRect.brRadiusY * 2.0)), (scaledRRect.brRadiusX * 2.0), (scaledRRect.brRadiusY * 2.0));
        var blCorner = global::Doroti.Ui.Rect.fromLTWH(scaledRRect.left, (scaledRRect.bottom - (scaledRRect.blRadiusY * 2.0)), (scaledRRect.blRadiusX * 2.0), (scaledRRect.blRadiusY * 2.0));
        double cornerArcSweep = (Dart_mathLibrary.pi / 2.0);
        var path = new global::Doroti.Ui.Path();
        if ((!object.Equals(scaledRRect.tlRadius, Radius.zero)))
        {
            double tlCornerArcSweep = global::Doroti.Runtime.Dart_mathLibrary.acos(Dart_uiLibrary.clampDouble((1L - (start / scaledRRect.tlRadiusX)), 0.0, 1.0));
            path.addArc(tlCorner, Dart_mathLibrary.pi, tlCornerArcSweep);
        }
        else
        {
            path.moveTo((scaledRRect.left + (((global::Doroti.Framework.Painting.BorderSide)this.borderSide).strokeOffset / 2L)), scaledRRect.top);
        }
        if ((start > scaledRRect.tlRadiusX))
        {
            path.lineTo(start, scaledRRect.top);
        }
        double trCornerArcStart = (((3L * Dart_mathLibrary.pi)) / 2.0);
        var trCornerArcSweep = cornerArcSweep;
        if (((start + extent) < (outerWidth - scaledRRect.trRadiusX)))
        {
            path.moveTo((start + extent), scaledRRect.top);
            path.lineTo((scaledRRect.right - scaledRRect.trRadiusX), scaledRRect.top);
            if ((!object.Equals(scaledRRect.trRadius, Radius.zero)))
            {
                path.addArc(trCorner, trCornerArcStart, trCornerArcSweep);
            }
        }
        else
        {
            if (((start + extent) < outerWidth))
            {
                double dx = (outerWidth - ((start + extent)));
                double sweep = global::Doroti.Runtime.Dart_mathLibrary.asin(Dart_uiLibrary.clampDouble((1L - (dx / scaledRRect.trRadiusX)), 0.0, 1.0));
                path.addArc(trCorner, (trCornerArcStart + sweep), (trCornerArcSweep - sweep));
            }
        }
        if ((!object.Equals(scaledRRect.brRadius, Radius.zero)))
        {
            path.moveTo(scaledRRect.right, (scaledRRect.top + scaledRRect.trRadiusY));
        }
        path.lineTo(scaledRRect.right, (scaledRRect.bottom - scaledRRect.brRadiusY));
        if ((!object.Equals(scaledRRect.brRadius, Radius.zero)))
        {
            path.addArc(brCorner, 0.0, cornerArcSweep);
        }
        path.lineTo((scaledRRect.left + scaledRRect.blRadiusX), scaledRRect.bottom);
        if ((!object.Equals(scaledRRect.blRadius, Radius.zero)))
        {
            path.addArc(blCorner, (Dart_mathLibrary.pi / 2.0), cornerArcSweep);
        }
        path.lineTo(scaledRRect.left, (scaledRRect.top + scaledRRect.tlRadiusY));
        return ((global::Doroti.Ui.Path)(object?)path);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null)
    {
        DartRuntimePrimitives.Assert(() => ((gapPercentage >= 0.0) && (gapPercentage <= 1.0)));
        DartRuntimePrimitives.Assert(() => OutlineInputBorder._cornersAreCircular(this.borderRadius));
        global::Doroti.Ui.Paint paintLocal = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
        global::Doroti.Ui.RRect outer = ((global::Doroti.Ui.RRect)(object?)this.borderRadius.toRRect(rect));
        global::Doroti.Ui.RRect center = ((global::Doroti.Ui.RRect)(object?)outer.inflate((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).strokeOffset / 2L)));
        if ((((gapStart is null) || (gapExtent <= 0.0)) || (gapPercentage == 0.0)))
        {
            canvas.drawRRect(center, paintLocal);
        }
        else
        {
            double extent = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, (gapExtent + (this.gapPadding * 2.0)), gapPercentage));
            double start = (DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => ((DartRuntimePrimitives.RequireValue(gapStart) + this.gapPadding) - extent), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(gapStart) - this.gapPadding), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Ui.Path path = ((global::Doroti.Ui.Path)(object?)_gapBorderPath(canvas, center, outer.width, Math.Max(0.0, start), extent));
            canvas.drawPath(path, paintLocal);
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
    public virtual global::Doroti.Framework.Painting.ShapeBorder shape { get; private set; } = default!;

    public ShapedInputBorder(global::Doroti.Framework.Painting.BorderSide borderSide = default!, global::Doroti.Framework.Painting.ShapeBorder shape = default!, double gapPadding = 4.0) : base(borderSide: borderSide ?? new global::Doroti.Framework.Painting.BorderSide())
    {
        this.shape = shape;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert((gapPadding >= 0.0));
    }

    public override bool isOutline => true;
    public override ShapedInputBorder copyWith(global::Doroti.Framework.Painting.BorderSide? borderSide = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, double? gapPadding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new ShapedInputBorder(borderSide: (borderSide ?? this.borderSide), shape: (shape ?? this.shape), gapPadding: (gapPadding ?? this.gapPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry dimensions
    {
        get
        {
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateAll(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width));
            return default!;
        }
    }
    public override ShapedInputBorder scale(double t)
    {
        return new ShapedInputBorder(borderSide: this.borderSide.scale(t), shape: this.shape.scale(t), gapPadding: (this.gapPadding * t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.ShapeBorder? lerpFrom(global::Doroti.Framework.Painting.ShapeBorder? a, double t)
    {
        if ((a is ShapedInputBorder))
        {
            ShapedInputBorder a__as23771 = (ShapedInputBorder)a;
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new ShapedInputBorder(borderSide: BorderSide.lerp(((ShapedInputBorder)a__as23771).borderSide, this.borderSide, t), shape: ShapeBorder.lerp(((ShapedInputBorder)((ShapedInputBorder)a__as23771)).shape, this.shape, t)!, gapPadding: DartRuntimePrimitives.RequireValue(((ShapedInputBorder)((ShapedInputBorder)a__as23771)).gapPadding)));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpFrom(a, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Painting.ShapeBorder? lerpTo(global::Doroti.Framework.Painting.ShapeBorder? b, double t)
    {
        if ((b is ShapedInputBorder))
        {
            ShapedInputBorder b__as24105 = (ShapedInputBorder)b;
            return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)new ShapedInputBorder(borderSide: BorderSide.lerp(this.borderSide, ((ShapedInputBorder)b__as24105).borderSide, t), shape: ShapeBorder.lerp(this.shape, ((ShapedInputBorder)((ShapedInputBorder)b__as24105)).shape, t)!, gapPadding: DartRuntimePrimitives.RequireValue(((ShapedInputBorder)((ShapedInputBorder)b__as24105)).gapPadding)));
        }
        return ((global::Doroti.Framework.Painting.ShapeBorder?)(object?)base.lerpTo(b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Path)(object?)this.shape.getInnerPath(rect.deflate(((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width), textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return ((Path)(object?)this.shape.getOuterPath(rect, textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(Canvas canvas, Rect rect, Paint paint, TextDirection? textDirection = null)
    {
        if (((global::Doroti.Framework.Painting.ShapeBorder)this.shape).preferPaintInterior)
        {
            this.shape.paintInterior(canvas, rect, paint, textDirection: textDirection);
        }
        else
        {
            canvas.drawPath(this.shape.getOuterPath(rect, textDirection: textDirection), paint);
        }
    }

    public override bool preferPaintInterior => ((global::Doroti.Framework.Painting.ShapeBorder)this.shape).preferPaintInterior;
    internal virtual global::Doroti.Ui.Path _gapBorderPath(Rect rect, double start, double extent, TextDirection? textDirection = null)
    {
        global::Doroti.Ui.Path outerPath = ((global::Doroti.Ui.Path)(object?)this.shape.getOuterPath(rect, textDirection: textDirection));
        if (((start <= 0L) && (extent <= 0L)))
        {
            return ((global::Doroti.Ui.Path)(object?)outerPath);
        }
        var gapLeft = start;
        double gapRight = (start + extent);
        var gapRect = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addRect(global::Doroti.Ui.Rect.fromLTRB(Dart_uiLibrary.clampDouble(gapLeft, rect.left, rect.right), (rect.top - 1.0), Dart_uiLibrary.clampDouble(gapRight, rect.left, rect.right), (rect.top + 1.0)));
    return __cascade;
}))();
        return ((global::Doroti.Ui.Path)(object?)Dart_uiLibrary.Path.combine(PathOperation.difference, outerPath, gapRect));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(Canvas canvas, Rect rect, TextDirection? textDirection = null, double? gapStart = 0.0, double gapExtent = 0.0, double gapPercentage = default!, global::Doroti.Framework.Painting.BoxShape shape = global::Doroti.Framework.Painting.BoxShape.rectangle, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null)
    {
        DartRuntimePrimitives.Assert(() => ((gapPercentage >= 0.0) && (gapPercentage <= 1.0)));
        global::Doroti.Ui.Paint paintLocal = ((global::Doroti.Ui.Paint)(object?)this.borderSide.toPaint());
        global::Doroti.Ui.Rect deflatedRect = ((global::Doroti.Ui.Rect)(object?)rect.deflate((((global::Doroti.Framework.Painting.BorderSide)this.borderSide).width / 2.0)));
        if ((((gapStart is null) || (gapExtent <= 0.0)) || (gapPercentage == 0.0)))
        {
            if ((this.shape is global::Doroti.Framework.Painting.OutlinedBorder))
            {
                global::Doroti.Framework.Painting.OutlinedBorder shape__as27236 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(shape);
                var outlinedShape = ((global::Doroti.Framework.Painting.OutlinedBorder?)(object?)this.shape)!;
                global::Doroti.Framework.Painting.OutlinedBorder shapedBorder = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)outlinedShape.copyWith(side: this.borderSide));
                shapedBorder.paint(canvas, deflatedRect, textDirection: textDirection);
            }
            else
            {
                canvas.drawPath(this.shape.getOuterPath(deflatedRect, textDirection: textDirection), paintLocal);
            }
        }
        else
        {
            double extent = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0.0, (gapExtent + (this.gapPadding * 2.0)), gapPercentage));
            double start = (DartRuntimePrimitives.RequireValue(textDirection) switch { TextDirection.rtl => ((DartRuntimePrimitives.RequireValue(gapStart) + this.gapPadding) - extent), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(gapStart) - this.gapPadding), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            global::Doroti.Ui.Path path = ((global::Doroti.Ui.Path)(object?)_gapBorderPath(deflatedRect, Math.Max(0.0, start), extent, textDirection: DartRuntimePrimitives.RequireValue(textDirection)));
            canvas.drawPath(path, paintLocal);
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
