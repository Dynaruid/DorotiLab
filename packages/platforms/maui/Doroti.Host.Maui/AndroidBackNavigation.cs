using Doroti.Framework.Services;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Host.Maui;

/// <summary>Connects Android system Back to the owning framework's Navigator/Router.</summary>
internal sealed class AndroidBackNavigation(IPlatformMessageHostCapability fallback)
    : IPlatformMessageHostCapability, IDisposable
{
    private readonly JSONMethodCodec _codec = new();
    private DorotiView? _view;
    private bool _pending;
    private bool _disposed;
    internal bool FrameworkHandlesBack { get; private set; }
    internal event Action<bool>? HandlesBackChanged;
    internal event Action? SystemPopRequested;

    internal void Attach(DorotiView view) => _view = view;

    internal Task RequestBack()
    {
        if (_disposed || _pending || _view is not { } view) return Task.CompletedTask;
        _pending = true;
        Task delivery = Task.CompletedTask;
        try
        {
            view.DispatchPlatformEvent(() =>
            {
                delivery = DeliverAsync(view);
                DartRuntimePrimitives.ObserveTask(delivery, "Android system Back");
            });
            return delivery;
        }
        catch
        {
            _pending = false;
            throw;
        }
    }

    private async Task DeliverAsync(DorotiView view)
    {
        try
        {
            await view.platformDispatcher.channelBuffers.push("flutter/navigation",
                _codec.encodeMethodCall(new MethodCall("popRoute")), _ => { });
        }
        finally { _pending = false; }
    }

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        if (channel == "flutter/platform" && data is { } bytes)
        {
            var call = _codec.decodeMethodCall((ByteData)bytes);
            switch (call.method)
            {
                case "SystemNavigator.setFrameworkHandlesBack":
                    FrameworkHandlesBack = (bool)call.arguments!;
                    HandlesBackChanged?.Invoke(FrameworkHandlesBack);
                    return ValueTask.FromResult<ReadOnlyMemory<byte>?>(_codec.encodeSuccessEnvelope(null).asMemory());
                case "SystemNavigator.pop":
                    SystemPopRequested?.Invoke();
                    return ValueTask.FromResult<ReadOnlyMemory<byte>?>(_codec.encodeSuccessEnvelope(null).asMemory());
            }
        }
        return fallback.SendAsync(channel, data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
        fallback.SetMessageHandler(channel, handler);

    public void Dispose()
    {
        _disposed = true;
        _view = null;
        HandlesBackChanged = null;
        SystemPopRequested = null;
    }
}
