using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;

namespace Doroti.Tooling;

public sealed record DevelopmentSupport(string Platform, string Runner, IReadOnlyList<string> Operations, string Provider, string Version, ToolMode ToolMode, string Backend, string Profile, DevelopmentCapability Development, string Discovery = "resolved");
public sealed record CliWorkspace(string SchemaVersion, string Manifest, string Root, string ApplicationProject, IReadOnlyDictionary<string, string> Platforms, IReadOnlyList<string> DevelopmentTargets, IReadOnlyList<DevelopmentSupport> DevelopmentSupport);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true)]
[JsonSerializable(typeof(CliWorkspace))]
[JsonSerializable(typeof(DiagnosticResult))]
[JsonSerializable(typeof(DeviceResult))]
[JsonSerializable(typeof(ToolConfiguration))]
[JsonSerializable(typeof(ConfigurationResult))]
[JsonSerializable(typeof(TemplateResult))]
internal partial class CliJsonContext : JsonSerializerContext;

public static class ProviderCli
{
    private static string Inside(string root, string relative)
    {
        if (string.IsNullOrWhiteSpace(relative) || Path.IsPathRooted(relative)) throw new ToolContractException("invalid-project-path", "Projects must use workspace-relative paths.");
        var full = Path.GetFullPath(relative, root);
        var traversal = Path.GetRelativePath(root, full);
        if (traversal == ".." || traversal.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) || !File.Exists(full)) throw new ToolContractException("invalid-project-path", relative);
        return full;
    }
    private static T Read<T>(string path, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> type) where T : class
    {
        var bytes = File.ReadAllBytes(path);
        ToolFrames.ValidateJson(bytes);
        return JsonSerializer.Deserialize(bytes, type) ?? throw new ToolContractException("empty-manifest", path);
    }
    public static async Task<int> RunAsync(string[] args)
    {
        try
        {
            if (args.Length == 0) throw new ToolContractException("missing-command", "Expected describe, doctor, devices, configuration, templates, generate, build, run, publish or dev.");
            var command = args[0];
            var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var index = 1; index < args.Length; index++)
            {
                if (!args[index].StartsWith('-') || index + 1 == args.Length) throw new ToolContractException("invalid-arguments", args[index]);
                if (!options.TryAdd(args[index].TrimStart('-'), args[++index])) throw new ToolContractException("duplicate-argument", args[index - 1]);
            }
            string Option(string name, string? fallback = null) => options.GetValueOrDefault(name) ?? fallback ?? throw new ToolContractException("missing-argument", name);
            var input = Path.GetFullPath(Option("app"));
            var manifest = Directory.Exists(input) ? Path.Combine(input, "doroti-workspace.json") : Path.GetFileName(input) == "doroti-workspace.json" ? input : Path.Combine(Path.GetDirectoryName(input)!, "doroti-workspace.json");
            var root = Path.GetDirectoryName(manifest)!;
            var workspace = Read(manifest, ToolWireJson.Context.WorkspaceDescriptor);
            if (workspace.SchemaVersion != "doroti.workspace/v2" || workspace.Platforms is null || workspace.Platforms.Count == 0) throw new ToolContractException("unsupported-schema", "Expected doroti.workspace/v2 with explicit platform entries.");
            var application = Inside(root, workspace.ApplicationProject);
            var runners = new Dictionary<string, string>(StringComparer.Ordinal);
            var providers = new Dictionary<string, (ProviderDescriptor Descriptor, string Directory)>(StringComparer.Ordinal);
            var unresolved = new HashSet<string>(StringComparer.Ordinal);
            var selected = command == "describe" ? options.GetValueOrDefault("platform", "all") : Option("platform");
            foreach (var (alias, platform) in workspace.Platforms)
            {
                ToolContract.ValidateAlias(alias);
                runners.Add(alias, Inside(root, platform.Runner));
                if (string.IsNullOrWhiteSpace(platform.ProviderManifest) || string.IsNullOrWhiteSpace(platform.TargetPackage) || string.IsNullOrWhiteSpace(platform.TargetFramework) || string.IsNullOrWhiteSpace(platform.Backend) || string.IsNullOrWhiteSpace(platform.Profile)) throw new ToolContractException("incomplete-platform", alias);
                if (selected != "all" && selected != alias) continue;
                string providerManifest;
                try { providerManifest = PlatformMetadata.ProviderManifest(runners[alias], platform.ProviderManifest, root); }
                catch (ToolContractException error) when (command == "describe" && error.Code is "restore-required" or "unresolved-provider-package")
                { unresolved.Add(alias); continue; }
                var provider = Read(providerManifest, ToolWireJson.Context.ProviderDescriptor);
                if (provider.SchemaVersion != "doroti.platform-provider/v1" || provider.Id != platform.Provider || provider.ProtocolVersion != ToolContract.ProtocolVersion || provider.Services is null || provider.Operations is null || provider.Tool is null || provider.Services.Any(x => !Enum.IsDefined(x)) || provider.Services.Distinct().Count() != provider.Services.Count || !Enum.IsDefined(provider.Tool.Mode)) throw new ToolContractException("invalid-provider", alias);
                if (!ToolVersionRange.Contains(provider.CoreRange, ToolContract.HostCoreVersion)) throw new ToolContractException("unsupported-core", alias);
                if (provider.Profiles is { Count: > 0 } && !provider.Profiles.ContainsKey(platform.Profile)) throw new ToolContractException("undeclared-profile", platform.Profile);
                PlatformMetadata.ValidateResolvedTarget(runners[alias], platform, provider);
                providers.Add(alias, (provider, Path.GetDirectoryName(providerManifest)!));
            }
            if (selected != "all" && !workspace.Platforms.ContainsKey(selected)) throw new ToolContractException("undeclared-platform", selected);
            if (command == "describe")
            {
                var support = workspace.Platforms.Where(pair => selected == "all" || selected == pair.Key).Select(pair =>
                {
                    if (unresolved.Contains(pair.Key)) return new DevelopmentSupport(pair.Key, runners[pair.Key], [], pair.Value.Provider, "unresolved", ToolMode.DotnetInproc,
                        pair.Value.Backend, pair.Value.Profile, new("none", false, false, false, false), "restore-required");
                    var descriptor = providers[pair.Key].Descriptor;
                    var profile = descriptor.Profiles?.GetValueOrDefault(pair.Value.Profile);
                    var host = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : "linux";
                    var operations = profile is not null && !profile.HostOperatingSystems.Contains(host) ? Array.Empty<string>() : profile?.Operations ?? descriptor.Operations;
                    return new DevelopmentSupport(pair.Key, runners[pair.Key], operations, pair.Value.Provider, descriptor.Version, descriptor.Tool.Mode, pair.Value.Backend, pair.Value.Profile, profile?.Development ?? new("none", false, false, false, false));
                }).ToArray();
                Console.WriteLine(JsonSerializer.Serialize(new CliWorkspace("doroti.cli-workspace/v2", manifest, root, application, runners, support.Where(x => x.Operations.Contains("dev")).Select(x => x.Platform).ToArray(), support), CliJsonContext.Default.CliWorkspace));
                return 0;
            }
            var aliases = selected == "all" ? workspace.Platforms.Keys.ToArray() : new[] { selected };
            using var cancel = new CancellationTokenSource();
            ConsoleCancelEventHandler stop = (_, eventArgs) => { eventArgs.Cancel = true; cancel.Cancel(); };
            Console.CancelKeyPress += stop;
            try
            {
                var doctorExit = 0;
                foreach (var alias in aliases)
                {
                    if (!workspace.Platforms.TryGetValue(alias, out var platform)) throw new ToolContractException("undeclared-platform", alias);
                    var (provider, directory) = providers[alias];
                    var selectedProfile = provider.Profiles?.GetValueOrDefault(platform.Profile);
                    var currentHost = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : "linux";
                    if (!(selectedProfile?.HostOperatingSystems ?? provider.HostOperatingSystems).Contains(currentHost))
                    {
                        if (command == "doctor") { Console.WriteLine(JsonSerializer.Serialize(new DiagnosticResult([new("host", DiagnosticStatus.Skipped, $"{alias}/{platform.Profile} is unavailable on {currentHost}.", "provider host restriction")]), CliJsonContext.Default.DiagnosticResult)); continue; }
                        throw new ToolContractException("unsupported-host", alias);
                    }
                    if (selectedProfile is not null && command is "build" or "run" or "publish" or "dev" && !selectedProfile.Operations.Contains(command)) throw new ToolContractException("unsupported-operation", $"{alias}/{platform.Profile}/{command}");
                    if (command is "build" or "run" or "publish" or "dev")
                        await PlatformMetadata.ValidateRunnerAsync(Option("dotnetpath", "dotnet"), runners[alias], platform, provider, cancel.Token);
                    ToolAssemblyLoader? loader = null;
                    ToolExtensionLifetime? processLifetime = null;
                    try
                    {
                        ToolExtensionLifetime lifetime;
                        if (provider.Tool.Mode == ToolMode.DotnetInproc)
                        {
                            var assemblyPath = Path.GetFullPath(provider.Tool.Assembly, directory);
                            if (provider.Tool.Project is { } toolProject)
                            {
                                var project = Path.GetFullPath(toolProject, directory);
                                await using var preparation = new ExecutionSession();
                                using var preparationTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancel.Token);
                                preparationTimeout.CancelAfter(TimeSpan.FromMinutes(20));
                                var exit = await preparation.ExecuteAsync(new("prepare-tool", [new(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet", ["build", project, "-c", "Release", "--nologo"], directory, [])]), Console.Error.WriteLine, preparationTimeout.Token);
                                if (exit != 0) throw new ToolContractException("tool-build-failed", alias);
                            }
                            if (!File.Exists(assemblyPath)) throw new ToolContractException("provider-not-installed", assemblyPath);
                            loader = new(provider, assemblyPath);
                            lifetime = loader.Lifetime;
                        }
                        else
                        {
                            var executable = provider.Tool.Executable ?? throw new ToolContractException("missing-process-entry", alias);
                            var entry = new ProcessStep(executable, provider.Tool.Arguments.Select(arg => arg.Replace("${providerDirectory}", directory, StringComparison.Ordinal)).ToArray(), directory, []);
                            processLifetime = new(provider, _ => ValueTask.FromResult<IDorotiToolExtension>(new ProcessToolExtension(entry, log: Console.Error.WriteLine)));
                            lifetime = processLifetime;
                        }
                        var context = new ToolContext(root, runners[alias], platform.Provider, platform.Target, platform.Profile, options.GetValueOrDefault("device"), 1, platform.TargetFramework, platform.RuntimeIdentifier, platform.Backend, Path.Combine(directory, "doroti-provider.json"), options.Where(x => !new[] { "app", "platform", "configuration", "device", "output", "template", "design" }.Contains(x.Key, StringComparer.OrdinalIgnoreCase)).Select(x => new OptionValue(x.Key.ToLowerInvariant(), x.Value)).ToArray());
                        switch (command)
                        {
                            case "doctor":
                                var diagnostics = await lifetime.InvokeAsync(ToolService.Diagnostics, 1, (tool, token) => tool.DiagnoseAsync(context, token), cancel.Token);
                                Console.WriteLine(JsonSerializer.Serialize(diagnostics, CliJsonContext.Default.DiagnosticResult));
                                if (diagnostics.Diagnostics.Any(x => x.Status == DiagnosticStatus.Fail)) doctorExit = 1;
                                else if (doctorExit == 0 && diagnostics.Diagnostics.Any(x => x.Status == DiagnosticStatus.Partial)) doctorExit = 2;
                                break;
                            case "devices":
                                Console.WriteLine(JsonSerializer.Serialize(await lifetime.InvokeAsync(ToolService.Devices, 1, (tool, token) => tool.GetDevicesAsync(context, token), cancel.Token), CliJsonContext.Default.DeviceResult)); break;
                            case "configure":
                                var configurationValues = options.Where(x => !new[] { "app", "platform" }.Contains(x.Key, StringComparer.OrdinalIgnoreCase)).Select(x => new OptionValue(x.Key.ToLowerInvariant(), x.Value)).ToArray();
                                Console.WriteLine(JsonSerializer.Serialize(await lifetime.InvokeAsync(ToolService.Configuration, 1, (tool, token) => tool.ConfigureAsync(new(context, configurationValues), token), cancel.Token), CliJsonContext.Default.ConfigurationResult)); break;
                            case "configuration":
                                Console.WriteLine(JsonSerializer.Serialize(await lifetime.InvokeAsync(ToolService.Configuration, 1, (tool, token) => tool.GetConfigurationAsync(context, token), cancel.Token), CliJsonContext.Default.ToolConfiguration)); break;
                            case "templates":
                                Console.WriteLine(JsonSerializer.Serialize(await lifetime.InvokeAsync(ToolService.Templates, 1, (tool, token) => tool.GetTemplatesAsync(context, token), cancel.Token), CliJsonContext.Default.TemplateResult)); break;
                            case "generate":
                                var destination = Path.GetFullPath(Option("output"));
                                var generated = await lifetime.InvokeAsync(ToolService.Templates, 1, (tool, token) => tool.GenerateAsync(new(context, Option("template"), Option("design", "widgets"), destination, []), token), cancel.Token);
                                // Validate the entire result before creating any files.
                                var files = new Dictionary<string, string>(OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
                                foreach (var file in generated.Files)
                                {
                                    var target = Path.GetFullPath(file.RelativePath, destination);
                                    var relative = Path.GetRelativePath(destination, target);
                                    if (Path.IsPathRooted(file.RelativePath) || relative == ".." || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal) || File.Exists(target) || !files.TryAdd(target, file.Content)) throw new ToolContractException("invalid-template-output", file.RelativePath);
                                }
                                foreach (var (file, content) in files) { Directory.CreateDirectory(Path.GetDirectoryName(file)!); await File.WriteAllTextAsync(file, content, cancel.Token); }
                                break;
                            default:
                                if (!provider.Operations.Contains(command)) throw new ToolContractException("unsupported-operation", command);
                                if (context.DeviceId is not null)
                                {
                                    var devices = await lifetime.InvokeAsync(ToolService.Devices, 1, (tool, token) => tool.GetDevicesAsync(context, token), cancel.Token);
                                    if (!devices.Devices.Any(x => x.Id == context.DeviceId && x.Targets.Contains(platform.Target) && x.Profiles.Contains(platform.Profile))) throw new ToolContractException("unsupported-device", context.DeviceId);
                                }
                                var plan = await lifetime.InvokeAsync(ToolService.Operations, 1, (tool, token) => tool.PlanAsync(new(context, command, Option("configuration", command == "dev" ? "Debug" : "Release"), options.Where(x => !new[] { "app", "platform", "configuration", "device" }.Contains(x.Key, StringComparer.OrdinalIgnoreCase)).Select(x => new OptionValue(x.Key.ToLowerInvariant(), x.Value)).ToArray()), token), cancel.Token);
                                // The execution session survives tool cleanup; the extension never owns the app process.
                                if (loader is not null) { await loader.DisposeAsync(); loader = null; }
                                if (processLifetime is not null) { await processLifetime.DisposeAsync(); processLifetime = null; }
                                await using (var session = new ExecutionSession()) { var exit = await session.ExecuteAsync(plan, Console.WriteLine, cancel.Token); if (exit != 0) return exit; }
                                break;
                        }
                    }
                    finally
                    {
                        if (loader is not null) await loader.DisposeAsync();
                        if (processLifetime is not null) await processLifetime.DisposeAsync();
                    }
                }
                return doctorExit;
            }
            finally { Console.CancelKeyPress -= stop; }
        }
        catch (OperationCanceledException) { Console.Error.WriteLine("canceled: operation stopped."); return 130; }
        catch (Exception error) { Console.Error.WriteLine($"{(error is ToolContractException contract ? contract.Code : "tool-failed")}: {error.Message}"); return 1; }
    }
}
