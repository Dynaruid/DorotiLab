using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Doroti.Tooling;

namespace Doroti.SourceTools;

public static partial class G4BoundaryAudit
{
    public const string ReportSchema = "doroti.g4-boundary-audit-report/v1";
    private const string OwnerAuditSchema = "doroti.current-owner-audit/v1";
    private const string SourceBoundarySchema = "doroti.flutter-avalonia-source-boundary/v1";
    private const string CapabilityMapSchema = "doroti.flutter-avalonia-capability-map/v1";
    private const string ProjectBoundarySchema = "doroti.source-port-boundaries/v2";

    private static readonly string[] AuditedProjects =
    {
        "Doroti.Core",
        "Doroti.Platform",
        "Doroti.Rendering",
        "Doroti.Widgets",
        "Doroti.Engine",
        "Doroti.Flutter.Runtime",
    };

    private static readonly HashSet<string> AllowedOwnerDispositions = new(StringComparer.Ordinal)
    {
        "keep-bridge",
        "move-to-framework",
        "move-to-ui-contract",
        "replace-by-avalonia",
        "remove-after-cutover",
    };

    private static readonly HashSet<string> AllowedSourceDispositions = new(StringComparer.Ordinal)
    {
        "flutter-framework",
        "dart-runtime",
        "dart-ui-contract",
        "avalonia-binding",
        "doroti-glue",
        "tooling-only",
        "excluded-with-owner",
        "unsupported-blocker",
    };

    public static G4BoundaryAuditReport Run(string root, bool writeOwnerAudit)
    {
        var migrationRoot = Path.Combine(root, "migration", "flutter-avalonia");
        var ownerAuditPath = Path.Combine(migrationRoot, "current-owner-audit.json");
        var sourceBoundaryPath = Path.Combine(migrationRoot, "source-boundary.json");
        var capabilityMapPath = Path.Combine(migrationRoot, "capability-map.json");
        var projectBoundaryPath = Path.Combine(root, "migration", "avalonia-shell", "source-port-boundaries.json");
        var findings = new List<G4BoundaryFinding>();

        var ownerDocument = CreateOwnerAudit(root);
        if (writeOwnerAudit)
        {
            Directory.CreateDirectory(migrationRoot);
            File.WriteAllText(
                ownerAuditPath,
                JsonSerializer.Serialize(ownerDocument, JsonOptions) + "\n",
                new UTF8Encoding(false));
        }
        else if (!File.Exists(ownerAuditPath))
        {
            findings.Add(new("G4B001", ownerAuditPath, "Current owner audit is missing."));
        }
        else
        {
            var committed = JsonNode.Parse(File.ReadAllText(ownerAuditPath));
            var expected = JsonSerializer.SerializeToNode(ownerDocument, JsonOptions);
            if (!JsonNode.DeepEquals(committed, expected))
            {
                findings.Add(new("G4B002", ownerAuditPath, "Current owner audit is stale; regenerate it with --write-owner-audit."));
            }
        }

        ValidateSourceBoundary(root, sourceBoundaryPath, findings, out var sourceCounts, out var bindingCapabilities);
        ValidateCapabilityMap(capabilityMapPath, bindingCapabilities, findings, out var capabilityCount);
        ValidateProjectBoundary(projectBoundaryPath, findings);

        return new(
            ReportSchema,
            findings.Count == 0,
            ownerDocument.Summary.AuditedSymbolCount,
            sourceCounts,
            capabilityCount,
            findings.ToArray());
    }

