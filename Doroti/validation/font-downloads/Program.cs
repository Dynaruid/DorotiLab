using System.Diagnostics;
using System.Net;
using System.Text;
using Doroti.Host.Web;
using Doroti.Skia.Rendering;
using SkiaSharp;

var output = Path.GetFullPath("Doroti/artifacts/font-downloads");
Directory.CreateDirectory(output);
var catalog = FontFallbackCatalog.Default;
Check(catalog.Fonts.Count == 727, "full pinned Flutter catalog");
var sample = "한글 日本語 中文 العربية हिन्दी ไทย தமிழ் ქართული עברית 😀 🚀 🐱";
var points = sample.EnumerateRunes().Where(r => r.Value > 127).Select(r => r.Value).Distinct().ToArray();
Check(points.All(cp => catalog.Candidates(cp).Count > 0), "representative scripts and emoji have candidates");
Check(catalog.Candidates(0x10ffff).Count == 0, "unassigned scalar has no font");
var notices = new List<string>();
using (var fonts = new SkiaFallbackFontCollection())
{
    var requests = 0;
    using var missing = new BrowserFontFallbackLoader(fonts, (url, ct) =>
    {
        requests++;
        return Task.FromException<byte[]>(new HttpRequestException("not found", null, HttpStatusCode.NotFound));
    }, () => throw new Exception("failed font should not notify"), log: notices.Add);
    missing.Request('한'); missing.Request('한'); missing.Request(0x10ffff); missing.Request(0x200d);
    await missing.WhenIdleAsync();
    Check(requests is > 0 and <= 3, "404 alternatives bounded; duplicate requests coalesced");
    var previous = requests;
    missing.Request('한'); missing.Request(0x10ffff);
    await missing.WhenIdleAsync();
    Check(requests == previous, "unsupported glyphs do not create frame-by-frame downloads");
}
using (var fonts = new SkiaFallbackFontCollection())
{
    var requests = 0;
    using var retry = new BrowserFontFallbackLoader(fonts, (url, ct) =>
    {
        requests++;
        return Task.FromException<byte[]>(new HttpRequestException("offline"));
    }, () => { }, log: notices.Add);
    retry.Request(0x1f600);
    await retry.WhenIdleAsync();
    Check(requests is >= 2 and <= 6 && requests % 2 == 0, "transient failure retries twice per candidate and terminates");
}
using (var fonts = new SkiaFallbackFontCollection())
{
    var requests = 0;
    var changed = 0;
    using var success = new BrowserFontFallbackLoader(fonts, (url, ct) =>
    {
        requests++;
        if (requests == 1) return Task.FromException<byte[]>(new HttpRequestException("transient"));
        return Task.FromResult(File.ReadAllBytes("samples/DorotiTestbedApp/assets/fonts/Roboto-regular.ttf"));
    }, () => changed++, log: notices.Add);
    success.Request('A'); success.Request('A'); success.Request('B');
    await success.WhenIdleAsync();
    Check(requests == 2 && changed == 1 && success.LoadedCount == 1,
        "retry succeeds, batch coverage deduplicates registration and notification");
}
using (var fonts = new SkiaFallbackFontCollection())
{
    using var disabled = new BrowserFontFallbackLoader(fonts,
        (url, ct) => throw new Exception("disabled download"), () => { }, new() { Enabled = false });
    disabled.Request(0x1f600);
    await disabled.WhenIdleAsync();
    Check(disabled.DownloadCount == 0, "automatic downloads can be disabled");
}
using (var fonts = new SkiaFallbackFontCollection())
{
    var started = new TaskCompletionSource();
    using var cancelled = new BrowserFontFallbackLoader(fonts, async (url, ct) =>
    {
        started.SetResult();
        await Task.Delay(Timeout.Infinite, ct);
        return [];
    }, () => throw new Exception("disposed loader must not register"));
    cancelled.Request(0x1f600);
    await started.Task;
    var idle = cancelled.WhenIdleAsync();
    cancelled.Dispose();
    await idle;
    Check(fonts.Families.Count == 0, "dispose cancels in-flight work without late registration");
}

