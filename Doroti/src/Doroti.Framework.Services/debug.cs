// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/debug.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public static partial class DebugLibrary
{
    public static KeyDataTransitMode? debugKeyEventSimulatorTransitModeOverride;
}

public static partial class DebugLibrary
{
    public static bool debugPrintKeyboardEvents = false;
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllServicesVarsUnset(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (debugKeyEventSimulatorTransitModeOverride is not null)
            {
                throw new FlutterError(reason);
            }
            if (debugPrintKeyboardEvents)
            {
                throw new FlutterError(reason);
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class DebugLibrary
{
    public static bool debugProfilePlatformChannels = false;
}
