using System.Text.Json;
using Doroti.Intl;
using Doroti.Runtime;
using Microsoft.CodeAnalysis.CSharp;

namespace Doroti.Localization.Generator;

internal sealed record Placeholder(string Name, string Type, string? Format, int? Digits)
{
    internal bool CanPlural => Type is "int" or "long" or "decimal";
}

internal sealed record ArbEntry(IReadOnlyList<MessagePart> Parts, Placeholder[] Placeholders);

internal sealed class ArbBundle(ArbSource source, LocaleTag locale)
{
    internal ArbSource Source { get; } = source;
    internal LocaleTag Locale { get; } = locale;
    internal Dictionary<string, ArbEntry> Messages { get; } = new(StringComparer.Ordinal);

    internal static bool IsIdentifier(string value) =>
        value.Length > 0 && !value.StartsWith('@') && SyntaxFacts.IsValidIdentifier(value)
        && SyntaxFacts.GetKeywordKind(value) == SyntaxKind.None;

    internal void ReadMessages(ArbBundle template, string className)
    {
        var isTemplate = ReferenceEquals(this, template);
        foreach (var (key, value) in Source.Properties)
        {
            if (key.StartsWith('@'))
            {
                if (!key.StartsWith("@@", StringComparison.Ordinal) && !Source.Properties.ContainsKey(key[1..]))
                    Source.Error(key, $"Metadata '{key}' does not refer to a message in this file.");
                continue;
            }
            try
            {
                if (!IsIdentifier(key) || key == className || key.StartsWith("__Doroti", StringComparison.Ordinal)
                    || key is "Locale" or "SupportedLocales" or "Delegate" or "Of" or "Get" or "IsSupported"
                    or "GeneratorVersion" or "ToString" or "Equals" or "GetType" or "GetHashCode" or "Finalize" or "MemberwiseClone")
                    throw new FormatException($"'{key}' cannot be used as a generated C# member.");
                if (value.ValueKind != JsonValueKind.String)
                    throw new FormatException("A message must be a JSON string.");

                var parts = new ArbMessage(value.GetString()!).Parse();
                var metadata = ReadPlaceholders(key);
                if (!isTemplate)
                {
                    if (!template.Messages.TryGetValue(key, out var entry))
                        throw new FormatException("The template has no valid message for this key.");
                    if (metadata is not null && !metadata.OrderBy(p => p.Name, StringComparer.Ordinal)
                        .SequenceEqual(entry.Placeholders.OrderBy(p => p.Name, StringComparer.Ordinal)))
                        throw new FormatException("Placeholder metadata must match the template.");
                    metadata = entry.Placeholders;
                }
                metadata ??= [];
                ValidateParts(parts, metadata);
                Messages.Add(key, new(parts, metadata));
            }
            catch (FormatException error) { Source.Error(key, error.Message); }
            catch (InvalidOperationException error) { Source.Error(key, "Invalid JSON value. " + error.Message); }
        }

        if (!isTemplate)
            foreach (var key in template.Messages.Keys.Where(key => !Source.Properties.ContainsKey(key)))
                Source.Error("@@locale", $"Missing translation for '{key}'.");
    }

    private Placeholder[]? ReadPlaceholders(string key)
    {
        if (!Source.Properties.TryGetValue("@" + key, out var metadata)) return null;
        if (metadata.ValueKind != JsonValueKind.Object) throw new FormatException("Message metadata must be an object.");
        if (!metadata.TryGetProperty("placeholders", out var placeholders)) return null;
        if (placeholders.ValueKind != JsonValueKind.Object) throw new FormatException("'placeholders' must be an object.");
        return placeholders.EnumerateObject().Select(p => ReadPlaceholder(p.Name, p.Value)).ToArray();
    }

    private static Placeholder ReadPlaceholder(string name, JsonElement value)
    {
        if (!IsIdentifier(name) || value.ValueKind != JsonValueKind.Object)
            throw new FormatException($"Invalid placeholder '{name}'.");
        var type = OptionalString(value, "type") switch
        {
            "String" or "string" => "string",
            "int" => "int",
            "long" => "long",
            "decimal" or "num" => "decimal",
            "double" => "double",
            "DateTime" => "DateTime",
            "TimeOnly" => "TimeOnly",
            var unknown => throw new FormatException($"Unsupported type '{unknown}' for placeholder '{name}'."),
        };
        var format = OptionalString(value, "format");
        int? digits = null;
        if (value.TryGetProperty("optionalParameters", out var options))
        {
            if (options.ValueKind != JsonValueKind.Object) throw new FormatException("'optionalParameters' must be an object.");
            foreach (var pair in options.EnumerateObject())
            {
                if (pair.Name != "decimalDigits" || pair.Value.ValueKind != JsonValueKind.Number
                    || !pair.Value.TryGetInt32(out var digitCount) || digitCount is < 0 or > 28)
                    throw new FormatException($"Unsupported format option '{pair.Name}' for '{name}'.");
                digits = digitCount;
            }
        }
        var supported = type switch
        {
            "DateTime" => format is "yMd" or "yMMMd" or "MMMEd" or "yMMMMEEEEd" or "yMMMM" or "MMMd" or "y" or "Hm" or "jm",
            "TimeOnly" => format is "Hm" or "jm",
            "int" or "long" or "decimal" => format is null or "decimalPattern" or "decimalPatternDigits"
                or "currency" or "simpleCurrency" or "percentPattern" or "decimalPercentPattern",
            _ => format is null,
        };
        if (!supported || (digits is not null && format is not ("decimalPatternDigits" or "currency"
                or "simpleCurrency" or "decimalPercentPattern")))
            throw new FormatException($"Unsupported format '{format}' for {type} placeholder '{name}'.");
        return new(name, type, format, digits);
    }

    private static string? OptionalString(JsonElement value, string key) =>
        !value.TryGetProperty(key, out var property) ? null : property.ValueKind == JsonValueKind.String
            ? property.GetString() : throw new FormatException($"'{key}' must be a string.");

    private void ValidateParts(IReadOnlyList<MessagePart> parts, Placeholder[] metadata)
    {
        var expected = metadata.ToDictionary(p => p.Name, StringComparer.Ordinal);
        var used = new HashSet<string>(StringComparer.Ordinal);
        Visit(parts);
        if (used.Count != expected.Count)
            throw new FormatException($"Missing placeholder(s): {string.Join(", ", expected.Keys.Except(used))}.");

        void Visit(IReadOnlyList<MessagePart> current)
        {
            foreach (var part in current)
            {
                switch (part)
                {
                    case PlaceholderPart argument: RequirePlaceholder(argument.Name); break;
                    case ChoicePart choice:
                        var placeholder = RequirePlaceholder(choice.Placeholder);
                        if (choice.IsPlural && (!placeholder.CanPlural || CardinalPluralRules.Group(Locale) < 0))
                            throw new FormatException($"Plural requires int, long or decimal and supported cardinal rules for {Locale}.");
                        if (!choice.IsPlural && placeholder.Type != "string")
                            throw new FormatException($"Select requires a string placeholder ('{choice.Placeholder}').");
                        foreach (var branch in choice.Branches) Visit(branch.Parts);
                        break;
                }
            }
        }

        Placeholder RequirePlaceholder(string name)
        {
            if (!expected.TryGetValue(name, out var result))
                throw new FormatException($"Unknown placeholder '{name}'. Declare it in the template.");
            used.Add(name);
            return result;
        }
    }
}
