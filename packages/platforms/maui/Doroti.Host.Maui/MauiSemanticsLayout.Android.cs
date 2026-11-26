#if ANDROID
using Android.OS;
using Android.Views.Accessibility;
using Doroti.Ui;
using NativeView = Android.Views.View;
using Android.Views;
using AndroidAction = Android.Views.Accessibility.Action;
using CheckedState = Doroti.Ui.CheckedState;
namespace Doroti.Host.Maui;

internal sealed partial class MauiSemanticsLayout
{
    private NativeView? _accessibilityView;
    private MauiSemanticsDelegate? _accessibilityDelegate;

    internal void ObserveAndroidProxy(Microsoft.Maui.Controls.View proxy) =>
        proxy.HandlerChanged += (_, _) => RefreshAndroidSemantics();

    private void RefreshAndroidSemantics()
    {
        var view = Handler?.PlatformView as NativeView;
        if (view?.Handle != _accessibilityView?.Handle)
        {
            if (_accessibilityView is { Handle: var handle } previous && handle != IntPtr.Zero)
                previous.SetAccessibilityDelegate(null);
            _accessibilityDelegate?.Dispose();
            _accessibilityDelegate = null;
            _accessibilityView = view;
        }
        if (view is not { Handle: var nativeHandle } || nativeHandle == IntPtr.Zero) return;
        if (Node is null && _accessibilityDelegate is not null)
        {
            view.SetAccessibilityDelegate(null);
            _accessibilityDelegate.Dispose();
            _accessibilityDelegate = null;
        }
        else if (Node is not null && _accessibilityDelegate is null)
        {
            _accessibilityDelegate = new MauiSemanticsDelegate(this);
            view.SetAccessibilityDelegate(_accessibilityDelegate);
        }
        var importance = Node is null ? ImportantForAccessibility.NoHideDescendants : ImportantForAccessibility.Yes;
        if (view.ImportantForAccessibility != importance) view.ImportantForAccessibility = importance;
        if (Children.Count > 0 && Children[0].Handler?.PlatformView is NativeView proxy
            && proxy.ImportantForAccessibility != ImportantForAccessibility.No)
            proxy.ImportantForAccessibility = ImportantForAccessibility.No;
    }

    internal bool OwnsAndroidSemantics(NativeView host, MauiSemanticsDelegate accessibilityDelegate) =>
        ReferenceEquals(_accessibilityDelegate, accessibilityDelegate)
        && host.Handle != IntPtr.Zero && host.Handle == _accessibilityView?.Handle;
}

internal sealed class MauiSemanticsDelegate(MauiSemanticsLayout owner) : NativeView.AccessibilityDelegate
{
    private const SemanticsAction ScrollForward = SemanticsAction.scrollUp | SemanticsAction.scrollLeft;
    private const SemanticsAction ScrollBackward = SemanticsAction.scrollDown | SemanticsAction.scrollRight;

