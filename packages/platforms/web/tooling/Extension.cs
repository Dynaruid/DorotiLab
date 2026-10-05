using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.Web;
public sealed class Extension() : DotnetToolExtension("web", ["windows", "macos", "linux"])
{
    protected override IReadOnlyList<string> RequiredWorkloads(ToolContext context) => ["wasm-tools"];
    protected override IReadOnlyList<string> DiagnosticBuildProperties(ToolContext context) => ["TypeScriptMSBuildVersion", "DorotiTypeScriptVersion", "WasmBuildNative", "WasmEnableThreads"];


}
