// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/context_menu_controller.dart
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

public class ContextMenuController
{
    public virtual global::System.Action? onRemove { get; private set; }
    internal static global::System.Func<BuildContext, Widget>? _contextMenuBuilder = default;
    internal static ContextMenuController? _shownInstance = default;
    internal static OverlayEntry? _menuOverlayEntry = default;

    public ContextMenuController(global::System.Action? onRemove = null)
    {
        this.onRemove = onRemove;
    }

    public virtual void show(BuildContext context, global::System.Func<BuildContext, Widget> contextMenuBuilder, Widget? debugRequiredFor = null)
    {
        if (this.isShown)
        {
            _contextMenuBuilder = (global::System.Func<BuildContext, Widget>)contextMenuBuilder;
            _menuOverlayEntry?.markNeedsBuild();
            return;
        }
        ContextMenuController.removeAny();
        OverlayState overlayState__2265 = ((OverlayState)(object?)Overlay.of(context, rootOverlay: true, debugRequiredFor: debugRequiredFor));
        _contextMenuBuilder = (global::System.Func<BuildContext, Widget>)contextMenuBuilder;
        _menuOverlayEntry = new OverlayEntry(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            CapturedThemes capturedThemes__2535 = ((CapturedThemes)(object?)InheritedTheme.capture(from: context, to: Navigator.maybeOf(context)?.context));
            return ((Widget)(object?)capturedThemes__2535.wrap(_contextMenuBuilder!(context)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        _shownInstance = this;
        overlayState__2265.insert(_menuOverlayEntry!);
    }

    public static void removeAny()
    {
        _menuOverlayEntry?.remove();
        _menuOverlayEntry?.dispose();
        _menuOverlayEntry = null;
        _contextMenuBuilder = null;
        if ((_shownInstance is not null))
        {
            _shownInstance!.onRemove?.Invoke();
            _shownInstance = null;
        }
    }

    public virtual bool isShown => DartRuntimePrimitives.ConvertValue<bool>((object.Equals(_shownInstance, this)));
    public virtual void markNeedsBuild()
    {
        DartRuntimePrimitives.Assert(() => this.isShown);
        _menuOverlayEntry?.markNeedsBuild();
    }

    public virtual void remove()
    {
        if (!this.isShown)
        {
            return;
        }
        ContextMenuController.removeAny();
    }

}

