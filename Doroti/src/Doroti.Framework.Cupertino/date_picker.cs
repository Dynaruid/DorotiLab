// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/date_picker.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
    internal static double _kMagnification = (2.35 / 2.1);
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
    internal static global::Doroti.Framework.Painting.TextStyle _kDefaultPickerTextStyle = new global::Doroti.Framework.Painting.TextStyle(letterSpacing: -0.83);
}

public static partial class Date_pickerLibrary
{
    internal static double _kTimerPickerMagnification = (34L / 32L);
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
    internal static global::Doroti.Framework.Painting.TextStyle _themeTextStyle(global::Doroti.Framework.Widgets.BuildContext context, bool isValid = true)
    {
        global::Doroti.Framework.Painting.TextStyle style = CupertinoTheme.of(context).textTheme.dateTimePickerTextStyle;
        return (isValid ? style.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)style).color, context)) : style.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.inactiveGray, context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static void _animateColumnControllerToItem(global::Doroti.Framework.Widgets.FixedExtentScrollController controller, long targetItem)
    {
        DartRuntimePrimitives.Ignore(controller.animateToItem(targetItem, curve: global::Doroti.Framework.Animation.Curves.easeInOut, duration: Duration.Create(milliseconds: 200L)));
    }
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _startSelectionOverlay = ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capEndEdge: false));
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _centerSelectionOverlay = ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capStartEdge: false, capEndEdge: false));
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _endSelectionOverlay = ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capStartEdge: false));
}

public delegate global::Doroti.Framework.Widgets.Widget? SelectionOverlayBuilder(global::Doroti.Framework.Widgets.BuildContext context, long columnCount, long selectedIndex);

internal class _DatePickerLayoutDelegate__date_picker : global::Doroti.Framework.Rendering.MultiChildLayoutDelegate
{
    public virtual List<double> columnWidths { get; private set; } = default!;
    public virtual long textDirectionFactor { get; private set; } = default!;
    public virtual double maxWidth { get; private set; } = default!;

    internal _DatePickerLayoutDelegate__date_picker(List<double> columnWidths, long textDirectionFactor, double maxWidth)
    {
        this.columnWidths = columnWidths;
        this.textDirectionFactor = textDirectionFactor;
        this.maxWidth = maxWidth;
    }

