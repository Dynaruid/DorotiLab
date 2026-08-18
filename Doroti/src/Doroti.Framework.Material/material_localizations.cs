// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/material_localizations.dart
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

public abstract class MaterialLocalizations
{
    public MaterialLocalizations() { }

    public abstract string openAppDrawerTooltip { get; }
    public abstract string backButtonTooltip { get; }
    public abstract string clearButtonTooltip { get; }
    public abstract string closeButtonTooltip { get; }
    public abstract string deleteButtonTooltip { get; }
    public abstract string moreButtonTooltip { get; }
    public abstract string nextMonthTooltip { get; }
    public abstract string previousMonthTooltip { get; }
    public abstract string firstPageTooltip { get; }
    public abstract string lastPageTooltip { get; }
    public abstract string nextPageTooltip { get; }
    public abstract string previousPageTooltip { get; }
    public abstract string showMenuTooltip { get; }
    public abstract string aboutListTileTitle(string applicationName);
    public abstract string licensesPageTitle { get; }
    public abstract string licensesPackageDetailText(long licenseCount);
    public abstract string pageRowsInfoTitle(long firstRow, long lastRow, long rowCount, bool rowCountIsApproximate);
    public abstract string rowsPerPageTitle { get; }
    public abstract string tabLabel(long tabIndex, long tabCount);
    public abstract string selectedRowCountTitle(long selectedRowCount);
    public abstract string cancelButtonLabel { get; }
    public abstract string closeButtonLabel { get; }
    public abstract string continueButtonLabel { get; }
    public abstract string copyButtonLabel { get; }
    public abstract string cutButtonLabel { get; }
    public abstract string scanTextButtonLabel { get; }
    public abstract string okButtonLabel { get; }
    public abstract string pasteButtonLabel { get; }
    public abstract string selectAllButtonLabel { get; }
    public abstract string lookUpButtonLabel { get; }
    public abstract string searchWebButtonLabel { get; }
    public abstract string shareButtonLabel { get; }
    public abstract string viewLicensesButtonLabel { get; }
    public abstract string anteMeridiemAbbreviation { get; }
    public abstract string postMeridiemAbbreviation { get; }
    public abstract string timePickerHourModeAnnouncement { get; }
    public abstract string timePickerMinuteModeAnnouncement { get; }
    public abstract string modalBarrierDismissLabel { get; }
    public abstract string menuDismissLabel { get; }
    public abstract string drawerLabel { get; }
    public abstract string popupMenuLabel { get; }
    public abstract string menuBarMenuLabel { get; }
    public abstract string dialogLabel { get; }
    public abstract string alertDialogLabel { get; }
    public abstract string searchFieldLabel { get; }
    public abstract string currentDateLabel { get; }
    public abstract string selectedDateLabel { get; }
    public abstract string scrimLabel { get; }
    public abstract string bottomSheetLabel { get; }
    public abstract string scrimOnTapHint(string modalRouteContentName);
    public abstract TimeOfDayFormat timeOfDayFormat(bool alwaysUse24HourFormat = false);
    public abstract ScriptCategory scriptCategory { get; }
    public abstract string formatDecimal(long number);
    public abstract string formatHour(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false);
    public abstract string formatMinute(TimeOfDay timeOfDay);
    public abstract string formatTimeOfDay(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false);
    public abstract string formatYear(DateTime date);
    public abstract string formatCompactDate(DateTime date);
    public abstract string formatShortDate(DateTime date);
    public abstract string formatMediumDate(DateTime date);
    public abstract string formatFullDate(DateTime date);
    public abstract string formatMonthYear(DateTime date);
    public abstract string formatShortMonthDay(DateTime date);
    public abstract DateTime? parseCompactDate(string? inputString);
    public abstract List<string> narrowWeekdays { get; }
    public abstract long firstDayOfWeekIndex { get; }
    public abstract string dateSeparator { get; }
    public abstract string dateHelpText { get; }
    public abstract string selectYearSemanticsLabel { get; }
    public abstract string unspecifiedDate { get; }
    public abstract string unspecifiedDateRange { get; }
    public abstract string dateInputLabel { get; }
    public abstract string dateRangeStartLabel { get; }
    public abstract string dateRangeEndLabel { get; }
    public abstract string dateRangeStartDateSemanticLabel(string formattedDate);
    public abstract string dateRangeEndDateSemanticLabel(string formattedDate);
    public abstract string invalidDateFormatLabel { get; }
    public abstract string invalidDateRangeLabel { get; }
    public abstract string dateOutOfRangeLabel { get; }
    public abstract string saveButtonLabel { get; }
    public abstract string datePickerHelpText { get; }
    public abstract string dateRangePickerHelpText { get; }
    public abstract string calendarModeButtonLabel { get; }
    public abstract string inputDateModeButtonLabel { get; }
    public abstract string timePickerDialHelpText { get; }
    public abstract string timePickerInputHelpText { get; }
    public abstract string timePickerHourLabel { get; }
    public abstract string timePickerMinuteLabel { get; }
    public abstract string invalidTimeLabel { get; }
    public abstract string dialModeButtonLabel { get; }
    public abstract string inputTimeModeButtonLabel { get; }
    public abstract string signedInLabel { get; }
    public abstract string hideAccountsLabel { get; }
    public abstract string showAccountsLabel { get; }
    public abstract string reorderItemToStart { get; }
    public abstract string reorderItemToEnd { get; }
    public abstract string reorderItemUp { get; }
    public abstract string reorderItemDown { get; }
    public abstract string reorderItemLeft { get; }
    public abstract string reorderItemRight { get; }
    public virtual string expandedIconTapHint => "Collapse";
    public virtual string collapsedIconTapHint => "Expand";
    public virtual string expansionTileExpandedHint => "double tap to collapse";
    public virtual string expansionTileCollapsedHint => "double tap to expand";
    public virtual string expansionTileExpandedTapHint => "Collapse";
    public virtual string expansionTileCollapsedTapHint => "Expand for more details";
    public virtual string expandedHint => "Collapsed";
    public virtual string collapsedHint => "Expanded";
    public abstract string remainingTextFieldCharacterCount(long remaining);
    public abstract string refreshIndicatorSemanticLabel { get; }
    public abstract string keyboardKeyAlt { get; }
    public abstract string keyboardKeyAltGraph { get; }
    public abstract string keyboardKeyBackspace { get; }
    public abstract string keyboardKeyCapsLock { get; }
    public abstract string keyboardKeyChannelDown { get; }
    public abstract string keyboardKeyChannelUp { get; }
    public abstract string keyboardKeyControl { get; }
    public abstract string keyboardKeyDelete { get; }
    public abstract string keyboardKeyEject { get; }
    public abstract string keyboardKeyEnd { get; }
    public abstract string keyboardKeyEscape { get; }
    public abstract string keyboardKeyFn { get; }
    public abstract string keyboardKeyHome { get; }
    public abstract string keyboardKeyInsert { get; }
    public abstract string keyboardKeyMeta { get; }
    public abstract string keyboardKeyMetaMacOs { get; }
    public abstract string keyboardKeyMetaWindows { get; }
    public abstract string keyboardKeyNumLock { get; }
    public abstract string keyboardKeyNumpad1 { get; }
    public abstract string keyboardKeyNumpad2 { get; }
    public abstract string keyboardKeyNumpad3 { get; }
    public abstract string keyboardKeyNumpad4 { get; }
    public abstract string keyboardKeyNumpad5 { get; }
    public abstract string keyboardKeyNumpad6 { get; }
    public abstract string keyboardKeyNumpad7 { get; }
    public abstract string keyboardKeyNumpad8 { get; }
    public abstract string keyboardKeyNumpad9 { get; }
    public abstract string keyboardKeyNumpad0 { get; }
    public abstract string keyboardKeyNumpadAdd { get; }
    public abstract string keyboardKeyNumpadComma { get; }
    public abstract string keyboardKeyNumpadDecimal { get; }
    public abstract string keyboardKeyNumpadDivide { get; }
    public abstract string keyboardKeyNumpadEnter { get; }
    public abstract string keyboardKeyNumpadEqual { get; }
    public abstract string keyboardKeyNumpadMultiply { get; }
    public abstract string keyboardKeyNumpadParenLeft { get; }
    public abstract string keyboardKeyNumpadParenRight { get; }
    public abstract string keyboardKeyNumpadSubtract { get; }
    public abstract string keyboardKeyPageDown { get; }
    public abstract string keyboardKeyPageUp { get; }
    public abstract string keyboardKeyPower { get; }
    public abstract string keyboardKeyPowerOff { get; }
    public abstract string keyboardKeyPrintScreen { get; }
    public abstract string keyboardKeyScrollLock { get; }
    public abstract string keyboardKeySelect { get; }
    public abstract string keyboardKeyShift { get; }
    public abstract string keyboardKeySpace { get; }
    public static MaterialLocalizations of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        return Localizations.of<MaterialLocalizations>(context, typeof(MaterialLocalizations))!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MaterialLocalizationsDelegate__material_localizations : global::Doroti.Framework.Widgets.LocalizationsDelegate<MaterialLocalizations>
{
    internal _MaterialLocalizationsDelegate__material_localizations()
    {
    }

    public override bool isSupported(Locale locale) => DartRuntimePrimitives.ConvertValue<bool>((locale.languageCode == "en"));
    public override Future<MaterialLocalizations> load(Locale locale) => DefaultMaterialLocalizations.load(locale);
    public override bool shouldReload(global::Doroti.Framework.Widgets.LocalizationsDelegate<MaterialLocalizations> old) => false;
    public override string ToString() => "DefaultMaterialLocalizations.delegate(en_US)";
}

public class DefaultMaterialLocalizations : MaterialLocalizations
{
    internal static List<string> _shortWeekdays = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    internal static List<string> _weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    internal static List<string> _narrowWeekdays = new List<string> { "S", "M", "T", "W", "T", "F", "S" };
    internal static List<string> _shortMonths = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    internal static List<string> _months = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    public static global::Doroti.Framework.Widgets.LocalizationsDelegate<MaterialLocalizations> @delegate = ((global::Doroti.Framework.Widgets.LocalizationsDelegate<MaterialLocalizations>)(object?)new _MaterialLocalizationsDelegate__material_localizations());

