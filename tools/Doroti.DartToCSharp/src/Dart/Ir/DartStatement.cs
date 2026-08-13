namespace Doroti.DartToCSharp;

internal abstract record DartStatement(SourceOrigin Origin);
internal sealed record DartBlock(SourceOrigin Origin, DartStatement[] Statements) : DartStatement(Origin);
internal sealed record DartExpressionStatement(SourceOrigin Origin, DartExpression Expression) : DartStatement(Origin);
internal sealed record DartReturn(SourceOrigin Origin, DartExpression? Expression) : DartStatement(Origin);
internal sealed record DartIf(
    SourceOrigin Origin,
    DartExpression Condition,
    DartStatement Then,
    DartStatement? Otherwise) : DartStatement(Origin);
internal sealed record DartWhile(SourceOrigin Origin, DartExpression Condition, DartStatement Body) : DartStatement(Origin);
internal sealed record DartBreak(SourceOrigin Origin, string? Label) : DartStatement(Origin);
internal sealed record DartContinue(SourceOrigin Origin, string? Label) : DartStatement(Origin);
internal sealed record DartThrow(SourceOrigin Origin, DartExpression Expression) : DartStatement(Origin);
