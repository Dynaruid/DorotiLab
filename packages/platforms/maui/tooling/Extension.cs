using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.Maui;
public sealed class Extension() : DotnetToolExtension("maui", ["windows", "macos", "linux"])
{
    protected override IReadOnlyList<string> RequiredWorkloads(ToolContext context) => context.TargetId switch
    { "Windows" => ["maui-windows"], "Android" => ["android"], "iOS" => ["ios"], "MacCatalyst" => ["maccatalyst"], "macOS" => ["macos"], _ => [] };
    protected override IReadOnlyList<string> DiagnosticBuildProperties(ToolContext context) => ["MauiVersion"];


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
            "Android" => new([..configuration.Options, new("adbpath", OptionKind.Text, AndroidDevices.DefaultExecutable(), false, [])]),
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
        var plan = await base.PlanAsync(request with { Context = request.Context with { DeviceId = null } }, token);
        if (request.Operation != "run" && request.Context.DeviceId is null) return plan;
        var context = request.Context with { Options = request.Options };
        var devices = (await GetDevicesAsync(context, token)).Devices;
        var selected = request.Context.DeviceId;
        if (selected is null)
        {
            if (devices.Count != 1) throw new ToolContractException("device-selection-required", "Select one authorized Android device matching the runner RID with -Device <serial>.");
            selected = devices[0].Id;
        }
        if (!devices.Any(device => device.Id == selected)) throw new ToolContractException("unsupported-device", selected);
        return plan with { Steps = plan.Steps.Select(step => step with { Arguments = [..step.Arguments, "-p:AdbTarget=-s " + selected] }).ToArray() };
    }
    protected override IReadOnlyList<string> Arguments(OperationRequest request)
    {
        if (request.Operation == "dev" && request.Context.TargetId is "Android" or "iOS") throw new ToolContractException("unsupported-operation", "Mobile development requires the device transport profile; generic dotnet watch is not advertised.");
        if (request.Operation == "run" && request.Context.TargetId is "Android" or "iOS") return ["build", request.Context.Project, "-t:Run", "-c", request.Configuration];
        return base.Arguments(request);
    }

}
