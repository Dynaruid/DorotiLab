using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class LayoutCallbackContract
{
    public static void Run()
    {
        var previousErrorHandler = Doroti.Framework.Foundation.FlutterError.onError;
        Doroti.Framework.Foundation.FlutterError.onError = details =>
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(details.exceptionThrown);
        try { RunCore(); }
        finally { Doroti.Framework.Foundation.FlutterError.onError = previousErrorHandler; }
    }

    private static void RunCore()
    {
        VerifyLayerHandleLifetime();
        var box = (RenderBox)new LayoutBuilder(builder: (_, _) => throw new InvalidOperationException("Element callback is not used by this render contract."))
            .createRenderObject(null!);
        var builder = (RenderAbstractLayoutBuilderMixin<BoxConstraints, RenderObject>)box;
        var children = (IRenderObjectWithChild)box;
        var child = new RenderConstrainedBox(additionalConstraints: BoxConstraints.CreateTight(new Size(80, 40)));
        children.child = child;
        var owner = new PipelineOwner();
        var size = BoxConstraints.CreateTight(new Size(100, 60));
        var shell = new RenderConstrainedBox(additionalConstraints: size);
        owner.rootNode = shell;
        // Prime the root's constraints before adding a callback-driven child.
        // Layout callbacks must run inside PipelineOwner.flushLayout in Debug.
        shell.layout(size);
        shell.child = box;
        Require(child.attached && ReferenceEquals(child.parent, box), "Box child attach contract.");
        var calls = 0;
        builder._updateCallback(constraints =>
        {
            Require(ReferenceEquals(constraints, builder.layoutInfo), "Box callback and layoutInfo use the same constraints.");
            calls++;
        });
        owner.flushLayout();
        Require(calls == 1 && box.size == new Size(100, 60), "Box callback runs during layout.");
        builder._updateCallback(_ => calls += 10);
        owner.flushLayout();
        Require(calls == 11, "Callback replacement schedules another layout with unchanged constraints.");
        builder._callback = null;
        shell.child = null;
        Require(!child.attached, "Box child detach contract.");
        children.child = null;
        Require(child.parent is null, "Box child removal contract.");

        var sliver = (RenderSliver)new SliverLayoutBuilder(builder: (_, _) => throw new InvalidOperationException("Element callback is not used by this render contract."))
            .createRenderObject(null!);
        var sliverBuilder = (RenderAbstractLayoutBuilderMixin<SliverConstraints, RenderObject>)sliver;
        var sliverChild = new RenderSliverToBoxAdapter(child: new RenderConstrainedBox(additionalConstraints: BoxConstraints.CreateTight(new Size(100, 30))));
        var sliverChildren = (IRenderObjectWithChild)sliver;
        sliverChildren.child = sliverChild;
        shell.layout(BoxConstraints.CreateTight(new Size(100, 200)));
        var viewport = new RenderViewport(crossAxisDirection: AxisDirection.right,
            offset: ViewportOffset.CreateZero());
        viewport.add(sliver);
        viewport.center = sliver;
        shell.child = viewport;
        Require(sliverChild.attached, "Sliver child attach contract.");
        var sliverCalls = 0;
        sliverBuilder._updateCallback(constraints =>
        {
            Require(ReferenceEquals(constraints, sliverBuilder.layoutInfo), "Sliver layoutInfo retains its actual type.");
            sliverCalls++;
        });
        owner.flushLayout();
        Require(sliverCalls == 1 && sliver.geometry is not null, "Sliver callback produces geometry.");
        sliverBuilder._callback = null;
        shell.child = null;
        Require(!sliverChild.attached, "Sliver child detach contract.");
        sliverChildren.child = null;
        Require(sliverChild.parent is null, "Sliver child removal contract.");
        owner.rootNode = null;
        Console.WriteLine("Box/sliver static layout callback contracts: PASS");
    }

    private static void VerifyLayerHandleLifetime()
    {
        var first = new ExternalLayer();
        var second = new ExternalLayer();
        var a = new LayerHandle<ExternalLayer>(first);
        var b = new LayerHandle<Layer>();
        b.layer = first;
        a.layer = first; // Assigning the same layer must not retain it twice.
        a.layer = second;
        Require(first.DisposeCount == 0, "A shared layer survives one handle being replaced.");
        b.layer = null;
        Require(first.DisposeCount == 1, "The final handle releases a shared layer exactly once.");
        b.layer = null;
        a.layer = null;
        Require(first.DisposeCount == 1 && second.DisposeCount == 1,
            "Constructor and setter acquisitions balance their releases.");
        Console.WriteLine("External layer handle lifetime contracts: PASS");
    }

    private sealed class ExternalLayer : Layer
    {
        public int DisposeCount { get; private set; }
        public override void addToScene(SceneBuilder builder) { }
        public override void dispose()
        {
            base.dispose();
            DisposeCount++;
        }
    }

    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
