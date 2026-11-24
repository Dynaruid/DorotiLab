using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Doroti.DeployHelper;

using var deadline = new CancellationTokenSource(TimeSpan.FromMinutes(20));
var ct = deadline.Token;
var currentDirectory = Directory.GetCurrentDirectory();
var repository = Directory.Exists(Path.Combine(currentDirectory, "samples"))
    ? currentDirectory
    : Path.GetFullPath("../..", currentDirectory);
var root = Path.Combine(repository, "Doroti", "artifacts", "deploy-helper-tests", Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
var sdkRoot = Path.Combine(root, "sdk with spaces");
foreach (var sdk in new[] { "10.0.403", "11.0.100" })
{
    Directory.CreateDirectory(Path.Combine(sdkRoot, sdk));
    File.WriteAllText(Path.Combine(sdkRoot, sdk, "dotnet.dll"), "SDK fixture");
}
var sdkOutput = $"10.0.403 [{sdkRoot}]\n11.0.100 [{sdkRoot}]\n11.0.200-preview.1 [{sdkRoot}]\n12.0.100 [{sdkRoot}]";
var dotnet = Environment.ProcessPath!;
var checks = new List<(string Name, Func<Task> Run)>();
void Test(string name, Func<Task> run) => checks.Add((name, run));
void Require(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
Options WebOptions(int version = 10, string? mode = null) => new()
{
    Platform = "web", App = "Demo", DotnetVersion = version, Mode = mode, DotnetPath = dotnet, NoLaunch = true,
};
var demo = SampleApp.All.Single(app => app.Key == "Demo");

Test("iOS defaults to stable .NET 11 CoreCLR on Simulator and retains explicit .NET 10", async () =>
{
    var cli = new IosDeploymentCli(repository, new IosBuildRunner(repository), new TestUi());
    var simulator = new IosTarget("simulator", "iPhone", "test-udid", "test-udid", "Shutdown", "27.2");
    var plan = await cli.BuildPlanAsync(new Options(), demo, simulator, null, null, dotnet, ct);
    Require(plan.Sdk == "11.0.100" && plan.Mode == "CoreClr" && plan.Configuration == "Debug" &&
        plan.Command.WorkingDirectory == Path.GetDirectoryName(demo.Project(repository)) &&
        plan.Command.Arguments.Contains("-p:DorotiIosTargetFramework=net11.0-ios") &&
        plan.Command.Arguments.Contains("-p:PublishAot=false"), "iOS Simulator did not select its stable .NET 11 profile.");

    var legacy = await cli.BuildPlanAsync(Options.Parse(["--dotnet-version", "10"]), demo, simulator, null, null, dotnet, ct);
    Require(legacy.Sdk == "10.0.403" && legacy.Mode == "Mono" && legacy.Command.WorkingDirectory == Path.Combine(repository, "Doroti") &&
        legacy.Command.Arguments.Contains("-p:DorotiIosTargetFramework=net10.0-ios"), "Explicit .NET 10 iOS selection was lost.");

    var device = simulator with { Kind = "device" };
    var identity = new Identity("test-key", "Development");
    var profile = new ProvisionProfile("test-profile", new() { ["UUID"] = "test-uuid" });
    var release = await cli.BuildPlanAsync(new Options(), demo, device, identity, profile, dotnet, ct);
    Require(release.Sdk == "11.0.100" && release.Mode == "NativeAot" && release.Configuration == "Release" &&
        release.Command.Arguments[0] == "publish" && release.Command.Arguments.Contains("-p:CodesignProvision=test-uuid"),
        "Default iOS device build did not select .NET 11 NativeAOT and signing.");
});

Test("Web CLI and mobile options stay separate", () =>
{
    var parsed = Options.Parse(["--platform", "web", "--app", "Demo", "--dotnet-version", "11", "--mode", "MonoAot", "--web-execution-mode", "compatible", "--port", "5192"]);
    parsed.ValidateForPlatform("web");
    Require(parsed.Port == 5192 && parsed.WebExecutionMode == "compatible", "Web options were lost.");
    foreach (var args in new[]
    {
        new[] { "--platform", "web", "--mode", "NativeAot" },
        new[] { "--platform", "web", "--mode", "MonoAot", "--configuration", "Debug" },
        new[] { "--platform", "web", "--device", "mobile" },
        new[] { "--platform", "android", "--port", "5192" },
        new[] { "--platform", "ios", "--web-execution-mode", "compatible" },
        new[] { "--platform", "ios", "--mode", "Mono" },
        new[] { "--platform", "ios", "--dotnet-version", "10", "--mode", "CoreClr" },
        new[] { "--platform", "web", "--port", "65536" },
    })
    {
        try
        {
            var invalid = Options.Parse(args);
            invalid.ValidateForPlatform(invalid.Platform!);
            throw new Exception("Invalid platform options were accepted: " + string.Join(' ', args));
        }
        catch (DeployException) { }
    }
    foreach (var platform in new[] { "ios", "android" }) new Options().ValidateForPlatform(platform);
    return Task.CompletedTask;
});

Test("Windows dry run resolves all sample runners and requests Native AOT publish", async () =>
{
    foreach (var app in SampleApp.All)
    {
        var options = Options.Parse(["--platform", "windows", "--app", app.Key, "--dry-run", "--dotnet-path", dotnet]);
        var runner = new FakeRunner("");
        var ui = new TestUi();
        await new WindowsDeploymentCli(repository, runner, ui).RunAsync(options, ct);
        var plan = ui.Lines.Single(line => line.StartsWith("Dry run:", StringComparison.Ordinal));
        Require(plan.Contains(app.WindowsProject(repository), StringComparison.Ordinal) &&
            plan.Contains(" publish ", StringComparison.Ordinal) && plan.Contains("-c Release -r win-x64", StringComparison.Ordinal) &&
            plan.Contains("-p:DorotiCompilationMode=NativeAot", StringComparison.Ordinal) && runner.Publishes == 0,
            $"Unexpected Windows publish plan for {app.Key}.");
    }
});

Test("Windows rejects mobile, Web and incompatible build options", () =>
{
    foreach (var arguments in new[]
    {
        new[] { "--target", "device" }, new[] { "--port", "5192" }, new[] { "--dotnet-version", "11" },
        new[] { "--mode", "CoreClrJit" }, new[] { "--configuration", "Debug" },
    })
    {
        try
        {
            Options.Parse(arguments).ValidateForPlatform("windows");
            throw new Exception("Invalid Windows options were accepted: " + string.Join(' ', arguments));
        }
        catch (DeployException) { }
    }
    return Task.CompletedTask;
});

if (OperatingSystem.IsWindows())
{
    Test("Windows rejects an apphost with a managed runner before launch", async () =>
    {
        var project = demo.WindowsProject(root);
        Directory.CreateDirectory(Path.GetDirectoryName(project)!);
        File.WriteAllText(project, "project fixture");
        var runner = new FakeRunner("")
        {
            OnPublish = command =>
            {
                var publish = command.Arguments.SkipWhile(argument => argument != "-o").Skip(1).First();
                Directory.CreateDirectory(publish);
                File.Copy(dotnet, Path.Combine(publish, "DorotiDemoApp.WindowsAppSdk.exe"));
                File.WriteAllText(Path.Combine(publish, "DorotiDemoApp.WindowsAppSdk.dll"), "managed entry fixture");
                File.WriteAllText(Path.Combine(publish, "doroti_windows_appsdk_host_v1.dll"), "native host fixture");
            },
        };
        try
        {
            await new WindowsDeploymentCli(root, runner, new TestUi()).RunAsync(new() { App = "Demo", DotnetPath = dotnet }, ct);
            throw new Exception("The Windows apphost was accepted as Native AOT.");
        }
        catch (DeployException error)
        {
            Require(runner.Publishes == 1 && error.Message.Contains("not a win-x64 Native AOT image", StringComparison.Ordinal),
                "Unexpected Windows publish validation result.");
        }
    });
}

Test("SDK discovery excludes previews and unsupported majors", () =>
{
    var sdks = WebDeploymentCli.ParseSdks(sdkOutput + $"\n11.0.101 [{sdkRoot}]");
    Require(sdks.Count == 3 && sdks[0].Version == "11.0.101" && sdks[2].Version == "10.0.403", "SDK ordering/filtering failed.");
    Require(sdks[0].EntryPoint.Contains("sdk with spaces", StringComparison.Ordinal), "SDK path was split.");
    return Task.CompletedTask;
});

Test("Interactive Web choices reach the publish command", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    var ui = new ScriptedUi("2", "1", "2", "3");
    var cli = new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer());
    var plan = await cli.BuildPlanAsync(options, demo, dotnet, ct);
    Require(ui.ReadCount == 4, "Expected SDK, configuration, compilation and execution menus.");
    Require(plan.DotnetVersion == 11 && plan.Configuration == "Release" && plan.Mode == "Mono" && plan.ExecutionMode == "auto", "Menu selections were replaced by defaults.");
    Require(plan.Command.Arguments.Contains("-p:DorotiWebTargetFramework=net11.0") &&
        plan.Command.Arguments.Contains("-p:RunAOTCompilation=false") &&
        plan.Command.Arguments.Contains("-p:DorotiWebExecutionMode=auto"), "Selected values did not reach publish.");
});

