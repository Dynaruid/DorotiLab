using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Doroti.Runtime;
using Doroti.Tooling;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Doroti.DartToCSharp;

// Internal orchestration while the frontend, identity, lowering and publication services own their boundaries.
internal static partial class ConverterEngine
{
    private static readonly ConcurrentDictionary<string, object> C5PackagePrepareLocks = new(StringComparer.Ordinal);

    private sealed record InputCompileResult(
        MigrationIrInput IrInput,
        AnalyzerOutput AnalyzerOutput,
        FrameworkCoverageInput? Coverage,
        ConverterDiagnostic[] Diagnostics,
        string? OutputName,
        string? GeneratedCode,
        SourceMapEntry[] Mappings,
        string[] DeclarationNames,
        DartResolvedDeclaration[] DartResolvedDeclarations,
        CoreResolvedDeclaration[] CoreResolvedDeclarations,
        bool GraphOnly);

    internal static ConverterReport Convert(
        string manifestPath,
        string outputDirectory,
        string? previousOutputDirectory = null,
        string? cacheDirectory = null,
        int? maxDegreeOfParallelism = null,
        CompilerDumpOptions? dumpOptions = null,
        CompilerProfiler? profiler = null,
        int analyzerWorkers = 1,
        int? loweringParallelism = null)
    {
        profiler ??= new CompilerProfiler(
            manifestPath,
            telemetryPath: null,
            analyzerWorkers,
            CompilerParallelism.ResolveLoweringParallelism(loweringParallelism, maxDegreeOfParallelism));
        SelectionManifest manifest;
        using (profiler.MeasureInvocation("identity-fingerprint"))
        {
            manifest = ArtifactFiles.ReadJson<SelectionManifest>(manifestPath);
        }
        if (manifest.SchemaVersion is not ("doroti.converter-selection/v3" or "doroti.converter-selection/v4"))
        {
            throw new InvalidDataException($"Unsupported selection schema: {manifest.SchemaVersion}");
        }

        if (manifest.ConverterVersion != CompilerVersions.Converter)
        {
            throw new InvalidDataException($"Manifest requests converter {manifest.ConverterVersion}; this executable is {CompilerVersions.Converter}.");
        }
        if (manifest.AnalysisMode is not null and not "syntax-only")
        {
            throw new InvalidDataException($"Unsupported analyzer mode: {manifest.AnalysisMode}.");
        }
        var profile = CompilerProfileRegistry.Resolve(manifest);

        var manifestDirectory = Path.GetDirectoryName(Path.GetFullPath(manifestPath))!;
        ApplicationGraphPlan? applicationPlan = null;
        if (manifest.Application is not null)
        {
            (manifest, applicationPlan) = ApplicationGraphResolver.Expand(
                manifest,
                manifestDirectory,
                previousOutputDirectory);
        }
        var analyzerHome = AnalyzerHomeResolver.Resolve(manifestPath, manifest);
        var analyzerProject = analyzerHome.AnalyzerRoot;
        var flutterBaselinePath = Path.GetFullPath(manifest.FlutterBaseline, manifestDirectory);
        CompilerIdentity identity;
        using (profiler.MeasureInvocation("compiler-identity"))
        {
            identity = CompilerIdentityFactory.Create(
                analyzerHome,
                flutterBaselinePath,
                ComputeWorkspaceId(manifestPath),
                profile);
            profiler.SetCompilerIdentity(identity.WorkspaceId);
        }
        var packageGraph = CreatePackageGraph(manifest, manifestDirectory);
        var compatibilityRules = profile.CompatibilityRules;
        var diagnostics = new List<ConverterDiagnostic>();
        var outputs = new List<ConverterOutput>();
        var irInputs = new List<MigrationIrInput>();
        var frameworkCoverageInputs = new List<FrameworkCoverageInput>();
        var mappings = new List<SourceMapEntry>();
        Directory.CreateDirectory(outputDirectory);

        var orderedInputs = manifest.Inputs.OrderBy(item => item.Path, StringComparer.Ordinal).ToArray();
        profiler.InputCount = orderedInputs.Length;
        var analyzerSession = new AnalyzerSession(analyzerProject, profiler);
        var parallelism = CompilerParallelism.ResolveLoweringParallelism(loweringParallelism, maxDegreeOfParallelism);
        var platformReferences = CreatePlatformReferences(profile);
        var results = new InputCompileResult[orderedInputs.Length];
        var migrationFragments = new string[orderedInputs.Length];
        var migrationFragmentDirectory = Path.Combine(outputDirectory, ".migration-ir-fragments");
        Directory.CreateDirectory(migrationFragmentDirectory);
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = parallelism };

        string[] analyzerPayloads;
        using (profiler.MeasureInvocation("analyzer-session"))
        {
            var analyzerInputs = orderedInputs.Select((input, index) =>
            {
                var inputPath = ResolveInputPath(manifest, manifestDirectory, input.Path);
                var analyzerInputPath = profile.IsC5
                    ? PrepareC5AnalyzerInput(manifest, manifestDirectory, input.Path, inputPath, File.ReadAllText(inputPath))
                    : inputPath;
                return new AnalyzerSessionInput(index, input.Path, analyzerInputPath);
            }).ToArray();
            analyzerPayloads = analyzerSession.Analyze(
                analyzerInputs,
                cacheDirectory,
                string.Equals(manifest.AnalysisMode, "syntax-only", StringComparison.Ordinal),
                profile.IsFrameworkGraph,
                applicationPlan?.PackageConfigPath);
        }
        using (profiler.MeasureInvocation("analyze-and-core-lowering"))
        {
            Parallel.For(0, orderedInputs.Length, parallelOptions, index =>
            {
                using var queue = profiler.EnterWorkerQueue();
                var result = CompileInput(
                    orderedInputs[index],
                    manifest,
                    profile,
                    manifestDirectory,
                    packageGraph,
                    analyzerPayloads[index],
                    platformReferences,
                    CompilationContext.Empty,
                    profiler);
                migrationFragments[index] = StageMigrationInput(result.IrInput, migrationFragmentDirectory, index);
                result = result with { IrInput = CompactMigrationInput(result.IrInput) };
                results[index] = profile.IsFrameworkGraph
                    ? TrimDumpState(result, dumpOptions, retainCore: true)
                    : StageGeneratedResult(result, outputDirectory, dumpOptions, profiler);
            });
        }

        if (profile.IsFrameworkGraph)
        {
            CompilationContext compilationContext;
            using (profiler.MeasureInvocation("semantic-index-build"))
            {
                compilationContext = CompilationContext.Create(
                    results.SelectMany(result => result.CoreResolvedDeclarations),
                    results.Where(result => !result.GraphOnly)
                        .SelectMany(result => result.CoreResolvedDeclarations));
            }
            using (profiler.MeasureInvocation("csharp-lowering-printing"))
            {
                Parallel.For(0, orderedInputs.Length, parallelOptions, index =>
                {
                    using var queue = profiler.EnterWorkerQueue();
                    using var phase = profiler.MeasureLibrary("csharp-lowering-printing", orderedInputs[index].Path);
                    var compiled = CompileResolvedFramework(
                        results[index],
                        manifest,
                        profile,
                        packageGraph,
                        platformReferences,
                        compilationContext);
                    results[index] = applicationPlan is not null &&
                        !applicationPlan.AffectedLibraries.Contains(compiled.IrInput.Library, StringComparer.Ordinal) &&
                        TryReuseGeneratedResult(compiled, previousOutputDirectory, outputDirectory)
                            ? TrimDumpState(compiled with { GeneratedCode = null }, dumpOptions)
                            : StageGeneratedResult(compiled, outputDirectory, dumpOptions, profiler);
                });
            }
        }
        using (profiler.MeasureInvocation("generated-file-publish"))
        {
            // Deterministic sequential publish: reports stay sorted by input path.
            foreach (var result in results)
            {
                diagnostics.AddRange(result.Diagnostics);
                irInputs.Add(result.IrInput);
                if (result.Coverage is not null)
                {
                    frameworkCoverageInputs.Add(result.Coverage);
                }

                if (result.GraphOnly || result.OutputName is null)
                {
                    continue;
                }

                var outputPath = Path.Combine(outputDirectory, result.OutputName);
                if (result.GeneratedCode is not null && !File.Exists(outputPath))
                {
                    ArtifactFiles.WriteUtf8(outputPath, result.GeneratedCode);
                    profiler.AddOutputBytes(Encoding.UTF8.GetByteCount(result.GeneratedCode));
                }
                if (!File.Exists(outputPath))
                {
                    throw new InvalidDataException($"Generated staging file is missing: {result.OutputName}");
                }
                mappings.AddRange(result.Mappings);
                outputs.Add(new(result.IrInput.Path, result.OutputName, ArtifactFiles.Sha256(outputPath), result.DeclarationNames));
            }

            if (profile.IsFrameworkGraph)
            {
                if (applicationPlan is null)
                {
                    WriteFrameworkProjectGraph(outputDirectory, manifest, manifestDirectory, identity, irInputs, outputs, diagnostics);
                }
                else
                {
                    WriteApplicationProjectGraph(
                        outputDirectory,
                        manifest,
                        manifestDirectory,
                        identity,
                        irInputs,
                        outputs,
                        diagnostics,
                        applicationPlan);
                }
            }
            else
            {
                WriteGeneratedProject(
                    outputDirectory,
                    manifest,
                    profile.ReferenceRuntimeBindings && !profile.IsC5,
                    profile.IsC5,
                    packageGraph,
                    identity,
                    reportOutputs: outputs);
            }
        }
        var sortedDiagnostics = diagnostics
            .OrderBy(item => item.Source, StringComparer.Ordinal)
            .ThenBy(item => item.Offset)
            .ThenBy(item => item.Code, StringComparer.Ordinal)
            .ToArray();
        var report = new ConverterReport(
            "doroti.converter-report/v2",
            identity,
            sortedDiagnostics.All(item => item.Severity != "error"),
            outputs.OrderBy(item => item.Input, StringComparer.Ordinal).ToArray(),
            sortedDiagnostics);
        using (profiler.MeasureInvocation("report-serialization"))
        {
            WriteJsonStreaming(Path.Combine(outputDirectory, "converter-report.json"), report);
            WriteJsonStreaming(
                Path.Combine(outputDirectory, "source-map.json"),
                new SourceMapDocument(
                    "doroti.source-map/v1",
                    mappings.OrderBy(item => item.Source, StringComparer.Ordinal).ThenBy(item => item.SourceOffset).ToArray()));
            WriteMigrationIrStreaming(
                Path.Combine(outputDirectory, "migration-ir.json"),
                profile.EnableTypedSemanticCompiler ? "doroti.migration-ir/v3" : "doroti.migration-ir/v2",
                identity,
                profile.IrVersion,
                manifest.GenerationMode,
                manifest.CompatibilityProfile,
                packageGraph,
                migrationFragments,
                compatibilityRules.OrderBy(item => item.Id, StringComparer.Ordinal).ToArray(),
                report.Outputs);
            Directory.Delete(migrationFragmentDirectory, recursive: true);
            if (profile.EnableTypedSemanticCompiler)
            {
                var allNodes = frameworkCoverageInputs.SelectMany(item => item.Classifications).Sum(item => item.Count);
                var compileErrors = sortedDiagnostics.Count(item => item.Code == "DOTCONV901");
                var omissions = sortedDiagnostics.Count(item => item.Code == "DOTF0002");
                var unclassified = frameworkCoverageInputs.SelectMany(item => item.Classifications)
                    .Where(item => item.Category == "unclassified")
                    .Sum(item => item.Count);
                WriteJsonStreaming(
                    Path.Combine(outputDirectory, "framework-coverage.json"),
                    new FrameworkCoverageDocument(
                        "doroti.framework-coverage/v1",
                        sortedDiagnostics.Any(item => item.Severity == "error") ? "failed" : "mechanical-generated",
                        identity,
                        frameworkCoverageInputs.OrderBy(item => item.Source, StringComparer.Ordinal).ToArray(),
                        irInputs.Sum(item => item.Declarations.Length),
                        irInputs.Sum(item => item.Declarations.Sum(declaration => declaration.Members.Length)),
                        allNodes,
                        unclassified,
                        omissions,
                        compileErrors));
            }
            if (profile.IsC5)
            {
                WritePackageRelease(outputDirectory, manifest, identity, packageGraph, report, manifestDirectory);
            }
            if (dumpOptions is not null)
            {
                var dumpDirectory = Path.GetFullPath(dumpOptions.Directory);
                var outputRoot = Path.GetFullPath(outputDirectory).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                if (dumpDirectory.TrimEnd(Path.DirectorySeparatorChar).Equals(
                        Path.GetFullPath(outputDirectory).TrimEnd(Path.DirectorySeparatorChar),
                        StringComparison.OrdinalIgnoreCase) ||
                    dumpDirectory.StartsWith(outputRoot, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidDataException("--dump-ir must be outside the compiler-owned generated workspace.");
                }
                CompilerArtifactDumper.Write(
                    dumpOptions,
                    identity,
                    results.Select(result => new CompilerDumpInput(
                        result.IrInput.Path,
                        result.IrInput.Library,
                        result.AnalyzerOutput,
                        result.DartResolvedDeclarations,
                        result.CoreResolvedDeclarations,
                        result.OutputName,
                        result.GeneratedCode,
                        result.Mappings)));
            }
        }
        return report;
    }