    public override void performLayout(Size size)
    {
        double remainingWidth = ((this.maxWidth < size.width) ? this.maxWidth : size.width);
        double currentHorizontalOffset = (((size.width - remainingWidth)) / 2L);
        for (var i = 0L; (i < checked((long)(this.columnWidths.Count))); i++)
        {
            remainingWidth -= (this.columnWidths[(int)(i)] + (Date_pickerLibrary._kDatePickerPadSize * 2L));
        }
        for (var iLocal = 0L; (iLocal < checked((long)(this.columnWidths.Count))); iLocal++)
        {
            long index = ((this.textDirectionFactor == 1L) ? iLocal : ((checked((long)(this.columnWidths.Count)) - iLocal) - 1L));
            double childWidth = (this.columnWidths[(int)(index)] + (Date_pickerLibrary._kDatePickerPadSize * 2L));
            if (((index == 0L) || (index == (checked((long)(this.columnWidths.Count)) - 1L))))
            {
                childWidth += (remainingWidth / 2L);
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((childWidth < 0L))
                    {
                        FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Framework.Foundation.FlutterError.Create("Insufficient horizontal space to render the " + "CupertinoDatePicker because the parent is too narrow at " + $"{size.width}px.\n" + $"An additional {-remainingWidth}px is needed to avoid " + "overlapping columns.")));
                    }
                    return true;
                });
            layoutChild(index, global::Doroti.Framework.Rendering.BoxConstraints.CreateTight(new global::Doroti.Ui.Size(Math.Max(0.0, childWidth), size.height)));
            positionChild(index, new global::Doroti.Ui.Offset(currentHorizontalOffset, 0.0));
            currentHorizontalOffset += childWidth;
        }
    }

    public override bool shouldRelayout(global::Doroti.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
    {
        var __oldDelegate = (_DatePickerLayoutDelegate__date_picker)(object)oldDelegate;
        return ((!object.Equals(this.columnWidths, ((_DatePickerLayoutDelegate__date_picker)__oldDelegate).columnWidths)) || (this.textDirectionFactor != ((_DatePickerLayoutDelegate__date_picker)__oldDelegate).textDirectionFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum CupertinoDatePickerMode
{
    time,
    date,
    dateAndTime,
    monthYear
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
    timeSeparator
}

public class CupertinoDatePicker : global::Doroti.Framework.Widgets.StatefulWidget
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
    public virtual global::System.Action<DateTime> onDateTimeChanged { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool showDayOfWeek { get; private set; } = default!;
    public virtual bool showTimeSeparator { get; private set; } = default!;
    public virtual global::System.Func<DateTime, bool>? selectableDayPredicate { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual SelectionOverlayBuilder? selectionOverlayBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoDatePicker(global::Doroti.Framework.Foundation.Key? key = null, CupertinoDatePickerMode mode = CupertinoDatePickerMode.dateAndTime, global::System.Action<DateTime> onDateTimeChanged = default!, DateTime? initialDateTime = null, DateTime? minimumDate = null, DateTime? maximumDate = null, long minimumYear = 1, long? maximumYear = null, long minuteInterval = 1, bool use24hFormat = false, DatePickerDateOrder? dateOrder = null, Color? backgroundColor = null, bool showDayOfWeek = false, bool showTimeSeparator = false, double? itemExtent = null, SelectionOverlayBuilder? selectionOverlayBuilder = null, global::System.Func<DateTime, bool>? selectableDayPredicate = null, global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate) : base(key: key)
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
        this.initialDateTime = (initialDateTime ?? new DateTime());
        System.Diagnostics.Debug.Assert((__itemExtent > 0L));
        System.Diagnostics.Debug.Assert(((minuteInterval > 0L) && ((60L % minuteInterval) == 0L)));
        System.Diagnostics.Debug.Assert((((!object.Equals(mode, CupertinoDatePickerMode.dateAndTime)) || (minimumDate is null)) || !((initialDateTime ?? new DateTime())).isBefore(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(minimumDate)))));
        System.Diagnostics.Debug.Assert((((!object.Equals(mode, CupertinoDatePickerMode.dateAndTime)) || (maximumDate is null)) || !((initialDateTime ?? new DateTime())).isAfter(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(maximumDate)))));
        System.Diagnostics.Debug.Assert(((((!object.Equals(mode, CupertinoDatePickerMode.date)) && (!object.Equals(mode, CupertinoDatePickerMode.monthYear)))) || (((minimumYear >= 1L) && (((initialDateTime ?? new DateTime())).Year >= minimumYear)))));
        System.Diagnostics.Debug.Assert((((((!object.Equals(mode, CupertinoDatePickerMode.date)) && (!object.Equals(mode, CupertinoDatePickerMode.monthYear)))) || (maximumYear is null)) || (((initialDateTime ?? new DateTime())).Year <= DartRuntimePrimitives.RequireValue(maximumYear))));
        System.Diagnostics.Debug.Assert((((((!object.Equals(mode, CupertinoDatePickerMode.date)) && (!object.Equals(mode, CupertinoDatePickerMode.monthYear)))) || (minimumDate is null)) || !DartRuntimePrimitives.RequireValue(minimumDate).isAfter((initialDateTime ?? new DateTime()))));
        System.Diagnostics.Debug.Assert((((((!object.Equals(mode, CupertinoDatePickerMode.date)) && (!object.Equals(mode, CupertinoDatePickerMode.monthYear)))) || (maximumDate is null)) || !DartRuntimePrimitives.RequireValue(maximumDate).isBefore((initialDateTime ?? new DateTime()))));
        System.Diagnostics.Debug.Assert((((object.Equals(mode, CupertinoDatePickerMode.date))) || !showDayOfWeek));
        System.Diagnostics.Debug.Assert(((((initialDateTime ?? new DateTime())).Minute % minuteInterval) == 0L));
        System.Diagnostics.Debug.Assert(((!showTimeSeparator || (object.Equals(mode, CupertinoDatePickerMode.dateAndTime))) || (object.Equals(mode, CupertinoDatePickerMode.time))));
        System.Diagnostics.Debug.Assert((((selectableDayPredicate is null) || (initialDateTime is null)) || selectableDayPredicate(DartRuntimePrimitives.RequireValue(initialDateTime))));
    }

    public override IState createState()
    {
        return ((IState)(object?)(this.mode switch { CupertinoDatePickerMode.time => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateTimeState__date_picker()), CupertinoDatePickerMode.dateAndTime => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateTimeState__date_picker()), CupertinoDatePickerMode.date => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateState__date_picker(dateOrder: this.dateOrder)), CupertinoDatePickerMode.monthYear => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerMonthYearState__date_picker(dateOrder: this.dateOrder)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getColumnWidth(_PickerColumnType__date_picker columnType, CupertinoLocalizations localizations, global::Doroti.Framework.Widgets.BuildContext context, bool showDayOfWeek, bool standaloneMonth = false)
    {
        var longTexts = new List<string>();
        switch (columnType)
        {
            case _PickerColumnType__date_picker.date:
                {
                    for (var i = 1L; (i <= 12L); i++)
                    {
                        string dateLocal = localizations.datePickerMediumDate(DartRuntimePrimitives.CreateDateTime(2018L, i, 25L));
                        longTexts.Add(dateLocal);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.hour:
                {
                    for (var iLocal = 0L; (iLocal < 24L); iLocal++)
                    {
                        string hourLocal = localizations.datePickerHour(iLocal);
                        longTexts.Add(hourLocal);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.minute:
                {
                    for (var iAlternate = 0L; (iAlternate < 60L); iAlternate++)
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
                    for (var iNested = 1L; (iNested <= 31L); iNested++)
                    {
                        string dayOfMonthLocal = localizations.datePickerDayOfMonth(iNested);
                        longTexts.Add(dayOfMonthLocal);
                        longestDayOfMonth = iNested;
                    }
                    if (showDayOfWeek)
                    {
                        for (var wd = 1L; (wd < 7L); wd++)
                        {
                            string dayOfMonthAlternate = localizations.datePickerDayOfMonth(longestDayOfMonth, wd);
                            longTexts.Add(dayOfMonthAlternate);
                        }
                    }
                    break;
                }
            case _PickerColumnType__date_picker.month:
                {
                    for (var iCurrent = 1L; (iCurrent <= 12L); iCurrent++)
                    {
                        string monthLocal = (standaloneMonth ? localizations.datePickerStandaloneMonth(iCurrent) : localizations.datePickerMonth(iCurrent));
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
        DartRuntimePrimitives.Assert(() => (System.Linq.Enumerable.Any(longTexts) && longTexts.All(((text) => (text.Length != 0)))), () => (object?)"column type is not appropriate");
        return CupertinoDatePicker.getColumnWidth(texts: longTexts, context: context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double getColumnWidth(List<string> texts, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Painting.TextStyle? textStyle = null)
    {
        return texts.map<string, double>(((text) => TextPainter.computeMaxIntrinsicWidth(text: new global::Doroti.Framework.Painting.TextSpan(style: (textStyle ?? Date_pickerLibrary._themeTextStyle(context)), text: text), textDirection: Directionality.of(context)))).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::Doroti.Framework.Widgets.Widget _ColumnBuilder__date_picker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay);

internal class _CupertinoDatePickerDateTimeState__date_picker : global::Doroti.Framework.Widgets.State<CupertinoDatePicker>
{
    internal const double _kMaximumOffAxisFraction = 0.45;
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual DateTime initialDateTime { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController dateController { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController hourController { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController minuteController { get; set; } = default!;
    public virtual long selectedAmPm { get; set; } = default!;
    public virtual long meridiemRegion { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController meridiemController { get; set; } = default!;
    public virtual bool isDatePickerScrolling { get; set; } = false;
    public virtual bool isHourPickerScrolling { get; set; } = false;
    public virtual bool isMinutePickerScrolling { get; set; } = false;
    public virtual bool isMeridiemPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; private set; } = new DartMap<long, double>();

    public virtual long selectedDayFromInitial
    {
        get
        {
            switch (((CupertinoDatePicker)this.widget).mode)
            {
                case CupertinoDatePickerMode.dateAndTime:
                    {
                        return (this.dateController.hasClients ? ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.dateController).selectedItem : 0L);
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
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            DartRuntimePrimitives.Assert(() => false, () => (object?)$"{this.GetType()} is only meant for dateAndTime mode or time mode");
            return 0L;
            return default!;
        }
    }
    public virtual long selectedHour => _selectedHour(this.selectedAmPm, this._selectedHourIndex);
    internal virtual long _selectedHourIndex => (this.hourController.hasClients ? (((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem % 24L) : this.initialDateTime.Hour);
    internal virtual long _selectedHour(long selectedAmPm, long selectedHour)
    {
        return (_isHourRegionFlipped(selectedAmPm) ? (((selectedHour + 12L)) % 24L) : selectedHour);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long selectedMinute
    {
        get
        {
            return (this.minuteController.hasClients ? ((((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.minuteController).selectedItem * ((CupertinoDatePicker)this.widget).minuteInterval) % 60L) : this.initialDateTime.Minute);
            return default!;
        }
    }
    public virtual bool isHourRegionFlipped => _isHourRegionFlipped(this.selectedAmPm);
    internal virtual bool _isHourRegionFlipped(long selectedAmPm) => DartRuntimePrimitives.ConvertValue<bool>((selectedAmPm != this.meridiemRegion));
    public virtual bool isScrolling
    {
        get
        {
            return (((this.isDatePickerScrolling || this.isHourPickerScrolling) || this.isMinutePickerScrolling) || this.isMeridiemPickerScrolling);
            return default!;
        }
    }
    public override void initState()
    {
        base.initState();
        initialDateTime = ((CupertinoDatePicker)this.widget).initialDateTime;
        selectedAmPm = (checked((long)(this.initialDateTime.Hour / 12L)));
        meridiemRegion = this.selectedAmPm;
        meridiemController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedAmPm);
        hourController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: this.initialDateTime.Hour);
        minuteController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(this.initialDateTime.Minute / ((CupertinoDatePicker)this.widget).minuteInterval))));
        dateController = new global::Doroti.Framework.Widgets.FixedExtentScrollController();
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() =>
        {
            this.estimatedColumnWidths.Clear();
        })));
    }

    public override void dispose()
    {
        this.dateController.dispose();
        this.hourController.dispose();
        this.minuteController.dispose();
        this.meridiemController.dispose();
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoDatePicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((CupertinoDatePicker)oldWidget).mode, ((CupertinoDatePicker)this.widget).mode)), () => (object?)$"The {this.GetType()}'s mode cannot change once it's built.");
        if ((!((CupertinoDatePicker)this.widget).use24hFormat && ((CupertinoDatePicker)oldWidget).use24hFormat))
        {
            this.meridiemController.dispose();
            meridiemController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedAmPm);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerLeft : global::Doroti.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerRight : global::Doroti.Framework.Painting.Alignment.centerLeft);
        this.estimatedColumnWidths.Clear();
    }

    internal virtual double _getEstimatedColumnWidth(_PickerColumnType__date_picker columnType)
    {
        this.estimatedColumnWidths.putIfAbsent(FoundationRuntimePorts.EnumIndex(columnType), () => CupertinoDatePicker._getColumnWidth(columnType, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek));
        return DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(columnType)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DateTime selectedDateTime
    {
        get
        {
            return DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + this.selectedDayFromInitial), this.selectedHour, this.selectedMinute);
            return default!;
        }
    }
    internal virtual void _onSelectedItemChange(long index)
    {
        bool isDateInvalid = (((((CupertinoDatePicker)this.widget).minimumDate?.isAfter(this.selectedDateTime) ?? false)) || ((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(this.selectedDateTime) ?? false)));
        if (isDateInvalid)
        {
            return;
        }
        else
        {
            if (!_isSelectableDate(this.selectedDateTime))
            {
                return;
            }
        }
        this.widget.onDateTimeChanged(this.selectedDateTime);
    }

    internal virtual bool _isSelectableDate(DateTime date)
    {
        return ((((CupertinoDatePicker)this.widget).selectableDayPredicate is null ? true : ((CupertinoDatePicker)this.widget).selectableDayPredicate.Invoke(date)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMediumDatePicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isDatePickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isDatePickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: CupertinoPicker.CreateBuilder(scrollController: this.dateController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            _onSelectedItemChange(index);
        })), itemBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, index) =>
        {
            var rangeStart = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + index));
            var rangeEnd = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, ((this.initialDateTime.Day + index) + 1L));
            var now = new DateTime();
            if ((((CupertinoDatePicker)this.widget).minimumDate?.isBefore(rangeEnd) == false))
            {
                return null;
            }
            if ((((CupertinoDatePicker)this.widget).maximumDate?.isAfter(rangeStart) == false))
            {
                return null;
            }
            string dateText = ((object.Equals(rangeStart, DartRuntimePrimitives.CreateDateTime(now.Year, now.Month, now.Day))) ? this.localizations.todayLabel : this.localizations.datePickerMediumDate(rangeStart));
            bool isDisabled = !_isSelectableDate(rangeStart);
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(context, new global::Doroti.Framework.Widgets.Text(dateText, style: Date_pickerLibrary._themeTextStyle(context, isValid: !isDisabled)));
            return (isDisabled ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isValidHour(long meridiemIndex, long hourIndex)
    {
        var rangeStart = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + this.selectedDayFromInitial), _selectedHour(meridiemIndex, hourIndex));
        DateTime rangeEnd = rangeStart.add(Duration.Create(hours: 1L));
        return (((((CupertinoDatePicker)this.widget).minimumDate?.isBefore(rangeEnd) ?? true)) && !((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(rangeStart) ?? false)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHourPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isHourPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isHourPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.hourController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            var regionChanged = (this.meridiemRegion != (checked((long)(index / 12L))));
            bool debugIsFlipped = this.isHourRegionFlipped;
            if (regionChanged)
            {
                meridiemRegion = (checked((long)(index / 12L)));
                selectedAmPm = (1L - this.selectedAmPm);
            }
            if ((!((CupertinoDatePicker)this.widget).use24hFormat && regionChanged))
            {
                DartRuntimePrimitives.Ignore(this.meridiemController.animateToItem(this.selectedAmPm, duration: Duration.Create(milliseconds: 300L), curve: global::Doroti.Framework.Animation.Curves.easeOut));
            }
            else
            {
                _onSelectedItemChange(index);
            }
            DartRuntimePrimitives.Assert(() => (debugIsFlipped == this.isHourRegionFlipped));
        })), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)24L)), ((index) =>
        {
            long hour = (this.isHourRegionFlipped ? (((index + 12L)) % 24L) : index);
            long displayHour = (((CupertinoDatePicker)this.widget).use24hFormat ? hour : ((((hour + 11L)) % 12L) + 1L));
            bool isDisabled = !_isValidHour(this.selectedAmPm, index);
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(this.localizations.datePickerHour(displayHour), semanticsLabel: this.localizations.datePickerHourSemanticsLabel(displayHour), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isDisabled)));
            return (isDisabled ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMinutePicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isMinutePickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isMinutePickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.minuteController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: (global::System.Action<long>)this._onSelectedItemChange, looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoDatePicker)this.widget).minuteInterval))))), ((index) =>
        {
            long minute = (index * ((CupertinoDatePicker)this.widget).minuteInterval);
            var date = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + this.selectedDayFromInitial), this.selectedHour, minute);
            bool isInvalidMinute = (((((CupertinoDatePicker)this.widget).minimumDate?.isAfter(date) ?? false)) || ((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(date) ?? false)));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(this.localizations.datePickerMinute(minute), semanticsLabel: this.localizations.datePickerMinuteSemanticsLabel(minute), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMinute)));
            return (isInvalidMinute ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildAmPmPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isMeridiemPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isMeridiemPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.meridiemController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedAmPm = index;
            DartRuntimePrimitives.Assert(() => ((this.selectedAmPm == 0L) || (this.selectedAmPm == 1L)));
            _onSelectedItemChange(index);
        })), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)2L)), ((index) =>
        {
            bool isDisabled = !_isValidHour(index, this._selectedHourIndex);
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(((index == 0L) ? this.localizations.anteMeridiemAbbreviation : this.localizations.postMeridiemAbbreviation), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isDisabled)));
            return (isDisabled ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildTimeSeparatorWidget(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ExcludeSemantics(child: new CupertinoPicker(offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
        })), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)1L)), ((index) =>
        {
            return itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(":", style: Date_pickerLibrary._themeTextStyle(this.context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _scrollToFirstSelectableDate()
    {
        if (!_isSelectableDate(this.selectedDateTime))
        {
            var daysThreshold = 1L;
            DateTime targetDate = this.selectedDateTime.add(Duration.Create(days: daysThreshold));
            _scrollToDate(targetDate, this.selectedDateTime, false, focusedIndex: (((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.dateController).selectedItem + daysThreshold));
        }
    }

    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() =>
        {
        })));
        if (this.isScrolling)
        {
            return;
        }
        DateTime selectedDate = this.selectedDateTime;
        bool minCheck = (((CupertinoDatePicker)this.widget).minimumDate?.isAfter(selectedDate) ?? false);
        bool maxCheck = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(selectedDate) ?? false);
        _scrollToFirstSelectableDate();
        if ((minCheck || maxCheck))
        {
            DateTime targetDate = (minCheck ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate));
            _scrollToDate(targetDate, selectedDate, minCheck);
        }
    }

    internal virtual void _scrollToDate(DateTime newDate, DateTime fromDate, bool minCheck, long? focusedIndex = null)
    {
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
        {
            if ((((fromDate.Year != newDate.Year) || (fromDate.Month != newDate.Month)) || (fromDate.Day != newDate.Day)))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.dateController, ((focusedIndex ?? (long)this.selectedDayFromInitial)));
            }
            if ((fromDate.Hour != newDate.Hour))
            {
                bool needsMeridiemChange = (!((CupertinoDatePicker)this.widget).use24hFormat && ((checked((long)(fromDate.Hour / 12L))) != (checked((long)(newDate.Hour / 12L)))));
                if (needsMeridiemChange)
                {
                    Date_pickerLibrary._animateColumnControllerToItem(this.meridiemController, (1L - ((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.meridiemController).selectedItem));
                    long newItem = ((((checked((long)(((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem / 12L)))) * 12L) + ((((((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem + newDate.Hour) - fromDate.Hour)) % 12L));
                    Date_pickerLibrary._animateColumnControllerToItem(this.hourController, newItem);
                }
                else
                {
                    Date_pickerLibrary._animateColumnControllerToItem(this.hourController, ((((global::Doroti.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem + newDate.Hour) - fromDate.Hour));
                }
            }
            if ((fromDate.Minute != newDate.Minute))
            {
                double positionDouble = (newDate.Minute / ((CupertinoDatePicker)this.widget).minuteInterval);
                long position = (minCheck ? positionDouble.ceil() : positionDouble.floor());
                Date_pickerLibrary._animateColumnControllerToItem(this.minuteController, position);
            }
        })), debugLabel: "DatePicker.scrollToDate");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var columnWidthsLocal = new List<double> { _getEstimatedColumnWidth(_PickerColumnType__date_picker.hour), _getEstimatedColumnWidth(_PickerColumnType__date_picker.minute) };
        var pickerBuilders = ((List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>)((object.Equals(Directionality.of(context), TextDirection.rtl)) ? new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildMinutePicker, this._buildHourPicker } : new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildHourPicker, this._buildMinutePicker }));
        if (((CupertinoDatePicker)this.widget).showTimeSeparator)
        {
            columnWidthsLocal.Insert(checked((int)1L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.timeSeparator));
            pickerBuilders.Insert(checked((int)1L), this._buildTimeSeparatorWidget);
        }
        if (!((CupertinoDatePicker)this.widget).use24hFormat)
        {
            switch (this.localizations.datePickerDateTimeOrder)
            {
                case var __constant47363 when (object.Equals(__constant47363, DatePickerDateTimeOrder.date_time_dayPeriod)):
                case var __constant47421 when (object.Equals(__constant47421, DatePickerDateTimeOrder.time_dayPeriod_date)):
                    {
                        pickerBuilders.Add(this._buildAmPmPicker);
                        columnWidthsLocal.Add(_getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod));
                        break;
                    }
                case var __constant47610 when (object.Equals(__constant47610, DatePickerDateTimeOrder.date_dayPeriod_time)):
                case var __constant47668 when (object.Equals(__constant47668, DatePickerDateTimeOrder.dayPeriod_time_date)):
                    {
                        pickerBuilders.Insert(checked((int)0L), this._buildAmPmPicker);
                        columnWidthsLocal.Insert(checked((int)0L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod));
                        break;
                    }
            }
        }
        if ((object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.dateAndTime)))
        {
            switch (this.localizations.datePickerDateTimeOrder)
            {
                case var __constant48071 when (object.Equals(__constant48071, DatePickerDateTimeOrder.time_dayPeriod_date)):
                case var __constant48129 when (object.Equals(__constant48129, DatePickerDateTimeOrder.dayPeriod_time_date)):
                    {
                        pickerBuilders.Add(this._buildMediumDatePicker);
                        columnWidthsLocal.Add(_getEstimatedColumnWidth(_PickerColumnType__date_picker.date));
                        break;
                    }
                case var __constant48319 when (object.Equals(__constant48319, DatePickerDateTimeOrder.date_time_dayPeriod)):
                case var __constant48377 when (object.Equals(__constant48377, DatePickerDateTimeOrder.date_dayPeriod_time)):
                    {
                        pickerBuilders.Insert(checked((int)0L), this._buildMediumDatePicker);
                        columnWidthsLocal.Insert(checked((int)0L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.date));
                        break;
                    }
            }
        }
        var pickers = new List<global::Doroti.Framework.Widgets.Widget>();
        double totalColumnWidths = (4L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i, width) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = ((i == 0L), (i == (checked((long)(columnWidthsLocal.Count)) - 1L)));
            var offAxisFraction = 0.0;
            global::Doroti.Framework.Widgets.Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i, columnCount: checked((long)(columnWidthsLocal.Count)));
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
                offAxisFraction = (-_kMaximumOffAxisFraction * this.textDirectionFactor);
            }
            else
            {
                if (((i >= 2L) || (checked((long)(columnWidthsLocal.Count)) == 2L)))
                {
                    offAxisFraction = (_kMaximumOffAxisFraction * this.textDirectionFactor);
                }
            }
            var paddingLocal = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if (lastColumn)
            {
                paddingLocal = ((global::Doroti.Framework.Painting.EdgeInsets)paddingLocal).flipped;
            }
            if ((this.textDirectionFactor == -1L))
            {
                paddingLocal = ((global::Doroti.Framework.Painting.EdgeInsets)paddingLocal).flipped;
            }
            totalColumnWidths += (width + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            pickers.Add(new global::Doroti.Framework.Widgets.LayoutId(id: i, child: pickerBuilders[(int)(i)](offAxisFraction, ((context, child) =>
            {
                global::Doroti.Framework.Widgets.Widget constrained = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxWidth: (width + Date_pickerLibrary._kDatePickerPadSize)), child: child));
                return new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal, child: new global::Doroti.Framework.Widgets.Align(alignment: (lastColumn ? this.alignCenterLeft : this.alignCenterRight), child: ((firstColumn || lastColumn) ? constrained : child)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), selectionOverlay)));
        }
        double maxPickerWidth = ((totalColumnWidths > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidthsLocal, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth), children: pickers))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDatePickerDateState__date_picker : global::Doroti.Framework.Widgets.State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedDay { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController dayController { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController monthController { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController yearController { get; set; } = default!;
    public virtual bool isDayPickerScrolling { get; set; } = false;
    public virtual bool isMonthPickerScrolling { get; set; } = false;
    public virtual bool isYearPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; set; } = new DartMap<long, double>();

    internal _CupertinoDatePickerDateState__date_picker(DatePickerDateOrder? dateOrder)
    {
        this.dateOrder = dateOrder;
    }

    public virtual bool isScrolling => DartRuntimePrimitives.ConvertValue<bool>(((this.isDayPickerScrolling || this.isMonthPickerScrolling) || this.isYearPickerScrolling));
    public override void initState()
    {
        base.initState();
        selectedDay = ((CupertinoDatePicker)this.widget).initialDateTime.Day;
        selectedMonth = ((CupertinoDatePicker)this.widget).initialDateTime.Month;
        selectedYear = ((CupertinoDatePicker)this.widget).initialDateTime.Year;
        dayController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedDay - 1L));
        monthController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedMonth - 1L));
        yearController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedYear);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() =>
        {
            _refreshEstimatedColumnWidths();
        })));
    }

    public override void dispose()
    {
        this.dayController.dispose();
        this.monthController.dispose();
        this.yearController.dispose();
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerLeft : global::Doroti.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerRight : global::Doroti.Framework.Painting.Alignment.centerLeft);
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.dayOfMonth, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.month, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.year, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
    }

    internal virtual DateTime _lastDayInMonth(long year, long month) => DartRuntimePrimitives.CreateDateTime(year, (month + 1L), 0L);
    internal virtual global::Doroti.Framework.Widgets.Widget _buildDayPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        long daysInCurrentMonth = _lastDayInMonth(this.selectedYear, this.selectedMonth).Day;
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isDayPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isDayPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.dayController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedDay = (index + 1L);
            if (this._isCurrentDateValid)
            {
                this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
            }
        })), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)31L)), ((index) =>
        {
            long day = (index + 1L);
            long? dayOfWeek = (((CupertinoDatePicker)this.widget).showDayOfWeek ? DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, day).DayOfWeek.ToDartWeekday() : null);
            bool isInvalidDay = ((((day > daysInCurrentMonth)) || ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month == this.selectedMonth)) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Day > day)))) || ((((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month == this.selectedMonth)) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Day < day))));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(this.localizations.datePickerDayOfMonth(day, dayOfWeek), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidDay)));
            return (isInvalidDay ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMonthPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isMonthPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isMonthPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.monthController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedMonth = (index + 1L);
            if (this._isCurrentDateValid)
            {
                this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
            }
        })), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)12L)), ((index) =>
        {
            long month = (index + 1L);
            bool isInvalidMonth = ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month > month))) || (((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month < month))));
            string monthName = (((object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear))) ? this.localizations.datePickerStandaloneMonth(month) : this.localizations.datePickerMonth(month));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(monthName, style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMonth)));
            return (isInvalidMonth ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildYearPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isYearPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isYearPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: CupertinoPicker.CreateBuilder(scrollController: this.yearController, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, offAxisFraction: offAxisFraction, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedYear = index;
            if (this._isCurrentDateValid)
            {
                this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
            }
        })), itemBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, year) =>
        {
            if ((year < ((CupertinoDatePicker)this.widget).minimumYear))
            {
                return null;
            }
            if (((((CupertinoDatePicker)this.widget).maximumYear is not null) && (year > DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumYear))))
            {
                return null;
            }
            bool isValidYear = ((((((CupertinoDatePicker)this.widget).minimumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Year <= year))) && (((((CupertinoDatePicker)this.widget).maximumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Year >= year))));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(context, new global::Doroti.Framework.Widgets.Text(this.localizations.datePickerYear(year), style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear)));
            return (isValidYear ? childLocal : new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay);
            var maxSelectedDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (this.selectedDay + 1L));
            bool minCheck = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectedDate) ?? true);
            bool maxCheck = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectedDate) ?? false);
            return ((minCheck && !maxCheck) && (minSelectedDate.Day == this.selectedDay));
            return default!;
        }
    }
    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() =>
        {
        })));
        if (this.isScrolling)
        {
            return;
        }
        var minSelectDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay);
        var maxSelectDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (this.selectedDay + 1L));
        bool minCheck = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectDate) ?? true);
        bool maxCheck = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectDate) ?? false);
        if ((!minCheck || maxCheck))
        {
            DateTime targetDate = (minCheck ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate));
            _scrollToDate(targetDate);
            return;
        }
        if ((minSelectDate.Day != this.selectedDay))
        {
            DateTime lastDay = _lastDayInMonth(this.selectedYear, this.selectedMonth);
            _scrollToDate(lastDay);
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
        {
            if ((this.selectedYear != newDate.Year))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.yearController, newDate.Year);
            }
            if ((this.selectedMonth != newDate.Month))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.monthController, (newDate.Month - 1L));
            }
            if ((this.selectedDay != newDate.Day))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.dayController, (newDate.Day - 1L));
            }
        })), debugLabel: "DatePicker.scrollToDate");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>();
        var columnWidthsLocal = new List<double>();
        DatePickerDateOrder datePickerDateOrderLocal = (this.dateOrder ?? this.localizations.datePickerDateOrder);
        switch (datePickerDateOrderLocal)
        {
            case var __constant63400 when (object.Equals(__constant63400, DatePickerDateOrder.mdy)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildMonthPicker, this._buildDayPicker, this._buildYearPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant63776 when (object.Equals(__constant63776, DatePickerDateOrder.dmy)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildDayPicker, this._buildMonthPicker, this._buildYearPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant64152 when (object.Equals(__constant64152, DatePickerDateOrder.ymd)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildMonthPicker, this._buildDayPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))) };
                    break;
                }
            case var __constant64528 when (object.Equals(__constant64528, DatePickerDateOrder.ydm)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildDayPicker, this._buildMonthPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))) };
                    break;
                }
        }
        var pickers = new List<global::Doroti.Framework.Widgets.Widget>();
        double totalColumnWidths = (4L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i, widthLocal) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = ((i == 0L), (i == (checked((long)(columnWidthsLocal.Count)) - 1L)));
            double offAxisFraction = ((((i - 1L)) * 0.3) * this.textDirectionFactor);
            var paddingLocal = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if ((this.textDirectionFactor == -1L))
            {
                paddingLocal = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(left: Date_pickerLibrary._kDatePickerPadSize);
            }
            global::Doroti.Framework.Widgets.Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i, columnCount: checked((long)(columnWidthsLocal.Count)));
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
            totalColumnWidths += (widthLocal + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            pickers.Add(new global::Doroti.Framework.Widgets.LayoutId(id: i, child: pickerBuilders[(int)(i)](offAxisFraction, ((context, child) =>
            {
                return new global::Doroti.Framework.Widgets.Padding(padding: (firstColumn ? global::Doroti.Framework.Painting.EdgeInsets.zero : paddingLocal), child: new global::Doroti.Framework.Widgets.Align(alignment: (lastColumn ? this.alignCenterLeft : this.alignCenterRight), child: new global::Doroti.Framework.Widgets.SizedBox(width: (widthLocal + Date_pickerLibrary._kDatePickerPadSize), child: new global::Doroti.Framework.Widgets.Align(alignment: (firstColumn ? this.alignCenterLeft : this.alignCenterRight), child: child))));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), selectionOverlay)));
        }
        double maxPickerWidth = ((totalColumnWidths > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidthsLocal, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth), children: pickers))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDatePickerMonthYearState__date_picker : global::Doroti.Framework.Widgets.State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController monthController { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FixedExtentScrollController yearController { get; set; } = default!;
    public virtual bool isMonthPickerScrolling { get; set; } = false;
    public virtual bool isYearPickerScrolling { get; set; } = false;
    public virtual DartMap<long, double> estimatedColumnWidths { get; set; } = new DartMap<long, double>();

    internal _CupertinoDatePickerMonthYearState__date_picker(DatePickerDateOrder? dateOrder)
    {
        this.dateOrder = dateOrder;
    }

    public virtual bool isScrolling => DartRuntimePrimitives.ConvertValue<bool>((this.isMonthPickerScrolling || this.isYearPickerScrolling));
    public override void initState()
    {
        base.initState();
        selectedMonth = ((CupertinoDatePicker)this.widget).initialDateTime.Month;
        selectedYear = ((CupertinoDatePicker)this.widget).initialDateTime.Year;
        monthController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedMonth - 1L));
        yearController = new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedYear);
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() =>
        {
            _refreshEstimatedColumnWidths();
        })));
    }

    public override void dispose()
    {
        this.monthController.dispose();
        this.yearController.dispose();
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerLeft : global::Doroti.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Framework.Painting.Alignment.centerRight : global::Doroti.Framework.Painting.Alignment.centerLeft);
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.month, this.localizations, this.context, false, standaloneMonth: (object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear)));
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.year, this.localizations, this.context, false);
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMonthPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isMonthPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isMonthPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new CupertinoPicker(scrollController: this.monthController, offAxisFraction: offAxisFraction, itemExtent: Date_pickerLibrary._kItemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedMonth = (index + 1L);
            if (this._isCurrentDateValid)
            {
                this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth));
            }
        })), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)12L)), ((index) =>
        {
            long month = (index + 1L);
            bool isInvalidMonth = ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month > month))) || (((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month < month))));
            string monthName = (((object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear))) ? this.localizations.datePickerStandaloneMonth(month) : this.localizations.datePickerMonth(month));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(this.context, new global::Doroti.Framework.Widgets.Text(monthName, style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMonth)));
            return (isInvalidMonth ? new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal) : childLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildYearPicker(double offAxisFraction, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool>)((notification) =>
        {
            if ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification))
            {
                isYearPickerScrolling = true;
            }
            else
            {
                if ((notification is global::Doroti.Framework.Widgets.ScrollEndNotification))
                {
                    isYearPickerScrolling = false;
                    _pickerDidStopScrolling();
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: CupertinoPicker.CreateBuilder(scrollController: this.yearController, itemExtent: Date_pickerLibrary._kItemExtent, offAxisFraction: offAxisFraction, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            selectedYear = index;
            if (this._isCurrentDateValid)
            {
                this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth));
            }
        })), itemBuilder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, year) =>
        {
            if ((year < ((CupertinoDatePicker)this.widget).minimumYear))
            {
                return null;
            }
            if (((((CupertinoDatePicker)this.widget).maximumYear is not null) && (year > DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumYear))))
            {
                return null;
            }
            bool isValidYear = ((((((CupertinoDatePicker)this.widget).minimumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Year <= year))) && (((((CupertinoDatePicker)this.widget).maximumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Year >= year))));
            global::Doroti.Framework.Widgets.Widget childLocal = itemPositioningBuilder(context, new global::Doroti.Framework.Widgets.Text(this.localizations.datePickerYear(year), style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear)));
            return (isValidYear ? childLocal : new global::Doroti.Framework.Widgets.ExcludeSemantics(child: childLocal));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth);
            var maxSelectedDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (((CupertinoDatePicker)this.widget).initialDateTime.Day + 1L));
            bool minCheck = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectedDate) ?? true);
            bool maxCheck = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectedDate) ?? false);
            return (minCheck && !maxCheck);
            return default!;
        }
    }
    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() =>
        {
        })));
        if (this.isScrolling)
        {
            return;
        }
        var minSelectDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth);
        var maxSelectDate = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (((CupertinoDatePicker)this.widget).initialDateTime.Day + 1L));
        bool minCheck = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectDate) ?? true);
        bool maxCheck = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectDate) ?? false);
        if ((!minCheck || maxCheck))
        {
            DateTime targetDate = (minCheck ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate));
            _scrollToDate(targetDate);
            return;
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) =>
        {
            if ((this.selectedYear != newDate.Year))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.yearController, newDate.Year);
            }
            if ((this.selectedMonth != newDate.Month))
            {
                Date_pickerLibrary._animateColumnControllerToItem(this.monthController, (newDate.Month - 1L));
            }
        })), debugLabel: "DatePicker.scrollToDate");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>>();
        var columnWidthsLocal = new List<double>();
        DatePickerDateOrder datePickerDateOrderLocal = (this.dateOrder ?? this.localizations.datePickerDateOrder);
        switch (datePickerDateOrderLocal)
        {
            case var __constant76081 when (object.Equals(__constant76081, DatePickerDateOrder.mdy)):
            case var __constant76117 when (object.Equals(__constant76117, DatePickerDateOrder.dmy)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildMonthPicker, this._buildYearPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant76406 when (object.Equals(__constant76406, DatePickerDateOrder.ymd)):
            case var __constant76442 when (object.Equals(__constant76442, DatePickerDateOrder.ydm)):
                {
                    pickerBuilders = new List<global::System.Func<double, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildMonthPicker };
                    columnWidthsLocal = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))) };
                    break;
                }
        }
        var pickers = new List<global::Doroti.Framework.Widgets.Widget>();
        double totalColumnWidths = (3L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i, widthLocal) in columnWidthsLocal.indexed())
        {
            var (firstColumn, lastColumn) = ((i == 0L), (i == (checked((long)(columnWidthsLocal.Count)) - 1L)));
            double offAxisFraction = (this.textDirectionFactor * ((firstColumn ? -0.3 : 0.5)));
            totalColumnWidths += (widthLocal + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            global::Doroti.Framework.Widgets.Widget? selectionOverlay = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i, columnCount: checked((long)(columnWidthsLocal.Count)));
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
            pickers.Add(new global::Doroti.Framework.Widgets.LayoutId(id: i, child: pickerBuilders[(int)(i)](offAxisFraction, ((context, child) =>
            {
                global::Doroti.Framework.Widgets.Widget contents = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: (lastColumn ? this.alignCenterLeft : this.alignCenterRight), child: new global::Doroti.Framework.Widgets.SizedBox(width: (widthLocal + Date_pickerLibrary._kDatePickerPadSize), child: new global::Doroti.Framework.Widgets.Align(alignment: (firstColumn ? this.alignCenterLeft : this.alignCenterRight), child: child))));
                if (firstColumn)
                {
                    return contents;
                }
                var paddingLocal = global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: ((this.textDirectionFactor == -1L) ? ((global::Doroti.Framework.Painting.EdgeInsets)paddingLocal).flipped : paddingLocal), child: contents));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), selectionOverlay)));
        }
        double maxPickerWidth = ((totalColumnWidths > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidthsLocal, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth), children: pickers))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum CupertinoTimerPickerMode
{
    hm,
    ms,
    hms
}

