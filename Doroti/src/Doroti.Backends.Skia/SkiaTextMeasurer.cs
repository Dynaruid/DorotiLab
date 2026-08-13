using SkiaSharp;
using Doroti.Vendor.Avalonia.Skia;

namespace Doroti.Backends.Skia;

public readonly record struct SkiaTextMetrics(double Width, double Height, double Baseline);

/// <summary>Measures the same font fallback input consumed by the HarfBuzz-backed raster canvases.</summary>
public static class SkiaTextMeasurer
{
    public static SkiaTextMetrics Measure(string text, string? fontFamily, double fontSize, double maximumWidth)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);
        if (!double.IsFinite(maximumWidth) || maximumWidth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumWidth));
        }

        var measured = FontFallbackTextRenderer.Measure(text, (float)fontSize, fontFamily);
        var width = maximumWidth == 0 ? measured.Width : Math.Min(measured.Width, maximumWidth);
        return new(width, measured.Height, measured.Baseline);
    }

    public static IReadOnlyList<string> ResolveFallbackFamilies(
        string text,
        double fontSize = 14,
        string? fontFamily = null) =>
        FontFallbackTextRenderer.Measure(text, (float)fontSize, fontFamily).Families;
}
