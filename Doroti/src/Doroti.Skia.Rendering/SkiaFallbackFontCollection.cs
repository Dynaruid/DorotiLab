using System.Text;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>
/// Owns application- or host-supplied typefaces that are not discoverable
/// through the platform font manager. Browser WebAssembly uses this because
/// CSS/system fonts are not exposed to Skia as font data.
/// </summary>
public sealed class SkiaFallbackFontCollection(string? defaultFamily = null) : IDisposable
{
    private readonly List<RegisteredFont> _fonts = [];
    private bool _disposed;

    /// <summary>Host default, used when requested families cannot be resolved.</summary>
    public string? DefaultFamily { get; } = defaultFamily;
    public event Action<int>? CharacterMissing;
    public event Action? Changed;
    internal void ReportMissingCharacter(int codePoint) => CharacterMissing?.Invoke(codePoint);

    public IReadOnlyList<string> Families =>
        _fonts.Select(font => font.Typeface.FamilyName).ToArray();

    public string Register(ReadOnlyMemory<byte> bytes, string? family = null, bool preferForFallback = false, SkiaFontFaceDescriptor? descriptor = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (bytes.IsEmpty)
        {
            throw new ArgumentException("Font data cannot be empty.", nameof(bytes));
        }

        using var data = SKData.CreateCopy(bytes.ToArray());
        var typeface =
            SKTypeface.FromData(data)
            ?? throw new InvalidDataException("Skia could not decode the supplied fallback font.");
        if (typeface.GlyphCount == 0)
        {
            typeface.Dispose();
            throw new InvalidDataException("The supplied font contains no usable glyphs.");
        }
        var font = new RegisteredFont(typeface, family, descriptor);
        if (preferForFallback)
            _fonts.Insert(0, font);
        else
            _fonts.Add(font);
        Changed?.Invoke();
        return typeface.FamilyName;
    }

    public bool ContainsCharacter(int codePoint)
    {
        if (!Rune.IsValid(codePoint))
        {
            throw new ArgumentOutOfRangeException(nameof(codePoint));
        }

        return MatchCharacter(codePoint) is not null;
    }

    internal SKTypeface? MatchFamily(string? family, SKFontStyle? style = null, int? codePoint = null) =>
        string.IsNullOrWhiteSpace(family) ? null : _fonts.AsEnumerable().Reverse()
            .Where(font => (string.Equals(font.Alias, family, StringComparison.OrdinalIgnoreCase)
                || string.Equals(font.Typeface.FamilyName, family, StringComparison.OrdinalIgnoreCase))
                && (codePoint is null || font.Covers(codePoint.Value)))
            .OrderBy(font => font.Distance(style)).FirstOrDefault()?.Typeface;

    internal bool Covers(SKTypeface face, int codePoint) =>
        _fonts.FirstOrDefault(f => f.Typeface == face)?.Covers(codePoint) ?? true;

    internal SkiaFontFaceDescriptor? Descriptor(SKTypeface face) =>
        _fonts.FirstOrDefault(f => f.Typeface == face)?.Descriptor;

    internal IReadOnlyList<Doroti.Ui.FontVariation> Variations(SKTypeface face) =>
        _fonts.FirstOrDefault(f => f.Typeface == face)?.Descriptor?.Variations ?? [];

    internal SKTypeface? MatchCharacter(int codePoint, SKFontStyle? style = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        foreach (var family in _fonts.OrderBy(font => font.Descriptor is null ? 0 : 1).GroupBy(font => font.Alias ?? font.Typeface.FamilyName))
        {
            var match = family.Where(font => font.Covers(codePoint)).OrderBy(font => font.Distance(style)).FirstOrDefault();
            if (match is not null) return match.Typeface;
        }
        return null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        foreach (var font in _fonts)
        {
            font.Dispose();
        }

        _fonts.Clear();
    }

    private sealed class RegisteredFont(SKTypeface typeface, string? alias, SkiaFontFaceDescriptor? descriptor) : IDisposable
    {
        internal SkiaFontFaceDescriptor? Descriptor { get; } = descriptor;
        internal bool Covers(int codePoint) => (Descriptor?.Covers(codePoint) ?? true) && Probe.ContainsGlyph(codePoint);
        internal int Distance(SKFontStyle? style)
        {
            var requested = style?.Weight ?? 400;
            var axes = Typeface.VariationDesignParameters;
            var weightAxis = axes.FirstOrDefault(a => a.Tag.ToString() == "wght");
            var min = Descriptor?.WeightMin ?? (weightAxis.Max > 0 ? (int)weightAxis.Min : Typeface.FontWeight);
            var max = Descriptor?.WeightMax ?? (weightAxis.Max > 0 ? (int)weightAxis.Max : Typeface.FontWeight);
            return Math.Abs(requested - Math.Clamp(requested, min, max))
                + Math.Abs((Descriptor?.Width ?? Typeface.FontWidth) - (style?.Width ?? 5)) * 10000
                + ((Descriptor?.Slant ?? Typeface.FontSlant) == (style?.Slant ?? SKFontStyleSlant.Upright) ? 0 : 1000);
        }
        internal string? Alias { get; } = alias;
        internal SKTypeface Typeface { get; } = typeface;
        internal SKFont Probe { get; } = new(typeface, 16);

        public void Dispose()
        {
            Probe.Dispose();
            Typeface.Dispose();
        }
    }
}
