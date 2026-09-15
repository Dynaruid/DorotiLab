// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/debug.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Gestures;

public static partial class DebugLibrary
{
    public static bool debugPrintHitTestResults = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintMouseHoverEvents = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintGestureArenaDiagnostics = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintRecognizerCallbacksTrace = false;
}

public static partial class DebugLibrary
{
    public static bool debugPrintResamplingMargin = false;
}

public static partial class DebugLibrary
{
    public static bool debugAssertAllGesturesVarsUnset(string reason)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((DebugLibrary.debugPrintHitTestResults || DebugLibrary.debugPrintGestureArenaDiagnostics) || DebugLibrary.debugPrintRecognizerCallbacksTrace) || DebugLibrary.debugPrintResamplingMargin))
                {
                    throw new FlutterError(reason);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

