using System.Text.Json;
using System.Text.RegularExpressions;

namespace Doroti.DeployHelper;

public sealed record WebSdk(string Version, string Directory, int Major)
{
    public string VersionDirectory => Path.Combine(Directory, Version);
    public string EntryPoint => Path.Combine(VersionDirectory, "dotnet.dll");
}

public sealed record WebPublishPlan(Command Command, string WebRoot, string Sdk, int DotnetVersion,
    string Mode, string Configuration, string ExecutionMode, string BuildId);

public sealed class WebDeploymentCli(string repositoryRoot, ICommandRunner runner, IUserInterface ui, IWebPreviewServer server)
{
    public string RepositoryRoot { get; } = Path.GetFullPath(repositoryRoot);

    public static IReadOnlyList<WebSdk> ParseSdks(string output) => output.Split('\n')
        .Select(line => Regex.Match(line.Trim(), @"^(\d+\.\d+\.\d+)\s+\[(.+)\]$"))
        .Where(match => match.Success)
        .Select(match => new WebSdk(match.Groups[1].Value, match.Groups[2].Value, Version.Parse(match.Groups[1].Value).Major))
        .Where(sdk => sdk.Major is 10 or 11)
        .OrderByDescending(sdk => Version.Parse(sdk.Version)).ToArray();

    public async Task<WebPublishPlan> BuildPlanAsync(Options options, SampleApp app, string dotnet, CancellationToken ct)
    {
        options.ValidateForPlatform("web");
        var sdks = ParseSdks(await runner.RunAsync(new(dotnet, ["--list-sdks"], RepositoryRoot), ct));
        if (ui.IsInteractive && !options.DotnetVersionSpecified)
        {
            var available = sdks.GroupBy(sdk => sdk.Major).Select(group => group.First()).OrderBy(sdk => sdk.Major).ToArray();
            var defaultSdk = Array.FindIndex(available, sdk => sdk.Major == 10);
            var selected = await new Selector(ui).SelectAsync("Web .NET version", available,
                sdk => $".NET {sdk.Major} / SDK {sdk.Version}" + (sdk.Major == 11 ? " (qualification preview)" : ""),
                "--dotnet-version / -DotnetVersion", ct, defaultIndex: defaultSdk >= 0 ? defaultSdk : null);
            options.DotnetVersion = selected.Major;
        }
        var sdk = sdks.FirstOrDefault(sdk => sdk.Major == options.DotnetVersion)
                  ?? throw new DeployException($"A stable .NET {options.DotnetVersion} SDK is required. Install it with its wasm-tools workload, or select another --dotnet-version.");
        if (!File.Exists(sdk.EntryPoint)) throw new DeployException($"SDK entry point not found: {sdk.EntryPoint}");
        await SelectBuildOptionsAsync(options, ct);
        options.ValidateForPlatform("web");
        var configuration = options.Configuration ?? "Release";
        var mode = options.Mode ?? (configuration == "Release" ? "MonoAot" : "Mono");
        var execution = options.WebExecutionMode ?? "threaded";
        var buildId = Guid.NewGuid().ToString("N");
        var artifacts = Path.Combine(RepositoryRoot, "Doroti", "artifacts",
            $"{app.ArtifactSlug}-web-{(mode == "MonoAot" ? "aot" : "mono")}-net{options.DotnetVersion}-{configuration.ToLowerInvariant()}-{execution}");
        var publish = Path.Combine(artifacts, "publish");
        var arguments = new List<string>
        {
            // Invoke the selected SDK directly; neither repository global.json is changed.
            sdk.EntryPoint, "publish", app.WebProject(RepositoryRoot), "--disable-build-servers", "-nr:false",
            "-c", configuration, "--artifacts-path", Path.Combine(artifacts, "build"), "-o", publish,
            $"-p:DorotiWebTargetFramework=net{options.DotnetVersion}.0", $"-p:DorotiWebExecutionMode={execution}",
            // Publish reevaluates static web assets in inner builds. A global ID
            // keeps their runtime profiles and the final validator in one generation.
            $"-p:DorotiWebBuildId={buildId}",
            $"-p:RunAOTCompilation={(mode == "MonoAot" ? "true" : "false")}",
            // Desktop preview includes widget/rendering/runtime assemblies in AOT.
            // Sample defaults can otherwise opt those assemblies into interpretation.
            "-p:DorotiWebInterpretWidgetTree=false",
        };
        if (options.DotnetVersion == 11) arguments.Add("-p:DorotiWebQualificationProbe=true");
        // The SDK used by dotnet run can export its MSBuild paths into this helper's environment.
        var environment = new Dictionary<string, string>
        {
            ["DOTNET_HOST_PATH"] = dotnet,
            ["MSBUILD_EXE_PATH"] = Path.Combine(sdk.VersionDirectory, "MSBuild.dll"),
            ["MSBuildExtensionsPath"] = sdk.VersionDirectory,
            ["MSBuildSDKsPath"] = Path.Combine(sdk.VersionDirectory, "Sdks"),
        };
        ui.WriteLine($"\nSDK {sdk.Version} / net{options.DotnetVersion}.0 / {mode} / {configuration} / {execution}\nPublished files: {publish}");
        if (options.DotnetVersion == 11) ui.WriteLine(".NET 11 Web is a local qualification preview; product support remains gated.");
        return new(new(dotnet, arguments, RepositoryRoot, Capture: false, TimeoutSeconds: 1200,
                Environment: environment, CaptureFailureOutput: true),
            Path.Combine(publish, "wwwroot"), sdk.Version, options.DotnetVersion, mode, configuration, execution, buildId);
    }

