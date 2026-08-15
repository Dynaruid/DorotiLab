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
if (options is ["--compositing-raster", var compositingOutput])
{
    WriteCompositingRaster(compositingOutput);
    return;
}

RunManagedRasterContracts();
RunCompositingEffectContracts();
RunRetainedLayerContracts();
RunBundledFontContracts();
Console.WriteLine("G6-5R managed path/clip/fill/stroke/shadow contracts: PASS");
Console.WriteLine("G6-5R-C managed group-opacity/saveLayer/backdrop contracts: PASS");
Console.WriteLine("G6-5R-C retained subtree ownership/replay contracts: PASS");
Console.WriteLine("G6-5R Roboto/Material/Cupertino Icons glyph contracts: PASS");

static void WriteCompositingRaster(string output)
{
    const int width = 256;
    const int height = 160;
    var pixels = new byte[width * height * 4];
    var canvas = new SoftwareRasterCanvas(pixels, width, height);
    for (var y = 0; y < height; y += 16)
    for (var x = 0; x < width; x += 16)
    {
        var color = (((x / 16) + (y / 16)) & 1) == 0
            ? Color.FromArgb(255, 245, 241, 247)
            : Color.FromArgb(255, 73, 69, 79);
        canvas.DrawRect(Rect.FromLeftTopWidthHeight(x, y, 16, 16), new RasterPaint(color));
    }

    var panel = Rect.FromLeftTopWidthHeight(32, 32, 192, 96);
    canvas.Save();
    canvas.ClipRect(panel);
    canvas.SaveLayer(new RasterLayerOptions(
        Bounds: panel,
        BackdropFilter: new RasterImageFilter(RasterImageFilterKind.Blur, 6, 2, RasterTileMode.Clamp)));
    canvas.DrawRect(panel, new RasterPaint(Color.FromArgb(107, 255, 255, 255)));
    canvas.Restore();
    canvas.Restore();

    var foreground = Rect.FromLeftTopWidthHeight(52, 52, 44, 32);
    canvas.SaveLayer(new RasterLayerOptions(
        Bounds: Rect.FromLeftTopWidthHeight(46, 46, 56, 44),
        ImageFilter: new RasterImageFilter(RasterImageFilterKind.Blur, 2, 2, RasterTileMode.Clamp)));
    canvas.DrawRect(foreground, new RasterPaint(Color.FromArgb(255, 179, 38, 30)));
    canvas.Restore();

    var opacity = Rect.FromLeftTopWidthHeight(168, 80, 40, 36);
    canvas.SaveLayer(new RasterLayerOptions(Bounds: opacity, Opacity: 0.65));
    canvas.DrawRect(opacity, new RasterPaint(Color.FromArgb(255, 103, 80, 164)));
    canvas.Restore();

    output = Path.GetFullPath(output);
    Directory.CreateDirectory(Path.GetDirectoryName(output)!);
    using var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
    System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bitmap.GetPixels(), pixels.Length);
    using var image = SKImage.FromBitmap(bitmap);
    using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = File.Create(output);
    encoded.SaveTo(stream);
}

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

