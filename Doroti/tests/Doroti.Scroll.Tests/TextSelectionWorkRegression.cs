using Doroti.Framework.Foundation;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class TextSelectionWorkRegression
{
    internal static void Run()
    {
        var controller = new TextEditingController("a😀한");
        var edits = 0;
        var textChanges = 0;
        controller.addListener(() => edits++);
        var text = controller.textListenable;
        Action changed = () => textChanges++;
        text.addListener(changed);
        controller.value = controller.value.copyWith(selection: TextSelection.CreateCollapsed(offset: 1));
        controller.value = controller.value.copyWith(composing: new TextRange(start: 1, end: 3));
        controller.selection = TextSelection.CreateCollapsed(offset: 3);
        Check(edits == 3 && textChanges == 0, "selection/composition still notify editors without rebuilding text-only consumers");
        controller.value = controller.value.copyWith(text: "z😀한");
        Check(textChanges == 1 && text.value == "z😀한", "same-length text edits update text-only consumers");
        controller.clearComposing();
        Check(textChanges == 1, "composition commit retains text-only decoration");
        controller.clear();
        Check(textChanges == 2 && text.value == "", "empty text updates labels/counters/semantics length");
        text.removeListener(changed);
        controller.text = "replacement";
        Check(textChanges == 2, "detached consumers receive no notifications");
        controller.dispose();
        Check(controller.debugListenerCount == 0 && ((ChangeNotifier)text).debugListenerCount == 0,
            "controller disposal releases the projection and its consumers");
        var reentrant = new TextEditingController("old");
        string? observed = null;
        reentrant.addListener(() => observed = reentrant.textListenable.value);
        _ = reentrant.textListenable;
        reentrant.text = "new";
        Check(observed == "new", "earlier listeners read the latest text revision");
        reentrant.dispose();

        var source = new SemanticsTextGeometry("a😀b", 10,
            [new(0,1,Rect.fromLTWH(0,0,10,20),0,4), new(1,3,Rect.fromLTWH(10,0,20,20),0,4), new(3,4,Rect.fromLTWH(30,0,10,20),0,4)]);
        var node = new SemanticsNodeUpdate(1, Rect.fromLTWH(0,0,100,30), "field", "a😀b",
            SemanticsAction.setSelection, [], textSelectionBase: 1, textSelectionExtent: 1,
            coordinateTransform: Matrix4.translationValues(5,7,0).storage, textGeometry: source);
        var original = SemanticsGeometryProjection.ToViewCoordinates([node])[0];
        var moved = SemanticsGeometryProjection.ToViewCoordinates([node with { textSelectionBase = 3, textSelectionExtent = 3 }])[0];
        Check(ReferenceEquals(original.textGeometry, moved.textGeometry) && moved.textSelectionBase == 3,
            "selection changes reuse projected glyphs while publishing the current selection");
        var delta = SemanticsUpdateDiffer.Diff(new Dictionary<int,SemanticsNodeUpdate> { [1] = original }, [moved]);
        Check(delta.changedNodes.Single().changedProperties == SemanticsNodeProperty.selection,
            "selection remains an immediate semantic change rather than a geometry rebuild");
        var translated = SemanticsGeometryProjection.ToViewCoordinates([node with { coordinateTransform = Matrix4.translationValues(15,7,0).storage }])[0];
        Check(translated.textGeometry!.Runs[0].Bounds.left == 15 && original.textGeometry!.Runs[0].Bounds.left == 5,
            "ancestor transform changes refresh glyphs without mutating old snapshots");
        var clipped = SemanticsGeometryProjection.ToViewCoordinates([node with { rect = Rect.fromLTWH(0,0,8,30) }])[0];
        Check(clipped.textGeometry!.Runs[0].Bounds.right == 13, "clip changes invalidate the projection");
        var scaled = SemanticsGeometryProjection.ToViewCoordinates([node], 2)[0];
        Check(scaled.textGeometry!.Runs[0].Bounds.left == 2.5, "DPR changes invalidate the projection");
        var hidden = SemanticsGeometryProjection.ToViewCoordinates([node with { flags = new SemanticsFlags(isObscured: true) }])[0];
        Check(hidden.textGeometry is null, "obscured text never publishes cached glyphs");
        Parallel.For(0, 20, i =>
        {
            var projected = SemanticsGeometryProjection.ToViewCoordinates([node with { coordinateTransform = Matrix4.translationValues(i,7,0).storage }])[0];
            Check(projected.textGeometry!.Runs[0].Bounds.left == i, "shared immutable geometry projects safely on multiple threads");
        });
        Console.WriteLine("PASS: text-only decoration notifications, latest semantic selection, projection/DPR/clip/obscure/thread invalidation");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
