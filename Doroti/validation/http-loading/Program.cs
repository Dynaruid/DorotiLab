using System.Net;
using Doroti.Runtime;
using Response = Doroti.Runtime.HttpClientResponse;

var failures = new List<string>();
await Check("request headers", () => Headers(null));
await Check("timed request headers", () => Headers(TimeProvider.System));
await Check("complete response", async () =>
{
    var content = new ProbeContent();
    var chunks = new List<ReadOnlyMemory<byte>>();
    await foreach (var chunk in Wrap(content))
    {
        chunks.Add(chunk);
    }
    Require(chunks.Count > 1, "fixture must span multiple chunks");
    Require(chunks.SelectMany(chunk => chunk.ToArray()).SequenceEqual(content.Bytes), "bytes changed");
    Require(content.Disposed, "response content was not disposed");
});
await Check("early exit", async () =>
{
    var content = new ProbeContent();
    await foreach (var chunk in Wrap(content))
    {
        Require(chunk.Length > 0, "empty chunk");
        break;
    }
    Require(content.Disposed, "early exit retained response");
});
await Check("read failure", async () =>
{
    var content = new ProbeContent(failRead: true);
    await Expect<IOException>(async () =>
    {
        await foreach (var chunk in Wrap(content)) { }
    });
    Require(content.Disposed, "read failure retained response");
});
await Check("cancelled read", async () =>
{
    var content = new ProbeContent();
    using var cancellation = new CancellationTokenSource();
    await Expect<OperationCanceledException>(async () =>
    {
        await foreach (var chunk in Wrap(content).WithCancellation(cancellation.Token))
        {
            cancellation.Cancel();
        }
    });
    Require(content.Disposed, "cancellation retained response");
});
await Check("streamed error drain", async () =>
{
    var content = new ProbeContent();
    var result = await Wrap(content).drain(42);
    Require(result == 42, "drain result changed");
    Require(content.Disposed, "drain retained response");
    Require(!content.Serialized, "drain buffered the response body");
});
await Check("failed error drain", async () =>
{
    var content = new ProbeContent(failRead: true);
    await Expect<IOException>(async () => { await Wrap(content).drain(42); });
    Require(content.Disposed, "failed drain retained response");
});
await Check("unread rejected response", () =>
{
    var content = new ProbeContent();
    using (var response = Wrap(content))
    {
        Require(response.statusCode == 200, "status changed");
    }
    Require(content.Disposed, "unread response retained content");
    return Task.CompletedTask;
});
if (failures.Count > 0)
{
    throw new InvalidOperationException(string.Join(Environment.NewLine, failures));
}
Console.WriteLine("PASS: all HTTP loading contracts");

async Task Check(string name, Func<Task> action)
{
    try
    {
        await action();
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception error)
    {
        failures.Add($"FAIL: {name}: {error.Message}");
        Console.WriteLine(failures[^1]);
    }
}

static async Task Headers(TimeProvider? timeProvider)
{
    using var handler = new CaptureHandler();
    using var client = new System.Net.Http.HttpClient(handler);
    client.DefaultRequestHeaders.Add("X-Default", "preserved");
    var request = new HttpClientRequest(client, new DartUri("https://example.invalid/image"), timeProvider);
    request.headers.add("Authorization", "Bearer fixture-token");
    request.headers.add("X-Fixture", "one");
    request.headers.add("x-fixture", "two");
    var response = await request.close();
    await response.drain(0);
    Require(handler.Headers.TryGetValue("Authorization", out var auth) && auth.SequenceEqual(["Bearer fixture-token"]), "authorization missing");
    Require(handler.Headers.TryGetValue("X-Fixture", out var values) && values.SequenceEqual(["one", "two"]), "multi-value headers missing");
    Require(handler.Headers.ContainsKey("X-Default"), "default header lost");
    Require(!client.DefaultRequestHeaders.Contains("Authorization"), "per-request credentials leaked into shared client");
    var next = new HttpClientRequest(client, new DartUri("https://example.invalid/next"), timeProvider);
    await (await next.close()).drain(0);
    Require(!handler.Headers.ContainsKey("Authorization"), "credentials leaked into next request");
}

static Response Wrap(ProbeContent content) => new(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static async Task Expect<T>(Func<Task> action) where T : Exception
{
    try { await action(); }
    catch (T) { return; }
    throw new InvalidOperationException($"Expected {typeof(T).Name}");
}

sealed class CaptureHandler : HttpMessageHandler
{
    public Dictionary<string, string[]> Headers { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Headers = request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) });
    }
}

sealed class ProbeContent(bool failRead = false) : HttpContent
{
    public byte[] Bytes { get; } = Enumerable.Range(0, 20000).Select(i => (byte)(i % 251)).ToArray();
    public bool Disposed { get; private set; }
    public bool Serialized { get; private set; }

    protected override Task<Stream> CreateContentReadStreamAsync() =>
        Task.FromResult<Stream>(new ProbeStream(Bytes, failRead));

    protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken) =>
        CreateContentReadStreamAsync();

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        Serialized = true;
        await stream.WriteAsync(Bytes);
    }

    protected override bool TryComputeLength(out long length)
    {
        length = Bytes.Length;
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Disposed = true;
        base.Dispose(disposing);
    }
}

sealed class ProbeStream(byte[] bytes, bool failRead) : MemoryStream(bytes, writable: false)
{
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (failRead) throw new IOException("fixture read failure");
        return base.ReadAsync(buffer, cancellationToken);
    }

    public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        var buffer = new byte[bufferSize];
        int count;
        while ((count = await ReadAsync(buffer.AsMemory(), cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, count), cancellationToken);
        }
    }
}