    private async Task SelectBuildOptionsAsync(Options options, CancellationToken ct)
    {
        if (!ui.IsInteractive) return;
        var select = new Selector(ui);
        if (options.Configuration is null)
        {
            string[] configurations = options.Mode == "MonoAot" ? ["Release"] : ["Release", "Debug"];
            options.Configuration = await select.SelectAsync("Web publish configuration", configurations,
                value => value == "Release" ? "Release (optimized publish)" : "Debug (non-AOT publish)",
                "--configuration / -Configuration", ct, defaultIndex: 0);
        }
        if (options.Mode is null)
        {
            string[] modes = options.Configuration == "Debug" ? ["Mono"] : ["MonoAot", "Mono"];
            options.Mode = await select.SelectAsync("Web compilation", modes,
                value => value == "MonoAot" ? "AOT (Mono WASM AOT; longer publish)" : "Non-AOT (Mono)",
                "--mode / -Mode", ct, defaultIndex: 0);
        }
        if (options.WebExecutionMode is null)
            options.WebExecutionMode = await select.SelectAsync("Web execution mode", new[] { "threaded", "compatible", "auto" },
                value => value switch
                {
                    "threaded" => "threaded (threads, isolation headers)",
                    "compatible" => "compatible (no threads)",
                    _ => "auto (publish both variants)",
                }, "--web-execution-mode / -WebExecutionMode", ct, defaultIndex: 0);
    }

    public static void ValidatePublish(WebPublishPlan plan)
    {
        if (!File.Exists(Path.Combine(plan.WebRoot, "index.html")))
            throw new DeployException($"Publish did not produce index.html in {plan.WebRoot}; preview was not started.");
        var profilePath = Path.Combine(plan.WebRoot, "doroti.runtime-profile.json");
        if (!File.Exists(profilePath)) throw new DeployException("Published runtime profile is missing; preview was not started.");
        using var document = JsonDocument.Parse(File.ReadAllText(profilePath));
        var profile = document.RootElement;
        static string? Text(JsonElement element, string name) => element.ValueKind == JsonValueKind.Object &&
            element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        var valid = Text(profile, "buildId") == plan.BuildId && Text(profile, "runtimeContract") == $"mono-{plan.DotnetVersion}" &&
            profile.TryGetProperty("provenance", out var provenance) &&
            Text(provenance, "sdk") == plan.Sdk && Text(provenance, "targetFramework") == $"net{plan.DotnetVersion}.0" &&
            Text(provenance, "configuration") == plan.Configuration && Text(provenance, "aot") == (plan.Mode == "MonoAot" ? "true" : "false") &&
            Text(provenance, "interpretWidgetTree") == "false";
        if (plan.ExecutionMode == "auto")
            valid = valid && profile.TryGetProperty("variants", out var variants) && variants.ValueKind == JsonValueKind.Object &&
                variants.TryGetProperty("threaded", out _) && variants.TryGetProperty("compatible", out _);
        else
            valid = valid && profile.TryGetProperty("threads", out var threads) &&
                threads.ValueKind == (plan.ExecutionMode == "threaded" ? JsonValueKind.True : JsonValueKind.False);
        if (!valid) throw new DeployException("Published runtime profile does not match the requested generation, SDK, runtime, AOT or execution mode; preview was not started.");
    }

    public async Task RunAsync(Options options, CancellationToken ct)
    {
        options.ValidateForPlatform("web");
        var dotnet = Executables.Resolve(options.DotnetPath);
        if (options.List)
        {
            ui.WriteLine("\nWeb samples");
            foreach (var item in SampleApp.All) ui.WriteLine($"  {item.Key}: {item.WebProject(RepositoryRoot)}");
            ui.WriteLine("\nInstalled stable Web SDKs");
            foreach (var sdk in ParseSdks(await runner.RunAsync(new(dotnet, ["--list-sdks"], RepositoryRoot), ct)))
                ui.WriteLine($"  {sdk.Version}: {sdk.EntryPoint}");
            return;
        }
        var app = await new Selector(ui).SelectAsync("Sample app", SampleApp.All, item => item.Folder, "--app / -App", ct,
            options.App, (item, value) => item.Key.Equals(value, StringComparison.OrdinalIgnoreCase));
        var plan = await BuildPlanAsync(options, app, dotnet, ct);
        if (options.DryRun)
        {
            ui.WriteLine("\nDry run: " + plan.Command.Display);
            ui.WriteLine($"After a successful publish: serve {plan.WebRoot} on 127.0.0.1:{(options.Port is null or 0 ? "<automatic>" : options.Port.Value.ToString())}" +
                (options.NoLaunch ? "." : "; open the default browser.") + " Stop the server with Ctrl+C to exit deploy-helper.");
            return;
        }
        await runner.RunAsync(plan.Command, ct);
        ValidatePublish(plan);
        await server.RunAsync(plan.WebRoot, options.Port ?? 0, plan.ExecutionMode != "compatible", !options.NoLaunch, ct);
    }
}
