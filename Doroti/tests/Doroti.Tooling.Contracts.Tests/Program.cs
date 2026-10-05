using System.Text.Json;
using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;

static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
static async Task Reject<T>(Func<Task> action) where T : Exception { try { await action(); } catch (T) { return; } throw new InvalidOperationException($"Expected {typeof(T).Name}."); }
var descriptor = new ProviderDescriptor("doroti.platform-provider/v1", "test-headless", "0.4.0-alpha.1", "[0.4.0-alpha.1,0.5.0)", 1, "headless", ["windows", "linux", "macos"], ["build", "run"], Enum.GetValues<ToolService>(), new(ToolMode.DotnetInproc, "", "TestHeadless.Extension", null, []));
var context = new ToolContext(Environment.CurrentDirectory, "fixture.csproj", "test-headless", "test-headless", "default", "host-process", 9007199254740993L);
Require(ToolVersionRange.Contains("[0.4.0-alpha.1,0.5.0)", "0.4.0-alpha.10"), "Numeric prerelease ordering diverged.");
Require(!ToolVersionRange.Contains("[0.4.0-alpha.1,0.5.0)", "0.5.0"), "Upper range was accepted.");
Require(!ToolVersionRange.Contains("[0.4.0-alpha.2,0.5.0)", "0.4.0-alpha.1"), "Lower range was accepted.");
await Reject<ToolContractException>(() => { ToolContract.Validate(new("test-headless", "0.4.0-alpha.1", 1, "[0.5.0,1.0.0)", "net10.0", ["windows", "linux", "macos"], Enum.GetValues<ToolService>()), descriptor with { CoreRange = "[0.5.0,1.0.0)" }); return Task.CompletedTask; });
Require(ToolVersionRange.Contains("[0.4.0-alpha.999999999999999999999999999999,0.5.0)", "0.4.0-alpha.1000000000000000000000000000000"), "Large numeric prerelease ordering overflowed.");
foreach (var invalidVersion in new[] { "0.4.0+", "0.4.0+a..b", "0.4.0+a+b", "0.4.0+bad_underscore" })
    await Reject<ToolContractException>(() => { ToolVersionRange.Contains("[0.4.0,0.5.0)", invalidVersion); return Task.CompletedTask; });
TestHeadless.Extension.Activations = 0; TestHeadless.Extension.Disposals = 0;
await using (var lifetime = new ToolExtensionLifetime(descriptor, _ => ValueTask.FromResult<IDorotiToolExtension>(new TestHeadless.Extension())))
{
    var requests = Enumerable.Range(0, 12).Select(_ => lifetime.InvokeAsync(ToolService.Devices, 1, (extension, token) => extension.GetDevicesAsync(context, token))).ToArray();
    await Task.WhenAll(requests);
    Require(TestHeadless.Extension.Activations == 1, "Concurrent activation created duplicate extensions.");
    Require(requests.All(x => x.Result.Devices.Single().Id == "host-process"), "Typed result changed.");
    using var cancel = new CancellationTokenSource(30);
    await Reject<OperationCanceledException>(() => lifetime.InvokeAsync(ToolService.Configuration, 1, (extension, token) => extension.ConfigureAsync(new(context, [new("delay", "30000")]), token), cancel.Token));
    await Reject<ToolContractException>(() => lifetime.InvokeAsync(ToolService.Configuration, 1, (extension, token) => extension.ConfigureAsync(new(context, [new("delay", "-1")]), token)));
}
Require(TestHeadless.Extension.Disposals == 1, "Shared extension disposed multiple times.");
var assembly = typeof(TestHeadless.Extension).Assembly.Location;
await using (var loader = new ToolAssemblyLoader(descriptor, assembly))
{
    var devices = await loader.Lifetime.InvokeAsync(ToolService.Devices, 1, (extension, token) => extension.GetDevicesAsync(context, token));
    Require(devices.Devices.Single().Id == "host-process", "ALC Contracts identity diverged.");
}
var cwd = Path.GetDirectoryName(assembly)!;
await using (var process = new ProcessToolExtension(new("dotnet", [assembly, "--process-server"], cwd, []), TimeSpan.FromSeconds(5)))
{
    ToolContract.Validate(await process.GetCapabilitiesAsync(default), descriptor);
    Require((await process.GetDevicesAsync(context, default)).Devices.Single().Id == "host-process", "Process result differs from typed result.");
    var values = new ConfigurationRequest(context, [new("trace", "false")]);
    Require((await process.ConfigureAsync(values, default)).Values.Single().Value == "false", "Process config lost fields.");
    using var cancel = new CancellationTokenSource(30);
    await Reject<OperationCanceledException>(() => process.ConfigureAsync(new(context, [new("delay", "30000")]), cancel.Token).AsTask());
    await Reject<ToolContractException>(() => process.ConfigureAsync(new(context, [new("delay", "-1")]), default).AsTask());
    Require((await process.GetDevicesAsync(context, default)).Devices.Count == 1, "Cancellation poisoned next invocation.");
}
await using (var crash = new ProcessToolExtension(new("dotnet", [assembly, "--crash-server"], cwd, []), TimeSpan.FromSeconds(2)))
    await Reject<Exception>(() => crash.GetCapabilitiesAsync(default).AsTask());
