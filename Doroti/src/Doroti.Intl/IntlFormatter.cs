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
/// P0 formatting contract for en-US and ko-KR. Instances own a read-only culture and never
/// change CurrentCulture. Further cultures and widget delegates require separate qualification.
/// </summary>
public sealed class IntlFormatter
{
    private static readonly Lazy<CultureInfo> English = new(() => CreateCulture("en-US"));
    private static readonly Lazy<CultureInfo> Korean = new(() => CreateCulture("ko-KR"));
    private static readonly string[] NumberFormats = CreateFormats('N');
    private static readonly string[] CurrencyFormats = CreateFormats('C');
    private static readonly string[] PercentFormats = CreateFormats('P');
    private readonly bool _korean;

    public IntlFormatter(string localeName)
    {
        ArgumentNullException.ThrowIfNull(localeName);
        // Neutral resource locales have an explicit, stable regional policy in this first profile.
        // Tag parsing/alias resolution belongs to P1; arbitrary variants/extensions are not accepted.
        if (localeName.Equals("en", StringComparison.OrdinalIgnoreCase)
            || localeName.Equals("en-US", StringComparison.OrdinalIgnoreCase))
        {
            Culture = English.Value;
        }
        else if (localeName.Equals("ko", StringComparison.OrdinalIgnoreCase)
            || localeName.Equals("ko-KR", StringComparison.OrdinalIgnoreCase))
        {
            Culture = Korean.Value;
            _korean = true;
        }
        else
        {
            throw new CultureNotFoundException(nameof(localeName), localeName,
                "The P0 formatter supports en-US and ko-KR (including en/ko neutral aliases).");
        }
    }

    public CultureInfo Culture { get; }
    public string LocaleName => Culture.Name;
    public string CurrencyCode => _korean ? "KRW" : "USD";
    public DayOfWeek FirstDayOfWeek => Culture.DateTimeFormat.FirstDayOfWeek;
    public string DateInputPattern => _korean ? "yyyy. MM. dd." : "MM/dd/yyyy";

    public string FormatInteger(long value) => value.ToString("N0", Culture);

    public string FormatNumber(decimal value, int fractionalDigits = 2) =>
        value.ToString(StandardFormat('N', fractionalDigits), Culture);

    /// <summary>Uses the culture's default currency (USD or KRW), including its default minor units.</summary>
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
        value.ToString(alwaysUse24HourFormat ? "HH:mm" : _korean ? "tt h:mm" : "h:mm tt", Culture);

    public string FormatHour(TimeOnly value, bool alwaysUse24HourFormat = false) =>
        value.ToString(alwaysUse24HourFormat ? "HH" : "%h", Culture);

    public string FormatMinute(TimeOnly value) => value.ToString("mm", Culture);

    private string DatePattern(WidgetDateFormat format) => (format, _korean) switch
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
        if (!CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Any(culture => culture.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Required .NET globalization data for {name} is unavailable. Invariant mode is not supported.");
        }

        var culture = new CultureInfo(name, useUserOverride: false);
        if (!culture.OptionalCalendars.Any(calendar => calendar is GregorianCalendar))
        {
            throw new InvalidOperationException($"The {name} profile requires a Gregorian calendar.");
        }
        culture.DateTimeFormat.Calendar = new GregorianCalendar();

        // A runtime can recognize a tag even when the selected WASM shard lacks its resources.
        // These are required profile data, checked before formatting rather than silently using root/en.
        var expectedJanuary = name == "ko-KR" ? "1월" : "January";
        // AM/PM strings can differ by .NET/ICU profile (for example ICU 72 vs ICU 78).
        // They are formatting data, not an invariant signal. Do not turn such a difference
        // into a missing-data error or substitute our own translation.
        if (culture.DateTimeFormat.GetMonthName(1) != expectedJanuary)
        {
            throw new InvalidOperationException($"Required .NET globalization resources for {name} are missing or incompatible. Select ICU data containing the profile's cultures.");
        }
        return CultureInfo.ReadOnly(culture);
    }
}
