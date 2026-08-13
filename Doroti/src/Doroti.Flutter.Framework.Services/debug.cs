#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/debug.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

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