await using (var hung = new ProcessToolExtension(new("dotnet", [assembly, "--hung-server"], cwd, []), TimeSpan.FromMilliseconds(100)))
    await Reject<OperationCanceledException>(() => hung.GetCapabilitiesAsync(default).AsTask());
// Malformed/truncated/oversized frames and duplicate properties fail at the external boundary.
foreach (var bad in new[] { "Content-Length: 4194305\r\n\r\n", "Content-Length: -1\r\n\r\n", "Content-Length: 5\r\n\r\n{}", "Wrong: 2\r\n\r\n{}" })
    await Reject<Exception>(async () => { await ToolFrames.ReadAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(bad)), default); });
await Reject<InvalidDataException>(() => { ToolFrames.ValidateJson("{\"id\":\"1\",\"id\":\"2\"}"u8); return Task.CompletedTask; });
var wire = new WireRequest("2.0", "9007199254740993", "device.list", new(Context: context));
var encoded = JsonSerializer.SerializeToUtf8Bytes(wire, ToolWireJson.Context.WireRequest);
Require(System.Text.Encoding.UTF8.GetString(encoded).Contains("\"generation\":\"9007199254740993\""), "Int64 was emitted as a JSON number.");
await Reject<JsonException>(() => { JsonSerializer.Deserialize(System.Text.Encoding.UTF8.GetString(encoded).Replace("\"9007199254740993\"", "9007199254740993"), ToolWireJson.Context.WireRequest); return Task.CompletedTask; });
await Reject<JsonException>(() => { JsonSerializer.Deserialize("{\"mode\":999,\"assembly\":\"\",\"entryType\":\"\",\"executable\":null,\"arguments\":[]}", ToolWireJson.Context.GetTypeInfo(typeof(ToolEntry))!); return Task.CompletedTask; });
foreach (var invalidInt64 in new[] { "+9007199254740993", "09007199254740993", "-0", "9223372036854775808" })
    await Reject<JsonException>(() => { JsonSerializer.Deserialize(System.Text.Encoding.UTF8.GetString(encoded).Replace("\"generation\":\"9007199254740993\"", "\"generation\":\"" + invalidInt64 + "\""), ToolWireJson.Context.WireRequest); return Task.CompletedTask; });
Require(JsonSerializer.Deserialize(encoded, ToolWireJson.Context.WireRequest)!.Params.Context!.Generation == context.Generation, "64-bit generation lost precision.");
await using (var session = new ExecutionSession())
{
    var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var run = session.ExecuteAsync(new("stop-probe", [new("dotnet", [assembly, "--child", "--wait"], cwd, [])]), line => { if (line == "child-started") started.TrySetResult(); });
    await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
    var pid = session.ProcessId;
    // Independent connection shutdown does not own the application's process.
    await using (var connection = new ProcessToolExtension(new("dotnet", [assembly, "--process-server"], cwd, []))) await connection.GetCapabilitiesAsync(default);
    Require(session.ProcessId == pid && !run.IsCompleted, "Tool shutdown terminated app process.");
    await session.StopAsync();
    await Reject<OperationCanceledException>(() => run);
    Require(session.ProcessId is null, "Stop retained its process.");
    Require(await session.ExecuteAsync(new("restart-probe", [new("dotnet", [assembly, "--child"], cwd, [])])) == 0, "Restart did not create a new session run.");
}
await using (var oversized = new ExecutionSession())
{
    await Reject<ToolContractException>(() => oversized.ExecuteAsync(new("oversized-log", [new("dotnet", [assembly, "--child", "--large-output", "--wait"], cwd, [])])));
    Require(oversized.ProcessId is null, "Output reader failure retained a running child.");
    Require(await oversized.ExecuteAsync(new("reader-failure-restart", [new("dotnet", [assembly, "--child"], cwd, [])])) == 0, "Reader failure poisoned Restart.");
}
var gracefulDirectory = Path.Combine(Path.GetTempPath(), "doroti-stop-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(gracefulDirectory);
try
{
    var path = Path.Combine(gracefulDirectory, "stop.json");
    await using var session = new ExecutionSession();
    var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var plan = new ExecutionPlan("graceful-probe", [new("dotnet", [assembly, "--child", "--graceful", path], cwd, [], new(path, "graceful-session", 5))]);
    var roundtrip = JsonSerializer.Deserialize(JsonSerializer.Serialize(plan, ToolWireJson.Context.ExecutionPlan), ToolWireJson.Context.ExecutionPlan)!;
    Require(roundtrip.Steps[0].StopSignal == plan.Steps[0].StopSignal, "Wire plan lost graceful Stop.");
    var run = session.ExecuteAsync(roundtrip, line => { if (line == "child-started") started.TrySetResult(); });
    await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
    await session.StopAsync();
    await Reject<OperationCanceledException>(() => run);
    Require(File.Exists(path + ".cleaned") && session.ProcessId is null, "Stop killed the adapter before owned cleanup.");
    await Reject<ToolContractException>(() => session.ExecuteAsync(plan with { Steps = [plan.Steps[0] with { StopSignal = new("relative.json", "session", 5) }] }));
}
finally { Directory.Delete(gracefulDirectory, true); }
Console.WriteLine("PASS: typed concurrent activation/cleanup, declared ALC/shared Contracts, real process handshake/result/error/cancel/crash/hang/reconnect, framed malformed input, 64-bit IDs and separate app Stop/Restart ownership.");
