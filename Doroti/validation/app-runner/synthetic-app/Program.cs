using Doroti.Hosting;

namespace SyntheticApp;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => _ = builder;
}
