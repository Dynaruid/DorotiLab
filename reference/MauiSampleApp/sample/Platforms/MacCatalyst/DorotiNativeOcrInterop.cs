using DorotiOcrMaciOS;

namespace MauiSampleApp;

public static partial class DorotiNativeOcrInterop
{
    static DorotiNativeOcrInterop() => EngineName = "Apple Vision";

    private static partial Task<string> RecognizePlatformAsync(byte[] imageBytes, string script) =>
        DorotiNativeOcr.RecognizeAsync(imageBytes, script);
}
