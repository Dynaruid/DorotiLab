using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.AvaloniaPort;

public static partial class AvaloniaPortWorkflow
{
    public const string GraphSchemaVersion = "doroti.avalonia-shell-closure/v1";
    public const string AuditSchemaVersion = "doroti.avalonia-port-audit/v1";
    private static readonly string[] AllowedDispositions = ["import", "adapt", "Doroti-port", "exclude-with-owner"];
    private static readonly string[] IgnoredDirectoryNames = [".git", "bin", "obj", "artifacts"];

    public static AvaloniaPortAuditReport Update(string repositoryRoot, string configPath)
    {
        var context = Load(repositoryRoot, configPath);
        var graph = CreateGraph(context.Config, context.SourceRoot, context.Config.PinnedRevision);
        ArtifactFiles.WriteJson(context.GraphPath, graph);
        return CreateAuditReport(context, graph, compareCommitted: false);
    }

    public static AvaloniaPortAuditReport Audit(string repositoryRoot, string configPath)
    {
        var context = Load(repositoryRoot, configPath);
        var actualGraph = CreateGraph(context.Config, context.SourceRoot, context.Config.PinnedRevision);
        return CreateAuditReport(context, actualGraph, compareCommitted: true);
    }

    public static AvaloniaPortStageReport Stage(string repositoryRoot, string configPath, string outputDirectory)
    {
        var context = Load(repositoryRoot, configPath);
        var audit = Audit(repositoryRoot, configPath);
        if (!audit.Success)
        {
            return new("doroti.avalonia-port-stage/v1", false, "blocked", audit.SourceIdentity, [], audit.Findings);
        }

        var graph = ArtifactFiles.ReadJson<AvaloniaShellClosureGraph>(context.GraphPath);
        var outputRoot = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(outputRoot);
        var staged = new List<AvaloniaPortStageFile>();
        foreach (var file in graph.Files.Where(item => item.Disposition is "import" or "adapt").OrderBy(item => item.Path, StringComparer.Ordinal))
        {
            var sourcePath = RepositoryPaths.ResolveWithin(context.SourceRoot, file.Path);
            var targetPath = RepositoryPaths.ResolveWithin(outputRoot, file.TargetProject + "/" + file.Path);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            var bytes = File.ReadAllBytes(sourcePath);
            if (file.Disposition == "adapt" && Path.GetExtension(file.Path).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                var text = Encoding.UTF8.GetString(bytes);
                foreach (var rewrite in context.Config.NamespaceRewrites)
                {
                    text = text.Replace(rewrite.From, rewrite.To, StringComparison.Ordinal);
                }
                bytes = new UTF8Encoding(false).GetBytes(text.Replace("\r\n", "\n", StringComparison.Ordinal));
            }
            File.WriteAllBytes(targetPath, bytes);
            ApplyPatches(context, file.Path, targetPath);
            staged.Add(new(file.Path, ArtifactFiles.NormalizePath(Path.GetRelativePath(outputRoot, targetPath)), file.Disposition, file.Sha256, ArtifactFiles.Sha256(targetPath), file.Owner));
        }

        var report = new AvaloniaPortStageReport("doroti.avalonia-port-stage/v1", true, "ready-for-review", audit.SourceIdentity, staged.ToArray(), []);
        ArtifactFiles.WriteJson(Path.Combine(outputRoot, "port-provenance.json"), report);
        File.Copy(Path.Combine(context.SourceRoot, context.Config.License), Path.Combine(outputRoot, Path.GetFileName(context.Config.License)), overwrite: true);
        return report;
    }

    public static AvaloniaPortRebaseReport Rebase(string repositoryRoot, string configPath, string previousSource, string currentSource)
    {
        var context = Load(repositoryRoot, configPath);
        var selectedPaths = SelectFiles(context.Config, context.SourceRoot).Select(item => item.Path).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        var files = new List<AvaloniaPortRebaseFile>();
        foreach (var path in selectedPaths)
        {
            var previous = RepositoryPaths.ResolveWithin(Path.GetFullPath(previousSource), path);
            var current = RepositoryPaths.ResolveWithin(Path.GetFullPath(currentSource), path);
            var previousHash = File.Exists(previous) ? ArtifactFiles.Sha256(previous) : null;
            var currentHash = File.Exists(current) ? ArtifactFiles.Sha256(current) : null;
            var status = previousHash is null ? "added" : currentHash is null ? "removed" : previousHash == currentHash ? "clean" : "manual-review";
            files.Add(new(path, status, previousHash, currentHash));
        }
        var success = files.All(item => item.Status is "clean" or "added");
        return new("doroti.avalonia-port-rebase/v1", success, success ? "clean" : "manual-review", files.ToArray());
    }

