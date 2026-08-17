using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private static bool IsDartPrivate(CoreResolvedDeclaration declaration) =>
        declaration.Element.IsPrivate ||
        declaration.Name.StartsWith("_", StringComparison.Ordinal) ||
        CanonicalSymbolIsPrivate(declaration.Element.CanonicalId);

    private static bool IsDartPrivate(CoreResolvedMember member) =>
        member.Element.IsPrivate ||
        member.Name.StartsWith("_", StringComparison.Ordinal) ||
        CanonicalSymbolIsPrivate(member.Element.CanonicalId);

    private static bool CanonicalSymbolIsPrivate(string? canonicalId)
    {
        if (string.IsNullOrWhiteSpace(canonicalId)) return false;
        var separator = Math.Max(canonicalId.LastIndexOf('#'), canonicalId.LastIndexOf('.'));
        return separator >= 0 && separator + 1 < canonicalId.Length && canonicalId[separator + 1] == '_';
    }

    private bool HasGlobalSetterOverride(CoreResolvedDeclaration declaration, string memberName) =>
        _semanticIndex.TypeUsers(declaration.Name)
            .Any(candidate =>
                candidate.Members.Any(member => member.IsSetter && member.Name == memberName) &&
                DirectBaseNames(candidate).Any(baseName =>
                    string.Equals(StripLibraryPrefix(baseName).Split('<')[0], declaration.Name, StringComparison.Ordinal)));

    private IEnumerable<CoreResolvedDeclaration> AppliedMixinDeclarations(CoreResolvedDeclaration declaration) =>
        (declaration.Element.Mixins ?? [])
            .Select(type => FindGlobalDeclaration(type) ?? FindGlobalDeclaration(MapType(type)))
            .Where(candidate => candidate?.Ast.Kind == CoreNodeKind.MixinDeclaration)
            .Cast<CoreResolvedDeclaration>()
            .DistinctBy(candidate => candidate.Element.CanonicalId, StringComparer.Ordinal);

    private string? ConcreteImplementedClass(CoreResolvedDeclaration declaration) =>
        (declaration.Element.Interfaces ?? [])
            .FirstOrDefault(interfaceType =>
            {
                var candidate = FindGlobalDeclaration(MapType(interfaceType));
                return candidate is not null &&
                    candidate.Ast.Kind != CoreNodeKind.MixinDeclaration &&
                    !WillEmitAsInterface(candidate);
            });

    private CoreResolvedDeclaration? DisplacedStructuralSuperclass(CoreResolvedDeclaration declaration)
    {
        if (!declaration.Name.StartsWith("_TransformedPointer", StringComparison.Ordinal) ||
            declaration.Name == "_TransformedPointerEvent" ||
            declaration.Element.Supertype is not { } supertype ||
            ConcreteImplementedClass(declaration) is null)
        {
            return null;
        }
        var candidate = FindGlobalDeclaration(MapType(supertype));
        return candidate?.Name == "_TransformedPointerEvent" ? candidate : null;
    }

    private IEnumerable<CoreResolvedDeclaration> ImplementationDonorDeclarations(CoreResolvedDeclaration declaration) =>
        AppliedMixinDeclarations(declaration)
            .Concat(DisplacedStructuralSuperclass(declaration) is { } displaced ? [displaced] : [])
            .DistinctBy(candidate => candidate.Element.CanonicalId, StringComparer.Ordinal);

    private IReadOnlyDictionary<string, string> TypeParameterSubstitutions(
        CoreResolvedDeclaration application,
        CoreResolvedDeclaration donor)
    {
        if (donor.Element.TypeParameters is not { Length: > 0 } parameters)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var appliedType = (application.Element.Mixins ?? [])
            .Select(StripLibraryPrefix)
            .FirstOrDefault(type => string.Equals(
                type.Split('<')[0],
                donor.Name,
                StringComparison.Ordinal));
        if (appliedType is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var genericStart = appliedType.IndexOf('<');
        if (genericStart < 0 || !appliedType.EndsWith('>'))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
        var arguments = SplitGenericArguments(appliedType[(genericStart + 1)..^1]);
        return parameters
            .Take(Math.Min(parameters.Length, arguments.Length))
            .Select((parameter, index) => new KeyValuePair<string, string>(parameter.Name, arguments[index]))
            .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
    }

    private IReadOnlyDictionary<string, string> DeclarationTypeParameterSubstitutions(
        CoreResolvedDeclaration declaration)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var declarationTypeParameters = (declaration.Element.TypeParameters ?? [])
            .Select(parameter => parameter.Name)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var donor in ImplementationDonorDeclarations(declaration))
        {
            foreach (var substitution in TypeParameterSubstitutions(declaration, donor))
            {
                // A mixin and its application may both use the conventional `T`
                // name for unrelated type parameters. The donor substitution is
                // valid while copying donor members, but must never rewrite the
                // application's own generic surface (for example
                // _AnimatedEvaluation<T> with AnimationWithParentMixin<double>).
                if (!declarationTypeParameters.Contains(substitution.Key))
                {
                    result.TryAdd(substitution.Key, substitution.Value);
                }
            }
        }
        if (declaration.Element.Supertype is { } supertype)
        {
            var stripped = StripLibraryPrefix(supertype);
            var genericStart = stripped.IndexOf('<');
            var superName = genericStart < 0 ? stripped : stripped[..genericStart];
            var superDeclaration = FindGlobalDeclaration(superName);
            if (genericStart >= 0 && stripped.EndsWith('>') &&
                superDeclaration?.Element.TypeParameters is { Length: > 0 } superParameters)
            {
                var arguments = SplitGenericArguments(stripped[(genericStart + 1)..^1]);
                for (var index = 0; index < Math.Min(superParameters.Length, arguments.Length); index++)
                {
                    if (!declarationTypeParameters.Contains(superParameters[index].Name))
                    {
                        result.TryAdd(superParameters[index].Name, arguments[index]);
                    }
                }
            }
        }
        return result;
    }

    private bool IsPublicMixinStorageType(string name)
    {
        if (name is "_CreationParams" or "_MutableTextRange" or
            "_NoopMouseCursorSession" or "_NoopMouseCursor")
        {
            return false;
        }
        var typePattern = $@"(?<![A-Za-z0-9_]){Regex.Escape(name)}(?![A-Za-z0-9_])";
        var declarations = _semanticIndex.TypeUsers(name);
        if (declarations.Any(candidate =>
            !IsDartPrivate(candidate) &&
            DirectBaseNames(candidate).Any(type => Regex.IsMatch(type, typePattern, RegexOptions.CultureInvariant))))
        {
            return true;
        }
        return declarations
            .Where(candidate => !IsDartPrivate(candidate) ||
                _semanticIndex.TypeUsers(candidate.Name).Any(user =>
                    !string.Equals(user.Name, candidate.Name, StringComparison.Ordinal) &&
                    user.Members.Where(member => !IsDartPrivate(member) ||
                            user.Ast.Kind == CoreNodeKind.MixinDeclaration && WillEmitAsInterface(user))
                        .SelectMany(member => new[] { member.Element.Type, member.Element.ReturnType }
                            .Concat((member.Element.Parameters ?? []).Select(parameter => parameter.Type)))
                        .Where(type => !string.IsNullOrWhiteSpace(type))
                        .Any(type => Regex.IsMatch(type!,
                            $@"(?<![A-Za-z0-9_]){Regex.Escape(candidate.Name)}(?![A-Za-z0-9_])",
                            RegexOptions.CultureInvariant))))
            // A Dart-private storage type only has to cross a CLR public
            // boundary when a Dart-public member actually exposes it. Private
            // implementation fields such as SchedulerBinding._taskQueue must
            // not promote their private entry types into the product API.
            .SelectMany(candidate => candidate.Members.Where(member =>
                !IsDartPrivate(member) ||
                candidate.Ast.Kind == CoreNodeKind.MixinDeclaration && WillEmitAsInterface(candidate)))
            .SelectMany(member => new[] { member.Element.Type, member.Element.ReturnType }
                .Concat((member.Element.Parameters ?? []).Select(parameter => parameter.Type)))
            .Where(type => !string.IsNullOrWhiteSpace(type))
            .Any(type => Regex.IsMatch(type!, typePattern, RegexOptions.CultureInvariant));
    }

    private bool HasSelectedMixinApplication(CoreResolvedDeclaration mixin) => _semanticIndex.TypeUsers(mixin.Name)
        .Any(candidate => (candidate.Element.Mixins ?? []).Any(type => string.Equals(
            StripLibraryPrefix(type).Split('<')[0],
            mixin.Name,
            StringComparison.Ordinal)));

    private bool IsGeneratedDeclaration(CoreResolvedDeclaration declaration) =>
        _generatedDeclarationIds.Contains(declaration.Element.CanonicalId);

    private static bool HasPromotedClassRepresentation(CoreResolvedDeclaration declaration) =>
        HasPromotedClassRepresentation(declaration.Name);

    private static bool HasPromotedClassRepresentation(string declarationName) =>
        declarationName is "GestureBinding" or "SchedulerBinding" or "ServicesBinding" or
            "DiagnosticableTreeMixin" or "RenderObjectWithLayoutCallbackMixin" or
            "CustomClipper" or "TextScaler" or
            "ValueNotifier" or "DiagnosticsProperty";

    private static bool HasPromotedInterfaceRepresentation(CoreResolvedDeclaration declaration) =>
        HasPromotedInterfaceRepresentation(declaration.Name);

    private static bool HasPromotedInterfaceRepresentation(string declarationName) =>
        declarationName is "Diagnosticable" or "DiagnosticableTree" or "Listenable" or "ValueListenable" or
            "AutofillScopeMixin" or "TextSelectionDelegate" or "TextInputClient" or "DeltaTextInputClient" or
            "SelectionRegistrant" or "UndoManagerClient" or "RenderSliverBoxChildManager" or
            "PaintingBinding" or "RendererBinding" or "SemanticsBinding" or
            "ViewportNotificationMixin";

    private bool WillEmitAsInterface(CoreResolvedDeclaration declaration) =>
        HasPromotedInterfaceRepresentation(declaration) ||
        ((IsGeneratedDeclaration(declaration) || !HasPromotedClassRepresentation(declaration)) &&
         declaration.Name is not ("SystemContextMenuClient" or "TextInputControl") &&
         ((declaration.Name == "NetworkImage" &&
          !IsPrivateCompanionLibrary(LibraryUriFromElementId(declaration.Element.CanonicalId))) ||
         declaration.Name == "WidgetsBindingObserver" ||
         (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration && HasSelectedMixinApplication(declaration)) ||
         (declaration.Element.IsAbstract &&
          declaration.Element.Supertype is null or "Object" &&
          declaration.Members.Any(member => member.IsAbstract) &&
          declaration.Members.All(member => member.IsStatic || member.IsAbstract || member.Kind == "constructor"))));

    private bool SameMemberShape(CoreResolvedMember left, CoreResolvedMember right) =>
        left.Kind is "method" or "field" && left.Name == right.Name &&
        (!left.IsOperator || !right.IsOperator ||
         (left.Element.Parameters?.Length ?? 0) == (right.Element.Parameters?.Length ?? 0));

    private bool IsPrivateCompanionLibrary(string libraryUri)
    {
        var lastSlash = libraryUri.LastIndexOf('/');
        var fileName = lastSlash >= 0 ? libraryUri[(lastSlash + 1)..] : libraryUri;
        return fileName.StartsWith('_');
    }

    private IEnumerable<CoreAstNode> DescendantsAndSelf(CoreAstNode node) =>
        _astIndex.DescendantsAndSelf(node);

    private static IEnumerable<CoreAstNode> DescendantsExcludingNestedFunctions(CoreAstNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            if (child.Kind == CoreNodeKind.FunctionExpression)
            {
                continue;
            }
            foreach (var descendant in DescendantsExcludingNestedFunctions(child))
            {
                yield return descendant;
            }
        }
    }

    private void AddUnsupportedDiagnostic(
        List<ConverterDiagnostic> diagnostics,
        string package,
        string library,
        string inputPath,
        CoreResolvedDeclaration declaration,
        CoreAstNode node,
        string semanticArea,
        string action)
    {
        diagnostics.Add(new(
            "DOTF0001",
            "error",
            package,
            library,
            inputPath,
            node.Offset,
            node.Length,
            declaration.Name,
            $"Typed semantic compiler has no {semanticArea} lowering for {node.Kind}.",
            "unclassified-semantic-lowering",
            "blocked",
            action,
            node.ElementId ?? declaration.Element.CanonicalId,
            [library, declaration.Element.CanonicalId]));
    }

    private string LibraryStaticClassName(string libraryUri)
    {
        var lastSlash = libraryUri.LastIndexOf('/');
        var fileName = lastSlash >= 0 ? libraryUri[(lastSlash + 1)..] : libraryUri;
        if (fileName.EndsWith(".dart", StringComparison.Ordinal))
        {
            fileName = fileName[..^5];
        }
        if (string.IsNullOrEmpty(fileName))
        {
            return "GlobalLibrary";
        }
        // The promoted Foundation package owns this reviewed public interop name.
        if (fileName == "memory_allocations")
        {
            return "MemoryAllocationsLibrary";
        }
        var safe = SafeIdentifier(fileName);
        if (safe.StartsWith("@", StringComparison.Ordinal))
        {
            safe = safe.Substring(1);
        }
        var pascal = char.ToUpperInvariant(safe[0]) + safe.Substring(1);
        return pascal + "Library";
    }

    private string QualifiedLibraryStaticClassName(string libraryUri, string currentLibrary)
    {
        if (libraryUri.StartsWith("dart:", StringComparison.Ordinal))
        {
            return MapDartLibraryStaticClass(libraryUri);
        }
        var name = LibraryStaticClassName(libraryUri);
        if (string.Equals(libraryUri, currentLibrary, StringComparison.Ordinal))
        {
            return name;
        }
        if (libraryUri.Contains("/foundation/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Foundation." + name;
        }
        if (libraryUri.Contains("/scheduler/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Scheduler." + name;
        }
        if (libraryUri.Contains("/services/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Services." + name;
        }
        if (libraryUri.Contains("/physics/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Physics." + name;
        }
        if (libraryUri.Contains("/animation/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Animation." + name;
        }
        if (libraryUri.Contains("/gestures/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Gestures." + name;
        }
        if (libraryUri.Contains("/painting/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Painting." + name;
        }
        if (libraryUri.Contains("/rendering/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Rendering." + name;
        }
        if (libraryUri.Contains("/semantics/", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Semantics." + name;
        }
        if (libraryUri.Contains("/widgets/", StringComparison.Ordinal) ||
            string.Equals(libraryUri, "package:flutter/widgets.dart", StringComparison.Ordinal))
        {
            return "global::Doroti.Framework.Widgets." + name;
        }
        return name;
    }

    private string MapDartLibraryStaticClass(string libraryUri) => libraryUri switch
    {
        "dart:math" => "global::Doroti.Runtime.Dart_mathLibrary",
        "dart:ui" => "Dart_uiLibrary",
        "dart:async" => "global::Doroti.Runtime.DartAsyncRuntime",
        "dart:convert" => "global::Doroti.Runtime.Dart_convertLibrary",
        _ => LibraryStaticClassName(libraryUri),
    };

    private bool HasNullableValueStorage(CoreAstNode node, CoreResolvedDeclaration declaration)
    {
        var mappedType = MapType(node.StaticType ?? string.Empty);
        if (!IsValueType(mappedType.TrimEnd('?'))) return false;
        if (mappedType.EndsWith("?", StringComparison.Ordinal)) return true;
        var elementId = node.ElementId;
        if (elementId is null)
        {
            var localName = node.Text(CoreProperty.name);
            return !string.IsNullOrEmpty(localName) && DescendantsAndSelf(declaration.Ast).Any(item =>
                item.Kind == CoreNodeKind.VariableDeclaration && item.Offset < node.Offset &&
                string.Equals(item.Text(CoreProperty.name), localName, StringComparison.Ordinal) &&
                item.StaticType?.EndsWith("?", StringComparison.Ordinal) == true);
        }
        if (FindGlobalMember(elementId) is { } resolvedMember &&
            (resolvedMember.Element.ReturnType ?? resolvedMember.Element.Type)?.EndsWith("?", StringComparison.Ordinal) == true)
        {
            return true;
        }
        if (DescendantsAndSelf(declaration.Ast).Any(item =>
            item.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.VariableDeclaration &&
            string.Equals(item.ElementId, elementId, StringComparison.Ordinal) &&
            item.StaticType?.EndsWith("?", StringComparison.Ordinal) == true))
        {
            return true;
        }
        var name = node.Text(CoreProperty.name);
        if (HasNullableMemberStorage(node, name)) return true;
        if (!string.IsNullOrEmpty(name) &&
            AssignmentStorageType(_session.ActiveDonorDeclaration ?? declaration, name, null)?.EndsWith("?", StringComparison.Ordinal) == true)
        {
            return true;
        }
        return !string.IsNullOrEmpty(name) && DescendantsAndSelf(declaration.Ast).Any(item =>
            item.Kind == CoreNodeKind.VariableDeclaration && item.Offset < node.Offset &&
            string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal) &&
            item.StaticType?.EndsWith("?", StringComparison.Ordinal) == true);
    }

    private bool NeedsNullableValuePromotion(CoreAstNode node, CoreResolvedDeclaration declaration)
    {
        if (node.StaticType is null || node.StaticType.EndsWith("?", StringComparison.Ordinal) ||
            !IsValueType(MapType(node.StaticType)))
        {
            return false;
        }
        var name = node.Text(CoreProperty.name);
        var elementId = node.ElementId;
        if (name is null)
        {
            return false;
        }
        if (HasNullableMemberStorage(node, name)) return true;
        if (declaration.Members.SelectMany(member => member.Element.Parameters ?? []).Any(parameter =>
            string.Equals(parameter.Name, name, StringComparison.Ordinal) &&
            parameter.Type.EndsWith("?", StringComparison.Ordinal)))
        {
            return true;
        }
        if ((declaration.Element.Parameters ?? []).Any(parameter =>
            string.Equals(parameter.Name, name, StringComparison.Ordinal) &&
            parameter.Type.EndsWith("?", StringComparison.Ordinal)))
        {
            return true;
        }
        if (DescendantsAndSelf(declaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.VariableDeclaration &&
                string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal) &&
                item.Offset <= node.Offset)
            .OrderByDescending(item => item.Offset)
            .FirstOrDefault()?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)
        {
            return true;
        }
        if (elementId is null) return false;
        if (FindGlobalMember(elementId) is { } resolvedMember &&
            (resolvedMember.Element.ReturnType ?? resolvedMember.Element.Type)?.EndsWith("?", StringComparison.Ordinal) == true)
        {
            return true;
        }
        if (declaration.Members.Any(member =>
            string.Equals(member.Element.CanonicalId, elementId, StringComparison.Ordinal) &&
            member.Element.Type?.EndsWith("?", StringComparison.Ordinal) == true))
        {
            return true;
        }
        if (DescendantsAndSelf(declaration.Ast).Any(item =>
            item.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.VariableDeclaration &&
            string.Equals(item.ElementId, elementId, StringComparison.Ordinal) &&
            item.StaticType?.EndsWith("?", StringComparison.Ordinal) == true))
        {
            return true;
        }
        return declaration.Members.Any(member =>
            elementId.StartsWith(member.Element.CanonicalId + ".", StringComparison.Ordinal) &&
            member.Element.Parameters?.Any(parameter =>
                string.Equals(parameter.Name, name, StringComparison.Ordinal) &&
                (parameter.Type.EndsWith("?", StringComparison.Ordinal) ||
                 NeedsNonConstValueDefault(parameter))) == true);
    }

    private bool HasNullableMemberStorage(CoreAstNode node, string? name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        var target = node.Kind == CoreNodeKind.PropertyAccess
            ? node.Child(CoreChildRole.targetOffset)
            : node.Kind == CoreNodeKind.PrefixedIdentifier
                ? node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                    item.Text(CoreProperty.name) == node.Text(CoreProperty.prefix))
                : null;
        if (target is null) return false;
        var targetType = ResolvedExpressionValueType(target) ?? target.StaticType ?? string.Empty;
        var owner = FindGlobalDeclaration(StripLibraryPrefix(targetType.TrimEnd('?'))) ??
            FindGlobalDeclaration(MapType(targetType).TrimEnd('?'));
        return owner is not null &&
            AssignmentStorageType(owner, name, targetType)?.EndsWith("?", StringComparison.Ordinal) == true;
    }

    private static bool IsLocalOrParameter(CoreAstNode node, CoreResolvedDeclaration declaration)
    {
        if (node.ElementId?.Contains("@local", StringComparison.Ordinal) == true) return true;
        var name = node.Text(CoreProperty.name);
        return name is not null &&
            (declaration.Element.Parameters ?? []).Concat(
                declaration.Members.SelectMany(member => member.Element.Parameters ?? []))
            .Any(parameter => string.Equals(parameter.Name, name, StringComparison.Ordinal));
    }

    private bool NeedsReferenceTypePromotion(CoreAstNode node, CoreResolvedDeclaration declaration)
    {
        if (node.ElementId is null || node.StaticType is null ||
            node.StaticType is "dynamic" or "Object" or "Object?" or "object" or "object?" or "Type" ||
            node.StaticType.Contains(" Function", StringComparison.Ordinal))
        {
            return false;
        }
        var mapped = MapType(node.StaticType);
        if (mapped is "object" or "object?" || mapped.StartsWith("Func<", StringComparison.Ordinal) ||
            mapped.StartsWith("Action<", StringComparison.Ordinal) || mapped is "Action" or "Action?" ||
            mapped.Contains("GestureTapCallback", StringComparison.Ordinal) ||
            mapped.Contains("GestureLongPressCallback", StringComparison.Ordinal) ||
            mapped.Contains("GestureDragUpdateCallback", StringComparison.Ordinal))
        {
            return false;
        }
        if (FindGlobalMember(node.ElementId) is { } resolvedMember &&
            (resolvedMember.Element.ReturnType ?? resolvedMember.Element.Type) is { } storageType &&
            ShouldCastInvocationArgument(MapType(storageType), mapped))
        {
            return true;
        }
        return DescendantsAndSelf(declaration.Ast).Any(item =>
            item.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(item.ElementId, node.ElementId, StringComparison.Ordinal) &&
            (item.StaticType is "dynamic" or "Object" or "Object?" or "object" or "object?" ||
             item.StaticType is { } candidateType && ShouldCastInvocationArgument(MapType(candidateType), mapped)));
    }

    private bool HasOverrideAnnotation(CoreAstNode node) =>
        node.Children.Any(child => child.Kind == CoreNodeKind.Annotation &&
            string.Equals(child.ElementId, "dart:core#override", StringComparison.Ordinal));

    private bool HasAwaitOutsideNestedFunctions(CoreAstNode node)
    {
        foreach (var child in node.Children)
        {
            if (child.Kind == CoreNodeKind.FunctionExpression) continue;
            if (child.Kind == CoreNodeKind.AwaitExpression || HasAwaitOutsideNestedFunctions(child)) return true;
        }
        return false;
    }

    private bool IsDartAsync(CoreAstNode node)
    {
        if (node.Text(CoreProperty.isAsync) == "true" ||
            node.Children.Any(child => child.Text(CoreProperty.isAsync) == "true")) return true;
        if (HasAwaitOutsideNestedFunctions(node)) return true;
        var body = node.Kind is CoreNodeKind.BlockFunctionBody
            ? node
            : node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
        if (body is null) return false;
        var contentOffsetText = body.Text(CoreProperty.blockOffset);
        return int.TryParse(contentOffsetText, out var contentOffset) && contentOffset > body.Offset;
    }

    private bool IsInsideDartAsyncFunction(CoreResolvedDeclaration declaration, CoreAstNode statement)
    {
        var owningDeclaration = _session.ActiveDonorDeclaration ?? declaration;
        var enclosingFunction = DescendantsAndSelf(owningDeclaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.FunctionExpression && ContainsOffset(item, statement.Offset))
            .OrderBy(item => item.Length)
            .FirstOrDefault();
        if (enclosingFunction is not null)
        {
            return IsDartAsync(enclosingFunction);
        }
        var localFunction = DescendantsAndSelf(owningDeclaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.FunctionDeclarationStatement && ContainsOffset(item, statement.Offset))
            .OrderBy(item => item.Length)
            .FirstOrDefault();
        if (localFunction is not null)
        {
            return DescendantsAndSelf(localFunction).Any(item =>
                item.Kind == CoreNodeKind.FunctionExpression && item.Text(CoreProperty.isAsync) == "true");
        }
        var member = owningDeclaration.Members
            .Where(item => ContainsOffset(item.Ast, statement.Offset))
            .OrderBy(item => item.Ast.Length)
            .FirstOrDefault();
        return IsFutureType(ContainingReturnType(owningDeclaration, statement)) &&
            (member is not null ? IsDartAsync(member.Ast) : IsDartAsync(owningDeclaration.Ast));
    }

    private string ContainingReturnType(CoreResolvedDeclaration declaration, CoreAstNode statement)
    {
        // Mixin members are emitted into the concrete application while their
        // source offsets and return contracts still belong to the donor.
        // Looking those offsets up in the receiver can accidentally select an
        // unrelated member with an overlapping source range.
        var owningDeclaration = _session.ActiveDonorDeclaration ?? declaration;
        var enclosingFunction = DescendantsAndSelf(owningDeclaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.FunctionExpression && ContainsOffset(item, statement.Offset))
            .OrderBy(item => item.Length)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(_session.ActiveFunctionReturnType))
        {
            return _session.ActiveFunctionReturnType;
        }
        if (!string.IsNullOrWhiteSpace(enclosingFunction?.StaticType))
        {
            var functionIndex = FindTopLevelFunctionIndex(enclosingFunction.StaticType);
            if (functionIndex >= 0)
            {
                return MapType(enclosingFunction.StaticType[..functionIndex].Trim());
            }
        }
        var localFunction = DescendantsAndSelf(owningDeclaration.Ast)
            .Where(item => item.Kind == CoreNodeKind.FunctionDeclarationStatement && ContainsOffset(item, statement.Offset))
            .OrderBy(item => item.Length)
            .FirstOrDefault();
        var localFunctionType = localFunction?.Children
            .FirstOrDefault(item => item.Kind == CoreNodeKind.FunctionDeclaration)?
            .Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FunctionExpression)?
            .StaticType;
        if (!string.IsNullOrWhiteSpace(localFunctionType))
        {
            var functionIndex = FindTopLevelFunctionIndex(localFunctionType);
            if (functionIndex >= 0)
            {
                return MapType(localFunctionType[..functionIndex].Trim());
            }
        }
        var owningMember = owningDeclaration.Members
            .Where(item => ContainsOffset(item.Ast, statement.Offset))
            .OrderBy(item => item.Ast.Length)
            .FirstOrDefault();
        var contractMember = owningMember is null ? null : FindBaseContractMember(owningDeclaration, owningMember);
        return MapType(owningMember?.Element.ReturnType ??
            (owningMember?.Kind == "field" ? owningMember.Element.Type : null) ??
            contractMember?.Element.ReturnType ??
            (contractMember?.Kind == "field" ? contractMember.Element.Type : null) ??
            owningDeclaration.Element.ReturnType ??
            "void");
    }

    private bool ContainsOffset(CoreAstNode node, int offset) =>
        offset >= node.Offset && offset < node.Offset + node.Length;

    private bool IsFutureType(string type) =>
        type == "Future" || type.StartsWith("Future<", StringComparison.Ordinal);

    private bool ExpressionProducesFuture(CoreAstNode expression) =>
        expression.StaticType is { } type && (type == "Future" || type.StartsWith("Future<", StringComparison.Ordinal));

    private bool HasOverridableBaseMember(CoreResolvedDeclaration declaration, CoreResolvedMember member) =>
        FindOverriddenBaseMember(declaration, member) is not null;

    private CoreResolvedMember? FindOverriddenBaseMember(CoreResolvedDeclaration declaration, CoreResolvedMember member)
        => FindBaseMember(declaration, member, includeInterfaces: false);

    private CoreResolvedMember? FindBaseContractMember(CoreResolvedDeclaration declaration, CoreResolvedMember member)
        => FindBaseMember(declaration, member, includeInterfaces: true);

    private CoreResolvedMember? FindDirectSuperclassMember(
        CoreResolvedDeclaration declaration,
        CoreResolvedMember member)
    {
        if (declaration.Element.Supertype is not { } supertype || supertype == "Object")
        {
            return null;
        }
        var superDeclaration = FindGlobalDeclaration(MapType(supertype));
        return superDeclaration?.Members.FirstOrDefault(candidate => SameMemberShape(candidate, member));
    }

    private CoreResolvedMember? FindBaseMember(
        CoreResolvedDeclaration declaration,
        CoreResolvedMember member,
        bool includeInterfaces)
    {
        var pending = new Queue<string>(ClrBaseNames(declaration, includeInterfaces));
        var visited = new HashSet<string>(StringComparer.Ordinal);
        while (pending.Count > 0)
        {
            var baseName = pending.Dequeue();
            if (string.IsNullOrEmpty(baseName) || baseName == "Object" || !visited.Add(baseName)) continue;
            var simpleName = StripLibraryPrefix(baseName);
            var generic = simpleName.IndexOf('<');
            if (generic >= 0) simpleName = simpleName[..generic];
            var baseDeclaration = _currentDeclarations?.FirstOrDefault(candidate => candidate.Name == simpleName)
                ?? FindGlobalDeclaration(simpleName);
            if (baseDeclaration is null)
            {
                continue;
            }
            var baseIsInterface = WillEmitAsInterface(baseDeclaration);
            if (baseIsInterface && !includeInterfaces)
            {
                continue;
            }
            var effectiveMembers = baseDeclaration.Members
                .Concat(AppliedMixinDeclarations(baseDeclaration).SelectMany(mixin => mixin.Members));
            var match = effectiveMembers.FirstOrDefault(candidate =>
                SameMemberShape(candidate, member) &&
                (!member.Name.StartsWith("_", StringComparison.Ordinal) ||
                 string.Equals(
                     LibraryUriFromElementId(candidate.Element.CanonicalId),
                     LibraryUriFromElementId(member.Element.CanonicalId),
                     StringComparison.Ordinal)));
            if (match is not null)
            {
                return match;
            }
            if (!baseIsInterface &&
                baseDeclaration.Element.IsAbstract &&
                baseDeclaration.Element.Supertype is { } interfaceSupertype)
            {
                var interfaceDeclaration = FindGlobalDeclaration(MapType(interfaceSupertype));
                if (interfaceDeclaration is not null && WillEmitAsInterface(interfaceDeclaration))
                {
                    var interfaceMatch = interfaceDeclaration.Members.FirstOrDefault(candidate =>
                        candidate.Kind is "method" or "field" && candidate.Name == member.Name);
                    if (interfaceMatch is not null)
                    {
                        return interfaceMatch;
                    }
                }
            }
            foreach (var parent in ClrBaseNames(baseDeclaration, includeInterfaces)) pending.Enqueue(parent);
        }
        return null;
    }

    private IEnumerable<string> ClrBaseNames(CoreResolvedDeclaration declaration, bool includeInterfaces)
    {
        if (declaration.Name == "WidgetsFlutterBinding")
        {
            yield return "GestureBinding";
            yield break;
        }
        if (declaration.Name == "RenderingFlutterBinding")
        {
            yield return "GestureBinding";
            yield break;
        }
        if (DisplacedStructuralSuperclass(declaration) is not null && ConcreteImplementedClass(declaration) is { } structuralBase)
        {
            yield return structuralBase;
        }
        else if (declaration.Element.Supertype is { } supertype && supertype != "Object")
        {
            yield return supertype;
        }
        else if ((declaration.Element.Mixins ?? []).FirstOrDefault(type =>
            StripLibraryPrefix(type).Split('<')[0] == "ChangeNotifier") is { } changeNotifier)
        {
            yield return changeNotifier;
        }
        else if ((declaration.Element.Mixins ?? []).FirstOrDefault(type =>
            FindGlobalDeclaration(MapType(type)) is { } mixin && !WillEmitAsInterface(mixin)) is { } classMixin)
        {
            // Keep symbol-family traversal aligned with the CLR base selected
            // during declaration emission for class-backed Dart mixins.
            yield return classMixin;
        }
        else if ((declaration.Element.Interfaces ?? []).FirstOrDefault(type =>
            FindGlobalDeclaration(MapType(type)) is { } implementedClass &&
            implementedClass.Ast.Kind != CoreNodeKind.MixinDeclaration &&
            !WillEmitAsInterface(implementedClass)) is { } structuralClass)
        {
            // Dart `implements ConcreteClass` is represented by the same CLR
            // base selected in declaration emission when no true superclass
            // exists. Override discovery must traverse that structural base.
            yield return structuralClass;
        }

        if (!includeInterfaces)
        {
            yield break;
        }
        foreach (var candidate in (declaration.Element.Mixins ?? [])
            .Concat(declaration.Element.Interfaces ?? []))
        {
            var candidateDeclaration = FindGlobalDeclaration(MapType(candidate));
            if (candidateDeclaration is not null && WillEmitAsInterface(candidateDeclaration))
            {
                yield return candidate;
            }
        }
    }

    private IEnumerable<string> DirectBaseNames(CoreResolvedDeclaration declaration)
    {
        if (WillEmitAsInterface(declaration))
        {
            foreach (var interfaceType in declaration.Element.Interfaces ?? []) yield return interfaceType;
            foreach (var mixinType in declaration.Element.Mixins ?? []) yield return mixinType;
            if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration)
            {
                var onClause = declaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.MixinOnClause);
                if (onClause is not null)
                {
                    foreach (var type in onClause.Children.Where(item => item.Category == "type").Select(MapTypeFromAst))
                    {
                        yield return type;
                    }
                }
            }
            yield break;
        }
        if (DisplacedStructuralSuperclass(declaration) is not null && ConcreteImplementedClass(declaration) is { } structuralBase)
        {
            yield return structuralBase;
        }
        else if (declaration.Element.Supertype is { } supertype && supertype != "Object") yield return supertype;
        else
        {
            foreach (var mixinType in declaration.Element.Mixins ?? [])
            {
                var mixinDeclaration = FindGlobalDeclaration(MapType(mixinType));
                if (mixinDeclaration is not null && !WillEmitAsInterface(mixinDeclaration))
                {
                    yield return mixinType;
                    break;
                }
            }
            foreach (var interfaceType in declaration.Element.Interfaces ?? [])
            {
                var interfaceDeclaration = FindGlobalDeclaration(MapType(interfaceType));
                if (interfaceDeclaration is not null &&
                    interfaceDeclaration.Ast.Kind != CoreNodeKind.MixinDeclaration &&
                    !WillEmitAsInterface(interfaceDeclaration))
                {
                    yield return interfaceType;
                    break;
                }
            }
        }
        foreach (var mixin in declaration.Element.Mixins ?? []) yield return mixin;
        if (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration)
        {
            var onClause = declaration.Ast.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.MixinOnClause);
            if (onClause is not null)
            {
                foreach (var type in onClause.Children.Where(item => item.Category == "type").Select(MapTypeFromAst)) yield return type;
            }
        }
    }

    private IEnumerable<CoreResolvedDeclaration> InterfaceContractDeclarations(CoreResolvedDeclaration declaration)
    {
        yield return declaration;
        foreach (var baseDeclaration in DirectBaseNames(declaration)
            .Select(FindGlobalDeclaration)
            .Where(candidate => candidate is not null && WillEmitAsInterface(candidate))
            .Cast<CoreResolvedDeclaration>())
        {
            foreach (var contract in InterfaceContractDeclarations(baseDeclaration))
            {
                yield return contract;
            }
        }
    }

    private bool IsKnownExternalOverride(CoreResolvedDeclaration declaration, CoreResolvedMember member) =>
        DirectBaseNames(declaration).Any(baseName =>
        {
            var simpleBase = StripLibraryPrefix(baseName).Split('<')[0];
            return simpleBase switch
            {
                "StatelessWidget" => member.Name == "build",
                "StatefulWidget" => member.Name == "createState",
                "State" => member.Name == "build",
                "BindingBase" when HasOverrideAnnotation(member.Ast) => member.Name is "initInstances" or "initServiceExtensions" or "unlocked" or "performReassemble" or "debugCheckZone" or "reassembleApplication",
                "GestureRecognizer" when HasOverrideAnnotation(member.Ast) => member.Name is "debugDescription" or "dispose" or "acceptGesture" or "rejectGesture",
                "OneSequenceGestureRecognizer" when HasOverrideAnnotation(member.Ast) => member.Name is "debugDescription" or "addAllowedPointer" or "didStopTrackingLastPointer" or "handleEvent" or "acceptGesture" or "rejectGesture" or "stopTrackingPointer" or "dispose",
                _ => false,
            };
        });

    private CoreResolvedDeclaration? FindGlobalDeclaration(string mappedType)
    {
        var simpleName = StripLibraryPrefix(mappedType).TrimEnd('?');
        var generic = simpleName.IndexOf('<');
        if (generic >= 0) simpleName = simpleName[..generic];
        var namespaceSeparator = simpleName.LastIndexOf('.');
        if (namespaceSeparator >= 0) simpleName = simpleName[(namespaceSeparator + 1)..];
        if (_semanticIndex.DeclarationsBySimpleName.TryGetValue(simpleName, out var matches))
        {
            return matches[0];
        }
        return _semanticIndex.FindEmittedDeclaration(simpleName);
    }

    private CoreResolvedMember? FindGlobalMember(string? elementId) => _semanticIndex.FindMember(elementId);

    private bool IsRequiredByMixinConstraint(CoreResolvedMember member)
    {
        // Dart-private names are library scoped and cannot be part of an `on`
        // contract consumed from another library. Promoting them to CLR public
        // leaked implementation storage into the candidate API.
        if (IsDartPrivate(member)) return false;
        foreach (var mixin in _semanticIndex.MixinDeclarations)
        {
            foreach (var constraintName in DirectBaseNames(mixin))
            {
                var constraint = FindGlobalDeclaration(constraintName);
                if (constraint?.Members.Any(candidate =>
                    candidate.Element.CanonicalId == member.Element.CanonicalId) == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsStructurallyImplementedClass(CoreResolvedDeclaration declaration) =>
        declaration.Ast.Kind == CoreNodeKind.ClassDeclaration &&
        !IsInterfaceDeclaration(declaration) &&
        _semanticIndex.TypeUsers(declaration.Name).Any(candidate =>
            (candidate.Element.Interfaces ?? []).Any(interfaceType =>
                string.Equals(
                    StripLibraryPrefix(interfaceType).Split('<')[0],
                    declaration.Name,
                    StringComparison.Ordinal)));

    private bool IsInterfaceDeclaration(CoreResolvedDeclaration declaration) =>
        declaration.Name is not ("SystemContextMenuClient" or "TextInputControl") &&
        (declaration.Ast.Kind == CoreNodeKind.MixinDeclaration ||
        (declaration.Element.IsAbstract &&
        declaration.Members.Any(candidate => candidate.IsAbstract) &&
        declaration.Members.All(candidate => candidate.IsStatic || candidate.IsAbstract || candidate.Kind == "constructor")));

    private bool IsUnboundTypeParameterName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            return false;
        }
        // Analyzer-erased type parameters typically appear as single-letter names
        // (T, U, …) without namespace or generic arity.
        return typeName.Length == 1 && char.IsUpper(typeName[0]);
    }

    private bool ContainsUnboundTypeParameter(string typeName) =>
        Regex.IsMatch(typeName, @"(?<![A-Za-z0-9_])[A-Z](?![A-Za-z0-9_])", RegexOptions.CultureInvariant);

    private string SafeIdentifier(string value)
    {
        var identifierBuilder = new StringBuilder(value.Length + 1);
        if (value.Length == 0 || !(char.IsLetter(value[0]) || value[0] == '_'))
        {
            identifierBuilder.Append('_');
        }
        foreach (var character in value)
        {
            identifierBuilder.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }
        var identifier = identifierBuilder.ToString();
        return CSharpKeywords.Contains(identifier) ? "@" + identifier : identifier;
    }

    private string SyntheticIdentifier(string value) => "__" + SafeIdentifier(value).TrimStart('@');

    private string NamedConstructorMethodName(string name) =>
        "Create" + (name.Length == 0 ? string.Empty : char.ToUpperInvariant(name[0]) + name[1..]);

    private bool TryResolveEmittedNamedConstructor(string typeName, string constructorName, out string methodName)
    {
        methodName = NamedConstructorMethodName(constructorName);
        var declaration = FindGlobalDeclaration(typeName);
        if (declaration is null) return false;
        if (declaration.Ast.Kind == CoreNodeKind.ExtensionTypeDeclaration)
        {
            var representationConstructor = declaration.Ast.Children
                .FirstOrDefault(item => item.Kind == CoreNodeKind.RepresentationDeclaration)?
                .Text(CoreProperty.constructor);
            if (string.Equals(representationConstructor, constructorName, StringComparison.Ordinal))
            {
                return true;
            }
        }
        var constructors = declaration.Members.Where(item => item.Kind == "constructor").OrderBy(item => item.Offset).ToArray();
        var match = constructors.FirstOrDefault(item => item.Name == constructorName);
        if (match is null)
        {
            return false;
        }
        // Factory constructors become static Create* methods. Generative named
        // constructors become real C# ctors when the primary is a factory, or
        // static Create* bridges when multiple generative ctors exist.
        if (match.IsFactory)
        {
            return true;
        }
        var primaryConstructor = PrimaryGenerativeConstructor(constructors);
        return constructors.Length > 1 &&
            match != primaryConstructor &&
            primaryConstructor is { IsFactory: false };
    }

    private CoreResolvedMember? PrimaryGenerativeConstructor(CoreResolvedMember[] constructors) =>
        constructors.FirstOrDefault(item => !item.IsFactory &&
            !DescendantsAndSelf(item.Ast).Any(node => node.Kind == CoreNodeKind.RedirectingConstructorInvocation))
        ?? constructors.FirstOrDefault(item => !item.IsFactory)
        ?? constructors.FirstOrDefault();

    private bool TryResolveEmittedDefaultFactoryConstructor(string typeName, out string methodName)
    {
        methodName = "Create";
        var declaration = FindGlobalDeclaration(typeName);
        if (declaration is null) return false;
        return declaration.Members.Any(item =>
            item.Kind == "constructor" && item.IsFactory && item.Name is "new" or "");
    }

}
