using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed record AnalyzerSessionInput(int Ordinal, string LogicalPath, string PhysicalPath);

internal sealed record AnalyzerBatchCompletion(
    string SchemaVersion,
    int AnalysisContextCount,
    long ContextSetupMicroseconds,
    AnalyzerBatchCompletionItem[] Items);

internal sealed record AnalyzerBatchCompletionItem(
    int Ordinal,
    string OutputPath,
    long OutputBytes,
    long ElapsedMicroseconds,
    string DependenciesPath);

internal sealed record AnalyzerDependencyFingerprint(string Path, string Sha256);

/// <summary>Invocation-owned analyzer session. Cache hits are resolved first; misses share one Dart process/context.</summary>
internal sealed class AnalyzerSession(string analyzerRoot, CompilerProfiler profiler)
{
    public string[] Analyze(
        IReadOnlyList<AnalyzerSessionInput> inputs,
        string? cacheDirectory,
        bool syntaxOnly,
        bool useFrameworkPackageConfig,
        string? applicationPackageConfig = null)
    {
        var packageConfigPath = applicationPackageConfig ?? (useFrameworkPackageConfig
            ? Path.Combine(analyzerRoot, "flutter_package_config.json")
            : null);
        AnalyzerSessionIdentity identity;
        using (profiler.MeasureLibrary("analyzer-session-identity", $"{inputs.Count} input(s)"))
        {
            identity = AnalyzerSessionIdentity.Create(analyzerRoot, packageConfigPath);
        }
        var cacheStore = string.IsNullOrWhiteSpace(cacheDirectory)
            ? null
            : new AnalyzerCacheStore(cacheDirectory, identity, profiler);
        var results = new string[inputs.Count];
        var misses = new List<(AnalyzerSessionInput Input, string CacheKey)>();
        foreach (var input in inputs)
        {
            string cacheKey;
            using (profiler.MeasureLibrary("cache-key", input.LogicalPath))
            {
                cacheKey = AnalyzerCacheKey.Create(identity, input.PhysicalPath, syntaxOnly);
            }
            if (cacheStore?.TryRead(cacheKey, out var cached) == true)
            {
                results[input.Ordinal] = cached;
            }
            else
            {
                profiler.RecordCacheMiss();
                misses.Add((input, cacheKey));
            }
        }
        if (misses.Count == 0) return results;

        var dorotiRoot = RepositoryLocalStorage.FindDorotiRoot(analyzerRoot);
        var localRoot = RepositoryLocalStorage.ResolveRoot(dorotiRoot);
        var sessionDirectory = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "analyzer-session");
        try
        {
            var requestPath = Path.Combine(sessionDirectory, "request.json");
            var request = new
            {
                schemaVersion = "doroti.dart-analyzer-batch/v1",
                syntaxOnly,
                packagesPath = packageConfigPath,
                items = misses.Select((item, index) => new
                {
                    ordinal = index,
                    path = item.Input.PhysicalPath,
                    outputPath = Path.Combine(sessionDirectory, $"item-{index:D5}.json"),
                }).ToArray(),
            };
            ArtifactFiles.WriteJson(requestPath, request);
            profiler.RecordDartProcess();
            ProcessResult process;
            using (profiler.MeasureLibrary("dart-batch-startup-resolve-extract-json", $"{misses.Count} input(s)"))
            {
                process = ProcessRunner.Run(
                    "dart",
                    ["run", "entrypoints/extract_batch.dart", requestPath],
                    analyzerRoot,
                    new Dictionary<string, string?> { [RepositoryLocalStorage.EnvironmentVariable] = localRoot });
            }
            process.EnsureSuccess("Dart analyzer batch");
            var completion = JsonSerializer.Deserialize<AnalyzerBatchCompletion>(
                process.StandardOutput,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("Dart analyzer batch returned an empty completion record.");
            if (completion.SchemaVersion != "doroti.dart-analyzer-batch-completion/v1" || completion.Items.Length != misses.Count)
            {
                throw new InvalidDataException("Dart analyzer batch completion record is incomplete or unsupported.");
            }
            profiler.RecordAnalysisContext(completion.AnalysisContextCount);
            foreach (var completionItem in completion.Items.OrderBy(item => item.Ordinal))
            {
                var miss = misses[completionItem.Ordinal];
                var payload = File.ReadAllText(completionItem.OutputPath);
                var dependencies = ArtifactFiles.ReadJson<AnalyzerDependencyFingerprint[]>(completionItem.DependenciesPath);
                results[miss.Input.Ordinal] = payload;
                cacheStore?.Write(miss.CacheKey, payload, dependencies);
            }
            return results;
        }
        finally
        {
            RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, sessionDirectory);
        }
    }
}
