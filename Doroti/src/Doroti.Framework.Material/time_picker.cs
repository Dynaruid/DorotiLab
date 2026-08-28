// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/time_picker.dart
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

public static partial class Time_pickerLibrary
{
    internal static Duration _kDialogSizeAnimationDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Time_pickerLibrary
{
    internal static Duration _kDialAnimateDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Time_pickerLibrary
{
    internal static double _kTwoPi = (2L * Dart_mathLibrary.pi);
}

public static partial class Time_pickerLibrary
{
    internal static Duration _kVibrateCommitDelay = Duration.Create(milliseconds: 100L);
}

public static partial class Time_pickerLibrary
{
    internal static double _kTimePickerHeaderLandscapeWidth = 216;
}

public static partial class Time_pickerLibrary
{
    internal static double _kTimePickerInnerDialOffset = 28;
}

public static partial class Time_pickerLibrary
{
    internal static double _kTimePickerDialMinRadius = 50;
}

public static partial class Time_pickerLibrary
{
    internal static double _kTimePickerDialPadding = 28;
}

public enum TimePickerEntryMode
{
    dial,
    input,
    dialOnly,
    inputOnly
}

public enum _HourMinuteMode__time_picker
{
    hour,
    minute
}

internal enum _TimePickerAspect__time_picker
{
    use24HourFormat,
    useMaterial3,
    entryMode,
    hourMinuteMode,
    onHourMinuteModeChanged,
    onHourDoubleTapped,
    onMinuteDoubleTapped,
    hourDialType,
    selectedTime,
    onSelectedTimeChanged,
    orientation,
    theme,
    defaultTheme
}

internal class _TimePickerModel__time_picker : global::Doroti.Framework.Widgets.InheritedModel<_TimePickerAspect__time_picker>
{
    public virtual TimePickerEntryMode entryMode { get; private set; } = default!;
    public virtual _HourMinuteMode__time_picker hourMinuteMode { get; private set; } = default!;
    public virtual global::System.Action<_HourMinuteMode__time_picker> onHourMinuteModeChanged { get; private set; } = default!;
    public virtual global::System.Action onHourDoubleTapped { get; private set; } = default!;
    public virtual global::System.Action onMinuteDoubleTapped { get; private set; } = default!;
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual global::System.Action<TimeOfDay> onSelectedTimeChanged { get; private set; } = default!;
    public virtual bool use24HourFormat { get; private set; } = default!;
    public virtual bool useMaterial3 { get; private set; } = default!;
    public virtual _HourDialType__time_picker hourDialType { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;
    public virtual TimePickerThemeData theme { get; private set; } = default!;
    public virtual _TimePickerDefaults__time_picker defaultTheme { get; private set; } = default!;

    internal _TimePickerModel__time_picker(TimePickerEntryMode entryMode, _HourMinuteMode__time_picker hourMinuteMode, global::System.Action<_HourMinuteMode__time_picker> onHourMinuteModeChanged, global::System.Action onHourDoubleTapped, global::System.Action onMinuteDoubleTapped, TimeOfDay selectedTime, global::System.Action<TimeOfDay> onSelectedTimeChanged, bool use24HourFormat, bool useMaterial3, _HourDialType__time_picker hourDialType, global::Doroti.Framework.Widgets.Orientation orientation, TimePickerThemeData theme, _TimePickerDefaults__time_picker defaultTheme, global::Doroti.Framework.Widgets.Widget child) : base(child: child)
    {
        this.entryMode = entryMode;
        this.hourMinuteMode = hourMinuteMode;
        this.onHourMinuteModeChanged = onHourMinuteModeChanged;
        this.onHourDoubleTapped = onHourDoubleTapped;
        this.onMinuteDoubleTapped = onMinuteDoubleTapped;
        this.selectedTime = selectedTime;
        this.onSelectedTimeChanged = onSelectedTimeChanged;
        this.use24HourFormat = use24HourFormat;
        this.useMaterial3 = useMaterial3;
        this.hourDialType = hourDialType;
        this.orientation = orientation;
        this.theme = theme;
        this.defaultTheme = defaultTheme;
    }

    public static _TimePickerModel__time_picker of(global::Doroti.Framework.Widgets.BuildContext context, _TimePickerAspect__time_picker? aspect = null) => DartRuntimePrimitives.ConvertValue<_TimePickerModel__time_picker>(global::Doroti.Framework.Widgets.InheritedModel<object>.inheritFrom<_TimePickerModel__time_picker>(context, aspect: aspect)!);
    public static TimePickerEntryMode entryModeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.entryMode).entryMode;
    public static _HourMinuteMode__time_picker hourMinuteModeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.hourMinuteMode).hourMinuteMode;
    public static TimeOfDay selectedTimeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.selectedTime).selectedTime;
    public static bool use24HourFormatOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.use24HourFormat).use24HourFormat;
    public static bool useMaterial3Of(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.useMaterial3).useMaterial3;
    public static _HourDialType__time_picker hourDialTypeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.hourDialType).hourDialType;
    public static global::Doroti.Framework.Widgets.Orientation orientationOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.orientation).orientation;
    public static TimePickerThemeData themeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.theme).theme;
    public static _TimePickerDefaults__time_picker defaultThemeOf(global::Doroti.Framework.Widgets.BuildContext context) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.defaultTheme).defaultTheme;
    public static void setSelectedTime(global::Doroti.Framework.Widgets.BuildContext context, TimeOfDay value) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onSelectedTimeChanged).onSelectedTimeChanged(value);
    public static void setHourMinuteMode(global::Doroti.Framework.Widgets.BuildContext context, _HourMinuteMode__time_picker value) => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onHourMinuteModeChanged).onHourMinuteModeChanged(value);
    public override bool updateShouldNotifyDependent(global::Doroti.Framework.Widgets.InheritedModel<_TimePickerAspect__time_picker> oldWidget, HashSet<_TimePickerAspect__time_picker> dependencies)
    {
        var __oldWidget = (_TimePickerModel__time_picker)(object)oldWidget;
        if (((this.use24HourFormat != ((_TimePickerModel__time_picker)__oldWidget).use24HourFormat) && dependencies.Contains(_TimePickerAspect__time_picker.use24HourFormat)))
        {
            return true;
        }
        if (((this.useMaterial3 != ((_TimePickerModel__time_picker)__oldWidget).useMaterial3) && dependencies.Contains(_TimePickerAspect__time_picker.useMaterial3)))
        {
            return true;
        }
        if (((!object.Equals(this.entryMode, ((_TimePickerModel__time_picker)__oldWidget).entryMode)) && dependencies.Contains(_TimePickerAspect__time_picker.entryMode)))
        {
            return true;
        }
        if (((!object.Equals(this.hourMinuteMode, ((_TimePickerModel__time_picker)__oldWidget).hourMinuteMode)) && dependencies.Contains(_TimePickerAspect__time_picker.hourMinuteMode)))
        {
            return true;
        }
        if (((!object.Equals((global::System.Action<_HourMinuteMode__time_picker>)this.onHourMinuteModeChanged, (global::System.Action<_HourMinuteMode__time_picker>)((_TimePickerModel__time_picker)__oldWidget).onHourMinuteModeChanged)) && dependencies.Contains(_TimePickerAspect__time_picker.onHourMinuteModeChanged)))
        {
            return true;
        }
        if (((!object.Equals((global::System.Action<_HourMinuteMode__time_picker>)this.onHourMinuteModeChanged, (global::System.Action)((_TimePickerModel__time_picker)__oldWidget).onHourDoubleTapped)) && dependencies.Contains(_TimePickerAspect__time_picker.onHourDoubleTapped)))
        {
            return true;
        }
        if (((!object.Equals((global::System.Action<_HourMinuteMode__time_picker>)this.onHourMinuteModeChanged, (global::System.Action)((_TimePickerModel__time_picker)__oldWidget).onMinuteDoubleTapped)) && dependencies.Contains(_TimePickerAspect__time_picker.onMinuteDoubleTapped)))
        {
            return true;
        }
        if (((!object.Equals(this.hourDialType, ((_TimePickerModel__time_picker)__oldWidget).hourDialType)) && dependencies.Contains(_TimePickerAspect__time_picker.hourDialType)))
        {
            return true;
        }
        if (((!object.Equals(this.selectedTime, ((_TimePickerModel__time_picker)__oldWidget).selectedTime)) && dependencies.Contains(_TimePickerAspect__time_picker.selectedTime)))
        {
            return true;
        }
        if (((!object.Equals((global::System.Action<TimeOfDay>)this.onSelectedTimeChanged, (global::System.Action<TimeOfDay>)((_TimePickerModel__time_picker)__oldWidget).onSelectedTimeChanged)) && dependencies.Contains(_TimePickerAspect__time_picker.onSelectedTimeChanged)))
        {
            return true;
        }
        if (((!object.Equals(this.orientation, ((_TimePickerModel__time_picker)__oldWidget).orientation)) && dependencies.Contains(_TimePickerAspect__time_picker.orientation)))
        {
            return true;
        }
        if (((!object.Equals(this.theme, ((_TimePickerModel__time_picker)__oldWidget).theme)) && dependencies.Contains(_TimePickerAspect__time_picker.theme)))
        {
            return true;
        }
        if (((!object.Equals(this.defaultTheme, ((_TimePickerModel__time_picker)__oldWidget).defaultTheme)) && dependencies.Contains(_TimePickerAspect__time_picker.defaultTheme)))
        {
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_TimePickerModel__time_picker)(object)oldWidget;
        return (((((((((((((this.use24HourFormat != ((_TimePickerModel__time_picker)__oldWidget).use24HourFormat) || (this.useMaterial3 != ((_TimePickerModel__time_picker)__oldWidget).useMaterial3)) || (!object.Equals(this.entryMode, ((_TimePickerModel__time_picker)__oldWidget).entryMode))) || (!object.Equals(this.hourMinuteMode, ((_TimePickerModel__time_picker)__oldWidget).hourMinuteMode))) || (!object.Equals((global::System.Action<_HourMinuteMode__time_picker>)this.onHourMinuteModeChanged, (global::System.Action<_HourMinuteMode__time_picker>)((_TimePickerModel__time_picker)__oldWidget).onHourMinuteModeChanged))) || (!object.Equals((global::System.Action)this.onHourDoubleTapped, (global::System.Action)((_TimePickerModel__time_picker)__oldWidget).onHourDoubleTapped))) || (!object.Equals((global::System.Action)this.onMinuteDoubleTapped, (global::System.Action)((_TimePickerModel__time_picker)__oldWidget).onMinuteDoubleTapped))) || (!object.Equals(this.hourDialType, ((_TimePickerModel__time_picker)__oldWidget).hourDialType))) || (!object.Equals(this.selectedTime, ((_TimePickerModel__time_picker)__oldWidget).selectedTime))) || (!object.Equals((global::System.Action<TimeOfDay>)this.onSelectedTimeChanged, (global::System.Action<TimeOfDay>)((_TimePickerModel__time_picker)__oldWidget).onSelectedTimeChanged))) || (!object.Equals(this.orientation, ((_TimePickerModel__time_picker)__oldWidget).orientation))) || (!object.Equals(this.theme, ((_TimePickerModel__time_picker)__oldWidget).theme))) || (!object.Equals(this.defaultTheme, ((_TimePickerModel__time_picker)__oldWidget).defaultTheme)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialTimePickerHeader__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual string helpText { get; private set; } = default!;

    internal _DialTimePickerHeader__time_picker(string helpText)
    {
        this.helpText = helpText;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        TimeOfDayFormat timeOfDayFormatLocal = MaterialLocalizations.of(context).timeOfDayFormat(alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        _TimePickerDefaults__time_picker defaultTheme = ((_TimePickerDefaults__time_picker)(object?)_TimePickerModel__time_picker.defaultThemeOf(context));
        global::Doroti.Framework.Widgets.Orientation orientation = _TimePickerModel__time_picker.orientationOf(context);
        double dayPeriodHeight = ((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodPortraitSize.height : ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodLandscapeSize.height);
        double minInteractiveVerticalPadding = ((object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait)) ? Math.Max(0, ((2L * global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension) - dayPeriodHeight)) : Math.Max(0, (global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension - dayPeriodHeight)));
        _HourDialType__time_picker hourDialType = _TimePickerModel__time_picker.hourDialTypeOf(context);
        global::Doroti.Framework.Widgets.RenderObjectWidget orientationSpecificHeader = (orientation switch { global::Doroti.Framework.Widgets.Orientation.portrait => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.RenderObjectWidget>(new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(bottom: (((_TimePickerModel__time_picker.useMaterial3Of(context) ? 20L : 24L)) - (minInteractiveVerticalPadding / 2L))), child: new global::Doroti.Framework.Widgets.Text(this.helpText, style: ((_TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).helpTextStyle))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(textDirection: ((object.Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm)) ? TextDirection.rtl : TextDirection.ltr), spacing: 12, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection10472 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection10472.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Row(textDirection: TextDirection.ltr, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _DialHourControl__time_picker())), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _DialMinuteControl__time_picker())) })))); if ((object.Equals(hourDialType, _HourDialType__time_picker.twelveHour))) { __collection10472.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DayPeriodControl__time_picker())); } return __collection10472; }))())) })), global::Doroti.Framework.Widgets.Orientation.landscape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.RenderObjectWidget>(new global::Doroti.Framework.Widgets.SizedBox(width: Time_pickerLibrary._kTimePickerHeaderLandscapeWidth, child: new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text(this.helpText, style: ((_TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).helpTextStyle)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(verticalDirection: ((object.Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm)) ? global::Doroti.Framework.Painting.VerticalDirection.up : global::Doroti.Framework.Painting.VerticalDirection.down), mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, spacing: Math.Max(0, (16L - (minInteractiveVerticalPadding / 2L))), children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection11810 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection11810.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(textDirection: TextDirection.ltr, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _DialHourControl__time_picker())), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new _DialMinuteControl__time_picker())) }))); if ((object.Equals(hourDialType, _HourDialType__time_picker.twelveHour))) { __collection11810.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DayPeriodControl__time_picker())); } return __collection11810; }))())) }))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).formatTimeOfDay(_TimePickerModel__time_picker.selectedTimeOf(context), alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context)), child: orientationSpecificHeader));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialTimeSelectorControl__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual string text { get; private set; } = default!;
    public virtual global::System.Action onTap { get; private set; } = default!;
    public virtual global::System.Action onDoubleTap { get; private set; } = default!;
    public virtual bool isSelected { get; private set; } = default!;

    internal _DialTimeSelectorControl__time_picker(string text, global::System.Action onTap, global::System.Action onDoubleTap, bool isSelected)
    {
        this.text = text;
        this.onTap = onTap;
        this.onDoubleTap = onDoubleTap;
        this.isSelected = isSelected;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        TimePickerThemeData timePickerTheme = ((TimePickerThemeData)(object?)_TimePickerModel__time_picker.themeOf(context));
        _TimePickerDefaults__time_picker defaultTheme = ((_TimePickerDefaults__time_picker)(object?)_TimePickerModel__time_picker.defaultThemeOf(context));
        global::Doroti.Ui.Color backgroundColor = ((global::Doroti.Ui.Color)(object?)((timePickerTheme.hourMinuteColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteColor)));
        global::Doroti.Framework.Painting.ShapeBorder shapeLocal = ((timePickerTheme.hourMinuteShape ?? (global::Doroti.Framework.Painting.ShapeBorder)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteShape));
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection13567 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (this.isSelected) { __collection13567.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection13567; }))();
        global::Doroti.Ui.Color effectiveTextColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>((_TimePickerModel__time_picker.themeOf(context).hourMinuteTextColor ?? _TimePickerModel__time_picker.defaultThemeOf(context).hourMinuteTextColor), states));
        global::Doroti.Framework.Painting.TextStyle effectiveStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.TextStyle>(((timePickerTheme.hourMinuteTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteTextStyle)), states).copyWith(color: effectiveTextColor));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteSize.height, child: new Material(color: WidgetStateProperty.resolveAs(backgroundColor, states), clipBehavior: Clip.antiAlias, shape: shapeLocal, child: new InkWell(onTap: this.onTap, onDoubleTap: ((global::System.Action)(this.isSelected ? this.onDoubleTap : null)), child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(this.text, style: effectiveStyle, textScaler: global::Doroti.Framework.Painting.TextScaler.noScaling))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialHourControl__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal _DialHourControl__time_picker()
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool alwaysUse24HourFormatLocal = MediaQuery.alwaysUse24HourFormatOf(context);
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string formattedHour = localizations.formatHour(selectedTime, alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        TimeOfDay hoursFromSelected(long hoursToAdd)
        {
            switch (_TimePickerModel__time_picker.hourDialTypeOf(context))
            {
                case _HourDialType__time_picker.twentyFourHour:
                case _HourDialType__time_picker.twentyFourHourDoubleRing:
                    {
                        long selectedHour = ((TimeOfDay)selectedTime).hour;
                        return ((TimeOfDay)(object?)selectedTime.replacing(hour: (((selectedHour + hoursToAdd)) % TimeOfDay.hoursPerDay)));
                    }
                case _HourDialType__time_picker.twelveHour:
                    {
                        long periodOffsetLocal = ((TimeOfDay)selectedTime).periodOffset;
                        long hours = ((TimeOfDay)selectedTime).hourOfPeriod;
                        return ((TimeOfDay)(object?)selectedTime.replacing(hour: (periodOffsetLocal + (((hours + hoursToAdd)) % TimeOfDay.hoursPerPeriod))));
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        TimeOfDay nextHour = hoursFromSelected(1L);
        string formattedNextHour = localizations.formatHour(nextHour, alwaysUse24HourFormat: alwaysUse24HourFormatLocal);
        TimeOfDay previousHour = hoursFromSelected(-1L);
        string formattedPreviousHour = localizations.formatHour(previousHour, alwaysUse24HourFormat: alwaysUse24HourFormatLocal);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(value: $"{localizations.timePickerHourModeAnnouncement} {formattedHour}", excludeSemantics: true, increasedValue: formattedNextHour, onIncrease: ((global::System.Action)(() =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, nextHour);
        })), decreasedValue: formattedPreviousHour, onDecrease: ((global::System.Action)(() =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, previousHour);
        })), child: new _DialTimeSelectorControl__time_picker(isSelected: (object.Equals(_TimePickerModel__time_picker.hourMinuteModeOf(context), _HourMinuteMode__time_picker.hour)), text: formattedHour, onTap: ((global::System.Action)(() => { _TimePickerModel__time_picker.setHourMinuteMode(context, _HourMinuteMode__time_picker.hour); })), onDoubleTap: () => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onHourDoubleTapped).onHourDoubleTapped())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TimeSelectorSeparator__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual TimeOfDayFormat timeOfDayFormat { get; private set; } = default!;

    internal _TimeSelectorSeparator__time_picker(TimeOfDayFormat timeOfDayFormat)
    {
        this.timeOfDayFormat = timeOfDayFormat;
    }

    internal virtual string _timeSelectorSeparatorValue(TimeOfDayFormat timeOfDayFormat) => (timeOfDayFormat switch { TimeOfDayFormat.h_colon_mm_space_a or TimeOfDayFormat.a_space_h_colon_mm or TimeOfDayFormat.H_colon_mm => ":", TimeOfDayFormat.HH_colon_mm => ":", TimeOfDayFormat.HH_dot_mm => ".", TimeOfDayFormat.frenchCanadian => "h", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = (theme.useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
        var states = new HashSet<global::Doroti.Framework.Widgets.WidgetState>();
        global::Doroti.Ui.Color effectiveTextColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>((((((timePickerTheme.timeSelectorSeparatorColor?.resolve(states) ?? timePickerTheme.hourMinuteTextColor) ?? (Color)defaultTheme.timeSelectorSeparatorColor?.resolve(states))) ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteTextColor)), states));
        global::Doroti.Framework.Painting.TextStyle effectiveStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.TextStyle>((((((timePickerTheme.timeSelectorSeparatorTextStyle?.resolve(states) ?? timePickerTheme.hourMinuteTextStyle) ?? (global::Doroti.Framework.Painting.TextStyle)defaultTheme.timeSelectorSeparatorTextStyle?.resolve(states))) ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteTextStyle)), states).copyWith(color: effectiveTextColor, height: 1.0));
        double heightLocal = (_TimePickerModel__time_picker.entryModeOf(context) switch { TimePickerEntryMode.dial => ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteSize.height, TimePickerEntryMode.dialOnly => ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteSize.height, TimePickerEntryMode.input => ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteInputSize.height, TimePickerEntryMode.inputOnly => ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteInputSize.height, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.SizedBox(width: ((object.Equals(this.timeOfDayFormat, TimeOfDayFormat.frenchCanadian)) ? 36 : 24), height: heightLocal, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(_timeSelectorSeparatorValue(this.timeOfDayFormat), style: effectiveStyle, textScaler: global::Doroti.Framework.Painting.TextScaler.noScaling)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialMinuteControl__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal _DialMinuteControl__time_picker()
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        string formattedMinute = localizations.formatMinute(selectedTime);
        TimeOfDay nextMinute = ((TimeOfDay)(object?)selectedTime.replacing(minute: (((((TimeOfDay)selectedTime).minute + 1L)) % TimeOfDay.minutesPerHour)));
        string formattedNextMinute = localizations.formatMinute(nextMinute);
        TimeOfDay previousMinute = ((TimeOfDay)(object?)selectedTime.replacing(minute: (((((TimeOfDay)selectedTime).minute - 1L)) % TimeOfDay.minutesPerHour)));
        string formattedPreviousMinute = localizations.formatMinute(previousMinute);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(excludeSemantics: true, value: $"{localizations.timePickerMinuteModeAnnouncement} {formattedMinute}", increasedValue: formattedNextMinute, onIncrease: ((global::System.Action)(() =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, nextMinute);
        })), decreasedValue: formattedPreviousMinute, onDecrease: ((global::System.Action)(() =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, previousMinute);
        })), child: new _DialTimeSelectorControl__time_picker(isSelected: (object.Equals(_TimePickerModel__time_picker.hourMinuteModeOf(context), _HourMinuteMode__time_picker.minute)), text: formattedMinute, onTap: ((global::System.Action)(() => { _TimePickerModel__time_picker.setHourMinuteMode(context, _HourMinuteMode__time_picker.minute); })), onDoubleTap: () => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onMinuteDoubleTapped).onMinuteDoubleTapped())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPeriodControl__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action<TimeOfDay>? onPeriodChanged { get; private set; }

    internal _DayPeriodControl__time_picker(global::System.Action<TimeOfDay>? onPeriodChanged = null)
    {
        this.onPeriodChanged = onPeriodChanged;
    }

    internal virtual void _togglePeriod(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        long newHour = (((((TimeOfDay)selectedTime).hour + TimeOfDay.hoursPerPeriod)) % TimeOfDay.hoursPerDay);
        TimeOfDay newTime = ((TimeOfDay)(object?)selectedTime.replacing(hour: newHour));
        if ((this.onPeriodChanged is not null))
        {
            this.onPeriodChanged!(newTime);
        }
        else
        {
            _TimePickerModel__time_picker.setSelectedTime(context, newTime);
        }
    }

    internal virtual void _setAm(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        if ((object.Equals(((TimeOfDay)selectedTime).period, DayPeriod.am)))
        {
            return;
        }
        _togglePeriod(context);
    }

    internal virtual void _setPm(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        if ((object.Equals(((TimeOfDay)selectedTime).period, DayPeriod.pm)))
        {
            return;
        }
        _togglePeriod(context);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations materialLocalizations = MaterialLocalizations.of(context);
        TimePickerThemeData timePickerTheme = ((TimePickerThemeData)(object?)_TimePickerModel__time_picker.themeOf(context));
        _TimePickerDefaults__time_picker defaultTheme = ((_TimePickerDefaults__time_picker)(object?)_TimePickerModel__time_picker.defaultThemeOf(context));
        TimeOfDay selectedTime = ((TimeOfDay)(object?)_TimePickerModel__time_picker.selectedTimeOf(context));
        var amSelected = (object.Equals(((TimeOfDay)selectedTime).period, DayPeriod.am));
        bool pmSelected = !amSelected;
        global::Doroti.Framework.Painting.BorderSide resolvedSide = ((timePickerTheme.dayPeriodBorderSide ?? (global::Doroti.Framework.Painting.BorderSide)((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodBorderSide));
        global::Doroti.Framework.Painting.OutlinedBorder resolvedShape = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)(((timePickerTheme.dayPeriodShape ?? (global::Doroti.Framework.Painting.OutlinedBorder)((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodShape))).copyWith(side: resolvedSide));
        global::Doroti.Ui.Size dayPeriodSize = default!;
        global::Doroti.Framework.Widgets.Orientation orientationLocal = default!;
        switch (_TimePickerModel__time_picker.entryModeOf(context))
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    orientationLocal = _TimePickerModel__time_picker.orientationOf(context);
                    dayPeriodSize = (orientationLocal switch { global::Doroti.Framework.Widgets.Orientation.portrait => ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodPortraitSize, global::Doroti.Framework.Widgets.Orientation.landscape => ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodLandscapeSize, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    break;
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    orientationLocal = global::Doroti.Framework.Widgets.Orientation.portrait;
                    dayPeriodSize = ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodInputSize;
                    break;
                }
        }
        var amShape = resolvedShape;
        var pmShape = resolvedShape;
        bool hasRoundedBorder = ((resolvedShape is global::Doroti.Framework.Painting.RoundedRectangleBorder) && (((global::Doroti.Framework.Painting.RoundedRectangleBorder)((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape)).borderRadius is global::Doroti.Framework.Painting.BorderRadius));
        switch (orientationLocal)
        {
            case global::Doroti.Framework.Widgets.Orientation.portrait:
                {
                    if (hasRoundedBorder)
                    {
                        var borderRadiusLocal = ((global::Doroti.Framework.Painting.BorderRadius?)(object?)((global::Doroti.Framework.Painting.RoundedRectangleBorder)((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape)).borderRadius)!;
                        amShape = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new global::Doroti.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusLocal).topLeft, topRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusLocal).topRight)));
                        pmShape = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new global::Doroti.Framework.Painting.BorderRadius(bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusLocal).bottomLeft, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusLocal).bottomRight)));
                    }
                    var minInteractiveSize = new global::Doroti.Ui.Size(dayPeriodSize.width, Math.Max(dayPeriodSize.height, (2L * global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension)));
                    global::Doroti.Framework.Widgets.Widget amButton = ((global::Doroti.Framework.Widgets.Widget)(object?)new _AmPmButton__time_picker(selected: amSelected, onPressed: ((global::System.Action)(() => { _setAm(context); })), label: materialLocalizations.anteMeridiemAbbreviation, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: (((minInteractiveSize.height - dayPeriodSize.height)) / 2L)), shape: amShape));
                    global::Doroti.Framework.Widgets.Widget pmButton = ((global::Doroti.Framework.Widgets.Widget)(object?)new _AmPmButton__time_picker(selected: pmSelected, onPressed: ((global::System.Action)(() => { _setPm(context); })), label: materialLocalizations.postMeridiemAbbreviation, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: (((minInteractiveSize.height - dayPeriodSize.height)) / 2L)), shape: pmShape));
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DayPeriodInputPadding__time_picker(minSize: minInteractiveSize, orientation: orientationLocal, child: global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: minInteractiveSize, child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: amButton)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: pmButton)) }))));
                }
            case global::Doroti.Framework.Widgets.Orientation.landscape:
                {
                    if (hasRoundedBorder)
                    {
                        var borderRadiusAlternate = ((global::Doroti.Framework.Painting.BorderRadius?)(object?)((global::Doroti.Framework.Painting.RoundedRectangleBorder)((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape)).borderRadius)!;
                        amShape = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new global::Doroti.Framework.Painting.BorderRadius(topLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusAlternate).topLeft, bottomLeft: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusAlternate).bottomLeft)));
                        pmShape = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.OutlinedBorder>(((global::Doroti.Framework.Painting.RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new global::Doroti.Framework.Painting.BorderRadius(topRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusAlternate).topRight, bottomRight: ((global::Doroti.Framework.Painting.BorderRadius)borderRadiusAlternate).bottomRight)));
                    }
                    var minInteractiveSizeLocal = new global::Doroti.Ui.Size(dayPeriodSize.width, Math.Max(dayPeriodSize.height, global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension));
                    global::Doroti.Framework.Widgets.Widget amButtonLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new _AmPmButton__time_picker(selected: amSelected, onPressed: ((global::System.Action)(() => { _setAm(context); })), label: materialLocalizations.anteMeridiemAbbreviation, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: (((minInteractiveSizeLocal.height - dayPeriodSize.height)) / 2L)), shape: amShape));
                    global::Doroti.Framework.Widgets.Widget pmButtonLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new _AmPmButton__time_picker(selected: pmSelected, onPressed: ((global::System.Action)(() => { _setPm(context); })), label: materialLocalizations.postMeridiemAbbreviation, padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(vertical: (((minInteractiveSizeLocal.height - dayPeriodSize.height)) / 2L)), shape: pmShape));
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DayPeriodInputPadding__time_picker(minSize: minInteractiveSizeLocal, orientation: orientationLocal, child: new global::Doroti.Framework.Widgets.SizedBox(height: minInteractiveSizeLocal.height, child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: amButtonLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: pmButtonLocal)) }))));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AmPmButton__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual bool selected { get; private set; } = default!;
    public virtual global::System.Action onPressed { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets padding { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.OutlinedBorder shape { get; private set; } = default!;

    internal _AmPmButton__time_picker(global::System.Action onPressed, bool selected, string label, global::Doroti.Framework.Painting.EdgeInsets padding, global::Doroti.Framework.Painting.OutlinedBorder shape)
    {
        this.onPressed = onPressed;
        this.selected = selected;
        this.label = label;
        this.padding = padding;
        this.shape = shape;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection28792 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (this.selected) { __collection28792.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection28792; }))();
        TimePickerThemeData timePickerTheme = ((TimePickerThemeData)(object?)_TimePickerModel__time_picker.themeOf(context));
        _TimePickerDefaults__time_picker defaultTheme = ((_TimePickerDefaults__time_picker)(object?)_TimePickerModel__time_picker.defaultThemeOf(context));
        global::Doroti.Ui.Color resolvedBackgroundColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(((timePickerTheme.dayPeriodColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodColor)), states));
        global::Doroti.Ui.Color resolvedTextColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(((timePickerTheme.dayPeriodTextColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodTextColor)), states));
        global::Doroti.Framework.Painting.TextStyle? resolvedTextStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.TextStyle?>(((timePickerTheme.dayPeriodTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodTextStyle)), states)?.copyWith(color: resolvedTextColor));
        global::Doroti.Framework.Painting.TextScaler buttonTextScaler = ((global::Doroti.Framework.Painting.TextScaler)(object?)MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 2.0));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(selected: ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) ? this.selected : null), @checked: ((object.Equals(global::Doroti.Framework.Foundation.PlatformLibrary.defaultTargetPlatform, global::Doroti.Framework.Foundation.TargetPlatform.iOS)) ? null : this.selected), inMutuallyExclusiveGroup: true, button: true, child: new global::Doroti.Framework.Widgets.Padding(padding: this.padding, child: new Material(clipBehavior: Clip.antiAlias, color: resolvedBackgroundColor, shape: this.shape, child: new InkWell(onTap: this.onPressed, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(this.label, style: resolvedTextStyle, textScaler: buttonTextScaler)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPeriodInputPadding__time_picker : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation orientation { get; private set; } = default!;

    internal _DayPeriodInputPadding__time_picker(global::Doroti.Framework.Widgets.Widget child, Size minSize, global::Doroti.Framework.Widgets.Orientation orientation) : base(child: child)
    {
        this.minSize = minSize;
        this.orientation = orientation;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderInputPadding__time_picker(this.minSize, this.orientation));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__time_picker)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderInputPadding__time_picker>)(() =>
{
    var __cascade = __renderObject;
    __cascade.minSize = this.minSize;
    __cascade.orientation = this.orientation;
    return __cascade;
}))());
    }

}

