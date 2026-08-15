using System.Runtime.Versioning;
using Doroti.Host.Web;
using Doroti.Target.Web;
using DorotiApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

[assembly: SupportedOSPlatform("browser")]

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<DorotiRoot>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<IDorotiBrowserTarget, BrowserWasmTarget>();
builder.Services.AddSingleton(new DorotiWebApplication(
    DorotiApplication.CreateEntrypoint,
    typeof(DorotiApplication).Assembly,
    DorotiApplication.ViewConfiguration,
    [new("echo", "doroti.example/echo", "doroti.browser-js-plugin/v1", "./plugins/echo.js", "invoke")]));
await builder.Build().RunAsync();
