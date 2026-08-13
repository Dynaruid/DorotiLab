namespace Doroti.DartToCSharp;

internal abstract record CoreStatement(SourceOrigin Origin);
internal sealed record CoreBlock(SourceOrigin Origin, CoreStatement[] Statements) : CoreStatement(Origin);
internal sealed record CoreReturn(SourceOrigin Origin, CoreExpression? Value) : CoreStatement(Origin);
internal sealed record CoreExpressionStatement(SourceOrigin Origin, CoreExpression Expression) : CoreStatement(Origin);
internal sealed record CoreIf(
    SourceOrigin Origin,
    CoreExpression Condition,
    CoreStatement Then,
    CoreStatement? Otherwise) : CoreStatement(Origin);
