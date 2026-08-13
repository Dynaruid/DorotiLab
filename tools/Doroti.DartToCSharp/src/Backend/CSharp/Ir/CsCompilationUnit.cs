namespace Doroti.DartToCSharp;

internal sealed record CsOrigin(string Source, int Offset, int Length, string? SymbolId);

internal sealed record CsCompilationUnit(
    string Namespace,
    CsUsing[] Usings,
    CsDeclaration[] Declarations);

internal sealed record CsUsing(string Namespace, string? Alias = null, bool IsStatic = false);

internal enum CsAccessibility
{
    Private,
    Internal,
    Protected,
    Public,
}

internal enum CsTypeKind
{
    Class,
    Struct,
    Interface,
    Enum,
}

internal abstract record CsDeclaration(CsOrigin Origin);

internal sealed record CsTypeDeclaration(
    CsOrigin Origin,
    CsAccessibility Accessibility,
    CsTypeKind Kind,
    string Name,
    string[] TypeParameters,
    CsTypeReference[] BaseTypes,
    CsMember[] Members,
    bool IsAbstract = false,
    bool IsSealed = false,
    bool IsStatic = false,
    bool IsPartial = true) : CsDeclaration(Origin);

internal sealed record CsDelegateDeclaration(
    CsOrigin Origin,
    CsAccessibility Accessibility,
    string Name,
    string[] TypeParameters,
    CsTypeReference ReturnType,
    CsParameter[] Parameters) : CsDeclaration(Origin);

internal sealed record CsTypeReference(
    string Namespace,
    string Name,
    CsTypeReference[] TypeArguments,
    bool IsNullable = false)
{
    public static CsTypeReference Named(string name, bool isNullable = false) => new(string.Empty, name, [], isNullable);
}

internal sealed record CsParameter(CsTypeReference Type, string Name, CsExpression? DefaultValue = null);

internal abstract record CsMember(CsOrigin Origin, CsAccessibility Accessibility, string Name);

internal sealed record CsField(
    CsOrigin Origin,
    CsAccessibility Accessibility,
    string Name,
    CsTypeReference Type,
    CsExpression? Initializer = null,
    bool IsStatic = false,
    bool IsReadOnly = false,
    bool IsConst = false) : CsMember(Origin, Accessibility, Name);

internal sealed record CsMethod(
    CsOrigin Origin,
    CsAccessibility Accessibility,
    string Name,
    CsTypeReference ReturnType,
    string[] TypeParameters,
    CsParameter[] Parameters,
    CsStatement[] Body,
    bool IsStatic = false,
    bool IsAbstract = false,
    bool IsOverride = false,
    bool IsAsync = false) : CsMember(Origin, Accessibility, Name);

internal abstract record CsStatement(CsOrigin Origin);
internal sealed record CsReturn(CsOrigin Origin, CsExpression? Expression) : CsStatement(Origin);
internal sealed record CsExpressionStatement(CsOrigin Origin, CsExpression Expression) : CsStatement(Origin);
internal sealed record CsBlock(CsOrigin Origin, CsStatement[] Statements) : CsStatement(Origin);

internal abstract record CsExpression(CsOrigin Origin);
internal sealed record CsLiteral(CsOrigin Origin, object? Value) : CsExpression(Origin);
internal sealed record CsIdentifier(CsOrigin Origin, string Name) : CsExpression(Origin);
internal sealed record CsMemberAccess(CsOrigin Origin, CsExpression Target, string Name) : CsExpression(Origin);
internal sealed record CsInvocation(CsOrigin Origin, CsExpression Target, CsExpression[] Arguments) : CsExpression(Origin);
