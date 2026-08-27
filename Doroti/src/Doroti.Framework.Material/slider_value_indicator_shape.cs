// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider_value_indicator_shape.dart
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

public interface SliderComponentShape
{
    public static SliderComponentShape noThumb = ((SliderComponentShape)(object?)new _EmptySliderComponentShape__slider_value_indicator_shape());
    public static SliderComponentShape noOverlay = ((SliderComponentShape)(object?)new _EmptySliderComponentShape__slider_value_indicator_shape());

    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null);
    public void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow);
}

internal class _EmptySliderComponentShape__slider_value_indicator_shape : SliderComponentShape
{
    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => Size.zero;
    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
    }

}

public class RoundSliderOverlayShape : SliderComponentShape
{
    public virtual double overlayRadius { get; private set; } = default!;

    public RoundSliderOverlayShape(double overlayRadius = 24.0)
    {
        this.overlayRadius = overlayRadius;
    }

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        return global::Doroti.Ui.Size.fromRadius(this.overlayRadius);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: this.overlayRadius);
        canvasLocal.drawCircle(center, radiusTween.evaluate(activationAnimation), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = sliderTheme.overlayColor!;
    return __cascade;
}))());
    }

}

public class RectangularSliderValueIndicatorShape : SliderComponentShape
{
    internal static _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape _pathPainter = new _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape();

    public RectangularSliderValueIndicatorShape()
    {
    }

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return _pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvasLocal, center: center, scale: scaleLocal, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
    }

}

public class RectangularRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape _pathPainter = new _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape();

    public RectangularRangeSliderValueIndicatorShape()
    {
    }

    public override global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        DartRuntimePrimitives.Assert(() => (textScaleFactor >= 0L));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox? parentBox = null, Offset? center = null, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, global::Doroti.Framework.Animation.Animation<double>? activationAnimation = null, double? textScaleFactor = null, Size? sizeWithOverflow = null)
    {
        return _pathPainter.getHorizontalShift(parentBox: parentBox!, center: DartRuntimePrimitives.RequireValue(center), labelPainter: labelPainter!, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), scale: DartRuntimePrimitives.RequireValue(activationAnimation!.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = activationAnimation!.value;
        _pathPainter.paint(parentBox: parentBox!, canvas: canvasLocal, center: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(center)), scale: scaleLocal, labelPainter: labelPainter!, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme!.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
    }

}

internal class _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape
{
    internal const double _triangleHeight = 8.0;
    internal const double _labelPadding = 16.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _bottomTipYOffset = 14.0;
    internal static double _preferredHalfHeight = (_preferredHeight / 2L);
    internal const double _upperRectRadius = 4;

    internal _RectangularSliderValueIndicatorPathPainter__slider_value_indicator_shape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        return new global::Doroti.Ui.Size(_upperRectangleWidth(labelPainter, 1, textScaleFactor), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height + _labelPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding = 8.0;
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale, textScaleFactor);
        global::Doroti.Ui.Offset globalCenter = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft = Math.Max(0, (((rectangleWidth / 2L) - globalCenter.dx) + edgePadding));
        double overflowRight = Math.Max(0, ((rectangleWidth / 2L) - (((sizeWithOverflow.width - globalCenter.dx) - edgePadding))));
        if ((rectangleWidth < sizeWithOverflow.width))
        {
            return (overflowLeft - overflowRight);
        }
        else
        {
            if (((overflowLeft - overflowRight) > 0L))
            {
                return (overflowLeft - ((edgePadding * textScaleFactor)));
            }
            else
            {
                return (-overflowRight + ((edgePadding * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale, double textScaleFactor)
    {
        double unscaledWidth = (Math.Max((_minLabelWidth * textScaleFactor), ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + (_labelPadding * 2L));
        return (unscaledWidth * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale, textScaleFactor);
        double horizontalShift = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        double rectHeight = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height + _labelPadding);
        var upperRect = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth / 2L) + horizontalShift), (-_triangleHeight - rectHeight), rectangleWidth, rectHeight);
        var trianglePath = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.lineTo(-_triangleHeight, -_triangleHeight);
    __cascade.lineTo(_triangleHeight, -_triangleHeight);
    __cascade.close();
    return __cascade;
}))();
        var fillPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundPaintColor;
    return __cascade;
}))();
        var upperRRect = global::Doroti.Ui.RRect.fromRectAndRadius(upperRect, global::Doroti.Ui.Radius.circular(_upperRectRadius));
        trianglePath.addRRect(upperRRect);
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        if ((strokePaintColor is not null))
        {
            var strokePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = strokePaintColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            canvas.drawPath(trianglePath, strokePaint);
        }
        canvas.drawPath(trianglePath, fillPaint);
        double bottomTipToUpperRectTranslateY = ((-_preferredHalfHeight / 2L) - upperRect.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY);
        var boxCenter = new global::Doroti.Ui.Offset(horizontalShift, (upperRect.height / 2L));
        var halfLabelPainterOffset = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset = ((global::Doroti.Ui.Offset)(object?)(boxCenter - halfLabelPainterOffset));
        labelPainter.paint(canvas, labelOffset);
        canvas.restore();
    }

}

