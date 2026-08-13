// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/_platform_selectable_region_context_menu_web.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static string _viewType = "Browser__WebContextMenuViewType__";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static string _kClassName = "web-selectable-region-context-menu";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static string _kClassSelectionRule = $".{_platform_selectable_region_context_menu_webLibrary._kClassName}::selection {{ background: transparent; }}";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static string _kClassRule = $".{_platform_selectable_region_context_menu_webLibrary._kClassName} {{\n  color: transparent;\n  user-select: text;\n  -webkit-user-select: text; /* Safari */\n  -moz-user-select: text; /* Firefox */\n  -ms-user-select: text; /* IE10+ */\n}}\n";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static long _kRightClickButton = 2L;
}

internal delegate void _WebSelectionCallBack___platform_selectable_region_context_menu_web(HTMLElement __unused0, MouseEvent __unused1);

public delegate void RegisterViewFactoryIo(string __unused0, global::System.Func<long, object> __unused1, bool isVisible = default!);

public class PlatformSelectableRegionContextMenuIo : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    internal static SelectionContainerDelegate? _activeClient = default;
    internal static string? _registeredViewType = default;
    public static RegisterViewFactoryIo? debugOverrideRegisterViewFactory = default;

    public PlatformSelectableRegionContextMenuIo(Widget child, global::Doroti.Generated.Framework.Foundation.Key? key = null) : base(key: key)
    {
        this.child = child;
    }

    public static void attach(SelectionContainerDelegate client)
    {
        _activeClient = client;
    }

    public static void detach(SelectionContainerDelegate client)
    {
        if ((object.Equals(_activeClient, client)))
        {
            _activeClient = null;
        }
    }

    public static SelectionContainerDelegate? debugActiveClient => _activeClient;
    internal static RegisterViewFactoryIo _registerViewFactory => DartRuntimePrimitives.ConvertValue<RegisterViewFactoryIo>(((debugOverrideRegisterViewFactory ?? (RegisterViewFactoryIo)Dart_ui_webLibrary.platformViewRegistry.registerViewFactory)));
    public static void debugResetRegistry()
    {
        _registeredViewType = null;
    }

    internal static void _register()
    {
        DartRuntimePrimitives.Assert(() => (_registeredViewType is null));
        _registeredViewType = PlatformSelectableRegionContextMenuIo._registerWebSelectionCallback(((global::System.Action<HTMLElement, MouseEvent>)((element, @event) => {
SelectionContainerDelegate? client__3377 = _activeClient;
if ((client__3377 is not null))
{
    var localOffset__3513 = new global::Doroti.Flutter.Ui.Offset(@event.offsetX.toDouble(), @event.offsetY.toDouble());
    Matrix4 transform__3609 = ((Matrix4)(object?)client__3377.getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderObject)(object)null)));
    global::Doroti.Flutter.Ui.Offset globalOffset__3671 = ((global::Doroti.Flutter.Ui.Offset)(object?)MatrixUtils.transformPoint(transform__3609, localOffset__3513));
    client__3377.dispatchSelectionEvent(new global::Doroti.Generated.Framework.Rendering.SelectWordSelectionEvent(globalPosition: globalOffset__3671));
    element.innerText = (client__3377.getSelectedContent()?.plainText ?? "");
    Range range__4093 = ((Func<Range>)(() =>
{            var __cascade = WebLibrary.document.createRange();
            __cascade.selectNode(element);
            return __cascade;        }))();
    DartRuntimePrimitives.Ignore(((Func<Selection?>)(() =>
{            var __cascade = WebLibrary.window.getSelection();
            __cascade.removeAllRanges();
            __cascade.addRange(range__4093);
            return __cascade;        }))());
}
})));
    }

    internal static string _registerWebSelectionCallback(global::System.Action<HTMLElement, MouseEvent> callback)
    {
        var styleElement__4397 = ((HTMLStyleElement?)(object?)WebLibrary.document.createElement("style"))!;
        WebLibrary.document.head!.append(((JSAny?)(object?)styleElement__4397)!);
        CSSStyleSheet sheet__4555 = styleElement__4397.sheet!;
        sheet__4555.insertRule(_platform_selectable_region_context_menu_webLibrary._kClassRule, 0L);
        sheet__4555.insertRule(_platform_selectable_region_context_menu_webLibrary._kClassSelectionRule, 1L);
        _registerViewFactory(_platform_selectable_region_context_menu_webLibrary._viewType, ((viewId, arg1) => {
var htmlElement__4751 = ((HTMLElement?)(object?)WebLibrary.document.createElement("div"))!;
DartRuntimePrimitives.Ignore(((Func<HTMLElement>)(() =>
{            var __cascade = htmlElement__4751;
            __cascade.style.width = "100%";
            __cascade.style.height = "100%";
            __cascade.classList.add(_platform_selectable_region_context_menu_webLibrary._kClassName);
            return __cascade;        }))());
htmlElement__4751.addEventListener("mousedown", ((@event) => {
var mouseEvent__5040 = ((MouseEvent?)(object?)@event)!;
mouseEvent__5040.preventDefault();
if ((mouseEvent__5040.button != _platform_selectable_region_context_menu_webLibrary._kRightClickButton))
{
    return;
}
callback(htmlElement__4751, mouseEvent__5040);
}).toJS);
return htmlElement__4751;
throw new InvalidOperationException("Dart closure completed without a value.");
}), isVisible: false);
        return _platform_selectable_region_context_menu_webLibrary._viewType;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Stack(fit: global::Doroti.Generated.Framework.Rendering.StackFit.passthrough, children: new List<Widget> { Positioned.CreateFill(child: new HtmlElementView(viewType: _platform_selectable_region_context_menu_webLibrary._viewType)), this.child }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

