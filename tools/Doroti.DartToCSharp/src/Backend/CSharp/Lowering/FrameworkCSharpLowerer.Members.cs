using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitField(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember field,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        bool forcePublic = false)
    {
        // Dart privacy is library-wide. A Dart library is emitted as several C#
        // types, so private C# visibility would incorrectly block peer types.
        var visibility = forcePublic || !IsDartPrivate(field) || IsRequiredByMixinConstraint(field) || field.Name == "_layerHandle" ||
            declaration.Name == "OneSequenceGestureRecognizer" && field.Name is "_entries" or "_trackedPointers" ||
            field.IsStatic && field.Name == "_instance" && WillEmitAsInterface(declaration)
            ? "public"
            : "internal";
        var contractField = field.IsStatic ? null : FindBaseContractMember(declaration, field);
        if (contractField is null && declaration.Name == "DefaultWidgetsLocalizations")
        {
            contractField = FindDirectSuperclassMember(declaration, field);
        }
        var initializer = field.Ast.Child(CoreChildRole.initializerOffset);
        var ownType = MapType(field.Element.Type ?? "object");
        // Field elements expose display names such as `TextStyle` without the
        // declaring library. When an initializer calls a resolved method, its
        // return-type AST retains that identity (for example ui.TextStyle).
        // Prefer that qualified type when it is the same Dart type name.
        if (initializer is not null && FindGlobalMember(initializer.ElementId) is { } initializerMember)
        {
            var initializerReturnTypeNode = initializerMember.Ast.Children
                .FirstOrDefault(item => item.Category == "type");
            if (initializerReturnTypeNode is not null)
            {
                var resolvedInitializerType = MapTypeFromAst(initializerReturnTypeNode);
                var declaredTypeName = (field.Element.Type ?? string.Empty).TrimEnd('?');
                var sameQualifiedType = string.Equals(
                    StripLibraryPrefix(resolvedInitializerType).TrimEnd('?'),
                    StripLibraryPrefix(ownType).TrimEnd('?'),
                    StringComparison.Ordinal);
                var sameUnqualifiedType = declaredTypeName.Length > 0 &&
                    !declaredTypeName.Contains('<', StringComparison.Ordinal) &&
                    resolvedInitializerType.TrimEnd('?').EndsWith('.' + declaredTypeName, StringComparison.Ordinal);
                if (sameQualifiedType || sameUnqualifiedType)
                {
                    ownType = resolvedInitializerType;
                }
            }
        }
        var contractType = contractField is null
            ? ownType
            : MapType(contractField.Element.ReturnType ?? contractField.Element.Type ?? field.Element.Type ?? "object");
        var type = Regex.IsMatch(contractType, @"\b[A-Z]\b", RegexOptions.CultureInvariant) &&
            !Regex.IsMatch(ownType, @"\b[A-Z]\b", RegexOptions.CultureInvariant)
                ? ownType
                : contractType;
        if (field.Name == "renderObject")
        {
            type = contractField is null
                ? ownType
                : "global::Doroti.Framework.Rendering.RenderObject" +
                    (ownType.EndsWith("?", StringComparison.Ordinal) ? "?" : string.Empty);
        }
        if (field.Name == "compareOrder")
        {
            type = "global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>";
        }
        if (declaration.Name == "TreeSliver" && field.Name == "treeRowExtentBuilder")
        {
            type = "global::System.Func<TreeSliverNode<T>, global::Doroti.Framework.Rendering.SliverLayoutDimensions, double?>";
        }
        if (declaration.Name == "_TreeSliverState" && field.Name == "_activeAnimations")
        {
            type = "DartMap<global::Doroti.Framework.Foundation.UniqueKey, global::Doroti.Framework.Rendering.TreeSliverNodesAnimation>";
        }
        if (declaration.Name == "ScrollAwareImageProvider" && field.Name == "context")
        {
            type = "dynamic";
        }
        if (field.Name == "value" && declaration.Name is "SelectionContainerDelegate" or "_SelectionContainerState")
        {
            type = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        if (declaration.Name == "PrioritizedAction" && field.Name == "_selectedAction")
        {
            type = "Action<Intent>";
        }
        if (DisplacedStructuralSuperclass(declaration) is not null &&
            field.Name is "original" or "transform")
        {
            type = ownType;
        }
        // CLR property overrides are invariant even when Dart narrows a getter.
        // Keep the inherited contract type; the getter body may still return a
        // more-specific value through normal reference conversion.
        var name = SafeIdentifier(field.Name);
        var contractOwner = contractField is null ? null : FindDeclaringDeclaration(contractField);
        var contractOwnerName = contractOwner?.Name;
        var canOverrideContract = contractOwner is not null && !WillEmitAsInterface(contractOwner) &&
            contractOwnerName is not ("ValueNotifier" or "DiagnosticsProperty" or "DiagnosticsSerializationDelegate");
        if (declaration.Element.Supertype is { } fieldSupertype &&
            StripLibraryPrefix(fieldSupertype).Split('<')[0] == "DiagnosticsSerializationDelegate")
        {
            canOverrideContract = false;
        }
        if (declaration.Name is "_OverridableAction" or "_OverridableContextAction" &&
            field.Name is "_defaultAction" or "_lookupContext")
        {
            canOverrideContract = false;
            if (declaration.Name == "_OverridableContextAction" && field.Name == "_defaultAction")
            {
                type = ownType;
            }
        }
        if (field.IsStatic)
        {
            if (field.IsConst && initializer is { Kind: CoreNodeKind.SimpleIdentifier } &&
                FindGlobalMember(initializer.ElementId) is { IsStatic: true })
            {
                // Dart const aliases are canonical compile-time values and do
                // not observe textual static initialization order. A CLR field
                // alias can read a later field as null, so retain the dependency
                // as an expression-bodied static property.
                builder.Append($"    {visibility} static {type} {name} => ");
                EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
                return;
            }
            var modifier = field.IsConst && IsCSharpConstant(initializer) ? "const" : "static";
            builder.Append($"    {visibility} {modifier} {type} {name}");
            if (initializer is not null)
            {
                builder.Append(" = ");
                EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
            }
            else if (modifier == "const")
            {
                builder.Append(" = default");
            }
            else
            {
                builder.Append(" = default");
            }
            builder.AppendLine(";");
            return;
        }

        if (!field.IsFinal && field.IsLate && initializer is not null)
        {
            var propertyModifier = canOverrideContract ? "override " : "virtual ";
            var storageName = "__late_" + name.TrimStart('@');
            builder.AppendLine($"    private bool {storageName}_initialized;");
            builder.AppendLine($"    private {type} {storageName} = default!;");
            builder.AppendLine($"    {visibility} {propertyModifier}{type} {name}");
            builder.AppendLine("    {");
            builder.AppendLine("        get");
            builder.AppendLine("        {");
            builder.AppendLine($"            if (!{storageName}_initialized)");
            builder.AppendLine("            {");
            builder.Append($"                {storageName} = ");
            EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine(";");
            builder.AppendLine($"                {storageName}_initialized = true;");
            builder.AppendLine("            }");
            builder.AppendLine($"            return {storageName};");
            builder.AppendLine("        }");
            builder.AppendLine($"        set {{ {storageName} = value; {storageName}_initialized = true; }}");
            builder.AppendLine("    }");
            return;
        }

        if (field.IsFinal)
        {
            var propertyModifier = canOverrideContract ? "override " : "virtual ";
            if (field.IsLate && initializer is not null)
            {
                var storageName = "__late_" + name.TrimStart('@');
                builder.AppendLine($"    private bool {storageName}_initialized;");
                builder.AppendLine($"    private {type} {storageName} = default!;");
                builder.AppendLine($"    {visibility} {propertyModifier}{type} {name}");
                builder.AppendLine("    {");
                builder.AppendLine("        get");
                builder.AppendLine("        {");
                builder.AppendLine($"            if (!{storageName}_initialized)");
                builder.AppendLine("            {");
                builder.Append($"                {storageName} = ");
                EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
                builder.AppendLine($"                {storageName}_initialized = true;");
                builder.AppendLine("            }");
                builder.AppendLine($"            return {storageName};");
                builder.AppendLine("        }");
                builder.AppendLine("    }");
                return;
            }
            if (propertyModifier == "override ")
            {
                var storageName = "__field_" + name.TrimStart('@');
                builder.Append($"    private {type} {storageName}");
                if (initializer is not null)
                {
                    builder.Append(" = ");
                    EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
                }
                else
                {
                    builder.Append(" = default!");
                }
                builder.AppendLine(";");
                builder.AppendLine($"    {visibility} override {type} {name} {{ get => {storageName}; }}");
                return;
            }
            var setter = propertyModifier == "override " ? string.Empty : forcePublic ? " set;" : " private set;";
            builder.Append($"    {visibility} {propertyModifier}{type} {name} {{ get;{setter} }}");
            if (initializer is not null)
            {
                builder.Append(" = ");
                EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
                builder.Append(';');
            }
            else if (!type.EndsWith("?", StringComparison.Ordinal))
            {
                builder.Append(" = default!;");
            }
            builder.AppendLine();
            return;
        }

        var mutablePropertyModifier = canOverrideContract ? "override " : "virtual ";
        builder.Append($"    {visibility} {mutablePropertyModifier}{type} {name} {{ get; set; }}");
        if (initializer is not null)
        {
            builder.Append(" = ");
            EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
        }
        else
        {
            builder.Append(type.EndsWith("?", StringComparison.Ordinal) ? " = default" : " = default!");
        }
        builder.AppendLine(";");
        if (declaration.Name == "_RenderCustomClip" && field.Name == "_clip")
        {
            builder.AppendLine("    // `T?` on an unconstrained C# generic is only a nullable annotation. When T is");
            builder.AppendLine("    // Rect or RRect, default(T) is an empty value rather than Dart's null sentinel.");
            builder.AppendLine("    // Track validity explicitly so the first paint computes the actual clip.");
            builder.AppendLine("    internal virtual bool _clipIsValid { get; set; }");
        }
    }

    private void EmitFieldInitializer(
        CsSyntaxBuilder builder,
        string expectedType,
        CoreAstNode initializer,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var actualType = MapType(initializer.StaticType ?? string.Empty);
        var expected = expectedType.TrimEnd('?');
        var actual = actualType.TrimEnd('?');
        var argumentList = initializer.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList);
        if (initializer.Kind == CoreNodeKind.ListLiteral &&
            initializer.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList)?
                .Children.FirstOrDefault(item => item.Category == "type") is { } explicitListArgument)
        {
            actualType = $"List<{MapTypeFromAst(explicitListArgument)}>";
            actual = actualType;
        }
        if (initializer.Kind == CoreNodeKind.InstanceCreationExpression &&
            expected.StartsWith("GlobalKey<", StringComparison.Ordinal) &&
            (argumentList ?? DescendantsAndSelf(initializer).FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList))?
                .Children.Any(item => item.Category == "expression") != true)
        {
            builder.Append(expected).Append(".Create()");
            return;
        }
        var emptySameGenericConstruction = initializer.Kind == CoreNodeKind.InstanceCreationExpression &&
            (argumentList ?? DescendantsAndSelf(initializer).FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList))?
                .Children.Any(item => item.Category == "expression") != true &&
            TryGetGenericTypeArguments(expected, out _) &&
            TryGetGenericTypeArguments(actual, out _) &&
            string.Equals(expected[..expected.IndexOf('<')], actual[..actual.IndexOf('<')], StringComparison.Ordinal) &&
            expected[..expected.IndexOf('<')] is not ("IEnumerable" or "IReadOnlyList" or "IList" or "IDictionary" or "GlobalKey") &&
            (FindGlobalDeclaration(expected[..expected.IndexOf('<')]) is not { } expectedDeclaration ||
             !expectedDeclaration.Element.IsAbstract && !WillEmitAsInterface(expectedDeclaration));
        if (emptySameGenericConstruction)
        {
            builder.Append("new ").Append(expected).Append("()");
            return;
        }

        var isCollectionConversion =
            expected.StartsWith("List<", StringComparison.Ordinal) &&
            (actual.StartsWith("List<", StringComparison.Ordinal) ||
             actual.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
             actual.StartsWith("IReadOnlyList<", StringComparison.Ordinal)) ||
            expected.StartsWith("DartMap<", StringComparison.Ordinal) &&
            actual.StartsWith("DartMap<", StringComparison.Ordinal);
        var needsCheckedCast = !isCollectionConversion &&
            expected is not ("object" or "dynamic" or "void") &&
            actual is not ("void") &&
            !IsValueType(expected) &&
            !string.Equals(expected, actual, StringComparison.Ordinal);
        if (needsCheckedCast) builder.Append("((").Append(expectedType).Append(")(object?)");
        LowerExpression(builder, initializer, declaration, package, library, inputPath, diagnostics);
        if (needsCheckedCast) builder.Append(')');
        else AppendFieldInitializerCollectionConversion(builder, expectedType, initializer);
    }

    private void AppendFieldInitializerCollectionConversion(
        CsSyntaxBuilder builder,
        string expectedType,
        CoreAstNode initializer)
    {
        var actualType = MapType(initializer.StaticType ?? string.Empty).TrimEnd('?');
        if (initializer.Kind == CoreNodeKind.ListLiteral &&
            initializer.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList)?
                .Children.FirstOrDefault(item => item.Category == "type") is { } explicitListArgument)
        {
            actualType = $"List<{MapTypeFromAst(explicitListArgument)}>";
        }
        var expected = expectedType.TrimEnd('?');
        if (expected.StartsWith("List<", StringComparison.Ordinal) &&
            (actualType.StartsWith("List<", StringComparison.Ordinal) ||
             actualType.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
             actualType.StartsWith("IReadOnlyList<", StringComparison.Ordinal)) &&
            TryGetGenericTypeArguments(expected, out var expectedArguments) &&
            TryGetGenericTypeArguments(actualType, out var actualArguments) &&
            expectedArguments.Length == 1 && actualArguments.Length == 1)
        {
            if (!string.Equals(expectedArguments[0], actualArguments[0], StringComparison.Ordinal))
            {
                builder.Append(".Cast<").Append(expectedArguments[0]).Append(">()");
            }
            if (!actualType.StartsWith("List<", StringComparison.Ordinal) ||
                !string.Equals(expectedArguments[0], actualArguments[0], StringComparison.Ordinal))
            {
                builder.Append(".ToList()");
            }
            return;
        }
        if (expected.StartsWith("DartMap<", StringComparison.Ordinal) &&
            actualType.StartsWith("DartMap<", StringComparison.Ordinal) &&
            !string.Equals(expected, actualType, StringComparison.Ordinal))
        {
            builder.Append(".cast<").Append(DartMapTypeArguments(expected)).Append(">()");
        }
    }

    private void LowerExpressionWithExpectedType(
        CsSyntaxBuilder builder,
        CoreAstNode expression,
        string expectedType,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var expected = expectedType.TrimEnd('?');
        var actualType = MapType(expression.StaticType ?? string.Empty);
        var actual = actualType.TrimEnd('?');
        if (expression.Kind == CoreNodeKind.ThrowExpression)
        {
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            return;
        }
        if (expected == "void")
        {
            if (expression.Kind == CoreNodeKind.ConditionalExpression &&
                expression.Child(CoreChildRole.conditionOffset) is { } condition &&
                expression.Child(CoreChildRole.thenOffset) is { } thenExpression &&
                expression.Child(CoreChildRole.elseOffset) is { } elseExpression)
            {
                builder.Append("((Action)(() => { if (");
                LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                builder.Append(") { ");
                LowerExpression(builder, thenExpression, declaration, package, library, inputPath, diagnostics);
                builder.Append("; } else { ");
                LowerExpression(builder, elseExpression, declaration, package, library, inputPath, diagnostics);
                builder.Append("; } }))()");
                return;
            }
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            return;
        }
        if (actual == "void")
        {
            builder.Append("DartRuntimePrimitives.CaptureVoid(() => ");
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (expected.StartsWith("List<", StringComparison.Ordinal) &&
            (actual.StartsWith("List<", StringComparison.Ordinal) ||
             actual.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
             actual.StartsWith("IReadOnlyList<", StringComparison.Ordinal)) &&
            !ExpressionProducesFuture(expression) &&
            TryGetGenericTypeArguments(expected, out var expectedArguments) &&
            expectedArguments.Length == 1)
        {
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            if (!string.Equals(expected, actual, StringComparison.Ordinal))
            {
                builder.Append(".Cast<").Append(expectedArguments[0]).Append(">().ToList()");
            }
            return;
        }
        if (IsValueType(expected) && !expectedType.EndsWith("?", StringComparison.Ordinal) &&
            actualType == expected + "?")
        {
            builder.Append("DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (expected is not ("object" or "dynamic") &&
            !expected.StartsWith("___", StringComparison.Ordinal) &&
            actual.Length > 0 && actual != "void" &&
            !string.Equals(expected, actual, StringComparison.Ordinal))
        {
            if (expression.Kind == CoreNodeKind.FunctionExpression)
            {
                builder.Append("((").Append(expectedType).Append(')');
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<").Append(expectedType.TrimEnd('?')).Append(">(");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            return;
        }
        if (expected is not ("object" or "dynamic") &&
            !expected.StartsWith("___", StringComparison.Ordinal) &&
            expression.Kind is CoreNodeKind.BinaryExpression or CoreNodeKind.PostfixExpression &&
            expression.Kind != CoreNodeKind.FunctionExpression)
        {
            builder.Append("DartRuntimePrimitives.ConvertValue<").Append(expectedType.TrimEnd('?')).Append(">(");
            LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitBlockBodyWithReturnContract(
        CsSyntaxBuilder builder,
        CoreAstNode block,
        CoreResolvedDeclaration declaration,
        string expectedReturnType,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        int indent)
    {
        var previousReturnType = _session.ActiveFunctionReturnType;
        _session.ActiveFunctionReturnType = expectedReturnType;
        try
        {
            EmitBlockBody(builder, block, declaration, package, library, inputPath, diagnostics, indent);
        }
        finally
        {
            _session.ActiveFunctionReturnType = previousReturnType;
        }
    }

    private void EmitVirtualContractStub(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember member)
    {
        var returnType = MapType(member.Element.ReturnType ?? member.Element.Type ?? "void");
        if (declaration.Name == "SelectionContainerDelegate" && member.Name == "value")
        {
            returnType = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        var name = MapMethodDeclarationName(member);
        if (member.IsGetter)
        {
            builder.AppendLine($"    public virtual {returnType} {name} => throw new NotSupportedException();");
            return;
        }
        if (member.IsSetter)
        {
            builder.AppendLine($"    public virtual {returnType} {name} {{ set => throw new NotSupportedException(); }}");
            return;
        }
        builder.AppendLine($"    public virtual {returnType} {name}({string.Join(", ", MapParameters(member.Element.Parameters ?? []))}) => throw new NotSupportedException();");
    }

    private void EmitMixinConstraintMember(CsSyntaxBuilder builder, CoreResolvedDeclaration declaration, CoreResolvedMember member)
    {
        var returnType = MapType(member.Element.ReturnType ?? member.Element.Type ?? "void");
        var name = MapMethodDeclarationName(member);
        var isInterface = WillEmitAsInterface(declaration);
        var modifier = isInterface ? string.Empty : "public abstract ";
        if (member.Kind == "field")
        {
            builder.AppendLine($"    {modifier}{returnType} {name} {{ get; }}");
            return;
        }
        if (member.IsGetter)
        {
            var hasSetter = FindDeclaringDeclaration(member)?.Members.Any(candidate =>
                candidate.IsSetter && candidate.Name == member.Name) == true;
            builder.AppendLine($"    {modifier}{returnType} {name} {{ get;{(hasSetter ? " set;" : string.Empty)} }}");
            return;
        }
        if (member.IsSetter)
        {
            var setterType = MapType(member.Element.Parameters?.FirstOrDefault()?.Type ?? member.Element.Type ?? "object");
            builder.AppendLine($"    {modifier}{setterType} {name} {{ set; }}");
            return;
        }
        var parameters = CanonicalOverrideParameters(
            declaration,
            member,
            member.Element.Parameters ?? [],
            member);
        var typeParameters = member.Element.TypeParameters is { Length: > 0 }
            ? $"<{string.Join(", ", member.Element.TypeParameters.Select(item => SafeIdentifier(item.Name)))}>"
            : string.Empty;
        var typeParameterConstraints = FormatTypeParameterConstraints(
            member.Element.TypeParameters,
            parameters.Select(item => MapType(item.Type)).Append(returnType));
        builder.AppendLine($"    {modifier}{returnType} {name}{typeParameters}({string.Join(", ", MapParameters(parameters))}){typeParameterConstraints};");
    }

    private void EmitClassMethod(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        CoreResolvedMember[] members,
        bool isInterface,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var previousMethodTypeParameters = _session.ActiveMethodTypeParameters;
        var previousContractSubstitutions = _session.ActiveMemberContractSubstitutions;
        _session.ActiveMethodTypeParameters = (method.Element.TypeParameters ?? [])
            .Select(parameter => parameter.Name)
            .ToHashSet(StringComparer.Ordinal);
        _session.ActiveMemberContractSubstitutions =
            new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            EmitClassMethodCore(builder, declaration, method, members, isInterface, package, library, inputPath, diagnostics);
        }
        finally
        {
            _session.ActiveMethodTypeParameters = previousMethodTypeParameters;
            _session.ActiveMemberContractSubstitutions = previousContractSubstitutions;
        }
    }

    private void EmitClassMethodCore(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        CoreResolvedMember[] members,
        bool isInterface,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var sourceParameters = method.Element.Parameters ?? [];
        if (TryEmitRenderCustomClipCacheMethod(builder, declaration, method))
        {
            return;
        }
        if (declaration.Name == "LocalizationsDelegate" && method.Name == "shouldReload" &&
            declaration.Element.TypeParameters is { Length: 1 } localizationDelegateParameters)
        {
            builder.AppendLine($"    public abstract bool shouldReload(LocalizationsDelegate<{SafeIdentifier(localizationDelegateParameters[0].Name)}> old);");
            return;
        }
        if (declaration.Name == "WidgetInspectorService" && method.Name == "instance" && method.IsSetter)
        {
            return;
        }
        // Dart static methods are library/class scoped declarations, not virtual
        // override-family members. Treating a same-named base static as an
        // override rewrites the derived signature to the base signature (for
        // example LinearGradient.lerp(LinearGradient, ...) became
        // lerp(Gradient, ...)), which loses the analyzer's concrete types.
        var overriddenMember = method.IsStatic ? null : FindOverriddenBaseMember(declaration, method);
        if (_session.ActiveDonorDeclaration is { } donor && HasPromotedClassRepresentation(donor))
        {
            overriddenMember = null;
        }
        if (overriddenMember is not null &&
            FindDeclaringDeclaration(overriddenMember)?.Name is
                "ChangeNotifier" or "ValueNotifier" or "DiagnosticsProperty" or "DiagnosticsSerializationDelegate")
        {
            // The promoted host-neutral ChangeNotifier intentionally exposes a
            // non-virtual CLR surface. Dart subclasses may shadow these methods,
            // but cannot emit a CLR override against that reviewed owner.
            overriddenMember = null;
        }
        var contractMember = method.IsStatic ? null : overriddenMember ?? FindBaseContractMember(declaration, method);
        var contractSubstitutions = ContractTypeParameterSubstitutions(declaration, contractMember);
        _session.ActiveMemberContractSubstitutions = contractSubstitutions;
        var parameters = (method.IsStatic
            ? sourceParameters
            : CanonicalOverrideParameters(declaration, method, sourceParameters, contractMember))
            .Select(parameter => parameter with
            {
                Type = ApplyTypeParameterSubstitutions(parameter.Type, contractSubstitutions)
            })
            .ToArray();
        if (declaration.Name == "RestorationMixin" && method.Name == "didUpdateWidget" &&
            declaration.Element.TypeParameters is { Length: > 0 } restorationParameters && parameters.Length == 1)
        {
            parameters[0] = parameters[0] with { Type = restorationParameters[0].Name };
        }
        if (declaration.Name == "LocalizationsDelegate" && method.Name == "shouldReload" &&
            declaration.Element.TypeParameters is { Length: 1 } localizationParameters && parameters.Length == 1)
        {
            parameters[0] = parameters[0] with { Type = $"LocalizationsDelegate<{localizationParameters[0].Name}>" };
        }
        if (method.Name == "debugFillProperties" && sourceParameters.Length > 0 &&
            declaration.Name is not ("TextStyle" or "StrutStyle"))
        {
            parameters = [sourceParameters[0]];
        }
        if ((method.Name == "markNeedsLayout" || method.Name == "debugDescribeChildren" && sourceParameters.Length == 0) &&
            !(declaration.Name == "RenderTwoDimensionalViewport" && method.Name == "markNeedsLayout"))
        {
            parameters = [];
        }
        if (method.Name == "shouldReclip" && declaration.Element.Supertype is { } clipperBase &&
            StripLibraryPrefix(clipperBase).StartsWith("CustomClipper<", StringComparison.Ordinal) &&
            parameters.Length == 1)
        {
            parameters[0] = parameters[0] with { Type = clipperBase };
        }
        if (method.Name == "shouldReload" && declaration.Element.Supertype is { } localizationBase && parameters.Length == 1)
        {
            parameters[0] = parameters[0] with { Type = localizationBase };
        }
        if (method.Name == "updateShouldNotifyDependent" && declaration.Element.Supertype is { } inheritedModelBase &&
            TryGenericTypeApplication(inheritedModelBase, "InheritedModel", out var inheritedArguments) &&
            inheritedArguments.Length == 1 && parameters.Length == 2)
        {
            parameters[0] = parameters[0] with { Type = inheritedModelBase };
            parameters[1] = parameters[1] with { Type = $"Set<{inheritedArguments[0]}>" };
        }
        if (method.Name == "didUpdateValue" && declaration.Element.Supertype is { } restorableBase &&
            TryGenericTypeApplication(restorableBase, "RestorableValue", out var restorableArguments) &&
            restorableArguments.Length == 1 && parameters.Length == 1)
        {
            parameters[0] = parameters[0] with { Type = restorableArguments[0] };
        }
        var returnTypeNode = method.Ast.Children.FirstOrDefault(item => item.Category == "type");
        var returnType = method.Element.ReturnType == "dynamic"
            ? "dynamic"
            : returnTypeNode is null
            ? MapType(method.Element.ReturnType ?? "void")
            : MapTypeFromAst(returnTypeNode);
        if (method.Name == "createRenderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.Name == "createState")
        {
            // Dart permits covariant State<DerivedWidget> return types through
            // intermediate StatefulWidget subclasses. Closed CLR State<T>
            // types are invariant, so every createState override crosses the
            // framework boundary through the common IState contract.
            returnType = "IState";
        }
        // Contract canonicalization above may replace a Dart-covariant getter
        // with an intermediate mixin type. These CLR contracts are invariant,
        // so apply the reviewed G5-3 surface type last.
        if (method.Name == "renderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.Name == "compareOrder")
        {
            returnType = "global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>";
        }
        if (method.Name == "value" && declaration.Name is "SelectionContainerDelegate" or "_SelectionContainerState")
        {
            returnType = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        if (method.Name == "root" && declaration.Name is "_RawMenuAnchorState" or "_RawMenuAnchorGroupState" &&
            (declaration.Element.Mixins ?? []).FirstOrDefault(type =>
                StripLibraryPrefix(type).Split('<')[0] == "_RawMenuAnchorBaseMixin") is { } finalMenuMixin)
        {
            returnType = MapType(finalMenuMixin);
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "screenshot")
        {
            returnType = "Future<global::Doroti.Ui.Image?>";
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "_getSelectedWidgetLocation")
        {
            returnType = "global::Doroti.Runtime.CreationLocation?";
        }
        if (declaration.Name == "_UnspecifiedTextScaler" && method.Name is "scale" or "textScaleFactor")
        {
            returnType = "double";
        }
        if (method.Name == "renderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.Name == "compareOrder")
        {
            returnType = "global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>";
        }
        if (method.Name == "value" && declaration.Name is "SelectionContainerDelegate" or "_SelectionContainerState")
        {
            returnType = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        if (method.Name == "root" && declaration.Name is "_RawMenuAnchorState" or "_RawMenuAnchorGroupState" &&
            (declaration.Element.Mixins ?? []).FirstOrDefault(type =>
                StripLibraryPrefix(type).Split('<')[0] == "_RawMenuAnchorBaseMixin") is { } effectiveMenuMixin)
        {
            returnType = MapType(effectiveMenuMixin);
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "screenshot")
        {
            returnType = "Future<global::Doroti.Ui.Image?>";
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "_getSelectedWidgetLocation")
        {
            returnType = "global::Doroti.Runtime.CreationLocation?";
        }
        if (declaration.Name == "_UnspecifiedTextScaler" && method.Name is "scale" or "textScaleFactor")
        {
            returnType = "double";
        }
        if (method.IsGetter && method.Name == "renderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.IsGetter && method.Name == "compareOrder")
        {
            returnType = "global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>";
        }
        if (method.IsGetter && method.Name == "value" && declaration.Name is "SelectionContainerDelegate" or "_SelectionContainerState")
        {
            returnType = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        if (method.IsGetter && method.Name == "root" &&
            declaration.Name is "_RawMenuAnchorState" or "_RawMenuAnchorGroupState" &&
            (declaration.Element.Mixins ?? []).FirstOrDefault(type =>
                StripLibraryPrefix(type).Split('<')[0] == "_RawMenuAnchorBaseMixin") is { } menuMixin)
        {
            returnType = MapType(menuMixin);
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "screenshot")
        {
            returnType = "Future<global::Doroti.Ui.Image?>";
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "_getSelectedWidgetLocation")
        {
            returnType = "global::Doroti.Runtime.CreationLocation?";
        }
        if (method.IsGetter && method.Name == "renderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject?";
        }
        if (method.IsGetter && method.Name == "compareOrder")
        {
            returnType = "global::System.Func<global::Doroti.Framework.Rendering.Selectable, global::Doroti.Framework.Rendering.Selectable, long>";
        }
        if (declaration.Name == "_RouterState" && method.Name == "_handleRoutePopped")
        {
            returnType = "global::System.Func<bool, Future<bool>>";
        }
        if (declaration.Name == "MouseCursorManager" && method.Name == "handleDeviceCursorUpdate")
        {
            parameters = parameters.Select(parameter => parameter.Name == "triggeringEvent"
                ? parameter with { Type = "IPointerEvent" }
                : parameter).ToArray();
        }
        if (declaration.Name == "RenderViewportBase" && method.Name == "showInViewport")
        {
            parameters = parameters.Select(parameter => parameter.Name == "viewport"
                ? parameter with { Type = "RenderViewportBase<ParentDataClass>" }
                : parameter).ToArray();
        }
        if (isInterface && declaration.Ast.Kind == CoreNodeKind.MixinDeclaration &&
            method.Element.ReturnType?.Contains(" Function", StringComparison.Ordinal) == true)
        {
            returnType = MapType(method.Element.ReturnType);
        }
        if (isInterface && declaration.Name == "RenderAnimatedOpacityMixin" &&
            method.Name == "updateCompositedLayer")
        {
            returnType = "OffsetLayer";
            parameters = parameters.Select((parameter, index) => index == 0
                ? parameter with { Type = "OffsetLayer?" }
                : parameter).ToArray();
        }
        if (!method.IsStatic && (contractMember?.Element.ReturnType ?? contractMember?.Element.Type) is { } baseReturnType)
        {
            var mappedBaseReturnType = MapType(ApplyTypeParameterSubstitutions(baseReturnType, contractSubstitutions));
            // Future<T> derives from Future in the runtime, so Dart's covariant
            // Future return specialization is also a valid C# override.
            if (method.Name != "createRenderObject" &&
                !(mappedBaseReturnType == "Future" && returnType.StartsWith("Future<", StringComparison.Ordinal)) &&
                !(mappedBaseReturnType == "object" &&
                  returnType.StartsWith("Future<", StringComparison.Ordinal) &&
                  (contractMember?.Element.ReturnType?.StartsWith("FutureOr<", StringComparison.Ordinal) == true)))
            {
                // Prefer the specialized override return type when the inherited
                // contract still exposes an unbound type parameter (e.g. Curve
                // overriding ParametricCurve<double>.transform).
                if (!(ContainsUnboundTypeParameter(mappedBaseReturnType) &&
                      !ContainsUnboundTypeParameter(returnType)))
                {
                    var ownReturnDeclaration = FindGlobalDeclaration(returnType);
                    var baseReturnDeclaration = FindGlobalDeclaration(mappedBaseReturnType);
                    if (ownReturnDeclaration is null || baseReturnDeclaration is null ||
                        !IsDescendantOf(ownReturnDeclaration, baseReturnDeclaration))
                    {
                        returnType = mappedBaseReturnType;
                    }
                }
            }
        }
        if (method.Element.ReturnType == "dynamic" && contractMember is not null)
        {
            // Dart uses `dynamic` to implement generic contracts such as
            // MessageCodec<Object?>. The CLR override must expose the closed
            // contract type; keeping an analyzer AST placeholder here can form
            // an invalid synthetic nominal type such as Object_.
            returnType = "object";
        }
        if (method.Name == "createRenderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.Name == "createState")
        {
            returnType = "IState";
        }
        if (method.Name == "renderObject")
        {
            returnType = "global::Doroti.Framework.Rendering.RenderObject";
        }
        if (method.Name == "compareOrder")
        {
            returnType = "Comparison<global::Doroti.Framework.Rendering.Selectable>";
        }
        if (method.Name == "value" && declaration.Name is "SelectionContainerDelegate" or "_SelectionContainerState")
        {
            returnType = "global::Doroti.Framework.Rendering.SelectionGeometry";
        }
        if (method.Name == "root" && declaration.Name is "_RawMenuAnchorState" or "_RawMenuAnchorGroupState" &&
            (declaration.Element.Mixins ?? []).FirstOrDefault(type =>
                StripLibraryPrefix(type).Split('<')[0] == "_RawMenuAnchorBaseMixin") is { } canonicalMenuMixin)
        {
            returnType = MapType(canonicalMenuMixin);
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "screenshot")
        {
            returnType = "Future<global::Doroti.Ui.Image?>";
        }
        if (declaration.Name == "_WidgetInspectorService" && method.Name == "_getSelectedWidgetLocation")
        {
            returnType = "global::Doroti.Runtime.CreationLocation?";
        }
        if (declaration.Name == "_UnspecifiedTextScaler" && method.Name is "scale" or "textScaleFactor")
        {
            returnType = "double";
        }
        if (!method.IsStatic && method.Name == "performReassemble" &&
            IsKnownExternalOverride(declaration, method))
        {
            returnType = "Task";
        }
        var staticModifier = method.IsStatic ? "static " : string.Empty;
        var overrideModifier = method.Name switch
        {
            "toString" when parameters.Length == 0 => "override ",
            "hashCode" or "==" => "override ",
            _ when !method.IsStatic && (overriddenMember is not null || IsKnownExternalOverride(declaration, method)) => "override ",
            _ when method.IsStatic && overriddenMember is not null => "new ",
            _ => string.Empty,
        };
        if (declaration.Name == "RenderTwoDimensionalViewport" && method.Name == "markNeedsLayout")
        {
            // The Dart override adds an optional named argument. That is not a
            // CLR override of RenderObject.markNeedsLayout(), but callers still
            // need the optional argument on this concrete type.
            overrideModifier = string.Empty;
        }
        if (contractMember is not null && FindDeclaringDeclaration(contractMember) is { } contractOwner &&
            WillEmitAsInterface(contractOwner))
        {
            overrideModifier = string.Empty;
        }
        if (declaration.Element.Supertype is { } directSupertype &&
            ((StripLibraryPrefix(directSupertype).Split('<')[0] is "ShortcutActivator" or "TextScaler" &&
              method.Name is "accepts" or "debugDescribeKeys" or "scale" or "textScaleFactor") ||
             (declaration.Name == "TextSelectionHandleControls" && method.Name == "buildToolbar") ||
             (declaration.Name == "PopNavigatorRouterDelegateMixin" && method.Name == "popRoute")))
        {
            overrideModifier = "override ";
        }
        if (declaration.Name == "DefaultWidgetsLocalizations" && !method.IsStatic ||
            declaration.Name is "SingleActivator" or "CharacterActivator" && method.Name is "accepts" or "debugDescribeKeys" ||
            declaration.Name is "_NestedScrollPosition" or "ScrollPositionWithSingleContext" && method.Name == "axisDirection" ||
            declaration.Name == "_UnspecifiedTextScaler" && method.Name is "scale" or "textScaleFactor" ||
            declaration.Name == "TextSelectionHandleControls" && method.Name == "buildToolbar" ||
            declaration.Name == "PopNavigatorRouterDelegateMixin" && method.Name == "popRoute" ||
            method.Name == "createRenderObject" && declaration.Name != "RenderObjectWidget")
        {
            overrideModifier = "override ";
        }
        if (overrideModifier.Length == 0 && !method.IsStatic && !method.IsAbstract)
        {
            // Dart instance members participate in virtual dispatch by default.
            overrideModifier = "virtual ";
        }
        if (declaration.Name is "_OverridableAction" or "_OverridableContextAction" &&
            method.Name is "_defaultAction" or "_lookupContext")
        {
            overrideModifier = "virtual ";
        }
        if (declaration.Element.Supertype is { } slottedSupertype &&
            StripLibraryPrefix(slottedSupertype).Split('<')[0] == "SlottedMultiChildRenderObjectWidget" &&
            method.Name is "slots" or "childForSlot")
        {
            overrideModifier = "override ";
        }
        if (declaration.Name == "_SliverResizingHeader" && method.Name is "slots" or "childForSlot" ||
            declaration.Name is "_WidgetStateAnd" or "_WidgetStateOr" && method.Name == "isSatisfiedBy")
        {
            overrideModifier = "virtual ";
        }
        if (method.Name == "hashCode")
        {
            returnType = "int";
        }
        var methodName = MapMethodDeclarationName(method);
        if (methodName == "contains" && declaration.Element.TypeParameters is { Length: > 0 })
        {
            var elementType = declaration.Element.TypeParameters[0].Name;
            parameters = parameters.Select(parameter => parameter.Type is "Object?" or "object?" ? parameter with { Type = elementType } : parameter).ToArray();
        }
        if (declaration.Name == "BasicMessageChannel" && method.Name == "setMessageHandler")
        {
            parameters = parameters.Select((parameter, index) => index == 0
                ? parameter with { Type = "Future Function(T)?" }
                : parameter).ToArray();
        }
        if (method.Name is "loadStructuredData" or "loadStructuredBinaryData" && method.Element.TypeParameters is { Length: > 0 })
        {
            parameters = parameters.Select(parameter => parameter.Name == "parser"
                ? parameter with
                {
                    Type = parameter.Type.Contains("String", StringComparison.Ordinal)
                    ? "FutureOr<T> Function(String)"
                    : "FutureOr<T> Function(ByteData)"
                }
                : parameter).ToArray();
        }
        if (declaration.Name == "MethodChannel" && method.Name is "setMethodCallHandler" or "_handleAsMethodCall")
        {
            parameters = parameters.Select(parameter => parameter.Name == "handler"
                ? parameter with { Type = "Future Function(MethodCall)" }
                : parameter).ToArray();
        }
        if (declaration.Name == "ImageProvider" && method.Name == "_createErrorHandlerAndKey")
        {
            parameters = parameters.Select(parameter => parameter.Name == "successCallback"
                ? parameter with { Type = "void Function(T, void Function(Object, StackTrace?))" }
                : parameter).ToArray();
        }
        var visibility = IsDartPrivate(method) ? "internal" : "public";
        if (IsDartPrivate(method) &&
            (IsRequiredByMixinConstraint(method) ||
             AppliedMixinDeclarations(declaration).Any(mixin => mixin.Members.Any(candidate =>
                 candidate.Kind == "method" && candidate.Name == method.Name))))
        {
            // A C# interface contract is always public, including Dart library-
            // private members copied from an applied mixin.
            visibility = "public";
        }
        if (contractMember is not null && overriddenMember is null)
        {
            visibility = "public";
        }
        if (declaration.Name is "_OverridableAction" or "_OverridableContextAction" &&
            method.Name == "_updateCallingAction")
        {
            visibility = "internal";
        }
        if (overriddenMember is not null &&
            (IsRequiredByMixinConstraint(overriddenMember) || method.Name == "_layerHandle"))
        {
            visibility = "public";
        }
        if (overrideModifier == "override " && method.Name == "itemExtentBuilder")
        {
            returnType = "ItemExtentBuilder?";
            overrideModifier = "new ";
        }
        if (methodName is "ToString" or "GetHashCode" or "Equals" || isInterface)
        {
            visibility = "public";
        }
        if (method.Name == "_addPointerToArena")
        {
            // Doroti.Widgets owns the concrete recognizer policy in a separate
            // assembly and overrides this Dart-private framework hook.
            visibility = "public";
        }
        if (!method.IsStatic && overrideModifier == "override " &&
            method.Name is "initInstances" or "initServiceExtensions" or "unlocked" or "performReassemble")
        {
            visibility = "protected";
            overrideModifier = "override ";
        }
        var typeParameters = method.Element.TypeParameters is { Length: > 0 }
            ? $"<{string.Join(", ", method.Element.TypeParameters.Select(item => SafeIdentifier(item.Name)))}>"
            : string.Empty;
        var effectiveContractTypeParameters = contractMember is not null && overriddenMember is null
            ? null
            : method.Element.TypeParameters;
        var overrideTypeParameterConstraints = effectiveContractTypeParameters is not { Length: > 0 }
            ? string.Empty
            : overrideModifier == "override "
                ? string.Concat(effectiveContractTypeParameters
                    .Where(parameter => (method.Element.ReturnType?.Contains(parameter.Name + "?", StringComparison.Ordinal) ?? false) ||
                        parameters.Any(item => item.Type.Contains(parameter.Name + "?", StringComparison.Ordinal)))
                    .Select(parameter => $" where {SafeIdentifier(parameter.Name)} : default"))
                : FormatTypeParameterConstraints(effectiveContractTypeParameters,
                    parameters.Select(item => MapType(item.Type)).Append(returnType).Concat(
                        declaration.Members.SelectMany(member =>
                            new[] { member.Element.Type, member.Element.ReturnType }
                                .Concat((member.Element.Parameters ?? []).Select(parameter => parameter.Type)))
                            .Where(type => !string.IsNullOrWhiteSpace(type))
                            .Select(type => MapType(type!))));
        if (declaration.Name == "InheritedModel" &&
            method.Name is "_findModels" or "inheritFrom" &&
            method.Element.TypeParameters is { Length: 1 } inheritedModelMethodParameters)
        {
            // Dart's `T extends InheritedModel<Object?>` is covariant at the
            // call site. CLR generics are invariant, so constrain the lookup
            // to the actual widget base; the aspect hook is dispatched using
            // the analyzer-known type parameter at the invocation site.
            overrideTypeParameterConstraints =
                $" where {SafeIdentifier(inheritedModelMethodParameters[0].Name)} : InheritedWidget";
        }
        if (method.Name == "_getOverrideAction" &&
            method.Element.TypeParameters is { Length: 1 } overrideActionTypeParameters)
        {
            // The concrete mixin carrier implements the generic constraint
            // contract emitted for _OverridableActionMixin<T>.
            overrideTypeParameterConstraints =
                $" where {SafeIdentifier(overrideActionTypeParameters[0].Name)} : Intent";
        }
        var expressionBody = method.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
        var blockBody = method.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
        var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
        var block = blockBody is null ? null : blockBody.Child(CoreChildRole.blockOffset);
        var asyncModifier = IsDartAsync(method.Ast) && (IsFutureType(returnType) || returnType == "Task") ? "async " : string.Empty;
        IEnumerable<(CoreResolvedParameter First, CoreResolvedParameter Second)> comparableOverrideParameters = sourceParameters.Length == parameters.Length
            ? sourceParameters.Zip(parameters)
            : sourceParameters
                .Select(source => (First: source, Second: parameters.FirstOrDefault(candidate => candidate.Name == source.Name)))
                .Where(pair => pair.Second is not null)
                .Select(pair => (pair.First, pair.Second!));
        var promotedOverrideParameters = comparableOverrideParameters
            .Where(pair => pair.First.Type != pair.Second.Type &&
                pair.Second.Type.EndsWith("?", StringComparison.Ordinal) &&
                !pair.First.Type.EndsWith("?", StringComparison.Ordinal) &&
                IsValueType(MapType(pair.First.Type)))
            .Select(pair => (
                Name: SafeIdentifier(pair.First.Name),
                Local: SyntheticIdentifier(pair.First.Name),
                SourceType: MapType(pair.First.Type)))
            .ToArray();
        var narrowedOverrideParameters = comparableOverrideParameters
            .Select(pair =>
            {
                var sourceType = MapType(pair.First.Type);
                var contractType = MapType(pair.Second.Type);
                var sourceDeclaration = FindGlobalDeclaration(sourceType.TrimEnd('?'));
                var contractDeclaration = FindGlobalDeclaration(contractType.TrimEnd('?'));
                return (pair.First.Name, SourceType: sourceType, ContractType: contractType,
                    IsNarrower: sourceType.TrimEnd('?') != contractType.TrimEnd('?') &&
                        !IsValueType(sourceType.TrimEnd('?')) &&
                        ((sourceDeclaration is not null && contractDeclaration is not null &&
                          IsDescendantOf(sourceDeclaration, contractDeclaration)) ||
                         sourceDeclaration is null || contractDeclaration is null));
            })
            .Where(pair => pair.IsNarrower)
            .Select(pair => (
                Name: SafeIdentifier(pair.Name),
                Local: SyntheticIdentifier(pair.Name),
                pair.SourceType))
            .ToArray();
        if (method.Name == "handleEvent")
        {
            // Dart narrows HitTestEntry covariantly for render-object
            // implementations, but the body consumes only the common entry
            // contract. A CLR cast to BoxHitTestEntry/SliverHitTestEntry is
            // both unnecessary and invalid after HitTestResult.add adapts
            // typed entries to HitTestEntry<HitTestTarget>.
            narrowedOverrideParameters = narrowedOverrideParameters
                .Where(item => item.Name != "entry")
                .ToArray();
        }
        var renamedOverrideParameters = (sourceParameters.Length == parameters.Length
            ? sourceParameters.Zip(parameters)
            : [])
            .Where(pair => !string.Equals(pair.First.Name, pair.Second.Name, StringComparison.Ordinal))
            .Select(pair => (Source: SafeIdentifier(pair.First.Name), Contract: SafeIdentifier(pair.Second.Name)))
            .ToArray();

        if (method.IsOperator && method.Name is "[]" or "[]=")
        {
            if (method.Name == "[]=" && members.Any(item => item.IsOperator && item.Name == "[]"))
            {
                return;
            }
            var setter = method.Name == "[]"
                ? members.FirstOrDefault(item => item.IsOperator && item.Name == "[]=")
                : method;
            var getter = method.Name == "[]" ? method : null;
            EmitIndexer(builder, declaration, getter, setter, returnType, visibility, isInterface, package, library, inputPath, diagnostics);
            return;
        }

        if (isInterface)
        {
            if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration &&
                method.Name is "initInstances" or "initServiceExtensions" or "unlocked" or "performReassemble")
            {
                return;
            }
            if (method.IsStatic)
            {
                if (expression is not null)
                {
                    if (method.IsGetter)
                    {
                        var hasStaticSetter = members.Any(candidate =>
                            candidate.IsSetter && candidate.Name == method.Name && candidate.IsStatic);
                        builder.AppendLine($"    {visibility} {staticModifier}{returnType} {methodName}");
                        builder.AppendLine("    {");
                        builder.Append("        get => ");
                        LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                        if (hasStaticSetter && declaration.Name == "WidgetInspectorService" && method.Name == "instance")
                        {
                            builder.AppendLine("        set => _instance = value;");
                        }
                        builder.AppendLine("    }");
                        return;
                    }
                    builder.Append($"    {visibility} {staticModifier}{asyncModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))}) => ");
                    LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                    return;
                }
                if (block is not null)
                {
                    if (method.IsGetter)
                    {
                        builder.AppendLine($"    {visibility} {staticModifier}{returnType} {methodName}");
                        builder.AppendLine("    {");
                        builder.AppendLine("        get");
                        builder.AppendLine("        {");
                        EmitBlockBody(builder, block, declaration, package, library, inputPath, diagnostics, 3);
                        builder.AppendLine("        }");
                        builder.AppendLine("    }");
                        return;
                    }
                    builder.AppendLine($"    {visibility} {staticModifier}{asyncModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))})");
                    builder.AppendLine("    {");
                    EmitBlockBody(builder, block, declaration, package, library, inputPath, diagnostics, 2);
                    builder.AppendLine("    }");
                    return;
                }
            }
            if (declaration.Name == "WidgetsBindingObserver" && !method.IsAbstract &&
                (expression is not null || block is not null))
            {
                if (method.IsGetter)
                {
                    builder.AppendLine($"    public {returnType} {methodName}");
                    builder.AppendLine("    {");
                    if (expression is not null)
                    {
                        builder.Append("        get => ");
                        LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
                        builder.AppendLine(";");
                    }
                    else
                    {
                        builder.AppendLine("        get");
                        builder.AppendLine("        {");
                        EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 3);
                        builder.AppendLine("        }");
                    }
                    builder.AppendLine("    }");
                    return;
                }
                if (expression is not null)
                {
                    builder.Append($"    public {asyncModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))}) => ");
                    LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                    return;
                }
                builder.AppendLine($"    public {asyncModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))})");
                builder.AppendLine("    {");
                EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 2);
                builder.AppendLine("    }");
                return;
            }
            if (method.IsAbstract || method.IsGetter || method.IsSetter ||
                declaration.Ast.Kind == CoreNodeKind.MixinDeclaration ||
                HasPromotedInterfaceRepresentation(declaration))
            {
                if (method.IsGetter)
                {
                    var hasSetter = members.Any(candidate =>
                        candidate.IsSetter && candidate.Name == method.Name && candidate.IsStatic == method.IsStatic);
                    builder.AppendLine($"    {visibility} {staticModifier}{returnType} {methodName} {{ get;{(hasSetter ? " set;" : string.Empty)} }}");
                }
                else if (method.IsSetter)
                {
                    if (!members.Any(candidate =>
                        candidate.IsGetter && candidate.Name == method.Name && candidate.IsStatic == method.IsStatic))
                    {
                        var valueType = method.Element.Parameters is { Length: > 0 } setterParameters
                            ? MapType(setterParameters[^1].Type)
                            : "object";
                        builder.AppendLine($"    {visibility} {staticModifier}{valueType} {methodName} {{ set; }}");
                    }
                }
                else
                {
                    builder.AppendLine($"    {visibility} {staticModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))});");
                }
                return;
            }
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, method.Ast,
                "interface-member", "Only abstract and static members are supported in interface emission.");
            return;
        }

        if (method.Name == "==")
        {
            parameters = parameters.Select((item, index) => index == 0 ? item with { Type = "object?" } : item).ToArray();
        }

        if (methodName == "Equals")
        {
            var classType = $"{EmittedTypeName(library, declaration.Name)}{FormatTypeParameters(declaration.Element.TypeParameters)}";
            if (expression is not null)
            {
                builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}({string.Join(", ", MapParameters(parameters))})");
                builder.AppendLine("    {");
                builder.AppendLine($"        var __other = other as {classType};");
                builder.AppendLine("        if (__other is null) return false;");
                builder.Append("        return ");
                LowerExpressionWithLocalRename(builder, expression, declaration, package, library, inputPath, diagnostics, "other", "__other");
                builder.AppendLine(";");
                builder.AppendLine("    }");
                builder.AppendLine();
                return;
            }
            if (block is not null)
            {
                builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}({string.Join(", ", MapParameters(parameters))})");
                builder.AppendLine("    {");
                builder.AppendLine($"        var __other = other as {classType};");
                builder.AppendLine("        if (__other is null) return false;");
                EmitBlockBodyWithLocalRename(builder, block, declaration, package, library, inputPath, diagnostics, 2, "other", "__other");
                builder.AppendLine("    }");
                builder.AppendLine();
                return;
            }
        }

        if (method.IsGetter)
        {
            var matchingSetter = members.FirstOrDefault(item => item.IsSetter && item.Name == method.Name);
            EmitGetter(builder, declaration, method, members, matchingSetter, returnType, staticModifier, overrideModifier, methodName, visibility, expression, block, package, library, inputPath, diagnostics);
            return;
        }

        if (method.IsSetter)
        {
            if (members.Any(item => item.IsGetter && item.Name == method.Name))
            {
                return;
            }
            returnType = MapType(method.Element.Parameters?.FirstOrDefault()?.Type ?? "object");
            EmitSetterOnly(builder, declaration, method, returnType, staticModifier, overrideModifier, methodName, visibility, block, expression, package, library, inputPath, diagnostics);
            return;
        }

        if (method.IsAbstract)
        {
            builder.AppendLine($"    {visibility} abstract {staticModifier}{overrideModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))}){overrideTypeParameterConstraints};");
            return;
        }

        if (expression is not null)
        {
            builder.Append($"    {visibility} {staticModifier}{asyncModifier}{overrideModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))}){overrideTypeParameterConstraints} => ");
            if (renamedOverrideParameters.Length == 0)
            {
                LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
            }
            else
            {
                var expressionBuilder = new CsSyntaxBuilder();
                LowerExpression(expressionBuilder, expression, declaration, package, library, inputPath, diagnostics);
                var emittedExpression = expressionBuilder.Build();
                foreach (var renamed in renamedOverrideParameters)
                {
                    emittedExpression = emittedExpression.RenameIdentifier(renamed.Source, renamed.Contract);
                }
                builder.Append(emittedExpression);
            }
            builder.AppendLine(";");
            return;
        }

        if (block is not null)
        {
            builder.AppendLine($"    {visibility} {staticModifier}{asyncModifier}{overrideModifier}{returnType} {methodName}{typeParameters}({string.Join(", ", MapParameters(parameters))}){overrideTypeParameterConstraints}");
            builder.AppendLine("    {");
            foreach (var promoted in promotedOverrideParameters)
            {
                var renamed = renamedOverrideParameters.FirstOrDefault(item => item.Source == promoted.Name);
                var parameterName = string.IsNullOrEmpty(renamed.Contract) ? promoted.Name : renamed.Contract;
                builder.AppendLine($"        {promoted.SourceType} {promoted.Local} = DartRuntimePrimitives.ConvertValue<{promoted.SourceType}>({parameterName});");
            }
            foreach (var narrowed in narrowedOverrideParameters)
            {
                var renamed = renamedOverrideParameters.FirstOrDefault(item => item.Source == narrowed.Name);
                var parameterName = string.IsNullOrEmpty(renamed.Contract) ? narrowed.Name : renamed.Contract;
                if (narrowed.SourceType.EndsWith("?", StringComparison.Ordinal))
                {
                    builder.AppendLine($"        var {narrowed.Local} = {parameterName} is null ? null : ({narrowed.SourceType.TrimEnd('?')})(object){parameterName};");
                }
                else
                {
                    builder.AppendLine($"        var {narrowed.Local} = ({narrowed.SourceType})(object){parameterName};");
                }
            }
            if (promotedOverrideParameters.Length == 0 && narrowedOverrideParameters.Length == 0 && renamedOverrideParameters.Length == 0)
            {
                EmitBlockBodyWithReturnContract(
                    builder, block, declaration, returnType, package, library, inputPath, diagnostics, 2);
            }
            else
            {
                var bodyBuilder = new CsSyntaxBuilder();
                EmitBlockBodyWithReturnContract(
                    bodyBuilder, block, declaration, returnType, package, library, inputPath, diagnostics, 2);
                var body = bodyBuilder.Build();
                if (method.Name == "handleEvent" &&
                    narrowedOverrideParameters.Any(item => item.Name == "entry" && item.Local == "__entry"))
                {
                    body = body.RenameIdentifierInInvocation("handleEvent", "entry", "__contract_entry")
                        .RenameIdentifierInInvocation("debugHandleEvent", "entry", "__contract_entry");
                }
                foreach (var promoted in promotedOverrideParameters)
                {
                    body = body.RenameIdentifier(promoted.Name, promoted.Local);
                }
                foreach (var narrowed in narrowedOverrideParameters)
                {
                    body = body.RenameIdentifier(narrowed.Name, narrowed.Local);
                }
                foreach (var renamed in renamedOverrideParameters)
                {
                    body = body.RenameIdentifier(renamed.Source, renamed.Contract);
                }
                body = body.RenameIdentifier("__contract_entry", "entry");
                builder.Append(body);
            }
            if (returnType != "void" && !(asyncModifier.Length > 0 && returnType == "Future"))
            {
                builder.AppendLine("        throw new InvalidOperationException(\"Dart control flow completed without a value.\");");
            }
            builder.AppendLine("    }");
            builder.AppendLine();
            return;
        }

        AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, method.Ast,
            "class-method-body", "Add a typed block or expression method visitor before selecting this class.");
    }

    private static bool TryEmitRenderCustomClipCacheMethod(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method)
    {
        if (declaration.Name != "_RenderCustomClip")
        {
            return false;
        }

        if (method.Name == "_markNeedsClip")
        {
            builder.AppendLine("    internal virtual void _markNeedsClip()");
            builder.AppendLine("    {");
            builder.AppendLine("        _clip = default;");
            builder.AppendLine("        _clipIsValid = false;");
            builder.AppendLine("        markNeedsPaint();");
            builder.AppendLine("        markNeedsSemanticsUpdate();");
            builder.AppendLine("    }");
            return true;
        }

        if (method.Name == "performLayout")
        {
            builder.AppendLine("    public override void performLayout()");
            builder.AppendLine("    {");
            builder.AppendLine("        global::Doroti.Ui.Size? oldSize = hasSize ? size : null;");
            builder.AppendLine("        base.performLayout();");
            builder.AppendLine("        if (!object.Equals(oldSize, size))");
            builder.AppendLine("        {");
            builder.AppendLine("            _clip = default;");
            builder.AppendLine("            _clipIsValid = false;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            return true;
        }

        if (method.Name == "_updateClip")
        {
            builder.AppendLine("    internal virtual void _updateClip()");
            builder.AppendLine("    {");
            builder.AppendLine("        if (!_clipIsValid)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (_clipper is null)");
            builder.AppendLine("            {");
            builder.AppendLine("                _clip = _defaultClip;");
            builder.AppendLine("            }");
            builder.AppendLine("            else");
            builder.AppendLine("            {");
            builder.AppendLine("                _clip = _clipper.getClip(size);");
            builder.AppendLine("            }");
            builder.AppendLine("            _clipIsValid = true;");
            builder.AppendLine("        }");
            builder.AppendLine("    }");
            return true;
        }

        return false;
    }

    private string MapMethodDeclarationName(CoreResolvedMember method) => method.Name switch
    {
        "toString" => "ToString",
        "hashCode" => "GetHashCode",
        "iterator" => "GetEnumerator",
        "==" => "Equals",
        "+" => "op_Add",
        "-" => "op_Subtract",
        "*" => "op_Multiply",
        "/" => "op_Divide",
        "&" => "op_BitwiseAnd",
        "|" => "op_BitwiseOr",
        "~" => "op_OnesComplement",
        "%" => "op_Modulus",
        "~/" => "op_TruncateDivide",
        _ => SafeIdentifier(method.Name),
    };

    private void EmitIndexer(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember? getter,
        CoreResolvedMember? setter,
        string returnType,
        string visibility,
        bool isInterface,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var indexSource = getter ?? setter;
        if (indexSource is null)
        {
            return;
        }
        var indexParameters = (getter?.Element.Parameters ?? setter?.Element.Parameters?.Take(1).ToArray()) ?? [];
        if (indexParameters.Length != 1)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, indexSource.Ast,
                "indexer-shape", "Index operators must take an index parameter.");
            return;
        }
        var indexParameter = MapParameter(indexParameters[0]);
        var valueType = getter is not null
            ? returnType
            : MapType(setter!.Element.Parameters![1].Type);
        var abstractLike = isInterface || (getter?.IsAbstract ?? false) || (setter?.IsAbstract ?? false);
        if (abstractLike)
        {
            var accessors = (getter is not null ? "get; " : string.Empty) + (setter is not null ? "set; " : string.Empty);
            builder.AppendLine($"    {visibility} {valueType} this[{indexParameter}] {{ {accessors}}}");
            return;
        }

        builder.AppendLine($"    {visibility} {valueType} this[{indexParameter}]");
        builder.AppendLine("    {");
        if (getter is not null)
        {
            var getterExpression = getter.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody) is { } ge
                ? ge.Child(CoreChildRole.expressionOffset)
                : null;
            var getterBlock = getter.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody) is { } gb
                ? gb.Child(CoreChildRole.blockOffset)
                : null;
            builder.AppendLine("        get");
            builder.AppendLine("        {");
            if (getterExpression is not null)
            {
                builder.Append("            return ");
                LowerExpression(builder, getterExpression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
            else if (getterBlock is not null)
            {
                EmitBlockBody(builder, getterBlock, declaration, package, library, inputPath, diagnostics, 3);
                builder.AppendLine("            return default!;");
            }
            builder.AppendLine("        }");
        }
        if (setter is not null)
        {
            var setterParameters = setter.Element.Parameters ?? [];
            var setterExpression = setter.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody) is { } se
                ? se.Child(CoreChildRole.expressionOffset)
                : null;
            var setterBlock = setter.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody) is { } sb
                ? sb.Child(CoreChildRole.blockOffset)
                : null;
            builder.AppendLine("        set");
            builder.AppendLine("        {");
            if (setterParameters.Length >= 2)
            {
                var valueName = SafeIdentifier(setterParameters[1].Name);
                if (valueName != "value")
                {
                    builder.AppendLine($"            var {valueName} = value;");
                }
            }
            if (setterExpression is not null)
            {
                LowerExpression(builder, setterExpression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
            else if (setterBlock is not null)
            {
                EmitBlockBody(builder, setterBlock, declaration, package, library, inputPath, diagnostics, 3);
            }
            builder.AppendLine("        }");
        }
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private void EmitGetter(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        CoreResolvedMember[] members,
        CoreResolvedMember? matchingSetter,
        string returnType,
        string staticModifier,
        string overrideModifier,
        string methodName,
        string visibility,
        CoreAstNode? expression,
        CoreAstNode? block,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (matchingSetter is not null)
        {
            EmitGetterSetterProperty(builder, declaration, method, matchingSetter, returnType, staticModifier, overrideModifier, methodName, visibility, expression, block, package, library, inputPath, diagnostics);
            return;
        }

        if (methodName is "GetHashCode" or "GetEnumerator")
        {
            if (method.IsAbstract || (block is null && expression is null))
            {
                builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}();");
                return;
            }
            if (expression is not null)
            {
                builder.Append($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}() => ");
                LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
                return;
            }
            builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}()");
            builder.AppendLine("    {");
            EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 2);
            builder.AppendLine("        return default!;");
            builder.AppendLine("    }");
            return;
        }

        if (method.IsAbstract || (block is null && expression is null))
        {
            if (HasGlobalSetterOverride(declaration, method.Name))
            {
                builder.AppendLine($"    {visibility} virtual {staticModifier}{returnType} {methodName}");
                builder.AppendLine("    {");
                builder.AppendLine("        get => throw new NotSupportedException(\"Dart getter contract has no base implementation.\");");
                builder.AppendLine("        set => throw new NotSupportedException(\"Dart setter contract has no base implementation.\");");
                builder.AppendLine("    }");
                return;
            }
            builder.AppendLine($"    {visibility} abstract {staticModifier}{overrideModifier}{returnType} {methodName} {{ get; }}");
            return;
        }

        if (expression is not null)
        {
            builder.Append($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName} => ");
            LowerExpressionWithExpectedType(builder, expression, returnType, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine(";");
            return;
        }

        builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}");
        builder.AppendLine("    {");
        builder.AppendLine("        get");
        builder.AppendLine("        {");
        EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 3);
        builder.AppendLine("            return default!;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private void EmitGetterSetterProperty(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember getter,
        CoreResolvedMember setter,
        string returnType,
        string staticModifier,
        string overrideModifier,
        string methodName,
        string visibility,
        CoreAstNode? getterExpression,
        CoreAstNode? getterBlock,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var setterExpressionBody = DescendantsAndSelf(setter.Ast).FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
        var setterBlockBody = DescendantsAndSelf(setter.Ast).FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
        var setterExpression = setterExpressionBody is null ? null : setterExpressionBody.Child(CoreChildRole.expressionOffset);
        var setterBlock = setterBlockBody is null ? null : setterBlockBody.Child(CoreChildRole.blockOffset);
        var setterParameter = setter.Element.Parameters?.FirstOrDefault();
        var setterParameterName = setterParameter?.Name ?? "value";
        var localName = setterParameterName == "value" ? "__value" : setterParameterName;
        var needsRename = localName != setterParameterName;
        var setterSourceType = MapType(setterParameter?.Type ?? returnType);
        var setterSourceDeclaration = FindGlobalDeclaration(setterSourceType.TrimEnd('?'));
        var propertyDeclaration = FindGlobalDeclaration(returnType.TrimEnd('?'));
        var needsSetterNarrowing = setterSourceType.TrimEnd('?') != returnType.TrimEnd('?') &&
            !IsValueType(setterSourceType.TrimEnd('?')) &&
            ((setterSourceDeclaration is not null && propertyDeclaration is not null &&
              IsDescendantOf(setterSourceDeclaration, propertyDeclaration)) ||
             setterSourceDeclaration is null || propertyDeclaration is null);
        var needsSetterValuePromotion = IsValueType(setterSourceType) &&
            !setterSourceType.EndsWith("?", StringComparison.Ordinal) &&
            returnType == setterSourceType + "?";
        var needsNullableSetterLocal = setterSourceType == returnType + "?";

        builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}");
        builder.AppendLine("    {");
        builder.Append("        get");
        if (getterExpression is not null)
        {
            builder.Append(" => ");
            LowerExpression(builder, getterExpression, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine(";");
        }
        else if (getterBlock is not null)
        {
            builder.AppendLine("{");
            EmitBlockBody(builder, getterBlock, declaration, package, library, inputPath, diagnostics, 3);
            builder.AppendLine("            return default!;");
            builder.AppendLine("        }");
        }
        else
        {
            builder.AppendLine(" => throw new NotSupportedException(\"DOTF0001\");");
        }
        builder.AppendLine("        set");
        builder.AppendLine("        {");
        if (needsSetterValuePromotion)
        {
            builder.AppendLine($"            var {localName} = DartRuntimePrimitives.RequireValue(value);");
        }
        else if (needsNullableSetterLocal)
        {
            builder.AppendLine($"            {setterSourceType} {localName} = value;");
        }
        else if (needsSetterNarrowing)
        {
            builder.AppendLine(setterSourceType.EndsWith("?", StringComparison.Ordinal)
                ? $"            var {localName} = value is null ? null : ({setterSourceType.TrimEnd('?')})(object)value;"
                : $"            var {localName} = ({setterSourceType})(object)value;");
        }
        else if (needsRename)
        {
            builder.AppendLine($"            var {localName} = value;");
        }
        else
        {
            builder.AppendLine($"            var {setterParameterName} = value;");
        }
        if (setterBlock is not null)
        {
            EmitBlockBodyWithLocalRename(builder, setterBlock, declaration, package, library, inputPath, diagnostics, 3, setterParameterName, localName);
        }
        else if (setterExpression is not null)
        {
            builder.Append("            ");
            LowerExpressionWithLocalRename(builder, setterExpression, declaration, package, library, inputPath, diagnostics, setterParameterName, localName);
            builder.AppendLine(";");
        }
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private void EmitSetterOnly(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        string returnType,
        string staticModifier,
        string overrideModifier,
        string methodName,
        string visibility,
        CoreAstNode? block,
        CoreAstNode? expression,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var setterParameter = method.Element.Parameters?.FirstOrDefault();
        var setterParameterName = setterParameter?.Name ?? "value";
        var localName = setterParameterName == "value" ? "__value" : setterParameterName;
        var needsRename = localName != setterParameterName;
        var setterSourceType = MapType(setterParameter?.Type ?? returnType);
        var setterSourceDeclaration = FindGlobalDeclaration(setterSourceType.TrimEnd('?'));
        var propertyDeclaration = FindGlobalDeclaration(returnType.TrimEnd('?'));
        var needsSetterNarrowing = setterSourceType.TrimEnd('?') != returnType.TrimEnd('?') &&
            !IsValueType(setterSourceType.TrimEnd('?')) &&
            ((setterSourceDeclaration is not null && propertyDeclaration is not null &&
              IsDescendantOf(setterSourceDeclaration, propertyDeclaration)) ||
             setterSourceDeclaration is null || propertyDeclaration is null);
        var needsSetterValuePromotion = IsValueType(setterSourceType) &&
            !setterSourceType.EndsWith("?", StringComparison.Ordinal) &&
            returnType == setterSourceType + "?";
        var inheritsReadableProperty = declaration.Name == "TextEditingController" && method.Name == "value";
        if (inheritsReadableProperty) overrideModifier = "override ";
        builder.AppendLine($"    {visibility} {staticModifier}{overrideModifier}{returnType} {methodName}");
        builder.AppendLine("    {");
        if (inheritsReadableProperty)
        {
            builder.AppendLine($"        get => base.{methodName};");
        }
        builder.AppendLine("        set");
        builder.AppendLine("        {");
        if (needsSetterValuePromotion)
        {
            builder.AppendLine($"            var {localName} = DartRuntimePrimitives.RequireValue(value);");
        }
        else if (needsSetterNarrowing)
        {
            builder.AppendLine(setterSourceType.EndsWith("?", StringComparison.Ordinal)
                ? $"            var {localName} = value is null ? null : ({setterSourceType.TrimEnd('?')})(object)value;"
                : $"            var {localName} = ({setterSourceType})(object)value;");
        }
        else if (needsRename)
        {
            builder.AppendLine($"            var {localName} = value;");
        }
        else
        {
            builder.AppendLine($"            var {setterParameterName} = value;");
        }
        if (block is not null)
        {
            EmitBlockBodyWithLocalRename(builder, block, declaration, package, library, inputPath, diagnostics, 3, setterParameterName, localName);
        }
        else if (expression is not null)
        {
            builder.Append("            ");
            LowerExpressionWithLocalRename(builder, expression, declaration, package, library, inputPath, diagnostics, setterParameterName, localName);
            builder.AppendLine(";");
        }
        builder.AppendLine("        }");
        builder.AppendLine("    }");
    }

    private void EmitTopLevelFunction(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var functionExpression = declaration.Ast.Children.SingleOrDefault(item => item.Kind == CoreNodeKind.FunctionExpression);
        if (functionExpression is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, declaration.Ast,
                "function-shape", "Provide a typed FunctionExpression lowering rule.");
            return;
        }

        var returnType = MapType(declaration.Element.ReturnType ?? "void");
        if (declaration.Name == "_getCreationLocation" &&
            library.EndsWith("/widget_inspector.dart", StringComparison.Ordinal))
        {
            returnType = "global::Doroti.Runtime.CreationLocation?";
        }
        var parameters = declaration.Element.Parameters ?? [];
        var relevantTypes = new List<string>();
        if (declaration.Element.ReturnType is not null)
        {
            relevantTypes.Add(MapType(declaration.Element.ReturnType));
        }
        relevantTypes.AddRange(parameters.Select(parameter => MapType(parameter.Type)));
        var typeParameters = FormatTypeParameters(declaration.Element.TypeParameters);
        if (declaration.Name == "_stringify" && string.IsNullOrEmpty(typeParameters) && parameters.Length == 1)
        {
            typeParameters = "<T>";
            parameters = [parameters[0] with { Type = "List<T>" }];
        }
        var typeParameterConstraints = FormatTypeParameterConstraints(declaration.Element.TypeParameters, relevantTypes);
        var className = string.Equals(declaration.Name, "objectRuntimeType", StringComparison.Ordinal) &&
            string.Equals(library, "package:flutter/src/foundation/object.dart", StringComparison.Ordinal)
                ? "objectRuntimeTypeFunctions"
                : LibraryStaticClassName(library);
        var isGetter = declaration.Ast.Text(CoreProperty.isGetter) == "true";
        var isSetter = declaration.Ast.Text(CoreProperty.isSetter) == "true";
        var expressionBody = functionExpression.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
        var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
        var ownedBody = functionExpression.Child(CoreChildRole.bodyOffset);
        var body = DescendantsAndSelf(functionExpression).SingleOrDefault(item => item.Kind == CoreNodeKind.Block && item.Offset == ownedBody?.Offset);
        if (body is null)
        {
            body = DescendantsAndSelf(functionExpression).FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
        }
        var asyncModifier = IsDartAsync(functionExpression) && IsFutureType(returnType) ? "async " : string.Empty;
        var memberVisibility = IsDartPrivate(declaration) ? "internal" : "public";
        var restoredValueParameters = parameters.Where(NeedsNonConstValueDefault).ToArray();

        builder.AppendLine($"public static partial class {className}");
        builder.AppendLine("{");
        if (string.Equals(library, "package:flutter/src/widgets/_window_io.dart", StringComparison.Ordinal) &&
            declaration.Name == "createDefaultOwner")
        {
            // The generated Widgets layer is host-neutral. Concrete Win32/Linux/macOS
            // owner construction belongs to the registered host bridge.
            builder.AppendLine($"    {memberVisibility} static {returnType} createDefaultOwner() => null;");
            builder.AppendLine("}");
            builder.AppendLine();
            return;
        }
        if (isGetter)
        {
            var matchingSetter = _currentDeclarations?.FirstOrDefault(item =>
                item.Name == declaration.Name && item.Ast.Text(CoreProperty.isSetter) == "true");
            if (matchingSetter is not null)
            {
                var setterExpression = matchingSetter.Ast.Children.SingleOrDefault(item => item.Kind == CoreNodeKind.FunctionExpression);
                var setterExpressionBody = setterExpression?.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
                var setterValue = setterExpressionBody is null ? null : setterExpressionBody.Child(CoreChildRole.expressionOffset);
                var setterBody = setterExpression is null ? null : DescendantsAndSelf(setterExpression).FirstOrDefault(item => item.Kind == CoreNodeKind.Block);
                builder.AppendLine($"    {memberVisibility} static {returnType} {SafeIdentifier(declaration.Name)}");
                builder.AppendLine("    {");
                builder.Append("        get");
                if (expression is not null)
                {
                    builder.Append(" => ");
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                }
                else
                {
                    builder.AppendLine();
                    builder.AppendLine("        {");
                    if (body is not null)
                    {
                        EmitBlockBody(builder, body, declaration, package, library, inputPath, diagnostics, 3);
                    }
                    builder.AppendLine("        }");
                }
                builder.AppendLine("        set");
                builder.AppendLine("        {");
                if (setterBody is not null)
                {
                    EmitBlockBody(builder, setterBody, matchingSetter, package, library, inputPath, diagnostics, 3);
                }
                else if (setterValue is not null)
                {
                    builder.Append("            ");
                    LowerExpression(builder, setterValue, matchingSetter, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                }
                builder.AppendLine("        }");
                builder.AppendLine("    }");
            }
            else if (expression is not null)
            {
                builder.Append($"    {memberVisibility} static {returnType} {SafeIdentifier(declaration.Name)} => ");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
            else if (body is not null)
            {
                builder.AppendLine($"    {memberVisibility} static {returnType} {SafeIdentifier(declaration.Name)}");
                builder.AppendLine("    {");
                builder.AppendLine("        get");
                builder.AppendLine("        {");
                EmitBlockBody(builder, body, declaration, package, library, inputPath, diagnostics, 3);
                builder.AppendLine("        }");
                builder.AppendLine("    }");
            }
            else
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, functionExpression,
                    "function-body", "Add an expression-body or block-body typed lowering rule.");
            }
        }
        else if (isSetter)
        {
            builder.AppendLine($"    {memberVisibility} static {returnType} {SafeIdentifier(declaration.Name)}");
            builder.AppendLine("    {");
            builder.AppendLine("        set");
            builder.AppendLine("        {");
            if (expression is not null)
            {
                builder.Append("            ");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
            else if (body is not null)
            {
                EmitBlockBody(builder, body, declaration, package, library, inputPath, diagnostics, 3);
            }
            else
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, functionExpression,
                    "function-body", "Add an expression-body or block-body typed lowering rule.");
            }
            builder.AppendLine("        }");
            builder.AppendLine("    }");
        }
        else if (expression is not null)
        {
            if (restoredValueParameters.Length == 0)
            {
                builder.Append($"    {memberVisibility} static {asyncModifier}{returnType} {SafeIdentifier(declaration.Name)}{typeParameters}({string.Join(", ", MapParameters(parameters))}){typeParameterConstraints} => ");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                builder.AppendLine(";");
            }
            else
            {
                builder.AppendLine($"    {memberVisibility} static {asyncModifier}{returnType} {SafeIdentifier(declaration.Name)}{typeParameters}({string.Join(", ", MapParameters(parameters))}){typeParameterConstraints}");
                builder.AppendLine("    {");
                foreach (var parameter in restoredValueParameters)
                {
                    builder.AppendLine($"        {MapType(parameter.Type)} {SyntheticIdentifier(parameter.Name)} = {SafeIdentifier(parameter.Name)} ?? {MapParameterRuntimeDefault(parameter, library)};");
                }
                var expressionBuilder = new CsSyntaxBuilder();
                LowerExpression(expressionBuilder, expression, declaration, package, library, inputPath, diagnostics);
                var expressionSyntax = expressionBuilder.Build();
                foreach (var parameter in restoredValueParameters)
                {
                    expressionSyntax = expressionSyntax.RenameIdentifier(SafeIdentifier(parameter.Name), SyntheticIdentifier(parameter.Name));
                }
                builder.Append("        return ").Append(expressionSyntax).AppendLine(";");
                builder.AppendLine("    }");
            }
        }
        else
        {
            builder.AppendLine($"    {memberVisibility} static {asyncModifier}{returnType} {SafeIdentifier(declaration.Name)}{typeParameters}({string.Join(", ", MapParameters(parameters))}){typeParameterConstraints}");
            builder.AppendLine("    {");
            if (body is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, functionExpression,
                    "function-body", "Add an expression-body or block-body typed lowering rule.");
            }
            else
            {
                foreach (var parameter in restoredValueParameters)
                {
                    builder.AppendLine($"        {MapType(parameter.Type)} {SyntheticIdentifier(parameter.Name)} = {SafeIdentifier(parameter.Name)} ?? {MapParameterRuntimeDefault(parameter, library)};");
                }
                var bodyBuilder = new CsSyntaxBuilder();
                EmitBlockBody(bodyBuilder, body, declaration, package, library, inputPath, diagnostics, 2);
                var bodySyntax = bodyBuilder.Build();
                foreach (var parameter in restoredValueParameters)
                {
                    bodySyntax = bodySyntax.RenameIdentifier(SafeIdentifier(parameter.Name), SyntheticIdentifier(parameter.Name));
                }
                builder.Append(bodySyntax);
            }
            if (returnType != "void" && !(asyncModifier.Length > 0 && returnType == "Future") &&
                !(string.Equals(declaration.Name, "objectRuntimeType", StringComparison.Ordinal) &&
                  string.Equals(library, "package:flutter/src/foundation/object.dart", StringComparison.Ordinal)))
            {
                builder.AppendLine("        throw new InvalidOperationException(\"Dart control flow completed without a value.\");");
            }
            builder.AppendLine("    }");
        }

        builder.AppendLine("}");
        builder.AppendLine();
    }

}