    private static PortContext Load(string repositoryRoot, string configPath)
    {
        var config = ArtifactFiles.ReadJson<AvaloniaPortSelection>(configPath);
        if (config.SchemaVersion != "doroti.avalonia-port-selection/v1")
        {
            throw new InvalidDataException($"Unsupported Avalonia port selection schema: {config.SchemaVersion}");
        }
        var sourceRoot = Path.GetFullPath(config.SourceRoot, Path.GetDirectoryName(configPath)!);
        return new(
            Path.GetFullPath(repositoryRoot),
            Path.GetFullPath(configPath),
            sourceRoot,
            RepositoryPaths.ResolveWithin(repositoryRoot, config.GraphPath),
            config);
    }

    private static IEnumerable<string> EnumerateSourceFiles(string sourceRoot) =>
        Directory.EnumerateFiles(sourceRoot, "*", SearchOption.AllDirectories)
            .Where(path => !ArtifactFiles.NormalizePath(Path.GetRelativePath(sourceRoot, path)).Split('/').Any(segment => IgnoredDirectoryNames.Contains(segment, StringComparer.OrdinalIgnoreCase)));

    private static AvaloniaShellClosureGraph CreateGraph(AvaloniaPortSelection config, string sourceRoot, string sourceIdentity)
    {
        var selected = SelectFiles(config, sourceRoot).ToArray();
        var selectedPaths = selected.Select(item => item.Path).ToHashSet(StringComparer.Ordinal);
        var allCSharp = Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !ArtifactFiles.NormalizePath(Path.GetRelativePath(sourceRoot, path)).Split('/').Any(segment => IgnoredDirectoryNames.Contains(segment, StringComparer.OrdinalIgnoreCase)))
            .Where(path =>
            {
                var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(sourceRoot, path));
                return relative.StartsWith("src/", StringComparison.Ordinal) || relative.StartsWith("packages/", StringComparison.Ordinal);
            })
            .Select(path => ParseSource(sourceRoot, path))
            .ToArray();
        var symbols = allCSharp.SelectMany(file => file.Symbols.Select(symbol => (Symbol: symbol, File: file.Path)))
            .GroupBy(item => item.Symbol, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(item => item.File).Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray(), StringComparer.Ordinal);
        var simpleSymbols = symbols.Where(item => item.Value.Any(selectedPaths.Contains)).Select(item => item.Key)
            .GroupBy(symbol => symbol[(symbol.LastIndexOf('.') + 1)..], StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToArray(), StringComparer.Ordinal);
        var selectedByPath = selected.ToDictionary(item => item.Path, StringComparer.Ordinal);
        var dependencies = new Dictionary<string, AvaloniaDependencyNode>(StringComparer.Ordinal);
        var files = new List<AvaloniaClosureFile>();
        foreach (var selectedFile in selected)
        {
            var parsed = allCSharp.FirstOrDefault(item => item.Path == selectedFile.Path) ?? ParseSource(sourceRoot, RepositoryPaths.ResolveWithin(sourceRoot, selectedFile.Path));
            var direct = new HashSet<string>(StringComparer.Ordinal);
            foreach (var reference in parsed.References)
            {
                var targets = ResolveReference(reference, symbols, simpleSymbols);
                if (targets.Length == 0 && reference.Contains('.'))
                {
                    targets = [reference];
                }
                foreach (var target in targets)
                {
                    var dependency = ClassifyDependency(target, symbols, selectedByPath, config.DependencyRules);
                    dependencies.TryAdd(dependency.Id, dependency);
                    direct.Add(dependency.Id);
                }
            }
            files.Add(new(selectedFile.Path, selectedFile.SourceSetId, selectedFile.Disposition, selectedFile.Owner, selectedFile.TargetProject, selectedFile.Platforms, ArtifactFiles.Sha256(RepositoryPaths.ResolveWithin(sourceRoot, selectedFile.Path)), parsed.Symbols, direct.OrderBy(item => item, StringComparer.Ordinal).ToArray()));
        }

        var selectedSymbols = files.SelectMany(file => file.Symbols).ToHashSet(StringComparer.Ordinal);
        var seeds = config.SeedSymbols.Select(seed => new AvaloniaSeedResult(seed.Platform, seed.Symbol, seed.SourcePath, selectedSymbols.Contains(seed.Symbol) && files.Any(file => file.Path == seed.SourcePath))).ToArray();
        var unclassified = dependencies.Values.Count(item => !AllowedDispositions.Contains(item.Disposition, StringComparer.Ordinal));
        return new(
            GraphSchemaVersion,
            sourceIdentity,
            files.Count,
            selectedSymbols.Count,
            dependencies.Count,
            unclassified,
            seeds,
            config.PlatformDecisions,
            config.NativeAssets,
            config.CompileStages,
            files.OrderBy(item => item.Path, StringComparer.Ordinal).ToArray(),
            dependencies.Values.OrderBy(item => item.Id, StringComparer.Ordinal).ToArray());
    }

    private static IEnumerable<SelectedFile> SelectFiles(AvaloniaPortSelection config, string sourceRoot)
    {
        var sourceSets = config.SourceSets.ToArray();
        foreach (var path in EnumerateSourceFiles(sourceRoot).OrderBy(item => item, StringComparer.Ordinal))
        {
            var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(sourceRoot, path));
            var sourceSet = sourceSets.FirstOrDefault(set => set.Paths.Any(prefix => MatchesPath(relative, prefix)));
            if (sourceSet is null || !sourceSet.Extensions.Contains(Path.GetExtension(relative), StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }
            yield return new(relative, sourceSet.Id, sourceSet.Disposition, sourceSet.Owner, sourceSet.TargetProject, sourceSet.Platforms);
        }
    }

    private static bool MatchesPath(string path, string prefix)
    {
        var normalized = ArtifactFiles.NormalizePath(prefix).TrimEnd('/');
        return path.Equals(normalized, StringComparison.Ordinal) || path.StartsWith(normalized + "/", StringComparison.Ordinal);
    }

    private static ParsedSource ParseSource(string sourceRoot, string path)
    {
        var text = File.ReadAllText(path);
        var namespaces = NamespaceRegex().Matches(text).ToArray();
        var symbols = DeclarationRegex().Matches(text).Select(match =>
        {
            var sourceNamespace = namespaces.LastOrDefault(item => item.Index < match.Index)?.Groups[1].Value ?? string.Empty;
            return sourceNamespace.Length == 0 ? match.Groups[1].Value : sourceNamespace + "." + match.Groups[1].Value;
        }).Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        var references = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match match in UsingRegex().Matches(text))
        {
            references.Add(match.Groups[1].Value);
        }
        foreach (Match match in QualifiedAvaloniaRegex().Matches(text))
        {
            references.Add(match.Value.Replace("global::", string.Empty, StringComparison.Ordinal));
        }
        foreach (Match match in TypeIdentifierRegex().Matches(text))
        {
            references.Add(match.Value);
        }
        foreach (var symbol in symbols)
        {
            references.Remove(symbol);
            references.Remove(symbol[(symbol.LastIndexOf('.') + 1)..]);
        }
        return new(ArtifactFiles.NormalizePath(Path.GetRelativePath(sourceRoot, path)), symbols, references.OrderBy(item => item, StringComparer.Ordinal).ToArray());
    }

    private static string[] ResolveReference(string reference, IReadOnlyDictionary<string, string[]> symbols, IReadOnlyDictionary<string, string[]> simpleSymbols)
    {
        var normalized = reference.Replace("global::", string.Empty, StringComparison.Ordinal);
        if (symbols.ContainsKey(normalized))
        {
            return [normalized];
        }
        for (var index = normalized.Length; index > 0; index = normalized.LastIndexOf('.', index - 1))
        {
            var prefix = normalized[..index];
            if (symbols.ContainsKey(prefix))
            {
                return [prefix];
            }
        }
        return !normalized.Contains('.') && simpleSymbols.TryGetValue(normalized, out var candidates) && candidates.Length == 1 ? candidates : [];
    }

    private static AvaloniaDependencyNode ClassifyDependency(string id, IReadOnlyDictionary<string, string[]> symbols, IReadOnlyDictionary<string, SelectedFile> selected, AvaloniaDependencyRule[] rules)
    {
        if (symbols.TryGetValue(id, out var files))
        {
            var selectedFile = files.Select(path => selected.GetValueOrDefault(path)).FirstOrDefault(item => item is not null);
            if (selectedFile is not null)
            {
                return new(id, "selected-symbol", selectedFile.Disposition, selectedFile.Owner, files);
            }
        }
        var rule = rules.FirstOrDefault(item => item.Prefixes.Any(prefix => id.Equals(prefix, StringComparison.Ordinal) || id.StartsWith(prefix + ".", StringComparison.Ordinal)));
        return rule is null
            ? new(id, "unclassified", "unclassified", "unassigned", symbols.GetValueOrDefault(id) ?? [])
            : new(id, symbols.ContainsKey(id) ? "source-symbol" : "external-contract", rule.Disposition, rule.Owner, symbols.GetValueOrDefault(id) ?? []);
    }

    private static void ApplyPatches(PortContext context, string sourcePath, string targetPath)
    {
        foreach (var patchPath in context.Config.PatchFiles.OrderBy(item => item, StringComparer.Ordinal))
        {
            var document = ArtifactFiles.ReadJson<AvaloniaTextPatchDocument>(RepositoryPaths.ResolveWithin(context.RepositoryRoot, patchPath));
            foreach (var patch in document.Patches.Where(item => item.SourcePath == sourcePath))
            {
                var text = File.ReadAllText(targetPath);
                if (!text.Contains(patch.OldText, StringComparison.Ordinal))
                {
                    throw new InvalidDataException($"Patch source text was not found in {sourcePath}: {patch.Id}");
                }
                text = text.Replace(patch.OldText, patch.NewText, StringComparison.Ordinal);
                ArtifactFiles.WriteUtf8(targetPath, text);
            }
        }
    }

    private static AvaloniaPortAuditReport CreateAuditReport(PortContext context, AvaloniaShellClosureGraph graph, bool compareCommitted)
    {
        var findings = new List<AvaloniaPortFinding>();
        if (!Regex.IsMatch(context.Config.PinnedRevision, "^git:[0-9a-f]{40}$", RegexOptions.CultureInvariant) ||
            string.IsNullOrWhiteSpace(context.Config.UpstreamRef) ||
            !Uri.TryCreate(context.Config.UpstreamRepository, UriKind.Absolute, out _))
        {
            findings.Add(new("DOTA0-001", "error", "upstream-pin", "An official repository, reference, and immutable 40-hex Git revision are required."));
        }
        if (!graph.Seeds.All(item => item.Found))
        {
            foreach (var seed in graph.Seeds.Where(item => !item.Found))
            {
                findings.Add(new("DOTA0-002", "error", seed.Symbol, $"Seed symbol was not selected from {seed.SourcePath}."));
            }
        }
        if (graph.UnclassifiedCount != 0)
        {
            findings.Add(new("DOTA0-003", "error", "dependency-closure", $"{graph.UnclassifiedCount} dependency nodes have no import/adapt/Doroti-port/exclude-with-owner classification."));
        }
        foreach (var stage in graph.CompileStages.Where(item => item.AvaloniaBinaryPackages.Length != 0))
        {
            findings.Add(new("DOTA0-004", "error", stage.Id, "A source-port compile stage retains an Avalonia binary package."));
        }
        if (graph.NativeAssets.Length == 0 || graph.PlatformDecisions.SelectMany(item => item.Platforms).Distinct(StringComparer.Ordinal).Count() < 3)
        {
            findings.Add(new("DOTA0-005", "error", "platform-closure", "Windows, Linux, macOS and macOS native-asset decisions are required."));
        }
        if (compareCommitted)
        {
            CompareCommitted(context.GraphPath, graph, "dependency graph", findings);
        }
        var success = findings.Count == 0;
        return new(AuditSchemaVersion, success, success ? "pass" : "fail", graph.SourceIdentity, graph.SelectedFileCount, graph.SelectedSymbolCount, graph.DependencyCount, findings.ToArray());
    }

    private static void CompareCommitted<T>(string path, T value, string subject, List<AvaloniaPortFinding> findings)
    {
        if (!File.Exists(path))
        {
            findings.Add(new("DOTA0-006", "error", subject, $"Committed artifact is missing: {path}"));
            return;
        }
        var expected = File.ReadAllBytes(path);
        var json = System.Text.Json.JsonSerializer.Serialize(value, ArtifactFiles.JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal) + "\n";
        if (!expected.AsSpan().SequenceEqual(Encoding.UTF8.GetBytes(json)))
        {
            findings.Add(new("DOTA0-007", "error", subject, "Committed artifact drifted; run Doroti.AvaloniaPort update and review the result."));
        }
    }

    [GeneratedRegex(@"\bnamespace\s+([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*)", RegexOptions.CultureInvariant)]
    private static partial Regex NamespaceRegex();

    [GeneratedRegex(@"\b(?:class|interface|struct|enum|delegate\s+[^;{()]+|record(?:\s+class|\s+struct)?)\s+([A-Za-z_]\w*)", RegexOptions.CultureInvariant)]
    private static partial Regex DeclarationRegex();

    [GeneratedRegex(@"^\s*(?:global\s+)?using\s+(?:static\s+)?(?:[A-Za-z_]\w*\s*=\s*)?([A-Za-z_]\w*(?:\.[A-Za-z_]\w*)*)\s*;", RegexOptions.Multiline | RegexOptions.CultureInvariant)]
    private static partial Regex UsingRegex();

    [GeneratedRegex(@"(?:global::)?Avalonia(?:\.[A-Za-z_]\w*)+", RegexOptions.CultureInvariant)]
    private static partial Regex QualifiedAvaloniaRegex();

    [GeneratedRegex(@"\bI?[A-Z][A-Za-z0-9_]{2,}\b", RegexOptions.CultureInvariant)]
    private static partial Regex TypeIdentifierRegex();

    private sealed record PortContext(string RepositoryRoot, string ConfigPath, string SourceRoot, string GraphPath, AvaloniaPortSelection Config);
    private sealed record SelectedFile(string Path, string SourceSetId, string Disposition, string Owner, string TargetProject, string[] Platforms);
    private sealed record ParsedSource(string Path, string[] Symbols, string[] References);
}

