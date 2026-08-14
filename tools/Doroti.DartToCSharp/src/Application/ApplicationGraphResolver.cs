using System.Text.Json;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed record ApplicationGraphPlan(
    string PackageConfigPath,
    string EntryPoint,
    ApplicationLibraryNode[] Libraries,
    ApplicationImportEdge[] Edges,
    ApplicationSccNode[] Sccs,
    string[] AffectedLibraries,
    string ResourceManifestPath,
    string PluginManifestPath,
    string ContractSha256);

internal sealed record ApplicationLibraryNode(string Library, string Path, string Sha256);
internal sealed record ApplicationImportEdge(
    string From,
    string To,
    string Directive,
    string[] Candidates,
    string? Condition);
internal sealed record ApplicationSccNode(string Id, string[] Libraries, bool IsCycle);

internal static partial class ApplicationGraphResolver
{
    private sealed record ImportDirective(string Directive, string[] Candidates, string Selected, string? Condition);

    public static (SelectionManifest Manifest, ApplicationGraphPlan Plan) Expand(
        SelectionManifest manifest,
        string manifestDirectory,
        string? previousOutputDirectory)
    {
        var application = manifest.Application
            ?? throw new InvalidDataException("Application graph expansion requires an application manifest.");
        if (string.IsNullOrWhiteSpace(manifest.PackageRoot) || string.IsNullOrWhiteSpace(manifest.EntryPoint))
        {
            throw new InvalidDataException("Application compilation requires packageRoot and entryPoint.");
        }
        if (manifest.Inputs.Length != 0)
        {
            throw new InvalidDataException("Application compilation discovers its package closure from entryPoint; fixture-specific inputs are not allowed.");
        }

        var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
        var packageConfigPath = Path.Combine(packageRoot, ".dart_tool", "package_config.json");
        if (!File.Exists(packageConfigPath))
        {
            throw new InvalidDataException($"Application package configuration is missing: {packageConfigPath}");
        }
        var resourceManifestPath = ResolveWithinPackage(packageRoot, application.ResourceManifest);
        var pluginManifestPath = ResolveWithinPackage(packageRoot, application.PluginManifest);
        if (!File.Exists(resourceManifestPath) || !File.Exists(pluginManifestPath))
        {
            throw new InvalidDataException("Application resource/plugin manifest is missing.");
        }

        var conditions = new Dictionary<string, bool>(StringComparer.Ordinal)
        {
            ["dart.library.io"] = true,
            ["dart.library.html"] = false,
            ["dart.library.js_interop"] = false,
        };
        foreach (var pair in application.Conditions ?? []) conditions[pair.Key] = pair.Value;

        var pending = new Queue<string>();
        var physicalByLibrary = new Dictionary<string, string>(StringComparer.Ordinal);
        var edges = new List<ApplicationImportEdge>();
        pending.Enqueue(manifest.EntryPoint);
        while (pending.TryDequeue(out var library))
        {
            if (physicalByLibrary.ContainsKey(library)) continue;
            var physical = ResolveLibrary(packageConfigPath, library);
            physicalByLibrary.Add(library, physical);
            var source = File.ReadAllText(physical);
            foreach (var import in ParseImports(source, conditions))
            {
                var selectedLibrary = CanonicalizeImport(packageConfigPath, library, physical, import.Selected);
                if (selectedLibrary is null) continue;
                var candidates = import.Candidates
                    .Select(candidate => CanonicalizeImport(packageConfigPath, library, physical, candidate))
                    .Where(candidate => candidate is not null)
                    .Cast<string>()
                    .Distinct(StringComparer.Ordinal)
                    .Order(StringComparer.Ordinal)
                    .ToArray();
                edges.Add(new(library, selectedLibrary, import.Directive, candidates, import.Condition));
                foreach (var candidate in candidates.Where(candidate => !candidate.StartsWith("package:flutter/", StringComparison.Ordinal)))
                {
                    pending.Enqueue(candidate);
                }
            }
        }

        var libraries = physicalByLibrary.OrderBy(item => item.Key, StringComparer.Ordinal)
            .Select(item => new ApplicationLibraryNode(item.Key, item.Value, ArtifactFiles.Sha256(item.Value)))
            .ToArray();
        var localEdges = edges.SelectMany(edge => edge.Candidates
                .Where(physicalByLibrary.ContainsKey)
                .Select(candidate => new ApplicationImportEdge(edge.From, candidate, edge.Directive, edge.Candidates, edge.Condition)))
            .DistinctBy(edge => (edge.From, edge.To))
            .OrderBy(edge => edge.From, StringComparer.Ordinal)
            .ThenBy(edge => edge.To, StringComparer.Ordinal)
            .ToArray();
        var sccs = ComputeSccs(libraries.Select(item => item.Library).ToArray(), localEdges);
        var contractSha = HashText(
            ArtifactFiles.Sha256(resourceManifestPath) + "\n" +
            ArtifactFiles.Sha256(pluginManifestPath) + "\n" +
            application.TargetRid + "\n" +
            string.Join('\n', application.FrameworkPackages.Order(StringComparer.Ordinal)));
        var affected = ComputeAffected(previousOutputDirectory, libraries, localEdges, sccs, contractSha);
        var inputs = libraries.Select(item => new SelectionInput(
            item.Library,
            ["*"],
            item.Library,
            "generate")).ToArray();
        return (
            manifest with { Inputs = inputs },
            new(
                packageConfigPath,
                manifest.EntryPoint,
                libraries,
                edges.OrderBy(edge => edge.From, StringComparer.Ordinal).ThenBy(edge => edge.To, StringComparer.Ordinal).ToArray(),
                sccs,
                affected,
                resourceManifestPath,
                pluginManifestPath,
                contractSha));
    }

