// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/context_menu_controller.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class ContextMenuController
{
    public virtual global::System.Action? onRemove { get; private set; }
    internal static global::System.Func<BuildContext, Widget>? _contextMenuBuilder = default;
    internal static CapturedThemes? _capturedThemes;
    internal static ContextMenuController? _shownInstance = default;
    internal static OverlayEntry? _menuOverlayEntry = default;

    public ContextMenuController(global::System.Action? onRemove = null)
    {
        this.onRemove = onRemove;
    }

    public virtual void show(BuildContext context, global::System.Func<BuildContext, Widget> contextMenuBuilder, Widget? debugRequiredFor = null)
    {
        OverlayState overlayState = Overlay.of(context, rootOverlay: true, debugRequiredFor: debugRequiredFor);
        // Capture from the caller before crossing into the root overlay. A local
        // Theme can override both the platform toolbar and its light/dark palette.
        CapturedThemes capturedThemes = InheritedTheme.capture(from: context, to: overlayState.context);
        if (this.isShown)
        {
            _contextMenuBuilder = (global::System.Func<BuildContext, Widget>)contextMenuBuilder;
            _capturedThemes = capturedThemes;
            _menuOverlayEntry?.markNeedsBuild();
            return;
        }
        removeAny();
        _contextMenuBuilder = (global::System.Func<BuildContext, Widget>)contextMenuBuilder;
        _capturedThemes = capturedThemes;
        _menuOverlayEntry = new OverlayEntry(builder: overlayContext =>
            _capturedThemes!.wrap(new Builder(builder: menuContext => _contextMenuBuilder!(menuContext))));
        _shownInstance = this;
        overlayState.insert(_menuOverlayEntry!);
    }

    public static void removeAny()
    {
        _menuOverlayEntry?.remove();
        _menuOverlayEntry?.dispose();
        _menuOverlayEntry = null;
        _contextMenuBuilder = null;
        _capturedThemes = null;
        if ((_shownInstance is not null))
        {
            _shownInstance!.onRemove?.Invoke();
            _shownInstance = null;
        }
    }

    public virtual bool isShown => DartRuntimePrimitives.ConvertValue<bool>((Equals(_shownInstance, this)));
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
        removeAny();
    }

}
