using System.Text.Json;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal static class AnalyzerProtocolReader
{
    public static AnalyzerOutput Read(string path, string json)
    {
        var output = JsonSerializer.Deserialize<AnalyzerOutput>(json, ArtifactFiles.JsonOptions)
            ?? throw new InvalidDataException($"Dart analyzer returned empty JSON for {path}.");
        if (output.SchemaVersion is not ("doroti.dart-analyzer-output/v2" or "doroti.dart-analyzer-output/v3"))
        {
            throw new InvalidDataException($"Unsupported Dart analyzer schema: {output.SchemaVersion}");
        }

        return output;
    }
}
