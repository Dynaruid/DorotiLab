using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;
namespace Doroti.Tool.WindowsAppSdk;
public sealed class Extension() : DotnetToolExtension("windowsappsdk", ["windows"])
{
    protected override IReadOnlyList<string> DiagnosticBuildProperties(ToolContext context) => ["WindowsAppSDKVersion", "DorotiGraphiteNativeAsset"];


}
