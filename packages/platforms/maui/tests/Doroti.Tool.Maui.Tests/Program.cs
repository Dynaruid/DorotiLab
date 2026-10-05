using Doroti.Tooling.Contracts;
using Doroti.Tool.Maui;

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
