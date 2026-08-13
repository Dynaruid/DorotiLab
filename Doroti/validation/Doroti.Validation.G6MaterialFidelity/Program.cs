using System.Text.Json;
using Doroti.Composition;
using Doroti.Backends.Skia;
using Doroti.Flutter.Runtime;
using Doroti.Graphics;
using Doroti.Platform;
using SkiaSharp;

var options = args.ToList();
if (options.Contains("--colors", StringComparer.Ordinal))
{
    WriteColorReference();
    return;
}

RunManagedRasterContracts();
RunBundledFontContracts();
Console.WriteLine("G6-5R managed path/clip/fill/stroke/shadow contracts: PASS");
Console.WriteLine("G6-5R Roboto/Material Icons glyph contracts: PASS");

static void WriteColorReference()
{
    long[] seeds = [0xff6750a4, 0xff006e1c, 0xffb3261e];
    double[] contrasts = [0, -1, 1];
    string[] roles =
    [
        "primary", "onPrimary", "primaryContainer", "onPrimaryContainer", "secondary", "tertiary",
        "surface", "surfaceDim", "surfaceBright", "surfaceContainer", "surfaceContainerHighest",
        "onSurface", "onSurfaceVariant", "outline", "outlineVariant",
    ];
    var cases = new List<object>();
    foreach (var seed in seeds)
    foreach (var dark in new[] { false, true })
    foreach (var contrast in contrasts)
    {
        cases.Add(new
        {
            seed,
            dark,
            contrast,
            variant = "tonalSpot",
            roles = roles.ToDictionary(
                role => role,
                role => MaterialColorSchemeRuntime.GetArgb(seed, dark, "tonalSpot", contrast, role),
                StringComparer.Ordinal),
        });
    }
    foreach (var variant in new[] { "fidelity", "content", "monochrome", "neutral", "vibrant", "expressive", "rainbow", "fruitSalad" })
    foreach (var dark in new[] { false, true })
    {
        cases.Add(new
        {
            seed = seeds[0],
            dark,
            contrast = 0d,
            variant,
            roles = roles.ToDictionary(
                role => role,
                role => MaterialColorSchemeRuntime.GetArgb(seeds[0], dark, variant, 0, role),
                StringComparer.Ordinal),
        });
    }
    Console.WriteLine(JsonSerializer.Serialize(new
    {
        schemaVersion = "doroti.g6-material-color-reference/v1",
        materialColorUtilitiesVersion = "0.13.0",
        variant = "tonalSpot",
        cases,
    }, new JsonSerializerOptions { WriteIndented = true }));
}

static void RunManagedRasterContracts()
{
    var pixels = new byte[24 * 24 * 4];

    {
        Clear(pixels);
        var canvas = new SoftwareRasterCanvas(pixels, 24, 24);
        canvas.ClipPath(new PathGeometry([
            new Offset(2, 2), new Offset(21, 2), new Offset(2, 21),
        ]));
        canvas.DrawColor(Color.FromArgb(255, 0, 0, 0));
        AssertPixel(pixels, 24, 3, 3, black: true, "triangle clip interior");
        AssertPixel(pixels, 24, 20, 20, black: false, "triangle clip must not collapse to bounds");
    }

    {
        Clear(pixels);
        var canvas = new SoftwareRasterCanvas(pixels, 24, 24);
        var triangle = new PathGeometry([
            new Offset(3, 3), new Offset(20, 3), new Offset(3, 20),
        ]);
        canvas.DrawPath(triangle, new RasterPaint(Color.FromArgb(255, 0, 0, 0), Style: RasterPaintStyle.Stroke, StrokeWidth: 2));
        AssertPixel(pixels, 24, 3, 10, black: true, "path stroke edge");
        AssertPixel(pixels, 24, 8, 8, black: false, "path stroke must preserve hollow interior");
    }

    {
        Clear(pixels);
        var canvas = new SoftwareRasterCanvas(pixels, 24, 24);
        var square = new PathGeometry([
            new Offset(8, 8), new Offset(16, 8), new Offset(16, 16), new Offset(8, 16),
        ]);
        canvas.DrawPath(square, new RasterPaint(Color.FromArgb(255, 0, 0, 0), Opacity: 0.7, BlurSigma: 2));
        AssertPixelChanged(pixels, 24, 6, 12, "blurred path must extend outside geometry bounds");
        AssertPixel(pixels, 24, 1, 1, black: false, "blur extent must remain bounded");
    }
}

static void RunBundledFontContracts()
{
    var root = FindRepositoryRoot();
    var assetRoot = Path.Combine(root, "Doroti", "src", "Doroti.Vendor.Avalonia.Skia", "Assets");
    var robotoPath = Path.Combine(assetRoot, "Roboto-Regular.ttf");
    var iconsPath = Path.Combine(assetRoot, "MaterialIcons-Regular.otf");
    foreach (var path in new[] { robotoPath, iconsPath, Path.Combine(assetRoot, "Roboto_LICENSE.txt"), Path.Combine(assetRoot, "MaterialIcons_LICENSE.txt") })
        if (!File.Exists(path) || new FileInfo(path).Length == 0) throw new InvalidDataException($"Bundled font asset is missing: {path}");

    using var roboto = SKTypeface.FromFile(robotoPath) ?? throw new InvalidDataException("Roboto typeface could not be loaded.");
    using var icons = SKTypeface.FromFile(iconsPath) ?? throw new InvalidDataException("Material Icons typeface could not be loaded.");
    using var robotoFont = new SKFont(roboto, 16);
    using var iconFont = new SKFont(icons, 24);
    if (robotoFont.GetGlyphs("August 2026").Any(glyph => glyph == 0))
        throw new InvalidDataException("Roboto does not cover the calendar reference text.");
    foreach (var codepoint in new[] { 0xe5c5, 0xe5cb, 0xe5cc, 0xe5c8 })
    {
        var text = char.ConvertFromUtf32(codepoint);
        if (iconFont.GetGlyphs(text) is not [var glyph] || glyph == 0)
            throw new InvalidDataException($"Material Icons codepoint U+{codepoint:X4} resolved to tofu.");
    }
}

static string FindRepositoryRoot()
{
    for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        if (File.Exists(Path.Combine(directory.FullName, "goal6.md"))) return directory.FullName;
    throw new DirectoryNotFoundException("Could not locate the DorotiLab repository root.");
}

static void Clear(byte[] pixels)
{
    for (var offset = 0; offset < pixels.Length; offset += 4)
        pixels.AsSpan(offset, 4).Fill(255);
}

static void AssertPixel(byte[] pixels, int width, int x, int y, bool black, string name)
{
    var offset = ((y * width) + x) * 4;
    var isBlack = pixels[offset] < 32 && pixels[offset + 1] < 32 && pixels[offset + 2] < 32;
    if (isBlack != black) throw new InvalidDataException($"{name} failed at ({x},{y}).");
}

static void AssertPixelChanged(byte[] pixels, int width, int x, int y, string name)
{
    var offset = ((y * width) + x) * 4;
    if (pixels[offset] > 250 && pixels[offset + 1] > 250 && pixels[offset + 2] > 250)
        throw new InvalidDataException($"{name} failed at ({x},{y}).");
}
