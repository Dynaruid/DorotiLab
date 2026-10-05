using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Doroti.Tooling.Contracts;

namespace Doroti.Tooling.Extension.Sdk;

public sealed class ProcessToolExtension : IDorotiToolExtension
{
    private readonly Process _process;
    private readonly SemaphoreSlim _writer = new(1);
    private readonly ConcurrentDictionary<string, TaskCompletionSource<WireResult>> _pending = new(StringComparer.Ordinal);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Task _reader;
    private readonly Task _stderr;
    private readonly TimeSpan _timeout;
    private readonly SemaphoreSlim _capacity = new(128);
    private long _nextId;
    private int _stopping;
    public ProcessToolExtension(ProcessStep entry, TimeSpan? timeout = null, Action<string>? log = null)
    {
        _timeout = timeout ?? TimeSpan.FromSeconds(30);
        var start = new ProcessStartInfo(entry.Executable) { WorkingDirectory = entry.WorkingDirectory, UseShellExecute = false, RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (var arg in entry.Arguments) start.ArgumentList.Add(arg);
        foreach (var item in entry.Environment) start.Environment[item.Name] = item.Value;
        _process = Process.Start(start) ?? throw new ToolContractException("spawn-failed", "Tool process did not start.");
        _reader = ReadResponsesAsync();
        _stderr = ProcessOutput.ReadLinesAsync(_process.StandardError, log);
        _ = _stderr.ContinueWith(_ => _lifetime.Cancel(), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }
    private async Task ReadResponsesAsync()
    {
        Exception failure = new EndOfStreamException("The tool extension exited.");
        try
        {
            while (await ToolFrames.ReadAsync(_process.StandardOutput.BaseStream, _lifetime.Token) is { } payload)
            {
                ToolFrames.ValidateJson(payload);
                var response = JsonSerializer.Deserialize(payload, ToolWireJson.Context.WireResponse) ?? throw new InvalidDataException("Null response.");
                if (response.Jsonrpc != "2.0" || string.IsNullOrEmpty(response.Id) || (response.Result is null) == (response.Error is null)) throw new InvalidDataException("Invalid tool response.");
                if (!_pending.TryRemove(response.Id, out var completion)) continue; // canceled/previous generation response
                if (response.Error is { } error) completion.TrySetException(new ToolContractException(error.Code, error.Message));
                else completion.TrySetResult(response.Result!);
            }
        }
        catch (Exception error) { failure = error; }
        finally { foreach (var id in _pending.Keys) if (_pending.TryRemove(id, out var item)) item.TrySetException(failure); }
    }
    private async ValueTask<WireResult> CallAsync(string method, WireArguments arguments, CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _stopping) != 0 && method != "extension.shutdown", this);
        if (_reader.IsCompleted) throw new ToolContractException("connection-closed", "Tool connection is closed.");
        var id = Interlocked.Increment(ref _nextId).ToString(System.Globalization.CultureInfo.InvariantCulture);
        var completion = new TaskCompletionSource<WireResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        await _capacity.WaitAsync(token);
        _pending[id] = completion;
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, _lifetime.Token);
        linked.CancelAfter(_timeout);
        try
        {
            await SendAsync(new("2.0", id, method, arguments), linked.Token);
            return await completion.Task.WaitAsync(linked.Token);
        }
        catch (OperationCanceledException)
        {
            if (!_lifetime.IsCancellationRequested && !_process.HasExited)
            {
                using var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                try { await SendAsync(new("2.0", null, "$/cancelRequest", new(RequestId: id)), cancel.Token); } catch { }
            }
            throw;
        }
        finally { _pending.TryRemove(id, out _); _capacity.Release(); }
    }
    private ValueTask SendAsync(WireRequest request, CancellationToken token) => ToolFrames.WriteAsync(_process.StandardInput.BaseStream, JsonSerializer.SerializeToUtf8Bytes(request, ToolWireJson.Context.WireRequest), _writer, token);
    private static T Required<T>(T? value) where T : class => value ?? throw new ToolContractException("invalid-result", "The response omitted its required typed result.");
    public async ValueTask<ToolIdentity> GetCapabilitiesAsync(CancellationToken token) => Required((await CallAsync("extension.getCapabilities", new(), token)).Identity);
    public async ValueTask<ToolConfiguration> GetConfigurationAsync(ToolContext context, CancellationToken token) => Required((await CallAsync("configuration.describe", new(Context: context), token)).Configuration);
    public async ValueTask<ConfigurationResult> ConfigureAsync(ConfigurationRequest request, CancellationToken token) => Required((await CallAsync("configuration.evaluate", new(Configuration: request), token)).Configured);
    public async ValueTask<DiagnosticResult> DiagnoseAsync(ToolContext context, CancellationToken token) => Required((await CallAsync("diagnostics.probe", new(Context: context), token)).Diagnostics);
    public async ValueTask<DeviceResult> GetDevicesAsync(ToolContext context, CancellationToken token) => Required((await CallAsync("device.list", new(Context: context), token)).Devices);
    public async ValueTask<TemplateResult> GetTemplatesAsync(ToolContext context, CancellationToken token) => Required((await CallAsync("templates.list", new(Context: context), token)).Templates);
    public async ValueTask<TemplateGeneration> GenerateAsync(TemplateRequest request, CancellationToken token) => Required((await CallAsync("templates.generate", new(Template: request), token)).Generated);
    public async ValueTask<ExecutionPlan> PlanAsync(OperationRequest request, CancellationToken token) => Required((await CallAsync("operations.plan", new(Operation: request), token)).Plan);
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _stopping, 1) != 0) return;
        try { await CallAsync("extension.shutdown", new(), CancellationToken.None); } catch { }
        try { await _process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(3)); }
        catch (TimeoutException) { if (!_process.HasExited) _process.Kill(entireProcessTree: true); await _process.WaitForExitAsync(); }
        _lifetime.Cancel();
        try { await Task.WhenAll(_reader, _stderr); }
        finally { _process.Dispose(); _writer.Dispose(); _capacity.Dispose(); _lifetime.Dispose(); }
    }
}

