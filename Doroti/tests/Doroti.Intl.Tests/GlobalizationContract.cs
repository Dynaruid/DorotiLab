using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doroti.Intl.Tests;

public static class GlobalizationContract
{
    public static string Run()
    {
        var coldStart = Stopwatch.GetTimestamp();
        var english = new IntlFormatter("en-US");
        var firstOutput = english.FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Full);
        var coldMicroseconds = Stopwatch.GetElapsedTime(coldStart).TotalMicroseconds;
        var korean = new IntlFormatter("ko-KR");
        var checks = 0;
        void Check(bool result, string description)
        {
            if (!result) throw new InvalidOperationException($"FAIL: {description}");
            checks++;
        }

        Check(firstOutput == "Thursday, February 29, 2024", "Gregorian full date fields");
        Check(korean.FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Full) == "2024년 2월 29일 목요일", "Korean culture resources");
        Check(new IntlFormatter("en").LocaleName == "en-US", "explicit neutral English policy");
        Check(new IntlFormatter("KO-kr").LocaleName == "ko-KR", "case-insensitive profile selection");
        Throws<CultureNotFoundException>(() => new IntlFormatter("qaaa-QZ"));
        Throws<CultureNotFoundException>(() => new IntlFormatter("en-US-u-ca-buddhist"));
        Check(new IntlFormatter("ko_KR").LocaleName == "ko-KR", "underscore profile normalization");
        Throws<ArgumentNullException>(() => new IntlFormatter(null!));

        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            foreach (var formatter in new[] { english, korean })
            {
                Check(formatter.Culture.IsReadOnly && formatter.Culture.DateTimeFormat.IsReadOnly
                    && formatter.Culture.NumberFormat.IsReadOnly, "immutable culture and child formats");
                Check(!formatter.Culture.UseUserOverride, "OS user override excluded from widget contract");
                Check(formatter.Culture.Calendar is GregorianCalendar, "Gregorian widget calendar");
                Check(formatter.FirstDayOfWeek == DayOfWeek.Sunday, "profile week starts Sunday");
                Throws<InvalidOperationException>(() => formatter.Culture.NumberFormat.NumberDecimalSeparator = "!");
                Check(formatter.FormatInteger(long.MinValue) == "-9,223,372,036,854,775,808", "long.MinValue without magnitude overflow");
                Check(formatter.FormatInteger(long.MaxValue) == "9,223,372,036,854,775,807", "large integer retains all digits");
                Check(formatter.FormatNumber(1234.5m) == "1,234.50", "Latin digits, explicit fraction and grouping");
                Check(formatter.FormatNumber(decimal.MaxValue, 0) == decimal.MaxValue.ToString("N0", formatter.Culture), "decimal full range");
                Check(formatter.FormatNumber(1.005m, 2) == 1.005m.ToString("N2", formatter.Culture), ".NET rounding semantics");
                Check(formatter.FormatCurrency(1234.5m) == 1234.5m.ToString("C", formatter.Culture), "culture currency minor units");
                Check(formatter.FormatCurrency(1234.5m, 3) == 1234.5m.ToString("C3", formatter.Culture), "explicit currency precision");
                Check(formatter.FormatPercent(0.125m, 1) == 0.125m.ToString("P1", formatter.Culture), "percent ratio semantics and culture spacing");
                Check(formatter.TryParseNumber("-1,234.50", out var number) && number == -1234.5m, "explicit numeric parsing culture");
                Check(!formatter.TryParseNumber("NaN", out _) && !formatter.TryParseNumber("Infinity", out _), "decimal excludes non-finite input");
                Check(!formatter.TryParseNumber("١٢٣", out _), "native digits are not declared supported");
                foreach (var date in new[] { new DateTime(1, 1, 1), new DateTime(2024, 2, 29), new DateTime(2023, 12, 31), DateTime.MaxValue.Date })
                {
                    var text = formatter.FormatDate(date, WidgetDateFormat.Compact);
                    Check(formatter.TryParseDate(text, out var parsed) && parsed == date
                        && parsed.Kind == DateTimeKind.Unspecified, "strict canonical date round trip");
                }
                var valid = formatter.FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Compact);
                Check(!formatter.TryParseDate(" " + valid, out _) && !formatter.TryParseDate(valid + " ", out _), "no implicit whitespace");
                Check(!formatter.TryParseDate(valid + "T00:00:00Z", out _), "date-only input excludes timezone suffixes");
                Check(!formatter.TryParseDate(valid.Replace("2024", "24", StringComparison.Ordinal), out _), "four-digit year required");
                Check(!formatter.TryParseDate(valid.Replace("2024", "2023", StringComparison.Ordinal), out _), "invalid leap day rejected");
                Check(!formatter.TryParseDate(null, out _) && !formatter.TryParseDate("", out _), "empty user input is an ordinary parse failure");
                Check(!formatter.TryParseDate(formatter.LocaleName == "en-US" ? "2/29/2024" : "2024. 2. 29.", out _), "canonical padding required");
                var dateWithoutKind = new DateTime(2024, 1, 1, 0, 30, 0);
                Check(formatter.FormatDate(DateTime.SpecifyKind(dateWithoutKind, DateTimeKind.Utc), WidgetDateFormat.Full)
                    == formatter.FormatDate(DateTime.SpecifyKind(dateWithoutKind, DateTimeKind.Local), WidgetDateFormat.Full), "formatting does not convert UTC/local");
                Check(formatter.FormatTime(new TimeOnly(0, 5), true) == "00:05", "24-hour midnight and zero padding");
                Check(formatter.FormatHour(new TimeOnly(0, 5)) == "12" && formatter.FormatMinute(new TimeOnly(0, 5)) == "05", "picker hour and minute fields");
                Check(formatter.FormatHour(new TimeOnly(3, 5), true) == "03", "24-hour picker hour padding");
                var afternoon = formatter.Culture.DateTimeFormat.PMDesignator;
                Check(formatter.FormatTime(new TimeOnly(13, 5)) == (formatter.LocaleName == "ko-KR" ? afternoon + " 1:05" : "1:05 " + afternoon), "profile day period order without overriding runtime data");
                foreach (var style in Enum.GetValues<WidgetDateFormat>())
                {
                    Check(!string.IsNullOrEmpty(formatter.FormatDate(dateWithoutKind, style)), "every declared date field combination");
                }
                Throws<ArgumentOutOfRangeException>(() => formatter.FormatNumber(1, -1));
                Throws<ArgumentOutOfRangeException>(() => formatter.FormatPercent(1, 29));
                Throws<ArgumentOutOfRangeException>(() => formatter.FormatCurrency(1, 29));
                Throws<ArgumentOutOfRangeException>(() => formatter.FormatDate(dateWithoutKind, (WidgetDateFormat)99));
            }
            Check(CultureInfo.CurrentCulture == CultureInfo.InvariantCulture
                && CultureInfo.CurrentUICulture == CultureInfo.InvariantCulture, "formatter leaves caller cultures untouched");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }

        // These cases are independently taken from the pinned CLDR rule boundaries, including
        // visible decimal zeros and Arabic integer-range membership (3.5 is not 'few').
        (string Language, decimal Value, PluralCategory Category)[] fixtures =
        [
            ("en", 0m, PluralCategory.Other), ("en", 1m, PluralCategory.One),
            ("en", -1m, PluralCategory.One), ("en", 2m, PluralCategory.Other),
            ("en", 1.0m, PluralCategory.Other), ("en", 1.00m, PluralCategory.Other),
            ("ko", 0m, PluralCategory.Other), ("ko", 1m, PluralCategory.Other),
            ("ko", 1.00m, PluralCategory.Other), ("ko", decimal.MaxValue, PluralCategory.Other),
            ("ru", 0m, PluralCategory.Many), ("ru", 1m, PluralCategory.One),
            ("ru", 2m, PluralCategory.Few), ("ru", 4m, PluralCategory.Few),
            ("ru", 5m, PluralCategory.Many), ("ru", 11m, PluralCategory.Many),
            ("ru", 12m, PluralCategory.Many), ("ru", 14m, PluralCategory.Many),
            ("ru", 21m, PluralCategory.One), ("ru", 22m, PluralCategory.Few),
            ("ru", 101m, PluralCategory.One), ("ru", 1.0m, PluralCategory.Other),
            ("ru", 2.1m, PluralCategory.Other), ("ru", -22m, PluralCategory.Few),
            ("ar", 0m, PluralCategory.Zero), ("ar", 0.0m, PluralCategory.Zero),
            ("ar", 1m, PluralCategory.One), ("ar", 2m, PluralCategory.Two),
            ("ar", 3m, PluralCategory.Few), ("ar", 3.0m, PluralCategory.Few),
            ("ar", 3.5m, PluralCategory.Other), ("ar", 10m, PluralCategory.Few),
            ("ar", 11m, PluralCategory.Many), ("ar", 99m, PluralCategory.Many),
            ("ar", 100m, PluralCategory.Other), ("ar", 102m, PluralCategory.Other),
            ("ar", 103m, PluralCategory.Few), ("ar", -11m, PluralCategory.Many),
            ("en", decimal.MinValue, PluralCategory.Other),
        ];
        foreach (var fixture in fixtures)
        {
            Check(CardinalPlural.Select(fixture.Language, fixture.Value) == fixture.Category,
                $"CLDR cardinal fixture {fixture.Language}/{fixture.Value.ToString(CultureInfo.InvariantCulture)}");
        }
        Check(CardinalPlural.Select("en", 1m, 1) == PluralCategory.Other, "visible precision pads zeros");
        Check(CardinalPlural.Select("en", 1.00m, 0) == PluralCategory.One, "explicit precision removes trailing zeros");
        Throws<ArgumentException>(() => CardinalPlural.Select("en", 1.5m, 0));
        Throws<ArgumentOutOfRangeException>(() => CardinalPlural.Select("en", 1m, 29));
        Throws<ArgumentOutOfRangeException>(() => CardinalPlural.Select("en", 1m, -1));
        Check(CardinalPlural.Select("fr", 0m) == PluralCategory.One, "French cardinal zero");
        Check(CardinalPlural.Select("en-US", 1m) == PluralCategory.One, "regional default cardinal rule");
        Check(CardinalPlural.Select("pt-PT", 0m) == PluralCategory.Other && CardinalPlural.Select("pt-BR", 0m) == PluralCategory.One,
            "European and Brazilian Portuguese use distinct cardinal rules");
        Throws<ArgumentException>(() => CardinalPlural.Select("qaaa-QZ", 1m));
        var exact = new Dictionary<decimal, string> { [0] = "zero exact", [1] = "one exact", [-1] = "negative exact" };
        var categories = new Dictionary<PluralCategory, string> { [PluralCategory.One] = "one", [PluralCategory.Other] = "other" };
        Check(CardinalPlural.SelectMessage("en", 0, exact, categories) == "zero exact", "=0 precedes other");
        Check(CardinalPlural.SelectMessage("en", 1.0m, exact, categories) == "one exact", "=1 precedes visible-decimal category");
        Check(CardinalPlural.SelectMessage("en", -1m, exact, categories) == "negative exact", "exact selector retains sign");
        Check(CardinalPlural.SelectMessage("ar", 3m, exact, categories) == "other", "missing optional category uses declared other");
        Throws<ArgumentException>(() => CardinalPlural.SelectMessage("en", 1m, exact, new Dictionary<PluralCategory, string>()));

        var report = new ContractReport
        {
            Runtime = RuntimeInformation.FrameworkDescription,
            OperatingSystem = RuntimeInformation.OSDescription,
            Architecture = RuntimeInformation.ProcessArchitecture.ToString(),
            SortVersion = $"{english.Culture.CompareInfo.Version.FullVersion}/{english.Culture.CompareInfo.Version.SortId}",
            Checks = checks,
            PluralFixtures = fixtures.Length,
            FirstContractFormatterMicroseconds = coldMicroseconds,
            FormatterCacheState = OperatingSystem.IsBrowser() ? "already-used-for-locale-transition" : "fresh-process",
            English = Capture(english),
            Korean = Capture(korean),
            AdapterCost = Measure(() => english.FormatNumber(1234.5m, 2)),
            DirectNetCost = Measure(() => 1234.5m.ToString("N2", english.Culture)),
            ParallelTasks = !OperatingSystem.IsBrowser() ? CheckConcurrentAsync().GetAwaiter().GetResult() : 0,
        };
        return JsonSerializer.Serialize(report, ContractJsonContext.Default.ContractReport);
    }

    public static async Task<int> CheckConcurrentAsync()
    {
        // Sixteen tasks, each using different operands; no unbounded stress loop.
        await Task.WhenAll(Enumerable.Range(0, 16).Select(index => Task.Run(() =>
        {
            var expected = index % 2 == 0 ? PluralCategory.One : PluralCategory.Other;
            if (CardinalPlural.Select("en", 1m, index % 2) != expected
                || new IntlFormatter(index % 2 == 0 ? "ko-KR" : "en-US").FormatInteger(1234) != "1,234")
            {
                throw new InvalidOperationException("Concurrent formatting/plural state leaked between calls.");
            }
        })));
        return 16;
    }

    private static CultureReport Capture(IntlFormatter formatter)
    {
        var currency = formatter.FormatCurrency(1234.5m);
        var percent = formatter.FormatPercent(0.125m, 1);
        return new CultureReport
        {
            Locale = formatter.LocaleName,
            Calendar = formatter.Culture.Calendar.GetType().Name,
            CompactDate = formatter.FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Compact),
            FullDate = formatter.FormatDate(new DateTime(2024, 2, 29), WidgetDateFormat.Full),
            Currency = currency,
            CurrencyCode = formatter.CurrencyCode,
            CurrencyCodePoints = CodePoints(currency),
            Percent = percent,
            PercentCodePoints = CodePoints(percent),
            Time12 = formatter.FormatTime(new TimeOnly(13, 5)),
            Time24 = formatter.FormatTime(new TimeOnly(13, 5), true),
        };
    }

    private static string CodePoints(string text) =>
        string.Join(" ", text.EnumerateRunes().Select(rune => $"U+{rune.Value:X4}"));

    private static CostReport Measure(Func<string> format)
    {
        var samples = new double[30];
        var bytes = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < samples.Length; index++)
        {
            var start = Stopwatch.GetTimestamp();
            _ = format();
            samples[index] = Stopwatch.GetElapsedTime(start).TotalMicroseconds;
        }
        var allocation = GC.GetAllocatedBytesForCurrentThread() - bytes;
        Array.Sort(samples);
        return new CostReport { Samples = samples.Length, P50Microseconds = samples[14], P95Microseconds = samples[28], AllocatedBytes = allocation };
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }
}

