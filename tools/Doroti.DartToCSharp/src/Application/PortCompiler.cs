using System.Security.Cryptography;
using System.Text;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

public sealed record PortGeneratedFile(string Path, string Sha256);

public sealed record PortComposedFile(string Path, string Sha256, string Origin, string? Source);

public sealed record PortSymbolOwnership(
    string Library,
    string Symbol,
    string? Member,
    string Owner,
    string GeneratedBaseSha256,
    string? Source);

public sealed record PortWorkspaceDocument(
    string SchemaVersion,
    string WorkspaceId,
    string Mode,
    PortSource Source,
    string CompilerProfile,
    string PortManifestSha256,
    string CompilerSelectionSha256,
    string GeneratedBase,
    string CompositionVersion,
    string EffectiveProject,
    string[] OwnershipKinds,
    string[] RequiredFixtures,
    PortGeneratedFile[] GeneratedFiles,
    PortGeneratedFile[] ManualSnapshotFiles,
    PortComposedFile[] EffectiveFiles,
    PortGeneratedFile[] WorkspaceFiles,
    PortSymbolOwnership[] SymbolOwnership);

public sealed record PortWorkspace(
    string Path,
    string GeneratedBasePath,
    string EffectivePath,
    ConverterReport Report,
    PortWorkspaceDocument Ownership,
    PortStateDocument State);

/// <summary>
/// Application boundary for a user-owned port, immutable generated base, and composed effective project.
/// </summary>
public sealed class PortCompiler
{
    private static readonly string[] OwnershipKinds =
    [
        PortSchemas.Generated,
        PortSchemas.ManualReplacement,
        PortSchemas.PartialExtension,
        PortSchemas.PlatformPort,
        PortSchemas.AdoptedProduct,
    ];

    private readonly PortManifestLoader _loader = new();
    private readonly DartCompiler _compiler = new();

    public PortWorkspace Compile(string manifestPath, string workspaceRoot, string? cacheDirectory = null)
    {
        var portPath = Path.GetFullPath(manifestPath);
        var manifest = _loader.Load(portPath);
        var selectionPath = PortManifestLoader.ResolveCompilerSelection(portPath, manifest);
        var selection = ArtifactFiles.ReadJson<SelectionManifest>(selectionPath);
        if (!string.Equals(selection.CompatibilityProfile, manifest.CompilerProfile, StringComparison.Ordinal))
        {
            throw Error(
                "DORPORT008",
                $"Port profile '{manifest.CompilerProfile}' does not match compiler selection profile '{selection.CompatibilityProfile}'.");
        }

        var workspaceId = ComputeWorkspaceId(portPath, manifest, selectionPath);
        var root = Path.GetFullPath(workspaceRoot);
        if (Path.GetPathRoot(root) == root)
        {
            throw Error("DORPORT003", "A filesystem root cannot be used as the port workspace root.");
        }
        Directory.CreateDirectory(root);
        var target = Path.Combine(root, workspaceId);
        if (Directory.Exists(target))
        {
            return OpenExisting(target, workspaceId, portPath, manifest, selectionPath);
        }

        var staging = Path.Combine(root, $".{workspaceId}.doroti-staging-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(staging);
            var generatedBase = Path.Combine(staging, "generated-base");
            var report = _compiler.Compile(selectionPath, generatedBase, cacheDirectory);
            var ir = ArtifactFiles.ReadJson<MigrationIr>(Path.Combine(generatedBase, "migration-ir.json"));
            ValidateSelection(manifest, ir);
            var replacements = _loader.LoadReplacements(portPath, manifest);
            var composition = CustomizationComposer.Compose(
                staging,
                generatedBase,
                portPath,
                selectionPath,
                manifest,
                report,
                replacements);
            var ownership = CreateOwnership(
                staging,
                workspaceId,
                portPath,
                manifest,
                selectionPath,
                report,
                ir,
                replacements,
                composition);
            var stateArtifacts = PortStateArtifacts.Create(
                staging,
                workspaceId,
                portPath,
                manifest,
                report,
                ir,
                replacements,
                ownership);
            ArtifactFiles.WriteJson(Path.Combine(staging, "port-state.json"), stateArtifacts.State);
            ArtifactFiles.WriteJson(Path.Combine(staging, "provenance.json"), stateArtifacts.Provenance);
            ArtifactFiles.WriteJson(Path.Combine(staging, "source-map.json"), stateArtifacts.SourceMap);
            ownership = ownership with
            {
                ManualSnapshotFiles = stateArtifacts.ManualSnapshotFiles,
                WorkspaceFiles = new[] { "port-state.json", "provenance.json", "source-map.json" }
                    .Select(path => new PortGeneratedFile(path, ArtifactFiles.Sha256(Path.Combine(staging, path))))
                    .ToArray(),
            };
            ArtifactFiles.WriteJson(Path.Combine(staging, "port-workspace.json"), ownership);

            try
            {
                Directory.Move(staging, target);
            }
            catch (IOException) when (Directory.Exists(target))
            {
                return OpenExisting(target, workspaceId, portPath, manifest, selectionPath);
            }
            return new(target, Path.Combine(target, "generated-base"), Path.Combine(target, "effective"), report, ownership, stateArtifacts.State);
        }
        finally
        {
            DeleteStaging(staging, root);
        }
    }

