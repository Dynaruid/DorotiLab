using System.Text;

namespace Doroti.Host.Web;

public sealed record FontFallbackEntry(int Index, string Family, string Path)
{
    public bool IsEmoji => Family.StartsWith("Noto Color Emoji", StringComparison.Ordinal);
}

/// <summary>Generated Flutter/Noto coverage, independent of installed system fonts.</summary>
public sealed class FontFallbackCatalog
{
    private readonly FontFallbackEntry[] _fonts;
    private readonly int[][] _components;
    private readonly int[] _ends;
    private readonly int[] _rangeComponents;
    private static readonly Lazy<FontFallbackCatalog> Instance = new(() => new());
    public static FontFallbackCatalog Default => Instance.Value;
    public IReadOnlyList<FontFallbackEntry> Fonts => _fonts;

    private FontFallbackCatalog()
    {
        using var stream = typeof(FontFallbackCatalog).Assembly.GetManifestResourceStream(
            "Doroti.Host.Web.Fonts.fallbacks.bin")!;
        using var reader = new BinaryReader(stream, Encoding.UTF8);
        if (reader.ReadUInt32() != 0x31464644)
            throw new InvalidDataException("Unknown font fallback catalog.");
        string ReadString() => Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));
        _fonts = new FontFallbackEntry[reader.ReadInt32()];
        for (var i = 0; i < _fonts.Length; i++)
            _fonts[i] = new(i, ReadString(), ReadString());
        _components = new int[reader.ReadInt32()][];
        for (var i = 0; i < _components.Length; i++)
        {
            _components[i] = new int[reader.ReadInt32()];
            for (var j = 0; j < _components[i].Length; j++)
                _components[i][j] = reader.ReadInt32();
        }
        _ends = new int[reader.ReadInt32()];
        _rangeComponents = new int[_ends.Length];
        for (var i = 0; i < _ends.Length; i++)
        {
            _ends[i] = reader.ReadInt32();
            _rangeComponents[i] = reader.ReadInt32();
        }
    }

    public IReadOnlyList<int> Candidates(int codePoint)
    {
        if ((uint)codePoint > 0x10ffff)
            return [];
        var index = Array.BinarySearch(_ends, codePoint + 1);
        if (index < 0) index = ~index;
        return _components[_rangeComponents[index]];
    }

    internal FontFallbackEntry Select(Dictionary<int, int> coverage, string language)
    {
        var tag = language.ToLowerInvariant();
        string[] preferred = tag switch
        {
            var s when s.StartsWith("zh-hk") => ["Noto Sans HK", "Noto Sans TC"],
            var s when s.StartsWith("zh-hant") || s.StartsWith("zh-tw") || s.StartsWith("zh-mo") => ["Noto Sans TC"],
            var s when s.StartsWith("zh") => ["Noto Sans SC"],
            var s when s is "ja" || s.StartsWith("ja-") => ["Noto Sans JP"],
            var s when s is "ko" || s.StartsWith("ko-") => ["Noto Sans KR"],
            _ => [],
        };
        foreach (var prefix in preferred)
        {
            var matches = coverage.Keys.Where(i => _fonts[i].Family.StartsWith(prefix, StringComparison.Ordinal)).ToArray();
            if (matches.Length > 0)
                return _fonts[matches.OrderByDescending(i => coverage[i]).ThenBy(i => i).First()];
        }
        var maximum = coverage.Values.Max();
        var best = coverage.Keys.Where(i => coverage[i] == maximum).OrderBy(i => i).ToArray();
        foreach (var prefix in new[] { "Noto Color Emoji", "Noto Sans Symbols", "Noto Sans SC",
            "Noto Sans TC", "Noto Sans HK", "Noto Sans JP", "Noto Sans KR" })
            foreach (var i in best)
                if (_fonts[i].Family.StartsWith(prefix, StringComparison.Ordinal))
                    return _fonts[i];
        return _fonts[best[0]];
    }
}
