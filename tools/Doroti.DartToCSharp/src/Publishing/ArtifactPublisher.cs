namespace Doroti.DartToCSharp;

internal static class ArtifactPublisher
{
    public static ConverterReport CompileAndPublish(
        string manifestPath,
        string outputDirectory,
        string? cacheDirectory,
        int? maxDegreeOfParallelism = null,
        CompilerDumpOptions? dumpOptions = null,
        string? telemetryPath = null,
        int? analyzerWorkers = null,
        int? loweringParallelism = null)
    {
        var target = Path.GetFullPath(outputDirectory);
        var parent = Path.GetDirectoryName(target)
            ?? throw new InvalidDataException($"Generated output must have a parent directory: {target}");
        if (Path.GetPathRoot(target) == target)
        {
            throw new InvalidDataException("A filesystem root cannot be used as a generated workspace.");
        }
        if (telemetryPath is not null)
        {
            var resolvedTelemetry = Path.GetFullPath(telemetryPath);
            var outputPrefix = target.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (string.Equals(resolvedTelemetry, target, StringComparison.OrdinalIgnoreCase) ||
                resolvedTelemetry.StartsWith(outputPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("--telemetry must be outside the compiler-owned generated workspace.");
            }
        }

        Directory.CreateDirectory(parent);
        var name = Path.GetFileName(target.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var token = Guid.NewGuid().ToString("N");
        var staging = Path.Combine(parent, $".{name}.doroti-staging-{token}");
        var backup = Path.Combine(parent, $".{name}.doroti-backup-{token}");
        var resolvedAnalyzerWorkers = CompilerParallelism.ResolveAnalyzerWorkers(analyzerWorkers, maxDegreeOfParallelism);
        var resolvedLoweringParallelism = CompilerParallelism.ResolveLoweringParallelism(loweringParallelism, maxDegreeOfParallelism);
        using var profiler = new CompilerProfiler(manifestPath, telemetryPath, resolvedAnalyzerWorkers, resolvedLoweringParallelism);
        ConverterReport report;
        var published = false;
        try
        {
            report = ConverterEngine.Convert(
                manifestPath,
                staging,
                Directory.Exists(target) ? target : null,
                cacheDirectory,
                maxDegreeOfParallelism,
                dumpOptions,
                profiler,
                resolvedAnalyzerWorkers,
                resolvedLoweringParallelism);
            PublishDirectory(staging, target, backup);
            published = true;
            profiler.Complete();
        }
        catch (Exception exception)
        {
            profiler.Fail(exception);
            throw;
        }
        finally
        {
            DeleteOwnedDirectory(staging, parent);
            if (published || Directory.Exists(target))
            {
                DeleteOwnedDirectory(backup, parent);
            }
        }

        return report;
    }

    private static void PublishDirectory(string staging, string target, string backup)
    {
        var hadTarget = Directory.Exists(target);
        if (hadTarget)
        {
            MoveDirectoryWithRetry(target, backup);
        }

        try
        {
            MoveDirectoryWithRetry(staging, target);
        }
        catch
        {
            if (hadTarget && Directory.Exists(backup) && !Directory.Exists(target))
            {
                MoveDirectoryWithRetry(backup, target);
            }

            throw;
        }
    }

    private static void DeleteOwnedDirectory(string path, string expectedParent)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        var resolvedParent = Path.GetFullPath(Path.GetDirectoryName(Path.GetFullPath(path))!);
        if (!string.Equals(resolvedParent, Path.GetFullPath(expectedParent), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Refusing to clean generated directory outside its expected parent: {path}");
        }

        for (var attempt = 0; ; attempt++)
        {
            try
            {
                Directory.Delete(path, recursive: true);
                return;
            }
            catch (UnauthorizedAccessException) when (attempt < 9)
            {
                Thread.Sleep(100 * (attempt + 1));
            }
            catch (IOException) when (attempt < 9)
            {
                Thread.Sleep(100 * (attempt + 1));
            }
        }
    }

    private static void MoveDirectoryWithRetry(string source, string destination)
    {
        for (var attempt = 0; ; attempt++)
        {
            try
            {
                Directory.Move(source, destination);
                return;
            }
            catch (UnauthorizedAccessException) when (attempt < 9)
            {
                Thread.Sleep(100 * (attempt + 1));
            }
            catch (IOException) when (attempt < 9)
            {
                Thread.Sleep(100 * (attempt + 1));
            }
        }
    }
}
