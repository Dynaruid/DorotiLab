using System.Reflection;
using System.Runtime.Loader;
using Doroti.DartToCSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var repo = Path.GetFullPath(args[0]);
var fixture = Path.Combine(repo, "tools/Doroti.DartToCSharp/validation/null-semantics/fixtures");
var output = Path.Combine(repo, ".doroti", "compiler-null-semantics");
var report = new DartCompiler().Compile(
    Path.Combine(fixture, "selection.json"),
    output,
    Path.Combine(repo, ".doroti/compiler-null-semantics-cache")
);
Require(
    report.Success,
    string.Join(Environment.NewLine, report.Diagnostics.Select(d => d.Message))
);

var generated = Directory.GetFiles(output, "*.g.cs", SearchOption.AllDirectories);
Require(generated.Length != 0, "The converter emitted no C# candidates.");
var source = string.Join(Environment.NewLine, generated.Select(File.ReadAllText));
Require(
    !source.Contains("RequireValue", StringComparison.Ordinal),
    "RequireValue escaped generation."
);
Require(
    !source.Contains("__dorotiNullAssert", StringComparison.Ordinal),
    "The internal null assertion placeholder escaped generation."
);
Require(
    source.Contains("Dart null assertion failed.", StringComparison.Ordinal),
    "Generated C# omitted the null assertion contract."
);

var trees = generated.Select(path =>
    CSharpSyntaxTree.ParseText(File.ReadAllText(path), path: path)
);
var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
    .Split(Path.PathSeparator)
    .Append(typeof(Doroti.Runtime.DartRuntimePrimitives).Assembly.Location)
    .Append(typeof(Doroti.Ui.Color).Assembly.Location)
    .Distinct();
var compilation = CSharpCompilation.Create(
    "CompilerNullFixture",
    trees,
    paths.Select(path => MetadataReference.CreateFromFile(path)),
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithWarningLevel(9999)
        .WithNullableContextOptions(NullableContextOptions.Enable)
);
using var stream = new MemoryStream();
var emit = compilation.Emit(stream);
Require(
    emit.Success,
    string.Join(
        Environment.NewLine,
        emit.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
    )
);
stream.Position = 0;
var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
var type = assembly.GetType("CompilerNullFixture.Framework.NullSemanticsProbe")!;
var instance = Activator.CreateInstance(type)!;

Require(Invoke("requiredInt", 0L).Equals(0L), "int zero must remain valid.");
Require(Invoke("requiredBool", false).Equals(false), "bool false must remain valid.");
Require(Invoke("requiredString", "present").Equals("present"), "reference value changed.");
Require(Invoke("requiredGeneric", "generic").Equals("generic"), "generic value changed.");
Require(Invoke("requiredDynamic", "dynamic").Equals("dynamic"), "dynamic value changed.");
Require(
    Invoke("defaulted", Type.Missing).Equals(7L),
    "an omitted optional argument must retain its Dart default."
);
var defaultBuilder = (Delegate)Invoke("defaultBuilder", Type.Missing);
Require(
    defaultBuilder.DynamicInvoke()!.Equals(7L),
    "a builder must capture the normalized optional default."
);
var delayed = (Delegate)Invoke("delayed", null);
try
{
    _ = delayed.DynamicInvoke();
    throw new InvalidOperationException("The delayed null assertion accepted null.");
}
catch (TargetInvocationException error)
    when (error.InnerException is NullReferenceException { Message: "Dart null assertion failed." })
{ }
ExpectNullAssertion("requiredInt", null);
ExpectNullAssertion("requiredString", null);
ExpectNullAssertion("requiredGeneric", null);
ExpectNullAssertion("requiredDynamic", null);

Console.WriteLine(
    "Compiler null semantics: PASS (analyzer, generation, C# compile, defaults, values, null failures)"
);
return;

object Invoke(string name, object? value)
{
    var method = type.GetMethods().Single(candidate => candidate.Name == name);
    if (method.IsGenericMethodDefinition)
    {
        method = method.MakeGenericMethod(typeof(string));
    }
    return method.Invoke(instance, [value])!;
}

void ExpectNullAssertion(string name, object? value)
{
    try
    {
        _ = Invoke(name, value);
        throw new InvalidOperationException($"{name} accepted null.");
    }
    catch (TargetInvocationException error)
        when (error.InnerException
                is NullReferenceException { Message: "Dart null assertion failed." }
        ) { }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
