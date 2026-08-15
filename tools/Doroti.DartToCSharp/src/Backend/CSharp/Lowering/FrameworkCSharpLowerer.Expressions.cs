using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void LowerExpression(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        using var syntaxRegion = builder.BeginRegion(CsSyntaxRegionKind.Expression, ToCsOrigin(node.Origin));
        if (string.Equals(node.ElementId, "dart:math#pi", StringComparison.Ordinal))
        {
            builder.Append("Dart_mathLibrary.pi");
            return;
        }
        switch (node.Kind)
        {
            case CoreNodeKind.SimpleIdentifier:
                if (node.Text(CoreProperty.name) == "debugFormatDouble" &&
                    node.StaticType?.Contains("Function", StringComparison.Ordinal) == true)
                {
                    builder.Append("(value => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugFormatDouble(value))");
                    return;
                }
                EmitIdentifier(builder, node, declaration, library);
                return;
            case CoreNodeKind.BooleanLiteral:
            case CoreNodeKind.IntegerLiteral:
                {
                    var literal = node.Text(CoreProperty.value) ?? "0";
                    builder.Append(literal);
                    if (long.TryParse(literal, out var number) &&
                        (number > int.MaxValue || number < int.MinValue ||
                         string.Equals(node.StaticType, "int", StringComparison.Ordinal)))
                    {
                        builder.Append('L');
                    }
                    return;
                }
            case CoreNodeKind.DoubleLiteral:
                builder.Append(node.Text(CoreProperty.value));
                return;
            case CoreNodeKind.SimpleStringLiteral:
                builder.Append('"').Append(Escape(node.Text(CoreProperty.value) ?? string.Empty)).Append('"');
                return;
            case CoreNodeKind.NullLiteral:
                builder.Append("null");
                return;
            case CoreNodeKind.ThisExpression:
                builder.Append(_session.ExplicitThisExpression ?? "this");
                return;
            case CoreNodeKind.SuperExpression:
                builder.Append("base");
                return;
            case CoreNodeKind.ParenthesizedExpression:
                if (node.Child(CoreChildRole.expressionOffset) is { } parenthesized)
                {
                    if (parenthesized.Kind == CoreNodeKind.ThrowExpression)
                    {
                        LowerExpression(builder, parenthesized, declaration, package, library, inputPath, diagnostics);
                        return;
                    }
                    builder.Append('(');
                    LowerExpression(builder, parenthesized, declaration, package, library, inputPath, diagnostics);
                    builder.Append(')');
                }
                return;
            case CoreNodeKind.ConditionalExpression:
                EmitConditional(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.InstanceCreationExpression:
                EmitInstanceCreation(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.ListLiteral:
                EmitListLiteral(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.SetOrMapLiteral:
                EmitSetOrMapLiteral(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.StringInterpolation:
                EmitStringInterpolation(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.NamedExpression:
                if (node.Children.FirstOrDefault(item => item.Category == "expression" && item.Kind != CoreNodeKind.Label) is { } namedValue)
                {
                    LowerExpression(builder, namedValue, declaration, package, library, inputPath, diagnostics);
                    return;
                }
                break;
            case CoreNodeKind.AssignmentExpression:
                {
                    var op = node.Text(CoreProperty.@operator);
                    var left = node.Child(CoreChildRole.leftOffset);
                    var right = node.Child(CoreChildRole.rightOffset);
                    if (op == "??=" && left is not null && right is not null && left.Kind == CoreNodeKind.SimpleIdentifier)
                    {
                        var leftName = left.Text(CoreProperty.name);
                        var leftType = left.StaticType;
                        if (string.IsNullOrEmpty(leftType) && declaration.Element.Parameters is not null)
                        {
                            var parameter = declaration.Element.Parameters.FirstOrDefault(item => item.Name == leftName);
                            if (parameter is not null)
                            {
                                leftType = parameter.Type;
                            }
                        }
                        if ((leftType ?? "").TrimEnd('?') is "int" or "Integer")
                        {
                            builder.Append("if (");
                            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                            builder.Append(" == -1) ");
                            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                            builder.Append(" = ");
                            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                    }
                    if (op == "??=" && left is not null && right is not null && left.Kind == CoreNodeKind.IndexExpression)
                    {
                        var indexParts = left.Children.Where(item => item.Category == "expression").ToArray();
                        var mapType = indexParts.FirstOrDefault()?.StaticType ?? string.Empty;
                        if (indexParts.Length >= 2 &&
                            (mapType.Contains("Map<", StringComparison.Ordinal) ||
                             mapType.Contains("Dictionary<", StringComparison.Ordinal)))
                        {
                            LowerExpression(builder, indexParts[0], declaration, package, library, inputPath, diagnostics);
                            builder.Append(".putIfAbsent(");
                            LowerExpression(builder, indexParts[1], declaration, package, library, inputPath, diagnostics);
                            builder.Append(", () => ");
                            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                            return;
                        }
                    }
                    EmitBinaryLike(builder, node, declaration, package, library, inputPath, diagnostics);
                    return;
                }
            case CoreNodeKind.BinaryExpression:
                {
                    var op = node.Text(CoreProperty.@operator);
                    if (op is "==" or "!=")
                    {
                        EmitEqualityComparison(builder, node, declaration, package, library, inputPath, diagnostics);
                        return;
                    }
                    var operands = node.Children.Where(item => item.Category == "expression").ToArray();
                    if (op == "*" && operands.Length >= 2 && operands[0].StaticType == "String")
                    {
                        builder.Append("DartCoreExtensions.repeat(");
                        LowerExpression(builder, operands[0], declaration, package, library, inputPath, diagnostics);
                        builder.Append(", ");
                        LowerExpression(builder, operands[1], declaration, package, library, inputPath, diagnostics);
                        builder.Append(')');
                        return;
                    }
                    builder.Append('(');
                    EmitBinaryLike(builder, node, declaration, package, library, inputPath, diagnostics);
                    builder.Append(')');
                    return;
                }
            case CoreNodeKind.IfNullExpression:
                {
                    var left = node.Children.FirstOrDefault(item => item.Category == "expression");
                    var right = node.Children.Skip(1).FirstOrDefault(item => item.Category == "expression");
                    if (left is not null && right is not null)
                    {
                        if (DescendantsAndSelf(left).Any(item => item.Text(CoreProperty.name) == "index"))
                        {
                            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                        var mappedLeft = MapType(left.StaticType ?? "object");
                        if ((IsValueType(mappedLeft) && !mappedLeft.EndsWith("?", StringComparison.Ordinal)) ||
                            DescendantsAndSelf(left).Any(item => item.Text(CoreProperty.name) == "isInScribbleRect"))
                        {
                            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                        var mappedRight = MapType(right.StaticType ?? "object");
                        if ((MapType(node.StaticType ?? "object").TrimEnd('?') is "object" or "dynamic") &&
                            mappedLeft.TrimEnd('?') != mappedRight.TrimEnd('?'))
                        {
                            builder.Append("((object?)");
                            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                            builder.Append(" ?? (object?)");
                            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                            return;
                        }
                        builder.Append('(');
                        LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                        builder.Append(" ?? ");
                        LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                        builder.Append(')');
                        return;
                    }
                    break;
                }
            case CoreNodeKind.FunctionReference:
                {
                    var target = node.Children.FirstOrDefault(item => item.Category == "expression");
                    if (target is null)
                    {
                        AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                            "function-reference-target", "Provide the resolved function tear-off target.");
                        builder.Append("default!");
                        return;
                    }
                    LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                    return;
                }
            case CoreNodeKind.PrefixedIdentifier:
                EmitPrefixedIdentifier(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.DotShorthandPropertyAccess:
                EmitDotShorthandPropertyAccess(builder, node);
                return;
            case CoreNodeKind.PropertyAccess:
                EmitPropertyAccess(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.IndexExpression:
                EmitIndexExpression(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.MethodInvocation:
                EmitMethodInvocation(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.FunctionExpressionInvocation:
                {
                    var function = node.Child(CoreChildRole.functionOffset);
                    var arguments = node.Child(CoreChildRole.argumentsOffset);
                    if (function?.Kind == CoreNodeKind.ConstructorReference &&
                        DescendantsAndSelf(function).Any(item => item.Text(CoreProperty.name)?.Contains("GlobalKey", StringComparison.Ordinal) == true))
                    {
                        var constructorName = function.Child(CoreChildRole.constructorOffset) ??
                            function.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ConstructorName);
                        var globalKeyType = constructorName is null ? "GlobalKey<IState>" : MapTypeFromAst(constructorName);
                        builder.Append(globalKeyType).Append(".Create(");
                        EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics, preserveNames: false);
                        builder.Append(')');
                        return;
                    }
                    if (function is not null)
                    {
                        var requiresDelegateCast = function.StaticType?.Contains(" Function", StringComparison.Ordinal) == true &&
                            function.Kind is CoreNodeKind.FunctionExpression or CoreNodeKind.SwitchExpression;
                        if (requiresDelegateCast)
                        {
                            builder.Append("((").Append(MapType(function.StaticType ?? "object Function()")).Append(")");
                        }
                        LowerExpression(builder, function, declaration, package, library, inputPath, diagnostics);
                        if (requiresDelegateCast)
                        {
                            builder.Append(')');
                        }
                    }
                    builder.Append('(');
                    var functionArguments = arguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
                    if (function?.Text(CoreProperty.name) == "successCallback" && functionArguments.Length == 2)
                    {
                        LowerExpression(builder, functionArguments[0], declaration, package, library, inputPath, diagnostics);
                        builder.Append(", (__exception, __stack) => { _ = ");
                        LowerExpression(builder, functionArguments[1], declaration, package, library, inputPath, diagnostics);
                        builder.Append("(__exception, __stack); }");
                    }
                    else if (function?.StaticType?.Contains("bool", StringComparison.Ordinal) == true &&
                        functionArguments.Any(item => item.Kind == CoreNodeKind.NamedExpression && item.Text(CoreProperty.name) == "allowUpscaling"))
                    {
                        var orderedNames = new[] { "allowUpscaling", "cacheHeight", "cacheWidth" };
                        var positional = functionArguments.FirstOrDefault(item => item.Kind != CoreNodeKind.NamedExpression);
                        if (positional is not null)
                        {
                            LowerExpression(builder, positional, declaration, package, library, inputPath, diagnostics);
                        }
                        foreach (var orderedName in orderedNames)
                        {
                            builder.Append(", ");
                            var named = functionArguments.FirstOrDefault(item =>
                                item.Kind == CoreNodeKind.NamedExpression && item.Text(CoreProperty.name) == orderedName);
                            var value = named?.Children.FirstOrDefault(item => item.Category == "expression");
                            if (value is null) builder.Append(orderedName == "allowUpscaling" ? "false" : "null");
                            else LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
                        }
                    }
                    else
                    {
                        // Custom delegates retain Dart named-parameter names.
                        // Preserve those names at indirect invocation sites so
                        // CLR delegate declaration order does not reorder Dart's
                        // named arguments.
                        if ((function?.Text(CoreProperty.name) == "onStateChange" ||
                             declaration.Name == "AppLifecycleListener" && functionArguments.Length == 1 &&
                             MapType(functionArguments[0].StaticType ?? string.Empty).TrimEnd('?').EndsWith("AppLifecycleState", StringComparison.Ordinal)) &&
                            functionArguments.Length == 1)
                        {
                            builder.Append("DartRuntimePrimitives.RequireValue(");
                            LowerExpression(builder, functionArguments[0], declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else
                        {
                            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics, preserveNames: true);
                        }
                    }
                    builder.Append(')');
                    return;
                }
            case CoreNodeKind.FunctionExpression:
                EmitFunctionExpression(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.AsExpression:
                {
                    var expression = node.Child(CoreChildRole.expressionOffset);
                    var typeNode = node.Child(CoreChildRole.typeOffset);
                    if (expression is not null && typeNode is not null)
                    {
                        var targetType = MapTypeFromAst(typeNode);
                        if ((_session.ActiveDonorDeclaration ?? declaration).Name == "IndexedSlot" &&
                            targetType.StartsWith("IndexedSlot<", StringComparison.Ordinal))
                        {
                            targetType = "IndexedSlot<T>";
                        }
                        var nonNullableTargetType = targetType.TrimEnd('?');
                        var mappedSourceType = MapType(expression.StaticType ?? string.Empty);
                        var sourceType = mappedSourceType.TrimEnd('?');
                        if ((nonNullableTargetType == "Size" || nonNullableTargetType.EndsWith(".Size", StringComparison.Ordinal)) &&
                            (sourceType == "Offset" || sourceType.EndsWith(".Offset", StringComparison.Ordinal)))
                        {
                            builder.Append("global::Doroti.Ui.Size.fromOffset(DartRuntimePrimitives.RequireValue(");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append("))");
                            return;
                        }
                        if ((nonNullableTargetType == "Offset" || nonNullableTargetType.EndsWith(".Offset", StringComparison.Ordinal)) &&
                            (sourceType == "Size" || sourceType.EndsWith(".Size", StringComparison.Ordinal)))
                        {
                            builder.Append("global::Doroti.Ui.Offset.fromSize(DartRuntimePrimitives.RequireReference(");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append("))");
                            return;
                        }
                        if (nonNullableTargetType is "Size" or "Offset" ||
                            nonNullableTargetType.EndsWith(".Size", StringComparison.Ordinal) ||
                            nonNullableTargetType.EndsWith(".Offset", StringComparison.Ordinal))
                        {
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                        if (targetType.StartsWith("DartMap<", StringComparison.Ordinal))
                        {
                            var nonNullableMapType = targetType.TrimEnd('?');
                            var mapArguments = SplitGenericArguments(nonNullableMapType[8..^1]);
                            builder.Append("DartRuntimePrimitives.ConvertMap<").Append(string.Join(", ", mapArguments)).Append(">((System.Collections.IDictionary)");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else if (IsValueType(targetType))
                        {
                            builder.Append("((").Append(targetType).Append(")");
                            if (IsUnboundTypeParameterName(sourceType)) builder.Append("(object)");
                            if (!targetType.EndsWith("?", StringComparison.Ordinal) &&
                                mappedSourceType.EndsWith("?", StringComparison.Ordinal))
                            {
                                builder.Append("DartRuntimePrimitives.RequireValue(");
                            }
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            if (!targetType.EndsWith("?", StringComparison.Ordinal) &&
                                mappedSourceType.EndsWith("?", StringComparison.Ordinal))
                            {
                                builder.Append(')');
                            }
                            builder.Append(')');
                        }
                        else
                        {
                            var castType = MakeNullable(targetType);
                            builder.Append("((").Append(castType).Append(")(object?)");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(")!");
                        }
                        return;
                    }
                    break;
                }
            case CoreNodeKind.IsExpression:
                {
                    var expression = node.Child(CoreChildRole.expressionOffset);
                    var typeNode = node.Child(CoreChildRole.typeOffset);
                    var isNot = node.Text(CoreProperty.isNot) == "true";
                    if (expression is not null && typeNode is not null)
                    {
                        var mappedIsType = MapTypeFromAst(typeNode).TrimEnd('?');
                        if (mappedIsType == "dynamic" &&
                            typeNode.Text(CoreProperty.name) is { } declaredTypeName &&
                            declaredTypeName != "dynamic")
                        {
                            var declaredArguments = typeNode.Children
                                .FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList)?
                                .Children.Where(item => item.Category == "type")
                                .Select(MapTypeFromAst)
                                .ToArray() ?? [];
                            mappedIsType = ResolveEmittedTypeName(typeNode, declaredTypeName) +
                                (declaredArguments.Length == 0
                                    ? string.Empty
                                    : $"<{string.Join(", ", declaredArguments)}>");
                        }
                        if (!mappedIsType.Contains('<', StringComparison.Ordinal) &&
                            FindGlobalDeclaration(mappedIsType) is { Element.TypeParameters.Length: > 0 } rawGenericType)
                        {
                            mappedIsType += $"<{string.Join(", ", rawGenericType.Element.TypeParameters.Select(_ => "object"))}>";
                        }
                        var isClipStorage = expression.Text(CoreProperty.name) == "_clip" ||
                            DescendantsAndSelf(expression).Any(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                                item.Text(CoreProperty.name) == "_clip");
                        if ((mappedIsType is "null" or "Null" || typeNode.Text(CoreProperty.name) == "Null") && isClipStorage)
                        {
                            if (isNot) builder.Append('!');
                            builder.Append("object.Equals(");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(", null)");
                            return;
                        }
                        var mappedExpressionType = MapType(expression.StaticType ?? string.Empty).TrimEnd('?');
                        if (mappedExpressionType is "Type" or "System.Type" or "global::System.Type")
                        {
                            if (isNot) builder.Append('!');
                            builder.Append("object.Equals(");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(", typeof(").Append(mappedIsType).Append("))");
                            return;
                        }
                        if (string.Equals(mappedExpressionType, mappedIsType, StringComparison.Ordinal))
                        {
                            builder.Append(isNot ? "false" : "true");
                            return;
                        }
                        if (mappedIsType == "Enum" &&
                            (MapType(expression.StaticType ?? string.Empty).TrimEnd('?').StartsWith('_') ||
                             FindGlobalDeclaration(StripLibraryPrefix((expression.StaticType ?? string.Empty).TrimEnd('?')))?.Ast.Kind == CoreNodeKind.EnumDeclaration))
                        {
                            builder.Append(isNot ? "false" : "true");
                            return;
                        }
                        builder.Append('(');
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.Append(isNot ? " is not " : " is ").Append(mappedIsType);
                        builder.Append(')');
                        return;
                    }
                    break;
                }
            case CoreNodeKind.TypeLiteral:
                {
                    var typeNode = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                    if (typeNode is not null)
                    {
                        var literalName = typeNode.Text(CoreProperty.name);
                        var owningDeclaration = _session.ActiveDonorDeclaration ?? declaration;
                        var isRuntimeTypeParameter = !string.IsNullOrEmpty(literalName) &&
                            owningDeclaration.Members
                                .Where(member => ContainsOffset(member.Ast, node.Offset))
                                .SelectMany(member => member.Element.Parameters ?? [])
                                .Any(parameter => parameter.Name == literalName &&
                                    MapType(parameter.Type) is "Type" or "System.Type" or "global::System.Type");
                        if (isRuntimeTypeParameter)
                        {
                            // The analyzer represents a value of Dart `Type`
                            // as a TypeLiteral in several reflective APIs. A
                            // method parameter is already a System.Type value;
                            // `typeof(parameter)` is invalid C#.
                            builder.Append(SafeIdentifier(literalName!));
                            return;
                        }
                        var literalType = MapTypeFromAst(typeNode);
                        builder.Append("typeof(").Append(literalType.TrimEnd('?') == "dynamic" ? "object" : literalType).Append(')');
                        return;
                    }
                    break;
                }
            case CoreNodeKind.PrefixExpression:
                {
                    var operand = node.Child(CoreChildRole.operandOffset);
                    var op = node.Text(CoreProperty.@operator) ?? "!";
                    if (operand is null)
                    {
                        break;
                    }
                    if (op == "!")
                    {
                        builder.Append('!');
                        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
                    }
                    else
                    {
                        builder.Append(MapOperator(op));
                        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
                    }
                    return;
                }
            case CoreNodeKind.PostfixExpression:
                {
                    var operand = node.Child(CoreChildRole.operandOffset);
                    var op = node.Text(CoreProperty.@operator) ?? "++";
                    if (operand is null)
                    {
                        break;
                    }
                    if (op == "!" &&
                        (IsValueType(MapType(node.StaticType ?? operand.StaticType ?? "object").TrimEnd('?')) ||
                         HasNullableValueStorage(operand, _session.ActiveDonorDeclaration ?? declaration)))
                    {
                        builder.Append("DartRuntimePrimitives.RequireValue(");
                        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
                        builder.Append(')');
                    }
                    else if (op == "!" && operand.Kind == CoreNodeKind.SimpleIdentifier && operand.Text(CoreProperty.name) == "state")
                    {
                        builder.Append("DartRuntimePrimitives.RequireValue(");
                        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
                        builder.Append(')');
                    }
                    else
                    {
                        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
                        builder.Append(MapOperator(op));
                    }
                    return;
                }
            case CoreNodeKind.AdjacentStrings:
                {
                    var values = node.Children.Where(item => item.Kind is CoreNodeKind.SimpleStringLiteral or CoreNodeKind.StringInterpolation).ToArray();
                    for (var index = 0; index < values.Length; index++)
                    {
                        if (index > 0)
                        {
                            builder.Append(" + ");
                        }
                        LowerExpression(builder, values[index], declaration, package, library, inputPath, diagnostics);
                    }
                    return;
                }
            case CoreNodeKind.CascadeExpression:
                EmitCascade(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.AwaitExpression:
                {
                    var expression = node.Child(CoreChildRole.expressionOffset);
                    if (expression?.Kind == CoreNodeKind.NullLiteral)
                    {
                        builder.Append("await Task.Yield()");
                        return;
                    }
                    var handlerInvocation = expression?.Kind == CoreNodeKind.FunctionExpressionInvocation &&
                        DescendantsAndSelf(expression).Any(item => item.Kind == CoreNodeKind.SimpleIdentifier && item.Text(CoreProperty.name) == "handler") &&
                        declaration.Name is "BasicMessageChannel" or "MethodChannel";
                    if (handlerInvocation)
                    {
                        var resultType = MapType(node.StaticType ?? "object");
                        builder.Append("((").Append(resultType).Append(")await DartAsyncRuntime.AwaitObject(");
                        LowerExpression(builder, expression!, declaration, package, library, inputPath, diagnostics);
                        builder.Append("))!");
                        return;
                    }
                    var expressionType = expression?.StaticType ?? string.Empty;
                    var futureOr = expressionType.StartsWith("FutureOr<", StringComparison.Ordinal);
                    var awaitable = expressionType == "Future" || expressionType.StartsWith("Future<", StringComparison.Ordinal) || futureOr;
                    if (awaitable) builder.Append("await ");
                    if (futureOr) builder.Append("DartAsyncRuntime.AwaitFutureOrValue<").Append(MapGenericArgument(node.StaticType ?? "object")).Append(">(");
                    if (expression is not null)
                    {
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    }
                    if (futureOr) builder.Append(')');
                    return;
                }
            case CoreNodeKind.ThrowExpression:
                {
                    var expression = node.Children.FirstOrDefault(item => item.Category == "expression");
                    builder.Append("throw");
                    if (expression is not null)
                    {
                        builder.Append(' ');
                        var thrownType = MapType(expression.StaticType ?? string.Empty).TrimEnd('?');
                        var wrapThrownValue = thrownType is "object" or "dynamic" ||
                            !thrownType.EndsWith("Exception", StringComparison.Ordinal);
                        if (wrapThrownValue) builder.Append("DartRuntimePrimitives.AsException(");
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        if (wrapThrownValue) builder.Append(')');
                    }
                    return;
                }
            case CoreNodeKind.RethrowExpression:
                builder.Append("throw");
                return;
            case CoreNodeKind.SwitchExpression:
                EmitSwitchExpression(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.RecordLiteral:
                EmitRecordLiteral(builder, node, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.PatternAssignment:
                {
                    var pattern = node.Children.FirstOrDefault(item => item.Category == "pattern");
                    var expression = node.Children.FirstOrDefault(item => item.Category == "expression");
                    if (pattern is null || expression is null)
                    {
                        break;
                    }
                    EmitDeconstructionPattern(builder, pattern, declaration);
                    builder.Append(" = ");
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    return;
                }
            case CoreNodeKind.ConstructorReference:
                EmitConstructorReference(builder, node);
                return;
        }

        AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
            "expression", "Add a typed expression visitor for this AST node before selecting the containing library.");
        builder.Append("throw new NotSupportedException(\"DOTF0001\")");
    }

    private string MapOperator(string op) => op switch
    {
        "==" => "==",
        "!=" => "!=",
        "&&" => "&&",
        "||" => "||",
        "|" => "|",
        "&" => "&",
        "^" => "^",
        "<<" => "<<",
        ">>" => ">>",
        ">>>" => ">>>",
        "+" => "+",
        "-" => "-",
        "*" => "*",
        "/" => "/",
        "~/" => "/",
        "%" => "%",
        "<" => "<",
        ">" => ">",
        "<=" => "<=",
        ">=" => ">=",
        "!" => "!",
        "++" => "++",
        "--" => "--",
        "??" => "??",
        _ => op,
    };

    private void EmitIdentifier(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string currentLibrary)
    {
        var name = node.Text(CoreProperty.name) ?? "missing";
        var elementId = node.ElementId;
        if (name == "renderObject" &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "SliverMultiBoxAdaptorElement")
        {
            builder.Append("((dynamic)this.renderObject)");
            return;
        }
        if (name == "simulation" && declaration.Name == "DrivenScrollActivity" &&
            _session.ExplicitThisExpression is not null)
        {
            builder.Append("simulation");
            return;
        }
        if (name == "kTouchSlop")
        {
            builder.Append("global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop");
            return;
        }
        if (name == "kLongPressTimeout")
        {
            builder.Append("global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kLongPressTimeout");
            return;
        }
        if (name == "defaultTargetPlatform")
        {
            builder.Append("global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform");
            return;
        }
        if (name == "PlatformSelectableRegionContextMenu")
        {
            builder.Append("PlatformSelectableRegionContextMenuIo");
            return;
        }
        if (name == "_disabledGeometry" &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "_SelectionContainerState")
        {
            builder.Append(MapType((_session.ActiveDonorDeclaration ?? declaration).Name))
                .Append("._disabledGeometry");
            return;
        }
        if (name == "kLongPressTimeout")
        {
            builder.Append("global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kLongPressTimeout");
            return;
        }
        if (name == "window_impl" &&
            string.Equals(currentLibrary, "package:flutter/src/widgets/_window.dart", StringComparison.Ordinal))
        {
            builder.Append("_window_ioLibrary");
            return;
        }
        if (name is "InheritedModel" or "ModalRoute" &&
            (elementId is null || !elementId.Contains("@local", StringComparison.Ordinal)))
        {
            builder.Append(MapStaticOwnerType(name, declaration));
            return;
        }
        if (elementId?.EndsWith("#SemanticsBinding._instance", StringComparison.Ordinal) == true)
        {
            builder.Append("global::Doroti.Generated.Framework.Semantics.SemanticsBinding.")
                .Append(SafeIdentifier(name));
            return;
        }
        if (name == "size" &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "_RenderTheater")
        {
            builder.Append("this.size");
            return;
        }
        if (elementId?.LastIndexOf("@local", StringComparison.Ordinal) is >= 0)
        {
            var emittedName = EmittedLocalIdentifier(node, name);
            var owningDeclaration = _session.ActiveDonorDeclaration ?? declaration;
            if (!_session.EmittingAssignmentLeft && NeedsReferenceTypePromotion(node, owningDeclaration))
            {
                builder.Append("((").Append(MapType(node.StaticType!)).Append(')').Append(emittedName).Append(')');
            }
            else if (!_session.EmittingAssignmentLeft && NeedsNullableValuePromotion(node, owningDeclaration))
            {
                builder.Append("DartRuntimePrimitives.RequireValue(").Append(emittedName).Append(')');
            }
            else
            {
                builder.Append(emittedName);
            }
            return;
        }
        if (elementId is null)
        {
            var recoveredLocal = EmittedLocalIdentifier(node, name);
            if (!string.Equals(recoveredLocal, SafeIdentifier(name), StringComparison.Ordinal))
            {
                builder.Append(recoveredLocal);
                return;
            }
        }
        // A Dart member named `value` is common in controller setters. C# also
        // introduces an implicit setter parameter named `value`; leaving the
        // resolved member unqualified silently rebinds the assignment to that
        // parameter. Element identity distinguishes the member from an actual
        // Dart parameter/local, so keep the member receiver explicit.
        if (name == "value" &&
            (FindGlobalMember(elementId) is { IsStatic: false } ||
             elementId is null &&
             AssignmentStorageType(_session.ActiveDonorDeclaration ?? declaration, name, null) is not null))
        {
            builder.Append(_session.ExplicitThisExpression ?? "this").Append(".value");
            return;
        }
        var identifierOwner = _session.ActiveDonorDeclaration ?? declaration;
        if (DescendantsAndSelf(identifierOwner.Ast).Any(item =>
                item.Kind == CoreNodeKind.FunctionDeclaration &&
                string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal)))
        {
            builder.Append(SafeIdentifier(name));
            return;
        }
        if (name == "SemanticsBinding")
        {
            builder.Append("global::Doroti.Generated.Framework.Semantics.SemanticsBinding");
            return;
        }
        if (name is "PluginUtilities" or "ViewFocusState" or "ViewFocusDirection")
        {
            builder.Append("global::Doroti.Ui.").Append(name);
            return;
        }
        if (name == "hashCode")
        {
            builder.Append("GetHashCode()");
            return;
        }
        if (name == declaration.Name && node.StaticType == "Type" &&
            declaration.Element.TypeParameters is { Length: > 0 } currentTypeParameters)
        {
            builder.Append("typeof(").Append(EmittedTypeName(currentLibrary, declaration.Name))
                .Append('<')
                .Append(string.Join(", ", currentTypeParameters.Select(item => SafeIdentifier(item.Name))))
                .Append(">)");
            return;
        }
        if (name == "RenderViewportBase" && node.StaticType == "Type" &&
            declaration.Element.Supertype is { } viewportBase &&
            StripLibraryPrefix(viewportBase).StartsWith("RenderViewportBase<", StringComparison.Ordinal))
        {
            builder.Append(MapType(viewportBase));
            return;
        }
        if (_session.ActiveDonorDeclaration is { } donorDeclaration &&
            donorDeclaration.Members.Any(member =>
                (member.Element.Parameters ?? []).Any(parameter =>
                    string.Equals(parameter.Name, name, StringComparison.Ordinal) &&
                    (string.Equals(elementId, member.Element.CanonicalId + "." + parameter.Name, StringComparison.Ordinal) ||
                     ContainsOffset(member.Ast, node.Offset) ||
                     (_session.ExplicitThisExpression is not null &&
                      member.Kind == "constructor" &&
                      (elementId is null || !elementId.Contains("#" + donorDeclaration.Name + "." + name, StringComparison.Ordinal)))))))
        {
            // Resolved method parameters use an owner-qualified canonical ID,
            // not the @local form. Check them before donor fields so a mixin
            // getter such as `scale` cannot capture copyWith(scale: ...).
            builder.Append(SafeIdentifier(name));
            return;
        }
        if (_session.ActiveDonorDeclaration is { } donorWithMember &&
            donorWithMember.Members.FirstOrDefault(member =>
                string.Equals(member.Name, name, StringComparison.Ordinal)) is { } donorMember)
        {
            if (donorMember.IsStatic)
            {
                builder.Append(EmittedTypeName(
                        LibraryUriFromElementId(donorWithMember.Element.CanonicalId),
                        donorWithMember.Name))
                    .Append('.').Append(SafeIdentifier(name));
            }
            else
            {
                builder.Append(_session.ExplicitThisExpression ?? "this").Append('.').Append(SafeIdentifier(name));
            }
            return;
        }
        if (declaration.Ast.Kind == CoreNodeKind.EnumDeclaration &&
            _session.ExplicitThisExpression is { } enumReceiver && name == "name")
        {
            builder.Append(enumReceiver).Append(".ToString()");
            return;
        }
        if (_session.ActiveSourceLibrary is { } sourceLibrary &&
            _semanticIndex.FindDeclaration(sourceLibrary, name) is { Ast.Kind: CoreNodeKind.FunctionDeclaration or CoreNodeKind.TopLevelVariableDeclaration })
        {
            builder.Append(QualifiedLibraryStaticClassName(sourceLibrary, currentLibrary))
                .Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (declaration.Members.Any(member =>
            !member.IsStatic && string.Equals(member.Element.CanonicalId, elementId, StringComparison.Ordinal)))
        {
            builder.Append(_session.ExplicitThisExpression ?? "this").Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (FindGlobalMember(elementId) is { IsStatic: false } inheritedMember &&
            FindDeclaringDeclaration(inheritedMember) is { } inheritedOwner &&
            IsDescendantOf(declaration, inheritedOwner))
        {
            builder.Append(_session.ExplicitThisExpression ?? "this").Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (_semanticIndex.DeclarationsBySimpleName.TryGetValue(name, out var namedDeclarations) &&
            namedDeclarations.FirstOrDefault(candidate =>
                candidate.Ast.Kind == CoreNodeKind.TopLevelVariableDeclaration) is { } topLevelVariable)
        {
            var ownerLibrary = LibraryUriFromElementId(topLevelVariable.Element.CanonicalId);
            builder.Append(QualifiedLibraryStaticClassName(ownerLibrary, currentLibrary))
                .Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (_session.ExplicitEnumDeclaration is { } explicitEnum &&
            explicitEnum.Ast.Kind == CoreNodeKind.EnumDeclaration &&
            elementId?.StartsWith(explicitEnum.Element.CanonicalId + ".", StringComparison.Ordinal) == true)
        {
            builder.Append(EmittedTypeName(LibraryUriFromElementId(explicitEnum.Element.CanonicalId), explicitEnum.Name))
                .Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (name == "ui" && elementId?.EndsWith("#ui", StringComparison.Ordinal) == true)
        {
            builder.Append("Dart_uiLibrary");
            return;
        }
        if (name == "kLongPressTimeout" &&
            elementId?.Contains("/gestures/constants.dart#kLongPressTimeout", StringComparison.Ordinal) == true)
        {
            builder.Append("FoundationRuntimePorts.kLongPressTimeout");
            return;
        }
        if (name == "Timer" && string.Equals(LibraryUriFromElementId(elementId), "dart:async", StringComparison.Ordinal))
        {
            builder.Append("global::Doroti.Runtime.Timer");
            return;
        }
        if (name == "Uri" && node.StaticType == "Type")
        {
            builder.Append("DartUri");
            return;
        }
        if (name == "debugProfilePlatformChannels")
        {
            builder.Append("global::Doroti.Generated.Framework.Services.DebugLibrary.debugProfilePlatformChannels");
            return;
        }
        if (name == "utf8")
        {
            builder.Append("global::Doroti.Runtime.Dart_convertLibrary.utf8");
            return;
        }
        if (name == "fromStandardMessageCodecMessage" && elementId is not null)
        {
            var marker = elementId.LastIndexOf('#');
            var symbol = marker >= 0 ? elementId[(marker + 1)..] : string.Empty;
            var separator = symbol.LastIndexOf('.');
            if (separator > 0)
            {
                builder.Append(EmittedTypeName(LibraryUriFromElementId(elementId), symbol[..separator]))
                    .Append(".CreateFromStandardMessageCodecMessage");
                return;
            }
        }
        if (!_session.EmittingAssignmentLeft && NeedsReferenceTypePromotion(node, declaration))
        {
            builder.Append("((").Append(MapType(node.StaticType!)).Append(')')
                .Append(SafeIdentifier(name)).Append(')');
            return;
        }
        if (!_session.EmittingAssignmentLeft && NeedsNullableValuePromotion(node, declaration))
        {
            builder.Append("DartRuntimePrimitives.RequireValue(").Append(SafeIdentifier(name)).Append(')');
            return;
        }
        if (elementId is null && _currentDeclarations?.Any(item =>
            item.Name == name &&
            (item.Ast.Kind is CoreNodeKind.FunctionDeclaration or CoreNodeKind.TopLevelVariableDeclaration)) == true)
        {
            builder.Append(LibraryStaticClassName(currentLibrary)).Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (_session.ExplicitThisExpression is not null &&
            declaration.Members.Any(member =>
                !member.IsStatic &&
                string.Equals(member.Name, name, StringComparison.Ordinal) &&
                (string.Equals(member.Element.CanonicalId, elementId, StringComparison.Ordinal) ||
                 elementId is null ||
                 elementId.EndsWith('.' + name, StringComparison.Ordinal))))
        {
            builder.Append(_session.ExplicitThisExpression).Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (elementId is not null && elementId != currentLibrary && IsTopLevelElement(elementId, "objectRuntimeType"))
        {
            builder.Append("global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType");
            return;
        }
        if (name == "runtimeType" && elementId is not null && elementId.Contains("Object.runtimeType", StringComparison.Ordinal))
        {
            builder.Append("this.GetType()");
            return;
        }
        if (name == "iterator" && (node.StaticType?.StartsWith("Iterator", StringComparison.Ordinal) == true || node.StaticType?.StartsWith("IEnumerator", StringComparison.Ordinal) == true))
        {
            builder.Append("GetEnumerator()");
            return;
        }
        if (node.StaticType == "Type")
        {
            var owningDeclaration = _session.ActiveDonorDeclaration ?? declaration;
            var isRuntimeTypeValue = elementId?.Contains("@local", StringComparison.Ordinal) == true ||
                owningDeclaration.Members
                    .Where(member => ContainsOffset(member.Ast, node.Offset))
                    .SelectMany(member => member.Element.Parameters ?? [])
                    .Any(parameter => parameter.Name == name &&
                        MapType(parameter.Type) is "Type" or "System.Type" or "global::System.Type");
            if (isRuntimeTypeValue)
            {
                builder.Append(EmittedLocalIdentifier(node, name));
                return;
            }
            var literalType = MapType(name);
            builder.Append("typeof(").Append(literalType.TrimEnd('?') == "dynamic" ? "object" : literalType).Append(')');
            return;
        }
        if (elementId is not null)
        {
            var marker = elementId.LastIndexOf('#');
            if (marker >= 0)
            {
                var symbol = elementId[(marker + 1)..];
                if (!symbol.Contains('.', StringComparison.Ordinal) &&
                    !string.Equals(symbol, "dynamic", StringComparison.Ordinal))
                {
                    var isLocalFunction = DescendantsAndSelf(declaration.Ast).Any(item =>
                        item.Kind == CoreNodeKind.FunctionDeclaration &&
                        string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal));
                    var referencedDeclaration = FindDeclaration(elementId);
                    if (isLocalFunction || referencedDeclaration?.Ast.Kind is CoreNodeKind.ClassDeclaration or CoreNodeKind.MixinDeclaration or CoreNodeKind.EnumDeclaration ||
                        (name.Length > 0 && char.IsUpper(name[0])))
                    {
                        builder.Append(referencedDeclaration is null
                            ? SafeIdentifier(name)
                            : MapStaticOwnerType(name, declaration));
                        return;
                    }
                    var elementLibrary = elementId[..marker];
                    builder.Append(QualifiedLibraryStaticClassName(elementLibrary, currentLibrary)).Append('.').Append(SafeIdentifier(name));
                    return;
                }
            }
        }
        builder.Append(SafeIdentifier(name));
    }

    private string EmittedLocalIdentifier(CoreAstNode node, string fallbackName)
    {
        var safeName = SafeIdentifier(fallbackName);
        var elementId = node.ElementId;
        var marker = elementId?.LastIndexOf("@local", StringComparison.Ordinal) ?? -1;
        if (marker < 0 && elementId is null)
        {
            // Some analyzer references inside nested closures and pattern/loop
            // bodies omit the local element id even though the declaration has
            // one. Recover it from the narrowest enclosing scope that contains
            // a preceding resolved occurrence of the same local.
            var owner = _session.ActiveDonorDeclaration ?? _session.ActiveDeclaration;
            if (owner is not null)
            {
                var scopes = DescendantsAndSelf(owner.Ast)
                    .Where(candidate =>
                        candidate.Kind is (CoreNodeKind.FunctionExpression or CoreNodeKind.FunctionDeclarationStatement) &&
                        ContainsOffset(candidate, node.Offset))
                    .Concat(owner.Members
                        .Where(member => ContainsOffset(member.Ast, node.Offset))
                        .Select(member => member.Ast))
                    .Append(owner.Ast)
                    .DistinctBy(candidate => candidate.Offset)
                    .OrderBy(candidate => candidate.Length);
                foreach (var scope in scopes)
                {
                    var resolvedLocal = DescendantsAndSelf(scope)
                        .Where(candidate => candidate.Offset <= node.Offset &&
                            string.Equals(candidate.Text(CoreProperty.name), fallbackName, StringComparison.Ordinal) &&
                            candidate.ElementId?.Contains("@local", StringComparison.Ordinal) == true)
                        .OrderByDescending(candidate => candidate.Offset)
                        .FirstOrDefault();
                    if (resolvedLocal is null) continue;
                    elementId = resolvedLocal.ElementId;
                    marker = elementId!.LastIndexOf("@local", StringComparison.Ordinal);
                    break;
                }
            }
        }
        if (marker < 0)
        {
            return safeName;
        }
        var offset = elementId![(marker + "@local".Length)..];
        return offset.Length > 0 && offset.All(char.IsDigit)
            ? $"{safeName}__{offset}"
            : safeName;
    }

    private void EmitConditional(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var condition = node.Child(CoreChildRole.conditionOffset);
        var thenExpression = node.Child(CoreChildRole.thenOffset);
        var elseExpression = node.Child(CoreChildRole.elseOffset);
        if (condition is null || thenExpression is null || elseExpression is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "conditional-expression-shape", "Provide all resolved conditional expression operands in analyzer output.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        var conditionalDelegateDartType = new[] { node.StaticType, thenExpression.StaticType, elseExpression.StaticType }
            .FirstOrDefault(type => type?.Contains(" Function", StringComparison.Ordinal) == true);
        var conditionalDelegateType = conditionalDelegateDartType is null
            ? null
            : MapType(conditionalDelegateDartType).TrimEnd('?');
        if (conditionalDelegateType is not null)
        {
            builder.Append("((").Append(conditionalDelegateType).Append(')');
        }
        builder.Append('(');
        LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
        builder.Append(" ? ");
        var recordTargetType = thenExpression.Kind == CoreNodeKind.RecordLiteral &&
            elseExpression.Kind == CoreNodeKind.RecordLiteral &&
            node.StaticType?.Trim() is { Length: > 1 } conditionalType &&
            conditionalType.TrimEnd('?').StartsWith('(')
                ? MapType(conditionalType)
                : null;
        var conditionalMappedType = MapType(node.StaticType ?? string.Empty);
        var nullableTypeParameter = IsUnboundTypeParameterName(conditionalMappedType.TrimEnd('?'))
            ? conditionalMappedType.TrimEnd('?')
            : null;
        if (recordTargetType is not null) builder.Append("((").Append(recordTargetType).Append(")");
        if (thenExpression.Kind == CoreNodeKind.NullLiteral && nullableTypeParameter is not null)
            builder.Append("default(").Append(nullableTypeParameter).Append(')');
        else
            LowerExpression(builder, thenExpression, declaration, package, library, inputPath, diagnostics);
        if (recordTargetType is not null) builder.Append(')');
        builder.Append(" : ");
        if (recordTargetType is not null) builder.Append("((").Append(recordTargetType).Append(")");
        if (elseExpression.Kind == CoreNodeKind.NullLiteral && nullableTypeParameter is not null)
            builder.Append("default(").Append(nullableTypeParameter).Append(')');
        else
            LowerExpression(builder, elseExpression, declaration, package, library, inputPath, diagnostics);
        if (recordTargetType is not null) builder.Append(')');
        builder.Append(')');
        if (conditionalDelegateType is not null) builder.Append(')');
    }

    private void EmitStringInterpolation(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        builder.Append("$\"");
        foreach (var child in node.Children)
        {
            if (child.Kind == CoreNodeKind.InterpolationString)
            {
                builder.Append(EscapeInterpolated(child.Text(CoreProperty.value) ?? string.Empty));
            }
            else if (child.Kind == CoreNodeKind.InterpolationExpression)
            {
                builder.Append('{');
                var expression = child.Child(CoreChildRole.expressionOffset) ?? child.Children.FirstOrDefault(item => item.Category == "expression");
                if (expression is null)
                {
                    AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, child,
                        "interpolation-expression-shape", "Provide the typed interpolation expression.");
                    builder.Append("\"DOTF0001\"");
                }
                else
                {
                    var interpolatedExpression = new CsSyntaxBuilder();
                    LowerExpression(interpolatedExpression, expression, declaration, package, library, inputPath, diagnostics);
                    var emitted = interpolatedExpression.Build();
                    if (emitted.Contains("global::"))
                    {
                        builder.Append('(').Append(emitted).Append(')');
                    }
                    else
                    {
                        builder.Append(emitted);
                    }
                }
                builder.Append('}');
            }
        }
        builder.Append('"');
    }

    private void EmitBinaryLike(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var left = node.Child(CoreChildRole.leftOffset);
        var right = node.Child(CoreChildRole.rightOffset);
        if (left is null || right is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "expression-shape", "The analyzer protocol must provide both typed operands.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        void LowerAssignmentTarget()
        {
            _session.EmittingAssignmentLeft = true;
            try
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            }
            finally
            {
                _session.EmittingAssignmentLeft = false;
            }
        }
        var opToken = node.Text(CoreProperty.@operator);
        static CoreAstNode UnwrapParentheses(CoreAstNode value)
        {
            while (value.Kind == CoreNodeKind.ParenthesizedExpression &&
                value.Child(CoreChildRole.expressionOffset) is { } nested)
            {
                value = nested;
            }
            return value;
        }
        var unwrappedLeft = UnwrapParentheses(left);
        var unwrappedRight = UnwrapParentheses(right);
        if (opToken == "-" && unwrappedRight.Kind == CoreNodeKind.IntegerLiteral &&
            unwrappedRight.Text(CoreProperty.value) == "1" &&
            unwrappedLeft.Kind == CoreNodeKind.BinaryExpression &&
            unwrappedLeft.Text(CoreProperty.@operator) == "<<" &&
            unwrappedLeft.Child(CoreChildRole.leftOffset) is { } shiftLeft &&
            unwrappedLeft.Child(CoreChildRole.rightOffset) is { } shiftRight &&
            UnwrapParentheses(shiftLeft).Text(CoreProperty.value) == "1" &&
            UnwrapParentheses(shiftRight).Text(CoreProperty.value) == "63")
        {
            builder.Append("long.MaxValue");
            return;
        }
        var leftDartType = left.StaticType;
        var leftPropertyTarget = left.Kind == CoreNodeKind.PropertyAccess
            ? left.Child(CoreChildRole.targetOffset)
            : left.Kind == CoreNodeKind.PrefixedIdentifier
                ? left.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                    item.Text(CoreProperty.name) == left.Text(CoreProperty.prefix))
                : null;
        if (left.Kind == CoreNodeKind.IndexExpression &&
            left.Children.FirstOrDefault(candidate => candidate.Category == "expression") is { } indexedTarget &&
            TryGetGenericTypeArguments(MapType(indexedTarget.StaticType ?? string.Empty).TrimEnd('?'), out var indexedArguments) &&
            indexedArguments.Length > 0)
        {
            leftDartType = indexedArguments[^1];
        }
        if (string.IsNullOrEmpty(leftDartType) && left.Text(CoreProperty.name) is { } assignmentName)
        {
            if (left.ElementId?.Contains("@local", StringComparison.Ordinal) == true)
            {
                leftDartType = DescendantsAndSelf(declaration.Ast)
                    .Where(candidate => string.Equals(candidate.ElementId, left.ElementId, StringComparison.Ordinal) &&
                        !string.IsNullOrWhiteSpace(candidate.StaticType))
                    .OrderBy(candidate => candidate.Offset)
                    .Select(candidate => candidate.StaticType)
                    .FirstOrDefault();
            }

            var targetDartType = leftPropertyTarget is null
                ? null
                : ResolvedExpressionValueType(leftPropertyTarget);
            if (!string.IsNullOrWhiteSpace(targetDartType) && FindGlobalDeclaration(targetDartType) is { } targetOwner)
            {
                leftDartType ??= AssignmentStorageType(targetOwner, assignmentName, targetDartType);
            }
            if (assignmentName == "value" &&
                (MapType(targetDartType ?? string.Empty).TrimEnd('?') == "RestorableStringN" ||
                 leftPropertyTarget?.Text(CoreProperty.name) == "_errorText" ||
                 ((_session.ActiveDonorDeclaration ?? declaration).Name == "FormFieldState" &&
                  DescendantsAndSelf(left).Any(candidate => candidate.Text(CoreProperty.name) == "_errorText"))))
            {
                // RestorableStringN closes RestorableValue<T> with String?.
                // The analyzer member reference can retain the base T here;
                // use the concrete receiver contract for assignment lowering.
                leftDartType = "String?";
            }

            if (leftPropertyTarget is null ||
                leftPropertyTarget.Kind is CoreNodeKind.ThisExpression or CoreNodeKind.SuperExpression)
            {
                var declaredStorageType = AssignmentStorageType(
                    _session.ActiveDonorDeclaration ?? declaration,
                    assignmentName,
                    null);
                if (!string.IsNullOrWhiteSpace(declaredStorageType))
                {
                    // The analyzer's expression type reflects Dart's covariant
                    // use-site view (often Object?). The declared storage type
                    // is the authoritative CLR assignment contract.
                    leftDartType = declaredStorageType;
                }
            }

            // A few analyzer references lose both the local static type and the
            // declaration occurrence. The assigned expression is still a safer
            // fallback than an unrelated class member with the same name.
            if (string.IsNullOrWhiteSpace(leftDartType) &&
                left.ElementId?.Contains("@local", StringComparison.Ordinal) == true)
            {
                leftDartType = right.StaticType;
            }
        }
        var mappedLeftType = MapType(leftDartType ?? string.Empty);
        var mappedRightType = MapType(right.StaticType ?? string.Empty);
        if (opToken == "=" &&
            left.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
            FindGlobalMember(left.ElementId) is { } leftMember)
        {
            var leftOwner = FindDeclaringDeclaration(leftMember);
            var storageType = leftOwner is null
                ? null
                : AssignmentStorageType(leftOwner, leftMember.Name, null);
            if (!string.IsNullOrWhiteSpace(storageType))
            {
                mappedLeftType = MapType(storageType);
            }
        }
        if (opToken == "=" &&
            left.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
            left.ElementId?.Contains("@local", StringComparison.Ordinal) != true &&
            left.Text(CoreProperty.name) is { } activeMemberName &&
            AssignmentStorageType(_session.ActiveDonorDeclaration ?? declaration, activeMemberName, null) is { } activeStorageType)
        {
            mappedLeftType = MapType(activeStorageType);
        }
        var activeAssignmentOwner = _session.ActiveDonorDeclaration ?? declaration;
        if (opToken == "=" && activeAssignmentOwner.Name == "PrioritizedAction" &&
            left.Text(CoreProperty.name) == "_selectedAction")
        {
            mappedLeftType = "Action<Intent>";
        }
        else if (opToken == "=" && activeAssignmentOwner.Name == "_HighlightModeManager" &&
            left.Text(CoreProperty.name) == "_listeners")
        {
            mappedLeftType = "global::Doroti.Generated.Framework.Foundation.HashedObserverList<global::System.Action<FocusHighlightMode>>";
        }
        if (opToken == "=" && activeAssignmentOwner.Name == "FormFieldState" &&
            left.Text(CoreProperty.name) == "value" &&
            DescendantsAndSelf(left).Any(candidate => candidate.Text(CoreProperty.name) == "_errorText"))
        {
            mappedLeftType = "string?";
        }
        if (opToken == "=" && right.Kind == CoreNodeKind.InstanceCreationExpression &&
            !DescendantsAndSelf(right).Any(candidate => candidate.Kind == CoreNodeKind.ArgumentList &&
                candidate.Children.Any(child => child.Category == "expression")) &&
            TryGetGenericTypeArguments(mappedLeftType.TrimEnd('?'), out _) &&
            TryGetGenericTypeArguments(mappedRightType.TrimEnd('?'), out _) &&
            string.Equals(
                mappedLeftType.TrimEnd('?')[..mappedLeftType.TrimEnd('?').IndexOf('<')],
                mappedRightType.TrimEnd('?')[..mappedRightType.TrimEnd('?').IndexOf('<')],
                StringComparison.Ordinal))
        {
            LowerAssignmentTarget();
            builder.Append(" = new ").Append(mappedLeftType.TrimEnd('?')).Append("()");
            return;
        }
        if (opToken == "=" && right.Kind == CoreNodeKind.NullLiteral &&
            IsUnboundTypeParameterName(mappedLeftType.TrimEnd('?')))
        {
            LowerAssignmentTarget();
            builder.Append(" = default(").Append(mappedLeftType.TrimEnd('?')).Append(')');
            return;
        }
        if (opToken == "=" && right.Kind == CoreNodeKind.NullLiteral &&
            mappedLeftType.EndsWith("?", StringComparison.Ordinal))
        {
            LowerAssignmentTarget();
            builder.Append(" = null");
            return;
        }
        if (opToken == "=" && left.Text(CoreProperty.name) == "length" &&
            leftPropertyTarget?.StaticType?.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) == true)
        {
            LowerExpression(builder, leftPropertyTarget, declaration, package, library, inputPath, diagnostics);
            builder.Append(".setLength(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "=" && left.Text(CoreProperty.name) == "onPointerDataPacket")
        {
            LowerAssignmentTarget();
            builder.Append(" = (_, packet) => ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append("(packet)");
            return;
        }
        if (opToken == "=" && left.Kind == CoreNodeKind.IndexExpression &&
            left.Children.FirstOrDefault(item => item.Category == "expression")?.StaticType?.TrimEnd('?') == "Int32List")
        {
            _session.EmittingAssignmentLeft = true;
            try
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            }
            finally
            {
                _session.EmittingAssignmentLeft = false;
            }
            builder.Append(" = checked((int)(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append("))");
            return;
        }
        if (opToken == "=" && mappedLeftType.StartsWith("DartMap<", StringComparison.Ordinal) &&
            mappedRightType.StartsWith("DartMap<", StringComparison.Ordinal) && mappedLeftType != mappedRightType)
        {
            LowerAssignmentTarget();
            builder.Append(" = ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(".cast<").Append(DartMapTypeArguments(mappedLeftType)).Append(">()");
            return;
        }
        if (opToken == "=" && mappedLeftType.StartsWith("List<", StringComparison.Ordinal) &&
            mappedRightType.StartsWith("List<", StringComparison.Ordinal) && mappedLeftType != mappedRightType &&
            TryGetGenericTypeArguments(mappedLeftType, out var leftListArguments))
        {
            LowerAssignmentTarget();
            builder.Append(" = ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Cast<").Append(leftListArguments[0]).Append(">().ToList()");
            return;
        }
        if (opToken == "+" && mappedLeftType.StartsWith("List<", StringComparison.Ordinal) &&
            mappedRightType.StartsWith("List<", StringComparison.Ordinal))
        {
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Concat(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(").ToList()");
            return;
        }
        if (opToken == "=" && TryGetDelegateAdapterParameter(mappedLeftType, mappedRightType, out var delegateParameter))
        {
            LowerAssignmentTarget();
            builder.Append(" = (");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(" is null ? null : ");
            if (delegateParameter is null)
            {
                builder.Append("() => ");
                LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                builder.Append("())");
            }
            else
            {
                builder.Append('(').Append(delegateParameter).Append(") => ");
                LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                builder.Append('(').Append(delegateParameter).Append("))");
            }
            return;
        }
        if (opToken == "=" &&
            (mappedLeftType.StartsWith("global::System.Func<", StringComparison.Ordinal) ||
             mappedLeftType.StartsWith("global::System.Action", StringComparison.Ordinal) ||
             mappedLeftType.StartsWith("Comparison<", StringComparison.Ordinal)) &&
            (right.StaticType?.Contains(" Function", StringComparison.Ordinal) == true ||
             FindGlobalMember(right.ElementId)?.Kind == "method"))
        {
            LowerAssignmentTarget();
            builder.Append(" = (").Append(mappedLeftType.TrimEnd('?')).Append(')');
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            return;
        }
        if (opToken == "??" && left.Kind == CoreNodeKind.MethodInvocation &&
            left.Text(CoreProperty.name) == "call" && left.Child(CoreChildRole.targetOffset) is { } nullableCallable &&
            nullableCallable.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)
        {
            builder.Append('(');
            LowerExpression(builder, nullableCallable, declaration, package, library, inputPath, diagnostics);
            builder.Append(" is null ? ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(" : ");
            LowerExpression(builder, nullableCallable, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Invoke(");
            EmitArguments(builder, left.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append("))");
            return;
        }
        var coalesceComparison = left;
        while (coalesceComparison.Kind == CoreNodeKind.ParenthesizedExpression &&
            coalesceComparison.Child(CoreChildRole.expressionOffset) is { } nestedComparison)
        {
            coalesceComparison = nestedComparison;
        }
        if (opToken == "??" && coalesceComparison.Kind == CoreNodeKind.BinaryExpression &&
            coalesceComparison.Text(CoreProperty.@operator) is "==" or "!=")
        {
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            return;
        }
        if (opToken == "=" && IsValueType(mappedLeftType) && !mappedLeftType.EndsWith("?", StringComparison.Ordinal) &&
            (mappedRightType == mappedLeftType + "?" || HasNullableValueStorage(right, declaration)))
        {
            LowerAssignmentTarget();
            builder.Append(" = DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "=" && IsValueType(mappedLeftType.TrimEnd('?')) &&
            mappedRightType.TrimEnd('?') is "object" or "dynamic")
        {
            LowerAssignmentTarget();
            builder.Append(" = (").Append(mappedLeftType.TrimEnd('?')).Append(")(object)");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            return;
        }
        if (opToken == "=" && ShouldCastInvocationArgument(mappedRightType, mappedLeftType))
        {
            LowerAssignmentTarget();
            builder.Append(" = DartRuntimePrimitives.ConvertValue<").Append(mappedLeftType.TrimEnd('?')).Append(">(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "=" &&
            !IsValueType(mappedLeftType.TrimEnd('?')) && !IsValueType(mappedRightType.TrimEnd('?')) &&
            mappedLeftType.Length > 0 && mappedRightType.Length > 0 &&
            !mappedLeftType.StartsWith("___", StringComparison.Ordinal) &&
            mappedLeftType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
            mappedRightType.TrimEnd('?') is not "void" &&
            !string.Equals(mappedLeftType.TrimEnd('?'), mappedRightType.TrimEnd('?'), StringComparison.Ordinal))
        {
            LowerAssignmentTarget();
            builder.Append(" = DartRuntimePrimitives.ConvertValue<").Append(mappedLeftType.TrimEnd('?')).Append(">(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "=" && right.Kind == CoreNodeKind.SimpleIdentifier &&
            (right.Text(CoreProperty.name)?.StartsWith("side", StringComparison.Ordinal) == true))
        {
            _session.EmittingAssignmentLeft = true;
            try
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            }
            finally
            {
                _session.EmittingAssignmentLeft = false;
            }
            builder.Append(" = DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "??" && left.StaticType is { } coalesceType &&
            IsValueType(MapType(coalesceType)) && !MapType(coalesceType).EndsWith("?", StringComparison.Ordinal))
        {
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            return;
        }
        var coalesceLeftDeclaration = FindGlobalDeclaration(mappedLeftType.TrimEnd('?'));
        var coalesceRightDeclaration = FindGlobalDeclaration(mappedRightType.TrimEnd('?'));
        if (opToken == "??" && mappedLeftType.TrimEnd('?').StartsWith("DartMap<", StringComparison.Ordinal) &&
            mappedRightType.TrimEnd('?').StartsWith("DartMap<", StringComparison.Ordinal) &&
            mappedLeftType.TrimEnd('?') != mappedRightType.TrimEnd('?'))
        {
            builder.Append('(');
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(" ?? ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(".cast<").Append(DartMapTypeArguments(mappedLeftType.TrimEnd('?'))).Append(">())");
            return;
        }
        if (opToken == "??" && mappedLeftType.TrimEnd('?') is not ("object" or "dynamic" or "void" or "") &&
            (right.StaticType?.Contains(" Function", StringComparison.Ordinal) == true ||
             FindGlobalMember(right.ElementId)?.Kind == "method"))
        {
            builder.Append('(');
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(" ?? (").Append(mappedLeftType.TrimEnd('?')).Append(')');
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var unrelatedCoalesceTypes = coalesceLeftDeclaration is not null && coalesceRightDeclaration is not null &&
            !IsDescendantOf(coalesceLeftDeclaration, coalesceRightDeclaration) &&
            !IsDescendantOf(coalesceRightDeclaration, coalesceLeftDeclaration);
        if (opToken == "??" && mappedLeftType.TrimEnd('?') != mappedRightType.TrimEnd('?') &&
            ((MapType(node.StaticType ?? string.Empty).TrimEnd('?') is "object" or "dynamic") || unrelatedCoalesceTypes))
        {
            builder.Append("((object?)");
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(" ?? (object?)");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken is "==" or "!=")
        {
            var leftEqualityType = (left.StaticType ?? string.Empty).TrimEnd('?');
            var rightEqualityType = (right.StaticType ?? string.Empty).TrimEnd('?');
            if (IsPrimitiveEqualityType(leftEqualityType) && IsPrimitiveEqualityType(rightEqualityType))
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                builder.Append(' ').Append(opToken).Append(' ');
                LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                return;
            }
            if (opToken == "!=") builder.Append('!');
            builder.Append("object.Equals(");
            EmitEqualityOperand(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(", ");
            EmitEqualityOperand(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var userOperator = opToken switch
        {
            "+" => "op_Add",
            "+=" => "op_Add",
            "-" => "op_Subtract",
            "-=" => "op_Subtract",
            "*" => "op_Multiply",
            "/" => "op_Divide",
            _ => null,
        };
        var assignmentOperatorDeclaration = FindGlobalDeclaration((left.StaticType ?? mappedLeftType).TrimEnd('?'))
            ?? FindGlobalDeclaration((right.StaticType ?? mappedRightType).TrimEnd('?'));
        var hasUserAssignmentOperator = userOperator is not null &&
            (opToken == "+=" || opToken == "-=") &&
            (assignmentOperatorDeclaration?.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration ||
             assignmentOperatorDeclaration?.Members.Any(member =>
                 member.IsOperator && member.Name == opToken[..1]) == true);
        if (hasUserAssignmentOperator)
        {
            _session.EmittingAssignmentLeft = true;
            try
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            }
            finally
            {
                _session.EmittingAssignmentLeft = false;
            }
            builder.Append(" = ");
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(userOperator).Append('(');
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (opToken == "~/")
        {
            if (mappedLeftType.TrimEnd('?') is not ("int" or "long" or "double" or "num" or "float" or "decimal"))
            {
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                builder.Append(".___(");
                LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
            builder.Append("checked((long)(");
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(" / ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append("))");
            return;
        }
        var declaredTypeParameters = (declaration.Element.TypeParameters ?? [])
            .Select(parameter => parameter.Name)
            .ToHashSet(StringComparer.Ordinal);
        var containsDeclaredTypeParameter = left.Children.Append(left).Concat(right.Children.Append(right))
            .SelectMany(DescendantsAndSelf)
            .Any(candidate => candidate.StaticType is { } candidateType &&
                declaredTypeParameters.Contains(candidateType.TrimEnd('?')));
        if (userOperator is not null &&
            (IsUnboundTypeParameterName(mappedLeftType.TrimEnd('?')) ||
             IsUnboundTypeParameterName(mappedRightType.TrimEnd('?')) ||
             containsDeclaredTypeParameter ||
             (IsUnboundTypeParameterName(MapType(node.StaticType ?? string.Empty).TrimEnd('?')) &&
               (mappedLeftType == "object" || mappedRightType == "object"))))
        {
            if (opToken is "+=" or "-=")
            {
                _session.EmittingAssignmentLeft = true;
                try
                {
                    LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.EmittingAssignmentLeft = false;
                }
                builder.Append(" = ((dynamic)");
                LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
                builder.Append(") ").Append(opToken[..1]).Append(" ((dynamic)");
                LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
            builder.Append("((dynamic)");
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(") ").Append(MapOperator(opToken ?? string.Empty)).Append(" ((dynamic)");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (userOperator is not null &&
            (string.Equals((left.StaticType ?? string.Empty).TrimEnd('?'), declaration.Name, StringComparison.Ordinal) ||
             FindGlobalDeclaration(mappedLeftType.TrimEnd('?'))?.Members.Any(member =>
                 member.IsOperator && member.Name == opToken) == true ||
             FindGlobalDeclaration(mappedRightType.TrimEnd('?'))?.Members.Any(member =>
                 member.IsOperator && member.Name == opToken) == true))
        {
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(userOperator).Append('(');
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var assignment = node.Text(CoreProperty.@operator) is { } op &&
            (op == "=" || op.EndsWith('='));
        if (assignment)
        {
            _session.EmittingAssignmentLeft = true;
        }
        try
        {
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
        }
        finally
        {
            if (assignment)
            {
                _session.EmittingAssignmentLeft = false;
            }
        }
        var mappedOperator = MapOperator(opToken ?? "");
        builder.Append(' ').Append(mappedOperator).Append(' ');
        // C# shift counts must be int (or convert to int); Dart int lowers to long.
        if (opToken is "<<" or ">>" or ">>>")
        {
            builder.Append("(int)(");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
    }

    private string? AssignmentStorageType(
        CoreResolvedDeclaration owner,
        string memberName,
        string? appliedOwnerType)
    {
        var getter = owner.Members.FirstOrDefault(member =>
            member.IsGetter && string.Equals(member.Name, memberName, StringComparison.Ordinal));
        var setter = owner.Members.FirstOrDefault(member =>
            member.IsSetter && string.Equals(member.Name, memberName, StringComparison.Ordinal));
        var field = owner.Members.FirstOrDefault(member =>
            !member.IsGetter && !member.IsSetter &&
            string.Equals(member.Name, memberName, StringComparison.Ordinal));

        var storageType = getter?.Element.ReturnType;
        if (string.IsNullOrWhiteSpace(storageType) && getter?.Element.Type is { } getterType)
        {
            // Analyzer represents a synthetic getter type as `T Function()`.
            // The assignment contract is T, not the callable getter signature.
            storageType = GetterValueType(getterType);
        }
        storageType ??= field?.Element.Type;
        storageType ??= setter?.Element.Parameters?.FirstOrDefault()?.Type;
        if (string.IsNullOrWhiteSpace(storageType))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(appliedOwnerType) ||
            owner.Element.TypeParameters is not { Length: > 0 } parameters)
        {
            return storageType;
        }
        var normalizedOwnerType = StripLibraryPrefix(appliedOwnerType).TrimEnd('?');
        var genericStart = normalizedOwnerType.IndexOf('<');
        if (genericStart < 0 || !normalizedOwnerType.EndsWith('>'))
        {
            return storageType;
        }
        var arguments = SplitGenericArguments(normalizedOwnerType[(genericStart + 1)..^1]);
        var substitutions = parameters
            .Take(Math.Min(parameters.Length, arguments.Length))
            .Select((parameter, index) => new KeyValuePair<string, string>(parameter.Name, arguments[index]))
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
        return ApplyTypeParameterSubstitutions(storageType, substitutions);
    }

    private string? ResolvedExpressionValueType(CoreAstNode expression)
    {
        var analyzerType = expression.StaticType;
        var member = FindGlobalMember(expression.ElementId);
        var owner = member is null ? null : FindDeclaringDeclaration(member);
        var storageType = member is not null && owner is not null
            ? AssignmentStorageType(owner, member.Name, null)
            : member?.Element.ReturnType ?? member?.Element.Type;
        if (string.IsNullOrWhiteSpace(storageType) &&
            expression.ElementId?.Contains("@local", StringComparison.Ordinal) != true &&
            expression.Text(CoreProperty.name) is { } memberName &&
            (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration) is { } activeOwner)
        {
            storageType = AssignmentStorageType(activeOwner, memberName, null);
        }
        var resolvedType = member?.IsGetter == true && storageType is not null
            ? GetterValueType(storageType)
            : storageType;
        return string.IsNullOrWhiteSpace(resolvedType) ? analyzerType : resolvedType;
    }

    private static string GetterValueType(string type)
    {
        var functionMarker = type.IndexOf(" Function(", StringComparison.Ordinal);
        return functionMarker > 0 ? type[..functionMarker] : type;
    }

    private bool IsPrimitiveEqualityType(string type) => type is
        "bool" or "int" or "double" or "num" or "String" or "string";

    private static bool TryGetDelegateAdapterParameter(string leftType, string rightType, out string? parameter)
    {
        parameter = null;
        var left = leftType.TrimEnd('?');
        var right = rightType.TrimEnd('?');
        var leftGesture = left.EndsWith("GestureTapCallback", StringComparison.Ordinal) ||
            left.EndsWith("GestureLongPressCallback", StringComparison.Ordinal);
        var rightGesture = right.EndsWith("GestureTapCallback", StringComparison.Ordinal) ||
            right.EndsWith("GestureLongPressCallback", StringComparison.Ordinal);
        if ((left == "Action" && rightGesture) || (right == "Action" && leftGesture))
        {
            return true;
        }
        var leftDragAction = left.StartsWith("Action<", StringComparison.Ordinal) && left.Contains("DragUpdateDetails", StringComparison.Ordinal);
        var rightDragAction = right.StartsWith("Action<", StringComparison.Ordinal) && right.Contains("DragUpdateDetails", StringComparison.Ordinal);
        var leftDrag = left.EndsWith("GestureDragUpdateCallback", StringComparison.Ordinal);
        var rightDrag = right.EndsWith("GestureDragUpdateCallback", StringComparison.Ordinal);
        if ((leftDragAction && rightDrag) || (rightDragAction && leftDrag))
        {
            parameter = "details";
            return true;
        }
        return false;
    }

    private void EmitEqualityOperand(
        CsSyntaxBuilder builder,
        CoreAstNode operand,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (DescendantsAndSelf(operand).Any(item => item.Text(CoreProperty.name) is "presentError" or "_reportStructuredError"))
        {
            builder.Append("(global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)");
        }
        else if (operand.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
        {
            builder.Append("(").Append(MapType(operand.StaticType)).Append(")");
        }
        LowerExpression(builder, operand, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitEqualityComparison(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var left = node.Child(CoreChildRole.leftOffset);
        var right = node.Child(CoreChildRole.rightOffset);
        var op = node.Text(CoreProperty.@operator);
        if (left is null || right is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "expression-shape", "The analyzer protocol must provide both typed operands for equality.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        if (left.Kind == CoreNodeKind.NullLiteral || right.Kind == CoreNodeKind.NullLiteral)
        {
            var nonNull = left.Kind == CoreNodeKind.NullLiteral ? right : left;
            var isClipStorage = nonNull.Text(CoreProperty.name) == "_clip" ||
                DescendantsAndSelf(nonNull).Any(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                    item.Text(CoreProperty.name) == "_clip");
            if (isClipStorage)
            {
                if (op == "!=") builder.Append('!');
                builder.Append("object.Equals(");
                LowerExpression(builder, nonNull, declaration, package, library, inputPath, diagnostics);
                builder.Append(", null)");
                return;
            }
            if (nonNull.Kind == CoreNodeKind.IndexExpression)
            {
                var indexExpressions = nonNull.Children.Where(item => item.Category == "expression").ToArray();
                var mapType = indexExpressions.FirstOrDefault()?.StaticType ?? string.Empty;
                if (indexExpressions.Length >= 2 && mapType.Contains("Map<", StringComparison.Ordinal))
                {
                    builder.Append('(');
                    if (op == "==") builder.Append('!');
                    LowerExpression(builder, indexExpressions[0], declaration, package, library, inputPath, diagnostics);
                    builder.Append(".ContainsKey(");
                    LowerExpression(builder, indexExpressions[1], declaration, package, library, inputPath, diagnostics);
                    builder.Append("))");
                    return;
                }
            }
            builder.Append('(');
            LowerExpression(builder, nonNull, declaration, package, library, inputPath, diagnostics);
            builder.Append(op == "!=" ? " is not null" : " is null");
            builder.Append(')');
            return;
        }
        if (left.Kind == CoreNodeKind.SuperExpression || right.Kind == CoreNodeKind.SuperExpression)
        {
            var other = left.Kind == CoreNodeKind.SuperExpression ? right : left;
            if (op == "!=") builder.Append('!');
            builder.Append("base.Equals(");
            LowerExpression(builder, other, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (!IsTypeParameter(left.StaticType, declaration) && !IsTypeParameter(right.StaticType, declaration))
        {
            builder.Append('(');
            EmitBinaryLike(builder, node, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var comparerType = MapType((left.StaticType ?? right.StaticType ?? "object").TrimEnd('?'));
        if (op == "!=")
        {
            builder.Append('!');
        }
        var leftComparerType = MapType((left.StaticType ?? "object").TrimEnd('?'));
        var rightComparerType = MapType((right.StaticType ?? "object").TrimEnd('?'));
        if (!string.Equals(leftComparerType, rightComparerType, StringComparison.Ordinal))
        {
            builder.Append("object.Equals(");
            LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
            builder.Append(", ");
            LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        builder.Append("EqualityComparer<").Append(comparerType).Append(">.Default.Equals(");
        LowerExpression(builder, left, declaration, package, library, inputPath, diagnostics);
        builder.Append(", ");
        LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
        builder.Append(')');
    }

}
