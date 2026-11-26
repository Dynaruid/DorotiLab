using System.IO.Compression;
using Doroti.Skia.Rendering.Unicode;

namespace Doroti.Skia.Rendering;

internal sealed class WordBreakDictionary(string resource)
{
    private static readonly WordBreakDictionary Thai = new("thaidict");
    private static readonly WordBreakDictionary Lao = new("laodict");
    private static readonly WordBreakDictionary Khmer = new("khmerdict");
    private static readonly WordBreakDictionary Burmese = new("burmesedict");
    private readonly Lazy<DictionaryData> _data = new(() => Load(resource));

    internal static WordBreakDictionary? ForScript(Script script) => script switch
    {
        Script.Thai => Thai,
        Script.Lao => Lao,
        Script.Khmer => Khmer,
        Script.Myanmar => Burmese,
        _ => null,
    };

    internal List<int> FindBreaks(string text, ReadOnlySpan<ParagraphTextAnalysis.Cluster> clusters)
    {
        var data = _data.Value;
        var words = data.Words.GetAlternateLookup<ReadOnlySpan<char>>();
        var count = clusters.Length;
        var best = new Path[count + 1];
        for (var index = count - 1; index >= 0; index--)
        {
            var start = clusters[index].Start;
            // Unknown graphemes join the following unknown piece so an
            // unrecognized word does not acquire arbitrary word boundaries.
            var next = best[index + 1];
            var winning = next.IsWord
                ? new Path(index + 1, next.UnknownCount + 1, next.Pieces + 1, IsWord: false)
                : next with { UnknownCount = next.UnknownCount + 1 };

            if (winning.End == 0) winning = winning with { End = count, Pieces = 1 };
            for (var end = index + 1; end <= count; end++)
            {
                var length = clusters[end - 1].End - start;
                if (length > data.MaxLength) break;
                var rest = best[end];
                if (rest.UnknownCount > winning.UnknownCount
                    || (rest.UnknownCount == winning.UnknownCount && rest.Pieces + 1 > winning.Pieces)
                    || !words.Contains(text.AsSpan(start, length)))
                    continue;
                winning = new Path(end, rest.UnknownCount, rest.Pieces + 1, IsWord: true);
            }
            best[index] = winning;
        }

        var breaks = new List<int>();
        for (var index = 0; index < count; )
        {
            index = best[index].End;
            breaks.Add(clusters[index - 1].End);
        }
        return breaks;
    }

    private static DictionaryData Load(string resource)
    {
        using var stream = typeof(WordBreakDictionary).Assembly.GetManifestResourceStream(
            $"Doroti.Skia.Rendering.Unicode.{resource}.txt.gz")
            ?? throw new InvalidDataException($"The word dictionary '{resource}' is missing from the Skia renderer.");
        using var decompressor = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(decompressor);
        var words = new HashSet<string>(StringComparer.Ordinal);
        var maxLength = 0;
        while (reader.ReadLine() is { } word)
        {
            words.Add(word);
            maxLength = Math.Max(maxLength, word.Length);
        }
        return new DictionaryData(words, maxLength);
    }

    private sealed record DictionaryData(HashSet<string> Words, int MaxLength);
    private readonly record struct Path(int End, int UnknownCount, int Pieces, bool IsWord);
}
