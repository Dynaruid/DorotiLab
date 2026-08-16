// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/localizations.dart
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

public enum DatePickerDateTimeOrder
{
    date_time_dayPeriod,
    date_dayPeriod_time,
    time_dayPeriod_date,
    dayPeriod_time_date
}

public enum DatePickerDateOrder
{
    dmy,
    mdy,
    ymd,
    ydm
}

public abstract class CupertinoLocalizations
{
    public CupertinoLocalizations() { }

    public abstract string datePickerYear(long yearIndex);
    public abstract string datePickerMonth(long monthIndex);
    public abstract string datePickerStandaloneMonth(long monthIndex);
    public abstract string datePickerDayOfMonth(long dayIndex, long? weekDay = null);
    public abstract string datePickerMediumDate(DateTime date);
    public abstract string datePickerHour(long hour);
    public abstract string? datePickerHourSemanticsLabel(long hour);
    public abstract string datePickerMinute(long minute);
    public abstract string? datePickerMinuteSemanticsLabel(long minute);
    public abstract DatePickerDateOrder datePickerDateOrder { get; }
    public abstract DatePickerDateTimeOrder datePickerDateTimeOrder { get; }
    public abstract string anteMeridiemAbbreviation { get; }
    public abstract string postMeridiemAbbreviation { get; }
    public abstract string todayLabel { get; }
    public abstract string alertDialogLabel { get; }
    public abstract string tabSemanticsLabel(long tabIndex, long tabCount);
    public abstract string timerPickerHour(long hour);
    public abstract string timerPickerMinute(long minute);
    public abstract string timerPickerSecond(long second);
    public abstract string? timerPickerHourLabel(long hour);
    public abstract List<string> timerPickerHourLabels { get; }
    public abstract string? timerPickerMinuteLabel(long minute);
    public abstract List<string> timerPickerMinuteLabels { get; }
    public abstract string? timerPickerSecondLabel(long second);
    public abstract List<string> timerPickerSecondLabels { get; }
    public abstract string cutButtonLabel { get; }
    public abstract string copyButtonLabel { get; }
    public abstract string pasteButtonLabel { get; }
    public abstract string clearButtonLabel { get; }
    public abstract string noSpellCheckReplacementsLabel { get; }
    public abstract string selectAllButtonLabel { get; }
    public abstract string lookUpButtonLabel { get; }
    public abstract string searchWebButtonLabel { get; }
    public abstract string shareButtonLabel { get; }
    public abstract string searchTextFieldPlaceholderLabel { get; }
    public abstract string modalBarrierDismissLabel { get; }
    public abstract string menuDismissLabel { get; }
    public abstract string cancelButtonLabel { get; }
    public abstract string backButtonLabel { get; }
    public virtual string expansionTileExpandedHint => "double tap to collapse";
    public virtual string expansionTileCollapsedHint => "double tap to expand";
    public virtual string expansionTileExpandedTapHint => "Collapse";
    public virtual string expansionTileCollapsedTapHint => "Expand for more details";
    public virtual string expandedHint => "Collapsed";
    public virtual string collapsedHint => "Expanded";
    public static CupertinoLocalizations of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasCupertinoLocalizations(context));
        return Localizations.of<CupertinoLocalizations>(context, typeof(CupertinoLocalizations))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoLocalizationsDelegate__localizations : global::Doroti.Framework.Widgets.LocalizationsDelegate<CupertinoLocalizations>
{
    internal _CupertinoLocalizationsDelegate__localizations()
    {
    }

    public override bool isSupported(Locale locale) => DartRuntimePrimitives.ConvertValue<bool>((locale.languageCode == "en"));
    public override Future<CupertinoLocalizations> load(Locale locale) => DefaultCupertinoLocalizations.load(locale);
    public override bool shouldReload(global::Doroti.Framework.Widgets.LocalizationsDelegate<CupertinoLocalizations> old) => false;
    public override string ToString() => "DefaultCupertinoLocalizations.delegate(en_US)";
}

public class DefaultCupertinoLocalizations : CupertinoLocalizations
{
    internal static List<string> _shortWeekdays = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    internal static List<string> _shortMonths = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    internal static List<string> _months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    public static global::Doroti.Framework.Widgets.LocalizationsDelegate<CupertinoLocalizations> @delegate = ((global::Doroti.Framework.Widgets.LocalizationsDelegate<CupertinoLocalizations>)(object?)new _CupertinoLocalizationsDelegate__localizations());

