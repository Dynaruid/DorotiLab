using Doroti.Host.Web;

namespace DorotiDemoApp.Web;

public static class WebFonts
{
    public static BrowserFontFallbackOptions Options => new()
    {
        PreloadLanguages = ["ko", "en"],
    };
}
