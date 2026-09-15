// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/binding.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Semantics;

public interface SemanticsBinding
{
    public static global::Doroti.Framework.Semantics.SemanticsBinding? _instance = default;
    ValueNotifier<bool> _semanticsEnabled { get; }
    ObserverList<Action<SemanticsActionEvent>> _semanticsActionListeners { get; }
    long _outstandingHandles { get; set; }
    SemanticsHandle? _semanticsHandle { get; set; }
    AccessibilityFeatures _accessibilityFeatures { get; set; }

    public static global::Doroti.Framework.Semantics.SemanticsBinding instance
    {
        get => BindingBase.checkInstance(global::Doroti.Framework.Semantics.SemanticsBinding._instance);
    }
    public bool semanticsEnabled { get; }
    public void addSemanticsEnabledListener(Action listener);
    public void removeSemanticsEnabledListener(Action listener);
    public void addSemanticsActionListener(Action<SemanticsActionEvent> listener);
    public void removeSemanticsActionListener(Action<SemanticsActionEvent> listener);
    public global::Doroti.Ui.Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId);
    public long debugOutstandingSemanticsHandles { get; }
    public SemanticsHandle ensureSemantics();
    public void _didDisposeSemanticsHandle();
    public void _handleSemanticsEnabledChanged();
    public void _handleSemanticsActionEvent(SemanticsActionEvent action);
    public void _handleFrameworkSemanticsEnabledChanged();
    public void performSemanticsAction(SemanticsActionEvent action);
    public global::Doroti.Ui.AccessibilityFeatures accessibilityFeatures { get; }
    public void handleAccessibilityFeaturesChanged();
    public global::Doroti.Ui.SemanticsUpdateBuilder createSemanticsUpdateBuilder();
    public bool disableAnimations { get; }
}

public class SemanticsHandle
{
    internal virtual Action _onDispose { get; private set; } = default!;
    public SemanticsHandle() { }


    public SemanticsHandle(Action _onDispose)
    {
        this._onDispose = _onDispose;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._onDispose();
    }

}

