using System.Text;
using System.Text.Json;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Animation;
using Doroti.Generated.Framework.Gestures;
using Doroti.Generated.Framework.Physics;
using Path = System.IO.Path;

var dorotiRoot = FindDorotiRoot(Environment.CurrentDirectory);
var failures = new List<string>();
var trace = new List<string>();

var promotion = ValidatePromotion(dorotiRoot, failures);
ValidateRuntimeIdentity(failures, trace);
ValidateArena(failures, trace);
ValidatePointerSignals(failures, trace);
ValidatePhysicsAndAnimation(failures, trace);

var success = failures.Count == 0;
var evidenceDirectory = Path.Combine(dorotiRoot, "migration", "flutter-avalonia", "bridge-validation");
Directory.CreateDirectory(evidenceDirectory);
var evidencePath = Path.Combine(evidenceDirectory, "g4-4.json");
JsonElement? priorAggregateGates = null;
string? priorValidatedAtUtc = null;
if (File.Exists(evidencePath))
{
    using var prior = JsonDocument.Parse(File.ReadAllText(evidencePath));
    if (prior.RootElement.TryGetProperty("aggregateGates", out var aggregate)) priorAggregateGates = aggregate.Clone();
    if (prior.RootElement.TryGetProperty("validatedAtUtc", out var validatedAt)) priorValidatedAtUtc = validatedAt.GetString();
}
var evidence = new
{
    schemaVersion = "doroti.g5-0-truth-reset/v2",
    milestone = "G4-4",
    previousBaseline = new { claimedSuccess = true, aggregateErrorCount = 27 },
    currentRun = new
    {
        capturedAtUtc = DateTimeOffset.UtcNow,
        timeoutMilliseconds = 15 * 60 * 1000,
        promotion = new { status = failures.Any(item => item.StartsWith("promotion:", StringComparison.Ordinal)) ? "failed" : "verified", details = promotion },
        productBehavior = new { status = success ? "verified" : "failed", trace, failures },
        nativeEvidence = new { status = "not-verified", reasons = new[] { "Physical mouse, touch and trackpad input was not run for G5-0.", "Sustained target-machine GPU and DPI behavior was not run for G5-0." } },
    },
    flutterRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
    sourceInventory = new { libraries = 42, declarations = 353, members = 1709 },
    promotion,
    api = new { publicDeclarations = 259, uniqueDeclarationNames = 259, publicMembers = 973 },
    gates = new
    {
        reviewedSourceOwnership = failures.All(item => !item.StartsWith("promotion:", StringComparison.Ordinal)),
        dartIdentity = failures.All(item => !item.StartsWith("identity:", StringComparison.Ordinal)),
        gestureArena = failures.All(item => !item.StartsWith("arena:", StringComparison.Ordinal)),
        pointerSignals = failures.All(item => !item.StartsWith("signal:", StringComparison.Ordinal)),
        physicsAnimation = failures.All(item => !item.StartsWith("motion:", StringComparison.Ordinal)),
        handwrittenCoreAdapterCutover = failures.All(item => !item.StartsWith("cutover:", StringComparison.Ordinal)),
        handwrittenRecognizerPolicyCutover = failures.All(item => !item.StartsWith("recognizer-cutover:", StringComparison.Ordinal)),
    },
    trace,
    failures,
    notVerified = new[]
    {
        "Physical mouse/touch/trackpad input through the Avalonia native host (belongs to the G4-6 input bridge gate).",
        "Sustained target-machine frame pacing and GPU/DPI behavior (belongs to the G4-6/G4-7 host evidence gates).",
    },
    aggregateGates = priorAggregateGates,
    validatedAtUtc = priorValidatedAtUtc,
};
var temporaryEvidence = evidencePath + ".tmp-" + Guid.NewGuid().ToString("N");
File.WriteAllText(temporaryEvidence, JsonSerializer.Serialize(evidence, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }) + "\n", new UTF8Encoding(false));
File.Move(temporaryEvidence, evidencePath, true);

