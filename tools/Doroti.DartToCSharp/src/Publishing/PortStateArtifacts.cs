using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

public sealed record PortManualInput(
    string Path,
    string SnapshotPath,
    string Sha256,
    string Origin);

public sealed record PortArtifactState(
    string Path,
    string Sha256,
    string Origin,
    string? Source);

public sealed record PortStateDocument(
    string SchemaVersion,
    string WorkspaceId,
    string Mode,
    PortSource Source,
    CompilerIdentity CompilerIdentity,
    PackageGraph UpstreamGraph,
    string UpstreamGraphSha256,
    string PortManifestSha256,
    string GeneratedBaseSha256,
    string ManualInputsSha256,
    string EffectiveSha256,
    PortManualInput[] ManualInputs,
    PortArtifactState[] EffectiveArtifacts,
    string[] RequiredFixtures);

public sealed record PortProvenanceDocument(
    string SchemaVersion,
    string WorkspaceId,
    PortSource Upstream,
    CompilerIdentity CompilerIdentity,
    PortProvenanceEntry[] Entries);

public sealed record PortProvenanceEntry(
    string Path,
    string Origin,
    string Sha256,
    string? Source,
    string? Library,
    string? Symbol,
    string? Member,
    string? GeneratedBaseSha256);

public sealed record PortSourceMapDocument(
    string SchemaVersion,
    string WorkspaceId,
    PortOriginMapEntry[] Mappings);

public sealed record PortOriginMapEntry(
    string Path,
    string Origin,
    string Sha256,
    string? Source,
    string? Library,
    string? Symbol,
    string? Member,
    int? SourceOffset,
    int? SourceLength);

internal sealed record PortStateBuildResult(
    PortStateDocument State,
    PortProvenanceDocument Provenance,
    PortSourceMapDocument SourceMap,
    PortGeneratedFile[] ManualSnapshotFiles);

internal static class PortStateArtifacts
{
    public static PortStateBuildResult Create(
        string staging,
        string workspaceId,
        string portPath,
        PortManifest manifest,
        ConverterReport report,
        MigrationIr ir,
        PortReplacement[] replacements,
        PortWorkspaceDocument ownership)
    {
        var manualInputs = SnapshotManualInputs(staging, portPath, manifest, replacements);
        var effectiveArtifacts = ownership.EffectiveFiles
            .Select(item => new PortArtifactState(item.Path, item.Sha256, item.Origin, item.Source))
            .ToArray();
        var graphSha = HashJson(ir.PackageGraph);
        var state = new PortStateDocument(
            PortSchemas.State,
            workspaceId,
            manifest.Mode,
            manifest.Source,
            report.Identity,
            ir.PackageGraph,
            graphSha,
            ArtifactFiles.Sha256(portPath),
            HashInventory(ownership.GeneratedFiles.Select(item => (item.Path, item.Sha256))),
            HashInventory(manualInputs.Select(item => (item.Path, item.Sha256))),
            HashInventory(effectiveArtifacts.Select(item => (item.Path, item.Sha256))),
            manualInputs,
            effectiveArtifacts,
            manifest.RequiredFixtures.OrderBy(value => value, StringComparer.Ordinal).ToArray());

        var provenance = new PortProvenanceDocument(
            PortSchemas.Provenance,
            workspaceId,
            manifest.Source,
            report.Identity,
            CreateProvenanceEntries(ownership, ir));
        var sourceMap = new PortSourceMapDocument(
            PortSchemas.SourceMap,
            workspaceId,
            CreateOriginMappings(staging, ir, ownership));
        var snapshotFiles = EnumerateFiles(Path.Combine(staging, "manual-snapshot"))
            .Select(item => new PortGeneratedFile(item.Key, item.Value))
            .ToArray();
        return new(state, provenance, sourceMap, snapshotFiles);
    }

    public static string HashInventory(IEnumerable<(string Path, string Sha256)> items)
    {
        var identity = string.Join('\n', items
            .OrderBy(item => item.Path, StringComparer.Ordinal)
            .Select(item => $"{ArtifactFiles.NormalizePath(item.Path)}:{item.Sha256}")) + "\n";
        return HashText(identity);
    }

    public static string HashJson<T>(T value)
    {
        var json = JsonSerializer.Serialize(value, ArtifactFiles.JsonOptions)
            .Replace("\r\n", "\n", StringComparison.Ordinal) + "\n";
        return HashText(json);
    }

    private static PortManualInput[] SnapshotManualInputs(
        string staging,
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
        AddRoots(inputs, portRoot, manifest.Customizations.ExtensionRoots ?? [], PortSchemas.PartialExtension);
        AddRoots(inputs, portRoot, manifest.Customizations.PlatformPortRoots ?? [], PortSchemas.PlatformPort);

        var result = new List<PortManualInput>();
        foreach (var item in inputs.OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            var source = item.Key == Path.GetFileName(portPath)
                ? portPath
                : PortManifestLoader.ResolveUserPath(portRoot, item.Key, requireDirectory: false);
            var sha = ArtifactFiles.Sha256(source);
            var snapshot = ArtifactFiles.NormalizePath(Path.Combine(item.Value, sha, item.Key));
            var target = Path.Combine(staging, "manual-snapshot", snapshot);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            File.Copy(source, target, overwrite: false);
            result.Add(new(item.Key, $"manual-snapshot/{snapshot}", sha, item.Value));
        }
        return result.ToArray();
    }

