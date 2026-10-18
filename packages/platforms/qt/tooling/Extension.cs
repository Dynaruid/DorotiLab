using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.Qt;
public sealed class Extension() : DotnetToolExtension("qt", ["linux"])
{
    public override async ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken token)
    {
        var configuration = await base.GetConfigurationAsync(context, token);
        return new([..configuration.Options,
            new("packageformat", OptionKind.Choice, "directory", false, ["directory", "deb"]),
            new("output", OptionKind.Text, "", false, [])]);
    }
    protected override IReadOnlyList<(string Id, string Executable, IReadOnlyList<string> Arguments)> RequiredTools(ToolContext context) => [("cmake", "cmake", ["--version"]), ("qt6", "pkg-config", ["--modversion", "Qt6Core"]), ("cxx", "c++", ["--version"])];
    protected override IReadOnlyList<string> Arguments(OperationRequest request) => request.Operation == "dev"
        ? ["watch", "--project", request.Context.Project, "run", "--configuration", "Debug", "--no-launch-profile", "--property:DorotiQtDevelopment=true"]
        : base.Arguments(request);
    public override async ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token)
    {
        var plan = await base.PlanAsync(request, token);
        var format = request.Options.SingleOrDefault(option => option.Name == "packageformat")?.Value ?? "directory";
        var output = request.Options.SingleOrDefault(option => option.Name == "output")?.Value;
        if (request.Operation != "publish" && request.Options.Any(option => option.Name is "packageformat" or "output"))
            throw new ToolContractException("publish-option-only", "packageformat and output require publish.");
        if (format == "deb")
        {
            if (request.Configuration != "Release" || request.Context.RuntimeIdentifier != "linux-x64" || request.Context.TargetFramework != "net10.0" || request.Context.TargetId != "Linux")
                throw new ToolContractException("unsupported-deb-profile", "deb requires Linux/linux-x64, net10.0 and Release.");
            var step = plan.Steps.Single();
            var script = Path.Combine(Path.GetDirectoryName(typeof(Extension).Assembly.Location)!, "linux_deb.py");
            var inspectArguments = new[] { script, "inspect", "--dotnet", step.Executable, "--project", request.Context.Project,
                "--framework", request.Context.TargetFramework, "--rid", request.Context.RuntimeIdentifier, "--configuration", request.Configuration };
            var inspected = await ProbeAsync("python3", inspectArguments, token, step.WorkingDirectory, 120);
            if (inspected.Exit != 0) throw new ToolContractException("deb-preflight-failed", inspected.Output);
            // One owned process wraps publish + packaging so failure/cancellation can
            // remove the private publish/staging directories as well as child tools.
            var identity = Guid.NewGuid().ToString("N");
            var signal = Path.Combine(Path.GetTempPath(), "doroti-deb-" + identity + ".stop.json");
            return plan with { Steps = [new("python3", [script, "build", "--dotnet", step.Executable,
                "--publish-arguments", System.Text.Json.JsonSerializer.Serialize(step.Arguments.ToArray(), DebJsonContext.Default.StringArray),
                "--metadata", inspected.Output, "--output", Path.GetFullPath(string.IsNullOrEmpty(output) ? Path.Combine(request.Context.Workspace, "artifacts", "linux-deb") : output),
                "--stop-signal", signal, "--session-id", identity], step.WorkingDirectory, step.Environment, new(signal, identity))] };
        }
        if (request.Operation == "publish" && !string.IsNullOrEmpty(output))
            plan = plan with { Steps = plan.Steps.Select(step => step with { Arguments = [..step.Arguments, "--output", Path.GetFullPath(output)] }).ToArray() };
        if (request.Operation != "dev") return plan;
        return plan with { Steps = plan.Steps.Select(step => step with
        { Environment = [..step.Environment, new("DOTNET_USE_POLLING_FILE_WATCHER", Environment.GetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER") ?? "1")] }).ToArray() };
    }
}

[System.Text.Json.Serialization.JsonSerializable(typeof(string[]))]
internal partial class DebJsonContext : System.Text.Json.Serialization.JsonSerializerContext;
