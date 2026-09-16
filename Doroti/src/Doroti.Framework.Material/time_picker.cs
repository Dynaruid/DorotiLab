// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/time_picker.dart

using Doroti.Runtime;
using Doroti.Ui;

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
    internal static double _kTwoPi = 2L * Dart_mathLibrary.pi;
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

internal class _TimePickerModel__time_picker : InheritedModel<_TimePickerAspect__time_picker>
{
    public virtual TimePickerEntryMode entryMode { get; private set; } = default!;
    public virtual _HourMinuteMode__time_picker hourMinuteMode { get; private set; } = default!;
    public virtual Action<_HourMinuteMode__time_picker> onHourMinuteModeChanged { get; private set; } = default!;
    public virtual Action onHourDoubleTapped { get; private set; } = default!;
    public virtual Action onMinuteDoubleTapped { get; private set; } = default!;
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual Action<TimeOfDay> onSelectedTimeChanged { get; private set; } = default!;
    public virtual bool use24HourFormat { get; private set; } = default!;
    public virtual _HourDialType__time_picker hourDialType { get; private set; } = default!;
    public virtual Orientation orientation { get; private set; } = default!;
    public virtual TimePickerThemeData theme { get; private set; } = default!;
    public virtual _TimePickerDefaults__time_picker defaultTheme { get; private set; } = default!;

    internal _TimePickerModel__time_picker(TimePickerEntryMode entryMode, _HourMinuteMode__time_picker hourMinuteMode, Action<_HourMinuteMode__time_picker> onHourMinuteModeChanged, Action onHourDoubleTapped, Action onMinuteDoubleTapped, TimeOfDay selectedTime, Action<TimeOfDay> onSelectedTimeChanged, bool use24HourFormat, _HourDialType__time_picker hourDialType, Orientation orientation, TimePickerThemeData theme, _TimePickerDefaults__time_picker defaultTheme, Widget child) : base(child: child)
    {
        this.entryMode = entryMode;
        this.hourMinuteMode = hourMinuteMode;
        this.onHourMinuteModeChanged = onHourMinuteModeChanged;
        this.onHourDoubleTapped = onHourDoubleTapped;
        this.onMinuteDoubleTapped = onMinuteDoubleTapped;
        this.selectedTime = selectedTime;
        this.onSelectedTimeChanged = onSelectedTimeChanged;
        this.use24HourFormat = use24HourFormat;
        this.hourDialType = hourDialType;
        this.orientation = orientation;
        this.theme = theme;
        this.defaultTheme = defaultTheme;
    }

    public static _TimePickerModel__time_picker of(BuildContext context, _TimePickerAspect__time_picker? aspect = null) => DartRuntimePrimitives.ConvertValue<_TimePickerModel__time_picker>(InheritedModel<object>.inheritFrom<_TimePickerModel__time_picker>(context, aspect: aspect)!);
    public static TimePickerEntryMode entryModeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.entryMode).entryMode;
    public static _HourMinuteMode__time_picker hourMinuteModeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.hourMinuteMode).hourMinuteMode;
    public static TimeOfDay selectedTimeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.selectedTime).selectedTime;
    public static bool use24HourFormatOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.use24HourFormat).use24HourFormat;
    public static _HourDialType__time_picker hourDialTypeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.hourDialType).hourDialType;
    public static Orientation orientationOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.orientation).orientation;
    public static TimePickerThemeData themeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.theme).theme;
    public static _TimePickerDefaults__time_picker defaultThemeOf(BuildContext context) => of(context, _TimePickerAspect__time_picker.defaultTheme).defaultTheme;
    public static void setSelectedTime(BuildContext context, TimeOfDay value) => of(context, _TimePickerAspect__time_picker.onSelectedTimeChanged).onSelectedTimeChanged(value);
    public static void setHourMinuteMode(BuildContext context, _HourMinuteMode__time_picker value) => of(context, _TimePickerAspect__time_picker.onHourMinuteModeChanged).onHourMinuteModeChanged(value);
    public override bool updateShouldNotifyDependent(InheritedModel<_TimePickerAspect__time_picker> oldWidget, HashSet<_TimePickerAspect__time_picker> dependencies)
    {
        var __oldWidget = (_TimePickerModel__time_picker)oldWidget;
        if ((use24HourFormat != __oldWidget.use24HourFormat) && dependencies.Contains(_TimePickerAspect__time_picker.use24HourFormat))
        {
            return true;
        }
        if ((!Equals(entryMode, __oldWidget.entryMode)) && dependencies.Contains(_TimePickerAspect__time_picker.entryMode))
        {
            return true;
        }
        if ((!Equals(hourMinuteMode, __oldWidget.hourMinuteMode)) && dependencies.Contains(_TimePickerAspect__time_picker.hourMinuteMode))
        {
            return true;
        }
        if ((!Equals(onHourMinuteModeChanged, __oldWidget.onHourMinuteModeChanged)) && dependencies.Contains(_TimePickerAspect__time_picker.onHourMinuteModeChanged))
        {
            return true;
        }
        if ((!Equals(onHourMinuteModeChanged, __oldWidget.onHourDoubleTapped)) && dependencies.Contains(_TimePickerAspect__time_picker.onHourDoubleTapped))
        {
            return true;
        }
        if ((!Equals(onHourMinuteModeChanged, __oldWidget.onMinuteDoubleTapped)) && dependencies.Contains(_TimePickerAspect__time_picker.onMinuteDoubleTapped))
        {
            return true;
        }
        if ((!Equals(hourDialType, __oldWidget.hourDialType)) && dependencies.Contains(_TimePickerAspect__time_picker.hourDialType))
        {
            return true;
        }
        if ((!Equals(selectedTime, __oldWidget.selectedTime)) && dependencies.Contains(_TimePickerAspect__time_picker.selectedTime))
        {
            return true;
        }
        if ((!Equals(onSelectedTimeChanged, __oldWidget.onSelectedTimeChanged)) && dependencies.Contains(_TimePickerAspect__time_picker.onSelectedTimeChanged))
        {
            return true;
        }
        if ((!Equals(orientation, __oldWidget.orientation)) && dependencies.Contains(_TimePickerAspect__time_picker.orientation))
        {
            return true;
        }
        if ((!Equals(theme, __oldWidget.theme)) && dependencies.Contains(_TimePickerAspect__time_picker.theme))
        {
            return true;
        }
        if ((!Equals(defaultTheme, __oldWidget.defaultTheme)) && dependencies.Contains(_TimePickerAspect__time_picker.defaultTheme))
        {
            return true;
        }
        return false;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_TimePickerModel__time_picker)oldWidget;
        return use24HourFormat != __oldWidget.use24HourFormat || (!Equals(entryMode, __oldWidget.entryMode)) || (!Equals(hourMinuteMode, __oldWidget.hourMinuteMode)) || (!Equals(onHourMinuteModeChanged, __oldWidget.onHourMinuteModeChanged)) || (!Equals(onHourDoubleTapped, __oldWidget.onHourDoubleTapped)) || (!Equals(onMinuteDoubleTapped, __oldWidget.onMinuteDoubleTapped)) || (!Equals(hourDialType, __oldWidget.hourDialType)) || (!Equals(selectedTime, __oldWidget.selectedTime)) || (!Equals(onSelectedTimeChanged, __oldWidget.onSelectedTimeChanged)) || (!Equals(orientation, __oldWidget.orientation)) || (!Equals(theme, __oldWidget.theme)) || (!Equals(defaultTheme, __oldWidget.defaultTheme));
    }

}

internal class _DialTimePickerHeader__time_picker : StatelessWidget
{
    public virtual string helpText { get; private set; } = default!;

    internal _DialTimePickerHeader__time_picker(string helpText)
    {
        this.helpText = helpText;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        TimeOfDayFormat timeOfDayFormatLocal = MaterialLocalizations.of(context).timeOfDayFormat(alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        _TimePickerDefaults__time_picker defaultTheme = _TimePickerModel__time_picker.defaultThemeOf(context);
        Orientation orientation = _TimePickerModel__time_picker.orientationOf(context);
        double dayPeriodHeight = Equals(orientation, Orientation.portrait) ? defaultTheme.dayPeriodPortraitSize.height : defaultTheme.dayPeriodLandscapeSize.height;
        double minInteractiveVerticalPadding = Equals(orientation, Orientation.portrait) ? Math.Max(0, (2L * Widgets.ConstantsLibrary.kMinInteractiveDimension) - dayPeriodHeight) : Math.Max(0, Widgets.ConstantsLibrary.kMinInteractiveDimension - dayPeriodHeight);
        _HourDialType__time_picker hourDialType = _TimePickerModel__time_picker.hourDialTypeOf(context);
        RenderObjectWidget orientationSpecificHeader = orientation switch { Orientation.portrait => DartRuntimePrimitives.ConvertValue<RenderObjectWidget>(new Column(crossAxisAlignment: CrossAxisAlignment.start, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(bottom: 20L - (minInteractiveVerticalPadding / 2L)), child: new Text(helpText, style: _TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? defaultTheme.helpTextStyle))), DartRuntimePrimitives.ConvertValue<Widget>(new Row(textDirection: Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm) ? TextDirection.rtl : TextDirection.ltr, spacing: 12, children: ((Func<List<Widget>>)(() => { var __collection10472 = new List<Widget>(); __collection10472.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Row(textDirection: TextDirection.ltr, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new _DialHourControl__time_picker())), DartRuntimePrimitives.ConvertValue<Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new _DialMinuteControl__time_picker())) })))); if (Equals(hourDialType, _HourDialType__time_picker.twelveHour)) { __collection10472.Add(DartRuntimePrimitives.ConvertValue<Widget>(new _DayPeriodControl__time_picker())); } return __collection10472; }))())) })), Orientation.landscape => DartRuntimePrimitives.ConvertValue<RenderObjectWidget>(new SizedBox(width: Time_pickerLibrary._kTimePickerHeaderLandscapeWidth, child: new Stack(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Text(helpText, style: _TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? defaultTheme.helpTextStyle)), DartRuntimePrimitives.ConvertValue<Widget>(new Column(verticalDirection: Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm) ? VerticalDirection.up : VerticalDirection.down, mainAxisAlignment: MainAxisAlignment.center, crossAxisAlignment: CrossAxisAlignment.start, spacing: Math.Max(0, 16L - (minInteractiveVerticalPadding / 2L)), children: ((Func<List<Widget>>)(() => { var __collection11810 = new List<Widget>(); __collection11810.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Row(textDirection: TextDirection.ltr, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new _DialHourControl__time_picker())), DartRuntimePrimitives.ConvertValue<Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new _DialMinuteControl__time_picker())) }))); if (Equals(hourDialType, _HourDialType__time_picker.twelveHour)) { __collection11810.Add(DartRuntimePrimitives.ConvertValue<Widget>(new _DayPeriodControl__time_picker())); } return __collection11810; }))())) }))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new Widgets.Semantics(label: MaterialLocalizations.of(context).formatTimeOfDay(_TimePickerModel__time_picker.selectedTimeOf(context), alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context)), child: orientationSpecificHeader);
    }

}

internal class _DialTimeSelectorControl__time_picker : StatelessWidget
{
    public virtual string text { get; private set; } = default!;
    public virtual Action onTap { get; private set; } = default!;
    public virtual Action onDoubleTap { get; private set; } = default!;
    public virtual bool isSelected { get; private set; } = default!;

