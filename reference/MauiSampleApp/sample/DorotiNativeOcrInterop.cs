namespace MauiSampleApp;

public static partial class DorotiNativeOcrInterop
{
    public static string EngineName { get; private set; } = "unavailable";

    public static Task<string> RecognizeAsync(byte[] imageBytes, string script = "auto")
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        return RecognizePlatformAsync(imageBytes, script);
    }

    private static partial Task<string> RecognizePlatformAsync(byte[] imageBytes, string script);
}
