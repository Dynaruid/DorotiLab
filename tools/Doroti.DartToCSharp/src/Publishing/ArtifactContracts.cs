namespace Doroti.DartToCSharp;

public sealed record ConverterOutput(string Input, string Output, string Sha256, string[] Symbols);

public sealed record ConverterReport(
    string SchemaVersion,
    CompilerIdentity Identity,
    bool Success,
    ConverterOutput[] Outputs,
    ConverterDiagnostic[] Diagnostics)
{
    public string ConverterVersion => Identity.ConverterVersion;
}

public sealed record GeneratedFile(string Code, SourceMapEntry[] Mappings);
public sealed record SourceMapDocument(string SchemaVersion, SourceMapEntry[] Mappings);

public sealed record SourceMapEntry(
    string Source,
    int SourceOffset,
    int SourceLength,
    string Symbol,
    string GeneratedFile,
    int GeneratedLineStart,
    int GeneratedLineEnd,
    string? GeneratedDeclarationShape = null);

public sealed record CompilerWorkspace(string Path, ConverterReport Report);

public sealed record PackageReleaseDocument(
    string SchemaVersion,
    string PackageId,
    string PackageVersion,
    string Tier,
    PackageReleaseSource Source,
    CompilerIdentity Identity,
    PackageGraph PackageGraph,
    PackageReleaseInput[] Inputs,
    PackageReleaseArtifact[] Artifacts,
    ConverterDiagnostic[] Diagnostics,
    bool Success,
    string RuntimeDependencyMode,
    string DartCompilerRequirement);

public sealed record PackageReleaseSource(string Package, string Version, string License, string LicenseSha256, string Source);
public sealed record PackageReleaseInput(string Uri, string Sha256, string[] Symbols);
public sealed record PackageReleaseArtifact(string Path, string Sha256);
