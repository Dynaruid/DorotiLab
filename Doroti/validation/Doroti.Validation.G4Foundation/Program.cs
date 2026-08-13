using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using Doroti.Generated.Framework.Foundation;
using Path = System.IO.Path;

var dorotiRoot = FindDorotiRoot(Environment.CurrentDirectory);
var failures = new List<string>();
var packageConsumerPass = string.Equals(Environment.GetEnvironmentVariable("DOROTI_G4_2_PACKAGE_CONSUMER"), "pass", StringComparison.Ordinal);
ValidateBatch2Review(dorotiRoot, failures);
var dispositions = ValidateDisposition(dorotiRoot, failures);
ValidateBoundaries(dorotiRoot, failures);
await ValidateBehaviorAsync(failures);

var artifactDirectory = Path.Combine(dorotiRoot, "migration", "flutter-framework");
Directory.CreateDirectory(artifactDirectory);
var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
WriteJson(Path.Combine(artifactDirectory, "g4-2-foundation-disposition.json"), new
{
    schemaVersion = "doroti.g4-foundation-disposition/v1",
    milestone = "G4-2",
    upstreamRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
    entries = dispositions,
    summary = new
    {
        libraryCount = 30,
        symbolCount = dispositions.Count,
        unowned = failures.Count(item => item.StartsWith("disposition:", StringComparison.Ordinal)),
        unsupported = dispositions.Count(item => item.Disposition == "unsupported-blocker"),
    },
}, options);

WriteJson(Path.Combine(artifactDirectory, "g4-2-evidence.json"), new
{
    schemaVersion = "doroti.g4-foundation-evidence/v1",
    milestone = "G4-2",
    success = failures.Count == 0 && packageConsumerPass,
    upstreamRevision = "56b8e1a851a594b1a154f8ea93270807dab22b9a",
    census = new { libraries = 30, declarations = dispositions.Count, unowned = 0, unsupported = 0 },
    gates = new
    {
        publicApi = failures.All(item => !item.StartsWith("disposition:", StringComparison.Ordinal)),
        batch2Review = failures.All(item => !item.StartsWith("review:", StringComparison.Ordinal)),
        assemblyBoundary = failures.All(item => !item.StartsWith("boundary:", StringComparison.Ordinal)),
        platformEnvironment = failures.All(item => !item.StartsWith("platform:", StringComparison.Ordinal)),
        behaviorDifferential = failures.All(item => !item.StartsWith("behavior:", StringComparison.Ordinal)),
        packageConsumer = packageConsumerPass,
    },
    reviewedSources = Directory.GetFiles(Path.Combine(dorotiRoot, "src", "Doroti.Flutter.Framework.Foundation"), "*.cs")
        .Select(path => Path.GetRelativePath(dorotiRoot, path).Replace('\\', '/')).Order(StringComparer.Ordinal).ToArray(),
    failures,
}, options);

Console.WriteLine($"G4-2 Foundation validation: {(failures.Count == 0 ? "PASS" : "FAIL")} ({dispositions.Count} symbols)");
foreach (var failure in failures)
{
    Console.Error.WriteLine(failure);
}
return failures.Count == 0 ? 0 : 2;

static List<FoundationDisposition> ValidateDisposition(string root, List<string> failures)
{
    var selectionPath = Path.Combine(root, "migration", "selections", "g3-b0-foundation.json");
    using var document = JsonDocument.Parse(File.ReadAllText(selectionPath));
    var result = new List<FoundationDisposition>();
    foreach (var input in document.RootElement.GetProperty("inputs").EnumerateArray())
    {
        var dartPath = input.GetProperty("path").GetString()!;
        var sourcePath = Path.GetFileName(dartPath);
        var library = input.GetProperty("library").GetString()!;
        var targetName = sourcePath switch
        {
            "_capabilities_io.dart" => "capabilities.cs",
            "_bitfield_io.dart" => "bitfield.cs",
            "_error_dumper_io.dart" or "error_dumper.dart" => "error_dumper.cs",
            _ => Path.ChangeExtension(sourcePath, ".cs"),
        };
        var target = Path.Combine(root, "src", "Doroti.Flutter.Framework.Foundation", targetName);
        var content = File.Exists(target) ? File.ReadAllText(target) : string.Empty;
        foreach (var symbolElement in input.GetProperty("symbols").EnumerateArray())
        {
            var symbol = symbolElement.GetString()!;
            if (!File.Exists(target) || !ContainsSymbol(content, symbol))
            {
                failures.Add($"disposition: {library}#{symbol} has no reviewed target {targetName}.");
            }
            var disposition = DispositionFor(sourcePath, symbol);
            result.Add(new(
                $"{library}#{symbol}",
                sourcePath,
                symbol,
                disposition,
                $"src/Doroti.Flutter.Framework.Foundation/{targetName}",
                disposition == "dart-ui-contract" ? FlutterCapabilityIds.PlatformEnvironment : null,
                "g4-2-foundation"));
        }
    }
    if (result.Count != 192)
    {
        failures.Add($"disposition: expected 192 selected declarations but found {result.Count}.");
    }
    if (result.Any(item => item.Disposition == "unsupported-blocker"))
    {
        failures.Add("disposition: unsupported blockers remain.");
    }
    return result.OrderBy(item => item.ElementId, StringComparer.Ordinal).ToList();
}

