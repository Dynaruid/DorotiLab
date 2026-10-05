using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Doroti.DeployHelper;

internal static partial class Program
{
    private static readonly SampleApp App = SampleApp.All[0];
    private static readonly IosTarget Device = new("device", "Test iPhone", "DEVICE-UDID", "COREDEVICE-ID", "connected", "27.0");
    private static readonly IosTarget Simulator = new("simulator", "iPhone", "SIM-UDID", "SIM-UDID", "Shutdown", "27.0");
    private static readonly byte[] Certificate = [1, 2, 3, 4];
    private static readonly Identity Identity = new(Convert.ToHexString(SHA1.HashData(Certificate)), "Apple Development: Test Person (CERTTEAM)");

    public static async Task<int> Main(string[] args)
    {
        // These modes exercise ProcessRunner using a real .NET process without Apple tools.
        if (args.FirstOrDefault() == "--sleep-probe") { await Task.Delay(TimeSpan.FromMinutes(1)); return 0; }
        if (args.FirstOrDefault() == "--exit-probe") return 7;
        if (args.FirstOrDefault() == "--echo-probe") { Console.Write(JsonSerializer.Serialize(args.Skip(1))); return 0; }
        if (args.FirstOrDefault() == "--environment-probe") { Console.Write(Environment.GetEnvironmentVariable("DOROTI_DEPLOY_TEST")); return 0; }
        (string Name, Func<Task> Run)[] tests =
        [
            ("Old and new Xcode physical device schemas", DeviceDiscovery),
            ("Only available iOS simulators, including Shutdown", SimulatorDiscovery),
            ("XML plist dates, DER data, arrays and DTD", PlistParsing),
            ("Compatible profiles and wildcard bundle IDs", CompatibleProfiles),
            ("Reject incompatible provisioning profiles", IncompatibleProfiles),
            ("Valid development identities with private keys", SigningIdentities),
            ("Read both profile directories and deduplicate", ProfileDiscovery),
            ("Interactive selection and noninteractive ambiguity", Selection),
            ("CLI arguments and repeatable launch environment", Arguments),
            (".NET 10 NativeAOT verified build settings", Net10Plan),
            (".NET 11 SDK directory for both samples", Net11Plan),
            ("ARM64 and Intel simulator plans without signing", SimulatorPlans),
            ("Invalid mode and SDK prevent builds", InvalidPlans),
            ("Device readiness and developer mode", DeviceReadiness),
            ("DryRun performs no build, boot, install or launch", DryRun),
            ("Failed build never installs existing output", FailedBuild),
            ("Device install failure prevents launch", InstallFailure),
            ("Device launch environment and NoLaunch", DeviceLaunch),
            ("Selected simulator boot, install and environment", SimulatorDeployment),
            ("Reject wrong or ambiguous app bundles", BundleValidation),
            ("Process arguments, environment, exit, timeout and cancellation", ProcessExecution),
            ("Android adb inventory and TCP emulator discovery", AndroidDiscovery),
            ("Android runtime, SDK, ABI and sample build plans", AndroidPlans),
            ("Reject incompatible platform options before commands", AndroidInvalidOptions),
            ("Android authorization, boot and ABI admission", AndroidReadiness),
            ("Android List and DryRun do not build or deploy", AndroidReadOnly),
            ("Android failed builds and missing output never install", AndroidFailedBuild),
            ("Android signed APK selection rejects ambiguity", AndroidApks),
            ("Android launcher component parsing and failed resolution", AndroidLauncherResolution),
            ("Android install, launch extras, PID and NoLaunch", AndroidDeployment),
            ("Android install and launch failures stop deployment", AndroidDeployFailures),
            ("Android PID wait supports cancellation", AndroidPidCancellation),
        ];
        var failures = 0;
        foreach (var (name, test) in tests)
        {
            try { await test(); Console.WriteLine("PASS " + name); }
            catch (Exception error) { failures++; Console.Error.WriteLine($"FAIL {name}: {error}"); }
        }
        Console.WriteLine($"{tests.Length - failures}/{tests.Length} passed.");
        return failures == 0 ? 0 : 1;
    }

