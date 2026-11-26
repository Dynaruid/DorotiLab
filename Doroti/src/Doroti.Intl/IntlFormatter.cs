using System.Collections.Concurrent;
using System.Globalization;

namespace Doroti.Intl;

/// <summary>The initial Gregorian widget field combinations. These are .NET patterns, not ICU skeletons.</summary>
public enum WidgetDateFormat
{
    Compact,
    Short,
    Medium,
    Full,
    MonthYear,
    ShortMonthDay,
    Year,
}

/// <summary>
/// Explicit .NET culture, Gregorian date input and widget formatting policies.
/// </summary>
public sealed class IntlFormatter
{
    private static readonly ConcurrentDictionary<string, Lazy<CultureInfo>> Cultures = new(StringComparer.Ordinal);
    private static readonly Lazy<bool> HasSpecificCultures = new(() =>
        CultureInfo.GetCultures(CultureTypes.SpecificCultures).Any(culture => culture.Name.Length != 0));
    private static readonly string[] NumberFormats = CreateFormats('N');
    private static readonly string[] CurrencyFormats = CreateFormats('C');
    private static readonly string[] PercentFormats = CreateFormats('P');
    private readonly bool _korean;
    private readonly bool _english;

    public IntlFormatter(string localeName)
    {
        ArgumentNullException.ThrowIfNull(localeName);
        var profile = IntlLocaleProfile.Resolve(localeName) ?? throw new CultureNotFoundException(
            nameof(localeName), localeName, "Expected a language[-Script][-Region] tag.");
        _korean = profile == "ko-KR";
        _english = profile == "en-US";
        Culture = GetCulture(profile);
        DateInputPattern = _korean ? "yyyy. MM. dd." : _english ? "MM/dd/yyyy" :
            IntlDateInputPattern.Canonicalize(Culture.DateTimeFormat.ShortDatePattern);
    }

    /// <summary>Whether the runtime provides the requested Gregorian formatting profile.</summary>
    public static bool SupportsLocale(string? localeName)
    {
        if (IntlLocaleProfile.Resolve(localeName) is not { } profile) return false;
        try { _ = GetCulture(profile); return true; }
        catch (CultureNotFoundException) { return false; }
        catch (InvalidOperationException) { return false; }
    }

    public CultureInfo Culture { get; }
    public string LocaleName => Culture.Name;
    public string CurrencyCode
    {
        get
        {
            var symbol = new RegionInfo(Culture.Name).ISOCurrencySymbol;
            // A BCP 47 macroregion can have formats without one national currency.
            return symbol.Length == 3 && symbol.All(char.IsAsciiLetterUpper) ? symbol : "XXX";
        }
    }
    public DayOfWeek FirstDayOfWeek => Culture.DateTimeFormat.FirstDayOfWeek;
    public string DateInputPattern { get; }

    private static CultureInfo GetCulture(string profile) =>
        Cultures.GetOrAdd(profile, static name => new(() => CreateCulture(name))).Value;

    public string FormatInteger(long value) => value.ToString("N0", Culture);

    public string FormatNumber(decimal value, int fractionalDigits = 2) =>
        value.ToString(StandardFormat('N', fractionalDigits), Culture);

    /// <summary>Uses the culture's default currency and minor units.</summary>
    public string FormatCurrency(decimal value, int? fractionalDigits = null) =>
        value.ToString(fractionalDigits is int digits ? StandardFormat('C', digits) : "C", Culture);

    /// <summary>Input is a ratio: 0.25 means 25 percent. Scaling and rounding are performed by .NET.</summary>
    public string FormatPercent(decimal ratio, int fractionalDigits = 0) =>
        ratio.ToString(StandardFormat('P', fractionalDigits), Culture);

    /// <summary>.NET NumberStyles.Number parsing; grouping placement is not a strict input grammar.</summary>
    public bool TryParseNumber(string? text, out decimal value) =>
        decimal.TryParse(text, NumberStyles.Number, Culture, out value);

    public string FormatDate(DateTime value, WidgetDateFormat format) =>
        value.ToString(DatePattern(format), Culture);

    /// <summary>
    /// Canonical, four-digit-year Gregorian input only. No whitespace, two-digit years, rollover,
    /// time component or implicit local/UTC conversion. Invalid user input returns false.
    /// </summary>
    public bool TryParseDate(string? text, out DateTime value)
    {
        if (DateTime.TryParseExact(text, DateInputPattern, Culture, DateTimeStyles.None, out var parsed)
            && string.Equals(text, FormatDate(parsed, WidgetDateFormat.Compact), StringComparison.Ordinal))
        {
            value = DateTime.SpecifyKind(parsed, DateTimeKind.Unspecified);
            return true;
        }
        value = default;
        return false;
    }

    public string FormatTime(TimeOnly value, bool alwaysUse24HourFormat = false) =>
        value.ToString(alwaysUse24HourFormat ? "HH':'mm" : _korean ? "tt h:mm" : _english ? "h:mm tt" : Culture.DateTimeFormat.ShortTimePattern, Culture);

