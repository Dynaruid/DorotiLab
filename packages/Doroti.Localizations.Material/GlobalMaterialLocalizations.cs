using System.Globalization;
using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Intl;
using Doroti.Localizations.Cupertino;
using Doroti.Localizations.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using global::Doroti.Material;

namespace Doroti.Localizations.Material;

public sealed class GlobalMaterialLocalizations : MaterialLocalizations
{
    private readonly MaterialStrings _strings;
    private readonly IntlFormatter _formatter;

    private GlobalMaterialLocalizations(Locale locale)
    {
        _formatter = new(locale.toLanguageTag());
        _strings = MaterialStrings.Get(locale);
    }

    public static IReadOnlyList<Locale> SupportedLocales => MaterialStrings.SupportedLocales;
    public static LocalizationsDelegate<MaterialLocalizations> Delegate { get; } = new ResourceDelegate();
    public static IReadOnlyList<ILocalizationsDelegate> Delegates { get; } =
        Array.AsReadOnly<ILocalizationsDelegate>([Delegate, GlobalCupertinoLocalizations.Delegate, GlobalWidgetsLocalizations.Delegate]);

    public override ScriptCategory scriptCategory => _strings.scriptCategory switch
    {
        "dense" => ScriptCategory.dense,
        "tall" => ScriptCategory.tall,
        _ => ScriptCategory.englishLike,
    };
    public override string anteMeridiemAbbreviation => _formatter.Culture.DateTimeFormat.AMDesignator;
    public override string postMeridiemAbbreviation => _formatter.Culture.DateTimeFormat.PMDesignator;
    public override TimeOfDayFormat timeOfDayFormat(bool alwaysUse24HourFormat = false) =>
        alwaysUse24HourFormat ? TimeOfDayFormat.HH_colon_mm : _strings.timeOfDayFormat switch
        {
            "H:mm" => TimeOfDayFormat.H_colon_mm,
            "HH:mm" => TimeOfDayFormat.HH_colon_mm,
            "HH.mm" => TimeOfDayFormat.HH_dot_mm,
            "HH 'h' mm" => TimeOfDayFormat.frenchCanadian,
            "a h:mm" or "ah:mm" => TimeOfDayFormat.a_space_h_colon_mm,
            "h:mm a" => TimeOfDayFormat.h_colon_mm_space_a,
            var pattern => throw new InvalidOperationException($"Unknown Material time layout '{pattern}'."),
        };

    public override string formatDecimal(long number) => _formatter.FormatInteger(number);
    public override string formatHour(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false) =>
        AsTime(timeOfDay).ToString(timeOfDayFormat(alwaysUse24HourFormat) switch
        {
            TimeOfDayFormat.h_colon_mm_space_a or TimeOfDayFormat.a_space_h_colon_mm => "%h",
            TimeOfDayFormat.H_colon_mm => "%H",
            _ => "HH",
        }, _formatter.Culture);
    public override string formatMinute(TimeOfDay timeOfDay) => _formatter.FormatMinute(AsTime(timeOfDay));
    public override string formatTimeOfDay(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false) =>
        AsTime(timeOfDay).ToString(timeOfDayFormat(alwaysUse24HourFormat) switch
        {
            TimeOfDayFormat.H_colon_mm => "H':'mm",
            TimeOfDayFormat.HH_dot_mm => "HH'.'mm",
            TimeOfDayFormat.frenchCanadian => "HH 'h' mm",
            TimeOfDayFormat.h_colon_mm_space_a => "h':'mm tt",
            TimeOfDayFormat.a_space_h_colon_mm => _strings.timeOfDayFormat == "ah:mm" ? "tth':'mm" : "tt h':'mm",
            _ => "HH':'mm",
        }, _formatter.Culture);
    public override string formatYear(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Year);
    public override string formatCompactDate(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Compact);
    public override string formatShortDate(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Short);
    public override string formatMediumDate(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Medium);
    public override string formatFullDate(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Full);
    public override string formatMonthYear(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.MonthYear);
    public override string formatShortMonthDay(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.ShortMonthDay);
    public override DateTime? parseCompactDate(string? inputString) =>
        _formatter.TryParseDate(inputString, out var date) ? date : null;

