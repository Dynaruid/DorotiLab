using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Build.Locator;

namespace Doroti.Editor.Assist;

public static class Program
{
    public const string Schema = "doroti.editor/v1";
    public const int MaxMessage = 8 * 1024 * 1024;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static async Task Main()
    {
        // Resolve the user's SDK before loading any Microsoft.Build assembly.
        MSBuildLocator.RegisterDefaults();
        await ServeAsync();
    }

    private static async Task ServeAsync()
    {
        // Use an asynchronous pipe reader rather than Console.In's synchronized reader.
        // A blocked console read must never prevent the worker from writing an error response.
        using var input = new StreamReader(Console.OpenStandardInput(), new System.Text.UTF8Encoding(false));
        var output = Console.Out;
        var diagnostics = Console.Error;
        using var outputLock = new SemaphoreSlim(1, 1);
        using var engine = new Engine();
        var pending = new ConcurrentDictionary<string, CancellationTokenSource>();
        var queue = Channel.CreateBounded<Request>(new BoundedChannelOptions(64)
        { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait });
        async Task Reply(object value)
        {
            await outputLock.WaitAsync();
            try { await output.WriteLineAsync(JsonSerializer.Serialize(value, Json)); }
            finally { outputLock.Release(); }
        }
        var worker = Task.Run(async () =>
        {
            await foreach (var request in queue.Reader.ReadAllAsync())
            {
                if (!pending.TryGetValue(request.Id, out var cancellation)) continue;
                try
                {
                    var result = await engine.HandleAsync(request, cancellation.Token);
                    await Reply(new { schemaVersion = Schema, request.Id, request.Version, result });
                }
                catch (OperationCanceledException) { await Reply(new { schemaVersion = Schema, request.Id, error = "Canceled" }); }
                catch (Exception error)
                {
                    await diagnostics.WriteLineAsync(error.ToString());
                    await Reply(new { schemaVersion = Schema, request.Id, error = error.Message });
                }
                finally { pending.TryRemove(request.Id, out _); cancellation.Dispose(); }
            }
        });
        try
        {
            await foreach (var line in ReadMessagesAsync(input))
            {
                Request? request;
                try { request = JsonSerializer.Deserialize<Request>(line, Json); }
                catch (JsonException) { await Console.Error.WriteLineAsync("Invalid JSON request"); continue; }
                if (request is null || request.Id.Length > 128) continue;
                if (request.Operation == "hello")
                { await Reply(new { schemaVersion = Schema, request.Id, result = new { maxMessage = MaxMessage, language = "C# 14", roslyn = "5.9.0" } }); continue; }
                if (request.Operation == "cancel")
                { if (pending.TryGetValue(request.Id, out var c)) { try { c.Cancel(); } catch (ObjectDisposedException) { } } continue; }
                if (request.Operation == "invalidate") { engine.Invalidate(); continue; }
                if (pending.Count >= 64 || !pending.TryAdd(request.Id, new()))
                { await Console.Error.WriteLineAsync("Request queue full or duplicate ID"); continue; }
                await queue.Writer.WriteAsync(request);
            }
        }
        finally
        {
            queue.Writer.TryComplete();
            await worker;
        }
    }

    private static async IAsyncEnumerable<string> ReadMessagesAsync(TextReader input)
    {
        // Read a complete newline-delimited frame. ReadAsync(buffer) may wait for
        // another buffer's worth of data on a still-open pipe after a long frame.
        while (await input.ReadLineAsync() is { } line)
        {
            if (line.Length > MaxMessage) throw new InvalidDataException("Message exceeds 8 MiB");
            yield return line;
        }
    }
}
