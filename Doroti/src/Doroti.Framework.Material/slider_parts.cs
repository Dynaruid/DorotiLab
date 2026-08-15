// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/slider_parts.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public interface SliderTickMarkShape
{
    public static SliderTickMarkShape noTickMark = ((SliderTickMarkShape)(object?)new _EmptySliderTickMarkShape__slider_parts());

    public global::Doroti.Ui.Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled);
    public void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection);
}

public abstract class SliderTrackShape
{
    protected SliderTrackShape()
    {
    }

    public abstract global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = default!, bool isDiscrete = default!);
    public abstract void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = default!, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2);
    public virtual bool isRounded => false;
}

public interface BaseSliderTrackShape
{
    public global::Doroti.Ui.Rect getPreferredRect(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false);
    public bool isRounded { get; }
}

public class RectangularSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public RectangularSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
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
        var activeTrackColorTween__12885 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__13030 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__13181 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__12885.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var inactivePaint__13272 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__13030.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var (leftTrackPaint__13374, rightTrackPaint__13396) = (textDirection switch { TextDirection.ltr => (((Paint, Paint))((activePaint__13181, inactivePaint__13272))), TextDirection.rtl => (((Paint, Paint))((inactivePaint__13272, activePaint__13181))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect__13577 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var leftTrackSegment__13765 = global::Doroti.Ui.Rect.fromLTRB(trackRect__13577.left, trackRect__13577.top, thumbCenter.dx, trackRect__13577.bottom);
        if (!leftTrackSegment__13765.isEmpty)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRect(leftTrackSegment__13765, leftTrackPaint__13374);
        }
        var rightTrackSegment__14013 = global::Doroti.Ui.Rect.fromLTRB(thumbCenter.dx, trackRect__13577.top, trackRect__13577.right, trackRect__13577.bottom);
        if (!rightTrackSegment__14013.isEmpty)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRect(rightTrackSegment__14013, rightTrackPaint__13396);
        }
        bool showSecondaryTrack__14272 = ((secondaryOffset is not null) && (textDirection switch { TextDirection.rtl => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx < thumbCenter.dx), TextDirection.ltr => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx > thumbCenter.dx), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        if (showSecondaryTrack__14272)
        {
            var secondaryTrackColorTween__14551 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint__14725 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = secondaryTrackColorTween__14551.evaluate(enableAnimation)!;
            return __cascade;        }))();
            global::Doroti.Ui.Rect secondaryTrackSegment__14843 = ((global::Doroti.Ui.Rect)(object?)(textDirection switch { TextDirection.rtl => global::Doroti.Ui.Rect.fromLTRB(DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect__13577.top, thumbCenter.dx, trackRect__13577.bottom), TextDirection.ltr => global::Doroti.Ui.Rect.fromLTRB(thumbCenter.dx, trackRect__13577.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect__13577.bottom), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            if (!secondaryTrackSegment__14843.isEmpty)
            {
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRect(secondaryTrackSegment__14843, secondaryTrackPaint__14725);
            }
        }
    }

    public override Rect getPreferredRect(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth__8890 = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth__8991 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__9108 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__8991 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__9108 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__9108 = 0;
        }
        double trackLeft__9504 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__8991 / 2L), (thumbWidth__8890 / 2L)) : 0L)));
        double trackTop__9633 = (offset.dy + (((((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__9108)) / 2L));
        double trackRight__9716 = ((trackLeft__9504 + ((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth__8890, overlayWidth__8991) : 0L)));
        double trackBottom__9877 = (trackTop__9633 + trackHeight__9108);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__9504, trackRight__9716), trackTop__9633, Math.Max(trackLeft__9504, trackRight__9716), trackBottom__9877);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRounded => false;
}

