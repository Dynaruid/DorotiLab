using Doroti.Host.Web;
namespace DorotiTemplateApp.Web;

/// <summary>Selectable self-contained Latin/Korean preset. CDN remains the default.</summary>
public static class WebFonts
{
    public static BrowserFontFallbackOptions Offline => BrowserFontFallbackOptions.AssetsOnly(
        "TemplateSans", new BrowserFontAsset("TemplateSans", "fonts/offline/SUITE-Variable.woff2")) with
    {
        DecoderUrl = new Uri("fonts/decoder/decompress.js", UriKind.Relative),
    };
}
