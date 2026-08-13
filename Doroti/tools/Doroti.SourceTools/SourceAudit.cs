using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static partial class SourceAudit
{
    public const string SchemaVersion = "doroti.source-audit/v1";

    public static AuditReport Run(string repositoryRoot, string manifestPath, string provenancePath)
    {
        var manifest = ArtifactFiles.ReadJson<SourceManifest>(manifestPath);
        if (manifest.SchemaVersion != "doroti.source-manifest/v1")
        {
            throw new InvalidDataException($"Unsupported source manifest schema: {manifest.SchemaVersion}");
        }

        var findings = new List<AuditFinding>();
        var sources = new List<SourceInventory>();
        foreach (var source in manifest.Sources.OrderBy(item => item.Id, StringComparer.Ordinal))
        {
            var sourcePath = Path.GetFullPath(source.Path, Path.GetDirectoryName(manifestPath)!);
            if (!Directory.Exists(sourcePath))
            {
                findings.Add(new("DOTAUD001", "error", source.Id, $"Source directory is missing: {source.Path}"));
                continue;
            }

            var licensePath = Path.GetFullPath(source.License, sourcePath);
            if (!File.Exists(licensePath))
            {
                findings.Add(new("DOTAUD002", "error", source.Id, $"License file is missing: {source.License}"));
            }

            var auditedFiles = new List<AuditedFileInventory>();
            foreach (var relativePath in (source.AuditFiles ?? Array.Empty<string>()).OrderBy(path => path, StringComparer.Ordinal))
            {
                var path = RepositoryPaths.ResolveWithin(sourcePath, relativePath);
                if (!File.Exists(path))
                {
                    findings.Add(new("DOTAUD008", "error", $"{source.Id}:{relativePath}", "Selected audit/dependency-closure file is missing."));
                    continue;
                }

                auditedFiles.Add(new AuditedFileInventory(
                    ArtifactFiles.NormalizePath(relativePath),
                    new FileInfo(path).Length,
                    ArtifactFiles.Sha256(path),
                    ReadDirectDependencies(path)));
            }

            var independentRevision = ReadIndependentGitRevision(sourcePath);
            var revision = source.PinnedRevision ?? independentRevision ?? CreateSelectedContentRevision(auditedFiles);
            if (source.PinnedRevision is not null && !Regex.IsMatch(source.PinnedRevision, "^git:[0-9a-f]{40}$", RegexOptions.CultureInvariant))
            {
                findings.Add(new("DOTAUD013", "error", source.Id, "Pinned source revision must be an immutable Git commit in git:<40-hex> form."));
            }
            if (source.PinnedRevision is not null && independentRevision is not null && source.PinnedRevision != independentRevision)
            {
                findings.Add(new("DOTAUD013", "error", source.Id, $"Independent source checkout is at '{independentRevision}', not the pinned '{source.PinnedRevision}'."));
            }
            if (revision is null)
            {
                findings.Add(new("DOTAUD003", "error", source.Id, "Neither an independent Git revision nor selected content files could identify this source."));
            }

            sources.Add(new SourceInventory(
                source.Id,
                ArtifactFiles.NormalizePath(source.Path),
                revision ?? "unresolved",
                ArtifactFiles.NormalizePath(source.License),
                File.Exists(licensePath) ? ArtifactFiles.Sha256(licensePath) : "missing",
                auditedFiles.ToArray()));
        }

        var provenance = ArtifactFiles.ReadJson<ProvenanceManifest>(provenancePath);
        if (provenance.SchemaVersion != "doroti.provenance/v1")
        {
            throw new InvalidDataException($"Unsupported provenance schema: {provenance.SchemaVersion}");
        }

        var sourceIds = manifest.Sources.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        var sourceEntries = manifest.Sources.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var sourceInventories = sources.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var provenanceTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in provenance.Items.OrderBy(item => item.Target, StringComparer.Ordinal))
        {
            if (!sourceIds.Contains(item.SourceId))
            {
                findings.Add(new("DOTAUD004", "error", item.Target, $"Unknown source id: {item.SourceId}"));
            }

            var targetPath = Path.GetFullPath(item.Target, repositoryRoot);
            if (!File.Exists(targetPath))
            {
                findings.Add(new("DOTAUD005", "error", item.Target, "Promoted target is missing."));
            }

            if (string.IsNullOrWhiteSpace(item.SourcePath) || string.IsNullOrWhiteSpace(item.Decision) || string.IsNullOrWhiteSpace(item.License))
            {
                findings.Add(new("DOTAUD006", "error", item.Target, "Provenance sourcePath, decision, and license are required."));
            }

            var normalizedTarget = ArtifactFiles.NormalizePath(item.Target);
            if (!provenanceTargets.Add(normalizedTarget))
            {
                findings.Add(new("DOTAUD009", "error", item.Target, "Provenance target is duplicated."));
            }

            if (sourceEntries.TryGetValue(item.SourceId, out var sourceEntry) &&
                sourceInventories.TryGetValue(item.SourceId, out var sourceInventory))
            {
                var sourceRoot = Path.GetFullPath(sourceEntry.Path, Path.GetDirectoryName(manifestPath)!);
                var sourceFile = RepositoryPaths.ResolveWithin(sourceRoot, item.SourcePath);
                if (!File.Exists(sourceFile))
                {
                    findings.Add(new("DOTAUD010", "error", item.Target, $"Provenance source file is missing: {item.SourcePath}"));
                }

                if (!string.Equals(ArtifactFiles.NormalizePath(item.License), ArtifactFiles.NormalizePath(sourceEntry.License), StringComparison.Ordinal))
                {
                    findings.Add(new("DOTAUD011", "error", item.Target, $"Provenance license must match source manifest license '{sourceEntry.License}'."));
                }

                if (!string.Equals(item.Revision, sourceInventory.Revision, StringComparison.Ordinal))
                {
                    findings.Add(new("DOTAUD012", "error", item.Target, $"Provenance revision must match audited source revision '{sourceInventory.Revision}'."));
                }
            }
        }

        findings.AddRange(FindMissingProvenance(repositoryRoot, provenance));
        return new AuditReport(SchemaVersion, findings.All(item => item.Severity != "error"), sources.ToArray(), findings
            .OrderBy(item => item.Code, StringComparer.Ordinal)
            .ThenBy(item => item.Subject, StringComparer.Ordinal)
            .ToArray());
    }

    public static string ToMarkdown(AuditReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Doroti source audit");
        builder.AppendLine();
        builder.AppendLine($"Status: **{(report.Success ? "PASS" : "FAIL")}**");
        builder.AppendLine();
        builder.AppendLine("| Source | Revision | Hashed selected/closure files | License |");
        builder.AppendLine("|---|---:|---:|---|");
        foreach (var source in report.Sources)
        {
            builder.AppendLine($"| {source.Id} | `{source.Revision}` | {source.AuditedFiles.Length} | `{source.License}` |");
        }

        builder.AppendLine();
        builder.AppendLine("## Findings");
        builder.AppendLine();
        if (report.Findings.Length == 0)
        {
            builder.AppendLine("No findings.");
        }
        else
        {
            foreach (var finding in report.Findings)
            {
                builder.AppendLine($"- **{finding.Code}** ({finding.Severity}) `{finding.Subject}`: {finding.Message}");
            }
        }

        return builder.ToString();
    }

    private static IEnumerable<AuditFinding> FindMissingProvenance(string repositoryRoot, ProvenanceManifest provenance)
    {
        var tracked = provenance.Items.Select(item => ArtifactFiles.NormalizePath(item.Target)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var roots = new[] { "src", "samples" };
        foreach (var root in roots)
        {
            var path = Path.Combine(repositoryRoot, root);
            if (!Directory.Exists(path))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).OrderBy(item => item, StringComparer.Ordinal))
            {
                var text = File.ReadAllText(file);
                var match = ProvenanceHeaderRegex().Match(text);
                if (!match.Success)
                {
                    continue;
                }

                var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(repositoryRoot, file));
                if (!tracked.Contains(relative))
                {
                    yield return new("DOTAUD007", "error", relative, "File declares an adapted-from header but has no provenance manifest entry.");
                }
            }
        }
    }

    private static string? ReadIndependentGitRevision(string sourcePath)
    {
        var topLevel = ProcessRunner.Run("git", new[] { "-C", sourcePath, "rev-parse", "--show-toplevel" }, sourcePath);
        if (topLevel.ExitCode != 0 || !string.Equals(Path.GetFullPath(topLevel.StandardOutput), Path.GetFullPath(sourcePath), StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var revision = ProcessRunner.Run("git", new[] { "-C", sourcePath, "rev-parse", "HEAD" }, sourcePath);
        return revision.ExitCode == 0 && revision.StandardOutput.Length == 40 ? $"git:{revision.StandardOutput}" : null;
    }

    private static string? CreateSelectedContentRevision(IReadOnlyCollection<AuditedFileInventory> files)
    {
        if (files.Count == 0)
        {
            return null;
        }

        var content = string.Join("\n", files.OrderBy(file => file.Path, StringComparer.Ordinal).Select(file => $"{file.Path}:{file.Sha256}")) + "\n";
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
        return $"selected-content-sha256:{hash}";
    }

    private static string[] ReadDirectDependencies(string path)
    {
        var extension = Path.GetExtension(path);
        var lines = File.ReadLines(path);
        var dependencies = new List<string>();
        foreach (var line in lines)
        {
            Match match;
            if (extension.Equals(".dart", StringComparison.OrdinalIgnoreCase))
            {
                match = Regex.Match(line, @"^\s*(?:import|export|part)\s+['""]([^'""]+)['""]", RegexOptions.CultureInvariant);
            }
            else if (extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                match = Regex.Match(line, @"^\s*using\s+(?:static\s+)?([^;=]+)\s*;", RegexOptions.CultureInvariant);
            }
            else
            {
                continue;
            }

            if (match.Success)
            {
                dependencies.Add(match.Groups[1].Value.Trim());
            }
        }

        return dependencies.Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray();
    }

    [GeneratedRegex(@"^\s*//\s*Doroti-Adapted-From:", RegexOptions.Multiline | RegexOptions.CultureInvariant)]
    private static partial Regex ProvenanceHeaderRegex();
}

public sealed record SourceManifest(string SchemaVersion, SourceEntry[] Sources);
public sealed record SourceEntry(
    string Id,
    string Path,
    string License,
    string Role,
    string[]? AuditFiles = null,
    string? UpstreamRepository = null,
    string? UpstreamRef = null,
    string? PinnedRevision = null);
public sealed record ProvenanceManifest(string SchemaVersion, ProvenanceEntry[] Items);
public sealed record ProvenanceEntry(string Target, string SourceId, string SourcePath, string Revision, string License, string Decision);
public sealed record AuditReport(string SchemaVersion, bool Success, SourceInventory[] Sources, AuditFinding[] Findings);
public sealed record SourceInventory(string Id, string Path, string Revision, string License, string LicenseSha256, AuditedFileInventory[] AuditedFiles);
public sealed record AuditedFileInventory(string Path, long Bytes, string Sha256, string[] DirectDependencies);
public sealed record AuditFinding(string Code, string Severity, string Subject, string Message);