    private static bool TryReuseGeneratedResult(
        InputCompileResult result,
        string? previousOutputDirectory,
        string outputDirectory)
    {
        if (result.OutputName is null || string.IsNullOrWhiteSpace(previousOutputDirectory)) return false;
        var source = Path.Combine(previousOutputDirectory, result.OutputName);
        if (!File.Exists(source)) return false;
        var destination = Path.Combine(outputDirectory, result.OutputName);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(source, destination, overwrite: true);
        return true;
    }

    private static InputCompileResult TrimDumpState(
        InputCompileResult result,
        CompilerDumpOptions? dumpOptions,
        bool retainCore = false) => result with
        {
            AnalyzerOutput = dumpOptions?.Stages.HasFlag(CompilerDumpStage.AnalyzerProtocol) == true
                ? result.AnalyzerOutput
                : null!,
            DartResolvedDeclarations = dumpOptions?.Stages.HasFlag(CompilerDumpStage.DartIr) == true
                ? result.DartResolvedDeclarations
                : [],
            CoreResolvedDeclarations = retainCore || dumpOptions?.Stages.HasFlag(CompilerDumpStage.CoreIr) == true
                ? result.CoreResolvedDeclarations
                : [],
        };

    private static string StageMigrationInput(MigrationIrInput input, string directory, int ordinal)
    {
        var path = Path.Combine(directory, $"{ordinal:D5}.json");
        WriteJsonStreaming(path, input, trailingNewLine: false);
        return path;
    }

    private static MigrationIrInput CompactMigrationInput(MigrationIrInput input) => input with
    {
        Directives = [],
        Declarations = input.Declarations.Select(declaration => declaration with
        {
            Ast = null,
            Members = declaration.Members.Select(member => member with { Ast = null, Statements = [] }).ToArray(),
        }).ToArray(),
    };

    private static InputCompileResult StageGeneratedResult(
        InputCompileResult result,
        string outputDirectory,
        CompilerDumpOptions? dumpOptions,
        CompilerProfiler profiler)
    {
        if (result.GeneratedCode is null || result.OutputName is null) return TrimDumpState(result, dumpOptions);
        var outputPath = Path.Combine(outputDirectory, result.OutputName);
        ArtifactFiles.WriteUtf8(outputPath, result.GeneratedCode);
        profiler.AddOutputBytes(Encoding.UTF8.GetByteCount(result.GeneratedCode));
        return dumpOptions?.Stages.HasFlag(CompilerDumpStage.CSharpIr) == true
            ? result
            : TrimDumpState(result with { GeneratedCode = null }, dumpOptions);
    }

