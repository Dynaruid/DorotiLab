using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class FlutterFrameworkEvidenceReset
{
    private const string FlutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a";
    private const string F0ElementId = "package:flutter/src/foundation/object.dart#objectRuntimeType";
    private const string F0Candidate = "migration/generated-candidates/flutter-framework/56b8e1a851a594b1a154f8ea93270807dab22b9a/foundation/object.g.cs";

    public static FlutterFrameworkEvidenceResetReport Regenerate(string repositoryRoot, string? outputRoot = null)
    {
        var root = Path.GetFullPath(repositoryRoot);
        var destinationRoot = outputRoot is null ? root : Path.GetFullPath(outputRoot);
        var candidatePath = RepositoryPaths.ResolveWithin(root, F0Candidate);
        if (!File.Exists(candidatePath))
        {
            throw new FileNotFoundException("The F0 mechanical candidate must be regenerated before evidence reset.", candidatePath);
        }

        var artifacts = new List<FlutterFrameworkEvidenceResetArtifact>();
        foreach (var milestone in new[] { "F1", "F2", "F3", "F4" })
        {
            var lower = milestone.ToLowerInvariant();
            var closureRelative = $"migration/flutter-framework/{lower}-closure.json";
            var closure = ArtifactFiles.ReadJson<ResolvedFrameworkClosure>(RepositoryPaths.ResolveWithin(root, closureRelative));
            if (closure.Milestone != milestone || closure.FlutterGitRevision != FlutterRevision)
            {
                throw new InvalidDataException($"{milestone} closure does not match the pinned Goal3 source revision.");
            }

            var inventory = new FlutterFrameworkInventory(
                closure.Libraries.Length,
                closure.Libraries.Sum(library => library.Declarations.Length),
                closure.Libraries.Sum(library => library.Declarations.Sum(declaration => declaration.Members.Length)),
                closure.Coverage.AnalyzerErrors,
                closure.Coverage.UnclassifiedDeclarations,
                closure.Coverage.UnclassifiedMembers,
                closure.Coverage.UnsupportedBlockers);
            var f0Matches = closure.Libraries
                .Where(library => library.Path == "src/foundation/object.dart")
                .SelectMany(library => library.Declarations)
                .Count(declaration => declaration.Name == "objectRuntimeType");
            if (f0Matches != 1)
            {
                throw new InvalidDataException($"{milestone} closure must contain exactly one F0 objectRuntimeType declaration; found {f0Matches}.");
            }

            var candidateTarget = new FlutterFrameworkSymbolTarget(
                F0ElementId,
                "declaration",
                "src/foundation/object.dart",
                F0Candidate,
                ArtifactFiles.Sha256(candidatePath),
                "flutter-framework-f0");
            var evidence = new FlutterFrameworkEvidence(
                FlutterFrameworkEvidenceAudit.EvidenceSchemaVersion,
                milestone,
                "implementation-blocked",
                FlutterRevision,
                closureRelative,
                inventory,
                new("mechanical-generated", 1, 0, [candidateTarget]),
                EmptyState("reviewed-generated-cs"),
                EmptyState("reviewed-source-port-cs"),
                EmptyState("runtime-binding"),
                new(false, false, false, false, []),
                new(0, 0, []),
                new(
                    false,
                    ["generated", "manual-adaptation", "runtime-binding"],
                    "The v1 closure disposition and owner fields are resolved-inventory annotations only. They never contribute to Goal3 implementation coverage."),
                new(
                    false,
                    milestone == "F4"
                        ? ["doroti.flutter-f4-evidence/v1", "doroti.flutter-f4-support-matrix/v1", "doroti.product-ui-support/v1", "doroti.h6-product-ui-evidence/v1"]
                        : [$"doroti.flutter-{lower}-evidence/v1"]),
                new(["flutter-framework-f0"], null, 0, 0),
                new(
                    inventory.Declarations - 1,
                    inventory.Members,
                    inventory.Declarations,
                    inventory.Members,
                    inventory.Declarations + inventory.Members,
                    inventory.UnsupportedBlockers),
                false);
            var relative = $"migration/flutter-framework/{lower}-evidence.json";
            var output = RepositoryPaths.ResolveWithin(destinationRoot, relative);
            ArtifactFiles.WriteJson(output, evidence);
            artifacts.Add(new(milestone, relative, inventory.Declarations + inventory.Members - 1, inventory.Declarations + inventory.Members));
        }
        return new("doroti.flutter-framework-evidence-reset/v1", artifacts.ToArray());
    }

    private static FlutterFrameworkImplementationState EmptyState(string disposition) => new(disposition, 0, 0, []);
}

public sealed record FlutterFrameworkEvidenceResetReport(string SchemaVersion, FlutterFrameworkEvidenceResetArtifact[] Artifacts);
public sealed record FlutterFrameworkEvidenceResetArtifact(string Milestone, string Path, int MissingMechanicalSymbols, int ImplementationBlockers);
