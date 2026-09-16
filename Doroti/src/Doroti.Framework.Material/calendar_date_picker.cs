// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/calendar_date_picker.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Calendar_date_pickerLibrary
{
    internal static Duration _monthScrollDuration = Duration.Create(milliseconds: 200L);
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _dayPickerRowHeightLandscape = 42.0;
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
    internal static double _maxDayPickerHeightLandscape = _dayPickerRowHeightLandscape * (_maxDayPickerRowCount + 1L);
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _maxDayPickerHeightM3 = _dayPickerRowHeightM3 * (_maxDayPickerRowCount + 1L);
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
        this.initialDate = (initialDate is null) ? null : this.calendarDelegate.dateOnly(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(initialDate)));
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
        this.currentDate = this.calendarDelegate.dateOnly(currentDate ?? this.calendarDelegate.now());
        DartRuntimePrimitives.Assert(() => !this.lastDate.isBefore(this.firstDate), () => (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => (this.initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate), () => (object?)$"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}.");
        DartRuntimePrimitives.Assert(() => (this.initialDate is null) || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate), () => (object?)$"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}.");
        DartRuntimePrimitives.Assert(() => (this.selectableDayPredicate is null) || (this.initialDate is null) || this.selectableDayPredicate!(DartRuntimePrimitives.RequireValue(this.initialDate)), () => (object?)$"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate.");
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
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _monthPickerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _yearPickerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _mode = widget.initialCalendarMode;
        DateTime currentDisplayedDate = widget.initialDate ?? widget.currentDate;
        _currentDisplayedMonthDate = widget.calendarDelegate.getMonth(currentDisplayedDate.Year, currentDisplayedDate.Month);
        if (widget.initialDate is not null)
        {
            _selectedDate = widget.initialDate;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        _localizations = MaterialLocalizations.of(context);
        _textDirection = Directionality.of(context);
        if (!_announcedInitialDate && (widget.initialDate is not null))
        {
            DartRuntimePrimitives.Assert(() => _selectedDate is not null);
            _announcedInitialDate = true;
            bool isToday = widget.calendarDelegate.isSameDay(widget.currentDate, _selectedDate);
            var semanticLabelSuffix = isToday ? $", {_localizations.currentDateLabel}" : "";
            _announce($"{_localizations.formatFullDate(DartRuntimePrimitives.RequireValue(_selectedDate))}{semanticLabelSuffix}");
        }
    }

    internal virtual void _announce(string message)
    {
        if (MediaQuery.maybeSupportsAnnounceOf(context) ?? false)
        {
            DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(context), message, Directionality.of(context)).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
        }
        else
        {
            _announcementText = message;
        }
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
                    DartRuntimePrimitives.Ignore(HapticFeedback.vibrate());
                    break;
                }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
                {
                    break;
                }
        }
    }

    internal virtual void _handleModeChanged(DatePickerMode mode)
    {
        _vibrate();
        setState(() =>
        {
            _mode = mode;
            if (_selectedDate is DateTime selected)
            {
                string message = mode switch { DatePickerMode.day => widget.calendarDelegate.formatMonthYear(selected, _localizations), DatePickerMode.year => widget.calendarDelegate.formatYear(selected.Year, _localizations), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                _announce(message);
            }
        });
    }

    internal virtual void _handleMonthChanged(DateTime date)
    {
        setState(() =>
        {
            if ((_currentDisplayedMonthDate.Year != date.Year) || (_currentDisplayedMonthDate.Month != date.Month))
            {
                _currentDisplayedMonthDate = widget.calendarDelegate.getMonth(date.Year, date.Month);
                widget.onDisplayedMonthChanged?.Invoke(_currentDisplayedMonthDate);
            }
        });
    }

    internal virtual void _handleYearChanged(DateTime value)
    {
        _vibrate();
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(value.Year, value.Month);
        long preferredDay = Math.Min(_selectedDate?.Day ?? 1L, daysInMonth);
        value = widget.calendarDelegate.getDay(value.Year, value.Month, preferredDay);
        if (value.isBefore(widget.firstDate))
        {
            value = widget.firstDate;
        }
        else
        {
            if (value.isAfter(widget.lastDate))
            {
                value = widget.lastDate;
            }
        }
        setState(() =>
        {
            _mode = DatePickerMode.day;
            _handleMonthChanged(value);
            if (_isSelectable(value))
            {
                _selectedDate = value;
                widget.onDateChanged(DartRuntimePrimitives.RequireValue(_selectedDate));
            }
        });
    }

    internal virtual void _handleDayChanged(DateTime value)
    {
        _vibrate();
        setState(() =>
        {
            _selectedDate = value;
            widget.onDateChanged(DartRuntimePrimitives.RequireValue(_selectedDate));
            switch (Theme.of(context).platform)
            {
                case TargetPlatform.linux:
                case TargetPlatform.macOS:
                case TargetPlatform.windows:
                    {
                        bool isToday = widget.calendarDelegate.isSameDay(widget.currentDate, _selectedDate);
                        var semanticLabelSuffix = isToday ? $", {_localizations.currentDateLabel}" : "";
                        DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(context), $"{_localizations.selectedDateLabel} {widget.calendarDelegate.formatFullDate(DartRuntimePrimitives.RequireValue(_selectedDate), _localizations)}{semanticLabelSuffix}", _textDirection).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
                        break;
                    }
                case TargetPlatform.android:
                case TargetPlatform.iOS:
                case TargetPlatform.fuchsia:
                    {
                        break;
                    }
            }
        });
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return widget.selectableDayPredicate is null ? true : widget.selectableDayPredicate.Invoke(date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildPicker()
    {
        switch (_mode)
        {
            case DatePickerMode.day:
                {
                    return new _MonthPicker__calendar_date_picker(key: _monthPickerKey, calendarDelegate: widget.calendarDelegate, initialMonth: _currentDisplayedMonthDate, currentDate: widget.currentDate, firstDate: widget.firstDate, lastDate: widget.lastDate, selectedDate: _selectedDate, onChanged: _handleDayChanged, onDisplayedMonthChanged: _handleMonthChanged, selectableDayPredicate: widget.selectableDayPredicate);
                }
            case DatePickerMode.year:
                {
                    return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: Calendar_date_pickerLibrary._subHeaderHeight), child: new YearPicker(key: _yearPickerKey, calendarDelegate: widget.calendarDelegate, currentDate: widget.currentDate, firstDate: widget.firstDate, lastDate: widget.lastDate, selectedDate: _currentDisplayedMonthDate, onChanged: _handleYearChanged));
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
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        double textScaleFactor = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale;
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        double maxDayPickerHeight = Equals(orientation, Orientation.portrait) ? Calendar_date_pickerLibrary._maxDayPickerHeightM3 : Calendar_date_pickerLibrary._maxDayPickerHeightLandscape;
        double scaledMaxDayPickerHeight = (textScaleFactor > 1.3) ? (maxDayPickerHeight + (Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L) * ((textScaleFactor - 1L) * 8L)) : maxDayPickerHeight;
        var picker = new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight + scaledMaxDayPickerHeight, child: _buildPicker());
        return new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection15772 = new List<global::Doroti.Framework.Widgets.Widget>(); if (MediaQuery.maybeSupportsAnnounceOf(context) ?? false) { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(picker)); } else { __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(container: true, liveRegion: true, accessibilityFocusBlockType: AccessibilityFocusBlockType.blockNode, label: _announcementText, child: picker))); } __collection15772.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(MediaQuery.withClampedTextScaling(maxScaleFactor: Calendar_date_pickerLibrary._kModeToggleButtonMaxScaleFactor, child: new _DatePickerModeToggleButton__calendar_date_picker(mode: _mode, title: widget.calendarDelegate.formatMonthYear(_currentDisplayedMonthDate, _localizations), onTitlePressed: () => { _handleModeChanged(_mode switch { DatePickerMode.day => DatePickerMode.year, DatePickerMode.year => DatePickerMode.day, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }); })))); return __collection15772; }))());
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
        _controller = new global::Doroti.Framework.Animation.AnimationController(value: Equals(widget.mode, DatePickerMode.year) ? 0.5 : 0, upperBound: 0.5, duration: Duration.Create(milliseconds: 200L), vsync: this);
    }

    public override void didUpdateWidget(_DatePickerModeToggleButton__calendar_date_picker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (Equals(oldWidget.mode, widget.mode))
        {
            return;
        }
        if (Equals(widget.mode, DatePickerMode.year))
        {
            _controller.forward();
        }
        else
        {
            _controller.reverse();
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Painting.TextStyle? buttonTextStyle = datePickerTheme.toggleButtonTextStyle ?? defaultsLocal.toggleButtonTextStyle;
        global::Doroti.Ui.Color? subHeaderForegroundColorLocal = datePickerTheme.subHeaderForegroundColor ?? defaultsLocal.subHeaderForegroundColor;
        global::Doroti.Ui.Color? buttonTextColor = (datePickerTheme.toggleButtonTextStyle?.color ?? datePickerTheme.subHeaderForegroundColor) ?? defaultsLocal.toggleButtonTextStyle?.color;
        return new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Framework.Widgets.Row(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection19110 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Semantics(label: MaterialLocalizations.of(context).selectYearSemanticsLabel, button: true, container: true, child: new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new InkWell(onTap: widget.onTitlePressed, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 8), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: new global::Doroti.Framework.Widgets.Text(widget.title, overflow: TextOverflow.ellipsis, style: buttonTextStyle?.apply(color: buttonTextColor)))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.RotationTransition(turns: _controller, child: new global::Doroti.Framework.Widgets.Icon(Icons.arrow_drop_down, color: subHeaderForegroundColorLocal))) })))))))); if (Equals(widget.mode, DatePickerMode.day)) { __collection19110.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: Calendar_date_pickerLibrary._monthNavButtonsWidth))); } return __collection19110; }))())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
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
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
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
        System.Diagnostics.Debug.Assert((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate));
        System.Diagnostics.Debug.Assert((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MonthPickerState__calendar_date_picker());
}

