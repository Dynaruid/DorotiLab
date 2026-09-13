using Doroti.Ui;

internal static class WordBoundaryContracts
{
    internal static void Verify()
    {
        foreach (var (text, ranges) in new (string, (int Start, int End)[] Ranges)[] {
            ("abc def", [(0, 3), (3, 4), (4, 7)]),
            ("abc   def", [(0, 3), (3, 6), (6, 9)]),
            ("가나다\u00a0라마바", [(0, 3), (3, 4), (4, 7)]),
            (" \tabc\r\n def ", [(0, 2), (2, 5), (5, 6), (6, 7), (7, 8), (8, 11), (11, 12)]),
            ("   ", [(0, 3)]),
            ("", []),
        }) {
            var builder = new ParagraphBuilder(new ParagraphStyle());
            builder.addText(text);
            using var paragraph = builder.build();
            foreach (var (start, end) in ranges) {
                for (var offset = start; offset < end; offset++) {
                    var actual = paragraph.getWordBoundary(new TextPosition(offset));
                    if (actual.start != start || actual.end != end)
                        throw new Exception($"Word boundary at {offset} in '{text}': expected {start}:{end}, got {actual.start}:{actual.end}");
                }
            }
            var final = paragraph.getWordBoundary(new TextPosition(text.Length));
            var expectedStart = text.Length == 0 || char.IsWhiteSpace(text[^1]) ? text.Length : ranges[^1].Start;
            if (final.start != expectedStart || final.end != text.Length)
                throw new Exception($"End-of-text word boundary changed in '{text}': {final.start}:{final.end}");
        }
        Console.WriteLine("Paragraph whitespace and hard-break word boundaries: PASS");
    }
}
