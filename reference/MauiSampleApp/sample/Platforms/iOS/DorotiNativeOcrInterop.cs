using DorotiOcrMaciOS;
using Foundation;

namespace MauiSampleApp;

public static partial class DorotiNativeOcrInterop
{
    static DorotiNativeOcrInterop() => EngineName = "Apple Vision";

    private static async partial Task<string> RecognizePlatformAsync(byte[] imageBytes, string script)
    {
        using var data = NSData.FromArray(imageBytes);
        var text = await DorotiNativeOcr.RecognizeAsync(data, script);
        return text ?? string.Empty;
    }
}
