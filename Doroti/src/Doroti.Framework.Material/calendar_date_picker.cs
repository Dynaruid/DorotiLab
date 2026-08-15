// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/calendar_date_picker.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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

public class CalendarDatePicker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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

    public CalendarDatePicker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime? initialDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? currentDate = null, global::System.Action<DateTime> onDateChanged = default!, global::System.Action<DateTime>? onDisplayedMonthChanged = null, DatePickerMode initialCalendarMode = DatePickerMode.day, global::System.Func<DateTime, bool>? selectableDayPredicate = null, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
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

internal class _CalendarDatePickerState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<CalendarDatePicker>
{
    internal virtual bool _announcedInitialDate { get; set; } = false;
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DatePickerMode _mode { get; set; } = default!;
    internal virtual DateTime _currentDisplayedMonthDate { get; set; } = default!;
    internal virtual DateTime? _selectedDate { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _monthPickerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _yearPickerKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _mode = ((CalendarDatePicker)this.widget).initialCalendarMode;
        DateTime currentDisplayedDate__8881 = (((CalendarDatePicker)this.widget).initialDate ?? ((CalendarDatePicker)this.widget).currentDate);
        _currentDisplayedMonthDate = ((CalendarDatePicker)this.widget).calendarDelegate.getMonth(currentDisplayedDate__8881.Year, currentDisplayedDate__8881.Month);
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(this.context));
        _localizations = MaterialLocalizations.of(this.context);
        _textDirection = Directionality.of(this.context);
        if ((!this._announcedInitialDate && (((CalendarDatePicker)this.widget).initialDate is not null)))
        {
            DartRuntimePrimitives.Assert(() => (this._selectedDate is not null));
            _announcedInitialDate = true;
            bool isToday__9668 = ((CalendarDatePicker)this.widget).calendarDelegate.isSameDay(((CalendarDatePicker)this.widget).currentDate, this._selectedDate);
            var semanticLabelSuffix__9760 = (isToday__9668 ? $", {((MaterialLocalizations)this._localizations).currentDateLabel}" : "");
            _announce($"{this._localizations.formatFullDate(DartRuntimePrimitives.RequireValue(this._selectedDate))}{semanticLabelSuffix__9760}");
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
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                {
                    break;
                }
        }
    }

    internal virtual void _handleModeChanged(DatePickerMode mode)
    {
        _vibrate();
        setState(((global::System.Action)(() => {
_mode = mode;
if (this._selectedDate is DateTime selected__10936)
{
    string message__10969 = (mode switch { DatePickerMode.day => ((CalendarDatePicker)this.widget).calendarDelegate.formatMonthYear(selected__10936, this._localizations), DatePickerMode.year => ((CalendarDatePicker)this.widget).calendarDelegate.formatYear(selected__10936.Year, this._localizations), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    _announce(message__10969);
}
})));
    }

    internal virtual void _handleMonthChanged(DateTime date)
    {
        setState(((global::System.Action)(() => {
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
        long daysInMonth__11698 = ((CalendarDatePicker)this.widget).calendarDelegate.getDaysInMonth(value.Year, value.Month);
        long preferredDay__11791 = Math.Min((this._selectedDate?.Day ?? 1L), daysInMonth__11698);
        value = ((CalendarDatePicker)this.widget).calendarDelegate.getDay(value.Year, value.Month, preferredDay__11791);
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
        setState(((global::System.Action)(() => {
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
        setState(((global::System.Action)(() => {
_selectedDate = value;
this.widget.onDateChanged(DartRuntimePrimitives.RequireValue(this._selectedDate));
switch (Theme.of(this.context).platform)
{
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
        {
            bool isToday__12642 = ((CalendarDatePicker)this.widget).calendarDelegate.isSameDay(((CalendarDatePicker)this.widget).currentDate, this._selectedDate);
            var semanticLabelSuffix__12738 = (isToday__12642 ? $", {((MaterialLocalizations)this._localizations).currentDateLabel}" : "");
            DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(this.context), $"{((MaterialLocalizations)this._localizations).selectedDateLabel} {((CalendarDatePicker)this.widget).calendarDelegate.formatFullDate(DartRuntimePrimitives.RequireValue(this._selectedDate), this._localizations)}{semanticLabelSuffix__12738}", this._textDirection).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
            break;
        }
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
    case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
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

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildPicker()
    {
        switch (this._mode)
        {
            case DatePickerMode.day:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MonthPicker__calendar_date_picker(key: this._monthPickerKey, calendarDelegate: ((CalendarDatePicker)this.widget).calendarDelegate, initialMonth: this._currentDisplayedMonthDate, currentDate: ((CalendarDatePicker)this.widget).currentDate, firstDate: ((CalendarDatePicker)this.widget).firstDate, lastDate: ((CalendarDatePicker)this.widget).lastDate, selectedDate: this._selectedDate, onChanged: (global::System.Action<DateTime>)this._handleDayChanged, onDisplayedMonthChanged: (global::System.Action<DateTime>)this._handleMonthChanged, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((CalendarDatePicker)this.widget).selectableDayPredicate));
                }
            case DatePickerMode.year:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: Calendar_date_pickerLibrary._subHeaderHeight), child: new YearPicker(key: this._yearPickerKey, calendarDelegate: ((CalendarDatePicker)this.widget).calendarDelegate, currentDate: ((CalendarDatePicker)this.widget).currentDate, firstDate: ((CalendarDatePicker)this.widget).firstDate, lastDate: ((CalendarDatePicker)this.widget).lastDate, selectedDate: this._currentDisplayedMonthDate, onChanged: (global::System.Action<DateTime>)this._handleYearChanged)));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        double textScaleFactor__14651 = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__14928 = MediaQuery.orientationOf(context);
        double maxDayPickerHeight__14994 = ((Theme.of(context).useMaterial3 && (object.Equals(orientation__14928, global::Doroti.Generated.Framework.Widgets.Orientation.portrait))) ? Calendar_date_pickerLibrary._maxDayPickerHeightM3 : Calendar_date_pickerLibrary._maxDayPickerHeightM2);
        double scaledMaxDayPickerHeight__15444 = ((textScaleFactor__14651 > 1.3) ? (maxDayPickerHeight__14994 + ((((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)) * ((((textScaleFactor__14651 - 1L)) * 8L))))) : maxDayPickerHeight__14994);
        var picker__15624 = new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (Calendar_date_pickerLibrary._subHeaderHeight + scaledMaxDayPickerHeight__15444), child: _buildPicker());
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection15772 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); if ((MediaQuery.maybeSupportsAnnounceOf(context) ?? false)) { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(picker__15624)); } else { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, liveRegion: true, accessibilityFocusBlockType: global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.blockNode, label: this._announcementText, child: picker__15624))); } __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(MediaQuery.withClampedTextScaling(maxScaleFactor: Calendar_date_pickerLibrary._kModeToggleButtonMaxScaleFactor, child: new _DatePickerModeToggleButton__calendar_date_picker(mode: this._mode, title: ((CalendarDatePicker)this.widget).calendarDelegate.formatMonthYear(this._currentDisplayedMonthDate, this._localizations), onTitlePressed: ((global::System.Action)(() => { _handleModeChanged((this._mode switch { DatePickerMode.day => DatePickerMode.year, DatePickerMode.year => DatePickerMode.day, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })); })))))); return __collection15772; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DatePickerModeToggleButton__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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

public class _DatePickerModeToggleButtonState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<_DatePickerModeToggleButton__calendar_date_picker>, global::Doroti.Generated.Framework.Widgets.SingleTickerProviderStateMixin<_DatePickerModeToggleButton__calendar_date_picker>
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new global::Doroti.Generated.Framework.Animation.AnimationController(value: ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode, DatePickerMode.year)) ? 0.5 : 0), upperBound: 0.5, duration: Duration.Create(milliseconds: 200L), vsync: this);
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DatePickerThemeData datePickerTheme__18360 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__18437 = DatePickerTheme.defaults(context);
        global::Doroti.Generated.Framework.Painting.TextStyle? buttonTextStyle__18504 = (datePickerTheme__18360.toggleButtonTextStyle ?? defaults__18437.toggleButtonTextStyle);
        global::Doroti.Ui.Color? subHeaderForegroundColor__18620 = ((global::Doroti.Ui.Color?)(object?)(datePickerTheme__18360.subHeaderForegroundColor ?? defaults__18437.subHeaderForegroundColor));
        global::Doroti.Ui.Color? buttonTextColor__18751 = ((global::Doroti.Ui.Color?)(object?)((datePickerTheme__18360.toggleButtonTextStyle?.color ?? datePickerTheme__18360.subHeaderForegroundColor) ?? defaults__18437.toggleButtonTextStyle?.color));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Generated.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection19110 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).selectYearSemanticsLabel, button: true, container: true, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new InkWell(onTap: ((_DatePickerModeToggleButton__calendar_date_picker)this.widget).onTitlePressed, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: new global::Doroti.Generated.Framework.Widgets.Text(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).title, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, style: buttonTextStyle__18504?.apply(color: buttonTextColor__18751)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.RotationTransition(turns: this._controller, child: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.arrow_drop_down, color: subHeaderForegroundColor__18620))) })))))))); if ((object.Equals(((_DatePickerModeToggleButton__calendar_date_picker)this.widget).mode, DatePickerMode.day))) { __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: Calendar_date_pickerLibrary._monthNavButtonsWidth))); } return __collection19110; }))()))));
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
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), this._ticker!.describeForError("The offending ticker was") }));
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        this._ticker = new global::Doroti.Generated.Framework.Scheduler.Ticker((global::System.Action<Duration>)onTick, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
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
        TickerModeData values__15157 = this._tickerModeNotifier!.value;
        if ((this._ticker is not null))
        {
            this._ticker!.muted = !((TickerModeData)values__15157).enabled;
            this._ticker!.forceFrames = ((TickerModeData)values__15157).forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__15400 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__15400, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTicker());
        newNotifier__15400.addListener(() => this._updateTicker());
        this._tickerModeNotifier = newNotifier__15400;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription__15805 = ((this._ticker?.isActive, this._ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) });
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Scheduler.Ticker>("ticker", this._ticker, description: tickerDescription__15805, showSeparator: false, defaultValue: default));
    }

}

