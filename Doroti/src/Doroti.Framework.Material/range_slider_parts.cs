// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/range_slider_parts.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public interface RangeSliderThumbShape
{
    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete);
    public void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isEnabled = default!, bool isOnTop = default!, TextDirection textDirection = default!, SliderThemeData sliderTheme = default!, Thumb thumb = default!, bool isPressed = default!);
}

public abstract class RangeSliderValueIndicatorShape
{
    protected RangeSliderValueIndicatorShape()
    {
    }

    public abstract global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor);
    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox? parentBox = null, Offset? center = null, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, global::Doroti.Framework.Animation.Animation<double>? activationAnimation = null, double? textScaleFactor = null, Size? sizeWithOverflow = null)
    {
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!);
}

public interface RangeSliderTickMarkShape
{
    public global::Doroti.Ui.Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled = default!);
    public void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = default!, TextDirection textDirection = default!);
}

public abstract class RangeSliderTrackShape
{
    protected RangeSliderTrackShape()
    {
    }

    public abstract global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = default!, bool isDiscrete = default!);
    public abstract void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2);
    public virtual bool isRounded => false;
}

public interface BaseRangeSliderTrackShape
{
    public global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false);
}

public class RectangularRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{

    public RectangularRangeSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (enableAnimation is not null));
        var activeTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = activeTrackColorTween.evaluate(enableAnimation!)!;
    return __cascade;
}))();
        var inactivePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
        var (leftThumbOffset, rightThumbOffset) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var leftTrackSegment = global::Doroti.Ui.Rect.fromLTRB(trackRect.left, trackRect.top, leftThumbOffset.dx, trackRect.bottom);
        if (!leftTrackSegment.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(leftTrackSegment, inactivePaint);
        }
        var middleTrackSegment = global::Doroti.Ui.Rect.fromLTRB(leftThumbOffset.dx, trackRect.top, rightThumbOffset.dx, trackRect.bottom);
        if (!middleTrackSegment.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(middleTrackSegment, activePaint);
        }
        var rightTrackSegment = global::Doroti.Ui.Rect.fromLTRB(rightThumbOffset.dx, trackRect.top, trackRect.right, trackRect.bottom);
        if (!rightTrackSegment.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(rightTrackSegment, inactivePaint);
        }
    }

    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbSize.width / 2L)) : ((thumbSize.width / 2L)))));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize.width, overlayWidth) : thumbSize.width)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundedRectRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{

    public RoundedRectRangeSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        if (((sliderTheme.trackHeight is null) || (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) <= 0L)))
        {
            return;
        }
        var activeTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = activeTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
        var inactivePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
        var (leftThumbOffset, rightThumbOffset) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Size thumbSize = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double thumbRadius = (thumbSize.width / 2L);
        DartRuntimePrimitives.Assert(() => (thumbRadius > 0L));
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackRadius = global::Doroti.Ui.Radius.circular((trackRect.height / 2L));
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect.left, trackRect.top, leftThumbOffset.dx, trackRect.bottom, topLeft: trackRadius, bottomLeft: trackRadius), inactivePaint);
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(rightThumbOffset.dx, trackRect.top, trackRect.right, trackRect.bottom, topRight: trackRadius, bottomRight: trackRadius), inactivePaint);
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((leftThumbOffset.dx - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (trackRect.top - ((additionalActiveTrackHeight / 2L))), (rightThumbOffset.dx + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (trackRect.bottom + ((additionalActiveTrackHeight / 2L))), trackRadius), activePaint);
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbSize.width / 2L)) : ((thumbSize.width / 2L)))));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize.width, overlayWidth) : thumbSize.width)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundRangeSliderTickMarkShape : RangeSliderTickMarkShape
{
    public virtual double? tickMarkRadius { get; private set; }

    public RoundRangeSliderTickMarkShape(double? tickMarkRadius = null)
    {
        this.tickMarkRadius = tickMarkRadius;
    }

    public virtual Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        return global::Doroti.Ui.Size.fromRadius((this.tickMarkRadius ?? (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 4L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, TextDirection textDirection = default!)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTickMarkColor is not null));
        bool hasGap = ((sliderTheme.trackGap is not null) && (DartRuntimePrimitives.RequireValue(sliderTheme.trackGap) > 0L));
        bool underThumb = ((startThumbCenter.dx == center.dx) || (endThumbCenter.dx == center.dx));
        if ((hasGap && underThumb))
        {
            return;
        }
        bool isBetweenThumbs = (textDirection switch { TextDirection.ltr => ((startThumbCenter.dx < center.dx) && (center.dx < endThumbCenter.dx)), TextDirection.rtl => ((endThumbCenter.dx < center.dx) && (center.dx < startThumbCenter.dx)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color? beginLocal = ((global::Doroti.Ui.Color?)(object?)(isBetweenThumbs ? sliderTheme.disabledActiveTickMarkColor : sliderTheme.disabledInactiveTickMarkColor));
        global::Doroti.Ui.Color? endLocal = ((global::Doroti.Ui.Color?)(object?)(isBetweenThumbs ? sliderTheme.activeTickMarkColor : sliderTheme.inactiveTickMarkColor));
        var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Framework.Animation.ColorTween(begin: beginLocal, end: endLocal).evaluate(enableAnimation)!;
    return __cascade;
}))();
        double tickMarkRadius = (getPreferredSize(isEnabled: isEnabled, sliderTheme: sliderTheme).width / 2L);
        if ((DartRuntimePrimitives.RequireValue(tickMarkRadius) > 0L))
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(center, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(tickMarkRadius)), paintLocal);
        }
    }

}

