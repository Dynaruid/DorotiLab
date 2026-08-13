namespace Doroti.DartToCSharp;

internal static class CompilerVersions
{
    public const string Converter = "3.0.0";
    public const string FrameworkMigrationIr = "3.0.0";
    public const string Analyzer = "9.0.0";
    public const string F0Lowering = "framework-f0.0-typed";
    public const string F0Emitter = "csharp-net10.f0-typed-visitor";
    public const string FrameworkLowering = "framework-g3.1-multilibrary-typed";
    public const string FrameworkEmitter = "csharp-net10.g3.1-project-graph";
}

internal static class CompilerProfiles
{
    public const string FrameworkGenerationMode = "framework-semantic";
    public const string F0 = "flutter-framework-f0";
    public const string Framework = "flutter-framework";
}

internal sealed record CompilerProfile(
    string Id,
    string GenerationMode,
    string CompatibilityProfile,
    string IrVersion,
    string LoweringRuleSetVersion,
    string EmitterVersion,
    bool EnableCollectionLowering,
    bool EmitApplicationEntry,
    bool EnableAsyncNavigationLowering,
    bool EnablePackageLowering,
    bool EmitPackagePlatformPorts,
    bool ReferenceRuntimeBindings,
    bool EnableTypedSemanticCompiler,
    CompatibilityRule[] CompatibilityRules)
{
    public bool IsC5 => false;
    public bool IsFrameworkGraph => CompatibilityProfile == CompilerProfiles.Framework;
    public bool IsApplication => Id == "G5-5";
}

internal static class CompilerProfileRegistry
{
    public static CompilerProfile Resolve(SelectionManifest manifest)
    {
        if (manifest.GenerationMode != CompilerProfiles.FrameworkGenerationMode ||
            manifest.CompatibilityProfile is not (CompilerProfiles.F0 or CompilerProfiles.Framework))
        {
            throw new InvalidDataException(
                "Historical compatibility profiles were removed by G3-T0; use framework-semantic with flutter-framework-f0 or flutter-framework.");
        }

        if (manifest.MigrationIrVersion != CompilerVersions.FrameworkMigrationIr)
        {
            throw new InvalidDataException(
                $"Framework selection must request IR {CompilerVersions.FrameworkMigrationIr}.");
        }

        if (manifest.CompatibilityProfile == CompilerProfiles.Framework && string.IsNullOrWhiteSpace(manifest.FrameworkMilestone))
        {
            throw new InvalidDataException("The general flutter-framework profile requires an explicit frameworkMilestone selection.");
        }

        if (manifest.Application is not null && manifest.FrameworkMilestone != "G5-5")
        {
            throw new InvalidDataException("Application compilation requires frameworkMilestone G5-5.");
        }

        var isGeneral = manifest.CompatibilityProfile == CompilerProfiles.Framework;
        return new(
            isGeneral ? manifest.FrameworkMilestone! : "f0",
            manifest.GenerationMode,
            manifest.CompatibilityProfile,
            CompilerVersions.FrameworkMigrationIr,
            isGeneral ? CompilerVersions.FrameworkLowering : CompilerVersions.F0Lowering,
            isGeneral ? CompilerVersions.FrameworkEmitter : CompilerVersions.F0Emitter,
            EnableCollectionLowering: false,
            EmitApplicationEntry: false,
            EnableAsyncNavigationLowering: false,
            EnablePackageLowering: false,
            EmitPackagePlatformPorts: false,
            ReferenceRuntimeBindings: true,
            EnableTypedSemanticCompiler: true,
            CompatibilityRules: []);
    }
}
