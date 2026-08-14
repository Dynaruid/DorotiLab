using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitExtension(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var onClause = declaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExtensionOnClause);
        var receiverNode = onClause?.Children.FirstOrDefault(item => item.Category == "type");
        if (receiverNode is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, declaration.Ast,
                "extension-receiver", "Expose the resolved extension receiver type in typed IR.");
            return;
        }

        var visibility = IsDartPrivate(declaration) ? "internal" : "public";
        var receiverType = MapTypeFromAst(receiverNode);
        var extensionName = EmittedTypeName(library, declaration.Name) + "Extension";
        builder.AppendLine($"{visibility} static class {extensionName}");
        builder.AppendLine("{");
        foreach (var member in declaration.Members.Where(item => item.Kind == "method").OrderBy(item => item.Offset))
        {
            var expressionBody = member.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
            var blockBody = member.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
            var expression = expressionBody?.Child(CoreChildRole.expressionOffset);
            var block = blockBody?.Child(CoreChildRole.blockOffset);
            if (expression is null && block is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, member.Ast,
                    "extension-member-body", "Expose the typed extension member body.");
                continue;
            }

            var returnType = MapType(member.Element.ReturnType ?? member.Element.Type ?? "object");
            var methodName = MapMethodDeclarationName(member);
            var parameters = MapParameters(member.Element.Parameters ?? []).ToArray();
            var receiver = $"this {receiverType} value";
            var previousThis = _session.ExplicitThisExpression;
            _session.ExplicitThisExpression = "value";
            try
            {
                builder.Append($"    {visibility} static {returnType} {methodName}({string.Join(", ", new[] { receiver }.Concat(parameters))})");
                if (expression is not null)
                {
                    builder.Append(" => ");
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                }
                else
                {
                    builder.AppendLine();
                    builder.AppendLine("    {");
                    EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 2);
                    if (returnType != "void")
                    {
                        builder.AppendLine("        throw new InvalidOperationException(\"Dart control flow completed without a value.\");");
                    }
                    builder.AppendLine("    }");
                }
            }
            finally
            {
                _session.ExplicitThisExpression = previousThis;
            }
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private void EmitTypeAlias(CsSyntaxBuilder builder, CoreResolvedDeclaration declaration)
    {
        var visibility = IsDartPrivate(declaration) ? "internal" : "public";
        if (declaration.Name == "PageRouteFactory")
        {
            // CLR delegates cannot declare a generic Invoke method. Dart uses
            // this callback through Route<dynamic>, so erase only the method
            // type parameter while retaining the named callback boundary.
            builder.AppendLine($"{visibility} delegate Route<object> PageRouteFactory(RouteSettings settings, global::System.Func<BuildContext, Widget> builder);");
            builder.AppendLine();
            return;
        }
        if (declaration.Name == "TraversalRequestFocusCallback")
        {
            // The analyzer canonicalizes named parameters alphabetically.  C#
            // delegate binding, however, is positional, so retain Flutter's
            // source order for the requestFocusCallback method tear-offs.
            builder.AppendLine($"{visibility} delegate void TraversalRequestFocusCallback(FocusNode node, ScrollPositionAlignmentPolicy? alignmentPolicy = null, double? alignment = null, Duration? duration = null, global::Doroti.Generated.Framework.Animation.Curve? curve = null);");
            builder.AppendLine();
            return;
        }
        var returnType = MapType(declaration.Element.ReturnType ?? "void");
        var typeParameters = FormatTypeParameters(declaration.Element.TypeParameters);
        var constraints = FormatTypeParameterConstraints(
            declaration.Element.TypeParameters,
            new[] { returnType }.Concat((declaration.Element.Parameters ?? []).Select(item => MapType(item.Type))));
        var parameters = string.Join(", ", MapParameters(declaration.Element.Parameters ?? []));
        var emittedName = EmittedTypeName(LibraryUriFromElementId(declaration.Element.CanonicalId), declaration.Name);
        builder.AppendLine($"{visibility} delegate {returnType} {emittedName}{typeParameters}({parameters}){constraints};");
        builder.AppendLine();
    }

    private void EmitTopLevelVariable(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        // A single Dart declaration may introduce multiple top-level variables.
        // Every resolved declaration points at the shared declaration-list AST,
        // so select the variable matching the element instead of always lowering
        // the first sibling (which duplicated names and initializers in C#).
        var variable = DescendantsAndSelf(declaration.Ast).FirstOrDefault(item =>
                item.Kind == CoreNodeKind.VariableDeclaration &&
                string.Equals(item.Text(CoreProperty.name), declaration.Name, StringComparison.Ordinal))
            ?? DescendantsAndSelf(declaration.Ast).FirstOrDefault(item => item.Kind == CoreNodeKind.VariableDeclaration);
        var name = SafeIdentifier(variable?.Text(CoreProperty.name) ?? declaration.Name);
        var type = MapType(declaration.Element.Type ?? "object");
        var visibility = IsDartPrivate(declaration) ? "internal" : "public";
        var initializer = variable is null ? null : variable.Child(CoreChildRole.initializerOffset);
        var containerName = LibraryStaticClassName(library);
        var fieldModifier = "static";

        builder.AppendLine($"public static partial class {containerName}");
        builder.AppendLine("{");
        builder.Append($"    {visibility} {fieldModifier} {type} {name}");
        if (initializer is not null)
        {
            builder.Append(" = ");
            EmitFieldInitializer(builder, type, initializer, declaration, package, library, inputPath, diagnostics);
        }
        builder.AppendLine(";");
        builder.AppendLine("}");
        builder.AppendLine();

    }

    private void EmitEnum(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var visibility = declaration.Name == "_StateLifecycle"
            ? "public"
            : IsDartPrivate(declaration) && !IsPublicMixinStorageType(declaration.Name) ? "internal" : "public";
        var enumName = EmittedTypeName(library, declaration.Name);
        var names = DescendantsAndSelf(declaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.EnumConstantDeclaration)
            .Select(item => item.Text(CoreProperty.name))
            .Where(item => !string.IsNullOrEmpty(item))
            .Select(item => SafeIdentifier(item!))
            .ToArray();
        builder.AppendLine($"{visibility} enum {enumName}");
        builder.AppendLine("{");
        for (var index = 0; index < names.Length; index++)
        {
            var suffix = index == names.Length - 1 ? string.Empty : ",";
            builder.AppendLine($"    {names[index]}{suffix}");
        }
        builder.AppendLine("}");
        builder.AppendLine();

        var instanceMembers = declaration.Members
            .Where(member => member.Kind == "method" && !member.IsStatic)
            .OrderBy(member => member.Offset)
            .ToArray();
        if (instanceMembers.Length == 0)
        {
            return;
        }

        builder.AppendLine($"{visibility} static class {enumName}Members");
        builder.AppendLine("{");
        foreach (var member in instanceMembers)
        {
            var expressionBody = member.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
            var blockBody = member.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
            var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
            var block = blockBody is null ? null : blockBody.Child(CoreChildRole.blockOffset);
            if (expression is null && block is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, member.Ast,
                    "enum-member", "Enum instance members require a typed expression or block body.");
                continue;
            }
            var memberVisibility = IsDartPrivate(member) ? "internal" : "public";
            var returnType = MapType(member.Element.ReturnType ?? member.Element.Type ?? "object");
            var methodName = MapMethodDeclarationName(member);
            var parameters = MapParameters(member.Element.Parameters ?? []).ToArray();
            var receiverAndParameters = string.Join(", ", new[] { $"this {enumName} value" }.Concat(parameters));
            var previousThis = _session.ExplicitThisExpression;
            var previousEnum = _session.ExplicitEnumDeclaration;
            _session.ExplicitThisExpression = "value";
            _session.ExplicitEnumDeclaration = member.IsGetter ? declaration : null;
            try
            {
                if (member.IsGetter)
                {
                    builder.Append($"    {memberVisibility} static {returnType} {methodName}(this {enumName} value)");
                }
                else
                {
                    builder.Append($"    {memberVisibility} static {returnType} {methodName}({receiverAndParameters})");
                }
                if (expression is not null)
                {
                    builder.Append(" => ");
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    builder.AppendLine(";");
                }
                else
                {
                    builder.AppendLine();
                    builder.AppendLine("    {");
                    EmitBlockBody(builder, block!, declaration, package, library, inputPath, diagnostics, 2);
                    if (returnType != "void")
                    {
                        builder.AppendLine("        throw new InvalidOperationException(\"Dart control flow completed without a value.\");");
                    }
                    builder.AppendLine("    }");
                }
            }
            finally
            {
                _session.ExplicitThisExpression = previousThis;
                _session.ExplicitEnumDeclaration = previousEnum;
            }
        }
        builder.AppendLine("}");
        builder.AppendLine();

        var implementedInterface = (declaration.Element.Interfaces ?? [])
            .FirstOrDefault(type => type.Contains('<', StringComparison.Ordinal));
        if (implementedInterface is not null)
        {
            var adapterName = enumName + "InterfaceAdapter";
            builder.AppendLine($"{visibility} sealed class {adapterName} : {MapType(implementedInterface)}");
            builder.AppendLine("{");
            builder.AppendLine($"    private readonly {enumName} _value;");
            builder.AppendLine($"    public {adapterName}({enumName} value) => _value = value;");
            foreach (var member in instanceMembers)
            {
                var returnType = MapType(member.Element.ReturnType ?? member.Element.Type ?? "object");
                var methodName = MapMethodDeclarationName(member);
                var parameters = MapParameters(member.Element.Parameters ?? []).ToArray();
                var argumentNames = string.Join(", ", (member.Element.Parameters ?? []).Select(parameter => SafeIdentifier(parameter.Name)));
                if (member.IsGetter)
                {
                    builder.AppendLine($"    public {returnType} {methodName} => _value.{methodName}();");
                }
                else
                {
                    builder.AppendLine($"    public {returnType} {methodName}({string.Join(", ", parameters)}) => _value.{methodName}({argumentNames});");
                }
            }
            builder.AppendLine("}");
            builder.AppendLine();
        }
    }

    private void EmitClass(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var visibility = IsDartPrivate(declaration) || IsPrivateCompanionLibrary(library)
            ? IsPublicMixinStorageType(declaration.Name) ? "public" : "internal"
            : "public";
        if (declaration.Name is "_UiKitPlatformView" or "_AppKitPlatformView")
        {
            visibility = "internal";
        }
        var abstractModifier = declaration.Element.IsAbstract || declaration.Ast.Kind == CoreNodeKind.MixinDeclaration
            ? "abstract "
            : string.Empty;
        if (declaration.Name == "GlobalKey") abstractModifier = string.Empty;
        var typeParameters = FormatTypeParameters(declaration.Element.TypeParameters);
        var typeParameterConstraints = FormatTypeParameterConstraints(declaration.Element.TypeParameters, declaration);
        var bases = new List<string>();
        var displacedStructuralSuperclass = DisplacedStructuralSuperclass(declaration);
        var structuralWrapperBase = displacedStructuralSuperclass is null
            ? null
            : ConcreteImplementedClass(declaration);
        var mappedDeclaredSupertype = declaration.Element.Supertype is { } declaredSupertype && declaredSupertype != "Object"
            ? MapInheritanceType(declaredSupertype)
            : null;
        var declaredSupertypeDeclaration = mappedDeclaredSupertype is null
            ? null
            : FindGlobalDeclaration(mappedDeclaredSupertype);
        var declaredSupertypeIsInterface = mappedDeclaredSupertype is not null &&
            (mappedDeclaredSupertype is "IEnumerable" || mappedDeclaredSupertype.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
             declaredSupertypeDeclaration is not null && WillEmitAsInterface(declaredSupertypeDeclaration));
        var hasConcreteBase = structuralWrapperBase is not null ||
            mappedDeclaredSupertype is not null && !declaredSupertypeIsInterface;
        if (structuralWrapperBase is not null)
        {
            bases.Add(MapType(structuralWrapperBase));
        }
        else if (declaration.Element.Supertype is { } supertype && supertype != "Object")
        {
            bases.Add(MapInheritanceType(supertype));
        }
        if (declaration.Name == "RenderingFlutterBinding" &&
            (declaration.Element.Mixins ?? []).Any(type => StripLibraryPrefix(type).Split('<')[0] == "GestureBinding"))
        {
            // The reviewed binding chain is a CLR abstract-class chain through
            // Scheduler -> Services -> Gestures. Use its most-specific product
            // base while the G4-5 mixins remain generated interfaces below.
            bases.Clear();
            bases.Add("global::Doroti.Generated.Framework.Gestures.GestureBinding");
            hasConcreteBase = true;
        }
        if (declaration.Name == "WidgetsFlutterBinding" &&
            (declaration.Element.Mixins ?? []).Any(type => StripLibraryPrefix(type).Split('<')[0] == "GestureBinding"))
        {
            // G5-3 consumes the reviewed Scheduler -> Services -> Gestures CLR
            // class chain and keeps the remaining binding mixins as interfaces.
            bases.Clear();
            bases.Add("global::Doroti.Generated.Framework.Gestures.GestureBinding");
            hasConcreteBase = true;
        }
        if (!hasConcreteBase && (declaration.Element.Mixins ?? []).Any(type =>
            StripLibraryPrefix(type).Split('<')[0] == "ChangeNotifier"))
        {
            bases.Insert(0, "ChangeNotifier");
            hasConcreteBase = true;
        }
        if (!hasConcreteBase && (declaration.Element.Mixins ?? [])
            .Select(MapType)
            .FirstOrDefault(type => FindGlobalDeclaration(type) is { } mixin && !WillEmitAsInterface(mixin)) is { } promotedMixinBase)
        {
            // A Dart `with` application may reference a contract deliberately
            // emitted as a CLR class (for example TextInputControl). With no
            // Dart superclass, that class is the one legal CLR base; the other
            // mixin interfaces remain in the base list below.
            bases.Add(promotedMixinBase);
            hasConcreteBase = true;
        }
        if (declaration.Name == "SelectionRegistrant" && !bases.Contains("ChangeNotifier"))
        {
            // The analyzer exposes this `with ChangeNotifier` application as a
            // synthetic superclass rather than in Element.Mixins. Preserve the
            // concrete mixin implementation before processing Dart's structural
            // `implements Selectable` clause.
            bases.Insert(0, "ChangeNotifier");
            hasConcreteBase = true;
        }
        if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration)
        {
            var onClause = declaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.MixinOnClause);
            if (onClause is not null)
            {
                var onTypes = onClause.Children
                    .Where(item => item.Category == "type")
                    .Select(MapTypeFromAst)
                    .ToArray();
                if (onTypes.Length > 0)
                {
                    // Dart permits multiple superclass constraints; the final constraint
                    // is the most-specific class in Flutter's binding mixin chain.
                    bases.Add(onTypes[^1]);
                }
            }
        }
        if (declaration.Name == "GestureBinding")
        {
            // Flutter applies SchedulerBinding, ServicesBinding, and
            // GestureBinding together on concrete bindings. CLR inheritance is
            // linear, so retain the reviewed Scheduler -> Services -> Gestures
            // product chain at the generated GestureBinding boundary.
            bases.Clear();
            bases.Add("global::Doroti.Generated.Framework.Services.ServicesBinding");
            hasConcreteBase = true;
        }
        if (declaration.Name == "PointerEvent")
        {
            bases.Add("global::Doroti.Flutter.Runtime.IPointerEvent");
        }
        if (declaration.Name == "State")
        {
            bases.Add("IState");
        }
        bases.AddRange((declaration.Element.Mixins ?? [])
            .Where(type => !(declaration.Name == "SlottedMultiChildRenderObjectWidget" &&
                StripLibraryPrefix(type).Split('<')[0] == "SlottedMultiChildRenderObjectWidgetMixin"))
            .Where(type => !(declaration.Name is "_OverridableAction" or "_OverridableContextAction" &&
                StripLibraryPrefix(type).Split('<')[0] == "_OverridableActionMixin"))
            .Select(MapType)
            .Where(type => FindGlobalDeclaration(type) is { } mixin
                ? WillEmitAsInterface(mixin)
                : HasPromotedInterfaceRepresentation(StripLibraryPrefix(type).Split('<')[0])));
        // Dart `implements Class` is structural; C# allows only one class base. Keep
        // the true superclass and omit class-typed implements entries (mixin/interface
        // implements remain).
        foreach (var interfaceType in declaration.Element.Interfaces ?? [])
        {
            var mapped = IsPrivateCompanionLibrary(library) &&
                StripLibraryPrefix(interfaceType).Split('<')[0] == "NetworkImage"
                ? "NetworkImage"
                : MapType(interfaceType);
            if (mapped.Length == 0)
            {
                continue;
            }
            var interfaceDeclaration = FindGlobalDeclaration(mapped);
            var simpleInterface = StripLibraryPrefix(mapped).Split('<')[0];
            if (simpleInterface == "MouseTrackerAnnotation")
            {
                // MouseTrackerAnnotation lives in Services while its callback
                // event types live in Gestures (which already depends on
                // Services). Preserve Dart's structural implements relation
                // through the generated cross-assembly companion contract.
                bases.Add("global::Doroti.Generated.Framework.Services.IMouseTrackerAnnotation");
                continue;
            }
            if (!hasConcreteBase && simpleInterface == "Color")
            {
                // dart:ui Color is a concrete SDK class. Dart permits structural
                // `implements Color`; the CLR representation must put that class
                // before mixin interfaces such as Diagnosticable.
                bases.Insert(0, mapped);
                hasConcreteBase = true;
                continue;
            }
            if (!hasConcreteBase &&
                interfaceDeclaration is not null &&
                interfaceDeclaration.Ast.Kind != CoreNodeKind.MixinDeclaration &&
                !WillEmitAsInterface(interfaceDeclaration))
            {
                // Dart's abstract `implements ConcreteClass` is structural. When
                // there is no other superclass, deriving from that class is the
                // only CLR representation that preserves its contract and lets
                // concrete subclasses remain assignable to the implemented type.
                // A concrete CLR base must precede mixin interfaces already
                // collected above (for example Diagnosticable).
                bases.Insert(0, mapped);
                hasConcreteBase = true;
                continue;
            }
            if (interfaceDeclaration is null ||
                interfaceDeclaration.Ast.Kind == CoreNodeKind.MixinDeclaration ||
                WillEmitAsInterface(interfaceDeclaration))
            {
                bases.Add(mapped);
                continue;
            }
            // Known product interfaces that are not in this compilation unit.
            if (simpleInterface is "Diagnosticable" or "DiagnosticableTree" or "Listenable" or "ValueListenable")
            {
                bases.Add(mapped);
            }
        }
        var isPlatformNetworkImage = IsPrivateCompanionLibrary(library) &&
            string.Equals(declaration.Name, "NetworkImage", StringComparison.Ordinal);
        if (isPlatformNetworkImage)
        {
            bases.Add("NetworkImage");
        }
        var baseList = bases.Count == 0 ? string.Empty : " : " + string.Join(", ", bases.Distinct(StringComparer.Ordinal));
        var isMixinDeclaration = declaration.Ast.Kind == CoreNodeKind.MixinDeclaration;
        // Dart mixins must lower to interfaces so classes can `extend Super, Mixin1, Mixin2`.
        var isInterface = WillEmitAsInterface(declaration);
        var kind = isInterface ? "interface" : "class";
        var classModifiers = isInterface ? string.Empty : abstractModifier;
        var emittedName = EmittedTypeName(library, declaration.Name);
        // Mixin interfaces may only extend other interfaces (never the `on` class).
        if (isInterface)
        {
            var mixinInterfaces = (declaration.Element.Interfaces ?? [])
                .Select(MapType)
                .Where(type => type.Length > 0)
                .Where(type =>
                {
                    var interfaceDeclaration = FindGlobalDeclaration(type);
                    return interfaceDeclaration is null ||
                        interfaceDeclaration.Ast.Kind == CoreNodeKind.MixinDeclaration ||
                        WillEmitAsInterface(interfaceDeclaration);
                })
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            baseList = mixinInterfaces.Length == 0
                ? string.Empty
                : " : " + string.Join(", ", mixinInterfaces);
        }
        if (!isInterface && declaration.Name == "MouseTrackerAnnotation")
        {
            builder.AppendLine("public interface IMouseTrackerAnnotation");
            builder.AppendLine("{");
            builder.AppendLine("    dynamic onEnter => ((dynamic)this).onEnter;");
            builder.AppendLine("    dynamic onExit => ((dynamic)this).onExit;");
            builder.AppendLine("    MouseCursor cursor => (MouseCursor)((dynamic)this).cursor;");
            builder.AppendLine("    bool validForMouseTracker => (bool)((dynamic)this).validForMouseTracker;");
            builder.AppendLine("}");
            builder.AppendLine();
            bases.Add("IMouseTrackerAnnotation");
            baseList = " : " + string.Join(", ", bases.Distinct(StringComparer.Ordinal));
        }
        builder.AppendLine($"{visibility} {classModifiers}{kind} {emittedName}{typeParameters}{baseList}{typeParameterConstraints}");
        builder.AppendLine("{");

        if (declaration.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration)
        {
            var representation = declaration.Ast.Children.FirstOrDefault(item =>
                item.Kind == CoreNodeKind.RepresentationDeclaration);
            var representationTypeNode = representation?.Children.FirstOrDefault(item => item.Category == "type");
            if (representation is null || representationTypeNode is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, declaration.Ast,
                    "extension-type-representation", "Expose the typed extension representation declaration.");
            }
            else
            {
                var representationType = MapTypeFromAst(representationTypeNode);
                var representationName = SafeIdentifier(representation.Text(CoreProperty.name) ?? "value");
                var representationConstructor = representation.Text(CoreProperty.constructor) ?? "new";
                builder.AppendLine($"    public {representationType} {representationName} {{ get; }}");
                builder.AppendLine();
                if (representationConstructor is "new" or "")
                {
                    builder.AppendLine($"    public {emittedName}({representationType} {representationName})");
                }
                else
                {
                    builder.AppendLine($"    private {emittedName}({representationType} {representationName})");
                }
                builder.AppendLine("    {");
                builder.AppendLine($"        this.{representationName} = {representationName};");
                builder.AppendLine("    }");
                builder.AppendLine();
                if (representationConstructor is not ("new" or ""))
                {
                    builder.AppendLine($"    public static {emittedName} {NamedConstructorMethodName(representationConstructor)}({representationType} {representationName}) => new {emittedName}({representationName});");
                    builder.AppendLine();
                }
                if (representationType is not ("object" or "dynamic"))
                {
                    builder.AppendLine($"    public static implicit operator {representationType}({emittedName} value) => value.{representationName};");
                    builder.AppendLine($"    public static implicit operator {emittedName}({representationType} value) => new {emittedName}(value);");
                    builder.AppendLine();
                }
            }
        }

        var requiredMixinStorage = ImplementationDonorDeclarations(declaration)
            .Where(mixin => !HasPromotedClassRepresentation(mixin))
            .SelectMany(mixin => mixin.Members)
            .Where(member => member.Kind == "field")
            .Select(member => member.Name)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var field in declaration.Members.Where(item => item.Kind == "field").OrderBy(item => item.Offset))
        {
            if (isInterface)
            {
                if (field.IsStatic)
                {
                    EmitField(builder, declaration, field, package, library, inputPath, diagnostics);
                    continue;
                }
                // Interface-hosted state: abstract properties filled by the mixing class.
                var fieldType = MapType(field.Element.Type ?? "object");
                var fieldName = SafeIdentifier(field.Name);
                var getterOnlyBindingField = declaration.Name == "RendererBinding" &&
                    field.Name is "_manifold" or "pipelineOwner" or "renderView";
                var setter = field.IsFinal || getterOnlyBindingField
                    ? string.Empty
                    : " set;";
                builder.AppendLine($"    {fieldType} {fieldName} {{ get;{setter} }}");
            }
            else
            {
                EmitField(builder, declaration, field, package, library, inputPath, diagnostics, requiredMixinStorage.Contains(field.Name));
            }
        }
        if (!isInterface && declaration.Name == "RenderMouseRegion")
        {
            builder.AppendLine("    dynamic global::Doroti.Generated.Framework.Services.IMouseTrackerAnnotation.onEnter => this.onEnter;");
            builder.AppendLine("    dynamic global::Doroti.Generated.Framework.Services.IMouseTrackerAnnotation.onExit => this.onExit;");
        }
        // Concrete storage for mixin interface fields used by this class.
        if (!isMixinDeclaration)
        {
            var emittedFieldNames = declaration.Members
                .Where(item => item.Kind == "field")
                .Select(item => item.Name)
                .ToHashSet(StringComparer.Ordinal);
            foreach (var mixinDeclaration in ImplementationDonorDeclarations(declaration)
                .Where(mixin => !HasPromotedClassRepresentation(mixin)))
            {
                var previousSubstitutions = _session.TypeParameterSubstitutions;
                _session.TypeParameterSubstitutions = TypeParameterSubstitutions(declaration, mixinDeclaration);
                try
                {
                    foreach (var field in mixinDeclaration.Members.Where(item => item.Kind == "field" && !item.IsStatic).OrderBy(item => item.Offset))
                    {
                        if (!emittedFieldNames.Add(field.Name))
                        {
                            continue;
                        }
                        var previousSourceLibrary = _session.ActiveSourceLibrary;
                        var previousDonorDeclaration = _session.ActiveDonorDeclaration;
                        _session.ActiveSourceLibrary = LibraryUriFromElementId(mixinDeclaration.Element.CanonicalId);
                        _session.ActiveDonorDeclaration = mixinDeclaration;
                        try
                        {
                            EmitField(builder, mixinDeclaration, field, package, library, inputPath, diagnostics, forcePublic: true);
                        }
                        finally
                        {
                            _session.ActiveSourceLibrary = previousSourceLibrary;
                            _session.ActiveDonorDeclaration = previousDonorDeclaration;
                        }
                    }
                }
                finally
                {
                    _session.TypeParameterSubstitutions = previousSubstitutions;
                }
            }
        }
        if (!isInterface && IsStructurallyImplementedClass(declaration) &&
            !declaration.Members.Any(member => member.Kind == "constructor" && (member.Element.Parameters?.Length ?? 0) == 0))
        {
            var structuralBaseCall = string.Empty;
            if (declaration.Element.Supertype is { } structuralSupertype &&
                FindGlobalDeclaration(structuralSupertype) is { } structuralBase)
            {
                var baseConstructors = structuralBase.Members
                    .Where(member => member.Kind == "constructor")
                    .OrderBy(member => member.Offset)
                    .ToArray();
                var baseConstructor = PrimaryGenerativeConstructor(baseConstructors);
                var baseParameters = baseConstructor?.Element.Parameters ?? [];
                if (baseParameters.Any(parameter => parameter.Kind is "required-named" or "required-positional"))
                {
                    structuralBaseCall = $" : base({string.Join(", ", baseParameters.Select(_ => "default!"))})";
                }
            }
            builder.AppendLine($"    public {emittedName}(){structuralBaseCall} {{ }}");
            builder.AppendLine();
        }
        if (declaration.Members.Any(item => item.Kind == "field") ||
            (!isMixinDeclaration && (declaration.Element.Mixins?.Length ?? 0) > 0))
        {
            builder.AppendLine();
        }
        var constructors = declaration.Members.Where(item => item.Kind == "constructor").OrderBy(item => item.Offset).ToArray();
        if (!isInterface)
        {
            foreach (var constructor in constructors)
            {
                EmitConstructor(builder, declaration, constructor, package, library, inputPath, diagnostics);
            }
        }
        else
        {
            foreach (var constructor in constructors.Where(item => item.IsFactory))
            {
                EmitConstructor(builder, declaration, constructor, package, library, inputPath, diagnostics);
            }
        }
        var orderedMembers = declaration.Members.OrderBy(item => item.Offset).ToArray();
        if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration ||
            (!isInterface && declaration.Element.IsAbstract))
        {
            var appliedMixinMemberNames = ImplementationDonorDeclarations(declaration)
                .SelectMany(mixin => mixin.Members)
                .Select(item => item.Name)
                .ToHashSet(StringComparer.Ordinal);
            var contractBases = declaration.Ast.Kind == CoreNodeKind.MixinDeclaration
                ? DirectBaseNames(declaration)
                    .Concat(declaration.Name == "AutofillScopeMixin"
                        ? declaration.Element.Interfaces ?? []
                        : [])
                    .Select(MapType)
                    .Distinct(StringComparer.Ordinal)
                : bases;
            var emittedConstraintMemberNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (var constraint in contractBases
                .Select(FindGlobalDeclaration)
                .Where(candidate => candidate is not null &&
                    (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration || WillEmitAsInterface(candidate)))
                .Cast<CoreResolvedDeclaration>()
                .SelectMany(InterfaceContractDeclarations)
                .DistinctBy(candidate => candidate.Element.CanonicalId, StringComparer.Ordinal))
            {
                var constraintMembers = constraint.Members.OrderBy(item => item.Offset).ToArray();
                foreach (var requiredMember in constraintMembers.Where(item =>
                     item.Kind is "method" or "field" &&
                     !item.IsStatic &&
                     (!item.IsSetter || !constraintMembers.Any(candidate => candidate.IsGetter && candidate.Name == item.Name)) &&
                     emittedConstraintMemberNames.Add(item.Name) &&
                     !orderedMembers.Any(own => own.Name == item.Name) &&
                    !appliedMixinMemberNames.Contains(item.Name)))
                {
                    if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration)
                    {
                        if (WillEmitAsInterface(declaration))
                        {
                            // The concrete CLR application carries the `on` class.
                            // Do not widen that class's complete surface into the
                            // public interface; required cross-mixin members are
                            // emitted explicitly at the owning milestone boundary.
                            continue;
                        }
                        // A class-lowered mixin already inherits every member of a
                        // concrete `on` constraint. Re-declaring those members as
                        // bodyless contracts is invalid C# and also hides the base
                        // implementation (ServicesBinding/SchedulerBinding is the
                        // representative Flutter binding-chain case).
                        if (!WillEmitAsInterface(declaration) &&
                            !WillEmitAsInterface(constraint))
                        {
                            continue;
                        }
                        EmitMixinConstraintMember(builder, declaration, requiredMember);
                    }
                    else
                    {
                        EmitVirtualContractStub(builder, declaration, requiredMember);
                    }
                }
            }
        }
        if (!isInterface && declaration.Name == "SelectionRegistrant" &&
            !orderedMembers.Any(member => member.Name == "value"))
        {
            builder.AppendLine("    public abstract SelectionGeometry value { get; }");
        }
        var emittedWidgetDiagnosticsNode = false;
        foreach (var method in orderedMembers.Where(item => item.Kind == "method"))
        {
            if (declaration.Name == "Widget" && method.Name == "toDiagnosticsNode")
            {
                if (emittedWidgetDiagnosticsNode)
                {
                    continue;
                }
                emittedWidgetDiagnosticsNode = true;
            }
            EmitClassMethod(builder, declaration, method, orderedMembers, isInterface, package, library, inputPath, diagnostics);
            EmitTypedCallbackInvariantOverload(builder, declaration, method, isInterface);
        }
        if (!isInterface && declaration.Name is "RouteInformationProvider" or "SelectionContainerDelegate")
        {
            builder.AppendLine("    private readonly HashSet<global::System.Action> __listeners = new();");
            builder.AppendLine("    public virtual bool hasListeners => __listeners.Count != 0;");
            builder.AppendLine("    public virtual void addListener(global::System.Action listener) => __listeners.Add(listener);");
            builder.AppendLine("    public virtual void removeListener(global::System.Action listener) => __listeners.Remove(listener);");
            builder.AppendLine("    public virtual void notifyListeners() { foreach (var listener in __listeners.ToArray()) listener(); }");
            builder.AppendLine("    public virtual void dispose() => __listeners.Clear();");
        }
        if (!isInterface && declaration.Name == "ScrollPosition")
        {
            builder.AppendLine("    public abstract AxisDirection axisDirection { get; }");
        }
        if (declaration.Name == "_WidgetStateCombo")
        {
            builder.AppendLine(isInterface
                ? "    public bool isSatisfiedBy(HashSet<WidgetState> states);"
                : "    public virtual bool isSatisfiedBy(HashSet<WidgetState> states) => throw new NotSupportedException();");
        }
        if (!isInterface && declaration.Name == "State" && declaration.Element.TypeParameters is { Length: 1 })
        {
            var stateType = SafeIdentifier(declaration.Element.TypeParameters[0].Name);
            builder.AppendLine($"    StatefulWidget? IState._widget {{ get => _widget; set => _widget = ({stateType}?)value; }}");
            builder.AppendLine("    _StateLifecycle__framework IState._debugLifecycleState { get => _debugLifecycleState; set => _debugLifecycleState = value; }");
            builder.AppendLine("    StatefulElement? IState._element { get => _element; set => _element = value; }");
            builder.AppendLine("    StatefulWidget IState.widget => widget;");
            builder.AppendLine($"    void IState.didUpdateWidget(StatefulWidget oldWidget) => didUpdateWidget(({stateType})oldWidget);");
            builder.AppendLine("    public virtual void didChangeAppLifecycleState(AppLifecycleState state) { }");
            builder.AppendLine("    public virtual void didChangeAccessibilityFeatures() { }");
            builder.AppendLine();
        }
        if (isInterface && string.Equals(declaration.Name, "PaintingBinding", StringComparison.Ordinal) &&
            !orderedMembers.Any(member => member.Name == "platformDispatcher"))
        {
            // The G4-5 graph consumes BindingBase from the reviewed Foundation
            // project instead of regenerating it. Preserve the Dart mixin's
            // `on BindingBase` requirement as an explicit CLR interface member.
            builder.AppendLine("    PlatformDispatcher platformDispatcher { get; }");
        }
        if (isInterface && string.Equals(declaration.Name, "RendererBinding", StringComparison.Ordinal))
        {
            // RendererBinding's Dart `on SemanticsBinding` contract is consumed
            // by _BindingPipelineManifold. The concrete application supplies it;
            // retain only the members the mixin body actually calls.
            builder.AppendLine("    void ensureVisualUpdate();");
            builder.AppendLine("    bool semanticsEnabled { get; }");
            builder.AppendLine("    void removeSemanticsEnabledListener(Action listener);");
        }
        if (isInterface && string.Equals(declaration.Name, "WidgetsBinding", StringComparison.Ordinal))
        {
            // WidgetsBinding is a Dart mixin constrained by the complete binding
            // chain. CLR interfaces cannot inherit that class chain, so retain the
            // public capabilities used through a WidgetsBinding-typed value.
            builder.AppendLine("    PlatformDispatcher platformDispatcher { get; }");
            builder.AppendLine("    AppLifecycleState? lifecycleState { get; }");
            builder.AppendLine("    bool debugCheckZone(string entryPoint);");
            builder.AppendLine("    void addPostFrameCallback(global::System.Action<Duration> callback, string debugLabel = \"callback\");");
            builder.AppendLine("    void scheduleWarmUpFrame();");
            builder.AppendLine("    IEnumerable<global::Doroti.Generated.Framework.Rendering.RenderView> renderViews { get; }");
            builder.AppendLine("    void hitTestInView(global::Doroti.Generated.Framework.Gestures.HitTestResult result, Offset position, long viewId);");
            builder.AppendLine("    FlutterView window => platformDispatcher.implicitView ?? throw new InvalidOperationException(\"WidgetsBinding.window requires exactly one Flutter view.\");");
            builder.AppendLine("    Future endOfFrame => global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.endOfFrame;");
            builder.AppendLine("    void cancelPointer(long pointer) => global::Doroti.Generated.Framework.Gestures.GestureBinding.instance.cancelPointer(pointer);");
        }
        if (!isInterface)
        {
            var emittedMethodSignatures = orderedMembers
                .Where(item => item.Kind == "method")
                .Select(MethodSignatureKey)
                .ToHashSet(StringComparer.Ordinal);
            foreach (var mixinDeclaration in ImplementationDonorDeclarations(declaration)
                .Where(mixin => !HasPromotedClassRepresentation(mixin)))
            {
                var previousSubstitutions = _session.TypeParameterSubstitutions;
                _session.TypeParameterSubstitutions = TypeParameterSubstitutions(declaration, mixinDeclaration);
                try
                {
                    var mixinMembers = mixinDeclaration.Members.OrderBy(item => item.Offset).ToArray();
                    foreach (var method in mixinMembers.Where(item =>
                                 item.Kind == "method" &&
                                 !item.IsStatic &&
                                 !item.IsAbstract &&
                                 !orderedMembers.Any(own => own.Name == item.Name) &&
                                 emittedMethodSignatures.Add(MethodSignatureKey(item))))
                    {
                        var previousSourceLibrary = _session.ActiveSourceLibrary;
                        var previousDonorDeclaration = _session.ActiveDonorDeclaration;
                        _session.ActiveSourceLibrary = LibraryUriFromElementId(mixinDeclaration.Element.CanonicalId);
                        _session.ActiveDonorDeclaration = mixinDeclaration;
                        try
                        {
                            EmitClassMethod(builder, declaration, method, mixinMembers, false, package, library, inputPath, diagnostics);
                        }
                        finally
                        {
                            _session.ActiveSourceLibrary = previousSourceLibrary;
                            _session.ActiveDonorDeclaration = previousDonorDeclaration;
                        }
                    }
                }
                finally
                {
                    _session.TypeParameterSubstitutions = previousSubstitutions;
                }
            }
        }
        if (!isInterface && declaration.Name == "RenderingFlutterBinding")
        {
            builder.AppendLine("    public void handleMetricsChanged(FlutterView _) => handleMetricsChanged();");
        }
        if (!isInterface && declaration.Name == "MultiChildRenderObjectWidget")
        {
            builder.AppendLine("    protected MultiChildRenderObjectWidget(global::Doroti.Generated.Framework.Foundation.Key? key = null, IEnumerable<Widget> children = default!) : this(key, children.ToList()) { }");
        }
        if (!isInterface && declaration.Name == "InheritedWidget")
        {
            // Several private Flutter scopes use the positional `super(child)`
            // shorthand. The canonical generated constructor keeps the named
            // key/child API, while this forwarding overload preserves that Dart
            // positional base-initializer form.
            builder.AppendLine("    protected InheritedWidget(Widget child) : this(null, child) { }");
        }
        if (!isInterface && declaration.Name == "GlobalKey")
        {
            builder.AppendLine("    public GlobalKey(string? debugLabel) { _ = debugLabel; }");
        }
        if (!isInterface && declaration.Name == "Velocity")
        {
            builder.AppendLine("    public static Velocity operator -(Velocity value) => value.op_Subtract();");
        }
        if (!isInterface && declaration.Name == "_ReorderableItemGlobalKey")
        {
            builder.AppendLine("    internal static _ReorderableItemGlobalKey__reorderable_list Create(global::Doroti.Generated.Framework.Foundation.Key key, long index, SliverReorderableListState state) => new(key, index, state);");
        }
        if (!isInterface && bases.Any(b => b == "IEnumerable" || b.StartsWith("IEnumerable<", StringComparison.Ordinal)))
        {
            builder.AppendLine("    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();");
        }
        if (!isInterface && bases.FirstOrDefault(b => b.StartsWith("IEnumerator<", StringComparison.Ordinal)) is { } iteratorBase)
        {
            var iteratorType = iteratorBase["IEnumerator<".Length..^1];
            builder.AppendLine($"    {iteratorType} IEnumerator<{iteratorType}>.Current => current;");
            builder.AppendLine("    object System.Collections.IEnumerator.Current => current!;");
            builder.AppendLine("    bool System.Collections.IEnumerator.MoveNext() => moveNext();");
            builder.AppendLine("    void System.Collections.IEnumerator.Reset() => throw new NotSupportedException();");
            builder.AppendLine("    void IDisposable.Dispose() { }");
        }
        if (!isInterface && bases.FirstOrDefault(item => item.StartsWith("IComparable<", StringComparison.Ordinal)) is { } comparableBase &&
            orderedMembers.Any(member => member.Name == "compareTo"))
        {
            var comparableType = comparableBase["IComparable<".Length..^1];
            builder.AppendLine($"    public int CompareTo({comparableType}? other) => checked((int)compareTo(other!));");
        }
        if (!isInterface && declaration.Name == "PointerEvent" &&
            !orderedMembers.Any(member => member.Name == "toDiagnosticsNode"))
        {
            // Diagnosticable is a CLR interface with default implementations.
            // Pointer-event subclasses call these Dart-inherited members as
            // concrete methods, so expose the bridge once on the family root.
            builder.AppendLine("    public DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) =>");
            builder.AppendLine("        ((Diagnosticable)this).toDiagnosticsNode(name, style);");
            builder.AppendLine("    public virtual string toStringShort() => ((Diagnosticable)this).toStringShort();");
        }
        if (!isInterface && declaration.Name is "PointerEnterEvent" or "PointerExitEvent")
        {
            var uiEventType = $"global::Doroti.Flutter.Ui.{declaration.Name}";
            builder.AppendLine($"    public static implicit operator {uiEventType}({declaration.Name} value) => new()");
            builder.AppendLine("    {");
            builder.AppendLine("        pointer = value.pointer,");
            builder.AppendLine("        embedderId = value.embedderId,");
            builder.AppendLine("        platformData = value.platformData,");
            builder.AppendLine("        timeStamp = value.timeStamp,");
            builder.AppendLine("        position = value.position,");
            builder.AppendLine("        kind = value.kind,");
            builder.AppendLine("        orientation = value.orientation,");
            builder.AppendLine("        pressure = value.pressure,");
            builder.AppendLine("        size = value.size,");
            builder.AppendLine("        radiusMajor = value.radiusMajor,");
            builder.AppendLine("        radiusMinor = value.radiusMinor,");
            builder.AppendLine("    };");
        }
        if (!isInterface && declaration.Name == "HitTestResult")
        {
            builder.AppendLine("    protected HitTestResult(HitTestResult result)");
            builder.AppendLine("    {");
            builder.AppendLine("        ArgumentNullException.ThrowIfNull(result);");
            builder.AppendLine("        _path = result._path;");
            builder.AppendLine("        _transforms = result._transforms;");
            builder.AppendLine("        _localTransforms = result._localTransforms;");
            builder.AppendLine("    }");
            builder.AppendLine("    public virtual void add<T>(HitTestEntry<T> entry) where T : HitTestTarget");
            builder.AppendLine("    {");
            builder.AppendLine("        DartRuntimePrimitives.Assert(() => entry._transform is null);");
            builder.AppendLine("        var compatibleEntry = new HitTestEntry<HitTestTarget>(entry.target)");
            builder.AppendLine("        {");
            builder.AppendLine("            _transform = _lastTransform,");
            builder.AppendLine("        };");
            builder.AppendLine("        entry._transform = compatibleEntry._transform;");
            builder.AppendLine("        _path.Add(compatibleEntry);");
            builder.AppendLine("    }");
        }
        if (!isInterface && bases.Any(item => StripLibraryPrefix(item).Split('<')[0] == "DiagnosticableTree") &&
            !orderedMembers.Any(member => member.Name == "toStringDeep"))
        {
            builder.AppendLine("    public virtual string toStringDeep(string prefixLineOne = \"\", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>");
            builder.AppendLine("        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);");
        }
        if (!isInterface && isPlatformNetworkImage)
        {
            builder.AppendLine("    ImageStreamCompleter NetworkImage.loadBuffer(NetworkImage key, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode) =>");
            builder.AppendLine($"        loadBuffer(({emittedName})key, decode);");
            builder.AppendLine("    ImageStreamCompleter NetworkImage.loadImage(NetworkImage key, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode) =>");
            builder.AppendLine($"        loadImage(({emittedName})key, decode);");
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private void EmitConstructor(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember constructor,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var parameters = constructor.Element.Parameters ?? [];
        var visibility = ConstructorVisibility(declaration, constructor);

        var constructors = declaration.Members.Where(item => item.Kind == "constructor").OrderBy(item => item.Offset).ToArray();
        var primaryConstructor = PrimaryGenerativeConstructor(constructors);
        var primaryHasRedirectSignatureCollision = constructor == primaryConstructor &&
            constructors.Any(candidate => candidate != primaryConstructor &&
                !candidate.IsFactory && candidate.Name == "new" &&
                DescendantsAndSelf(candidate.Ast).Any(node => node.Kind == CoreNodeKind.RedirectingConstructorInvocation) &&
                SameMappedParameterTypes(candidate.Element.Parameters ?? [], primaryConstructor?.Element.Parameters ?? []));
        var generativeRedirect = !constructor.IsFactory && constructor.Name == "new"
            ? DescendantsAndSelf(constructor.Ast)
                .FirstOrDefault(item => item.Kind == CoreNodeKind.RedirectingConstructorInvocation)
            : null;
        if (generativeRedirect is not null && constructor != primaryConstructor &&
            primaryConstructor is { IsFactory: false })
        {
            // An unnamed generative redirect remains a real CLR constructor:
            // derived classes bind their super-formals to this signature. A
            // static Create method loses that constructor contract (notably for
            // _SemanticsBase) and cannot participate in base initialization.
            var redirectedArguments = new CsSyntaxBuilder();
            var primaryParameters = primaryConstructor.Element.Parameters ?? [];
            EmitArguments(
                redirectedArguments,
                generativeRedirect.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList),
                declaration,
                package,
                library,
                inputPath,
                diagnostics,
                expectedParameters: primaryParameters,
                expectedArgumentTypes: primaryParameters.Select(item => MapType(item.Type)).ToArray());
            builder.Append("    ").Append(visibility).Append(' ')
                .Append(EmittedTypeName(library, declaration.Name)).Append('(')
                .Append(string.Join(", ", MapParameters(parameters))).Append(") : this(")
                .Append(redirectedArguments.RenderFragment());
            if (SameMappedParameterTypes(parameters, primaryParameters))
            {
                if (parameters.Length > 0) builder.Append(", ");
                builder.Append("__dorotiPrimary: true");
            }
            builder.AppendLine(")");
            builder.AppendLine("    {");
            builder.AppendLine("    }");
            builder.AppendLine();
            return;
        }
        // Multiple generative constructors: emit non-primary ones as static factories.
        // Prefer the non-redirecting constructor as the primary so final fields are
        // initialized by the constructor that actually declares them.
        // Skip this when the primary is itself a factory
        // (e.g. _CriticalSolution factory + withArgs generative) — emit a real ctor.
        if (!constructor.IsFactory && constructors.Length > 1 && constructor != primaryConstructor &&
            primaryConstructor is { IsFactory: false })
        {
            var namedClassName = EmittedTypeName(library, declaration.Name) + FormatTypeParameters(declaration.Element.TypeParameters);
            var methodName = constructor.Name == "new" ? "Create" : NamedConstructorMethodName(constructor.Name);
            if (constructor.Name == "wrap" && parameters is [{ IsSuperFormal: true } wrapParameter])
            {
                builder.AppendLine($"    private {namedClassName}({string.Join(", ", MapParameters(parameters))}) : base({SafeIdentifier(wrapParameter.Name)})");
                builder.AppendLine("    {");
                builder.AppendLine("    }");
                builder.AppendLine($"    public static {namedClassName} CreateWrap({string.Join(", ", MapParameters(parameters))}) => new {namedClassName}({SafeIdentifier(wrapParameter.Name)});");
                builder.AppendLine();
                return;
            }
            builder.AppendLine($"    {visibility} static {namedClassName} {methodName}({string.Join(", ", MapParameters(parameters))})");
            builder.AppendLine("    {");
            if (declaration.Element.IsAbstract)
            {
                builder.AppendLine("        throw new InvalidOperationException(\"Dart abstract constructors cannot be invoked directly.\");");
                builder.AppendLine("    }");
                builder.AppendLine();
                return;
            }
            var redirect = DescendantsAndSelf(constructor.Ast)
                .FirstOrDefault(item => item.Kind == CoreNodeKind.RedirectingConstructorInvocation);
            if (redirect is not null)
            {
                var targetName = redirect.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier)
                    ?.Text(CoreProperty.name) ?? primaryConstructor.Name;
                var targetCall = string.Equals(targetName, primaryConstructor.Name, StringComparison.Ordinal)
                    ? $"new {namedClassName}"
                    : $"{namedClassName}.{NamedConstructorMethodName(targetName)}";
                builder.Append("        return ").Append(targetCall).Append('(');
                var redirectedPrimaryParameters = primaryConstructor.Element.Parameters ?? [];
                EmitArguments(
                    builder,
                    redirect.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList),
                    declaration,
                    package,
                    library,
                    inputPath,
                    diagnostics,
                    expectedParameters: redirectedPrimaryParameters,
                    expectedArgumentTypes: redirectedPrimaryParameters.Select(item => MapType(item.Type)).ToArray(),
                    invocationName: namedClassName,
                    nullAsGenericDefault: ContainsUnboundTypeParameter(namedClassName));
                builder.AppendLine(");");
                builder.AppendLine("    }");
                builder.AppendLine();
                return;
            }
            var primaryParameters = primaryConstructor.Element.Parameters ?? [];
            // A named generative constructor still executes the unnamed
            // constructor contract before applying its own field initializers.
            // Forward parameters shared by name and let the primary C#
            // constructor supply its declared defaults for everything else.
            // Passing a positional `default!` for every primary parameter made
            // nullable Dart defaults hit `RequireValue` before the named
            // constructor could install its delegate (ListView.builder was the
            // first live Material consumer to expose this).
            var availableParameterNames = parameters
                .Select(item => item.Name)
                .ToHashSet(StringComparer.Ordinal);
            var forwardedPrimaryArguments = primaryParameters
                .Where(item => availableParameterNames.Contains(item.Name) ||
                    NeedsRuntimeDefaultRestore(item) ||
                    item.Kind is not "optional-named" and not "optional-positional")
                .Select(item => availableParameterNames.Contains(item.Name)
                    ? $"{SafeIdentifier(item.Name)}: {SafeIdentifier(item.Name)}"
                    : NeedsRuntimeDefaultRestore(item)
                        ? $"{SafeIdentifier(item.Name)}: {MapParameterRuntimeDefault(item, library)}"
                        : $"{SafeIdentifier(item.Name)}: default!");
            builder.AppendLine($"        var __instance = new {namedClassName}({string.Join(", ", forwardedPrimaryArguments)});");
            var restoredValueParameters = parameters.Where(NeedsRuntimeDefaultRestore).ToArray();
            foreach (var parameter in restoredValueParameters)
            {
                var mappedParameterType = MapType(parameter.Type);
                var mappedDefault = MapParameterRuntimeDefault(parameter, library);
                builder.AppendLine(
                    $"        {mappedParameterType} {SyntheticIdentifier(parameter.Name)} = {SafeIdentifier(parameter.Name)} ?? {mappedDefault};");
            }
            foreach (var parameter in parameters.Where(item => item.IsInitializingFormal))
            {
                var parameterField = declaration.Members.FirstOrDefault(member => member.Kind == "field" && member.Name == parameter.Name);
                var targetName = parameterField is { IsFinal: true } && HasOverridableBaseMember(declaration, parameterField)
                    ? "__field_" + SafeIdentifier(parameter.Name).TrimStart('@')
                    : SafeIdentifier(parameter.Name);
                var sourceName = restoredValueParameters.Contains(parameter)
                    ? SyntheticIdentifier(parameter.Name)
                    : SafeIdentifier(parameter.Name);
                builder.AppendLine($"        __instance.{targetName} = {sourceName};");
            }
            foreach (var initializer in DescendantsAndSelf(constructor.Ast).Where(item => item.Kind == CoreNodeKind.ConstructorFieldInitializer))
            {
                var fieldName = initializer.Text(CoreProperty.fieldName);
                var value = initializer.Child(CoreChildRole.expressionOffset);
                if (fieldName is null || value is null)
                {
                    continue;
                }
                var initializedField = declaration.Members.FirstOrDefault(member => member.Name == fieldName);
                var initializedTarget = initializedField is { IsFinal: true } && HasOverridableBaseMember(declaration, initializedField)
                    ? "__field_" + SafeIdentifier(fieldName).TrimStart('@')
                    : SafeIdentifier(fieldName);
                builder.Append($"        __instance.{initializedTarget} = ");
                var restoredParameter = value.Kind == CoreNodeKind.SimpleIdentifier
                    ? restoredValueParameters.FirstOrDefault(parameter =>
                        string.Equals(parameter.Name, value.Text(CoreProperty.name), StringComparison.Ordinal))
                    : null;
                if (restoredParameter is not null)
                {
                    builder.Append(SyntheticIdentifier(restoredParameter.Name));
                }
                else
                {
                    LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
                    if (fieldName == "displayFeatures" &&
                        DescendantsAndSelf(value).Any(item => item.Text(CoreProperty.name) == "displayFeatures"))
                    {
                        builder.Append(".ToList()");
                    }
                }
                builder.AppendLine(";");
            }
            var constructorBody = constructor.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody) is { } bodyNode
                ? bodyNode.Child(CoreChildRole.blockOffset)
                : null;
            if (constructorBody is not null)
            {
                var previousThis = _session.ExplicitThisExpression;
                _session.ExplicitThisExpression = "__instance";
                try
                {
                    var emittedBody = new CsSyntaxBuilder();
                    EmitBlockBody(emittedBody, constructorBody, declaration, package, library, inputPath, diagnostics, 2);
                    var bodySyntax = emittedBody.Build();
                    foreach (var parameter in restoredValueParameters)
                    {
                        bodySyntax = bodySyntax.RenameIdentifier(SafeIdentifier(parameter.Name), SyntheticIdentifier(parameter.Name));
                    }
                    builder.Append(bodySyntax);
                }
                finally
                {
                    _session.ExplicitThisExpression = previousThis;
                }
            }
            builder.AppendLine("        return __instance;");
            builder.AppendLine("    }");
            builder.AppendLine();
            return;
        }

        if (constructor.IsFactory)
        {
            var methodName = constructor.Name == "new" ? "Create" : NamedConstructorMethodName(constructor.Name);
            var returnType = MapType(constructor.Element.ReturnType ?? declaration.Name);
            var expressionBody = constructor.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
            var blockBody = constructor.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
            var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
            var block = blockBody is null ? null : blockBody.Child(CoreChildRole.blockOffset);
            if (expression is not null)
            {
                builder.Append($"    {visibility} static {returnType} {methodName}({string.Join(", ", MapParameters(parameters))}) => ");
                var expressionType = MapType(expression.StaticType ?? string.Empty);
                var needsCheckedCast = returnType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                    expressionType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                    !IsValueType(returnType.TrimEnd('?')) &&
                    !IsValueType(expressionType.TrimEnd('?')) &&
                    !string.Equals(returnType.TrimEnd('?'), expressionType.TrimEnd('?'), StringComparison.Ordinal);
                if (needsCheckedCast) builder.Append("((").Append(returnType).Append(")(object?)");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                if (needsCheckedCast) builder.Append(')');
                builder.AppendLine(";");
                builder.AppendLine();
                return;
            }
            if (block is not null)
            {
                builder.AppendLine($"    {visibility} static {returnType} {methodName}({string.Join(", ", MapParameters(parameters))})");
                builder.AppendLine("    {");
                var restoredValueParameters = parameters.Where(NeedsRuntimeDefaultRestore).ToArray();
                foreach (var parameter in restoredValueParameters)
                {
                    builder.AppendLine(
                        $"        {MapType(parameter.Type)} {SyntheticIdentifier(parameter.Name)} = {SafeIdentifier(parameter.Name)} ?? {MapParameterRuntimeDefault(parameter, library)};");
                }
                var emittedBody = new CsSyntaxBuilder();
                EmitBlockBody(emittedBody, block, declaration, package, library, inputPath, diagnostics, 2);
                var bodySyntax = emittedBody.Build();
                foreach (var parameter in restoredValueParameters)
                {
                    bodySyntax = bodySyntax.RenameIdentifier(SafeIdentifier(parameter.Name), SyntheticIdentifier(parameter.Name));
                }
                builder.Append(bodySyntax);
                builder.AppendLine("    }");
                builder.AppendLine();
                return;
            }
            var redirectName = DescendantsAndSelf(constructor.Ast).FirstOrDefault(item => item.Kind == CoreNodeKind.ConstructorName);
            if (redirectName is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, constructor.Ast,
                    "factory-redirect-target", "Add typed factory redirect target resolution.");
                return;
            }
            var targetType = MapRedirectTargetType(redirectName, declaration);
            var redirectConstructor = redirectName.Children
                .FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier)
                ?.Text(CoreProperty.name);
            var targetCall = !string.IsNullOrEmpty(redirectConstructor) &&
                TryResolveEmittedNamedConstructor(targetType, redirectConstructor, out var redirectFactory)
                    ? $"{targetType}.{redirectFactory}"
                    : $"new {targetType}";
            builder.AppendLine($"    {visibility} static {returnType} {methodName}({string.Join(", ", MapParameters(parameters))})");
            var redirectExpression = $"{targetCall}({string.Join(", ", parameters.Select(item => SafeIdentifier(item.Name)))})";
            var redirectRequiresCheckedCast = returnType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                !string.Equals(returnType.TrimEnd('?'), targetType.TrimEnd('?'), StringComparison.Ordinal);
            builder.AppendLine(redirectRequiresCheckedCast
                ? $"        => (({returnType})(object?){redirectExpression});"
                : $"        => {redirectExpression};");
            builder.AppendLine();
            return;
        }

        var emittedClassName = EmittedTypeName(library, declaration.Name);
        var baseCall = BuildBaseInitializer(constructor.Ast, parameters, declaration, package, library, inputPath, diagnostics);
        var mappedConstructorParameters = string.Join(", ", MapParameters(parameters));
        if (primaryHasRedirectSignatureCollision)
        {
            if (mappedConstructorParameters.Length > 0) mappedConstructorParameters += ", ";
            mappedConstructorParameters += "bool __dorotiPrimary";
        }
        builder.AppendLine($"    {visibility} {emittedClassName}({mappedConstructorParameters}){baseCall}");
        builder.AppendLine("    {");
        var restoredConstructorParameters = parameters
            .Where(parameter => NeedsRuntimeDefaultRestore(parameter) && !parameter.IsSuperFormal)
            .ToArray();
        foreach (var parameter in restoredConstructorParameters)
        {
            builder.AppendLine(
                $"        {MapType(parameter.Type)} {SyntheticIdentifier(parameter.Name)} = {SafeIdentifier(parameter.Name)} ?? {MapParameterRuntimeDefault(parameter, library)};");
        }
        if (declaration.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration &&
            DescendantsAndSelf(constructor.Ast).FirstOrDefault(item => item.Kind == CoreNodeKind.RedirectingConstructorInvocation) is { } extensionRedirect &&
            declaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.RepresentationDeclaration) is { } representation &&
            extensionRedirect.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList)?
                .Children.FirstOrDefault(item => item.Category == "expression") is { } representationValue)
        {
            builder.Append("        this.").Append(SafeIdentifier(representation.Text(CoreProperty.name) ?? "value")).Append(" = ");
            LowerExpression(builder, representationValue, declaration, package, library, inputPath, diagnostics);
            builder.AppendLine(";");
        }
        foreach (var parameter in parameters.Where(item => item.IsInitializingFormal))
        {
            var mappedParameterType = MapType(parameter.Type);
            var parameterField = declaration.Members.FirstOrDefault(member => member.Kind == "field" && member.Name == parameter.Name);
            var targetName = parameterField is { IsFinal: true } && HasOverridableBaseMember(declaration, parameterField)
                ? "__field_" + SafeIdentifier(parameter.Name).TrimStart('@')
                : SafeIdentifier(parameter.Name);
            if (!IsCompileTimeConstantDefault(parameter.DefaultValue, parameter.Type) &&
                (NeedsNonConstValueDefault(parameter) || !IsValueType(mappedParameterType) || mappedParameterType.EndsWith("?", StringComparison.Ordinal)) &&
                !string.IsNullOrEmpty(parameter.DefaultValue))
            {
                var restoredName = NeedsRuntimeDefaultRestore(parameter)
                    ? SyntheticIdentifier(parameter.Name)
                    : $"{SafeIdentifier(parameter.Name)} ?? {MapParameterRuntimeDefault(parameter, library)}";
                builder.AppendLine($"        this.{targetName} = {restoredName};");
            }
            else
            {
                builder.AppendLine($"        this.{targetName} = {SafeIdentifier(parameter.Name)};");
            }
        }
        foreach (var initializer in DescendantsAndSelf(constructor.Ast).Where(item => item.Kind == CoreNodeKind.ConstructorFieldInitializer))
        {
            var fieldName = initializer.Text(CoreProperty.fieldName);
            var expression = initializer.Child(CoreChildRole.expressionOffset);
            if (string.IsNullOrEmpty(fieldName) || expression is null)
            {
                continue;
            }
            var initializedField = declaration.Members.FirstOrDefault(member => member.Kind == "field" && member.Name == fieldName);
            var initializedTarget = initializedField is { IsFinal: true } && HasOverridableBaseMember(declaration, initializedField)
                ? "__field_" + SafeIdentifier(fieldName).TrimStart('@')
                : SafeIdentifier(fieldName);
            var fieldType = MapType(declaration.Members.FirstOrDefault(member => member.Name == fieldName)?.Element.Type ?? string.Empty);
            var expressionType = MapType(expression.StaticType ?? string.Empty);
            var defaultedParameter = expression.Kind == CoreNodeKind.SimpleIdentifier
                ? parameters.FirstOrDefault(parameter =>
                    string.Equals(parameter.Name, expression.Text(CoreProperty.name), StringComparison.Ordinal) &&
                    NeedsRuntimeDefaultRestore(parameter))
                : null;
            var requiresValue = IsValueType(fieldType) && !fieldType.EndsWith("?", StringComparison.Ordinal) &&
                expressionType == fieldType + "?" && defaultedParameter is null;
            builder.Append($"        this.{initializedTarget} = ");
            if (defaultedParameter is not null)
            {
                builder.Append(SyntheticIdentifier(defaultedParameter.Name));
            }
            else
            {
                if (requiresValue) builder.Append("DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                if (requiresValue) builder.Append(')');
            }
            if (fieldType.StartsWith("DartMap<", StringComparison.Ordinal) &&
                expressionType.StartsWith("DartMap<", StringComparison.Ordinal) && fieldType != expressionType)
            {
                builder.Append(".cast<").Append(DartMapTypeArguments(fieldType)).Append(">()");
            }
            else if (fieldType.StartsWith("List<", StringComparison.Ordinal) &&
                IsDartTypedDataList(expression.StaticType))
            {
                builder.Append(".ToList()");
            }
            builder.AppendLine(";");
        }
        foreach (var assertInitializer in DescendantsAndSelf(constructor.Ast).Where(item => item.Kind == CoreNodeKind.AssertInitializer))
        {
            var condition = assertInitializer.Child(CoreChildRole.conditionOffset);
            if (condition is null)
            {
                continue;
            }
            var emittedCondition = new CsSyntaxBuilder();
            LowerExpression(emittedCondition, condition, declaration, package, library, inputPath, diagnostics);
            var conditionSyntax = emittedCondition.Build();
            foreach (var parameter in restoredConstructorParameters)
            {
                conditionSyntax = conditionSyntax.RenameIdentifier(
                    SafeIdentifier(parameter.Name),
                    SyntheticIdentifier(parameter.Name));
            }
            builder.Append("        System.Diagnostics.Debug.Assert(").Append(conditionSyntax).AppendLine(");");
        }
        var generativeConstructorBody = constructor.Ast.Children
            .FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody)?
            .Child(CoreChildRole.blockOffset);
        if (generativeConstructorBody is not null)
        {
            var emittedBody = new CsSyntaxBuilder();
            EmitBlockBody(
                emittedBody,
                generativeConstructorBody,
                declaration,
                package,
                library,
                inputPath,
                diagnostics,
                2);
            var bodySyntax = emittedBody.Build();
            foreach (var parameter in restoredConstructorParameters)
            {
                bodySyntax = bodySyntax.RenameIdentifier(
                    SafeIdentifier(parameter.Name),
                    SyntheticIdentifier(parameter.Name));
            }
            builder.Append(bodySyntax);
        }
        builder.AppendLine("    }");
        builder.AppendLine();
    }

    private bool SameMappedParameterTypes(CoreResolvedParameter[] left, CoreResolvedParameter[] right) =>
        left.Length == right.Length && left.Select(item => MapType(item.Type))
            .SequenceEqual(right.Select(item => MapType(item.Type)), StringComparer.Ordinal);

    private string ConstructorVisibility(CoreResolvedDeclaration declaration, CoreResolvedMember constructor)
    {
        if (IsDartPrivate(declaration))
        {
            return "internal";
        }
        if ((constructor.Element.Parameters ?? []).Any(parameter =>
                FindGlobalDeclaration(MapType(parameter.Type)) is { } parameterType &&
                IsDartPrivate(parameterType)))
        {
            // A public CLR constructor cannot expose a Dart library-private
            // implementation type. Public redirecting/factory entrypoints can
            // still call this reviewed generative constructor from the same
            // assembly, matching Dart's library visibility.
            return "internal";
        }
        if (declaration.Element.IsAbstract && !constructor.IsFactory)
        {
            if (declaration.Name == "GlobalKey") return "public";
            return "protected";
        }
        if (IsDartPrivate(constructor))
        {
            // A Dart named constructor's library-private selector has no CLR
            // constructor-name equivalent. Keep private declaring types
            // internal above, but preserve the reviewed callable constructor
            // surface for public concrete framework types.
            return "public";
        }
        return "public";
    }

    private void EmitTypedCallbackInvariantOverload(
        CsSyntaxBuilder builder,
        CoreResolvedDeclaration declaration,
        CoreResolvedMember method,
        bool isInterface)
    {
        if (isInterface || method.IsAbstract || method.IsStatic || IsDartPrivate(declaration) || IsDartPrivate(method))
        {
            return;
        }

        var typeParameters = method.Element.TypeParameters ?? [];
        var parameters = method.Element.Parameters ?? [];
        if (typeParameters.Length != 1 || parameters.Length == 0)
        {
            return;
        }

        var typeParameter = SafeIdentifier(typeParameters[0].Name);
        var returnType = MapType(method.Element.ReturnType ?? method.Element.Type ?? "object");
        var firstParameterType = MapType(parameters[0].Type);
        if (returnType != $"Future<{typeParameter}>" ||
            firstParameterType is not ("Func<object>" or "global::System.Func<object>"))
        {
            return;
        }

        var mappedParameters = MapParameters(parameters).ToArray();
        var callbackName = SafeIdentifier(parameters[0].Name);
        mappedParameters[0] = $"global::System.Func<{typeParameter}> {callbackName}";
        var arguments = new[] { $"() => (object?){callbackName}()!" }
            .Concat(parameters.Skip(1).Select(parameter => SafeIdentifier(parameter.Name)));
        builder.AppendLine(
            $"    public virtual {returnType} {MapMethodDeclarationName(method)}<{typeParameter}>({string.Join(", ", mappedParameters)}) =>");
        builder.AppendLine(
            $"        {MapMethodDeclarationName(method)}<{typeParameter}>({string.Join(", ", arguments)});");
        builder.AppendLine();
    }

    private string BuildBaseInitializer(
        CoreAstNode constructorAst,
        CoreResolvedParameter[] parameters,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var baseDeclaration = declaration.Element.Supertype is { } supertype
            ? FindGlobalDeclaration(supertype)
            : null;
        var baseLibrary = baseDeclaration is null
            ? library
            : LibraryUriFromElementId(baseDeclaration.Element.CanonicalId);
        var superInvocation = DescendantsAndSelf(constructorAst).FirstOrDefault(item => item.Kind == CoreNodeKind.SuperConstructorInvocation);
        if (superInvocation is not null)
        {
            var arguments = superInvocation.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ArgumentList);
            var argumentValues = arguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
            var namedArguments = argumentValues
                .Where(item => item.Kind == CoreNodeKind.NamedExpression)
                .Select(item => item.Text(CoreProperty.name))
                .Where(name => !string.IsNullOrEmpty(name))
                .ToHashSet(StringComparer.Ordinal);
            var expectedBaseConstructor = baseDeclaration?.Members
                .Where(member => member.Kind == "constructor" && !member.IsFactory)
                .OrderByDescending(member => (member.Element.Parameters ?? []).Length)
                .FirstOrDefault(member => namedArguments.All(name =>
                    (member.Element.Parameters ?? []).Any(parameter => parameter.Name == name)));
            var expectedBaseParameters = expectedBaseConstructor?.Element.Parameters;
            var baseTypeSubstitutions = baseDeclaration is null
                ? new Dictionary<string, string>(StringComparer.Ordinal)
                : TypeParameterSubstitutions(declaration, baseDeclaration);
            var expectedBaseArgumentTypes = expectedBaseParameters?.Select(parameter =>
            {
                var substituted = ApplyTypeParameterSubstitutions(parameter.Type, baseTypeSubstitutions);
                var mapped = MapType(substituted);
                return baseDeclaration is not null &&
                    IsTypeParameter(parameter.Type.TrimEnd('?'), baseDeclaration) &&
                    IsValueType(mapped.TrimEnd('?'))
                        ? mapped.TrimEnd('?')
                        : mapped;
            }).ToArray();
            var temp = new CsSyntaxBuilder();
            EmitArguments(
                temp,
                arguments,
                declaration,
                package,
                library,
                inputPath,
                diagnostics,
                expectedParameters: expectedBaseParameters,
                expectedArgumentTypes: expectedBaseArgumentTypes);
            if (temp.Length > 0)
            {
                var explicitArguments = temp.RenderFragment();
                foreach (var parameter in parameters.Where(parameter => MapType(parameter.Type) == "dynamic"))
                {
                    var name = SafeIdentifier(parameter.Name);
                    // A dynamic expression in a C# constructor initializer
                    // attempts dynamic constructor dispatch, which the CLR
                    // forbids. Dart `dynamic` parameters still cross this base
                    // boundary as ordinary object values.
                    explicitArguments = Regex.Replace(
                        explicitArguments,
                        $@"\b{Regex.Escape(name)}\b(?!\s*:)",
                        $"(object){name}",
                        RegexOptions.CultureInvariant);
                }
                foreach (var parameter in parameters.Where(NeedsRuntimeDefaultRestore))
                {
                    var name = SafeIdentifier(parameter.Name);
                    explicitArguments = explicitArguments.Replace(
                        $"{name}: {name}",
                        $"{name}: {name} ?? {MapParameterRuntimeDefault(parameter, library)}",
                        StringComparison.Ordinal);
                }
                var forwarded = parameters.Where(item => item.IsSuperFormal)
                    .Select(item => item.Kind.EndsWith("-named", StringComparison.Ordinal)
                        ? $"{SafeIdentifier(item.Name)}: {SuperFormalArgument(item, declaration, baseDeclaration, baseLibrary)}"
                        : SuperFormalArgument(item, declaration, baseDeclaration, baseLibrary))
                    .ToArray();
                return $" : base({string.Join(", ", forwarded.Concat([explicitArguments]))})";
            }
            var forwardedSuperParameters = parameters.Where(item => item.IsSuperFormal).ToArray();
            if (forwardedSuperParameters.Length > 0)
            {
                return $" : base({string.Join(", ", forwardedSuperParameters.Select(item => SuperFormalArgument(item, declaration, baseDeclaration, baseLibrary)))})";
            }
        }
        var superParameters = parameters.Where(item => item.IsSuperFormal).ToArray();
        if (superParameters.Length > 0)
        {
            return $" : base({string.Join(", ", superParameters.Select(item => item.Kind.EndsWith("-named", StringComparison.Ordinal)
                ? $"{SafeIdentifier(item.Name)}: {SuperFormalArgument(item, declaration, baseDeclaration, baseLibrary)}"
                : SuperFormalArgument(item, declaration, baseDeclaration, baseLibrary)))})";
        }
        // A Dart generative constructor implicitly invokes the matching unnamed
        // super constructor. Preserve that forwarding when the analyzer exposes
        // identical constructor contracts on both classes.
        var matchingBaseConstructor = baseDeclaration?.Members.FirstOrDefault(member =>
        {
            if (member.Kind != "constructor" || member.IsFactory) return false;
            var baseParameters = member.Element.Parameters ?? [];
            return baseParameters.Length == parameters.Length &&
                baseParameters.Zip(parameters).All(pair => pair.First.Name == pair.Second.Name);
        });
        if (matchingBaseConstructor is not null && parameters.Length > 0)
        {
            return $" : base({string.Join(", ", parameters.Select(BaseConstructorArgument))})";
        }
        return string.Empty;

        string BaseConstructorArgument(CoreResolvedParameter parameter)
        {
            var name = SafeIdentifier(parameter.Name);
            // Constructor initializers cannot contain dynamically dispatched
            // calls. Preserve Dart's dynamic value while forcing an ordinary
            // CLR base-constructor invocation.
            if (MapType(parameter.Type) == "dynamic") return $"(object?){name}";
            var baseParameter = matchingBaseConstructor?.Element.Parameters?
                .FirstOrDefault(candidate => candidate.Name == parameter.Name);
            if (baseDeclaration is not null && baseParameter is not null)
            {
                var substitutions = TypeParameterSubstitutions(declaration, baseDeclaration);
                var expected = MapType(ApplyTypeParameterSubstitutions(baseParameter.Type, substitutions));
                if (IsTypeParameter(baseParameter.Type.TrimEnd('?'), baseDeclaration) &&
                    IsValueType(expected.TrimEnd('?')) &&
                    MapType(parameter.Type) == expected.TrimEnd('?') + "?")
                {
                    return $"DartRuntimePrimitives.RequireValue({name})";
                }
            }
            return name;
        }
    }

    private string SuperFormalArgument(
        CoreResolvedParameter parameter,
        CoreResolvedDeclaration declaration,
        CoreResolvedDeclaration? baseDeclaration,
        string library)
    {
        var name = SafeIdentifier(parameter.Name);
        var mapped = MapType(parameter.Type);
        if (mapped == "dynamic")
        {
            // C# forbids dynamic dispatch in constructor initializers.
            return $"(object?){name}";
        }
        if (NeedsRuntimeDefaultRestore(parameter))
        {
            return $"{name} ?? {MapInheritedDefaultExpression(parameter.DefaultValue!, baseDeclaration, library)}";
        }
        if (parameter.Type.EndsWith("?", StringComparison.Ordinal))
        {
            // A nullable super-formal is a nullable base-constructor argument
            // in Dart. Do not let erased/generic contract lookup turn omission
            // into an eager null assertion (for example Stack.textDirection).
            return name;
        }
        var baseParameter = baseDeclaration?.Members
            .Where(member => member.Kind == "constructor" && !member.IsFactory)
            .SelectMany(member => member.Element.Parameters ?? [])
            .FirstOrDefault(candidate => candidate.Name == parameter.Name);
        if (baseDeclaration is not null && baseParameter is not null)
        {
            if (MapType(baseParameter.Type) == mapped) return name;
            var substitutions = TypeParameterSubstitutions(declaration, baseDeclaration);
            var expected = MapType(ApplyTypeParameterSubstitutions(baseParameter.Type, substitutions));
            if (IsTypeParameter(baseParameter.Type.TrimEnd('?'), baseDeclaration) &&
                IsValueType(expected.TrimEnd('?')) &&
                mapped == expected.TrimEnd('?') + "?")
            {
                return $"DartRuntimePrimitives.RequireValue({name})";
            }
        }
        // A nullable super-formal remains nullable when forwarded to the base
        // constructor. Non-null inherited defaults are restored above; forcing
        // every remaining nullable value through RequireValue turns valid Dart
        // omissions such as FocusScope.canRequestFocus and Semantics.enabled
        // into startup null-assertion failures.
        return name;
    }

    private string MapInheritedDefaultExpression(
        string value,
        CoreResolvedDeclaration? baseDeclaration,
        string fallbackLibrary)
    {
        var expression = value.StartsWith("const ", StringComparison.Ordinal)
            ? value["const ".Length..].Trim()
            : value.Trim();
        if (Regex.IsMatch(expression, @"^[A-Za-z_]\w*$", RegexOptions.CultureInvariant))
        {
            var visited = new HashSet<string>(StringComparer.Ordinal);
            for (var owner = baseDeclaration;
                 owner is not null && visited.Add(owner.Element.CanonicalId);
                 owner = owner.Element.Supertype is { } supertype ? FindGlobalDeclaration(supertype) : null)
            {
                if (owner.Members.Any(member => member.IsStatic && member.Name == expression))
                {
                    var ownerLibrary = LibraryUriFromElementId(owner.Element.CanonicalId);
                    return EmittedTypeName(ownerLibrary, owner.Name) + "." + SafeIdentifier(expression);
                }
            }
        }
        return MapNonConstDefaultExpression(value, fallbackLibrary);
    }

}
