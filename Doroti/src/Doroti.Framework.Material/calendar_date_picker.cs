// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/calendar_date_picker.dart
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

public static partial class Calendar_date_pickerLibrary
{
    internal static Duration _monthScrollDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _dayPickerRowHeightM2 = 42.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _dayPickerRowHeightM3 = 48.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static long _maxDayPickerRowCount = 6L;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _maxDayPickerHeightM2 = (Calendar_date_pickerLibrary._dayPickerRowHeightM2 * ((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)));
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _maxDayPickerHeightM3 = (Calendar_date_pickerLibrary._dayPickerRowHeightM3 * ((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)));
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _monthPickerHorizontalPaddingPortraitM3 = 12.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _monthPickerHorizontalPaddingOther = 8.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static long _yearPickerColumnCount = 3L;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _yearPickerPadding = 16.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _yearPickerRowHeight = 52.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _yearPickerRowSpacing = 8.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _subHeaderHeight = 52.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _monthNavButtonsWidth = 108.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _kMaxTextScaleFactor = 3.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _kModeToggleButtonMaxScaleFactor = 2.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _kDayPickerGridPortraitMaxScaleFactor = 2.0;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _kDayPickerGridLandscapeMaxScaleFactor = 1.5;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _fontSizeToScale = 14.0;
}

