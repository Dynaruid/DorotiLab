using System.Security.Cryptography;
using System.Text;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static class Promotion
{
    private const string ManifestSchema = "doroti.framework-promotion/v2";
    private static readonly HashSet<string> IssueCategories = new(StringComparer.Ordinal)
    {
        "compiler-general",
        "framework-specific",
        "runtime-boundary",
        "upstream-ambiguity",
    };

    public static PromotionReviewReport Review(string repositoryRoot, string manifestPath, string outputDirectory)
    {
        var context = LoadAndValidate(repositoryRoot, manifestPath);
        Directory.CreateDirectory(outputDirectory);
        var items = context.Items.Select(item => new PromotionReviewItem(
            item.Manifest.ElementId,
            item.Manifest.Symbol,
            item.Manifest.Candidate,
            item.Manifest.CandidateSha256,
            item.Manifest.Target,
            item.Manifest.Reviewer,
            item.Manifest.ReviewState,
            item.Manifest.Issues.Select(issue => issue.Category).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray(),
            item.Manifest.ValidationCases.Order(StringComparer.Ordinal).ToArray())).ToArray();
        var report = new PromotionReviewReport(
            "doroti.framework-promotion-review/v1",
            context.Manifest.UpstreamRevision,
            context.Manifest.CompilerIdentity,
            true,
            items);
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "review-report.json"), report);
        return report;
    }

    public static PromotionDiffReport Diff(string repositoryRoot, string manifestPath, string outputDirectory) =>
        CreateDiff(repositoryRoot, manifestPath, outputDirectory, "diff");

    public static PromotionDiffReport Rebase(string repositoryRoot, string manifestPath, string outputDirectory) =>
        CreateDiff(repositoryRoot, manifestPath, outputDirectory, "rebase");

    public static PromotionReport Promote(string repositoryRoot, string manifestPath, string outputDirectory)
    {
        var context = LoadAndValidate(repositoryRoot, manifestPath);
        Review(repositoryRoot, manifestPath, outputDirectory);
        var diff = CreateDiff(context, outputDirectory, "promote");
        var conflicts = diff.Changes.Where(change => change.Status == "conflict").ToArray();
        if (conflicts.Length > 0)
        {
            throw new PromotionConflictException($"Promotion blocked by {conflicts.Length} conflict(s): {string.Join(", ", conflicts.Select(item => item.ElementId))}");
        }

        var writes = context.Items
            .GroupBy(item => item.TargetPath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Where(item => !File.Exists(item.TargetPath) || !NormalizeBytes(File.ReadAllBytes(item.TargetPath)).SequenceEqual(item.DesiredBytes))
            .ToArray();
        foreach (var item in writes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(item.TargetPath)!);
            var temporary = item.TargetPath + $".doroti-promote-{Guid.NewGuid():N}.tmp";
            try
            {
                File.WriteAllBytes(temporary, item.DesiredBytes);
                File.Move(temporary, item.TargetPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporary))
                {
                    File.Delete(temporary);
                }
            }
        }

        var report = new PromotionReport(
            "doroti.framework-promotion-result/v1",
            true,
            writes.Length,
            diff.Changes.Select(change => new PromotionChange(
                change.ElementId,
                change.Candidate,
                change.Target,
                change.Status == "add" || change.Status == "clean-update" ? "written" : "unchanged",
                change.DesiredSha256)).ToArray());
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, "promotion-report.json"), report);
        return report;
    }

    public static PromotionManifest ReadAndValidateManifest(string repositoryRoot, string manifestPath) =>
        LoadAndValidate(repositoryRoot, manifestPath).Manifest;

    private static PromotionDiffReport CreateDiff(string repositoryRoot, string manifestPath, string outputDirectory, string operation) =>
        CreateDiff(LoadAndValidate(repositoryRoot, manifestPath), outputDirectory, operation);

    private static PromotionDiffReport CreateDiff(PromotionContext context, string outputDirectory, string operation)
    {
        Directory.CreateDirectory(outputDirectory);
        var changes = new List<PromotionDiffChange>();
        var markdown = new StringBuilder();
        markdown.AppendLine($"# Framework promotion {operation}");
        markdown.AppendLine();
        markdown.AppendLine("This report compares the reviewed old candidate, current product source, and new candidate. Conflicts never overwrite product source.");
        foreach (var item in context.Items)
        {
            var currentExists = File.Exists(item.TargetPath);
            var currentBytes = currentExists ? NormalizeBytes(File.ReadAllBytes(item.TargetPath)) : null;
            var currentSha = currentBytes is null ? "missing" : Sha256(currentBytes);
            var baseSha = Sha256(item.BaseBytes);
            var desiredSha = Sha256(item.DesiredBytes);
            var status = currentBytes is null
                ? item.Manifest.TargetBaseSha256 == "missing" ? "add" : "conflict"
                : currentBytes.SequenceEqual(item.DesiredBytes)
                    ? "unchanged"
                    : currentBytes.SequenceEqual(item.BaseBytes) ? "clean-update" : "conflict";
            changes.Add(new(
                item.Manifest.ElementId,
                item.Manifest.Candidate,
                item.Manifest.BaseCandidate,
                item.Manifest.Target,
                status,
                baseSha,
                currentSha,
                desiredSha));
            markdown.AppendLine();
            markdown.AppendLine($"## {item.Manifest.ElementId}");
            markdown.AppendLine();
            markdown.AppendLine($"- Status: `{status}`");
            markdown.AppendLine($"- Old candidate: `{item.Manifest.BaseCandidate}` (`{baseSha}`)");
            markdown.AppendLine($"- Current source: `{item.Manifest.Target}` (`{currentSha}`)");
            markdown.AppendLine($"- New candidate: `{item.Manifest.Candidate}` (`{desiredSha}`)");
        }

        var report = new PromotionDiffReport(
            operation == "rebase" ? "doroti.framework-promotion-rebase/v1" : "doroti.framework-promotion-diff/v1",
            operation,
            changes.All(change => change.Status != "conflict"),
            changes.ToArray());
        ArtifactFiles.WriteJson(Path.Combine(outputDirectory, $"{operation}-report.json"), report);
        ArtifactFiles.WriteUtf8(Path.Combine(outputDirectory, $"{operation}.md"), markdown.ToString());
        return report;
    }

    private static PromotionContext LoadAndValidate(string repositoryRoot, string manifestPath)
    {
        var manifest = ArtifactFiles.ReadJson<PromotionManifest>(manifestPath);
        if (manifest.SchemaVersion != ManifestSchema)
        {
            throw new InvalidDataException($"Unsupported promotion schema: {manifest.SchemaVersion}");
        }
        if (string.IsNullOrWhiteSpace(manifest.UpstreamRevision) || manifest.UpstreamRevision.Length != 40)
        {
            throw new InvalidDataException("Promotion manifest must pin a 40-character upstream revision.");
        }
        if (string.IsNullOrWhiteSpace(manifest.CompilerIdentity.MigrationIrVersion) ||
            string.IsNullOrWhiteSpace(manifest.CompilerIdentity.LoweringRuleSetVersion) ||
            string.IsNullOrWhiteSpace(manifest.CompilerIdentity.EmitterVersion))
        {
            throw new InvalidDataException("Promotion manifest must pin IR, lowering, and emitter identities.");
        }

        var candidateRoot = RepositoryPaths.ResolveWithin(repositoryRoot, manifest.CandidateRoot);
        var productRoot = RepositoryPaths.ResolveWithin(repositoryRoot, manifest.ProductRoot);
        if (IsWithin(candidateRoot, productRoot) || IsWithin(productRoot, candidateRoot))
        {
            throw new InvalidDataException("Generated candidate root and product source root must be physically separate.");
        }
        if (manifest.Items.Length == 0)
        {
            throw new InvalidDataException("Promotion manifest contains no reviewed symbols.");
        }

        var duplicateElement = manifest.Items.GroupBy(item => item.ElementId, StringComparer.Ordinal).FirstOrDefault(group => group.Count() > 1);
        if (duplicateElement is not null)
        {
            throw new InvalidDataException($"Promotion manifest contains a duplicate element: {duplicateElement.Key}");
        }

        var resolved = new List<ResolvedPromotionItem>();
        foreach (var item in manifest.Items.OrderBy(item => item.ElementId, StringComparer.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(item.ElementId) || string.IsNullOrWhiteSpace(item.Symbol) ||
                string.IsNullOrWhiteSpace(item.Reviewer) || item.ReviewState != "approved")
            {
                throw new InvalidDataException($"Promotion item must have an element, symbol, reviewer, and approved state: {item.ElementId}");
            }
            if (item.SourceSpan.Offset < 0 || item.SourceSpan.Length <= 0 || item.ValidationCases.Length == 0)
            {
                throw new InvalidDataException($"Promotion item must record a positive Dart span and validation cases: {item.ElementId}");
            }
            if (item.Issues.Any(issue => !IssueCategories.Contains(issue.Category) || string.IsNullOrWhiteSpace(issue.Status) || string.IsNullOrWhiteSpace(issue.Detail)))
            {
                throw new InvalidDataException($"Promotion item has an invalid issue classification: {item.ElementId}");
            }
            if (item.Issues.Any(issue => issue.Category == "compiler-general" && issue.Status != "resolved-in-compiler"))
            {
                throw new InvalidDataException($"compiler-general issues must be closed by a compiler fix and regeneration: {item.ElementId}");
            }
            if (item.Patch is not null && (string.IsNullOrWhiteSpace(item.FixOwner) ||
                item.Issues.Length == 0 || item.Issues.Any(issue => issue.Category == "compiler-general") ||
                item.Issues.Any(issue => issue.Status != "approved-adaptation")))
            {
                throw new InvalidDataException($"Only approved non-compiler-general adaptations may carry a symbol patch: {item.ElementId}");
            }

            var candidatePath = RepositoryPaths.ResolveWithin(repositoryRoot, item.Candidate);
            var baseCandidatePath = RepositoryPaths.ResolveWithin(repositoryRoot, item.BaseCandidate);
            var targetPath = RepositoryPaths.ResolveWithin(repositoryRoot, item.Target);
            EnsureWithin(candidateRoot, candidatePath, "candidate", item.ElementId);
            EnsureWithin(candidateRoot, baseCandidatePath, "base candidate", item.ElementId);
            EnsureWithin(productRoot, targetPath, "target", item.ElementId);
            if (!item.Candidate.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
                !item.BaseCandidate.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
                !item.Target.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || item.Target.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"Candidates must be .g.cs and promoted targets must be ordinary .cs: {item.ElementId}");
            }
            EnsureHash(candidatePath, item.CandidateSha256, $"candidate for {item.ElementId}");
            EnsureHash(baseCandidatePath, item.BaseCandidateSha256, $"base candidate for {item.ElementId}");
            foreach (var alias in item.CandidateAliases ?? [])
            {
                var aliasPath = RepositoryPaths.ResolveWithin(repositoryRoot, alias.Path);
                EnsureWithin(candidateRoot, aliasPath, "candidate alias", item.ElementId);
                EnsureHash(aliasPath, alias.Sha256, $"candidate alias for {item.ElementId}");
                if (!File.ReadAllBytes(aliasPath).SequenceEqual(File.ReadAllBytes(candidatePath)))
                {
                    throw new InvalidDataException($"Candidate alias is not byte-identical for {item.ElementId}: {alias.Path}");
                }
            }

            var sourcePath = ResolveSource(repositoryRoot, item.DartSource);
            var source = File.ReadAllText(sourcePath);
            if (item.SourceSpan.Offset + item.SourceSpan.Length > source.Length ||
                !source.AsSpan(item.SourceSpan.Offset, item.SourceSpan.Length).Contains(item.Symbol, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"Recorded Dart element span does not contain {item.Symbol}: {item.DartSource}");
            }

            var baseBytes = ApplyPatch(repositoryRoot, item, baseCandidatePath, item.BasePatch);
            var desiredBytes = ApplyPatch(repositoryRoot, item, candidatePath, item.Patch);
            var baseSha = Sha256(baseBytes);
            if (item.TargetBaseSha256 != "missing" && !string.Equals(item.TargetBaseSha256, baseSha, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"Target base hash is not the reviewed old candidate for {item.ElementId}.");
            }
            resolved.Add(new(item, candidatePath, baseCandidatePath, targetPath, baseBytes, desiredBytes));
        }
        foreach (var targetGroup in resolved.GroupBy(item => item.TargetPath, StringComparer.OrdinalIgnoreCase))
        {
            if (targetGroup.Select(item => Sha256(item.BaseBytes)).Distinct(StringComparer.Ordinal).Count() != 1 ||
                targetGroup.Select(item => Sha256(item.DesiredBytes)).Distinct(StringComparer.Ordinal).Count() != 1)
            {
                throw new InvalidDataException($"Symbols sharing target {targetGroup.Key} do not compose to byte-identical source.");
            }
        }
        return new(manifest, resolved.ToArray());
    }

    private static byte[] ApplyPatch(string repositoryRoot, PromotionItem item, string sourcePath, PromotionPatchReference? reference)
    {
        var text = NormalizeText(File.ReadAllText(sourcePath));
        const string generatedHeader = "// <auto-generated />\n";
        const string reviewedHeader = "// <doroti-reviewed-framework-source />\n";
        if (!text.StartsWith(generatedHeader, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Mechanical candidate is missing its generated marker: {item.ElementId}");
        }
        text = reviewedHeader + text[generatedHeader.Length..];
        if (reference is null)
        {
            return new UTF8Encoding(false).GetBytes(text);
        }
        var patchPath = RepositoryPaths.ResolveWithin(repositoryRoot, reference.Path);
        EnsureHash(patchPath, reference.Sha256, $"symbol patch for {item.ElementId}");
        var patch = ArtifactFiles.ReadJson<PromotionSymbolPatch>(patchPath);
        if (patch.SchemaVersion != "doroti.framework-symbol-patch/v1" || patch.ElementId != item.ElementId || patch.Replacements.Length == 0)
        {
            throw new InvalidDataException($"Invalid symbol patch for {item.ElementId}.");
        }
        foreach (var replacement in patch.Replacements)
        {
            var first = text.IndexOf(replacement.Before, StringComparison.Ordinal);
            if (first < 0 || text.IndexOf(replacement.Before, first + replacement.Before.Length, StringComparison.Ordinal) >= 0)
            {
                throw new InvalidDataException($"Symbol patch match must occur exactly once for {item.ElementId}.");
            }
            text = string.Concat(text.AsSpan(0, first), replacement.After, text.AsSpan(first + replacement.Before.Length));
        }
        return new UTF8Encoding(false).GetBytes(text);
    }

    private static string ResolveSource(string repositoryRoot, string relativePath)
    {
        var workspaceRoot = Directory.GetParent(repositoryRoot)?.FullName
            ?? throw new DirectoryNotFoundException("Doroti workspace root is unavailable.");
        var fullPath = Path.GetFullPath(relativePath, repositoryRoot);
        EnsureWithin(workspaceRoot, fullPath, "Dart source", relativePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Recorded Dart source is missing.", fullPath);
        }
        return fullPath;
    }

    private static void EnsureHash(string path, string expected, string description)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Missing {description}.", path);
        }
        var actual = ArtifactFiles.Sha256(path);
        if (!string.Equals(actual, expected, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"SHA-256 mismatch for {description}: expected {expected}, actual {actual}.");
        }
    }

    private static void EnsureWithin(string root, string path, string kind, string elementId)
    {
        if (!IsWithin(root, path))
        {
            throw new InvalidDataException($"Promotion {kind} escapes its declared root for {elementId}: {path}");
        }
    }

    private static bool IsWithin(string root, string path)
    {
        var prefix = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var fullPath = Path.GetFullPath(path);
        return fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fullPath.TrimEnd(Path.DirectorySeparatorChar), prefix.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
    }

    private static byte[] NormalizeBytes(byte[] bytes) => new UTF8Encoding(false).GetBytes(NormalizeText(Encoding.UTF8.GetString(bytes)));
    private static string NormalizeText(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal);
    private static string Sha256(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
}

