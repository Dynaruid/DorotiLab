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
        return new global::Doroti.Ui.Size(this.overlayRadius);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvas__8800 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween__8835 = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: this.overlayRadius);
        canvas__8800.drawCircle(center, radiusTween__8835.evaluate(activationAnimation), ((Func<Paint>)(() =>
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
        global::Doroti.Ui.Canvas canvas__10676 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scale__10718 = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvas__10676, center: center, scale: scale__10718, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
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
        global::Doroti.Ui.Canvas canvas__13212 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scale__13254 = activationAnimation!.value;
        _pathPainter.paint(parentBox: parentBox!, canvas: canvas__13212, center: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(center)), scale: scale__13254, labelPainter: labelPainter!, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme!.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
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
        var edgePadding__14686 = 8.0;
        double rectangleWidth__14722 = _upperRectangleWidth(labelPainter, scale, textScaleFactor);
        global::Doroti.Ui.Offset globalCenter__14976 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft__15344 = Math.Max(0, (((rectangleWidth__14722 / 2L) - globalCenter__14976.dx) + edgePadding__14686));
        double overflowRight__15441 = Math.Max(0, ((rectangleWidth__14722 / 2L) - (((sizeWithOverflow.width - globalCenter__14976.dx) - edgePadding__14686))));
        if ((rectangleWidth__14722 < sizeWithOverflow.width))
        {
            return (overflowLeft__15344 - overflowRight__15441);
        }
        else
        {
            if (((overflowLeft__15344 - overflowRight__15441) > 0L))
            {
                return (overflowLeft__15344 - ((edgePadding__14686 * textScaleFactor)));
            }
            else
            {
                return (-overflowRight__15441 + ((edgePadding__14686 * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale, double textScaleFactor)
    {
        double unscaledWidth__15975 = (Math.Max((_minLabelWidth * textScaleFactor), ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + (_labelPadding * 2L));
        return (unscaledWidth__15975 * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth__16635 = _upperRectangleWidth(labelPainter, scale, textScaleFactor);
        double horizontalShift__16729 = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        double rectHeight__16978 = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height + _labelPadding);
        var upperRect__17038 = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth__16635 / 2L) + horizontalShift__16729), (-_triangleHeight - rectHeight__16978), rectangleWidth__16635, rectHeight__16978);
        var trianglePath__17205 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.lineTo(-_triangleHeight, -_triangleHeight);
    __cascade.lineTo(_triangleHeight, -_triangleHeight);
    __cascade.close();
    return __cascade;
}))();
        var fillPaint__17355 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundPaintColor;
    return __cascade;
}))();
        var upperRRect__17416 = global::Doroti.Ui.RRect.fromRectAndRadius(upperRect__17038, global::Doroti.Ui.Radius.circular(_upperRectRadius));
        trianglePath__17205.addRRect(upperRRect__17416);
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        if ((strokePaintColor is not null))
        {
            var strokePaint__17817 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = strokePaintColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            canvas.drawPath(trianglePath__17205, strokePaint__17817);
        }
        canvas.drawPath(trianglePath__17205, fillPaint__17355);
        double bottomTipToUpperRectTranslateY__18124 = ((-_preferredHalfHeight / 2L) - upperRect__17038.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY__18124);
        var boxCenter__18270 = new global::Doroti.Ui.Offset(horizontalShift__16729, (upperRect__17038.height / 2L));
        var halfLabelPainterOffset__18339 = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset__18438 = ((global::Doroti.Ui.Offset)(object?)(boxCenter__18270 - halfLabelPainterOffset__18339));
        labelPainter.paint(canvas, labelOffset__18438);
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
        var enableColor__20255 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.valueIndicatorColor);
        _pathPainter.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, center, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = enableColor__20255.evaluate(enableAnimation)!;
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
        var enableColor__22823 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.valueIndicatorColor);
        _pathPainter.paint(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(center)), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = enableColor__22823.evaluate(enableAnimation)!;
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
        double width__25719 = (Math.Max((_minLabelWidth * textScaleFactor), ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L) * textScaleFactor));
        return new global::Doroti.Ui.Size(width__25719, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static void _addArc(Path path, Offset center, double radius, double startAngle, double endAngle)
    {
        DartRuntimePrimitives.Assert(() => center.isFinite);
        var arcRect__26184 = global::Doroti.Ui.Rect.fromCircle(center: center, radius: radius);
        path.arcTo(arcRect__26184, startAngle, (endAngle - startAngle), false);
    }

    public virtual double getHorizontalShift(Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double scale, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double inverseTextScale__26575 = ((textScaleFactor != 0L) ? (1.0 / textScaleFactor) : 0.0);
        double labelHalfWidth__26663 = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2.0);
        double halfWidthNeeded__26723 = Math.Max(0.0, ((inverseTextScale__26575 * labelHalfWidth__26663) - ((_topLobeRadius - _labelPadding))));
        double shift__26862 = _getIdealOffset(halfWidthNeeded__26723, (textScaleFactor * scale), center, sizeWithOverflow.width);
        return (shift__26862 * textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getIdealOffset(double halfWidthNeeded, double scale, Offset center, double widthWithOverflow)
    {
        var edgeMargin__27329 = 8.0;
        var topLobeRect__27357 = global::Doroti.Ui.Rect.fromLTWH((-_topLobeRadius - halfWidthNeeded), (-_topLobeRadius - _distanceBetweenTopBottomCenters), (2.0 * ((_topLobeRadius + halfWidthNeeded))), (2.0 * _topLobeRadius));
        global::Doroti.Ui.Offset topLeft__27688 = ((global::Doroti.Ui.Offset)(object?)(((topLobeRect__27357.topLeft * scale)) + center));
        global::Doroti.Ui.Offset bottomRight__27755 = ((global::Doroti.Ui.Offset)(object?)(((topLobeRect__27357.bottomRight * scale)) + center));
        var shift__27821 = 0.0;
        if ((topLeft__27688.dx < edgeMargin__27329))
        {
            shift__27821 = (edgeMargin__27329 - topLeft__27688.dx);
        }
        var endGlobal__27926 = widthWithOverflow;
        if ((bottomRight__27755.dx > (endGlobal__27926 - edgeMargin__27329)))
        {
            shift__27821 = ((endGlobal__27926 - edgeMargin__27329) - bottomRight__27755.dx);
        }
        shift__27821 = ((scale == 0.0) ? 0.0 : (shift__27821 / scale));
        if ((shift__27821 < 0.0))
        {
            shift__27821 = Math.Max(shift__27821, -halfWidthNeeded);
        }
        else
        {
            shift__27821 = Math.Min(shift__27821, halfWidthNeeded);
        }
        return shift__27821;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(Canvas canvas, Offset center, Paint paint, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color? strokePaintColor)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double overallScale__28981 = (scale * textScaleFactor);
        double inverseTextScale__29038 = ((textScaleFactor != 0L) ? (1.0 / textScaleFactor) : 0.0);
        double labelHalfWidth__29126 = (((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2.0);
        canvas.save();
        canvas.translate(center.dx, center.dy);
        canvas.scale(overallScale__28981, overallScale__28981);
        double bottomNeckTriangleHypotenuse__29297 = (_bottomNeckRadius + (_bottomLobeRadius / overallScale__28981));
        double rightBottomNeckCenterY__29407 = -global::Doroti.Runtime.Dart_mathLibrary.sqrt((global::Doroti.Runtime.Dart_mathLibrary.pow(bottomNeckTriangleHypotenuse__29297, 2L) - global::Doroti.Runtime.Dart_mathLibrary.pow(_rightBottomNeckCenterX, 2L)));
        double rightBottomNeckAngleEnd__29556 = (Dart_mathLibrary.pi + global::Doroti.Runtime.Dart_mathLibrary.atan((rightBottomNeckCenterY__29407 / _rightBottomNeckCenterX)));
        var path__29671 = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.moveTo((_middleNeckWidth / 2L), rightBottomNeckCenterY__29407);
    return __cascade;
}))();
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, new global::Doroti.Ui.Offset(_rightBottomNeckCenterX, rightBottomNeckCenterY__29407), _bottomNeckRadius, _rightBottomNeckAngleStart, rightBottomNeckAngleEnd__29556);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, Offset.zero, (_bottomLobeRadius / overallScale__28981), (rightBottomNeckAngleEnd__29556 - Dart_mathLibrary.pi), ((2L * Dart_mathLibrary.pi) - rightBottomNeckAngleEnd__29556));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, new global::Doroti.Ui.Offset(-_rightBottomNeckCenterX, rightBottomNeckCenterY__29407), _bottomNeckRadius, (Dart_mathLibrary.pi - rightBottomNeckAngleEnd__29556), 0);
        double halfWidthNeeded__30444 = Math.Max(0.0, ((inverseTextScale__29038 * labelHalfWidth__29126) - ((_topLobeRadius - _labelPadding))));
        double shift__30584 = _getIdealOffset(halfWidthNeeded__30444, overallScale__28981, center, sizeWithOverflow.width);
        double leftWidthNeeded__30720 = (halfWidthNeeded__30444 - shift__30584);
        double rightWidthNeeded__30780 = (halfWidthNeeded__30444 + shift__30584);
        double leftAmount__30946 = Math.Max(0.0, Math.Min(1.0, (leftWidthNeeded__30720 / _neckTriangleBase)));
        double rightAmount__31043 = Math.Max(0.0, Math.Min(1.0, (rightWidthNeeded__30780 / _neckTriangleBase)));
        double leftTheta__31348 = (((1.0 - leftAmount__30946)) * _thirtyDegrees);
        double rightTheta__31414 = (((1.0 - rightAmount__31043)) * _thirtyDegrees);
        var leftTopNeckCenter__31519 = new global::Doroti.Ui.Offset(-_neckTriangleBase, (_topLobeCenter.dy + (global::Doroti.Runtime.Dart_mathLibrary.cos(leftTheta__31348) * _neckTriangleHypotenuse)));
        var neckRightCenter__31663 = new global::Doroti.Ui.Offset(_neckTriangleBase, (_topLobeCenter.dy + (global::Doroti.Runtime.Dart_mathLibrary.cos(rightTheta__31414) * _neckTriangleHypotenuse)));
        double leftNeckArcAngle__31812 = (_ninetyDegrees - leftTheta__31348);
        double rightNeckArcAngle__31876 = ((Dart_mathLibrary.pi + _ninetyDegrees) - rightTheta__31414);
        double neckStretchBaseline__32146 = Math.Max(0.0, (rightBottomNeckCenterY__29407 - Math.Max(leftTopNeckCenter__31519.dy, neckRightCenter__31663.dy)));
        var t__32289 = ((double)global::Doroti.Runtime.Dart_mathLibrary.pow(inverseTextScale__29038, 3.0));
        double stretch__32353 = Dart_uiLibrary.clampDouble((neckStretchBaseline__32146 * t__32289), 0.0, (10.0 * neckStretchBaseline__32146));
        var neckStretch__32444 = new global::Doroti.Ui.Offset(0.0, (neckStretchBaseline__32146 - stretch__32353));
        DartRuntimePrimitives.Assert(() => (!_debuggingLabelLocation || ((global::System.Func<bool>)(() =>
        {
            global::Doroti.Ui.Offset leftCenter__32589 = ((global::Doroti.Ui.Offset)(object?)((_topLobeCenter - new global::Doroti.Ui.Offset(leftWidthNeeded__30720, 0.0)) + neckStretch__32444));
            global::Doroti.Ui.Offset rightCenter__32688 = ((global::Doroti.Ui.Offset)(object?)((_topLobeCenter + new global::Doroti.Ui.Offset(rightWidthNeeded__30780, 0.0)) + neckStretch__32444));
            var valueRect__32782 = global::Doroti.Ui.Rect.fromLTRB((leftCenter__32589.dx - _topLobeRadius), (leftCenter__32589.dy - _topLobeRadius), (rightCenter__32688.dx + _topLobeRadius), (rightCenter__32688.dy + _topLobeRadius));
            var outlinePaint__33028 = ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = new global::Doroti.Ui.Color(4294901760L);
                __cascade.style = PaintingStyle.stroke;
                __cascade.strokeWidth = 1.0;
                return __cascade;
            }))();
            canvas.drawRect(valueRect__32782, outlinePaint__33028);
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))()));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, (leftTopNeckCenter__31519 + neckStretch__32444), _topNeckRadius, 0.0, -leftNeckArcAngle__31812);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, ((_topLobeCenter - new global::Doroti.Ui.Offset(leftWidthNeeded__30720, 0.0)) + neckStretch__32444), _topLobeRadius, (_ninetyDegrees + leftTheta__31348), _twoSeventyDegrees);
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, ((_topLobeCenter + new global::Doroti.Ui.Offset(rightWidthNeeded__30780, 0.0)) + neckStretch__32444), _topLobeRadius, _twoSeventyDegrees, ((_twoSeventyDegrees + Dart_mathLibrary.pi) - rightTheta__31414));
        _PaddleSliderValueIndicatorPathPainter__slider_value_indicator_shape._addArc(path__29671, (neckRightCenter__31663 + neckStretch__32444), _topNeckRadius, rightNeckArcAngle__31876, Dart_mathLibrary.pi);
        if ((strokePaintColor is not null))
        {
            var strokePaint__33894 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = strokePaintColor;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            canvas.drawPath(path__29671, strokePaint__33894);
        }
        canvas.drawPath(path__29671, paint);
        canvas.save();
        canvas.translate(shift__30584, (-_distanceBetweenTopBottomCenters + neckStretch__32444.dy));
        canvas.scale(inverseTextScale__29038, inverseTextScale__29038);
        labelPainter.paint(canvas, (Offset.zero - new global::Doroti.Ui.Offset(labelHalfWidth__29126, (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2.0))));
        canvas.restore();
        canvas.restore();
    }

}
