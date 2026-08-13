using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static partial class FlutterApiManifestGenerator
{
    public const string BaselineSchemaVersion = "doroti.flutter-baseline/v1";
    public const string ManifestSchemaVersion = "doroti.flutter-api/v2";

    public static FlutterApiManifest Generate(string baselinePath, string outputPath)
    {
        var baseline = ArtifactFiles.ReadJson<FlutterBaseline>(baselinePath);
        if (baseline.SchemaVersion != BaselineSchemaVersion)
        {
            throw new InvalidDataException($"Unsupported Flutter baseline schema: {baseline.SchemaVersion}");
        }

        if (!FullGitRevisionRegex().IsMatch(baseline.FlutterGitRevision))
        {
            throw new InvalidDataException("flutterGitRevision must be an exact 40-character Git revision.");
        }

        var baselineDirectory = Path.GetDirectoryName(Path.GetFullPath(baselinePath))!;
        var sourceRoot = Path.GetFullPath(baseline.SourceRoot, baselineDirectory);
        var inputs = new List<FlutterApiInput>();
        var symbols = new List<FlutterApiSymbol>();
        foreach (var input in baseline.Inputs.OrderBy(item => item.Path, StringComparer.Ordinal))
        {
            var sourcePath = ResolveWithin(sourceRoot, input.Path);
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException($"Flutter API input is missing: {input.Path}", sourcePath);
            }

            var source = File.ReadAllText(sourcePath).Replace("\r\n", "\n", StringComparison.Ordinal);
            inputs.Add(new(input.Library, ArtifactFiles.NormalizePath(input.Path), input.Sha256));
            foreach (var selection in input.Symbols.OrderBy(item => item.Name, StringComparer.Ordinal))
            {
                var declaration = FindDeclaration(source, input.Path, selection.Name);
                symbols.Add(new(
                    input.Library,
                    declaration.Kind,
                    selection.Name,
                    declaration.Signature,
                    declaration.Extends,
                    declaration.Mixins,
                    declaration.Interfaces,
                    declaration.TypeParameters,
                    declaration.IsDeprecated,
                    declaration.Constructors,
                    declaration.Members,
                    selection.SupportState,
                    selection.BehaviorFixture));
            }
        }

        var manifest = new FlutterApiManifest(
            ManifestSchemaVersion,
            baseline.Repository,
            baseline.FlutterGitRevision,
            baseline.DartSdkRange,
            inputs.ToArray(),
            symbols
                .OrderBy(item => item.Library, StringComparer.Ordinal)
                .ThenBy(item => item.Name, StringComparer.Ordinal)
                .ToArray());
        ArtifactFiles.WriteJson(outputPath, manifest);
        return manifest;
    }

    private static ParsedDeclaration FindDeclaration(string source, string path, string name)
    {
        var matches = DeclarationRegex().Matches(source)
            .Cast<Match>()
            .Where(match => string.Equals(match.Groups["name"].Value, name, StringComparison.Ordinal))
            .ToArray();
        if (matches.Length != 1)
        {
            throw new InvalidDataException(
                $"Selected Flutter symbol must have exactly one declaration: {path}::{name} (found {matches.Length}).");
        }

        var match = matches[0];
        var structuralSource = CommentRegex().Replace(source, item => new string(' ', item.Length));
        var bodyStart = FindHeaderTerminator(structuralSource, match.Index + match.Length, '{');
        var declarationEnd = FindHeaderTerminator(structuralSource, match.Index + match.Length, ';');
        var headerEnd = bodyStart < 0
            ? declarationEnd
            : declarationEnd < 0
                ? bodyStart
                : Math.Min(bodyStart, declarationEnd);
        if (headerEnd < 0)
        {
            throw new InvalidDataException($"Could not determine declaration header: {path}::{name}");
        }

        var header = Normalize(source[match.Index..(headerEnd + 1)]);
        var kind = match.Groups["kind"].Value;
        var typeParameters = ParseTypeParameters(header, name);
        var extends = ReadClause(header, "extends", ["with", "implements", "on", "{"]);
        var mixins = SplitTypes(ReadClause(header, kind == "mixin" ? "on" : "with", ["implements", "{"]));
        var interfaces = SplitTypes(ReadClause(header, "implements", ["{"]));
        var deprecated = HasDeprecatedAnnotation(source, match.Index);
        if (bodyStart < 0 || bodyStart != headerEnd)
        {
            return new(kind, header, extends, mixins, interfaces, typeParameters, deprecated, [], []);
        }

        var bodyEnd = FindMatching(source, bodyStart, '{', '}');
        if (bodyEnd < 0)
        {
            throw new InvalidDataException($"Unbalanced declaration body: {path}::{name}");
        }

        var constructors = new List<FlutterApiMember>();
        var members = new List<FlutterApiMember>();
        foreach (var memberHeader in EnumerateMemberHeaders(source[(bodyStart + 1)..bodyEnd]))
        {
            var member = ParseMember(memberHeader, name);
            if (member is null || member.Name.StartsWith('_'))
            {
                continue;
            }
            if (member.Kind == "constructor")
            {
                constructors.Add(member);
            }
            else
            {
                members.Add(member);
            }
        }

        return new(
            kind,
            header,
            extends,
            mixins,
            interfaces,
            typeParameters,
            deprecated,
            constructors.OrderBy(item => item.Name, StringComparer.Ordinal).ThenBy(item => item.Signature, StringComparer.Ordinal).ToArray(),
            members.OrderBy(item => item.Name, StringComparer.Ordinal).ThenBy(item => item.Kind, StringComparer.Ordinal).ThenBy(item => item.Signature, StringComparer.Ordinal).ToArray());
    }

    private static FlutterApiMember? ParseMember(string text, string owner)
    {
        var deprecated = text.Contains("@Deprecated", StringComparison.Ordinal);
        var signature = Normalize(AnnotationRegex().Replace(text, string.Empty));
        if (signature.Length == 0 || signature.StartsWith("assert ", StringComparison.Ordinal))
        {
            return null;
        }

        var constructorMatch = Regex.Match(
            signature,
            $@"^(?:(?:external|const|factory)\s+)*(?<name>{Regex.Escape(owner)}(?:\.[A-Za-z_][A-Za-z0-9_]*)?)\s*\(",
            RegexOptions.CultureInvariant);
        if (constructorMatch.Success)
        {
            var constructorName = constructorMatch.Groups["name"].Value;
            return new("constructor", constructorName, signature, null, false, deprecated, ParseParameters(signature));
        }

        var getter = Regex.Match(signature, @"^(?:(?<static>static)\s+)?(?<type>.+?)\s+get\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\b", RegexOptions.CultureInvariant);
        if (getter.Success)
        {
            return new("getter", getter.Groups["name"].Value, signature, getter.Groups["type"].Value, getter.Groups["static"].Success, deprecated, []);
        }

        var setter = Regex.Match(signature, @"^(?:(?<static>static)\s+)?(?:void\s+)?set\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*\(", RegexOptions.CultureInvariant);
        if (setter.Success)
        {
            return new("setter", setter.Groups["name"].Value, signature, "void", setter.Groups["static"].Success, deprecated, ParseParameters(signature));
        }

        var operatorMatch = Regex.Match(signature, @"^(?:(?<static>static)\s+)?(?<type>.+?)\s+operator\s+(?<name>[^\s(]+)\s*\(", RegexOptions.CultureInvariant);
        if (operatorMatch.Success)
        {
            return new("operator", operatorMatch.Groups["name"].Value, signature, operatorMatch.Groups["type"].Value, operatorMatch.Groups["static"].Success, deprecated, ParseParameters(signature));
        }

        var openParen = signature.IndexOf('(');
        if (openParen >= 0)
        {
            var prefix = signature[..openParen].Trim();
            var methodName = LastIdentifierRegex().Match(prefix);
            if (!methodName.Success)
            {
                return null;
            }
            var name = methodName.Value;
            var returnType = prefix[..methodName.Index].Replace("static ", string.Empty, StringComparison.Ordinal).Trim();
            return new("method", name, signature, returnType.Length == 0 ? null : returnType, prefix.Contains("static ", StringComparison.Ordinal), deprecated, ParseParameters(signature));
        }

        var fieldText = signature.TrimEnd(';').Trim();
        var equals = FindTopLevel(fieldText, '=');
        var declaration = equals >= 0 ? fieldText[..equals].Trim() : fieldText;
        var fieldName = LastIdentifierRegex().Match(declaration);
        if (!fieldName.Success)
        {
            return null;
        }
        var modifiersAndType = declaration[..fieldName.Index].Trim();
        var isStatic = Regex.IsMatch(modifiersAndType, @"\bstatic\b", RegexOptions.CultureInvariant);
        var type = FieldModifierRegex().Replace(modifiersAndType, string.Empty).Trim();
        return new("field", fieldName.Value, signature, type.Length == 0 ? null : type, isStatic, deprecated, []);
    }

    private static FlutterApiParameter[] ParseParameters(string signature)
    {
        var open = signature.IndexOf('(');
        if (open < 0)
        {
            return [];
        }
        var close = FindMatching(signature, open, '(', ')');
        if (close < 0)
        {
            return [];
        }

        var parameters = new List<FlutterApiParameter>();
        var content = signature[(open + 1)..close];
        foreach (var part in SplitParameters(content))
        {
            var value = part.Text.Trim();
            if (value.Length == 0)
            {
                continue;
            }
            var required = value.StartsWith("required ", StringComparison.Ordinal);
            if (required)
            {
                value = value["required ".Length..].TrimStart();
            }
            var equals = FindTopLevel(value, '=');
            var defaultValue = equals >= 0 ? Normalize(value[(equals + 1)..]) : null;
            var declaration = equals >= 0 ? value[..equals].Trim() : value;
            var nameMatch = LastIdentifierRegex().Match(declaration);
            if (!nameMatch.Success)
            {
                continue;
            }
            var parameterName = nameMatch.Value;
            var type = declaration[..nameMatch.Index].Trim();
            if (type is "this." or "super.")
            {
                type = string.Empty;
            }
            parameters.Add(new(
                parameterName,
                type.Length == 0 ? null : type,
                part.Kind,
                required || part.Kind == "required-positional",
                defaultValue));
        }
        return parameters.ToArray();
    }

    private static IEnumerable<(string Text, string Kind)> SplitParameters(string content)
    {
        var builder = new StringBuilder();
        var paren = 0;
        var bracket = 0;
        var brace = 0;
        var quote = '\0';
        foreach (var character in content.Append(','))
        {
            if (quote != '\0')
            {
                builder.Append(character);
                if (character == quote)
                {
                    quote = '\0';
                }
                continue;
            }
            if (character is '\'' or '"')
            {
                quote = character;
                builder.Append(character);
                continue;
            }
            switch (character)
            {
                case '(':
                    paren++;
                    builder.Append(character);
                    break;
                case ')':
                    paren--;
                    builder.Append(character);
                    break;
                case '[':
                    if (paren == 0 && bracket == 0 && brace == 0)
                    {
                        bracket = 1;
                    }
                    else
                    {
                        bracket++;
                        builder.Append(character);
                    }
                    break;
                case ']':
                    if (paren == 0 && bracket == 1 && brace == 0)
                    {
                        bracket = 0;
                    }
                    else
                    {
                        bracket--;
                        builder.Append(character);
                    }
                    break;
                case '{':
                    if (paren == 0 && bracket == 0 && brace == 0)
                    {
                        brace = 1;
                    }
                    else
                    {
                        brace++;
                        builder.Append(character);
                    }
                    break;
                case '}':
                    if (paren == 0 && bracket == 0 && brace == 1)
                    {
                        brace = 0;
                    }
                    else
                    {
                        brace--;
                        builder.Append(character);
                    }
                    break;
                case ',' when paren == 0 && bracket <= 1 && brace <= 1:
                    var kind = brace == 1 ? "named" : bracket == 1 ? "optional-positional" : "required-positional";
                    yield return (builder.ToString(), kind);
                    builder.Clear();
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }
    }

    private static IEnumerable<string> EnumerateMemberHeaders(string body)
    {
        var builder = new StringBuilder();
        var paren = 0;
        var bracket = 0;
        var expressionBraces = 0;
        var quote = '\0';
        for (var index = 0; index < body.Length; index++)
        {
            var character = body[index];
            if (quote != '\0')
            {
                builder.Append(character);
                if (character == '\\' && index + 1 < body.Length)
                {
                    builder.Append(body[++index]);
                }
                else if (character == quote)
                {
                    quote = '\0';
                }
                continue;
            }
            if (character == '/' && index + 1 < body.Length && body[index + 1] == '/')
            {
                var end = body.IndexOf('\n', index + 2);
                if (end < 0)
                {
                    break;
                }
                builder.Append('\n');
                index = end;
                continue;
            }
            if (character == '/' && index + 1 < body.Length && body[index + 1] == '*')
            {
                var end = body.IndexOf("*/", index + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    throw new InvalidDataException("Unbalanced block comment in Flutter member body.");
                }
                builder.Append(' ');
                index = end + 1;
                continue;
            }
            if ((character is '\'' or '"') && index + 2 < body.Length && body[index + 1] == character && body[index + 2] == character)
            {
                var close = body.IndexOf(new string(character, 3), index + 3, StringComparison.Ordinal);
                if (close < 0)
                {
                    throw new InvalidDataException("Unbalanced triple-quoted Dart string in Flutter member body.");
                }
                builder.Append(body, index, (close + 3) - index);
                index = close + 2;
                continue;
            }
            if (character is '\'' or '"')
            {
                quote = character;
                builder.Append(character);
                continue;
            }
            if (character == '(')
            {
                paren++;
            }
            else if (character == ')')
            {
                paren--;
            }
            else if (character == '[')
            {
                bracket++;
            }
            else if (character == ']')
            {
                bracket--;
            }
            else if (character == '{' && paren == 0 && bracket == 0)
            {
                var prefix = Normalize(builder.ToString());
                var looksLikeBody = prefix.Contains(')') || Regex.IsMatch(prefix, @"\bget\s+[A-Za-z_]", RegexOptions.CultureInvariant);
                if (expressionBraces == 0 && looksLikeBody)
                {
                    builder.Append(" {");
                    yield return builder.ToString();
                    builder.Clear();
                    index = FindMatching(body, index, '{', '}');
                    if (index < 0)
                    {
                        throw new InvalidDataException($"Unbalanced Flutter member body after: {prefix}");
                    }
                    continue;
                }
                expressionBraces++;
            }
            else if (character == '}' && paren == 0 && bracket == 0 && expressionBraces > 0)
            {
                expressionBraces--;
            }

            builder.Append(character);
            if (character == ';' && paren == 0 && bracket == 0 && expressionBraces == 0)
            {
                yield return builder.ToString();
                builder.Clear();
            }
        }
    }

    private static FlutterApiTypeParameter[] ParseTypeParameters(string header, string name)
    {
        var nameIndex = header.IndexOf(name, StringComparison.Ordinal);
        var open = nameIndex < 0 ? -1 : header.IndexOf('<', nameIndex + name.Length);
        if (open < 0)
        {
            return [];
        }
        var close = FindMatching(header, open, '<', '>');
        if (close < 0)
        {
            return [];
        }
        return header[(open + 1)..close]
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Split(" extends ", 2, StringSplitOptions.TrimEntries))
            .Select(parts => new FlutterApiTypeParameter(parts[0], parts.Length == 2 ? parts[1] : null))
            .ToArray();
    }

    private static string? ReadClause(string header, string keyword, string[] terminators)
    {
        var match = Regex.Match(header, $@"\b{Regex.Escape(keyword)}\s+", RegexOptions.CultureInvariant);
        if (!match.Success)
        {
            return null;
        }
        var end = header.Length;
        foreach (var terminator in terminators)
        {
            var index = header.IndexOf(terminator, match.Index + match.Length, StringComparison.Ordinal);
            if (index >= 0)
            {
                end = Math.Min(end, index);
            }
        }
        return header[(match.Index + match.Length)..end].Trim().TrimEnd('{').Trim();
    }

    private static string[] SplitTypes(string? value) => value is null
        ? []
        : value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private static bool HasDeprecatedAnnotation(string source, int declarationIndex)
    {
        var start = Math.Max(0, source.LastIndexOf('\n', Math.Max(0, declarationIndex - 1)));
        start = Math.Max(0, source.LastIndexOf('\n', Math.Max(0, start - 1)));
        return source[start..declarationIndex].Contains("@Deprecated", StringComparison.Ordinal);
    }

    private static int FindHeaderTerminator(string source, int start, char terminator)
    {
        var paren = 0;
        var bracket = 0;
        for (var index = start; index < source.Length; index++)
        {
            switch (source[index])
            {
                case '(':
                    paren++;
                    break;
                case ')':
                    paren--;
                    break;
                case '[':
                    bracket++;
                    break;
                case ']':
                    bracket--;
                    break;
                default:
                    if (source[index] == terminator && paren == 0 && bracket == 0)
                    {
                        return index;
                    }
                    break;
            }
        }
        return -1;
    }

    private static int FindMatching(string source, int openIndex, char open, char close)
    {
        var depth = 0;
        var quote = '\0';
        for (var index = openIndex; index < source.Length; index++)
        {
            var character = source[index];
            if (quote != '\0')
            {
                if (character == '\\')
                {
                    index++;
                }
                else if (character == quote)
                {
                    quote = '\0';
                }
                continue;
            }
            if (character == '/' && index + 1 < source.Length && source[index + 1] == '/')
            {
                index = source.IndexOf('\n', index + 2);
                if (index < 0)
                {
                    return -1;
                }
                continue;
            }
            if (character == '/' && index + 1 < source.Length && source[index + 1] == '*')
            {
                index = source.IndexOf("*/", index + 2, StringComparison.Ordinal);
                if (index < 0)
                {
                    return -1;
                }
                index++;
                continue;
            }
            if ((character is '\'' or '"') && index + 2 < source.Length && source[index + 1] == character && source[index + 2] == character)
            {
                var end = source.IndexOf(new string(character, 3), index + 3, StringComparison.Ordinal);
                if (end < 0)
                {
                    return -1;
                }
                index = end + 2;
                continue;
            }
            if (character is '\'' or '"')
            {
                quote = character;
            }
            else if (character == open)
            {
                depth++;
            }
            else if (character == close && --depth == 0)
            {
                return index;
            }
        }
        return -1;
    }

    private static int FindTopLevel(string value, char character)
    {
        var paren = 0;
        var bracket = 0;
        var brace = 0;
        for (var index = 0; index < value.Length; index++)
        {
            switch (value[index])
            {
                case '(':
                    paren++;
                    break;
                case ')':
                    paren--;
                    break;
                case '[':
                    bracket++;
                    break;
                case ']':
                    bracket--;
                    break;
                case '{':
                    brace++;
                    break;
                case '}':
                    brace--;
                    break;
                default:
                    if (value[index] == character && paren == 0 && bracket == 0 && brace == 0)
                    {
                        return index;
                    }
                    break;
            }
        }
        return -1;
    }

    private static string Normalize(string value) => WhitespaceRegex().Replace(value, " ").Trim();

    private static string ResolveWithin(string root, string relativePath)
    {
        var normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var path = Path.GetFullPath(relativePath, root);
        if (!path.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"Flutter API input escapes sourceRoot: {relativePath}");
        }

        return path;
    }

    private sealed record ParsedDeclaration(
        string Kind,
        string Signature,
        string? Extends,
        string[] Mixins,
        string[] Interfaces,
        FlutterApiTypeParameter[] TypeParameters,
        bool IsDeprecated,
        FlutterApiMember[] Constructors,
        FlutterApiMember[] Members);

    [GeneratedRegex("^[0-9a-f]{40}$", RegexOptions.CultureInvariant)]
    private static partial Regex FullGitRevisionRegex();

    [GeneratedRegex(@"(?m)^[ \t]*(?:(?:abstract|base|final|interface|sealed)\s+)*(?<kind>class|enum|mixin|typedef)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)\b", RegexOptions.CultureInvariant)]
    private static partial Regex DeclarationRegex();

    [GeneratedRegex(@"(?s:/\*.*?\*/)|(?m://.*$)", RegexOptions.CultureInvariant)]
    private static partial Regex CommentRegex();

    [GeneratedRegex(@"@[A-Za-z_][A-Za-z0-9_.]*(?:\([^\n]*?\))?", RegexOptions.CultureInvariant)]
    private static partial Regex AnnotationRegex();

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex LastIdentifierRegex();

    [GeneratedRegex(@"\b(?:external|final|late|const|static|covariant)\b", RegexOptions.CultureInvariant)]
    private static partial Regex FieldModifierRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhitespaceRegex();
}

