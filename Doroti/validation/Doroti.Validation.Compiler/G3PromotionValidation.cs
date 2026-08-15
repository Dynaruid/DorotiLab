using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Doroti.Generated.Framework.Foundation;
using Doroti.Generated.Framework.Physics;
using Doroti.SourceTools;
using Doroti.Tooling;

internal static class G3PromotionValidation
{
    public static G3PromotionResult Validate(string dorotiRoot, List<string> failures)
    {
        var manifestPath = Path.Combine(dorotiRoot, "migration", "promotion.json");
        var tempRoot = RepositoryLocalStorage.CreateTemporaryDirectory(dorotiRoot, "g3-2");
        try
        {
            var review = Promotion.Review(dorotiRoot, manifestPath, Path.Combine(tempRoot, "review"));
            var diff = Promotion.Diff(dorotiRoot, manifestPath, Path.Combine(tempRoot, "review"));
            var manifest = Promotion.ReadAndValidateManifest(dorotiRoot, manifestPath);
            var milestoneItems = G32Items(manifest).ToArray();
            var milestoneElementIds = milestoneItems.Select(item => item.ElementId).ToHashSet(StringComparer.Ordinal);
            var milestoneReviews = review.Items.Where(item => milestoneElementIds.Contains(item.ElementId)).ToArray();
            var milestoneChanges = diff.Changes.Where(item => milestoneElementIds.Contains(item.ElementId)).ToArray();
            var manifestValid = review.Success && milestoneReviews.Length == 5 &&
                milestoneChanges.Length == 5 && milestoneChanges.All(change => change.Status == "unchanged") &&
                milestoneItems.All(item => item.ReviewState == "approved" && item.ValidationCases.Length > 0);
            manifestValid = ValidateEvidence(dorotiRoot, manifestPath) && manifestValid;
            if (!manifestValid)
            {
                failures.Add("G3-2 review manifest or committed promotion diff is invalid.");
            }

            var architectureValid = AuditProductSources(dorotiRoot, manifest, out var architectureFindings);
            failures.AddRange(architectureFindings.Select(finding => $"G3-2 architecture: {finding}"));

            var behaviorValid = ValidateBehavior();
            if (!behaviorValid)
            {
                failures.Add("G3-2 promoted framework behavior differs from the approved candidate cases.");
            }

            var productBuild = RunExternal(
                "dotnet",
                ["build", Path.Combine(dorotiRoot, "Doroti.Product.slnx"), "--configuration", "Release", "--no-restore", "--nologo"],
                dorotiRoot);
            var productCompiled = productBuild.ExitCode == 0;
            if (!productCompiled)
            {
                failures.Add($"G3-2 clean product compile failed: {productBuild.Output}");
            }

            var mechanicalCompileRejected = ValidateMechanicalCompileRejection(dorotiRoot, failures);
            var unmanifestedSourceRejected = ValidateUnmanifestedSourceRejection(dorotiRoot, manifest);
            if (!unmanifestedSourceRejected)
            {
                failures.Add("G3-2 architecture audit did not reject an ordinary .cs addition without a promotion manifest entry.");
            }

            var deterministic = ValidateMirroredDeterminism(dorotiRoot, manifestPath, manifest, tempRoot, failures);
            var conflictBlocked = ValidateConflictBlocker(dorotiRoot, manifestPath, manifest, tempRoot, failures);
            var noCompilerGeneralOverwrite = milestoneItems.All(item =>
                item.Patch is null || item.Issues.All(issue => issue.Category != "compiler-general"));
            if (!noCompilerGeneralOverwrite)
            {
                failures.Add("G3-2 compiler-general fix remains as a promoted-source patch.");
            }

            return new(
                manifestValid && architectureValid && behaviorValid && productCompiled && mechanicalCompileRejected &&
                    unmanifestedSourceRejected && deterministic && conflictBlocked && noCompilerGeneralOverwrite,
                milestoneReviews.Length,
                milestoneItems.Select(item => item.Target).Distinct(StringComparer.OrdinalIgnoreCase).Count(),
                milestoneItems.SelectMany(item => item.CandidateAliases ?? []).Count(alias => alias.Selection == "F0"),
                productCompiled,
                behaviorValid,
                deterministic,
                conflictBlocked,
                mechanicalCompileRejected,
                unmanifestedSourceRejected);
        }
        finally
        {
            RepositoryLocalStorage.DeleteTemporaryDirectory(dorotiRoot, tempRoot);
        }
    }

