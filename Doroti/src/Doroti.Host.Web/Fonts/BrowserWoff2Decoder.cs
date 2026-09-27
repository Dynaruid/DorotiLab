using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
internal static partial class BrowserWoff2Decoder
{
    private static Task<JSObject>? _module;
    internal static async Task<byte[]> DecodeAsync(byte[] bytes, Uri decoderUrl, CancellationToken cancellationToken)
    {
        if (bytes.Length < 4 || bytes[0] != 'w' || bytes[1] != 'O' || bytes[2] != 'F' || bytes[3] != '2')
            return bytes;
        try
        {
            _module ??= JSHost.ImportAsync("doroti.fonts", "../_content/Doroti.Host.Web/doroti.web.fonts.js");
            await _module;
            cancellationToken.ThrowIfCancellationRequested();
            using var decoded = await Decode(bytes, decoderUrl.AbsoluteUri);
            cancellationToken.ThrowIfCancellationRequested();
            return CopyDecoded(decoded);
        }
        catch (JSException error)
        {
            _module = null;
            throw new InvalidDataException("WOFF2 decoding failed.", error);
        }
    }

    [JSImport("decode", "doroti.fonts")]
    [return: JSMarshalAs<JSType.Promise<JSType.Object>>]
    private static partial Task<JSObject> Decode([JSMarshalAs<JSType.Array<JSType.Number>>] byte[] bytes, string decoderUrl);

    [JSImport("copyDecoded", "doroti.fonts")]
    [return: JSMarshalAs<JSType.Array<JSType.Number>>]
    private static partial byte[] CopyDecoded(JSObject bytes);
}
