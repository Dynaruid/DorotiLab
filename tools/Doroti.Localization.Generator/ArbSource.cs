using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Doroti.Localization.Generator;

internal sealed record SourceProblem(Location Location, string Message);

internal sealed class ArbSource
{
    internal string Path { get; }
    internal SourceText Text { get; }
    internal Dictionary<string, JsonElement> Properties { get; } = new(StringComparer.Ordinal);
    internal List<SourceProblem> Problems { get; } = [];
    private readonly Dictionary<string, int> _positions = new(StringComparer.Ordinal);

    private ArbSource(string path, SourceText text) => (Path, Text) = (path, text);

    internal static ArbSource Read(AdditionalText file, CancellationToken cancellationToken)
    {
        var text = file.GetText(cancellationToken);
        var source = new ArbSource(file.Path, text ?? SourceText.From(""));
        if (text is null)
        {
            source.Error(null, "Could not read the ARB file.");
            return source;
        }
        var bytes = Encoding.UTF8.GetBytes(text.ToString());
        try
        {
            var reader = new Utf8JsonReader(bytes);
            var objects = new Stack<HashSet<string>>();
            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject: objects.Push(new(StringComparer.Ordinal)); break;
                    case JsonTokenType.EndObject: objects.Pop(); break;
                    case JsonTokenType.PropertyName:
                        var name = reader.GetString()!;
                        var offset = checked((int)reader.TokenStartIndex);
                        if (reader.CurrentDepth == 1) source._positions.TryAdd(name, offset);
                        if (!objects.Peek().Add(name))
                            source.ErrorAt(Encoding.UTF8.GetCharCount(bytes.AsSpan(0, offset)), $"Duplicate JSON key '{name}'.");
                        break;
                }
            }
            if (source.Problems.Count != 0) return source;

            using var document = JsonDocument.Parse(bytes);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                source.Error(null, "An ARB file must contain a JSON object.");
            else
                foreach (var property in document.RootElement.EnumerateObject())
                    source.Properties.Add(property.Name, property.Value.Clone());
        }
        catch (JsonException error)
        {
            var line = text.Lines[(int)Math.Clamp(error.LineNumber ?? 0, 0, text.Lines.Count - 1)];
            var lineBytes = Encoding.UTF8.GetBytes(text.ToString(line.Span));
            var position = Math.Clamp(error.BytePositionInLine ?? 0, 0, lineBytes.Length);
            source.ErrorAt(line.Start + Encoding.UTF8.GetCharCount(lineBytes.AsSpan(0, (int)position)),
                "Invalid JSON. " + error.Message);
        }
        return source;
    }

    internal void Error(string? key, string message) => Problems.Add(new(At(key), message));

    // The incremental provider can cache a parsed file across several compilations.
    internal ArbSource ForValidation()
    {
        var copy = new ArbSource(Path, Text);
        foreach (var pair in Properties) copy.Properties.Add(pair.Key, pair.Value);
        foreach (var pair in _positions) copy._positions.Add(pair.Key, pair.Value);
        copy.Problems.AddRange(Problems);
        return copy;
    }

    internal Location At(string? key)
    {
        var offset = key is not null && _positions.TryGetValue(key, out var bytes) && bytes > 0
            ? Encoding.UTF8.GetCharCount(Encoding.UTF8.GetBytes(Text.ToString()).AsSpan(0, bytes)) : 0;
        return LocationAt(offset);
    }

    private void ErrorAt(int offset, string message) => Problems.Add(new(LocationAt(offset), message));

    private Location LocationAt(int offset)
    {
        var span = new TextSpan(offset, Math.Min(1, Text.Length - offset));
        return Location.Create(Path, span, Text.Lines.GetLinePositionSpan(span));
    }
}
