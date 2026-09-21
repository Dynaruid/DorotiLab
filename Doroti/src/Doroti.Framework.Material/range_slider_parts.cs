// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/range_slider_parts.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public interface RangeSliderThumbShape
{
    public Size getPreferredSize(bool isEnabled, bool isDiscrete);
    public void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete = default!,
        bool isEnabled = default!,
        bool isOnTop = default!,
        TextDirection textDirection = default!,
        SliderThemeData sliderTheme = default!,
        Thumb thumb = default!,
        bool isPressed = default!
    );
}

public abstract class RangeSliderValueIndicatorShape
{
    protected RangeSliderValueIndicatorShape() { }

    public abstract Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter labelPainter,
        double textScaleFactor
    );

    public virtual double getHorizontalShift(
        RenderBox? parentBox = null,
        Offset? center = null,
        TextPainter? labelPainter = null,
        Animation<double>? activationAnimation = null,
        double? textScaleFactor = null,
        Size? sizeWithOverflow = null
    )
    {
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete = default!,
        bool isOnTop = default!,
        TextPainter labelPainter = default!,
        double textScaleFactor = default!,
        Size sizeWithOverflow = default!,
        RenderBox parentBox = default!,
        SliderThemeData sliderTheme = default!,
        TextDirection textDirection = default!,
        double value = default!,
        Thumb thumb = default!
    );
}

public interface RangeSliderTickMarkShape
{
    public Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled = default!);
    public void paint(
        PaintingContext context,
        Offset center,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = default!,
        TextDirection textDirection = default!
    );
}

public abstract class RangeSliderTrackShape
{
    protected RangeSliderTrackShape() { }

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
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = false,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    );
    public virtual bool isRounded => false;
}

public interface BaseRangeSliderTrackShape
{
    public Rect getPreferredRect(
        RenderBox parentBox,
        Offset offset = default,
        SliderThemeData sliderTheme = default!,
        bool isEnabled = false,
        bool isDiscrete = false
    );
}

public class RectangularRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{
    public RectangularRangeSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = false,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
        DartRuntimePrimitives.Assert(() => enableAnimation is not null);
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
                    __cascade.color = activeTrackColorTween.evaluate(enableAnimation!)!;
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
        var (leftThumbOffset, rightThumbOffset) = textDirection switch
        {
            TextDirection.ltr => (startThumbCenter, endThumbCenter),
            TextDirection.rtl => (endThumbCenter, startThumbCenter),
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
            leftThumbOffset.dx,
            trackRect.bottom
        );
        if (!leftTrackSegment.isEmpty)
        {
            context.canvas.drawRect(leftTrackSegment, inactivePaint);
        }
        var middleTrackSegment = Rect.fromLTRB(
            leftThumbOffset.dx,
            trackRect.top,
            rightThumbOffset.dx,
            trackRect.bottom
        );
        if (!middleTrackSegment.isEmpty)
        {
            context.canvas.drawRect(middleTrackSegment, activePaint);
        }
        var rightTrackSegment = Rect.fromLTRB(
            rightThumbOffset.dx,
            trackRect.top,
            trackRect.right,
            trackRect.bottom
        );
        if (!rightTrackSegment.isEmpty)
        {
            context.canvas.drawRect(rightTrackSegment, inactivePaint);
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
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.overlayShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.trackHeight is not null);
        Size thumbSize = sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete);
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
            + (
                (sliderTheme.padding is null)
                    ? Math.Max(overlayWidth / 2L, thumbSize.width / 2L)
                    : (thumbSize.width / 2L)
            );
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - (
                (sliderTheme.padding is null)
                    ? Math.Max(thumbSize.width, overlayWidth)
                    : thumbSize.width
            );
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

