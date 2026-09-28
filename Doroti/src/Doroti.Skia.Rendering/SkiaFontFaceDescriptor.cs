using System.Globalization;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>CSS aliases/descriptors are independent of the font's internal names.</summary>
public sealed record SkiaFontFaceDescriptor
{
    public int WeightMin { get; init; } = 400;
    public int WeightMax { get; init; } = 400;
    public int Width { get; init; } = 5;
    public SKFontStyleSlant Slant { get; init; }
    public IReadOnlyList<(int Start, int End)> UnicodeRanges { get; init; } = [];
    public IReadOnlyList<FontVariation> Variations { get; init; } = [];
    internal bool Covers(int codePoint) => UnicodeRanges.Count == 0 || UnicodeRanges.Any(r => codePoint >= r.Start && codePoint <= r.End);
}

/// <summary>Normalized axes are shared by measuring and drawing. Last explicit axis wins.</summary>
public static class SkiaFontVariations
{
    public static string Key(IReadOnlyList<FontVariation>? variations) => string.Join(";", Normalize(variations)
        .Select(v => v.axis + "=" + v.value.ToString("R", CultureInfo.InvariantCulture)));

    public static FontVariation[] Normalize(IReadOnlyList<FontVariation>? variations) => (variations ?? [])
        .Where(v => v.axis.Length == 4 && v.axis.All(c => c is >= ' ' and <= '~') && double.IsFinite(v.value))
        .GroupBy(v => v.axis, StringComparer.Ordinal).Select(g => g.Last()).OrderBy(v => v.axis, StringComparer.Ordinal).ToArray();

    public static SKTypeface? Clone(SKTypeface face, int weight, IReadOnlyList<FontVariation>? variations,
        out bool variableWeight)
    {
        var axes = face.VariationDesignParameters;
        variableWeight = axes.Any(a => a.Tag.ToString() == "wght");
        if (axes.Length == 0) return null;
        var requested = Normalize(variations).ToDictionary(v => v.axis, v => v.value, StringComparer.Ordinal);
        var coordinates = axes.Select(a => new SKFontVariationPositionCoordinate
        {
            Axis = a.Tag,
            Value = (float)Math.Clamp(requested.GetValueOrDefault(a.Tag.ToString(), a.Tag.ToString() == "wght" ? weight : a.Default), a.Min, a.Max),
        }).ToArray();
        return face.Clone(coordinates) ?? throw new InvalidDataException("Skia could not instantiate the requested font axes.");
    }
}
