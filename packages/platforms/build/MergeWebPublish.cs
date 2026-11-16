using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public sealed class DorotiWebPublishProperties : Microsoft.Build.Utilities.Task
{
    [Output] public string Arguments { get; set; } = string.Empty;
    public override bool Execute()
    {
        var excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "ArtifactsPath", "PublishDir", "OutputPath", "BaseOutputPath", "IntermediateOutputPath", "BaseIntermediateOutputPath",
            "MSBuildProjectExtensionsPath", "DorotiWebExecutionMode", "WasmEnableThreads", "DorotiWebBuildId", "DorotiWebPublishChild",
            "NoBuild", "NoRestore", "UseArtifactsOutput", "ArtifactsPivots", "Configuration"
        };
        var properties = ((IBuildEngine6)BuildEngine).GetGlobalProperties();
        Arguments = string.Join(" ", properties.Where(pair => !excluded.Contains(pair.Key) && !pair.Key.StartsWith("_") && !pair.Key.StartsWith("MSBuild", StringComparison.OrdinalIgnoreCase))
            .Select(pair => "\"-p:" + pair.Key + "=" + pair.Value.Replace("%", "%25").Replace(";", "%3B").Replace(",", "%2C").Replace("\"", "%22").Replace("\r", "%0D").Replace("\n", "%0A") + "\""));
        return true;
    }
}

public sealed class DorotiMergeWebPublish : Microsoft.Build.Utilities.Task
{
    [Required] public string ThreadedRoot { get; set; } = string.Empty;
    [Required] public string CompatibleRoot { get; set; } = string.Empty;
    [Required] public string OutputRoot { get; set; } = string.Empty;
    [Required] public string BuildId { get; set; } = string.Empty;

