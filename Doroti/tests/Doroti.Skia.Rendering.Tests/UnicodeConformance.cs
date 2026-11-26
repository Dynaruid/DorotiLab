#if DOROTI_SHAPED_PARAGRAPHS
using System.Globalization;
using System.Text;
using Doroti.Skia.Rendering;
using Doroti.Skia.Rendering.Unicode;

internal static class UnicodeConformance
{
    internal static void Run(string directory)
    {
        var discrepancies = 0;
        Check("BidiCharacterTest.txt", Bidi);
        Check("GraphemeBreakTest.txt", Graphemes);
        Check("WordBreakTest.txt", Words);
        Check("LineBreakTest.txt", Lines);
        CheckAsciiLineBreaks();
        if (discrepancies != 0)
            throw new InvalidOperationException($"{discrepancies} Unicode 17.0.0 conformance discrepancies.");

        void Check(string name, Func<string, string?> compare)
        {
            var cases = 0;
            var failures = 0;
            var lineNumber = 0;
            foreach (var row in File.ReadLines(Path.Combine(directory, name)))
            {
                lineNumber++;
                var comment = row.IndexOf('#');
                var data = (comment < 0 ? row : row[..comment]).Trim();
                if (data.Length == 0) continue;
                cases++;
                var difference = compare(data);
                if (difference is null) continue;
                failures++;
                if (failures <= 5) Console.WriteLine($"{name}:{lineNumber} {difference} [{data}]");
            }

            Console.WriteLine($"{name}: {cases - failures}/{cases} pass");
            discrepancies += failures;
        }
    }

    private static void CheckAsciiLineBreaks()
    {
        string[] texts = ["", " ", "   ", "A", "a2 B  12", " Add 2 items ", "11 22 33", "Price 24.00",
            "Time 12:59", "1,000.00", "A : 3", "AB ;2", "A  ,A", "A .1", "A . ", "A  ::2", "A ,:;.",
            "e.g. text, continued.", "(text) ‛here’", "red\u00A0blue", "some\twords", "A\r\nB", "a\u0301 b"];
        foreach (var text in texts)
        {
            var expected = new bool[text.Length + 1];
            var breaks = new LineBreakEnumerator(text);
            while (breaks.MoveNext(out var line)) expected[line.PositionWrap] = true;
            var actual = new ParagraphTextAnalysis(text, Doroti.Ui.TextDirection.ltr).Breaks;
            if (!expected.AsSpan().SequenceEqual(actual))
                throw new InvalidOperationException($"Paragraph line breaks differ from the Unicode rules for '{text}'.");
        }
        Console.WriteLine($"Paragraph line break equivalence: {texts.Length}/{texts.Length} pass");
    }

    private static string? Bidi(string data)
    {
        var fields = data.Split(';');
        var text = string.Concat(Tokens(fields[0]).Select(Scalar));
        var paragraph = new BidiData { ParagraphEmbeddingLevel = sbyte.Parse(fields[1], CultureInfo.InvariantCulture) };
        paragraph.Append(text);
        var algorithm = new BidiAlgorithm();
        algorithm.Process(paragraph);
        if (algorithm.ResolvedParagraphEmbeddingLevel != int.Parse(fields[2], CultureInfo.InvariantCulture))
            return $"paragraph level {algorithm.ResolvedParagraphEmbeddingLevel}";

        var expected = Tokens(fields[3]);
        var indices = new List<int>(expected.Length);
        var levels = new List<sbyte>(expected.Length);
        for (var i = 0; i < expected.Length; i++)
        {
            if (expected[i] == "x") continue;
            var level = algorithm.ResolvedLevels[i];
            if (level != sbyte.Parse(expected[i], CultureInfo.InvariantCulture))
                return $"resolved level {level} at index {i}";
            indices.Add(i);
            levels.Add(level);
        }
        var visualOrder = new int[levels.Count];
        ParagraphTextAnalysis.ReorderVisual(visualOrder, levels.ToArray());
        var order = visualOrder.Select(index => indices[index]);
        return order.SequenceEqual(Tokens(fields[4]).Select(token => int.Parse(token, CultureInfo.InvariantCulture)))
            ? null
            : $"visual order {string.Join(",", order)}";
    }

    private static string? Graphemes(string data)
    {
        var (text, expected) = ParseBreaks(data);
        var results = new List<int> { 0 };
        var graphemes = new GraphemeEnumerator(text);
        while (graphemes.MoveNext(out var grapheme))
            results.Add(grapheme.Offset + grapheme.Length);
        return CheckBreaks(results, expected);
    }

    private static string? Words(string data)
    {
        var (text, expected) = ParseBreaks(data);
        var results = new List<int> { 0 };
        var words = new WordBreakEnumerator(text);
        while (words.MoveNext(out var word))
            results.Add(word.Offset + word.Length);
        return CheckBreaks(results, expected);
    }

    private static string? Lines(string data)
    {
        var (text, expected) = ParseBreaks(data);
        var results = new List<int>();
        var lines = new LineBreakEnumerator(text);
        while (lines.MoveNext(out var line))
            results.Add(line.PositionWrap);
        return CheckBreaks(results, expected);
    }

    private static (string Text, List<int> Breaks) ParseBreaks(string data)
    {
        var text = new StringBuilder();
        var breaks = new List<int>();
        foreach (var token in Tokens(data))
        {
            if (token == "÷") breaks.Add(text.Length);
            else if (token != "×") text.Append(Scalar(token));
        }
        return (text.ToString(), breaks);
    }

    private static string? CheckBreaks(List<int> actual, List<int> expected) =>
        actual.SequenceEqual(expected) ? null : $"breaks {string.Join(",", actual)}, expected {string.Join(",", expected)}";

    private static string[] Tokens(string value) =>
        value.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

    private static string Scalar(string value) =>
        char.ConvertFromUtf32(int.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
}
#endif