    internal _DialTimeSelectorControl__time_picker(string text, Action onTap, Action onDoubleTap, bool isSelected)
    {
        this.text = text;
        this.onTap = onTap;
        this.onDoubleTap = onDoubleTap;
        this.isSelected = isSelected;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        TimePickerThemeData timePickerTheme = _TimePickerModel__time_picker.themeOf(context);
        _TimePickerDefaults__time_picker defaultTheme = _TimePickerModel__time_picker.defaultThemeOf(context);
        Color backgroundColor = timePickerTheme.hourMinuteColor ?? defaultTheme.hourMinuteColor;
        ShapeBorder shapeLocal = timePickerTheme.hourMinuteShape ?? defaultTheme.hourMinuteShape;
        var states = ((Func<HashSet<WidgetState>>)(() => { var __collection13567 = new HashSet<WidgetState>(); if (isSelected) { __collection13567.Add(WidgetState.selected); } return __collection13567; }))();
        Color effectiveTextColor = WidgetStateProperty.resolveAs(_TimePickerModel__time_picker.themeOf(context).hourMinuteTextColor ?? _TimePickerModel__time_picker.defaultThemeOf(context).hourMinuteTextColor, states);
        TextStyle effectiveStyle = WidgetStateProperty.resolveAs(timePickerTheme.hourMinuteTextStyle ?? defaultTheme.hourMinuteTextStyle, states).copyWith(color: effectiveTextColor);
        return new SizedBox(height: defaultTheme.hourMinuteSize.height, child: new Material(color: WidgetStateProperty.resolveAs(backgroundColor, states), clipBehavior: Clip.antiAlias, shape: shapeLocal, child: new InkWell(onTap: onTap, onDoubleTap: isSelected ? onDoubleTap : null, child: new Center(child: new Text(text, style: effectiveStyle, textScaler: TextScaler.noScaling)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DialHourControl__time_picker : StatelessWidget
{
    internal _DialHourControl__time_picker()
    {
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        bool alwaysUse24HourFormatLocal = MediaQuery.alwaysUse24HourFormatOf(context);
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        string formattedHour = localizations.formatHour(selectedTime, alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        TimeOfDay hoursFromSelected(long hoursToAdd)
        {
            switch (_TimePickerModel__time_picker.hourDialTypeOf(context))
            {
                case _HourDialType__time_picker.twentyFourHourDoubleRing:
                    {
                        long selectedHour = selectedTime.hour;
                        return selectedTime.replacing(hour: (selectedHour + hoursToAdd + TimeOfDay.hoursPerDay) % TimeOfDay.hoursPerDay);
                    }
                case _HourDialType__time_picker.twelveHour:
                    {
                        long periodOffsetLocal = selectedTime.periodOffset;
                        long hours = selectedTime.hourOfPeriod;
                        return selectedTime.replacing(hour: periodOffsetLocal + ((hours + hoursToAdd + TimeOfDay.hoursPerPeriod) % TimeOfDay.hoursPerPeriod));
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
        return new Widgets.Semantics(value: $"{localizations.timePickerHourModeAnnouncement} {formattedHour}", excludeSemantics: true, increasedValue: formattedNextHour, onIncrease: () =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, nextHour);
        }, decreasedValue: formattedPreviousHour, onDecrease: () =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, previousHour);
        }, child: new _DialTimeSelectorControl__time_picker(isSelected: Equals(_TimePickerModel__time_picker.hourMinuteModeOf(context), _HourMinuteMode__time_picker.hour), text: formattedHour, onTap: () => { _TimePickerModel__time_picker.setHourMinuteMode(context, _HourMinuteMode__time_picker.hour); }, onDoubleTap: () => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onHourDoubleTapped).onHourDoubleTapped()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TimeSelectorSeparator__time_picker : StatelessWidget
{
    public virtual TimeOfDayFormat timeOfDayFormat { get; private set; } = default!;

    internal _TimeSelectorSeparator__time_picker(TimeOfDayFormat timeOfDayFormat)
    {
        this.timeOfDayFormat = timeOfDayFormat;
    }

    internal virtual string _timeSelectorSeparatorValue(TimeOfDayFormat timeOfDayFormat) => timeOfDayFormat switch { TimeOfDayFormat.h_colon_mm_space_a or TimeOfDayFormat.a_space_h_colon_mm or TimeOfDayFormat.H_colon_mm => ":", TimeOfDayFormat.HH_colon_mm => ":", TimeOfDayFormat.HH_dot_mm => ".", TimeOfDayFormat.frenchCanadian => "h", _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
        var states = new HashSet<WidgetState>();
        Color effectiveTextColor = WidgetStateProperty.resolveAs(((timePickerTheme.timeSelectorSeparatorColor?.resolve(states) ?? timePickerTheme.hourMinuteTextColor) ?? (defaultTheme.timeSelectorSeparatorColor?.resolve(states))) ?? defaultTheme.hourMinuteTextColor, states);
        TextStyle effectiveStyle = WidgetStateProperty.resolveAs(((timePickerTheme.timeSelectorSeparatorTextStyle?.resolve(states) ?? timePickerTheme.hourMinuteTextStyle) ?? (defaultTheme.timeSelectorSeparatorTextStyle?.resolve(states))) ?? defaultTheme.hourMinuteTextStyle, states).copyWith(color: effectiveTextColor, height: 1.0);
        double heightLocal = _TimePickerModel__time_picker.entryModeOf(context) switch { TimePickerEntryMode.dial => defaultTheme.hourMinuteSize.height, TimePickerEntryMode.dialOnly => defaultTheme.hourMinuteSize.height, TimePickerEntryMode.input => defaultTheme.hourMinuteInputSize.height, TimePickerEntryMode.inputOnly => defaultTheme.hourMinuteInputSize.height, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return new ExcludeSemantics(child: new SizedBox(width: Equals(timeOfDayFormat, TimeOfDayFormat.frenchCanadian) ? 36 : 24, height: heightLocal, child: new Center(child: new Text(_timeSelectorSeparatorValue(timeOfDayFormat), style: effectiveStyle, textScaler: TextScaler.noScaling))));
    }

}

internal class _DialMinuteControl__time_picker : StatelessWidget
{
    internal _DialMinuteControl__time_picker()
    {
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        string formattedMinute = localizations.formatMinute(selectedTime);
        TimeOfDay nextMinute = selectedTime.replacing(minute: (selectedTime.minute + 1L) % TimeOfDay.minutesPerHour);
        string formattedNextMinute = localizations.formatMinute(nextMinute);
        TimeOfDay previousMinute = selectedTime.replacing(minute: (selectedTime.minute + TimeOfDay.minutesPerHour - 1L) % TimeOfDay.minutesPerHour);
        string formattedPreviousMinute = localizations.formatMinute(previousMinute);
        return new Widgets.Semantics(excludeSemantics: true, value: $"{localizations.timePickerMinuteModeAnnouncement} {formattedMinute}", increasedValue: formattedNextMinute, onIncrease: () =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, nextMinute);
        }, decreasedValue: formattedPreviousMinute, onDecrease: () =>
        {
            _TimePickerModel__time_picker.setSelectedTime(context, previousMinute);
        }, child: new _DialTimeSelectorControl__time_picker(isSelected: Equals(_TimePickerModel__time_picker.hourMinuteModeOf(context), _HourMinuteMode__time_picker.minute), text: formattedMinute, onTap: () => { _TimePickerModel__time_picker.setHourMinuteMode(context, _HourMinuteMode__time_picker.minute); }, onDoubleTap: () => _TimePickerModel__time_picker.of(context, _TimePickerAspect__time_picker.onMinuteDoubleTapped).onMinuteDoubleTapped()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPeriodControl__time_picker : StatelessWidget
{
    public virtual Action<TimeOfDay>? onPeriodChanged { get; private set; }

    internal _DayPeriodControl__time_picker(Action<TimeOfDay>? onPeriodChanged = null)
    {
        this.onPeriodChanged = onPeriodChanged;
    }

    internal virtual void _togglePeriod(BuildContext context)
    {
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        long newHour = (selectedTime.hour + TimeOfDay.hoursPerPeriod) % TimeOfDay.hoursPerDay;
        TimeOfDay newTime = selectedTime.replacing(hour: newHour);
        if (onPeriodChanged is not null)
        {
            onPeriodChanged!(newTime);
        }
        else
        {
            _TimePickerModel__time_picker.setSelectedTime(context, newTime);
        }
    }

    internal virtual void _setAm(BuildContext context)
    {
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        if (Equals(selectedTime.period, DayPeriod.am))
        {
            return;
        }
        _togglePeriod(context);
    }

    internal virtual void _setPm(BuildContext context)
    {
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        if (Equals(selectedTime.period, DayPeriod.pm))
        {
            return;
        }
        _togglePeriod(context);
    }

    public override Widget build(BuildContext context)
    {
        MaterialLocalizations materialLocalizations = MaterialLocalizations.of(context);
        TimePickerThemeData timePickerTheme = _TimePickerModel__time_picker.themeOf(context);
        _TimePickerDefaults__time_picker defaultTheme = _TimePickerModel__time_picker.defaultThemeOf(context);
        TimeOfDay selectedTime = _TimePickerModel__time_picker.selectedTimeOf(context);
        var amSelected = Equals(selectedTime.period, DayPeriod.am);
        bool pmSelected = !amSelected;
        BorderSide resolvedSide = timePickerTheme.dayPeriodBorderSide ?? defaultTheme.dayPeriodBorderSide;
        OutlinedBorder resolvedShape = (timePickerTheme.dayPeriodShape ?? defaultTheme.dayPeriodShape).copyWith(side: resolvedSide);
        Size dayPeriodSize = default!;
        Orientation orientationLocal = default!;
        switch (_TimePickerModel__time_picker.entryModeOf(context))
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    orientationLocal = _TimePickerModel__time_picker.orientationOf(context);
                    dayPeriodSize = orientationLocal switch { Orientation.portrait => defaultTheme.dayPeriodPortraitSize, Orientation.landscape => defaultTheme.dayPeriodLandscapeSize, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                    break;
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    orientationLocal = Orientation.portrait;
                    dayPeriodSize = defaultTheme.dayPeriodInputSize;
                    break;
                }
        }
        var amShape = resolvedShape;
        var pmShape = resolvedShape;
        bool hasRoundedBorder = (resolvedShape is RoundedRectangleBorder) && (((RoundedRectangleBorder)resolvedShape).borderRadius is BorderRadius);
        switch (orientationLocal)
        {
            case Orientation.portrait:
                {
                    if (hasRoundedBorder)
                    {
                        var borderRadiusLocal = ((BorderRadius?)((RoundedRectangleBorder)resolvedShape).borderRadius)!;
                        amShape = DartRuntimePrimitives.ConvertValue<OutlinedBorder>(((RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new BorderRadius(topLeft: borderRadiusLocal.topLeft, topRight: borderRadiusLocal.topRight)));
                        pmShape = DartRuntimePrimitives.ConvertValue<OutlinedBorder>(((RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new BorderRadius(bottomLeft: borderRadiusLocal.bottomLeft, bottomRight: borderRadiusLocal.bottomRight)));
                    }
                    var minInteractiveSize = new Size(dayPeriodSize.width, Math.Max(dayPeriodSize.height, 2L * Widgets.ConstantsLibrary.kMinInteractiveDimension));
                    Widget amButton = new _AmPmButton__time_picker(selected: amSelected, onPressed: () => { _setAm(context); }, label: materialLocalizations.anteMeridiemAbbreviation, padding: EdgeInsets.CreateOnly(top: (minInteractiveSize.height - dayPeriodSize.height) / 2L), shape: amShape);
                    Widget pmButton = new _AmPmButton__time_picker(selected: pmSelected, onPressed: () => { _setPm(context); }, label: materialLocalizations.postMeridiemAbbreviation, padding: EdgeInsets.CreateOnly(bottom: (minInteractiveSize.height - dayPeriodSize.height) / 2L), shape: pmShape);
                    return new _DayPeriodInputPadding__time_picker(minSize: minInteractiveSize, orientation: orientationLocal, child: SizedBox.CreateFromSize(size: minInteractiveSize, child: new Column(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: amButton)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: pmButton)) })));
                }
            case Orientation.landscape:
                {
                    if (hasRoundedBorder)
                    {
                        var borderRadiusAlternate = ((BorderRadius?)((RoundedRectangleBorder)resolvedShape).borderRadius)!;
                        amShape = DartRuntimePrimitives.ConvertValue<OutlinedBorder>(((RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new BorderRadius(topLeft: borderRadiusAlternate.topLeft, bottomLeft: borderRadiusAlternate.bottomLeft)));
                        pmShape = DartRuntimePrimitives.ConvertValue<OutlinedBorder>(((RoundedRectangleBorder)resolvedShape).copyWith(borderRadius: new BorderRadius(topRight: borderRadiusAlternate.topRight, bottomRight: borderRadiusAlternate.bottomRight)));
                    }
                    var minInteractiveSizeLocal = new Size(dayPeriodSize.width, Math.Max(dayPeriodSize.height, Widgets.ConstantsLibrary.kMinInteractiveDimension));
                    Widget amButtonLocal = new _AmPmButton__time_picker(selected: amSelected, onPressed: () => { _setAm(context); }, label: materialLocalizations.anteMeridiemAbbreviation, padding: EdgeInsets.CreateSymmetric(vertical: (minInteractiveSizeLocal.height - dayPeriodSize.height) / 2L), shape: amShape);
                    Widget pmButtonLocal = new _AmPmButton__time_picker(selected: pmSelected, onPressed: () => { _setPm(context); }, label: materialLocalizations.postMeridiemAbbreviation, padding: EdgeInsets.CreateSymmetric(vertical: (minInteractiveSizeLocal.height - dayPeriodSize.height) / 2L), shape: pmShape);
                    return new _DayPeriodInputPadding__time_picker(minSize: minInteractiveSizeLocal, orientation: orientationLocal, child: new SizedBox(height: minInteractiveSizeLocal.height, child: new Row(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: amButtonLocal)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: pmButtonLocal)) })));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _AmPmButton__time_picker : StatelessWidget
{
    public virtual bool selected { get; private set; } = default!;
    public virtual Action onPressed { get; private set; } = default!;
    public virtual string label { get; private set; } = default!;
    public virtual EdgeInsets padding { get; private set; } = default!;
    public virtual OutlinedBorder shape { get; private set; } = default!;

    internal _AmPmButton__time_picker(Action onPressed, bool selected, string label, EdgeInsets padding, OutlinedBorder shape)
    {
        this.onPressed = onPressed;
        this.selected = selected;
        this.label = label;
        this.padding = padding;
        this.shape = shape;
    }

    public override Widget build(BuildContext context)
    {
        var states = ((Func<HashSet<WidgetState>>)(() => { var __collection28792 = new HashSet<WidgetState>(); if (selected) { __collection28792.Add(WidgetState.selected); } return __collection28792; }))();
        TimePickerThemeData timePickerTheme = _TimePickerModel__time_picker.themeOf(context);
        _TimePickerDefaults__time_picker defaultTheme = _TimePickerModel__time_picker.defaultThemeOf(context);
        Color resolvedBackgroundColor = WidgetStateProperty.resolveAs(timePickerTheme.dayPeriodColor ?? defaultTheme.dayPeriodColor, states);
        Color resolvedTextColor = WidgetStateProperty.resolveAs(timePickerTheme.dayPeriodTextColor ?? defaultTheme.dayPeriodTextColor, states);
        TextStyle? resolvedTextStyle = WidgetStateProperty.resolveAs<TextStyle?>(timePickerTheme.dayPeriodTextStyle ?? defaultTheme.dayPeriodTextStyle, states)?.copyWith(color: resolvedTextColor);
        TextScaler buttonTextScaler = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 2.0);
        return new Widgets.Semantics(selected: Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) ? selected : null, @checked: Equals(PlatformLibrary.defaultTargetPlatform, TargetPlatform.iOS) ? null : selected, inMutuallyExclusiveGroup: true, button: true, child: new Padding(padding: padding, child: new Material(clipBehavior: Clip.antiAlias, color: resolvedBackgroundColor, shape: shape, child: new InkWell(onTap: onPressed, child: new Center(child: new Text(label, style: resolvedTextStyle, textScaler: buttonTextScaler))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPeriodInputPadding__time_picker : SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;
    public virtual Orientation orientation { get; private set; } = default!;

    internal _DayPeriodInputPadding__time_picker(Widget child, Size minSize, Orientation orientation) : base(child: child)
    {
        this.minSize = minSize;
        this.orientation = orientation;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderInputPadding__time_picker(minSize, orientation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__time_picker)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderInputPadding__time_picker>)(() =>
{
    var __cascade = __renderObject;
    __cascade.minSize = minSize;
    __cascade.orientation = orientation;
    return __cascade;
}))());
    }

}

public class _RenderInputPadding__time_picker : RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;
    internal virtual Orientation _orientation { get; set; } = default!;

    internal _RenderInputPadding__time_picker(Size _minSize, Orientation _orientation, RenderBox? child = null) : base(child)
    {
        this._minSize = _minSize;
        this._orientation = _orientation;
    }

    public virtual Size minSize
    {
        get => _minSize;
        set
        {
            var __value = value;
            if (Equals(_minSize, __value))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }
    public virtual Orientation orientation
    {
        get => _orientation;
        set
        {
            var __value = value;
            if (Equals(_orientation, __value))
            {
                return;
            }
            _orientation = __value;
            markNeedsLayout();
        }
    }
    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicWidth(height), minSize.width);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicHeight(width), minSize.height);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicWidth(height), minSize.width);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicHeight(width), minSize.height);
        }
        return 0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        if (child is not null)
        {
            Size childSize = layoutChild(child!, constraints);
            double widthLocal = Math.Max(childSize.width, minSize.width);
            double heightLocal = Math.Max(childSize.height, minSize.height);
            return constraints.constrain(new Size(widthLocal, heightLocal));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size drySize = getDryLayout(constraints);
        Size childSize = childLocal.getDryLayout(constraints);
        Offset childOffset = Alignment.center.alongOffset(drySize - childSize);
        return DartRuntimePrimitives.RequireValue(result) + childOffset.dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
        if (child is not null)
        {
            var childParentData = ((BoxParentData?)child!.parentData!)!;
            childParentData.offset = Alignment.center.alongOffset(size - child!.size);
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        if ((position.dx < 0L) || (position.dx > Math.Max(child!.size.width, minSize.width)) || (position.dy < 0L) || (position.dy > Math.Max(child!.size.height, minSize.height)))
        {
            return false;
        }
        Offset newPosition = child!.size.center(Offset.zero);
        newPosition += orientation switch { Orientation.portrait when position.dy > newPosition.dy => new Offset(0, 1), Orientation.landscape when position.dx > newPosition.dx => new Offset(1, 0), Orientation.portrait => new Offset(0, -1), Orientation.landscape => new Offset(-1, 0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(newPosition), position: newPosition, hitTest: (result, position) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(position, newPosition));
            return child!.hitTest(result, position: newPosition);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TappableLabel__time_picker
{
    public virtual long value { get; private set; } = default!;
    public virtual bool inner { get; private set; } = default!;
    public virtual TextPainter painter { get; private set; } = default!;
    public virtual Action onTap { get; private set; } = default!;

    internal _TappableLabel__time_picker(long value, bool inner, TextPainter painter, Action onTap)
    {
        this.value = value;
        this.inner = inner;
        this.painter = painter;
        this.onTap = onTap;
    }

}

public class _DialPainter__time_picker : CustomPainter
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

    internal _DialPainter__time_picker(List<_TappableLabel__time_picker> primaryLabels, List<_TappableLabel__time_picker> selectedLabels, Color backgroundColor, Color handColor, double handWidth, Color dotColor, double dotRadius, double centerRadius, double theta, double radius, TextDirection textDirection, long selectedValue) : base(repaint: PaintingBinding.instance.systemFonts)
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
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchCreated("material", "_DialPainter", this));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        foreach (_TappableLabel__time_picker label in primaryLabels)
        {
            label.painter.dispose();
        }
        foreach (_TappableLabel__time_picker labelLocal in selectedLabels)
        {
            labelLocal.painter.dispose();
        }
        primaryLabels.Clear();
        selectedLabels.Clear();
    }

    public override void paint(Canvas canvas, Size size)
    {
        double dialRadius = Dart_uiLibrary.clampDouble(size.shortestSide / 2L, Time_pickerLibrary._kTimePickerDialMinRadius + dotRadius, double.PositiveInfinity);
        double labelRadius = Dart_uiLibrary.clampDouble(dialRadius - Time_pickerLibrary._kTimePickerDialPadding, Time_pickerLibrary._kTimePickerDialMinRadius, double.PositiveInfinity);
        double innerLabelRadius = Dart_uiLibrary.clampDouble(labelRadius - Time_pickerLibrary._kTimePickerInnerDialOffset, 0, double.PositiveInfinity);
        double handleRadius = Dart_uiLibrary.clampDouble(labelRadius - (((radius < 0.5) ? 1L : 0L) * (labelRadius - innerLabelRadius)), Time_pickerLibrary._kTimePickerDialMinRadius, double.PositiveInfinity);
        var centerLocal = new Offset(size.width / 2L, size.height / 2L);
        var centerPoint = centerLocal;
        canvas.drawCircle(centerPoint, dialRadius, ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = backgroundColor;
    return __cascade;
}))());
        Offset getOffsetForTheta(double theta, double radius)
        {
            return centerLocal + new Offset(radius * Dart_mathLibrary.cos(theta), -radius * Dart_mathLibrary.sin(theta));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        void paintLabels(List<_TappableLabel__time_picker> labels, double radius)
        {
            if (!Enumerable.Any(labels))
            {
                return;
            }
            double labelThetaIncrement = -Time_pickerLibrary._kTwoPi / checked(labels.Count);
            double labelTheta = Dart_mathLibrary.pi / 2L;
            foreach (var labelLocal in labels)
            {
                TextPainter labelPainter = labelLocal.painter;
                var labelOffset = new Offset(-labelPainter.width / 2L, -labelPainter.height / 2L);
                labelPainter.paint(canvas, getOffsetForTheta(labelTheta, radius) + labelOffset);
                labelTheta += labelThetaIncrement;
            }
        }
        void paintInnerOuterLabels(List<_TappableLabel__time_picker>? labels)
        {
            if (labels is null)
            {
                return;
            }
            paintLabels(labels.where((label) => !label.inner).ToList(), labelRadius);
            paintLabels(labels.where((label) => label.inner).ToList(), innerLabelRadius);
        }
        paintInnerOuterLabels(primaryLabels);
        var selectorPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.color = handColor;
    return __cascade;
}))();
        Offset focusedPoint = getOffsetForTheta(theta, handleRadius);
        canvas.drawCircle(centerPoint, centerRadius, selectorPaint);
        canvas.drawCircle(focusedPoint, dotRadius, selectorPaint);
        selectorPaint.strokeWidth = handWidth;
        canvas.drawLine(centerPoint, focusedPoint, selectorPaint);
        double labelThetaIncrementLocal = -Time_pickerLibrary._kTwoPi / checked(primaryLabels.Count);
        if (((theta % labelThetaIncrementLocal) > 0.1) && ((theta % labelThetaIncrementLocal) < 0.45))
        {
            canvas.drawCircle(focusedPoint, 2, ((Func<Paint>)(() =>
{
    var __cascade = selectorPaint;
    __cascade.color = dotColor;
    return __cascade;
}))());
        }
        var focusedRect = Rect.fromCircle(center: focusedPoint, radius: dotRadius);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.clipPath(((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addOval(focusedRect);
    return __cascade;
}))());
    return __cascade;
}))());
        paintInnerOuterLabels(selectedLabels);
        canvas.restore();
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldPainter = (_DialPainter__time_picker)oldDelegate;
        return (!Equals(__oldPainter.primaryLabels, primaryLabels)) || (!Equals(__oldPainter.selectedLabels, selectedLabels)) || (!Equals(__oldPainter.backgroundColor, backgroundColor)) || (!Equals(__oldPainter.handColor, handColor)) || (__oldPainter.theta != theta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _HourDialType__time_picker
{
    twentyFourHourDoubleRing,
    twelveHour
}

public class _Dial__time_picker : StatefulWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual _HourMinuteMode__time_picker hourMinuteMode { get; private set; } = default!;
    public virtual _HourDialType__time_picker hourDialType { get; private set; } = default!;
    public virtual Action<TimeOfDay>? onChanged { get; private set; }
    public virtual Action? onHourSelected { get; private set; }

    internal _Dial__time_picker(TimeOfDay selectedTime, _HourMinuteMode__time_picker hourMinuteMode, _HourDialType__time_picker hourDialType, Action<TimeOfDay>? onChanged, Action? onHourSelected)
    {
        this.selectedTime = selectedTime;
        this.hourMinuteMode = hourMinuteMode;
        this.hourDialType = hourDialType;
        this.onChanged = onChanged;
        this.onHourSelected = onHourSelected;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DialState__time_picker());
}

