// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/scheduler/debug.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Scheduler;

public static partial class DebugLibrary
{
    public static bool debugPrintBeginFrameBanner = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintEndFrameBanner = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintScheduleFrameStacks = false;
}

public static partial class DebugLibrary
{
    public static bool debugTracePostFrameCallbacks = false;
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllSchedulerVarsUnset(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((DebugLibrary.debugPrintBeginFrameBanner || DebugLibrary.debugPrintEndFrameBanner))
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

