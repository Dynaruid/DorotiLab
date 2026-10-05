using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.RegularExpressions;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Web;

public sealed record BrowserCssFontFace(string Family, string Stylesheet, IReadOnlyList<string> Sources, SkiaFontFaceDescriptor Descriptor);

public static partial class BrowserCssFonts
{
    [SupportedOSPlatform("browser")]
    public static async Task<IReadOnlyList<BrowserCssFontFace>> DiscoverAsync(BrowserFontFallbackOptions options)
    {
        if (!options.DiscoverCssFonts && options.CssStylesheets.Count == 0) return [];
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            writer.WriteStartObject();
            writer.WriteBoolean("discover", options.DiscoverCssFonts);
            writer.WriteBoolean("sameOriginOnly", options.CssFontsSameOriginOnly);
            writer.WriteStartArray("stylesheets");
            foreach (var url in options.CssStylesheets) writer.WriteStringValue(url);
            writer.WriteEndArray(); writer.WriteEndObject();
        }
        return Parse(await Discover(System.Text.Encoding.UTF8.GetString(buffer.ToArray())));
    }

    [SupportedOSPlatform("browser")]
    [JSImport("discoverCssFonts", "doroti.web")]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    private static partial Task<string> Discover(string optionsJson);

    public static IReadOnlyList<BrowserCssFontFace> Parse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var faces = new List<BrowserCssFontFace>();
        foreach (var face in doc.RootElement.EnumerateArray())
        {
            string Read(string name) => face.GetProperty(name).GetString() ?? "";
            try
            {
                var weight = Read("weight").Replace("normal", "400").Replace("bold", "700").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var min = int.Parse(weight[0], CultureInfo.InvariantCulture);
                var max = weight.Length > 1 ? int.Parse(weight[1], CultureInfo.InvariantCulture) : min;
                if (min < 1 || max > 1000 || min > max) throw new FormatException("Invalid weight range");
                var stretch = Read("stretch");
                string[] widths = ["ultra-condensed", "extra-condensed", "condensed", "semi-condensed", "normal", "semi-expanded", "expanded", "extra-expanded", "ultra-expanded"];
                var width = Array.IndexOf(widths, stretch) + 1;
                if (width == 0 && double.TryParse(stretch.TrimEnd('%'), CultureInfo.InvariantCulture, out var percent))
                {
                    double[] values = [50, 62.5, 75, 87.5, 100, 112.5, 125, 150, 200];
                    width = Enumerable.Range(0, values.Length).MinBy(i => Math.Abs(values[i] - percent)) + 1;
                }
                if (width == 0) throw new FormatException($"Unsupported stretch '{stretch}'");
                var ranges = new List<(int, int)>();
                foreach (var range in Read("unicodeRange").Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = range.Trim().Replace("U+", "", StringComparison.OrdinalIgnoreCase).Split('-');
                    var start = Convert.ToInt32(parts[0].Replace('?', '0'), 16);
                    var end = Convert.ToInt32(parts.Length == 2 ? parts[1] : parts[0].Replace('?', 'F'), 16);
                    if (start > end || end > 0x10ffff) throw new FormatException("Invalid unicode-range");
                    ranges.Add((start, end));
                }
                var variations = new List<FontVariation>();
                foreach (Match m in Regex.Matches(Read("variationSettings"), "[\"'](?<axis>.{4})[\"']\\s+(?<value>[-+0-9.eE]+)"))
                    variations.Add(new(m.Groups["axis"].Value, double.Parse(m.Groups["value"].Value, CultureInfo.InvariantCulture)));
                faces.Add(new(Read("family"), Read("stylesheet"), face.GetProperty("sources").EnumerateArray().Select(v => v.GetString()!).ToArray(), new()
                {
                    WeightMin = min, WeightMax = max, Width = width, UnicodeRanges = ranges, Variations = variations,
                    Slant = Read("style").StartsWith("italic") ? SKFontStyleSlant.Italic : Read("style").StartsWith("oblique") ? SKFontStyleSlant.Oblique : SKFontStyleSlant.Upright,
                }));
            }
            catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
            {
                Console.WriteLine($"Doroti CSS fonts ({Read("stylesheet")}, {Read("family")}): {error.Message}");
            }
        }
        return faces;
    }
}