public sealed class ContractReport
{
    public string Runtime { get; set; } = "";
    public string OperatingSystem { get; set; } = "";
    public string Architecture { get; set; } = "";
    public string SortVersion { get; set; } = "";
    public int Checks { get; set; }
    public int PluralFixtures { get; set; }
    public int ParallelTasks { get; set; }
    public double FirstContractFormatterMicroseconds { get; set; }
    public string FormatterCacheState { get; set; } = "";
    public CultureReport English { get; set; } = new();
    public CultureReport Korean { get; set; } = new();
    public CostReport AdapterCost { get; set; } = new();
    public CostReport DirectNetCost { get; set; } = new();
}

public sealed class CultureReport
{
    public string Locale { get; set; } = "";
    public string Calendar { get; set; } = "";
    public string CompactDate { get; set; } = "";
    public string FullDate { get; set; } = "";
    public string Currency { get; set; } = "";
    public string CurrencyCode { get; set; } = "";
    public string CurrencyCodePoints { get; set; } = "";
    public string Percent { get; set; } = "";
    public string PercentCodePoints { get; set; } = "";
    public string Time12 { get; set; } = "";
    public string Time24 { get; set; } = "";
}

public sealed class CostReport
{
    public int Samples { get; set; }
    public double P50Microseconds { get; set; }
    public double P95Microseconds { get; set; }
    public long AllocatedBytes { get; set; }
}

[JsonSerializable(typeof(ContractReport))]
internal partial class ContractJsonContext : JsonSerializerContext;
