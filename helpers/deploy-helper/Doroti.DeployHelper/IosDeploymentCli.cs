using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Xml;

namespace Doroti.DeployHelper;

public sealed class IosDeploymentCli(string repositoryRoot, ICommandRunner runner, IUserInterface ui)
{
    public string RepositoryRoot { get; } = Path.GetFullPath(repositoryRoot);

    private async Task<JsonNode> CoreDeviceAsync(string[] args, CancellationToken ct)
    {
        var directory = Directory.CreateTempSubdirectory("doroti-ios-");
        try
        {
            var output = Path.Combine(directory.FullName, "result.json");
            await runner.RunAsync(new("xcrun", ["devicectl", .. args, "--timeout", "30", "--json-output", output]), ct);
            var data = JsonNode.Parse(await File.ReadAllTextAsync(output, ct));
            if (data?["info"]?["outcome"]?.ToString() != "success")
                throw new DeployException($"devicectl did not succeed: {data?["error"] ?? data?["info"]}");
            return data["result"] ?? throw new DeployException("devicectl did not return a result.");
        }
        finally { directory.Delete(recursive: true); }
    }

    private static string Text(JsonNode? node, string key, string fallback = "") => node?[key]?.ToString() ?? fallback;

    private static (JsonNode? Hardware, JsonNode? State, JsonNode? Connection, JsonNode? Software) Fields(JsonNode node) => (
        node["properties"]?["hardware"] ?? node["hardwareProperties"],
        node["properties"]?["state"] ?? node["deviceProperties"],
        node["properties"]?["connection"] ?? node["connectionProperties"],
        node["properties"]?["software"] ?? node["deviceProperties"]);

    public static List<IosTarget> PhysicalTargets(JsonNode devices)
    {
        var targets = new List<IosTarget>();
        foreach (var device in devices.AsArray().OfType<JsonObject>())
        {
            var (hardware, state, connection, software) = Fields(device);
            if (Text(hardware, "platform") != "iOS" || Text(hardware, "reality") != "physical") continue;
            var udid = Text(hardware, "udid");
            if (udid.Length == 0) continue;
            var version = software?["osVersionNumber"];
            targets.Add(new("device", Text(state, "name", Text(hardware, "marketingName", "iPhone/iPad")), udid,
                Text(device, "identifier", udid), Text(connection, "state", Text(connection, "tunnelState", "unknown")),
                version is JsonObject ? Text(version, "stringValue", "?") : version?.ToString() ?? "?"));
        }
        return targets;
    }

