// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/_background_isolate_binary_messenger_web.dart
using Doroti.Ui;

namespace Doroti.Framework.Services;

internal class BackgroundIsolateBinaryMessengerIo
{
    public static BinaryMessenger instance
    {
        get { throw new NotSupportedException("Isolates not supported on web."); }
    }

    public static void ensureInitialized(RootIsolateToken token)
    {
        throw new NotSupportedException("Isolates not supported on web.");
    }
}
