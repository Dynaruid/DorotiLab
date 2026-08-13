using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitSwitchExpression(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var expression = node.Child(CoreChildRole.expressionOffset) ?? node.Children.FirstOrDefault(item => item.Category == "expression");
        var cases = node.Children.Where(item => item.Kind == CoreNodeKind.SwitchExpressionCase).ToArray();
        var mappedSwitchResultType = MapType(node.StaticType ?? string.Empty);
        var targetTypeRecordArms = mappedSwitchResultType.StartsWith('(') && mappedSwitchResultType.EndsWith(')');
        var patternExtensionDeclaration = declaration.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration
            ? declaration
            : FindGlobalDeclaration(expression?.StaticType?.TrimEnd('?') ?? string.Empty);
        var representation = patternExtensionDeclaration?.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration
            ? patternExtensionDeclaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.RepresentationDeclaration)
            : null;
        var representationName = representation is null
            ? null
            : SafeIdentifier(representation.Text(CoreProperty.name) ?? "value");
        var unwrapExtensionValue = representationName is not null &&
            cases.SelectMany(DescendantsAndSelf).Any(item => item.Kind is CoreNodeKind.RecordPattern or CoreNodeKind.NullCheckPattern);
        var unwrapExtensionRecord = unwrapExtensionValue && expression?.Kind == CoreNodeKind.RecordLiteral;
        var previousPatternExtensionTypeName = _session.ActivePatternExtensionTypeName;
        if (unwrapExtensionValue)
        {
            _session.ActivePatternExtensionTypeName = patternExtensionDeclaration!.Name;
        }
        builder.Append('(');
        if (expression is not null)
        {
            if (unwrapExtensionRecord)
            {
                var fields = expression.Children.Where(item => item.Category == "expression").ToArray();
                builder.Append('(');
                for (var index = 0; index < fields.Length; index++)
                {
                    if (index > 0) builder.Append(", ");
                    var field = fields[index].Kind == CoreNodeKind.NamedExpression
                        ? fields[index].Children.FirstOrDefault(item => item.Category == "expression") ?? fields[index]
                        : fields[index];
                    var fieldType = StripLibraryPrefix(field.StaticType ?? string.Empty).TrimEnd('?');
                    var unwrapField = field.Kind == CoreNodeKind.ThisExpression ||
                        string.Equals(fieldType, patternExtensionDeclaration!.Name, StringComparison.Ordinal);
                    if (unwrapField) builder.Append('(');
                    LowerExpression(builder, field, declaration, package, library, inputPath, diagnostics);
                    if (unwrapField) builder.Append(").").Append(representationName);
                }
                builder.Append(')');
            }
            else if (unwrapExtensionValue)
            {
                builder.Append('(');
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.Append(").").Append(representationName);
            }
            else
            {
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            }
        }
        builder.Append(" switch { ");
        var switchInputName = expression?.Kind == CoreNodeKind.SimpleIdentifier
            ? EmittedLocalIdentifier(expression, expression.Text(CoreProperty.name) ?? string.Empty)
            : null;
        var emittedArmCount = 0;
        for (var index = 0; index < cases.Length; index++)
        {
            var @case = cases[index];
            var pattern = @case.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern)
                ?? @case.Children.FirstOrDefault(item => item.Category == "pattern");
            var effectivePattern = pattern?.Kind == CoreNodeKind.GuardedPattern
                ? pattern.Children.FirstOrDefault(item => item.Category == "pattern")
                : pattern;
            var expansionPattern = effectivePattern;
            while (expansionPattern?.Kind == CoreNodeKind.ParenthesizedPattern)
            {
                expansionPattern = expansionPattern.Children.FirstOrDefault(item => item.Category == "pattern");
            }
            var guardedHasWhenClause = pattern?.Kind == CoreNodeKind.GuardedPattern &&
                pattern.Children.Any(item => item.Kind == CoreNodeKind.WhenClause);
            var armPatterns = !guardedHasWhenClause &&
                expansionPattern?.Kind == CoreNodeKind.LogicalOrPattern
                ? expansionPattern.Children.Where(item => item.Category == "pattern").ToArray()
                : pattern is null ? [] : [pattern];
            var result = @case.Child(CoreChildRole.expressionOffset)
                ?? @case.Children.LastOrDefault(item => item.Category == "expression");
            foreach (var armPattern in armPatterns)
            {
                if (emittedArmCount++ > 0)
                {
                    builder.Append(", ");
                }
                var caseBuilder = new CsSyntaxBuilder();
                EmitPatternForSwitch(caseBuilder, armPattern, declaration, package, library, inputPath, diagnostics);
                caseBuilder.Append(" => ");
                if (result is not null)
                {
                    if (targetTypeRecordArms) caseBuilder.Append("((").Append(mappedSwitchResultType).Append(")(");
                    var mappedResultType = MapType(result.StaticType ?? string.Empty).TrimEnd('?');
                    if (result.Kind == CoreNodeKind.NullLiteral &&
                        (IsTypeParameter(mappedSwitchResultType, _session.ActiveDonorDeclaration ?? declaration) ||
                         Regex.IsMatch(mappedSwitchResultType, @"^[A-Z]\??$", RegexOptions.CultureInvariant)))
                    {
                        caseBuilder.Append("default");
                    }
                    else if (mappedResultType == "void")
                    {
                        caseBuilder.Append("DartRuntimePrimitives.CaptureVoid(() => ");
                        LowerExpression(caseBuilder, result, declaration, package, library, inputPath, diagnostics);
                        caseBuilder.Append(')');
                    }
                    else
                    {
                        if (mappedSwitchResultType.TrimEnd('?') is "object" or "dynamic")
                        {
                            caseBuilder.Append("(object?)");
                            LowerExpression(caseBuilder, result, declaration, package, library, inputPath, diagnostics);
                        }
                        else if (mappedSwitchResultType.Length > 0 &&
                            mappedSwitchResultType.TrimEnd('?') != "void" &&
                            result.Kind is not (CoreNodeKind.ThrowExpression or CoreNodeKind.RethrowExpression) &&
                            !string.Equals(mappedSwitchResultType.TrimEnd('?'), mappedResultType, StringComparison.Ordinal))
                        {
                            caseBuilder.Append("DartRuntimePrimitives.ConvertValue<")
                                .Append(mappedSwitchResultType.TrimEnd('?')).Append(">(");
                            LowerExpression(caseBuilder, result, declaration, package, library, inputPath, diagnostics);
                            caseBuilder.Append(')');
                        }
                        else
                        {
                            LowerExpression(caseBuilder, result, declaration, package, library, inputPath, diagnostics);
                        }
                    }
                    if (targetTypeRecordArms) caseBuilder.Append("))");
                }
                else
                {
                    caseBuilder.Append("default");
                }
                var caseSyntax = caseBuilder.Build();
                if (expansionPattern?.Kind == CoreNodeKind.LogicalOrPattern &&
                    armPatterns.Length > 0 &&
                    !ReferenceEquals(armPattern, armPatterns[0]))
                {
                    var sharedDeclarations = DescendantsAndSelf(armPatterns[0])
                        .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern)
                        .GroupBy(item => item.Text(CoreProperty.name) ?? "value", StringComparer.Ordinal)
                        .ToDictionary(group => group.Key, group => group.OrderBy(item => item.Offset).First(), StringComparer.Ordinal);
                    foreach (var branchDeclaration in DescendantsAndSelf(armPattern)
                        .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern))
                    {
                        var bindingName = branchDeclaration.Text(CoreProperty.name) ?? "value";
                        if (sharedDeclarations.TryGetValue(bindingName, out var sharedDeclaration))
                        {
                            // Dart binds one logical variable across the alternatives,
                            // but expanded C# switch arms need the result expression to
                            // refer to the declaration emitted for the current branch.
                            caseSyntax = caseSyntax.RenameIdentifier(
                                EmittedLocalIdentifier(sharedDeclaration, bindingName),
                                EmittedLocalIdentifier(branchDeclaration, bindingName),
                                renameAssignments: false);
                        }
                    }
                }
                var armEffectivePattern = armPattern.Kind == CoreNodeKind.GuardedPattern
                    ? armPattern.Children.FirstOrDefault(item => item.Category == "pattern") ?? armPattern
                    : armPattern;
                if (switchInputName is { Length: > 0 } && armEffectivePattern.Kind == CoreNodeKind.ObjectPattern)
                {
                    caseSyntax = caseSyntax.RenameIdentifier(switchInputName, $"__object{armEffectivePattern.Offset}", renameAssignments: false);
                }
                if (switchInputName is { Length: > 0 } &&
                    armEffectivePattern.Kind == CoreNodeKind.WildcardPattern &&
                    armEffectivePattern.Children.Any(item => item.Kind == CoreNodeKind.NamedType))
                {
                    caseSyntax = caseSyntax.RenameIdentifier(switchInputName, $"__typed{armEffectivePattern.Offset}", renameAssignments: false);
                }
                foreach (var declared in DescendantsAndSelf(armPattern)
                    .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern))
                {
                    if (declared.ElementId?.Contains("@local", StringComparison.Ordinal) == true)
                    {
                        continue;
                    }
                    var declaredName = SafeIdentifier(declared.Text(CoreProperty.name) ?? "value");
                    caseSyntax = caseSyntax.RenameIdentifier(declaredName, $"{declaredName}__pattern{declared.Offset}", renameAssignments: false);
                }
                builder.Append(caseSyntax);
            }
        }
        // C# enums admit unnamed numeric values, unlike Dart enums, so retain a
        // defensive wildcard for non-record switches. Analyzer-valid record/tuple
        // switches are already exhaustive and a synthetic arm is unreachable.
        var expressionType = expression?.StaticType?.TrimStart() ?? string.Empty;
        var recordNeedsClrEnumFallback = expressionType.StartsWith('(') &&
            expressionType.Contains("TextDirection", StringComparison.Ordinal);
        if (!unwrapExtensionValue &&
            (!expressionType.StartsWith('(') || recordNeedsClrEnumFallback) &&
            !cases.Any(IsCatchAllSwitchCase) &&
            !HasObjectAndNullCatchAll(cases) &&
            !ClosedNullableSwitchIsExhaustive(expression, cases))
        {
            if (emittedArmCount > 0) builder.Append(", ");
            builder.Append("_ => throw new InvalidOperationException(\"Non-exhaustive Dart switch value.\")");
        }
        builder.Append(" })");
        _session.ActivePatternExtensionTypeName = previousPatternExtensionTypeName;
    }

    private bool IsCatchAllSwitchCase(CoreAstNode switchCase)
    {
        var guarded = switchCase.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern);
        var pattern = guarded?.Children.FirstOrDefault(item => item.Category == "pattern")
            ?? switchCase.Children.FirstOrDefault(item => item.Category == "pattern");
        if (guarded?.Children.Any(item => item.Kind == CoreNodeKind.WhenClause) == true)
        {
            return false;
        }
        return pattern is not null && IsCatchAllPattern(pattern);
    }

    private bool HasObjectAndNullCatchAll(CoreAstNode[] cases)
    {
        var hasObject = cases.Any(switchCase => DescendantsAndSelf(switchCase).Any(node =>
            node.Kind == CoreNodeKind.NamedType &&
            node.Text(CoreProperty.name) is "Object" or "object" or "dynamic"));
        var hasNull = cases.Any(switchCase => DescendantsAndSelf(switchCase).Any(node =>
            node.Kind == CoreNodeKind.NullLiteral));
        return hasObject && hasNull;
    }

    private bool IsCatchAllPattern(CoreAstNode pattern)
    {
        while (pattern.Kind is CoreNodeKind.ParenthesizedPattern or CoreNodeKind.GuardedPattern &&
               pattern.Children.FirstOrDefault(item => item.Category == "pattern") is { } nested)
        {
            pattern = nested;
        }
        if (pattern.Kind == CoreNodeKind.WildcardPattern)
        {
            return !pattern.Children.Any(item => item.Kind == CoreNodeKind.NamedType);
        }
        if (string.Equals(pattern.Text(CoreProperty.name), "_", StringComparison.Ordinal))
        {
            return true;
        }
        if (pattern.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(pattern.Text(CoreProperty.name), "_", StringComparison.Ordinal))
        {
            return true;
        }
        if (pattern.Kind is CoreNodeKind.AssignedVariablePattern or CoreNodeKind.DeclaredVariablePattern &&
            !pattern.Children.Any(item => item.Kind == CoreNodeKind.NamedType) &&
            pattern.Children.Any(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                string.Equals(item.Text(CoreProperty.name), "_", StringComparison.Ordinal)))
        {
            return true;
        }
        if (pattern.Kind == CoreNodeKind.DeclaredVariablePattern)
        {
            return !pattern.Children.Any(item => item.Kind == CoreNodeKind.NamedType);
        }
        if (pattern.Kind == CoreNodeKind.RecordPattern)
        {
            var fields = pattern.Children.SelectMany(item => item.Category == "pattern"
                ? [item]
                : item.Children.Where(child => child.Category == "pattern")).ToArray();
            return fields.Length > 0 && fields.All(IsCatchAllPattern);
        }
        return false;
    }

    private bool ClosedRecordSwitchIsExhaustive(CoreAstNode? expression, CoreAstNode[] cases)
    {
        var staticType = expression?.StaticType?.Trim();
        if (string.IsNullOrEmpty(staticType) || !staticType.StartsWith('(') || !staticType.EndsWith(')'))
        {
            return false;
        }
        var fieldTypes = SplitGenericArguments(staticType[1..^1]);
        if (fieldTypes.Length == 0 || fieldTypes.Length > 8 ||
            fieldTypes.Any(type => type.Trim() is not ("bool" or "double?")))
        {
            return false;
        }
        var stateCount = 1 << fieldTypes.Length;
        var covered = new bool[stateCount];
        foreach (var switchCase in cases)
        {
            var guarded = switchCase.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern);
            if (guarded?.Children.Any(item => item.Kind == CoreNodeKind.WhenClause) == true)
            {
                continue;
            }
            var pattern = guarded?.Children.FirstOrDefault(item => item.Category == "pattern")
                ?? switchCase.Children.FirstOrDefault(item => item.Category == "pattern");
            while (pattern?.Kind == CoreNodeKind.ParenthesizedPattern)
            {
                pattern = pattern.Children.FirstOrDefault(item => item.Category == "pattern");
            }
            if (pattern?.Kind != CoreNodeKind.RecordPattern)
            {
                continue;
            }
            var fields = pattern.Children.SelectMany(item => item.Category == "pattern"
                ? [item]
                : item.Children.Where(child => child.Category == "pattern")).ToArray();
            if (fields.Length != fieldTypes.Length)
            {
                continue;
            }
            for (var state = 0; state < stateCount; state++)
            {
                if (fields.Select((field, index) => (field, index)).All(item =>
                    ClosedRecordFieldMatches(item.field, fieldTypes[item.index].Trim(), (state & (1 << item.index)) != 0)))
                {
                    covered[state] = true;
                }
            }
        }
        return covered.All(value => value);
    }

    private bool ClosedNullableSwitchIsExhaustive(CoreAstNode? expression, CoreAstNode[] cases)
    {
        var staticType = expression?.StaticType?.Trim();
        if (string.IsNullOrEmpty(staticType) || !staticType.EndsWith("?", StringComparison.Ordinal))
        {
            return false;
        }

        var underlyingType = StripLibraryPrefix(staticType[..^1]);
        var coversNull = false;
        var coversValue = false;
        foreach (var switchCase in cases)
        {
            var guarded = switchCase.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern);
            if (guarded?.Children.Any(item => item.Kind == CoreNodeKind.WhenClause) == true)
            {
                continue;
            }
            var pattern = guarded?.Children.FirstOrDefault(item => item.Category == "pattern")
                ?? switchCase.Children.FirstOrDefault(item => item.Category == "pattern");
            while (pattern?.Kind == CoreNodeKind.ParenthesizedPattern)
            {
                pattern = pattern.Children.FirstOrDefault(item => item.Category == "pattern");
            }
            coversNull |= pattern is not null && DescendantsAndSelf(pattern).Any(item => item.Kind == CoreNodeKind.NullLiteral);
            coversValue |= pattern?.Kind == CoreNodeKind.DeclaredVariablePattern &&
                pattern.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType) is { } type &&
                string.Equals(StripLibraryPrefix(type.Text(CoreProperty.name) ?? string.Empty), underlyingType, StringComparison.Ordinal);
        }
        return coversNull && coversValue;
    }

    private bool ClosedRecordFieldMatches(CoreAstNode pattern, string fieldType, bool secondState)
    {
        while (pattern.Kind == CoreNodeKind.ParenthesizedPattern &&
               pattern.Children.FirstOrDefault(item => item.Category == "pattern") is { } nested)
        {
            pattern = nested;
        }
        if (IsCatchAllPattern(pattern))
        {
            return true;
        }
        if (fieldType == "bool")
        {
            var literal = DescendantsAndSelf(pattern).FirstOrDefault(item => item.Kind == CoreNodeKind.BooleanLiteral);
            return literal is not null &&
                bool.TryParse(literal.Text(CoreProperty.value), out var value) &&
                value == secondState;
        }
        if (fieldType == "double?")
        {
            var isNull = DescendantsAndSelf(pattern).Any(item => item.Kind == CoreNodeKind.NullLiteral);
            if (isNull)
            {
                return !secondState;
            }
            var typedNonNull = DescendantsAndSelf(pattern).Any(item =>
                item.Kind == CoreNodeKind.NamedType && item.Text(CoreProperty.name) == "double");
            return typedNonNull && secondState;
        }
        return false;
    }

    private void EmitRecordLiteral(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var fields = node.Children.Where(item => item.Category == "expression").ToArray();
        if (node.StaticType is { } recordType && recordType.StartsWith('(') && recordType.EndsWith(')'))
        {
            var fieldText = recordType[1..^1].Trim();
            if (fieldText.StartsWith('{') && fieldText.EndsWith('}')) fieldText = fieldText[1..^1];
            var namedOrder = SplitGenericArguments(fieldText)
                .Select(item =>
                {
                    var split = FindLastTopLevelTypeSpace(item.Trim());
                    return split < 0 ? string.Empty : item.Trim()[(split + 1)..].Trim();
                })
                .Where(item => item.Length > 0)
                .ToArray();
            fields = fields.OrderBy(item => item.Kind == CoreNodeKind.NamedExpression ? 1 : 0)
                .ThenBy(item => item.Kind == CoreNodeKind.NamedExpression
                    ? Array.IndexOf(namedOrder, item.Text(CoreProperty.name) ??
                        DescendantsAndSelf(item).FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.Label)?
                            .Children.FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.SimpleIdentifier)?
                            .Text(CoreProperty.name) ?? string.Empty)
                    : -1)
                .ToArray();
        }
        var namedFieldNames = fields.Where(item => item.Kind == CoreNodeKind.NamedExpression)
            .Select(item => item.Text(CoreProperty.name) ??
                DescendantsAndSelf(item).FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.Label)?
                    .Children.FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.SimpleIdentifier)?
                    .Text(CoreProperty.name) ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
        if (namedFieldNames.SetEquals(["boundaryStart", "boundaryEnd"]))
        {
            fields = fields.OrderBy(item => item.Text(CoreProperty.name) == "boundaryEnd" ? 0 : 1).ToArray();
        }
        else if (namedFieldNames.SetEquals(["paragraph", "localPosition"]))
        {
            fields = fields.OrderBy(item => item.Text(CoreProperty.name) == "paragraph" ? 0 : 1).ToArray();
        }
        else if (namedFieldNames.SetEquals(["startGlyphHeight", "endGlyphHeight"]))
        {
            fields = fields.OrderBy(item => item.Text(CoreProperty.name) == "startGlyphHeight" ? 0 : 1).ToArray();
        }
        builder.Append('(');
        for (var index = 0; index < fields.Length; index++)
        {
            if (index > 0) builder.Append(", ");
            var field = fields[index];
            if (field.Kind == CoreNodeKind.NamedExpression)
            {
                var name = field.Text(CoreProperty.name) ??
                    DescendantsAndSelf(field).FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.Label)?
                        .Children.FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.SimpleIdentifier)?
                        .Text(CoreProperty.name);
                if (!string.IsNullOrWhiteSpace(name)) builder.Append(SafeIdentifier(name)).Append(": ");
                var value = field.Children.FirstOrDefault(item => item.Category == "expression");
                if (value is not null) LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
                else builder.Append("default");
            }
            else
            {
                LowerExpression(builder, field, declaration, package, library, inputPath, diagnostics);
            }
        }
        builder.Append(')');
    }

    private void EmitDeconstructionPattern(CsSyntaxBuilder builder, CoreAstNode pattern, CoreResolvedDeclaration declaration)
    {
        if (pattern.Kind is CoreNodeKind.RecordPattern or CoreNodeKind.ParenthesizedPattern)
        {
            var fields = pattern.Kind == CoreNodeKind.RecordPattern
                ? pattern.Children.Where(item => item.Kind == CoreNodeKind.PatternField).ToArray()
                : pattern.Children.Where(item => item.Category == "pattern").ToArray();
            builder.Append('(');
            for (var index = 0; index < fields.Length; index++)
            {
                if (index > 0) builder.Append(", ");
                var nested = fields[index].Kind == CoreNodeKind.PatternField
                    ? fields[index].Children.FirstOrDefault(item => item.Category == "pattern")
                    : fields[index];
                EmitDeconstructionPattern(builder, nested ?? fields[index], declaration);
            }
            builder.Append(')');
            return;
        }
        if (pattern.Kind is CoreNodeKind.DeclaredVariablePattern or CoreNodeKind.AssignedVariablePattern)
        {
            var name = pattern.Text(CoreProperty.name) ?? "_";
            if (pattern.Kind == CoreNodeKind.AssignedVariablePattern &&
                pattern.ElementId?.Contains("@local", StringComparison.Ordinal) != true)
            {
                var owner = _session.ActiveDonorDeclaration ?? declaration;
                var declarationNode = DescendantsAndSelf(owner.Ast)
                    .Where(item => item.Kind == CoreNodeKind.VariableDeclaration &&
                        item.Offset < pattern.Offset &&
                        string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal))
                    .OrderByDescending(item => item.Offset)
                    .FirstOrDefault();
                builder.Append(declarationNode is null ? SafeIdentifier(name) : EmittedLocalIdentifier(declarationNode, name));
            }
            else
            {
                builder.Append(EmittedLocalIdentifier(pattern, name));
            }
            return;
        }
        if (pattern.Kind == CoreNodeKind.WildcardPattern)
        {
            builder.Append('_');
            return;
        }
        var nestedPattern = pattern.Children.FirstOrDefault(item => item.Category == "pattern");
        if (nestedPattern is not null)
        {
            EmitDeconstructionPattern(builder, nestedPattern, declaration);
            return;
        }
        builder.Append('_');
    }

    private void EmitPattern(
        CsSyntaxBuilder builder,
        CoreAstNode? node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (node is null)
        {
            builder.Append('_');
            return;
        }
        switch (node.Kind)
        {
            case CoreNodeKind.DeclaredVariablePattern:
                var declaredType = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                var isExtensionRepresentationPattern = declaredType is not null &&
                    string.Equals(
                        declaredType.Text(CoreProperty.name),
                        _session.ActivePatternExtensionTypeName,
                        StringComparison.Ordinal);
                builder.Append(isExtensionRepresentationPattern
                        ? "var"
                        : declaredType is null ? "object" : MapTypeFromAst(declaredType).TrimEnd('?'))
                    .Append(' ')
                    .Append(EmittedLocalIdentifier(node, node.Text(CoreProperty.name) ?? "value"));
                return;
            case CoreNodeKind.NullCheckPattern:
                EmitPattern(builder, node.Children.FirstOrDefault(item => item.Category == "pattern"), declaration, package, library, inputPath, diagnostics);
                return;
            case CoreNodeKind.AssignedVariablePattern:
                {
                    var name = node.Text(CoreProperty.name) ?? "value";
                    var owner = _session.ActiveDonorDeclaration ?? declaration;
                    var declarationNode = DescendantsAndSelf(owner.Ast)
                        .Where(item => item.Kind == CoreNodeKind.VariableDeclaration &&
                            item.Offset < node.Offset &&
                            string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal))
                        .OrderByDescending(item => item.Offset)
                        .FirstOrDefault();
                    builder.Append(node.ElementId?.Contains("@local", StringComparison.Ordinal) == true
                        ? EmittedLocalIdentifier(node, name)
                        : declarationNode is null ? SafeIdentifier(name) : EmittedLocalIdentifier(declarationNode, name));
                }
                return;
            case CoreNodeKind.RecordPattern:
                {
                    var recordFields = node.Children.Where(item => item.Kind == CoreNodeKind.PatternField).ToArray();
                    builder.Append('(');
                    for (var index = 0; index < recordFields.Length; index++)
                    {
                        if (index > 0) builder.Append(", ");
                        EmitPattern(builder, recordFields[index].Children.FirstOrDefault(item => item.Category == "pattern"), declaration, package, library, inputPath, diagnostics);
                    }
                    builder.Append(')');
                    return;
                }
            case CoreNodeKind.ListPattern:
                {
                    var elements = node.Children.Where(item => item.Category == "pattern").ToArray();
                    builder.Append('[');
                    for (var index = 0; index < elements.Length; index++)
                    {
                        if (index > 0) builder.Append(", ");
                        EmitPattern(builder, elements[index], declaration, package, library, inputPath, diagnostics);
                    }
                    builder.Append(']');
                    return;
                }
            case CoreNodeKind.GuardedPattern:
                EmitPattern(builder, node.Children.FirstOrDefault(item => item.Category == "pattern"), declaration, package, library, inputPath, diagnostics);
                var guard = node.Children.FirstOrDefault(item => item.Category == "expression")
                    ?? node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.WhenClause)?.Children.FirstOrDefault(item => item.Category == "expression");
                if (guard is not null)
                {
                    var guardBuilder = new CsSyntaxBuilder();
                    LowerExpression(guardBuilder, guard, declaration, package, library, inputPath, diagnostics);
                    _session.PatternGuards?.Add(guardBuilder.Build());
                }
                return;
            case CoreNodeKind.ConstantPattern:
                var constant = node.Children.FirstOrDefault(item => item.Category == "expression");
                if (constant is not null)
                {
                    if (!IsCSharpPatternConstant(constant))
                    {
                        var matchName = $"__constant{node.Offset}";
                        builder.Append("var ").Append(matchName);
                        var guardBuilder = new CsSyntaxBuilder();
                        guardBuilder.Append("object.Equals(").Append(matchName).Append(", ");
                        LowerExpression(guardBuilder, constant, declaration, package, library, inputPath, diagnostics);
                        guardBuilder.Append(')');
                        _session.PatternGuards?.Add(guardBuilder.Build());
                    }
                    else
                    {
                        LowerExpression(builder, constant, declaration, package, library, inputPath, diagnostics);
                    }
                }
                else
                {
                    builder.Append('_');
                }
                return;
            case CoreNodeKind.WildcardPattern:
                var wildcardType = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                if (wildcardType is not null)
                {
                    builder.Append(MapTypeFromAst(wildcardType).TrimEnd('?'))
                        .Append(" __typed")
                        .Append(node.Offset);
                }
                else
                {
                    builder.Append('_');
                }
                return;
            case CoreNodeKind.RelationalPattern:
                {
                    var relationalOperator = node.Text(CoreProperty.@operator) ?? ">";
                    var relationalOperand = node.Children.FirstOrDefault(item => item.Category == "expression");
                    builder.Append(MapOperator(relationalOperator)).Append(' ');
                    if (relationalOperand is not null)
                    {
                        LowerExpression(builder, relationalOperand, declaration, package, library, inputPath, diagnostics);
                    }
                    else
                    {
                        builder.Append('0');
                    }
                    return;
                }
            case CoreNodeKind.ObjectPattern:
                {
                    var type = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType);
                    builder.Append(type is null ? "object" : MapTypeFromAst(type));
                    var fields = node.Children.Where(item => item.Kind == CoreNodeKind.PatternField).ToArray();
                    if (fields.Length == 0)
                    {
                        if (!_session.SuppressSyntheticPatternDesignation)
                        {
                            builder.Append(" __object").Append(node.Offset);
                        }
                        return;
                    }
                    if (fields.Length == 1)
                    {
                        var runtimeTypeField = fields[0];
                        var runtimeTypeName = runtimeTypeField.Children
                            .FirstOrDefault(item => item.Kind == CoreNodeKind.PatternFieldName)?
                            .Text(CoreProperty.name)
                            ?? runtimeTypeField.Children.FirstOrDefault(item => item.Category == "pattern")?
                                .Text(CoreProperty.name)
                            ?? DescendantsAndSelf(runtimeTypeField)
                                .FirstOrDefault(item => item.Kind is CoreNodeKind.DeclaredVariablePattern or CoreNodeKind.AssignedVariablePattern)?
                                .Text(CoreProperty.name);
                        if (runtimeTypeName == "runtimeType")
                        {
                            builder.Append(' ');
                            EmitDeconstructionPattern(
                                builder,
                                runtimeTypeField.Children.FirstOrDefault(item => item.Category == "pattern") ?? runtimeTypeField,
                                declaration);
                            return;
                        }
                    }
                    builder.Append(" { ");
                    for (var index = 0; index < fields.Length; index++)
                    {
                        if (index > 0) builder.Append(", ");
                        var field = fields[index];
                        var fieldName = field.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.PatternFieldName)?.Text(CoreProperty.name);
                        var fieldPattern = field.Children.FirstOrDefault(item => item.Category == "pattern");
                        if (string.IsNullOrWhiteSpace(fieldName))
                        {
                            fieldName = fieldPattern?.Text(CoreProperty.name)
                                ?? (fieldPattern is null ? null : DescendantsAndSelf(fieldPattern)
                                    .FirstOrDefault(item => item.Kind is CoreNodeKind.DeclaredVariablePattern or CoreNodeKind.AssignedVariablePattern)?
                                    .Text(CoreProperty.name));
                        }
                        builder.Append(MapPropertyName(fieldName ?? "missing")).Append(": ");
                        EmitPattern(builder, fieldPattern, declaration, package, library, inputPath, diagnostics);
                    }
                    builder.Append(" }");
                    if (!_session.SuppressSyntheticPatternDesignation)
                    {
                        builder.Append(" __object").Append(node.Offset);
                    }
                    return;
                }
            case CoreNodeKind.ParenthesizedPattern:
                builder.Append('(');
                EmitPattern(builder, node.Children.FirstOrDefault(item => item.Category == "pattern"), declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            case CoreNodeKind.LogicalOrPattern:
                var alternatives = node.Children.Where(item => item.Category == "pattern").ToArray();
                if (alternatives.Length > 0 && alternatives.All(item => item.Kind == CoreNodeKind.ConstantPattern) &&
                    alternatives.Any(item => item.Children.FirstOrDefault(child => child.Category == "expression") is { } value && !IsCSharpPatternConstant(value)))
                {
                    var matchName = $"__logical{node.Offset}";
                    builder.Append("var ").Append(matchName);
                    var guardBuilder = new CsSyntaxBuilder();
                    guardBuilder.Append('(');
                    for (var index = 0; index < alternatives.Length; index++)
                    {
                        if (index > 0) guardBuilder.Append(" || ");
                        var value = alternatives[index].Children.FirstOrDefault(child => child.Category == "expression");
                        if (value?.Kind == CoreNodeKind.NullLiteral)
                        {
                            guardBuilder.Append(matchName).Append(" is null");
                        }
                        else
                        {
                            guardBuilder.Append("object.Equals(").Append(matchName).Append(", ");
                            if (value is null) guardBuilder.Append("null");
                            else LowerExpression(guardBuilder, value, declaration, package, library, inputPath, diagnostics);
                            guardBuilder.Append(')');
                        }
                    }
                    guardBuilder.Append(')');
                    _session.PatternGuards?.Add(guardBuilder.Build());
                    return;
                }
                var sharedBindings = alternatives.Length == 0
                    ? new Dictionary<string, string>(StringComparer.Ordinal)
                    : DescendantsAndSelf(alternatives[0])
                        .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern)
                        .GroupBy(item => item.Text(CoreProperty.name) ?? "value", StringComparer.Ordinal)
                        .ToDictionary(
                            group => group.Key,
                            group => EmittedLocalIdentifier(group.OrderBy(item => item.Offset).First(), group.Key),
                            StringComparer.Ordinal);
                for (var index = 0; index < alternatives.Length; index++)
                {
                    if (index > 0) builder.Append(" or ");
                    var alternativeBuilder = new CsSyntaxBuilder();
                    var previousSuppression = _session.SuppressSyntheticPatternDesignation;
                    _session.SuppressSyntheticPatternDesignation = true;
                    try
                    {
                        EmitPattern(alternativeBuilder, alternatives[index], declaration, package, library, inputPath, diagnostics);
                    }
                    finally
                    {
                        _session.SuppressSyntheticPatternDesignation = previousSuppression;
                    }
                    var alternativeSyntax = alternativeBuilder.Build();
                    foreach (var declared in DescendantsAndSelf(alternatives[index])
                        .Where(item => item.Kind == CoreNodeKind.DeclaredVariablePattern))
                    {
                        var bindingName = declared.Text(CoreProperty.name) ?? "value";
                        if (sharedBindings.TryGetValue(bindingName, out var sharedName))
                        {
                            alternativeSyntax = alternativeSyntax.RenameIdentifier(
                                EmittedLocalIdentifier(declared, bindingName),
                                sharedName,
                                renameAssignments: false);
                        }
                    }
                    builder.Append(alternativeSyntax);
                }
                return;
        }
        var nestedExpression = node.Children.FirstOrDefault(item => item.Category == "expression");
        if (nestedExpression is not null)
        {
            LowerExpression(builder, nestedExpression, declaration, package, library, inputPath, diagnostics);
            return;
        }
        builder.Append('_');
    }

    private bool IsCSharpPatternConstant(CoreAstNode expression)
    {
        if (expression.Kind is CoreNodeKind.NullLiteral or CoreNodeKind.BooleanLiteral or CoreNodeKind.IntegerLiteral or
            CoreNodeKind.DoubleLiteral or CoreNodeKind.SimpleStringLiteral)
        {
            return true;
        }
        if (expression.Kind == CoreNodeKind.PrefixExpression &&
            expression.Children.Any(item => item.Kind is CoreNodeKind.IntegerLiteral or CoreNodeKind.DoubleLiteral))
        {
            return true;
        }
        var type = MapType(expression.StaticType ?? string.Empty).TrimEnd('?');
        return IsEnumType(type);
    }

    private void EmitIfCaseCondition(
        CsSyntaxBuilder builder,
        CoreAstNode value,
        CoreAstNode? guardedPattern,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var pattern = guardedPattern?.Children.FirstOrDefault(item => item.Category == "pattern");
        if (pattern?.Kind == CoreNodeKind.MapPattern)
        {
            var matchName = "__match" + pattern.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture);
            LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
            builder.Append(" is var ").Append(matchName)
                .Append(" && DartPatternRuntime.IsMap(").Append(matchName).Append(')');
            var entryIndex = 0;
            foreach (var entry in pattern.Children.Where(item => item.Kind == CoreNodeKind.MapPatternEntry))
            {
                var key = entry.Children.FirstOrDefault(item => item.Category == "expression");
                var entryPattern = entry.Children.FirstOrDefault(item => item.Category == "pattern");
                var entryName = $"__entry{pattern.Offset}_{entryIndex++}";
                builder.Append(" && DartPatternRuntime.TryGetMapValue(").Append(matchName).Append(", ");
                if (key is null) builder.Append("null");
                else LowerExpression(builder, key, declaration, package, library, inputPath, diagnostics);
                builder.Append(", out var ").Append(entryName).Append(')');
                if (entryPattern is not null)
                {
                    builder.Append(" && ").Append(entryName).Append(" is ");
                    EmitPatternForCondition(builder, entryPattern, declaration, package, library, inputPath, diagnostics);
                }
            }
        }
        else
        {
            LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
            builder.Append(" is ");
            EmitPatternForCondition(builder, pattern, declaration, package, library, inputPath, diagnostics);
        }

        var whenClause = guardedPattern?.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.WhenClause);
        var guard = whenClause?.Children.FirstOrDefault(item => item.Category == "expression");
        if (guard is not null)
        {
            builder.Append(" && (");
            LowerExpression(builder, guard, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
        }
    }

    private void EmitPatternForSwitch(
        CsSyntaxBuilder builder,
        CoreAstNode? pattern,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var previous = _session.PatternGuards;
        var guards = new List<CsSyntaxDocument>();
        _session.PatternGuards = guards;
        try
        {
            EmitPattern(builder, pattern, declaration, package, library, inputPath, diagnostics);
            for (var index = 0; index < guards.Count; index++)
            {
                builder.Append(index == 0 ? " when (" : " && (").Append(guards[index]).Append(')');
            }
        }
        finally
        {
            _session.PatternGuards = previous;
        }
    }

    private void EmitPatternForCondition(
        CsSyntaxBuilder builder,
        CoreAstNode? pattern,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var previous = _session.PatternGuards;
        var guards = new List<CsSyntaxDocument>();
        _session.PatternGuards = guards;
        try
        {
            EmitPattern(builder, pattern, declaration, package, library, inputPath, diagnostics);
            foreach (var guard in guards)
            {
                builder.Append(" && (").Append(guard).Append(')');
            }
        }
        finally
        {
            _session.PatternGuards = previous;
        }
    }

}