    public override List<string> narrowWeekdays => Enum.GetValues<DayOfWeek>()
        .Select(day => _formatter.LocaleName is "en-US" or "ko-KR"
            ? StringInfo.GetNextTextElement(_formatter.Culture.DateTimeFormat.GetAbbreviatedDayName(day))
            : _formatter.Culture.DateTimeFormat.GetShortestDayName(day)).ToList();
    public override long firstDayOfWeekIndex => (long)_formatter.FirstDayOfWeek;
    public override string dateSeparator => _formatter.LocaleName == "ko-KR" ? "." : _formatter.Culture.DateTimeFormat.DateSeparator;
    public override string aboutListTileTitle(string applicationName) => _strings.aboutListTileTitle(applicationName);
    public override string licensesPackageDetailText(long licenseCount) => _strings.licensesPackageDetailText(licenseCount);
    public override string pageRowsInfoTitle(long firstRow, long lastRow, long rowCount, bool rowCountIsApproximate) =>
        rowCountIsApproximate
            ? _strings.pageRowsInfoTitleApproximate(firstRow, lastRow, rowCount)
            : _strings.pageRowsInfoTitle(firstRow, lastRow, rowCount);
    public override string tabLabel(long tabIndex, long tabCount) => _strings.tabLabel(tabIndex, tabCount);
    public override string selectedRowCountTitle(long selectedRowCount) => _strings.selectedRowCountTitle(selectedRowCount);
    public override string scrimOnTapHint(string modalRouteContentName) => _strings.scrimOnTapHint(modalRouteContentName);
    public override string dateRangeStartDateSemanticLabel(string formattedDate) => _strings.dateRangeStartDateSemanticLabel(formattedDate);
    public override string dateRangeEndDateSemanticLabel(string formattedDate) => _strings.dateRangeEndDateSemanticLabel(formattedDate);
    public override string remainingTextFieldCharacterCount(long remaining) => _strings.remainingTextFieldCharacterCount(remaining);

