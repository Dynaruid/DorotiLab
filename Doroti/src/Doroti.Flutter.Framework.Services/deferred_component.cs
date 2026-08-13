#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/deferred_component.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

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

