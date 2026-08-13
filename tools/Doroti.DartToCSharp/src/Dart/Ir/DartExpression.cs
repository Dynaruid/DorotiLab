namespace Doroti.DartToCSharp;

internal abstract record DartExpression(SourceOrigin Origin, DartType StaticType);
internal sealed record DartLiteral(SourceOrigin Origin, DartType StaticType, object? Value) : DartExpression(Origin, StaticType);
internal sealed record DartIdentifier(SourceOrigin Origin, DartType StaticType, SymbolId Symbol) : DartExpression(Origin, StaticType);
internal sealed record DartMemberAccess(
    SourceOrigin Origin,
    DartType StaticType,
    DartExpression Receiver,
    SymbolId Member,
    bool IsNullAware = false) : DartExpression(Origin, StaticType);
internal sealed record DartCall(
    SourceOrigin Origin,
    DartType StaticType,
    DartExpression? Receiver,
    SymbolId Target,
    DartArgument[] Arguments,
    bool IsNullAware = false) : DartExpression(Origin, StaticType);
internal sealed record DartAssignment(
    SourceOrigin Origin,
    DartType StaticType,
    DartExpression Target,
    string Operator,
    DartExpression Value) : DartExpression(Origin, StaticType);
internal sealed record DartConditional(
    SourceOrigin Origin,
    DartType StaticType,
    DartExpression Condition,
    DartExpression WhenTrue,
    DartExpression WhenFalse) : DartExpression(Origin, StaticType);
internal sealed record DartAwait(SourceOrigin Origin, DartType StaticType, DartExpression Value) : DartExpression(Origin, StaticType);
internal sealed record DartArgument(string? Name, DartExpression Value);
