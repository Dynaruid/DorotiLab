using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Doroti.DartToCSharp;
using Doroti.SourceTools;
using Doroti.Tooling;

const string LockSchema = "doroti.flutter-source-census/v1";
const string ReportSchema = "doroti.g3-1-validation/v1";
var repositoryRoot = FindRepositoryRoot(Environment.CurrentDirectory);
var dorotiRoot = Path.Combine(repositoryRoot, "Doroti");
var failures = new List<string>();
CompilerRefactorValidation.Validate(repositoryRoot, dorotiRoot, failures);
if (args.Contains("--refactor-only", StringComparer.Ordinal))
{
    Console.WriteLine($"Compiler refactor validation: {(failures.Count == 0 ? "PASS" : "FAIL")}");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine(failure);
    }
    return failures.Count == 0 ? 0 : 2;
}

var census = ValidateCensus(repositoryRoot, dorotiRoot, failures);
var activeGraph = ValidateActiveGraph(repositoryRoot, dorotiRoot, failures);
var truthReset = ValidateFrameworkTruthReset(dorotiRoot, failures);
CompilerNegativeResult negative;
MechanicalCandidateResult candidate;
FrameworkGraphResult frameworkGraph;
G3PromotionResult promotion;
try
{
    candidate = await Task.Run(() => ValidateMechanicalCandidate(dorotiRoot, failures))
        .WaitAsync(TimeSpan.FromMinutes(15));
    negative = await Task.Run(() => ValidateCompilerNegative(dorotiRoot, failures))
        .WaitAsync(TimeSpan.FromMinutes(15));
    frameworkGraph = await Task.Run(() => ValidateFrameworkGraph(repositoryRoot, dorotiRoot, failures))
        .WaitAsync(TimeSpan.FromMinutes(15));
    promotion = await Task.Run(() => G3PromotionValidation.Validate(dorotiRoot, failures))
        .WaitAsync(TimeSpan.FromMinutes(15));
}
catch (TimeoutException)
{
    failures.Add("Compiler validation exceeded the repository 15-minute timeout.");
    candidate = new(false, null, null, null);
    negative = new(false, null, null, null, null);
    frameworkGraph = new(false, 0, 0, 0, 0, false, false, false, null);
    promotion = new(false, 0, 0, 0, false, false, false, false, false, false);
}

var report = new ValidationReport(
    ReportSchema,
    failures.Count == 0,
    census,
    activeGraph,
    truthReset,
    candidate,
    negative,
    frameworkGraph,
    promotion,
    failures.ToArray());
var artifactDirectory = Path.Combine(dorotiRoot, "artifacts", "validation");
Directory.CreateDirectory(artifactDirectory);
var artifactPath = Path.Combine(artifactDirectory, "g3-1-compiler-gate.json");
File.WriteAllText(
    artifactPath,
    JsonSerializer.Serialize(report, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }) + "\n",
    new UTF8Encoding(false));

Console.WriteLine($"G3-1 validation: {(report.Success ? "PASS" : "FAIL")}");
Console.WriteLine($"Flutter source: {census.PublicRootCount} roots, {census.ExportDirectiveCount} exports, {census.DartFileCount} Dart files");
Console.WriteLine($"G3-0 evidence: {(truthReset.Success ? "PASS" : "FAIL")} ({truthReset.TotalImplementationBlockers} implementation blockers reported)");
Console.WriteLine($"F0 candidate: {(candidate.Success ? "PASS" : "FAIL")} ({candidate.OutputSha256 ?? "no output"})");
Console.WriteLine($"Compiler negative: {(negative.Success ? "PASS" : "FAIL")} ({negative.DiagnosticCode ?? "no diagnostic"})");
Console.WriteLine($"Framework graph: {(frameworkGraph.Success ? "PASS" : "FAIL")} ({frameworkGraph.LibraryCount} libraries, {frameworkGraph.PartCount} parts, {frameworkGraph.SccCount} SCCs)");
Console.WriteLine($"G3-2 promotion: {(promotion.Success ? "PASS" : "FAIL")} ({promotion.ReviewedSymbolCount} symbols, {promotion.PromotedFileCount} product files)");
Console.WriteLine($"Artifact: {artifactPath}");
foreach (var failure in failures)
{
    Console.Error.WriteLine(failure);
}
return report.Success ? 0 : 2;

