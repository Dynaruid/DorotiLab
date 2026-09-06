using System.Runtime.InteropServices.JavaScript;
using Doroti.Framework.Painting;
using Doroti.Validation;

namespace DorotiTestbedApp.Web;

// Only compiled when DorotiImageValidation=true. Calls shared public APIs in the live UI Worker.
[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public static partial class ImagePipelineExport
{
    [JSExport]
    public static async Task<string> Run(string source)
    {
        using var scope = PaintingBinding.instance.platformDispatcher.EnterScope();
        await ImagePipelineValidation.VerifyPixels();
        await ImagePipelineValidation.VerifyProviderFailure();
        using var client = new HttpClient();
        var bytes = source.StartsWith("base64:", StringComparison.Ordinal)
            ? Convert.FromBase64String(source[7..]) : await client.GetByteArrayAsync(source);
        return await ImagePipelineValidation.Inspect(bytes, realProvider: true,
            networkUrl: source.StartsWith("base64:", StringComparison.Ordinal) ? null : source);
    }
}
