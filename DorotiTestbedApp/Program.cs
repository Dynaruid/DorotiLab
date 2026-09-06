using Doroti.Hosting;

namespace DorotiTestbedApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(global::App.Definition)
        .UseView(global::App.ViewConfiguration);
}
