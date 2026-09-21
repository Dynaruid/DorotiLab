// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/date_picker.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Date_pickerLibrary
{
    internal static double _kItemExtent = 32.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kPickerWidth = 320.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kPickerHeight = 216.0;
}

public static partial class Date_pickerLibrary
{
    internal static bool _kUseMagnifier = true;
}

public static partial class Date_pickerLibrary
{
    internal static double _kMagnification = 2.35 / 2.1;
}

public static partial class Date_pickerLibrary
{
    internal static double _kDatePickerPadSize = 12.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kSqueeze = 1.25;
}

public static partial class Date_pickerLibrary
{
    internal static TextStyle _kDefaultPickerTextStyle = new TextStyle(letterSpacing: -0.83);
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerMagnification = 34L / 32L;
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerMinHorizontalPadding = 30;
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerHalfColumnPadding = 4;
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerLabelPadSize = 6;
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerLabelFontSize = 17.0;
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerColumnIntrinsicWidth = 106;
}

public static partial class Date_pickerLibrary
{
    internal static TextStyle _themeTextStyle(BuildContext context, bool isValid = true)
    {
        TextStyle style = CupertinoTheme.of(context).textTheme.dateTimePickerTextStyle;
        return isValid
            ? style.copyWith(color: CupertinoDynamicColor.maybeResolve(style.color, context))
            : style.copyWith(
                color: CupertinoDynamicColor.resolve(CupertinoColors.inactiveGray, context)
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static void _animateColumnControllerToItem(
        FixedExtentScrollController controller,
        long targetItem
    )
    {
        DartRuntimePrimitives.Ignore(
            controller.animateToItem(
                targetItem,
                curve: Curves.easeInOut,
                duration: Duration.Create(milliseconds: 200L)
            )
        );
    }
}

public static partial class Date_pickerLibrary
{
    internal static Widget _startSelectionOverlay = new CupertinoPickerDefaultSelectionOverlay(
        capEndEdge: false
    );
}

public static partial class Date_pickerLibrary
{
    internal static Widget _centerSelectionOverlay = new CupertinoPickerDefaultSelectionOverlay(
        capStartEdge: false,
        capEndEdge: false
    );
}

public static partial class Date_pickerLibrary
{
    internal static Widget _endSelectionOverlay = new CupertinoPickerDefaultSelectionOverlay(
        capStartEdge: false
    );
}

public delegate Widget? SelectionOverlayBuilder(
    BuildContext context,
    long columnCount,
    long selectedIndex
);

internal class _DatePickerLayoutDelegate__date_picker : MultiChildLayoutDelegate
{
    public virtual List<double> columnWidths { get; private set; } = default!;
    public virtual long textDirectionFactor { get; private set; } = default!;
    public virtual double maxWidth { get; private set; } = default!;

    internal _DatePickerLayoutDelegate__date_picker(
        List<double> columnWidths,
        long textDirectionFactor,
        double maxWidth
    )
    {
        this.columnWidths = columnWidths;
        this.textDirectionFactor = textDirectionFactor;
        this.maxWidth = maxWidth;
    }

    public override void performLayout(Size size)
    {
        double remainingWidth = (maxWidth < size.width) ? maxWidth : size.width;
        double currentHorizontalOffset = (size.width - remainingWidth) / 2L;
        for (var i = 0L; i < checked(columnWidths.Count); i++)
        {
            remainingWidth -= columnWidths[(int)i] + (Date_pickerLibrary._kDatePickerPadSize * 2L);
        }
        for (var iLocal = 0L; iLocal < checked(columnWidths.Count); iLocal++)
        {
            long index =
                (textDirectionFactor == 1L) ? iLocal : (checked(columnWidths.Count) - iLocal - 1L);
            double childWidth =
                columnWidths[(int)index] + (Date_pickerLibrary._kDatePickerPadSize * 2L);
            if ((index == 0L) || (index == (checked(columnWidths.Count) - 1L)))
            {
                childWidth += remainingWidth / 2L;
            }
            DartRuntimePrimitives.Assert(() =>
            {
                if (childWidth < 0L)
                {
                    FlutterError.reportError(
                        new FlutterErrorDetails(
                            exception: FlutterError.Create(
                                "Insufficient horizontal space to render the "
                                    + "CupertinoDatePicker because the parent is too narrow at "
                                    + $"{size.width}px.\n"
                                    + $"An additional {-remainingWidth}px is needed to avoid "
                                    + "overlapping columns."
                            )
                        )
                    );
                }
                return true;
            });
            layoutChild(
                index,
                BoxConstraints.CreateTight(new Size(Math.Max(0.0, childWidth), size.height))
            );
            positionChild(index, new Offset(currentHorizontalOffset, 0.0));
            currentHorizontalOffset += childWidth;
        }
    }

    public override bool shouldRelayout(MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_DatePickerLayoutDelegate__date_picker)oldDelegate;
        return (!Equals(columnWidths, __oldDelegate.columnWidths))
            || (textDirectionFactor != __oldDelegate.textDirectionFactor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum CupertinoDatePickerMode
{
    time,
    date,
    dateAndTime,
    monthYear,
}

internal enum _PickerColumnType__date_picker
{
    dayOfMonth,
    month,
    year,
    date,
    hour,
    minute,
    dayPeriod,
    timeSeparator,
}

public class CupertinoDatePicker : StatefulWidget
{
    public virtual CupertinoDatePickerMode mode { get; private set; } = default!;
    public virtual DateTime initialDateTime { get; private set; } = default!;
    public virtual DateTime? minimumDate { get; private set; }
    public virtual DateTime? maximumDate { get; private set; }
    public virtual long minimumYear { get; private set; } = default!;
    public virtual long? maximumYear { get; private set; }
    public virtual long minuteInterval { get; private set; } = default!;
    public virtual bool use24hFormat { get; private set; } = default!;
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual Action<DateTime> onDateTimeChanged { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool showDayOfWeek { get; private set; } = default!;
    public virtual bool showTimeSeparator { get; private set; } = default!;
    public virtual Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual SelectionOverlayBuilder? selectionOverlayBuilder { get; private set; }
    public virtual ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoDatePicker(
        Key? key = null,
        CupertinoDatePickerMode mode = CupertinoDatePickerMode.dateAndTime,
        Action<DateTime> onDateTimeChanged = default!,
        DateTime? initialDateTime = null,
        DateTime? minimumDate = null,
        DateTime? maximumDate = null,
        long minimumYear = 1,
        long? maximumYear = null,
        long minuteInterval = 1,
        bool use24hFormat = false,
        DatePickerDateOrder? dateOrder = null,
        Color? backgroundColor = null,
        bool showDayOfWeek = false,
        bool showTimeSeparator = false,
        double? itemExtent = null,
        SelectionOverlayBuilder? selectionOverlayBuilder = null,
        Func<DateTime, bool>? selectableDayPredicate = null,
        ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate
    )
        : base(key: key)
    {
        double __itemExtent = itemExtent ?? Date_pickerLibrary._kItemExtent;
        this.mode = mode;
        this.onDateTimeChanged = onDateTimeChanged;
        this.minimumDate = minimumDate;
        this.maximumDate = maximumDate;
        this.minimumYear = minimumYear;
        this.maximumYear = maximumYear;
        this.minuteInterval = minuteInterval;
        this.use24hFormat = use24hFormat;
        this.dateOrder = dateOrder;
        this.backgroundColor = backgroundColor;
        this.showDayOfWeek = showDayOfWeek;
        this.showTimeSeparator = showTimeSeparator;
        this.itemExtent = __itemExtent;
        this.selectionOverlayBuilder = selectionOverlayBuilder;
        this.selectableDayPredicate = selectableDayPredicate;
        this.changeReportingBehavior = changeReportingBehavior;
        this.initialDateTime = initialDateTime ?? new DateTime();
        System.Diagnostics.Debug.Assert(__itemExtent > 0L);
        System.Diagnostics.Debug.Assert((minuteInterval > 0L) && ((60L % minuteInterval) == 0L));
        System.Diagnostics.Debug.Assert(
            (!Equals(mode, CupertinoDatePickerMode.dateAndTime))
                || (minimumDate is null)
                || !(initialDateTime ?? new DateTime()).isBefore(
                    (
                        (
                            minimumDate
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (!Equals(mode, CupertinoDatePickerMode.dateAndTime))
                || (maximumDate is null)
                || !(initialDateTime ?? new DateTime()).isAfter(
                    (
                        (
                            maximumDate
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (
                (!Equals(mode, CupertinoDatePickerMode.date))
                && (!Equals(mode, CupertinoDatePickerMode.monthYear))
            ) || ((minimumYear >= 1L) && ((initialDateTime ?? new DateTime()).Year >= minimumYear))
        );
        System.Diagnostics.Debug.Assert(
            (
                (!Equals(mode, CupertinoDatePickerMode.date))
                && (!Equals(mode, CupertinoDatePickerMode.monthYear))
            )
                || (maximumYear is null)
                || (
                    (initialDateTime ?? new DateTime()).Year
                    <= (
                        maximumYear
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
        );
        System.Diagnostics.Debug.Assert(
            (
                (!Equals(mode, CupertinoDatePickerMode.date))
                && (!Equals(mode, CupertinoDatePickerMode.monthYear))
            )
                || (minimumDate is null)
                || !(
                    minimumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).isAfter(initialDateTime ?? new DateTime())
        );
        System.Diagnostics.Debug.Assert(
            (
                (!Equals(mode, CupertinoDatePickerMode.date))
                && (!Equals(mode, CupertinoDatePickerMode.monthYear))
            )
                || (maximumDate is null)
                || !(
                    maximumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ).isBefore(initialDateTime ?? new DateTime())
        );
        System.Diagnostics.Debug.Assert(
            Equals(mode, CupertinoDatePickerMode.date) || !showDayOfWeek
        );
        System.Diagnostics.Debug.Assert(
            ((initialDateTime ?? new DateTime()).Minute % minuteInterval) == 0L
        );
        System.Diagnostics.Debug.Assert(
            !showTimeSeparator
                || Equals(mode, CupertinoDatePickerMode.dateAndTime)
                || Equals(mode, CupertinoDatePickerMode.time)
        );
        System.Diagnostics.Debug.Assert(
            (selectableDayPredicate is null)
                || (initialDateTime is null)
                || selectableDayPredicate(
                    (
                        initialDateTime
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
        );
    }

    public override IState createState()
    {
        return mode switch
        {
            CupertinoDatePickerMode.time => DartRuntimePrimitives.ConvertValue<
                State<CupertinoDatePicker>
            >(new _CupertinoDatePickerDateTimeState__date_picker()),
            CupertinoDatePickerMode.dateAndTime => DartRuntimePrimitives.ConvertValue<
                State<CupertinoDatePicker>
            >(new _CupertinoDatePickerDateTimeState__date_picker()),
            CupertinoDatePickerMode.date => DartRuntimePrimitives.ConvertValue<
                State<CupertinoDatePicker>
            >(new _CupertinoDatePickerDateState__date_picker(dateOrder: dateOrder)),
            CupertinoDatePickerMode.monthYear => DartRuntimePrimitives.ConvertValue<
                State<CupertinoDatePicker>
            >(new _CupertinoDatePickerMonthYearState__date_picker(dateOrder: dateOrder)),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _getColumnWidth(
        _PickerColumnType__date_picker columnType,
        CupertinoLocalizations localizations,
        BuildContext context,
        bool showDayOfWeek,
        bool standaloneMonth = false
    )
    {
        var longTexts = new List<string>();
        switch (columnType)
        {
            case _PickerColumnType__date_picker.date:
            {
                for (var i = 1L; i <= 12L; i++)
                {
                    string dateLocal = localizations.datePickerMediumDate(
                        DartRuntimePrimitives.CreateDateTime(2018L, i, 25L)
                    );
                    longTexts.Add(dateLocal);
                }
                break;
            }
            case _PickerColumnType__date_picker.hour:
            {
                for (var iLocal = 0L; iLocal < 24L; iLocal++)
                {
                    string hourLocal = localizations.datePickerHour(iLocal);
                    longTexts.Add(hourLocal);
                }
                break;
            }
            case _PickerColumnType__date_picker.minute:
            {
                for (var iAlternate = 0L; iAlternate < 60L; iAlternate++)
                {
                    string minuteLocal = localizations.datePickerMinute(iAlternate);
                    longTexts.Add(minuteLocal);
                }
                break;
            }
            case _PickerColumnType__date_picker.dayPeriod:
            {
                longTexts.Add(localizations.anteMeridiemAbbreviation);
                longTexts.Add(localizations.postMeridiemAbbreviation);
                break;
            }
            case _PickerColumnType__date_picker.dayOfMonth:
            {
                var longestDayOfMonth = 1L;
                for (var iNested = 1L; iNested <= 31L; iNested++)
                {
                    string dayOfMonthLocal = localizations.datePickerDayOfMonth(iNested);
                    longTexts.Add(dayOfMonthLocal);
                    longestDayOfMonth = iNested;
                }
                if (showDayOfWeek)
                {
                    for (var wd = 1L; wd < 7L; wd++)
                    {
                        string dayOfMonthAlternate = localizations.datePickerDayOfMonth(
                            longestDayOfMonth,
                            wd
                        );
                        longTexts.Add(dayOfMonthAlternate);
                    }
                }
                break;
            }
            case _PickerColumnType__date_picker.month:
            {
                for (var iCurrent = 1L; iCurrent <= 12L; iCurrent++)
                {
                    string monthLocal = standaloneMonth
                        ? localizations.datePickerStandaloneMonth(iCurrent)
                        : localizations.datePickerMonth(iCurrent);
                    longTexts.Add(monthLocal);
                }
                break;
            }
            case _PickerColumnType__date_picker.year:
            {
                longTexts.Add(localizations.datePickerYear(2018L));
                break;
            }
            case _PickerColumnType__date_picker.timeSeparator:
            {
                longTexts.Add(":");
                break;
            }
        }
        DartRuntimePrimitives.Assert(
            () => Enumerable.Any(longTexts) && longTexts.All((text) => text.Length != 0),
            () => (object?)"column type is not appropriate"
        );
        return getColumnWidth(texts: longTexts, context: context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static double getColumnWidth(
        List<string> texts,
        BuildContext context,
        TextStyle? textStyle = null
    )
    {
        return texts
            .map(
                (text) =>
                    TextPainter.computeMaxIntrinsicWidth(
                        text: new TextSpan(
                            style: textStyle ?? Date_pickerLibrary._themeTextStyle(context),
                            text: text
                        ),
                        textDirection: Directionality.of(context)
                    )
            )
            .reduce(Dart_mathLibrary.max);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate Widget _ColumnBuilder__date_picker(
    double offAxisFraction,
    Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
    Widget? selectionOverlay
);

internal class _CupertinoDatePickerDateTimeState__date_picker : State<CupertinoDatePicker>
{
    internal const double _kMaximumOffAxisFraction = 0.45;
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual Alignment alignCenterLeft { get; set; } = default!;
    public virtual Alignment alignCenterRight { get; set; } = default!;
    public virtual DateTime initialDateTime { get; set; } = default!;
    public virtual FixedExtentScrollController dateController { get; set; } = default!;
    public virtual FixedExtentScrollController hourController { get; set; } = default!;
    public virtual FixedExtentScrollController minuteController { get; set; } = default!;
    public virtual long selectedAmPm { get; set; } = default!;
    public virtual long meridiemRegion { get; set; } = default!;
    public virtual FixedExtentScrollController meridiemController { get; set; } = default!;
    public virtual bool isDatePickerScrolling { get; set; } = false;
    public virtual bool isHourPickerScrolling { get; set; } = false;
    public virtual bool isMinutePickerScrolling { get; set; } = false;
    public virtual bool isMeridiemPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; private set; } =
        new DartMap<long, double>();

    public virtual long selectedDayFromInitial
    {
        get
        {
            switch (widget.mode)
            {
                case CupertinoDatePickerMode.dateAndTime:
                {
                    return dateController.hasClients ? dateController.selectedItem : 0L;
                }
                case CupertinoDatePickerMode.time:
                {
                    return 0L;
                }
                case CupertinoDatePickerMode.date:
                case CupertinoDatePickerMode.monthYear:
                {
                    break;
                }
                default:
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    );
            }
            DartRuntimePrimitives.Assert(
                () => false,
                () => (object?)$"{GetType()} is only meant for dateAndTime mode or time mode"
            );
            return 0L;
        }
    }
    public virtual long selectedHour => _selectedHour(selectedAmPm, _selectedHourIndex);
    internal virtual long _selectedHourIndex =>
        hourController.hasClients ? (hourController.selectedItem % 24L) : initialDateTime.Hour;

    internal virtual long _selectedHour(long selectedAmPm, long selectedHour)
    {
        return _isHourRegionFlipped(selectedAmPm) ? ((selectedHour + 12L) % 24L) : selectedHour;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long selectedMinute
    {
        get
        {
            return minuteController.hasClients
                ? (minuteController.selectedItem * widget.minuteInterval % 60L)
                : initialDateTime.Minute;
        }
    }
    public virtual bool isHourRegionFlipped => _isHourRegionFlipped(selectedAmPm);

    internal virtual bool _isHourRegionFlipped(long selectedAmPm) =>
        DartRuntimePrimitives.ConvertValue<bool>(selectedAmPm != meridiemRegion);

    public virtual bool isScrolling
    {
        get
        {
            return isDatePickerScrolling
                || isHourPickerScrolling
                || isMinutePickerScrolling
                || isMeridiemPickerScrolling;
        }
    }

    public override void initState()
    {
        base.initState();
        initialDateTime = widget.initialDateTime;
        selectedAmPm = checked(initialDateTime.Hour / 12L);
        meridiemRegion = selectedAmPm;
        meridiemController = new FixedExtentScrollController(initialItem: selectedAmPm);
        hourController = new FixedExtentScrollController(initialItem: initialDateTime.Hour);
        minuteController = new FixedExtentScrollController(
            initialItem: checked(initialDateTime.Minute / widget.minuteInterval)
        );
        dateController = new FixedExtentScrollController();
        PaintingBinding.instance.systemFonts.addListener(_handleSystemFontsChange);
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(() =>
        {
            estimatedColumnWidths.Clear();
        });
    }

    public override void dispose()
    {
        dateController.dispose();
        hourController.dispose();
        minuteController.dispose();
        meridiemController.dispose();
        PaintingBinding.instance.systemFonts.removeListener(_handleSystemFontsChange);
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoDatePicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(
            () => Equals(oldWidget.mode, widget.mode),
            () => (object?)$"The {GetType()}'s mode cannot change once it's built."
        );
        if (!widget.use24hFormat && oldWidget.use24hFormat)
        {
            meridiemController.dispose();
            meridiemController = new FixedExtentScrollController(initialItem: selectedAmPm);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = Equals(Directionality.of(context), TextDirection.ltr) ? 1L : -1L;
        localizations = CupertinoLocalizations.of(context);
        alignCenterLeft =
            (textDirectionFactor == 1L) ? Alignment.centerLeft : Alignment.centerRight;
        alignCenterRight =
            (textDirectionFactor == 1L) ? Alignment.centerRight : Alignment.centerLeft;
        estimatedColumnWidths.Clear();
    }

    internal virtual double _getEstimatedColumnWidth(_PickerColumnType__date_picker columnType)
    {
        estimatedColumnWidths.putIfAbsent(
            FoundationRuntimePorts.EnumIndex(columnType),
            () =>
                CupertinoDatePicker._getColumnWidth(
                    columnType,
                    localizations,
                    context,
                    widget.showDayOfWeek
                )
        );
        return (
            DartCollectionRuntime.NullableMapValue<double>(
                estimatedColumnWidths,
                FoundationRuntimePorts.EnumIndex(columnType)
            ) ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DateTime selectedDateTime
    {
        get
        {
            return DartRuntimePrimitives.CreateDateTime(
                initialDateTime.Year,
                initialDateTime.Month,
                initialDateTime.Day + selectedDayFromInitial,
                selectedHour,
                selectedMinute
            );
        }
    }

    internal virtual void _onSelectedItemChange(long index)
    {
        bool isDateInvalid =
            (widget.minimumDate?.isAfter(selectedDateTime) ?? false)
            || (widget.maximumDate?.isBefore(selectedDateTime) ?? false);
        if (isDateInvalid)
        {
            return;
        }
        else
        {
            if (!_isSelectableDate(selectedDateTime))
            {
                return;
            }
        }
        widget.onDateTimeChanged(selectedDateTime);
    }

    internal virtual bool _isSelectableDate(DateTime date)
    {
        return widget.selectableDayPredicate is null
            ? true
            : widget.selectableDayPredicate.Invoke(date);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildMediumDatePicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isDatePickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isDatePickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: CupertinoPicker.CreateBuilder(
                scrollController: dateController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    _onSelectedItemChange(index);
                },
                itemBuilder: (context, index) =>
                {
                    var rangeStart = DartRuntimePrimitives.CreateDateTime(
                        initialDateTime.Year,
                        initialDateTime.Month,
                        initialDateTime.Day + index
                    );
                    var rangeEnd = DartRuntimePrimitives.CreateDateTime(
                        initialDateTime.Year,
                        initialDateTime.Month,
                        initialDateTime.Day + index + 1L
                    );
                    var now = new DateTime();
                    if (widget.minimumDate?.isBefore(rangeEnd) == false)
                    {
                        return null;
                    }
                    if (widget.maximumDate?.isAfter(rangeStart) == false)
                    {
                        return null;
                    }
                    string dateText = Equals(
                        rangeStart,
                        DartRuntimePrimitives.CreateDateTime(now.Year, now.Month, now.Day)
                    )
                        ? localizations.todayLabel
                        : localizations.datePickerMediumDate(rangeStart);
                    bool isDisabled = !_isSelectableDate(rangeStart);
                    Widget childLocal = itemPositioningBuilder(
                        context,
                        new Text(
                            dateText,
                            style: Date_pickerLibrary._themeTextStyle(context, isValid: !isDisabled)
                        )
                    );
                    return isDisabled ? new ExcludeSemantics(child: childLocal) : childLocal;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                selectionOverlay: selectionOverlay
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isValidHour(long meridiemIndex, long hourIndex)
    {
        var rangeStart = DartRuntimePrimitives.CreateDateTime(
            initialDateTime.Year,
            initialDateTime.Month,
            initialDateTime.Day + selectedDayFromInitial,
            _selectedHour(meridiemIndex, hourIndex)
        );
        DateTime rangeEnd = rangeStart.add(Duration.Create(hours: 1L));
        return (widget.minimumDate?.isBefore(rangeEnd) ?? true)
            && !(widget.maximumDate?.isBefore(rangeStart) ?? false);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildHourPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isHourPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isHourPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: hourController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    var regionChanged = meridiemRegion != checked(index / 12L);
                    bool debugIsFlipped = isHourRegionFlipped;
                    if (regionChanged)
                    {
                        meridiemRegion = checked(index / 12L);
                        selectedAmPm = 1L - selectedAmPm;
                    }
                    if (!widget.use24hFormat && regionChanged)
                    {
                        DartRuntimePrimitives.Ignore(
                            meridiemController.animateToItem(
                                selectedAmPm,
                                duration: Duration.Create(milliseconds: 300L),
                                curve: Curves.easeOut
                            )
                        );
                    }
                    else
                    {
                        _onSelectedItemChange(index);
                    }
                    DartRuntimePrimitives.Assert(() => debugIsFlipped == isHourRegionFlipped);
                },
                looping: true,
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)24L)),
                        (index) =>
                        {
                            long hour = isHourRegionFlipped ? ((index + 12L) % 24L) : index;
                            long displayHour = widget.use24hFormat
                                ? hour
                                : (((hour + 11L) % 12L) + 1L);
                            bool isDisabled = !_isValidHour(selectedAmPm, index);
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    localizations.datePickerHour(displayHour),
                                    semanticsLabel: localizations.datePickerHourSemanticsLabel(
                                        displayHour
                                    ),
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isDisabled
                                    )
                                )
                            );
                            return isDisabled
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildMinutePicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isMinutePickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isMinutePickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: minuteController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: _onSelectedItemChange,
                looping: true,
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)checked(60L / widget.minuteInterval))),
                        (index) =>
                        {
                            long minute = index * widget.minuteInterval;
                            var date = DartRuntimePrimitives.CreateDateTime(
                                initialDateTime.Year,
                                initialDateTime.Month,
                                initialDateTime.Day + selectedDayFromInitial,
                                selectedHour,
                                minute
                            );
                            bool isInvalidMinute =
                                (widget.minimumDate?.isAfter(date) ?? false)
                                || (widget.maximumDate?.isBefore(date) ?? false);
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    localizations.datePickerMinute(minute),
                                    semanticsLabel: localizations.datePickerMinuteSemanticsLabel(
                                        minute
                                    ),
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isInvalidMinute
                                    )
                                )
                            );
                            return isInvalidMinute
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildAmPmPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isMeridiemPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isMeridiemPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: meridiemController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedAmPm = index;
                    DartRuntimePrimitives.Assert(() =>
                        (selectedAmPm == 0L) || (selectedAmPm == 1L)
                    );
                    _onSelectedItemChange(index);
                },
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)2L)),
                        (index) =>
                        {
                            bool isDisabled = !_isValidHour(index, _selectedHourIndex);
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    (index == 0L)
                                        ? localizations.anteMeridiemAbbreviation
                                        : localizations.postMeridiemAbbreviation,
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isDisabled
                                    )
                                )
                            );
                            return isDisabled
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildTimeSeparatorWidget(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new ExcludeSemantics(
            child: new CupertinoPicker(
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                onSelectedItemChanged: (index) => { },
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)1L)),
                        (index) =>
                        {
                            return itemPositioningBuilder(
                                context,
                                new Text(":", style: Date_pickerLibrary._themeTextStyle(context))
                            );
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _scrollToFirstSelectableDate()
    {
        if (!_isSelectableDate(selectedDateTime))
        {
            var daysThreshold = 1L;
            DateTime targetDate = selectedDateTime.add(Duration.Create(days: daysThreshold));
            _scrollToDate(
                targetDate,
                selectedDateTime,
                false,
                focusedIndex: dateController.selectedItem + daysThreshold
            );
        }
    }

    internal virtual void _pickerDidStopScrolling()
    {
        setState(() => { });
        if (isScrolling)
        {
            return;
        }
        DateTime selectedDate = selectedDateTime;
        bool minCheck = widget.minimumDate?.isAfter(selectedDate) ?? false;
        bool maxCheck = widget.maximumDate?.isBefore(selectedDate) ?? false;
        _scrollToFirstSelectableDate();
        if (minCheck || maxCheck)
        {
            DateTime targetDate = minCheck
                ? (
                    widget.minimumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
                : (
                    widget.maximumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            _scrollToDate(targetDate, selectedDate, minCheck);
        }
    }

    internal virtual void _scrollToDate(
        DateTime newDate,
        DateTime fromDate,
        bool minCheck,
        long? focusedIndex = null
    )
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (timestamp) =>
            {
                if (
                    (fromDate.Year != newDate.Year)
                    || (fromDate.Month != newDate.Month)
                    || (fromDate.Day != newDate.Day)
                )
                {
                    Date_pickerLibrary._animateColumnControllerToItem(
                        dateController,
                        focusedIndex ?? selectedDayFromInitial
                    );
                }
                if (fromDate.Hour != newDate.Hour)
                {
                    bool needsMeridiemChange =
                        !widget.use24hFormat
                        && (checked(fromDate.Hour / 12L) != checked(newDate.Hour / 12L));
                    if (needsMeridiemChange)
                    {
                        Date_pickerLibrary._animateColumnControllerToItem(
                            meridiemController,
                            1L - meridiemController.selectedItem
                        );
                        long newItem =
                            (checked(hourController.selectedItem / 12L) * 12L)
                            + ((hourController.selectedItem + newDate.Hour - fromDate.Hour) % 12L);
                        Date_pickerLibrary._animateColumnControllerToItem(hourController, newItem);
                    }
                    else
                    {
                        Date_pickerLibrary._animateColumnControllerToItem(
                            hourController,
                            hourController.selectedItem + newDate.Hour - fromDate.Hour
                        );
                    }
                }
                if (fromDate.Minute != newDate.Minute)
                {
                    double positionDouble = newDate.Minute / widget.minuteInterval;
                    long position = minCheck ? positionDouble.ceil() : positionDouble.floor();
                    Date_pickerLibrary._animateColumnControllerToItem(minuteController, position);
                }
            },
            debugLabel: "DatePicker.scrollToDate"
        );
    }

    public override Widget build(BuildContext context)
    {
        var columnWidthsLocal = new List<double>
        {
            _getEstimatedColumnWidth(_PickerColumnType__date_picker.hour),
            _getEstimatedColumnWidth(_PickerColumnType__date_picker.minute),
        };
        var pickerBuilders = Equals(Directionality.of(context), TextDirection.rtl)
            ? new List<Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>>
            {
                _buildMinutePicker,
                _buildHourPicker,
            }
            : new List<Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>>
            {
                _buildHourPicker,
                _buildMinutePicker,
            };
        if (widget.showTimeSeparator)
        {
            columnWidthsLocal.Insert(
                checked((int)1L),
                _getEstimatedColumnWidth(_PickerColumnType__date_picker.timeSeparator)
            );
            pickerBuilders.Insert(checked((int)1L), _buildTimeSeparatorWidget);
        }
        if (!widget.use24hFormat)
        {
            switch (localizations.datePickerDateTimeOrder)
            {
                case var __constant47363
                    when Equals(__constant47363, DatePickerDateTimeOrder.date_time_dayPeriod):
                case var __constant47421
                    when Equals(__constant47421, DatePickerDateTimeOrder.time_dayPeriod_date):
                {
                    pickerBuilders.Add(_buildAmPmPicker);
                    columnWidthsLocal.Add(
                        _getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod)
                    );
                    break;
                }
                case var __constant47610
                    when Equals(__constant47610, DatePickerDateTimeOrder.date_dayPeriod_time):
                case var __constant47668
                    when Equals(__constant47668, DatePickerDateTimeOrder.dayPeriod_time_date):
                {
                    pickerBuilders.Insert(checked((int)0L), _buildAmPmPicker);
                    columnWidthsLocal.Insert(
                        checked((int)0L),
                        _getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod)
                    );
                    break;
                }
            }
        }
        if (Equals(widget.mode, CupertinoDatePickerMode.dateAndTime))
        {
            switch (localizations.datePickerDateTimeOrder)
            {
                case var __constant48071
                    when Equals(__constant48071, DatePickerDateTimeOrder.time_dayPeriod_date):
                case var __constant48129
                    when Equals(__constant48129, DatePickerDateTimeOrder.dayPeriod_time_date):
                {
                    pickerBuilders.Add(_buildMediumDatePicker);
                    columnWidthsLocal.Add(
                        _getEstimatedColumnWidth(_PickerColumnType__date_picker.date)
                    );
                    break;
                }
                case var __constant48319
                    when Equals(__constant48319, DatePickerDateTimeOrder.date_time_dayPeriod):
                case var __constant48377
                    when Equals(__constant48377, DatePickerDateTimeOrder.date_dayPeriod_time):
                {
                    pickerBuilders.Insert(checked((int)0L), _buildMediumDatePicker);
                    columnWidthsLocal.Insert(
                        checked((int)0L),
                        _getEstimatedColumnWidth(_PickerColumnType__date_picker.date)
                    );
                    break;
                }
            }
        }
        var pickers = new List<Widget>();
        double totalColumnWidths = 4L * Date_pickerLibrary._kDatePickerPadSize;
        foreach (var (i, width) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = (i == 0L, i == (checked(columnWidthsLocal.Count) - 1L));
            var offAxisFraction = 0.0;
            Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if (widget.selectionOverlayBuilder is not null)
            {
                selectionOverlay = widget.selectionOverlayBuilder!(
                    context,
                    selectedIndex: i,
                    columnCount: checked(columnWidthsLocal.Count)
                );
            }
            else
            {
                if (firstColumn)
                {
                    selectionOverlay = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn)
                    {
                        selectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            if (firstColumn)
            {
                offAxisFraction = -_kMaximumOffAxisFraction * textDirectionFactor;
            }
            else
            {
                if ((i >= 2L) || (checked(columnWidthsLocal.Count) == 2L))
                {
                    offAxisFraction = _kMaximumOffAxisFraction * textDirectionFactor;
                }
            }
            var paddingLocal = EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if (lastColumn)
            {
                paddingLocal = paddingLocal.flipped;
            }
            if (textDirectionFactor == -1L)
            {
                paddingLocal = paddingLocal.flipped;
            }
            totalColumnWidths += width + (2L * Date_pickerLibrary._kDatePickerPadSize);
            pickers.Add(
                new LayoutId(
                    id: i,
                    child: pickerBuilders[(int)i]
                        (
                            offAxisFraction,
                            (context, child) =>
                            {
                                Widget constrained = new ConstrainedBox(
                                    constraints: new BoxConstraints(
                                        maxWidth: width + Date_pickerLibrary._kDatePickerPadSize
                                    ),
                                    child: child
                                );
                                return new Padding(
                                    padding: paddingLocal,
                                    child: new Align(
                                        alignment: lastColumn ? alignCenterLeft : alignCenterRight,
                                        child: (firstColumn || lastColumn) ? constrained : child
                                    )
                                );
                                throw new InvalidOperationException(
                                    "Callback completed without returning a value."
                                );
                            },
                            selectionOverlay
                        )
                )
            );
        }
        double maxPickerWidth =
            (totalColumnWidths > Date_pickerLibrary._kPickerWidth)
                ? totalColumnWidths
                : Date_pickerLibrary._kPickerWidth;
        return MediaQuery.withNoTextScaling(
            child: DefaultTextStyle.merge(
                style: Date_pickerLibrary._kDefaultPickerTextStyle,
                child: new CustomMultiChildLayout(
                    @delegate: new _DatePickerLayoutDelegate__date_picker(
                        columnWidths: columnWidthsLocal,
                        textDirectionFactor: textDirectionFactor,
                        maxWidth: maxPickerWidth
                    ),
                    children: pickers
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CupertinoDatePickerDateState__date_picker : State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual Alignment alignCenterLeft { get; set; } = default!;
    public virtual Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedDay { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual FixedExtentScrollController dayController { get; set; } = default!;
    public virtual FixedExtentScrollController monthController { get; set; } = default!;
    public virtual FixedExtentScrollController yearController { get; set; } = default!;
    public virtual bool isDayPickerScrolling { get; set; } = false;
    public virtual bool isMonthPickerScrolling { get; set; } = false;
    public virtual bool isYearPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; set; } =
        new DartMap<long, double>();

    internal _CupertinoDatePickerDateState__date_picker(DatePickerDateOrder? dateOrder)
    {
        this.dateOrder = dateOrder;
    }

    public virtual bool isScrolling =>
        DartRuntimePrimitives.ConvertValue<bool>(
            isDayPickerScrolling || isMonthPickerScrolling || isYearPickerScrolling
        );

    public override void initState()
    {
        base.initState();
        selectedDay = widget.initialDateTime.Day;
        selectedMonth = widget.initialDateTime.Month;
        selectedYear = widget.initialDateTime.Year;
        dayController = new FixedExtentScrollController(initialItem: selectedDay - 1L);
        monthController = new FixedExtentScrollController(initialItem: selectedMonth - 1L);
        yearController = new FixedExtentScrollController(initialItem: selectedYear);
        PaintingBinding.instance.systemFonts.addListener(_handleSystemFontsChange);
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(() =>
        {
            _refreshEstimatedColumnWidths();
        });
    }

    public override void dispose()
    {
        dayController.dispose();
        monthController.dispose();
        yearController.dispose();
        PaintingBinding.instance.systemFonts.removeListener(_handleSystemFontsChange);
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = Equals(Directionality.of(context), TextDirection.ltr) ? 1L : -1L;
        localizations = CupertinoLocalizations.of(context);
        alignCenterLeft =
            (textDirectionFactor == 1L) ? Alignment.centerLeft : Alignment.centerRight;
        alignCenterRight =
            (textDirectionFactor == 1L) ? Alignment.centerRight : Alignment.centerLeft;
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        estimatedColumnWidths[
            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth)
        ] = CupertinoDatePicker._getColumnWidth(
            _PickerColumnType__date_picker.dayOfMonth,
            localizations,
            context,
            widget.showDayOfWeek
        );
        estimatedColumnWidths[
            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
        ] = CupertinoDatePicker._getColumnWidth(
            _PickerColumnType__date_picker.month,
            localizations,
            context,
            widget.showDayOfWeek
        );
        estimatedColumnWidths[
            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
        ] = CupertinoDatePicker._getColumnWidth(
            _PickerColumnType__date_picker.year,
            localizations,
            context,
            widget.showDayOfWeek
        );
    }

    internal virtual DateTime _lastDayInMonth(long year, long month) =>
        DartRuntimePrimitives.CreateDateTime(year, month + 1L, 0L);

    internal virtual Widget _buildDayPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        long daysInCurrentMonth = _lastDayInMonth(selectedYear, selectedMonth).Day;
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isDayPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isDayPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: dayController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedDay = index + 1L;
                    if (_isCurrentDateValid)
                    {
                        widget.onDateTimeChanged(
                            DartRuntimePrimitives.CreateDateTime(
                                selectedYear,
                                selectedMonth,
                                selectedDay
                            )
                        );
                    }
                },
                looping: true,
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)31L)),
                        (index) =>
                        {
                            long day = index + 1L;
                            long? dayOfWeek = widget.showDayOfWeek
                                ? DartRuntimePrimitives
                                    .CreateDateTime(selectedYear, selectedMonth, day)
                                    .DayOfWeek.ToDartWeekday()
                                : null;
                            bool isInvalidDay =
                                day > daysInCurrentMonth
                                || (
                                    (widget.minimumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.minimumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month == selectedMonth
                                    )
                                    && (
                                        (
                                            widget.minimumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Day > day
                                    )
                                )
                                || (
                                    (widget.maximumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.maximumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month == selectedMonth
                                    )
                                    && (
                                        (
                                            widget.maximumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Day < day
                                    )
                                );
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    localizations.datePickerDayOfMonth(day, dayOfWeek),
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isInvalidDay
                                    )
                                )
                            );
                            return isInvalidDay
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildMonthPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isMonthPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isMonthPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: monthController,
                offAxisFraction: offAxisFraction,
                itemExtent: widget.itemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedMonth = index + 1L;
                    if (_isCurrentDateValid)
                    {
                        widget.onDateTimeChanged(
                            DartRuntimePrimitives.CreateDateTime(
                                selectedYear,
                                selectedMonth,
                                selectedDay
                            )
                        );
                    }
                },
                looping: true,
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)12L)),
                        (index) =>
                        {
                            long month = index + 1L;
                            bool isInvalidMonth =
                                (
                                    (widget.minimumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.minimumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month > month
                                    )
                                )
                                || (
                                    (widget.maximumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.maximumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month < month
                                    )
                                );
                            string monthName = Equals(
                                widget.mode,
                                CupertinoDatePickerMode.monthYear
                            )
                                ? localizations.datePickerStandaloneMonth(month)
                                : localizations.datePickerMonth(month);
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    monthName,
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isInvalidMonth
                                    )
                                )
                            );
                            return isInvalidMonth
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildYearPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isYearPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isYearPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: CupertinoPicker.CreateBuilder(
                scrollController: yearController,
                itemExtent: widget.itemExtent,
                offAxisFraction: offAxisFraction,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedYear = index;
                    if (_isCurrentDateValid)
                    {
                        widget.onDateTimeChanged(
                            DartRuntimePrimitives.CreateDateTime(
                                selectedYear,
                                selectedMonth,
                                selectedDay
                            )
                        );
                    }
                },
                itemBuilder: (context, year) =>
                {
                    if (year < widget.minimumYear)
                    {
                        return null;
                    }
                    if (
                        (widget.maximumYear is not null)
                        && (
                            year
                            > (
                                widget.maximumYear
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )
                    {
                        return null;
                    }
                    bool isValidYear =
                        (
                            (widget.minimumDate is null)
                            || (
                                (
                                    widget.minimumDate
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ).Year <= year
                            )
                        )
                        && (
                            (widget.maximumDate is null)
                            || (
                                (
                                    widget.maximumDate
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ).Year >= year
                            )
                        );
                    Widget childLocal = itemPositioningBuilder(
                        context,
                        new Text(
                            localizations.datePickerYear(year),
                            style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear)
                        )
                    );
                    return isValidYear ? childLocal : new ExcludeSemantics(child: childLocal);
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                selectionOverlay: selectionOverlay
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate = DartRuntimePrimitives.CreateDateTime(
                selectedYear,
                selectedMonth,
                selectedDay
            );
            var maxSelectedDate = DartRuntimePrimitives.CreateDateTime(
                selectedYear,
                selectedMonth,
                selectedDay + 1L
            );
            bool minCheck = widget.minimumDate?.isBefore(maxSelectedDate) ?? true;
            bool maxCheck = widget.maximumDate?.isBefore(minSelectedDate) ?? false;
            return minCheck && !maxCheck && (minSelectedDate.Day == selectedDay);
        }
    }

    internal virtual void _pickerDidStopScrolling()
    {
        setState(() => { });
        if (isScrolling)
        {
            return;
        }
        var minSelectDate = DartRuntimePrimitives.CreateDateTime(
            selectedYear,
            selectedMonth,
            selectedDay
        );
        var maxSelectDate = DartRuntimePrimitives.CreateDateTime(
            selectedYear,
            selectedMonth,
            selectedDay + 1L
        );
        bool minCheck = widget.minimumDate?.isBefore(maxSelectDate) ?? true;
        bool maxCheck = widget.maximumDate?.isBefore(minSelectDate) ?? false;
        if (!minCheck || maxCheck)
        {
            DateTime targetDate = minCheck
                ? (
                    widget.maximumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
                : (
                    widget.minimumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            _scrollToDate(targetDate);
            return;
        }
        if (minSelectDate.Day != selectedDay)
        {
            DateTime lastDay = _lastDayInMonth(selectedYear, selectedMonth);
            _scrollToDate(lastDay);
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (timestamp) =>
            {
                if (selectedYear != newDate.Year)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(yearController, newDate.Year);
                }
                if (selectedMonth != newDate.Month)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(
                        monthController,
                        newDate.Month - 1L
                    );
                }
                if (selectedDay != newDate.Day)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(
                        dayController,
                        newDate.Day - 1L
                    );
                }
            },
            debugLabel: "DatePicker.scrollToDate"
        );
    }

    public override Widget build(BuildContext context)
    {
        var pickerBuilders =
            new List<Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>>();
        var columnWidthsLocal = new List<double>();
        DatePickerDateOrder datePickerDateOrderLocal =
            dateOrder ?? localizations.datePickerDateOrder;
        switch (datePickerDateOrderLocal)
        {
            case var __constant63400 when Equals(__constant63400, DatePickerDateOrder.mdy):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildMonthPicker,
                    _buildDayPicker,
                    _buildYearPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(
                                _PickerColumnType__date_picker.dayOfMonth
                            )
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
            case var __constant63776 when Equals(__constant63776, DatePickerDateOrder.dmy):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildDayPicker,
                    _buildMonthPicker,
                    _buildYearPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(
                                _PickerColumnType__date_picker.dayOfMonth
                            )
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
            case var __constant64152 when Equals(__constant64152, DatePickerDateOrder.ymd):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildYearPicker,
                    _buildMonthPicker,
                    _buildDayPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(
                                _PickerColumnType__date_picker.dayOfMonth
                            )
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
            case var __constant64528 when Equals(__constant64528, DatePickerDateOrder.ydm):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildYearPicker,
                    _buildDayPicker,
                    _buildMonthPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(
                                _PickerColumnType__date_picker.dayOfMonth
                            )
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
        }
        var pickers = new List<Widget>();
        double totalColumnWidths = 4L * Date_pickerLibrary._kDatePickerPadSize;
        foreach (var (i, widthLocal) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = (i == 0L, i == (checked(columnWidthsLocal.Count) - 1L));
            double offAxisFraction = (i - 1L) * 0.3 * textDirectionFactor;
            var paddingLocal = EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if (textDirectionFactor == -1L)
            {
                paddingLocal = EdgeInsets.CreateOnly(left: Date_pickerLibrary._kDatePickerPadSize);
            }
            Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if (widget.selectionOverlayBuilder is not null)
            {
                selectionOverlay = widget.selectionOverlayBuilder!(
                    context,
                    selectedIndex: i,
                    columnCount: checked(columnWidthsLocal.Count)
                );
            }
            else
            {
                if (firstColumn)
                {
                    selectionOverlay = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn)
                    {
                        selectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            totalColumnWidths += widthLocal + (2L * Date_pickerLibrary._kDatePickerPadSize);
            pickers.Add(
                new LayoutId(
                    id: i,
                    child: pickerBuilders[(int)i]
                        (
                            offAxisFraction,
                            (context, child) =>
                            {
                                return new Padding(
                                    padding: firstColumn ? EdgeInsets.zero : paddingLocal,
                                    child: new Align(
                                        alignment: lastColumn ? alignCenterLeft : alignCenterRight,
                                        child: new SizedBox(
                                            width: widthLocal
                                                + Date_pickerLibrary._kDatePickerPadSize,
                                            child: new Align(
                                                alignment: firstColumn
                                                    ? alignCenterLeft
                                                    : alignCenterRight,
                                                child: child
                                            )
                                        )
                                    )
                                );
                                throw new InvalidOperationException(
                                    "Callback completed without returning a value."
                                );
                            },
                            selectionOverlay
                        )
                )
            );
        }
        double maxPickerWidth =
            (totalColumnWidths > Date_pickerLibrary._kPickerWidth)
                ? totalColumnWidths
                : Date_pickerLibrary._kPickerWidth;
        return MediaQuery.withNoTextScaling(
            child: DefaultTextStyle.merge(
                style: Date_pickerLibrary._kDefaultPickerTextStyle,
                child: new CustomMultiChildLayout(
                    @delegate: new _DatePickerLayoutDelegate__date_picker(
                        columnWidths: columnWidthsLocal,
                        textDirectionFactor: textDirectionFactor,
                        maxWidth: maxPickerWidth
                    ),
                    children: pickers
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _CupertinoDatePickerMonthYearState__date_picker : State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual Alignment alignCenterLeft { get; set; } = default!;
    public virtual Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual FixedExtentScrollController monthController { get; set; } = default!;
    public virtual FixedExtentScrollController yearController { get; set; } = default!;
    public virtual bool isMonthPickerScrolling { get; set; } = false;
    public virtual bool isYearPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; set; } =
        new DartMap<long, double>();

    internal _CupertinoDatePickerMonthYearState__date_picker(DatePickerDateOrder? dateOrder)
    {
        this.dateOrder = dateOrder;
    }

    public virtual bool isScrolling =>
        DartRuntimePrimitives.ConvertValue<bool>(isMonthPickerScrolling || isYearPickerScrolling);

    public override void initState()
    {
        base.initState();
        selectedMonth = widget.initialDateTime.Month;
        selectedYear = widget.initialDateTime.Year;
        monthController = new FixedExtentScrollController(initialItem: selectedMonth - 1L);
        yearController = new FixedExtentScrollController(initialItem: selectedYear);
        PaintingBinding.instance.systemFonts.addListener(_handleSystemFontsChange);
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(() =>
        {
            _refreshEstimatedColumnWidths();
        });
    }

    public override void dispose()
    {
        monthController.dispose();
        yearController.dispose();
        PaintingBinding.instance.systemFonts.removeListener(_handleSystemFontsChange);
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = Equals(Directionality.of(context), TextDirection.ltr) ? 1L : -1L;
        localizations = CupertinoLocalizations.of(context);
        alignCenterLeft =
            (textDirectionFactor == 1L) ? Alignment.centerLeft : Alignment.centerRight;
        alignCenterRight =
            (textDirectionFactor == 1L) ? Alignment.centerRight : Alignment.centerLeft;
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        estimatedColumnWidths[
            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
        ] = CupertinoDatePicker._getColumnWidth(
            _PickerColumnType__date_picker.month,
            localizations,
            context,
            false,
            standaloneMonth: Equals(widget.mode, CupertinoDatePickerMode.monthYear)
        );
        estimatedColumnWidths[
            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
        ] = CupertinoDatePicker._getColumnWidth(
            _PickerColumnType__date_picker.year,
            localizations,
            context,
            false
        );
    }

    internal virtual Widget _buildMonthPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isMonthPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isMonthPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new CupertinoPicker(
                scrollController: monthController,
                offAxisFraction: offAxisFraction,
                itemExtent: Date_pickerLibrary._kItemExtent,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                squeeze: Date_pickerLibrary._kSqueeze,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedMonth = index + 1L;
                    if (_isCurrentDateValid)
                    {
                        widget.onDateTimeChanged(
                            DartRuntimePrimitives.CreateDateTime(selectedYear, selectedMonth)
                        );
                    }
                },
                looping: true,
                selectionOverlay: selectionOverlay,
                children: new List<Widget>(
                    Enumerable.Select(
                        Enumerable.Range(0, checked((int)12L)),
                        (index) =>
                        {
                            long month = index + 1L;
                            bool isInvalidMonth =
                                (
                                    (widget.minimumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.minimumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month > month
                                    )
                                )
                                || (
                                    (widget.maximumDate?.Year == selectedYear)
                                    && (
                                        (
                                            widget.maximumDate
                                            ?? throw new global::System.NullReferenceException(
                                                "A required value was null."
                                            )
                                        ).Month < month
                                    )
                                );
                            string monthName = Equals(
                                widget.mode,
                                CupertinoDatePickerMode.monthYear
                            )
                                ? localizations.datePickerStandaloneMonth(month)
                                : localizations.datePickerMonth(month);
                            Widget childLocal = itemPositioningBuilder(
                                context,
                                new Text(
                                    monthName,
                                    style: Date_pickerLibrary._themeTextStyle(
                                        context,
                                        isValid: !isInvalidMonth
                                    )
                                )
                            );
                            return isInvalidMonth
                                ? new ExcludeSemantics(child: childLocal)
                                : childLocal;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        }
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildYearPicker(
        double offAxisFraction,
        Func<BuildContext, Widget?, Widget> itemPositioningBuilder,
        Widget? selectionOverlay
    )
    {
        return new NotificationListener<ScrollNotification>(
            onNotification: (notification) =>
            {
                if (notification is ScrollStartNotification)
                {
                    isYearPickerScrolling = true;
                }
                else
                {
                    if (notification is ScrollEndNotification)
                    {
                        isYearPickerScrolling = false;
                        _pickerDidStopScrolling();
                    }
                }
                return false;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: CupertinoPicker.CreateBuilder(
                scrollController: yearController,
                itemExtent: Date_pickerLibrary._kItemExtent,
                offAxisFraction: offAxisFraction,
                useMagnifier: Date_pickerLibrary._kUseMagnifier,
                magnification: Date_pickerLibrary._kMagnification,
                backgroundColor: widget.backgroundColor,
                changeReportingBehavior: widget.changeReportingBehavior,
                onSelectedItemChanged: (index) =>
                {
                    selectedYear = index;
                    if (_isCurrentDateValid)
                    {
                        widget.onDateTimeChanged(
                            DartRuntimePrimitives.CreateDateTime(selectedYear, selectedMonth)
                        );
                    }
                },
                itemBuilder: (context, year) =>
                {
                    if (year < widget.minimumYear)
                    {
                        return null;
                    }
                    if (
                        (widget.maximumYear is not null)
                        && (
                            year
                            > (
                                widget.maximumYear
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )
                    {
                        return null;
                    }
                    bool isValidYear =
                        (
                            (widget.minimumDate is null)
                            || (
                                (
                                    widget.minimumDate
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ).Year <= year
                            )
                        )
                        && (
                            (widget.maximumDate is null)
                            || (
                                (
                                    widget.maximumDate
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                ).Year >= year
                            )
                        );
                    Widget childLocal = itemPositioningBuilder(
                        context,
                        new Text(
                            localizations.datePickerYear(year),
                            style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear)
                        )
                    );
                    return isValidYear ? childLocal : new ExcludeSemantics(child: childLocal);
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                },
                selectionOverlay: selectionOverlay
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate = DartRuntimePrimitives.CreateDateTime(selectedYear, selectedMonth);
            var maxSelectedDate = DartRuntimePrimitives.CreateDateTime(
                selectedYear,
                selectedMonth,
                widget.initialDateTime.Day + 1L
            );
            bool minCheck = widget.minimumDate?.isBefore(maxSelectedDate) ?? true;
            bool maxCheck = widget.maximumDate?.isBefore(minSelectedDate) ?? false;
            return minCheck && !maxCheck;
        }
    }

    internal virtual void _pickerDidStopScrolling()
    {
        setState(() => { });
        if (isScrolling)
        {
            return;
        }
        var minSelectDate = DartRuntimePrimitives.CreateDateTime(selectedYear, selectedMonth);
        var maxSelectDate = DartRuntimePrimitives.CreateDateTime(
            selectedYear,
            selectedMonth,
            widget.initialDateTime.Day + 1L
        );
        bool minCheck = widget.minimumDate?.isBefore(maxSelectDate) ?? true;
        bool maxCheck = widget.maximumDate?.isBefore(minSelectDate) ?? false;
        if (!minCheck || maxCheck)
        {
            DateTime targetDate = minCheck
                ? (
                    widget.maximumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
                : (
                    widget.minimumDate
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            _scrollToDate(targetDate);
            return;
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback(
            (timestamp) =>
            {
                if (selectedYear != newDate.Year)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(yearController, newDate.Year);
                }
                if (selectedMonth != newDate.Month)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(
                        monthController,
                        newDate.Month - 1L
                    );
                }
            },
            debugLabel: "DatePicker.scrollToDate"
        );
    }

    public override Widget build(BuildContext context)
    {
        var pickerBuilders =
            new List<Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>>();
        var columnWidthsLocal = new List<double>();
        DatePickerDateOrder datePickerDateOrderLocal =
            dateOrder ?? localizations.datePickerDateOrder;
        switch (datePickerDateOrderLocal)
        {
            case var __constant76081 when Equals(__constant76081, DatePickerDateOrder.mdy):
            case var __constant76117 when Equals(__constant76117, DatePickerDateOrder.dmy):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildMonthPicker,
                    _buildYearPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
            case var __constant76406 when Equals(__constant76406, DatePickerDateOrder.ymd):
            case var __constant76442 when Equals(__constant76442, DatePickerDateOrder.ydm):
            {
                pickerBuilders = new List<
                    Func<double, Func<BuildContext, Widget?, Widget>, Widget?, Widget>
                >
                {
                    _buildYearPicker,
                    _buildMonthPicker,
                };
                columnWidthsLocal = new List<double>
                {
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    (
                        DartCollectionRuntime.NullableMapValue<double>(
                            estimatedColumnWidths,
                            FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                };
                break;
            }
        }
        var pickers = new List<Widget>();
        double totalColumnWidths = 3L * Date_pickerLibrary._kDatePickerPadSize;
        foreach (var (i, widthLocal) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = (i == 0L, i == (checked(columnWidthsLocal.Count) - 1L));
            double offAxisFraction = textDirectionFactor * (firstColumn ? -0.3 : 0.5);
            totalColumnWidths += widthLocal + (2L * Date_pickerLibrary._kDatePickerPadSize);
            Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if (widget.selectionOverlayBuilder is not null)
            {
                selectionOverlay = widget.selectionOverlayBuilder!(
                    context,
                    selectedIndex: i,
                    columnCount: checked(columnWidthsLocal.Count)
                );
            }
            else
            {
                if (firstColumn)
                {
                    selectionOverlay = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn)
                    {
                        selectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            pickers.Add(
                new LayoutId(
                    id: i,
                    child: pickerBuilders[(int)i]
                        (
                            offAxisFraction,
                            (context, child) =>
                            {
                                Widget contents = new Align(
                                    alignment: lastColumn ? alignCenterLeft : alignCenterRight,
                                    child: new SizedBox(
                                        width: widthLocal + Date_pickerLibrary._kDatePickerPadSize,
                                        child: new Align(
                                            alignment: firstColumn
                                                ? alignCenterLeft
                                                : alignCenterRight,
                                            child: child
                                        )
                                    )
                                );
                                if (firstColumn)
                                {
                                    return contents;
                                }
                                var paddingLocal = EdgeInsets.CreateOnly(
                                    right: Date_pickerLibrary._kDatePickerPadSize
                                );
                                return new Padding(
                                    padding: (textDirectionFactor == -1L)
                                        ? paddingLocal.flipped
                                        : paddingLocal,
                                    child: contents
                                );
                                throw new InvalidOperationException(
                                    "Callback completed without returning a value."
                                );
                            },
                            selectionOverlay
                        )
                )
            );
        }
        double maxPickerWidth =
            (totalColumnWidths > Date_pickerLibrary._kPickerWidth)
                ? totalColumnWidths
                : Date_pickerLibrary._kPickerWidth;
        return MediaQuery.withNoTextScaling(
            child: DefaultTextStyle.merge(
                style: Date_pickerLibrary._kDefaultPickerTextStyle,
                child: new CustomMultiChildLayout(
                    @delegate: new _DatePickerLayoutDelegate__date_picker(
                        columnWidths: columnWidthsLocal,
                        textDirectionFactor: textDirectionFactor,
                        maxWidth: maxPickerWidth
                    ),
                    children: pickers
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum CupertinoTimerPickerMode
{
    hm,
    ms,
    hms,
}

public class CupertinoTimerPicker : StatefulWidget
{
    public virtual CupertinoTimerPickerMode mode { get; private set; } = default!;
    public virtual Duration initialTimerDuration { get; private set; } = default!;
    public virtual long minuteInterval { get; private set; } = default!;
    public virtual long secondInterval { get; private set; } = default!;
    public virtual Action<Duration> onTimerDurationChanged { get; private set; } = default!;
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual SelectionOverlayBuilder? selectionOverlayBuilder { get; private set; }
    public virtual ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoTimerPicker(
        Key? key = null,
        CupertinoTimerPickerMode mode = CupertinoTimerPickerMode.hms,
        Duration initialTimerDuration = default,
        long minuteInterval = 1,
        long secondInterval = 1,
        AlignmentGeometry alignment = default!,
        Color? backgroundColor = null,
        double? itemExtent = null,
        Action<Duration> onTimerDurationChanged = default!,
        ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate,
        SelectionOverlayBuilder? selectionOverlayBuilder = null
    )
        : base(key: key)
    {
        AlignmentGeometry __alignment = alignment ?? Alignment.center;
        double __itemExtent = itemExtent ?? Date_pickerLibrary._kItemExtent;
        this.mode = mode;
        this.initialTimerDuration = initialTimerDuration;
        this.minuteInterval = minuteInterval;
        this.secondInterval = secondInterval;
        this.alignment = __alignment;
        this.backgroundColor = backgroundColor;
        this.itemExtent = __itemExtent;
        this.onTimerDurationChanged = onTimerDurationChanged;
        this.changeReportingBehavior = changeReportingBehavior;
        this.selectionOverlayBuilder = selectionOverlayBuilder;
        System.Diagnostics.Debug.Assert(initialTimerDuration >= Duration.zero);
        System.Diagnostics.Debug.Assert(initialTimerDuration < Duration.Create(days: 1L));
        System.Diagnostics.Debug.Assert((minuteInterval > 0L) && ((60L % minuteInterval) == 0L));
        System.Diagnostics.Debug.Assert((secondInterval > 0L) && ((60L % secondInterval) == 0L));
        System.Diagnostics.Debug.Assert((initialTimerDuration.inMinutes % minuteInterval) == 0L);
        System.Diagnostics.Debug.Assert((initialTimerDuration.inSeconds % secondInterval) == 0L);
        System.Diagnostics.Debug.Assert(__itemExtent > 0L);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTimerPickerState__date_picker());
}

internal class _CupertinoTimerPickerState__date_picker : State<CupertinoTimerPicker>
{
    public virtual TextDirection textDirection { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual long? selectedHour { get; set; } = default;
    public virtual long selectedMinute { get; set; } = default!;
    public virtual long? selectedSecond { get; set; } = default;
    public virtual long? lastSelectedHour { get; set; } = default;
    public virtual long? lastSelectedMinute { get; set; } = default;
    public virtual long? lastSelectedSecond { get; set; } = default;
    public virtual TextPainter textPainter { get; private set; } = new TextPainter();
    public virtual List<string> numbers { get; private set; } =
        new List<string>(
            Enumerable.Select(Enumerable.Range(0, checked((int)10L)), (i) => $"{9L - i}")
        );
    public virtual double numberLabelWidth { get; set; } = default!;
    public virtual double numberLabelHeight { get; set; } = default!;
    public virtual double numberLabelBaseline { get; set; } = default!;
    public virtual double hourLabelWidth { get; set; } = default!;
    public virtual double minuteLabelWidth { get; set; } = default!;
    public virtual double secondLabelWidth { get; set; } = default!;
    public virtual double totalWidth { get; set; } = default!;
    public virtual double pickerColumnWidth { get; set; } = default!;
    internal virtual FixedExtentScrollController? _hourScrollController { get; set; } = default;
    internal virtual FixedExtentScrollController? _minuteScrollController { get; set; } = default;
    internal virtual FixedExtentScrollController? _secondScrollController { get; set; } = default;

    public virtual long textDirectionFactor =>
        textDirection switch
        {
            TextDirection.ltr => 1L,
            TextDirection.rtl => -1L,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };

    public override void initState()
    {
        base.initState();
        selectedMinute = widget.initialTimerDuration.inMinutes % 60L;
        if (!Equals(widget.mode, CupertinoTimerPickerMode.ms))
        {
            selectedHour = widget.initialTimerDuration.inHours;
        }
        if (!Equals(widget.mode, CupertinoTimerPickerMode.hm))
        {
            selectedSecond = widget.initialTimerDuration.inSeconds % 60L;
        }
        PaintingBinding.instance.systemFonts.addListener(_handleSystemFontsChange);
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(() =>
        {
            textPainter.markNeedsLayout();
            _measureLabelMetrics();
        });
    }

    public override void dispose()
    {
        PaintingBinding.instance.systemFonts.removeListener(_handleSystemFontsChange);
        textPainter.dispose();
        _hourScrollController?.dispose();
        _minuteScrollController?.dispose();
        _secondScrollController?.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoTimerPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(
            () => Equals(oldWidget.mode, widget.mode),
            () => (object?)"The CupertinoTimerPicker's mode cannot change once it's built"
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirection = Directionality.of(context);
        localizations = CupertinoLocalizations.of(context);
        _measureLabelMetrics();
    }

    internal virtual void _measureLabelMetrics()
    {
        textPainter.textDirection = textDirection;
        TextStyle textStyle = _textStyleFrom(
            context,
            Date_pickerLibrary._kTimerPickerMagnification
        );
        double maxWidth = double.NegativeInfinity;
        string? widestNumber = default!;
        foreach (string input in numbers)
        {
            textPainter.text = DartRuntimePrimitives.ConvertValue<InlineSpan>(
                new TextSpan(text: input, style: textStyle)
            );
            textPainter.layout();
            if (textPainter.maxIntrinsicWidth > maxWidth)
            {
                maxWidth = textPainter.maxIntrinsicWidth;
                widestNumber = input;
            }
        }
        textPainter.text = DartRuntimePrimitives.ConvertValue<InlineSpan>(
            new TextSpan(text: $"{widestNumber}{widestNumber}", style: textStyle)
        );
        textPainter.layout();
        numberLabelWidth = textPainter.maxIntrinsicWidth;
        numberLabelHeight = textPainter.height;
        numberLabelBaseline = textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        minuteLabelWidth = _measureLabelsMaxWidth(
            localizations.timerPickerMinuteLabels.Cast<string?>().ToList(),
            textStyle
        );
        if (!Equals(widget.mode, CupertinoTimerPickerMode.ms))
        {
            hourLabelWidth = _measureLabelsMaxWidth(
                localizations.timerPickerHourLabels.Cast<string?>().ToList(),
                textStyle
            );
        }
        if (!Equals(widget.mode, CupertinoTimerPickerMode.hm))
        {
            secondLabelWidth = _measureLabelsMaxWidth(
                localizations.timerPickerSecondLabels.Cast<string?>().ToList(),
                textStyle
            );
        }
    }

    internal virtual double _measureLabelsMaxWidth(List<string?> labels, TextStyle style)
    {
        double maxWidth = double.NegativeInfinity;
        for (var i = 0L; i < checked(labels.Count); i++)
        {
            string? label = labels[(int)i];
            if (label is null)
            {
                continue;
            }
            textPainter.text = DartRuntimePrimitives.ConvertValue<InlineSpan>(
                new TextSpan(text: label, style: style)
            );
            textPainter.layout();
            DartRuntimePrimitives.Ignore(textPainter.maxIntrinsicWidth);
            if (textPainter.maxIntrinsicWidth > maxWidth)
            {
                maxWidth = textPainter.maxIntrinsicWidth;
            }
        }
        return maxWidth;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildLabel(string text, EdgeInsetsDirectional pickerPadding)
    {
        var paddingLocal = EdgeInsetsDirectional.CreateOnly(
            start: numberLabelWidth
                + Date_pickerLibrary._kTimerPickerLabelPadSize
                + pickerPadding.start
        );
        return new IgnorePointer(
            child: new Padding(
                padding: paddingLocal.resolve(textDirection),
                child: new Align(
                    alignment: AlignmentDirectional.centerStart.resolve(textDirection),
                    child: new SizedBox(
                        height: numberLabelHeight,
                        child: new Baseline(
                            baseline: numberLabelBaseline,
                            baselineType: TextBaseline.alphabetic,
                            child: new Text(
                                text,
                                style: new TextStyle(
                                    fontSize: Date_pickerLibrary._kTimerPickerLabelFontSize,
                                    fontWeight: FontWeight.w600
                                ),
                                maxLines: 1L,
                                softWrap: false
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildPickerNumberLabel(string text, EdgeInsetsDirectional padding)
    {
        return new SizedBox(
            width: Date_pickerLibrary._kTimerPickerColumnIntrinsicWidth + padding.horizontal,
            child: new Padding(
                padding: padding.resolve(textDirection),
                child: new Align(
                    alignment: AlignmentDirectional.centerStart.resolve(textDirection),
                    child: new SizedBox(
                        width: numberLabelWidth,
                        child: new Align(
                            alignment: AlignmentDirectional.centerEnd.resolve(textDirection),
                            child: new Text(
                                text,
                                softWrap: false,
                                maxLines: 1L,
                                overflow: TextOverflow.visible
                            )
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildHourPicker(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        _hourScrollController ??= new FixedExtentScrollController(
            initialItem: (
                selectedHour
                ?? throw new global::System.NullReferenceException("A required value was null.")
            )
        );
        return new CupertinoPicker(
            scrollController: _hourScrollController,
            magnification: Date_pickerLibrary._kMagnification,
            offAxisFraction: _calculateOffAxisFraction(additionalPadding.start, 0L),
            itemExtent: widget.itemExtent,
            backgroundColor: widget.backgroundColor,
            squeeze: Date_pickerLibrary._kSqueeze,
            changeReportingBehavior: widget.changeReportingBehavior,
            onSelectedItemChanged: (index) =>
            {
                setState(() =>
                {
                    selectedHour = index;
                    widget.onTimerDurationChanged(
                        Duration.Create(
                            hours: (
                                selectedHour
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ),
                            minutes: selectedMinute,
                            seconds: selectedSecond ?? 0L
                        )
                    );
                });
            },
            selectionOverlay: selectionOverlay,
            children: new List<Widget>(
                Enumerable.Select(
                    Enumerable.Range(0, checked((int)24L)),
                    (index) =>
                    {
                        string labelLocal = localizations.timerPickerHourLabel(index) ?? "";
                        string semanticsLabel =
                            (textDirectionFactor == 1L)
                                ? (localizations.timerPickerHour(index) + labelLocal)
                                : (labelLocal + localizations.timerPickerHour(index));
                        return new Widgets.Semantics(
                            label: semanticsLabel,
                            excludeSemantics: true,
                            child: _buildPickerNumberLabel(
                                localizations.timerPickerHour(index),
                                additionalPadding
                            )
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildHourColumn(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        additionalPadding = EdgeInsetsDirectional.CreateOnly(
            start: Math.Max(additionalPadding.start, 0),
            end: Math.Max(additionalPadding.end, 0)
        );
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new NotificationListener<ScrollEndNotification>(
                        onNotification: (notification) =>
                        {
                            setState(() =>
                            {
                                lastSelectedHour = selectedHour;
                            });
                            return false;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        },
                        child: _buildHourPicker(additionalPadding, selectionOverlay)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    _buildLabel(
                        localizations.timerPickerHourLabel(
                            lastSelectedHour
                                ?? (
                                    selectedHour
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                )
                        ) ?? "",
                        additionalPadding
                    )
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildMinutePicker(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        _minuteScrollController ??= new FixedExtentScrollController(
            initialItem: checked(selectedMinute / widget.minuteInterval)
        );
        return new CupertinoPicker(
            scrollController: _minuteScrollController,
            magnification: Date_pickerLibrary._kMagnification,
            offAxisFraction: _calculateOffAxisFraction(
                additionalPadding.start,
                Equals(widget.mode, CupertinoTimerPickerMode.ms) ? 0L : 1L
            ),
            itemExtent: widget.itemExtent,
            backgroundColor: widget.backgroundColor,
            squeeze: Date_pickerLibrary._kSqueeze,
            looping: true,
            changeReportingBehavior: widget.changeReportingBehavior,
            onSelectedItemChanged: (index) =>
            {
                setState(() =>
                {
                    selectedMinute = index * widget.minuteInterval;
                    widget.onTimerDurationChanged(
                        Duration.Create(
                            hours: selectedHour ?? 0L,
                            minutes: selectedMinute,
                            seconds: selectedSecond ?? 0L
                        )
                    );
                });
            },
            selectionOverlay: selectionOverlay,
            children: new List<Widget>(
                Enumerable.Select(
                    Enumerable.Range(0, checked((int)checked(60L / widget.minuteInterval))),
                    (index) =>
                    {
                        long minute = index * widget.minuteInterval;
                        string labelLocal = localizations.timerPickerMinuteLabel(minute) ?? "";
                        string semanticsLabel =
                            (textDirectionFactor == 1L)
                                ? (localizations.timerPickerMinute(minute) + labelLocal)
                                : (labelLocal + localizations.timerPickerMinute(minute));
                        return new Widgets.Semantics(
                            label: semanticsLabel,
                            excludeSemantics: true,
                            child: _buildPickerNumberLabel(
                                localizations.timerPickerMinute(minute),
                                additionalPadding
                            )
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildMinuteColumn(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        additionalPadding = EdgeInsetsDirectional.CreateOnly(
            start: Math.Max(additionalPadding.start, 0),
            end: Math.Max(additionalPadding.end, 0)
        );
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new NotificationListener<ScrollEndNotification>(
                        onNotification: (notification) =>
                        {
                            setState(() =>
                            {
                                lastSelectedMinute = selectedMinute;
                            });
                            return false;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        },
                        child: _buildMinutePicker(additionalPadding, selectionOverlay)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    _buildLabel(
                        localizations.timerPickerMinuteLabel(lastSelectedMinute ?? selectedMinute)
                            ?? "",
                        additionalPadding
                    )
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildSecondPicker(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        _secondScrollController ??= new FixedExtentScrollController(
            initialItem: checked(
                (
                    selectedSecond
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) / widget.secondInterval
            )
        );
        return new CupertinoPicker(
            scrollController: _secondScrollController,
            magnification: Date_pickerLibrary._kMagnification,
            offAxisFraction: _calculateOffAxisFraction(
                additionalPadding.start,
                Equals(widget.mode, CupertinoTimerPickerMode.ms) ? 1L : 2L
            ),
            itemExtent: widget.itemExtent,
            backgroundColor: widget.backgroundColor,
            squeeze: Date_pickerLibrary._kSqueeze,
            looping: true,
            changeReportingBehavior: widget.changeReportingBehavior,
            onSelectedItemChanged: (index) =>
            {
                setState(() =>
                {
                    selectedSecond = index * widget.secondInterval;
                    widget.onTimerDurationChanged(
                        Duration.Create(
                            hours: selectedHour ?? 0L,
                            minutes: selectedMinute,
                            seconds: (
                                selectedSecond
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    );
                });
            },
            selectionOverlay: selectionOverlay,
            children: new List<Widget>(
                Enumerable.Select(
                    Enumerable.Range(0, checked((int)checked(60L / widget.secondInterval))),
                    (index) =>
                    {
                        long second = index * widget.secondInterval;
                        string labelLocal = localizations.timerPickerSecondLabel(second) ?? "";
                        string semanticsLabel =
                            (textDirectionFactor == 1L)
                                ? (localizations.timerPickerSecond(second) + labelLocal)
                                : (labelLocal + localizations.timerPickerSecond(second));
                        return new Widgets.Semantics(
                            label: semanticsLabel,
                            excludeSemantics: true,
                            child: _buildPickerNumberLabel(
                                localizations.timerPickerSecond(second),
                                additionalPadding
                            )
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildSecondColumn(
        EdgeInsetsDirectional additionalPadding,
        Widget? selectionOverlay
    )
    {
        additionalPadding = EdgeInsetsDirectional.CreateOnly(
            start: Math.Max(additionalPadding.start, 0),
            end: Math.Max(additionalPadding.end, 0)
        );
        return new Stack(
            children: new List<Widget>
            {
                DartRuntimePrimitives.ConvertValue<Widget>(
                    new NotificationListener<ScrollEndNotification>(
                        onNotification: (notification) =>
                        {
                            setState(() =>
                            {
                                lastSelectedSecond = selectedSecond;
                            });
                            return false;
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        },
                        child: _buildSecondPicker(additionalPadding, selectionOverlay)
                    )
                ),
                DartRuntimePrimitives.ConvertValue<Widget>(
                    _buildLabel(
                        localizations.timerPickerSecondLabel(
                            lastSelectedSecond
                                ?? (
                                    selectedSecond
                                    ?? throw new global::System.NullReferenceException(
                                        "A required value was null."
                                    )
                                )
                        ) ?? "",
                        additionalPadding
                    )
                ),
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual TextStyle _textStyleFrom(BuildContext context, double magnification = 1.0)
    {
        TextStyle textStyle = CupertinoTheme.of(context).textTheme.pickerTextStyle;
        return textStyle.copyWith(
            color: CupertinoDynamicColor.maybeResolve(textStyle.color, context),
            fontSize: (
                textStyle.fontSize
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) * magnification
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _calculateOffAxisFraction(double paddingStart, long position)
    {
        double centerPoint = paddingStart + (numberLabelWidth / 2L);
        double pickerColumnOffAxisFraction = 0.5 - (centerPoint / pickerColumnWidth);
        double timerPickerOffAxisFraction =
            0.5 - ((centerPoint + (pickerColumnWidth * position)) / totalWidth);
        return (pickerColumnOffAxisFraction - timerPickerOffAxisFraction) * textDirectionFactor;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new LayoutBuilder(
            builder: (context, constraints) =>
            {
                List<Widget> columns = default!;
                if (Equals(widget.mode, CupertinoTimerPickerMode.hms))
                {
                    pickerColumnWidth =
                        Date_pickerLibrary._kTimerPickerColumnIntrinsicWidth
                        + (Date_pickerLibrary._kTimerPickerHalfColumnPadding * 2L);
                    totalWidth = pickerColumnWidth * 3L;
                }
                else
                {
                    totalWidth = Date_pickerLibrary._kPickerWidth;
                    pickerColumnWidth = totalWidth / 2L;
                }
                if (constraints.maxWidth < totalWidth)
                {
                    totalWidth = constraints.maxWidth;
                    pickerColumnWidth =
                        totalWidth / (Equals(widget.mode, CupertinoTimerPickerMode.hms) ? 3L : 2L);
                }
                double baseLabelContentWidth =
                    numberLabelWidth + Date_pickerLibrary._kTimerPickerLabelPadSize;
                double minuteLabelContentWidth = baseLabelContentWidth + minuteLabelWidth;
                switch (widget.mode)
                {
                    case CupertinoTimerPickerMode.hm:
                    {
                        double hourLabelContentWidth = baseLabelContentWidth + hourLabelWidth;
                        double hourColumnStartPadding =
                            pickerColumnWidth
                            - hourLabelContentWidth
                            - Date_pickerLibrary._kTimerPickerHalfColumnPadding;
                        if (
                            hourColumnStartPadding
                            < Date_pickerLibrary._kTimerPickerMinHorizontalPadding
                        )
                        {
                            hourColumnStartPadding =
                                Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        double minuteColumnEndPadding =
                            pickerColumnWidth
                            - minuteLabelContentWidth
                            - Date_pickerLibrary._kTimerPickerHalfColumnPadding;
                        if (
                            minuteColumnEndPadding
                            < Date_pickerLibrary._kTimerPickerMinHorizontalPadding
                        )
                        {
                            minuteColumnEndPadding =
                                Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        Widget? hourSelectionOverlay = Date_pickerLibrary._startSelectionOverlay;
                        Widget? minuteSelectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                        if (widget.selectionOverlayBuilder is not null)
                        {
                            hourSelectionOverlay = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 0L,
                                columnCount: 2L
                            );
                            minuteSelectionOverlay = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 1L,
                                columnCount: 2L
                            );
                        }
                        columns = new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildHourColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: hourColumnStartPadding,
                                        end: pickerColumnWidth
                                            - hourColumnStartPadding
                                            - hourLabelContentWidth
                                    ),
                                    hourSelectionOverlay
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildMinuteColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: pickerColumnWidth
                                            - minuteColumnEndPadding
                                            - minuteLabelContentWidth,
                                        end: minuteColumnEndPadding
                                    ),
                                    minuteSelectionOverlay
                                )
                            ),
                        };
                        break;
                    }
                    case CupertinoTimerPickerMode.ms:
                    {
                        double secondLabelContentWidth = baseLabelContentWidth + secondLabelWidth;
                        double secondColumnEndPadding =
                            pickerColumnWidth
                            - secondLabelContentWidth
                            - Date_pickerLibrary._kTimerPickerHalfColumnPadding;
                        if (
                            secondColumnEndPadding
                            < Date_pickerLibrary._kTimerPickerMinHorizontalPadding
                        )
                        {
                            secondColumnEndPadding =
                                Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        double minuteColumnStartPadding =
                            pickerColumnWidth
                            - minuteLabelContentWidth
                            - Date_pickerLibrary._kTimerPickerHalfColumnPadding;
                        if (
                            minuteColumnStartPadding
                            < Date_pickerLibrary._kTimerPickerMinHorizontalPadding
                        )
                        {
                            minuteColumnStartPadding =
                                Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        Widget? minuteSelectionOverlayLocal =
                            Date_pickerLibrary._startSelectionOverlay;
                        Widget? secondSelectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                        if (widget.selectionOverlayBuilder is not null)
                        {
                            minuteSelectionOverlayLocal = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 0L,
                                columnCount: 2L
                            );
                            secondSelectionOverlay = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 1L,
                                columnCount: 2L
                            );
                        }
                        columns = new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildMinuteColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: minuteColumnStartPadding,
                                        end: pickerColumnWidth
                                            - minuteColumnStartPadding
                                            - minuteLabelContentWidth
                                    ),
                                    minuteSelectionOverlayLocal
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildSecondColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: pickerColumnWidth
                                            - secondColumnEndPadding
                                            - minuteLabelContentWidth,
                                        end: secondColumnEndPadding
                                    ),
                                    secondSelectionOverlay
                                )
                            ),
                        };
                        break;
                    }
                    case CupertinoTimerPickerMode.hms:
                    {
                        double hourColumnEndPadding =
                            pickerColumnWidth
                            - baseLabelContentWidth
                            - hourLabelWidth
                            - Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        double minuteColumnPadding =
                            (pickerColumnWidth - minuteLabelContentWidth) / 2L;
                        double secondColumnStartPadding =
                            pickerColumnWidth
                            - baseLabelContentWidth
                            - secondLabelWidth
                            - Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        Widget? hourSelectionOverlayLocal =
                            Date_pickerLibrary._startSelectionOverlay;
                        Widget? minuteSelectionOverlayAlternate =
                            Date_pickerLibrary._centerSelectionOverlay;
                        Widget? secondSelectionOverlayLocal =
                            Date_pickerLibrary._endSelectionOverlay;
                        if (widget.selectionOverlayBuilder is not null)
                        {
                            hourSelectionOverlayLocal = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 0L,
                                columnCount: 3L
                            );
                            minuteSelectionOverlayAlternate = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 1L,
                                columnCount: 3L
                            );
                            secondSelectionOverlayLocal = widget.selectionOverlayBuilder!(
                                context,
                                selectedIndex: 2L,
                                columnCount: 3L
                            );
                        }
                        columns = new List<Widget>
                        {
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildHourColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: Date_pickerLibrary._kTimerPickerMinHorizontalPadding,
                                        end: Math.Max(hourColumnEndPadding, 0)
                                    ),
                                    hourSelectionOverlayLocal
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildMinuteColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: minuteColumnPadding,
                                        end: minuteColumnPadding
                                    ),
                                    minuteSelectionOverlayAlternate
                                )
                            ),
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                _buildSecondColumn(
                                    EdgeInsetsDirectional.CreateOnly(
                                        start: Math.Max(secondColumnStartPadding, 0),
                                        end: Date_pickerLibrary._kTimerPickerMinHorizontalPadding
                                    ),
                                    secondSelectionOverlayLocal
                                )
                            ),
                        };
                        break;
                    }
                }
                Widget contents = new SizedBox(
                    width: totalWidth,
                    height: Date_pickerLibrary._kPickerHeight,
                    child: new DefaultTextStyle(
                        style: _textStyleFrom(context),
                        child: new Row(
                            children: columns
                                .map((child) => new Expanded(child: child))
                                .ToList()
                                .Cast<Widget>()
                                .ToList()
                        )
                    )
                );
                Color? colorLocal = CupertinoDynamicColor.maybeResolve(
                    widget.backgroundColor,
                    context
                );
                if (colorLocal is not null)
                {
                    contents = DartRuntimePrimitives.ConvertValue<Widget>(
                        new ColoredBox(color: colorLocal, child: contents)
                    );
                }
                CupertinoThemeData themeData = CupertinoTheme.of(context);
                return MediaQuery.withNoTextScaling(
                    child: new CupertinoTheme(
                        data: themeData.copyWith(
                            textTheme: themeData.textTheme.copyWith(
                                pickerTextStyle: _textStyleFrom(
                                    context,
                                    Date_pickerLibrary._kTimerPickerMagnification
                                )
                            )
                        ),
                        child: new Align(alignment: widget.alignment, child: contents)
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
