using System.Text;
using System.Collections.Concurrent;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace Doroti.Vendor.Avalonia.Skia;

internal static class FontFallbackTextRenderer
{
    private static readonly ConcurrentDictionary<(string RequestedFamily, int Scalar), string> FamilyByScalar = new();
    private static readonly ConcurrentDictionary<string, SKTypeface> TypefacesByFamily =
        new(StringComparer.OrdinalIgnoreCase);

    internal static void Draw(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        float fontSize,
        SKPaint paint,
        string? fontFamily = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(text);

        var cursor = x;
        foreach (var run in ResolveRuns(text, fontFamily))
        {
            var typeface = GetTypeface(run.FamilyName);
            using var font = new SKFont(typeface, fontSize);
            canvas.DrawShapedText(run.Text, cursor, y, SKTextAlign.Left, font, paint);
            cursor += font.MeasureText(run.Text);
        }
    }

    internal static (float Width, float Height, float Baseline, IReadOnlyList<string> Families) Measure(
        string text,
        float fontSize,
        string? fontFamily = null)
    {
        var width = 0f;
        var height = 0f;
        var baseline = 0f;
        var families = new List<string>();
        foreach (var run in ResolveRuns(text, fontFamily))
        {
            var typeface = GetTypeface(run.FamilyName);
            using var font = new SKFont(typeface, fontSize);
            width += font.MeasureText(run.Text);
            var metrics = font.Metrics;
            height = Math.Max(height, metrics.Descent - metrics.Ascent);
            baseline = Math.Max(baseline, -metrics.Ascent);
            if (!families.Contains(typeface.FamilyName, StringComparer.OrdinalIgnoreCase))
            {
                families.Add(typeface.FamilyName);
            }
        }
        return (width, height, baseline, families);
    }

    private static IReadOnlyList<FontRun> ResolveRuns(string text, string? requestedFamily)
    {
        if (text.Length == 0) return [];

        var runs = new List<FontRun>();
        var buffer = new StringBuilder();
        string? family = null;
        foreach (var rune in text.EnumerateRunes())
        {
            var nextFamily = FamilyByScalar.GetOrAdd(
                (requestedFamily ?? string.Empty, rune.Value),
                static key => ResolveFamily(key.RequestedFamily, key.Scalar));
            if (family is not null && !string.Equals(family, nextFamily, StringComparison.OrdinalIgnoreCase))
            {
                runs.Add(new(buffer.ToString(), family));
                buffer.Clear();
            }
            family = nextFamily;
            buffer.Append(rune.ToString());
        }
        if (buffer.Length > 0)
        {
            runs.Add(new(buffer.ToString(), family ?? SKTypeface.Default.FamilyName));
        }
        return runs;
    }

    private static string ResolveFamily(string requestedFamily, int scalar)
    {
        if (requestedFamily.Length > 0)
        {
            var requested = GetTypeface(requestedFamily);
            if (requested.ContainsGlyph(scalar))
            {
                return requestedFamily;
            }
        }

        var match = SKFontManager.Default.MatchCharacter(scalar);
        if (match is null)
        {
            return SKTypeface.Default.FamilyName;
        }
        using (match)
        {
            return match.FamilyName;
        }
    }

    private static SKTypeface GetTypeface(string family) => TypefacesByFamily.GetOrAdd(family, static requested =>
    {
        var resourceName = requested switch
        {
            "MaterialIcons" => "Doroti.Vendor.Avalonia.Skia.Assets.MaterialIcons-Regular.otf",
            "Roboto" => "Doroti.Vendor.Avalonia.Skia.Assets.Roboto-Regular.ttf",
            _ => null,
        };
        if (resourceName is not null)
        {
            using var stream = typeof(FontFallbackTextRenderer).Assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidDataException($"Bundled Flutter font is missing: {resourceName}");
            return SKTypeface.FromStream(stream)
                ?? throw new InvalidDataException($"Bundled Flutter font is invalid: {resourceName}");
        }
        return SKTypeface.FromFamilyName(requested) ?? SKTypeface.Default;
    });

    private sealed record FontRun(string Text, string FamilyName);
}
