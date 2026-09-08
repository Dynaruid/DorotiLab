namespace Doroti.Framework.Rendering;

/// <summary>Bounded, exact-width height estimates for an indexed variable-height list.
/// Estimates locate work; only the child's actual layout supplies visible geometry.</summary>
public sealed class SectionExtentIndex
{
    public enum Measurement { Unmeasured, Estimated, Measured, Invalidated }
    private sealed class Configuration(int count, double width, object? revision)
    {
        public readonly double Width = width;
        public readonly object? Revision = revision;
        public readonly double[] Heights = new double[count];
        public readonly double[] Tree = new double[count + 1];
        public readonly Measurement[] States = new Measurement[count];
    }
    private Configuration? _current, _previous;
    private readonly double _estimate;
    public int Count { get; }
    public int CachedConfigurations => _current is null ? 0 : _previous is null ? 1 : 2;
    public long NumericStorageBytes => CachedConfigurations * (Count * 20L + 8);
    public int AnchorIndex { get; internal set; }
    public double AnchorOffset { get; internal set; }
    internal (int Index, double Offset)? RequestedAnchor;
    internal Action? Changed;

    public SectionExtentIndex(int count, double estimatedExtent = 300)
    {
        if (count is < 1 or > 4096) throw new ArgumentOutOfRangeException(nameof(count));
        if (!double.IsFinite(estimatedExtent) || estimatedExtent <= 0) throw new ArgumentOutOfRangeException(nameof(estimatedExtent));
        Count = count; _estimate = estimatedExtent;
    }
    public void RequestItem(int index, double offset = 0)
    {
        if ((uint)index >= Count || !double.IsFinite(offset) || offset < 0) throw new ArgumentOutOfRangeException(nameof(index));
        RequestedAnchor = (index, offset); Changed?.Invoke();
    }
    public void Configure(double width, object? metricRevision)
    {
        if (!double.IsFinite(width) || width < 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (_current is { } same && same.Width == width && Equals(same.Revision, metricRevision)) return;
        if (_previous is { } old && old.Width == width && Equals(old.Revision, metricRevision))
        { (_current, _previous) = (_previous, _current); return; }
        var next = new Configuration(Count, width, metricRevision);
        for (var i = 0; i < Count; i++)
        {
            next.Heights[i] = _current?.Heights[i] ?? _estimate;
            next.States[i] = _current is null ? Measurement.Unmeasured : Measurement.Invalidated;
            Add(next, i, next.Heights[i]);
        }
        _previous = _current; _current = next;
    }
    private static void Add(Configuration config, int index, double delta)
    { for (var i = index + 1; i < config.Tree.Length; i += i & -i) config.Tree[i] += delta; }
    public double Prefix(int count)
    {
        if ((uint)count > Count) throw new ArgumentOutOfRangeException(nameof(count));
        var config = _current ?? throw new InvalidOperationException("Configure the index first.");
        double sum = 0;
        for (var i = count; i > 0; i -= i & -i) sum += config.Tree[i];
        return sum;
    }
    public double Total => Prefix(Count);
    public int Find(double offset)
    {
        if (!double.IsFinite(offset)) throw new ArgumentOutOfRangeException(nameof(offset));
        var config = _current ?? throw new InvalidOperationException("Configure the index first.");
        var index = 0; double sum = 0;
        for (var bit = 4096; bit != 0; bit >>= 1)
        {
            var next = index + bit;
            if (next <= Count && sum + config.Tree[next] <= offset)
            { index = next; sum += config.Tree[next]; }
        }
        return Math.Min(index, Count - 1);
    }
    public Measurement State(int index) => _current!.States[index];
    public void Invalidate(int index)
    {
        if ((uint)index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
        if (_current is { } current) current.States[index] = Measurement.Invalidated;
        if (_previous is { } previous) previous.States[index] = Measurement.Invalidated;
    }
    public void Measure(int index, double extent)
    {
        if ((uint)index >= Count || !double.IsFinite(extent) || extent < 0) throw new ArgumentOutOfRangeException(nameof(extent));
        var config = _current ?? throw new InvalidOperationException("Configure the index first.");
        Add(config, index, extent - config.Heights[index]);
        config.Heights[index] = extent; config.States[index] = Measurement.Measured;
    }
}
