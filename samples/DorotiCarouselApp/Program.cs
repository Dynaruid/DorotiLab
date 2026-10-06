using Doroti.Hosting;

namespace DorotiCarouselApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) =>
        builder.UseEntrypoint(App.Definition).UseView(App.ViewConfiguration);
}
