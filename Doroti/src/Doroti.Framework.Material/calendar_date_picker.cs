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
    internal static double _maxDayPickerHeightLandscape =
        _dayPickerRowHeightLandscape * (_maxDayPickerRowCount + 1L);
}

public static partial class Calendar_date_pickerLibrary
{
    internal static double _maxDayPickerHeightM3 =
        _dayPickerRowHeightM3 * (_maxDayPickerRowCount + 1L);
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

public class CalendarDatePicker : StatefulWidget
{
    public virtual DateTime? initialDate { get; private set; }
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual Action<DateTime> onDateChanged { get; private set; } = default!;
    public virtual Action<DateTime>? onDisplayedMonthChanged { get; private set; }
    public virtual DatePickerMode initialCalendarMode { get; private set; } = default!;
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public CalendarDatePicker(
        Key? key = null,
        DateTime? initialDate = default!,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? currentDate = null,
        Action<DateTime> onDateChanged = default!,
        Action<DateTime>? onDisplayedMonthChanged = null,
        DatePickerMode initialCalendarMode = DatePickerMode.day,
        Func<DateTime, bool>? selectableDayPredicate = null,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate =
            calendarDelegate ?? new GregorianCalendarDelegate();
        this.onDateChanged = onDateChanged;
        this.onDisplayedMonthChanged = onDisplayedMonthChanged;
        this.initialCalendarMode = initialCalendarMode;
        this.selectableDayPredicate = selectableDayPredicate;
        this.calendarDelegate = __calendarDelegate;
        this.initialDate =
            (initialDate is null)
                ? null
                : this.calendarDelegate.dateOnly(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(initialDate)
                    )
                );
        this.firstDate = this.calendarDelegate.dateOnly(firstDate);
        this.lastDate = this.calendarDelegate.dateOnly(lastDate);
        this.currentDate = this.calendarDelegate.dateOnly(
            currentDate ?? this.calendarDelegate.now()
        );
        DartRuntimePrimitives.Assert(
            () => !this.lastDate.isBefore(this.firstDate),
            () =>
                (object?)$"lastDate {this.lastDate} must be on or after firstDate {this.firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (this.initialDate is null)
                || !DartRuntimePrimitives.RequireValue(this.initialDate).isBefore(this.firstDate),
            () =>
                (object?)
                    $"initialDate {this.initialDate} must be on or after firstDate {this.firstDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (this.initialDate is null)
                || !DartRuntimePrimitives.RequireValue(this.initialDate).isAfter(this.lastDate),
            () =>
                (object?)
                    $"initialDate {this.initialDate} must be on or before lastDate {this.lastDate}."
        );
        DartRuntimePrimitives.Assert(
            () =>
                (this.selectableDayPredicate is null)
                || (this.initialDate is null)
                || this.selectableDayPredicate!(
                    DartRuntimePrimitives.RequireValue(this.initialDate)
                ),
            () =>
                (object?)
                    $"Provided initialDate {this.initialDate} must satisfy provided selectableDayPredicate."
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CalendarDatePickerState__calendar_date_picker()
        );
}

internal class _CalendarDatePickerState__calendar_date_picker : State<CalendarDatePicker>
{
    internal virtual bool _announcedInitialDate { get; set; } = false;
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DatePickerMode _mode { get; set; } = default!;
    internal virtual DateTime _currentDisplayedMonthDate { get; set; } = default!;
    internal virtual DateTime? _selectedDate { get; set; } = default;
    internal virtual GlobalKey<IState> _monthPickerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual GlobalKey<IState> _yearPickerKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _mode = widget.initialCalendarMode;
        DateTime currentDisplayedDate = widget.initialDate ?? widget.currentDate;
        _currentDisplayedMonthDate = widget.calendarDelegate.getMonth(
            currentDisplayedDate.Year,
            currentDisplayedDate.Month
        );
        if (widget.initialDate is not null)
        {
            _selectedDate = widget.initialDate;
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        _localizations = MaterialLocalizations.of(context);
        _textDirection = Directionality.of(context);
        if (!_announcedInitialDate && (widget.initialDate is not null))
        {
            DartRuntimePrimitives.Assert(() => _selectedDate is not null);
            _announcedInitialDate = true;
            bool isToday = widget.calendarDelegate.isSameDay(widget.currentDate, _selectedDate);
            var semanticLabelSuffix = isToday ? $", {_localizations.currentDateLabel}" : "";
            _announce(
                $"{_localizations.formatFullDate(DartRuntimePrimitives.RequireValue(_selectedDate))}{semanticLabelSuffix}"
            );
        }
    }

