#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_navigator.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public abstract class SystemNavigator
{
    public static async Future setFrameworkHandlesBack(bool frameworkHandlesBack)
    {
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return;
        }
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case var __case1074 when object.Equals(__case1074, TargetPlatform.iOS):
            case var __case1105 when object.Equals(__case1105, TargetPlatform.macOS):
            case var __case1138 when object.Equals(__case1138, TargetPlatform.fuchsia):
            case var __case1173 when object.Equals(__case1173, TargetPlatform.linux):
            case var __case1206 when object.Equals(__case1206, TargetPlatform.windows):
                {
                    return;
                }
            case var __case1257 when object.Equals(__case1257, TargetPlatform.android):
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
        return SystemChannels.navigation.invokeMethod<object?>("routeInformationUpdated", new DartMap<string, object> { ["uri"] = uri.ToString(), ["state"] = state, ["replace"] = replace });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

