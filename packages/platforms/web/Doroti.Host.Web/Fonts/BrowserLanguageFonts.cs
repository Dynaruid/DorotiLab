namespace Doroti.Host.Web;

/// <summary>Resolve language hints to complete faces in the pinned fallback catalog.
/// A sample string or the browser locale cannot predict future IME combinations.</summary>
internal static class BrowserLanguageFonts
{
    internal static BrowserFontAsset[] Resolve(BrowserFontFallbackOptions options)
    {
        var families = new HashSet<string>(StringComparer.Ordinal);
        var assets = new List<BrowserFontAsset>();
        foreach (var language in options.PreloadLanguages)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);
            var tag = language.Trim().Replace('_', '-').ToLowerInvariant();
            var parts = tag.Split('-');
            var family = parts[0] switch
            {
                "en" when options.LoadDefaultFontsFromCdn => null, // Already covered by Roboto.
                "ko" => "Noto Sans KR",
                "ja" => "Noto Sans JP",
                "zh" when parts.Contains("hans") => "Noto Sans SC",
                "zh" when parts.Contains("hk") => "Noto Sans HK",
                "zh" when parts.Contains("hant") || parts.Contains("tw") || parts.Contains("mo") => "Noto Sans TC",
                "zh" => "Noto Sans SC",
                "ar" or "fa" or "ur" => "Noto Sans Arabic",
                "he" => "Noto Sans Hebrew",
                "hi" or "mr" or "ne" => "Noto Sans Devanagari",
                "bn" => "Noto Sans Bengali",
                "ta" => "Noto Sans Tamil",
                "te" => "Noto Sans Telugu",
                "kn" => "Noto Sans Kannada",
                "ml" => "Noto Sans Malayalam",
                "gu" => "Noto Sans Gujarati",
                "th" => "Noto Sans Thai",
                "lo" => "Noto Sans Lao",
                "km" => "Noto Sans Khmer",
                "my" => "Noto Sans Myanmar",
                "ka" => "Noto Sans Georgian",
                "hy" => "Noto Sans Armenian",
                "en" or "de" or "fr" or "es" or "it" or "pt" or "nl" or "sv" or "da" or "no"
                    or "fi" or "pl" or "cs" or "sk" or "hu" or "ro" or "tr" or "vi" or "id" or "ms"
                    or "ru" or "uk" or "bg" or "sr" or "el" => "Noto Sans",
                _ => throw new ArgumentException($"Unsupported preload language '{language}'. Supply a complete font in Assets instead.", nameof(options)),
            };
            if (family is null || !families.Add(family)) continue;
            // Exact family selects the monolithic font, never a numbered subset.
            var font = FontFallbackCatalog.Default.Fonts.Single(entry => entry.Family == family);
            assets.Add(new(family, new Uri(options.BaseUrl, font.Path).AbsoluteUri));
        }
        return assets.ToArray();
    }
}