    internal virtual void _announce(string message)
    {
        if (MediaQuery.maybeSupportsAnnounceOf(context) ?? false)
        {
            DartRuntimePrimitives.Ignore(
                SemanticsService
                    .sendAnnouncement(View.of(context), message, Directionality.of(context))
                    .catchError(Calendar_date_pickerLibrary._reportAnnouncementError)
            );
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
                string message = mode switch
                {
                    DatePickerMode.day => widget.calendarDelegate.formatMonthYear(
                        selected,
                        _localizations
                    ),
                    DatePickerMode.year => widget.calendarDelegate.formatYear(
                        selected.Year,
                        _localizations
                    ),
                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                        throw new InvalidOperationException("Non-exhaustive Dart switch value."),
                };
                _announce(message);
            }
        });
    }

    internal virtual void _handleMonthChanged(DateTime date)
    {
        setState(() =>
        {
            if (
                (_currentDisplayedMonthDate.Year != date.Year)
                || (_currentDisplayedMonthDate.Month != date.Month)
            )
            {
                _currentDisplayedMonthDate = widget.calendarDelegate.getMonth(
                    date.Year,
                    date.Month
                );
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
                    bool isToday = widget.calendarDelegate.isSameDay(
                        widget.currentDate,
                        _selectedDate
                    );
                    var semanticLabelSuffix = isToday ? $", {_localizations.currentDateLabel}" : "";
                    DartRuntimePrimitives.Ignore(
                        SemanticsService
                            .sendAnnouncement(
                                View.of(context),
                                $"{_localizations.selectedDateLabel} {widget.calendarDelegate.formatFullDate(DartRuntimePrimitives.RequireValue(_selectedDate), _localizations)}{semanticLabelSuffix}",
                                _textDirection
                            )
                            .catchError(Calendar_date_pickerLibrary._reportAnnouncementError)
                    );
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
        return widget.selectableDayPredicate is null
            ? true
            : widget.selectableDayPredicate.Invoke(date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildPicker()
    {
        switch (_mode)
        {
            case DatePickerMode.day:
            {
                return new _MonthPicker__calendar_date_picker(
                    key: _monthPickerKey,
                    calendarDelegate: widget.calendarDelegate,
                    initialMonth: _currentDisplayedMonthDate,
                    currentDate: widget.currentDate,
                    firstDate: widget.firstDate,
                    lastDate: widget.lastDate,
                    selectedDate: _selectedDate,
                    onChanged: _handleDayChanged,
                    onDisplayedMonthChanged: _handleMonthChanged,
                    selectableDayPredicate: widget.selectableDayPredicate
                );
            }
            case DatePickerMode.year:
            {
                return new Padding(
                    padding: EdgeInsets.CreateOnly(
                        top: Calendar_date_pickerLibrary._subHeaderHeight
                    ),
                    child: new YearPicker(
                        key: _yearPickerKey,
                        calendarDelegate: widget.calendarDelegate,
                        currentDate: widget.currentDate,
                        firstDate: widget.firstDate,
                        lastDate: widget.lastDate,
                        selectedDate: _currentDisplayedMonthDate,
                        onChanged: _handleYearChanged
                    )
                );
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: Calendar_date_pickerLibrary._kMaxTextScaleFactor)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        Orientation orientation = MediaQuery.orientationOf(context);
        double maxDayPickerHeight = Equals(orientation, Orientation.portrait)
            ? Calendar_date_pickerLibrary._maxDayPickerHeightM3
            : Calendar_date_pickerLibrary._maxDayPickerHeightLandscape;
        double scaledMaxDayPickerHeight =
            (textScaleFactor > 1.3)
                ? (
                    maxDayPickerHeight
                    + (
                        (Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)
                        * ((textScaleFactor - 1L) * 8L)
                    )
                )
                : maxDayPickerHeight;
        var picker = new SizedBox(
            height: Calendar_date_pickerLibrary._subHeaderHeight + scaledMaxDayPickerHeight,
            child: _buildPicker()
        );
        return new Stack(
            children: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection15772 = new List<Widget>();
                        if (MediaQuery.maybeSupportsAnnounceOf(context) ?? false)
                        {
                            __collection15772.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(picker)
                            );
                        }
                        else
                        {
                            __collection15772.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new Widgets.Semantics(
                                        container: true,
                                        liveRegion: true,
                                        accessibilityFocusBlockType: AccessibilityFocusBlockType.blockNode,
                                        label: _announcementText,
                                        child: picker
                                    )
                                )
                            );
                        }
                        __collection15772.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                MediaQuery.withClampedTextScaling(
                                    maxScaleFactor: Calendar_date_pickerLibrary._kModeToggleButtonMaxScaleFactor,
                                    child: new _DatePickerModeToggleButton__calendar_date_picker(
                                        mode: _mode,
                                        title: widget.calendarDelegate.formatMonthYear(
                                            _currentDisplayedMonthDate,
                                            _localizations
                                        ),
                                        onTitlePressed: () =>
                                        {
                                            _handleModeChanged(
                                                _mode switch
                                                {
                                                    DatePickerMode.day => DatePickerMode.year,
                                                    DatePickerMode.year => DatePickerMode.day,
                                                    _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                                                        throw new InvalidOperationException(
                                                            "Non-exhaustive Dart switch value."
                                                        ),
                                                }
                                            );
                                        }
                                    )
                                )
                            )
                        );
                        return __collection15772;
                    }
                )
            )()
        );
    }
}

public class _DatePickerModeToggleButton__calendar_date_picker : StatefulWidget
{
    public virtual DatePickerMode mode { get; private set; } = default!;
    public virtual string title { get; private set; } = default!;
    public virtual Action onTitlePressed { get; private set; } = default!;

    internal _DatePickerModeToggleButton__calendar_date_picker(
        DatePickerMode mode,
        string title,
        Action onTitlePressed
    )
    {
        this.mode = mode;
        this.title = title;
        this.onTitlePressed = onTitlePressed;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _DatePickerModeToggleButtonState__calendar_date_picker()
        );
}