public class CalendarDatePicker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onDateChanged { get; private set; } = default!;
    public virtual global::System.Action<DateTime>? onDisplayedMonthChanged { get; private set; }
    public virtual DatePickerMode initialCalendarMode { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public CalendarDatePicker(global::Doroti.Framework.Foundation.Key? key = null, DateTime? initialDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, global::System.Action<DateTime> onDateChanged = default!, global::System.Action<DateTime>? onDisplayedMonthChanged = null, DatePickerMode initialCalendarMode = DatePickerMode.day, global::System.Func<DateTime, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
        this.onDateChanged = onDateChanged;
        this.onDisplayedMonthChanged = onDisplayedMonthChanged;
        this.initialCalendarMode = initialCalendarMode;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = __calendarDelegate;
        this.initialDate = ((initialDate is null) ? null : calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate))));
        this.firstDate = calendarDelegate.dateOnly(firstDate);
        this.lastDate = calendarDelegate.dateOnly(lastDate);
        this.currentDate = calendarDelegate.dateOnly(((currentDate ?? (DateTime)calendarDelegate.now())));
        DartRuntimePrimitives.Assert(() => !this.lastDate.isBefore(this.firstDate), () => (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((this.initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate)), () => (object?)$"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => ((this.initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate)), () => (object?)$"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}.");
        DartRuntimePrimitives.Assert(() => (((this.selectableDayPredicate is null) || (this.initialDate is null)) || this.selectableDayPredicate!(DartRuntimePrimitives.RequireValue(this.initialDate))), () => (object?)$"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CalendarDatePickerState__calendar_date_picker());
}

internal class _CalendarDatePickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<CalendarDatePicker>
{
    internal virtual bool _announcedInitialDate { get; set; } = false;
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DatePickerMode _mode { get; set; } = default!;
    internal virtual DateTime _currentDisplayedMonthDate { get; set; } = default!;
    internal virtual DateTime? _selectedDate { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _monthPickerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _yearPickerKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _mode = ((CalendarDatePicker)this.widget).initialCalendarMode;
        DateTime currentDisplayedDate = (((CalendarDatePicker)this.widget).initialDate ?? ((CalendarDatePicker)this.widget).currentDate);
        _currentDisplayedMonthDate = ((CalendarDatePicker)this.widget).calendarDelegate.getMonth(currentDisplayedDate.Year, currentDisplayedDate.Month);
        if ((((CalendarDatePicker)this.widget).initialDate is not null))
        {
            _selectedDate = ((CalendarDatePicker)this.widget).initialDate;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(this.context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(this.context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(this.context));
        _localizations = MaterialLocalizations.of(this.context);
        _textDirection = Directionality.of(this.context);
        if ((!this._announcedInitialDate && (((CalendarDatePicker)this.widget).initialDate is not null)))
        {
            DartRuntimePrimitives.Assert(() => (this._selectedDate is not null));
            _announcedInitialDate = true;
            bool isToday = ((CalendarDatePicker)this.widget).calendarDelegate.isSameDay(((CalendarDatePicker)this.widget).currentDate, this._selectedDate);
            var semanticLabelSuffix = (isToday ? $", {((MaterialLocalizations)this._localizations).currentDateLabel}" : "");
            _announce($"{this._localizations.formatFullDate(DartRuntimePrimitives.RequireValue(this._selectedDate))}{semanticLabelSuffix}");
        }
    }

    internal virtual void _announce(string message)
    {
        if ((MediaQuery.maybeSupportsAnnounceOf(this.context) ?? false))
        {
            DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), message, Directionality.of(this.context)).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
        }
        else
        {
            _announcementText = message;
        }
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
                    DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                    break;
                }
            case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
        }
    }

    internal virtual void _handleModeChanged(DatePickerMode mode)
    {
        _vibrate();
        setState(((global::System.Action)(() =>
        {
            _mode = mode;
            if (this._selectedDate is DateTime selected)
            {
                string message = (mode switch { DatePickerMode.day => ((CalendarDatePicker)this.widget).calendarDelegate.formatMonthYear(selected, this._localizations), DatePickerMode.year => ((CalendarDatePicker)this.widget).calendarDelegate.formatYear(selected.Year, this._localizations), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                _announce(message);
            }
        })));
    }

    internal virtual void _handleMonthChanged(DateTime date)
    {
        setState(((global::System.Action)(() =>
        {
            if (((this._currentDisplayedMonthDate.Year != date.Year) || (this._currentDisplayedMonthDate.Month != date.Month)))
            {
                _currentDisplayedMonthDate = ((CalendarDatePicker)this.widget).calendarDelegate.getMonth(date.Year, date.Month);
                ((CalendarDatePicker)this.widget).onDisplayedMonthChanged?.Invoke(this._currentDisplayedMonthDate);
            }
        })));
    }

    internal virtual void _handleYearChanged(DateTime value)
    {
        _vibrate();
        long daysInMonth = ((CalendarDatePicker)this.widget).calendarDelegate.getDaysInMonth(value.Year, value.Month);
        long preferredDay = Math.Min((this._selectedDate?.Day ?? 1L), daysInMonth);
        value = ((CalendarDatePicker)this.widget).calendarDelegate.getDay(value.Year, value.Month, preferredDay);
        if (value.isBefore(((CalendarDatePicker)this.widget).firstDate))
        {
            value = ((CalendarDatePicker)this.widget).firstDate;
        }
        else
        {
            if (value.isAfter(((CalendarDatePicker)this.widget).lastDate))
            {
                value = ((CalendarDatePicker)this.widget).lastDate;
            }
        }
        setState(((global::System.Action)(() =>
        {
            _mode = DatePickerMode.day;
            _handleMonthChanged(value);
            if (_isSelectable(value))
            {
                _selectedDate = value;
                this.widget.onDateChanged(DartRuntimePrimitives.RequireValue(this._selectedDate));
            }
        })));
    }

    internal virtual void _handleDayChanged(DateTime value)
    {
        _vibrate();
        setState(((global::System.Action)(() =>
        {
            _selectedDate = value;
            this.widget.onDateChanged(DartRuntimePrimitives.RequireValue(this._selectedDate));
            switch (Theme.of(this.context).platform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                    {
                        bool isToday = ((CalendarDatePicker)this.widget).calendarDelegate.isSameDay(((CalendarDatePicker)this.widget).currentDate, this._selectedDate);
                        var semanticLabelSuffix = (isToday ? $", {((MaterialLocalizations)this._localizations).currentDateLabel}" : "");
                        DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), $"{((MaterialLocalizations)this._localizations).selectedDateLabel} {((CalendarDatePicker)this.widget).calendarDelegate.formatFullDate(DartRuntimePrimitives.RequireValue(this._selectedDate), this._localizations)}{semanticLabelSuffix}", this._textDirection).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        break;
                    }
            }
        })));
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return ((((CalendarDatePicker)this.widget).selectableDayPredicate is null ? true : ((CalendarDatePicker)this.widget).selectableDayPredicate.Invoke(date)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildPicker()
    {
        switch (this._mode)
        {
            case DatePickerMode.day:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _MonthPicker__calendar_date_picker(key: this._monthPickerKey, calendarDelegate: ((CalendarDatePicker)this.widget).calendarDelegate, initialMonth: this._currentDisplayedMonthDate, currentDate: ((CalendarDatePicker)this.widget).currentDate, firstDate: ((CalendarDatePicker)this.widget).firstDate, lastDate: ((CalendarDatePicker)this.widget).lastDate, selectedDate: this._selectedDate, onChanged: (global::System.Action<DateTime>)this._handleDayChanged, onDisplayedMonthChanged: (global::System.Action<DateTime>)this._handleMonthChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((CalendarDatePicker)this.widget).selectableDayPredicate));
                }
            case DatePickerMode.year:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: Calendar_date_pickerLibrary._subHeaderHeight), child: new YearPicker(key: this._yearPickerKey, calendarDelegate: ((CalendarDatePicker)this.widget).calendarDelegate, currentDate: ((CalendarDatePicker)this.widget).currentDate, firstDate: ((CalendarDatePicker)this.widget).firstDate, lastDate: ((CalendarDatePicker)this.widget).lastDate, selectedDate: this._currentDisplayedMonthDate, onChanged: (global::System.Action<DateTime>)this._handleYearChanged)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        double maxDayPickerHeight = ((Theme.of(context).useMaterial3 && (object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait))) ? Calendar_date_pickerLibrary._maxDayPickerHeightM3 : Calendar_date_pickerLibrary._maxDayPickerHeightM2);
        double scaledMaxDayPickerHeight = ((textScaleFactor > 1.3) ? (maxDayPickerHeight + ((((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)) * ((((textScaleFactor - 1L)) * 8L))))) : maxDayPickerHeight);
        var picker = new global::Doroti.Framework.Widgets.SizedBox(height: (Calendar_date_pickerLibrary._subHeaderHeight + scaledMaxDayPickerHeight), child: _buildPicker());
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection15772 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((MediaQuery.maybeSupportsAnnounceOf(context) ?? false)) { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(picker)); } else { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: true, accessibilityFocusBlockType: global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.blockNode, label: this._announcementText, child: picker))); } __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withClampedTextScaling(maxScaleFactor: Calendar_date_pickerLibrary._kModeToggleButtonMaxScaleFactor, child: new _DatePickerModeToggleButton__calendar_date_picker(mode: this._mode, title: ((CalendarDatePicker)this.widget).calendarDelegate.formatMonthYear(this._currentDisplayedMonthDate, this._localizations), onTitlePressed: ((global::System.Action)(() => { _handleModeChanged((this._mode switch { DatePickerMode.day => DatePickerMode.year, DatePickerMode.year => DatePickerMode.day, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })); })))))); return __collection15772; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DatePickerModeToggleButton__calendar_date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DatePickerMode mode { get; private set; } = default!;
    public virtual string title { get; private set; } = default!;
    public virtual global::System.Action onTitlePressed { get; private set; } = default!;

    internal _DatePickerModeToggleButton__calendar_date_picker(DatePickerMode mode, string title, global::System.Action onTitlePressed)
    {
        this.mode = mode;
        this.title = title;
        this.onTitlePressed = onTitlePressed;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DatePickerModeToggleButtonState__calendar_date_picker());
}

