namespace Doroti.DartToCSharp;

/// <summary>
/// Compiler-owned Dart AST projection. Migration IR is an external artifact
/// contract; lowering and backends consume this immutable decoded model.
/// </summary>
internal sealed record DartAstNode(
    string Kind,
    string AnalyzerKind,
    string Category,
    SourceOrigin Origin,
    DartType? ResolvedType,
    SymbolId? ResolvedElement,
    IReadOnlyDictionary<string, string?> Properties,
    DartAstNode[] Children)
{
    // Compatibility accessors are intentionally confined to the active
    // strangler lowerer while construct-specific typed nodes are introduced.
    public int Offset => Origin.Offset;
    public int Length => Origin.Length;
    public string? StaticType { get; init; }
    public string? ElementId => ResolvedElement?.Value;
}
