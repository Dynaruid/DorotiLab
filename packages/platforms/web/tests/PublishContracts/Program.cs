using Microsoft.Build.Framework;
using Newtonsoft.Json.Linq;

var passed = 0;
foreach (var scenario in new[] { "merge", "conflict", "generation", "missing-loader", "scoped-imports" })
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
            foreach (var file in new[] { "dotnet." + mode + ".js", "dotnet.obsolete.js", "dotnet.runtime.hash.js", "dotnet.native.hash.wasm" })
                File.WriteAllText(Path.Combine(framework, file), mode);
            if (mode == "threaded") File.WriteAllText(Path.Combine(framework, "dotnet.native.worker.hash.mjs"), "worker");
            File.WriteAllText(Path.Combine(child, "doroti_bootstrap.js"), "shared bootstrap");
            File.WriteAllText(Path.Combine(child, "index.html"), "<html><head><link rel=\"preload\" href=\"_framework/dotnet." + mode + ".js\"><script type=\"importmap\">{\"imports\":{\"./_framework/dotnet.js\":\"./_framework/dotnet." + mode + ".js\",\"app\":\"./app.js\"},\"scopes\":{},\"integrity\":{\"./_framework/dotnet.js\":\"hash\"}}</script></head></html>");
            File.WriteAllText(Path.Combine(child, "doroti.runtime-profile.json"), new JObject {
                ["schemaVersion"] = 1, ["threads"] = mode == "threaded", ["buildId"] = "test-generation",
                ["runtimeContract"] = "mono-10", ["runtimeVersion"] = "10.0.15",
                ["provenance"] = new JObject { ["sdk"] = "10.0.403" }
            }.ToString());
        }
        var output = Path.Combine(root, "deploy", "wwwroot");
        Directory.CreateDirectory(Path.Combine(output, "_framework"));
        File.WriteAllText(Path.Combine(output, "doroti.runtime-profile.json"), "previous deployment");
        switch (scenario)
        {
            case "conflict": File.WriteAllText(Path.Combine(root, "compatible", "doroti_bootstrap.js"), "different"); break;
            case "generation":
                var path = Path.Combine(root, "compatible", "doroti.runtime-profile.json");
                var profile = JObject.Parse(File.ReadAllText(path)); profile["buildId"] = "stale"; File.WriteAllText(path, profile.ToString()); break;
            case "missing-loader": File.Delete(Path.Combine(root, "compatible", "_framework", "dotnet.compatible.js")); break;
            case "scoped-imports": File.WriteAllText(Path.Combine(root, "compatible", "index.html"), "<script type=\"importmap\">{\"scopes\":{\"/\":{\"x\":\"./x.js\"}}}</script>"); break;
        }
        var engine = new TestBuildEngine();
        var merge = new DorotiMergeWebPublish { ThreadedRoot = Path.Combine(root, "threaded"), CompatibleRoot = Path.Combine(root, "compatible"),
            OutputRoot = output, BuildId = "test-generation", BuildEngine = engine };
        var success = merge.Execute();
        if (success != (scenario == "merge")) throw new InvalidOperationException("Unexpected merge result: " + scenario);
        var manifestPath = Path.Combine(output, "doroti.runtime-profile.json");
        if (success)
        {
            var manifest = JObject.Parse(File.ReadAllText(manifestPath));
            if ((int?)manifest["schemaVersion"] != 2 || (string?)manifest["variants"]?["compatible"]?["loaderUrl"] != "runtimes/compatible/_framework/dotnet.compatible.js" ||
                File.ReadAllText(Path.Combine(output, "index.html")).Contains("_framework") || Directory.Exists(Path.Combine(output, "_framework")))
                throw new InvalidOperationException("Invalid merged deployment shape.");
            foreach (var mode in modes)
                if (!File.Exists(Path.Combine(output, "runtimes", mode, "_framework", "dotnet.native.hash.wasm")))
                    throw new InvalidOperationException("Missing runtime variant.");
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
