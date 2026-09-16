// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_navigator.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public abstract class SystemNavigator
{
    public static async Future setFrameworkHandlesBack(bool frameworkHandlesBack)
    {
        if (ConstantsLibrary.kIsWeb)
        {
            return;
        }
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case var __case1074 when Equals(__case1074, TargetPlatform.iOS):
            case var __case1105 when Equals(__case1105, TargetPlatform.macOS):
            case var __case1138 when Equals(__case1138, TargetPlatform.fuchsia):
            case var __case1173 when Equals(__case1173, TargetPlatform.linux):
            case var __case1206 when Equals(__case1206, TargetPlatform.windows):
                {
                    return;
                }
            case var __case1257 when Equals(__case1257, TargetPlatform.android):
                {
                    await SystemChannels.platform.invokeMethod<object?>("SystemNavigator.setFrameworkHandlesBack", frameworkHandlesBack);
                    return;
                }
        }
    }

    public static async Future pop(bool? animated = null)
    {
        await SystemChannels.platform.invokeMethod<object?>("SystemNavigator.pop", animated);
    }

    public static Future selectSingleEntryHistory()
    {
        return SystemChannels.navigation.invokeMethod<object?>("selectSingleEntryHistory");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future selectMultiEntryHistory()
    {
        return SystemChannels.navigation.invokeMethod<object?>("selectMultiEntryHistory");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future routeInformationUpdated(string? location = null, DartUri? uri = null, object? state = null, bool replace = false)
    {
        DartRuntimePrimitives.Assert(() => (((location is not null)) != ((uri is not null))));
        uri ??= DartUri.parse(location!);
        return SystemChannels.navigation.invokeMethod<object?>("routeInformationUpdated", new DartMap<string, object?> { ["uri"] = uri.ToString(), ["state"] = state, ["replace"] = replace });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

