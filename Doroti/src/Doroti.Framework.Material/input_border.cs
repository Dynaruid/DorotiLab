// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/input_border.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public abstract class InputBorder : ShapeBorder
{
    public static InputBorder none = new _NoInputBorder__input_border();
    public virtual BorderSide borderSide { get; private set; } = default!;

    protected InputBorder(BorderSide borderSide = default!)
    {
        BorderSide __borderSide = borderSide ?? BorderSide.none;
        this.borderSide = __borderSide;
    }

    public abstract InputBorder copyWith(
        BorderSide? borderSide = null,
        BorderRadius? borderRadius = null,
        double? gapPadding = null,
        ShapeBorder? shape = null
    );
    public abstract bool isOutline { get; }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    ) =>
        paint(
            canvas,
            rect,
            textDirection: textDirection,
            gapStart: 0.0,
            gapExtent: 0.0,
            gapPercentage: 0.0,
            shape: shape,
            borderRadius: borderRadius
        );

    public abstract void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        double? gapStart = 0.0,
        double gapExtent = 0.0,
        double gapPercentage = default!,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    );
}

internal class _NoInputBorder__input_border : InputBorder
{
    internal _NoInputBorder__input_border()
        : base(borderSide: BorderSide.none) { }

    public override _NoInputBorder__input_border copyWith(
        BorderSide? borderSide = null,
        BorderRadius? borderRadius = null,
        double? gapPadding = null,
        ShapeBorder? shape = null
    ) => new _NoInputBorder__input_border();

    public override bool isOutline => false;
    public override EdgeInsetsGeometry dimensions =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.zero);

    public override _NoInputBorder__input_border scale(double t) =>
        new _NoInputBorder__input_border();

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(rect);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(rect);
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        canvas.drawRect(rect, paint);
    }

    public override bool preferPaintInterior => true;

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        double? gapStart = 0.0,
        double gapExtent = 0.0,
        double gapPercentage = default!,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    ) { }
}

public class UnderlineInputBorder : InputBorder
{
    public virtual BorderRadius borderRadius { get; private set; } = default!;

    public UnderlineInputBorder(
        BorderSide borderSide = default!,
        BorderRadius borderRadius = default!
    )
        : base(borderSide: borderSide ?? new BorderSide())
    {
        BorderRadius __borderRadius = DartRuntimePrimitives.ConvertValue<BorderRadius>(
            borderRadius
                ?? BorderRadiusGeometry.CreateOnly(
                    topLeft: Radius.circular(4.0),
                    topRight: Radius.circular(4.0)
                )
        );
        this.borderRadius = __borderRadius;
    }

    public override bool isOutline => false;

