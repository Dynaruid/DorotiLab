using Doroti.Tooling;

namespace Doroti.DartToCSharp;

public sealed record RebaseEntry(
    string Library,
    string Symbol,
    string? Member,
    string Status,
    string? PreviousFingerprint,
    string? CurrentFingerprint,
    string? PreviousGeneratedBaseSha256,
    string? CurrentGeneratedBaseSha256,
    string? PreviousManualSha256,
    string? CurrentManualSha256,
    string Detail);

public sealed record RebaseReportDocument(
    string SchemaVersion,
    string PreviousWorkspaceId,
    PortSource PreviousSource,
    PortSource CurrentSource,
    CompilerIdentity PreviousCompilerIdentity,
    CompilerIdentity CurrentCompilerIdentity,
    string PreviousUpstreamGraphSha256,
    string CurrentUpstreamGraphSha256,
    string PreviousManualInputsSha256,
    string CurrentManualInputsSha256,
    PortManualInput[] PreviousManualInputs,
    PortManualInput[] CurrentManualInputs,
    RebaseEntry[] Entries,
    string[] RequiredFixtures,
    bool HasBlockingChanges);

public sealed record RebaseBundle(string Path, RebaseReportDocument Report);

/// <summary>Compares a published base with a freshly generated upstream base without composing or overwriting user source.</summary>
public sealed class PortRebaser
{
    public RebaseBundle Create(
        string previousWorkspacePath,
        string manifestPath,
        string sourceRevision,
        string outputDirectory,
        string? cacheDirectory = null)
    {
        if (string.IsNullOrWhiteSpace(sourceRevision))
        {
            throw Error("DORPORT012", "Rebase requires a non-empty source revision.");
        }
        var previousRoot = Path.GetFullPath(previousWorkspacePath);
        var previousState = ArtifactFiles.ReadJson<PortStateDocument>(Path.Combine(previousRoot, "port-state.json"));
        var previousOwnership = ArtifactFiles.ReadJson<PortWorkspaceDocument>(Path.Combine(previousRoot, "port-workspace.json"));
        ValidatePrevious(previousRoot, previousState, previousOwnership);

        var portPath = Path.GetFullPath(manifestPath);
        var loader = new PortManifestLoader();
        var manifest = loader.Load(portPath);
        var selectionPath = PortManifestLoader.ResolveCompilerSelection(portPath, manifest);
        var replacements = loader.LoadReplacements(portPath, manifest);
        var currentManualInputs = InventoryCurrentManualInputs(portPath, manifest, replacements);
        RebaseReportDocument? report = null;
        ReviewBundlePublisher.Publish(outputDirectory, staging =>
        {
            var currentBase = Path.Combine(staging, "current-generated-base");
            var currentReport = new DartCompiler().Compile(selectionPath, currentBase, cacheDirectory);
            var previousIr = ArtifactFiles.ReadJson<MigrationIr>(Path.Combine(previousRoot, "generated-base", "migration-ir.json"));
            var currentIr = ArtifactFiles.ReadJson<MigrationIr>(Path.Combine(currentBase, "migration-ir.json"));
            ValidateSelection(manifest, currentIr);
            var previousTargets = DescribeTargets(previousIr);
            var currentTargets = DescribeTargets(currentIr);
            var replacementByTarget = replacements.ToDictionary(
                item => PortManifestLoader.TargetKey(item.Library, item.Symbol, item.Member),
                StringComparer.Ordinal);
            var previousManualByPath = previousState.ManualInputs.ToDictionary(item => item.Path, StringComparer.Ordinal);
            var currentManualByPath = currentManualInputs.ToDictionary(item => item.Path, StringComparer.Ordinal);
            var keys = previousTargets.Keys.Union(currentTargets.Keys, StringComparer.Ordinal)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            var entries = new List<RebaseEntry>();
            foreach (var key in keys)
            {
                previousTargets.TryGetValue(key, out var previous);
                currentTargets.TryGetValue(key, out var current);
                replacementByTarget.TryGetValue(key, out var replacement);
                var previousManual = replacement is not null && previousManualByPath.TryGetValue(replacement.Source, out var oldInput)
                    ? oldInput.Sha256
                    : null;
                var currentManual = replacement is not null && currentManualByPath.TryGetValue(replacement.Source, out var newInput)
                    ? newInput.Sha256
                    : null;
                var (status, detail) = Classify(previous, current, replacement is not null, previousManual, currentManual);
                var identity = previous ?? current!;
                entries.Add(new(
                    identity.Library,
                    identity.Symbol,
                    identity.Member,
                    status,
                    previous?.Fingerprint,
                    current?.Fingerprint,
                    previous?.GeneratedBaseSha256,
                    current?.GeneratedBaseSha256,
                    previousManual,
                    currentManual,
                    detail));
            }

            CopyReviewInputs(previousRoot, portPath, staging, previousState.ManualInputs, currentManualInputs);
            var currentSource = manifest.Source with { Revision = sourceRevision };
            report = new(
                PortSchemas.Rebase,
                previousState.WorkspaceId,
                previousState.Source,
                currentSource,
                previousState.CompilerIdentity,
                currentReport.Identity,
                previousState.UpstreamGraphSha256,
                PortStateArtifacts.HashJson(currentIr.PackageGraph),
                previousState.ManualInputsSha256,
                PortStateArtifacts.HashInventory(currentManualInputs.Select(item => (item.Path, item.Sha256))),
                previousState.ManualInputs,
                currentManualInputs,
                entries.ToArray(),
                manifest.RequiredFixtures.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                entries.Any(item => item.Status is PortSchemas.Conflict or PortSchemas.UpstreamSymbolRemoved));
            ArtifactFiles.WriteJson(Path.Combine(staging, "rebase-report.json"), report);
            ArtifactFiles.WriteUtf8(Path.Combine(staging, "review.md"), CreateReview(report, currentReport.Diagnostics));
        });
        return new(Path.GetFullPath(outputDirectory), report!);
    }

