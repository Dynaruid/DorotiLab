// Cardinal rules derived from Unicode CLDR 49; Copyright Unicode, Inc.
// SPDX-License-Identifier: Unicode-3.0. See data/plural-provenance.json and LICENSES/Unicode-3.0.txt.
using Doroti.Runtime;

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
/// CLDR 49 cardinal categories for plain decimal operands. Decimal scale preserves visible trailing zeros.
/// </summary>
public static class CardinalPlural
{
    public const string CldrVersion = "49";

    public static bool SupportsLocale(string? languageTag) => RuleGroup(languageTag) >= 0;

    /// <param name="visibleFractionDigits">
    /// Null preserves decimal scale. An explicit value (0..28) may pad/remove trailing zeros but
    /// must represent the value exactly. Round the input explicitly before choosing a category.
    /// </param>
    public static PluralCategory Select(string languageCode, decimal value, int? visibleFractionDigits = null)
    {
        ArgumentNullException.ThrowIfNull(languageCode);
        var group = RuleGroup(languageCode);
        if (group < 0)
            throw new ArgumentException("No CLDR 49 cardinal rule matches the language tag.", nameof(languageCode));
        var digits = FractionDigits(value, visibleFractionDigits);
        return CardinalPluralRules.Select(group, new(decimal.Abs(value), digits));
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

    private static int RuleGroup(string? tag)
    {
        if (tag is { Length: 2 or 3 })
        {
            var known = CardinalPluralRules.Group(new(tag, null, null));
            if (known >= 0) return known;
        }
        return LocaleTag.TryParse(tag, out var locale) ? CardinalPluralRules.Group(locale) : -1;
    }

}

internal readonly record struct CardinalOperands(decimal N, int V)
{
    private static readonly decimal[] PowersOfTen = CreatePowers();
    public decimal I => decimal.Truncate(N);
    public bool NIsInteger => I == N;
    public decimal F => decimal.Truncate((N % 1) * PowersOfTen[V]);
    public decimal T
    {
        get
        {
            var result = F;
            while (result != 0 && result % 10 == 0) result /= 10;
            return result;
        }
    }

    private static decimal[] CreatePowers()
    {
        var powers = new decimal[29];
        powers[0] = 1;
        for (var i = 1; i < powers.Length; i++) powers[i] = 10 * powers[i - 1];
        return powers;
    }
}
