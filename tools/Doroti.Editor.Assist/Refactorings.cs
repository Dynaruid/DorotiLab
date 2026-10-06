using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Doroti.Editor.Assist;

public sealed class Refactorings(CompilationUnitSyntax root, SemanticModel model, INamedTypeSymbol widget,
    string source, Request request, CancellationToken token)
{
    private const string Widgets = "global::Doroti.Framework.Widgets.";
    private readonly string _nl = source.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
    private readonly TextSpan _selection = new(request.Offset, request.Length);
    private ExpressionSyntax? Target()
    {
        if (request.Offset < 0 || request.Offset > root.FullSpan.End || !Engine.IsCode(root, request.Offset)) return null;
        var expressions = root.FindToken(Math.Min(request.Offset, Math.Max(0, root.FullSpan.End - 1))).Parent!.AncestorsAndSelf().OfType<ExpressionSyntax>();
        foreach (var expression in expressions)
        {
            if (expression.ContainsDiagnostics || expression.ContainsDirectives || model.GetSymbolInfo(expression, token).Symbol is INamedTypeSymbol
                || model.GetDiagnostics(expression.Span, token).Any(d => d.Severity == DiagnosticSeverity.Error)) continue;
            if (!Engine.Derives(model.GetTypeInfo(expression, token).Type ?? model.GetTypeInfo(expression, token).ConvertedType, widget)) continue;
            if (request.Length == 0 || source[_selection.Start.._selection.End].Trim() == source[expression.Span.Start..expression.Span.End]) return expression;
        }
        return null;
    }

    private (TextSpan Span, ExpressionSyntax[] Items)? ListTarget()
    {
        if (request.Length == 0 || _selection.End > root.FullSpan.End) return null;
        foreach (var collection in root.DescendantNodes().Where(n => n is CollectionExpressionSyntax or InitializerExpressionSyntax))
        {
            if (!collection.FullSpan.Contains(_selection) || collection.ContainsDiagnostics || collection.ContainsDirectives) continue;
            var arg = collection.Parent as ArgumentSyntax ?? (collection.Parent is ObjectCreationExpressionSyntax or ArrayCreationExpressionSyntax ? collection.Parent.Parent as ArgumentSyntax : null);
            if (arg?.NameColon?.Name.Identifier.ValueText != "children") continue;
            var parent = arg.Parent?.Parent as BaseObjectCreationExpressionSyntax;
            if (parent is null || !Engine.Derives(model.GetTypeInfo(parent, token).Type, widget)) continue;
            var items = collection is CollectionExpressionSyntax c ? c.Elements.Select(e => e is ExpressionElementSyntax x ? x.Expression : e is SpreadElementSyntax s ? s.Expression : null).OfType<ExpressionSyntax>().ToArray()
                : ((InitializerExpressionSyntax)collection).Expressions.ToArray();
            var chosen = items.Where(e => _selection.IntersectsWith(e.Span)).ToArray();
            if (chosen.Length < 2) continue;
            var start = chosen[0].Span.Start;
            if (chosen[0].Parent is SpreadElementSyntax firstSpread) start = firstSpread.Span.Start;
            var end = chosen[^1].Span.End;
            if (_selection.Start > start || _selection.End < end) continue;
            // The outer comma remains with the parent collection. Interior commas/trivia/spreads travel unchanged.
            var span = TextSpan.FromBounds(start, end);
            var tail = source[end.._selection.End];
            var comma = tail.IndexOf(',');
            if (comma >= 0 && tail[..comma].Trim().Length == 0) tail = tail[(comma + 1)..];
            if (source[_selection.Start..start].Trim().Length != 0 || SyntaxFactory.ParseLeadingTrivia(tail).ToFullString() != tail) continue;
            if (chosen.Any(e => e.Parent is not SpreadElementSyntax && !Engine.Derives(model.GetTypeInfo(e, token).Type, widget))) continue;
            if (chosen.Where(e => e.Parent is SpreadElementSyntax).Any(e => !WidgetSequence(model.GetTypeInfo(e, token).Type))) continue;
            return (span, chosen);
        }
        return null;
    }

    private bool WidgetSequence(ITypeSymbol? type) => type is IArrayTypeSymbol a ? Engine.Derives(a.ElementType, widget)
        : type is INamedTypeSymbol n && n.AllInterfaces.Append(n).Any(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T && Engine.Derives(i.TypeArguments.FirstOrDefault(), widget));

    public Analysis Analyze(string? warning)
    {
        token.ThrowIfCancellationRequested();
        var code = Engine.IsCode(root, request.Offset);
        var cursor = root.FindToken(Math.Min(Math.Max(0, request.Offset), Math.Max(0, root.FullSpan.End - 1))).Parent!;
        var @class = cursor.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        var state = model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets.State`1");
        var classType = @class is null ? null : model.GetDeclaredSymbol(@class, token);
        var isState = Engine.Derives(classType, state);
        var stateBase = classType;
        while (stateBase is not null && !SymbolEqualityComparer.Default.Equals(stateBase.OriginalDefinition, state)) stateBase = stateBase.BaseType;
        var args = ConstructorArguments(cursor);
        var actions = new List<ActionInfo>();
        var expression = code ? Target() : null;
        var list = code ? ListTarget() : null;
        if (expression is not null || list is not null)
        {
            var wrappers = list is not null ? new[] { "Row", "Column", "Stack" }
                : new[] { "Center", "Padding", "Container", "SizedBox", "Align", "Row", "Column", "Stack", "Builder", "LayoutBuilder", "SingleChildScrollView" };
            var targetType = expression is null ? null : model.GetTypeInfo(expression, token).Type?.ToDisplayString();
            if (targetType is "Doroti.Framework.Widgets.Expanded" or "Doroti.Framework.Widgets.Flexible") wrappers = ["Row", "Column", "Builder"];
            if (targetType == "Doroti.Framework.Widgets.Positioned") wrappers = ["Stack", "Builder"];
            if (list is not null && list.Value.Items.Any(e => model.GetTypeInfo(e, token).Type?.ToDisplayString() is "Doroti.Framework.Widgets.Expanded" or "Doroti.Framework.Widgets.Flexible")) wrappers = ["Row", "Column"];
            if (list is not null && list.Value.Items.Any(e => model.GetTypeInfo(e, token).Type?.ToDisplayString() == "Doroti.Framework.Widgets.Positioned")) wrappers = ["Stack"];
            foreach (var wrapper in wrappers)
                if (model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets." + wrapper) is not null)
                    actions.Add(new("wrap:" + wrapper, "Wrap with " + wrapper));
            if (expression is not null)
            {
                var parent = DirectCollectionParent(expression);
                if (parent is "Row" or "Column" or "Flex")
                { actions.Add(new("wrap:Expanded", "Wrap with Expanded")); actions.Add(new("wrap:Flexible", "Wrap with Flexible")); }
                if (parent == "Stack") actions.Add(new("wrap:Positioned", "Wrap with Positioned"));
                if (Child(expression) is not null) actions.Add(new("remove", "Remove widget wrapper (preview)", true));
                var reason = ExtractionReason(expression);
                actions.Add(new("extract", "Extract Doroti Widget…", true, reason));
                foreach (var action in Replacements(expression)) actions.Add(action);
            }
        }
        if (@class is not null && Engine.Derives(classType, model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets.StatelessWidget")))
            actions.Add(new("stateful", "Convert to StatefulWidget (preview)", true, ConversionReason(@class)));
        var globals = model.Compilation.SyntaxTrees.SelectMany(t => t.GetRoot(token).DescendantNodes().OfType<UsingDirectiveSyntax>())
            .Where(u => u.GlobalKeyword.RawKind != 0).Select(u => u.Alias is null ? u.Name!.ToString().Replace("global::", "") : u.Alias.Name + "=" + u.Name).Distinct().ToArray();
        var conflicts = model.LookupSymbols(request.Offset).OfType<INamedTypeSymbol>().Where(t => !t.ContainingNamespace.ToDisplayString().StartsWith("Doroti.", StringComparison.Ordinal)).Select(t => t.Name)
            .Concat(root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u => u.Alias is not null).Select(u => u.Alias!.Name.Identifier.ValueText)).Distinct().ToArray();
        var insideMethod = cursor.AncestorsAndSelf().Any(n => n is BaseMethodDeclarationSyntax or AccessorDeclarationSyntax or AnonymousFunctionExpressionSyntax);
        var initializer = cursor.AncestorsAndSelf().Any(n => n is EqualsValueClauseSyntax);
        var widgetMember = Engine.Derives(classType, model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets.StatelessWidget"));
        var members = @class?.Members.SelectMany(m => m switch { MethodDeclarationSyntax method => new[] { method.Identifier.ValueText }, FieldDeclarationSyntax field => field.Declaration.Variables.Select(v => v.Identifier.ValueText), PropertyDeclarationSyntax property => new[] { property.Identifier.ValueText }, _ => Array.Empty<string>() }).ToArray() ?? [];
        return new(true, warning, true, code, args.Length > 0 ? "arguments" : initializer && !insideMethod ? "initializer" : isState ? insideMethod ? "stateBody" : "state" : expression is not null ? "widget" : @class is null ? "declaration" : insideMethod ? "body" : widgetMember ? "member" : "otherMember",
            members,
            stateBase?.TypeArguments.FirstOrDefault()?.ToDisplayString(Engine.DisplayFormat), args, actions.ToArray(), globals, conflicts,
            cursor.AncestorsAndSelf().OfType<ExpressionSyntax>().Select(DirectCollectionParent).FirstOrDefault(p => p is not null));
    }

    private ParameterInfo[] ConstructorArguments(SyntaxNode cursor)
    {
        var creation = cursor.AncestorsAndSelf().OfType<BaseObjectCreationExpressionSyntax>().FirstOrDefault(c => c.ArgumentList?.FullSpan.Contains(request.Offset) == true);
        if (creation?.ArgumentList is null) return [];
        // Only an argument boundary; do not offer named arguments inside an existing expression/lambda.
        if (creation.ArgumentList.Arguments.Any(a => a.Expression.Span.Start < request.Offset && request.Offset < a.Expression.Span.End)) return [];
        var symbolInfo = model.GetSymbolInfo(creation, token);
        var candidates = symbolInfo.Symbol is IMethodSymbol method ? new[] { method } : symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().ToArray();
        if (candidates.Length == 0 && model.GetTypeInfo(creation, token).Type is INamedTypeSymbol type) candidates = type.InstanceConstructors.ToArray();
        if (candidates.Length != 1 || !Engine.Derives(candidates[0].ContainingType, widget)) return [];
        var supplied = creation.ArgumentList.Arguments;
        var named = supplied.Where(a => a.NameColon is not null).Select(a => a.NameColon!.Name.Identifier.ValueText).ToHashSet();
        var positional = supplied.Count(a => a.NameColon is null && !a.Expression.IsMissing);
        return candidates[0].Parameters.Skip(positional).Where(p => !named.Contains(p.Name)).Select(Engine.Parameter).ToArray();
    }

    private string? DirectCollectionParent(ExpressionSyntax e)
    {
        SyntaxNode? collection = e.Parent is ExpressionElementSyntax element ? element.Parent : e.Parent is InitializerExpressionSyntax init ? init : null;
        var creation = collection?.Ancestors().OfType<BaseObjectCreationExpressionSyntax>().FirstOrDefault();
        return creation is null ? null : (model.GetTypeInfo(creation, token).Type as INamedTypeSymbol)?.ToDisplayString() switch
        {
            "Doroti.Framework.Widgets.Row" => "Row", "Doroti.Framework.Widgets.Column" => "Column",
            "Doroti.Framework.Widgets.Flex" => "Flex", "Doroti.Framework.Widgets.Stack" => "Stack", _ => null
        };
    }

    private ExpressionSyntax? Child(ExpressionSyntax e)
    {
        if (e is not BaseObjectCreationExpressionSyntax creation) return null;
        var type = model.GetTypeInfo(e, token).Type?.ToDisplayString();
        if (type is null || !new[] { "Center", "Padding", "Container", "SizedBox", "Align", "SingleChildScrollView", "Expanded", "Flexible", "Positioned", "ClipRRect" }.Any(n => type == "Doroti.Framework.Widgets." + n)) return null;
        var ctor = model.GetSymbolInfo(e, token).Symbol as IMethodSymbol;
        var args = creation.ArgumentList?.Arguments;
        if (ctor is null || args is null) return null;
        for (var i = 0; i < args.Value.Count; i++)
        {
            var arg = args.Value[i];
            var name = arg.NameColon?.Name.Identifier.ValueText ?? ctor.Parameters.ElementAtOrDefault(i)?.Name;
            if (name == "child" && Engine.Derives(model.GetTypeInfo(arg.Expression, token).Type, widget)) return arg.Expression;
        }
        return null;
    }

    private IEnumerable<ActionInfo> Replacements(ExpressionSyntax e)
    {
        if (e is not BaseObjectCreationExpressionSyntax c || c.ArgumentList is null) yield break;
        var name = model.GetTypeInfo(e, token).Type?.ToDisplayString();
        var args = c.ArgumentList.Arguments;
        if (name is "Doroti.Framework.Widgets.Row" or "Doroti.Framework.Widgets.Column" or "Doroti.Framework.Widgets.Stack")
        {
            foreach (var target in new[] { "Row", "Column", "Stack" })
            {
                if (name.EndsWith("." + target, StringComparison.Ordinal)) continue;
                var type = model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets." + target);
                if (!SafeParentData(target, args)) continue;
                if (type?.InstanceConstructors.Any(ctor => args.All(a => a.NameColon is not null && ctor.Parameters.Any(p => p.Name == a.NameColon.Name.Identifier.ValueText))) == true)
                    yield return new("swap:" + target, "Replace with " + target + " (preview)", true);
            }
            var children = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.ValueText == "children")?.Expression as CollectionExpressionSyntax;
            if (args.All(a => a.NameColon?.Name.Identifier.ValueText is "key" or "children") && children?.Elements is [ExpressionElementSyntax] && SafeParentData("Center", args))
                yield return new("child", "Convert children to child / Center (preview)", true);
        }
        else if (name == "Doroti.Framework.Widgets.Center" && args.All(a => a.NameColon?.Name.Identifier.ValueText is "key" or "child") && Child(e) is not null)
            yield return new("children", "Convert child to children / Column (preview)", true);
    }

    private bool SafeParentData(string target, SeparatedSyntaxList<ArgumentSyntax> args)
    {
        var collection = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.ValueText == "children")?.Expression as CollectionExpressionSyntax;
        if (collection is null) return true;
        foreach (var element in collection.Elements.OfType<ExpressionElementSyntax>())
        {
            var type = model.GetTypeInfo(element.Expression, token).Type?.ToDisplayString();
            if (type is "Doroti.Framework.Widgets.Expanded" or "Doroti.Framework.Widgets.Flexible" && target is not ("Row" or "Column")) return false;
            if (type == "Doroti.Framework.Widgets.Positioned" && target != "Stack") return false;
        }
        return true;
    }

    public Transform Transform(string action, string? name)
    {
        var allowed = Analyze(null).Actions.FirstOrDefault(a => a.Id == action);
        if (allowed is null || allowed.Reason is not null) return new([], true, allowed?.Reason ?? "Action no longer applies to this snapshot");
        var expression = Target();
        if (action.StartsWith("wrap:", StringComparison.Ordinal))
        {
            var list = ListTarget();
            var span = list?.Span ?? expression!.Span;
            var original = source[span.Start..span.End];
            var wrapper = action[5..];
            string wrapped;
            if (wrapper is "Row" or "Column" or "Stack") wrapped = $"new {Widgets}{wrapper}(children: [{original}])";
            else if (wrapper is "Builder" or "LayoutBuilder")
            {
                var context = Unique("builderContext"); var constraints = Unique("constraints");
                wrapped = $"new {Widgets}{wrapper}(builder: ({context}{(wrapper == "LayoutBuilder" ? ", " + constraints : "")}) => {original})";
            }
            else wrapped = $"new {Widgets}{wrapper}({(wrapper == "Padding" ? "padding: global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8), " : "")}child: {original})";
            return new([new(span.Start, span.Length, wrapped)], false);
        }
        if (action == "remove") return new([new(expression!.SpanStart, expression.Span.Length, Child(expression)!.ToFullString().Trim())], true);
        if (action == "extract") return Extract(expression!, name ?? "ExtractedWidget");
        if (action == "stateful")
        {
            var @class = root.FindToken(request.Offset).Parent!.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
            return Convert(@class);
        }
        if (expression is BaseObjectCreationExpressionSyntax creation)
        {
            var args = creation.ArgumentList!.Arguments;
            if (action.StartsWith("swap:", StringComparison.Ordinal))
                return new([new(expression.SpanStart, expression.Span.Length, $"new {Widgets}{action[5..]}{creation.ArgumentList}")], true);
            var key = args.FirstOrDefault(a => a.NameColon?.Name.Identifier.ValueText == "key");
            var keyText = key is null ? "" : key + ", ";
            if (action == "children") return new([new(expression.SpanStart, expression.Span.Length, $"new {Widgets}Column({keyText}children: [{Child(expression)}])")], true);
            if (action == "child")
            {
                var collection = (CollectionExpressionSyntax)args.First(a => a.NameColon?.Name.Identifier.ValueText == "children").Expression;
                return new([new(expression.SpanStart, expression.Span.Length, $"new {Widgets}Center({keyText}child: {((ExpressionElementSyntax)collection.Elements[0]).Expression})")], true);
            }
        }
        return new([], true, "Unsupported transformation");
    }

    private string Unique(string name)
    {
        var result = name; var suffix = 1;
        while (model.LookupSymbols(request.Offset, name: result).Length > 0 || source.Contains(result, StringComparison.Ordinal)) result = name + suffix++;
        return result;
    }

    private ISymbol[] Inputs(ExpressionSyntax e) => e.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
        .Where(n => n.Parent is not NameColonSyntax and not NameEqualsSyntax)
        .Select(n => model.GetSymbolInfo(n, token).Symbol).OfType<ISymbol>()
        .Where(s => s is ILocalSymbol or IParameterSymbol || s is IFieldSymbol { IsStatic: false } || s is IPropertySymbol { IsStatic: false })
        .Where(s => !s.DeclaringSyntaxReferences.Any(r => e.Span.Contains(r.Span)))
        .Distinct(SymbolEqualityComparer.Default).ToArray();
    private string? ExtractionReason(ExpressionSyntax e)
    {
        var @class = e.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
        if (@class is null || @class.Parent is ClassDeclarationSyntax || @class.ContainsDirectives) return "Extract from a top-level widget class without preprocessor boundaries";
        if (e.DescendantNodesAndSelf().Any(n => n is ThisExpressionSyntax t && t.Parent is not MemberAccessExpressionSyntax || n is BaseExpressionSyntax or AssignmentExpressionSyntax or AwaitExpressionSyntax
            || n is PrefixUnaryExpressionSyntax p && (p.Kind() is SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression)
            || n is PostfixUnaryExpressionSyntax q && (q.Kind() is SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression)))
            return "this/base, mutation and async evaluation cannot be moved safely";
        if (e.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(call => model.GetSymbolInfo(call, token).Symbol is not IMethodSymbol { IsStatic: true } method
            || method.ContainingAssembly.Identity.Name != "Doroti.Framework.Painting" || method.ContainingType.Name is not ("EdgeInsets" or "BorderRadius") || !method.Name.StartsWith("Create", StringComparison.Ordinal)))
            return "Extract a pure construction expression; method evaluation order must remain at the call site";
        if (e.DescendantNodes().OfType<AnonymousFunctionExpressionSyntax>().Any()) return "Pass callbacks as delegate inputs; inline closure capture is not moved";
        if (e.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Select(n => model.GetSymbolInfo(n, token).Symbol)
            .Any(s => s is { IsStatic: true, DeclaredAccessibility: Accessibility.Private })) return "Private static references cannot be moved to another class";
        if (e.DescendantNodesAndSelf().OfType<BaseObjectCreationExpressionSyntax>().Any(creation => model.GetSymbolInfo(creation, token).Symbol is IMethodSymbol ctor
            && !model.Compilation.IsSymbolAccessibleWithin(ctor, model.Compilation.Assembly))) return "A private/protected constructor is inaccessible to the extracted class";
        foreach (var input in Inputs(e))
        {
            if (input is IFieldSymbol or IPropertySymbol && !SymbolEqualityComparer.Default.Equals(input.ContainingType, model.GetDeclaredSymbol(@class, token)))
                return "External member reads need an explicit local value before extraction";
            if (input is IFieldSymbol { IsReadOnly: false } || input is IPropertySymbol p && !p.DeclaringSyntaxReferences.Any(r => r.GetSyntax() is PropertyDeclarationSyntax { ExpressionBody: null, AccessorList.Accessors: [{ Body: null, ExpressionBody: null }] }))
                return "Mutable members and computed properties may change evaluation/capture";
            var type = InputType(input);
            if (type is null || type.TypeKind is TypeKind.Error or TypeKind.Pointer || type.IsRefLikeType
                || input is IParameterSymbol { RefKind: not RefKind.None }) return "Input type/ref lifetime is unavailable or unsupported";
            if (type is ITypeParameterSymbol { TypeParameterKind: TypeParameterKind.Method } || !AccessibleType(type))
                return "Input type or method generic parameters are inaccessible to the extracted class";
        }
        return null;
    }
    private bool AccessibleType(ITypeSymbol type) => type switch
    {
        IArrayTypeSymbol array => AccessibleType(array.ElementType),
        INamedTypeSymbol named => model.Compilation.IsSymbolAccessibleWithin(named, model.Compilation.Assembly) && named.TypeArguments.All(AccessibleType),
        _ => true
    };
    private static ITypeSymbol? InputType(ISymbol input) => input switch { ILocalSymbol s => s.Type, IParameterSymbol s => s.Type, IFieldSymbol s => s.Type, IPropertySymbol s => s.Type, _ => null };

    private Transform Extract(ExpressionSyntax e, string name)
    {
        if (!SyntaxFacts.IsValidIdentifier(name) || SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None) return new([], true, "Use a non-keyword C# identifier");
        var @class = e.Ancestors().OfType<ClassDeclarationSyntax>().First();
        var type = model.GetDeclaredSymbol(@class, token)!;
        if (type.ContainingNamespace.GetTypeMembers(name).Length != 0) return new([], true, "Type name already exists in this namespace");
        var inputs = Inputs(e);
        var generics = @class.TypeParameterList?.ToString() ?? "";
        var constraints = string.Join(" ", @class.ConstraintClauses.Select(c => c.ToString()));
        var parameters = string.Join(", ", inputs.Select((s, i) => $"{InputType(s)!.ToDisplayString(Engine.DisplayFormat)} input{i}"));
        var fields = string.Join(_nl, inputs.Select((s, i) => $"    private readonly {InputType(s)!.ToDisplayString(Engine.DisplayFormat)} _input{i};"));
        var assignments = string.Join(_nl, inputs.Select((_, i) => $"        _input{i} = input{i};"));
        var replacements = new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default);
        for (var i = 0; i < inputs.Length; i++) replacements[inputs[i]] = "_input" + i;
        var rewritten = new InputRewriter(model, replacements).Visit(e)!.ToString();
        var ctorParams = parameters.Length == 0 ? "" : parameters + ", ";
        var declaration = $"{_nl}{_nl}internal sealed class {name}{generics} : {Widgets}StatelessWidget {constraints}{_nl}{{{_nl}{fields}{_nl}    public {name}({ctorParams}global::Doroti.Framework.Foundation.Key? key = null) : base(key: key){_nl}    {{{_nl}{assignments}{_nl}    }}{_nl}    public override {Widgets}Widget build({Widgets}BuildContext context) => {rewritten};{_nl}}}";
        var replacement = $"new {name}{generics}({string.Join(", ", inputs.Select(s => Escape(s.Name)))})";
        return new([new(e.SpanStart, e.Span.Length, replacement), new(@class.FullSpan.End, 0, declaration)], true);
    }

    private string? ConversionReason(ClassDeclarationSyntax c)
    {
        var symbol = model.GetDeclaredSymbol(c, token)!;
        if (c.ContainsDirectives || c.Modifiers.Any(SyntaxKind.PartialKeyword) || c.Parent is ClassDeclarationSyntax || symbol.BaseType?.ToDisplayString() != "Doroti.Framework.Widgets.StatelessWidget"
            || c.BaseList?.Types.Count != 1) return "Only a non-partial top-level class directly deriving StatelessWidget is supported";
        var build = c.Members.OfType<MethodDeclarationSyntax>().Where(m => m.Identifier.ValueText == "build" && m.Modifiers.Any(SyntaxKind.OverrideKeyword)).ToArray();
        if (build.Length != 1 || build[0].ContainsDiagnostics || build[0].ContainsDirectives
            || model.GetDiagnostics(build[0].Span, token).Any(d => d.Severity == DiagnosticSeverity.Error)) return "One complete, resolved build override is required";
        if (build[0].DescendantNodes().Any(n => n is BaseExpressionSyntax || n is AssignmentExpressionSyntax))
            return "base dispatch or build-time mutation requires an explicit ownership design";
        foreach (var n in build[0].DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            var s = model.GetSymbolInfo(n, token).Symbol;
            if (s is IParameterSymbol p && p.DeclaringSyntaxReferences.Any(r => r.GetSyntax().Parent?.Parent == c)
                && c.DescendantNodes().Any(node => node is AssignmentExpressionSyntax assignment && SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(assignment.Left, token).Symbol, p)
                    || node is PrefixUnaryExpressionSyntax pre && (pre.Kind() is SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression) && SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(pre.Operand, token).Symbol, p)
                    || node is PostfixUnaryExpressionSyntax post && (post.Kind() is SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression) && SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(post.Operand, token).Symbol, p)))
                return "Mutable primary-constructor capture must be made explicit before conversion";
            if (s is IMethodSymbol method && method.Name == "build" && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbol))
                return "Recursive build calls must be resolved before conversion";
        }
        if (symbol.GetMembers(c.Identifier.ValueText + "State").Length != 0) return "Nested State class name already exists";
        return null;
    }

    private Transform Convert(ClassDeclarationSyntax c)
    {
        var symbol = model.GetDeclaredSymbol(c, token)!;
        var build = c.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.ValueText == "build" && m.Modifiers.Any(SyntaxKind.OverrideKeyword));
        var snapshotName = Unique("currentWidget");
        var replacements = new Dictionary<ISymbol, string>(SymbolEqualityComparer.Default);
        var captured = new Dictionary<IParameterSymbol, string>(SymbolEqualityComparer.Default);
        var fields = new List<MemberDeclarationSyntax>();
        foreach (var node in build.DescendantNodes().OfType<IdentifierNameSyntax>())
        {
            var member = model.GetSymbolInfo(node, token).Symbol;
            if (member is null || member.DeclaringSyntaxReferences.Any(r => build.Span.Contains(r.Span))) continue;
            if (member is IParameterSymbol p && p.DeclaringSyntaxReferences.Any(r => r.GetSyntax().Parent?.Parent == c))
            {
                if (!captured.TryGetValue(p, out var field))
                {
                    field = Unique("__doroti_" + p.Name); captured.Add(p, field);
                    fields.Add(SyntaxFactory.ParseMemberDeclaration($"private readonly {p.Type.ToDisplayString(Engine.DisplayFormat)} {field} = {Escape(p.Name)};")!);
                }
                replacements[p] = snapshotName + "." + field; continue;
            }
            var belongs = false;
            for (var owner = symbol; owner is not null; owner = owner.BaseType)
                if (SymbolEqualityComparer.Default.Equals(owner, member.ContainingType)) { belongs = true; break; }
            if (belongs && member is IFieldSymbol or IPropertySymbol or IMethodSymbol)
                replacements[member] = (member.IsStatic ? member.ContainingType.ToDisplayString(Engine.DisplayFormat) : snapshotName) + "." + Escape(member.Name);
        }
        var rewritten = (MethodDeclarationSyntax)new InputRewriter(model, replacements, snapshotName).Visit(build)!;
        var capture = SyntaxFactory.ParseStatement($"var {snapshotName} = this.widget;");
        var body = rewritten.Body ?? SyntaxFactory.Block(SyntaxFactory.ReturnStatement(rewritten.ExpressionBody!.Expression)
            .WithReturnKeyword(SyntaxFactory.Token(SyntaxKind.ReturnKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        rewritten = rewritten.WithExpressionBody(null).WithSemicolonToken(default).WithBody(body.WithStatements(body.Statements.Insert(0, capture)));
        var name = c.Identifier.Text;
        var generics = c.TypeParameterList?.ToString() ?? "";
        var stateName = c.Identifier.ValueText + "State";
        var create = SyntaxFactory.ParseMemberDeclaration($"public override {Widgets}IState createState() => new {stateName}();")!;
        var newBase = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(Widgets + "StatefulWidget")).WithTriviaFrom(c.BaseList!.Types[0]);
        var state = SyntaxFactory.ParseMemberDeclaration($"private sealed class {stateName} : {Widgets}State<{name}{generics}> {_nl}{{{_nl}{rewritten.ToFullString().Trim()}{_nl}}}")!;
        var replacement = c.WithBaseList(c.BaseList.WithTypes(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(newBase)))
            .WithMembers(c.Members.Replace(build, create).AddRange(fields).Add(state)).ToString();
        return new([new(c.SpanStart, c.Span.Length, replacement)], true);
    }

    private static string Escape(string name) => SyntaxFacts.GetKeywordKind(name) == SyntaxKind.None ? name : "@" + name;

    private sealed class InputRewriter(SemanticModel model, Dictionary<ISymbol, string> replacements, string? thisReplacement = null) : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitThisExpression(ThisExpressionSyntax node) => thisReplacement is null ? node : SyntaxFactory.IdentifierName(thisReplacement).WithTriviaFrom(node);
        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (node.Expression is ThisExpressionSyntax && replacements.TryGetValue(model.GetSymbolInfo(node).Symbol!, out var replacement))
                return SyntaxFactory.ParseExpression(replacement).WithTriviaFrom(node);
            return base.VisitMemberAccessExpression(node);
        }
        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node.Parent is NameColonSyntax or NameEqualsSyntax) return node;
            var symbol = model.GetSymbolInfo(node).Symbol;
            if (symbol is not null && replacements.TryGetValue(symbol, out var replacement)
                && (node.Parent is not MemberAccessExpressionSyntax member || member.Expression == node))
                return SyntaxFactory.ParseExpression(replacement).WithTriviaFrom(node);
            return base.VisitIdentifierName(node);
        }
    }
}
