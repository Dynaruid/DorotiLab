using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

internal sealed partial class MauiSemanticsLayout : AbsoluteLayout
{
    private SemanticsNodeUpdate? _node;
    internal SemanticsNodeUpdate? Node => _node;

    internal void UpdateNode(SemanticsNodeUpdate? node)
    {
        var previous = _node;
        _node = node;
#if ANDROID
        if ((previous is null) != (node is null)) RefreshAndroidSemantics();
#elif MACOS || IOS || MACCATALYST
        UpdateAppleNode(previous);
#endif
    }

    internal MauiSemanticsLayout()
    {
#if ANDROID
        // The projected bounds already include the framework's safe-area offsets.
        SafeAreaEdges = Microsoft.Maui.SafeAreaEdges.None;
        HandlerChanged += (_, _) => RefreshAndroidSemantics();
#elif MACOS || IOS || MACCATALYST
        HandlerChanged += (_, _) =>
        {
            AttachAppleSemantics();
            (Parent as MauiSemanticsLayout)?.RefreshAppleChildren();
        };
        ChildAdded += (_, _) => Dispatcher.Dispatch(RefreshAppleChildren);
        ChildRemoved += (_, _) => Dispatcher.Dispatch(RefreshAppleChildren);
#endif
    }
    internal Func<SemanticsAction, object?, bool>? DispatchAction { get; set; }
    internal bool Dispatch(SemanticsAction action, object? arguments = null) => Node is { } node
        && SemanticsActionPolicy.Allows(node, action) && DispatchAction?.Invoke(action, arguments) == true;

}
