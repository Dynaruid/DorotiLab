using Microsoft.Build.Framework;
using Newtonsoft.Json.Linq;

var passed = 0;
foreach (var scenario in new[] { "merge", "mono11", "conflict", "generation", "missing-loader", "scoped-imports", "missing-native",
    "missing-runtime", "missing-worker", "missing-profile", "provenance", "missing-common", "invalid-thread-type", "invalid-schema-type",
    "missing-active-native", "missing-active-runtime", "missing-active-worker", "missing-assembly", "missing-boot-config", "resource-path", "malformed-native-group" })
{
    var root = Path.Combine(Path.GetTempPath(), "doroti-publish-tests-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(root);
    try
    {
        var modes = new[] { "threaded", "compatible" };
        foreach (var mode in modes)
        {
            var child = Path.Combine(root, mode);
            var framework = Path.Combine(child, "_framework");
            Directory.CreateDirectory(framework);
            foreach (var file in new[] { "dotnet." + mode + ".js", "dotnet.obsolete.js", "dotnet.runtime.hash.js", "dotnet.native.hash.js", "dotnet.native.hash.wasm" })
                File.WriteAllText(Path.Combine(framework, file), mode);
            if (mode == "threaded") File.WriteAllText(Path.Combine(framework, "dotnet.native.worker.hash.mjs"), "worker");
            File.WriteAllText(Path.Combine(framework, "Fixture.0123456789.wasm"), "current assembly");
            File.WriteAllText(Path.Combine(framework, "Fixture.abcdefghij.wasm"), "previous assembly");
            File.WriteAllText(Path.Combine(framework, "Fixture.abcdefghij.wasm.br"), "previous compressed assembly");
            File.WriteAllText(Path.Combine(framework, "dotnet.native.stale.wasm"), "previous native");
            File.WriteAllText(Path.Combine(framework, "dotnet.runtime.stale.js"), "previous runtime");
            File.WriteAllText(Path.Combine(framework, "dotnet.native.worker.stale.mjs"), "previous worker");
            File.WriteAllText(Path.Combine(framework, "support.json"), "stable support file");
            Directory.CreateDirectory(Path.Combine(framework, "fr"));
            File.WriteAllText(Path.Combine(framework, "fr", "Fixture.resources.0123456789.wasm"), "active satellite assembly");
            File.WriteAllText(Path.Combine(framework, "fr", "Fixture.resources.abcdefghij.wasm"), "previous satellite assembly");
            var resources = new JObject {
                ["jsModuleNative"] = new JArray(new JObject { ["name"] = "dotnet.native.hash.js" }),
                ["jsModuleRuntime"] = new JArray(new JObject { ["name"] = "dotnet.runtime.hash.js" }),
                ["wasmNative"] = new JArray(new JObject { ["name"] = "dotnet.native.hash.wasm" }),
                ["assembly"] = new JArray(new JObject { ["name"] = "Fixture.0123456789.wasm" }),
                ["satelliteResources"] = new JObject { ["fr"] = new JArray(new JObject { ["name"] = "Fixture.resources.0123456789.wasm" }) }
            };
            if (mode == "threaded" && scenario != "mono11") resources["jsModuleWorker"] = new JArray(new JObject { ["name"] = "dotnet.native.worker.hash.mjs" });
            File.WriteAllText(Path.Combine(framework, "dotnet." + mode + ".js"),
                "export const config = /*json-start*/" + new JObject { ["mainAssemblyName"] = "Fixture", ["resources"] = resources }.ToString() + "/*json-end*/;");
            File.WriteAllText(Path.Combine(child, "doroti_bootstrap.js"), "shared bootstrap");
            var host = Path.Combine(child, "_content", "Doroti.Host.Web"); Directory.CreateDirectory(host);
            foreach (var file in new[] { "doroti.loader.js", "doroti.web.js", "doroti.raster.worker.js" }) File.WriteAllText(Path.Combine(host, file), "shared host");
            File.WriteAllText(Path.Combine(child, "index.html"), "<html><head><link rel=\"preload\" href=\"_framework/dotnet." + mode + ".js\"><script type=\"importmap\">{\"imports\":{\"./_framework/dotnet.js\":\"./_framework/dotnet." + mode + ".js\",\"app\":\"./app.js\"},\"scopes\":{},\"integrity\":{\"./_framework/dotnet.js\":\"hash\"}}</script></head></html>");
            File.WriteAllText(Path.Combine(child, "doroti.runtime-profile.json"), new JObject {
                ["schemaVersion"] = 1, ["threads"] = mode == "threaded", ["buildId"] = "test-generation",
                ["runtimeContract"] = scenario == "mono11" ? "mono-11" : "mono-10", ["runtimeVersion"] = scenario == "mono11" ? "11.0.0" : "10.0.15",
                ["provenance"] = new JObject { ["sdk"] = scenario == "mono11" ? "11.0.100" : "10.0.403",
                    ["targetFramework"] = scenario == "mono11" ? "net11.0" : "net10.0" }
            }.ToString());
        }
        var output = Path.Combine(root, "deploy", "wwwroot");
        Directory.CreateDirectory(Path.Combine(output, "_framework"));
        File.WriteAllText(Path.Combine(output, "doroti.runtime-profile.json"), "previous deployment");
        var staleFramework = Path.Combine(output, "runtimes", "compatible", "_framework"); Directory.CreateDirectory(staleFramework);
        File.WriteAllText(Path.Combine(staleFramework, "dotnet.previous-generation.js"), "stale");
        File.WriteAllText(Path.Combine(output, "removed-common.js"), "stale");
        switch (scenario)
        {
            case "conflict": File.WriteAllText(Path.Combine(root, "compatible", "doroti_bootstrap.js"), "different"); break;
            case "generation":
                var path = Path.Combine(root, "compatible", "doroti.runtime-profile.json");
                var profile = JObject.Parse(File.ReadAllText(path)); profile["buildId"] = "stale"; File.WriteAllText(path, profile.ToString()); break;
            case "missing-loader": File.Delete(Path.Combine(root, "compatible", "_framework", "dotnet.compatible.js")); break;
            case "scoped-imports": File.WriteAllText(Path.Combine(root, "compatible", "index.html"), "<script type=\"importmap\">{\"scopes\":{\"/\":{\"x\":\"./x.js\"}}}</script>"); break;
            case "missing-native": File.Delete(Path.Combine(root, "compatible", "_framework", "dotnet.native.hash.wasm")); break;
            case "missing-runtime": File.Delete(Path.Combine(root, "compatible", "_framework", "dotnet.runtime.hash.js")); break;
            case "missing-worker": File.Delete(Path.Combine(root, "threaded", "_framework", "dotnet.native.worker.hash.mjs")); break;
            case "missing-active-native": File.Delete(Path.Combine(root, "threaded", "_framework", "dotnet.native.hash.wasm")); break;
            case "missing-active-runtime": File.Delete(Path.Combine(root, "threaded", "_framework", "dotnet.runtime.hash.js")); break;
            case "missing-active-worker": File.Delete(Path.Combine(root, "threaded", "_framework", "dotnet.native.worker.hash.mjs")); break;
            case "missing-assembly": File.Delete(Path.Combine(root, "compatible", "_framework", "Fixture.0123456789.wasm")); break;
            case "missing-boot-config": File.WriteAllText(Path.Combine(root, "compatible", "_framework", "dotnet.compatible.js"), "export {};"); break;
            case "resource-path":
                var loaderPath = Path.Combine(root, "compatible", "_framework", "dotnet.compatible.js");
                File.WriteAllText(loaderPath, File.ReadAllText(loaderPath).Replace("Fixture.0123456789.wasm", "../../outside.wasm")); break;
            case "malformed-native-group":
                var malformedLoaderPath = Path.Combine(root, "compatible", "_framework", "dotnet.compatible.js");
                var malformedLoader = File.ReadAllText(malformedLoaderPath);
                var match = System.Text.RegularExpressions.Regex.Match(malformedLoader, @"/\*json-start\*/(.*?)/\*json-end\*/", System.Text.RegularExpressions.RegexOptions.Singleline);
                var malformedConfig = JObject.Parse(match.Groups[1].Value);
                malformedConfig["resources"]!["wasmNative"] = new JArray("invalid");
                File.WriteAllText(malformedLoaderPath, "export const config = /*json-start*/" + malformedConfig + "/*json-end*/;"); break;
            case "missing-profile": File.Delete(Path.Combine(root, "compatible", "doroti.runtime-profile.json")); break;
            case "missing-common": File.Delete(Path.Combine(root, "compatible", "_content", "Doroti.Host.Web", "doroti.loader.js")); break;
            case "provenance":
                var provenancePath = Path.Combine(root, "compatible", "doroti.runtime-profile.json");
                var different = JObject.Parse(File.ReadAllText(provenancePath)); different["provenance"]!["sdk"] = "11.0.100";
                File.WriteAllText(provenancePath, different.ToString()); break;
            case "invalid-thread-type":
            case "invalid-schema-type":
                var malformedPath = Path.Combine(root, "compatible", "doroti.runtime-profile.json");
                var malformed = JObject.Parse(File.ReadAllText(malformedPath));
                malformed[scenario == "invalid-thread-type" ? "threads" : "schemaVersion"] = "invalid";
                File.WriteAllText(malformedPath, malformed.ToString()); break;
        }
        var engine = new TestBuildEngine();
        var merge = new DorotiMergeWebPublish { ThreadedRoot = Path.Combine(root, "threaded"), CompatibleRoot = Path.Combine(root, "compatible"),
            OutputRoot = output, BuildId = "test-generation", BuildEngine = engine };
        var success = merge.Execute();
        if (success != (scenario is "merge" or "mono11")) throw new InvalidOperationException("Unexpected merge result: " + scenario);
        var manifestPath = Path.Combine(output, "doroti.runtime-profile.json");
        if (success)
        {
            var manifest = JObject.Parse(File.ReadAllText(manifestPath));
            if ((int?)manifest["schemaVersion"] != 2 || (string?)manifest["variants"]?["compatible"]?["loaderUrl"] != "runtimes/compatible/_framework/dotnet.compatible.js" ||
                File.ReadAllText(Path.Combine(output, "index.html")).Contains("_framework") || Directory.Exists(Path.Combine(output, "_framework")) ||
                File.Exists(Path.Combine(staleFramework, "dotnet.previous-generation.js")) || File.Exists(Path.Combine(output, "removed-common.js")))
                throw new InvalidOperationException("Invalid merged deployment shape.");
            foreach (var mode in modes)
            {
                var framework = Path.Combine(output, "runtimes", mode, "_framework");
                if (!File.Exists(Path.Combine(framework, "dotnet.native.hash.wasm")) ||
                    !File.Exists(Path.Combine(framework, "Fixture.0123456789.wasm")) || !File.Exists(Path.Combine(framework, "support.json")) ||
                    !File.Exists(Path.Combine(framework, "fr", "Fixture.resources.0123456789.wasm")))
                    throw new InvalidOperationException("Missing runtime variant.");
                foreach (var stale in new[] { "dotnet.obsolete.js", "dotnet.native.stale.wasm", "dotnet.runtime.stale.js",
                    "dotnet.native.worker.stale.mjs", "Fixture.abcdefghij.wasm", "Fixture.abcdefghij.wasm.br", "fr/Fixture.resources.abcdefghij.wasm" })
                    if (File.Exists(Path.Combine(framework, stale))) throw new InvalidOperationException("Reused child leaked a stale SDK payload: " + stale);
                if ((scenario == "mono11" || mode == "compatible") && File.Exists(Path.Combine(framework, "dotnet.native.worker.hash.mjs")))
                    throw new InvalidOperationException("Unselected pthread bootstrap leaked into the framework inventory.");
            }
            foreach (var mutation in new[] { "loader", "generation", "profile", "runtime" })
            {
                var file = mutation switch {
                    "loader" => Path.Combine(output, "runtimes", "compatible", "_framework", "dotnet.compatible.js"),
                    "runtime" => Path.Combine(output, "runtimes", "compatible", "_framework", "dotnet.native.hash.js"),
                    _ => Path.Combine(output, "runtimes", "compatible", "doroti.runtime-profile.json")
                };
                var original = File.ReadAllText(file);
                if (mutation == "generation") File.WriteAllText(file, original.Replace("test-generation", "stale"));
                else File.Delete(file);
                var validationEngine = new TestBuildEngine();
                if (new DorotiValidateWebPublish { Root = output, BuildId = "test-generation", BuildEngine = validationEngine }.Execute() ||
                    validationEngine.Errors.Count != 1 || !validationEngine.Errors[0].Contains("DOROTIWEB046"))
                    throw new InvalidOperationException("Final validator accepted damaged deployment: " + mutation);
                File.WriteAllText(file, original);
                Console.WriteLine("PASS final-" + mutation); passed++;
            }
        }
        else if (engine.Errors.Count != 1 || !engine.Errors[0].Contains("DOROTIWEB045") || File.ReadAllText(manifestPath) != "previous deployment")
            throw new InvalidOperationException("Preflight failure changed the previous deployment or lost its diagnostic.");
        Console.WriteLine("PASS " + scenario);
        passed++;
    }
    finally { Directory.Delete(root, true); }
}
Console.WriteLine($"{passed} publish contract tests passed.");

internal sealed class TestBuildEngine : IBuildEngine
{
    public List<string> Errors { get; } = [];
    public bool ContinueOnError => false;
    public int LineNumberOfTaskNode => 0;
    public int ColumnNumberOfTaskNode => 0;
    public string ProjectFileOfTaskNode => "tests";
    public void LogErrorEvent(BuildErrorEventArgs e) => Errors.Add(e.Message ?? "Missing error message.");
    public void LogWarningEvent(BuildWarningEventArgs e) => Console.WriteLine(e.Message);
    public void LogMessageEvent(BuildMessageEventArgs e) { }
    public void LogCustomEvent(CustomBuildEventArgs e) { }
    public bool BuildProjectFile(string projectFileName, string[] targetNames, System.Collections.IDictionary globalProperties, System.Collections.IDictionary targetOutputs) =>
        throw new NotSupportedException();
}