Test("Explicit .NET 10 and AOT skip their menus and constrain configuration", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo", "--dotnet-version", "10", "--mode", "MonoAot"]);
    var ui = new ScriptedUi("2");
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(options.DotnetVersionSpecified && ui.ReadCount == 1, "Explicit default-valued options were treated as missing.");
    Require(plan.DotnetVersion == 10 && plan.Configuration == "Release" && plan.Mode == "MonoAot" && plan.ExecutionMode == "compatible", "Explicit options or the remaining menu selection changed.");
    Require(!ui.Lines.Any(line => line.Contains("Debug (non-AOT publish)", StringComparison.Ordinal)), "AOT was offered an invalid Debug configuration.");
});

Test("Interactive Debug publish only offers non-AOT", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    var ui = new ScriptedUi("1", "2", "2");
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(ui.ReadCount == 3 && plan.DotnetVersion == 10 && plan.Configuration == "Debug" && plan.Mode == "Mono" && plan.ExecutionMode == "compatible", "Debug selection did not constrain compilation.");
    Require(!ui.Lines.Any(line => line.Contains("Mono WASM AOT", StringComparison.Ordinal)), "Debug offered AOT.");
    Require(plan.Command.Arguments.Contains("publish") && plan.Command.Arguments.Contains("-p:RunAOTCompilation=false"), "Debug opened development mode instead of publishing.");
});