public sealed record AvaloniaPortSelection(
    string SchemaVersion,
    string SourceId,
    string SourceRoot,
    string License,
    string UpstreamRepository,
    string UpstreamRef,
    string PinnedRevision,
    string GraphPath,
    AvaloniaNamespaceRewrite[] NamespaceRewrites,
    string[] PatchFiles,
    AvaloniaSourceSet[] SourceSets,
    AvaloniaDependencyRule[] DependencyRules,
    AvaloniaSeedSymbol[] SeedSymbols,
    AvaloniaPlatformDecision[] PlatformDecisions,
    AvaloniaNativeAsset[] NativeAssets,
    AvaloniaCompileStage[] CompileStages);

public sealed record AvaloniaNamespaceRewrite(string From, string To);
public sealed record AvaloniaSourceSet(string Id, string[] Platforms, string[] Paths, string[] Extensions, string Disposition, string Owner, string TargetProject, string Reason);
public sealed record AvaloniaDependencyRule(string Id, string[] Prefixes, string Disposition, string Owner, string Reason);
public sealed record AvaloniaSeedSymbol(string Platform, string Symbol, string SourcePath);
public sealed record AvaloniaPlatformDecision(string Id, string[] Platforms, string Decision, string Owner, string Reason);
public sealed record AvaloniaNativeAsset(string Id, string[] Platforms, string[] Paths, string License, string Build, string RebaseOwner);
public sealed record AvaloniaCompileStage(string Id, string[] Projects, string[] SourceSets, string[] DorotiPorts, string[] AvaloniaBinaryPackages, string Completion);
public sealed record AvaloniaShellClosureGraph(
    string SchemaVersion,
    string SourceIdentity,
    int SelectedFileCount,
    int SelectedSymbolCount,
    int DependencyCount,
    int UnclassifiedCount,
    AvaloniaSeedResult[] Seeds,
    AvaloniaPlatformDecision[] PlatformDecisions,
    AvaloniaNativeAsset[] NativeAssets,
    AvaloniaCompileStage[] CompileStages,
    AvaloniaClosureFile[] Files,
    AvaloniaDependencyNode[] Dependencies);
