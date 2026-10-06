using System.Runtime.InteropServices;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

// Opt-in wall/CPU timing around the actual application-owner rendezvous.
// Keeps recent frames plus bounded slow-frame receipts so a later quiet period
// cannot erase an intermittent scheduling stall from the exit report.
internal sealed partial class WindowsFrameworkFrameTiming
{
    private readonly object _gate = new();
    private Sample[]? _recent, _slow;
    private long _frames, _slowFrames, _maximumQueue, _maximumWork, _maximumResume;
    private string? _ownerPriority;
    internal bool Enabled { get; } = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_FRAME_TIMING") == "1";

    internal sealed class Measurement(long frameId, int inputCount)
    {
        internal readonly long FrameId = frameId;
        internal readonly int InputCount = inputCount;
        internal readonly long QueuedAt = Now;
        internal readonly long GcAt = GC.GetTotalPauseDuration().Ticks / 10;
        internal long UiStart, UiEnd, CpuStart, CpuUsed;
        internal string? Priority;
        internal void EnterUi()
        {
            UiStart = Now;
            CpuStart = CpuTime();
            Priority = Thread.CurrentThread.Priority.ToString();
        }
        internal void ExitUi()
        {
            CpuUsed = CpuTime() - CpuStart;
            UiEnd = Now;
        }
    }

    internal readonly record struct Sample(long FrameId, long TimestampMicroseconds, int InputCount,
        long UiQueueMicroseconds, long UiWorkMicroseconds, long UiCpuMicroseconds,
        long CompletionResumeMicroseconds, long GcPauseMicroseconds);

    internal void Record(Measurement measurement)
    {
        var resume = Now;
        if (measurement.UiStart == 0 || measurement.UiEnd == 0) return;
        var sample = new Sample(measurement.FrameId, measurement.QueuedAt, measurement.InputCount,
            Math.Max(0, measurement.UiStart - measurement.QueuedAt),
            Math.Max(0, measurement.UiEnd - measurement.UiStart), Math.Max(0, measurement.CpuUsed),
            Math.Max(0, resume - measurement.UiEnd),
            Math.Max(0, GC.GetTotalPauseDuration().Ticks / 10 - measurement.GcAt));
        lock (_gate)
        {
            _ownerPriority = measurement.Priority;
            _recent ??= new Sample[2048];
            _recent[_frames++ % _recent.Length] = sample;
            _maximumQueue = Math.Max(_maximumQueue, sample.UiQueueMicroseconds);
            _maximumWork = Math.Max(_maximumWork, sample.UiWorkMicroseconds);
            _maximumResume = Math.Max(_maximumResume, sample.CompletionResumeMicroseconds);
            if (sample.UiQueueMicroseconds + sample.UiWorkMicroseconds + sample.CompletionResumeMicroseconds >= 16_667)
            {
                _slow ??= new Sample[256];
                _slow[_slowFrames++ % _slow.Length] = sample;
            }
        }
    }

    internal object Snapshot()
    {
        lock (_gate)
        {
            Sample[] Copy(Sample[]? ring, long count) => ring is null ? [] : Enumerable.Range(0, (int)Math.Min(count, ring.Length))
                .Select(i => ring[(Math.Max(0, count - ring.Length) + i) % ring.Length]).ToArray();
            return new
            {
                enabled = Enabled, frames = _frames, slowFrames = _slowFrames, ownerPriority = _ownerPriority,
                maximumUiQueueMicroseconds = _maximumQueue, maximumUiWorkMicroseconds = _maximumWork,
                maximumCompletionResumeMicroseconds = _maximumResume,
                recent = Copy(_recent, _frames), slow = Copy(_slow, _slowFrames),
            };
        }
    }

    private static long Now => DorotiFrameClock.Now.Ticks / 10;
    private static long CpuTime() => GetThreadTimes(GetCurrentThread(), out _, out _, out var kernel, out var user)
        ? checked((long)((kernel + user) / 10)) : 0;
    [LibraryImport("kernel32.dll")]
    private static partial nint GetCurrentThread();
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetThreadTimes(nint thread, out ulong created, out ulong exited, out ulong kernel, out ulong user);
}
