using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;
using System.Xml.Linq;

// A maintenance tool, never an application runtime dependency. Only rewrite
// explicitly selected operations after resolving their receiver in source.
var product = Path.GetFullPath(args[0]);
var project = Path.GetFullPath(args[1]);
var apply = args.Contains("--apply");
var references = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
foreach (var file in ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator))
    references.TryAdd(Path.GetFileName(file), file);
var visited = new HashSet<string>();
void AddProject(string file)
{
    if (!visited.Add(file)) return;
    var document = XDocument.Load(file);
    foreach (var item in document.Descendants("ProjectReference"))
    {
        var include = (string?)item.Attribute("Include");
        if (include is null) continue;
        var dependency = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, include.Replace('\\', Path.DirectorySeparatorChar)));
        if (!File.Exists(dependency)) continue;
        AddProject(dependency);
        var assembly = Path.Combine(Path.GetDirectoryName(dependency)!, "bin", "Release", "net10.0", Path.GetFileNameWithoutExtension(dependency) + ".dll");
        if (File.Exists(assembly)) references.TryAdd(Path.GetFileName(assembly), assembly);
    }
}
AddProject(Path.Combine(project, Path.GetFileName(project) + ".csproj"));
foreach (var file in Directory.GetFiles(Path.Combine(product, "..", "DorotiTestbedApp", "bin", "Release", "net10.0"), "*.dll").Where(f => !Path.GetFileName(f).StartsWith("Doroti")))
    references.TryAdd(Path.GetFileName(file), file);
