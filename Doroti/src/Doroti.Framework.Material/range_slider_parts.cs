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
        var activeTrackColorTween__21179 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__21324 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__21475 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__21179.evaluate(enableAnimation!)!;
            return __cascade;        }))();
        var inactivePaint__21567 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__21324.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var (leftThumbOffset__21671, rightThumbOffset__21695) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect__21889 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var leftTrackSegment__22076 = global::Doroti.Ui.Rect.fromLTRB(trackRect__21889.left, trackRect__21889.top, leftThumbOffset__21671.dx, trackRect__21889.bottom);
        if (!leftTrackSegment__22076.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(leftTrackSegment__22076, inactivePaint__21567);
        }
        var middleTrackSegment__22327 = global::Doroti.Ui.Rect.fromLTRB(leftThumbOffset__21671.dx, trackRect__21889.top, rightThumbOffset__21695.dx, trackRect__21889.bottom);
        if (!middleTrackSegment__22327.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(middleTrackSegment__22327, activePaint__21475);
        }
        var rightTrackSegment__22587 = global::Doroti.Ui.Rect.fromLTRB(rightThumbOffset__21695.dx, trackRect__21889.top, trackRect__21889.right, trackRect__21889.bottom);
        if (!rightTrackSegment__22587.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(rightTrackSegment__22587, inactivePaint__21567);
        }
    }

    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize__17286 = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth__17385 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__17502 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__17385 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__17502 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__17502 = 0;
        }
        double trackLeft__17898 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__17385 / 2L), (thumbSize__17286.width / 2L)) : ((thumbSize__17286.width / 2L)))));
        double trackTop__18084 = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__17502)) / 2L));
        double trackRight__18167 = ((trackLeft__17898 + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize__17286.width, overlayWidth__17385) : thumbSize__17286.width)));
        double trackBottom__18347 = (trackTop__18084 + trackHeight__17502);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__17898, trackRight__18167), trackTop__18084, Math.Max(trackLeft__17898, trackRight__18167), trackBottom__18347);
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
        var activeTrackColorTween__25538 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__25683 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__25834 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__25538.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var inactivePaint__25925 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__25683.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var (leftThumbOffset__26029, rightThumbOffset__26053) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Size thumbSize__26246 = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double thumbRadius__26345 = (thumbSize__26246.width / 2L);
        DartRuntimePrimitives.Assert(() => (thumbRadius__26345 > 0L));
        global::Doroti.Ui.Rect trackRect__26425 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackRadius__26613 = global::Doroti.Ui.Radius.circular((trackRect__26425.height / 2L));
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect__26425.left, trackRect__26425.top, leftThumbOffset__26029.dx, trackRect__26425.bottom, topLeft: trackRadius__26613, bottomLeft: trackRadius__26613), inactivePaint__25925);
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(rightThumbOffset__26053.dx, trackRect__26425.top, trackRect__26425.right, trackRect__26425.bottom, topRight: trackRadius__26613, bottomRight: trackRadius__26613), inactivePaint__25925);
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((leftThumbOffset__26029.dx - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (trackRect__26425.top - ((additionalActiveTrackHeight / 2L))), (rightThumbOffset__26053.dx + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (trackRect__26425.bottom + ((additionalActiveTrackHeight / 2L))), trackRadius__26613), activePaint__25834);
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize__17286 = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth__17385 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__17502 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__17385 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__17502 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__17502 = 0;
        }
        double trackLeft__17898 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__17385 / 2L), (thumbSize__17286.width / 2L)) : ((thumbSize__17286.width / 2L)))));
        double trackTop__18084 = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__17502)) / 2L));
        double trackRight__18167 = ((trackLeft__17898 + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize__17286.width, overlayWidth__17385) : thumbSize__17286.width)));
        double trackBottom__18347 = (trackTop__18084 + trackHeight__17502);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__17898, trackRight__18167), trackTop__18084, Math.Max(trackLeft__17898, trackRight__18167), trackBottom__18347);
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
        return new global::Doroti.Ui.Size((this.tickMarkRadius ?? (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 4L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset startThumbCenter, Offset endThumbCenter, bool isEnabled = false, TextDirection textDirection = default!)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTickMarkColor is not null));
        bool hasGap__29778 = ((sliderTheme.trackGap is not null) && (DartRuntimePrimitives.RequireValue(sliderTheme.trackGap) > 0L));
        bool underThumb__29861 = ((startThumbCenter.dx == center.dx) || (endThumbCenter.dx == center.dx));
        if ((hasGap__29778 && underThumb__29861))
        {
            return;
        }
        bool isBetweenThumbs__30009 = (textDirection switch { TextDirection.ltr => ((startThumbCenter.dx < center.dx) && (center.dx < endThumbCenter.dx)), TextDirection.rtl => ((endThumbCenter.dx < center.dx) && (center.dx < startThumbCenter.dx)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Color? begin__30262 = ((global::Doroti.Ui.Color?)(object?)(isBetweenThumbs__30009 ? sliderTheme.disabledActiveTickMarkColor : sliderTheme.disabledInactiveTickMarkColor));
        global::Doroti.Ui.Color? end__30406 = ((global::Doroti.Ui.Color?)(object?)(isBetweenThumbs__30009 ? sliderTheme.activeTickMarkColor : sliderTheme.inactiveTickMarkColor));
        var paint__30525 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Framework.Animation.ColorTween(begin: begin__30262, end: end__30406).evaluate(enableAnimation)!;
            return __cascade;        }))();
        double tickMarkRadius__30709 = (getPreferredSize(isEnabled: isEnabled, sliderTheme: sliderTheme).width / 2L);
        if ((DartRuntimePrimitives.RequireValue(tickMarkRadius__30709) > 0L))
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(center, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(tickMarkRadius__30709)), paint__30525);
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
        return new global::Doroti.Ui.Size((isEnabled ? this.enabledThumbRadius : this._disabledThumbRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete = false, bool isEnabled = false, bool isOnTop = default!, TextDirection textDirection = default!, SliderThemeData sliderTheme = default!, Thumb thumb = default!, bool isPressed = default!)
    {
        var __sliderTheme = (SliderThemeData)(object)textDirection;
        DartRuntimePrimitives.Assert(() => (__sliderTheme.showValueIndicator is not null));
        DartRuntimePrimitives.Assert(() => (__sliderTheme.overlappingShapeStrokeColor is not null));
        global::Doroti.Ui.Canvas canvas__33126 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween__33161 = new global::Doroti.Framework.Animation.Tween<double>(begin: this._disabledThumbRadius, end: this.enabledThumbRadius);
        var colorTween__33254 = new global::Doroti.Framework.Animation.ColorTween(begin: __sliderTheme.disabledThumbColor, end: __sliderTheme.thumbColor);
        double radius__33383 = radiusTween__33161.evaluate(enableAnimation);
        var elevationTween__33441 = new global::Doroti.Framework.Animation.Tween<double>(begin: this.elevation, end: this.pressedElevation);
        if (isOnTop)
        {
            var strokePaint__33652 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = __sliderTheme.overlappingShapeStrokeColor!;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            canvas__33126.drawCircle(center, radius__33383, strokePaint__33652);
        }
        global::Doroti.Ui.Color color__33878 = ((global::Doroti.Ui.Color)(object?)colorTween__33254.evaluate(enableAnimation)!);
        double evaluatedElevation__33943 = (DartRuntimePrimitives.RequireValue(isPressed) ? elevationTween__33441.evaluate(activationAnimation) : this.elevation);
        var shadowPath__34061 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.addArc(global::Doroti.Ui.Rect.fromCenter(center: center, width: (2L * radius__33383), height: (2L * radius__33383)), 0, (Dart_mathLibrary.pi * 2L));
            return __cascade;        }))();
        var paintShadows__34227 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Painting.DebugLibrary.debugDisableShadows)
                {
                    Range_slider_partsLibrary._debugDrawShadow(canvas__33126, shadowPath__34061, evaluatedElevation__33943);
                    paintShadows__34227 = false;
                }
                return true;
            });
        if (paintShadows__34227)
        {
            canvas__33126.drawShadow(shadowPath__34061, Colors.black, evaluatedElevation__33943, true);
        }
        canvas__33126.drawCircle(center, radius__33383, ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = color__33878;
            return __cascade;        }))());
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
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = Colors.black;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = (elevation * 2.0);
            return __cascade;        }))());
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
        var activeTrackColorTween__39798 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__39943 = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__40095 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__39798.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var inactivePaint__40186 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__39943.evaluate(enableAnimation)!;
            return __cascade;        }))();
        global::Doroti.Ui.Rect trackRect__40287 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackCornerRadius__40475 = global::Doroti.Ui.Radius.circular((trackRect__40287.shortestSide / 2L));
        var trackInsideCornerRadius__40550 = global::Doroti.Ui.Radius.circular(2.0);
        var (leftThumbOffset__40617, rightThumbOffset__40641) = (textDirection switch { TextDirection.ltr => (((Offset, Offset))((startThumbCenter, endThumbCenter))), TextDirection.rtl => (((Offset, Offset))((endThumbCenter, startThumbCenter))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Size thumbSize__40835 = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double thumbRadius__40934 = (thumbSize__40835.width / 2L);
        DartRuntimePrimitives.Assert(() => (thumbRadius__40934 > 0L));
        double trackGap__41015 = DartRuntimePrimitives.RequireValue(sliderTheme.trackGap);
        var trackRRect__41060 = global::Doroti.Ui.RRect.fromRectAndCorners(trackRect__40287, topLeft: trackCornerRadius__40475, bottomLeft: trackCornerRadius__40475, topRight: trackCornerRadius__40475, bottomRight: trackCornerRadius__40475);
        var leftRRect__41278 = global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect__40287.left, trackRect__40287.top, (leftThumbOffset__40617.dx - trackGap__41015), trackRect__40287.bottom, topLeft: trackCornerRadius__40475, bottomLeft: trackCornerRadius__40475, topRight: trackInsideCornerRadius__40550, bottomRight: trackInsideCornerRadius__40550);
        var rightRRect__41594 = global::Doroti.Ui.RRect.fromLTRBAndCorners((rightThumbOffset__40641.dx + trackGap__41015), trackRect__40287.top, trackRect__40287.right, trackRect__40287.bottom, topLeft: trackInsideCornerRadius__40550, bottomLeft: trackInsideCornerRadius__40550, topRight: trackCornerRadius__40475, bottomRight: trackCornerRadius__40475);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.save();
            __cascade.clipRRect(trackRRect__41060);
            return __cascade;        }))());
        bool drawLeftTrack__41983 = (startThumbCenter.dx > ((leftRRect__41278.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        bool drawRightTrack__42095 = (endThumbCenter.dx < ((rightRRect__41594.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawLeftTrack__41983)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(leftRRect__41278, inactivePaint__40186);
        }
        if (drawRightTrack__42095)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(rightRRect__41594, inactivePaint__40186);
        }
        if (((leftThumbOffset__40617.dx + trackGap__41015) < (rightThumbOffset__40641.dx - trackGap__41015)))
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((leftThumbOffset__40617.dx + trackGap__41015), trackRect__40287.top, (rightThumbOffset__40641.dx - trackGap__41015), trackRect__40287.bottom, trackInsideCornerRadius__40550), activePaint__40095);
        }
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
        var stopIndicatorRadius__42766 = 2.0;
        double stopIndicatorTrailingSpace__42810 = (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L);
        var startStopIndicatorOffset__42879 = new global::Doroti.Ui.Offset((trackRect__40287.centerLeft.dx + stopIndicatorTrailingSpace__42810), ((Offset)((dynamic)trackRect__40287).center).dy);
        var endStopIndicatorOffset__43018 = new global::Doroti.Ui.Offset((trackRect__40287.centerRight.dx - stopIndicatorTrailingSpace__42810), ((Offset)((dynamic)trackRect__40287).center).dy);
        bool showStartStopIndicator__43162 = (startThumbCenter.dx > startStopIndicatorOffset__42879.dx);
        if ((showStartStopIndicator__43162 && !isDiscrete))
        {
            var stopIndicatorRect__43299 = global::Doroti.Ui.Rect.fromCircle(center: startStopIndicatorOffset__42879, radius: stopIndicatorRadius__42766);
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRect__43299).center), stopIndicatorRadius__42766, activePaint__40095);
        }
        bool showEndStopIndicator__43539 = (endThumbCenter.dx < endStopIndicatorOffset__43018.dx);
        if ((showEndStopIndicator__43539 && !isDiscrete))
        {
            var stopIndicatorRect__43668 = global::Doroti.Ui.Rect.fromCircle(center: endStopIndicatorOffset__43018, radius: stopIndicatorRadius__42766);
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRect__43668).center), stopIndicatorRadius__42766, activePaint__40095);
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.rangeThumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.overlayShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        global::Doroti.Ui.Size thumbSize__17286 = ((global::Doroti.Ui.Size)(object?)sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete));
        double overlayWidth__17385 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__17502 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__17385 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__17502 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__17502 = 0;
        }
        double trackLeft__17898 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__17385 / 2L), (thumbSize__17286.width / 2L)) : ((thumbSize__17286.width / 2L)))));
        double trackTop__18084 = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__17502)) / 2L));
        double trackRight__18167 = ((trackLeft__17898 + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbSize__17286.width, overlayWidth__17385) : thumbSize__17286.width)));
        double trackBottom__18347 = (trackTop__18084 + trackHeight__17502);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__17898, trackRight__18167), trackTop__18084, Math.Max(trackLeft__17898, trackRight__18167), trackBottom__18347);
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
        var colorTween__45688 = new global::Doroti.Framework.Animation.ColorTween(begin: __sliderTheme.disabledThumbColor, end: __sliderTheme.thumbColor);
        global::Doroti.Ui.Color color__45816 = ((global::Doroti.Ui.Color)(object?)colorTween__45688.evaluate(enableAnimation)!);
        global::Doroti.Ui.Canvas canvas__45880 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        global::Doroti.Ui.Size thumbSize__45921 = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.RequireValue(__sliderTheme.thumbSize!.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())));
        var rrect__46047 = global::Doroti.Ui.RRect.fromRectAndRadius(global::Doroti.Ui.Rect.fromCenter(center: center, width: thumbSize__45921.width, height: thumbSize__45921.height), global::Doroti.Ui.Radius.circular((thumbSize__45921.shortestSide / 2L)));
        canvas__45880.drawRRect(rrect__46047, ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = color__45816;
            return __cascade;        }))());
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
        global::Doroti.Ui.Canvas canvas__48766 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scale__48808 = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvas__48766, center: center, scale: scale__48808, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
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
        global::Doroti.Ui.Canvas canvas__51383 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scale__51425 = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvas__51383, center: center, scale: scale__51425, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(textScaleFactor), sizeWithOverflow: DartRuntimePrimitives.RequireValue(sizeWithOverflow), backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: (DartRuntimePrimitives.RequireValue(isOnTop) ? sliderTheme.overlappingShapeStrokeColor : sliderTheme.valueIndicatorStrokeColor));
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
        double width__52402 = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + (((_labelPadding * 2L)) * textScaleFactor));
        return new global::Doroti.Ui.Size(width__52402, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding__52857 = 8.0;
        double rectangleWidth__52893 = _upperRectangleWidth(labelPainter, scale);
        global::Doroti.Ui.Offset globalCenter__53130 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft__53498 = Math.Max(0, (((rectangleWidth__52893 / 2L) - globalCenter__53130.dx) + edgePadding__52857));
        double overflowRight__53595 = Math.Max(0, ((rectangleWidth__52893 / 2L) - (((sizeWithOverflow.width - globalCenter__53130.dx) - edgePadding__52857))));
        if ((rectangleWidth__52893 < sizeWithOverflow.width))
        {
            return (overflowLeft__53498 - overflowRight__53595);
        }
        else
        {
            if (((overflowLeft__53498 - overflowRight__53595) > 0L))
            {
                return (overflowLeft__53498 - ((edgePadding__52857 * textScaleFactor)));
            }
            else
            {
                return (-overflowRight__53595 + ((edgePadding__52857 * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth__54105 = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L)));
        return (unscaledWidth__54105 * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth__54741 = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift__54818 = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect__55060 = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth__54741 / 2L) + horizontalShift__54818), (-_rectYOffset - _preferredHeight), rectangleWidth__54741, _preferredHeight);
        var fillPaint__55236 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = backgroundPaintColor;
            return __cascade;        }))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        var rrect__55522 = global::Doroti.Ui.RRect.fromRectAndRadius(upperRect__55060, global::Doroti.Ui.Radius.circular((upperRect__55060.height / 2L)));
        if ((strokePaintColor is not null))
        {
            var strokePaint__55653 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = strokePaintColor;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            canvas.drawRRect(rrect__55522, strokePaint__55653);
        }
        canvas.drawRRect(rrect__55522, fillPaint__55236);
        double bottomTipToUpperRectTranslateY__55949 = ((-_preferredHalfHeight / 2L) - upperRect__55060.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY__55949);
        var boxCenter__56095 = new global::Doroti.Ui.Offset(horizontalShift__54818, (upperRect__55060.height / 2.3));
        var halfLabelPainterOffset__56166 = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset__56265 = ((global::Doroti.Ui.Offset)(object?)(boxCenter__56095 - halfLabelPainterOffset__56166));
        labelPainter.paint(canvas, labelOffset__56265);
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
        double width__57004 = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L) * textScaleFactor));
        return new global::Doroti.Ui.Size(width__57004, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding__57457 = 8.0;
        double rectangleWidth__57493 = _upperRectangleWidth(labelPainter, scale);
        global::Doroti.Ui.Offset globalCenter__57730 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft__58098 = Math.Max(0, (((rectangleWidth__57493 / 2L) - globalCenter__57730.dx) + edgePadding__57457));
        double overflowRight__58195 = Math.Max(0, ((rectangleWidth__57493 / 2L) - (((sizeWithOverflow.width - globalCenter__57730.dx) - edgePadding__57457))));
        if ((rectangleWidth__57493 < sizeWithOverflow.width))
        {
            return (overflowLeft__58098 - overflowRight__58195);
        }
        else
        {
            if (((overflowLeft__58098 - overflowRight__58195) > 0L))
            {
                return (overflowLeft__58098 - ((edgePadding__57457 * textScaleFactor)));
            }
            else
            {
                return (-overflowRight__58195 + ((edgePadding__57457 * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth__58705 = (Math.Max(_minLabelWidth, ((global::Doroti.Framework.Painting.TextPainter)labelPainter).width) + _labelPadding);
        return (unscaledWidth__58705 * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.BorderRadius _adjustBorderRadius(Rect rect)
    {
        var rectness__58880 = 0.0;
        return BorderRadius.lerp(global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(_upperRectRadius)), global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((rect.shortestSide / 2.0))), (1.0 - rectness__58880))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth__59604 = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift__59681 = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect__59922 = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth__59604 / 2L) + horizontalShift__59681), (-_rectYOffset - _minRectHeight), rectangleWidth__59604, _minRectHeight);
        var fillPaint__60094 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = backgroundPaintColor;
            return __cascade;        }))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        global::Doroti.Framework.Painting.BorderRadius adjustedBorderRadius__60285 = ((global::Doroti.Framework.Painting.BorderRadius)(object?)_adjustBorderRadius(upperRect__59922));
        global::Doroti.Ui.RRect borderRect__60356 = ((global::Doroti.Ui.RRect)(object?)adjustedBorderRadius__60285.resolve(((global::Doroti.Framework.Painting.TextPainter)labelPainter).textDirection).toRRect(upperRect__59922));
        var trianglePath__60474 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.lineTo(-_triangleHeight, -_triangleHeight);
            __cascade.lineTo(_triangleHeight, -_triangleHeight);
            __cascade.close();
            return __cascade;        }))();
        trianglePath__60474.addRRect(borderRect__60356);
        if ((strokePaintColor is not null))
        {
            var strokePaint__60702 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = strokePaintColor;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            canvas.drawPath(trianglePath__60474, strokePaint__60702);
        }
        canvas.drawPath(trianglePath__60474, fillPaint__60094);
        double bottomTipToUpperRectTranslateY__61010 = ((-_preferredHalfHeight / 2L) - upperRect__59922.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY__61010);
        var boxCenter__61156 = new global::Doroti.Ui.Offset(horizontalShift__59681, (upperRect__59922.height / 1.75));
        var halfLabelPainterOffset__61228 = new global::Doroti.Ui.Offset((((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset__61327 = ((global::Doroti.Ui.Offset)(object?)(boxCenter__61156 - halfLabelPainterOffset__61228));
        labelPainter.paint(canvas, labelOffset__61327);
        canvas.restore();
    }

}
