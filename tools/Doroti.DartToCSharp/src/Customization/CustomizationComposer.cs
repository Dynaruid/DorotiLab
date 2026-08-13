using Doroti.Tooling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Doroti.DartToCSharp;

internal sealed record CustomizationComposition(
    string Path,
    string ProjectRelativePath,
    PortComposedFile[] Files);

internal static class CustomizationComposer
{
    public const string Version = "p1.0";

    public static CustomizationComposition Compose(
        string staging,
        string generatedBase,
        string portPath,
        string selectionPath,
        PortManifest manifest,
        ConverterReport report,
        PortReplacement[] replacements)
    {
        var effective = Path.Combine(staging, "effective");
        var origins = new Dictionary<string, (string Origin, string? Source)>(StringComparer.Ordinal);
        CopyGeneratedBase(generatedBase, effective, origins);

        var outputByLibrary = CreateOutputByLibrary(generatedBase, report);
        SuppressGeneratedImplementations(effective, outputByLibrary, replacements);

        var portRoot = Path.GetDirectoryName(portPath)!;
        CopyReplacementSources(portRoot, effective, replacements, origins);
        CopyCustomizationRoots(
            portRoot,
            effective,
            manifest.Customizations.ExtensionRoots ?? [],
            "manual/extensions",
            PortSchemas.PartialExtension,
            origins);
        CopyCustomizationRoots(
            portRoot,
            effective,
            manifest.Customizations.PlatformPortRoots ?? [],
            "manual/platform-ports",
            PortSchemas.PlatformPort,
            origins);

        ValidateReplacementProviders(portRoot, generatedBase, outputByLibrary, replacements);
        var project = Directory.EnumerateFiles(effective, "*.csproj", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        if (project.Length != 1)
        {
            throw Error("DORPORT010", $"Effective workspace must contain exactly one top-level project; found {project.Length}.");
        }
        ValidateBuild(project[0], selectionPath, effective);

        var files = EnumerateFiles(effective)
            .Select(item =>
            {
                var origin = origins.TryGetValue(item.Key, out var value)
                    ? value
                    : (PortSchemas.Generated, (string?)null);
                return new PortComposedFile(item.Key, item.Value, origin.Item1, origin.Item2);
            })
            .ToArray();
        return new(
            effective,
            ArtifactFiles.NormalizePath(Path.GetRelativePath(effective, project[0])),
            files);
    }

    private static Dictionary<string, string> CreateOutputByLibrary(string generatedBase, ConverterReport report)
    {
        var ir = ArtifactFiles.ReadJson<MigrationIr>(Path.Combine(generatedBase, "migration-ir.json"));
        var outputByInput = report.Outputs.ToDictionary(item => item.Input, item => item.Output, StringComparer.Ordinal);
        return ir.Inputs.ToDictionary(
            input => input.Library,
            input => outputByInput.TryGetValue(input.Path, out var output)
                ? output
                : throw Error("DORPORT008", $"Compiler report omitted selected input '{input.Path}'."),
            StringComparer.Ordinal);
    }

    private static void SuppressGeneratedImplementations(
        string effective,
        IReadOnlyDictionary<string, string> outputByLibrary,
        PortReplacement[] replacements)
    {
        foreach (var group in replacements.GroupBy(item => item.Library, StringComparer.Ordinal))
        {
            if (!outputByLibrary.TryGetValue(group.Key, out var relativeOutput))
            {
                throw Error("DORPORT007", $"Replacement library was not generated: {group.Key}");
            }
            var path = Path.Combine(effective, relativeOutput);
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
            var root = tree.GetCompilationUnitRoot();
            foreach (var replacement in group.OrderBy(
                         item => PortManifestLoader.TargetKey(item.Library, item.Symbol, item.Member),
                         StringComparer.Ordinal))
            {
                var nodes = FindGeneratedTargets(root, replacement).ToArray();
                if (nodes.Length != 1)
                {
                    throw Error(
                        "DORPORT009",
                        $"Replacement must suppress exactly one generated implementation for " +
                        $"{PortManifestLoader.TargetKey(replacement.Library, replacement.Symbol, replacement.Member)}; found {nodes.Length}.");
                }
                root = root.RemoveNode(nodes[0], SyntaxRemoveOptions.KeepExteriorTrivia)
                    ?? throw Error("DORPORT009", $"Could not suppress generated target {replacement.Symbol}.");
            }
            ArtifactFiles.WriteUtf8(path, root.ToFullString());
        }
    }

    private static IEnumerable<SyntaxNode> FindGeneratedTargets(CompilationUnitSyntax root, PortReplacement replacement)
    {
        var types = root.DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .Where(type => type.Identifier.ValueText == replacement.Symbol &&
                !type.Ancestors().OfType<BaseTypeDeclarationSyntax>().Any())
            .ToArray();
        if (string.IsNullOrWhiteSpace(replacement.Member))
        {
            return types;
        }
        return types.OfType<TypeDeclarationSyntax>()
            .SelectMany(type => type.Members)
            .Where(member => MemberNames(member).Contains(replacement.Member!, StringComparer.Ordinal));
    }

    private static IEnumerable<string> MemberNames(MemberDeclarationSyntax member) => member switch
    {
        MethodDeclarationSyntax method => [method.Identifier.ValueText],
        PropertyDeclarationSyntax property => [property.Identifier.ValueText],
        EventDeclarationSyntax eventDeclaration => [eventDeclaration.Identifier.ValueText],
        ConstructorDeclarationSyntax constructor => [constructor.Identifier.ValueText],
        FieldDeclarationSyntax field => field.Declaration.Variables.Select(item => item.Identifier.ValueText),
        EventFieldDeclarationSyntax eventField => eventField.Declaration.Variables.Select(item => item.Identifier.ValueText),
        _ => [],
    };

    private static void ValidateReplacementProviders(
        string portRoot,
        string generatedBase,
        IReadOnlyDictionary<string, string> outputByLibrary,
        PortReplacement[] replacements)
    {
        foreach (var sourceGroup in replacements.GroupBy(item => item.Source, StringComparer.Ordinal))
        {
            var sourcePath = PortManifestLoader.ResolveUserPath(portRoot, sourceGroup.Key, requireDirectory: false);
            var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(sourcePath), path: sourceGroup.Key);
            var errors = tree.GetDiagnostics().Where(item => item.Severity == DiagnosticSeverity.Error).ToArray();
            if (errors.Length > 0)
            {
                throw Error("DORPORT010", $"Replacement source has invalid C# syntax: {sourceGroup.Key}: {errors[0].GetMessage()}");
            }
            var root = tree.GetCompilationUnitRoot();
            foreach (var replacement in sourceGroup)
            {
                var generatedRoot = CSharpSyntaxTree.ParseText(
                        File.ReadAllText(Path.Combine(generatedBase, outputByLibrary[replacement.Library])))
                    .GetCompilationUnitRoot();
                var generatedTarget = FindGeneratedTargets(generatedRoot, replacement).Single();
                var expectedNamespace = ContainingNamespace(generatedTarget);
                var provided = FindGeneratedTargets(root, replacement)
                    .Count(item => string.Equals(ContainingNamespace(item), expectedNamespace, StringComparison.Ordinal));
                if (provided != 1)
                {
                    throw Error(
                        "DORPORT009",
                        $"Replacement source must provide exactly one implementation for " +
                        $"{PortManifestLoader.TargetKey(replacement.Library, replacement.Symbol, replacement.Member)} " +
                        $"in namespace '{expectedNamespace}'; found {provided} in {replacement.Source}.");
                }
            }
        }
    }