    public static List<IosTarget> SimulatorTargets(JsonNode data)
    {
        var targets = new List<IosTarget>();
        if (data["devices"] is not JsonObject runtimes) return targets;
        foreach (var (runtime, devices) in runtimes)
        {
            const string marker = ".iOS-";
            var index = runtime.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0 || devices is not JsonArray list) continue;
            var version = runtime[(index + marker.Length)..].Replace('-', '.');
            foreach (var device in list.OfType<JsonObject>())
                if (device["isAvailable"]?.GetValue<bool>() == true)
                    targets.Add(new("simulator", Text(device, "name"), Text(device, "udid"), Text(device, "udid"), Text(device, "state"), version));
        }
        return targets;
    }

    public async Task<List<IosTarget>> DiscoverTargetsAsync(string kind, CancellationToken ct)
    {
        var targets = new List<IosTarget>();
        if (kind is "auto" or "device")
        {
            try
            {
                var result = await CoreDeviceAsync(["list", "devices"], ct);
                targets.AddRange(PhysicalTargets(result["devices"] ?? new JsonArray()));
            }
            catch (DeployException error) { ui.WriteLine("Discovery warning: " + error.Message); }
        }
        if (kind is "auto" or "simulator")
        {
            try
            {
                var output = await runner.RunAsync(new("xcrun", ["simctl", "list", "devices", "available", "-j"]), ct);
                targets.AddRange(SimulatorTargets(JsonNode.Parse(output) ?? new JsonObject()));
            }
            catch (DeployException error) { ui.WriteLine("Discovery warning: " + error.Message); }
        }
        return targets.OrderBy(target => target.Kind, StringComparer.Ordinal).ThenBy(target => target.State != "Booted")
            .ThenBy(target => target.Name, StringComparer.Ordinal).ThenBy(target => target.Udid, StringComparer.Ordinal).ToList();
    }

    public static List<Identity> ParseIdentities(string output) => Regex.Matches(output,
            "^\\s*\\d+\\)\\s+([0-9A-Fa-f]{40})\\s+\"([^\"]+)\"\\s*$", RegexOptions.Multiline)
        .Select(match => new Identity(match.Groups[1].Value.ToUpperInvariant(), match.Groups[2].Value))
        .Where(identity => identity.Name.StartsWith("Apple Development:", StringComparison.Ordinal) || identity.Name.StartsWith("iPhone Developer:", StringComparison.Ordinal))
        .DistinctBy(identity => identity.Sha1).OrderBy(identity => identity.Name, StringComparer.Ordinal).ThenBy(identity => identity.Sha1).ToList();

    public async Task<List<Identity>> DiscoverIdentitiesAsync(CancellationToken ct) =>
        ParseIdentities(await runner.RunAsync(new("security", ["find-identity", "-v", "-p", "codesigning"]), ct));

    public async Task<List<ProvisionProfile>> DiscoverProfilesAsync(CancellationToken ct, IEnumerable<string>? directories = null)
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        directories ??= [Path.Combine(home, "Library/MobileDevice/Provisioning Profiles"), Path.Combine(home, "Library/Developer/Xcode/UserData/Provisioning Profiles")];
        var profiles = new Dictionary<string, ProvisionProfile>(StringComparer.OrdinalIgnoreCase);
        foreach (var directory in directories.Where(Directory.Exists))
            foreach (var path in Directory.EnumerateFiles(directory, "*.mobileprovision").Order(StringComparer.Ordinal))
            {
                try
                {
                    var xml = await runner.RunAsync(new("security", ["cms", "-D", "-i", path]), ct);
                    var profile = new ProvisionProfile(path, Plist.Parse(xml));
                    if (profile.Uuid.Length > 0) profiles[profile.Uuid] = profile;
                }
                catch (Exception error) when (error is DeployException or XmlException or FormatException or IOException)
                { ui.WriteLine($"Skipping unreadable profile {Path.GetFileName(path)}: {error.Message}"); }
            }
        return profiles.Values.OrderBy(profile => profile.Name, StringComparer.Ordinal).ThenBy(profile => profile.Uuid).ToList();
    }

    public Task<T> SelectAsync<T>(string title, IReadOnlyList<T> items, Func<T, string> label, string option,
        CancellationToken ct, string? requested = null, Func<T, string, bool>? matches = null)
        => new Selector(ui).SelectAsync(title, items, label, option, ct, requested, matches);

    public async Task EnsureDeviceReadyAsync(IosTarget target, CancellationToken ct)
    {
        var details = await CoreDeviceAsync(["device", "info", "details", "--device", target.Identifier], ct);
        var (_, state, connection, _) = Fields(details);
        if (Text(connection, "state", Text(connection, "tunnelState")) != "connected")
            throw new DeployException("The device is disconnected. Connect it, unlock it and trust this Mac in Finder/Xcode.");
        var developerMode = state?["developerModeStatus"];
        if (developerMode?.ToString() != "enabled" && !(developerMode is JsonObject obj && obj.ContainsKey("enabled")))
            throw new DeployException("Enable Settings > Privacy & Security > Developer Mode on the device, then restart it.");
    }

    public static void ValidateBuildOptions(Options options, IosTarget target)
    {
        options.ValidateForPlatform("ios");
        var mode = options.Mode ?? (target.Kind == "device" ? "NativeAot" : "Mono");
        var configuration = options.Configuration ?? (target.Kind == "device" ? "Release" : "Debug");
        if (mode == "NativeAot" && (target.Kind != "device" || configuration != "Release"))
            throw new DeployException("NativeAot requires a physical device and Release. Use -Mode Mono for Simulator/Debug.");
        if (target.Kind == "simulator" && (options.CodesignKey is not null || options.CodesignProvision is not null))
            throw new DeployException("Signing options require -Target device; Simulator does not need a development certificate.");
    }

    public async Task<BuildPlan> BuildPlanAsync(Options options, SampleApp app, IosTarget target, Identity? identity,
        ProvisionProfile? profile, string dotnet, CancellationToken ct)
    {
        ValidateBuildOptions(options, target);
        var mode = options.Mode ?? (target.Kind == "device" ? "NativeAot" : "Mono");
        var configuration = options.Configuration ?? (target.Kind == "device" ? "Release" : "Debug");
        var rid = "ios-arm64";
        if (target.Kind == "simulator")
        {
            bool arm64;
            try { arm64 = (await runner.RunAsync(new("sysctl", ["-n", "hw.optional.arm64"]), ct)).Trim() == "1"; }
            catch (DeployException) { arm64 = RuntimeInformation.OSArchitecture == Architecture.Arm64; }
            rid = arm64 ? "iossimulator-arm64" : "iossimulator-x64";
        }
        var cwd = options.DotnetVersion == 10 ? RepositoryRoot : Path.Combine(RepositoryRoot, "samples/DorotiTestbedApp/ios");
        var sdk = (await runner.RunAsync(new(dotnet, ["--version"], cwd), ct)).Trim();
        if (!sdk.StartsWith($"{options.DotnetVersion}.", StringComparison.Ordinal))
            throw new DeployException($"Expected .NET {options.DotnetVersion} SDK, but {cwd}/global.json selected {sdk}.");
        var artifacts = Path.Combine(RepositoryRoot, "Doroti/artifacts",
            $"{app.ArtifactSlug}-ios-{mode.ToLowerInvariant()}-net{options.DotnetVersion}-{configuration.ToLowerInvariant()}-{rid}");
        var framework = options.DotnetVersion == 10 ? "net10.0-ios27.0" : "net11.0-ios";
        var maui = options.DotnetVersion == 10 ? "10.0.110" : "11.0.0-rc.1.26451.6";
        var args = new List<string>
        {
            mode == "NativeAot" ? "publish" : "build", app.Project(RepositoryRoot), "--disable-build-servers", "-nr:false",
            "-c", configuration, "-r", rid, $"-p:DorotiCompilationMode={mode}", $"-p:DorotiIosTargetFramework={framework}",
            $"-p:DorotiIosMauiVersion={maui}", $"-p:ArtifactsPath={artifacts}",
        };
        if (mode == "Mono") args.Add("-p:PublishAot=false");
        if (mode == "NativeAot")
            // Both pinned SDKs generate invalid NSObject DynamicDependency entries
            // for inherited interface members (IL2037). Use the managed registrar
            // and each sample's NativeAotRoots.xml to preserve UIKit entry points.
            args.AddRange(["-p:Registrar=managed-static", "-p:_UseDynamicDependenciesForMarkNSObjects=false",
                "-p:MtouchExtraArgs=--skip-marking-nsobjects-in-user-assemblies=true", "-p:Optimize=true"]);
        if (mode == "NativeAot" && options.DotnetVersion == 11)
            // The RC1 assembly-preparer cannot run MarkNSObjects without dynamic
            // dependencies (MT2080). Use the supported ILLink registrar path.
            args.AddRange(["-p:PrepareAssemblies=false", "-p:PostProcessAssemblies=false"]);
        if (options.SkipXcodeValidation) args.Add("-p:ValidateXcodeVersion=false");
        if (target.Kind == "device")
        {
            if (identity is null || profile is null) throw new DeployException("Physical device builds require a development certificate and profile.");
            args.AddRange(["-p:EnableCodeSigning=true", $"-p:CodesignKey={identity.Sha1}", $"-p:CodesignProvision={profile.Uuid}"]);
        }
        else args.AddRange(["-p:EnableCodeSigning=false", "-p:CodesignRequireProvisioningProfile=false"]);
        ui.WriteLine($"\nSDK {sdk} / {mode} / {configuration} / {rid}\nSDK working directory: {cwd}\nArtifacts: {artifacts}");
        return new(new(dotnet, args, cwd, Capture: false, TimeoutSeconds: 1200,
            Environment: new Dictionary<string, string> { ["DOTNET_HOST_PATH"] = dotnet }), artifacts, sdk, mode, configuration, rid);
    }

    public async Task<string> FindAppBundleAsync(string artifacts, SampleApp app, CancellationToken ct)
    {
        var directory = Path.Combine(artifacts, "bin", app.Folder + ".iOS");
        var candidates = Directory.Exists(directory)
            ? Directory.GetDirectories(directory, app.Folder + ".iOS.app", SearchOption.AllDirectories) : [];
        if (candidates.Length != 1)
            throw new DeployException($"Expected one {app.Folder}.iOS.app under {artifacts}/bin; found {candidates.Length}.");
        // plutil handles both binary and XML Info.plist without altering the app bundle.
        var xml = await runner.RunAsync(new("plutil", ["-convert", "xml1", "-o", "-", Path.Combine(candidates[0], "Info.plist")]), ct);
        var bundleId = Plist.String(Plist.Parse(xml), "CFBundleIdentifier");
        if (bundleId != app.BundleId) throw new DeployException($"Unexpected bundle ID '{bundleId}'; expected '{app.BundleId}'.");
        return candidates[0];
    }

    public async Task DeployAsync(IosTarget target, string bundle, SampleApp app, IReadOnlyDictionary<string, string> environment, bool noLaunch, CancellationToken ct)
    {
        if (target.Kind == "device")
        {
            await runner.RunAsync(new("codesign", ["--verify", "--deep", "--strict", bundle], Capture: false), ct);
            await runner.RunAsync(new("xcrun", ["devicectl", "device", "install", "app", "--device", target.Identifier, bundle], Capture: false, TimeoutSeconds: 180), ct);
            if (!noLaunch)
            {
                var args = new List<string> { "devicectl", "device", "process", "launch", "--device", target.Identifier, "--terminate-existing" };
                if (environment.Count > 0) args.AddRange(["--environment-variables", JsonSerializer.Serialize(environment)]);
                args.Add(app.BundleId);
                await runner.RunAsync(new("xcrun", args, Capture: false), ct);
            }
        }
        else
        {
            var data = await runner.RunAsync(new("xcrun", ["simctl", "list", "devices", "available", "-j"]), ct);
            var selected = SimulatorTargets(JsonNode.Parse(data) ?? new JsonObject()).SingleOrDefault(item => item.Udid == target.Udid)
                ?? throw new DeployException("The selected simulator is no longer available.");
            if (selected.State != "Booted") await runner.RunAsync(new("xcrun", ["simctl", "boot", target.Udid], Capture: false), ct);
            await runner.RunAsync(new("xcrun", ["simctl", "bootstatus", target.Udid, "-b"], Capture: false, TimeoutSeconds: 180), ct);
            await runner.RunAsync(new("open", ["-a", "Simulator", "--args", "-CurrentDeviceUDID", target.Udid], Capture: false), ct);
            await runner.RunAsync(new("xcrun", ["simctl", "install", target.Udid, bundle], Capture: false, TimeoutSeconds: 180), ct);
            if (!noLaunch)
                await runner.RunAsync(new("xcrun", ["simctl", "launch", "--terminate-running-process", target.Udid, app.BundleId], Capture: false,
                    Environment: environment.ToDictionary(pair => "SIMCTL_CHILD_" + pair.Key, pair => pair.Value)), ct);
        }
        ui.WriteLine($"\nInstalled{(noLaunch ? "" : " and launched")}: {app.BundleId} / {target.Name}\nApp: {bundle}");
    }

    public async Task RunAsync(Options options, CancellationToken ct)
    {
        options.ValidateForPlatform("ios");
        var targets = await DiscoverTargetsAsync(options.Target, ct);
        if (options.List)
        {
            void Print(string title, IEnumerable<string> labels)
            {
                var items = labels.ToArray();
                ui.WriteLine("\n" + title);
                foreach (var label in items) ui.WriteLine("  " + label);
                if (items.Length == 0) ui.WriteLine("  (none)");
            }
            Print("Known devices and available iOS simulators", targets.Select(target => target.Label));
            Print("Valid development signing identities (with private keys)", (await DiscoverIdentitiesAsync(ct)).Select(identity => identity.Label));
            Print("Installed provisioning profiles (compatibility checked during selection)", (await DiscoverProfilesAsync(ct)).Select(profile => profile.Label));
            return;
        }
        var dotnet = Executables.Resolve(options.DotnetPath);
        var app = await SelectAsync("Sample app", SampleApp.All, item => item.Folder, "--app / -App", ct, options.App,
            (item, value) => item.Key.Equals(value, StringComparison.OrdinalIgnoreCase));
        if (targets.Count == 0) throw new DeployException("No iOS targets found. Connect/trust a device or install an iOS Simulator runtime in Xcode.");
        var target = await SelectAsync("iPhone/iPad or Simulator", targets, item => item.Label, "--device / -Device", ct, options.Device,
            (item, value) => item.Udid.Equals(value, StringComparison.OrdinalIgnoreCase) || item.Identifier.Equals(value, StringComparison.OrdinalIgnoreCase));
        ValidateBuildOptions(options, target);
        Identity? identity = null;
        ProvisionProfile? profile = null;
        if (target.Kind == "device")
        {
            if (!options.DryRun) await EnsureDeviceReadyAsync(target, ct);
            var identities = await DiscoverIdentitiesAsync(ct);
            if (identities.Count == 0) throw new DeployException("No valid development signing identity with a private key. Create/download one in Xcode > Settings > Accounts.");
            identity = await SelectAsync("Development certificate (CodesignKey)", identities, item => item.Label, "--codesign-key / -CodesignKey", ct,
                options.CodesignKey, (item, value) => item.Sha1.Equals(value, StringComparison.OrdinalIgnoreCase) || item.Name == value);
            var profiles = (await DiscoverProfilesAsync(ct)).Where(item => item.Matches(app, target, identity)).ToList();
            if (profiles.Count == 0)
                throw new DeployException($"No unexpired iOS development profile matches {app.BundleId}, {target.Udid} and the selected certificate. " +
                    "In Xcode, sign an app with this bundle ID and device, then download its profile. " +
                    "Both ~/Library/MobileDevice/Provisioning Profiles and ~/Library/Developer/Xcode/UserData/Provisioning Profiles are searched.");
            profile = await SelectAsync("Compatible development profile (CodesignProvision)", profiles, item => item.Label, "--codesign-provision / -CodesignProvision", ct,
                options.CodesignProvision, (item, value) => item.Uuid.Equals(value, StringComparison.OrdinalIgnoreCase) || item.Name == value);
        }
        var plan = await BuildPlanAsync(options, app, target, identity, profile, dotnet, ct);
        if (options.DryRun)
        {
            ui.WriteLine("\nDry run: " + plan.Command.Display);
            ui.WriteLine($"After a successful build: install on {target.Udid}" + (options.NoLaunch ? "." : $"; launch {app.BundleId}"));
            return;
        }
        await runner.RunAsync(plan.Command, ct);
        var bundle = await FindAppBundleAsync(plan.Artifacts, app, ct);
        if (target.Kind == "device")
        {
            if (!profile!.Matches(app, target, identity!)) throw new DeployException("The selected provisioning profile expired during the build. Renew it in Xcode and retry.");
            await EnsureDeviceReadyAsync(target, ct);
        }
        await DeployAsync(target, bundle, app, options.Environment, options.NoLaunch, ct);
    }
}
