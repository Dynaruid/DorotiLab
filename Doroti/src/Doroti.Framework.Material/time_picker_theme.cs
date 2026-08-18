// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/time_picker_theme.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

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
        this._inputDecorationTheme = inputDecorationTheme;
        this._dayPeriodColor = dayPeriodColor;
        System.Diagnostics.Debug.Assert(((inputDecorationTheme is null) || (((inputDecorationTheme is InputDecorationTheme) || (inputDecorationTheme is InputDecorationThemeData)))));
    }

    public virtual global::Doroti.Ui.Color? dayPeriodColor
    {
        get
        {
            if (((this._dayPeriodColor is null) || (this._dayPeriodColor is global::Doroti.Framework.Widgets.WidgetStateColor)))
            {
                return this._dayPeriodColor;
            }
            return ((Color?)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return this._dayPeriodColor;
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
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
    public virtual TimePickerThemeData copyWith(Color? backgroundColor = null, ButtonStyle? cancelButtonStyle = null, ButtonStyle? confirmButtonStyle = null, ButtonStyle? dayPeriodButtonStyle = null, global::Doroti.Framework.Painting.BorderSide? dayPeriodBorderSide = null, Color? dayPeriodColor = null, global::Doroti.Framework.Painting.OutlinedBorder? dayPeriodShape = null, Color? dayPeriodTextColor = null, global::Doroti.Framework.Painting.TextStyle? dayPeriodTextStyle = null, Color? dialBackgroundColor = null, Color? dialHandColor = null, Color? dialTextColor = null, global::Doroti.Framework.Painting.TextStyle? dialTextStyle = null, double? elevation = null, Color? entryModeIconColor = null, global::Doroti.Framework.Painting.TextStyle? helpTextStyle = null, Color? hourMinuteColor = null, global::Doroti.Framework.Painting.ShapeBorder? hourMinuteShape = null, Color? hourMinuteTextColor = null, global::Doroti.Framework.Painting.TextStyle? hourMinuteTextStyle = null, InputDecorationTheme? inputDecorationTheme = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? timeSelectorSeparatorColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? timeSelectorSeparatorTextStyle = null)
    {
        return new TimePickerThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), cancelButtonStyle: (cancelButtonStyle ?? this.cancelButtonStyle), confirmButtonStyle: (confirmButtonStyle ?? this.confirmButtonStyle), dayPeriodBorderSide: (dayPeriodBorderSide ?? this.dayPeriodBorderSide), dayPeriodColor: (dayPeriodColor ?? this.dayPeriodColor), dayPeriodShape: (dayPeriodShape ?? this.dayPeriodShape), dayPeriodTextColor: (dayPeriodTextColor ?? this.dayPeriodTextColor), dayPeriodTextStyle: (dayPeriodTextStyle ?? this.dayPeriodTextStyle), dialBackgroundColor: (dialBackgroundColor ?? this.dialBackgroundColor), dialHandColor: (dialHandColor ?? this.dialHandColor), dialTextColor: (dialTextColor ?? this.dialTextColor), dialTextStyle: (dialTextStyle ?? this.dialTextStyle), elevation: (elevation ?? this.elevation), entryModeIconColor: (entryModeIconColor ?? this.entryModeIconColor), helpTextStyle: (helpTextStyle ?? this.helpTextStyle), hourMinuteColor: (hourMinuteColor ?? this.hourMinuteColor), hourMinuteShape: (hourMinuteShape ?? this.hourMinuteShape), hourMinuteTextColor: (hourMinuteTextColor ?? this.hourMinuteTextColor), hourMinuteTextStyle: (hourMinuteTextStyle ?? this.hourMinuteTextStyle), inputDecorationTheme: DartRuntimePrimitives.ConvertValue<InputDecorationThemeData>((object?)inputDecorationTheme ?? this.inputDecorationTheme), padding: (padding ?? this.padding), shape: (shape ?? this.shape), timeSelectorSeparatorColor: (timeSelectorSeparatorColor ?? this.timeSelectorSeparatorColor), timeSelectorSeparatorTextStyle: (timeSelectorSeparatorTextStyle ?? this.timeSelectorSeparatorTextStyle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TimePickerThemeData lerp(TimePickerThemeData? a, TimePickerThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        global::Doroti.Framework.Painting.BorderSide? lerpedBorderSide__15082 = default!;
        if (((a?.dayPeriodBorderSide is null) && (b?.dayPeriodBorderSide is null)))
        {
            lerpedBorderSide__15082 = null;
        }
        else
        {
            if ((a?.dayPeriodBorderSide is null))
            {
                lerpedBorderSide__15082 = b?.dayPeriodBorderSide;
            }
            else
            {
                if ((b?.dayPeriodBorderSide is null))
                {
                    lerpedBorderSide__15082 = a?.dayPeriodBorderSide;
                }
                else
                {
                    lerpedBorderSide__15082 = BorderSide.lerp(a!.dayPeriodBorderSide!, b!.dayPeriodBorderSide!, t);
                }
            }
        }
        return new TimePickerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), cancelButtonStyle: ButtonStyle.lerp(a?.cancelButtonStyle, b?.cancelButtonStyle, t), confirmButtonStyle: ButtonStyle.lerp(a?.confirmButtonStyle, b?.confirmButtonStyle, t), dayPeriodBorderSide: lerpedBorderSide__15082, dayPeriodColor: Dart_uiLibrary.Color.lerp(a?.dayPeriodColor, b?.dayPeriodColor, t), dayPeriodShape: ((global::Doroti.Framework.Painting.OutlinedBorder?)(object?)ShapeBorder.lerp(a?.dayPeriodShape, b?.dayPeriodShape, t))!, dayPeriodTextColor: Dart_uiLibrary.Color.lerp(a?.dayPeriodTextColor, b?.dayPeriodTextColor, t), dayPeriodTextStyle: TextStyle.lerp(a?.dayPeriodTextStyle, b?.dayPeriodTextStyle, t), dialBackgroundColor: Dart_uiLibrary.Color.lerp(a?.dialBackgroundColor, b?.dialBackgroundColor, t), dialHandColor: Dart_uiLibrary.Color.lerp(a?.dialHandColor, b?.dialHandColor, t), dialTextColor: Dart_uiLibrary.Color.lerp(a?.dialTextColor, b?.dialTextColor, t), dialTextStyle: TextStyle.lerp(a?.dialTextStyle, b?.dialTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), entryModeIconColor: Dart_uiLibrary.Color.lerp(a?.entryModeIconColor, b?.entryModeIconColor, t), helpTextStyle: TextStyle.lerp(a?.helpTextStyle, b?.helpTextStyle, t), hourMinuteColor: Dart_uiLibrary.Color.lerp(a?.hourMinuteColor, b?.hourMinuteColor, t), hourMinuteShape: ShapeBorder.lerp(a?.hourMinuteShape, b?.hourMinuteShape, t), hourMinuteTextColor: Dart_uiLibrary.Color.lerp(a?.hourMinuteTextColor, b?.hourMinuteTextColor, t), hourMinuteTextStyle: TextStyle.lerp(a?.hourMinuteTextStyle, b?.hourMinuteTextStyle, t), inputDecorationTheme: ((t < 0.5) ? a?.inputDecorationTheme : b?.inputDecorationTheme), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), timeSelectorSeparatorColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.timeSelectorSeparatorColor, b?.timeSelectorSeparatorColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), timeSelectorSeparatorTextStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.timeSelectorSeparatorTextStyle, b?.timeSelectorSeparatorTextStyle, t, (global::System.Func<global::Doroti.Framework.Painting.TextStyle?, global::Doroti.Framework.Painting.TextStyle?, double, global::Doroti.Framework.Painting.TextStyle?>)global::Doroti.Framework.Painting.TextStyle.lerp));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { this.backgroundColor, this.cancelButtonStyle, this.confirmButtonStyle, this.dayPeriodBorderSide, this.dayPeriodColor, this.dayPeriodShape, this.dayPeriodTextColor, this.dayPeriodTextStyle, this.dialBackgroundColor, this.dialHandColor, this.dialTextColor, this.dialTextStyle, this.elevation, this.entryModeIconColor, this.helpTextStyle, this.hourMinuteColor, this.hourMinuteShape, this.hourMinuteTextColor, this.hourMinuteTextStyle, this.inputDecorationTheme, this.padding, this.shape, this.timeSelectorSeparatorColor, this.timeSelectorSeparatorTextStyle }));
    public override bool Equals(object? other)
    {
        var __other = other as TimePickerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((__other is TimePickerThemeData) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).cancelButtonStyle, this.cancelButtonStyle))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).confirmButtonStyle, this.confirmButtonStyle))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dayPeriodBorderSide, this.dayPeriodBorderSide))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dayPeriodColor, this.dayPeriodColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dayPeriodShape, this.dayPeriodShape))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dayPeriodTextColor, this.dayPeriodTextColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dayPeriodTextStyle, this.dayPeriodTextStyle))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dialBackgroundColor, this.dialBackgroundColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dialHandColor, this.dialHandColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dialTextColor, this.dialTextColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).dialTextStyle, this.dialTextStyle))) && (((TimePickerThemeData)((TimePickerThemeData)__other)).elevation == this.elevation)) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).entryModeIconColor, this.entryModeIconColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).helpTextStyle, this.helpTextStyle))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).hourMinuteColor, this.hourMinuteColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).hourMinuteShape, this.hourMinuteShape))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).hourMinuteTextColor, this.hourMinuteTextColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).hourMinuteTextStyle, this.hourMinuteTextStyle))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).inputDecorationTheme, this.inputDecorationTheme))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).padding, this.padding))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).shape, this.shape))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).timeSelectorSeparatorColor, this.timeSelectorSeparatorColor))) && (object.Equals(((TimePickerThemeData)((TimePickerThemeData)__other)).timeSelectorSeparatorTextStyle, this.timeSelectorSeparatorTextStyle)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("cancelButtonStyle", this.cancelButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ButtonStyle>("confirmButtonStyle", this.confirmButtonStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("dayPeriodBorderSide", this.dayPeriodBorderSide, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dayPeriodColor", this.dayPeriodColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("dayPeriodShape", this.dayPeriodShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dayPeriodTextColor", this.dayPeriodTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("dayPeriodTextStyle", this.dayPeriodTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialBackgroundColor", this.dialBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialHandColor", this.dialHandColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("dialTextColor", this.dialTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle?>("dialTextStyle", this.dialTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("entryModeIconColor", this.entryModeIconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("helpTextStyle", this.helpTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hourMinuteColor", this.hourMinuteColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("hourMinuteShape", this.hourMinuteShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("hourMinuteTextColor", this.hourMinuteTextColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("hourMinuteTextStyle", this.hourMinuteTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<InputDecorationThemeData>("inputDecorationTheme", this.inputDecorationTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("timeSelectorSeparatorColor", this.timeSelectorSeparatorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("timeSelectorSeparatorTextStyle", this.timeSelectorSeparatorTextStyle, defaultValue: null));
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

public class TimePickerTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual TimePickerThemeData data { get; private set; } = default!;

    public TimePickerTheme(global::Doroti.Framework.Foundation.Key? key = null, TimePickerThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static TimePickerThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimePickerTheme? timePickerTheme__23888 = ((TimePickerTheme?)(object?)context.dependOnInheritedWidgetOfExactType<TimePickerTheme>());
        return (timePickerTheme__23888?.data ?? Theme.of(context).timePickerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new TimePickerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((TimePickerTheme)oldWidget).data)));
}
