// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/date_picker_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class DatePickerThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Color? headerBackgroundColor { get; private set; }
    public virtual Color? headerForegroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headerHeadlineStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headerHelpStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayForegroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayOverlayColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? dayShape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayForegroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderSide? todayBorder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? yearStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearForegroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearOverlayColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? yearShape { get; private set; }
    public virtual Color? rangePickerBackgroundColor { get; private set; }
    public virtual double? rangePickerElevation { get; private set; }
    public virtual Color? rangePickerShadowColor { get; private set; }
    public virtual Color? rangePickerSurfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? rangePickerShape { get; private set; }
    public virtual Color? rangePickerHeaderBackgroundColor { get; private set; }
    public virtual Color? rangePickerHeaderForegroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHeadlineStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHelpStyle { get; private set; }
    public virtual Color? rangeSelectionBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? rangeSelectionOverlayColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual ButtonStyle? cancelButtonStyle { get; private set; }
    public virtual ButtonStyle? confirmButtonStyle { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? toggleButtonTextStyle { get; private set; }
    public virtual Color? subHeaderForegroundColor { get; private set; }

    public DatePickerThemeData(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Color? headerBackgroundColor = null, Color? headerForegroundColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? headerHeadlineStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? headerHelpStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayOverlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? dayShape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayBackgroundColor = null, global::Doroti.Generated.Framework.Painting.BorderSide? todayBorder = null, global::Doroti.Generated.Framework.Painting.TextStyle? yearStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearOverlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? yearShape = null, Color? rangePickerBackgroundColor = null, double? rangePickerElevation = null, Color? rangePickerShadowColor = null, Color? rangePickerSurfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? rangePickerShape = null, Color? rangePickerHeaderBackgroundColor = null, Color? rangePickerHeaderForegroundColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHeadlineStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHelpStyle = null, Color? rangeSelectionBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? rangeSelectionOverlayColor = null, Color? dividerColor = null, object? inputDecorationTheme = null, ButtonStyle? cancelButtonStyle = null, ButtonStyle? confirmButtonStyle = null, Locale? locale = null, global::Doroti.Generated.Framework.Painting.TextStyle? toggleButtonTextStyle = null, Color? subHeaderForegroundColor = null)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.headerBackgroundColor = headerBackgroundColor;
        this.headerForegroundColor = headerForegroundColor;
        this.headerHeadlineStyle = headerHeadlineStyle;
        this.headerHelpStyle = headerHelpStyle;
        this.weekdayStyle = weekdayStyle;
        this.dayStyle = dayStyle;
        this.dayForegroundColor = dayForegroundColor;
        this.dayBackgroundColor = dayBackgroundColor;
        this.dayOverlayColor = dayOverlayColor;
        this.dayShape = dayShape;
        this.todayForegroundColor = todayForegroundColor;
        this.todayBackgroundColor = todayBackgroundColor;
        this.todayBorder = todayBorder;
        this.yearStyle = yearStyle;
        this.yearForegroundColor = yearForegroundColor;
        this.yearBackgroundColor = yearBackgroundColor;
        this.yearOverlayColor = yearOverlayColor;
        this.yearShape = yearShape;
        this.rangePickerBackgroundColor = rangePickerBackgroundColor;
        this.rangePickerElevation = rangePickerElevation;
        this.rangePickerShadowColor = rangePickerShadowColor;
        this.rangePickerSurfaceTintColor = rangePickerSurfaceTintColor;
        this.rangePickerShape = rangePickerShape;
        this.rangePickerHeaderBackgroundColor = rangePickerHeaderBackgroundColor;
        this.rangePickerHeaderForegroundColor = rangePickerHeaderForegroundColor;
        this.rangePickerHeaderHeadlineStyle = rangePickerHeaderHeadlineStyle;
        this.rangePickerHeaderHelpStyle = rangePickerHeaderHelpStyle;
        this.rangeSelectionBackgroundColor = rangeSelectionBackgroundColor;
        this.rangeSelectionOverlayColor = rangeSelectionOverlayColor;
        this.dividerColor = dividerColor;
        this.cancelButtonStyle = cancelButtonStyle;
        this.confirmButtonStyle = confirmButtonStyle;
        this.locale = locale;
        this.toggleButtonTextStyle = toggleButtonTextStyle;
        this.subHeaderForegroundColor = subHeaderForegroundColor;
        this._inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert(((inputDecorationTheme is null) || (((inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData)))));
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if ((this._inputDecorationTheme is null))
            {
                return null;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(this._inputDecorationTheme);
            return default!;
        }
    }
    public virtual DatePickerThemeData copyWith(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, Color? headerBackgroundColor = null, Color? headerForegroundColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? headerHeadlineStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? headerHelpStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayOverlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? dayShape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayBackgroundColor = null, global::Doroti.Generated.Framework.Painting.BorderSide? todayBorder = null, global::Doroti.Generated.Framework.Painting.TextStyle? yearStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearForegroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearOverlayColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>? yearShape = null, Color? rangePickerBackgroundColor = null, double? rangePickerElevation = null, Color? rangePickerShadowColor = null, Color? rangePickerSurfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? rangePickerShape = null, Color? rangePickerHeaderBackgroundColor = null, Color? rangePickerHeaderForegroundColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHeadlineStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHelpStyle = null, Color? rangeSelectionBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? rangeSelectionOverlayColor = null, Color? dividerColor = null, InputDecorationTheme? inputDecorationTheme = null, ButtonStyle? cancelButtonStyle = null, ButtonStyle? confirmButtonStyle = null, Locale? locale = null, global::Doroti.Generated.Framework.Painting.TextStyle? toggleButtonTextStyle = null, Color? subHeaderForegroundColor = null)
    {
        return new DatePickerThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), headerBackgroundColor: (headerBackgroundColor ?? this.headerBackgroundColor), headerForegroundColor: (headerForegroundColor ?? this.headerForegroundColor), headerHeadlineStyle: (headerHeadlineStyle ?? this.headerHeadlineStyle), headerHelpStyle: (headerHelpStyle ?? this.headerHelpStyle), weekdayStyle: (weekdayStyle ?? this.weekdayStyle), dayStyle: (dayStyle ?? this.dayStyle), dayForegroundColor: (dayForegroundColor ?? this.dayForegroundColor), dayBackgroundColor: (dayBackgroundColor ?? this.dayBackgroundColor), dayOverlayColor: (dayOverlayColor ?? this.dayOverlayColor), dayShape: (dayShape ?? this.dayShape), todayForegroundColor: (todayForegroundColor ?? this.todayForegroundColor), todayBackgroundColor: (todayBackgroundColor ?? this.todayBackgroundColor), todayBorder: (todayBorder ?? this.todayBorder), yearStyle: (yearStyle ?? this.yearStyle), yearForegroundColor: (yearForegroundColor ?? this.yearForegroundColor), yearBackgroundColor: (yearBackgroundColor ?? this.yearBackgroundColor), yearOverlayColor: (yearOverlayColor ?? this.yearOverlayColor), yearShape: (yearShape ?? this.yearShape), rangePickerBackgroundColor: (rangePickerBackgroundColor ?? this.rangePickerBackgroundColor), rangePickerElevation: (rangePickerElevation ?? this.rangePickerElevation), rangePickerShadowColor: (rangePickerShadowColor ?? this.rangePickerShadowColor), rangePickerSurfaceTintColor: (rangePickerSurfaceTintColor ?? this.rangePickerSurfaceTintColor), rangePickerShape: (rangePickerShape ?? this.rangePickerShape), rangePickerHeaderBackgroundColor: (rangePickerHeaderBackgroundColor ?? this.rangePickerHeaderBackgroundColor), rangePickerHeaderForegroundColor: (rangePickerHeaderForegroundColor ?? this.rangePickerHeaderForegroundColor), rangePickerHeaderHeadlineStyle: (rangePickerHeaderHeadlineStyle ?? this.rangePickerHeaderHeadlineStyle), rangePickerHeaderHelpStyle: (rangePickerHeaderHelpStyle ?? this.rangePickerHeaderHelpStyle), rangeSelectionBackgroundColor: (rangeSelectionBackgroundColor ?? this.rangeSelectionBackgroundColor), rangeSelectionOverlayColor: (rangeSelectionOverlayColor ?? this.rangeSelectionOverlayColor), dividerColor: (dividerColor ?? this.dividerColor), inputDecorationTheme: DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>((object?)inputDecorationTheme ?? this.inputDecorationTheme), cancelButtonStyle: (cancelButtonStyle ?? this.cancelButtonStyle), confirmButtonStyle: (confirmButtonStyle ?? this.confirmButtonStyle), locale: (locale ?? this.locale), toggleButtonTextStyle: (toggleButtonTextStyle ?? this.toggleButtonTextStyle), subHeaderForegroundColor: (subHeaderForegroundColor ?? this.subHeaderForegroundColor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DatePickerThemeData lerp(DatePickerThemeData? a, DatePickerThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new DatePickerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), headerBackgroundColor: Dart_uiLibrary.Color.lerp(a?.headerBackgroundColor, b?.headerBackgroundColor, t), headerForegroundColor: Dart_uiLibrary.Color.lerp(a?.headerForegroundColor, b?.headerForegroundColor, t), headerHeadlineStyle: TextStyle.lerp(a?.headerHeadlineStyle, b?.headerHeadlineStyle, t), headerHelpStyle: TextStyle.lerp(a?.headerHelpStyle, b?.headerHelpStyle, t), weekdayStyle: TextStyle.lerp(a?.weekdayStyle, b?.weekdayStyle, t), dayStyle: TextStyle.lerp(a?.dayStyle, b?.dayStyle, t), dayForegroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.dayForegroundColor, b?.dayForegroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), dayBackgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.dayBackgroundColor, b?.dayBackgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), dayOverlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.dayOverlayColor, b?.dayOverlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), dayShape: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(a?.dayShape, b?.dayShape, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.OutlinedBorder?, global::Doroti.Generated.Framework.Painting.OutlinedBorder?, double, global::Doroti.Generated.Framework.Painting.OutlinedBorder?>)global::Doroti.Generated.Framework.Painting.OutlinedBorder.lerp), todayForegroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.todayForegroundColor, b?.todayForegroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), todayBackgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.todayBackgroundColor, b?.todayBackgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), todayBorder: DatePickerThemeData._lerpBorderSide(a?.todayBorder, b?.todayBorder, t), yearStyle: TextStyle.lerp(a?.yearStyle, b?.yearStyle, t), yearForegroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.yearForegroundColor, b?.yearForegroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), yearBackgroundColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.yearBackgroundColor, b?.yearBackgroundColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), yearOverlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.yearOverlayColor, b?.yearOverlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), yearShape: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(a?.yearShape, b?.yearShape, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.OutlinedBorder?, global::Doroti.Generated.Framework.Painting.OutlinedBorder?, double, global::Doroti.Generated.Framework.Painting.OutlinedBorder?>)global::Doroti.Generated.Framework.Painting.OutlinedBorder.lerp), rangePickerBackgroundColor: Dart_uiLibrary.Color.lerp(a?.rangePickerBackgroundColor, b?.rangePickerBackgroundColor, t), rangePickerElevation: Dart_uiLibrary.lerpDouble(a?.rangePickerElevation, b?.rangePickerElevation, t), rangePickerShadowColor: Dart_uiLibrary.Color.lerp(a?.rangePickerShadowColor, b?.rangePickerShadowColor, t), rangePickerSurfaceTintColor: Dart_uiLibrary.Color.lerp(a?.rangePickerSurfaceTintColor, b?.rangePickerSurfaceTintColor, t), rangePickerShape: ShapeBorder.lerp(a?.rangePickerShape, b?.rangePickerShape, t), rangePickerHeaderBackgroundColor: Dart_uiLibrary.Color.lerp(a?.rangePickerHeaderBackgroundColor, b?.rangePickerHeaderBackgroundColor, t), rangePickerHeaderForegroundColor: Dart_uiLibrary.Color.lerp(a?.rangePickerHeaderForegroundColor, b?.rangePickerHeaderForegroundColor, t), rangePickerHeaderHeadlineStyle: TextStyle.lerp(a?.rangePickerHeaderHeadlineStyle, b?.rangePickerHeaderHeadlineStyle, t), rangePickerHeaderHelpStyle: TextStyle.lerp(a?.rangePickerHeaderHelpStyle, b?.rangePickerHeaderHelpStyle, t), rangeSelectionBackgroundColor: Dart_uiLibrary.Color.lerp(a?.rangeSelectionBackgroundColor, b?.rangeSelectionBackgroundColor, t), rangeSelectionOverlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.rangeSelectionOverlayColor, b?.rangeSelectionOverlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), dividerColor: Dart_uiLibrary.Color.lerp(a?.dividerColor, b?.dividerColor, t), inputDecorationTheme: ((t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme), cancelButtonStyle: ButtonStyle.lerp(a?.cancelButtonStyle, b?.cancelButtonStyle, t), confirmButtonStyle: ButtonStyle.lerp(a?.confirmButtonStyle, b?.confirmButtonStyle, t), locale: ((t < 0.5) ? a?.locale : b?.locale), toggleButtonTextStyle: TextStyle.lerp(a?.toggleButtonTextStyle, b?.toggleButtonTextStyle, t), subHeaderForegroundColor: Dart_uiLibrary.Color.lerp(a?.subHeaderForegroundColor, b?.subHeaderForegroundColor, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Painting.BorderSide? _lerpBorderSide(global::Doroti.Generated.Framework.Painting.BorderSide? a, global::Doroti.Generated.Framework.Painting.BorderSide? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0, color: b!.color.withAlpha(0L)), b, t));
        }
        return ((global::Doroti.Generated.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(a, new global::Doroti.Generated.Framework.Painting.BorderSide(width: 0, color: ((global::Doroti.Generated.Framework.Painting.BorderSide)a).color.withAlpha(0L)), t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { this.backgroundColor, this.elevation, this.shadowColor, this.surfaceTintColor, this.shape, this.headerBackgroundColor, this.headerForegroundColor, this.headerHeadlineStyle, this.headerHelpStyle, this.weekdayStyle, this.dayStyle, this.dayForegroundColor, this.dayBackgroundColor, this.dayOverlayColor, this.dayShape, this.todayForegroundColor, this.todayBackgroundColor, this.todayBorder, this.yearStyle, this.yearForegroundColor, this.yearBackgroundColor, this.yearOverlayColor, this.yearShape, this.rangePickerBackgroundColor, this.rangePickerElevation, this.rangePickerShadowColor, this.rangePickerSurfaceTintColor, this.rangePickerShape, this.rangePickerHeaderBackgroundColor, this.rangePickerHeaderForegroundColor, this.rangePickerHeaderHeadlineStyle, this.rangePickerHeaderHelpStyle, this.rangeSelectionBackgroundColor, this.rangeSelectionOverlayColor, this.dividerColor, this.inputDecorationTheme, this.cancelButtonStyle, this.confirmButtonStyle, this.locale, this.toggleButtonTextStyle, this.subHeaderForegroundColor }));
    public override bool Equals(object? other)
    {
        var __other = other as DatePickerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((((((((((((((((((((((((((((((((((((((((((__other is DatePickerThemeData) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).backgroundColor, this.backgroundColor))) && (((DatePickerThemeData)((DatePickerThemeData)__other)).elevation == this.elevation)) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).shape, this.shape))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).headerBackgroundColor, this.headerBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).headerForegroundColor, this.headerForegroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).headerHeadlineStyle, this.headerHeadlineStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).headerHelpStyle, this.headerHelpStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).weekdayStyle, this.weekdayStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dayStyle, this.dayStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dayForegroundColor, this.dayForegroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dayBackgroundColor, this.dayBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dayOverlayColor, this.dayOverlayColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dayShape, this.dayShape))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).todayForegroundColor, this.todayForegroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).todayBackgroundColor, this.todayBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).todayBorder, this.todayBorder))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).yearStyle, this.yearStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).yearForegroundColor, this.yearForegroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).yearBackgroundColor, this.yearBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).yearOverlayColor, this.yearOverlayColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).yearShape, this.yearShape))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerBackgroundColor, this.rangePickerBackgroundColor))) && (((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerElevation == this.rangePickerElevation)) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerShadowColor, this.rangePickerShadowColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerSurfaceTintColor, this.rangePickerSurfaceTintColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerShape, this.rangePickerShape))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerHeaderBackgroundColor, this.rangePickerHeaderBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerHeaderForegroundColor, this.rangePickerHeaderForegroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerHeaderHeadlineStyle, this.rangePickerHeaderHeadlineStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangePickerHeaderHelpStyle, this.rangePickerHeaderHelpStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangeSelectionBackgroundColor, this.rangeSelectionBackgroundColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).rangeSelectionOverlayColor, this.rangeSelectionOverlayColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).dividerColor, this.dividerColor))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).inputDecorationTheme, this.inputDecorationTheme))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).cancelButtonStyle, this.cancelButtonStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).confirmButtonStyle, this.confirmButtonStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).locale, this.locale))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).toggleButtonTextStyle, this.toggleButtonTextStyle))) && (object.Equals(((DatePickerThemeData)((DatePickerThemeData)__other)).subHeaderForegroundColor, this.subHeaderForegroundColor)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("headerBackgroundColor", this.headerBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("headerForegroundColor", this.headerForegroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headerHeadlineStyle", this.headerHeadlineStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headerHelpStyle", this.headerHelpStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("weekDayStyle", this.weekdayStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("dayStyle", this.dayStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("dayForegroundColor", this.dayForegroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("dayBackgroundColor", this.dayBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("dayOverlayColor", this.dayOverlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>>("dayShape", this.dayShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("todayForegroundColor", this.todayForegroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("todayBackgroundColor", this.todayBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderSide?>("todayBorder", this.todayBorder, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("yearStyle", this.yearStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("yearForegroundColor", this.yearForegroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("yearBackgroundColor", this.yearBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("yearOverlayColor", this.yearOverlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>>("yearShape", this.yearShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangePickerBackgroundColor", this.rangePickerBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("rangePickerElevation", this.rangePickerElevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangePickerShadowColor", this.rangePickerShadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangePickerSurfaceTintColor", this.rangePickerSurfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("rangePickerShape", this.rangePickerShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangePickerHeaderBackgroundColor", this.rangePickerHeaderBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangePickerHeaderForegroundColor", this.rangePickerHeaderForegroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("rangePickerHeaderHeadlineStyle", this.rangePickerHeaderHeadlineStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("rangePickerHeaderHelpStyle", this.rangePickerHeaderHelpStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("rangeSelectionBackgroundColor", this.rangeSelectionBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("rangeSelectionOverlayColor", this.rangeSelectionOverlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("dividerColor", this.dividerColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationTheme", this.inputDecorationTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("cancelButtonStyle", this.cancelButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("confirmButtonStyle", this.confirmButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Locale>("locale", this.locale, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("toggleButtonTextStyle", this.toggleButtonTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("subHeaderForegroundColor", this.subHeaderForegroundColor, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class DatePickerTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual DatePickerThemeData data { get; private set; } = default!;

    public DatePickerTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, DatePickerThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DatePickerThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return (DatePickerTheme.maybeOf(context) ?? Theme.of(context).datePickerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DatePickerThemeData? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DatePickerTheme>()?.data;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DatePickerThemeData defaults(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return (Theme.of(context).useMaterial3 ? new _DatePickerDefaultsM3__date_picker_theme(context) : new _DatePickerDefaultsM2__date_picker_theme(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DatePickerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DatePickerTheme)oldWidget).data)));
}

internal class _DatePickerDefaultsM2__date_picker_theme : DatePickerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = ((ThemeData)this._theme).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = ((ThemeData)this._theme).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    private bool __late__isDark_initialized;
    private bool __late__isDark = default!;
    internal virtual bool _isDark
    {
        get
        {
            if (!__late__isDark_initialized)
            {
                __late__isDark = (object.Equals(((ColorScheme)this._colors).brightness, Brightness.dark));
                __late__isDark_initialized = true;
            }
            return __late__isDark;
        }
    }

    internal _DatePickerDefaultsM2__date_picker_theme(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 24.0, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4.0))), dayShape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.CircleBorder()), yearShape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()), rangePickerElevation: 0.0, rangePickerShape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder())
    {
        this.context = context;
    }

    public override Color? headerBackgroundColor => (this._isDark ? ((ColorScheme)this._colors).surface : ((ColorScheme)this._colors).primary);
    public override Color? subHeaderForegroundColor => ((ColorScheme)this._colors).onSurface.withOpacity(0.6);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? toggleButtonTextStyle => ((TextTheme)this._textTheme).titleSmall?.apply(color: this.subHeaderForegroundColor);
    public override ButtonStyle? cancelButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
            return default!;
        }
    }
    public override ButtonStyle? confirmButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
            return default!;
        }
    }
    public override Color? headerForegroundColor => (this._isDark ? ((ColorScheme)this._colors).onSurface : ((ColorScheme)this._colors).onPrimary);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? headerHeadlineStyle => ((TextTheme)this._textTheme).headlineSmall;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? headerHelpStyle => ((TextTheme)this._textTheme).labelSmall;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle => ((TextTheme)this._textTheme).bodySmall?.apply(color: ((ColorScheme)this._colors).onSurface.withOpacity(0.6));
    public override global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle => ((TextTheme)this._textTheme).bodySmall;
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayForegroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).onPrimary);
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (((ColorScheme)this._colors).onSurface.withOpacity(0.38));
    }
}
return (((ColorScheme)this._colors).onSurface);
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayBackgroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).primary);
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayOverlayColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.38));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.12));
    }
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.12));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.12));
    }
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayForegroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).onPrimary);
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (((ColorScheme)this._colors).onSurface.withOpacity(0.38));
    }
}
return (((ColorScheme)this._colors).primary);
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayBackgroundColor => this.dayBackgroundColor;
    public override global::Doroti.Generated.Framework.Painting.BorderSide? todayBorder => new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((ColorScheme)this._colors).primary);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? yearStyle => ((TextTheme)this._textTheme).bodyLarge;
    public override Color? rangePickerBackgroundColor => ((ColorScheme)this._colors).surface;
    public override Color? rangePickerShadowColor => Colors.transparent;
    public override Color? rangePickerSurfaceTintColor => Colors.transparent;
    public override Color? rangePickerHeaderBackgroundColor => (this._isDark ? ((ColorScheme)this._colors).surface : ((ColorScheme)this._colors).primary);
    public override Color? rangePickerHeaderForegroundColor => (this._isDark ? ((ColorScheme)this._colors).onSurface : ((ColorScheme)this._colors).onPrimary);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHeadlineStyle => ((TextTheme)this._textTheme).headlineSmall;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHelpStyle => ((TextTheme)this._textTheme).labelSmall;
    public override Color? rangeSelectionBackgroundColor => ((ColorScheme)this._colors).primary.withOpacity(0.12);
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? rangeSelectionOverlayColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.38));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.12));
    }
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.12));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.12));
    }
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
}

