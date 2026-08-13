using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class FlutterFrameworkEvidenceAudit
{
    public const string EvidenceSchemaVersion = "doroti.flutter-framework-evidence/v2";
    public const string ReportSchemaVersion = "doroti.flutter-framework-evidence-audit/v2";

    private static readonly string[] CompletionDispositions =
    [
        "reviewed-generated-cs",
        "reviewed-source-port-cs",
        "runtime-binding",
    ];

    public static FlutterFrameworkEvidenceAuditReport Run(string repositoryRoot, string evidencePath, string expectedMilestone)
    {
        var root = Path.GetFullPath(repositoryRoot);
        var evidence = ArtifactFiles.ReadJson<FlutterFrameworkEvidence>(evidencePath);
        var findings = new List<AuditFinding>();
        if (evidence.SchemaVersion != EvidenceSchemaVersion || evidence.Milestone != expectedMilestone)
        {
            throw new InvalidDataException($"Unsupported {expectedMilestone} framework evidence: {evidence.SchemaVersion} / {evidence.Milestone}.");
        }

        var closurePath = RepositoryPaths.ResolveWithin(root, evidence.Closure);
        var closure = ArtifactFiles.ReadJson<ResolvedFrameworkClosure>(closurePath);
        CheckRevision(evidence, closure, findings);
        var inventory = CreateInventory(closure, findings);
        CheckInventory(evidence.ResolvedInventory, inventory, findings);

        var registeredProfiles = ReadRegisteredProfiles(root, findings);
        var symbols = CreateSymbolInventory(closure, findings);
        var claimedTargets = new List<FlutterFrameworkSymbolTarget>();
        CheckState(root, evidence.MechanicalGenerated, "mechanical-generated", symbols, registeredProfiles, findings, claimedTargets);
        CheckState(root, evidence.ReviewedGeneratedCs, "reviewed-generated-cs", symbols, registeredProfiles, findings, claimedTargets);
        CheckState(root, evidence.ReviewedSourcePortCs, "reviewed-source-port-cs", symbols, registeredProfiles, findings, claimedTargets);
        CheckState(root, evidence.RuntimeBound, "runtime-binding", symbols, registeredProfiles, findings, claimedTargets);

        foreach (var duplicate in claimedTargets.GroupBy(target => target.ElementId, StringComparer.Ordinal).Where(group => group.Count() != 1))
        {
            findings.Add(new("DOTG3008", "error", duplicate.Key, "A framework symbol has more than one implementation disposition/target."));
        }

        var completedDeclarations = evidence.ReviewedGeneratedCs.Declarations + evidence.ReviewedSourcePortCs.Declarations + evidence.RuntimeBound.Declarations;
        var completedMembers = evidence.ReviewedGeneratedCs.Members + evidence.ReviewedSourcePortCs.Members + evidence.RuntimeBound.Members;
        var expectedMissingMechanicalDeclarations = inventory.Declarations - evidence.MechanicalGenerated.Declarations;
        var expectedMissingMechanicalMembers = inventory.Members - evidence.MechanicalGenerated.Members;
        var expectedMissingCompletedDeclarations = inventory.Declarations - completedDeclarations;
        var expectedMissingCompletedMembers = inventory.Members - completedMembers;
        if (evidence.Blockers.MissingMechanicalDeclarations != expectedMissingMechanicalDeclarations ||
            evidence.Blockers.MissingMechanicalMembers != expectedMissingMechanicalMembers ||
            evidence.Blockers.MissingCompletedDeclarations != expectedMissingCompletedDeclarations ||
            evidence.Blockers.MissingCompletedMembers != expectedMissingCompletedMembers ||
            evidence.Blockers.ImplementationBlockers != expectedMissingCompletedDeclarations + expectedMissingCompletedMembers ||
            evidence.Blockers.UnsupportedBlockers != inventory.UnsupportedBlockers)
        {
            findings.Add(new("DOTG3009", "error", expectedMilestone, "Framework blocker counts do not equal the unresolved symbol cardinality."));
        }

        if (evidence.Compiled.MechanicalGenerated || evidence.Compiled.ReviewedGeneratedCs ||
            evidence.Compiled.ReviewedSourcePortCs || evidence.Compiled.RuntimeBound || evidence.Compiled.Evidence.Length != 0)
        {
            findings.Add(new("DOTG3010", "error", expectedMilestone, "Pre-G3 compile results must not be imported into the new completion graph."));
        }
        if (evidence.BehaviorVerified.VerifiedDeclarations != 0 || evidence.BehaviorVerified.VerifiedMembers != 0 ||
            evidence.BehaviorVerified.Evidence.Length != 0)
        {
            findings.Add(new("DOTG3011", "error", expectedMilestone, "Pre-G3 behavior results must not be imported into the new completion graph."));
        }
        if (evidence.LegacyClassification.IncludedInImplementation ||
            !evidence.LegacyClassification.ExcludedDispositions.Contains("manual-adaptation", StringComparer.Ordinal))
        {
            findings.Add(new("DOTG3012", "error", expectedMilestone, "Broad manual-adaptation classification must be excluded from implementation coverage."));
        }
        if (evidence.HistoricalEvidence.AcceptedAsCompletionInput)
        {
            findings.Add(new("DOTG3013", "error", expectedMilestone, "Historical evidence cannot be accepted by the Goal3 completion graph."));
        }
        if (evidence.CompilerGate.MilestoneProfile is not null || evidence.CompilerGate.MilestoneGeneratedDeclarations != 0 ||
            evidence.CompilerGate.MilestoneGeneratedMembers != 0)
        {
            findings.Add(new("DOTG3014", "error", expectedMilestone, "F1-F4 cannot claim generated coverage before a milestone framework compiler profile exists."));
        }
        if (evidence.Status != "implementation-blocked" || evidence.MilestoneComplete || completedDeclarations != 0 || completedMembers != 0)
        {
            findings.Add(new("DOTG3015", "error", expectedMilestone, "Truth-reset evidence must remain implementation-blocked until reviewed/runtime-bound symbols exist."));
        }

        return new(
            ReportSchemaVersion,
            findings.Count == 0,
            expectedMilestone,
            inventory,
            evidence.MechanicalGenerated.Declarations,
            evidence.MechanicalGenerated.Members,
            completedDeclarations,
            completedMembers,
            evidence.Blockers.MissingMechanicalDeclarations,
            evidence.Blockers.MissingMechanicalMembers,
            evidence.Blockers.ImplementationBlockers,
            false,
            findings.ToArray());
    }

    private static void CheckRevision(FlutterFrameworkEvidence evidence, ResolvedFrameworkClosure closure, List<AuditFinding> findings)
    {
        if (evidence.FlutterGitRevision != closure.FlutterGitRevision || closure.FlutterGitRevision.Length != 40)
        {
            findings.Add(new("DOTG3001", "error", evidence.Milestone, "Evidence and resolved inventory must use one immutable Flutter revision."));
        }
    }

    private static FlutterFrameworkInventory CreateInventory(ResolvedFrameworkClosure closure, List<AuditFinding> findings)
    {
        var declarations = closure.Libraries.Sum(library => library.Declarations.Length);
        var members = closure.Libraries.Sum(library => library.Declarations.Sum(declaration => declaration.Members.Length));
        if (closure.Libraries.Select(library => library.Path).Distinct(StringComparer.Ordinal).Count() != closure.Libraries.Length)
        {
            findings.Add(new("DOTG3002", "error", closure.Milestone, "Resolved inventory contains duplicate library paths."));
        }
        return new(
            closure.Libraries.Length,
            declarations,
            members,
            closure.Coverage.AnalyzerErrors,
            closure.Coverage.UnclassifiedDeclarations,
            closure.Coverage.UnclassifiedMembers,
            closure.Coverage.UnsupportedBlockers);
    }

    private static void CheckInventory(FlutterFrameworkInventory expected, FlutterFrameworkInventory actual, List<AuditFinding> findings)
    {
        if (expected != actual)
        {
            findings.Add(new("DOTG3003", "error", "resolvedInventory", "Evidence inventory does not match the resolved closure symbol cardinality."));
        }
    }

    private static Dictionary<string, FrameworkClosureSymbol> CreateSymbolInventory(ResolvedFrameworkClosure closure, List<AuditFinding> findings)
    {
        var symbols = new Dictionary<string, FrameworkClosureSymbol>(StringComparer.Ordinal);
        foreach (var library in closure.Libraries)
        {
            var declarationNameCounts = library.Declarations
                .GroupBy(declaration => declaration.Name, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
            for (var declarationIndex = 0; declarationIndex < library.Declarations.Length; declarationIndex++)
            {
                var declaration = library.Declarations[declarationIndex];
                var libraryUri = $"package:flutter/{library.Path}";
                var declarationId = declarationNameCounts[declaration.Name] == 1
                    ? $"{libraryUri}#{declaration.Name}"
                    : $"{libraryUri}#{declaration.Name}@declaration[{declarationIndex}]";
                AddSymbol(symbols, new(declarationId, "declaration", library.Path, declaration.Name), findings);
                for (var index = 0; index < declaration.Members.Length; index++)
                {
                    var member = declaration.Members[index];
                    AddSymbol(symbols, new($"{declarationId}.{member}@{index}", "member", library.Path, member), findings);
                }
            }
        }
        return symbols;
    }

    private static void AddSymbol(Dictionary<string, FrameworkClosureSymbol> symbols, FrameworkClosureSymbol symbol, List<AuditFinding> findings)
    {
        if (!symbols.TryAdd(symbol.ElementId, symbol))
        {
            findings.Add(new("DOTG3004", "error", symbol.ElementId, "Resolved inventory cannot assign a unique canonical framework element ID."));
        }
    }

    private static HashSet<string> ReadRegisteredProfiles(string root, List<AuditFinding> findings)
    {
        var path = Path.Combine(root, "migration", "compiler-support.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty("frameworkCoverage", out var coverage) ||
            !coverage.TryGetProperty("registeredProfiles", out var profiles))
        {
            findings.Add(new("DOTG3005", "error", "compiler-support", "Compiler support does not declare registered framework profiles."));
            return [];
        }
        return profiles.EnumerateArray()
            .Select(profile => profile.GetProperty("profile").GetString())
            .Where(profile => !string.IsNullOrWhiteSpace(profile))
            .Select(profile => profile!)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static void CheckState(
        string root,
        FlutterFrameworkImplementationState state,
        string expectedDisposition,
        IReadOnlyDictionary<string, FrameworkClosureSymbol> symbols,
        IReadOnlySet<string> registeredProfiles,
        List<AuditFinding> findings,
        List<FlutterFrameworkSymbolTarget> claimedTargets)
    {
        var declarationTargets = state.SymbolTargets.Count(target => target.Kind == "declaration");
        var memberTargets = state.SymbolTargets.Count(target => target.Kind == "member");
        if (state.Disposition != expectedDisposition || state.Declarations != declarationTargets || state.Members != memberTargets)
        {
            findings.Add(new("DOTG3006", "error", expectedDisposition, "Implementation totals must equal one symbol-to-target row per claimed declaration/member."));
        }
        foreach (var target in state.SymbolTargets)
        {
            claimedTargets.Add(target);
            if (!symbols.TryGetValue(target.ElementId, out var symbol) || symbol.Kind != target.Kind || symbol.Source != target.Source)
            {
                findings.Add(new("DOTG3007", "error", target.ElementId, "Implementation target does not map one-to-one to a resolved inventory symbol."));
                continue;
            }
            if (!registeredProfiles.Contains(target.CompilerProfile))
            {
                findings.Add(new("DOTG3016", "error", target.ElementId, $"Implementation target uses an unregistered compiler profile: {target.CompilerProfile}."));
            }
            var path = RepositoryPaths.ResolveWithin(root, target.Target);
            if (!File.Exists(path) || ArtifactFiles.Sha256(path) != target.TargetSha256)
            {
                findings.Add(new("DOTG3017", "error", target.ElementId, $"Implementation target is absent or hash-mismatched: {target.Target}."));
            }
            if (expectedDisposition == "mechanical-generated" && !target.Target.EndsWith(".g.cs", StringComparison.Ordinal))
            {
                findings.Add(new("DOTG3018", "error", target.ElementId, "Mechanical candidate target must be a .g.cs file."));
            }
            if (CompletionDispositions.Contains(expectedDisposition, StringComparer.Ordinal) && target.Target.EndsWith(".g.cs", StringComparison.Ordinal))
            {
                findings.Add(new("DOTG3019", "error", target.ElementId, "Completed implementation dispositions cannot target an unreviewed .g.cs candidate."));
            }
        }
    }
}

public sealed record FlutterFrameworkEvidence(
    string SchemaVersion,
    string Milestone,
    string Status,
    string FlutterGitRevision,
    string Closure,
    FlutterFrameworkInventory ResolvedInventory,
    FlutterFrameworkImplementationState MechanicalGenerated,
    FlutterFrameworkImplementationState ReviewedGeneratedCs,
    FlutterFrameworkImplementationState ReviewedSourcePortCs,
    FlutterFrameworkImplementationState RuntimeBound,
    FlutterFrameworkCompileState Compiled,
    FlutterFrameworkBehaviorState BehaviorVerified,
    FlutterFrameworkLegacyClassification LegacyClassification,
    FlutterFrameworkHistoricalEvidence HistoricalEvidence,
    FlutterFrameworkCompilerGate CompilerGate,
    FlutterFrameworkBlockers Blockers,
    bool MilestoneComplete);

public sealed record FlutterFrameworkInventory(
    int Libraries,
    int Declarations,
    int Members,
    int AnalyzerErrors,
    int UnclassifiedDeclarations,
    int UnclassifiedMembers,
    int UnsupportedBlockers);

public sealed record FlutterFrameworkImplementationState(
    string Disposition,
    int Declarations,
    int Members,
    FlutterFrameworkSymbolTarget[] SymbolTargets);

public sealed record FlutterFrameworkSymbolTarget(
    string ElementId,
    string Kind,
    string Source,
    string Target,
    string TargetSha256,
    string CompilerProfile);

public sealed record FlutterFrameworkCompileState(
    bool MechanicalGenerated,
    bool ReviewedGeneratedCs,
    bool ReviewedSourcePortCs,
    bool RuntimeBound,
    string[] Evidence);

public sealed record FlutterFrameworkBehaviorState(int VerifiedDeclarations, int VerifiedMembers, string[] Evidence);
public sealed record FlutterFrameworkLegacyClassification(bool IncludedInImplementation, string[] ExcludedDispositions, string MigrationRule);
public sealed record FlutterFrameworkHistoricalEvidence(bool AcceptedAsCompletionInput, string[] RemovedInputs);
public sealed record FlutterFrameworkCompilerGate(string[] RegisteredProfiles, string? MilestoneProfile, int MilestoneGeneratedDeclarations, int MilestoneGeneratedMembers);
public sealed record FlutterFrameworkBlockers(
    int MissingMechanicalDeclarations,
    int MissingMechanicalMembers,
    int MissingCompletedDeclarations,
    int MissingCompletedMembers,
    int ImplementationBlockers,
    int UnsupportedBlockers);

public sealed record ResolvedFrameworkClosure(
    string SchemaVersion,
    string Milestone,
    string FlutterGitRevision,
    string AnalysisMode,
    string[] Roots,
    string[] ExternalExports,
    string SelectedContentSha256,
    ResolvedFrameworkCoverage Coverage,
    ResolvedFrameworkLibrary[] Libraries);

public sealed record ResolvedFrameworkCoverage(
    int Libraries,
    int Declarations,
    int Members,
    int AnalyzerErrors,
    int UnclassifiedDeclarations,
    int UnclassifiedMembers,
    int UnsupportedBlockers,
    Dictionary<string, ResolvedFrameworkDispositionCount> Dispositions);

public sealed record ResolvedFrameworkDispositionCount(int Declarations, int Members);
public sealed record ResolvedFrameworkLibrary(
    string Path,
    string Sha256,
    string LibraryUri,
    string[] Dependencies,
    string Disposition,
    string Owner,
    int AnalyzerErrors,
    ResolvedFrameworkDeclaration[] Declarations);
public sealed record ResolvedFrameworkDeclaration(string Name, string Kind, string? CanonicalElementId, string[] Members);
public sealed record FrameworkClosureSymbol(string ElementId, string Kind, string Source, string Name);

public sealed record FlutterFrameworkEvidenceAuditReport(
    string SchemaVersion,
    bool Success,
    string Milestone,
    FlutterFrameworkInventory ResolvedInventory,
    int MechanicalGeneratedDeclarations,
    int MechanicalGeneratedMembers,
    int CompletedDeclarations,
    int CompletedMembers,
    int MissingMechanicalDeclarations,
    int MissingMechanicalMembers,
    int ImplementationBlockers,
    bool MilestoneComplete,
    AuditFinding[] Findings);
