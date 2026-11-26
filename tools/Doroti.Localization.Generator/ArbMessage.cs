using System.Globalization;
using System.Text;

namespace Doroti.Localization.Generator;

internal abstract record MessagePart;
internal sealed record TextPart(string Value) : MessagePart;
internal sealed record PlaceholderPart(string Name) : MessagePart;
internal sealed record ChoicePart(string Placeholder, bool IsPlural, IReadOnlyList<ChoiceBranch> Branches) : MessagePart;
internal sealed record ChoiceBranch(string Key, decimal? Exact, IReadOnlyList<MessagePart> Parts);

internal sealed class ArbMessage(string input)
{
    private int _offset;

    internal IReadOnlyList<MessagePart> Parse() => ParseBody(0, insidePlural: false);

    private IReadOnlyList<MessagePart> ParseBody(int depth, bool insidePlural)
    {
        if (depth > 32) throw Error("Messages support at most 32 levels of nesting.");
        var parts = new List<MessagePart>();
        var literal = new StringBuilder();
        while (_offset < input.Length)
        {
            var next = input[_offset];
            if (next == '}' && depth > 0) break;
            _offset++;
            switch (next)
            {
                case '}': throw Error("Unexpected closing brace.");
                case '#' when insidePlural: throw Error("Use the named placeholder instead of unquoted '#'.");
                case '\'': ReadApostrophe(literal); break;
                case '{':
                    FlushText();
                    parts.Add(ParsePlaceholder(depth, insidePlural));
                    break;
                default: literal.Append(next); break;
            }
        }
        FlushText();
        return parts;

        void FlushText()
        {
            if (literal.Length == 0) return;
            parts.Add(new TextPart(literal.ToString()));
            literal.Clear();
        }
    }

    private MessagePart ParsePlaceholder(int depth, bool insidePlural)
    {
        var name = ReadToken();
        if (Take('}')) return new PlaceholderPart(name);
        Require(',');
        var kind = ReadToken();
        var plural = kind == "plural";
        if (!plural && kind != "select") throw Error($"Unsupported message argument type '{kind}'.");
        Require(',');

        var keys = new HashSet<string>(StringComparer.Ordinal);
        var numbers = new HashSet<decimal>();
        var branches = new List<ChoiceBranch>();
        while (!Take('}'))
        {
            var key = ReadToken();
            decimal? exact = null;
            if (plural && key.StartsWith('='))
            {
                if (!decimal.TryParse(key.AsSpan(1), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture, out var value))
                    throw Error($"Invalid exact plural selector '{key}'.");
                if (!numbers.Add(value)) throw Error($"Duplicate exact plural selector '{key}'.");
                exact = value;
            }
            else if (plural && key is not ("zero" or "one" or "two" or "few" or "many" or "other"))
            {
                throw Error($"Unsupported plural selector '{key}'.");
            }

            if (!keys.Add(key)) throw Error($"Duplicate '{key}' branch.");
            Require('{');
            var body = ParseBody(depth + 1, insidePlural || plural);
            Require('}');
            branches.Add(new(key, exact, body));
        }
        if (!keys.Contains("other")) throw Error($"'{kind}' requires an 'other' branch.");
        return new ChoicePart(name, plural, branches);
    }

    private void ReadApostrophe(StringBuilder literal)
    {
        if (TakeRaw('\'')) { literal.Append('\''); return; }
        if (_offset == input.Length || input[_offset] is not ('{' or '}' or '#'))
        {
            literal.Append('\'');
            return;
        }

        while (_offset < input.Length)
        {
            var next = input[_offset++];
            if (next == '\'' && !TakeRaw('\'')) return;
            literal.Append(next);
        }
        throw Error("Unterminated quoted literal.");
    }

    private string ReadToken()
    {
        SkipWhitespace();
        var start = _offset;
        while (_offset < input.Length && !char.IsWhiteSpace(input[_offset]) && input[_offset] is not ('{' or '}' or ','))
            _offset++;
        if (_offset == start) throw Error("Expected a placeholder, argument type or selector.");
        return input[start.._offset];
    }

    private void Require(char value)
    {
        if (!Take(value)) throw Error($"Expected '{value}'.");
    }

    private bool Take(char value) { SkipWhitespace(); return TakeRaw(value); }

    private bool TakeRaw(char value)
    {
        if (_offset == input.Length || input[_offset] != value) return false;
        _offset++;
        return true;
    }

    private void SkipWhitespace()
    {
        while (_offset < input.Length && char.IsWhiteSpace(input[_offset])) _offset++;
    }

    private FormatException Error(string message) => new($"{message} (message character {_offset + 1})");
}
