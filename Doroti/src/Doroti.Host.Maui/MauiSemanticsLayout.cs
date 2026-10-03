using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

internal sealed class MauiSemanticsLayout : AbsoluteLayout
{
    private SemanticsNodeUpdate? _node;
    internal SemanticsNodeUpdate? Node { get => _node; set { _node = value; RefreshNativeSemantics(); } }
    internal MauiSemanticsLayout()
    {
        HandlerChanged += (_, _) => RefreshNativeSemantics();
        ChildAdded += (_, _) => Dispatcher.Dispatch(RefreshNativeSemantics);
        ChildRemoved += (_, _) => Dispatcher.Dispatch(RefreshNativeSemantics);
    }
    internal Func<SemanticsAction, object?, bool>? DispatchAction { get; set; }
    internal bool Dispatch(SemanticsAction action, object? arguments = null) => Node is { } node
        && SemanticsActionPolicy.Allows(node, action) && DispatchAction?.Invoke(action, arguments) == true;

    private void RefreshNativeSemantics()
    {
#if ANDROID
        if (Handler?.PlatformView is Android.Views.View native)
        {
            native.ImportantForAccessibility = _node is null ? Android.Views.ImportantForAccessibility.NoHideDescendants : Android.Views.ImportantForAccessibility.Yes;
            native.SetAccessibilityDelegate(new MauiSemanticsDelegate(this));
            if (native is Android.Views.ViewGroup group && group.ChildCount > 0)
                group.GetChildAt(0)!.ImportantForAccessibility = Android.Views.ImportantForAccessibility.No;
        }
#elif IOS || MACCATALYST
        if (Handler?.PlatformView is UIKit.UIView native)
        {
            native.IsAccessibilityElement = _node is not null && (_node.children?.Count ?? 0) == 0;
            native.AccessibilityLabel = _node?.label;
            native.AccessibilityValue = _node?.flags?.isObscured == true ? null : _node?.value;
            native.AccessibilityHint = _node?.hint;
            native.AccessibilityTraits = _node?.flags?.isLink == true ? UIKit.UIAccessibilityTrait.Link
                : _node?.flags?.isHeader == true ? UIKit.UIAccessibilityTrait.Header
                : _node?.flags?.isImage == true ? UIKit.UIAccessibilityTrait.Image
                : _node?.actions.HasFlag(SemanticsAction.tap) == true ? UIKit.UIAccessibilityTrait.Button
                : UIKit.UIAccessibilityTrait.None;
            if (_node?.flags?.isEnabled == Tristate.isFalse) native.AccessibilityTraits |= UIKit.UIAccessibilityTrait.NotEnabled;
            if (_node?.flags?.isSelected == Tristate.isTrue) native.AccessibilityTraits |= UIKit.UIAccessibilityTrait.Selected;
            foreach (var child in native.Subviews.Take(1)) child.IsAccessibilityElement = false;
            native.AccessibilityCustomActions = _node is { } node
                ? Enum.GetValues<SemanticsAction>().Where(action => action != SemanticsAction.none &&
                    node.actions.HasFlag(action) && action is SemanticsAction.tap or SemanticsAction.longPress or
                    SemanticsAction.increase or SemanticsAction.decrease or SemanticsAction.expand or
                    SemanticsAction.collapse or SemanticsAction.showOnScreen)
                    .Select(action => new UIKit.UIAccessibilityCustomAction(action switch {
                        SemanticsAction.tap => "Activate", SemanticsAction.longPress => "Long press",
                        SemanticsAction.increase => "Increase", SemanticsAction.decrease => "Decrease",
                        SemanticsAction.expand => "Expand", SemanticsAction.collapse => "Collapse",
                        SemanticsAction.showOnScreen => "Show on screen", _ => "Activate"
                    }, _ => Dispatch(action))).ToArray()
                : [];
        }
#endif
    }
}
