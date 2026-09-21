// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/_platform_selectable_region_context_menu_web.dart
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
    internal static string _kClassSelectionRule =
        $".{_platform_selectable_region_context_menu_webLibrary._kClassName}::selection {{ background: transparent; }}";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static string _kClassRule =
        $".{_platform_selectable_region_context_menu_webLibrary._kClassName} {{\n  color: transparent;\n  user-select: text;\n  -webkit-user-select: text; /* Safari */\n  -moz-user-select: text; /* Firefox */\n  -ms-user-select: text; /* IE10+ */\n}}\n";
}

public static partial class _platform_selectable_region_context_menu_webLibrary
{
    internal static long _kRightClickButton = 2L;
}

internal delegate void _WebSelectionCallBack___platform_selectable_region_context_menu_web(
    HTMLElement __unused0,
    MouseEvent __unused1
);

public delegate void RegisterViewFactoryIo(
    string __unused0,
    global::System.Func<long, object> __unused1,
    bool isVisible = default!
);

public class PlatformSelectableRegionContextMenuIo : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    internal static SelectionContainerDelegate? _activeClient = default;
    internal static string? _registeredViewType = default;
    public static RegisterViewFactoryIo? debugOverrideRegisterViewFactory = default;

    public PlatformSelectableRegionContextMenuIo(
        Widget child,
        global::Doroti.Framework.Foundation.Key? key = null
    )
        : base(key: key)
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
    internal static RegisterViewFactoryIo _registerViewFactory =>
        DartRuntimePrimitives.ConvertValue<RegisterViewFactoryIo>(
            (
                (
                    debugOverrideRegisterViewFactory
                    ?? (RegisterViewFactoryIo)
                        DorotiUiWebLibrary.platformViewRegistry.registerViewFactory
                )
            )
        );

    public static void debugResetRegistry()
    {
        _registeredViewType = null;
    }

    internal static void _register()
    {
        DartRuntimePrimitives.Assert(() => (_registeredViewType is null));
        _registeredViewType = PlatformSelectableRegionContextMenuIo._registerWebSelectionCallback(
            (
                (global::System.Action<HTMLElement, MouseEvent>)(
                    (element, @event) =>
                    {
                        SelectionContainerDelegate? client = _activeClient;
                        if ((client is not null))
                        {
                            var localOffset = new global::Doroti.Ui.Offset(
                                @event.offsetX.toDouble(),
                                @event.offsetY.toDouble()
                            );
                            Matrix4 transform = (
                                (Matrix4)
                                    (object?)
                                        client.getTransformTo(
                                            (
                                                (global::Doroti.Framework.Rendering.RenderObject)
                                                    (object)null
                                            )
                                        )
                            );
                            global::Doroti.Ui.Offset globalOffset = (
                                (global::Doroti.Ui.Offset)
                                    (object?)MatrixUtils.transformPoint(transform, localOffset)
                            );
                            client.dispatchSelectionEvent(
                                new global::Doroti.Framework.Rendering.SelectWordSelectionEvent(
                                    globalPosition: globalOffset
                                )
                            );
                            element.innerText = (client.getSelectedContent()?.plainText ?? "");
                            Range range = (
                                (Func<Range>)(
                                    () =>
                                    {
                                        var __cascade = WebLibrary.document.createRange();
                                        __cascade.selectNode(element);
                                        return __cascade;
                                    }
                                )
                            )();
                            DartRuntimePrimitives.Ignore(
                                (
                                    (Func<Selection?>)(
                                        () =>
                                        {
                                            var __cascade = WebLibrary.window.getSelection();
                                            __cascade.removeAllRanges();
                                            __cascade.addRange(range);
                                            return __cascade;
                                        }
                                    )
                                )()
                            );
                        }
                    }
                )
            )
        );
    }

    internal static string _registerWebSelectionCallback(
        global::System.Action<HTMLElement, MouseEvent> callback
    )
    {
        var styleElement = (
            (HTMLStyleElement?)(object?)WebLibrary.document.createElement("style")
        )!;
        WebLibrary.document.head!.append(((JSAny?)(object?)styleElement)!);
        CSSStyleSheet sheetLocal = styleElement.sheet!;
        sheetLocal.insertRule(_platform_selectable_region_context_menu_webLibrary._kClassRule, 0L);
        sheetLocal.insertRule(
            _platform_selectable_region_context_menu_webLibrary._kClassSelectionRule,
            1L
        );
        _registerViewFactory(
            _platform_selectable_region_context_menu_webLibrary._viewType,
            (
                (viewId, arg1) =>
                {
                    var htmlElement = (
                        (HTMLElement?)(object?)WebLibrary.document.createElement("div")
                    )!;
                    DartRuntimePrimitives.Ignore(
                        (
                            (Func<HTMLElement>)(
                                () =>
                                {
                                    var __cascade = htmlElement;
                                    __cascade.style.width = "100%";
                                    __cascade.style.height = "100%";
                                    __cascade.classList.add(
                                        _platform_selectable_region_context_menu_webLibrary._kClassName
                                    );
                                    return __cascade;
                                }
                            )
                        )()
                    );
                    htmlElement.addEventListener(
                        "mousedown",
                        (
                            (@event) =>
                            {
                                var mouseEvent = ((MouseEvent?)(object?)@event)!;
                                mouseEvent.preventDefault();
                                if (
                                    (
                                        mouseEvent.button
                                        != _platform_selectable_region_context_menu_webLibrary._kRightClickButton
                                    )
                                )
                                {
                                    return;
                                }
                                callback(htmlElement, mouseEvent);
                            }
                        ).toJS
                    );
                    return htmlElement;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            ),
            isVisible: false
        );
        return _platform_selectable_region_context_menu_webLibrary._viewType;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return (
            (Widget)
                (object?)
                    new Stack(
                        fit: global::Doroti.Framework.Rendering.StackFit.passthrough,
                        children: new List<Widget>
                        {
                            Positioned.CreateFill(
                                child: new HtmlElementView(
                                    viewType: _platform_selectable_region_context_menu_webLibrary._viewType
                                )
                            ),
                            this.child,
                        }
                    )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
