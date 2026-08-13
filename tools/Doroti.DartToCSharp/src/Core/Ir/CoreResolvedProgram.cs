namespace Doroti.DartToCSharp;

internal sealed record CoreResolvedElement(
    string Kind,
    string Name,
    string CanonicalId,
    SymbolId Symbol,
    bool IsDeprecated,
    string? Type,
    CoreType? ResolvedType,
    string? Supertype,
    string[]? Mixins,
    string[]? Interfaces,
    CoreResolvedTypeParameter[]? TypeParameters,
    string? ReturnType,
    CoreType? ResolvedReturnType,
    CoreResolvedParameter[]? Parameters,
    bool IsAbstract,
    bool IsPrivate);

internal sealed record CoreResolvedTypeParameter(string Name, string? Bound, CoreType? ResolvedBound);

internal sealed record CoreResolvedParameter(
    string Name,
    string Type,
    CoreType ResolvedType,
    string Kind,
    string? DefaultValue,
    bool IsInitializingFormal,
    bool IsSuperFormal);

internal sealed record CoreAstNode(
    CoreNodeKind Kind,
    string SourceKind,
    string AnalyzerKind,
    string Category,
    SourceOrigin Origin,
    CoreType? ResolvedType,
    SymbolId? ResolvedElement,
    RuntimeIntrinsic? RuntimeIntrinsic,
    FlutterBinding? FlutterBinding,
    IReadOnlyDictionary<string, string?> Properties,
    CoreAstNode[] Children)
{
    public int Offset => Origin.Offset;
    public int Length => Origin.Length;
    public string? StaticType { get; init; }
    public string? ElementId => ResolvedElement?.Value;

    public string? Text(CoreProperty property) =>
        Properties.GetValueOrDefault(property.ToString());

    public CoreAstNode? Child(CoreChildRole role)
    {
        var property = role.ToString();
        return int.TryParse(Properties.GetValueOrDefault(property), out var offset)
            ? Children.FirstOrDefault(item => item.Offset == offset)
            : null;
    }

    public string? ParameterType(int index) => Properties.GetValueOrDefault($"parameter{index}Type");
    public string? ParameterName(int index) => Properties.GetValueOrDefault($"parameter{index}Name");
}

internal sealed record CoreResolvedDeclaration(
    string Kind,
    string Name,
    int Offset,
    int Length,
    CoreResolvedElement Element,
    CoreAstNode Ast,
    CoreResolvedMember[] Members);

internal sealed record CoreResolvedMember(
    string Kind,
    string Name,
    int Offset,
    int Length,
    CoreResolvedElement Element,
    CoreAstNode Ast,
    bool IsStatic,
    bool IsFinal,
    bool IsConst,
    bool IsLate,
    bool IsAbstract,
    bool IsGetter,
    bool IsSetter,
    bool IsOperator,
    bool IsFactory);
