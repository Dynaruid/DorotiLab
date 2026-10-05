namespace Doroti.Host.Web;

public static class BrowserDefaultFonts
{
    /// <summary>Versioned Google Fonts TTF faces, ordered 400 / 500 / 700.</summary>
    public static IReadOnlyList<string> Paths { get; } = Array.AsReadOnly(new[]
    {
        "roboto/v51/KFOMCnqEu92Fr1ME7kSn66aGLdTylUAMQXC89YmC2DPNWubEbWmT.ttf",
        "roboto/v51/KFOMCnqEu92Fr1ME7kSn66aGLdTylUAMQXC89YmC2DPNWub2bWmT.ttf",
        "roboto/v51/KFOMCnqEu92Fr1ME7kSn66aGLdTylUAMQXC89YmC2DPNWuYjammT.ttf",
    });
}
