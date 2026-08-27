// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider_parts.dart
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

public interface SliderTickMarkShape
{
    public static SliderTickMarkShape noTickMark = ((SliderTickMarkShape)(object?)new _EmptySliderTickMarkShape__slider_parts());

    public global::Doroti.Ui.Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled);
    public void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection);
}

public abstract class SliderTrackShape
{
    protected SliderTrackShape()
    {
    }

    public abstract global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = default!, bool isDiscrete = default!);
    public abstract void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = default!, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2);
    public virtual bool isRounded => false;
}

public interface BaseSliderTrackShape
{
    public global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false);
    public bool isRounded { get; }
}

public class RectangularSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public RectangularSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbShape is not null));
        if ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) <= 0L))
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
        var (leftTrackPaint, rightTrackPaint) = (textDirection switch { TextDirection.ltr => (((Paint, Paint))((activePaint, inactivePaint))), TextDirection.rtl => (((Paint, Paint))((inactivePaint, activePaint))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var leftTrackSegment = global::Doroti.Ui.Rect.fromLTRB(trackRect.left, trackRect.top, thumbCenter.dx, trackRect.bottom);
        if (!leftTrackSegment.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(leftTrackSegment, leftTrackPaint);
        }
        var rightTrackSegment = global::Doroti.Ui.Rect.fromLTRB(thumbCenter.dx, trackRect.top, trackRect.right, trackRect.bottom);
        if (!rightTrackSegment.isEmpty)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(rightTrackSegment, rightTrackPaint);
        }
        bool showSecondaryTrack = ((secondaryOffset is not null) && (textDirection switch { TextDirection.rtl => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx < thumbCenter.dx), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx > thumbCenter.dx), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
            global::Doroti.Ui.Rect secondaryTrackSegment = ((global::Doroti.Ui.Rect)(object?)(textDirection switch { TextDirection.rtl => global::Doroti.Ui.Rect.fromLTRB(DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect.top, thumbCenter.dx, trackRect.bottom), TextDirection.ltr => global::Doroti.Ui.Rect.fromLTRB(thumbCenter.dx, trackRect.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect.bottom), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            if (!secondaryTrackSegment.isEmpty)
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(secondaryTrackSegment, secondaryTrackPaint);
            }
        }
    }

    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbWidth / 2L)) : 0L)));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRounded => false;
}

