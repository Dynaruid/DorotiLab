using Doroti.Host.Qt;
using Doroti.Ui;
using SkiaSharp;
using System.Runtime.InteropServices;

internal static class WindowAppearanceContracts
{
    internal static void Verify()
    {
        var acrylic = new WindowBackdropOptions(WindowBackdropMode.acrylic);
        var glass = new WindowBackdropOptions(WindowBackdropMode.liquidGlass);
        var appearance = new WindowAppearanceOptions(acrylic, macOSBackdrop: glass);
        Check(appearance.titlebarStyle == WindowTitlebarStyle.unified, "Unified is not the default.");
        Check(appearance.ResolveBackdrop(true) == glass && appearance.ResolveBackdrop(false) == acrylic,
            "macOS material override leaked to other desktops.");
        var legacy = new DorotiViewConfiguration("legacy", new(100, 100), backdrop: acrylic);
        Check(legacy.ResolveAppearance().ResolveBackdrop(true) == acrylic, "Legacy backdrop was lost.");
        var explicitAppearance = legacy with { appearance = appearance with { titlebarStyle = WindowTitlebarStyle.solid } };
        Check(explicitAppearance.ResolveAppearance().ResolveBackdrop(true) == glass &&
            explicitAppearance.ResolveAppearance().titlebarStyle == WindowTitlebarStyle.solid,
            "Explicit appearance did not take precedence.");
        Check((legacy with { appearance = new() }).ResolveAppearance().ResolveBackdrop(false).mode == WindowBackdropMode.system,
            "An explicit system appearance unexpectedly inherited the legacy material.");

        var bytes = new byte[Marshal.SizeOf<QtNativeV2.Surface>()];
        BitConverter.GetBytes(800).CopyTo(bytes, 28);
        BitConverter.GetBytes(2d).CopyTo(bytes, 40);
        BitConverter.GetBytes(32u).CopyTo(bytes, 128);
        BitConverter.GetBytes(2u).CopyTo(bytes, 132);
        BitConverter.GetBytes(3u).CopyTo(bytes, 136);
        var descriptor = MemoryMarshal.Read<QtNativeV2.Surface>(bytes);
        using var target = SKSurface.Create(new SKImageInfo(800, 240));
        target.Canvas.Clear(SKColors.Blue);
        target.Canvas.Translate(7, 11);
        var matrix = target.Canvas.TotalMatrix;
        var saveCount = target.Canvas.SaveCount;
        var invalidations = 0;
        var caption = new QtTitlebarAppearance(() => invalidations++);
        var light = new WindowTitlebarTheme(new Color(0x99fffbfe), Brightness.light);
        var dark = new WindowTitlebarTheme(new Color(0x99141218), Brightness.dark);
        caption.SetTheme(light);
        caption.SetTheme(light with { });
        Check(invalidations == 1, "An unchanged caption theme requested another frame.");
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor, caption.Theme);
        Check(target.Canvas.TotalMatrix == matrix && target.Canvas.SaveCount == saveCount,
            "Caption painting leaked canvas state.");
        using var pixels = target.PeekPixels();
        CheckColor(pixels.GetPixelColor(300, 20), new SKColor(0x99fffbfe), "Light caption tint differs from the application surface.");
        Check(pixels.GetPixelColor(300, 100) == SKColors.Blue, "Caption painting damaged the client surface.");
        Check(pixels.GetPixelColor(716, 8).Red > 180, "Close hover feedback is missing at Retina scale.");
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor, caption.Theme);
        CheckColor(pixels.GetPixelColor(300, 20), new SKColor(0x99fffbfe), "Caption tint accumulated on repaint.");
        caption.SetTheme(dark);
        Check(invalidations == 2, "A theme switch did not request a frame.");
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor, caption.Theme);
        CheckColor(pixels.GetPixelColor(300, 20), new SKColor(0x99141218), "Dark caption tint did not update.");
        Check(pixels.GetPixelColor(562, 40).Red > 180, "Caption icons followed the system light theme instead of the application dark theme.");
        caption.SetTheme(new(new Color(0xffd0e8d5), Brightness.light));
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor, caption.Theme);
        CheckColor(pixels.GetPixelColor(300, 20), new SKColor(0xffd0e8d5), "Opaque/custom application surface was not preserved.");
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor);
        Check(pixels.GetPixelColor(300, 20).Alpha == 153, "An unthemed caption has no translucent fallback tint.");
        BitConverter.GetBytes(0u).CopyTo(bytes, 128);
        descriptor = MemoryMarshal.Read<QtNativeV2.Surface>(bytes);
        target.Canvas.Clear(SKColors.Blue);
        QtTitlebarPainter.Paint(target.Canvas, "Doroti", descriptor, caption.Theme);
        Check(pixels.GetPixelColor(300, 20) == SKColors.Blue, "A hidden/native caption painted over the client surface.");
        Console.WriteLine("Window appearance routing and Qt caption compositing: PASS");
    }

    private static void CheckColor(SKColor actual, SKColor expected, string message) =>
        Check(actual.Alpha == expected.Alpha && Math.Abs(actual.Red - expected.Red) <= 1 &&
            Math.Abs(actual.Green - expected.Green) <= 1 && Math.Abs(actual.Blue - expected.Blue) <= 1, message);

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
