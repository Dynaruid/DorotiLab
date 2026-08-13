using System.Globalization;

namespace Doroti.DartToCSharp;

internal sealed class DartTypeDecodeException(string sourceType, string message)
    : FormatException($"Unsupported analyzer type '{sourceType}': {message}")
{
    public string SourceType { get; } = sourceType;
}

internal static class DartTypeDecoder
{
    public static DartType Decode(string sourceType)
    {
        if (string.IsNullOrWhiteSpace(sourceType))
        {
            throw new DartTypeDecodeException(sourceType, "type text is empty");
        }

        return DecodeCore(sourceType.Trim(), sourceType);
    }

    private static DartType DecodeCore(string value, string original)
    {
        // Analyzer flow analysis represents a promoted type parameter as
        // `T & Bound`. The promoted bound is the usable runtime surface for
        // generated C#; retaining the unconstrained left-hand parameter would
        // discard the members proven by the promotion.
        var intersectionIndex = FindTopLevel(value, " & ");
        if (intersectionIndex >= 0)
        {
            return DecodeCore(value[(intersectionIndex + " & ".Length)..].Trim(), original);
        }

        var nullability = Nullability.NonNullable;
        if (value.EndsWith("?", StringComparison.Ordinal))
        {
            nullability = Nullability.Nullable;
            value = value[..^1].TrimEnd();
        }
        else if (value.EndsWith("*", StringComparison.Ordinal))
        {
            nullability = Nullability.Legacy;
            value = value[..^1].TrimEnd();
        }

        if (value.Length == 0)
        {
            throw new DartTypeDecodeException(original, "missing type before nullability suffix");
        }
        if (value == "dynamic") return new DartDynamicType();
        if (value == "void") return new DartVoidType();
        if (value == "Never") return new DartNeverType(nullability);
        if (value == "Null") return new DartNullType();

        var functionIndex = FindOutermostFunction(value);
        if (functionIndex >= 0 && value.EndsWith(')'))
        {
            var returnType = DecodeCore(value[..functionIndex].Trim(), original);
            var signature = value[(functionIndex + " Function".Length)..];
            var parametersStart = FunctionParametersStart(signature, original);
            var parametersText = signature[(parametersStart + 1)..^1];
            var parameters = DecodeFunctionParameters(parametersText, original);
            return new DartFunctionType(returnType, parameters, nullability);
        }

        if (value.StartsWith('(') && value.EndsWith(')'))
        {
            return DecodeRecordType(value[1..^1], original, nullability);
        }

        var genericStart = FindTopLevelCharacter(value, '<');
        string name;
        DartType[] arguments;
        if (genericStart >= 0)
        {
            if (!value.EndsWith('>') || !IsBalanced(value[genericStart..], '<', '>'))
            {
                throw new DartTypeDecodeException(original, "generic argument list is not balanced");
            }
            name = value[..genericStart].Trim();
            arguments = SplitTopLevel(value[(genericStart + 1)..^1], ',')
                .Select(item => DecodeCore(item, original))
                .ToArray();
        }
        else
        {
            name = value;
            arguments = [];
        }

        if (!IsTypeName(name))
        {
            throw new DartTypeDecodeException(original, $"invalid type name '{name}'");
        }
        return new DartInterfaceType(SymbolId.TypeName(name), arguments, nullability);
    }

    private static DartRecordType DecodeRecordType(string fieldsText, string original, Nullability nullability)
    {
        var positional = new List<DartType>();
        var named = new Dictionary<string, DartType>(StringComparer.Ordinal);
        foreach (var field in SplitTopLevel(fieldsText, ',').Where(item => item.Length > 0))
        {
            var trimmed = field.Trim();
            if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
            {
                foreach (var namedField in SplitTopLevel(trimmed[1..^1], ',').Where(item => item.Length > 0))
                {
                    var separator = FindLastTopLevelSpace(namedField);
                    if (separator < 0)
                    {
                        throw new DartTypeDecodeException(original, $"named record field is missing its name: '{namedField}'");
                    }
                    var name = namedField[(separator + 1)..].Trim();
                    if (!IsIdentifier(name))
                    {
                        throw new DartTypeDecodeException(original, $"invalid named record field '{name}'");
                    }
                    if (!named.TryAdd(name, DecodeCore(namedField[..separator].Trim(), original)))
                    {
                        throw new DartTypeDecodeException(original, $"duplicate named record field '{name}'");
                    }
                }
            }
            else
            {
                positional.Add(DecodeCore(trimmed, original));
            }
        }
        return new DartRecordType(positional.ToArray(), named, nullability);
    }

    private static DartFunctionParameter DecodeFunctionParameter(string value, string original)
    {
        var trimmed = value.Trim();
        var kind = DartParameterKind.RequiredPositional;
        if ((trimmed.StartsWith('[') && trimmed.EndsWith(']')) ||
            (trimmed.StartsWith('{') && trimmed.EndsWith('}')))
        {
            var named = trimmed[0] == '{';
            trimmed = trimmed[1..^1].Trim();
            kind = named ? DartParameterKind.OptionalNamed : DartParameterKind.OptionalPositional;
        }
        if (trimmed.StartsWith("required ", StringComparison.Ordinal))
        {
            trimmed = trimmed["required ".Length..];
            kind = DartParameterKind.RequiredNamed;
        }

        var separator = FindLastTopLevelSpace(trimmed);
        if (separator < 0)
        {
            return new(null, DecodeCore(trimmed, original), kind);
        }
        var name = trimmed[(separator + 1)..].Trim();
        if (!IsIdentifier(name))
        {
            return new(null, DecodeCore(trimmed, original), kind);
        }
        return new(name, DecodeCore(trimmed[..separator].Trim(), original), kind);
    }

