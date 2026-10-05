using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.Maui;
public sealed class Extension() : DotnetToolExtension("maui", ["windows", "macos", "linux"])
{
    protected override IReadOnlyList<string> RequiredWorkloads(ToolContext context) => context.TargetId switch
    { "Windows" => ["maui-windows"], "Android" => ["android"], "iOS" => ["ios"], "MacCatalyst" => ["maccatalyst"], "macOS" => ["macos"], _ => [] };
    protected override IReadOnlyList<string> DiagnosticBuildProperties(ToolContext context) => ["MauiVersion"];

    protected override IReadOnlyList<(string Id, string Executable, IReadOnlyList<string> Arguments)> RequiredTools(ToolContext context) =>
        context.TargetId == "Android" ? [
            ("adb", context.Options?.SingleOrDefault(value => value.Name == "adbpath")?.Value ?? AndroidDevices.DefaultExecutable(), ["version"]),
            ("python", context.Options?.SingleOrDefault(value => value.Name == "pythonpath")?.Value ?? (OperatingSystem.IsWindows() ? "python" : "python3"), ["--version"])] : [];

    public override async ValueTask<DiagnosticResult> DiagnoseAsync(ToolContext context, CancellationToken token)
    {
        var result = await base.DiagnoseAsync(context, token);
        var scope = context.Options?.SingleOrDefault(value => value.Name == "scope")?.Value ?? "full";
        if (context.TargetId != "Android" || scope == "managed") return result;
        var dotnet = context.Options?.SingleOrDefault(value => value.Name == "dotnetpath")?.Value ?? "dotnet";
        var timeout = int.Parse(context.Options?.SingleOrDefault(value => value.Name == "timeoutseconds")?.Value ?? "15",
            System.Globalization.CultureInfo.InvariantCulture);
        var arguments = new List<string> { "msbuild", context.Project, "-nologo", "-t:_ResolveMonoAndroidSdks",
            "-getProperty:_JavaSdkDirectory,_AndroidSdkDirectory,_AndroidNdkDirectory,_AndroidApiLevel,AndroidSdkBuildToolsVersion" };
        if (context.TargetFramework is { Length: > 0 } tfm) arguments.Add("-p:TargetFramework=" + tfm);
        if (context.RuntimeIdentifier is { Length: > 0 } rid) arguments.Add("-p:RuntimeIdentifier=" + rid);
        try
        {
            var probe = await ProbeAsync(dotnet, arguments, token, context.Workspace, timeout);
            if (probe.Exit != 0) return new([..result.Diagnostics, new("android-sdk-resolution", DiagnosticStatus.Fail, probe.Output, "Android workload SDK resolution; no build/deployment")]);
            using var document = System.Text.Json.JsonDocument.Parse(probe.Output);
            var properties = document.RootElement.GetProperty("Properties");
            var diagnostics = properties.EnumerateObject().Select(property => new Diagnostic("android:" + property.Name,
                string.IsNullOrWhiteSpace(property.Value.GetString()) ? DiagnosticStatus.Partial : DiagnosticStatus.Pass,
                property.Value.GetString() ?? "", "resolved Android workload prerequisites; not device/runtime acceptance"));
            return new([..result.Diagnostics, ..diagnostics]);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        { return new([..result.Diagnostics, new("android-sdk-resolution", DiagnosticStatus.Partial, "SDK resolution timed out.", scope)]); }
        catch (System.ComponentModel.Win32Exception error)
        { return new([..result.Diagnostics, new("android-sdk-resolution", DiagnosticStatus.Fail, error.Message, scope)]); }
    }


    protected override IReadOnlyList<string> RequiredHosts(ToolContext context) => context.TargetId switch
    {
        "Windows" => ["windows"], "iOS" or "macOS" or "MacCatalyst" => ["macos"], "Android" => ["windows", "macos", "linux"],
        _ => throw new ToolContractException("unsupported-target", context.TargetId),
    };
    public override async ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken token)
    {
        var configuration = await base.GetConfigurationAsync(context, token);
        return context.TargetId switch
        {
            "Android" => new([..configuration.Options, new("adbpath", OptionKind.Text, AndroidDevices.DefaultExecutable(), false, []),
                new("pythonpath", OptionKind.Text, OperatingSystem.IsWindows() ? "python" : "python3", false, [])]),
            "iOS" => new([..configuration.Options, new("xcrunpath", OptionKind.Text, "xcrun", false, [])]),
            _ => configuration,
        };
    }
    public override async ValueTask<DeviceResult> GetDevicesAsync(ToolContext context, CancellationToken token)
    {
        Check(token); ValidateHost(context);
        if (context.TargetId == "Android")
        {
            var schema = await GetConfigurationAsync(context, token);
            ToolContract.ValidateConfiguration(schema, context.Options ?? []);
            var adb = context.Options?.SingleOrDefault(value => value.Name == "adbpath")?.Value
                ?? AndroidDevices.DefaultExecutable();
            return await AndroidDevices.DiscoverAsync(context, adb, ProbeAsync, token);
        }
        if (context.TargetId == "iOS")
        {
            ToolContract.ValidateConfiguration(await GetConfigurationAsync(context, token), context.Options ?? []);
            var xcrun = context.Options?.SingleOrDefault(value => value.Name == "xcrunpath")?.Value ?? "xcrun";
            return await AppleDevices.DiscoverAsync(context, xcrun, ProbeAsync, token);
        }
        if (context.TargetId is not ("Windows" or "macOS" or "MacCatalyst"))
            throw new ToolContractException("unsupported-service", "This profile requires its authorized device discovery extension.");
        return await base.GetDevicesAsync(context, token);
    }
    public override async ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token)
    {
        if (request.Context.TargetId == "iOS")
        {
            var iosPlan = await base.PlanAsync(request with { Context = request.Context with { DeviceId = null } }, token);
            if (request.Operation != "run") return iosPlan;
            var iosDevices = (await GetDevicesAsync(request.Context with { Options = request.Options }, token)).Devices;
            var iosSelected = request.Context.DeviceId;
            if (iosSelected is null)
            {
                if (iosDevices.Count != 1) throw new ToolContractException("device-selection-required", "Select one connected iOS device or available simulator matching the runner RID with -Device <UDID>.");
                iosSelected = iosDevices[0].Id;
            }
            if (!iosDevices.Any(device => device.Id == iosSelected)) throw new ToolContractException("unsupported-device", iosSelected);
            if (request.Context.RuntimeIdentifier is null) throw new ToolContractException("runtime-selection-required", "iOS launch requires an explicit device/simulator RID.");
            return iosPlan with { Steps = iosPlan.Steps.Select(step => step with { Arguments = [..step.Arguments,
                "-p:_DeviceName=" + (request.Context.RuntimeIdentifier.StartsWith("iossimulator-", StringComparison.Ordinal) ? ":v2:udid=" : "") + iosSelected] }).ToArray() };
        }
        if (request.Context.TargetId != "Android") return await base.PlanAsync(request, token);
        Check(token); ValidateHost(request.Context);
        ToolContract.ValidateConfiguration(await GetConfigurationAsync(request.Context, token), request.Options);
        if (request.Operation == "dev" && request.Configuration != "Debug")
            throw new ToolContractException("unsupported-development-profile", "Android dev requires Debug.");
        var plan = request.Operation == "dev" ? null : await base.PlanAsync(request with { Context = request.Context with { DeviceId = null } }, token);
        if (request.Operation is not ("run" or "dev") && request.Context.DeviceId is null) return plan!;
        var context = request.Context with { Options = request.Options };
        var devices = (await GetDevicesAsync(context, token)).Devices;
        var selected = request.Context.DeviceId;
        if (selected is null)
        {
            if (devices.Count != 1) throw new ToolContractException("device-selection-required", "Select one authorized Android device matching the runner RID with -Device <serial>.");
            selected = devices[0].Id;
        }
        if (!devices.Any(device => device.Id == selected)) throw new ToolContractException("unsupported-device", selected);
        if (request.Operation == "dev") return AndroidDevelopment.Plan(request, selected,
            request.Options.SingleOrDefault(value => value.Name == "adbpath")?.Value ?? AndroidDevices.DefaultExecutable());
        return plan! with { Steps = plan.Steps.Select(step => step with { Arguments = [..step.Arguments, "-p:AdbTarget=-s " + selected] }).ToArray() };
    }
    protected override IReadOnlyList<string> Arguments(OperationRequest request)
    {
        if (request.Operation == "dev" && request.Context.TargetId == "iOS") throw new ToolContractException("unsupported-operation", "iOS development requires its device transport profile; generic dotnet watch is not advertised.");
        if (request.Operation == "run" && request.Context.TargetId is "Android" or "iOS") return ["build", request.Context.Project, "-t:Run", "-c", request.Configuration];
        return base.Arguments(request);
    }

}
