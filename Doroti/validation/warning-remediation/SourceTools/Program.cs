using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

AssemblyLoadContext.Default.Resolving += (_, name) =>
{
    var path = Path.Combine(AppContext.BaseDirectory, name.Name + ".dll");
    return File.Exists(path) ? Assembly.LoadFrom(path) : null;
};
if (args[0] == "index")
{
    var run = Path.GetFullPath(args[1]);
    var cache = new Dictionary<string, SyntaxNode>();
    foreach (var name in new[] { "diagnostics-debug.json", "diagnostics-release.json", "warning-ledger.json" })
    {
        var path = Path.Combine(run, name);
        var rows = JsonNode.Parse(File.ReadAllText(path))!.AsArray();
        foreach (var row in rows.Select(r => r!.AsObject()))
        {
            var file = row["file"]!.GetValue<string>();
            if (!cache.TryGetValue(file, out var root))
            {
                root = CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(run, "isolated", file))).GetRoot();
                cache.Add(file, root);
            }
            var line = row["line"]!.GetValue<int>() - 1;
            var column = row["column"]!.GetValue<int>() - 1;
            var position = root.SyntaxTree.GetText().Lines[line].Start + column;
            var ancestors = root.FindToken(position).Parent!.AncestorsAndSelf().Reverse().ToArray();
            var parts = ancestors.Select(n => n switch
            {
                BaseNamespaceDeclarationSyntax ns => ns.Name.ToString(),
                TypeDeclarationSyntax type => type.Identifier.Text + type.TypeParameterList?.ToString(),
                MethodDeclarationSyntax method => method.Identifier.Text + method.TypeParameterList?.ToString(),
                ConstructorDeclarationSyntax ctor => ctor.Identifier.Text,
                PropertyDeclarationSyntax property => property.Identifier.Text,
                IndexerDeclarationSyntax => "this[]",
                LocalFunctionStatementSyntax local => local.Identifier.Text,
                VariableDeclaratorSyntax variable => variable.Identifier.Text,
                _ => null,
            }).Where(p => p is not null);
            row["symbol"] = string.Join(".", parts);
            var code = row["code"]!.GetValue<string>();
            row["category"] = code switch
            {
                "CS0693" or "CS8981" or "CS8609" or "CS8613" or "CS8765" or "CS8767" or "CS8714" => "declaration-contract",
                "CS4014" => "async-ownership",
                "CS0659" => "equality-contract",
                "CS8321" => "unused-or-disconnected-code",
                _ => "nullable-flow-needs-contract-review",
            };
        }
        File.WriteAllText(path, rows.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
    }
    return;
}

var sdk = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>().Single(a => a.Key == "SdkPath").Value!;
MSBuildLocator.RegisterMSBuildPath(Path.GetFullPath(sdk));
var workspaceProperties = args.Where(a => a.StartsWith("--property=", StringComparison.Ordinal))
    .Select(a => a[11..].Split('=', 2))
    .ToDictionary(p => p[0], p => p.Length == 2 ? p[1] : throw new ArgumentException("Expected --property=Name=Value."));
