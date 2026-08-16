// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/debug.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
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

