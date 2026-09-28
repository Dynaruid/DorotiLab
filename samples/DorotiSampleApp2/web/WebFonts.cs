using Doroti.Host.Web;

namespace DorotiSampleApp2.Web;

// Selected by -p:DorotiSampleWebFontSource=Assets. The normal sample uses CDN fonts.
public static class WebFonts
{
#if DOROTI_FONT_PROBE
    static WebFonts() { if (OperatingSystem.IsBrowser()) App.ValidateFontsAsync = WebFontProbe.RunAsync; }
#endif
    public static BrowserFontFallbackOptions CdnOptions => new()
    {
        PreloadLanguages = ["ko", "en"],
#if DOROTI_LOCAL_FONT_DECODER
        DecoderUrl = LocalDecoder,
#endif
    };

    public static BrowserFontFallbackOptions Options => BrowserFontFallbackOptions.AssetsOnly(
        "SampleSans",
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-regular.ttf"),
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-medium.ttf"),
        BrowserFontAsset.Embedded("SampleSans", typeof(WebFonts).Assembly, "AssetFonts.Roboto-bold.ttf")) with
    {
        CssStylesheets = ["fonts/SUITE/SUITE-Variable.css", "fonts/Galmuri/galmuri-local.css"],
        DecoderUrl = LocalDecoder,
    };

    private static Uri LocalDecoder => new("fonts/decoder/decompress.js", UriKind.Relative);
}