public class RoundedRectSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public RoundedRectSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
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
        var activeTrackColorTween__18030 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__18175 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__18326 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__18030.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var inactivePaint__18417 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__18175.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var (leftTrackPaint__18519, rightTrackPaint__18541) = (textDirection switch { TextDirection.ltr => (((Paint, Paint))((activePaint__18326, inactivePaint__18417))), TextDirection.rtl => (((Paint, Paint))((inactivePaint__18417, activePaint__18326))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        global::Doroti.Ui.Rect trackRect__18722 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackRadius__18909 = global::Doroti.Ui.Radius.circular((trackRect__18722.height / 2L));
        var activeTrackRadius__18972 = global::Doroti.Ui.Radius.circular((((trackRect__18722.height + additionalActiveTrackHeight)) / 2L));
        var isLTR__19073 = (object.Equals(textDirection, TextDirection.ltr));
        var isRTL__19127 = (object.Equals(textDirection, TextDirection.rtl));
        bool drawInactiveTrack__19187 = (__thumbCenter.dx < ((trackRect__18722.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawInactiveTrack__19187)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR((__thumbCenter.dx - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (isRTL__19127 ? (trackRect__18722.top - ((additionalActiveTrackHeight / 2L))) : trackRect__18722.top), trackRect__18722.right, (isRTL__19127 ? (trackRect__18722.bottom + ((additionalActiveTrackHeight / 2L))) : trackRect__18722.bottom), (isLTR__19073 ? trackRadius__18909 : activeTrackRadius__18972)), rightTrackPaint__18541);
        }
        bool drawActiveTrack__19791 = (__thumbCenter.dx > ((trackRect__18722.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawActiveTrack__19791)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBR(trackRect__18722.left, (isLTR__19073 ? (trackRect__18722.top - ((additionalActiveTrackHeight / 2L))) : trackRect__18722.top), (__thumbCenter.dx + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L))), (isLTR__19073 ? (trackRect__18722.bottom + ((additionalActiveTrackHeight / 2L))) : trackRect__18722.bottom), (isLTR__19073 ? activeTrackRadius__18972 : trackRadius__18909)), leftTrackPaint__18519);
        }
        bool showSecondaryTrack__20379 = (((secondaryOffset is not null)) && ((isLTR__19073 ? ((DartRuntimePrimitives.RequireValue(secondaryOffset).dx > __thumbCenter.dx)) : ((DartRuntimePrimitives.RequireValue(secondaryOffset).dx < __thumbCenter.dx)))));
        if (showSecondaryTrack__20379)
        {
            var secondaryTrackColorTween__20577 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint__20751 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = secondaryTrackColorTween__20577.evaluate(enableAnimation)!;
            return __cascade;        }))();
            if (isLTR__19073)
            {
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(__thumbCenter.dx, trackRect__18722.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect__18722.bottom, topRight: trackRadius__18909, bottomRight: trackRadius__18909), secondaryTrackPaint__20751);
            }
            else
            {
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners(DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect__18722.top, __thumbCenter.dx, trackRect__18722.bottom, topLeft: trackRadius__18909, bottomLeft: trackRadius__18909), secondaryTrackPaint__20751);
            }
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth__8890 = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth__8991 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__9108 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__8991 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__9108 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__9108 = 0;
        }
        double trackLeft__9504 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__8991 / 2L), (thumbWidth__8890 / 2L)) : 0L)));
        double trackTop__9633 = (offset.dy + (((((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__9108)) / 2L));
        double trackRight__9716 = ((trackLeft__9504 + ((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth__8890, overlayWidth__8991) : 0L)));
        double trackBottom__9877 = (trackTop__9633 + trackHeight__9108);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__9504, trackRight__9716), trackTop__9633, Math.Max(trackLeft__9504, trackRight__9716), trackBottom__9877);
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
        return new global::Doroti.Ui.Size((this.tickMarkRadius ?? (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 4L)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledActiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledInactiveTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.activeTickMarkColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.inactiveTickMarkColor is not null));
        double xOffset__23981 = (center.dx - thumbCenter.dx);
        var (begin__24037, end__24051) = (textDirection switch { TextDirection.ltr when ((xOffset__23981 > 0L)) => (((Color?, Color?))((sliderTheme.disabledInactiveTickMarkColor, sliderTheme.inactiveTickMarkColor))), TextDirection.rtl when ((xOffset__23981 < 0L)) => (((Color?, Color?))((sliderTheme.disabledInactiveTickMarkColor, sliderTheme.inactiveTickMarkColor))), TextDirection.ltr => (((Color?, Color?))((sliderTheme.disabledActiveTickMarkColor, sliderTheme.activeTickMarkColor))), TextDirection.rtl => (((Color?, Color?))((sliderTheme.disabledActiveTickMarkColor, sliderTheme.activeTickMarkColor))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var paint__24547 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: begin__24037, end: end__24051).evaluate(enableAnimation)!;
            return __cascade;        }))();
        double tickMarkRadius__24731 = (getPreferredSize(isEnabled: isEnabled, sliderTheme: sliderTheme).width / 2L);
        if ((DartRuntimePrimitives.RequireValue(tickMarkRadius__24731) > 0L))
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawCircle(center, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(tickMarkRadius__24731)), paint__24547);
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

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, bool isEnabled, TextDirection textDirection)
    {
    }

}

