using System.Globalization;
using System.Text;

namespace Doroti.DartToCSharp;

internal enum CsSyntaxTokenKind
{
    Trivia,
    Comment,
    Directive,
    Keyword,
    Identifier,
    Literal,
    Operator,
    Punctuation,
}

internal enum CsSyntaxRegionKind
{
    Declaration,
    Statement,
    Expression,
}

internal sealed record CsSyntaxToken(CsSyntaxTokenKind Kind, string Text, int StartOffset, int EndOffset);

internal sealed record CsSyntaxRegion(
    CsSyntaxRegionKind Kind,
    CsOrigin Origin,
    int StartOffset,
    int EndOffset);

internal sealed record CsSyntaxDocument(CsSyntaxToken[] Tokens, CsSyntaxRegion[] Regions)
{
    public bool Contains(string value) =>
        string.Concat(Tokens.Select(item => item.Text)).Contains(value, StringComparison.Ordinal);

    public CsSyntaxDocument RenameIdentifier(string oldName, string newName, bool renameAssignments = true)
    {
        var rewritten = new List<CsSyntaxToken>(Tokens.Length);
        var newOffset = 0;
        for (var index = 0; index < Tokens.Length; index++)
        {
            var token = Tokens[index];
            var isTarget = token.Kind == CsSyntaxTokenKind.Identifier &&
                string.Equals(token.Text, oldName, StringComparison.Ordinal);
            if (isTarget)
            {
                var previous = Tokens.Take(index).LastOrDefault(item => item.Kind != CsSyntaxTokenKind.Trivia);
                var next = Tokens.Skip(index + 1).FirstOrDefault(item => item.Kind != CsSyntaxTokenKind.Trivia);
                isTarget = previous is not { Kind: CsSyntaxTokenKind.Punctuation, Text: "." } &&
                    next is not { Kind: CsSyntaxTokenKind.Punctuation, Text: ":" } &&
                    (renameAssignments || next is not { Kind: CsSyntaxTokenKind.Operator, Text: "=" });
            }
            var text = isTarget
                ? newName
                : token.Kind == CsSyntaxTokenKind.Literal && token.Text.Contains('{', StringComparison.Ordinal) &&
                  (token.Text.StartsWith("$\"", StringComparison.Ordinal) ||
                   token.Text.StartsWith("$@\"", StringComparison.Ordinal) ||
                   token.Text.StartsWith("@$\"", StringComparison.Ordinal) ||
                   index > 0 && Tokens[index - 1].Kind == CsSyntaxTokenKind.Operator &&
                   Tokens[index - 1].Text.Contains('$', StringComparison.Ordinal) ||
                   token.Text.Contains('}', StringComparison.Ordinal))
                    ? RenameInterpolatedIdentifiers(token.Text, oldName, newName)
                    : token.Text;
            rewritten.Add(token with { Text = text, StartOffset = newOffset, EndOffset = newOffset + text.Length });
            newOffset += text.Length;
        }

        var regions = Regions.Select(item => item with
        {
            StartOffset = MapBoundary(item.StartOffset, Tokens, rewritten),
            EndOffset = MapBoundary(item.EndOffset, Tokens, rewritten),
        }).ToArray();
        return new(rewritten.ToArray(), regions);
    }

