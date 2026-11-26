namespace Doroti.Runtime;

/// <summary>The language, script and region fields shared by host input and internationalization.</summary>
public readonly record struct LocaleTag(string Language, string? Script, string? Region)
{
    /// <summary>Parses the language[-Script][-Region] subset of BCP 47.</summary>
    public static bool TryParse(string? value, out LocaleTag tag)
    {
        tag = default;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var parts = value.Trim().Split('-', '_');
        var next = 1;
        string? script = null;
        if (next < parts.Length && parts[next].Length == 4 && IsLetters(parts[next]))
            script = parts[next++];

        // The old Windows Chinese names encode a script in a three-letter suffix.
        if (script is null && parts.Length == 2 && parts[0].Equals("zh", StringComparison.OrdinalIgnoreCase))
        {
            script = parts[1].ToUpperInvariant() switch { "CHS" => "Hans", "CHT" => "Hant", _ => null };
            if (script is not null) next++;
        }

        var region = next < parts.Length ? parts[next++] : null;
        return next == parts.Length && TryCreate(parts[0], script, region, out tag);
    }

    public static bool TryCreate(string? language, string? script, string? region, out LocaleTag tag)
    {
        tag = default;
        if (language is not { Length: >= 2 and <= 8 } || !IsLetters(language)
            || (script is not null && (script.Length != 4 || !IsLetters(script)))
            || (region is not null && !IsRegion(region)))
        {
            return false;
        }

        tag = new(
            language.ToLowerInvariant() switch
            {
                "in" => "id", "iw" => "he", "ji" => "yi", "jw" => "jv", "mo" => "ro",
                var code => code,
            },
            script is null ? null : char.ToUpperInvariant(script[0]) + script[1..].ToLowerInvariant(),
            region?.ToUpperInvariant() switch
            {
                "BU" => "MM", "DD" => "DE", "FX" => "FR", "TP" => "TL", "YD" => "YE", "ZR" => "CD",
                var code => code,
            }
        );
        return true;
    }

    private static bool IsLetters(string value) => value.All(char.IsAsciiLetter);

    private static bool IsRegion(string value) =>
        (value.Length == 2 && IsLetters(value)) || (value.Length == 3 && value.All(char.IsAsciiDigit));

    public override string ToString() =>
        Language + (Script is null ? "" : "-" + Script) + (Region is null ? "" : "-" + Region);
}