Test("Fully specified Web options never ask build questions", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo", "--dotnet-version", "11", "--configuration", "Release", "--mode", "MonoAot", "--web-execution-mode", "threaded"]);
    var ui = new ScriptedUi();
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(ui.ReadCount == 0 && plan.DotnetVersion == 11 && plan.Mode == "MonoAot" && plan.ExecutionMode == "threaded", "Explicit options opened menus or changed values.");
});

Test("Redirected input preserves omitted Web defaults", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), new TestUi(), new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(!options.DotnetVersionSpecified && plan.DotnetVersion == 10 && plan.Configuration == "Release" && plan.Mode == "MonoAot" && plan.ExecutionMode == "threaded", "Noninteractive defaults changed or requested input.");
});

Test("Interactive SDK selection only uses installed supported versions", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    var ui = new ScriptedUi("1", "1", "1");
    var available = $"11.0.100 [{sdkRoot}]\n12.0.100 [{sdkRoot}]\n10.0.500-preview.1 [{sdkRoot}]";
    var plan = await new WebDeploymentCli(root, new FakeRunner(available), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(plan.DotnetVersion == 11 && ui.ReadCount == 3, "A missing/default, preview or unsupported SDK was selected.");
});

Test("Cancelling a Web build menu never publishes or starts a server", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    options.DotnetPath = dotnet;
    var runner = new FakeRunner(sdkOutput);
    var server = new FakeServer();
    try
    {
        await new WebDeploymentCli(root, runner, new ScriptedUi("q"), server).RunAsync(options, ct);
        throw new Exception("Menu cancellation was ignored.");
    }
    catch (OperationCanceledException) { }
    Require(runner.Publishes == 0 && server.Calls == 0, "Menu cancellation started publish or preview.");
});

