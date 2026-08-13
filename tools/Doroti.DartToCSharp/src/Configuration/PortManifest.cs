using Doroti.Tooling;

namespace Doroti.DartToCSharp;

public static class PortSchemas
{
    public const string Manifest = "doroti.port/v1";
    public const string Replacements = "doroti.replacements/v1";
    public const string Workspace = "doroti.port-workspace/v1";
    public const string State = "doroti.port-state/v1";
    public const string Provenance = "doroti.port-provenance/v1";
    public const string SourceMap = "doroti.port-source-map/v1";
    public const string Adoption = "doroti.adoption/v1";
    public const string Rebase = "doroti.rebase-report/v1";
    public const string RegeneratablePackage = "regeneratable-package";
    public const string RuntimeAdoption = "runtime-adoption";
    public const string FlutterOrDartPackage = "flutter-or-dart-package";
    public const string Generated = "generated";
    public const string ManualReplacement = "manual-replacement";
    public const string PartialExtension = "partial-extension";
    public const string PlatformPort = "platform-port";
    public const string AdoptedProduct = "adopted-product";
    public const string Clean = "clean";
    public const string ManualReview = "manual-review";
    public const string Conflict = "conflict";
    public const string UpstreamSymbolRemoved = "upstream-symbol-removed";
    public const string FixtureRequired = "fixture-required";
}

public sealed record PortManifest(
    string SchemaVersion,
    string Mode,
    PortSource Source,
    string CompilerProfile,
    string CompilerSelection,
    PortInput[] Inputs,
    PortCustomizations Customizations,
    string[] RequiredFixtures);

public sealed record PortSource(string Kind, string Revision, string License);

public sealed record PortInput(string Library, string[] Symbols);

public sealed record PortCustomizations(
    string[] MappingFiles,
    string? ReplacementManifest,
    string[] ExtensionRoots,
    string[] PlatformPortRoots);

public sealed record PortReplacementManifest(string SchemaVersion, PortReplacement[] Replacements);

public sealed record PortReplacement(
    string Library,
    string Symbol,
    string GeneratedBaseSha256,
    string Source,
    string? Member = null);

public sealed class PortContractException : Exception
{
    public PortContractException(string code, string message)
        : base($"{code}: {message}")
    {
        Code = code;
    }

    public string Code { get; }
}

public sealed class PortManifestLoader
{
    public PortManifest Load(string manifestPath)
    {
        var path = Path.GetFullPath(manifestPath);
        var manifest = Read<PortManifest>(path, PortSchemas.Manifest, "DORPORT001");
        var portRoot = Path.GetDirectoryName(path)!;

        if (manifest.Mode is not PortSchemas.RegeneratablePackage and not PortSchemas.RuntimeAdoption)
        {
            throw Error("DORPORT002", $"Unsupported port mode '{manifest.Mode}'.");
        }
        RequireText(manifest.Source?.Kind, "source.kind");
        if (manifest.Source!.Kind != PortSchemas.FlutterOrDartPackage)
        {
            throw Error("DORPORT001", $"Unsupported source kind '{manifest.Source.Kind}'.");
        }
        RequireText(manifest.Source?.Revision, "source.revision");
        RequireText(manifest.Source?.License, "source.license");
        RequireText(manifest.CompilerProfile, "compilerProfile");
        RequireRelativePath(manifest.CompilerSelection, "compilerSelection");
        if (manifest.Inputs is null || manifest.Inputs.Length == 0)
        {
            throw Error("DORPORT001", "inputs must select at least one library and symbol.");
        }
        if (manifest.Customizations is null)
        {
            throw Error("DORPORT001", "customizations is required.");
        }
        if (manifest.RequiredFixtures is null || manifest.RequiredFixtures.Length == 0 ||
            manifest.RequiredFixtures.Any(string.IsNullOrWhiteSpace))
        {
            throw Error("DORPORT001", "requiredFixtures must contain at least one fixture id.");
        }
        RequireUnique(manifest.RequiredFixtures, "required fixture");

        var selectedSymbols = new HashSet<string>(StringComparer.Ordinal);
        foreach (var input in manifest.Inputs)
        {
            RequireText(input?.Library, "inputs.library");
            if (input!.Symbols is null || input.Symbols.Length == 0 || input.Symbols.Any(string.IsNullOrWhiteSpace))
            {
                throw Error("DORPORT001", $"Port input '{input.Library}' must select at least one symbol.");
            }
            foreach (var symbol in input.Symbols)
            {
                if (!selectedSymbols.Add(TargetKey(input.Library, symbol, null)))
                {
                    throw Error("DORPORT005", $"Duplicate selected symbol ownership: {input.Library}#{symbol}.");
                }
            }
        }

        ValidateCustomizationPaths(portRoot, manifest.Customizations);
        var replacements = LoadReplacements(path, manifest);
        var replacementTargets = new HashSet<string>(StringComparer.Ordinal);
        var replacementScopes = new Dictionary<string, List<string?>>(StringComparer.Ordinal);
        foreach (var replacement in replacements)
        {
            RequireText(replacement.Library, "replacement.library");
            RequireText(replacement.Symbol, "replacement.symbol");
            RequireSha256(replacement.GeneratedBaseSha256, "replacement.generatedBaseSha256");
            RequireRelativePath(replacement.Source, "replacement.source");
            ResolveUserPath(portRoot, replacement.Source, requireDirectory: false);
            var key = TargetKey(replacement.Library, replacement.Symbol, replacement.Member);
            if (!replacementTargets.Add(key))
            {
                throw Error("DORPORT005", $"Multiple manual owners claim {key}.");
            }
            var symbolKey = TargetKey(replacement.Library, replacement.Symbol, null);
            if (!replacementScopes.TryGetValue(symbolKey, out var members))
            {
                members = [];
                replacementScopes.Add(symbolKey, members);
            }
            members.Add(replacement.Member);
            if (members.Count > 1 && members.Any(string.IsNullOrWhiteSpace))
            {
                throw Error("DORPORT005", $"A whole-symbol replacement overlaps a member replacement for {symbolKey}.");
            }
            if (!selectedSymbols.Contains(TargetKey(replacement.Library, replacement.Symbol, null)))
            {
                throw Error("DORPORT007", $"Replacement target is not selected by the port: {key}.");
            }
        }

        return manifest;
    }

