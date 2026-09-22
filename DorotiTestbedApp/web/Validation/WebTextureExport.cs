using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using MaterialSample;

namespace DorotiTestbedApp.Web.Validation;

[SupportedOSPlatform("browser")]
public static partial class WebTextureExport
{
    [JSExport]
    public static Task Initialize()
    {
        TextureSampleProbe.SelectSource = async operation =>
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

    private static Task<T> OnOwner<T>(Func<Task<T>> action)
    {
        var result = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        var owner =
            TextureSampleProbe.Owner
            ?? throw new InvalidOperationException("Texture sample is not mounted.");
        var context =
            TextureSampleProbe.OwnerContext
            ?? throw new InvalidOperationException("Texture owner context is unavailable.");
        context.Post(
            async _ =>
            {
                try
                {
                    Task<T>? task = null;
                    owner.DispatchPlatformEvent(() => task = action());
                    result.SetResult(await task!);
                }
                catch (Exception error)
                {
                    result.SetException(error);
                }
            },
            null
        );
        return result.Task;
    }

    [JSExport]
    public static Task<string> LocalCanvas(string operation) =>
        OnOwner(async () =>
        {
            await JSHost.ImportAsync(
                "texture-local-example",
                ResolveUrl("./plugins/texture-local.js")
            );
            return await LocalCanvasCommand(operation);
        });

    [JSImport("resolveResourceUrl", "doroti.web")]
    private static partial string ResolveUrl(string url);

    [JSImport("command", "texture-local-example")]
    private static partial Task<string> LocalCanvasCommand(string operation);

    [JSExport]
    public static Task<int> Show(string id, bool freeze, int effect) =>
        OnOwner(() =>
        {
            TextureSampleProbe.SetTexture!(
                long.Parse(id, System.Globalization.CultureInfo.InvariantCulture),
                freeze,
                effect
            );
            return Task.FromResult(TextureSampleProbe.Builds);
        });

    [JSExport]
    public static Task<int> Builds() => Task.FromResult(TextureSampleProbe.Builds);
}
