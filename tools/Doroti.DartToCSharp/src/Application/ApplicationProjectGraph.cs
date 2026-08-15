using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static partial class ConverterEngine
{
    private sealed record ResourceSourceDocument(string SchemaVersion, ResourceSourceEntry[] Resources);
    private sealed record ResourceSourceEntry(string Key, string Path, string Kind, string? FontFamily = null, string? Locale = null);
    private sealed record PluginSourceDocument(string SchemaVersion, PluginSourceEntry[] Plugins);
    private sealed record PluginSourceEntry(string Id, string Channel, string Codec, string CapabilityId, NativePackageSource[] NativePackages);
    private sealed record NativePackageSource(string Rid, string PackageId, string Version, string AbiVersion, string HandlerType);

    private static void WriteApplicationProjectGraph(
        string outputDirectory,
        SelectionManifest manifest,
        string manifestDirectory,
        CompilerIdentity identity,
        List<MigrationIrInput> inputs,
        List<ConverterOutput> outputs,
        List<ConverterDiagnostic> diagnostics,
        ApplicationGraphPlan plan)
    {
        var application = manifest.Application!;
        var resources = ArtifactFiles.ReadJson<ResourceSourceDocument>(plan.ResourceManifestPath);
        var plugins = ArtifactFiles.ReadJson<PluginSourceDocument>(plan.PluginManifestPath);
        if (resources.SchemaVersion != "doroti.application-resources/v1")
            throw new InvalidDataException($"Unsupported application resource schema: {resources.SchemaVersion}");
        if (plugins.SchemaVersion != "doroti.application-plugins/v1")
            throw new InvalidDataException($"Unsupported application plugin schema: {plugins.SchemaVersion}");
        if (resources.Resources.Select(item => item.Key).Distinct(StringComparer.Ordinal).Count() != resources.Resources.Length)
            throw new InvalidDataException("Application resource keys must be unique.");
        if (plugins.Plugins.Select(item => item.Channel).Distinct(StringComparer.Ordinal).Count() != plugins.Plugins.Length)
            throw new InvalidDataException("Application plugin channels must be unique.");

        var packageRoot = Path.GetFullPath(manifest.PackageRoot!, manifestDirectory);
        var projectDirectory = Path.Combine(outputDirectory, "projects", "Framework");
        Directory.CreateDirectory(projectDirectory);
        var embeddedResources = new List<object>();
        var embeddedItems = new List<string>();
        for (var index = 0; index < resources.Resources.Length; index++)
        {
            var resource = resources.Resources[index];
            var source = ResolveApplicationResource(packageRoot, resource.Path);
            var relative = $"resources/{index:D4}-{Path.GetFileName(source)}";
            var destination = Path.Combine(projectDirectory, relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(source, destination, overwrite: true);
            var embeddedName = $"Doroti.Application.Resource.{index:D4}";
            embeddedItems.Add($"    <EmbeddedResource Include=\"{relative}\" LogicalName=\"{embeddedName}\" />");
            embeddedResources.Add(new
            {
                resource.Key,
                resource.Kind,
                resource.FontFamily,
                resource.Locale,
                embeddedResourceName = embeddedName,
                sha256 = ArtifactFiles.Sha256(source),
                length = new FileInfo(source).Length,
            });
        }

        var pluginEntries = new List<object>();
        foreach (var plugin in plugins.Plugins.OrderBy(item => item.Channel, StringComparer.Ordinal))
        {
            var native = plugin.NativePackages.SingleOrDefault(item => item.Rid == application.TargetRid);
            if (native is null)
            {
                diagnostics.Add(Diagnostic(
                    "DOTAPP005", "error", "application", manifest.EntryPoint!, plan.PluginManifestPath, 0, 1, plugin.Id,
                    $"Plugin '{plugin.Id}' has no native handler package for RID '{application.TargetRid}'.",
                    "unsupported-plugin-rid", "blocked",
                    "Register an explicit RID package and ABI handler; plugins never silently succeed."));
            }
            pluginEntries.Add(new
            {
                plugin.Id,
                plugin.Channel,
                plugin.Codec,
                plugin.CapabilityId,
                nativePackage = native,
            });
        }

        var runtimeManifest = new
        {
            schemaVersion = "doroti.application-capabilities/v1",
            applicationId = manifest.OutputAssemblyName,
            targetRid = application.TargetRid,
            resources = embeddedResources,
            plugins = pluginEntries,
        };
        var runtimeManifestPath = Path.Combine(projectDirectory, "doroti.application-capabilities.json");
        ArtifactFiles.WriteJson(runtimeManifestPath, runtimeManifest);
        embeddedItems.Add("    <EmbeddedResource Include=\"doroti.application-capabilities.json\" LogicalName=\"Doroti.Application.Manifest\" />");

        var frameworkReferences = BuildApplicationFrameworkReferences(application.FrameworkPackages, identity.RuntimeBindingVersion);
        var hostReference = BuildApplicationPackageReference(application.HostBootstrapPackage, identity.RuntimeBindingVersion);
        var assemblyName = manifest.OutputAssemblyName + ".Framework";
        ArtifactFiles.WriteUtf8(
            Path.Combine(projectDirectory, assemblyName + ".csproj"),
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
            {frameworkReferences}
            {hostReference}
            {string.Join('\n', embeddedItems)}
              </ItemGroup>
            </Project>
            """ + "\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(projectDirectory, "Application.GlobalUsings.g.cs"),
            string.Join('\n', new[]
            {
                "Foundation", "Scheduler", "Services", "Physics", "Animation", "Gestures",
                "Painting", "Semantics", "Rendering", "Widgets",
            }.Concat(application.FrameworkPackages).Distinct(StringComparer.Ordinal).Select(package =>
                $"global using Doroti.Generated.Framework.{package};")) + "\n" +
            "global using Doroti.Flutter.Hosting;\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Directory.Build.props"),
            "<Project>\n  <PropertyGroup>\n    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>\n    <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>\n  </PropertyGroup>\n</Project>\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Directory.Packages.props"),
            "<Project>\n  <PropertyGroup>\n    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>\n  </PropertyGroup>\n</Project>\n");
        var solution = $"<Solution>\n  <Project Path=\"projects/Framework/{assemblyName}.csproj\" />\n</Solution>\n";
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "Doroti.Generated.Application.slnx"), solution);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "Doroti.Generated.Framework.slnx"), solution);

        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "application-graph.json"), new
        {
            schemaVersion = "doroti.application-graph/v1",
            entryPoint = plan.EntryPoint,
            targetRid = application.TargetRid,
            frameworkPackages = application.FrameworkPackages.Order(StringComparer.Ordinal).ToArray(),
            hostBootstrapPackage = application.HostBootstrapPackage,
            contractSha256 = plan.ContractSha256,
            libraries = plan.Libraries.Select(item => new
            {
                item.Library,
                path = ArtifactFiles.NormalizePath(Path.GetRelativePath(packageRoot, item.Path)),
                item.Sha256,
            }).ToArray(),
            edges = plan.Edges,
            sccs = plan.Sccs,
            incremental = new
            {
                changedAndDependentSccLibraries = plan.AffectedLibraries,
                regeneratedOutputs = outputs.Where(output => plan.AffectedLibraries.Contains(
                    inputs.Single(input => input.Path == output.Input).Library,
                    StringComparer.Ordinal)).Select(output => output.Output).Order(StringComparer.Ordinal).ToArray(),
                reusedOutputs = outputs.Where(output => !plan.AffectedLibraries.Contains(
                    inputs.Single(input => input.Path == output.Input).Library,
                    StringComparer.Ordinal)).Select(output => output.Output).Order(StringComparer.Ordinal).ToArray(),
            },
            resources = embeddedResources,
            plugins = pluginEntries,
            directReferences = application.FrameworkPackages.Select(package => $"Doroti.Flutter.Framework.{package}")
                .Append(application.HostBootstrapPackage).Order(StringComparer.Ordinal).ToArray(),
        });
    }

    private static string ResolveApplicationResource(string packageRoot, string relative)
    {
        if (Path.IsPathRooted(relative)) throw new InvalidDataException("Resource paths must be package-relative.");
        var prefix = packageRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var path = Path.GetFullPath(relative, packageRoot);
        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            throw new InvalidDataException($"Application resource is missing or escapes packageRoot: {relative}");
        return path;
    }

    private static string BuildApplicationFrameworkReferences(string[] packages, string version)
    {
        var allowed = new HashSet<string>(["Foundation", "Scheduler", "Services", "Physics", "Animation", "Gestures", "Painting", "Semantics", "Rendering", "Widgets", "Material", "Cupertino"], StringComparer.Ordinal);
        if (packages.Length == 0 || packages.Any(package => !allowed.Contains(package)))
            throw new InvalidDataException("Application frameworkPackages contains an unsupported or empty package selection.");
        return string.Join('\n', packages.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).Select(package =>
            $"    <PackageReference Include=\"Doroti.Flutter.Framework.{package}\" Version=\"[{version}]\" />"));
    }

    private static string BuildApplicationPackageReference(string package, string version)
    {
        if (package != "Doroti.Flutter.Hosting")
            throw new InvalidDataException($"Unsupported host bootstrap package: {package}");
        return $"    <PackageReference Include=\"{package}\" Version=\"[{version}]\" />";
    }
}
