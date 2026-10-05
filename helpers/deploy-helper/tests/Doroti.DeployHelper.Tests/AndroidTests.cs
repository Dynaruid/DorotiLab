using Doroti.DeployHelper;

internal static partial class Program
{
    private static readonly AndroidTarget AndroidDevice = new("USB-SERIAL", "device", "Test Phone", "device");
    private static AndroidDeploymentCli AndroidCli(FakeRunner runner, FakeUi? ui = null, string? root = null) =>
        new(root ?? Path.GetTempPath(), runner, ui ?? new FakeUi());

    private const string AdbInventory = """
        * daemon started successfully
        List of devices attached
        USB-SERIAL device product:test model:Test_Phone device:test transport_id:1
        emulator-5554 device product:sdk model:Emulator device:emu
        127.0.0.1:5555 device model:TCP_Emulator
        LOCKED unauthorized
        GONE offline
        PERMISSION no permissions (user is not in the plugdev group)
        """;

    private static async Task AndroidDiscovery()
    {
        var parsed = AndroidDeploymentCli.ParseTargets(AdbInventory);
        Equal(6, parsed.Count);
        Equal(AndroidDevice, parsed.Single(target => target.Serial == "USB-SERIAL"));
        Equal("emulator", parsed.Single(target => target.Serial == "emulator-5554").Kind);
        Equal("no permissions", parsed.Single(target => target.Serial == "PERMISSION").State);
        var runner = new FakeRunner(command => command.Arguments[0] == "devices" ? AdbInventory : command.Arguments[1] == "127.0.0.1:5555" ? "1" : "0");
        var emulators = await AndroidCli(runner).DiscoverTargetsAsync("adb", "emulator", default);
        Equal(2, emulators.Count);
        True(emulators.All(target => target.Kind == "emulator"));
        True(runner.Commands.Where(command => command.Arguments.Contains("shell")).All(command =>
            command.Arguments[1] is "USB-SERIAL" or "127.0.0.1:5555"));
    }

    private static async Task AndroidPlans()
    {
        foreach (var app in SampleApp.All)
            foreach (var (version, mode) in new[] { (10, "Mono"), (10, "MonoAot"), (10, "CoreClrJit"), (10, "CoreClrR2R"), (11, "CoreClrJit"), (11, "CoreClrR2R") })
                foreach (var rid in new[] { "android-arm64", "android-x64" })
                {
                    var cli = AndroidCli(new FakeRunner(_ => $"{version}.0.401"));
                    var plan = await cli.BuildPlanAsync(new() { DotnetVersion = version, Mode = mode }, app, rid, "/custom/dotnet", default);
                    Equal("Release", plan.Configuration); Equal(mode, plan.Mode); Equal(rid, plan.Rid);
                    Equal(version == 10 ? cli.RepositoryRoot : Path.Combine(cli.RepositoryRoot, "samples/DorotiSampleApp2/android/sdk/net11"), plan.Command.WorkingDirectory);
                    True(plan.Command.Arguments.Contains(app.AndroidProject(cli.RepositoryRoot)));
                    True(plan.Command.Arguments.Contains($"-p:DorotiAndroidTargetFramework=net{version}.0-android"));
                    True(plan.Command.Arguments.Contains($"-p:UseMonoRuntime={(mode is "Mono" or "MonoAot" ? "true" : "false")}"));
                    True(plan.Command.Arguments.Contains($"-p:PublishReadyToRun={(mode == "CoreClrR2R" ? "true" : "false")}"));
                    True(plan.Command.Arguments.Contains($"-p:RunAOTCompilation={(mode == "MonoAot" ? "true" : "false")}"));
                    True(plan.Command.Arguments.Contains($"-p:AndroidEnableProfiledAot={(mode == "MonoAot" ? "true" : "false")}"));
                    True(plan.Command.Arguments.Contains("-p:EmbedAssembliesIntoApk=true"));
                    True(plan.Command.Arguments.Contains("-p:PublishAot=false"));
                    var slug = mode switch { "MonoAot" => "aot", "CoreClrJit" => "jit", "CoreClrR2R" => "r2r", _ => "mono" };
                    Equal($"{app.ArtifactSlug}-android-{slug}-net{version}-release-{rid["android-".Length..]}", Path.GetFileName(plan.Artifacts));
                    Equal("/custom/dotnet", plan.Command.Environment!["DOTNET_HOST_PATH"]); Equal(1200, plan.Command.TimeoutSeconds);
                }
        foreach (var (version, configuration, expected) in new[] { (10, "Release", "MonoAot"), (10, "Debug", "Mono"), (11, "Release", "CoreClrR2R") })
        {
            var plan = await AndroidCli(new FakeRunner(_ => $"{version}.0.401")).BuildPlanAsync(new() { DotnetVersion = version, Configuration = configuration }, App, "android-arm64", "dotnet", default);
            Equal(expected, plan.Mode);
        }
        var options = Options.Parse(["--platform", "ANDROID", "--target", "EMULATOR", "--serial", "emulator-5554", "--mode", "coreclrjit", "--adb-path", "custom adb", "--extra", "doroti_sample=input", "--extra", "VALUE=a=b", "--extra", "VALUE=updated"]);
        Equal("android", options.Platform); Equal("emulator", options.Target); Equal("CoreClrJit", options.Mode);
        Equal("emulator-5554", options.Device); Equal("updated", options.Extras["VALUE"]);
    }

