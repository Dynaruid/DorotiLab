using System.Text;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class VendorReview
{
    public static VendorReviewReport Create(
        string repositoryRoot,
        string sourceManifestPath,
        string selectionPath,
        string provenancePath,
        string outputDirectory)
    {
        var sourceManifest = ArtifactFiles.ReadJson<SourceManifest>(sourceManifestPath);
        var selection = ArtifactFiles.ReadJson<VendorSelectionManifest>(selectionPath);
        var source = sourceManifest.Sources.SingleOrDefault(item => item.Id == selection.SourceId)
            ?? throw new InvalidDataException($"Unknown vendor source id: {selection.SourceId}");
        var sourceRoot = Path.GetFullPath(source.Path, Path.GetDirectoryName(sourceManifestPath)!);
        Directory.CreateDirectory(outputDirectory);

        var audit = VendorAudit.Run(repositoryRoot, sourceManifestPath, selectionPath, provenancePath);
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "vendor-audit.json"), audit);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "vendor-audit.md"), VendorAudit.ToMarkdown(audit));

        var entries = new List<VendorReviewEntry>();
        var patch = new StringBuilder();
        patch.AppendLine("# Avalonia vendor update review");
        patch.AppendLine();
        patch.AppendLine("This bundle never modifies vendor production source. Upstream content is copied only into this review directory.");
        foreach (var entry in selection.Entries.OrderBy(item => item.SourcePath, StringComparer.Ordinal))
        {
            var sourcePath = RepositoryPaths.ResolveWithin(sourceRoot, entry.SourcePath);
            if (!File.Exists(sourcePath))
            {
                continue;
            }

            var currentSourceHash = ArtifactFiles.Sha256(sourcePath);
            string? currentAdaptedHash = null;
            string? bundleFile = null;
            if (!string.IsNullOrWhiteSpace(entry.TargetPath))
            {
                var targetPath = RepositoryPaths.ResolveWithin(repositoryRoot, entry.TargetPath);
                currentAdaptedHash = File.Exists(targetPath) ? ArtifactFiles.Sha256(targetPath) : null;
                bundleFile = $"{entries.Count + 1:D3}-{Path.GetFileName(entry.SourcePath)}.upstream";
                File.Copy(sourcePath, Path.Combine(outputDirectory, bundleFile), overwrite: true);
                AppendPatch(patch, entry, sourcePath, targetPath, bundleFile);
            }

            entries.Add(new(
                ArtifactFiles.NormalizePath(entry.SourcePath),
                entry.Disposition,
                string.IsNullOrWhiteSpace(entry.TargetPath) ? null : ArtifactFiles.NormalizePath(entry.TargetPath),
                entry.SourceSha256,
                currentSourceHash,
                !string.Equals(entry.SourceSha256, currentSourceHash, StringComparison.Ordinal),
                entry.AdaptedSha256,
                currentAdaptedHash,
                entry.AdaptedSha256 is not null && !string.Equals(entry.AdaptedSha256, currentAdaptedHash, StringComparison.Ordinal),
                bundleFile));
        }

        var report = new VendorReviewReport("doroti.vendor-review/v1", selection.SourceId, audit.Success, entries.ToArray());
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "vendor-review.json"), report);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "review.patch.md"), patch.ToString());
        return report;
    }

    private static void AppendPatch(StringBuilder patch, VendorSelectionEntry entry, string sourcePath, string targetPath, string bundleFile)
    {
        var before = File.Exists(targetPath) ? File.ReadAllText(targetPath).Replace("\r\n", "\n", StringComparison.Ordinal) : string.Empty;
        var after = File.ReadAllText(sourcePath).Replace("\r\n", "\n", StringComparison.Ordinal);
        var status = !File.Exists(targetPath) ? "add candidate" : before == after ? "byte-identical" : "manual adaptation required";
        patch.AppendLine();
        patch.AppendLine($"## {status}: `{ArtifactFiles.NormalizePath(entry.TargetPath!)}`");
        patch.AppendLine();
        patch.AppendLine($"- Disposition: `{entry.Disposition}`");
        patch.AppendLine($"- Upstream review copy: `{bundleFile}`");
        patch.AppendLine($"- Current target SHA-256: `{(File.Exists(targetPath) ? ArtifactFiles.Sha256(targetPath) : "missing")}`");
        patch.AppendLine($"- Current upstream SHA-256: `{ArtifactFiles.Sha256(sourcePath)}`");
        if (before == after)
        {
            return;
        }

        var beforeLines = before.Length == 0 ? Array.Empty<string>() : before.TrimEnd('\n').Split('\n');
        var afterLines = after.Length == 0 ? Array.Empty<string>() : after.TrimEnd('\n').Split('\n');
        patch.AppendLine();
        patch.AppendLine("```diff");
        patch.AppendLine($"--- a/{ArtifactFiles.NormalizePath(entry.TargetPath!)}");
        patch.AppendLine($"+++ upstream/{ArtifactFiles.NormalizePath(entry.SourcePath)}");
        patch.AppendLine($"@@ -1,{beforeLines.Length} +1,{afterLines.Length} @@");
        foreach (var line in beforeLines)
        {
            patch.AppendLine("-" + line);
        }
        foreach (var line in afterLines)
        {
            patch.AppendLine("+" + line);
        }
        patch.AppendLine("```");
    }
}

public sealed record VendorReviewReport(string SchemaVersion, string SourceId, bool AuditSuccess, VendorReviewEntry[] Entries);

public sealed record VendorReviewEntry(
    string SourcePath,
    string Disposition,
    string? TargetPath,
    string ExpectedSourceSha256,
    string CurrentSourceSha256,
    bool SourceDrift,
    string? ExpectedAdaptedSha256,
    string? CurrentAdaptedSha256,
    bool AdaptedDrift,
    string? BundleFile);