    private static void AddRoots(
        Dictionary<string, string> inputs,
        string portRoot,
        IEnumerable<string> relativeRoots,
        string origin)
    {
        foreach (var relativeRoot in relativeRoots.OrderBy(value => value, StringComparer.Ordinal))
        {
            var root = PortManifestLoader.ResolveUserPath(portRoot, relativeRoot, requireDirectory: true);
            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                         .Where(path => !IsBuildOutput(Path.GetRelativePath(root, path))))
            {
                inputs[ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, path))] = origin;
            }
        }
    }

    private static PortProvenanceEntry[] CreateProvenanceEntries(PortWorkspaceDocument ownership, MigrationIr ir)
    {
        var ownershipBySource = ownership.SymbolOwnership
            .Where(item => item.Source is not null)
            .GroupBy(item => item.Source!, StringComparer.Ordinal)
            .ToDictionary(item => item.Key, item => item.ToArray(), StringComparer.Ordinal);
        var entries = new List<PortProvenanceEntry>();
        var generatedTargetsByPath = ir.Inputs
            .Select(input => (Input: input, Output: ir.Outputs.Single(output => output.Input == input.Path)))
            .ToDictionary(
                item => item.Output.Output,
                item => item.Input.Declarations.Select(declaration => new
                {
                    item.Input.Path,
                    item.Input.Library,
                    declaration.Name,
                    item.Output.Sha256,
                }).ToArray(),
                StringComparer.Ordinal);
        foreach (var file in ownership.EffectiveFiles)
        {
            if (file.Source is not null && ownershipBySource.TryGetValue(file.Source, out var targets))
            {
                entries.AddRange(targets.Select(target => new PortProvenanceEntry(
                    file.Path,
                    file.Origin,
                    file.Sha256,
                    file.Source,
                    target.Library,
                    target.Symbol,
                    target.Member,
                    target.GeneratedBaseSha256)));
            }
            else if (file.Origin == PortSchemas.Generated && generatedTargetsByPath.TryGetValue(file.Path, out var generatedTargets))
            {
                entries.AddRange(generatedTargets.Select(target => new PortProvenanceEntry(
                    file.Path,
                    file.Origin,
                    file.Sha256,
                    target.Path,
                    target.Library,
                    target.Name,
                    null,
                    target.Sha256)));
            }
            else
            {
                entries.Add(new(file.Path, file.Origin, file.Sha256, file.Source, null, null, null, null));
            }
        }
        return entries
            .OrderBy(item => item.Path, StringComparer.Ordinal)
            .ThenBy(item => item.Symbol, StringComparer.Ordinal)
            .ThenBy(item => item.Member, StringComparer.Ordinal)
            .ToArray();
    }

    private static PortOriginMapEntry[] CreateOriginMappings(
        string staging,
        MigrationIr ir,
        PortWorkspaceDocument ownership)
    {
        var generatedMap = ArtifactFiles.ReadJson<SourceMapDocument>(Path.Combine(staging, "generated-base", "source-map.json"));
        var inputByPath = ir.Inputs.ToDictionary(item => item.Path, StringComparer.Ordinal);
        var fileByPath = ownership.EffectiveFiles.ToDictionary(item => item.Path, StringComparer.Ordinal);
        var mappings = new List<PortOriginMapEntry>();
        foreach (var mapping in generatedMap.Mappings)
        {
            var file = fileByPath[mapping.GeneratedFile];
            var input = inputByPath[mapping.Source];
            mappings.Add(new(
                file.Path,
                PortSchemas.Generated,
                file.Sha256,
                mapping.Source,
                input.Library,
                mapping.Symbol,
                null,
                mapping.SourceOffset,
                mapping.SourceLength));
        }
        var mappedGeneratedPaths = mappings.Select(item => item.Path).ToHashSet(StringComparer.Ordinal);
        foreach (var file in ownership.EffectiveFiles)
        {
            var targets = ownership.SymbolOwnership
                .Where(item => file.Source is not null && item.Source == file.Source)
                .ToArray();
            if (targets.Length > 0)
            {
                mappings.AddRange(targets.Select(target => new PortOriginMapEntry(
                    file.Path,
                    file.Origin,
                    file.Sha256,
                    file.Source,
                    target.Library,
                    target.Symbol,
                    target.Member,
                    null,
                    null)));
            }
            else if (file.Origin != PortSchemas.Generated || !mappedGeneratedPaths.Contains(file.Path))
            {
                mappings.Add(new(file.Path, file.Origin, file.Sha256, file.Source, null, null, null, null, null));
            }
        }
        return mappings
            .OrderBy(item => item.Path, StringComparer.Ordinal)
            .ThenBy(item => item.Symbol, StringComparer.Ordinal)
            .ThenBy(item => item.Member, StringComparer.Ordinal)
            .ToArray();
    }

    private static SortedDictionary<string, string> EnumerateFiles(string root) => new(
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(path => !IsBuildOutput(Path.GetRelativePath(root, path)))
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)), StringComparer.Ordinal)
            .ToDictionary(
                path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)),
                ArtifactFiles.Sha256,
                StringComparer.Ordinal),
        StringComparer.Ordinal);

    private static bool IsBuildOutput(string relativePath) => relativePath
        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .Any(part => part is "bin" or "obj");

    private static string HashText(string value) => Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