    public override string openAppDrawerTooltip => _strings.openAppDrawerTooltip;
    public override string backButtonTooltip => _strings.backButtonTooltip;
    public override string clearButtonTooltip => _strings.clearButtonTooltip;
    public override string closeButtonTooltip => _strings.closeButtonTooltip;
    public override string deleteButtonTooltip => _strings.deleteButtonTooltip;
    public override string moreButtonTooltip => _strings.moreButtonTooltip;
    public override string nextMonthTooltip => _strings.nextMonthTooltip;
    public override string previousMonthTooltip => _strings.previousMonthTooltip;
    public override string firstPageTooltip => _strings.firstPageTooltip;
    public override string lastPageTooltip => _strings.lastPageTooltip;
    public override string nextPageTooltip => _strings.nextPageTooltip;
    public override string previousPageTooltip => _strings.previousPageTooltip;
    public override string showMenuTooltip => _strings.showMenuTooltip;
    public override string licensesPageTitle => _strings.licensesPageTitle;
    public override string rowsPerPageTitle => _strings.rowsPerPageTitle;
    public override string cancelButtonLabel => _strings.cancelButtonLabel;
    public override string closeButtonLabel => _strings.closeButtonLabel;
    public override string continueButtonLabel => _strings.continueButtonLabel;
    public override string copyButtonLabel => _strings.copyButtonLabel;
    public override string cutButtonLabel => _strings.cutButtonLabel;
    public override string scanTextButtonLabel => _strings.scanTextButtonLabel;
    public override string okButtonLabel => _strings.okButtonLabel;
    public override string pasteButtonLabel => _strings.pasteButtonLabel;
    public override string selectAllButtonLabel => _strings.selectAllButtonLabel;
    public override string lookUpButtonLabel => _strings.lookUpButtonLabel;
    public override string searchWebButtonLabel => _strings.searchWebButtonLabel;
    public override string shareButtonLabel => _strings.shareButtonLabel;
    public override string viewLicensesButtonLabel => _strings.viewLicensesButtonLabel;
    public override string timePickerHourModeAnnouncement => _strings.timePickerHourModeAnnouncement;
    public override string timePickerMinuteModeAnnouncement => _strings.timePickerMinuteModeAnnouncement;
    public override string modalBarrierDismissLabel => _strings.modalBarrierDismissLabel;
    public override string menuDismissLabel => _strings.menuDismissLabel;
    public override string drawerLabel => _strings.drawerLabel;
    public override string popupMenuLabel => _strings.popupMenuLabel;
    public override string menuBarMenuLabel => _strings.menuBarMenuLabel;
    public override string dialogLabel => _strings.dialogLabel;
    public override string alertDialogLabel => _strings.alertDialogLabel;
    public override string searchFieldLabel => _strings.searchFieldLabel;
    public override string currentDateLabel => _strings.currentDateLabel;
    public override string selectedDateLabel => _strings.selectedDateLabel;
    public override string scrimLabel => _strings.scrimLabel;
    public override string bottomSheetLabel => _strings.bottomSheetLabel;
    public override string dateHelpText => _formatter.LocaleName is "en-US" or "ko-KR"
        ? _strings.dateHelpText : _formatter.DateInputPattern;
    public override string selectYearSemanticsLabel => _strings.selectYearSemanticsLabel;
    public override string unspecifiedDate => _strings.unspecifiedDate;
    public override string unspecifiedDateRange => _strings.unspecifiedDateRange;
    public override string dateInputLabel => _strings.dateInputLabel;
    public override string dateRangeStartLabel => _strings.dateRangeStartLabel;
    public override string dateRangeEndLabel => _strings.dateRangeEndLabel;
    public override string invalidDateFormatLabel => _strings.invalidDateFormatLabel;
    public override string invalidDateRangeLabel => _strings.invalidDateRangeLabel;
    public override string dateOutOfRangeLabel => _strings.dateOutOfRangeLabel;
    public override string saveButtonLabel => _strings.saveButtonLabel;
    public override string datePickerHelpText => _strings.datePickerHelpText;
    public override string dateRangePickerHelpText => _strings.dateRangePickerHelpText;
    public override string calendarModeButtonLabel => _strings.calendarModeButtonLabel;
    public override string inputDateModeButtonLabel => _strings.inputDateModeButtonLabel;
    public override string timePickerDialHelpText => _strings.timePickerDialHelpText;
    public override string timePickerInputHelpText => _strings.timePickerInputHelpText;
    public override string timePickerHourLabel => _strings.timePickerHourLabel;
    public override string timePickerMinuteLabel => _strings.timePickerMinuteLabel;
    public override string invalidTimeLabel => _strings.invalidTimeLabel;
    public override string dialModeButtonLabel => _strings.dialModeButtonLabel;
    public override string inputTimeModeButtonLabel => _strings.inputTimeModeButtonLabel;
    public override string signedInLabel => _strings.signedInLabel;
    public override string hideAccountsLabel => _strings.hideAccountsLabel;
    public override string showAccountsLabel => _strings.showAccountsLabel;
    public override string reorderItemToStart => _strings.reorderItemToStart;
    public override string reorderItemToEnd => _strings.reorderItemToEnd;
    public override string reorderItemUp => _strings.reorderItemUp;
    public override string reorderItemDown => _strings.reorderItemDown;
    public override string reorderItemLeft => _strings.reorderItemLeft;
    public override string reorderItemRight => _strings.reorderItemRight;
    public override string expandedIconTapHint => _strings.expandedIconTapHint;
    public override string collapsedIconTapHint => _strings.collapsedIconTapHint;
    public override string expansionTileExpandedHint => _strings.expansionTileExpandedHint;
    public override string expansionTileCollapsedHint => _strings.expansionTileCollapsedHint;
    public override string expansionTileExpandedTapHint => _strings.expansionTileExpandedTapHint;
    public override string expansionTileCollapsedTapHint => _strings.expansionTileCollapsedTapHint;
    public override string expandedHint => _strings.expandedHint;
    public override string collapsedHint => _strings.collapsedHint;
    public override string refreshIndicatorSemanticLabel => _strings.refreshIndicatorSemanticLabel;
    public override string keyboardKeyAlt => _strings.keyboardKeyAlt;
    public override string keyboardKeyAltGraph => _strings.keyboardKeyAltGraph;
    public override string keyboardKeyBackspace => _strings.keyboardKeyBackspace;
    public override string keyboardKeyCapsLock => _strings.keyboardKeyCapsLock;
    public override string keyboardKeyChannelDown => _strings.keyboardKeyChannelDown;
    public override string keyboardKeyChannelUp => _strings.keyboardKeyChannelUp;
    public override string keyboardKeyControl => _strings.keyboardKeyControl;
    public override string keyboardKeyDelete => _strings.keyboardKeyDelete;
    public override string keyboardKeyEject => _strings.keyboardKeyEject;
    public override string keyboardKeyEnd => _strings.keyboardKeyEnd;
    public override string keyboardKeyEscape => _strings.keyboardKeyEscape;
    public override string keyboardKeyFn => _strings.keyboardKeyFn;
    public override string keyboardKeyHome => _strings.keyboardKeyHome;
    public override string keyboardKeyInsert => _strings.keyboardKeyInsert;
    public override string keyboardKeyMeta => _strings.keyboardKeyMeta;
    public override string keyboardKeyMetaMacOs => _strings.keyboardKeyMetaMacOs;
    public override string keyboardKeyMetaWindows => _strings.keyboardKeyMetaWindows;
    public override string keyboardKeyNumLock => _strings.keyboardKeyNumLock;
    public override string keyboardKeyNumpad1 => _strings.keyboardKeyNumpad1;
    public override string keyboardKeyNumpad2 => _strings.keyboardKeyNumpad2;
    public override string keyboardKeyNumpad3 => _strings.keyboardKeyNumpad3;
    public override string keyboardKeyNumpad4 => _strings.keyboardKeyNumpad4;
    public override string keyboardKeyNumpad5 => _strings.keyboardKeyNumpad5;
    public override string keyboardKeyNumpad6 => _strings.keyboardKeyNumpad6;
    public override string keyboardKeyNumpad7 => _strings.keyboardKeyNumpad7;
    public override string keyboardKeyNumpad8 => _strings.keyboardKeyNumpad8;
    public override string keyboardKeyNumpad9 => _strings.keyboardKeyNumpad9;
    public override string keyboardKeyNumpad0 => _strings.keyboardKeyNumpad0;
    public override string keyboardKeyNumpadAdd => _strings.keyboardKeyNumpadAdd;
    public override string keyboardKeyNumpadComma => _strings.keyboardKeyNumpadComma;
    public override string keyboardKeyNumpadDecimal => _strings.keyboardKeyNumpadDecimal;
    public override string keyboardKeyNumpadDivide => _strings.keyboardKeyNumpadDivide;
    public override string keyboardKeyNumpadEnter => _strings.keyboardKeyNumpadEnter;
    public override string keyboardKeyNumpadEqual => _strings.keyboardKeyNumpadEqual;
    public override string keyboardKeyNumpadMultiply => _strings.keyboardKeyNumpadMultiply;
    public override string keyboardKeyNumpadParenLeft => _strings.keyboardKeyNumpadParenLeft;
    public override string keyboardKeyNumpadParenRight => _strings.keyboardKeyNumpadParenRight;
    public override string keyboardKeyNumpadSubtract => _strings.keyboardKeyNumpadSubtract;
    public override string keyboardKeyPageDown => _strings.keyboardKeyPageDown;
    public override string keyboardKeyPageUp => _strings.keyboardKeyPageUp;
    public override string keyboardKeyPower => _strings.keyboardKeyPower;
    public override string keyboardKeyPowerOff => _strings.keyboardKeyPowerOff;
    public override string keyboardKeyPrintScreen => _strings.keyboardKeyPrintScreen;
    public override string keyboardKeyScrollLock => _strings.keyboardKeyScrollLock;
    public override string keyboardKeySelect => _strings.keyboardKeySelect;
    public override string keyboardKeyShift => _strings.keyboardKeyShift;
    public override string keyboardKeySpace => _strings.keyboardKeySpace;

    private static TimeOnly AsTime(TimeOfDay value) => new(checked((int)value.hour), checked((int)value.minute));

    private sealed class ResourceDelegate : LocalizationsDelegate<MaterialLocalizations>
    {
        public override bool isSupported(Locale locale) => MaterialStrings.IsSupported(locale);
        public override Future<MaterialLocalizations> load(Locale locale) =>
            new SynchronousFuture<MaterialLocalizations>(new GlobalMaterialLocalizations(locale));
        public override bool shouldReload(LocalizationsDelegate<MaterialLocalizations> old) => false;
    }
}