public class _DatePickerModeToggleButtonState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_DatePickerModeToggleButton__calendar_date_picker>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_DatePickerModeToggleButton__calendar_date_picker>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Framework.Animation.AnimationController(value: ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode, DatePickerMode.year)) ? 0.5 : 0), upperBound: 0.5, duration: Duration.Create(milliseconds: 200L), vsync: this);
    }

    public override void didUpdateWidget(_DatePickerModeToggleButton__calendar_date_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)oldWidget).mode, ((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode)))
        {
            return;
        }
        if ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode, DatePickerMode.year)))
        {
            this._controller.forward();
        }
        else
        {
            this._controller.reverse();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Painting.TextStyle? buttonTextStyle = (datePickerTheme.toggleButtonTextStyle ?? defaultsLocal.toggleButtonTextStyle);
        global::Doroti.Ui.Color? subHeaderForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme.subHeaderForegroundColor ?? defaultsLocal.subHeaderForegroundColor));
        global::Doroti.Ui.Color? buttonTextColor = ((global::Doroti.Ui.Color?)(object?)((datePickerTheme.toggleButtonTextStyle?.color ?? datePickerTheme.subHeaderForegroundColor) ?? defaultsLocal.toggleButtonTextStyle?.color));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection19110 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).selectYearSemanticsLabel, button: true, container: true, child: new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new InkWell(onTap: ((_DatePickerModeToggleButton__calendar_date_picker)this.widget).onTitlePressed, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Text(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).title, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, style: buttonTextStyle?.apply(color: buttonTextColor)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.RotationTransition(turns: this._controller, child: new global::Doroti.Framework.Widgets.Icon(Icons.arrow_drop_down, color: subHeaderForegroundColorLocal))) })))))))); if ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode, DatePickerMode.day))) { __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: Calendar_date_pickerLibrary._monthNavButtonsWidth))); } return __collection19110; }))()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._controller.dispose();
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

