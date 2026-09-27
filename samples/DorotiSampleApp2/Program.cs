using Doroti.Hosting;

namespace DorotiSampleApp2;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) =>
        builder.UseEntrypoint(App.Definition).UseView(App.ViewConfiguration);
}