    private static DartFunctionParameter[] DecodeFunctionParameters(string value, string original)
    {
        var result = new List<DartFunctionParameter>();
        foreach (var segment in SplitTopLevel(value, ',').Where(item => item.Length > 0))
        {
            var trimmed = segment.Trim();
            if ((trimmed.StartsWith('{') && trimmed.EndsWith('}')) ||
                (trimmed.StartsWith('[') && trimmed.EndsWith(']')))
            {
                var open = trimmed[0];
                foreach (var grouped in SplitTopLevel(trimmed[1..^1], ',').Where(item => item.Length > 0))
                {
                    result.Add(DecodeFunctionParameter($"{open}{grouped}{(open == '{' ? '}' : ']')}", original));
                }
            }
            else
            {
                result.Add(DecodeFunctionParameter(trimmed, original));
            }
        }
        return result.ToArray();
    }

    private static int FunctionParametersStart(string signature, string original)
    {
        if (signature.StartsWith('('))
        {
            return 0;
        }
        if (!signature.StartsWith('<'))
        {
            throw new DartTypeDecodeException(original, "function type is missing its parameter list");
        }
        var depth = 0;
        for (var index = 0; index < signature.Length; index++)
        {
            if (signature[index] == '<') depth++;
            else if (signature[index] == '>' && --depth == 0)
            {
                var parameterStart = index + 1;
                if (parameterStart >= signature.Length || signature[parameterStart] != '(')
                {
                    throw new DartTypeDecodeException(original, "generic function type is missing its parameter list");
                }
                return parameterStart;
            }
        }
        throw new DartTypeDecodeException(original, "generic function type parameters are not balanced");
    }

    private static string[] SplitTopLevel(string value, char separator)
    {
        if (value.Trim().Length == 0) return [];
        var result = new List<string>();
        var start = 0;
        var angle = 0;
        var round = 0;
        var square = 0;
        var curly = 0;
        for (var index = 0; index < value.Length; index++)
        {
            switch (value[index])
            {
                case '<': angle++; break;
                case '>': angle--; break;
                case '(': round++; break;
                case ')': round--; break;
                case '[': square++; break;
                case ']': square--; break;
                case '{': curly++; break;
                case '}': curly--; break;
            }
            if (angle < 0 || round < 0 || square < 0 || curly < 0)
            {
                throw new DartTypeDecodeException(value, "unbalanced delimiters");
            }
            if (value[index] == separator && angle == 0 && round == 0 && square == 0 && curly == 0)
            {
                result.Add(value[start..index].Trim());
                start = index + 1;
            }
        }
        if (angle != 0 || round != 0 || square != 0 || curly != 0)
        {
            throw new DartTypeDecodeException(value, "unbalanced delimiters");
        }
        result.Add(value[start..].Trim());
        return result.ToArray();
    }

    private static int FindTopLevel(string value, string token)
    {
        var angle = 0;
        for (var index = 0; index <= value.Length - token.Length; index++)
        {
            if (value[index] == '<') angle++;
            else if (value[index] == '>') angle--;
            if (angle == 0 && value.AsSpan(index).StartsWith(token, StringComparison.Ordinal)) return index;
        }
        return -1;
    }

    private static int FindOutermostFunction(string value)
    {
        const string token = " Function";
        var angle = 0;
        var round = 0;
        var square = 0;
        var curly = 0;
        var result = -1;
        for (var index = 0; index <= value.Length - token.Length; index++)
        {
            if (angle == 0 && round == 0 && square == 0 && curly == 0 &&
                value.AsSpan(index).StartsWith(token, StringComparison.Ordinal))
            {
                result = index;
            }
            switch (value[index])
            {
                case '<': angle++; break;
                case '>': angle--; break;
                case '(': round++; break;
                case ')': round--; break;
                case '[': square++; break;
                case ']': square--; break;
                case '{': curly++; break;
                case '}': curly--; break;
            }
        }
        return result;
    }

    private static int FindTopLevelCharacter(string value, char target)
    {
        var round = 0;
        for (var index = 0; index < value.Length; index++)
        {
            if (value[index] == '(') round++;
            else if (value[index] == ')') round--;
            else if (value[index] == target && round == 0) return index;
        }
        return -1;
    }

    private static int FindLastTopLevelSpace(string value)
    {
        var angle = 0;
        var round = 0;
        for (var index = value.Length - 1; index >= 0; index--)
        {
            if (value[index] == '>') angle++;
            else if (value[index] == '<') angle--;
            else if (value[index] == ')') round++;
            else if (value[index] == '(') round--;
            else if (value[index] == ' ' && angle == 0 && round == 0) return index;
        }
        return -1;
    }

    private static bool IsBalanced(string value, char open, char close)
    {
        var depth = 0;
        foreach (var character in value)
        {
            if (character == open) depth++;
            else if (character == close && --depth < 0) return false;
        }
        return depth == 0;
    }

    private static bool IsTypeName(string value) => value
        .Split('.', StringSplitOptions.RemoveEmptyEntries)
        .All(IsIdentifier);

    private static bool IsIdentifier(string value) => value.Length > 0 &&
        (char.IsLetter(value[0]) || value[0] == '_') &&
        value.Skip(1).All(IsIdentifierContinuation);

    private static bool IsIdentifierContinuation(char character)
    {
        if (char.IsLetterOrDigit(character) || character == '_') return true;
        return CharUnicodeInfo.GetUnicodeCategory(character) is UnicodeCategory.OtherNumber;
    }
}
