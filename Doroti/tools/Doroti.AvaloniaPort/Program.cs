using Doroti.AvaloniaPort;
using Doroti.Tooling;

try
{
    var root = RepositoryPaths.FindRoot(Environment.CurrentDirectory);
    var command = args.FirstOrDefault() ?? "audit";
    var config = ReadOption(args, "--config") ?? Path.Combine(root, "migration", "avalonia-shell", "port-selection.json");
    switch (command)
    {
        case "graph":
        case "update":
            {
                var report = AvaloniaPortWorkflow.Update(root, config);
                Console.WriteLine($"Avalonia shell port update: {report.Status} ({report.SelectedFileCount} selected files, {report.DependencyCount} dependency nodes)");
                foreach (var finding in report.Findings)
                {
                    Console.Error.WriteLine($"{finding.Code} {finding.Subject}: {finding.Message}");
                }
                return report.Success ? 0 : 2;
            }
        case "audit":
            {
                var report = AvaloniaPortWorkflow.Audit(root, config);
                Console.WriteLine($"Avalonia shell port audit: {report.Status} ({report.Findings.Length} findings)");
                foreach (var finding in report.Findings)
                {
                    Console.Error.WriteLine($"{finding.Code} {finding.Subject}: {finding.Message}");
                }
                return report.Success ? 0 : 2;
            }
        case "stage":
            {
                var output = ReadOption(args, "--output") ?? throw new ArgumentException("stage requires --output.");
                var report = AvaloniaPortWorkflow.Stage(root, config, output);
                Console.WriteLine($"Avalonia shell port stage: {report.Status} ({report.Files.Length} files at {output})");
                return report.Success ? 0 : 2;
            }
        case "rebase":
            {
                var previous = ReadOption(args, "--previous-source") ?? throw new ArgumentException("rebase requires --previous-source.");
                var current = ReadOption(args, "--current-source") ?? throw new ArgumentException("rebase requires --current-source.");
                var output = ReadOption(args, "--output") ?? throw new ArgumentException("rebase requires --output.");
                var report = AvaloniaPortWorkflow.Rebase(root, config, previous, current);
                ArtifactFiles.WriteJson(output, report);
                Console.WriteLine($"Avalonia shell rebase: {report.Status} ({report.Files.Length} selected files)");
                return report.Success ? 0 : 2;
            }
        default:
            throw new ArgumentException("Usage: Doroti.AvaloniaPort <update|audit|stage|rebase> [--config path] [--output path] [--previous-source path] [--current-source path]");
    }
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
