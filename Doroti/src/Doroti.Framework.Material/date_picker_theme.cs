// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/date_picker_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DatePickerThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual Color? headerBackgroundColor { get; private set; }
    public virtual Color? headerForegroundColor { get; private set; }
    public virtual TextStyle? headerHeadlineStyle { get; private set; }
    public virtual TextStyle? headerHelpStyle { get; private set; }
    public virtual TextStyle? weekdayStyle { get; private set; }
    public virtual TextStyle? dayStyle { get; private set; }
    public virtual WidgetStateProperty<Color?>? dayForegroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? dayBackgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? dayOverlayColor { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? dayShape { get; private set; }
    public virtual WidgetStateProperty<Color?>? todayForegroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? todayBackgroundColor { get; private set; }
    public virtual BorderSide? todayBorder { get; private set; }
    public virtual TextStyle? yearStyle { get; private set; }
    public virtual WidgetStateProperty<Color?>? yearForegroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? yearBackgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? yearOverlayColor { get; private set; }
    public virtual WidgetStateProperty<OutlinedBorder?>? yearShape { get; private set; }
    public virtual Color? rangePickerBackgroundColor { get; private set; }
    public virtual double? rangePickerElevation { get; private set; }
    public virtual Color? rangePickerShadowColor { get; private set; }
    public virtual Color? rangePickerSurfaceTintColor { get; private set; }
    public virtual ShapeBorder? rangePickerShape { get; private set; }
    public virtual Color? rangePickerHeaderBackgroundColor { get; private set; }
    public virtual Color? rangePickerHeaderForegroundColor { get; private set; }
    public virtual TextStyle? rangePickerHeaderHeadlineStyle { get; private set; }
    public virtual TextStyle? rangePickerHeaderHelpStyle { get; private set; }
    public virtual Color? rangeSelectionBackgroundColor { get; private set; }
    public virtual WidgetStateProperty<Color?>? rangeSelectionOverlayColor { get; private set; }
    public virtual Color? dividerColor { get; private set; }
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual ButtonStyle? cancelButtonStyle { get; private set; }
    public virtual ButtonStyle? confirmButtonStyle { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual TextStyle? toggleButtonTextStyle { get; private set; }
    public virtual Color? subHeaderForegroundColor { get; private set; }

    public DatePickerThemeData(
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        Color? headerBackgroundColor = null,
        Color? headerForegroundColor = null,
        TextStyle? headerHeadlineStyle = null,
        TextStyle? headerHelpStyle = null,
        TextStyle? weekdayStyle = null,
        TextStyle? dayStyle = null,
        WidgetStateProperty<Color?>? dayForegroundColor = null,
        WidgetStateProperty<Color?>? dayBackgroundColor = null,
        WidgetStateProperty<Color?>? dayOverlayColor = null,
        WidgetStateProperty<OutlinedBorder?>? dayShape = null,
        WidgetStateProperty<Color?>? todayForegroundColor = null,
        WidgetStateProperty<Color?>? todayBackgroundColor = null,
        BorderSide? todayBorder = null,
        TextStyle? yearStyle = null,
        WidgetStateProperty<Color?>? yearForegroundColor = null,
        WidgetStateProperty<Color?>? yearBackgroundColor = null,
        WidgetStateProperty<Color?>? yearOverlayColor = null,
        WidgetStateProperty<OutlinedBorder?>? yearShape = null,
        Color? rangePickerBackgroundColor = null,
        double? rangePickerElevation = null,
        Color? rangePickerShadowColor = null,
        Color? rangePickerSurfaceTintColor = null,
        ShapeBorder? rangePickerShape = null,
        Color? rangePickerHeaderBackgroundColor = null,
        Color? rangePickerHeaderForegroundColor = null,
        TextStyle? rangePickerHeaderHeadlineStyle = null,
        TextStyle? rangePickerHeaderHelpStyle = null,
        Color? rangeSelectionBackgroundColor = null,
        WidgetStateProperty<Color?>? rangeSelectionOverlayColor = null,
        Color? dividerColor = null,
        object? inputDecorationTheme = null,
        ButtonStyle? cancelButtonStyle = null,
        ButtonStyle? confirmButtonStyle = null,
        Locale? locale = null,
        TextStyle? toggleButtonTextStyle = null,
        Color? subHeaderForegroundColor = null
    )
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
        _inputDecorationTheme = inputDecorationTheme;
        System.Diagnostics.Debug.Assert(
            (inputDecorationTheme is null)
                || (inputDecorationTheme is InputDecorationTheme)
                || (inputDecorationTheme is InputDecorationThemeData)
        );
    }

    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if (_inputDecorationTheme is null)
            {
                return null;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(
                _inputDecorationTheme
            );
        }
    }

    public virtual DatePickerThemeData copyWith(
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        Color? headerBackgroundColor = null,
        Color? headerForegroundColor = null,
        TextStyle? headerHeadlineStyle = null,
        TextStyle? headerHelpStyle = null,
        TextStyle? weekdayStyle = null,
        TextStyle? dayStyle = null,
        WidgetStateProperty<Color?>? dayForegroundColor = null,
        WidgetStateProperty<Color?>? dayBackgroundColor = null,
        WidgetStateProperty<Color?>? dayOverlayColor = null,
        WidgetStateProperty<OutlinedBorder?>? dayShape = null,
        WidgetStateProperty<Color?>? todayForegroundColor = null,
        WidgetStateProperty<Color?>? todayBackgroundColor = null,
        BorderSide? todayBorder = null,
        TextStyle? yearStyle = null,
        WidgetStateProperty<Color?>? yearForegroundColor = null,
        WidgetStateProperty<Color?>? yearBackgroundColor = null,
        WidgetStateProperty<Color?>? yearOverlayColor = null,
        WidgetStateProperty<OutlinedBorder?>? yearShape = null,
        Color? rangePickerBackgroundColor = null,
        double? rangePickerElevation = null,
        Color? rangePickerShadowColor = null,
        Color? rangePickerSurfaceTintColor = null,
        ShapeBorder? rangePickerShape = null,
        Color? rangePickerHeaderBackgroundColor = null,
        Color? rangePickerHeaderForegroundColor = null,
        TextStyle? rangePickerHeaderHeadlineStyle = null,
        TextStyle? rangePickerHeaderHelpStyle = null,
        Color? rangeSelectionBackgroundColor = null,
        WidgetStateProperty<Color?>? rangeSelectionOverlayColor = null,
        Color? dividerColor = null,
        InputDecorationTheme? inputDecorationTheme = null,
        ButtonStyle? cancelButtonStyle = null,
        ButtonStyle? confirmButtonStyle = null,
        Locale? locale = null,
        TextStyle? toggleButtonTextStyle = null,
        Color? subHeaderForegroundColor = null
    )
    {
        return new DatePickerThemeData(
            backgroundColor: backgroundColor ?? this.backgroundColor,
            elevation: elevation ?? this.elevation,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shape: shape ?? this.shape,
            headerBackgroundColor: headerBackgroundColor ?? this.headerBackgroundColor,
            headerForegroundColor: headerForegroundColor ?? this.headerForegroundColor,
            headerHeadlineStyle: headerHeadlineStyle ?? this.headerHeadlineStyle,
            headerHelpStyle: headerHelpStyle ?? this.headerHelpStyle,
            weekdayStyle: weekdayStyle ?? this.weekdayStyle,
            dayStyle: dayStyle ?? this.dayStyle,
            dayForegroundColor: dayForegroundColor ?? this.dayForegroundColor,
            dayBackgroundColor: dayBackgroundColor ?? this.dayBackgroundColor,
            dayOverlayColor: dayOverlayColor ?? this.dayOverlayColor,
            dayShape: dayShape ?? this.dayShape,
            todayForegroundColor: todayForegroundColor ?? this.todayForegroundColor,
            todayBackgroundColor: todayBackgroundColor ?? this.todayBackgroundColor,
            todayBorder: todayBorder ?? this.todayBorder,
            yearStyle: yearStyle ?? this.yearStyle,
            yearForegroundColor: yearForegroundColor ?? this.yearForegroundColor,
            yearBackgroundColor: yearBackgroundColor ?? this.yearBackgroundColor,
            yearOverlayColor: yearOverlayColor ?? this.yearOverlayColor,
            yearShape: yearShape ?? this.yearShape,
            rangePickerBackgroundColor: rangePickerBackgroundColor
                ?? this.rangePickerBackgroundColor,
            rangePickerElevation: rangePickerElevation ?? this.rangePickerElevation,
            rangePickerShadowColor: rangePickerShadowColor ?? this.rangePickerShadowColor,
            rangePickerSurfaceTintColor: rangePickerSurfaceTintColor
                ?? this.rangePickerSurfaceTintColor,
            rangePickerShape: rangePickerShape ?? this.rangePickerShape,
            rangePickerHeaderBackgroundColor: rangePickerHeaderBackgroundColor
                ?? this.rangePickerHeaderBackgroundColor,
            rangePickerHeaderForegroundColor: rangePickerHeaderForegroundColor
                ?? this.rangePickerHeaderForegroundColor,
            rangePickerHeaderHeadlineStyle: rangePickerHeaderHeadlineStyle
                ?? this.rangePickerHeaderHeadlineStyle,
            rangePickerHeaderHelpStyle: rangePickerHeaderHelpStyle
                ?? this.rangePickerHeaderHelpStyle,
            rangeSelectionBackgroundColor: rangeSelectionBackgroundColor
                ?? this.rangeSelectionBackgroundColor,
            rangeSelectionOverlayColor: rangeSelectionOverlayColor
                ?? this.rangeSelectionOverlayColor,
            dividerColor: dividerColor ?? this.dividerColor,
            inputDecorationTheme: DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(
                (object?)inputDecorationTheme ?? this.inputDecorationTheme
            ),
            cancelButtonStyle: cancelButtonStyle ?? this.cancelButtonStyle,
            confirmButtonStyle: confirmButtonStyle ?? this.confirmButtonStyle,
            locale: locale ?? this.locale,
            toggleButtonTextStyle: toggleButtonTextStyle ?? this.toggleButtonTextStyle,
            subHeaderForegroundColor: subHeaderForegroundColor ?? this.subHeaderForegroundColor
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DatePickerThemeData lerp(DatePickerThemeData? a, DatePickerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DatePickerThemeData(
            backgroundColor: DorotiUiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            elevation: DorotiUiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shadowColor: DorotiUiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: DorotiUiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
            headerBackgroundColor: DorotiUiLibrary.Color.lerp(
                a?.headerBackgroundColor,
                b?.headerBackgroundColor,
                t
            ),
            headerForegroundColor: DorotiUiLibrary.Color.lerp(
                a?.headerForegroundColor,
                b?.headerForegroundColor,
                t
            ),
            headerHeadlineStyle: TextStyle.lerp(a?.headerHeadlineStyle, b?.headerHeadlineStyle, t),
            headerHelpStyle: TextStyle.lerp(a?.headerHelpStyle, b?.headerHelpStyle, t),
            weekdayStyle: TextStyle.lerp(a?.weekdayStyle, b?.weekdayStyle, t),
            dayStyle: TextStyle.lerp(a?.dayStyle, b?.dayStyle, t),
            dayForegroundColor: WidgetStateProperty.lerp(
                a?.dayForegroundColor,
                b?.dayForegroundColor,
                t,
                Color.lerp
            ),
            dayBackgroundColor: WidgetStateProperty.lerp(
                a?.dayBackgroundColor,
                b?.dayBackgroundColor,
                t,
                Color.lerp
            ),
            dayOverlayColor: WidgetStateProperty.lerp(
                a?.dayOverlayColor,
                b?.dayOverlayColor,
                t,
                Color.lerp
            ),
            dayShape: WidgetStateProperty.lerp(a?.dayShape, b?.dayShape, t, OutlinedBorder.lerp),
            todayForegroundColor: WidgetStateProperty.lerp(
                a?.todayForegroundColor,
                b?.todayForegroundColor,
                t,
                Color.lerp
            ),
            todayBackgroundColor: WidgetStateProperty.lerp(
                a?.todayBackgroundColor,
                b?.todayBackgroundColor,
                t,
                Color.lerp
            ),
            todayBorder: _lerpBorderSide(a?.todayBorder, b?.todayBorder, t),
            yearStyle: TextStyle.lerp(a?.yearStyle, b?.yearStyle, t),
            yearForegroundColor: WidgetStateProperty.lerp(
                a?.yearForegroundColor,
                b?.yearForegroundColor,
                t,
                Color.lerp
            ),
            yearBackgroundColor: WidgetStateProperty.lerp(
                a?.yearBackgroundColor,
                b?.yearBackgroundColor,
                t,
                Color.lerp
            ),
            yearOverlayColor: WidgetStateProperty.lerp(
                a?.yearOverlayColor,
                b?.yearOverlayColor,
                t,
                Color.lerp
            ),
            yearShape: WidgetStateProperty.lerp(a?.yearShape, b?.yearShape, t, OutlinedBorder.lerp),
            rangePickerBackgroundColor: DorotiUiLibrary.Color.lerp(
                a?.rangePickerBackgroundColor,
                b?.rangePickerBackgroundColor,
                t
            ),
            rangePickerElevation: DorotiUiLibrary.lerpDouble(
                a?.rangePickerElevation,
                b?.rangePickerElevation,
                t
            ),
            rangePickerShadowColor: DorotiUiLibrary.Color.lerp(
                a?.rangePickerShadowColor,
                b?.rangePickerShadowColor,
                t
            ),
            rangePickerSurfaceTintColor: DorotiUiLibrary.Color.lerp(
                a?.rangePickerSurfaceTintColor,
                b?.rangePickerSurfaceTintColor,
                t
            ),
            rangePickerShape: ShapeBorder.lerp(a?.rangePickerShape, b?.rangePickerShape, t),
            rangePickerHeaderBackgroundColor: DorotiUiLibrary.Color.lerp(
                a?.rangePickerHeaderBackgroundColor,
                b?.rangePickerHeaderBackgroundColor,
                t
            ),
            rangePickerHeaderForegroundColor: DorotiUiLibrary.Color.lerp(
                a?.rangePickerHeaderForegroundColor,
                b?.rangePickerHeaderForegroundColor,
                t
            ),
            rangePickerHeaderHeadlineStyle: TextStyle.lerp(
                a?.rangePickerHeaderHeadlineStyle,
                b?.rangePickerHeaderHeadlineStyle,
                t
            ),
            rangePickerHeaderHelpStyle: TextStyle.lerp(
                a?.rangePickerHeaderHelpStyle,
                b?.rangePickerHeaderHelpStyle,
                t
            ),
            rangeSelectionBackgroundColor: DorotiUiLibrary.Color.lerp(
                a?.rangeSelectionBackgroundColor,
                b?.rangeSelectionBackgroundColor,
                t
            ),
            rangeSelectionOverlayColor: WidgetStateProperty.lerp(
                a?.rangeSelectionOverlayColor,
                b?.rangeSelectionOverlayColor,
                t,
                Color.lerp
            ),
            dividerColor: DorotiUiLibrary.Color.lerp(a?.dividerColor, b?.dividerColor, t),
            inputDecorationTheme: (t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme,
            cancelButtonStyle: ButtonStyle.lerp(a?.cancelButtonStyle, b?.cancelButtonStyle, t),
            confirmButtonStyle: ButtonStyle.lerp(a?.confirmButtonStyle, b?.confirmButtonStyle, t),
            locale: (t < 0.5) ? a?.locale : b?.locale,
            toggleButtonTextStyle: TextStyle.lerp(
                a?.toggleButtonTextStyle,
                b?.toggleButtonTextStyle,
                t
            ),
            subHeaderForegroundColor: DorotiUiLibrary.Color.lerp(
                a?.subHeaderForegroundColor,
                b?.subHeaderForegroundColor,
                t
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static BorderSide? _lerpBorderSide(BorderSide? a, BorderSide? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return (BorderSide?)
                BorderSide.lerp(new BorderSide(width: 0, color: b!.color.withAlpha(0L)), b, t);
        }
        return (BorderSide?)
            BorderSide.lerp(a, new BorderSide(width: 0, color: a.color.withAlpha(0L)), t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHashAll(
                new List<object?>
                {
                    backgroundColor,
                    elevation,
                    shadowColor,
                    surfaceTintColor,
                    shape,
                    headerBackgroundColor,
                    headerForegroundColor,
                    headerHeadlineStyle,
                    headerHelpStyle,
                    weekdayStyle,
                    dayStyle,
                    dayForegroundColor,
                    dayBackgroundColor,
                    dayOverlayColor,
                    dayShape,
                    todayForegroundColor,
                    todayBackgroundColor,
                    todayBorder,
                    yearStyle,
                    yearForegroundColor,
                    yearBackgroundColor,
                    yearOverlayColor,
                    yearShape,
                    rangePickerBackgroundColor,
                    rangePickerElevation,
                    rangePickerShadowColor,
                    rangePickerSurfaceTintColor,
                    rangePickerShape,
                    rangePickerHeaderBackgroundColor,
                    rangePickerHeaderForegroundColor,
                    rangePickerHeaderHeadlineStyle,
                    rangePickerHeaderHelpStyle,
                    rangeSelectionBackgroundColor,
                    rangeSelectionOverlayColor,
                    dividerColor,
                    inputDecorationTheme,
                    cancelButtonStyle,
                    confirmButtonStyle,
                    locale,
                    toggleButtonTextStyle,
                    subHeaderForegroundColor,
                }
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as DatePickerThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is DatePickerThemeData)
            && Equals(__other.backgroundColor, backgroundColor)
            && (__other.elevation == elevation)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.shape, shape)
            && Equals(__other.headerBackgroundColor, headerBackgroundColor)
            && Equals(__other.headerForegroundColor, headerForegroundColor)
            && Equals(__other.headerHeadlineStyle, headerHeadlineStyle)
            && Equals(__other.headerHelpStyle, headerHelpStyle)
            && Equals(__other.weekdayStyle, weekdayStyle)
            && Equals(__other.dayStyle, dayStyle)
            && Equals(__other.dayForegroundColor, dayForegroundColor)
            && Equals(__other.dayBackgroundColor, dayBackgroundColor)
            && Equals(__other.dayOverlayColor, dayOverlayColor)
            && Equals(__other.dayShape, dayShape)
            && Equals(__other.todayForegroundColor, todayForegroundColor)
            && Equals(__other.todayBackgroundColor, todayBackgroundColor)
            && Equals(__other.todayBorder, todayBorder)
            && Equals(__other.yearStyle, yearStyle)
            && Equals(__other.yearForegroundColor, yearForegroundColor)
            && Equals(__other.yearBackgroundColor, yearBackgroundColor)
            && Equals(__other.yearOverlayColor, yearOverlayColor)
            && Equals(__other.yearShape, yearShape)
            && Equals(__other.rangePickerBackgroundColor, rangePickerBackgroundColor)
            && (__other.rangePickerElevation == rangePickerElevation)
            && Equals(__other.rangePickerShadowColor, rangePickerShadowColor)
            && Equals(__other.rangePickerSurfaceTintColor, rangePickerSurfaceTintColor)
            && Equals(__other.rangePickerShape, rangePickerShape)
            && Equals(__other.rangePickerHeaderBackgroundColor, rangePickerHeaderBackgroundColor)
            && Equals(__other.rangePickerHeaderForegroundColor, rangePickerHeaderForegroundColor)
            && Equals(__other.rangePickerHeaderHeadlineStyle, rangePickerHeaderHeadlineStyle)
            && Equals(__other.rangePickerHeaderHelpStyle, rangePickerHeaderHelpStyle)
            && Equals(__other.rangeSelectionBackgroundColor, rangeSelectionBackgroundColor)
            && Equals(__other.rangeSelectionOverlayColor, rangeSelectionOverlayColor)
            && Equals(__other.dividerColor, dividerColor)
            && Equals(__other.inputDecorationTheme, inputDecorationTheme)
            && Equals(__other.cancelButtonStyle, cancelButtonStyle)
            && Equals(__other.confirmButtonStyle, confirmButtonStyle)
            && Equals(__other.locale, locale)
            && Equals(__other.toggleButtonTextStyle, toggleButtonTextStyle)
            && Equals(__other.subHeaderForegroundColor, subHeaderForegroundColor);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new ColorProperty("headerBackgroundColor", headerBackgroundColor, defaultValue: null)
        );
        properties.add(
            new ColorProperty("headerForegroundColor", headerForegroundColor, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headerHeadlineStyle",
                headerHeadlineStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headerHelpStyle",
                headerHelpStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>("weekDayStyle", weekdayStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>("dayStyle", dayStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "dayForegroundColor",
                dayForegroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "dayBackgroundColor",
                dayBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "dayOverlayColor",
                dayOverlayColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<OutlinedBorder?>>(
                "dayShape",
                dayShape,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "todayForegroundColor",
                todayForegroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "todayBackgroundColor",
                todayBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<BorderSide?>("todayBorder", todayBorder, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>("yearStyle", yearStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "yearForegroundColor",
                yearForegroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "yearBackgroundColor",
                yearBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "yearOverlayColor",
                yearOverlayColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<OutlinedBorder?>>(
                "yearShape",
                yearShape,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty(
                "rangePickerBackgroundColor",
                rangePickerBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DoubleProperty("rangePickerElevation", rangePickerElevation, defaultValue: null)
        );
        properties.add(
            new ColorProperty("rangePickerShadowColor", rangePickerShadowColor, defaultValue: null)
        );
        properties.add(
            new ColorProperty(
                "rangePickerSurfaceTintColor",
                rangePickerSurfaceTintColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ShapeBorder>(
                "rangePickerShape",
                rangePickerShape,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty(
                "rangePickerHeaderBackgroundColor",
                rangePickerHeaderBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty(
                "rangePickerHeaderForegroundColor",
                rangePickerHeaderForegroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "rangePickerHeaderHeadlineStyle",
                rangePickerHeaderHeadlineStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "rangePickerHeaderHelpStyle",
                rangePickerHeaderHelpStyle,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty(
                "rangeSelectionBackgroundColor",
                rangeSelectionBackgroundColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "rangeSelectionOverlayColor",
                rangeSelectionOverlayColor,
                defaultValue: null
            )
        );
        properties.add(new ColorProperty("dividerColor", dividerColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<InputDecorationThemeData>(
                "inputDecorationTheme",
                inputDecorationTheme,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ButtonStyle>(
                "cancelButtonStyle",
                cancelButtonStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<ButtonStyle>(
                "confirmButtonStyle",
                confirmButtonStyle,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<Locale>("locale", locale, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "toggleButtonTextStyle",
                toggleButtonTextStyle,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty(
                "subHeaderForegroundColor",
                subHeaderForegroundColor,
                defaultValue: null
            )
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

public class DatePickerTheme : InheritedTheme
{
    public virtual DatePickerThemeData data { get; private set; } = default!;

    public DatePickerTheme(
        Key? key = null,
        DatePickerThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DatePickerThemeData of(BuildContext context)
    {
        return maybeOf(context) ?? Theme.of(context).datePickerTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DatePickerThemeData? maybeOf(BuildContext context)
    {
        return context.dependOnInheritedWidgetOfExactType<DatePickerTheme>()?.data;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DatePickerThemeData defaults(BuildContext context)
    {
        return new _DatePickerDefaultsM3__date_picker_theme(context);
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DatePickerTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DatePickerTheme)oldWidget).data));
}

internal class _DatePickerDefaultsM3__date_picker_theme : DatePickerThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    private bool __late__theme_initialized;
    private ThemeData __late__theme = default!;
    internal virtual ThemeData _theme
    {
        get
        {
            if (!__late__theme_initialized)
            {
                __late__theme = Theme.of(context);
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
                __late__colors = _theme.colorScheme;
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
                __late__textTheme = _theme.textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _DatePickerDefaultsM3__date_picker_theme(BuildContext context)
        : base(
            elevation: 6.0,
            shape: new RoundedRectangleBorder(
                borderRadius: BorderRadius.CreateAll(Radius.circular(28.0))
            ),
            dayShape: new WidgetStatePropertyAll<OutlinedBorder>(new CircleBorder()),
            yearShape: new WidgetStatePropertyAll<OutlinedBorder>(new StadiumBorder()),
            rangePickerElevation: 0.0,
            rangePickerShape: new RoundedRectangleBorder()
        )
    {
        this.context = context;
    }

    public override Color? backgroundColor => _colors.surfaceContainerHigh;
    public override Color? subHeaderForegroundColor => _colors.onSurface.withOpacity(0.6);
    public override TextStyle? toggleButtonTextStyle =>
        _textTheme.titleSmall?.apply(color: subHeaderForegroundColor);
    public override ButtonStyle? cancelButtonStyle
    {
        get { return TextButton.styleFrom(); }
    }
    public override ButtonStyle? confirmButtonStyle
    {
        get { return TextButton.styleFrom(); }
    }
    public override Color? shadowColor => Colors.transparent;
    public override Color? surfaceTintColor => Colors.transparent;
    public override Color? headerBackgroundColor => Colors.transparent;
    public override Color? headerForegroundColor => _colors.onSurfaceVariant;
    public override TextStyle? headerHeadlineStyle => _textTheme.headlineLarge;
    public override TextStyle? headerHelpStyle => _textTheme.labelLarge;
    public override TextStyle? weekdayStyle =>
        _textTheme.bodyLarge?.apply(color: _colors.onSurface);
    public override TextStyle? dayStyle => _textTheme.bodyLarge;
    public override WidgetStateProperty<Color?>? dayForegroundColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.onPrimary;
                }
                else
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return _colors.onSurface.withOpacity(0.38);
                    }
                }
                return _colors.onSurface;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? dayBackgroundColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.primary;
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? dayOverlayColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onPrimary.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onPrimary.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onPrimary.withOpacity(0.1);
                    }
                }
                else
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.1);
                    }
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? todayForegroundColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.onPrimary;
                }
                else
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return _colors.primary.withOpacity(0.38);
                    }
                }
                return _colors.primary;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? todayBackgroundColor => dayBackgroundColor;
    public override BorderSide? todayBorder => new BorderSide(color: _colors.primary);
    public override TextStyle? yearStyle => _textTheme.bodyLarge;
    public override WidgetStateProperty<Color?>? yearForegroundColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.onPrimary;
                }
                else
                {
                    if (states.Contains(WidgetState.disabled))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.38);
                    }
                }
                return _colors.onSurfaceVariant;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? yearBackgroundColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.primary;
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override WidgetStateProperty<Color?>? yearOverlayColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onPrimary.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onPrimary.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onPrimary.withOpacity(0.1);
                    }
                }
                else
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.1);
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.08);
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurfaceVariant.withOpacity(0.1);
                    }
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override Color? rangePickerShadowColor => Colors.transparent;
    public override Color? rangePickerSurfaceTintColor => Colors.transparent;
    public override Color? rangeSelectionBackgroundColor => _colors.secondaryContainer;
    public override WidgetStateProperty<Color?>? rangeSelectionOverlayColor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.onPrimaryContainer.withOpacity(0.1);
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onPrimaryContainer.withOpacity(0.08);
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onPrimaryContainer.withOpacity(0.1);
                }
                return null;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
    public override Color? rangePickerHeaderBackgroundColor => Colors.transparent;
    public override Color? rangePickerHeaderForegroundColor => _colors.onSurfaceVariant;
    public override TextStyle? rangePickerHeaderHeadlineStyle => _textTheme.titleLarge;
    public override TextStyle? rangePickerHeaderHelpStyle => _textTheme.titleSmall;
}
