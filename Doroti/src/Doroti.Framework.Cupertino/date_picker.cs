// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/date_picker.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

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
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _kDefaultPickerTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(letterSpacing: -0.83);
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
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _themeTextStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context, bool isValid = true)
    {
        global::Doroti.Generated.Framework.Painting.TextStyle style__2220 = CupertinoTheme.of(context).textTheme.dateTimePickerTextStyle;
        return (isValid ? style__2220.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Generated.Framework.Painting.TextStyle)style__2220).color, context)) : style__2220.copyWith(color: CupertinoDynamicColor.resolve(CupertinoColors.inactiveGray, context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Date_pickerLibrary
{
    internal static void _animateColumnControllerToItem(global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController controller, long targetItem)
    {
        DartRuntimePrimitives.Ignore(controller.animateToItem(targetItem, curve: global::Doroti.Generated.Framework.Animation.Curves.easeInOut, duration: Duration.Create(milliseconds: 200L)));
    }
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _startSelectionOverlay = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capEndEdge: false));
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _centerSelectionOverlay = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capStartEdge: false, capEndEdge: false));
}

public static partial class Date_pickerLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _endSelectionOverlay = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPickerDefaultSelectionOverlay(capStartEdge: false));
}

public delegate global::Doroti.Generated.Framework.Widgets.Widget? SelectionOverlayBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, long columnCount, long selectedIndex);

internal class _DatePickerLayoutDelegate__date_picker : global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate
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
        double remainingWidth__4301 = ((this.maxWidth < size.width) ? this.maxWidth : size.width);
        double currentHorizontalOffset__4377 = (((size.width - remainingWidth__4301)) / 2L);
        for (var i__4452 = 0L; (i__4452 < checked((long)(this.columnWidths.Count))); i__4452++)
        {
            remainingWidth__4301 -= (this.columnWidths[(int)(i__4452)] + (Date_pickerLibrary._kDatePickerPadSize * 2L));
        }
        for (var i__4578 = 0L; (i__4578 < checked((long)(this.columnWidths.Count))); i__4578++)
        {
            long index__4633 = ((this.textDirectionFactor == 1L) ? i__4578 : ((checked((long)(this.columnWidths.Count)) - i__4578) - 1L));
            double childWidth__4715 = (this.columnWidths[(int)(index__4633)] + (Date_pickerLibrary._kDatePickerPadSize * 2L));
            if (((index__4633 == 0L) || (index__4633 == (checked((long)(this.columnWidths.Count)) - 1L))))
            {
                childWidth__4715 += (remainingWidth__4301 / 2L);
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((childWidth__4715 < 0L))
                    {
                        FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: global::Doroti.Generated.Framework.Foundation.FlutterError.Create("Insufficient horizontal space to render the " + "CupertinoDatePicker because the parent is too narrow at " + $"{size.width}px.\n" + $"An additional {-remainingWidth__4301}px is needed to avoid " + "overlapping columns.")));
                    }
                    return true;
                });
            layoutChild(index__4633, global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTight(new global::Doroti.Ui.Size(Math.Max(0.0, childWidth__4715), size.height)));
            positionChild(index__4633, new global::Doroti.Ui.Offset(currentHorizontalOffset__4377, 0.0));
            currentHorizontalOffset__4377 += childWidth__4715;
        }
    }

    public override bool shouldRelayout(global::Doroti.Generated.Framework.Rendering.MultiChildLayoutDelegate oldDelegate)
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

