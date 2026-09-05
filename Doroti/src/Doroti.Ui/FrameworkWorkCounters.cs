using System.Runtime.CompilerServices;

namespace Doroti.Ui;

// Diagnostic-only cumulative work. No node identity, stacks, strings, or tree
// references are retained. The UI Worker owns one thread-local fixed buffer.
public enum FrameworkWork
{
    BuildEnqueueAttempt, BuildEnqueued, BuildEnqueueDuplicate, BuildDuringFlush,
    BuildSort, BuildResort, Rebuild, ForcedRebuild, MarkBuild, MarkBuildAlreadyDirty,
    SetState, DependencyChanged,
    LayoutEntry, LayoutFastPath, LayoutWork, LayoutDirtySameConstraints,
    MarkLayout, MarkLayoutAlreadyDirty, LayoutBoundary, LayoutParentPropagation,
    MarkPaint, MarkPaintAlreadyDirty, PaintBoundary, PaintParentPropagation,
    RepaintBoundary, NewPicture,
    MediaUpdate, MediaChanged, MediaDependentCheck, MediaDependentNotified, MediaAspectSubscriptions,
    HostSnapshotApply, HostMetricsNotified,
    Count,
}

public static class FrameworkWorkCounters
{
    public static bool Enabled { get; } = Environment.GetEnvironmentVariable("DOROTI_STAGE_TRACE") == "1";
    private const int Capacity = 2048;
    private const int Width = (int)FrameworkWork.Count;
    [ThreadStatic] private static Buffer? _buffer;
    private sealed class Buffer
    {
        public readonly long[] Totals = new long[Width];
        public readonly long[] Values = new long[Capacity * Width];
        public readonly Boundary[] Boundaries = new Boundary[Capacity];
        public long Count;
    }
    public readonly record struct Boundary(long TraceSequence, ulong ViewId, DorotiFramePhase Phase,
        long RecordedAtMicroseconds, long Generation, long Frame, long Scene);
    public sealed record Sample(Boundary Boundary, long[] Totals);
    public sealed record Capture(bool Enabled, long Dropped, string[] Names, Sample[] Samples);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add(FrameworkWork kind, long count = 1)
    {
        if (Enabled) (_buffer ??= new Buffer()).Totals[(int)kind] += count;
    }

    internal static void Record(long sequence, ulong view, DorotiFramePhase phase,
        long generation, long frame, long scene)
    {
        if (!Enabled || phase is not (DorotiFramePhase.metrics or DorotiFramePhase.build or
            DorotiFramePhase.layout or DorotiFramePhase.paint or DorotiFramePhase.sceneBuild or
            DorotiFramePhase.sceneSubmitted or DorotiFramePhase.drawFrame)) return;
        var buffer = _buffer ??= new Buffer();
        var slot = (int)(buffer.Count++ % Capacity);
        buffer.Boundaries[slot] = new(sequence, view, phase, DorotiFrameClock.Now.Ticks / 10, generation, frame, scene);
        Array.Copy(buffer.Totals, 0, buffer.Values, slot * Width, Width);
    }

    public static Capture Snapshot()
    {
        var buffer = _buffer;
        if (buffer is null) return new(Enabled, 0, Enum.GetNames<FrameworkWork>()[..Width], []);
        var count = (int)Math.Min(buffer.Count, Capacity);
        var samples = new Sample[count];
        for (var index = 0; index < count; index++)
        {
            var slot = (int)((Math.Max(0, buffer.Count - Capacity) + index) % Capacity);
            var values = new long[Width];
            Array.Copy(buffer.Values, slot * Width, values, 0, Width);
            samples[index] = new(buffer.Boundaries[slot], values);
        }
        return new(Enabled, Math.Max(0, buffer.Count - Capacity), Enum.GetNames<FrameworkWork>()[..Width], samples);
    }
}