public class RoundedRectSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public RoundedRectSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        Offset __thumbCenter = thumbCenter;
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbShape is not null));
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
        var (leftTrackPaint, rightTrackPaint) = (textDirection switch { TextDirection.ltr => (((Paint, Paint))((activePaint, inactivePaint))), TextDirection.rtl => (((Paint, Paint))((inactivePaint, activePaint))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackRadius = global::Doroti.Ui.Radius.circular((trackRect.height / 2L));
        var activeTrackRadius = global::Doroti.Ui.Radius.circular((((trackRect.height + additionalActiveTrackHeight)) / 2L));
        var isLTR = (object.Equals(textDirection, TextDirection.ltr));
        var isRTL = (object.Equals(textDirection, TextDirection.rtl));
        bool drawInactiveTrack = (__thumbCenter.dx < ((trackRect.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawInactiveTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((__thumbCenter.dx - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (isRTL ? (trackRect.top - ((additionalActiveTrackHeight / 2L))) : trackRect.top), trackRect.right, (isRTL ? (trackRect.bottom + ((additionalActiveTrackHeight / 2L))) : trackRect.bottom), (isLTR ? trackRadius : activeTrackRadius)), rightTrackPaint);
        }
        bool drawActiveTrack = (__thumbCenter.dx > ((trackRect.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawActiveTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR(trackRect.left, (isLTR ? (trackRect.top - ((additionalActiveTrackHeight / 2L))) : trackRect.top), (__thumbCenter.dx + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (isLTR ? (trackRect.bottom + ((additionalActiveTrackHeight / 2L))) : trackRect.bottom), (isLTR ? activeTrackRadius : trackRadius)), leftTrackPaint);
        }
        bool showSecondaryTrack = (((secondaryOffset is not null)) && ((isLTR ? ((DartRuntimePrimitives.RequireValue(secondaryOffset).dx > __thumbCenter.dx)) : ((DartRuntimePrimitives.RequireValue(secondaryOffset).dx < __thumbCenter.dx)))));
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
            if (isLTR)
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(__thumbCenter.dx, trackRect.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect.bottom, topRight: trackRadius, bottomRight: trackRadius), secondaryTrackPaint);
            }
            else
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect.top, __thumbCenter.dx, trackRect.bottom, topLeft: trackRadius, bottomLeft: trackRadius), secondaryTrackPaint);
            }
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbWidth / 2L)) : 0L)));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundSliderTickMarkShape : SliderTickMarkShape
{
    public virtual double? tickMarkRadius { get; private set; }

    public RoundSliderTickMarkShape(double? tickMarkRadius = null)
    {
        this.tickMarkRadius = tickMarkRadius;
    }

    public virtual Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackHeight is not null));
        return global::Doroti.Ui.Size.fromRadius((this.tickMarkRadius ?? (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 4L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTickMarkColor is not null));
        double xOffset = (center.dx - thumbCenter.dx);
        var (beginLocal, endLocal) = (textDirection switch { TextDirection.ltr when ((xOffset > 0L)) => (((Color?, Color?))((sliderTheme.disabledInactiveTickMarkColor, sliderTheme.inactiveTickMarkColor))), TextDirection.rtl when ((xOffset < 0L)) => (((Color?, Color?))((sliderTheme.disabledInactiveTickMarkColor, sliderTheme.inactiveTickMarkColor))), TextDirection.ltr => (((Color?, Color?))((sliderTheme.disabledActiveTickMarkColor, sliderTheme.activeTickMarkColor))), TextDirection.rtl => (((Color?, Color?))((sliderTheme.disabledActiveTickMarkColor, sliderTheme.activeTickMarkColor))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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

internal class _EmptySliderTickMarkShape__slider_parts : SliderTickMarkShape
{
    public virtual Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled)
    {
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection)
    {
    }

}

public class RoundSliderThumbShape : SliderComponentShape
{
    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);

    public virtual double enabledThumbRadius { get; private set; } = default!;
    public virtual double? disabledThumbRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual double pressedElevation { get; private set; } = default!;

    public RoundSliderThumbShape(double enabledThumbRadius = 10.0, double? disabledThumbRadius = null, double elevation = 1.0, double pressedElevation = 6.0)
    {
        this.enabledThumbRadius = enabledThumbRadius;
        this.disabledThumbRadius = disabledThumbRadius;
        this.elevation = elevation;
        this.pressedElevation = pressedElevation;
    }

    internal virtual double _disabledThumbRadius => DartRuntimePrimitives.ConvertValue<double>((this.disabledThumbRadius ?? this.enabledThumbRadius));
    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return global::Doroti.Ui.Size.fromRadius((isEnabled ? this.enabledThumbRadius : this._disabledThumbRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledThumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbColor is not null));
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: this._disabledThumbRadius, end: this.enabledThumbRadius);
        var colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.thumbColor);
        global::Doroti.Ui.Color colorLocal = ((global::Doroti.Ui.Color)(object?)colorTween.evaluate(enableAnimation)!);
        double radius = radiusTween.evaluate(enableAnimation);
        var elevationTween = new global::Doroti.Framework.Animation.Tween<double>(begin: this.elevation, end: this.pressedElevation);
        double evaluatedElevation = elevationTween.evaluate(activationAnimation);
        var path = ((Func<Path>)(() =>
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
                    Slider_partsLibrary._debugDrawShadow(canvasLocal, path, evaluatedElevation);
                    paintShadows = false;
                }
                return true;
            });
        if (paintShadows)
        {
            canvasLocal.drawShadow(path, Colors.black, evaluatedElevation, true);
        }
        canvasLocal.drawCircle(center, radius, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = colorLocal;
    return __cascade;
}))());
    }

}

public class DropSliderValueIndicatorShape : SliderComponentShape
{
    internal static _DropSliderValueIndicatorPathPainter__slider_parts _pathPainter = new _DropSliderValueIndicatorPathPainter__slider_parts();

    public DropSliderValueIndicatorShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvasLocal, center: center, scale: scaleLocal, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
    }

}

internal class _DropSliderValueIndicatorPathPainter__slider_parts
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

    internal _DropSliderValueIndicatorPathPainter__slider_parts()
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

public class HandleThumbShape : SliderComponentShape
{
    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);

    public HandleThumbShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return new global::Doroti.Ui.Size(4.0, 44.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledThumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbSize is not null));
        var colorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.thumbColor);
        global::Doroti.Ui.Color colorLocal = ((global::Doroti.Ui.Color)(object?)colorTween.evaluate(enableAnimation)!);
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        global::Doroti.Ui.Size thumbSizeLocal = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.RequireValue(sliderTheme.thumbSize!.resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>())));
        var rrect = global::Doroti.Ui.RRect.fromRectAndRadius(global::Doroti.Ui.Rect.fromCenter(center: center, width: thumbSizeLocal.width, height: thumbSizeLocal.height), global::Doroti.Ui.Radius.circular((thumbSizeLocal.shortestSide / 2L)));
        canvasLocal.drawRRect(rrect, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = colorLocal;
    return __cascade;
}))());
    }

}