public class RoundSliderThumbShape : SliderComponentShape
{
    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);

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
        return new global::Doroti.Ui.Size((isEnabled ? this.enabledThumbRadius : this._disabledThumbRadius));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledThumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbColor is not null));
        global::Doroti.Ui.Canvas canvas__28420 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas);
        var radiusTween__28455 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: this._disabledThumbRadius, end: this.enabledThumbRadius);
        var colorTween__28548 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.thumbColor);
        global::Doroti.Ui.Color color__28677 = ((global::Doroti.Ui.Color)(object?)colorTween__28548.evaluate(enableAnimation)!);
        double radius__28741 = radiusTween__28455.evaluate(enableAnimation);
        var elevationTween__28800 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: this.elevation, end: this.pressedElevation);
        double evaluatedElevation__28891 = elevationTween__28800.evaluate(activationAnimation);
        var path__28968 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.addArc(global::Doroti.Ui.Rect.fromCenter(center: center, width: (2L * radius__28741), height: (2L * radius__28741)), 0, (Dart_mathLibrary.pi * 2L));
            return __cascade;        }))();
        var paintShadows__29128 = true;
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Painting.DebugLibrary.debugDisableShadows)
                {
                    Slider_partsLibrary._debugDrawShadow(canvas__28420, path__28968, evaluatedElevation__28891);
                    paintShadows__29128 = false;
                }
                return true;
            });
        if (paintShadows__29128)
        {
            canvas__28420.drawShadow(path__28968, Colors.black, evaluatedElevation__28891, true);
        }
        canvas__28420.drawCircle(center, radius__28741, ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = color__28677;
            return __cascade;        }))());
    }

}

public class DropSliderValueIndicatorShape : SliderComponentShape
{
    internal static _DropSliderValueIndicatorPathPainter__slider_parts _pathPainter = new _DropSliderValueIndicatorPathPainter__slider_parts();

    public DropSliderValueIndicatorShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvas__30881 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas);
        double scale__30923 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvas__30881, center: center, scale: scale__30923, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
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

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        double width__31944 = (Math.Max(_minLabelWidth, ((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L) * textScaleFactor));
        return new global::Doroti.Ui.Size(width__31944, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding__32397 = 8.0;
        double rectangleWidth__32433 = _upperRectangleWidth(labelPainter, scale);
        global::Doroti.Ui.Offset globalCenter__32670 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft__33038 = Math.Max(0, (((rectangleWidth__32433 / 2L) - globalCenter__32670.dx) + edgePadding__32397));
        double overflowRight__33135 = Math.Max(0, ((rectangleWidth__32433 / 2L) - (((sizeWithOverflow.width - globalCenter__32670.dx) - edgePadding__32397))));
        if ((rectangleWidth__32433 < sizeWithOverflow.width))
        {
            return (overflowLeft__33038 - overflowRight__33135);
        }
        else
        {
            if (((overflowLeft__33038 - overflowRight__33135) > 0L))
            {
                return (overflowLeft__33038 - ((edgePadding__32397 * textScaleFactor)));
            }
            else
            {
                return (-overflowRight__33135 + ((edgePadding__32397 * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth__33645 = (Math.Max(_minLabelWidth, ((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width) + _labelPadding);
        return (unscaledWidth__33645 * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderRadius _adjustBorderRadius(Rect rect)
    {
        var rectness__33820 = 0.0;
        return BorderRadius.lerp(global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(_upperRectRadius)), global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular((rect.shortestSide / 2.0))), (1.0 - rectness__33820))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth__34544 = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift__34621 = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect__34862 = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth__34544 / 2L) + horizontalShift__34621), (-_rectYOffset - _minRectHeight), rectangleWidth__34544, _minRectHeight);
        var fillPaint__35034 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = backgroundPaintColor;
            return __cascade;        }))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        global::Doroti.Generated.Framework.Painting.BorderRadius adjustedBorderRadius__35225 = ((global::Doroti.Generated.Framework.Painting.BorderRadius)(object?)_adjustBorderRadius(upperRect__34862));
        global::Doroti.Ui.RRect borderRect__35296 = ((global::Doroti.Ui.RRect)(object?)adjustedBorderRadius__35225.resolve(((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).textDirection).toRRect(upperRect__34862));
        var trianglePath__35414 = ((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Ui.Path();
            __cascade.lineTo(-_triangleHeight, -_triangleHeight);
            __cascade.lineTo(_triangleHeight, -_triangleHeight);
            __cascade.close();
            return __cascade;        }))();
        trianglePath__35414.addRRect(borderRect__35296);
        if ((strokePaintColor is not null))
        {
            var strokePaint__35642 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = strokePaintColor;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            canvas.drawPath(trianglePath__35414, strokePaint__35642);
        }
        canvas.drawPath(trianglePath__35414, fillPaint__35034);
        double bottomTipToUpperRectTranslateY__35950 = ((-_preferredHalfHeight / 2L) - upperRect__34862.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY__35950);
        var boxCenter__36096 = new global::Doroti.Ui.Offset(horizontalShift__34621, (upperRect__34862.height / 1.75));
        var halfLabelPainterOffset__36168 = new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset__36267 = ((global::Doroti.Ui.Offset)(object?)(boxCenter__36096 - halfLabelPainterOffset__36168));
        labelPainter.paint(canvas, labelOffset__36267);
        canvas.restore();
    }

}

public class HandleThumbShape : SliderComponentShape
{
    public global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null) => getPreferredSize(isEnabled, isDiscrete);

    public HandleThumbShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return new global::Doroti.Ui.Size(4.0, 44.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        DartRuntimePrimitives.Assert(() => (sliderTheme.disabledThumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbColor is not null));
        DartRuntimePrimitives.Assert(() => (sliderTheme.thumbSize is not null));
        var colorTween__38055 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledThumbColor, end: sliderTheme.thumbColor);
        global::Doroti.Ui.Color color__38183 = ((global::Doroti.Ui.Color)(object?)colorTween__38055.evaluate(enableAnimation)!);
        global::Doroti.Ui.Canvas canvas__38248 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas);
        global::Doroti.Ui.Size thumbSize__38288 = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.RequireValue(sliderTheme.thumbSize!.resolve(new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>())));
        var rrect__38414 = global::Doroti.Ui.RRect.fromRectAndRadius(global::Doroti.Ui.Rect.fromCenter(center: center, width: thumbSize__38288.width, height: thumbSize__38288.height), global::Doroti.Ui.Radius.circular((thumbSize__38288.shortestSide / 2L)));
        canvas__38248.drawRRect(rrect__38414, ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = color__38183;
            return __cascade;        }))());
    }

}

public class GappedSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{

