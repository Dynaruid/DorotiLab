using System.Globalization;
using Doroti.Localization.Tests;
using Doroti.Intl;
using Doroti.Localizations.Cupertino;
using Doroti.Localizations.Material;
using Doroti.Localizations.Widgets;
using Doroti.Material;
using Doroti.Ui;

if (args is ["--plural-corpus", var corpus])
{
    var count = 0;
    foreach (var row in File.ReadLines(corpus))
    {
        var fields = row.Split('\t');
        var value = decimal.Parse(fields[1], CultureInfo.InvariantCulture);
        Check(CardinalPlural.Select(fields[0], value).ToString().Equals(fields[2], StringComparison.OrdinalIgnoreCase), row);
        count++;
    }
    Console.WriteLine($"PASS: {count} plain decimal examples from CLDR 49 cardinal rules");
    return;
}

var en = new Locale("en", "US");
var ko = new Locale("ko", "KR");
var leapDay = new DateTime(2024, 2, 29);
var english = ContractLocalizations.Get(en);
var korean = ContractLocalizations.Get(ko);
Check(english.Locale == en && korean.Locale == ko, "generated parent lookup preserves the resolved locale");
Check(english.greeting("Ada", 0, "gift") == "{Hello} Ada: nothing.", "escaping and exact branch");
Check(english.greeting("Ada", 1, "gift") == "{Hello} Ada: a gift.", "nested select");
Check(english.greeting("Ada", 1.0m, "gift") == "{Hello} Ada: 1.0 items.", "decimal scale controls plural");
Check(korean.greeting("민수", 0, "gift") == "민수님, 선물 없음", "Korean nested selection");
Check(korean.stamp(leapDay, 50m, .5m, new TimeOnly(3, 5)).Contains("2024. 02. 29.", StringComparison.Ordinal), "typed date metadata");
Check(english.stamp(leapDay, 50m, .5m, new TimeOnly(3, 5)) == "Date 02/29/2024, value $50.00, tax 50%, time 03:05",
    "typed currency, percent and time metadata");
Check(!ContractLocalizations.Delegate.isSupported(new Locale("ru")), "no implicit English ARB fallback");

var material = GlobalMaterialLocalizations.Delegate.load(ko).asTask().GetAwaiter().GetResult();
Check(material.cancelButtonLabel == "취소" && material.selectedRowCountTitle(0) == "선택한 항목 없음", "Korean Material labels");
Check(material.formatHour(new TimeOfDay(3, 7), alwaysUse24HourFormat: true) == "03"
    && material.formatTimeOfDay(new TimeOfDay(3, 7), alwaysUse24HourFormat: true) == "03:07", "Material 24h padding");
Check(material.parseCompactDate("2024. 02. 29.") == leapDay && material.parseCompactDate("2023. 02. 29.") is null,
    "Material Gregorian strict parsing");
Check(new DefaultMaterialLocalizations().cancelButtonLabel == "Cancel", "core English remains available without opt-in");

if (args is ["--selected"])
{
    Check(GlobalMaterialLocalizations.SupportedLocales.Select(locale => locale.toLanguageTag()).SequenceEqual(["en", "ko"]),
        "the selected source build embeds only the explicitly requested locale resources");
    Console.WriteLine("PASS: source-selected widget registry");
    return;
}

var ar = new Locale("ar");
Check(GlobalWidgetsLocalizations.Delegate.load(ar).asTask().Result.textDirection == TextDirection.rtl
    && GlobalWidgetsLocalizations.Delegate.load(new Locale("ar", scriptCode: "Latn")).asTask().Result.textDirection == TextDirection.ltr,
    "direction follows explicit script and Arabic resource direction");
Check(GlobalWidgetsLocalizations.Delegate.load(new Locale("ug")).asTask().Result.textDirection == TextDirection.rtl,
    "Arabic-script Uyghur direction");
Check(!GlobalCupertinoLocalizations.Delegate.isSupported(new Locale("ps"))
    && GlobalMaterialLocalizations.Delegate.isSupported(new Locale("ps")), "pinned design coverage for Pashto");
Check(GlobalMaterialLocalizations.Delegate.load(new Locale("zh", scriptCode: "Hant")).asTask().Result.cancelButtonLabel
    != GlobalMaterialLocalizations.Delegate.load(new Locale("zh", scriptCode: "Hans")).asTask().Result.cancelButtonLabel
    || GlobalMaterialLocalizations.Delegate.load(new Locale("zh", scriptCode: "Hant")).asTask().Result.aboutListTileTitle("Doroti")
    != GlobalMaterialLocalizations.Delegate.load(new Locale("zh", scriptCode: "Hans")).asTask().Result.aboutListTileTitle("Doroti"),
    "distinct Chinese script bundles");

var errors = new List<string>();
foreach (var locale in GlobalMaterialLocalizations.SupportedLocales)
{
    try
    {
        var localized = GlobalMaterialLocalizations.Delegate.load(locale).asTask().GetAwaiter().GetResult();
        var formatter = new IntlFormatter(locale.toLanguageTag());
        var compact = localized.formatCompactDate(leapDay);
        Check(localized.parseCompactDate(compact) == leapDay, $"round-trip '{compact}' with '{formatter.DateInputPattern}'");
        Check(localized.narrowWeekdays.Count == 7 && localized.formatFullDate(leapDay).Length > 0
            && localized.licensesPackageDetailText(12).Length > 0 && localized.formatTimeOfDay(new TimeOfDay(13, 5)).Length > 0,
            "localized number, date and time");
        Check(formatter.CurrencyCode.Length == 3 && formatter.FormatCurrency(400m).Length > 0, "currency profile");
    }
    catch (Exception error) { errors.Add($"Material/{locale.toLanguageTag()} {error.Message}"); }
}
foreach (var locale in GlobalCupertinoLocalizations.SupportedLocales)
{
    try
    {
        var localized = GlobalCupertinoLocalizations.Delegate.load(locale).asTask().GetAwaiter().GetResult();
        _ = localized.datePickerDateOrder;
        _ = localized.datePickerDateTimeOrder;
        Check(localized.datePickerMonth(1).Length > 0 && localized.datePickerYear(2024).Length > 0
            && localized.timerPickerMinuteLabels.Count > 0 && localized.datePickerHourSemanticsLabel(3)?.Length > 0,
            "Cupertino Gregorian fields and accessibility strings");
    }
    catch (Exception error) { errors.Add($"Cupertino/{locale.toLanguageTag()} {error.Message}"); }
}
foreach (var locale in GlobalWidgetsLocalizations.SupportedLocales)
    Check(GlobalWidgetsLocalizations.Delegate.load(locale).asTask().Result.copyButtonLabel.Length > 0, $"Widgets/{locale.toLanguageTag()}");
Check(errors.Count == 0, string.Join(Environment.NewLine, errors));
Console.WriteLine($"PASS: static ARB generation and widget resources: Widgets {GlobalWidgetsLocalizations.SupportedLocales.Count}, "
    + $"Cupertino {GlobalCupertinoLocalizations.SupportedLocales.Count}, Material {GlobalMaterialLocalizations.SupportedLocales.Count}");

LocalizationScopeRegression.Run();

static void Check(bool value, string label)
{
    if (!value) throw new InvalidOperationException(label);
}