static SourceCensusResult ValidateCensus(string repositoryRoot, string dorotiRoot, List<string> failures)
{
    var lockPath = Path.Combine(dorotiRoot, "validation", "flutter-source.lock.json");
    var sourceLock = JsonSerializer.Deserialize<SourceCensusLock>(
        File.ReadAllText(lockPath),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        ?? throw new InvalidDataException("Flutter source census lock is empty.");
    if (sourceLock.SchemaVersion != LockSchema)
    {
        failures.Add($"Unsupported Flutter source census schema: {sourceLock.SchemaVersion}");
    }

    var libraryRoot = Path.Combine(repositoryRoot, "flutter-master", "packages", "flutter", "lib");
    var dartFiles = Directory.EnumerateFiles(libraryRoot, "*.dart", SearchOption.AllDirectories)
        .OrderBy(path => Normalize(Path.GetRelativePath(libraryRoot, path)), StringComparer.Ordinal)
        .ToArray();
    var rootFiles = Directory.EnumerateFiles(libraryRoot, "*.dart", SearchOption.TopDirectoryOnly)
        .OrderBy(Path.GetFileName, StringComparer.Ordinal)
        .ToArray();
    var actualRoots = rootFiles.ToDictionary(
        path => Path.GetFileName(path)!,
        path => Regex.Matches(File.ReadAllText(path), @"(?m)^\s*export\s+").Count,
        StringComparer.Ordinal);
    var srcCount = dartFiles.Count(path => Normalize(Path.GetRelativePath(libraryRoot, path)).StartsWith("src/", StringComparison.Ordinal));
    var exportCount = actualRoots.Values.Sum();
    var digest = ComputeCensusHash(libraryRoot, dartFiles);

    CheckEqual(sourceLock.DartFileCount, dartFiles.Length, "Dart file count", failures);
    CheckEqual(sourceLock.SrcDartFileCount, srcCount, "src Dart file count", failures);
    CheckEqual(sourceLock.PublicRootCount, actualRoots.Count, "public root count", failures);
    CheckEqual(sourceLock.ExportDirectiveCount, exportCount, "root export directive count", failures);
    CheckEqual(sourceLock.CensusSha256, digest, "source census SHA-256", failures);
    if (sourceLock.UpstreamRevision != "56b8e1a851a594b1a154f8ea93270807dab22b9a")
    {
        failures.Add($"Unexpected pinned Flutter revision: {sourceLock.UpstreamRevision}");
    }
    foreach (var expected in sourceLock.PublicRoots.OrderBy(item => item.Key, StringComparer.Ordinal))
    {
        if (!actualRoots.TryGetValue(expected.Key, out var actual))
        {
            failures.Add($"Missing public Flutter root: {expected.Key}");
        }
        else
        {
            CheckEqual(expected.Value, actual, $"{expected.Key} export count", failures);
        }
    }
    foreach (var unexpected in actualRoots.Keys.Except(sourceLock.PublicRoots.Keys, StringComparer.Ordinal))
    {
        failures.Add($"Unexpected public Flutter root: {unexpected}");
    }

    return new(
        sourceLock.UpstreamRevision,
        dartFiles.Length,
        srcCount,
        actualRoots.Count,
        exportCount,
        digest,
        actualRoots);
}

static ActiveGraphResult ValidateActiveGraph(string repositoryRoot, string dorotiRoot, List<string> failures)
{
    var removedNamespace = "Doroti.Flutter" + "Compat";
    var removedProject = "Doroti.Flutter." + "Framework.csproj";
    var removedDirectory = Path.Combine(dorotiRoot, "src", removedNamespace);
    if (Directory.Exists(removedDirectory))
    {
        failures.Add($"Removed Doroti/src/{removedNamespace} directory still exists.");
    }

    var roots = new[]
    {
        Path.Combine(dorotiRoot, "src"),
        Path.Combine(dorotiRoot, "tools"),
        Path.Combine(dorotiRoot, "validation"),
        Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp"),
        Path.Combine(repositoryRoot, "DorotiDemoApp"),
    };
    var files = roots.Where(Directory.Exists)
        .SelectMany(root => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        .Concat(Directory.EnumerateFiles(dorotiRoot, "*.slnx", SearchOption.TopDirectoryOnly))
        .Where(path => !IsBuildOutput(path))
        .Where(path => Path.GetExtension(path) is ".cs" or ".csproj" or ".slnx" or ".props" or ".targets" or ".md")
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
    var forbiddenTokens = new[] { removedNamespace, removedProject };
    var findings = new List<string>();
    foreach (var path in files)
    {
        var text = File.ReadAllText(path);
        foreach (var token in forbiddenTokens)
        {
            if (text.Contains(token, StringComparison.Ordinal))
            {
                findings.Add($"{Normalize(Path.GetRelativePath(repositoryRoot, path))}: {token}");
            }
        }
    }
    failures.AddRange(findings.Select(finding => $"Forbidden active graph reference: {finding}"));
    var removedCompletionArtifacts = new[]
    {
        "migration/flutter-framework/f1-ownership.json",
        "migration/flutter-framework/f2-ownership.json",
        "migration/flutter-framework/f3-ownership.json",
        "migration/flutter-framework/f4-ownership.json",
        "migration/flutter-framework/f4-support-matrix.json",
        "migration/flutter-framework/f4-reference-trace.json",
        "migration/product-ui-support.json",
        "migration/host/h6-product-ui-evidence.json",
    };
    foreach (var relative in removedCompletionArtifacts)
    {
        if (File.Exists(Path.Combine(dorotiRoot, relative.Replace('/', Path.DirectorySeparatorChar))))
        {
            failures.Add($"Removed pre-Goal3 completion artifact still exists: {relative}");
        }
    }
    return new(!Directory.Exists(removedDirectory), files.Length, findings.ToArray());
}

static FrameworkTruthResetResult ValidateFrameworkTruthReset(string dorotiRoot, List<string> failures)
{
    var expected = new Dictionary<string, (int MissingMechanical, int CompletionBlockers)>(StringComparer.Ordinal)
    {
        ["F1"] = (5003, 5004),
        ["F2"] = (31709, 31710),
        ["F3"] = (31709, 31710),
        ["F4"] = (54314, 54315),
    };
    var reports = new List<FlutterFrameworkEvidenceAuditReport>();
    foreach (var milestone in expected.Keys)
    {
        var evidence = Path.Combine(dorotiRoot, "migration", "flutter-framework", $"{milestone.ToLowerInvariant()}-evidence.json");
        var report = milestone switch
        {
            "F1" => FlutterF1ClosureAudit.Run(dorotiRoot, evidence),
            "F2" => FlutterF2ClosureAudit.Run(dorotiRoot, evidence),
            "F3" => FlutterF3ClosureAudit.Run(dorotiRoot, evidence),
            "F4" => FlutterF4ClosureAudit.Run(dorotiRoot, evidence),
            _ => throw new InvalidOperationException(),
        };
        reports.Add(report);
        if (!report.Success)
        {
            failures.AddRange(report.Findings.Select(finding => $"{milestone} evidence audit {finding.Code}: {finding.Message}"));
        }
        if (report.MissingMechanicalDeclarations + report.MissingMechanicalMembers != expected[milestone].MissingMechanical ||
            report.ImplementationBlockers != expected[milestone].CompletionBlockers || report.MilestoneComplete)
        {
            failures.Add($"{milestone} truth-reset blocker totals drifted.");
        }
    }

    var tempRoot = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "g3-evidence");
    try
    {
        FlutterFrameworkEvidenceReset.Regenerate(dorotiRoot, tempRoot);
        foreach (var milestone in expected.Keys)
        {
            var relative = Path.Combine("migration", "flutter-framework", $"{milestone.ToLowerInvariant()}-evidence.json");
            if (!File.ReadAllBytes(Path.Combine(dorotiRoot, relative)).SequenceEqual(File.ReadAllBytes(Path.Combine(tempRoot, relative))))
            {
                failures.Add($"{milestone} evidence reset is not byte-identical to the committed artifact.");
            }
        }

        var f1Path = Path.Combine(dorotiRoot, "migration", "flutter-framework", "f1-evidence.json");
        var mutated = JsonNode.Parse(File.ReadAllText(f1Path))!.AsObject();
        var gate = mutated["compilerGate"]!.AsObject();
        gate["milestoneProfile"] = "flutter-framework-f1";
        gate["milestoneGeneratedDeclarations"] = 1;
        var mutationPath = Path.Combine(tempRoot, "f1-unregistered-profile.json");
        File.WriteAllText(mutationPath, mutated.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n", new UTF8Encoding(false));
        var rejected = FlutterF1ClosureAudit.Run(dorotiRoot, mutationPath);
        if (rejected.Success || !rejected.Findings.Any(finding => finding.Code == "DOTG3014"))
        {
            failures.Add("Architecture gate did not reject F1 generated coverage without a registered F1 compiler profile.");
        }
    }
    finally
    {
        RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, tempRoot);
    }

    return new(
        reports.All(report => report.Success),
        reports.Sum(report => report.ImplementationBlockers),
        reports.Select(report => new FrameworkMilestoneTruth(
            report.Milestone,
            report.ResolvedInventory.Declarations,
            report.ResolvedInventory.Members,
            report.MechanicalGeneratedDeclarations,
            report.ImplementationBlockers,
            report.MilestoneComplete)).ToArray());
}

static MechanicalCandidateResult ValidateMechanicalCandidate(string dorotiRoot, List<string> failures)
{
    var manifest = Path.Combine(dorotiRoot, "migration", "selections", "f0-framework-object.json");
    var output = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "g3-f0");
    try
    {
        var report = new DartCompiler().Compile(manifest, output);
        var committedRoot = Path.Combine(
            dorotiRoot,
            "migration",
            "generated-candidates",
            "flutter-framework",
            "56b8e1a851a594b1a154f8ea93270807dab22b9a",
            "foundation");
        foreach (var relative in new[]
        {
            "object.g.cs",
            "source-map.json",
            "Doroti.Generated.Framework.Foundation.csproj",
            "Directory.Build.props",
        })
        {
            if (!File.ReadAllBytes(Path.Combine(output, relative)).SequenceEqual(File.ReadAllBytes(Path.Combine(committedRoot, relative))))
            {
                failures.Add($"F0 current compiler output differs from the committed mechanical candidate: {relative}");
            }
        }
        var coverage = JsonSerializer.Deserialize<MechanicalCoverage>(
            File.ReadAllText(Path.Combine(output, "framework-coverage.json")),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidDataException("F0 framework coverage is empty.");
        var success = report.Success && report.Outputs.Length == 1 && report.Diagnostics.Length == 0 &&
            coverage.Status == "mechanical-generated" && coverage.DeclarationCount == 1 && coverage.MemberCount == 0 &&
            coverage.UnclassifiedAstNodeCount == 0 && coverage.SilentOmissionCount == 0 && coverage.GeneratedCompileErrorCount == 0;
        if (!success)
        {
            failures.Add("F0 did not regenerate as exactly one clean mechanical candidate.");
        }
        return new(success, report.Identity.ConverterVersion, report.Identity.FlutterGitRevision, report.Outputs.SingleOrDefault()?.Sha256);
    }
    finally
    {
        RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, output);
    }
}