    private static async Task AndroidInvalidOptions()
    {
        foreach (var args in new[] {
            new[] { "--target", "simulator" }, new[] { "--mode", "NativeAot" }, new[] { "--codesign-key", "certificate" },
            new[] { "--codesign-provision", "profile" }, new[] { "--skip-xcode-validation" }, new[] { "--env", "NAME=value" },
            new[] { "--dotnet-version", "11", "--mode", "MonoAot" }, new[] { "--dotnet-version", "11", "--mode", "Mono" },
            new[] { "--mode", "MonoAot", "--configuration", "Debug" } })
        {
            var runner = new FakeRunner();
            await Throws<DeployException>(() => AndroidCli(runner).RunAsync(Options.Parse(args), default));
            Equal(0, runner.Commands.Count);
        }
        foreach (var args in new[] { new[] { "--extra", "bad-name=x" }, new[] { "--extra", "=x" }, new[] { "--platform", "windows" } })
            await Throws<DeployException>(() => Task.FromResult(Options.Parse(args)));
        foreach (var args in new[] { new[] { "--target", "emulator" }, new[] { "--mode", "CoreClrJit" }, new[] { "--extra", "NAME=x" } })
            await Throws<DeployException>(() => Cli(new FakeRunner()).RunAsync(Options.Parse(args), default));
        await Throws<DeployException>(() => AndroidCli(new FakeRunner(_ => "10.0.400")).BuildPlanAsync(new() { DotnetVersion = 11 }, App, "android-arm64", "dotnet", default));
    }

    private static string AndroidProbe(Command command, string abi = "arm64-v8a", string state = "device", string boot = "1") =>
        command.Arguments.Contains("get-state") ? state :
        command.Arguments.Last().Contains("sys.boot_completed", StringComparison.Ordinal) ? boot :
        command.Arguments.Last().Contains("ro.product.cpu.abi", StringComparison.Ordinal) ? abi : "0";

    private static async Task AndroidReadiness()
    {
        foreach (var (abi, rid) in new[] { ("arm64-v8a", "android-arm64"), ("x86_64", "android-x64") })
            Equal(rid, await AndroidCli(new FakeRunner(command => AndroidProbe(command, abi))).EnsureReadyAsync("adb", AndroidDevice, default));
        foreach (var (abi, state, boot) in new[] { ("armeabi-v7a", "device", "1"), ("arm64-v8a", "offline", "1"), ("arm64-v8a", "device", "0") })
            await Throws<DeployException>(() => AndroidCli(new FakeRunner(command => AndroidProbe(command, abi, state, boot))).EnsureReadyAsync("adb", AndroidDevice, default));
    }

    private static Options AndroidOptions(bool list = false, bool dryRun = false) => new()
    {
        App = "Sample2", Device = AndroidDevice.Serial, AdbPath = Executables.Resolve("dotnet"), List = list, DryRun = dryRun,
    };

    private static string AndroidRunProbe(Command command)
    {
        if (command.Arguments[0] == "devices") return "USB-SERIAL device model:Test_Phone";
        if (command.Arguments[0] == "--version") return "10.0.401";
        return AndroidProbe(command);
    }

    private static async Task AndroidReadOnly()
    {
        foreach (var options in new[] { AndroidOptions(list: true), AndroidOptions(dryRun: true) })
        {
            var runner = new FakeRunner(AndroidRunProbe);
            await AndroidCli(runner).RunAsync(options, default);
            True(runner.Commands.All(command => command.Capture));
            True(runner.Commands.All(command => command.Arguments[0] is not ("build" or "publish") && !command.Arguments.Contains("install") && !command.Arguments.Last().Contains("am", StringComparison.Ordinal)));
            if (options.List) True(runner.Commands.All(command => !command.Arguments.Contains("--version")));
        }
        foreach (var state in new[] { "offline", "unauthorized", "no permissions" })
        {
            var runner = new FakeRunner(_ => "USB-SERIAL " + state);
            await Throws<DeployException>(() => AndroidCli(runner).RunAsync(AndroidOptions(dryRun: true), default));
            Equal(1, runner.Commands.Count);
        }
    }

