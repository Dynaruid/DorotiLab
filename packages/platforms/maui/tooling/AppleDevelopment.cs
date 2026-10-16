using System.Net;
using System.Text.RegularExpressions;
using Doroti.Tooling.Contracts;

namespace Doroti.Tool.Maui;

internal static class AppleDevelopment
{
    internal static ExecutionPlan Plan(OperationRequest request, string? device = null)
    {
        if (request.Configuration != "Debug")
            throw new ToolContractException("unsupported-development-profile", "Apple metadata development requires Debug without NativeAOT.");
        var context = request.Context;
        var target = context.TargetId;
        var rid = context.RuntimeIdentifier;
        var framework = context.TargetFramework ?? "";
        var valid = target switch
        {
            "iOS" => rid is "ios-arm64" or "iossimulator-arm64" or "iossimulator-x64"
                && Regex.IsMatch(framework, @"^net10\.0-ios(?:\d+(?:\.\d+)*)?$")
                || rid == "ios-arm64" && framework == "net11.0-ios",
            "macOS" => rid is "osx-arm64" or "osx-x64" && Regex.IsMatch(framework, @"^net10\.0-macos(?:\d+(?:\.\d+)*)?$"),
            "MacCatalyst" => rid is "maccatalyst-arm64" or "maccatalyst-x64" && Regex.IsMatch(framework, @"^net10\.0-maccatalyst(?:\d+(?:\.\d+)*)?$"),
            _ => false,
        };
        if (!valid) throw new ToolContractException("unsupported-development-profile", "Select an explicit matching Apple TFM/RID in the workspace and runner.");
        if (target == "iOS" && string.IsNullOrWhiteSpace(device))
            throw new ToolContractException("device-selection-required", "Select one available simulator or paired iPhone matching the workspace RID.");
        if (target != "iOS" && context.DeviceId is not (null or "host"))
            throw new ToolContractException("unsupported-device", context.DeviceId);
        var values = request.Options.ToDictionary(value => value.Name, value => value.Value);
        var sessionId = values.GetValueOrDefault("sessionid", Guid.NewGuid().ToString("N"));
        if (!Regex.IsMatch(sessionId, "^[A-Za-z0-9-]{1,80}$"))
            throw new ToolContractException("missing-development-session", "Apple development requires a valid session identity.");
        var directory = values.GetValueOrDefault("sessiondirectory", Path.Combine(context.Workspace, ".doroti", "dev", sessionId));
        if (string.IsNullOrWhiteSpace(directory))
            throw new ToolContractException("missing-development-session", "Apple development requires a session directory.");
        directory = Path.GetFullPath(directory);
        var adapter = Path.Combine(Path.GetDirectoryName(typeof(Extension).Assembly.Location)!, "apple",
            target == "iOS" ? "ios-development.py" : "mac-development.py");
        if (!File.Exists(adapter)) throw new ToolContractException("provider-not-installed", "The Apple development adapter is missing from the provider tool package.");
        var arguments = new List<string> { adapter, "--runner", context.Project, "--app-root", context.Workspace,
            "--working-directory", Path.GetDirectoryName(context.Project)!, "--framework", framework,
            "--rid", rid!, "--session-directory", directory, "--session-id", sessionId,
            "--dotnet", values.GetValueOrDefault("dotnetpath", "dotnet") };
        if (target == "iOS")
        {
            arguments.AddRange(["--device", device!, "--xcrun", values.GetValueOrDefault("xcrunpath", "xcrun")]);
            if (values.GetValueOrDefault("hotreloadhost", "") is { Length: > 0 } host)
            {
                if (rid != "ios-arm64" || framework.StartsWith("net11.", StringComparison.Ordinal) ||
                    !IPAddress.TryParse(host, out var address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork ||
                    IPAddress.IsLoopback(address) || address.Equals(IPAddress.Any) || address.GetAddressBytes()[0] >= 224)
                    throw new ToolContractException("invalid-configuration", "hotreloadhost requires a LAN IPv4 address for a .NET 10 iPhone profile.");
                arguments.AddRange(["--host", host]);
            }
        }
        else arguments.AddRange(["--platform", target == "macOS" ? "macos" : "maccatalyst"]);
        return new(Guid.NewGuid().ToString("N"), [new(values.GetValueOrDefault("pythonpath", "python3"),
            arguments, Path.GetDirectoryName(context.Project)!, [], new(Path.Combine(directory, "stop.json"), sessionId, 60))]);
    }
}
