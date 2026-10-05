using System.Text.RegularExpressions;

namespace Doroti.DeployHelper;

public sealed class AndroidDeploymentCli(string repositoryRoot, ICommandRunner runner, IUserInterface ui)
{
    public string RepositoryRoot { get; } = Path.GetFullPath(repositoryRoot);

    public static List<AndroidTarget> ParseTargets(string output)
    {
        var targets = new List<AndroidTarget>();
        foreach (var line in output.Split('\n'))
        {
            var match = Regex.Match(line, @"^(\S+)\s+(device|offline|unauthorized|no permissions)(?:\s|$)");
            if (!match.Success) continue;
            var serial = match.Groups[1].Value;
            var model = Regex.Match(line, @"\bmodel:(\S+)");
            targets.Add(new(serial, match.Groups[2].Value, model.Success ? model.Groups[1].Value.Replace('_', ' ') : serial,
                serial.StartsWith("emulator-", StringComparison.Ordinal) ? "emulator" : "device"));
        }
        return targets.OrderBy(target => target.Kind, StringComparer.Ordinal).ThenBy(target => target.Serial, StringComparer.Ordinal).ToList();
    }

    public async Task<List<AndroidTarget>> DiscoverTargetsAsync(string adb, string kind, CancellationToken ct)
    {
        var targets = ParseTargets(await runner.RunAsync(new(adb, ["devices", "-l"]), ct));
        // An emulator connected over TCP can have an IP:port serial instead of emulator-NNNN.
        for (var i = 0; i < targets.Count; i++)
            if (targets[i] is { State: "device", Kind: "device" } target)
            {
                var qemu = await runner.RunAsync(Shell(adb, target, ["getprop", "ro.kernel.qemu"]), ct);
                if (qemu.Trim() == "1") targets[i] = target with { Kind = "emulator" };
            }
        return targets.Where(target => kind == "auto" || target.Kind == kind).ToList();
    }

    // adb joins shell arguments for a remote POSIX shell. Quote each token at that
    // boundary, including Intent values containing spaces, quotes or metacharacters.
    public static Command Shell(string adb, AndroidTarget target, IEnumerable<string> arguments, int timeoutSeconds = 60) =>
        new(adb, ["-s", target.Serial, "shell", string.Join(" ", arguments.Select(RemoteQuote))], TimeoutSeconds: timeoutSeconds);

    private static string RemoteQuote(string value) => "'" + value.Replace("'", "'\"'\"'", StringComparison.Ordinal) + "'";

    public async Task<string> EnsureReadyAsync(string adb, AndroidTarget target, CancellationToken ct)
    {
        var state = (await runner.RunAsync(new(adb, ["-s", target.Serial, "get-state"]), ct)).Trim();
        if (state != "device") throw new DeployException($"Device '{target.Serial}' is {state}. Unlock it and authorize USB debugging, or start the emulator.");
        var booted = (await runner.RunAsync(Shell(adb, target, ["getprop", "sys.boot_completed"]), ct)).Trim();
        if (booted != "1") throw new DeployException($"Device '{target.Serial}' has not completed boot. Wait for Android to start and retry.");
        var abi = (await runner.RunAsync(Shell(adb, target, ["getprop", "ro.product.cpu.abi"]), ct)).Trim();
        return abi switch
        {
            "arm64-v8a" => "android-arm64",
            "x86_64" => "android-x64",
            _ => throw new DeployException($"Device '{target.Serial}' uses unsupported ABI '{abi}'. These samples support arm64-v8a and x86_64."),
        };
    }

