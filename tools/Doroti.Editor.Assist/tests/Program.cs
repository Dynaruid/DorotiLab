using System.Reflection;
using Doroti.Editor.Assist;
using Doroti.Framework.Widgets;
using Doroti.Framework.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Transform = Doroti.Editor.Assist.Transform;

var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
    .Concat(Directory.GetFiles(AppContext.BaseDirectory, "Doroti.*.dll")).Distinct()
    .Select(p => MetadataReference.CreateFromFile(p)).ToArray();
int passed = 0;
void Check(bool condition, string label) { if (!condition) throw new Exception(label); Console.WriteLine("PASS: " + label); passed++; }
CSharpCompilation Compilation(string text) => CSharpCompilation.Create("Fixture" + Guid.NewGuid().ToString("N"),
    [CSharpSyntaxTree.ParseText(text, new CSharpParseOptions(LanguageVersion.CSharp14))], references,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
Refactorings Assist(string text, int offset, int length = 0)
{
    var compilation = Compilation(text); var tree = compilation.SyntaxTrees.Single();
    return new((CompilationUnitSyntax)tree.GetRoot(), compilation.GetSemanticModel(tree), compilation.GetTypeByMetadataName("Doroti.Framework.Widgets.Widget")!,
        text, new Request("test", "analyze", Offset: offset, Length: length), default);
}
string Apply(string text, Transform transform)
{
    Check(transform.Reason is null && transform.Edits.Length > 0, "transform enabled");
    foreach (var edit in transform.Edits.OrderByDescending(e => e.Start)) text = text[..edit.Start] + edit.Text + text[(edit.Start + edit.Length)..];
    return text;
}
Assembly Compile(string text)
{
    var compilation = Compilation(text); using var stream = new MemoryStream(); var emit = compilation.Emit(stream);
    Check(emit.Success, "generated code compiles: " + string.Join(" | ", emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
    return Assembly.Load(stream.ToArray());
}
const string prefix = "using Doroti.Framework.Widgets;\nnamespace Fixtures;\n";
if (args.Length > 0) Compile(File.ReadAllText(args[0]));
if (OperatingSystem.IsWindows()) Check(Engine.FilePath("file:///c%3A/Users/example%23name/한글.cs") == @"c:\Users\example#name\한글.cs", "encoded Windows drive, hash and Unicode URI");
var defaults = Compilation("public class Values { public Values(bool reverse = false, double maximum = double.PositiveInfinity, string title = \"x\", System.Threading.CancellationToken token = default) { } }");
var parameters = defaults.GetTypeByMetadataName("Values")!.InstanceConstructors.Single().Parameters.Select(Engine.Parameter).ToArray();
Check(parameters.Select(p => p.Default).SequenceEqual(new[] { "false", "double.PositiveInfinity", "\"x\"", "default" }), "catalog defaults are C# literals rather than localized CLR strings");
var original = prefix + "public sealed class Hello : StatelessWidget { public override Widget build(BuildContext context) => new Text(\"한글\"); }";
foreach (var wrapper in new[] { "Center", "Padding", "Container", "SizedBox", "Align", "Row", "Column", "Stack", "Builder", "LayoutBuilder", "SingleChildScrollView" })
{
    var assist = Assist(original, original.IndexOf("Text(", StringComparison.Ordinal) + 2);
    var transformed = Apply(original, assist.Transform("wrap:" + wrapper, null));
    var assembly = Compile(transformed);
    var instance = (StatelessWidget)Activator.CreateInstance(assembly.GetType("Fixtures.Hello")!)!;
    Check(instance.build(null!) is Widget, wrapper + " constructs an actual Widget");
}
var collection = prefix + "public class Hello : StatelessWidget { public override Widget build(BuildContext context) => new Row(children: [new Text(\"before\"), new Text(\"A\"), /* keep */ new Text(\"B\"), new Text(\"after\")]); }";
var start = collection.IndexOf("new Text(\"A\")", StringComparison.Ordinal); var end = collection.IndexOf("new Text(\"B\")", StringComparison.Ordinal) + "new Text(\"B\")".Length;
var listed = Apply(collection, Assist(collection, start, end - start).Transform("wrap:Column", null));
Check(listed.Contains("/* keep */") && listed.Contains("\"before\"") && listed.Contains("\"after\""), "collection commas, comments and surrounding items preserved");
Compile(listed);
var targetTyped = prefix + "public class Hello : StatelessWidget { Text Message() => new(\"Hello\"); public override Widget build(BuildContext context) => Message(); }";
Check(Assist(targetTyped, targetTyped.IndexOf("=> Message", StringComparison.Ordinal) + 3).Analyze(null).Actions.Any(a => a.Id == "wrap:Center"), "Widget-returning invocation resolved");
var wrong = "class Widget {} class Text : Widget {} class A { Widget M() => new Text(); }";
Check(Assist(wrong, wrong.IndexOf("new Text", StringComparison.Ordinal) + 5).Analyze(null).Actions.Length == 0, "same-name unrelated Widget rejected");
var raw = prefix + "class Hello : StatelessWidget { public override Widget build(BuildContext c) => new Text(\"\"\"new Column()\"\"\"); }";
Check(!Assist(raw, raw.IndexOf("new Column", StringComparison.Ordinal) + 5).Analyze(null).Code, "raw string is not code");
var invalid = original.Replace("new Text(\"한글\")", "new Text(");
Check(!Assist(invalid, invalid.IndexOf("new Text", StringComparison.Ordinal) + 5).Analyze(null).Actions.Any(a => a.Id.StartsWith("wrap:")), "unfinished construction rejected");
Check(!Assist(original, original.IndexOf("new Text", StringComparison.Ordinal)).Analyze(null).Actions.Any(a => a.Id == "wrap:Expanded"), "Expanded withheld outside Flex children");
var flex = Assist(collection, start).Analyze(null);
Check(flex.Actions.Any(a => a.Id == "wrap:Expanded"), "Expanded offered for a direct Row child");
var flexSource = Apply(collection, Assist(collection, start).Transform("wrap:Expanded", null));
Compile(flexSource);
Check(!Assist(flexSource, flexSource.IndexOf("Widgets.Expanded", StringComparison.Ordinal) + 10).Analyze(null).Actions.Any(a => a.Id == "wrap:Padding"), "parent-data widget is not wrapped in an incompatible render object");
Check(!Assist(flexSource, flexSource.IndexOf("new Row", StringComparison.Ordinal) + 5).Analyze(null).Actions.Any(a => a.Id == "swap:Stack"), "Row with Expanded cannot be swapped into Stack");
var render = new SizedBox().createRenderObject(null!); render.parentData = new FlexParentData();
Check(new Expanded(child: new SizedBox()).debugIsValidRenderObject(render), "Expanded parent-data accepts FlexParentData");
render.parentData = new StackParentData();
Check(!new Expanded(child: new SizedBox()).debugIsValidRenderObject(render), "Expanded rejects StackParentData");
var rootRender = new RenderProxyBox();
var buildOwner = new BuildOwner(focusManager: new FocusManager());
var tree = new RenderObjectToWidgetAdapter<RenderBox>(container: rootRender,
    child: new Directionality(textDirection: Doroti.Ui.TextDirection.ltr, child: new Row(children:
    [new Expanded(child: new Padding(padding: Doroti.Framework.Painting.EdgeInsets.CreateAll(8), child: new SizedBox(width: 20, height: 20))), new SizedBox(width: 10, height: 10)])));
var element = tree.attachToRenderTree(buildOwner);
rootRender.layout(Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: 200, height: 100));
Check(rootRender.child is RenderFlex flexRender && flexRender.firstChild!.parentData is FlexParentData { flex: 1 }, "mounted Row/Expanded/Padding tree lays out with real Flex parent data");
new RenderObjectToWidgetAdapter<RenderBox>(container: rootRender).attachToRenderTree(buildOwner, element);
buildOwner.buildScope(element); buildOwner.finalizeTree();
Check(rootRender.child is null, "mounted widget tree releases children");
var input = prefix + "public sealed class Hello : StatelessWidget { public string Title { get; } = \"hello\"; public override Widget build(BuildContext context) { string suffix = \"!\"; return new Text(Title + suffix); } }";
var extracted = Apply(input, Assist(input, input.IndexOf("new Text", StringComparison.Ordinal) + 5).Transform("extract", "Extracted"));
Compile(extracted);
Check(extracted.Contains("new Extracted(Title, suffix)") && extracted.Contains("_input0 + _input1"), "extraction passes member/local values at original call site");
var generic = prefix + "public class Hello<T>(T value) : StatelessWidget where T : class { public override Widget build(BuildContext context) => new Text(\"hello\"); }";
Compile(Apply(generic, Assist(generic, generic.IndexOf("new Text", StringComparison.Ordinal) + 5).Transform("extract", "GenericExtract")));
var stateful = Apply(input, Assist(input, input.IndexOf("class Hello", StringComparison.Ordinal) + 7).Transform("stateful", null));
Compile(stateful);
Check(stateful.Contains("currentWidget.Title") && stateful.Contains("string suffix"), "Stateful conversion preserves Widget property and build local");
Compile(Apply(generic, Assist(generic, generic.IndexOf("class Hello", StringComparison.Ordinal) + 7).Transform("stateful", null)));
var privateInput = input.Replace("public string Title", "private string Title");
Compile(Apply(privateInput, Assist(privateInput, privateInput.IndexOf("class Hello", StringComparison.Ordinal) + 7).Transform("stateful", null)));
var primary = prefix + "public sealed class Primary(string value) : StatelessWidget { public override Widget build(BuildContext c) => new Text(value); }";
Compile(Apply(primary, Assist(primary, primary.IndexOf("class Primary", StringComparison.Ordinal) + 7).Transform("stateful", null)));
var mutablePrimary = primary.Replace("public override Widget", "private void Change() { value = \"changed\"; } public override Widget");
Check(Assist(mutablePrimary, mutablePrimary.IndexOf("class Primary", StringComparison.Ordinal) + 7).Analyze(null).Actions.Any(a => a.Id == "stateful" && a.Reason is not null), "mutable primary capture has an explicit restriction");
var closure = prefix + "public sealed class Closures(string value) : StatelessWidget { private Widget Label() => new Text(value); public override Widget build(BuildContext context) => new GestureDetector(onTap: () => System.Console.WriteLine(value), child: Label()); }";
var convertedClosure = Apply(closure, Assist(closure, closure.IndexOf("class Closures", StringComparison.Ordinal) + 7).Transform("stateful", null));
Compile(convertedClosure);
Check(convertedClosure.Contains("var currentWidget = this.widget;") && convertedClosure.Contains("currentWidget.Label()"), "callbacks capture the build's Widget instance and retain private helper access");
var nullable = prefix + "public sealed class NullableInputs : StatelessWidget { public override Widget build(BuildContext c) { System.Action? callback = null; string? value = null; return new GestureDetector(onTap: callback, child: new Text(value ?? \"empty\")); } }";
var nullableExtract = Apply(nullable, Assist(nullable, nullable.IndexOf("new GestureDetector", StringComparison.Ordinal) + 6).Transform("extract", "NullableExtract"));
Compile(nullableExtract);
Check(nullableExtract.Contains("global::System.Action?") && nullableExtract.Contains("string?"), "nullable callback and local input contracts preserved");
var boxSource = prefix + "public sealed class BoxExample : StatelessWidget { public override Widget build(BuildContext context) { double width = 20; return new SizedBox(width: width, height: 10); } }";
var boxAssembly = Compile(Apply(boxSource, Assist(boxSource, boxSource.IndexOf("new SizedBox", StringComparison.Ordinal) + 6).Transform("extract", "ExtractedBox")));
var boxWidget = (Widget)Activator.CreateInstance(boxAssembly.GetType("Fixtures.BoxExample")!)!;
new RenderObjectToWidgetAdapter<RenderBox>(container: rootRender, child: boxWidget).attachToRenderTree(buildOwner, element);
buildOwner.buildScope(element); rootRender.layout(Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: 200, height: 100));
Check(rootRender.child is RenderConstrainedBox { additionalConstraints.minWidth: 20 }, "extracted widget mounts and retains its constructor input");
var boxStateAssembly = Compile(Apply(boxSource, Assist(boxSource, boxSource.IndexOf("class BoxExample", StringComparison.Ordinal) + 7).Transform("stateful", null)));
var stateWidget = (Widget)Activator.CreateInstance(boxStateAssembly.GetType("Fixtures.BoxExample")!)!;
new RenderObjectToWidgetAdapter<RenderBox>(container: rootRender, child: stateWidget).attachToRenderTree(buildOwner, element);
buildOwner.buildScope(element); rootRender.layout(Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: 200, height: 100));
Check(rootRender.child is RenderConstrainedBox { additionalConstraints.minWidth: 20 }, "converted Stateful widget mounts and builds through its State");
new RenderObjectToWidgetAdapter<RenderBox>(container: rootRender).attachToRenderTree(buildOwner, element);
buildOwner.buildScope(element); buildOwner.finalizeTree();
var removal = Apply(Apply(original, Assist(original, original.IndexOf("new Text", StringComparison.Ordinal) + 5).Transform("wrap:Padding", null)),
    Assist(Apply(original, Assist(original, original.IndexOf("new Text", StringComparison.Ordinal) + 5).Transform("wrap:Padding", null)), original.IndexOf("new Text", StringComparison.Ordinal) + 5).Transform("remove", null));
Compile(removal);
Console.WriteLine($"RESULT: {passed} checks passed");