Test("Enter accepts the recommended Release AOT defaults", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    var ui = new ScriptedUi("", "", "", "");
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(ui.ReadCount == 4 && plan.DotnetVersion == 10 && plan.Configuration == "Release" && plan.Mode == "MonoAot" && plan.ExecutionMode == "threaded", "Enter did not select the documented Web defaults.");
    Require(plan.Command.Arguments.Contains("-p:RunAOTCompilation=true") && ui.Lines.Contains("  1. AOT (Mono WASM AOT; longer publish) (default)"), "AOT was not marked or applied as the default.");
});

Test("Explicit non-AOT survives accepting other Web defaults", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo", "--dotnet-version", "11", "--mode", "Mono"]);
    var ui = new ScriptedUi("", "");
    var plan = await new WebDeploymentCli(root, new FakeRunner(sdkOutput), ui, new FakeServer()).BuildPlanAsync(options, demo, dotnet, ct);
    Require(ui.ReadCount == 2 && plan.DotnetVersion == 11 && plan.Configuration == "Release" && plan.Mode == "Mono" && plan.ExecutionMode == "threaded", "Defaults overrode explicit non-AOT or SDK selection.");
    Require(plan.Command.Arguments.Contains("-p:RunAOTCompilation=false"), "Non-AOT opt-in was lost.");
});

Test("End of input cancels instead of accepting a Web default", async () =>
{
    var options = Options.Parse(["--platform", "web", "--app", "Demo"]);
    options.DotnetPath = dotnet;
    var runner = new FakeRunner(sdkOutput);
    var server = new FakeServer();
    try
    {
        await new WebDeploymentCli(root, runner, new ScriptedUi((string?)null), server).RunAsync(options, ct);
        throw new Exception("End of input accepted a default and continued.");
    }
    catch (OperationCanceledException) { }
    Require(runner.Publishes == 0 && server.Calls == 0, "End of input started publish or preview.");
});

Test("Selectors without a default still require a numbered choice", async () =>
{
    var ui = new ScriptedUi("", "2");
    var selected = await new Selector(ui).SelectAsync("Existing platform/device menu", new[] { "first", "second" }, value => value, "--device", ct);
    Require(selected == "second" && ui.ReadCount == 2, "Web defaults changed existing platform/device selection behavior.");
});

foreach (var version in new[] { 10, 11 })
foreach (var mode in new[] { "MonoAot", "Mono" })
{
    Test($"net{version} {mode} selects the real target and publish mode", async () =>
    {
        var runner = new FakeRunner(sdkOutput);
        var cli = new WebDeploymentCli(root, runner, new TestUi(), new FakeServer());
        var plan = await cli.BuildPlanAsync(WebOptions(version, mode), demo, dotnet, ct);
        var args = plan.Command.Arguments;
        Require(args[0] == Path.Combine(sdkRoot, version == 10 ? "10.0.403" : "11.0.100", "dotnet.dll"), "SDK pin was used instead of the selected SDK.");
        Require(args.Contains("publish") && !args.Contains("run") && !args.Contains("watch"), "Preview must publish.");
        Require(args.Contains($"-p:DorotiWebTargetFramework=net{version}.0"), "SDK and runtime target were conflated.");
        Require(Guid.TryParseExact(plan.BuildId, "N", out _) && args.Contains("-p:DorotiWebBuildId=" + plan.BuildId), "Inner builds can generate different publish IDs.");
        Require(args.Contains($"-p:RunAOTCompilation={(mode == "MonoAot" ? "true" : "false")}"), "AOT choice was lost.");
        Require(args.Contains("-p:DorotiWebInterpretWidgetTree=false"), "Sample interpretation would hide the AOT comparison.");
        Require(args.Contains("-p:DorotiWebQualificationProbe=true") == (version == 11), "net11 qualification opt-in escaped its selection.");
        Require(plan.Command.TimeoutSeconds == 1200 && plan.Command.WorkingDirectory == root, "Publish is not bounded or independent of the caller directory.");
        Require(plan.WebRoot.Contains($"-net{version}-release-threaded", StringComparison.Ordinal), "Runtime variants share publish output.");
    });
}

