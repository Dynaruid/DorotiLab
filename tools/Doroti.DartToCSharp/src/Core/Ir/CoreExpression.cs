namespace Doroti.DartToCSharp;

internal abstract record CoreExpression(SourceOrigin Origin, CoreType Type);
internal sealed record CoreLiteral(SourceOrigin Origin, CoreType Type, object? Value) : CoreExpression(Origin, Type);
internal sealed record CoreLocal(SourceOrigin Origin, CoreType Type, LocalId Local) : CoreExpression(Origin, Type);
internal sealed record CoreIntrinsicCall(
    SourceOrigin Origin,
    CoreType Type,
    RuntimeIntrinsic Intrinsic,
    CoreExpression[] Arguments) : CoreExpression(Origin, Type);
internal sealed record CoreCall(
    SourceOrigin Origin,
    CoreType Type,
    SymbolId Target,
    CoreExpression? Receiver,
    CoreExpression[] Arguments) : CoreExpression(Origin, Type);

internal readonly record struct LocalId(int Scope, int Ordinal, string SuggestedName);
