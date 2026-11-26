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
    public override void OnInitializeAccessibilityNodeInfo(NativeView host, AccessibilityNodeInfo info)
    {
        base.OnInitializeAccessibilityNodeInfo(host, info);
        if (!owner.OwnsAndroidSemantics(host, this) || owner.Node is not { } node)
        {
            info.Enabled = false;
            return;
        }
        info.ClassName = node.role switch
        {
            SemanticsRole.dialog or SemanticsRole.alertDialog => "android.app.Dialog",
            SemanticsRole.list => "android.widget.ListView",
            SemanticsRole.tabBar => "android.widget.TabWidget",
            SemanticsRole.progressBar or SemanticsRole.loadingSpinner => "android.widget.ProgressBar",
            SemanticsRole.menu or SemanticsRole.menuBar => "android.widget.PopupMenu",
            _ when node.flags?.isTextField == true => "android.widget.EditText",
            _ when node.flags?.isSlider == true => "android.widget.SeekBar",
            _ when node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed => "android.widget.CheckBox",
            _ when node.actions.HasFlag(SemanticsAction.tap) => "android.widget.Button",
            _ => "android.view.View",
        };
        info.ContentDescription = node.label;
        info.Text = node.flags?.isObscured == true ? "" : node.value;
        info.Password = node.flags?.isObscured == true;
        info.Enabled = node.flags?.isEnabled != Tristate.isFalse;
        info.Selected = node.flags?.isSelected == Tristate.isTrue;
        info.Checkable = node.flags?.isChecked is CheckedState.isTrue or CheckedState.isFalse or CheckedState.mixed;
        if (OperatingSystem.IsAndroidVersionAtLeast(36))
            info.CheckedState = node.flags?.isChecked == CheckedState.mixed ? Android.Views.Accessibility.CheckedState.Partial
                : node.flags?.isChecked == CheckedState.isTrue ? Android.Views.Accessibility.CheckedState.True : Android.Views.Accessibility.CheckedState.False;
        else info.Checked = node.flags?.isChecked == CheckedState.isTrue;
        info.Editable = node.flags?.isTextField == true && node.flags?.isReadOnly != true;
        info.Clickable = node.actions.HasFlag(SemanticsAction.tap);
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
        if (!owner.OwnsAndroidSemantics(host, this)) return false;
        if (action is AndroidAction.AccessibilityFocus or AndroidAction.ClearAccessibilityFocus)
            return base.PerformAccessibilityAction(host, action, arguments);

        var semantic = (int)action switch {
            16 => SemanticsAction.tap, 32 => SemanticsAction.longPress, 262144 => SemanticsAction.expand,
            524288 => SemanticsAction.collapse, 2097152 => SemanticsAction.setText,
            131072 => SemanticsAction.setSelection, 16908342 => SemanticsAction.showOnScreen,
            _ => SemanticsAction.none };
        object? payload = semantic == SemanticsAction.setText ? arguments?.GetCharSequence("ACTION_ARGUMENT_SET_TEXT_CHARSEQUENCE")?.ToString()
            : semantic == SemanticsAction.setSelection ? new Dictionary<string, object?> {
                ["base"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_START_INT", -1),
                ["extent"] = arguments?.GetInt("ACTION_ARGUMENT_SELECTION_END_INT", -1) } : null;
        return semantic != SemanticsAction.none && owner.Dispatch(semantic, payload);
    }
}
#endif
