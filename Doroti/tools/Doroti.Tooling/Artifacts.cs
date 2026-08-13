using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doroti.Tooling;

public static class ArtifactFiles
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // Resolved Flutter expressions can exceed System.Text.Json's default
        // depth of 64 (for example deeply nested layer-tree transforms).
        MaxDepth = 256,
    };

    public static void WriteJson<T>(string path, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions).Replace("\r\n", "\n", StringComparison.Ordinal);
        WriteUtf8(path, json + "\n");
    }

    public static T ReadJson<T>(string path)
    {
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream, JsonOptions)
            ?? throw new InvalidDataException($"JSON document is empty: {path}");
    }

    public static void WriteUtf8(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, content.Replace("\r\n", "\n", StringComparison.Ordinal), new UTF8Encoding(false));
    }

    public static string Sha256(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    public static string NormalizePath(string path) => path.Replace('\\', '/');
}

public static class ProcessRunner
{
    public static ProcessResult Run(
        string fileName,
        IEnumerable<string> arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string?>? environment = null)
    {
        var startInfo = new ProcessStartInfo(ResolveExecutable(fileName))
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }
        if (environment is not null)
        {
            foreach (var (name, value) in environment)
            {
                if (value is null)
                {
                    startInfo.Environment.Remove(name);
                }
                else
                {
                    startInfo.Environment[name] = value;
                }
            }
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Could not start {fileName}.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, stdout.Trim(), stderr.Trim());
    }

    private static string ResolveExecutable(string fileName)
    {
        if (!OperatingSystem.IsWindows() || Path.IsPathRooted(fileName) || Path.HasExtension(fileName))
        {
            return fileName;
        }

        var extensions = (Environment.GetEnvironmentVariable("PATHEXT") ?? ".EXE;.CMD;.BAT")
            .Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory.Trim('"'), fileName + extension.ToLowerInvariant());
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        return fileName;
    }
}

public sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public void EnsureSuccess(string operation)
    {
        if (ExitCode != 0)
        {
            throw new InvalidOperationException($"{operation} failed ({ExitCode}).\n{StandardError}\n{StandardOutput}".Trim());
        }
    }
}

public static class RepositoryPaths
{
    public static string FindRoot(string startDirectory)
    {
        for (var directory = new DirectoryInfo(Path.GetFullPath(startDirectory)); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException($"Could not find Doroti.slnx from {startDirectory}.");
    }

    public static string ResolveWithin(string root, string relativePath)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(relativePath, root);
        if (!fullPath.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fullPath.TrimEnd(Path.DirectorySeparatorChar), fullRoot.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Path escapes the allowed root: {relativePath}");
        }

        return fullPath;
    }
}

public static class RepositoryLocalStorage
{
    public const string EnvironmentVariable = "DOROTI_LOCAL_ROOT";

    public static string FindDorotiRoot(string startDirectory)
    {
        for (var directory = new DirectoryInfo(Path.GetFullPath(startDirectory)); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")))
            {
                return directory.FullName;
            }

            var nested = Path.Combine(directory.FullName, "Doroti");
            if (File.Exists(Path.Combine(nested, "Doroti.slnx")))
            {
                return nested;
            }
        }

        throw new DirectoryNotFoundException($"Could not find the Doroti workspace from {startDirectory}.");
    }

    public static string ResolveRoot(string dorotiRoot)
    {
        var resolvedDorotiRoot = Path.GetFullPath(dorotiRoot).TrimEnd(Path.DirectorySeparatorChar);
        var workspaceRoot = Directory.Exists(Path.Combine(Path.GetDirectoryName(resolvedDorotiRoot)!, "tools", "Doroti.DartToCSharp"))
            ? Path.GetDirectoryName(resolvedDorotiRoot)!
            : resolvedDorotiRoot;
        var configured = Environment.GetEnvironmentVariable(EnvironmentVariable);
        var root = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(workspaceRoot, ".doroti")
            : Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(workspaceRoot, configured);
        root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);
        Directory.CreateDirectory(root);
        return root;
    }

    public static string GetTemporaryRoot(string dorotiRoot)
    {
        var root = Path.Combine(ResolveRoot(dorotiRoot), "tmp");
        Directory.CreateDirectory(root);
        return root;
    }

    public static string GetCacheRoot(string dorotiRoot)
    {
        var root = Path.Combine(ResolveRoot(dorotiRoot), "cache");
        Directory.CreateDirectory(root);
        return root;
    }

    public static string CreateTemporaryDirectory(string dorotiRoot, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Any(character =>
                !char.IsAsciiLetterOrDigit(character) && character is not '-' and not '_' and not '.'))
        {
            throw new ArgumentException($"Invalid Doroti temporary directory name: {name}", nameof(name));
        }

        var path = Path.Combine(GetTemporaryRoot(dorotiRoot), $"{name}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        return path;
    }

    public static void DeleteTemporaryDirectory(string dorotiRoot, string path)
    {
        var temporaryRoot = Path.GetFullPath(GetTemporaryRoot(dorotiRoot)).TrimEnd(Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        var resolved = Path.GetFullPath(path);
        if (!resolved.StartsWith(temporaryRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Refusing to clean a path outside the Doroti temporary root: {resolved}");
        }
        for (var attempt = 1; attempt <= 5; attempt++)
        {
            try
            {
                if (Directory.Exists(resolved))
                {
                    Directory.Delete(resolved, recursive: true);
                }
                return;
            }
            catch (Exception exception) when (attempt < 5 && exception is IOException or UnauthorizedAccessException)
            {
                Thread.Sleep(200);
            }
        }
    }
}