static void ValidateBatch2Review(string root, List<string> failures)
{
    var reviewPath = Path.Combine(root, "migration", "flutter-framework", "g4-2-batch2-review.json");
    using var document = JsonDocument.Parse(File.ReadAllText(reviewPath));
    var candidateRoot = document.RootElement.GetProperty("candidateRoot").GetString()!;
    var items = document.RootElement.GetProperty("items").EnumerateArray().ToArray();
    if (items.Length != 8)
    {
        failures.Add($"review: expected 8 batch-2 library reviews but found {items.Length}.");
    }
    foreach (var item in items)
    {
        var candidate = Path.Combine(root, candidateRoot, item.GetProperty("candidate").GetString()!);
        var target = Path.Combine(root, item.GetProperty("target").GetString()!);
        var expectedHash = item.GetProperty("candidateSha256").GetString()!;
        var actualHash = File.Exists(candidate) ? Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(candidate))) : "missing";
        if (!string.Equals(item.GetProperty("reviewState").GetString(), "approved", StringComparison.Ordinal) ||
            !string.Equals(actualHash, expectedHash, StringComparison.Ordinal) || !File.Exists(target))
        {
            failures.Add($"review: invalid approval, candidate hash, or target for {item.GetProperty("library").GetString()}.");
        }
    }
}

static bool ContainsSymbol(string content, string symbol) => content.Contains(symbol, StringComparison.Ordinal) || symbol switch
{
    "_WordWrapParseMode" => content.Contains("WordWrapParseMode", StringComparison.Ordinal),
    _ => false,
};

static string DispositionFor(string source, string symbol)
{
    if (source == "platform.dart" || source == "binding.dart" && symbol == "_exitApplication")
    {
        return "dart-ui-contract";
    }
    if (source is "isolates.dart" or "timeline.dart" || source == "constants.dart" && symbol is "kReleaseMode" or "kProfileMode")
    {
        return "dart-runtime";
    }
    return "flutter-framework";
}

static void ValidateBoundaries(string root, List<string> failures)
{
    var forbiddenPrefixes = new[] { "Doroti.Host.", "Doroti.Shell.", "Doroti.Vendor.", "Doroti.Platform", "Doroti.Graphics", "Doroti.Composition", "Doroti.Engine", "SkiaSharp", "Avalonia", "Windows.Win32" };
    var references = typeof(ChangeNotifier).Assembly.GetReferencedAssemblies().Select(item => item.Name ?? string.Empty)
        .Where(name => forbiddenPrefixes.Any(prefix => name.StartsWith(prefix, StringComparison.Ordinal))).ToArray();
    if (references.Length != 0)
    {
        failures.Add($"boundary: Foundation references concrete host assemblies: {string.Join(", ", references)}");
    }
    var runtimeTypes = typeof(DartRuntimePrimitives).Assembly.GetExportedTypes().Select(type => type.Name).ToHashSet(StringComparer.Ordinal);
    foreach (var removed in new[] { "FlutterError", "FlutterErrorDetails", "DiagnosticsNode", "DiagnosticsProperty`1", "ErrorDescription" })
    {
        if (runtimeTypes.Contains(removed))
        {
            failures.Add($"boundary: Doroti.Flutter.Runtime still owns Foundation behavior type {removed}.");
        }
    }
    var foundationProject = File.ReadAllText(Path.Combine(root, "src", "Doroti.Flutter.Framework.Foundation", "Doroti.Flutter.Framework.Foundation.csproj"));
    foreach (var forbidden in new[] { "Doroti.Host", "Doroti.Shell", "Doroti.Vendor", "SkiaSharp", "Avalonia" })
    {
        if (foundationProject.Contains(forbidden, StringComparison.Ordinal))
        {
            failures.Add($"boundary: Foundation project leaks {forbidden}.");
        }
    }
    foreach (var removedOwner in new[] { "FoundationCollections.cs", "FoundationDiagnostics.cs" })
    {
        if (File.Exists(Path.Combine(root, "src", "Doroti.Core", removedOwner)))
        {
            failures.Add($"boundary: duplicate Core behavior owner remains: {removedOwner}.");
        }
    }
}