public class PaddleSliderValueIndicatorShape : SliderComponentShape
{
    internal static _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape _pathPainter = new _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape();

    public PaddleSliderValueIndicatorShape()
    {
    }

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return _pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var enableColor = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.valueIndicatorColor);
        _pathPainter.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, center, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = enableColor.evaluate(enableAnimation)!;
    return __cascade;
}))(), ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value, labelPainter, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow, sliderTheme.valueIndicatorStrokeColor);
    }

}

public class PaddleRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape _pathPainter = new _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape();

    public PaddleRangeSliderValueIndicatorShape()
    {
    }

    public override global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        DartRuntimePrimitives.Assert(() => (textScaleFactor >= 0L));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox? parentBox = null, Offset? center = null, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, global::Doroti.Framework.Animation.Animation<double>? activationAnimation = null, double? textScaleFactor = null, Size? sizeWithOverflow = null)
    {
        return _pathPainter.getHorizontalShift(center: DartRuntimePrimitives.RequireValue(center), labelPainter: labelPainter!, scale: DartRuntimePrimitives.RequireValue(activationAnimation!.value), textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)
    {
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(sizeWithOverflow).isEmpty);
        var enableColor = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.valueIndicatorColor);
        _pathPainter.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(center)), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = enableColor.evaluate(enableAnimation)!;
    return __cascade;
}))(), DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value), labelPainter, DartRuntimePrimitives.RequireValue(textScaleFactor), DartRuntimePrimitives.RequireValue(sizeWithOverflow), (isOnTop ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
    }

}

internal class _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape
{
    internal const double _topLobeRadius = 16.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _bottomLobeRadius = 10.0;
    internal const double _labelPadding = 8.0;
    internal const double _distanceBetweenTopBottomCenters = 40.0;
    internal const double _middleNeckWidth = 3.0;
    internal const double _bottomNeckRadius = 4.5;
    internal static double _neckTriangleBase = (_topNeckRadius + (_middleNeckWidth / 2L));
    internal static double _rightBottomNeckCenterX = ((_middleNeckWidth / 2L) + _bottomNeckRadius);
    internal static double _rightBottomNeckAngleStart = Dart_mathLibrary.pi;
    internal static Offset _topLobeCenter = new global::Doroti.Ui.Offset(0.0, -_distanceBetweenTopBottomCenters);
    internal const double _topNeckRadius = 13.0;
    internal static double _neckTriangleHypotenuse = (_topLobeRadius + _topNeckRadius);
    internal static double _twoSeventyDegrees = ((3.0 * Dart_mathLibrary.pi) / 2.0);
    internal static double _ninetyDegrees = (Dart_mathLibrary.pi / 2.0);
    internal static double _thirtyDegrees = (Dart_mathLibrary.pi / 6.0);
    internal static double _preferredHeight = ((_distanceBetweenTopBottomCenters + _topLobeRadius) + _bottomLobeRadius);
    internal const bool _debuggingLabelLocation = false;

