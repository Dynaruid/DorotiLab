using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static partial class ConverterEngine
{
    private static string FrameworkPartition(string value)
    {
        var normalized = ArtifactFiles.NormalizePath(value);
        var marker = normalized.IndexOf("/src/", StringComparison.Ordinal);
        if (marker >= 0)
        {
            var tail = normalized[(marker + 5)..];
            var slash = tail.IndexOf('/');
            var area = slash < 0 ? Path.GetFileNameWithoutExtension(tail) : tail[..slash];
            return PascalCase(area);
        }
        if (normalized.StartsWith("package:flutter/", StringComparison.Ordinal))
        {
            return PascalCase(Path.GetFileNameWithoutExtension(normalized["package:flutter/".Length..]));
        }
        return "Framework";
    }

    private static string FrameworkNamespace(string root, string value) => $"{root}.{FrameworkPartition(value)}";

    private static void WriteFrameworkProjectGraph(
        string outputDirectory,
        SelectionManifest manifest,
        string manifestDirectory,
        CompilerIdentity identity,
        List<MigrationIrInput> inputs,
        List<ConverterOutput> outputs,
        List<ConverterDiagnostic> diagnostics)
    {
        var orderedInputs = inputs.OrderBy(item => item.Library, StringComparer.Ordinal).ToArray();
        var knownLibraries = orderedInputs.Select(item => item.Library).ToHashSet(StringComparer.Ordinal);
        var edges = orderedInputs
            .SelectMany(input => input.Imports
                .Where(knownLibraries.Contains)
                .Select(import => new FrameworkLibraryEdge(input.Library, import)))
            .Distinct()
            .OrderBy(item => item.From, StringComparer.Ordinal)
            .ThenBy(item => item.To, StringComparer.Ordinal)
            .ToArray();
        var components = ComputeStronglyConnectedComponents(knownLibraries, edges);
        var componentByLibrary = components
            .SelectMany((component, index) => component.Select(library => (library, index)))
            .ToDictionary(item => item.library, item => item.index, StringComparer.Ordinal);
        var libraries = orderedInputs.Select(input =>
        {
            var graph = input.LibraryGraph;
            var importDetails = graph?.ImportDetails ?? [];
            return new FrameworkLibraryNode(
                input.Library,
                FrameworkPartition(input.Library),
                $"{FrameworkNamespace(manifest.OutputNamespace, input.Library)}",
                componentByLibrary[input.Library],
                input.SelectedSymbols,
                input.Declarations.Select(item => item.Element!.CanonicalId).OrderBy(item => item, StringComparer.Ordinal).ToArray(),
                graph?.Fragments.Select(fragment => new FrameworkFragmentNode(
                    fragment.Uri,
                    fragment.OwnerLibrary ?? input.Library,
                    fragment.IsDefining,
                    fragment.Declarations)).OrderBy(item => item.Uri, StringComparer.Ordinal).ToArray() ?? [],
                importDetails.Select(item => new FrameworkImportNode(item.Uri, item.Prefix, item.IsSynthetic))
                    .OrderBy(item => item.Uri, StringComparer.Ordinal).ToArray(),
                graph?.AccessibleExtensions ?? []);
        }).ToArray();

        var emittedPartitions = outputs
            .Select(output => FrameworkPartition(output.Input))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(item => item, StringComparer.Ordinal)
            .ToArray();
        var partitionDependencies = orderedInputs
            .SelectMany(input => input.Imports.Select(import => new FrameworkProjectReference(
                FrameworkPartition(input.Library),
                FrameworkPartition(import))))
            .Where(edge => edge.From != edge.To && emittedPartitions.Contains(edge.From, StringComparer.Ordinal) && emittedPartitions.Contains(edge.To, StringComparer.Ordinal))
            .Distinct()
            .OrderBy(item => item.From, StringComparer.Ordinal)
            .ThenBy(item => item.To, StringComparer.Ordinal)
            .ToArray();

        foreach (var partition in emittedPartitions)
        {
            var directory = Path.Combine(outputDirectory, "projects", partition);
            Directory.CreateDirectory(directory);
            var assemblyName = $"{manifest.OutputAssemblyName}.{partition}";
            var references = partitionDependencies.Where(item => item.From == partition).Select(item => item.To).ToArray();
            var projectReferences = string.Join('\n', references.Select(reference =>
                $"    <ProjectReference Include=\"..\\{reference}\\{manifest.OutputAssemblyName}.{reference}.csproj\" />"));
            var inactiveConditionalOutputs = outputs
                .Where(output => FrameworkPartition(output.Input) == partition &&
                    ((output.Input.EndsWith("_web.dart", StringComparison.Ordinal) &&
                      outputs.Any(candidate => string.Equals(
                          candidate.Input,
                          output.Input[..^"_web.dart".Length] + "_io.dart",
                          StringComparison.Ordinal))) ||
                     (manifest.FrameworkMilestone is "G5-3" or "G5-4" &&
                      output.Input.EndsWith("/widgets/_window_linux.dart", StringComparison.Ordinal)) ||
                     (manifest.FrameworkMilestone is "G5-3" or "G5-4" &&
                      output.Input.EndsWith("/widgets/_window_macos.dart", StringComparison.Ordinal)) ||
                     (manifest.FrameworkMilestone is "G5-3" or "G5-4" &&
                      output.Input.EndsWith("/widgets/_window_win32.dart", StringComparison.Ordinal))))
                .Select(output => Path.GetFileName(output.Output))
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            var inactiveCompileItems = string.Join('\n', inactiveConditionalOutputs.Select(name =>
                $"    <Compile Remove=\"{name}\" />"));
            var promotedFrameworkReferences = BuildPromotedFrameworkReferences(
                manifest.FrameworkMilestone,
                identity.RuntimeBindingVersion,
                includeWidgets: manifest.FrameworkMilestone is "G5-3" or "G5-4" &&
                    !outputs.Any(output => output.Input.Contains("/widgets/", StringComparison.Ordinal)));
            ArtifactFiles.WriteUtf8(
                Path.Combine(directory, assemblyName + ".csproj"),
                $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                    <LangVersion>14.0</LangVersion>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
                    <Deterministic>true</Deterministic>
                    <AssemblyName>{assemblyName}</AssemblyName>
                  </PropertyGroup>
                  <ItemGroup>
                    <ProjectReference Include="$(DorotiRepositoryRoot)\src\Doroti.Flutter.Runtime\Doroti.Flutter.Runtime.csproj" Condition="'$(DorotiRepositoryRoot)' != ''" />
                    <ProjectReference Include="$(DorotiRepositoryRoot)\src\Doroti.Flutter.Ui\Doroti.Flutter.Ui.csproj" Condition="'$(DorotiRepositoryRoot)' != ''" />
                    <PackageReference Include="Doroti.Flutter.Runtime" Version="[{identity.RuntimeBindingVersion}]" Condition="'$(DorotiRepositoryRoot)' == ''" />
                    <PackageReference Include="Doroti.Flutter.Ui" Version="[{identity.RuntimeBindingVersion}]" Condition="'$(DorotiRepositoryRoot)' == ''" />
                {promotedFrameworkReferences}
                {projectReferences}
                {inactiveCompileItems}
                  </ItemGroup>
                </Project>
                """ + "\n");
            if (manifest.FrameworkMilestone is "G4-3" or "G4-4" or "G4-5" or "G5-3" or "G5-4")
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "Foundation.GlobalUsings.g.cs"),
                    "global using Doroti.Generated.Framework.Foundation;\n");
            }
            if (manifest.FrameworkMilestone is "G4-4" or "G4-5" or "G5-3" or "G5-4")
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "SchedulerServices.GlobalUsings.g.cs"),
                    "global using Doroti.Generated.Framework.Scheduler;\nglobal using Doroti.Generated.Framework.Services;\nglobal using Timer = Doroti.Flutter.Runtime.Timer;\n");
            }
            if (manifest.FrameworkMilestone is "G4-5" or "G5-3" or "G5-4")
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "G45.GlobalUsings.g.cs"),
                    "global using Doroti.Generated.Framework.Physics;\n" +
                    "global using Doroti.Generated.Framework.Animation;\n" +
                    "global using Doroti.Generated.Framework.Gestures;\n" +
                    "global using Path = Doroti.Flutter.Ui.Path;\n" +
                    "global using PointerEvent = Doroti.Generated.Framework.Gestures.PointerEvent;\n" +
                    "global using PointerDownEvent = Doroti.Generated.Framework.Gestures.PointerDownEvent;\n" +
                    "global using PointerEnterEvent = Doroti.Generated.Framework.Gestures.PointerEnterEvent;\n" +
                    "global using PointerExitEvent = Doroti.Generated.Framework.Gestures.PointerExitEvent;\n");
            }
            if (manifest.FrameworkMilestone is "G5-3" or "G5-4")
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "G53.GlobalUsings.g.cs"),
                    "global using Doroti.Generated.Framework.Painting;\n" +
                    "global using Doroti.Generated.Framework.Rendering;\n" +
                    "global using Doroti.Generated.Framework.Semantics;\n" +
                    "global using TextStyle = Doroti.Generated.Framework.Painting.TextStyle;\n" +
                    "global using StrutStyle = Doroti.Generated.Framework.Painting.StrutStyle;\n" +
                    "global using PointerUpEvent = Doroti.Generated.Framework.Gestures.PointerUpEvent;\n" +
                    "global using PointerHoverEvent = Doroti.Generated.Framework.Gestures.PointerHoverEvent;\n" +
                    "global using PointerCancelEvent = Doroti.Generated.Framework.Gestures.PointerCancelEvent;\n" +
                    "global using PointerMoveEvent = Doroti.Generated.Framework.Gestures.PointerMoveEvent;\n");
            }
            if (manifest.FrameworkMilestone == "G5-4")
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "G54.GlobalUsings.g.cs"),
                    "global using Doroti.Generated.Framework.Widgets;\n");
            }
            if (references.Length > 0)
            {
                ArtifactFiles.WriteUtf8(
                    Path.Combine(directory, "ProjectReferences.GlobalUsings.g.cs"),
                    string.Join('\n', references.OrderBy(item => item, StringComparer.Ordinal)
                        .Select(reference => $"global using {manifest.OutputNamespace}.{reference};")) + "\n");
            }
        }

        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Directory.Build.props"),
            """
            <Project>
              <PropertyGroup>
                <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
              </PropertyGroup>
            </Project>
            """ + "\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Directory.Packages.props"),
            """
            <Project>
              <PropertyGroup>
                <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
              </PropertyGroup>
            </Project>
            """ + "\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Doroti.Generated.Framework.slnx"),
            "<Solution>\n" + string.Join('\n', emittedPartitions.Select(partition =>
                $"  <Project Path=\"projects/{partition}/{manifest.OutputAssemblyName}.{partition}.csproj\" />")) + "\n</Solution>\n");

        var sccs = components.Select((component, index) => new FrameworkScc(
            $"scc-{index:D3}",
            component,
            component.Select(FrameworkPartition).Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            component.Length > 1 || edges.Any(edge => edge.From == component[0] && edge.To == component[0])))
            .ToArray();
        var unmerged = sccs.Where(item => item.IsCycle && item.Partitions.Length > 1).ToArray();
        foreach (var component in unmerged)
        {
            diagnostics.Add(Diagnostic(
                "DOTF0011", "error", "flutter", component.Libraries[0], string.Empty, 0, 1, null,
                $"Dependency SCC {component.Id} spans unmerged generated partitions: {string.Join(", ", component.Partitions)}.",
                "unmerged-library-cycle", "blocked", "Merge every library in the SCC into one generated project partition.",
                null, component.Libraries));
        }

        ArtifactFiles.WriteJson(
            Path.Combine(outputDirectory, "framework-project-graph.json"),
            new FrameworkProjectGraphDocument(
                "doroti.framework-project-graph/v1",
                manifest.FrameworkMilestone!,
                identity,
                CreateFrameworkCensus(manifest, manifestDirectory),
                libraries,
                edges,
                sccs,
                emittedPartitions.Select(partition => new FrameworkProjectPartition(
                    partition,
                    $"{manifest.OutputAssemblyName}.{partition}",
                    $"{manifest.OutputNamespace}.{partition}",
                    partitionDependencies.Where(item => item.From == partition).Select(item => item.To).ToArray(),
                    [
                        new("Doroti.Flutter.Runtime", identity.RuntimeBindingVersion, "project-when-sdk-root-package-otherwise"),
                        new("Doroti.Flutter.Ui", identity.RuntimeBindingVersion, "project-when-sdk-root-package-otherwise"),
                    ]))
                    .ToArray(),
                partitionDependencies,
                unmerged.Length == 0));
    }

    private static FrameworkSourceCensus CreateFrameworkCensus(SelectionManifest manifest, string manifestDirectory)
    {
        var seed = ResolveInputPath(manifest, manifestDirectory, manifest.Inputs[0].Path);
        DirectoryInfo? libraryRoot = new FileInfo(seed).Directory;
        while (libraryRoot is not null &&
               !(libraryRoot.Name == "lib" && libraryRoot.Parent?.Name == "flutter"))
        {
            libraryRoot = libraryRoot.Parent;
        }
        if (libraryRoot is null)
        {
            throw new InvalidDataException("Could not locate packages/flutter/lib from the framework selection.");
        }
        var files = Directory.EnumerateFiles(libraryRoot.FullName, "*.dart", SearchOption.AllDirectories)
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(libraryRoot.FullName, path)), StringComparer.Ordinal)
            .Select(path =>
            {
                var relative = ArtifactFiles.NormalizePath(Path.GetRelativePath(libraryRoot.FullName, path));
                var source = File.ReadAllText(path);
                var isPublicRoot = !relative.Contains('/', StringComparison.Ordinal);
                var isPart = Regex.IsMatch(source, @"(?m)^\s*part\s+of\s+", RegexOptions.CultureInvariant);
                return new FrameworkSourceFileNode(
                    relative,
                    "package:flutter/" + relative,
                    FrameworkPartition("package:flutter/" + relative),
                    $"{manifest.OutputNamespace}.{FrameworkPartition("package:flutter/" + relative)}",
                    isPublicRoot ? "public-root" : isPart ? "part" : "internal-library",
                    Regex.Matches(source, @"(?m)^\s*export\s+", RegexOptions.CultureInvariant).Count,
                    Regex.Matches(source, "(?m)^\\s*part\\s+['\"]", RegexOptions.CultureInvariant).Count,
                    Regex.IsMatch(source, @"(?m)^\s*extension(?:\s+type)?\s+", RegexOptions.CultureInvariant));
            })
            .ToArray();
        return new(
            "doroti.flutter-source-census-graph/v1",
            files.Length,
            files.Count(item => item.Role == "public-root"),
            files.Where(item => item.Role == "public-root").Sum(item => item.ExportDirectiveCount),
            files.Count(item => item.Role == "part"),
            files);
    }

    private static string[][] ComputeStronglyConnectedComponents(
        HashSet<string> libraries,
        FrameworkLibraryEdge[] edges)
    {
        var adjacency = libraries.ToDictionary(
            item => item,
            item => edges.Where(edge => edge.From == item).Select(edge => edge.To).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
            StringComparer.Ordinal);
        var index = 0;
        var indices = new Dictionary<string, int>(StringComparer.Ordinal);
        var lowLinks = new Dictionary<string, int>(StringComparer.Ordinal);
        var stack = new Stack<string>();
        var onStack = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<string[]>();

        void Visit(string library)
        {
            indices[library] = index;
            lowLinks[library] = index++;
            stack.Push(library);
            onStack.Add(library);
            foreach (var dependency in adjacency[library])
            {
                if (!indices.ContainsKey(dependency))
                {
                    Visit(dependency);
                    lowLinks[library] = Math.Min(lowLinks[library], lowLinks[dependency]);
                }
                else if (onStack.Contains(dependency))
                {
                    lowLinks[library] = Math.Min(lowLinks[library], indices[dependency]);
                }
            }
            if (lowLinks[library] != indices[library])
            {
                return;
            }
            var component = new List<string>();
            string current;
            do
            {
                current = stack.Pop();
                onStack.Remove(current);
                component.Add(current);
            }
            while (current != library);
            result.Add(component.OrderBy(item => item, StringComparer.Ordinal).ToArray());
        }

        foreach (var library in libraries.OrderBy(item => item, StringComparer.Ordinal))
        {
            if (!indices.ContainsKey(library))
            {
                Visit(library);
            }
        }
        return result.OrderBy(item => item[0], StringComparer.Ordinal).ToArray();
    }

    private static string BuildPromotedFrameworkReferences(string? milestone, string runtimeBindingVersion, bool includeWidgets = false)
    {
        if (milestone is not ("G4-3" or "G4-4" or "G4-5" or "G5-3" or "G5-4"))
        {
            return string.Empty;
        }

        var packages = new List<string> { "Foundation" };
        if (milestone is "G4-4" or "G4-5" or "G5-3" or "G5-4")
        {
            packages.Add("Scheduler");
            packages.Add("Services");
        }
        if (milestone is "G4-5" or "G5-3" or "G5-4")
        {
            packages.Add("Physics");
            packages.Add("Animation");
            packages.Add("Gestures");
        }
        if (milestone is "G5-3" or "G5-4")
        {
            packages.Add("Painting");
            packages.Add("Semantics");
            packages.Add("Rendering");
            if (includeWidgets)
            {
                packages.Add("Widgets");
            }
        }

        return string.Join('\n', packages.Select(package =>
            $"    <ProjectReference Include=\"$(DorotiRepositoryRoot)\\src\\Doroti.Flutter.Framework.{package}\\Doroti.Flutter.Framework.{package}.csproj\" Condition=\"'$(DorotiRepositoryRoot)' != ''\" />\n" +
            $"    <PackageReference Include=\"Doroti.Flutter.Framework.{package}\" Version=\"[{runtimeBindingVersion}]\" Condition=\"'$(DorotiRepositoryRoot)' == ''\" />"));
    }

    private static string PascalCase(string value) => string.Concat(
        value.Split(['_', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Select(item => char.ToUpperInvariant(item[0]) + item[1..]));
}

internal sealed record FrameworkLibraryNode(
    string Library,
    string Partition,
    string Namespace,
    int SccIndex,
    string[] SelectedSymbols,
    string[] DeclarationElementIds,
    FrameworkFragmentNode[] Fragments,
    FrameworkImportNode[] Imports,
    string[] AccessibleExtensions);
internal sealed record FrameworkFragmentNode(string Uri, string OwnerLibrary, bool IsDefining, string[] DeclarationElementIds);
internal sealed record FrameworkImportNode(string Uri, string? Prefix, bool IsSynthetic);
internal sealed record FrameworkLibraryEdge(string From, string To);
internal sealed record FrameworkScc(string Id, string[] Libraries, string[] Partitions, bool IsCycle);
internal sealed record FrameworkProjectReference(string From, string To);
internal sealed record FrameworkPackageReference(string Package, string Version, string Mode);
internal sealed record FrameworkProjectPartition(
    string Id,
    string AssemblyName,
    string Namespace,
    string[] ProjectReferences,
    FrameworkPackageReference[] PackageReferences);
internal sealed record FrameworkProjectGraphDocument(
    string SchemaVersion,
    string Milestone,
    CompilerIdentity Identity,
    FrameworkSourceCensus SourceCensus,
    FrameworkLibraryNode[] Libraries,
    FrameworkLibraryEdge[] Edges,
    FrameworkScc[] Sccs,
    FrameworkProjectPartition[] Partitions,
    FrameworkProjectReference[] ProjectReferences,
    bool EveryCycleMerged);
internal sealed record FrameworkSourceCensus(
    string SchemaVersion,
    int DartFileCount,
    int PublicRootCount,
    int PublicRootExportDirectiveCount,
    int PartFileCount,
    FrameworkSourceFileNode[] Files);
internal sealed record FrameworkSourceFileNode(
    string Path,
    string Library,
    string Partition,
    string Namespace,
    string Role,
    int ExportDirectiveCount,
    int PartDirectiveCount,
    bool DeclaresExtension);
