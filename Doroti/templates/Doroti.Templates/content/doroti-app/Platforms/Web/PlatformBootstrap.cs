using System.Runtime.Versioning;
using Doroti.Host.Web;
using Doroti.Target.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace DorotiApp.Platforms.Web;

[SupportedOSPlatform("browser")]
internal static class PlatformBootstrap
{
    internal static async Task RunAsync(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<DorotiRoot>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddSingleton<IDorotiBrowserTarget, BrowserWasmTarget>();
        builder.Services.AddSingleton(new DorotiWebApplication(
            App.Definition,
            typeof(App).Assembly,
            App.ViewConfiguration,
            [new("echo", "doroti.example/echo", "doroti.browser-js-plugin/v1", "./plugins/echo.js", "invoke")]));
        await builder.Build().RunAsync();
    }
}