    private static string ResolveWithinPackage(string packageRoot, string relative)
    {
        if (Path.IsPathRooted(relative)) throw new InvalidDataException("Application manifest paths must be package-relative.");
        var root = packageRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var resolved = Path.GetFullPath(relative, packageRoot);
        if (!resolved.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Application path escapes packageRoot: {relative}");
        }
        return resolved;
    }

    private static string ResolveLibrary(string packageConfigPath, string library)
    {
        if (!library.StartsWith("package:", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Application libraries must use package URIs: {library}");
        }
        using var config = JsonDocument.Parse(File.ReadAllText(packageConfigPath));
        var slash = library.IndexOf('/', "package:".Length);
        if (slash < 0) throw new InvalidDataException($"Invalid package URI: {library}");
        var packageName = library["package:".Length..slash];
        var relative = library[(slash + 1)..];
        var package = config.RootElement.GetProperty("packages").EnumerateArray()
            .SingleOrDefault(item => item.GetProperty("name").GetString() == packageName);
        if (package.ValueKind == JsonValueKind.Undefined)
        {
            throw new InvalidDataException($"Package is absent from package_config.json: {packageName}");
        }
        var configUri = new Uri(Path.GetFullPath(packageConfigPath));
        var root = new Uri(configUri, EnsureSlash(package.GetProperty("rootUri").GetString()!));
        var packageUri = package.TryGetProperty("packageUri", out var value) ? value.GetString() ?? "lib/" : "lib/";
        var path = new Uri(root, EnsureSlash(packageUri) + relative).LocalPath;
        return File.Exists(path) ? path : throw new FileNotFoundException($"Application library does not exist: {library}", path);
    }

    private static string? CanonicalizeImport(string packageConfigPath, string ownerLibrary, string ownerPath, string import)
    {
        if (import.StartsWith("dart:", StringComparison.Ordinal)) return null;
        if (import.StartsWith("package:", StringComparison.Ordinal)) return import;
        if (Uri.TryCreate(import, UriKind.Absolute, out _))
        {
            throw new InvalidDataException($"Unsupported application import URI: {import}");
        }
        var path = Path.GetFullPath(import.Replace('/', Path.DirectorySeparatorChar), Path.GetDirectoryName(ownerPath)!);
        using var config = JsonDocument.Parse(File.ReadAllText(packageConfigPath));
        foreach (var package in config.RootElement.GetProperty("packages").EnumerateArray())
        {
            var configUri = new Uri(Path.GetFullPath(packageConfigPath));
            var root = new Uri(configUri, EnsureSlash(package.GetProperty("rootUri").GetString()!));
            var packageUri = package.TryGetProperty("packageUri", out var value) ? value.GetString() ?? "lib/" : "lib/";
            var libRoot = new Uri(root, EnsureSlash(packageUri)).LocalPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!path.StartsWith(libRoot, StringComparison.OrdinalIgnoreCase)) continue;
            return $"package:{package.GetProperty("name").GetString()}/{ArtifactFiles.NormalizePath(Path.GetRelativePath(libRoot, path))}";
        }
        throw new InvalidDataException($"Relative import from {ownerLibrary} resolves outside the package graph: {import}");
    }

    private static ImportDirective[] ParseImports(string source, IReadOnlyDictionary<string, bool> conditions)
    {
        var result = new List<ImportDirective>();
        foreach (Match match in ImportDirectiveRegex().Matches(source))
        {
            var body = match.Groups["body"].Value;
            var uris = QuotedUriRegex().Matches(body).Select(item => item.Groups["uri"].Value).ToArray();
            if (uris.Length == 0) continue;
            var selected = uris[0];
            string? selectedCondition = null;
            foreach (Match condition in ConditionalUriRegex().Matches(body))
            {
                var name = condition.Groups["condition"].Value;
                if (conditions.TryGetValue(name, out var enabled) && enabled)
                {
                    selected = condition.Groups["uri"].Value;
                    selectedCondition = name;
                }
            }
            result.Add(new(match.Value.Trim(), uris, selected, selectedCondition));
        }
        return result.ToArray();
    }

