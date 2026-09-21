// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/binary_messenger.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public delegate Future<ByteData?>? MessageHandler(ByteData? message);

public interface BinaryMessenger
{
    public Future handlePlatformMessage(
        string channel,
        ByteData? data,
        Action<ByteData?>? callback
    );
    public Future<ByteData?>? send(string channel, ByteData? message);
    public void setMessageHandler(string channel, Func<ByteData?, Future<ByteData?>?>? handler);
}
