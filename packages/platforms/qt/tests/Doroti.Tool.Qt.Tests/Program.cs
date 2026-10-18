using Doroti.Tooling.Contracts;
using Doroti.Tool.Qt;
static void Check(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
var context = new ToolContext(Environment.CurrentDirectory, Path.Combine(Environment.CurrentDirectory, "fixture.csproj"), "qt", "Linux", "default:linux-x64", "host", 1, "net10.0", "linux-x64");
var options = new OptionValue[] { new("dotnetpath", "/tmp/한글 도구/dotnet"), new("sessionid", "linux-profiles"), new("sessiondirectory", Path.Combine(Environment.CurrentDirectory, "session")) };
await using var extension = new Extension();
var original = Environment.GetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER");
try
{
    foreach (var polling in new string?[] { null, "false" })
    {
        Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", polling);
        var plan = await extension.PlanAsync(new(context, "dev", "Debug", options), default);
        var step = plan.Steps.Single();
        Check(step.Executable == options[0].Value, "Executable path changed.");
        Check(step.Arguments.Contains("--property:DorotiQtDevelopment=true") && step.Arguments.Contains("--no-launch-profile"), "Qt development profile was omitted.");
        Check(step.Environment.Single(value => value.Name == "DOTNET_USE_POLLING_FILE_WATCHER").Value == (polling ?? "1"), "Polling preference changed.");
        Check(step.Environment.Single(value => value.Name == "DOTNET_WATCH_RESTART_ON_RUDE_EDIT").Value == "false", "Automatic rude-edit restart was enabled.");
        Check(step.Environment.Single(value => value.Name == "DOROTI_DEV_SESSION_ID").Value == "linux-profiles", "Session identity changed.");
    }
    try { await extension.PlanAsync(new(context, "dev", "Release", options), default); throw new InvalidOperationException("Release development was admitted."); }
    catch (ToolContractException error) when (error.Code == "unsupported-development-profile") { }
}
finally { Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", original); }
var schema = await extension.GetConfigurationAsync(context, default);
Check(schema.Options.Single(value => value.Name == "packageformat").Choices.SequenceEqual(["directory", "deb"]), "Package format schema changed.");
var directory = await extension.PlanAsync(new(context, "publish", "Release", options), default);
Check(directory.Steps.Single().Executable == options[0].Value && directory.Steps.Single().Arguments.SequenceEqual(new[] { "publish", context.Project, "-c", "Release", "-f", "net10.0", "-r", "linux-x64" }), "Default directory publish changed.");
var destination = Path.Combine(Path.GetTempPath(), "한글 패키지 출력");
var custom = await extension.PlanAsync(new(context, "publish", "Release", [..options, new("output", destination)]), default);
Check(custom.Steps.Single().Arguments.TakeLast(2).SequenceEqual(["--output", destination]), "Directory output path lost argument boundaries.");
async Task Reject(OperationRequest request, string code)
{
    try { await extension.PlanAsync(request, default); throw new InvalidOperationException("Invalid packaging request was admitted: " + code); }
    catch (ToolContractException error) when (error.Code == code) { }
}
await Reject(new(context, "build", "Release", [new("packageformat", "deb")]), "publish-option-only");
await Reject(new(context, "run", "Release", [new("output", destination)]), "publish-option-only");
await Reject(new(context, "publish", "Debug", [new("packageformat", "deb")]), "unsupported-deb-profile");
await Reject(new(context with { RuntimeIdentifier = "linux-arm64" }, "publish", "Release", [new("packageformat", "deb")]), "unsupported-deb-profile");
await Reject(new(context with { TargetFramework = "net11.0" }, "publish", "Release", [new("packageformat", "deb")]), "unsupported-deb-profile");
await Reject(new(context, "publish", "Release", [new("packageformat", "rpm")]), "invalid-option");
using var canceled = new CancellationTokenSource(); canceled.Cancel();
try { await extension.PlanAsync(new(context, "publish", "Release", [new("packageformat", "deb")]), canceled.Token); throw new InvalidOperationException("Canceled plan was admitted."); }
catch (OperationCanceledException) { }
Console.WriteLine("PASS: typed Qt development plan, executable/session identity, polling override, no rude-edit restart and Release rejection.");
Console.WriteLine("PASS: default/custom directory publish, packaging schema, publish-only options, RID/TFM/Release rejection and cancellation.");