    private static IEnumerable<PromotionItem> G32Items(PromotionManifest manifest) => manifest.Items.Where(item =>
        item.ValidationCases.Any(validationCase =>
            validationCase.StartsWith("g3-2-", StringComparison.Ordinal) ||
            validationCase.StartsWith("f0-", StringComparison.Ordinal)));

    private static bool ValidateBehavior()
    {
        var category = new Category(new[] { "Framework", "Review" });
        var icon = new DocumentationIcon("https://example.invalid/icon.svg");
        var summary = new Summary("reviewed");
        var defaults = Tolerance.defaultTolerance;
        var custom = new Tolerance(distance: 0.25, time: 0.5, velocity: 0.75);
        return category.sections.SequenceEqual(["Framework", "Review"], StringComparer.Ordinal) &&
            icon.url.EndsWith("icon.svg", StringComparison.Ordinal) && summary.text == "reviewed" &&
            defaults.distance == 0.001 && defaults.time == 0.001 && defaults.velocity == 0.001 &&
            objectRuntimeTypeFunctions.objectRuntimeType(custom, "Tolerance") == "Tolerance" &&
            custom.ToString() == "Tolerance(distance: ±0.25, time: ±0.5, velocity: ±0.75)";
    }

    private static bool ValidateEvidence(string dorotiRoot, string manifestPath)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(dorotiRoot, "migration", "flutter-framework", "g3-2-evidence.json")));
        var root = document.RootElement;
        var manifest = root.GetProperty("promotionManifest");
        var promotedSources = root.GetProperty("promotedSources").EnumerateArray().ToArray();
        using var f0Document = JsonDocument.Parse(File.ReadAllText(Path.Combine(dorotiRoot, "migration", "flutter-framework", "f0-evidence.json")));
        var f0 = f0Document.RootElement;
        return root.GetProperty("schemaVersion").GetString() == "doroti.g3-2-evidence/v1" &&
            root.GetProperty("status").GetString() == "complete" && root.GetProperty("milestoneComplete").GetBoolean() &&
            manifest.GetProperty("reviewedSymbols").GetInt32() == 5 && manifest.GetProperty("promotedFiles").GetInt32() == 3 &&
            manifest.GetProperty("sha256").GetString() == Sha256(manifestPath) &&
            promotedSources.Length == 3 && promotedSources.All(item =>
            {
                var path = Path.Combine(dorotiRoot, item.GetProperty("path").GetString()!.Replace('/', Path.DirectorySeparatorChar));
                return File.Exists(path) && Sha256(path) == item.GetProperty("sha256").GetString();
            }) &&
            f0.GetProperty("status").GetString() == "reviewed-generated-cs" &&
            f0.GetProperty("reviewedGeneratedCs").GetProperty("declarations").GetInt32() == 1 &&
            f0.GetProperty("compiled").GetBoolean() && f0.GetProperty("behaviorVerified").GetBoolean() &&
            f0.GetProperty("milestoneComplete").GetBoolean();
    }

    private static bool AuditProductSources(string dorotiRoot, PromotionManifest manifest, out string[] findings)
    {
        var result = new List<string>();
        var expectedTargets = manifest.Items.Select(item => Normalize(item.Target)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var ownedTargets = CollectFrameworkSourceOwnership(dorotiRoot, expectedTargets);
        var managedDirectories = expectedTargets.Select(path => Path.GetDirectoryName(path.Replace('/', Path.DirectorySeparatorChar))!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var actualTargets = managedDirectories
            .Select(directory => Path.Combine(dorotiRoot, directory))
            .Where(Directory.Exists)
            .SelectMany(directory => Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
            .Where(path => !IsBuildOutput(path))
            .Select(path => Normalize(Path.GetRelativePath(dorotiRoot, path)))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var missing in expectedTargets.Except(actualTargets, StringComparer.OrdinalIgnoreCase))
        {
            result.Add($"Manifest target is missing: {missing}");
        }
        foreach (var unexpected in actualTargets.Except(ownedTargets, StringComparer.OrdinalIgnoreCase))
        {
            result.Add($"Promoted source has no manifest entry: {unexpected}");
        }
        var mechanicalSources = Directory.EnumerateFiles(Path.Combine(dorotiRoot, "src"), "*.g.cs", SearchOption.AllDirectories)
            .Where(path => !IsBuildOutput(path))
            .Select(path => Normalize(Path.GetRelativePath(dorotiRoot, path)))
            .ToArray();
        result.AddRange(mechanicalSources.Select(path => $"Mechanical candidate exists below product source root: {path}"));
        foreach (var item in manifest.Items)
        {
            var target = Path.Combine(dorotiRoot, item.Target.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(target) || !File.ReadAllText(target).StartsWith("// <doroti-reviewed-framework-source />\n", StringComparison.Ordinal))
            {
                result.Add($"Promoted target lacks the reviewed-source marker: {item.Target}");
            }
        }
        findings = result.ToArray();
        return result.Count == 0;
    }

    private static HashSet<string> CollectFrameworkSourceOwnership(string dorotiRoot, IEnumerable<string> promotionTargets)
    {
        var owned = promotionTargets.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var frameworkRoot = Path.Combine(dorotiRoot, "migration", "flutter-framework");
        foreach (var path in Directory.EnumerateFiles(frameworkRoot, "*.json", SearchOption.TopDirectoryOnly))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(path));
            CollectFrameworkSourcePaths(document.RootElement, owned);
        }
        return owned;
    }

    private static void CollectFrameworkSourcePaths(JsonElement element, HashSet<string> owned)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    CollectFrameworkSourcePaths(property.Value, owned);
                }
                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    CollectFrameworkSourcePaths(item, owned);
                }
                break;
            case JsonValueKind.String:
                var value = Normalize(element.GetString()!);
                if (value.StartsWith("src/Doroti.Framework.", StringComparison.Ordinal) &&
                    value.EndsWith(".cs", StringComparison.Ordinal))
                {
                    owned.Add(value);
                }
                break;
        }
    }

    private static bool ValidateUnmanifestedSourceRejection(string dorotiRoot, PromotionManifest manifest)
    {
        var targetDirectory = Path.Combine(dorotiRoot, Path.GetDirectoryName(manifest.Items[0].Target.Replace('/', Path.DirectorySeparatorChar))!);
        var rogue = Path.Combine(targetDirectory, $"Unmanifested-{Guid.NewGuid():N}.cs");
        try
        {
            File.WriteAllText(rogue, "namespace Doroti.Generated.Framework.Foundation; internal class Unmanifested;\n", new UTF8Encoding(false));
            return !AuditProductSources(dorotiRoot, manifest, out var findings) && findings.Any(item => item.Contains("no manifest entry", StringComparison.Ordinal));
        }
        finally
        {
            if (File.Exists(rogue))
            {
                File.Delete(rogue);
            }
        }
    }

    private static bool ValidateMechanicalCompileRejection(string dorotiRoot, List<string> failures)
    {
        var projectRoot = Path.Combine(dorotiRoot, "artifacts", "validation", $"g3-2-mechanical-negative-{Guid.NewGuid():N}");
        Directory.CreateDirectory(projectRoot);
        try
        {
            File.WriteAllText(
                Path.Combine(projectRoot, "MechanicalNegative.csproj"),
                """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                    <IsDorotiProduct>true</IsDorotiProduct>
                    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
                  </PropertyGroup>
                  <ItemGroup><Compile Include="bad.g.cs" /></ItemGroup>
                </Project>
                """ + "\n",
                new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(projectRoot, "bad.g.cs"), "internal class BadCandidate;\n", new UTF8Encoding(false));
            var build = RunExternal("dotnet", ["build", "MechanicalNegative.csproj", "--nologo"], projectRoot);
            var rejected = build.ExitCode != 0 && build.Output.Contains("Mechanical .g.cs candidates cannot enter", StringComparison.Ordinal);
            if (!rejected)
            {
                failures.Add($"G3-2 product graph accepted a mechanical .g.cs compile item: {build.Output}");
            }
            return rejected;
        }
        finally
        {
            if (Directory.Exists(projectRoot))
            {
                Directory.Delete(projectRoot, recursive: true);
            }
        }
    }

    private static bool ValidateMirroredDeterminism(
        string dorotiRoot,
        string manifestPath,
        PromotionManifest manifest,
        string tempRoot,
        List<string> failures)
    {
        var first = Path.Combine(tempRoot, "mirror-a", "Doroti");
        var second = Path.Combine(tempRoot, "different-depth", "mirror-b", "Doroti");
        CreateMirror(dorotiRoot, manifestPath, manifest, first);
        CreateMirror(dorotiRoot, manifestPath, manifest, second);
        var firstManifest = Path.Combine(first, "migration", "promotion.json");
        var secondManifest = Path.Combine(second, "migration", "promotion.json");
        var firstRun = Promotion.Promote(first, firstManifest, Path.Combine(first, "artifacts", "promotion"));
        var incremental = Promotion.Promote(first, firstManifest, Path.Combine(first, "artifacts", "promotion-incremental"));
        var secondRun = Promotion.Promote(second, secondManifest, Path.Combine(second, "artifacts", "promotion"));
        var milestoneItems = G32Items(manifest).ToArray();
        var targetsEqual = milestoneItems.Select(item => Normalize(item.Target)).Distinct(StringComparer.OrdinalIgnoreCase).All(target =>
            File.ReadAllBytes(Path.Combine(first, target.Replace('/', Path.DirectorySeparatorChar)))
                .SequenceEqual(File.ReadAllBytes(Path.Combine(second, target.Replace('/', Path.DirectorySeparatorChar)))) &&
            File.ReadAllBytes(Path.Combine(first, target.Replace('/', Path.DirectorySeparatorChar)))
                .SequenceEqual(File.ReadAllBytes(Path.Combine(dorotiRoot, target.Replace('/', Path.DirectorySeparatorChar)))));
        var candidateBytesEqual = milestoneItems.All(item =>
            File.ReadAllBytes(Path.Combine(first, item.Candidate.Replace('/', Path.DirectorySeparatorChar)))
                .SequenceEqual(File.ReadAllBytes(Path.Combine(second, item.Candidate.Replace('/', Path.DirectorySeparatorChar)))));
        var promotedFileCount = manifest.Items.Select(item => Normalize(item.Target))
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var deterministic = firstRun.WrittenCount == promotedFileCount && incremental.WrittenCount == 0 &&
            secondRun.WrittenCount == promotedFileCount &&
            targetsEqual && candidateBytesEqual;
        if (!deterministic)
        {
            failures.Add("G3-2 clean/incremental/mirrored promotion was not byte-identical.");
        }
        return deterministic;
    }

    private static bool ValidateConflictBlocker(
        string dorotiRoot,
        string manifestPath,
        PromotionManifest manifest,
        string tempRoot,
        List<string> failures)
    {
        var mirror = Path.Combine(tempRoot, "conflict", "Doroti");
        CreateMirror(dorotiRoot, manifestPath, manifest, mirror);
        var mirrorManifest = Path.Combine(mirror, "migration", "promotion.json");
        Promotion.Promote(mirror, mirrorManifest, Path.Combine(mirror, "artifacts", "initial"));
        var document = JsonNode.Parse(File.ReadAllText(mirrorManifest))!.AsObject();
        var item = document["items"]!.AsArray().Select(node => node!.AsObject())
            .Single(node => node["symbol"]!.GetValue<string>() == "Tolerance");
        var candidateRelative = item["candidate"]!.GetValue<string>();
        var candidate = Path.Combine(mirror, candidateRelative.Replace('/', Path.DirectorySeparatorChar));
        var baselineRelative = Path.Combine(Path.GetDirectoryName(candidateRelative)!, "baseline", "tolerance.g.cs").Replace('\\', '/');
        var baseline = Path.Combine(mirror, baselineRelative.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(baseline)!);
        File.Copy(candidate, baseline);
        item["baseCandidate"] = baselineRelative;
        item["baseCandidateSha256"] = Sha256(baseline);
        File.AppendAllText(candidate, "// intentional upstream change\n", new UTF8Encoding(false));
        item["candidateSha256"] = Sha256(candidate);
        var target = Path.Combine(mirror, item["target"]!.GetValue<string>().Replace('/', Path.DirectorySeparatorChar));
        item["targetBaseSha256"] = Sha256(target);
        File.AppendAllText(target, "// intentional local review conflict\n", new UTF8Encoding(false));
        File.WriteAllText(mirrorManifest, document.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n", new UTF8Encoding(false));
        var before = File.ReadAllBytes(target);
        var rebase = Promotion.Rebase(mirror, mirrorManifest, Path.Combine(mirror, "artifacts", "rebase"));
        var threw = false;
        try
        {
            Promotion.Promote(mirror, mirrorManifest, Path.Combine(mirror, "artifacts", "blocked-promote"));
        }
        catch (PromotionConflictException)
        {
            threw = true;
        }
        var blocked = !rebase.Success && rebase.Changes.Single(change => change.ElementId.EndsWith("#Tolerance", StringComparison.Ordinal)).Status == "conflict" &&
            threw && before.SequenceEqual(File.ReadAllBytes(target));
        if (!blocked)
        {
            failures.Add("G3-2 intentional upstream/local conflict overwrote product source or did not block promotion.");
        }
        return blocked;
    }

    private static void CreateMirror(string dorotiRoot, string manifestPath, PromotionManifest manifest, string mirrorRoot)
    {
        Directory.CreateDirectory(mirrorRoot);
        CopyFile(manifestPath, Path.Combine(mirrorRoot, "migration", "promotion.json"));
        foreach (var item in manifest.Items)
        {
            CopyRelative(dorotiRoot, mirrorRoot, item.Candidate);
            CopyRelative(dorotiRoot, mirrorRoot, item.BaseCandidate);
            foreach (var alias in item.CandidateAliases ?? [])
            {
                CopyRelative(dorotiRoot, mirrorRoot, alias.Path);
            }
            if (item.Patch is not null) CopyRelative(dorotiRoot, mirrorRoot, item.Patch.Path);
            if (item.BasePatch is not null) CopyRelative(dorotiRoot, mirrorRoot, item.BasePatch.Path);
            var source = Path.GetFullPath(item.DartSource, dorotiRoot);
            var mirroredSource = Path.GetFullPath(item.DartSource, mirrorRoot);
            CopyFile(source, mirroredSource);
        }
    }

    private static void CopyRelative(string sourceRoot, string targetRoot, string relative) =>
        CopyFile(Path.Combine(sourceRoot, relative.Replace('/', Path.DirectorySeparatorChar)), Path.Combine(targetRoot, relative.Replace('/', Path.DirectorySeparatorChar)));

    private static void CopyFile(string source, string target)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(source, target, overwrite: true);
    }

    private static ExternalResult RunExternal(string fileName, string[] arguments, string workingDirectory)
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
        foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
        process.Start();
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit((int)TimeSpan.FromMinutes(15).TotalMilliseconds))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"G3-2 external validation exceeded 15 minutes: {fileName}");
        }
        Task.WaitAll(output, error);
        return new(process.ExitCode, string.Join('\n', output.Result, error.Result).Trim());
    }

    private static bool IsBuildOutput(string path)
    {
        var normalized = Normalize(path);
        return normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase) || normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase);
    }

    private static string Sha256(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    private static string Normalize(string path) => path.Replace('\\', '/');
    private sealed record ExternalResult(int ExitCode, string Output);
}

internal sealed record G3PromotionResult(
    bool Success,
    int ReviewedSymbolCount,
    int PromotedFileCount,
    int F0AliasCount,
    bool ProductCompiled,
    bool BehaviorVerified,
    bool MirroredDeterministic,
    bool ConflictBlocked,
    bool MechanicalCompileRejected,
    bool UnmanifestedSourceRejected);