    private static ApplicationSccNode[] ComputeSccs(string[] libraries, ApplicationImportEdge[] edges)
    {
        var adjacency = libraries.ToDictionary(
            item => item,
            item => edges.Where(edge => edge.From == item).Select(edge => edge.To).Order(StringComparer.Ordinal).ToArray(),
            StringComparer.Ordinal);
        var index = 0;
        var indices = new Dictionary<string, int>(StringComparer.Ordinal);
        var low = new Dictionary<string, int>(StringComparer.Ordinal);
        var stack = new Stack<string>();
        var onStack = new HashSet<string>(StringComparer.Ordinal);
        var components = new List<string[]>();
        void Visit(string library)
        {
            indices[library] = low[library] = index++;
            stack.Push(library);
            onStack.Add(library);
            foreach (var dependency in adjacency[library])
            {
                if (!indices.ContainsKey(dependency)) { Visit(dependency); low[library] = Math.Min(low[library], low[dependency]); }
                else if (onStack.Contains(dependency)) low[library] = Math.Min(low[library], indices[dependency]);
            }
            if (low[library] != indices[library]) return;
            var component = new List<string>();
            string current;
            do { current = stack.Pop(); onStack.Remove(current); component.Add(current); } while (current != library);
            components.Add(component.Order(StringComparer.Ordinal).ToArray());
        }
        foreach (var library in libraries.Order(StringComparer.Ordinal)) if (!indices.ContainsKey(library)) Visit(library);
        return components.OrderBy(item => item[0], StringComparer.Ordinal).Select((item, position) => new ApplicationSccNode(
            $"scc-{position:D3}", item, item.Length > 1 || edges.Any(edge => edge.From == item[0] && edge.To == item[0]))).ToArray();
    }

    private static string[] ComputeAffected(
        string? previousOutputDirectory,
        ApplicationLibraryNode[] libraries,
        ApplicationImportEdge[] edges,
        ApplicationSccNode[] sccs,
        string contractSha)
    {
        var changed = new HashSet<string>(StringComparer.Ordinal);
        var previousPath = string.IsNullOrWhiteSpace(previousOutputDirectory)
            ? null
            : Path.Combine(previousOutputDirectory, "application-graph.json");
        if (previousPath is null || !File.Exists(previousPath)) return libraries.Select(item => item.Library).ToArray();
        using var previous = JsonDocument.Parse(File.ReadAllText(previousPath));
        if (previous.RootElement.GetProperty("contractSha256").GetString() != contractSha)
        {
            return libraries.Select(item => item.Library).ToArray();
        }
        var hashes = previous.RootElement.GetProperty("libraries").EnumerateArray().ToDictionary(
            item => item.GetProperty("library").GetString()!,
            item => item.GetProperty("sha256").GetString()!,
            StringComparer.Ordinal);
        foreach (var library in libraries)
        {
            if (!hashes.TryGetValue(library.Library, out var hash) || hash != library.Sha256) changed.Add(library.Library);
        }
        foreach (var removed in hashes.Keys.Except(libraries.Select(item => item.Library), StringComparer.Ordinal)) changed.Add(removed);
        var affected = new HashSet<string>(changed.Where(item => libraries.Any(library => library.Library == item)), StringComparer.Ordinal);
        var queue = new Queue<string>(affected);
        while (queue.TryDequeue(out var dependency))
        {
            foreach (var importer in edges.Where(edge => edge.To == dependency).Select(edge => edge.From))
            {
                if (affected.Add(importer)) queue.Enqueue(importer);
            }
            var component = sccs.SingleOrDefault(item => item.Libraries.Contains(dependency, StringComparer.Ordinal));
            if (component is null) continue;
            foreach (var member in component.Libraries) if (affected.Add(member)) queue.Enqueue(member);
        }
        return affected.Order(StringComparer.Ordinal).ToArray();
    }

    private static string EnsureSlash(string value) => value.EndsWith("/", StringComparison.Ordinal) ? value : value + "/";
    private static string HashText(string value) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    [GeneratedRegex(@"(?ms)^\s*(?:import|export)\s+(?<body>.*?);")]
    private static partial Regex ImportDirectiveRegex();
    [GeneratedRegex("['\"](?<uri>[^'\"]+)['\"]", RegexOptions.CultureInvariant)]
    private static partial Regex QuotedUriRegex();
    [GeneratedRegex("if\\s*\\(\\s*(?<condition>[A-Za-z0-9_.]+)\\s*\\)\\s*['\"](?<uri>[^'\"]+)['\"]", RegexOptions.CultureInvariant)]
    private static partial Regex ConditionalUriRegex();
}
