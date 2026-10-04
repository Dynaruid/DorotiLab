using Doroti.Host.Maui;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Rect = Doroti.Ui.Rect;

namespace DorotiTestbedApp.WinUI;

internal static class MauiSemanticsRegression
{
    public static void Run(AbsoluteLayout layer)
    {
        var queue = new Queue<Action>();
        using var bridge = new MauiSemanticsBridge(layer, (callback, _) => queue.Enqueue(callback));
        var a = new SemanticsNodeUpdate(987, Rect.fromLTWH(1, 2, 30, 40), "latest", null, SemanticsAction.tap, []);
        var b = a with { rect = Rect.fromLTWH(20, 30, 30, 40) };
        var actionOwner = "";
        bridge.Update(new(1, [a]), (_, _, _) => actionOwner = "original");
        queue.Dequeue()();
        var peer = layer.Children.OfType<MauiSemanticsLayout>().Single();
        bridge.Update(new(2, [b]), (_, _, _) => actionOwner = "stale B");
        bridge.Update(new(3, [a], SemanticsUpdateUrgency.scrollEnd), (_, _, _) => actionOwner = "latest A");
        while (queue.TryDequeue(out var callback)) callback();
        if (peer.Node?.rect != a.rect || !peer.Dispatch(SemanticsAction.tap) || actionOwner != "latest A")
            throw new InvalidOperationException("A B A coalescing restored stale geometry/action ownership.");
        bridge.Update(new(2, [b]), (_, _, _) => actionOwner = "old generation");
        if (queue.Count != 0) throw new InvalidOperationException("Older semantics generation was scheduled.");
        bridge.Update(new(4, [b]), (_, _, _) => actionOwner = "disposed");
        bridge.Dispose();
        while (queue.TryDequeue(out var callback)) callback();
        if (peer.Dispatch(SemanticsAction.tap)) throw new InvalidOperationException("Disposed provider action revived.");
        layer.Children.Clear();
    }
}
