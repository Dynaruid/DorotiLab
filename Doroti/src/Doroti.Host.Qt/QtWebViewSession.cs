using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Doroti.Ui;

namespace Doroti.Host.Qt;

// A command adapter for the coordinator-owned Quick item. No second native view.
internal sealed partial class QtWebViewSession : IDisposable
{
    internal const ulong RequiredFeatures = 1 | 2 | 4 | 8 | 16;
    internal static bool SupportsFeatures(ulong features) =>
        (features & RequiredFeatures) == RequiredFeatures;
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Api
    {
        internal uint Version,
            Size;
        internal ulong Features;
        internal delegate* unmanaged[Cdecl]<
            ulong,
            ulong,
            delegate* unmanaged[Cdecl]<nint, QtNativeV2.Utf8, void>,
            nint,
            int> Bind;
        internal delegate* unmanaged[Cdecl]<ulong, ulong, QtNativeV2.Utf8, int> Execute;
    }

    private readonly QtPlatformViewHost _host;
    private readonly PlatformViewHandle _handle;
    private readonly ulong _owner,
        _id;
    private readonly Action<WebViewEvent> _changed;
    private Api _api;
    private GCHandle _context;
    private readonly Dictionary<long, TaskCompletionSource<WebViewResult>> _pending = [];
    private readonly object _gate = new();
    private long _request;
    private bool _closed;

    internal unsafe QtWebViewSession(
        QtPlatformViewHost host,
        PlatformViewHandle handle,
        ulong owner,
        ulong id,
        Action<WebViewEvent> changed
    )
    {
        _host = host;
        _handle = handle;
        _owner = owner;
        _id = id;
        _changed = changed;
        Check(GetApi(1, (uint)sizeof(Api), out _api), "get_webview_api");
        if (
            _api.Version != 1
            || _api.Size != sizeof(Api)
            || !SupportsFeatures(_api.Features)
            || _api.Bind == null
            || _api.Execute == null
        )
        {
            throw new WebViewException(WebViewError.Unsupported, "Invalid Qt WebView ABI 1 table.");
        }

        _context = GCHandle.Alloc(this);
        try
        {
            Check(_api.Bind(owner, id, &Callback, GCHandle.ToIntPtr(_context)), "bind", owner);
        }
        catch
        {
            _context.Free();
            throw;
        }
    }

    internal static void Check(int status, string operation, ulong owner = 0)
    {
        if (status != 0)
        {
            var error = status switch
            {
                64 or 72 => WebViewError.InvalidRequest,
                69 or 70 or 80 or 81 or 82 or 83 => WebViewError.ProcessFailed,
                71 or 73 => WebViewError.Closed,
                _ => WebViewError.Unsupported,
            };
            throw new WebViewException(
                error,
                $"Qt WebView {operation} rejected (native status {status}, owner {owner})."
            ) { NativeStatus = status, NativeOperation = operation, NativeOwner = owner };
        }
    }

    internal static async Task<byte[]> PrepareAsync(
        ReadOnlyMemory<byte> parameters,
        IApplicationResourceHostCapability? resources,
        string applicationId,
        CancellationToken cancellation
    )
    {
        var text = Encoding.UTF8.GetString(parameters.Span);
        if (
            text.StartsWith("doroti-webview:", StringComparison.Ordinal)
            && !text.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Unknown WebView options version."
            );
        }

