namespace Doroti.DartToCSharp;

/// <summary>
/// Bounded parallelism for per-library analyze / lower / emit.
/// Final staging publish and report aggregation stay single-threaded.
/// </summary>
public static class CompilerParallelism
{
    public const string EnvironmentVariableName = "DOROTI_COMPILER_PARALLELISM";
    public const string AnalyzerWorkersEnvironmentVariableName = "DOROTI_ANALYZER_WORKERS";
    public const string LoweringParallelismEnvironmentVariableName = "DOROTI_LOWERING_PARALLELISM";

    public static int Resolve(int? requested = null)
    {
        if (requested is > 0)
        {
            return requested.Value;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        if (int.TryParse(fromEnvironment, out var parsed) && parsed > 0)
        {
            return parsed;
        }

        return Math.Max(1, Environment.ProcessorCount);
    }

    public static int ResolveAnalyzerWorkers(int? requested = null, int? compatibilityParallelism = null) =>
        1;

    public static int ResolveLoweringParallelism(int? requested = null, int? compatibilityParallelism = null) =>
        ResolveNamed(
            requested,
            LoweringParallelismEnvironmentVariableName,
            compatibilityParallelism is > 0
                ? Math.Min(compatibilityParallelism.Value, 4)
                : Math.Min(Math.Max(1, Environment.ProcessorCount), 4));

    private static int ResolveNamed(int? requested, string environmentVariable, int defaultValue)
    {
        if (requested is > 0) return requested.Value;
        var fromEnvironment = Environment.GetEnvironmentVariable(environmentVariable);
        return int.TryParse(fromEnvironment, out var parsed) && parsed > 0 ? parsed : Math.Max(1, defaultValue);
    }
}