Test("Default Release uses AOT and Debug still publishes without AOT", async () =>
{
    var cli = new WebDeploymentCli(root, new FakeRunner(sdkOutput), new TestUi(), new FakeServer());
    var options = WebOptions();
    Require((await cli.BuildPlanAsync(options, demo, dotnet, ct)).Mode == "MonoAot", "Release must default to AOT.");
    options.Configuration = "Debug";
    var debug = await cli.BuildPlanAsync(options, demo, dotnet, ct);
    Require(debug.Mode == "Mono" && debug.Command.Arguments.Contains("publish"), "Debug switched into a development server.");
});

Test("Missing SDK fails before publish or serve", async () =>
{
    var runner = new FakeRunner($"10.0.403 [{sdkRoot}]");
    var server = new FakeServer();
    try
    {
        await new WebDeploymentCli(root, runner, new TestUi(), server).RunAsync(WebOptions(11), ct);
        throw new Exception("Missing SDK accepted.");
    }
    catch (DeployException) { }
    Require(runner.Publishes == 0 && server.Calls == 0, "Missing SDK started work.");
});

Test("Dry run never publishes, serves or opens a browser", async () =>
{
    var runner = new FakeRunner(sdkOutput);
    var server = new FakeServer();
    var options = WebOptions(11);
    options.DryRun = true;
    await new WebDeploymentCli(root, runner, new TestUi(), server).RunAsync(options, ct);
    Require(runner.Publishes == 0 && server.Calls == 0, "Dry run started publish/preview.");
});

Test("Publish failure never serves an older output", async () =>
{
    var runner = new FakeRunner(sdkOutput);
    var server = new FakeServer();
    var cli = new WebDeploymentCli(root, runner, new TestUi(), server);
    var options = WebOptions();
    var plan = await cli.BuildPlanAsync(options, demo, dotnet, ct);
    WritePublish(plan);
    runner.OnPublish = _ => throw new DeployException("Publish fixture failed.");
    try
    {
        await cli.RunAsync(options, ct);
        throw new Exception("Failed publish was accepted.");
    }
    catch (DeployException) { }
    Require(server.Calls == 0, "An old publish was served after failure.");
});

Test("Published runtime metadata must match the requested AOT/runtime", async () =>
{
    var cli = new WebDeploymentCli(root, new FakeRunner(sdkOutput), new TestUi(), new FakeServer());
    var plan = await cli.BuildPlanAsync(WebOptions(11), demo, dotnet, ct);
    WritePublish(plan);
    WebDeploymentCli.ValidatePublish(plan);
    foreach (var changed in new[] { plan with { Mode = "Mono" }, plan with { DotnetVersion = 10 }, plan with { ExecutionMode = "compatible" }, plan with { Sdk = "11.0.999" }, plan with { BuildId = Guid.NewGuid().ToString("N") } })
    {
        try { WebDeploymentCli.ValidatePublish(changed); throw new Exception("Mismatched publish accepted."); }
        catch (DeployException) { }
    }
});

Test("Helper awaits the server and exits when the server completes", async () =>
{
    var runner = new FakeRunner(sdkOutput);
    var server = new FakeServer();
    var cli = new WebDeploymentCli(root, runner, new TestUi(), server);
    var options = WebOptions(11);
    options.WebExecutionMode = "compatible";
    var plan = await cli.BuildPlanAsync(options, demo, dotnet, ct);
    runner.OnPublish = command => WritePublish(plan with
    {
        BuildId = command.Arguments.Single(argument => argument.StartsWith("-p:DorotiWebBuildId=", StringComparison.Ordinal))["-p:DorotiWebBuildId=".Length..],
    });
    var run = cli.RunAsync(options, ct);
    if (await Task.WhenAny(server.Started.Task, run).WaitAsync(ct) == run) await run;
    await server.Started.Task.WaitAsync(ct);
    Require(!run.IsCompleted && !server.Isolated && !server.LaunchBrowser, "Server was detached or preview options changed.");
    Require(server.WebRoot == plan.WebRoot, "Build output was served instead of publish output.");
    server.Stopped.SetResult();
    await run.WaitAsync(ct);
});