    public CsSyntaxDocument RenameIdentifierInInvocation(
        string invocationName,
        string oldName,
        string newName)
    {
        var rewritten = new List<CsSyntaxToken>(Tokens.Length);
        var newOffset = 0;
        for (var index = 0; index < Tokens.Length; index++)
        {
            var token = Tokens[index];
            var text = token.Text;
            if (token.Kind == CsSyntaxTokenKind.Identifier &&
                string.Equals(text, oldName, StringComparison.Ordinal) &&
                IsInsideInvocation(index, invocationName))
            {
                text = newName;
            }
            rewritten.Add(token with { Text = text, StartOffset = newOffset, EndOffset = newOffset + text.Length });
            newOffset += text.Length;
        }

        var regions = Regions.Select(item => item with
        {
            StartOffset = MapBoundary(item.StartOffset, Tokens, rewritten),
            EndOffset = MapBoundary(item.EndOffset, Tokens, rewritten),
        }).ToArray();
        return new(rewritten.ToArray(), regions);

        bool IsInsideInvocation(int tokenIndex, string name)
        {
            var depth = 0;
            for (var cursor = tokenIndex - 1; cursor >= 0; cursor--)
            {
                var candidate = Tokens[cursor];
                if (candidate.Kind == CsSyntaxTokenKind.Punctuation && candidate.Text == ")") depth++;
                if (candidate.Kind != CsSyntaxTokenKind.Punctuation || candidate.Text != "(") continue;
                if (depth > 0)
                {
                    depth--;
                    continue;
                }
                var callee = Tokens.Take(cursor).LastOrDefault(item => item.Kind != CsSyntaxTokenKind.Trivia);
                return callee is { Kind: CsSyntaxTokenKind.Identifier } &&
                    string.Equals(callee.Text, name, StringComparison.Ordinal);
            }
            return false;
        }
    }

    private static int MapBoundary(int boundary, CsSyntaxToken[] original, IReadOnlyList<CsSyntaxToken> rewritten)
    {
        if (boundary <= 0 || original.Length == 0) return 0;
        for (var index = 0; index < original.Length; index++)
        {
            var source = original[index];
            if (boundary > source.EndOffset) continue;
            var target = rewritten[index];
            if (boundary == source.EndOffset) return target.EndOffset;
            return target.StartOffset + Math.Min(boundary - source.StartOffset, target.Text.Length);
        }
        return rewritten.Count == 0 ? 0 : rewritten[^1].EndOffset;
    }

    private static string RenameInterpolatedIdentifiers(string text, string oldName, string newName)
    {
        var result = new StringBuilder(text.Length);
        var cursor = 0;
        while (cursor < text.Length)
        {
            var match = text.IndexOf(oldName, cursor, StringComparison.Ordinal);
            if (match < 0)
            {
                result.Append(text, cursor, text.Length - cursor);
                break;
            }

            var before = match == 0 ? '\0' : text[match - 1];
            var afterIndex = match + oldName.Length;
            var after = afterIndex >= text.Length ? '\0' : text[afterIndex];
            var isIdentifier = !(char.IsLetterOrDigit(before) || before is '_' or '.') &&
                !(char.IsLetterOrDigit(after) || after == '_');
            var labelIndex = afterIndex;
            while (labelIndex < text.Length && char.IsWhiteSpace(text[labelIndex])) labelIndex++;
            var isNamedLabel = labelIndex < text.Length && text[labelIndex] == ':';

            result.Append(text, cursor, match - cursor);
            result.Append(isIdentifier && !isNamedLabel ? newName : oldName);
            cursor = afterIndex;
        }
        return result.ToString();
    }
}

/// <summary>
/// Builds the lossless structured syntax IR consumed by <see cref="CSharpPrinter"/>.
/// It deliberately does not expose generated line numbers: source line ownership is
/// computed only while the printer serializes the final token stream.
/// </summary>
internal sealed class CsSyntaxBuilder
{
    private readonly StringBuilder _buffer;
    private readonly List<(CsSyntaxRegionKind Kind, CsOrigin Origin, int Start)> _openRegions = [];
    private readonly List<CsSyntaxRegion> _regions = [];

    public CsSyntaxBuilder(int capacity = 16) => _buffer = new StringBuilder(capacity);

    public int Length => _buffer.Length;

    public CsSyntaxBuilder Append(char value)
    {
        _buffer.Append(value);
        return this;
    }

    public CsSyntaxBuilder Append(string? value)
    {
        _buffer.Append(value);
        return this;
    }

    public CsSyntaxBuilder Append(object? value) => Append(
        value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : value?.ToString());

    public CsSyntaxBuilder Append(CsSyntaxDocument document)
    {
        var start = _buffer.Length;
        foreach (var token in document.Tokens)
        {
            _buffer.Append(token.Text);
        }
        _regions.AddRange(document.Regions.Select(item => item with
        {
            StartOffset = start + item.StartOffset,
            EndOffset = start + item.EndOffset,
        }));
        return this;
    }

