using System.Reflection;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Ui;

internal static class CachedSectionRegression
{
    internal static void Run()
    {
        var manager = new Manager();
        var sliver = new Sliver(manager); manager.Owner = sliver;
        var pipeline = new PipelineOwner { rootNode = sliver };
        var constraints = new SliverConstraints(axisDirection: AxisDirection.down, growthDirection: GrowthDirection.forward,
            userScrollDirection: ScrollDirection.idle, scrollOffset: 0, precedingScrollExtent: 0, overlap: 0,
            remainingPaintExtent: 400, crossAxisExtent: 200, crossAxisDirection: AxisDirection.right,
            viewportMainAxisExtent: 400, remainingCacheExtent: 400, cacheOrigin: 0);
        Prepare(sliver, constraints);
        void Step(int step) { sliver.Step = step; sliver.markNeedsLayout(); Flush(pipeline); }
        try
        {
            Step(0);
            var a = manager.Children[0]; var b = manager.Children[1];
            var parentData = a.parentData;
            Check(sliver.childCount == 2 && a.Attaches == 1 && b.Attaches == 1, "initial children attach once");
            a.Leaf.markNeedsLayout();
            Step(1);
            Check(sliver.childCount == 1 && ReferenceEquals(sliver.firstChild, b), "caching unlinks only the visible list");
            Check(a.attached && ReferenceEquals(a.parent, sliver) && ReferenceEquals(a.parentData, parentData)
                && a.Detaches == 0 && a.Leaf.Detaches == 0, "cached subtree retains its exact owner, parent data and attachment");
            // Cached descendants can rebuild after their owner has completed
            // layout without laying them out. They must not inherit a stale
            // boundary path through that now-clean owner.
            a.Leaf.markNeedsLayout();
            Step(2);
            Check(sliver.childCount == 2 && ReferenceEquals(sliver.firstChild, a) && ReferenceEquals(sliver.lastChild, b)
                && ReferenceEquals(sliver.childAfter(a), b) && ReferenceEquals(sliver.childBefore(b), a), "restoration preserves sibling order");
            Check(a.Attaches == 1 && a.Leaf.Attaches == 1 && manager.Created == 2 && manager.Adopted == 2,
                "restoration neither recreates nor readopts the loaded subtree");
            Step(3);
            Check(sliver.childCount == 0 && sliver.firstChild is null && sliver.lastChild is null,
                "all visible children may enter the retained bucket");
            Step(4);
            Check(sliver.childCount == 2 && a.Attaches == 1 && b.Attaches == 1, "repeated reuse does not accumulate attach callbacks");
            Step(5);
            Check(!a.attached && a.parent is null && a.Detaches == 1 && a.Leaf.Detaches == 1,
                "real eviction still detaches the entire subtree exactly once");
            Check(sliver.childCount == 1 && ReferenceEquals(sliver.firstChild, b), "eviction preserves the remaining visible child");
        }
        finally { sliver.removeAll(); pipeline.rootNode = null; sliver.dispose(); pipeline.dispose(); }
        CheckSuspendedOwner();
        SectionReparentingRegression.Run();
        Console.WriteLine("PASS: cached section ownership, dirty descendants, suspended owner/resume, retained attachment and exact eviction teardown");
    }

    private static void CheckSuspendedOwner()
    {
        var manager = new Manager(1);
        var index = new SectionExtentIndex(1);
        var sliver = new RenderSectionList(manager, index, null, [0]);
        manager.Owner = sliver;
        var pipeline = new PipelineOwner { rootNode = sliver };
        var constraints = new SliverConstraints(axisDirection: AxisDirection.down, growthDirection: GrowthDirection.forward,
            userScrollDirection: ScrollDirection.idle, scrollOffset: 0, precedingScrollExtent: 0, overlap: 0,
            remainingPaintExtent: 400, crossAxisExtent: 200, crossAxisDirection: AxisDirection.right,
            viewportMainAxisExtent: 400, remainingCacheExtent: 400, cacheOrigin: 0);
        Prepare(sliver, constraints);
        try
        {
            Flush(pipeline);
            var child = manager.Children[0];
            child.Leaf.markNeedsLayout();
            sliver.Update(index, null, [0], suspended: true);
            Flush(pipeline);
            Check(sliver.childCount == 0 && child.attached && ReferenceEquals(child.parent, sliver),
                "suspending a viewport caches its linked children without losing ownership");
            child.Leaf.markNeedsLayout();
            Check(child.Attaches == 1 && child.Detaches == 0 && child.Leaf.Detaches == 0,
                "offstage descendant updates preserve attachment");
            sliver.Update(index, null, [0], restoreRetainedChildren: true);
            Flush(pipeline);
            Check(ReferenceEquals(sliver.firstChild, child) && child.Attaches == 1 && manager.Created == 1,
                "resuming lays out the same child and restores a valid inherited boundary");
            child.Leaf.markNeedsLayout();
            Flush(pipeline);
        }
        finally
        {
            sliver.markNeedsLayout();
            sliver.removeAll();
            pipeline.rootNode = null;
            sliver.dispose();
            pipeline.dispose();
        }
    }