    public DefaultCupertinoLocalizations()
    {
    }

    public override string datePickerYear(long yearIndex) => yearIndex.ToString();
    public override string datePickerMonth(long monthIndex) => _months[(int)((monthIndex - 1L))];
    public override string datePickerStandaloneMonth(long monthIndex) => _months[(int)((monthIndex - 1L))];
    public override string datePickerDayOfMonth(long dayIndex, long? weekDay = null)
    {
        if ((weekDay is not null))
        {
            long weekDay__value15239 = DartRuntimePrimitives.RequireValue(weekDay);
            return $" {_shortWeekdays[(int)((DartRuntimePrimitives.RequireValue(weekDay__value15239) - 1L))]} {dayIndex} ";
        }
        return dayIndex.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string datePickerHour(long hour) => hour.ToString();
    public override string? datePickerHourSemanticsLabel(long hour) => $"{hour} o'clock";
    public override string datePickerMinute(long minute) => minute.ToString().padLeft(2L, "0");
    public override string? datePickerMinuteSemanticsLabel(long minute)
    {
        if ((minute == 1L))
        {
            return "1 minute";
        }
        return $"{minute} minutes";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string datePickerMediumDate(DateTime date)
    {
        return $"{_shortWeekdays[(int)((date.DayOfWeek.ToDartWeekday() - 1L))]} " + $"{_shortMonths[(int)((date.Month - 1L))]} " + $"{date.Day.ToString().padRight(2L)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DatePickerDateOrder datePickerDateOrder => DatePickerDateOrder.mdy;
    public override DatePickerDateTimeOrder datePickerDateTimeOrder => DatePickerDateTimeOrder.date_time_dayPeriod;
    public override string anteMeridiemAbbreviation => "AM";
    public override string postMeridiemAbbreviation => "PM";
    public override string todayLabel => "Today";
    public override string alertDialogLabel => "Alert";
    public override string tabSemanticsLabel(long tabIndex, long tabCount)
    {
        DartRuntimePrimitives.Assert(() => (tabIndex >= 1L));
        DartRuntimePrimitives.Assert(() => (tabCount >= 1L));
        return $"Tab {tabIndex} of {tabCount}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string timerPickerHour(long hour) => hour.ToString();
    public override string timerPickerMinute(long minute) => minute.ToString();
    public override string timerPickerSecond(long second) => second.ToString();
    public override string? timerPickerHourLabel(long hour) => ((hour == 1L) ? "hour" : "hours");
    public override List<string> timerPickerHourLabels => new List<string> { "hour", "hours" };
    public override string? timerPickerMinuteLabel(long minute) => "min.";
    public override List<string> timerPickerMinuteLabels => new List<string> { "min." };
    public override string? timerPickerSecondLabel(long second) => "sec.";
    public override List<string> timerPickerSecondLabels => new List<string> { "sec." };
    public override string cutButtonLabel => "Cut";
    public override string copyButtonLabel => "Copy";
    public override string pasteButtonLabel => "Paste";
    public override string clearButtonLabel => "Clear";
    public override string noSpellCheckReplacementsLabel => "No Replacements Found";
    public override string selectAllButtonLabel => "Select All";
    public override string lookUpButtonLabel => "Look Up";
    public override string searchWebButtonLabel => "Search Web";
    public override string shareButtonLabel => "Share...";
    public override string searchTextFieldPlaceholderLabel => "Search";
    public override string modalBarrierDismissLabel => "Dismiss";
    public override string menuDismissLabel => "Dismiss menu";
    public override string cancelButtonLabel => "Cancel";
    public override string backButtonLabel => "Back";
    public override string expansionTileExpandedHint => "double tap to collapse";
    public override string expansionTileCollapsedHint => "double tap to expand";
    public override string expansionTileExpandedTapHint => "Collapse";
    public override string expansionTileCollapsedTapHint => "Expand for more details";
    public override string expandedHint => "Collapsed";
    public override string collapsedHint => "Expanded";
    public static Future<CupertinoLocalizations> load(Locale locale)
    {
        return ((Future<CupertinoLocalizations>)(object?)new global::Doroti.Framework.Foundation.SynchronousFuture<CupertinoLocalizations>(new DefaultCupertinoLocalizations()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