public class _DialState__time_picker : State<_Dial__time_picker>, SingleTickerProviderStateMixin<_Dial__time_picker>
{
    public virtual ThemeData themeData { get; set; } = default!;
    public virtual MaterialLocalizations localizations { get; set; } = default!;
    public virtual _DialPainter__time_picker? painter { get; set; } = default;
    internal virtual AnimationController _animationController { get; set; } = default!;
    internal virtual Tween<double> _thetaTween { get; set; } = default!;
    internal virtual Animation<double> _theta { get; set; } = default!;
    internal virtual Tween<double> _radiusTween { get; set; } = default!;
    internal virtual Animation<double> _radius { get; set; } = default!;
    internal virtual bool _dragging { get; set; } = false;
    internal virtual Offset? _position { get; set; } = default;
    internal virtual Offset? _center { get; set; } = default;
    internal virtual Size? _dialSize { get; set; } = default;
    internal static List<TimeOfDay> _amHours = new List<TimeOfDay> { new TimeOfDay(hour: 12L, minute: 0L), new TimeOfDay(hour: 1L, minute: 0L), new TimeOfDay(hour: 2L, minute: 0L), new TimeOfDay(hour: 3L, minute: 0L), new TimeOfDay(hour: 4L, minute: 0L), new TimeOfDay(hour: 5L, minute: 0L), new TimeOfDay(hour: 6L, minute: 0L), new TimeOfDay(hour: 7L, minute: 0L), new TimeOfDay(hour: 8L, minute: 0L), new TimeOfDay(hour: 9L, minute: 0L), new TimeOfDay(hour: 10L, minute: 0L), new TimeOfDay(hour: 11L, minute: 0L) };
    internal static List<TimeOfDay> _twentyFourHours = new List<TimeOfDay> { new TimeOfDay(hour: 0L, minute: 0L), new TimeOfDay(hour: 1L, minute: 0L), new TimeOfDay(hour: 2L, minute: 0L), new TimeOfDay(hour: 3L, minute: 0L), new TimeOfDay(hour: 4L, minute: 0L), new TimeOfDay(hour: 5L, minute: 0L), new TimeOfDay(hour: 6L, minute: 0L), new TimeOfDay(hour: 7L, minute: 0L), new TimeOfDay(hour: 8L, minute: 0L), new TimeOfDay(hour: 9L, minute: 0L), new TimeOfDay(hour: 10L, minute: 0L), new TimeOfDay(hour: 11L, minute: 0L), new TimeOfDay(hour: 12L, minute: 0L), new TimeOfDay(hour: 13L, minute: 0L), new TimeOfDay(hour: 14L, minute: 0L), new TimeOfDay(hour: 15L, minute: 0L), new TimeOfDay(hour: 16L, minute: 0L), new TimeOfDay(hour: 17L, minute: 0L), new TimeOfDay(hour: 18L, minute: 0L), new TimeOfDay(hour: 19L, minute: 0L), new TimeOfDay(hour: 20L, minute: 0L), new TimeOfDay(hour: 21L, minute: 0L), new TimeOfDay(hour: 22L, minute: 0L), new TimeOfDay(hour: 23L, minute: 0L) };
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _animationController = new AnimationController(duration: Time_pickerLibrary._kDialAnimateDuration, vsync: this);
        _thetaTween = new Tween<double>(begin: _getThetaForTime(widget.selectedTime));
        _radiusTween = new Tween<double>(begin: _getRadiusForTime(widget.selectedTime));
        _theta = ((Func<Animation<double>>)(() =>
{
    var __cascade = _animationController.drive(new CurveTween(curve: CurvesLibrary.standardEasing)).drive(_thetaTween);
    __cascade.addListener(() =>
    {
        setState(() =>
        {
        });
    });
    return __cascade;
}))();
        _radius = ((Func<Animation<double>>)(() =>
{
    var __cascade = _animationController.drive(new CurveTween(curve: CurvesLibrary.standardEasing)).drive(_radiusTween);
    __cascade.addListener(() =>
    {
        setState(() =>
        {
        });
    });
    return __cascade;
}))();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        themeData = Theme.of(context);
        localizations = MaterialLocalizations.of(context);
    }

    public override void didUpdateWidget(_Dial__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(widget.hourMinuteMode, oldWidget.hourMinuteMode)) || (!Equals(widget.selectedTime, oldWidget.selectedTime)))
        {
            if (!_dragging)
            {
                _animateTo(_getThetaForTime(widget.selectedTime), _getRadiusForTime(widget.selectedTime));
            }
        }
    }

    public override void dispose()
    {
        _animationController.dispose();
        painter?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal static double _nearest(double target, double a, double b)
    {
        return ((target - a).abs() < (target - b).abs()) ? a : b;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _animateTo(double targetTheta, double targetRadius)
    {
        void animateToValue(double target, Animation<double> animation, Tween<double> tween, AnimationController controller, double min, double max)
        {
            double beginValue = _nearest(target, animation.value, max);
            beginValue = _nearest(target, beginValue, min);
            DartRuntimePrimitives.Ignore(((Func<Tween<double>>)(() =>
{
    var __cascade = tween;
    __cascade.begin = beginValue;
    __cascade.end = target;
    return __cascade;
}))());
            DartRuntimePrimitives.Ignore(((Func<AnimationController>)(() =>
{
    var __cascade = controller;
    __cascade.value = 0;
    __cascade.forward();
    return __cascade;
}))());
        }
        animateToValue(target: targetTheta, animation: _theta, tween: _thetaTween, controller: _animationController, min: _theta.value - Time_pickerLibrary._kTwoPi, max: _theta.value + Time_pickerLibrary._kTwoPi);
        animateToValue(target: targetRadius, animation: _radius, tween: _radiusTween, controller: _animationController, min: 0, max: 1);
    }

    internal virtual double _getRadiusForTime(TimeOfDay time)
    {
        switch (widget.hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    return widget.hourDialType switch { _HourDialType__time_picker.twentyFourHourDoubleRing => (time.hour >= 12L) ? 0 : 1, _HourDialType__time_picker.twelveHour => 1, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
        long hoursFactor = widget.hourDialType switch { _HourDialType__time_picker.twentyFourHourDoubleRing => TimeOfDay.hoursPerPeriod, _HourDialType__time_picker.twelveHour => TimeOfDay.hoursPerPeriod, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double fraction = widget.hourMinuteMode switch { _HourMinuteMode__time_picker.hour => (double)(time.hour % hoursFactor) / hoursFactor, _HourMinuteMode__time_picker.minute => (double)time.minute / TimeOfDay.minutesPerHour, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var theta = (Dart_mathLibrary.pi / 2) - fraction * Time_pickerLibrary._kTwoPi;
        return (theta % Time_pickerLibrary._kTwoPi + Time_pickerLibrary._kTwoPi) % Time_pickerLibrary._kTwoPi;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TimeOfDay _getTimeForTheta(double theta, bool roundMinutes = false, double radius = default!)
    {
        // Dart modulo is nonnegative; CLR remainder is negative on the left half of the dial.
        double fraction = ((0.25 - theta / Time_pickerLibrary._kTwoPi) % 1 + 1) % 1;
        switch (widget.hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    long newHour = default!;
                    switch (widget.hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                newHour = (fraction * TimeOfDay.hoursPerPeriod).round() % TimeOfDay.hoursPerPeriod;
                                if (radius < 0.5)
                                {
                                    newHour = newHour + TimeOfDay.hoursPerPeriod;
                                }
                                break;
                            }
                        case _HourDialType__time_picker.twelveHour:
                            {
                                newHour = (fraction * TimeOfDay.hoursPerPeriod).round() % TimeOfDay.hoursPerPeriod;
                                newHour = newHour + widget.selectedTime.periodOffset;
                                break;
                            }
                    }
                    return widget.selectedTime.replacing(hour: newHour);
                }
            case _HourMinuteMode__time_picker.minute:
                {
                    long minuteLocal = (fraction * TimeOfDay.minutesPerHour).round() % TimeOfDay.minutesPerHour;
                    if (roundMinutes)
                    {
                        minuteLocal = checked((minuteLocal + 2L) / 5L) * 5L % TimeOfDay.minutesPerHour;
                    }
                    return widget.selectedTime.replacing(minute: minuteLocal);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TimeOfDay _notifyOnChangedIfNeeded(bool roundMinutes = false)
    {
        TimeOfDay current = _getTimeForTheta(_theta.value, roundMinutes: roundMinutes, radius: _radius.value);
        if (widget.onChanged is null)
        {
            return current;
        }
        if (!Equals(current, widget.selectedTime))
        {
            widget.onChanged!(current);
        }
        return current;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateThetaForPan(bool roundMinutes = false)
    {
        setState(() =>
        {
            Offset offset = DartRuntimePrimitives.RequireValue(_position) - DartRuntimePrimitives.RequireValue(_center);
            double labelRadius = (DartRuntimePrimitives.RequireValue(_dialSize).shortestSide / 2L) - Time_pickerLibrary._kTimePickerDialPadding;
            double innerRadius = labelRadius - Time_pickerLibrary._kTimePickerInnerDialOffset;
            double angle = (Dart_mathLibrary.atan2(offset.dx, offset.dy) - (Dart_mathLibrary.pi / 2L)) % Time_pickerLibrary._kTwoPi;
            double radiusLocal = Dart_uiLibrary.clampDouble((offset.distance - innerRadius) / Time_pickerLibrary._kTimePickerInnerDialOffset, 0, 1);
            if (roundMinutes)
            {
                angle = _getThetaForTime(_getTimeForTheta(angle, roundMinutes: roundMinutes, radius: radiusLocal));
            }
            DartRuntimePrimitives.Ignore(((Func<Tween<double>>)(() =>
            {
                var __cascade = _thetaTween;
                __cascade.begin = angle;
                __cascade.end = angle;
                return __cascade;
            }))());
            DartRuntimePrimitives.Ignore(((Func<Tween<double>>)(() =>
            {
                var __cascade = _radiusTween;
                __cascade.begin = radiusLocal;
                __cascade.end = radiusLocal;
                return __cascade;
            }))());
        });
    }

    internal virtual void _handlePanStart(Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => !_dragging);
        _dragging = true;
        var box = ((RenderBox?)context.findRenderObject()!)!;
        _position = box.globalToLocal(details.globalPosition);
        _dialSize = box.size;
        _center = DartRuntimePrimitives.RequireValue(_dialSize).center(Offset.zero);
        _updateThetaForPan();
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _handlePanUpdate(Gestures.DragUpdateDetails details)
    {
        _position = DartRuntimePrimitives.RequireValue(_position) + details.delta;
        _updateThetaForPan();
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _handlePanEnd(Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => _dragging);
        _dragging = false;
        _position = null;
        _center = null;
        _dialSize = null;
        _animateTo(_getThetaForTime(widget.selectedTime), _getRadiusForTime(widget.selectedTime));
        if (Equals(widget.hourMinuteMode, _HourMinuteMode__time_picker.hour))
        {
            widget.onHourSelected?.Invoke();
        }
    }

    internal virtual void _handleTapUp(Gestures.TapUpDetails details)
    {
        var box = ((RenderBox?)context.findRenderObject()!)!;
        _position = box.globalToLocal(details.globalPosition);
        _center = box.size.center(Offset.zero);
        _dialSize = box.size;
        _updateThetaForPan(roundMinutes: true);
        _notifyOnChangedIfNeeded(roundMinutes: true);
        if (Equals(widget.hourMinuteMode, _HourMinuteMode__time_picker.hour))
        {
            widget.onHourSelected?.Invoke();
        }
        TimeOfDay time = _getTimeForTheta(_theta.value, roundMinutes: true, radius: _radius.value);
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
            return widget.selectedTime.period switch { DayPeriod.am => new TimeOfDay(hour: hour, minute: widget.selectedTime.minute), DayPeriod.pm => new TimeOfDay(hour: hour + TimeOfDay.hoursPerPeriod, minute: widget.selectedTime.minute), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        switch (widget.hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    switch (widget.hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                time = new TimeOfDay(hour: hour, minute: widget.selectedTime.minute);
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
        DartRuntimePrimitives.Ignore(((Func<Tween<double>>)(() =>
{
    var __cascade = _thetaTween;
    __cascade.begin = angle;
    __cascade.end = angle;
    return __cascade;
}))());
        _notifyOnChangedIfNeeded();
    }

    internal virtual void _selectMinute(long minute)
    {
        var time = new TimeOfDay(hour: widget.selectedTime.hour, minute: minute);
        double angle = _getThetaForTime(time);
        DartRuntimePrimitives.Ignore(((Func<Tween<double>>)(() =>
{
    var __cascade = _thetaTween;
    __cascade.begin = angle;
    __cascade.end = angle;
    return __cascade;
}))());
        _notifyOnChangedIfNeeded();
    }

    internal virtual _TappableLabel__time_picker _buildTappableLabel(TextStyle? textStyle, long selectedValue, long value, bool inner, string label, Action onTap)
    {
        return new _TappableLabel__time_picker(value: value, inner: inner, painter: ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter(text: new TextSpan(style: textStyle, text: label), textDirection: TextDirection.ltr, textScaler: MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 2.0));
    __cascade.layout();
    return __cascade;
}))(), onTap: onTap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<_TappableLabel__time_picker> _build24HourRing(TextStyle? textStyle, long selectedValue)
    {
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection52599 = new List<_TappableLabel__time_picker>(); {
                foreach (var timeOfDay in _twentyFourHours)
                {
                    __collection52599.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: timeOfDay.hour >= 12L, value: timeOfDay.hour, label: (timeOfDay.hour != 0L) ? localizations.formatDecimal(timeOfDay.hour) : localizations.formatHour(timeOfDay, alwaysUse24HourFormat: true), onTap: () =>
                    {
                        _selectHour(timeOfDay.hour);
                    }));
                }
            }
            return __collection52599;
        }))();
    }

    internal virtual List<_TappableLabel__time_picker> _build12HourRing(TextStyle? textStyle, long selectedValue)
    {
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection53903 = new List<_TappableLabel__time_picker>(); foreach (var timeOfDay in _amHours)
            {
                __collection53903.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: false, value: timeOfDay.hour, label: localizations.formatHour(timeOfDay, alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context)), onTap: () =>
                {
                    _selectHour(timeOfDay.hour);
                }));
            }
            return __collection53903;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<_TappableLabel__time_picker> _buildMinutes(TextStyle? textStyle, long selectedValue)
    {
        var minuteMarkerValues = new List<TimeOfDay> { new TimeOfDay(hour: 0L, minute: 0L), new TimeOfDay(hour: 0L, minute: 5L), new TimeOfDay(hour: 0L, minute: 10L), new TimeOfDay(hour: 0L, minute: 15L), new TimeOfDay(hour: 0L, minute: 20L), new TimeOfDay(hour: 0L, minute: 25L), new TimeOfDay(hour: 0L, minute: 30L), new TimeOfDay(hour: 0L, minute: 35L), new TimeOfDay(hour: 0L, minute: 40L), new TimeOfDay(hour: 0L, minute: 45L), new TimeOfDay(hour: 0L, minute: 50L), new TimeOfDay(hour: 0L, minute: 55L) };
        return ((Func<List<_TappableLabel__time_picker>>)(() =>
        {
            var __collection55004 = new List<_TappableLabel__time_picker>(); foreach (var timeOfDay in minuteMarkerValues)
            {
                __collection55004.Add(_buildTappableLabel(textStyle: textStyle, selectedValue: selectedValue, inner: false, value: timeOfDay.minute, label: localizations.formatMinute(timeOfDay), onTap: () =>
                {
                    _selectMinute(timeOfDay.minute);
                }));
            }
            return __collection55004;
        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Time_pickerLibrary._debugDialTimePickerEntryMode(context));
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
        Color backgroundColorLocal = timePickerTheme.dialBackgroundColor ?? defaultTheme.dialBackgroundColor;
        Color dialHandColorLocal = timePickerTheme.dialHandColor ?? defaultTheme.dialHandColor;
        TextStyle labelStyle = timePickerTheme.dialTextStyle ?? defaultTheme.dialTextStyle;
        Color dialTextUnselectedColor = WidgetStateProperty.resolveAs(timePickerTheme.dialTextColor ?? defaultTheme.dialTextColor, new HashSet<WidgetState>());
        Color dialTextSelectedColor = WidgetStateProperty.resolveAs(timePickerTheme.dialTextColor ?? defaultTheme.dialTextColor, new HashSet<WidgetState> { WidgetState.selected });
        TextStyle resolvedUnselectedLabelStyle = labelStyle.copyWith(color: dialTextUnselectedColor);
        TextStyle resolvedSelectedLabelStyle = labelStyle.copyWith(color: dialTextSelectedColor);
        var dotColorLocal = dialTextSelectedColor;
        List<_TappableLabel__time_picker> primaryLabelsLocal = default!;
        List<_TappableLabel__time_picker> selectedLabelsLocal = default!;
        long selectedDialValue = default!;
        double radiusValue = default!;
        switch (widget.hourMinuteMode)
        {
            case _HourMinuteMode__time_picker.hour:
                {
                    switch (widget.hourDialType)
                    {
                        case _HourDialType__time_picker.twentyFourHourDoubleRing:
                            {
                                selectedDialValue = widget.selectedTime.hour;
                                primaryLabelsLocal = _build24HourRing(textStyle: resolvedUnselectedLabelStyle, selectedValue: selectedDialValue);
                                selectedLabelsLocal = _build24HourRing(textStyle: resolvedSelectedLabelStyle, selectedValue: selectedDialValue);
                                radiusValue = _radius.value;
                                break;
                            }
                        case _HourDialType__time_picker.twelveHour:
                            {
                                selectedDialValue = widget.selectedTime.hourOfPeriod;
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
                    selectedDialValue = widget.selectedTime.minute;
                    primaryLabelsLocal = _buildMinutes(textStyle: resolvedUnselectedLabelStyle, selectedValue: selectedDialValue);
                    selectedLabelsLocal = _buildMinutes(textStyle: resolvedSelectedLabelStyle, selectedValue: selectedDialValue);
                    radiusValue = 1;
                    break;
                }
        }
        painter?.dispose();
        painter = new _DialPainter__time_picker(selectedValue: selectedDialValue, primaryLabels: primaryLabelsLocal, selectedLabels: selectedLabelsLocal, backgroundColor: backgroundColorLocal, handColor: dialHandColorLocal, handWidth: defaultTheme.handWidth, dotColor: dotColorLocal, dotRadius: defaultTheme.dotRadius, centerRadius: defaultTheme.centerRadius, theta: _theta.value, radius: radiusValue, textDirection: Directionality.of(context));
        return new GestureDetector(excludeFromSemantics: true, onPanStart: _handlePanStart, onPanUpdate: _handlePanUpdate, onPanEnd: _handlePanEnd, onTapUp: _handleTapUp, child: new CustomPaint(painter: painter));
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
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
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new DiagnosticsProperty<Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _TimePickerInput__time_picker : StatefulWidget
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

internal class _TimePickerInputState__time_picker : State<_TimePickerInput__time_picker>, RestorationMixin<_TimePickerInput__time_picker>
{
    private bool __late__selectedTime_initialized;
    private RestorableTimeOfDay __late__selectedTime = default!;
    internal virtual RestorableTimeOfDay _selectedTime
    {
        get
        {
            if (!__late__selectedTime_initialized)
            {
                __late__selectedTime = new RestorableTimeOfDay(widget.initialSelectedTime);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    public virtual RestorableBool hourHasError { get; private set; } = new RestorableBool(false);
    public virtual RestorableBool minuteHasError { get; private set; } = new RestorableBool(false);
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } = new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        _selectedTime.dispose();
        hourHasError.dispose();
        minuteHasError.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => widget.restorationId;
    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_selectedTime, "selected_time");
        registerForRestoration(hourHasError, "hour_has_error");
        registerForRestoration(minuteHasError, "minute_has_error");
    }

    internal virtual long? _parseHour(string? value)
    {
        if (value is null)
        {
            return null;
        }
        long? newHour = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));
        if (newHour is null)
        {
            return null;
        }
        if (MediaQuery.alwaysUse24HourFormatOf(context))
        {
            if ((newHour >= 0L) && (DartRuntimePrimitives.RequireValue(newHour) < 24L))
            {
                return DartRuntimePrimitives.RequireValue(newHour);
            }
        }
        else
        {
            if ((DartRuntimePrimitives.RequireValue(newHour) > 0L) && (DartRuntimePrimitives.RequireValue(newHour) < 13L))
            {
                if (Equals(_selectedTime.value.period, DayPeriod.pm) && (DartRuntimePrimitives.RequireValue(newHour) != 12L) || Equals(_selectedTime.value.period, DayPeriod.am) && (DartRuntimePrimitives.RequireValue(newHour) == 12L))
                {
                    newHour = (DartRuntimePrimitives.RequireValue(newHour) + TimeOfDay.hoursPerPeriod) % TimeOfDay.hoursPerDay;
                }
                return DartRuntimePrimitives.RequireValue(newHour);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _parseMinute(string? value)
    {
        if (value is null)
        {
            return null;
        }
        long? newMinute = DartRuntimePrimitives.ConvertValue<long?>(Dart_coreLibrary.tryParse(value));
        if (newMinute is null)
        {
            return null;
        }
        if ((newMinute >= 0L) && (DartRuntimePrimitives.RequireValue(newMinute) < 60L))
        {
            return DartRuntimePrimitives.RequireValue(newMinute);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleHourSavedSubmitted(string? value)
    {
        long? newHour = _parseHour(value);
        if (newHour is not null)
        {
            long newHour__62523__value62560 = DartRuntimePrimitives.RequireValue(newHour);
            _selectedTime.value = new TimeOfDay(hour: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(newHour__62523__value62560)), minute: _selectedTime.value.minute);
            _TimePickerModel__time_picker.setSelectedTime(context, _selectedTime.value);
            FocusScope.of(context).requestFocus();
        }
    }

    internal virtual void _handleHourChanged(string value)
    {
        long? newHour = _parseHour(value);
        if ((newHour is not null) && (value.Length == 2L))
        {
            long newHour__62852__value62889 = DartRuntimePrimitives.RequireValue(newHour);
            FocusScope.of(context).nextFocus();
        }
    }

    internal virtual void _handleMinuteSavedSubmitted(string? value)
    {
        long? newMinute = _parseMinute(value);
        if (newMinute is not null)
        {
            long newMinute__63120__value63161 = DartRuntimePrimitives.RequireValue(newMinute);
            _selectedTime.value = new TimeOfDay(hour: _selectedTime.value.hour, minute: long.Parse(value!, System.Globalization.CultureInfo.InvariantCulture));
            _TimePickerModel__time_picker.setSelectedTime(context, _selectedTime.value);
            FocusScope.of(context).unfocus();
        }
    }

    internal virtual void _handleDayPeriodChanged(TimeOfDay value)
    {
        _selectedTime.value = value;
        _TimePickerModel__time_picker.setSelectedTime(context, _selectedTime.value);
    }

    internal virtual string? _validateHour(string? value)
    {
        long? newHour = _parseHour(value);
        setState(() =>
        {
            hourHasError.value = newHour is null;
        });
        return (newHour is null) ? "" : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string? _validateMinute(string? value)
    {
        long? newMinute = _parseMinute(value);
        setState(() =>
        {
            minuteHasError.value = newMinute is null;
        });
        return (newMinute is null) ? "" : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        TimeOfDayFormat timeOfDayFormatLocal = MaterialLocalizations.of(context).timeOfDayFormat(alwaysUse24HourFormat: _TimePickerModel__time_picker.use24HourFormatOf(context));
        var use24HourDials = !Equals(TimeLibrary.hourFormat(of: timeOfDayFormatLocal), HourFormat.h);
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = _TimePickerModel__time_picker.themeOf(context);
        _TimePickerDefaults__time_picker defaultTheme = _TimePickerModel__time_picker.defaultThemeOf(context);
        TextStyle hourMinuteStyle = timePickerTheme.hourMinuteTextStyle ?? defaultTheme.hourMinuteTextStyle;
        double minInteractiveVerticalPadding = Math.Max(0, (2L * Widgets.ConstantsLibrary.kMinInteractiveDimension) - defaultTheme.dayPeriodInputSize.height);
        return new Padding(padding: EdgeInsets.zero, child: new Column(crossAxisAlignment: CrossAxisAlignment.start, children: ((Func<List<Widget>>)(() => { var __collection65491 = new List<Widget>(); __collection65491.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(bottom: 20L - (minInteractiveVerticalPadding / 2L)), child: new Text(widget.helpText, style: _TimePickerModel__time_picker.themeOf(context).helpTextStyle ?? _TimePickerModel__time_picker.defaultThemeOf(context).helpTextStyle)))); __collection65491.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Row(crossAxisAlignment: CrossAxisAlignment.start, children: ((Func<List<Widget>>)(() => { var __collection66075 = new List<Widget>(); if (!use24HourDials && Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm)) { __collection66075.AddRange(new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(end: 12), child: new _DayPeriodControl__time_picker(onPeriodChanged: _handleDayPeriodChanged))) }); } __collection66075.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(top: minInteractiveVerticalPadding / 2L), child: new Row(crossAxisAlignment: CrossAxisAlignment.start, textDirection: TextDirection.ltr, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: ((Func<List<Widget>>)(() => { var __collection67027 = new List<Widget>(); __collection67027.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateOnly(bottom: 10), child: new _HourTextField__time_picker(restorationId: "hour_text_field", selectedTime: _selectedTime.value, style: hourMinuteStyle, autofocus: widget.autofocusHour, inputAction: TextInputAction.next, validator: _validateHour, onSavedSubmitted: _handleHourSavedSubmitted, onChanged: _handleHourChanged, hourLabelText: widget.hourLabelText, emptyInitialTime: widget.emptyInitialTime)))); if (!hourHasError.value && !minuteHasError.value) { __collection67027.Add(DartRuntimePrimitives.ConvertValue<Widget>(new ExcludeSemantics(child: new Text(widget.hourLabelText ?? MaterialLocalizations.of(context).timePickerHourLabel, style: theme.textTheme.bodySmall, maxLines: 1L, overflow: TextOverflow.ellipsis)))); } return __collection67027; }))()))), DartRuntimePrimitives.ConvertValue<Widget>(new _TimeSelectorSeparator__time_picker(timeOfDayFormat: timeOfDayFormatLocal)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: ((Func<List<Widget>>)(() => { var __collection68842 = new List<Widget>(); __collection68842.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateOnly(bottom: 10), child: new _MinuteTextField__time_picker(restorationId: "minute_text_field", selectedTime: _selectedTime.value, style: hourMinuteStyle, autofocus: widget.autofocusMinute, inputAction: TextInputAction.done, validator: _validateMinute, onSavedSubmitted: _handleMinuteSavedSubmitted, minuteLabelText: widget.minuteLabelText, emptyInitialTime: widget.emptyInitialTime)))); if (!hourHasError.value && !minuteHasError.value) { __collection68842.Add(DartRuntimePrimitives.ConvertValue<Widget>(new ExcludeSemantics(child: new Text(widget.minuteLabelText ?? MaterialLocalizations.of(context).timePickerMinuteLabel, style: theme.textTheme.bodySmall, maxLines: 1L, overflow: TextOverflow.ellipsis)))); } return __collection68842; }))()))) }))))); if (!use24HourDials && (!Equals(timeOfDayFormatLocal, TimeOfDayFormat.a_space_h_colon_mm))) { __collection66075.AddRange(new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 12), child: new _DayPeriodControl__time_picker(onPeriodChanged: _handleDayPeriodChanged))) }); } return __collection66075; }))()))); if (hourHasError.value || minuteHasError.value) { __collection65491.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Text(widget.errorInvalidText ?? MaterialLocalizations.of(context).invalidTimeLabel, style: theme.textTheme.bodyMedium!.copyWith(color: theme.colorScheme.error)))); } else { __collection65491.Add(DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: 2))); } return __collection65491; }))()));
    }

    public virtual RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
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
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(((Func<List<DiagnosticsNode>>)(() => { var __collection41817 = new List<DiagnosticsNode>(); __collection41817.Add(new ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<IRestorableProperty, DiagnosticsNode>((property) => new ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

internal class _HourTextField__time_picker : StatelessWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual TextStyle style { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual TextInputAction inputAction { get; private set; } = default!;
    public virtual Func<string?, string?> validator { get; private set; } = default!;
    public virtual Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual Action<string> onChanged { get; private set; } = default!;
    public virtual string? hourLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _HourTextField__time_picker(TimeOfDay selectedTime, TextStyle style, bool? autofocus, TextInputAction inputAction, Func<string?, string?> validator, Action<string?> onSavedSubmitted, Action<string> onChanged, string? hourLabelText, bool emptyInitialTime, string? restorationId = null)
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

    public override Widget build(BuildContext context)
    {
        return new _HourMinuteTextField__time_picker(restorationId: restorationId, selectedTime: selectedTime, isHour: true, autofocus: autofocus, inputAction: inputAction, style: style, semanticHintText: hourLabelText ?? MaterialLocalizations.of(context).timePickerHourLabel, validator: validator, onSavedSubmitted: onSavedSubmitted, emptyInitialTime: emptyInitialTime, onChanged: onChanged);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MinuteTextField__time_picker : StatelessWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual TextStyle style { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual TextInputAction inputAction { get; private set; } = default!;
    public virtual Func<string?, string?> validator { get; private set; } = default!;
    public virtual Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual string? minuteLabelText { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _MinuteTextField__time_picker(TimeOfDay selectedTime, TextStyle style, bool? autofocus, TextInputAction inputAction, Func<string?, string?> validator, Action<string?> onSavedSubmitted, string? minuteLabelText, bool emptyInitialTime, string? restorationId = null)
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

    public override Widget build(BuildContext context)
    {
        return new _HourMinuteTextField__time_picker(restorationId: restorationId, selectedTime: selectedTime, isHour: false, autofocus: autofocus, inputAction: inputAction, style: style, semanticHintText: minuteLabelText ?? MaterialLocalizations.of(context).timePickerMinuteLabel, validator: validator, emptyInitialTime: emptyInitialTime, onSavedSubmitted: onSavedSubmitted);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HourMinuteTextField__time_picker : StatefulWidget
{
    public virtual TimeOfDay selectedTime { get; private set; } = default!;
    public virtual bool isHour { get; private set; } = default!;
    public virtual bool? autofocus { get; private set; }
    public virtual TextInputAction inputAction { get; private set; } = default!;
    public virtual TextStyle style { get; private set; } = default!;
    public virtual string semanticHintText { get; private set; } = default!;
    public virtual Func<string?, string?> validator { get; private set; } = default!;
    public virtual Action<string?> onSavedSubmitted { get; private set; } = default!;
    public virtual Action<string>? onChanged { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool emptyInitialTime { get; private set; } = default!;

    internal _HourMinuteTextField__time_picker(TimeOfDay selectedTime, bool isHour, bool? autofocus, TextInputAction inputAction, TextStyle style, string semanticHintText, Func<string?, string?> validator, Action<string?> onSavedSubmitted, string? restorationId = null, bool emptyInitialTime = default!, Action<string>? onChanged = null)
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

internal class _HourMinuteTextFieldState__time_picker : State<_HourMinuteTextField__time_picker>, RestorationMixin<_HourMinuteTextField__time_picker>
{
    public virtual RestorableTextEditingController controller { get; private set; } = RestorableTextEditingController.Create();
    public virtual RestorableBool controllerHasBeenSet { get; private set; } = new RestorableBool(false);
    public virtual FocusNode focusNode { get; set; } = default!;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } = new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public override void initState()
    {
        base.initState();
        focusNode = ((Func<FocusNode>)(() =>
{
    var __cascade = new FocusNode();
    __cascade.addListener(() =>
    {
        setState(() =>
        {
            if (Foundation.ConstantsLibrary.kIsWeb && focusNode.hasFocus && (Focus_managerLibrary.primaryFocus?.context is not null))
            {
                Actions.maybeInvoke(Focus_managerLibrary.primaryFocus!.context!, new SelectAllTextIntent(SelectionChangedCause.keyboard));
            }
        });
    });
    return __cascade;
}))();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
        if (!controllerHasBeenSet.value)
        {
            controllerHasBeenSet.value = true;
            string initialTextValue = widget.emptyInitialTime ? "" : _formattedValue;
            controller.value.value = new TextEditingValue(text: initialTextValue);
        }
    }

    public override void dispose()
    {
        controller.dispose();
        controllerHasBeenSet.dispose();
        focusNode.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => widget.restorationId;
    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(controller, "text_editing_controller");
        registerForRestoration(controllerHasBeenSet, "has_controller_been_set");
    }

    internal virtual string _formattedValue
    {
        get
        {
            bool alwaysUse24HourFormatLocal = MediaQuery.alwaysUse24HourFormatOf(context);
            MaterialLocalizations localizations = MaterialLocalizations.of(context);
            return !widget.isHour ? localizations.formatMinute(widget.selectedTime) : localizations.formatHour(widget.selectedTime, alwaysUse24HourFormat: alwaysUse24HourFormatLocal);
        }
    }
    public override Widget build(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        TimePickerThemeData timePickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
        bool alwaysUse24HourFormat = MediaQuery.alwaysUse24HourFormatOf(context);
        InputDecorationThemeData inputDecorationThemeLocal = timePickerTheme.inputDecorationTheme ?? defaultTheme.inputDecorationTheme;
        InputDecoration inputDecoration = new InputDecoration(errorStyle: defaultTheme.inputDecorationTheme.errorStyle).applyDefaults(inputDecorationThemeLocal);
        string? hintTextLocal = (focusNode.hasFocus || widget.emptyInitialTime) ? null : _formattedValue;
        Color startingFillColor = (timePickerTheme.inputDecorationTheme?.fillColor ?? timePickerTheme.hourMinuteColor) ?? defaultTheme.hourMinuteColor;
        Color fillColorLocal = default!;
        {
            fillColorLocal = WidgetStateProperty.resolveAs(startingFillColor, ((Func<HashSet<WidgetState>>)(() => { var __collection78509 = new HashSet<WidgetState>(); if (focusNode.hasFocus) { __collection78509.Add(WidgetState.focused); } if (focusNode.hasFocus) { __collection78509.Add(WidgetState.selected); } return __collection78509; }))());
        }
        inputDecoration = inputDecoration.copyWith(hintText: hintTextLocal, fillColor: fillColorLocal);
        var states = ((Func<HashSet<WidgetState>>)(() => { var __collection78850 = new HashSet<WidgetState>(); if (focusNode.hasFocus) { __collection78850.Add(WidgetState.focused); } if (focusNode.hasFocus) { __collection78850.Add(WidgetState.selected); } return __collection78850; }))();
        Color effectiveTextColor = WidgetStateProperty.resolveAs(timePickerTheme.hourMinuteTextColor ?? defaultTheme.hourMinuteTextColor, states);
        TextStyle effectiveStyle = WidgetStateProperty.resolveAs(widget.style, states).copyWith(color: effectiveTextColor);
        return SizedBox.CreateFromSize(size: alwaysUse24HourFormat ? defaultTheme.hourMinuteInputSize24Hour : defaultTheme.hourMinuteInputSize, child: MediaQuery.withNoTextScaling(child: new UnmanagedRestorationScope(bucket: bucket, child: new Widgets.Semantics(label: widget.semanticHintText, child: new TextFormField(restorationId: "hour_minute_text_form_field", autofocus: widget.autofocus ?? false, expands: true, maxLines: null, inputFormatters: new List<TextInputFormatter> { new LengthLimitingTextInputFormatter(2L) }, focusNode: focusNode, textAlign: TextAlign.center, textInputAction: widget.inputAction, keyboardType: TextInputType.number, style: effectiveStyle, controller: controller.value, decoration: inputDecoration, validator: widget.validator, onEditingComplete: () => { widget.onSavedSubmitted(controller.value.text); }, onSaved: widget.onSavedSubmitted, onFieldSubmitted: widget.onSavedSubmitted, onChanged: widget.onChanged)))));
    }

    public virtual RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
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
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(((Func<List<DiagnosticsNode>>)(() => { var __collection41817 = new List<DiagnosticsNode>(); __collection41817.Add(new ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<IRestorableProperty, DiagnosticsNode>((property) => new ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public delegate void EntryModeChangeCallback(TimePickerEntryMode mode);

public class TimePickerDialog : StatefulWidget
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
    public virtual Orientation? orientation { get; private set; }
    public virtual Action<TimePickerEntryMode>? onEntryModeChanged { get; private set; }
    public virtual Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual Icon? switchToTimerEntryModeIcon { get; private set; }
    public virtual bool emptyInitialInput { get; private set; } = default!;

    public TimePickerDialog(Key? key = null, TimeOfDay initialTime = default!, string? cancelText = null, string? confirmText = null, string? helpText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, string? restorationId = null, TimePickerEntryMode initialEntryMode = TimePickerEntryMode.dial, Orientation? orientation = null, Action<TimePickerEntryMode>? onEntryModeChanged = null, Icon? switchToInputEntryModeIcon = null, Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = false) : base(key: key)
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

internal class _TimePickerDialogState__time_picker : State<TimePickerDialog>, RestorationMixin<TimePickerDialog>
{
    private bool __late__entryMode_initialized;
    private RestorableEnum<TimePickerEntryMode> __late__entryMode = default!;
    internal virtual RestorableEnum<TimePickerEntryMode> _entryMode
    {
        get
        {
            if (!__late__entryMode_initialized)
            {
                __late__entryMode = new RestorableEnum<TimePickerEntryMode>(widget.initialEntryMode, values: Enum.GetValues<TimePickerEntryMode>().ToList().Cast<TimePickerEntryMode>());
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
                __late__selectedTime = new RestorableTimeOfDay(widget.initialTime);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    internal virtual GlobalKey<FormState> _formKey { get; private set; } = GlobalKey<FormState>.Create();
    internal virtual RestorableEnum<AutovalidateMode> _autovalidateMode { get; private set; } = new RestorableEnum<AutovalidateMode>(AutovalidateMode.disabled, values: Enum.GetValues<AutovalidateMode>().ToList().Cast<AutovalidateMode>());
    private bool __late__orientation_initialized;
    private RestorableEnumN<Orientation> __late__orientation = default!;
    internal virtual RestorableEnumN<Orientation> _orientation
    {
        get
        {
            if (!__late__orientation_initialized)
            {
                __late__orientation = new RestorableEnumN<Orientation>(widget.orientation, values: Enum.GetValues<Orientation>().ToList().Cast<Orientation>());
                __late__orientation_initialized = true;
            }
            return __late__orientation;
        }
    }
    internal static Size _kTimePickerPortraitSize = new Size(310, 468);
    internal static Size _kTimePickerLandscapeSize = new Size(524, 342);
    internal static Size _kTimePickerInputSize = new Size(312, 252);
    internal const double _kTimePickerInputMinimumHeight = 216;
    internal static Size _kTimePickerMinPortraitSize = new Size(238, 326);
    internal static Size _kTimePickerMinLandscapeSize = new Size(416, 248);
    internal static Size _kTimePickerMinInputSize = new Size(312, 196);
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } = new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public override void dispose()
    {
        _selectedTime.dispose();
        _entryMode.dispose();
        _autovalidateMode.dispose();
        _orientation.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual string? restorationId => widget.restorationId;
    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_selectedTime, "selected_time");
        registerForRestoration(_entryMode, "entry_mode");
        registerForRestoration(_autovalidateMode, "autovalidate_mode");
        registerForRestoration(_orientation, "orientation");
    }

    internal virtual void _handleTimeChanged(TimeOfDay value)
    {
        if (!Equals(value, _selectedTime.value))
        {
            setState(() =>
            {
                _selectedTime.value = value;
            });
        }
    }

    internal virtual void _handleEntryModeChanged(TimePickerEntryMode value)
    {
        if (!Equals(value, _entryMode.value))
        {
            setState(() =>
            {
                switch (_entryMode.value)
                {
                    case TimePickerEntryMode.dial:
                        {
                            _autovalidateMode.value = AutovalidateMode.disabled;
                            break;
                        }
                    case TimePickerEntryMode.input:
                        {
                            _formKey.currentState!.save();
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
                _entryMode.value = value;
                widget.onEntryModeChanged?.Invoke(value);
            });
        }
    }

    internal virtual void _toggleEntryMode()
    {
        switch (_entryMode.value)
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
                    FlutterError.Create($"Can not change entry mode from {_entryMode}");
                    break;
                }
        }
    }

    internal virtual void _handleCancel()
    {
        Navigator.pop<object>(context);
    }

    internal virtual void _handleOk()
    {
        if (Equals(_entryMode.value, TimePickerEntryMode.input) || Equals(_entryMode.value, TimePickerEntryMode.inputOnly))
        {
            FormState form = _formKey.currentState!;
            if (!form.validate())
            {
                setState(() =>
                {
                    _autovalidateMode.value = AutovalidateMode.always;
                });
                return;
            }
            form.save();
        }
        Navigator.pop<object>(context, _selectedTime.value);
    }

    internal virtual Size _minDialogSize(BuildContext context)
    {
        Orientation orientation = _orientation.value ?? MediaQuery.orientationOf(context);
        switch (_entryMode.value)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    return orientation switch { Orientation.portrait => _kTimePickerMinPortraitSize, Orientation.landscape => _kTimePickerMinLandscapeSize, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
                                _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
                                timePickerWidth = _kTimePickerMinInputSize.width - defaultTheme.dayPeriodPortraitSize.width - 12L;
                                break;
                            }
                        case TimeOfDayFormat.a_space_h_colon_mm:
                        case TimeOfDayFormat.h_colon_mm_space_a:
                            {
                                timePickerWidth = _kTimePickerMinInputSize.width - 32L;
                                break;
                            }
                    }
                    return new Size(timePickerWidth, _kTimePickerMinInputSize.height);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _dialogSize(BuildContext context)
    {
        Orientation orientation = _orientation.value ?? MediaQuery.orientationOf(context);
        var fontSizeToScale = 14.0;
        double textScaleFactor = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 1.1).scale(fontSizeToScale) / fontSizeToScale;
        Size timePickerSize = default!;
        switch (_entryMode.value)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    switch (orientation)
                    {
                        case Orientation.portrait:
                            {
                                timePickerSize = _kTimePickerPortraitSize;
                                break;
                            }
                        case Orientation.landscape:
                            {
                                timePickerSize = new Size(_kTimePickerLandscapeSize.width * textScaleFactor, _kTimePickerLandscapeSize.height);
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
                                _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
                                timePickerWidth = _kTimePickerInputSize.width - defaultTheme.dayPeriodPortraitSize.width - 12L;
                                break;
                            }
                        case TimeOfDayFormat.a_space_h_colon_mm:
                        case TimeOfDayFormat.h_colon_mm_space_a:
                            {
                                timePickerWidth = _kTimePickerInputSize.width - 32L;
                                break;
                            }
                    }
                    timePickerSize = new Size(timePickerWidth, _kTimePickerInputSize.height);
                    break;
                }
        }
        return new Size(timePickerSize.width, timePickerSize.height * textScaleFactor);
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        ThemeData theme = Theme.of(context);
        TimePickerThemeData pickerTheme = TimePickerTheme.of(context);
        _TimePickerDefaults__time_picker defaultTheme = new _TimePickerDefaultsM3__time_picker(context);
        ShapeBorder shapeLocal = pickerTheme.shape ?? defaultTheme.shape;
        Color entryModeIconColorLocal = pickerTheme.entryModeIconColor ?? defaultTheme.entryModeIconColor;
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Widget actions = new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 0), child: new Row(children: ((Func<List<Widget>>)(() => { var __collection92775 = new List<Widget>(); if (Equals(_entryMode.value, TimePickerEntryMode.dial) || Equals(_entryMode.value, TimePickerEntryMode.input)) { __collection92775.Add(DartRuntimePrimitives.ConvertValue<Widget>(new IconButton(color: null, style: IconButton.styleFrom(foregroundColor: entryModeIconColorLocal), onPressed: _toggleEntryMode, icon: Equals(_entryMode.value, TimePickerEntryMode.dial) ? (widget.switchToInputEntryModeIcon ?? new Icon(Icons.keyboard_outlined)) : (widget.switchToTimerEntryModeIcon ?? new Icon(Icons.access_time)), tooltip: Equals(_entryMode.value, TimePickerEntryMode.dial) ? MaterialLocalizations.of(context).inputTimeModeButtonLabel : MaterialLocalizations.of(context).dialModeButtonLabel))); } __collection92775.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new ConstrainedBox(constraints: new BoxConstraints(minHeight: 36), child: new Align(alignment: AlignmentDirectional.centerEnd, child: new OverflowBar(spacing: 8, overflowAlignment: OverflowBarAlignment.end, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(style: pickerTheme.cancelButtonStyle ?? defaultTheme.cancelButtonStyle, onPressed: () => _handleCancel(), child: new Text(widget.cancelText ?? localizations.cancelButtonLabel))), DartRuntimePrimitives.ConvertValue<Widget>(new TextButton(style: pickerTheme.confirmButtonStyle ?? defaultTheme.confirmButtonStyle, onPressed: () => _handleOk(), child: new Text(widget.confirmText ?? localizations.okButtonLabel))) })))))); return __collection92775; }))()));
        Offset tapTargetSizeOffset = theme.materialTapTargetSize switch { var __constant95285 when Equals(__constant95285, MaterialTapTargetSize.padded) => Offset.zero, var __constant95381 when Equals(__constant95381, MaterialTapTargetSize.shrinkWrap) => new Offset(0, -12), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        Size dialogSize = _dialogSize(context) + tapTargetSizeOffset;
        Size minDialogSize = _minDialogSize(context) + tapTargetSizeOffset;
        return new Dialog(shape: shapeLocal, elevation: pickerTheme.elevation ?? defaultTheme.elevation, backgroundColor: pickerTheme.backgroundColor ?? defaultTheme.backgroundColor, insetPadding: EdgeInsets.CreateSymmetric(horizontal: 16, vertical: (Equals(_entryMode.value, TimePickerEntryMode.input) || Equals(_entryMode.value, TimePickerEntryMode.inputOnly)) ? 0 : 24), child: new Padding(padding: pickerTheme.padding ?? defaultTheme.padding, child: new LayoutBuilder(builder: (context, constraints) =>
        {
            Size constrainedSize = constraints.constrain(dialogSize);
            var allowedSize = new Size((constrainedSize.width < minDialogSize.width) ? minDialogSize.width : constrainedSize.width, (constrainedSize.height < minDialogSize.height) ? minDialogSize.height : constrainedSize.height);
            return new SingleChildScrollView(restorationId: "time_picker_scroll_view_horizontal", scrollDirection: Axis.horizontal, child: new SingleChildScrollView(restorationId: "time_picker_scroll_view_vertical", child: new AnimatedContainer(width: allowedSize.width, duration: Time_pickerLibrary._kDialogSizeAnimationDuration, curve: Curves.easeIn, constraints: new BoxConstraints(minHeight: _kTimePickerInputMinimumHeight, maxHeight: allowedSize.height), child: new Column(mainAxisSize: MainAxisSize.min, mainAxisAlignment: MainAxisAlignment.spaceBetween, crossAxisAlignment: CrossAxisAlignment.start, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Builder(builder: (context) => {
Widget childLocal = new Form(key: _formKey, autovalidateMode: _autovalidateMode.value, child: new _TimePicker__time_picker(time: widget.initialTime, onTimeChanged: _handleTimeChanged, helpText: widget.helpText, cancelText: widget.cancelText, confirmText: widget.confirmText, errorInvalidText: widget.errorInvalidText, hourLabelText: widget.hourLabelText, minuteLabelText: widget.minuteLabelText, restorationId: "time_picker", entryMode: DartRuntimePrimitives.RequireValue(_entryMode.value), orientation: widget.orientation, onEntryModeChanged: _handleEntryModeChanged, switchToInputEntryModeIcon: widget.switchToInputEntryModeIcon, switchToTimerEntryModeIcon: widget.switchToTimerEntryModeIcon, emptyInitialInput: widget.emptyInitialInput));
if ((!Equals(_entryMode.value, TimePickerEntryMode.input)) && (!Equals(_entryMode.value, TimePickerEntryMode.inputOnly)))
{
    return new Flexible(child: childLocal);
}
return childLocal;
throw new InvalidOperationException("Dart closure completed without a value.");
})), DartRuntimePrimitives.ConvertValue<Widget>(actions) }))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
    }

    public virtual RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
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
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(((Func<List<DiagnosticsNode>>)(() => { var __collection41817 = new List<DiagnosticsNode>(); __collection41817.Add(new ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<IRestorableProperty, DiagnosticsNode>((property) => new ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public class _TimePicker__time_picker : StatefulWidget
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
    public virtual Action<TimeOfDay>? onTimeChanged { get; private set; }
    public virtual Orientation? orientation { get; private set; }
    public virtual Action<TimePickerEntryMode>? onEntryModeChanged { get; private set; }
    public virtual Icon? switchToInputEntryModeIcon { get; private set; }
    public virtual Icon? switchToTimerEntryModeIcon { get; private set; }
    public virtual bool emptyInitialInput { get; private set; } = default!;

    internal _TimePicker__time_picker(TimeOfDay time, Action<TimeOfDay>? onTimeChanged, string? helpText = null, string? cancelText = null, string? confirmText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, string? restorationId = null, TimePickerEntryMode entryMode = TimePickerEntryMode.dial, Orientation? orientation = null, Action<TimePickerEntryMode>? onEntryModeChanged = null, Icon? switchToInputEntryModeIcon = null, Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = default!)
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

internal class _TimePickerState__time_picker : State<_TimePicker__time_picker>, RestorationMixin<_TimePicker__time_picker>
{
    internal virtual Timer? _vibrateTimer { get; set; } = default;
    public virtual MaterialLocalizations localizations { get; set; } = default!;
    internal virtual RestorableEnum<_HourMinuteMode__time_picker> _hourMinuteMode { get; private set; } = new RestorableEnum<_HourMinuteMode__time_picker>(_HourMinuteMode__time_picker.hour, values: Enum.GetValues<_HourMinuteMode__time_picker>().ToList().Cast<_HourMinuteMode__time_picker>());
    internal virtual RestorableEnumN<_HourMinuteMode__time_picker> _lastModeAnnounced { get; private set; } = new RestorableEnumN<_HourMinuteMode__time_picker>(null, values: Enum.GetValues<_HourMinuteMode__time_picker>().ToList().Cast<_HourMinuteMode__time_picker>());
    internal virtual RestorableBoolN _autofocusHour { get; private set; } = new RestorableBoolN(null);
    internal virtual RestorableBoolN _autofocusMinute { get; private set; } = new RestorableBoolN(null);
    private bool __late__orientation_initialized;
    private RestorableEnumN<Orientation> __late__orientation = default!;
    internal virtual RestorableEnumN<Orientation> _orientation
    {
        get
        {
            if (!__late__orientation_initialized)
            {
                __late__orientation = new RestorableEnumN<Orientation>(widget.orientation, values: Enum.GetValues<Orientation>().ToList().Cast<Orientation>());
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
                __late__selectedTime = new RestorableTimeOfDay(widget.time);
                __late__selectedTime_initialized = true;
            }
            return __late__selectedTime;
        }
    }
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } = new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual RestorableTimeOfDay selectedTime => _selectedTime;
    public override void dispose()
    {
        _vibrateTimer?.cancel();
        _vibrateTimer = null;
        _orientation.dispose();
        _selectedTime.dispose();
        _hourMinuteMode.dispose();
        _lastModeAnnounced.dispose();
        _autofocusHour.dispose();
        _autofocusMinute.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
        localizations = MaterialLocalizations.of(context);
    }

    public override void didUpdateWidget(_TimePicker__time_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (!Equals(oldWidget.orientation, widget.orientation))
        {
            _orientation.value = widget.orientation;
        }
        if (!Equals(oldWidget.time, widget.time))
        {
            _selectedTime.value = widget.time;
        }
    }

    internal virtual void _setEntryMode(TimePickerEntryMode mode)
    {
        widget.onEntryModeChanged?.Invoke(mode);
    }

    public virtual string? restorationId => widget.restorationId;
    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_hourMinuteMode, "hour_minute_mode");
        registerForRestoration(_lastModeAnnounced, "last_mode_announced");
        registerForRestoration(_autofocusHour, "autofocus_hour");
        registerForRestoration(_autofocusMinute, "autofocus_minute");
        registerForRestoration(_selectedTime, "selected_time");
        registerForRestoration(_orientation, "orientation");
    }

    internal virtual void _vibrate()
    {
        switch (Theme.of(context).platform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
                {
                    _vibrateTimer?.cancel();
                    _vibrateTimer = new Timer(Time_pickerLibrary._kVibrateCommitDelay, () =>
                    {
                        DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                        _vibrateTimer = null;
                    });
                    break;
                }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    break;
                }
        }
    }

    internal virtual void _handleHourMinuteModeChanged(_HourMinuteMode__time_picker mode)
    {
        _vibrate();
        setState(() =>
        {
            _hourMinuteMode.value = mode;
        });
    }

    internal virtual void _handleEntryModeToggle()
    {
        setState(() =>
        {
            TimePickerEntryMode newMode = widget.entryMode;
            switch (widget.entryMode)
            {
                case TimePickerEntryMode.dial:
                    {
                        newMode = TimePickerEntryMode.input;
                        break;
                    }
                case TimePickerEntryMode.input:
                    {
                        _autofocusHour.value = false;
                        _autofocusMinute.value = false;
                        newMode = TimePickerEntryMode.dial;
                        break;
                    }
                case TimePickerEntryMode.dialOnly:
                case TimePickerEntryMode.inputOnly:
                    {
                        FlutterError.Create($"Can not change entry mode from {widget.entryMode}");
                        break;
                    }
            }
            _setEntryMode(newMode);
        });
    }

    internal virtual void _handleTimeChanged(TimeOfDay value)
    {
        _vibrate();
        setState(() =>
        {
            _selectedTime.value = value;
            widget.onTimeChanged?.Invoke(value);
        });
    }

    internal virtual void _handleHourDoubleTapped()
    {
        _autofocusHour.value = true;
        _handleEntryModeToggle();
    }

    internal virtual void _handleMinuteDoubleTapped()
    {
        _autofocusMinute.value = true;
        _handleEntryModeToggle();
    }

    internal virtual void _handleHourSelected()
    {
        setState(() =>
        {
            _hourMinuteMode.value = _HourMinuteMode__time_picker.minute;
        });
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        TimeOfDayFormat timeOfDayFormatLocal = localizations.timeOfDayFormat(alwaysUse24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context));
        ThemeData themeLocal = Theme.of(context);
        _TimePickerDefaults__time_picker defaultThemeLocal = new _TimePickerDefaultsM3__time_picker(context, entryMode: widget.entryMode);
        Orientation orientationLocal = _orientation.value ?? MediaQuery.orientationOf(context);
        HourFormat timeOfDayHour = TimeLibrary.hourFormat(of: timeOfDayFormatLocal);
        _HourDialType__time_picker hourMode = timeOfDayHour switch { HourFormat.HH or HourFormat.H => _HourDialType__time_picker.twentyFourHourDoubleRing, HourFormat.h => _HourDialType__time_picker.twelveHour, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        string helpTextLocal = default!;
        Widget picker = default!;
        switch (widget.entryMode)
        {
            case TimePickerEntryMode.dial:
            case TimePickerEntryMode.dialOnly:
                {
                    helpTextLocal = widget.helpText ?? localizations.timePickerDialHelpText;
                    double portraitMinInteractiveVerticalAdjustment = Math.Max(0, (2L * Widgets.ConstantsLibrary.kMinInteractiveDimension) - defaultThemeLocal.dayPeriodPortraitSize.height);
                    EdgeInsetsGeometry dialPadding = orientationLocal switch { Orientation.portrait => DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.CreateOnly(left: 12, right: 12, top: 36L - (portraitMinInteractiveVerticalAdjustment / 2L))), Orientation.landscape => DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsetsDirectional.CreateOnly(start: 64)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                    Widget dialLocal = new Padding(padding: dialPadding, child: new ExcludeSemantics(child: SizedBox.CreateFromSize(size: defaultThemeLocal.dialSize, child: new AspectRatio(aspectRatio: 1, child: new _Dial__time_picker(hourMinuteMode: DartRuntimePrimitives.RequireValue(_hourMinuteMode.value), hourDialType: hourMode, selectedTime: _selectedTime.value, onChanged: _handleTimeChanged, onHourSelected: () => _handleHourSelected())))));
                    switch (orientationLocal)
                    {
                        case Orientation.portrait:
                            {
                                picker = DartRuntimePrimitives.ConvertValue<Widget>(new Column(mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 0), child: new _DialTimePickerHeader__time_picker(helpText: helpTextLocal))), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Column(mainAxisSize: MainAxisSize.min, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 0), child: dialLocal))) }))) }));
                                break;
                            }
                        case Orientation.landscape:
                            {
                                picker = DartRuntimePrimitives.ConvertValue<Widget>(new Column(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 0), child: new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new _DialTimePickerHeader__time_picker(helpText: helpTextLocal)), DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: dialLocal)) })))) }));
                                break;
                            }
                    }
                    break;
                }
            case TimePickerEntryMode.input:
            case TimePickerEntryMode.inputOnly:
                {
                    string helpTextAlternate = widget.helpText ?? localizations.timePickerInputHelpText;
                    picker = DartRuntimePrimitives.ConvertValue<Widget>(new Column(mainAxisSize: MainAxisSize.min, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new _TimePickerInput__time_picker(initialSelectedTime: _selectedTime.value, errorInvalidText: widget.errorInvalidText, hourLabelText: widget.hourLabelText, minuteLabelText: widget.minuteLabelText, helpText: helpTextAlternate, autofocusHour: _autofocusHour.value, autofocusMinute: _autofocusMinute.value, restorationId: "time_picker_input", emptyInitialTime: widget.emptyInitialInput)) }));
                    break;
                }
        }
        return new _TimePickerModel__time_picker(entryMode: widget.entryMode, selectedTime: _selectedTime.value, hourMinuteMode: DartRuntimePrimitives.RequireValue(_hourMinuteMode.value), orientation: orientationLocal, onHourMinuteModeChanged: _handleHourMinuteModeChanged, onHourDoubleTapped: () => _handleHourDoubleTapped(), onMinuteDoubleTapped: () => _handleMinuteDoubleTapped(), hourDialType: hourMode, onSelectedTimeChanged: _handleTimeChanged, use24HourFormat: MediaQuery.alwaysUse24HourFormatOf(context), theme: TimePickerTheme.of(context), defaultTheme: defaultThemeLocal, child: picker);
    }

    public virtual RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(((Func<List<DiagnosticsNode>>)(() => { var __collection41817 = new List<DiagnosticsNode>(); __collection41817.Add(new ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\".")); __collection41817.Add(new ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:")); __collection41817.AddRange(_debugPropertiesWaitingForReregistration!.map<IRestorableProperty, DiagnosticsNode>((property) => new ErrorDescription($" * {property._restorationId}"))); return __collection41817; }))()));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
            });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((Action<IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public static partial class Time_pickerLibrary
{
    public static async Future<TimeOfDay?> showTimePicker(BuildContext context, TimeOfDay initialTime, Func<BuildContext, Widget?, Widget>? builder = null, bool barrierDismissible = true, Color? barrierColor = null, string? barrierLabel = null, bool useRootNavigator = true, TimePickerEntryMode initialEntryMode = TimePickerEntryMode.dial, string? cancelText = null, string? confirmText = null, string? helpText = null, string? errorInvalidText = null, string? hourLabelText = null, string? minuteLabelText = null, RouteSettings? routeSettings = null, Action<TimePickerEntryMode>? onEntryModeChanged = null, Offset? anchorPoint = null, Orientation? orientation = null, Icon? switchToInputEntryModeIcon = null, Icon? switchToTimerEntryModeIcon = null, bool emptyInitialInput = false)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        Widget dialog = new TimePickerDialog(initialTime: initialTime, initialEntryMode: initialEntryMode, cancelText: cancelText, confirmText: confirmText, helpText: helpText, errorInvalidText: errorInvalidText, hourLabelText: hourLabelText, minuteLabelText: minuteLabelText, orientation: orientation, onEntryModeChanged: onEntryModeChanged, switchToInputEntryModeIcon: switchToInputEntryModeIcon, switchToTimerEntryModeIcon: switchToTimerEntryModeIcon, emptyInitialInput: emptyInitialInput);
        return await DialogLibrary.showDialog<TimeOfDay>(context: context, barrierDismissible: barrierDismissible, barrierColor: barrierColor, barrierLabel: barrierLabel, useRootNavigator: useRootNavigator, builder: (context) =>
        {
            return (builder is null) ? dialog : builder(context, dialog);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, routeSettings: routeSettings, anchorPoint: anchorPoint);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal abstract class _TimePickerDefaults__time_picker : TimePickerThemeData
{
    public abstract override Color backgroundColor { get; }
    public abstract override ButtonStyle cancelButtonStyle { get; }
    public abstract override ButtonStyle confirmButtonStyle { get; }
    public abstract override BorderSide dayPeriodBorderSide { get; }
    public abstract override Color dayPeriodColor { get; }
    public abstract override OutlinedBorder dayPeriodShape { get; }
    public abstract Size dayPeriodInputSize { get; }
    public abstract Size dayPeriodLandscapeSize { get; }
    public abstract Size dayPeriodPortraitSize { get; }
    public abstract override Color dayPeriodTextColor { get; }
    public abstract override TextStyle dayPeriodTextStyle { get; }
    public abstract override Color dialBackgroundColor { get; }
    public abstract override Color dialHandColor { get; }
    public abstract Size dialSize { get; }
    public abstract double handWidth { get; }
    public abstract double dotRadius { get; }
    public abstract double centerRadius { get; }
    public abstract override Color dialTextColor { get; }
    public abstract override TextStyle dialTextStyle { get; }
    public abstract override double? elevation { get; }
    public abstract override Color entryModeIconColor { get; }
    public abstract override TextStyle helpTextStyle { get; }
    public abstract override Color hourMinuteColor { get; }
    public abstract override ShapeBorder hourMinuteShape { get; }
    public abstract Size hourMinuteSize { get; }
    public abstract Size hourMinuteSize24Hour { get; }
    public abstract Size hourMinuteInputSize { get; }
    public abstract Size hourMinuteInputSize24Hour { get; }
    public abstract override Color hourMinuteTextColor { get; }
    public abstract override TextStyle hourMinuteTextStyle { get; }
    public abstract override InputDecorationThemeData inputDecorationTheme { get; }
    public abstract override EdgeInsetsGeometry padding { get; }
    public abstract override ShapeBorder shape { get; }
}

public static partial class Time_pickerLibrary
{
    internal static bool _debugDialTimePickerEntryMode(BuildContext context)
    {
        TimePickerEntryMode entryMode = _TimePickerModel__time_picker.entryModeOf(context);
        return Equals(entryMode, TimePickerEntryMode.dial) || Equals(entryMode, TimePickerEntryMode.dialOnly);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TimePickerDefaultsM3__time_picker : _TimePickerDefaults__time_picker
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual TimePickerEntryMode entryMode { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
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
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _TimePickerDefaultsM3__time_picker(BuildContext context, TimePickerEntryMode entryMode = TimePickerEntryMode.dial)
    {
        this.context = context;
        this.entryMode = entryMode;
    }

    public override Color backgroundColor
    {
        get
        {
            return _colors.surfaceContainerHigh;
        }
    }
    public override ButtonStyle cancelButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
        }
    }
    public override ButtonStyle confirmButtonStyle
    {
        get
        {
            return TextButton.styleFrom();
        }
    }
    public override BorderSide dayPeriodBorderSide
    {
        get
        {
            return new BorderSide(color: _colors.outline);
        }
    }
    public override Color dayPeriodColor
    {
        get
        {
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.tertiaryContainer;
                }
                return Colors.transparent;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override OutlinedBorder dayPeriodShape
    {
        get
        {
            return new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))).copyWith(side: dayPeriodBorderSide);
        }
    }
    public override Size dayPeriodPortraitSize
    {
        get
        {
            return new Size(52, 80);
        }
    }
    public override Size dayPeriodLandscapeSize
    {
        get
        {
            return new Size(216, 38);
        }
    }
    public override Size dayPeriodInputSize
    {
        get
        {
            return new Size(dayPeriodPortraitSize.width, dayPeriodPortraitSize.height - 8L);
        }
    }
    public override Color dayPeriodTextColor
    {
        get
        {
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onTertiaryContainer;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onTertiaryContainer;
                    }
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onTertiaryContainer;
                    }
                    return _colors.onTertiaryContainer;
                }
                if (states.Contains(WidgetState.focused))
                {
                    return _colors.onSurfaceVariant;
                }
                if (states.Contains(WidgetState.hovered))
                {
                    return _colors.onSurfaceVariant;
                }
                if (states.Contains(WidgetState.pressed))
                {
                    return _colors.onSurfaceVariant;
                }
                return _colors.onSurfaceVariant;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override TextStyle dayPeriodTextStyle
    {
        get
        {
            return _textTheme.titleMedium!.copyWith(color: dayPeriodTextColor);
        }
    }
    public override Color dialBackgroundColor
    {
        get
        {
            return _colors.surfaceContainerHighest;
        }
    }
    public override Color dialHandColor
    {
        get
        {
            return _colors.primary;
        }
    }
    public override Size dialSize
    {
        get
        {
            return new Size(256.0);
        }
    }
    public override double handWidth
    {
        get
        {
            return new Size(2, double.PositiveInfinity).width;
        }
    }
    public override double dotRadius
    {
        get
        {
            return new Size(48.0).width / 2L;
        }
    }
    public override double centerRadius
    {
        get
        {
            return new Size(8.0).width / 2L;
        }
    }
    public override Color dialTextColor
    {
        get
        {
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    return _colors.onPrimary;
                }
                return _colors.onSurface;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override TextStyle dialTextStyle
    {
        get
        {
            return _textTheme.bodyLarge!;
        }
    }
    public override double? elevation
    {
        get
        {
            return 6.0;
        }
    }
    public override Color entryModeIconColor
    {
        get
        {
            return _colors.onSurface;
        }
    }
    public override TextStyle helpTextStyle
    {
        get
        {
            return WidgetStateTextStyle.CreateResolveWith((states) =>
            {
                TextStyle textStyle = _textTheme.labelMedium!;
                return textStyle.copyWith(color: _colors.onSurfaceVariant);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override EdgeInsetsGeometry padding
    {
        get
        {
            return EdgeInsets.CreateAll(24);
        }
    }
    public override Color hourMinuteColor
    {
        get
        {
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    Color overlayColor = _colors.primaryContainer;
                    if (states.Contains(WidgetState.pressed))
                    {
                        overlayColor = _colors.onPrimaryContainer;
                    }
                    else
                    {
                        if (states.Contains(WidgetState.hovered))
                        {
                            var hoverOpacity = 0.08;
                            overlayColor = _colors.onPrimaryContainer.withOpacity(hoverOpacity);
                        }
                        else
                        {
                            if (states.Contains(WidgetState.focused))
                            {
                                var focusOpacity = 0.1;
                                overlayColor = _colors.onPrimaryContainer.withOpacity(focusOpacity);
                            }
                        }
                    }
                    return Dart_uiLibrary.Color.alphaBlend(overlayColor, _colors.primaryContainer);
                }
                else
                {
                    Color overlayColorLocal = _colors.surfaceContainerHighest;
                    if (states.Contains(WidgetState.pressed))
                    {
                        overlayColorLocal = _colors.onSurface;
                    }
                    else
                    {
                        if (states.Contains(WidgetState.hovered))
                        {
                            var hoverOpacityLocal = 0.08;
                            overlayColorLocal = _colors.onSurface.withOpacity(hoverOpacityLocal);
                        }
                        else
                        {
                            if (states.Contains(WidgetState.focused))
                            {
                                var focusOpacityLocal = 0.1;
                                overlayColorLocal = _colors.onSurface.withOpacity(focusOpacityLocal);
                            }
                        }
                    }
                    return Dart_uiLibrary.Color.alphaBlend(overlayColorLocal, _colors.surfaceContainerHighest);
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override ShapeBorder hourMinuteShape
    {
        get
        {
            return new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(8.0)));
        }
    }
    public override Size hourMinuteSize
    {
        get
        {
            return new Size(96, 80);
        }
    }
    public override Size hourMinuteSize24Hour
    {
        get
        {
            return new Size(new Size(114, double.PositiveInfinity).width, hourMinuteSize.height);
        }
    }
    public override Size hourMinuteInputSize
    {
        get
        {
            return new Size(hourMinuteSize.width, hourMinuteSize.height - 8L);
        }
    }
    public override Size hourMinuteInputSize24Hour
    {
        get
        {
            return new Size(hourMinuteSize24Hour.width, hourMinuteSize24Hour.height - 8L);
        }
    }
    public override Color hourMinuteTextColor
    {
        get
        {
            return WidgetStateColor.CreateResolveWith((states) =>
            {
                return _hourMinuteTextColor.resolve(states);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    internal virtual WidgetStateProperty<Color> _hourMinuteTextColor
    {
        get
        {
            return WidgetStateProperty.resolveWith((states) =>
            {
                if (states.Contains(WidgetState.selected))
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onPrimaryContainer;
                    }
                    return _colors.onPrimaryContainer;
                }
                else
                {
                    if (states.Contains(WidgetState.pressed))
                    {
                        return _colors.onSurface;
                    }
                    if (states.Contains(WidgetState.hovered))
                    {
                        return _colors.onSurface;
                    }
                    if (states.Contains(WidgetState.focused))
                    {
                        return _colors.onSurface;
                    }
                    return _colors.onSurface;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override TextStyle hourMinuteTextStyle
    {
        get
        {
            return WidgetStateTextStyle.CreateResolveWith((states) =>
            {
                return entryMode switch { TimePickerEntryMode.dial => _textTheme.displayLarge!.copyWith(color: _hourMinuteTextColor.resolve(states)), TimePickerEntryMode.dialOnly => _textTheme.displayLarge!.copyWith(color: _hourMinuteTextColor.resolve(states)), TimePickerEntryMode.input => _textTheme.displayMedium!.copyWith(color: _hourMinuteTextColor.resolve(states)), TimePickerEntryMode.inputOnly => _textTheme.displayMedium!.copyWith(color: _hourMinuteTextColor.resolve(states)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
    }
    public override InputDecorationThemeData inputDecorationTheme
    {
        get
        {
            BorderRadius selectorRadius = new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))).borderRadius.resolve(Directionality.of(context));
            return new InputDecorationThemeData(contentPadding: EdgeInsets.zero, filled: true, fillColor: hourMinuteColor, focusColor: _colors.primaryContainer, enabledBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new BorderSide(color: Colors.transparent)), errorBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new BorderSide(color: _colors.error, width: 2)), focusedBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new BorderSide(color: _colors.primary, width: 2)), focusedErrorBorder: new OutlineInputBorder(borderRadius: selectorRadius, borderSide: new BorderSide(color: _colors.error, width: 2)), hintStyle: hourMinuteTextStyle.copyWith(color: _colors.onSurface.withOpacity(0.36)), errorStyle: new TextStyle(fontSize: 0));
        }
    }
    public override ShapeBorder shape
    {
        get
        {
            return new RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(28.0)));
        }
    }
    public override WidgetStateProperty<Color?>? timeSelectorSeparatorColor
    {
        get
        {
            return (WidgetStateProperty<Color?>?)new WidgetStatePropertyAll<Color>(_colors.onSurface);
        }
    }
    public override WidgetStateProperty<TextStyle?>? timeSelectorSeparatorTextStyle
    {
        get
        {
            return (WidgetStateProperty<TextStyle?>?)new WidgetStatePropertyAll<TextStyle?>(_textTheme.displayLarge);
        }
    }
}
