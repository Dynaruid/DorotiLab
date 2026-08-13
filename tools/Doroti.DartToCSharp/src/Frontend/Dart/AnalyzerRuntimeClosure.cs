using System.Security.Cryptography;
using System.Text;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class AnalyzerRuntimeClosure
{
    private static readonly string[] RootFiles =
    [
        "pubspec.yaml",
        "pubspec.lock",
        "flutter_package_config.json",
        "entrypoints/extract.dart",
        "entrypoints/extract_batch.dart",
    ];

    public static string ComputeDigest(string analyzerRoot)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var path in EnumerateFiles(analyzerRoot))
        {
            var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(analyzerRoot, path));
            hash.AppendData(Encoding.UTF8.GetBytes($"{relative}\0{ArtifactFiles.Sha256(path)}\n"));
        }

        return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
    }

    internal static string[] EnumerateFiles(string analyzerRoot)
    {
        var files = RootFiles
            .Select(relative => Path.Combine(analyzerRoot, relative))
            .Where(File.Exists)
            .Concat(EnumerateTree(analyzerRoot, "lib"))
            .Concat(EnumerateTree(analyzerRoot, "stubs"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(analyzerRoot, path)), StringComparer.Ordinal)
            .ToArray();
        if (!files.Any(path => string.Equals(
                ArtifactFiles.NormalizePath(Path.GetRelativePath(analyzerRoot, path)),
                "entrypoints/extract.dart",
                StringComparison.Ordinal)))
        {
            throw new FileNotFoundException(
                "Analyzer runtime closure is missing entrypoints/extract.dart.",
                Path.Combine(analyzerRoot, "entrypoints", "extract.dart"));
        }

        return files;
    }

    private static IEnumerable<string> EnumerateTree(string analyzerRoot, string relativeRoot)
    {
        var root = Path.Combine(analyzerRoot, relativeRoot);
        return Directory.Exists(root)
            ? Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                .Where(path => !ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path))
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Any(part => part == ".dart_tool"))
            : [];
    }
}
