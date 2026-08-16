// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/slider_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class SliderTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual SliderThemeData data { get; private set; } = default!;

    public SliderTheme(global::Doroti.Framework.Foundation.Key? key = null, SliderThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SliderThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        SliderTheme? inheritedTheme__3612 = ((SliderTheme?)(object?)context.dependOnInheritedWidgetOfExactType<SliderTheme>());
        return ((inheritedTheme__3612 is not null) ? ((SliderTheme)inheritedTheme__3612).data : Theme.of(context).sliderTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new SliderTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SliderTheme)oldWidget).data)));
}

public enum ShowValueIndicator
{
    onlyForDiscrete,
    onlyForContinuous,
    always,
    onDrag,
    alwaysVisible,
    never
}

public enum Thumb
{
    start,
    end
}

public class SliderThemeData : global::Doroti.Framework.Foundation.Diagnosticable
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
    public virtual global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle { get; private set; }
    public virtual double? minThumbSeparation { get; private set; }
    public virtual global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>? thumbSelector { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor { get; private set; }
    public virtual SliderInteraction? allowedInteraction { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? thumbSize { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual bool? year2023 { get; private set; }

    public SliderThemeData(double? trackHeight = null, Color? activeTrackColor = null, Color? inactiveTrackColor = null, Color? secondaryActiveTrackColor = null, Color? disabledActiveTrackColor = null, Color? disabledInactiveTrackColor = null, Color? disabledSecondaryActiveTrackColor = null, Color? activeTickMarkColor = null, Color? inactiveTickMarkColor = null, Color? disabledActiveTickMarkColor = null, Color? disabledInactiveTickMarkColor = null, Color? thumbColor = null, Color? overlappingShapeStrokeColor = null, Color? disabledThumbColor = null, Color? overlayColor = null, Color? valueIndicatorColor = null, Color? valueIndicatorStrokeColor = null, SliderComponentShape? overlayShape = null, SliderTickMarkShape? tickMarkShape = null, SliderComponentShape? thumbShape = null, SliderTrackShape? trackShape = null, SliderComponentShape? valueIndicatorShape = null, RangeSliderTickMarkShape? rangeTickMarkShape = null, RangeSliderThumbShape? rangeThumbShape = null, RangeSliderTrackShape? rangeTrackShape = null, RangeSliderValueIndicatorShape? rangeValueIndicatorShape = null, ShowValueIndicator? showValueIndicator = null, global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle = null, double? minThumbSeparation = null, global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>? thumbSelector = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, SliderInteraction? allowedInteraction = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? thumbSize = null, double? trackGap = null, bool? year2023 = null)
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

    public static SliderThemeData CreateFromPrimaryColors(Color primaryColor, Color primaryColorDark, Color primaryColorLight, global::Doroti.Framework.Painting.TextStyle valueIndicatorTextStyle)
    {
        var activeTrackAlpha__12859 = 255L;
        var inactiveTrackAlpha__12894 = 61L;
        var secondaryActiveTrackAlpha__12946 = 138L;
        var disabledActiveTrackAlpha__13005 = 82L;
        var disabledInactiveTrackAlpha__13063 = 31L;
        var disabledSecondaryActiveTrackAlpha__13123 = 31L;
        var activeTickMarkAlpha__13190 = 138L;
        var inactiveTickMarkAlpha__13243 = 138L;
        var disabledActiveTickMarkAlpha__13298 = 31L;
        var disabledInactiveTickMarkAlpha__13359 = 31L;
        var thumbAlpha__13422 = 255L;
        var disabledThumbAlpha__13451 = 82L;
        var overlayAlpha__13503 = 31L;
        var valueIndicatorAlpha__13549 = 255L;
        return new SliderThemeData(trackHeight: 2.0, activeTrackColor: primaryColor.withAlpha(activeTrackAlpha__12859), inactiveTrackColor: primaryColor.withAlpha(inactiveTrackAlpha__12894), secondaryActiveTrackColor: primaryColor.withAlpha(secondaryActiveTrackAlpha__12946), disabledActiveTrackColor: primaryColorDark.withAlpha(disabledActiveTrackAlpha__13005), disabledInactiveTrackColor: primaryColorDark.withAlpha(disabledInactiveTrackAlpha__13063), disabledSecondaryActiveTrackColor: primaryColorDark.withAlpha(disabledSecondaryActiveTrackAlpha__13123), activeTickMarkColor: primaryColorLight.withAlpha(activeTickMarkAlpha__13190), inactiveTickMarkColor: primaryColor.withAlpha(inactiveTickMarkAlpha__13243), disabledActiveTickMarkColor: primaryColorLight.withAlpha(disabledActiveTickMarkAlpha__13298), disabledInactiveTickMarkColor: primaryColorDark.withAlpha(disabledInactiveTickMarkAlpha__13359), thumbColor: primaryColor.withAlpha(thumbAlpha__13422), overlappingShapeStrokeColor: Colors.white, disabledThumbColor: primaryColorDark.withAlpha(disabledThumbAlpha__13451), overlayColor: primaryColor.withAlpha(overlayAlpha__13503), valueIndicatorColor: primaryColor.withAlpha(valueIndicatorAlpha__13549), valueIndicatorStrokeColor: primaryColor.withAlpha(valueIndicatorAlpha__13549), overlayShape: new RoundSliderOverlayShape(), tickMarkShape: new RoundSliderTickMarkShape(), thumbShape: new RoundSliderThumbShape(), trackShape: new RoundedRectSliderTrackShape(), valueIndicatorShape: new PaddleSliderValueIndicatorShape(), rangeTickMarkShape: new RoundRangeSliderTickMarkShape(), rangeThumbShape: new RoundRangeSliderThumbShape(), rangeTrackShape: new RoundedRectRangeSliderTrackShape(), rangeValueIndicatorShape: new PaddleRangeSliderValueIndicatorShape(), valueIndicatorTextStyle: valueIndicatorTextStyle, showValueIndicator: ShowValueIndicator.onlyForDiscrete);
    }

    public virtual SliderThemeData copyWith(double? trackHeight = null, Color? activeTrackColor = null, Color? inactiveTrackColor = null, Color? secondaryActiveTrackColor = null, Color? disabledActiveTrackColor = null, Color? disabledInactiveTrackColor = null, Color? disabledSecondaryActiveTrackColor = null, Color? activeTickMarkColor = null, Color? inactiveTickMarkColor = null, Color? disabledActiveTickMarkColor = null, Color? disabledInactiveTickMarkColor = null, Color? thumbColor = null, Color? overlappingShapeStrokeColor = null, Color? disabledThumbColor = null, Color? overlayColor = null, Color? valueIndicatorColor = null, Color? valueIndicatorStrokeColor = null, SliderComponentShape? overlayShape = null, SliderTickMarkShape? tickMarkShape = null, SliderComponentShape? thumbShape = null, SliderTrackShape? trackShape = null, SliderComponentShape? valueIndicatorShape = null, RangeSliderTickMarkShape? rangeTickMarkShape = null, RangeSliderThumbShape? rangeThumbShape = null, RangeSliderTrackShape? rangeTrackShape = null, RangeSliderValueIndicatorShape? rangeValueIndicatorShape = null, ShowValueIndicator? showValueIndicator = null, global::Doroti.Framework.Painting.TextStyle? valueIndicatorTextStyle = null, double? minThumbSeparation = null, global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>? thumbSelector = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>? mouseCursor = null, SliderInteraction? allowedInteraction = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Size?>? thumbSize = null, double? trackGap = null, bool? year2023 = null)
    {
        return new SliderThemeData(trackHeight: (trackHeight ?? this.trackHeight), activeTrackColor: (activeTrackColor ?? this.activeTrackColor), inactiveTrackColor: (inactiveTrackColor ?? this.inactiveTrackColor), secondaryActiveTrackColor: (secondaryActiveTrackColor ?? this.secondaryActiveTrackColor), disabledActiveTrackColor: (disabledActiveTrackColor ?? this.disabledActiveTrackColor), disabledInactiveTrackColor: (disabledInactiveTrackColor ?? this.disabledInactiveTrackColor), disabledSecondaryActiveTrackColor: (disabledSecondaryActiveTrackColor ?? this.disabledSecondaryActiveTrackColor), activeTickMarkColor: (activeTickMarkColor ?? this.activeTickMarkColor), inactiveTickMarkColor: (inactiveTickMarkColor ?? this.inactiveTickMarkColor), disabledActiveTickMarkColor: (disabledActiveTickMarkColor ?? this.disabledActiveTickMarkColor), disabledInactiveTickMarkColor: (disabledInactiveTickMarkColor ?? this.disabledInactiveTickMarkColor), thumbColor: (thumbColor ?? this.thumbColor), overlappingShapeStrokeColor: (overlappingShapeStrokeColor ?? this.overlappingShapeStrokeColor), disabledThumbColor: (disabledThumbColor ?? this.disabledThumbColor), overlayColor: (overlayColor ?? this.overlayColor), valueIndicatorColor: (valueIndicatorColor ?? this.valueIndicatorColor), valueIndicatorStrokeColor: (valueIndicatorStrokeColor ?? this.valueIndicatorStrokeColor), overlayShape: (overlayShape ?? this.overlayShape), tickMarkShape: (tickMarkShape ?? this.tickMarkShape), thumbShape: (thumbShape ?? this.thumbShape), trackShape: (trackShape ?? this.trackShape), valueIndicatorShape: (valueIndicatorShape ?? this.valueIndicatorShape), rangeTickMarkShape: (rangeTickMarkShape ?? this.rangeTickMarkShape), rangeThumbShape: (rangeThumbShape ?? this.rangeThumbShape), rangeTrackShape: (rangeTrackShape ?? this.rangeTrackShape), rangeValueIndicatorShape: (rangeValueIndicatorShape ?? this.rangeValueIndicatorShape), showValueIndicator: (showValueIndicator ?? this.showValueIndicator), valueIndicatorTextStyle: (valueIndicatorTextStyle ?? this.valueIndicatorTextStyle), minThumbSeparation: (minThumbSeparation ?? this.minThumbSeparation), thumbSelector: ((thumbSelector ?? (global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>)this.thumbSelector)), mouseCursor: (mouseCursor ?? this.mouseCursor), allowedInteraction: (allowedInteraction ?? this.allowedInteraction), padding: (padding ?? this.padding), thumbSize: (thumbSize ?? this.thumbSize), trackGap: (trackGap ?? this.trackGap), year2023: (year2023 ?? this.year2023));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliderThemeData lerp(SliderThemeData a, SliderThemeData b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new SliderThemeData(trackHeight: Dart_uiLibrary.lerpDouble(((SliderThemeData)a).trackHeight, ((SliderThemeData)b).trackHeight, t), activeTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).activeTrackColor, ((SliderThemeData)b).activeTrackColor, t), inactiveTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).inactiveTrackColor, ((SliderThemeData)b).inactiveTrackColor, t), secondaryActiveTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).secondaryActiveTrackColor, ((SliderThemeData)b).secondaryActiveTrackColor, t), disabledActiveTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledActiveTrackColor, ((SliderThemeData)b).disabledActiveTrackColor, t), disabledInactiveTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledInactiveTrackColor, ((SliderThemeData)b).disabledInactiveTrackColor, t), disabledSecondaryActiveTrackColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledSecondaryActiveTrackColor, ((SliderThemeData)b).disabledSecondaryActiveTrackColor, t), activeTickMarkColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).activeTickMarkColor, ((SliderThemeData)b).activeTickMarkColor, t), inactiveTickMarkColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).inactiveTickMarkColor, ((SliderThemeData)b).inactiveTickMarkColor, t), disabledActiveTickMarkColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledActiveTickMarkColor, ((SliderThemeData)b).disabledActiveTickMarkColor, t), disabledInactiveTickMarkColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledInactiveTickMarkColor, ((SliderThemeData)b).disabledInactiveTickMarkColor, t), thumbColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).thumbColor, ((SliderThemeData)b).thumbColor, t), overlappingShapeStrokeColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).overlappingShapeStrokeColor, ((SliderThemeData)b).overlappingShapeStrokeColor, t), disabledThumbColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).disabledThumbColor, ((SliderThemeData)b).disabledThumbColor, t), overlayColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).overlayColor, ((SliderThemeData)b).overlayColor, t), valueIndicatorColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).valueIndicatorColor, ((SliderThemeData)b).valueIndicatorColor, t), valueIndicatorStrokeColor: Dart_uiLibrary.Color.lerp(((SliderThemeData)a).valueIndicatorStrokeColor, ((SliderThemeData)b).valueIndicatorStrokeColor, t), overlayShape: ((t < 0.5) ? ((SliderThemeData)a).overlayShape : ((SliderThemeData)b).overlayShape), tickMarkShape: ((t < 0.5) ? ((SliderThemeData)a).tickMarkShape : ((SliderThemeData)b).tickMarkShape), thumbShape: ((t < 0.5) ? ((SliderThemeData)a).thumbShape : ((SliderThemeData)b).thumbShape), trackShape: ((t < 0.5) ? ((SliderThemeData)a).trackShape : ((SliderThemeData)b).trackShape), valueIndicatorShape: ((t < 0.5) ? ((SliderThemeData)a).valueIndicatorShape : ((SliderThemeData)b).valueIndicatorShape), rangeTickMarkShape: ((t < 0.5) ? ((SliderThemeData)a).rangeTickMarkShape : ((SliderThemeData)b).rangeTickMarkShape), rangeThumbShape: ((t < 0.5) ? ((SliderThemeData)a).rangeThumbShape : ((SliderThemeData)b).rangeThumbShape), rangeTrackShape: ((t < 0.5) ? ((SliderThemeData)a).rangeTrackShape : ((SliderThemeData)b).rangeTrackShape), rangeValueIndicatorShape: ((t < 0.5) ? ((SliderThemeData)a).rangeValueIndicatorShape : ((SliderThemeData)b).rangeValueIndicatorShape), showValueIndicator: ((t < 0.5) ? ((SliderThemeData)a).showValueIndicator : ((SliderThemeData)b).showValueIndicator), valueIndicatorTextStyle: TextStyle.lerp(((SliderThemeData)a).valueIndicatorTextStyle, ((SliderThemeData)b).valueIndicatorTextStyle, t), minThumbSeparation: Dart_uiLibrary.lerpDouble(((SliderThemeData)a).minThumbSeparation, ((SliderThemeData)b).minThumbSeparation, t), thumbSelector: ((global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>)((t < 0.5) ? ((SliderThemeData)a).thumbSelector : ((SliderThemeData)b).thumbSelector)), mouseCursor: ((t < 0.5) ? ((SliderThemeData)a).mouseCursor : ((SliderThemeData)b).mouseCursor), allowedInteraction: ((t < 0.5) ? ((SliderThemeData)a).allowedInteraction : ((SliderThemeData)b).allowedInteraction), padding: EdgeInsetsGeometry.lerp(((SliderThemeData)a).padding, ((SliderThemeData)b).padding, t), thumbSize: WidgetStateProperty.lerp<global::Doroti.Ui.Size?>(((SliderThemeData)a).thumbSize, ((SliderThemeData)b).thumbSize, t, (global::System.Func<Size?, Size?, double, Size?>)Size.lerp), trackGap: Dart_uiLibrary.lerpDouble(((SliderThemeData)a).trackGap, ((SliderThemeData)b).trackGap, t), year2023: ((t < 0.5) ? ((SliderThemeData)a).year2023 : ((SliderThemeData)b).year2023));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.trackHeight, this.activeTrackColor, this.inactiveTrackColor, this.secondaryActiveTrackColor, this.disabledActiveTrackColor, this.disabledInactiveTrackColor, this.disabledSecondaryActiveTrackColor, this.activeTickMarkColor, this.inactiveTickMarkColor, this.disabledActiveTickMarkColor, this.disabledInactiveTickMarkColor, this.thumbColor, this.overlappingShapeStrokeColor, this.disabledThumbColor, this.overlayColor, this.valueIndicatorColor, this.overlayShape, this.tickMarkShape, this.thumbShape, FoundationRuntimePorts.ObjectHash(this.trackShape, this.valueIndicatorShape, this.rangeTickMarkShape, this.rangeThumbShape, this.rangeTrackShape, this.rangeValueIndicatorShape, this.showValueIndicator, this.valueIndicatorTextStyle, this.minThumbSeparation, this.thumbSelector, this.mouseCursor, this.allowedInteraction, this.padding, this.thumbSize, this.trackGap, this.year2023)));
    public override bool Equals(object? other)
    {
        var __other = other as SliderThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((((((((((((((__other is SliderThemeData) && (((SliderThemeData)((SliderThemeData)__other)).trackHeight == this.trackHeight)) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).activeTrackColor, this.activeTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).inactiveTrackColor, this.inactiveTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).secondaryActiveTrackColor, this.secondaryActiveTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledActiveTrackColor, this.disabledActiveTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledInactiveTrackColor, this.disabledInactiveTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledSecondaryActiveTrackColor, this.disabledSecondaryActiveTrackColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).activeTickMarkColor, this.activeTickMarkColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).inactiveTickMarkColor, this.inactiveTickMarkColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledActiveTickMarkColor, this.disabledActiveTickMarkColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledInactiveTickMarkColor, this.disabledInactiveTickMarkColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).thumbColor, this.thumbColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).overlappingShapeStrokeColor, this.overlappingShapeStrokeColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).disabledThumbColor, this.disabledThumbColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).overlayColor, this.overlayColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).valueIndicatorColor, this.valueIndicatorColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).valueIndicatorStrokeColor, this.valueIndicatorStrokeColor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).overlayShape, this.overlayShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).tickMarkShape, this.tickMarkShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).thumbShape, this.thumbShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).trackShape, this.trackShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).valueIndicatorShape, this.valueIndicatorShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).rangeTickMarkShape, this.rangeTickMarkShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).rangeThumbShape, this.rangeThumbShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).rangeTrackShape, this.rangeTrackShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).rangeValueIndicatorShape, this.rangeValueIndicatorShape))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).showValueIndicator, this.showValueIndicator))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).valueIndicatorTextStyle, this.valueIndicatorTextStyle))) && (((SliderThemeData)((SliderThemeData)__other)).minThumbSeparation == this.minThumbSeparation)) && (object.Equals((global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>?)((SliderThemeData)((SliderThemeData)__other)).thumbSelector, (global::System.Func<TextDirection, RangeValues, double, Size, Size, double, Thumb?>?)this.thumbSelector))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).mouseCursor, this.mouseCursor))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).allowedInteraction, this.allowedInteraction))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).padding, this.padding))) && (object.Equals(((SliderThemeData)((SliderThemeData)__other)).thumbSize, this.thumbSize))) && (((SliderThemeData)((SliderThemeData)__other)).trackGap == this.trackGap)) && (((SliderThemeData)((SliderThemeData)__other)).year2023 == this.year2023));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultData__37487 = new SliderThemeData();
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("trackHeight", this.trackHeight, defaultValue: ((SliderThemeData)defaultData__37487).trackHeight));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("activeTrackColor", this.activeTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).activeTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inactiveTrackColor", this.inactiveTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).inactiveTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryActiveTrackColor", this.secondaryActiveTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).secondaryActiveTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledActiveTrackColor", this.disabledActiveTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledActiveTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledInactiveTrackColor", this.disabledInactiveTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledInactiveTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledSecondaryActiveTrackColor", this.disabledSecondaryActiveTrackColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledSecondaryActiveTrackColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("activeTickMarkColor", this.activeTickMarkColor, defaultValue: ((SliderThemeData)defaultData__37487).activeTickMarkColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inactiveTickMarkColor", this.inactiveTickMarkColor, defaultValue: ((SliderThemeData)defaultData__37487).inactiveTickMarkColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledActiveTickMarkColor", this.disabledActiveTickMarkColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledActiveTickMarkColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledInactiveTickMarkColor", this.disabledInactiveTickMarkColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledInactiveTickMarkColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("thumbColor", this.thumbColor, defaultValue: ((SliderThemeData)defaultData__37487).thumbColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("overlappingShapeStrokeColor", this.overlappingShapeStrokeColor, defaultValue: ((SliderThemeData)defaultData__37487).overlappingShapeStrokeColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledThumbColor", this.disabledThumbColor, defaultValue: ((SliderThemeData)defaultData__37487).disabledThumbColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("overlayColor", this.overlayColor, defaultValue: ((SliderThemeData)defaultData__37487).overlayColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("valueIndicatorColor", this.valueIndicatorColor, defaultValue: ((SliderThemeData)defaultData__37487).valueIndicatorColor));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("valueIndicatorStrokeColor", this.valueIndicatorStrokeColor, defaultValue: ((SliderThemeData)defaultData__37487).valueIndicatorStrokeColor));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderComponentShape>("overlayShape", this.overlayShape, defaultValue: ((SliderThemeData)defaultData__37487).overlayShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderTickMarkShape>("tickMarkShape", this.tickMarkShape, defaultValue: ((SliderThemeData)defaultData__37487).tickMarkShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderComponentShape>("thumbShape", this.thumbShape, defaultValue: ((SliderThemeData)defaultData__37487).thumbShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderTrackShape>("trackShape", this.trackShape, defaultValue: ((SliderThemeData)defaultData__37487).trackShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliderComponentShape>("valueIndicatorShape", this.valueIndicatorShape, defaultValue: ((SliderThemeData)defaultData__37487).valueIndicatorShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RangeSliderTickMarkShape>("rangeTickMarkShape", this.rangeTickMarkShape, defaultValue: ((SliderThemeData)defaultData__37487).rangeTickMarkShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RangeSliderThumbShape>("rangeThumbShape", this.rangeThumbShape, defaultValue: ((SliderThemeData)defaultData__37487).rangeThumbShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RangeSliderTrackShape>("rangeTrackShape", this.rangeTrackShape, defaultValue: ((SliderThemeData)defaultData__37487).rangeTrackShape));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RangeSliderValueIndicatorShape>("rangeValueIndicatorShape", this.rangeValueIndicatorShape, defaultValue: ((SliderThemeData)defaultData__37487).rangeValueIndicatorShape));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<ShowValueIndicator>("showValueIndicator", this.showValueIndicator, defaultValue: ((SliderThemeData)defaultData__37487).showValueIndicator));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("valueIndicatorTextStyle", this.valueIndicatorTextStyle, defaultValue: ((SliderThemeData)defaultData__37487).valueIndicatorTextStyle));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("minThumbSeparation", this.minThumbSeparation, defaultValue: ((SliderThemeData)defaultData__37487).minThumbSeparation));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<RangeThumbSelector>("thumbSelector", this.thumbSelector, defaultValue: ((SliderThemeData)defaultData__37487).thumbSelector));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Services.MouseCursor?>>("mouseCursor", this.mouseCursor, defaultValue: ((SliderThemeData)defaultData__37487).mouseCursor));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<SliderInteraction>("allowedInteraction", this.allowedInteraction, defaultValue: ((SliderThemeData)defaultData__37487).allowedInteraction));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: ((SliderThemeData)defaultData__37487).padding));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Size?>>("thumbSize", this.thumbSize, defaultValue: ((SliderThemeData)defaultData__37487).thumbSize));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("trackGap", this.trackGap, defaultValue: ((SliderThemeData)defaultData__37487).trackGap));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("year2023", this.year2023, defaultValue: ((SliderThemeData)defaultData__37487).year2023));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate string SemanticFormatterCallback(double value);
