using Doroti.Tool.Maui;
using Doroti.Tooling.Contracts;

internal static class AppleDevelopmentRegression
{
    internal static async Task RunAsync(Extension extension)
    {
        var directory = Path.Combine(Path.GetTempPath(), "doroti-apple-contract", "한글 app");
        var options = new OptionValue[] { new("dotnetpath", "selected dotnet"), new("pythonpath", "selected python"),
            new("sessiondirectory", Path.Combine(directory, "session")), new("sessionid", "apple-session") };
        foreach (var (target, framework, rid, adapter) in new[] {
            ("macOS", "net10.0-macos27.0", "osx-arm64", "mac-development.py"),
            ("MacCatalyst", "net10.0-maccatalyst27.0", "maccatalyst-arm64", "mac-development.py"),
            ("iOS", "net10.0-ios27.0", "iossimulator-arm64", "ios-development.py"),
            ("iOS", "net10.0-ios", "iossimulator-x64", "ios-development.py"),
            ("iOS", "net10.0-ios27.0", "ios-arm64", "ios-development.py"),
            ("iOS", "net11.0-ios", "ios-arm64", "ios-development.py") })
        {
            var context = new ToolContext(directory, Path.Combine(directory, "runner", "App.csproj"), "maui", target,
                "default:" + rid, null, 1, framework, rid);
            var configuration = await extension.GetConfigurationAsync(context, default);
            ToolContract.ValidateConfiguration(configuration, options);
            var request = new OperationRequest(context, "dev", "Debug", options);
            var step = AppleDevelopment.Plan(request, target == "iOS" ? "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA" : null).Steps.Single();
            Require(step.Executable == "selected python" && File.Exists(step.Arguments[0]) && Path.GetFileName(step.Arguments[0]) == adapter,
                "The packaged Apple adapter was not selected.");
            Require(Value(step, "--framework") == framework && Value(step, "--rid") == rid && Value(step, "--dotnet") == "selected dotnet",
                "The manifest TFM/RID or selected dotnet host was lost.");
            Require(step.WorkingDirectory == Path.GetDirectoryName(context.Project) && Value(step, "--working-directory") == step.WorkingDirectory,
                "Runner SDK selection was replaced with the application working directory.");
            Require(step.StopSignal is { SessionId: "apple-session", TimeoutSeconds: 60 } && Path.IsPathFullyQualified(step.StopSignal.Path),
                "The adapter has no bounded session-matched Stop.");
            Reject("unsupported-development-profile", () => AppleDevelopment.Plan(request with { Configuration = "Release" }, "device"));
            Reject("missing-development-session", () => AppleDevelopment.Plan(request with { Options = [new("sessionid", "../other")] }, "device"));
            var implicitSession = AppleDevelopment.Plan(request with { Options = [] }, "device").Steps.Single();
            Require(implicitSession.StopSignal is not null && Value(implicitSession, "--session-directory").StartsWith(directory, StringComparison.Ordinal),
                "Standalone CLI development did not allocate an owned session.");
            if (target == "iOS")
            {
                Reject("device-selection-required", () => AppleDevelopment.Plan(request));
                var custom = options.Concat(new[] { new OptionValue("xcrunpath", "selected xcrun") }).ToArray();
                Require(Value(AppleDevelopment.Plan(request with { Options = custom }, "device").Steps.Single(), "--xcrun") == "selected xcrun",
                    "The selected CoreDevice tool was lost.");
                Reject("invalid-configuration", () => AppleDevelopment.Plan(request with { Options = [..options, new("hotreloadhost", "127.0.0.1")] }, "device"));
                if (rid == "ios-arm64" && framework.StartsWith("net10.", StringComparison.Ordinal))
                    Require(Value(AppleDevelopment.Plan(request with { Options = [..options, new("hotreloadhost", "192.168.1.2")] }, "device").Steps.Single(), "--host") == "192.168.1.2",
                        "The .NET 10 iPhone LAN relay address was lost.");
                Reject("unsupported-development-profile", () => AppleDevelopment.Plan(request with { Context = context with { TargetFramework = "net11.0-ios", RuntimeIdentifier = "iossimulator-arm64" } }, "device"));
            }
            else Reject("unsupported-device", () => AppleDevelopment.Plan(request with { Context = context with { DeviceId = "another" } }));
        }
        Console.WriteLine("PASS: packaged Apple development plans, explicit TFM/RID/SDK host, simulator/device profiles, session Stop and unsupported profile rejection (no Apple execution).");
    }
    private static string Value(ProcessStep step, string key) => step.Arguments[step.Arguments.ToList().IndexOf(key) + 1];
    private static void Require(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Reject(string code, Action action)
    {
        try { action(); } catch (ToolContractException error) when (error.Code == code) { return; }
        throw new Exception("Expected " + code);
    }
}