internal class _MonthPicker__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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

    internal _MonthPicker__calendar_date_picker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime initialMonth = default!, DateTime currentDate = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, global::System.Action<DateTime> onDisplayedMonthChanged = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::System.Func<DateTime, bool>? selectableDayPredicate = null) : base(key: key)
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

internal class _MonthPickerState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<_MonthPicker__calendar_date_picker>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> _pageViewKey { get; private set; } = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create();
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DateTime _currentMonth { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.PageController _pageController { get; set; } = default!;
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? _shortcutMap { get; set; } = default;
    internal virtual DartMap<Type, dynamic>? _actionMap { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<global::Doroti.Generated.Framework.Widgets.TraversalDirection, long> _directionOffset = new DartMap<global::Doroti.Generated.Framework.Widgets.TraversalDirection, long> { [global::Doroti.Generated.Framework.Widgets.TraversalDirection.up] = -7L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.right] = 1L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.down] = 7L, [global::Doroti.Generated.Framework.Widgets.TraversalDirection.left] = -1L };

    public override void initState()
    {
        base.initState();
        _currentMonth = ((_MonthPicker__calendar_date_picker)this.widget).initialMonth;
        _pageController = new global::Doroti.Generated.Framework.Widgets.PageController(initialPage: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, this._currentMonth));
        _shortcutMap = new DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent> { [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowLeft)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.left)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowRight)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.right)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowDown)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.down)), [new global::Doroti.Generated.Framework.Widgets.SingleActivator(global::Doroti.Generated.Framework.Services.LogicalKeyboardKey.arrowUp)] = ((global::Doroti.Generated.Framework.Widgets.Intent)(object?)new global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent(global::Doroti.Generated.Framework.Widgets.TraversalDirection.up)) }.cast<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>();
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Widgets.NextFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.NextFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.NextFocusIntent>)this._handleGridNextFocus), [typeof(global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent>)this._handleGridPreviousFocus), [typeof(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)] = new global::Doroti.Generated.Framework.Widgets.CallbackAction<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>(onInvoke: (global::System.Action<global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent>)this._handleDirectionFocus) }.cast<Type, dynamic>();
        _dayGridFocus = new global::Doroti.Generated.Framework.Widgets.FocusNode(debugLabel: "Day Grid");
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
        setState(((global::System.Action)(() => {
DateTime monthDate__25217 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_MonthPicker__calendar_date_picker)this.widget).firstDate, monthPage);
if (!((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(this._currentMonth, monthDate__25217))
{
    _currentMonth = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getMonth(monthDate__25217.Year, monthDate__25217.Month);
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
        long daysInMonth__26424 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(month.Year, month.Month);
        if ((preferredDay <= daysInMonth__26424))
        {
            DateTime newFocus__26615 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(month.Year, month.Month, preferredDay);
            if (_isSelectable(newFocus__26615))
            {
                return newFocus__26615;
            }
        }
        for (var day__26880 = 1L; (day__26880 <= daysInMonth__26424); day__26880++)
        {
            DateTime newFocus__26939 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(month.Year, month.Month, day__26880);
            if (_isSelectable(newFocus__26939))
            {
                return newFocus__26939;
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleNextMonth()
    {
        if (!this._isDisplayingLastMonth)
        {
            DartRuntimePrimitives.Ignore(this._pageController.nextPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease));
        }
    }

    internal virtual void _handlePreviousMonth()
    {
        if (!this._isDisplayingFirstMonth)
        {
            DartRuntimePrimitives.Ignore(this._pageController.previousPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease));
        }
    }

    internal virtual void _showMonth(DateTime month, bool jump = false)
    {
        long monthPage__27613 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, month);
        if (jump)
        {
            this._pageController.jumpToPage(monthPage__27613);
        }
        else
        {
            DartRuntimePrimitives.Ignore(this._pageController.animateToPage(monthPage__27613, duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: global::Doroti.Generated.Framework.Animation.Curves.ease));
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
        setState(((global::System.Action)(() => {
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

    internal virtual void _handleGridNextFocus(global::Doroti.Generated.Framework.Widgets.NextFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(global::Doroti.Generated.Framework.Widgets.PreviousFocusIntent intent)
    {
        this._dayGridFocus.requestFocus();
        this._dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => (this._focusedDay is not null));
        setState(((global::System.Action)(() => {
DateTime? nextDate__29939 = _nextDateInDirection(DartRuntimePrimitives.RequireValue(this._focusedDay), ((global::Doroti.Generated.Framework.Widgets.DirectionalFocusIntent)intent).direction);
if ((nextDate__29939 is not null))
{
    DateTime nextDate__29939__value30014 = DartRuntimePrimitives.RequireValue(nextDate__29939);
    _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__29939__value30014);
    if (!((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(this._focusedDay, this._currentMonth))
    {
        _showMonth(DartRuntimePrimitives.RequireValue(this._focusedDay));
    }
}
})));
    }

    internal virtual long _dayDirectionOffset(global::Doroti.Generated.Framework.Widgets.TraversalDirection traversalDirection, TextDirection textDirection)
    {
        if ((object.Equals(textDirection, TextDirection.rtl)))
        {
            if ((object.Equals(traversalDirection, global::Doroti.Generated.Framework.Widgets.TraversalDirection.left)))
            {
                traversalDirection = global::Doroti.Generated.Framework.Widgets.TraversalDirection.right;
            }
            else
            {
                if ((object.Equals(traversalDirection, global::Doroti.Generated.Framework.Widgets.TraversalDirection.right)))
                {
                    traversalDirection = global::Doroti.Generated.Framework.Widgets.TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, global::Doroti.Generated.Framework.Widgets.TraversalDirection direction)
    {
        global::Doroti.Ui.TextDirection textDirection__31082 = Directionality.of(this.context);
        DateTime nextDate__31139 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addDaysToDate(date, _dayDirectionOffset(direction, textDirection__31082));
        while ((!nextDate__31139.isBefore(((_MonthPicker__calendar_date_picker)this.widget).firstDate) && !nextDate__31139.isAfter(((_MonthPicker__calendar_date_picker)this.widget).lastDate)))
        {
            if (_isSelectable(nextDate__31139))
            {
                return nextDate__31139;
            }
            nextDate__31139 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addDaysToDate(nextDate__31139, _dayDirectionOffset(direction, textDirection__31082));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return ((((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate is null ? true : ((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate.Invoke(date)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildItems(global::Doroti.Generated.Framework.Widgets.BuildContext context, long index)
    {
        DateTime month__31766 = ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.addMonthsToMonthDate(((_MonthPicker__calendar_date_picker)this.widget).firstDate, index);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _DayPicker__calendar_date_picker(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<DateTime>(month__31766), calendarDelegate: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate, selectedDate: ((_MonthPicker__calendar_date_picker)this.widget).selectedDate, currentDate: ((_MonthPicker__calendar_date_picker)this.widget).currentDate, onChanged: (global::System.Action<DateTime>)this._handleDateSelected, firstDate: ((_MonthPicker__calendar_date_picker)this.widget).firstDate, lastDate: ((_MonthPicker__calendar_date_picker)this.widget).lastDate, displayedMonth: month__31766, selectableDayPredicate: (global::System.Func<DateTime, bool>?)((_MonthPicker__calendar_date_picker)this.widget).selectableDayPredicate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? subHeaderForegroundColor__32311 = ((global::Doroti.Ui.Color?)(object?)(DatePickerTheme.of(context).subHeaderForegroundColor ?? DatePickerTheme.defaults(context).subHeaderForegroundColor));
        bool supportsAnnounce__32486 = (MediaQuery.maybeSupportsAnnounceOf(context) ?? false);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, explicitChildNodes: true, liveRegion: !supportsAnnounce__32486, accessibilityFocusBlockType: (!supportsAnnounce__32486 ? global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.blockNode : global::Doroti.Generated.Framework.Semantics.AccessibilityFocusBlockType.none), label: (!supportsAnnounce__32486 ? this._announcementText : null), child: new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Spacer()), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.chevron_left, semanticLabel: (this._isDisplayingFirstMonth ? ((MaterialLocalizations)this._localizations).previousMonthTooltip : null)), color: subHeaderForegroundColor__32311, tooltip: (this._isDisplayingFirstMonth ? null : ((MaterialLocalizations)this._localizations).previousMonthTooltip), onPressed: ((global::System.Action)(this._isDisplayingFirstMonth ? null : this._handlePreviousMonth)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Generated.Framework.Widgets.Icon(Icons.chevron_right, semanticLabel: (this._isDisplayingLastMonth ? ((MaterialLocalizations)this._localizations).nextMonthTooltip : null)), color: subHeaderForegroundColor__32311, tooltip: (this._isDisplayingLastMonth ? null : ((MaterialLocalizations)this._localizations).nextMonthTooltip), onPressed: ((global::System.Action)(this._isDisplayingLastMonth ? null : this._handleNextMonth)))) })))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.FocusableActionDetector(shortcuts: this._shortcutMap, actions: this._actionMap, focusNode: this._dayGridFocus, onFocusChange: (global::System.Action<bool>)this._handleGridFocusChange, child: new _FocusedDate__calendar_date_picker(calendarDelegate: ((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate, date: (((global::Doroti.Generated.Framework.Widgets.FocusNode)this._dayGridFocus).hasFocus ? this._focusedDay : null), child: new Material(type: MaterialType.transparency, child: global::Doroti.Generated.Framework.Widgets.PageView.CreateBuilder(key: this._pageViewKey, controller: this._pageController, itemBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildItems, itemCount: (((_MonthPicker__calendar_date_picker)this.widget).calendarDelegate.monthDelta(((_MonthPicker__calendar_date_picker)this.widget).firstDate, ((_MonthPicker__calendar_date_picker)this.widget).lastDate) + 1L), onPageChanged: (global::System.Action<long>)this._handleMonthPageChanged)))))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FocusedDate__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }

    internal _FocusedDate__calendar_date_picker(global::Doroti.Generated.Framework.Widgets.Widget child, CalendarDelegate<DateTime> calendarDelegate, DateTime? date = null) : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__calendar_date_picker)(object)oldWidget;
        return !this.calendarDelegate.isSameDay(this.date, ((_FocusedDate__calendar_date_picker)__oldWidget).date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime? maybeOf(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _FocusedDate__calendar_date_picker? focusedDate__36101 = ((_FocusedDate__calendar_date_picker?)(object?)context.dependOnInheritedWidgetOfExactType<_FocusedDate__calendar_date_picker>());
        return focusedDate__36101?.date;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _DayPicker__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual DateTime? selectedDate { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime displayedMonth { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _DayPicker__calendar_date_picker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime currentDate = default!, DateTime displayedMonth = default!, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, CalendarDelegate<DateTime> calendarDelegate = default!, global::System.Func<DateTime, bool>? selectableDayPredicate = null) : base(key: key)
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

internal class _DayPickerState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<_DayPicker__calendar_date_picker>
{
    internal virtual List<global::Doroti.Generated.Framework.Widgets.FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth__38135 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Year, ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Month);
        _dayFocusNodes = new List<global::Doroti.Generated.Framework.Widgets.FocusNode>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)daysInMonth__38135)), ((index) => new global::Doroti.Generated.Framework.Widgets.FocusNode(skipTraversal: true, debugLabel: $"Day {(index + 1L)}"))));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate__38602 = _FocusedDate__calendar_date_picker.maybeOf(this.context);
        if (((focusedDate__38602 is not null) && ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameMonth(((_DayPicker__calendar_date_picker)this.widget).displayedMonth, DartRuntimePrimitives.RequireValue(focusedDate__38602))))
        {
            DateTime focusedDate__38602__value38655 = DartRuntimePrimitives.RequireValue(focusedDate__38602);
            this._dayFocusNodes[(int)((DartRuntimePrimitives.RequireValue(focusedDate__38602__value38655).Day - 1L))].requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Generated.Framework.Widgets.FocusNode node__38886 in this._dayFocusNodes)
        {
            node__38886.dispose();
        }
        base.dispose();
    }

    internal virtual List<global::Doroti.Generated.Framework.Widgets.Widget> _dayHeaders(global::Doroti.Generated.Framework.Painting.TextStyle? headerStyle, MaterialLocalizations localizations)
    {
        var result__39631 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        for (long i__39672 = ((MaterialLocalizations)localizations).firstDayOfWeekIndex; (checked((long)(result__39631.Count)) < 7L); i__39672 = (((i__39672 + 1L)) % 7L))
        {
            string weekday__39823 = ((MaterialLocalizations)localizations).narrowWeekdays[(int)(i__39672)];
            result__39631.Add(new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Text(weekday__39823, style: headerStyle))));
        }
        return result__39631;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations localizations__40110 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        DatePickerThemeData datePickerTheme__40191 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__40268 = DatePickerTheme.defaults(context);
        global::Doroti.Generated.Framework.Painting.TextStyle? weekdayStyle__40335 = (datePickerTheme__40191.weekdayStyle ?? defaults__40268.weekdayStyle);
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__40428 = MediaQuery.orientationOf(context);
        var isLandscapeOrientation__40487 = (object.Equals(orientation__40428, global::Doroti.Generated.Framework.Widgets.Orientation.landscape));
        long year__40565 = ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Year;
        long month__40614 = ((_DayPicker__calendar_date_picker)this.widget).displayedMonth.Month;
        long daysInMonth__40666 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDaysInMonth(year__40565, month__40614);
        long dayOffset__40747 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.firstDayOffset(year__40565, month__40614, localizations__40110);
        List<global::Doroti.Generated.Framework.Widgets.Widget> dayItems__40851 = ((List<global::Doroti.Generated.Framework.Widgets.Widget>)(object?)_dayHeaders(weekdayStyle__40335, localizations__40110));
        long day__41009 = -dayOffset__40747;
        while ((day__41009 < daysInMonth__40666))
        {
            day__41009++;
            if ((day__41009 < 1L))
            {
                dayItems__40851.Add(global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
            }
            else
            {
                DateTime dayToBuild__41178 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.getDay(year__40565, month__40614, day__41009);
                bool isDisabled__41260 = ((dayToBuild__41178.isAfter(((_DayPicker__calendar_date_picker)this.widget).lastDate) || dayToBuild__41178.isBefore(((_DayPicker__calendar_date_picker)this.widget).firstDate)) || (((((_DayPicker__calendar_date_picker)this.widget).selectableDayPredicate is not null) && !((_DayPicker__calendar_date_picker)this.widget).selectableDayPredicate!(dayToBuild__41178))));
                bool isSelectedDay__41496 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameDay(((_DayPicker__calendar_date_picker)this.widget).selectedDate, dayToBuild__41178);
                bool isToday__41630 = ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate.isSameDay(((_DayPicker__calendar_date_picker)this.widget).currentDate, dayToBuild__41178);
                dayItems__40851.Add(new _Day__calendar_date_picker(dayToBuild__41178, key: new global::Doroti.Generated.Framework.Foundation.ValueKey<DateTime>(dayToBuild__41178), isDisabled: isDisabled__41260, isSelectedDay: isSelectedDay__41496, isToday: isToday__41630, onChanged: (global::System.Action<DateTime>)((_DayPicker__calendar_date_picker)this.widget).onChanged, focusNode: this._dayFocusNodes[(int)((day__41009 - 1L))], calendarDelegate: ((_DayPicker__calendar_date_picker)this.widget).calendarDelegate));
            }
        }
        double monthPickerHorizontalPadding__42127 = ((Theme.of(context).useMaterial3 && !isLandscapeOrientation__40487) ? Calendar_date_pickerLibrary._monthPickerHorizontalPaddingPortraitM3 : Calendar_date_pickerLibrary._monthPickerHorizontalPaddingOther);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: monthPickerHorizontalPadding__42127), child: MediaQuery.withClampedTextScaling(maxScaleFactor: (isLandscapeOrientation__40487 ? Calendar_date_pickerLibrary._kDayPickerGridLandscapeMaxScaleFactor : Calendar_date_pickerLibrary._kDayPickerGridPortraitMaxScaleFactor), child: global::Doroti.Generated.Framework.Widgets.GridView.CreateCustom(physics: new global::Doroti.Generated.Framework.Widgets.ClampingScrollPhysics(), gridDelegate: new _DayPickerGridDelegate__calendar_date_picker(context), childrenDelegate: new global::Doroti.Generated.Framework.Widgets.SliverChildListDelegate(dayItems__40851, addRepaintBoundaries: false)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _Day__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual bool isDisabled { get; private set; } = default!;
    public virtual bool isSelectedDay { get; private set; } = default!;
    public virtual bool isToday { get; private set; } = default!;
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FocusNode focusNode { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _Day__calendar_date_picker(DateTime day, global::Doroti.Generated.Framework.Foundation.Key? key = null, bool isDisabled = default!, bool isSelectedDay = default!, bool isToday = default!, global::System.Action<DateTime> onChanged = default!, global::Doroti.Generated.Framework.Widgets.FocusNode focusNode = default!, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
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

internal class _DayState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<_Day__calendar_date_picker>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DatePickerThemeData defaults__43639 = DatePickerTheme.defaults(context);
        DatePickerThemeData datePickerTheme__43715 = DatePickerTheme.of(context);
        global::Doroti.Generated.Framework.Painting.TextStyle? dayStyle__43783 = (datePickerTheme__43715.dayStyle ?? defaults__43639.dayStyle);
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme__43715) ?? getProperty(defaults__43639));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) => {
return getProperty(theme) is { } property ? property.resolve(states) : default;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        MaterialLocalizations localizations__44294 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        var semanticLabelSuffix__44355 = (((_Day__calendar_date_picker)this.widget).isToday ? $", {((MaterialLocalizations)localizations__44294).currentDateLabel}" : "");
        var states__44449 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection44458 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (((_Day__calendar_date_picker)this.widget).isDisabled) { __collection44458.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } if (((_Day__calendar_date_picker)this.widget).isSelectedDay) { __collection44458.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection44458; }))();
        this._statesController.value = states__44449;
        global::Doroti.Ui.Color? dayForegroundColor__44642 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (((_Day__calendar_date_picker)this.widget).isToday ? theme?.todayForegroundColor : theme?.dayForegroundColor)), states__44449));
        global::Doroti.Ui.Color? dayBackgroundColor__44840 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (((_Day__calendar_date_picker)this.widget).isToday ? theme?.todayBackgroundColor : theme?.dayBackgroundColor)), states__44449));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> dayOverlayColor__45059 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => theme?.dayOverlayColor?.resolve(states)))))));
        global::Doroti.Generated.Framework.Painting.OutlinedBorder dayShape__45284 = resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((theme) => theme?.dayShape), states__44449)!;
        bool hasCustomBorderColor__45413 = ((datePickerTheme__43715.todayBorder is not null) && (datePickerTheme__43715.todayBorder!.color.opacity != 0.0));
        global::Doroti.Generated.Framework.Painting.BorderSide todayBorderSide__45555 = (hasCustomBorderColor__45413 ? datePickerTheme__43715.todayBorder! : ((datePickerTheme__43715.todayBorder ?? defaults__43639.todayBorder!)).copyWith(color: dayForegroundColor__44642));
        var decoration__45770 = (((_Day__calendar_date_picker)this.widget).isToday ? new global::Doroti.Generated.Framework.Painting.ShapeDecoration(color: dayBackgroundColor__44840, shape: dayShape__45284.copyWith(side: todayBorderSide__45555)) : new global::Doroti.Generated.Framework.Painting.ShapeDecoration(color: dayBackgroundColor__44840, shape: dayShape__45284));
        global::Doroti.Generated.Framework.Widgets.Widget dayWidget__46020 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Ink(decoration: decoration__45770, child: new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Text(localizations__44294.formatDecimal(((_Day__calendar_date_picker)this.widget).day.Day), style: dayStyle__43783?.apply(color: dayForegroundColor__44642)))));
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__46413 = MediaQuery.orientationOf(context);
        if ((Theme.of(context).useMaterial3 && (object.Equals(orientation__46413, global::Doroti.Generated.Framework.Widgets.Orientation.portrait))))
        {
            dayWidget__46020 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll(4.0), child: dayWidget__46020));
        }
        dayWidget__46020 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Semantics(label: $"{localizations__44294.formatDecimal(((_Day__calendar_date_picker)this.widget).day.Day)}, {((_Day__calendar_date_picker)this.widget).calendarDelegate.formatFullDate(((_Day__calendar_date_picker)this.widget).day, localizations__44294)}{semanticLabelSuffix__44355}", button: true, selected: ((_Day__calendar_date_picker)this.widget).isSelectedDay, enabled: !((_Day__calendar_date_picker)this.widget).isDisabled, excludeSemantics: true, child: dayWidget__46020));
        if (!((_Day__calendar_date_picker)this.widget).isDisabled)
        {
            dayWidget__46020 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkResponse(focusNode: ((_Day__calendar_date_picker)this.widget).focusNode, onTap: (() => { this.widget.onChanged(((_Day__calendar_date_picker)this.widget).day); }), statesController: this._statesController, overlayColor: dayOverlayColor__45059, customBorder: dayShape__45284, containedInkWell: true, child: dayWidget__46020));
        }
        return dayWidget__46020;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._statesController.dispose();
        base.dispose();
    }

}