    public GappedSliderTrackShape()
    {
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, Offset thumbCenter, Offset? secondaryOffset = null, bool isEnabled = default!, bool isDiscrete = false, TextDirection textDirection = default!, double additionalActiveTrackHeight = 2)
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
        var activeTrackColorTween__41193 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledActiveTrackColor, end: sliderTheme.activeTrackColor);
        var inactiveTrackColorTween__41338 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledInactiveTrackColor, end: sliderTheme.inactiveTrackColor);
        var activePaint__41489 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = activeTrackColorTween__41193.evaluate(enableAnimation)!;
            return __cascade;        }))();
        var inactivePaint__41580 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = inactiveTrackColorTween__41338.evaluate(enableAnimation)!;
            return __cascade;        }))();
        global::Doroti.Ui.Paint leftTrackPaint__41681 = default!;
        global::Doroti.Ui.Paint rightTrackPaint__41713 = default!;
        switch (textDirection)
        {
            case TextDirection.ltr:
                {
                    leftTrackPaint__41681 = activePaint__41489;
                    rightTrackPaint__41713 = inactivePaint__41580;
                    break;
                }
            case TextDirection.rtl:
                {
                    leftTrackPaint__41681 = inactivePaint__41580;
                    rightTrackPaint__41713 = activePaint__41489;
                    break;
                }
        }
        double trackGap__42052 = DartRuntimePrimitives.RequireValue(sliderTheme.trackGap);
        global::Doroti.Ui.Rect trackRect__42102 = ((global::Doroti.Ui.Rect)(object?)getPreferredRect(parentBox: parentBox, offset: offset, sliderTheme: sliderTheme, isEnabled: isEnabled, isDiscrete: isDiscrete));
        var trackCornerRadius__42290 = global::Doroti.Ui.Radius.circular((trackRect__42102.shortestSide / 2L));
        var trackInsideCornerRadius__42365 = global::Doroti.Ui.Radius.circular(2.0);
        var trackRRect__42424 = global::Doroti.Ui.RRect.fromRectAndCorners(trackRect__42102, topLeft: trackCornerRadius__42290, bottomLeft: trackCornerRadius__42290, topRight: trackCornerRadius__42290, bottomRight: trackCornerRadius__42290);
        var leftRRect__42642 = global::Doroti.Ui.RRect.fromLTRBAndCorners(trackRect__42102.left, trackRect__42102.top, Math.Max(trackRect__42102.left, (__thumbCenter.dx - trackGap__42052)), trackRect__42102.bottom, topLeft: trackCornerRadius__42290, bottomLeft: trackCornerRadius__42290, topRight: trackInsideCornerRadius__42365, bottomRight: trackInsideCornerRadius__42365);
        var rightRRect__42980 = global::Doroti.Ui.RRect.fromLTRBAndCorners((__thumbCenter.dx + trackGap__42052), trackRect__42102.top, trackRect__42102.right, trackRect__42102.bottom, topRight: trackCornerRadius__42290, bottomRight: trackCornerRadius__42290, topLeft: trackInsideCornerRadius__42365, bottomLeft: trackInsideCornerRadius__42365);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas;
            __cascade.save();
            __cascade.clipRRect(trackRRect__42424);
            return __cascade;        }))());
        bool drawLeftTrack__43364 = (__thumbCenter.dx > ((leftRRect__42642.left + ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        bool drawRightTrack__43463 = (__thumbCenter.dx < ((rightRRect__42980.right - ((DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L)))));
        if (drawLeftTrack__43364)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(leftRRect__42642, leftTrackPaint__41681);
        }
        if (drawRightTrack__43463)
        {
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(rightRRect__42980, rightTrackPaint__41713);
        }
        var isLTR__43752 = (object.Equals(textDirection, TextDirection.ltr));
        bool showSecondaryTrack__43811 = (((secondaryOffset is not null)) && (((object)isLTR__43752) switch { true => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx > (__thumbCenter.dx + trackGap__42052)), false => (DartRuntimePrimitives.RequireValue(secondaryOffset).dx < (__thumbCenter.dx - trackGap__42052)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        if (showSecondaryTrack__43811)
        {
            var secondaryTrackColorTween__44081 = new global::Doroti.Generated.Framework.Animation.ColorTween(begin: sliderTheme.disabledSecondaryActiveTrackColor, end: sliderTheme.secondaryActiveTrackColor);
            var secondaryTrackPaint__44255 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = secondaryTrackColorTween__44081.evaluate(enableAnimation)!;
            return __cascade;        }))();
            if (isLTR__43752)
            {
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners((__thumbCenter.dx + trackGap__42052), trackRect__42102.top, DartRuntimePrimitives.RequireValue(secondaryOffset).dx, trackRect__42102.bottom, topLeft: trackInsideCornerRadius__42365, bottomLeft: trackInsideCornerRadius__42365, topRight: trackCornerRadius__42290, bottomRight: trackCornerRadius__42290), secondaryTrackPaint__44255);
            }
            else
            {
                ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawRRect(global::Doroti.Ui.RRect.fromLTRBAndCorners((DartRuntimePrimitives.RequireValue(secondaryOffset).dx - trackGap__42052), trackRect__42102.top, __thumbCenter.dx, trackRect__42102.bottom, topLeft: trackInsideCornerRadius__42365, bottomLeft: trackInsideCornerRadius__42365, topRight: trackCornerRadius__42290, bottomRight: trackCornerRadius__42290), secondaryTrackPaint__44255);
            }
        }
        ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.restore();
        var stopIndicatorRadius__45311 = 2.0;
        double stopIndicatorTrailingSpace__45355 = (DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight) / 2L);
        var stopIndicatorOffset__45424 = new global::Doroti.Ui.Offset((((object.Equals(textDirection, TextDirection.ltr))) ? (trackRect__42102.centerRight.dx - stopIndicatorTrailingSpace__45355) : (trackRect__42102.centerLeft.dx + stopIndicatorTrailingSpace__45355)), ((Offset)((dynamic)trackRect__42102).center).dy);
        bool showStopIndicator__45679 = (((object.Equals(textDirection, TextDirection.ltr))) ? (__thumbCenter.dx < stopIndicatorOffset__45424.dx) : (__thumbCenter.dx > stopIndicatorOffset__45424.dx));
        if ((showStopIndicator__45679 && !isDiscrete))
        {
            var stopIndicatorRect__45893 = global::Doroti.Ui.Rect.fromCircle(center: stopIndicatorOffset__45424, radius: stopIndicatorRadius__45311);
            ((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas.drawCircle(((Offset)((dynamic)stopIndicatorRect__45893).center), stopIndicatorRadius__45311, activePaint__41489);
        }
    }

    public override bool isRounded => true;
    public override Rect getPreferredRect(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset offset = default, SliderThemeData sliderTheme = default!, bool isEnabled = false, bool isDiscrete = false)
    {
        double thumbWidth__8890 = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth__8991 = sliderTheme.overlayShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double trackHeight__9108 = DartRuntimePrimitives.RequireValue(sliderTheme.trackHeight);
        DartRuntimePrimitives.Assert(() => (overlayWidth__8991 >= 0L));
        DartRuntimePrimitives.Assert(() => (trackHeight__9108 >= 0L));
        if (((object.Equals(sliderTheme.activeTrackColor, Colors.transparent)) && (object.Equals(sliderTheme.inactiveTrackColor, Colors.transparent))))
        {
            trackHeight__9108 = 0;
        }
        double trackLeft__9504 = (offset.dx + (((sliderTheme.padding is null) ? Math.Max((overlayWidth__8991 / 2L), (thumbWidth__8890 / 2L)) : 0L)));
        double trackTop__9633 = (offset.dy + (((((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.height - trackHeight__9108)) / 2L));
        double trackRight__9716 = ((trackLeft__9504 + ((global::Doroti.Generated.Framework.Rendering.RenderBox)parentBox).size.width) - (((sliderTheme.padding is null) ? Math.Max(thumbWidth__8890, overlayWidth__8991) : 0L)));
        double trackBottom__9877 = (trackTop__9633 + trackHeight__9108);
        return global::Doroti.Ui.Rect.fromLTRB(Math.Min(trackLeft__9504, trackRight__9716), trackTop__9633, Math.Max(trackLeft__9504, trackRight__9716), trackBottom__9877);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RoundedRectSliderValueIndicatorShape : SliderComponentShape
{
    internal static _RoundedRectSliderValueIndicatorPathPainter__slider_parts _pathPainter = new _RoundedRectSliderValueIndicatorPathPainter__slider_parts();

    public RoundedRectSliderValueIndicatorShape()
    {
    }

    public virtual global::Doroti.Ui.Size getPreferredSize(bool isEnabled, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter? labelPainter = null, double? textScaleFactor = null)
    {
        DartRuntimePrimitives.Assert(() => (labelPainter is not null));
        DartRuntimePrimitives.Assert(() => ((textScaleFactor is not null) && (textScaleFactor >= 0L)));
        return ((global::Doroti.Ui.Size)(object?)_pathPainter.getPreferredSize(labelPainter!, DartRuntimePrimitives.RequireValue(textScaleFactor)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset center, global::Doroti.Generated.Framework.Animation.Animation<double> activationAnimation, global::Doroti.Generated.Framework.Animation.Animation<double> enableAnimation, bool isDiscrete, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, SliderThemeData sliderTheme, TextDirection textDirection, double value, double textScaleFactor, Size sizeWithOverflow)
    {
        global::Doroti.Ui.Canvas canvas__48454 = ((global::Doroti.Ui.Canvas)(object?)((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas);
        double scale__48496 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)activationAnimation).value;
        _pathPainter.paint(parentBox: parentBox, canvas: canvas__48454, center: center, scale: scale__48496, labelPainter: labelPainter, textScaleFactor: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(textScaleFactor)), sizeWithOverflow: sizeWithOverflow, backgroundPaintColor: sliderTheme.valueIndicatorColor!, strokePaintColor: sliderTheme.valueIndicatorStrokeColor);
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

    public virtual global::Doroti.Ui.Size getPreferredSize(global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor)
    {
        double width__49398 = (Math.Max(_minLabelWidth, ((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width) + (((_labelPadding * 2L)) * textScaleFactor));
        return new global::Doroti.Ui.Size(width__49398, (_preferredHeight * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Offset center, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, double scale)
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding__49853 = 8.0;
        double rectangleWidth__49889 = _upperRectangleWidth(labelPainter, scale);
        global::Doroti.Ui.Offset globalCenter__50126 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)parentBox).localToGlobal(center)));
        double overflowLeft__50494 = Math.Max(0, (((rectangleWidth__49889 / 2L) - globalCenter__50126.dx) + edgePadding__49853));
        double overflowRight__50591 = Math.Max(0, ((rectangleWidth__49889 / 2L) - (((sizeWithOverflow.width - globalCenter__50126.dx) - edgePadding__49853))));
        if ((rectangleWidth__49889 < sizeWithOverflow.width))
        {
            return (overflowLeft__50494 - overflowRight__50591);
        }
        else
        {
            if (((overflowLeft__50494 - overflowRight__50591) > 0L))
            {
                return (overflowLeft__50494 - ((edgePadding__49853 * textScaleFactor)));
            }
            else
            {
                return (-overflowRight__50591 + ((edgePadding__49853 * textScaleFactor)));
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double scale)
    {
        double unscaledWidth__51101 = (Math.Max(_minLabelWidth, ((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width) + ((_labelPadding * 2L)));
        return (unscaledWidth__51101 * scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(global::Doroti.Generated.Framework.Rendering.RenderBox parentBox, Canvas canvas, Offset center, double scale, global::Doroti.Generated.Framework.Painting.TextPainter labelPainter, double textScaleFactor, Size sizeWithOverflow, Color backgroundPaintColor, Color? strokePaintColor = null)
    {
        if ((scale == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth__51737 = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift__51814 = getHorizontalShift(parentBox: parentBox, center: center, labelPainter: labelPainter, textScaleFactor: textScaleFactor, sizeWithOverflow: sizeWithOverflow, scale: scale);
        var upperRect__52056 = global::Doroti.Ui.Rect.fromLTWH(((-rectangleWidth__51737 / 2L) + horizontalShift__51814), (-_rectYOffset - _preferredHeight), rectangleWidth__51737, _preferredHeight);
        var fillPaint__52232 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = backgroundPaintColor;
            return __cascade;        }))();
        canvas.save();
        canvas.translate(center.dx, (center.dy - _bottomTipYOffset));
        canvas.scale(scale, scale);
        var rrect__52518 = global::Doroti.Ui.RRect.fromRectAndRadius(upperRect__52056, global::Doroti.Ui.Radius.circular((upperRect__52056.height / 2L)));
        if ((strokePaintColor is not null))
        {
            var strokePaint__52649 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = strokePaintColor;
            __cascade.strokeWidth = 1.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
            canvas.drawRRect(rrect__52518, strokePaint__52649);
        }
        canvas.drawRRect(rrect__52518, fillPaint__52232);
        double bottomTipToUpperRectTranslateY__52945 = ((-_preferredHalfHeight / 2L) - upperRect__52056.height);
        canvas.translate(0, bottomTipToUpperRectTranslateY__52945);
        var boxCenter__53091 = new global::Doroti.Ui.Offset(horizontalShift__51814, (upperRect__52056.height / 2.3));
        var halfLabelPainterOffset__53162 = new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).width / 2L), (((global::Doroti.Generated.Framework.Painting.TextPainter)labelPainter).height / 2L));
        global::Doroti.Ui.Offset labelOffset__53261 = ((global::Doroti.Ui.Offset)(object?)(boxCenter__53091 - halfLabelPainterOffset__53162));
        labelPainter.paint(canvas, labelOffset__53261);
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
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = Colors.black;
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = (elevation * 2.0);
            return __cascade;        }))());
        }
    }
}
