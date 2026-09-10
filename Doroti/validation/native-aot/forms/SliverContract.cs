using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;

internal static class SliverContract
{
    internal static void Run()
    {
        var widget = SliverList.CreateList(children: []);
        var element = widget.createElement();
        element._renderObject = widget.createRenderObject(element);
        var owner = new PipelineOwner { rootNode = element.renderObject };
        var first = new ExternalBox();
        var second = new ExternalBox();
        element._currentlyUpdatingChildIndex = 0;
        element.insertRenderObjectChild(first, 0L);
        element._currentlyUpdatingChildIndex = 1;
        element._currentBeforeChild = first;
        element.insertRenderObjectChild(second, 1L);
        Check(first.attached && second.attached && element.renderObject.childCount == 2, "native sliver child attachment");
        Check(element.renderObject.indexOf(first) == 0 && element.renderObject.indexOf(second) == 1, "native slot/index propagation");
        element.renderObject.debugChildIntegrityEnabled = false;
        element._currentlyUpdatingChildIndex = 1;
        element.didAdoptChild(first);
        element._currentlyUpdatingChildIndex = 0;
        element._currentBeforeChild = null;
        element.moveRenderObjectChild(second, 1L, 0L);
        element.renderObject.debugChildIntegrityEnabled = true;
        Check(ReferenceEquals(element.renderObject.firstChild, second) && element.renderObject.indexOf(first) == 1, "native physical reorder and updated index");
        element._currentlyUpdatingChildIndex = 1;
        element.removeRenderObjectChild(first, 1L);
        Check(!first.attached && first.parent is null && first.Disposals == 0, "removal detaches before owner disposes");
        element._currentlyUpdatingChildIndex = 0;
        element.removeRenderObjectChild(second, 0L);
        Check(element.renderObject.childCount == 0 && !second.attached, "last child removal");
        first.dispose(); second.dispose();
        Check(first.Disposals == 1 && second.Disposals == 1, "external render resources disposed once");
        owner.rootNode = null;
        element.renderObject.dispose();

        var prototypeWidget = SliverPrototypeExtentList.CreateList(children: [], prototypeItem: new SizedBox(height: 70));
        var prototypeElement = (_SliverPrototypeExtentListElement__sliver_prototype_extent_list)prototypeWidget.createElement();
        prototypeElement._renderObject = prototypeWidget.createRenderObject(prototypeElement);
        owner.rootNode = prototypeElement.renderObject;
        var prototype = new ExternalBox();
        prototypeElement.insertRenderObjectChild(prototype, _SliverPrototypeExtentListElement__sliver_prototype_extent_list._prototypeSlot);
        Check(prototype.attached && ReferenceEquals(prototypeElement.renderObject.child, prototype), "prototype slot attaches its dedicated child");
        prototypeElement.removeRenderObjectChild(prototype, _SliverPrototypeExtentListElement__sliver_prototype_extent_list._prototypeSlot);
        Check(!prototype.attached && prototypeElement.renderObject.child is null, "prototype removal detaches child");
        prototype.dispose();
        owner.rootNode = null;
        prototypeElement.renderObject.dispose();
        Console.WriteLine("NativeAOT sliver hooks: external boxes, attach, slots, move, remove, prototype child and disposal PASS");
    }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private sealed class ExternalBox() : RenderConstrainedBox(additionalConstraints: new BoxConstraints())
    {
        public int Disposals;
        public override void dispose() { Disposals++; base.dispose(); }
    }
}
