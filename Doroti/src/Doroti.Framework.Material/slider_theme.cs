// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class SliderTheme : InheritedTheme
{
    public virtual SliderThemeData data { get; private set; } = default!;

    public SliderTheme(Key? key = null, SliderThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SliderThemeData of(BuildContext context)
    {
        SliderTheme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<SliderTheme>();
        return (inheritedTheme is not null) ? inheritedTheme.data : Theme.of(context).sliderTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new SliderTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SliderTheme)oldWidget).data));
}

public enum ShowValueIndicator
{
    onlyForDiscrete,
    onlyForContinuous,
    always,
    onDrag,
    alwaysVisible,
    never,
}

public enum Thumb
{
    start,
    end,
}

public class SliderThemeData : Diagnosticable
{
    public virtual double? trackHeight { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual Color? secondaryActiveTrackColor { get; private set; }
    public virtual Color? disabledActiveTrackColor { get; private set; }
    public virtual Color? disabledSecondaryActiveTrackColor { get; private set; }
    public virtual Color? disabledInactiveTrackColor { get; private set; }
    public virtual Color? activeTickMarkColor { get; private set; }
    public virtual Color? inactiveTickMarkColor { get; private set; }
    public virtual Color? disabledActiveTickMarkColor { get; private set; }
    public virtual Color? disabledInactiveTickMarkColor { get; private set; }
    public virtual Color? thumbColor { get; private set; }
    public virtual Color? overlappingShapeStrokeColor { get; private set; }
    public virtual Color? disabledThumbColor { get; private set; }
    public virtual Color? overlayColor { get; private set; }
    public virtual Color? valueIndicatorColor { get; private set; }
    public virtual Color? valueIndicatorStrokeColor { get; private set; }
    public virtual SliderComponentShape? overlayShape { get; private set; }
    public virtual SliderTickMarkShape? tickMarkShape { get; private set; }
    public virtual SliderComponentShape? thumbShape { get; private set; }
    public virtual SliderTrackShape? trackShape { get; private set; }
    public virtual SliderComponentShape? valueIndicatorShape { get; private set; }
    public virtual RangeSliderTickMarkShape? rangeTickMarkShape { get; private set; }
    public virtual RangeSliderThumbShape? rangeThumbShape { get; private set; }
    public virtual RangeSliderTrackShape? rangeTrackShape { get; private set; }
    public virtual RangeSliderValueIndicatorShape? rangeValueIndicatorShape { get; private set; }
    public virtual ShowValueIndicator? showValueIndicator { get; private set; }
    public virtual TextStyle? valueIndicatorTextStyle { get; private set; }
    public virtual double? minThumbSeparation { get; private set; }
    public virtual Func<
        TextDirection,
        RangeValues,
        double,
        Size,
        Size,
        double,
        Thumb?
    >? thumbSelector { get; private set; }
    public virtual WidgetStateProperty<MouseCursor?>? mouseCursor { get; private set; }
    public virtual SliderInteraction? allowedInteraction { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual WidgetStateProperty<Size?>? thumbSize { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual bool? year2023 { get; private set; }

    public SliderThemeData(
        double? trackHeight = null,
        Color? activeTrackColor = null,
        Color? inactiveTrackColor = null,
        Color? secondaryActiveTrackColor = null,
        Color? disabledActiveTrackColor = null,
        Color? disabledInactiveTrackColor = null,
        Color? disabledSecondaryActiveTrackColor = null,
        Color? activeTickMarkColor = null,
        Color? inactiveTickMarkColor = null,
        Color? disabledActiveTickMarkColor = null,
        Color? disabledInactiveTickMarkColor = null,
        Color? thumbColor = null,
        Color? overlappingShapeStrokeColor = null,
        Color? disabledThumbColor = null,
        Color? overlayColor = null,
        Color? valueIndicatorColor = null,
        Color? valueIndicatorStrokeColor = null,
        SliderComponentShape? overlayShape = null,
        SliderTickMarkShape? tickMarkShape = null,
        SliderComponentShape? thumbShape = null,
        SliderTrackShape? trackShape = null,
        SliderComponentShape? valueIndicatorShape = null,
        RangeSliderTickMarkShape? rangeTickMarkShape = null,
        RangeSliderThumbShape? rangeThumbShape = null,
        RangeSliderTrackShape? rangeTrackShape = null,
        RangeSliderValueIndicatorShape? rangeValueIndicatorShape = null,
        ShowValueIndicator? showValueIndicator = null,
        TextStyle? valueIndicatorTextStyle = null,
        double? minThumbSeparation = null,
        Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>? thumbSelector = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        SliderInteraction? allowedInteraction = null,
        EdgeInsetsGeometry? padding = null,
        WidgetStateProperty<Size?>? thumbSize = null,
        double? trackGap = null,
        bool? year2023 = null
    )
    {
        this.trackHeight = trackHeight;
        this.activeTrackColor = activeTrackColor;
        this.inactiveTrackColor = inactiveTrackColor;
        this.secondaryActiveTrackColor = secondaryActiveTrackColor;
        this.disabledActiveTrackColor = disabledActiveTrackColor;
        this.disabledInactiveTrackColor = disabledInactiveTrackColor;
        this.disabledSecondaryActiveTrackColor = disabledSecondaryActiveTrackColor;
        this.activeTickMarkColor = activeTickMarkColor;
        this.inactiveTickMarkColor = inactiveTickMarkColor;
        this.disabledActiveTickMarkColor = disabledActiveTickMarkColor;
        this.disabledInactiveTickMarkColor = disabledInactiveTickMarkColor;
        this.thumbColor = thumbColor;
        this.overlappingShapeStrokeColor = overlappingShapeStrokeColor;
        this.disabledThumbColor = disabledThumbColor;
        this.overlayColor = overlayColor;
        this.valueIndicatorColor = valueIndicatorColor;
        this.valueIndicatorStrokeColor = valueIndicatorStrokeColor;
        this.overlayShape = overlayShape;
        this.tickMarkShape = tickMarkShape;
        this.thumbShape = thumbShape;
        this.trackShape = trackShape;
        this.valueIndicatorShape = valueIndicatorShape;
        this.rangeTickMarkShape = rangeTickMarkShape;
        this.rangeThumbShape = rangeThumbShape;
        this.rangeTrackShape = rangeTrackShape;
        this.rangeValueIndicatorShape = rangeValueIndicatorShape;
        this.showValueIndicator = showValueIndicator;
        this.valueIndicatorTextStyle = valueIndicatorTextStyle;
        this.minThumbSeparation = minThumbSeparation;
        this.thumbSelector = thumbSelector;
        this.mouseCursor = mouseCursor;
        this.allowedInteraction = allowedInteraction;
        this.padding = padding;
        this.thumbSize = thumbSize;
        this.trackGap = trackGap;
        this.year2023 = year2023;
    }

    public static SliderThemeData CreateFromPrimaryColors(
        Color primaryColor,
        Color primaryColorDark,
        Color primaryColorLight,
        TextStyle valueIndicatorTextStyle
    )
    {
        var activeTrackAlpha = 255L;
        var inactiveTrackAlpha = 61L;
        var secondaryActiveTrackAlpha = 138L;
        var disabledActiveTrackAlpha = 82L;
        var disabledInactiveTrackAlpha = 31L;
        var disabledSecondaryActiveTrackAlpha = 31L;
        var activeTickMarkAlpha = 138L;
        var inactiveTickMarkAlpha = 138L;
        var disabledActiveTickMarkAlpha = 31L;
        var disabledInactiveTickMarkAlpha = 31L;
        var thumbAlpha = 255L;
        var disabledThumbAlpha = 82L;
        var overlayAlpha = 31L;
        var valueIndicatorAlpha = 255L;
        return new SliderThemeData(
            trackHeight: 2.0,
            activeTrackColor: primaryColor.withAlpha(activeTrackAlpha),
            inactiveTrackColor: primaryColor.withAlpha(inactiveTrackAlpha),
            secondaryActiveTrackColor: primaryColor.withAlpha(secondaryActiveTrackAlpha),
            disabledActiveTrackColor: primaryColorDark.withAlpha(disabledActiveTrackAlpha),
            disabledInactiveTrackColor: primaryColorDark.withAlpha(disabledInactiveTrackAlpha),
            disabledSecondaryActiveTrackColor: primaryColorDark.withAlpha(
                disabledSecondaryActiveTrackAlpha
            ),
            activeTickMarkColor: primaryColorLight.withAlpha(activeTickMarkAlpha),
            inactiveTickMarkColor: primaryColor.withAlpha(inactiveTickMarkAlpha),
            disabledActiveTickMarkColor: primaryColorLight.withAlpha(disabledActiveTickMarkAlpha),
            disabledInactiveTickMarkColor: primaryColorDark.withAlpha(
                disabledInactiveTickMarkAlpha
            ),
            thumbColor: primaryColor.withAlpha(thumbAlpha),
            overlappingShapeStrokeColor: Colors.white,
            disabledThumbColor: primaryColorDark.withAlpha(disabledThumbAlpha),
            overlayColor: primaryColor.withAlpha(overlayAlpha),
            valueIndicatorColor: primaryColor.withAlpha(valueIndicatorAlpha),
            valueIndicatorStrokeColor: primaryColor.withAlpha(valueIndicatorAlpha),
            overlayShape: new RoundSliderOverlayShape(),
            tickMarkShape: new RoundSliderTickMarkShape(),
            thumbShape: new RoundSliderThumbShape(),
            trackShape: new RoundedRectSliderTrackShape(),
            valueIndicatorShape: new PaddleSliderValueIndicatorShape(),
            rangeTickMarkShape: new RoundRangeSliderTickMarkShape(),
            rangeThumbShape: new RoundRangeSliderThumbShape(),
            rangeTrackShape: new RoundedRectRangeSliderTrackShape(),
            rangeValueIndicatorShape: new PaddleRangeSliderValueIndicatorShape(),
            valueIndicatorTextStyle: valueIndicatorTextStyle,
            showValueIndicator: ShowValueIndicator.onlyForDiscrete
        );
    }

    public virtual SliderThemeData copyWith(
        double? trackHeight = null,
        Color? activeTrackColor = null,
        Color? inactiveTrackColor = null,
        Color? secondaryActiveTrackColor = null,
        Color? disabledActiveTrackColor = null,
        Color? disabledInactiveTrackColor = null,
        Color? disabledSecondaryActiveTrackColor = null,
        Color? activeTickMarkColor = null,
        Color? inactiveTickMarkColor = null,
        Color? disabledActiveTickMarkColor = null,
        Color? disabledInactiveTickMarkColor = null,
        Color? thumbColor = null,
        Color? overlappingShapeStrokeColor = null,
        Color? disabledThumbColor = null,
        Color? overlayColor = null,
        Color? valueIndicatorColor = null,
        Color? valueIndicatorStrokeColor = null,
        SliderComponentShape? overlayShape = null,
        SliderTickMarkShape? tickMarkShape = null,
        SliderComponentShape? thumbShape = null,
        SliderTrackShape? trackShape = null,
        SliderComponentShape? valueIndicatorShape = null,
        RangeSliderTickMarkShape? rangeTickMarkShape = null,
        RangeSliderThumbShape? rangeThumbShape = null,
        RangeSliderTrackShape? rangeTrackShape = null,
        RangeSliderValueIndicatorShape? rangeValueIndicatorShape = null,
        ShowValueIndicator? showValueIndicator = null,
        TextStyle? valueIndicatorTextStyle = null,
        double? minThumbSeparation = null,
        Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>? thumbSelector = null,
        WidgetStateProperty<MouseCursor?>? mouseCursor = null,
        SliderInteraction? allowedInteraction = null,
        EdgeInsetsGeometry? padding = null,
        WidgetStateProperty<Size?>? thumbSize = null,
        double? trackGap = null,
        bool? year2023 = null
    )
    {
        return new SliderThemeData(
            trackHeight: trackHeight ?? this.trackHeight,
            activeTrackColor: activeTrackColor ?? this.activeTrackColor,
            inactiveTrackColor: inactiveTrackColor ?? this.inactiveTrackColor,
            secondaryActiveTrackColor: secondaryActiveTrackColor ?? this.secondaryActiveTrackColor,
            disabledActiveTrackColor: disabledActiveTrackColor ?? this.disabledActiveTrackColor,
            disabledInactiveTrackColor: disabledInactiveTrackColor
                ?? this.disabledInactiveTrackColor,
            disabledSecondaryActiveTrackColor: disabledSecondaryActiveTrackColor
                ?? this.disabledSecondaryActiveTrackColor,
            activeTickMarkColor: activeTickMarkColor ?? this.activeTickMarkColor,
            inactiveTickMarkColor: inactiveTickMarkColor ?? this.inactiveTickMarkColor,
            disabledActiveTickMarkColor: disabledActiveTickMarkColor
                ?? this.disabledActiveTickMarkColor,
            disabledInactiveTickMarkColor: disabledInactiveTickMarkColor
                ?? this.disabledInactiveTickMarkColor,
            thumbColor: thumbColor ?? this.thumbColor,
            overlappingShapeStrokeColor: overlappingShapeStrokeColor
                ?? this.overlappingShapeStrokeColor,
            disabledThumbColor: disabledThumbColor ?? this.disabledThumbColor,
            overlayColor: overlayColor ?? this.overlayColor,
            valueIndicatorColor: valueIndicatorColor ?? this.valueIndicatorColor,
            valueIndicatorStrokeColor: valueIndicatorStrokeColor ?? this.valueIndicatorStrokeColor,
            overlayShape: overlayShape ?? this.overlayShape,
            tickMarkShape: tickMarkShape ?? this.tickMarkShape,
            thumbShape: thumbShape ?? this.thumbShape,
            trackShape: trackShape ?? this.trackShape,
            valueIndicatorShape: valueIndicatorShape ?? this.valueIndicatorShape,
            rangeTickMarkShape: rangeTickMarkShape ?? this.rangeTickMarkShape,
            rangeThumbShape: rangeThumbShape ?? this.rangeThumbShape,
            rangeTrackShape: rangeTrackShape ?? this.rangeTrackShape,
            rangeValueIndicatorShape: rangeValueIndicatorShape ?? this.rangeValueIndicatorShape,
            showValueIndicator: showValueIndicator ?? this.showValueIndicator,
            valueIndicatorTextStyle: valueIndicatorTextStyle ?? this.valueIndicatorTextStyle,
            minThumbSeparation: minThumbSeparation ?? this.minThumbSeparation,
            thumbSelector: thumbSelector ?? this.thumbSelector,
            mouseCursor: mouseCursor ?? this.mouseCursor,
            allowedInteraction: allowedInteraction ?? this.allowedInteraction,
            padding: padding ?? this.padding,
            thumbSize: thumbSize ?? this.thumbSize,
            trackGap: trackGap ?? this.trackGap,
            year2023: year2023 ?? this.year2023
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static SliderThemeData lerp(SliderThemeData a, SliderThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new SliderThemeData(
            trackHeight: Dart_uiLibrary.lerpDouble(a.trackHeight, b.trackHeight, t),
            activeTrackColor: Dart_uiLibrary.Color.lerp(a.activeTrackColor, b.activeTrackColor, t),
            inactiveTrackColor: Dart_uiLibrary.Color.lerp(
                a.inactiveTrackColor,
                b.inactiveTrackColor,
                t
            ),
            secondaryActiveTrackColor: Dart_uiLibrary.Color.lerp(
                a.secondaryActiveTrackColor,
                b.secondaryActiveTrackColor,
                t
            ),
            disabledActiveTrackColor: Dart_uiLibrary.Color.lerp(
                a.disabledActiveTrackColor,
                b.disabledActiveTrackColor,
                t
            ),
            disabledInactiveTrackColor: Dart_uiLibrary.Color.lerp(
                a.disabledInactiveTrackColor,
                b.disabledInactiveTrackColor,
                t
            ),
            disabledSecondaryActiveTrackColor: Dart_uiLibrary.Color.lerp(
                a.disabledSecondaryActiveTrackColor,
                b.disabledSecondaryActiveTrackColor,
                t
            ),
            activeTickMarkColor: Dart_uiLibrary.Color.lerp(
                a.activeTickMarkColor,
                b.activeTickMarkColor,
                t
            ),
            inactiveTickMarkColor: Dart_uiLibrary.Color.lerp(
                a.inactiveTickMarkColor,
                b.inactiveTickMarkColor,
                t
            ),
            disabledActiveTickMarkColor: Dart_uiLibrary.Color.lerp(
                a.disabledActiveTickMarkColor,
                b.disabledActiveTickMarkColor,
                t
            ),
            disabledInactiveTickMarkColor: Dart_uiLibrary.Color.lerp(
                a.disabledInactiveTickMarkColor,
                b.disabledInactiveTickMarkColor,
                t
            ),
            thumbColor: Dart_uiLibrary.Color.lerp(a.thumbColor, b.thumbColor, t),
            overlappingShapeStrokeColor: Dart_uiLibrary.Color.lerp(
                a.overlappingShapeStrokeColor,
                b.overlappingShapeStrokeColor,
                t
            ),
            disabledThumbColor: Dart_uiLibrary.Color.lerp(
                a.disabledThumbColor,
                b.disabledThumbColor,
                t
            ),
            overlayColor: Dart_uiLibrary.Color.lerp(a.overlayColor, b.overlayColor, t),
            valueIndicatorColor: Dart_uiLibrary.Color.lerp(
                a.valueIndicatorColor,
                b.valueIndicatorColor,
                t
            ),
            valueIndicatorStrokeColor: Dart_uiLibrary.Color.lerp(
                a.valueIndicatorStrokeColor,
                b.valueIndicatorStrokeColor,
                t
            ),
            overlayShape: (t < 0.5) ? a.overlayShape : b.overlayShape,
            tickMarkShape: (t < 0.5) ? a.tickMarkShape : b.tickMarkShape,
            thumbShape: (t < 0.5) ? a.thumbShape : b.thumbShape,
            trackShape: (t < 0.5) ? a.trackShape : b.trackShape,
            valueIndicatorShape: (t < 0.5) ? a.valueIndicatorShape : b.valueIndicatorShape,
            rangeTickMarkShape: (t < 0.5) ? a.rangeTickMarkShape : b.rangeTickMarkShape,
            rangeThumbShape: (t < 0.5) ? a.rangeThumbShape : b.rangeThumbShape,
            rangeTrackShape: (t < 0.5) ? a.rangeTrackShape : b.rangeTrackShape,
            rangeValueIndicatorShape: (t < 0.5)
                ? a.rangeValueIndicatorShape
                : b.rangeValueIndicatorShape,
            showValueIndicator: (t < 0.5) ? a.showValueIndicator : b.showValueIndicator,
            valueIndicatorTextStyle: TextStyle.lerp(
                a.valueIndicatorTextStyle,
                b.valueIndicatorTextStyle,
                t
            ),
            minThumbSeparation: Dart_uiLibrary.lerpDouble(
                a.minThumbSeparation,
                b.minThumbSeparation,
                t
            ),
            thumbSelector: (t < 0.5) ? a.thumbSelector : b.thumbSelector,
            mouseCursor: (t < 0.5) ? a.mouseCursor : b.mouseCursor,
            allowedInteraction: (t < 0.5) ? a.allowedInteraction : b.allowedInteraction,
            padding: EdgeInsetsGeometry.lerp(a.padding, b.padding, t),
            thumbSize: WidgetStateProperty.lerp(a.thumbSize, b.thumbSize, t, Size.lerp),
            trackGap: Dart_uiLibrary.lerpDouble(a.trackGap, b.trackGap, t),
            year2023: (t < 0.5) ? a.year2023 : b.year2023
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                trackHeight,
                activeTrackColor,
                inactiveTrackColor,
                secondaryActiveTrackColor,
                disabledActiveTrackColor,
                disabledInactiveTrackColor,
                disabledSecondaryActiveTrackColor,
                activeTickMarkColor,
                inactiveTickMarkColor,
                disabledActiveTickMarkColor,
                disabledInactiveTickMarkColor,
                thumbColor,
                overlappingShapeStrokeColor,
                disabledThumbColor,
                overlayColor,
                valueIndicatorColor,
                overlayShape,
                tickMarkShape,
                thumbShape,
                FoundationRuntimePorts.ObjectHash(
                    trackShape,
                    valueIndicatorShape,
                    rangeTickMarkShape,
                    rangeThumbShape,
                    rangeTrackShape,
                    rangeValueIndicatorShape,
                    showValueIndicator,
                    valueIndicatorTextStyle,
                    minThumbSeparation,
                    thumbSelector,
                    mouseCursor,
                    allowedInteraction,
                    padding,
                    thumbSize,
                    trackGap,
                    year2023
                )
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as SliderThemeData;
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
        return (__other is SliderThemeData)
            && (__other.trackHeight == trackHeight)
            && Equals(__other.activeTrackColor, activeTrackColor)
            && Equals(__other.inactiveTrackColor, inactiveTrackColor)
            && Equals(__other.secondaryActiveTrackColor, secondaryActiveTrackColor)
            && Equals(__other.disabledActiveTrackColor, disabledActiveTrackColor)
            && Equals(__other.disabledInactiveTrackColor, disabledInactiveTrackColor)
            && Equals(__other.disabledSecondaryActiveTrackColor, disabledSecondaryActiveTrackColor)
            && Equals(__other.activeTickMarkColor, activeTickMarkColor)
            && Equals(__other.inactiveTickMarkColor, inactiveTickMarkColor)
            && Equals(__other.disabledActiveTickMarkColor, disabledActiveTickMarkColor)
            && Equals(__other.disabledInactiveTickMarkColor, disabledInactiveTickMarkColor)
            && Equals(__other.thumbColor, thumbColor)
            && Equals(__other.overlappingShapeStrokeColor, overlappingShapeStrokeColor)
            && Equals(__other.disabledThumbColor, disabledThumbColor)
            && Equals(__other.overlayColor, overlayColor)
            && Equals(__other.valueIndicatorColor, valueIndicatorColor)
            && Equals(__other.valueIndicatorStrokeColor, valueIndicatorStrokeColor)
            && Equals(__other.overlayShape, overlayShape)
            && Equals(__other.tickMarkShape, tickMarkShape)
            && Equals(__other.thumbShape, thumbShape)
            && Equals(__other.trackShape, trackShape)
            && Equals(__other.valueIndicatorShape, valueIndicatorShape)
            && Equals(__other.rangeTickMarkShape, rangeTickMarkShape)
            && Equals(__other.rangeThumbShape, rangeThumbShape)
            && Equals(__other.rangeTrackShape, rangeTrackShape)
            && Equals(__other.rangeValueIndicatorShape, rangeValueIndicatorShape)
            && Equals(__other.showValueIndicator, showValueIndicator)
            && Equals(__other.valueIndicatorTextStyle, valueIndicatorTextStyle)
            && (__other.minThumbSeparation == minThumbSeparation)
            && Equals(__other.thumbSelector, thumbSelector)
            && Equals(__other.mouseCursor, mouseCursor)
            && Equals(__other.allowedInteraction, allowedInteraction)
            && Equals(__other.padding, padding)
            && Equals(__other.thumbSize, thumbSize)
            && (__other.trackGap == trackGap)
            && (__other.year2023 == year2023);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        var defaultData = new SliderThemeData();
        properties.add(
            new DoubleProperty("trackHeight", trackHeight, defaultValue: defaultData.trackHeight)
        );
        properties.add(
            new ColorProperty(
                "activeTrackColor",
                activeTrackColor,
                defaultValue: defaultData.activeTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "inactiveTrackColor",
                inactiveTrackColor,
                defaultValue: defaultData.inactiveTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "secondaryActiveTrackColor",
                secondaryActiveTrackColor,
                defaultValue: defaultData.secondaryActiveTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledActiveTrackColor",
                disabledActiveTrackColor,
                defaultValue: defaultData.disabledActiveTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledInactiveTrackColor",
                disabledInactiveTrackColor,
                defaultValue: defaultData.disabledInactiveTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledSecondaryActiveTrackColor",
                disabledSecondaryActiveTrackColor,
                defaultValue: defaultData.disabledSecondaryActiveTrackColor
            )
        );
        properties.add(
            new ColorProperty(
                "activeTickMarkColor",
                activeTickMarkColor,
                defaultValue: defaultData.activeTickMarkColor
            )
        );
        properties.add(
            new ColorProperty(
                "inactiveTickMarkColor",
                inactiveTickMarkColor,
                defaultValue: defaultData.inactiveTickMarkColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledActiveTickMarkColor",
                disabledActiveTickMarkColor,
                defaultValue: defaultData.disabledActiveTickMarkColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledInactiveTickMarkColor",
                disabledInactiveTickMarkColor,
                defaultValue: defaultData.disabledInactiveTickMarkColor
            )
        );
        properties.add(
            new ColorProperty("thumbColor", thumbColor, defaultValue: defaultData.thumbColor)
        );
        properties.add(
            new ColorProperty(
                "overlappingShapeStrokeColor",
                overlappingShapeStrokeColor,
                defaultValue: defaultData.overlappingShapeStrokeColor
            )
        );
        properties.add(
            new ColorProperty(
                "disabledThumbColor",
                disabledThumbColor,
                defaultValue: defaultData.disabledThumbColor
            )
        );
        properties.add(
            new ColorProperty("overlayColor", overlayColor, defaultValue: defaultData.overlayColor)
        );
        properties.add(
            new ColorProperty(
                "valueIndicatorColor",
                valueIndicatorColor,
                defaultValue: defaultData.valueIndicatorColor
            )
        );
        properties.add(
            new ColorProperty(
                "valueIndicatorStrokeColor",
                valueIndicatorStrokeColor,
                defaultValue: defaultData.valueIndicatorStrokeColor
            )
        );
        properties.add(
            new DiagnosticsProperty<SliderComponentShape>(
                "overlayShape",
                overlayShape,
                defaultValue: defaultData.overlayShape
            )
        );
        properties.add(
            new DiagnosticsProperty<SliderTickMarkShape>(
                "tickMarkShape",
                tickMarkShape,
                defaultValue: defaultData.tickMarkShape
            )
        );
        properties.add(
            new DiagnosticsProperty<SliderComponentShape>(
                "thumbShape",
                thumbShape,
                defaultValue: defaultData.thumbShape
            )
        );
        properties.add(
            new DiagnosticsProperty<SliderTrackShape>(
                "trackShape",
                trackShape,
                defaultValue: defaultData.trackShape
            )
        );
        properties.add(
            new DiagnosticsProperty<SliderComponentShape>(
                "valueIndicatorShape",
                valueIndicatorShape,
                defaultValue: defaultData.valueIndicatorShape
            )
        );
        properties.add(
            new DiagnosticsProperty<RangeSliderTickMarkShape>(
                "rangeTickMarkShape",
                rangeTickMarkShape,
                defaultValue: defaultData.rangeTickMarkShape
            )
        );
        properties.add(
            new DiagnosticsProperty<RangeSliderThumbShape>(
                "rangeThumbShape",
                rangeThumbShape,
                defaultValue: defaultData.rangeThumbShape
            )
        );
        properties.add(
            new DiagnosticsProperty<RangeSliderTrackShape>(
                "rangeTrackShape",
                rangeTrackShape,
                defaultValue: defaultData.rangeTrackShape
            )
        );
        properties.add(
            new DiagnosticsProperty<RangeSliderValueIndicatorShape>(
                "rangeValueIndicatorShape",
                rangeValueIndicatorShape,
                defaultValue: defaultData.rangeValueIndicatorShape
            )
        );
        properties.add(
            new EnumProperty<ShowValueIndicator>(
                "showValueIndicator",
                showValueIndicator,
                defaultValue: defaultData.showValueIndicator
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "valueIndicatorTextStyle",
                valueIndicatorTextStyle,
                defaultValue: defaultData.valueIndicatorTextStyle
            )
        );
        properties.add(
            new DoubleProperty(
                "minThumbSeparation",
                minThumbSeparation,
                defaultValue: defaultData.minThumbSeparation
            )
        );
        properties.add(
            new DiagnosticsProperty<RangeThumbSelector>(
                "thumbSelector",
                thumbSelector,
                defaultValue: defaultData.thumbSelector
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<MouseCursor?>>(
                "mouseCursor",
                mouseCursor,
                defaultValue: defaultData.mouseCursor
            )
        );
        properties.add(
            new EnumProperty<SliderInteraction>(
                "allowedInteraction",
                allowedInteraction,
                defaultValue: defaultData.allowedInteraction
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "padding",
                padding,
                defaultValue: defaultData.padding
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Size?>>(
                "thumbSize",
                thumbSize,
                defaultValue: defaultData.thumbSize
            )
        );
        properties.add(
            new DoubleProperty("trackGap", trackGap, defaultValue: defaultData.trackGap)
        );
        properties.add(
            new DiagnosticsProperty<bool>("year2023", year2023, defaultValue: defaultData.year2023)
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate string SemanticFormatterCallback(double value);