using var workspace = MSBuildWorkspace.Create(workspaceProperties);
workspace.RegisterWorkspaceFailedHandler(e => Console.Error.WriteLine(e.Diagnostic));
if (args[1].EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
    await workspace.OpenProjectAsync(args[1]);
else
    await workspace.OpenSolutionAsync(args[1]);
if (workspace.Diagnostics.Any(d => d.Kind == WorkspaceDiagnosticKind.Failure))
    throw new InvalidOperationException("Workspace contains load failures.");

var projectIds = workspace.CurrentSolution.GetProjectDependencyGraph().GetTopologicallySortedProjects().ToArray();
if (args[0] == "simplify-icon-data-names")
{
    var report = new List<object>();
    foreach (var projectId in projectIds)
    {
        var project = workspace.CurrentSolution.GetProject(projectId)!;
        if (project.Name != "Doroti.Framework.Material") continue;
        foreach (var documentId in project.DocumentIds)
        {
            var document = workspace.CurrentSolution.GetDocument(documentId)!;
            if (IsGenerated(document)) continue;
            var source = await document.GetTextAsync();
            var headerText = source.ToString(new TextSpan(0, Math.Min(500, source.Length)));
            if (!headerText.Contains("/animated_icons/data/", StringComparison.Ordinal) &&
                !headerText.Contains("/material/icons.dart", StringComparison.Ordinal)) continue;
            var root = (CompilationUnitSyntax)(await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException());
            var names = root.DescendantNodes().OfType<QualifiedNameSyntax>()
                .Where(n => n.ToString() is "global::Doroti.Ui.Offset" or "global::Doroti.Ui.Size" or "global::Doroti.Framework.Widgets.IconData").ToArray();
            if (names.Length == 0) continue;
            Console.WriteLine($"Checking icon data type bindings: {document.Name}: {names.Length}");
            var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
            // These globally qualified, non-generic type names have a unique
            // metadata identity. Lookup avoids binding the entire initializer per name.
            var expected = names.Select(n => model.Compilation.GetTypeByMetadataName(n.ToString()[8..])
                ?? throw new InvalidOperationException("Icon data name is not a resolved type.")).ToArray();
            var indices = names.Select((node, index) => (node, index)).ToDictionary(p => p.node, p => p.index);
            const string annotationKind = "DorotiIconDataType";
            var updated = root.ReplaceNodes(names, (original, _) => SyntaxFactory.IdentifierName(original.Right.Identifier)
                .WithTriviaFrom(original).WithAdditionalAnnotations(new SyntaxAnnotation(annotationKind, indices[original].ToString())));
            // Each large field initializer is bound once after the batch edit, rather
            // than speculatively rebinding it for every repeated coordinate type.
            if (names.Any(n => n.ToString().StartsWith("global::Doroti.Ui.", StringComparison.Ordinal)) &&
                !updated.Usings.Any(u => u.Alias is null && u.Name?.ToString() == "Doroti.Ui"))
            {
                var header = updated.GetLeadingTrivia();
                updated = updated.WithoutLeadingTrivia().AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Doroti.Ui"))
                    .WithUsingKeyword(SyntaxFactory.Token(SyntaxKind.UsingKeyword).WithTrailingTrivia(SyntaxFactory.Space))
                    .WithTrailingTrivia(SyntaxFactory.EndOfLine("\n"), SyntaxFactory.EndOfLine("\n"))).WithLeadingTrivia(header);
            }
            var candidate = document.WithSyntaxRoot(updated);
            var candidateRoot = await candidate.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
            var candidateModel = await candidate.GetSemanticModelAsync() ?? throw new InvalidOperationException();
            var checkedNames = 0;
            foreach (var node in candidateRoot.GetAnnotatedNodes(annotationKind))
            {
                var index = int.Parse(node.GetAnnotations(annotationKind).Single().Data!);
                var actual = candidateModel.LookupNamespacesAndTypes(node.SpanStart, name: expected[index].Name);
                if (actual.Length != 1 || !SymbolEqualityComparer.Default.Equals(expected[index], actual[0]))
                    throw new InvalidOperationException($"Icon data type binding changed in {document.Name} at {index}.");
                checkedNames++;
            }
            if (checkedNames != names.Length || candidateModel.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error))
                throw new InvalidOperationException($"Icon data validation failed in {document.Name}.");
            if (!workspace.TryApplyChanges(candidate.Project.Solution)) throw new InvalidOperationException("Icon data simplification could not be applied.");
            report.Add(new { file = document.FilePath, count = checkedNames });
        }
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

if (args[0] is "fix-ide0001" or "check-ide0001")
{
    var featuresAssembly = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features");
    var nameAnalyzerType = featuresAssembly.GetType("Microsoft.CodeAnalysis.CSharp.Diagnostics.SimplifyTypeNames.CSharpSimplifyTypeNamesDiagnosticAnalyzer", true)!;
    var nameAnalyzer = (DiagnosticAnalyzer)Activator.CreateInstance(nameAnalyzerType, true)!;
    var nameProviderType = featuresAssembly.GetTypes().Single(t => t.Name == "SimplifyTypeNamesCodeFixProvider");
    var nameProvider = (CodeFixProvider)Activator.CreateInstance(nameProviderType, true)!;
    var report = new List<object>();
    var module = args.FirstOrDefault(a => a.StartsWith("--module=", StringComparison.Ordinal))?[9..];
    foreach (var projectId in projectIds)
    {
        var project = workspace.CurrentSolution.GetProject(projectId)!;
        if (module is not null && project.Name != module) continue;
        Console.WriteLine($"Analyzing IDE0001: {project.Name}");
        var compilation = await project.GetCompilationAsync() ?? throw new InvalidOperationException();
        var analysis = compilation.WithAnalyzers([nameAnalyzer], project.AnalyzerOptions);
        foreach (var documentId in project.DocumentIds)
        {
            var document = workspace.CurrentSolution.GetDocument(documentId)!;
            if (IsGenerated(document)) continue;
            Console.WriteLine($"  Checking {document.Name}");
            var originalTree = await project.GetDocument(documentId)!.GetSyntaxTreeAsync() ?? throw new InvalidOperationException();
            var documentDiagnostics = await analysis.GetAnalyzerSemanticDiagnosticsAsync(compilation.GetSemanticModel(originalTree), null, CancellationToken.None);
            if (documentDiagnostics.Any(d => d.Id == "AD0001"))
                throw new InvalidOperationException("IDE0001 analyzer failed: " + string.Join("\n", documentDiagnostics.Where(d => d.Id == "AD0001")));
            var diagnostics = documentDiagnostics.Where(d => d.Id == "IDE0001" && d.Location.SourceTree == originalTree).ToImmutableArray();
            if (diagnostics.IsEmpty) continue;
            report.Add(new { file = document.FilePath, count = diagnostics.Length });
            Console.WriteLine($"  {document.Name}: {diagnostics.Length}");
            if (args[0] != "fix-ide0001") continue;
            var actions = new List<CodeAction>();
            await nameProvider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, _) => actions.Add(a), CancellationToken.None));
            var context = new FixAllContext(document, nameProvider, FixAllScope.Document, actions.First().EquivalenceKey, ["IDE0001"],
                new DocumentDiagnostics(documentId, diagnostics), CancellationToken.None);
            var action = await nameProvider.GetFixAllProvider()!.GetFixAsync(context) ?? throw new InvalidOperationException("No IDE0001 fix-all action.");
            var operation = (await action.GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
            if (!workspace.TryApplyChanges(operation.ChangedSolution)) throw new InvalidOperationException("Name simplification could not be applied.");
        }
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    Console.WriteLine($"IDE0001 affected documents: {report.Count}");
    if (args[0] == "check-ide0001" && report.Count != 0) Environment.ExitCode = 1;
    return;
}

