// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/debug.dart
using System.Globalization;
using Doroti.Ui;

namespace Doroti.Generated.Framework.Foundation;

public static class DebugLibrary
{
    public static bool debugInstrumentationEnabled { get; set; }
    public static int? debugDoublePrecision { get; set; }
    public static Brightness? debugBrightnessOverride { get; set; }
    public static Uri? connectedVmServiceUri { get; set; }
    public static Uri? activeDevToolsServerAddress { get; set; }

    public static string debugFormatDouble(double? value)
    {
        if (value is null)
        {
            return "null";
        }
        return debugDoublePrecision is { } precision
            ? value.Value.ToString($"F{precision}", CultureInfo.InvariantCulture)
            : value.Value.ToString("G", CultureInfo.InvariantCulture);
    }

    public static T debugInstrumentAction<T>(string description, Func<T> action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(action);
        if (!debugInstrumentationEnabled)
        {
            return action();
        }
        using var block = FlutterTimeline.startSync(description);
        return action();
    }

    public static bool debugAssertAllFoundationVarsUnset(string reason, bool debugPrintOverride = false)
    {
        if (debugInstrumentationEnabled || debugDoublePrecision is not null || debugBrightnessOverride is not null || debugPrintOverride)
        {
            throw new FlutterError(reason);
        }
        return true;
    }

    public static bool debugMaybeDispatchCreated(string libraryName, string className, object instance)
    {
        MemoryAllocations.instance.dispatchObjectCreated(libraryName, className, instance);
        return true;
    }

    public static bool debugMaybeDispatchDisposed(object instance)
    {
        MemoryAllocations.instance.dispatchObjectDisposed(instance);
        return true;
    }
}
