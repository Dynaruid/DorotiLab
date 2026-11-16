using Doroti.Host.Web;

namespace DorotiTestbedApp.Web;

public static class WebFonts
{
    public static BrowserFontFallbackOptions Options => new()
    {
        PreloadLanguages = ["ko", "en"],
    };
}