static void RunCompositingEffectContracts()
{
    const int width = 24;
    const int height = 24;

    {
        var pixels = new byte[width * height * 4];
        Clear(pixels);
        var canvas = new SoftwareRasterCanvas(pixels, width, height);
        canvas.SaveLayer(new RasterLayerOptions(Bounds: new Rect(1, 1, 23, 23), Opacity: 0.5));
        canvas.DrawRect(new Rect(2, 2, 15, 20), new RasterPaint(Color.FromArgb(255, 255, 0, 0)));
        canvas.DrawRect(new Rect(8, 4, 21, 22), new RasterPaint(Color.FromArgb(255, 0, 0, 255)));
        canvas.Restore();
        AssertChannels(pixels, width, 10, 10, 255, 128, 128, 255, 2,
            "group opacity must composite overlapping children once");
    }

    {
        var pixels = new byte[width * height * 4];
        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var value = (x / 2) % 2 == 0 ? (byte)0 : (byte)255;
            var offset = ((y * width) + x) * 4;
            pixels[offset] = pixels[offset + 1] = pixels[offset + 2] = value;
            pixels[offset + 3] = 255;
        }
        var before = pixels.ToArray();
        var canvas = new SoftwareRasterCanvas(pixels, width, height);
        canvas.SaveLayer(new RasterLayerOptions(
            Bounds: new Rect(4, 4, 20, 20),
            BackdropFilter: new RasterImageFilter(RasterImageFilterKind.Blur, 3, 1, RasterTileMode.Clamp)));
        canvas.Restore();
        if (PixelDelta(before, pixels, width, 1, 1) != 0)
            throw new InvalidDataException("Backdrop blur changed pixels outside its layer bounds.");
        var beforeEnergy = HorizontalEnergy(before, width, 4, 20, 10);
        var afterEnergy = HorizontalEnergy(pixels, width, 4, 20, 10);
        if (afterEnergy >= beforeEnergy * 0.7)
            throw new InvalidDataException($"Anisotropic backdrop blur did not reduce horizontal edge energy: {beforeEnergy} -> {afterEnergy}.");
    }

    {
        var pixels = new byte[width * height * 4];
        var canvas = new SoftwareRasterCanvas(pixels, width, height);
        canvas.DrawColor(Color.FromArgb(255, 0, 255, 0));
        var before = pixels.ToArray();
        canvas.SaveLayer(new RasterLayerOptions(
            Bounds: new Rect(6, 6, 18, 18),
            BlendMode: RasterBlendMode.SourceAtop));
        canvas.DrawColor(Color.FromArgb(255, 255, 0, 0));
        canvas.Restore();
        AssertChannels(pixels, width, 10, 10, 0, 0, 255, 255, 1, "saveLayer srcATop interior");
        if (PixelDelta(before, pixels, width, 2, 2) != 0)
            throw new InvalidDataException("saveLayer bounds did not isolate the blend group.");
    }
}

static void RunRetainedLayerContracts()
{
    var recorder = new Doroti.Flutter.Ui.PictureRecorder();
    var canvas = new Doroti.Flutter.Ui.Canvas(recorder);
    canvas.drawRect(Doroti.Flutter.Ui.Rect.fromLTWH(0, 0, 12, 12), new Doroti.Flutter.Ui.Paint
    {
        color = new Doroti.Flutter.Ui.Color(0xff6750a4L),
    });
    var picture = recorder.endRecording();
    var first = new Doroti.Flutter.Ui.SceneBuilder(73);
    var retained = first.pushOffset(4, 5);
    first.addPicture(Doroti.Flutter.Ui.Offset.zero, picture);
    first.pop();
    using var firstScene = first.build();
    var second = new Doroti.Flutter.Ui.SceneBuilder(73);
    second.addRetained(retained);
    using var secondScene = second.build();
    if (secondScene.Commands is not [{ Operation: "retained" }])
        throw new InvalidDataException("Retained subtree was not recorded as one immutable replay command.");

    var crossViewRejected = false;
    try { new Doroti.Flutter.Ui.SceneBuilder(74).addRetained(retained); }
    catch (InvalidOperationException) { crossViewRejected = true; }
    if (!crossViewRejected) throw new InvalidDataException("Cross-view retained replay was not rejected.");

    retained.dispose();
    var disposedRejected = false;
    try { new Doroti.Flutter.Ui.SceneBuilder(73).addRetained(retained); }
    catch (ObjectDisposedException) { disposedRejected = true; }
    if (!disposedRejected) throw new InvalidDataException("Disposed retained replay was not rejected.");

    var unbalancedRejected = false;
    try
    {
        var unbalanced = new Doroti.Flutter.Ui.SceneBuilder(73);
        unbalanced.pushOffset(1, 1);
        _ = unbalanced.build();
    }
    catch (InvalidOperationException) { unbalancedRejected = true; }
    if (!unbalancedRejected) throw new InvalidDataException("Unbalanced scene effect scopes were not rejected.");
}

static int HorizontalEnergy(byte[] pixels, int width, int left, int right, int y)
{
    var energy = 0;
    for (var x = left + 1; x < right; x++)
    {
        var current = ((y * width) + x) * 4;
        var previous = current - 4;
        energy += Math.Abs(pixels[current] - pixels[previous]);
    }
    return energy;
}

