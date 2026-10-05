using System.Diagnostics;
using Doroti.Tooling.Contracts;
using Doroti.Tooling.Extension.Sdk;

if (args.Contains("--process-server"))
    await ToolProcessServer.RunAsync(new TestHeadless.Extension(), Console.OpenStandardInput(), Console.OpenStandardOutput());
else if (args.Contains("--crash-server")) Environment.Exit(27);
else if (args.Contains("--hung-server")) await Task.Delay(Timeout.InfiniteTimeSpan);
else if (args.Contains("--child"))
{
    Console.WriteLine("child-started");
    if (args.Contains("--graceful"))
    {
        var path = args[Array.IndexOf(args, "--graceful") + 1];
        while (!File.Exists(path)) await Task.Delay(20);
        var value = System.Text.Json.JsonDocument.Parse(await File.ReadAllTextAsync(path));
        if (value.RootElement.GetProperty("sessionId").GetString() != "graceful-session") throw new InvalidOperationException("Wrong Stop owner.");
        await File.WriteAllTextAsync(path + ".cleaned", "cleanup-complete");
        return;
    }
    if (args.Contains("--large-output")) { Console.Write(new string('x', 100000)); Console.Out.Flush(); }
    if (args.Contains("--wait")) await Task.Delay(Timeout.InfiniteTimeSpan);
}

namespace TestHeadless
{
    public sealed class Extension : IDorotiToolExtension
    {
        public static int Activations;
        public static int Disposals;
        public Extension() => Interlocked.Increment(ref Activations);
        private bool _disposed;
        private void Check(CancellationToken token) { ObjectDisposedException.ThrowIf(_disposed, this); token.ThrowIfCancellationRequested(); }
        public ValueTask<ToolIdentity> GetCapabilitiesAsync(CancellationToken token)
        {
            Check(token);
            return ValueTask.FromResult(new ToolIdentity("test-headless", "0.4.0-alpha.1", 1, "[0.4.0-alpha.1,0.5.0)", "net10.0", ["windows", "linux", "macos"], Enum.GetValues<ToolService>()));
        }
        public ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken token)
        {
            Check(token);
            return ValueTask.FromResult(new ToolConfiguration([new("delay", OptionKind.Integer, "0", false, [], 0, 30000), new("trace", OptionKind.Boolean, "true", false, [])]));
        }
        public async ValueTask<ConfigurationResult> ConfigureAsync(ConfigurationRequest request, CancellationToken token)
        {
            var configuration = await GetConfigurationAsync(request.Context, token);
            ToolContract.ValidateConfiguration(configuration, request.Values);
            if (request.Values.SingleOrDefault(x => x.Name == "delay") is { } delay) await Task.Delay(int.Parse(delay.Value), token);
            return new(request.Values);
        }
        public async ValueTask<DiagnosticResult> DiagnoseAsync(ToolContext context, CancellationToken token)
        {
            Check(token);
            using var process = Process.Start(new ProcessStartInfo("dotnet") { ArgumentList = { "--version" }, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true })!;
            try
            {
                var stdout = process.StandardOutput.ReadToEndAsync(token);
                var stderr = process.StandardError.ReadToEndAsync(token);
                await process.WaitForExitAsync(token);
                return new([new("dotnet-sdk", process.ExitCode == 0 ? DiagnosticStatus.Pass : DiagnosticStatus.Fail, (await stdout).Trim() + (await stderr).Trim(), "actual SDK probe; headless only")]);
            }
            finally { if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); } }
        }
        public ValueTask<DeviceResult> GetDevicesAsync(ToolContext context, CancellationToken token)
        { Check(token); return ValueTask.FromResult(new DeviceResult([new("host-process", "Headless host process", ["test-headless"], ["default"])])); }
        public ValueTask<TemplateResult> GetTemplatesAsync(ToolContext context, CancellationToken token)
        { Check(token); return ValueTask.FromResult(new TemplateResult([new("headless-app", "0.4.0-alpha.1", ["widgets"], [])])); }
        public ValueTask<TemplateGeneration> GenerateAsync(TemplateRequest request, CancellationToken token)
        {
            Check(token);
            if (request.TemplateId != "headless-app" || request.Design != "widgets") throw new ToolContractException("unsupported-template", request.TemplateId);
            return ValueTask.FromResult(new TemplateGeneration([
                new("Generated.csproj", "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup><ItemGroup><PackageReference Include=\"Doroti.TestProvider.Runtime\" Version=\"0.4.0-alpha.1\"/></ItemGroup></Project>"),
                new("Program.cs", "await Doroti.TestProvider.Runtime.HeadlessApplication.RunAsync();") ]));
        }
        public ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token)
        {
            Check(token);
            if (request.Context.DeviceId is not (null or "host-process")) throw new ToolContractException("unsupported-device", request.Context.DeviceId);
            var arguments = request.Operation switch
            {
                "build" => new[] { "build", request.Context.Project, "-c", request.Configuration },
                "run" => new[] { "run", "--project", request.Context.Project, "-c", request.Configuration },
                "publish" => new[] { "publish", request.Context.Project, "-c", request.Configuration },
                "dev" => new[] { "watch", "--project", request.Context.Project, "run" },
                _ => throw new ToolContractException("unsupported-operation", request.Operation),
            };
            return ValueTask.FromResult(new ExecutionPlan(Guid.NewGuid().ToString("N"), [new("dotnet", arguments, request.Context.Workspace, [])]));
        }
        public ValueTask DisposeAsync() { if (!_disposed) { _disposed = true; Interlocked.Increment(ref Disposals); } return ValueTask.CompletedTask; }
    }
}