    public CsSyntaxBuilder AppendLine()
    {
        _buffer.Append('\n');
        return this;
    }

    public CsSyntaxBuilder AppendLine(string? value)
    {
        Append(value);
        return AppendLine();
    }

    public IDisposable BeginRegion(CsSyntaxRegionKind kind, CsOrigin origin)
    {
        _openRegions.Add((kind, origin, _buffer.Length));
        return new RegionScope(this, _openRegions.Count - 1);
    }

    public CsSyntaxDocument Build()
    {
        if (_openRegions.Count != 0)
        {
            throw new InvalidOperationException("C# syntax IR contains an unclosed region.");
        }

        return new(Tokenize(_buffer.ToString()), _regions
            .OrderBy(item => item.StartOffset)
            .ThenByDescending(item => item.EndOffset)
            .ToArray());
    }

    // Temporary fragments are kept inside lowering only for lexical rename and
    // look-ahead decisions. Product output always goes through Build + printer.
    public string RenderFragment() => _buffer.ToString();

    private void EndRegion(int index)
    {
        if (index != _openRegions.Count - 1)
        {
            throw new InvalidOperationException("C# syntax IR regions must be closed in stack order.");
        }

        var region = _openRegions[index];
        _openRegions.RemoveAt(index);
        _regions.Add(new(region.Kind, region.Origin, region.Start, _buffer.Length));
    }

    private static CsSyntaxToken[] Tokenize(string text)
    {
        var tokens = new List<CsSyntaxToken>();
        var index = 0;
        while (index < text.Length)
        {
            var start = index;
            var current = text[index];
            CsSyntaxTokenKind kind;

            if (char.IsWhiteSpace(current))
            {
                kind = CsSyntaxTokenKind.Trivia;
                while (index < text.Length && char.IsWhiteSpace(text[index])) index++;
            }
            else if (current == '#' && (start == 0 || text[start - 1] == '\n'))
            {
                kind = CsSyntaxTokenKind.Directive;
                while (index < text.Length && text[index] != '\n') index++;
            }
            else if (current == '/' && index + 1 < text.Length && text[index + 1] == '/')
            {
                kind = CsSyntaxTokenKind.Comment;
                index += 2;
                while (index < text.Length && text[index] != '\n') index++;
            }
            else if (current is '\"' or '\'')
            {
                kind = CsSyntaxTokenKind.Literal;
                var quote = current;
                index++;
                while (index < text.Length)
                {
                    if (text[index] == '\\')
                    {
                        index = Math.Min(index + 2, text.Length);
                    }
                    else if (text[index++] == quote)
                    {
                        break;
                    }
                }
            }
            else if (char.IsLetter(current) || current is '_' or '@')
            {
                index++;
                while (index < text.Length && (char.IsLetterOrDigit(text[index]) || text[index] == '_')) index++;
                var value = text[start..index].TrimStart('@');
                kind = current == '@' || !CSharpKeywords.Contains(value)
                    ? CsSyntaxTokenKind.Identifier
                    : CsSyntaxTokenKind.Keyword;
            }
            else if (char.IsDigit(current))
            {
                kind = CsSyntaxTokenKind.Literal;
                index++;
                while (index < text.Length && (char.IsLetterOrDigit(text[index]) || text[index] is '.' or '_')) index++;
            }
            else if ("{}()[];,.:".Contains(current, StringComparison.Ordinal))
            {
                kind = CsSyntaxTokenKind.Punctuation;
                index++;
            }
            else
            {
                kind = CsSyntaxTokenKind.Operator;
                index++;
                while (index < text.Length && !char.IsWhiteSpace(text[index]) &&
                       !char.IsLetterOrDigit(text[index]) &&
                       !"{}()[];,.:'\"".Contains(text[index], StringComparison.Ordinal))
                {
                    index++;
                }
            }

            tokens.Add(new(kind, text[start..index], start, index));
        }
        return tokens.ToArray();
    }

    private sealed class RegionScope(CsSyntaxBuilder owner, int index) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            owner.EndRegion(index);
        }
    }

    private static readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum",
        "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
        "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
        "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked",
        "unsafe", "ushort", "using", "virtual", "void", "volatile", "while",
    };
}
