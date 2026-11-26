using System.Buffers;
using Doroti.Ui;
using Doroti.Skia.Rendering.Unicode;

namespace Doroti.Skia.Rendering;

internal sealed class ParagraphTextAnalysis
{
    private static readonly SearchValues<char> SimpleLineBreakCharacters =
        SearchValues.Create(" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,.:;");

    internal readonly record struct Cluster(int Start, int End, Codepoint First, sbyte Level,
        Script Script, bool IsHardBreak);

    internal ParagraphTextAnalysis(string text, TextDirection direction)
    {
        BaseLevel = direction == TextDirection.rtl ? (sbyte)1 : (sbyte)0;
        Clusters = AnalyzeClusters(text, BaseLevel);

        Breaks = FindDefaultLineBreaks(text);

        List<DictionaryRun>? dictionaryRuns = null;
        for (var first = 0; first < Clusters.Length; )
        {
            var script = Clusters[first].Script;
            var dictionary = WordBreakDictionary.ForScript(script);
            if (dictionary is null || !IsDictionaryCluster(Clusters[first], script))
            {
                first++;
                continue;
            }

            var last = first + 1;
            while (last < Clusters.Length && IsDictionaryCluster(Clusters[last], script)) last++;
            var start = Clusters[first].Start;
            var end = Clusters[last - 1].End;
            var offsets = dictionary.FindBreaks(text, Clusters.AsSpan(first, last - first));
            (dictionaryRuns ??= []).Add(new(start, end, offsets));
            foreach (var offset in offsets)
            {
                if (offset < end && text[offset - 1] != '\u200d')
                    Breaks[offset] = true;
            }
            first = last;
        }

        WordBoundaries = new(() => FindWordBoundaries(text, dictionaryRuns));
    }

    internal sbyte BaseLevel { get; }
    internal Cluster[] Clusters { get; }
    internal bool[] Breaks { get; }
    internal Lazy<int[]> WordBoundaries { get; }

    private static bool[] FindDefaultLineBreaks(ReadOnlySpan<char> text)
    {
        var breaks = new bool[text.Length + 1];
        if (text.IsEmpty) return breaks;
        if (!text.ContainsAnyExcept(SimpleLineBreakCharacters))
        {
            // UAX #14 for AL, NU, SP and ASCII IS. Only the end of a
            // space run can break before end of text (LB7 and LB18).
            // Before IS that also requires a following digit (LB15c/d).
            for (var i = 1; i < text.Length; i++)
            {
                if (text[i - 1] != ' ' || text[i] == ' ') continue;
                breaks[i] = text[i] is not (',' or '.' or ':' or ';')
                    || (i + 1 < text.Length && char.IsAsciiDigit(text[i + 1]));
            }
            breaks[text.Length] = true;
            return breaks;
        }
        var lineBreaks = new LineBreakEnumerator(text);
        while (lineBreaks.MoveNext(out var lineBreak))
            breaks[lineBreak.PositionWrap] = true;
        return breaks;
    }

    private static Cluster[] AnalyzeClusters(string text, sbyte baseLevel)
    {
        // Printable ASCII in an LTR paragraph has one grapheme per code unit
        // and a single embedding level. It uses the normal shaper and line
        // break rules, including Latin OpenType features.
        if (baseLevel == 0 && text.AsSpan().IndexOfAnyExceptInRange(' ', '~') < 0)
        {
            var ascii = new Cluster[text.Length];
            for (var i = 0; i < ascii.Length; i++)
                ascii[i] = new(i, i + 1, new(text[i]), 0, Script.Latin, false);
            return ascii;
        }

        var levels = ResolveParagraphLevels(text, baseLevel);
        var clusters = new List<Cluster>();
        var graphemes = new GraphemeEnumerator(text);
        while (graphemes.MoveNext(out var grapheme))
        {
            var first = grapheme.FirstCodepoint;
            var hardBreak = first.LineBreakClass is LineBreakClass.MandatoryBreak
                or LineBreakClass.CarriageReturn or LineBreakClass.LineFeed or LineBreakClass.NextLine;
            clusters.Add(new(grapheme.Offset, grapheme.Offset + grapheme.Length, first,
                levels[grapheme.Offset], first.Script, hardBreak));
        }
        var result = clusters.ToArray();
        ResolveCommonScripts(result);
        return result;
    }