public sealed record AvaloniaSeedResult(string Platform, string Symbol, string SourcePath, bool Found);
public sealed record AvaloniaClosureFile(string Path, string SourceSet, string Disposition, string Owner, string TargetProject, string[] Platforms, string Sha256, string[] Symbols, string[] DirectDependencies);
public sealed record AvaloniaDependencyNode(string Id, string Kind, string Disposition, string Owner, string[] SourcePaths);
public sealed record AvaloniaPortAuditReport(string SchemaVersion, bool Success, string Status, string SourceIdentity, int SelectedFileCount, int SelectedSymbolCount, int DependencyCount, AvaloniaPortFinding[] Findings);
public sealed record AvaloniaPortFinding(string Code, string Severity, string Subject, string Message);
public sealed record AvaloniaPortStageReport(string SchemaVersion, bool Success, string Status, string SourceIdentity, AvaloniaPortStageFile[] Files, AvaloniaPortFinding[] Findings);
public sealed record AvaloniaPortStageFile(string SourcePath, string TargetPath, string Disposition, string SourceSha256, string AdaptedSha256, string Owner);
public sealed record AvaloniaTextPatchDocument(string SchemaVersion, AvaloniaTextPatch[] Patches);
public sealed record AvaloniaTextPatch(string Id, string SourcePath, string OldText, string NewText);
public sealed record AvaloniaPortRebaseReport(string SchemaVersion, bool Success, string Status, AvaloniaPortRebaseFile[] Files);
public sealed record AvaloniaPortRebaseFile(string Path, string Status, string? PreviousSha256, string? CurrentSha256);
