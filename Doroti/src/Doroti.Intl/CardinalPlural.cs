// Cardinal rules derived from Unicode CLDR 49; Copyright Unicode, Inc.
// SPDX-License-Identifier: Unicode-3.0. See data/plural-provenance.json and LICENSES/Unicode-3.0.txt.
namespace Doroti.Intl;

public enum PluralCategory
{
    Zero,
    One,
    Two,
    Few,
    Many,
    Other,
}

/// <summary>
/// A static CLDR 49 cardinal subset. Language codes are en, ko, ru and ar; this is not a locale
/// negotiator. Decimal scale preserves visible trailing zeros. No process-global calculation state.
/// </summary>
public static class CardinalPlural
{
    public const string CldrVersion = "49";

    /// <param name="visibleFractionDigits">
    /// Null preserves decimal scale. An explicit value (0..28) may pad/remove trailing zeros but
    /// must represent the value exactly. Round the input explicitly before choosing a category.
    /// </param>
    public static PluralCategory Select(string languageCode, decimal value, int? visibleFractionDigits = null)
    {
        ValidateLanguage(languageCode);
        var digits = FractionDigits(value, visibleFractionDigits);
        var number = decimal.Abs(value);
        var integer = decimal.Truncate(number);

        return languageCode switch
        {
            "en" => integer == 1 && digits == 0 ? PluralCategory.One : PluralCategory.Other,
            "ko" => PluralCategory.Other,
            "ru" when digits == 0 && integer % 10 == 1 && integer % 100 != 11 => PluralCategory.One,
            "ru" when digits == 0 && integer % 10 is >= 2 and <= 4 && integer % 100 is not (>= 12 and <= 14) => PluralCategory.Few,
            "ru" when digits == 0 && (integer % 10 == 0 || integer % 10 is >= 5 and <= 9 || integer % 100 is >= 11 and <= 14) => PluralCategory.Many,
            "ar" when number == 0 => PluralCategory.Zero,
            "ar" when number == 1 => PluralCategory.One,
            "ar" when number == 2 => PluralCategory.Two,
            // CLDR '=' ranges contain integers, unlike the 'within' operator.
            "ar" when number == integer && number % 100 is >= 3 and <= 10 => PluralCategory.Few,
            "ar" when number == integer && number % 100 is >= 11 and <= 99 => PluralCategory.Many,
            _ => PluralCategory.Other,
        };
    }

    /// <summary>Exact signed numeric selectors precede cardinal categories. Other is mandatory.</summary>
    public static T SelectMessage<T>(
        string languageCode,
        decimal value,
        IReadOnlyDictionary<decimal, T> exactMatches,
        IReadOnlyDictionary<PluralCategory, T> categories,
        int? visibleFractionDigits = null)
    {
        ArgumentNullException.ThrowIfNull(exactMatches);
        ArgumentNullException.ThrowIfNull(categories);
        var category = Select(languageCode, value, visibleFractionDigits);
        if (!categories.TryGetValue(PluralCategory.Other, out var other))
        {
            throw new ArgumentException("Plural messages require an Other branch.", nameof(categories));
        }
        return exactMatches.TryGetValue(value, out var exact)
            ? exact : categories.TryGetValue(category, out var selected) ? selected : other;
    }

    private static int FractionDigits(decimal value, int? visibleFractionDigits)
    {
        if (visibleFractionDigits is int digits)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(digits);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(digits, 28);
            if (decimal.Round(value, digits) != value)
            {
                throw new ArgumentException("Visible precision must represent the value exactly; round the value explicitly first.", nameof(visibleFractionDigits));
            }
            return digits;
        }
        Span<int> bits = stackalloc int[4];
        decimal.GetBits(value, bits);
        return (bits[3] >> 16) & 0xff;
    }

    private static void ValidateLanguage(string languageCode)
    {
        ArgumentNullException.ThrowIfNull(languageCode);
        if (languageCode is not ("en" or "ko" or "ru" or "ar"))
        {
            throw new ArgumentException("The CLDR 49 subset supports the language codes en, ko, ru and ar. Resolve locale tags before calling it.", nameof(languageCode));
        }
    }
}