internal class _MonthPicker__calendar_date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime initialMonth { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime? selectedDate { get; private set; }
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onDisplayedMonthChanged { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _MonthPicker__calendar_date_picker(global::Doroti.Framework.Foundation.Key? key = null, DateTime initialMonth = default!, DateTime currentDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, global::System.Action<DateTime> onDisplayedMonthChanged = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::System.Func<DateTime, bool>? selectableDayPredicate = null) : base(key: key)
    {
        this.initialMonth = initialMonth;
        this.currentDate = currentDate;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.selectedDate = selectedDate;
        this.onChanged = onChanged;
        this.onDisplayedMonthChanged = onDisplayedMonthChanged;
        this.calendarDelegate = calendarDelegate;
        this.selectableDayPredicate = selectableDayPredicate;
        System.Diagnostics.Debug.Assert(!firstDate.isAfter(lastDate));
        System.Diagnostics.Debug.Assert(((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate)));
        System.Diagnostics.Debug.Assert(((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MonthPickerState__calendar_date_picker());
}

internal class _MonthPickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_MonthPicker__calendar_date_picker>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _pageViewKey { get; private set; } = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DateTime _currentMonth { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.PageController _pageController { get; set; } = default!;
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent>? _shortcutMap { get; set; } = default;
    internal virtual DartMap<Type, dynamic>? _actionMap { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> _directionOffset = new DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> { [global::Doroti.Framework.Widgets.TraversalDirection.up] = -7L, [global::Doroti.Framework.Widgets.TraversalDirection.right] = 1L, [global::Doroti.Framework.Widgets.TraversalDirection.down] = 7L, [global::Doroti.Framework.Widgets.TraversalDirection.left] = -1L };

    public override void initState()
    {
        base.initState();
        _currentMonth = ((_MonthPicker__calendar_date_picker)this.widget).initialMonth;
        _pageController = new global::Doroti.Framework.Widgets.PageController(initialPage: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, this._currentMonth));
        _shortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.left)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.right)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Framework.Widgets.SingleActivator(global::Doroti.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Framework.Widgets.Intent)(object?)new global::Doroti.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Framework.Widgets.TraversalDirection.up)) }.cast<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent>();
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.NextFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.NextFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.NextFocusIntent>)this._handleGridNextFocus), [typeof(global::Doroti.Framework.Widgets.PreviousFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.PreviousFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.PreviousFocusIntent>)this._handleGridPreviousFocus), [typeof(global::Doroti.Framework.Widgets.DirectionalFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.DirectionalFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Framework.Widgets.DirectionalFocusIntent>)this._handleDirectionFocus) }.cast<Type, dynamic>();
        _dayGridFocus = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: "Day Grid");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _localizations = MaterialLocalizations.of(this.context);
    }

    public override void dispose()
    {
        this._pageController.dispose();
        this._dayGridFocus.dispose();
        base.dispose();
    }

    internal virtual void _handleDateSelected(DateTime selectedDate)
    {
        _focusedDay = selectedDate;
        this.widget.onChanged(selectedDate);
    }

    internal virtual void _announce(string message)
    {
        if ((MediaQuery.maybeSupportsAnnounceOf(this.context) ?? false))
        {
            DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), message, Directionality.of(this.context)).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
        }
        else
        {
            _announcementText = message;
        }
    }

    internal virtual void _handleMonthPageChanged(long monthPage)
    {
        setState(((global::System.Action)(() =>
        {
            DateTime monthDate = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_MonthPicker__calendar_date_picker)this.widget).firstDate, monthPage);
            if (!((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(this._currentMonth, monthDate))
            {
                _currentMonth = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getMonth(monthDate.Year, monthDate.Month);
                this.widget.onDisplayedMonthChanged(this._currentMonth);
                if (((this._focusedDay is not null) && !((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(this._focusedDay, this._currentMonth)))
                {
                    _focusedDay = _focusableDayForMonth(this._currentMonth, DartRuntimePrimitives.RequireValue(this._focusedDay).Day);
                }
                _announce(((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.formatMonthYear(this._currentMonth, this._localizations));
            }
        })));
    }

    internal virtual DateTime? _focusableDayForMonth(DateTime month, long preferredDay)
    {
        long daysInMonth = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(month.Year, month.Month);
        if ((preferredDay <= daysInMonth))
        {
            DateTime newFocus = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(month.Year, month.Month, preferredDay);
            if (_isSelectable(newFocus))
            {
                return newFocus;
            }
        }
        for (var day = 1L; (day <= daysInMonth); day++)
        {
            DateTime newFocusLocal = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(month.Year, month.Month, day);
            if (_isSelectable(newFocusLocal))
            {
                return newFocusLocal;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleNextMonth()
    {
        if (!this._isDisplayingLastMonth)
        {
            DartRuntimePrimitives.Ignore(this._pageController.nextPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Framework.Animation.Curves.ease));
        }
    }

    internal virtual void _handlePreviousMonth()
    {
        if (!this._isDisplayingFirstMonth)
        {
            DartRuntimePrimitives.Ignore(this._pageController.previousPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Framework.Animation.Curves.ease));
        }
    }

    internal virtual void _showMonth(DateTime month, bool jump = false)
    {
        long monthPage = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, month);
        if (jump)
        {
            this._pageController.jumpToPage(monthPage);
        }
        else
        {
            DartRuntimePrimitives.Ignore(this._pageController.animateToPage(monthPage, duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Framework.Animation.Curves.ease));
        }
    }

    internal virtual bool _isDisplayingFirstMonth
    {
        get
        {
            return !this._currentMonth.isAfter(((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getMonth(((_MonthPicker__calendar_date_picker)this.widget).firstDate.Year, ((_MonthPicker__calendar_date_picker)this.widget).firstDate.Month));
            return default!;
        }
    }
    internal virtual bool _isDisplayingLastMonth
    {
        get
        {
            return !this._currentMonth.isBefore(((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getMonth(((_MonthPicker__calendar_date_picker)this.widget).lastDate.Year, ((_MonthPicker__calendar_date_picker)this.widget).lastDate.Month));
            return default!;
        }
    }
    internal virtual void _handleGridFocusChange(bool focused)
    {
        setState(((global::System.Action)(() =>
        {
            if ((focused && (this._focusedDay is null)))
            {
                if (((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(((_MonthPicker__calendar_date_picker)this.widget).selectedDate, this._currentMonth))
                {
                    _focusedDay = ((_MonthPicker__calendar_date_picker)this.widget).selectedDate;
                }
                else
                {
                    if (((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(((_MonthPicker__calendar_date_picker)this.widget).currentDate, this._currentMonth))
                    {
                        _focusedDay = _focusableDayForMonth(this._currentMonth, ((_MonthPicker__calendar_date_picker)this.widget).currentDate.Day);
                    }
                    else
                    {
                        _focusedDay = _focusableDayForMonth(this._currentMonth, 1L);
                    }
                }
            }
        })));
    }

    internal virtual void _handleGridNextFocus(global::Doroti.Framework.Widgets.NextFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(global::Doroti.Framework.Widgets.PreviousFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(global::Doroti.Framework.Widgets.DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => (this._focusedDay is not null));
        setState(((global::System.Action)(() =>
        {
            DateTime? nextDate = _nextDateInDirection(DartRuntimePrimitives.RequireValue(this._focusedDay), ((global::Doroti.Framework.Widgets.DirectionalFocusIntent)intent).direction);
            if ((nextDate is not null))
            {
                DateTime nextDate__29939__value30014 = DartRuntimePrimitives.RequireValue(nextDate);
                _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__29939__value30014);
                if (!((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(this._focusedDay, this._currentMonth))
                {
                    _showMonth(DartRuntimePrimitives.RequireValue(this._focusedDay));
                }
            }
        })));
    }

    internal virtual long _dayDirectionOffset(global::Doroti.Framework.Widgets.TraversalDirection traversalDirection, TextDirection textDirection)
    {
        if ((object.Equals(textDirection, TextDirection.rtl)))
        {
            if ((object.Equals(traversalDirection, global::Doroti.Framework.Widgets.TraversalDirection.left)))
            {
                traversalDirection = global::Doroti.Framework.Widgets.TraversalDirection.right;
            }
            else
            {
                if ((object.Equals(traversalDirection, global::Doroti.Framework.Widgets.TraversalDirection.right)))
                {
                    traversalDirection = global::Doroti.Framework.Widgets.TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, global::Doroti.Framework.Widgets.TraversalDirection direction)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(this.context);
        DateTime nextDate = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addDaysToDate(date, _dayDirectionOffset(direction, textDirection));
        while ((!nextDate.isBefore(((_MonthPicker__calendar_date_picker)this.widget).firstDate) && !nextDate.isAfter(((_MonthPicker__calendar_date_picker)this.widget).lastDate)))
        {
            if (_isSelectable(nextDate))
            {
                return nextDate;
            }
            nextDate = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addDaysToDate(nextDate, _dayDirectionOffset(direction, textDirection));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return ((((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate is null ? true : ((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate.Invoke(date)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildItems(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        DateTime month = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_MonthPicker__calendar_date_picker)this.widget).firstDate, index);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _DayPicker__calendar_date_picker(key: new global::Doroti.Framework.Foundation.ValueKey<DateTime>(month), calendarDelegate: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate, selectedDate: ((_MonthPicker__calendar_date_picker)this.widget).selectedDate, currentDate: ((_MonthPicker__calendar_date_picker)this.widget).currentDate, onChanged: (global::System.Action<DateTime>)this._handleDateSelected, firstDate: ((_MonthPicker__calendar_date_picker)this.widget).firstDate, lastDate: ((_MonthPicker__calendar_date_picker)this.widget).lastDate, displayedMonth: month, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? subHeaderForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)(DatePickerTheme.of(context).subHeaderForegroundColor ?? DatePickerTheme.defaults(context).subHeaderForegroundColor));
        bool supportsAnnounce = (MediaQuery.maybeSupportsAnnounceOf(context) ?? false);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(container: true, explicitChildNodes: true, liveRegion: !supportsAnnounce, accessibilityFocusBlockType: (!supportsAnnounce ? global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.blockNode : global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.none), label: (!supportsAnnounce ? this._announcementText : null), child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Spacer()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.chevron_left, semanticLabel: (this._isDisplayingFirstMonth ? ((MaterialLocalizations)this._localizations).previousMonthTooltip : null)), color: subHeaderForegroundColorLocal, tooltip: (this._isDisplayingFirstMonth ? null : ((MaterialLocalizations)this._localizations).previousMonthTooltip), onPressed: ((global::System.Action)(this._isDisplayingFirstMonth ? null : this._handlePreviousMonth)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.chevron_right, semanticLabel: (this._isDisplayingLastMonth ? ((MaterialLocalizations)this._localizations).nextMonthTooltip : null)), color: subHeaderForegroundColorLocal, tooltip: (this._isDisplayingLastMonth ? null : ((MaterialLocalizations)this._localizations).nextMonthTooltip), onPressed: ((global::System.Action)(this._isDisplayingLastMonth ? null : this._handleNextMonth)))) })))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.FocusableActionDetector(shortcuts: this._shortcutMap, actions: this._actionMap, focusNode: this._dayGridFocus, onFocusChange: (global::System.Action<bool>)this._handleGridFocusChange, child: new _FocusedDate__calendar_date_picker(calendarDelegate: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate, date: (((global::Doroti.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._focusedDay : null), child: new Material(type: MaterialType.transparency, child: global::Doroti.Framework.Widgets.PageView.CreateBuilder(key: this._pageViewKey, controller: this._pageController, itemBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget>)this._buildItems, itemCount: (((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, ((_MonthPicker__calendar_date_picker)this.widget).lastDate) + 1L), onPageChanged: (global::System.Action<long>)this._handleMonthPageChanged)))))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusedDate__calendar_date_picker : global::Doroti.Framework.Widgets.InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }

    internal _FocusedDate__calendar_date_picker(global::Doroti.Framework.Widgets.Widget child, CalendarDelegate<DateTime> calendarDelegate, DateTime? date = null) : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__calendar_date_picker)(object)oldWidget;
        return !this.calendarDelegate.isSameDay(this.date, ((_FocusedDate__calendar_date_picker)__oldWidget).date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _FocusedDate__calendar_date_picker? focusedDate = ((_FocusedDate__calendar_date_picker?)(object?)context.dependOnInheritedWidgetOfExactType<_FocusedDate__calendar_date_picker>());
        return focusedDate?.date;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPicker__calendar_date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? selectedDate { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime displayedMonth { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _DayPicker__calendar_date_picker(global::Doroti.Framework.Foundation.Key? key = null, DateTime currentDate = default!, DateTime displayedMonth = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::System.Func<DateTime, bool>? selectableDayPredicate = null) : base(key: key)
    {
        this.currentDate = currentDate;
        this.displayedMonth = displayedMonth;
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.selectedDate = selectedDate;
        this.onChanged = onChanged;
        this.calendarDelegate = calendarDelegate;
        this.selectableDayPredicate = selectableDayPredicate;
        System.Diagnostics.Debug.Assert(!firstDate.isAfter(lastDate));
        System.Diagnostics.Debug.Assert(((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate)));
        System.Diagnostics.Debug.Assert(((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DayPickerState__calendar_date_picker());
}

internal class _DayPickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_DayPicker__calendar_date_picker>
{
    internal virtual List<global::Doroti.Framework.Widgets.FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Year, ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Month);
        _dayFocusNodes = new List<global::Doroti.Framework.Widgets.FocusNode>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)daysInMonth)), ((index) => new global::Doroti.Framework.Widgets.FocusNode(skipTraversal: true, debugLabel: $"Day {(index + 1L)}"))));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate = _FocusedDate__calendar_date_picker.maybeOf(this.context);
        if (((focusedDate is not null) && ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(((_DayPicker__calendar_date_picker)this.widget).displayedMonth, DartRuntimePrimitives.RequireValue(focusedDate))))
        {
            DateTime focusedDate__38602__value38655 = DartRuntimePrimitives.RequireValue(focusedDate);
            this._dayFocusNodes[(int)((DartRuntimePrimitives.RequireValue(focusedDate__38602__value38655).Day - 1L))].requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.FocusNode node in this._dayFocusNodes)
        {
            node.dispose();
        }
        base.dispose();
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _dayHeaders(global::Doroti.Framework.Painting.TextStyle? headerStyle, MaterialLocalizations localizations)
    {
        var result = new List<global::Doroti.Framework.Widgets.Widget>();
        for (long i = ((MaterialLocalizations)localizations).firstDayOfWeekIndex; (checked((long)(result.Count)) < 7L); i = (((i + 1L)) % 7L))
        {
            string weekday = ((MaterialLocalizations)localizations).narrowWeekdays[(int)(i)];
            result.Add(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(weekday, style: headerStyle))));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Painting.TextStyle? weekdayStyleLocal = (datePickerTheme.weekdayStyle ?? defaultsLocal.weekdayStyle);
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        var isLandscapeOrientation = (object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.landscape));
        long year = ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Year;
        long month = ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Month;
        long daysInMonth = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(year, month);
        long dayOffset = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.firstDayOffset(year, month, localizations);
        List<global::Doroti.Framework.Widgets.Widget> dayItems = ((List<global::Doroti.Framework.Widgets.Widget>)(object?)_dayHeaders(weekdayStyleLocal, localizations));
        long day = -dayOffset;
        while ((day < daysInMonth))
        {
            day++;
            if ((day < 1L))
            {
                dayItems.Add(global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
            }
            else
            {
                DateTime dayToBuild = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(year, month, day);
                bool isDisabledLocal = ((dayToBuild.isAfter(((_DayPicker__calendar_date_picker)this.widget).lastDate) || dayToBuild.isBefore(((_DayPicker__calendar_date_picker)this.widget).firstDate)) || (((((_DayPicker__calendar_date_picker)this.widget).selectableDayPredicate is not null) && !((_DayPicker__calendar_date_picker)this.widget).selectableDayPredicate!(dayToBuild))));
                bool isSelectedDayLocal = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameDay(((_DayPicker__calendar_date_picker)this.widget).selectedDate, dayToBuild);
                bool isTodayLocal = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameDay(((_DayPicker__calendar_date_picker)this.widget).currentDate, dayToBuild);
                dayItems.Add(new _Day__calendar_date_picker(dayToBuild, key: new global::Doroti.Framework.Foundation.ValueKey<DateTime>(dayToBuild), isDisabled: isDisabledLocal, isSelectedDay: isSelectedDayLocal, isToday: isTodayLocal, onChanged: (global::System.Action<DateTime>)((_DayPicker__calendar_date_picker)this.widget).onChanged, focusNode: this._dayFocusNodes[(int)((day - 1L))], calendarDelegate: ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate));
            }
        }
        double monthPickerHorizontalPadding = ((Theme.of(context).useMaterial3 && !isLandscapeOrientation) ? Calendar_date_pickerLibrary._monthPickerHorizontalPaddingPortraitM3 : Calendar_date_pickerLibrary._monthPickerHorizontalPaddingOther);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: monthPickerHorizontalPadding), child: MediaQuery.withClampedTextScaling(maxScaleFactor: (isLandscapeOrientation ? Calendar_date_pickerLibrary._kDayPickerGridLandscapeMaxScaleFactor : Calendar_date_pickerLibrary._kDayPickerGridPortraitMaxScaleFactor), child: global::Doroti.Framework.Widgets.GridView.CreateCustom(physics: new global::Doroti.Framework.Widgets.ClampingScrollPhysics(), gridDelegate: new _DayPickerGridDelegate__calendar_date_picker(context), childrenDelegate: new global::Doroti.Framework.Widgets.SliverChildListDelegate(dayItems, addRepaintBoundaries: false)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Day__calendar_date_picker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual bool isDisabled { get; private set; } = default!;
    public virtual bool isSelectedDay { get; private set; } = default!;
    public virtual bool isToday { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode focusNode { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _Day__calendar_date_picker(DateTime day, global::Doroti.Framework.Foundation.Key? key = null, bool isDisabled = default!, bool isSelectedDay = default!, bool isToday = default!, global::System.Action<DateTime> onChanged = default!, global::Doroti.Framework.Widgets.FocusNode focusNode = default!, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        this.day = day;
        this.isDisabled = isDisabled;
        this.isSelectedDay = isSelectedDay;
        this.isToday = isToday;
        this.onChanged = onChanged;
        this.focusNode = focusNode;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DayState__calendar_date_picker());
}

internal class _DayState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_Day__calendar_date_picker>
{
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Framework.Widgets.WidgetStatesController();

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        global::Doroti.Framework.Painting.TextStyle? dayStyleLocal = (datePickerTheme.dayStyle ?? defaultsLocal.dayStyle);
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme) ?? getProperty(defaultsLocal));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) =>
            {
                return getProperty(theme) is { } property ? property.resolve(states) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        var semanticLabelSuffix = (((_Day__calendar_date_picker)this.widget).isToday ? $", {((MaterialLocalizations)localizations).currentDateLabel}" : "");
        var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection44458 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((_Day__calendar_date_picker)this.widget).isDisabled) { __collection44458.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (((_Day__calendar_date_picker)this.widget).isSelectedDay) { __collection44458.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection44458; }))();
        this._statesController.value = statesLocal;
        global::Doroti.Ui.Color? dayForegroundColorLocal = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (((_Day__calendar_date_picker)this.widget).isToday ? theme?.todayForegroundColor : theme?.dayForegroundColor)), statesLocal));
        global::Doroti.Ui.Color? dayBackgroundColorLocal = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (((_Day__calendar_date_picker)this.widget).isToday ? theme?.todayBackgroundColor : theme?.dayBackgroundColor)), statesLocal));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> dayOverlayColorLocal = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => theme?.dayOverlayColor?.resolve(states)))))));
        global::Doroti.Framework.Painting.OutlinedBorder dayShapeLocal = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((theme) => theme?.dayShape), statesLocal)!;
        bool hasCustomBorderColor = ((datePickerTheme.todayBorder is not null) && (datePickerTheme.todayBorder!.color.opacity != 0.0));
        global::Doroti.Framework.Painting.BorderSide todayBorderSide = (hasCustomBorderColor ? datePickerTheme.todayBorder! : ((datePickerTheme.todayBorder ?? defaultsLocal.todayBorder!)).copyWith(color: dayForegroundColorLocal));
        var decorationLocal = (((_Day__calendar_date_picker)this.widget).isToday ? new global::Doroti.Framework.Painting.ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal.copyWith(side: todayBorderSide)) : new global::Doroti.Framework.Painting.ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal));
        global::Doroti.Framework.Widgets.Widget dayWidget = ((global::Doroti.Framework.Widgets.Widget)(object?)new Ink(decoration: decorationLocal, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(localizations.formatDecimal(((_Day__calendar_date_picker)this.widget).day.Day), style: dayStyleLocal?.apply(color: dayForegroundColorLocal)))));
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        if ((Theme.of(context).useMaterial3 && (object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait))))
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0), child: dayWidget));
        }
        dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: $"{localizations.formatDecimal(((_Day__calendar_date_picker)this.widget).day.Day)}, {((_Day__calendar_date_picker)this.widget).calendarDelegate.formatFullDate(((_Day__calendar_date_picker)this.widget).day, localizations)}{semanticLabelSuffix}", button: true, selected: ((_Day__calendar_date_picker)this.widget).isSelectedDay, enabled: !((_Day__calendar_date_picker)this.widget).isDisabled, excludeSemantics: true, child: dayWidget));
        if (!((_Day__calendar_date_picker)this.widget).isDisabled)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkResponse(focusNode: ((_Day__calendar_date_picker)this.widget).focusNode, onTap: (() => { this.widget.onChanged(((_Day__calendar_date_picker)this.widget).day); }), statesController: this._statesController, overlayColor: dayOverlayColorLocal, customBorder: dayShapeLocal, containedInkWell: true, child: dayWidget));
        }
        return dayWidget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._statesController.dispose();
        base.dispose();
    }

}