internal class _DayPickerGridDelegate__calendar_date_picker : global::Doroti.Generated.Framework.Rendering.SliverGridDelegate
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _DayPickerGridDelegate__calendar_date_picker(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints)
    {
        double textScaleFactor__48105 = (MediaQuery.textScalerOf(this.context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        global::Doroti.Generated.Framework.Widgets.Orientation orientation__48343 = MediaQuery.orientationOf(this.context);
        double dayPickerRowHeight__48409 = ((Theme.of(this.context).useMaterial3 && (object.Equals(orientation__48343, global::Doroti.Generated.Framework.Widgets.Orientation.portrait))) ? Calendar_date_pickerLibrary._dayPickerRowHeightM3 : Calendar_date_pickerLibrary._dayPickerRowHeightM2);
        double scaledRowHeight__48590 = ((textScaleFactor__48105 > 1.3) ? (((((textScaleFactor__48105 - 1L)) * 30L)) + dayPickerRowHeight__48409) : dayPickerRowHeight__48409);
        long columnCount__48734 = 7L;
        double tileWidth__48787 = (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent / columnCount__48734);
        double tileHeight__48859 = Math.Min(scaledRowHeight__48590, (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).viewportMainAxisExtent / ((Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L))));
        return ((global::Doroti.Generated.Framework.Rendering.SliverGridLayout)(object?)new global::Doroti.Generated.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth__48787, childMainAxisExtent: tileHeight__48859, crossAxisCount: columnCount__48734, crossAxisStride: tileWidth__48787, mainAxisStride: tileHeight__48859, reverseCrossAxis: global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public class YearPicker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime? selectedDate { get; private set; }
    public virtual global::System.Action<DateTime> onChanged { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public YearPicker(global::Doroti.Generated.Framework.Foundation.Key? key = null, DateTime? currentDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? initialDate = null, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
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

internal class _YearPickerState__calendar_date_picker : global::Doroti.Generated.Framework.Widgets.State<YearPicker>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollController? _scrollController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetStatesController _statesController { get; private set; } = new global::Doroti.Generated.Framework.Widgets.WidgetStatesController();
    public const long minYears = 18L;

    public override void initState()
    {
        base.initState();
        _scrollController = new global::Doroti.Generated.Framework.Widgets.ScrollController(initialScrollOffset: _scrollOffsetForYear((((YearPicker)this.widget).selectedDate ?? ((YearPicker)this.widget).firstDate)));
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
        long initialYearIndex__52419 = (date.Year - ((YearPicker)this.widget).firstDate.Year);
        long initialYearRow__52487 = (checked((long)(initialYearIndex__52419 / Calendar_date_pickerLibrary._yearPickerColumnCount)));
        long centeredYearRow__52628 = (initialYearRow__52487 - 2L);
        return ((this._itemCount < minYears) ? 0 : (centeredYearRow__52628 * Calendar_date_pickerLibrary._yearPickerRowHeight));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildYearItem(global::Doroti.Generated.Framework.Widgets.BuildContext context, long index)
    {
        DatePickerThemeData datePickerTheme__52839 = DatePickerTheme.of(context);
        DatePickerThemeData defaults__52916 = DatePickerTheme.defaults(context);
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return (getProperty(datePickerTheme__52839) ?? getProperty(defaults__52916));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue(((theme) => {
return getProperty(theme) is { } property ? property.resolve(states) : default;
throw new InvalidOperationException("Dart closure completed without a value.");
}));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double textScaleFactor__53401 = (MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        long offset__53620 = ((this._itemCount < minYears) ? (checked((long)(((minYears - this._itemCount)) / 2L))) : 0L);
        long year__53701 = ((((YearPicker)this.widget).firstDate.Year + index) - offset__53620);
        var isSelected__53758 = (year__53701 == ((YearPicker)this.widget).selectedDate?.Year);
        var isCurrentYear__53816 = (year__53701 == ((YearPicker)this.widget).currentDate.Year);
        bool isDisabled__53880 = ((year__53701 < ((YearPicker)this.widget).firstDate.Year) || (year__53701 > ((YearPicker)this.widget).lastDate.Year));
        double decorationHeight__53971 = (36.0 * textScaleFactor__53401);
        double decorationWidth__54031 = (72.0 * textScaleFactor__53401);
        var states__54084 = ((Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>>)(() => { var __collection54093 = new HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>(); if (isDisabled__53880) { __collection54093.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.disabled); } if (isSelected__53758) { __collection54093.Add(global::Doroti.Generated.Framework.Widgets.WidgetState.selected); } return __collection54093; }))();
        global::Doroti.Ui.Color? textColor__54221 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (isCurrentYear__53816 ? theme?.todayForegroundColor : theme?.yearForegroundColor)), states__54084));
        global::Doroti.Ui.Color? background__54410 = ((global::Doroti.Ui.Color?)(object?)resolve<global::Doroti.Ui.Color?>(((theme) => (isCurrentYear__53816 ? theme?.todayBackgroundColor : theme?.yearBackgroundColor)), states__54084));
        global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColor__54621 = ((global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>(((global::System.Func<HashSet<global::Doroti.Generated.Framework.Widgets.WidgetState>, global::Doroti.Ui.Color?>)((states) => effectiveValue(((theme) => theme?.yearOverlayColor?.resolve(states)))))));
        global::Doroti.Generated.Framework.Painting.OutlinedBorder yearShape__54845 = resolve<global::Doroti.Generated.Framework.Painting.OutlinedBorder?>(((theme) => theme?.yearShape), states__54084)!;
        global::Doroti.Generated.Framework.Painting.BorderSide? borderSide__54978 = default!;
        if (isCurrentYear__53816)
        {
            borderSide__54978 = (datePickerTheme__52839.todayBorder ?? defaults__52916.todayBorder);
            if ((borderSide__54978 is not null))
            {
                borderSide__54978 = borderSide__54978.copyWith(color: textColor__54221);
            }
        }
        var decoration__55203 = new global::Doroti.Generated.Framework.Painting.ShapeDecoration(color: background__54410, shape: yearShape__54845.copyWith(side: borderSide__54978));
        global::Doroti.Generated.Framework.Painting.TextStyle? itemStyle__55338 = ((global::Doroti.Generated.Framework.Painting.TextStyle?)(object?)((datePickerTheme__52839.yearStyle ?? defaults__52916.yearStyle))?.apply(color: textColor__54221));
        MaterialLocalizations localizations__55471 = ((MaterialLocalizations)(object?)MaterialLocalizations.of(context));
        global::Doroti.Generated.Framework.Widgets.Widget yearItem__55533 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Container(decoration: decoration__55203, height: decorationHeight__53971, width: decorationWidth__54031, alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, child: new global::Doroti.Generated.Framework.Widgets.Semantics(selected: isSelected__53758, enabled: !isDisabled__53880, button: true, child: new global::Doroti.Generated.Framework.Widgets.Text(((YearPicker)this.widget).calendarDelegate.formatYear(year__53701, localizations__55471), style: itemStyle__55338)))));
        if (!isDisabled__53880)
        {
            DateTime date__55989 = ((YearPicker)this.widget).calendarDelegate.getMonth(year__53701, (((YearPicker)this.widget).selectedDate?.Month ?? 1L));
            if (date__55989.isBefore(((YearPicker)this.widget).calendarDelegate.getMonth(((YearPicker)this.widget).firstDate.Year, ((YearPicker)this.widget).firstDate.Month)))
            {
                DartRuntimePrimitives.Assert(() => (date__55989.Year == ((YearPicker)this.widget).firstDate.Year));
                date__55989 = ((YearPicker)this.widget).calendarDelegate.getMonth(year__53701, ((YearPicker)this.widget).firstDate.Month);
            }
            else
            {
                if (date__55989.isAfter(((YearPicker)this.widget).lastDate))
                {
                    DartRuntimePrimitives.Assert(() => (date__55989.Year == ((YearPicker)this.widget).lastDate.Year));
                    date__55989 = ((YearPicker)this.widget).calendarDelegate.getMonth(year__53701, ((YearPicker)this.widget).lastDate.Month);
                }
            }
            this._statesController.value = states__54084;
            yearItem__55533 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new InkWell(key: new global::Doroti.Generated.Framework.Foundation.ValueKey<long>(year__53701), onTap: (() => { this.widget.onChanged(date__55989); }), statesController: this._statesController, overlayColor: overlayColor__54621, child: yearItem__55533));
        }
        return yearItem__55533;
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Column(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Divider()), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new Material(type: MaterialType.transparency, child: global::Doroti.Generated.Framework.Widgets.GridView.CreateBuilder(controller: this._scrollController, dragStartBehavior: ((YearPicker)this.widget).dragStartBehavior, gridDelegate: new _YearPickerGridDelegate__calendar_date_picker(context), itemBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildYearItem, itemCount: Math.Max(this._itemCount, minYears), padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: Calendar_date_pickerLibrary._yearPickerPadding))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Divider()) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _YearPickerGridDelegate__calendar_date_picker : global::Doroti.Generated.Framework.Rendering.SliverGridDelegate
{
    public virtual global::Doroti.Generated.Framework.Widgets.BuildContext context { get; private set; } = default!;

    internal _YearPickerGridDelegate__calendar_date_picker(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this.context = context;
    }

    public virtual global::Doroti.Generated.Framework.Rendering.SliverGridLayout getLayout(global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints)
    {
        double textScaleFactor__58029 = (MediaQuery.textScalerOf(this.context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale);
        long scaledYearPickerColumnCount__58181 = ((textScaleFactor__58029 > 1.65) ? (Calendar_date_pickerLibrary._yearPickerColumnCount - 1L) : Calendar_date_pickerLibrary._yearPickerColumnCount);
        double tileWidth__58322 = Math.Max((((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisExtent - (((scaledYearPickerColumnCount__58181 - 1L)) * Calendar_date_pickerLibrary._yearPickerRowSpacing))) / scaledYearPickerColumnCount__58181), 0.0);
        double scaledYearPickerRowHeight__58516 = ((textScaleFactor__58029 > 1L) ? (Calendar_date_pickerLibrary._yearPickerRowHeight + ((((textScaleFactor__58029 - 1L)) * 9L))) : Calendar_date_pickerLibrary._yearPickerRowHeight);
        return ((global::Doroti.Generated.Framework.Rendering.SliverGridLayout)(object?)new global::Doroti.Generated.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth__58322, childMainAxisExtent: scaledYearPickerRowHeight__58516, crossAxisCount: scaledYearPickerColumnCount__58181, crossAxisStride: (tileWidth__58322 + Calendar_date_pickerLibrary._yearPickerRowSpacing), mainAxisStride: scaledYearPickerRowHeight__58516, reverseCrossAxis: global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints).crossAxisDirection)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static void _reportAnnouncementError(object exception, global::System.Diagnostics.StackTrace stack)
    {
        FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
    }
}
