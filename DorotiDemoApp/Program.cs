using Doroti.Hosting;

namespace DorotiDemoApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(global::App.Definition)
        .UseView(global::App.ViewConfiguration);
}