foreach (var isolated in new[] { true, false })
{
    Test($"Real HTTP preview: isolation={isolated}, assets, missing files and shutdown", async () =>
    {
        var webRoot = Path.Combine(root, "http-" + isolated);
        Directory.CreateDirectory(webRoot);
        File.WriteAllText(Path.Combine(webRoot, "index.html"), "<html>Published preview</html>");
        File.WriteAllText(Path.Combine(webRoot, "module.mjs"), "export const ready = true;");
        File.WriteAllBytes(Path.Combine(webRoot, "app.wasm"), [0, 97, 115, 109, 1, 0, 0, 0]);
        File.WriteAllText(Path.Combine(root, "outside.txt"), "outside-publish-root");
        var ui = new TestUi();
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var serving = new WebPreviewServer(ui).RunAsync(webRoot, 0, isolated, false, stop.Token);
        try
        {
            if (await Task.WhenAny(ui.Address.Task, serving) == serving) await serving;
            var address = await ui.Address.Task.WaitAsync(ct);
            using var client = new HttpClient { BaseAddress = address, Timeout = TimeSpan.FromMinutes(20) };
            using var page = await client.GetAsync("/", ct);
            Require(page.IsSuccessStatusCode && (await page.Content.ReadAsStringAsync(ct)).Contains("Published preview", StringComparison.Ordinal), "Published index was not served.");
            Require(page.Headers.CacheControl?.NoStore == true, "Preview can reuse stale browser cache.");
            Require(page.Headers.TryGetValues("Cross-Origin-Opener-Policy", out var coop) == isolated, "COOP isolation mode mismatch.");
            if (isolated) Require(coop!.Single() == "same-origin" && page.Headers.GetValues("Cross-Origin-Embedder-Policy").Single() == "require-corp", "Threaded isolation headers are incomplete.");
            using var wasm = await client.GetAsync("/app.wasm", ct);
            Require(wasm.Content.Headers.ContentType?.MediaType == "application/wasm" && (await wasm.Content.ReadAsByteArrayAsync(ct)).Length == 8, "WASM MIME/content changed.");
            using var module = await client.GetAsync("/module.mjs", ct);
            Require(module.Content.Headers.ContentType?.MediaType == "text/javascript", "Module MIME is wrong.");
            using var head = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/app.wasm"), ct);
            Require(head.Content.Headers.ContentLength == 8 && (await head.Content.ReadAsByteArrayAsync(ct)).Length == 0, "HEAD response returned a body or lost its length.");
            foreach (var path in new[] { "/_framework/missing", "/runtimes/missing/file.wasm", "/missing.js", "/%2e%2e/outside.txt" })
            {
                using var missing = await client.GetAsync(path, ct);
                Require(missing.StatusCode == HttpStatusCode.NotFound, "A missing/outside asset was served: " + path);
            }
            using var route = await client.GetAsync("/gallery/components", ct);
            Require(route.IsSuccessStatusCode && (await route.Content.ReadAsStringAsync(ct)).Contains("Published preview", StringComparison.Ordinal), "Client route fallback failed.");
            try
            {
                await new WebPreviewServer(new TestUi()).RunAsync(webRoot, address.Port, isolated, false, ct);
                throw new Exception("Occupied port was silently replaced.");
            }
            catch (IOException) { }
        }
        finally
        {
            stop.Cancel();
            await serving.WaitAsync(ct);
        }
        var released = await ui.Address.Task;
        using var listener = new TcpListener(IPAddress.Loopback, released.Port);
        listener.Start();
        Require(ui.Lines.Any(line => line.Contains("Preview server stopped.", StringComparison.Ordinal)), "Server shutdown did not complete.");
    });
}

