using System.Collections.Concurrent;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed class AnalyzerSessionIdentity
{
    private readonly ConcurrentDictionary<string, string> _fileHashes = new(StringComparer.OrdinalIgnoreCase);

    private AnalyzerSessionIdentity(string analyzerRoot, string runtimeDigest, string packageConfigDigest)
    {
        AnalyzerRoot = analyzerRoot;
        RuntimeDigest = runtimeDigest;
        PackageConfigDigest = packageConfigDigest;
    }

    public string AnalyzerRoot { get; }
    public string RuntimeDigest { get; }
    public string PackageConfigDigest { get; }

    public static AnalyzerSessionIdentity Create(string analyzerRoot, string? packageConfigPath) => new(
        Path.GetFullPath(analyzerRoot),
        AnalyzerRuntimeClosure.ComputeDigest(analyzerRoot),
        packageConfigPath is null ? "none" : ArtifactFiles.Sha256(packageConfigPath));

    public string HashFile(string path) => _fileHashes.GetOrAdd(Path.GetFullPath(path), ArtifactFiles.Sha256);
}
