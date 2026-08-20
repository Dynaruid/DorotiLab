using Doroti.Ui;

namespace Doroti.Host.Qt;

internal sealed class QtPlatformMessageCapability : IPlatformMessageHostCapability
{
    private readonly object _gate = new();
    private readonly Dictionary<string, PlatformMessageHandler> _handlers = new(StringComparer.Ordinal);

    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        PlatformMessageHandler? handler;
        lock (_gate) _handlers.TryGetValue(channel, out handler);
        return handler is null ? ValueTask.FromResult<ReadOnlyMemory<byte>?>(null) : handler(data, cancellationToken);
    }

    public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        lock (_gate)
        {
            if (handler is null) _handlers.Remove(channel);
            else _handlers[channel] = handler;
        }
    }
}