    public static string FindPreviousWorkspace(string workspaceRoot, string manifestPath)
    {
        var manifest = new PortManifestLoader().Load(manifestPath);
        var expectedTargets = manifest.Inputs
            .SelectMany(input => input.Symbols.Select(symbol => PortManifestLoader.TargetKey(input.Library, symbol, null)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        var candidates = new List<(string Path, PortStateDocument State)>();
        if (Directory.Exists(workspaceRoot))
        {
            foreach (var statePath in Directory.EnumerateFiles(workspaceRoot, "port-state.json", SearchOption.AllDirectories))
            {
                try
                {
                    var root = Path.GetDirectoryName(statePath)!;
                    var state = ArtifactFiles.ReadJson<PortStateDocument>(statePath);
                    var ownership = ArtifactFiles.ReadJson<PortWorkspaceDocument>(Path.Combine(root, "port-workspace.json"));
                    var actualTargets = ownership.SymbolOwnership
                        .Where(item => item.Member is null)
                        .Select(item => PortManifestLoader.TargetKey(item.Library, item.Symbol, null))
                        .OrderBy(value => value, StringComparer.Ordinal)
                        .ToArray();
                    if (state.SchemaVersion == PortSchemas.State &&
                        state.Mode == manifest.Mode &&
                        actualTargets.SequenceEqual(expectedTargets, StringComparer.Ordinal))
                    {
                        candidates.Add((root, state));
                    }
                }
                catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException)
                {
                    // Ignore unrelated or incomplete workspace directories while searching.
                }
            }
        }
        return candidates
            .OrderByDescending(item => Directory.GetLastWriteTimeUtc(item.Path))
            .Select(item => item.Path)
            .FirstOrDefault()
            ?? throw Error("DORPORT012", "No previous compiled workspace matches the port. Run compile --port first.");
    }

    private static Dictionary<string, TargetDescription> DescribeTargets(MigrationIr ir)
    {
        var outputByInput = ir.Outputs.ToDictionary(item => item.Input, StringComparer.Ordinal);
        var targets = new Dictionary<string, TargetDescription>(StringComparer.Ordinal);
        foreach (var input in ir.Inputs)
        {
            var output = outputByInput[input.Path];
            foreach (var declaration in input.Declarations)
            {
                var symbolKey = PortManifestLoader.TargetKey(input.Library, declaration.Name, null);
                targets.Add(symbolKey, new(
                    input.Library,
                    declaration.Name,
                    null,
                    PortStateArtifacts.HashJson(new { declaration.Kind, declaration.Name, declaration.Element }),
                    output.Sha256));
                foreach (var member in declaration.Members)
                {
                    var memberKey = PortManifestLoader.TargetKey(input.Library, declaration.Name, member.Name);
                    targets.Add(memberKey, new(
                        input.Library,
                        declaration.Name,
                        member.Name,
                        PortStateArtifacts.HashJson(new
                        {
                            member.Kind,
                            member.Name,
                            member.Element,
                            Statements = member.Statements.Select(item => new { item.Kind, item.Source }).ToArray(),
                        }),
                        output.Sha256));
                }
            }
        }
        return targets;
    }

    private static (string Status, string Detail) Classify(
        TargetDescription? previous,
        TargetDescription? current,
        bool hasReplacement,
        string? previousManual,
        string? currentManual)
    {
        if (previous is null)
        {
            return (PortSchemas.FixtureRequired, "New upstream target requires behavior fixture review.");
        }
        if (current is null)
        {
            return (PortSchemas.UpstreamSymbolRemoved, "The previously selected upstream target was removed.");
        }
        var semanticChanged = previous.Fingerprint != current.Fingerprint;
        var baseChanged = previous.GeneratedBaseSha256 != current.GeneratedBaseSha256;
        var manualChanged = previousManual != currentManual;
        if (hasReplacement && semanticChanged)
        {
            return (PortSchemas.Conflict, "Upstream meaning changed beneath a manual replacement.");
        }
        if (hasReplacement && (baseChanged || manualChanged))
        {
            return (PortSchemas.ManualReview, "Manual replacement or its generated-base context changed and must be re-reviewed.");
        }
        if (semanticChanged)
        {
            return (PortSchemas.FixtureRequired, "Compiler-owned upstream meaning changed; rerun the required fixture.");
        }
        return (PortSchemas.Clean, "No target-level upstream or manual drift was detected.");
    }

    private static PortManualInput[] InventoryCurrentManualInputs(
        string portPath,
        PortManifest manifest,
        PortReplacement[] replacements)
    {
        var portRoot = Path.GetDirectoryName(portPath)!;
        var inputs = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ArtifactFiles.NormalizePath(Path.GetFileName(portPath))] = "port-manifest",
        };
        foreach (var mapping in manifest.Customizations.MappingFiles ?? [])
        {
            inputs[ArtifactFiles.NormalizePath(mapping)] = "mapping";
        }
        if (!string.IsNullOrWhiteSpace(manifest.Customizations.ReplacementManifest))
        {
            inputs[ArtifactFiles.NormalizePath(manifest.Customizations.ReplacementManifest)] = "replacement-manifest";
        }
        foreach (var replacement in replacements)
        {
            inputs[ArtifactFiles.NormalizePath(replacement.Source)] = PortSchemas.ManualReplacement;
        }
        AddRootInputs(inputs, portRoot, manifest.Customizations.ExtensionRoots ?? [], PortSchemas.PartialExtension);
        AddRootInputs(inputs, portRoot, manifest.Customizations.PlatformPortRoots ?? [], PortSchemas.PlatformPort);
        return inputs.OrderBy(item => item.Key, StringComparer.Ordinal)
            .Select(item =>
            {
                var source = item.Key == Path.GetFileName(portPath)
                    ? portPath
                    : PortManifestLoader.ResolveUserPath(portRoot, item.Key, requireDirectory: false);
                var sha = ArtifactFiles.Sha256(source);
                return new PortManualInput(item.Key, $"current-manual/{item.Key}", sha, item.Value);
            })
            .ToArray();
    }

    private static void AddRootInputs(
        Dictionary<string, string> inputs,
        string portRoot,
        IEnumerable<string> roots,
        string origin)
    {
        foreach (var relativeRoot in roots)
        {
            var root = PortManifestLoader.ResolveUserPath(portRoot, relativeRoot, requireDirectory: true);
            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                         .Where(path => !Path.GetRelativePath(root, path)
                             .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                             .Any(part => part is "bin" or "obj")))
            {
                inputs[ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, path))] = origin;
            }
        }
    }

    private static void CopyReviewInputs(
        string previousRoot,
        string portPath,
        string staging,
        IEnumerable<PortManualInput> previousInputs,
        IEnumerable<PortManualInput> currentInputs)
    {
        foreach (var input in previousInputs)
        {
            Copy(Path.Combine(previousRoot, input.SnapshotPath), Path.Combine(staging, "previous-" + input.SnapshotPath));
        }
        var portRoot = Path.GetDirectoryName(portPath)!;
        foreach (var input in currentInputs)
        {
            var source = input.Path == Path.GetFileName(portPath)
                ? portPath
                : PortManifestLoader.ResolveUserPath(portRoot, input.Path, requireDirectory: false);
            Copy(source, Path.Combine(staging, input.SnapshotPath));
        }
    }

    private static void Copy(string source, string target)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(source, target, overwrite: false);
    }

    private static void ValidatePrevious(
        string root,
        PortStateDocument state,
        PortWorkspaceDocument ownership)
    {
        if (state.SchemaVersion != PortSchemas.State || ownership.SchemaVersion != PortSchemas.Workspace ||
            state.WorkspaceId != ownership.WorkspaceId || Path.GetFileName(root) != state.WorkspaceId)
        {
            throw Error("DORPORT012", "Previous workspace state and ownership identities do not match.");
        }
        foreach (var file in ownership.GeneratedFiles)
        {
            var path = Path.Combine(root, "generated-base", file.Path);
            if (!File.Exists(path) || ArtifactFiles.Sha256(path) != file.Sha256)
            {
                throw Error("DORPORT004", $"Previous generated base was edited: {file.Path}");
            }
        }
    }

    private static void ValidateSelection(PortManifest manifest, MigrationIr ir)
    {
        var expected = manifest.Inputs
            .SelectMany(input => input.Symbols.Select(symbol => PortManifestLoader.TargetKey(input.Library, symbol, null)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        var actual = ir.Inputs
            .SelectMany(input => input.SelectedSymbols.Select(symbol => PortManifestLoader.TargetKey(input.Library, symbol, null)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();
        if (!expected.SequenceEqual(actual, StringComparer.Ordinal))
        {
            throw Error("DORPORT008", "Rebase selection differs from the port manifest.");
        }
    }

    private static string CreateReview(RebaseReportDocument report, ConverterDiagnostic[] diagnostics)
    {
        var lines = new List<string>
        {
            "# Port rebase review",
            string.Empty,
            $"- Previous revision: `{report.PreviousSource.Revision}`",
            $"- Current revision: `{report.CurrentSource.Revision}`",
            $"- Blocking changes: `{report.HasBlockingChanges.ToString().ToLowerInvariant()}`",
            $"- Compiler diagnostics: `{diagnostics.Length}`",
            string.Empty,
            "| Target | Status | Detail |",
            "|---|---|---|",
        };
        lines.AddRange(report.Entries.Select(item =>
            $"| `{PortManifestLoader.TargetKey(item.Library, item.Symbol, item.Member)}` | `{item.Status}` | {item.Detail} |"));
        lines.Add(string.Empty);
        return string.Join('\n', lines);
    }

    private sealed record TargetDescription(
        string Library,
        string Symbol,
        string? Member,
        string Fingerprint,
        string GeneratedBaseSha256);

    private static PortContractException Error(string code, string message) => new(code, message);
}
