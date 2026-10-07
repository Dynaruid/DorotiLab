using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Doroti.Tooling.Contracts;
namespace Doroti.Tooling.Extension.Sdk;

/// <summary>Shared typed .NET operations. Providers supply OS restrictions and actual command plans.</summary>
public abstract class DotnetToolExtension(string provider, IReadOnlyList<string> hostOperatingSystems) : IDorotiToolExtension
{
    private bool _disposed;
    protected void Check(CancellationToken token) { ObjectDisposedException.ThrowIf(_disposed, this); token.ThrowIfCancellationRequested(); }
    protected virtual IReadOnlyList<ToolService> Services => [ToolService.Configuration, ToolService.Diagnostics, ToolService.Devices, ToolService.Operations];
    protected virtual IReadOnlyList<string> RequiredHosts(ToolContext context) => hostOperatingSystems;
    protected virtual void ValidateHost(ToolContext context)
    {
        var current = OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : "linux";
        if (!RequiredHosts(context).Contains(current)) throw new ToolContractException("unsupported-host", $"{context.TargetId}/{context.Profile} requires {string.Join(",", RequiredHosts(context))}.");
    }
    public ValueTask<ToolIdentity> GetCapabilitiesAsync(CancellationToken token)
    {
        Check(token);
        var version = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion.Split('+')[0];
        return ValueTask.FromResult(new ToolIdentity(provider, version, ToolContract.ProtocolVersion, "[0.4.0-alpha.1,0.5.0)", "net10.0", hostOperatingSystems, Services));
    }
    public virtual ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken token)
    {
        Check(token);
        return ValueTask.FromResult(new ToolConfiguration([new("dotnetpath", OptionKind.Text, "dotnet", false, []), new("sessiondirectory", OptionKind.Text, "", false, []), new("sessionid", OptionKind.Text, "", false, []), new("scope", OptionKind.Choice, "full", false, ["managed", "target", "native", "tools", "full"]), new("timeoutseconds", OptionKind.Integer, "15", false, [], 1, 120)]));
    }
    public async ValueTask<ConfigurationResult> ConfigureAsync(ConfigurationRequest request, CancellationToken token)
    {
        var schema = await GetConfigurationAsync(request.Context, token);
        ToolContract.ValidateConfiguration(schema, request.Values);
        return new(request.Values);
    }
    protected static async Task<(int Exit, string Output)> ProbeAsync(string executable, IEnumerable<string> args, CancellationToken token, string? workingDirectory = null, int timeoutSeconds = 15)
    {
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true, WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory };
        foreach (var argument in args) start.ArgumentList.Add(argument);
        using var process = Process.Start(start) ?? throw new ToolContractException("probe-start-failed", executable);
        async Task<string> ReadAsync(StreamReader reader)
        {
            var text = new System.Text.StringBuilder(); var buffer = new char[4096];
            while (await reader.ReadAsync(buffer.AsMemory(), token) is var count && count != 0)
            {
                if (text.Length + count > 1024 * 1024) throw new ToolContractException("probe-output-too-large", executable);
                text.Append(buffer, 0, count);
            }
            return text.ToString();
        }
        var output = ReadAsync(process.StandardOutput); var error = ReadAsync(process.StandardError);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token); timeout.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        Exception? failure = null;
        try { await ProcessOutput.WaitAsync(process, [output, error], timeout.Token); return (process.ExitCode, (await output + "\n" + await error).Trim()); }
        catch (Exception exception) { failure = exception; throw; }
        finally
        {
            if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); }
            try { await Task.WhenAll(output, error); } catch when (failure is not null) { }
        }
    }
    protected virtual IReadOnlyList<string> RequiredWorkloads(ToolContext context) => [];
    protected virtual IReadOnlyList<(string Id, string Executable, IReadOnlyList<string> Arguments)> RequiredTools(ToolContext context) => [];
    protected virtual IReadOnlyList<string> DiagnosticBuildProperties(ToolContext context) => [];
    public virtual async ValueTask<DiagnosticResult> DiagnoseAsync(ToolContext context, CancellationToken token)
    {
        Check(token);
        var schema = await GetConfigurationAsync(context, token);
        ToolContract.ValidateConfiguration(schema, context.Options ?? []);
        var values = schema.Options.ToDictionary(value => value.Name, value => value.Default ?? "");
        foreach (var value in context.Options ?? []) values[value.Name] = value.Value;
        try { ValidateHost(context); }
        catch (ToolContractException error) { return new([new("host", DiagnosticStatus.Skipped, error.Message, "provider host restriction")]); }
        var diagnostics = new List<Diagnostic>();
        var executable = values["dotnetpath"]; var scope = values["scope"];
        var timeout = int.Parse(values["timeoutseconds"], System.Globalization.CultureInfo.InvariantCulture);
        async Task<(int Exit, string Output)?> Probe(string id, string file, IReadOnlyList<string> arguments)
        {
            try { return await ProbeAsync(file, arguments, token, Path.GetDirectoryName(context.Project), timeout); }
            catch (OperationCanceledException) when (!token.IsCancellationRequested) { diagnostics.Add(new(id, DiagnosticStatus.Partial, $"Probe timed out after {timeout} seconds.", scope)); return null; }
            catch (System.ComponentModel.Win32Exception error) { diagnostics.Add(new(id, DiagnosticStatus.Fail, error.Message, scope)); return null; }
        }
        var sdk = await Probe("dotnet-sdk", executable, ["--version"]);
        if (sdk is { } sdkResult)
        {
            var accepted = SdkSelection.Accepts(sdkResult.Output, context.Project, context.TargetFramework ?? "net10.0", out var reason);
            diagnostics.Add(new("dotnet-sdk", sdkResult.Exit == 0 && accepted ? DiagnosticStatus.Pass : DiagnosticStatus.Fail,
                reason, $"Selected executable and runner/global.json SDK for {context.TargetFramework}"));
        }
        if (scope != "managed")
        {
            var properties = new[] { "NETCoreSdkVersion", "TargetFramework", "RuntimeIdentifier" }.Concat(DiagnosticBuildProperties(context)).Distinct().ToArray();
            var arguments = new List<string> { "msbuild", context.Project, "-nologo", "-getProperty:" + string.Join(',', properties) };
            if (context.TargetFramework is { Length: > 0 } tfm) arguments.Add("-p:TargetFramework=" + tfm);
            if (context.RuntimeIdentifier is { Length: > 0 } rid) arguments.Add("-p:RuntimeIdentifier=" + rid);
            var evaluated = await Probe("build-metadata", executable, arguments);
            if (evaluated is { } evaluatedResult)
            {
                if (evaluatedResult.Exit != 0) diagnostics.Add(new("build-metadata", DiagnosticStatus.Partial, "Runner metadata evaluation failed; restore the selected provider/workload first. " + evaluatedResult.Output, scope));
                else
                {
                    using var document = System.Text.Json.JsonDocument.Parse(evaluatedResult.Output);
                    var resolved = document.RootElement.GetProperty("Properties");
                    var expectedFramework = context.TargetFramework ?? ""; var expectedRid = context.RuntimeIdentifier ?? "";
                    var resolvedTfm = resolved.GetProperty("TargetFramework").GetString(); var resolvedRid = resolved.GetProperty("RuntimeIdentifier").GetString();
                    diagnostics.Add(new("build-metadata", resolvedTfm == expectedFramework && resolvedRid == expectedRid ? DiagnosticStatus.Pass : DiagnosticStatus.Fail,
                        $"Evaluated TFM={resolvedTfm}, RID={resolvedRid}, SDK={resolved.GetProperty("NETCoreSdkVersion").GetString()}", "actual runner evaluation without application execution"));
                    foreach (var property in DiagnosticBuildProperties(context))
                        diagnostics.Add(new("property:" + property, resolved.TryGetProperty(property, out var value) && !string.IsNullOrWhiteSpace(value.GetString()) ? DiagnosticStatus.Pass : DiagnosticStatus.Partial,
                            resolved.TryGetProperty(property, out value) ? value.GetString() ?? "" : "not evaluated", "provider build metadata"));
                }
            }
        }
        if (scope is "native" or "tools" or "full")
        {
            var workloads = RequiredWorkloads(context);
            if (workloads.Count != 0 && await Probe("workloads", executable, ["workload", "list"]) is { } workloadResult)
                foreach (var workload in workloads) diagnostics.Add(new("workload:" + workload,
                    workloadResult.Exit == 0 && Regex.IsMatch(workloadResult.Output, "(?m)^\\s*" + Regex.Escape(workload) + "(?:\\s|$)") ? DiagnosticStatus.Pass : DiagnosticStatus.Fail,
                    workloadResult.Output, "provider-required installed workload"));
            foreach (var requirement in RequiredTools(context))
                if (await Probe(requirement.Id, requirement.Executable, requirement.Arguments) is { } result)
                    diagnostics.Add(new(requirement.Id, result.Exit == 0 ? DiagnosticStatus.Pass : DiagnosticStatus.Fail, result.Output, "actual provider tool probe"));
            diagnostics.Add(new("runtime-acceptance", DiagnosticStatus.Partial, "Build tools do not verify display, native execution, signing, physical devices or input.", $"{context.TargetId}/{context.Profile}"));
        }
        return new(diagnostics);
    }
    public virtual ValueTask<DeviceResult> GetDevicesAsync(ToolContext context, CancellationToken token)
    {
        Check(token); ValidateHost(context);
        return ValueTask.FromResult(new DeviceResult([new("host", "Current host", [context.TargetId], [context.Profile])]));
    }
    public virtual ValueTask<TemplateResult> GetTemplatesAsync(ToolContext context, CancellationToken token) => throw new ToolContractException("unsupported-service", "templates");
    public virtual ValueTask<TemplateGeneration> GenerateAsync(TemplateRequest request, CancellationToken token) => throw new ToolContractException("unsupported-service", "templates");
    protected virtual IReadOnlyList<string> Arguments(OperationRequest request) => request.Operation switch
    {
        "build" => ["build", request.Context.Project, "-c", request.Configuration],
        "run" => ["run", "--project", request.Context.Project, "-c", request.Configuration],
        "publish" => ["publish", request.Context.Project, "-c", request.Configuration],
        "dev" => ["watch", "--project", request.Context.Project, "run", "--configuration", "Debug"],
        _ => throw new ToolContractException("unsupported-operation", request.Operation),
    };
    public virtual async ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token)
    {
        Check(token); ValidateHost(request.Context);
        if (request.Operation == "dev" && request.Configuration != "Debug")
            throw new ToolContractException("unsupported-development-profile", "dev requires Debug.");
        var schema = await GetConfigurationAsync(request.Context, token);
        ToolContract.ValidateConfiguration(schema, request.Options);
        var values = schema.Options.ToDictionary(x => x.Name, x => x.Default ?? "", StringComparer.Ordinal);
        foreach (var option in request.Options) values[option.Name] = option.Value;
        if (request.Context.DeviceId is not (null or "host")) throw new ToolContractException("unsupported-device", request.Context.DeviceId);
        var environment = new List<EnvironmentValue>();
        if (values["sessiondirectory"].Length != 0) environment.Add(new("DOROTI_DEV_SESSION", Path.GetFullPath(values["sessiondirectory"])));
        if (values["sessionid"].Length != 0) environment.Add(new("DOROTI_DEV_SESSION_ID", values["sessionid"]));
        environment.Add(new("DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER", "1"));
        environment.Add(new("DOTNET_WATCH_RESTART_ON_RUDE_EDIT", "false"));
        var arguments = Arguments(request).ToList();
        if (request.Context.TargetFramework is { Length: > 0 } framework) arguments.AddRange(["-f", framework]);
        // dev uses the runner's fixed RID, checked by PlatformMetadata before planning.
        // A command-line RID becomes a global graph property in dotnet watch. The
        // RID-free app reference then duplicates shared projects by path + TFM.
        if (request.Operation != "dev" && request.Context.RuntimeIdentifier is { Length: > 0 } rid) arguments.AddRange(["-r", rid]);
        return new(Guid.NewGuid().ToString("N"), [new(values["dotnetpath"], arguments, Path.GetDirectoryName(request.Context.Project)!, environment)]);
    }
    public ValueTask DisposeAsync() { _disposed = true; return ValueTask.CompletedTask; }
}