    public override bool Execute()
    {
        string? staging = null;
        try
        {
            var roots = new[] { Path.GetFullPath(ThreadedRoot), Path.GetFullPath(CompatibleRoot) };
            var output = Path.GetFullPath(OutputRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (Path.GetFileName(output) != "wwwroot" || roots.Any(root => output.StartsWith(root, StringComparison.OrdinalIgnoreCase) || root.StartsWith(output, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException("Dual output must be a separate wwwroot directory.");
            foreach (var root in roots.Concat(new[] { output })) ValidateTree(root);
            var profiles = roots.Select(root => JObject.Parse(File.ReadAllText(Path.Combine(root, "doroti.runtime-profile.json")))).ToArray();
            for (var i = 0; i < profiles.Length; i++)
            {
                var profile = profiles[i];
                if ((int?)profile["schemaVersion"] != 1 || profile["threads"]?.Type != JTokenType.Boolean || (bool?)profile["threads"] != (i == 0) || (string?)profile["buildId"] != BuildId ||
                    !new[] { "mono-10", "mono-11" }.Contains((string?)profile["runtimeContract"]) ||
                    !Regex.IsMatch((string?)profile["runtimeVersion"] ?? "", (string?)profile["runtimeContract"] == "mono-11" ? @"^11\.\d+\.\d+(?:[-+].*)?$" : @"^10\.\d+\.\d+(?:[-+].*)?$") || !(profile["provenance"] is JObject))
                    throw new InvalidDataException("Invalid child runtime generation/Mono/thread profile.");
            }
            foreach (var field in new[] { "runtimeContract", "runtimeVersion", "provenance" })
                if (!JToken.DeepEquals(profiles[0][field], profiles[1][field]))
                    throw new InvalidDataException("Child runtime/toolchain provenance differs: " + field);

            var common = roots.Select(root => Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                .Select(file => file.Substring(root.TrimEnd(Path.DirectorySeparatorChar).Length + 1).Replace('\\', '/'))
                .Where(path => !path.StartsWith("_framework/", StringComparison.Ordinal) && !path.StartsWith("doroti.runtime-profile.json", StringComparison.Ordinal) &&
                    !path.StartsWith("index.html", StringComparison.Ordinal)).OrderBy(path => path, StringComparer.Ordinal).ToArray()).ToArray();
            if (!common[0].SequenceEqual(common[1])) throw new InvalidDataException("Child common asset inventories differ.");
            foreach (var path in common[0])
                if (!File.ReadAllBytes(Path.Combine(roots[0], path)).SequenceEqual(File.ReadAllBytes(Path.Combine(roots[1], path))))
                    throw new InvalidDataException("Conflicting common asset: " + path);
            var originalHtml = roots.Select(root => File.ReadAllText(Path.Combine(root, "index.html"))).ToArray();
            var html = originalHtml.Select(NormalizeHtml).ToArray();
            if (html[0] != html[1]) throw new InvalidDataException("Child application HTML differs after removing runtime preload/imports.");
            var variants = new JObject();
            var modes = new[] { "threaded", "compatible" };
            var activeFrameworkFiles = new HashSet<string>[roots.Length];
            for (var i = 0; i < roots.Length; i++)
            {
                var framework = Path.Combine(roots[i], "_framework");
                var loader = ResolveLoader(framework, originalHtml[i]);
                activeFrameworkFiles[i] = ValidateFramework(framework, loader, profiles[i]);
                var prefix = "runtimes/" + modes[i] + "/";
                variants[modes[i]] = new JObject { ["threads"] = i == 0, ["loaderUrl"] = prefix + "_framework/" + loader,
                    ["profileUrl"] = prefix + "doroti.runtime-profile.json",
                    ["renderers"] = i == 0 ? new JArray("worker-direct-webgl", "worker-direct-webgpu") : new JArray("worker-direct-webgl") };
            }
            // Build a fresh tree, including only this generation's fingerprints.
            // A failed copy leaves the last complete deployment untouched.
            staging = output + ".doroti-stage-" + Guid.NewGuid().ToString("N");
            Directory.CreateDirectory(staging);
            foreach (var path in common[0]) Copy(Path.Combine(roots[0], path), Path.Combine(staging, path));
            for (var i = 0; i < roots.Length; i++)
            {
                var framework = Path.Combine(roots[i], "_framework");
                foreach (var file in Directory.GetFiles(framework, "*", SearchOption.AllDirectories))
                {
                    var relative = file.Substring(framework.Length + 1).Replace('\\', '/');
                    var payload = Regex.Replace(relative, @"\.(?:gz|br)$", "", RegexOptions.CultureInvariant);
                    // SDK publish can leave the previous compiler/assembly
                    // fingerprints in a reused child. Include only the active
                    // SDK inventory and its compressed companions. Keep stable
                    // ancillary files (maps/config/support scripts) unchanged.
                    if (IsSdkFingerprint(payload) && !activeFrameworkFiles[i].Contains(payload)) continue;
                    Copy(file, Path.Combine(staging, "runtimes", modes[i], "_framework", relative));
                }
                foreach (var suffix in new[] { "", ".gz", ".br" })
                {
                    var source = Path.Combine(roots[i], "doroti.runtime-profile.json" + suffix);
                    var destination = Path.Combine(staging, "runtimes", modes[i], "doroti.runtime-profile.json" + suffix);
                    if (File.Exists(source)) Copy(source, destination);
                    else File.Delete(destination);
                }
            }
            var document = Regex.Replace(html[0], "</head\\s*>", "<meta name=\"doroti-build-id\" content=\"" +
                System.Net.WebUtility.HtmlEncode(BuildId) + "\"></head>", RegexOptions.IgnoreCase);
            if (document == html[0]) throw new InvalidDataException("Application HTML must contain a head element for the deployment generation.");
            File.WriteAllText(Path.Combine(staging, "index.html"), document, new UTF8Encoding(false));
            var manifest = new JObject { ["schemaVersion"] = 2, ["hostContract"] = "doroti.web.execution/v1", ["buildId"] = BuildId,
                ["runtimeContract"] = profiles[0]["runtimeContract"]!.DeepClone(), ["runtimeVersion"] = profiles[0]["runtimeVersion"]!.DeepClone(),
                ["provenance"] = profiles[0]["provenance"]!.DeepClone(), ["variants"] = variants };
            File.WriteAllText(Path.Combine(staging, "doroti.runtime-profile.json"), manifest.ToString(Formatting.Indented), new UTF8Encoding(false));
            DorotiValidateWebPublish.Validate(staging, BuildId);
            var backup = output + ".doroti-previous-" + Guid.NewGuid().ToString("N");
            var previous = Directory.Exists(output);
            if (previous) Directory.Move(output, backup);
            try { Directory.Move(staging, output); staging = null; }
            catch { if (previous) Directory.Move(backup, output); throw; }
            if (previous)
                try { Directory.Delete(backup, true); }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
                { Log.LogWarning("Complete publish installed; previous deployment cleanup failed: " + error.Message); }
            return true;
        }
        catch (Exception error) when (error is IOException || error is InvalidDataException || error is UnauthorizedAccessException || error is JsonException || error is ArgumentException || error is FormatException || error is InvalidCastException || error is OverflowException)
        {
            Log.LogError("DOROTIWEB045: Dual publish failed: " + error.Message);
            return false;
        }
        finally
        {
            if (staging != null && Directory.Exists(staging))
                try { Directory.Delete(staging, true); }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
                { Log.LogWarning("Publish staging cleanup failed: " + error.Message); }
        }
    }

    private static void Copy(string source, string destination)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? throw new InvalidDataException("Missing destination directory."));
        File.Copy(source, destination, true);
    }
    internal static string ResolveLoader(string framework, string html)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match match in Regex.Matches(html, @"<script\b[^>]*type\s*=\s*[""']importmap[""'][^>]*>(.*?)</script>", RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            var imports = JObject.Parse(match.Groups[1].Value)["imports"] as JObject;
            var mapped = (string?)(imports?["./_framework/dotnet.js"] ?? imports?["/_framework/dotnet.js"] ?? imports?["_framework/dotnet.js"]);
            if (mapped == null) continue;
            var relative = mapped.StartsWith("./", StringComparison.Ordinal) ? mapped.Substring(2) : mapped.StartsWith("/", StringComparison.Ordinal) ? mapped.Substring(1) : mapped;
            if (!relative.StartsWith("_framework/", StringComparison.Ordinal) || relative.Substring("_framework/".Length).Contains('/') || relative.Contains('\\') || relative.Contains('?') || relative.Contains('#'))
                throw new InvalidDataException("The SDK dotnet loader mapping must reference this publish's framework directory.");
            names.Add(relative.Substring("_framework/".Length));
        }
        if (names.Count == 0 && File.Exists(Path.Combine(framework, "dotnet.js"))) names.Add("dotnet.js");
        if (names.Count != 1 || !File.Exists(Path.Combine(framework, names.Single())))
            throw new InvalidDataException("Missing or ambiguous SDK dotnet loader import-map metadata.");
        return names.Single();
    }
    internal static void ValidateTree(string root)
    {
        for (var current = root; !string.IsNullOrEmpty(current); current = Path.GetDirectoryName(current))
            if (Directory.Exists(current) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidDataException("Linked publish ancestor: " + current);
        if (!Directory.Exists(root)) return;
        var pending = new Stack<string>(); pending.Push(root);
        while (pending.Count > 0)
            foreach (var entry in Directory.EnumerateFileSystemEntries(pending.Pop()))
            {
                var attributes = File.GetAttributes(entry);
                if ((attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Linked publish entry: " + entry);
                if ((attributes & FileAttributes.Directory) != 0) pending.Push(entry);
            }
    }
    private static bool IsSdkFingerprint(string path) =>
        Regex.IsMatch(path, @"(?:^|/)dotnet(?:\.[^/]*)?\.(?:js|mjs|wasm)$", RegexOptions.CultureInvariant) ||
        Regex.IsMatch(path, @"\.[A-Za-z0-9_-]{10}\.(?:js|mjs|wasm|dat|dll|pdb)$", RegexOptions.CultureInvariant);

    internal static HashSet<string> ValidateFramework(string framework, string loader, JObject profile)
    {
        if (!Regex.IsMatch(loader, @"^dotnet(?:\.[A-Za-z0-9_-]+)?\.js$") ||
            loader.StartsWith("dotnet.native", StringComparison.Ordinal) || loader.StartsWith("dotnet.runtime", StringComparison.Ordinal) ||
            !File.Exists(Path.Combine(framework, loader)))
            throw new InvalidDataException("Incomplete or invalid framework: " + framework);

        // Both qualified .NET 10/11 SDKs embed their exact boot resource names.
        // A wildcard match can incorrectly accept another generation's native
        // file when the active loader's actual dependency is missing.
        var metadata = Regex.Match(File.ReadAllText(Path.Combine(framework, loader)),
            @"/\*json-start\*/(.*?)/\*json-end\*/", RegexOptions.Singleline | RegexOptions.CultureInvariant);
        if (!metadata.Success || JObject.Parse(metadata.Groups[1].Value)["resources"] is not JObject resources)
            throw new InvalidDataException("Missing or unsupported SDK inline boot resources: " + loader);
        var active = new HashSet<string>(StringComparer.Ordinal) { loader };
        var root = Path.GetFullPath(framework).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var ownedGroups = new HashSet<string>(new[] { "jsModuleNative", "jsModuleRuntime", "jsModuleWorker", "jsModuleDiagnostics",
            "wasmNative", "icu", "coreAssembly", "assembly", "pdb", "corePdb", "vfs", "coreVfs", "wasmSymbols", "satelliteResources" }, StringComparer.Ordinal);
        foreach (var group in resources.Properties())
        {
            if (group.Value is not JContainer container) continue;
            foreach (var resource in container.DescendantsAndSelf().OfType<JObject>())
            {
                if (resource["name"] is not JValue { Type: JTokenType.String } name) continue;
                var relative = ((string)name!).Replace('\\', '/');
                if (group.Name == "satelliteResources" && resource.Parent?.Parent is JProperty culture)
                    relative = culture.Name + "/" + relative;
                if (relative.StartsWith("./", StringComparison.Ordinal)) relative = relative.Substring(2);
                var inside = relative.Length > 0 && !Path.IsPathRooted(relative) && !relative.Contains(':') &&
                    !relative.Contains('?') && !relative.Contains('#');
                var absolute = inside ? Path.GetFullPath(Path.Combine(framework, relative)) : "";
                inside = inside && absolute.StartsWith(root, StringComparison.Ordinal);
                // Initializer modules may live in the common application's
                // _content tree; native/managed framework payloads may not.
                if (!inside)
                {
                    if (ownedGroups.Contains(group.Name)) throw new InvalidDataException("Invalid SDK framework resource path: " + relative);
                    continue;
                }
                if (File.Exists(absolute)) active.Add(Path.GetRelativePath(framework, absolute).Replace('\\', '/'));
                else if (ownedGroups.Contains(group.Name)) throw new InvalidDataException("Missing active SDK resource: " + relative);
            }
        }
        void RequireNative(string group, string prefix, string extension)
        {
            if (resources[group] is not JArray { Count: 1 } entries || entries[0] is not JObject entry || entry["name"]?.Type != JTokenType.String)
                throw new InvalidDataException("Missing or unsupported SDK native resource group: " + group);
            var name = (string)entry["name"]!;
            if (Path.GetFileName(name) != name || !name.StartsWith(prefix, StringComparison.Ordinal) ||
                !name.EndsWith(extension, StringComparison.Ordinal) || !active.Contains(name))
                throw new InvalidDataException("Invalid active SDK native resource: " + group);
        }
        RequireNative("wasmNative", "dotnet.native.", ".wasm");
        RequireNative("jsModuleNative", "dotnet.native.", ".js");
        RequireNative("jsModuleRuntime", "dotnet.runtime.", ".js");
        if ((bool?)profile["threads"] == true && (string?)profile["runtimeContract"] == "mono-10")
            RequireNative("jsModuleWorker", "dotnet.native.worker.", ".mjs");
        return active;
    }
    private static string NormalizeHtml(string html)
    {
        html = Regex.Replace(html, @"<link\b[^>]*\b(?:href\s*=\s*[""'][^""']*_framework/[^""']*[""']|id\s*=\s*[""']webassembly[""'])[^>]*>", "", RegexOptions.IgnoreCase);
        return Regex.Replace(html, @"<script\b[^>]*type\s*=\s*[""']importmap[""'][^>]*>(.*?)</script>", match =>
        {
            var map = JObject.Parse(match.Groups[1].Value);
            var imports = map["imports"] as JObject;
            if (imports != null)
                foreach (var property in imports.Properties().Where(p => p.Name.Contains("_framework/") || p.Value.ToString().Contains("_framework/")).ToArray()) property.Remove();
            if (map["scopes"] is JObject scopes && scopes.Properties().Any()) throw new InvalidDataException("Scoped import maps require explicit dual-runtime handling.");
            // Integrity entries for the old root framework must not preload it.
            var integrity = map["integrity"] as JObject;
            if (integrity != null)
                foreach (var property in integrity.Properties().Where(p => p.Name.Contains("_framework/")).ToArray()) property.Remove();
            return "<script type=\"importmap\">" + map.ToString(Formatting.None) + "</script>";
        }, RegexOptions.IgnoreCase | RegexOptions.Singleline);
    }
}

public sealed class DorotiValidateWebPublish : Microsoft.Build.Utilities.Task
{
    [Required] public string Root { get; set; } = string.Empty;
    public string BuildId { get; set; } = string.Empty;
    public override bool Execute()
    {
        try { Validate(Path.GetFullPath(Root), BuildId); return true; }
        catch (Exception error) when (error is IOException || error is InvalidDataException || error is UnauthorizedAccessException || error is JsonException || error is ArgumentException || error is FormatException || error is InvalidCastException || error is OverflowException)
        { Log.LogError("DOROTIWEB046: Invalid published deployment: " + error.Message); return false; }
    }
    internal static void Validate(string root, string buildId)
    {
        DorotiMergeWebPublish.ValidateTree(root);
        foreach (var asset in new[] { "index.html", "doroti_bootstrap.js", "_content/Doroti.Host.Web/doroti.loader.js",
            "_content/Doroti.Host.Web/doroti.web.js", "_content/Doroti.Host.Web/doroti.raster.worker.js" })
            if (!File.Exists(Path.Combine(root, asset))) throw new InvalidDataException("Missing application asset: " + asset);
        var manifest = JObject.Parse(File.ReadAllText(Path.Combine(root, "doroti.runtime-profile.json")));
        ValidateProfile(manifest, buildId, (int?)manifest["schemaVersion"] == 1);
        if ((int?)manifest["schemaVersion"] == 1)
        {
            var framework = Path.Combine(root, "_framework");
            DorotiMergeWebPublish.ValidateFramework(framework, DorotiMergeWebPublish.ResolveLoader(framework, File.ReadAllText(Path.Combine(root, "index.html"))), manifest);
            return;
        }
        if ((int?)manifest["schemaVersion"] != 2 || (string?)manifest["hostContract"] != "doroti.web.execution/v1" ||
            !(manifest["variants"] is JObject variants) || variants.Count != 2 || Directory.Exists(Path.Combine(root, "_framework")))
            throw new InvalidDataException("Expected a complete dual manifest without root runtime assets.");
        foreach (var mode in new[] { "threaded", "compatible" })
        {
            var variant = variants[mode] as JObject ?? throw new InvalidDataException("Missing variant: " + mode);
            var prefix = "runtimes/" + mode + "/";
            var loaderUrl = (string?)variant["loaderUrl"] ?? "";
            if (variant["threads"]?.Type != JTokenType.Boolean || (bool?)variant["threads"] != (mode == "threaded") || (string?)variant["profileUrl"] != prefix + "doroti.runtime-profile.json" ||
                !loaderUrl.StartsWith(prefix + "_framework/", StringComparison.Ordinal) || loaderUrl.Substring((prefix + "_framework/").Length).IndexOfAny(new[] { '/', '\\', '?', '#' }) >= 0)
                throw new InvalidDataException("Invalid variant URLs/threads: " + mode);
            var profile = JObject.Parse(File.ReadAllText(Path.Combine(root, prefix, "doroti.runtime-profile.json")));
            ValidateProfile(profile, (string)manifest["buildId"]!, true);
            if ((bool?)profile["threads"] != (mode == "threaded")) throw new InvalidDataException("Variant thread profile differs: " + mode);
            foreach (var field in new[] { "runtimeContract", "runtimeVersion", "provenance" })
                if (!JToken.DeepEquals(manifest[field], profile[field])) throw new InvalidDataException("Variant generation/toolchain differs: " + field);
            DorotiMergeWebPublish.ValidateFramework(Path.Combine(root, prefix, "_framework"), loaderUrl.Substring((prefix + "_framework/").Length), profile);
        }
    }
    private static void ValidateProfile(JObject profile, string buildId, bool single)
    {
        var runtime = (string?)profile["runtimeContract"];
        if (single && ((int?)profile["schemaVersion"] != 1 || profile["threads"]?.Type != JTokenType.Boolean) ||
            string.IsNullOrEmpty((string?)profile["buildId"]) || buildId.Length != 0 && (string?)profile["buildId"] != buildId ||
            runtime != "mono-10" && runtime != "mono-11" ||
            !Regex.IsMatch((string?)profile["runtimeVersion"] ?? "", runtime == "mono-11" ? @"^11\.\d+\.\d+(?:[-+].*)?$" : @"^10\.\d+\.\d+(?:[-+].*)?$") ||
            !(profile["provenance"] is JObject))
            throw new InvalidDataException("Invalid runtime generation/Mono/toolchain profile.");
    }
}
