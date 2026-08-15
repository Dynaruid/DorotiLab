using System.Security.Cryptography;
using System.Text;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class WorkspaceFingerprint
{
    public static string Compute(string manifestPath)
    {
        var manifest = ArtifactFiles.ReadJson<SelectionManifest>(manifestPath);
        var manifestDirectory = Path.GetDirectoryName(Path.GetFullPath(manifestPath))!;
        ApplicationGraphPlan? applicationPlan = null;
        if (manifest.Application is not null)
        {
            (manifest, applicationPlan) = ApplicationGraphResolver.Expand(manifest, manifestDirectory, previousOutputDirectory: null);
        }
        var analyzerHome = AnalyzerHomeResolver.Resolve(manifestPath, manifest);
        var analyzerProject = analyzerHome.AnalyzerRoot;
        var repositoryRoot = analyzerHome.DorotiRoot;
        var compilerRoot = Path.Combine(analyzerHome.RepositoryRoot, "tools", "Doroti.DartToCSharp");
        var buildPropsPath = Path.Combine(repositoryRoot, "Directory.Build.props");
        var inputs = new List<string>
        {
            $"converter:{CompilerVersions.Converter}",
            $"build-props:{ArtifactFiles.Sha256(buildPropsPath)}",
            $"manifest:{ArtifactFiles.Sha256(manifestPath)}",
            $"analyzer-runtime:{AnalyzerRuntimeClosure.ComputeDigest(analyzerProject)}",
            $"flutter-baseline:{ArtifactFiles.Sha256(Path.GetFullPath(manifest.FlutterBaseline, manifestDirectory))}",
        };

        inputs.AddRange(Directory.EnumerateFiles(Path.Combine(compilerRoot, "src"), "*.cs", SearchOption.AllDirectories)
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(compilerRoot, path)), StringComparer.Ordinal)
            .Select(path => $"compiler-source:{ArtifactFiles.NormalizePath(Path.GetRelativePath(compilerRoot, path))}:{ArtifactFiles.Sha256(path)}"));

        var runtimeRoot = Path.Combine(repositoryRoot, "src", "Doroti.Runtime");
        inputs.AddRange(Directory.GetFiles(runtimeRoot, "*", SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetExtension(path) is ".cs" or ".csproj")
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => $"runtime-binding:{ArtifactFiles.NormalizePath(Path.GetRelativePath(repositoryRoot, path))}:{ArtifactFiles.Sha256(path)}"));
        var uiContractRoot = Path.Combine(repositoryRoot, "src", "Doroti.Ui");
        inputs.AddRange(Directory.GetFiles(uiContractRoot, "*", SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetExtension(path) is ".cs" or ".csproj")
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => $"dart-ui-contract:{ArtifactFiles.NormalizePath(Path.GetRelativePath(repositoryRoot, path))}:{ArtifactFiles.Sha256(path)}"));
        inputs.AddRange(manifest.Inputs
            .OrderBy(item => item.Path, StringComparer.Ordinal)
            .Select(item => $"input:{ArtifactFiles.NormalizePath(item.Path)}:{ArtifactFiles.Sha256(CompilerInputResolver.Resolve(manifest, manifestDirectory, item.Path))}"));
        if (applicationPlan is not null)
        {
            inputs.Add($"application-resource-manifest:{ArtifactFiles.Sha256(applicationPlan.ResourceManifestPath)}");
            inputs.Add($"application-plugin-manifest:{ArtifactFiles.Sha256(applicationPlan.PluginManifestPath)}");
            inputs.Add($"application-contract:{applicationPlan.ContractSha256}");
        }

        if (!string.IsNullOrWhiteSpace(manifest.PackageRoot))
        {
            var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
            foreach (var name in new[] { "pubspec.yaml", "pubspec.lock" })
            {
                var path = Path.Combine(packageRoot, name);
                if (File.Exists(path))
                {
                    inputs.Add($"package-{name}:{ArtifactFiles.Sha256(path)}");
                }
            }
        }

        var bytes = Encoding.UTF8.GetBytes(string.Join('\n', inputs) + "\n");
        return System.Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }
}