public class CupertinoDatePicker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
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
    public virtual global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoDatePicker(global::Doroti.Generated.Framework.Foundation.Key? key = null, CupertinoDatePickerMode mode = CupertinoDatePickerMode.dateAndTime, global::System.Action<DateTime> onDateTimeChanged = default!, DateTime? initialDateTime = null, DateTime? minimumDate = null, DateTime? maximumDate = null, long minimumYear = 1, long? maximumYear = null, long minuteInterval = 1, bool use24hFormat = false, DatePickerDateOrder? dateOrder = null, Color? backgroundColor = null, bool showDayOfWeek = false, bool showTimeSeparator = false, double? itemExtent = null, SelectionOverlayBuilder? selectionOverlayBuilder = null, global::System.Func<DateTime, bool>? selectableDayPredicate = null, global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate) : base(key: key)
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
        return ((IState)(object?)(this.mode switch { CupertinoDatePickerMode.time => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateTimeState__date_picker()), CupertinoDatePickerMode.dateAndTime => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateTimeState__date_picker()), CupertinoDatePickerMode.date => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerDateState__date_picker(dateOrder: this.dateOrder)), CupertinoDatePickerMode.monthYear => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>>(new _CupertinoDatePickerMonthYearState__date_picker(dateOrder: this.dateOrder)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _getColumnWidth(_PickerColumnType__date_picker columnType, CupertinoLocalizations localizations, global::Doroti.Generated.Framework.Widgets.BuildContext context, bool showDayOfWeek, bool standaloneMonth = false)
    {
        var longTexts__22718 = new List<string>();
        switch (columnType)
        {
            case _PickerColumnType__date_picker.date:
                {
                    for (var i__22821 = 1L; (i__22821 <= 12L); i__22821++)
                    {
                        string date__22867 = localizations.datePickerMediumDate(DartRuntimePrimitives.CreateDateTime(2018L, i__22821, 25L));
                        longTexts__22718.Add(date__22867);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.hour:
                {
                    for (var i__23026 = 0L; (i__23026 < 24L); i__23026++)
                    {
                        string hour__23071 = localizations.datePickerHour(i__23026);
                        longTexts__22718.Add(hour__23071);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.minute:
                {
                    for (var i__23206 = 0L; (i__23206 < 60L); i__23206++)
                    {
                        string minute__23251 = localizations.datePickerMinute(i__23206);
                        longTexts__22718.Add(minute__23251);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.dayPeriod:
                {
                    longTexts__22718.Add(localizations.anteMeridiemAbbreviation);
                    longTexts__22718.Add(localizations.postMeridiemAbbreviation);
                    break;
                }
            case _PickerColumnType__date_picker.dayOfMonth:
                {
                    var longestDayOfMonth__23557 = 1L;
                    for (var i__23597 = 1L; (i__23597 <= 31L); i__23597++)
                    {
                        string dayOfMonth__23643 = localizations.datePickerDayOfMonth(i__23597);
                        longTexts__22718.Add(dayOfMonth__23643);
                        longestDayOfMonth__23557 = i__23597;
                    }
                    if (showDayOfWeek)
                    {
                        for (var wd__23823 = 1L; (wd__23823 < 7L); wd__23823++)
                        {
                            string dayOfMonth__23891 = localizations.datePickerDayOfMonth(longestDayOfMonth__23557, wd__23823);
                            longTexts__22718.Add(dayOfMonth__23891);
                        }
                    }
                    break;
                }
            case _PickerColumnType__date_picker.month:
                {
                    for (var i__24077 = 1L; (i__24077 <= 12L); i__24077++)
                    {
                        string month__24123 = (standaloneMonth ? localizations.datePickerStandaloneMonth(i__24077) : localizations.datePickerMonth(i__24077));
                        longTexts__22718.Add(month__24123);
                    }
                    break;
                }
            case _PickerColumnType__date_picker.year:
                {
                    longTexts__22718.Add(localizations.datePickerYear(2018L));
                    break;
                }
            case _PickerColumnType__date_picker.timeSeparator:
                {
                    longTexts__22718.Add(":");
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => (System.Linq.Enumerable.Any(longTexts__22718) && longTexts__22718.All(((text) => (text.Length != 0)))), () => (object?)"column type is not appropriate");
        return CupertinoDatePicker.getColumnWidth(texts: longTexts__22718, context: context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static double getColumnWidth(List<string> texts, global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Painting.TextStyle? textStyle = null)
    {
        return texts.map<string, double>(((text) => TextPainter.computeMaxIntrinsicWidth(text: new global::Doroti.Generated.Framework.Painting.TextSpan(style: (textStyle ?? Date_pickerLibrary._themeTextStyle(context)), text: text), textDirection: Directionality.of(context)))).reduce(global::Doroti.Runtime.Dart_mathLibrary.max);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::Doroti.Generated.Framework.Widgets.Widget _ColumnBuilder__date_picker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay);

internal class _CupertinoDatePickerDateTimeState__date_picker : global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>
{
    internal const double _kMaximumOffAxisFraction = 0.45;
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual DateTime initialDateTime { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController dateController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController hourController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController minuteController { get; set; } = default!;
    public virtual long selectedAmPm { get; set; } = default!;
    public virtual long meridiemRegion { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController meridiemController { get; set; } = default!;
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
                        return (this.dateController.hasClients ? ((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.dateController).selectedItem : 0L);
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
    internal virtual long _selectedHourIndex => (this.hourController.hasClients ? (((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem % 24L) : this.initialDateTime.Hour);
    internal virtual long _selectedHour(long selectedAmPm, long selectedHour)
    {
        return (_isHourRegionFlipped(selectedAmPm) ? (((selectedHour + 12L)) % 24L) : selectedHour);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long selectedMinute
    {
        get
        {
            return (this.minuteController.hasClients ? ((((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.minuteController).selectedItem * ((CupertinoDatePicker)this.widget).minuteInterval) % 60L) : this.initialDateTime.Minute);
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
        meridiemController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedAmPm);
        hourController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: this.initialDateTime.Hour);
        minuteController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(this.initialDateTime.Minute / ((CupertinoDatePicker)this.widget).minuteInterval))));
        dateController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController();
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() => {
this.estimatedColumnWidths.Clear();
})));
    }

    public override void dispose()
    {
        this.dateController.dispose();
        this.hourController.dispose();
        this.minuteController.dispose();
        this.meridiemController.dispose();
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didUpdateWidget(CupertinoDatePicker oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((CupertinoDatePicker)oldWidget).mode, ((CupertinoDatePicker)this.widget).mode)), () => (object?)$"The {this.GetType()}'s mode cannot change once it's built.");
        if ((!((CupertinoDatePicker)this.widget).use24hFormat && ((CupertinoDatePicker)oldWidget).use24hFormat))
        {
            this.meridiemController.dispose();
            meridiemController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedAmPm);
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerLeft : global::Doroti.Generated.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerRight : global::Doroti.Generated.Framework.Painting.Alignment.centerLeft);
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
        bool isDateInvalid__32824 = (((((CupertinoDatePicker)this.widget).minimumDate?.isAfter(this.selectedDateTime) ?? false)) || ((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(this.selectedDateTime) ?? false)));
        if (isDateInvalid__32824)
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

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMediumDatePicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isDatePickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isDatePickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: CupertinoPicker.CreateBuilder(scrollController: this.dateController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
_onSelectedItemChange(index);
})), itemBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long, global::Doroti.Generated.Framework.Widgets.Widget?>)((context, index) => {
var rangeStart__34476 = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + index));
var rangeEnd__34663 = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, ((this.initialDateTime.Day + index) + 1L));
var now__34828 = new DateTime();
if ((((CupertinoDatePicker)this.widget).minimumDate?.isBefore(rangeEnd__34663) == false))
{
    return null;
}
if ((((CupertinoDatePicker)this.widget).maximumDate?.isAfter(rangeStart__34476) == false))
{
    return null;
}
string dateText__35080 = ((object.Equals(rangeStart__34476, DartRuntimePrimitives.CreateDateTime(now__34828.Year, now__34828.Month, now__34828.Day))) ? this.localizations.todayLabel : this.localizations.datePickerMediumDate(rangeStart__34476));
bool isDisabled__35271 = !_isSelectableDate(rangeStart__34476);
global::Doroti.Generated.Framework.Widgets.Widget child__35339 = itemPositioningBuilder(context, new global::Doroti.Generated.Framework.Widgets.Text(dateText__35080, style: Date_pickerLibrary._themeTextStyle(context, isValid: !isDisabled__35271)));
return (isDisabled__35271 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__35339) : child__35339);
throw new InvalidOperationException("Dart closure completed without a value.");
})), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isValidHour(long meridiemIndex, long hourIndex)
    {
        var rangeStart__35914 = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + this.selectedDayFromInitial), _selectedHour(meridiemIndex, hourIndex));
        DateTime rangeEnd__36197 = rangeStart__35914.add(Duration.Create(hours: 1L));
        return (((((CupertinoDatePicker)this.widget).minimumDate?.isBefore(rangeEnd__36197) ?? true)) && !((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(rangeStart__35914) ?? false)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildHourPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isHourPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isHourPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.hourController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
var regionChanged__37338 = (this.meridiemRegion != (checked((long)(index / 12L))));
bool debugIsFlipped__37406 = this.isHourRegionFlipped;
if (regionChanged__37338)
{
    meridiemRegion = (checked((long)(index / 12L)));
    selectedAmPm = (1L - this.selectedAmPm);
}
if ((!((CupertinoDatePicker)this.widget).use24hFormat && regionChanged__37338))
{
    DartRuntimePrimitives.Ignore(this.meridiemController.animateToItem(this.selectedAmPm, duration: Duration.Create(milliseconds: 300L), curve: global::Doroti.Generated.Framework.Animation.Curves.easeOut));
}
else
{
    _onSelectedItemChange(index);
}
DartRuntimePrimitives.Assert(() => (debugIsFlipped__37406 == this.isHourRegionFlipped));
})), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)24L)), ((index) => {
long hour__38346 = (this.isHourRegionFlipped ? (((index + 12L)) % 24L) : index);
long displayHour__38422 = (((CupertinoDatePicker)this.widget).use24hFormat ? hour__38346 : ((((hour__38346 + 11L)) % 12L) + 1L));
bool isDisabled__38508 = !_isValidHour(this.selectedAmPm, index);
global::Doroti.Generated.Framework.Widgets.Widget child__38581 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(this.localizations.datePickerHour(displayHour__38422), semanticsLabel: this.localizations.datePickerHourSemanticsLabel(displayHour__38422), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isDisabled__38508)));
return (isDisabled__38508 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__38581) : child__38581);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMinutePicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isMinutePickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isMinutePickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.minuteController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: (global::System.Action<long>)this._onSelectedItemChange, looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoDatePicker)this.widget).minuteInterval))))), ((index) => {
long minute__40124 = (index * ((CupertinoDatePicker)this.widget).minuteInterval);
var date__40181 = DartRuntimePrimitives.CreateDateTime(this.initialDateTime.Year, this.initialDateTime.Month, (this.initialDateTime.Day + this.selectedDayFromInitial), this.selectedHour, minute__40124);
bool isInvalidMinute__40406 = (((((CupertinoDatePicker)this.widget).minimumDate?.isAfter(date__40181) ?? false)) || ((((CupertinoDatePicker)this.widget).maximumDate?.isBefore(date__40181) ?? false)));
global::Doroti.Generated.Framework.Widgets.Widget child__40571 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(this.localizations.datePickerMinute(minute__40124), semanticsLabel: this.localizations.datePickerMinuteSemanticsLabel(minute__40124), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMinute__40406)));
return (isInvalidMinute__40406 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__40571) : child__40571);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildAmPmPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isMeridiemPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isMeridiemPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.meridiemController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedAmPm = index;
DartRuntimePrimitives.Assert(() => ((this.selectedAmPm == 0L) || (this.selectedAmPm == 1L)));
_onSelectedItemChange(index);
})), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)2L)), ((index) => {
bool isDisabled__42206 = !_isValidHour(index, this._selectedHourIndex);
global::Doroti.Generated.Framework.Widgets.Widget child__42284 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(((index == 0L) ? this.localizations.anteMeridiemAbbreviation : this.localizations.postMeridiemAbbreviation), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isDisabled__42206)));
return (isDisabled__42206 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__42284) : child__42284);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildTimeSeparatorWidget(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: new CupertinoPicker(offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
})), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)1L)), ((index) => {
return itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(":", style: Date_pickerLibrary._themeTextStyle(this.context)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _scrollToFirstSelectableDate()
    {
        if (!_isSelectableDate(this.selectedDateTime))
        {
            var daysThreshold__43635 = 1L;
            DateTime targetDate__43675 = this.selectedDateTime.add(Duration.Create(days: daysThreshold__43635));
            _scrollToDate(targetDate__43675, this.selectedDateTime, false, focusedIndex: (((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.dateController).selectedItem + daysThreshold__43635));
        }
    }

    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() => {
})));
        if (this.isScrolling)
        {
            return;
        }
        DateTime selectedDate__44273 = this.selectedDateTime;
        bool minCheck__44322 = (((CupertinoDatePicker)this.widget).minimumDate?.isAfter(selectedDate__44273) ?? false);
        bool maxCheck__44400 = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(selectedDate__44273) ?? false);
        _scrollToFirstSelectableDate();
        if ((minCheck__44322 || maxCheck__44400))
        {
            DateTime targetDate__44595 = (minCheck__44322 ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate));
            _scrollToDate(targetDate__44595, selectedDate__44273, minCheck__44322);
        }
    }

    internal virtual void _scrollToDate(DateTime newDate, DateTime fromDate, bool minCheck, long? focusedIndex = null)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