var failures = 0;
foreach (var (name, run) in checks)
{
    try { await run().WaitAsync(ct); Console.WriteLine("PASS " + name); }
    catch (Exception error) { failures++; Console.Error.WriteLine($"FAIL {name}\n{error}"); }
}
Console.WriteLine($"{checks.Count - failures}/{checks.Count} deploy-helper checks passed. Fixtures: {root}");
return failures == 0 ? 0 : 1;

void WritePublish(WebPublishPlan plan)
{
    Directory.CreateDirectory(plan.WebRoot);
    File.WriteAllText(Path.Combine(plan.WebRoot, "index.html"), "<html>Published fixture</html>");
    File.WriteAllText(Path.Combine(plan.WebRoot, "doroti.runtime-profile.json"), JsonSerializer.Serialize(new
    {
        schemaVersion = 1, buildId = plan.BuildId, threads = plan.ExecutionMode == "threaded", runtimeContract = $"mono-{plan.DotnetVersion}",
        provenance = new { sdk = plan.Sdk, targetFramework = $"net{plan.DotnetVersion}.0", configuration = plan.Configuration,
            aot = plan.Mode == "MonoAot" ? "true" : "false", interpretWidgetTree = "false" },
    }));
}

sealed class FakeRunner(string sdks) : ICommandRunner
{
    public int Publishes { get; private set; }
    public Action<Command>? OnPublish { get; set; }
    public Task<string> RunAsync(Command command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (command.Arguments.Contains("--list-sdks")) return Task.FromResult(sdks);
        if (!command.Arguments.Contains("publish")) throw new Exception("Unexpected command: " + command.Display);
        Publishes++;
        OnPublish?.Invoke(command);
        return Task.FromResult("");
    }
}

sealed class IosBuildRunner(string repository) : ICommandRunner
{
    public Task<string> RunAsync(Command command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (command.FileName == "sysctl") return Task.FromResult("1");
        if (command.Arguments.SequenceEqual(["--version"]))
            return Task.FromResult(command.WorkingDirectory == Path.Combine(repository, "Doroti") ? "10.0.403" : "11.0.100");
        throw new Exception("Unexpected iOS command: " + command.Display);
    }
}

sealed class FakeServer : IWebPreviewServer
{
    public int Calls { get; private set; }
    public string? WebRoot { get; private set; }
    public bool Isolated { get; private set; }
    public bool LaunchBrowser { get; private set; }
    public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource Stopped { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public async Task RunAsync(string webRoot, int port, bool isolated, bool launchBrowser, CancellationToken ct)
    {
        Calls++;
        WebRoot = webRoot;
        Isolated = isolated;
        LaunchBrowser = launchBrowser;
        Started.SetResult();
        await Stopped.Task.WaitAsync(ct);
    }
}

sealed class TestUi : IUserInterface
{
    public bool IsInteractive => false;
    public List<string> Lines { get; } = [];
    public TaskCompletionSource<Uri> Address { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public void WriteLine(string message)
    {
        Lines.Add(message);
        const string prefix = "Preview URL: ";
        var index = message.IndexOf(prefix, StringComparison.Ordinal);
        if (index >= 0) Address.TrySetResult(new Uri(message[(index + prefix.Length)..].Split('\n')[0]));
    }
    public Task<string?> ReadLineAsync(CancellationToken cancellationToken) => throw new Exception("Unexpected interactive prompt.");
}

sealed class ScriptedUi(params string?[] inputs) : IUserInterface
{
    private readonly Queue<string?> answers = new(inputs);
    public bool IsInteractive => true;
    public int ReadCount { get; private set; }
    public List<string> Lines { get; } = [];
    public void WriteLine(string message) => Lines.Add(message);
    public Task<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ReadCount++;
        if (answers.Count == 0) throw new Exception("Unexpected additional menu prompt.");
        return Task.FromResult<string?>(answers.Dequeue());
    }
}