public class GappedSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public GappedSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
    {
        Offset __thumbCenter = thumbCenter;
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTrackColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbShape is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.trackGap is not null));
        DartRuntimePrimitives.Assert(() => !DartRuntimePrimitives.RequireValue(sliderTheme.trackGap).isNegative());
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
        global::Doroti.Ui.Paint leftTrackPaint = default!;
        global::Doroti.Ui.Paint rightTrackPaint = default!;
        switch (textDirection)
        {
            case TextDirection.ltr:
                {
                    leftTrackPaint = activePaint;
                    rightTrackPaint = inactivePaint;
                    break;
                }
            case TextDirection.rtl:
                {
                    leftTrackPaint = inactivePaint;
                    rightTrackPaint = activePaint;
                    break;
                }
        }
        double trackGapLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackGap);
        global::Doroti.Ui.Rect trackRect = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackCornerRadius = global::Doroti.Ui.Radius.circular((trackRect.shortestSide / 2L));
        var trackInsideCornerRadius = global::Doroti.Ui.Radius.circular(2.0);
        var trackRRect = global::Doroti.Ui.RRect.fromRectAndCorners(trackRect, topLeft: trackCornerRadius, bottomLeft: trackCornerRadius, topRight: trackCornerRadius, bottomRight: trackCornerRadius);
        var leftRRect = global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect.left, trackRect.top, Math.Max(trackRect.left, (__thumbCenter.dx - trackGapLocal)), trackRect.bottom, topLeft: trackCornerRadius, bottomLeft: trackCornerRadius, topRight: trackInsideCornerRadius, bottomRight: trackInsideCornerRadius);
        var rightRRect = global::Doroti.Ui.RRect.fromLTRBAndCorners((__thumbCenter.dx + trackGapLocal), trackRect.top, trackRect.right, trackRect.bottom, topRight: trackCornerRadius, bottomRight: trackCornerRadius, topLeft: trackInsideCornerRadius, bottomLeft: trackInsideCornerRadius);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas;
    __cascade.save();
    __cascade.clipRRect(trackRRect);
    return __cascade;
}))());
        bool drawLeftTrack = (__thumbCenter.dx > ((leftRRect.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        bool drawRightTrack = (__thumbCenter.dx < ((rightRRect.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawLeftTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(leftRRect, leftTrackPaint);
        }
        if (drawRightTrack)
        {
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(rightRRect, rightTrackPaint);
        }
        var isLTR = (object.Equals(textDirection, TextDirection.ltr));
        bool showSecondaryTrack = (((secondaryOffset is not null)) && (((object)isLTR) switch { true => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx > (__thumbCenter.dx + trackGapLocal)), false => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx < (__thumbCenter.dx - trackGapLocal)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new global::Doroti.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
    return __cascade;
}))();
            if (isLTR)
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners((__thumbCenter.dx + trackGapLocal), trackRect.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect.bottom, topLeft: trackInsideCornerRadius, bottomLeft: trackInsideCornerRadius, topRight: trackCornerRadius, bottomRight: trackCornerRadius), secondaryTrackPaint);
            }
            else
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners((DartRuntimePrimitives.RequireValue(secondaryOffset).dx - trackGapLocal), trackRect.top, __thumbCenter.dx, trackRect.bottom, topLeft: trackInsideCornerRadius, bottomLeft: trackInsideCornerRadius, topRight: trackCornerRadius, bottomRight: trackCornerRadius), secondaryTrackPaint);
            }
        }
        ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
        var stopIndicatorRadius = 2.0;
        double stopIndicatorTrailingSpace = (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L);
        var stopIndicatorOffset = new global::Doroti.Ui.Offset((((object.Equals(textDirection, TextDirection.ltr))) ? (trackRect.centerRight.dx - stopIndicatorTrailingSpace) : (trackRect.centerLeft.dx + stopIndicatorTrailingSpace)), ((Offset)((dynamic)trackRect).center).dy);
        bool showStopIndicator = (((object.Equals(textDirection, TextDirection.ltr))) ? (__thumbCenter.dx < stopIndicatorOffset.dx) : (__thumbCenter.dx > stopIndicatorOffset.dx));
        if ((showStopIndicator && !isDiscrete))
        {
            var stopIndicatorRect = global::Doroti.Ui.Rect.fromCircle(center: stopIndicatorOffset, radius: stopIndicatorRadius);
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRect).center), stopIndicatorRadius, activePaint);
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeightLocal = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeightLocal >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeightLocal = 0;
        }
        double trackLeft = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth / 2L), (thumbWidth / 2L)) : 0L)));
        double trackTop = (offset.dy + (((((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.height - trackHeightLocal)) / 2L));
        double trackRight = ((trackLeft + ((global::Doroti.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L)));
        double trackBottom = (trackTop + trackHeightLocal);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft, trackRight), trackTop, Math.Max(trackLeft, trackRight), trackBottom);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundedRectSliderValueIndicatorShape : SliderComponentShape
{
    internal static _RoundedRectSliderValueIndicatorPathPainter__slider_parts _pathPainter = new _RoundedRectSliderValueIndicatorPathPainter__slider_parts();

    public RoundedRectSliderValueIndicatorShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Framework.Painting.TextPainter labelPainter, global::Doroti.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvasLocal = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Framework.Rendering.PaintingContext)context).canvas);
        double scaleLocal = ((global::Doroti.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvasLocal, center: center, scale: scaleLocal, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
    }

}

internal class _RoundedRectSliderValueIndicatorPathPainter__slider_parts
{
    internal const double _labelPadding = 10.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _rectYOffset = 10.0;
    internal const double _bottomTipYOffset = 16.0;
    internal static double _preferredHalfHeight = (_preferredHeight / 2L);

    internal _RoundedRectSliderValueIndicatorPathPainter__slider_parts()
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

public static partial class Slider_partsLibrary
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