public sealed class PromotionConflictException(string message) : InvalidOperationException(message);

public sealed record PromotionManifest(
    string SchemaVersion,
    string CandidateRoot,
    string ProductRoot,
    string UpstreamRevision,
    PromotionCompilerIdentity CompilerIdentity,
    PromotionItem[] Items);
public sealed record PromotionCompilerIdentity(string MigrationIrVersion, string LoweringRuleSetVersion, string EmitterVersion);
public sealed record PromotionItem(
    string ElementId,
    string Symbol,
    string DartSource,
    PromotionSourceSpan SourceSpan,
    string Candidate,
    string CandidateSha256,
    string BaseCandidate,
    string BaseCandidateSha256,
    PromotionCandidateAlias[]? CandidateAliases,
    string Target,
    string TargetBaseSha256,
    string Reviewer,
    string ReviewState,
    PromotionIssue[] Issues,
    string? FixOwner,
    PromotionPatchReference? Patch,
    PromotionPatchReference? BasePatch,
    string[] ValidationCases);
public sealed record PromotionSourceSpan(int Offset, int Length);
public sealed record PromotionCandidateAlias(string Selection, string Path, string Sha256);
public sealed record PromotionIssue(string Category, string Status, string Detail);
public sealed record PromotionPatchReference(string Path, string Sha256);
public sealed record PromotionSymbolPatch(string SchemaVersion, string ElementId, PromotionTextReplacement[] Replacements);
public sealed record PromotionTextReplacement(string Before, string After);
public sealed record PromotionReviewReport(string SchemaVersion, string UpstreamRevision, PromotionCompilerIdentity CompilerIdentity, bool Success, PromotionReviewItem[] Items);
public sealed record PromotionReviewItem(string ElementId, string Symbol, string Candidate, string CandidateSha256, string Target, string Reviewer, string ReviewState, string[] IssueCategories, string[] ValidationCases);
public sealed record PromotionDiffReport(string SchemaVersion, string Operation, bool Success, PromotionDiffChange[] Changes);
public sealed record PromotionDiffChange(string ElementId, string Candidate, string BaseCandidate, string Target, string Status, string BaseSha256, string CurrentSha256, string DesiredSha256);
public sealed record PromotionReport(string SchemaVersion, bool Success, int WrittenCount, PromotionChange[] Changes);
public sealed record PromotionChange(string ElementId, string Candidate, string Target, string Status, string Sha256);

internal sealed record PromotionContext(PromotionManifest Manifest, ResolvedPromotionItem[] Items);
internal sealed record ResolvedPromotionItem(PromotionItem Manifest, string CandidatePath, string BaseCandidatePath, string TargetPath, byte[] BaseBytes, byte[] DesiredBytes);
