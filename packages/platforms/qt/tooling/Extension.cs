using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.Qt;
public sealed class Extension() : DotnetToolExtension("qt", ["linux"])
{
    protected override IReadOnlyList<(string Id, string Executable, IReadOnlyList<string> Arguments)> RequiredTools(ToolContext context) => [("cmake", "cmake", ["--version"]), ("qt6", "pkg-config", ["--modversion", "Qt6Core"]), ("cxx", "c++", ["--version"])];
    protected override IReadOnlyList<string> Arguments(OperationRequest request) => request.Operation == "dev"
        ? ["watch", "--project", request.Context.Project, "run", "--configuration", "Debug", "--no-launch-profile", "--property:DorotiQtDevelopment=true"]
        : base.Arguments(request);
    public override async ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token)
    {
        var plan = await base.PlanAsync(request, token);
        if (request.Operation != "dev") return plan;
        return plan with { Steps = plan.Steps.Select(step => step with
        { Environment = [..step.Environment, new("DOTNET_USE_POLLING_FILE_WATCHER", Environment.GetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER") ?? "1")] }).ToArray() };
    }
}
