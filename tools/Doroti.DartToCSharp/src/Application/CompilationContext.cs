using System.Collections.ObjectModel;

namespace Doroti.DartToCSharp;

internal sealed record CompilationContext(
    IReadOnlyDictionary<string, CoreResolvedDeclaration[]> GlobalDeclarations,
    FrameworkSemanticIndex SemanticIndex,
    FrameworkAstIndex AstIndex,
    IReadOnlySet<string> GeneratedDeclarationIds)
{
    private static readonly CoreResolvedDeclaration[] NoDeclarations = [];
    public static CompilationContext Empty { get; } = Create(NoDeclarations);

    public static CompilationContext Create(
        IEnumerable<CoreResolvedDeclaration> declarations,
        IEnumerable<CoreResolvedDeclaration>? generatedDeclarations = null)
    {
        var materialized = declarations as CoreResolvedDeclaration[] ?? declarations.ToArray();
        var generatedIds = (generatedDeclarations ?? materialized)
            .Select(declaration => declaration.Element.CanonicalId)
            .ToHashSet(StringComparer.Ordinal);
        var index = materialized
            .GroupBy(declaration => declaration.Name, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(item => item.Element.CanonicalId, StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
        return new(
            new ReadOnlyDictionary<string, CoreResolvedDeclaration[]>(index),
            new FrameworkSemanticIndex(materialized),
            new FrameworkAstIndex(materialized),
            generatedIds);
    }
}

internal sealed record LibraryCompilationContext(
    CompilationContext Compilation,
    string Library,
    CoreResolvedDeclaration[] Declarations);

internal sealed record PassResult<T>(T Value, ConverterDiagnostic[] Diagnostics);
