using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

[SupportedOSPlatform("browser")]
public static partial class Probe
{
    public static void Main() { }

#if DOROTI_SHAPED_PARAGRAPHS
    [JSExport]
    public static async Task<string> RunProduct(byte[] arabic, byte[] devanagari, byte[] thai, byte[] emoji,
        byte[] lao, byte[] khmer, byte[] burmese)
    {
        var fonts = new Dictionary<string, byte[]>(StringComparer.Ordinal)
        {
            ["Arabic"] = arabic,
            ["Devanagari"] = devanagari,
            ["Thai"] = thai,
            ["Emoji"] = emoji,
            ["Lao"] = lao,
            ["Khmer"] = khmer,
            ["Burmese"] = burmese,
        };
        var result = await ProductParagraphProbe.RunAsync(name => fonts[name]);
        return result.Receipt + "\n" + Convert.ToBase64String(result.Png);
    }
#else
    [JSExport]
    public static Task<string> Run(byte[] arabic, byte[] devanagari, byte[] thai, byte[] emoji)
    {
        byte[] ReadFont(string name) => name switch
        {
            "Arabic" => arabic,
            "Devanagari" => devanagari,
            "Thai" => thai,
            "Emoji" => emoji,
            _ => throw new ArgumentOutOfRangeException(nameof(name)),
        };
        var result = ParagraphProbe.Run(ReadFont);
        return Task.FromResult(result.Receipt + "\n" + Convert.ToBase64String(result.Png));
    }
#endif
}
