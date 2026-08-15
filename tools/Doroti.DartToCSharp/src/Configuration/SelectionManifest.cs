namespace Doroti.DartToCSharp;

public sealed record SelectionManifest(
    string SchemaVersion,
    string ConverterVersion,
    string MigrationIrVersion,
    string GenerationMode,
    string CompatibilityProfile,
    string OutputNamespace,
    string OutputAssemblyName,
    string? AnalyzerProject,
    string FlutterBaseline,
    string? PackageRoot,
    SelectionInput[] Inputs,
    string? EntryPoint = null,
    string? PackageId = null,
    string? PackageVersion = null,
    string? PackageTier = null,
    string? SourcePackage = null,
    string? SourceVersion = null,
    string? SourceLicense = null,
    string? AnalysisMode = null,
    string? FrameworkMilestone = null,
    ApplicationCompilation? Application = null);

public sealed record SelectionInput(
    string Path,
    string[] Symbols,
    string? Library = null,
    string EmissionMode = "generate",
    string[]? BoundarySymbols = null);

public sealed record ApplicationCompilation(
    string ResourceManifest,
    string PluginManifest,
    string TargetRid,
    string[] FrameworkPackages,
    string HostBootstrapPackage = "Doroti.Hosting",
    Dictionary<string, bool>? Conditions = null);