    private static ProvisionProfile Profile(Action<Dictionary<string, object?>>? change = null)
    {
        var data = new Dictionary<string, object?>
        {
            ["UUID"] = "PROFILE-UUID", ["Name"] = "Test development profile", ["ExpirationDate"] = DateTimeOffset.UtcNow.AddDays(1),
            ["Platform"] = new object?[] { "iOS" }, ["ApplicationIdentifierPrefix"] = new object?[] { "APPTEAM" },
            ["ProvisionedDevices"] = new object?[] { Device.Udid }, ["DeveloperCertificates"] = new object?[] { Certificate },
            ["Entitlements"] = Entitlements("APPTEAM.dev.doroti.sample2"),
        };
        change?.Invoke(data);
        return new("test.mobileprovision", data);
    }

    private static Dictionary<string, object?> Entitlements(string identifier, bool development = true) => new()
    { ["application-identifier"] = identifier, ["get-task-allow"] = development };

    private static JsonNode OldDevice() => JsonNode.Parse("""
        {"identifier":"COREDEVICE-ID", "hardwareProperties":{"platform":"iOS","reality":"physical","udid":"DEVICE-UDID"},
         "deviceProperties":{"name":"Test iPhone","osVersionNumber":"27.0","developerModeStatus":"enabled"},
         "connectionProperties":{"tunnelState":"connected"}}
        """)!;

    private static JsonNode NewDevice() => JsonNode.Parse("""
        {"identifier":"COREDEVICE-ID", "properties":{
          "hardware":{"platform":"iOS","reality":"physical","udid":"DEVICE-UDID"},
          "state":{"name":"Test iPhone","developerModeStatus":{"enabled":{"mode":1}}},
          "software":{"osVersionNumber":{"stringValue":"27.0"}}, "connection":{"state":"connected"}}}
        """)!;

    private static JsonNode Simulators(string state = "Shutdown") => JsonNode.Parse("""
        {"devices":{"com.apple.CoreSimulator.SimRuntime.iOS-27-0":[
          {"name":"iPhone","udid":"SIM-UDID","state":"STATE","isAvailable":true},
          {"name":"Unavailable","udid":"MISSING","state":"Shutdown","isAvailable":false}],
        "com.apple.CoreSimulator.SimRuntime.tvOS-27-0":[
          {"name":"Apple TV","udid":"TV","state":"Booted","isAvailable":true}]}}
        """.Replace("STATE", state, StringComparison.Ordinal))!;

    private static Task DeviceDiscovery()
    {
        foreach (var node in new[] { OldDevice(), NewDevice() })
            Equal(Device, IosDeploymentCli.PhysicalTargets(new JsonArray(node)).Single());
        var simulated = NewDevice(); simulated["properties"]!["hardware"]!["reality"] = "simulated";
        var tv = NewDevice(); tv["properties"]!["hardware"]!["platform"] = "tvOS";
        Equal(0, IosDeploymentCli.PhysicalTargets(new JsonArray(simulated, tv)).Count);
        return Task.CompletedTask;
    }

    private static Task SimulatorDiscovery()
    { Equal(Simulator, IosDeploymentCli.SimulatorTargets(Simulators()).Single()); return Task.CompletedTask; }

    private static Task PlistParsing()
    {
        var data = Plist.Parse(ProfileXml());
        Equal("PROFILE-UUID", Plist.String(data, "UUID"));
        True(data["ExpirationDate"] is DateTimeOffset);
        True(Plist.Array(data, "DeveloperCertificates").OfType<byte[]>().Single().SequenceEqual(Certificate));
        True(new ProvisionProfile("p", data).Matches(App, Device, Identity));
        return Task.CompletedTask;
    }

    private static Task CompatibleProfiles()
    {
        True(Profile().Matches(App, Device, Identity)); // Certificate common-name team differs from the profile's App ID prefix.
        foreach (var pattern in new[] { "APPTEAM.*", "APPTEAM.dev.doroti.*" })
            True(Profile(data => data["Entitlements"] = Entitlements(pattern)).Matches(App, Device, Identity));
        return Task.CompletedTask;
    }

