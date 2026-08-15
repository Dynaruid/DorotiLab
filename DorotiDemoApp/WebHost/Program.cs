using System.Runtime.Versioning;
using Doroti.Host.Web;
using Doroti.Target.Web;
using Doroti.Ui;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

[assembly: SupportedOSPlatform("browser")]

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<DorotiRoot>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<IDorotiBrowserTarget, BrowserWasmTarget>();
builder.Services.AddSingleton(new DorotiWebApplication(
    () => new MaterialDemoEntrypoint(DemoEntryMode.Home, requireExternalUia: false),
    typeof(MaterialDemoEntrypoint).Assembly,
    new DorotiViewConfiguration("Doroti Material Demo", new Size(720, 640)),
    [new("echo", "doroti.example/echo", "doroti.browser-js-plugin/v1", "./plugins/echo.js", "invoke")]));
await builder.Build().RunAsync();
