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

    public string Register(ReadOnlyMemory<byte> bytes, string? family = null, bool preferForFallback = false)
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
        var font = new RegisteredFont(typeface, family);
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

    internal SKTypeface? MatchFamily(string? family, SKFontStyle? style = null) =>
        string.IsNullOrWhiteSpace(family)
            ? null
            : _fonts
                .AsEnumerable()
                .Reverse()
                .Where(font =>
                    string.Equals(
                        font.Alias,
                        family,
                        StringComparison.OrdinalIgnoreCase
                    )
                    || string.Equals(font.Typeface.FamilyName, family, StringComparison.OrdinalIgnoreCase)
                )
                .OrderBy(font =>
                    Math.Abs(font.Typeface.FontWeight - (style?.Weight ?? 400))
                    + (
                        font.Typeface.FontSlant == (style?.Slant ?? SKFontStyleSlant.Upright)
                            ? 0
                            : 1000
                    )
                )
                .FirstOrDefault()
                ?.Typeface;

    internal SKTypeface? MatchCharacter(int codePoint, SKFontStyle? style = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        // Preserve family priority, but choose the closest face within that family.
        foreach (var family in _fonts.GroupBy(font => font.Typeface.FamilyName))
        {
            var match = family.Where(font => font.Probe.ContainsGlyph(codePoint))
                .OrderBy(font => Math.Abs(font.Typeface.FontWeight - (style?.Weight ?? 400))
                    + (font.Typeface.FontSlant == (style?.Slant ?? SKFontStyleSlant.Upright) ? 0 : 1000))
                .FirstOrDefault();
            if (match is not null)
                return match.Typeface;
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

    private sealed class RegisteredFont(SKTypeface typeface, string? alias) : IDisposable
    {
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
