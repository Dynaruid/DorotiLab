using Doroti.Skia.Rendering;

namespace Doroti.Skia.Fonts;

/// <summary>Native hosts register these bundled faces before their first layout.</summary>
public static class NativeDefaultFonts
{
    private static readonly Lazy<byte[][]> FontData = new(() =>
        new[] { "regular", "medium", "bold" }.Select(weight =>
        {
            using var stream = typeof(NativeDefaultFonts).Assembly.GetManifestResourceStream(
                $"Doroti.Skia.Fonts.Roboto-{weight}.ttf")
                ?? throw new InvalidOperationException("Bundled Roboto font is missing.");
            using var bytes = new MemoryStream();
            stream.CopyTo(bytes);
            return bytes.ToArray();
        }).ToArray());

    public static void Register(SkiaSceneRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        foreach (var bytes in FontData.Value)
            renderer.RegisterFontAsync(bytes, "Roboto").GetAwaiter().GetResult();
    }
}
