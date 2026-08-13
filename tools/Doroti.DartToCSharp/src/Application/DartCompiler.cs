namespace Doroti.DartToCSharp;

/// <summary>
/// Application boundary for deterministic Dart-to-C# compilation.
/// Per-library analyze/lower/emit runs with bounded parallelism; publish stays atomic.
/// </summary>
public sealed class DartCompiler
{
    public ConverterReport Compile(
        string manifestPath,
        string outputDirectory,
        string? cacheDirectory = null,
        int? maxDegreeOfParallelism = null,
        CompilerDumpOptions? dumpOptions = null,
        string? telemetryPath = null,
        int? analyzerWorkers = null,
        int? loweringParallelism = null) =>
        ArtifactPublisher.CompileAndPublish(
            manifestPath,
            outputDirectory,
            cacheDirectory,
            maxDegreeOfParallelism,
            dumpOptions,
            telemetryPath,
            analyzerWorkers,
            loweringParallelism);

    public CompilerWorkspace CompileToWorkspace(
        string manifestPath,
        string workspaceRoot,
        string? cacheDirectory = null,
        int? maxDegreeOfParallelism = null,
        CompilerDumpOptions? dumpOptions = null,
        string? telemetryPath = null,
        int? analyzerWorkers = null,
        int? loweringParallelism = null)
    {
        var workspaceId = ComputeWorkspaceId(manifestPath);
        var path = Path.Combine(Path.GetFullPath(workspaceRoot), workspaceId);
        var report = Compile(
            manifestPath,
            path,
            cacheDirectory,
            maxDegreeOfParallelism,
            dumpOptions,
            telemetryPath,
            analyzerWorkers,
            loweringParallelism);
        if (report.Identity.WorkspaceId != workspaceId)
        {
            throw new InvalidDataException("Compiler workspace identity changed during generation.");
        }

        return new(path, report);
    }

    public string ComputeWorkspaceId(string manifestPath) => WorkspaceFingerprint.Compute(manifestPath);
}