if ((((fromDate.Year != newDate.Year) || (fromDate.Month != newDate.Month)) || (fromDate.Day != newDate.Day)))
{
    Date_pickerLibrary._animateColumnControllerToItem(this.dateController, ((focusedIndex ?? (long)this.selectedDayFromInitial)));
}
if ((fromDate.Hour != newDate.Hour))
{
    bool needsMeridiemChange__45196 = (!((CupertinoDatePicker)this.widget).use24hFormat && ((checked((long)(fromDate.Hour / 12L))) != (checked((long)(newDate.Hour / 12L)))));
    if (needsMeridiemChange__45196)
    {
        Date_pickerLibrary._animateColumnControllerToItem(this.meridiemController, (1L - ((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.meridiemController).selectedItem));
        long newItem__45614 = ((((checked((long)(((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem / 12L)))) * 12L) + ((((((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem + newDate.Hour) - fromDate.Hour)) % 12L));
        Date_pickerLibrary._animateColumnControllerToItem(this.hourController, newItem__45614);
    }
    else
    {
        Date_pickerLibrary._animateColumnControllerToItem(this.hourController, ((((global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController)this.hourController).selectedItem + newDate.Hour) - fromDate.Hour));
    }
}
if ((fromDate.Minute != newDate.Minute))
{
    double positionDouble__46088 = (newDate.Minute / ((CupertinoDatePicker)this.widget).minuteInterval);
    long position__46163 = (minCheck ? positionDouble__46088.ceil() : positionDouble__46088.floor());
    Date_pickerLibrary._animateColumnControllerToItem(this.minuteController, position__46163);
}
})), debugLabel: "DatePicker.scrollToDate");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var columnWidths__46495 = new List<double> { _getEstimatedColumnWidth(_PickerColumnType__date_picker.hour), _getEstimatedColumnWidth(_PickerColumnType__date_picker.minute) };
        var pickerBuilders__46737 = ((List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>>)((object.Equals(Directionality.of(context), TextDirection.rtl)) ? new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildMinutePicker, this._buildHourPicker } : new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildHourPicker, this._buildMinutePicker }));
        if (((CupertinoDatePicker)this.widget).showTimeSeparator)
        {
            columnWidths__46495.Insert(checked((int)1L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.timeSeparator));
            pickerBuilders__46737.Insert(checked((int)1L), this._buildTimeSeparatorWidget);
        }
        if (!((CupertinoDatePicker)this.widget).use24hFormat)
        {
            switch (this.localizations.datePickerDateTimeOrder)
            {
                case var __constant47363 when (object.Equals(__constant47363, DatePickerDateTimeOrder.date_time_dayPeriod)):
                case var __constant47421 when (object.Equals(__constant47421, DatePickerDateTimeOrder.time_dayPeriod_date)):
                    {
                        pickerBuilders__46737.Add(this._buildAmPmPicker);
                        columnWidths__46495.Add(_getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod));
                        break;
                    }
                case var __constant47610 when (object.Equals(__constant47610, DatePickerDateTimeOrder.date_dayPeriod_time)):
                case var __constant47668 when (object.Equals(__constant47668, DatePickerDateTimeOrder.dayPeriod_time_date)):
                    {
                        pickerBuilders__46737.Insert(checked((int)0L), this._buildAmPmPicker);
                        columnWidths__46495.Insert(checked((int)0L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.dayPeriod));
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
                        pickerBuilders__46737.Add(this._buildMediumDatePicker);
                        columnWidths__46495.Add(_getEstimatedColumnWidth(_PickerColumnType__date_picker.date));
                        break;
                    }
                case var __constant48319 when (object.Equals(__constant48319, DatePickerDateTimeOrder.date_time_dayPeriod)):
                case var __constant48377 when (object.Equals(__constant48377, DatePickerDateTimeOrder.date_dayPeriod_time)):
                    {
                        pickerBuilders__46737.Insert(checked((int)0L), this._buildMediumDatePicker);
                        columnWidths__46495.Insert(checked((int)0L), _getEstimatedColumnWidth(_PickerColumnType__date_picker.date));
                        break;
                    }
            }
        }
        var pickers__48591 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        double totalColumnWidths__48624 = (4L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i__48690, width__48700) in columnWidths__46495.indexed())
        {
            var (firstColumn__48752, lastColumn__48770) = ((i__48690 == 0L), (i__48690 == (checked((long)(columnWidths__46495.Count)) - 1L)));
            var offAxisFraction__48834 = 0.0;
            global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay__48871 = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay__48871 = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i__48690, columnCount: checked((long)(columnWidths__46495.Count)));
            }
            else
            {
                if (firstColumn__48752)
                {
                    selectionOverlay__48871 = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn__48770)
                    {
                        selectionOverlay__48871 = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            if (firstColumn__48752)
            {
                offAxisFraction__48834 = (-_kMaximumOffAxisFraction * this.textDirectionFactor);
            }
            else
            {
                if (((i__48690 >= 2L) || (checked((long)(columnWidths__46495.Count)) == 2L)))
                {
                    offAxisFraction__48834 = (_kMaximumOffAxisFraction * this.textDirectionFactor);
                }
            }
            var padding__49576 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if (lastColumn__48770)
            {
                padding__49576 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__49576).flipped;
            }
            if ((this.textDirectionFactor == -1L))
            {
                padding__49576 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__49576).flipped;
            }
            totalColumnWidths__48624 += (width__48700 + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            pickers__48591.Add(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: i__48690, child: pickerBuilders__46737[(int)(i__48690)](offAxisFraction__48834, ((context, child) => {
global::Doroti.Generated.Framework.Widgets.Widget constrained__50026 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxWidth: (width__48700 + Date_pickerLibrary._kDatePickerPadSize)), child: child));
return new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__49576, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: (lastColumn__48770 ? this.alignCenterLeft : this.alignCenterRight), child: ((firstColumn__48752 || lastColumn__48770) ? constrained__50026 : child)));
throw new InvalidOperationException("Dart closure completed without a value.");
}), selectionOverlay__48871)));
        }
        double maxPickerWidth__50526 = ((totalColumnWidths__48624 > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths__48624 : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Generated.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidths__46495, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth__50526), children: pickers__48591))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDatePickerDateState__date_picker : global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedDay { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController dayController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController monthController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController yearController { get; set; } = default!;
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
        dayController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedDay - 1L));
        monthController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedMonth - 1L));
        yearController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedYear);
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() => {
_refreshEstimatedColumnWidths();
})));
    }

    public override void dispose()
    {
        this.dayController.dispose();
        this.monthController.dispose();
        this.yearController.dispose();
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerLeft : global::Doroti.Generated.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerRight : global::Doroti.Generated.Framework.Painting.Alignment.centerLeft);
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.dayOfMonth, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.month, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.year, this.localizations, this.context, ((CupertinoDatePicker)this.widget).showDayOfWeek);
    }

    internal virtual DateTime _lastDayInMonth(long year, long month) => DartRuntimePrimitives.CreateDateTime(year, (month + 1L), 0L);
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildDayPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        long daysInCurrentMonth__54703 = _lastDayInMonth(this.selectedYear, this.selectedMonth).Day;
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isDayPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isDayPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.dayController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedDay = (index + 1L);
if (this._isCurrentDateValid)
{
    this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
}
})), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)31L)), ((index) => {
long day__55905 = (index + 1L);
long? dayOfWeek__55943 = (((CupertinoDatePicker)this.widget).showDayOfWeek ? DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, day__55905).DayOfWeek.ToDartWeekday() : null);
bool isInvalidDay__56086 = ((((day__55905 > daysInCurrentMonth__54703)) || ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month == this.selectedMonth)) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Day > day__55905)))) || ((((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month == this.selectedMonth)) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Day < day__55905))));
global::Doroti.Generated.Framework.Widgets.Widget child__56516 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(this.localizations.datePickerDayOfMonth(day__55905, dayOfWeek__55943), style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidDay__56086)));
return (isInvalidDay__56086 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__56516) : child__56516);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMonthPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isMonthPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isMonthPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.monthController, offAxisFraction: offAxisFraction, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedMonth = (index + 1L);
if (this._isCurrentDateValid)
{
    this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
}
})), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)12L)), ((index) => {
long month__58134 = (index + 1L);
bool isInvalidMonth__58174 = ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month > month__58134))) || (((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month < month__58134))));
string monthName__58406 = (((object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear))) ? this.localizations.datePickerStandaloneMonth(month__58134) : this.localizations.datePickerMonth(month__58134));
global::Doroti.Generated.Framework.Widgets.Widget child__58610 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(monthName__58406, style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMonth__58174)));
return (isInvalidMonth__58174 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__58610) : child__58610);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildYearPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isYearPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isYearPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: CupertinoPicker.CreateBuilder(scrollController: this.yearController, itemExtent: ((CupertinoDatePicker)this.widget).itemExtent, offAxisFraction: offAxisFraction, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedYear = index;
if (this._isCurrentDateValid)
{
    this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay));
}
})), itemBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long, global::Doroti.Generated.Framework.Widgets.Widget?>)((context, year) => {
if ((year < ((CupertinoDatePicker)this.widget).minimumYear))
{
    return null;
}
if (((((CupertinoDatePicker)this.widget).maximumYear is not null) && (year > DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumYear))))
{
    return null;
}
bool isValidYear__60272 = ((((((CupertinoDatePicker)this.widget).minimumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Year <= year))) && (((((CupertinoDatePicker)this.widget).maximumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Year >= year))));
global::Doroti.Generated.Framework.Widgets.Widget child__60472 = itemPositioningBuilder(context, new global::Doroti.Generated.Framework.Widgets.Text(this.localizations.datePickerYear(year), style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear__60272)));
return (isValidYear__60272 ? child__60472 : new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__60472));
throw new InvalidOperationException("Dart closure completed without a value.");
})), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate__60967 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay);
            var maxSelectedDate__61047 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (this.selectedDay + 1L));
            bool minCheck__61137 = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectedDate__61047) ?? true);
            bool maxCheck__61218 = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectedDate__60967) ?? false);
            return ((minCheck__61137 && !maxCheck__61218) && (minSelectedDate__60967.Day == this.selectedDay));
            return default!;
        }
    }
    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() => {
})));
        if (this.isScrolling)
        {
            return;
        }
        var minSelectDate__61764 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, this.selectedDay);
        var maxSelectDate__61842 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (this.selectedDay + 1L));
        bool minCheck__61930 = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectDate__61842) ?? true);
        bool maxCheck__62009 = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectDate__61764) ?? false);
        if ((!minCheck__61930 || maxCheck__62009))
        {
            DateTime targetDate__62170 = (minCheck__61930 ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate));
            _scrollToDate(targetDate__62170);
            return;
        }
        if ((minSelectDate__61764.Day != this.selectedDay))
        {
            DateTime lastDay__62487 = _lastDayInMonth(this.selectedYear, this.selectedMonth);
            _scrollToDate(lastDay__62487);
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var pickerBuilders__63180 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>>();
        var columnWidths__63225 = new List<double>();
        DatePickerDateOrder datePickerDateOrder__63283 = (this.dateOrder ?? this.localizations.datePickerDateOrder);
        switch (datePickerDateOrder__63283)
        {
            case var __constant63400 when (object.Equals(__constant63400, DatePickerDateOrder.mdy)):
                {
                    pickerBuilders__63180 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildMonthPicker, this._buildDayPicker, this._buildYearPicker };
                    columnWidths__63225 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant63776 when (object.Equals(__constant63776, DatePickerDateOrder.dmy)):
                {
                    pickerBuilders__63180 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildDayPicker, this._buildMonthPicker, this._buildYearPicker };
                    columnWidths__63225 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant64152 when (object.Equals(__constant64152, DatePickerDateOrder.ymd)):
                {
                    pickerBuilders__63180 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildMonthPicker, this._buildDayPicker };
                    columnWidths__63225 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))) };
                    break;
                }
            case var __constant64528 when (object.Equals(__constant64528, DatePickerDateOrder.ydm)):
                {
                    pickerBuilders__63180 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildDayPicker, this._buildMonthPicker };
                    columnWidths__63225 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.dayOfMonth))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))) };
                    break;
                }
        }
        var pickers__64910 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        double totalColumnWidths__64943 = (4L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i__65009, width__65019) in columnWidths__63225.indexed())
        {
            var (firstColumn__65071, lastColumn__65089) = ((i__65009 == 0L), (i__65009 == (checked((long)(columnWidths__63225.Count)) - 1L)));
            double offAxisFraction__65162 = ((((i__65009 - 1L)) * 0.3) * this.textDirectionFactor);
            var padding__65228 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
            if ((this.textDirectionFactor == -1L))
            {
                padding__65228 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(left: Date_pickerLibrary._kDatePickerPadSize);
            }
            global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay__65419 = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay__65419 = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i__65009, columnCount: checked((long)(columnWidths__63225.Count)));
            }
            else
            {
                if (firstColumn__65071)
                {
                    selectionOverlay__65419 = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn__65089)
                    {
                        selectionOverlay__65419 = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            totalColumnWidths__64943 += (width__65019 + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            pickers__64910.Add(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: i__65009, child: pickerBuilders__63180[(int)(i__65009)](offAxisFraction__65162, ((context, child) => {
return new global::Doroti.Generated.Framework.Widgets.Padding(padding: (firstColumn__65071 ? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero : padding__65228), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: (lastColumn__65089 ? this.alignCenterLeft : this.alignCenterRight), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (width__65019 + Date_pickerLibrary._kDatePickerPadSize), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: (firstColumn__65071 ? this.alignCenterLeft : this.alignCenterRight), child: child))));
throw new InvalidOperationException("Dart closure completed without a value.");
}), selectionOverlay__65419)));
        }
        double maxPickerWidth__66663 = ((totalColumnWidths__64943 > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths__64943 : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Generated.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidths__63225, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth__66663), children: pickers__64910))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoDatePickerMonthYearState__date_picker : global::Doroti.Generated.Framework.Widgets.State<CupertinoDatePicker>
{
    public virtual DatePickerDateOrder? dateOrder { get; private set; }
    public virtual long textDirectionFactor { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterLeft { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignCenterRight { get; set; } = default!;
    public virtual long selectedYear { get; set; } = default!;
    public virtual long selectedMonth { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController monthController { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController yearController { get; set; } = default!;
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
        monthController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (this.selectedMonth - 1L));
        yearController = new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: this.selectedYear);
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() => {
_refreshEstimatedColumnWidths();
})));
    }

    public override void dispose()
    {
        this.monthController.dispose();
        this.yearController.dispose();
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
        base.dispose();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        textDirectionFactor = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? 1L : -1L);
        localizations = CupertinoLocalizations.of(this.context);
        alignCenterLeft = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerLeft : global::Doroti.Generated.Framework.Painting.Alignment.centerRight);
        alignCenterRight = ((this.textDirectionFactor == 1L) ? global::Doroti.Generated.Framework.Painting.Alignment.centerRight : global::Doroti.Generated.Framework.Painting.Alignment.centerLeft);
        _refreshEstimatedColumnWidths();
    }

    internal virtual void _refreshEstimatedColumnWidths()
    {
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.month, this.localizations, this.context, false, standaloneMonth: (object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear)));
        this.estimatedColumnWidths[FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year)] = CupertinoDatePicker._getColumnWidth(_PickerColumnType__date_picker.year, this.localizations, this.context, false);
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMonthPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isMonthPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isMonthPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new CupertinoPicker(scrollController: this.monthController, offAxisFraction: offAxisFraction, itemExtent: Date_pickerLibrary._kItemExtent, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedMonth = (index + 1L);
if (this._isCurrentDateValid)
{
    this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth));
}
})), looping: true, selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)12L)), ((index) => {
long month__71305 = (index + 1L);
bool isInvalidMonth__71345 = ((((((CupertinoDatePicker)this.widget).minimumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Month > month__71305))) || (((((CupertinoDatePicker)this.widget).maximumDate?.Year == this.selectedYear) && (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Month < month__71305))));
string monthName__71577 = (((object.Equals(((CupertinoDatePicker)this.widget).mode, CupertinoDatePickerMode.monthYear))) ? this.localizations.datePickerStandaloneMonth(month__71305) : this.localizations.datePickerMonth(month__71305));
global::Doroti.Generated.Framework.Widgets.Widget child__71781 = itemPositioningBuilder(this.context, new global::Doroti.Generated.Framework.Widgets.Text(monthName__71577, style: Date_pickerLibrary._themeTextStyle(this.context, isValid: !isInvalidMonth__71345)));
return (isInvalidMonth__71345 ? new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__71781) : child__71781);
throw new InvalidOperationException("Dart closure completed without a value.");
}))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildYearPicker(double offAxisFraction, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget> itemPositioningBuilder, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)((notification) => {
if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification))
{
    isYearPickerScrolling = true;
}
else
{
    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
    {
        isYearPickerScrolling = false;
        _pickerDidStopScrolling();
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: CupertinoPicker.CreateBuilder(scrollController: this.yearController, itemExtent: Date_pickerLibrary._kItemExtent, offAxisFraction: offAxisFraction, useMagnifier: Date_pickerLibrary._kUseMagnifier, magnification: Date_pickerLibrary._kMagnification, backgroundColor: ((CupertinoDatePicker)this.widget).backgroundColor, changeReportingBehavior: ((CupertinoDatePicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
selectedYear = index;
if (this._isCurrentDateValid)
{
    this.widget.onDateTimeChanged(DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth));
}
})), itemBuilder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, long, global::Doroti.Generated.Framework.Widgets.Widget?>)((context, year) => {
if ((year < ((CupertinoDatePicker)this.widget).minimumYear))
{
    return null;
}
if (((((CupertinoDatePicker)this.widget).maximumYear is not null) && (year > DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumYear))))
{
    return null;
}
bool isValidYear__73397 = ((((((CupertinoDatePicker)this.widget).minimumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate).Year <= year))) && (((((CupertinoDatePicker)this.widget).maximumDate is null) || (DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate).Year >= year))));
global::Doroti.Generated.Framework.Widgets.Widget child__73597 = itemPositioningBuilder(context, new global::Doroti.Generated.Framework.Widgets.Text(this.localizations.datePickerYear(year), style: Date_pickerLibrary._themeTextStyle(context, isValid: isValidYear__73397)));
return (isValidYear__73397 ? child__73597 : new global::Doroti.Generated.Framework.Widgets.ExcludeSemantics(child: child__73597));
throw new InvalidOperationException("Dart closure completed without a value.");
})), selectionOverlay: selectionOverlay)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isCurrentDateValid
    {
        get
        {
            var minSelectedDate__74092 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth);
            var maxSelectedDate__74159 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (((CupertinoDatePicker)this.widget).initialDateTime.Day + 1L));
            bool minCheck__74264 = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectedDate__74159) ?? true);
            bool maxCheck__74345 = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectedDate__74092) ?? false);
            return (minCheck__74264 && !maxCheck__74345);
            return default!;
        }
    }
    internal virtual void _pickerDidStopScrolling()
    {
        setState(((global::System.Action)(() => {
})));
        if (this.isScrolling)
        {
            return;
        }
        var minSelectDate__74853 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth);
        var maxSelectDate__74918 = DartRuntimePrimitives.CreateDateTime(this.selectedYear, this.selectedMonth, (((CupertinoDatePicker)this.widget).initialDateTime.Day + 1L));
        bool minCheck__75021 = (((CupertinoDatePicker)this.widget).minimumDate?.isBefore(maxSelectDate__74918) ?? true);
        bool maxCheck__75100 = (((CupertinoDatePicker)this.widget).maximumDate?.isBefore(minSelectDate__74853) ?? false);
        if ((!minCheck__75021 || maxCheck__75100))
        {
            DateTime targetDate__75261 = (minCheck__75021 ? DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).maximumDate) : DartRuntimePrimitives.RequireValue(((CupertinoDatePicker)this.widget).minimumDate));
            _scrollToDate(targetDate__75261);
            return;
        }
    }

    internal virtual void _scrollToDate(DateTime newDate)
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var pickerBuilders__75861 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>>();
        var columnWidths__75906 = new List<double>();
        DatePickerDateOrder datePickerDateOrder__75964 = (this.dateOrder ?? this.localizations.datePickerDateOrder);
        switch (datePickerDateOrder__75964)
        {
            case var __constant76081 when (object.Equals(__constant76081, DatePickerDateOrder.mdy)):
            case var __constant76117 when (object.Equals(__constant76117, DatePickerDateOrder.dmy)):
                {
                    pickerBuilders__75861 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildMonthPicker, this._buildYearPicker };
                    columnWidths__75906 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))) };
                    break;
                }
            case var __constant76406 when (object.Equals(__constant76406, DatePickerDateOrder.ymd)):
            case var __constant76442 when (object.Equals(__constant76442, DatePickerDateOrder.ydm)):
                {
                    pickerBuilders__75861 = new List<global::System.Func<double, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>> { this._buildYearPicker, this._buildMonthPicker };
                    columnWidths__75906 = new List<double> { DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.year))), DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<double>(this.estimatedColumnWidths, FoundationRuntimePorts.EnumIndex(_PickerColumnType__date_picker.month))) };
                    break;
                }
        }
        var pickers__76737 = new List<global::Doroti.Generated.Framework.Widgets.Widget>();
        double totalColumnWidths__76770 = (3L * Date_pickerLibrary._kDatePickerPadSize);
        foreach (var (i__76836, width__76846) in columnWidths__75906.indexed())
        {
            var (firstColumn__76898, lastColumn__76916) = ((i__76836 == 0L), (i__76836 == (checked((long)(columnWidths__75906.Count)) - 1L)));
            double offAxisFraction__76989 = (this.textDirectionFactor * ((firstColumn__76898 ? -0.3 : 0.5)));
            totalColumnWidths__76770 += (width__76846 + ((2L * Date_pickerLibrary._kDatePickerPadSize)));
            global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay__77135 = Date_pickerLibrary._centerSelectionOverlay;
            if ((((CupertinoDatePicker)this.widget).selectionOverlayBuilder is not null))
            {
                selectionOverlay__77135 = ((CupertinoDatePicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: i__76836, columnCount: checked((long)(columnWidths__75906.Count)));
            }
            else
            {
                if (firstColumn__76898)
                {
                    selectionOverlay__77135 = Date_pickerLibrary._startSelectionOverlay;
                }
                else
                {
                    if (lastColumn__76916)
                    {
                        selectionOverlay__77135 = Date_pickerLibrary._endSelectionOverlay;
                    }
                }
            }
            pickers__76737.Add(new global::Doroti.Generated.Framework.Widgets.LayoutId(id: i__76836, child: pickerBuilders__75861[(int)(i__76836)](offAxisFraction__76989, ((context, child) => {
global::Doroti.Generated.Framework.Widgets.Widget contents__77763 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: (lastColumn__76916 ? this.alignCenterLeft : this.alignCenterRight), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (width__76846 + Date_pickerLibrary._kDatePickerPadSize), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: (firstColumn__76898 ? this.alignCenterLeft : this.alignCenterRight), child: child))));
if (firstColumn__76898)
{
    return contents__77763;
}
var padding__78225 = global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(right: Date_pickerLibrary._kDatePickerPadSize);
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: ((this.textDirectionFactor == -1L) ? ((global::Doroti.Generated.Framework.Painting.EdgeInsets)padding__78225).flipped : padding__78225), child: contents__77763));
throw new InvalidOperationException("Dart closure completed without a value.");
}), selectionOverlay__77135)));
        }
        double maxPickerWidth__78508 = ((totalColumnWidths__76770 > Date_pickerLibrary._kPickerWidth) ? totalColumnWidths__76770 : Date_pickerLibrary._kPickerWidth);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: DefaultTextStyle.merge(style: Date_pickerLibrary._kDefaultPickerTextStyle, child: new global::Doroti.Generated.Framework.Widgets.CustomMultiChildLayout(@delegate: new _DatePickerLayoutDelegate__date_picker(columnWidths: columnWidths__75906, textDirectionFactor: this.textDirectionFactor, maxWidth: maxPickerWidth__78508), children: pickers__76737))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum CupertinoTimerPickerMode
{
    hm,
    ms,
    hms
}

