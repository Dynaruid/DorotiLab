using Doroti.SourceTools;
using Doroti.Tooling;

try
{
    if (args.Length == 0)
    {
        throw new ArgumentException("Usage: Doroti.SourceTools <audit|framework-evidence-reset|review|diff|promote|rebase> [options]");
    }

    var root = RepositoryPaths.FindRoot(Environment.CurrentDirectory);
    switch (args[0])
    {
        case "audit":
            {
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "audit");
                var report = SourceAudit.Run(
                    root,
                    Path.Combine(root, "migration", "source-manifest.json"),
                    Path.Combine(root, "migration", "provenance.json"));
                ArtifactFiles.WriteJson(Path.Combine(output, "source-audit.json"), report);
                ArtifactFiles.WriteUtf8(Path.Combine(output, "source-audit.md"), SourceAudit.ToMarkdown(report));
                Console.WriteLine($"Source audit: {(report.Success ? "PASS" : "FAIL")} ({report.Sources.Length} sources, {report.Findings.Length} findings)");
                return report.Success ? 0 : 2;
            }
        case "framework-evidence-reset":
            {
                var output = ReadOption(args, "--output-root");
                var report = FlutterFrameworkEvidenceReset.Regenerate(root, output);
                foreach (var artifact in report.Artifacts)
                {
                    Console.WriteLine($"{artifact.Milestone}: {artifact.MissingMechanicalSymbols} ungenerated symbol(s), {artifact.ImplementationBlockers} completion blocker(s)");
                }
                return 0;
            }
        case "review":
            {
                var manifest = ReadOption(args, "--manifest") ?? Path.Combine(root, "migration", "promotion.json");
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "promotion");
                var report = Promotion.Review(root, manifest, output);
                Console.WriteLine($"Promotion review: {report.Items.Length} approved symbol(s) at {output}");
                return 0;
            }
        case "diff":
            {
                var manifest = ReadOption(args, "--manifest") ?? Path.Combine(root, "migration", "promotion.json");
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "promotion");
                var report = Promotion.Diff(root, manifest, output);
                Console.WriteLine($"Promotion diff: {(report.Success ? "PASS" : "CONFLICT")} ({report.Changes.Length} symbol(s))");
                return report.Success ? 0 : 2;
            }
        case "promote":
            {
                var manifest = ReadOption(args, "--manifest") ?? Path.Combine(root, "migration", "promotion.json");
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "promotion");
                var report = Promotion.Promote(root, manifest, output);
                Console.WriteLine($"Promotion: PASS ({report.WrittenCount} source file(s) written, {report.Changes.Length} symbol(s) reviewed)");
                return 0;
            }
        case "rebase":
            {
                var manifest = ReadOption(args, "--manifest") ?? Path.Combine(root, "migration", "promotion.json");
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "promotion");
                var report = Promotion.Rebase(root, manifest, output);
                Console.WriteLine($"Promotion rebase: {(report.Success ? "PASS" : "CONFLICT")} ({report.Changes.Length} symbol(s))");
                return report.Success ? 0 : 2;
            }
        default:
            throw new ArgumentException($"Unknown command: {args[0]}");
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
