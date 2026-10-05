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
Console.WriteLine("PASS: typed Qt development plan, executable/session identity, polling override, no rude-edit restart and Release rejection.");