public sealed record FlutterBaseline(
    string SchemaVersion,
    string Repository,
    string FlutterGitRevision,
    string DartSdkRange,
    string SourceRoot,
    FlutterBaselineInput[] Inputs);

public sealed record FlutterBaselineInput(
    string Library,
    string Path,
    string Sha256,
    FlutterSymbolSelection[] Symbols);

public sealed record FlutterSymbolSelection(string Name, string SupportState, string BehaviorFixture);

public sealed record FlutterApiManifest(
    string SchemaVersion,
    string Repository,
    string FlutterGitRevision,
    string DartSdkRange,
    FlutterApiInput[] Inputs,
    FlutterApiSymbol[] Symbols);

public sealed record FlutterApiInput(string Library, string Path, string Sha256);

public sealed record FlutterApiSymbol(
    string Library,
    string Kind,
    string Name,
    string Signature,
    string? Extends,
    string[] Mixins,
    string[] Interfaces,
    FlutterApiTypeParameter[] TypeParameters,
    bool IsDeprecated,
    FlutterApiMember[] Constructors,
    FlutterApiMember[] Members,
    string SupportState,
    string BehaviorFixture);

public sealed record FlutterApiTypeParameter(string Name, string? Bound);

public sealed record FlutterApiMember(
    string Kind,
    string Name,
    string Signature,
    string? ReturnType,
    bool IsStatic,
    bool IsDeprecated,
    FlutterApiParameter[] Parameters);

public sealed record FlutterApiParameter(
    string Name,
    string? Type,
    string Kind,
    bool IsRequired,
    string? DefaultValue);
