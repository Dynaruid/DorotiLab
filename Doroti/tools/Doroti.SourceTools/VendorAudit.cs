using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class VendorAudit
{
    public const string SchemaVersion = "doroti.vendor-audit/v1";
    private const string SelectionSchemaVersion = "doroti.vendor-selection/v1";
    private static readonly string[] AllowedDispositions = ["copy", "adapt", "rewrite", "exclude"];

    public static VendorAuditReport Run(string repositoryRoot, string sourceManifestPath, string selectionPath, string provenancePath)
    {
        var sourceManifest = ArtifactFiles.ReadJson<SourceManifest>(sourceManifestPath);
        var selection = ArtifactFiles.ReadJson<VendorSelectionManifest>(selectionPath);
        var provenance = ArtifactFiles.ReadJson<VendorProvenanceManifest>(provenancePath);
        if (selection.SchemaVersion != SelectionSchemaVersion)
        {
            throw new InvalidDataException($"Unsupported vendor selection schema: {selection.SchemaVersion}");
        }
        if (provenance.SchemaVersion != "doroti.vendor-provenance/v1")
        {
            throw new InvalidDataException($"Unsupported vendor provenance schema: {provenance.SchemaVersion}");
        }

        var findings = new List<AuditFinding>();
        var source = sourceManifest.Sources.SingleOrDefault(item => item.Id == selection.SourceId);
        if (source is null)
        {
            findings.Add(new("DOTVEN001", "error", selection.SourceId, "Vendor source is not declared in source-manifest.json."));
            return CreateReport(selection, provenance, findings, 0);
        }

        var sourceRoot = Path.GetFullPath(source.Path, Path.GetDirectoryName(sourceManifestPath)!);
        ValidateAllowlist(selection, findings);
        var selectedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in selection.Entries.OrderBy(item => item.SourcePath, StringComparer.Ordinal))
        {
            ValidateEntry(repositoryRoot, sourceRoot, entry, selection, selectedTargets, findings);
        }
        ValidateProvenance(repositoryRoot, sourceRoot, selection, provenance, selectedTargets, findings);

        var ownedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var ownedFile in (selection.OwnedFiles ?? []).OrderBy(item => item, StringComparer.Ordinal))
        {
            var normalized = ArtifactFiles.NormalizePath(ownedFile);
            if (!ownedTargets.Add(normalized) || selectedTargets.Contains(normalized))
            {
                findings.Add(new("DOTVEN015", "error", normalized, "Doroti-owned vendor infrastructure must be unique and outside the Avalonia-derived selection."));
                continue;
            }

            var fullPath = RepositoryPaths.ResolveWithin(repositoryRoot, normalized);
            if (!File.Exists(fullPath))
            {
                findings.Add(new("DOTVEN015", "error", normalized, "Doroti-owned vendor infrastructure file is missing."));
                continue;
            }

            var text = File.ReadAllText(fullPath);
            if (!text.Contains("Doroti-owned", StringComparison.Ordinal))
            {
                findings.Add(new("DOTVEN015", "error", normalized, "Doroti-owned vendor infrastructure must carry an explicit ownership header."));
            }
            ValidateForbiddenDependencies(normalized, text, selection, findings);
        }

        var compiledSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var projectPath in selection.VendorProjects.OrderBy(item => item, StringComparer.Ordinal))
        {
            var project = RepositoryPaths.ResolveWithin(repositoryRoot, projectPath);
            if (!File.Exists(project))
            {
                findings.Add(new("DOTVEN011", "error", projectPath, "Vendor project is missing."));
                continue;
            }

            ValidateProjectReferences(repositoryRoot, projectPath, project, selection, findings);
            foreach (var compiledSource in ReadCompiledSources(repositoryRoot, project))
            {
                compiledSources.Add(compiledSource);
                if (!selectedTargets.Contains(compiledSource) && !ownedTargets.Contains(compiledSource))
                {
                    findings.Add(new("DOTVEN008", "error", compiledSource, "Vendor project compiles a C# file outside the approved selection."));
                    var fullPath = RepositoryPaths.ResolveWithin(repositoryRoot, compiledSource);
                    ValidateForbiddenDependencies(compiledSource, File.ReadAllText(fullPath), selection, findings);
                }
            }
        }

        if (selection.VendorProjects.Length > 0)
        {
            foreach (var target in selectedTargets
                         .Concat(ownedTargets)
                         .Except(compiledSources, StringComparer.OrdinalIgnoreCase)
                         .OrderBy(item => item, StringComparer.Ordinal))
            {
                findings.Add(new("DOTVEN009", "error", target, "Selected vendor target is not compiled by a declared vendor project."));
            }
        }

        return CreateReport(selection, provenance, findings, compiledSources.Count);
    }

    public static string ToMarkdown(VendorAuditReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# Avalonia vendor selection audit");
        builder.AppendLine();
        builder.AppendLine($"Status: **{(report.Success ? "PASS" : "FAIL")}**");
        builder.AppendLine();
        builder.AppendLine($"- Source: `{report.SourceId}`");
        builder.AppendLine($"- Selection entries: {report.SelectionCount}");
        builder.AppendLine($"- Vendor projects: {report.VendorProjectCount}");
        builder.AppendLine($"- Compiled vendor sources: {report.CompiledSourceCount}");
        builder.AppendLine($"- Provenance entries: {report.ProvenanceCount}");
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

    private static void ValidateEntry(
        string repositoryRoot,
        string sourceRoot,
        VendorSelectionEntry entry,
        VendorSelectionManifest selection,
        HashSet<string> selectedTargets,
        List<AuditFinding> findings)
    {
        if (!AllowedDispositions.Contains(entry.Disposition, StringComparer.Ordinal))
        {
            findings.Add(new("DOTVEN002", "error", entry.SourcePath, $"Unsupported disposition: {entry.Disposition}"));
        }

        var sourcePath = RepositoryPaths.ResolveWithin(sourceRoot, entry.SourcePath);
        if (!File.Exists(sourcePath))
        {
            findings.Add(new("DOTVEN003", "error", entry.SourcePath, "Selected Avalonia source file is missing."));
        }
        else
        {
            var actualDependencies = ReadDirectDependencies(sourcePath);
            var expectedDependencies = entry.DirectDependencies
                .Distinct(StringComparer.Ordinal)
                .OrderBy(item => item, StringComparer.Ordinal)
                .ToArray();
            if (!actualDependencies.SequenceEqual(expectedDependencies, StringComparer.Ordinal))
            {
                findings.Add(new(
                    "DOTVEN006",
                    "error",
                    entry.SourcePath,
                    $"Direct dependency drift. Expected [{string.Join(", ", expectedDependencies)}], actual [{string.Join(", ", actualDependencies)}]."));
            }
        }

        if (entry.Disposition == "exclude")
        {
            if (!string.IsNullOrWhiteSpace(entry.TargetPath))
            {
                findings.Add(new("DOTVEN010", "error", entry.SourcePath, "Excluded entries must not declare a target."));
            }
            return;
        }

        if (string.IsNullOrWhiteSpace(entry.TargetPath))
        {
            findings.Add(new("DOTVEN005", "error", entry.SourcePath, "copy/adapt/rewrite entries require targetPath."));
            return;
        }

        var normalizedTarget = ArtifactFiles.NormalizePath(entry.TargetPath);
        if (!selectedTargets.Add(normalizedTarget))
        {
            findings.Add(new("DOTVEN012", "error", normalizedTarget, "Vendor target is selected more than once."));
        }

        var targetPath = RepositoryPaths.ResolveWithin(repositoryRoot, normalizedTarget);
        if (!File.Exists(targetPath))
        {
            findings.Add(new("DOTVEN005", "error", normalizedTarget, "Selected vendor target is missing."));
            return;
        }

        ValidateForbiddenDependencies(normalizedTarget, File.ReadAllText(targetPath), selection, findings);
    }

    private static void ValidateAllowlist(VendorSelectionManifest selection, List<AuditFinding> findings)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var allowance in selection.DependencyAllowlist)
        {
            var key = $"{ArtifactFiles.NormalizePath(allowance.Path)}\n{allowance.Dependency}";
            if (string.IsNullOrWhiteSpace(allowance.Path) ||
                string.IsNullOrWhiteSpace(allowance.Reason) ||
                !selection.ForbiddenDependencies.Contains(allowance.Dependency, StringComparer.Ordinal) ||
                !keys.Add(key))
            {
                findings.Add(new("DOTVEN013", "error", allowance.Path, "Dependency allowlist entries must be unique, name a configured forbidden dependency, and include a reason."));
            }
        }
    }

    private static void ValidateProvenance(
        string repositoryRoot,
        string sourceRoot,
        VendorSelectionManifest selection,
        VendorProvenanceManifest provenance,
        HashSet<string> selectedTargets,
        List<AuditFinding> findings)
    {
        var byTarget = new Dictionary<string, VendorProvenanceEntry>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in provenance.Items)
        {
            var target = ArtifactFiles.NormalizePath(item.TargetPath);
            if (!byTarget.TryAdd(target, item))
            {
                findings.Add(new("DOTVEN014", "error", target, "Vendor provenance target is duplicated."));
            }
            if (!selectedTargets.Contains(target))
            {
                findings.Add(new("DOTVEN014", "error", target, "Vendor provenance target is outside the approved selection."));
            }

            var selected = selection.Entries.FirstOrDefault(entry =>
                !string.IsNullOrWhiteSpace(entry.TargetPath) &&
                string.Equals(ArtifactFiles.NormalizePath(entry.TargetPath), target, StringComparison.OrdinalIgnoreCase));
            if (selected is not null && !string.Equals(ArtifactFiles.NormalizePath(selected.SourcePath), ArtifactFiles.NormalizePath(item.SourcePath), StringComparison.OrdinalIgnoreCase))
            {
                findings.Add(new("DOTVEN015", "error", target, "Vendor provenance sourcePath does not match the selection."));
            }
            if (string.IsNullOrWhiteSpace(item.SourceRevision) || string.IsNullOrWhiteSpace(item.License) || string.IsNullOrWhiteSpace(item.LocalChanges))
            {
                findings.Add(new("DOTVEN015", "error", target, "Vendor provenance requires sourceRevision, license, and localChanges."));
            }
            else if (!File.Exists(RepositoryPaths.ResolveWithin(sourceRoot, item.License)))
            {
                findings.Add(new("DOTVEN015", "error", target, $"Vendor provenance license is missing: {item.License}"));
            }

            foreach (var patch in item.PatchFiles)
            {
                if (!File.Exists(RepositoryPaths.ResolveWithin(repositoryRoot, patch)))
                {
                    findings.Add(new("DOTVEN015", "error", target, $"Vendor provenance patch is missing: {patch}"));
                }
            }
        }

        foreach (var target in selectedTargets.Except(byTarget.Keys, StringComparer.OrdinalIgnoreCase).OrderBy(item => item, StringComparer.Ordinal))
        {
            findings.Add(new("DOTVEN014", "error", target, "Selected vendor target has no provenance entry."));
        }
    }

    private static string[] ReadDirectDependencies(string path)
    {
        var dependencies = new List<string>();
        foreach (var line in File.ReadLines(path))
        {
            var match = Regex.Match(line, @"^\s*(?:global\s+)?using\s+(?:static\s+)?(?:[A-Za-z_]\w*\s*=\s*)?([^;=]+)\s*;", RegexOptions.CultureInvariant);
            if (match.Success)
            {
                dependencies.Add(match.Groups[1].Value.Trim());
            }
        }
        return dependencies.Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray();
    }

    private static void ValidateProjectReferences(
        string repositoryRoot,
        string projectPath,
        string fullProjectPath,
        VendorSelectionManifest selection,
        List<AuditFinding> findings)
    {
        var project = XDocument.Load(fullProjectPath);
        foreach (var reference in project.Descendants().Where(element => element.Name.LocalName is "PackageReference" or "ProjectReference"))
        {
            var include = reference.Attribute("Include")?.Value ?? string.Empty;
            foreach (var dependency in selection.ForbiddenDependencies)
            {
                if (include.Contains(dependency, StringComparison.OrdinalIgnoreCase) && !IsAllowed(projectPath, dependency, selection))
                {
                    findings.Add(new("DOTVEN007", "error", projectPath, $"Vendor project references forbidden dependency '{include}'."));
                }
            }
        }
    }

    private static void ValidateForbiddenDependencies(
        string targetPath,
        string text,
        VendorSelectionManifest? selection,
        List<AuditFinding> findings)
    {
        if (selection is null)
        {
            return;
        }

        foreach (var dependency in selection.ForbiddenDependencies.OrderBy(item => item, StringComparer.Ordinal))
        {
            if (text.Contains(dependency, StringComparison.Ordinal) && !IsAllowed(targetPath, dependency, selection))
            {
                findings.Add(new("DOTVEN007", "error", targetPath, $"Forbidden vendor dependency entered the selected source: {dependency}"));
            }
        }
    }

    private static bool IsAllowed(string path, string dependency, VendorSelectionManifest selection) =>
        selection.DependencyAllowlist.Any(item =>
            string.Equals(ArtifactFiles.NormalizePath(item.Path), ArtifactFiles.NormalizePath(path), StringComparison.OrdinalIgnoreCase) &&
            string.Equals(item.Dependency, dependency, StringComparison.Ordinal));

    private static IEnumerable<string> ReadCompiledSources(string repositoryRoot, string projectPath)
    {
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var project = XDocument.Load(projectPath);
        var defaultItems = !project.Descendants().Any(element =>
            element.Name.LocalName == "EnableDefaultCompileItems" &&
            string.Equals(element.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase));
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (defaultItems)
        {
            foreach (var file in Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var relativeToProject = ArtifactFiles.NormalizePath(Path.GetRelativePath(projectDirectory, file));
                if (!relativeToProject.StartsWith("bin/", StringComparison.OrdinalIgnoreCase) &&
                    !relativeToProject.StartsWith("obj/", StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(Path.GetFullPath(file));
                }
            }
        }

        foreach (var include in project.Descendants().Where(element => element.Name.LocalName == "Compile").Select(element => element.Attribute("Include")?.Value).Where(value => !string.IsNullOrWhiteSpace(value)).Cast<string>())
        {
            foreach (var file in ExpandCompilePath(projectDirectory, include))
            {
                files.Add(file);
            }
        }

        foreach (var remove in project.Descendants().Where(element => element.Name.LocalName == "Compile").Select(element => element.Attribute("Remove")?.Value).Where(value => !string.IsNullOrWhiteSpace(value)).Cast<string>())
        {
            foreach (var file in ExpandCompilePath(projectDirectory, remove))
            {
                files.Remove(file);
            }
        }

        return files
            .Select(file => ArtifactFiles.NormalizePath(Path.GetRelativePath(repositoryRoot, file)))
            .OrderBy(item => item, StringComparer.Ordinal)
            .ToArray();
    }

    private static IEnumerable<string> ExpandCompilePath(string projectDirectory, string expression)
    {
        foreach (var part in expression.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!part.Contains('*') && !part.Contains('?'))
            {
                var fullPath = Path.GetFullPath(part, projectDirectory);
                if (File.Exists(fullPath))
                {
                    yield return fullPath;
                }
                continue;
            }

            var normalizedPattern = ArtifactFiles.NormalizePath(part);
            foreach (var file in Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
            {
                var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(projectDirectory, file));
                if (System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(normalizedPattern, relative, ignoreCase: OperatingSystem.IsWindows()))
                {
                    yield return Path.GetFullPath(file);
                }
            }
        }
    }

    private static VendorAuditReport CreateReport(VendorSelectionManifest selection, VendorProvenanceManifest provenance, List<AuditFinding> findings, int compiledSourceCount) =>
        new(
            SchemaVersion,
            findings.All(item => item.Severity != "error"),
            selection.SourceId,
            selection.Entries.Length,
            selection.VendorProjects.Length,
            compiledSourceCount,
            provenance.Items.Length,
            findings.OrderBy(item => item.Code, StringComparer.Ordinal).ThenBy(item => item.Subject, StringComparer.Ordinal).ThenBy(item => item.Message, StringComparer.Ordinal).ToArray());
}

public sealed record VendorSelectionManifest(
    string SchemaVersion,
    string SourceId,
    string[] VendorProjects,
    string[] ForbiddenDependencies,
    VendorDependencyAllowance[] DependencyAllowlist,
    VendorSelectionEntry[] Entries,
    string[]? OwnedFiles = null);

public sealed record VendorDependencyAllowance(string Path, string Dependency, string Reason);

public sealed record VendorProvenanceManifest(string SchemaVersion, VendorProvenanceEntry[] Items);

public sealed record VendorProvenanceEntry(
    string TargetPath,
    string SourcePath,
    string SourceRevision,
    string License,
    string LocalChanges,
    string[] PatchFiles);

public sealed record VendorSelectionEntry(
    string SourcePath,
    string Disposition,
    string SourceSha256,
    string? TargetPath,
    string? AdaptedSha256,
    string[] DirectDependencies);

public sealed record VendorAuditReport(
    string SchemaVersion,
    bool Success,
    string SourceId,
    int SelectionCount,
    int VendorProjectCount,
    int CompiledSourceCount,
    int ProvenanceCount,
    AuditFinding[] Findings);