using (var fonts = new SkiaFallbackFontCollection())
{
    using var concurrent = new BrowserFontFallbackLoader(fonts, async (url, ct) =>
    {
        await Task.Delay(10, ct);
        return File.ReadAllBytes("samples/DorotiTestbedApp/assets/fonts/Roboto-regular.ttf");
    }, () => { }, log: notices.Add);
    await Task.WhenAll(Enumerable.Range(0, 24).Select(i => Task.Run(() => concurrent.Request('A' + i))));
    await concurrent.WhenIdleAsync();
    Check(concurrent.DownloadCount == 1 && concurrent.LoadedCount == 1,
        "concurrent missing-glyph requests coalesce without mutating an active enumeration");
}

// --network also exercises the production WOFF2 wrapper and real CDN files.
if (!args.Contains("--network"))
{
    Console.WriteLine("PASS fallback catalog and failure lifecycle (network not requested)");
    return;
}
using var http = new HttpClient();
foreach (var (path, weight) in BrowserDefaultFonts.Paths.Zip(new[] { 400, 500, 700 }))
{
    using var data = SKData.CreateCopy(await http.GetByteArrayAsync(new Uri(new BrowserFontFallbackOptions().BaseUrl, path)));
    using var face = SKTypeface.FromData(data);
    Check(face is { FamilyName: "Roboto" } && face.FontWeight == weight, $"CDN default Roboto weight {weight}");
}
using var collection = new SkiaFallbackFontCollection("Roboto");
collection.Register(File.ReadAllBytes("samples/DorotiTestbedApp/assets/fonts/Roboto-regular.ttf"));
var changes = 0;
using var loader = new BrowserFontFallbackLoader(collection, Download, () => changes++,
    new() { PreferredLanguage = "ko-KR" });
foreach (var cp in points) { loader.Request(cp); loader.Request(cp); }
await loader.WhenIdleAsync();
Check(points.All(collection.ContainsCharacter), "real downloaded fonts cover every representative scalar");
Check(loader.LoadedCount == changes && changes > 4, "each decoded font registers and notifies");
var downloads = loader.DownloadCount;
foreach (var cp in points) loader.Request(cp);
await loader.WhenIdleAsync();
Check(downloads == loader.DownloadCount, "loaded coverage avoids repeat network requests");
// Raster every decoded face directly to verify COLRv1 survives decompression.
using var bitmap = new SKBitmap(1000, 1000);
using var canvas = new SKCanvas(bitmap);
canvas.Clear(SKColors.White);
using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
var y = 50f;
foreach (var file in Directory.GetFiles(output, "font-*.ttf").Order())
{
    using var face = SKTypeface.FromFile(file);
    using var font = new SKFont(face, 30);
    var text = string.Concat(sample.EnumerateRunes().Where(r => font.ContainsGlyph(r.Value)).Select(r => r.ToString()));
    canvas.DrawText(text, 10, y, SKTextAlign.Left, font, paint);
    y += 48;
}
using var image = SKImage.FromBitmap(bitmap);
using var png = image.Encode();
File.WriteAllBytes(Path.Combine(output, "downloaded-fonts.png"), png.ToArray());
// Windows DirectWrite cannot render this COLRv1 font, whereas the web FreeType
// backend can. Color acceptance is the actual Chrome screenshot, not this raster.
Console.WriteLine($"INFO native backend COLRv1 color: {bitmap.Pixels.Any(p => p.Red != p.Green || p.Green != p.Blue)}");
Console.WriteLine($"PASS {loader.LoadedCount} real fallback fonts; script coverage, decode and registration");

async Task<byte[]> Download(Uri uri, CancellationToken cancellationToken)
{
    var index = catalog.Fonts.Single(f => uri.AbsoluteUri.EndsWith(f.Path, StringComparison.Ordinal)).Index;
    var compressed = Path.Combine(output, $"font-{index}.woff2");
    var decoded = Path.Combine(output, $"font-{index}.ttf");
    await File.WriteAllBytesAsync(compressed, await http.GetByteArrayAsync(uri, cancellationToken), cancellationToken);
    var start = new ProcessStartInfo("node") { UseShellExecute = false, CreateNoWindow = true };
    start.ArgumentList.Add(Path.GetFullPath("Doroti/validation/font-downloads/decode-file.mjs"));
    start.ArgumentList.Add(compressed);
    start.ArgumentList.Add(decoded);
    using var process = Process.Start(start)!;
    await process.WaitForExitAsync(cancellationToken);
    if (process.ExitCode != 0) throw new InvalidDataException("WOFF2 decoder failed");
    return await File.ReadAllBytesAsync(decoded, cancellationToken);
}
static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    Console.WriteLine($"PASS {message}");
}
