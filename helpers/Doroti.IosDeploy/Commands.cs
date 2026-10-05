using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Doroti.IosDeploy;

public sealed class DeployException(string message) : Exception(message);

public record Command(string FileName, IReadOnlyList<string> Arguments, string? WorkingDirectory = null,
    bool Capture = true, int TimeoutSeconds = 60, IReadOnlyDictionary<string, string>? Environment = null)
{
    public string Display => string.Join(" ", new[] { FileName }.Concat(Arguments).Select(Quote));
    private static string Quote(string value) => Regex.IsMatch(value, "^[A-Za-z0-9_./:=+-]+$")
        ? value : "'" + value.Replace("'", "'\\''", StringComparison.Ordinal) + "'";
}

public interface ICommandRunner
{
    Task<string> RunAsync(Command command, CancellationToken cancellationToken = default);
}

public interface IUserInterface
{
    bool IsInteractive { get; }
    void WriteLine(string message);
    Task<string?> ReadLineAsync(CancellationToken cancellationToken);
}

public sealed class ConsoleUi : IUserInterface
{
    public bool IsInteractive => !Console.IsInputRedirected;
    public void WriteLine(string message) => Console.WriteLine(message);
    public Task<string?> ReadLineAsync(CancellationToken cancellationToken) =>
        Task.Run(Console.ReadLine, cancellationToken).WaitAsync(cancellationToken);
}

public sealed class ProcessRunner(IUserInterface ui) : ICommandRunner
{
    public async Task<string> RunAsync(Command command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var info = new ProcessStartInfo(command.FileName)
        {
            UseShellExecute = false, RedirectStandardOutput = command.Capture, RedirectStandardError = command.Capture,
            WorkingDirectory = command.WorkingDirectory ?? System.Environment.CurrentDirectory,
        };
        foreach (var argument in command.Arguments) info.ArgumentList.Add(argument);
        if (command.Environment is not null)
            foreach (var (key, value) in command.Environment) info.Environment[key] = value;
        if (!command.Capture) ui.WriteLine("+ " + command.Display);
        using var process = Process.Start(info) ?? throw new DeployException($"Cannot start {command.FileName}.");
        var output = command.Capture ? process.StandardOutput.ReadToEndAsync() : Task.FromResult("");
        var error = command.Capture ? process.StandardError.ReadToEndAsync() : Task.FromResult("");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(command.TimeoutSeconds));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { /* The process exited during cancellation. */ }
            await process.WaitForExitAsync(CancellationToken.None);
            if (cancellationToken.IsCancellationRequested) throw;
            throw new DeployException($"Command timed out after {command.TimeoutSeconds}s: {command.Display}");
        }
        var stdout = await output;
        var stderr = await error;
        if (process.ExitCode != 0)
            throw new DeployException($"Command failed (exit {process.ExitCode}): {command.Display}\n{(stderr.Length > 0 ? stderr : stdout).Trim()}");
        return stdout;
    }
}

public static class Executables
{
    public static string Resolve(string name)
    {
        if (name.Contains(Path.DirectorySeparatorChar))
        {
            var path = Path.GetFullPath(name);
            return File.Exists(path) ? path : throw new DeployException($"Executable not found: {path}");
        }
        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            var path = Path.GetFullPath(Path.Combine(directory, name));
            if (File.Exists(path)) return path;
        }
        throw new DeployException($"Required executable not found: {name}");
    }
}
