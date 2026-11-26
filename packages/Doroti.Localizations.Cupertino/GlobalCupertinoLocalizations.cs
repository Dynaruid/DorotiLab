using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Intl;
using Doroti.Localizations.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using global::Doroti.Cupertino;

namespace Doroti.Localizations.Cupertino;

public sealed class GlobalCupertinoLocalizations : CupertinoLocalizations
{
    private readonly CupertinoStrings _strings;
    private readonly IntlFormatter _formatter;

    private GlobalCupertinoLocalizations(Locale locale)
    {
        _formatter = new(locale.toLanguageTag());
        _strings = CupertinoStrings.Get(locale);
    }

    public static IReadOnlyList<Locale> SupportedLocales => CupertinoStrings.SupportedLocales;
    public static LocalizationsDelegate<CupertinoLocalizations> Delegate { get; } = new ResourceDelegate();
    public static IReadOnlyList<ILocalizationsDelegate> Delegates { get; } =
        Array.AsReadOnly<ILocalizationsDelegate>([Delegate, GlobalWidgetsLocalizations.Delegate]);

    private System.Globalization.DateTimeFormatInfo DateTimeFormat => _formatter.Culture.DateTimeFormat;

    public override DatePickerDateOrder datePickerDateOrder => Enum.Parse<DatePickerDateOrder>(_strings.datePickerDateOrder);
    public override DatePickerDateTimeOrder datePickerDateTimeOrder =>
        Enum.Parse<DatePickerDateTimeOrder>(_strings.datePickerDateTimeOrder);
    public override string anteMeridiemAbbreviation => DateTimeFormat.AMDesignator;
    public override string postMeridiemAbbreviation => DateTimeFormat.PMDesignator;

    public override string datePickerYear(long yearIndex) => _strings.datePickerYearLabel(Field(yearIndex));
    public override string datePickerMonth(long monthIndex) => DateTimeFormat.GetMonthName(checked((int)monthIndex));
    public override string datePickerStandaloneMonth(long monthIndex) => datePickerMonth(monthIndex);
    public override string datePickerDayOfMonth(long dayIndex, long? weekDay = null)
    {
        if (weekDay is not long day) return _strings.datePickerDayLabel(Field(dayIndex));
        ArgumentOutOfRangeException.ThrowIfLessThan(day, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(day, 7);
        return _strings.datePickerDayWithWeekday(Field(dayIndex), DateTimeFormat.GetAbbreviatedDayName((DayOfWeek)(day % 7)));
    }
    public override string datePickerMediumDate(DateTime date) => _formatter.FormatDate(date, WidgetDateFormat.Medium);
    public override string datePickerHour(long hour) => Field(hour);
    public override string datePickerHourSemanticsLabel(long hour) => _strings.datePickerHourSemanticsLabel(hour);
    public override string datePickerMinute(long minute) => minute.ToString("D2", _formatter.Culture);
    public override string datePickerMinuteSemanticsLabel(long minute) => _strings.datePickerMinuteSemanticsLabel(minute);
    public override string tabSemanticsLabel(long tabIndex, long tabCount) => _strings.tabSemanticsLabel(tabIndex, tabCount);
    public override string timerPickerHour(long hour) => Field(hour);
    public override string timerPickerMinute(long minute) => Field(minute);
    public override string timerPickerSecond(long second) => Field(second);
    public override string timerPickerHourLabel(long hour) => _strings.timerPickerHourLabel(hour);
    public override List<string> timerPickerHourLabels => _strings.timerPickerHourLabels.Split('\n').ToList();
    public override string timerPickerMinuteLabel(long minute) => _strings.timerPickerMinuteLabel(minute);
    public override List<string> timerPickerMinuteLabels => _strings.timerPickerMinuteLabels.Split('\n').ToList();
    public override string timerPickerSecondLabel(long second) => _strings.timerPickerSecondLabel(second);
    public override List<string> timerPickerSecondLabels => _strings.timerPickerSecondLabels.Split('\n').ToList();

    public override string todayLabel => _strings.todayLabel;
    public override string alertDialogLabel => _strings.alertDialogLabel;
    public override string cutButtonLabel => _strings.cutButtonLabel;
    public override string copyButtonLabel => _strings.copyButtonLabel;
    public override string pasteButtonLabel => _strings.pasteButtonLabel;
    public override string clearButtonLabel => _strings.clearButtonLabel;
    public override string noSpellCheckReplacementsLabel => _strings.noSpellCheckReplacementsLabel;
    public override string selectAllButtonLabel => _strings.selectAllButtonLabel;
    public override string lookUpButtonLabel => _strings.lookUpButtonLabel;
    public override string searchWebButtonLabel => _strings.searchWebButtonLabel;
    public override string shareButtonLabel => _strings.shareButtonLabel;
    public override string searchTextFieldPlaceholderLabel => _strings.searchTextFieldPlaceholderLabel;
    public override string modalBarrierDismissLabel => _strings.modalBarrierDismissLabel;
    public override string menuDismissLabel => _strings.menuDismissLabel;
    public override string cancelButtonLabel => _strings.cancelButtonLabel;
    public override string backButtonLabel => _strings.backButtonLabel;
    public override string expansionTileExpandedHint => _strings.expansionTileExpandedHint;
    public override string expansionTileCollapsedHint => _strings.expansionTileCollapsedHint;
    public override string expansionTileExpandedTapHint => _strings.expansionTileExpandedTapHint;
    public override string expansionTileCollapsedTapHint => _strings.expansionTileCollapsedTapHint;
    public override string expandedHint => _strings.expandedHint;
    public override string collapsedHint => _strings.collapsedHint;

    private string Field(long value) => value.ToString("D", _formatter.Culture);

    private sealed class ResourceDelegate : LocalizationsDelegate<CupertinoLocalizations>
    {
        public override bool isSupported(Locale locale) => CupertinoStrings.IsSupported(locale);
        public override Future<CupertinoLocalizations> load(Locale locale) =>
            new SynchronousFuture<CupertinoLocalizations>(new GlobalCupertinoLocalizations(locale));
        public override bool shouldReload(LocalizationsDelegate<CupertinoLocalizations> old) => false;
    }
}
