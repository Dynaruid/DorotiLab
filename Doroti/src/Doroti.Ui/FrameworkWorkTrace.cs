using System.Runtime.CompilerServices;

namespace Doroti.Ui;

/// <summary>Explicit, thread-owned contract capture; disabled during performance runs.</summary>
public static class FrameworkWorkTrace
{
    public enum Kind { Enqueue, Build, UpdateChild, Dependency, Activate, Deactivate, Unmount,
        Layout, LayoutFastReturn, PerformLayout, PerformResize, MarkLayout, DidUpdateWidget, RegisterDependency, RemoveDependency }
    public readonly record struct Entry(int Tick, Kind Kind, int Node, int Related, long Flags,
        double A, double B, double C, double D);
    public sealed record Node(int Id, int LogicalId, string Type);
    public sealed record Capture(bool Valid, long Dropped, Node[] Nodes, Entry[] Events);
    private sealed record Identity(int Id);
    private sealed class Buffer(int capacity)
    {
        public readonly Entry[] Events = new Entry[capacity];
        public readonly ConditionalWeakTable<object, Identity> Ids = new();
        public readonly List<(int LogicalId, Type Type)> Nodes = [];
        public int Count, Tick;
        public long Dropped;
    }
    [ThreadStatic] private static Buffer? _buffer;
    public static bool Enabled => _buffer is not null;
    public static void Start(int capacity = 65536)
    {
        if (capacity is < 1 or > 1048576) throw new ArgumentOutOfRangeException(nameof(capacity));
        if (Enabled) throw new InvalidOperationException("A contract capture is already active.");
        _buffer = new(capacity);
    }
    public static void SetTick(int tick) { if (_buffer is { } b) b.Tick = tick; }
    public static void Register(object node, int logicalId)
    {
        if (_buffer is not { } b) throw new InvalidOperationException("Start capture before registering nodes.");
        if (logicalId <= 0 || b.Nodes.Any(n => n.LogicalId == logicalId))
            throw new ArgumentException("Logical IDs must be positive and unique.", nameof(logicalId));
        var id = Id(node);
        if (id != 0)
        {
            if (b.Nodes[id - 1].LogicalId != 0) throw new InvalidOperationException("A node already has a logical ID.");
            b.Nodes[id - 1] = (logicalId, b.Nodes[id - 1].Type);
        }
    }
    private static int Id(object? node)
    {
        if (node is null || _buffer is not { } b) return 0;
        if (b.Ids.TryGetValue(node, out var identity)) return identity.Id;
        if (b.Nodes.Count == b.Events.Length) { b.Dropped++; return 0; }
        var id = b.Nodes.Count + 1;
        b.Ids.Add(node, new(id));
        b.Nodes.Add((0, node.GetType()));
        return id;
    }
    public static void Record(Kind kind, object node, object? related = null, long flags = 0,
        double a = 0, double b = 0, double c = 0, double d = 0)
    {
        if (_buffer is not { } buffer) return;
        if (buffer.Count == buffer.Events.Length) { buffer.Dropped++; return; }
        buffer.Events[buffer.Count++] = new(buffer.Tick, kind, Id(node), Id(related), flags, a, b, c, d);
    }
    public static Capture Stop()
    {
        var b = _buffer ?? throw new InvalidOperationException("No active capture.");
        _buffer = null;
        return new(b.Dropped == 0, b.Dropped, b.Nodes.Select((n, i) => new Node(i + 1, n.LogicalId, n.Type.FullName ?? n.Type.Name)).ToArray(), b.Events[..b.Count]);
    }
}
