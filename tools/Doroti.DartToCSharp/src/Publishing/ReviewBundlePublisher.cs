using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class ReviewBundlePublisher
{
    public static void Publish(string outputDirectory, Action<string> write)
    {
        var target = Path.GetFullPath(outputDirectory);
        var parent = Path.GetDirectoryName(target)
            ?? throw new PortContractException("DORPORT003", "Review output must have a parent directory.");
        if (Path.GetPathRoot(target) == target)
        {
            throw new PortContractException("DORPORT003", "A filesystem root cannot be used as review output.");
        }
        Directory.CreateDirectory(parent);
        var staging = Path.Combine(parent, $".{Path.GetFileName(target)}.doroti-review-{Guid.NewGuid():N}");
        try
        {
            Directory.CreateDirectory(staging);
            write(staging);
            if (Directory.Exists(target))
            {
                EnsureEqual(staging, target);
                return;
            }
            Directory.Move(staging, target);
        }
        finally
        {
            if (Directory.Exists(staging))
            {
                Directory.Delete(staging, recursive: true);
            }
        }
    }

    private static void EnsureEqual(string expectedRoot, string actualRoot)
    {
        var expected = Inventory(expectedRoot);
        var actual = Inventory(actualRoot);
        if (!expected.Keys.SequenceEqual(actual.Keys, StringComparer.Ordinal) ||
            expected.Any(item => !string.Equals(item.Value, actual[item.Key], StringComparison.Ordinal)))
        {
            throw new PortContractException("DORPORT004", $"Existing review bundle differs from deterministic output: {actualRoot}");
        }
    }

    private static SortedDictionary<string, string> Inventory(string root) => new(
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .OrderBy(path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)), StringComparer.Ordinal)
            .ToDictionary(
                path => ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)),
                ArtifactFiles.Sha256,
                StringComparer.Ordinal),
        StringComparer.Ordinal);
}