    private static CurrentOwnerAuditDocument CreateOwnerAudit(string root)
    {
        var symbols = new List<CurrentOwnerSymbol>();
        foreach (var project in AuditedProjects)
        {
            var directory = Path.Combine(root, "src", project);
            foreach (var path in Directory.EnumerateFiles(directory, "*.cs", SearchOption.TopDirectoryOnly).OrderBy(path => path, StringComparer.Ordinal))
            {
                var text = File.ReadAllText(path);
                var namespaceName = NamespaceRegex().Match(text).Groups[1].Value;
                var names = TypeRegex().Matches(text).Select(match => match.Groups[1].Value)
                    .Concat(DelegateRegex().Matches(text).Select(match => match.Groups[1].Value))
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(name => name, StringComparer.Ordinal);
                foreach (var name in names)
                {
                    var decision = Classify(project, Path.GetFileName(path), name);
                    symbols.Add(new(
                        $"{namespaceName}.{name}",
                        project,
                        ArtifactFiles.NormalizePath(Path.GetRelativePath(root, path)),
                        decision.Disposition,
                        decision.TargetOwner,
                        decision.CutoverMilestone,
                        decision.Reason));
                }
            }
        }

        symbols.Sort((left, right) => string.CompareOrdinal(left.Symbol, right.Symbol));
        var duplicateCount = symbols.GroupBy(item => item.Symbol, StringComparer.Ordinal).Count(group => group.Count() > 1);
        var counts = AllowedOwnerDispositions.ToDictionary(
            disposition => disposition,
            disposition => symbols.Count(item => item.Disposition == disposition),
            StringComparer.Ordinal);
        var unclassified = symbols.Count(item => !AllowedOwnerDispositions.Contains(item.Disposition));
        return new(
            OwnerAuditSchema,
            "G4-0",
            "Declared C# type/delegate symbols at every explicit visibility in the six pre-cutover owner projects; methods inherit their declaring type decision unless a later promotion manifest records a narrower cutover.",
            AuditedProjects,
            new(symbols.Count, duplicateCount, unclassified, counts),
            symbols.ToArray());
    }

    private static OwnerDecision Classify(string project, string file, string symbol)
    {
        if (project == "Doroti.Core")
        {
            return file == "Clock.cs"
                ? new("keep-bridge", "Doroti.Flutter.Runtime or host-neutral scheduling contracts", "G4-3", "Clock/dispatcher abstractions remain neutral only after Scheduler owns frame policy.")
                : new("move-to-framework", "Doroti.Flutter.Framework.Foundation", "G4-2", "Foundation behavior is owned by reviewed Flutter source.");
        }

        if (project == "Doroti.Platform")
        {
            if (file == "HardwareKeyboard.cs")
            {
                return new("move-to-framework", "Doroti.Flutter.Framework.Services", "G4-3", "Pressed-key state is Flutter Services behavior.");
            }
            if (file == "ChannelContracts.cs")
            {
                return symbol is "IBinaryMessenger" or "PlatformMessageHandler"
                    ? new("move-to-ui-contract", "Doroti.Flutter.Ui platform message ABI", "G4-1", "The host-neutral message ABI moves beside dart:ui.")
                    : new("move-to-framework", "Doroti.Flutter.Framework.Services", "G4-3", "Messenger implementation and channel semantics are Flutter Services behavior.");
            }
            if (file == "AccessibilityContracts.cs" && symbol is "SemanticsRole" or "SemanticsState" or "SemanticsAction")
            {
                return new("move-to-framework", "Doroti.Flutter.Framework.Semantics", "G4-5", "Semantics vocabulary and state are Flutter-owned.");
            }
            if (file == "TextAndCursorContracts.cs" && symbol is "TextSelection" or "TextEditingState" or "TextEditingStateReducer" or "TextInputAction")
            {
                return new("move-to-framework", "Doroti.Flutter.Framework.Services", "G4-3", "Text editing state/protocol semantics are Flutter Services behavior.");
            }
            if (symbol.StartsWith("Unsupported", StringComparison.Ordinal))
            {
                return new("remove-after-cutover", "typed Doroti.Flutter.Ui capability failure", "G4-1", "Ad-hoc unsupported implementations are replaced by typed capability errors.");
            }
            return new("keep-bridge", "Doroti.Platform raw DTO/capability contracts", "G4-1", "Raw native DTOs and capability seams remain host neutral.");
        }

        if (project == "Doroti.Rendering")
        {
            if (file is "DisplayList.cs" or "RenderingContracts.cs" or "SceneCommitter.cs")
            {
                return new("keep-bridge", "Doroti rendering/frame protocol", "G4-5", "Immutable display/resource submission is a host-neutral bridge protocol.");
            }
            return new("move-to-framework", "Doroti.Flutter.Framework.Rendering", "G4-5", "Layout, layer, painting and semantics algorithms are Flutter Rendering behavior.");
        }

        if (project == "Doroti.Widgets")
        {
            return new("move-to-framework", "Doroti.Flutter.Framework.Widgets/Gestures/Animation", symbol.Contains("Gesture", StringComparison.Ordinal) ? "G4-4" : "G4-6", "Handwritten Widget, Element, gesture and animation owners are replaced by reviewed Flutter source.");
        }

        if (project == "Doroti.Engine")
        {
            if (file == "InteractiveApplication.cs")
            {
                return new("remove-after-cutover", "Doroti.Flutter.Hosting + host composition", "G4-6", "The handwritten Widget composition root is removed during product cutover.");
            }
            if (file == "ManagedBgraRenderSurface.cs")
            {
                return new("remove-after-cutover", "Avalonia.Skia strict GPU surface", "G4-5", "Managed full-frame software rendering is not a product fallback.");
            }
            return new("keep-bridge", "Doroti host-neutral frame/resource protocol", "G4-5", "Mailbox, ACK, surface generation and diagnostics may remain if symbol parity proves no duplicated framework/platform behavior.");
        }

        if (file == "PlatformChannels.cs")
        {
            if (symbol is "ClipboardPort")
            {
                return new("replace-by-avalonia", "Flutter Services clipboard API + Avalonia platform.services binding", "G4-3", "Runtime cannot own a concrete platform service.");
            }
            return new("move-to-framework", "Doroti.Flutter.Framework.Services", "G4-3", "Codec, MethodChannel and platform exceptions are Flutter Services behavior.");
        }
        if (file == "FoundationRuntimePorts.cs")
        {
            return new("move-to-framework", "Doroti.Flutter.Framework.Foundation", "G4-2", "Flutter diagnostics and error objects are framework behavior, not Dart runtime primitives.");
        }
        return new("keep-bridge", "Doroti.Flutter.Runtime", "G4-1", "Dart async/stream/language primitives remain host neutral with zero host references.");
    }