public class _RenderInputPadding__time_picker : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.Orientation _orientation { get; set; } = default!;

    internal _RenderInputPadding__time_picker(Size _minSize, global::Doroti.Framework.Widgets.Orientation _orientation, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
        this._orientation = _orientation;
    }

    public virtual global::Doroti.Ui.Size minSize
    {
        get => this._minSize;
        set
        {
            var __value = value;
            if ((object.Equals(this._minSize, __value)))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Widgets.Orientation orientation
    {
        get => this._orientation;
        set
        {
            var __value = value;
            if ((object.Equals(this._orientation, __value)))
            {
                return;
            }
            _orientation = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicWidth(height), this.minSize.width);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMinIntrinsicHeight(width), this.minSize.height);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicWidth(height), this.minSize.width);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if ((this.child is not null))
        {
            return Math.Max(this.child!.getMaxIntrinsicHeight(width), this.minSize.height);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeSize(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        if ((this.child is not null))
        {
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)layoutChild(this.child!, constraints));
            double widthLocal = Math.Max(childSize.width, this.minSize.width);
            double heightLocal = Math.Max(childSize.height, this.minSize.height);
            return ((global::Doroti.Ui.Size)(object?)constraints.constrain(new global::Doroti.Ui.Size(widthLocal, heightLocal)));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size drySize = ((global::Doroti.Ui.Size)(object?)getDryLayout(constraints));
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(constraints));
        global::Doroti.Ui.Offset childOffset = ((global::Doroti.Ui.Offset)(object?)global::Doroti.Framework.Painting.Alignment.center.alongOffset((drySize - childSize)));
        return (DartRuntimePrimitives.RequireValue(result) + childOffset.dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: this.constraints, layoutChild: (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild);
        if ((this.child is not null))
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
            childParentData.offset = global::Doroti.Framework.Painting.Alignment.center.alongOffset((this.size - this.child!.size));
        }
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        if (((((position.dx < 0L) || (position.dx > Math.Max(this.child!.size.width, this.minSize.width))) || (position.dy < 0L)) || (position.dy > Math.Max(this.child!.size.height, this.minSize.height))))
        {
            return false;
        }
        global::Doroti.Ui.Offset newPosition = ((global::Doroti.Ui.Offset)(object?)this.child!.size.center(Offset.zero));
        newPosition += (this.orientation switch { global::Doroti.Framework.Widgets.Orientation.portrait when ((position.dy > newPosition.dy)) => new global::Doroti.Ui.Offset(0, 1), global::Doroti.Framework.Widgets.Orientation.landscape when ((position.dx > newPosition.dx)) => new global::Doroti.Ui.Offset(1, 0), global::Doroti.Framework.Widgets.Orientation.portrait => new global::Doroti.Ui.Offset(0, -1), global::Doroti.Framework.Widgets.Orientation.landscape => new global::Doroti.Ui.Offset(-1, 0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(newPosition), position: newPosition, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, newPosition)));
            return this.child!.hitTest(result, position: newPosition);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TappableLabel__time_picker
{
    public virtual long value { get; private set; } = default!;
    public virtual bool inner { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextPainter painter { get; private set; } = default!;
    public virtual global::System.Action onTap { get; private set; } = default!;

    internal _TappableLabel__time_picker(long value, bool inner, global::Doroti.Framework.Painting.TextPainter painter, global::System.Action onTap)
    {
        this.value = value;
        this.inner = inner;
        this.painter = painter;
        this.onTap = onTap;
    }

}

public class _DialPainter__time_picker : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual List<_TappableLabel__time_picker> primaryLabels { get; private set; } = default!;
    public virtual List<_TappableLabel__time_picker> selectedLabels { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual Color handColor { get; private set; } = default!;
    public virtual double handWidth { get; private set; } = default!;
    public virtual Color dotColor { get; private set; } = default!;
    public virtual double dotRadius { get; private set; } = default!;
    public virtual double centerRadius { get; private set; } = default!;
    public virtual double theta { get; private set; } = default!;
    public virtual double radius { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;
    public virtual long selectedValue { get; private set; } = default!;

    internal _DialPainter__time_picker(List<_TappableLabel__time_picker> primaryLabels, List<_TappableLabel__time_picker> selectedLabels, Color backgroundColor, Color handColor, double handWidth, Color dotColor, double dotRadius, double centerRadius, double theta, double radius, TextDirection textDirection, long selectedValue) : base(repaint: global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts)
    {
        this.primaryLabels = primaryLabels;
        this.selectedLabels = selectedLabels;
        this.backgroundColor = backgroundColor;
        this.handColor = handColor;
        this.handWidth = handWidth;
        this.dotColor = dotColor;
        this.dotRadius = dotRadius;
        this.centerRadius = centerRadius;
        this.theta = theta;
        this.radius = radius;
        this.textDirection = textDirection;
        this.selectedValue = selectedValue;
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_DialPainter", this));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        foreach (_TappableLabel__time_picker label in this.primaryLabels)
        {
            ((_TappableLabel__time_picker)label).painter.dispose();
        }
        foreach (_TappableLabel__time_picker labelLocal in this.selectedLabels)
        {
            ((_TappableLabel__time_picker)labelLocal).painter.dispose();
        }
        this.primaryLabels.Clear();
        this.selectedLabels.Clear();
    }

    public override void paint(Canvas canvas, Size size)
    {
        double dialRadius = Dart_uiLibrary.clampDouble((size.shortestSide / 2L), (Time_pickerLibrary._kTimePickerDialMinRadius + this.dotRadius), double.PositiveInfinity);
        double labelRadius = Dart_uiLibrary.clampDouble((dialRadius - Time_pickerLibrary._kTimePickerDialPadding), Time_pickerLibrary._kTimePickerDialMinRadius, double.PositiveInfinity);
        double innerLabelRadius = Dart_uiLibrary.clampDouble((labelRadius - Time_pickerLibrary._kTimePickerInnerDialOffset), 0, double.PositiveInfinity);
        double handleRadius = Dart_uiLibrary.clampDouble((labelRadius - ((((this.radius < 0.5) ? 1L : 0L)) * ((labelRadius - innerLabelRadius)))), Time_pickerLibrary._kTimePickerDialMinRadius, double.PositiveInfinity);
        var centerLocal = new global::Doroti.Ui.Offset((size.width / 2L), (size.height / 2L));
        var centerPoint = centerLocal;
        canvas.drawCircle(centerPoint, dialRadius, ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.backgroundColor;
    return __cascade;
}))());
        Offset getOffsetForTheta(double theta, double radius)
        {
            return (centerLocal + new global::Doroti.Ui.Offset((radius * global::Doroti.Runtime.Dart_mathLibrary.cos(theta)), (-radius * global::Doroti.Runtime.Dart_mathLibrary.sin(theta))));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        void paintLabels(List<_TappableLabel__time_picker> labels, double radius)
        {
            if (!System.Linq.Enumerable.Any(labels))
            {
                return;
            }
            double labelThetaIncrement = (-Time_pickerLibrary._kTwoPi / checked((long)(labels.Count)));
            double labelTheta = (Dart_mathLibrary.pi / 2L);
            foreach (var labelLocal in labels)
            {
                global::Doroti.Framework.Painting.TextPainter labelPainter = ((_TappableLabel__time_picker)labelLocal).painter;
                var labelOffset = new global::Doroti.Ui.Offset((-((global::Doroti.Framework.Painting.TextPainter)labelPainter).width / 2L), (-((global::Doroti.Framework.Painting.TextPainter)labelPainter).height / 2L));
                labelPainter.paint(canvas, (getOffsetForTheta(labelTheta, radius) + labelOffset));
                labelTheta += labelThetaIncrement;
            }
        }
        void paintInnerOuterLabels(List<_TappableLabel__time_picker>? labels)
        {
            if ((labels is null))
            {
                return;
            }
            paintLabels(labels.where(((label) => !((_TappableLabel__time_picker)label).inner)).ToList(), labelRadius);
            paintLabels(labels.where(((label) => ((_TappableLabel__time_picker)label).inner)).ToList(), innerLabelRadius);
        }
        paintInnerOuterLabels(this.primaryLabels);
        var selectorPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.handColor;
    return __cascade;
}))();
        global::Doroti.Ui.Offset focusedPoint = ((global::Doroti.Ui.Offset)(object?)getOffsetForTheta(this.theta, handleRadius));
        canvas.drawCircle(centerPoint, this.centerRadius, selectorPaint);
        canvas.drawCircle(focusedPoint, this.dotRadius, selectorPaint);
        selectorPaint.strokeWidth = this.handWidth;
        canvas.drawLine(centerPoint, focusedPoint, selectorPaint);
        double labelThetaIncrementLocal = (-Time_pickerLibrary._kTwoPi / checked((long)(this.primaryLabels.Count)));
        if ((((this.theta % labelThetaIncrementLocal) > 0.1) && ((this.theta % labelThetaIncrementLocal) < 0.45)))
        {
            canvas.drawCircle(focusedPoint, 2, ((Func<Paint>)(() =>
{
    var __cascade = selectorPaint;
    __cascade.color = this.dotColor;
    return __cascade;
}))());
        }
        var focusedRect = global::Doroti.Ui.Rect.fromCircle(center: focusedPoint, radius: this.dotRadius);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.clipPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addOval(focusedRect);
    return __cascade;
}))());
    return __cascade;
}))());
        paintInnerOuterLabels(this.selectedLabels);
        canvas.restore();
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldPainter = (_DialPainter__time_picker)(object)oldDelegate;
        return (((((!object.Equals(((_DialPainter__time_picker)__oldPainter).primaryLabels, this.primaryLabels)) || (!object.Equals(((_DialPainter__time_picker)__oldPainter).selectedLabels, this.selectedLabels))) || (!object.Equals(((_DialPainter__time_picker)__oldPainter).backgroundColor, this.backgroundColor))) || (!object.Equals(((_DialPainter__time_picker)__oldPainter).handColor, this.handColor))) || (((_DialPainter__time_picker)__oldPainter).theta != this.theta));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _HourDialType__time_picker
{
    twentyFourHour,
    twentyFourHourDoubleRing,
    twelveHour
}

