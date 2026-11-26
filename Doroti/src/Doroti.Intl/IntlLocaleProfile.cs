using Doroti.Runtime;

namespace Doroti.Intl;

internal static class IntlLocaleProfile
{
    internal static string? Resolve(string? value) =>
        LocaleTag.TryParse(value, out var tag) ? Resolve(tag) : null;

    internal static string Resolve(LocaleTag tag) => tag switch
    {
        { Language: "en", Script: null or "Latn", Region: null or "US" } => "en-US",
        { Language: "ko", Script: null or "Kore", Region: null or "KR" } => "ko-KR",
        { Language: "tl", Script: null or "Latn", Region: null or "PH" } => "fil-PH",
        _ => tag.ToString(),
    };
}
