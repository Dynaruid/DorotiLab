namespace Doroti.Ui;

/// <summary>Lossless bounded event delivery. Slow readers backpressure the producer; closing the view cancels readers.</summary>
public interface IPlatformPluginEventsHostCapability
{
    IReadOnlyCollection<string> EventChannels { get; }
    IAsyncEnumerable<ReadOnlyMemory<byte>> SubscribeAsync(string channel, ReadOnlyMemory<byte>? arguments = null,
        int capacity = 16, CancellationToken cancellationToken = default);
}