public class RoundedRectRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{
    public RoundedRectRangeSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = false,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
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
        var (leftThumbOffset, rightThumbOffset) = textDirection switch
        {
            TextDirection.ltr => (startThumbCenter, endThumbCenter),
            TextDirection.rtl => (endThumbCenter, startThumbCenter),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Size thumbSize = sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete);
        double thumbRadius = thumbSize.width / 2L;
        DartRuntimePrimitives.Assert(() => thumbRadius > 0L);
        Rect trackRect = getPreferredRect(
            parentBox: parentBox,
            offset: offset,
            sliderTheme: sliderTheme,
            isEnabled: isEnabled,
            isDiscrete: isDiscrete
        );
        var trackRadius = Radius.circular(trackRect.height / 2L);
        context.canvas.drawRRect(
            RRect.fromLTRBAndCorners(
                trackRect.left,
                trackRect.top,
                leftThumbOffset.dx,
                trackRect.bottom,
                topLeft: trackRadius,
                bottomLeft: trackRadius
            ),
            inactivePaint
        );
        context.canvas.drawRRect(
            RRect.fromLTRBAndCorners(
                rightThumbOffset.dx,
                trackRect.top,
                trackRect.right,
                trackRect.bottom,
                topRight: trackRadius,
                bottomRight: trackRadius
            ),
            inactivePaint
        );
        context.canvas.drawRRect(
            RRect.fromLTRBR(
                leftThumbOffset.dx
                    - (
                        (
                            sliderTheme.trackHeight
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ) / 2L
                    ),
                trackRect.top - (additionalActiveTrackHeight / 2L),
                rightThumbOffset.dx
                    + (
                        (
                            sliderTheme.trackHeight
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ) / 2L
                    ),
                trackRect.bottom + (additionalActiveTrackHeight / 2L),
                trackRadius
            ),
            activePaint
        );
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
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.overlayShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.trackHeight is not null);
        Size thumbSize = sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete);
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
            + (
                (sliderTheme.padding is null)
                    ? Math.Max(overlayWidth / 2L, thumbSize.width / 2L)
                    : (thumbSize.width / 2L)
            );
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - (
                (sliderTheme.padding is null)
                    ? Math.Max(thumbSize.width, overlayWidth)
                    : thumbSize.width
            );
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

public class RoundRangeSliderTickMarkShape : RangeSliderTickMarkShape
{
    public virtual double? tickMarkRadius { get; private set; }

    public RoundRangeSliderTickMarkShape(double? tickMarkRadius = null)
    {
        this.tickMarkRadius = tickMarkRadius;
    }

