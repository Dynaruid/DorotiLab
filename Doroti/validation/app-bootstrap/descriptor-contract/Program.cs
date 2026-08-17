using Doroti.Hosting;
using Doroti.Ui;

var launch = DorotiLaunchContext.Create("Contract", "synthetic-contract", ["one"], new("https://doroti.invalid/"));
var plugin = new DorotiApplicationPluginRegistration("echo", "doroti.example/echo", "contract", "module", "invoke");
var descriptor = DorotiApplicationFactory.Create<ContractStartup>(launch, [plugin]);
if (descriptor.ApplicationAssembly != typeof(ContractStartup).Assembly ||
    descriptor.LaunchContext != launch ||
    descriptor.PluginRegistrations.Single() != plugin ||
    descriptor.ViewConfiguration.title != "contract" ||
    descriptor.EntrypointFactory() is not ContractEntrypoint)
    throw new InvalidOperationException("The Doroti application descriptor lost startup data.");

var duplicateFailed = false;
try
{
    _ = DorotiApplicationFactory.Create<ContractStartup>(launch, [plugin, plugin]);
}
catch (InvalidOperationException)
{
    duplicateFailed = true;
}
if (!duplicateFailed) throw new InvalidOperationException("Duplicate plugin registration did not fail closed.");

Console.WriteLine("Doroti application descriptor contract: PASS");

public sealed class ContractStartup : IDorotiApplicationStartup
{
    public void Configure(DorotiApplicationBuilder builder) => builder
        .UseEntrypoint(() => new ContractEntrypoint())
        .UseView(new DorotiViewConfiguration("contract", new Size(640, 480)));
}

public sealed class ContractEntrypoint : IDorotiViewEntrypoint
{
    public void Bootstrap(PlatformDispatcher dispatcher) => _ = dispatcher;
    public void Shutdown() { }
    public void AttachView(DorotiView view) => _ = view;
    public void DetachView(DorotiView view) => _ = view;
}