references.Remove(Path.GetFileName(project) + ".dll");
var csproj = XDocument.Load(Path.Combine(project, Path.GetFileName(project) + ".csproj"));
var excluded = csproj.Descendants("Compile").Select(x => (string?)x.Attribute("Remove")).OfType<string>().ToHashSet();
var files = Directory.GetFiles(project, "*.cs").Where(f => !excluded.Contains(Path.GetFileName(f))).ToList();
var implicitUsings = Path.Combine(project, "obj", "Debug", "net10.0", Path.GetFileName(project) + ".GlobalUsings.g.cs");
if (File.Exists(implicitUsings)) files.Add(implicitUsings);
var parse = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: ["DEBUG"]);
var trees = files.Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f), parse, f)).ToArray();
var compilation = CSharpCompilation.Create(Path.GetFileName(project), trees,
    references.Values.Select(f => MetadataReference.CreateFromFile(f)),
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
if (errors.Length != 0)
{
    Console.Error.WriteLine(string.Join("\n", errors.Take(15).Select(e => e.ToString())));
    return 2;
}
if (args.Contains("--audit"))
{
    var dynamicOperations = new List<object>();
    foreach (var tree in trees)
    {
        var model = compilation.GetSemanticModel(tree);
        foreach (var node in tree.GetRoot().DescendantNodes().OfType<ExpressionSyntax>())
        {
            var operation = model.GetOperation(node);
            if (operation is null || operation.Syntax != node) continue;
            if (operation.Kind is OperationKind.DynamicInvocation or OperationKind.DynamicMemberReference or OperationKind.DynamicIndexerAccess
                || operation is Microsoft.CodeAnalysis.Operations.IConversionOperation conversion && conversion.Operand.Type?.TypeKind == TypeKind.Dynamic)
                dynamicOperations.Add(new { file = tree.FilePath, line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    kind = operation.Kind.ToString(), source = node.ToString() });
        }
    }
    Console.WriteLine(JsonSerializer.Serialize(dynamicOperations, new JsonSerializerOptions { WriteIndented = true }));
    return 0;
}
var geometry = new HashSet<string> { "Doroti.Ui.Rect", "Doroti.Ui.Offset", "Doroti.Ui.Size" };
var operations = new HashSet<string>
{
    "localToGlobal", "globalToLocal", "getTransformTo", "sendSemanticsEvent",
    "markNeedsLayout", "markNeedsSemanticsUpdate", "getPositionForPoint",
    "getEndpointsForSelection", "getRectForComposingRange", "parentData", "paintBounds",
    "child", "constraints", "toStringDeep", "toDiagnosticsNode", "toStringShort",
    "attach", "detach", "owner", "textDirection", "value", "contentLength", "addListener", "removeListener",
    "visitChildren", "applyPaintTransform", "describeApproximatePaintClip", "debugPaint", "markNeedsPaint",
    "file", "line", "column", "toJsonMap", "debugCreator", "attached", "debugDescribeChildren"
};
var changes = new List<object>();
var replacements = new Dictionary<SyntaxTree, SyntaxNode>();
foreach (var tree in trees)
{
    var model = compilation.GetSemanticModel(tree);
    var root = tree.GetRoot();
    var selected = new HashSet<CastExpressionSyntax>();
    foreach (var cast in root.DescendantNodes().OfType<CastExpressionSyntax>())
    {
        if (cast.Type.ToString() != "dynamic") continue;
        SyntaxNode receiver = cast;
        while (receiver.Parent is ParenthesizedExpressionSyntax parent) receiver = parent;
        var access = receiver.Parent as MemberAccessExpressionSyntax;
        var conditional = receiver.Parent as ConditionalAccessExpressionSyntax;
        var name = access?.Name.Identifier.ValueText
            ?? (conditional?.WhenNotNull as MemberBindingExpressionSyntax)?.Name.Identifier.ValueText
            ?? ((conditional?.WhenNotNull as InvocationExpressionSyntax)?.Expression as MemberBindingExpressionSyntax)?.Name.Identifier.ValueText;
        if (name is null) continue;
        if (model.GetTypeInfo(cast.Expression).Type is not INamedTypeSymbol type || type.TypeKind == TypeKind.Error || type.SpecialType == SpecialType.System_Object) continue;
        if (!geometry.Contains(type.ToDisplayString()) && !operations.Contains(name)) continue;
        var members = new List<ISymbol>();
        for (var current = type; current is not null; current = current.BaseType)
            members.AddRange(current.GetMembers(name).Where(m => !m.IsStatic));
        if (members.Count == 0) continue;
        // Refuse overloaded methods; inherited overrides of one signature are OK.
        var methods = members.OfType<IMethodSymbol>().Select(m => string.Join(",", m.Parameters.Select(p => p.Type.ToDisplayString()))).Distinct().ToArray();
        if (methods.Length > 1) continue;
        ExpressionSyntax? expression = conditional ?? (ExpressionSyntax?)access;
        if (expression?.Parent is InvocationExpressionSyntax invocation && invocation.Expression == expression) expression = invocation;
        if (expression is null) continue;
        var speculative = expression.ReplaceNode(cast, cast.Expression.WithTriviaFrom(cast));
        var resultType = model.GetSpeculativeTypeInfo(expression.SpanStart, speculative, SpeculativeBindingOption.BindAsExpression).Type;
        if (resultType is null || resultType.TypeKind is TypeKind.Error or TypeKind.Dynamic) continue;
        selected.Add(cast);
        changes.Add(new { file = tree.FilePath, line = cast.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
            receiver = type.ToDisplayString(), member = name, before = cast.ToString(), after = cast.Expression.ToString() });
    }
    if (selected.Count > 0)
    {
        var rewritten = root.ReplaceNodes(selected, (original, rewritten) => ((CastExpressionSyntax)rewritten).Expression.WithTriviaFrom(rewritten));
        replacements.Add(tree, rewritten);
    }
}
var checkedCompilation = compilation;
foreach (var (tree, root) in replacements)
    checkedCompilation = checkedCompilation.ReplaceSyntaxTree(tree, CSharpSyntaxTree.Create((CSharpSyntaxNode)root, parse, tree.FilePath));
var introduced = checkedCompilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
if (introduced.Length != 0)
{
    Console.Error.WriteLine(string.Join("\n", introduced.Take(15).Select(e => e.ToString())));
    return 3;
}
if (apply)
    foreach (var (tree, root) in replacements) File.WriteAllText(tree.FilePath, root.ToFullString());
Console.WriteLine(JsonSerializer.Serialize(new { applied = apply, count = changes.Count, changes }, new JsonSerializerOptions { WriteIndented = true }));
return 0;
