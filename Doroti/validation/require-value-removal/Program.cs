using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

const string RuntimeType = "Doroti.Runtime.DartRuntimePrimitives";

if (args.Length < 3 || args[0] is not ("scan" or "apply" or "verify"))
{
    Console.Error.WriteLine(
        "Usage: RequireValueRemoval <scan|apply|verify> <solution> <manifest.json>"
    );
    return 2;
}

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
var solution = await workspace.OpenSolutionAsync(Path.GetFullPath(args[1]));
if (workspace.Diagnostics.Any(d => d.Kind == WorkspaceDiagnosticKind.Failure))
    throw new InvalidOperationException("Workspace contains load failures.");

var rows = new List<ManifestRow>();
var updated = solution;
var includedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var projectId in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects())
{
    var project = updated.GetProject(projectId)!;
    if (!project.Name.StartsWith("Doroti.Framework.", StringComparison.Ordinal))
        continue;

    foreach (var documentId in project.DocumentIds)
    {
        var document = updated.GetDocument(documentId)!;
        if (
            document.FilePath is null
            || !document.FilePath.EndsWith(".cs", StringComparison.Ordinal)
        )
            continue;
        includedFiles.Add(Path.GetFullPath(document.FilePath));
        var root = await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
        if (!root.ToFullString().Contains("RequireValue", StringComparison.Ordinal))
            continue;
        var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
        var replacements = new Dictionary<(int Start, int Length), Replacement>();
        foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var method = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (
                method?.Name != "RequireValue"
                || method.ContainingType.ToDisplayString() != RuntimeType
            )
                continue;
            if (invocation.ArgumentList.Arguments.Count != 1)
                throw new InvalidOperationException(
                    $"Unexpected RequireValue arity at {document.FilePath}:{Line(invocation)}"
                );

            var parameter = method.Parameters[0].Type;
            var classification = Classify(method, parameter);
            var argument = invocation.ArgumentList.Arguments[0].Expression;
            replacements.Add(
                (invocation.SpanStart, invocation.Span.Length),
                new Replacement(classification)
            );
            rows.Add(
                new ManifestRow(
                    project.Name,
                    Path.GetRelativePath(Environment.CurrentDirectory, document.FilePath),
                    Line(invocation),
                    classification,
                    model.GetTypeInfo(argument).Type?.ToDisplayString() ?? "<unknown>",
                    method.ReturnType.ToDisplayString(),
                    MayHaveSideEffects(argument)
                )
            );
        }

        if (args[0] == "apply" && replacements.Count != 0)
        {
            var rewritten = new Rewriter(replacements).Visit(root);
            updated = updated.WithDocumentSyntaxRoot(documentId, rewritten!);
        }
    }
}

// Platform-specific generated framework files are intentionally removed from the
// ordinary framework projects. Bind each one against its owning project compilation
// so the removal covers all product source without treating text as a call symbol.
var sourceRoot = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1]))!, "src");
foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories))
{
    var fullPath = Path.GetFullPath(file);
    if (includedFiles.Contains(fullPath))
        continue;
    var text = await File.ReadAllTextAsync(fullPath);
    if (!text.Contains("DartRuntimePrimitives.RequireValue", StringComparison.Ordinal))
        continue;
    var projectName = new DirectoryInfo(Path.GetDirectoryName(fullPath)!).Name;
    var owner = solution.Projects.SingleOrDefault(project => project.Name == projectName);
    if (owner is null)
        throw new InvalidOperationException($"No owning project for {fullPath}.");
    var ownerCompilation =
        await owner.GetCompilationAsync() ?? throw new InvalidOperationException();
    var tree = CSharpSyntaxTree.ParseText(text, (CSharpParseOptions?)owner.ParseOptions, fullPath);
    var looseCompilation = CSharpCompilation.Create(
        $"{projectName}.RequireValueRemoval",
        [tree],
        ownerCompilation.References.Append(ownerCompilation.ToMetadataReference()),
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    );
    var root = await tree.GetRootAsync();
    var model = looseCompilation.GetSemanticModel(tree);
    var replacements = new Dictionary<(int Start, int Length), Replacement>();
    foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
    {
        var method = model.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        ExpressionSyntax? argument =
            invocation.ArgumentList.Arguments.Count == 1
                ? invocation.ArgumentList.Arguments[0].Expression
                : null;
        if (
            method is null
            && argument is MemberAccessExpressionSyntax { Name.Identifier.Text: "engineId" }
            && IsRequireValueSyntax(invocation.Expression)
        )
        {
            const string staleClassification = "excluded-stale-engine-id";
            replacements.Add(
                (invocation.SpanStart, invocation.Span.Length),
                new Replacement(staleClassification)
            );
            rows.Add(
                new ManifestRow(
                    projectName + " (platform source)",
                    Path.GetRelativePath(Environment.CurrentDirectory, fullPath),
                    Line(invocation),
                    staleClassification,
                    "<unbound removed platform API>",
                    "long",
                    MayHaveSideEffects(argument)
                )
            );
            continue;
        }
        if (
            method?.Name != "RequireValue"
            || method.ContainingType.ToDisplayString() != RuntimeType
        )
            continue;
        if (invocation.ArgumentList.Arguments.Count != 1)
            throw new InvalidOperationException(
                $"Unexpected RequireValue arity at {fullPath}:{Line(invocation)}"
            );
        argument = invocation.ArgumentList.Arguments[0].Expression;
        var classification = Classify(method, method.Parameters[0].Type);
        replacements.Add(
            (invocation.SpanStart, invocation.Span.Length),
            new Replacement(classification)
        );
        rows.Add(
            new ManifestRow(
                projectName + " (platform source)",
                Path.GetRelativePath(Environment.CurrentDirectory, fullPath),
                Line(invocation),
                classification,
                model.GetTypeInfo(argument).Type?.ToDisplayString() ?? "<unknown>",
                method.ReturnType.ToDisplayString(),
                MayHaveSideEffects(argument)
            )
        );
    }
    if (replacements.Count == 0)
        throw new InvalidOperationException($"Unbound RequireValue text remains in {fullPath}.");
    if (args[0] == "apply")
    {
        var rewritten = new Rewriter(replacements).Visit(root)!;
        await File.WriteAllTextAsync(fullPath, rewritten.ToFullString());
    }
}