public class _Dial__time_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual _HourMinuteMode__time_picker hourMinuteMode { get; private set; } = default!;
    public virtual _HourDialType__time_picker hourDialType { get; private set; } = default!;
    public virtual global::System.Action<TimeOfDay>? onChanged { get; private set; }
    public virtual global::System.Action? onHourSelected { get; private set; }

    internal _Dial__time_picker(TimeOfDay selectedTime, _HourMinuteMode__time_picker hourMinuteMode, _HourDialType__time_picker hourDialType, global::System.Action<TimeOfDay>? onChanged, global::System.Action? onHourSelected)
    {
        this.selectedTime = selectedTime;
        this.hourMinuteMode = hourMinuteMode;
        this.hourDialType = hourDialType;
        this.onChanged = onChanged;
        this.onHourSelected = onHourSelected;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DialState__time_picker());
}

public class _DialState__time_picker : global::Doroti.Framework.Widgets.State<_Dial__time_picker>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_Dial__time_picker>
{
    public virtual ThemeData themeData { get; set; } = default!;
    public virtual MaterialLocalizations localizations { get; set; } = default!;
    public virtual _DialPainter__time_picker? painter { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Tween<double> _thetaTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _theta { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Tween<double> _radiusTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _radius { get; set; } = default!;
    internal virtual bool _dragging { get; set; } = false;
    internal virtual Offset? _position { get; set; } = default;
    internal virtual Offset? _center { get; set; } = default;
    internal virtual Size? _dialSize { get; set; } = default;
    internal static List<TimeOfDay> _amHours = new List<TimeOfDay> { new TimeOfDay(hour: 12L, minute: 0L), new TimeOfDay(hour: 1L, minute: 0L), new TimeOfDay(hour: 2L, minute: 0L), new TimeOfDay(hour: 3L, minute: 0L), new TimeOfDay(hour: 4L, minute: 0L), new TimeOfDay(hour: 5L, minute: 0L), new TimeOfDay(hour: 6L, minute: 0L), new TimeOfDay(hour: 7L, minute: 0L), new TimeOfDay(hour: 8L, minute: 0L), new TimeOfDay(hour: 9L, minute: 0L), new TimeOfDay(hour: 10L, minute: 0L), new TimeOfDay(hour: 11L, minute: 0L) };
    internal static List<TimeOfDay> _twentyFourHoursM2 = new List<TimeOfDay> { new TimeOfDay(hour: 0L, minute: 0L), new TimeOfDay(hour: 2L, minute: 0L), new TimeOfDay(hour: 4L, minute: 0L), new TimeOfDay(hour: 6L, minute: 0L), new TimeOfDay(hour: 8L, minute: 0L), new TimeOfDay(hour: 10L, minute: 0L), new TimeOfDay(hour: 12L, minute: 0L), new TimeOfDay(hour: 14L, minute: 0L), new TimeOfDay(hour: 16L, minute: 0L), new TimeOfDay(hour: 18L, minute: 0L), new TimeOfDay(hour: 20L, minute: 0L), new TimeOfDay(hour: 22L, minute: 0L) };
    internal static List<TimeOfDay> _twentyFourHours = new List<TimeOfDay> { new TimeOfDay(hour: 0L, minute: 0L), new TimeOfDay(hour: 1L, minute: 0L), new TimeOfDay(hour: 2L, minute: 0L), new TimeOfDay(hour: 3L, minute: 0L), new TimeOfDay(hour: 4L, minute: 0L), new TimeOfDay(hour: 5L, minute: 0L), new TimeOfDay(hour: 6L, minute: 0L), new TimeOfDay(hour: 7L, minute: 0L), new TimeOfDay(hour: 8L, minute: 0L), new TimeOfDay(hour: 9L, minute: 0L), new TimeOfDay(hour: 10L, minute: 0L), new TimeOfDay(hour: 11L, minute: 0L), new TimeOfDay(hour: 12L, minute: 0L), new TimeOfDay(hour: 13L, minute: 0L), new TimeOfDay(hour: 14L, minute: 0L), new TimeOfDay(hour: 15L, minute: 0L), new TimeOfDay(hour: 16L, minute: 0L), new TimeOfDay(hour: 17L, minute: 0L), new TimeOfDay(hour: 18L, minute: 0L), new TimeOfDay(hour: 19L, minute: 0L), new TimeOfDay(hour: 20L, minute: 0L), new TimeOfDay(hour: 21L, minute: 0L), new TimeOfDay(hour: 22L, minute: 0L), new TimeOfDay(hour: 23L, minute: 0L) };
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _animationController = new global::Doroti.Framework.Animation.AnimationController(duration: Time_pickerLibrary._kDialAnimateDuration, vsync: this);
        _thetaTween = new global::Doroti.Framework.Animation.Tween<double>(begin: _getThetaForTime(((_Dial__time_picker)this.widget).selectedTime));
        _radiusTween = new global::Doroti.Framework.Animation.Tween<double>(begin: _getRadiusForTime(((_Dial__time_picker)this.widget).selectedTime));
        _theta = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = this._animationController.drive(new global::Doroti.Framework.Animation.CurveTween(curve: CurvesLibrary.standardEasing)).drive(this._thetaTween);
    __cascade.addListener(((global::System.Action)(() =>
    {
        setState(((global::System.Action)(() =>
        {
        })));
    })));
    return __cascade;
}))();
        _radius = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = this._animationController.drive(new global::Doroti.Framework.Animation.CurveTween(curve: CurvesLibrary.standardEasing)).drive(this._radiusTween);
    __cascade.addListener(((global::System.Action)(() =>
    {
        setState(((global::System.Action)(() =>
        {
        })));
    })));
    return __cascade;
}))();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(this.context));
        themeData = Theme.of(this.context);
        localizations = MaterialLocalizations.of(this.context);
    }

    public override void didUpdateWidget(_Dial__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((_Dial__time_picker)this.widget).hourMinuteMode, ((_Dial__time_picker)oldWidget).hourMinuteMode)) || (!object.Equals(((_Dial__time_picker)this.widget).selectedTime, ((_Dial__time_picker)oldWidget).selectedTime))))
        {
            if (!this._dragging)
            {
                _animateTo(_getThetaForTime(((_Dial__time_picker)this.widget).selectedTime), _getRadiusForTime(((_Dial__time_picker)this.widget).selectedTime));
            }
        }
    }

    public override void dispose()
    {
        this._animationController.dispose();
        this.painter?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this._ticker is null) || !this._ticker!.isActive))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal static double _nearest(double target, double a, double b)
    {
        return (((((target - a)).abs() < ((target - b)).abs())) ? a : b);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _animateTo(double targetTheta, double targetRadius)
    {
        void animateToValue(double target, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Tween<double> tween, global::Doroti.Framework.Animation.AnimationController controller, double min, double max)
        {
            double beginValue = _DialState__time_picker._nearest(target, ((global::Doroti.Framework.Animation.Animation<double>)animation).value, max);
            beginValue = _DialState__time_picker._nearest(target, beginValue, min);
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.Tween<double>>)(() =>
{
    var __cascade = tween;
    __cascade.begin = beginValue;
    __cascade.end = target;
    return __cascade;
}))());
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = controller;
    __cascade.value = 0;
    __cascade.forward();
    return __cascade;
}))());
        }
        animateToValue(target: targetTheta, animation: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>(this._theta), tween: this._thetaTween, controller: this._animationController, min: (((global::Doroti.Framework.Animation.Animation<double>)this._theta).value - Time_pickerLibrary._kTwoPi), max: (((global::Doroti.Framework.Animation.Animation<double>)this._theta).value + Time_pickerLibrary._kTwoPi));
        animateToValue(target: targetRadius, animation: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.AnimationController>(this._radius), tween: this._radiusTween, controller: this._animationController, min: 0, max: 1);
    }

    internal virtual double _getRadiusForTime(TimeOfDay time)
    {
        switch (((_Dial__time_picker)this.widget).hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    return (((_Dial__time_picker)this.widget).hourDialType switch { _HourDialType__time_picker.twentyFourHourDoubleRing => ((((TimeOfDay)time).hour >= 12L) ? 0 : 1), _HourDialType__time_picker.twentyFourHour => 1, _HourDialType__time_picker.twelveHour => 1, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case _HourMinuteMode__time_picker.minute:
                {
                    return 1;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getThetaForTime(TimeOfDay time)
    {
        long hoursFactor = (((_Dial__time_picker)this.widget).hourDialType switch { _HourDialType__time_picker.twentyFourHour => TimeOfDay.hoursPerDay, _HourDialType__time_picker.twentyFourHourDoubleRing => TimeOfDay.hoursPerPeriod, _HourDialType__time_picker.twelveHour => TimeOfDay.hoursPerPeriod, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double fraction = (((_Dial__time_picker)this.widget).hourMinuteMode switch { _HourMinuteMode__time_picker.hour => (((((TimeOfDay)time).hour / hoursFactor)) % hoursFactor), _HourMinuteMode__time_picker.minute => (((((TimeOfDay)time).minute / TimeOfDay.minutesPerHour)) % TimeOfDay.minutesPerHour), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return ((((Dart_mathLibrary.pi / 2L) - (fraction * Time_pickerLibrary._kTwoPi))) % Time_pickerLibrary._kTwoPi);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TimeOfDay _getTimeForTheta(double theta, bool roundMinutes = false, double radius = default!)
    {
        double fraction = (((0.25 - (((theta % Time_pickerLibrary._kTwoPi)) / Time_pickerLibrary._kTwoPi))) % 1L);
        switch (((_Dial__time_picker)this.widget).hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    long newHour = default!;
                    switch (((_Dial__time_picker)this.widget).hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHour:
                            {
                                newHour = (((fraction * TimeOfDay.hoursPerDay)).round() % TimeOfDay.hoursPerDay);
                                break;
                            }
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                newHour = (((fraction * TimeOfDay.hoursPerPeriod)).round() % TimeOfDay.hoursPerPeriod);
                                if ((radius < 0.5))
                                {
                                    newHour = (newHour + TimeOfDay.hoursPerPeriod);
                                }
                                break;
                            }
                        case _HourDialType__time_picker.twelveHour:
                            {
                                newHour = (((fraction * TimeOfDay.hoursPerPeriod)).round() % TimeOfDay.hoursPerPeriod);
                                newHour = (newHour + ((_Dial__time_picker)this.widget).selectedTime.periodOffset);
                                break;
                            }
                    }
                    return ((TimeOfDay)(object?)((_Dial__time_picker)this.widget).selectedTime.replacing(hour: newHour));
                }
            case _HourMinuteMode__time_picker.minute:
                {
                    long minuteLocal = (((fraction * TimeOfDay.minutesPerHour)).round() % TimeOfDay.minutesPerHour);
                    if (roundMinutes)
                    {
                        minuteLocal = ((((checked((long)(((minuteLocal + 2L)) / 5L)))) * 5L) % TimeOfDay.minutesPerHour);
                    }
                    return ((TimeOfDay)(object?)((_Dial__time_picker)this.widget).selectedTime.replacing(minute: minuteLocal));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TimeOfDay _notifyOnChangedIfNeeded(bool roundMinutes = false)
    {
        TimeOfDay current = ((TimeOfDay)(object?)_getTimeForTheta(((global::Doroti.Framework.Animation.Animation<double>)this._theta).value, roundMinutes: roundMinutes, radius: ((global::Doroti.Framework.Animation.Animation<double>)this._radius).value));
        if ((((_Dial__time_picker)this.widget).onChanged is null))
        {
            return current;
        }
        if ((!object.Equals(current, ((_Dial__time_picker)this.widget).selectedTime)))
        {
            ((_Dial__time_picker)this.widget).onChanged!(current);
        }
        return current;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateThetaForPan(bool roundMinutes = false)
    {
        setState(((global::System.Action)(() =>
        {
            global::Doroti.Ui.Offset offset = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(this._position) - DartRuntimePrimitives.RequireValue(this._center)));
            double labelRadius = ((DartRuntimePrimitives.RequireValue(this._dialSize).shortestSide / 2L) - Time_pickerLibrary._kTimePickerDialPadding);
            double innerRadius = (labelRadius - Time_pickerLibrary._kTimePickerInnerDialOffset);
            double angle = (((global::Doroti.Runtime.Dart_mathLibrary.atan2(offset.dx, offset.dy) - (Dart_mathLibrary.pi / 2L))) % Time_pickerLibrary._kTwoPi);
            double radiusLocal = Dart_uiLibrary.clampDouble((((offset.distance - innerRadius)) / Time_pickerLibrary._kTimePickerInnerDialOffset), 0, 1);
            if (roundMinutes)
            {
                angle = _getThetaForTime(_getTimeForTheta(angle, roundMinutes: roundMinutes, radius: radiusLocal));
            }
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.Tween<double>>)(() =>
            {
                var __cascade = this._thetaTween;
                __cascade.begin = angle;
                __cascade.end = angle;
                return __cascade;
            }))());
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.Tween<double>>)(() =>
            {
                var __cascade = this._radiusTween;
                __cascade.begin = radiusLocal;
                __cascade.end = radiusLocal;
                return __cascade;
            }))());
        })));
    }

    internal virtual void _handlePanStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !this._dragging);
        _dragging = true;
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        _position = ((Offset)((dynamic)box).globalToLocal(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition));
        _dialSize = ((global::Doroti.Framework.Rendering.RenderBox)box).size;
        _center = DartRuntimePrimitives.RequireValue(this._dialSize).center(Offset.zero);
        _updateThetaForPan();
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _handlePanUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        _position = (DartRuntimePrimitives.RequireValue(this._position) + ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta);
        _updateThetaForPan();
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _handlePanEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => this._dragging);
        _dragging = false;
        _position = null;
        _center = null;
        _dialSize = null;
        _animateTo(_getThetaForTime(((_Dial__time_picker)this.widget).selectedTime), _getRadiusForTime(((_Dial__time_picker)this.widget).selectedTime));
        if ((object.Equals(((_Dial__time_picker)this.widget).hourMinuteMode, _HourMinuteMode__time_picker.hour)))
        {
            ((_Dial__time_picker)this.widget).onHourSelected?.Invoke();
        }
    }

    internal virtual void _handleTapUp(global::Doroti.Framework.Gestures.TapUpDetails details)
    {
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        _position = ((Offset)((dynamic)box).globalToLocal(((global::Doroti.Framework.Gestures.TapUpDetails)details).globalPosition));
        _center = ((global::Doroti.Framework.Rendering.RenderBox)box).size.center(Offset.zero);
        _dialSize = ((global::Doroti.Framework.Rendering.RenderBox)box).size;
        _updateThetaForPan(roundMinutes: true);
        _notifyOnChangedIfNeeded(roundMinutes: true);
        if ((object.Equals(((_Dial__time_picker)this.widget).hourMinuteMode, _HourMinuteMode__time_picker.hour)))
        {
            ((_Dial__time_picker)this.widget).onHourSelected?.Invoke();
        }
        TimeOfDay time = ((TimeOfDay)(object?)_getTimeForTheta(((global::Doroti.Framework.Animation.Animation<double>)this._theta).value, roundMinutes: true, radius: ((global::Doroti.Framework.Animation.Animation<double>)this._radius).value));
        _animateTo(_getThetaForTime(time), _getRadiusForTime(time));
        _dragging = false;
        _position = null;
        _center = null;
        _dialSize = null;
    }

    internal virtual void _selectHour(long hour)
    {
        TimeOfDay time = default!;
        TimeOfDay getAmPmTime()
        {
            return (((_Dial__time_picker)this.widget).selectedTime.period switch { DayPeriod.am => new TimeOfDay(hour: hour, minute: ((_Dial__time_picker)this.widget).selectedTime.minute), DayPeriod.pm => new TimeOfDay(hour: (hour + TimeOfDay.hoursPerPeriod), minute: ((_Dial__time_picker)this.widget).selectedTime.minute), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (((_Dial__time_picker)this.widget).hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    switch (((_Dial__time_picker)this.widget).hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHour:
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                time = new TimeOfDay(hour: hour, minute: ((_Dial__time_picker)this.widget).selectedTime.minute);
                                break;
                            }
                        case _HourDialType__time_picker.twelveHour:
                            {
                                time = getAmPmTime();
                                break;
                            }
                    }
                    break;
                }
            case _HourMinuteMode__time_picker.minute:
                {
                    time = getAmPmTime();
                    break;
                }
        }
        double angle = _getThetaForTime(time);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.Tween<double>>)(() =>
{
    var __cascade = this._thetaTween;
    __cascade.begin = angle;
    __cascade.end = angle;
    return __cascade;
}))());
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _selectMinute(long minute)
    {
        var time = new TimeOfDay(hour: ((_Dial__time_picker)this.widget).selectedTime.hour, minute: minute);
        double angle = _getThetaForTime(time);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.Tween<double>>)(() =>
{
    var __cascade = this._thetaTween;
    __cascade.begin = angle;
    __cascade.end = angle;
    return __cascade;
}))());
        _notifyOnChangedIfNeeded();
    }

    internal virtual _TappableLabel__time_picker _buildTappableLabel(global::Doroti.Framework.Painting.TextStyle? textStyle, long selectedValue, long value, bool inner, string label, global::System.Action onTap)
    {
        return new _TappableLabel__time_picker(value: value, inner: inner, painter: ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter(text: new global::Doroti.Framework.Painting.TextSpan(style: textStyle, text: label), textDirection: TextDirection.ltr, textScaler: MediaQuery.textScalerOf(this.context).clamp(maxScaleFactor: 2.0));
    __cascade.layout();
    return __cascade;
}))(), onTap: onTap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<_TappableLabel__time_picker> _build24HourRing(global::Doroti.Framework.Painting.TextStyle? textStyle, long selectedValue)
    {
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection52599 = new List<_TappableLabel__time_picker>(); if (this.themeData.useMaterial3)
            {
                foreach (var timeOfDay in _twentyFourHours)
                {
                    __collection52599.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: (((TimeOfDay)timeOfDay).hour >= 12L), value: ((TimeOfDay)timeOfDay).hour, label: ((((TimeOfDay)timeOfDay).hour != 0L) ? this.localizations.formatDecimal(((TimeOfDay)timeOfDay).hour) : this.localizations.formatHour(timeOfDay, alwaysUse24HourFormat: true)), onTap: ((global::System.Action)(() =>
                    {
                        _selectHour(((TimeOfDay)timeOfDay).hour);
                    }))));
                }
            }
            if (!this.themeData.useMaterial3)
            {
                foreach (var timeOfDayLocal in _twentyFourHoursM2)
                {
                    __collection52599.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: false, value: ((TimeOfDay)timeOfDayLocal).hour, label: this.localizations.formatHour(timeOfDayLocal, alwaysUse24HourFormat: true), onTap: ((global::System.Action)(() =>
                    {
                        _selectHour(((TimeOfDay)timeOfDayLocal).hour);
                    }))));
                }
            }
            return __collection52599;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<_TappableLabel__time_picker> _build12HourRing(global::Doroti.Framework.Painting.TextStyle? textStyle, long selectedValue)
    {
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection53903 = new List<_TappableLabel__time_picker>(); foreach (var timeOfDay in _amHours)
            {
                __collection53903.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: false, value: ((TimeOfDay)timeOfDay).hour, label: this.localizations.formatHour(timeOfDay, alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(this.context)), onTap: ((global::System.Action)(() =>
                {
                    _selectHour(((TimeOfDay)timeOfDay).hour);
                }))));
            }
            return __collection53903;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<_TappableLabel__time_picker> _buildMinutes(global::Doroti.Framework.Painting.TextStyle? textStyle, long selectedValue)
    {
        var minuteMarkerValues = new List<TimeOfDay> { new TimeOfDay(hour: 0L, minute: 0L), new TimeOfDay(hour: 0L, minute: 5L), new TimeOfDay(hour: 0L, minute: 10L), new TimeOfDay(hour: 0L, minute: 15L), new TimeOfDay(hour: 0L, minute: 20L), new TimeOfDay(hour: 0L, minute: 25L), new TimeOfDay(hour: 0L, minute: 30L), new TimeOfDay(hour: 0L, minute: 35L), new TimeOfDay(hour: 0L, minute: 40L), new TimeOfDay(hour: 0L, minute: 45L), new TimeOfDay(hour: 0L, minute: 50L), new TimeOfDay(hour: 0L, minute: 55L) };
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection55004 = new List<_TappableLabel__time_picker>(); foreach (var timeOfDay in minuteMarkerValues)
            {
                __collection55004.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: false, value: ((TimeOfDay)timeOfDay).minute, label: this.localizations.formatMinute(timeOfDay), onTap: ((global::System.Action)(() =>
                {
                    _selectMinute(((TimeOfDay)timeOfDay).minute);
                }))));
            }
            return __collection55004;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = (theme.useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)((timePickerTheme.dialBackgroundColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dialBackgroundColor)));
        global::Doroti.Ui.Color dialHandColorLocal = ((global::Doroti.Ui.Color)(object?)((timePickerTheme.dialHandColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dialHandColor)));
        global::Doroti.Framework.Painting.TextStyle labelStyle = ((timePickerTheme.dialTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).dialTextStyle));
        global::Doroti.Ui.Color dialTextUnselectedColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(((timePickerTheme.dialTextColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dialTextColor)), new HashSet<global::Doroti.Framework.Widgets.WidgetState>()));
        global::Doroti.Ui.Color dialTextSelectedColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(((timePickerTheme.dialTextColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).dialTextColor)), new HashSet<global::Doroti.Framework.Widgets.WidgetState> { global::Doroti.Framework.Widgets.WidgetState.selected }));
        global::Doroti.Framework.Painting.TextStyle resolvedUnselectedLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)labelStyle.copyWith(color: dialTextUnselectedColor));
        global::Doroti.Framework.Painting.TextStyle resolvedSelectedLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)labelStyle.copyWith(color: dialTextSelectedColor));
        var dotColorLocal = dialTextSelectedColor;
        List<_TappableLabel__time_picker> primaryLabelsLocal = default!;
        List<_TappableLabel__time_picker> selectedLabelsLocal = default!;
        long selectedDialValue = default!;
        double radiusValue = default!;
        switch (((_Dial__time_picker)this.widget).hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    switch (((_Dial__time_picker)this.widget).hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHour:
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                selectedDialValue = ((_Dial__time_picker)this.widget).selectedTime.hour;
                                primaryLabelsLocal = _build24HourRing(textStyle: resolvedUnselectedLabelStyle, selectedValue: selectedDialValue);
                                selectedLabelsLocal = _build24HourRing(textStyle: resolvedSelectedLabelStyle, selectedValue: selectedDialValue);
                                radiusValue = (theme.useMaterial3 ? ((global::Doroti.Framework.Animation.Animation<double>)this._radius).value : 1);
                                break;
                            }
                        case _HourDialType__time_picker.twelveHour:
                            {
                                selectedDialValue = ((_Dial__time_picker)this.widget).selectedTime.hourOfPeriod;
                                primaryLabelsLocal = _build12HourRing(textStyle: resolvedUnselectedLabelStyle, selectedValue: selectedDialValue);
                                selectedLabelsLocal = _build12HourRing(textStyle: resolvedSelectedLabelStyle, selectedValue: selectedDialValue);
                                radiusValue = 1;
                                break;
                            }
                    }
                    break;
                }
            case _HourMinuteMode__time_picker.minute:
                {
                    selectedDialValue = ((_Dial__time_picker)this.widget).selectedTime.minute;
                    primaryLabelsLocal = _buildMinutes(textStyle: resolvedUnselectedLabelStyle, selectedValue: selectedDialValue);
                    selectedLabelsLocal = _buildMinutes(textStyle: resolvedSelectedLabelStyle, selectedValue: selectedDialValue);
                    radiusValue = 1;
                    break;
                }
        }
        this.painter?.dispose();
        painter = new _DialPainter__time_picker(selectedValue: selectedDialValue, primaryLabels: primaryLabelsLocal, selectedLabels: selectedLabelsLocal, backgroundColor: backgroundColorLocal, handColor: dialHandColorLocal, handWidth: ((_TimePickerDefaults__time_picker)defaultTheme).handWidth, dotColor: dotColorLocal, dotRadius: ((_TimePickerDefaults__time_picker)defaultTheme).dotRadius, centerRadius: ((_TimePickerDefaults__time_picker)defaultTheme).centerRadius, theta: ((global::Doroti.Framework.Animation.Animation<double>)this._theta).value, radius: radiusValue, textDirection: Directionality.of(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.GestureDetector(excludeFromSemantics: true, onPanStart: (global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)this._handlePanStart, onPanUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handlePanUpdate, onPanEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handlePanEnd, onTapUp: (global::System.Action<global::Doroti.Framework.Gestures.TapUpDetails>)this._handleTapUp, child: new global::Doroti.Framework.Widgets.CustomPaint(painter: this.painter)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
        _updateTickerModeNotifier();
        _updateTicker();
        return this._ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTicker);
        newNotifier.addListener(this._updateTicker);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _TimePickerInput__time_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual TimeOfDay initialSelectedTime { get; private set; } = default!;
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? hourLabelText { get; private set; }
    public virtual string? minuteLabelText { get; private set; }
    public virtual string helpText { get; private set; } = default!;
    public virtual bool? autofocusHour { get; private set; }
    public virtual bool? autofocusMinute { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _TimePickerInput__time_picker(TimeOfDay initialSelectedTime, string? errorInvalidText, string? hourLabelText, string? minuteLabelText, string helpText, bool? autofocusHour, bool? autofocusMinute, bool emptyInitialTime, string? restorationId = null)
    {
        this.initialSelectedTime = initialSelectedTime;
        this.errorInvalidText = errorInvalidText;
        this.hourLabelText = hourLabelText;
        this.minuteLabelText = minuteLabelText;
        this.helpText = helpText;
        this.autofocusHour = autofocusHour;
        this.autofocusMinute = autofocusMinute;
        this.emptyInitialTime = emptyInitialTime;
        this.restorationId = restorationId;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TimePickerInputState__time_picker());
}

internal class _TimePickerInputState__time_picker : global::Doroti.Framework.Widgets.State<_TimePickerInput__time_picker>, global::Doroti.Framework.Widgets.RestorationMixin<_TimePickerInput__time_picker>
{
    private bool __late__selectedTime_initialized;
    private RestorableTimeOfDay __late__selectedTime = default!;
    internal virtual RestorableTimeOfDay _selectedTime
    {
        get
        {
            if (!__late__selectedTime_initialized)
            {
                __late__selectedTime = new RestorableTimeOfDay(((_TimePickerInput__time_picker)this.widget).initialSelectedTime);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    public virtual global::Doroti.Framework.Widgets.RestorableBool hourHasError { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    public virtual global::Doroti.Framework.Widgets.RestorableBool minuteHasError { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        this._selectedTime.dispose();
        this.hourHasError.dispose();
        this.minuteHasError.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => ((_TimePickerInput__time_picker)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedTime), "selected_time");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this.hourHasError), "hour_has_error");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this.minuteHasError), "minute_has_error");
    }

    internal virtual long? _parseHour(string? value)
    {
        if ((value is null))
        {
            return null;
        }
        long? newHour = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));
        if ((newHour is null))
        {
            return null;
        }
        if (MediaQuery.alwaysUse24HourFormatOf(this.context))
        {
            if (((newHour >= 0L) && (DartRuntimePrimitives.RequireValue(newHour) < 24L)))
            {
                return DartRuntimePrimitives.RequireValue(newHour);
            }
        }
        else
        {
            if (((DartRuntimePrimitives.RequireValue(newHour) > 0L) && (DartRuntimePrimitives.RequireValue(newHour) < 13L)))
            {
                if (((((object.Equals(this._selectedTime.value.period, DayPeriod.pm)) && (DartRuntimePrimitives.RequireValue(newHour) != 12L))) || (((object.Equals(this._selectedTime.value.period, DayPeriod.am)) && (DartRuntimePrimitives.RequireValue(newHour) == 12L)))))
                {
                    newHour = (((DartRuntimePrimitives.RequireValue(newHour) + TimeOfDay.hoursPerPeriod)) % TimeOfDay.hoursPerDay);
                }
                return DartRuntimePrimitives.RequireValue(newHour);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _parseMinute(string? value)
    {
        if ((value is null))
        {
            return null;
        }
        long? newMinute = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));
        if ((newMinute is null))
        {
            return null;
        }
        if (((newMinute >= 0L) && (DartRuntimePrimitives.RequireValue(newMinute) < 60L)))
        {
            return DartRuntimePrimitives.RequireValue(newMinute);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleHourSavedSubmitted(string? value)
    {
        long? newHour = _parseHour(value);
        if ((newHour is not null))
        {
            long newHour__62523__value62560 = DartRuntimePrimitives.RequireValue(newHour);
            this._selectedTime.value = new TimeOfDay(hour: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newHour__62523__value62560)), minute: this._selectedTime.value.minute);
            _TimePickerModel__time_picker.setSelectedTime(this.context, this._selectedTime.value);
            FocusScope.of(this.context).requestFocus();
        }
    }

    internal virtual void _handleHourChanged(string value)
    {
        long? newHour = _parseHour(value);
        if (((newHour is not null) && (value.Length == 2L)))
        {
            long newHour__62852__value62889 = DartRuntimePrimitives.RequireValue(newHour);
            FocusScope.of(this.context).nextFocus();
        }
    }

    internal virtual void _handleMinuteSavedSubmitted(string? value)
    {
        long? newMinute = _parseMinute(value);
        if ((newMinute is not null))
        {
            long newMinute__63120__value63161 = DartRuntimePrimitives.RequireValue(newMinute);
            this._selectedTime.value = new TimeOfDay(hour: this._selectedTime.value.hour, minute: Dart_coreLibrary.parse(value!));
            _TimePickerModel__time_picker.setSelectedTime(this.context, this._selectedTime.value);
            FocusScope.of(this.context).unfocus();
        }
    }

    internal virtual void _handleDayPeriodChanged(TimeOfDay value)
    {
        this._selectedTime.value = value;
        _TimePickerModel__time_picker.setSelectedTime(this.context, this._selectedTime.value);
    }

    internal virtual string? _validateHour(string? value)
    {
        long? newHour = _parseHour(value);
        setState(((global::System.Action)(() =>
        {
            this.hourHasError.value = (newHour is null);
        })));
        return ((newHour is null) ? "" : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateMinute(string? value)
    {
        long? newMinute = _parseMinute(value);
        setState(((global::System.Action)(() =>
        {
            this.minuteHasError.value = (newMinute is null);
        })));
        return ((newMinute is null) ? "" : null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        TimeOfDayFormat timeOfDayFormatLocal = MaterialLocalizations.of(context).timeOfDayFormat(alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        var use24HourDials = (!object.Equals(TimeLibrary.hourFormat(of: timeOfDayFormatLocal), HourFormat.h));
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = ((TimePickerThemeData)(object?)_TimePickerModel__time_picker.themeOf(context));
        _TimePickerDefaults__time_picker defaultTheme = ((_TimePickerDefaults__time_picker)(object?)_TimePickerModel__time_picker.defaultThemeOf(context));
        global::Doroti.Framework.Painting.TextStyle hourMinuteStyle = ((timePickerTheme.hourMinuteTextStyle ?? (global::Doroti.Framework.Painting.TextStyle)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteTextStyle));
        double minInteractiveVerticalPadding = Math.Max(0, ((2L * global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension) - ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodInputSize.height));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: (_TimePickerModel__time_picker.useMaterial3Of(context) ? global::Doroti.Framework.Painting.EdgeInsets.zero : global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16)), child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection65491 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection65491.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(bottom: (((_TimePickerModel__time_picker.useMaterial3Of(context) ? 20L : 24L)) - (minInteractiveVerticalPadding / 2L))), child: new global::Doroti.Framework.Widgets.Text(((_TimePickerInput__time_picker)this.widget).helpText, style: (_TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? _TimePickerModel__time_picker.defaultThemeOf(context).helpTextStyle))))); __collection65491.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection66075 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((!use24HourDials && (object.Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm)))) { __collection66075.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: 12), child: new _DayPeriodControl__time_picker(onPeriodChanged: (global::System.Action<TimeOfDay>)this._handleDayPeriodChanged))) }); } __collection66075.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(top: (minInteractiveVerticalPadding / 2L)), child: new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, textDirection: TextDirection.ltr, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection67027 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection67027.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: 10), child: new _HourTextField__time_picker(restorationId: "hour_text_field", selectedTime: this._selectedTime.value, style: hourMinuteStyle, autofocus: ((_TimePickerInput__time_picker)this.widget).autofocusHour, inputAction: global::Doroti.Framework.Services.TextInputAction.next, validator: (global::System.Func<string?, string?>)this._validateHour, onSavedSubmitted: (global::System.Action<string?>)this._handleHourSavedSubmitted, onChanged: (global::System.Action<string>)this._handleHourChanged, hourLabelText: ((_TimePickerInput__time_picker)this.widget).hourLabelText, emptyInitialTime: ((_TimePickerInput__time_picker)this.widget).emptyInitialTime)))); if ((!this.hourHasError.value && !this.minuteHasError.value)) { __collection67027.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Text((((_TimePickerInput__time_picker)this.widget).hourLabelText ?? MaterialLocalizations.of(context).timePickerHourLabel), style: theme.textTheme.bodySmall, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis)))); } return __collection67027; }))()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Column(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection68842 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection68842.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: 10), child: new _MinuteTextField__time_picker(restorationId: "minute_text_field", selectedTime: this._selectedTime.value, style: hourMinuteStyle, autofocus: ((_TimePickerInput__time_picker)this.widget).autofocusMinute, inputAction: global::Doroti.Framework.Services.TextInputAction.done, validator: (global::System.Func<string?, string?>)this._validateMinute, onSavedSubmitted: (global::System.Action<string?>)this._handleMinuteSavedSubmitted, minuteLabelText: ((_TimePickerInput__time_picker)this.widget).minuteLabelText, emptyInitialTime: ((_TimePickerInput__time_picker)this.widget).emptyInitialTime)))); if ((!this.hourHasError.value && !this.minuteHasError.value)) { __collection68842.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Text((((_TimePickerInput__time_picker)this.widget).minuteLabelText ?? MaterialLocalizations.of(context).timePickerMinuteLabel), style: theme.textTheme.bodySmall, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis)))); } return __collection68842; }))()))) }))))); if ((!use24HourDials && (!object.Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm)))) { __collection66075.AddRange(new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 12), child: new _DayPeriodControl__time_picker(onPeriodChanged: (global::System.Action<TimeOfDay>)this._handleDayPeriodChanged))) }); } return __collection66075; }))()))); if ((this.hourHasError.value || this.minuteHasError.value)) { __collection65491.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Text((((_TimePickerInput__time_picker)this.widget).errorInvalidText ?? MaterialLocalizations.of(context).invalidTimeLabel), style: theme.textTheme.bodyMedium!.copyWith(color: theme.colorScheme.error)))); } else { __collection65491.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: 2))); } return __collection65491; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public override void didUpdateWidget(_TimePickerInput__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

internal class _HourTextField__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputAction inputAction { get; private set; } = default!;
    public virtual global::System.Func<string?, string?> validator { get; private set; } = default!;
    public virtual global::System.Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual global::System.Action<string> onChanged { get; private set; } = default!;
    public virtual string? hourLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _HourTextField__time_picker(TimeOfDay selectedTime, global::Doroti.Framework.Painting.TextStyle style, bool? autofocus, global::Doroti.Framework.Services.TextInputAction inputAction, global::System.Func<string?, string?> validator, global::System.Action<string?> onSavedSubmitted, global::System.Action<string> onChanged, string? hourLabelText, bool emptyInitialTime, string? restorationId = null)
    {
        this.selectedTime = selectedTime;
        this.style = style;
        this.autofocus = autofocus;
        this.inputAction = inputAction;
        this.validator = validator;
        this.onSavedSubmitted = onSavedSubmitted;
        this.onChanged = onChanged;
        this.hourLabelText = hourLabelText;
        this.emptyInitialTime = emptyInitialTime;
        this.restorationId = restorationId;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _HourMinuteTextField__time_picker(restorationId: this.restorationId, selectedTime: this.selectedTime, isHour: true, autofocus: this.autofocus, inputAction: this.inputAction, style: this.style, semanticHintText: (this.hourLabelText ?? MaterialLocalizations.of(context).timePickerHourLabel), validator: (global::System.Func<string?, string?>)this.validator, onSavedSubmitted: (global::System.Action<string?>)this.onSavedSubmitted, emptyInitialTime: this.emptyInitialTime, onChanged: (global::System.Action<string>)this.onChanged));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MinuteTextField__time_picker : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputAction inputAction { get; private set; } = default!;
    public virtual global::System.Func<string?, string?> validator { get; private set; } = default!;
    public virtual global::System.Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual string? minuteLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _MinuteTextField__time_picker(TimeOfDay selectedTime, global::Doroti.Framework.Painting.TextStyle style, bool? autofocus, global::Doroti.Framework.Services.TextInputAction inputAction, global::System.Func<string?, string?> validator, global::System.Action<string?> onSavedSubmitted, string? minuteLabelText, bool emptyInitialTime, string? restorationId = null)
    {
        this.selectedTime = selectedTime;
        this.style = style;
        this.autofocus = autofocus;
        this.inputAction = inputAction;
        this.validator = validator;
        this.onSavedSubmitted = onSavedSubmitted;
        this.minuteLabelText = minuteLabelText;
        this.emptyInitialTime = emptyInitialTime;
        this.restorationId = restorationId;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _HourMinuteTextField__time_picker(restorationId: this.restorationId, selectedTime: this.selectedTime, isHour: false, autofocus: this.autofocus, inputAction: this.inputAction, style: this.style, semanticHintText: (this.minuteLabelText ?? MaterialLocalizations.of(context).timePickerMinuteLabel), validator: (global::System.Func<string?, string?>)this.validator, emptyInitialTime: this.emptyInitialTime, onSavedSubmitted: (global::System.Action<string?>)this.onSavedSubmitted));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HourMinuteTextField__time_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual bool isHour { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual global::Doroti.Framework.Services.TextInputAction inputAction { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle style { get; private set; } = default!;
    public virtual string semanticHintText { get; private set; } = default!;
    public virtual global::System.Func<string?, string?> validator { get; private set; } = default!;
    public virtual global::System.Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual global::System.Action<string>? onChanged { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _HourMinuteTextField__time_picker(TimeOfDay selectedTime, bool isHour, bool? autofocus, global::Doroti.Framework.Services.TextInputAction inputAction, global::Doroti.Framework.Painting.TextStyle style, string semanticHintText, global::System.Func<string?, string?> validator, global::System.Action<string?> onSavedSubmitted, string? restorationId = null, bool emptyInitialTime = default!, global::System.Action<string>? onChanged = null)
    {
        this.selectedTime = selectedTime;
        this.isHour = isHour;
        this.autofocus = autofocus;
        this.inputAction = inputAction;
        this.style = style;
        this.semanticHintText = semanticHintText;
        this.validator = validator;
        this.onSavedSubmitted = onSavedSubmitted;
        this.restorationId = restorationId;
        this.emptyInitialTime = emptyInitialTime;
        this.onChanged = onChanged;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _HourMinuteTextFieldState__time_picker());
}

internal class _HourMinuteTextFieldState__time_picker : global::Doroti.Framework.Widgets.State<_HourMinuteTextField__time_picker>, global::Doroti.Framework.Widgets.RestorationMixin<_HourMinuteTextField__time_picker>
{
    public virtual global::Doroti.Framework.Widgets.RestorableTextEditingController controller { get; private set; } = global::Doroti.Framework.Widgets.RestorableTextEditingController.Create();
    public virtual global::Doroti.Framework.Widgets.RestorableBool controllerHasBeenSet { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBool(false);
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode { get; set; } = default!;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public override void initState()
    {
        base.initState();
        focusNode = ((Func<global::Doroti.Framework.Widgets.FocusNode>)(() =>
{
    var __cascade = new global::Doroti.Framework.Widgets.FocusNode();
    __cascade.addListener(((global::System.Action)(() =>
    {
        setState(((global::System.Action)(() =>
        {
            if (((global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb && ((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus) && (global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus?.context is not null)))
            {
                Actions.maybeInvoke(global::Doroti.Framework.Widgets.Focus_managerLibrary.primaryFocus!.context!, new global::Doroti.Framework.Widgets.SelectAllTextIntent(global::Doroti.Framework.Services.SelectionChangedCause.keyboard));
            }
        })));
    })));
    return __cascade;
}))();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
        if (!this.controllerHasBeenSet.value)
        {
            this.controllerHasBeenSet.value = true;
            string initialTextValue = (((_HourMinuteTextField__time_picker)this.widget).emptyInitialTime ? "" : this._formattedValue);
            this.controller.value.value = new global::Doroti.Framework.Services.TextEditingValue(text: initialTextValue);
        }
    }

    public override void dispose()
    {
        this.controller.dispose();
        this.controllerHasBeenSet.dispose();
        this.focusNode.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => ((_HourMinuteTextField__time_picker)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this.controller), "text_editing_controller");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this.controllerHasBeenSet), "has_controller_been_set");
    }

    internal virtual string _formattedValue
    {
        get
        {
            bool alwaysUse24HourFormatLocal = MediaQuery.alwaysUse24HourFormatOf(this.context);
            MaterialLocalizations localizations = MaterialLocalizations.of(this.context);
            return (!((_HourMinuteTextField__time_picker)this.widget).isHour ? localizations.formatMinute(((_HourMinuteTextField__time_picker)this.widget).selectedTime) : localizations.formatHour(((_HourMinuteTextField__time_picker)this.widget).selectedTime, alwaysUse24HourFormat: alwaysUse24HourFormatLocal));
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = (theme.useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
        bool alwaysUse24HourFormat = MediaQuery.alwaysUse24HourFormatOf(context);
        InputDecorationThemeData inputDecorationThemeLocal = ((timePickerTheme.inputDecorationTheme ?? (InputDecorationThemeData)((_TimePickerDefaults__time_picker)defaultTheme).inputDecorationTheme));
        InputDecoration inputDecoration = new InputDecoration(errorStyle: ((_TimePickerDefaults__time_picker)defaultTheme).inputDecorationTheme.errorStyle).applyDefaults(inputDecorationThemeLocal);
        string? hintTextLocal = ((((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus || ((_HourMinuteTextField__time_picker)this.widget).emptyInitialTime) ? null : this._formattedValue);
        global::Doroti.Ui.Color startingFillColor = ((global::Doroti.Ui.Color)(object?)(((timePickerTheme.inputDecorationTheme?.fillColor ?? timePickerTheme.hourMinuteColor) ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteColor)));
        global::Doroti.Ui.Color fillColorLocal = default!;
        if (theme.useMaterial3)
        {
            fillColorLocal = WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(startingFillColor, ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection78509 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus) { __collection78509.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } if (((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus) { __collection78509.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection78509; }))());
        }
        else
        {
            fillColorLocal = (((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus ? Colors.transparent : startingFillColor);
        }
        inputDecoration = inputDecoration.copyWith(hintText: hintTextLocal, fillColor: fillColorLocal);
        var states = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection78850 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus) { __collection78850.Add(global::Doroti.Framework.Widgets.WidgetState.focused); } if (((global::Doroti.Framework.Widgets.FocusNode)this.focusNode).hasFocus) { __collection78850.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection78850; }))();
        global::Doroti.Ui.Color effectiveTextColor = ((global::Doroti.Ui.Color)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color>(((timePickerTheme.hourMinuteTextColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteTextColor)), states));
        global::Doroti.Framework.Painting.TextStyle effectiveStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.TextStyle>(((_HourMinuteTextField__time_picker)this.widget).style, states).copyWith(color: effectiveTextColor));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: (alwaysUse24HourFormat ? ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteInputSize24Hour : ((_TimePickerDefaults__time_picker)defaultTheme).hourMinuteInputSize), child: MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.UnmanagedRestorationScope(bucket: this.bucket, child: new global::Doroti.Framework.Widgets.Semantics(label: ((_HourMinuteTextField__time_picker)this.widget).semanticHintText, child: new TextFormField(restorationId: "hour_minute_text_form_field", autofocus: (((_HourMinuteTextField__time_picker)this.widget).autofocus ?? false), expands: true, maxLines: null, inputFormatters: new List<global::Doroti.Framework.Services.TextInputFormatter> { new global::Doroti.Framework.Services.LengthLimitingTextInputFormatter(2L) }, focusNode: this.focusNode, textAlign: global::Doroti.Ui.TextAlign.center, textInputAction: ((_HourMinuteTextField__time_picker)this.widget).inputAction, keyboardType: global::Doroti.Framework.Services.TextInputType.number, style: effectiveStyle, controller: this.controller.value, decoration: inputDecoration, validator: (global::System.Func<string?, string?>)((_HourMinuteTextField__time_picker)this.widget).validator, onEditingComplete: ((global::System.Action)(() => { this.widget.onSavedSubmitted(this.controller.value.text); })), onSaved: (global::System.Action<string?>)((_HourMinuteTextField__time_picker)this.widget).onSavedSubmitted, onFieldSubmitted: (global::System.Action<string?>)((_HourMinuteTextField__time_picker)this.widget).onSavedSubmitted, onChanged: (global::System.Action<string>?)((_HourMinuteTextField__time_picker)this.widget).onChanged))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public override void didUpdateWidget(_HourMinuteTextField__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public delegate void EntryModeChangeCallback(TimePickerEntryMode mode);

public class TimePickerDialog : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual TimeOfDay initialTime { get; private set; } = default!;
    public virtual string? cancelText { get; private set; }
    public virtual string? confirmText { get; private set; }
    public virtual string? helpText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? hourLabelText { get; private set; }
    public virtual string? minuteLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual TimePickerEntryMode initialEntryMode { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Orientation? orientation { get; private set; }
    public virtual global::System.Action<TimePickerEntryMode>? onEntryModeChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToTimerEntryModeIcon { get; private set; }
    public virtual bool emptyInitialInput { get; private set; } = default!;

    public TimePickerDialog(global::Doroti.Framework.Foundation.Key? key = null, TimeOfDay initialTime = default!, string? cancelText = null, string? confirmText = null, string? helpText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, string? restorationId = null, TimePickerEntryMode initialEntryMode = TimePickerEntryMode.dial, global::Doroti.Framework.Widgets.Orientation? orientation = null, global::System.Action<TimePickerEntryMode>? onEntryModeChanged = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = false) : base(key: key)
    {
        this.initialTime = initialTime;
        this.cancelText = cancelText;
        this.confirmText = confirmText;
        this.helpText = helpText;
        this.errorInvalidText = errorInvalidText;
        this.hourLabelText = hourLabelText;
        this.minuteLabelText = minuteLabelText;
        this.restorationId = restorationId;
        this.initialEntryMode = initialEntryMode;
        this.orientation = orientation;
        this.onEntryModeChanged = onEntryModeChanged;
        this.switchToInputEntryModeIcon = switchToInputEntryModeIcon;
        this.switchToTimerEntryModeIcon = switchToTimerEntryModeIcon;
        this.emptyInitialInput = emptyInitialInput;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TimePickerDialogState__time_picker());
}

internal class _TimePickerDialogState__time_picker : global::Doroti.Framework.Widgets.State<TimePickerDialog>, global::Doroti.Framework.Widgets.RestorationMixin<TimePickerDialog>
{
    private bool __late__entryMode_initialized;
    private global::Doroti.Framework.Widgets.RestorableEnum<TimePickerEntryMode> __late__entryMode = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableEnum<TimePickerEntryMode> _entryMode
    {
        get
        {
            if (!__late__entryMode_initialized)
            {
                __late__entryMode = new global::Doroti.Framework.Widgets.RestorableEnum<TimePickerEntryMode>(((TimePickerDialog)this.widget).initialEntryMode, values: System.Enum.GetValues<TimePickerEntryMode>().ToList().Cast<TimePickerEntryMode>());
                __late__entryMode_initialized = true;
            }
            return __late__entryMode;
        }
    }
    private bool __late__selectedTime_initialized;
    private RestorableTimeOfDay __late__selectedTime = default!;
    internal virtual RestorableTimeOfDay _selectedTime
    {
        get
        {
            if (!__late__selectedTime_initialized)
            {
                __late__selectedTime = new RestorableTimeOfDay(((TimePickerDialog)this.widget).initialTime);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState> _formKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>.Create();
    internal virtual global::Doroti.Framework.Widgets.RestorableEnum<global::Doroti.Framework.Widgets.AutovalidateMode> _autovalidateMode { get; private set; } = new global::Doroti.Framework.Widgets.RestorableEnum<global::Doroti.Framework.Widgets.AutovalidateMode>(global::Doroti.Framework.Widgets.AutovalidateMode.disabled, values: System.Enum.GetValues<global::Doroti.Framework.Widgets.AutovalidateMode>().ToList().Cast<global::Doroti.Framework.Widgets.AutovalidateMode>());
    private bool __late__orientation_initialized;
    private global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation> __late__orientation = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation> _orientation
    {
        get
        {
            if (!__late__orientation_initialized)
            {
                __late__orientation = new global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation>(((TimePickerDialog)this.widget).orientation, values: System.Enum.GetValues<global::Doroti.Framework.Widgets.Orientation>().ToList().Cast<global::Doroti.Framework.Widgets.Orientation>());
                __late__orientation_initialized = true;
            }
            return __late__orientation;
        }
    }
    internal static Size _kTimePickerPortraitSize = new global::Doroti.Ui.Size(310, 468);
    internal static Size _kTimePickerLandscapeSize = new global::Doroti.Ui.Size(524, 342);
    internal static Size _kTimePickerLandscapeSizeM2 = new global::Doroti.Ui.Size(508, 300);
    internal static Size _kTimePickerInputSize = new global::Doroti.Ui.Size(312, 252);
    internal const double _kTimePickerInputMinimumHeight = 216;
    internal static Size _kTimePickerMinPortraitSize = new global::Doroti.Ui.Size(238, 326);
    internal static Size _kTimePickerMinLandscapeSize = new global::Doroti.Ui.Size(416, 248);
    internal static Size _kTimePickerMinInputSize = new global::Doroti.Ui.Size(312, 196);
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        this._selectedTime.dispose();
        this._entryMode.dispose();
        this._autovalidateMode.dispose();
        this._orientation.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => ((TimePickerDialog)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedTime), "selected_time");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._entryMode), "entry_mode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autovalidateMode), "autovalidate_mode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._orientation), "orientation");
    }

    internal virtual void _handleTimeChanged(TimeOfDay value)
    {
        if ((!object.Equals(value, this._selectedTime.value)))
        {
            setState(((global::System.Action)(() =>
            {
                this._selectedTime.value = value;
            })));
        }
    }

    internal virtual void _handleEntryModeChanged(TimePickerEntryMode value)
    {
        if ((!object.Equals(value, this._entryMode.value)))
        {
            setState(((global::System.Action)(() =>
            {
                switch (this._entryMode.value)
                {
                    case TimePickerEntryMode.dial:
                        {
                            this._autovalidateMode.value = global::Doroti.Framework.Widgets.AutovalidateMode.disabled;
                            break;
                        }
                    case TimePickerEntryMode.input:
                        {
                            ((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>)this._formKey).currentState!.save();
                            break;
                        }
                    case TimePickerEntryMode.dialOnly:
                        {
                            break;
                        }
                    case TimePickerEntryMode.inputOnly:
                        {
                            break;
                        }
                }
                this._entryMode.value = value;
                ((TimePickerDialog)this.widget).onEntryModeChanged?.Invoke(value);
            })));
        }
    }

    internal virtual void _toggleEntryMode()
    {
        switch (this._entryMode.value)
        {
            case TimePickerEntryMode.dial:
                {
                    _handleEntryModeChanged(TimePickerEntryMode.input);
                    break;
                }
            case TimePickerEntryMode.input:
                {
                    _handleEntryModeChanged(TimePickerEntryMode.dial);
                    break;
                }
            case TimePickerEntryMode.dialOnly:
            case TimePickerEntryMode.inputOnly:
                {
                    global::Doroti.Framework.Foundation.FlutterError.Create($"Can not change entry mode from {this._entryMode}");
                    break;
                }
        }
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(this.context);
    }

    internal virtual void _handleOk()
    {
        if (((object.Equals(this._entryMode.value, TimePickerEntryMode.input)) || (object.Equals(this._entryMode.value, TimePickerEntryMode.inputOnly))))
        {
            global::Doroti.Framework.Widgets.FormState form = ((global::Doroti.Framework.Widgets.GlobalKey<global::Doroti.Framework.Widgets.FormState>)this._formKey).currentState!;
            if (!form.validate())
            {
                setState(((global::System.Action)(() =>
                {
                    this._autovalidateMode.value = global::Doroti.Framework.Widgets.AutovalidateMode.always;
                })));
                return;
            }
            form.save();
        }
        Navigator.pop<object>(this.context, this._selectedTime.value);
    }

    internal virtual global::Doroti.Ui.Size _minDialogSize(global::Doroti.Framework.Widgets.BuildContext context, bool useMaterial3)
    {
        global::Doroti.Framework.Widgets.Orientation orientation = ((this._orientation.value ?? (global::Doroti.Framework.Widgets.Orientation)MediaQuery.orientationOf(context)));
        switch (this._entryMode.value)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    return (orientation switch { global::Doroti.Framework.Widgets.Orientation.portrait => _kTimePickerMinPortraitSize, global::Doroti.Framework.Widgets.Orientation.landscape => _kTimePickerMinLandscapeSize, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    MaterialLocalizations localizations = MaterialLocalizations.of(context);
                    TimeOfDayFormat timeOfDayFormatLocal = localizations.timeOfDayFormat(alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context));
                    double timePickerWidth = default!;
                    switch (timeOfDayFormatLocal)
                    {
                        case TimeOfDayFormat.HH_colon_mm:
                        case TimeOfDayFormat.HH_dot_mm:
                        case TimeOfDayFormat.frenchCanadian:
                        case TimeOfDayFormat.H_colon_mm:
                            {
                                _TimePickerDefaults__time_picker defaultTheme = (useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
                                timePickerWidth = ((_kTimePickerMinInputSize.width - ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodPortraitSize.width) - 12L);
                                break;
                            }
                        case TimeOfDayFormat.a_space_h_colon_mm:
                        case TimeOfDayFormat.h_colon_mm_space_a:
                            {
                                timePickerWidth = (_kTimePickerMinInputSize.width - ((useMaterial3 ? 32L : 0L)));
                                break;
                            }
                    }
                    return new global::Doroti.Ui.Size(timePickerWidth, _kTimePickerMinInputSize.height);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _dialogSize(global::Doroti.Framework.Widgets.BuildContext context, bool useMaterial3)
    {
        global::Doroti.Framework.Widgets.Orientation orientation = ((this._orientation.value ?? (global::Doroti.Framework.Widgets.Orientation)MediaQuery.orientationOf(context)));
        var fontSizeToScale = 14.0;
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 1.1).scale(fontSizeToScale) / fontSizeToScale);
        global::Doroti.Ui.Size timePickerSize = default!;
        switch (this._entryMode.value)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    switch (orientation)
                    {
                        case global::Doroti.Framework.Widgets.Orientation.portrait:
                            {
                                timePickerSize = _kTimePickerPortraitSize;
                                break;
                            }
                        case global::Doroti.Framework.Widgets.Orientation.landscape:
                            {
                                timePickerSize = new global::Doroti.Ui.Size((_kTimePickerLandscapeSize.width * textScaleFactor), (useMaterial3 ? _kTimePickerLandscapeSize.height : _kTimePickerLandscapeSizeM2.height));
                                break;
                            }
                    }
                    break;
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    MaterialLocalizations localizations = MaterialLocalizations.of(context);
                    TimeOfDayFormat timeOfDayFormatLocal = localizations.timeOfDayFormat(alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context));
                    double timePickerWidth = default!;
                    switch (timeOfDayFormatLocal)
                    {
                        case TimeOfDayFormat.HH_colon_mm:
                        case TimeOfDayFormat.HH_dot_mm:
                        case TimeOfDayFormat.frenchCanadian:
                        case TimeOfDayFormat.H_colon_mm:
                            {
                                _TimePickerDefaults__time_picker defaultTheme = (useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
                                timePickerWidth = ((_kTimePickerInputSize.width - ((_TimePickerDefaults__time_picker)defaultTheme).dayPeriodPortraitSize.width) - 12L);
                                break;
                            }
                        case TimeOfDayFormat.a_space_h_colon_mm:
                        case TimeOfDayFormat.h_colon_mm_space_a:
                            {
                                timePickerWidth = (_kTimePickerInputSize.width - ((useMaterial3 ? 32L : 0L)));
                                break;
                            }
                    }
                    timePickerSize = new global::Doroti.Ui.Size(timePickerWidth, _kTimePickerInputSize.height);
                    break;
                }
        }
        return new global::Doroti.Ui.Size(timePickerSize.width, (timePickerSize.height * textScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme = Theme.of(context);
        TimePickerThemeData pickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = (theme.useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context) : new _TimePickerDefaultsM2__time_picker(context));
        global::Doroti.Framework.Painting.ShapeBorder shapeLocal = ((pickerTheme.shape ?? (global::Doroti.Framework.Painting.ShapeBorder)((_TimePickerDefaults__time_picker)defaultTheme).shape));
        global::Doroti.Ui.Color entryModeIconColorLocal = ((global::Doroti.Ui.Color)(object?)((pickerTheme.entryModeIconColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).entryModeIconColor)));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget actions = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (theme.useMaterial3 ? 0 : 4)), child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection92775 = new List<global::Doroti.Framework.Widgets.Widget>(); if (((object.Equals(this._entryMode.value, TimePickerEntryMode.dial)) || (object.Equals(this._entryMode.value, TimePickerEntryMode.input)))) { __collection92775.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(color: (theme.useMaterial3 ? null : entryModeIconColorLocal), style: (theme.useMaterial3 ? IconButton.styleFrom(foregroundColor: entryModeIconColorLocal) : null), onPressed: this._toggleEntryMode, icon: ((object.Equals(this._entryMode.value, TimePickerEntryMode.dial)) ? (((TimePickerDialog)this.widget).switchToInputEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.keyboard_outlined)) : (((TimePickerDialog)this.widget).switchToTimerEntryModeIcon ?? new global::Doroti.Framework.Widgets.Icon(Icons.access_time))), tooltip: ((object.Equals(this._entryMode.value, TimePickerEntryMode.dial)) ? MaterialLocalizations.of(context).inputTimeModeButtonLabel : MaterialLocalizations.of(context).dialModeButtonLabel)))); } __collection92775.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: 36), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Framework.Widgets.OverflowBar(spacing: 8, overflowAlignment: global::Doroti.Framework.Widgets.OverflowBarAlignment.end, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(style: ((pickerTheme.cancelButtonStyle ?? (ButtonStyle)((_TimePickerDefaults__time_picker)defaultTheme).cancelButtonStyle)), onPressed: () => this._handleCancel(), child: new global::Doroti.Framework.Widgets.Text((((TimePickerDialog)this.widget).cancelText ?? ((theme.useMaterial3 ? localizations.cancelButtonLabel : localizations.cancelButtonLabel.toUpperCase())))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new TextButton(style: ((pickerTheme.confirmButtonStyle ?? (ButtonStyle)((_TimePickerDefaults__time_picker)defaultTheme).confirmButtonStyle)), onPressed: () => this._handleOk(), child: new global::Doroti.Framework.Widgets.Text((((TimePickerDialog)this.widget).confirmText ?? localizations.okButtonLabel)))) })))))); return __collection92775; }))())));
        global::Doroti.Ui.Offset tapTargetSizeOffset = ((global::Doroti.Ui.Offset)(object?)(theme.materialTapTargetSize switch { var __constant95285 when (object.Equals(__constant95285, MaterialTapTargetSize.padded)) => Offset.zero, var __constant95381 when (object.Equals(__constant95381, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Ui.Offset(0, -12), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        global::Doroti.Ui.Size dialogSize = ((global::Doroti.Ui.Size)(object?)(_dialogSize(context, useMaterial3: theme.useMaterial3) + tapTargetSizeOffset));
        global::Doroti.Ui.Size minDialogSize = ((global::Doroti.Ui.Size)(object?)(_minDialogSize(context, useMaterial3: theme.useMaterial3) + tapTargetSizeOffset));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new Dialog(shape: shapeLocal, elevation: ((pickerTheme.elevation ?? (double)((_TimePickerDefaults__time_picker)defaultTheme).elevation)), backgroundColor: ((pickerTheme.backgroundColor ?? (Color)((_TimePickerDefaults__time_picker)defaultTheme).backgroundColor)), insetPadding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 16, vertical: ((((object.Equals(this._entryMode.value, TimePickerEntryMode.input)) || (object.Equals(this._entryMode.value, TimePickerEntryMode.inputOnly)))) ? 0 : 24)), child: new global::Doroti.Framework.Widgets.Padding(padding: ((pickerTheme.padding ?? (global::Doroti.Framework.Painting.EdgeInsetsGeometry)((_TimePickerDefaults__time_picker)defaultTheme).padding)), child: new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            global::Doroti.Ui.Size constrainedSize = ((global::Doroti.Ui.Size)(object?)constraints.constrain(dialogSize));
            var allowedSize = new global::Doroti.Ui.Size(((constrainedSize.width < minDialogSize.width) ? minDialogSize.width : constrainedSize.width), ((constrainedSize.height < minDialogSize.height) ? minDialogSize.height : constrainedSize.height));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SingleChildScrollView(restorationId: "time_picker_scroll_view_horizontal", scrollDirection: global::Doroti.Framework.Painting.Axis.horizontal, child: new global::Doroti.Framework.Widgets.SingleChildScrollView(restorationId: "time_picker_scroll_view_vertical", child: new global::Doroti.Framework.Widgets.AnimatedContainer(width: allowedSize.width, duration: Time_pickerLibrary._kDialogSizeAnimationDuration, curve: global::Doroti.Framework.Animation.Curves.easeIn, constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: _kTimePickerInputMinimumHeight, maxHeight: allowedSize.height), child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.spaceBetween, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.start, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) => {
global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Form(key: this._formKey, autovalidateMode: this._autovalidateMode.value, child: new _TimePicker__time_picker(time: ((TimePickerDialog)this.widget).initialTime, onTimeChanged: (global::System.Action<TimeOfDay>)this._handleTimeChanged, helpText: ((TimePickerDialog)this.widget).helpText, cancelText: ((TimePickerDialog)this.widget).cancelText, confirmText: ((TimePickerDialog)this.widget).confirmText, errorInvalidText: ((TimePickerDialog)this.widget).errorInvalidText, hourLabelText: ((TimePickerDialog)this.widget).hourLabelText, minuteLabelText: ((TimePickerDialog)this.widget).minuteLabelText, restorationId: "time_picker", entryMode: DartRuntimePrimitives.RequireValue(this._entryMode.value), orientation: ((TimePickerDialog)this.widget).orientation, onEntryModeChanged: (global::System.Action<TimePickerEntryMode>)this._handleEntryModeChanged, switchToInputEntryModeIcon: ((TimePickerDialog)this.widget).switchToInputEntryModeIcon, switchToTimerEntryModeIcon: ((TimePickerDialog)this.widget).switchToTimerEntryModeIcon, emptyInitialInput: ((TimePickerDialog)this.widget).emptyInitialInput)));
if (((!object.Equals(this._entryMode.value, TimePickerEntryMode.input)) && (!object.Equals(this._entryMode.value, TimePickerEntryMode.inputOnly))))
{
    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Flexible(child: childLocal));
}
return childLocal;
throw new InvalidOperationException("Dart closure completed without a value.");
})))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(actions) })))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public override void didUpdateWidget(TimePickerDialog oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        this._currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public class _TimePicker__time_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual string? helpText { get; private set; }
    public virtual string? cancelText { get; private set; }
    public virtual string? confirmText { get; private set; }
    public virtual string? errorInvalidText { get; private set; }
    public virtual string? hourLabelText { get; private set; }
    public virtual string? minuteLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual TimePickerEntryMode entryMode { get; private set; } = default!;
    public virtual TimeOfDay time { get; private set; } = default!;
    public virtual global::System.Action<TimeOfDay>? onTimeChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Orientation? orientation { get; private set; }
    public virtual global::System.Action<TimePickerEntryMode>? onEntryModeChanged { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Icon? switchToTimerEntryModeIcon { get; private set; }
    public virtual bool emptyInitialInput { get; private set; } = default!;

    internal _TimePicker__time_picker(TimeOfDay time, global::System.Action<TimeOfDay>? onTimeChanged, string? helpText = null, string? cancelText = null, string? confirmText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, string? restorationId = null, TimePickerEntryMode entryMode = TimePickerEntryMode.dial, global::Doroti.Framework.Widgets.Orientation? orientation = null, global::System.Action<TimePickerEntryMode>? onEntryModeChanged = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = default!)
    {
        this.time = time;
        this.onTimeChanged = onTimeChanged;
        this.helpText = helpText;
        this.cancelText = cancelText;
        this.confirmText = confirmText;
        this.errorInvalidText = errorInvalidText;
        this.hourLabelText = hourLabelText;
        this.minuteLabelText = minuteLabelText;
        this.restorationId = restorationId;
        this.entryMode = entryMode;
        this.orientation = orientation;
        this.onEntryModeChanged = onEntryModeChanged;
        this.switchToInputEntryModeIcon = switchToInputEntryModeIcon;
        this.switchToTimerEntryModeIcon = switchToTimerEntryModeIcon;
        this.emptyInitialInput = emptyInitialInput;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _TimePickerState__time_picker());
}

internal class _TimePickerState__time_picker : global::Doroti.Framework.Widgets.State<_TimePicker__time_picker>, global::Doroti.Framework.Widgets.RestorationMixin<_TimePicker__time_picker>
{
    internal virtual Timer? _vibrateTimer { get; set; } = default;
    public virtual MaterialLocalizations localizations { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableEnum<_HourMinuteMode__time_picker> _hourMinuteMode { get; private set; } = new global::Doroti.Framework.Widgets.RestorableEnum<_HourMinuteMode__time_picker>(_HourMinuteMode__time_picker.hour, values: System.Enum.GetValues<_HourMinuteMode__time_picker>().ToList().Cast<_HourMinuteMode__time_picker>());
    internal virtual global::Doroti.Framework.Widgets.RestorableEnumN<_HourMinuteMode__time_picker> _lastModeAnnounced { get; private set; } = new global::Doroti.Framework.Widgets.RestorableEnumN<_HourMinuteMode__time_picker>(null, values: System.Enum.GetValues<_HourMinuteMode__time_picker>().ToList().Cast<_HourMinuteMode__time_picker>());
    internal virtual global::Doroti.Framework.Widgets.RestorableBoolN _autofocusHour { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBoolN(null);
    internal virtual global::Doroti.Framework.Widgets.RestorableBoolN _autofocusMinute { get; private set; } = new global::Doroti.Framework.Widgets.RestorableBoolN(null);
    private bool __late__orientation_initialized;
    private global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation> __late__orientation = default!;
    internal virtual global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation> _orientation
    {
        get
        {
            if (!__late__orientation_initialized)
            {
                __late__orientation = new global::Doroti.Framework.Widgets.RestorableEnumN<global::Doroti.Framework.Widgets.Orientation>(((_TimePicker__time_picker)this.widget).orientation, values: System.Enum.GetValues<global::Doroti.Framework.Widgets.Orientation>().ToList().Cast<global::Doroti.Framework.Widgets.Orientation>());
                __late__orientation_initialized = true;
            }
            return __late__orientation;
        }
    }
    private bool __late__selectedTime_initialized;
    private RestorableTimeOfDay __late__selectedTime = default!;
    internal virtual RestorableTimeOfDay _selectedTime
    {
        get
        {
            if (!__late__selectedTime_initialized)
            {
                __late__selectedTime = new RestorableTimeOfDay(((_TimePicker__time_picker)this.widget).time);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual RestorableTimeOfDay selectedTime => this._selectedTime;
    public override void dispose()
    {
        this._vibrateTimer?.cancel();
        _vibrateTimer = null;
        this._orientation.dispose();
        this._selectedTime.dispose();
        this._hourMinuteMode.dispose();
        this._lastModeAnnounced.dispose();
        this._autofocusHour.dispose();
        this._autofocusMinute.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
        localizations = MaterialLocalizations.of(this.context);
    }

    public override void didUpdateWidget(_TimePicker__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((!object.Equals(((_TimePicker__time_picker)oldWidget).orientation, ((_TimePicker__time_picker)this.widget).orientation)))
        {
            this._orientation.value = ((_TimePicker__time_picker)this.widget).orientation;
        }
        if ((!object.Equals(((_TimePicker__time_picker)oldWidget).time, ((_TimePicker__time_picker)this.widget).time)))
        {
            this._selectedTime.value = ((_TimePicker__time_picker)this.widget).time;
        }
    }

    internal virtual void _setEntryMode(TimePickerEntryMode mode)
    {
        ((_TimePicker__time_picker)this.widget).onEntryModeChanged?.Invoke(mode);
    }

    public virtual string? restorationId => ((_TimePicker__time_picker)this.widget).restorationId;
    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._hourMinuteMode), "hour_minute_mode");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._lastModeAnnounced), "last_mode_announced");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autofocusHour), "autofocus_hour");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._autofocusMinute), "autofocus_minute");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._selectedTime), "selected_time");
        registerForRestoration(DartRuntimePrimitives.ConvertValue<dynamic>(this._orientation), "orientation");
    }

    internal virtual void _vibrate()
    {
        switch (Theme.of(this.context).platform)
        {
            case global::Doroti.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                {
                    this._vibrateTimer?.cancel();
                    _vibrateTimer = new Timer(Time_pickerLibrary._kVibrateCommitDelay, (() =>
                    {
                        DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                        _vibrateTimer = null;
                    }));
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
        }
    }

    internal virtual void _handleHourMinuteModeChanged(_HourMinuteMode__time_picker mode)
    {
        _vibrate();
        setState(((global::System.Action)(() =>
        {
            this._hourMinuteMode.value = mode;
        })));
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(((global::System.Action)(() =>
        {
            TimePickerEntryMode newMode = ((_TimePicker__time_picker)this.widget).entryMode;
            switch (((_TimePicker__time_picker)this.widget).entryMode)
            {
                case TimePickerEntryMode.dial:
                    {
                        newMode = TimePickerEntryMode.input;
                        break;
                    }
                case TimePickerEntryMode.input:
                    {
                        this._autofocusHour.value = false;
                        this._autofocusMinute.value = false;
                        newMode = TimePickerEntryMode.dial;
                        break;
                    }
                case TimePickerEntryMode.dialOnly:
                case TimePickerEntryMode.inputOnly:
                    {
                        global::Doroti.Framework.Foundation.FlutterError.Create($"Can not change entry mode from {((_TimePicker__time_picker)this.widget).entryMode}");
                        break;
                    }
            }
            _setEntryMode(newMode);
        })));
    }

    internal virtual void _handleTimeChanged(TimeOfDay value)
    {
        _vibrate();
        setState(((global::System.Action)(() =>
        {
            this._selectedTime.value = value;
            ((_TimePicker__time_picker)this.widget).onTimeChanged?.Invoke(value);
        })));
    }

    internal virtual void _handleHourDoubleTapped()
    {
        this._autofocusHour.value = true;
        _handleEntryModeToggle();
    }

    internal virtual void _handleMinuteDoubleTapped()
    {
        this._autofocusMinute.value = true;
        _handleEntryModeToggle();
    }

    internal virtual void _handleHourSelected()
    {
        setState(((global::System.Action)(() =>
        {
            this._hourMinuteMode.value = _HourMinuteMode__time_picker.minute;
        })));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        TimeOfDayFormat timeOfDayFormatLocal = this.localizations.timeOfDayFormat(alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context));
        ThemeData themeLocal = Theme.of(context);
        _TimePickerDefaults__time_picker defaultThemeLocal = (themeLocal.useMaterial3 ? new _TimePickerDefaultsM3__time_picker(context, entryMode: ((_TimePicker__time_picker)this.widget).entryMode) : new _TimePickerDefaultsM2__time_picker(context));
        global::Doroti.Framework.Widgets.Orientation orientationLocal = ((this._orientation.value ?? (global::Doroti.Framework.Widgets.Orientation)MediaQuery.orientationOf(context)));
        HourFormat timeOfDayHour = TimeLibrary.hourFormat(of: timeOfDayFormatLocal);
        _HourDialType__time_picker hourMode = (timeOfDayHour switch { HourFormat.HH or HourFormat.H when (themeLocal.useMaterial3) => _HourDialType__time_picker.twentyFourHourDoubleRing, HourFormat.HH => _HourDialType__time_picker.twentyFourHour, HourFormat.H => _HourDialType__time_picker.twentyFourHour, HourFormat.h => _HourDialType__time_picker.twelveHour, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        string helpTextLocal = default!;
        global::Doroti.Framework.Widgets.Widget picker = default!;
        switch (((_TimePicker__time_picker)this.widget).entryMode)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    helpTextLocal = (((_TimePicker__time_picker)this.widget).helpText ?? ((themeLocal.useMaterial3 ? this.localizations.timePickerDialHelpText : this.localizations.timePickerDialHelpText.toUpperCase())));
                    double portraitMinInteractiveVerticalAdjustment = Math.Max(0, ((2L * global::Doroti.Framework.Widgets.ConstantsLibrary.kMinInteractiveDimension) - ((_TimePickerDefaults__time_picker)defaultThemeLocal).dayPeriodPortraitSize.height));
                    global::Doroti.Framework.Painting.EdgeInsetsGeometry dialPadding = (orientationLocal switch { global::Doroti.Framework.Widgets.Orientation.portrait => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: 12, right: 12, top: (36L - (portraitMinInteractiveVerticalAdjustment / 2L)))), global::Doroti.Framework.Widgets.Orientation.landscape => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 64)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    global::Doroti.Framework.Widgets.Widget dialLocal = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: dialPadding, child: new global::Doroti.Framework.Widgets.ExcludeSemantics(child: global::Doroti.Framework.Widgets.SizedBox.CreateFromSize(size: ((_TimePickerDefaults__time_picker)defaultThemeLocal).dialSize, child: new global::Doroti.Framework.Widgets.AspectRatio(aspectRatio: 1, child: new _Dial__time_picker(hourMinuteMode: DartRuntimePrimitives.RequireValue(this._hourMinuteMode.value), hourDialType: hourMode, selectedTime: this._selectedTime.value, onChanged: (global::System.Action<TimeOfDay>)this._handleTimeChanged, onHourSelected: () => this._handleHourSelected()))))));
                    switch (orientationLocal)
                    {
                        case global::Doroti.Framework.Widgets.Orientation.portrait:
                            {
                                picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (themeLocal.useMaterial3 ? 0 : 16)), child: new _DialTimePickerHeader__time_picker(helpText: helpTextLocal))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (themeLocal.useMaterial3 ? 0 : 16)), child: dialLocal))) }))) }));
                                break;
                            }
                        case global::Doroti.Framework.Widgets.Orientation.landscape:
                            {
                                picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: (themeLocal.useMaterial3 ? 0 : 16)), child: new global::Doroti.Framework.Widgets.Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.stretch, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _DialTimePickerHeader__time_picker(helpText: helpTextLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: dialLocal)) })))) }));
                                break;
                            }
                    }
                    break;
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    string helpTextAlternate = (((_TimePicker__time_picker)this.widget).helpText ?? ((themeLocal.useMaterial3 ? this.localizations.timePickerInputHelpText : this.localizations.timePickerInputHelpText.toUpperCase())));
                    picker = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Column(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _TimePickerInput__time_picker(initialSelectedTime: this._selectedTime.value, errorInvalidText: ((_TimePicker__time_picker)this.widget).errorInvalidText, hourLabelText: ((_TimePicker__time_picker)this.widget).hourLabelText, minuteLabelText: ((_TimePicker__time_picker)this.widget).minuteLabelText, helpText: helpTextAlternate, autofocusHour: this._autofocusHour.value, autofocusMinute: this._autofocusMinute.value, restorationId: "time_picker_input", emptyInitialTime: ((_TimePicker__time_picker)this.widget).emptyInitialInput)) }));
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _TimePickerModel__time_picker(entryMode: ((_TimePicker__time_picker)this.widget).entryMode, selectedTime: this._selectedTime.value, hourMinuteMode: DartRuntimePrimitives.RequireValue(this._hourMinuteMode.value), orientation: orientationLocal, onHourMinuteModeChanged: (global::System.Action<_HourMinuteMode__time_picker>)this._handleHourMinuteModeChanged, onHourDoubleTapped: () => this._handleHourDoubleTapped(), onMinuteDoubleTapped: () => this._handleMinuteDoubleTapped(), hourDialType: hourMode, onSelectedTimeChanged: (global::System.Action<TimeOfDay>)this._handleTimeChanged, useMaterial3: themeLocal.useMaterial3, use24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context), theme: TimePickerTheme.of(context), defaultTheme: defaultThemeLocal, child: picker));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(((Func<List<global::Doroti.Framework.Foundation.DiagnosticsNode>>)(() => { var __collection41817 = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>(); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(this._debugPropertiesWaitingForReregistration!.map<dynamic, global::Doroti.Framework.Foundation.DiagnosticsNode>(((property) => new global::Doroti.Framework.Foundation.ErrorDescription($" * {((dynamic)property)._restorationId}")))); return __collection41817; }))()));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public static partial class Time_pickerLibrary
{
    public static async Future<TimeOfDay?> showTimePicker(global::Doroti.Framework.Widgets.BuildContext context, TimeOfDay initialTime, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>? builder = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, TimePickerEntryMode initialEntryMode = TimePickerEntryMode.dial, string? cancelText = null, string? confirmText = null, string? helpText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, global::Doroti.Framework.Widgets.RouteSettings? routeSettings = null, global::System.Action<TimePickerEntryMode>? onEntryModeChanged = null, Offset? anchorPoint = null, global::Doroti.Framework.Widgets.Orientation? orientation = null, global::Doroti.Framework.Widgets.Icon? switchToInputEntryModeIcon = null, global::Doroti.Framework.Widgets.Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = false)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.Widget dialog = ((global::Doroti.Framework.Widgets.Widget)(object?)new TimePickerDialog(initialTime: initialTime, initialEntryMode: initialEntryMode, cancelText: cancelText, confirmText: confirmText, helpText: helpText, errorInvalidText: errorInvalidText, hourLabelText: hourLabelText, minuteLabelText: minuteLabelText, orientation: orientation, onEntryModeChanged: (global::System.Action<TimePickerEntryMode>?)onEntryModeChanged, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToTimerEntryModeIcon: switchToTimerEntryModeIcon, emptyInitialInput: emptyInitialInput));
        return await DialogLibrary.showDialog<TimeOfDay>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: ((context) =>
        {
            return ((builder is null) ? dialog : builder(context, dialog));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), routeSettings: routeSettings, anchorPoint: DartRuntimePrimitives.RequireValue(anchorPoint));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal abstract class _TimePickerDefaults__time_picker : TimePickerThemeData
{
    public abstract global::Doroti.Ui.Color backgroundColor { get; }
    public abstract ButtonStyle cancelButtonStyle { get; }
    public abstract ButtonStyle confirmButtonStyle { get; }
    public abstract global::Doroti.Framework.Painting.BorderSide dayPeriodBorderSide { get; }
    public abstract global::Doroti.Ui.Color dayPeriodColor { get; }
    public abstract global::Doroti.Framework.Painting.OutlinedBorder dayPeriodShape { get; }
    public abstract global::Doroti.Ui.Size dayPeriodInputSize { get; }
    public abstract global::Doroti.Ui.Size dayPeriodLandscapeSize { get; }
    public abstract global::Doroti.Ui.Size dayPeriodPortraitSize { get; }
    public abstract global::Doroti.Ui.Color dayPeriodTextColor { get; }
    public abstract global::Doroti.Framework.Painting.TextStyle dayPeriodTextStyle { get; }
    public abstract global::Doroti.Ui.Color dialBackgroundColor { get; }
    public abstract global::Doroti.Ui.Color dialHandColor { get; }
    public abstract global::Doroti.Ui.Size dialSize { get; }
    public abstract double handWidth { get; }
    public abstract double dotRadius { get; }
    public abstract double centerRadius { get; }
    public abstract global::Doroti.Ui.Color dialTextColor { get; }
    public abstract global::Doroti.Framework.Painting.TextStyle dialTextStyle { get; }
    public abstract double elevation { get; }
    public abstract global::Doroti.Ui.Color entryModeIconColor { get; }
    public abstract global::Doroti.Framework.Painting.TextStyle helpTextStyle { get; }
    public abstract global::Doroti.Ui.Color hourMinuteColor { get; }
    public abstract global::Doroti.Framework.Painting.ShapeBorder hourMinuteShape { get; }
    public abstract global::Doroti.Ui.Size hourMinuteSize { get; }
    public abstract global::Doroti.Ui.Size hourMinuteSize24Hour { get; }
    public abstract global::Doroti.Ui.Size hourMinuteInputSize { get; }
    public abstract global::Doroti.Ui.Size hourMinuteInputSize24Hour { get; }
    public abstract global::Doroti.Ui.Color hourMinuteTextColor { get; }
    public abstract global::Doroti.Framework.Painting.TextStyle hourMinuteTextStyle { get; }
    public abstract InputDecorationThemeData inputDecorationTheme { get; }
    public abstract global::Doroti.Framework.Painting.EdgeInsetsGeometry padding { get; }
    public abstract global::Doroti.Framework.Painting.ShapeBorder shape { get; }
}

internal class _TimePickerDefaultsM2__time_picker : _TimePickerDefaults__time_picker
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }
    internal static global::Doroti.Framework.Painting.OutlinedBorder _kDefaultShape = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(4))));

    internal _TimePickerDefaultsM2__time_picker(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public override Color backgroundColor
    {
        get
        {
            return this._colors.surface;
            return default!;
        }
    }
    public override ButtonStyle cancelButtonStyle
    {
        get
        {
            return ((ButtonStyle)(object?)TextButton.styleFrom());
            return default!;
        }
    }
    public override ButtonStyle confirmButtonStyle
    {
        get
        {
            return ((ButtonStyle)(object?)TextButton.styleFrom());
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.BorderSide dayPeriodBorderSide
    {
        get
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: Dart_uiLibrary.Color.alphaBlend(this._colors.onSurface.withOpacity(0.38), this._colors.surface));
            return default!;
        }
    }
    public override Color dayPeriodColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return this._colors.primary.withOpacity(((object.Equals(this._colors.brightness, Brightness.dark)) ? 0.24 : 0.12));
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.OutlinedBorder dayPeriodShape
    {
        get
        {
            return _kDefaultShape;
            return default!;
        }
    }
    public override Size dayPeriodPortraitSize
    {
        get
        {
            return new global::Doroti.Ui.Size(52, 80);
            return default!;
        }
    }
    public override Size dayPeriodLandscapeSize
    {
        get
        {
            return new global::Doroti.Ui.Size(0, 40);
            return default!;
        }
    }
    public override Size dayPeriodInputSize
    {
        get
        {
            return new global::Doroti.Ui.Size(52, 70);
            return default!;
        }
    }
    public override Color dayPeriodTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                return (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.primary : this._colors.onSurface.withOpacity(0.6));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle dayPeriodTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)this._textTheme.titleMedium!.copyWith(color: this.dayPeriodTextColor));
            return default!;
        }
    }
    public override Color dialBackgroundColor
    {
        get
        {
            return this._colors.onSurface.withOpacity(((object.Equals(this._colors.brightness, Brightness.dark)) ? 0.12 : 0.08));
            return default!;
        }
    }
    public override Color dialHandColor
    {
        get
        {
            return this._colors.primary;
            return default!;
        }
    }
    public override Size dialSize
    {
        get
        {
            return new global::Doroti.Ui.Size(280);
            return default!;
        }
    }
    public override double handWidth
    {
        get
        {
            return 2;
            return default!;
        }
    }
    public override double dotRadius
    {
        get
        {
            return 22;
            return default!;
        }
    }
    public override double centerRadius
    {
        get
        {
            return 4;
            return default!;
        }
    }
    public override Color dialTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return this._colors.surface;
                }
                return this._colors.onSurface;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle dialTextStyle
    {
        get
        {
            return this._textTheme.bodyLarge!;
            return default!;
        }
    }
    public override double elevation
    {
        get
        {
            return 6;
            return default!;
        }
    }
    public override Color entryModeIconColor
    {
        get
        {
            return this._colors.onSurface.withOpacity(((object.Equals(this._colors.brightness, Brightness.dark)) ? 1.0 : 0.6));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle helpTextStyle
    {
        get
        {
            return this._textTheme.labelSmall!;
            return default!;
        }
    }
    public override Color hourMinuteColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                return (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.primary.withOpacity(((object.Equals(this._colors.brightness, Brightness.dark)) ? 0.24 : 0.12)) : this._colors.onSurface.withOpacity(0.12));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.ShapeBorder hourMinuteShape
    {
        get
        {
            return ((global::Doroti.Framework.Painting.ShapeBorder)(object?)_kDefaultShape);
            return default!;
        }
    }
    public override Size hourMinuteSize
    {
        get
        {
            return new global::Doroti.Ui.Size(96, 80);
            return default!;
        }
    }
    public override Size hourMinuteSize24Hour
    {
        get
        {
            return new global::Doroti.Ui.Size(114, 80);
            return default!;
        }
    }
    public override Size hourMinuteInputSize
    {
        get
        {
            return new global::Doroti.Ui.Size(96, 70);
            return default!;
        }
    }
    public override Size hourMinuteInputSize24Hour
    {
        get
        {
            return new global::Doroti.Ui.Size(114, 70);
            return default!;
        }
    }
    public override Color hourMinuteTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                return (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? this._colors.primary : this._colors.onSurface);
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle hourMinuteTextStyle
    {
        get
        {
            return this._textTheme.displayMedium!;
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Color _hourMinuteInputColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                return (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) ? Colors.transparent : this._colors.onSurface.withOpacity(0.12));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override InputDecorationThemeData inputDecorationTheme
    {
        get
        {
            return new InputDecorationThemeData(contentPadding: global::Doroti.Framework.Painting.EdgeInsets.zero, filled: true, fillColor: this._hourMinuteInputColor, focusColor: Colors.transparent, enabledBorder: new OutlineInputBorder(borderSide: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent)), errorBorder: new OutlineInputBorder(borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2)), focusedBorder: new OutlineInputBorder(borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.primary, width: 2)), focusedErrorBorder: new OutlineInputBorder(borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2)), hintStyle: this.hourMinuteTextStyle.copyWith(color: this._colors.onSurface.withOpacity(0.36)), errorStyle: new global::Doroti.Framework.Painting.TextStyle(fontSize: 0, height: 1));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry padding
    {
        get
        {
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)new global::Doroti.Framework.Painting.EdgeInsets(8, 18, 8, 8));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.ShapeBorder shape
    {
        get
        {
            return ((global::Doroti.Framework.Painting.ShapeBorder)(object?)_kDefaultShape);
            return default!;
        }
    }
}

