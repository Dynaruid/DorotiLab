using System.Net;
using Doroti.Host.Web;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

var baseUri = new Uri("https://app.invalid/nested/");
var bytes = File.ReadAllBytes("samples/DorotiTestbedApp/assets/fonts/Roboto-regular.ttf");
var handler = new AssetHandler(baseUri, bytes);
using var http = new HttpClient(handler);
var options = BrowserFontFallbackOptions.AssetsOnly("LocalSans",
    new BrowserFontAsset("LocalSans", "fonts/regular.ttf"),
    BrowserFontAsset.Embedded("LocalSans", typeof(AssetHandler).Assembly, "TestFont.ttf"));
using var fonts = new SkiaFallbackFontCollection(options.DefaultFamily);
var aliases = new List<string>();
await BrowserStartupFonts.LoadAsync(options, http, baseUri, (data, family) =>
{
    aliases.Add(family);
    fonts.Register(data, family);
}, (data, url, ct) => throw new Exception("TTF asset mode must never request a decoder"));
Check(handler.Requests.SequenceEqual(new[] { "https://app.invalid/nested/fonts/regular.ttf" }),
    "asset mode requests only the app-relative font, no default CDN URLs");
Check(aliases.SequenceEqual(new[] { "LocalSans", "LocalSans" }), "URL and embedded fonts retain registration aliases/order");
Check(!options.Enabled && !options.LoadDefaultFontsFromCdn && options.DecoderUrl is null,
    "AssetsOnly disables all implicit font/decoder CDNs");
using var renderer = new SkiaSceneRenderer(1, new Host(), null, null, "asset", "asset", "asset", fallbackFonts: fonts);
var paragraph = renderer.Layout(new ParagraphRequest("iiii WWWW asset", 500, null, 24), new("asset"));
using var data = SKData.CreateCopy(bytes);
using var face = SKTypeface.FromData(data);
using var font = new SKFont(face, 24);
Check(Math.Abs(paragraph.maxIntrinsicWidth - font.GetGlyphWidths("iiii WWWW asset").Sum()) < .01,
    "unspecified family uses the configured asset default and its real measurements");

handler.Requests.Clear();
await BrowserStartupFonts.LoadAsync(BrowserFontFallbackOptions.AssetsOnly("LocalSans",
    BrowserFontAsset.Embedded("LocalSans", typeof(AssetHandler).Assembly, "TestFont.ttf")),
    http, baseUri, (_, _) => { });
Check(handler.Requests.Count == 0, "embedded-only startup performs zero HTTP requests");
await Reject(BrowserFontFallbackOptions.AssetsOnly("Missing"), "no asset for default family");
await Reject(BrowserFontFallbackOptions.AssetsOnly("LocalSans",
    BrowserFontAsset.Embedded("LocalSans", typeof(AssetHandler).Assembly, "missing.ttf")), "missing resource");
await Reject(BrowserFontFallbackOptions.AssetsOnly("LocalSans",
    new BrowserFontAsset("LocalSans", "fonts/missing.ttf")), "missing URL");
await Reject(BrowserFontFallbackOptions.AssetsOnly("LocalSans",
    new BrowserFontAsset("LocalSans", "fonts/font.woff2")), "WOFF2 does not silently import a decoder CDN");
Check(handler.Requests.All(url => url.StartsWith(baseUri.AbsoluteUri, StringComparison.Ordinal)),
    "asset failures do not fall back to external URLs");

var order = new List<string>();
var combined = new BrowserFontFallbackOptions
{
    BaseUrl = new Uri(baseUri, "cdn-mirror/"),
    Assets = [new BrowserFontAsset("Additional", "fonts/regular.ttf")],
};
await BrowserStartupFonts.LoadAsync(combined, http, baseUri, (_, family) => order.Add(family));
Check(order.SequenceEqual(new[] { "Roboto", "Roboto", "Roboto", "Additional" }),
    "default CDN mode can add assets without changing registration priority");
var cancellations = new CancellationTokenSource();
cancellations.Cancel();
try
{
    await BrowserStartupFonts.LoadAsync(options, http, baseUri,
        (_, _) => throw new Exception("cancelled fonts must not register"), cancellationToken: cancellations.Token);
    throw new Exception("cancelled startup succeeded");
}
catch (OperationCanceledException) { Console.WriteLine("PASS startup cancellation"); }
Console.WriteLine("PASS asset-only startup, embedded resources, default family, CDN coexistence and failure isolation");

async Task Reject(BrowserFontFallbackOptions invalid, string name)
{
    try
    {
        await BrowserStartupFonts.LoadAsync(invalid, http, baseUri,
            (_, _) => throw new Exception("invalid font registered"));
    }
    catch (Exception error) when (error is ArgumentException or InvalidDataException or HttpRequestException)
    {
        Console.WriteLine($"PASS {name}");
        return;
    }
    throw new Exception($"Expected startup failure: {name}");
}
static void Check(bool value, string message)
{
    if (!value) throw new Exception(message);
    Console.WriteLine($"PASS {message}");
}
sealed class AssetHandler(Uri baseUri, byte[] font) : HttpMessageHandler
{
    public List<string> Requests { get; } = [];
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var url = request.RequestUri!.AbsoluteUri;
        Requests.Add(url);
        if (!url.StartsWith(baseUri.AbsoluteUri, StringComparison.Ordinal))
            throw new Exception($"Unexpected external request: {url}");
        if (url.EndsWith("missing.ttf", StringComparison.Ordinal))
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        var content = url.EndsWith(".woff2", StringComparison.Ordinal) ? "wOF2-invalid-fixture"u8.ToArray() : font;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(content) });
    }
}
sealed class Host : ISkiaSceneRendererHost
{
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 500, 100, 500, 100, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 500, 100, 500, 100, 1, 1);
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
    public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public void UpdateSemantics(SemanticsUpdate update) { }
    public void ClearSemantics() { }
    public void RequestInvalidate() { }
}