        WebViewOptions options;
        try
        {
            options = text.StartsWith(WebViewOptions.Prefix, StringComparison.Ordinal)
                ? JsonSerializer.Deserialize(
                    text[WebViewOptions.Prefix.Length..],
                    WebViewJsonContext.Default.WebViewOptions
                ) ?? throw new WebViewException(WebViewError.InvalidRequest, "Missing options.")
                : new(Html: text);
        }
        catch (JsonException error)
        {
            throw new WebViewException(WebViewError.InvalidRequest, error.Message);
        }
        options.Validate();
        if (
            options.Profile == WebViewProfile.SharedPersistent
            && options.Resources is { Count: > 0 }
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Per-view app content routes require an ephemeral Qt profile."
            );
        }

        if (
            options.MessageOrigins is { Length: > 0 } origins
            && (
                origins.Length != 1
                || origins[0] != "doroti-app://content"
                || options.Resources is not { Count: > 0 }
            )
        )
        {
            throw new WebViewException(
                WebViewError.Unsupported,
                "Qt messages support only trusted manifest app content."
            );
        }

        var value = JsonNode
            .Parse(JsonSerializer.Serialize(options, WebViewJsonContext.Default.WebViewOptions))!
            .AsObject();
        var routes = new JsonObject();
        long total = 0;
        foreach (var (path, route) in options.Resources ?? [])
        {
            var manifest =
                resources?.Resources.SingleOrDefault(item => item.Key == route.ResourceKey)
                ?? throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Resource is not registered in the application manifest."
                );
            if (manifest.Length > 8 * 1024 * 1024 || (total += manifest.Length) > 12 * 1024 * 1024)
            {
                throw new WebViewException(
                    WebViewError.InvalidRequest,
                    "Qt content is limited to 8 MiB per resource and 12 MiB per view."
                );
            }

            var bytes = await resources!
                .LoadAsync(route.ResourceKey, cancellation)
                .ConfigureAwait(false);
            routes[path] = new JsonObject
            {
                ["MimeType"] = route.MimeType,
                ["Data"] = Convert.ToBase64String(bytes.Span),
            };
        }
        value["NativeResources"] = routes;
        value["NativeApplicationId"] = applicationId;
        return Encoding.UTF8.GetBytes(WebViewOptions.Prefix + value.ToJsonString());
    }

    internal Task<WebViewResult> ExecuteAsync(
        WebViewCommand command,
        CancellationToken cancellation
    )
    {
        cancellation.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(command.Operation))
        {
            throw new WebViewException(WebViewError.Unsupported, "Unknown operation.");
        }

        if (Encoding.UTF8.GetByteCount(command.Text ?? "") > 2 * 1024 * 1024)
        {
            throw new WebViewException(WebViewError.InvalidRequest, "Command exceeds 2 MiB.");
        }

        long request;
        var completion = new TaskCompletionSource<WebViewResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        lock (_gate)
        {
            if (_closed)
            {
                throw new WebViewException(WebViewError.Closed, "WebView is closed.");
            }

            if (_pending.Count >= 32)
            {
                throw new WebViewException(WebViewError.Busy, "32 WebView commands are pending.");
            }

            request = ++_request;
            _pending.Add(request, completion);
        }
        try
        {
            var value = new JsonObject
            {
                ["request"] = request.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["operation"] = (int)command.Operation,
                ["text"] = command.Text,
                ["generation"] = command.DocumentGeneration,
            };
            Send(Encoding.UTF8.GetBytes(value.ToJsonString()));
        }
        catch
        {
            lock (_gate)
            {
                _pending.Remove(request);
            }
            throw;
        }
        return Await();
        async Task<WebViewResult> Await()
        {
            try
            {
                return await completion
                    .Task.WaitAsync(TimeSpan.FromSeconds(16), cancellation)
                    .ConfigureAwait(false);
            }
            finally
            {
                lock (_gate)
                {
                    _pending.Remove(request);
                }
            }
        }
    }

    private unsafe void Send(byte[] bytes)
    {
        fixed (byte* data = bytes)
        {
            Check(_api.Execute(_owner, _id, new(data, (ulong)bytes.Length)), "execute", _owner);
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void Callback(nint context, QtNativeV2.Utf8 value)
    {
        QtWebViewSession? session = null;
        try
        {
            session = (QtWebViewSession)GCHandle.FromIntPtr(context).Target!;
            using var document = JsonDocument.Parse(
                new ReadOnlySpan<byte>(value.Data, checked((int)value.Length)).ToArray()
            );
            session.Receive(document.RootElement);
        }
        catch (Exception error)
        {
            session?.Fail(error);
        }
    }

    private void Receive(JsonElement value)
    {
        lock (_gate)
        {
            if (_closed)
            {
                return;
            }
        }

        var navigation = value.GetProperty("navigation").GetInt64();
        var generation = value.GetProperty("generation").GetInt64();
        var url = value.GetProperty("url").GetString();
        if (value.TryGetProperty("event", out var kind))
        {
            var message = value.TryGetProperty("message", out var data) ? data : default;
            _changed(
                new(
                    _handle,
                    navigation,
                    generation,
                    (WebViewEventKind)kind.GetInt32(),
                    url,
                    value.TryGetProperty("text", out var error) ? error.GetString() : null,
                    message.ValueKind == JsonValueKind.Object
                        ? message.GetProperty("name").GetString()
                        : null,
                    message.ValueKind == JsonValueKind.Object
                    && message.TryGetProperty("payload", out var payload)
                        ? payload.GetRawText()
                        : null,
                    message.ValueKind == JsonValueKind.Object
                        ? message.GetProperty("requestId").GetInt64()
                        : 0
                )
            );
            return;
        }
        var request = long.Parse(
            value.GetProperty("request").GetString()!,
            System.Globalization.CultureInfo.InvariantCulture
        );
        TaskCompletionSource<WebViewResult>? completion;
        lock (_gate)
        {
            if (!_pending.Remove(request, out completion))
            {
                return;
            }
        }

        var code = value.GetProperty("error").GetInt32();
        if (code >= 0)
        {
            completion.TrySetException(
                new WebViewException(
                    (WebViewError)code,
                    value.GetProperty("text").GetString() ?? "Qt WebView failed."
                )
            );
            return;
        }
        WebViewFeatures? features = value.TryGetProperty("features", out var feature)
            ? new(
                true,
                true,
                true,
                true,
                false,
                feature.GetProperty("messages").GetBoolean(),
                feature.GetProperty("content").GetBoolean()
            )
            : null;
        completion.TrySetResult(
            new(
                request,
                navigation,
                generation,
                value.TryGetProperty("text", out var text) ? text.GetString() : null,
                value.TryGetProperty("undefined", out var undefined) && undefined.GetBoolean(),
                url,
                value.GetProperty("title").GetString(),
                value.GetProperty("loading").GetBoolean(),
                value.GetProperty("back").GetBoolean(),
                value.GetProperty("forward").GetBoolean(),
                features
            )
        );
    }

    private void Fail(Exception error)
    {
        lock (_gate)
        {
            foreach (var completion in _pending.Values)
            {
                completion.TrySetException(error);
            }
            _pending.Clear();
        }
    }

    // Native close disconnects before canceling posts/destroying QML. Normal retirement unbinds first.
    internal unsafe void Close(bool nativeClosed)
    {
        lock (_gate)
        {
            if (_closed)
            {
                return;
            }
        }
        if (!nativeClosed)
        {
            var status = _api.Bind(_owner, _id, null, 0);
            // Native removal/owner close disconnects callbacks before rejecting
            // further operations, so already-retired tokens are safe to release.
            if (status is not (0 or 71 or 73))
            {
                Check(status, "unbind", _owner);
            }
        }
        lock (_gate)
        {
            _closed = true;
        }

        Fail(new WebViewException(WebViewError.Closed, "WebView closed."));
        if (_context.IsAllocated)
        {
            _context.Free();
        }
    }

    public void Dispose() => Close(false);

    [LibraryImport("doroti_qt_host", EntryPoint = "doroti_qt_get_webview_api")]
    private static partial int GetApi(uint version, uint size, out Api api);
}