public class RoundRangeSliderThumbShape : RangeSliderThumbShape
{
    public virtual double enabledThumbRadius { get; private set; } = default!;
    public virtual double? disabledThumbRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual double pressedElevation { get; private set; } = default!;

    public RoundRangeSliderThumbShape(double enabledThumbRadius = 10.0, double? disabledThumbRadius = null, double elevation = 1.0, double pressedElevation = 6.0)
    {
        this.enabledThumbRadius = enabledThumbRadius;
        this.disabledThumbRadius = disabledThumbRadius;
        this.elevation = elevation;
        this.pressedElevation = pressedElevation;
    }

    internal virtual double _disabledThumbRadius => DartRuntimePrimitives.ConvertValue<double>((this.disabledThumbRadius ?? this.enabledThumbRadius));
    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return global::Doroti.Ui.Size.fromRadius((isEnabled ? this.enabledThumbRadius : this._disabledThumbRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = false, bool isEnabled = false, bool isOnTop = default!, TextDirection textDirection = default!, SliderThemeData sliderTheme = default!, Thumb thumb = default!, bool isPressed = default!)
    {
        var __sliderTheme = (SliderThemeData)(object)textDirection;
        DartRuntimePrimitives.Assert(() => (__sliderTheme.showValueIndicator is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.overlappingShapeStrokeColor is not null));
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: this._disabledThumbRadius, end: this.enabledThumbRadius);
        var colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: __sliderTheme.disabledThumbColor, end: __sliderTheme.thumbColor);
        double radius = radiusTween.evaluate(enableAnimation);
        var elevationTween = new global::Doroti.Framework.Animation.Tween<double>(begin: this.elevation, end: this.pressedElevation);
        if (isOnTop)
        {
            var strokePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = __sliderTheme.overlappingShapeStrokeColor!;
    __cascade.strokeWidth = 1.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
            canvasLocal.drawCircle(center, radius, strokePaint);
        }
        global::Doroti.Ui.Color colorLocal = ((global::Doroti.Ui.Color)(object?)colorTween.evaluate(enableAnimation)!);
        double evaluatedElevation = (DartRuntimePrimitives.RequireValue(isPressed) ? elevationTween.evaluate(activationAnimation) : this.elevation);
        var shadowPath = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addArc(global::Doroti.Ui.Rect.fromCenter(center: center, width: (2L * radius), height: (2L * radius)), 0, (Dart_mathLibrary.pi * 2L));
    return __cascade;
}))();
        var paintShadows = true;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows)
                {
                    Range_slider_partsLibrary._debugDrawShadow(canvasLocal, shadowPath, evaluatedElevation);
                    paintShadows = false;
                }
                return true;
            });
        if (paintShadows)
        {
            canvasLocal.drawShadow(shadowPath, Colors.black, evaluatedElevation, true);
        }
        canvasLocal.drawCircle(center, radius, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = colorLocal;
    return __cascade;
}))());
    }

}