public class CupertinoTimerPicker : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual CupertinoTimerPickerMode mode { get; private set; } = default!;
    public virtual Duration initialTimerDuration { get; private set; } = default!;
    public virtual long minuteInterval { get; private set; } = default!;
    public virtual long secondInterval { get; private set; } = default!;
    public virtual global::System.Action<Duration> onTimerDurationChanged { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual SelectionOverlayBuilder? selectionOverlayBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoTimerPicker(global::Doroti.Framework.Foundation.Key? key = null, CupertinoTimerPickerMode mode = CupertinoTimerPickerMode.hms, Duration initialTimerDuration = default, long minuteInterval = 1, long secondInterval = 1, global::Doroti.Framework.Painting.AlignmentGeometry alignment = default!, Color? backgroundColor = null, double? itemExtent = null, global::System.Action<Duration> onTimerDurationChanged = default!, global::Doroti.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate, SelectionOverlayBuilder? selectionOverlayBuilder = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Framework.Painting.Alignment.center;
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
        System.Diagnostics.Debug.Assert((initialTimerDuration >= Duration.zero));
        System.Diagnostics.Debug.Assert((initialTimerDuration < Duration.Create(days: 1L)));
        System.Diagnostics.Debug.Assert(((minuteInterval > 0L) && ((60L % minuteInterval) == 0L)));
        System.Diagnostics.Debug.Assert(((secondInterval > 0L) && ((60L % secondInterval) == 0L)));
        System.Diagnostics.Debug.Assert(((initialTimerDuration.inMinutes % minuteInterval) == 0L));
        System.Diagnostics.Debug.Assert(((initialTimerDuration.inSeconds % secondInterval) == 0L));
        System.Diagnostics.Debug.Assert((__itemExtent > 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoTimerPickerState__date_picker());
}

internal class _CupertinoTimerPickerState__date_picker : global::Doroti.Framework.Widgets.State<CupertinoTimerPicker>
{
    public virtual TextDirection textDirection { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual long? selectedHour { get; set; } = default;
    public virtual long selectedMinute { get; set; } = default!;
    public virtual long? selectedSecond { get; set; } = default;
    public virtual long? lastSelectedHour { get; set; } = default;
    public virtual long? lastSelectedMinute { get; set; } = default;
    public virtual long? lastSelectedSecond { get; set; } = default;
    public virtual global::Doroti.Framework.Painting.TextPainter textPainter { get; private set; } = new global::Doroti.Framework.Painting.TextPainter();
    public virtual List<string> numbers { get; private set; } = new List<string>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)10L)), ((i) => $"{(9L - i)}")));
    public virtual double numberLabelWidth { get; set; } = default!;
    public virtual double numberLabelHeight { get; set; } = default!;
    public virtual double numberLabelBaseline { get; set; } = default!;
    public virtual double hourLabelWidth { get; set; } = default!;
    public virtual double minuteLabelWidth { get; set; } = default!;
    public virtual double secondLabelWidth { get; set; } = default!;
    public virtual double totalWidth { get; set; } = default!;
    public virtual double pickerColumnWidth { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController? _hourScrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController? _minuteScrollController { get; set; } = default;
    internal virtual global::Doroti.Framework.Widgets.FixedExtentScrollController? _secondScrollController { get; set; } = default;

    public virtual long textDirectionFactor => (this.textDirection switch { TextDirection.ltr => 1L, TextDirection.rtl => -1L, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override void initState()
    {
        base.initState();
        selectedMinute = (((CupertinoTimerPicker)this.widget).initialTimerDuration.inMinutes % 60L);
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)))
        {
            selectedHour = ((CupertinoTimerPicker)this.widget).initialTimerDuration.inHours;
        }
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hm)))
        {
            selectedSecond = (((CupertinoTimerPicker)this.widget).initialTimerDuration.inSeconds % 60L);
        }
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() =>
        {
            this.textPainter.markNeedsLayout();
            _measureLabelMetrics();
        })));
    }

    public override void dispose()
    {
        global::Doroti.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        this.textPainter.dispose();
        this._hourScrollController?.dispose();
        this._minuteScrollController?.dispose();
        this._secondScrollController?.dispose();
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoTimerPicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((CupertinoTimerPicker)oldWidget).mode, ((CupertinoTimerPicker)this.widget).mode)), () => (object?)"The CupertinoTimerPicker's mode cannot change once it's built");
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirection = Directionality.of(this.context);
        localizations = CupertinoLocalizations.of(this.context);
        _measureLabelMetrics();
    }

    internal virtual void _measureLabelMetrics()
    {
        ((dynamic)this.textPainter).textDirection = this.textDirection;
        global::Doroti.Framework.Painting.TextStyle textStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)_textStyleFrom(this.context, Date_pickerLibrary._kTimerPickerMagnification));
        double maxWidth = double.NegativeInfinity;
        string? widestNumber = default!;
        foreach (string input in this.numbers)
        {
            this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.InlineSpan>(new global::Doroti.Framework.Painting.TextSpan(text: input, style: textStyle));
            this.textPainter.layout();
            if ((((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth > maxWidth))
            {
                maxWidth = ((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
                widestNumber = input;
            }
        }
        this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.InlineSpan>(new global::Doroti.Framework.Painting.TextSpan(text: $"{widestNumber}{widestNumber}", style: textStyle));
        this.textPainter.layout();
        numberLabelWidth = ((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
        numberLabelHeight = ((global::Doroti.Framework.Painting.TextPainter)this.textPainter).height;
        numberLabelBaseline = this.textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        minuteLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerMinuteLabels.Cast<string?>().ToList(), textStyle);
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)))
        {
            hourLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerHourLabels.Cast<string?>().ToList(), textStyle);
        }
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hm)))
        {
            secondLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerSecondLabels.Cast<string?>().ToList(), textStyle);
        }
    }

    internal virtual double _measureLabelsMaxWidth(List<string?> labels, global::Doroti.Framework.Painting.TextStyle style)
    {
        double maxWidth = double.NegativeInfinity;
        for (var i = 0L; (i < checked((long)(labels.Count))); i++)
        {
            string? label = labels[(int)(i)];
            if ((label is null))
            {
                continue;
            }
            this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.InlineSpan>(new global::Doroti.Framework.Painting.TextSpan(text: label, style: style));
            this.textPainter.layout();
            DartRuntimePrimitives.Ignore(((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth);
            if ((((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth > maxWidth))
            {
                maxWidth = ((global::Doroti.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
            }
        }
        return maxWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildLabel(string text, global::Doroti.Framework.Painting.EdgeInsetsDirectional pickerPadding)
    {
        var paddingLocal = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.numberLabelWidth + Date_pickerLibrary._kTimerPickerLabelPadSize) + ((global::Doroti.Framework.Painting.EdgeInsetsDirectional)pickerPadding).start));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.IgnorePointer(child: new global::Doroti.Framework.Widgets.Padding(padding: paddingLocal.resolve(this.textDirection), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart.resolve(this.textDirection), child: new global::Doroti.Framework.Widgets.SizedBox(height: this.numberLabelHeight, child: new global::Doroti.Framework.Widgets.Baseline(baseline: this.numberLabelBaseline, baselineType: TextBaseline.alphabetic, child: new global::Doroti.Framework.Widgets.Text(text, style: new global::Doroti.Framework.Painting.TextStyle(fontSize: Date_pickerLibrary._kTimerPickerLabelFontSize, fontWeight: FontWeight.w600), maxLines: 1L, softWrap: false)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildPickerNumberLabel(string text, global::Doroti.Framework.Painting.EdgeInsetsDirectional padding)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: (Date_pickerLibrary._kTimerPickerColumnIntrinsicWidth + padding.horizontal), child: new global::Doroti.Framework.Widgets.Padding(padding: padding.resolve(this.textDirection), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart.resolve(this.textDirection), child: new global::Doroti.Framework.Widgets.SizedBox(width: this.numberLabelWidth, child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd.resolve(this.textDirection), child: new global::Doroti.Framework.Widgets.Text(text, softWrap: false, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.visible)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHourPicker(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        _hourScrollController ??= new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: DartRuntimePrimitives.RequireValue(this.selectedHour));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._hourScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0L), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            setState(((global::System.Action)(() =>
            {
                selectedHour = index;
                this.widget.onTimerDurationChanged(Duration.Create(hours: DartRuntimePrimitives.RequireValue(this.selectedHour), minutes: this.selectedMinute, seconds: (this.selectedSecond ?? 0L)));
            })));
        })), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)24L)), ((index) =>
        {
            string labelLocal = (this.localizations.timerPickerHourLabel(index) ?? "");
            string semanticsLabel = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerHour(index) + labelLocal) : (labelLocal + this.localizations.timerPickerHour(index)));
            return new global::Doroti.Framework.Widgets.Semantics(label: semanticsLabel, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerHour(index), additionalPadding));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildHourColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedHour = this.selectedHour;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildHourPicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerHourLabel((this.lastSelectedHour ?? DartRuntimePrimitives.RequireValue(this.selectedHour))) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMinutePicker(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        _minuteScrollController ??= new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(this.selectedMinute / ((CupertinoTimerPicker)this.widget).minuteInterval))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._minuteScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, ((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)) ? 0L : 1L)), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, looping: true, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            setState(((global::System.Action)(() =>
            {
                selectedMinute = (index * ((CupertinoTimerPicker)this.widget).minuteInterval);
                this.widget.onTimerDurationChanged(Duration.Create(hours: (this.selectedHour ?? 0L), minutes: this.selectedMinute, seconds: (this.selectedSecond ?? 0L)));
            })));
        })), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoTimerPicker)this.widget).minuteInterval))))), ((index) =>
        {
            long minute = (index * ((CupertinoTimerPicker)this.widget).minuteInterval);
            string labelLocal = (this.localizations.timerPickerMinuteLabel(minute) ?? "");
            string semanticsLabel = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerMinute(minute) + labelLocal) : (labelLocal + this.localizations.timerPickerMinute(minute)));
            return new global::Doroti.Framework.Widgets.Semantics(label: semanticsLabel, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerMinute(minute), additionalPadding));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildMinuteColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedMinute = this.selectedMinute;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildMinutePicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerMinuteLabel((this.lastSelectedMinute ?? this.selectedMinute)) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSecondPicker(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        _secondScrollController ??= new global::Doroti.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(DartRuntimePrimitives.RequireValue(this.selectedSecond) / ((CupertinoTimerPicker)this.widget).secondInterval))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._secondScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, ((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)) ? 1L : 2L)), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, looping: true, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) =>
        {
            setState(((global::System.Action)(() =>
            {
                selectedSecond = (index * ((CupertinoTimerPicker)this.widget).secondInterval);
                this.widget.onTimerDurationChanged(Duration.Create(hours: (this.selectedHour ?? 0L), minutes: this.selectedMinute, seconds: DartRuntimePrimitives.RequireValue(this.selectedSecond)));
            })));
        })), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoTimerPicker)this.widget).secondInterval))))), ((index) =>
        {
            long second = (index * ((CupertinoTimerPicker)this.widget).secondInterval);
            string labelLocal = (this.localizations.timerPickerSecondLabel(second) ?? "");
            string semanticsLabel = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerSecond(second) + labelLocal) : (labelLocal + this.localizations.timerPickerSecond(second)));
            return new global::Doroti.Framework.Widgets.Semantics(label: semanticsLabel, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerSecond(second), additionalPadding));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSecondColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Stack(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedSecond = this.selectedSecond;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildSecondPicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerSecondLabel((this.lastSelectedSecond ?? DartRuntimePrimitives.RequireValue(this.selectedSecond))) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Painting.TextStyle _textStyleFrom(global::Doroti.Framework.Widgets.BuildContext context, double magnification = 1.0)
    {
        global::Doroti.Framework.Painting.TextStyle textStyle = CupertinoTheme.of(context).textTheme.pickerTextStyle;
        return ((global::Doroti.Framework.Painting.TextStyle)(object?)textStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Framework.Painting.TextStyle)textStyle).color, context), fontSize: (DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Painting.TextStyle)textStyle).fontSize) * magnification)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _calculateOffAxisFraction(double paddingStart, long position)
    {
        double centerPoint = (paddingStart + ((this.numberLabelWidth / 2L)));
        double pickerColumnOffAxisFraction = (0.5 - (centerPoint / this.pickerColumnWidth));
        double timerPickerOffAxisFraction = (0.5 - (((centerPoint + (this.pickerColumnWidth * position))) / this.totalWidth));
        return (((pickerColumnOffAxisFraction - timerPickerOffAxisFraction)) * this.textDirectionFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            List<global::Doroti.Framework.Widgets.Widget> columns = default!;
            if ((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hms)))
            {
                pickerColumnWidth = (Date_pickerLibrary._kTimerPickerColumnIntrinsicWidth + ((Date_pickerLibrary._kTimerPickerHalfColumnPadding * 2L)));
                totalWidth = (this.pickerColumnWidth * 3L);
            }
            else
            {
                totalWidth = Date_pickerLibrary._kPickerWidth;
                pickerColumnWidth = (this.totalWidth / 2L);
            }
            if ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth < this.totalWidth))
            {
                totalWidth = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth;
                pickerColumnWidth = (this.totalWidth / (((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hms)) ? 3L : 2L)));
            }
            double baseLabelContentWidth = (this.numberLabelWidth + Date_pickerLibrary._kTimerPickerLabelPadSize);
            double minuteLabelContentWidth = (baseLabelContentWidth + this.minuteLabelWidth);
            switch (((CupertinoTimerPicker)this.widget).mode)
            {
                case CupertinoTimerPickerMode.hm:
                    {
                        double hourLabelContentWidth = (baseLabelContentWidth + this.hourLabelWidth);
                        double hourColumnStartPadding = ((this.pickerColumnWidth - hourLabelContentWidth) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
                        if ((hourColumnStartPadding < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
                        {
                            hourColumnStartPadding = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        double minuteColumnEndPadding = ((this.pickerColumnWidth - minuteLabelContentWidth) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
                        if ((minuteColumnEndPadding < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
                        {
                            minuteColumnEndPadding = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        global::Doroti.Framework.Widgets.Widget? hourSelectionOverlay = Date_pickerLibrary._startSelectionOverlay;
                        global::Doroti.Framework.Widgets.Widget? minuteSelectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                        if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
                        {
                            hourSelectionOverlay = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 2L);
                            minuteSelectionOverlay = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 2L);
                        }
                        columns = new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildHourColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: hourColumnStartPadding, end: ((this.pickerColumnWidth - hourColumnStartPadding) - hourLabelContentWidth)), hourSelectionOverlay)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.pickerColumnWidth - minuteColumnEndPadding) - minuteLabelContentWidth), end: minuteColumnEndPadding), minuteSelectionOverlay)) };
                        break;
                    }
                case CupertinoTimerPickerMode.ms:
                    {
                        double secondLabelContentWidth = (baseLabelContentWidth + this.secondLabelWidth);
                        double secondColumnEndPadding = ((this.pickerColumnWidth - secondLabelContentWidth) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
                        if ((secondColumnEndPadding < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
                        {
                            secondColumnEndPadding = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        double minuteColumnStartPadding = ((this.pickerColumnWidth - minuteLabelContentWidth) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
                        if ((minuteColumnStartPadding < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
                        {
                            minuteColumnStartPadding = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
                        }
                        global::Doroti.Framework.Widgets.Widget? minuteSelectionOverlayLocal = Date_pickerLibrary._startSelectionOverlay;
                        global::Doroti.Framework.Widgets.Widget? secondSelectionOverlay = Date_pickerLibrary._endSelectionOverlay;
                        if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
                        {
                            minuteSelectionOverlayLocal = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 2L);
                            secondSelectionOverlay = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 2L);
                        }
                        columns = new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: minuteColumnStartPadding, end: ((this.pickerColumnWidth - minuteColumnStartPadding) - minuteLabelContentWidth)), minuteSelectionOverlayLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSecondColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.pickerColumnWidth - secondColumnEndPadding) - minuteLabelContentWidth), end: secondColumnEndPadding), secondSelectionOverlay)) };
                        break;
                    }
                case CupertinoTimerPickerMode.hms:
                    {
                        double hourColumnEndPadding = (((this.pickerColumnWidth - baseLabelContentWidth) - this.hourLabelWidth) - Date_pickerLibrary._kTimerPickerMinHorizontalPadding);
                        double minuteColumnPadding = (((this.pickerColumnWidth - minuteLabelContentWidth)) / 2L);
                        double secondColumnStartPadding = (((this.pickerColumnWidth - baseLabelContentWidth) - this.secondLabelWidth) - Date_pickerLibrary._kTimerPickerMinHorizontalPadding);
                        global::Doroti.Framework.Widgets.Widget? hourSelectionOverlayLocal = Date_pickerLibrary._startSelectionOverlay;
                        global::Doroti.Framework.Widgets.Widget? minuteSelectionOverlayAlternate = Date_pickerLibrary._centerSelectionOverlay;
                        global::Doroti.Framework.Widgets.Widget? secondSelectionOverlayLocal = Date_pickerLibrary._endSelectionOverlay;
                        if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
                        {
                            hourSelectionOverlayLocal = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 3L);
                            minuteSelectionOverlayAlternate = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 3L);
                            secondSelectionOverlayLocal = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 2L, columnCount: 3L);
                        }
                        columns = new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildHourColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Date_pickerLibrary._kTimerPickerMinHorizontalPadding, end: Math.Max(hourColumnEndPadding, 0)), hourSelectionOverlayLocal)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: minuteColumnPadding, end: minuteColumnPadding), minuteSelectionOverlayAlternate)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSecondColumn(global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(secondColumnStartPadding, 0), end: Date_pickerLibrary._kTimerPickerMinHorizontalPadding), secondSelectionOverlayLocal)) };
                        break;
                    }
            }
            global::Doroti.Framework.Widgets.Widget contents = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: this.totalWidth, height: Date_pickerLibrary._kPickerHeight, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: _textStyleFrom(context), child: new global::Doroti.Framework.Widgets.Row(children: columns.map<global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Expanded>(((child) => new global::Doroti.Framework.Widgets.Expanded(child: child))).ToList().Cast<global::Doroti.Framework.Widgets.Widget>().ToList()))));
            global::Doroti.Ui.Color? colorLocal = ((global::Doroti.Ui.Color?)(object?)CupertinoDynamicColor.maybeResolve(((CupertinoTimerPicker)this.widget).backgroundColor, context));
            if ((colorLocal is not null))
            {
                contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ColoredBox(color: colorLocal, child: contents));
            }
            CupertinoThemeData themeData = CupertinoTheme.of(context);
            return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new CupertinoTheme(data: themeData.copyWith(textTheme: themeData.textTheme.copyWith(pickerTextStyle: _textStyleFrom(context, Date_pickerLibrary._kTimerPickerMagnification))), child: new global::Doroti.Framework.Widgets.Align(alignment: ((CupertinoTimerPicker)this.widget).alignment, child: contents))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
