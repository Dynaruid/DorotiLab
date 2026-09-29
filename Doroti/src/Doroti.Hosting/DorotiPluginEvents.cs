namespace Doroti.Hosting;

/// <summary>Optional handler extension. Honor cancellation and await demand; release native subscription in finally.</summary>
public interface IDorotiPluginEventHandler : IDorotiNativePluginHandler
{
    IAsyncEnumerable<ReadOnlyMemory<byte>> SubscribeAsync(DorotiPluginContext context, string channel, string codec,
        ReadOnlyMemory<byte>? arguments, CancellationToken cancellationToken = default);
}