    private static void ValidateSourceBoundary(
        string repositoryRoot,
        string path,
        List<G4BoundaryFinding> findings,
        out int sourceCount,
        out HashSet<string> bindingCapabilities)
    {
        sourceCount = 0;
        bindingCapabilities = new(StringComparer.Ordinal);
        if (!File.Exists(path))
        {
            findings.Add(new("G4B010", path, "Source boundary manifest is missing."));
            return;
        }
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        RequireSchema(root, SourceBoundarySchema, path, findings);
        var census = root.GetProperty("sourceCensus");
        Check(census.GetProperty("publicRootCount").GetInt32() == 13, "G4B011", path, "Flutter public root census must be 13.", findings);
        Check(census.GetProperty("dartFileCount").GetInt32() == 695, "G4B012", path, "Flutter Dart file census must be 695.", findings);
        Check(census.GetProperty("srcDartFileCount").GetInt32() == 682, "G4B013", path, "Flutter src Dart file census must be 682.", findings);
        using (var sourceLock = JsonDocument.Parse(File.ReadAllText(Path.Combine(repositoryRoot, "validation", "flutter-source.lock.json"))))
        {
            Check(
                census.GetProperty("contentSha256").GetString() == sourceLock.RootElement.GetProperty("censusSha256").GetString(),
                "G4B013",
                path,
                "Flutter source-boundary content hash differs from the pinned census lock.",
                findings);
        }
        Check(root.GetProperty("summary").GetProperty("unclassifiedCount").GetInt32() == 0, "G4B014", path, "Source boundary has unclassified entries.", findings);
        Check(!File.ReadAllText(path).Contains("runtime-binding", StringComparison.Ordinal), "G4B015", path, "Broad runtime-binding is forbidden in G4 completion counts.", findings);

        foreach (var arrayName in new[] { "boundaryUses", "vmPragmas", "nativeMethods", "platformChannels" })
        {
            foreach (var item in root.GetProperty(arrayName).EnumerateArray())
            {
                sourceCount++;
                var disposition = item.GetProperty("disposition").GetString() ?? string.Empty;
                if (!AllowedSourceDispositions.Contains(disposition))
                {
                    findings.Add(new("G4B016", $"{path}:{arrayName}", $"Unsupported source disposition '{disposition}'."));
                }
                var hasOwner = item.TryGetProperty("owner", out var owner);
                if (!item.TryGetProperty("elementId", out var elementId) || string.IsNullOrWhiteSpace(elementId.GetString()) ||
                    !hasOwner || string.IsNullOrWhiteSpace(owner.GetString()))
                {
                    findings.Add(new("G4B017", $"{path}:{arrayName}", "Every boundary entry requires elementId and owner."));
                }
                if (disposition == "avalonia-binding")
                {
                    if (!item.TryGetProperty("capabilityId", out var capability) || string.IsNullOrWhiteSpace(capability.GetString()))
                    {
                        findings.Add(new("G4B018", $"{path}:{arrayName}", "avalonia-binding entry has no capabilityId."));
                    }
                    else
                    {
                        bindingCapabilities.Add(capability.GetString()!);
                    }
                }
                if (arrayName == "boundaryUses")
                {
                    var reachable = item.GetProperty("reachableFromPublicRoot").GetBoolean();
                    var dependencyPath = item.GetProperty("dependencyPath");
                    if (reachable != (dependencyPath.GetArrayLength() > 0))
                    {
                        findings.Add(new("G4B018", $"{path}:{arrayName}", "Public-root reachability and dependencyPath disagree."));
                    }
                }
                if (disposition == "unsupported-blocker" && hasOwner && owner.GetString()!.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    findings.Add(new("G4B019", $"{path}:{arrayName}", "Unsupported blockers cannot claim implementation success."));
                }
            }
        }
        foreach (var conditional in root.GetProperty("conditionalImports").EnumerateArray())
        {
            foreach (var prefix in new[] { "default", "branch" })
            {
                var disposition = conditional.GetProperty($"{prefix}Disposition").GetString() ?? string.Empty;
                if (!AllowedSourceDispositions.Contains(disposition) ||
                    string.IsNullOrWhiteSpace(conditional.GetProperty($"{prefix}Owner").GetString()) ||
                    conditional.GetProperty($"{prefix}Targets").GetArrayLength() == 0)
                {
                    findings.Add(new("G4B019", path, $"Conditional import {prefix} branch is not fully classified."));
                }
            }
        }
    }

