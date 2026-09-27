using Doroti.Host.Web;

namespace DorotiSampleApp2.Web;

// Selected by -p:DorotiSampleWebFontSource=Assets. The normal sample uses CDN fonts.
public static class WebFonts
{
    public static BrowserFontFallbackOptions CdnOptions => new()
    {
        PreloadLanguages = ["ko", "en"],
    };

    public static BrowserFontFallbackOptions Options => BrowserFontFallbackOptions.AssetsOnly(
        "SampleSans",
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-regular.ttf"),
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-medium.ttf"),
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-bold.ttf"));
}
