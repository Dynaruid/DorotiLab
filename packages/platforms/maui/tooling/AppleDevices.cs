using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Doroti.Tooling.Contracts;

namespace Doroti.Tool.Maui;

internal static class AppleDevices
{
    internal static async Task<DeviceResult> DiscoverAsync(ToolContext context, string executable,
        Func<string, IEnumerable<string>, CancellationToken, string?, int, Task<(int Exit, string Output)>> probe,
        CancellationToken token)
    {
        var devices = new List<ToolDevice>();
        var rid = context.RuntimeIdentifier;
        if (rid is null or "iossimulator-arm64" or "iossimulator-x64")
        {
            var hostRid = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "iossimulator-arm64" : "iossimulator-x64";
            if (rid is null || rid == hostRid)
            {
                var result = await probe(executable, ["simctl", "list", "devices", "available", "--json"], token,
                    Path.GetDirectoryName(context.Project), 15);
                if (result.Exit != 0) throw new ToolContractException("device-discovery-failed", result.Output);
                using var document = Parse(result.Output);
                if (!document.RootElement.TryGetProperty("devices", out var runtimes) || runtimes.ValueKind != JsonValueKind.Object)
                    throw new ToolContractException("invalid-device-list", "simctl did not return a device inventory.");
                foreach (var runtime in runtimes.EnumerateObject())
                {
                    if (!runtime.Name.Contains(".iOS-", StringComparison.Ordinal)) continue;
                    if (runtime.Value.ValueKind != JsonValueKind.Array)
                        throw new ToolContractException("invalid-device-list", "Invalid simulator runtime inventory.");
                    foreach (var device in runtime.Value.EnumerateArray())
                    {
                        if (!device.TryGetProperty("isAvailable", out var available) || available.ValueKind != JsonValueKind.True) continue;
                        Add(context, devices, String(device, "udid"), String(device, "name"));
                    }
                }
            }
        }
        if (rid is null or "ios-arm64")
        {
            var path = Path.Combine(Path.GetTempPath(), "doroti-apple-devices-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                var result = await probe(executable, ["devicectl", "list", "devices", "--json-output", path], token,
                    Path.GetDirectoryName(context.Project), 15);
                if (result.Exit != 0) throw new ToolContractException("device-discovery-failed", result.Output);
                token.ThrowIfCancellationRequested();
                if (!File.Exists(path)) throw new ToolContractException("invalid-device-list", "devicectl did not write its result.");
                using var document = Parse(await File.ReadAllTextAsync(path, token));
                if (!document.RootElement.TryGetProperty("result", out var inventory) ||
                    !inventory.TryGetProperty("devices", out var nativeDevices) || nativeDevices.ValueKind != JsonValueKind.Array)
                    throw new ToolContractException("invalid-device-list", "Invalid CoreDevice inventory.");
                if (nativeDevices.GetArrayLength() > 256)
                    throw new ToolContractException("invalid-device-list", "Device inventory exceeds 256 records.");
                var connectionProbes = Stopwatch.StartNew();
                foreach (var device in nativeDevices.EnumerateArray())
                {
                    var modern = device.TryGetProperty("properties", out var properties);
                    var hardware = Get(device, modern ? properties : device, "hardware", "hardwareProperties", modern);
                    var connection = Get(device, modern ? properties : device, "connection", "connectionProperties", modern);
                    var state = Get(device, modern ? properties : device, "state", "deviceProperties", modern);
                    if (String(hardware, "platform") != "iOS" || String(hardware, "reality") != "physical" ||
                        String(connection, "pairingState") != "paired") continue;
                    var id = String(hardware, "udid");
                    ValidateId(id);
                    if (String(connection, modern ? "state" : "tunnelState") != "connected")
                    {
                        // CoreDevice closes idle tunnels even for an available
                        // wired phone. Confirm that connectable paired candidates
                        // can actually establish their tunnel before advertising.
                        if (!device.TryGetProperty("capabilities", out var capabilities) || capabilities.ValueKind != JsonValueKind.Array ||
                            !capabilities.EnumerateArray().Any(value => String(value, "featureIdentifier") == "com.apple.coredevice.feature.connectdevice")) continue;
                        if (context.DeviceId is not null && context.DeviceId != id) continue;
                        if (connectionProbes.Elapsed > TimeSpan.FromSeconds(30))
                            throw new ToolContractException("device-discovery-timeout", "Paired device connection checks exceeded 30 seconds.");
                        var detailsPath = Path.Combine(Path.GetTempPath(), "doroti-apple-details-" + Guid.NewGuid().ToString("N") + ".json");
                        try
                        {
                            (int Exit, string Output) details;
                            try { details = await probe(executable, ["devicectl", "device", "info", "details", "--device", id, "--json-output", detailsPath], token, Path.GetDirectoryName(context.Project), 5); }
                            catch (OperationCanceledException) when (!token.IsCancellationRequested) { continue; }
                            if (details.Exit != 0) continue;
                            if (!File.Exists(detailsPath)) throw new ToolContractException("invalid-device-list", "Device details result is missing.");
                            using var detailsDocument = Parse(await File.ReadAllTextAsync(detailsPath, token));
                            if (!detailsDocument.RootElement.TryGetProperty("result", out var detail))
                                throw new ToolContractException("invalid-device-list", "Device details result is invalid.");
                            var detailModern = detail.TryGetProperty("properties", out var detailProperties);
                            var detailHardware = Get(detail, detailModern ? detailProperties : detail, "hardware", "hardwareProperties", detailModern);
                            var detailConnection = Get(detail, detailModern ? detailProperties : detail, "connection", "connectionProperties", detailModern);
                            if (String(detailHardware, "udid") != id || String(detailHardware, "reality") != "physical" ||
                                String(detailConnection, "pairingState") != "paired" || String(detailConnection, detailModern ? "state" : "tunnelState") != "connected") continue;
                        }
                        finally { File.Delete(detailsPath); }
                    }
                    Add(context, devices, id, String(state, "name"));
                }
            }
            finally { File.Delete(path); }
        }
        token.ThrowIfCancellationRequested();
        return new(devices);
    }

    private static JsonElement Get(JsonElement device, JsonElement properties, string modernName, string oldName, bool modern) =>
        (modern ? properties : device).TryGetProperty(modern ? modernName : oldName, out var value) && value.ValueKind == JsonValueKind.Object
            ? value : throw new ToolContractException("invalid-device-list", "Missing device properties.");
    private static string String(JsonElement element, string key) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()! : "";
    private static JsonDocument Parse(string value)
    {
        if (value.Length > 4 * 1024 * 1024) throw new ToolContractException("invalid-device-list", "Device inventory exceeds the bound.");
        try { return JsonDocument.Parse(value, new JsonDocumentOptions { MaxDepth = 32 }); }
        catch (JsonException error) { throw new ToolContractException("invalid-device-list", error.Message); }
    }
    private static void Add(ToolContext context, List<ToolDevice> devices, string id, string name)
    {
        ValidateId(id);
        if (devices.Count >= 256 || devices.Any(device => device.Id == id))
            throw new ToolContractException("invalid-device-list", "Device discovery requires at most 256 unique devices.");
        devices.Add(new(id, name.Length == 0 ? id : name, [context.TargetId], [context.Profile]));
    }
    private static void ValidateId(string id)
    {
        if (!Regex.IsMatch(id, "^[A-Fa-f0-9-]{8,64}$")) throw new ToolContractException("invalid-device-id", id);
    }
}