internal class _DatePickerDefaultsM3__date_picker_theme : DatePickerThemeData
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(this.context);
                __late__theme_initialized = true;
            }
            return __late__theme;
        }
    }
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = ((ThemeData)this._theme).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = ((ThemeData)this._theme).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _DatePickerDefaultsM3__date_picker_theme(global::Doroti.Generated.Framework.Widgets.BuildContext context) : base(elevation: 6.0, shape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Generated.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))), dayShape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.CircleBorder()), yearShape: new global::Doroti.Generated.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Generated.Framework.Painting.OutlinedBorder>(new global::Doroti.Generated.Framework.Painting.StadiumBorder()), rangePickerElevation: 0.0, rangePickerShape: new global::Doroti.Generated.Framework.Painting.RoundedRectangleBorder())
    {
        this.context = context;
    }

    public override Color? backgroundColor => ((ColorScheme)this._colors).surfaceContainerHigh;
    public override Color? subHeaderForegroundColor => ((ColorScheme)this._colors).onSurface.withOpacity(0.6);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? toggleButtonTextStyle => ((TextTheme)this._textTheme).titleSmall?.apply(color: this.subHeaderForegroundColor);
    public override ButtonStyle? cancelButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
            return default!;
        }
    }
    public override ButtonStyle? confirmButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
            return default!;
        }
    }
    public override Color? shadowColor => Colors.transparent;
    public override Color? surfaceTintColor => Colors.transparent;
    public override Color? headerBackgroundColor => Colors.transparent;
    public override Color? headerForegroundColor => ((ColorScheme)this._colors).onSurfaceVariant;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? headerHeadlineStyle => ((TextTheme)this._textTheme).headlineLarge;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? headerHelpStyle => ((TextTheme)this._textTheme).labelLarge;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle => ((TextTheme)this._textTheme).bodyLarge?.apply(color: ((ColorScheme)this._colors).onSurface);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle => ((TextTheme)this._textTheme).bodyLarge;
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayForegroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).onPrimary);
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (((ColorScheme)this._colors).onSurface.withOpacity(0.38));
    }
}
return (((ColorScheme)this._colors).onSurface);
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayBackgroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).primary);
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? dayOverlayColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.1));
    }
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.1));
    }
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayForegroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).onPrimary);
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (((ColorScheme)this._colors).primary.withOpacity(0.38));
    }
}
return (((ColorScheme)this._colors).primary);
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? todayBackgroundColor => this.dayBackgroundColor;
    public override global::Doroti.Generated.Framework.Painting.BorderSide? todayBorder => new global::Doroti.Generated.Framework.Painting.BorderSide(color: ((ColorScheme)this._colors).primary);
    public override global::Doroti.Generated.Framework.Painting.TextStyle? yearStyle => ((TextTheme)this._textTheme).bodyLarge;
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearForegroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).onPrimary);
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.38));
    }
}
return (((ColorScheme)this._colors).onSurfaceVariant);
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearBackgroundColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    return (((ColorScheme)this._colors).primary);
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? yearOverlayColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.selected))
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onPrimary.withOpacity(0.1));
    }
}
else
{
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.1));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.08));
    }
    if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
    {
        return (((ColorScheme)this._colors).onSurfaceVariant.withOpacity(0.1));
    }
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override Color? rangePickerShadowColor => Colors.transparent;
    public override Color? rangePickerSurfaceTintColor => Colors.transparent;
    public override Color? rangeSelectionBackgroundColor => ((ColorScheme)this._colors).secondaryContainer;
    public override global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? rangeSelectionOverlayColor => WidgetStateProperty.resolveWith((states) => {
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.pressed))
{
    return (((ColorScheme)this._colors).onPrimaryContainer.withOpacity(0.1));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.hovered))
{
    return (((ColorScheme)this._colors).onPrimaryContainer.withOpacity(0.08));
}
if (states.Contains(global::Doroti.Generated.Framework.Widgets.WidgetState.focused))
{
    return (((ColorScheme)this._colors).onPrimaryContainer.withOpacity(0.1));
}
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
});
    public override Color? rangePickerHeaderBackgroundColor => Colors.transparent;
    public override Color? rangePickerHeaderForegroundColor => ((ColorScheme)this._colors).onSurfaceVariant;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHeadlineStyle => ((TextTheme)this._textTheme).titleLarge;
    public override global::Doroti.Generated.Framework.Painting.TextStyle? rangePickerHeaderHelpStyle => ((TextTheme)this._textTheme).titleSmall;
}