rows.Sort(
    (left, right) =>
    {
        var file = StringComparer.Ordinal.Compare(left.File, right.File);
        return file != 0 ? file : left.Line.CompareTo(right.Line);
    }
);
var manifestPath = Path.GetFullPath(args[2]);
Directory.CreateDirectory(Path.GetDirectoryName(manifestPath)!);
await File.WriteAllTextAsync(
    manifestPath,
    JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true }) + "\n"
);

if (args[0] == "verify")
{
    if (rows.Count != 0)
        throw new InvalidOperationException($"Found {rows.Count} executable RequireValue calls.");
}
else if (args[0] == "apply" && !workspace.TryApplyChanges(updated))
{
    throw new InvalidOperationException("Could not apply RequireValue replacements.");
}

foreach (var group in rows.GroupBy(row => row.Classification).OrderBy(group => group.Key))
    Console.WriteLine($"{group.Key}: {group.Count()}");
Console.WriteLine($"{args[0]} complete: {rows.Count} calls; manifest {manifestPath}");
return 0;

static int Line(SyntaxNode node) =>
    node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition.Line + 1;

static bool IsRequireValueSyntax(ExpressionSyntax expression) =>
    expression
        is MemberAccessExpressionSyntax
        {
            Expression: IdentifierNameSyntax { Identifier.Text: "DartRuntimePrimitives" },
            Name.Identifier.Text: "RequireValue",
        };

static string Classify(IMethodSymbol method, ITypeSymbol parameter) =>
    method.Parameters.Length == 2 ? "required-reference"
    : parameter
        is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T }
        ? "required-nullable-value"
    : "already-nonnullable-value";

static bool MayHaveSideEffects(ExpressionSyntax expression) =>
    expression
        .DescendantNodesAndSelf()
        .Any(node =>
            node
                is InvocationExpressionSyntax
                    or AssignmentExpressionSyntax
                    or AwaitExpressionSyntax
                    or ObjectCreationExpressionSyntax
                    or PostfixUnaryExpressionSyntax
                    or PrefixUnaryExpressionSyntax
                    and not PrefixUnaryExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.LogicalNotExpression
                            or (int)SyntaxKind.UnaryMinusExpression
                            or (int)SyntaxKind.UnaryPlusExpression
                            or (int)SyntaxKind.BitwiseNotExpression
                    }
        );

sealed record Replacement(string Classification);

sealed record ManifestRow(
    string Project,
    string File,
    int Line,
    string Classification,
    string ArgumentType,
    string ResultType,
    bool MayHaveSideEffects
);

sealed class Rewriter(IReadOnlyDictionary<(int Start, int Length), Replacement> replacements)
    : CSharpSyntaxRewriter
{
    private const string NullMessage = "A required value was null.";

    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (!replacements.TryGetValue((node.SpanStart, node.Span.Length), out var replacement))
            return base.VisitInvocationExpression(node);

        var argument = (
            (ExpressionSyntax)Visit(node.ArgumentList.Arguments[0].Expression)!
        ).WithoutTrivia();
        ExpressionSyntax result =
            replacement.Classification == "already-nonnullable-value"
                ? SyntaxFactory.ParenthesizedExpression(argument)
                : SyntaxFactory.ParseExpression(
                    $"({argument} ?? throw new global::System.NullReferenceException(\"{NullMessage}\"))"
                );
        return result.WithTriviaFrom(node);
    }
}