internal class _DayPickerGridDelegate__calendar_date_picker : global::Doroti.Framework.Rendering.SliverGridDelegate
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DayPickerGridDelegate__calendar_date_picker(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Framework.Rendering.SliverConstraints constraints)
    {
        double textScaleFactor = (MediaQuery.textScalerOf(this.context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(this.context);
        double dayPickerRowHeight = ((Theme.of(this.context).useMaterial3 && (object.Equals(orientation, global::Doroti.Framework.Widgets.Orientation.portrait))) ? Calendar_date_pickerLibrary._dayPickerRowHeightM3 : Calendar_date_pickerLibrary._dayPickerRowHeightM2);
        double scaledRowHeight = ((textScaleFactor > 1.3) ? (((((textScaleFactor - 1L)) * 30L)) + dayPickerRowHeight) : dayPickerRowHeight);
        long columnCount = 7L;
        double tileWidth = (((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent / columnCount);
        double tileHeight = Math.Min(scaledRowHeight, (((global::Doroti.Framework.Rendering.SliverConstraints)constraints).viewportMainAxisExtent / ((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L))));
        return ((global::Doroti.Framework.Rendering.SliverGridLayout)(object?)new global::Doroti.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth, childMainAxisExtent: tileHeight, crossAxisCount: columnCount, crossAxisStride: tileWidth, mainAxisStride: tileHeight, reverseCrossAxis: global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public class YearPicker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime? selectedDate { get; private set; }
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public YearPicker(global::Doroti.Framework.Foundation.Key? key = null, DateTime? currentDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? initialDate = null, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.selectedDate = selectedDate;
        this.onChanged = onChanged;
        this.dragStartBehavior = dragStartBehavior;
        this.calendarDelegate = __calendarDelegate;
        this.currentDate = calendarDelegate.dateOnly((currentDate ?? new DateTime()));
        System.Diagnostics.Debug.Assert(!firstDate.isAfter(lastDate));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _YearPickerState__calendar_date_picker());
}

internal class _YearPickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<YearPicker>
{
    internal virtual global::Doroti.Framework.Widgets.ScrollController? _scrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Framework.Widgets.WidgetStatesController();
    public const long minYears = 18L;

    public override void initState()
    {
        base.initState();
        _scrollController = new global::Doroti.Framework.Widgets.ScrollController(initialScrollOffset: _scrollOffsetForYear((((YearPicker)this.widget).selectedDate ?? ((YearPicker)this.widget).firstDate)));
    }

    public override void dispose()
    {
        this._scrollController?.dispose();
        this._statesController.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(YearPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((YearPicker)this.widget).selectedDate, ((YearPicker)oldWidget).selectedDate)) && (((YearPicker)this.widget).selectedDate is not null)))
        {
            this._scrollController!.jumpTo(_scrollOffsetForYear(DartRuntimePrimitives.RequireValue(((YearPicker)this.widget).selectedDate)));
        }
    }

    internal virtual double _scrollOffsetForYear(DateTime date)
    {
        long initialYearIndex = (date.Year - ((YearPicker)this.widget).firstDate.Year);
        long initialYearRow = (checked((long)(initialYearIndex / Calendar_date_pickerLibrary._yearPickerColumnCount)));
        long centeredYearRow = (initialYearRow - 2L);
        return ((this._itemCount < minYears) ? 0 : (centeredYearRow * Calendar_date_pickerLibrary._yearPickerRowHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildYearItem(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme) ?? getProperty(defaultsLocal));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) =>
            {
                return getProperty(theme) is { } property ? property.resolve(states) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double textScaleFactor = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        long offset = ((this._itemCount < minYears) ? (checked((long)(((minYears - this._itemCount)) / 2L))) : 0L);
        long year = ((((YearPicker)this.widget).firstDate.Year + index) - offset);
        var isSelected = (year == ((YearPicker)this.widget).selectedDate?.Year);
        var isCurrentYear = (year == ((YearPicker)this.widget).currentDate.Year);
        bool isDisabled = ((year < ((YearPicker)this.widget).firstDate.Year) || (year > ((YearPicker)this.widget).lastDate.Year));
        double decorationHeight = (36.0 * textScaleFactor);
        double decorationWidth = (72.0 * textScaleFactor);
        var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection54093 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isDisabled) { __collection54093.Add(global::Doroti.Framework.Widgets.WidgetState.disabled); } if (isSelected) { __collection54093.Add(global::Doroti.Framework.Widgets.WidgetState.selected); } return __collection54093; }))();
        global::Doroti.Ui.Color? textColor = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (isCurrentYear ? theme?.todayForegroundColor : theme?.yearForegroundColor)), statesLocal));
        global::Doroti.Ui.Color? background = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (isCurrentYear ? theme?.todayBackgroundColor : theme?.yearBackgroundColor)), statesLocal));
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColorLocal = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => theme?.yearOverlayColor?.resolve(states)))))));
        global::Doroti.Framework.Painting.OutlinedBorder yearShapeLocal = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>(((theme) => theme?.yearShape), statesLocal)!;
        global::Doroti.Framework.Painting.BorderSide? borderSide = default!;
        if (isCurrentYear)
        {
            borderSide = (datePickerTheme.todayBorder ?? defaultsLocal.todayBorder);
            if ((borderSide is not null))
            {
                borderSide = borderSide.copyWith(color: textColor);
            }
        }
        var decorationLocal = new global::Doroti.Framework.Painting.ShapeDecoration(color: background, shape: yearShapeLocal.copyWith(side: borderSide));
        global::Doroti.Framework.Painting.TextStyle? itemStyle = ((global::Doroti.Framework.Painting.TextStyle?)(object?)((datePickerTheme.yearStyle ?? defaultsLocal.yearStyle))?.apply(color: textColor));
        MaterialLocalizations localizations = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Framework.Widgets.Widget yearItem = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(decoration: decorationLocal, height: decorationHeight, width: decorationWidth, alignment: global::Doroti.Framework.Painting.Alignment.center, child: new global::Doroti.Framework.Widgets.Semantics(selected: isSelected, enabled: !isDisabled, button: true, child: new global::Doroti.Framework.Widgets.Text(((YearPicker)this.widget).calendarDelegate.formatYear(year, localizations), style: itemStyle)))));
        if (!isDisabled)
        {
            DateTime date = ((YearPicker)this.widget).calendarDelegate.getMonth(year, (((YearPicker)this.widget).selectedDate?.Month ?? 1L));
            if (date.isBefore(((YearPicker)this.widget).calendarDelegate.getMonth(((YearPicker)this.widget).firstDate.Year, ((YearPicker)this.widget).firstDate.Month)))
            {
                DartRuntimePrimitives.Assert(() => (date.Year == ((YearPicker)this.widget).firstDate.Year));
                date = ((YearPicker)this.widget).calendarDelegate.getMonth(year, ((YearPicker)this.widget).firstDate.Month);
            }
            else
            {
                if (date.isAfter(((YearPicker)this.widget).lastDate))
                {
                    DartRuntimePrimitives.Assert(() => (date.Year == ((YearPicker)this.widget).lastDate.Year));
                    date = ((YearPicker)this.widget).calendarDelegate.getMonth(year, ((YearPicker)this.widget).lastDate.Month);
                }
            }
            this._statesController.value = statesLocal;
            yearItem = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(key: new global::Doroti.Framework.Foundation.ValueKey<long>(year), onTap: (() => { this.widget.onChanged(date); }), statesController: this._statesController, overlayColor: overlayColorLocal, child: yearItem));
        }
        return yearItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _itemCount
    {
        get
        {
            return ((((YearPicker)this.widget).lastDate.Year - ((YearPicker)this.widget).firstDate.Year) + 1L);
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new Material(type: MaterialType.transparency, child: global::Doroti.Framework.Widgets.GridView.CreateBuilder(controller: this._scrollController, dragStartBehavior: ((YearPicker)this.widget).dragStartBehavior, gridDelegate: new _YearPickerGridDelegate__calendar_date_picker(context), itemBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget>)this._buildYearItem, itemCount: Math.Max(this._itemCount, minYears), padding: global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Calendar_date_pickerLibrary._yearPickerPadding))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider()) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _YearPickerGridDelegate__calendar_date_picker : global::Doroti.Framework.Rendering.SliverGridDelegate
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _YearPickerGridDelegate__calendar_date_picker(global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Framework.Rendering.SliverConstraints constraints)
    {
        double textScaleFactor = (MediaQuery.textScalerOf(this.context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        long scaledYearPickerColumnCount = ((textScaleFactor > 1.65) ? (Calendar_date_pickerLibrary._yearPickerColumnCount - 1L) : Calendar_date_pickerLibrary._yearPickerColumnCount);
        double tileWidth = Math.Max((((((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent - (((scaledYearPickerColumnCount - 1L)) * Calendar_date_pickerLibrary._yearPickerRowSpacing))) / scaledYearPickerColumnCount), 0.0);
        double scaledYearPickerRowHeight = ((textScaleFactor > 1L) ? (Calendar_date_pickerLibrary._yearPickerRowHeight + ((((textScaleFactor - 1L)) * 9L))) : Calendar_date_pickerLibrary._yearPickerRowHeight);
        return ((global::Doroti.Framework.Rendering.SliverGridLayout)(object?)new global::Doroti.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth, childMainAxisExtent: scaledYearPickerRowHeight, crossAxisCount: scaledYearPickerColumnCount, crossAxisStride: (tileWidth + Calendar_date_pickerLibrary._yearPickerRowSpacing), mainAxisStride: scaledYearPickerRowHeight, reverseCrossAxis: global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static void _reportAnnouncementError(object exception, global::System.Diagnostics.StackTrace stack)
    {
        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
    }
}