    public async Task<BuildPlan> BuildPlanAsync(Options options, SampleApp app, string rid, string dotnet, CancellationToken ct)
    {
        options.ValidateForPlatform("android");
        if (rid is not ("android-arm64" or "android-x64")) throw new DeployException($"Unsupported Android RID: {rid}.");
        var configuration = options.Configuration ?? "Release";
        var mode = options.Mode ?? (options.DotnetVersion == 11 ? "CoreClrR2R" : configuration == "Release" ? "MonoAot" : "Mono");
        var cwd = options.DotnetVersion == 10 ? RepositoryRoot : Path.Combine(RepositoryRoot, "samples/DorotiSampleApp2/android/sdk/net11");
        var sdk = (await runner.RunAsync(new(dotnet, ["--version"], cwd), ct)).Trim();
        if (!sdk.StartsWith($"{options.DotnetVersion}.", StringComparison.Ordinal))
            throw new DeployException($"Expected .NET {options.DotnetVersion} SDK, but {cwd}/global.json selected {sdk}.");
        var artifactMode = mode switch { "MonoAot" => "aot", "CoreClrJit" => "jit", "CoreClrR2R" => "r2r", _ => "mono" };
        // Keep Windows aapt2 resource paths short while isolating every variant.
        var artifacts = Path.Combine(RepositoryRoot, "Doroti/artifacts",
            $"{app.ArtifactSlug}-android-{artifactMode}-net{options.DotnetVersion}-{configuration.ToLowerInvariant()}-{rid["android-".Length..]}");
        var args = new List<string>
        {
            "build", app.AndroidProject(RepositoryRoot), "--disable-build-servers", "-nr:false", "-c", configuration, "-r", rid,
            // The runner's Mono compilation profile permits both Mono and CoreCLR
            // Android variants; UseMonoRuntime chooses the actual runtime.
            "-p:DorotiCompilationMode=Mono", "-p:PublishAot=false", $"-p:UseMonoRuntime={(mode is "Mono" or "MonoAot" ? "true" : "false")}",
            $"-p:PublishReadyToRun={(mode == "CoreClrR2R" ? "true" : "false")}",
            $"-p:RunAOTCompilation={(mode == "MonoAot" ? "true" : "false")}",
            $"-p:AndroidEnableProfiledAot={(mode == "MonoAot" ? "true" : "false")}",
            $"-p:DorotiAndroidTargetFramework=net{options.DotnetVersion}.0-android",
            "-p:AndroidPackageFormats=apk", "-p:EmbedAssembliesIntoApk=true", $"-p:ArtifactsPath={artifacts}",
        };
        if (options.DotnetVersion == 11)
            args.AddRange(["-p:DorotiAndroidMauiVersion=11.0.0-rc.1.26451.6", "-p:MauiVersion=11.0.0-rc.1.26451.6"]);
        ui.WriteLine($"\nSDK {sdk} / {mode} / {configuration} / {rid}\nSDK working directory: {cwd}\nArtifacts: {artifacts}");
        return new(new(dotnet, args, cwd, Capture: false, TimeoutSeconds: 1200,
            Environment: new Dictionary<string, string> { ["DOTNET_HOST_PATH"] = dotnet }), artifacts, sdk, mode, configuration, rid);
    }

    public static string FindApk(string artifacts, SampleApp app)
    {
        var directory = Path.Combine(artifacts, "bin", app.Folder + ".Android");
        var candidates = Directory.Exists(directory)
            ? Directory.GetFiles(directory, app.BundleId + "-Signed.apk", SearchOption.AllDirectories) : [];
        if (candidates.Length != 1)
            throw new DeployException($"Expected one {app.BundleId}-Signed.apk under {directory}; found {candidates.Length}.");
        return candidates[0];
    }

    public static string ParseLauncher(string output, SampleApp app)
    {
        // --brief can emit a priority/status line before the component. Accept
        // only a component belonging to the requested package, never that status line.
        var components = Regex.Matches(output, @"(?m)^\s*(" + Regex.Escape(app.BundleId) + @"/[A-Za-z0-9_.$]+)\s*$")
            .Select(match => match.Groups[1].Value).Distinct(StringComparer.Ordinal).ToArray();
        if (components.Length != 1)
            throw new DeployException($"Cannot resolve one launcher activity for {app.BundleId} on the current Android user; found {components.Length}. Verify the app is installed and its launcher is enabled.");
        return components[0];
    }

    public async Task DeployAsync(string adb, AndroidTarget target, string apk, SampleApp app,
        IReadOnlyDictionary<string, string> extras, bool noLaunch, CancellationToken ct)
    {
        var install = await runner.RunAsync(new(adb, ["-s", target.Serial, "install", "-r", apk], TimeoutSeconds: 180), ct);
        ui.WriteLine(install.Trim());
        // Some adb versions report package-manager failures with a successful exit.
        if (!Regex.IsMatch(install, @"(?m)^Success\s*$") || install.Contains("Failure", StringComparison.OrdinalIgnoreCase))
            throw new DeployException("Android installation did not report Success; launch was skipped.");
        if (!noLaunch) await LaunchAsync(adb, target, app, extras, ct);
        ui.WriteLine($"\nInstalled{(noLaunch ? "" : " and launched")}: {app.BundleId} / {target.Name}\nAPK: {apk}");
    }

