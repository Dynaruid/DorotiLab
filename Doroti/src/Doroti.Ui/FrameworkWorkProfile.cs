namespace Doroti.Ui;

// Opt-in, thread-owned aggregates. Only Type metadata is retained, never an
// Element, State, delegate, or scene. Names are materialized on capture only.
public static class FrameworkWorkProfile
{
    private const int Capacity = 512;
    [ThreadStatic] private static Buffer? _buffer;
    private sealed class Buffer
    {
        public readonly Dictionary<(Type, int), int> Ids = new();
        public readonly long[] Calls = new long[Capacity], Inclusive = new long[Capacity], Self = new long[Capacity];
        public readonly long[] Children = new long[Capacity];
        public readonly long[] StartCalls = new long[Capacity], StartSelf = new long[Capacity], StartInclusive = new long[Capacity];
        public readonly long[] Frames = new long[512 * 22];
        public long FrameCount, BuildSequence;
        public int Depth;
        public long Dropped;
    }
    public readonly struct Scope : IDisposable
    {
        private readonly int _id, _depth;
        private readonly long _start;
        private readonly bool _active;
        internal Scope(int id, int depth) { _id = id; _depth = depth; _start = DorotiFrameClock.Now.Ticks; _active = true; }
        public void Dispose()
        {
            if (!_active || _buffer is not { } buffer) return;
            var elapsed = DorotiFrameClock.Now.Ticks - _start;
            buffer.Depth--;
            buffer.Calls[_id]++;
            buffer.Inclusive[_id] += elapsed;
            buffer.Self[_id] += elapsed - buffer.Children[_depth];
            if (_depth > 0) buffer.Children[_depth - 1] += elapsed;
        }
    }
    public static Scope Begin(Type type, int kind = 0)
    {
        if (!FrameworkWorkCounters.Enabled) return default;
        var buffer = _buffer ??= new Buffer();
        if (buffer.Depth == Capacity) { buffer.Dropped++; return default; }
        if (!buffer.Ids.TryGetValue((type, kind), out var id))
        {
            if (buffer.Ids.Count == Capacity) { buffer.Dropped++; return default; }
            buffer.Ids.Add((type, kind), id = buffer.Ids.Count);
        }
        var depth = buffer.Depth++;
        buffer.Children[depth] = 0;
        return new(id, depth);
    }
    public static void Count(Type type, int kind)
    {
        using var scope = Begin(type, kind);
    }
    internal static void Record(long sequence, DorotiFramePhase phase)
    {
        if (!FrameworkWorkCounters.Enabled) return;
        var buffer = _buffer ??= new Buffer();
        if (phase == DorotiFramePhase.build)
        {
            buffer.BuildSequence = sequence;
            Array.Copy(buffer.Calls, buffer.StartCalls, Capacity);
            Array.Copy(buffer.Self, buffer.StartSelf, Capacity);
            Array.Copy(buffer.Inclusive, buffer.StartInclusive, Capacity);
        }
        if (phase != DorotiFramePhase.layout) return;
        var offset = (int)(buffer.FrameCount++ % 512) * 22;
        Array.Clear(buffer.Frames, offset, 22);
        buffer.Frames[offset] = buffer.BuildSequence;
        buffer.Frames[offset + 1] = sequence;
        for (var id = 0; id < buffer.Ids.Count; id++)
        {
            var self = (buffer.Self[id] - buffer.StartSelf[id]) / 10;
            for (var rank = 0; rank < 5; rank++)
            {
                var slot = offset + 2 + rank * 4;
                if (self <= buffer.Frames[slot + 3]) continue;
                Array.Copy(buffer.Frames, slot, buffer.Frames, slot + 4, (4-rank)*4);
                buffer.Frames[slot] = id;
                buffer.Frames[slot+1] = buffer.Calls[id] - buffer.StartCalls[id];
                buffer.Frames[slot+2] = (buffer.Inclusive[id] - buffer.StartInclusive[id]) / 10;
                buffer.Frames[slot+3] = self;
                break;
            }
        }
    }
    private static long[][] Frames()
    {
        if (_buffer is not { } buffer) return [];
        return Enumerable.Range(0, (int)Math.Min(buffer.FrameCount,512)).Select(index => {
            var offset = (int)((Math.Max(0,buffer.FrameCount-512)+index)%512)*22;
            return buffer.Frames[offset..(offset+22)];
        }).ToArray();
    }
    public sealed record Entry(int Id, string Type, int Kind, long Calls, long InclusiveMicroseconds, long SelfMicroseconds);
    public static object Snapshot() => new {
        frames = Frames(), framesDropped = Math.Max(0, (_buffer?.FrameCount ?? 0) - 512),
        enabled = FrameworkWorkCounters.Enabled, thread = Environment.CurrentManagedThreadId,
        dropped = _buffer?.Dropped ?? 0,
        entries = _buffer?.Ids.Select(pair => new Entry(pair.Value, pair.Key.Item1.FullName ?? pair.Key.Item1.Name,
            pair.Key.Item2, _buffer.Calls[pair.Value], _buffer.Inclusive[pair.Value] / 10, _buffer.Self[pair.Value] / 10)).ToArray() ?? [],
        managedAllocatedBytes = GC.GetTotalAllocatedBytes(), managedHeapBytes = GC.GetTotalMemory(false),
        gcCollections = new[] { GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2) },
    };
}