    private static Task IncompatibleProfiles()
    {
        Action<Dictionary<string, object?>>[] changes =
        [
            data => data["ExpirationDate"] = DateTimeOffset.UtcNow.AddSeconds(-1),
            data => data["ProvisionedDevices"] = new object?[] { "OTHER" },
            data => data["DeveloperCertificates"] = new object?[] { new byte[] { 9 } },
            data => data["Platform"] = new object?[] { "tvOS" },
            data => data["Entitlements"] = Entitlements("APPTEAM.dev.doroti.sample2", false),
            data => data["Entitlements"] = Entitlements("APPTEAM.dev.doroti.testbed"),
            data => data["ApplicationIdentifierPrefix"] = new object?[] { "OTHERTEAM" },
            data => data.Remove("ExpirationDate"),
        ];
        foreach (var change in changes) True(!Profile(change).Matches(App, Device, Identity));
        return Task.CompletedTask;
    }

    private static Task SigningIdentities()
    {
        var output = $"  1) {Identity.Sha1} \"{Identity.Name}\"\n" +
                     $"  2) {new string('A', 40)} \"Apple Distribution: Test (TEAM)\"\n" +
                     $"  3) {new string('B', 40)} \"Apple Development: Expired (TEAM)\" (CSSMERR_TP_CERT_EXPIRED)\n";
        Equal(Identity, IosDeploymentCli.ParseIdentities(output).Single());
        return Task.CompletedTask;
    }

    private static async Task ProfileDiscovery()
    {
        using var temp = new TempDirectory();
        var directories = new[] { Path.Combine(temp.Path, "old"), Path.Combine(temp.Path, "new") };
        foreach (var directory in directories) { Directory.CreateDirectory(directory); File.WriteAllText(Path.Combine(directory, "test.mobileprovision"), ""); }
        var runner = new FakeRunner(_ => ProfileXml());
        Equal(1, (await Cli(runner).DiscoverProfilesAsync(default, directories)).Count);
        Equal(2, runner.Commands.Count);
    }

    private static async Task Selection()
    {
        var ui = new FakeUi { IsInteractive = true }; ui.Responses.Enqueue("invalid"); ui.Responses.Enqueue("2");
        var cli = Cli(new FakeRunner(), ui);
        Equal("second", await cli.SelectAsync("pick", new[] { "first", "second" }, item => item, "--item", default));
        var identities = new[] { Identity, new Identity(new string('B', 40), Identity.Name) };
        bool Match(Identity item, string value) => item.Sha1.Equals(value, StringComparison.OrdinalIgnoreCase) || item.Name == value;
        Equal(Identity, await cli.SelectAsync("identity", identities, item => item.Label, "--key", default, Identity.Sha1.ToLowerInvariant(), Match));
        await Throws<DeployException>(() => cli.SelectAsync("identity", identities, item => item.Label, "--key", default, Identity.Name, Match));
        ui.IsInteractive = false;
        await Throws<DeployException>(() => cli.SelectAsync("identity", identities, item => item.Label, "--key", default));
        ui.IsInteractive = true; ui.Responses.Enqueue("q");
        await Throws<OperationCanceledException>(() => cli.SelectAsync("identity", identities, item => item.Label, "--key", default));
    }

    private static async Task Arguments()
    {
        var options = Options.Parse(["--app", "sample2", "--target", "SIMULATOR", "--env", "A=one two", "--env", "B=a=b", "--env", "A=updated", "--dry-run"]);
        Equal("Sample2", options.App); Equal("simulator", options.Target); Equal("updated", options.Environment["A"]); Equal("a=b", options.Environment["B"]);
        True(options.DryRun);
        foreach (var invalid in new[] { new[] { "--unknown" }, new[] { "--app" }, new[] { "--mode", "CoreClr" }, new[] { "--env", "INVALID-NAME=x" } })
            await Throws<DeployException>(() => Task.FromResult(Options.Parse(invalid)));
    }