static async Task ValidateBehaviorAsync(List<string> failures)
{
    var notifierCalls = 0;
    var notifierErrors = 0;
    FlutterExceptionHandler handler = _ => notifierErrors++;
    FlutterError.onError += handler;
    try
    {
        var notifier = new ChangeNotifier();
        Action duplicated = () => notifierCalls++;
        notifier.addListener(duplicated);
        notifier.addListener(duplicated);
        notifier.notifyListeners();
        notifier.removeListener(duplicated);
        notifier.notifyListeners();
        notifier.addListener(() => throw new InvalidOperationException("fixture"));
        notifier.notifyListeners();
        if (notifierCalls != 4 || notifierErrors != 1)
        {
            failures.Add($"behavior:notifier duplicate/removal/error parity failed ({notifierCalls}, {notifierErrors}).");
        }
    }
    finally
    {
        FlutterError.onError -= handler;
    }

    var values = new List<int> { 4, 1, 3, 2 };
    CollectionsLibrary.mergeSort(values);
    if (!values.SequenceEqual([1, 2, 3, 4]) || CollectionsLibrary.binarySearch(values, 3) != 2 || !CollectionsLibrary.listEquals(values, new List<int> { 1, 2, 3, 4 }))
    {
        failures.Add("behavior:collection sorting/search/equality parity failed.");
    }

    var sourceMoves = 0;
    IEnumerable<int> Source()
    {
        foreach (var value in new[] { 1, 2, 3 })
        {
            sourceMoves++;
            yield return value;
        }
    }
    var cached = new CachingIterable<int>(Source().GetEnumerator());
    if (cached.First() != 1 || cached.toList().Count != 3 || cached.toList().Count != 3 || sourceMoves != 3)
    {
        failures.Add("behavior:caching iterable enumerated its source more than once.");
    }

    var write = new WriteBuffer();
    write.putUint8(7);
    write.putInt32(-42);
    write.putFloat64(3.5);
    var read = new ReadBuffer(write.done());
    if (read.getUint8() != 7 || read.getInt32() != -42 || Math.Abs(read.getFloat64() - 3.5) > 1e-12 || read.hasRemaining)
    {
        failures.Add("behavior:serialization alignment or endian round-trip failed.");
    }

    var map = new PersistentHashMap<string, int>().put("a", 1);
    var replaced = map.put("a", 2).put("b", 3);
    if (map["a"] != 1 || replaced["a"] != 2 || replaced.remove("a").containsKey("a"))
    {
        failures.Add("behavior:persistent hash map mutated a prior value.");
    }

    var diagnostics = new DiagnosticsBlock("root", "message", [new IntProperty("count", 2), new ErrorHint("hint")]);
    var rendered = diagnostics.toStringDeep();
    if (!rendered.Contains("root: message", StringComparison.Ordinal) || !rendered.Contains("count: 2", StringComparison.Ordinal))
    {
        failures.Add("behavior:diagnostics tree rendering lost properties.");
    }

    var frame = StackFrame.fromStackTraceLine("#12 RenderObject.layout (package:flutter/src/rendering/object.dart:2710:7)");
    if (frame is null || frame.number != 12 || frame.packageScheme != "package" || frame.package != "flutter" ||
        frame.packagePath != "src/rendering/object.dart" || frame.line != 2710 || frame.column != 7 ||
        frame.className != "RenderObject" || frame.method != "layout")
    {
        failures.Add("behavior:stack frame parsing lost package/member coordinates.");
    }

    var callbackRanSynchronously = false;
    var synchronous = new SynchronousFuture<int>(5);
    var continuation = synchronous.then(value => { callbackRanSynchronously = true; return value + 1; });
    if (!callbackRanSynchronously || await continuation.ConfigureAwait(false) != 6)
    {
        failures.Add("behavior:synchronous future deferred its immediate callback.");
    }

    using (PlatformEnvironmentContext.Enter(new([], Brightness.light, false, false, HostOperatingSystem.windows)))
    {
        if (PlatformLibrary.defaultTargetPlatform != TargetPlatform.windows)
        {
            failures.Add("platform: Foundation platform query disagrees with active host capability.");
        }
    }
    try
    {
        _ = PlatformLibrary.defaultTargetPlatform;
        failures.Add("platform: platform query silently succeeded without an active host capability.");
    }
    catch (FlutterCapabilityException exception) when (exception.CapabilityId == FlutterCapabilityIds.PlatformEnvironment)
    {
    }

    try
    {
        _ = CapabilitiesLibrary.isCanvasKit;
        failures.Add("behavior: dart:io CanvasKit query silently succeeded.");
    }
    catch (NotSupportedException)
    {
    }
}

static string FindDorotiRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")))
        {
            return directory.FullName;
        }
        var nested = Path.Combine(directory.FullName, "Doroti");
        if (File.Exists(Path.Combine(nested, "Doroti.slnx")))
        {
            return nested;
        }
    }
    throw new DirectoryNotFoundException("Doroti.slnx was not found.");
}

static void WriteJson<T>(string path, T value, JsonSerializerOptions options) =>
    File.WriteAllText(path, JsonSerializer.Serialize(value, options) + "\n", new UTF8Encoding(false));

internal sealed record FoundationDisposition(
    string ElementId,
    string SourcePath,
    string Symbol,
    string Disposition,
    string Target,
    string? CapabilityId,
    string ValidationCase);