    public override UnderlineInputBorder copyWith(
        BorderSide? borderSide = null,
        BorderRadius? borderRadius = null,
        double? gapPadding = null,
        ShapeBorder? shape = null
    )
    {
        return new UnderlineInputBorder(
            borderSide: borderSide ?? this.borderSide,
            borderRadius: borderRadius ?? this.borderRadius
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get { return EdgeInsets.CreateOnly(bottom: borderSide.width); }
    }

    public override UnderlineInputBorder scale(double t)
    {
        return new UnderlineInputBorder(borderSide: borderSide.scale(t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(
                        Rect.fromLTWH(
                            rect.left,
                            rect.top,
                            rect.width,
                            Math.Max(0.0, rect.height - borderSide.width)
                        )
                    );
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRRect(borderRadius.resolve(textDirection).toRRect(rect));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        canvas.drawRRect(borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is UnderlineInputBorder)
        {
            UnderlineInputBorder a__as7313 = (UnderlineInputBorder)a;
            return (ShapeBorder?)
                new UnderlineInputBorder(
                    borderSide: BorderSide.lerp(a__as7313.borderSide, borderSide, t),
                    borderRadius: BorderRadius.lerp(a__as7313.borderRadius, borderRadius, t)!
                );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is UnderlineInputBorder)
        {
            UnderlineInputBorder b__as7641 = (UnderlineInputBorder)b;
            return (ShapeBorder?)
                new UnderlineInputBorder(
                    borderSide: BorderSide.lerp(borderSide, b__as7641.borderSide, t),
                    borderRadius: BorderRadius.lerp(borderRadius, b__as7641.borderRadius, t)!
                );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        double? gapStart = 0.0,
        double gapExtent = 0.0,
        double gapPercentage = default!,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        if (Equals(borderSide.style, BorderStyle.none))
        {
            return;
        }
        if (
            (!Equals(this.borderRadius.bottomLeft, Radius.zero))
            || (!Equals(this.borderRadius.bottomRight, Radius.zero))
        )
        {
            var updatedBorderRadius = new BorderRadius(
                bottomLeft: this.borderRadius.bottomLeft.clamp(
                    maximum: Radius.circular(rect.height / 2L)
                ),
                bottomRight: this.borderRadius.bottomRight.clamp(
                    maximum: Radius.circular(rect.height / 2L)
                )
            );
            BoxBorder.paintNonUniformBorder(
                canvas,
                rect,
                textDirection: textDirection,
                borderRadius: updatedBorderRadius,
                bottom: borderSide.copyWith(strokeAlign: BorderSide.strokeAlignInside),
                color: borderSide.color
            );
        }
        else
        {
            var alignInsideOffset = new Offset(0, borderSide.width / 2L);
            canvas.drawLine(
                rect.bottomLeft - alignInsideOffset,
                rect.bottomRight - alignInsideOffset,
                borderSide.toPaint()
            );
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as UnderlineInputBorder;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is UnderlineInputBorder)
            && Equals(__other.borderSide, borderSide)
            && Equals(__other.borderRadius, borderRadius);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(borderSide, borderRadius)
        );
}

public class OutlineInputBorder : InputBorder
{
    public virtual double gapPadding { get; private set; } = default!;
    public virtual BorderRadius borderRadius { get; private set; } = default!;

    public OutlineInputBorder(
        BorderSide borderSide = default!,
        BorderRadius borderRadius = default!,
        double gapPadding = 4.0
    )
        : base(borderSide: borderSide ?? new BorderSide())
    {
        BorderRadius __borderRadius = borderRadius ?? BorderRadius.CreateAll(Radius.circular(4.0));
        this.borderRadius = __borderRadius;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert(gapPadding >= 0.0);
    }

    internal static bool _cornersAreCircular(BorderRadius borderRadius)
    {
        return (borderRadius.topLeft.x == borderRadius.topLeft.y)
            && (borderRadius.bottomLeft.x == borderRadius.bottomLeft.y)
            && (borderRadius.topRight.x == borderRadius.topRight.y)
            && (borderRadius.bottomRight.x == borderRadius.bottomRight.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isOutline => true;

    public override OutlineInputBorder copyWith(
        BorderSide? borderSide = null,
        BorderRadius? borderRadius = null,
        double? gapPadding = null,
        ShapeBorder? shape = null
    )
    {
        return new OutlineInputBorder(
            borderSide: borderSide ?? this.borderSide,
            borderRadius: borderRadius ?? this.borderRadius,
            gapPadding: gapPadding ?? this.gapPadding
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get { return EdgeInsets.CreateAll(borderSide.strokeInset); }
    }

    public override OutlineInputBorder scale(double t)
    {
        return new OutlineInputBorder(
            borderSide: borderSide.scale(t),
            borderRadius: borderRadius.op_Multiply(t),
            gapPadding: gapPadding * t
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is OutlineInputBorder)
        {
            OutlineInputBorder a__as13586 = (OutlineInputBorder)a;
            OutlineInputBorder outline = a__as13586;
            return (ShapeBorder?)
                new OutlineInputBorder(
                    borderRadius: BorderRadius.lerp(outline.borderRadius, borderRadius, t)!,
                    borderSide: BorderSide.lerp(outline.borderSide, borderSide, t),
                    gapPadding: DartRuntimePrimitives.RequireValue(outline.gapPadding)
                );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is OutlineInputBorder)
        {
            OutlineInputBorder b__as14006 = (OutlineInputBorder)b;
            OutlineInputBorder outline = b__as14006;
            return (ShapeBorder?)
                new OutlineInputBorder(
                    borderRadius: BorderRadius.lerp(borderRadius, outline.borderRadius, t)!,
                    borderSide: BorderSide.lerp(borderSide, outline.borderSide, t),
                    gapPadding: DartRuntimePrimitives.RequireValue(outline.gapPadding)
                );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRRect(
                        borderRadius
                            .resolve(textDirection)
                            .toRRect(rect)
                            .deflate(borderSide.strokeInset)
                    );
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRRect(borderRadius.resolve(textDirection).toRRect(rect));
                    return __cascade;
                }
            )
        )();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        canvas.drawRRect(borderRadius.resolve(textDirection).toRRect(rect), paint);
    }

    public override bool preferPaintInterior => true;

    internal virtual Path _gapBorderPath(
        Canvas canvas,
        RRect center,
        double outerWidth,
        double start,
        double extent
    )
    {
        RRect scaledRRect = center.scaleRadii();
        var tlCorner = Rect.fromLTWH(
            scaledRRect.left,
            scaledRRect.top,
            scaledRRect.tlRadiusX * 2.0,
            scaledRRect.tlRadiusY * 2.0
        );
        var trCorner = Rect.fromLTWH(
            scaledRRect.right - (scaledRRect.trRadiusX * 2.0),
            scaledRRect.top,
            scaledRRect.trRadiusX * 2.0,
            scaledRRect.trRadiusY * 2.0
        );
        var brCorner = Rect.fromLTWH(
            scaledRRect.right - (scaledRRect.brRadiusX * 2.0),
            scaledRRect.bottom - (scaledRRect.brRadiusY * 2.0),
            scaledRRect.brRadiusX * 2.0,
            scaledRRect.brRadiusY * 2.0
        );
        var blCorner = Rect.fromLTWH(
            scaledRRect.left,
            scaledRRect.bottom - (scaledRRect.blRadiusY * 2.0),
            scaledRRect.blRadiusX * 2.0,
            scaledRRect.blRadiusY * 2.0
        );
        double cornerArcSweep = Dart_mathLibrary.pi / 2.0;
        var path = new Path();
        if (!Equals(scaledRRect.tlRadius, Radius.zero))
        {
            double tlCornerArcSweep = Dart_mathLibrary.acos(
                Dart_uiLibrary.clampDouble(1L - (start / scaledRRect.tlRadiusX), 0.0, 1.0)
            );
            path.addArc(tlCorner, Dart_mathLibrary.pi, tlCornerArcSweep);
        }
        else
        {
            path.moveTo(scaledRRect.left + (borderSide.strokeOffset / 2L), scaledRRect.top);
        }
        if (start > scaledRRect.tlRadiusX)
        {
            path.lineTo(start, scaledRRect.top);
        }
        double trCornerArcStart = 3L * Dart_mathLibrary.pi / 2.0;
        var trCornerArcSweep = cornerArcSweep;
        if ((start + extent) < (outerWidth - scaledRRect.trRadiusX))
        {
            path.moveTo(start + extent, scaledRRect.top);
            path.lineTo(scaledRRect.right - scaledRRect.trRadiusX, scaledRRect.top);
            if (!Equals(scaledRRect.trRadius, Radius.zero))
            {
                path.addArc(trCorner, trCornerArcStart, trCornerArcSweep);
            }
        }
        else
        {
            if ((start + extent) < outerWidth)
            {
                double dx = outerWidth - (start + extent);
                double sweep = Dart_mathLibrary.asin(
                    Dart_uiLibrary.clampDouble(1L - (dx / scaledRRect.trRadiusX), 0.0, 1.0)
                );
                path.addArc(trCorner, trCornerArcStart + sweep, trCornerArcSweep - sweep);
            }
        }
        if (!Equals(scaledRRect.brRadius, Radius.zero))
        {
            path.moveTo(scaledRRect.right, scaledRRect.top + scaledRRect.trRadiusY);
        }
        path.lineTo(scaledRRect.right, scaledRRect.bottom - scaledRRect.brRadiusY);
        if (!Equals(scaledRRect.brRadius, Radius.zero))
        {
            path.addArc(brCorner, 0.0, cornerArcSweep);
        }
        path.lineTo(scaledRRect.left + scaledRRect.blRadiusX, scaledRRect.bottom);
        if (!Equals(scaledRRect.blRadius, Radius.zero))
        {
            path.addArc(blCorner, Dart_mathLibrary.pi / 2.0, cornerArcSweep);
        }
        path.lineTo(scaledRRect.left, scaledRRect.top + scaledRRect.tlRadiusY);
        return path;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        double? gapStart = 0.0,
        double gapExtent = 0.0,
        double gapPercentage = default!,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        DartRuntimePrimitives.Assert(() => (gapPercentage >= 0.0) && (gapPercentage <= 1.0));
        DartRuntimePrimitives.Assert(() => _cornersAreCircular(this.borderRadius));
        Paint paintLocal = borderSide.toPaint();
        RRect outer = this.borderRadius.toRRect(rect);
        RRect center = outer.inflate(borderSide.strokeOffset / 2L);
        if ((gapStart is null) || (gapExtent <= 0.0) || (gapPercentage == 0.0))
        {
            canvas.drawRRect(center, paintLocal);
        }
        else
        {
            double extent = DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(0.0, gapExtent + (gapPadding * 2.0), gapPercentage)
            );
            double start = DartRuntimePrimitives.RequireValue(textDirection) switch
            {
                TextDirection.rtl => DartRuntimePrimitives.RequireValue(gapStart)
                    + gapPadding
                    - extent,
                TextDirection.ltr => DartRuntimePrimitives.RequireValue(gapStart) - gapPadding,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            Path path = _gapBorderPath(canvas, center, outer.width, Math.Max(0.0, start), extent);
            canvas.drawPath(path, paintLocal);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as OutlineInputBorder;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is OutlineInputBorder)
            && Equals(__other.borderSide, borderSide)
            && Equals(__other.borderRadius, borderRadius)
            && (__other.gapPadding == gapPadding);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(borderSide, borderRadius, gapPadding)
        );
}

public class ShapedInputBorder : InputBorder
{
    public virtual double gapPadding { get; private set; } = default!;
    public virtual ShapeBorder shape { get; private set; } = default!;

    public ShapedInputBorder(
        BorderSide borderSide = default!,
        ShapeBorder shape = default!,
        double gapPadding = 4.0
    )
        : base(borderSide: borderSide ?? new BorderSide())
    {
        this.shape = shape;
        this.gapPadding = gapPadding;
        System.Diagnostics.Debug.Assert(gapPadding >= 0.0);
    }

    public override bool isOutline => true;

    public override ShapedInputBorder copyWith(
        BorderSide? borderSide = null,
        BorderRadius? borderRadius = null,
        double? gapPadding = null,
        ShapeBorder? shape = null
    )
    {
        return new ShapedInputBorder(
            borderSide: borderSide ?? this.borderSide,
            shape: shape ?? this.shape,
            gapPadding: gapPadding ?? this.gapPadding
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override EdgeInsetsGeometry dimensions
    {
        get { return EdgeInsets.CreateAll(borderSide.width); }
    }

    public override ShapedInputBorder scale(double t)
    {
        return new ShapedInputBorder(
            borderSide: borderSide.scale(t),
            shape: shape.scale(t),
            gapPadding: gapPadding * t
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t)
    {
        if (a is ShapedInputBorder)
        {
            ShapedInputBorder a__as23771 = (ShapedInputBorder)a;
            return (ShapeBorder?)
                new ShapedInputBorder(
                    borderSide: BorderSide.lerp(a__as23771.borderSide, borderSide, t),
                    shape: lerp(a__as23771.shape, shape, t)!,
                    gapPadding: DartRuntimePrimitives.RequireValue(a__as23771.gapPadding)
                );
        }
        return base.lerpFrom(a, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t)
    {
        if (b is ShapedInputBorder)
        {
            ShapedInputBorder b__as24105 = (ShapedInputBorder)b;
            return (ShapeBorder?)
                new ShapedInputBorder(
                    borderSide: BorderSide.lerp(borderSide, b__as24105.borderSide, t),
                    shape: lerp(shape, b__as24105.shape, t)!,
                    gapPadding: DartRuntimePrimitives.RequireValue(b__as24105.gapPadding)
                );
        }
        return base.lerpTo(b, t);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null)
    {
        return shape.getInnerPath(rect.deflate(borderSide.width), textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null)
    {
        return shape.getOuterPath(rect, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    )
    {
        if (shape.preferPaintInterior)
        {
            shape.paintInterior(canvas, rect, paint, textDirection: textDirection);
        }
        else
        {
            canvas.drawPath(shape.getOuterPath(rect, textDirection: textDirection), paint);
        }
    }

    public override bool preferPaintInterior => shape.preferPaintInterior;

    internal virtual Path _gapBorderPath(
        Rect rect,
        double start,
        double extent,
        TextDirection? textDirection = null
    )
    {
        Path outerPath = shape.getOuterPath(rect, textDirection: textDirection);
        if ((start <= 0L) && (extent <= 0L))
        {
            return outerPath;
        }
        var gapLeft = start;
        double gapRight = start + extent;
        var gapRect = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addRect(
                        Rect.fromLTRB(
                            Dart_uiLibrary.clampDouble(gapLeft, rect.left, rect.right),
                            rect.top - 1.0,
                            Dart_uiLibrary.clampDouble(gapRight, rect.left, rect.right),
                            rect.top + 1.0
                        )
                    );
                    return __cascade;
                }
            )
        )();
        return Dart_uiLibrary.Path.combine(PathOperation.difference, outerPath, gapRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        double? gapStart = 0.0,
        double gapExtent = 0.0,
        double gapPercentage = default!,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    )
    {
        DartRuntimePrimitives.Assert(() => (gapPercentage >= 0.0) && (gapPercentage <= 1.0));
        Paint paintLocal = borderSide.toPaint();
        Rect deflatedRect = rect.deflate(borderSide.width / 2.0);
        if ((gapStart is null) || (gapExtent <= 0.0) || (gapPercentage == 0.0))
        {
            if (this.shape is OutlinedBorder)
            {
                OutlinedBorder shape__as27236 = DartRuntimePrimitives.ConvertValue<OutlinedBorder>(
                    shape
                );
                var outlinedShape = ((OutlinedBorder?)this.shape)!;
                OutlinedBorder shapedBorder = outlinedShape.copyWith(side: borderSide);
                shapedBorder.paint(canvas, deflatedRect, textDirection: textDirection);
            }
            else
            {
                canvas.drawPath(
                    this.shape.getOuterPath(deflatedRect, textDirection: textDirection),
                    paintLocal
                );
            }
        }
        else
        {
            double extent = DartRuntimePrimitives.RequireValue(
                Dart_uiLibrary.lerpDouble(0.0, gapExtent + (gapPadding * 2.0), gapPercentage)
            );
            double start = DartRuntimePrimitives.RequireValue(textDirection) switch
            {
                TextDirection.rtl => DartRuntimePrimitives.RequireValue(gapStart)
                    + gapPadding
                    - extent,
                TextDirection.ltr => DartRuntimePrimitives.RequireValue(gapStart) - gapPadding,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            Path path = _gapBorderPath(
                deflatedRect,
                Math.Max(0.0, start),
                extent,
                textDirection: DartRuntimePrimitives.RequireValue(textDirection)
            );
            canvas.drawPath(path, paintLocal);
        }
    }

    public override bool Equals(object? other)
    {
        var __other = other as ShapedInputBorder;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ShapedInputBorder)
            && Equals(__other.borderSide, borderSide)
            && Equals(__other.shape, shape)
            && (__other.gapPadding == gapPadding);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(borderSide, shape, gapPadding)
        );
}
