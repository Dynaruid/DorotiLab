using System.Text.RegularExpressions;
using Doroti.Tooling.Contracts;

namespace Doroti.Tool.Maui;

internal static class AndroidDevelopment
{
    internal static ExecutionPlan Plan(OperationRequest request, string device, string adb)
    {
        if (request.Configuration != "Debug")
            throw new ToolContractException("unsupported-development-profile", "Android dev requires Debug Mono without trimming or AOT.");
        var values = request.Options.ToDictionary(value => value.Name, value => value.Value);
        var sessionId = values.GetValueOrDefault("sessionid", "");
        if (!Regex.IsMatch(sessionId, "^[A-Za-z0-9-]{1,80}$") ||
            !values.TryGetValue("sessiondirectory", out var directory) || string.IsNullOrWhiteSpace(directory))
            throw new ToolContractException("missing-development-session", "Android dev requires sessionid and sessiondirectory.");
        directory = Path.GetFullPath(directory);
        var adapter = Path.Combine(Path.GetDirectoryName(typeof(Extension).Assembly.Location)!, "android", "android-development.py");
        if (!File.Exists(adapter)) throw new ToolContractException("provider-not-installed", "Android development adapter is missing from the provider tool package.");
        if (request.Context.RuntimeIdentifier is not ("android-arm64" or "android-x64"))
            throw new ToolContractException("runtime-selection-required", "Android dev requires an explicit arm64/x64 RID.");
        var cache = Path.Combine(Path.GetTempPath(), "doroti-ad");
        var lease = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".doroti", "android-owners");
        return new(Guid.NewGuid().ToString("N"), [new(values.GetValueOrDefault("pythonpath", OperatingSystem.IsWindows() ? "python" : "python3"),
            [adapter, "--runner", request.Context.Project, "--app-root", request.Context.Workspace,
                "--device", device, "--rid", request.Context.RuntimeIdentifier,
                "--session-directory", directory, "--session-id", sessionId,
                "--dotnet", values.GetValueOrDefault("dotnetpath", "dotnet"), "--adb", adb,
                "--cache-root", cache, "--lease-directory", lease], request.Context.Workspace, [],
            new(Path.Combine(directory, "stop.json"), sessionId))]);
    }
}