public class _DatePickerModeToggleButtonState__calendar_date_picker
    : State<_DatePickerModeToggleButton__calendar_date_picker>,
        SingleTickerProviderStateMixin<_DatePickerModeToggleButton__calendar_date_picker>
{
    internal virtual AnimationController _controller { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(
            value: Equals(widget.mode, DatePickerMode.year) ? 0.5 : 0,
            upperBound: 0.5,
            duration: Duration.Create(milliseconds: 200L),
            vsync: this
        );
    }

    public override void didUpdateWidget(
        _DatePickerModeToggleButton__calendar_date_picker oldWidget
    )
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

    public override Widget build(BuildContext context)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextStyle? buttonTextStyle =
            datePickerTheme.toggleButtonTextStyle ?? defaultsLocal.toggleButtonTextStyle;
        Color? subHeaderForegroundColorLocal =
            datePickerTheme.subHeaderForegroundColor ?? defaultsLocal.subHeaderForegroundColor;
        Color? buttonTextColor =
            (
                datePickerTheme.toggleButtonTextStyle?.color
                ?? datePickerTheme.subHeaderForegroundColor
            ) ?? defaultsLocal.toggleButtonTextStyle?.color;
        return new SizedBox(
            height: Calendar_date_pickerLibrary._subHeaderHeight,
            child: new Padding(
                padding: EdgeInsetsDirectional.CreateOnly(start: 16, end: 4),
                child: new Row(
                    children: (
                        (Func<List<Widget>>)(
                            () =>
                            {
                                var __collection19110 = new List<Widget>();
                                __collection19110.Add(
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        new Flexible(
                                            child: new Widgets.Semantics(
                                                label: MaterialLocalizations
                                                    .of(context)
                                                    .selectYearSemanticsLabel,
                                                button: true,
                                                container: true,
                                                child: new SizedBox(
                                                    height: Calendar_date_pickerLibrary._subHeaderHeight,
                                                    child: new InkWell(
                                                        onTap: widget.onTitlePressed,
                                                        child: new Padding(
                                                            padding: EdgeInsets.CreateSymmetric(
                                                                horizontal: 8
                                                            ),
                                                            child: new Row(
                                                                children: new List<Widget>
                                                                {
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        new Flexible(
                                                                            child: new Text(
                                                                                widget.title,
                                                                                overflow: TextOverflow.ellipsis,
                                                                                style: buttonTextStyle?.apply(
                                                                                    color: buttonTextColor
                                                                                )
                                                                            )
                                                                        )
                                                                    ),
                                                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                                                        new RotationTransition(
                                                                            turns: _controller,
                                                                            child: new Icon(
                                                                                Icons.arrow_drop_down,
                                                                                color: subHeaderForegroundColorLocal
                                                                            )
                                                                        )
                                                                    ),
                                                                }
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                );
                                if (Equals(widget.mode, DatePickerMode.day))
                                {
                                    __collection19110.Add(
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new SizedBox(
                                                width: Calendar_date_pickerLibrary._monthNavButtonsWidth
                                            )
                                        )
                                    );
                                }
                                return __collection19110;
                            }
                        )
                    )()
                )
            )
        );
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
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
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
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _MonthPicker__calendar_date_picker : StatefulWidget
{
    public virtual DateTime initialMonth { get; private set; } = default!;
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime? selectedDate { get; private set; }
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual Action<DateTime> onDisplayedMonthChanged { get; private set; } = default!;
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _MonthPicker__calendar_date_picker(
        Key? key = null,
        DateTime initialMonth = default!,
        DateTime currentDate = default!,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? selectedDate = default!,
        Action<DateTime> onChanged = default!,
        Action<DateTime> onDisplayedMonthChanged = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!,
        Func<DateTime, bool>? selectableDayPredicate = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (selectedDate is null)
                || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDate is null)
                || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate)
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MonthPickerState__calendar_date_picker());
}

internal class _MonthPickerState__calendar_date_picker : State<_MonthPicker__calendar_date_picker>
{
    internal virtual GlobalKey<IState> _pageViewKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual string _announcementText { get; set; } = "";
    internal virtual DateTime _currentMonth { get; set; } = default!;
    internal virtual PageController _pageController { get; set; } = default!;
    internal virtual MaterialLocalizations _localizations { get; set; } = default!;
    internal virtual DartMap<ShortcutActivator, Intent>? _shortcutMap { get; set; } = default;
    internal virtual DartMap<Type, dynamic>? _actionMap { get; set; } = default;
    internal virtual FocusNode _dayGridFocus { get; set; } = default!;
    internal virtual DateTime? _focusedDay { get; set; } = default;
    internal static DartMap<TraversalDirection, long> _directionOffset = new DartMap<
        TraversalDirection,
        long
    >
    {
        [TraversalDirection.up] = -7L,
        [TraversalDirection.right] = 1L,
        [TraversalDirection.down] = 7L,
        [TraversalDirection.left] = -1L,
    };

    public override void initState()
    {
        base.initState();
        _currentMonth = widget.initialMonth;
        _pageController = new PageController(
            initialPage: widget.calendarDelegate.monthDelta(widget.firstDate, _currentMonth)
        );
        _shortcutMap = new DartMap<ShortcutActivator, Intent>
        {
            [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new DirectionalFocusIntent(
                TraversalDirection.left
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new DirectionalFocusIntent(
                TraversalDirection.right
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new DirectionalFocusIntent(
                TraversalDirection.down
            ),
            [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new DirectionalFocusIntent(
                TraversalDirection.up
            ),
        }.cast<ShortcutActivator, Intent>();
        _actionMap = new DartMap<Type, dynamic>
        {
            [typeof(NextFocusIntent)] = new CallbackAction<NextFocusIntent>(
                onInvoke: _handleGridNextFocus
            ),
            [typeof(PreviousFocusIntent)] = new CallbackAction<PreviousFocusIntent>(
                onInvoke: _handleGridPreviousFocus
            ),
            [typeof(DirectionalFocusIntent)] = new CallbackAction<DirectionalFocusIntent>(
                onInvoke: _handleDirectionFocus
            ),
        }.cast<Type, dynamic>();
        _dayGridFocus = new FocusNode(debugLabel: "Day Grid");
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
            DartRuntimePrimitives.Ignore(
                SemanticsService
                    .sendAnnouncement(View.of(context), message, Directionality.of(context))
                    .catchError(Calendar_date_pickerLibrary._reportAnnouncementError)
            );
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
            DateTime monthDate = widget.calendarDelegate.addMonthsToMonthDate(
                widget.firstDate,
                monthPage
            );
            if (!widget.calendarDelegate.isSameMonth(_currentMonth, monthDate))
            {
                _currentMonth = widget.calendarDelegate.getMonth(monthDate.Year, monthDate.Month);
                widget.onDisplayedMonthChanged(_currentMonth);
                if (
                    (_focusedDay is not null)
                    && !widget.calendarDelegate.isSameMonth(_focusedDay, _currentMonth)
                )
                {
                    _focusedDay = _focusableDayForMonth(
                        _currentMonth,
                        DartRuntimePrimitives.RequireValue(_focusedDay).Day
                    );
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
            DateTime newFocus = widget.calendarDelegate.getDay(
                month.Year,
                month.Month,
                preferredDay
            );
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
            DartRuntimePrimitives.Ignore(
                _pageController.nextPage(
                    duration: Calendar_date_pickerLibrary._monthScrollDuration,
                    curve: Curves.ease
                )
            );
        }
    }

    internal virtual void _handlePreviousMonth()
    {
        if (!_isDisplayingFirstMonth)
        {
            DartRuntimePrimitives.Ignore(
                _pageController.previousPage(
                    duration: Calendar_date_pickerLibrary._monthScrollDuration,
                    curve: Curves.ease
                )
            );
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
            DartRuntimePrimitives.Ignore(
                _pageController.animateToPage(
                    monthPage,
                    duration: Calendar_date_pickerLibrary._monthScrollDuration,
                    curve: Curves.ease
                )
            );
        }
    }

    internal virtual bool _isDisplayingFirstMonth
    {
        get
        {
            return !_currentMonth.isAfter(
                widget.calendarDelegate.getMonth(widget.firstDate.Year, widget.firstDate.Month)
            );
        }
    }
    internal virtual bool _isDisplayingLastMonth
    {
        get
        {
            return !_currentMonth.isBefore(
                widget.calendarDelegate.getMonth(widget.lastDate.Year, widget.lastDate.Month)
            );
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

    internal virtual void _handleGridNextFocus(NextFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.nextFocus();
    }

    internal virtual void _handleGridPreviousFocus(PreviousFocusIntent intent)
    {
        _dayGridFocus.requestFocus();
        _dayGridFocus.previousFocus();
    }

    internal virtual void _handleDirectionFocus(DirectionalFocusIntent intent)
    {
        DartRuntimePrimitives.Assert(() => _focusedDay is not null);
        setState(() =>
        {
            DateTime? nextDate = _nextDateInDirection(
                DartRuntimePrimitives.RequireValue(_focusedDay),
                intent.direction
            );
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

    internal virtual long _dayDirectionOffset(
        TraversalDirection traversalDirection,
        TextDirection textDirection
    )
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
        return DartRuntimePrimitives.RequireValue(
            DartCollectionRuntime.NullableMapValue<long>(_directionOffset, traversalDirection)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DateTime? _nextDateInDirection(DateTime date, TraversalDirection direction)
    {
        TextDirection textDirection = Directionality.of(context);
        DateTime nextDate = widget.calendarDelegate.addDaysToDate(
            date,
            _dayDirectionOffset(direction, textDirection)
        );
        while (!nextDate.isBefore(widget.firstDate) && !nextDate.isAfter(widget.lastDate))
        {
            if (_isSelectable(nextDate))
            {
                return nextDate;
            }
            nextDate = widget.calendarDelegate.addDaysToDate(
                nextDate,
                _dayDirectionOffset(direction, textDirection)
            );
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isSelectable(DateTime date)
    {
        return widget.selectableDayPredicate is null
            ? true
            : widget.selectableDayPredicate.Invoke(date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildItems(BuildContext context, long index)
    {
        DateTime month = widget.calendarDelegate.addMonthsToMonthDate(widget.firstDate, index);
        return new _DayPicker__calendar_date_picker(
            key: new ValueKey<DateTime>(month),
            calendarDelegate: widget.calendarDelegate,
            selectedDate: widget.selectedDate,
            currentDate: widget.currentDate,
            onChanged: _handleDateSelected,
            firstDate: widget.firstDate,
            lastDate: widget.lastDate,
            displayedMonth: month,
            selectableDayPredicate: widget.selectableDayPredicate
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Color? subHeaderForegroundColorLocal =
            DatePickerTheme.of(context).subHeaderForegroundColor
            ?? DatePickerTheme.defaults(context).subHeaderForegroundColor;
        bool supportsAnnounce = MediaQuery.maybeSupportsAnnounceOf(context) ?? false;
        return new Widgets.Semantics(
            container: true,
            explicitChildNodes: true,
            liveRegion: !supportsAnnounce,
            accessibilityFocusBlockType: !supportsAnnounce
                ? AccessibilityFocusBlockType.blockNode
                : AccessibilityFocusBlockType.none,
            label: !supportsAnnounce ? _announcementText : null,
            child: new Column(
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new SizedBox(
                            height: Calendar_date_pickerLibrary._subHeaderHeight,
                            child: new Padding(
                                padding: EdgeInsetsDirectional.CreateOnly(start: 16, end: 4),
                                child: new Row(
                                    children: new List<Widget>
                                    {
                                        DartRuntimePrimitives.ConvertValue<Widget>(new Spacer()),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new IconButton(
                                                icon: new Icon(
                                                    Icons.chevron_left,
                                                    semanticLabel: _isDisplayingFirstMonth
                                                        ? _localizations.previousMonthTooltip
                                                        : null
                                                ),
                                                color: subHeaderForegroundColorLocal,
                                                tooltip: _isDisplayingFirstMonth
                                                    ? null
                                                    : _localizations.previousMonthTooltip,
                                                onPressed: _isDisplayingFirstMonth
                                                    ? null
                                                    : _handlePreviousMonth
                                            )
                                        ),
                                        DartRuntimePrimitives.ConvertValue<Widget>(
                                            new IconButton(
                                                icon: new Icon(
                                                    Icons.chevron_right,
                                                    semanticLabel: _isDisplayingLastMonth
                                                        ? _localizations.nextMonthTooltip
                                                        : null
                                                ),
                                                color: subHeaderForegroundColorLocal,
                                                tooltip: _isDisplayingLastMonth
                                                    ? null
                                                    : _localizations.nextMonthTooltip,
                                                onPressed: _isDisplayingLastMonth
                                                    ? null
                                                    : _handleNextMonth
                                            )
                                        ),
                                    }
                                )
                            )
                        )
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Expanded(
                            child: new FocusableActionDetector(
                                shortcuts: _shortcutMap,
                                actions: _actionMap,
                                focusNode: _dayGridFocus,
                                onFocusChange: _handleGridFocusChange,
                                child: new _FocusedDate__calendar_date_picker(
                                    calendarDelegate: widget.calendarDelegate,
                                    date: _dayGridFocus.hasFocus ? _focusedDay : null,
                                    child: new Material(
                                        type: MaterialType.transparency,
                                        child: PageView.CreateBuilder(
                                            key: _pageViewKey,
                                            controller: _pageController,
                                            itemBuilder: _buildItems,
                                            itemCount: widget.calendarDelegate.monthDelta(
                                                widget.firstDate,
                                                widget.lastDate
                                            ) + 1L,
                                            onPageChanged: _handleMonthPageChanged
                                        )
                                    )
                                )
                            )
                        )
                    ),
                }
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FocusedDate__calendar_date_picker : InheritedWidget
{
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;
    public virtual DateTime? date { get; private set; }

    internal _FocusedDate__calendar_date_picker(
        Widget child,
        CalendarDelegate<DateTime> calendarDelegate,
        DateTime? date = null
    )
        : base(child: child)
    {
        this.calendarDelegate = calendarDelegate;
        this.date = date;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __oldWidget = (_FocusedDate__calendar_date_picker)oldWidget;
        return !calendarDelegate.isSameDay(date, __oldWidget.date);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DateTime? maybeOf(BuildContext context)
    {
        _FocusedDate__calendar_date_picker? focusedDate =
            context.dependOnInheritedWidgetOfExactType<_FocusedDate__calendar_date_picker>();
        return focusedDate?.date;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DayPicker__calendar_date_picker : StatefulWidget
{
    public virtual DateTime? selectedDate { get; private set; }
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime displayedMonth { get; private set; } = default!;
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _DayPicker__calendar_date_picker(
        Key? key = null,
        DateTime currentDate = default!,
        DateTime displayedMonth = default!,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? selectedDate = default!,
        Action<DateTime> onChanged = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!,
        Func<DateTime, bool>? selectableDayPredicate = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (selectedDate is null)
                || !DartRuntimePrimitives.RequireValue(selectedDate).isBefore(firstDate)
        );
        System.Diagnostics.Debug.Assert(
            (selectedDate is null)
                || !DartRuntimePrimitives.RequireValue(selectedDate).isAfter(lastDate)
        );
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DayPickerState__calendar_date_picker());
}

internal class _DayPickerState__calendar_date_picker : State<_DayPicker__calendar_date_picker>
{
    internal virtual List<FocusNode> _dayFocusNodes { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(
            widget.displayedMonth.Year,
            widget.displayedMonth.Month
        );
        _dayFocusNodes = new List<FocusNode>(
            Enumerable.Select(
                Enumerable.Range(0, checked((int)daysInMonth)),
                (index) => new FocusNode(skipTraversal: true, debugLabel: $"Day {index + 1L}")
            )
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        DateTime? focusedDate = _FocusedDate__calendar_date_picker.maybeOf(context);
        if (
            (focusedDate is not null)
            && widget.calendarDelegate.isSameMonth(
                widget.displayedMonth,
                DartRuntimePrimitives.RequireValue(focusedDate)
            )
        )
        {
            DateTime focusedDate__38602__value38655 = DartRuntimePrimitives.RequireValue(
                focusedDate
            );
            _dayFocusNodes[
                (int)(DartRuntimePrimitives.RequireValue(focusedDate__38602__value38655).Day - 1L)
            ]
                .requestFocus();
        }
    }

    public override void dispose()
    {
        foreach (FocusNode node in _dayFocusNodes)
        {
            node.dispose();
        }
        base.dispose();
    }

    internal virtual List<Widget> _dayHeaders(
        TextStyle? headerStyle,
        MaterialLocalizations localizations
    )
    {
        var result = new List<Widget>();
        for (
            long i = localizations.firstDayOfWeekIndex;
            checked(result.Count) < 7L;
            i = (i + 1L) % 7L
        )
        {
            string weekday = localizations.narrowWeekdays[(int)i];
            result.Add(
                new ExcludeSemantics(
                    child: new Center(child: new Text(weekday, style: headerStyle))
                )
            );
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        TextStyle? weekdayStyleLocal = datePickerTheme.weekdayStyle ?? defaultsLocal.weekdayStyle;
        Orientation orientation = MediaQuery.orientationOf(context);
        var isLandscapeOrientation = Equals(orientation, Orientation.landscape);
        long year = widget.displayedMonth.Year;
        long month = widget.displayedMonth.Month;
        long daysInMonth = widget.calendarDelegate.getDaysInMonth(year, month);
        long dayOffset = widget.calendarDelegate.firstDayOffset(year, month, localizations);
        List<Widget> dayItems = _dayHeaders(weekdayStyleLocal, localizations);
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
                bool isDisabledLocal =
                    dayToBuild.isAfter(widget.lastDate)
                    || dayToBuild.isBefore(widget.firstDate)
                    || (
                        (widget.selectableDayPredicate is not null)
                        && !widget.selectableDayPredicate!(dayToBuild)
                    );
                bool isSelectedDayLocal = widget.calendarDelegate.isSameDay(
                    widget.selectedDate,
                    dayToBuild
                );
                bool isTodayLocal = widget.calendarDelegate.isSameDay(
                    widget.currentDate,
                    dayToBuild
                );
                dayItems.Add(
                    new _Day__calendar_date_picker(
                        dayToBuild,
                        key: new ValueKey<DateTime>(dayToBuild),
                        isDisabled: isDisabledLocal,
                        isSelectedDay: isSelectedDayLocal,
                        isToday: isTodayLocal,
                        onChanged: widget.onChanged,
                        focusNode: _dayFocusNodes[(int)(day - 1L)],
                        calendarDelegate: widget.calendarDelegate
                    )
                );
            }
        }
        double monthPickerHorizontalPadding =
            (!isLandscapeOrientation)
                ? Calendar_date_pickerLibrary._monthPickerHorizontalPaddingPortraitM3
                : Calendar_date_pickerLibrary._monthPickerHorizontalPaddingOther;
        return new Padding(
            padding: EdgeInsets.CreateSymmetric(horizontal: monthPickerHorizontalPadding),
            child: MediaQuery.withClampedTextScaling(
                maxScaleFactor: isLandscapeOrientation
                    ? Calendar_date_pickerLibrary._kDayPickerGridLandscapeMaxScaleFactor
                    : Calendar_date_pickerLibrary._kDayPickerGridPortraitMaxScaleFactor,
                child: GridView.CreateCustom(
                    physics: new ClampingScrollPhysics(),
                    gridDelegate: new _DayPickerGridDelegate__calendar_date_picker(context),
                    childrenDelegate: new SliverChildListDelegate(
                        dayItems,
                        addRepaintBoundaries: false
                    )
                )
            )
        );
    }
}

internal class _Day__calendar_date_picker : StatefulWidget
{
    public virtual DateTime day { get; private set; } = default!;
    public virtual bool isDisabled { get; private set; } = default!;
    public virtual bool isSelectedDay { get; private set; } = default!;
    public virtual bool isToday { get; private set; } = default!;
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    internal _Day__calendar_date_picker(
        DateTime day,
        Key? key = null,
        bool isDisabled = default!,
        bool isSelectedDay = default!,
        bool isToday = default!,
        Action<DateTime> onChanged = default!,
        FocusNode focusNode = default!,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        this.day = day;
        this.isDisabled = isDisabled;
        this.isSelectedDay = isSelectedDay;
        this.isToday = isToday;
        this.onChanged = onChanged;
        this.focusNode = focusNode;
        this.calendarDelegate = calendarDelegate;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _DayState__calendar_date_picker());
}

internal class _DayState__calendar_date_picker : State<_Day__calendar_date_picker>
{
    internal virtual WidgetStatesController _statesController { get; private set; } =
        new WidgetStatesController();

    public override Widget build(BuildContext context)
    {
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        TextStyle? dayStyleLocal = datePickerTheme.dayStyle ?? defaultsLocal.dayStyle;
        P? effectiveValue<P>(Func<DatePickerThemeData?, P?> getProperty)
        {
            return getProperty(datePickerTheme) ?? getProperty(defaultsLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(
            Func<DatePickerThemeData?, WidgetStateProperty<P>?> getProperty,
            HashSet<WidgetState> states
        )
        {
            return effectiveValue(
                (theme) =>
                {
                    return getProperty(theme) is { } property ? property.resolve(states) : default;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        var semanticLabelSuffix = widget.isToday ? $", {localizations.currentDateLabel}" : "";
        var statesLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection44458 = new HashSet<WidgetState>();
                    if (widget.isDisabled)
                    {
                        __collection44458.Add(WidgetState.disabled);
                    }
                    if (widget.isSelectedDay)
                    {
                        __collection44458.Add(WidgetState.selected);
                    }
                    return __collection44458;
                }
            )
        )();
        _statesController.value = statesLocal;
        Color? dayForegroundColorLocal = resolve(
            (theme) => widget.isToday ? theme?.todayForegroundColor : theme?.dayForegroundColor,
            statesLocal
        );
        Color? dayBackgroundColorLocal = resolve(
            (theme) => widget.isToday ? theme?.todayBackgroundColor : theme?.dayBackgroundColor,
            statesLocal
        );
        WidgetStateProperty<Color?> dayOverlayColorLocal = WidgetStateProperty.resolveWith(
            (states) => effectiveValue((theme) => theme?.dayOverlayColor?.resolve(states))
        );
        OutlinedBorder dayShapeLocal = resolve((theme) => theme?.dayShape, statesLocal)!;
        bool hasCustomBorderColor =
            (datePickerTheme.todayBorder is not null)
            && (datePickerTheme.todayBorder!.color.opacity != 0.0);
        BorderSide todayBorderSide = hasCustomBorderColor
            ? datePickerTheme.todayBorder!
            : (datePickerTheme.todayBorder ?? defaultsLocal.todayBorder!).copyWith(
                color: dayForegroundColorLocal
            );
        var decorationLocal = widget.isToday
            ? new ShapeDecoration(
                color: dayBackgroundColorLocal,
                shape: dayShapeLocal.copyWith(side: todayBorderSide)
            )
            : new ShapeDecoration(color: dayBackgroundColorLocal, shape: dayShapeLocal);
        Widget dayWidget = new Ink(
            decoration: decorationLocal,
            child: new Center(
                child: new Text(
                    localizations.formatDecimal(widget.day.Day),
                    style: dayStyleLocal?.apply(color: dayForegroundColorLocal)
                )
            )
        );
        Orientation orientation = MediaQuery.orientationOf(context);
        if (Equals(orientation, Orientation.portrait))
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Padding(padding: EdgeInsets.CreateAll(4.0), child: dayWidget)
            );
        }
        dayWidget = DartRuntimePrimitives.ConvertValue<Widget>(
            new Widgets.Semantics(
                label: $"{localizations.formatDecimal(widget.day.Day)}, {widget.calendarDelegate.formatFullDate(widget.day, localizations)}{semanticLabelSuffix}",
                button: true,
                selected: widget.isSelectedDay,
                enabled: !widget.isDisabled,
                excludeSemantics: true,
                child: dayWidget
            )
        );
        if (!widget.isDisabled)
        {
            dayWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new InkResponse(
                    focusNode: widget.focusNode,
                    onTap: () =>
                    {
                        widget.onChanged(widget.day);
                    },
                    statesController: _statesController,
                    overlayColor: dayOverlayColorLocal,
                    customBorder: dayShapeLocal,
                    containedInkWell: true,
                    child: dayWidget
                )
            );
        }
        return dayWidget;
    }

    public override void dispose()
    {
        _statesController.dispose();
        base.dispose();
    }
}

internal class _DayPickerGridDelegate__calendar_date_picker : SliverGridDelegate
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DayPickerGridDelegate__calendar_date_picker(BuildContext context)
    {
        this.context = context;
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: 3.0)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        Orientation orientation = MediaQuery.orientationOf(context);
        double dayPickerRowHeight = Equals(orientation, Orientation.portrait)
            ? Calendar_date_pickerLibrary._dayPickerRowHeightM3
            : Calendar_date_pickerLibrary._dayPickerRowHeightLandscape;
        double scaledRowHeight =
            (textScaleFactor > 1.3)
                ? (((textScaleFactor - 1L) * 30L) + dayPickerRowHeight)
                : dayPickerRowHeight;
        long columnCount = 7L;
        double tileWidth = constraints.crossAxisExtent / columnCount;
        double tileHeight = Math.Min(
            scaledRowHeight,
            constraints.viewportMainAxisExtent
                / (Calendar_date_pickerLibrary._maxDayPickerRowCount + 1L)
        );
        return new SliverGridRegularTileLayout(
            childCrossAxisExtent: tileWidth,
            childMainAxisExtent: tileHeight,
            crossAxisCount: columnCount,
            crossAxisStride: tileWidth,
            mainAxisStride: tileHeight,
            reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(
                constraints.crossAxisDirection
            )
        );
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate) => false;
}

public class YearPicker : StatefulWidget
{
    public virtual DateTime currentDate { get; private set; } = default!;
    public virtual DateTime firstDate { get; private set; } = default!;
    public virtual DateTime lastDate { get; private set; } = default!;
    public virtual DateTime? selectedDate { get; private set; }
    public virtual Action<DateTime> onChanged { get; private set; } = default!;
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual CalendarDelegate<DateTime> calendarDelegate { get; private set; } = default!;

    public YearPicker(
        Key? key = null,
        DateTime? currentDate = null,
        DateTime firstDate = default!,
        DateTime lastDate = default!,
        DateTime? initialDate = null,
        DateTime? selectedDate = default!,
        Action<DateTime> onChanged = default!,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        CalendarDelegate<DateTime> calendarDelegate = default!
    )
        : base(key: key)
    {
        CalendarDelegate<DateTime> __calendarDelegate =
            calendarDelegate ?? new GregorianCalendarDelegate();
        this.firstDate = firstDate;
        this.lastDate = lastDate;
        this.selectedDate = selectedDate;
        this.onChanged = onChanged;
        this.dragStartBehavior = dragStartBehavior;
        this.calendarDelegate = __calendarDelegate;
        this.currentDate = this.calendarDelegate.dateOnly(currentDate ?? new DateTime());
        System.Diagnostics.Debug.Assert(!firstDate.isAfter(lastDate));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _YearPickerState__calendar_date_picker());
}

internal class _YearPickerState__calendar_date_picker : State<YearPicker>
{
    internal virtual ScrollController? _scrollController { get; set; } = default;
    internal virtual WidgetStatesController _statesController { get; private set; } =
        new WidgetStatesController();
    public const long minYears = 18L;

    public override void initState()
    {
        base.initState();
        _scrollController = new ScrollController(
            initialScrollOffset: _scrollOffsetForYear(widget.selectedDate ?? widget.firstDate)
        );
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
        if (
            (!Equals(widget.selectedDate, oldWidget.selectedDate))
            && (widget.selectedDate is not null)
        )
        {
            _scrollController!.jumpTo(
                _scrollOffsetForYear(DartRuntimePrimitives.RequireValue(widget.selectedDate))
            );
        }
    }

    internal virtual double _scrollOffsetForYear(DateTime date)
    {
        long initialYearIndex = date.Year - widget.firstDate.Year;
        long initialYearRow = checked(
            initialYearIndex / Calendar_date_pickerLibrary._yearPickerColumnCount
        );
        long centeredYearRow = initialYearRow - 2L;
        return (_itemCount < minYears)
            ? 0
            : (centeredYearRow * Calendar_date_pickerLibrary._yearPickerRowHeight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildYearItem(BuildContext context, long index)
    {
        DatePickerThemeData datePickerTheme = DatePickerTheme.of(context);
        DatePickerThemeData defaultsLocal = DatePickerTheme.defaults(context);
        P? effectiveValue<P>(Func<DatePickerThemeData?, P?> getProperty)
        {
            return getProperty(datePickerTheme) ?? getProperty(defaultsLocal);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(
            Func<DatePickerThemeData?, WidgetStateProperty<P>?> getProperty,
            HashSet<WidgetState> states
        )
        {
            return effectiveValue(
                (theme) =>
                {
                    return getProperty(theme) is { } property ? property.resolve(states) : default;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: 3.0)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        long offset = (_itemCount < minYears) ? checked((minYears - _itemCount) / 2L) : 0L;
        long year = widget.firstDate.Year + index - offset;
        var isSelected = year == widget.selectedDate?.Year;
        var isCurrentYear = year == widget.currentDate.Year;
        bool isDisabled = (year < widget.firstDate.Year) || (year > widget.lastDate.Year);
        double decorationHeight = 36.0 * textScaleFactor;
        double decorationWidth = 72.0 * textScaleFactor;
        var statesLocal = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection54093 = new HashSet<WidgetState>();
                    if (isDisabled)
                    {
                        __collection54093.Add(WidgetState.disabled);
                    }
                    if (isSelected)
                    {
                        __collection54093.Add(WidgetState.selected);
                    }
                    return __collection54093;
                }
            )
        )();
        Color? textColor = resolve(
            (theme) => isCurrentYear ? theme?.todayForegroundColor : theme?.yearForegroundColor,
            statesLocal
        );
        Color? background = resolve(
            (theme) => isCurrentYear ? theme?.todayBackgroundColor : theme?.yearBackgroundColor,
            statesLocal
        );
        WidgetStateProperty<Color?> overlayColorLocal = WidgetStateProperty.resolveWith(
            (states) => effectiveValue((theme) => theme?.yearOverlayColor?.resolve(states))
        );
        OutlinedBorder yearShapeLocal = resolve((theme) => theme?.yearShape, statesLocal)!;
        BorderSide? borderSide = default!;
        if (isCurrentYear)
        {
            borderSide = datePickerTheme.todayBorder ?? defaultsLocal.todayBorder;
            if (borderSide is not null)
            {
                borderSide = borderSide.copyWith(color: textColor);
            }
        }
        var decorationLocal = new ShapeDecoration(
            color: background,
            shape: yearShapeLocal.copyWith(side: borderSide)
        );
        TextStyle? itemStyle = (datePickerTheme.yearStyle ?? defaultsLocal.yearStyle)?.apply(
            color: textColor
        );
        MaterialLocalizations localizations = MaterialLocalizations.of(context);
        Widget yearItem = new Center(
            child: new Container(
                decoration: decorationLocal,
                height: decorationHeight,
                width: decorationWidth,
                alignment: Alignment.center,
                child: new Widgets.Semantics(
                    selected: isSelected,
                    enabled: !isDisabled,
                    button: true,
                    child: new Text(
                        widget.calendarDelegate.formatYear(year, localizations),
                        style: itemStyle
                    )
                )
            )
        );
        if (!isDisabled)
        {
            DateTime date = widget.calendarDelegate.getMonth(
                year,
                widget.selectedDate?.Month ?? 1L
            );
            if (
                date.isBefore(
                    widget.calendarDelegate.getMonth(widget.firstDate.Year, widget.firstDate.Month)
                )
            )
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
            yearItem = DartRuntimePrimitives.ConvertValue<Widget>(
                new InkWell(
                    key: new ValueKey<long>(year),
                    onTap: () =>
                    {
                        widget.onChanged(date);
                    },
                    statesController: _statesController,
                    overlayColor: overlayColorLocal,
                    child: yearItem
                )
            );
        }
        return yearItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _itemCount
    {
        get { return widget.lastDate.Year - widget.firstDate.Year + 1L; }
    }

    public override Widget build(BuildContext context)
    {
        return new Column(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(new Divider()),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new Expanded(
                        child: new Material(
                            type: MaterialType.transparency,
                            child: GridView.CreateBuilder(
                                controller: _scrollController,
                                dragStartBehavior: widget.dragStartBehavior,
                                gridDelegate: new _YearPickerGridDelegate__calendar_date_picker(
                                    context
                                ),
                                itemBuilder: _buildYearItem,
                                itemCount: Math.Max(_itemCount, minYears),
                                padding: EdgeInsets.CreateSymmetric(
                                    horizontal: Calendar_date_pickerLibrary._yearPickerPadding
                                )
                            )
                        )
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(new Divider()),
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _YearPickerGridDelegate__calendar_date_picker : SliverGridDelegate
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _YearPickerGridDelegate__calendar_date_picker(BuildContext context)
    {
        this.context = context;
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        double textScaleFactor =
            MediaQuery
                .textScalerOf(context)
                .clamp(maxScaleFactor: 3.0)
                .scale(Calendar_date_pickerLibrary._fontSizeToScale)
            / Calendar_date_pickerLibrary._fontSizeToScale;
        long scaledYearPickerColumnCount =
            (textScaleFactor > 1.65)
                ? (Calendar_date_pickerLibrary._yearPickerColumnCount - 1L)
                : Calendar_date_pickerLibrary._yearPickerColumnCount;
        double tileWidth = Math.Max(
            (
                constraints.crossAxisExtent
                - (
                    (scaledYearPickerColumnCount - 1L)
                    * Calendar_date_pickerLibrary._yearPickerRowSpacing
                )
            ) / scaledYearPickerColumnCount,
            0.0
        );
        double scaledYearPickerRowHeight =
            (textScaleFactor > 1L)
                ? (Calendar_date_pickerLibrary._yearPickerRowHeight + ((textScaleFactor - 1L) * 9L))
                : Calendar_date_pickerLibrary._yearPickerRowHeight;
        return new SliverGridRegularTileLayout(
            childCrossAxisExtent: tileWidth,
            childMainAxisExtent: scaledYearPickerRowHeight,
            crossAxisCount: scaledYearPickerColumnCount,
            crossAxisStride: tileWidth + Calendar_date_pickerLibrary._yearPickerRowSpacing,
            mainAxisStride: scaledYearPickerRowHeight,
            reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(
                constraints.crossAxisDirection
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate) => false;
}

public static partial class Calendar_date_pickerLibrary
{
    internal static void _reportAnnouncementError(
        object exception,
        System.Diagnostics.StackTrace? stack
    )
    {
        FlutterError.reportError(
            new FlutterErrorDetails(
                exception: exception,
                stack: stack,
                library: "material library",
                context: new ErrorDescription("while sending semantics announcement")
            )
        );
    }
}