    public string ComputeWorkspaceId(string manifestPath)
    {
        var portPath = Path.GetFullPath(manifestPath);
        var manifest = _loader.Load(portPath);
        var selectionPath = PortManifestLoader.ResolveCompilerSelection(portPath, manifest);
        return ComputeWorkspaceId(portPath, manifest, selectionPath);
    }

    private string ComputeWorkspaceId(string portPath, PortManifest manifest, string selectionPath)
    {
        var portRoot = Path.GetDirectoryName(portPath)!;
        var inputs = new List<string>
        {
            $"port-manifest:{ArtifactFiles.Sha256(portPath)}",
            $"compiler-workspace:{_compiler.ComputeWorkspaceId(selectionPath)}",
            $"customization-composer:{CustomizationComposer.Version}",
        };
        AddFile(inputs, portRoot, manifest.Customizations.ReplacementManifest, "replacement-manifest");
        foreach (var mapping in manifest.Customizations.MappingFiles ?? [])
        {
            AddFile(inputs, portRoot, mapping, "mapping");
        }
        foreach (var replacement in _loader.LoadReplacements(portPath, manifest))
        {
            AddFile(inputs, portRoot, replacement.Source, "replacement-source");
        }
        foreach (var root in manifest.Customizations.ExtensionRoots ?? [])
        {
            AddTree(inputs, portRoot, root, "extension");
        }
        foreach (var root in manifest.Customizations.PlatformPortRoots ?? [])
        {
            AddTree(inputs, portRoot, root, "platform-port");
        }
        var identity = string.Join('\n', inputs.OrderBy(value => value, StringComparer.Ordinal)) + "\n";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity))).ToLowerInvariant();
    }

    private static PortWorkspace OpenExisting(
        string target,
        string workspaceId,
        string portPath,
        PortManifest manifest,
        string selectionPath)
    {
        PortWorkspaceDocument ownership;
        try
        {
            ownership = ArtifactFiles.ReadJson<PortWorkspaceDocument>(Path.Combine(target, "port-workspace.json"));
        }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException)
        {
            throw Error("DORPORT004", $"Compiler-owned workspace metadata was edited or removed: {exception.Message}");
        }
        if (ownership.SchemaVersion != PortSchemas.Workspace ||
            ownership.WorkspaceId != workspaceId ||
            ownership.PortManifestSha256 != ArtifactFiles.Sha256(portPath) ||
            ownership.CompilerSelectionSha256 != ArtifactFiles.Sha256(selectionPath) ||
            ownership.CompositionVersion != CustomizationComposer.Version)
        {
            throw Error("DORPORT004", "Compiler-owned workspace metadata no longer matches its content identity.");
        }

        var generatedBase = Path.Combine(target, "generated-base");
        var actual = EnumerateGeneratedFiles(generatedBase);
        var expected = ownership.GeneratedFiles.ToDictionary(item => item.Path, item => item.Sha256, StringComparer.Ordinal);
        if (!actual.Keys.SequenceEqual(expected.Keys, StringComparer.Ordinal))
        {
            var changedSet = actual.Keys.Except(expected.Keys, StringComparer.Ordinal)
                .Concat(expected.Keys.Except(actual.Keys, StringComparer.Ordinal))
                .OrderBy(value => value, StringComparer.Ordinal);
            throw Error("DORPORT004", $"Compiler-owned generated file set was edited: {string.Join(", ", changedSet)}");
        }
        foreach (var path in actual.Keys)
        {
            if (!string.Equals(actual[path], expected[path], StringComparison.Ordinal))
            {
                throw Error("DORPORT004", $"Compiler-owned generated file was edited: {path}");
            }
        }

        var effective = Path.Combine(target, "effective");
        var actualEffective = EnumerateGeneratedFiles(effective);
        var expectedEffective = ownership.EffectiveFiles.ToDictionary(item => item.Path, item => item.Sha256, StringComparer.Ordinal);
        if (!actualEffective.Keys.SequenceEqual(expectedEffective.Keys, StringComparer.Ordinal))
        {
            var changedSet = actualEffective.Keys.Except(expectedEffective.Keys, StringComparer.Ordinal)
                .Concat(expectedEffective.Keys.Except(actualEffective.Keys, StringComparer.Ordinal))
                .OrderBy(value => value, StringComparer.Ordinal);
            throw Error("DORPORT004", $"Compiler-owned effective file set was edited: {string.Join(", ", changedSet)}");
        }
        foreach (var path in actualEffective.Keys)
        {
            if (!string.Equals(actualEffective[path], expectedEffective[path], StringComparison.Ordinal))
            {
                throw Error("DORPORT004", $"Compiler-owned effective file was edited: {path}");
            }
        }

        ValidateInventory(Path.Combine(target, "manual-snapshot"), ownership.ManualSnapshotFiles, "manual snapshot");
        foreach (var file in ownership.WorkspaceFiles)
        {
            var path = Path.Combine(target, file.Path);
            if (!File.Exists(path) || !string.Equals(ArtifactFiles.Sha256(path), file.Sha256, StringComparison.Ordinal))
            {
                throw Error("DORPORT004", $"Compiler-owned workspace artifact was edited or removed: {file.Path}");
            }
        }

        var report = ArtifactFiles.ReadJson<ConverterReport>(Path.Combine(generatedBase, "converter-report.json"));
        var state = ArtifactFiles.ReadJson<PortStateDocument>(Path.Combine(target, "port-state.json"));
        if (state.SchemaVersion != PortSchemas.State || state.WorkspaceId != workspaceId)
        {
            throw Error("DORPORT004", "Compiler-owned port state no longer matches the workspace identity.");
        }
        return new(target, generatedBase, effective, report, ownership, state);
    }

    private static PortWorkspaceDocument CreateOwnership(
        string staging,
        string workspaceId,
        string portPath,
        PortManifest manifest,
        string selectionPath,
        ConverterReport report,
        MigrationIr ir,
        PortReplacement[] replacements,
        CustomizationComposition composition)
    {
        var portRoot = Path.GetDirectoryName(portPath)!;
        var outputByPath = report.Outputs.ToDictionary(item => item.Input, StringComparer.Ordinal);
        var replacementsBySymbol = replacements
            .Where(item => string.IsNullOrWhiteSpace(item.Member))
            .ToDictionary(item => PortManifestLoader.TargetKey(item.Library, item.Symbol, null), StringComparer.Ordinal);
        var ownership = new List<PortSymbolOwnership>();
        foreach (var input in ir.Inputs.OrderBy(item => item.Library, StringComparer.Ordinal))
        {
            if (!outputByPath.TryGetValue(input.Path, out var output))
            {
                throw Error("DORPORT008", $"Compiler report omitted selected input '{input.Path}'.");
            }
            foreach (var symbol in input.SelectedSymbols.OrderBy(value => value, StringComparer.Ordinal))
            {
                var key = PortManifestLoader.TargetKey(input.Library, symbol, null);
                if (replacementsBySymbol.TryGetValue(key, out var replacement))
                {
                    ValidateReplacementHash(replacement, output.Sha256);
                    ownership.Add(new(
                        input.Library,
                        symbol,
                        null,
                        PortSchemas.ManualReplacement,
                        output.Sha256,
                        ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, Path.GetFullPath(replacement.Source, portRoot)))));
                }
                else
                {
                    ownership.Add(new(input.Library, symbol, null, PortSchemas.Generated, output.Sha256, null));
                }
            }
        }
        foreach (var replacement in replacements.Where(item => !string.IsNullOrWhiteSpace(item.Member))
                     .OrderBy(item => PortManifestLoader.TargetKey(item.Library, item.Symbol, item.Member), StringComparer.Ordinal))
        {
            var input = ir.Inputs.Single(item => item.Library == replacement.Library);
            var declaration = input.Declarations.Single(item => item.Name == replacement.Symbol);
            if (!declaration.Members.Any(item => item.Name == replacement.Member))
            {
                throw Error("DORPORT007", $"Replacement member does not exist: {replacement.Library}#{replacement.Symbol}.{replacement.Member}");
            }
            var output = outputByPath[input.Path];
            ValidateReplacementHash(replacement, output.Sha256);
            ownership.Add(new(
                replacement.Library,
                replacement.Symbol,
                replacement.Member,
                PortSchemas.ManualReplacement,
                output.Sha256,
                ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, Path.GetFullPath(replacement.Source, portRoot)))));
        }

        return new(
            PortSchemas.Workspace,
            workspaceId,
            manifest.Mode,
            manifest.Source,
            manifest.CompilerProfile,
            ArtifactFiles.Sha256(portPath),
            ArtifactFiles.Sha256(selectionPath),
            "generated-base",
            CustomizationComposer.Version,
            $"effective/{composition.ProjectRelativePath}",
            OwnershipKinds,
            manifest.RequiredFixtures.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            EnumerateGeneratedFiles(Path.Combine(staging, "generated-base"))
                .Select(item => new PortGeneratedFile(item.Key, item.Value))
                .ToArray(),
            [],
            composition.Files,
            [],
            ownership.OrderBy(item => PortManifestLoader.TargetKey(item.Library, item.Symbol, item.Member), StringComparer.Ordinal).ToArray());
    }

    private static void ValidateSelection(PortManifest manifest, MigrationIr ir)
    {
        if (!string.Equals(ir.CompatibilityProfile, manifest.CompilerProfile, StringComparison.Ordinal))
        {
            throw Error("DORPORT008", "Generated Migration IR profile differs from the port manifest.");
        }
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
            throw Error("DORPORT008", "Port inputs do not exactly match the resolved compiler selection.");
        }
    }

    private static void ValidateReplacementHash(PortReplacement replacement, string actualHash)
    {
        if (!string.Equals(replacement.GeneratedBaseSha256, actualHash, StringComparison.OrdinalIgnoreCase))
        {
            throw Error(
                "DORPORT006",
                $"Replacement base is stale for {PortManifestLoader.TargetKey(replacement.Library, replacement.Symbol, replacement.Member)}; " +
                $"expected {replacement.GeneratedBaseSha256}, generated {actualHash}.");
        }
    }

    private static SortedDictionary<string, string> EnumerateGeneratedFiles(string generatedBase)
    {
        if (!Directory.Exists(generatedBase))
        {
            throw Error("DORPORT004", "Compiler-owned generated-base directory is missing.");
        }
        var files = Directory.EnumerateFiles(generatedBase, "*", SearchOption.AllDirectories)
            .Where(path => !IsBuildOutput(Path.GetRelativePath(generatedBase, path)))
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(generatedBase, path)), StringComparer.Ordinal)
            .ToDictionary(
                path => ArtifactFiles.NormalizePath(Path.GetRelativePath(generatedBase, path)),
                ArtifactFiles.Sha256,
                StringComparer.Ordinal);
        return new(files, StringComparer.Ordinal);
    }

    private static void ValidateInventory(string root, PortGeneratedFile[] expectedFiles, string description)
    {
        var actual = EnumerateGeneratedFiles(root);
        var expected = expectedFiles.ToDictionary(item => item.Path, item => item.Sha256, StringComparer.Ordinal);
        if (!actual.Keys.SequenceEqual(expected.Keys, StringComparer.Ordinal))
        {
            throw Error("DORPORT004", $"Compiler-owned {description} file set was edited.");
        }
        foreach (var path in actual.Keys)
        {
            if (!string.Equals(actual[path], expected[path], StringComparison.Ordinal))
            {
                throw Error("DORPORT004", $"Compiler-owned {description} file was edited: {path}");
            }
        }
    }

    private static bool IsBuildOutput(string relativePath) => relativePath
        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .Any(part => part is "bin" or "obj");

    private static void AddFile(List<string> inputs, string portRoot, string? relativePath, string kind)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }
        var path = PortManifestLoader.ResolveUserPath(portRoot, relativePath, requireDirectory: false);
        inputs.Add($"{kind}:{ArtifactFiles.NormalizePath(relativePath)}:{ArtifactFiles.Sha256(path)}");
    }

    private static void AddTree(List<string> inputs, string portRoot, string relativeRoot, string kind)
    {
        var root = PortManifestLoader.ResolveUserPath(portRoot, relativeRoot, requireDirectory: true);
        foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                     .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, path)), StringComparer.Ordinal))
        {
            inputs.Add($"{kind}:{ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, path))}:{ArtifactFiles.Sha256(path)}");
        }
    }

    private static void DeleteStaging(string staging, string workspaceRoot)
    {
        if (!Directory.Exists(staging))
        {
            return;
        }
        var actualParent = Path.GetFullPath(Path.GetDirectoryName(staging)!);
        if (!string.Equals(actualParent, Path.GetFullPath(workspaceRoot), StringComparison.OrdinalIgnoreCase) ||
            !Path.GetFileName(staging).Contains(".doroti-staging-", StringComparison.Ordinal))
        {
            throw Error("DORPORT003", $"Refusing to clean an unowned staging directory: {staging}");
        }
        Directory.Delete(staging, recursive: true);
    }

    private static PortContractException Error(string code, string message) => new(code, message);
}