    private static string ContainingNamespace(SyntaxNode node) => node.AncestorsAndSelf()
        .OfType<BaseNamespaceDeclarationSyntax>()
        .Select(item => item.Name.ToString())
        .FirstOrDefault() ?? string.Empty;

    private static void CopyGeneratedBase(
        string generatedBase,
        string effective,
        Dictionary<string, (string Origin, string? Source)> origins)
    {
        foreach (var source in Directory.EnumerateFiles(generatedBase, "*", SearchOption.AllDirectories)
                     .Where(path => !IsBuildOutput(Path.GetRelativePath(generatedBase, path)))
                     .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(generatedBase, path)), StringComparer.Ordinal))
        {
            var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(generatedBase, source));
            CopyFile(source, Path.Combine(effective, relative));
            origins.Add(relative, (PortSchemas.Generated, null));
        }
    }

    private static void CopyReplacementSources(
        string portRoot,
        string effective,
        PortReplacement[] replacements,
        Dictionary<string, (string Origin, string? Source)> origins)
    {
        foreach (var relativeSource in replacements.Select(item => item.Source).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal))
        {
            var source = PortManifestLoader.ResolveUserPath(portRoot, relativeSource, requireDirectory: false);
            var relative = ArtifactFiles.NormalizePath(Path.Combine("manual/replacements", relativeSource));
            CopyFile(source, Path.Combine(effective, relative));
            origins.Add(relative, (PortSchemas.ManualReplacement, ArtifactFiles.NormalizePath(relativeSource)));
        }
    }

    private static void CopyCustomizationRoots(
        string portRoot,
        string effective,
        IEnumerable<string> relativeRoots,
        string targetPrefix,
        string origin,
        Dictionary<string, (string Origin, string? Source)> origins)
    {
        var index = 0;
        foreach (var relativeRoot in relativeRoots.OrderBy(value => value, StringComparer.Ordinal))
        {
            var root = PortManifestLoader.ResolveUserPath(portRoot, relativeRoot, requireDirectory: true);
            foreach (var source in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                         .Where(path => !IsBuildOutput(Path.GetRelativePath(root, path)))
                         .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)), StringComparer.Ordinal))
            {
                var sourceRelative = ArtifactFiles.NormalizePath(Path.GetRelativePath(portRoot, source));
                var relative = ArtifactFiles.NormalizePath(Path.Combine(targetPrefix, index.ToString("D2"), Path.GetRelativePath(root, source)));
                CopyFile(source, Path.Combine(effective, relative));
                origins.Add(relative, (origin, sourceRelative));
            }
            index++;
        }
    }

    private static void ValidateBuild(string projectPath, string selectionPath, string effective)
    {
        var selection = ArtifactFiles.ReadJson<SelectionManifest>(selectionPath);
        var analyzerHome = AnalyzerHomeResolver.Resolve(selectionPath, selection);
        var repositoryRoot = analyzerHome.DorotiRoot;
        var result = ProcessRunner.Run(
            "dotnet",
            ["build", projectPath, "--nologo", "--verbosity", "quiet", $"-p:DorotiRepositoryRoot={repositoryRoot}"],
            effective);
        if (result.ExitCode != 0)
        {
            throw Error("DORPORT010", $"Effective project did not compile.\n{result.StandardError}\n{result.StandardOutput}".Trim());
        }
    }

    private static SortedDictionary<string, string> EnumerateFiles(string root)
    {
        var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(path => !IsBuildOutput(Path.GetRelativePath(root, path)))
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)), StringComparer.Ordinal)
            .ToDictionary(
                path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)),
                ArtifactFiles.Sha256,
                StringComparer.Ordinal);
        return new(files, StringComparer.Ordinal);
    }

    private static bool IsBuildOutput(string relativePath) => relativePath
        .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        .Any(part => part is "bin" or "obj");

    private static void CopyFile(string source, string target)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(source, target, overwrite: false);
    }

    private static PortContractException Error(string code, string message) => new(code, message);
}
