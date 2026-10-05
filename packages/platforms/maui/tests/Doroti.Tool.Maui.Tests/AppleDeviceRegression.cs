using System.Runtime.InteropServices;
using Doroti.Tool.Maui;
using Doroti.Tooling.Contracts;

internal static class AppleDeviceRegression
{
    internal static async Task RunAsync(Extension extension, string fixture, bool live)
    {
        if (!OperatingSystem.IsMacOS()) { Console.WriteLine("SKIPPED: iOS device service requires an Apple host."); return; }
        var rid = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "iossimulator-arm64" : "iossimulator-x64";
        var options = new OptionValue[] { new("xcrunpath", live ? "xcrun" : fixture) };
        var context = new ToolContext(Environment.CurrentDirectory, Path.Combine(Environment.CurrentDirectory, "fixture.csproj"), "maui", "iOS",
            "ios27:" + rid, null, 1, "net10.0-ios27.0", rid, Options: options);
        var result = await extension.GetDevicesAsync(context, default);
        if (live)
        {
            Console.WriteLine($"PASS: live typed simulator discovery: {result.Devices.Count} available iOS devices for {rid}.");
            var physical = await extension.GetDevicesAsync(context with { RuntimeIdentifier = "ios-arm64", Profile = "ios27:ios-arm64" }, default);
            Console.WriteLine($"Live connected paired physical iOS devices: {physical.Devices.Count}; no deployment performed.");
            return;
        }
        Require(result.Devices.Count == 1 && result.Devices[0].Name == "Test iPhone", "Available iOS simulator filtering failed.");
        var plan = await extension.PlanAsync(new(context, "run", "Debug", options), default);
        Require(plan.Steps.Single().Arguments.Contains("-p:_DeviceName=:v2:udid=AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), "Simulator selection was not sent to the Apple SDK.");
        await Reject("unsupported-device", () => extension.PlanAsync(new(context with { DeviceId = "missing" }, "run", "Debug", options), default).AsTask());
        var physicalContext = context with { RuntimeIdentifier = "ios-arm64", Profile = "ios27:ios-arm64" };
        var physicalDevices = await extension.GetDevicesAsync(physicalContext, default);
        Require(physicalDevices.Devices.Count == 1, "Connected paired physical iOS device was lost.");
        var physicalPlan = await extension.PlanAsync(new(physicalContext, "run", "Debug", options), default);
        Require(physicalPlan.Steps.Single().Arguments.Contains("-p:_DeviceName=00008101-001144CA3642001E"), "Physical device selection was lost.");
        try
        {
            Environment.SetEnvironmentVariable("DOROTI_SIMCTL_FIXTURE", "{\"devices\":{}}");
            Require((await extension.GetDevicesAsync(context, default)).Devices.Count == 0, "No simulator became a host device.");
            await Reject("device-selection-required", () => extension.PlanAsync(new(context, "run", "Debug", options), default).AsTask());
            Environment.SetEnvironmentVariable("DOROTI_SIMCTL_FIXTURE", "broken");
            await Reject("invalid-device-list", () => extension.GetDevicesAsync(context, default).AsTask());
            Environment.SetEnvironmentVariable("DOROTI_SIMCTL_FIXTURE", "{\"devices\":{\"com.apple.CoreSimulator.SimRuntime.iOS-27-0\":[{\"udid\":\"bad;id\",\"isAvailable\":true}]}}");
            await Reject("invalid-device-id", () => extension.GetDevicesAsync(context, default).AsTask());
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_FIXTURE", "{\"result\":{\"devices\":[{\"hardwareProperties\":{\"platform\":\"iOS\",\"reality\":\"physical\",\"udid\":\"00008101-001144CA3642001E\"},\"connectionProperties\":{\"pairingState\":\"paired\",\"tunnelState\":\"disconnected\"},\"deviceProperties\":{\"name\":\"Offline\"}}]}}");
            Require((await extension.GetDevicesAsync(physicalContext, default)).Devices.Count == 0, "Disconnected device advertised for launch.");
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_FIXTURE", "{\"result\":{\"devices\":[{\"properties\":{\"hardware\":{\"platform\":\"iOS\",\"reality\":\"physical\",\"udid\":\"00008101-001144CA3642001E\"},\"connection\":{\"pairingState\":\"paired\",\"state\":\"disconnected\"},\"state\":{\"name\":\"Available Phone\"}},\"capabilities\":[{\"featureIdentifier\":\"com.apple.coredevice.feature.connectdevice\"}]}]}}");
            Require((await extension.GetDevicesAsync(physicalContext, default)).Devices.Count == 1, "Available paired phone with an idle tunnel was lost.");
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FAIL", "1");
            Require((await extension.GetDevicesAsync(physicalContext, default)).Devices.Count == 0, "Unreachable paired phone was advertised.");
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FAIL", null);
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FIXTURE", "{\"result\":{\"properties\":{\"hardware\":{\"platform\":\"iOS\",\"reality\":\"physical\",\"udid\":\"00008101-9999999999999999\"},\"connection\":{\"pairingState\":\"paired\",\"state\":\"connected\"}}}}");
            Require((await extension.GetDevicesAsync(physicalContext, default)).Devices.Count == 0, "Details for a different phone were accepted.");
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FIXTURE", null);
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_FIXTURE", "{\"result\":{\"devices\":[]}}");
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            try { await extension.GetDevicesAsync(context, cancellation.Token); throw new Exception("Canceled discovery ran."); }
            catch (OperationCanceledException) { }
            await Reject("unsupported-operation", () => extension.PlanAsync(new(physicalContext, "dev", "Debug", options), default).AsTask());
        }
        finally
        {
            Environment.SetEnvironmentVariable("DOROTI_SIMCTL_FIXTURE", null);
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_FIXTURE", null);
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FAIL", null);
            Environment.SetEnvironmentVariable("DOROTI_COREDEVICE_DETAILS_FIXTURE", null);
        }
        Console.WriteLine("PASS: typed iOS discovery, simulator/physical selection, unavailable/disconnected rejection, malformed inventory, cancellation and unimplemented dev rejection (fixture processes).");
    }
    private static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    private static async Task Reject(string code, Func<Task> action)
    {
        try { await action(); } catch (ToolContractException error) when (error.Code == code) { return; }
        throw new Exception("Expected " + code);
    }
}
