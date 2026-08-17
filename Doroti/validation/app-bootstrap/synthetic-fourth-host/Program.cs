using Doroti.Hosting;

namespace SyntheticFourthHost;

public sealed class Program : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => _ = builder;
}