internal class _MonthPickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_MonthPicker__calendar_date_picker>
{
    internal virtual global::Doroti.Framework.Widgets.GlobalKey<IState> _pageViewKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DateTime _currentMonth { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.PageController _pageController { get; set; } = default!;
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent>? _shortcutMap { get; set; } = default;
    internal virtual DartMap<Type, dynamic>? _actionMap { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> _directionOffset = new DartMap<global::Doroti.Framework.Widgets.TraversalDirection, long> { [TraversalDirection.up] = -7L, [TraversalDirection.right] = 1L, [TraversalDirection.down] = 7L, [TraversalDirection.left] = -1L };

    public override void initState()
    {
        base.initState();
        _currentMonth = widget.initialMonth;
        _pageController = new global::Doroti.Framework.Widgets.PageController(initialPage: widget.calendarDelegate.monthDelta(widget.firstDate, _currentMonth));
        _shortcutMap = new DartMap<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent> { [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowLeft)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.left), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowRight)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.right), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowDown)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.down), [new global::Doroti.Framework.Widgets.SingleActivator(LogicalKeyboardKey.arrowUp)] = new global::Doroti.Framework.Widgets.DirectionalFocusIntent(TraversalDirection.up) }.cast<global::Doroti.Framework.Widgets.ShortcutActivator, global::Doroti.Framework.Widgets.Intent>();
        _actionMap = new DartMap<Type, dynamic> { [typeof(global::Doroti.Framework.Widgets.NextFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.NextFocusIntent>(onInvoke: _handleGridNextFocus), [typeof(global::Doroti.Framework.Widgets.PreviousFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.PreviousFocusIntent>(onInvoke: _handleGridPreviousFocus), [typeof(global::Doroti.Framework.Widgets.DirectionalFocusIntent)] = new global::Doroti.Framework.Widgets.CallbackAction<global::Doroti.Framework.Widgets.DirectionalFocusIntent>(onInvoke: _handleDirectionFocus) }.cast<Type, dynamic>();
        _dayGridFocus = new global::Doroti.Framework.Widgets.FocusNode(debugLabel: "Day Grid");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _localizations = MaterialLocalizations.of(context);
    }

    public override void dispose()
    {
        _pageController.dispose();
        _dayGridFocus.dispose();
        base.dispose();
    }

    internal virtual void _handleDateSelected(DateTime selectedDate)
    {
        _focusedDay = selectedDate;
        widget.onChanged(selectedDate);
    }

    internal virtual void _announce(string message)
    {
        if (MediaQuery.maybeSupportsAnnounceOf(context) ?? false)
        {
            DartRuntimePrimitives.Ignore(SemanticsService.sendAnnouncement(View.of(context), message, Directionality.of(context)).catchError(Calendar_date_pickerLibrary._reportAnnouncementError));
        }
        else
        {
            _announcementText = message;
        }
    }

    internal virtual void _handleMonthPageChanged(long monthPage)
    {
        setState(() =>
        {
            DateTime monthDate = widget.calendarDelegate.addMonthsToMonthDate(widget.firstDate, monthPage);
            if (!widget.calendarDelegate.isSameMonth(_currentMonth, monthDate))
            {
                _currentMonth = widget.calendarDelegate.getMonth(monthDate.Year, monthDate.Month);
                widget.onDisplayedMonthChanged(_currentMonth);
                if ((_focusedDay is not null) && !widget.calendarDelegate.isSameMonth(_focusedDay, _currentMonth))
                {
                    _focusedDay = _focusableDayForMonth(_currentMonth, DartRuntimePrimitives.RequireValue(_focusedDay).Day);
                }
                _announce(widget.calendarDelegate.formatMonthYear(_currentMonth, _localizations));
            }
        });
    }

    internal virtual DateTime? _focusableDayForMonth(DateTime month, long preferredDay)
    {
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(month.Year, month.Month);
        if (preferredDay <= daysInMonth)
        {
            DateTime newFocus = widget.calendarDelegate.getDay(month.Year, month.Month, preferredDay);
            if (_isSelectable(newFocus))
            {
                return newFocus;
            }
        }
        for (var day = 1L; day <= daysInMonth; day++)
        {
            DateTime newFocusLocal = widget.calendarDelegate.getDay(month.Year, month.Month, day);
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
        if (!_isDisplayingLastMonth)
        {
            DartRuntimePrimitives.Ignore(_pageController.nextPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: Curves.ease));
        }
    }

    internal virtual void _handlePreviousMonth()
    {
        if (!_isDisplayingFirstMonth)
        {
            DartRuntimePrimitives.Ignore(_pageController.previousPage(duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: Curves.ease));
        }
    }

    internal virtual void _showMonth(DateTime month, bool jump = false)
    {
        long monthPage = widget.calendarDelegate.monthDelta(widget.firstDate, month);
        if (jump)
        {
            _pageController.jumpToPage(monthPage);
        }
        else
        {
            DartRuntimePrimitives.Ignore(_pageController.animateToPage(monthPage, duration: Calendar_date_pickerLibrary._monthScrollDuration, curve: Curves.ease));
        }
    }

    internal virtual bool _isDisplayingFirstMonth
    {
        get
        {
            return !_currentMonth.isAfter(widget.calendarDelegate.getMonth(widget.firstDate.Year, widget.firstDate.Month));
        }
    }
    internal virtual bool _isDisplayingLastMonth
    {
        get
        {
            return !_currentMonth.isBefore(widget.calendarDelegate.getMonth(widget.lastDate.Year, widget.lastDate.Month));
        }
    }
    internal virtual void _handleGridFocusChange(bool focused)
    {
        setState(() =>
        {
            if (focused && (_focusedDay is null))
            {
                if (widget.calendarDelegate.isSameMonth(widget.selectedDate, _currentMonth))
                {
                    _focusedDay = widget.selectedDate;
                }
                else
                {
                    if (widget.calendarDelegate.isSameMonth(widget.currentDate, _currentMonth))
                    {
                        _focusedDay = _focusableDayForMonth(_currentMonth, widget.currentDate.Day);
                    }
                    else
                    {
                        _focusedDay = _focusableDayForMonth(_currentMonth, 1L);
                    }
                }
            }
        });
    }

    internal virtual void _handleGridNextFocus(global::Doroti.Framework.Widgets.NextFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(global::Doroti.Framework.Widgets.PreviousFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(global::Doroti.Framework.Widgets.DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => _focusedDay is not null);
        setState(() =>
        {
            DateTime? nextDate = _nextDateInDirection(DartRuntimePrimitives.RequireValue(_focusedDay), intent.direction);
            if (nextDate is not null)
            {
                DateTime nextDate__29939__value30014 = DartRuntimePrimitives.RequireValue(nextDate);
                _focusedDay = DartRuntimePrimitives.RequireValue(nextDate__29939__value30014);
                if (!widget.calendarDelegate.isSameMonth(_focusedDay, _currentMonth))
                {
                    _showMonth(DartRuntimePrimitives.RequireValue(_focusedDay));
                }
            }
        });
    }

    internal virtual long _dayDirectionOffset(global::Doroti.Framework.Widgets.TraversalDirection traversalDirection, TextDirection textDirection)
    {
        if (Equals(textDirection, TextDirection.rtl))
        {
            if (Equals(traversalDirection, TraversalDirection.left))
            {
                traversalDirection = TraversalDirection.right;
            }
            else
            {
                if (Equals(traversalDirection, TraversalDirection.right))
                {
                    traversalDirection = TraversalDirection.left;
                }
            }
        }
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, global::Doroti.Framework.Widgets.TraversalDirection direction)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        DateTime nextDate = widget.calendarDelegate.addDaysToDate(date, _dayDirectionOffset(direction, textDirection));
        while (!nextDate.isBefore(widget.firstDate) && !nextDate.isAfter(widget.lastDate))
        {
            if (_isSelectable(nextDate))
            {
                return nextDate;
            }
            nextDate = widget.calendarDelegate.addDaysToDate(nextDate, _dayDirectionOffset(direction, textDirection));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return widget.selectableDayPredicate is null ? true : widget.selectableDayPredicate.Invoke(date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildItems(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        DateTime month = widget.calendarDelegate.addMonthsToMonthDate(widget.firstDate, index);
        return new _DayPicker__calendar_date_picker(key: new global::Doroti.Framework.Foundation.ValueKey<DateTime>(month), calendarDelegate: widget.calendarDelegate, selectedDate: widget.selectedDate, currentDate: widget.currentDate, onChanged: _handleDateSelected, firstDate: widget.firstDate, lastDate: widget.lastDate, displayedMonth: month, selectableDayPredicate: widget.selectableDayPredicate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? subHeaderForegroundColorLocal = DatePickerTheme.of(context).subHeaderForegroundColor ?? DatePickerTheme.defaults(context).subHeaderForegroundColor;
        bool supportsAnnounce = MediaQuery.maybeSupportsAnnounceOf(context) ?? false;
        return new global::Doroti.Framework.Widgets.Semantics(container: true, explicitChildNodes: true, liveRegion: !supportsAnnounce, accessibilityFocusBlockType: !supportsAnnounce ? AccessibilityFocusBlockType.blockNode : AccessibilityFocusBlockType.none, label: !supportsAnnounce ? _announcementText : null, child: new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: Calendar_date_pickerLibrary._subHeaderHeight, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 16, end: 4), child: new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Spacer()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.chevron_left, semanticLabel: _isDisplayingFirstMonth ? _localizations.previousMonthTooltip : null), color: subHeaderForegroundColorLocal, tooltip: _isDisplayingFirstMonth ? null : _localizations.previousMonthTooltip, onPressed: _isDisplayingFirstMonth ? null : _handlePreviousMonth)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new IconButton(icon: new global::Doroti.Framework.Widgets.Icon(Icons.chevron_right, semanticLabel: _isDisplayingLastMonth ? _localizations.nextMonthTooltip : null), color: subHeaderForegroundColorLocal, tooltip: _isDisplayingLastMonth ? null : _localizations.nextMonthTooltip, onPressed: _isDisplayingLastMonth ? null : _handleNextMonth)) })))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.FocusableActionDetector(shortcuts: _shortcutMap, actions: _actionMap, focusNode: _dayGridFocus, onFocusChange: _handleGridFocusChange, child: new _FocusedDate__calendar_date_picker(calendarDelegate: widget.calendarDelegate, date: _dayGridFocus.hasFocus ? _focusedDay : null, child: new Material(type: MaterialType.transparency, child: PageView.CreateBuilder(key: _pageViewKey, controller: _pageController, itemBuilder: _buildItems, itemCount: widget.calendarDelegate.monthDelta(widget.firstDate, widget.lastDate) + 1L, onPageChanged: _handleMonthPageChanged)))))) }));
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
        var __oldWidget = (_FocusedDate__calendar_date_picker)oldWidget;
        return !calendarDelegate.isSameDay(date, __oldWidget.date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime? maybeOf(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _FocusedDate__calendar_date_picker? focusedDate = context.dependOnInheritedWidgetOfExactType<_FocusedDate__calendar_date_picker>();
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
        System.Diagnostics.Debug.Assert((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate));
        System.Diagnostics.Debug.Assert((selectedDate is null) || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DayPickerState__calendar_date_picker());
}

internal class _DayPickerState__calendar_date_picker : global::Doroti.Framework.Widgets.State<_DayPicker__calendar_date_picker>
{
    internal virtual List<global::Doroti.Framework.Widgets.FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(widget.displayedMonth.Year, widget.displayedMonth.Month);
        _dayFocusNodes = new List<global::Doroti.Framework.Widgets.FocusNode>(Enumerable.Select(Enumerable.Range(0, checked((int)daysInMonth)), (index) => new global::Doroti.Framework.Widgets.FocusNode(skipTraversal: true, debugLabel: $"Day {index + 1L}")));
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate = _FocusedDate__calendar_date_picker.maybeOf(context);
        if ((focusedDate is not null) && widget.calendarDelegate.isSameMonth(widget.displayedMonth, DartRuntimePrimitives.RequireValue(focusedDate)))
        {
            DateTime focusedDate__38602__value38655 = DartRuntimePrimitives.RequireValue(focusedDate);
            _dayFocusNodes[(int)(DartRuntimePrimitives.RequireValue(focusedDate__38602__value38655).Day - 1L)].requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (global::Doroti.Framework.Widgets.FocusNode node in _dayFocusNodes)
        {
            node.dispose();
        }
        base.dispose();
    }

    internal virtual List<global::Doroti.Framework.Widgets.Widget> _dayHeaders(global::Doroti.Framework.Painting.TextStyle? headerStyle, MaterialLocalizations localizations)
    {
        var result = new List<global::Doroti.Framework.Widgets.Widget>();
        for (long i = localizations.firstDayOfWeekIndex; checked(result.Count) < 7L; i = (i + 1L) % 7L)
        {
            string weekday = localizations.narrowWeekdays[(int)i];
            result.Add(new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(weekday, style: headerStyle))));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        global::Doroti.Framework.Painting.TextStyle? weekdayStyleLocal = datePickerTheme.weekdayStyle ?? defaultsLocal.weekdayStyle;
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        var isLandscapeOrientation = Equals(orientation, Orientation.landscape);
        long year = widget.displayedMonth.Year;
        long month = widget.displayedMonth.Month;
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(year, month);
        long dayOffset = widget.calendarDelegate.firstDayOffset(year, month, localizations);
        List<global::Doroti.Framework.Widgets.Widget> dayItems = _dayHeaders(weekdayStyleLocal, localizations);
        long day = -dayOffset;
        while (day < daysInMonth)
        {
            day++;
            if (day < 1L)
            {
                dayItems.Add(SizedBox.CreateShrink());
            }
            else
            {
                DateTime dayToBuild = widget.calendarDelegate.getDay(year, month, day);
                bool isDisabledLocal = dayToBuild.isAfter(widget.lastDate) || dayToBuild.isBefore(widget.firstDate) || (widget.selectableDayPredicate is not null) && !widget.selectableDayPredicate!(dayToBuild);
                bool isSelectedDayLocal = widget.calendarDelegate.isSameDay(widget.selectedDate, dayToBuild);
                bool isTodayLocal = widget.calendarDelegate.isSameDay(widget.currentDate, dayToBuild);
                dayItems.Add(new _Day__calendar_date_picker(dayToBuild, key: new global::Doroti.Framework.Foundation.ValueKey<DateTime>(dayToBuild), isDisabled: isDisabledLocal, isSelectedDay: isSelectedDayLocal, isToday: isTodayLocal, onChanged: widget.onChanged, focusNode: _dayFocusNodes[(int)(day - 1L)], calendarDelegate: widget.calendarDelegate));
            }
        }
        double monthPickerHorizontalPadding = (!isLandscapeOrientation) ? Calendar_date_pickerLibrary._monthPickerHorizontalPaddingPortraitM3 : Calendar_date_pickerLibrary._monthPickerHorizontalPaddingOther;
        return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateSymmetric(horizontal: monthPickerHorizontalPadding), child: MediaQuery.withClampedTextScaling(maxScaleFactor: isLandscapeOrientation ? Calendar_date_pickerLibrary._kDayPickerGridLandscapeMaxScaleFactor : Calendar_date_pickerLibrary._kDayPickerGridPortraitMaxScaleFactor, child: GridView.CreateCustom(physics: new global::Doroti.Framework.Widgets.ClampingScrollPhysics(), gridDelegate: new _DayPickerGridDelegate__calendar_date_picker(context), childrenDelegate: new global::Doroti.Framework.Widgets.SliverChildListDelegate(dayItems, addRepaintBoundaries: false))));
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
        global::Doroti.Framework.Painting.TextStyle? dayStyleLocal = datePickerTheme.dayStyle ?? defaultsLocal.dayStyle;
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return getProperty(datePickerTheme) ?? getProperty(defaultsLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue((theme) =>
            {
                return getProperty(theme) is { } property ? property.resolve(states) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        var semanticLabelSuffix = widget.isToday ? $", {localizations.currentDateLabel}" : "";
        var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection44458 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (widget.isDisabled) { __collection44458.Add(WidgetState.disabled); } if (widget.isSelectedDay) { __collection44458.Add(WidgetState.selected); } return __collection44458; }))();
        _statesController.value = statesLocal;
        global::Doroti.Ui.Color? dayForegroundColorLocal = resolve<global::Doroti.Ui.Color?>((theme) => widget.isToday ? theme?.todayForegroundColor : theme?.dayForegroundColor, statesLocal);
        global::Doroti.Ui.Color? dayBackgroundColorLocal = resolve<global::Doroti.Ui.Color?>((theme) => widget.isToday ? theme?.todayBackgroundColor : theme?.dayBackgroundColor, statesLocal);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> dayOverlayColorLocal = WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => effectiveValue((theme) => theme?.dayOverlayColor?.resolve(states)));
        global::Doroti.Framework.Painting.OutlinedBorder dayShapeLocal = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>((theme) => theme?.dayShape, statesLocal)!;
        bool hasCustomBorderColor = (datePickerTheme.todayBorder is not null) && (datePickerTheme.todayBorder!.color.opacity != 0.0);
        global::Doroti.Framework.Painting.BorderSide todayBorderSide = hasCustomBorderColor ? datePickerTheme.todayBorder! : (datePickerTheme.todayBorder ?? defaultsLocal.todayBorder!).copyWith(color: dayForegroundColorLocal);
        var decorationLocal = widget.isToday ? new global::Doroti.Framework.Painting.ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal.copyWith(side: todayBorderSide)) : new global::Doroti.Framework.Painting.ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal);
        global::Doroti.Framework.Widgets.Widget dayWidget = new Ink(decoration: decorationLocal, child: new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Text(localizations.formatDecimal(widget.day.Day), style: dayStyleLocal?.apply(color: dayForegroundColorLocal))));
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        if (Equals(orientation, Orientation.portrait))
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateAll(4.0), child: dayWidget));
        }
        dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Semantics(label: $"{localizations.formatDecimal(widget.day.Day)}, {widget.calendarDelegate.formatFullDate(widget.day, localizations)}{semanticLabelSuffix}", button: true, selected: widget.isSelectedDay, enabled: !widget.isDisabled, excludeSemantics: true, child: dayWidget));
        if (!widget.isDisabled)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkResponse(focusNode: widget.focusNode, onTap: () => { widget.onChanged(widget.day); }, statesController: _statesController, overlayColor: dayOverlayColorLocal, customBorder: dayShapeLocal, containedInkWell: true, child: dayWidget));
        }
        return dayWidget;
    }

    public override void dispose()
    {
        _statesController.dispose();
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
        double textScaleFactor = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale;
        global::Doroti.Framework.Widgets.Orientation orientation = MediaQuery.orientationOf(context);
        double dayPickerRowHeight = Equals(orientation, Orientation.portrait) ? Calendar_date_pickerLibrary._dayPickerRowHeightM3 : Calendar_date_pickerLibrary._dayPickerRowHeightLandscape;
        double scaledRowHeight = (textScaleFactor > 1.3) ? ((textScaleFactor - 1L) * 30L + dayPickerRowHeight) : dayPickerRowHeight;
        long columnCount = 7L;
        double tileWidth = constraints.crossAxisExtent / columnCount;
        double tileHeight = Math.Min(scaledRowHeight, constraints.viewportMainAxisExtent / (Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L));
        return new global::Doroti.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth, childMainAxisExtent: tileHeight, crossAxisCount: columnCount, crossAxisStride: tileWidth, mainAxisStride: tileHeight, reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(constraints.crossAxisDirection));
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

    public YearPicker(global::Doroti.Framework.Foundation.Key? key = null, DateTime? currentDate = null, DateTime firstDate = default!, DateTime lastDate = default!, DateTime? initialDate = null, DateTime? selectedDate = default!, global::System.Action<DateTime> onChanged = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start, CalendarDelegate<DateTime> calendarDelegate = default!) : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate = calendarDelegate ?? new GregorianCalendarDelegate();
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.selectedDate = selectedDate;
        this.onChanged = onChanged;
        this.dragStartBehavior = dragStartBehavior;
        this.calendarDelegate = __calendarDelegate;
        this.currentDate = this.calendarDelegate.dateOnly(currentDate ?? new DateTime());
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
        _scrollController = new global::Doroti.Framework.Widgets.ScrollController(initialScrollOffset: _scrollOffsetForYear(widget.selectedDate ?? widget.firstDate));
    }

    public override void dispose()
    {
        _scrollController?.dispose();
        _statesController.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(YearPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!Equals(widget.selectedDate, oldWidget.selectedDate)) && (widget.selectedDate is not null))
        {
            _scrollController!.jumpTo(_scrollOffsetForYear(DartRuntimePrimitives.RequireValue(widget.selectedDate)));
        }
    }

    internal virtual double _scrollOffsetForYear(DateTime date)
    {
        long initialYearIndex = date.Year - widget.firstDate.Year;
        long initialYearRow = checked(initialYearIndex / Calendar_date_pickerLibrary._yearPickerColumnCount);
        long centeredYearRow = initialYearRow - 2L;
        return (_itemCount < minYears) ? 0 : (centeredYearRow * Calendar_date_pickerLibrary._yearPickerRowHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildYearItem(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        P? effectiveValue<P>(global::System.Func<DatePickerThemeData?, P?> getProperty)
        {
            return getProperty(datePickerTheme) ?? getProperty(defaultsLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(global::System.Func<DatePickerThemeData?, global::Doroti.Framework.Widgets.WidgetStateProperty<P>?> getProperty, HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
        {
            return effectiveValue((theme) =>
            {
                return getProperty(theme) is { } property ? property.resolve(states) : default;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double textScaleFactor = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale;
        long offset = (_itemCount < minYears) ? checked((minYears - _itemCount) / 2L) : 0L;
        long year = widget.firstDate.Year + index - offset;
        var isSelected = year == widget.selectedDate?.Year;
        var isCurrentYear = year == widget.currentDate.Year;
        bool isDisabled = (year < widget.firstDate.Year) || (year > widget.lastDate.Year);
        double decorationHeight = 36.0 * textScaleFactor;
        double decorationWidth = 72.0 * textScaleFactor;
        var statesLocal = ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection54093 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (isDisabled) { __collection54093.Add(WidgetState.disabled); } if (isSelected) { __collection54093.Add(WidgetState.selected); } return __collection54093; }))();
        global::Doroti.Ui.Color? textColor = resolve<global::Doroti.Ui.Color?>((theme) => isCurrentYear ? theme?.todayForegroundColor : theme?.yearForegroundColor, statesLocal);
        global::Doroti.Ui.Color? background = resolve<global::Doroti.Ui.Color?>((theme) => isCurrentYear ? theme?.todayBackgroundColor : theme?.yearBackgroundColor, statesLocal);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> overlayColorLocal = WidgetStateProperty.resolveWith<global::Doroti.Ui.Color?>((states) => effectiveValue((theme) => theme?.yearOverlayColor?.resolve(states)));
        global::Doroti.Framework.Painting.OutlinedBorder yearShapeLocal = resolve<global::Doroti.Framework.Painting.OutlinedBorder?>((theme) => theme?.yearShape, statesLocal)!;
        global::Doroti.Framework.Painting.BorderSide? borderSide = default!;
        if (isCurrentYear)
        {
            borderSide = datePickerTheme.todayBorder ?? defaultsLocal.todayBorder;
            if (borderSide is not null)
            {
                borderSide = borderSide.copyWith(color: textColor);
            }
        }
        var decorationLocal = new global::Doroti.Framework.Painting.ShapeDecoration(color: background, shape: yearShapeLocal.copyWith(side: borderSide));
        global::Doroti.Framework.Painting.TextStyle? itemStyle = (datePickerTheme.yearStyle ?? defaultsLocal.yearStyle)?.apply(color: textColor);
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        global::Doroti.Framework.Widgets.Widget yearItem = new global::Doroti.Framework.Widgets.Center(child: new global::Doroti.Framework.Widgets.Container(decoration: decorationLocal, height: decorationHeight, width: decorationWidth, alignment: Alignment.center, child: new global::Doroti.Framework.Widgets.Semantics(selected: isSelected, enabled: !isDisabled, button: true, child: new global::Doroti.Framework.Widgets.Text(widget.calendarDelegate.formatYear(year, localizations), style: itemStyle))));
        if (!isDisabled)
        {
            DateTime date = widget.calendarDelegate.getMonth(year, widget.selectedDate?.Month ?? 1L);
            if (date.isBefore(widget.calendarDelegate.getMonth(widget.firstDate.Year, widget.firstDate.Month)))
            {
                DartRuntimePrimitives.Assert(() => date.Year == widget.firstDate.Year);
                date = widget.calendarDelegate.getMonth(year, widget.firstDate.Month);
            }
            else
            {
                if (date.isAfter(widget.lastDate))
                {
                    DartRuntimePrimitives.Assert(() => date.Year == widget.lastDate.Year);
                    date = widget.calendarDelegate.getMonth(year, widget.lastDate.Month);
                }
            }
            _statesController.value = statesLocal;
            yearItem = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new InkWell(key: new global::Doroti.Framework.Foundation.ValueKey<long>(year), onTap: () => { widget.onChanged(date); }, statesController: _statesController, overlayColor: overlayColorLocal, child: yearItem));
        }
        return yearItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _itemCount
    {
        get
        {
            return widget.lastDate.Year - widget.firstDate.Year + 1L;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Column(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new Material(type: MaterialType.transparency, child: GridView.CreateBuilder(controller: _scrollController, dragStartBehavior: widget.dragStartBehavior, gridDelegate: new _YearPickerGridDelegate__calendar_date_picker(context), itemBuilder: _buildYearItem, itemCount: Math.Max(_itemCount, minYears), padding: EdgeInsets.CreateSymmetric(horizontal: Calendar_date_pickerLibrary._yearPickerPadding))))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Divider()) });
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
        double textScaleFactor = MediaQuery.textScalerOf(context).clamp(maxScaleFactor: 3.0).scale(Calendar_date_pickerLibrary._fontSizeToScale) / Calendar_date_pickerLibrary._fontSizeToScale;
        long scaledYearPickerColumnCount = (textScaleFactor > 1.65) ? (Calendar_date_pickerLibrary._yearPickerColumnCount - 1L) : Calendar_date_pickerLibrary._yearPickerColumnCount;
        double tileWidth = Math.Max((constraints.crossAxisExtent - ((scaledYearPickerColumnCount - 1L) * Calendar_date_pickerLibrary._yearPickerRowSpacing)) / scaledYearPickerColumnCount, 0.0);
        double scaledYearPickerRowHeight = (textScaleFactor > 1L) ? (Calendar_date_pickerLibrary._yearPickerRowHeight + (textScaleFactor - 1L) * 9L) : Calendar_date_pickerLibrary._yearPickerRowHeight;
        return new global::Doroti.Framework.Rendering.SliverGridRegularTileLayout(childCrossAxisExtent: tileWidth, childMainAxisExtent: scaledYearPickerRowHeight, crossAxisCount: scaledYearPickerColumnCount, crossAxisStride: tileWidth + Calendar_date_pickerLibrary._yearPickerRowSpacing, mainAxisStride: scaledYearPickerRowHeight, reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(constraints.crossAxisDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(global::Doroti.Framework.Rendering.SliverGridDelegate oldDelegate) => false;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static void _reportAnnouncementError(object exception, global::System.Diagnostics.StackTrace? stack)
    {
        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exception, stack: stack, library: "material library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while sending semantics announcement")));
    }
}