    private static async Task Net10Plan()
    {
        var cli = Cli(new FakeRunner(_ => "10.0.401"));
        var plan = await cli.BuildPlanAsync(new(), App, Device, Identity, Profile(), "/custom/dotnet", default);
        Equal(cli.RepositoryRoot, plan.Command.WorkingDirectory); Equal("publish", plan.Command.Arguments[0]);
        foreach (var option in new[] { "-p:DorotiIosTargetFramework=net10.0-ios27.0", "-p:Registrar=managed-static", "-p:_UseDynamicDependenciesForMarkNSObjects=false",
                     "-p:CodesignKey=" + Identity.Sha1, "-p:CodesignProvision=PROFILE-UUID" }) True(plan.Command.Arguments.Contains(option));
        True(!plan.Command.Arguments.Contains("-p:ValidateXcodeVersion=false"));
        Equal("sample2-ios-nativeaot-net10-release-ios-arm64", Path.GetFileName(plan.Artifacts));
        Equal("/custom/dotnet", plan.Command.Environment!["DOTNET_HOST_PATH"]); Equal(1200, plan.Command.TimeoutSeconds);
    }

    private static async Task Net11Plan()
    {
        foreach (var app in SampleApp.All)
        {
            var cli = Cli(new FakeRunner(_ => "11.0.100-rc.1.26425.128"));
            var plan = await cli.BuildPlanAsync(new() { DotnetVersion = 11, SkipXcodeValidation = true }, app, Device, Identity, Profile(), "dotnet", default);
            Equal(Path.Combine(cli.RepositoryRoot, "samples/DorotiTestbedApp/ios"), plan.Command.WorkingDirectory);
            True(plan.Command.Arguments.Contains(app.Project(cli.RepositoryRoot)));
            True(plan.Command.Arguments.Contains("-p:DorotiIosTargetFramework=net11.0-ios"));
            True(plan.Command.Arguments.Contains("-p:ValidateXcodeVersion=false"));
            True(plan.Command.Arguments.Contains("-p:Registrar=managed-static"));
            True(plan.Command.Arguments.Contains("-p:_UseDynamicDependenciesForMarkNSObjects=false"));
            True(plan.Command.Arguments.Contains("-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true"));
            True(plan.Command.Arguments.Contains("-p:PrepareAssemblies=false"));
            True(plan.Command.Arguments.Contains("-p:PostProcessAssemblies=false"));
            True(plan.Artifacts.Contains("net11", StringComparison.Ordinal));
        }
    }

    private static async Task SimulatorPlans()
    {
        foreach (var (cpu, rid) in new[] { ("1", "iossimulator-arm64"), ("0", "iossimulator-x64") })
        {
            var runner = new FakeRunner(command => command.FileName == "sysctl" ? cpu : "10.0.401");
            var plan = await Cli(runner).BuildPlanAsync(new(), App, Simulator, null, null, "dotnet", default);
            Equal(rid, plan.Rid); Equal("build", plan.Command.Arguments[0]); Equal("Debug", plan.Configuration);
            True(plan.Command.Arguments.Contains("-p:PublishAot=false")); True(plan.Command.Arguments.Contains("-p:EnableCodeSigning=false"));
            True(!plan.Command.Arguments.Any(option => option.StartsWith("-p:CodesignKey=", StringComparison.Ordinal)));
        }
    }

    private static async Task InvalidPlans()
    {
        foreach (var (options, target) in new[] { (new Options { Configuration = "Debug" }, Device), (new Options { Mode = "NativeAot" }, Simulator), (new Options { CodesignKey = "anything" }, Simulator) })
        {
            var runner = new FakeRunner();
            await Throws<DeployException>(() => Cli(runner).BuildPlanAsync(options, App, target, Identity, Profile(), "dotnet", default));
            Equal(0, runner.Commands.Count);
        }
        await Throws<DeployException>(() => Cli(new FakeRunner(_ => "11.0.100")).BuildPlanAsync(new(), App, Device, Identity, Profile(), "dotnet", default));
    }

    private static async Task DeviceReadiness()
    {
        foreach (var device in new[] { OldDevice(), NewDevice() })
            await Cli(new FakeRunner(command => CoreResult(command, device))).EnsureDeviceReadyAsync(Device, default);
        var disconnected = NewDevice(); disconnected["properties"]!["connection"]!["state"] = "disconnected";
        await Throws<DeployException>(() => Cli(new FakeRunner(command => CoreResult(command, disconnected))).EnsureDeviceReadyAsync(Device, default));
        var disabled = OldDevice(); disabled["deviceProperties"]!["developerModeStatus"] = "disabled";
        await Throws<DeployException>(() => Cli(new FakeRunner(command => CoreResult(command, disabled))).EnsureDeviceReadyAsync(Device, default));
    }