    internal _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        DartRuntimePrimitives.Assert(() => (textScaleFactor >= 0L));
        double widthLocal = (Math.Max((_minLabelWidth * textScaleFactor), ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L) * textScaleFactor));
        return new global::Doroti.Ui.Size(widthLocal, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _addArc(Path path, Offset center, double radius, double startAngle, double endAngle)
    {
        DartRuntimePrimitives.Assert(() => center.isFinite);
        var arcRect = global::Doroti.Ui.Rect.fromCircle(center: center, radius: radius);
        path.arcTo(arcRect, startAngle, (endAngle - startAngle), false);
    }

    public virtual double getHorizontalShift(Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double scale, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double inverseTextScale = ((textScaleFactor != 0L) ? (1.0 / textScaleFactor) : 0.0);
        double labelHalfWidth = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2.0);
        double halfWidthNeeded = Math.Max(0.0, ((inverseTextScale * labelHalfWidth) - ((_topLobeRadius - _labelPadding))));
        double shift = _getIdealOffset(halfWidthNeeded, (textScaleFactor * scale), center, sizeWithOverflow.width);
        return (shift * textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getIdealOffset(double halfWidthNeeded, double scale, Offset center, double widthWithOverflow)
    {
        var edgeMargin = 8.0;
        var topLobeRect = global::Doroti.Ui.Rect.fromLTWH((-_topLobeRadius - halfWidthNeeded), (-_topLobeRadius - _distanceBetweenTopBottomCenters), (2.0 * ((_topLobeRadius + halfWidthNeeded))), (2.0 * _topLobeRadius));
        global::Doroti.Ui.Offset topLeftLocal = ((global::Doroti.Ui.Offset)(object?)(((topLobeRect.topLeft * scale)) + center));
        global::Doroti.Ui.Offset bottomRightLocal = ((global::Doroti.Ui.Offset)(object?)(((topLobeRect.bottomRight * scale)) + center));
        var shift = 0.0;
        if ((topLeftLocal.dx < edgeMargin))
        {
            shift = (edgeMargin - topLeftLocal.dx);
        }
        var endGlobal = widthWithOverflow;
        if ((bottomRightLocal.dx > (endGlobal - edgeMargin)))
        {
            shift = ((endGlobal - edgeMargin) - bottomRightLocal.dx);
        }
        shift = ((scale == 0.0) ? 0.0 : (shift / scale));
        if ((shift < 0.0))
        {
            shift = Math.Max(shift, -halfWidthNeeded);
        }
        else
        {
            shift = Math.Min(shift, halfWidthNeeded);
        }
        return shift;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(Canvas canvas, Offset center, Paint paint, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color? strokePaintColor)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double overallScale = (scale * textScaleFactor);
        double inverseTextScale = ((textScaleFactor != 0L) ? (1.0 / textScaleFactor) : 0.0);
        double labelHalfWidth = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2.0);
        canvas.save();
        canvas.translate(center.dx, center.dy);
        canvas.scale(overallScale, overallScale);
        double bottomNeckTriangleHypotenuse = (_bottomNeckRadius + (_bottomLobeRadius / overallScale));
        double rightBottomNeckCenterY = -global::Doroti.Runtime.Dart_mathLibrary.sqrt((global::Doroti.Runtime.Dart_mathLibrary.pow(bottomNeckTriangleHypotenuse, 2L) - global::Doroti.Runtime.Dart_mathLibrary.pow(_rightBottomNeckCenterX, 2L)));
        double rightBottomNeckAngleEnd = (Dart_mathLibrary.pi + global::Doroti.Runtime.Dart_mathLibrary.atan((rightBottomNeckCenterY / _rightBottomNeckCenterX)));
        var path = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo((_middleNeckWidth / 2L), rightBottomNeckCenterY);
    return __cascade;
}))();
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, new global::Doroti.Ui.Offset(_rightBottomNeckCenterX, rightBottomNeckCenterY), _bottomNeckRadius, _rightBottomNeckAngleStart, rightBottomNeckAngleEnd);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, Offset.zero, (_bottomLobeRadius / overallScale), (rightBottomNeckAngleEnd - Dart_mathLibrary.pi), ((2L * Dart_mathLibrary.pi) - rightBottomNeckAngleEnd));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, new global::Doroti.Ui.Offset(-_rightBottomNeckCenterX, rightBottomNeckCenterY), _bottomNeckRadius, (Dart_mathLibrary.pi - rightBottomNeckAngleEnd), 0);
        double halfWidthNeeded = Math.Max(0.0, ((inverseTextScale * labelHalfWidth) - ((_topLobeRadius - _labelPadding))));
        double shift = _getIdealOffset(halfWidthNeeded, overallScale, center, sizeWithOverflow.width);
        double leftWidthNeeded = (halfWidthNeeded - shift);
        double rightWidthNeeded = (halfWidthNeeded + shift);
        double leftAmount = Math.Max(0.0, Math.Min(1.0, (leftWidthNeeded / _neckTriangleBase)));
        double rightAmount = Math.Max(0.0, Math.Min(1.0, (rightWidthNeeded / _neckTriangleBase)));
        double leftTheta = (((1.0 - leftAmount)) * _thirtyDegrees);
        double rightTheta = (((1.0 - rightAmount)) * _thirtyDegrees);
        var leftTopNeckCenter = new global::Doroti.Ui.Offset(-_neckTriangleBase, (_topLobeCenter.dy + (global::Doroti.Runtime.Dart_mathLibrary.cos(leftTheta) * _neckTriangleHypotenuse)));
        var neckRightCenter = new global::Doroti.Ui.Offset(_neckTriangleBase, (_topLobeCenter.dy + (global::Doroti.Runtime.Dart_mathLibrary.cos(rightTheta) * _neckTriangleHypotenuse)));
        double leftNeckArcAngle = (_ninetyDegrees - leftTheta);
        double rightNeckArcAngle = ((Dart_mathLibrary.pi + _ninetyDegrees) - rightTheta);
        double neckStretchBaseline = Math.Max(0.0, (rightBottomNeckCenterY - Math.Max(leftTopNeckCenter.dy, neckRightCenter.dy)));
        var t = ((double)global::Doroti.Runtime.Dart_mathLibrary.pow(inverseTextScale, 3.0));
        double stretch = Dart_uiLibrary.clampDouble((neckStretchBaseline * t), 0.0, (10.0 * neckStretchBaseline));
        var neckStretch = new global::Doroti.Ui.Offset(0.0, (neckStretchBaseline - stretch));
        DartRuntimePrimitives.Assert(() => (!_debuggingLabelLocation || ((global::System.Func<bool>)(() =>
        {
            global::Doroti.Ui.Offset leftCenter = ((global::Doroti.Ui.Offset)(object?)((_topLobeCenter - new global::Doroti.Ui.Offset(leftWidthNeeded, 0.0)) + neckStretch));
            global::Doroti.Ui.Offset rightCenter = ((global::Doroti.Ui.Offset)(object?)((_topLobeCenter + new global::Doroti.Ui.Offset(rightWidthNeeded, 0.0)) + neckStretch));
            var valueRect = global::Doroti.Ui.Rect.fromLTRB((leftCenter.dx - _topLobeRadius), (leftCenter.dy - _topLobeRadius), (rightCenter.dx + _topLobeRadius), (rightCenter.dy + _topLobeRadius));
            var outlinePaint = ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = new global::Doroti.Ui.Color(4294901760L);
                __cascade.style = PaintingStyle.stroke;
                __cascade.strokeWidth = 1.0;
                return __cascade;
            }))();
            canvas.drawRect(valueRect, outlinePaint);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))()));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, (leftTopNeckCenter + neckStretch), _topNeckRadius, 0.0, -leftNeckArcAngle);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, ((_topLobeCenter - new global::Doroti.Ui.Offset(leftWidthNeeded, 0.0)) + neckStretch), _topLobeRadius, (_ninetyDegrees + leftTheta), _twoSeventyDegrees);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, ((_topLobeCenter + new global::Doroti.Ui.Offset(rightWidthNeeded, 0.0)) + neckStretch), _topLobeRadius, _twoSeventyDegrees, ((_twoSeventyDegrees + Dart_mathLibrary.pi) - rightTheta));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path, (neckRightCenter + neckStretch), _topNeckRadius, rightNeckArcAngle, Dart_mathLibrary.pi);
        if ((strokePaintColor is not null))
        {
            var strokePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = strokePaintColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            canvas.drawPath(path, strokePaint);
        }
        canvas.drawPath(path, paint);
        canvas.save();
        canvas.translate(shift, (-_distanceBetweenTopBottomCenters + neckStretch.dy));
        canvas.scale(inverseTextScale, inverseTextScale);
        labelPainter.paint(canvas, (Offset.zero - new global::Doroti.Ui.Offset(labelHalfWidth, (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2.0))));
        canvas.restore();
        canvas.restore();
    }

}
