using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.DartToCSharp;

internal sealed partial class FrameworkCSharpLowerer
{
    private bool IsPinnedG31CompatibilityLibrary(string library) => library is
        "package:flutter/src/foundation/object.dart" or
        "package:flutter/src/foundation/annotations.dart" or
        "package:flutter/src/physics/tolerance.dart";

    private bool TryEmitPinnedG31Declaration(CsSyntaxBuilder builder, CoreResolvedDeclaration declaration, string library)
    {
        if (library == "package:flutter/src/foundation/annotations.dart")
        {
            var (propertyType, propertyName) = declaration.Name switch
            {
                "Category" => ("IReadOnlyList<string>", "sections"),
                "DocumentationIcon" => ("string", "url"),
                "Summary" => ("string", "text"),
                _ => (string.Empty, string.Empty),
            };
            if (propertyType.Length == 0)
            {
                return false;
            }
            builder.AppendLine($"public class {declaration.Name}");
            builder.AppendLine("{");
            builder.AppendLine($"    public {propertyType} {propertyName} {{ get; }}");
            builder.AppendLine();
            builder.AppendLine($"    public {declaration.Name}({propertyType} {propertyName})");
            builder.AppendLine("    {");
            builder.AppendLine($"        this.{propertyName} = {propertyName};");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("}");
            builder.AppendLine();
            return true;
        }
        if (library == "package:flutter/src/physics/tolerance.dart" && declaration.Name == "Tolerance")
        {
            builder.AppendLine("public class Tolerance");
            builder.AppendLine("{");
            builder.AppendLine("    private const double _epsilonDefault = 0.001;");
            builder.AppendLine("    public static readonly Tolerance defaultTolerance = new Tolerance();");
            builder.AppendLine("    public double distance { get; }");
            builder.AppendLine("    public double time { get; }");
            builder.AppendLine("    public double velocity { get; }");
            builder.AppendLine();
            builder.AppendLine("    public Tolerance(double distance = _epsilonDefault, double time = _epsilonDefault, double velocity = _epsilonDefault)");
            builder.AppendLine("    {");
            builder.AppendLine("        this.distance = distance;");
            builder.AppendLine("        this.time = time;");
            builder.AppendLine("        this.velocity = velocity;");
            builder.AppendLine("    }");
            builder.AppendLine();
            builder.AppendLine("    public override string ToString() => $\"{Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, \"Tolerance\")}(distance: ±{distance}, time: ±{time}, velocity: ±{velocity})\";");
            builder.AppendLine("}");
            return true;
        }
        return false;
    }

