using System.Collections.Concurrent;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed record AnalyzerTelemetry(int InvocationCount, int CacheHits, int CacheMisses);

internal sealed class AnalyzerProcessClient(string analyzerRoot, CompilerProfiler profiler)
{
    private readonly ConcurrentDictionary<string, object> _cacheLocks = new(StringComparer.OrdinalIgnoreCase);
    private int _invocationCount;
    private int _cacheHits;
    private int _cacheMisses;

    public AnalyzerTelemetry Telemetry => new(
        Volatile.Read(ref _invocationCount),
        Volatile.Read(ref _cacheHits),
        Volatile.Read(ref _cacheMisses));

    public string Analyze(
        string inputPath,
        string? cacheDirectory,
        bool syntaxOnly,
        bool useFrameworkPackageConfig)
    {
        var packageConfigPath = useFrameworkPackageConfig
            ? Path.Combine(analyzerRoot, "flutter_package_config.json")
            : null;
        string cacheKey;
        using (profiler.MeasureLibrary("cache-key", inputPath))
        {
            cacheKey = AnalyzerCacheKey.Create(analyzerRoot, inputPath, syntaxOnly, packageConfigPath);
        }
        var cachePath = string.IsNullOrWhiteSpace(cacheDirectory)
            ? null
            : Path.Combine(Path.GetFullPath(cacheDirectory), cacheKey + ".json");
        if (TryReadCache(cachePath, out var cached))
        {
            return cached;
        }

        var cacheGate = _cacheLocks.GetOrAdd(cacheKey, static _ => new object());
        lock (cacheGate)
        {
            if (TryReadCache(cachePath, out cached))
            {
                return cached;
            }

            Interlocked.Increment(ref _cacheMisses);
            profiler.RecordCacheMiss();
            var arguments = syntaxOnly
                ? new[] { "run", "entrypoints/extract.dart", inputPath, "--syntax-only" }
                : packageConfigPath is not null
                    ? new[] { "run", "entrypoints/extract.dart", inputPath, "--packages", packageConfigPath }
                    : new[] { "run", "entrypoints/extract.dart", inputPath };
            Interlocked.Increment(ref _invocationCount);
            profiler.RecordDartProcess();
            profiler.RecordAnalysisContext();
            ProcessResult analyzer;
            using (profiler.MeasureLibrary("dart-startup-resolve-extract-json", inputPath))
            {
                analyzer = ProcessRunner.Run("dart", arguments, analyzerRoot);
            }
            analyzer.EnsureSuccess($"Dart analyzer for {inputPath}");
            if (cachePath is not null)
            {
                var payload = analyzer.StandardOutput + "\n";
                ArtifactFiles.WriteUtf8(cachePath, payload);
                profiler.RecordCacheWrite(System.Text.Encoding.UTF8.GetByteCount(payload));
            }

            return analyzer.StandardOutput;
        }
    }

    private bool TryReadCache(string? cachePath, out string content)
    {
        if (cachePath is not null && File.Exists(cachePath))
        {
            Interlocked.Increment(ref _cacheHits);
            content = File.ReadAllText(cachePath);
            profiler.RecordCacheHit(new FileInfo(cachePath).Length);
            return true;
        }

        content = string.Empty;
        return false;
    }
}