    public string FormatHour(TimeOnly value, bool alwaysUse24HourFormat = false) =>
        value.ToString(alwaysUse24HourFormat ? "HH" : HourPattern(), Culture);

    public string FormatMinute(TimeOnly value) => value.ToString("mm", Culture);

    private string HourPattern()
    {
        if (_english || _korean) return "%h";
        var pattern = Culture.DateTimeFormat.ShortTimePattern;
        if (pattern.Contains("HH", StringComparison.Ordinal)) return "HH";
        if (pattern.Contains('H')) return "%H";
        return pattern.Contains("hh", StringComparison.Ordinal) ? "hh" : "%h";
    }

    private string DatePattern(WidgetDateFormat format)
    {
        if (!_english && !_korean)
        {
            var dateTime = Culture.DateTimeFormat;
            return format switch
            {
                WidgetDateFormat.Compact => DateInputPattern,
                WidgetDateFormat.Short => dateTime.ShortDatePattern,
                WidgetDateFormat.Medium => dateTime.MonthDayPattern + " (ddd)",
                WidgetDateFormat.Full => dateTime.LongDatePattern,
                WidgetDateFormat.MonthYear => dateTime.YearMonthPattern,
                WidgetDateFormat.ShortMonthDay => dateTime.MonthDayPattern,
                WidgetDateFormat.Year => "yyyy",
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };
        }
        return (format, _korean) switch
        {
            (WidgetDateFormat.Compact, _) => DateInputPattern,
            (WidgetDateFormat.Short, false) => "MMM d, yyyy",
            (WidgetDateFormat.Medium, false) => "ddd, MMM d",
            (WidgetDateFormat.Full, false) => "dddd, MMMM d, yyyy",
            (WidgetDateFormat.MonthYear, false) => "MMMM yyyy",
            (WidgetDateFormat.ShortMonthDay, false) => "MMM d",
            (WidgetDateFormat.Short, true) => "yyyy년 M월 d일",
            (WidgetDateFormat.Medium, true) => "M월 d일 (ddd)",
            (WidgetDateFormat.Full, true) => "yyyy년 M월 d일 dddd",
            (WidgetDateFormat.MonthYear, true) => "yyyy년 M월",
            (WidgetDateFormat.ShortMonthDay, true) => "M월 d일",
            (WidgetDateFormat.Year, _) => "yyyy",
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        };
    }

    private static string StandardFormat(char format, int fractionalDigits)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(fractionalDigits);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(fractionalDigits, 28);
        return (format switch
        {
            'N' => NumberFormats,
            'C' => CurrencyFormats,
            'P' => PercentFormats,
            _ => throw new ArgumentOutOfRangeException(nameof(format)),
        })[fractionalDigits];
    }

    private static string[] CreateFormats(char format) => Enumerable.Range(0, 29)
        .Select(digits => string.Concat(format, digits.ToString(CultureInfo.InvariantCulture))).ToArray();

    private static CultureInfo CreateCulture(string name)
    {
        // The public enumeration also catches invariant mode with PredefinedCulturesOnly=false,
        // where constructing a named culture can otherwise appear to succeed with invariant data.
        if (!HasSpecificCultures.Value)
        {
            throw new InvalidOperationException($"Required .NET globalization data for {name} is unavailable. Invariant mode is not supported.");
        }

        var known = CultureInfo.GetCultureInfo(name, predefinedOnly: true);
        var calendarCulture = known.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(known.Name).Name : known.Name;
        if (calendarCulture.Length == 0)
            throw new CultureNotFoundException(nameof(name), name, "The runtime did not resolve a specific formatting culture.");
        var culture = new CultureInfo(calendarCulture, useUserOverride: false);
        if (culture.DateTimeFormat.Calendar is not GregorianCalendar)
        {
            culture.DateTimeFormat.Calendar = culture.OptionalCalendars.OfType<GregorianCalendar>().FirstOrDefault()
                ?? throw new InvalidOperationException($"The {name} profile requires a Gregorian calendar.");
        }

        // A runtime can recognize a tag even when the selected WASM shard lacks its resources.
        // These are required profile data, checked before formatting rather than silently using root/en.
        var expectedJanuary = name switch { "ko-KR" => "1월", "en-US" => "January", _ => null };
        // AM/PM strings can differ by .NET/ICU profile (for example ICU 72 vs ICU 78).
        // They are formatting data, not an invariant signal. Do not turn such a difference
        // into a missing-data error or substitute our own translation.
        if (expectedJanuary is not null && culture.DateTimeFormat.GetMonthName(1) != expectedJanuary)
        {
            throw new InvalidOperationException($"Required .NET globalization resources for {name} are missing or incompatible. Select ICU data containing the profile's cultures.");
        }
        return CultureInfo.ReadOnly(culture);
    }
}
