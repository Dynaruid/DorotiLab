using Doroti.DartToCSharp;

try
{
    var firstCommand = args.FirstOrDefault();
    if (firstCommand is "cache-status" or "cache-prune")
    {
        var cacheCommand = firstCommand;
        var cacheDirectory = ReadOption(args, "--cache-dir")
            ?? throw new ArgumentException("cache-status/cache-prune requires --cache-dir <directory>.");
        if (cacheCommand == "cache-prune")
        {
            var maximumBytes = ReadLong(args, "--max-bytes") ?? 2L * 1024 * 1024 * 1024;
            var maximumAgeDays = ReadLong(args, "--max-age-days") ?? 30;
            var removed = AnalyzerCacheStore.Prune(cacheDirectory, maximumBytes, TimeSpan.FromDays(maximumAgeDays));
            Console.WriteLine($"Analyzer cache prune: removed={removed}");
        }
        var status = AnalyzerCacheStore.ReadStatus(cacheDirectory);
        Console.WriteLine($"Analyzer cache: entries={status.EntryCount}; bytes={status.Bytes}; oldest={status.OldestWriteUtc:O}; newest={status.NewestWriteUtc:O}");
        return 0;
    }
    var root = FindDorotiRoot(Environment.CurrentDirectory);
    var port = ReadOption(args, "--port");
    if (port is not null)
    {
        var portCache = ReadOption(args, "--cache-dir");
        var portWorkspaceRoot = ReadOption(args, "--workspace-root") ?? DefaultPortWorkspaceRoot(root);
        var command = args.Length > 0 ? args[0] : string.Empty;
        if (command == "compile")
        {
            if (ReadOption(args, "--output") is not null)
            {
                throw new ArgumentException("Port compilation publishes only below --workspace-root; --output is not supported.");
            }
            var workspace = new PortCompiler().Compile(port, portWorkspaceRoot, portCache);
            Console.WriteLine($"Doroti port compile: {(workspace.Report.Success ? "PASS" : "REVIEW REQUIRED")} ({workspace.Report.Outputs.Length} file(s), {workspace.Report.Diagnostics.Length} diagnostic(s))");
            Console.WriteLine($"Workspace: {workspace.Path}");
            Console.WriteLine($"Generated base: {workspace.GeneratedBasePath}");
            Console.WriteLine($"Effective project: {Path.Combine(workspace.Path, workspace.Ownership.EffectiveProject)}");
            Console.WriteLine($"Port state: {Path.Combine(workspace.Path, "port-state.json")}");
            return workspace.Report.Success ? 0 : 2;
        }
        if (command == "adopt")
        {
            var symbol = ReadRequiredOption(args, "--symbol");
            var reviewOutput = ReadOption(args, "--output") ?? Path.Combine(DefaultReviewRoot(root), "adopt", SafeName(symbol));
            var bundle = new PortAdoption().Create(port, portWorkspaceRoot, symbol, reviewOutput, portCache, ReadOptionValue(args, "--library"));
            Console.WriteLine("Doroti adoption review: PASS (product source unchanged)");
            Console.WriteLine($"Review bundle: {bundle.Path}");
            return 0;
        }
        if (command == "rebase")
        {
            var revision = ReadRequiredOption(args, "--source-revision");
            var previous = ReadOption(args, "--previous-workspace") ?? PortRebaser.FindPreviousWorkspace(portWorkspaceRoot, port);
            var reviewOutput = ReadOption(args, "--output") ?? Path.Combine(DefaultReviewRoot(root), "rebase", $"{Path.GetFileName(previous)}-{SafeName(revision)}");
            var bundle = new PortRebaser().Create(previous, port, revision, reviewOutput, portCache);
            Console.WriteLine($"Doroti port rebase: {(bundle.Report.HasBlockingChanges ? "REVIEW REQUIRED" : "PASS")}");
            Console.WriteLine($"Review bundle: {bundle.Path}");
            return bundle.Report.HasBlockingChanges ? 2 : 0;
        }
        throw new ArgumentException("Port manifests support 'compile', 'adopt', or 'rebase'.");
    }

    var manifest = ReadOption(args, "--manifest") ?? Path.Combine(root, "migration", "selections", "r1.json");
    var cache = ReadOption(args, "--cache-dir");
    var workspaceRoot = ReadOption(args, "--workspace-root");
    var output = ReadOption(args, "--output");
    var parallelism = ReadParallelism(args);
    var analyzerWorkers = ReadPositiveInteger(args, "--analyzer-workers");
    var loweringParallelism = ReadPositiveInteger(args, "--lowering-parallelism");
    var telemetryPath = ReadOption(args, "--telemetry");
    var dumpDirectory = ReadOption(args, "--dump-ir");
    var dumpOptions = dumpDirectory is null
        ? null
        : new CompilerDumpOptions(dumpDirectory, ReadDumpStages(args));
    if (dumpDirectory is null && ReadOptionValue(args, "--dump-stage") is not null)
    {
        throw new ArgumentException("--dump-stage requires --dump-ir <directory>.");
    }
    if (workspaceRoot is not null && output is not null)
    {
        throw new ArgumentException("Use either --workspace-root or --output, not both.");
    }
    ConverterReport report;
    var compiler = new DartCompiler();
    if (workspaceRoot is not null)
    {
        var workspace = compiler.CompileToWorkspace(
            manifest,
            workspaceRoot,
            cache,
            parallelism,
            dumpOptions,
            telemetryPath,
            analyzerWorkers,
            loweringParallelism);
        output = workspace.Path;
        report = workspace.Report;
    }
    else
    {
        output ??= Path.Combine(root, "migration", "generated", "r1");
        report = compiler.Compile(
            manifest,
            output,
            cache,
            parallelism,
            dumpOptions,
            telemetryPath,
            analyzerWorkers,
            loweringParallelism);
    }
    Console.WriteLine(
        $"Dart to C# draft: {(report.Success ? "PASS" : "REVIEW REQUIRED")} " +
        $"({report.Outputs.Length} file(s), {report.Diagnostics.Length} diagnostic(s)); workspace={output}; " +
        $"analyzer-workers={CompilerParallelism.ResolveAnalyzerWorkers(analyzerWorkers, parallelism)}; " +
        $"lowering-parallelism={CompilerParallelism.ResolveLoweringParallelism(loweringParallelism, parallelism)}");
    return report.Success ? 0 : 2;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static string? ReadOption(string[] arguments, string name)
{
    var index = Array.IndexOf(arguments, name);
    return index >= 0 && index + 1 < arguments.Length ? Path.GetFullPath(arguments[index + 1]) : null;
}

static string? ReadOptionValue(string[] arguments, string name)
{
    var index = Array.IndexOf(arguments, name);
    return index >= 0 && index + 1 < arguments.Length ? arguments[index + 1] : null;
}

static int? ReadParallelism(string[] arguments)
{
    var raw = ReadOptionValue(arguments, "--parallelism") ?? ReadOptionValue(arguments, "-j");
    if (raw is null)
    {
        return null;
    }

    if (!int.TryParse(raw, out var value) || value < 1)
    {
        throw new ArgumentException("--parallelism / -j must be a positive integer.");
    }

    return value;
}

static int? ReadPositiveInteger(string[] arguments, string name)
{
    var raw = ReadOptionValue(arguments, name);
    if (raw is null) return null;
    if (!int.TryParse(raw, out var value) || value < 1)
    {
        throw new ArgumentException($"{name} must be a positive integer.");
    }
    return value;
}

static long? ReadLong(string[] arguments, string name)
{
    var raw = ReadOptionValue(arguments, name);
    if (raw is null) return null;
    if (!long.TryParse(raw, out var value) || value < 0)
    {
        throw new ArgumentException($"{name} must be a non-negative integer.");
    }
    return value;
}

static CompilerDumpStage ReadDumpStages(string[] arguments)
{
    var raw = ReadOptionValue(arguments, "--dump-stage");
    if (raw is null)
    {
        return CompilerDumpStage.All;
    }
    var stages = CompilerDumpStage.None;
    foreach (var value in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        stages |= value.ToLowerInvariant() switch
        {
            "all" => CompilerDumpStage.All,
            "analyzer" or "analyzer-protocol" => CompilerDumpStage.AnalyzerProtocol,
            "dart" or "dart-ir" => CompilerDumpStage.DartIr,
            "core" or "core-ir" => CompilerDumpStage.CoreIr,
            "csharp" or "csharp-ir" => CompilerDumpStage.CSharpIr,
            _ => throw new ArgumentException($"Unsupported --dump-stage value: {value}"),
        };
    }
    return stages == CompilerDumpStage.None
        ? throw new ArgumentException("--dump-stage must select at least one stage.")
        : stages;
}

static string ReadRequiredOption(string[] arguments, string name) =>
    ReadOptionValue(arguments, name) ?? throw new ArgumentException($"Missing required option {name}.");

static string FindDorotiRoot(string startDirectory)
{
    for (var directory = new DirectoryInfo(Path.GetFullPath(startDirectory)); directory is not null; directory = directory.Parent)
    {
        if (File.Exists(Path.Combine(directory.FullName, "Doroti.slnx")))
        {
            return directory.FullName;
        }

        var nestedRoot = Path.Combine(directory.FullName, "Doroti");
        if (File.Exists(Path.Combine(nestedRoot, "Doroti.slnx")))
        {
            return nestedRoot;
        }
    }

    throw new DirectoryNotFoundException($"Could not find the Doroti workspace from {startDirectory}.");
}

static string DefaultPortWorkspaceRoot(string dorotiRoot)
{
    var repositoryRoot = Path.GetFullPath(Path.Combine(dorotiRoot, ".."));
    return Directory.Exists(Path.Combine(repositoryRoot, "tools", "Doroti.DartToCSharp"))
        ? Path.Combine(repositoryRoot, ".doroti", "workspaces")
        : Path.Combine(dorotiRoot, ".doroti", "workspaces");
}

static string DefaultReviewRoot(string dorotiRoot)
{
    var workspaceRoot = DefaultPortWorkspaceRoot(dorotiRoot);
    return Path.Combine(Path.GetDirectoryName(workspaceRoot)!, "reviews");
}

static string SafeName(string value) => string.Concat(value.Select(character =>
    char.IsLetterOrDigit(character) || character is '-' or '_' or '.' ? character : '_'));
