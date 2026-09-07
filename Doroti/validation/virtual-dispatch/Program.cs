using System.Text.Json;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var repo = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
var sourceRoot = Path.Combine(repo, "Doroti/src");
var findings = new List<Finding>();
var nearMisses = new List<Finding>();
var errors = new List<object>();
var totalMembers = 0;
var totalFiles = 0;
var projects = Directory.GetDirectories(sourceRoot, "Doroti.Framework.*").Where(p => File.Exists(Path.Combine(p, Path.GetFileName(p)+".csproj"))).Order().ToArray();
var assemblyFiles = Directory.GetFiles(sourceRoot, "*.dll", SearchOption.AllDirectories)
    .Where(p => p.Contains("/bin/Release/".Replace('/', Path.DirectorySeparatorChar)) && !p.Contains("/ref/".Replace('/', Path.DirectorySeparatorChar)))
    .GroupBy(Path.GetFileNameWithoutExtension).Select(g => (Name: g.Key!, Path: g.OrderBy(p => p.Contains(Path.DirectorySeparatorChar+g.Key+Path.DirectorySeparatorChar+"bin") ? 0 : 1).ThenBy(p => p.Length).First()))
    .Where(p => IsManagedAssembly(p.Path)).ToDictionary(p => p.Name, p => p.Path);
var trusted = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
foreach (var project in projects)
{
    var name = Path.GetFileName(project);
    var xml = XDocument.Load(Path.Combine(project, name + ".csproj"));
    var removed = xml.Descendants("Compile").Select(e => (string?)e.Attribute("Remove")).OfType<string>().ToHashSet();
    var files = Directory.GetFiles(project, "*.cs", SearchOption.AllDirectories)
        .Where(p => !Path.GetRelativePath(project,p).Split(Path.DirectorySeparatorChar).Any(s => s is "bin" or "obj"))
        .Where(p => !removed.Contains(Path.GetRelativePath(project, p))).ToArray();
    totalFiles += files.Length;
    var trees = files.Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), CSharpParseOptions.Default, p)).ToList();
    trees.Add(CSharpSyntaxTree.ParseText("global using System; global using System.Collections.Generic; global using System.IO; global using System.Linq; global using System.Net.Http; global using System.Threading; global using System.Threading.Tasks;"));
    var refs = trusted.Concat(assemblyFiles.Where(p => p.Key != name && !p.Key.StartsWith("System.") && !p.Key.StartsWith("Microsoft.")).Select(p => p.Value))
        .Distinct().Select(p => MetadataReference.CreateFromFile(p));
    var compilation = CSharpCompilation.Create(name, trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));
    var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
    if(diagnostics.Length > 0) errors.Add(new { project = name, count = diagnostics.Length, examples = diagnostics.Take(5).Select(d => d.ToString()).ToArray() });
    var countBefore = findings.Count;
    foreach(var tree in trees.Where(t => t.FilePath.Length > 0))
    {
        var model = compilation.GetSemanticModel(tree);
        foreach(var node in tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            if(node is not (MethodDeclarationSyntax or PropertyDeclarationSyntax or IndexerDeclarationSyntax or EventDeclarationSyntax)) continue;
            var member = model.GetDeclaredSymbol(node);
            if(member is null || member.IsStatic || member.IsOverride || member.ContainingType?.TypeKind != TypeKind.Class) continue;
            totalMembers++;
            for(var baseType = member.ContainingType.BaseType; baseType is not null; baseType = baseType.BaseType)
            {
                var candidates = baseType.GetMembers(member.Name).Where(b => !b.IsStatic && b.DeclaredAccessibility != Accessibility.Private && SameSignature(member,b)).ToArray();
                if(candidates.Length == 0) {
                    if(member is IMethodSymbol && (member.IsVirtual || member.IsAbstract) && !member.Name.StartsWith('_'))
                    foreach(var candidate in baseType.GetMembers(member.Name).OfType<IMethodSymbol>().Where(m => m.IsVirtual || m.IsAbstract || m.IsOverride))
                        if (!IsIntentionalOverload(member, candidate))
                            nearMisses.Add(new Finding(Path.GetRelativePath(repo, tree.FilePath).Replace('\\', '/'), node.GetLocation().GetLineSpan().StartLinePosition.Line + 1, member.ToDisplayString(), candidate.ToDisplayString()));
                    continue;
                }
                var parent = candidates[0];
                var virtualBase = parent.IsVirtual || parent.IsAbstract || parent.IsOverride;
                if(virtualBase)
                    findings.Add(new Finding(Path.GetRelativePath(repo, tree.FilePath).Replace('\\', '/'), node.GetLocation().GetLineSpan().StartLinePosition.Line + 1, member.ToDisplayString(), parent.ToDisplayString()));
                break;
            }
        }
    }
    Console.WriteLine($"{name}: files={files.Length}, findings={findings.Count-countBefore}, compilationErrors={diagnostics.Length}");
}
var output = args.Length > 1 ? args[1] : Path.Combine(repo,".doroti/virtual-dispatch-before.json");
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
File.WriteAllText(output, JsonSerializer.Serialize(new { projects=projects.Length, totalFiles, totalMembers, findings, nearMisses = nearMisses.Distinct(), errors }, new JsonSerializerOptions { WriteIndented=true }));
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var allowed = JsonSerializer.Deserialize<ExceptionEntry[]>(File.ReadAllText(Path.Combine(repo, "Doroti/validation/virtual-dispatch/intentional-hiding.json")), options)!;
nearMisses = nearMisses.Distinct().ToList();
var unexpected = findings.Where(f => !allowed.Any(a => a.Member == f.Member && a.BaseMember == f.BaseMember)).ToArray();
var stale = allowed.Where(a => string.IsNullOrWhiteSpace(a.Reason) || !findings.Any(f => a.Member == f.Member && a.BaseMember == f.BaseMember)).ToArray();
foreach (var f in unexpected.Concat(nearMisses)) Console.Error.WriteLine($"{f.File}:{f.Line}: {f.Member} does not override {f.BaseMember}");
foreach (var a in stale) Console.Error.WriteLine($"Stale or undocumented exception: {a.Member}");
Console.WriteLine($"Audit: {findings.Count} shadows ({findings.Count - unexpected.Length} documented), {unexpected.Length} unexpected shadows, {nearMisses.Count} signature mismatches, {errors.Count} project errors; report={output}");
Environment.ExitCode = unexpected.Length + nearMisses.Count + stale.Length + errors.Count == 0 ? 0 : 1;

