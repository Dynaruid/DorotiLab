namespace Doroti.DartToCSharp;

internal sealed record LoweredMember(
    string Kind,
    string Name,
    int Offset,
    int Length,
    string Source,
    AnalyzerElement? Element,
    MigrationIrStatement[] Statements);

internal sealed record LoweredDeclaration(
    string Kind,
    string Name,
    int Offset,
    int Length,
    string Source,
    AnalyzerElement? Element,
    LoweredMember[] Members);

// Historical C0-C5 fixture compatibility. Product/framework compilation never uses this source-slice path.
internal static class FixtureHistoryLoweringPipeline
{
    public static LoweredDeclaration[] Lower(MigrationIrDeclaration[] declarations, string source) => declarations
        .Select(declaration => new LoweredDeclaration(
            declaration.Kind,
            declaration.Name,
            declaration.Offset,
            declaration.Length,
            Slice(source, declaration.Offset, declaration.Length, declaration.Name),
            declaration.Element,
            declaration.Members.Select(member => new LoweredMember(
                member.Kind,
                member.Name,
                member.Offset,
                member.Length,
                Slice(source, member.Offset, member.Length, $"{declaration.Name}.{member.Name}"),
                member.Element,
                member.Statements)).ToArray()))
        .ToArray();

    private static string Slice(string source, int offset, int length, string symbol)
    {
        if (offset < 0 || length < 0 || offset > source.Length - length)
        {
            throw new InvalidDataException($"Analyzer span for {symbol} is outside its source file: {offset}+{length} of {source.Length}.");
        }

        return source.Substring(offset, length);
    }
}

internal sealed record DartResolvedDeclaration(
    string Kind,
    string Name,
    int Offset,
    int Length,
    DartResolvedElement Element,
    DartAstNode Ast,
    DartResolvedMember[] Members);

internal sealed record DartResolvedMember(
    string Kind,
    string Name,
    int Offset,
    int Length,
    DartResolvedElement Element,
    DartAstNode Ast,
    bool IsStatic,
    bool IsFinal,
    bool IsConst,
    bool IsLate,
    bool IsAbstract,
    bool IsGetter,
    bool IsSetter,
    bool IsOperator,
    bool IsFactory);

internal static class DartIrBuilder
{
    public static DartResolvedDeclaration[] Build(MigrationIrDeclaration[] declarations, string source)
    {
        return declarations.Select(declaration =>
        {
            var analyzerElement = declaration.Element
                ?? throw new InvalidDataException($"Typed framework declaration {declaration.Name} has no resolved element identity.");
            var element = ResolveElement(analyzerElement);
            var ast = declaration.Ast
                ?? throw new InvalidDataException($"Typed framework declaration {declaration.Name} has no AST.");
            var typedAst = BuildNode(ast, source, element.CanonicalId);
            var members = declaration.Members.Select(member => new DartResolvedMember(
                member.Kind,
                member.Name,
                member.Offset,
                member.Length,
                ResolveElement(member.Element ?? throw new InvalidDataException($"Typed framework member {declaration.Name}.{member.Name} has no resolved element identity.")),
                BuildNode(
                    member.Ast ?? throw new InvalidDataException($"Typed framework member {declaration.Name}.{member.Name} has no AST."),
                    source,
                    member.Element!.CanonicalId),
                member.IsStatic,
                member.IsFinal,
                member.IsConst,
                member.IsLate,
                member.IsAbstract,
                member.IsGetter,
                member.IsSetter,
                member.IsOperator,
                member.IsFactory))
                .ToArray();
            return new DartResolvedDeclaration(
                declaration.Kind,
                declaration.Name,
                declaration.Offset,
                declaration.Length,
                element,
                typedAst,
                members);
        }).ToArray();
    }

    private static DartResolvedElement ResolveElement(AnalyzerElement element) => new(
        element.Kind,
        element.Name,
        element.CanonicalId,
        SymbolId.Parse(element.CanonicalId),
        element.IsDeprecated,
        element.Type,
        DecodeOptionalType(element.Type),
        element.Supertype,
        element.Mixins,
        element.Interfaces,
        element.TypeParameters?.Select(parameter => new DartResolvedTypeParameter(
            parameter.Name,
            parameter.Bound,
            DecodeOptionalType(parameter.Bound))).ToArray(),
        element.ReturnType,
        DecodeOptionalType(element.ReturnType),
        element.Parameters?.Select(parameter => new DartResolvedParameter(
            parameter.Name,
            parameter.Type,
            DartTypeDecoder.Decode(parameter.Type),
            parameter.Kind,
            parameter.DefaultValue,
            parameter.IsInitializingFormal,
            parameter.IsSuperFormal)).ToArray(),
        element.IsAbstract,
        element.IsPrivate);

    private static DartType? DecodeOptionalType(string? value) =>
        string.IsNullOrWhiteSpace(value) || value == "InvalidType" ? null : DartTypeDecoder.Decode(value);

    private static DartAstNode BuildNode(MigrationIrNode node, string source, string fallbackElementId)
    {
        DartType? resolvedType = null;
        if (!string.IsNullOrWhiteSpace(node.StaticType) && node.StaticType is not "dynamic" and not "Type" and not "InvalidType")
        {
            try
            {
                resolvedType = DartTypeDecoder.Decode(node.StaticType);
            }
            catch (DartTypeDecodeException exception)
            {
                throw new InvalidDataException(
                    $"Analyzer type '{node.StaticType}' could not be decoded at {source}:{node.Offset}+{node.Length}.",
                    exception);
            }
        }
        SymbolId? symbol = null;
        if (!string.IsNullOrWhiteSpace(node.ElementId) && node.ElementId.Contains('#', StringComparison.Ordinal))
        {
            symbol = SymbolId.Parse(node.ElementId);
        }
        var originSymbol = symbol ?? SymbolId.Parse(fallbackElementId);
        return new DartAstNode(
            node.Kind,
            node.AnalyzerKind,
            node.Category,
            new SourceOrigin(source, node.Offset, node.Length, originSymbol),
            resolvedType,
            symbol,
            new Dictionary<string, string?>(node.Properties, StringComparer.Ordinal),
            node.Children.Select(child => BuildNode(child, source, fallbackElementId)).ToArray())
        {
            StaticType = node.StaticType,
        };
    }
}