public delegate Thumb? RangeThumbSelector(TextDirection textDirection, RangeValues values, double tapValue, Size thumbSize, Size trackSize, double dx);

public class RangeValues
{
    public virtual double start { get; private set; } = default!;
    public virtual double end { get; private set; } = default!;

    public RangeValues(double start, double end)
    {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object? other)
    {
        var __other = other as RangeValues;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is RangeValues) && (((RangeValues)((RangeValues)__other)).start == this.start)) && (((RangeValues)((RangeValues)__other)).end == this.end));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.start, this.end));
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RangeValues"))}({this.start}, {this.end})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RangeLabels
{
    public virtual string start { get; private set; } = default!;
    public virtual string end { get; private set; } = default!;

    public RangeLabels(string start, string end)
    {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object? other)
    {
        var __other = other as RangeLabels;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is RangeLabels) && (((RangeLabels)((RangeLabels)__other)).start == this.start)) && (((RangeLabels)((RangeLabels)__other)).end == this.end));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.start, this.end));
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "RangeLabels"))}({this.start}, {this.end})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Range_slider_partsLibrary
{
    internal static void _debugDrawShadow(Canvas canvas, Path path, double elevation)
    {
        if ((elevation > 0.0))
        {
            canvas.drawPath(path, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = Colors.black;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = (elevation * 2.0);
    return __cascade;
}))());
        }
    }
}

public class GappedRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{

