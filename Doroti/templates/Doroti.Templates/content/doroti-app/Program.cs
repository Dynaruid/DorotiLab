using Doroti.Hosting;

namespace DorotiTemplateApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(App.Definition)
        .UseView(App.ViewConfiguration);
}
