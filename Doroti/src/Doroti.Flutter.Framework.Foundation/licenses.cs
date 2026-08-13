// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/licenses.dart
namespace Doroti.Generated.Framework.Foundation;

public delegate Doroti.Flutter.Runtime.Stream<LicenseEntry> LicenseEntryCollector();

public abstract class LicenseEntry
{
    public abstract IReadOnlyList<string> packages { get; }
    public abstract IEnumerable<LicenseParagraph> paragraphs { get; }
}

public sealed record LicenseParagraph(string text, int indent)
{
    public const int centeredIndent = -1;
}

public sealed class LicenseEntryWithLineBreaks : LicenseEntry
{
    private readonly IReadOnlyList<string> _packages;
    private readonly string _text;

    public LicenseEntryWithLineBreaks(IEnumerable<string> packages, string text)
    {
        _packages = [.. packages ?? throw new ArgumentNullException(nameof(packages))];
        _text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public override IReadOnlyList<string> packages => _packages;

    public override IEnumerable<LicenseParagraph> paragraphs => Parse(_text);

    private static IEnumerable<LicenseParagraph> Parse(string text)
    {
        foreach (var paragraph in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split("\n\n", StringSplitOptions.None))
        {
            var lines = paragraph.Split('\n');
            var nonBlank = lines.Where(line => line.Length > 0).ToArray();
            if (nonBlank.Length == 0)
            {
                continue;
            }
            var indent = nonBlank.Min(line => line.TakeWhile(char.IsWhiteSpace).Count());
            yield return new LicenseParagraph(string.Join("\n", lines.Select(line => line.Length >= indent ? line[indent..] : string.Empty)), indent);
        }
    }
}

public static class LicenseRegistry
{
    private static readonly object Gate = new();
    private static readonly List<LicenseEntryCollector> Collectors = [];

    public static void addLicense(LicenseEntryCollector collector)
    {
        ArgumentNullException.ThrowIfNull(collector);
        lock (Gate)
        {
            Collectors.Add(collector);
        }
    }

    public static void reset()
    {
        lock (Gate)
        {
            Collectors.Clear();
        }
    }

    public static async IAsyncEnumerable<LicenseEntry> licenses()
    {
        LicenseEntryCollector[] snapshot;
        lock (Gate)
        {
            snapshot = [.. Collectors];
        }
        foreach (var collector in snapshot)
        {
            await foreach (var entry in collector().ConfigureAwait(false))
            {
                yield return entry;
            }
        }
    }
}

internal enum _LicenseEntryWithLineBreaksParserState { beforeParagraph, inParagraph }
