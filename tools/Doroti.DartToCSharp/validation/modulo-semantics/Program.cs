using System.Reflection;
using System.Runtime.Loader;
using Doroti.DartToCSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var repo = Path.GetFullPath(args[0]);
var fixture = Path.Combine(repo, "tools/Doroti.DartToCSharp/validation/modulo-semantics/fixtures");
var output = Path.Combine(repo, ".doroti", "compiler-modulo-semantics");
var report = new DartCompiler().Compile(
    Path.Combine(fixture, "selection.json"),
    output,
    Path.Combine(repo, ".doroti/compiler-modulo-semantics-cache")
);
Require(
    report.Success,
    string.Join(Environment.NewLine, report.Diagnostics.Select(d => d.Message))
);

var generated = Directory.GetFiles(output, "*.g.cs", SearchOption.AllDirectories);
Require(generated.Length != 0, "The converter emitted no C# candidates.");
var source = string.Join(Environment.NewLine, generated.Select(File.ReadAllText));
Require(
    !CSharpSyntaxTree
        .ParseText(source)
        .GetRoot()
        .DescendantNodes()
        .Any(node =>
            node.IsKind(SyntaxKind.ModuloExpression)
            || node.IsKind(SyntaxKind.ModuloAssignmentExpression)
        ),
    "A raw C# numeric remainder escaped modulo lowering."
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
    "CompilerModuloFixture",
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
var type = assembly.GetType("CompilerModuloFixture.Framework.ModuloProbe")!;
var instance = Activator.CreateInstance(type)!;

var integerPairs = new (long A, long B)[]
{
    (-5, 3),
    (-5, -3),
    (5, -3),
    (5, 3),
    (-6, 3),
    (0, -3),
    (long.MinValue, -1),
    (long.MinValue, 3),
    (long.MinValue, long.MinValue),
    (-1, long.MinValue),
    (long.MaxValue, long.MinValue),
    (long.MaxValue, 97),
};
foreach (var (a, b) in integerPairs)
{
    var expectedBig = (System.Numerics.BigInteger)a % b;
    if (expectedBig < 0)
        expectedBig += System.Numerics.BigInteger.Abs(b);
    var expected = (long)expectedBig;
    Require(Doroti.Runtime.DartNumeric.Modulo(a, b) == expected, $"runtime integer {a} % {b}");
    Require(Invoke("integer", a, b).Equals(expected), $"generated integer {a} % {b}");
    Require(Invoke("compound", a, b).Equals(expected), $"generated compound {a} %= {b}");
}
try
{
    Doroti.Runtime.DartNumeric.Modulo(1L, 0L);
    throw new Exception("zero divisor accepted");
}
catch (DivideByZeroException) { }
Require(Doroti.Runtime.DartNumeric.Modulo(int.MinValue, -1) == 0, "Int32 minimum");

// Execute the installed Dart SDK as the oracle, including its IEEE-754 edge cases.
var dart =
    args.Length > 1 ? args[1]
    : OperatingSystem.IsWindows()
        ? Environment
            .GetEnvironmentVariable("PATH")!
            .Split(Path.PathSeparator)
            .Select(directory => Path.Combine(directory, "cache/dart-sdk/bin/dart.exe"))
            .First(File.Exists)
    : "dart";
var start = new System.Diagnostics.ProcessStartInfo(dart)
{
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
};
start.ArgumentList.Add(Path.Combine(fixture, "oracle.dart"));
using var process = System.Diagnostics.Process.Start(start)!;
var oracleText = process.StandardOutput.ReadToEnd();
var oracleError = process.StandardError.ReadToEnd();
process.WaitForExit();
Require(process.ExitCode == 0, oracleError);
using var oracle = System.Text.Json.JsonDocument.Parse(oracleText);
var floatingPairs = new (double A, double B)[]
{
    (-5, 3),
    (-5, -3),
    (5, -3),
    (-6, 3),
    (-0.0, 3),
    (5.5, 3),
    (-5.5, 3),
    (-5.5, -3),
    (5, 0),
    (5, -0.0),
    (double.PositiveInfinity, 3),
    (double.NegativeInfinity, 3),
    (5, double.PositiveInfinity),
    (-5, double.PositiveInfinity),
    (-5, double.NegativeInfinity),
    (double.NaN, 3),
    (3, double.NaN),
    (-double.Epsilon, 3),
};
for (var i = 0; i < floatingPairs.Length; i++)
{
    var (a, b) = floatingPairs[i];
    var expected = double.Parse(
        oracle.RootElement.GetProperty("double")[i].GetString()!,
        System.Globalization.CultureInfo.InvariantCulture
    );
    CheckDouble(Doroti.Runtime.DartNumeric.Modulo(a, b), expected, "runtime floating modulo");
    CheckDouble((double)Invoke("floating", a, b), expected, "generated floating modulo");
}
Require(Invoke("mixed", -5L, 3d).Equals(1d), "mixed int/double");
Require(Invoke("number", -5d, 3d).Equals(1d), "num modulo");
Require(
    Invoke("dynamicNumber", -5L, 3L) is long dynamicInteger && dynamicInteger == 1,
    "dynamic modulo preserves int"
);
Require(Invoke("dynamicNumber", -5.5, 3L).Equals(0.5), "dynamic mixed modulo");
Require(
    Doroti.Runtime.DartNumeric.Modulo((object)(-5L), (object)3L) is long,
    "boxed modulo preserves int"
);
Require(Invoke("compoundValue", -5L, 3L).Equals(11L), "compound expression yields stored value");
Require(Invoke("nested", -5L, 8L, 3L).Equals(1L), "nested modulo");
Require(Invoke("constant").Equals(1L), "constant modulo");
Require(Invoke("staticCompound").Equals(1L), "static assignment");
Require(Invoke("cascade").Equals(1L), "cascade assignment");
Require(Invoke("cascadeIndex").Equals(1L), "cascade index assignment");
Require(Invoke("dynamicCustom").Equals(101L), "dynamic custom operator");
Require(Invoke("dynamicCustomCompound").Equals(101L), "dynamic custom compound operator");
Require(
    ((Doroti.Runtime.Future<long>)Invoke("asyncCompound")).GetAwaiter().GetResult() == 1L,
    "awaited compound assignment"
);
Require(
    type.GetMethod("nullableProperty")!.Invoke(instance, new object?[] { null }) is null,
    "null receiver skips assignment"
);
Require(
    Invoke(
            "nullableProperty",
            Activator.CreateInstance(
                assembly.GetType("CompilerModuloFixture.Framework.ModuloCell")!
            )!
        )
        .Equals(1L),
    "nullable property assignment"
);
foreach (var method in new[] { "indexed", "property", "custom", "customCompound" })
    Require(
        Invoke(method).Equals(oracle.RootElement.GetProperty(method).GetInt64()),
        method + " matches Dart"
    );
Console.WriteLine(
    "PASS modulo: Dart oracle, integer limits, floating edges, dynamic, operators and single-evaluation assignments"
);

object Invoke(string name, params object[] values) =>
    type.GetMethod(name)!.Invoke(instance, values)!;
static void Require(bool value, string message)
{
    if (!value)
        throw new InvalidOperationException(message);
}
static void CheckDouble(double actual, double expected, string label) =>
    Require(
        double.IsNaN(expected)
            ? double.IsNaN(actual)
            : BitConverter.DoubleToInt64Bits(actual) == BitConverter.DoubleToInt64Bits(expected),
        $"{label}: {actual} != {expected}"
    );