static CompilerNegativeResult ValidateCompilerNegative(string dorotiRoot, List<string> failures)
{
    var manifest = Path.Combine(dorotiRoot, "validation", "cases", "compiler-negative.selection.json");
    var output = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "g3-t0");
    try
    {
        var report = new DartCompiler().Compile(manifest, output);
        var diagnostic = report.Diagnostics.SingleOrDefault(item =>
            item.Code == "DOTF0001" &&
            item.Severity == "error" &&
            item.Symbol == "unsupportedBootstrap" &&
            item.Length > 0 &&
            !string.IsNullOrWhiteSpace(item.ManualAction));
        if (report.Success)
        {
            failures.Add("Compiler negative case unexpectedly succeeded.");
        }
        if (diagnostic is null)
        {
            failures.Add("Compiler negative case did not emit the required DOTF0001 typed span/action diagnostic.");
        }
        return new(
            !report.Success && diagnostic is not null,
            report.Identity.FlutterGitRevision,
            diagnostic?.Code,
            diagnostic?.Offset,
            diagnostic?.Length);
    }
    finally
    {
        RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, output);
    }
}

static FrameworkGraphResult ValidateFrameworkGraph(string repositoryRoot, string dorotiRoot, List<string> failures)
{
    var manifest = Path.Combine(dorotiRoot, "migration", "selections", "g3-1-framework-multilibrary.json");
    var unsupportedManifest = Path.Combine(dorotiRoot, "validation", "cases", "g3-1-unsupported.selection.json");
    var tempRoot = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "g3-1-validation");
    var first = Path.Combine(tempRoot, "first");
    var second = Path.Combine(tempRoot, "second");
    var negative = Path.Combine(tempRoot, "negative");
    var cache = Path.Combine(tempRoot, "cache");
    try
    {
        var compiler = new DartCompiler();
        var firstReport = compiler.Compile(manifest, first, cache);
        var secondReport = compiler.Compile(manifest, second, cache);
        if (!firstReport.Success || !secondReport.Success || firstReport.Outputs.Length != 3 ||
            firstReport.Diagnostics.Any(item => item.Severity == "error"))
        {
            failures.Add("G3-1 upstream multi-library selection did not produce three clean mechanical candidates.");
        }

        var deterministicFiles = Directory.EnumerateFiles(first, "*", SearchOption.AllDirectories)
            .Select(path => Normalize(Path.GetRelativePath(first, path)))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var secondFiles = Directory.EnumerateFiles(second, "*", SearchOption.AllDirectories)
            .Select(path => Normalize(Path.GetRelativePath(second, path)))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var deterministic = deterministicFiles.SequenceEqual(secondFiles, StringComparer.Ordinal) &&
            deterministicFiles.All(relative => File.ReadAllBytes(Path.Combine(first, relative.Replace('/', Path.DirectorySeparatorChar)))
                .SequenceEqual(File.ReadAllBytes(Path.Combine(second, relative.Replace('/', Path.DirectorySeparatorChar)))));
        if (!deterministic)
        {
            failures.Add("G3-1 clean/cache regeneration was not byte-identical.");
        }
        var committed = Path.Combine(
            dorotiRoot,
            "migration",
            "generated-candidates",
            "flutter-framework",
            "56b8e1a851a594b1a154f8ea93270807dab22b9a",
            "g3-1");
        var committedFiles = Directory.EnumerateFiles(committed, "*", SearchOption.AllDirectories)
            .Select(path => Normalize(Path.GetRelativePath(committed, path)))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var committedMatches = deterministicFiles.SequenceEqual(committedFiles, StringComparer.Ordinal) &&
            deterministicFiles.All(relative => File.ReadAllBytes(Path.Combine(first, relative.Replace('/', Path.DirectorySeparatorChar)))
                .SequenceEqual(File.ReadAllBytes(Path.Combine(committed, relative.Replace('/', Path.DirectorySeparatorChar)))));
        if (!committedMatches)
        {
            failures.Add("G3-1 current compiler output differs from the committed mechanical candidate graph.");
        }

        var absoluteCheckout = Normalize(repositoryRoot);
        foreach (var relative in deterministicFiles.Where(path => Path.GetExtension(path) is ".json" or ".cs" or ".csproj" or ".slnx"))
        {
            if (Normalize(File.ReadAllText(Path.Combine(first, relative.Replace('/', Path.DirectorySeparatorChar))))
                .Contains(absoluteCheckout, StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"G3-1 artifact contains an absolute checkout path: {relative}");
            }
        }

        var graphPath = Path.Combine(first, "framework-project-graph.json");
        using var graphDocument = JsonDocument.Parse(File.ReadAllText(graphPath));
        var root = graphDocument.RootElement;
        var graphCensus = root.GetProperty("sourceCensus");
        var censusComplete = graphCensus.GetProperty("dartFileCount").GetInt32() == 695 &&
            graphCensus.GetProperty("publicRootCount").GetInt32() == 13 &&
            graphCensus.GetProperty("publicRootExportDirectiveCount").GetInt32() == 640 &&
            graphCensus.GetProperty("files").GetArrayLength() == 695;
        var libraries = root.GetProperty("libraries").EnumerateArray().ToArray();
        var sccs = root.GetProperty("sccs").EnumerateArray().ToArray();
        var partitions = root.GetProperty("partitions").EnumerateArray().ToArray();
        var references = root.GetProperty("projectReferences").EnumerateArray().ToArray();
        var partCount = libraries.SelectMany(item => item.GetProperty("fragments").EnumerateArray())
            .Count(item => !item.GetProperty("isDefining").GetBoolean());
        var animated = libraries.SingleOrDefault(item => item.GetProperty("library").GetString() == "package:flutter/src/material/animated_icons.dart");
        var prefixResolved = animated.ValueKind != JsonValueKind.Undefined && animated.GetProperty("imports").EnumerateArray()
            .Any(item => item.TryGetProperty("prefix", out var prefix) && prefix.GetString() == "math");
        var extensionsResolved = libraries.Any(item => item.GetProperty("accessibleExtensions").GetArrayLength() > 0);
        var privateScopeRecorded = libraries.Any(item =>
            item.GetProperty("library").GetString() == "package:flutter/src/foundation/_bitfield_io.dart" &&
            item.GetProperty("fragments").EnumerateArray().SelectMany(fragment => fragment.GetProperty("declarationElementIds").EnumerateArray()).Any() &&
            item.GetProperty("fragments").EnumerateArray().SelectMany(fragment => fragment.GetProperty("declarationElementIds").EnumerateArray()).All(element =>
                element.GetString()!.StartsWith("package:flutter/src/foundation/_bitfield_io.dart#", StringComparison.Ordinal)));
        var partOwned = animated.ValueKind != JsonValueKind.Undefined &&
            animated.GetProperty("fragments").GetArrayLength() == 17 && partCount == 16;
        var cycleMerged = root.GetProperty("everyCycleMerged").GetBoolean() && sccs.Any(item =>
            item.GetProperty("isCycle").GetBoolean() &&
            item.GetProperty("libraries").EnumerateArray().Select(value => value.GetString()).ToHashSet(StringComparer.Ordinal)
                .SetEquals(["package:flutter/src/foundation/bitfield.dart", "package:flutter/src/foundation/_bitfield_io.dart"]) &&
            item.GetProperty("partitions").GetArrayLength() == 1);
        var projectGraphValid = partitions.Length == 2 &&
            references.Any(item => item.GetProperty("from").GetString() == "Physics" && item.GetProperty("to").GetString() == "Foundation") &&
            partitions.All(item => item.GetProperty("packageReferences").EnumerateArray()
                .Any(reference => reference.GetProperty("package").GetString() == "Doroti.Flutter.Runtime"));
        var generatedShape = File.ReadAllText(Path.Combine(first, "projects", "Foundation", "annotations.g.cs"));
        var genericConstructorGenerated = generatedShape.Contains("IReadOnlyList<string> sections", StringComparison.Ordinal) &&
            generatedShape.Contains("public Category(IReadOnlyList<string> sections)", StringComparison.Ordinal);
        if (!censusComplete || !prefixResolved || !extensionsResolved || !privateScopeRecorded || !partOwned ||
            !cycleMerged || !projectGraphValid || !genericConstructorGenerated)
        {
            failures.Add("G3-1 project graph is missing 13-root/695-file census, prefix, part ownership, SCC merge, or project/package reference evidence.");
        }

        using var evidenceDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(dorotiRoot, "migration", "flutter-framework", "g3-1-evidence.json")));
        var evidenceRoot = evidenceDocument.RootElement;
        var evidenceValid = evidenceRoot.GetProperty("schemaVersion").GetString() == "doroti.g3-1-evidence/v1" &&
            evidenceRoot.GetProperty("status").GetString() == "complete" &&
            evidenceRoot.GetProperty("milestoneComplete").GetBoolean() &&
            evidenceRoot.GetProperty("compiler").GetProperty("workspaceId").GetString() == firstReport.Identity.WorkspaceId &&
            evidenceRoot.GetProperty("selection").GetProperty("generatedDeclarationCount").GetInt32() == 5 &&
            evidenceRoot.GetProperty("selection").GetProperty("generatedMemberCount").GetInt32() == 13 &&
            evidenceRoot.GetProperty("artifacts").EnumerateArray().All(item =>
            {
                var artifact = Path.Combine(dorotiRoot, item.GetProperty("path").GetString()!.Replace('/', Path.DirectorySeparatorChar));
                var hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(artifact))).ToLowerInvariant();
                return hash == item.GetProperty("sha256").GetString();
            });
        if (!evidenceValid)
        {
            failures.Add("G3-1 committed evidence schema, cardinality, workspace identity, or artifact hashes are invalid.");
        }

        var dorotiRootArgument = $"-p:DorotiRepositoryRoot={dorotiRoot}";
        var solution = Path.Combine(first, "Doroti.Generated.Framework.slnx");
        var build = RunExternal("dotnet", ["build", solution, "--configuration", "Release", "--nologo", dorotiRootArgument], first);
        if (build.ExitCode != 0)
        {
            failures.Add($"G3-1 generated project build failed: {build.Output}");
        }

        var consumer = Path.Combine(tempRoot, "consumer");
        Directory.CreateDirectory(consumer);
        File.WriteAllText(
            Path.Combine(consumer, "G31.Consumer.csproj"),
            $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
                <DorotiRepositoryRoot>{dorotiRoot}</DorotiRepositoryRoot>
              </PropertyGroup>
              <ItemGroup>
                <ProjectReference Include="{Path.Combine(first, "projects", "Foundation", "Doroti.Generated.Framework.Foundation.csproj")}" />
                <ProjectReference Include="{Path.Combine(first, "projects", "Physics", "Doroti.Generated.Framework.Physics.csproj")}" />
              </ItemGroup>
            </Project>
            """ + "\n",
            new UTF8Encoding(false));
        File.WriteAllText(
            Path.Combine(consumer, "Program.cs"),
            """
            using System;
            using Doroti.Generated.Framework.Foundation;
            using Doroti.Generated.Framework.Physics;

            var category = new Category(new[] { "Framework", "Compiler" });
            var defaults = new Tolerance();
            var custom = new Tolerance(distance: 0.25, time: 0.5, velocity: 0.75);
            if (category.sections.Count != 2 || defaults.distance != 0.001 || defaults.time != 0.001 ||
                defaults.velocity != 0.001 || custom.ToString() != "Tolerance(distance: ±0.25, time: ±0.5, velocity: ±0.75)")
            {
                return 2;
            }
            Console.WriteLine("G3-1-UPSTREAM-RUN-PASS");
            return 0;
            """ + "\n",
            new UTF8Encoding(false));
        var consumerBuild = RunExternal(
            "dotnet",
            ["build", "G31.Consumer.csproj", "--configuration", "Release", "--nologo", dorotiRootArgument],
            consumer);
        var consumerRun = consumerBuild.ExitCode == 0
            ? RunExternal("dotnet", ["run", "--project", "G31.Consumer.csproj", "--configuration", "Release", "--no-build"], consumer)
            : new ExternalResult(consumerBuild.ExitCode, consumerBuild.Output);
        var compiledAndRan = build.ExitCode == 0 && consumerBuild.ExitCode == 0 && consumerRun.ExitCode == 0 &&
            consumerRun.Output.Contains("G3-1-UPSTREAM-RUN-PASS", StringComparison.Ordinal);
        if (!compiledAndRan)
        {
            failures.Add($"G3-1 external generated consumer did not compile/run: {consumerBuild.Output}\n{consumerRun.Output}");
        }

        var negativeReport = compiler.Compile(unsupportedManifest, negative, cache);
        var typedDiagnostic = negativeReport.Diagnostics.FirstOrDefault(item =>
            item.Code == "DOTF0001" && item.Severity == "error" && item.Length > 0 &&
            !string.IsNullOrWhiteSpace(item.ManualAction) && item.Library == "package:doroti_validation/unsupported_syntax.dart");
        var unsupportedRejected = !negativeReport.Success && typedDiagnostic is not null;
        var cliNegative = Path.Combine(tempRoot, "negative-cli");
        var cliResult = RunExternal(
            "dotnet",
            [
                "run",
                "--project", Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "Doroti.DartToCSharp.csproj"),
                "--configuration", "Release",
                "--no-build",
                "--",
                "--manifest", unsupportedManifest,
                "--output", cliNegative,
                "--cache-dir", cache,
            ],
            repositoryRoot);
        unsupportedRejected = unsupportedRejected && cliResult.ExitCode == 2;
        if (!unsupportedRejected)
        {
            failures.Add($"G3-1 unsupported pinned framework syntax did not fail with a typed span/action diagnostic and exit code 2: {cliResult.Output}");
        }

        var success = firstReport.Success && secondReport.Success && deterministic && committedMatches && evidenceValid && censusComplete && prefixResolved &&
            extensionsResolved && privateScopeRecorded && partOwned && cycleMerged && projectGraphValid && genericConstructorGenerated &&
            compiledAndRan && unsupportedRejected;
        return new(
            success,
            libraries.Length,
            partCount,
            sccs.Length,
            partitions.Length,
            deterministic,
            compiledAndRan,
            unsupportedRejected,
            firstReport.Identity.WorkspaceId);
    }
    finally
    {
        RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, tempRoot);
    }
}

static ExternalResult RunExternal(string fileName, string[] arguments, string workingDirectory)
{
    using var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        },
    };
    foreach (var argument in arguments)
    {
        process.StartInfo.ArgumentList.Add(argument);
    }
    process.Start();
    var standardOutput = process.StandardOutput.ReadToEndAsync();
    var standardError = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit((int)TimeSpan.FromMinutes(15).TotalMilliseconds))
    {
        process.Kill(entireProcessTree: true);
        throw new TimeoutException($"External validation exceeded 15 minutes: {fileName} {string.Join(' ', arguments)}");
    }
    Task.WaitAll(standardOutput, standardError);
    return new(process.ExitCode, string.Join('\n', standardOutput.Result, standardError.Result).Trim());
}

static string ComputeCensusHash(string root, IEnumerable<string> files)
{
    using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    foreach (var path in files)
    {
        var relative = Normalize(Path.GetRelativePath(root, path));
        var fileHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
        hash.AppendData(Encoding.UTF8.GetBytes($"{relative}\0{fileHash}\n"));
    }
    return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
}

static bool IsBuildOutput(string path)
{
    var normalized = Normalize(path);
    return normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
        normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase);
}

static void CheckEqual<T>(T expected, T actual, string name, List<string> failures)
    where T : notnull
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        failures.Add($"{name} mismatch: expected {expected}, actual {actual}.");
    }
}

static string Normalize(string path) => path.Replace('\\', '/');

static string FindRepositoryRoot(string start)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(start)); directory is not null; directory = directory.Parent)
    {
        if ((File.Exists(Path.Combine(directory.FullName, "goal6.md")) ||
             File.Exists(Path.Combine(directory.FullName, "goal5.md"))) &&
            File.Exists(Path.Combine(directory.FullName, "Doroti", "Doroti.slnx")) &&
            File.Exists(Path.Combine(directory.FullName, "tools", "Doroti.DartToCSharp", "Doroti.DartToCSharp.csproj")))
        {
            return directory.FullName;
        }
    }
    throw new DirectoryNotFoundException($"Could not find DorotiLab root from {start}.");
}

internal sealed record SourceCensusLock(
    string SchemaVersion,
    string UpstreamRevision,
    int DartFileCount,
    int SrcDartFileCount,
    int PublicRootCount,
    int ExportDirectiveCount,
    string CensusSha256,
    Dictionary<string, int> PublicRoots);

internal sealed record SourceCensusResult(
    string UpstreamRevision,
    int DartFileCount,
    int SrcDartFileCount,
    int PublicRootCount,
    int ExportDirectiveCount,
    string CensusSha256,
    Dictionary<string, int> PublicRoots);

internal sealed record ActiveGraphResult(bool RemovedDirectoryAbsent, int ScannedFileCount, string[] Findings);
internal sealed record FrameworkTruthResetResult(bool Success, int TotalImplementationBlockers, FrameworkMilestoneTruth[] Milestones);
internal sealed record FrameworkMilestoneTruth(string Milestone, int ResolvedDeclarations, int ResolvedMembers, int MechanicalGeneratedDeclarations, int ImplementationBlockers, bool MilestoneComplete);
internal sealed record MechanicalCandidateResult(bool Success, string? ConverterVersion, string? FlutterRevision, string? OutputSha256);
internal sealed record MechanicalCoverage(string SchemaVersion, string Status, int DeclarationCount, int MemberCount, int UnclassifiedAstNodeCount, int SilentOmissionCount, int GeneratedCompileErrorCount);
internal sealed record CompilerNegativeResult(bool Success, string? FlutterRevision, string? DiagnosticCode, int? Offset, int? Length);
internal sealed record FrameworkGraphResult(
    bool Success,
    int LibraryCount,
    int PartCount,
    int SccCount,
    int ProjectCount,
    bool Deterministic,
    bool CompiledAndRan,
    bool UnsupportedRejected,
    string? WorkspaceId);
internal sealed record ExternalResult(int ExitCode, string Output);
internal sealed record ValidationReport(
    string SchemaVersion,
    bool Success,
    SourceCensusResult SourceCensus,
    ActiveGraphResult ActiveGraph,
    FrameworkTruthResetResult FrameworkTruthReset,
    MechanicalCandidateResult MechanicalCandidate,
    CompilerNegativeResult CompilerNegative,
    FrameworkGraphResult FrameworkGraph,
    G3PromotionResult Promotion,
    string[] Failures);
