using System.Reflection;
using System.Text.Json;
using Path = System.IO.Path;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

// Public probes also exercise virtual dispatch through the pre-change DLR path.
public static class ExecutionCostContracts
{
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS " + message);
    }

    public static void Verify(string output)
    {
        var box = new CacheProbe();
        Func<double, double>[] requests = [box.getMinIntrinsicWidth, box.getMaxIntrinsicWidth,
            box.getMinIntrinsicHeight, box.getMaxIntrinsicHeight];
        foreach (var request in requests)
        {
            foreach (var input in new[] { 0.0, -0.0, 20.0, double.PositiveInfinity, double.NaN })
            {
                var before = box.IntrinsicCalls;
                var first = request(input);
                var afterFirst = box.IntrinsicCalls;
                Require(request(input).Equals(first) && box.IntrinsicCalls == afterFirst,
                    "intrinsic repeated key is cached, including NaN/infinity/signed zero");
                Require(afterFirst - before <= 1, "intrinsic miss computes at most once");
            }
        }
        Require(box.IntrinsicCalls == 16, "four intrinsic kinds stay distinct; signed zero shares a key");
        var constraints = new BoxConstraints(maxWidth: 100, maxHeight: 80);
        var equalConstraints = new BoxConstraints(maxWidth: 100, maxHeight: 80);
        Require(box.getDryLayout(constraints).Equals(box.getDryLayout(equalConstraints)) && box.DryCalls == 1,
            "dry cache uses constraint equality and virtual compute");
        Require(box.getDryBaseline(constraints, TextBaseline.alphabetic) is null &&
            box.getDryBaseline(equalConstraints, TextBaseline.alphabetic) is null && box.BaselineCalls == 1,
            "null baseline is a cached result");
        box.getDryBaseline(constraints, TextBaseline.ideographic);
        Require(box.BaselineCalls == 2, "baseline kinds have independent caches");
        box.markNeedsLayout();
        box.getDryLayout(equalConstraints);
        box.getDryBaseline(equalConstraints, TextBaseline.alphabetic);
        box.getMinIntrinsicWidth(20);
        Require(box.DryCalls == 2 && box.BaselineCalls == 3 && box.IntrinsicCalls == 17,
            "dirty with equal constraints invalidates all cache kinds");

        var exceptional = new CacheProbe { ThrowOnce = true };
        try { exceptional.getMinIntrinsicWidth(7); throw new Exception("Expected compute failure"); }
        catch (ApplicationException) { }
        Require(exceptional.getMinIntrinsicWidth(7) == 107 && exceptional.IntrinsicCalls == 2,
            "failed compute leaves no cache entry");
        var reentrant = new CacheProbe { Reenter = true };
        Require(reentrant.getMinIntrinsicWidth(7) == 108 && reentrant.getMinIntrinsicWidth(7) == 108 &&
            reentrant.IntrinsicCalls == 2, "reentrant same-key compute stores the outer result last");

        var element = new AdaptorProbe();
        var child = new CacheProbe();
        element.insertRenderObjectChild(child, 0L);
        element.moveRenderObjectChild(child, 0L, 1L);
        element.removeRenderObjectChild(child, 1L);
        Require(element.Target.Calls.SequenceEqual(new[] { "insert", "move", "remove" }) && element.Reads == 3,
            "typed sliver receiver preserves virtual getter and insert/move/remove overrides");

        VerifyLayoutMerge();
        var measurements = new List<object>();
        foreach (var request in requests)
        {
            request(20); // Populate the single key outside the measured eight requests.
            var before = GC.GetAllocatedBytesForCurrentThread();
            for (var i = 0; i < 8; i++) request(20);
            measurements.Add(new { requests = 8, allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - before });
        }
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        File.WriteAllText(output, JsonSerializer.Serialize(new { intrinsicHitAllocations = measurements },
            new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("PASS bounded CLR allocation capture (not WASM latency proof): " + output);
    }

    private static PropertyInfo Property(Type type, string name) =>
        type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private static void VerifyLayoutMerge()
    {
        var owner = new PipelineOwner();
        var calls = new List<string>();
        var first = new LayoutProbe("first", calls);
        var tail = new LayoutProbe("tail", calls);
        var added = new LayoutProbe("added", calls);
        var queue = Property(typeof(PipelineOwner), "_nodesNeedingLayout");
        foreach (var node in new[] { first, tail, added })
        {
            node.layout(BoxConstraints.CreateTight(new Size(10, 10)));
            node.attach(owner);
        }
        calls.Clear();
        Property(typeof(RenderObject), "_depth").SetValue(first, 0L);
        Property(typeof(RenderObject), "_depth").SetValue(added, 1L);
        Property(typeof(RenderObject), "_depth").SetValue(tail, 2L);
        first.DuringLayout = () =>
        {
            added.markNeedsLayout();
            Property(typeof(PipelineOwner), "_shouldMergeDirtyNodes").SetValue(owner, true);
        };
        first.markNeedsLayout();
        tail.markNeedsLayout();
        owner.flushLayout();
        Require(calls.SequenceEqual(new[] { "first", "added", "tail" }),
            "new dirty node merges before remaining deeper node without duplicate layout");
        Require(((List<RenderObject>)queue.GetValue(owner)!).Count == 0 &&
            ((List<RenderObject>)typeof(PipelineOwner).GetField("_nodesNeedingLayoutScratch",
                BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(owner)!).Count == 0,
            "both layout queues release processed references");
        foreach (var node in new[] { first, tail, added }) node.detach();
        owner.dispose();
    }

    public sealed class CacheProbe : RenderBox
    {
        public int IntrinsicCalls, DryCalls, BaselineCalls;
        public bool ThrowOnce, Reenter;
        public override double computeMinIntrinsicWidth(double height)
        {
            IntrinsicCalls++;
            if (ThrowOnce) { ThrowOnce = false; throw new ApplicationException("intentional compute failure"); }
            if (Reenter) { Reenter = false; return getMinIntrinsicWidth(height) + 1; }
            return 100 + height;
        }
        public override double computeMaxIntrinsicWidth(double height) { IntrinsicCalls++; return 200 + height; }
        public override double computeMinIntrinsicHeight(double width) { IntrinsicCalls++; return 300 + width; }
        public override double computeMaxIntrinsicHeight(double width) { IntrinsicCalls++; return 400 + width; }
        public override Size computeDryLayout(BoxConstraints constraints) { DryCalls++; return new Size(10, 20); }
        public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline) { BaselineCalls++; return null; }
        public override void performLayout() => size = new Size(10, 10);
    }

    public sealed class AdaptorProbe : SliverMultiBoxAdaptorElement
    {
        public readonly SliverProbe Target;
        public int Reads;
        public AdaptorProbe() : base(new SliverList(@delegate: new SliverChildListDelegate(new List<Widget>())))
            => Target = new SliverProbe(this);
        public override RenderSliverMultiBoxAdaptor renderObject { get { Reads++; return Target; } }
    }

    public sealed class SliverProbe(RenderSliverBoxChildManager manager) : RenderSliverList(manager)
    {
        public readonly List<string> Calls = [];
        public override void insert(RenderBox child, RenderBox? after = null) => Calls.Add("insert");
        public override void move(RenderBox child, RenderBox? after = null) => Calls.Add("move");
        public override void remove(RenderBox child) => Calls.Add("remove");
    }

    private sealed class LayoutProbe(string name, List<string> calls) : RenderBox
    {
        public Action? DuringLayout;
        public override void performLayout() { calls.Add(name); DuringLayout?.Invoke(); size = new Size(10, 10); }
    }
}