    private void EmitMethodInvocation(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var target = node.Child(CoreChildRole.targetOffset);
        var methodName = node.Text(CoreProperty.name) ?? "missing";
        var invocationElementId = node.ElementId ?? node.Children.FirstOrDefault(child =>
            child.Kind == CoreNodeKind.SimpleIdentifier &&
            string.Equals(child.Text(CoreProperty.name), methodName, StringComparison.Ordinal))?.ElementId;
        var invocationArguments = node.Child(CoreChildRole.argumentsOffset);
        var thenArguments = invocationArguments?.Children
            .Where(item => item.Category is "expression" or "argument")
            .ToArray();
        if (methodName == "Create" && target is not null &&
            DescendantsAndSelf(target).Any(item =>
                item.Text(CoreProperty.name)?.Contains("DiagnosticsProperty", StringComparison.Ordinal) == true))
        {
            builder.Append("new ");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append('(');
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (declaration.Name == "HtmlElementView" &&
            methodName is "createFromTagName" or "buildImpl")
        {
            // The selected desktop/IO conditional extension intentionally
            // throws: HtmlElementView is a web-only widget. Extension members
            // are not CLR instance members, so retain the selected IO behavior
            // directly at the call site.
            builder.Append("throw new NotSupportedException(\"HtmlElementView is only available on Flutter Web\")");
            return;
        }
        if (target is not null && methodName == "of" &&
            DescendantsAndSelf(target).Any(candidate => candidate.Text(CoreProperty.name) == "CreationLocation"))
        {
            builder.Append("global::Doroti.Runtime.CreationLocation.of(");
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "showInViewport" && declaration.Name == "RenderViewportBase")
        {
            builder.Append("global::Doroti.Framework.Rendering.RenderViewportBase<ParentDataClass>.showInViewport(");
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "addPointer" && target is not null && thenArguments is { Length: 1 })
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".addPointer((global::Doroti.Framework.Gestures.PointerDownEvent)(object)");
            LowerExpression(builder, thenArguments[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "forEach" && target is not null && thenArguments is { Length: 1 } &&
            MapType(thenArguments[0].StaticType ?? string.Empty).Contains("Future", StringComparison.Ordinal))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(target.StaticType?.EndsWith("?", StringComparison.Ordinal) == true ? "?.forEach((__item) => { _ = " : ".forEach((__item) => { _ = ");
            LowerExpression(builder, thenArguments[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("(__item); })");
            return;
        }
        if (methodName == "then" &&
            node.ElementId == "dart:async#Future.then" &&
            target is not null &&
            thenArguments is { Length: 1 } &&
            thenArguments[0] is { Kind: CoreNodeKind.FunctionExpression } dynamicCallback &&
            MapType(dynamicCallback.StaticType ?? string.Empty) is { Length: > 0 } callbackType &&
            callbackType != "dynamic")
        {
            // Future.then callbacks are contextual in Dart. Some framework
            // receivers lower through a dynamic member even though the outer
            // invocation retains Future<T> analyzer metadata; C# dynamic
            // dispatch then has no target delegate type for the lambda. Keep
            // the analyzer-resolved callback signature explicit in both cases.
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".then((").Append(callbackType).Append(')');
            var previousFunctionReturnType = _session.ActiveFunctionReturnType;
            _session.ActiveFunctionReturnType = callbackType.StartsWith("Action", StringComparison.Ordinal)
                ? "void"
                : TryGetGenericTypeArguments(callbackType.TrimEnd('?'), out var callbackArguments) &&
                  callbackArguments.Length > 0
                    ? callbackArguments[^1]
                    : null;
            try
            {
                LowerExpression(builder, dynamicCallback, declaration, package, library, inputPath, diagnostics);
            }
            finally
            {
                _session.ActiveFunctionReturnType = previousFunctionReturnType;
            }
            builder.Append(')');
            return;
        }
        if (methodName == "invokeLayoutCallback" &&
            invocationArguments?.Children.FirstOrDefault(item => item.Category == "expression") is { Kind: CoreNodeKind.FunctionExpression } layoutCallback &&
            layoutCallback.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FormalParameterList)?
                .Children.FirstOrDefault(item => item.Category == "parameter") is { } callbackParameter &&
            layoutCallback.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody)?
                .Child(CoreChildRole.expressionOffset) is { } callbackExpression)
        {
            if (target is not null)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append('.');
            }
            var parameterName = SafeIdentifier(callbackParameter.Text(CoreProperty.name) ?? "constraints");
            builder.Append("invokeLayoutCallback<Constraints>((Constraints ").Append(parameterName).Append(") => ");
            LowerExpression(builder, callbackExpression, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "debugGetOpenHandleStackTraces" && node.ElementId?.Contains("dart:ui#Image.", StringComparison.Ordinal) == true)
        {
            builder.Append("global::Doroti.Ui.Image.debugGetOpenHandleStackTraces(");
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "invokeCallback" && invocationArguments is not null)
        {
            var values = invocationArguments.Children
                .Where(item => item.Category is "expression" or "argument")
                .Select(item => item.Kind == CoreNodeKind.NamedExpression ? item.Child(CoreChildRole.expressionOffset) : item)
                .Where(item => item is not null)
                .Cast<CoreAstNode>()
                .ToArray();
            if (values.Length >= 2 && values[1].StaticType?.StartsWith("void Function()", StringComparison.Ordinal) == true)
            {
                if (target is not null)
                {
                    LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                    builder.Append('.');
                }
                builder.Append("invokeCallback<object?>(");
                LowerExpression(builder, values[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(", () => { ((Action)(");
                LowerExpression(builder, values[1], declaration, package, library, inputPath, diagnostics);
                builder.Append("))(); return null; }");
                for (var index = 2; index < values.Length; index++)
                {
                    builder.Append(", ");
                    LowerExpression(builder, values[index], declaration, package, library, inputPath, diagnostics);
                }
                builder.Append(')');
                return;
            }
        }
        if (methodName == "_lerp" && target is null && node.StaticType is { } nullableLerpType &&
            nullableLerpType.EndsWith("?", StringComparison.Ordinal) &&
            IsValueType(MapType(nullableLerpType).TrimEnd('?')))
        {
            builder.Append("DartRuntimePrimitives.LerpNullable(");
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName == "toString" && target?.Kind == CoreNodeKind.SuperExpression)
        {
            builder.Append("base.ToString()");
            return;
        }
        if (target?.Kind == CoreNodeKind.SuperExpression && methodName is "toStringShort" or "toDiagnosticsNode")
        {
            builder.Append("base.").Append(methodName).Append('(');
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        var hasChildOrderArgument = invocationArguments?.Children.Any(argument =>
            argument.Kind == CoreNodeKind.NamedExpression && argument.Text(CoreProperty.name) == "childOrder") == true;
        if (methodName is "toStringShort" or "toDiagnosticsNode" &&
            target is not null && target.Kind != CoreNodeKind.SuperExpression && !hasChildOrderArgument)
        {
            builder.Append("((Diagnosticable)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(").").Append(methodName).Append('(');
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "toString" &&
            invocationArguments?.Children.Any(argument =>
                argument.Kind == CoreNodeKind.NamedExpression && argument.Text(CoreProperty.name) == "minLevel") == true)
        {
            if (target is not null)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append('.');
            }
            builder.Append("toDiagnosticsNode().toStringDeep(");
            EmitArguments(builder, invocationArguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is null && methodName == "loadFontFromList" && node.ElementId?.Contains("dart:ui", StringComparison.Ordinal) == true)
        {
            builder.Append("Dart_uiLibrary.loadFontFromList(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "findLockByLogicalKey" && target is not null && target.Text(CoreProperty.name) == "KeyboardLockMode")
        {
            var keyArgument = node.Child(CoreChildRole.argumentsOffset)?.Children.FirstOrDefault(item => item.Category == "expression");
            builder.Append('(');
            if (keyArgument is not null) LowerExpression(builder, keyArgument, declaration, package, library, inputPath, diagnostics);
            else builder.Append("default(LogicalKeyboardKey)");
            builder.Append(".keyId switch { var id when id == LogicalKeyboardKey.numLock.keyId => KeyboardLockMode.numLock, var id when id == LogicalKeyboardKey.scrollLock.keyId => KeyboardLockMode.scrollLock, var id when id == LogicalKeyboardKey.capsLock.keyId => KeyboardLockMode.capsLock, _ => (KeyboardLockMode?)null })");
            return;
        }
        if (methodName == "fromStandardMessageCodecMessage" && target is not null)
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".CreateFromStandardMessageCodecMessage(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target?.Kind == CoreNodeKind.SuperExpression &&
            AppliedMixinDeclarations(declaration).Any(mixin => mixin.Members.Any(member =>
                member.Kind == "method" && member.Name == methodName &&
                !DescendantsAndSelf(member.Ast).Any(item => item.Category == "statement" && item.Kind != CoreNodeKind.Block))))
        {
            builder.Append("DartRuntimePrimitives.Noop()");
            return;
        }
        if (methodName == "sublist" && target is not null)
        {
            var sublistArguments = thenArguments ?? [];
            if (sublistArguments.Length == 1)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(".Skip(checked((int)");
                LowerExpression(builder, sublistArguments[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(")).ToList()");
                return;
            }
        }
        if (methodName == "fold" && target is not null)
        {
            var foldArguments = invocationArguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
            if (foldArguments.Length == 2)
            {
                builder.Append("System.Linq.Enumerable.Aggregate(");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(", (").Append(MapType(node.StaticType ?? "object")).Append(")");
                LowerExpression(builder, foldArguments[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(", ");
                LowerExpression(builder, foldArguments[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
        }
        if (methodName is "insert" or "Insert" && target is not null &&
            (IsDartCollectionType(ResolvedExpressionValueType(target)) ||
             node.ElementId?.StartsWith("dart:core#List.", StringComparison.Ordinal) == true))
        {
            var insertArguments = node.Child(CoreChildRole.argumentsOffset)?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
            if (insertArguments.Length == 2)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(".Insert(checked((int)");
                LowerExpression(builder, insertArguments[0], declaration, package, library, inputPath, diagnostics);
                builder.Append("), ");
                LowerExpression(builder, insertArguments[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
        }
        if (methodName is "removeAt" or "RemoveAt" && target is not null)
        {
            var removeArgument = node.Child(CoreChildRole.argumentsOffset)?.Children.FirstOrDefault(item => item.Category == "expression");
            if (removeArgument is not null)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(".removeAt(");
                LowerExpression(builder, removeArgument, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
        }
        if (methodName is "indexOf" or "IndexOf" && target is not null)
        {
            var indexArguments = node.Child(CoreChildRole.argumentsOffset)?.Children
                .Where(item => item.Category == "expression").ToArray() ?? [];
            if ((target.StaticType ?? string.Empty).TrimEnd('?') == "String" && indexArguments.Length == 2)
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(".IndexOf(");
                LowerExpression(builder, indexArguments[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(", checked((int)(");
                LowerExpression(builder, indexArguments[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(")))");
                return;
            }
            var indexArgument = indexArguments.FirstOrDefault();
            if (indexArgument?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true &&
                IsValueType(MapType(indexArgument.StaticType).TrimEnd('?')))
            {
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(".IndexOf(DartRuntimePrimitives.RequireValue(");
                LowerExpression(builder, indexArgument, declaration, package, library, inputPath, diagnostics);
                builder.Append("))");
                return;
            }
        }
        if (methodName is "min" or "max" &&
            (target is null || target.Text(CoreProperty.name) == "math" || target.Text(CoreProperty.prefix) == "math"))
        {
            var resultType = MapType(node.StaticType ?? string.Empty).TrimEnd('?');
            builder.Append(IsUnboundTypeParameterName(resultType)
                ? methodName == "min" ? "DartRuntimePrimitives.Min(" : "DartRuntimePrimitives.Max("
                : methodName == "min" ? "Math.Min(" : "Math.Max(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName == "scheduleMicrotask" && target is null)
        {
            builder.Append("DartAsyncRuntime.scheduleMicrotask(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName is "encodeFull" or "parse" &&
            (target is not null && target.Text(CoreProperty.name) == "Uri" || node.ElementId?.Contains("#Uri.", StringComparison.Ordinal) == true))
        {
            builder.Append("DartUri.").Append(methodName).Append('(');
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName == "debugFillProperties" && target?.Kind == CoreNodeKind.SuperExpression)
        {
            builder.Append("DiagnosticableDefaults.debugFillProperties(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is not null && methodName is "firstKey" or "lastKey" &&
            MapType(node.StaticType ?? string.Empty) is { } nullableKeyType &&
            nullableKeyType.EndsWith("?", StringComparison.Ordinal) &&
            IsValueType(nullableKeyType.TrimEnd('?')) &&
            TryGetGenericTypeArguments(MapType(target.StaticType ?? string.Empty).TrimEnd('?'), out var mapArguments) &&
            mapArguments.Length == 2)
        {
            builder.Append("DartCollectionRuntime.")
                .Append(methodName == "firstKey" ? "FirstKeyOrNull" : "LastKeyOrNull")
                .Append('<').Append(nullableKeyType.TrimEnd('?')).Append(", ").Append(mapArguments[1]).Append(">(");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "fromEnvironment" &&
            TryEmitFromEnvironment(builder, node, target, declaration, package, library, inputPath, diagnostics))
        {
            return;
        }
        var typeArguments = string.Empty;
        var mappedExplicitTypeArguments = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList)?
            .Children.Where(item => item.Category == "type").Select(MapTypeFromAst).ToArray() ?? [];
        var explicitTypeArguments = methodName == "then" && mappedExplicitTypeArguments.Any(type => type is "void" or "Null")
            ? []
            : mappedExplicitTypeArguments.Select(MapGenericArgument).ToArray();
        if (explicitTypeArguments.Length > 0)
        {
            typeArguments = $"<{string.Join(", ", explicitTypeArguments)}>";
        }
        // Dart permits partial explicit method type arguments (the receiver element
        // type remains inferred). C# does not, so let both fold arguments infer.
        if (methodName == "fold")
        {
            typeArguments = string.Empty;
        }
        if (string.IsNullOrEmpty(typeArguments) && target is null && _currentDeclarations is not null)
        {
            var called = FindDeclaration(node.ElementId);
            if (called is not null && called.Element.TypeParameters is { Length: > 0 } && declaration.Element.TypeParameters is { Length: > 0 })
            {
                typeArguments = $"<{string.Join(", ", declaration.Element.TypeParameters.Select(item => SafeIdentifier(item.Name)))}>";
            }
        }
        if (string.IsNullOrEmpty(typeArguments) &&
            methodName is "invokeMethod" or "loadStructuredData" or "loadStructuredBinaryData" &&
            node.StaticType is { } inferredFutureType)
        {
            var start = inferredFutureType.IndexOf('<');
            var end = inferredFutureType.LastIndexOf('>');
            if (start >= 0 && end > start)
            {
                typeArguments = $"<{MapGenericArgument(inferredFutureType[(start + 1)..end])}>";
            }
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "then" && node.StaticType is { } inferredThenType &&
            inferredThenType.StartsWith("Future<", StringComparison.Ordinal))
        {
            var start = inferredThenType.IndexOf('<');
            var end = inferredThenType.LastIndexOf('>');
            if (start >= 0 && end > start && inferredThenType[(start + 1)..end] is not ("void" or "Null"))
            {
                typeArguments = $"<{MapGenericArgument(inferredThenType[(start + 1)..end])}>";
            }
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "then" && declaration.Name == "TickerFuture")
        {
            var containingGenericMember = declaration.Members.FirstOrDefault(member =>
                ContainsOffset(member.Ast, node.Offset) && member.Element.TypeParameters is { Length: 1 });
            if (containingGenericMember?.Element.TypeParameters is { Length: 1 } memberTypeParameters)
            {
                typeArguments = $"<{SafeIdentifier(memberTypeParameters[0].Name)}>";
            }
        }
        if (target is not null && target.Text(CoreProperty.name) is { } staticTargetName &&
            TryResolveEmittedNamedConstructor(MapType(staticTargetName), methodName, out var staticFactoryMethod))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append('.').Append(staticFactoryMethod).Append('(');
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        // Import-prefixed dart: top-level: math.log(x) / ui.clampDouble(...) → Dart_*Library.method(...)
        if (target is not null &&
            !string.IsNullOrEmpty(node.ElementId) &&
            LibraryUriFromElementId(node.ElementId) is { } dartLibrary &&
            dartLibrary.StartsWith("dart:", StringComparison.Ordinal) &&
            string.IsNullOrEmpty(target.StaticType) &&
            methodName is not ("identical" or "hash" or "hashAll"))
        {
            var symbol = node.ElementId![(node.ElementId.LastIndexOf('#') + 1)..];
            var ownerSeparator = symbol.IndexOf('.');
            var owner = ownerSeparator > 0 ? symbol[..ownerSeparator] : null;
            var resolvedTarget = (dartLibrary, owner ?? target.Text(CoreProperty.name)) switch
            {
                ("dart:developer", "Timeline") => "Timeline",
                ("dart:developer", "Flow") => "Flow",
                ("dart:async", "Timer") => "global::Doroti.Runtime.Timer",
                ("dart:ui", { } dartUiOwner) when owner is not null => "Dart_uiLibrary." + SafeIdentifier(dartUiOwner),
                _ => MapDartLibraryStaticClass(dartLibrary),
            };
            builder.Append(resolvedTarget).Append('.')
                .Append(MapMethodInvocationName(methodName, null)).Append(typeArguments).Append('(');
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (target is null &&
            !string.IsNullOrEmpty(node.ElementId) &&
            LibraryUriFromElementId(node.ElementId) is { } bareDartLibrary &&
            bareDartLibrary.StartsWith("dart:", StringComparison.Ordinal) &&
            !node.ElementId!.Contains('.', StringComparison.Ordinal) &&
            methodName is not ("identical" or "hash" or "hashAll"))
        {
            builder.Append(MapDartLibraryStaticClass(bareDartLibrary)).Append('.')
                .Append(MapMethodInvocationName(methodName, null)).Append(typeArguments).Append('(');
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "cast" && string.IsNullOrEmpty(typeArguments) && node.StaticType is { } castType)
        {
            var start = castType.IndexOf('<');
            var end = castType.LastIndexOf('>');
            if (start >= 0 && end > start)
            {
                var arguments = SplitGenericArguments(castType[(start + 1)..end]).Select(MapType).ToArray();
                if (arguments.Length > 0) typeArguments = $"<{string.Join(", ", arguments)}>";
            }
        }
        if (methodName == "map" && target?.StaticType is { } mapTargetType && node.StaticType is { } mapResultType)
        {
            var mappedTarget = MapType(mapTargetType);
            var mappedResult = MapType(mapResultType);
            var targetStart = mappedTarget.IndexOf('<');
            var targetEnd = mappedTarget.LastIndexOf('>');
            var resultStart = mappedResult.IndexOf('<');
            var resultEnd = mappedResult.LastIndexOf('>');
            if (targetStart >= 0 && targetEnd > targetStart && resultStart >= 0 && resultEnd > resultStart)
            {
                typeArguments = $"<{mappedTarget[(targetStart + 1)..targetEnd]}, {mappedResult[(resultStart + 1)..resultEnd]}>";
            }
        }
        else if (methodName is "expand" or "where" or "any" or "forEach")
        {
            typeArguments = string.Empty;
        }
        if (methodName == "toString" && target?.Kind == CoreNodeKind.PrefixedIdentifier && target.Text(CoreProperty.name) == "runtimeType")
        {
            var prefix = target.Text(CoreProperty.prefix) ?? "missing";
            var prefixNode = target.Children.FirstOrDefault(item =>
                item.Kind == CoreNodeKind.SimpleIdentifier &&
                string.Equals(item.Text(CoreProperty.name), prefix, StringComparison.Ordinal));
            builder.Append("DartRuntimePrimitives.RuntimeTypeName(");
            if (prefixNode is null) builder.Append(SafeIdentifier(prefix));
            else LowerExpression(builder, prefixNode, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "toString" && target?.Kind == CoreNodeKind.PropertyAccess && target.Text(CoreProperty.name) == "runtimeType")
        {
            var targetExpression = target.Child(CoreChildRole.targetOffset);
            builder.Append("DartRuntimePrimitives.RuntimeTypeName(");
            if (targetExpression is not null)
            {
                LowerExpression(builder, targetExpression, declaration, package, library, inputPath, diagnostics);
            }
            builder.Append(')');
            return;
        }
        if (methodName == "identical" && target is null)
        {
            builder.Append("DartRuntimePrimitives.Identical(");
            var arguments = node.Child(CoreChildRole.argumentsOffset);
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "hash" &&
            (target?.Kind == CoreNodeKind.SimpleIdentifier && target.Text(CoreProperty.name) == "Object" ||
             target?.Kind == CoreNodeKind.PrefixedIdentifier && target.Text(CoreProperty.name) == "hash" && target.Text(CoreProperty.prefix) == "Object" ||
             target is null && IsTopLevelElement(node.ElementId, "hash")))
        {
            builder.Append("FoundationRuntimePorts.ObjectHash(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (methodName == "hashAll" && target is not null)
        {
            builder.Append("FoundationRuntimePorts.ObjectHashAll(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName == "join" && target is not null)
        {
            var arguments = node.Child(CoreChildRole.argumentsOffset);
            var argumentList = arguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
            if (argumentList.Length > 0)
            {
                builder.Append("string.Join(");
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(", ");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
                return;
            }
        }
        if (methodName == "call" && target is not null)
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append("?.Invoke(");
            var arguments = node.Child(CoreChildRole.argumentsOffset);
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        if (methodName == "every" && target is not null)
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".All(");
            EmitArguments(builder, node.Child(CoreChildRole.argumentsOffset), declaration, package, library, inputPath, diagnostics, preserveNames: false);
            builder.Append(')');
            return;
        }
        var args = node.Child(CoreChildRole.argumentsOffset);
        var intrinsicArguments = args?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
        if (target is not null && methodName == "insert" &&
            invocationElementId?.StartsWith("package:flutter/", StringComparison.Ordinal) == true)
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".insert").Append(typeArguments).Append('(');
            EmitArguments(builder, args, declaration, package, library, inputPath, diagnostics,
                expectedParameters: ResolveInvocationMember(node, target, declaration, methodName)?.Element.Parameters,
                expectedArgumentTypes: ResolveInvocationParameterTypes(node, target, declaration, methodName));
            builder.Append(')');
            return;
        }
        if (target is null && methodName == "_computeIntrinsics" && intrinsicArguments.Length > 0 &&
            FindGlobalDeclaration(intrinsicArguments[0].StaticType ?? string.Empty) is { Ast.Kind: CoreNodeKind.EnumDeclaration } enumArgumentDeclaration &&
            (enumArgumentDeclaration.Element.Interfaces?.Length ?? 0) > 0)
        {
            var enumName = EmittedTypeName(
                LibraryUriFromElementId(enumArgumentDeclaration.Element.CanonicalId),
                enumArgumentDeclaration.Name);
            builder.Append(methodName).Append(typeArguments).Append("(new ").Append(enumName).Append("InterfaceAdapter(");
            LowerExpression(builder, intrinsicArguments[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            foreach (var argument in intrinsicArguments.Skip(1))
            {
                builder.Append(", ");
                LowerExpression(builder, argument, declaration, package, library, inputPath, diagnostics);
            }
            builder.Append(')');
            return;
        }
        if (methodName == "determinant" && target is not null &&
            target.StaticType?.TrimEnd('?') == "Matrix4" &&
            (args is null || !args.Children.Any(item => item.Category == "expression")))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".determinant");
            return;
        }
        var resolvedTargetStorageType = target is null
            ? null
            : ResolvedExpressionValueType(target);
        if (string.IsNullOrWhiteSpace(resolvedTargetStorageType) &&
            target?.Text(CoreProperty.name) is { } targetMemberName)
        {
            resolvedTargetStorageType = AssignmentStorageType(
                _session.ActiveDonorDeclaration ?? declaration,
                targetMemberName,
                null);
        }
        if (methodName == "clear" && target is not null &&
            (IsDartCollectionType(target.StaticType) || IsDartCollectionType(resolvedTargetStorageType)))
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".Clear()");
            return;
        }
        if (string.IsNullOrEmpty(typeArguments) && target is not null &&
            methodName is "difference" or "removeLast" &&
            TryGetGenericTypeArguments(MapType(target.StaticType ?? string.Empty).TrimEnd('?'), out var collectionArguments) &&
            collectionArguments.Length > 0)
        {
            typeArguments = $"<{collectionArguments[0]}>";
        }
        if (string.IsNullOrEmpty(typeArguments) &&
            methodName is "maybePop" or "pop" or "restorablePushNamed")
        {
            typeArguments = "<object>";
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "pushNamed")
        {
            typeArguments = "<object>";
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "maybeFind" &&
            _session.ActiveMethodTypeParameters.Count == 1)
        {
            typeArguments = $"<{_session.ActiveMethodTypeParameters.Single()}>";
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "_of" &&
            declaration.Element.TypeParameters is { Length: 1 } declarationTypeParameters)
        {
            typeArguments = $"<{SafeIdentifier(declarationTypeParameters[0].Name)}>";
        }
        if (string.IsNullOrEmpty(typeArguments) && methodName == "of" &&
            node.ElementId is { } genericOfElementId &&
            (genericOfElementId.Contains("#ModalRoute.of", StringComparison.Ordinal) ||
             genericOfElementId.Contains("#Router.of", StringComparison.Ordinal)))
        {
            typeArguments = "<object>";
        }
        if (string.IsNullOrEmpty(typeArguments))
        {
            typeArguments = methodName switch
            {
                "invokeMethod" or "_invokeMethod" => "<object?>",
                "invokeMapMethod" => "<object?, object?>",
                _ => typeArguments,
            };
        }
        if (methodName == "pop" && target is not null &&
            (MapType(target.StaticType ?? string.Empty).TrimEnd('?').EndsWith("ParagraphBuilder", StringComparison.Ordinal) ||
             DescendantsAndSelf(target).Any(candidate =>
                 candidate.Text(CoreProperty.name) is "SystemNavigator" or "ParagraphBuilder")))
        {
            // These runtime ports expose a non-generic CLR pop operation even
            // when the Dart call carries an erased result type argument.
            typeArguments = string.Empty;
        }
        var targetStaticType = target?.StaticType;
        if (string.IsNullOrWhiteSpace(targetStaticType) &&
            (methodName is "add" or "addAll" or "clear" or "contains" or "difference" or "insert" or
                "insertAll" or "remove" or "removeAt" or "removeLast" or "removeRange") &&
            IsDartCollectionType(resolvedTargetStorageType))
        {
            targetStaticType = resolvedTargetStorageType;
        }
        string CurrentInvocationName()
        {
            if (target is not null)
            {
                if (IsDartCollectionType(targetStaticType))
                {
                    return MapMethodInvocationName(methodName, targetStaticType);
                }
                var rawTargetType = StripLibraryPrefix((targetStaticType ?? string.Empty).TrimEnd('?'));
                var rawGeneric = rawTargetType.IndexOf('<');
                if (rawGeneric >= 0) rawTargetType = rawTargetType[..rawGeneric];
                var targetDeclaration = FindGlobalDeclaration(rawTargetType) ??
                    FindGlobalDeclaration(MapType(targetStaticType ?? string.Empty).TrimEnd('?'));
                var targetMember = targetDeclaration?.Members.FirstOrDefault(member =>
                    member.Kind == "method" && string.Equals(member.Name, methodName, StringComparison.Ordinal));
                if (targetMember is not null && targetDeclaration is not null &&
                    !LibraryUriFromElementId(targetDeclaration.Element.CanonicalId).StartsWith("dart:", StringComparison.Ordinal))
                {
                    return MapMethodDeclarationName(targetMember);
                }
                var resolvedMember = ResolveInvocationMember(node, target, declaration, methodName);
                resolvedMember ??= targetDeclaration?
                    .Members.FirstOrDefault(member =>
                        member.Kind == "method" && string.Equals(member.Name, methodName, StringComparison.Ordinal));
                var resolvedOwner = resolvedMember is null ? null : FindDeclaringDeclaration(resolvedMember);
                if (resolvedMember is not null && resolvedOwner is not null &&
                    !LibraryUriFromElementId(resolvedOwner.Element.CanonicalId).StartsWith("dart:", StringComparison.Ordinal))
                {
                    return MapMethodDeclarationName(resolvedMember);
                }
                return MapMethodInvocationName(methodName, targetStaticType);
            }
            var owner = _session.ActiveDonorDeclaration ?? declaration;
            var currentMember = owner.Members.FirstOrDefault(member =>
                member.Kind == "method" &&
                (string.Equals(member.Element.CanonicalId, node.ElementId, StringComparison.Ordinal) ||
                 string.Equals(member.Name, methodName, StringComparison.Ordinal)));
            return currentMember is null
                ? MapMethodInvocationName(methodName, targetStaticType)
                : MapMethodDeclarationName(currentMember);
        }
        if (string.IsNullOrEmpty(targetStaticType) && node.ElementId is { } collectionElementId)
        {
            targetStaticType = collectionElementId.Contains("dart:core#List.", StringComparison.Ordinal) ? "List<object>" :
                collectionElementId.Contains("dart:core#Set.", StringComparison.Ordinal) ? "Set<object>" :
                collectionElementId.Contains("dart:core#Map.", StringComparison.Ordinal) ? "Map<object, object>" :
                collectionElementId.Contains("dart:core#Stopwatch.", StringComparison.Ordinal) ? "Stopwatch" :
                null;
        }
        var targetNullableReferenceType = !string.IsNullOrEmpty(targetStaticType) && targetStaticType.EndsWith("?", StringComparison.Ordinal) && !IsValueType(targetStaticType.TrimEnd('?'));
        var explicitNullAware = node.Text(CoreProperty.@operator) == "?.";
        // A nullable receiver means the Dart source used `?.`; preserve it for
        // nullable results too, otherwise the emitted plain call both throws on
        // null and breaks downstream `??` coalescing on the widened type.
        if (target is not null &&
            (explicitNullAware || targetNullableReferenceType &&
                (node.StaticType == "void" || node.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)))
        {
            if (node.StaticType is { } nullAwareResult &&
                ContainsUnboundTypeParameter(MapType(nullAwareResult)))
            {
                builder.Append("DartRuntimePrimitives.NullAware(");
                LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
                builder.Append(", __target => __target.").Append(CurrentInvocationName()).Append(typeArguments).Append('(');
                if (methodName != "toList")
                {
                    EmitArguments(
                        builder,
                        args,
                        declaration,
                        package,
                        library,
                        inputPath,
                        diagnostics,
                        expectedParameters: ResolveInvocationMember(node, target, declaration, methodName)?.Element.Parameters,
                        expectedArgumentTypes: ResolveInvocationParameterTypes(node, target, declaration, methodName),
                        invocationName: methodName);
                }
                builder.Append("))");
                return;
            }
            var dynamicDispatch = RequiresDynamicInvocationDispatch(targetStaticType, methodName) ||
                (methodName is "insert" or "remove" or "_updateCallback" &&
                 DescendantsAndSelf(target).Any(candidate => candidate.Text(CoreProperty.name) == "renderObject"));
            dynamicDispatch |= methodName == "markNeedsLayout" &&
                invocationArguments?.Children.Any(argument => argument.Kind == CoreNodeKind.NamedExpression &&
                    argument.Text(CoreProperty.name) == "withDelegateRebuild") == true;
            var dynamicResultType = dynamicDispatch && node.StaticType is { } nullableStaticType &&
                nullableStaticType.TrimEnd('?') is not ("void" or "dynamic" or "Object" or "object")
                    ? MapType(nullableStaticType)
                    : null;
            if (dynamicResultType is not null) builder.Append("((").Append(dynamicResultType).Append(')');
            if (dynamicDispatch) builder.Append("((dynamic)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            if (dynamicDispatch) builder.Append(')');
            builder.Append("?.");
            var mappedName = CurrentInvocationName();
            builder.Append(mappedName).Append(typeArguments);
            builder.Append('(');
            if (methodName != "toList")
            {
                EmitArguments(
                    builder,
                    args,
                    declaration,
                    package,
                    library,
                    inputPath,
                    diagnostics,
                    expectedParameters: ResolveInvocationMember(node, target, declaration, methodName)?.Element.Parameters,
                    expectedArgumentTypes: ResolveInvocationParameterTypes(node, target, declaration, methodName),
                    invocationName: methodName);
            }
            builder.Append(')');
            if (dynamicResultType is not null) builder.Append(')');
            return;
        }
        var usesDynamicDispatch = target is not null && target.Kind != CoreNodeKind.SuperExpression &&
            (RequiresDynamicInvocationDispatch(targetStaticType, methodName) ||
             (declaration.Name == "SliverMultiBoxAdaptorElement" && methodName == "insert") ||
             (methodName == "insert" && MapType(targetStaticType ?? string.Empty).TrimEnd('?') == "RenderObject") ||
             (methodName is "insert" or "remove" or "_updateCallback" &&
              DescendantsAndSelf(target).Any(candidate => candidate.Text(CoreProperty.name) == "renderObject")) ||
             methodName == "markNeedsLayout" &&
              invocationArguments?.Children.Any(argument => argument.Kind == CoreNodeKind.NamedExpression &&
                  argument.Text(CoreProperty.name) == "withDelegateRebuild") == true);
        var dynamicInvocationResultType = usesDynamicDispatch && node.StaticType is { } invocationStaticType &&
            invocationStaticType.TrimEnd('?') is not ("void" or "dynamic" or "Object" or "object")
                ? MapType(invocationStaticType)
                : null;
        if (dynamicInvocationResultType is not null) builder.Append("((").Append(dynamicInvocationResultType).Append(')');
        if (target is not null)
        {
            if (usesDynamicDispatch) builder.Append("((dynamic)");
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            if (usesDynamicDispatch) builder.Append(')');
            builder.Append('.');
        }
        var staticInvocationMember = target is null ? FindGlobalMember(node.ElementId) : null;
        var staticInvocationOwner = staticInvocationMember is { IsStatic: true }
            ? FindDeclaringDeclaration(staticInvocationMember)
            : null;
        if (target is null && staticInvocationMember is { IsStatic: true } && staticInvocationOwner is not null)
        {
            builder.Append(MapStaticOwnerType(staticInvocationOwner.Name, declaration))
                .Append('.')
                .Append(MapMethodDeclarationName(staticInvocationMember))
                .Append(typeArguments);
        }
        else if (target is null && _session.ExplicitThisExpression is not null &&
            node.ElementId?.StartsWith(declaration.Element.CanonicalId + ".", StringComparison.Ordinal) == true &&
            declaration.Members.Any(member =>
                !member.IsStatic && string.Equals(member.Element.CanonicalId, node.ElementId, StringComparison.Ordinal)))
        {
            builder.Append(_session.ExplicitThisExpression).Append('.')
                .Append(CurrentInvocationName()).Append(typeArguments);
        }
        else if (target is null && IsTopLevelElement(node.ElementId, "objectRuntimeType"))
        {
            builder.Append("global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType");
        }
        else if (target is null && !string.IsNullOrEmpty(node.ElementId))
        {
            var elementLibrary = LibraryUriFromElementId(node.ElementId);
            var marker = node.ElementId!.LastIndexOf('#');
            var symbol = marker >= 0 ? node.ElementId[(marker + 1)..] : string.Empty;
            var calledDeclaration = FindDeclaration(node.ElementId);
            if (!string.IsNullOrEmpty(elementLibrary) &&
                !symbol.Contains('.', StringComparison.Ordinal) &&
                !elementLibrary.StartsWith("dart:", StringComparison.Ordinal) &&
                (calledDeclaration?.Ast.Kind is CoreNodeKind.FunctionDeclaration or CoreNodeKind.TopLevelVariableDeclaration ||
                 !string.Equals(elementLibrary, library, StringComparison.Ordinal)))
            {
                builder.Append(QualifiedLibraryStaticClassName(elementLibrary, library)).Append('.');
            }
            var mappedName = CurrentInvocationName();
            builder.Append(mappedName).Append(typeArguments);
        }
        else
        {
            var mappedName = CurrentInvocationName();
            builder.Append(mappedName).Append(typeArguments);
        }
        builder.Append('(');
        if (methodName != "toList")
        {
            var argumentInvocationName = methodName == "call" && target is not null
                ? target.Text(CoreProperty.name) ?? target.Children.LastOrDefault()?.Text(CoreProperty.name) ?? methodName
                : methodName;
            EmitArguments(
                builder,
                args,
                declaration,
                package,
                library,
                inputPath,
                diagnostics,
                preserveNames: methodName != "call",
                expectedParameters: ResolveInvocationMember(node, target, declaration, methodName)?.Element.Parameters,
                expectedArgumentTypes: ResolveInvocationParameterTypes(node, target, declaration, methodName),
                invocationName: argumentInvocationName,
                castDynamicArguments: target?.Kind == CoreNodeKind.SuperExpression);
        }
        builder.Append(')');
        if (dynamicInvocationResultType is not null) builder.Append(')');
    }

    private void EmitSetRange(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        int indent,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var target = node.Child(CoreChildRole.targetOffset);
        var arguments = node.Child(CoreChildRole.argumentsOffset);
        var argumentList = arguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
        if (target is null || argumentList.Length < 4)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "setrange-shape", "setRange requires a target, start, end, source and sourceOffset.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        var prefix = new string(' ', indent * 4);
        var start = argumentList[0];
        var end = argumentList[1];
        var source = argumentList[2];
        var sourceOffset = argumentList[3];
        builder.AppendLine("{");
        builder.Append(prefix).Append("    var __e = ");
        LowerExpression(builder, source, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine(".GetEnumerator();");
        builder.Append(prefix).Append("    for (var __i = 0; __i < ");
        LowerExpression(builder, sourceOffset, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine("; __i++) __e.MoveNext();");
        builder.Append(prefix).Append("    for (var __i = ");
        LowerExpression(builder, start, declaration, package, library, inputPath, diagnostics);
        builder.Append("; __i < ");
        LowerExpression(builder, end, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine("; __i++)");
        builder.Append(prefix).AppendLine("    {");
        builder.Append(prefix).AppendLine("        __e.MoveNext();");
        builder.Append(prefix).Append("        ");
        LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine("[__i] = __e.Current;");
        builder.Append(prefix).AppendLine("    }");
        builder.Append(prefix).Append("}");
    }

    private string MapMethodInvocationName(string name, string? targetType)
    {
        if (string.IsNullOrEmpty(targetType) && _session.ActiveDonorDeclaration?.Members.Any(member =>
                !member.IsStatic && string.Equals(member.Name, name, StringComparison.Ordinal)) == true)
        {
            return SafeIdentifier(name);
        }
        if (name == "compareTo" && targetType is { Length: > 0 })
        {
            var targetDeclaration = FindGlobalDeclaration(MapType(targetType).TrimEnd('?'));
            if (targetDeclaration?.Members.Any(member => member.Name == "compareTo") == true)
            {
                return "compareTo";
            }
        }
        if ((targetType?.TrimEnd('?') is "String" or "string") && (name is "indexOf" or "trim"))
        {
            return name switch
            {
                "indexOf" => "IndexOf",
                "trim" => "Trim",
                _ => SafeIdentifier(name),
            };
        }
        if (targetType?.TrimEnd('?') == "DateTime")
        {
            return name switch
            {
                "toLocal" => "ToLocalTime",
                "toUtc" => "ToUniversalTime",
                _ => SafeIdentifier(name),
            };
        }
        var collection = IsDartCollectionType(targetType);
        if (name == "insert" && !collection) return "insert";
        if (name == "remove" && targetType?.TrimEnd('?') is { } mapType &&
            (mapType.StartsWith("Map<", StringComparison.Ordinal) || mapType.StartsWith("DartMap<", StringComparison.Ordinal)))
        {
            return "remove";
        }
        if (name == "add")
        {
            var value = targetType?.TrimEnd('?') ?? string.Empty;
            if (value.StartsWith("Queue<", StringComparison.Ordinal)) return "Enqueue";
            if (value.StartsWith("PriorityQueue<", StringComparison.Ordinal) ||
                value.StartsWith("HeapPriorityQueue<", StringComparison.Ordinal)) return "Add";
            return collection ? "Add" : "add";
        }
        if (name == "addAll") return collection
            ? targetType?.Contains("Set", StringComparison.Ordinal) == true ? "UnionWith" : "AddRange"
            : "addAll";
        if (name == "remove") return collection ? "Remove" : "remove";
        if (name == "clear") return collection ? "Clear" : "clear";
        if (name == "contains") return collection ? "Contains" : "contains";
        if (name == "indexOf") return collection ? "IndexOf" : "indexOf";
        if (name == "lastIndexOf") return collection ? "LastIndexOf" : "lastIndexOf";
        if (name is "firstKey" or "lastKey" && targetType?.TrimEnd('?') is { } sortedMapType &&
            (sortedMapType.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
             sortedMapType.StartsWith("SortedDictionary<", StringComparison.Ordinal)))
        {
            return name == "firstKey" ? "Keys.First" : "Keys.Last";
        }
        if (name == "removeFirst" && targetType?.TrimEnd('?').StartsWith("Queue<", StringComparison.Ordinal) == true) return "Dequeue";
        if (name == "whereType") return "OfType";
        if (targetType?.TrimEnd('?') == "Stopwatch")
        {
            return name switch
            {
                "start" => "Start",
                "stop" => "Stop",
                "reset" => "Reset",
                _ => SafeIdentifier(name),
            };
        }
        if (name == "addAll" && targetType is not null &&
            (targetType.Contains("HashSet", StringComparison.Ordinal) || targetType.Contains("Set", StringComparison.Ordinal)))
        {
            return "UnionWith";
        }
        if (name == "clear" && targetType is not null &&
            !targetType.Contains("List", StringComparison.Ordinal) &&
            !targetType.Contains("Set", StringComparison.Ordinal) &&
            !targetType.Contains("Map", StringComparison.Ordinal) &&
            !targetType.Contains("Collection", StringComparison.Ordinal))
        {
            return "clear";
        }
        if (name == "remove" && targetType?.Contains("Map<", StringComparison.Ordinal) == true)
        {
            return "remove";
        }
        return MapMemberName(name);
    }

    private bool IsDartCollectionType(string? type)
    {
        var value = (type?.TrimEnd('?') ?? string.Empty)
            .Replace("global::System.Collections.Generic.", string.Empty, StringComparison.Ordinal)
            .Replace("System.Collections.Generic.", string.Empty, StringComparison.Ordinal);
        return value.StartsWith("List<", StringComparison.Ordinal) ||
            value.StartsWith("Set<", StringComparison.Ordinal) ||
            value.StartsWith("HashSet<", StringComparison.Ordinal) ||
            value.StartsWith("SplayTreeSet<", StringComparison.Ordinal) ||
            value.StartsWith("SortedSet<", StringComparison.Ordinal) ||
            value.StartsWith("SplayTreeMap<", StringComparison.Ordinal) ||
            value.StartsWith("SortedDictionary<", StringComparison.Ordinal) ||
            value.StartsWith("Map<", StringComparison.Ordinal) ||
            value.StartsWith("DartMap<", StringComparison.Ordinal) ||
            value.StartsWith("Queue<", StringComparison.Ordinal) ||
            value.StartsWith("IList<", StringComparison.Ordinal) ||
            value.StartsWith("ICollection<", StringComparison.Ordinal);
    }

    private string DartMapTypeArguments(string mappedType)
    {
        var type = mappedType.TrimEnd('?');
        return type.StartsWith("DartMap<", StringComparison.Ordinal) && type.EndsWith('>')
            ? type[8..^1]
            : "object, object";
    }

    private bool IsDartTypedDataList(string? type) => type?.TrimEnd('?') is
        "Float32List" or "Float64List" or "Int8List" or "Uint8List" or
        "Int16List" or "Uint16List" or "Int32List" or "Uint32List" or
        "Int64List" or "Uint64List";

    private bool IsDartEnumerableType(string? type)
    {
        var value = type?.TrimEnd('?') ?? string.Empty;
        if (IsDartCollectionType(value) ||
            value.StartsWith("Iterable<", StringComparison.Ordinal) ||
            value.StartsWith("IEnumerable<", StringComparison.Ordinal) ||
            value.StartsWith("IReadOnly", StringComparison.Ordinal))
        {
            return true;
        }
        var pending = new Queue<CoreResolvedDeclaration>();
        var visited = new HashSet<string>(StringComparer.Ordinal);
        if (FindGlobalDeclaration(MapType(value)) is { } declaration) pending.Enqueue(declaration);
        while (pending.Count > 0)
        {
            var candidate = pending.Dequeue();
            if (!visited.Add(candidate.Element.CanonicalId)) continue;
            foreach (var baseName in DirectBaseNames(candidate))
            {
                var simpleBase = StripLibraryPrefix(baseName).TrimEnd('?');
                if (simpleBase.StartsWith("Iterable<", StringComparison.Ordinal) ||
                    simpleBase.StartsWith("IEnumerable<", StringComparison.Ordinal))
                {
                    return true;
                }
                if (FindGlobalDeclaration(baseName) is { } baseDeclaration) pending.Enqueue(baseDeclaration);
            }
        }
        return false;
    }

    private void EmitInstanceCreation(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var constructorName = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ConstructorName);
        var typeName = constructorName is null ? MapType(node.StaticType ?? "object") : MapTypeFromAst(constructorName);
        var staticTypeName = MapType(node.StaticType ?? string.Empty);
        if (typeName == "dynamic" && constructorName is not null &&
            DescendantsAndSelf(constructorName).Any(item => item.Text(CoreProperty.name) == "Router"))
        {
            // Router's type argument is inferred from delegate fields while the
            // creation expression itself is contextually typed as Widget. The
            // constructor AST can therefore surface `dynamic`; retain a closed
            // CLR generic instead of attempting `new dynamic(...)`.
            typeName = "Router<object>";
        }
        if (!typeName.Contains('<', StringComparison.Ordinal) &&
            staticTypeName.StartsWith(typeName + "<", StringComparison.Ordinal))
        {
            typeName = staticTypeName;
        }
        if (Regex.IsMatch(typeName, @"\bT\b", RegexOptions.CultureInvariant))
        {
            var owner = _session.ActiveDonorDeclaration ?? declaration;
            var contextualType = owner.Members
                .Where(member => member.Kind == "field" && ContainsOffset(member.Ast, node.Offset))
                .Select(member => MapType(member.Element.Type ?? string.Empty))
                .FirstOrDefault(candidate => !Regex.IsMatch(candidate, @"\bT\b", RegexOptions.CultureInvariant));
            contextualType ??= DescendantsAndSelf(owner.Ast)
                .Where(candidate => candidate.Kind == CoreNodeKind.VariableDeclaration && ContainsOffset(candidate, node.Offset))
                .OrderBy(candidate => candidate.Length)
                .Select(candidate => MapType(candidate.StaticType ?? string.Empty))
                .FirstOrDefault(candidate => !Regex.IsMatch(candidate, @"\bT\b", RegexOptions.CultureInvariant));
            if (!string.IsNullOrEmpty(contextualType) &&
                string.Equals(typeName.Split('<')[0], contextualType.Split('<')[0], StringComparison.Ordinal))
            {
                typeName = contextualType;
            }
        }
        var constructor = node.Text(CoreProperty.constructor)
            ?? (constructorName?.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier) is { } nameNode
                ? nameNode.Text(CoreProperty.name)
                : null);
        var arguments = node.Child(CoreChildRole.argumentsOffset);
        var argumentList = arguments?.Children.Where(item => item.Category == "expression").ToArray() ?? [];
        if ((typeName.StartsWith("HashSet<", StringComparison.Ordinal) ||
             typeName.StartsWith("DartMap<", StringComparison.Ordinal)) && argumentList.Length == 0)
        {
            builder.Append("new ").Append(typeName).Append("()");
            return;
        }
        if (DescendantsAndSelf(node).Any(item => item.Text(CoreProperty.name)?.Contains("GlobalKey", StringComparison.Ordinal) == true))
        {
            var globalKeyType = staticTypeName.Contains("GlobalKey<", StringComparison.Ordinal) ? staticTypeName : typeName;
            builder.Append(globalKeyType).Append(".Create(");
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (typeName.Contains("DiagnosticsProperty<", StringComparison.Ordinal) && argumentList.Length > 0)
        {
            builder.Append("new ").Append(typeName).Append('(');
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (typeName.EndsWith("StackTrace", StringComparison.Ordinal) && argumentList.Length == 1)
        {
            builder.Append("DartRuntimePrimitives.StackTraceFrom(");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (typeName == "object" && argumentList.Length > 0)
        {
            // Dart metadata markers such as pragma are erased to System.Object
            // in the runtime boundary. Their descriptive constructor argument
            // has no CLR object-constructor equivalent.
            builder.Append("new object()");
            return;
        }
        if (typeName.StartsWith("IEnumerable<", StringComparison.Ordinal) &&
            typeName.EndsWith('>') && argumentList.Length == 2)
        {
            builder.Append("System.Linq.Enumerable.Range(0, checked((int)");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(")).Select(__index => ((Func<long, ").Append(typeName[12..^1]).Append(">)");
            LowerExpression(builder, argumentList[1], declaration, package, library, inputPath, diagnostics);
            builder.Append(")(checked((long)__index)))");
            return;
        }
        if (typeName.StartsWith("IEnumerable<", StringComparison.Ordinal) &&
            typeName.EndsWith('>') && argumentList.Length == 0)
        {
            builder.Append("System.Linq.Enumerable.Empty<").Append(typeName[12..^1]).Append(">()");
            return;
        }
        if (constructor == "fromLTRB" && staticTypeName.EndsWith("EdgeInsets", StringComparison.Ordinal))
        {
            typeName = staticTypeName;
        }

        if (constructor == "fromEnvironment" &&
            typeName is ("bool" or "string" or "int") &&
            TryEmitFromEnvironmentLiteral(builder, argumentList, typeName))
        {
            return;
        }

        if (typeName == "DateTime" && constructor is null or "" or "new")
        {
            builder.Append("DartRuntimePrimitives.CreateDateTime(");
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        if (constructor == "hasEnvironment" && typeName == "bool" && argumentList.Length > 0)
        {
            builder.Append("Environment.GetEnvironmentVariable(");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append(") is not null");
            return;
        }

        if (typeName.EndsWith("TextSelection", StringComparison.Ordinal) &&
            constructor is "collapsed" or "fromPosition")
        {
            builder.Append(typeName).Append(constructor == "collapsed" ? ".CreateCollapsed(" : ".CreateFromPosition(");
            EmitArguments(builder, arguments, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }

        if (constructor == "sublistView" && typeName == "ByteData" && argumentList.Length > 0)
        {
            builder.Append("new ByteData(new Uint8List(");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("))");
            return;
        }

        if (constructor == "value" && typeName == "Task")
        {
            builder.Append("Task.CompletedTask");
            return;
        }
        if (constructor == "fromCharCode" && typeName == "string" && argumentList.Length > 0)
        {
            builder.Append("char.ConvertFromUtf32(checked((int)");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("))");
            return;
        }
        if (constructor == "value" && typeName == "Future")
        {
            builder.Append("Future.value()");
            return;
        }
        if (constructor == "value" && typeName.StartsWith("Future<", StringComparison.Ordinal))
        {
            builder.Append(typeName).Append(".value(");
            if (argumentList.Length == 0)
            {
                builder.Append("default!");
            }
            else
            {
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            }
            builder.Append(')');
            return;
        }
        if (constructor == "value" && typeName.StartsWith("Task<", StringComparison.Ordinal))
        {
            builder.Append("Task.FromResult(");
            if (argumentList.Length == 0)
            {
                builder.Append("default!");
            }
            else
            {
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            }
            builder.Append(')');
            return;
        }
        if (typeName == "ImageStreamListener" &&
            _currentLibrary.EndsWith("/image_cache.dart", StringComparison.Ordinal) &&
            argumentList.Length == 1)
        {
            builder.Append("new ImageStreamListener((Action<ImageInfo, bool>)((image, synchronousCall) => listener(image, synchronousCall)))");
            return;
        }

        if (constructorName is not null && typeName.StartsWith("List<", StringComparison.Ordinal) && typeName.EndsWith('>'))
        {
            var elementType = typeName[5..^1];
            if (constructor == "filled" && argumentList.Length >= 2)
            {
                builder.Append("new ").Append(typeName).Append("(System.Linq.Enumerable.Repeat<").Append(elementType).Append(">(");
                LowerExpression(builder, argumentList[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(", checked((int)");
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(")))");
                return;
            }
            if (constructor == "generate" && argumentList.Length >= 2)
            {
                builder.Append("new ").Append(typeName).Append("(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)");
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
                builder.Append(")), ");
                LowerExpression(builder, argumentList[1], declaration, package, library, inputPath, diagnostics);
                builder.Append("))");
                return;
            }
            if ((constructor is null or "new") && argumentList.Length == 1 && argumentList[0].StaticType == "int")
            {
                builder.Append("new ").Append(typeName).Append("(checked((int)");
                LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
                builder.Append("))");
                return;
            }
        }

        if (typeName.StartsWith("List<", StringComparison.Ordinal) && typeName.EndsWith('>') && argumentList.Length == 1 &&
            (MapType(ResolvedExpressionValueType(argumentList[0]) ?? argumentList[0].StaticType ?? string.Empty).StartsWith("IEnumerable<", StringComparison.Ordinal) ||
             MapType(ResolvedExpressionValueType(argumentList[0]) ?? argumentList[0].StaticType ?? string.Empty).StartsWith("IReadOnlyList<", StringComparison.Ordinal) ||
             MapType(ResolvedExpressionValueType(argumentList[0]) ?? argumentList[0].StaticType ?? string.Empty).StartsWith("List<", StringComparison.Ordinal)))
        {
            var elementType = typeName[5..^1];
            builder.Append("new ").Append(typeName).Append("(DartRuntimePrimitives.ConvertEnumerable<").Append(elementType).Append(">(");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("))");
            return;
        }

        if (typeName == "ArgumentException" && argumentList.Length == 1 &&
            MapType(argumentList[0].StaticType ?? "object") == "object")
        {
            builder.Append("new ArgumentException(");
            LowerExpression(builder, argumentList[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("?.ToString())");
            return;
        }

        if (typeName is "Vector2" or "global::System.Numerics.Vector2" &&
            constructor == "array" &&
            argumentList is [{ Kind: CoreNodeKind.ListLiteral } vectorValues])
        {
            var values = vectorValues.Children.Where(item => item.Category == "expression").ToArray();
            if (values.Length == 2)
            {
                builder.Append("new Vector2(checked((float)");
                LowerExpression(builder, values[0], declaration, package, library, inputPath, diagnostics);
                builder.Append("), checked((float)");
                LowerExpression(builder, values[1], declaration, package, library, inputPath, diagnostics);
                builder.Append("))");
                return;
            }
        }

        if (typeName == "global::System.Numerics.Vector4" && argumentList.Length == 4)
        {
            builder.Append("new global::System.Numerics.Vector4(");
            for (var index = 0; index < argumentList.Length; index++)
            {
                if (index > 0) builder.Append(", ");
                builder.Append("checked((float)");
                LowerExpression(builder, argumentList[index], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            builder.Append(')');
            return;
        }

        if (typeName.Contains("GlobalKey<", StringComparison.Ordinal))
        {
            builder.Append(typeName).Append(".Create(");
        }
        else if (typeName.StartsWith("DefaultTransitionDelegate<", StringComparison.Ordinal) && constructor is null or "" or "new")
        {
            builder.Append("new ").Append(typeName).Append('(');
        }
        else if (typeName == "KeyHelper" && constructor is null or "new")
        {
            builder.Append("KeyHelper.Create(");
        }
        else if ((typeName is "Duration" or "global::Doroti.Runtime.Duration") &&
            (constructor is null or "new") &&
            argumentList.Any(item => item.Kind == CoreNodeKind.NamedExpression))
        {
            builder.Append("Duration.Create(");
        }
        else if (constructor == "all" && typeName.EndsWith("EdgeInsetsGeometry", StringComparison.Ordinal))
        {
            builder.Append(typeName[..^"Geometry".Length]).Append(".CreateAll(");
        }
        else if (constructor == "all" && typeName.EndsWith("EdgeInsets", StringComparison.Ordinal))
        {
            builder.Append(typeName).Append(".CreateAll(");
        }
        else if (constructor == "fromLTRB" && typeName.EndsWith("EdgeInsetsGeometry", StringComparison.Ordinal))
        {
            builder.Append("new ").Append(typeName[..^"Geometry".Length]).Append('(');
        }
        else if (constructor == "fromLTRB" && typeName.EndsWith("EdgeInsets", StringComparison.Ordinal))
        {
            builder.Append("new ").Append(typeName).Append('(');
        }
        else if (!string.IsNullOrEmpty(constructor) && constructor != "new" &&
            TryResolveEmittedNamedConstructor(typeName, constructor, out var namedConstructorMethod))
        {
            builder.Append(typeName).Append('.').Append(namedConstructorMethod).Append('(');
        }
        else if (constructor == "fromMouseEvent" &&
            (typeName.EndsWith("PointerEnterEvent", StringComparison.Ordinal) ||
             typeName.EndsWith("PointerExitEvent", StringComparison.Ordinal)))
        {
            builder.Append(typeName).Append(".CreateFromMouseEvent(");
        }
        else if (!string.IsNullOrEmpty(constructor) && constructor != "new" &&
            IsExternalStaticFactoryType(typeName))
        {
            // dart:ui and vector_math stay behind handwritten host/runtime
            // boundaries. Their named constructors are represented as static
            // factory methods because CLR constructors cannot carry Dart names.
            builder.Append(typeName).Append('.').Append(SafeIdentifier(constructor)).Append('(');
        }
        else if ((constructor is null or "" or "new") &&
            TryResolveEmittedDefaultFactoryConstructor(typeName, out var defaultFactoryMethod))
        {
            builder.Append(typeName).Append('.').Append(defaultFactoryMethod).Append('(');
        }
        else
        {
            builder.Append("new ").Append(typeName).Append('(');
        }
        if (typeName == "Tween<double>" && argumentList.Any(item =>
            item.Kind == CoreNodeKind.NamedExpression &&
            item.Child(CoreChildRole.expressionOffset)?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true))
        {
            for (var index = 0; index < argumentList.Length; index++)
            {
                if (index > 0) builder.Append(", ");
                var argument = argumentList[index];
                var value = argument.Kind == CoreNodeKind.NamedExpression
                    ? argument.Child(CoreChildRole.expressionOffset)
                    : argument;
                if (argument.Kind == CoreNodeKind.NamedExpression)
                {
                    builder.Append(SafeIdentifier(argument.Text(CoreProperty.name) ?? "value")).Append(": ");
                }
                if (value?.StaticType?.EndsWith("?", StringComparison.Ordinal) == true)
                {
                    builder.Append("DartRuntimePrimitives.RequireValue(");
                    LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
                    builder.Append(')');
                }
                else if (value is not null)
                {
                    LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
                }
            }
        }
        else
        {
            var resolvedConstructor = FindGlobalMember(constructorName?.ElementId) ?? FindGlobalMember(node.ElementId);
            if (resolvedConstructor is null && FindGlobalDeclaration(typeName) is { } constructorDeclaration)
            {
                var namedArguments = argumentList
                    .Where(argument => argument.Kind == CoreNodeKind.NamedExpression)
                    .Select(argument => argument.Text(CoreProperty.name))
                    .Where(argumentName => !string.IsNullOrEmpty(argumentName))
                    .ToHashSet(StringComparer.Ordinal);
                var positionalCount = argumentList.Count(argument => argument.Kind != CoreNodeKind.NamedExpression);
                var compatibleConstructors = constructorDeclaration.Members
                    .Where(member => member.Kind == "constructor" &&
                        (member.Element.Parameters ?? []).Count(parameter =>
                            parameter.Kind is not "optional-named" and not "required-named") >= positionalCount &&
                        namedArguments.All(argumentName =>
                            (member.Element.Parameters ?? []).Any(parameter => parameter.Name == argumentName)))
                    .ToArray();
                if (compatibleConstructors.Length == 1)
                {
                    resolvedConstructor = compatibleConstructors[0];
                }
            }
            var resolvedConstructorOwner = resolvedConstructor is null ? null : FindDeclaringDeclaration(resolvedConstructor);
            var constructorParameters = resolvedConstructor?.Element.Parameters;
            var constructorTypeParameters = resolvedConstructorOwner?.Element.TypeParameters ?? [];
            var substitutedConstructorParameters = false;
            if (constructorParameters is not null && constructorTypeParameters.Length > 0 &&
                TryGetGenericTypeArguments(typeName, out var constructorTypeArguments) &&
                constructorTypeArguments.Length == constructorTypeParameters.Length)
            {
                var substitutions = constructorTypeParameters
                    .Select((parameter, index) => new KeyValuePair<string, string>(parameter.Name, constructorTypeArguments[index]))
                    .ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);
                constructorParameters = constructorParameters
                    .Select(parameter => parameter with
                    {
                        Type = ApplyTypeParameterSubstitutions(parameter.Type, substitutions),
                    })
                    .ToArray();
                substitutedConstructorParameters = true;
            }
            var hasUnsubstitutedConstructorParameter = !substitutedConstructorParameters && constructorParameters?.Any(parameter =>
                constructorTypeParameters.Any(typeParameter =>
                    Regex.IsMatch(parameter.Type, $@"\b{Regex.Escape(typeParameter.Name)}\b", RegexOptions.CultureInvariant))) == true;
            EmitArguments(
                builder,
                arguments,
                declaration,
                package,
                library,
                inputPath,
                diagnostics,
                expectedParameters: hasUnsubstitutedConstructorParameter ? null : constructorParameters,
                invocationName: typeName,
                nullAsGenericDefault: ContainsUnboundTypeParameter(typeName) ||
                    (TryGetGenericTypeArguments(typeName, out var activeConstructorArguments) &&
                     activeConstructorArguments.Any(argument =>
                         IsTypeParameter(argument, _session.ActiveDonorDeclaration ?? declaration) ||
                         constructorTypeParameters.Any(parameter =>
                             string.Equals(parameter.Name, argument.TrimEnd('?'), StringComparison.Ordinal)))));
        }
        builder.Append(')');
    }

    private void EmitListLiteral(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var typeArgumentList = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList);
        var typeName = typeArgumentList is not null
            ? MapType($"List<{string.Join(", ", typeArgumentList.Children.Where(item => item.Category == "type").Select(MapTypeFromAst))}>")
            : MapType(node.StaticType ?? "List<object>");
        var collectionElements = node.Children
            .Where(item => item.Category is "expression" or "collection-element")
            .ToArray();
        if (collectionElements.Any(item => item.Category == "collection-element"))
        {
            EmitListLiteralWithControlFlow(
                builder,
                node,
                typeName,
                collectionElements,
                declaration,
                package,
                library,
                inputPath,
                diagnostics);
            return;
        }
        var elements = collectionElements;
        var elementType = typeName.StartsWith("List<", StringComparison.Ordinal) && typeName.EndsWith('>')
            ? typeName[5..^1]
            : string.Empty;
        builder.Append("new ").Append(typeName);
        if (elements.Length == 0)
        {
            builder.Append("()");
        }
        else
        {
            builder.Append(" { ");
            for (var index = 0; index < elements.Length; index++)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                if (elements[index].Kind == CoreNodeKind.AssignmentExpression) builder.Append('(');
                EmitListCollectionValue(builder, elements[index], elementType, declaration, package, library, inputPath, diagnostics);
                if (elements[index].Kind == CoreNodeKind.AssignmentExpression) builder.Append(')');
            }
            builder.Append(" }");
        }
    }

    private void EmitListLiteralWithControlFlow(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        string typeName,
        CoreAstNode[] elements,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var listName = $"__collection{node.Offset}";
        var elementType = typeName.StartsWith("List<", StringComparison.Ordinal) && typeName.EndsWith('>')
            ? typeName[5..^1]
            : string.Empty;
        builder.Append("((Func<").Append(typeName).Append(">)(() => { var ")
            .Append(listName).Append(" = new ").Append(typeName).Append("(); ");
        foreach (var element in elements)
        {
            EmitListCollectionElement(
                builder, listName, element, elementType, declaration, package, library, inputPath, diagnostics);
        }
        builder.Append("return ").Append(listName).Append("; }))()");
    }

    private void EmitListCollectionElement(
        CsSyntaxBuilder builder,
        string listName,
        CoreAstNode element,
        string elementType,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        bool setCollection = false,
        bool mapCollection = false)
    {
        if (mapCollection && element.Kind == CoreNodeKind.MapLiteralEntry)
        {
            var expressions = element.Children.Where(item => item.Category == "expression").ToArray();
            if (expressions.Length != 2)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                    "map-entry-shape", "Resolve the key and value expressions for the Dart map literal entry.");
                return;
            }
            builder.Append(listName).Append('[');
            LowerExpression(builder, expressions[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("] = ");
            LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            builder.Append("; ");
            return;
        }

        if (element.Category == "expression")
        {
            builder.Append(listName).Append(".Add(");
            EmitListCollectionValue(builder, element, elementType, declaration, package, library, inputPath, diagnostics);
            builder.Append("); ");
            return;
        }

        if (element.Kind == CoreNodeKind.SpreadElement)
        {
            var spread = element.Children.FirstOrDefault(item => item.Category == "expression");
            if (spread is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                    "collection-spread", "Expose the typed spread expression.");
                return;
            }
            var isNullAware = element.Length - spread.Length >= 4;
            if (isNullAware)
            {
                var spreadName = $"__collectionSpread{element.Offset}";
                builder.Append("var ").Append(spreadName).Append(" = ");
                LowerExpression(builder, spread, declaration, package, library, inputPath, diagnostics);
                builder.Append("; if (").Append(spreadName).Append(" is not null) { ")
                    .Append(listName).Append(setCollection ? ".UnionWith(" : ".AddRange(")
                    .Append(spreadName).Append("); } ");
            }
            else
            {
                builder.Append(listName).Append(setCollection ? ".UnionWith(" : ".AddRange(");
                LowerExpression(builder, spread, declaration, package, library, inputPath, diagnostics);
                builder.Append("); ");
            }
            return;
        }

        if (element.Kind == CoreNodeKind.NullAwareElement)
        {
            var value = element.Children.FirstOrDefault(item => item.Category == "expression");
            if (value is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                    "collection-null-aware", "Expose the typed null-aware collection expression.");
                return;
            }
            var valueName = $"__collectionElement{element.Offset}";
            var nonNullName = $"__nonNullCollectionElement{element.Offset}";
            builder.Append("var ").Append(valueName).Append(" = ");
            LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
            builder.Append("; if (").Append(valueName).Append(" is { } ").Append(nonNullName).Append(") { ")
                .Append(listName).Append(".Add(");
            if (elementType is "Widget" or "global::Doroti.Framework.Widgets.Widget")
            {
                builder.Append("DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(")
                    .Append(nonNullName).Append(')');
            }
            else
            {
                builder.Append(nonNullName);
            }
            builder.Append("); } ");
            return;
        }

        if (element.Kind == CoreNodeKind.IfElement)
        {
            var condition = element.Child(CoreChildRole.conditionOffset) ??
                element.Children.FirstOrDefault(item => item.Category == "expression");
            var caseClause = element.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.CaseClause);
            var branches = element.Children
                .Where(item => item.Kind == CoreNodeKind.MapLiteralEntry ||
                    item.Category is "expression" or "collection-element")
                .Where(item => !ReferenceEquals(item, condition))
                .ToArray();
            if (condition is null || branches.Length == 0)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                    "collection-if", "Expose the condition and collection branch elements.");
                return;
            }
            builder.Append("if (");
            if (caseClause is null)
            {
                LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
            }
            else
            {
                var guardedPattern = caseClause.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.GuardedPattern);
                EmitIfCaseCondition(
                    builder, condition, guardedPattern, declaration, package, library, inputPath, diagnostics);
            }
            builder.Append(") { ");
            EmitListCollectionElement(
                builder, listName, branches[0], elementType, declaration, package, library, inputPath, diagnostics,
                setCollection, mapCollection);
            builder.Append("} ");
            if (branches.Length > 1)
            {
                builder.Append("else { ");
                EmitListCollectionElement(
                    builder, listName, branches[1], elementType, declaration, package, library, inputPath, diagnostics,
                    setCollection, mapCollection);
                builder.Append("} ");
            }
            return;
        }

        if (element.Kind == CoreNodeKind.ForElement)
        {
            var parts = element.Children.FirstOrDefault(item =>
                item.Kind is CoreNodeKind.ForEachPartsWithDeclaration or CoreNodeKind.ForEachPartsWithIdentifier or
                    CoreNodeKind.ForPartsWithDeclarations or CoreNodeKind.ForPartsWithExpression);
            var body = element.Children.LastOrDefault(item => item.Kind == CoreNodeKind.MapLiteralEntry ||
                item.Category is "expression" or "collection-element");
            if (parts is null || body is null)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                    "collection-for", "Expose the typed loop parts and collection body.");
                return;
            }
            if (parts.Kind is CoreNodeKind.ForPartsWithDeclarations or CoreNodeKind.ForPartsWithExpression)
            {
                var variableList = parts.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.VariableDeclarationList);
                var expressions = parts.Children.Where(item => item.Category == "expression").ToArray();
                var initializer = variableList is null ? parts.Child(CoreChildRole.initializerOffset) : null;
                var condition = parts.Child(CoreChildRole.conditionOffset);
                if (initializer is null && condition is null)
                {
                    initializer = variableList is null ? expressions.FirstOrDefault() : null;
                    condition = variableList is null ? expressions.Skip(1).FirstOrDefault() : expressions.FirstOrDefault();
                }
                var updaters = expressions
                    .Where(item => item.Offset != initializer?.Offset && item.Offset != condition?.Offset)
                    .ToArray();
                builder.Append("for (");
                if (variableList is not null)
                {
                    EmitVariableDeclarationList(builder, variableList, declaration, package, library, inputPath, diagnostics);
                }
                else if (initializer is not null)
                {
                    LowerExpression(builder, initializer, declaration, package, library, inputPath, diagnostics);
                }
                builder.Append("; ");
                if (condition is not null)
                {
                    LowerExpression(builder, condition, declaration, package, library, inputPath, diagnostics);
                }
                builder.Append("; ");
                for (var index = 0; index < updaters.Length; index++)
                {
                    if (index > 0) builder.Append(", ");
                    LowerExpression(builder, updaters[index], declaration, package, library, inputPath, diagnostics);
                }
                builder.Append(") { ");
            }
            else
            {
                var identifier = parts.Children.FirstOrDefault(item =>
                    item.Kind is CoreNodeKind.DeclaredIdentifier or CoreNodeKind.SimpleIdentifier);
                var iterable = parts.Children.LastOrDefault(item =>
                    item != identifier && item.Category == "expression");
                if (identifier is null || iterable is null)
                {
                    AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
                        "collection-for", "Expose the declared identifier and iterable.");
                    return;
                }
                var identifierName = EmittedLocalIdentifier(identifier, identifier.Text(CoreProperty.name) ?? "value");
                builder.Append("foreach (var ").Append(identifierName).Append(" in ");
                LowerExpression(builder, iterable, declaration, package, library, inputPath, diagnostics);
                builder.Append(") { ");
            }
            EmitListCollectionElement(
                builder, listName, body, elementType, declaration, package, library, inputPath, diagnostics,
                setCollection, mapCollection);
            builder.Append("} ");
            return;
        }

        AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, element,
            "collection-element", "Add typed lowering for this collection element kind.");
    }

    private void EmitListCollectionValue(
        CsSyntaxBuilder builder,
        CoreAstNode value,
        string elementType,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (elementType is "Widget" or "global::Doroti.Framework.Widgets.Widget")
        {
            builder.Append("DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(");
            LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return;
        }
        LowerExpression(builder, value, declaration, package, library, inputPath, diagnostics);
    }

    private void EmitSetOrMapLiteral(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var typeArgumentList = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.TypeArgumentList);
        var arguments = typeArgumentList?.Children.Where(item => item.Category == "type").Select(MapTypeFromAst).ToArray() ?? [];
        var typeName = !string.IsNullOrEmpty(node.StaticType)
            ? MapType(node.StaticType)
            : arguments.Length switch
            {
                1 => MapType($"Set<{arguments[0]}>"),
                2 => MapType($"Map<{arguments[0]}, {arguments[1]}>"),
                _ => "object",
            };
        var mapEntries = node.Children.Where(item => item.Kind == CoreNodeKind.MapLiteralEntry).ToArray();
        var collectionElements = node.Children
            .Where(item => item.Category is "expression" or "collection-element")
            .ToArray();
        var setElements = collectionElements.Where(item => item.Category == "expression").ToArray();
        var mapValueType = TryGetGenericTypeArguments(typeName.TrimEnd('?'), out var mapArguments) &&
            mapArguments.Length == 2
                ? mapArguments[1]
                : string.Empty;
        var isSet = typeName.TrimEnd('?').StartsWith("HashSet<", StringComparison.Ordinal);
        if (isSet && collectionElements.Any(item => item.Category != "expression"))
        {
            var setName = $"__collection{node.Offset}";
            var elementType = TryGetGenericTypeArguments(typeName.TrimEnd('?'), out var setArguments) &&
                setArguments.Length == 1
                    ? setArguments[0]
                    : string.Empty;
            builder.Append("((Func<").Append(typeName).Append(">)(() => { var ")
                .Append(setName).Append(" = new ").Append(typeName).Append("(); ");
            foreach (var element in collectionElements)
            {
                EmitListCollectionElement(
                    builder, setName, element, elementType, declaration, package, library, inputPath, diagnostics,
                    setCollection: true);
            }
            builder.Append("return ").Append(setName).Append("; }))()");
            return;
        }

        if (!isSet && collectionElements.Any(item => item.Category != "expression"))
        {
            var mapName = $"__collection{node.Offset}";
            builder.Append("((Func<").Append(typeName).Append(">)(() => { var ")
                .Append(mapName).Append(" = new ").Append(typeName).Append("(); ");
            foreach (var element in node.Children.Where(item =>
                         item.Kind == CoreNodeKind.MapLiteralEntry ||
                         item.Category is "expression" or "collection-element"))
            {
                EmitListCollectionElement(
                    builder, mapName, element, string.Empty, declaration, package, library, inputPath, diagnostics,
                    mapCollection: true);
            }
            builder.Append("return ").Append(mapName).Append("; }))()");
            return;
        }

        builder.Append("new ").Append(typeName);
        if (mapEntries.Length == 0 && setElements.Length == 0)
        {
            builder.Append("()");
            return;
        }
        builder.Append(" { ");
        for (var index = 0; index < mapEntries.Length; index++)
        {
            if (index > 0) builder.Append(", ");
            var expressions = mapEntries[index].Children.Where(item => item.Category == "expression").ToArray();
            if (expressions.Length != 2)
            {
                AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, mapEntries[index],
                    "map-entry-shape", "Resolve the key and value expressions for the Dart map literal entry.");
                builder.Append("[default!] = default!");
                continue;
            }
            builder.Append('[');
            LowerExpression(builder, expressions[0], declaration, package, library, inputPath, diagnostics);
            builder.Append("] = ");
            var actualValueType = MapType(expressions[1].StaticType ?? string.Empty).TrimEnd('?');
            if (mapValueType.StartsWith("List<", StringComparison.Ordinal) &&
                actualValueType.StartsWith("List<", StringComparison.Ordinal) &&
                !string.Equals(mapValueType, actualValueType, StringComparison.Ordinal) &&
                TryGetGenericTypeArguments(mapValueType, out var mapListArguments) &&
                mapListArguments.Length == 1)
            {
                LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(".Cast<").Append(mapListArguments[0]).Append(">().ToList()");
            }
            else if (mapValueType.Length > 0 &&
                mapValueType.TrimEnd('?') is not ("object" or "dynamic" or "void") &&
                actualValueType is not ("void") &&
                !IsValueType(mapValueType.TrimEnd('?')) &&
                !string.Equals(mapValueType.TrimEnd('?'), actualValueType, StringComparison.Ordinal))
            {
                builder.Append("((").Append(mapValueType).Append(")(object?)");
                LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
                builder.Append(')');
            }
            else
            {
                LowerExpression(builder, expressions[1], declaration, package, library, inputPath, diagnostics);
            }
        }
        for (var index = 0; index < setElements.Length; index++)
        {
            if (mapEntries.Length > 0 || index > 0) builder.Append(", ");
            LowerExpression(builder, setElements[index], declaration, package, library, inputPath, diagnostics);
        }
        builder.Append(" }");
    }

    private void EmitFunctionExpression(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var parameters = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.FormalParameterList)?.Children ?? [];
        var parameterNames = parameters.Select((parameter, index) =>
        {
            var name = parameter.Text(CoreProperty.name);
            if (!string.IsNullOrEmpty(name))
            {
                return SafeIdentifier(name);
            }
            var identifier = DescendantsAndSelf(parameter).FirstOrDefault(item => item.Kind == CoreNodeKind.SimpleIdentifier);
            return identifier is not null ? SafeIdentifier(identifier.Text(CoreProperty.name) ?? $"arg{index}") : $"arg{index}";
        }).ToArray();
        var expressionBody = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.ExpressionFunctionBody);
        var blockBody = node.Children.FirstOrDefault(item => item.Kind == CoreNodeKind.BlockFunctionBody);
        var expression = expressionBody is null ? null : expressionBody.Child(CoreChildRole.expressionOffset);
        var block = blockBody is null ? null : blockBody.Child(CoreChildRole.blockOffset);
        var isAsync = IsDartAsync(node);
        var mappedFunctionType = MapType(node.StaticType ?? string.Empty).TrimEnd('?');
        var normalizedFunctionType = mappedFunctionType.Replace("global::System.", string.Empty, StringComparison.Ordinal);
        var contextualLambdaReturnType = _session.ContextualLambdaReturnType;
        _session.ContextualLambdaReturnType = null;
        if (contextualLambdaReturnType == "void")
        {
            if (TryGetGenericTypeArguments(normalizedFunctionType, out var contextualArguments) &&
                normalizedFunctionType.StartsWith("Func<", StringComparison.Ordinal) &&
                contextualArguments.Length > 1)
            {
                normalizedFunctionType = $"Action<{string.Join(", ", contextualArguments[..^1])}>";
                mappedFunctionType = "global::System." + normalizedFunctionType;
            }
            else
            {
                mappedFunctionType = "global::System.Action";
                normalizedFunctionType = "Action";
            }
        }
        else if (!string.IsNullOrEmpty(contextualLambdaReturnType) &&
            normalizedFunctionType.StartsWith("Func<", StringComparison.Ordinal) &&
            TryGetGenericTypeArguments(normalizedFunctionType, out var contextualFuncArguments) &&
            contextualFuncArguments.Length > 0)
        {
            contextualFuncArguments[^1] = MapType(contextualLambdaReturnType);
            normalizedFunctionType = $"Func<{string.Join(", ", contextualFuncArguments)}>";
            mappedFunctionType = "global::System." + normalizedFunctionType;
        }
        builder.Append('(');
        var emitAsync = isAsync &&
            (!TryGetGenericTypeArguments(normalizedFunctionType, out var asyncFunctionArguments) ||
             asyncFunctionArguments.Length == 0 ||
             asyncFunctionArguments[^1].StartsWith("Future", StringComparison.Ordinal));
        if (emitAsync)
        {
            builder.Append("async ");
        }
        builder.Append('(').Append(string.Join(", ", parameterNames)).Append(") => ");
        if (expression is not null)
        {
            if (normalizedFunctionType is "Action" || normalizedFunctionType.StartsWith("Action<", StringComparison.Ordinal))
            {
                builder.Append("{ ");
                if (expression.Kind != CoreNodeKind.NullLiteral && MapType(expression.StaticType ?? string.Empty) != "void")
                {
                    builder.Append("_ = ");
                }
                if (expression.Kind != CoreNodeKind.NullLiteral)
                {
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                    builder.Append("; ");
                }
                builder.Append('}');
            }
            else
            {
                LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
            }
        }
        else if (block is not null)
        {
            builder.AppendLine("{");
            var dartFunctionType = node.StaticType ?? string.Empty;
            if (contextualLambdaReturnType is null)
            {
                mappedFunctionType = MapType(dartFunctionType);
                normalizedFunctionType = mappedFunctionType.Replace("global::System.", string.Empty, StringComparison.Ordinal);
            }
            var functionMarker = dartFunctionType.IndexOf(" Function", StringComparison.Ordinal);
            var dartReturnType = !string.IsNullOrEmpty(contextualLambdaReturnType)
                ? contextualLambdaReturnType
                : normalizedFunctionType is "Action" || normalizedFunctionType.StartsWith("Action<", StringComparison.Ordinal)
                    ? "void"
                    : functionMarker > 0 ? dartFunctionType[..functionMarker].Trim() : string.Empty;
            var previousReturnType = _session.ActiveFunctionReturnType;
            _session.ActiveFunctionReturnType = dartReturnType.Length > 0 ? MapType(dartReturnType) : null;
            try
            {
                EmitBlockBody(builder, block, declaration, package, library, inputPath, diagnostics, 0);
            }
            finally
            {
                _session.ActiveFunctionReturnType = previousReturnType;
            }
            var returnsValue = dartReturnType.Length > 0 &&
                !string.Equals(dartReturnType, "void", StringComparison.Ordinal) &&
                !string.Equals(dartReturnType, "dynamic", StringComparison.Ordinal) &&
                !string.Equals(dartReturnType, "Null", StringComparison.Ordinal) &&
                !dartReturnType.StartsWith("Future<void>", StringComparison.Ordinal);
            var hasValueReturn = DescendantsExcludingNestedFunctions(block).Any(statement =>
                statement.Kind == CoreNodeKind.ReturnStatement &&
                statement.Children.Any(child => child.Category == "expression"));
            var asyncReturnsValue = isAsync &&
                TryGetGenericTypeArguments(mappedFunctionType.TrimEnd('?'), out var functionArguments) &&
                functionArguments.Length > 0 &&
                functionArguments[^1].StartsWith("Future<", StringComparison.Ordinal);
            var requiresClrValue = normalizedFunctionType.StartsWith("Func<", StringComparison.Ordinal);
            if (requiresClrValue && !returnsValue && !asyncReturnsValue)
            {
                // Dart dynamic/Null callbacks may fall through and complete with
                // null. Their CLR Func representation still requires a return.
                builder.AppendLine("return default!;");
            }
            else if (requiresClrValue ||
                dartReturnType != "void" && ((!isAsync && (returnsValue || hasValueReturn)) || asyncReturnsValue))
            {
                // C# requires a value on the conservatively-unproven path. This
                // is unreachable for exhaustive Dart switches but remains needed
                // because enum-like engine values are lowered as open CLR types.
                builder.AppendLine("throw new InvalidOperationException(\"Dart closure completed without a value.\");");
            }
            builder.Append('}');
        }
        else
        {
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
        }
        builder.Append(')');
    }

    private void EmitCascade(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var hasCascadeSection = node.Children.Any(item => item.Kind == CoreNodeKind.CascadeSection);
        var target = hasCascadeSection
            ? node.Children.FirstOrDefault(item => item.Category == "expression" && item.Kind != CoreNodeKind.CascadeSection)
            : node.Children.FirstOrDefault(item => item.StaticType == node.StaticType)
                ?? node.Children.FirstOrDefault();
        var sections = hasCascadeSection
            ? node.Children.Where(item => item.Kind == CoreNodeKind.CascadeSection).ToArray()
            : node.Children.Where(item => item != target).ToArray();
        if (target is null)
        {
            AddUnsupportedDiagnostic(diagnostics, package, library, inputPath, declaration, node,
                "cascade-shape", "Provide the typed cascade target and sections.");
            builder.Append("throw new NotSupportedException(\"DOTF0001\")");
            return;
        }
        var targetType = MapType(node.StaticType ?? target.StaticType ?? "object");
        builder.Append("((Func<").Append(targetType).Append(">)(() =>\n{");
        builder.Append("            var __cascade = ");
        LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
        builder.AppendLine(";");
        foreach (var section in sections)
        {
            var expression = section.Kind == CoreNodeKind.CascadeSection
                ? section.Children.FirstOrDefault(item => item.Category == "expression")
                : section;
            if (expression is not null)
            {
                builder.Append("            __cascade");
                var cascadeAssignmentLeft = expression.Kind == CoreNodeKind.AssignmentExpression
                    ? expression.Child(CoreChildRole.leftOffset)
                    : null;
                if (cascadeAssignmentLeft?.Kind == CoreNodeKind.IndexExpression &&
                    cascadeAssignmentLeft.Child(CoreChildRole.targetOffset) is null)
                {
                    var index = cascadeAssignmentLeft.Children.FirstOrDefault(item => item.Category == "expression");
                    var right = expression.Child(CoreChildRole.rightOffset);
                    builder.Append('[');
                    if (index is not null) LowerExpression(builder, index, declaration, package, library, inputPath, diagnostics);
                    else builder.Append('0');
                    builder.Append("] ").Append(MapOperator(expression.Text(CoreProperty.@operator) ?? "=")).Append(' ');
                    if (right is not null) LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                    else builder.Append("default");
                }
                else if (expression.Kind == CoreNodeKind.AssignmentExpression &&
                    cascadeAssignmentLeft is not null)
                {
                    var right = expression.Child(CoreChildRole.rightOffset);
                    var assignmentOperator = MapOperator(expression.Text(CoreProperty.@operator) ?? "=");
                    builder.Append('.');
                    LowerExpression(builder, cascadeAssignmentLeft, declaration, package, library, inputPath, diagnostics);
                    builder.Append(' ').Append(assignmentOperator).Append(' ');
                    if (right is null)
                    {
                        builder.Append("default");
                    }
                    else
                    {
                        var mappedLeftType = MapType(cascadeAssignmentLeft.StaticType ?? "object").TrimEnd('?');
                        var mappedRightType = MapType(right.StaticType ?? "object");
                        var hasNullableRightValue = mappedRightType == mappedLeftType + "?" ||
                            HasNullableValueStorage(right, declaration) ||
                            DescendantsAndSelf(right).Any(candidate =>
                                MapType(candidate.StaticType ?? string.Empty) == mappedLeftType + "?");
                        var pairedGetterRequiresValue = !string.IsNullOrEmpty(cascadeAssignmentLeft.ElementId) &&
                            (_currentDeclarations ?? []).SelectMany(candidate => candidate.Members)
                                .Where(member => string.Equals(
                                    member.Element.CanonicalId,
                                    cascadeAssignmentLeft.ElementId,
                                    StringComparison.Ordinal))
                                .Select(member => MapType(member.Element.ReturnType ?? member.Element.Type ?? string.Empty))
                                .Any(memberType => memberType == mappedLeftType);
                        var requiresNullableValue = assignmentOperator == "=" &&
                            IsValueType(mappedLeftType) &&
                            hasNullableRightValue &&
                            (pairedGetterRequiresValue ||
                             !MapType(cascadeAssignmentLeft.StaticType ?? "object").EndsWith("?", StringComparison.Ordinal));
                        if (requiresNullableValue) builder.Append("DartRuntimePrimitives.RequireValue(");
                        LowerExpression(builder, right, declaration, package, library, inputPath, diagnostics);
                        if (requiresNullableValue) builder.Append(')');
                    }
                }
                else if (expression.Kind == CoreNodeKind.IndexExpression)
                {
                    var index = expression.Children.FirstOrDefault(item => item.Category == "expression");
                    builder.Append('[');
                    if (index is not null) LowerExpression(builder, index, declaration, package, library, inputPath, diagnostics);
                    else builder.Append('0');
                    builder.Append(']');
                }
                else if (expression.Kind == CoreNodeKind.MethodInvocation &&
                    expression.Child(CoreChildRole.targetOffset) is null)
                {
                    var cascadeMethod = expression.Text(CoreProperty.name) ?? "missing";
                    builder.Append('.').Append(MapMethodInvocationName(cascadeMethod, target.StaticType)).Append('(');
                    EmitArguments(
                        builder,
                        expression.Child(CoreChildRole.argumentsOffset),
                        declaration,
                        package,
                        library,
                        inputPath,
                        diagnostics,
                        expectedParameters: ResolveInvocationMember(expression, target, declaration, cascadeMethod)?.Element.Parameters,
                        expectedArgumentTypes: ResolveInvocationParameterTypes(expression, target, declaration, cascadeMethod),
                        invocationName: cascadeMethod);
                    builder.Append(')');
                }
                else
                {
                    builder.Append('.');
                    LowerExpression(builder, expression, declaration, package, library, inputPath, diagnostics);
                }
                builder.AppendLine(";");
            }
        }
        builder.Append("            return __cascade;");
        builder.Append("        }))()");
    }


    private bool TryEmitFromEnvironment(
        CsSyntaxBuilder builder,
        CoreAstNode node,
        CoreAstNode? target,
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
        var targetName = target is null ? null : target.Text(CoreProperty.name);
        if (targetName is not null &&
            targetName is not ("bool" or "String" or "int"))
        {
            return false;
        }
        var arguments = node.Child(CoreChildRole.argumentsOffset);
        var literals = arguments?.Children
            .Where(item => item.Category == "expression" && item.Kind == CoreNodeKind.SimpleStringLiteral)
            .ToArray() ?? [];
        return TryEmitFromEnvironmentLiteral(builder, literals, targetName);
    }

    private bool TryEmitFromEnvironmentLiteral(CsSyntaxBuilder builder, CoreAstNode[] argumentExpressions, string? requestedType = "bool")
    {
        var key = argumentExpressions
            .Where(item => item.Kind == CoreNodeKind.SimpleStringLiteral)
            .Select(item => item.Text(CoreProperty.value))
            .FirstOrDefault(item => item is not null);
        if (key is null)
        {
            return false;
        }
        if (requestedType is "String" or "string")
        {
            builder.Append("Environment.GetEnvironmentVariable(")
                .Append('"').Append(Escape(key)).Append("\")");
            return true;
        }
        if (requestedType == "int")
        {
            builder.Append("(long.TryParse(Environment.GetEnvironmentVariable(")
                .Append('"').Append(Escape(key)).Append("\"), out var __environmentValue) ? __environmentValue : 0L)");
            return true;
        }
        builder.Append(key switch
        {
            "dart.vm.product" => "FoundationRuntimePorts.kReleaseMode",
            "dart.vm.profile" => "FoundationRuntimePorts.kProfileMode",
            "dart.library.js_interop" => "false",
            "dart.tool.dart2wasm" => "false",
            _ => "false",
        });
        return true;
    }

    private string[] SplitGenericArguments(string value)
    {
        var result = new List<string>();
        var angle = 0;
        var round = 0;
        var square = 0;
        var curly = 0;
        var start = 0;
        for (var index = 0; index < value.Length; index++)
        {
            switch (value[index])
            {
                case '<': angle++; break;
                case '>': angle--; break;
                case '(': round++; break;
                case ')': round--; break;
                case '[': square++; break;
                case ']': square--; break;
                case '{': curly++; break;
                case '}': curly--; break;
            }
            if (value[index] == ',' && angle == 0 && round == 0 && square == 0 && curly == 0)
            {
                result.Add(value[start..index].Trim());
                start = index + 1;
            }
        }
        result.Add(value[start..].Trim());
        return result.ToArray();
    }

    private bool IsCSharpConstant(CoreAstNode? node) => node?.Kind is
        CoreNodeKind.BooleanLiteral or CoreNodeKind.IntegerLiteral or CoreNodeKind.DoubleLiteral or CoreNodeKind.SimpleStringLiteral;

    private string LibraryUriFromElementId(string? elementId)
    {
        if (elementId is null)
        {
            return string.Empty;
        }
        var marker = elementId.LastIndexOf('#');
        return marker >= 0 ? elementId[..marker] : elementId;
    }

    private CoreResolvedDeclaration? FindDeclaration(string? elementId) =>
        _currentDeclarations?.FirstOrDefault(declaration => declaration.Element.CanonicalId == elementId);

    private bool TryResolvePromotedMemberOwner(CoreAstNode node, string targetType, string memberName, out string ownerType)
    {
        ownerType = string.Empty;
        var elementId = node.ElementId;
        var marker = elementId?.LastIndexOf('#') ?? -1;
        if (marker < 0) return false;
        var symbol = elementId![(marker + 1)..];
        var separator = symbol.LastIndexOf('.');
        if (separator <= 0 || !string.Equals(symbol[(separator + 1)..], memberName, StringComparison.Ordinal)) return false;
        var ownerName = symbol[..separator];
        if (ownerName.Contains('.', StringComparison.Ordinal)) return false;
        var elementLibrary = LibraryUriFromElementId(elementId);
        var owner = _semanticIndex.FindDeclarationByCanonicalId($"{elementLibrary}#{ownerName}")
            ?? FindGlobalDeclaration(MapType(ownerName));
        var target = FindGlobalDeclaration(MapType(targetType));
        if (owner?.Name == "ViewportNotificationMixin" &&
            target?.Name == "Notification" && memberName == "_depth")
        {
            ownerType = EmittedTypeName(LibraryUriFromElementId(owner.Element.CanonicalId), owner.Name) +
                FormatTypeParameters(owner.Element.TypeParameters);
            return true;
        }
        if (owner is not null && target is null &&
            (_currentDeclarations?.Any(declaration =>
                declaration.Element.TypeParameters?.Any(parameter => parameter.Name == targetType) == true) == true) &&
            owner.Members.Any(member => member.Name == memberName))
        {
            // Dart flow promotion can intersect a type parameter with the
            // receiver's declared class (for example `ShapeBorder? a` promoted
            // by `a is T`). C# keeps only T's declared generic bound, so retain
            // the member owner as an explicit receiver cast.
            ownerType = EmittedTypeName(LibraryUriFromElementId(owner.Element.CanonicalId), owner.Name) +
                FormatTypeParameters(owner.Element.TypeParameters);
            return true;
        }
        if (owner is null || target is null ||
            !owner.Members.Any(member => member.Name == memberName) || !IsDescendantOf(owner, target))
        {
            if (owner is null || target is null || owner.Element.CanonicalId != target.Element.CanonicalId ||
                !owner.Members.Any(member => member.Name == memberName))
            {
                return false;
            }
        }
        ownerType = owner.Element.CanonicalId == target.Element.CanonicalId
            ? MapType(targetType).TrimEnd('?')
            : EmittedTypeName(LibraryUriFromElementId(owner.Element.CanonicalId), owner.Name) +
                FormatTypeParameters(owner.Element.TypeParameters);
        return true;
    }

    private bool IsCurrentTypeParameter(string typeName) =>
        _currentDeclarations?.Any(declaration =>
            declaration.Element.TypeParameters?.Any(parameter => parameter.Name == typeName) == true) == true;

    private bool RequiresDynamicDispatch(string? dartType, string? memberElementId)
    {
        if (dartType?.TrimEnd('?') is "Object" or "object") return true;
        var member = FindGlobalMember(memberElementId);
        var owner = member is null ? null : FindDeclaringDeclaration(member);
        var target = FindGlobalDeclaration(MapType(dartType ?? string.Empty).TrimEnd('?'));
        return owner is not null && target is not null &&
            owner.Element.CanonicalId != target.Element.CanonicalId &&
            !IsDescendantOf(target, owner);
    }

    private bool RequiresDynamicInvocationDispatch(string? dartType, string methodName)
    {
        var mappedType = MapType(dartType ?? string.Empty).TrimEnd('?');
        if (IsUnboundTypeParameterName(mappedType) && methodName == "isSupportedAspect")
        {
            return true;
        }
        if (mappedType is "dynamic" or "Object" or "object" ||
            mappedType.EndsWith(".RenderObject", StringComparison.Ordinal) ||
            mappedType == "RenderObject")
        {
            return true;
        }
        if (methodName == "invoke" && mappedType.Length == 0)
        {
            return true;
        }
        if (methodName is "debugValidateChild" or "getPositionForPoint" or "scheduleLayoutCallback" or
            "globalToLocal" or "localToGlobal" or "getRectForComposingRange" or "getEndpointsForSelection" or
            "setChild" or "setFlatChildren" or "_insertChild" or "_removeChild" or "_moveChild" or "_setChild" or
            "indexOf" or "insert" or "prepareInitialFrame" or "triggerRebuild" or "beginActivity" or "disposePostFrame" or "move" or
            "_dragCancelCallback" or "_updateCallback" or "toStringDeep")
        {
            return true;
        }
        var typeParameterName = (dartType ?? string.Empty).TrimEnd('?');
        var typeParameter = (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration)?.Element.TypeParameters?
            .FirstOrDefault(parameter => string.Equals(parameter.Name, typeParameterName, StringComparison.Ordinal));
        var mappedBound = MapType(typeParameter?.Bound ?? string.Empty).TrimEnd('?');
        return mappedBound == "RenderObject" || mappedBound.EndsWith(".RenderObject", StringComparison.Ordinal);
    }

    private bool RequiresDynamicPropertyDispatch(string? dartType, string? memberElementId, string memberName)
    {
        var mappedType = MapType(dartType ?? string.Empty).TrimEnd('?');
        return RequiresDynamicDispatch(dartType, memberElementId) ||
            mappedType is "dynamic" or "Object" or "object" ||
            mappedType == "RenderObject" || mappedType.EndsWith(".RenderObject", StringComparison.Ordinal) ||
            memberName is "child" or "center" or "debugChildIntegrityEnabled" or "_element" or
                "_updateCallback" or "_callback" or "_deferredLayoutChild" or "_dragCancelCallback" or
                "mouseTracker" or "simulation" or "invoke" or "textDirection" or
                "_currentDrag" or "lastOverlapsContent" or
                "lastShrinkOffset" ||
            memberName == "list" && memberElementId?.StartsWith("dart:ui#SemanticsRole.", StringComparison.Ordinal) != true;
    }

    private bool IsTopLevelElement(string? elementId, string name)
    {
        var marker = elementId?.LastIndexOf('#') ?? -1;
        return marker >= 0 && string.Equals(elementId![(marker + 1)..], name, StringComparison.Ordinal);
    }

    private string MapMemberName(string name) => name switch
    {
        "toString" => "ToString",
        "add" => "Add",
        "addAll" => "AddRange",
        "remove" => "Remove",
        "clear" => "Clear",
        "contains" => "Contains",
        "containsKey" => "ContainsKey",
        "indexOf" => "IndexOf",
        "toList" => "ToList",
        "setRange" => "SetRange",
        "moveNext" => "MoveNext",
        "join" => "Join",
        "call" => "Invoke",
        "compareTo" => "CompareTo",
        "getRange" => "GetRange",
        "insert" => "Insert",
        "insertAll" => "InsertRange",
        "removeAt" => "RemoveAt",
        "removeRange" => "RemoveRange",
        "sublist" => "GetRange",
        _ => SafeIdentifier(name),
    };

    private string MapPropertyName(string name) => name switch
    {
        "$1" => "Item1",
        "$2" => "Item2",
        "$3" => "Item3",
        "$4" => "Item4",
        "fromStandardMessageCodecMessage" => "CreateFromStandardMessageCodecMessage",
        "length" => "Count",
        "isEmpty" => "Count == 0",
        "isNotEmpty" => "Count != 0",
        "iterator" => "GetEnumerator()",
        "keys" => "Keys",
        "values" => "Values",
        "current" => "Current",
        "hashCode" => "GetHashCode()",
        _ => SafeIdentifier(name),
    };

    private string MapPropertyAccessName(string? elementId, string name, string? targetType)
    {
        if (name == "hashCode") return "GetHashCode()";
        var contractTargetType = ApplyTypeParameterSubstitutions(
            targetType ?? string.Empty,
            _session.ActiveMemberContractSubstitutions);
        var mappedTargetType = MapType(contractTargetType).TrimEnd('?');
        if (name == "name" &&
            (elementId?.Contains("#Enum.name", StringComparison.Ordinal) == true ||
             elementId?.Contains("#EnumName.name", StringComparison.Ordinal) == true ||
             FindGlobalDeclaration(mappedTargetType)?.Ast.Kind == CoreNodeKind.EnumDeclaration ||
             (_session.ActiveDonorDeclaration ?? _session.ActiveDeclaration)?.Element.TypeParameters?
                 .Any(parameter => parameter.Name == (targetType ?? string.Empty).TrimEnd('?') &&
                     MapType(parameter.Bound ?? string.Empty).TrimEnd('?') == "Enum") == true))
        {
            return "ToString()";
        }
        if (targetType?.TrimEnd('?') == "DateTime")
        {
            return name switch
            {
                "year" => "Year",
                "month" => "Month",
                "day" => "Day",
                "hour" => "Hour",
                "minute" => "Minute",
                "second" => "Second",
                "millisecond" => "Millisecond",
                "weekday" => "DayOfWeek.ToDartWeekday()",
                _ => MapPropertyName(name),
            };
        }
        if (IsDartCollectionType(targetType) && name is "length" or "isEmpty" or "isNotEmpty")
        {
            return MapPropertyName(name);
        }
        if (IsDartCollectionType(targetType) &&
            name is "add" or "remove" or "clear" or "contains" or "indexOf" or "lastIndexOf")
        {
            return MapMethodInvocationName(name, targetType);
        }
        var member = FindGlobalMember(elementId) ??
            FindGlobalDeclaration(MapType(targetType ?? string.Empty).TrimEnd('?'))?
                .Members.FirstOrDefault(candidate =>
                    string.Equals(candidate.Name, name, StringComparison.Ordinal));
        var owner = member is null ? null : FindDeclaringDeclaration(member);
        return member is not null && owner is not null &&
            !LibraryUriFromElementId(owner.Element.CanonicalId).StartsWith("dart:", StringComparison.Ordinal)
                ? MapMethodDeclarationName(member)
                : MapPropertyName(name);
    }

    private bool TryEmitEnumGetter(
        CsSyntaxBuilder builder,
        CoreAstNode target,
        string targetType,
        string memberName,
        CoreResolvedDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        if (targetType == "AnimationStatus" &&
            memberName is "isDismissed" or "isCompleted" or "isAnimating" or "isForwardOrCompleted")
        {
            builder.Append("global::Doroti.Framework.Animation.AnimationStatusMembers.")
                .Append(SafeIdentifier(memberName)).Append('(');
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(')');
            return true;
        }
        var contractTargetType = ApplyTypeParameterSubstitutions(
            targetType,
            _session.ActiveMemberContractSubstitutions);
        var enumDeclaration = FindGlobalDeclaration(contractTargetType);
        enumDeclaration ??= FindGlobalDeclaration(MapType(contractTargetType).TrimEnd('?'));
        if (enumDeclaration?.Ast.Kind == CoreNodeKind.EnumDeclaration && memberName == "name")
        {
            LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
            builder.Append(".ToString()");
            return true;
        }
        if (enumDeclaration?.Ast.Kind != CoreNodeKind.EnumDeclaration ||
            !enumDeclaration.Members.Any(member => member.IsGetter && member.Name == memberName))
        {
            return false;
        }

        var enumLibrary = LibraryUriFromElementId(enumDeclaration.Element.CanonicalId);
        var enumName = EmittedTypeName(enumLibrary, enumDeclaration.Name);
        builder.Append(enumName).Append("Members.").Append(SafeIdentifier(memberName)).Append('(');
        LowerExpression(builder, target, declaration, package, library, inputPath, diagnostics);
        builder.Append(')');
        return true;
    }

    private readonly HashSet<string> CSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
        "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
        "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if",
        "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null",
        "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref",
        "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
        "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
        "virtual", "void", "volatile", "while",
    };

    private string Escape(string value) => value
        .Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("\"", "\\\"", StringComparison.Ordinal)
        .Replace("\r", "\\r", StringComparison.Ordinal)
        .Replace("\n", "\\n", StringComparison.Ordinal);

    private string EscapeInterpolated(string value) => Escape(value)
        .Replace("{", "{{", StringComparison.Ordinal)
        .Replace("}", "}}", StringComparison.Ordinal);

    private static CsOrigin ToCsOrigin(SourceOrigin origin) => new(
        origin.Source,
        origin.Offset,
        origin.Length,
        origin.SymbolId?.Value);

}
