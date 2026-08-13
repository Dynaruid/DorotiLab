using System.Text.RegularExpressions;

namespace Doroti.DartToCSharp;

internal sealed record AnalyzerHome(string RepositoryRoot, string DorotiRoot, string AnalyzerRoot);

internal static class AnalyzerHomeResolver
{
    private const string PackageName = "doroti_dart_analyzer";

    public static AnalyzerHome Resolve(string manifestPath, SelectionManifest manifest)
    {
        var manifestDirectory = Path.GetDirectoryName(Path.GetFullPath(manifestPath))!;
        var repositoryRoot = FindRepositoryRoot(manifestDirectory);
        var analyzerRoot = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "analyzer");
        var pubspecPath = Path.Combine(analyzerRoot, "pubspec.yaml");
        var entryPointPath = Path.Combine(analyzerRoot, "entrypoints", "extract.dart");
        if (!File.Exists(pubspecPath) || !File.Exists(entryPointPath))
        {
            throw new DirectoryNotFoundException($"Compiler-owned Dart analyzer is incomplete: {analyzerRoot}");
        }

        var pubspec = File.ReadAllText(pubspecPath);
        var packageMatch = Regex.Match(pubspec, @"(?m)^name:\s*(?<name>[^\s#]+)\s*$", RegexOptions.CultureInvariant);
        if (!packageMatch.Success || !string.Equals(packageMatch.Groups["name"].Value, PackageName, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Compiler-owned analyzer package must be named {PackageName}: {pubspecPath}");
        }

        if (string.Equals(manifest.SchemaVersion, "doroti.converter-selection/v4", StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(manifest.AnalyzerProject))
            {
                throw new InvalidDataException("Selection schema v4 must not inject analyzerProject; the compiler owns its Dart frontend.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(manifest.AnalyzerProject))
        {
            var requested = Path.GetFullPath(manifest.AnalyzerProject, manifestDirectory);
            if (!string.Equals(
                    Path.TrimEndingDirectorySeparator(requested),
                    Path.TrimEndingDirectorySeparator(analyzerRoot),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"Legacy selection analyzerProject must resolve to the compiler-owned analyzer: {requested}");
            }
        }

        return new(repositoryRoot, Path.Combine(repositoryRoot, "Doroti"), analyzerRoot);
    }

    private static string FindRepositoryRoot(string startDirectory)
    {
        for (var directory = new DirectoryInfo(Path.GetFullPath(startDirectory)); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Doroti", "Doroti.slnx")) &&
                File.Exists(Path.Combine(directory.FullName, "tools", "Doroti.DartToCSharp", "Doroti.DartToCSharp.csproj")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException($"Could not find the DorotiLab compiler root from {startDirectory}.");
    }
}