    public virtual Size getPreferredSize(SliderThemeData sliderTheme, bool isEnabled = false)
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
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = false,
        TextDirection textDirection = default!
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTickMarkColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTickMarkColor is not null);
        bool hasGap =
            (sliderTheme.trackGap is not null)
            && (
                (
                    sliderTheme.trackGap
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) > 0L
            );
        bool underThumb = (startThumbCenter.dx == center.dx) || (endThumbCenter.dx == center.dx);
        if (hasGap && underThumb)
        {
            return;
        }
        bool isBetweenThumbs = textDirection switch
        {
            TextDirection.ltr => (startThumbCenter.dx < center.dx)
                && (center.dx < endThumbCenter.dx),
            TextDirection.rtl => (endThumbCenter.dx < center.dx)
                && (center.dx < startThumbCenter.dx),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Color? beginLocal = isBetweenThumbs
            ? sliderTheme.disabledActiveTickMarkColor
            : sliderTheme.disabledInactiveTickMarkColor;
        Color? endLocal = isBetweenThumbs
            ? sliderTheme.activeTickMarkColor
            : sliderTheme.inactiveTickMarkColor;
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

public class RoundRangeSliderThumbShape : RangeSliderThumbShape
{
    public virtual double enabledThumbRadius { get; private set; } = default!;
    public virtual double? disabledThumbRadius { get; private set; }
    public virtual double elevation { get; private set; } = default!;
    public virtual double pressedElevation { get; private set; } = default!;

    public RoundRangeSliderThumbShape(
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
        bool isDiscrete = false,
        bool isEnabled = false,
        bool isOnTop = default!,
        TextDirection textDirection = default!,
        SliderThemeData sliderTheme = default!,
        Thumb thumb = default!,
        bool isPressed = default!
    )
    {
        var __sliderTheme = sliderTheme;
        DartRuntimePrimitives.Assert(() => __sliderTheme.showValueIndicator is not null);
        DartRuntimePrimitives.Assert(() => __sliderTheme.overlappingShapeStrokeColor is not null);
        Canvas canvasLocal = context.canvas;
        var radiusTween = new Tween<double>(begin: _disabledThumbRadius, end: enabledThumbRadius);
        var colorTween = new ColorTween(
            begin: __sliderTheme.disabledThumbColor,
            end: __sliderTheme.thumbColor
        );
        double radius = radiusTween.evaluate(enableAnimation);
        var elevationTween = new Tween<double>(begin: elevation, end: pressedElevation);
        if (isOnTop)
        {
            var strokePaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = __sliderTheme.overlappingShapeStrokeColor!;
                        __cascade.strokeWidth = 1.0;
                        __cascade.style = PaintingStyle.stroke;
                        return __cascade;
                    }
                )
            )();
            canvasLocal.drawCircle(center, radius, strokePaint);
        }
        Color colorLocal = colorTween.evaluate(enableAnimation)!;
        double evaluatedElevation =
            (isPressed) ? elevationTween.evaluate(activationAnimation) : elevation;
        var shadowPath = (
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
                Range_slider_partsLibrary._debugDrawShadow(
                    canvasLocal,
                    shadowPath,
                    evaluatedElevation
                );
                paintShadows = false;
            }
            return true;
        });
        if (paintShadows)
        {
            canvasLocal.drawShadow(shadowPath, Colors.black, evaluatedElevation, true);
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

public delegate Thumb? RangeThumbSelector(
    TextDirection textDirection,
    RangeValues values,
    double tapValue,
    Size thumbSize,
    Size trackSize,
    double dx
);

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
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is RangeValues) && (__other.start == start) && (__other.end == end);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(start, end));

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RangeValues")}({start}, {end})";
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
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is RangeLabels) && (__other.start == start) && (__other.end == end);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(start, end));

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "RangeLabels")}({start}, {end})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Range_slider_partsLibrary
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

public class GappedRangeSliderTrackShape : RangeSliderTrackShape, BaseRangeSliderTrackShape
{
    public GappedRangeSliderTrackShape() { }

