using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

if (args.Length != 3 || args[0] is not ("scan" or "apply" or "verify"))
    throw new ArgumentException("Usage: ModuloAudit <scan|apply|verify> <repo> <report.json>");
var mode = args[0];
var repo = Path.GetFullPath(args[1]);

// Modulo and its parent can share SpanStart; never rewrite a neighboring addition.
var probe = SyntaxFactory.ParseExpression("a % b + c * (a % (b % c))");
var probeSites = probe
    .DescendantNodesAndSelf()
    .OfType<BinaryExpressionSyntax>()
    .Where(node => node.IsKind(SyntaxKind.ModuloExpression))
    .Select(node => node.SpanStart)
    .ToHashSet();
var rewrittenProbe = new ModuloRewriter(probeSites).Visit(probe)!;
if (
    rewrittenProbe.DescendantNodesAndSelf().Count(node => node.IsKind(SyntaxKind.AddExpression))
        != 1
    || rewrittenProbe
        .DescendantNodesAndSelf()
        .Count(node => node.IsKind(SyntaxKind.MultiplyExpression)) != 1
    || rewrittenProbe.DescendantNodesAndSelf().Any(node => node.IsKind(SyntaxKind.ModuloExpression))
)
    throw new InvalidOperationException("Modulo rewriter changed a neighboring operator.");
AssemblyLoadContext.Default.Resolving += (_, name) =>
{
    var path = Path.Combine(AppContext.BaseDirectory, name.Name + ".dll");
    return File.Exists(path) ? Assembly.LoadFrom(path) : null;
};
var sdk = Assembly
    .GetExecutingAssembly()
    .GetCustomAttributes<AssemblyMetadataAttribute>()
    .Single(attribute => attribute.Key == "SdkPath")
    .Value!;
MSBuildLocator.RegisterMSBuildPath(Path.GetFullPath(sdk));
using var workspace = MSBuildWorkspace.Create();
workspace.RegisterWorkspaceFailedHandler(e => Console.Error.WriteLine(e.Diagnostic));

// These roots include all framework libraries and Ui, without loading platform workloads.
foreach (var name in new[] { "Material", "Cupertino" })
    if (
        !workspace.CurrentSolution.Projects.Any(project =>
            project.Name == $"Doroti.Framework.{name}"
        )
    )
        await workspace.OpenProjectAsync(
            Path.Combine(repo, $"Doroti/src/Doroti.Framework.{name}/Doroti.Framework.{name}.csproj")
        );
if (workspace.Diagnostics.Any(d => d.Kind == WorkspaceDiagnosticKind.Failure))
    throw new InvalidOperationException("Workspace load failed.");

var rows = new List<object>();
var eligible = 0;
var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var project in workspace.CurrentSolution.Projects)
{
    foreach (var document in project.Documents)
    {
        if (document.FilePath is not { } path || !visited.Add(Path.GetFullPath(path)))
            continue;
        var root = await document.GetSyntaxRootAsync();
        if (root is null)
            continue;
        if (
            project.Name.StartsWith("Doroti.Framework.", StringComparison.Ordinal)
            && root.DescendantNodes()
                .Any(node => node.IsKind(SyntaxKind.ModuloAssignmentExpression))
        )
            throw new InvalidOperationException(
                $"Dart compound modulo requires single-evaluation lowering: {path}"
            );
        var operations = root.DescendantNodes()
            .OfType<BinaryExpressionSyntax>()
            .Where(node => node.IsKind(SyntaxKind.ModuloExpression))
            .ToArray();
        if (operations.Length == 0)
            continue;
        var model = (await document.GetSemanticModelAsync())!;
        var migrate = new HashSet<int>();
        foreach (var operation in operations)
        {
            var symbol = model.GetSymbolInfo(operation).Symbol as IMethodSymbol;
            var type = model.GetTypeInfo(operation).Type;
            var framework = project.Name.StartsWith("Doroti.Framework.", StringComparison.Ordinal);
            var radius =
                project.Name == "Doroti.Ui"
                && operation.Ancestors().OfType<OperatorDeclarationSyntax>().Any();
            var userOperator = symbol?.MethodKind == MethodKind.UserDefinedOperator;
            var numeric =
                type?.SpecialType
                is SpecialType.System_Int32
                    or SpecialType.System_Int64
                    or SpecialType.System_Double;
            var category =
                userOperator ? "user-defined-operator"
                : !(framework || radius) ? "native-csharp"
                : numeric ? "dart-numeric"
                : "unresolved";
            if (category == "unresolved")
                throw new InvalidOperationException($"Unresolved modulo: {path}: {operation}");
            rows.Add(
                new
                {
                    file = Path.GetRelativePath(repo, path).Replace('\\', '/'),
                    line = operation.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                    category,
                    type = type?.ToDisplayString(),
                    expression = operation.ToString(),
                }
            );
            if (category == "dart-numeric")
            {
                eligible++;
                migrate.Add(operation.SpanStart);
            }
        }
        if (mode == "apply" && migrate.Count > 0)
            File.WriteAllText(path, new ModuloRewriter(migrate).Visit(root)!.ToFullString());
    }
}

// Inventory host/rendering sources too; their native C# remainder is intentionally retained.
foreach (
    var path in Directory.EnumerateFiles(
        Path.Combine(repo, "Doroti/src"),
        "*.cs",
        SearchOption.AllDirectories
    )
)
{
    if (
        visited.Contains(Path.GetFullPath(path))
        || path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Any(part => part is "bin" or "obj")
    )
        continue;
    var root = CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetRoot();
    foreach (
        var node in root.DescendantNodes()
            .Where(node =>
                node.IsKind(SyntaxKind.ModuloExpression)
                || node.IsKind(SyntaxKind.ModuloAssignmentExpression)
            )
    )
    {
        if (path.Contains("Doroti.Framework.", StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"Dart framework source was not loaded for semantic analysis: {path}"
            );
        rows.Add(
            new
            {
                file = Path.GetRelativePath(repo, path).Replace('\\', '/'),
                line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1,
                category = "native-csharp",
                expression = node.ToString(),
            }
        );
    }
}
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[2]))!);
File.WriteAllText(
    args[2],
    JsonSerializer.Serialize(
        new
        {
            mode,
            dartNumeric = eligible,
            operations = rows,
        },
        new JsonSerializerOptions { WriteIndented = true }
    ) + "\n"
);
Console.WriteLine(
    $"Modulo audit: {rows.Count} operators; {eligible} Dart numeric operators {(mode == "apply" ? "migrated" : "remaining")}."
);
return mode == "verify" && eligible != 0 ? 1 : 0;

sealed class ModuloRewriter(HashSet<int> migrate) : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        var updated = (BinaryExpressionSyntax)base.VisitBinaryExpression(node)!;
        if (!node.IsKind(SyntaxKind.ModuloExpression) || !migrate.Contains(node.SpanStart))
            return updated;
        return SyntaxFactory
            .InvocationExpression(
                SyntaxFactory.ParseExpression("global::Doroti.Runtime.DartNumeric.Modulo"),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(
                        new[]
                        {
                            SyntaxFactory.Argument(updated.Left.WithoutTrivia()),
                            SyntaxFactory.Argument(updated.Right.WithoutTrivia()),
                        }
                    )
                )
            )
            .WithTriviaFrom(node);
    }
}
