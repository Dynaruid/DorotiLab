using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace MauiSampleApp;

public static partial class DorotiNativeOcrInterop
{
    static DorotiNativeOcrInterop() => EngineName = "Windows.Media.Ocr";

    private static async partial Task<string> RecognizePlatformAsync(byte[] imageBytes, string script)
    {
        _ = script;
        var engine = OcrEngine.TryCreateFromUserProfileLanguages()
            ?? throw new InvalidOperationException("No Windows OCR language pack is installed for the current user.");

        using var stream = new InMemoryRandomAccessStream();
        await stream.WriteAsync(imageBytes.AsBuffer());
        stream.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(stream);
        using var bitmap = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        var result = await engine.RecognizeAsync(bitmap);
        return result.Text?.Trim() ?? string.Empty;
    }
}