    public override void OnInitializeAccessibilityNodeInfo(NativeView host, AccessibilityNodeInfo info)
    {
        base.OnInitializeAccessibilityNodeInfo(host, info);
        if (!owner.OwnsAndroidSemantics(host, this) || owner.Node is not { } node)
        {
            info.Enabled = false;
            return;
        }
        var textField = node.flags?.isTextField == true;
        var obscured = node.flags?.isObscured == true;
        info.ClassName = node.role switch
        {
            SemanticsRole.dialog or SemanticsRole.alertDialog => "android.app.Dialog",
            SemanticsRole.list => "android.widget.ListView",
            SemanticsRole.tabBar => "android.widget.TabWidget",
            SemanticsRole.progressBar or SemanticsRole.loadingSpinner => "android.widget.ProgressBar",
            SemanticsRole.menu or SemanticsRole.menuBar => "android.widget.PopupMenu",
            _ when textField => "android.widget.EditText",
            _ when node.flags?.isSlider == true => "android.widget.SeekBar",
            _ when node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed => "android.widget.CheckBox",
            _ when node.actions.HasFlag(SemanticsAction.tap) => "android.widget.Button",
            _ => "android.view.View",
        };
        info.ContentDescription = node.label;
        info.Text = obscured ? "" : node.value;
        info.Password = obscured;
        info.Enabled = node.flags?.isEnabled != Tristate.isFalse;
        info.Selected = node.flags?.isSelected == Tristate.isTrue;
        info.Checkable = node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed;
        if (OperatingSystem.IsAndroidVersionAtLeast(36))
            info.CheckedState = node.flags?.isChecked == CheckedState.mixed ? Android.Views.Accessibility.CheckedState.Partial
                : node.flags?.isChecked == CheckedState.isTrue ? Android.Views.Accessibility.CheckedState.True : Android.Views.Accessibility.CheckedState.False;
        else info.Checked = node.flags?.isChecked == CheckedState.isTrue;
        info.Editable = textField && node.flags?.isReadOnly != true;
        if (textField)
        {
            info.Focused = node.flags?.isFocused == Tristate.isTrue;
            if (!obscured && node.textSelectionBase >= 0 && node.textSelectionExtent >= 0)
            {
                var selection = MauiSemanticsBridge.ProjectedSelection(node);
                info.SetTextSelection(selection.Start, selection.End);
            }
        }
        info.Clickable = node.actions.HasFlag(SemanticsAction.tap);
        info.Scrollable = (node.actions & (ScrollForward | ScrollBackward)) != 0;
        if ((node.actions & ScrollForward) != 0) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionScrollForward);
        if ((node.actions & ScrollBackward) != 0) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionScrollBackward);
        if (info.Clickable) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionClick);
        if (node.actions.HasFlag(SemanticsAction.longPress)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionLongClick);
        if (node.actions.HasFlag(SemanticsAction.expand)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionExpand);
        if (node.actions.HasFlag(SemanticsAction.collapse)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionCollapse);
        if (node.actions.HasFlag(SemanticsAction.setText)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionSetText);
        if (node.actions.HasFlag(SemanticsAction.setSelection)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionSetSelection);
        if (node.actions.HasFlag(SemanticsAction.showOnScreen)) info.AddAction(AccessibilityNodeInfo.AccessibilityAction.ActionShowOnScreen);
    }
    public override bool PerformAccessibilityAction(NativeView host, AndroidAction action, Bundle? arguments)
    {
        if (!owner.OwnsAndroidSemantics(host, this) || owner.Node is not { } node) return false;
        if (action is AndroidAction.AccessibilityFocus or AndroidAction.ClearAccessibilityFocus)
            return base.PerformAccessibilityAction(host, action, arguments);

        var semantic = action switch
        {
            AndroidAction.Click => SemanticsAction.tap,
            AndroidAction.LongClick => SemanticsAction.longPress,
            AndroidAction.Expand => SemanticsAction.expand,
            AndroidAction.Collapse => SemanticsAction.collapse,
            AndroidAction.SetText => SemanticsAction.setText,
            AndroidAction.SetSelection => SemanticsAction.setSelection,
            AndroidAction.ScrollForward => node.actions.HasFlag(SemanticsAction.scrollUp)
                ? SemanticsAction.scrollUp : SemanticsAction.scrollLeft,
            AndroidAction.ScrollBackward => node.actions.HasFlag(SemanticsAction.scrollDown)
                ? SemanticsAction.scrollDown : SemanticsAction.scrollRight,
            (AndroidAction)16908342 => SemanticsAction.showOnScreen,
            _ => SemanticsAction.none,
        };
        object? payload = semantic switch
        {
            SemanticsAction.setText => arguments?.GetCharSequence("ACTION_ARGUMENT_SET_TEXT_CHARSEQUENCE")?.ToString(),
            SemanticsAction.setSelection => new Dictionary<string, long>
            {
                ["base"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_START_INT", -1) ?? -1,
                ["extent"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_END_INT", -1) ?? -1,
            },
            _ => null,
        };
        return semantic != SemanticsAction.none && owner.Dispatch(semantic, payload);
    }
}
#endif
