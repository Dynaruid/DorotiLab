using HarfBuzzSharp;
using SkiaSharp;
using SkiaSharp.HarfBuzz;
using Topten.RichTextKit;
using Buffer = HarfBuzzSharp.Buffer;

internal static class ParagraphProbe
{
    internal static (string Receipt, byte[] Png) Run(Func<string, byte[]> getFont)
    {
        using var arabic = Load("Arabic");
        using var devanagari = Load("Devanagari");
        using var thai = Load("Thai");
        using var emoji = Load("Emoji");
        using var bitmap = new SKBitmap(420, 240);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);
        using var recorder = new SKPictureRecorder();
        var recording = recorder.BeginRecording(SKRect.Create(bitmap.Width, bitmap.Height));

        const string arabicText = "سلام";
        using var arabicFont = new SKFont(arabic, 32);
        using var arabicShaper = new SKShaper(arabic);
        var connected = Shape(arabicShaper, arabicFont, arabicText, Direction.RightToLeft, "ar");
        var isolated = arabicText.Reverse().Select(c =>
            Shape(arabicShaper, arabicFont, c.ToString(), Direction.RightToLeft, "ar").Codepoints[0]);
        Expect(!connected.Codepoints.SequenceEqual(isolated) && connected.Clusters.SequenceEqual(connected.Clusters.OrderDescending()),
            "Arabic contextual shaping and visual cluster order");
        Draw(recording, arabicFont, connected, new SKPoint(16, 42));

        using var devanagariFont = new SKFont(devanagari, 32);
        using var devanagariShaper = new SKShaper(devanagari);
        var reordered = Shape(devanagariShaper, devanagariFont, "कि", Direction.LeftToRight, "hi");
        Expect(reordered.Codepoints.Length == 2 && reordered.Clusters.All(cluster => cluster == 0)
            && reordered.Codepoints[0] != devanagariFont.GetGlyph('क'), "Devanagari pre-base vowel and syllable cluster");
        Draw(recording, devanagariFont, reordered, new SKPoint(16, 85));

        using var emojiFont = new SKFont(emoji, 32);
        using var emojiShaper = new SKShaper(emoji);
        var joined = Shape(emojiShaper, emojiFont, "👩‍💻🙂", Direction.LeftToRight, "en");
        Expect(joined.Codepoints.Length == 2 && joined.Clusters.SequenceEqual(new uint[] { 0, 5 }),
            "ZWJ emoji and UTF-16 cluster offsets");
        Draw(recording, emojiFont, joined, new SKPoint(16, 128));

        var mixed = new TextBlock
        {
            BaseDirection = TextDirection.LTR,
            MaxWidth = 350,
            FontMapper = new FixedFontMapper(arabic),
        };
        mixed.AddText("abc سلام xyz", new Style { FontSize = 32, TextColor = SKColors.Black });
        mixed.Layout();
        var runSummary = string.Join(";", mixed.FontRuns.Select(run => $"{run.Direction}:{run.Start}-{run.End}"));
        var rtl = mixed.FontRuns.SingleOrDefault(run => run.Direction == TextDirection.RTL && run.Start == 4 && run.End == 8)
            ?? throw new InvalidOperationException($"Mixed LTR/RTL run order: {runSummary}");
        Expect(rtl.Glyphs.Length == connected.Codepoints.Length, "mixed LTR/RTL run shaping");
        Expect(rtl.GetXCoordOfCodePointIndex(rtl.Start) > rtl.GetXCoordOfCodePointIndex(rtl.End),
            "mixed text decreasing logical caret coordinates in the RTL run");
        var hitX = rtl.XCoord + rtl.Width / 2;
        var middle = mixed.HitTest(hitX, mixed.MeasuredHeight / 2);
        Expect(middle.OverCodePointIndex >= rtl.Start && middle.OverCodePointIndex < rtl.End,
            "mixed text hit test in the Arabic run");
        var rtlRange = new TextRange(rtl.Start, rtl.End);
        var selectionColor = new SKColor(170, 209, 255);
        mixed.Paint(recording, new SKPoint(16, 160), new TextPaintOptions
        {
            Selection = rtlRange,
            SelectionColor = selectionColor,
        });
        mixed.Clear();

        var thaiBlock = new TextBlock { FontMapper = new FixedFontMapper(thai) };
        thaiBlock.AddText("ภาษาไทยภาษาไทย", new Style { FontSize = 32 });
        thaiBlock.Layout();
        var thaiBreaks = string.Join(",", thaiBlock.WordBoundaryIndicies);

        using (var picture = recorder.EndRecording()) canvas.DrawPicture(picture);
        canvas.Flush();
        foreach (var (first, end) in new[] { (12, 52), (54, 94), (95, 140), (160, 225) })
        {
            var hasPaint = false;
            for (var y = first; y < end && !hasPaint; y++)
                for (var x = 0; x < bitmap.Width && !hasPaint; x++)
                    hasPaint = bitmap.GetPixel(x, y) != SKColors.White;
            Expect(hasPaint, $"retained drawing in the row {first}-{end}");
        }
        Expect(Enumerable.Range(160, 65).Any(y => bitmap.GetPixel((int)(16 + hitX), y) == selectionColor),
            "the hit-tested RTL range uses the recorded selection geometry");

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return ($"Arabic clusters={string.Join(",", connected.Clusters)} | Indic clusters={string.Join(",", reordered.Clusters)}"
            + $" | emoji UTF16={string.Join(",", joined.Clusters)} | mixed RTL={rtlRange.Minimum}-{rtlRange.Maximum}; hit={middle.OverCodePointIndex}"
            + $" | RichTextKit Thai word starts=[{thaiBreaks}]", data.ToArray());

        SKTypeface Load(string name)
        {
            using var data = SKData.CreateCopy(getFont(name));
            return SKTypeface.FromData(data) ?? throw new InvalidOperationException($"Cannot load {name}.");
        }
    }

    private static SKShaper.Result Shape(SKShaper shaper, SKFont font, string text, Direction direction, string languageTag)
    {
        using var buffer = new Buffer();
        using var language = new Language(languageTag);
        buffer.AddUtf16(text);
        buffer.Direction = direction;
        buffer.Language = language;
        buffer.GuessSegmentProperties();
        var result = shaper.Shape(buffer, font);
        Expect(result.Width > 0 && result.Codepoints.All(glyph => glyph != 0), "all shaped glyphs are present");
        return result;
    }

    private static void Draw(SKCanvas canvas, SKFont font, SKShaper.Result shape, SKPoint origin)
    {
        using var builder = new SKTextBlobBuilder();
        var run = builder.AllocatePositionedRun(font, shape.Codepoints.Length);
        run.SetGlyphs(Array.ConvertAll(shape.Codepoints, glyph => checked((ushort)glyph)));
        run.SetPositions(shape.Points);
        using var textBlob = builder.Build();
        using var paint = new SKPaint { IsAntialias = true, Color = SKColors.Black };
        canvas.DrawText(textBlob, origin.X, origin.Y, paint);
    }

    private static void Expect(bool condition, string expectation)
    {
        if (!condition) throw new InvalidOperationException(expectation);
    }

    private sealed class FixedFontMapper(SKTypeface typeface) : FontMapper
    {
        public override SKTypeface TypefaceFromStyle(IStyle style, bool ignoreFontVariant) => typeface;
    }
}
