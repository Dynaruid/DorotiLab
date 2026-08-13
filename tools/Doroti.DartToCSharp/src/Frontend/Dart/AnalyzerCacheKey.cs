using System.Security.Cryptography;
using System.Text;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class AnalyzerCacheKey
{
    public static string Create(
        string analyzerRoot,
        string inputPath,
        bool syntaxOnly,
        string? packageConfigPath)
    {
        var identity = AnalyzerSessionIdentity.Create(analyzerRoot, packageConfigPath);
        return Create(identity, inputPath, syntaxOnly);
    }

    public static string Create(
        AnalyzerSessionIdentity session,
        string inputPath,
        bool syntaxOnly)
    {
        var packageRoot = FindDartPackageRoot(inputPath);
        var packageState = new[] { "pubspec.yaml", "pubspec.lock", Path.Combine(".dart_tool", "package_config.json") }
            .Select(name => Path.Combine(packageRoot, name))
            .Where(File.Exists)
            .Select(path => $"{ArtifactFiles.NormalizePath(Path.GetRelativePath(packageRoot, path))}:{session.HashFile(path)}");
        var identity = $"input:{session.HashFile(inputPath)}\n" +
            $"analyzer-runtime:{session.RuntimeDigest}\n" +
            $"{string.Join('\n', packageState)}\n" +
            $"analyzer:{CompilerVersions.Analyzer}\n" +
            $"analysis-mode:{(syntaxOnly ? "syntax-only" : "resolved")}\n" +
            $"package-config:{session.PackageConfigDigest}\n";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity))).ToLowerInvariant();
    }

    private static string FindDartPackageRoot(string inputPath)
    {
        for (var directory = new DirectoryInfo(Path.GetDirectoryName(inputPath)!); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "pubspec.yaml")))
            {
                return directory.FullName;
            }
        }

        return Path.GetDirectoryName(inputPath)!;
    }
}
