// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/context_menu_controller.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class ContextMenuController
{
    public virtual Action? onRemove { get; private set; }
    private static readonly Doroti.Ui.DispatcherLocal<Func<BuildContext, Widget>?> _contextLocal_contextMenuBuilder = new(() => default);
    internal static Func<BuildContext, Widget>? _contextMenuBuilder { get => _contextLocal_contextMenuBuilder.Value; set => _contextLocal_contextMenuBuilder.Value = value; }
    private static readonly Doroti.Ui.DispatcherLocal<CapturedThemes?> _contextLocal_capturedThemes = new(() => default);
    internal static CapturedThemes? _capturedThemes { get => _contextLocal_capturedThemes.Value; set => _contextLocal_capturedThemes.Value = value; }
    private static readonly Doroti.Ui.DispatcherLocal<ContextMenuController?> _contextLocal_shownInstance = new(() => default);
    internal static ContextMenuController? _shownInstance { get => _contextLocal_shownInstance.Value; set => _contextLocal_shownInstance.Value = value; }
    private static readonly Doroti.Ui.DispatcherLocal<OverlayEntry?> _contextLocal_menuOverlayEntry = new(() => default);
    internal static OverlayEntry? _menuOverlayEntry { get => _contextLocal_menuOverlayEntry.Value; set => _contextLocal_menuOverlayEntry.Value = value; }

    public ContextMenuController(Action? onRemove = null)
    {
        this.onRemove = onRemove;
    }

    public virtual void show(
        BuildContext context,
        Func<BuildContext, Widget> contextMenuBuilder,
        Widget? debugRequiredFor = null
    )
    {
        OverlayState overlayState = Overlay.of(
            context,
            rootOverlay: true,
            debugRequiredFor: debugRequiredFor
        );
        // Capture from the caller before crossing into the root overlay. A local
        // Theme can override both the platform toolbar and its light/dark palette.
        CapturedThemes capturedThemes = InheritedTheme.capture(
            from: context,
            to: overlayState.context
        );
        if (isShown)
        {
            _contextMenuBuilder = contextMenuBuilder;
            _capturedThemes = capturedThemes;
            _menuOverlayEntry?.markNeedsBuild();
            return;
        }
        removeAny();
        _contextMenuBuilder = contextMenuBuilder;
        _capturedThemes = capturedThemes;
        _menuOverlayEntry = new OverlayEntry(builder: overlayContext =>
            _capturedThemes!.wrap(
                new Builder(builder: menuContext => _contextMenuBuilder!(menuContext))
            )
        );
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
        if (_shownInstance is not null)
        {
            _shownInstance!.onRemove?.Invoke();
            _shownInstance = null;
        }
    }

    public virtual bool isShown =>
        DartRuntimePrimitives.ConvertValue<bool>(Equals(_shownInstance, this));

    public virtual void markNeedsBuild()
    {
        DartRuntimePrimitives.Assert(() => isShown);
        _menuOverlayEntry?.markNeedsBuild();
    }

    public virtual void remove()
    {
        if (!isShown)
        {
            return;
        }
        removeAny();
    }
}
