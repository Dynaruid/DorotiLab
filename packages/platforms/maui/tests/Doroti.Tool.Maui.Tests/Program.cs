using Doroti.Tooling.Contracts;
using Doroti.Tool.Maui;

if (args.FirstOrDefault() == "simctl")
{
    Console.WriteLine(Environment.GetEnvironmentVariable("DOROTI_SIMCTL_FIXTURE") ?? "{\"devices\":{\"com.apple.CoreSimulator.SimRuntime.iOS-27-0\":[{\"udid\":\"AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA\",\"name\":\"Test iPhone\",\"isAvailable\":true},{\"udid\":\"BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB\",\"isAvailable\":false}],\"com.apple.CoreSimulator.SimRuntime.tvOS-27-0\":[{\"udid\":\"CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC\",\"isAvailable\":true}]}}");
    return;
}
if (args.FirstOrDefault() == "devicectl")
{
    if (args.Skip(1).Take(3).SequenceEqual(new[] { "device", "info", "details" }))
    {
        if (Environment.GetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FAIL") == "1") { Environment.ExitCode = 1; return; }
        File.WriteAllText(args[Array.IndexOf(args, "--json-output") + 1], Environment.GetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FIXTURE") ?? "{\"result\":{\"properties\":{\"hardware\":{\"platform\":\"iOS\",\"reality\":\"physical\",\"udid\":\"00008101-001144CA3642001E\"},\"connection\":{\"pairingState\":\"paired\",\"state\":\"connected\"}}}}");
        return;
    }
    File.WriteAllText(args[Array.IndexOf(args, "--json-output") + 1], Environment.GetEnvironmentVariable("DOROTI_COREDEVICE_FIXTURE") ?? "{\"result\":{\"devices\":[{\"properties\":{\"hardware\":{\"platform\":\"iOS\",\"reality\":\"physical\",\"udid\":\"00008101-001144CA3642001E\"},\"connection\":{\"pairingState\":\"paired\",\"state\":\"connected\"},\"state\":{\"name\":\"Test Phone\"}}}]}}");
    return;
}
if (args.FirstOrDefault() == "devices")
{
    Console.WriteLine(Environment.GetEnvironmentVariable("DOROTI_ADB_FIXTURE") ?? "List of devices attached\nphone device model:Test_Phone\nx86 device\noffline offline\nlocked unauthorized");
    return;
}
if (args.FirstOrDefault() == "-s") { Console.WriteLine(args[1] == "x86" ? "x86_64" : "arm64-v8a"); return; }
static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
static async Task Reject(string code, Func<Task> action)
{
    try { await action(); } catch (ToolContractException error) when (error.Code == code) { return; }
    throw new InvalidOperationException("Expected " + code);
}
var fixture = Path.Combine(AppContext.BaseDirectory, "Doroti.Tool.Maui.Tests" + (OperatingSystem.IsWindows() ? ".exe" : ""));
var options = new OptionValue[] { new("adbpath", fixture) };
var context = new ToolContext(Environment.CurrentDirectory, Path.Combine(Environment.CurrentDirectory, "fixture.csproj"), "maui", "Android",
    "default:android-arm64", null, 1, "net10.0-android", "android-arm64", Options: options);
await using var extension = new Extension();
await AppleDeviceRegression.RunAsync(extension, fixture, args.Contains("--live-ios"));
var devices = await extension.GetDevicesAsync(context, default);
Check(devices.Devices.Count == 1 && devices.Devices[0].Id == "phone" && devices.Devices[0].Name == "Test Phone", "Authorization or ABI filter failed.");
var plan = await extension.PlanAsync(new(context, "run", "Debug", options), default);
Check(plan.Steps.Single().Arguments.Contains("-p:AdbTarget=-s phone"), "Selected serial was not sent to the SDK.");
Check(plan.Steps.Single().Arguments.Contains("-t:Run"), "Android runner did not use SDK launch.");
await Reject("unsupported-device", () => extension.PlanAsync(new(context with { DeviceId = "locked" }, "run", "Debug", options), default).AsTask());
try
{
    Environment.SetEnvironmentVariable("DOROTI_ADB_FIXTURE", "List of devices attached\n");
    Check((await extension.GetDevicesAsync(context, default)).Devices.Count == 0, "No connected device became a host device.");
    await Reject("device-selection-required", () => extension.PlanAsync(new(context, "run", "Debug", options), default).AsTask());
    Environment.SetEnvironmentVariable("DOROTI_ADB_FIXTURE", "List of devices attached\na device\nb device");
    await Reject("device-selection-required", () => extension.PlanAsync(new(context, "run", "Debug", options), default).AsTask());
    Environment.SetEnvironmentVariable("DOROTI_ADB_FIXTURE", "List of devices attached\na device\na device");
    await Reject("invalid-device-list", () => extension.GetDevicesAsync(context, default).AsTask());
    Environment.SetEnvironmentVariable("DOROTI_ADB_FIXTURE", "List of devices attached\nbad;id device");
    await Reject("invalid-device-id", () => extension.GetDevicesAsync(context, default).AsTask());
}
finally { Environment.SetEnvironmentVariable("DOROTI_ADB_FIXTURE", null); }
Console.WriteLine("PASS: typed Android discovery, authorization/RID filtering, explicit SDK serial selection and ambiguous/invalid device rejection; fixture processes only.");
