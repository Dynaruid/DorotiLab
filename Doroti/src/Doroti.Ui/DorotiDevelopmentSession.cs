namespace Doroti.Ui;

// Shared managed state crosses the browser main/render threads without exposing
// framework types to the host. It correlates updates; it cannot apply code.
internal static class DorotiDevelopmentSession
{
    private static readonly object Gate = new();
    private static string? _runtimeId;
    private static string? _requestId;
    private static string _status = "{}";
    private static int _remoteStarted;
    internal static bool IsRemote => OperatingSystem.IsIOS() &&
        Environment.GetEnvironmentVariable("DOROTI_DEV_URL") is { Length: > 0 };
    internal static string Status { get { lock (Gate) return _status; } }
    internal static void Publish(string runtimeId, string json)
    {
        lock (Gate)
        {
            if (_runtimeId != runtimeId) _requestId = null;
            _runtimeId = runtimeId;
            _status = json;
        }
        if (IsRemote && Interlocked.Exchange(ref _remoteStarted, 1) == 0)
            _ = RunRemoteAsync();
    }
    internal static bool Prepare(string runtimeId, string requestId)
    {
        if (!Guid.TryParse(requestId, out _)) return false;
        lock (Gate)
        {
            if (_runtimeId != runtimeId) return false;
            _requestId = requestId;
            return true;
        }
    }
    internal static string? Request(string runtimeId)
    {
        lock (Gate) return _runtimeId == runtimeId ? _requestId : null;
    }

    // Only session status and request correlation cross this connection. The SDK's
    // authenticated Hot Reload agent owns metadata deltas and applies the code.
    private static async Task RunRemoteAsync()
    {
        using var client = new System.Net.Http.HttpClient(new System.Net.Http.SocketsHttpHandler
        {
            UseProxy = false,
            ConnectTimeout = TimeSpan.FromSeconds(2),
        }) { Timeout = TimeSpan.FromSeconds(3) };
        var url = Environment.GetEnvironmentVariable("DOROTI_DEV_URL")!;
        var token = Environment.GetEnvironmentVariable("DOROTI_DEV_TOKEN");
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        while (true)
        {
            try
            {
                string status;
                string? prepared;
                lock (Gate) { status = _status; prepared = _requestId; }
                using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
                request.Content = new System.Net.Http.StringContent(status, System.Text.Encoding.UTF8, "application/json");
                if (prepared is not null) request.Headers.Add("X-Doroti-Prepared", prepared);
                using var response = await client.SendAsync(request).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                using var json = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                var root = json.RootElement;
                if (root.TryGetProperty("sessionId", out var session) &&
                    session.GetString() == Environment.GetEnvironmentVariable("DOROTI_DEV_SESSION_ID") &&
                    root.TryGetProperty("runtimeId", out var runtime) &&
                    root.TryGetProperty("requestId", out var id))
                    Prepare(runtime.GetString()!, id.GetString()!);
                // Keep this process-level transport alive for a replacement
                // view; Publish resets its request when the runtime ID changes.
            }
            catch (Exception error) when (error is System.Net.Http.HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
            {
                // Backgrounding/disconnection must not stop rendering. The host
                // expires the heartbeat and disables the editor's Reload button.
            }
            await Task.Delay(150).ConfigureAwait(false);
        }
    }
}
