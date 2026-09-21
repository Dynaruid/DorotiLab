// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider_parts.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public interface SliderTickMarkShape
{
    public static SliderTickMarkShape noTickMark = new _EmptySliderTickMarkShape__slider_parts();

    public Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled);
    public void paint(
        PaintingContext context,
        Offset center,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        bool isEnabled,
        TextDirection textDirection
    );
}

public abstract class SliderTrackShape
{
    protected SliderTrackShape() { }

    public abstract Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = default!,
        bool isDiscrete = default!
    );
    public abstract void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        Offset? secondaryOffset = null,
        bool isEnabled = default!,
        bool isDiscrete = default!,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    );
    public virtual bool isRounded => false;
}

public interface BaseSliderTrackShape
{
    public Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = false,
        bool isDiscrete = false
    );
    public bool isRounded { get; }
}

public class RectangularSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{
    public RectangularSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        Offset? secondaryOffset = null,
        bool isEnabled = default!,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbShape is not null);
        if (
            (
                sliderTheme.trackHeight
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) <= 0L
        )
        {
            return;
        }
        var activeTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledActiveTrackColor,
            end: sliderTheme.activeTrackColor
        );
        var inactiveTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledInactiveTrackColor,
            end: sliderTheme.inactiveTrackColor
        );
        var activePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = activeTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        var inactivePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        var (leftTrackPaint, rightTrackPaint) = textDirection switch
        {
            TextDirection.ltr => ((Paint, Paint))(activePaint, inactivePaint),
            TextDirection.rtl => ((Paint, Paint))(inactivePaint, activePaint),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Rect trackRect = getPreferredRect(
            parentBox: parentBox,
            offset: offset,
            sliderTheme: sliderTheme,
            isEnabled: isEnabled,
            isDiscrete: isDiscrete
        );
        var leftTrackSegment = Rect.fromLTRB(
            trackRect.left,
            trackRect.top,
            thumbCenter.dx,
            trackRect.bottom
        );
        if (!leftTrackSegment.isEmpty)
        {
            context.canvas.drawRect(leftTrackSegment, leftTrackPaint);
        }
        var rightTrackSegment = Rect.fromLTRB(
            thumbCenter.dx,
            trackRect.top,
            trackRect.right,
            trackRect.bottom
        );
        if (!rightTrackSegment.isEmpty)
        {
            context.canvas.drawRect(rightTrackSegment, rightTrackPaint);
        }
        bool showSecondaryTrack =
            (secondaryOffset is not null)
            && (
                textDirection switch
                {
                    TextDirection.rtl => (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx < thumbCenter.dx,
                    TextDirection.ltr => (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx > thumbCenter.dx,
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            );
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new ColorTween(
                begin: sliderTheme.disabledSecondaryActiveTrackColor,
                end: sliderTheme.secondaryActiveTrackColor
            );
            var secondaryTrackPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
                        return __cascade;
                    }
                )
            )();
            Rect secondaryTrackSegment = textDirection switch
            {
                TextDirection.rtl => Rect.fromLTRB(
                    (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    trackRect.top,
                    thumbCenter.dx,
                    trackRect.bottom
                ),
                TextDirection.ltr => Rect.fromLTRB(
                    thumbCenter.dx,
                    trackRect.top,
                    (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx,
                    trackRect.bottom
                ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            if (!secondaryTrackSegment.isEmpty)
            {
                context.canvas.drawRect(secondaryTrackSegment, secondaryTrackPaint);
            }
        }
    }

    public override Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = false,
        bool isDiscrete = false
    )
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme
            .overlayShape!.getPreferredSize(isEnabled, isDiscrete)
            .width;
        double trackHeightLocal = (
            sliderTheme.trackHeight
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        DartRuntimePrimitives.Assert(() => overlayWidth >= 0L);
        DartRuntimePrimitives.Assert(() => trackHeightLocal >= 0L);
        if (
            Equals(sliderTheme.activeTrackColor, Colors.transparent)
            && Equals(sliderTheme.inactiveTrackColor, Colors.transparent)
        )
        {
            trackHeightLocal = 0;
        }
        double trackLeft =
            offset.dx
            + ((sliderTheme.padding is null) ? Math.Max(overlayWidth / 2L, thumbWidth / 2L) : 0L);
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - ((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L);
        double trackBottom = trackTop + trackHeightLocal;
        return Rect.fromLTRB(
            Math.Min(trackLeft, trackRight),
            trackTop,
            Math.Max(trackLeft, trackRight),
            trackBottom
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRounded => false;
}

public class RoundedRectSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{
    public RoundedRectSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        Offset? secondaryOffset = null,
        bool isEnabled = default!,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        Offset __thumbCenter = thumbCenter;
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbShape is not null);
        if (
            (sliderTheme.trackHeight is null)
            || (
                (
                    sliderTheme.trackHeight
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) <= 0L
            )
        )
        {
            return;
        }
        var activeTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledActiveTrackColor,
            end: sliderTheme.activeTrackColor
        );
        var inactiveTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledInactiveTrackColor,
            end: sliderTheme.inactiveTrackColor
        );
        var activePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = activeTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        var inactivePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        var (leftTrackPaint, rightTrackPaint) = textDirection switch
        {
            TextDirection.ltr => ((Paint, Paint))(activePaint, inactivePaint),
            TextDirection.rtl => ((Paint, Paint))(inactivePaint, activePaint),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Rect trackRect = getPreferredRect(
            parentBox: parentBox,
            offset: offset,
            sliderTheme: sliderTheme,
            isEnabled: isEnabled,
            isDiscrete: isDiscrete
        );
        var trackRadius = Radius.circular(trackRect.height / 2L);
        var activeTrackRadius = Radius.circular(
            (trackRect.height + additionalActiveTrackHeight) / 2L
        );
        var isLTR = Equals(textDirection, TextDirection.ltr);
        var isRTL = Equals(textDirection, TextDirection.rtl);
        bool drawInactiveTrack =
            __thumbCenter.dx
            < trackRect.right
                - (
                    (
                        sliderTheme.trackHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2L
                );
        if (drawInactiveTrack)
        {
            context.canvas.drawRRect(
                RRect.fromLTRBR(
                    __thumbCenter.dx
                        - (
                            (
                                sliderTheme.trackHeight
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            ) / 2L
                        ),
                    isRTL ? (trackRect.top - (additionalActiveTrackHeight / 2L)) : trackRect.top,
                    trackRect.right,
                    isRTL
                        ? (trackRect.bottom + (additionalActiveTrackHeight / 2L))
                        : trackRect.bottom,
                    isLTR ? trackRadius : activeTrackRadius
                ),
                rightTrackPaint
            );
        }
        bool drawActiveTrack =
            __thumbCenter.dx
            > trackRect.left
                + (
                    (
                        sliderTheme.trackHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2L
                );
        if (drawActiveTrack)
        {
            context.canvas.drawRRect(
                RRect.fromLTRBR(
                    trackRect.left,
                    isLTR ? (trackRect.top - (additionalActiveTrackHeight / 2L)) : trackRect.top,
                    __thumbCenter.dx
                        + (
                            (
                                sliderTheme.trackHeight
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            ) / 2L
                        ),
                    isLTR
                        ? (trackRect.bottom + (additionalActiveTrackHeight / 2L))
                        : trackRect.bottom,
                    isLTR ? activeTrackRadius : trackRadius
                ),
                leftTrackPaint
            );
        }
        bool showSecondaryTrack =
            secondaryOffset is not null
            && (
                isLTR
                    ? (
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx > __thumbCenter.dx
                    )
                    : (
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx < __thumbCenter.dx
                    )
            );
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new ColorTween(
                begin: sliderTheme.disabledSecondaryActiveTrackColor,
                end: sliderTheme.secondaryActiveTrackColor
            );
            var secondaryTrackPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
                        return __cascade;
                    }
                )
            )();
            if (isLTR)
            {
                context.canvas.drawRRect(
                    RRect.fromLTRBAndCorners(
                        __thumbCenter.dx,
                        trackRect.top,
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx,
                        trackRect.bottom,
                        topRight: trackRadius,
                        bottomRight: trackRadius
                    ),
                    secondaryTrackPaint
                );
            }
            else
            {
                context.canvas.drawRRect(
                    RRect.fromLTRBAndCorners(
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx,
                        trackRect.top,
                        __thumbCenter.dx,
                        trackRect.bottom,
                        topLeft: trackRadius,
                        bottomLeft: trackRadius
                    ),
                    secondaryTrackPaint
                );
            }
        }
    }

    public override bool isRounded => true;

    public override Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = false,
        bool isDiscrete = false
    )
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme
            .overlayShape!.getPreferredSize(isEnabled, isDiscrete)
            .width;
        double trackHeightLocal = (
            sliderTheme.trackHeight
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        DartRuntimePrimitives.Assert(() => overlayWidth >= 0L);
        DartRuntimePrimitives.Assert(() => trackHeightLocal >= 0L);
        if (
            Equals(sliderTheme.activeTrackColor, Colors.transparent)
            && Equals(sliderTheme.inactiveTrackColor, Colors.transparent)
        )
        {
            trackHeightLocal = 0;
        }
        double trackLeft =
            offset.dx
            + ((sliderTheme.padding is null) ? Math.Max(overlayWidth / 2L, thumbWidth / 2L) : 0L);
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - ((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L);
        double trackBottom = trackTop + trackHeightLocal;
        return Rect.fromLTRB(
            Math.Min(trackLeft, trackRight),
            trackTop,
            Math.Max(trackLeft, trackRight),
            trackBottom
        );
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
        DartRuntimePrimitives.Assert(() => sliderTheme.trackHeight is not null);
        return Size.fromRadius(
            tickMarkRadius
                ?? (
                    (
                        sliderTheme.trackHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 4L
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        PaintingContext context,
        Offset center,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        bool isEnabled,
        TextDirection textDirection
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTickMarkColor is not null);
        double xOffset = center.dx - thumbCenter.dx;
        var (beginLocal, endLocal) = textDirection switch
        {
            TextDirection.ltr when xOffset > 0L => (
                sliderTheme.disabledInactiveTickMarkColor,
                sliderTheme.inactiveTickMarkColor
            ),
            TextDirection.rtl when xOffset < 0L => (
                sliderTheme.disabledInactiveTickMarkColor,
                sliderTheme.inactiveTickMarkColor
            ),
            TextDirection.ltr => (
                sliderTheme.disabledActiveTickMarkColor,
                sliderTheme.activeTickMarkColor
            ),
            TextDirection.rtl => (
                sliderTheme.disabledActiveTickMarkColor,
                sliderTheme.activeTickMarkColor
            ),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = new ColorTween(begin: beginLocal, end: endLocal).evaluate(
                        enableAnimation
                    )!;
                    return __cascade;
                }
            )
        )();
        double tickMarkRadius =
            getPreferredSize(isEnabled: isEnabled, sliderTheme: sliderTheme).width / 2L;
        if ((tickMarkRadius) > 0L)
        {
            context.canvas.drawCircle(center, ((tickMarkRadius)), paintLocal);
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

    public virtual void paint(
        PaintingContext context,
        Offset center,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        bool isEnabled,
        TextDirection textDirection
    ) { }
}

public class RoundSliderThumbShape : SliderComponentShape
{
    public Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter? labelPainter = null,
        double? textScaleFactor = null
    ) => getPreferredSize(isEnabled, isDiscrete);

    public virtual double enabledThumbRadius { get; private set; } = default!;
    public virtual double? disabledThumbRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual double pressedElevation { get; private set; } = default!;

    public RoundSliderThumbShape(
        double enabledThumbRadius = 10.0,
        double? disabledThumbRadius = null,
        double elevation = 1.0,
        double pressedElevation = 6.0
    )
    {
        this.enabledThumbRadius = enabledThumbRadius;
        this.disabledThumbRadius = disabledThumbRadius;
        this.elevation = elevation;
        this.pressedElevation = pressedElevation;
    }

    internal virtual double _disabledThumbRadius =>
        DartRuntimePrimitives.ConvertValue<double>(disabledThumbRadius ?? enabledThumbRadius);

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return Size.fromRadius(isEnabled ? enabledThumbRadius : _disabledThumbRadius);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete,
        TextPainter labelPainter,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        TextDirection textDirection,
        double value,
        double textScaleFactor,
        Size sizeWithOverflow
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledThumbColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbColor is not null);
        Canvas canvasLocal = context.canvas;
        var radiusTween = new Tween<double>(begin: _disabledThumbRadius, end: enabledThumbRadius);
        var colorTween = new ColorTween(
            begin: sliderTheme.disabledThumbColor,
            end: sliderTheme.thumbColor
        );
        Color colorLocal = colorTween.evaluate(enableAnimation)!;
        double radius = radiusTween.evaluate(enableAnimation);
        var elevationTween = new Tween<double>(begin: elevation, end: pressedElevation);
        double evaluatedElevation = elevationTween.evaluate(activationAnimation);
        var path = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.addArc(
                        Rect.fromCenter(center: center, width: 2L * radius, height: 2L * radius),
                        0,
                        Dart_mathLibrary.pi * 2L
                    );
                    return __cascade;
                }
            )
        )();
        var paintShadows = true;
        DartRuntimePrimitives.Assert(() =>
        {
            if (Painting.DebugLibrary.debugDisableShadows)
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
        canvasLocal.drawCircle(
            center,
            radius,
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = colorLocal;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class DropSliderValueIndicatorShape : SliderComponentShape
{
    internal static _DropSliderValueIndicatorPathPainter__slider_parts _pathPainter =
        new _DropSliderValueIndicatorPathPainter__slider_parts();

    public DropSliderValueIndicatorShape() { }

    public virtual Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter? labelPainter = null,
        double? textScaleFactor = null
    )
    {
        DartRuntimePrimitives.Assert(() => labelPainter is not null);
        DartRuntimePrimitives.Assert(() =>
            (textScaleFactor is not null) && (textScaleFactor >= 0L)
        );
        return _pathPainter.getPreferredSize(
            labelPainter!,
            (
                textScaleFactor
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete,
        TextPainter labelPainter,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        TextDirection textDirection,
        double value,
        double textScaleFactor,
        Size sizeWithOverflow
    )
    {
        Canvas canvasLocal = context.canvas;
        double scaleLocal = activationAnimation.value;
        _pathPainter.paint(
            parentBox: parentBox,
            canvas: canvasLocal,
            center: center,
            scale: scaleLocal,
            labelPainter: labelPainter,
            textScaleFactor: ((textScaleFactor)),
            sizeWithOverflow: sizeWithOverflow,
            backgroundPaintColor: sliderTheme.valueIndicatorColor!,
            strokePaintColor: sliderTheme.valueIndicatorStrokeColor
        );
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
    internal static double _preferredHalfHeight = _preferredHeight / 2L;
    internal const double _upperRectRadius = 4;

    internal _DropSliderValueIndicatorPathPainter__slider_parts() { }

    public virtual Size getPreferredSize(TextPainter labelPainter, double textScaleFactor)
    {
        double widthLocal =
            Math.Max(_minLabelWidth, labelPainter.width) + (_labelPadding * 2L * textScaleFactor);
        return new Size(widthLocal, _preferredHeight * textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(
        RenderBox parentBox,
        Offset center,
        TextPainter labelPainter,
        double textScaleFactor,
        Size sizeWithOverflow,
        double scale
    )
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding = 8.0;
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        Offset globalCenter = parentBox.localToGlobal(center);
        double overflowLeft = Math.Max(0, (rectangleWidth / 2L) - globalCenter.dx + edgePadding);
        double overflowRight = Math.Max(
            0,
            (rectangleWidth / 2L) - (sizeWithOverflow.width - globalCenter.dx - edgePadding)
        );
        if (rectangleWidth < sizeWithOverflow.width)
        {
            return overflowLeft - overflowRight;
        }
        else
        {
            if ((overflowLeft - overflowRight) > 0L)
            {
                return overflowLeft - (edgePadding * textScaleFactor);
            }
            else
            {
                return -overflowRight + (edgePadding * textScaleFactor);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(TextPainter labelPainter, double scale)
    {
        double unscaledWidth = Math.Max(_minLabelWidth, labelPainter.width) + _labelPadding;
        return unscaledWidth * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderRadius _adjustBorderRadius(Rect rect)
    {
        var rectness = 0.0;
        return BorderRadius.lerp(
            BorderRadius.CreateAll(Radius.circular(_upperRectRadius)),
            BorderRadius.CreateAll(Radius.circular(rect.shortestSide / 2.0)),
            1.0 - rectness
        )!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        RenderBox parentBox,
        Canvas canvas,
        Offset center,
        double scale,
        TextPainter labelPainter,
        double textScaleFactor,
        Size sizeWithOverflow,
        Color backgroundPaintColor,
        Color? strokePaintColor = null
    )
    {
        if (scale == 0.0)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift = getHorizontalShift(
            parentBox: parentBox,
            center: center,
            labelPainter: labelPainter,
            textScaleFactor: textScaleFactor,
            sizeWithOverflow: sizeWithOverflow,
            scale: scale
        );
        var upperRect = Rect.fromLTWH(
            (-rectangleWidth / 2L) + horizontalShift,
            -_rectYOffset - _minRectHeight,
            rectangleWidth,
            _minRectHeight
        );
        var fillPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = backgroundPaintColor;
                    return __cascade;
                }
            )
        )();
        canvas.save();
        canvas.translate(center.dx, center.dy - _bottomTipYOffset);
        canvas.scale(scale, scale);
        BorderRadius adjustedBorderRadius = _adjustBorderRadius(upperRect);
        RRect borderRect = adjustedBorderRadius
            .resolve(labelPainter.textDirection)
            .toRRect(upperRect);
        var trianglePath = (
            (Func<Path>)(
                () =>
                {
                    var __cascade = new Path();
                    __cascade.lineTo(-_triangleHeight, -_triangleHeight);
                    __cascade.lineTo(_triangleHeight, -_triangleHeight);
                    __cascade.close();
                    return __cascade;
                }
            )
        )();
        trianglePath.addRRect(borderRect);
        if (strokePaintColor is not null)
        {
            var strokePaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = strokePaintColor;
                        __cascade.strokeWidth = 1.0;
                        __cascade.style = PaintingStyle.stroke;
                        return __cascade;
                    }
                )
            )();
            canvas.drawPath(trianglePath, strokePaint);
        }
        canvas.drawPath(trianglePath, fillPaint);
        double bottomTipToUpperRectTranslateY = (-_preferredHalfHeight / 2L) - upperRect.height;
        canvas.translate(0, bottomTipToUpperRectTranslateY);
        var boxCenter = new Offset(horizontalShift, upperRect.height / 1.75);
        var halfLabelPainterOffset = new Offset(labelPainter.width / 2L, labelPainter.height / 2L);
        Offset labelOffset = boxCenter - halfLabelPainterOffset;
        labelPainter.paint(canvas, labelOffset);
        canvas.restore();
    }
}

public class HandleThumbShape : SliderComponentShape
{
    public Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter? labelPainter = null,
        double? textScaleFactor = null
    ) => getPreferredSize(isEnabled, isDiscrete);

    public HandleThumbShape() { }

    public virtual Size getPreferredSize(bool isEnabled, bool isDiscrete)
    {
        return new Size(4.0, 44.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete,
        TextPainter labelPainter,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        TextDirection textDirection,
        double value,
        double textScaleFactor,
        Size sizeWithOverflow
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledThumbColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbSize is not null);
        var colorTween = new ColorTween(
            begin: sliderTheme.disabledThumbColor,
            end: sliderTheme.thumbColor
        );
        Color colorLocal = colorTween.evaluate(enableAnimation)!;
        Canvas canvasLocal = context.canvas;
        Size thumbSizeLocal = (
            sliderTheme.thumbSize!.resolve(new HashSet<WidgetState>())
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        var rrect = RRect.fromRectAndRadius(
            Rect.fromCenter(
                center: center,
                width: thumbSizeLocal.width,
                height: thumbSizeLocal.height
            ),
            Radius.circular(thumbSizeLocal.shortestSide / 2L)
        );
        canvasLocal.drawRRect(
            rrect,
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = colorLocal;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class GappedSliderTrackShape : SliderTrackShape, BaseSliderTrackShape
{
    public GappedSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset thumbCenter,
        Offset? secondaryOffset = null,
        bool isEnabled = default!,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        Offset __thumbCenter = thumbCenter;
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.thumbShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.trackGap is not null);
        DartRuntimePrimitives.Assert(() =>
            !(
                sliderTheme.trackGap
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ).isNegative()
        );
        if (
            (sliderTheme.trackHeight is null)
            || (
                (
                    sliderTheme.trackHeight
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) <= 0L
            )
        )
        {
            return;
        }
        var activeTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledActiveTrackColor,
            end: sliderTheme.activeTrackColor
        );
        var inactiveTrackColorTween = new ColorTween(
            begin: sliderTheme.disabledInactiveTrackColor,
            end: sliderTheme.inactiveTrackColor
        );
        var activePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = activeTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        var inactivePaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = inactiveTrackColorTween.evaluate(enableAnimation)!;
                    return __cascade;
                }
            )
        )();
        Paint leftTrackPaint = default!;
        Paint rightTrackPaint = default!;
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
        double trackGapLocal = (
            sliderTheme.trackGap
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        Rect trackRect = getPreferredRect(
            parentBox: parentBox,
            offset: offset,
            sliderTheme: sliderTheme,
            isEnabled: isEnabled,
            isDiscrete: isDiscrete
        );
        var trackCornerRadius = Radius.circular(trackRect.shortestSide / 2L);
        var trackInsideCornerRadius = Radius.circular(2.0);
        var trackRRect = RRect.fromRectAndCorners(
            trackRect,
            topLeft: trackCornerRadius,
            bottomLeft: trackCornerRadius,
            topRight: trackCornerRadius,
            bottomRight: trackCornerRadius
        );
        var leftRRect = RRect.fromLTRBAndCorners(
            trackRect.left,
            trackRect.top,
            Math.Max(trackRect.left, __thumbCenter.dx - trackGapLocal),
            trackRect.bottom,
            topLeft: trackCornerRadius,
            bottomLeft: trackCornerRadius,
            topRight: trackInsideCornerRadius,
            bottomRight: trackInsideCornerRadius
        );
        var rightRRect = RRect.fromLTRBAndCorners(
            __thumbCenter.dx + trackGapLocal,
            trackRect.top,
            trackRect.right,
            trackRect.bottom,
            topRight: trackCornerRadius,
            bottomRight: trackCornerRadius,
            topLeft: trackInsideCornerRadius,
            bottomLeft: trackInsideCornerRadius
        );
        DartRuntimePrimitives.Ignore(
            (
                (Func<Canvas>)(
                    () =>
                    {
                        var __cascade = context.canvas;
                        __cascade.save();
                        __cascade.clipRRect(trackRRect);
                        return __cascade;
                    }
                )
            )()
        );
        bool drawLeftTrack =
            __thumbCenter.dx
            > leftRRect.left
                + (
                    (
                        sliderTheme.trackHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2L
                );
        bool drawRightTrack =
            __thumbCenter.dx
            < rightRRect.right
                - (
                    (
                        sliderTheme.trackHeight
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) / 2L
                );
        if (drawLeftTrack)
        {
            context.canvas.drawRRect(leftRRect, leftTrackPaint);
        }
        if (drawRightTrack)
        {
            context.canvas.drawRRect(rightRRect, rightTrackPaint);
        }
        var isLTR = Equals(textDirection, TextDirection.ltr);
        bool showSecondaryTrack =
            secondaryOffset is not null
            && (
                (object)isLTR switch
                {
                    true => (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx > (__thumbCenter.dx + trackGapLocal),
                    false => (
                        secondaryOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ).dx < (__thumbCenter.dx - trackGapLocal),
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                }
            );
        if (showSecondaryTrack)
        {
            var secondaryTrackColorTween = new ColorTween(
                begin: sliderTheme.disabledSecondaryActiveTrackColor,
                end: sliderTheme.secondaryActiveTrackColor
            );
            var secondaryTrackPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = secondaryTrackColorTween.evaluate(enableAnimation)!;
                        return __cascade;
                    }
                )
            )();
            if (isLTR)
            {
                context.canvas.drawRRect(
                    RRect.fromLTRBAndCorners(
                        __thumbCenter.dx + trackGapLocal,
                        trackRect.top,
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx,
                        trackRect.bottom,
                        topLeft: trackInsideCornerRadius,
                        bottomLeft: trackInsideCornerRadius,
                        topRight: trackCornerRadius,
                        bottomRight: trackCornerRadius
                    ),
                    secondaryTrackPaint
                );
            }
            else
            {
                context.canvas.drawRRect(
                    RRect.fromLTRBAndCorners(
                        (
                            secondaryOffset
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ).dx - trackGapLocal,
                        trackRect.top,
                        __thumbCenter.dx,
                        trackRect.bottom,
                        topLeft: trackInsideCornerRadius,
                        bottomLeft: trackInsideCornerRadius,
                        topRight: trackCornerRadius,
                        bottomRight: trackCornerRadius
                    ),
                    secondaryTrackPaint
                );
            }
        }
        context.canvas.restore();
        var stopIndicatorRadius = 2.0;
        double stopIndicatorTrailingSpace =
            (
                sliderTheme.trackHeight
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) / 2L;
        var stopIndicatorOffset = new Offset(
            Equals(textDirection, TextDirection.ltr)
                ? (trackRect.centerRight.dx - stopIndicatorTrailingSpace)
                : (trackRect.centerLeft.dx + stopIndicatorTrailingSpace),
            trackRect.center.dy
        );
        bool showStopIndicator = Equals(textDirection, TextDirection.ltr)
            ? (__thumbCenter.dx < stopIndicatorOffset.dx)
            : (__thumbCenter.dx > stopIndicatorOffset.dx);
        if (showStopIndicator && !isDiscrete)
        {
            var stopIndicatorRect = Rect.fromCircle(
                center: stopIndicatorOffset,
                radius: stopIndicatorRadius
            );
            context.canvas.drawCircle(stopIndicatorRect.center, stopIndicatorRadius, activePaint);
        }
    }

    public override bool isRounded => true;

    public override Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = false,
        bool isDiscrete = false
    )
    {
        double thumbWidth = sliderTheme.thumbShape!.getPreferredSize(isEnabled, isDiscrete).width;
        double overlayWidth = sliderTheme
            .overlayShape!.getPreferredSize(isEnabled, isDiscrete)
            .width;
        double trackHeightLocal = (
            sliderTheme.trackHeight
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        DartRuntimePrimitives.Assert(() => overlayWidth >= 0L);
        DartRuntimePrimitives.Assert(() => trackHeightLocal >= 0L);
        if (
            Equals(sliderTheme.activeTrackColor, Colors.transparent)
            && Equals(sliderTheme.inactiveTrackColor, Colors.transparent)
        )
        {
            trackHeightLocal = 0;
        }
        double trackLeft =
            offset.dx
            + ((sliderTheme.padding is null) ? Math.Max(overlayWidth / 2L, thumbWidth / 2L) : 0L);
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - ((sliderTheme.padding is null) ? Math.Max(thumbWidth, overlayWidth) : 0L);
        double trackBottom = trackTop + trackHeightLocal;
        return Rect.fromLTRB(
            Math.Min(trackLeft, trackRight),
            trackTop,
            Math.Max(trackLeft, trackRight),
            trackBottom
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RoundedRectSliderValueIndicatorShape : SliderComponentShape
{
    internal static _RoundedRectSliderValueIndicatorPathPainter__slider_parts _pathPainter =
        new _RoundedRectSliderValueIndicatorPathPainter__slider_parts();

    public RoundedRectSliderValueIndicatorShape() { }

    public virtual Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter? labelPainter = null,
        double? textScaleFactor = null
    )
    {
        DartRuntimePrimitives.Assert(() => labelPainter is not null);
        DartRuntimePrimitives.Assert(() =>
            (textScaleFactor is not null) && (textScaleFactor >= 0L)
        );
        return _pathPainter.getPreferredSize(
            labelPainter!,
            (
                textScaleFactor
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete,
        TextPainter labelPainter,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        TextDirection textDirection,
        double value,
        double textScaleFactor,
        Size sizeWithOverflow
    )
    {
        Canvas canvasLocal = context.canvas;
        double scaleLocal = activationAnimation.value;
        _pathPainter.paint(
            parentBox: parentBox,
            canvas: canvasLocal,
            center: center,
            scale: scaleLocal,
            labelPainter: labelPainter,
            textScaleFactor: ((textScaleFactor)),
            sizeWithOverflow: sizeWithOverflow,
            backgroundPaintColor: sliderTheme.valueIndicatorColor!,
            strokePaintColor: sliderTheme.valueIndicatorStrokeColor
        );
    }
}

internal class _RoundedRectSliderValueIndicatorPathPainter__slider_parts
{
    internal const double _labelPadding = 10.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _rectYOffset = 10.0;
    internal const double _bottomTipYOffset = 16.0;
    internal static double _preferredHalfHeight = _preferredHeight / 2L;

    internal _RoundedRectSliderValueIndicatorPathPainter__slider_parts() { }

    public virtual Size getPreferredSize(TextPainter labelPainter, double textScaleFactor)
    {
        double widthLocal =
            Math.Max(_minLabelWidth, labelPainter.width) + (_labelPadding * 2L * textScaleFactor);
        return new Size(widthLocal, _preferredHeight * textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getHorizontalShift(
        RenderBox parentBox,
        Offset center,
        TextPainter labelPainter,
        double textScaleFactor,
        Size sizeWithOverflow,
        double scale
    )
    {
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        var edgePadding = 8.0;
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        Offset globalCenter = parentBox.localToGlobal(center);
        double overflowLeft = Math.Max(0, (rectangleWidth / 2L) - globalCenter.dx + edgePadding);
        double overflowRight = Math.Max(
            0,
            (rectangleWidth / 2L) - (sizeWithOverflow.width - globalCenter.dx - edgePadding)
        );
        if (rectangleWidth < sizeWithOverflow.width)
        {
            return overflowLeft - overflowRight;
        }
        else
        {
            if ((overflowLeft - overflowRight) > 0L)
            {
                return overflowLeft - (edgePadding * textScaleFactor);
            }
            else
            {
                return -overflowRight + (edgePadding * textScaleFactor);
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _upperRectangleWidth(TextPainter labelPainter, double scale)
    {
        double unscaledWidth = Math.Max(_minLabelWidth, labelPainter.width) + (_labelPadding * 2L);
        return unscaledWidth * scale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paint(
        RenderBox parentBox,
        Canvas canvas,
        Offset center,
        double scale,
        TextPainter labelPainter,
        double textScaleFactor,
        Size sizeWithOverflow,
        Color backgroundPaintColor,
        Color? strokePaintColor = null
    )
    {
        if (scale == 0.0)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => !sizeWithOverflow.isEmpty);
        double rectangleWidth = _upperRectangleWidth(labelPainter, scale);
        double horizontalShift = getHorizontalShift(
            parentBox: parentBox,
            center: center,
            labelPainter: labelPainter,
            textScaleFactor: textScaleFactor,
            sizeWithOverflow: sizeWithOverflow,
            scale: scale
        );
        var upperRect = Rect.fromLTWH(
            (-rectangleWidth / 2L) + horizontalShift,
            -_rectYOffset - _preferredHeight,
            rectangleWidth,
            _preferredHeight
        );
        var fillPaint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = backgroundPaintColor;
                    return __cascade;
                }
            )
        )();
        canvas.save();
        canvas.translate(center.dx, center.dy - _bottomTipYOffset);
        canvas.scale(scale, scale);
        var rrect = RRect.fromRectAndRadius(upperRect, Radius.circular(upperRect.height / 2L));
        if (strokePaintColor is not null)
        {
            var strokePaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = strokePaintColor;
                        __cascade.strokeWidth = 1.0;
                        __cascade.style = PaintingStyle.stroke;
                        return __cascade;
                    }
                )
            )();
            canvas.drawRRect(rrect, strokePaint);
        }
        canvas.drawRRect(rrect, fillPaint);
        double bottomTipToUpperRectTranslateY = (-_preferredHalfHeight / 2L) - upperRect.height;
        canvas.translate(0, bottomTipToUpperRectTranslateY);
        var boxCenter = new Offset(horizontalShift, upperRect.height / 2.3);
        var halfLabelPainterOffset = new Offset(labelPainter.width / 2L, labelPainter.height / 2L);
        Offset labelOffset = boxCenter - halfLabelPainterOffset;
        labelPainter.paint(canvas, labelOffset);
        canvas.restore();
    }
}

public static partial class Slider_partsLibrary
{
    internal static void _debugDrawShadow(Canvas canvas, Path path, double elevation)
    {
        if (elevation > 0.0)
        {
            canvas.drawPath(
                path,
                (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = Colors.black;
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.strokeWidth = elevation * 2.0;
                            return __cascade;
                        }
                    )
                )()
            );
        }
    }
}
