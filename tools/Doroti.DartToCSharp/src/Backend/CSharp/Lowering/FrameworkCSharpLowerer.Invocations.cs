using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private void EmitConstructorReference(CsSyntaxBuilder builder, CoreAstNode node)
    {
        var constructorName = node.Child(CoreChildRole.constructorOffset) ?? node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ConstructorName);
        if (constructorName is null)
        {
            builder.Append("default");
            return;
        }
        var type = MapTypeFromAst(constructorName);
        var named = constructorName.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier);
        var constructor = named?.Text(CoreProperty.name) ?? constructorName.Text(CoreProperty.name) ?? "new";
        var functionType = node.StaticType ?? string.Empty;
        var functionIndex = functionType.IndexOf(" Function", StringComparison.Ordinal);
        var parameterStart = functionIndex < 0 ? -1 : functionType.IndexOf('(', functionIndex);
        var parameterEnd = functionType.LastIndexOf(')');
        var parameterCount = parameterStart >= 0 && parameterEnd > parameterStart
            ? SplitGenericArguments(functionType[(parameterStart + 1)..parameterEnd]).Length
            : 0;
        var arguments = Enumerable.Range(0, parameterCount).Select(index => $"arg{index}").ToArray();
        var mappedFunctionType = functionIndex < 0 ? null : MapType(functionType).TrimEnd('?');
        if (!string.IsNullOrEmpty(mappedFunctionType))
        {
            // A constructor tear-off can initialize `var` in Dart because its
            // resolved function type is carried by the analyzer. C# lambdas do
            // not have a natural type, so preserve that typed-IR contract at
            // the expression boundary instead of relying on target inference.
            builder.Append("((").Append(mappedFunctionType).Append(")(");
        }
        builder.Append('(').Append(string.Join(", ", arguments)).Append(") => ");
        if (constructor == "new")
        {
            builder.Append("new ").Append(type).Append('(').Append(string.Join(", ", arguments)).Append(')');
        }
        else
        {
            var member = TryResolveEmittedNamedConstructor(type, constructor, out var namedConstructorMethod)
                ? namedConstructorMethod
                : SafeIdentifier(constructor);
            builder.Append(type).Append('.').Append(member)
                .Append('(').Append(string.Join(", ", arguments)).Append(')');
        }
        if (!string.IsNullOrEmpty(mappedFunctionType))
        {
            builder.Append("))");
        }
    }

    private void EmitDotShorthandPropertyAccess(CsSyntaxBuilder builder, CoreAstNode node)
    {
        var identifier = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier);
        var name = identifier is null ? "missing" : identifier.Text(CoreProperty.name) ?? "missing";
        if (identifier is not null &&
            !string.IsNullOrEmpty(identifier.ElementId) &&
            identifier.ElementId.Contains('#', StringComparison.Ordinal))
        {
            var symbol = identifier.ElementId[(identifier.ElementId.LastIndexOf('#') + 1)..];
            var separator = symbol.LastIndexOf('.');
            if (separator > 0)
            {
                builder.Append(MapType(symbol[..separator]).TrimEnd('?'))
                    .Append('.')
                    .Append(SafeIdentifier(symbol[(separator + 1)..]));
                return;
            }
        }

        var typeName = MapType(node.StaticType ?? identifier?.StaticType ?? "object").TrimEnd('?');
        builder.Append(typeName).Append('.').Append(SafeIdentifier(name));
    }

    private void EmitPrefixedIdentifier(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        _ = declaration;
        _ = package;
        _ = library;
        _ = inputPath;
        _ = diagnostics;
        var name = node.Text(CoreProperty.name) ?? "missing";
        if (name == "kLongPressTimeout")
        {
            builder.Append("global::Doroti.Framework.Gestures.ConstantsLibrary.kLongPressTimeout");
            return;
        }
        var prefix = node.Text(CoreProperty.prefix) ?? "missing";
        if (prefix == "developer" && name == "CreationLocation")
        {
            builder.Append("global::Doroti.Runtime.CreationLocation");
            return;
        }
        if (name == "dispatchPointerEvent")
        {
            builder.Append("((__event) => ").Append(SafeIdentifier(prefix))
                .Append(".dispatchPointerEvent(global::Doroti.Ui.PointerEvent.FromFrameworkEvent(")
                .Append("__event is global::Doroti.Framework.Gestures.PointerDownEvent ? 1L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerUpEvent ? 2L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerCancelEvent ? 3L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerHoverEvent ? 4L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerMoveEvent ? 5L : 0L, ")
                .Append("__event.pointer, __event.embedderId, __event.platformData, __event.timeStamp, __event.position, __event.kind, ")
                .Append("__event.orientation, __event.pressure, __event.size, __event.radiusMajor, __event.radiusMinor)))");
            return;
        }
        if (prefix == "RenderViewportBase" && name == "showInViewport")
        {
            builder.Append("global::Doroti.Framework.Rendering.RenderViewportBase<global::Doroti.Framework.Rendering.SliverPhysicalContainerParentData>.showInViewport");
            return;
        }
        if (name == "debugFormatDouble")
        {
            builder.Append("(value => global::Doroti.Framework.Foundation.DebugLibrary.debugFormatDouble(value))");
            return;
        }
        if (prefix == "SemanticsBinding")
        {
            builder.Append("global::Doroti.Framework.Semantics.SemanticsBinding.")
                .Append(SafeIdentifier(name));
            return;
        }
        if (string.Equals(node.ElementId, "dart:math#pi", StringComparison.Ordinal))
        {
            builder.Append("Dart_mathLibrary.pi");
            return;
        }
        if (prefix == "double" && name is "infinity" or "negativeInfinity" or "nan" or "maxFinite" or "minPositive")
        {
            builder.Append(MapDoubleConstant(name));
            return;
        }
        if (name == "fromStandardMessageCodecMessage")
        {
            builder.Append(SafeIdentifier(prefix)).Append(".CreateFromStandardMessageCodecMessage");
            return;
        }
        var prefixNode = node.Children.FirstOrDefault(item =>
            item.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(item.Text(CoreProperty.name), prefix, StringComparison.Ordinal));
        if (prefix == "innerRect" && name is "top" or "left" or "right" or "bottom" && prefixNode is not null)
        {
            builder.Append("DartRuntimePrimitives.RequireValue(")
                .Append(EmittedLocalIdentifier(prefixNode, prefix))
                .Append(").").Append(name);
            return;
        }
        if (name == "hashCode")
        {
            if (prefixNode is not null)
            {
                LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
                builder.Append(".GetHashCode()");
            }
            else
            {
                builder.Append("GetHashCode()");
            }
            return;
        }
        if (name is "isEmpty" or "isNotEmpty" && prefixNode?.StaticType?.TrimEnd('?') == "CharacterRange")
        {
            builder.Append('(');
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "isEmpty" ? ".Count == 0)" : ".Count != 0)");
            return;
        }
        if (name == "values" && FindGlobalDeclaration(prefix)?.Ast.Kind == CoreNodeKind.EnumDeclaration)
        {
            builder.Append("System.Enum.GetValues<").Append(MapType(prefix)).Append(">().ToList()");
            return;
        }
        if ((string.IsNullOrEmpty(prefixNode?.StaticType) || prefixNode.StaticType == "Type") &&
            FindGlobalDeclaration(prefix) is { } prefixedTypeDeclaration &&
            prefixedTypeDeclaration.Ast.Kind is CoreNodeKind.ClassDeclaration or CoreNodeKind.MixinDeclaration or
                CoreNodeKind.EnumDeclaration or CoreNodeKind.ExtensionTypeDeclaration)
        {
            builder.Append(MapStaticOwnerType(prefix, declaration)).Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (name == "runtimeType")
        {
            builder.Append("DartRuntimePrimitives.RuntimeType(");
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(')');
            return;
        }
        if (prefix == "Object" && name == "hash")
        {
            builder.Append("FoundationRuntimePorts.ObjectHash");
            return;
        }
        if (prefix == "Uri")
        {
            builder.Append("DartUri.").Append(SafeIdentifier(name));
            return;
        }
        if (prefix == "math" && name == "pi")
        {
            builder.Append("Dart_mathLibrary.pi");
            return;
        }
        if (prefix == "StackTrace" && name == "current")
        {
            builder.Append("new global::System.Diagnostics.StackTrace(true)");
            return;
        }
        if (StripLibraryPrefix(prefix) == "_CachedLayoutCalculation" &&
            (name is "dryLayout" or "baseline"))
        {
            var closedType = name == "dryLayout"
                ? "_CachedLayoutCalculation<BoxConstraints, Size>"
                : "_CachedLayoutCalculation<(BoxConstraints, TextBaseline), BaselineOffset>";
            builder.Append(MapType(closedType)).Append('.').Append(name);
            return;
        }
        if (!string.IsNullOrEmpty(node.ElementId))
        {
            var elementLibrary = LibraryUriFromElementId(node.ElementId);
            var symbol = node.ElementId!.Contains('#', StringComparison.Ordinal)
                ? node.ElementId[(node.ElementId.LastIndexOf('#') + 1)..]
                : name;
            var ownerSeparator = symbol.LastIndexOf('.');
            if (!string.IsNullOrEmpty(elementLibrary) && ownerSeparator > 0 &&
                (string.IsNullOrEmpty(prefixNode?.StaticType) || prefixNode.StaticType == "Type"))
            {
                var ownerName = symbol[..ownerSeparator];
                var ownerDeclaration = FindDeclaration(elementLibrary + "#" + ownerName);
                if (ownerDeclaration?.Ast.Kind is CoreNodeKind.ClassDeclaration or CoreNodeKind.MixinDeclaration or
                    CoreNodeKind.EnumDeclaration or CoreNodeKind.ExtensionTypeDeclaration)
                {
                    builder.Append(MapStaticOwnerType(ownerDeclaration.Name, declaration))
                        .Append('.').Append(SafeIdentifier(name));
                    return;
                }
            }
            // Import-prefixed top-level: library#symbol with no owner dots.
            if (!string.IsNullOrEmpty(elementLibrary) && !symbol.Contains('.', StringComparison.Ordinal) &&
                string.IsNullOrEmpty(prefixNode?.StaticType))
            {
                var referencedDeclaration = FindDeclaration(node.ElementId!);
                if (referencedDeclaration?.Ast.Kind is CoreNodeKind.ClassDeclaration or CoreNodeKind.MixinDeclaration or CoreNodeKind.EnumDeclaration or CoreNodeKind.ExtensionTypeDeclaration)
                {
                    builder.Append(EmittedTypeName(elementLibrary, referencedDeclaration.Name));
                    return;
                }
                if (elementLibrary == "dart:ui" && symbol.Length > 0 && char.IsUpper(symbol[0]))
                {
                    builder.Append(MapType(symbol));
                    return;
                }
                if (elementLibrary.StartsWith("dart:", StringComparison.Ordinal))
                {
                    builder.Append(MapDartLibraryStaticClass(elementLibrary)).Append('.').Append(SafeIdentifier(symbol));
                }
                else
                {
                    builder.Append(QualifiedLibraryStaticClassName(elementLibrary, library)).Append('.').Append(SafeIdentifier(symbol));
                }
                return;
            }
        }

        // Dart also uses PrefixedIdentifier for receiver.property (e.g. index.index).
        var prefixType = (prefixNode is null
            ? string.Empty
            : ResolvedExpressionValueType(prefixNode) ?? string.Empty).TrimEnd('?');
        if (prefixType.Length == 0 && prefixNode?.Text(CoreProperty.name) is { } prefixMemberName)
        {
            prefixType = (AssignmentStorageType(
                _session.ActiveDonorDeclaration ?? declaration,
                prefixMemberName,
                null) ?? string.Empty).TrimEnd('?');
        }
        if (prefixNode is not null && name == "_depth")
        {
            builder.Append("((ViewportNotificationMixin)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(")._depth");
            return;
        }
        if (prefixNode is not null && name == "mounted" &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "DisposableBuildContext")
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".mounted");
            return;
        }
        if (prefixNode is not null && name == "target" &&
            prefixType.StartsWith("WeakReference<", StringComparison.Ordinal))
        {
            builder.Append("DartCoreExtensions.weakTarget(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (prefixNode is not null && name is "isEmpty" or "isNotEmpty" &&
            (IsDartEnumerableType(prefixType) || IsDartEnumerableType(MapType(prefixType))))
        {
            var iterableNullAware = node.Text(CoreProperty.@operator) == "?." ||
                prefixNode.StaticType?.EndsWith("?", StringComparison.Ordinal) == true;
            if (iterableNullAware)
            {
                var promoted = $"__items{node.Offset}";
                builder.Append('(');
                LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
                builder.Append(" is { } ").Append(promoted).Append(" ? ");
                if (name == "isEmpty") builder.Append('!');
                builder.Append("System.Linq.Enumerable.Any(").Append(promoted).Append(") : (bool?)null)");
                return;
            }
            builder.Append(name == "isEmpty" ? "!System.Linq.Enumerable.Any(" : "System.Linq.Enumerable.Any(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (prefixNode?.Text(CoreProperty.name) == "TextAlign")
        {
            builder.Append("global::Doroti.Ui.TextAlign.").Append(SafeIdentifier(name));
            return;
        }
        if (prefixNode is not null && TryEmitEnumGetter(builder, prefixNode, prefixType, name, declaration, package, library, inputPath, diagnostics))
        {
            return;
        }
        if (prefixNode is not null && TryResolvePromotedMemberOwner(node, prefixType, name, out var promotedOwner))
        {
            builder.Append("((").Append(promotedOwner).Append(")");
            if (IsCurrentTypeParameter(prefixType)) builder.Append("(object)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(").").Append(MapPropertyAccessName(node.ElementId, name, prefixType));
            return;
        }
        if (node.ElementId?.Contains("#SemanticsBinding.disableAnimations", StringComparison.Ordinal) == true)
        {
            builder.Append("PlatformDispatcher.instance.accessibilityFeatures.disableAnimations");
            return;
        }
        if (prefixNode is not null && prefixType == "PointerData" &&
            name is "viewId" or "device" or "pointerIdentifier" or "embedderId")
        {
            builder.Append("checked((long)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(SafeIdentifier(name)).Append(')');
            return;
        }
        if (prefixNode is not null && prefixType == "DorotiView" && name == "viewId")
        {
            builder.Append("checked((long)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".viewId)");
            return;
        }
        if (prefixNode is not null && prefixType is "Vector4" or "global::System.Numerics.Vector4" && name == "storage")
        {
            builder.Append("new double[] { ");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".X, ");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Y, ");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Z, ");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".W }");
            return;
        }
        if (prefixNode is not null && prefixType is "PointerEvent" or "PointerPanZoomUpdateEvent" &&
            name is "pan" or "localPan" or "panDelta" or "localPanDelta")
        {
            builder.Append("(((PointerPanZoomUpdateEvent?)(object?)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(")!).").Append(name);
            return;
        }
        if (name == "isNaN" && prefixNode is not null && prefixType is "double" or "num")
        {
            builder.Append("double.IsNaN(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "isFinite" && prefixNode is not null && prefixType is "double" or "num")
        {
            if (node.Text(CoreProperty.@operator) == "?." || prefixNode.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)
            {
                var promoted = $"__finite{node.Offset}";
                builder.Append('(');
                LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
                builder.Append(" is { } ").Append(promoted).Append(" ? double.IsFinite(").Append(promoted).Append(") : (bool?)null)");
                return;
            }
            builder.Append("double.IsFinite(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "isFinite" && prefixNode is not null && prefixType == "int")
        {
            builder.Append("true");
            return;
        }
        if (name == "isInfinite" && prefixNode is not null && prefixType is "double" or "num")
        {
            builder.Append("double.IsInfinity(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name is "isOdd" or "isEven" && prefixNode is not null && prefixType is "int" or "double" or "num")
        {
            builder.Append("((checked((long)(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "isOdd" ? ")) & 1L) != 0L)" : ")) & 1L) == 0L)");
            return;
        }
        if (name == "eventSource")
        {
            builder.Append("((RawKeyEventDataAndroid)");
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(").eventSource");
            return;
        }
        if (name == "values" && prefix.Length > 0 &&
            (char.IsUpper(prefix[0]) || FindGlobalDeclaration(prefix)?.Ast.Kind == CoreNodeKind.EnumDeclaration))
        {
            if (prefix == "FontWeight")
            {
                builder.Append("global::Doroti.Ui.FontWeight.values");
                return;
            }
            builder.Append("System.Enum.GetValues<").Append(MapType(prefix)).Append(">().ToList()");
            return;
        }
        var isTypeParameter = declaration.Element.TypeParameters?.Any(item =>
            string.Equals(item.Name, prefixType, StringComparison.Ordinal)) == true;
        var prefixDeclaration = FindGlobalDeclaration(prefixType);
        if (name == "index" && (isTypeParameter || prefixType is "dynamic" or "object" or "Object" or "" ||
            IsEnumType(prefixType) || prefixDeclaration?.Ast.Kind == CoreNodeKind.EnumDeclaration))
        {
            builder.Append("FoundationRuntimePorts.EnumIndex(");
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(')');
            return;
        }
        if (name == "sign" && prefixType is "int" or "double" or "num")
        {
            builder.Append("Math.Sign(");
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(')');
            return;
        }
        if (name == "kind" && prefixNode?.StaticType == "SystemMouseCursor")
        {
            builder.Append("((SystemMouseCursor)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(").kind");
            return;
        }

        if (name == "length" && prefixType == "String")
        {
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(prefixNode?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ||
                node.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ||
                node.Text(CoreProperty.@operator) == "?." ? "?.Length" : ".Length");
            return;
        }
        if (name == "reversed" && prefixNode is not null && IsDartEnumerableType(prefixType))
        {
            builder.Append("System.Linq.Enumerable.Reverse(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name is "isEmpty" or "isNotEmpty" && prefixType is "String" or "string")
        {
            builder.Append('(');
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(".Length ").Append(name == "isEmpty" ? "== 0" : "!= 0").Append(')');
            return;
        }
        if (name is "characters" or "runes" && prefixType is "String" or "string")
        {
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append('.').Append(name).Append("()");
            return;
        }
        if (name == "isEmpty" && prefixType == "Size")
        {
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(".isEmpty");
            return;
        }
        if (name == "isEmpty" && prefixType == "Rect")
        {
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(".isEmpty");
            return;
        }
        if (name == "name" && IsEnumType(prefixType))
        {
            if (prefixNode is not null)
            {
                LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            }
            else
            {
                builder.Append(SafeIdentifier(prefix));
            }
            builder.Append(".ToString()");
            return;
        }
        if (name == "first" && prefixNode is not null && prefixType.StartsWith("Queue<", StringComparison.Ordinal))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Peek()");
            return;
        }
        if (name is "first" or "last" && prefixNode is not null && IsDartEnumerableType(prefixType))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "first" ? ".First()" : ".Last()");
            return;
        }
        if (name is "firstOrNull" or "lastOrNull" && prefixNode is not null)
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "firstOrNull" ? ".FirstOrDefault()" : ".LastOrDefault()");
            return;
        }
        if (name is "firstKey" or "lastKey" && prefixNode is not null &&
            (prefixType.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
             prefixType.StartsWith("SortedDictionary<", StringComparison.Ordinal)))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "firstKey" ? ".firstKey()" : ".lastKey()");
            return;
        }
        if (name == "single" && prefixNode is not null && IsDartEnumerableType(prefixType))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Single()");
            return;
        }
        if (name == "reversed" && prefixNode is not null && IsDartEnumerableType(prefixType))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Reverse()");
            return;
        }
        if (name == "millisecondsSinceEpoch" && prefixNode is not null)
        {
            builder.Append("DartRuntimePrimitives.MillisecondsSinceEpoch(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name is "stackTrace" or "stacktrace" && prefixNode is not null &&
            (prefixType.EndsWith("Exception", StringComparison.Ordinal) ||
             MapType(prefixType).TrimEnd('?').EndsWith("Exception", StringComparison.Ordinal)))
        {
            builder.Append("DartRuntimePrimitives.StackTraceFrom(");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "name" && prefixNode is not null && prefixType.StartsWith('_'))
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return;
        }
        if (name == "viewId" && prefixNode is not null)
        {
            builder.Append("checked((long)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(".viewId)");
            return;
        }
        if (prefixNode is not null && prefixType == "Stopwatch" && name is "elapsedMilliseconds" or "elapsedMicroseconds" or "elapsedTicks")
        {
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(name switch
            {
                "elapsedMilliseconds" => ".ElapsedMilliseconds",
                "elapsedMicroseconds" => ".ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000)",
                _ => ".ElapsedTicks",
            });
            return;
        }

        var mapped = MapPropertyAccessName(node.ElementId, name, prefixType);
        if (name is "isEmpty" or "isNotEmpty" && !IsDartEnumerableType(prefixType) &&
            !prefixType.StartsWith("PriorityQueue<", StringComparison.Ordinal) &&
            !prefixType.StartsWith("HeapPriorityQueue<", StringComparison.Ordinal) &&
            prefixType is not "String" and not "string")
        {
            mapped = SafeIdentifier(name);
        }
        if (mapped.StartsWith("Count", StringComparison.Ordinal) &&
            (prefixType.StartsWith("List<", StringComparison.Ordinal) ||
             prefixType.StartsWith("DartList<", StringComparison.Ordinal) ||
             prefixType.StartsWith("Set<", StringComparison.Ordinal) ||
             prefixType.StartsWith("HashSet<", StringComparison.Ordinal) ||
             prefixType.StartsWith("Map<", StringComparison.Ordinal) ||
             prefixType.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
             prefixType.StartsWith("SortedDictionary<", StringComparison.Ordinal) ||
             prefixType.StartsWith("PriorityQueue<", StringComparison.Ordinal) ||
             prefixType.StartsWith("HeapPriorityQueue<", StringComparison.Ordinal)))
        {
            var wrapComparison = mapped.Contains(' ', StringComparison.Ordinal);
            var nullableCount = node.Text(CoreProperty.@operator) == "?." ||
                prefixNode?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ||
                (prefixNode is not null && DescendantsAndSelf(prefixNode)
                    .Any(item => item.Text(CoreProperty.@operator) == "?."));
            if (wrapComparison && nullableCount)
            {
                var promotedCount = $"__count{node.Offset}";
                builder.Append("(((long?)(");
                if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
                else builder.Append(SafeIdentifier(prefix));
                builder.Append("?.Count)) is { } ").Append(promotedCount).Append(" ? ")
                    .Append(promotedCount).Append(mapped["Count".Length..]).Append(" : (bool?)null)");
                return;
            }
            if (wrapComparison) builder.Append('(');
            builder.Append(nullableCount ? "((long?)(" : "checked((long)(");
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(nullableCount ? "?.Count))" : ".Count))");
            if (wrapComparison) builder.Append(mapped["Count".Length..]).Append(')');
            return;
        }
        if (mapped.StartsWith("Count", StringComparison.Ordinal) &&
            (prefix == "details" || IsDartEnumerableType(prefixType)))
        {
            var wrapComparison = mapped.Contains(' ', StringComparison.Ordinal);
            if (wrapComparison) builder.Append('(');
            if (prefixNode is not null) LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            else builder.Append(SafeIdentifier(prefix));
            builder.Append(".Count()");
            if (wrapComparison) builder.Append(mapped["Count".Length..]).Append(')');
            return;
        }
        if (mapped == "Count" && prefixType is "dynamic" or "object" or "Object")
        {
            builder.Append("FoundationRuntimePorts.Length(").Append(SafeIdentifier(prefix)).Append(')');
            return;
        }
        var dynamicPrefixDispatch = prefixNode is not null &&
            RequiresDynamicPropertyDispatch(prefixNode.StaticType, node.ElementId, name);
        var dynamicPrefixResultType = dynamicPrefixDispatch && !_session.EmittingAssignmentLeft &&
            node.StaticType is { } prefixStaticType && prefixStaticType.TrimEnd('?') is not ("void" or "dynamic" or "Object" or "object")
                ? MapType(prefixStaticType)
                : null;
        if (dynamicPrefixResultType is not null) builder.Append("((").Append(dynamicPrefixResultType).Append(')');
        if (mapped.Contains(' ', StringComparison.Ordinal))
        {
            builder.Append('(');
        }
        var nullAware = node.Text(CoreProperty.@operator) == "?.";
        var nullableValuePrefix = prefixNode is not null && !nullAware &&
            prefixNode.StaticType?.EndsWith("?", StringComparison.Ordinal) == true &&
            IsValueType(MapType(prefixNode.StaticType).TrimEnd('?'));
        if (prefixNode?.StaticType == "Type")
        {
            builder.Append(MapType(prefix));
        }
        else if (nullableValuePrefix)
        {
            builder.Append("DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, prefixNode!, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
        }
        else if (prefixNode is not null)
        {
            if (dynamicPrefixDispatch) builder.Append("((dynamic)");
            LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            if (dynamicPrefixDispatch) builder.Append(')');
        }
        else
        {
            builder.Append(SafeIdentifier(prefix));
        }
        builder.Append(nullAware ? "?." : ".").Append(mapped);
        if (mapped.Contains(' ', StringComparison.Ordinal))
        {
            builder.Append(')');
        }
        if (dynamicPrefixResultType is not null) builder.Append(')');
    }

    private void EmitPropertyAccess(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var target = node.Child(CoreChildRole.targetOffset);
        var name = node.Text(CoreProperty.name) ?? "missing";
        var targetType = (target is null
            ? string.Empty
            : ResolvedExpressionValueType(target) ?? string.Empty).TrimEnd('?');
        if (targetType.Length == 0 && target?.Text(CoreProperty.name) is { } targetMemberName)
        {
            targetType = (AssignmentStorageType(
                _session.ActiveDonorDeclaration ?? declaration,
                targetMemberName,
                null) ?? string.Empty).TrimEnd('?');
        }
        if (name == "mounted" &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "DisposableBuildContext")
        {
            builder.Append("this._state!.mounted");
            return;
        }
        if (name == "renderObject" && target?.Kind == CoreNodeKind.SuperExpression &&
            (_session.ActiveDonorDeclaration ?? declaration).Name == "MultiChildRenderObjectElement")
        {
            builder.Append("base.renderObject");
            return;
        }
        if (name == "millisecondsSinceEpoch" && target is not null)
        {
            builder.Append("DartRuntimePrimitives.MillisecondsSinceEpoch(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "name" && target is not null && targetType.StartsWith("_", StringComparison.Ordinal))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return;
        }
        if (target is not null && name == "of" &&
            DescendantsAndSelf(target).Any(candidate => candidate.Text(CoreProperty.name) == "CreationLocation"))
        {
            builder.Append("global::Doroti.Runtime.CreationLocation.of");
            return;
        }
        if ((target?.StaticType == "Type" || target?.Kind == CoreNodeKind.TypeLiteral) &&
            target.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.TypeLiteral &&
            (target.Text(CoreProperty.name) ??
             DescendantsAndSelf(target).FirstOrDefault(item => item.Kind == CoreNodeKind.NamedType)?.Text(CoreProperty.name)) is { } staticPropertyOwner)
        {
            builder.Append(MapType(staticPropertyOwner).TrimEnd('?'))
                .Append('.').Append(SafeIdentifier(name));
            return;
        }
        if (target is not null && targetType.EndsWith("Constraints", StringComparison.Ordinal) &&
            name is "axis" or "scrollOffset" or "remainingPaintExtent")
        {
            builder.Append("((dynamic)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").").Append(name);
            return;
        }
        if (target is not null && name == "_depth")
        {
            builder.Append("((ViewportNotificationMixin)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(")._depth");
            return;
        }
        if (target is not null && name is "isEmpty" or "isNotEmpty" &&
            (IsDartEnumerableType(targetType) || IsDartEnumerableType(MapType(targetType))))
        {
            var nullAware = node.Text(CoreProperty.@operator) == "?." ||
                target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true;
            if (nullAware)
            {
                var promoted = $"__items{node.Offset}";
                builder.Append('(');
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(" is { } ").Append(promoted).Append(" ? ");
                if (name == "isEmpty") builder.Append('!');
                builder.Append("System.Linq.Enumerable.Any(").Append(promoted).Append(") : (bool?)null)");
                return;
            }
            builder.Append(name == "isEmpty" ? "!System.Linq.Enumerable.Any(" : "System.Linq.Enumerable.Any(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is not null && name == "target" &&
            targetType.StartsWith("WeakReference<", StringComparison.Ordinal))
        {
            builder.Append("DartCoreExtensions.weakTarget(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target?.Text(CoreProperty.name) == "double" &&
            name is "infinity" or "negativeInfinity" or "nan" or "maxFinite" or "minPositive")
        {
            builder.Append(MapDoubleConstant(name));
            return;
        }
        if (target?.Kind == CoreNodeKind.SimpleIdentifier &&
            target.Text(CoreProperty.name) == "innerRect" &&
            name is "top" or "left" or "right" or "bottom")
        {
            builder.Append("DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").").Append(name);
            return;
        }
        if (target is not null && name == "dispatchPointerEvent" &&
            node.StaticType?.Contains("Function", StringComparison.Ordinal) == true)
        {
            builder.Append("((__event) => ");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".dispatchPointerEvent(global::Doroti.Ui.PointerEvent.FromFrameworkEvent(")
                .Append("__event is global::Doroti.Framework.Gestures.PointerDownEvent ? 1L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerUpEvent ? 2L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerCancelEvent ? 3L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerHoverEvent ? 4L : ")
                .Append("__event is global::Doroti.Framework.Gestures.PointerMoveEvent ? 5L : 0L, ")
                .Append("__event.pointer, __event.embedderId, __event.platformData, __event.timeStamp, __event.position, __event.kind, ")
                .Append("__event.orientation, __event.pressure, __event.size, __event.radiusMajor, __event.radiusMinor)))");
            return;
        }
        if (target is not null && TryEmitEnumGetter(builder, target, targetType, name, declaration, package, library, inputPath, diagnostics))
        {
            return;
        }
        if (target is not null && name == "runtimeType")
        {
            builder.Append("DartRuntimePrimitives.RuntimeType(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is not null && name is "stackTrace" or "stacktrace" &&
            (targetType.EndsWith("Exception", StringComparison.Ordinal) ||
             MapType(targetType).TrimEnd('?').EndsWith("Exception", StringComparison.Ordinal)))
        {
            builder.Append("DartRuntimePrimitives.StackTraceFrom(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            if (MapType(targetType).Contains("PlatformException", StringComparison.Ordinal)) builder.Append(".stacktrace");
            builder.Append(')');
            return;
        }
        if (target is not null && TryResolvePromotedMemberOwner(node, targetType, name, out var promotedOwner))
        {
            builder.Append("((").Append(promotedOwner).Append(")");
            if (IsCurrentTypeParameter(targetType)) builder.Append("(object)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").").Append(MapPropertyAccessName(node.ElementId, name, targetType));
            return;
        }
        if (target?.Text(CoreProperty.name) == "math" && name == "pi")
        {
            builder.Append("Dart_mathLibrary.pi");
            return;
        }
        if (target?.Text(CoreProperty.name) == "StackTrace" && name == "current")
        {
            builder.Append("new global::System.Diagnostics.StackTrace(true)");
            return;
        }
        var resolvedPropertyElementId = node.ElementId ?? node.Children
            .FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier &&
                string.Equals(item.Text(CoreProperty.name), name, StringComparison.Ordinal))?
            .ElementId;
        if (name == "disableAnimations" &&
            resolvedPropertyElementId?.Contains("#SemanticsBinding.disableAnimations", StringComparison.Ordinal) == true)
        {
            builder.Append("PlatformDispatcher.instance.accessibilityFeatures.disableAnimations");
            return;
        }
        if (target is not null && targetType == "PointerData" &&
            name is "viewId" or "device" or "pointerIdentifier" or "embedderId")
        {
            builder.Append("checked((long)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(SafeIdentifier(name)).Append(')');
            return;
        }
        if (target is not null && targetType == "DorotiView" && name == "viewId")
        {
            builder.Append("checked((long)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".viewId)");
            return;
        }
        if (target is not null && name == "viewId")
        {
            builder.Append("checked((long)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".viewId)");
            return;
        }
        if (target is not null && targetType is "PointerEvent" or "PointerPanZoomUpdateEvent" &&
            name is "pan" or "localPan" or "panDelta" or "localPanDelta")
        {
            builder.Append("(((PointerPanZoomUpdateEvent?)(object?)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(")!).").Append(name);
            return;
        }
        if (name == "isNaN" && target is not null && targetType is "double" or "num")
        {
            builder.Append("double.IsNaN(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name is "isOdd" or "isEven" && target is not null && targetType is "int" or "double" or "num")
        {
            builder.Append("((checked((long)(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "isOdd" ? ")) & 1L) != 0L)" : ")) & 1L) == 0L)");
            return;
        }
        if (name == "length" && target is not null &&
            DescendantsAndSelf(target).Any(item => item.Kind == CoreNodeKind.SimpleIdentifier && item.Text(CoreProperty.name) == "details"))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Count()");
            return;
        }
        if (name == "eventSource" && target is not null && targetType == "RawKeyEventData")
        {
            builder.Append("((RawKeyEventDataAndroid)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").eventSource");
            return;
        }
        if (name == "kind" && target is not null && targetType == "MouseCursor")
        {
            builder.Append("((SystemMouseCursor)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").kind");
            return;
        }
        if (name == "values" && target is not null && target.StaticType == "Type")
        {
            var enumName = target.Text(CoreProperty.name) ?? "object";
            if (enumName == "FontWeight")
            {
                builder.Append("global::Doroti.Ui.FontWeight.values");
                return;
            }
            builder.Append("System.Enum.GetValues<").Append(MapType(enumName)).Append(">().ToList()");
            return;
        }
        if (name is "characters" or "runes" && target is not null && targetType is "String" or "string")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(name).Append("()");
            return;
        }
        if (name == "isFinite" && target is not null && targetType is "double" or "num")
        {
            if (node.Text(CoreProperty.@operator) == "?." || target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)
            {
                var promoted = $"__finite{node.Offset}";
                builder.Append('(');
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(" is { } ").Append(promoted).Append(" ? double.IsFinite(").Append(promoted).Append(") : (bool?)null)");
                return;
            }
            builder.Append("double.IsFinite(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "isFinite" && target is not null && targetType == "int")
        {
            builder.Append("true");
            return;
        }
        if (name == "isInfinite" && target is not null && targetType is "double" or "num")
        {
            builder.Append("double.IsInfinity(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "isEmpty" && target is not null && targetType == "Size")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".isEmpty");
            return;
        }
        if (name == "isEmpty" && target is not null && targetType == "Rect")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".isEmpty");
            return;
        }
        if (name == "length" && target is not null && targetType is "dynamic" or "object" or "Object")
        {
            builder.Append("FoundationRuntimePorts.Length(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "runtimeType" && target is not null)
        {
            builder.Append("DartRuntimePrimitives.RuntimeType(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "name" && target is not null &&
            (FindGlobalDeclaration(targetType)?.Ast.Kind == CoreNodeKind.EnumDeclaration ||
             FindGlobalDeclaration(StripLibraryPrefix((target.StaticType ?? string.Empty).TrimEnd('?')))?.Ast.Kind == CoreNodeKind.EnumDeclaration))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return;
        }
        if (name == "index" && target is not null)
        {
            var isTypeParameter = declaration.Element.TypeParameters?.Any(item =>
                string.Equals(item.Name, targetType, StringComparison.Ordinal) ||
                string.Equals(item.Name, target.Text(CoreProperty.name), StringComparison.Ordinal)) == true;
            var targetDeclaration = FindGlobalDeclaration(targetType) ??
                FindGlobalDeclaration(StripLibraryPrefix((target.StaticType ?? string.Empty).TrimEnd('?')));
            if (isTypeParameter || targetType is "dynamic" or "object" or "Object" ||
                IsEnumType(targetType) || IsEnumType(target.StaticType ?? string.Empty) ||
                targetDeclaration?.Ast.Kind == CoreNodeKind.EnumDeclaration)
            {
                var nullAware = node.Text(CoreProperty.@operator) == "?." ||
                    target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true;
                builder.Append(nullAware
                    ? "FoundationRuntimePorts.EnumIndexNullable("
                    : "FoundationRuntimePorts.EnumIndex(");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
        }
        if (name == "sign" && target is not null && target.StaticType?.TrimEnd('?') is "int" or "double" or "num")
        {
            builder.Append("Math.Sign(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "length" && target is not null && target.StaticType?.TrimEnd('?') == "String")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(node.Text(CoreProperty.@operator) == "?." || target.StaticType.EndsWith("?", StringComparison.Ordinal)
                ? "?.Length"
                : ".Length");
            return;
        }
        if (name is "isEmpty" or "isNotEmpty" && target is not null && targetType is "String" or "string")
        {
            var nullAware = node.Text(CoreProperty.@operator) == "?.";
            builder.Append('(');
            if (nullAware)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(" is null ? (bool?)null : ");
            }
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Length ").Append(name == "isEmpty" ? "== 0" : "!= 0").Append(')');
            return;
        }
        if (name is "isEmpty" or "isNotEmpty" && target is not null && targetType == "CharacterRange")
        {
            builder.Append('(');
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "isEmpty" ? ".Count == 0)" : ".Count != 0)");
            return;
        }
        if (name == "name" && target is not null && target.StaticType is { Length: > 0 } targetNameType && char.IsUpper(targetNameType.TrimEnd('?')[0]))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return;
        }
        if (name == "first" && target is not null && targetType.StartsWith("Queue<", StringComparison.Ordinal))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Peek()");
            return;
        }
        if (name is "first" or "last" && target is not null && IsDartEnumerableType(targetType))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "first" ? ".First()" : ".Last()");
            return;
        }
        if (name is "firstOrNull" or "lastOrNull" && target is not null)
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "firstOrNull" ? ".FirstOrDefault()" : ".LastOrDefault()");
            return;
        }
        if (name is "firstKey" or "lastKey" && target is not null &&
            (targetType.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
             targetType.StartsWith("SortedDictionary<", StringComparison.Ordinal)))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name == "firstKey" ? ".firstKey()" : ".lastKey()");
            return;
        }
        if (name == "single" && target is not null && IsDartEnumerableType(targetType))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Single()");
            return;
        }
        if (name == "reversed" && target is not null && IsDartEnumerableType(targetType))
        {
            builder.Append("System.Linq.Enumerable.Reverse(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (name == "millisecondsSinceEpoch" && target is not null &&
            (targetType.TrimEnd('?') == "DateTime" || MapType(target.StaticType ?? string.Empty).TrimEnd('?') == "DateTime"))
        {
            builder.Append("new DateTimeOffset(DartRuntimePrimitives.RequireValue(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(")).ToUnixTimeMilliseconds()");
            return;
        }
        if (name == "name" && target is not null && targetType.TrimEnd('?').StartsWith('_'))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return;
        }
        if (target is not null && name is "stackTrace" or "stacktrace" &&
            (targetType.EndsWith("Exception", StringComparison.Ordinal) ||
             MapType(targetType).TrimEnd('?').EndsWith("Exception", StringComparison.Ordinal)))
        {
            builder.Append("DartRuntimePrimitives.StackTraceFrom(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is not null && targetType == "Stopwatch" && name is "elapsedMilliseconds" or "elapsedMicroseconds" or "elapsedTicks")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(name switch
            {
                "elapsedMilliseconds" => ".ElapsedMilliseconds",
                "elapsedMicroseconds" => ".ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000)",
                _ => ".ElapsedTicks",
            });
            return;
        }
        var mapped = MapPropertyAccessName(resolvedPropertyElementId, name, targetType);
        if (target is not null &&
            (node.Text(CoreProperty.@operator) == "?." || target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true) &&
            node.StaticType is { } nullAwareResult &&
            ContainsUnboundTypeParameter(MapType(nullAwareResult)))
        {
            builder.Append("DartRuntimePrimitives.NullAware(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(", __target => __target.").Append(mapped).Append(')');
            return;
        }
        if (name is "isEmpty" or "isNotEmpty" && !IsDartEnumerableType(targetType) &&
            !targetType.StartsWith("PriorityQueue<", StringComparison.Ordinal) &&
            !targetType.StartsWith("HeapPriorityQueue<", StringComparison.Ordinal) &&
            targetType is not "String" and not "string")
        {
            mapped = SafeIdentifier(name);
        }
        if (mapped.StartsWith("Count", StringComparison.Ordinal) && target is not null &&
            (targetType.StartsWith("List<", StringComparison.Ordinal) ||
             targetType.StartsWith("DartList<", StringComparison.Ordinal) ||
             targetType.StartsWith("Set<", StringComparison.Ordinal) ||
             targetType.StartsWith("HashSet<", StringComparison.Ordinal) ||
             targetType.StartsWith("Map<", StringComparison.Ordinal) ||
             targetType.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
             targetType.StartsWith("SortedDictionary<", StringComparison.Ordinal) ||
             targetType.StartsWith("PriorityQueue<", StringComparison.Ordinal) ||
             targetType.StartsWith("HeapPriorityQueue<", StringComparison.Ordinal)))
        {
            var wrapComparison = mapped.Contains(' ', StringComparison.Ordinal);
            var nullableCount = node.Text(CoreProperty.@operator) == "?." ||
                target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ||
                DescendantsAndSelf(target).Any(item => item.Text(CoreProperty.@operator) == "?.");
            if (wrapComparison && nullableCount)
            {
                var promotedCount = $"__count{node.Offset}";
                builder.Append("(((long?)(");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append("?.Count)) is { } ").Append(promotedCount).Append(" ? ")
                    .Append(promotedCount).Append(mapped["Count".Length..]).Append(" : (bool?)null)");
                return;
            }
            if (wrapComparison) builder.Append('(');
            builder.Append(nullableCount ? "((long?)(" : "checked((long)(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(nullableCount ? "?.Count))" : ".Count))");
            if (wrapComparison) builder.Append(mapped["Count".Length..]).Append(')');
            return;
        }
        if (mapped.StartsWith("Count", StringComparison.Ordinal) && target is not null &&
            IsDartEnumerableType(targetType))
        {
            var wrapComparison = mapped.Contains(' ', StringComparison.Ordinal);
            if (wrapComparison) builder.Append('(');
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Count()");
            if (wrapComparison) builder.Append(mapped["Count".Length..]).Append(')');
            return;
        }
        var dynamicPropertyDispatch = target is not null &&
            RequiresDynamicPropertyDispatch(target.StaticType, resolvedPropertyElementId, name);
        var dynamicPropertyResultType = dynamicPropertyDispatch && !_session.EmittingAssignmentLeft &&
            node.StaticType is { } propertyStaticType && propertyStaticType.TrimEnd('?') is not ("void" or "dynamic" or "Object" or "object")
                ? MapType(propertyStaticType)
                : null;
        if (dynamicPropertyResultType is not null) builder.Append("((").Append(dynamicPropertyResultType).Append(')');
        var nullableBooleanExpression = mapped.Contains(' ', StringComparison.Ordinal) &&
            target?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true;
        if (nullableBooleanExpression) builder.Append("((bool?)(");
        if (mapped.Contains(' ', StringComparison.Ordinal))
        {
            builder.Append('(');
        }
        if (target is not null)
        {
            var nullAware = node.Text(CoreProperty.@operator) == "?.";
            var flattenedMixinReceiver = target.Kind == CoreNodeKind.SuperExpression &&
                AppliedMixinDeclarations(_session.ActiveDonorDeclaration ?? declaration)
                    .Any(mixin => mixin.Members.Any(member =>
                        !member.IsStatic && string.Equals(member.Name, name, StringComparison.Ordinal)));
            var promotedLocalValueTarget = !nullAware && target.Kind == CoreNodeKind.SimpleIdentifier &&
                HasNullableValueStorage(target, _session.ActiveDonorDeclaration ?? declaration);
            var nullableValueTarget = !nullAware &&
                target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true &&
                IsValueType(MapType(target.StaticType).TrimEnd('?'));
            if (flattenedMixinReceiver)
            {
                builder.Append(_session.ExplicitThisExpression ?? "this");
            }
            else if (promotedLocalValueTarget)
            {
                builder.Append("DartRuntimePrimitives.RequireValue(")
                    .Append(EmittedLocalIdentifier(target, target.Text(CoreProperty.name) ?? "value"))
                    .Append(')');
            }
            else if (nullableValueTarget)
            {
                builder.Append("DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else
            {
                if (dynamicPropertyDispatch) builder.Append("((dynamic)");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                if (dynamicPropertyDispatch) builder.Append(')');
            }
            builder.Append(nullAware || !nullableValueTarget &&
                target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ? "?." : ".");
        }
        builder.Append(mapped);
        if (node.ElementId is { } propertyElementId && _semanticIndex.IsEnumGetter(propertyElementId))
        {
            builder.Append("()");
        }
        if (mapped.Contains(' ', StringComparison.Ordinal))
        {
            builder.Append(')');
        }
        if (nullableBooleanExpression) builder.Append("))");
        if (dynamicPropertyResultType is not null) builder.Append(')');
    }

    private void EmitIndexExpression(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var expressions = node.Children.Where(item => item.Category == "expression").ToArray();
        if (expressions.Length < 2)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "index-expression-shape", "Provide the typed target and index operands.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        var targetType = expressions[0].StaticType ?? string.Empty;
        if (!_session.EmittingAssignmentLeft && targetType.TrimEnd('?') is "dynamic" or "object" or "Object")
        {
            builder.Append("FoundationRuntimePorts.Index(");
            LowerExpression(builder, expressions[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(", ");
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var isMapGet = !_session.EmittingAssignmentLeft && (
            targetType.StartsWith("Map<", StringComparison.Ordinal) ||
            targetType.StartsWith("Dictionary<", StringComparison.Ordinal) ||
            targetType.Contains("Map<", StringComparison.Ordinal) ||
            targetType.Contains("Dictionary<", StringComparison.Ordinal));
        var mappedResultType = MapType(node.StaticType ?? string.Empty);
        if (isMapGet && mappedResultType.EndsWith("?", StringComparison.Ordinal) &&
            IsValueType(mappedResultType.TrimEnd('?')))
        {
            builder.Append("DartCollectionRuntime.NullableMapValue<")
                .Append(mappedResultType.TrimEnd('?')).Append(">(");
            LowerExpression(builder, expressions[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(", ");
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        LowerExpression(builder, expressions[0], declaration, package, library, inputPath, diagnostics);
        if (isMapGet)
        {
            builder.Append(".GetValueOrDefault(");
            // Dart maps accept nullable lookup keys and return null/default when no
            // matching key exists. DartMap.GetValueOrDefault(object?) preserves that
            // contract; a generated null assertion here changes valid `map[key?]`
            // reads into runtime failures.
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        builder.Append('[');
        var isSequenceIndex = targetType.StartsWith("List<", StringComparison.Ordinal) ||
            targetType.StartsWith("IList<", StringComparison.Ordinal) ||
            targetType.StartsWith("string", StringComparison.Ordinal) || targetType == "String" ||
            targetType.TrimEnd('?') == "Vector4" ||
            targetType.EndsWith("[]", StringComparison.Ordinal);
        if (isSequenceIndex && expressions[1].StaticType == "int")
        {
            builder.Append("(int)(");
        }
        var nullableIndex = expressions[1].StaticType?.EndsWith("?", StringComparison.Ordinal) == true ||
            expressions[1].Kind == CoreNodeKind.SimpleIdentifier &&
            DescendantsAndSelf(declaration.Ast)
                .Where(item => item.Kind == CoreNodeKind.VariableDeclarationList)
                .Any(list => list.Children.Any(variable =>
                        variable.Kind == CoreNodeKind.VariableDeclaration &&
                        variable.Text(CoreProperty.name) == expressions[1].Text(CoreProperty.name)) &&
                    list.Children.FirstOrDefault(item => item.Category == "type") is { } localType &&
                    MapTypeFromAst(localType).EndsWith("?", StringComparison.Ordinal));
        if (nullableIndex)
        {
            var indexType = MapType(expressions[1].StaticType ?? "object").TrimEnd('?');
            builder.Append(IsValueType(indexType)
                ? "DartRuntimePrimitives.RequireValue("
                : "DartRuntimePrimitives.RequireReference(");
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
        }
        else
        {
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
        }
        if (isSequenceIndex && expressions[1].StaticType == "int")
        {
            builder.Append(')');
        }
        builder.Append(']');
        if (targetType == "String") builder.Append(".ToString()");
    }

    private bool IsTypeParameter(string? type, CoreResolvedDeclaration declaration)
    {
        if (string.IsNullOrEmpty(type))
        {
            return false;
        }
        var name = StripLibraryPrefix(type.TrimEnd('?'));
        return declaration.Element.TypeParameters?.Any(parameter => parameter.Name == name) ?? false;
    }

    private void EmitArguments(
        CsSyntaxBuilder builder,
        CoreAstNode? arguments,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        bool preserveNames = true,
        CoreResolvedParameter[]? expectedParameters = null,
        string[]? expectedArgumentTypes = null,
        string? invocationName = null,
        bool castDynamicArguments = false,
        bool nullAsGenericDefault = false)
    {
        if (arguments is null)
        {
            return;
        }
        var values = arguments.Children.Where(item => item.Category == "expression").ToArray();
        var positionalIndex = 0;
        var emittedNamedArgument = false;
        for (var index = 0; index < values.Length; index++)
        {
            var argumentValue = values[index].Kind == CoreNodeKind.NamedExpression
                ? values[index].Children.FirstOrDefault(item => item.Category == "expression") ?? values[index]
                : values[index];
            var namedArgument = values[index].Kind == CoreNodeKind.NamedExpression
                ? values[index].Text(CoreProperty.name) ??
                    DescendantsAndSelf(values[index])
                        .FirstOrDefault(item => item.Kind == CoreNodeKind.Label)?
                        .Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier)?
                        .Text(CoreProperty.name)
                : null;
            var expectedParameter = namedArgument is not null
                ? expectedParameters?.FirstOrDefault(parameter => string.Equals(parameter.Name, namedArgument, StringComparison.Ordinal))
                : expectedParameters?.Where(parameter => parameter.Kind is not "optional-named" and not "required-named")
                    .ElementAtOrDefault(positionalIndex++);
            if (index > 0)
            {
                builder.Append(", ");
            }
            if (preserveNames && namedArgument is not null)
            {
                builder.Append(SafeIdentifier(namedArgument)).Append(": ");
                emittedNamedArgument = true;
            }
            else if (preserveNames && emittedNamedArgument && expectedParameter is not null)
            {
                // Dart 3 permits positional arguments after named arguments.
                // C# does not, so name the later positional argument while
                // preserving source evaluation order.
                builder.Append(SafeIdentifier(expectedParameter.Name)).Append(": ");
            }
            if (nullAsGenericDefault &&
                (argumentValue.Kind == CoreNodeKind.NullLiteral || argumentValue.StaticType?.TrimEnd('?') == "Null"))
            {
                builder.Append("default");
                continue;
            }
            if (argumentValue.Kind == CoreNodeKind.NullLiteral &&
                invocationName is not null &&
                (invocationName.StartsWith("AsyncSnapshot<", StringComparison.Ordinal) ||
                 invocationName.Contains("RawRadio", StringComparison.Ordinal) ||
                 invocationName.Contains("RadioGroup", StringComparison.Ordinal)) &&
                (_session.ActiveDonorDeclaration ?? declaration).Element.TypeParameters?.FirstOrDefault() is { } nullTypeParameter)
            {
                builder.Append("default(").Append(SafeIdentifier(nullTypeParameter.Name)).Append(')');
                continue;
            }
            if (argumentValue.Text(CoreProperty.name) == "instantiateImageCodecFromBuffer")
            {
                builder.Append("(__buffer, __allowUpscaling, __cacheHeight, __cacheWidth) => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("(__buffer, __cacheWidth, __cacheHeight, __allowUpscaling)");
                continue;
            }
            if (namedArgument == "decode" && argumentValue.StaticType is { } decoderType)
            {
                if (decoderType.Contains("bool", StringComparison.Ordinal) && decoderType.Contains("int?", StringComparison.Ordinal))
                {
                    builder.Append("(ImmutableBuffer __buffer) => ");
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                    builder.Append("(__buffer, false, null, null)");
                    continue;
                }
                if (decoderType.Contains("TargetImageSize", StringComparison.Ordinal))
                {
                    builder.Append("(ImmutableBuffer __buffer) => ");
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                    builder.Append("(__buffer, null)");
                    continue;
                }
            }
            var argumentName = argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier
                ? argumentValue.Text(CoreProperty.name)
                : null;
            if (argumentName == "showOnScreen" &&
                argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
            {
                builder.Append("() => ");
                EmitFunctionTearOffReceiver(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("showOnScreen()");
                continue;
            }
            if (argumentName == "_marksConflictsInMergeGroup")
            {
                builder.Append("__fragments => ");
                EmitFunctionTearOffReceiver(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("_marksConflictsInMergeGroup(__fragments)");
                continue;
            }
            var expectedArgumentType = namedArgument is not null && expectedParameter is not null
                ? MapType(expectedParameter.Type)
                : expectedArgumentTypes?.ElementAtOrDefault(index) ??
                    (expectedParameter is not null ? MapType(expectedParameter.Type) : string.Empty);
            if (argumentValue.Kind == CoreNodeKind.NullLiteral &&
                (expectedArgumentType.EndsWith("?", StringComparison.Ordinal) ||
                  expectedArgumentType is "object" or "dynamic" or "object?" or "dynamic?"))
            {
                // Nullable constructor and method parameters accept Dart null
                // directly. Stripping nullable value types before the later
                // dynamic-boundary cast turns a valid `int?` null into
                // `((long)(object)null)`, which fails before the callee runs.
                builder.Append(ContainsUnboundTypeParameter(expectedArgumentType) ? "default" : "null");
                continue;
            }
            if (invocationName is "maybePop" or "pop" or "restorablePushNamed" or "pushNamed" &&
                IsUnboundTypeParameterName(expectedArgumentType.TrimEnd('?')))
            {
                expectedArgumentType = expectedArgumentType.EndsWith("?", StringComparison.Ordinal)
                    ? "object?"
                    : "object";
            }
            if (invocationName == "complete" && IsUnboundTypeParameterName(expectedArgumentType.TrimEnd('?')))
            {
                expectedArgumentType = expectedArgumentType.EndsWith("?", StringComparison.Ordinal) ? "object?" : "object";
            }
            var resolvedArgumentStorageType = argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true
                ? null
                : ResolvedExpressionValueType(argumentValue);
            var actualArgumentType = MapType(resolvedArgumentStorageType ?? argumentValue.StaticType ?? string.Empty);
            var expectedDelegateType = expectedArgumentType.Replace("global::System.", string.Empty, StringComparison.Ordinal);
            var actualDelegateType = actualArgumentType.Replace("global::System.", string.Empty, StringComparison.Ordinal);
            if (IsValueType(expectedArgumentType) &&
                !expectedArgumentType.EndsWith("?", StringComparison.Ordinal) &&
                actualArgumentType == expectedArgumentType + "?")
            {
                builder.Append("DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (expectedArgumentType is "Widget" or "Widget?" or
                    "global::Doroti.Framework.Widgets.Widget" or
                    "global::Doroti.Framework.Widgets.Widget?" &&
                actualArgumentType is not ("Widget" or "Widget?" or
                    "global::Doroti.Framework.Widgets.Widget" or
                    "global::Doroti.Framework.Widgets.Widget?") &&
                actualArgumentType.TrimEnd('?') is
                    "PreferredSizeWidget" or "ObstructingPreferredSizeWidget" or
                    "global::Doroti.Framework.Widgets.PreferredSizeWidget" or
                    "global::Doroti.Framework.Cupertino.ObstructingPreferredSizeWidget")
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (invocationName == "setListener" &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal))
            {
                // dart:ui ChannelBuffers owns an asynchronous listener. Resolve
                // this before the general Func-to-Action adapter so an async
                // listener remains an async lambda instead of an Action body
                // containing an illegal await expression.
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                continue;
            }
            if (invocationName == "setMessageHandler" &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                expectedDelegateType.Contains("ByteData", StringComparison.Ordinal) &&
                expectedDelegateType.StartsWith("Func<", StringComparison.Ordinal))
            {
                // BinaryMessenger's byte envelope is more specific than the
                // analyzer's dynamic EventChannel callback result. Lower the
                // closure in the resolved receiver contract so null replies
                // remain Future<ByteData?> rather than Future<object>.
                builder.Append("((").Append(expectedArgumentType).Append(')');
                var previousFunctionReturnType = _session.ActiveFunctionReturnType;
                _session.ActiveFunctionReturnType = TryGetGenericTypeArguments(
                    expectedDelegateType.TrimEnd('?'), out var messageHandlerArguments) &&
                    messageHandlerArguments.Length > 0
                        ? messageHandlerArguments[^1]
                        : null;
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ActiveFunctionReturnType = previousFunctionReturnType;
                }
                builder.Append(')');
                continue;
            }
            if (invocationName == "_requestTraversalFocusInDirection" && index == 2)
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<FocusScopeNode>(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (invocationName == "_updateParentData" && index == 0)
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<ParentDataWidget<global::Doroti.Framework.Rendering.ParentData>>(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (actualDelegateType.Contains("TickerFuture", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                expectedDelegateType.TrimEnd('?') is "" or "Action")
            {
                builder.Append("() => { _ = ((").Append(actualArgumentType).Append(')');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(default); }");
                continue;
            }
            if (argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                expectedDelegateType.TrimEnd('?') is "Action" ||
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                expectedDelegateType.StartsWith("Action<", StringComparison.Ordinal))
            {
                string expectedClrDelegateType;
                if (actualDelegateType.TrimEnd('?') == "Action" || actualDelegateType.StartsWith("Action<", StringComparison.Ordinal))
                {
                    expectedClrDelegateType = actualArgumentType.TrimEnd('?');
                }
                else if (actualDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                    TryGetGenericTypeArguments(actualDelegateType.TrimEnd('?'), out var actualCallbackArguments))
                {
                    expectedClrDelegateType = actualCallbackArguments.Length <= 1
                        ? "global::System.Action"
                        : $"global::System.Action<{string.Join(", ", actualCallbackArguments[..^1])}>";
                }
                else
                {
                    expectedClrDelegateType = expectedDelegateType.StartsWith("Action", StringComparison.Ordinal)
                        ? "global::System." + expectedDelegateType.TrimEnd('?')
                        : expectedArgumentType;
                }
                builder.Append("((").Append(expectedClrDelegateType).Append(')');
                var previousContextualReturn = _session.ContextualLambdaReturnType;
                _session.ContextualLambdaReturnType = "void";
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ContextualLambdaReturnType = previousContextualReturn;
                }
                builder.Append(')');
                continue;
            }
            if (argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                (invocationName == "whenComplete" ||
                 invocationName is "firstWhere" or "putIfAbsent" && namedArgument == "orElse" ||
                 invocationName == "putIfAbsent" && index == 1))
            {
                var callbackBlock = argumentValue.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody)?
                    .Child(CoreChildRole.blockOffset);
                if (invocationName is "firstWhere" or "putIfAbsent" &&
                    callbackBlock is not null &&
                    !callbackBlock.Children.Any(item => item.Category == "statement"))
                {
                    builder.Append("() => default!");
                    continue;
                }
                var previousContextualReturn = _session.ContextualLambdaReturnType;
                _session.ContextualLambdaReturnType = invocationName == "whenComplete" ? "void" : "object";
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ContextualLambdaReturnType = previousContextualReturn;
                }
                continue;
            }
            if ((argumentValue.Kind == CoreNodeKind.NullLiteral ||
                 argumentValue.StaticType?.TrimEnd('?') == "Null") &&
                (ContainsUnboundTypeParameter(expectedArgumentType) ||
                 invocationName == "onChanged" ||
                 expectedArgumentType.Length == 0 &&
                    ((_session.ActiveDonorDeclaration ?? declaration).Element.TypeParameters?.Length ?? 0) > 0 ||
                 ContainsUnboundTypeParameter(invocationName ?? string.Empty)))
            {
                builder.Append("default");
                continue;
            }
            if (invocationName == "any" &&
                argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
            {
                builder.Append("__item => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("(__item)");
                continue;
            }
            if (invocationName == "registerNumericServiceExtension" &&
                namedArgument is "getter" or "setter" &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression)
            {
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                continue;
            }
            if (invocationName == "registerBoolServiceExtension" &&
                namedArgument is "getter" or "setter" &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression)
            {
                var callbackType = namedArgument == "getter"
                    ? "Func<Future<bool>>"
                    : "Func<bool, Future>";
                builder.Append("((").Append(callbackType).Append(')');
                var previousFunctionReturnType = _session.ActiveFunctionReturnType;
                _session.ActiveFunctionReturnType = namedArgument == "getter" ? "Future<bool>" : "Future";
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ActiveFunctionReturnType = previousFunctionReturnType;
                }
                builder.Append(')');
                continue;
            }
            if (invocationName == "catchError" &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                (actualDelegateType.StartsWith("Func<", StringComparison.Ordinal) ||
                 actualDelegateType == "Action" ||
                 actualDelegateType.StartsWith("Action<", StringComparison.Ordinal)))
            {
                // Future.catchError has Action and Func overloads in the runtime
                // port. Preserve the analyzer-resolved callback contract so a
                // block lambda is neither ambiguous nor lowered with its owning
                // method's return type.
                builder.Append("((").Append(actualArgumentType).Append(')');
                var previousFunctionReturnType = _session.ActiveFunctionReturnType;
                _session.ActiveFunctionReturnType = actualDelegateType == "Action" ||
                    actualDelegateType.StartsWith("Action<", StringComparison.Ordinal)
                        ? "void"
                        : TryGetGenericTypeArguments(actualDelegateType.TrimEnd('?'), out var callbackArguments) &&
                          callbackArguments.Length > 0
                            ? callbackArguments[^1]
                            : null;
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ActiveFunctionReturnType = previousFunctionReturnType;
                }
                builder.Append(')');
                continue;
            }
            if (argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                actualArgumentType.Contains("Future<Null>", StringComparison.Ordinal))
            {
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                continue;
            }
            if (argumentName == "decodeResize" && argumentValue.StaticType?.Contains("bool", StringComparison.Ordinal) == true)
            {
                // DecoderBufferCallback orders its named Dart parameters as
                // allowUpscaling/cacheWidth/cacheHeight in the CLR delegate.
                // The local Dart function declares cacheWidth/cacheHeight/
                // allowUpscaling, so adapt by resolved role instead of passing
                // positional delegate slots through unchanged.
                builder.Append("((Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)((__buffer, __allowUpscaling, __cacheWidth, __cacheHeight) => decodeResize(__buffer, __cacheWidth, __cacheHeight, __allowUpscaling)))");
                continue;
            }
            if (argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                expectedDelegateType.TrimEnd('?') == "Action" &&
                argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
            {
                // Dart permits a method with only optional parameters to be
                // used as a zero-argument callback. An explicit invocation
                // also lets C# discard a value-returning callback result.
                builder.Append("() => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("()");
                continue;
            }
            if (argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                (expectedDelegateType == "Action" || expectedDelegateType.StartsWith("Action<", StringComparison.Ordinal)) &&
                CallableReturnsValue(argumentValue, declaration))
            {
                var callbackArguments = expectedDelegateType == "Action"
                    ? []
                    : TryGetGenericTypeArguments(expectedDelegateType, out var callbackTypes)
                        ? Enumerable.Range(0, callbackTypes.Length).Select(argumentIndex => $"__arg{argumentIndex}").ToArray()
                        : [];
                builder.Append('(').Append(string.Join(", ", callbackArguments)).Append(") => { _ = ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append('(').Append(string.Join(", ", callbackArguments)).Append("); }");
                continue;
            }
            if (expectedDelegateType.StartsWith("Action<", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(expectedDelegateType, out var actionArguments))
            {
                var lambdaNames = Enumerable.Range(0, actionArguments.Length).Select(argumentIndex => $"__arg{argumentIndex}").ToArray();
                builder.Append('(').Append(string.Join(", ", lambdaNames)).Append(") => { _ = ((")
                    .Append(actualArgumentType).Append(")");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(").Append(string.Join(", ", lambdaNames)).Append("); }");
                continue;
            }
            if (expectedDelegateType.StartsWith("Action<", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Action<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(expectedDelegateType, out var expectedActionTypes) &&
                TryGetGenericTypeArguments(actualDelegateType, out var actualActionTypes) &&
                expectedActionTypes.Length == actualActionTypes.Length &&
                !string.Equals(expectedArgumentType, actualArgumentType, StringComparison.Ordinal))
            {
                var lambdaNames = Enumerable.Range(0, expectedActionTypes.Length)
                    .Select(argumentIndex => $"__arg{argumentIndex}").ToArray();
                builder.Append('(').Append(string.Join(", ", lambdaNames)).Append(") => ((")
                    .Append(actualArgumentType).Append(')');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(");
                for (var argumentIndex = 0; argumentIndex < lambdaNames.Length; argumentIndex++)
                {
                    if (argumentIndex > 0) builder.Append(", ");
                    if (string.Equals(expectedActionTypes[argumentIndex], actualActionTypes[argumentIndex], StringComparison.Ordinal))
                    {
                        builder.Append(lambdaNames[argumentIndex]);
                    }
                    else
                    {
                        builder.Append("DartRuntimePrimitives.ConvertValue<")
                            .Append(actualActionTypes[argumentIndex].TrimEnd('?')).Append(">(")
                            .Append(lambdaNames[argumentIndex]).Append(')');
                    }
                }
                builder.Append(')');
                continue;
            }
            if (expectedDelegateType.TrimEnd('?') == "Action" &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal))
            {
                builder.Append("() => { _ = ((").Append(actualArgumentType).Append(")");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(); }");
                continue;
            }
            if ((expectedDelegateType.TrimEnd('?') == "Action" ||
                 invocationName is "onCloseRequested" or "scheduleMicrotask" or "buttonBuilder" ||
                 namedArgument == "onPressed" ||
                 declaration.Name == "_RawMenuAnchorState" && argumentName == "close") &&
                actualDelegateType.StartsWith("Action<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(actualDelegateType.TrimEnd('?'), out var optionalActionArguments))
            {
                // Dart can tear off a method whose parameters are all optional
                // as VoidCallback. C# method-group conversion cannot omit those
                // parameters, so preserve the zero-argument call explicitly.
                builder.Append("() => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append('(').Append(string.Join(", ", optionalActionArguments.Select(_ => "default"))).Append(')');
                continue;
            }
            if (invocationName == "then" &&
                argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                (actualDelegateType == "Action" || actualDelegateType.StartsWith("Action<", StringComparison.Ordinal)))
            {
                // Future.then<void> selects the Action overload. Adapting an
                // already-void tear-off to Func by appending a synthetic value
                // return makes the lambda invalid when overload resolution
                // correctly chooses Action.
                builder.Append("((").Append(actualArgumentType).Append(')');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                expectedDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Action<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(expectedDelegateType, out var expectedActionArguments) &&
                TryGetGenericTypeArguments(actualDelegateType, out var actualActionArguments) &&
                expectedActionArguments.Length == actualActionArguments.Length + 1)
            {
                var lambdaNames = Enumerable.Range(0, actualActionArguments.Length)
                    .Select(argumentIndex => $"__arg{argumentIndex}")
                    .ToArray();
                builder.Append('(').Append(string.Join(", ", lambdaNames)).Append(") => { ((")
                    .Append(actualArgumentType).Append(')');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(").Append(string.Join(", ", lambdaNames)).Append("); return default!; }");
                continue;
            }
            if (argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier &&
                expectedDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                actualDelegateType is "Action" or "Action?" &&
                TryGetGenericTypeArguments(expectedDelegateType, out var expectedActionResult) &&
                expectedActionResult.Length == 1)
            {
                builder.Append("() => { ((Action)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(")(); return default!; }");
                continue;
            }
            if (expectedParameters is not null &&
                expectedDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(expectedDelegateType, out var expectedFuncArguments) &&
                TryGetGenericTypeArguments(actualDelegateType, out var actualFuncArguments) &&
                expectedFuncArguments.Length == actualFuncArguments.Length &&
                expectedFuncArguments.Length > 1 &&
                string.Equals(expectedFuncArguments[^1], actualFuncArguments[^1], StringComparison.Ordinal) &&
                !string.Equals(expectedArgumentType, actualArgumentType, StringComparison.Ordinal))
            {
                var lambdaNames = Enumerable.Range(0, expectedFuncArguments.Length - 1)
                    .Select(argumentIndex => $"__arg{argumentIndex}")
                    .ToArray();
                builder.Append('(').Append(expectedArgumentType).Append(")((")
                    .Append(string.Join(", ", lambdaNames)).Append(") => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append('(');
                for (var argumentIndex = 0; argumentIndex < lambdaNames.Length; argumentIndex++)
                {
                    if (argumentIndex > 0) builder.Append(", ");
                    if (string.Equals(expectedFuncArguments[argumentIndex], actualFuncArguments[argumentIndex], StringComparison.Ordinal))
                    {
                        builder.Append(lambdaNames[argumentIndex]);
                    }
                    else
                    {
                        builder.Append("DartRuntimePrimitives.ConvertValue<")
                            .Append(actualFuncArguments[argumentIndex].TrimEnd('?')).Append(">(")
                            .Append(lambdaNames[argumentIndex]).Append(')');
                    }
                }
                builder.Append("))");
                continue;
            }
            if (argumentName == "_computeDryLayout" &&
                argumentValue.StaticType?.Contains("BoxConstraints", StringComparison.Ordinal) == true)
            {
                builder.Append("(BoxConstraints __constraints) => ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("(__constraints)");
                continue;
            }
            if (argumentName == "_animationStatusListener")
            {
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                continue;
            }
            if (expectedArgumentType.Contains("AnimationStatusListener", StringComparison.Ordinal) &&
                actualDelegateType.StartsWith("Func<", StringComparison.Ordinal))
            {
                builder.Append("(global::Doroti.Framework.Animation.AnimationStatus __status) => { _ = ");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append("(__status); }");
                continue;
            }
            if (expectedParameters is not null &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                expectedDelegateType.StartsWith("Func<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(expectedDelegateType.TrimEnd('?'), out var contextualFuncArguments) &&
                contextualFuncArguments.Length > 0)
            {
                builder.Append("((").Append(expectedArgumentType).Append(')');
                var previousContextualReturn = _session.ContextualLambdaReturnType;
                _session.ContextualLambdaReturnType = contextualFuncArguments[^1];
                try
                {
                    LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                }
                finally
                {
                    _session.ContextualLambdaReturnType = previousContextualReturn;
                }
                builder.Append(')');
                continue;
            }
            if (expectedParameters is not null &&
                argumentValue.Kind == CoreNodeKind.FunctionExpression &&
                argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
            {
                builder.Append("((").Append(MapType(argumentValue.StaticType)).Append(")");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                continue;
            }
            if (expectedParameters is not null &&
                (argumentValue.Kind is CoreNodeKind.SimpleIdentifier or CoreNodeKind.PropertyAccess or CoreNodeKind.PrefixedIdentifier) &&
                argumentValue.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)
            {
                builder.Append('(').Append(MapType(argumentValue.StaticType)).Append(')');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                continue;
            }
            var restoresNonConstValueDefault = argumentName is not null && declaration.Members
                .Where(member => member.Kind == "constructor")
                .SelectMany(member => member.Element.Parameters ?? [])
                .Any(parameter => parameter.Name == argumentName && NeedsNonConstValueDefault(parameter));
            if (argumentName?.StartsWith("lockMode", StringComparison.Ordinal) == true ||
                argumentName == "previousState" || restoresNonConstValueDefault)
            {
                builder.Append("DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, values[index], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (expectedArgumentType.Length > 0 &&
                IsValueType(expectedArgumentType) && !expectedArgumentType.EndsWith("?", StringComparison.Ordinal) &&
                (actualArgumentType == expectedArgumentType + "?" ||
                  NeedsNullableValuePromotion(argumentValue, _session.ActiveDonorDeclaration ?? declaration) ||
                  HasNullableValueStorage(argumentValue, _session.ActiveDonorDeclaration ?? declaration)))
            {
                builder.Append("DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (expectedArgumentType == "int" && actualArgumentType == "long")
            {
                builder.Append("checked((int)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (expectedArgumentType.Length == 0 && actualArgumentType == "long" &&
                invocationName is "insert" or "removeAt")
            {
                builder.Append("checked((int)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (expectedArgumentType == "long" && actualArgumentType == "ulong")
            {
                builder.Append("checked((long)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (invocationName == "dispatchLocalesChanged" &&
                DescendantsAndSelf(argumentValue).Any(item => item.Text(CoreProperty.name) == "locales"))
            {
                // dart:ui exposes an immutable view while the Flutter framework
                // API deliberately takes a mutable List snapshot.
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(".ToList()");
            }
            else if (invocationName == "_updateResolvedLocale" &&
                DescendantsAndSelf(argumentValue).Any(item => item.Text(CoreProperty.name) == "locales"))
            {
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(".ToList()");
            }
            else if (invocationName == "registerForRestoration" && index == 0)
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<RestorableProperty<object>>(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (invocationName is "addRenderView" or "removeRenderView" &&
                DescendantsAndSelf(argumentValue).Any(item => item.Text(CoreProperty.name) == "renderObject"))
            {
                // RenderObjectElement.renderObject is typed at its base contract,
                // but _RawViewElement's Dart invariant guarantees RenderView.
                builder.Append("DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderView>(");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if (argumentValue.Kind == CoreNodeKind.NullLiteral &&
                (IsUnboundTypeParameterName(expectedArgumentType.TrimEnd('?')) ||
                 IsTypeParameter(expectedArgumentType, _session.ActiveDonorDeclaration ?? declaration) ||
                 Regex.IsMatch(expectedArgumentType, @"^[A-Z]\??$", RegexOptions.CultureInvariant) ||
                 ContainsUnboundTypeParameter(invocationName ?? string.Empty)))
            {
                builder.Append("default");
            }
            else if (expectedArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) &&
                argumentValue.Kind == CoreNodeKind.MethodInvocation &&
                argumentValue.Text(CoreProperty.name) == "cast")
            {
                // Dart List.cast keeps List semantics. The CLR compatibility
                // extension exposes IEnumerable<T>, so restore the resolved
                // analyzer result type at a List-typed invocation boundary.
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(".ToList()");
            }
            else if (TryGetGenericTypeArguments(expectedArgumentType.TrimEnd('?'), out var expectedCollectionArguments) &&
                TryGetGenericTypeArguments(actualArgumentType.TrimEnd('?'), out _) &&
                expectedArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) &&
                actualArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) &&
                !ExpressionProducesFuture(argumentValue) &&
                expectedArgumentType.TrimEnd('?') != actualArgumentType.TrimEnd('?') &&
                !expectedCollectionArguments.Any(IsUnboundTypeParameterName))
            {
                var wrapAwaitedCollection = argumentValue.Kind == CoreNodeKind.AwaitExpression;
                if (wrapAwaitedCollection) builder.Append('(');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                if (wrapAwaitedCollection) builder.Append(')');
                builder.Append(".Cast<").Append(expectedCollectionArguments[0]).Append(">().ToList()");
            }
            else if (expectedArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) &&
                actualArgumentType.TrimEnd('?').StartsWith("IReadOnlyList<", StringComparison.Ordinal) &&
                !ExpressionProducesFuture(argumentValue) &&
                TryGetGenericTypeArguments(expectedArgumentType.TrimEnd('?'), out var expectedListArguments) &&
                TryGetGenericTypeArguments(actualArgumentType.TrimEnd('?'), out var actualReadOnlyArguments) &&
                expectedListArguments.Length == 1 && actualReadOnlyArguments.Length == 1)
            {
                var wrapAwaitedCollection = argumentValue.Kind == CoreNodeKind.AwaitExpression;
                if (wrapAwaitedCollection) builder.Append('(');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                if (wrapAwaitedCollection) builder.Append(')');
                if (!string.Equals(expectedListArguments[0], actualReadOnlyArguments[0], StringComparison.Ordinal))
                {
                    builder.Append(".Cast<").Append(expectedListArguments[0]).Append(">()");
                }
                builder.Append(".ToList()");
            }
            else if (expectedArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) &&
                actualArgumentType.TrimEnd('?').StartsWith("IEnumerable<", StringComparison.Ordinal) &&
                !ExpressionProducesFuture(argumentValue) &&
                TryGetGenericTypeArguments(expectedArgumentType.TrimEnd('?'), out var expectedEnumerableListArguments) &&
                TryGetGenericTypeArguments(actualArgumentType.TrimEnd('?'), out var actualEnumerableArguments) &&
                expectedEnumerableListArguments.Length == 1 && actualEnumerableArguments.Length == 1)
            {
                var wrapAwaitedCollection = argumentValue.Kind == CoreNodeKind.AwaitExpression;
                if (wrapAwaitedCollection) builder.Append('(');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                if (wrapAwaitedCollection) builder.Append(')');
                if (!string.Equals(expectedEnumerableListArguments[0], actualEnumerableArguments[0], StringComparison.Ordinal))
                {
                    builder.Append(".Cast<").Append(expectedEnumerableListArguments[0]).Append(">()");
                }
                builder.Append(".ToList()");
            }
            else if (expectedArgumentType.TrimEnd('?').StartsWith("IEnumerable<", StringComparison.Ordinal) &&
                (actualArgumentType.TrimEnd('?').StartsWith("List<", StringComparison.Ordinal) ||
                 actualArgumentType.TrimEnd('?').StartsWith("IEnumerable<", StringComparison.Ordinal) ||
                 actualArgumentType.TrimEnd('?').StartsWith("IReadOnlyList<", StringComparison.Ordinal)) &&
                !ExpressionProducesFuture(argumentValue) &&
                TryGetGenericTypeArguments(expectedArgumentType.TrimEnd('?'), out var expectedEnumerableArguments) &&
                expectedEnumerableArguments.Length == 1)
            {
                var wrapAwaitedCollection = argumentValue.Kind == CoreNodeKind.AwaitExpression;
                if (wrapAwaitedCollection) builder.Append('(');
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                if (wrapAwaitedCollection) builder.Append(')');
                builder.Append(".Cast<").Append(expectedEnumerableArguments[0]).Append(">()");
            }
            else if (expectedArgumentType.TrimEnd('?').StartsWith("DartMap<", StringComparison.Ordinal) &&
                actualArgumentType.TrimEnd('?').StartsWith("DartMap<", StringComparison.Ordinal) &&
                expectedArgumentType.TrimEnd('?') != actualArgumentType.TrimEnd('?') &&
                TryGetGenericTypeArguments(expectedArgumentType.TrimEnd('?'), out var expectedMapArguments) &&
                !expectedMapArguments.Any(IsUnboundTypeParameterName))
            {
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(".cast<").Append(DartMapTypeArguments(expectedArgumentType.TrimEnd('?'))).Append(">()");
            }
            else if (actualArgumentType.TrimEnd('?') is "object" or "dynamic" &&
                expectedArgumentType.TrimEnd('?') is not ("object" or "dynamic") &&
                expectedArgumentType.Length > 0)
            {
                builder.Append("((").Append(expectedArgumentType.TrimEnd('?')).Append(")(object)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else if ((castDynamicArguments || invocationName == "_invokeMethod") && actualArgumentType == "dynamic")
            {
                builder.Append("(object?)");
                LowerExpression(builder, argumentValue, declaration, package, library, inputPath, diagnostics);
            }
            else if (expectedArgumentType.Length > 0 &&
                ShouldCastInvocationArgument(actualArgumentType, expectedArgumentType))
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<").Append(expectedArgumentType.TrimEnd('?')).Append(">(");
                LowerExpression(builder, values[index], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else
            {
                LowerExpression(builder, values[index], declaration, package, library, inputPath, diagnostics);
            }
        }
    }

    private bool CallableReturnsValue(CoreAstNode argument, CoreResolvedDeclaration declaration)
    {
        if (FindGlobalMember(argument.ElementId) is { } member)
        {
            return MapType(member.Element.ReturnType ?? "void") != "void";
        }
        var name = argument.Text(CoreProperty.name) ?? argument.Children.FirstOrDefault(candidate =>
            candidate.Kind == CoreNodeKind.SimpleIdentifier)?.Text(CoreProperty.name);
        if (string.IsNullOrEmpty(name)) return false;
        var owner = _session.ActiveDonorDeclaration ?? declaration;
        var ownerMember = owner.Members.FirstOrDefault(candidate =>
            candidate.Kind == "method" && string.Equals(candidate.Name, name, StringComparison.Ordinal));
        if (ownerMember is not null)
        {
            return MapType(ownerMember.Element.ReturnType ?? "void") != "void";
        }
        var localFunction = DescendantsAndSelf(owner.Ast).FirstOrDefault(candidate =>
            candidate.Kind == CoreNodeKind.FunctionDeclaration &&
            string.Equals(candidate.Text(CoreProperty.name), name, StringComparison.Ordinal));
        var functionType = localFunction?.Children.FirstOrDefault(candidate => candidate.Kind == CoreNodeKind.FunctionExpression)?.StaticType
            ?? localFunction?.StaticType;
        if (string.IsNullOrEmpty(functionType)) return false;
        var functionIndex = FindTopLevelFunctionIndex(functionType);
        return functionIndex > 0 && MapType(functionType[..functionIndex].Trim()) != "void";
    }

    private CoreResolvedMember? ResolveInvocationMember(
        CoreAstNode invocation,
        CoreAstNode? target,
        CoreResolvedDeclaration declaration,
        string methodName)
    {
        var invocationElementId = invocation.ElementId ?? invocation.Children.FirstOrDefault(child =>
            child.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(child.Text(CoreProperty.name), methodName, StringComparison.Ordinal))?.ElementId;
        if (FindGlobalMember(invocationElementId) is { } resolved)
        {
            return resolved;
        }
        if (target is null)
        {
            return (_session.ActiveDonorDeclaration ?? declaration).Members.FirstOrDefault(member =>
                member.Kind == "method" && string.Equals(member.Name, methodName, StringComparison.Ordinal));
        }
        var rawTargetType = StripLibraryPrefix((target.StaticType ?? string.Empty).TrimEnd('?'));
        var rawGeneric = rawTargetType.IndexOf('<');
        if (rawGeneric >= 0) rawTargetType = rawTargetType[..rawGeneric];
        var targetDeclaration = FindGlobalDeclaration(rawTargetType) ??
            FindGlobalDeclaration(MapType(target.StaticType ?? string.Empty).TrimEnd('?'));
        if (targetDeclaration is null)
        {
            return null;
        }
        var pending = new Queue<CoreResolvedDeclaration>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        pending.Enqueue(targetDeclaration);
        while (pending.Count > 0)
        {
            var candidate = pending.Dequeue();
            if (!visited.Add(candidate.Element.CanonicalId)) continue;
            var member = candidate.Members.FirstOrDefault(item =>
                item.Kind == "method" && string.Equals(item.Name, methodName, StringComparison.Ordinal));
            if (member is not null) return member;
            foreach (var baseName in DirectBaseNames(candidate))
            {
                if (FindGlobalDeclaration(baseName) is { } baseDeclaration) pending.Enqueue(baseDeclaration);
            }
        }
        return null;
    }

    private string[]? ResolveInvocationParameterTypes(
        CoreAstNode invocation,
        CoreAstNode? target,
        CoreResolvedDeclaration declaration,
        string methodName)
    {
        if (methodName == "scheduleMicrotask")
        {
            return ["Action"];
        }
        if (methodName == "addPostFrameCallback")
        {
            return ["Action<Duration>", "string"];
        }
        if (methodName == "setMessageHandler" && target is not null)
        {
            var channelTargetMember = FindGlobalMember(target.ElementId);
            var channelTargetType = MapType(target.StaticType ?? channelTargetMember?.Element.ReturnType ??
                channelTargetMember?.Element.Type ?? string.Empty).TrimEnd('?');
            if (channelTargetType.StartsWith("BasicMessageChannel<", StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(channelTargetType, out var channelArguments) &&
                channelArguments.Length == 1)
            {
                // BasicMessageChannel's emitted contract deliberately erases
                // Future<dynamic> to the non-generic Future base. The analyzer
                // parameter type still contains Future<object>, so prefer the
                // receiver contract used by the generated method declaration.
                return [$"global::System.Func<{channelArguments[0]}, Future>?"];
            }
        }
        if (methodName is "loadStructuredData" or "loadStructuredBinaryData")
        {
            // The generated AssetBundle contract normalizes Dart FutureOr<T>
            // parsers to object and awaits their result inside the method.
            // Use that emitted CLR contract instead of the analyzer's still-
            // generic Future<T> callback type at invocation sites.
            var parserInputType = methodName == "loadStructuredData" ? "string" : "ByteData";
            return ["string", $"global::System.Func<{parserInputType}, object>"];
        }
        var analyzerParameterTypes = Enumerable.Range(0, 64)
            .Select(invocation.ParameterType)
            .TakeWhile(type => !string.IsNullOrWhiteSpace(type))
            .Select(type => MapType(type!))
            .ToArray();
        if (target is not null)
        {
            var targetMember = FindGlobalMember(target.ElementId);
            var targetType = MapType(target.StaticType ?? targetMember?.Element.ReturnType ??
                targetMember?.Element.Type ?? string.Empty).TrimEnd('?');
            if (TryGetGenericTypeArguments(targetType, out var targetArguments))
            {
                if (methodName == "insertAll" && targetArguments.Length == 1)
                {
                    return ["int", $"IEnumerable<{targetArguments[0]}>"];
                }
                if (methodName == "removeRange")
                {
                    return ["int", "int"];
                }
                if (methodName == "forEach" && targetArguments.Length == 1)
                {
                    return [$"Action<{targetArguments[0]}>"];
                }
                if (methodName == "map" && targetArguments.Length == 1 &&
                    TryGetGenericTypeArguments(MapType(invocation.StaticType ?? string.Empty).TrimEnd('?'), out var resultArguments) &&
                    resultArguments.Length == 1)
                {
                    return [$"Func<{targetArguments[0]}, {resultArguments[0]}>"];
                }
                if (methodName is "remove" or "containsKey" && targetArguments.Length >= 2 &&
                    (targetType.StartsWith("DartMap<", StringComparison.Ordinal) ||
                     targetType.StartsWith("Dictionary<", StringComparison.Ordinal)))
                {
                    return [targetArguments[0]];
                }
            }
        }
        var callableType = invocation.Children.FirstOrDefault(child =>
            child.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(child.Text(CoreProperty.name), methodName, StringComparison.Ordinal) &&
            child.StaticType?.Contains(" Function", StringComparison.Ordinal) == true)?.StaticType;
        if (callableType is not null)
        {
            var functionIndex = FindTopLevelFunctionIndex(callableType);
            var parameterStart = callableType.IndexOf('(', functionIndex);
            var parameterEnd = callableType.LastIndexOf(')');
            if (functionIndex >= 0 && parameterStart >= 0 && parameterEnd >= parameterStart)
            {
                var parameterText = callableType[(parameterStart + 1)..parameterEnd];
                return string.IsNullOrWhiteSpace(parameterText)
                    ? []
                    : SplitFunctionParameters(parameterText)
                        .Select(NormalizeFunctionParameterType)
                        .Select(MapType)
                        .ToArray();
            }
        }
        var resolvedMember = ResolveInvocationMember(invocation, target, declaration, methodName);
        if (resolvedMember?.Element.Parameters is not { } resolvedParameters)
        {
            return analyzerParameterTypes.Length > 0 ? analyzerParameterTypes : null;
        }
        var resolvedRawTypes = resolvedParameters.Select(parameter => parameter.Type).ToArray();
        if (target is not null && FindDeclaringDeclaration(resolvedMember) is { } memberOwner &&
            memberOwner.Element.TypeParameters is { Length: > 0 } ownerTypeParameters)
        {
            var targetType = StripLibraryPrefix((target.StaticType ?? string.Empty).TrimEnd('?'));
            var genericStart = targetType.IndexOf('<');
            if (genericStart > 0 && targetType.EndsWith('>'))
            {
                var targetTypeName = targetType[..genericStart];
                var targetDeclaration = FindGlobalDeclaration(targetTypeName);
                if (targetDeclaration?.Element.CanonicalId == memberOwner.Element.CanonicalId ||
                    string.Equals(targetTypeName, memberOwner.Name, StringComparison.Ordinal))
                {
                    var targetArguments = SplitGenericArguments(targetType[(genericStart + 1)..^1]);
                    var substitutions = ownerTypeParameters
                        .Take(Math.Min(ownerTypeParameters.Length, targetArguments.Length))
                        .Select((parameter, index) => new KeyValuePair<string, string>(parameter.Name, targetArguments[index]))
                        .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
                    resolvedRawTypes = resolvedRawTypes
                        .Select(type => ApplyTypeParameterSubstitutions(type, substitutions))
                        .ToArray();
                }
            }
        }
        var resolvedTypes = resolvedRawTypes.Select(MapType).ToArray();
        if (resolvedMember.Element.TypeParameters is { Length: > 0 } methodTypeParameters)
        {
            var methodArguments = invocation.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList)?
                .Children.Where(item => item.Category == "type").Select(MapTypeFromAst).ToArray() ?? [];
            if (methodArguments.Length == 0 && methodName == "resolveWith" &&
                _session.ActiveFunctionReturnType is { } contextualReturn &&
                TryGetGenericTypeArguments(contextualReturn.TrimEnd('?'), out var contextualArguments) &&
                contextualArguments.Length == 1 &&
                contextualReturn.Contains("WidgetStateProperty<", StringComparison.Ordinal))
            {
                // The analyzer can report the enclosing StatefulWidget as the
                // inferred T for a context-inferred resolveWith closure. The
                // getter/method return contract is the authoritative Dart type.
                methodArguments = contextualArguments;
            }
            if (methodArguments.Length == 0 && methodTypeParameters.Length == 1 &&
                TryGetGenericTypeArguments(MapType(invocation.StaticType ?? string.Empty).TrimEnd('?'), out var inferredReturnArguments) &&
                inferredReturnArguments.Length == 1)
            {
                // Analyzer IR omits an inferred TypeArgumentList even though
                // invocation.StaticType has already fixed T (for example,
                // loadStructuredData returning Future<AssetManifest>).
                methodArguments = inferredReturnArguments;
            }
            for (var index = 0; index < methodTypeParameters.Length && index < methodArguments.Length; index++)
            {
                resolvedTypes = resolvedTypes.Select(type => Regex.Replace(
                    type,
                    $@"\b{Regex.Escape(SafeIdentifier(methodTypeParameters[index].Name))}\b",
                    MapGenericArgument(methodArguments[index]),
                    RegexOptions.CultureInvariant)).ToArray();
            }
        }
        return resolvedTypes;
    }

    private bool ShouldCastInvocationArgument(string actualType, string expectedType)
    {
        var actual = actualType.TrimEnd('?');
        var expected = expectedType.TrimEnd('?');
        if (actual.Length == 0 || expected.Length == 0 || actual == expected ||
            IsValueType(actual) || IsValueType(expected))
        {
            return false;
        }
        var expectedDeclaration = FindGlobalDeclaration(expected);
        if (expectedDeclaration is null) return false;
        var pending = new Queue<string>(DirectBaseNames(expectedDeclaration));
        var visited = new HashSet<string>(StringComparer.Ordinal);
        while (pending.Count > 0)
        {
            var baseName = MapType(pending.Dequeue()).TrimEnd('?');
            if (!visited.Add(baseName)) continue;
            if (string.Equals(baseName, actual, StringComparison.Ordinal)) return true;
            if (FindGlobalDeclaration(baseName) is { } baseDeclaration)
            {
                foreach (var parent in DirectBaseNames(baseDeclaration)) pending.Enqueue(parent);
            }
        }
        return false;
    }

    private bool TryGetGenericTypeArguments(string type, out string[] arguments)
    {
        var open = type.IndexOf('<');
        if (open <= 0 || !type.EndsWith('>'))
        {
            arguments = [];
            return false;
        }
        arguments = SplitGenericArguments(type[(open + 1)..^1]);
        return true;
    }

    private void EmitFunctionTearOffReceiver(
        CsSyntaxBuilder builder,
        CoreAstNode tearOff,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var receiver = tearOff.Child(CoreChildRole.targetOffset);
        if (receiver is null && tearOff.Kind == CoreNodeKind.PrefixedIdentifier)
        {
            var prefix = tearOff.Text(CoreProperty.prefix);
            receiver = tearOff.Children.FirstOrDefault(item =>
                item.Kind == CoreNodeKind.SimpleIdentifier &&
                string.Equals(item.Text(CoreProperty.name), prefix, StringComparison.Ordinal));
        }
        if (receiver is null)
        {
            builder.Append("this.");
            return;
        }
        LowerExpression(builder, receiver, declaration, package, library, inputPath, diagnostics);
        builder.Append('.');
    }

}