    public PortReplacement[] LoadReplacements(string manifestPath, PortManifest manifest)
    {
        if (string.IsNullOrWhiteSpace(manifest.Customizations.ReplacementManifest))
        {
            return [];
        }
        var portRoot = Path.GetDirectoryName(Path.GetFullPath(manifestPath))!;
        var replacementPath = ResolveUserPath(portRoot, manifest.Customizations.ReplacementManifest, requireDirectory: false);
        var document = Read<PortReplacementManifest>(replacementPath, PortSchemas.Replacements, "DORPORT001");
        return document.Replacements ?? throw Error("DORPORT001", "replacements is required.");
    }

    internal static string ResolveUserPath(string portRoot, string relativePath, bool requireDirectory)
    {
        RequireRelativePath(relativePath, "customization path");
        string path;
        try
        {
            path = RepositoryPaths.ResolveWithin(portRoot, relativePath);
        }
        catch (InvalidOperationException exception)
        {
            throw Error("DORPORT003", exception.Message);
        }
        var exists = requireDirectory ? Directory.Exists(path) : File.Exists(path);
        if (!exists)
        {
            throw Error("DORPORT003", $"Port-owned {(requireDirectory ? "directory" : "file")} does not exist: {relativePath}");
        }
        return path;
    }

    internal static string ResolveCompilerSelection(string manifestPath, PortManifest manifest)
    {
        RequireRelativePath(manifest.CompilerSelection, "compilerSelection");
        var portRoot = Path.GetDirectoryName(Path.GetFullPath(manifestPath))!;
        var path = Path.GetFullPath(manifest.CompilerSelection, portRoot);
        if (!File.Exists(path))
        {
            throw Error("DORPORT003", $"Compiler selection does not exist: {manifest.CompilerSelection}");
        }
        return path;
    }

    internal static string TargetKey(string library, string symbol, string? member) =>
        string.IsNullOrWhiteSpace(member) ? $"{library}#{symbol}" : $"{library}#{symbol}.{member}";

    private static T Read<T>(string path, string schema, string code)
    {
        T document;
        try
        {
            document = ArtifactFiles.ReadJson<T>(path);
        }
        catch (Exception exception) when (exception is IOException or System.Text.Json.JsonException)
        {
            throw Error(code, $"Could not read {Path.GetFileName(path)}: {exception.Message}");
        }
        var actual = document switch
        {
            PortManifest value => value.SchemaVersion,
            PortReplacementManifest value => value.SchemaVersion,
            _ => null,
        };
        if (!string.Equals(actual, schema, StringComparison.Ordinal))
        {
            throw Error(code, $"Unsupported schema '{actual ?? "<missing>"}'; expected '{schema}'.");
        }
        return document;
    }

    private static void ValidateCustomizationPaths(string portRoot, PortCustomizations customizations)
    {
        foreach (var mapping in customizations.MappingFiles ?? [])
        {
            ResolveUserPath(portRoot, mapping, requireDirectory: false);
        }
        if (!string.IsNullOrWhiteSpace(customizations.ReplacementManifest))
        {
            ResolveUserPath(portRoot, customizations.ReplacementManifest, requireDirectory: false);
        }
        foreach (var root in customizations.ExtensionRoots ?? [])
        {
            ResolveUserPath(portRoot, root, requireDirectory: true);
        }
        foreach (var root in customizations.PlatformPortRoots ?? [])
        {
            ResolveUserPath(portRoot, root, requireDirectory: true);
        }
    }

    private static void RequireText(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw Error("DORPORT001", $"{name} is required.");
        }
    }

    private static void RequireRelativePath(string? value, string name)
    {
        RequireText(value, name);
        if (Path.IsPathRooted(value!))
        {
            throw Error("DORPORT003", $"{name} must be checkout-relative, not absolute: {value}");
        }
    }

    private static void RequireSha256(string? value, string name)
    {
        if (value is null || value.Length != 64 || value.Any(character => !Uri.IsHexDigit(character)))
        {
            throw Error("DORPORT001", $"{name} must be a 64-character SHA-256 value.");
        }
    }

    private static void RequireUnique(IEnumerable<string> values, string name)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            if (!seen.Add(value))
            {
                throw Error("DORPORT005", $"Duplicate {name}: {value}");
            }
        }
    }

    private static PortContractException Error(string code, string message) => new(code, message);
}
