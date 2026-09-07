using System.Reflection;
using System.Runtime.Loader;
using Doroti.DartToCSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var repo = Path.GetFullPath(args[0]);
var fixture = Path.Combine(repo, "tools/Doroti.DartToCSharp/validation/virtual-dispatch/fixtures");
var output = Path.Combine(repo, ".doroti", args.Contains("--before") ? "compiler-dispatch-before" : "compiler-dispatch-after");
if (!args.Contains("--before"))
{
    var report = new DartCompiler().Compile(Path.Combine(fixture, "selection.json"), output, Path.Combine(repo, ".doroti/compiler-dispatch-cache"));
    Require(report.Success, string.Join(Environment.NewLine, report.Diagnostics.Select(d => d.Message)));
}
var trees = Directory.GetFiles(output, "*.g.cs", SearchOption.AllDirectories).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p)).Append(CSharpSyntaxTree.ParseText("namespace CompilerDispatchFixture.Framework; public class ExternalBase { public virtual long calculate(long amount = 1) => amount; } public class ExternalGeneric { public virtual T? choose<T>(T? value) => value; } public class ExternalPainter { public virtual long paint(long area) => area; } public class PrivateBase { internal virtual long _value { get; set; } = 11; public long readBase() => _value; }")).ToArray();
var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
    .Concat(new[] { typeof(Doroti.Runtime.DartRuntimePrimitives).Assembly.Location, typeof(Doroti.Ui.Color).Assembly.Location }).Distinct();
