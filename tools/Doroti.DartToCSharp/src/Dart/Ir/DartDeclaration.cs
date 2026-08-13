namespace Doroti.DartToCSharp;

internal abstract record DartDeclaration(SourceOrigin Origin, SymbolId Symbol, string Name);

internal sealed record DartFunctionDeclaration(
    SourceOrigin Origin,
    SymbolId Symbol,
    string Name,
    DartFunctionType FunctionType,
    DartStatement Body) : DartDeclaration(Origin, Symbol, Name);

internal sealed record DartClassDeclaration(
    SourceOrigin Origin,
    SymbolId Symbol,
    string Name,
    SymbolId? Superclass,
    SymbolId[] Mixins,
    SymbolId[] Interfaces,
    DartDeclaration[] Members,
    bool IsAbstract = false) : DartDeclaration(Origin, Symbol, Name);

internal sealed record DartFieldDeclaration(
    SourceOrigin Origin,
    SymbolId Symbol,
    string Name,
    DartType Type,
    DartExpression? Initializer,
    bool IsStatic,
    bool IsFinal) : DartDeclaration(Origin, Symbol, Name);

internal sealed record DartProgram(
    LibraryId Library,
    DartDeclaration[] Declarations,
    IReadOnlyDictionary<SymbolId, DartDeclaration> Symbols);