public class CupertinoTimerPicker : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual CupertinoTimerPickerMode mode { get; private set; } = default!;
    public virtual Duration initialTimerDuration { get; private set; } = default!;
    public virtual long minuteInterval { get; private set; } = default!;
    public virtual long secondInterval { get; private set; } = default!;
    public virtual global::System.Action<Duration> onTimerDurationChanged { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual double itemExtent { get; private set; } = default!;
    public virtual SelectionOverlayBuilder? selectionOverlayBuilder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public CupertinoTimerPicker(global::Doroti.Generated.Framework.Foundation.Key? key = null, CupertinoTimerPickerMode mode = CupertinoTimerPickerMode.hms, Duration initialTimerDuration = default, long minuteInterval = 1, long secondInterval = 1, global::Doroti.Generated.Framework.Painting.AlignmentGeometry alignment = default!, Color? backgroundColor = null, double? itemExtent = null, global::System.Action<Duration> onTimerDurationChanged = default!, global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior changeReportingBehavior = global::Doroti.Generated.Framework.Widgets.ChangeReportingBehavior.onScrollUpdate, SelectionOverlayBuilder? selectionOverlayBuilder = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.AlignmentGeometry __alignment = alignment ?? global::Doroti.Generated.Framework.Painting.Alignment.center;
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

internal class _CupertinoTimerPickerState__date_picker : global::Doroti.Generated.Framework.Widgets.State<CupertinoTimerPicker>
{
    public virtual TextDirection textDirection { get; set; } = default!;
    public virtual CupertinoLocalizations localizations { get; set; } = default!;
    public virtual long? selectedHour { get; set; } = default;
    public virtual long selectedMinute { get; set; } = default!;
    public virtual long? selectedSecond { get; set; } = default;
    public virtual long? lastSelectedHour { get; set; } = default;
    public virtual long? lastSelectedMinute { get; set; } = default;
    public virtual long? lastSelectedSecond { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Painting.TextPainter textPainter { get; private set; } = new global::Doroti.Generated.Framework.Painting.TextPainter();
    public virtual List<string> numbers { get; private set; } = new List<string>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)10L)), ((i) => $"{(9L - i)}")));
    public virtual double numberLabelWidth { get; set; } = default!;
    public virtual double numberLabelHeight { get; set; } = default!;
    public virtual double numberLabelBaseline { get; set; } = default!;
    public virtual double hourLabelWidth { get; set; } = default!;
    public virtual double minuteLabelWidth { get; set; } = default!;
    public virtual double secondLabelWidth { get; set; } = default!;
    public virtual double totalWidth { get; set; } = default!;
    public virtual double pickerColumnWidth { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController? _hourScrollController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController? _minuteScrollController { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController? _secondScrollController { get; set; } = default;

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
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.addListener(() => this._handleSystemFontsChange());
    }

    internal virtual void _handleSystemFontsChange()
    {
        setState(((global::System.Action)(() => {
this.textPainter.markNeedsLayout();
_measureLabelMetrics();
})));
    }

    public override void dispose()
    {
        global::Doroti.Generated.Framework.Painting.PaintingBinding.instance.systemFonts.removeListener(() => this._handleSystemFontsChange());
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
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__88677 = ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)_textStyleFrom(this.context, Date_pickerLibrary._kTimerPickerMagnification));
        double maxWidth__88754 = double.NegativeInfinity;
        string? widestNumber__88802 = default!;
        foreach (string input__89244 in this.numbers)
        {
            this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.InlineSpan>(new global::Doroti.Generated.Framework.Painting.TextSpan(text: input__89244, style: textStyle__88677));
            this.textPainter.layout();
            if ((((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth > maxWidth__88754))
            {
                maxWidth__88754 = ((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
                widestNumber__88802 = input__89244;
            }
        }
        this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.InlineSpan>(new global::Doroti.Generated.Framework.Painting.TextSpan(text: $"{widestNumber__88802}{widestNumber__88802}", style: textStyle__88677));
        this.textPainter.layout();
        numberLabelWidth = ((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
        numberLabelHeight = ((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).height;
        numberLabelBaseline = this.textPainter.computeDistanceToActualBaseline(TextBaseline.alphabetic);
        minuteLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerMinuteLabels.Cast<string?>().ToList(), textStyle__88677);
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)))
        {
            hourLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerHourLabels.Cast<string?>().ToList(), textStyle__88677);
        }
        if ((!object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hm)))
        {
            secondLabelWidth = _measureLabelsMaxWidth(this.localizations.timerPickerSecondLabels.Cast<string?>().ToList(), textStyle__88677);
        }
    }

    internal virtual double _measureLabelsMaxWidth(List<string?> labels, global::Doroti.Generated.Framework.Painting.TextStyle style)
    {
        double maxWidth__90389 = double.NegativeInfinity;
        for (var i__90438 = 0L; (i__90438 < checked((long)(labels.Count))); i__90438++)
        {
            string? label__90491 = labels[(int)(i__90438)];
            if ((label__90491 is null))
            {
                continue;
            }
            this.textPainter.text = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.InlineSpan>(new global::Doroti.Generated.Framework.Painting.TextSpan(text: label__90491, style: style));
            this.textPainter.layout();
            DartRuntimePrimitives.Ignore(((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth);
            if ((((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth > maxWidth__90389))
            {
                maxWidth__90389 = ((global::Doroti.Generated.Framework.Painting.TextPainter)this.textPainter).maxIntrinsicWidth;
            }
        }
        return maxWidth__90389;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildLabel(string text, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional pickerPadding)
    {
        var padding__91206 = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.numberLabelWidth + Date_pickerLibrary._kTimerPickerLabelPadSize) + ((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)pickerPadding).start));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.IgnorePointer(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding__91206.resolve(this.textDirection), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart.resolve(this.textDirection), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: this.numberLabelHeight, child: new global::Doroti.Generated.Framework.Widgets.Baseline(baseline: this.numberLabelBaseline, baselineType: TextBaseline.alphabetic, child: new global::Doroti.Generated.Framework.Widgets.Text(text, style: new global::Doroti.Generated.Framework.Painting.TextStyle(fontSize: Date_pickerLibrary._kTimerPickerLabelFontSize, fontWeight: FontWeight.w600), maxLines: 1L, softWrap: false)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildPickerNumberLabel(string text, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional padding)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (Date_pickerLibrary._kTimerPickerColumnIntrinsicWidth + padding.horizontal), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: padding.resolve(this.textDirection), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart.resolve(this.textDirection), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(width: this.numberLabelWidth, child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerEnd.resolve(this.textDirection), child: new global::Doroti.Generated.Framework.Widgets.Text(text, softWrap: false, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.visible)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildHourPicker(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        _hourScrollController ??= new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: DartRuntimePrimitives.RequireValue(this.selectedHour));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._hourScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0L), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
setState(((global::System.Action)(() => {
selectedHour = index;
this.widget.onTimerDurationChanged(Duration.Create(hours: DartRuntimePrimitives.RequireValue(this.selectedHour), minutes: this.selectedMinute, seconds: (this.selectedSecond ?? 0L)));
})));
})), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)24L)), ((index) => {
string label__93774 = (this.localizations.timerPickerHourLabel(index) ?? "");
string semanticsLabel__93852 = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerHour(index) + label__93774) : (label__93774 + this.localizations.timerPickerHour(index)));
return new global::Doroti.Generated.Framework.Widgets.Semantics(label: semanticsLabel__93852, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerHour(index), additionalPadding));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildHourColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedHour = this.selectedHour;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildHourPicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerHourLabel((this.lastSelectedHour ?? DartRuntimePrimitives.RequireValue(this.selectedHour))) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMinutePicker(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        _minuteScrollController ??= new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(this.selectedMinute / ((CupertinoTimerPicker)this.widget).minuteInterval))));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._minuteScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, ((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)) ? 0L : 1L)), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, looping: true, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
setState(((global::System.Action)(() => {
selectedMinute = (index * ((CupertinoTimerPicker)this.widget).minuteInterval);
this.widget.onTimerDurationChanged(Duration.Create(hours: (this.selectedHour ?? 0L), minutes: this.selectedMinute, seconds: (this.selectedSecond ?? 0L)));
})));
})), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoTimerPicker)this.widget).minuteInterval))))), ((index) => {
long minute__96219 = (index * ((CupertinoTimerPicker)this.widget).minuteInterval);
string label__96280 = (this.localizations.timerPickerMinuteLabel(minute__96219) ?? "");
string semanticsLabel__96361 = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerMinute(minute__96219) + label__96280) : (label__96280 + this.localizations.timerPickerMinute(minute__96219)));
return new global::Doroti.Generated.Framework.Widgets.Semantics(label: semanticsLabel__96361, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerMinute(minute__96219), additionalPadding));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildMinuteColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedMinute = this.selectedMinute;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildMinutePicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerMinuteLabel((this.lastSelectedMinute ?? this.selectedMinute)) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildSecondPicker(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        _secondScrollController ??= new global::Doroti.Generated.Framework.Widgets.FixedExtentScrollController(initialItem: (checked((long)(DartRuntimePrimitives.RequireValue(this.selectedSecond) / ((CupertinoTimerPicker)this.widget).secondInterval))));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoPicker(scrollController: this._secondScrollController, magnification: Date_pickerLibrary._kMagnification, offAxisFraction: _calculateOffAxisFraction(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, ((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.ms)) ? 1L : 2L)), itemExtent: ((CupertinoTimerPicker)this.widget).itemExtent, backgroundColor: ((CupertinoTimerPicker)this.widget).backgroundColor, squeeze: Date_pickerLibrary._kSqueeze, looping: true, changeReportingBehavior: ((CupertinoTimerPicker)this.widget).changeReportingBehavior, onSelectedItemChanged: ((global::System.Action<long>)((index) => {
setState(((global::System.Action)(() => {
selectedSecond = (index * ((CupertinoTimerPicker)this.widget).secondInterval);
this.widget.onTimerDurationChanged(Duration.Create(hours: (this.selectedHour ?? 0L), minutes: this.selectedMinute, seconds: DartRuntimePrimitives.RequireValue(this.selectedSecond)));
})));
})), selectionOverlay: selectionOverlay, children: new List<global::Doroti.Generated.Framework.Widgets.Widget>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)(checked((long)(60L / ((CupertinoTimerPicker)this.widget).secondInterval))))), ((index) => {
long second__98727 = (index * ((CupertinoTimerPicker)this.widget).secondInterval);
string label__98788 = (this.localizations.timerPickerSecondLabel(second__98727) ?? "");
string semanticsLabel__98869 = ((this.textDirectionFactor == 1L) ? (this.localizations.timerPickerSecond(second__98727) + label__98788) : (label__98788 + this.localizations.timerPickerSecond(second__98727)));
return new global::Doroti.Generated.Framework.Widgets.Semantics(label: semanticsLabel__98869, excludeSemantics: true, child: _buildPickerNumberLabel(this.localizations.timerPickerSecond(second__98727), additionalPadding));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildSecondColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional additionalPadding, global::Doroti.Generated.Framework.Widgets.Widget? selectionOverlay)
    {
        additionalPadding = global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).start, 0), end: Math.Max(((global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional)additionalPadding).end, 0));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification>(onNotification: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollEndNotification, bool>)((notification) => {
setState(((global::System.Action)(() => {
lastSelectedSecond = this.selectedSecond;
})));
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _buildSecondPicker(additionalPadding, selectionOverlay))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildLabel((this.localizations.timerPickerSecondLabel((this.lastSelectedSecond ?? DartRuntimePrimitives.RequireValue(this.selectedSecond))) ?? ""), additionalPadding)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle _textStyleFrom(global::Doroti.Generated.Framework.Widgets.BuildContext context, double magnification = 1.0)
    {
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__100318 = CupertinoTheme.of(context).textTheme.pickerTextStyle;
        return ((global::Doroti.Generated.Framework.Painting.TextStyle)(object?)textStyle__100318.copyWith(color: CupertinoDynamicColor.maybeResolve(((global::Doroti.Generated.Framework.Painting.TextStyle)textStyle__100318).color, context), fontSize: (DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Painting.TextStyle)textStyle__100318).fontSize) * magnification)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _calculateOffAxisFraction(double paddingStart, long position)
    {
        double centerPoint__100761 = (paddingStart + ((this.numberLabelWidth / 2L)));
        double pickerColumnOffAxisFraction__100914 = (0.5 - (centerPoint__100761 / this.pickerColumnWidth));
        double timerPickerOffAxisFraction__101078 = (0.5 - (((centerPoint__100761 + (this.pickerColumnWidth * position))) / this.totalWidth));
        return (((pickerColumnOffAxisFraction__100914 - timerPickerOffAxisFraction__101078)) * this.textDirectionFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
List<global::Doroti.Generated.Framework.Widgets.Widget> columns__101634 = default!;
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
if ((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth < this.totalWidth))
{
    totalWidth = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth;
    pickerColumnWidth = (this.totalWidth / (((object.Equals(((CupertinoTimerPicker)this.widget).mode, CupertinoTimerPickerMode.hms)) ? 3L : 2L)));
}
double baseLabelContentWidth__102326 = (this.numberLabelWidth + Date_pickerLibrary._kTimerPickerLabelPadSize);
double minuteLabelContentWidth__102417 = (baseLabelContentWidth__102326 + this.minuteLabelWidth);
switch (((CupertinoTimerPicker)this.widget).mode)
{
    case CupertinoTimerPickerMode.hm:
        {
            double hourLabelContentWidth__102655 = (baseLabelContentWidth__102326 + this.hourLabelWidth);
            double hourColumnStartPadding__102738 = ((this.pickerColumnWidth - hourLabelContentWidth__102655) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
            if ((hourColumnStartPadding__102738 < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
            {
                hourColumnStartPadding__102738 = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
            }
            double minuteColumnEndPadding__103041 = ((this.pickerColumnWidth - minuteLabelContentWidth__102417) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
            if ((minuteColumnEndPadding__103041 < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
            {
                minuteColumnEndPadding__103041 = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
            }
            global::Doroti.Generated.Framework.Widgets.Widget? hourSelectionOverlay__103347 = Date_pickerLibrary._startSelectionOverlay;
            global::Doroti.Generated.Framework.Widgets.Widget? minuteSelectionOverlay__103414 = Date_pickerLibrary._endSelectionOverlay;
            if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
            {
                hourSelectionOverlay__103347 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 2L);
                minuteSelectionOverlay__103414 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 2L);
            }
            columns__101634 = new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildHourColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: hourColumnStartPadding__102738, end: ((this.pickerColumnWidth - hourColumnStartPadding__102738) - hourLabelContentWidth__102655)), hourSelectionOverlay__103347)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.pickerColumnWidth - minuteColumnEndPadding__103041) - minuteLabelContentWidth__102417), end: minuteColumnEndPadding__103041), minuteSelectionOverlay__103414)) };
            break;
        }
    case CupertinoTimerPickerMode.ms:
        {
            double secondLabelContentWidth__104595 = (baseLabelContentWidth__102326 + this.secondLabelWidth);
            double secondColumnEndPadding__104682 = ((this.pickerColumnWidth - secondLabelContentWidth__104595) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
            if ((secondColumnEndPadding__104682 < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
            {
                secondColumnEndPadding__104682 = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
            }
            double minuteColumnStartPadding__104987 = ((this.pickerColumnWidth - minuteLabelContentWidth__102417) - Date_pickerLibrary._kTimerPickerHalfColumnPadding);
            if ((minuteColumnStartPadding__104987 < Date_pickerLibrary._kTimerPickerMinHorizontalPadding))
            {
                minuteColumnStartPadding__104987 = Date_pickerLibrary._kTimerPickerMinHorizontalPadding;
            }
            global::Doroti.Generated.Framework.Widgets.Widget? minuteSelectionOverlay__105299 = Date_pickerLibrary._startSelectionOverlay;
            global::Doroti.Generated.Framework.Widgets.Widget? secondSelectionOverlay__105368 = Date_pickerLibrary._endSelectionOverlay;
            if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
            {
                minuteSelectionOverlay__105299 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 2L);
                secondSelectionOverlay__105368 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 2L);
            }
            columns__101634 = new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: minuteColumnStartPadding__104987, end: ((this.pickerColumnWidth - minuteColumnStartPadding__104987) - minuteLabelContentWidth__102417)), minuteSelectionOverlay__105299)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildSecondColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: ((this.pickerColumnWidth - secondColumnEndPadding__104682) - minuteLabelContentWidth__102417), end: secondColumnEndPadding__104682), secondSelectionOverlay__105368)) };
            break;
        }
    case CupertinoTimerPickerMode.hms:
        {
            double hourColumnEndPadding__106562 = (((this.pickerColumnWidth - baseLabelContentWidth__102326) - this.hourLabelWidth) - Date_pickerLibrary._kTimerPickerMinHorizontalPadding);
            double minuteColumnPadding__106770 = (((this.pickerColumnWidth - minuteLabelContentWidth__102417)) / 2L);
            double secondColumnStartPadding__106868 = (((this.pickerColumnWidth - baseLabelContentWidth__102326) - this.secondLabelWidth) - Date_pickerLibrary._kTimerPickerMinHorizontalPadding);
            global::Doroti.Generated.Framework.Widgets.Widget? hourSelectionOverlay__107078 = Date_pickerLibrary._startSelectionOverlay;
            global::Doroti.Generated.Framework.Widgets.Widget? minuteSelectionOverlay__107145 = Date_pickerLibrary._centerSelectionOverlay;
            global::Doroti.Generated.Framework.Widgets.Widget? secondSelectionOverlay__107215 = Date_pickerLibrary._endSelectionOverlay;
            if ((((CupertinoTimerPicker)this.widget).selectionOverlayBuilder is not null))
            {
                hourSelectionOverlay__107078 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 0L, columnCount: 3L);
                minuteSelectionOverlay__107145 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 1L, columnCount: 3L);
                secondSelectionOverlay__107215 = ((CupertinoTimerPicker)this.widget).selectionOverlayBuilder!(context, selectedIndex: 2L, columnCount: 3L);
            }
            columns__101634 = new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildHourColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Date_pickerLibrary._kTimerPickerMinHorizontalPadding, end: Math.Max(hourColumnEndPadding__106562, 0)), hourSelectionOverlay__107078)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildMinuteColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: minuteColumnPadding__106770, end: minuteColumnPadding__106770), minuteSelectionOverlay__107145)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(_buildSecondColumn(global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Math.Max(secondColumnStartPadding__106868, 0), end: Date_pickerLibrary._kTimerPickerMinHorizontalPadding), secondSelectionOverlay__107215)) };
            break;
        }
}
global::Doroti.Generated.Framework.Widgets.Widget contents__108680 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: this.totalWidth, height: Date_pickerLibrary._kPickerHeight, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: _textStyleFrom(context), child: new global::Doroti.Generated.Framework.Widgets.Row(children: columns__101634.map<global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Expanded>(((child) => new global::Doroti.Generated.Framework.Widgets.Expanded(child: child))).ToList().Cast<global::Doroti.Generated.Framework.Widgets.Widget>().ToList()))));
global::Doroti.Ui.Color? color__109068 = ((global::Doroti.Ui.Color?)(object?)CupertinoDynamicColor.maybeResolve(((CupertinoTimerPicker)this.widget).backgroundColor, context));
if ((color__109068 is not null))
{
    contents__108680 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ColoredBox(color: color__109068, child: contents__108680));
}
CupertinoThemeData themeData__109282 = CupertinoTheme.of(context);
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new CupertinoTheme(data: themeData__109282.copyWith(textTheme: themeData__109282.textTheme.copyWith(pickerTextStyle: _textStyleFrom(context, Date_pickerLibrary._kTimerPickerMagnification))), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: ((CupertinoTimerPicker)this.widget).alignment, child: contents__108680))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
