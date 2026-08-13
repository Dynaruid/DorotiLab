using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class FlutterFrameworkSliceAudit
{
    public const string SchemaVersion = "doroti.flutter-framework-slice-audit/v1";

    private static readonly string[] Dispositions =
    [
        "compiler-output-supported",
        "manual-replacement",
        "runtime-adoption",
    ];

    public static FlutterFrameworkSliceAuditReport Run(string repositoryRoot, string evidencePath)
    {
        var evidence = ArtifactFiles.ReadJson<FlutterFrameworkSliceEvidence>(evidencePath);
        if (evidence.SchemaVersion != "doroti.flutter-framework-slice/v1")
        {
            throw new InvalidDataException($"Unsupported Flutter framework slice schema: {evidence.SchemaVersion}");
        }

        var root = Path.GetFullPath(repositoryRoot);
        var findings = new List<AuditFinding>();
        CheckPath(root, evidence.Upstream.SourcePath, "upstream-source", findings);
        CheckPath(root, evidence.Upstream.LicensePath, "upstream-license", findings);
        foreach (var artifact in evidence.Artifacts.OrderBy(item => item.Role, StringComparer.Ordinal))
        {
            CheckPath(root, artifact.Path, artifact.Role, findings);
        }

        if (evidence.Upstream.Revision.Length != 40 || evidence.Upstream.Revision.Any(character => !Uri.IsHexDigit(character)))
        {
            findings.Add(new("DOTP3001", "error", evidence.Id, "Flutter revision must be an exact 40-character Git revision."));
        }
        if (evidence.AnalysisMode != "syntax-only")
        {
            findings.Add(new("DOTP3002", "error", evidence.Id, "The external framework slice must explicitly use syntax-only analysis."));
        }
        if (evidence.Symbols.Length == 0 || evidence.Symbols.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count() != evidence.Symbols.Length)
        {
            findings.Add(new("DOTP3003", "error", evidence.Id, "Selected symbols must be non-empty and unique."));
        }
        foreach (var symbol in evidence.Symbols)
        {
            if (!Dispositions.Contains(symbol.Disposition, StringComparer.Ordinal))
            {
                findings.Add(new("DOTP3004", "error", symbol.Name, $"Unsupported slice disposition: {symbol.Disposition}"));
            }
            foreach (var dependency in symbol.Dependencies)
            {
                if (!evidence.Symbols.Any(item => item.Name == dependency))
                {
                    findings.Add(new("DOTP3005", "error", symbol.Name, $"Selected dependency is missing from the closure: {dependency}"));
                }
            }
        }
        foreach (var disposition in Dispositions)
        {
            if (!evidence.Symbols.Any(item => item.Disposition == disposition))
            {
                findings.Add(new("DOTP3006", "error", evidence.Id, $"The pilot does not exercise disposition '{disposition}'."));
            }
        }
        if (evidence.ExcludedDependencies.Length == 0 || evidence.ExcludedDependencies.Any(item => string.IsNullOrWhiteSpace(item.Reason)))
        {
            findings.Add(new("DOTP3007", "error", evidence.Id, "Excluded closure dependencies require an explicit reason."));
        }
        if (!evidence.Behavior.Implementations.SequenceEqual(
                ["generated-base", "manual-effective", "adopted-product"],
                StringComparer.Ordinal))
        {
            findings.Add(new("DOTP3008", "error", evidence.Id, "Behavior evidence must compare generated, manual effective, and adopted implementations."));
        }

        return new(SchemaVersion, findings.Count == 0, evidence.Id, evidence.Symbols.Length, findings.ToArray());
    }

    private static void CheckPath(
        string root,
        string relativePath,
        string subject,
        List<AuditFinding> findings)
    {
        var path = Path.GetFullPath(relativePath, root);
        var checkout = Directory.GetParent(root)?.FullName ?? root;
        if (!path.StartsWith(checkout + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
        {
            findings.Add(new("DOTP3009", "error", subject, $"Evidence path is missing or outside the checkout: {relativePath}"));
            return;
        }
    }
}

public sealed record FlutterFrameworkSliceEvidence(
    string SchemaVersion,
    string Id,
    FlutterFrameworkSliceUpstream Upstream,
    string Library,
    string AnalysisMode,
    string GeneratedBaseSha256,
    FlutterFrameworkSliceSymbol[] Symbols,
    FlutterFrameworkSliceExclusion[] ExcludedDependencies,
    FlutterFrameworkSliceArtifact[] Artifacts,
    FlutterFrameworkSliceBehavior Behavior);

public sealed record FlutterFrameworkSliceUpstream(
    string Repository,
    string Revision,
    string License,
    string LicensePath,
    string LicenseSha256,
    string SourcePath,
    string SourceSha256);

public sealed record FlutterFrameworkSliceSymbol(string Name, string Disposition, string[] Dependencies);
public sealed record FlutterFrameworkSliceExclusion(string Name, string Reason);
public sealed record FlutterFrameworkSliceArtifact(string Role, string Path, string Sha256);
public sealed record FlutterFrameworkSliceBehavior(string Fixture, string[] Implementations);
public sealed record FlutterFrameworkSliceAuditReport(
    string SchemaVersion,
    bool Success,
    string SliceId,
    int SelectedSymbolCount,
    AuditFinding[] Findings);
