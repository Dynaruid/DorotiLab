namespace Doroti.Ui;

/// <summary>Bounded cost attribution, independent of contract event capture.</summary>
public static class FrameworkComponentProfile
{
    public enum Kind { InheritancePut, InheritanceRemove, IntrinsicCache, DryCache, BaselineCache }
    [ThreadStatic] private static long[]? _values;
    [ThreadStatic] private static ulong _visited;
    public readonly struct Scope : IDisposable
    {
        private readonly long _start, _allocated;
        private readonly int _offset;
        private readonly bool _active;
        internal Scope(Kind kind, int items)
        {
            _offset = (int)kind * 4;
            (_values ??= new long[20])[_offset + 3] += items;
            _allocated = GC.GetAllocatedBytesForCurrentThread();
            _start = DorotiFrameClock.Now.Ticks;
            _active = true;
        }
        public void Dispose()
        {
            if (!_active) return;
            var elapsed = DorotiFrameClock.Now.Ticks - _start;
            var allocated = GC.GetAllocatedBytesForCurrentThread() - _allocated;
            _values![_offset]++;
            _values[_offset + 1] += elapsed;
            _values[_offset + 2] += allocated;
        }
    }
    public static Scope Begin(Kind kind, int items = 0) => FrameworkWorkCounters.Enabled ? new(kind, items) : default;
    public static void VisitSection(int id)
    {
        if (FrameworkWorkCounters.Enabled && id is >= 0 and < 64) _visited |= 1UL << id;
    }
    public static object Snapshot() => new {
        names = Enum.GetNames<Kind>(),
        // Per kind: calls, inclusive ticks (100ns), allocated bytes, input item count.
        values = _values?.ToArray() ?? new long[20],
        visitedSections = Enumerable.Range(0, 64).Where(i => (_visited & (1UL << i)) != 0).ToArray(),
    };
}