    public GappedRangeSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        if (((sliderTheme.trackHeight is null) || (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) <= 0L)))
        {
            return;
        }
        var activeTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = activeTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
        var inactivePaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackCornerRadius = global::Doroti.Ui.Radius.circular((trackRect.shortestSide / 2L));
        var trackInsideCornerRadius = global::Doroti.Ui.Radius.circular(2.0);
        var (leftThumbOffset, rightThumbOffset) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Size thumbSize = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double thumbRadius = (thumbSize.width / 2L);
        DartRuntimePrimitives.Assert(() => (thumbRadius > 0L));
        double trackGapLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackGap);
        var trackRRect = global::Doroti.Ui.RRect.fromRectAndCorners(trackRect, topLeft: trackCornerRadius, bottomLeft: trackCornerRadius, topRight: trackCornerRadius, bottomRight: trackCornerRadius);
        var leftRRect = global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect.left, trackRect.top, (leftThumbOffset.dx - trackGapLocal), trackRect.bottom, topLeft: trackCornerRadius, bottomLeft: trackCornerRadius, topRight: trackInsideCornerRadius, bottomRight: trackInsideCornerRadius);
        var rightRRect = global::Doroti.Ui.RRect.fromLTRBAndCorners((rightThumbOffset.dx + trackGapLocal), trackRect.top, trackRect.right, trackRect.bottom, topLeft: trackInsideCornerRadius, bottomLeft: trackInsideCornerRadius, topRight: trackCornerRadius, bottomRight: trackCornerRadius);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.save();
    __cascade.clipRRect(trackRRect);
    return __cascade;
}))());
        bool drawLeftTrack = (startThumbCenter.dx > ((leftRRect.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        bool drawRightTrack = (endThumbCenter.dx < ((rightRRect.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawLeftTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(leftRRect, inactivePaint);
        }
        if (drawRightTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(rightRRect, inactivePaint);
        }
        if (((leftThumbOffset.dx + trackGapLocal) < (rightThumbOffset.dx - trackGapLocal)))
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((leftThumbOffset.dx + trackGapLocal), trackRect.top, (rightThumbOffset.dx - trackGapLocal), trackRect.bottom, trackInsideCornerRadius), activePaint);
        }
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
        var stopIndicatorRadius = 2.0;
        double stopIndicatorTrailingSpace = (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L);
        var startStopIndicatorOffset = new global::Doroti.Ui.Offset((trackRect.centerLeft.dx + stopIndicatorTrailingSpace), ((Offset)((dynamic)trackRect).center).dy);
        var endStopIndicatorOffset = new global::Doroti.Ui.Offset((trackRect.centerRight.dx - stopIndicatorTrailingSpace), ((Offset)((dynamic)trackRect).center).dy);
        bool showStartStopIndicator = (startThumbCenter.dx > startStopIndicatorOffset.dx);
        if ((showStartStopIndicator && !isDiscrete))
        {
            var stopIndicatorRect = global::Doroti.Ui.Rect.fromCircle(center: startStopIndicatorOffset, radius: stopIndicatorRadius);
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRect).center), stopIndicatorRadius, activePaint);
        }
        bool showEndStopIndicator = (endThumbCenter.dx < endStopIndicatorOffset.dx);
        if ((showEndStopIndicator && !isDiscrete))
        {
            var stopIndicatorRectLocal = global::Doroti.Ui.Rect.fromCircle(center: endStopIndicatorOffset, radius: stopIndicatorRadius);
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRectLocal).center), stopIndicatorRadius, activePaint);
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbSize.width / 2L)) : ((thumbSize.width / 2L)))));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize.width, overlayWidth) : thumbSize.width)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class HandleRangeSliderThumbShape : RangeSliderThumbShape
{
    public HandleRangeSliderThumbShape()
    {
    }

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return new global::Doroti.Ui.Size(4.0, 44.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = false, bool isEnabled = false, bool isOnTop = default!, TextDirection textDirection = default!, SliderThemeData sliderTheme = default!, Thumb thumb = default!, bool isPressed = default!)
    {
        var __sliderTheme = (SliderThemeData)(object)textDirection;
        DartRuntimePrimitives.Assert(() => (__sliderTheme.showValueIndicator is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.overlappingShapeStrokeColor is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.disabledThumbColor is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.thumbColor is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.thumbSize is not null));
        var colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: __sliderTheme.disabledThumbColor, end: __sliderTheme.thumbColor);
        global::Doroti.Ui.Color colorLocal = ((global::Doroti.Ui.Color)(object?)colorTween.evaluate(enableAnimation)!);
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        global::Doroti.Ui.Size thumbSizeLocal = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.RequireValue(__sliderTheme.thumbSize!.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())));
        var rrect = global::Doroti.Ui.RRect.fromRectAndRadius(global::Doroti.Ui.Rect.fromCenter(center: center, width: thumbSizeLocal.width, height: thumbSizeLocal.height), global::Doroti.Ui.Radius.circular((thumbSizeLocal.shortestSide / 2L)));
        canvasLocal.drawRRect(rrect, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = colorLocal;
    return __cascade;
}))());
    }

}

public class RoundedRectRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts _pathPainter = new _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts();

    public RoundedRectRangeSliderValueIndicatorShape()
    {
    }

    public override Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => (textScaleFactor >= 0L));
        return _pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)
    {
        DartRuntimePrimitives.Assert(() => true);
        DartRuntimePrimitives.Assert(() => (sizeWithOverflow is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.valueIndicatorColor is not null));
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvasLocal, center: center, scale: scaleLocal, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
    }

}

