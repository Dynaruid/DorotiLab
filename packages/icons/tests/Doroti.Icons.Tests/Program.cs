using System.Reflection;
using Doroti.Cupertino;
using Doroti.Framework.Widgets;
using Doroti.Material;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;
using Path = System.IO.Path;

var output = Path.GetFullPath(args.FirstOrDefault() ?? "temp/testing/icon-packages/rendering");
Directory.CreateDirectory(output);
var material = new[] { Icons.arrow_back, Icons.arrow_forward, Icons.home, Icons.zoom_in_map };
var cupertino = new[] { CupertinoIcons.heart, CupertinoIcons.heart_fill, CupertinoIcons.person_2_fill };
// The same icon/font descriptors must register in a newly created View after the previous View is disposed.
for (var view = 0; view < 2; view++)
{
    using var tester = new WidgetTester(new Size((material.Length + cupertino.Length + 1) * 56, 72));
    IconData? adaptive = null;
    tester.View.DispatchPlatformEvent(() => adaptive = Icons.adaptive.arrow_back);
    var icons = material.Concat([adaptive!]).Concat(cupertino).ToArray();
    tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr,
        child: new Row(children: icons.Select(icon => (Widget)new SizedBox(width: 56, height: 72,
            child: new Icon(icon, size: 40, color: new Color(0xff000000)))).ToList())));
    tester.pumpAndSettle();
    var capture = Path.Combine(output, $"icons-view-{view}.png");
    tester.WritePng(capture);
    using var bitmap = SKBitmap.Decode(capture);
    for (var icon = 0; icon < icons.Length; icon++)
    {
        var ink = 0;
        for (var y = 0; y < 72; y++)
            for (var x = icon * 56; x < (icon + 1) * 56; x++)
                if (bitmap.GetPixel(x, y).Red < 120) ink++;
        Require(ink > 25, $"Icon {icon} is blank in View {view}.");
    }
}
Require(!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name is "Doroti.Material" or "Doroti.Cupertino"),
    "Icons-only consumer loaded design widgets.");

var repo = FindRepository();
foreach (var (identity, original, used, unused) in new[]
{
    ("Doroti.Icons.Material", "packages/Doroti.Icons.Material/assets/MaterialIcons.otf", material, 0xe3af),
    ("Doroti.Icons.Cupertino", "packages/Doroti.Icons.Cupertino/assets/CupertinoIcons.ttf", cupertino, 0xf447),
})
{
    var assembly = Assembly.Load(identity);
    using var stream = assembly.GetManifestResourceStream(identity + ".font")!;
    using var memory = new MemoryStream(); stream.CopyTo(memory);
    using var source = SKTypeface.FromFile(Path.Combine(repo, original));
    using var subset = SKTypeface.FromData(SKData.CreateCopy(memory.ToArray()));
    Require(subset is not null, "Subset font cannot be decoded.");
    foreach (var icon in used)
    {
        var text = char.ConvertFromUtf32((int)icon.codePoint);
        Require(Raster(source!, text).SequenceEqual(Raster(subset!, text)), $"Subset changed U+{icon.codePoint:X} pixels.");
    }
    using var font = new SKFont(subset!, 40);
    Require(font.GetGlyphs(char.ConvertFromUtf32(unused)).All(g => g == 0), "Unused codepoint remains in the subset cmap.");
}
Console.WriteLine("PASS: icons-only dependencies, lazy registration in two Views, original/subset glyph pixels, non-BMP and unused glyph removal.");

static byte[] Raster(SKTypeface face, string text)
{
    using var surface = SKSurface.Create(new SKImageInfo(80, 80));
    surface.Canvas.Clear(SKColors.White);
    using var font = new SKFont(face, 48);
    using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
    surface.Canvas.DrawText(text, 10, 60, SKTextAlign.Left, font, paint);
    using var image = surface.Snapshot();
    using var bytes = image.Encode(SKEncodedImageFormat.Png, 100);
    return bytes.ToArray();
}
static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static string FindRepository()
{
    var directory = new DirectoryInfo(Environment.CurrentDirectory);
    while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "packages/Doroti.Icons.Material/assets"))) directory = directory.Parent;
    return directory?.FullName ?? throw new InvalidOperationException("Run this regression from the repository.");
}
