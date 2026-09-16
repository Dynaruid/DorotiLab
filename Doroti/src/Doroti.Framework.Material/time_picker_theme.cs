// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/time_picker_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TimePickerThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual ButtonStyle? cancelButtonStyle { get; private set; }
    public virtual ButtonStyle? confirmButtonStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? dayPeriodBorderSide { get; private set; }
    internal virtual Color? _dayPeriodColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? dayPeriodShape { get; private set; }
    public virtual Color? dayPeriodTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? dayPeriodTextStyle { get; private set; }
    public virtual Color? dialBackgroundColor { get; private set; }
    public virtual Color? dialHandColor { get; private set; }
    public virtual Color? dialTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? dialTextStyle { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? entryModeIconColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? helpTextStyle { get; private set; }
    public virtual Color? hourMinuteColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? hourMinuteShape { get; private set; }
    public virtual Color? hourMinuteTextColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? hourMinuteTextStyle { get; private set; }
    internal virtual object? _inputDecorationTheme { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? timeSelectorSeparatorColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? timeSelectorSeparatorTextStyle { get; private set; }

    public TimePickerThemeData(Color? backgroundColor = null, ButtonStyle? cancelButtonStyle = null, ButtonStyle? confirmButtonStyle = null, global::Doroti.Framework.Painting.BorderSide? dayPeriodBorderSide = null, Color? dayPeriodColor = null, global::Doroti.Framework.Painting.OutlinedBorder? dayPeriodShape = null, Color? dayPeriodTextColor = null, global::Doroti.Framework.Painting.TextStyle? dayPeriodTextStyle = null, Color? dialBackgroundColor = null, Color? dialHandColor = null, Color? dialTextColor = null, global::Doroti.Framework.Painting.TextStyle? dialTextStyle = null, double? elevation = null, Color? entryModeIconColor = null, global::Doroti.Framework.Painting.TextStyle? helpTextStyle = null, Color? hourMinuteColor = null, global::Doroti.Framework.Painting.ShapeBorder? hourMinuteShape = null, Color? hourMinuteTextColor = null, global::Doroti.Framework.Painting.TextStyle? hourMinuteTextStyle = null, object? inputDecorationTheme = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? timeSelectorSeparatorColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? timeSelectorSeparatorTextStyle = null)
    {
        this.backgroundColor = backgroundColor;
        this.cancelButtonStyle = cancelButtonStyle;
        this.confirmButtonStyle = confirmButtonStyle;
        this.dayPeriodBorderSide = dayPeriodBorderSide;
        this.dayPeriodShape = dayPeriodShape;
        this.dayPeriodTextColor = dayPeriodTextColor;
        this.dayPeriodTextStyle = dayPeriodTextStyle;
        this.dialBackgroundColor = dialBackgroundColor;
        this.dialHandColor = dialHandColor;
        this.dialTextColor = dialTextColor;
        this.dialTextStyle = dialTextStyle;
        this.elevation = elevation;
        this.entryModeIconColor = entryModeIconColor;
        this.helpTextStyle = helpTextStyle;
        this.hourMinuteColor = hourMinuteColor;
        this.hourMinuteShape = hourMinuteShape;
        this.hourMinuteTextColor = hourMinuteTextColor;
        this.hourMinuteTextStyle = hourMinuteTextStyle;
        this.padding = padding;
        this.shape = shape;
        this.timeSelectorSeparatorColor = timeSelectorSeparatorColor;
        this.timeSelectorSeparatorTextStyle = timeSelectorSeparatorTextStyle;
        _inputDecorationTheme = inputDecorationTheme;
        _dayPeriodColor = dayPeriodColor;
        System.Diagnostics.Debug.Assert((inputDecorationTheme is null) || (inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData));
    }

    public virtual global::Doroti.Ui.Color? dayPeriodColor
    {
        get
        {
            if ((_dayPeriodColor is null) || (_dayPeriodColor is global::Doroti.Framework.Widgets.WidgetStateColor))
            {
                return _dayPeriodColor;
            }
            return (Color?)WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _dayPeriodColor;
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public virtual InputDecorationThemeData? inputDecorationTheme
    {
        get
        {
            if (_inputDecorationTheme is null)
            {
                return null;
            }
            return DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>(_inputDecorationTheme);
        }
    }
    public virtual TimePickerThemeData copyWith(Color? backgroundColor = null, ButtonStyle? cancelButtonStyle = null, ButtonStyle? confirmButtonStyle = null, ButtonStyle? dayPeriodButtonStyle = null, global::Doroti.Framework.Painting.BorderSide? dayPeriodBorderSide = null, Color? dayPeriodColor = null, global::Doroti.Framework.Painting.OutlinedBorder? dayPeriodShape = null, Color? dayPeriodTextColor = null, global::Doroti.Framework.Painting.TextStyle? dayPeriodTextStyle = null, Color? dialBackgroundColor = null, Color? dialHandColor = null, Color? dialTextColor = null, global::Doroti.Framework.Painting.TextStyle? dialTextStyle = null, double? elevation = null, Color? entryModeIconColor = null, global::Doroti.Framework.Painting.TextStyle? helpTextStyle = null, Color? hourMinuteColor = null, global::Doroti.Framework.Painting.ShapeBorder? hourMinuteShape = null, Color? hourMinuteTextColor = null, global::Doroti.Framework.Painting.TextStyle? hourMinuteTextStyle = null, InputDecorationTheme? inputDecorationTheme = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? timeSelectorSeparatorColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? timeSelectorSeparatorTextStyle = null)
    {
        return new TimePickerThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, cancelButtonStyle: cancelButtonStyle ?? this.cancelButtonStyle, confirmButtonStyle: confirmButtonStyle ?? this.confirmButtonStyle, dayPeriodBorderSide: dayPeriodBorderSide ?? this.dayPeriodBorderSide, dayPeriodColor: dayPeriodColor ?? this.dayPeriodColor, dayPeriodShape: dayPeriodShape ?? this.dayPeriodShape, dayPeriodTextColor: dayPeriodTextColor ?? this.dayPeriodTextColor, dayPeriodTextStyle: dayPeriodTextStyle ?? this.dayPeriodTextStyle, dialBackgroundColor: dialBackgroundColor ?? this.dialBackgroundColor, dialHandColor: dialHandColor ?? this.dialHandColor, dialTextColor: dialTextColor ?? this.dialTextColor, dialTextStyle: dialTextStyle ?? this.dialTextStyle, elevation: elevation ?? this.elevation, entryModeIconColor: entryModeIconColor ?? this.entryModeIconColor, helpTextStyle: helpTextStyle ?? this.helpTextStyle, hourMinuteColor: hourMinuteColor ?? this.hourMinuteColor, hourMinuteShape: hourMinuteShape ?? this.hourMinuteShape, hourMinuteTextColor: hourMinuteTextColor ?? this.hourMinuteTextColor, hourMinuteTextStyle: hourMinuteTextStyle ?? this.hourMinuteTextStyle, inputDecorationTheme: DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>((object?)inputDecorationTheme ?? this.inputDecorationTheme), padding: padding ?? this.padding, shape: shape ?? this.shape, timeSelectorSeparatorColor: timeSelectorSeparatorColor ?? this.timeSelectorSeparatorColor, timeSelectorSeparatorTextStyle: timeSelectorSeparatorTextStyle ?? this.timeSelectorSeparatorTextStyle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TimePickerThemeData lerp(TimePickerThemeData? a, TimePickerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        global::Doroti.Framework.Painting.BorderSide? lerpedBorderSide = default!;
        if ((a?.dayPeriodBorderSide is null) && (b?.dayPeriodBorderSide is null))
        {
            lerpedBorderSide = null;
        }
        else
        {
            if (a?.dayPeriodBorderSide is null)
            {
                lerpedBorderSide = b?.dayPeriodBorderSide;
            }
            else
            {
                if (b?.dayPeriodBorderSide is null)
                {
                    lerpedBorderSide = a?.dayPeriodBorderSide;
                }
                else
                {
                    lerpedBorderSide = BorderSide.lerp(a!.dayPeriodBorderSide!, b!.dayPeriodBorderSide!, t);
                }
            }
        }
        return new TimePickerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), cancelButtonStyle: ButtonStyle.lerp(a?.cancelButtonStyle, b?.cancelButtonStyle, t), confirmButtonStyle: ButtonStyle.lerp(a?.confirmButtonStyle, b?.confirmButtonStyle, t), dayPeriodBorderSide: lerpedBorderSide, dayPeriodColor: Dart_uiLibrary.Color.lerp(a?.dayPeriodColor, b?.dayPeriodColor, t), dayPeriodShape: ((global::Doroti.Framework.Painting.OutlinedBorder?)ShapeBorder.lerp(a?.dayPeriodShape, b?.dayPeriodShape, t))!, dayPeriodTextColor: Dart_uiLibrary.Color.lerp(a?.dayPeriodTextColor, b?.dayPeriodTextColor, t), dayPeriodTextStyle: TextStyle.lerp(a?.dayPeriodTextStyle, b?.dayPeriodTextStyle, t), dialBackgroundColor: Dart_uiLibrary.Color.lerp(a?.dialBackgroundColor, b?.dialBackgroundColor, t), dialHandColor: Dart_uiLibrary.Color.lerp(a?.dialHandColor, b?.dialHandColor, t), dialTextColor: Dart_uiLibrary.Color.lerp(a?.dialTextColor, b?.dialTextColor, t), dialTextStyle: TextStyle.lerp(a?.dialTextStyle, b?.dialTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), entryModeIconColor: Dart_uiLibrary.Color.lerp(a?.entryModeIconColor, b?.entryModeIconColor, t), helpTextStyle: TextStyle.lerp(a?.helpTextStyle, b?.helpTextStyle, t), hourMinuteColor: Dart_uiLibrary.Color.lerp(a?.hourMinuteColor, b?.hourMinuteColor, t), hourMinuteShape: ShapeBorder.lerp(a?.hourMinuteShape, b?.hourMinuteShape, t), hourMinuteTextColor: Dart_uiLibrary.Color.lerp(a?.hourMinuteTextColor, b?.hourMinuteTextColor, t), hourMinuteTextStyle: TextStyle.lerp(a?.hourMinuteTextStyle, b?.hourMinuteTextStyle, t), inputDecorationTheme: (t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme, padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), timeSelectorSeparatorColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.timeSelectorSeparatorColor, b?.timeSelectorSeparatorColor, t, Color.lerp), timeSelectorSeparatorTextStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.timeSelectorSeparatorTextStyle, b?.timeSelectorSeparatorTextStyle, t, TextStyle.lerp));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { backgroundColor, cancelButtonStyle, confirmButtonStyle, dayPeriodBorderSide, dayPeriodColor, dayPeriodShape, dayPeriodTextColor, dayPeriodTextStyle, dialBackgroundColor, dialHandColor, dialTextColor, dialTextStyle, elevation, entryModeIconColor, helpTextStyle, hourMinuteColor, hourMinuteShape, hourMinuteTextColor, hourMinuteTextStyle, inputDecorationTheme, padding, shape, timeSelectorSeparatorColor, timeSelectorSeparatorTextStyle }));
    public override bool Equals(object? other)
    {
        var __other = other as TimePickerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is TimePickerThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.cancelButtonStyle, cancelButtonStyle) && Equals(__other.confirmButtonStyle, confirmButtonStyle) && Equals(__other.dayPeriodBorderSide, dayPeriodBorderSide) && Equals(__other.dayPeriodColor, dayPeriodColor) && Equals(__other.dayPeriodShape, dayPeriodShape) && Equals(__other.dayPeriodTextColor, dayPeriodTextColor) && Equals(__other.dayPeriodTextStyle, dayPeriodTextStyle) && Equals(__other.dialBackgroundColor, dialBackgroundColor) && Equals(__other.dialHandColor, dialHandColor) && Equals(__other.dialTextColor, dialTextColor) && Equals(__other.dialTextStyle, dialTextStyle) && (__other.elevation == elevation) && Equals(__other.entryModeIconColor, entryModeIconColor) && Equals(__other.helpTextStyle, helpTextStyle) && Equals(__other.hourMinuteColor, hourMinuteColor) && Equals(__other.hourMinuteShape, hourMinuteShape) && Equals(__other.hourMinuteTextColor, hourMinuteTextColor) && Equals(__other.hourMinuteTextStyle, hourMinuteTextStyle) && Equals(__other.inputDecorationTheme, inputDecorationTheme) && Equals(__other.padding, padding) && Equals(__other.shape, shape) && Equals(__other.timeSelectorSeparatorColor, timeSelectorSeparatorColor) && Equals(__other.timeSelectorSeparatorTextStyle, timeSelectorSeparatorTextStyle);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("cancelButtonStyle", cancelButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("confirmButtonStyle", confirmButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("dayPeriodBorderSide", dayPeriodBorderSide, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dayPeriodColor", dayPeriodColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("dayPeriodShape", dayPeriodShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dayPeriodTextColor", dayPeriodTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("dayPeriodTextStyle", dayPeriodTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialBackgroundColor", dialBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialHandColor", dialHandColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialTextColor", dialTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("dialTextStyle", dialTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("entryModeIconColor", entryModeIconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("helpTextStyle", helpTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hourMinuteColor", hourMinuteColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("hourMinuteShape", hourMinuteShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hourMinuteTextColor", hourMinuteTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("hourMinuteTextStyle", hourMinuteTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationTheme", inputDecorationTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("timeSelectorSeparatorColor", timeSelectorSeparatorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("timeSelectorSeparatorTextStyle", timeSelectorSeparatorTextStyle, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TimePickerTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual TimePickerThemeData data { get; private set; } = default!;

    public TimePickerTheme(global::Doroti.Framework.Foundation.Key? key = null, TimePickerThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static TimePickerThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimePickerTheme? timePickerThemeLocal = context.dependOnInheritedWidgetOfExactType<TimePickerTheme>();
        return timePickerThemeLocal?.data ?? Theme.of(context).timePickerTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new TimePickerTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((TimePickerTheme)oldWidget).data));
}
