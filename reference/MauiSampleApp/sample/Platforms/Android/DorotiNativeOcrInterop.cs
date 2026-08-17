using DorotiOcrAndroid;

namespace MauiSampleApp;

public static partial class DorotiNativeOcrInterop
{
    static DorotiNativeOcrInterop() => EngineName = "ML Kit text-recognition 16.0.1 (Maven)";

    private static partial Task<string> RecognizePlatformAsync(byte[] imageBytes, string script) =>
        DorotiNativeOcr.RecognizeAsync(imageBytes, script);
}
