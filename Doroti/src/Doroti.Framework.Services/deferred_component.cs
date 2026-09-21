// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/deferred_component.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public abstract class DeferredComponent
{
    public static async Future installDeferredComponent(string componentName)
    {
        await SystemChannels.deferredComponent.invokeMethod<object?>(
            "installDeferredComponent",
            new DartMap<string, object>
            {
                ["loadingUnitId"] = -1L,
                ["componentName"] = componentName,
            }
        );
    }

    public static async Future uninstallDeferredComponent(string componentName)
    {
        await SystemChannels.deferredComponent.invokeMethod<object?>(
            "uninstallDeferredComponent",
            new DartMap<string, object>
            {
                ["loadingUnitId"] = -1L,
                ["componentName"] = componentName,
            }
        );
    }
}
