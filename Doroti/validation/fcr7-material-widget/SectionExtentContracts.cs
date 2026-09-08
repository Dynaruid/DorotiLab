using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;

internal static class SectionExtentContracts
{
    internal static void Verify()
    {
        foreach (var count in new[] { 29, 290, 4096 })
        {
            var index = new SectionExtentIndex(count);
            index.Configure(400, 1);
            var oracle = Enumerable.Repeat(300d, count).ToArray();
            var random = new Random(42);
            for (var action = 0; action < 1000; action++)
            {
                var item = random.Next(count);
                oracle[item] = random.Next(0, 1000) / 4d;
                index.Measure(item, oracle[item]);
                var boundary = random.Next(count + 1);
                if (Math.Abs(index.Prefix(boundary) - oracle.Take(boundary).Sum()) > 1e-8)
                    throw new Exception("Section prefix differs from exact sum");
                var offset = random.NextDouble() * oracle.Sum();
                var found = 0; double sum = 0;
                while (found < count - 1 && sum + oracle[found] <= offset) sum += oracle[found++];
                if (index.Find(offset) != found) throw new Exception("Section lookup differs from linear oracle");
            }
            var original = index.Total;
            index.Configure(400.001, 1);
            if (index.State(0) == SectionExtentIndex.Measurement.Measured) throw new Exception("Rounded width reused measured geometry");
            index.Measure(0, 1);
            index.Configure(400, 1);
            if (index.Total != original) throw new Exception("Exact width cache did not restore");
            for (var width = 401; width < 500; width++) index.Configure(width, 2);
            if (index.CachedConfigurations != 2 || index.NumericStorageBytes > count * 40L + 16)
                throw new Exception("Height cache exceeded its storage bound");
            index.Configure(499, 3);
            if (index.State(0) == SectionExtentIndex.Measurement.Measured) throw new Exception("Metric revision reused measured height");
            Console.WriteLine($"SECTION_INDEX count={count} randomized=1000 configurations={index.CachedConfigurations} bytes={index.NumericStorageBytes}");
        }
    }
}

internal sealed class SectionLifetimeProbe(Widget child, Action disposed) : StatefulWidget
{
    internal Widget Child => child;
    internal Action Disposed => disposed;
    public override IState createState() => new ProbeState();
    private sealed class ProbeState : State<SectionLifetimeProbe>
    {
        public override Widget build(BuildContext context) => widget.Child;
        public override void dispose() { widget.Disposed(); base.dispose(); }
    }
}