    public override void paint(
        PaintingContext context,
        Offset offset,
        RenderBox parentBox,
        SliderThemeData sliderTheme,
        Animation<double> enableAnimation,
        Offset startThumbCenter,
        Offset endThumbCenter,
        bool isEnabled = false,
        bool isDiscrete = false,
        TextDirection textDirection = default!,
        double additionalActiveTrackHeight = 2
    )
    {
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledActiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.disabledInactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.activeTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.inactiveTrackColor is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
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
        Rect trackRect = getPreferredRect(
            parentBox: parentBox,
            offset: offset,
            sliderTheme: sliderTheme,
            isEnabled: isEnabled,
            isDiscrete: isDiscrete
        );
        var trackCornerRadius = Radius.circular(trackRect.shortestSide / 2L);
        var trackInsideCornerRadius = Radius.circular(2.0);
        var (leftThumbOffset, rightThumbOffset) = textDirection switch
        {
            TextDirection.ltr => (startThumbCenter, endThumbCenter),
            TextDirection.rtl => (endThumbCenter, startThumbCenter),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        Size thumbSize = sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete);
        double thumbRadius = thumbSize.width / 2L;
        DartRuntimePrimitives.Assert(() => thumbRadius > 0L);
        double trackGapLocal = (
            sliderTheme.trackGap
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
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
            leftThumbOffset.dx - trackGapLocal,
            trackRect.bottom,
            topLeft: trackCornerRadius,
            bottomLeft: trackCornerRadius,
            topRight: trackInsideCornerRadius,
            bottomRight: trackInsideCornerRadius
        );
        var rightRRect = RRect.fromLTRBAndCorners(
            rightThumbOffset.dx + trackGapLocal,
            trackRect.top,
            trackRect.right,
            trackRect.bottom,
            topLeft: trackInsideCornerRadius,
            bottomLeft: trackInsideCornerRadius,
            topRight: trackCornerRadius,
            bottomRight: trackCornerRadius
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
            startThumbCenter.dx
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
            endThumbCenter.dx
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
            context.canvas.drawRRect(leftRRect, inactivePaint);
        }
        if (drawRightTrack)
        {
            context.canvas.drawRRect(rightRRect, inactivePaint);
        }
        if ((leftThumbOffset.dx + trackGapLocal) < (rightThumbOffset.dx - trackGapLocal))
        {
            context.canvas.drawRRect(
                RRect.fromLTRBR(
                    leftThumbOffset.dx + trackGapLocal,
                    trackRect.top,
                    rightThumbOffset.dx - trackGapLocal,
                    trackRect.bottom,
                    trackInsideCornerRadius
                ),
                activePaint
            );
        }
        context.canvas.restore();
        var stopIndicatorRadius = 2.0;
        double stopIndicatorTrailingSpace =
            (
                sliderTheme.trackHeight
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) / 2L;
        var startStopIndicatorOffset = new Offset(
            trackRect.centerLeft.dx + stopIndicatorTrailingSpace,
            trackRect.center.dy
        );
        var endStopIndicatorOffset = new Offset(
            trackRect.centerRight.dx - stopIndicatorTrailingSpace,
            trackRect.center.dy
        );
        bool showStartStopIndicator = startThumbCenter.dx > startStopIndicatorOffset.dx;
        if (showStartStopIndicator && !isDiscrete)
        {
            var stopIndicatorRect = Rect.fromCircle(
                center: startStopIndicatorOffset,
                radius: stopIndicatorRadius
            );
            context.canvas.drawCircle(stopIndicatorRect.center, stopIndicatorRadius, activePaint);
        }
        bool showEndStopIndicator = endThumbCenter.dx < endStopIndicatorOffset.dx;
        if (showEndStopIndicator && !isDiscrete)
        {
            var stopIndicatorRectLocal = Rect.fromCircle(
                center: endStopIndicatorOffset,
                radius: stopIndicatorRadius
            );
            context.canvas.drawCircle(
                stopIndicatorRectLocal.center,
                stopIndicatorRadius,
                activePaint
            );
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
        DartRuntimePrimitives.Assert(() => sliderTheme.rangeThumbShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.overlayShape is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.trackHeight is not null);
        Size thumbSize = sliderTheme.rangeThumbShape!.getPreferredSize(isEnabled, isDiscrete);
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
            + (
                (sliderTheme.padding is null)
                    ? Math.Max(overlayWidth / 2L, thumbSize.width / 2L)
                    : (thumbSize.width / 2L)
            );
        double trackTop = offset.dy + ((parentBox.size.height - trackHeightLocal) / 2L);
        double trackRight =
            trackLeft
            + parentBox.size.width
            - (
                (sliderTheme.padding is null)
                    ? Math.Max(thumbSize.width, overlayWidth)
                    : thumbSize.width
            );
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

public class HandleRangeSliderThumbShape : RangeSliderThumbShape
{
    public HandleRangeSliderThumbShape() { }

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
        bool isDiscrete = false,
        bool isEnabled = false,
        bool isOnTop = default!,
        TextDirection textDirection = default!,
        SliderThemeData sliderTheme = default!,
        Thumb thumb = default!,
        bool isPressed = default!
    )
    {
        var __sliderTheme = (SliderThemeData)(object)textDirection;
        DartRuntimePrimitives.Assert(() => __sliderTheme.showValueIndicator is not null);
        DartRuntimePrimitives.Assert(() => __sliderTheme.overlappingShapeStrokeColor is not null);
        DartRuntimePrimitives.Assert(() => __sliderTheme.disabledThumbColor is not null);
        DartRuntimePrimitives.Assert(() => __sliderTheme.thumbColor is not null);
        DartRuntimePrimitives.Assert(() => __sliderTheme.thumbSize is not null);
        var colorTween = new ColorTween(
            begin: __sliderTheme.disabledThumbColor,
            end: __sliderTheme.thumbColor
        );
        Color colorLocal = colorTween.evaluate(enableAnimation)!;
        Canvas canvasLocal = context.canvas;
        Size thumbSizeLocal = (
            __sliderTheme.thumbSize!.resolve(new HashSet<WidgetState>())
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

public class RoundedRectRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts _pathPainter =
        new _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts();

    public RoundedRectRangeSliderValueIndicatorShape() { }

    public override Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter labelPainter = default!,
        double textScaleFactor = default!
    )
    {
        DartRuntimePrimitives.Assert(() => labelPainter is not null);
        DartRuntimePrimitives.Assert(() => textScaleFactor >= 0L);
        return _pathPainter.getPreferredSize(labelPainter!, (textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete = default!,
        bool isOnTop = default!,
        TextPainter labelPainter = default!,
        double textScaleFactor = default!,
        Size sizeWithOverflow = default!,
        RenderBox parentBox = default!,
        SliderThemeData sliderTheme = default!,
        TextDirection textDirection = default!,
        double value = default!,
        Thumb thumb = default!
    )
    {
        DartRuntimePrimitives.Assert(() => true);
        DartRuntimePrimitives.Assert(() => sizeWithOverflow is not null);
        DartRuntimePrimitives.Assert(() => sliderTheme.valueIndicatorColor is not null);
        Canvas canvasLocal = context.canvas;
        double scaleLocal = activationAnimation.value;
        _pathPainter.paint(
            parentBox: parentBox,
            canvas: canvasLocal,
            center: center,
            scale: scaleLocal,
            labelPainter: labelPainter,
            textScaleFactor: (textScaleFactor),
            sizeWithOverflow: (
                sizeWithOverflow
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            backgroundPaintColor: sliderTheme.valueIndicatorColor!,
            strokePaintColor: (isOnTop)
                ? sliderTheme.overlappingShapeStrokeColor
                : sliderTheme.valueIndicatorStrokeColor
        );
    }
}

public class DropRangeSliderValueIndicatorShape : RangeSliderValueIndicatorShape
{
    internal static _DropSliderValueIndicatorPathPainter__range_slider_parts _pathPainter =
        new _DropSliderValueIndicatorPathPainter__range_slider_parts();

    public DropRangeSliderValueIndicatorShape() { }

    public override Size getPreferredSize(
        bool isEnabled,
        bool isDiscrete,
        TextPainter labelPainter = default!,
        double textScaleFactor = default!
    )
    {
        DartRuntimePrimitives.Assert(() => labelPainter is not null);
        DartRuntimePrimitives.Assert(() => textScaleFactor >= 0L);
        return _pathPainter.getPreferredSize(labelPainter!, (textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(
        PaintingContext context,
        Offset center,
        Animation<double> activationAnimation,
        Animation<double> enableAnimation,
        bool isDiscrete = default!,
        bool isOnTop = default!,
        TextPainter labelPainter = default!,
        double textScaleFactor = default!,
        Size sizeWithOverflow = default!,
        RenderBox parentBox = default!,
        SliderThemeData sliderTheme = default!,
        TextDirection textDirection = default!,
        double value = default!,
        Thumb thumb = default!
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
            textScaleFactor: (textScaleFactor),
            sizeWithOverflow: (
                sizeWithOverflow
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            backgroundPaintColor: sliderTheme.valueIndicatorColor!,
            strokePaintColor: (isOnTop)
                ? sliderTheme.overlappingShapeStrokeColor
                : sliderTheme.valueIndicatorStrokeColor
        );
    }
}

internal class _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts
{
    internal const double _labelPadding = 10.0;
    internal const double _preferredHeight = 32.0;
    internal const double _minLabelWidth = 16.0;
    internal const double _rectYOffset = 10.0;
    internal const double _bottomTipYOffset = 16.0;
    internal static double _preferredHalfHeight = _preferredHeight / 2L;

    internal _RoundedRectSliderValueIndicatorPathPainter__range_slider_parts() { }

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

internal class _DropSliderValueIndicatorPathPainter__range_slider_parts
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

    internal _DropSliderValueIndicatorPathPainter__range_slider_parts() { }

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