if (args[0] == "rename-generics")
{
    var changes = new List<object>();
    foreach (var projectId in projectIds)
    {
        var project = workspace.CurrentSolution.GetProject(projectId)!;
        if (!project.Name.StartsWith("Doroti.Framework.", StringComparison.Ordinal)) continue;
        foreach (var documentId in project.DocumentIds)
        {
            while (true)
            {
                var document = workspace.CurrentSolution.GetDocument(documentId)!;
                if (IsGenerated(document)) break;
                var root = await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
                var parameter = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .SelectMany(m => m.TypeParameterList?.Parameters.ToArray() ?? [])
                    .FirstOrDefault(p => p.Ancestors().OfType<TypeDeclarationSyntax>()
                        .Any(t => t.TypeParameterList?.Parameters.Any(tp => tp.Identifier.Text == p.Identifier.Text) == true));
                if (parameter is null) break;
                var owner = parameter.Ancestors().OfType<TypeDeclarationSyntax>().First().Identifier.Text;
                var renamed = owner switch
                {
                    "ColorSwatch" or "KeySet" => "TKey",
                    "InheritedModel" => "TModel",
                    "RawAutocomplete" => "TOption",
                    "Router" => "TConfiguration",
                    "CupertinoRouteTransitionMixin" or "_CupertinoSheetRouteTransitionMixin__sheet" or "ModalRoute" => "TRouteResult",
                    _ => "TValue",
                };
                var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
                var symbol = model.GetDeclaredSymbol(parameter) ?? throw new InvalidOperationException();
                var solution = await Renamer.RenameSymbolAsync(workspace.CurrentSolution, symbol,
                    new SymbolRenameOptions(), renamed);
                if (!workspace.TryApplyChanges(solution)) throw new InvalidOperationException("Rename could not be applied.");
                changes.Add(new { file = document.FilePath, owner, before = parameter.Identifier.Text, after = renamed });
                Console.WriteLine($"Renamed {document.Name}: {owner}.{symbol.ContainingSymbol.Name} -> {renamed}");
            }
        }
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

if (args[0] == "annotate-null-casts")
{
    var locations = System.Text.RegularExpressions.Regex.Matches(File.ReadAllText(args[3]),
        @"^(.*?)\((\d+),(\d+)\): error CS8600:", System.Text.RegularExpressions.RegexOptions.Multiline)
        .GroupBy(m => Path.GetFullPath(m.Groups[1].Value), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(g => g.Key, g => g.Select(m => (Line: int.Parse(m.Groups[2].Value), Column: int.Parse(m.Groups[3].Value))).Distinct().ToArray(), StringComparer.OrdinalIgnoreCase);
    var changes = new List<object>();
    foreach (var projectId in projectIds)
    foreach (var documentId in workspace.CurrentSolution.GetProject(projectId)!.DocumentIds)
    {
        var document = workspace.CurrentSolution.GetDocument(documentId)!;
        if (IsGenerated(document) || !locations.TryGetValue(document.FilePath!, out var positions)) continue;
        var root = await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
        var sourceText = await document.GetTextAsync();
        var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
        var casts = new HashSet<CastExpressionSyntax>();
        foreach (var position in positions)
        {
            var token = root.FindToken(sourceText.Lines[position.Line - 1].Start + position.Column - 1);
            var cast = token.Parent?.AncestorsAndSelf().OfType<CastExpressionSyntax>().FirstOrDefault();
            if (cast is null || cast.Type is NullableTypeSyntax) continue;
            if (model.GetTypeInfo(cast.Type).Type is not { IsReferenceType: true } type || type.TypeKind is TypeKind.Dynamic or TypeKind.Error) continue;
            casts.Add(cast);
        }
        if (casts.Count == 0) continue;
        // A reference cast of null returns null. Mark that result truthfully;
        // required consumers still report CS8603/CS8604 and require a contract fix.
        var updated = root.ReplaceNodes(casts, (_, rewritten) =>
            rewritten.WithType(SyntaxFactory.NullableType(rewritten.Type.WithoutTrivia()).WithTriviaFrom(rewritten.Type)));
        if (!workspace.TryApplyChanges(document.WithSyntaxRoot(updated).Project.Solution)) throw new InvalidOperationException("Nullable cast annotation failed.");
        changes.Add(new { file = document.FilePath, count = casts.Count });
        Console.WriteLine($"Nullable reference casts: {document.Name}: {casts.Count}");
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

if (args[0] == "simplify-casts")
{
    var changes = new List<object>();
    var module = args.FirstOrDefault(a => a.StartsWith("--module=", StringComparison.Ordinal))?[9..];
    foreach (var projectId in projectIds)
    {
        var project = workspace.CurrentSolution.GetProject(projectId)!;
        if (!project.Name.StartsWith("Doroti.Framework.", StringComparison.Ordinal)) continue;
        if (module is not null && project.Name != module) continue;
        Console.WriteLine($"Checking object bridges: {project.Name}");
        foreach (var documentId in project.DocumentIds)
        {
            var document = workspace.CurrentSolution.GetDocument(documentId)!;
            if (IsGenerated(document)) continue;
            var root = await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
            if (module is null && !root.ToFullString().Contains("#pragma warning disable", StringComparison.Ordinal)) continue;
            var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
            var candidates = new List<CastExpressionSyntax>();
            var nullCandidates = new HashSet<CastExpressionSyntax>();
            foreach (var cast in root.DescendantNodes().OfType<CastExpressionSyntax>())
            {
                if (Unwrap(cast.Expression) is not CastExpressionSyntax bridge) continue;
                if (model.GetTypeInfo(bridge.Type).Type?.SpecialType != SpecialType.System_Object) continue;
                var source = model.GetTypeInfo(bridge.Expression).Type;
                var target = model.GetTypeInfo(cast.Type).Type;
                if (Unwrap(bridge.Expression).IsKind(SyntaxKind.NullLiteralExpression) && target is { IsReferenceType: true } && target.TypeKind is not (TypeKind.Dynamic or TypeKind.Error))
                {
                    // Null-to-reference casts have no runtime check. Express the null
                    // honestly; non-null consumers will still produce diagnostics.
                    candidates.Add(cast);
                    nullCandidates.Add(cast);
                    continue;
                }
                if (source is null || target is null || source.TypeKind is TypeKind.Dynamic or TypeKind.Error || target.TypeKind == TypeKind.Error) continue;
                var conversion = model.ClassifyConversion(bridge.Expression, target);
                // Retain the outer cast and its static type, hence overload resolution.
                // Numeric, user-defined, boxing/unboxing and dynamic conversions are excluded.
                if (!(conversion.IsIdentity || conversion.IsReference) || conversion.IsUserDefined) continue;
                if (bridge.Type.DescendantTrivia().Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia))) continue;
                candidates.Add(cast);
            }
            if (candidates.Count == 0) continue;
            var updated = root.ReplaceNodes(candidates, (original, rewritten) =>
            {
                var result = rewritten.WithExpression(((CastExpressionSyntax)Unwrap(rewritten.Expression)).Expression);
                return nullCandidates.Contains(original) && result.Type is not NullableTypeSyntax
                    ? result.WithType(SyntaxFactory.NullableType(result.Type.WithoutTrivia()).WithTriviaFrom(result.Type))
                    : result;
            });
            if (!workspace.TryApplyChanges(document.WithSyntaxRoot(updated).Project.Solution))
                throw new InvalidOperationException("Object bridge changes could not be applied.");
            changes.Add(new { file = document.FilePath, count = candidates.Count, nullReferences = nullCandidates.Count });
            Console.WriteLine($"  {document.Name}: {candidates.Count}");
        }
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

// dotnet-format's combined name analyzer also visits every qualified type name.
// Call its exact IDE0002 predicate only for member-access nodes, and use its code fix.
var features = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Features");
var analyzerType = features.GetType("Microsoft.CodeAnalysis.CSharp.Diagnostics.SimplifyTypeNames.CSharpSimplifyTypeNamesDiagnosticAnalyzer", true)!;
var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(analyzerType, true)!;
var predicate = analyzerType.GetMethod("CanSimplifyTypeNameExpression", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)!;
var optionsType = Assembly.Load("Microsoft.CodeAnalysis.CSharp.Workspaces").GetType("Microsoft.CodeAnalysis.CSharp.Simplification.CSharpSimplifierOptions", true)!;
var options = optionsType.GetField("Default", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(null);
var descriptor = analyzer.SupportedDiagnostics.Single(d => d.Id == "IDE0002");
var providerType = features.GetTypes().Single(t => t.Name == "SimplifyTypeNamesCodeFixProvider");
var provider = (CodeFixProvider)Activator.CreateInstance(providerType, true)!;
var evidence = new List<object>();
foreach (var projectId in projectIds)
{
    var project = workspace.CurrentSolution.GetProject(projectId)!;
    Console.WriteLine($"Analyzing {project.Name}");
    foreach (var documentId in project.DocumentIds)
    {
        var document = workspace.CurrentSolution.GetDocument(documentId)!;
        if (IsGenerated(document)) continue;
        var root = await document.GetSyntaxRootAsync() ?? throw new InvalidOperationException();
        var model = await document.GetSemanticModelAsync() ?? throw new InvalidOperationException();
        var diagnostics = new List<Diagnostic>();
        foreach (var node in root.DescendantNodes().OfType<MemberAccessExpressionSyntax>())
        {
            object?[] parameters = [model, node, options, null, null, false, CancellationToken.None];
            if ((bool)predicate.Invoke(analyzer, parameters)! && (string?)parameters[4] == "IDE0002")
            {
                var span = (TextSpan)parameters[3]!;
                diagnostics.Add(Diagnostic.Create(descriptor, Location.Create(root.SyntaxTree, span)));
            }
        }
        if (diagnostics.Count == 0) continue;
        evidence.Add(new { file = document.FilePath, count = diagnostics.Count, locations = diagnostics.Select(d => d.Location.GetLineSpan().StartLinePosition.ToString()).ToArray() });
        Console.WriteLine($"  {document.Name}: {diagnostics.Count}");
        if (args[0] != "fix-ide0002") continue;
        var actions = new List<CodeAction>();
        await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostics[0], (a, _) => actions.Add(a), CancellationToken.None));
        var key = actions.First().EquivalenceKey;
        var context = new FixAllContext(document, provider, FixAllScope.Document, key, ["IDE0002"],
            new DocumentDiagnostics(documentId, diagnostics.ToImmutableArray()), CancellationToken.None);
        var action = await provider.GetFixAllProvider()!.GetFixAsync(context) ?? throw new InvalidOperationException("No fix-all action.");
        var operation = (await action.GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
        if (!workspace.TryApplyChanges(operation.ChangedSolution)) throw new InvalidOperationException("Simplification could not be applied.");
    }
}
File.WriteAllText(args[2], JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }));
Console.WriteLine($"IDE0002 affected documents: {evidence.Count}");
if (args[0] == "check-ide0002" && evidence.Count != 0) Environment.ExitCode = 1;

static bool IsGenerated(Document document) => document.FilePath is null || document.FilePath.Replace('\\', '/').Contains("/obj/", StringComparison.Ordinal) || document.Name.EndsWith(".g.cs", StringComparison.Ordinal);

static ExpressionSyntax Unwrap(ExpressionSyntax expression)
{
    while (expression is ParenthesizedExpressionSyntax parentheses) expression = parentheses.Expression;
    return expression;
}

sealed class DocumentDiagnostics(DocumentId id, ImmutableArray<Diagnostic> diagnostics) : FixAllContext.DiagnosticProvider
{
    public override Task<IEnumerable<Diagnostic>> GetDocumentDiagnosticsAsync(Document document, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<Diagnostic>>(document.Id == id ? diagnostics : []);
    public override Task<IEnumerable<Diagnostic>> GetProjectDiagnosticsAsync(Project project, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<Diagnostic>>([]);
    public override Task<IEnumerable<Diagnostic>> GetAllDiagnosticsAsync(Project project, CancellationToken cancellationToken) => Task.FromResult<IEnumerable<Diagnostic>>(diagnostics);
}
