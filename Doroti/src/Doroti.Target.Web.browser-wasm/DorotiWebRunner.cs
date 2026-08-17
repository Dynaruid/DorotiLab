using Doroti.Host.Web;
using Doroti.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Doroti.Target.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public static class DorotiWebRunner
{
    public static async Task RunAsync<TStartup>(
        string[] args,
        IEnumerable<DorotiApplicationPluginRegistration>? plugins = null)
        where TStartup : IDorotiApplicationStartup, new()
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var descriptor = DorotiApplicationFactory.Create<TStartup>(
            DorotiLaunchContext.Create("Web", "browser-wasm", args, new(builder.HostEnvironment.BaseAddress)),
            plugins);
        builder.RootComponents.Add<DorotiRoot>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
        builder.Services.AddSingleton<IDorotiBrowserTarget, BrowserWasmTarget>();
        builder.Services.AddSingleton(descriptor);
        await builder.Build().RunAsync();
    }
}