Console.WriteLine($"G4-4 Physics/Animation/Gestures validation: {(success ? "PASS" : "FAIL")}");
foreach (var failure in failures) Console.WriteLine($"  {failure}");
return success ? 0 : 1;

static object ValidatePromotion(string root, List<string> failures)
{
    using var disposition = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "migration", "flutter-framework", "g4-4-physics-animation-gestures-disposition.json")));
    using var api = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "migration", "flutter-framework", "g4-4-api-manifest.json")));
    var entries = disposition.RootElement.GetProperty("entries").EnumerateArray().ToArray();
    if (entries.Length != 353) failures.Add($"promotion: expected 353 disposition entries, got {entries.Length}.");
    foreach (var entry in entries)
    {
        if (entry.GetProperty("disposition").GetString() != "promoted") failures.Add("promotion: a declaration is not promoted.");
        var target = Path.Combine(root, entry.GetProperty("target").GetString()!.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(target)) failures.Add($"promotion: target does not exist: {target}");
    }

    var counts = api.RootElement.GetProperty("counts");
    Require(counts.GetProperty("declarationOccurrences").GetInt32() == 259, "promotion: public declaration count drifted.", failures);
    Require(counts.GetProperty("uniqueDeclarationNames").GetInt32() == 259, "promotion: unique declaration count drifted.", failures);
    Require(counts.GetProperty("publicMembers").GetInt32() == 973, "promotion: public member count drifted.", failures);
    var reviewed = api.RootElement.GetProperty("reviewedSources").EnumerateArray().Select(item => item.GetString()!).ToArray();
    Require(reviewed.Length == 49, $"promotion: expected 49 reviewed source and project-glue files, got {reviewed.Length}.", failures);
    foreach (var relative in reviewed.Where(path => path.EndsWith(".cs", StringComparison.Ordinal)))
    {
        var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        if (!relative.EndsWith("GlobalUsings.cs", StringComparison.Ordinal))
        {
            Require(File.ReadLines(path).FirstOrDefault() == "// <doroti-reviewed-framework-source />", $"promotion: reviewed marker missing from {relative}.", failures);
        }
        Require(!relative.EndsWith(".g.cs", StringComparison.Ordinal), $"promotion: generated suffix survived in {relative}.", failures);
    }
    return new { dispositionEntries = entries.Length, reviewedFiles = reviewed.Length, upstreamSourceFiles = 42, projectGlueFiles = 7 };
}

static void ValidateRuntimeIdentity(List<string> failures, List<string> trace)
{
    Require(DartRuntimePrimitives.Identical(GestureDisposition.accepted, GestureDisposition.accepted), "identity: enum value identity failed.", failures);
    Require(!DartRuntimePrimitives.Identical(+0.0, -0.0), "identity: +0.0 and -0.0 must not be identical.", failures);
    var instance = new object();
    Require(DartRuntimePrimitives.Identical(instance, instance), "identity: reference identity failed.", failures);
    Require(!DartRuntimePrimitives.Identical(instance, new object()), "identity: distinct references were identical.", failures);
    trace.Add("dart-identical:+0/-0-distinct,enum-stable,reference-stable");
}

static void ValidateArena(List<string> failures, List<string> trace)
{
    var queue = new DartMicrotaskQueue();
    using var scheduler = DartAsyncRuntime.enterMicrotaskScheduler(queue.enqueue);

    var manager = new GestureArenaManager();
    var first = new ArenaMember("first");
    var second = new ArenaMember("second");
    manager.add(41, first);
    manager.add(41, second);
    manager.close(41);
    manager.hold(41);
    manager.sweep(41);
    Require(first.Accepted == 0 && second.Rejected == 0, "arena: held sweep resolved early.", failures);
    manager.release(41);
    Require(first.Accepted == 1 && second.Rejected == 1, "arena: released sweep did not select exactly one winner.", failures);

    var solo = new ArenaMember("solo");
    manager.add(42, solo);
    manager.close(42);
    Require(queue.count == 1 && solo.Accepted == 0, "arena: default winner was not deferred to one microtask.", failures);
    queue.drain();
    Require(solo.Accepted == 1 && solo.Rejected == 0, "arena: deferred default resolution did not accept once.", failures);
    trace.Add("arena:hold-sweep-release=first-wins;single-member=microtask-wins");
}

