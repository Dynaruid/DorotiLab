using System.Text.Json;

namespace Doroti.DartToCSharp;

internal static class CompilerInputResolver
{
    public static string Resolve(SelectionManifest manifest, string manifestDirectory, string logicalPath)
    {
        if (!logicalPath.StartsWith("package:", StringComparison.Ordinal))
        {
            return Path.GetFullPath(logicalPath, manifestDirectory);
        }

        if (string.IsNullOrWhiteSpace(manifest.PackageRoot))
        {
            throw new InvalidDataException($"Package URI input requires packageRoot: {logicalPath}");
        }

        var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
        var configPath = Path.Combine(packageRoot, ".dart_tool", "package_config.json");
        if (!File.Exists(configPath))
        {
            throw new InvalidDataException($"Package configuration is missing. Run pub get first: {configPath}");
        }

        var slash = logicalPath.IndexOf('/', "package:".Length);
        if (slash < 0 || slash == logicalPath.Length - 1)
        {
            throw new InvalidDataException($"Invalid package URI input: {logicalPath}");
        }

        var packageName = logicalPath["package:".Length..slash];
        var packageRelativePath = logicalPath[(slash + 1)..];
        using var config = JsonDocument.Parse(File.ReadAllText(configPath));
        var package = config.RootElement.GetProperty("packages").EnumerateArray()
            .SingleOrDefault(item => string.Equals(item.GetProperty("name").GetString(), packageName, StringComparison.Ordinal));
        if (package.ValueKind == JsonValueKind.Undefined)
        {
            throw new InvalidDataException($"Package URI is not present in the resolved lock graph: {logicalPath}");
        }

        var configDirectoryUri = new Uri(Path.GetFullPath(configPath));
        var rootValue = package.GetProperty("rootUri").GetString()!;
        var rootUri = new Uri(configDirectoryUri, rootValue.EndsWith("/", StringComparison.Ordinal) ? rootValue : rootValue + "/");
        var packageUri = package.TryGetProperty("packageUri", out var packageUriElement)
            ? packageUriElement.GetString() ?? "lib/"
            : "lib/";
        var fileUri = new Uri(rootUri, packageUri + packageRelativePath.Replace('\\', '/'));
        var resolved = fileUri.LocalPath;
        if (!File.Exists(resolved))
        {
            throw new FileNotFoundException($"Resolved package URI does not exist: {logicalPath}", resolved);
        }

        return resolved;
    }
}
