using System.Collections;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>Owner-local legacy codec bridge. Layout uses typed scene batches, never this channel.</summary>
public sealed class PlatformViewChannelAdapter : IPlatformMessageHostCapability, IDisposable
{
    private readonly IPlatformViewHostCapability _host;
    private readonly IPlatformMessageHostCapability _fallback;
    private readonly StandardMethodCodec _codec = new();
    private readonly object _gate = new();
    private readonly Dictionary<string, PlatformMessageHandler> _handlers = [];
    private readonly Dictionary<long, PlatformViewHandle> _handles = [];
    private readonly Dictionary<long, CancellationTokenSource> _creating = [];
    private bool _disposed;
    public event Action<Exception>? CallbackFailed;
    public PlatformViewChannelAdapter(IPlatformViewHostCapability host, IPlatformMessageHostCapability fallback)
    {
        _host = host;
        _fallback = fallback;
        _host.ViewFocused += OnFocused;
    }
    private static bool IsPlatformChannel(string channel) => channel is "flutter/platform_views" or "flutter/platform_views_2";
    public async ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default)
    {
        if (!IsPlatformChannel(channel)) return await _fallback.SendAsync(channel, data, cancellationToken);
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (data is null) return null;
        var call = _codec.decodeMethodCall((ByteData)data.Value);
        try
        {
            if (channel == "flutter/platform_views_2")
            {
                if (call.method == "isSurfaceControlEnabled") return _codec.encodeSuccessEnvelope(false).asMemory();
                throw Unsupported("SurfaceControl/HCPP strategy is not implemented");
            }
            object? result = null;
            var args = call.arguments as IDictionary;
            var id = Convert.ToInt64(args is null ? call.arguments : args["id"]);
            switch (call.method)
            {
                case "create":
                    if (args is null || args["viewType"] is not string viewType) throw new FormatException("create requires id and viewType");
                    // Android texture controllers expect a texture id and texture-backed resize.
                    if (args.Contains("hybridFallback") || args.Contains("width") || args.Contains("height") || args["hybrid"] is false)
                        throw Unsupported("texture/virtual-display Android strategy is not implemented");
                    if (args.Contains("direction") && Convert.ToInt64(args["direction"]) != 0)
                        throw Unsupported("native layout direction is not implemented by this bridge");
                    ReadOnlyMemory<byte> parameters = args["params"] switch
                    {
                        null => default,
                        Uint8List bytes => new ByteData(bytes).asMemory(),
                        byte[] bytes => bytes,
                        _ => throw new FormatException("creation parameters require encoded bytes"),
                    };
                    using (var pending = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
                    {
                        lock (_gate)
                        {
                            ObjectDisposedException.ThrowIf(_disposed, this);
                            if (_creating.ContainsKey(id) || _handles.ContainsKey(id)) throw Unsupported($"duplicate legacy id {id}");
                            _creating.Add(id, pending);
                        }
                        try
                        {
                            var handle = await _host.CreateAsync(new(id, viewType, CreationParameters: parameters), pending.Token);
                            bool dispose;
                            lock (_gate)
                            {
                                dispose = _disposed || pending.IsCancellationRequested;
                                if (!dispose) _handles[id] = handle;
                            }
                            if (dispose) { await _host.DisposeAsync(handle); throw new OperationCanceledException("Native creation was removed before attachment."); }
                        }
                        finally { lock (_gate) _creating.Remove(id); }
                    }
                    break;
                case "dispose":
                    PlatformViewHandle removed;
                    lock (_gate)
                    {
                        // Cancel before create replies; the coordinator reclaims a late native success.
                        if (_creating.TryGetValue(id, out var pending)) pending.Cancel();
                        if (!_handles.Remove(id, out removed)) break;
                    }
                    await _host.DisposeAsync(removed);
                    break;
                case "clearFocus":
                    await _host.SetFocusAsync(Resolve(id), false, cancellationToken);
                    break;
                default:
                    throw Unsupported($"legacy method '{call.method}' is unsupported; placement requires the typed scene path");
            }
            return _codec.encodeSuccessEnvelope(result).asMemory();
        }
        catch (DorotiCapabilityException exception)
        {
            return _codec.encodeErrorEnvelope("platform.views.unsupported", exception.Message,
                new DartMap<string, object> { ["ownerViewId"] = checked((long)_host.OwnerViewId), ["channel"] = channel }).asMemory();
        }
    }
    private PlatformViewHandle Resolve(long id)
    {
        lock (_gate) return _handles.TryGetValue(id, out var handle) ? handle : throw Unsupported($"unknown legacy id {id}");
    }
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
    {
        if (!IsPlatformChannel(channel)) { _fallback.SetMessageHandler(channel, handler); return; }
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (handler is null) _handlers.Remove(channel); else _handlers[channel] = handler;
        }
    }
    private void OnFocused(PlatformViewHandle handle)
    {
        PlatformMessageHandler? callback;
        lock (_gate)
        {
            if (_disposed || !_handles.TryGetValue(handle.InstanceId, out var current) || current != handle) return;
            callback = _handlers.GetValueOrDefault("flutter/platform_views");
        }
        if (callback is not null) _ = DispatchFocusAsync(callback, handle);
    }
    private async Task DispatchFocusAsync(PlatformMessageHandler callback, PlatformViewHandle handle)
    {
        try { await callback(_codec.encodeMethodCall(new MethodCall("viewFocused", handle.InstanceId)).asMemory(), default); }
        catch (Exception exception) { CallbackFailed?.Invoke(exception); }
    }
    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            _host.ViewFocused -= OnFocused;
            foreach (var pending in _creating.Values) pending.Cancel();
            _handlers.Clear();
            _handles.Clear();
            CallbackFailed = null;
        }
    }
    private DorotiCapabilityException Unsupported(string reason) => new(DorotiCapabilityIds.PlatformViews,
        _host.OwnerViewId, DartUiInvocation.Managed("flutter/platform_views"), reason);
}
