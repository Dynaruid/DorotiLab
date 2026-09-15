// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/debug.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
                if ((DebugLibrary.debugKeyEventSimulatorTransitModeOverride is not null))
                {
                    throw new FlutterError(reason);
                }
                if (DebugLibrary.debugPrintKeyboardEvents)
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

