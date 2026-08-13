namespace Doroti.DartToCSharp;

internal static class DartToCoreLowerer
{
    public static CoreResolvedDeclaration[] Lower(IEnumerable<DartResolvedDeclaration> declarations) => declarations
        .Select(declaration => new CoreResolvedDeclaration(
            declaration.Kind,
            declaration.Name,
            declaration.Offset,
            declaration.Length,
            LowerElement(declaration.Element),
            LowerNode(declaration.Ast),
            declaration.Members.Select(member => new CoreResolvedMember(
                member.Kind,
                member.Name,
                member.Offset,
                member.Length,
                LowerElement(member.Element),
                LowerNode(member.Ast),
                member.IsStatic,
                member.IsFinal,
                member.IsConst,
                member.IsLate,
                member.IsAbstract,
                member.IsGetter,
                member.IsSetter,
                member.IsOperator,
                member.IsFactory)).ToArray()))
        .ToArray();

    private static CoreResolvedElement LowerElement(DartResolvedElement element) => new(
        element.Kind,
        element.Name,
        element.CanonicalId,
        element.Symbol,
        element.IsDeprecated,
        element.Type,
        LowerType(element.ResolvedType),
        element.Supertype,
        element.Mixins,
        element.Interfaces,
        element.TypeParameters?.Select(parameter => new CoreResolvedTypeParameter(
            parameter.Name,
            parameter.Bound,
            LowerType(parameter.ResolvedBound))).ToArray(),
        element.ReturnType,
        LowerType(element.ResolvedReturnType),
        element.Parameters?.Select(parameter => new CoreResolvedParameter(
            parameter.Name,
            parameter.Type,
            LowerType(parameter.ResolvedType)!,
            parameter.Kind,
            parameter.DefaultValue,
            parameter.IsInitializingFormal,
            parameter.IsSuperFormal)).ToArray(),
        element.IsAbstract,
        element.IsPrivate);

    private static CoreAstNode LowerNode(DartAstNode node)
    {
        var runtime = node.ResolvedElement is { } symbol
            ? RuntimeIntrinsicRegistry.Resolve(symbol, node.ResolvedType)?.Intrinsic
            : null;
        var flutter = node.ResolvedElement is { } flutterSymbol
            ? FlutterBindingRegistry.Resolve(flutterSymbol)
            : null;
        return new CoreAstNode(
            CoreNodeKindCatalog.Parse(node.Kind),
            node.Kind,
            node.AnalyzerKind,
            node.Category,
            node.Origin.Through("dart-to-core"),
            LowerType(node.ResolvedType),
            node.ResolvedElement,
            runtime,
            flutter,
            node.Properties,
            node.Children.Select(LowerNode).ToArray())
        {
            StaticType = node.StaticType,
        };
    }

    private static CoreType? LowerType(DartType? type) => type switch
    {
        null => null,
        DartDynamicType => new CoreDynamicType(),
        DartVoidType => new CoreVoidType(),
        DartNeverType => new CoreNeverType(),
        DartNullType => new CoreDynamicType(),
        DartFunctionType function => new CoreFunctionType(
            LowerType(function.ReturnType)!,
            function.Parameters.Select(parameter => LowerType(parameter.Type)!).ToArray(),
            function.Nullability != Nullability.NonNullable),
        DartInterfaceType { Symbol.Name: "bool" } item => new CoreBooleanType(item.ValueNullability != Nullability.NonNullable),
        DartInterfaceType { Symbol.Name: "int" } item => new CoreIntegerType(item.ValueNullability != Nullability.NonNullable),
        DartInterfaceType { Symbol.Name: "double" or "num" } item => new CoreFloatingType(item.ValueNullability != Nullability.NonNullable),
        DartInterfaceType { Symbol.Name: "String" or "string" } item => new CoreStringType(item.ValueNullability != Nullability.NonNullable),
        DartInterfaceType item => new CoreNominalType(
            item.Symbol,
            item.TypeArguments.Select(argument => LowerType(argument)!).ToArray(),
            item.ValueNullability != Nullability.NonNullable),
        DartTypeParameterType item => new CoreNominalType(item.Symbol, [], item.ValueNullability != Nullability.NonNullable),
        DartRecordType item => new CoreNominalType(
            SymbolId.TypeName("Record"),
            item.Positional.Select(field => LowerType(field)!).Concat(item.Named.Values.Select(field => LowerType(field)!)).ToArray(),
            item.ValueNullability != Nullability.NonNullable),
        _ => throw new InvalidDataException($"Unregistered Dart type lowering: {type.GetType().Name}"),
    };
}
