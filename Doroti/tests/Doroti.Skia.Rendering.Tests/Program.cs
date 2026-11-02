using System.Collections;
using System.Reflection;
using Doroti.Skia.Rendering;
using SkiaSharp;

const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
var resourcesType = typeof(SkiaSceneRenderer).GetNestedType("TextRenderResources", BindingFlags.NonPublic)!;
var keyType = typeof(SkiaSceneRenderer).GetNestedType("TextRenderKey", BindingFlags.NonPublic)!;
var drawMethod = resourcesType.GetMethod("DrawText", Members)!;
var resolve = resourcesType.GetMethod("ResolveFont", Members)!;
var cacheField = resourcesType.GetField("_spacedText", Members)!;
var unitsField = resourcesType.GetField("_spacedTextCodeUnits", Members)!;
var paintProperty = resourcesType.GetProperty("Paint", Members)!;
var count = 0;
foreach (var (text, letter, word, italic) in new (string, float, float, bool)[]
{
    ("Material Button 123", .1f, 0, false), ("한글 선택 삭제", .5f, 2, false),
    ("A😀 한글 e\u0301", 1.25f, .75f, false), ("italic overlap", -.75f, 0, true),
    ("word spacing only", 0, 4.5f, false), ("\ud800 invalid scalar", .1f, 0, false),
    (new string('W', 1100), .1f, 0, false),
})
{
    using var resource = Create(letter, word, italic);
    var draw = drawMethod.CreateDelegate<Action<SKCanvas, string, float, float>>(resource);
    foreach (var scale in new[] { 1f, 2.75f, 4f })
    {
        using var expected = Render(false);
        using var actual = Render(true);
        Check(expected.GetPixelSpan().SequenceEqual(actual.GetPixelSpan()), $"pixel identity: {text}/{scale}");
        using var repeated = Render(true);
        Check(expected.GetPixelSpan().SequenceEqual(repeated.GetPixelSpan()), "cached replay retains exact pixels");
        count++;
        SKBitmap Render(bool cached)
        {
            var bitmap = new SKBitmap(1000, 300, SKColorType.Rgba8888, SKAlphaType.Premul);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);
            canvas.Scale(scale);
            canvas.Translate(.125f, .375f);
            canvas.ClipRect(new SKRect(1.25f, 2.5f, 900 / scale, 250 / scale));
            if (cached) draw(canvas, text, 7.375f, 38.125f);
            else DrawOriginal(resource, canvas, text, 7.375f, 38.125f, letter, word);
            return bitmap;
        }
    }
    if (text.Contains('\ud800') || text.Length > 1024)
    {
        Check(((IDictionary)cacheField.GetValue(resource)!).Count == 0, "malformed/oversized text retains the string drawing path");
        continue;
    }
    // Touch the same entry without allocations, then force bounded eviction.
    using var scratch = new SKBitmap(2, 2);
    using var scratchCanvas = new SKCanvas(scratch);
    draw(scratchCanvas, text, 7.375f, 38.125f);
    var bytes = GC.GetAllocatedBytesForCurrentThread();
    for (var i = 0; i < 20; i++) draw(scratchCanvas, text, 7.375f, 38.125f);
    Check(GC.GetAllocatedBytesForCurrentThread() == bytes, "cached spaced text allocates no managed objects");
    var firstBlob = FirstBlob(resource);
    for (var i = 0; i < 20; i++) draw(scratchCanvas, new string((char)('a' + i), 160), i, 38);
    Check(((IDictionary)cacheField.GetValue(resource)!).Count <= 32 && (int)unitsField.GetValue(resource)! <= 2048,
        "entry and text storage budgets are bounded");
    Check(firstBlob.Handle == IntPtr.Zero, "eviction disposes native blobs");
    var lastBlob = FirstBlob(resource);
    resource.Dispose();
    Check(lastBlob.Handle == IntPtr.Zero, "resource retirement disposes native blobs before fonts");
}
Console.WriteLine($"PASS: {count} exact spaced-text pixel comparisons, Unicode/fallback/spacing/transforms, zero-allocation replay and native cache retirement");
ShadowPictureRegression.Run();
SkiaFontVariations.Key(null); SkiaFontVariations.Normalize(null);
var emptyBytes = GC.GetAllocatedBytesForCurrentThread();
for (var i = 0; i < 20; i++)
{
    Check(SkiaFontVariations.Key(null) == string.Empty && SkiaFontVariations.Key([]) == string.Empty, "empty variation keys are unchanged");
    Check(SkiaFontVariations.Normalize(null).Length == 0 && SkiaFontVariations.Normalize([]).Length == 0, "empty variation normalization is unchanged");
}
Check(GC.GetAllocatedBytesForCurrentThread() == emptyBytes, "empty font variations allocate no query pipelines");
Check(SkiaFontVariations.Key([new("wght", 400), new("bad", 1), new("wdth", 90), new("wght", 600), new("slnt", double.NaN)])
    == "wdth=90;wght=600", "nonempty axes retain validation, last-value priority and ordinal ordering");
Console.WriteLine("PASS: allocation-free empty font variation keys and normalization");

IDisposable Create(float letter, float word, bool italic)
{
    var color = italic ? new SKColor(24, 71, 145, 103) : SKColors.Black;
    var key = Activator.CreateInstance(keyType, Members, null,
        ["Segoe UI", 17f, color, 400, italic, letter, word, "[]", ""], null)!;
    return (IDisposable)Activator.CreateInstance(resourcesType, Members, null,
        ["Segoe UI", 17f, color, null, key, null, null], null)!;
}
SKTextBlob FirstBlob(object resource)
{
    var cache = (IDictionary)cacheField.GetValue(resource)!;
    var entry = cache.Values.Cast<object>().First();
    return (SKTextBlob)entry.GetType().GetProperty("Blob")!.GetValue(entry)!;
}
void DrawOriginal(object resource, SKCanvas canvas, string text, float x, float baseline, float letter, float word)
{
    var paint = (SKPaint)paintProperty.GetValue(resource)!;
    for (var index = 0; index < text.Length; )
    {
        var length = char.IsHighSurrogate(text[index]) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]) ? 2 : 1;
        var codePoint = length == 2 ? char.ConvertToUtf32(text[index], text[index + 1]) : text[index];
        var fontResource = resolve.Invoke(resource, [codePoint])!;
        var font = (SKFont)fontResource.GetType().GetProperty("Font", Members)!.GetValue(fontResource)!;
        var glyph = text.Substring(index, length);
        canvas.DrawText(glyph, x + letter / 2, baseline, SKTextAlign.Left, font, paint);
        x += font.MeasureText(glyph, paint) + letter + (glyph == " " ? word : 0);
        index += length;
    }
}
static void Check(bool value, string message)
{
    if (!value) throw new InvalidOperationException(message);
}
