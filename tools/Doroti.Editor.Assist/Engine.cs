using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;

namespace Doroti.Editor.Assist;

public sealed class Engine : IDisposable
{
    public static readonly SymbolDisplayFormat DisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
        SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
    private sealed class Entry(MSBuildWorkspace workspace, Project project, string? reason)
    {
        public MSBuildWorkspace Workspace { get; } = workspace;
        public Project Project { get; set; } = project;
        public string? Reason { get; } = reason;
        public TypeInfo[]? Catalog { get; set; }
        public HashSet<string> Overlays { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
    private readonly Dictionary<string, Entry> _projects = new(StringComparer.OrdinalIgnoreCase);
    private int _generation;
    private int _loadedGeneration;
    private readonly HashSet<string> _reloads = [];
    public int ProjectLoads { get; private set; }
    public void Invalidate() => Interlocked.Increment(ref _generation);
    public void Dispose() { foreach (var entry in _projects.Values) entry.Workspace.Dispose(); _projects.Clear(); }

    public async Task<object> HandleAsync(Request request, CancellationToken token)
    {
        if (_loadedGeneration != _generation) { Dispose(); _loadedGeneration = _generation; }
        if (request.Operation == "stats") return new { projectLoads = ProjectLoads, cachedProjects = _projects.Count };
        if (request.Project is null || request.Uri is null) throw new ArgumentException("Project and URI are required");
        var projectPath = Path.GetFullPath(request.Project);
        var file = FilePath(request.Uri);
        var snapshots = request.Snapshots ?? [];
        if (snapshots.Length > 32 || snapshots.Sum(s => (long)s.Text.Length) > 4 * 1024 * 1024)
            throw new ArgumentException("Snapshot limit: 32 documents / 4 MiB UTF-16 characters");
        var snapshot = snapshots.FirstOrDefault(s => FilePath(s.Uri).Equals(file, StringComparison.OrdinalIgnoreCase));
        if (snapshot is null || snapshot.Version != request.Version) throw new ArgumentException("Current document snapshot/version required");
        if (!_projects.TryGetValue(projectPath, out var entry))
        {
            if (_projects.Count >= 4)
            { var first = _projects.First(); first.Value.Workspace.Dispose(); _projects.Remove(first.Key); }
            var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["Configuration"] = "Debug" });
            workspace.LoadMetadataForReferencedProjects = false;
            var warnings = new List<string>();
            workspace.RegisterWorkspaceFailedHandler(e => { warnings.Add(e.Diagnostic.Message); Console.Error.WriteLine(e.Diagnostic.Message); });
            try
            {
                ProjectLoads++;
                var project = await workspace.OpenProjectAsync(projectPath, cancellationToken: token);
                var failures = workspace.Diagnostics.Select(d => d.Message).ToArray();
                entry = new Entry(workspace, project, failures.Length == 0 ? null : string.Join("; ", failures).Truncate(800));
                _projects.Add(projectPath, entry);
            }
            catch (OperationCanceledException) { workspace.Dispose(); throw; }
            catch (Exception error)
            {
                workspace.Dispose();
                return Fallback(request, snapshot.Text, "Project evaluation failed: " + error.Message);
            }
        }
        var solution = entry.Project.Solution;
        var catalogChanged = false;
        var incoming = snapshots.Select(s => FilePath(s.Uri)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var oldPath in entry.Overlays.Where(p => !incoming.Contains(p)).ToArray())
        {
            foreach (var d in solution.Projects.SelectMany(p => p.Documents).Where(d => d.FilePath?.Equals(oldPath, StringComparison.OrdinalIgnoreCase) == true))
                if (File.Exists(oldPath)) solution = solution.WithDocumentText(d.Id, SourceText.From(await File.ReadAllTextAsync(oldPath, token)));
            entry.Overlays.Remove(oldPath); catalogChanged = true;
        }
        foreach (var s in snapshots)
        {
            var snapshotPath = FilePath(s.Uri);
            var docs = solution.Projects.SelectMany(p => p.Documents)
                .Where(d => d.FilePath?.Equals(snapshotPath, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) == true)
                .Select(d => d.Id).ToArray();
            if (docs.Length == 0 && snapshotPath.Equals(file, StringComparison.OrdinalIgnoreCase))
            {
                if (entry.Reason is not null) return Fallback(request, snapshot.Text, "Project evaluation/reference resolution failed: " + entry.Reason);
                // New unsaved files must already be members of evaluated Compile globs.
                // Reload once when the document exists on disk; never invent linked project ownership.
                if (File.Exists(file) && _reloads.Add(request.Id))
                {
                    entry.Workspace.Dispose(); _projects.Remove(projectPath);
                    try { return await HandleAsync(request, token); }
                    finally { _reloads.Remove(request.Id); }
                }
                return new Analysis(false, "Document is not in this application's evaluated Compile items", false, false, "none", [], null, [], [], [], []);
            }
            foreach (var id in docs)
            {
                var old = await solution.GetDocument(id)!.GetTextAsync(token);
                if (!old.ContentEquals(SourceText.From(s.Text)))
                { solution = solution.WithDocumentText(id, SourceText.From(s.Text)); catalogChanged = true; }
                entry.Overlays.Add(snapshotPath);
            }
        }
        entry.Project = solution.GetProject(entry.Project.Id)!;
        if (catalogChanged) entry.Catalog = null;
        var document = entry.Project.Documents.Concat(solution.Projects.Where(p => p.Id != entry.Project.Id).SelectMany(p => p.Documents))
            .FirstOrDefault(d => d.FilePath?.Equals(file, StringComparison.OrdinalIgnoreCase) == true);
        if (document is null) return new Analysis(false, "Document belongs to a different project", false, false, "none", [], null, [], [], [], []);
        var root = (CompilationUnitSyntax)(await document.GetSyntaxRootAsync(token))!;
        var model = (await document.GetSemanticModelAsync(token))!;
        var widget = model.Compilation.GetTypeByMetadataName("Doroti.Framework.Widgets.Widget");
        if (widget?.ContainingAssembly.Identity.Name != "Doroti.Framework.Widgets") widget = null;
        if (request.Operation == "catalog")
        {
            entry.Catalog ??= BuildCatalog(model.Compilation, widget, token);
            return new { types = entry.Catalog, semantic = widget is not null, reason = entry.Reason, projectLoads = ProjectLoads };
        }
        var semantic = widget is not null;
        if (!semantic) return Fallback(request, snapshot.Text, entry.Reason ?? "Restore/build references are unavailable; semantic refactorings disabled");
        var assist = new Refactorings(root, model, widget!, snapshot.Text, request, token);
        if (request.Operation == "transform")
        {
            var transformed = assist.Transform(request.Action ?? "", request.Name);
            if (transformed.Reason is not null || transformed.Edits.Length == 0) return transformed;
            var updated = SourceText.From(snapshot.Text).WithChanges(transformed.Edits.Select(e => new TextChange(new TextSpan(e.Start, e.Length), e.Text)));
            var shift = 0; var spans = new List<TextSpan>();
            foreach (var e in transformed.Edits.OrderBy(e => e.Start)) { spans.Add(new TextSpan(e.Start + shift, e.Text.Length)); shift += e.Text.Length - e.Length; }
            var options = entry.Workspace.Options.WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, snapshot.Text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n");
            var formatted = await Formatter.FormatAsync(document.WithText(updated), spans, options, cancellationToken: token);
            var changes = await formatted.GetTextChangesAsync(document, token);
            return transformed with { Edits = changes.Select(c => new Edit(c.Span.Start, c.Span.Length, c.NewText ?? "")).ToArray() };
        }
        return assist.Analyze(entry.Reason);
    }

