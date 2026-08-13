using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitBlockBody(
        CsSyntaxBuilder builder,
        CoreAstNode block,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        int indent)
    {
        foreach (var statement in block.Children.Where(item => item.Category == "statement"))
        {
            LowerStatement(builder, statement, indent, declaration, package, library, inputPath, diagnostics);
        }
    }

    private void EmitBlockBodyWithLocalRename(
        CsSyntaxBuilder builder,
        CoreAstNode block,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        int indent,
        string oldName,
        string newName)
    {
        var temp = new CsSyntaxBuilder();
        EmitBlockBody(temp, block, declaration, package, library, inputPath, diagnostics, indent);
        builder.Append(temp.Build().RenameIdentifier(oldName, newName));
    }

    private void EmitBlockBodyWithUniqueLocals(
        CsSyntaxBuilder builder,
        CoreAstNode block,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        int indent)
    {
        var temp = new CsSyntaxBuilder();
        EmitBlockBody(temp, block, declaration, package, library, inputPath, diagnostics, indent);
        var body = temp.Build();
        var duplicateNames = DescendantsAndSelf(declaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.VariableDeclaration)
            .Select(item => item.Text(CoreProperty.name))
            .Where(name => !string.IsNullOrEmpty(name))
            .GroupBy(name => name!, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var variable in block.Children
            .Where(item => item.Kind == CoreNodeKind.VariableDeclarationStatement)
            .SelectMany(item => DescendantsAndSelf(item).Where(child => child.Kind == CoreNodeKind.VariableDeclaration)))
        {
            var name = variable.Text(CoreProperty.name);
            if (!string.IsNullOrEmpty(name) && duplicateNames.Contains(name))
            {
                body = body.RenameIdentifier(SafeIdentifier(name), $"{SafeIdentifier(name)}__{variable.Offset}");
            }
        }
        builder.Append(body);
    }

    private void LowerExpressionWithLocalRename(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        string oldName,
        string newName)
    {
        var temp = new CsSyntaxBuilder();
        LowerExpression(temp, node, declaration, package, library, inputPath, diagnostics);
        builder.Append(temp.Build().RenameIdentifier(oldName, newName));
    }

    private void LowerStatement(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        using var syntaxRegion = builder.BeginRegion(CsSyntaxRegionKind.Statement, ToCsOrigin(node.Origin));
        var prefix = new string(' ', indent * 4);
        switch (node.Kind)
        {
            case CoreNodeKind.AssertStatement:
                {
                    var condition = node.Child(CoreChildRole.conditionOffset);
                    var message = node.Child(CoreChildRole.messageOffset);
                    if (condition?.Kind == CoreNodeKind.FunctionExpressionInvocation)
                    {
                        var function = condition.Child(CoreChildRole.functionOffset);
                        if (function?.Kind == CoreNodeKind.FunctionExpression)
                        {
                            builder.Append(prefix).Append("DartRuntimePrimitives.Assert(");
                            EmitLambda(
                                builder,
                                function,
                                condition.Child(CoreChildRole.argumentsOffset),
                                indent,
                                declaration,
                                package,
                                library,
                                inputPath,
                                diagnostics);
                            EmitAssertMessage(builder, message, declaration, package, library, inputPath, diagnostics);
                            builder.AppendLine(");");
                            return;
                        }
                    }
                    if (condition is not null)
                    {
                        builder.Append(prefix).Append("DartRuntimePrimitives.Assert(() => ");
                        LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                        EmitAssertMessage(builder, message, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(");");
                        return;
                    }
                    break;
                }
            case CoreNodeKind.ReturnStatement:
                {
                    var expression = node.Child(CoreChildRole.expressionOffset);
                    var returnOwner = _session.ActiveDonorDeclaration ?? declaration;
                    var returnMember = returnOwner.Members
                        .Where(candidate => ContainsOffset(candidate.Ast, node.Offset))
                        .OrderBy(candidate => candidate.Ast.Length)
                        .FirstOrDefault();
                    if (expression is null && returnMember is not null &&
                        DescendantsExcludingNestedFunctions(returnMember.Ast).Any(candidate => candidate.Kind == CoreNodeKind.YieldStatement))
                    {
                        builder.Append(prefix).AppendLine("yield break;");
                        return;
                    }
                    var sourceContainingReturn = ContainingReturnType(declaration, node);
                    if (expression is not null &&
                        string.Equals(MapType(sourceContainingReturn), "void", StringComparison.Ordinal))
                    {
                        if (expression.Kind == CoreNodeKind.SwitchExpression)
                        {
                            EmitSwitchExpressionStatement(builder, expression, indent, declaration, package, library, inputPath, diagnostics);
                            builder.Append(prefix).AppendLine("return;");
                            return;
                        }
                        builder.Append(prefix);
                        if (MapType(expression.StaticType ?? string.Empty) != "void" &&
                            expression.Kind is not (CoreNodeKind.ThrowExpression or CoreNodeKind.RethrowExpression))
                        {
                            builder.Append("_ = ");
                        }
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                        builder.Append(prefix).AppendLine("return;");
                        return;
                    }
                    if (expression?.Kind == CoreNodeKind.NullLiteral &&
                        IsInsideDartAsyncFunction(declaration, node) &&
                        string.Equals(sourceContainingReturn, "Future", StringComparison.Ordinal))
                    {
                        builder.Append(prefix).AppendLine("await Task.Yield();");
                        return;
                    }
                    var mappedContainingReturn = MapType(sourceContainingReturn);
                    var mappedExpressionType = MapType(expression is null
                        ? string.Empty
                        : ResolvedExpressionValueType(expression) ?? expression.StaticType ?? string.Empty);
                    if (expression is not null && IsInsideDartAsyncFunction(declaration, node) &&
                        mappedContainingReturn == "Future")
                    {
                        builder.Append(prefix).Append("await ");
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                        builder.Append(prefix).AppendLine("return;");
                        return;
                    }
                    if (expression?.Kind == CoreNodeKind.FunctionExpressionInvocation &&
                        DescendantsAndSelf(expression).Any(item => item.Kind == CoreNodeKind.SimpleIdentifier && item.Text(CoreProperty.name) == "parser"))
                    {
                        var containingReturn = sourceContainingReturn;
                        var start = containingReturn.IndexOf('<');
                        var end = containingReturn.LastIndexOf('>');
                        var resultType = start >= 0 && end > start ? containingReturn[(start + 1)..end] : "object";
                        builder.Append(prefix).Append("return await DartAsyncRuntime.AwaitFutureOrValue<").Append(resultType).Append(">(");
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(");");
                        return;
                    }
                    if (expression is not null && expression.Kind != CoreNodeKind.AwaitExpression &&
                        IsInsideDartAsyncFunction(declaration, node) && ExpressionProducesFuture(expression))
                    {
                        if (sourceContainingReturn == "Future")
                        {
                            builder.Append(prefix).Append("await ");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.AppendLine(";");
                            builder.Append(prefix).AppendLine("return;");
                        }
                        else
                        {
                            builder.Append(prefix).Append("return await ");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.AppendLine(";");
                        }
                        return;
                    }
                    if (expression is not null && MapType(sourceContainingReturn) == "void" && expression.StaticType == "void")
                    {
                        builder.Append(prefix);
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                        builder.Append(prefix).AppendLine("return;");
                        return;
                    }
                    builder.Append(prefix).Append("return");
                    if (expression is not null)
                    {
                        builder.Append(' ');
                        var containingReturn = sourceContainingReturn;
                        var mappedReturn = MapType(containingReturn);
                        if (returnOwner.Name == "_LerpSides" && returnMember?.Name == "resolve")
                        {
                            mappedReturn = "global::Doroti.Generated.Framework.Painting.BorderSide?";
                        }
                        else if (returnOwner.Name == "ContextAction" && returnMember?.Name == "_makeOverridableAction" &&
                            returnOwner.Element.TypeParameters is { Length: 1 } contextActionParameters)
                        {
                            mappedReturn = $"ContextAction<{SafeIdentifier(contextActionParameters[0].Name)}>";
                        }
                        if (IsInsideDartAsyncFunction(declaration, node) &&
                            mappedReturn.TrimEnd('?').StartsWith("Future<", StringComparison.Ordinal) &&
                            TryGetGenericTypeArguments(mappedReturn.TrimEnd('?'), out var asyncReturnArguments) &&
                            asyncReturnArguments.Length == 1)
                        {
                            // C# async methods are declared with Future<T>, but their
                            // return statements produce T rather than the async wrapper.
                            mappedReturn = asyncReturnArguments[0];
                        }
                        var returnsTypeParameter = expression.Kind == CoreNodeKind.NullLiteral &&
                            ((declaration.Element.TypeParameters ?? []).Any(parameter => containingReturn.Contains(parameter.Name, StringComparison.Ordinal)) ||
                             containingReturn.Contains("T?", StringComparison.Ordinal));
                        if (returnsTypeParameter) builder.Append("default");
                        else if (expression.Kind == CoreNodeKind.NullLiteral)
                        {
                            // A Dart null literal already carries the nullable
                            // return contract. Casting it through object before
                            // a nullable value type causes a runtime unbox NRE.
                            builder.Append("null");
                        }
                        else if (expression.Kind == CoreNodeKind.AssignmentExpression)
                        {
                            // An assignment expression is already typed by its
                            // storage location. Wrapping the whole expression in
                            // a return-type cast would also cast the assignment's
                            // left side and make it non-assignable in C#.
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        }
                        else if ((mappedReturn.StartsWith("global::System.Func<", StringComparison.Ordinal) ||
                                  mappedReturn.StartsWith("global::System.Action", StringComparison.Ordinal) ||
                                  mappedReturn.StartsWith("Comparison<", StringComparison.Ordinal)) &&
                                 (expression.StaticType?.Contains(" Function", StringComparison.Ordinal) == true ||
                                  FindGlobalMember(expression.ElementId)?.Kind == "method"))
                        {
                            builder.Append("((").Append(mappedReturn.TrimEnd('?')).Append(')');
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else if (IsValueType(mappedReturn) && !mappedReturn.EndsWith("?", StringComparison.Ordinal) &&
                            (mappedExpressionType == mappedReturn + "?" ||
                             HasNullableValueStorage(expression, _session.ActiveDonorDeclaration ?? declaration) ||
                             expression.Kind == CoreNodeKind.AssignmentExpression &&
                             (expression.Child(CoreChildRole.leftOffset) ??
                              expression.Children.FirstOrDefault(item => item.Category == "expression")) is { } assignmentTarget &&
                             (HasNullableValueStorage(assignmentTarget, _session.ActiveDonorDeclaration ?? declaration) ||
                              DescendantsAndSelf(assignmentTarget).Any(item => item.Text(CoreProperty.name) == "_cachedLineBreakCount"))))
                        {
                            builder.Append("DartRuntimePrimitives.RequireValue(");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else if (mappedExpressionType.TrimEnd('?') is "object" or "dynamic" &&
                            mappedReturn.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                            mappedReturn.Length > 0)
                        {
                            builder.Append("((").Append(mappedReturn.TrimEnd('?')).Append(")(object)");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else if (mappedReturn.StartsWith("List<", StringComparison.Ordinal) &&
                            mappedExpressionType.StartsWith("IReadOnlyList<", StringComparison.Ordinal) &&
                            TryGetGenericTypeArguments(mappedReturn, out var returnListArguments) &&
                            TryGetGenericTypeArguments(mappedExpressionType, out var expressionListArguments) &&
                            returnListArguments.SequenceEqual(expressionListArguments, StringComparer.Ordinal))
                        {
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(".ToList()");
                        }
                        else if (!IsValueType(mappedReturn.TrimEnd('?')) &&
                            !IsValueType(mappedExpressionType.TrimEnd('?')) &&
                            mappedReturn.Length > 0 && mappedExpressionType.Length > 0 &&
                            mappedReturn.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                            mappedExpressionType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                            !string.Equals(mappedReturn.TrimEnd('?'), mappedExpressionType.TrimEnd('?'), StringComparison.Ordinal))
                        {
                            builder.Append("((").Append(mappedReturn).Append(")(object?)");
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            builder.Append(')');
                        }
                        else
                        {
                            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                            if (containingReturn.Contains("List<", StringComparison.Ordinal) &&
                                expression.Kind == CoreNodeKind.MethodInvocation && expression.Text(CoreProperty.name) == "cast")
                            {
                                builder.Append(".ToList()");
                            }
                        }
                    }
                    else if (sourceContainingReturn == "Future" && !IsInsideDartAsyncFunction(declaration, node))
                    {
                        builder.Append(" Future.value()");
                    }
                    else if (sourceContainingReturn is not ("void" or "Future"))
                    {
                        builder.Append(" default!");
                    }
                    builder.AppendLine(";");
                    return;
                }
            case CoreNodeKind.ExpressionStatement:
                {
                    var expression = node.Children.FirstOrDefault(item => item.Category == "expression");
                    if (expression is not null)
                    {
                        if (expression.Kind == CoreNodeKind.MethodInvocation &&
                            expression.Child(CoreChildRole.targetOffset)?.Kind == CoreNodeKind.SuperExpression &&
                            expression.Text(CoreProperty.name) is { } superMethodName &&
                            AppliedMixinDeclarations(_session.ActiveDonorDeclaration ?? declaration)
                                .SelectMany(mixin => mixin.Members.Select(member => new { Mixin = mixin, Member = member }))
                                .LastOrDefault(candidate => candidate.Member.Kind == "method" && candidate.Member.Name == superMethodName) is { } mixinSuper &&
                            mixinSuper.Member.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody) is { } mixinBody &&
                            mixinBody.Child(CoreChildRole.blockOffset) is { } mixinBlock)
                        {
                            // Emit against the mixin declaration so a nested super call is
                            // resolved relative to that mixin instead of reselecting the same
                            // body from the concrete application indefinitely.
                            var superArguments = expression.Child(CoreChildRole.argumentsOffset)?.Children
                                .Where(item => item.Category == "expression")
                                .Select(item => item.Kind == CoreNodeKind.NamedExpression
                                    ? item.Children.FirstOrDefault(child => child.Category == "expression") ?? item
                                    : item)
                                .ToArray() ?? [];
                            var donorParameters = mixinSuper.Member.Element.Parameters ?? [];
                            foreach (var mixinStatement in mixinBlock.Children.Where(item =>
                                item.Category == "statement" && item.Kind != CoreNodeKind.ReturnStatement))
                            {
                                var mixinBuilder = new CsSyntaxBuilder();
                                LowerStatement(mixinBuilder, mixinStatement, indent, mixinSuper.Mixin, package, library, inputPath, diagnostics);
                                var mixinSyntax = mixinBuilder.Build();
                                for (var parameterIndex = 0;
                                    parameterIndex < donorParameters.Length && parameterIndex < superArguments.Length;
                                    parameterIndex++)
                                {
                                    var argument = superArguments[parameterIndex];
                                    if (argument.Kind == CoreNodeKind.SimpleIdentifier)
                                    {
                                        mixinSyntax = mixinSyntax.RenameIdentifier(
                                            SafeIdentifier(donorParameters[parameterIndex].Name),
                                            EmittedLocalIdentifier(argument, argument.Text(CoreProperty.name) ?? donorParameters[parameterIndex].Name));
                                    }
                                }
                                builder.Append(mixinSyntax);
                            }
                            return;
                        }
                        if (expression.Kind == CoreNodeKind.MethodInvocation && expression.Text(CoreProperty.name) == "setRange")
                        {
                            builder.Append(prefix);
                            EmitSetRange(builder, expression, indent, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                        var statementExpression = expression;
                        while (statementExpression.Kind == CoreNodeKind.ParenthesizedExpression &&
                            statementExpression.Child(CoreChildRole.expressionOffset) is { } unwrappedStatementExpression)
                        {
                            statementExpression = unwrappedStatementExpression;
                        }
                        if (statementExpression.Kind == CoreNodeKind.SwitchExpression)
                        {
                            EmitSwitchExpressionStatement(builder, statementExpression, indent, declaration, package, library, inputPath, diagnostics);
                            return;
                        }
                        builder.Append(prefix);
                        var wrapsDiscard = false;
                        if (expression.StaticType?.StartsWith("Future<", StringComparison.Ordinal) == true)
                        {
                            // Dart deliberately permits an unawaited Future expression. Preserve
                            // that behavior while making the discard explicit to the C# compiler.
                            builder.Append("DartRuntimePrimitives.Ignore(");
                            wrapsDiscard = true;
                        }
                        else if (expression.Kind is not (CoreNodeKind.MethodInvocation or CoreNodeKind.FunctionExpressionInvocation or
                            CoreNodeKind.AssignmentExpression or CoreNodeKind.PrefixExpression or CoreNodeKind.PostfixExpression or
                            CoreNodeKind.InstanceCreationExpression or CoreNodeKind.AwaitExpression or
                            CoreNodeKind.ThrowExpression or CoreNodeKind.RethrowExpression))
                        {
                            builder.Append("DartRuntimePrimitives.Ignore(");
                            wrapsDiscard = true;
                        }
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        if (wrapsDiscard) builder.Append(')');
                        builder.AppendLine(";");
                        return;
                    }
                    break;
                }
            case CoreNodeKind.Block:
                EmitBlockBody(builder, node, declaration, package, library, inputPath, diagnostics, indent);
                return;
            case CoreNodeKind.IfStatement:
                {
                    var condition = node.Child(CoreChildRole.conditionOffset);
                    var caseClause = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.CaseClause);
                    var thenStatement = node.Child(CoreChildRole.thenOffset);
                    var elseStatement = node.Child(CoreChildRole.elseOffset);
                    if (condition is null || thenStatement is null)
                    {
                        break;
                    }
                    builder.Append(prefix).Append("if (");
                    var promotions = new List<(string Name, string Local, string Type, bool RequireValue)>();
                    if (caseClause is null)
                    {
                        var promotionCondition = condition.Kind == CoreNodeKind.ParenthesizedExpression
                            ? condition.Child(CoreChildRole.expressionOffset) ?? condition
                            : condition;
                        CollectConditionPromotions(promotionCondition, declaration, promotions);
                        LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                    }
                    else
                    {
                        var guardedPattern = caseClause.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern);
                        EmitIfCaseCondition(builder, condition, guardedPattern, declaration, package, library, inputPath, diagnostics);
                    }
                    builder.AppendLine(")");
                    if (promotions.Count > 0)
                    {
                        var assignmentRoots = DescendantsAndSelf(thenStatement)
                            .Where(item => item.Kind == CoreNodeKind.AssignmentExpression)
                            .Select(item => item.Child(CoreChildRole.leftOffset))
                            .Where(item => item is not null)
                            .Cast<CoreAstNode>()
                            .Concat(DescendantsAndSelf(thenStatement)
                                .Where(item => item.Kind == CoreNodeKind.AssignedVariablePattern))
                            .Concat(DescendantsAndSelf(thenStatement)
                                .Where(item => (item.Kind is CoreNodeKind.PrefixExpression or CoreNodeKind.PostfixExpression) &&
                                    item.Text(CoreProperty.@operator) is "++" or "--"));
                        var assignedNames = assignmentRoots
                            .SelectMany(item => DescendantsAndSelf(item)
                                .Where(child => child.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.AssignedVariablePattern)
                                .Select(child => SafeIdentifier(child.Text(CoreProperty.name) ?? string.Empty)))
                            .ToHashSet(StringComparer.Ordinal);
                        promotions.RemoveAll(item => assignedNames.Contains(item.Name));
                    }
                    if (promotions.Count > 0)
                    {
                        EmitPromotedBracedStatement(builder, thenStatement, indent, declaration, package, library, inputPath, diagnostics, promotions);
                    }
                    else
                    {
                        EmitBracedStatement(builder, thenStatement, indent, declaration, package, library, inputPath, diagnostics);
                    }
                    if (elseStatement is not null)
                    {
                        builder.Append(prefix).AppendLine("else");
                        EmitBracedStatement(builder, elseStatement, indent, declaration, package, library, inputPath, diagnostics);
                    }
                    return;
                }
            case CoreNodeKind.ForStatement:
                {
                    var loopParts = node.Children.FirstOrDefault(item =>
                        item.Kind is CoreNodeKind.ForPartsWithDeclarations or CoreNodeKind.ForPartsWithExpression or
                            CoreNodeKind.ForEachPartsWithDeclaration or CoreNodeKind.ForEachPartsWithIdentifier or
                            CoreNodeKind.ForEachPartsWithPattern);
                    var body = node.Children.FirstOrDefault(item => item.Category == "statement");
                    if (loopParts is null || body is null)
                    {
                        break;
                    }
                    if (loopParts.Kind is CoreNodeKind.ForEachPartsWithDeclaration or CoreNodeKind.ForEachPartsWithIdentifier or
                        CoreNodeKind.ForEachPartsWithPattern)
                    {
                        EmitForEach(builder, prefix, loopParts, body, indent, declaration, package, library, inputPath, diagnostics);
                    }
                    else
                    {
                        EmitForLoop(builder, prefix, loopParts, body, indent, declaration, package, library, inputPath, diagnostics);
                    }
                    return;
                }
            case CoreNodeKind.WhileStatement:
                {
                    var condition = node.Children.FirstOrDefault(item => item.Category == "expression");
                    var body = node.Children.FirstOrDefault(item => item.Category == "statement");
                    if (condition is null || body is null)
                    {
                        break;
                    }
                    builder.Append(prefix).Append("while (");
                    LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(")");
                    EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
                    return;
                }
            case CoreNodeKind.DoStatement:
                {
                    var body = node.Children.FirstOrDefault(item => item.Category == "statement");
                    var condition = node.Children.FirstOrDefault(item => item.Category == "expression");
                    if (condition is null || body is null)
                    {
                        break;
                    }
                    builder.Append(prefix).AppendLine("do");
                    EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
                    builder.Append(prefix).Append("while (");
                    LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(");");
                    return;
                }
            case CoreNodeKind.VariableDeclarationStatement:
                {
                    var declarationList = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.VariableDeclarationList);
                    if (declarationList is null)
                    {
                        break;
                    }
                    var typeNode = declarationList.Children.FirstOrDefault(item => item.Category == "type");
                    var variables = declarationList.Children.Where(item => item.Kind == CoreNodeKind.VariableDeclaration).ToArray();
                    foreach (var variable in variables)
                    {
                        var name = EmittedLocalIdentifier(variable, variable.Text(CoreProperty.name) ?? "missing");
                        var initializer = variable.Child(CoreChildRole.initializerOffset);
                        var type = typeNode is null ? "var" : MapTypeFromAst(typeNode);
                        if (initializer is not null && ContainsUnboundTypeParameter(type))
                        {
                            var contextualInitializerType = MapType(initializer.StaticType ?? string.Empty);
                            if (!string.IsNullOrEmpty(contextualInitializerType) &&
                                !ContainsUnboundTypeParameter(contextualInitializerType))
                            {
                                type = contextualInitializerType;
                            }
                        }
                        if (typeNode is not null && initializer?.StaticType?.Contains(" Function", StringComparison.Ordinal) == true &&
                            (type.StartsWith("global::System.Action", StringComparison.Ordinal) ||
                             type.StartsWith("global::System.Func", StringComparison.Ordinal)))
                        {
                            // Analyzer-expanded function types are the
                            // authoritative instantiation for a local typedef.
                            // This also prevents an enclosing State<T>
                            // substitution from leaking into ValueChanged<U>.
                            type = MapType(initializer.StaticType);
                        }
                        if (name.StartsWith("defaultExceptionHandler", StringComparison.Ordinal))
                        {
                            type = "global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler";
                        }
                        if ((_session.ActiveDonorDeclaration ?? declaration).Name == "TreeSliver" &&
                            name.StartsWith("__treeRowExtentBuilder", StringComparison.Ordinal))
                        {
                            type = "global::System.Func<TreeSliverNode<T>, global::Doroti.Generated.Framework.Rendering.SliverLayoutDimensions, double?>";
                        }
                        if (declaration.Name is "_RawMenuAnchorState" or "_RawMenuAnchorGroupState" &&
                            name.StartsWith("anchor__", StringComparison.Ordinal))
                        {
                            // Dart exposes the covariant mixin owner through an erased
                            // StatefulWidget view while each concrete state closes it
                            // over a different widget. Keep that one traversal local
                            // dynamic instead of weakening the generated mixin surface.
                            type = "dynamic";
                        }
                        builder.Append(prefix).Append($"{type} {name}");
                        if (initializer is not null)
                        {
                            builder.Append(" = ");
                            var initializerType = MapType(
                                ResolvedExpressionValueType(initializer) ?? initializer.StaticType ?? string.Empty);
                            var needsListConversion = type.StartsWith("List<", StringComparison.Ordinal) &&
                                (initializerType.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
                                 initializerType.StartsWith("List<", StringComparison.Ordinal) ||
                                 IsDartTypedDataList(initializer.StaticType) ||
                                 initializer.Kind == CoreNodeKind.MethodInvocation && initializer.Text(CoreProperty.name) == "cast");
                            var needsMapConversion = type.StartsWith("DartMap<", StringComparison.Ordinal) &&
                                initializerType.StartsWith("DartMap<", StringComparison.Ordinal) &&
                                type != initializerType;
                            var needsCheckedReferenceCast = type != "var" &&
                                !needsListConversion && !needsMapConversion &&
                                type.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                                initializerType.TrimEnd('?') is not ("void") &&
                                !IsValueType(type.TrimEnd('?')) &&
                                !string.Equals(type.TrimEnd('?'), initializerType.TrimEnd('?'), StringComparison.Ordinal);
                            var needsRequiredValue = type != "var" &&
                                IsValueType(type.TrimEnd('?')) &&
                                !type.EndsWith("?", StringComparison.Ordinal) &&
                                initializerType == type + "?";
                            var needsObjectValueCast = type != "var" &&
                                IsValueType(type.TrimEnd('?')) &&
                                initializerType.TrimEnd('?') is "object" or "dynamic";
                            var wrapAwait = initializer.Kind == CoreNodeKind.AwaitExpression &&
                                (needsListConversion || needsMapConversion);
                            var capturesVoid = type != "var" &&
                                type.TrimEnd('?') is not ("void") &&
                                (initializerType.TrimEnd('?') == "void" ||
                                 declaration.Name == "State" && name.StartsWith("result", StringComparison.Ordinal) ||
                                 name.StartsWith("debugCheckForReturnedFuture", StringComparison.Ordinal));
                            if (needsRequiredValue)
                            {
                                builder.Append("DartRuntimePrimitives.RequireValue(");
                            }
                            else if (needsObjectValueCast)
                            {
                                builder.Append('(').Append(type.TrimEnd('?')).Append(")(object)");
                            }
                            else if (needsCheckedReferenceCast)
                            {
                                builder.Append("((").Append(type).Append(")(object?)");
                            }
                            if (wrapAwait) builder.Append('(');
                            if (capturesVoid) builder.Append("DartRuntimePrimitives.CaptureVoid(() => ");
                            var contextualRenderObjectCast = type != "var" &&
                                type.TrimEnd('?') is not ("object" or "dynamic") &&
                                DescendantsAndSelf(initializer).Any(item => item.Text(CoreProperty.name) == "renderObject") &&
                                !type.TrimEnd('?').EndsWith("RenderObject", StringComparison.Ordinal);
                            if (contextualRenderObjectCast)
                            {
                                builder.Append("DartRuntimePrimitives.ConvertValue<").Append(type.TrimEnd('?')).Append(">(");
                            }
                            var previousInitializerReturnType = _session.ContextualLambdaReturnType;
                            if (initializer.Kind == CoreNodeKind.FunctionExpression &&
                                (type == "global::System.Action" || type.StartsWith("global::System.Action<", StringComparison.Ordinal)))
                            {
                                _session.ContextualLambdaReturnType = "void";
                            }
                            var expressionToLower = initializer;
                            if (capturesVoid)
                            {
                                while (expressionToLower.Kind is CoreNodeKind.AsExpression or
                                       CoreNodeKind.ParenthesizedExpression or CoreNodeKind.PostfixExpression)
                                {
                                    var unwrapped = expressionToLower.Child(CoreChildRole.expressionOffset) ??
                                        expressionToLower.Child(CoreChildRole.operandOffset);
                                    if (unwrapped is null) break;
                                    expressionToLower = unwrapped;
                                }
                            }
                            try
                            {
                                LowerExpression(builder, expressionToLower, declaration, package, library, inputPath, diagnostics);
                            }
                            finally
                            {
                                _session.ContextualLambdaReturnType = previousInitializerReturnType;
                            }
                            if (contextualRenderObjectCast) builder.Append(')');
                            if (capturesVoid) builder.Append(')');
                            if (wrapAwait) builder.Append(')');
                            if (needsListConversion)
                            {
                                if (TryGetGenericTypeArguments(type, out var targetArguments) &&
                                    TryGetGenericTypeArguments(initializerType, out var sourceArguments) &&
                                    targetArguments.Length == 1 && sourceArguments.Length == 1 &&
                                    targetArguments[0] != sourceArguments[0])
                                {
                                    builder.Append(".Cast<").Append(targetArguments[0]).Append(">()");
                                }
                                builder.Append(".ToList()");
                            }
                            else if (needsMapConversion)
                            {
                                builder.Append(".cast<").Append(DartMapTypeArguments(type)).Append(">()");
                            }
                            if (needsRequiredValue || needsCheckedReferenceCast) builder.Append(')');
                        }
                        else if (type != "var")
                        {
                            builder.Append(" = default!");
                        }
                        builder.AppendLine(";");
                    }
                    return;
                }
            case CoreNodeKind.PatternVariableDeclarationStatement:
                {
                    var patternDeclaration = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.PatternVariableDeclaration);
                    var pattern = patternDeclaration?.Children.FirstOrDefault(item => item.Category == "pattern");
                    var initializer = patternDeclaration?.Children.FirstOrDefault(item => item.Category == "expression");
                    if (pattern is null || initializer is null)
                    {
                        break;
                    }
                    if (pattern.Kind == CoreNodeKind.ObjectPattern)
                    {
                        var patternValueName = $"__pattern{pattern.Offset}";
                        builder.Append(prefix).Append("var ").Append(patternValueName).Append(" = ");
                        LowerExpression(builder, initializer, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                        foreach (var field in pattern.Children.Where(item => item.Kind == CoreNodeKind.PatternField))
                        {
                            var fieldPattern = field.Children.FirstOrDefault(item => item.Category == "pattern");
                            var variablePattern = fieldPattern is null
                                ? null
                                : DescendantsAndSelf(fieldPattern).FirstOrDefault(item => item.Kind == CoreNodeKind.DeclaredVariablePattern);
                            if (variablePattern is null)
                            {
                                continue;
                            }
                            var fieldName = field.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.PatternFieldName)?
                                .Text(CoreProperty.name)
                                ?? variablePattern.Text(CoreProperty.name)
                                ?? "missing";
                            var variableName = variablePattern.Text(CoreProperty.name) ?? fieldName;
                            var variableTypeNode = variablePattern.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                            var variableType = variableTypeNode is null
                                ? MapType(variablePattern.StaticType ?? string.Empty)
                                : MapTypeFromAst(variableTypeNode);
                            builder.Append(prefix)
                                .Append(string.IsNullOrWhiteSpace(variableType) ? "var" : variableType)
                                .Append(' ')
                                .Append(EmittedLocalIdentifier(variablePattern, variableName))
                                .Append(" = ")
                                .Append(patternValueName)
                                .Append('.')
                                .Append(MapPropertyName(fieldName))
                                .AppendLine(";");
                        }
                        return;
                    }
                    builder.Append(prefix).Append("var ");
                    EmitDeconstructionPattern(builder, pattern, declaration);
                    builder.Append(" = ");
                    LowerExpression(builder, initializer, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                    return;
                }
            case CoreNodeKind.YieldStatement:
                {
                    var expression = node.Children.FirstOrDefault(item => item.Category == "expression");
                    builder.Append(prefix);
                    if (expression is null)
                    {
                        builder.AppendLine("yield break;");
                    }
                    else
                    {
                        builder.Append("yield return ");
                        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                    }
                    return;
                }
            case CoreNodeKind.BreakStatement:
                builder.AppendLine($"{prefix}break;");
                return;
            case CoreNodeKind.ContinueStatement:
                builder.AppendLine($"{prefix}continue;");
                return;
            case CoreNodeKind.TryStatement:
                {
                    var body = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
                    var catchClauses = node.Children.Where(item => item.Kind == CoreNodeKind.CatchClause).ToArray();
                    var finallyClause = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FinallyClause);
                    var directBlocks = node.Children.Where(item => item.Kind == CoreNodeKind.Block).ToArray();
                    var directFinallyBody = catchClauses.Length == 0 && directBlocks.Length > 1 ? directBlocks[^1] : null;
                    if (body is null)
                    {
                        break;
                    }
                    builder.Append(prefix).AppendLine("try");
                    EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
                    foreach (var clause in catchClauses)
                    {
                        var catchTypeNode = clause.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                        var catchType = catchTypeNode is null ? "Exception" : MapTypeFromAst(catchTypeNode);
                        var catchParameters = clause.Children.Where(item => item.Kind == CoreNodeKind.CatchClauseParameter).ToArray();
                        var exceptionParameter = catchParameters.Length > 0 ? catchParameters[0] : null;
                        var stackParameter = catchParameters.Length > 1 ? catchParameters[1] : null;
                        var exceptionName = exceptionParameter is null ? null : exceptionParameter.Text(CoreProperty.name);
                        var stackName = stackParameter is null ? null : stackParameter.Text(CoreProperty.name);
                        var emittedExceptionName = exceptionParameter is null || exceptionName is null
                            ? null
                            : $"{SafeIdentifier(exceptionName)}__{exceptionParameter.Offset}";
                        var emittedStackName = stackParameter is null || stackName is null
                            ? null
                            : $"{SafeIdentifier(stackName)}__{stackParameter.Offset}";
                        var catchBody = clause.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
                        if (catchBody is null)
                        {
                            continue;
                        }
                        if ((exceptionName is null or "_") && catchTypeNode is null)
                        {
                            builder.Append(prefix).AppendLine("catch");
                        }
                        else if (exceptionName is null or "_")
                        {
                            builder.Append(prefix).AppendLine($"catch ({catchType})");
                        }
                        else
                        {
                            builder.Append(prefix).AppendLine($"catch ({catchType} {emittedExceptionName})");
                        }
                        builder.AppendLine($"{prefix}{{");
                        if (stackName is not null and not "_")
                        {
                            builder.AppendLine($"{new string(' ', (indent + 1) * 4)}var {emittedStackName} = new System.Diagnostics.StackTrace();");
                        }
                        EmitBlockBody(builder, catchBody, declaration, package, library, inputPath, diagnostics, indent + 1);
                        builder.AppendLine($"{prefix}}}");
                    }
                    if (finallyClause is not null)
                    {
                        var finallyBody = finallyClause.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
                        if (finallyBody is not null)
                        {
                            builder.Append(prefix).AppendLine("finally");
                            EmitBracedStatement(builder, finallyBody, indent, declaration, package, library, inputPath, diagnostics);
                        }
                    }
                    else if (directFinallyBody is not null)
                    {
                        builder.Append(prefix).AppendLine("finally");
                        EmitBracedStatement(builder, directFinallyBody, indent, declaration, package, library, inputPath, diagnostics);
                    }
                    return;
                }
            case CoreNodeKind.SwitchStatement:
                EmitSwitchStatement(builder, node, indent, declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.FunctionDeclarationStatement:
                EmitLocalFunction(builder, node, indent, declaration, package, library, inputPath, diagnostics);
                return;
        }

        AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
            "statement", "Add a typed statement visitor for this AST node before selecting the containing library.");
    }

    private void EmitAssertMessage(
        CsSyntaxBuilder builder,
        CoreAstNode? message,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (message is null)
        {
            return;
        }

        builder.Append(", () => (object?)");
        LowerExpression(builder, message, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitSwitchExpressionStatement(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var prefix = new string(' ', indent * 4);
        var expression = node.Child(CoreChildRole.expressionOffset) ?? node.Children.FirstOrDefault(item => item.Category == "expression");
        builder.Append(prefix).Append("switch (");
        if (expression is not null) LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine(")");
        builder.AppendLine(prefix + "{");
        foreach (var @case in node.Children.Where(item => item.Kind == CoreNodeKind.SwitchExpressionCase))
        {
            var pattern = @case.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern)
                ?? @case.Children.FirstOrDefault(item => item.Category == "pattern");
            var casePrefix = new string(' ', (indent + 1) * 4);
            if (pattern is not null && IsCatchAllPattern(pattern))
            {
                builder.AppendLine(casePrefix + "default:");
            }
            else
            {
                builder.Append(casePrefix).Append("case ");
                EmitPatternForSwitch(builder, pattern, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(":");
            }
            var result = @case.Child(CoreChildRole.expressionOffset)
                ?? @case.Children.LastOrDefault(item => item.Category == "expression");
            if (result is not null && result.Kind != CoreNodeKind.NullLiteral)
            {
                builder.Append(new string(' ', (indent + 2) * 4));
                var resultBuilder = new CsSyntaxBuilder();
                LowerExpression(resultBuilder, result, declaration, package, library, inputPath, diagnostics);
                var resultSyntax = resultBuilder.Build();
                var effectivePattern = pattern?.Kind == CoreNodeKind.GuardedPattern
                    ? pattern.Children.FirstOrDefault(item => item.Category == "pattern")
                    : pattern;
                if (expression?.Kind == CoreNodeKind.SimpleIdentifier && effectivePattern?.Kind == CoreNodeKind.ObjectPattern)
                {
                    resultSyntax = resultSyntax.RenameIdentifier(
                        EmittedLocalIdentifier(expression, expression.Text(CoreProperty.name) ?? "value"),
                        $"__object{effectivePattern.Offset}",
                        renameAssignments: false);
                }
                builder.Append(resultSyntax);
                builder.AppendLine(";");
            }
            builder.AppendLine(new string(' ', (indent + 2) * 4) + "break;");
        }
        builder.AppendLine(prefix + "}");
    }

    private void EmitSwitchStatement(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var prefix = new string(' ', indent * 4);
        var expression = node.Child(CoreChildRole.expressionOffset) ?? node.Children.FirstOrDefault(item => item.Category == "expression");
        builder.Append(prefix).Append("switch (");
        if (expression is not null)
        {
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
        }
        builder.AppendLine(")");
        builder.AppendLine($"{prefix}{{");
        var switchMembers = node.Children.Where(item => item.Kind is CoreNodeKind.SwitchCase or CoreNodeKind.SwitchPatternCase or CoreNodeKind.SwitchDefault).ToArray();
        var pendingCaseBodyRenames = new Dictionary<int, Dictionary<string, string>>();
        for (var memberIndex = 0; memberIndex < switchMembers.Length; memberIndex++)
        {
            var member = switchMembers[memberIndex];
            var casePrefix = new string(' ', (indent + 1) * 4);
            var pattern = member.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern)
                ?? member.Children.FirstOrDefault(item => item.Category is "pattern" or "expression");
            var statements = member.Children.Where(item => item.Category == "statement").ToArray();
            if (member.Kind != CoreNodeKind.SwitchDefault && statements.Length == 0 &&
                memberIndex + 1 < switchMembers.Length &&
                switchMembers[memberIndex + 1] is { Kind: not CoreNodeKind.SwitchDefault } nextMember)
            {
                var nextPattern = nextMember.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern)
                    ?? nextMember.Children.FirstOrDefault(item => item.Category is "pattern" or "expression");
                var nextStatements = nextMember.Children.Where(item => item.Category == "statement").ToArray();
                var bindings = pattern is null ? [] : DescendantsAndSelf(pattern)
                    .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern)
                    .GroupBy(item => item.Text(CoreProperty.name) ?? "value", StringComparer.Ordinal)
                    .ToDictionary(group => group.Key, group => group.OrderBy(item => item.Offset).First(), StringComparer.Ordinal);
                var nextBindings = nextPattern is null ? [] : DescendantsAndSelf(nextPattern)
                    .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern)
                    .GroupBy(item => item.Text(CoreProperty.name) ?? "value", StringComparer.Ordinal)
                    .ToDictionary(group => group.Key, group => group.OrderBy(item => item.Offset).First(), StringComparer.Ordinal);
                if (bindings.Count > 0 && bindings.Keys.OrderBy(item => item, StringComparer.Ordinal)
                        .SequenceEqual(nextBindings.Keys.OrderBy(item => item, StringComparer.Ordinal), StringComparer.Ordinal) &&
                    nextStatements.Length > 0)
                {
                    statements = nextStatements;
                    pendingCaseBodyRenames[memberIndex + 1] = bindings.ToDictionary(
                        item => EmittedLocalIdentifier(item.Value, item.Key),
                        item => EmittedLocalIdentifier(nextBindings[item.Key], item.Key),
                        StringComparer.Ordinal);
                }
            }
            if (member.Kind == CoreNodeKind.SwitchDefault || pattern is not null && IsCatchAllPattern(pattern))
            {
                builder.AppendLine($"{casePrefix}default:");
            }
            else
            {
                builder.Append(casePrefix).Append("case ");
                EmitPatternForSwitch(builder, pattern, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(":");
            }

            if (statements.Length == 0)
            {
                // Dart empty switch cases fall through to the next case. C#
                // represents the same construct with adjacent case labels.
                if (memberIndex == switchMembers.Length - 1)
                {
                    builder.AppendLine($"{new string(' ', (indent + 2) * 4)}break;");
                }
                continue;
            }
            builder.AppendLine($"{new string(' ', (indent + 2) * 4)}{{");
            var caseBody = new CsSyntaxBuilder();
            foreach (var statement in statements)
            {
                LowerStatement(caseBody, statement, indent + 3, declaration, package, library, inputPath, diagnostics);
            }
            var renderedCaseBody = caseBody.Build();
            if (pendingCaseBodyRenames.TryGetValue(memberIndex, out var caseBodyRenames))
            {
                foreach (var rename in caseBodyRenames)
                {
                    renderedCaseBody = renderedCaseBody.RenameIdentifier(rename.Key, rename.Value, renameAssignments: false);
                }
            }
            foreach (var variable in statements
                .SelectMany(statement => DescendantsAndSelf(statement))
                .Where(item => item.Kind == CoreNodeKind.VariableDeclaration))
            {
                var name = variable.Text(CoreProperty.name);
                if (!string.IsNullOrEmpty(name))
                {
                    renderedCaseBody = renderedCaseBody.RenameIdentifier(SafeIdentifier(name), $"{SafeIdentifier(name)}__{variable.Offset}");
                }
            }
            builder.Append(renderedCaseBody);
            if (statements.Length > 0 && !IsTerminatingStatement(statements[^1]))
            {
                builder.AppendLine($"{new string(' ', (indent + 3) * 4)}break;");
            }
            builder.AppendLine($"{new string(' ', (indent + 2) * 4)}}}");
        }
        var hasDefault = switchMembers.Any(member => member.Kind == CoreNodeKind.SwitchDefault ||
            member.Children.Any(item => item.Category == "pattern" && IsCatchAllPattern(item)));
        var nonEmptyCaseStatements = switchMembers
            .Select(member => member.Children.Where(item => item.Category == "statement").ToArray())
            .Where(statements => statements.Length > 0)
            .ToArray();
        if (!hasDefault && nonEmptyCaseStatements.Length > 0 &&
            nonEmptyCaseStatements.All(statements => IsTerminatingStatement(statements[^1])))
        {
            builder.AppendLine($"{new string(' ', (indent + 1) * 4)}default:");
            builder.AppendLine($"{new string(' ', (indent + 2) * 4)}throw new InvalidOperationException(\"Non-exhaustive Dart switch value.\");");
        }
        builder.AppendLine($"{prefix}}}");
    }

    private bool IsTerminatingStatement(CoreAstNode statement) => statement.Kind switch
    {
        CoreNodeKind.ReturnStatement or CoreNodeKind.BreakStatement or CoreNodeKind.ContinueStatement => true,
        CoreNodeKind.ExpressionStatement => statement.Children.Any(item => item.Kind is CoreNodeKind.ThrowExpression or CoreNodeKind.RethrowExpression),
        _ => false,
    };

    private void EmitLocalFunction(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var prefix = new string(' ', indent * 4);
        var function = node.Child(CoreChildRole.functionOffset) ?? node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FunctionDeclaration);
        var functionExpression = function?.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FunctionExpression);
        if (function is null || functionExpression is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "local-function-shape", "Resolve the local function declaration and signature.");
            return;
        }
        var returnType = MapType(node.Text(CoreProperty.returnType) ?? "void");
        var dartName = function.Text(CoreProperty.name) ?? "localFunction";
        var name = SafeIdentifier(dartName);
        var parameterCount = int.TryParse(node.Text(CoreProperty.parameterCount), out var count) ? count : 0;
        var formalParameters = functionExpression.Child(CoreChildRole.parametersOffset)?.Children
            .Where(item => item.Category == "parameter")
            .ToArray() ?? [];
        var parameters = Enumerable.Range(0, parameterCount)
            .Select(index =>
            {
                var suffix = string.Empty;
                if (index < formalParameters.Length && formalParameters[index].Kind == CoreNodeKind.DefaultFormalParameter)
                {
                    var defaultExpression = formalParameters[index].Children.FirstOrDefault(item => item.Category == "expression");
                    suffix = defaultExpression?.Kind switch
                    {
                        CoreNodeKind.BooleanLiteral => " = " + (defaultExpression.Text(CoreProperty.value) ?? "false"),
                        CoreNodeKind.IntegerLiteral or CoreNodeKind.DoubleLiteral => " = " + (defaultExpression.Text(CoreProperty.value) ?? "0"),
                        CoreNodeKind.NullLiteral => " = null",
                        _ => string.Empty,
                    };
                    if (defaultExpression is null &&
                        MapType(node.ParameterType(index) ?? "object").EndsWith("?", StringComparison.Ordinal))
                    {
                        suffix = " = null";
                    }
                }
                return $"{MapType(node.ParameterType(index) ?? "object")} {SafeIdentifier(node.ParameterName(index) ?? $"arg{index}")}{suffix}";
            })
            .ToArray();
        var isAsync = IsDartAsync(functionExpression) && IsFutureType(returnType);
        builder.Append(prefix).Append(isAsync ? "async " : string.Empty)
            .Append(returnType).Append(' ').Append(name).Append('(').Append(string.Join(", ", parameters)).AppendLine(")");
        builder.AppendLine($"{prefix}{{");
        var body = DescendantsAndSelf(functionExpression).FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
        if (body is not null)
        {
            var previousReturnType = _session.ActiveFunctionReturnType;
            _session.ActiveFunctionReturnType = returnType;
            try
            {
                EmitBlockBody(builder, body, declaration, package, library, inputPath, diagnostics, indent + 1);
            }
            finally
            {
                _session.ActiveFunctionReturnType = previousReturnType;
            }
        }
        else
        {
            var expressionBody = DescendantsAndSelf(functionExpression)
                .FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
            var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
            if (expression is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                    "local-function-body", "Resolve the local function body.");
            }
            else
            {
                builder.Append(new string(' ', (indent + 1) * 4));
                if (returnType != "void")
                {
                    builder.Append("return ");
                }
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
        }
        if (returnType != "void" && !(isAsync && returnType == "Future"))
        {
            builder.Append(new string(' ', (indent + 1) * 4))
                .AppendLine("throw new InvalidOperationException(\"Dart control flow completed without a value.\");");
        }
        builder.AppendLine($"{prefix}}}");
        if (dartName == "toString")
        {
            var argumentNames = Enumerable.Range(0, parameterCount)
                .Select(index => SafeIdentifier(node.ParameterName(index) ?? $"arg{index}"));
            builder.Append(prefix).Append(returnType).Append(" ToString(")
                .Append(string.Join(", ", parameters)).Append(") => ")
                .Append(name).Append('(').Append(string.Join(", ", argumentNames)).AppendLine(");");
        }
    }

    private void EmitBracedStatement(
        CsSyntaxBuilder builder,
        CoreAstNode statement,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var prefix = new string(' ', indent * 4);
        if (statement.Kind == CoreNodeKind.Block)
        {
            builder.AppendLine($"{prefix}{{");
            EmitBlockBodyWithUniqueLocals(builder, statement, declaration, package, library, inputPath, diagnostics, indent + 1);
            builder.AppendLine($"{prefix}}}");
        }
        else
        {
            builder.AppendLine($"{prefix}{{");
            LowerStatement(builder, statement, indent + 1, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine($"{prefix}}}");
        }
    }

    private void CollectConditionPromotions(
        CoreAstNode condition,
        CoreResolvedDeclaration declaration,
        List<(string Name, string Local, string Type, bool RequireValue)> promotions)
    {
        if (condition.Kind == CoreNodeKind.ParenthesizedExpression &&
            condition.Child(CoreChildRole.expressionOffset) is { } nested)
        {
            CollectConditionPromotions(nested, declaration, promotions);
            return;
        }
        if (condition.Kind == CoreNodeKind.BinaryExpression && condition.Text(CoreProperty.@operator) == "&&")
        {
            if (condition.Child(CoreChildRole.leftOffset) is { } left) CollectConditionPromotions(left, declaration, promotions);
            if (condition.Child(CoreChildRole.rightOffset) is { } right) CollectConditionPromotions(right, declaration, promotions);
            return;
        }
        if (condition.Kind == CoreNodeKind.BinaryExpression && condition.Text(CoreProperty.@operator) == "!=" &&
            condition.Child(CoreChildRole.leftOffset) is { Kind: CoreNodeKind.SimpleIdentifier } nullableExpression &&
            condition.Child(CoreChildRole.rightOffset) is { Kind: CoreNodeKind.NullLiteral } &&
            nullableExpression.StaticType is { } nullableType && IsValueType(MapType(nullableType).TrimEnd('?')) &&
            IsLocalOrParameter(nullableExpression, declaration))
        {
            var name = EmittedLocalIdentifier(nullableExpression, nullableExpression.Text(CoreProperty.name) ?? "value");
            promotions.Add((name, $"{name}__value{condition.Offset}", MapType(nullableType).TrimEnd('?'), true));
            return;
        }
        if (condition.Kind == CoreNodeKind.IsExpression && condition.Text(CoreProperty.@operator) != "is!" &&
            condition.Text(CoreProperty.isNot) != "true" &&
            condition.Child(CoreChildRole.expressionOffset) is { Kind: CoreNodeKind.SimpleIdentifier } promotedExpression &&
            condition.Child(CoreChildRole.typeOffset) is { } promotedType &&
            IsLocalOrParameter(promotedExpression, declaration))
        {
            var name = EmittedLocalIdentifier(promotedExpression, promotedExpression.Text(CoreProperty.name) ?? "value");
            var mappedType = MapTypeFromAst(promotedType).TrimEnd('?');
            promotions.Add((name, $"{name}__as{condition.Offset}", mappedType, false));
        }
    }

    private void EmitPromotedBracedStatement(
        CsSyntaxBuilder builder,
        CoreAstNode statement,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        IReadOnlyList<(string Name, string Local, string Type, bool RequireValue)> promotions)
    {
        var prefix = new string(' ', indent * 4);
        builder.AppendLine($"{prefix}{{");
        foreach (var promotion in promotions.DistinctBy(item => item.Name))
        {
            builder.Append(prefix).Append("    ").Append(promotion.Type).Append(' ').Append(promotion.Local).Append(" = ");
            if (promotion.RequireValue)
            {
                builder.Append("DartRuntimePrimitives.RequireValue(").Append(promotion.Name).Append(')');
            }
            else
            {
                builder.Append('(').Append(promotion.Type).Append(')');
                if (IsTypeParameter(promotion.Type, declaration)) builder.Append("(object)");
                builder.Append(promotion.Name);
            }
            builder.AppendLine(";");
        }

        var bodyBuilder = new CsSyntaxBuilder();
        if (statement.Kind == CoreNodeKind.Block)
        {
            EmitBlockBodyWithUniqueLocals(bodyBuilder, statement, declaration, package, library, inputPath, diagnostics, indent + 1);
        }
        else
        {
            LowerStatement(bodyBuilder, statement, indent + 1, declaration, package, library, inputPath, diagnostics);
        }
        var body = bodyBuilder.Build();
        foreach (var promotion in promotions.DistinctBy(item => item.Name))
        {
            body = body.RenameIdentifier(promotion.Name, promotion.Local, renameAssignments: false);
        }
        builder.Append(body);
        builder.AppendLine($"{prefix}}}");
    }

    private void EmitForEach(
        CsSyntaxBuilder builder,
        string prefix,
        CoreAstNode loopParts,
        CoreAstNode body,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (loopParts.Kind == CoreNodeKind.ForEachPartsWithPattern)
        {
            var pattern = loopParts.Children.FirstOrDefault(item => item.Category == "pattern");
            var patternIterable = loopParts.Children.FirstOrDefault(item => item.Category == "expression");
            if (pattern is null || patternIterable is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, loopParts,
                    "for-each-pattern-shape", "Provide the typed foreach pattern and iterable.");
                return;
            }
            builder.Append(prefix).Append("foreach (var ");
            EmitDeconstructionPattern(builder, pattern, declaration);
            builder.Append(" in ");
            LowerExpression(builder, patternIterable, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine(")");
            EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
            return;
        }

        var loopVariable = loopParts.Children.FirstOrDefault(item => item.Kind is CoreNodeKind.DeclaredIdentifier or CoreNodeKind.SimpleIdentifier);
        var iterable = loopParts.Children.FirstOrDefault(item => item != loopVariable && item.Category == "expression");
        var name = loopVariable?.Kind switch
        {
            CoreNodeKind.DeclaredIdentifier => loopVariable.Text(CoreProperty.name),
            CoreNodeKind.SimpleIdentifier => loopVariable.Text(CoreProperty.name),
            _ => null
        };
        if (name is null || iterable is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, loopParts,
                "for-each-shape", "Provide the typed foreach variable and iterable.");
            return;
        }
        var typeNode = loopVariable?.Kind == CoreNodeKind.DeclaredIdentifier
            ? loopVariable.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType)
            : null;
        var type = typeNode is null ? "var" : MapTypeFromAst(typeNode);
        builder.Append(prefix).Append($"foreach ({type} {EmittedLocalIdentifier(loopVariable!, name)} in ");
        LowerExpression(builder, iterable, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine(")");
        EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitForLoop(
        CsSyntaxBuilder builder,
        string prefix,
        CoreAstNode loopParts,
        CoreAstNode body,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var variableList = loopParts.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.VariableDeclarationList);
        var expressions = loopParts.Children.Where(item => item.Category == "expression").ToArray();
        var initExpression = variableList is null ? loopParts.Child(CoreChildRole.initializerOffset) : null;
        var condition = loopParts.Child(CoreChildRole.conditionOffset);
        if (initExpression is null && condition is null)
        {
            // Backward compatibility with older analyzer payloads.
            initExpression = variableList is null ? expressions.FirstOrDefault() : null;
            condition = variableList is null ? expressions.Skip(1).FirstOrDefault() : expressions.FirstOrDefault();
        }
        var updaters = expressions.Where(item => item.Offset != initExpression?.Offset && item.Offset != condition?.Offset).ToArray();
        builder.Append(prefix).Append("for (");
        if (variableList is not null)
        {
            EmitVariableDeclarationList(builder, variableList, declaration, package, library, inputPath, diagnostics);
        }
        else if (initExpression is not null)
        {
            LowerExpression(builder, initExpression, declaration, package, library, inputPath, diagnostics);
        }
        builder.Append("; ");
        if (condition is not null)
        {
            LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
        }
        builder.Append("; ");
        if (updaters.Length > 0)
        {
            for (var index = 0; index < updaters.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                LowerExpression(builder, updaters[index], declaration, package, library, inputPath, diagnostics);
            }
        }
        builder.AppendLine(")");
        EmitBracedStatement(builder, body, indent, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitVariableDeclarationList(
        CsSyntaxBuilder builder,
        CoreAstNode variableList,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var typeNode = variableList.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
        var variables = variableList.Children.Where(item => item.Kind == CoreNodeKind.VariableDeclaration).ToArray();
        var type = typeNode is null ? "var" : MapTypeFromAst(typeNode);
        builder.Append(type).Append(' ');
        for (var index = 0; index < variables.Length; index++)
        {
            if (index > 0)
            {
                builder.Append(", ");
            }
            var variable = variables[index];
            var name = EmittedLocalIdentifier(variable, variable.Text(CoreProperty.name) ?? "missing");
            var initializer = variable.Child(CoreChildRole.initializerOffset);
            builder.Append(name);
            if (initializer is not null)
            {
                builder.Append(" = ");
                var initializerType = MapType(
                    ResolvedExpressionValueType(initializer) ?? initializer.StaticType ?? string.Empty);
                var needsListConversion = type.StartsWith("List<", StringComparison.Ordinal) &&
                    (initializerType.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
                     initializerType.StartsWith("List<", StringComparison.Ordinal) ||
                     IsDartTypedDataList(initializer.StaticType) ||
                     initializer.Kind == CoreNodeKind.MethodInvocation && initializer.Text(CoreProperty.name) == "cast");
                var needsMapConversion = type.StartsWith("DartMap<", StringComparison.Ordinal) &&
                    initializerType.StartsWith("DartMap<", StringComparison.Ordinal) &&
                    type != initializerType;
                var wrapAwait = initializer.Kind == CoreNodeKind.AwaitExpression &&
                    (needsListConversion || needsMapConversion);
                if (wrapAwait) builder.Append('(');
                var previousInitializerReturnType = _session.ContextualLambdaReturnType;
                if (initializer.Kind == CoreNodeKind.FunctionExpression &&
                    (type == "global::System.Action" || type.StartsWith("global::System.Action<", StringComparison.Ordinal)))
                {
                    _session.ContextualLambdaReturnType = "void";
                }
                try
                {
                    LowerExpression(builder, initializer, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ContextualLambdaReturnType = previousInitializerReturnType;
                }
                if (wrapAwait) builder.Append(')');
                if (needsListConversion)
                {
                    if (TryGetGenericTypeArguments(type, out var targetArguments) &&
                        TryGetGenericTypeArguments(initializerType, out var sourceArguments) &&
                        targetArguments.Length == 1 && sourceArguments.Length == 1 &&
                        targetArguments[0] != sourceArguments[0])
                    {
                        builder.Append(".Cast<").Append(targetArguments[0]).Append(">()");
                    }
                    builder.Append(".ToList()");
                }
                else if (needsMapConversion)
                {
                    builder.Append(".cast<").Append(DartMapTypeArguments(type)).Append(">()");
                }
            }
            else if (type != "var")
            {
                builder.Append(" = default!");
            }
        }
    }

    private void EmitLambda(
        CsSyntaxBuilder builder,
        CoreAstNode function,
        CoreAstNode? invocationArguments,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        builder.AppendLine("() =>");
        builder.Append(new string(' ', (indent + 1) * 4)).AppendLine("{");
        var body = DescendantsAndSelf(function).FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
        if (body is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, function,
                "closure-body", "Add typed expression-body closure lowering.");
        }
        else
        {
            var previousReturnType = _session.ActiveFunctionReturnType;
            _session.ActiveFunctionReturnType = "bool";
            try
            {
                var formalParameters = function.Children
                    .FirstOrDefault(item => item.Kind == CoreNodeKind.FormalParameterList)?
                    .Children.Where(item => item.Category == "parameter").ToArray() ?? [];
                var argumentValues = invocationArguments?.Children
                    .Where(item => item.Category == "expression").ToArray() ?? [];
                for (var index = 0; index < formalParameters.Length && index < argumentValues.Length; index++)
                {
                    var parameter = formalParameters[index];
                    var parameterName = SafeIdentifier(parameter.Text(CoreProperty.name) ?? $"arg{index}");
                    var parameterTypeNode = parameter.Children.FirstOrDefault(item => item.Category == "type");
                    var parameterType = parameterTypeNode is null
                        ? MapType(parameter.StaticType ?? "object")
                        : MapTypeFromAst(parameterTypeNode);
                    builder.Append(new string(' ', (indent + 2) * 4))
                        .Append(parameterType)
                        .Append(' ').Append(parameterName).Append(" = ");
                    LowerExpression(builder, argumentValues[index], declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                }
                var statements = body.Children.Where(item => item.Category == "statement").ToArray();
                foreach (var statement in statements)
                {
                    LowerStatement(builder, statement, indent + 2, declaration, package, library, inputPath, diagnostics);
                }
                if (statements.Length == 0 || !IsTerminatingStatement(statements[^1]))
                {
                    builder.Append(new string(' ', (indent + 2) * 4))
                        .AppendLine("throw new InvalidOperationException(\"Dart closure completed without a value.\");");
                }
            }
            finally
            {
                _session.ActiveFunctionReturnType = previousReturnType;
            }
        }
        builder.Append(new string(' ', (indent + 1) * 4)).Append('}');
    }

}