    private static async Task AndroidFailedBuild()
    {
        using var temp = new TempDirectory();
        foreach (var failBuild in new[] { true, false })
        {
            var runner = new FakeRunner(command => command.Arguments[0] == "build"
                ? failBuild ? throw new DeployException("Build failed") : ""
                : AndroidRunProbe(command));
            await Throws<DeployException>(() => AndroidCli(runner, root: temp.Path).RunAsync(AndroidOptions(), default));
            True(runner.Commands.Any(command => command.Arguments[0] == "build"));
            True(runner.Commands.All(command => !command.Arguments.Contains("install")));
        }
        // A successful build must still recheck the target before installing.
        foreach (var disconnect in new[] { false, true })
        {
            var built = false;
            var runner = new FakeRunner(command =>
            {
                if (command.Arguments[0] == "build")
                {
                    built = true;
                    var artifacts = command.Arguments.Single(value => value.StartsWith("-p:ArtifactsPath=", StringComparison.Ordinal))["-p:ArtifactsPath=".Length..];
                    var directory = Directory.CreateDirectory(Path.Combine(artifacts, "bin", App.Folder + ".Android", "release_android-arm64")).FullName;
                    File.WriteAllText(Path.Combine(directory, App.BundleId + "-Signed.apk"), "");
                    return "";
                }
                if (built && command.Arguments.Contains("get-state") && disconnect) return "offline";
                if (command.Arguments.Contains("install") || command.Arguments.Last().Contains("'resolve-activity'", StringComparison.Ordinal) || command.Arguments.Last().Contains("'start'", StringComparison.Ordinal)
                    || command.Arguments.Last().Contains("'force-stop'", StringComparison.Ordinal) || command.Arguments.Last().StartsWith("pidof", StringComparison.Ordinal))
                    return AndroidDeployProbe(command);
                return AndroidRunProbe(command);
            });
            if (disconnect)
            {
                await Throws<DeployException>(() => AndroidCli(runner, root: temp.Path).RunAsync(AndroidOptions(), default));
                True(runner.Commands.All(command => !command.Arguments.Contains("install")));
            }
            else
            {
                await AndroidCli(runner, root: temp.Path).RunAsync(AndroidOptions(), default);
                True(runner.Commands.Any(command => command.Arguments.Contains("install")));
            }
            Equal(2, runner.Commands.Count(command => command.Arguments.Contains("get-state")));
        }
    }

    private static Task AndroidApks()
    {
        using var temp = new TempDirectory();
        foreach (var app in SampleApp.All)
        {
            var directory = Path.Combine(temp.Path, "bin", app.Folder + ".Android", "release_android-arm64");
            Directory.CreateDirectory(directory);
            var apk = Path.Combine(directory, app.BundleId + "-Signed.apk");
            File.WriteAllText(apk, ""); File.WriteAllText(Path.Combine(directory, "other-Signed.apk"), "");
            Equal(apk, AndroidDeploymentCli.FindApk(temp.Path, app));
            var duplicate = Directory.CreateDirectory(Path.Combine(directory, "duplicate")).FullName;
            File.WriteAllText(Path.Combine(duplicate, app.BundleId + "-Signed.apk"), "");
            try { AndroidDeploymentCli.FindApk(temp.Path, app); throw new Exception("Expected rejection"); } catch (DeployException) { }
        }
        try { AndroidDeploymentCli.FindApk(Path.Combine(temp.Path, "missing"), App); throw new Exception("Expected rejection"); } catch (DeployException) { }
        return Task.CompletedTask;
    }

    private static async Task AndroidLauncherResolution()
    {
        // MAUI launcher filters have no DEFAULT category; --brief reports that
        // fact before printing the SDK-generated component name on the next line.
        const string component = "dev.doroti.sample2/crc6467bcc435301192e0.MainActivity";
        Equal(component, AndroidDeploymentCli.ParseLauncher("priority=0 preferredOrder=0 isDefault=false\r\n" + component + "\r\n", App));
        Equal("dev.doroti.sample2/.MainActivity", AndroidDeploymentCli.ParseLauncher("dev.doroti.sample2/.MainActivity", App));
        foreach (var invalid in new[] { "", "No activity found", "android/com.android.internal.app.ResolverActivity", "other.package/.MainActivity",
                     component + "\ndev.doroti.sample2/.OtherActivity" })
            await Throws<DeployException>(() => Task.FromResult(AndroidDeploymentCli.ParseLauncher(invalid, App)));
        var runner = new FakeRunner(_ => "No activity found");
        await Throws<DeployException>(() => AndroidCli(runner).LaunchAsync("adb", AndroidDevice, App, new Dictionary<string, string>(), default));
        Equal(1, runner.Commands.Count);
        True(runner.Commands[0].Arguments.Last().Contains("'--user' 'current'", StringComparison.Ordinal));
    }