static int PixelDelta(byte[] before, byte[] after, int width, int x, int y)
{
    var offset = ((y * width) + x) * 4;
    return Enumerable.Range(0, 4).Sum(channel => Math.Abs(before[offset + channel] - after[offset + channel]));
}

static void AssertChannels(byte[] pixels, int width, int x, int y, int blue, int green, int red, int alpha, int tolerance, string name)
{
    var offset = ((y * width) + x) * 4;
    var expected = new[] { blue, green, red, alpha };
    for (var channel = 0; channel < 4; channel++)
        if (Math.Abs(pixels[offset + channel] - expected[channel]) > tolerance)
            throw new InvalidDataException($"{name} failed at ({x},{y}) channel {channel}: expected {expected[channel]}, actual {pixels[offset + channel]}.");
}

static void RunBundledFontContracts()
{
    var root = FindRepositoryRoot();
    var assetRoot = Path.Combine(root, "Doroti", "src", "Doroti.Vendor.Avalonia.Skia", "Assets");
    var robotoPath = Path.Combine(assetRoot, "Roboto-Regular.ttf");
    var iconsPath = Path.Combine(assetRoot, "MaterialIcons-Regular.otf");
    var cupertinoIconsPath = Path.Combine(assetRoot, "CupertinoIcons.ttf");
    foreach (var path in new[] { robotoPath, iconsPath, cupertinoIconsPath, Path.Combine(assetRoot, "Roboto_LICENSE.txt"), Path.Combine(assetRoot, "MaterialIcons_LICENSE.txt"), Path.Combine(assetRoot, "CupertinoIcons_LICENSE.txt") })
        if (!File.Exists(path) || new FileInfo(path).Length == 0) throw new InvalidDataException($"Bundled font asset is missing: {path}");

    using var roboto = SKTypeface.FromFile(robotoPath) ?? throw new InvalidDataException("Roboto typeface could not be loaded.");
    using var icons = SKTypeface.FromFile(iconsPath) ?? throw new InvalidDataException("Material Icons typeface could not be loaded.");
    using var cupertinoIcons = SKTypeface.FromFile(cupertinoIconsPath) ?? throw new InvalidDataException("Cupertino Icons typeface could not be loaded.");
    using var robotoFont = new SKFont(roboto, 16);
    using var iconFont = new SKFont(icons, 24);
    using var cupertinoIconFont = new SKFont(cupertinoIcons, 24);
    if (robotoFont.GetGlyphs("August 2026").Any(glyph => glyph == 0))
        throw new InvalidDataException("Roboto does not cover the calendar reference text.");
    foreach (var codepoint in new[] { 0xe5c5, 0xe5cb, 0xe5cc, 0xe5c8 })
    {
        var text = char.ConvertFromUtf32(codepoint);
        if (iconFont.GetGlyphs(text) is not [var glyph] || glyph == 0)
            throw new InvalidDataException($"Material Icons codepoint U+{codepoint:X4} resolved to tofu.");
    }
    foreach (var codepoint in new[] { 0xf3cf, 0xf447, 0xf4a5, 0xf62d })
    {
        var text = char.ConvertFromUtf32(codepoint);
        if (cupertinoIconFont.GetGlyphs(text) is not [var glyph] || glyph == 0)
            throw new InvalidDataException($"Cupertino Icons codepoint U+{codepoint:X4} resolved to tofu.");
    }
    var qualifiedFamilies = SkiaTextMeasurer.ResolveFallbackFamilies(
        char.ConvertFromUtf32(0xf3cf),
        fontFamily: "packages/cupertino_icons/CupertinoIcons");
    if (!qualifiedFamilies.Contains("CupertinoIcons", StringComparer.OrdinalIgnoreCase))
        throw new InvalidDataException($"Package-qualified Cupertino icon family resolved through {string.Join(", ", qualifiedFamilies)}.");
}

static string FindRepositoryRoot()
{
    for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        if (File.Exists(Path.Combine(directory.FullName, "goal7.md")) &&
            File.Exists(Path.Combine(directory.FullName, "Doroti", "Doroti.slnx")))
            return directory.FullName;
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
