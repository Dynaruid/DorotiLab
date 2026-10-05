using System.Diagnostics;
using System.Text.Json;
using Doroti.Tooling.Contracts;
namespace Doroti.Tooling;

/// <summary>Read-only resolved NuGet/MSBuild metadata. No extension activation or native build.</summary>
internal static class PlatformMetadata
{
    private static JsonDocument Assets(string project)
    {
        var path = Path.Combine(Path.GetDirectoryName(project)!, "obj", "project.assets.json");
        if (!File.Exists(path)) throw new ToolContractException("restore-required", $"Restore {project} before resolving installed provider metadata.");
        return JsonDocument.Parse(File.ReadAllBytes(path));
    }
    public static (string Path, string Version) PackageFile(string project, string package, string relative)
    {
        using var assets = Assets(project);
        var matches = assets.RootElement.GetProperty("libraries").EnumerateObject()
            .Where(value => value.Name.Split('/')[0].Equals(package, StringComparison.OrdinalIgnoreCase) && value.Value.GetProperty("type").GetString() == "package").ToArray();
        if (matches.Length != 1) throw new ToolContractException("unresolved-provider-package", package);
        var library = matches[0];
        var version = library.Name.Split('/')[1];
        foreach (var folder in assets.RootElement.GetProperty("packageFolders").EnumerateObject())
        {
            var path = Path.GetFullPath(Path.Combine(folder.Name, library.Value.GetProperty("path").GetString()!, relative));
            if (File.Exists(path)) return (path, version);
        }
        throw new ToolContractException("missing-provider-asset", $"{package}/{version}/{relative}");
    }
    public static string ProviderManifest(string project, string declaration, string workspace)
    {
        if (!declaration.StartsWith("nuget:", StringComparison.Ordinal)) return Path.GetFullPath(declaration, workspace);
        var package = declaration[6..];
        if (package.Length == 0 || package.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not '.' and not '-' and not '_'))
            throw new ToolContractException("invalid-provider-package", declaration);
        return PackageFile(project, package, "lib/net10.0/doroti-provider.json").Path;
    }
    public static void ValidateResolvedTarget(string project, WorkspacePlatform platform, ProviderDescriptor provider)
    {
        if (!platform.ProviderManifest.StartsWith("nuget:", StringComparison.Ordinal)) return;
        var tool = PackageFile(project, platform.ProviderManifest[6..], "lib/net10.0/doroti-provider.json");
        if (tool.Version != provider.Version) throw new ToolContractException("resolved-provider-mismatch", "Provider descriptor version differs from the restored tool package.");
        var target = PackageFile(project, platform.TargetPackage, "doroti/doroti-target-manifest.json");
        using var manifest = JsonDocument.Parse(File.ReadAllBytes(target.Path));
        var root = manifest.RootElement;
        bool Match(string name, string expected) => root.TryGetProperty(name, out var value) && value.GetString() == expected;
        if (!Match("schemaVersion", "doroti.target-package/v2") || !Match("packageId", platform.TargetPackage) || !Match("packageVersion", target.Version) || !Match("providerVersion", provider.Version) ||
            !Match("provider", platform.Provider) || !Match("providerProfile", platform.Profile) || !Match("targetFramework", platform.TargetFramework) || !Match("rid", platform.RuntimeIdentifier) ||
            !root.TryGetProperty("protocolVersion", out var protocol) || protocol.GetInt32() != provider.ProtocolVersion || !Match("coreRange", provider.CoreRange))
            throw new ToolContractException("resolved-target-mismatch", $"Workspace declaration does not match restored {platform.TargetPackage}/{target.Version}.");
    }
    public static async Task ValidateRunnerAsync(string executable, string project, WorkspacePlatform platform, ProviderDescriptor provider, CancellationToken token)
    {
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true, WorkingDirectory = Path.GetDirectoryName(project)! };
        foreach (var arg in new[] { "msbuild", project, "-nologo", "-target:GetDorotiPlatformContract", "-getTargetResult:GetDorotiPlatformContract", "-p:DesignTimeBuild=true", "-p:BuildProjectReferences=false" }) start.ArgumentList.Add(arg);
        using var process = Process.Start(start) ?? throw new ToolContractException("metadata-start-failed", executable);
        async Task<string> ReadAsync(StreamReader reader)
        {
            var text = new System.Text.StringBuilder(); var buffer = new char[4096];
            while (await reader.ReadAsync(buffer.AsMemory(), token) is var count && count != 0)
            {
                if (text.Length + count > 4 * 1024 * 1024) throw new ToolContractException("metadata-too-large", project);
                text.Append(buffer, 0, count);
            }
            return text.ToString();
        }
        var output = ReadAsync(process.StandardOutput); var errors = ReadAsync(process.StandardError);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token); timeout.CancelAfter(TimeSpan.FromSeconds(30));
        try
        {
            var exit = process.WaitForExitAsync(timeout.Token);
            var first = await Task.WhenAny(exit, output, errors);
            if (!ReferenceEquals(first, exit)) await first; // A failed bounded reader must terminate the child immediately.
            if (ReferenceEquals(first, output))
            {
                var next = await Task.WhenAny(exit, errors);
                if (!ReferenceEquals(next, exit)) await next;
            }
            else if (ReferenceEquals(first, errors))
            {
                var next = await Task.WhenAny(exit, output);
                if (!ReferenceEquals(next, exit)) await next;
            }
            await exit;
            var result = await output; var error = await errors;
            if (process.ExitCode != 0) throw new ToolContractException("runner-contract-failed", result + error);
            using var json = JsonDocument.Parse(result);
            var items = json.RootElement.GetProperty("TargetResults").GetProperty("GetDorotiPlatformContract").GetProperty("Items");
            if (items.GetArrayLength() != 1) throw new ToolContractException("ambiguous-runner-contract", project);
            var contract = items[0];
            foreach (var pair in new[] { ("Provider", platform.Provider), ("ProviderVersion", provider.Version), ("CoreRange", provider.CoreRange), ("ProtocolVersion", provider.ProtocolVersion.ToString(System.Globalization.CultureInfo.InvariantCulture)), ("Profile", platform.Profile), ("TargetId", platform.Target), ("TargetPackage", platform.TargetPackage), ("Backend", platform.Backend), ("TargetFramework", platform.TargetFramework), ("RuntimeIdentifier", platform.RuntimeIdentifier) })
                if (!contract.TryGetProperty(pair.Item1, out var value) || value.GetString() != pair.Item2) throw new ToolContractException("runner-contract-mismatch", $"{project}: {pair.Item1}");
        }
        finally
        {
            if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); }
            try { await Task.WhenAll(output, errors); } catch { /* The original metadata error remains authoritative. */ }
        }
    }
}