    private static void Prepare(RenderSliverMultiBoxAdaptor sliver, SliverConstraints constraints)
    {
        typeof(RenderObject).GetProperty("_constraints", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(sliver, constraints);
        sliver.scheduleInitialLayout();
    }

    private static void Flush(PipelineOwner pipeline)
    {
        var errors = new List<FlutterErrorDetails>();
        var previous = FlutterError.onError;
        FlutterError.onError = errors.Add;
        try { pipeline.flushLayout(); }
        finally { FlutterError.onError = previous; }
        if (errors.Count != 0)
            throw new InvalidOperationException("Retained-section layout reported a framework error.", errors[0].exceptionThrown);
    }
    private sealed class Box : RenderProxyBox
    {
        internal int Attaches, Detaches;
        internal LeafBox Leaf { get; } = new();
        internal Box() { child = Leaf; }
        public override void attach(PipelineOwner owner) { base.attach(owner); Attaches++; }
        public override void detach() { Detaches++; base.detach(); }
    }
    private sealed class LeafBox : RenderBox
    {
        internal int Attaches, Detaches;
        public override void performLayout() { size = constraints.constrain(new Size(200, 100)); }
        public override void attach(PipelineOwner owner) { base.attach(owner); Attaches++; }
        public override void detach() { Detaches++; base.detach(); }
    }
    private sealed class Sliver(Manager manager) : RenderSliverMultiBoxAdaptor(manager)
    {
        protected override bool preserveCachedChildAttachment => true;
        internal int Step;
        private static readonly MethodInfo Cache = typeof(RenderSliverMultiBoxAdaptor).GetMethod("_destroyOrCacheChild", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private static readonly MethodInfo Obtain = typeof(RenderSliverMultiBoxAdaptor).GetMethod("_createOrObtainChild", BindingFlags.Instance | BindingFlags.NonPublic)!;
        private void Get(int id, RenderBox? after = null) => Obtain.Invoke(this, [(long)id, after]);
        private void Keep(Box child) => invokeLayoutCallback<SliverConstraints>(_ => Cache.Invoke(this, [child]));
        public override void performLayout()
        {
            switch (Step)
            {
                case 0: Get(0); Get(1, manager.Children[0]); break;
                case 1: Keep(manager.Children[0]); break;
                case 2: Get(0); break;
                case 3: Keep(manager.Children[0]); Keep(manager.Children[1]); break;
                case 4: Get(0); Get(1, manager.Children[0]); break;
                case 5:
                    ((SliverMultiBoxAdaptorParentData)manager.Children[0].parentData!).keepAlive = false;
                    Keep(manager.Children[0]); break;
            }
            foreach (var box in manager.Children.Values.Where(b => b.attached
                && b.parentData is SliverMultiBoxAdaptorParentData { keptAlive: false }))
                box.layout(new BoxConstraints(minWidth: 200, maxWidth: 200), parentUsesSize: true);
            geometry = new SliverGeometry(scrollExtent: 200, paintExtent: 200, maxPaintExtent: 200);
        }
    }
    private sealed class Manager(int count = 2) : RenderSliverBoxChildManager
    {
        internal RenderSliverMultiBoxAdaptor Owner = null!;
        internal readonly Dictionary<int, Box> Children = [];
        internal int Created, Adopted;
        private int _index;
        public void createChild(long index, RenderBox? after)
        {
            _index = (int)index;
            var child = new Box(); Children.Add(_index, child); Created++;
            Owner.insert(child, after);
            ((SliverMultiBoxAdaptorParentData)child.parentData!).keepAlive = true;
        }
        public void removeChild(RenderBox child) => Owner.remove(child);
        public void didAdoptChild(RenderBox child) { ((SliverMultiBoxAdaptorParentData)child.parentData!).index = _index; Adopted++; }
        public double estimateMaxScrollOffset(SliverConstraints c, long? firstIndex = null, long? lastIndex = null,
            double? leadingScrollOffset = null, double? trailingScrollOffset = null) => 200;
        public long childCount => count;
        public void setDidUnderflow(bool value) { }
    }
    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