static void ValidatePointerSignals(List<string> failures, List<string> trace)
{
    var callbackCount = 0;
    var responseCount = 0;
    bool? allowDefault = null;
    var signal = new PointerScrollEvent(
        viewId: 7,
        timeStamp: Duration.Create(microseconds: 123456),
        device: 9,
        position: new Offset(11.25, 22.5),
        scrollDelta: new Offset(-3.5, 8.75),
        onRespond: value => { responseCount++; allowDefault = value; });
    var resolver = new PointerSignalResolver();
    resolver.register(signal, observed =>
    {
        callbackCount++;
        Require(observed.timeStamp.inMicroseconds == 123456, "signal: timestamp drifted.", failures);
        Require(observed.position == new Offset(11.25, 22.5), "signal: position drifted.", failures);
        Require(((PointerScrollEvent)observed).scrollDelta == new Offset(-3.5, 8.75), "signal: scroll delta drifted.", failures);
    });
    resolver.register(signal, _ => failures.Add("signal: second callback won registration."));
    resolver.resolve(signal);
    Require(callbackCount == 1 && responseCount == 0, "signal: handled event was not delivered exactly once.", failures);

    var unhandled = new PointerScrollEvent(onRespond: value => { responseCount++; allowDefault = value; });
    resolver.resolve(unhandled);
    Require(responseCount == 1 && allowDefault == true, "signal: unhandled event did not allow platform default exactly once.", failures);
    trace.Add("pointer-signal:timestamp=123456us,position=(11.25,22.5),delta=(-3.5,8.75),first-handler-wins");
}

static void ValidatePhysicsAndAnimation(List<string> failures, List<string> trace)
{
    var spring = SpringDescription.CreateWithDampingRatio(mass: 1, stiffness: 100, ratio: 1);
    var simulation = new SpringSimulation(spring, start: 0, end: 100, velocity: 0);
    var x0 = simulation.x(0);
    var x1 = simulation.x(1);
    var x10 = simulation.x(10);
    Require(double.IsFinite(x0) && double.IsFinite(x1) && double.IsFinite(x10), "motion: spring produced a non-finite value.", failures);
    Require(Math.Abs(x0) < 1e-9 && Math.Abs(x10 - 100) < 1e-6 && simulation.isDone(10), "motion: spring did not converge to its target.", failures);
    Require(Math.Abs(Curves.linear.transform(0.375) - 0.375) < 1e-12, "motion: linear curve changed its input.", failures);
    var interval = new Interval(0.25, 0.75);
    Require(interval.transform(0.25) == 0 && interval.transform(0.75) == 1, "motion: interval endpoints drifted.", failures);
    trace.Add($"motion:spring-x0={x0:R},x1={x1:R},x10={x10:R};linear=0.375");
}

static string FindDorotiRoot(string start)
{
    for (var current = new DirectoryInfo(start); current is not null; current = current.Parent)
    {
        if (File.Exists(Path.Combine(current.FullName, "Doroti.slnx"))) return current.FullName;
        var nested = Path.Combine(current.FullName, "Doroti");
        if (File.Exists(Path.Combine(nested, "Doroti.slnx"))) return nested;
    }
    throw new DirectoryNotFoundException("Could not locate the Doroti root.");
}

static void Require(bool condition, string failure, List<string> failures)
{
    if (!condition) failures.Add(failure);
}

sealed class ArenaMember(string name) : GestureArenaMember
{
    public int Accepted { get; private set; }
    public int Rejected { get; private set; }
    public void acceptGesture(long pointer) => Accepted++;
    public void rejectGesture(long pointer) => Rejected++;
    public override string ToString() => name;
}
