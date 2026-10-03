using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

internal sealed partial class MauiSemanticsLayout : AbsoluteLayout
{
    private SemanticsNodeUpdate? _node;
    internal SemanticsNodeUpdate? Node { get => _node; set { _node = value; RefreshNativeSemantics(); } }
    internal MauiSemanticsLayout()
    {
        HandlerChanged += (_, _) => { RefreshNativeSemantics(); (Parent as MauiSemanticsLayout)?.RefreshNativeSemantics(); };
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
#elif MACOS || IOS || MACCATALYST
        RefreshAppleSemantics();
#endif
    }
}
