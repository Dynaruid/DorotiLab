using Doroti.Hosting;

namespace DorotiTestbedApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(App.Definition)
        .UseView(App.ViewConfiguration);
}