    public async Task LaunchAsync(string adb, AndroidTarget target, SampleApp app,
        IReadOnlyDictionary<string, string> extras, CancellationToken ct)
    {
        // Launcher filters normally contain MAIN/LAUNCHER without DEFAULT.
        // am start's implicit resolution requires DEFAULT, so resolve the
        // installed (potentially SDK-generated) class name and launch explicitly.
        var resolution = await runner.RunAsync(Shell(adb, target, ["cmd", "package", "resolve-activity", "--brief", "--user", "current",
            "-a", "android.intent.action.MAIN", "-c", "android.intent.category.LAUNCHER", "-p", app.BundleId]), ct);
        var component = ParseLauncher(resolution, app);
        await runner.RunAsync(Shell(adb, target, ["am", "force-stop", app.BundleId]), ct);
        var args = new List<string> { "am", "start", "-W", "-a", "android.intent.action.MAIN", "-c", "android.intent.category.LAUNCHER", "-n", component };
        foreach (var (key, value) in extras) args.AddRange(["--es", key, value]);
        var launch = await runner.RunAsync(Shell(adb, target, args), ct);
        ui.WriteLine(launch.Trim());
        if (!Regex.IsMatch(launch, @"(?m)^Status:\s*ok\s*$") || launch.Contains("Error:", StringComparison.OrdinalIgnoreCase))
            throw new DeployException("Android activity launch did not report Status: ok. Inspect adb logcat for startup errors.");
        var pid = "";
        for (var attempt = 0; attempt < 20; attempt++)
        {
            // The sentinel preserves the adb/transport exit status while allowing
            // pidof's 'not running' status to be retried after a successful launch.
            var probe = await runner.RunAsync(new(adb, ["-s", target.Serial, "shell", $"pidof {RemoteQuote(app.BundleId)} || echo DOROTI_NOT_RUNNING"]), ct);
            if (Regex.IsMatch(probe.Trim(), @"^\d+(?:\s+\d+)*$")) { pid = probe.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)[0]; break; }
            if (attempt < 19) await Task.Delay(500, ct);
        }
        if (pid.Length == 0) throw new DeployException("App process did not start. Inspect adb logcat for startup errors.");
        ui.WriteLine($"PID: {pid}\nLogs: {adb} -s {target.Serial} logcat --pid={pid}");
    }

    public async Task RunAsync(Options options, CancellationToken ct)
    {
        options.ValidateForPlatform("android");
        var adb = Executables.ResolveAdb(options.AdbPath);
        var targets = await DiscoverTargetsAsync(adb, options.Target, ct);
        if (options.List)
        {
            ui.WriteLine("\nAndroid devices and running emulators (adb devices -l)");
            foreach (var item in targets) ui.WriteLine("  " + item.Label);
            if (targets.Count == 0) ui.WriteLine("  (none)");
            return;
        }
        var select = new Selector(ui);
        var app = await select.SelectAsync("Sample app", SampleApp.All, item => item.Folder, "--app / -App", ct, options.App,
            (item, value) => item.Key.Equals(value, StringComparison.OrdinalIgnoreCase));
        var target = await select.SelectAsync("Android device or running emulator", targets, item => item.Label, "--device / -Device", ct,
            options.Device, (item, value) => item.Serial == value);
        if (target.State != "device") throw new DeployException($"Device '{target.Serial}' is {target.State}. Unlock it and authorize USB debugging, or start the emulator.");
        var rid = await EnsureReadyAsync(adb, target, ct);
        var dotnet = Executables.Resolve(options.DotnetPath);
        var plan = await BuildPlanAsync(options, app, rid, dotnet, ct);
        if (options.DryRun)
        {
            ui.WriteLine("\nDry run: " + plan.Command.Display);
            ui.WriteLine($"After a successful build: install on {target.Serial}" + (options.NoLaunch ? "." : $"; launch {app.BundleId}"));
            return;
        }
        await runner.RunAsync(plan.Command, ct);
        var apk = FindApk(plan.Artifacts, app);
        if (await EnsureReadyAsync(adb, target, ct) != rid) throw new DeployException("The selected device ABI changed during the build; deployment was stopped.");
        await DeployAsync(adb, target, apk, app, options.Extras, options.NoLaunch, ct);
    }
}
