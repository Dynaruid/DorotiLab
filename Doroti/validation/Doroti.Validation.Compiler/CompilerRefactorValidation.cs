using System.Text.Json;
using Doroti.DartToCSharp;
using Doroti.Tooling;

internal static class CompilerRefactorValidation
{
    public static void Validate(string repositoryRoot, string dorotiRoot, List<string> failures)
    {
        ValidateOwnedAnalyzer(repositoryRoot, dorotiRoot, failures);
        ValidateTypedTypeDecoder(failures);
        ValidateIntrinsicIdentity(failures);
        ValidatePrinterBoundary(repositoryRoot, failures);
        ValidateTypedBackendBoundary(repositoryRoot, failures);
        ValidateSemanticStateBoundary(repositoryRoot, failures);
        ValidateAnalyzerCache(repositoryRoot, dorotiRoot, failures);
        ValidateDeterministicDumps(repositoryRoot, dorotiRoot, failures);
        ValidateSelectionV4(repositoryRoot, dorotiRoot, failures);
    }

    private static void ValidateAnalyzerCache(string repositoryRoot, string dorotiRoot, List<string> failures)
    {
        var temporary = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "analyzer-cache-validation");
        try
        {
            Directory.CreateDirectory(temporary);
            var analyzerRoot = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "analyzer");
            var dependency = Path.Combine(temporary, "dependency.dart");
            var unrelated = Path.Combine(temporary, "unrelated.txt");
            File.WriteAllText(dependency, "const value = 1;\n");
            File.WriteAllText(unrelated, "unrelated\n");
            var identity = AnalyzerSessionIdentity.Create(analyzerRoot, Path.Combine(analyzerRoot, "flutter_package_config.json"));
            using var profiler = new CompilerProfiler("cache-validation", null, 1, 1);
            var store = new AnalyzerCacheStore(Path.Combine(temporary, "cache"), identity, profiler);
            const string key = "cache-validation";
            const string payload = "{\"schemaVersion\":\"test\"}\n";
            var dependencies = new[] { new AnalyzerDependencyFingerprint(dependency, identity.HashFile(dependency)) };
            Parallel.Invoke(
                () => store.Write(key, payload, dependencies),
                () => store.Write(key, payload, dependencies));
            if (!store.TryRead(key, out var cached) || cached != payload)
            {
                failures.Add("Analyzer cache did not atomically round-trip a concurrent compressed entry.");
            }
            File.AppendAllText(unrelated, "changed\n");
            if (!store.TryRead(key, out _))
            {
                failures.Add("Analyzer cache invalidated an unrelated non-dependency file.");
            }
            File.AppendAllText(dependency, "const changed = true;\n");
            var changedIdentity = AnalyzerSessionIdentity.Create(analyzerRoot, Path.Combine(analyzerRoot, "flutter_package_config.json"));
            var changedStore = new AnalyzerCacheStore(Path.Combine(temporary, "cache"), changedIdentity, profiler);
            if (changedStore.TryRead(key, out _))
            {
                failures.Add("Analyzer cache hid a resolved dependency change.");
            }
            var status = store.Status();
            if (status.EntryCount != 1 || status.Bytes <= 0)
            {
                failures.Add("Analyzer cache status did not report the versioned compressed entry.");
            }
        }
        finally
        {
            RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, temporary);
        }
    }

    private static void ValidateOwnedAnalyzer(string repositoryRoot, string dorotiRoot, List<string> failures)
    {
        var analyzerRoot = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "analyzer");
        var runtimeFiles = AnalyzerRuntimeClosure.EnumerateFiles(analyzerRoot)
            .Select(path => Normalize(Path.GetRelativePath(analyzerRoot, path)))
            .ToArray();
        if (!runtimeFiles.Contains("entrypoints/extract.dart", StringComparer.Ordinal) ||
            !runtimeFiles.Any(path => path.StartsWith("lib/", StringComparison.Ordinal)) ||
            runtimeFiles.Any(path => path.StartsWith("tool/", StringComparison.Ordinal) || path.Contains("/.dart_tool/", StringComparison.Ordinal)))
        {
            failures.Add("Compiler analyzer runtime closure does not enforce entrypoints/lib/stubs ownership or excludes tool/cache state.");
        }

        var temporary = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "analyzer-closure");
        try
        {
            foreach (var source in AnalyzerRuntimeClosure.EnumerateFiles(analyzerRoot))
            {
                var relative = Path.GetRelativePath(analyzerRoot, source);
                var target = Path.Combine(temporary, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(source, target);
            }
            var baseline = AnalyzerRuntimeClosure.ComputeDigest(temporary);
            var toolPath = Path.Combine(temporary, "tool", "closure", "README.md");
            Directory.CreateDirectory(Path.GetDirectoryName(toolPath)!);
            File.WriteAllText(toolPath, "audit documentation only\n");
            if (AnalyzerRuntimeClosure.ComputeDigest(temporary) != baseline)
            {
                failures.Add("Analyzer tool-only mutation changed the compiler runtime closure digest.");
            }
            var runtimePath = Path.Combine(temporary, "lib", "src", "extractor.dart");
            File.AppendAllText(runtimePath, "\n// runtime mutation\n");
            if (AnalyzerRuntimeClosure.ComputeDigest(temporary) == baseline)
            {
                failures.Add("Analyzer runtime mutation did not change the compiler runtime closure digest.");
            }
        }
        finally
        {
            RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, temporary);
        }
    }

    private static void ValidateTypedTypeDecoder(List<string> failures)
    {
        if (Doroti.Tooling.ArtifactFiles.JsonOptions.MaxDepth < 128)
        {
            failures.Add("Analyzer protocol JSON depth is too small for nested Flutter framework expressions.");
        }

        var nested = DartTypeDecoder.Decode("Map<String, List<Future<int?>>>?");
        if (nested is not DartInterfaceType
            {
                Symbol.Name: "Map",
                ValueNullability: Nullability.Nullable,
                TypeArguments.Length: 2,
            } map ||
            map.TypeArguments[1] is not DartInterfaceType { Symbol.Name: "List", TypeArguments.Length: 1 })
        {
            failures.Add("Typed type decoder did not preserve nested generic/nullability structure.");
        }

        var function = DartTypeDecoder.Decode("Future<void> Function(String value, int?)?");
        if (function is not DartFunctionType
            {
                ValueNullability: Nullability.Nullable,
                Parameters.Length: 2,
            })
        {
            failures.Add("Typed type decoder did not preserve function type structure.");
        }
        var genericFunction = DartTypeDecoder.Decode(
            "void Function({required bool allowPlatformDefault})? Function<T>(T Function(double))");
        if (genericFunction is not DartFunctionType
            {
                ReturnType: DartFunctionType { ValueNullability: Nullability.Nullable, Parameters.Length: 1 },
                Parameters.Length: 1,
            })
        {
            failures.Add("Typed type decoder did not preserve generic/named nested function structure.");
        }

        var analyzerGenericFunction = DartTypeDecoder.Decode(
            "Iterable<T₀> Function<T₀>(T₀ Function(AnnotationEntry<T>) transform)");
        if (analyzerGenericFunction is not DartFunctionType
            {
                ReturnType: DartInterfaceType { Symbol.Name: "Iterable", TypeArguments.Length: 1 },
                Parameters.Length: 1,
            })
        {
            failures.Add("Typed type decoder did not preserve analyzer-renamed generic function structure.");
        }

        var namedRecord = DartTypeDecoder.Decode(
            "({TextPosition boundaryEnd, TextPosition boundaryStart})?");
        if (namedRecord is not DartRecordType record || record is not
            {
                Positional.Length: 0,
                Named.Count: 2,
                ValueNullability: Nullability.Nullable,
            } || !record.Named.ContainsKey("boundaryStart") || !record.Named.ContainsKey("boundaryEnd"))
        {
            failures.Add("Typed type decoder did not preserve named record fields.");
        }

        var promotedTypeParameter = DartTypeDecoder.Decode(
            "T & ExtendSelectionVerticallyToAdjacentPageIntent");
        if (promotedTypeParameter is not DartInterfaceType
            {
                Symbol.Name: "ExtendSelectionVerticallyToAdjacentPageIntent",
                ValueNullability: Nullability.NonNullable,
            })
        {
            failures.Add("Typed type decoder did not preserve an analyzer flow-promotion intersection bound.");
        }

        try
        {
            _ = DartTypeDecoder.Decode("Future<List<int>");
            failures.Add("Malformed analyzer type silently decoded instead of failing.");
        }
        catch (DartTypeDecodeException)
        {
        }
    }

    private static void ValidateTypedBackendBoundary(string repositoryRoot, List<string> failures)
    {
        var backendRoot = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp");
        foreach (var path in Directory.EnumerateFiles(backendRoot, "*.cs", SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(path);
            foreach (var forbidden in new[]
                     {
                         "MigrationIr", "AnalyzerElement", "AnalyzerParameter", "AnalyzerTypeParameter",
                         "DartAstNode", "DartResolvedDeclaration", "DartResolvedMember",
                     })
            {
                if (source.Contains(forbidden, StringComparison.Ordinal))
                {
                    failures.Add($"Typed C# backend directly references frontend/artifact DTO token {forbidden}: {Normalize(Path.GetRelativePath(repositoryRoot, path))}");
                }
            }
        }
    }

    private static void ValidateIntrinsicIdentity(List<string> failures)
    {
        var member = SymbolId.Parse("dart:core#String.length");
        var receiver = new DartInterfaceType(SymbolId.Parse("dart:core#String"), [], Nullability.NonNullable);
        if (RuntimeIntrinsicRegistry.Resolve(member, receiver)?.Intrinsic != RuntimeIntrinsic.StringLength)
        {
            failures.Add("Canonical String.length intrinsic did not resolve.");
        }
        var sameName = SymbolId.Parse("package:sample/model.dart#String.length");
        if (RuntimeIntrinsicRegistry.Resolve(sameName, receiver) is not null)
        {
            failures.Add("Same-name user member incorrectly resolved as a runtime intrinsic.");
        }
        var wrongReceiver = new DartInterfaceType(SymbolId.Parse("package:sample/model.dart#String"), [], Nullability.Nullable);
        if (RuntimeIntrinsicRegistry.Resolve(member, wrongReceiver) is not null)
        {
            failures.Add("Runtime intrinsic ignored canonical receiver identity/nullability context.");
        }
        if (RuntimeIntrinsicRegistry.Resolve(SymbolId.Parse("dart:core#identical"), null)?.Intrinsic != RuntimeIntrinsic.Identical)
        {
            failures.Add("Canonical Dart identical intrinsic did not resolve.");
        }
    }

    private static void ValidatePrinterBoundary(string repositoryRoot, List<string> failures)
    {
        var origin = new CsOrigin("fixture.dart", 4, 8, "package:fixture/fixture.dart#Sample");
        var unit = new CsCompilationUnit(
            "Fixture.Generated",
            [new CsUsing("System")],
            [new CsTypeDeclaration(
                origin,
                CsAccessibility.Public,
                CsTypeKind.Class,
                "Sample",
                [],
                [],
                [new CsField(origin, CsAccessibility.Public, "Value", CsTypeReference.Named("int"), new CsLiteral(origin, 1))])]);
        var first = new CSharpPrinter().Print(unit);
        var second = new CSharpPrinter().Print(unit);
        if (!string.Equals(first.Text, second.Text, StringComparison.Ordinal) || first.Text.Contains('\r'))
        {
            failures.Add("C# IR printer is not byte deterministic LF output.");
        }
        if (first.Origins.Length != 1 || first.Origins[0].StartLine <= 0 || first.Origins[0].EndLine < first.Origins[0].StartLine)
        {
            failures.Add("C# IR printer did not preserve declaration origin line marks.");
        }

        var syntax = new CsSyntaxBuilder();
        using (syntax.BeginRegion(CsSyntaxRegionKind.Declaration, origin))
        {
            syntax.AppendLine("public static class Sample");
            syntax.AppendLine("{");
            using (syntax.BeginRegion(CsSyntaxRegionKind.Statement, origin))
            {
                syntax.AppendLine("    public static int Value = 1;");
            }
            syntax.AppendLine("}");
        }
        var document = syntax.Build();
        var printedSyntax = new CSharpPrinter().Print(document);
        if (document.Tokens.Length == 0 ||
            !document.Tokens.Any(item => item.Kind == CsSyntaxTokenKind.Keyword) ||
            document.Regions.Count(item => item.Kind == CsSyntaxRegionKind.Declaration) != 1 ||
            document.Regions.Count(item => item.Kind == CsSyntaxRegionKind.Statement) != 1 ||
            printedSyntax.Origins is not [{ StartLine: 1, EndLine: 4 }])
        {
            failures.Add("Product C# syntax IR did not preserve typed tokens and origin-aware regions through the printer.");
        }

        var printerPath = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp", "Printing", "CSharpPrinter.cs");
        var printerSource = File.ReadAllText(printerPath);
        foreach (var forbidden in new[] { "MigrationIr", "CoreIntrinsic", "ElementId", "StaticType", "DOTCONV", "DOTF" })
        {
            if (printerSource.Contains(forbidden, StringComparison.Ordinal))
            {
                failures.Add($"C# printer contains forbidden semantic token: {forbidden}.");
            }
        }
    }

    private static void ValidateSemanticStateBoundary(string repositoryRoot, List<string> failures)
    {
        var legacyPath = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp", "Legacy", "SemanticCSharpEmitter.cs");
        if (File.Exists(legacyPath))
        {
            failures.Add("Removed legacy SemanticCSharpEmitter path was reintroduced.");
            return;
        }
        var lowererDirectory = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp", "Lowering");
        var componentPaths = Directory.GetFiles(lowererDirectory, "*.cs", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        var lowererPaths = componentPaths
            .Where(path => Path.GetFileName(path).StartsWith("FrameworkCSharpLowerer", StringComparison.Ordinal))
            .ToArray();
        var source = string.Join("\n", lowererPaths.Select(File.ReadAllText));
        if (source.Contains("[ThreadStatic]", StringComparison.Ordinal) ||
            source.Contains("static IReadOnlyDictionary", StringComparison.Ordinal))
        {
            failures.Add("Compiler still contains mutable static/thread-static semantic state.");
        }
        foreach (var forbidden in new[]
                 {
                     "CSharpWriter", "CountLines", "ChildAt(", ".Properties", "EmitExpression", "EmitStatement",
                     "builder.ToString()",
                 })
        {
            if (source.Contains(forbidden, StringComparison.Ordinal))
            {
                failures.Add($"Active framework lowerer bypasses typed Core/C# IR boundary: {forbidden}.");
            }
        }
        foreach (var path in componentPaths)
        {
            var fileSource = File.ReadAllText(path);
            if (fileSource.Contains("[ThreadStatic]", StringComparison.Ordinal) ||
                fileSource.Contains("static IReadOnlyDictionary", StringComparison.Ordinal) ||
                fileSource.Contains("CSharpWriter", StringComparison.Ordinal) ||
                fileSource.Contains(".Properties", StringComparison.Ordinal))
            {
                failures.Add($"Lowering component violates the immutable typed boundary: {Path.GetFileName(path)}.");
            }
        }
        foreach (var required in new[]
                 {
                     "CoreNodeKind", "CoreProperty", "CoreChildRole", "CsSyntaxRegionKind.Expression",
                     "CsSyntaxRegionKind.Statement", "new CSharpPrinter().Print(builder.Build())",
                 })
        {
            if (!source.Contains(required, StringComparison.Ordinal))
            {
                failures.Add($"Active framework lowerer is missing structured Core/C# IR evidence: {required}.");
            }
        }

        var syntaxIrPath = Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp", "src", "Backend", "CSharp", "Ir", "CsSyntaxDocument.cs");
        var syntaxIr = File.ReadAllText(syntaxIrPath);
        foreach (var forbidden in new[] { "CsRawText", "CsUnknown", "enum CsSyntaxTokenKind\n{\n    Unknown" })
        {
            if (syntaxIr.Contains(forbidden, StringComparison.Ordinal))
            {
                failures.Add($"C# syntax IR reintroduced an unstructured escape hatch: {forbidden}.");
            }
        }
    }

    private static void ValidateDeterministicDumps(string repositoryRoot, string dorotiRoot, List<string> failures)
    {
        var temporary = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "compiler-dump");
        var first = Path.Combine(temporary, "first");
        var second = Path.Combine(temporary, "second");
        try
        {
            var symbol = SymbolId.Parse("package:fixture/sample.dart#Sample");
            var origin = new SourceOrigin("fixture/sample.dart", 1, 6, symbol);
            var node = new DartAstNode(
                "ClassDeclaration",
                "ClassDeclarationImpl",
                "declaration",
                origin,
                null,
                symbol,
                new Dictionary<string, string?>(),
                []);
            var element = new DartResolvedElement(
                "CLASS", "Sample", symbol.Value, symbol, false, "Sample",
                new DartInterfaceType(symbol, [], Nullability.NonNullable), null, null, null, [], null, null, [], false, false);
            var declaration = new DartResolvedDeclaration("ClassDeclarationImpl", "Sample", 1, 6, element, node, []);
            var analyzer = new AnalyzerOutput(
                "doroti.analyzer-output/v3", "package:fixture/sample.dart", [], [], [], [], "resolved");
            var identity = new CompilerIdentity("test", "test", "9.0.0", new string('0', 40),
                "doroti.migration-ir/v3", "3.0.0", "test", "test", "test", "test");
            var mapping = new SourceMapEntry("fixture/sample.dart", 1, 6, "Sample", "sample.g.cs", 1, 2);
            var coreDeclarations = DartToCoreLowerer.Lower([declaration]);
            var input = new CompilerDumpInput(
                "fixture/sample.dart", "package:fixture/sample.dart", analyzer, [declaration], coreDeclarations,
                "sample.g.cs", "class Sample {}\n", [mapping]);
            CompilerArtifactDumper.Write(new(first), identity, [input]);
            CompilerArtifactDumper.Write(new(second), identity, [input]);
            var firstFiles = Directory.EnumerateFiles(first).Select(Path.GetFileName).Order(StringComparer.Ordinal).ToArray();
            var secondFiles = Directory.EnumerateFiles(second).Select(Path.GetFileName).Order(StringComparer.Ordinal).ToArray();
            if (firstFiles.Length != 4 || !firstFiles.SequenceEqual(secondFiles, StringComparer.Ordinal) ||
                firstFiles.Any(file => !File.ReadAllBytes(Path.Combine(first, file!)).SequenceEqual(File.ReadAllBytes(Path.Combine(second, file!)))))
            {
                failures.Add("Compiler stage dumps are not byte deterministic.");
            }
            if (firstFiles.Any(file => File.ReadAllText(Path.Combine(first, file!)).Contains(repositoryRoot, StringComparison.OrdinalIgnoreCase)))
            {
                failures.Add("Compiler stage dump leaked an absolute checkout path.");
            }
        }
        finally
        {
            RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, temporary);
        }
    }

    private static void ValidateSelectionV4(string repositoryRoot, string dorotiRoot, List<string> failures)
    {
        var roots = new[]
        {
            Path.Combine(dorotiRoot, "migration", "selections"),
            Path.Combine(dorotiRoot, "validation", "cases"),
            Path.Combine(repositoryRoot, "migration", "selections"),
        };
        foreach (var path in roots.Where(Directory.Exists).SelectMany(root => Directory.EnumerateFiles(root, "*.json")))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            var root = document.RootElement;
            if (!root.TryGetProperty("schemaVersion", out var schema) ||
                !schema.GetString()!.StartsWith("doroti.converter-selection/", StringComparison.Ordinal))
            {
                continue;
            }
            if (schema.GetString() != "doroti.converter-selection/v4" || root.TryGetProperty("analyzerProject", out _))
            {
                failures.Add($"Active selection is not analyzer-owned schema v4: {Normalize(Path.GetRelativePath(repositoryRoot, path))}");
            }
        }
    }

    private static string Normalize(string path) => path.Replace('\\', '/');
}
