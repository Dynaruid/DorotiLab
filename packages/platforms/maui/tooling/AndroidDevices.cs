using System.Text.RegularExpressions;
using Doroti.Tooling.Contracts;

namespace Doroti.Tool.Maui;

internal static class AndroidDevices
{
    internal static string DefaultExecutable()
    {
        foreach (var root in new[] { Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"),
                     Environment.GetEnvironmentVariable("ANDROID_HOME"),
                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android", "Sdk") })
        {
            if (string.IsNullOrWhiteSpace(root)) continue;
            var candidate = Path.Combine(root, "platform-tools", OperatingSystem.IsWindows() ? "adb.exe" : "adb");
            if (File.Exists(candidate)) return candidate;
        }
        return "adb";
    }

    internal static async Task<DeviceResult> DiscoverAsync(ToolContext context, string executable,
        Func<string, IEnumerable<string>, CancellationToken, string?, int, Task<(int Exit, string Output)>> probe,
        CancellationToken token)
    {
        var listing = await probe(executable, ["devices", "-l"], token, Path.GetDirectoryName(context.Project), 15);
        if (listing.Exit != 0) throw new ToolContractException("device-discovery-failed", listing.Output);
        var ready = listing.Output.Split('\n').Select(line => line.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries))
            .Where(fields => fields.Length >= 2 && fields[1] == "device").ToArray();
        if (ready.Length > 30 || ready.Select(fields => fields[0]).Distinct().Count() != ready.Length)
            throw new ToolContractException("invalid-device-list", "Device discovery requires at most 30 unique authorized devices.");
        var devices = new List<ToolDevice>();
        foreach (var fields in ready)
        {
            var serial = fields[0];
            // This serial is later carried through the Android SDK's AdbTarget command string.
            if (!Regex.IsMatch(serial, "^[A-Za-z0-9_.:-]{1,160}$"))
                throw new ToolContractException("invalid-device-id", serial);
            var abi = await probe(executable, ["-s", serial, "shell", "getprop", "ro.product.cpu.abi"], token,
                Path.GetDirectoryName(context.Project), 15);
            if (abi.Exit != 0) throw new ToolContractException("device-discovery-failed", serial + ": " + abi.Output);
            var rid = abi.Output.Trim() switch { "arm64-v8a" => "android-arm64", "x86_64" => "android-x64", _ => null };
            if (rid is null || context.RuntimeIdentifier is { Length: > 0 } required && rid != required) continue;
            var model = fields.FirstOrDefault(field => field.StartsWith("model:", StringComparison.Ordinal))?[6..].Replace('_', ' ');
            devices.Add(new(serial, model ?? serial, [context.TargetId], [context.Profile]));
        }
        return new(devices);
    }
}
