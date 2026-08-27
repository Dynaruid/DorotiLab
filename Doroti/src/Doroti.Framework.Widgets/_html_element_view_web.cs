// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_html_element_view_web.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class _html_element_view_webLibrary
{
    internal static global::System.Action<long>? _createPlatformViewCallbackForElementCallback(global::System.Action<object>? onElementCreated)
    {
        if ((onElementCreated is null))
        {
            return ((global::System.Action<long>)(object)null);
        }
        return ((global::System.Action<long>)((id) => {
onElementCreated(Dart_ui_webLibrary.platformViewRegistry.getViewById(id));
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _HtmlElementViewController___html_element_view_web : global::Doroti.Framework.Services.PlatformViewController
{
    private long __field_viewId = default!;
    public override long viewId { get => __field_viewId; }
    public virtual string viewType { get; private set; } = default!;
    public virtual dynamic creationParams { get; private set; } = default!;
    internal virtual bool _initialized { get; set; } = false;

    internal _HtmlElementViewController___html_element_view_web(long viewId, string viewType, dynamic creationParams)
    {
        this.__field_viewId = viewId;
        this.viewType = viewType;
        this.creationParams = creationParams;
    }

    internal async virtual Future _initialize()
    {
        var args = new DartMap<string, object> { ["id"] = this.viewId, ["viewType"] = this.viewType, ["params"] = this.creationParams };
        await global::Doroti.Framework.Services.SystemChannels.platform_views.invokeMethod<object?>("create", args);
        _initialized = true;
    }

    public async override Future clearFocus()
    {
    }

    public async override Future dispatchPointerEvent(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
    }

    public async override Future dispose()
    {
        if (this._initialized)
        {
            await global::Doroti.Framework.Services.SystemChannels.platform_views.invokeMethod<object?>("dispose", this.viewId);
        }
    }

}