    private static void ValidateCapabilityMap(
        string path,
        HashSet<string> bindingCapabilities,
        List<G4BoundaryFinding> findings,
        out int capabilityCount)
    {
        capabilityCount = 0;
        if (!File.Exists(path))
        {
            findings.Add(new("G4B020", path, "Capability map is missing."));
            return;
        }
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        RequireSchema(root, CapabilityMapSchema, path, findings);
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var capability in root.GetProperty("capabilities").EnumerateArray())
        {
            capabilityCount++;
            var id = capability.GetProperty("id").GetString() ?? string.Empty;
            if (!ids.Add(id))
            {
                findings.Add(new("G4B021", id, "Capability id is empty or duplicated."));
            }
            Check(capability.GetProperty("flutterElementPatterns").GetArrayLength() > 0, "G4B022", id, "Capability requires Flutter element patterns.", findings);
            Check(capability.GetProperty("avaloniaUpstreamSymbols").GetArrayLength() > 0, "G4B023", id, "Capability requires Avalonia upstream symbols.", findings);
            foreach (var implementation in capability.GetProperty("implementations").EnumerateArray())
            {
                Check(implementation.GetProperty("os").GetArrayLength() > 0, "G4B024", id, "Implementation requires an OS list.", findings);
                Check(implementation.GetProperty("avaloniaSourcePaths").GetArrayLength() > 0, "G4B025", id, "Implementation requires Avalonia source paths.", findings);
                Check(implementation.GetProperty("localTargets").GetArrayLength() > 0, "G4B026", id, "Implementation requires local targets.", findings);
                Check(!string.IsNullOrWhiteSpace(implementation.GetProperty("validationOwner").GetString()), "G4B027", id, "Implementation requires a validation owner.", findings);
            }
        }
        foreach (var missing in bindingCapabilities.Except(ids, StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal))
        {
            findings.Add(new("G4B028", missing, "Source-boundary avalonia-binding capability is absent from capability-map.json."));
        }
        Check(root.GetProperty("summary").GetProperty("unmappedAvaloniaBindingCount").GetInt32() == 0, "G4B029", path, "Capability map reports unmapped bindings.", findings);
    }

    private static void ValidateProjectBoundary(string path, List<G4BoundaryFinding> findings)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var root = document.RootElement;
        RequireSchema(root, ProjectBoundarySchema, path, findings);
        var rules = root.GetProperty("projectRules").EnumerateArray().Select(rule => rule.GetProperty("id").GetString()).ToHashSet(StringComparer.Ordinal);
        foreach (var required in new[] { "flutter-runtime", "flutter-ui", "flutter-framework", "flutter-hosting", "desktop-flutter-composition", "vendor-source" })
        {
            if (!rules.Contains(required))
            {
                findings.Add(new("G4B030", path, $"Project boundary rule '{required}' is missing."));
            }
        }
        var fixtures = root.GetProperty("failureFixtures").EnumerateArray().ToArray();
        Check(fixtures.Any(item => item.GetProperty("expectedDiagnostic").GetString() == "DOTARCH009"), "G4B031", path, "DOTARCH009 failure fixture is not declared.", findings);
    }

    private static void RequireSchema(JsonElement root, string expected, string path, List<G4BoundaryFinding> findings)
    {
        var actual = root.TryGetProperty("schemaVersion", out var schema) ? schema.GetString() : null;
        if (actual != expected)
        {
            findings.Add(new("G4B040", path, $"Expected schema '{expected}', found '{actual}'."));
        }
    }

    private static void Check(bool condition, string code, string subject, string message, List<G4BoundaryFinding> findings)
    {
        if (!condition)
        {
            findings.Add(new(code, subject, message));
        }
    }

    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    [GeneratedRegex(@"(?m)^\s*namespace\s+([A-Za-z_][\w.]*)\s*[;{]", RegexOptions.CultureInvariant)]
    private static partial Regex NamespaceRegex();

    [GeneratedRegex(@"(?m)^\s*(?:public|internal|private|protected|file)\s+(?:(?:abstract|sealed|static|readonly|partial)\s+)*(?:class|interface|record(?:\s+(?:class|struct))?|struct|enum)\s+([A-Za-z_]\w*)", RegexOptions.CultureInvariant)]
    private static partial Regex TypeRegex();

    [GeneratedRegex(@"(?m)^\s*(?:public|internal|private|protected|file)\s+delegate\s+[^;(]+\s+([A-Za-z_]\w*)\s*\(", RegexOptions.CultureInvariant)]
    private static partial Regex DelegateRegex();
}

public sealed record G4BoundaryAuditReport(
    string SchemaVersion,
    bool Success,
    int CurrentOwnerSymbolCount,
    int SourceBoundaryEntryCount,
    int CapabilityCount,
    G4BoundaryFinding[] Findings);

public sealed record G4BoundaryFinding(string Code, string Subject, string Message);

public sealed record CurrentOwnerAuditDocument(
    string SchemaVersion,
    string Milestone,
    string Scope,
    string[] AuditedProjects,
    CurrentOwnerAuditSummary Summary,
    CurrentOwnerSymbol[] Symbols);

public sealed record CurrentOwnerAuditSummary(
    int AuditedSymbolCount,
    int DuplicateSymbolCount,
    int UnclassifiedCount,
    Dictionary<string, int> Dispositions);

public sealed record CurrentOwnerSymbol(
    string Symbol,
    string CurrentProject,
    string SourcePath,
    string Disposition,
    string TargetOwner,
    string CutoverMilestone,
    string Reason);

internal sealed record OwnerDecision(string Disposition, string TargetOwner, string CutoverMilestone, string Reason);
