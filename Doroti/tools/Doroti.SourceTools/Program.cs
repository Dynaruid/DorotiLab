using Doroti.SourceTools;
using Doroti.Tooling;

try
{
    if (args.Length == 0)
    {
        throw new ArgumentException("Usage: Doroti.SourceTools <audit|flutter-avalonia-boundary-audit|framework-evidence-reset|review|diff|promote|rebase|vendor-review> [options]");
    }

    var root = RepositoryPaths.FindRoot(Environment.CurrentDirectory);
    switch (args[0])
    {
        case "flutter-avalonia-boundary-audit":
            {
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "flutter-avalonia", "g4-0-boundary-audit.json");
                var report = G4BoundaryAudit.Run(root, args.Contains("--write-owner-audit", StringComparer.Ordinal));
                ArtifactFiles.WriteJson(output, report);
                Console.WriteLine($"G4-0 boundary audit: {(report.Success ? "PASS" : "FAIL")} ({report.CurrentOwnerSymbolCount} current symbols, {report.SourceBoundaryEntryCount} Flutter boundary entries, {report.CapabilityCount} capabilities)");
                foreach (var finding in report.Findings)
                {
                    Console.Error.WriteLine($"{finding.Code} {finding.Subject}: {finding.Message}");
                }
                return report.Success ? 0 : 2;
            }
        case "audit":
            {
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "audit");
                var report = SourceAudit.Run(
                    root,
                    Path.Combine(root, "migration", "source-manifest.json"),
                    Path.Combine(root, "migration", "provenance.json"));
                var vendorReport = VendorAudit.Run(
                    root,
                    Path.Combine(root, "migration", "source-manifest.json"),
                    Path.Combine(root, "migration", "vendor", "avalonia-platform", "selection.json"),
                    Path.Combine(root, "migration", "vendor", "avalonia-platform", "provenance.json"));
                ArtifactFiles.WriteJson(Path.Combine(output, "source-audit.json"), report);
                ArtifactFiles.WriteUtf8(Path.Combine(output, "source-audit.md"), SourceAudit.ToMarkdown(report));
                ArtifactFiles.WriteJson(Path.Combine(output, "vendor-audit.json"), vendorReport);
                ArtifactFiles.WriteUtf8(Path.Combine(output, "vendor-audit.md"), VendorAudit.ToMarkdown(vendorReport));
                var success = report.Success && vendorReport.Success;
                Console.WriteLine($"Source audit: {(success ? "PASS" : "FAIL")} ({report.Sources.Length} sources, {report.Findings.Length + vendorReport.Findings.Length} findings)");
                return success ? 0 : 2;
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
        case "vendor-review":
            {
                var sourceManifest = ReadOption(args, "--source-manifest") ?? Path.Combine(root, "migration", "source-manifest.json");
                var selection = ReadOption(args, "--selection") ?? Path.Combine(root, "migration", "vendor", "avalonia-platform", "selection.json");
                var provenance = ReadOption(args, "--provenance") ?? Path.Combine(root, "migration", "vendor", "avalonia-platform", "provenance.json");
                var output = ReadOption(args, "--output") ?? Path.Combine(root, "artifacts", "vendor-review");
                var report = VendorReview.Create(root, sourceManifest, selection, provenance, output);
                Console.WriteLine($"Vendor review bundle: {report.Entries.Length} selection entry(s) at {output}");
                return 0;
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