    private static async Task DryRun()
    {
        var runner = new FakeRunner(command => command.FileName == "sysctl" ? "1" : command.Arguments.Contains("--version") ? "10.0.401" : Simulators().ToJsonString());
        await Cli(runner).RunAsync(new() { App = "Sample2", Target = "simulator", Device = Simulator.Udid, DryRun = true }, default);
        Equal(3, runner.Commands.Count);
        True(runner.Commands.All(command => command.Capture));
        True(runner.Commands.All(command => !command.Arguments.Any(arg => arg is "build" or "publish" or "boot" or "install" or "launch")));
    }

    private static async Task FailedBuild()
    {
        var runner = new FakeRunner(command => command.FileName == "sysctl" ? "1" : command.Arguments.Contains("--version") ? "10.0.401" :
            command.Arguments[0] == "build" ? throw new DeployException("Build failed") : Simulators().ToJsonString());
        await Throws<DeployException>(() => Cli(runner).RunAsync(new() { App = "Sample2", Target = "simulator", Device = Simulator.Udid }, default));
        True(runner.Commands.Any(command => command.Arguments[0] == "build"));
        True(runner.Commands.All(command => command.FileName != "plutil" && !command.Arguments.Contains("install") && !command.Arguments.Contains("launch")));
    }

    private static async Task InstallFailure()
    {
        var runner = new FakeRunner(command => command.Arguments.Contains("install") ? throw new DeployException("Install failed") : "");
        await Throws<DeployException>(() => Cli(runner).DeployAsync(Device, "Test.app", App, new Dictionary<string, string>(), false, default));
        Equal(2, runner.Commands.Count); Equal("codesign", runner.Commands[0].FileName);
        True(!runner.Commands.Any(command => command.Arguments.Contains("launch")));
    }

    private static async Task DeviceLaunch()
    {
        var runner = new FakeRunner();
        await Cli(runner).DeployAsync(Device, "Test.app", App, new Dictionary<string, string> { ["DOROTI_SAMPLE"] = "input" }, false, default);
        var launch = runner.Commands.Single(command => command.Arguments.Contains("launch"));
        var index = launch.Arguments.ToList().IndexOf("--environment-variables");
        Equal("input", JsonSerializer.Deserialize<Dictionary<string, string>>(launch.Arguments[index + 1])!["DOROTI_SAMPLE"]);
        True(launch.Arguments.Contains(Device.Identifier)); True(launch.Arguments.Contains("--terminate-existing"));
        runner.Commands.Clear();
        await Cli(runner).DeployAsync(Device, "Test.app", App, new Dictionary<string, string>(), true, default);
        Equal(2, runner.Commands.Count);
    }

    private static async Task SimulatorDeployment()
    {
        var runner = new FakeRunner(command => command.Arguments.Contains("list") ? Simulators().ToJsonString() : "");
        await Cli(runner).DeployAsync(Simulator, "Test.app", App, new Dictionary<string, string> { ["DOROTI_SAMPLE"] = "input" }, false, default);
        foreach (var action in new[] { "boot", "bootstatus", "install", "launch" })
            True(runner.Commands.Single(command => command.Arguments.Contains(action)).Arguments.Contains(Simulator.Udid));
        Equal("input", runner.Commands.Last().Environment!["SIMCTL_CHILD_DOROTI_SAMPLE"]);
        Equal(6, runner.Commands.Count);
        var unavailable = new FakeRunner(_ => "{\"devices\":{}}");
        await Throws<DeployException>(() => Cli(unavailable).DeployAsync(Simulator, "Test.app", App, new Dictionary<string, string>(), false, default));
        Equal(1, unavailable.Commands.Count);
    }