    public DefaultMaterialLocalizations()
    {
    }

    internal virtual long _getDaysInMonth(long year, long month)
    {
        if ((month == 2L))
        {
            bool isLeapYear__29255 = (((((year % 4L) == 0L)) && (((year % 100L) != 0L))) || (((year % 400L) == 0L)));
            if (isLeapYear__29255)
            {
                return 29L;
            }
            return 28L;
        }
        var daysInMonth__29411 = new List<long> { 31L, -1L, 31L, 30L, 31L, 30L, 31L, 31L, 30L, 31L, 30L, 31L };
        return daysInMonth__29411[(int)((month - 1L))];
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatHour(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false)
    {
        TimeOfDayFormat format__29639 = ((TimeOfDayFormat)(object?)timeOfDayFormat(alwaysUse24HourFormat: alwaysUse24HourFormat));
        switch (format__29639)
        {
            case var __constant29744 when (object.Equals(__constant29744, TimeOfDayFormat.h_colon_mm_space_a)):
                {
                    return ((string)(object?)formatDecimal(((timeOfDay.hourOfPeriod == 0L) ? 12L : timeOfDay.hourOfPeriod)));
                }
            case var __constant29880 when (object.Equals(__constant29880, TimeOfDayFormat.HH_colon_mm)):
                {
                    return ((string)(object?)_formatTwoDigitZeroPad(timeOfDay.hour));
                }
            case var __constant29975 when (object.Equals(__constant29975, TimeOfDayFormat.a_space_h_colon_mm)):
            case var __constant30022 when (object.Equals(__constant30022, TimeOfDayFormat.frenchCanadian)):
            case var __constant30065 when (object.Equals(__constant30065, TimeOfDayFormat.H_colon_mm)):
            case var __constant30104 when (object.Equals(__constant30104, TimeOfDayFormat.HH_dot_mm)):
                {
                    throw DartRuntimePrimitives.AsException(new AssertionError($"{this.GetType()} does not support {format__29639}."));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _formatTwoDigitZeroPad(long number)
    {
        DartRuntimePrimitives.Assert(() => ((0L <= number) && (number < 100L)));
        if ((number < 10L))
        {
            return $"0{number}";
        }
        return $"{number}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatMinute(TimeOfDay timeOfDay)
    {
        long minute__30595 = timeOfDay.minute;
        return ((minute__30595 < 10L) ? $"0{minute__30595}" : minute__30595.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatYear(DateTime date) => date.Year.ToString();
    public override string formatCompactDate(DateTime date)
    {
        string month__30866 = ((string)(object?)_formatTwoDigitZeroPad(date.Month));
        string day__30927 = ((string)(object?)_formatTwoDigitZeroPad(date.Day));
        string year__30984 = date.Year.ToString().padLeft(4L, "0");
        return $"{month__30866}/{day__30927}/{year__30984}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatShortDate(DateTime date)
    {
        string month__31137 = _shortMonths[(int)((date.Month - 1L))];
        return $"{month__31137} {date.Day}, {date.Year}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatMediumDate(DateTime date)
    {
        string day__31314 = _shortWeekdays[(int)((date.DayOfWeek.ToDartWeekday() - 1L))];
        string month__31385 = _shortMonths[(int)((date.Month - 1L))];
        return $"{day__31314}, {month__31385} {date.Day}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatFullDate(DateTime date)
    {
        string month__31552 = _months[(int)((date.Month - 1L))];
        return $"{_weekdays[(int)((date.DayOfWeek.ToDartWeekday() - 1L))]}, {month__31552} {date.Day}, {date.Year}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatMonthYear(DateTime date)
    {
        string year__31769 = ((string)(object?)formatYear(date));
        string month__31811 = _months[(int)((date.Month - 1L))];
        return $"{month__31811} {year__31769}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatShortMonthDay(DateTime date)
    {
        string month__31966 = _shortMonths[(int)((date.Month - 1L))];
        return $"{month__31966} {date.Day}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DateTime? parseCompactDate(string? inputString)
    {
        if ((inputString is null))
        {
            return null;
        }
        List<string> inputParts__32237 = inputString.split("/").ToList();
        if ((checked((long)(inputParts__32237.Count)) != 3L))
        {
            return null;
        }
        long? year__32349 = Dart_coreLibrary.tryParse(inputParts__32237[(int)(2L)], radix: 10L);
        if (((year__32349 is null) || (DartRuntimePrimitives.RequireValue(year__32349) < 1L)))
        {
            return null;
        }
        long? month__32473 = Dart_coreLibrary.tryParse(inputParts__32237[(int)(0L)], radix: 10L);
        if ((((month__32473 is null) || (DartRuntimePrimitives.RequireValue(month__32473) < 1L)) || (DartRuntimePrimitives.RequireValue(month__32473) > 12L)))
        {
            return null;
        }
        long? day__32614 = Dart_coreLibrary.tryParse(inputParts__32237[(int)(1L)], radix: 10L);
        if ((((day__32614 is null) || (DartRuntimePrimitives.RequireValue(day__32614) < 1L)) || (DartRuntimePrimitives.RequireValue(day__32614) > _getDaysInMonth(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(year__32349)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(month__32473))))))
        {
            return null;
        }
        try
        {
            return DartRuntimePrimitives.CreateDateTime(DartRuntimePrimitives.RequireValue(year__32349), DartRuntimePrimitives.RequireValue(month__32473), DartRuntimePrimitives.RequireValue(day__32614));
        }
        catch (DartArgumentError)
        {
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<string> narrowWeekdays => _narrowWeekdays;
    public override long firstDayOfWeekIndex => 0L;
    public override string dateSeparator => "/";
    public override string dateHelpText => "mm/dd/yyyy";
    public override string selectYearSemanticsLabel => "Select year";
    public override string unspecifiedDate => "Date";
    public override string unspecifiedDateRange => "Date Range";
    public override string dateInputLabel => "Enter Date";
    public override string dateRangeStartLabel => "Start Date";
    public override string dateRangeEndLabel => "End Date";
    public override string dateRangeStartDateSemanticLabel(string formattedDate) => $"Start date {formattedDate}";
    public override string dateRangeEndDateSemanticLabel(string formattedDate) => $"End date {formattedDate}";
    public override string invalidDateFormatLabel => "Invalid format.";
    public override string invalidDateRangeLabel => "Invalid range.";
    public override string dateOutOfRangeLabel => "Out of range.";
    public override string saveButtonLabel => "Save";
    public override string datePickerHelpText => "Select date";
    public override string dateRangePickerHelpText => "Select range";
    public override string calendarModeButtonLabel => "Switch to calendar";
    public override string inputDateModeButtonLabel => "Switch to input";
    public override string timePickerDialHelpText => "Select time";
    public override string timePickerInputHelpText => "Enter time";
    public override string timePickerHourLabel => "Hour";
    public override string timePickerMinuteLabel => "Minute";
    public override string invalidTimeLabel => "Enter a valid time";
    public override string dialModeButtonLabel => "Switch to dial picker mode";
    public override string inputTimeModeButtonLabel => "Switch to text input mode";
    internal virtual string _formatDayPeriod(TimeOfDay timeOfDay)
    {
        return (timeOfDay.period switch { var __constant34816 when (object.Equals(__constant34816, DayPeriod.am)) => this.anteMeridiemAbbreviation, var __constant34864 when (object.Equals(__constant34864, DayPeriod.pm)) => this.postMeridiemAbbreviation, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatDecimal(long number)
    {
        if (((number > -1000L) && (number < 1000L)))
        {
            return number.ToString();
        }
        var digits__35059 = number.abs().ToString();
        var result__35103 = new StringBuffer(((number < 0L) ? "-" : ""));
        long maxDigitIndex__35163 = (digits__35059.Length - 1L);
        for (var i__35211 = 0L; (i__35211 <= maxDigitIndex__35163); i__35211 += 1L)
        {
            result__35103.write(digits__35059[(int)(i__35211)].ToString());
            if (((i__35211 < maxDigitIndex__35163) && ((((maxDigitIndex__35163 - i__35211)) % 3L) == 0L)))
            {
                result__35103.write(",");
            }
        }
        return result__35103.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string formatTimeOfDay(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false)
    {
        var buffer__35986 = new StringBuffer();
        DartRuntimePrimitives.Ignore(((Func<StringBuffer>)(() =>
{
    var __cascade = buffer__35986;
    __cascade.write(formatHour(timeOfDay, alwaysUse24HourFormat: alwaysUse24HourFormat));
    __cascade.write(":");
    __cascade.write(formatMinute(timeOfDay));
    return __cascade;
}))());
        if (alwaysUse24HourFormat)
        {
            return $"{buffer__35986}";
        }
        DartRuntimePrimitives.Ignore(((Func<StringBuffer>)(() =>
{
    var __cascade = buffer__35986;
    __cascade.write(" ");
    __cascade.write(_formatDayPeriod(timeOfDay));
    return __cascade;
}))());
        return $"{buffer__35986}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string openAppDrawerTooltip => "Open navigation menu";
    public override string backButtonTooltip => "Back";
    public override string clearButtonTooltip => "Clear text";
    public override string closeButtonTooltip => "Close";
    public override string deleteButtonTooltip => "Delete";
    public override string moreButtonTooltip => "More";
    public override string nextMonthTooltip => "Next month";
    public override string previousMonthTooltip => "Previous month";
    public override string nextPageTooltip => "Next page";
    public override string previousPageTooltip => "Previous page";
    public override string firstPageTooltip => "First page";
    public override string lastPageTooltip => "Last page";
    public override string showMenuTooltip => "Show menu";
    public override string drawerLabel => "Navigation menu";
    public override string menuBarMenuLabel => "Menu bar menu";
    public override string popupMenuLabel => "Popup menu";
    public override string dialogLabel => "Dialog";
    public override string alertDialogLabel => "Alert";
    public override string searchFieldLabel => "Search";
    public override string currentDateLabel => "Today";
    public override string selectedDateLabel => "Selected";
    public override string scrimLabel => "Scrim";
    public override string bottomSheetLabel => "Bottom Sheet";
    public override string scrimOnTapHint(string modalRouteContentName) => $"Close {modalRouteContentName}";
    public override string aboutListTileTitle(string applicationName) => $"About {applicationName}";
    public override string licensesPageTitle => "Licenses";
    public override string licensesPackageDetailText(long licenseCount)
    {
        DartRuntimePrimitives.Assert(() => (licenseCount >= 0L));
        return (licenseCount switch { 0L => "No licenses.", 1L => "1 license.", _ => $"{licenseCount} licenses." });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string pageRowsInfoTitle(long firstRow, long lastRow, long rowCount, bool rowCountIsApproximate)
    {
        return (rowCountIsApproximate ? $"{firstRow}–{lastRow} of about {rowCount}" : $"{firstRow}–{lastRow} of {rowCount}");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string rowsPerPageTitle => "Rows per page:";
    public override string tabLabel(long tabIndex, long tabCount)
    {
        DartRuntimePrimitives.Assert(() => (tabIndex >= 1L));
        DartRuntimePrimitives.Assert(() => (tabCount >= 1L));
        return $"Tab {tabIndex} of {tabCount}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string selectedRowCountTitle(long selectedRowCount)
    {
        return (selectedRowCount switch { 0L => "No items selected", 1L => "1 item selected", _ => $"{selectedRowCount} items selected" });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string cancelButtonLabel => "Cancel";
    public override string closeButtonLabel => "Close";
    public override string continueButtonLabel => "Continue";
    public override string copyButtonLabel => "Copy";
    public override string cutButtonLabel => "Cut";
    public override string scanTextButtonLabel => "Scan text";
    public override string okButtonLabel => "OK";
    public override string pasteButtonLabel => "Paste";
    public override string selectAllButtonLabel => "Select all";
    public override string lookUpButtonLabel => "Look Up";
    public override string searchWebButtonLabel => "Search Web";
    public override string shareButtonLabel => "Share";
    public override string viewLicensesButtonLabel => "View licenses";
    public override string anteMeridiemAbbreviation => "AM";
    public override string postMeridiemAbbreviation => "PM";
    public override string timePickerHourModeAnnouncement => "Select hours";
    public override string timePickerMinuteModeAnnouncement => "Select minutes";
    public override string modalBarrierDismissLabel => "Dismiss";
    public override string menuDismissLabel => "Dismiss menu";
    public override ScriptCategory scriptCategory => ScriptCategory.englishLike;
    public override TimeOfDayFormat timeOfDayFormat(bool alwaysUse24HourFormat = false)
    {
        return (alwaysUse24HourFormat ? TimeOfDayFormat.HH_colon_mm : TimeOfDayFormat.h_colon_mm_space_a);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string signedInLabel => "Signed in";
    public override string hideAccountsLabel => "Hide accounts";
    public override string showAccountsLabel => "Show accounts";
    public override string reorderItemUp => "Move up";
    public override string reorderItemDown => "Move down";
    public override string reorderItemLeft => "Move left";
    public override string reorderItemRight => "Move right";
    public override string reorderItemToEnd => "Move to the end";
    public override string reorderItemToStart => "Move to the start";
    public override string expandedIconTapHint => "Collapse";
    public override string collapsedIconTapHint => "Expand";
    public override string expansionTileExpandedHint => "double tap to collapse";
    public override string expansionTileCollapsedHint => "double tap to expand";
    public override string expansionTileExpandedTapHint => "Collapse";
    public override string expansionTileCollapsedTapHint => "Expand for more details";
    public override string expandedHint => "Collapsed";
    public override string collapsedHint => "Expanded";
    public override string refreshIndicatorSemanticLabel => "Refresh";
    public static Future<MaterialLocalizations> load(Locale locale)
    {
        return ((Future<MaterialLocalizations>)(object?)new global::Doroti.Framework.Foundation.SynchronousFuture<MaterialLocalizations>(new DefaultMaterialLocalizations()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string remainingTextFieldCharacterCount(long remaining)
    {
        return (remaining switch { 0L => "No characters remaining", 1L => "1 character remaining", _ => $"{remaining} characters remaining" });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string keyboardKeyAlt => "Alt";
    public override string keyboardKeyAltGraph => "AltGr";
    public override string keyboardKeyBackspace => "Backspace";
    public override string keyboardKeyCapsLock => "Caps Lock";
    public override string keyboardKeyChannelDown => "Channel Down";
    public override string keyboardKeyChannelUp => "Channel Up";
    public override string keyboardKeyControl => "Ctrl";
    public override string keyboardKeyDelete => "Del";
    public override string keyboardKeyEject => "Eject";
    public override string keyboardKeyEnd => "End";
    public override string keyboardKeyEscape => "Esc";
    public override string keyboardKeyFn => "Fn";
    public override string keyboardKeyHome => "Home";
    public override string keyboardKeyInsert => "Insert";
    public override string keyboardKeyMeta => "Meta";
    public override string keyboardKeyMetaMacOs => "Command";
    public override string keyboardKeyMetaWindows => "Win";
    public override string keyboardKeyNumLock => "Num Lock";
    public override string keyboardKeyNumpad1 => "Num 1";
    public override string keyboardKeyNumpad2 => "Num 2";
    public override string keyboardKeyNumpad3 => "Num 3";
    public override string keyboardKeyNumpad4 => "Num 4";
    public override string keyboardKeyNumpad5 => "Num 5";
    public override string keyboardKeyNumpad6 => "Num 6";
    public override string keyboardKeyNumpad7 => "Num 7";
    public override string keyboardKeyNumpad8 => "Num 8";
    public override string keyboardKeyNumpad9 => "Num 9";
    public override string keyboardKeyNumpad0 => "Num 0";
    public override string keyboardKeyNumpadAdd => "Num +";
    public override string keyboardKeyNumpadComma => "Num ,";
    public override string keyboardKeyNumpadDecimal => "Num .";
    public override string keyboardKeyNumpadDivide => "Num /";
    public override string keyboardKeyNumpadEnter => "Num Enter";
    public override string keyboardKeyNumpadEqual => "Num =";
    public override string keyboardKeyNumpadMultiply => "Num *";
    public override string keyboardKeyNumpadParenLeft => "Num (";
    public override string keyboardKeyNumpadParenRight => "Num )";
    public override string keyboardKeyNumpadSubtract => "Num -";
    public override string keyboardKeyPageDown => "PgDown";
    public override string keyboardKeyPageUp => "PgUp";
    public override string keyboardKeyPower => "Power";
    public override string keyboardKeyPowerOff => "Power Off";
    public override string keyboardKeyPrintScreen => "Print Screen";
    public override string keyboardKeyScrollLock => "Scroll Lock";
    public override string keyboardKeySelect => "Select";
    public override string keyboardKeyShift => "Shift";
    public override string keyboardKeySpace => "Space";
}