var compilation = CSharpCompilation.Create("DispatchFixture", trees, paths.Select(p => MetadataReference.CreateFromFile(p)), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
using var stream = new MemoryStream();
var emit = compilation.Emit(stream);
Require(emit.Success, string.Join(Environment.NewLine, emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
stream.Position = 0;
var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
Type Type(string name) => assembly.GetType("CompilerDispatchFixture.Framework." + name)!;
object Create(string name) => Activator.CreateInstance(Type(name))!;
var derived = Create("Derived");
Require(Equals(Type("Base").GetProperty("value")!.GetValue(derived), 42L), "base property dispatch");
Require(Equals(Type("Base").GetMethod("calculate")!.Invoke(derived, [2L, 7L]), 9L), "optional parameter family dispatch");
Require(Equals(Type("Base").GetMethod("accept")!.Invoke(derived, ["abc"]), 3L), "covariant parameter checked inside override");
Require(Equals(Type("Base").GetProperty("value")!.GetValue(Create("Concrete")), 99L), "abstract override chain");
Require(Equals(Type("NullableBase").GetProperty("elevation")!.GetValue(Create("NullableDerived")), 2d), "nullable value return contract");
Require(Create("Description").ToString() == "summary", "Object.ToString optional overload bridge");
var extent = Create("ExtentDerived");
Type("ExtentBase").GetProperty("extent")!.SetValue(extent, 73L);
Require(Equals(Type("ExtentBase").GetProperty("extent")!.GetValue(extent), 73L), "getter and setter use the same virtual slot");
var values = new List<long> { 1, 2 };
var generic = Type("GenericBase`1").MakeGenericType(typeof(List<long>));
Require(ReferenceEquals(generic.GetMethod("transform")!.Invoke(Create("Leaf"), [values]), values), "generic substitution across intermediate base");
Require(Equals(Type("MixedBase").GetProperty("metric")!.GetValue(Create("MixedDerived")), 30L), "mixin getter inherited through class slot");
Require(Equals(Type("MixedBase").GetMethod("measure")!.Invoke(Create("MixedDerived"), null), 40L), "mixin method inherited through class slot");
Require(Equals(Type("Metrics").GetMethod("measure")!.Invoke(Create("MixedDerived"), null), 40L), "inherited mixin interface uses the same override slot");
Require(Equals(Type("MixedBase").GetProperty("metric")!.GetValue(Create("MixedField")), 50L), "field overrides a materialized mixin getter");
Require(Equals(Type("RenamedBase").GetMethod("size")!.Invoke(Create("RenamedDerived"), ["abcd"]), 4L), "renamed covariant positional argument");
Require(Equals(Type("ExternalBase").GetMethod("calculate")!.Invoke(Create("ExternalDerived"), [2L]), 9L), "frozen external CLR slot forwards optional additions");
Require(ReferenceEquals(generic.GetMethod("transform")!.Invoke(Activator.CreateInstance(Type("LeafGeneric`1").MakeGenericType(typeof(long))), [values]), values), "open generic substitution across intermediate base");
Require(((Doroti.Ui.Color)Create("RuntimeColor")).resolveFrom("context").value == 77, "Ui generic color entry point forwards to typed context");
var privateDerived = Create("PrivateDerived");
Require(Equals(Type("PrivateBase").GetMethod("readBase")!.Invoke(privateDerived, null), 11L) &&
    Equals(Type("PrivateDerived").GetMethod("readDerived")!.Invoke(privateDerived, null), 22L), "Dart library-private state stays independent");
Require(Equals(Type("OrderDerived").GetMethod("run")!.Invoke(Create("OrderDerived"), [System.Type.Missing, System.Type.Missing]), 1020L), "reordered named arguments retain their own defaults");
Require(Equals(Type("ExternalPainter").GetMethod("paint")!.Invoke(Create("ConcretePainter"), [20L]), 15L), "external paint bridge invokes the derived abstract overload implementation");
Require(Equals(Type("ExternalGeneric").GetMethod("choose")!.MakeGenericMethod(typeof(string)).Invoke(Create("GenericChooser"), ["value"]), "value"), "generic nullable external bridge retains legal constraints");
var genericField = Activator.CreateInstance(Type("GenericField`1").MakeGenericType(typeof(long)));
Require(Type("GenericFieldBase`1").MakeGenericType(typeof(List<long>)).GetProperty("value")!.GetValue(genericField) is null, "generic field implements the substituted property type");
try
{
    Type("Base").GetMethod("accept")!.Invoke(derived, [new object()]);
    throw new InvalidOperationException("Invalid covariant input was accepted");
}
catch (TargetInvocationException error) when (error.InnerException is InvalidCastException) { }
var unresolved = new DartCompiler().Compile(Path.Combine(fixture, "missing-base-selection.json"), Path.Combine(repo, ".doroti/compiler-dispatch-unresolved"), Path.Combine(repo, ".doroti/compiler-dispatch-cache"));
Require(!unresolved.Success && unresolved.Diagnostics.Any(d => d.Code == "DOTCONV902"), "incomplete base graph is rejected rather than silently hiding overrides");
var deterministicOutput = Path.Combine(repo, ".doroti/compiler-dispatch-deterministic");
var deterministic = new DartCompiler().Compile(Path.Combine(fixture, "selection.json"), deterministicOutput, Path.Combine(repo, ".doroti/compiler-dispatch-cache"), maxDegreeOfParallelism: 1);
Require(deterministic.Success, "serial compilation succeeds");
foreach (var file in Directory.GetFiles(output, "*.g.cs", SearchOption.AllDirectories))
    Require(File.ReadAllBytes(file).SequenceEqual(File.ReadAllBytes(Path.Combine(deterministicOutput, Path.GetRelativePath(output, file)))), "serial/parallel generated source identity");
Require(File.ReadAllText(Path.Combine(output, "source-map.json")) == File.ReadAllText(Path.Combine(deterministicOutput, "source-map.json")), "serial/parallel source map identity");
if (args.Contains("--upstream"))
{
    var upstreamOutput = Path.Combine(repo, ".doroti/compiler-dispatch-upstream");
    var upstream = new DartCompiler().Compile(Path.Combine(fixture, "upstream-selection.json"), upstreamOutput, Path.Combine(repo, ".doroti/compiler-dispatch-cache"));
    Require(!upstream.Diagnostics.Any(d => d.Code == "DOTCONV900"), "upstream targeted output has valid C# syntax");
    var targets = new Dictionary<string, string[]>
    {
        ["_WidgetStateAnd__widget_state"] = ["isSatisfiedBy"],
        ["_WidgetStateOr__widget_state"] = ["isSatisfiedBy"],
        ["WordBoundary"] = ["getTextBoundaryAt"],
        ["_UntilTextBoundary__text_painter"] = ["getLeadingTextBoundaryAt", "getTrailingTextBoundaryAt"],
        ["AlignmentTween"] = ["lerp"],
        ["FractionalOffsetTween"] = ["lerp"],
        ["AlignmentGeometryTween"] = ["lerp"],
    };
    var upstreamClasses = Directory.GetFiles(upstreamOutput, "*.g.cs", SearchOption.AllDirectories)
        .SelectMany(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p)).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()).ToArray();
    foreach (var (name, members) in targets)
    foreach (var member in members)
        Require(upstreamClasses.Single(c => c.Identifier.ValueText == name).Members.OfType<MethodDeclarationSyntax>()
            .Single(m => m.Identifier.ValueText == member).Modifiers.Any(SyntaxKind.OverrideKeyword), $"pinned Flutter override: {name}.{member}");
    Console.WriteLine($"Pinned Flutter target slots: PASS; partial selection diagnostics={upstream.Diagnostics.Length}; aggregate build not claimed");
}
Console.WriteLine("Compiler virtual dispatch: PASS (generated C# compile, base/interface behavior, external bridges, private isolation, missing-base rejection, serial/parallel determinism)");
static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}