    private static void WriteMigrationIrStreaming(
        string path,
        string schemaVersion,
        CompilerIdentity identity,
        string irVersion,
        string generationMode,
        string compatibilityProfile,
        PackageGraph packageGraph,
        IReadOnlyList<string> inputFragments,
        CompatibilityRule[] compatibilityRules,
        ConverterOutput[] outputs)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan);
        using var writer = CreateJsonWriter(stream);
        writer.WriteStartObject();
        writer.WriteString("schemaVersion", schemaVersion);
        writer.WritePropertyName("identity");
        JsonSerializer.Serialize(writer, identity, ArtifactFiles.JsonOptions);
        writer.WriteString("irVersion", irVersion);
        writer.WriteString("generationMode", generationMode);
        writer.WriteString("compatibilityProfile", compatibilityProfile);
        writer.WritePropertyName("packageGraph");
        JsonSerializer.Serialize(writer, packageGraph, ArtifactFiles.JsonOptions);
        writer.WritePropertyName("inputs");
        writer.WriteStartArray();
        foreach (var fragment in inputFragments)
        {
            using var document = JsonDocument.Parse(
                File.ReadAllBytes(fragment),
                new JsonDocumentOptions { MaxDepth = ArtifactFiles.JsonOptions.MaxDepth });
            document.RootElement.WriteTo(writer);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("compatibilityRules");
        JsonSerializer.Serialize(writer, compatibilityRules, ArtifactFiles.JsonOptions);
        writer.WritePropertyName("outputs");
        JsonSerializer.Serialize(writer, outputs, ArtifactFiles.JsonOptions);
        writer.WriteEndObject();
        writer.Flush();
        stream.WriteByte((byte)'\n');
    }

    private static void WriteJsonStreaming<T>(string path, T value, bool trailingNewLine = true)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan);
        using (var writer = CreateJsonWriter(stream))
        {
            JsonSerializer.Serialize(writer, value, ArtifactFiles.JsonOptions);
            writer.Flush();
        }
        if (trailingNewLine) stream.WriteByte((byte)'\n');
    }

    private static Utf8JsonWriter CreateJsonWriter(Stream stream) => new(stream, new JsonWriterOptions
    {
        Indented = true,
        Encoder = ArtifactFiles.JsonOptions.Encoder,
        NewLine = "\n",
    });

    internal static string ComputeWorkspaceId(string manifestPath) => WorkspaceFingerprint.Compute(manifestPath);

    private static InputCompileResult CompileInput(
        SelectionInput input,
        SelectionManifest manifest,
        CompilerProfile profile,
        string manifestDirectory,
        PackageGraph packageGraph,
        string analyzerJson,
        IReadOnlyList<MetadataReference> platformReferences,
        CompilationContext compilationContext,
        CompilerProfiler profiler)
    {
        using var inputPhase = profiler.MeasureLibrary("analyze-and-core-lowering", input.Path);
        var localDiagnostics = new List<ConverterDiagnostic>();
        var inputPath = ResolveInputPath(manifest, manifestDirectory, input.Path);
        var inputSource = File.ReadAllText(inputPath);
        profiler.AddInputBytes(Encoding.UTF8.GetByteCount(inputSource));
        AnalyzerOutput ir;
        using (profiler.MeasureLibrary("protocol-deserialize", input.Path))
        {
            ir = AnalyzerProtocolReader.Read(input.Path, analyzerJson);
        }
        var library = input.Library ?? CanonicalLibraryUri(manifest, manifestDirectory, packageGraph.RootPackage, inputPath, input.Path);
        var sourcePackage = PackageNameFromLibrary(library, packageGraph.RootPackage);
        foreach (var item in ir.Diagnostics)
        {
            var runtimeBoundary = profile.IsFrameworkGraph && input.EmissionMode == "graph-only" &&
                item.Code == "URI_DOES_NOT_EXIST" && item.Message.Contains("dart:ui", StringComparison.Ordinal);
            var documentationOnlyImport = profile.IsFrameworkGraph &&
                item.Code == "URI_DOES_NOT_EXIST_IN_DOC_IMPORT" &&
                item.Message.Contains("package:flutter_test/flutter_test.dart", StringComparison.Ordinal);
            var inspectorSdkBoundary = manifest.FrameworkMilestone is "G5-3" or "G5-4" &&
                library == "package:flutter/src/widgets/widget_inspector.dart" &&
                item.Code is "UNDEFINED_CLASS" or "UNDEFINED_PREFIXED_NAME" or "NON_TYPE_AS_TYPE_ARGUMENT" &&
                item.Message.Contains("CreationLocation", StringComparison.Ordinal);
            localDiagnostics.Add(Diagnostic(
                runtimeBoundary ? "DOTF0012" : documentationOnlyImport ? "DOTF0013" : inspectorSdkBoundary ? "DOTF0015" : "DOTCONV001",
                documentationOnlyImport ? "info" : runtimeBoundary || inspectorSdkBoundary ? "warning" : item.Severity,
                sourcePackage,
                library,
                input.Path,
                item.Offset,
                item.Length,
                null,
                documentationOnlyImport
                    ? "A dartdoc-only flutter_test reference is tooling-owned and excluded from the product dependency graph."
                    : runtimeBoundary
                    ? "Pinned graph fragment reaches dart:ui, which is an explicit Doroti runtime-binding boundary."
                    : inspectorSdkBoundary
                    ? "Flutter inspector CreationLocation metadata requires the engine-patched Dart SDK and is debug-tooling-owned."
                    : $"Dart analyzer {item.Code}: {item.Message}",
                documentationOnlyImport ? "documentation-only-import" : runtimeBoundary ? "runtime-binding-boundary" : inspectorSdkBoundary ? "engine-sdk-debug-tooling-boundary" : "analyzer-diagnostic",
                documentationOnlyImport ? "excluded-with-owner" : runtimeBoundary ? "runtime-bound" : inspectorSdkBoundary ? "debug-tooling-owned" : "diagnostic-only",
                documentationOnlyImport
                    ? "Keep flutter_test out of product references; resolve it only in documentation tooling."
                    : runtimeBoundary
                    ? "Keep the fragment in the graph and bind dart:ui symbols before selecting it for emission."
                    : inspectorSdkBoundary
                    ? "Preserve the inspector API while binding CreationLocation only in the optional engine debug-tooling adapter."
                    : "Fix the Dart source before generation."));
        }

        var selectAll = input.Symbols.Length == 1 && input.Symbols[0] == "*";
        var selected = (selectAll ? ir.Declarations.Select(item => item.Name) : input.Symbols).ToHashSet(StringComparer.Ordinal);
        var boundarySymbols = (input.BoundarySymbols ?? []).ToHashSet(StringComparer.Ordinal);
        var declarations = ir.Declarations.Where(item => selected.Contains(item.Name)).ToArray();
        var declarationsForLowering = declarations.Where(item => !boundarySymbols.Contains(item.Name)).ToArray();
        foreach (var boundary in declarations.Where(item => boundarySymbols.Contains(item.Name)).OrderBy(item => item.Offset))
        {
            localDiagnostics.Add(Diagnostic(
                "DOTF0014", "info", sourcePackage, library, input.Path, boundary.Offset, boundary.Length, boundary.Name,
                $"Framework declaration is owned by an explicit host/platform boundary: {boundary.Name}",
                "explicit-platform-boundary", "excluded-with-owner",
                "Keep the declaration in the disposition ledger and implement it in the matching host capability.",
                boundary.Element?.CanonicalId, [library, boundary.Name]));
        }
        foreach (var missing in selected.Except(declarations.Select(item => item.Name), StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal))
        {
            localDiagnostics.Add(Diagnostic(
                "DOTCONV002", "error", sourcePackage, library, input.Path, 0, 0, missing,
                $"Selected symbol was not found: {missing}", "selection-drift", "unsupported", "Correct the selection manifest."));
        }
        if (profile.EnableTypedSemanticCompiler && input.EmissionMode != "graph-only")
        {
            foreach (var omitted in ir.Declarations.Where(item => !selected.Contains(item.Name)).OrderBy(item => item.Offset))
            {
                localDiagnostics.Add(Diagnostic(
                    "DOTF0002", "error", sourcePackage, library, input.Path, omitted.Offset, omitted.Length, omitted.Name,
                    $"Framework closure declaration was not selected: {omitted.Name}", "silent-declaration-omission",
                    "blocked", "Select every declaration in the library closure or move the library boundary.",
                    omitted.Element?.CanonicalId, [library, omitted.Name]));
            }
        }

        var migrationDeclarations = declarations
            .OrderBy(item => item.Offset)
            .ThenBy(item => item.Name, StringComparer.Ordinal)
            .Select(item => ToMigrationDeclaration(item, library))
            .ToArray();
        var loweredMigrationDeclarations = declarationsForLowering
            .OrderBy(item => item.Offset)
            .ThenBy(item => item.Name, StringComparer.Ordinal)
            .Select(item => ToMigrationDeclaration(item, library))
            .ToArray();
        var migrationLibraryGraph = ToMigrationLibraryGraph(
            ir.LibraryGraph,
            library,
            ir.Imports,
            manifest,
            manifestDirectory,
            packageGraph.RootPackage);
        var normalizedImports = ir.Imports
            .Select(item => NormalizeImport(item, manifest, manifestDirectory, packageGraph.RootPackage))
            .OrderBy(item => item, StringComparer.Ordinal)
            .ToArray();
        var irInput = new MigrationIrInput(
            input.Path,
            library,
            normalizedImports,
            ir.Directives.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            selected.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            migrationDeclarations,
            migrationLibraryGraph);
        DartResolvedDeclaration[] dartDeclarations;
        using (profiler.MeasureLibrary("migration-to-dart", input.Path))
        {
            dartDeclarations = profile.EnableTypedSemanticCompiler
                ? DartIrBuilder.Build(loweredMigrationDeclarations, input.Path)
                : [];
        }
        CoreResolvedDeclaration[] coreDeclarations;
        using (profiler.MeasureLibrary("dart-to-core", input.Path))
        {
            coreDeclarations = DartToCoreLowerer.Lower(dartDeclarations);
        }

        if (input.EmissionMode == "graph-only")
        {
            if (!profile.IsFrameworkGraph)
            {
                localDiagnostics.Add(Diagnostic(
                    "DOTF0010", "error", sourcePackage, library, input.Path, 0, 0, null,
                    "graph-only inputs require the general flutter-framework profile.", "invalid-selection-mode",
                    "blocked", "Use emissionMode generate or select flutter-framework with a milestone."));
            }

            return new(
                irInput,
                ir,
                CreateFrameworkCoverageInput(input.Path, library, ir, migrationDeclarations, normalizedImports),
                localDiagnostics.ToArray(),
                null,
                null,
                [],
                declarationsForLowering.Select(item => item.Name).ToArray(),
                dartDeclarations,
                coreDeclarations,
                GraphOnly: true);
        }

        var sourceDiagnostics = new List<ConverterDiagnostic>();
        GeneratedFile generated;
        FrameworkCoverageInput? coverage = null;
        if (profile.EnableTypedSemanticCompiler)
        {
            if (!string.Equals(ir.AnalysisMode, "resolved", StringComparison.Ordinal))
            {
                localDiagnostics.Add(Diagnostic(
                    "DOTF0003", "error", sourcePackage, library, input.Path, 0, 0, null,
                    "Framework semantic compilation requires a resolved analyzer graph.", "syntax-only-framework-analysis",
                    "blocked", "Resolve the selected library and its dependency closure with the pinned analyzer SDK.",
                    null, [library]));
            }
            if (profile.IsFrameworkGraph)
            {
                return new(
                    irInput,
                    ir,
                    CreateFrameworkCoverageInput(input.Path, library, ir, migrationDeclarations, normalizedImports),
                    localDiagnostics.ToArray(),
                    null,
                    null,
                    [],
                    declarations.Select(item => item.Name).ToArray(),
                    dartDeclarations,
                    coreDeclarations,
                    GraphOnly: false);
            }
            var effectiveCompilationContext = ReferenceEquals(compilationContext, CompilationContext.Empty)
                ? CompilationContext.Create(coreDeclarations)
                : compilationContext;
            generated = new FrameworkCSharpLowerer(new(effectiveCompilationContext, library, coreDeclarations)).Generate(
                profile.IsFrameworkGraph ? FrameworkNamespace(manifest.OutputNamespace, input.Path) : manifest.OutputNamespace,
                sourcePackage,
                library,
                input.Path,
                sourceDiagnostics);
            coverage = CreateFrameworkCoverageInput(input.Path, library, ir, migrationDeclarations, normalizedImports);
        }
        else
        {
            var loweredDeclarations = FixtureHistoryLoweringPipeline.Lower(migrationDeclarations, inputSource);
            generated = GenerateFile(
                manifest.OutputNamespace,
                sourcePackage,
                library,
                input.Path,
                loweredDeclarations,
                profile,
                inputSource,
                sourceDiagnostics);
        }
        localDiagnostics.AddRange(sourceDiagnostics);

        var outputName = profile.IsFrameworkGraph
            ? ArtifactFiles.NormalizePath(Path.Combine("projects", FrameworkPartition(input.Path), GeneratedOutputName(input.Path)))
            : GeneratedOutputName(input.Path);
        var mapped = generated.Mappings.Select(item => item with { GeneratedFile = outputName }).ToArray();

        var syntaxDiagnostics = CSharpSyntaxTree.ParseText(generated.Code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))
            .GetDiagnostics()
            .Where(item => item.Severity == DiagnosticSeverity.Error)
            .OrderBy(item => item.Location.SourceSpan.Start)
            .ThenBy(item => item.Id, StringComparer.Ordinal);
        foreach (var item in syntaxDiagnostics)
        {
            localDiagnostics.Add(Diagnostic(
                "DOTCONV900", "error", packageGraph.RootPackage, library, input.Path,
                item.Location.SourceSpan.Start, item.Location.SourceSpan.Length, null,
                $"Generated C# {item.Id}: {item.GetMessage()}", "emitter-invalid-syntax", "diagnostic-only",
                "Update the lowering rule; do not consume this generated solution."));
        }

        var compilationDiagnostics = profile.IsFrameworkGraph
            ? Array.Empty<Microsoft.CodeAnalysis.Diagnostic>()
            : CSharpCompilation.Create(
                "Doroti.GeneratedDraft.Validation",
                new[] { CSharpSyntaxTree.ParseText(generated.Code) },
                platformReferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 9999))
            .GetDiagnostics()
            .Where(item => item.Severity == DiagnosticSeverity.Error)
            .OrderBy(item => item.Location.SourceSpan.Start)
            .ThenBy(item => item.Id, StringComparer.Ordinal).ToArray();
        foreach (var item in compilationDiagnostics)
        {
            localDiagnostics.Add(Diagnostic(
                "DOTCONV901", "error", packageGraph.RootPackage, library, input.Path,
                item.Location.SourceSpan.Start, item.Location.SourceSpan.Length, null,
                $"Generated C# compile {item.Id}: {item.GetMessage()}", "emitter-compile-failure", "diagnostic-only",
                "Update the lowering rule; do not consume this generated solution."));
        }

        return new(
            irInput,
            ir,
            coverage,
            localDiagnostics.ToArray(),
            outputName,
            generated.Code,
            mapped,
            declarations.Select(item => item.Name).ToArray(),
            dartDeclarations,
            coreDeclarations,
            GraphOnly: false);
    }

    private static InputCompileResult CompileResolvedFramework(
        InputCompileResult resolved,
        SelectionManifest manifest,
        CompilerProfile profile,
        PackageGraph packageGraph,
        IReadOnlyList<MetadataReference> platformReferences,
        CompilationContext compilationContext)
    {
        if (resolved.GraphOnly)
        {
            return resolved;
        }

        var inputPath = resolved.IrInput.Path;
        var library = resolved.IrInput.Library;
        var sourcePackage = PackageNameFromLibrary(library, packageGraph.RootPackage);
        var localDiagnostics = resolved.Diagnostics.ToList();
        var sourceDiagnostics = new List<ConverterDiagnostic>();
        var generated = new FrameworkCSharpLowerer(
            new LibraryCompilationContext(compilationContext, library, resolved.CoreResolvedDeclarations)).Generate(
                FrameworkNamespace(manifest.OutputNamespace, inputPath),
                sourcePackage,
                library,
                inputPath,
                sourceDiagnostics);
        localDiagnostics.AddRange(sourceDiagnostics);

        var outputName = ArtifactFiles.NormalizePath(
            Path.Combine("projects", FrameworkPartition(inputPath), GeneratedOutputName(inputPath)));
        var mappings = generated.Mappings
            .Select(item => item with { GeneratedFile = outputName })
            .ToArray();
        AddGeneratedCodeDiagnostics(
            localDiagnostics,
            generated.Code,
            packageGraph.RootPackage,
            library,
            inputPath,
            profile,
            platformReferences);

        return resolved with
        {
            Diagnostics = localDiagnostics.ToArray(),
            OutputName = outputName,
            GeneratedCode = generated.Code,
            Mappings = mappings,
        };
    }

    private static void AddGeneratedCodeDiagnostics(
        List<ConverterDiagnostic> diagnostics,
        string generatedCode,
        string rootPackage,
        string library,
        string inputPath,
        CompilerProfile profile,
        IReadOnlyList<MetadataReference> platformReferences)
    {
        var syntaxDiagnostics = CSharpSyntaxTree.ParseText(
                generatedCode,
                CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))
            .GetDiagnostics()
            .Where(item => item.Severity == DiagnosticSeverity.Error)
            .OrderBy(item => item.Location.SourceSpan.Start)
            .ThenBy(item => item.Id, StringComparer.Ordinal);
        foreach (var item in syntaxDiagnostics)
        {
            diagnostics.Add(Diagnostic(
                "DOTCONV900", "error", rootPackage, library, inputPath,
                item.Location.SourceSpan.Start, item.Location.SourceSpan.Length, null,
                $"Generated C# {item.Id}: {item.GetMessage()}", "emitter-invalid-syntax", "diagnostic-only",
                "Update the lowering rule; do not consume this generated solution."));
        }

        var compilationDiagnostics = profile.IsFrameworkGraph
            ? Array.Empty<Microsoft.CodeAnalysis.Diagnostic>()
            : CSharpCompilation.Create(
                    "Doroti.GeneratedDraft.Validation",
                    [CSharpSyntaxTree.ParseText(generatedCode)],
                    platformReferences,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 9999))
                .GetDiagnostics()
                .Where(item => item.Severity == DiagnosticSeverity.Error)
                .OrderBy(item => item.Location.SourceSpan.Start)
                .ThenBy(item => item.Id, StringComparer.Ordinal)
                .ToArray();
        foreach (var item in compilationDiagnostics)
        {
            diagnostics.Add(Diagnostic(
                "DOTCONV901", "error", rootPackage, library, inputPath,
                item.Location.SourceSpan.Start, item.Location.SourceSpan.Length, null,
                $"Generated C# compile {item.Id}: {item.GetMessage()}", "emitter-compile-failure", "diagnostic-only",
                "Update the lowering rule; do not consume this generated solution."));
        }
    }

    private static IReadOnlyList<MetadataReference> CreatePlatformReferences(CompilerProfile profile)
    {
        var platformReferences = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToList();
        if (profile.ReferenceRuntimeBindings)
        {
            var runtimeDirectory = Path.GetDirectoryName(typeof(DartRuntimePrimitives).Assembly.Location)!;
            platformReferences.AddRange(Directory.GetFiles(runtimeDirectory, "Doroti.*.dll")
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => MetadataReference.CreateFromFile(path)));
        }
        return platformReferences;
    }

    private static PackageGraph CreatePackageGraph(SelectionManifest manifest, string manifestDirectory)
    {
        if (string.IsNullOrWhiteSpace(manifest.PackageRoot))
        {
            return new("doroti.package-graph/v1", "selection-fixture", [new("selection-fixture", "0.0.0", "selection", [])]);
        }

        var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
        if (manifest.Application is not null)
        {
            var configPath = Path.Combine(packageRoot, ".dart_tool", "package_config.json");
            using var config = JsonDocument.Parse(File.ReadAllText(configPath));
            var applicationPackages = config.RootElement.GetProperty("packages").EnumerateArray()
                .Select(item => new PackageGraphNode(
                    item.GetProperty("name").GetString()!,
                    item.TryGetProperty("version", out var version) ? version.GetString() ?? "unknown" : "unknown",
                    "package-config",
                    []))
                .OrderBy(item => item.Name, StringComparer.Ordinal)
                .ToArray();
            var slash = manifest.EntryPoint!.IndexOf('/', "package:".Length);
            var applicationRootPackage = manifest.EntryPoint["package:".Length..slash];
            return new("doroti.package-graph/v1", applicationRootPackage, applicationPackages);
        }
        var result = ProcessRunner.Run("dart", ["pub", "deps", "--json"], packageRoot);
        result.EnsureSuccess("Resolved Dart package graph (run `dart pub get` in the selected package first)");
        using var document = JsonDocument.Parse(result.StandardOutput);
        var rootPackage = document.RootElement.GetProperty("root").GetString()
            ?? throw new InvalidDataException("Dart package graph root is missing.");
        var packages = document.RootElement.GetProperty("packages")
            .EnumerateArray()
            .Select(item => new PackageGraphNode(
                item.GetProperty("name").GetString() ?? throw new InvalidDataException("Package name is missing."),
                item.TryGetProperty("version", out var version) ? version.GetString() ?? "unknown" : "unknown",
                item.TryGetProperty("source", out var source) ? source.GetString() ?? "unknown" : "unknown",
                item.TryGetProperty("dependencies", out var dependencies)
                    ? dependencies.EnumerateArray().Select(value => value.GetString()!).OrderBy(value => value, StringComparer.Ordinal).ToArray()
                    : []))
            .OrderBy(item => item.Name, StringComparer.Ordinal)
            .ToArray();
        return new("doroti.package-graph/v1", rootPackage, packages);
    }

    private static string ResolveInputPath(SelectionManifest manifest, string manifestDirectory, string logicalPath) =>
        CompilerInputResolver.Resolve(manifest, manifestDirectory, logicalPath);

    private static string ResolvePackageRoot(SelectionManifest manifest, string manifestDirectory, string packageName)
    {
        if (string.IsNullOrWhiteSpace(manifest.PackageRoot))
        {
            throw new InvalidDataException("C5 package provenance requires packageRoot.");
        }
        var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
        var configPath = Path.Combine(packageRoot, ".dart_tool", "package_config.json");
        using var config = JsonDocument.Parse(File.ReadAllText(configPath));
        var package = config.RootElement.GetProperty("packages").EnumerateArray()
            .Single(item => string.Equals(item.GetProperty("name").GetString(), packageName, StringComparison.Ordinal));
        var configDirectoryUri = new Uri(Path.GetFullPath(configPath));
        var rootValue = package.GetProperty("rootUri").GetString()!;
        return new Uri(configDirectoryUri, rootValue.EndsWith("/", StringComparison.Ordinal) ? rootValue : rootValue + "/").LocalPath.TrimEnd(Path.DirectorySeparatorChar);
    }

    private static string PrepareC5AnalyzerInput(
        SelectionManifest manifest,
        string manifestDirectory,
        string logicalPath,
        string inputPath,
        string inputSource)
    {
        if (!logicalPath.StartsWith("package:", StringComparison.Ordinal) ||
            !inputSource.Contains("import 'package:", StringComparison.Ordinal))
        {
            return inputPath;
        }
        var packageRoot = Path.GetFullPath(manifest.PackageRoot!, manifestDirectory);
        var packageSlash = logicalPath.IndexOf('/', "package:".Length);
        var packageName = logicalPath["package:".Length..packageSlash];
        var packageRelative = logicalPath[(packageSlash + 1)..].Replace('/', Path.DirectorySeparatorChar);
        var sourceRoot = ResolvePackageRoot(manifest, manifestDirectory, packageName);
        var sourceLib = Path.Combine(sourceRoot, "lib");
        var analysisLib = Path.Combine(packageRoot, ".dart_tool", "doroti_analysis", packageName);
        var gate = C5PackagePrepareLocks.GetOrAdd(packageName, static _ => new object());
        lock (gate)
        {
            foreach (var source in Directory.EnumerateFiles(sourceLib, "*.dart", SearchOption.AllDirectories))
            {
                var target = Path.Combine(analysisLib, Path.GetRelativePath(sourceLib, source));
                ArtifactFiles.WriteUtf8(target, File.ReadAllText(source));
            }
            var analysisPath = Path.Combine(analysisLib, packageRelative);
            ArtifactFiles.WriteUtf8(analysisPath, inputSource);
            return analysisPath;
        }
    }

    private static string PackageNameFromLibrary(string library, string fallback) => library.StartsWith("package:", StringComparison.Ordinal)
        ? library["package:".Length..library.IndexOf('/', "package:".Length)]
        : fallback;

    private static string GeneratedOutputName(string logicalPath)
    {
        if (!logicalPath.StartsWith("package:", StringComparison.Ordinal))
        {
            return Path.GetFileNameWithoutExtension(logicalPath) + ".g.cs";
        }
        var normalized = logicalPath["package:".Length..];
        return SafeIdentifier(Path.ChangeExtension(normalized, null)!) + ".g.cs";
    }

    private static string CanonicalLibraryUri(
        SelectionManifest manifest,
        string manifestDirectory,
        string packageName,
        string inputPath,
        string logicalPath)
    {
        if (logicalPath.StartsWith("package:", StringComparison.Ordinal))
        {
            return logicalPath;
        }
        if (!string.IsNullOrWhiteSpace(manifest.PackageRoot))
        {
            var packageRoot = Path.GetFullPath(manifest.PackageRoot, manifestDirectory);
            var libRoot = Path.Combine(packageRoot, "lib");
            var relative = Path.GetRelativePath(libRoot, inputPath);
            if (!relative.StartsWith("..", StringComparison.Ordinal))
            {
                return $"package:{packageName}/{ArtifactFiles.NormalizePath(relative)}";
            }
        }
        return $"selection:{ArtifactFiles.NormalizePath(logicalPath)}";
    }

    private static string NormalizeImport(string import, SelectionManifest manifest, string manifestDirectory, string packageName)
    {
        if (!import.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return import;
        }
        var path = new Uri(import).LocalPath;
        var normalizedPath = ArtifactFiles.NormalizePath(path);
        const string flutterLibraryMarker = "/packages/flutter/lib/";
        var flutterMarker = normalizedPath.IndexOf(flutterLibraryMarker, StringComparison.OrdinalIgnoreCase);
        if (flutterMarker >= 0)
        {
            return "package:flutter/" + normalizedPath[(flutterMarker + flutterLibraryMarker.Length)..];
        }
        return CanonicalLibraryUri(manifest, manifestDirectory, packageName, path, Path.GetFileName(path));
    }

    private static MigrationIrDeclaration ToMigrationDeclaration(AnalyzerDeclaration declaration, string library) => new(
        declaration.Kind,
        declaration.Name,
        declaration.Offset,
        declaration.Length,
        NormalizeElement(declaration.Element, library, declaration.Name),
        declaration.Members
            .OrderBy(item => item.Offset)
            .Select(item => new MigrationIrMember(
                item.Kind,
                item.Name,
                item.Offset,
                item.Length,
                NormalizeElement(item.Element, library, $"{declaration.Name}.{item.Name}"),
                item.Statements
                    .OrderBy(statement => statement.Offset)
                    .Select(statement => new MigrationIrStatement(statement.Kind, statement.Offset, statement.Length, statement.Source))
                    .ToArray(),
                NormalizeNode(item.Ast, library),
                item.IsStatic,
                item.IsFinal,
                item.IsConst,
                item.IsLate,
                item.IsAbstract,
                item.IsGetter,
                item.IsSetter,
                item.IsOperator,
                item.IsFactory))
            .ToArray(),
        NormalizeNode(declaration.Ast, library));

    private static MigrationIrNode? NormalizeNode(AnalyzerAstNode? node, string library)
    {
        if (node is null)
        {
            return null;
        }
        var elementId = node.ElementId is null ? null : NormalizeElementId(node.ElementId, library);
        return new(
            node.Kind,
            node.AnalyzerKind,
            node.Category,
            node.Offset,
            node.Length,
            node.StaticType,
            elementId,
            new Dictionary<string, string?>(node.Properties, StringComparer.Ordinal),
            node.Children.Select(child => NormalizeNode(child, library)!).ToArray());
    }

    private static string NormalizeElementId(string elementId, string library)
    {
        var marker = elementId.IndexOf('#');
        if (marker < 0 || !elementId.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
        {
            return elementId;
        }
        return library + elementId[marker..];
    }

    private static MigrationIrLibraryGraph ToMigrationLibraryGraph(
        AnalyzerLibraryGraph? graph,
        string library,
        string[] imports,
        SelectionManifest manifest,
        string manifestDirectory,
        string packageName)
    {
        if (graph is null)
        {
            return new(library, [new(library, [])], imports.OrderBy(item => item, StringComparer.Ordinal).ToArray());
        }
        return new(
            library,
            graph.Fragments.Select(fragment => new MigrationIrLibraryFragment(
                string.Equals(fragment.Uri, graph.Library, StringComparison.Ordinal)
                    ? library
                    : NormalizeImport(fragment.Uri, manifest, manifestDirectory, packageName),
                fragment.Declarations.Select(item => NormalizeElementId(item, library)).OrderBy(item => item, StringComparer.Ordinal).ToArray(),
                fragment.IsDefining,
                string.Equals(fragment.OwnerLibrary, graph.Library, StringComparison.Ordinal)
                    ? library
                    : fragment.OwnerLibrary is null
                        ? library
                        : NormalizeImport(fragment.OwnerLibrary, manifest, manifestDirectory, packageName)))
                .OrderBy(item => item.Uri, StringComparer.Ordinal)
                .ToArray(),
            graph.Imports.OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            graph.ImportDetails?.Select(item => new MigrationIrLibraryImport(
                    NormalizeImport(item.Uri, manifest, manifestDirectory, packageName), item.Prefix, item.IsSynthetic))
                .OrderBy(item => item.Uri, StringComparer.Ordinal).ToArray(),
            graph.AccessibleExtensions?.Select(item => NormalizeGraphElementId(
                    item, library, manifest, manifestDirectory, packageName))
                .OrderBy(item => item, StringComparer.Ordinal).ToArray());
    }

    private static string NormalizeGraphElementId(
        string elementId,
        string currentLibrary,
        SelectionManifest manifest,
        string manifestDirectory,
        string packageName)
    {
        var marker = elementId.IndexOf('#');
        if (marker < 0)
        {
            return elementId;
        }
        var owner = elementId[..marker];
        var normalizedOwner = owner.StartsWith("file:", StringComparison.OrdinalIgnoreCase)
            ? NormalizeImport(owner, manifest, manifestDirectory, packageName)
            : owner;
        return (normalizedOwner == currentLibrary ? currentLibrary : normalizedOwner) + elementId[marker..];
    }

    private static FrameworkCoverageInput CreateFrameworkCoverageInput(
        string source,
        string library,
        AnalyzerOutput analyzer,
        MigrationIrDeclaration[] declarations,
        string[] normalizedImports)
    {
        var nodes = declarations.SelectMany(declaration => Flatten(declaration.Ast)).ToArray();
        return new(
            source,
            library,
            analyzer.AnalysisMode ?? "unknown",
            [library, .. normalizedImports],
            declarations.Select(item => item.Element!.CanonicalId).OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            declarations.SelectMany(item => item.Members).Select(item => item.Element!.CanonicalId).OrderBy(item => item, StringComparer.Ordinal).ToArray(),
            nodes.GroupBy(item => (item.Category, item.Kind))
                .OrderBy(group => group.Key.Category, StringComparer.Ordinal)
                .ThenBy(group => group.Key.Kind, StringComparer.Ordinal)
                .Select(group => new FrameworkAstClassification(group.Key.Category, group.Key.Kind, group.Count()))
                .ToArray());
    }

    private static IEnumerable<MigrationIrNode> Flatten(MigrationIrNode? node)
    {
        if (node is null)
        {
            yield break;
        }
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in Flatten(child))
            {
                yield return descendant;
            }
        }
    }

    private static AnalyzerElement? NormalizeElement(AnalyzerElement? element, string library, string symbol) => element is null
        ? null
        : element with { CanonicalId = $"{library}#{symbol}" };

    private static ConverterDiagnostic Diagnostic(
        string code,
        string severity,
        string package,
        string library,
        string source,
        int offset,
        int length,
        string? symbol,
        string message,
        string cause,
        string supportState,
        string manualAction,
        string? canonicalElementId = null,
        string[]? dependencyPath = null) => new(
            code, severity, package, library, source, offset, length, symbol, message, cause, supportState, manualAction,
            canonicalElementId, dependencyPath);

    private static void WriteGeneratedProject(
        string outputDirectory,
        SelectionManifest manifest,
        bool requiresRuntimeBindings,
        bool isC5,
        PackageGraph packageGraph,
        CompilerIdentity identity,
        List<ConverterOutput> reportOutputs)
    {
        var assemblyName = manifest.OutputAssemblyName;
        if (!Regex.IsMatch(assemblyName, @"^[A-Za-z_][A-Za-z0-9_.]*$", RegexOptions.CultureInvariant))
        {
            throw new InvalidDataException($"Invalid generated assembly name: {assemblyName}");
        }
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "Directory.Build.props"),
            """
            <Project>
              <PropertyGroup>
                <TargetFramework>net10.0</TargetFramework>
                <LangVersion>14.0</LangVersion>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
                <Deterministic>true</Deterministic>
                <RestorePackagesWithLockFile>false</RestorePackagesWithLockFile>
              </PropertyGroup>
            </Project>
            """ + "\n");
        if (isC5)
        {
            ArtifactFiles.WriteUtf8(
                Path.Combine(outputDirectory, "Directory.Packages.props"),
                """
                <Project>
                  <PropertyGroup>
                    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
                  </PropertyGroup>
                </Project>
                """ + "\n");
        }
        var projectReference = requiresRuntimeBindings && !isC5
            ? IsPinnedF0ObjectSelection(manifest)
              ? string.Join('\n',
                    "  <ItemGroup>",
                    "    <ProjectReference Include=\"$(DorotiRepositoryRoot)\\src\\Doroti.Runtime\\Doroti.Runtime.csproj\" />",
                    "  </ItemGroup>",
                    "  <Target Name=\"RequireDorotiRepositoryRoot\" BeforeTargets=\"PrepareForBuild\" Condition=\"'$(DorotiRepositoryRoot)' == ''\">",
                    "    <Error Text=\"Generated framework projects require -p:DorotiRepositoryRoot=&lt;Doroti SDK root&gt;.\" />",
                    "  </Target>")
              : """
                <ItemGroup>
                  <ProjectReference Include="$(DorotiRepositoryRoot)\src\Doroti.Runtime\Doroti.Runtime.csproj" />
                  <ProjectReference Include="$(DorotiRepositoryRoot)\src\Doroti.Ui\Doroti.Ui.csproj" />
                </ItemGroup>
                <Target Name="RequireDorotiRepositoryRoot" BeforeTargets="PrepareForBuild" Condition="'$(DorotiRepositoryRoot)' == ''">
                  <Error Text="Generated framework projects require -p:DorotiRepositoryRoot=&lt;Doroti SDK root&gt;." />
                </Target>
              """
            : string.Empty;
        var packageMetadata = isC5
            ? $"""
                  <IsPackable>true</IsPackable>
                  <PackageId>{manifest.PackageId}</PackageId>
                  <Version>{manifest.PackageVersion}</Version>
                  <Description>Doroti C5 generated package for {manifest.SourcePackage} {manifest.SourceVersion}.</Description>
                  <Authors>Doroti compiler</Authors>
                  <PackageTags>doroti;dart;flutter;generated;{manifest.PackageTier}</PackageTags>
                  <RepositoryUrl>https://github.com/Dynaruid/DorotiLab</RepositoryUrl>
                  <RepositoryType>git</RepositoryType>
                  <IncludeSymbols>true</IncludeSymbols>
                  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
                  <DebugType>portable</DebugType>
                  <DebugSymbols>true</DebugSymbols>
                  <PackageReadmeFile>README.md</PackageReadmeFile>
                """
            : string.Empty;
        var packageReferences = isC5
            ? $"""
                <ItemGroup>
                {(string.Equals(manifest.PackageTier, "A", StringComparison.Ordinal) ? string.Empty : $"  <PackageReference Include=\"Doroti.Runtime\" Version=\"[{identity.RuntimeBindingVersion}]\" Condition=\"'$(DorotiRepositoryRoot)' == ''\" />\n  <PackageReference Include=\"Doroti.Ui\" Version=\"[{identity.RuntimeBindingVersion}]\" Condition=\"'$(DorotiRepositoryRoot)' == ''\" />\n  <ProjectReference Include=\"$(DorotiRepositoryRoot)\\src\\Doroti.Runtime\\Doroti.Runtime.csproj\" Condition=\"'$(DorotiRepositoryRoot)' != ''\" />\n  <ProjectReference Include=\"$(DorotiRepositoryRoot)\\src\\Doroti.Ui\\Doroti.Ui.csproj\" Condition=\"'$(DorotiRepositoryRoot)' != ''\" />")}
                </ItemGroup>
                <ItemGroup>
                  <None Include="README.md" Pack="true" PackagePath="/" />
                  <None Include="PACKAGE-LICENSE.txt" Pack="true" PackagePath="licenses/{manifest.SourcePackage}.txt" />
                  <None Include="converter-report.json" Pack="true" PackagePath="doroti/" />
                  <None Include="migration-ir.json" Pack="true" PackagePath="doroti/" />
                  <None Include="source-map.json" Pack="true" PackagePath="doroti/" />
                  <None Include="package-release.json" Pack="true" PackagePath="doroti/" />
                </ItemGroup>
              """
            : projectReference;
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, assemblyName + ".csproj"),
            $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <AssemblyName>{assemblyName}</AssemblyName>
            {packageMetadata}
              </PropertyGroup>
            {packageReferences}
            </Project>
            """ + "\n");
        _ = packageGraph;
        _ = reportOutputs;
    }

    private static bool IsPinnedF0ObjectSelection(SelectionManifest manifest) =>
        string.Equals(manifest.CompatibilityProfile, "flutter-framework-f0", StringComparison.Ordinal) &&
        manifest.Inputs.Length == 1 &&
        manifest.Inputs[0].Path.Replace('\\', '/').EndsWith("/foundation/object.dart", StringComparison.Ordinal) &&
        manifest.Inputs[0].Symbols.SequenceEqual(["objectRuntimeType"], StringComparer.Ordinal);

    private static void WritePackageRelease(
        string outputDirectory,
        SelectionManifest manifest,
        CompilerIdentity identity,
        PackageGraph packageGraph,
        ConverterReport report,
        string manifestDirectory)
    {
        var sourceNode = packageGraph.Packages.SingleOrDefault(item => string.Equals(item.Name, manifest.SourcePackage, StringComparison.Ordinal))
            ?? throw new InvalidDataException($"C5 source package is absent from the resolved lock graph: {manifest.SourcePackage}");
        if (!string.Equals(sourceNode.Version, manifest.SourceVersion, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"C5 source package version drift: selected {manifest.SourcePackage} {manifest.SourceVersion}, resolved {sourceNode.Version}.");
        }
        var sourceRoot = ResolvePackageRoot(manifest, manifestDirectory, manifest.SourcePackage!);
        var licensePath = new[] { "LICENSE", "LICENSE.md", "LICENSE.txt", "COPYING" }
            .Select(name => Path.Combine(sourceRoot, name))
            .FirstOrDefault(File.Exists)
            ?? throw new InvalidDataException($"C5 source package license is missing: {manifest.SourcePackage}");
        var licenseText = File.ReadAllText(licensePath).Replace("\r\n", "\n", StringComparison.Ordinal);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, "PACKAGE-LICENSE.txt"), licenseText.TrimEnd() + "\n");
        ArtifactFiles.WriteUtf8(
            Path.Combine(outputDirectory, "README.md"),
            $"""
            # {manifest.PackageId}

            Deterministic Doroti C5 output for `{manifest.SourcePackage}` `{manifest.SourceVersion}` (tier {manifest.PackageTier}).

            The package embeds compiler identity, migration IR, source map, provenance, diagnostics, and the original package license under `doroti/` and `licenses/`. Restoring, building, and running this package does not require Dart, Flutter, or the Doroti compiler.
            """ + "\n");

        var artifacts = Directory.EnumerateFiles(outputDirectory, "*", SearchOption.TopDirectoryOnly)
            .Where(path => Path.GetFileName(path) != "package-release.json")
            .OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal)
            .Select(path => new PackageReleaseArtifact(Path.GetFileName(path), ArtifactFiles.Sha256(path)))
            .ToArray();
        var inputs = manifest.Inputs.OrderBy(item => item.Path, StringComparer.Ordinal)
            .Select(item => new PackageReleaseInput(item.Path, ArtifactFiles.Sha256(ResolveInputPath(manifest, manifestDirectory, item.Path)), item.Symbols.OrderBy(value => value, StringComparer.Ordinal).ToArray()))
            .ToArray();
        ArtifactFiles.WriteJson(
            Path.Combine(outputDirectory, "package-release.json"),
            new PackageReleaseDocument(
                "doroti.package-release/v1",
                manifest.PackageId!,
                manifest.PackageVersion!,
                manifest.PackageTier!,
                new(manifest.SourcePackage!, manifest.SourceVersion!, manifest.SourceLicense!, ArtifactFiles.Sha256(licensePath), sourceNode.Source),
                identity,
                packageGraph,
                inputs,
                artifacts,
                report.Diagnostics,
                report.Success,
                "package-reference",
                "not-required-after-generation"));
    }

    private static GeneratedFile GenerateFile(
        string outputNamespace,
        string package,
        string library,
        string inputPath,
        LoweredDeclaration[] declarations,
        CompilerProfile profile,
        string inputSource,
        List<ConverterDiagnostic> diagnostics)
    {
        var builder = new StringBuilder();
        var mappings = new List<SourceMapEntry>();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine($"// Doroti converter {CompilerVersions.Converter}; source: {ArtifactFiles.NormalizePath(inputPath)}");
        builder.AppendLine(profile.EnablePackageLowering
            ? "// C5 deterministic package output; trace through embedded source-map.json."
            : "// REVIEW REQUIRED: generated migration drafts are not production source.");
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Threading.Tasks;");
        if (profile.EnableCollectionLowering)
        {
            builder.AppendLine("using Doroti.Runtime;");
        }
        builder.AppendLine();
        builder.AppendLine($"namespace {outputNamespace};");
        builder.AppendLine();

        foreach (var declaration in declarations.OrderBy(item => item.Offset))
        {
            var generatedLineStart = CountLines(builder) + 1;
            if (profile.EmitPackagePlatformPorts)
            {
                GeneratePackagePlatformPort(builder, declaration, package, library, inputPath, diagnostics);
            }
            else if (profile.EnableAsyncNavigationLowering && ContainsPlatformChannel(declaration.Source))
            {
                GeneratePlatformPort(builder, declaration, package, library, inputPath, diagnostics);
            }
            else if (declaration.Kind.Contains("EnumDeclaration", StringComparison.Ordinal))
            {
                GenerateEnum(builder, declaration);
            }
            else if (profile.EnableAsyncNavigationLowering && declaration.Kind.Contains("MixinDeclaration", StringComparison.Ordinal))
            {
                GenerateMixin(builder, declaration);
            }
            else if (profile.EnableAsyncNavigationLowering && declaration.Kind.Contains("ExtensionDeclaration", StringComparison.Ordinal))
            {
                GenerateExtension(builder, declaration);
            }
            else if (declaration.Kind.Contains("ClassDeclaration", StringComparison.Ordinal))
            {
                if (IsFlutterFoundationKeySlice(library, declaration.Name))
                {
                    GenerateFlutterFoundationKey(builder, declaration.Name);
                }
                else
                {
                    GenerateClass(
                        builder,
                        declaration,
                        declarations,
                        package,
                        library,
                        inputPath,
                        diagnostics,
                        profile.EnableAsyncNavigationLowering,
                        profile.EnablePackageLowering);
                }
            }
            else if (profile.EnablePackageLowering && declaration.Kind.Contains("FunctionDeclaration", StringComparison.Ordinal))
            {
                GenerateTopLevelFunction(builder, declaration);
            }
            else if (profile.EnablePackageLowering && declaration.Kind.Contains("TopLevelVariableDeclaration", StringComparison.Ordinal))
            {
                GenerateTopLevelVariable(builder, declaration);
            }
            else
            {
                diagnostics.Add(Diagnostic(
                    "DOTCONV100", "error", package, library, inputPath, declaration.Offset, declaration.Length,
                    declaration.Name, $"Unsupported declaration kind: {declaration.Kind}", "unsupported-declaration",
                    "diagnostic-only", "Rewrite this declaration manually."));
                builder.AppendLine($"[Obsolete(\"DOTCONV100: unsupported Dart declaration\", true)]");
                builder.AppendLine($"internal sealed class {SafeIdentifier(declaration.Name)}_Unsupported {{ }}");
                builder.AppendLine();
            }
            mappings.Add(new(
                inputPath,
                declaration.Offset,
                declaration.Length,
                declaration.Name,
                string.Empty,
                generatedLineStart,
                CountLines(builder),
                GeneratedDeclarationShape(declaration)));
        }

        if (profile.EmitApplicationEntry)
        {
            var entry = AppEntryRegex().Match(inputSource);
            if (!entry.Success)
            {
                diagnostics.Add(Diagnostic(
                    "DOTCONV102", "error", package, library, inputPath, 0, 0, "main",
                    "C3 requires a top-level main() that calls runApp with one const or new Widget graph.",
                    "missing-or-unsupported-app-entry", "diagnostic-only",
                    "Use `void main() { runApp(const App()); }`."));
            }
            else
            {
                var generatedLineStart = CountLines(builder) + 1;
                var rootExpression = TranslateAppRoot(entry.Groups["root"].Value);
                builder.AppendLine("public static class GeneratedApplication");
                builder.AppendLine("{");
                builder.AppendLine($"    public static Widget CreateRoot() => {rootExpression};");
                builder.AppendLine("}");
                builder.AppendLine();
                mappings.Add(new(
                    inputPath,
                    entry.Index,
                    entry.Length,
                    "main",
                    string.Empty,
                    generatedLineStart,
                    CountLines(builder)));
            }
        }

        return new(builder.ToString().Replace("\r\n", "\n", StringComparison.Ordinal), mappings.ToArray());
    }

    private static bool IsFlutterFoundationKeySlice(string library, string symbol) =>
        string.Equals(library, "package:flutter/foundation.dart", StringComparison.Ordinal) &&
        symbol is "Key" or "LocalKey" or "UniqueKey" or "ValueKey";

    private static void GenerateFlutterFoundationKey(StringBuilder builder, string symbol)
    {
        switch (symbol)
        {
            case "Key":
                builder.AppendLine("public abstract partial class Key");
                builder.AppendLine("{");
                builder.AppendLine("    protected Key() { }");
                builder.AppendLine("}");
                break;
            case "LocalKey":
                builder.AppendLine("public abstract partial class LocalKey : Key");
                builder.AppendLine("{");
                builder.AppendLine("    protected LocalKey() { }");
                builder.AppendLine("}");
                break;
            case "UniqueKey":
                builder.AppendLine("public partial class UniqueKey : LocalKey");
                builder.AppendLine("{");
                builder.AppendLine("}");
                break;
            case "ValueKey":
                builder.AppendLine("public partial class ValueKey<T> : LocalKey, IEquatable<ValueKey<T>>");
                builder.AppendLine("{");
                builder.AppendLine("    public ValueKey(T value) => this.value = value;");
                builder.AppendLine();
                builder.AppendLine("    public T value { get; }");
                builder.AppendLine();
                builder.AppendLine("    public bool Equals(ValueKey<T>? other) =>");
                builder.AppendLine("        other is not null && GetType() == other.GetType() && EqualityComparer<T>.Default.Equals(value, other.value);");
                builder.AppendLine();
                builder.AppendLine("    public override bool Equals(object? obj) => obj is ValueKey<T> other && Equals(other);");
                builder.AppendLine();
                builder.AppendLine("    public override int GetHashCode() => HashCode.Combine(GetType(), value);");
                builder.AppendLine("}");
                break;
            default:
                throw new InvalidDataException($"Unknown Flutter foundation key symbol: {symbol}.");
        }
        builder.AppendLine();
    }

    private static void GenerateEnum(StringBuilder builder, LoweredDeclaration declaration)
    {
        var match = EnumRegex().Match(declaration.Source);
        if (!match.Success)
        {
            throw new InvalidDataException($"Could not parse enum {declaration.Name}.");
        }

        var values = match.Groups[2].Value.Split(',')
            .Select(item => item.Trim())
            .Where(item => item.Length > 0)
            .ToArray();
        builder.AppendLine($"public enum {match.Groups[1].Value}");
        builder.AppendLine("{");
        foreach (var value in values)
        {
            builder.AppendLine($"    {value},");
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void GenerateClass(
        StringBuilder builder,
        LoweredDeclaration declaration,
        LoweredDeclaration[] declarations,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics,
        bool enableC4,
        bool enableC5)
    {
        var match = ClassRegex().Match(DeclarationCode(declaration.Source));
        if (!match.Success)
        {
            throw new InvalidDataException($"Could not parse class {declaration.Name}.");
        }

        var modifier = match.Groups["abstract"].Success ? "abstract " : string.Empty;
        var name = match.Groups["name"].Value;
        var typeParameters = match.Groups["typeParameters"].Value;
        var inheritedTypes = new List<string>();
        if (match.Groups["base"].Success)
        {
            inheritedTypes.Add(MapType(match.Groups["base"].Value, enableC4));
        }
        if (enableC4 && match.Groups["mixins"].Success)
        {
            inheritedTypes.AddRange(match.Groups["mixins"].Value.Split(',').Select(item => MapType(item.Trim(), true)));
        }
        var baseType = inheritedTypes.Count == 0 ? string.Empty : $" : {string.Join(", ", inheritedTypes)}";
        var body = match.Groups["body"].Value;
        var unsupported = enableC4 ? C4UnsupportedRegex().Match(declaration.Source) : UnsupportedRegex().Match(declaration.Source);
        if (unsupported.Success)
        {
            diagnostics.Add(Diagnostic(
                enableC4 ? "DOTCONV441" : "DOTCONV101", "error", package, library, inputPath,
                declaration.Offset + unsupported.Index, unsupported.Length,
                declaration.Name, enableC4
                    ? $"C4 does not silently lower '{unsupported.Value}' semantics."
                    : "Unsupported mixin, factory, dynamic, native, or platform construct.",
                "unsupported-language-or-platform-semantics", "diagnostic-only",
                enableC4 ? "Use a supported typed construct or provide the diagnosed public platform port."
                    : "Rewrite lifecycle and runtime semantics manually."));
        }

        builder.AppendLine($"public {modifier}partial class {name}{typeParameters}{baseType}");
        builder.AppendLine("{");
        var fields = FieldRegex().Matches(body).Cast<Match>().ToArray();
        foreach (var field in fields)
        {
            var canWrite = !field.Groups["modifier"].Success || field.Groups["modifier"].Value == "late";
            var accessor = canWrite ? "get; set;" : "get;";
            var initializer = field.Groups["initializer"].Success ? $" = {TranslateExpression(field.Groups["initializer"].Value)};" : string.Empty;
            builder.AppendLine($"    public {MapType(field.Groups["type"].Value, enableC4)} {field.Groups["name"].Value} {{ {accessor} }}{initializer}");
        }

        if (fields.Length > 0)
        {
            builder.AppendLine();
        }

        foreach (Match constructor in ConstructorRegex(name).Matches(body))
        {
            var parameters = SplitParameters(constructor.Groups["parameters"].Value);
            var mapped = new List<string>();
            foreach (var parameter in parameters)
            {
                var thisField = Regex.Match(parameter, @"^(?:required\s+)?this\.([A-Za-z_]\w*)(?:\s*=\s*(.+))?$");
                if (thisField.Success)
                {
                    var field = fields.FirstOrDefault(item => item.Groups["name"].Value == thisField.Groups[1].Value);
                    var defaultValue = thisField.Groups[2].Success ? $" = {TranslateExpression(thisField.Groups[2].Value)}" : string.Empty;
                    mapped.Add($"{(field is null ? "object" : MapType(field.Groups["type"].Value, enableC4))} {thisField.Groups[1].Value}{defaultValue}");
                }
                else
                {
                    mapped.Add(MapParameter(parameter, enableC4));
                }
            }
            var baseInitializer = constructor.Groups["initializer"].Success
                ? TranslateConstructorInitializer(constructor.Groups["initializer"].Value)
                : string.Empty;
            builder.AppendLine($"    public {name}({string.Join(", ", mapped)}){baseInitializer}");
            builder.AppendLine("    {");
            foreach (var parameter in parameters)
            {
                var thisField = Regex.Match(parameter, @"^(?:required\s+)?this\.([A-Za-z_]\w*)(?:\s*=\s*(.+))?$");
                if (thisField.Success)
                {
                    builder.AppendLine($"        this.{thisField.Groups[1].Value} = {thisField.Groups[1].Value};");
                }
            }
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        if (enableC4)
        {
            foreach (var constructor in declaration.Members.Where(item => item.Kind == "constructor" && !ConstructorRegex(name).IsMatch(item.Source)))
            {
                diagnostics.Add(Diagnostic(
                    "DOTCONV442", "error", package, library, inputPath, constructor.Offset, constructor.Length,
                    $"{name}.{constructor.Name}", "C4 does not silently omit factory or named constructor semantics.",
                    "unsupported-constructor-semantics", "diagnostic-only",
                    "Use the supported unnamed constructor slice or add an approved named/factory constructor lowering rule."));
            }
        }

        if (enableC4 && match.Groups["mixins"].Success)
        {
            foreach (var mixinName in match.Groups["mixins"].Value.Split(',').Select(item => item.Trim()))
            {
                var mixin = declarations.Single(item => item.Name == mixinName && item.Kind.Contains("MixinDeclaration", StringComparison.Ordinal));
                foreach (var member in mixin.Members.Where(item => item.Kind == "method").OrderBy(item => item.Offset))
                {
                    var method = ExpressionMethodRegex().Match(member.Source);
                    if (!method.Success)
                    {
                        throw new InvalidDataException($"C4 supports method-only mixins with expression-bodied members; unsupported member {member.Name}.");
                    }
                    builder.AppendLine($"    public {MapType(method.Groups["return"].Value, true)} {method.Groups["name"].Value}({MapParameters(method.Groups["parameters"].Value, true)}) => {TranslateExpression(method.Groups["expression"].Value)};");
                    builder.AppendLine();
                }
            }
        }

        var bodyWithoutFields = enableC4
            ? string.Join('\n', declaration.Members.Where(item => item.Kind == "method").Select(item => item.Source))
            : FieldRegex().Replace(body, string.Empty);
        if (!enableC4)
        {
            bodyWithoutFields = ConstructorRegex(name).Replace(bodyWithoutFields, string.Empty);
        }
        foreach (Match method in ExpressionMethodRegex().Matches(bodyWithoutFields))
        {
            var overrideKeyword = method.Groups["override"].Success ? " override" : string.Empty;
            var mappedReturn = MapType(method.Groups["return"].Value, enableC4);
            var mappedExpression = TranslateExpression(method.Groups["expression"].Value);
            if (enableC4 && mappedReturn == "void")
            {
                builder.AppendLine($"    public{overrideKeyword} void {method.Groups["name"].Value}({MapParameters(method.Groups["parameters"].Value, true)}) => _ = {mappedExpression};");
            }
            else
            {
                builder.AppendLine($"    public{overrideKeyword} {mappedReturn} {method.Groups["name"].Value}({MapParameters(method.Groups["parameters"].Value, enableC4)}) => {mappedExpression};");
            }
            builder.AppendLine();
        }

        bodyWithoutFields = ExpressionMethodRegex().Replace(bodyWithoutFields, string.Empty);
        foreach (Match method in BlockMethodRegex().Matches(bodyWithoutFields))
        {
            var returnType = MapType(method.Groups["return"].Value, enableC4);
            var asyncKeyword = method.Groups["async"].Success ? " async" : string.Empty;
            var overrideKeyword = method.Groups["override"].Success ? " override" : string.Empty;
            builder.AppendLine($"    public{overrideKeyword}{asyncKeyword} {returnType} {method.Groups["name"].Value}({MapParameters(method.Groups["parameters"].Value, enableC4)})");
            builder.AppendLine("    {");
            foreach (var line in TranslateBlock(method.Groups["body"].Value, enableC4))
            {
                builder.AppendLine("        " + line);
            }
            builder.AppendLine("    }");
            builder.AppendLine();
        }

        if (enableC5 && modifier.Length > 0)
        {
            foreach (var member in declaration.Members.Where(item => item.Kind == "method"))
            {
                var abstractMethod = AbstractMethodRegex().Match(DeclarationCode(member.Source));
                if (abstractMethod.Success)
                {
                    builder.AppendLine($"    public abstract {MapType(abstractMethod.Groups["return"].Value, true)} {abstractMethod.Groups["name"].Value}({MapParameters(abstractMethod.Groups["parameters"].Value, true)});");
                }
            }
        }

        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void GenerateMixin(StringBuilder builder, LoweredDeclaration declaration)
    {
        var match = MixinRegex().Match(declaration.Source);
        if (!match.Success)
        {
            throw new InvalidDataException($"Could not parse mixin {declaration.Name}.");
        }

        builder.AppendLine($"public partial interface {match.Groups["name"].Value}");
        builder.AppendLine("{");
        foreach (var member in declaration.Members.Where(item => item.Kind == "method").OrderBy(item => item.Offset))
        {
            var method = ExpressionMethodRegex().Match(member.Source);
            if (!method.Success)
            {
                throw new InvalidDataException($"C4 supports method-only mixins with expression-bodied members; unsupported member {member.Name}.");
            }
            builder.AppendLine($"    public {MapType(method.Groups["return"].Value, true)} {method.Groups["name"].Value}({MapParameters(method.Groups["parameters"].Value, true)}) => {TranslateExpression(method.Groups["expression"].Value)};");
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void GenerateExtension(StringBuilder builder, LoweredDeclaration declaration)
    {
        var match = ExtensionRegex().Match(declaration.Source);
        if (!match.Success)
        {
            throw new InvalidDataException($"Could not parse extension {declaration.Name}.");
        }

        var targetType = MapType(match.Groups["target"].Value, true);
        builder.AppendLine($"public static partial class {match.Groups["name"].Value}");
        builder.AppendLine("{");
        foreach (var member in declaration.Members.Where(item => item.Kind == "method").OrderBy(item => item.Offset))
        {
            var method = ExpressionMethodRegex().Match(member.Source);
            if (!method.Success)
            {
                throw new InvalidDataException($"C4 supports expression-bodied extension members; unsupported member {member.Name}.");
            }
            var parameters = MapParameters(method.Groups["parameters"].Value, true);
            var separator = parameters.Length == 0 ? string.Empty : ", ";
            var expression = Regex.Replace(TranslateExpression(method.Groups["expression"].Value), @"\bthis\b", "value", RegexOptions.CultureInvariant);
            builder.AppendLine($"    public static {MapType(method.Groups["return"].Value, true)} {method.Groups["name"].Value}(this {targetType} value{separator}{parameters}) => {expression};");
        }
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static bool ContainsPlatformChannel(string source) => PlatformChannelRegex().IsMatch(source);

    private static void GenerateTopLevelFunction(StringBuilder builder, LoweredDeclaration declaration)
    {
        var match = TopLevelExpressionFunctionRegex().Match(DeclarationCode(declaration.Source));
        if (!match.Success)
        {
            throw new InvalidDataException($"C5 supports expression-bodied package functions in the first pilot slice; unsupported function {declaration.Name}.");
        }
        var typeParameters = match.Groups["typeParameters"].Value;
        builder.AppendLine($"public static partial class {SafeIdentifier(declaration.Name)}Functions");
        builder.AppendLine("{");
        builder.AppendLine($"    public static {MapType(match.Groups["return"].Value, true)} {declaration.Name}{typeParameters}({MapParameters(match.Groups["parameters"].Value, true)}) => {TranslateExpression(match.Groups["expression"].Value)};");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void GenerateTopLevelVariable(StringBuilder builder, LoweredDeclaration declaration)
    {
        var match = TopLevelVariableRegex().Match(DeclarationCode(declaration.Source));
        if (!match.Success || !string.Equals(match.Groups["name"].Value, declaration.Name, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"C5 supports typed const/final package variables in the first pilot slice; unsupported variable {declaration.Name}.");
        }
        var keyword = match.Groups["modifier"].Value == "const" ? "const" : "static readonly";
        builder.AppendLine($"public static partial class {SafeIdentifier(declaration.Name)}Value");
        builder.AppendLine("{");
        builder.AppendLine($"    public {keyword} {MapType(match.Groups["type"].Value, true)} Value = {TranslateExpression(match.Groups["expression"].Value)};");
        builder.AppendLine("}");
        builder.AppendLine();
    }

    private static void GeneratePackagePlatformPort(
        StringBuilder builder,
        LoweredDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var interfaceName = $"I{SafeIdentifier(declaration.Name)}PlatformPort";
        builder.AppendLine($"public partial interface {interfaceName}");
        builder.AppendLine("{");
        builder.AppendLine("    Future<object?> InvokeAsync(string operation, object? arguments = null);");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"public sealed partial class {SafeIdentifier(declaration.Name)}");
        builder.AppendLine("{");
        builder.AppendLine($"    private readonly {interfaceName} _port;");
        builder.AppendLine($"    public {SafeIdentifier(declaration.Name)}({interfaceName} port) => _port = port ?? throw new ArgumentNullException(nameof(port));");
        builder.AppendLine("    public Future<object?> InvokeAsync(string operation, object? arguments = null) => _port.InvokeAsync(operation, arguments);");
        builder.AppendLine("}");
        builder.AppendLine();
        diagnostics.Add(Diagnostic(
            "DOTCONV540", "warning", package, library, inputPath, declaration.Offset, declaration.Length, declaration.Name,
            $"Tier C package symbol '{declaration.Name}' requires the generated {interfaceName} implementation.",
            "native-plugin-requires-platform-port", "generated-port",
            $"Provide {interfaceName}; generated code never installs a no-op plugin implementation."));
    }

    private static void GeneratePlatformPort(
        StringBuilder builder,
        LoweredDeclaration declaration,
        string package,
        string library,
        string inputPath,
        List<ConverterDiagnostic> diagnostics)
    {
        var match = PlatformChannelRegex().Match(declaration.Source);
        var interfaceName = $"I{SafeIdentifier(declaration.Name)}PlatformPort";
        builder.AppendLine($"public partial interface {interfaceName}");
        builder.AppendLine("{");
        builder.AppendLine("    Future<object?> InvokeAsync(string method, object? arguments = null);");
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"public sealed partial class {SafeIdentifier(declaration.Name)}");
        builder.AppendLine("{");
        builder.AppendLine($"    private readonly {interfaceName} _port;");
        builder.AppendLine($"    public {SafeIdentifier(declaration.Name)}({interfaceName} port) => _port = port ?? throw new ArgumentNullException(nameof(port));");
        builder.AppendLine("    public Future<object?> InvokeAsync(string method, object? arguments = null) => _port.InvokeAsync(method, arguments);");
        builder.AppendLine("}");
        builder.AppendLine();
        diagnostics.Add(Diagnostic(
            "DOTCONV440", "warning", package, library, inputPath,
            declaration.Offset + match.Index, match.Length, declaration.Name,
            $"MethodChannel '{match.Groups["channel"].Value}' requires the generated {interfaceName} implementation.",
            "platform-channel-requires-port", "generated-port",
            $"Provide {interfaceName}; generated code does not install a silent channel stub."));
    }

    private static IEnumerable<string> TranslateBlock(string block, bool enableC4 = false)
    {
        foreach (var rawLine in block.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0)
            {
                continue;
            }
            line = Regex.Replace(line, @"^final\s+(?=[A-Za-z_]\w*\s*=)", "var ");
            line = Regex.Replace(line, @"^for\s*\(final\s+([A-Za-z_]\w*)\s+in\s+([^)]+)\)", "foreach (var $1 in $2)");
            line = Regex.Replace(line, @"<([A-Za-z_]\w*)>\[([^\]]*)\]", "new $1[] { $2 }");
            line = Regex.Replace(line, @"\b([A-Za-z_]\w*)\.toInt\(\)", "Convert.ToInt32($1)");
            line = line.Replace("super.", "base.", StringComparison.Ordinal);
            if (enableC4)
            {
                line = TranslateExpression(line);
            }
            yield return line;
        }
    }

    private static string MapParameters(string parameters, bool enableC4 = false) =>
        string.Join(", ", SplitParameters(parameters).Select(item => MapParameter(item, enableC4)));

    private static string[] SplitParameters(string parameters)
    {
        var normalized = parameters.Trim();
        if (normalized.StartsWith('{') && normalized.EndsWith('}'))
        {
            normalized = normalized[1..^1];
        }
        return normalized.Split(',').Select(item => item.Trim()).Where(item => item.Length > 0).ToArray();
    }

    private static string MapParameter(string parameter, bool enableC4 = false)
    {
        parameter = Regex.Replace(parameter.Trim().Trim('{', '}'), @"^required\s+", string.Empty);
        var match = Regex.Match(parameter, @"^(?<type>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)(?:\s*=\s*(?<default>.+))?$");
        if (!match.Success)
        {
            return $"object {SafeIdentifier(parameter)}";
        }
        var defaultValue = match.Groups["default"].Success ? $" = {TranslateExpression(match.Groups["default"].Value)}" : string.Empty;
        return $"{MapType(match.Groups["type"].Value, enableC4)} {match.Groups["name"].Value}{defaultValue}";
    }

    private static string MapType(string dartType, bool enableC4 = false)
    {
        var type = dartType.Trim();
        var nullable = type.EndsWith("?", StringComparison.Ordinal);
        if (nullable)
        {
            type = type[..^1];
        }
        string mapped;
        if (type.StartsWith("Future<", StringComparison.Ordinal) && type.EndsWith(">", StringComparison.Ordinal))
        {
            var argument = type[7..^1];
            mapped = enableC4
                ? argument == "void" ? "Future" : $"Future<{MapType(argument, true)}>"
                : $"Task<{MapType(argument)}>";
        }
        else if (type.StartsWith("List<", StringComparison.Ordinal) && type.EndsWith(">", StringComparison.Ordinal))
        {
            mapped = $"IReadOnlyList<{MapType(type[5..^1], enableC4)}>";
        }
        else if (enableC4 && type.StartsWith("Stream<", StringComparison.Ordinal) && type.EndsWith(">", StringComparison.Ordinal))
        {
            mapped = $"Stream<{MapType(type[7..^1], true)}>";
        }
        else if (enableC4 && Regex.Match(type, @"^(?<outer>[A-Za-z_]\w*)<(?<argument>[^<>]+)>$", RegexOptions.CultureInvariant) is { Success: true } generic)
        {
            mapped = $"{generic.Groups["outer"].Value}<{MapType(generic.Groups["argument"].Value, true)}>";
        }
        else
        {
            mapped = type switch
            {
                "String" => "string",
                "Object" => "object",
                "num" => "double",
                "void" => "void",
                _ => type,
            };
        }
        return nullable ? mapped + "?" : mapped;
    }

    private static string TranslateExpression(string expression)
    {
        var translated = expression.Trim()
            .Replace("null!", "null", StringComparison.Ordinal)
            .Replace("BoxConstraints.tightFor", "BoxConstraints.TightFor", StringComparison.Ordinal)
            .Replace("RenderFixture.run", "RenderFixture.Run", StringComparison.Ordinal)
            .Replace("math.pi", "Math.PI", StringComparison.Ordinal)
            .Replace(".toString()", ".ToString()", StringComparison.Ordinal);
        translated = Regex.Replace(translated, @"\bconst\s+", string.Empty, RegexOptions.CultureInvariant);
        translated = Regex.Replace(translated, @"<(?<type>[A-Za-z_]\w*)>\[(?<items>.*)\]", "new ${type}[] { ${items} }", RegexOptions.CultureInvariant);
        translated = Regex.Replace(translated, @"'(?<text>[^'\\]*)'", "\"${text}\"", RegexOptions.CultureInvariant);
        return translated;
    }

    private static string TranslateAppRoot(string expression)
    {
        var translated = TranslateExpression(expression);
        return Regex.IsMatch(translated, @"^new\s+", RegexOptions.CultureInvariant)
            ? translated
            : Regex.Replace(translated, @"^(?<type>[A-Za-z_]\w*(?:<[^>]+>)?)\s*\(", "new ${type}(", RegexOptions.CultureInvariant);
    }
    private static string TranslateConstructorInitializer(string initializer)
    {
        var match = Regex.Match(initializer.Trim(), @"^super\s*\((.*)\)$", RegexOptions.CultureInvariant);
        if (!match.Success)
        {
            throw new InvalidDataException($"Unsupported constructor initializer: {initializer}");
        }
        return $" : base({TranslateExpression(match.Groups[1].Value)})";
    }
    private static string SafeIdentifier(string value) => Regex.Replace(value, "[^A-Za-z0-9_]", "_");
    private static string? GeneratedDeclarationShape(LoweredDeclaration declaration) =>
        declaration.Kind.Contains("ClassDeclaration", StringComparison.Ordinal) ||
        declaration.Kind.Contains("MixinDeclaration", StringComparison.Ordinal) ||
        declaration.Kind.Contains("ExtensionDeclaration", StringComparison.Ordinal) ||
        declaration.Kind.Contains("FunctionDeclaration", StringComparison.Ordinal) ||
        declaration.Kind.Contains("TopLevelVariableDeclaration", StringComparison.Ordinal)
            ? "partial-friendly"
            : null;
    private static string DeclarationCode(string source) => Regex.Replace(
        source,
        @"\A(?:(?:\s*///[^\n]*(?:\n|\z))|(?:\s*//[^\n]*(?:\n|\z))|(?:\s*@[A-Za-z_]\w*(?:\([^\n]*\))?\s*(?:\n|\z)))*",
        string.Empty,
        RegexOptions.CultureInvariant);
    private static int CountLines(StringBuilder builder) => builder.ToString().Count(character => character == '\n');

    [GeneratedRegex(@"enum\s+([A-Za-z_]\w*)\s*\{(.*?)\}", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex EnumRegex();
    [GeneratedRegex(@"^(?<abstract>abstract\s+)?class\s+(?<name>[A-Za-z_]\w*)(?<typeParameters><[^>{}]+>)?(?:\s+extends\s+(?<base>[A-Za-z_]\w*(?:<[^>{}]+>)?))?(?:\s+with\s+(?<mixins>[A-Za-z_]\w*(?:\s*,\s*[A-Za-z_]\w*)*))?\s*\{(?<body>.*)\}\s*$", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ClassRegex();
    [GeneratedRegex(@"^mixin\s+(?<name>[A-Za-z_]\w*)\s*\{(?<body>.*)\}\s*$", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex MixinRegex();
    [GeneratedRegex(@"^extension\s+(?<name>[A-Za-z_]\w*)\s+on\s+(?<target>[A-Za-z_]\w*(?:<[^>{}]+>)?)\s*\{(?<body>.*)\}\s*$", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex ExtensionRegex();
    [GeneratedRegex(@"(?m)^[ \t]{2}(?:(?<modifier>final|const|late)\s+)?(?<type>(?!(?:final|const|late|var|return)\b)[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)(?:\s*=\s*(?<initializer>[^;]+))?\s*;\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex FieldRegex();
    private static Regex ConstructorRegex(string name) => new($@"(?m)^\s*(?:const\s+)?{Regex.Escape(name)}\s*\((?<parameters>[^)]*)\)\s*(?::\s*(?<initializer>super\s*\([^;]*\)))?\s*;\s*$", RegexOptions.CultureInvariant);
    [GeneratedRegex(@"(?m)^\s*(?:(?<override>@override)\s*)?(?<return>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)\s*\((?<parameters>[^)]*)\)\s*=>\s*(?<expression>.+);\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex ExpressionMethodRegex();
    [GeneratedRegex(@"(?ms)^\s*(?:(?<override>@override)\s*)?(?<return>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)\s*\((?<parameters>[^)]*)\)\s*(?<async>async\s*)?\{(?<body>.*)^\s*\}\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex BlockMethodRegex();
    [GeneratedRegex(@"^\s*(?<return>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)\s*\((?<parameters>[^)]*)\)\s*;\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex AbstractMethodRegex();
    [GeneratedRegex(@"^\s*(?<return>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)(?<typeParameters><[^>{}]+>)?\s*\((?<parameters>[^)]*)\)\s*=>\s*(?<expression>.+);\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex TopLevelExpressionFunctionRegex();
    [GeneratedRegex(@"^\s*(?<modifier>const|final)\s+(?<type>[A-Za-z_]\w*(?:<[^>]+>)?\??)\s+(?<name>[A-Za-z_]\w*)\s*=\s*(?<expression>.+);\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex TopLevelVariableRegex();
    [GeneratedRegex(@"\b(mixin|with|factory|dynamic|isolate|dart:ui|MethodChannel)\b", RegexOptions.CultureInvariant)]
    private static partial Regex UnsupportedRegex();
    [GeneratedRegex(@"\b(factory|dynamic|isolate|Zone|dart:ui|dart:ffi|ffi\.)\b", RegexOptions.CultureInvariant)]
    private static partial Regex C4UnsupportedRegex();
    [GeneratedRegex("""MethodChannel\s*\(\s*['"](?<channel>[^'"]+)['"]\s*\)""", RegexOptions.CultureInvariant)]
    private static partial Regex PlatformChannelRegex();
    [GeneratedRegex(@"void\s+main\s*\(\s*\)\s*\{\s*runApp\s*\((?<root>.*?)\)\s*;\s*\}", RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex AppEntryRegex();
}