public class DropRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _DropSliderValueIndicatorPathPainter__range_slider_parts _pathPainter = new _DropSliderValueIndicatorPathPainter__range_slider_parts();

    public DropRangeSliderValueIndicatorShape()
    {
    }

    public override Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => (textScaleFactor >= 0L));
        return _pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = default!, bool isOnTop = default!, global::Doroti.Framework.Painting.TextPainter labelPainter = default!, double textScaleFactor = default!, Size sizeWithOverflow = default!, global::Doroti.Framework.Rendering.RenderBox parentBox = default!, SliderThemeData sliderTheme = default!, TextDirection textDirection = default!, double value = default!, Thumb thumb = default!)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvasLocal, center: center, scale: scaleLocal, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
    }

}

internal class _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts
{
    internal const double _labelPadding = 10.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _rectYOffset = 10.0;
    internal const double _bottomTipYOffset = 16.0;
    internal static double _preferredHalfHeight = (_preferredHeight / 2L);

    internal _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        double widthLocal = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + (((_labelPadding * 2L)) * textScaleFactor));
        return new global::Doroti.Ui.Size(widthLocal, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding = 8.0;
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
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

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L)));
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
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth / 2L) + horizontalShift), (-_rectYOffset - _preferredHeight), rectangleWidth, _preferredHeight);
        var fillPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundPaintColor;
    return __cascade;
}))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        var rrect = global::Doroti.Ui.RRect.fromRectAndRadius(upperRect, global::Doroti.Ui.Radius.circular((upperRect.height / 2L)));
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
            canvas.drawRRect(rrect, strokePaint);
        }
        canvas.drawRRect(rrect, fillPaint);
        double bottomTipToUpperRectTranslateY = ((-_preferredHalfHeight / 2L) - upperRect.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY);
        var boxCenter = new global::Doroti.Ui.Offset(horizontalShift, (upperRect.height / 2.3));
        var halfLabelPainterOffset = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset = ((global::Doroti.Ui.Offset)(object?)(boxCenter - halfLabelPainterOffset));
        labelPainter.paint(canvas, labelOffset);
        canvas.restore();
    }

}

internal class _DropSliderValueIndicatorPathPainter__range_slider_parts
{
    internal const double _triangleHeight = 10.0;
    internal const double _labelPadding = 8.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 20.0;
    internal const double _minRectHeight = 28.0;
    internal const double _rectYOffset = 6.0;
    internal const double _bottomTipYOffset = 16.0;
    internal static double _preferredHalfHeight = (_preferredHeight / 2L);
    internal const double _upperRectRadius = 4;

    internal _DropSliderValueIndicatorPathPainter__range_slider_parts()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        double widthLocal = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L) * textScaleFactor));
        return new global::Doroti.Ui.Size(widthLocal, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding = 8.0;
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
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

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + _labelPadding);
        return (unscaledWidth * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _adjustBorderRadius(Rect rect)
    {
        var rectness = 0.0;
        return BorderRadius.lerp(global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(_upperRectRadius)), global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((rect.shortestSide / 2.0))), (1.0 - rectness))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth / 2L) + horizontalShift), (-_rectYOffset - _minRectHeight), rectangleWidth, _minRectHeight);
        var fillPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = backgroundPaintColor;
    return __cascade;
}))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        global::Doroti.Framework.Painting.BorderRadius adjustedBorderRadius = ((global::Doroti.Framework.Painting.BorderRadius)(object?)_adjustBorderRadius(upperRect));
        global::Doroti.Ui.RRect borderRect = ((global::Doroti.Ui.RRect)(object?)adjustedBorderRadius.resolve(((global::Doroti.Framework.Painting.TextPainter)labelPainter).textDirection).toRRect(upperRect));
        var trianglePath = ((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.lineTo(-_triangleHeight, -_triangleHeight);
    __cascade.lineTo(_triangleHeight, -_triangleHeight);
    __cascade.close();
    return __cascade;
}))();
        trianglePath.addRRect(borderRect);
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
        var boxCenter = new global::Doroti.Ui.Offset(horizontalShift, (upperRect.height / 1.75));
        var halfLabelPainterOffset = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset = ((global::Doroti.Ui.Offset)(object?)(boxCenter - halfLabelPainterOffset));
        labelPainter.paint(canvas, labelOffset);
        canvas.restore();
    }

}