public static class ToolProcessServer
{
    public static async Task RunAsync(IDorotiToolExtension extension, Stream input, Stream output, CancellationToken token = default)
    {
        using var writer = new SemaphoreSlim(1);
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(token);
        var invocations = new ConcurrentDictionary<string, (CancellationTokenSource Cancel, Task Work)>();
        Task? shutdown = null;
        async Task HandleAsync(WireRequest request, CancellationToken cancellation)
        {
            WireResponse response;
            try
            {
                var args = request.Params;
                ToolContext Context() => args.Context ?? throw new ToolContractException("invalid-params", "Context is required.");
                var result = request.Method switch
                {
                    "extension.getCapabilities" => new WireResult(Identity: await extension.GetCapabilitiesAsync(cancellation)),
                    "configuration.describe" => new WireResult(Configuration: await extension.GetConfigurationAsync(Context(), cancellation)),
                    "configuration.evaluate" => new WireResult(Configured: await extension.ConfigureAsync(args.Configuration ?? throw new ToolContractException("invalid-params", "Configuration required."), cancellation)),
                    "diagnostics.probe" => new WireResult(Diagnostics: await extension.DiagnoseAsync(Context(), cancellation)),
                    "device.list" => new WireResult(Devices: await extension.GetDevicesAsync(Context(), cancellation)),
                    "templates.list" => new WireResult(Templates: await extension.GetTemplatesAsync(Context(), cancellation)),
                    "templates.generate" => new WireResult(Generated: await extension.GenerateAsync(args.Template ?? throw new ToolContractException("invalid-params", "Template required."), cancellation)),
                    "operations.plan" => new WireResult(Plan: await extension.PlanAsync(args.Operation ?? throw new ToolContractException("invalid-params", "Operation required."), cancellation)),
                    _ => throw new ToolContractException("method-not-found", request.Method),
                };
                cancellation.ThrowIfCancellationRequested();
                response = new("2.0", request.Id!, result, null);
            }
            catch (Exception error) { response = new("2.0", request.Id!, null, new(error is ToolContractException contract ? contract.Code : error is OperationCanceledException ? "canceled" : "extension-failed", error.Message)); }
            await ToolFrames.WriteAsync(output, JsonSerializer.SerializeToUtf8Bytes(response, ToolWireJson.Context.WireResponse), writer, token);
        }
        try
        {
            while (await ToolFrames.ReadAsync(input, token) is { } payload)
            {
                ToolFrames.ValidateJson(payload);
                var request = JsonSerializer.Deserialize(payload, ToolWireJson.Context.WireRequest) ?? throw new InvalidDataException("Null request.");
                if (request.Jsonrpc != "2.0" || string.IsNullOrWhiteSpace(request.Method) || request.Params is null) throw new InvalidDataException("Invalid request.");
                if (request.Method == "$/cancelRequest")
                {
                    if (request.Params.RequestId is { } canceled && invocations.TryGetValue(canceled, out var invocation))
                        try { invocation.Cancel.Cancel(); } catch (ObjectDisposedException) { }
                    continue;
                }
                if (request.Id is null || !long.TryParse(request.Id, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var id) || id <= 0) throw new InvalidDataException("Request ID must be a positive decimal string.");
                if (request.Method == "extension.shutdown")
                {
                    lifetime.Cancel();
                    shutdown = Task.WhenAll(invocations.Values.Select(x => x.Work));
                    await shutdown;
                    await extension.DisposeAsync();
                    await ToolFrames.WriteAsync(output, JsonSerializer.SerializeToUtf8Bytes(new WireResponse("2.0", request.Id, new(), null), ToolWireJson.Context.WireResponse), writer, token);
                    return;
                }
                var cancel = CancellationTokenSource.CreateLinkedTokenSource(lifetime.Token);
                // Start only after insertion: cancellation cannot miss a synchronously completed request.
                var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                if (invocations.Count >= 128) throw new InvalidDataException("Too many active invocations.");
                var task = Task.Run(async () =>
                {
                    await start.Task;
                    try { await HandleAsync(request, cancel.Token); }
                    finally { if (invocations.TryRemove(request.Id, out var finished)) finished.Cancel.Dispose(); }
                });
                if (!invocations.TryAdd(request.Id, (cancel, task))) { cancel.Dispose(); throw new InvalidDataException("Duplicate request ID."); }
                start.SetResult();
            }
        }
        finally
        {
            lifetime.Cancel();
            try { await Task.WhenAll(invocations.Values.Select(x => x.Work)); }
            finally { foreach (var invocation in invocations.Values) invocation.Cancel.Dispose(); if (shutdown is null) await extension.DisposeAsync(); }
        }
    }
}
