// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/semantics/binding.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Semantics;

public interface SemanticsBinding
{
    public static global::Doroti.Generated.Framework.Semantics.SemanticsBinding? _instance = default;
    ValueNotifier<bool> _semanticsEnabled { get; }
    ObserverList<Action<SemanticsActionEvent>> _semanticsActionListeners { get; }
    long _outstandingHandles { get; set; }
    SemanticsHandle? _semanticsHandle { get; set; }
    AccessibilityFeatures _accessibilityFeatures { get; set; }

    public static global::Doroti.Generated.Framework.Semantics.SemanticsBinding instance
    {
        get => BindingBase.checkInstance(global::Doroti.Generated.Framework.Semantics.SemanticsBinding._instance);
    }
    public bool semanticsEnabled { get; }
    public void addSemanticsEnabledListener(Action listener);
    public void removeSemanticsEnabledListener(Action listener);
    public void addSemanticsActionListener(Action<SemanticsActionEvent> listener);
    public void removeSemanticsActionListener(Action<SemanticsActionEvent> listener);
    public global::Doroti.Flutter.Ui.Rect? getRectOfSemanticsNodeInViewCoordinates(long viewId, long nodeId);
    public long debugOutstandingSemanticsHandles { get; }
    public SemanticsHandle ensureSemantics();
    public void _didDisposeSemanticsHandle();
    public void _handleSemanticsEnabledChanged();
    public void _handleSemanticsActionEvent(SemanticsActionEvent action);
    public void _handleFrameworkSemanticsEnabledChanged();
    public void performSemanticsAction(SemanticsActionEvent action);
    public global::Doroti.Flutter.Ui.AccessibilityFeatures accessibilityFeatures { get; }
    public void handleAccessibilityFeaturesChanged();
    public global::Doroti.Flutter.Ui.SemanticsUpdateBuilder createSemanticsUpdateBuilder();
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._onDispose();
    }

}