    private static Analysis Fallback(Request request, string source, string reason)
    {
        var root = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp14)).GetRoot();
        return new(false, reason, true, IsCode(root, request.Offset), "fallback", [], null, [], [], [], []);
    }

    public static bool IsCode(SyntaxNode root, int offset)
    {
        if (offset < 0 || offset > root.FullSpan.End) return false;
        var pos = Math.Min(offset, Math.Max(0, root.FullSpan.End - 1));
        var trivia = root.FindTrivia(pos, findInsideTrivia: true);
        if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)
            || trivia.IsKind(SyntaxKind.DisabledTextTrivia) || trivia.IsDirective || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)) return false;
        var token = root.FindToken(pos, findInsideTrivia: true);
        return !token.Parent!.AncestorsAndSelf().Any(n => n is LiteralExpressionSyntax l && l.Token.Value is string
            || n is InterpolatedStringExpressionSyntax);
    }

    public static string FilePath(string value)
    {
        var uri = new Uri(value);
        if (!uri.IsFile) throw new ArgumentException("Only local file URIs are supported");
        var local = uri.LocalPath;
        // VS Code encodes the drive colon (file:///c%3A/...). Uri.LocalPath then retains a leading slash.
        if (OperatingSystem.IsWindows() && local.Length >= 3 && local[0] == '/' && char.IsLetter(local[1]) && local[2] == ':') local = local[1..];
        return Path.GetFullPath(local);
    }

    public static bool Derives(ITypeSymbol? type, INamedTypeSymbol? ancestor)
    {
        if (ancestor is null) return false;
        for (var t = type as INamedTypeSymbol; t is not null; t = t.BaseType)
            if (SymbolEqualityComparer.Default.Equals(t.OriginalDefinition, ancestor.OriginalDefinition)) return true;
        return false;
    }

    private static TypeInfo[] BuildCatalog(Compilation compilation, INamedTypeSymbol? widget, CancellationToken token)
    {
        var types = new List<TypeInfo>();
        void Visit(INamespaceSymbol ns)
        {
            foreach (var child in ns.GetNamespaceMembers()) Visit(child);
            foreach (var type in ns.GetTypeMembers())
            {
                token.ThrowIfCancellationRequested();
                if (types.Count >= 6000 || type.DeclaredAccessibility != Accessibility.Public && !type.Locations.Any(l => l.IsInSource)) continue;
                var space = type.ContainingNamespace.IsGlobalNamespace ? "" : type.ContainingNamespace.ToDisplayString();
                var isWidget = Derives(type, widget);
                if (!isWidget && !space.StartsWith("Doroti.", StringComparison.Ordinal) && !type.Locations.Any(l => l.IsInSource)) continue;
                var constructors = type.InstanceConstructors.Where(c => !type.IsAbstract && compilation.IsSymbolAccessibleWithin(c, compilation.Assembly))
                    .Select(c => c.Parameters.Select(Parameter).ToArray()).ToArray();
                types.Add(new(type.Name, space, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    isWidget ? "widget" : type.TypeKind == TypeKind.Enum ? "enum" : "type", constructors,
                    type.TypeKind == TypeKind.Enum ? type.GetMembers().OfType<IFieldSymbol>().Where(f => f.HasConstantValue).Select(f => f.Name).ToArray() : [],
                    type.GetMembers().OfType<IMethodSymbol>().Where(m => m.IsStatic && m.DeclaredAccessibility == Accessibility.Public && m.Name.StartsWith("Create", StringComparison.Ordinal))
                        .Select(m => m.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)).ToArray(),
                    type.GetDocumentationCommentXml(cancellationToken: token).Truncate(1200)));
            }
        }
        Visit(compilation.GlobalNamespace);
        return types.ToArray();
    }

    public static ParameterInfo Parameter(IParameterSymbol p)
    {
        var invoke = (p.Type as INamedTypeSymbol)?.DelegateInvokeMethod;
        var defaultValue = p.HasExplicitDefaultValue ? p.ExplicitDefaultValue switch
        {
            null => p.Type.TypeKind == TypeKind.TypeParameter || p.Type.IsValueType && (p.Type as INamedTypeSymbol)?.OriginalDefinition.SpecialType != SpecialType.System_Nullable_T ? "default" : "null",
            bool b => b ? "true" : "false",
            string s => Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(s, true),
            char c => Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(c, true),
            var v when p.Type.TypeKind == TypeKind.Enum => $"({p.Type.ToDisplayString(DisplayFormat)}){Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture)}",
            decimal m => m.ToString(System.Globalization.CultureInfo.InvariantCulture) + "m",
            float f when float.IsNaN(f) => "float.NaN",
            float f when float.IsPositiveInfinity(f) => "float.PositiveInfinity",
            float f when float.IsNegativeInfinity(f) => "float.NegativeInfinity",
            float f => f.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "f",
            double d when double.IsNaN(d) => "double.NaN",
            double d when double.IsPositiveInfinity(d) => "double.PositiveInfinity",
            double d when double.IsNegativeInfinity(d) => "double.NegativeInfinity",
            double d => d.ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "d",
            var v => Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture)
        } : null;
        return new(p.Name, p.Type.ToDisplayString(DisplayFormat), p.IsOptional, defaultValue,
            invoke?.Parameters.All(a => a.RefKind == RefKind.None) == true ? invoke.Parameters.Select(a => a.Name).ToArray() : null,
            invoke?.ReturnType.ToDisplayString(DisplayFormat));
    }
}

internal static class TextLimits
{
    public static string Truncate(this string? text, int max) => text is null ? "" : text.Length > max ? text[..max] : text;
}