    private static string AndroidDeployProbe(Command command) => command.Arguments.Contains("install") ? "Performing Streamed Install\nSuccess\n" :
        command.Arguments.Last().Contains("'resolve-activity'", StringComparison.Ordinal) ? "priority=0 isDefault=false\n" +
            SampleApp.All.Single(app => command.Arguments.Last().Contains("'" + app.BundleId + "'", StringComparison.Ordinal)).BundleId + "/crc64.GeneratedMainActivity\n" :
        command.Arguments.Last().Contains("'start'", StringComparison.Ordinal) ? "Starting: Intent\nStatus: ok\nComplete" :
        command.Arguments.Last().StartsWith("pidof", StringComparison.Ordinal) ? "1234 5678" : "";

    private static async Task AndroidDeployment()
    {
        foreach (var app in SampleApp.All)
        {
            var runner = new FakeRunner(AndroidDeployProbe);
            var ui = new FakeUi();
            var extras = new Dictionary<string, string> { ["doroti_sample"] = "input", ["VALUE"] = "literal $(nothing); a=b 'quoted' \"double\"" };
            await AndroidCli(runner, ui).DeployAsync("custom adb", AndroidDevice, "APK with spaces.apk", app, extras, false, default);
            Equal(5, runner.Commands.Count); True(runner.Commands[0].Arguments.Contains("APK with spaces.apk"));
            True(runner.Commands[1].Arguments.Last().Contains("'resolve-activity'", StringComparison.Ordinal));
            True(runner.Commands[2].Arguments.Last().Contains("'force-stop'", StringComparison.Ordinal));
            var launch = runner.Commands[3].Arguments.Last();
            True(launch.Contains("'--es' 'doroti_sample' 'input'", StringComparison.Ordinal));
            True(launch.Contains("'VALUE' 'literal $(nothing); a=b '\"'\"'quoted'\"'\"' \"double\"'", StringComparison.Ordinal));
            True(launch.Contains("'-n' '" + app.BundleId + "/crc64.GeneratedMainActivity'", StringComparison.Ordinal));
            True(!launch.Contains("'-p'", StringComparison.Ordinal));
            True(ui.Output.Any(message => message.Contains("--pid=1234", StringComparison.Ordinal)));
            runner.Commands.Clear();
            await AndroidCli(runner).DeployAsync("adb", AndroidDevice, "Test.apk", app, extras, true, default);
            Equal(1, runner.Commands.Count);
        }
    }

    private static async Task AndroidDeployFailures()
    {
        foreach (var response in new[] { "Failure [INSTALL_FAILED_UPDATE_INCOMPATIBLE]", "", "Success\nFailure" })
        {
            var runner = new FakeRunner(_ => response);
            await Throws<DeployException>(() => AndroidCli(runner).DeployAsync("adb", AndroidDevice, "Test.apk", App, new Dictionary<string, string>(), false, default));
            True(runner.Commands.All(command => !command.Arguments.Last().Contains("'start'", StringComparison.Ordinal)));
        }
        var transport = new FakeRunner(_ => throw new DeployException("Transport lost"));
        await Throws<DeployException>(() => AndroidCli(transport).DeployAsync("adb", AndroidDevice, "Test.apk", App, new Dictionary<string, string>(), false, default));
        Equal(1, transport.Commands.Count);
        var launch = new FakeRunner(command => command.Arguments.Last().Contains("'start'", StringComparison.Ordinal) ? "Error: Activity not started" : AndroidDeployProbe(command));
        await Throws<DeployException>(() => AndroidCli(launch).DeployAsync("adb", AndroidDevice, "Test.apk", App, new Dictionary<string, string>(), false, default));
        True(launch.Commands.All(command => !command.Arguments.Last().StartsWith("pidof", StringComparison.Ordinal)));
    }

    private static async Task AndroidPidCancellation()
    {
        var runner = new FakeRunner(command => command.Arguments.Last().StartsWith("pidof", StringComparison.Ordinal) ? "DOROTI_NOT_RUNNING" : AndroidDeployProbe(command));
        var ui = new FakeUi();
        using var ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
        await Throws<OperationCanceledException>(() => AndroidCli(runner, ui).DeployAsync("adb", AndroidDevice, "Test.apk", App, new Dictionary<string, string>(), false, ct.Token));
        True(!ui.Output.Any(message => message.StartsWith("\nInstalled", StringComparison.Ordinal)));
    }
}
