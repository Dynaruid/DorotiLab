using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using MaterialSample;

namespace DorotiTestbedApp.Web;

[SupportedOSPlatform("browser")]
public static partial class WebTextureSources
{
    [JSExport]
    public static Task Initialize()
    {
        TextureSampleSources.SelectSource = async operation =>
        {
            var response = await InvokeTextureSample(
                "./plugins/textures.js",
                "invoke",
                "texture-sample",
                "json",
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(operation))
            );
            using var json = System.Text.Json.JsonDocument.Parse(response);
            return System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(json.RootElement.GetProperty("base64").GetString()!)
            );
        };
        return Task.CompletedTask;
    }

    [JSImport("invokePlugin", "doroti.web")]
    private static partial Task<string> InvokeTextureSample(
        string moduleUrl,
        string exportName,
        string channel,
        string codec,
        string payloadBase64
    );

}
