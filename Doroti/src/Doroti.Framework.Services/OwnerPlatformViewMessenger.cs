using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

/// <summary>Legacy channel endpoints bound to the originating view, even after an await or owner-scope switch.</summary>
internal sealed class OwnerPlatformViewMessenger(DorotiView owner) : BinaryMessenger
{
    private readonly IPlatformMessageHostCapability _messages = owner.RequireCapability<IPlatformMessageHostCapability>(
        DorotiCapabilityIds.PlatformMessaging, DartUiInvocation.Managed("PlatformView.messenger"));
    public async Future<ByteData?> send(string channel, ByteData? message)
    {
        var reply = await owner.SendPlatformMessageAsync(channel, message?.asMemory(), DartUiInvocation.Managed("PlatformView.channel"));
        return reply is { } data ? (ByteData)data : null;
    }
    public void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler)
    {
        _messages.SetMessageHandler(channel, handler is null ? null : async (message, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var dispatcherScope = owner.platformDispatcher.EnterScope();
            using var ownerScope = PlatformViewsService.EnterOwner(owner);
            var future = handler(message is { } bytes ? (ByteData)bytes : null);
            if (future is null) return null;
            var result = await future;
            return result?.asMemory();
        });
    }
    public Future handlePlatformMessage(string channel, ByteData? data, Action<ByteData?>? callback) =>
        throw new NotSupportedException("Host callbacks use the owner platform capability, not messenger loopback.");
}
