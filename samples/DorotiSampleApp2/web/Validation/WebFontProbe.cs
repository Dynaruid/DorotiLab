using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doroti.Host.Web;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace DorotiSampleApp2.Web;

// Opt-in qualification runs on the actual render owner during application startup.
[SupportedOSPlatform("browser")]
public static class WebFontProbe
{
    public static async Task RunAsync()
    {
        using var http = new HttpClient();
        var baseUri = new Uri(BrowserHostRuntime.ResolveResourceUrl("./"));
        using var output = new MemoryStream();
        using (var json = new Utf8JsonWriter(output))
        {
            json.WriteStartObject(); json.WriteBoolean("isBrowser", OperatingSystem.IsBrowser());
            json.WriteStartObject("directFormats");
            byte[] compressed = [];
            foreach (var (format, url) in new[] { ("TTF", "font-probe/Roboto-regular.ttf"), ("OTF", "font-probe/icons.otf"), ("WOFF2", "fonts/SUITE/SUITE-Variable.woff2") })
            {
                var bytes = await http.GetByteArrayAsync(new Uri(baseUri, url));
                using var data = SKData.CreateCopy(bytes); using var face = SKTypeface.FromData(data);
                json.WriteStartObject(format); json.WriteNumber("bytes", bytes.Length);
                json.WriteNumber("glyphs", face?.GlyphCount ?? 0); json.WriteBoolean("supported", face is { GlyphCount: > 0 }); json.WriteEndObject();
                if (format == "WOFF2") compressed = bytes;
            }
            json.WriteEndObject();
            byte[] decoded = [];
            await BrowserStartupFonts.LoadBrowserAsync(BrowserFontFallbackOptions.AssetsOnly("Suite",
                new BrowserFontAsset("Suite", "fonts/SUITE/SUITE-Variable.woff2")) with { DecoderUrl = new Uri(baseUri, "fonts/decoder/decompress.js") },
                http, baseUri, (bytes, _) => decoded = bytes.ToArray());
            json.WriteNumber("registeredBytes", decoded.Length);
            json.WriteBoolean("directWoff2", BrowserFontData.CanUseWoff2Directly(compressed));
            using var variableData = SKData.CreateCopy(decoded); using var variable = SKTypeface.FromData(variableData)!;
            using (var korean = new SKFont(variable, 32))
                json.WriteBoolean("koreanGlyphs", korean.GetGlyphs("한글 가나다라마바사").All(glyph => glyph != 0));
            json.WriteStartArray("axes");
            foreach (var value in new[] { 300, 400, 450, 500, 700, 900 })
            {
                using var clone = SkiaFontVariations.Clone(variable, 400, [new FontVariation("wght", value)], out var variableWeight)!;
                using var font = new SKFont(clone, 32); using var path = font.GetGlyphPath(font.GetGlyphs("A")[0]);
                json.WriteStartObject(); json.WriteNumber("requested", value);
                json.WriteNumber("actual", clone.VariationDesignPosition.Single(c => c.Axis.ToString() == "wght").Value);
                json.WriteBoolean("variableWeight", variableWeight);
                json.WriteString("outline", Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(path.ToSvgPathData())))); json.WriteEndObject();
            }
            json.WriteEndArray(); json.WriteEndObject();
        }
        Console.WriteLine("DOROTI_FONT_PROBE:" + Encoding.UTF8.GetString(output.ToArray()));
    }
}