static bool IsIntentionalOverload(ISymbol member, IMethodSymbol parent)
{
    // A typed convenience overload is safe only while the canonical base slot is also implemented.
    if (member.ContainingType.GetMembers(member.Name).OfType<IMethodSymbol>().Any(m => m.IsOverride && SameSignature(m, parent))) return true;
    if (member is not IMethodSymbol method) return false;
    // Dart unary '-' and binary '-' have distinct arity, despite the translated common name.
    return parent.Name == "op_Subtract" && parent.Parameters.Length == 0 && method.Parameters.Length == 1 &&
        member.ContainingNamespace.ToDisplayString() == "Doroti.Framework.Painting" &&
        member.ContainingType.Name is "Alignment" or "AlignmentDirectional" or "BorderRadius" or "BorderRadiusDirectional" or "EdgeInsets" or "EdgeInsetsDirectional";
}

static bool SameSignature(ISymbol a, ISymbol b) => (a,b) switch {
    (IMethodSymbol x, IMethodSymbol y) => x.Arity == y.Arity && Params(x.Parameters,y.Parameters),
    (IPropertySymbol x, IPropertySymbol y) => Params(x.Parameters,y.Parameters),
    (IEventSymbol, IEventSymbol) => true,
    _ => false,
};
static bool Params(System.Collections.Immutable.ImmutableArray<IParameterSymbol> a, System.Collections.Immutable.ImmutableArray<IParameterSymbol> b) =>
    a.Length == b.Length && a.Zip(b).All(p => p.First.RefKind == p.Second.RefKind && TypeKey(p.First.Type) == TypeKey(p.Second.Type));
static string TypeKey(ITypeSymbol t) => t switch {
    ITypeParameterSymbol p when p.TypeParameterKind == TypeParameterKind.Method => "!!"+p.Ordinal,
    IArrayTypeSymbol a => TypeKey(a.ElementType)+"["+a.Rank+"]",
    INamedTypeSymbol n when n.IsGenericType => n.OriginalDefinition.ToDisplayString()+"<"+string.Join(",",n.TypeArguments.Select(TypeKey))+">",
    _ => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
};


static bool IsManagedAssembly(string path)
{
    using var stream = File.OpenRead(path);
    using var pe = new PEReader(stream);
    return pe.HasMetadata;
}

internal sealed record Finding(string File, int Line, string Member, string BaseMember);
internal sealed record ExceptionEntry(string Member, string BaseMember, string Reason);