    private static int[] FindWordBoundaries(string text, List<DictionaryRun>? dictionaryRuns)
    {
        var words = new List<int> { 0 };
        var wordBreaks = new WordBreakEnumerator(text);
        while (wordBreaks.MoveNext(out var word))
        {
            var end = word.Offset + word.Length;
            if (end > words[^1]) words.Add(end);
        }
        if (words[^1] < text.Length) words.Add(text.Length);
        if (dictionaryRuns is null) return words.ToArray();

        var boundaries = new SortedSet<int>(words);
        foreach (var run in dictionaryRuns)
        {
            boundaries.RemoveWhere(offset => offset > run.Start && offset < run.End);
            boundaries.Add(run.Start);
            boundaries.UnionWith(run.Offsets);
        }
        return boundaries.ToArray();
    }

    private readonly record struct DictionaryRun(int Start, int End, List<int> Offsets);

    private static bool IsDictionaryCluster(Cluster cluster, Script script) =>
        cluster.Script == script && cluster.First.Script == script
        && cluster.First.LineBreakClass == LineBreakClass.ComplexContext;

    private static void ResolveCommonScripts(Span<Cluster> clusters)
    {
        // Common and inherited code points belong to a neighboring script run
        // when their Script_Extensions permit it. Keep the actual bidi level.
        var next = Script.Unknown;
        for (var i = clusters.Length - 1; i >= 0; i--)
        {
            var cluster = clusters[i];
            if (cluster.IsHardBreak)
                next = Script.Unknown;
            else if (IsStrongScript(cluster.Script))
                next = cluster.Script;
            else if (next != Script.Unknown)
                clusters[i] = cluster with { Script = next };
        }
        var previous = Script.Unknown;
        for (var i = 0; i < clusters.Length; i++)
        {
            var cluster = clusters[i];
            if (cluster.IsHardBreak)
                previous = Script.Unknown;
            else if (IsStrongScript(cluster.First.Script))
                previous = cluster.First.Script;
            else if (previous != Script.Unknown
                && (cluster.First.HasScriptExtension(previous) || !IsStrongScript(cluster.Script)))
                clusters[i] = cluster with { Script = previous };
        }
    }

    private static bool IsStrongScript(Script script) =>
        script is not (Script.Common or Script.Inherited or Script.Unknown);

    internal static sbyte[] ResolveParagraphLevels(string text, sbyte baseLevel)
    {
        var utf16Levels = new sbyte[text.Length];
        var algorithm = new BidiAlgorithm();
        var data = new BidiData { ParagraphEmbeddingLevel = baseLevel };
        var start = 0;
        for (var offset = 0; offset < text.Length; )
        {
            var codepoint = Codepoint.ReadAt(text, offset, out var unitCount);
            offset += unitCount;
            if (codepoint.BiDiClass == BidiClass.ParagraphSeparator)
            {
                Resolve(start, offset);
                start = offset;
            }
        }
        Resolve(start, text.Length);
        return utf16Levels;

        void Resolve(int begin, int end)
        {
            if (begin == end) return;
            data.Reset();
            data.ParagraphEmbeddingLevel = baseLevel;
            data.Append(text.AsSpan(begin, end - begin));
            algorithm.Process(data);
            var levels = algorithm.ResolvedLevels;
            var i = begin;
            for (var character = 0; i < end; character++)
            {
                Codepoint.ReadAt(text, i, out var length);
                utf16Levels.AsSpan(i, length).Fill(levels[character]);
                i += length;
            }
        }
    }

    internal static void ReorderVisual(Span<int> order, ReadOnlySpan<sbyte> levels)
    {
        // UAX #9 L2 reverses nested sequences, from the deepest level down
        // through the lowest odd level present on this line.
        var highest = 0;
        var lowestOdd = int.MaxValue;
        for (var i = 0; i < order.Length; i++)
        {
            order[i] = i;
            highest = Math.Max(highest, levels[i]);
            if ((levels[i] & 1) != 0) lowestOdd = Math.Min(lowestOdd, levels[i]);
        }
        for (var level = highest; level >= lowestOdd; level--)
        {
            for (var start = 0; start < order.Length; )
            {
                if (levels[order[start]] < level)
                {
                    start++;
                    continue;
                }
                var end = start + 1;
                while (end < order.Length && levels[order[end]] >= level) end++;
                order[start..end].Reverse();
                start = end;
            }
        }
    }
}
