namespace Doroti.DartToCSharp;

/// <summary>
/// Explicit worker-policy boundary. The default remains one persistent analyzer context;
/// more workers are enabled only after the benchmark demonstrates a throughput benefit.
/// </summary>
internal sealed record AnalyzerWorkerPoolClient(int WorkerCount)
{
    public int BoundedWorkerCount => Math.Max(1, WorkerCount);
}