    private static async Task BundleValidation()
    {
        using var temp = new TempDirectory();
        var name = App.Folder + ".iOS";
        var bundle = Path.Combine(temp.Path, "bin", name, "release_ios-arm64", name + ".app"); Directory.CreateDirectory(bundle);
        var runner = new FakeRunner(_ => $"<plist><dict><key>CFBundleIdentifier</key><string>{App.BundleId}</string></dict></plist>");
        Equal(bundle, await Cli(runner).FindAppBundleAsync(temp.Path, App, default));
        Equal("plutil", runner.Commands.Single().FileName);
        True(runner.Commands.Single().Arguments.Contains("-"));
        runner.Handler = _ => "<plist><dict><key>CFBundleIdentifier</key><string>wrong.app</string></dict></plist>";
        await Throws<DeployException>(() => Cli(runner).FindAppBundleAsync(temp.Path, App, default));
        Directory.CreateDirectory(Path.Combine(temp.Path, "bin", name, "duplicate", name + ".app"));
        await Throws<DeployException>(() => Cli(runner).FindAppBundleAsync(temp.Path, App, default));
    }

    private static async Task ProcessExecution()
    {
        var runner = new ProcessRunner(new FakeUi());
        var host = Executables.Resolve(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet");
        var assembly = typeof(Program).Assembly.Location;
        string[] values = ["Apple Development: Person With Spaces (TEAM)", "Profile With Spaces", "A=a=b", "literal $(nothing) 'quoted'"];
        var output = await runner.RunAsync(new(host, [assembly, "--echo-probe", .. values]));
        True(JsonSerializer.Deserialize<string[]>(output)!.SequenceEqual(values));
        Equal("one two", await runner.RunAsync(new(host, [assembly, "--environment-probe"], Environment: new Dictionary<string, string> { ["DOROTI_DEPLOY_TEST"] = "one two" })));
        await Throws<DeployException>(() => runner.RunAsync(new(host, [assembly, "--exit-probe"])));
        await Throws<DeployException>(() => runner.RunAsync(new(host, [assembly, "--sleep-probe"], TimeoutSeconds: 1)));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
        await Throws<OperationCanceledException>(() => runner.RunAsync(new(host, [assembly, "--sleep-probe"]), cancellation.Token));
    }

    private static IosDeploymentCli Cli(FakeRunner runner, FakeUi? ui = null) => new(Path.GetTempPath(), runner, ui ?? new FakeUi());

    private static string CoreResult(Command command, JsonNode result)
    {
        var path = command.Arguments[command.Arguments.ToList().IndexOf("--json-output") + 1];
        File.WriteAllText(path, new JsonObject { ["info"] = new JsonObject { ["outcome"] = "success" }, ["result"] = result.DeepClone() }.ToJsonString());
        return "";
    }

    private static string ProfileXml()
    {
        XElement Value(object? value) => value switch
        {
            Dictionary<string, object?> dict => new XElement("dict", dict.SelectMany(pair => new[] { new XElement("key", pair.Key), Value(pair.Value) })),
            object?[] array => new XElement("array", array.Select(Value)),
            byte[] bytes => new XElement("data", Convert.ToBase64String(bytes)),
            bool flag => new XElement(flag ? "true" : "false"),
            DateTimeOffset date => new XElement("date", date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
            _ => new XElement("string", value),
        };
        return "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" + new XElement("plist", Value(Profile().Data));
    }

    private static void True(bool condition) { if (!condition) throw new Exception("Assertion failed."); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected {expected}; actual {actual}."); }
    private static async Task Throws<T>(Func<Task> action) where T : Exception
    { try { await action(); } catch (T) { return; } throw new Exception($"Expected {typeof(T).Name}."); }

    private sealed class FakeRunner(Func<Command, string>? handler = null) : ICommandRunner
    {
        public List<Command> Commands { get; } = [];
        public Func<Command, string> Handler { get; set; } = handler ?? (_ => "");
        public Task<string> RunAsync(Command command, CancellationToken cancellationToken = default)
        { cancellationToken.ThrowIfCancellationRequested(); Commands.Add(command); return Task.FromResult(Handler(command)); }
    }

    private sealed class FakeUi : IUserInterface
    {
        public bool IsInteractive { get; set; }
        public Queue<string?> Responses { get; } = new();
        public List<string> Output { get; } = [];
        public void WriteLine(string message) => Output.Add(message);
        public Task<string?> ReadLineAsync(CancellationToken cancellationToken) => Task.FromResult(Responses.Dequeue());
    }

    private sealed class TempDirectory : IDisposable
    {
        public string Path { get; } = Directory.CreateTempSubdirectory("doroti-deploy-tests-").FullName;
        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