public static partial class Time_pickerLibrary
{
    internal static bool _debugDialTimePickerEntryMode(global::Doroti.Framework.Widgets.BuildContext context)
    {
        TimePickerEntryMode entryMode = _TimePickerModel__time_picker.entryModeOf(context);
        return ((object.Equals(entryMode, TimePickerEntryMode.dial)) || (object.Equals(entryMode, TimePickerEntryMode.dialOnly)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TimePickerDefaultsM3__time_picker : _TimePickerDefaults__time_picker
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
    public virtual TimePickerEntryMode entryMode { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _TimePickerDefaultsM3__time_picker(global::Doroti.Framework.Widgets.BuildContext context, TimePickerEntryMode entryMode = TimePickerEntryMode.dial)
    {
        this.context = context;
        this.entryMode = entryMode;
    }

    public override Color backgroundColor
    {
        get
        {
            return this._colors.surfaceContainerHigh;
            return default!;
        }
    }
    public override ButtonStyle cancelButtonStyle
    {
        get
        {
            return ((ButtonStyle)(object?)TextButton.styleFrom());
            return default!;
        }
    }
    public override ButtonStyle confirmButtonStyle
    {
        get
        {
            return ((ButtonStyle)(object?)TextButton.styleFrom());
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.BorderSide dayPeriodBorderSide
    {
        get
        {
            return new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outline);
            return default!;
        }
    }
    public override Color dayPeriodColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return this._colors.tertiaryContainer;
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.OutlinedBorder dayPeriodShape
    {
        get
        {
            return ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))).copyWith(side: this.dayPeriodBorderSide));
            return default!;
        }
    }
    public override Size dayPeriodPortraitSize
    {
        get
        {
            return new global::Doroti.Ui.Size(52, 80);
            return default!;
        }
    }
    public override Size dayPeriodLandscapeSize
    {
        get
        {
            return new global::Doroti.Ui.Size(216, 38);
            return default!;
        }
    }
    public override Size dayPeriodInputSize
    {
        get
        {
            return new global::Doroti.Ui.Size(this.dayPeriodPortraitSize.width, (this.dayPeriodPortraitSize.height - 8L));
            return default!;
        }
    }
    public override Color dayPeriodTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return this._colors.onTertiaryContainer;
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return this._colors.onTertiaryContainer;
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return this._colors.onTertiaryContainer;
                    }
                    return this._colors.onTertiaryContainer;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                {
                    return this._colors.onSurfaceVariant;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                {
                    return this._colors.onSurfaceVariant;
                }
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                {
                    return this._colors.onSurfaceVariant;
                }
                return this._colors.onSurfaceVariant;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle dayPeriodTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)this._textTheme.titleMedium!.copyWith(color: this.dayPeriodTextColor));
            return default!;
        }
    }
    public override Color dialBackgroundColor
    {
        get
        {
            return this._colors.surfaceContainerHighest;
            return default!;
        }
    }
    public override Color dialHandColor
    {
        get
        {
            return this._colors.primary;
            return default!;
        }
    }
    public override Size dialSize
    {
        get
        {
            return new global::Doroti.Ui.Size(256.0);
            return default!;
        }
    }
    public override double handWidth
    {
        get
        {
            return new global::Doroti.Ui.Size(2, double.PositiveInfinity).width;
            return default!;
        }
    }
    public override double dotRadius
    {
        get
        {
            return (new global::Doroti.Ui.Size(48.0).width / 2L);
            return default!;
        }
    }
    public override double centerRadius
    {
        get
        {
            return (new global::Doroti.Ui.Size(8.0).width / 2L);
            return default!;
        }
    }
    public override Color dialTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    return this._colors.onPrimary;
                }
                return this._colors.onSurface;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle dialTextStyle
    {
        get
        {
            return this._textTheme.bodyLarge!;
            return default!;
        }
    }
    public override double elevation
    {
        get
        {
            return 6.0;
            return default!;
        }
    }
    public override Color entryModeIconColor
    {
        get
        {
            return this._colors.onSurface;
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle helpTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
            {
                global::Doroti.Framework.Painting.TextStyle textStyle = this._textTheme.labelMedium!;
                return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle.copyWith(color: this._colors.onSurfaceVariant));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry padding
    {
        get
        {
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateAll(24));
            return default!;
        }
    }
    public override Color hourMinuteColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    global::Doroti.Ui.Color overlayColor = ((global::Doroti.Ui.Color)(object?)this._colors.primaryContainer);
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        overlayColor = this._colors.onPrimaryContainer;
                    }
                    else
                    {
                        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                        {
                            var hoverOpacity = 0.08;
                            overlayColor = this._colors.onPrimaryContainer.withOpacity(hoverOpacity);
                        }
                        else
                        {
                            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                            {
                                var focusOpacity = 0.1;
                                overlayColor = this._colors.onPrimaryContainer.withOpacity(focusOpacity);
                            }
                        }
                    }
                    return Dart_uiLibrary.Color.alphaBlend(overlayColor, this._colors.primaryContainer);
                }
                else
                {
                    global::Doroti.Ui.Color overlayColorLocal = ((global::Doroti.Ui.Color)(object?)this._colors.surfaceContainerHighest);
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        overlayColorLocal = this._colors.onSurface;
                    }
                    else
                    {
                        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                        {
                            var hoverOpacityLocal = 0.08;
                            overlayColorLocal = this._colors.onSurface.withOpacity(hoverOpacityLocal);
                        }
                        else
                        {
                            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                            {
                                var focusOpacityLocal = 0.1;
                                overlayColorLocal = this._colors.onSurface.withOpacity(focusOpacityLocal);
                            }
                        }
                    }
                    return Dart_uiLibrary.Color.alphaBlend(overlayColorLocal, this._colors.surfaceContainerHighest);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.ShapeBorder hourMinuteShape
    {
        get
        {
            return ((global::Doroti.Framework.Painting.ShapeBorder)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))));
            return default!;
        }
    }
    public override Size hourMinuteSize
    {
        get
        {
            return new global::Doroti.Ui.Size(96, 80);
            return default!;
        }
    }
    public override Size hourMinuteSize24Hour
    {
        get
        {
            return new global::Doroti.Ui.Size(new global::Doroti.Ui.Size(114, double.PositiveInfinity).width, this.hourMinuteSize.height);
            return default!;
        }
    }
    public override Size hourMinuteInputSize
    {
        get
        {
            return new global::Doroti.Ui.Size(this.hourMinuteSize.width, (this.hourMinuteSize.height - 8L));
            return default!;
        }
    }
    public override Size hourMinuteInputSize24Hour
    {
        get
        {
            return new global::Doroti.Ui.Size(this.hourMinuteSize24Hour.width, (this.hourMinuteSize24Hour.height - 8L));
            return default!;
        }
    }
    public override Color hourMinuteTextColor
    {
        get
        {
            return ((Color)(object?)global::Doroti.Framework.Widgets.WidgetStateColor.CreateResolveWith(((states) =>
            {
                return ((Color)(object?)this._hourMinuteTextColor.resolve(states));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color> _hourMinuteTextColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color>)(object?)WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onPrimaryContainer);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onPrimaryContainer);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onPrimaryContainer);
                    }
                    return (this._colors.onPrimaryContainer);
                }
                else
                {
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
                    {
                        return (this._colors.onSurface);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
                    {
                        return (this._colors.onSurface);
                    }
                    if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
                    {
                        return (this._colors.onSurface);
                    }
                    return (this._colors.onSurface);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.TextStyle hourMinuteTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Painting.TextStyle)(object?)global::Doroti.Framework.Widgets.WidgetStateTextStyle.CreateResolveWith(((states) =>
            {
                return (this.entryMode switch { TimePickerEntryMode.dial => this._textTheme.displayLarge!.copyWith(color: this._hourMinuteTextColor.resolve(states)), TimePickerEntryMode.dialOnly => this._textTheme.displayLarge!.copyWith(color: this._hourMinuteTextColor.resolve(states)), TimePickerEntryMode.input => this._textTheme.displayMedium!.copyWith(color: this._hourMinuteTextColor.resolve(states)), TimePickerEntryMode.inputOnly => this._textTheme.displayMedium!.copyWith(color: this._hourMinuteTextColor.resolve(states)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
            return default!;
        }
    }
    public override InputDecorationThemeData inputDecorationTheme
    {
        get
        {
            global::Doroti.Framework.Painting.BorderRadius selectorRadius = ((global::Doroti.Framework.Painting.BorderRadius)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))).borderRadius.resolve(Directionality.of(this.context)));
            return new InputDecorationThemeData(contentPadding: global::Doroti.Framework.Painting.EdgeInsets.zero, filled: true, fillColor: this.hourMinuteColor, focusColor: this._colors.primaryContainer, enabledBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new global::Doroti.Framework.Painting.BorderSide(color: Colors.transparent)), errorBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2)), focusedBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.primary, width: 2)), focusedErrorBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new global::Doroti.Framework.Painting.BorderSide(color: this._colors.error, width: 2)), hintStyle: this.hourMinuteTextStyle.copyWith(color: this._colors.onSurface.withOpacity(0.36)), errorStyle: new global::Doroti.Framework.Painting.TextStyle(fontSize: 0));
            return default!;
        }
    }
    public override global::Doroti.Framework.Painting.ShapeBorder shape
    {
        get
        {
            return ((global::Doroti.Framework.Painting.ShapeBorder)(object?)new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? timeSelectorSeparatorColor
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>?)(object?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Ui.Color>(this._colors.onSurface));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? timeSelectorSeparatorTextStyle
    {
        get
        {
            return ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>?)(object?)new global::Doroti.Framework.Widgets.WidgetStatePropertyAll<global::Doroti.Framework.Painting.TextStyle?>(this._textTheme.displayLarge));
            return default!;
        }
    }
}
