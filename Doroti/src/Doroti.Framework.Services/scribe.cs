#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/scribe.dart
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

public abstract class Scribe
{
    internal static MethodChannel _channel = SystemChannels.scribe;

    public static async Future<bool> isFeatureAvailable()
    {
        bool? result = await _channel.invokeMethod<bool?>("Scribe.isFeatureAvailable");
        if ((result is null))
        {
            throw new FlutterError("MethodChannel.invokeMethod unexpectedly returned null.");
        }
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future<bool> isStylusHandwritingAvailable()
    {
        bool? result = await _channel.invokeMethod<bool?>("Scribe.isStylusHandwritingAvailable");
        if ((result is null))
        {
            throw new FlutterError("MethodChannel.invokeMethod unexpectedly returned null.");
        }
        return DartRuntimePrimitives.RequireValue(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future startStylusHandwriting()
    {
        return _channel.invokeMethod<object?>("Scribe.startStylusHandwriting");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

