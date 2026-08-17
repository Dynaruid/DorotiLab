using Foundation;

namespace DorotiOcrMaciOS;

public partial class DorotiNativeOcr
{
    public static async Task<string> RecognizeAsync(byte[] imageBytes, string script = "auto")
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        using var data = NSData.FromArray(imageBytes);
        var text = await RecognizeAsync(data, script);
        return text?.ToString() ?? string.Empty;
    }
}
