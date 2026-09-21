// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/scribe.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public abstract class Scribe
{
    internal static MethodChannel _channel = SystemChannels.scribe;

    public static async Future<bool> isFeatureAvailable()
    {
        bool? result = await _channel.invokeMethod<bool?>("Scribe.isFeatureAvailable");
        if (result is null)
        {
            throw new FlutterError("MethodChannel.invokeMethod unexpectedly returned null.");
        }
        return (
            result ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static async Future<bool> isStylusHandwritingAvailable()
    {
        bool? result = await _channel.invokeMethod<bool?>("Scribe.isStylusHandwritingAvailable");
        if (result is null)
        {
            throw new FlutterError("MethodChannel.invokeMethod unexpectedly returned null.");
        }
        return (
            result ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future startStylusHandwriting()
    {
        return _channel.invokeMethod<object?>("Scribe.startStylusHandwriting");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
