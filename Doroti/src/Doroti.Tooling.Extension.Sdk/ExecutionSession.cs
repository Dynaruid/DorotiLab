using System.Diagnostics;
using Doroti.Tooling.Contracts;

namespace Doroti.Tooling.Extension.Sdk;

/// <summary>Owns only processes it starts. Closing a tool connection never stops this session.</summary>
public sealed class ExecutionSession : IAsyncDisposable
{
    private readonly SemaphoreSlim _serial = new(1);
    private readonly object _gate = new();
    private Process? _process;
    private CancellationTokenSource? _runLifetime;
    private bool _disposed;
    public int? ProcessId { get { lock (_gate) return _process?.Id; } }
    public async Task<int> ExecuteAsync(ExecutionPlan plan, Action<string>? output = null, CancellationToken token = default)
    {
        await _serial.WaitAsync(token);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(token);
            lock (_gate) _runLifetime = lifetime;
            try
            {
                foreach (var step in plan.Steps)
                {
                    lifetime.Token.ThrowIfCancellationRequested();
                    if (step.StopSignal is { } signal && (!Path.IsPathFullyQualified(signal.Path) ||
                        string.IsNullOrWhiteSpace(signal.SessionId) || signal.TimeoutSeconds is < 1 or > 60))
                        throw new ToolContractException("invalid-stop-signal", "Stop requires an absolute path, session identity and a bounded timeout.");
                    var start = new ProcessStartInfo(step.Executable) { WorkingDirectory = Path.GetFullPath(step.WorkingDirectory), UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
                    foreach (var argument in step.Arguments) start.ArgumentList.Add(argument);
                    foreach (var item in step.Environment) start.Environment[item.Name] = item.Value;
                    using var process = Process.Start(start) ?? throw new ToolContractException("spawn-failed", step.Executable);
                    lock (_gate) _process = process;
                    var readers = new[] { ProcessOutput.ReadLinesAsync(process.StandardOutput, output), ProcessOutput.ReadLinesAsync(process.StandardError, output) };
                    Exception? failure = null;
                    try { await ProcessOutput.WaitAsync(process, readers, lifetime.Token); }
                    catch (Exception error) { failure = error; throw; }
                    finally
                    {
                        if (!process.HasExited && step.StopSignal is { } graceful)
                        {
                            try
                            {
                                // Give device/process adapters time to release their owned resources.
                                // The signal is an external file boundary; it contains no live .NET objects.
                                Directory.CreateDirectory(Path.GetDirectoryName(graceful.Path)!);
                                await File.WriteAllTextAsync(graceful.Path,
                                    "{\"sessionId\":" + System.Text.Json.JsonSerializer.Serialize(graceful.SessionId,
                                        StopSignalJsonContext.Default.String) + "}");
                                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(graceful.TimeoutSeconds));
                            }
                            catch (Exception error) when (error is IOException or UnauthorizedAccessException or TimeoutException)
                            { output?.Invoke("Graceful Stop did not complete: " + error.Message); }
                        }
                        if (!process.HasExited) { process.Kill(entireProcessTree: true); await process.WaitForExitAsync(); }
                        try { await Task.WhenAll(readers); }
                        catch when (failure is not null) { /* Preserve the original cancellation/output error. */ }
                        finally { lock (_gate) _process = null; }
                    }
                    if (process.ExitCode != 0) return process.ExitCode;
                }
                return 0;
            }
            finally { lock (_gate) _runLifetime = null; }
        }
        finally { _serial.Release(); }
    }
    public async Task StopAsync()
    {
        lock (_gate) _runLifetime?.Cancel();
        await _serial.WaitAsync();
        _serial.Release();
    }
    public async ValueTask DisposeAsync()
    {
        lock (_gate) _disposed = true;
        await StopAsync();
    }
}

[System.Text.Json.Serialization.JsonSerializable(typeof(string))]
internal partial class StopSignalJsonContext : System.Text.Json.Serialization.JsonSerializerContext;
