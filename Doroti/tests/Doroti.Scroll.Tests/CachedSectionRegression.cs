using System.Reflection;
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
        void Step(int step) { sliver.Step = step; sliver.markNeedsLayout(); sliver.layout(constraints); }
        try
        {
            Step(0);
            var a = manager.Children[0]; var b = manager.Children[1];
            var parentData = a.parentData;
            Check(sliver.childCount == 2 && a.Attaches == 1 && b.Attaches == 1, "initial children attach once");
            Step(1);
            Check(sliver.childCount == 1 && ReferenceEquals(sliver.firstChild, b), "caching unlinks only the visible list");
            Check(a.attached && ReferenceEquals(a.parent, sliver) && ReferenceEquals(a.parentData, parentData)
                && a.Detaches == 0 && a.Leaf.Detaches == 0, "cached subtree retains its exact owner, parent data and attachment");
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
        Console.WriteLine("PASS: cached section owner/parent-data/sibling preservation, no repeated attach or recreation, exact eviction teardown");
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
        public override void performLayout() { size = constraints.smallest; }
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
        public override void performLayout()
        {
            switch (Step)
            {
                case 0: Get(0); Get(1, manager.Children[0]); break;
                case 1: Cache.Invoke(this, [manager.Children[0]]); break;
                case 2: Get(0); break;
                case 3: Cache.Invoke(this, [manager.Children[0]]); Cache.Invoke(this, [manager.Children[1]]); break;
                case 4: Get(0); Get(1, manager.Children[0]); break;
                case 5:
                    ((SliverMultiBoxAdaptorParentData)manager.Children[0].parentData!).keepAlive = false;
                    Cache.Invoke(this, [manager.Children[0]]); break;
            }
            foreach (var box in manager.Children.Values.Where(b => b.attached))
                box.layout(BoxConstraints.CreateTight(new Size(200, 100)), parentUsesSize: true);
            geometry = new SliverGeometry(scrollExtent: 200, paintExtent: 200, maxPaintExtent: 200);
        }
    }
    private sealed class Manager : RenderSliverBoxChildManager
    {
        internal Sliver Owner = null!;
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
        public long childCount => 2;
        public void setDidUnderflow(bool value) { }
    }
    private static void Check(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
    }
}
