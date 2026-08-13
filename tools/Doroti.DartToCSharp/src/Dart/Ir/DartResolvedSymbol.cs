namespace Doroti.DartToCSharp;

internal sealed record DartResolvedElement(
    string Kind,
    string Name,
    string CanonicalId,
    SymbolId Symbol,
    bool IsDeprecated,
    string? Type,
    DartType? ResolvedType,
    string? Supertype,
    string[]? Mixins,
    string[]? Interfaces,
    DartResolvedTypeParameter[]? TypeParameters,
    string? ReturnType,
    DartType? ResolvedReturnType,
    DartResolvedParameter[]? Parameters,
    bool IsAbstract,
    bool IsPrivate);

internal sealed record DartResolvedTypeParameter(string Name, string? Bound, DartType? ResolvedBound);

internal sealed record DartResolvedParameter(
    string Name,
    string Type,
    DartType ResolvedType,
    string Kind,
    string? DefaultValue,
    bool IsInitializingFormal,
    bool IsSuperFormal);
