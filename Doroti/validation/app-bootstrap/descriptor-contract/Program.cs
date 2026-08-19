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

var nativeBridge = new ContractNativeBridge();
var nativeDescriptor = DorotiApplicationFactory.Create<ContractStartup>(launch, nativePluginHandlers: [nativeBridge]);
if (nativeDescriptor.NativePluginHandlers.Single() != nativeBridge ||
    nativeBridge.PlatformInfo().BridgeVersion != DorotiNativePlatformBridgeContract.BridgeVersion ||
    nativeBridge.Echo("native") != "native" ||
    await nativeBridge.EchoOnUiThreadAsync("ui") != "ui")
    throw new InvalidOperationException("The default native platform bridge contract lost startup or echo data.");

var request = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new { method = "echo", value = "channel" });
var response = await nativeBridge.HandleAsync(DorotiNativePlatformBridgeContract.Channel, "json", request);
if (response is null || System.Text.Json.JsonSerializer.Deserialize<string>(response.Value.Span) != "channel")
    throw new InvalidOperationException("The native platform channel did not round-trip its payload.");

await AssertThrowsAsync<MissingMethodException>(() =>
    nativeBridge.HandleAsync(DorotiNativePlatformBridgeContract.Channel, "json",
        System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new { method = "missing" })).AsTask());
await AssertThrowsAsync<InvalidOperationException>(() => nativeBridge.EchoOnUiThreadAsync("throw").AsTask());

Console.WriteLine("Doroti application descriptor contract: PASS");

static async Task AssertThrowsAsync<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name} was not propagated.");
}

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

public sealed class ContractNativeBridge : DorotiNativePlatformBridgeBase
{
    public override DorotiNativePlatformInfo PlatformInfo() =>
        DorotiNativePlatformInfo.Parse("{\"platform\":\"Contract\",\"osVersion\":\"1\",\"bridgeVersion\":\"1.0.0\"}");

    public override string Echo(string value) => value;

    public override ValueTask<string> EchoOnUiThreadAsync(string value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (value == "throw") throw new InvalidOperationException("contract exception");
        return ValueTask.FromResult(value);
    }
}
