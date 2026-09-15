// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/deferred_component.dart
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

public abstract class DeferredComponent
{
    public static async Future installDeferredComponent(string componentName)
    {
        await SystemChannels.deferredComponent.invokeMethod<object?>("installDeferredComponent", new DartMap<string, object> { ["loadingUnitId"] = -1L, ["componentName"] = componentName });
    }

    public static async Future uninstallDeferredComponent(string componentName)
    {
        await SystemChannels.deferredComponent.invokeMethod<object?>("uninstallDeferredComponent", new DartMap<string, object> { ["loadingUnitId"] = -1L, ["componentName"] = componentName });
    }

}

