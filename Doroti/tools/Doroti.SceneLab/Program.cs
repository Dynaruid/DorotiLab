using Doroti.SceneLab;
using Doroti.Tooling;

try
{
    if (args.Length > 0 && args[0] == "approve")
    {
        var actual = RequiredOption(args, "--actual");
        var golden = RequiredOption(args, "--golden");
        if (!args.Contains("--accept", StringComparer.Ordinal))
        {
            throw new ArgumentException("Golden approval requires the explicit --accept flag after reviewing diff.png.");
        }
        Directory.CreateDirectory(Path.GetDirectoryName(golden)!);
        File.Copy(actual, golden, overwrite: true);
        Console.WriteLine($"Approved golden: {golden}");
        return 0;
    }

    var root = RepositoryPaths.FindRoot(Environment.CurrentDirectory);
    var scene = ReadOption(args, "--scene") ?? Path.Combine(root, "migration", "scenes", "solid.json");
    var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "scenes", "solid-r1");
    var actualColor = ReadOption(args, "--actual-color", makeFullPath: false);
    var result = SceneRunner.Render(scene, output, actualColor);
    Console.WriteLine($"SceneLab {result.Scene}: {(result.Matches ? "PASS" : "FAIL")} ({result.MismatchedPixels} mismatched pixels)");
    return result.Matches ? 0 : 2;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static string RequiredOption(string[] arguments, string name) => ReadOption(arguments, name) ?? throw new ArgumentException($"Missing required option: {name}");

static string? ReadOption(string[] arguments, string name, bool makeFullPath = true)
{
    var index = Array.IndexOf(arguments, name);
    if (index < 0 || index + 1 >= arguments.Length)
    {
        return null;
    }
    return makeFullPath ? Path.GetFullPath(arguments[index + 1]) : arguments[index + 1];
}
