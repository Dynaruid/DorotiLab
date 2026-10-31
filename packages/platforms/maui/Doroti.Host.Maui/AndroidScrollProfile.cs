#if ANDROID
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Skia.Vulkan;
using Doroti.Ui;

namespace Doroti.Host.Maui;

// UI-thread-owned, opt-in metadata only. No logging, serialization or tree
// snapshots in the measured interval. Export explicitly after gestures stop.
internal static class AndroidScrollProfile
{
    internal static readonly bool Enabled = Environment.GetEnvironmentVariable("DOROTI_SCROLL_PROFILE") == "1";
    private const int Capacity = 4096;
    private static readonly Frame[] Frames = Enabled ? new Frame[Capacity] : [];
    private static readonly Input[] Inputs = Enabled ? new Input[Capacity] : [];
    private static readonly Pulse[] Pulses = Enabled ? new Pulse[Capacity] : [];
    private static long _frameCount, _inputCount, _pulseCount, _gesture, _vsync;
    private static long _started, _allocated;
    private static double _callbackWait, _callback, _snap, _insert, _submit;
    private static WeakReference<DorotiFrameTrace>? _trace;
    private static long _magnifiersCreated, _magnifierUpdates, _magnifiersDismissed;
    internal static void RecordMagnifierCreated() { if (Enabled) _magnifiersCreated++; }
    internal static void RecordMagnifierUpdate() { if (Enabled) _magnifierUpdates++; }
    internal static void RecordMagnifierDismissed() { if (Enabled) _magnifiersDismissed++; }
    internal static readonly Action<string, double> Measure = (stage, ms) =>
    {
        if (stage == "callback-wait") _callbackWait = ms;
        if (stage == "callback") _callback = ms;
        if (stage == "snap") _snap = ms;
        if (stage == "insert") _insert = ms;
        if (stage == "submit") _submit = ms;
    };

    internal static void Attach(DorotiFrameTrace trace)
    {
        if (!Enabled) return;
        trace.MeasureRecordingTime = true;
        _trace = new(trace);
    }

    internal static void RecordPulse(long nanos, bool pending, bool painting, bool invalidated, bool touch)
    {
        if (!Enabled) return;
        _vsync = nanos;
        Pulses[_pulseCount++ % Capacity] = new(Now, nanos,
            Math.Max(0, Java.Lang.JavaSystem.NanoTime() - nanos) / 1000, pending, painting, invalidated, touch);
    }

    internal static void RecordInput(Android.Views.MotionEvent e, double dispatchMs)
    {
        if (!Enabled) return;
        if (e.ActionMasked == Android.Views.MotionEventActions.Down) _gesture++;
        Inputs[_inputCount++ % Capacity] = new(Now, _gesture, (int)e.ActionMasked,
            e.GetPointerId(e.ActionIndex), e.EventTime, Android.OS.SystemClock.UptimeMillis(),
            e.HistorySize, e.GetX(), e.GetY(), dispatchMs);
    }

    internal static void BeginDraw()
    {
        if (!Enabled) return;
        _started = Stopwatch.GetTimestamp();
        _allocated = GC.GetAllocatedBytesForCurrentThread();
        _callbackWait = _callback = _snap = _insert = _submit = 0;
    }

    internal static void EndDraw(GraphiteVulkanWindow? window, MauiSkiaPaintContext? paint,
        bool submitted, string reason)
    {
        if (!Enabled) return;
        Frames[_frameCount++ % Capacity] = new(Now, _gesture, _vsync, submitted, reason,
            paint?.Completion?.Descriptor.FrameworkFrameNumber ?? 0, paint?.Completion?.SceneSequence ?? 0,
            window?.SubmittedWindowFrames ?? 0, window?.WindowFramesInFlight ?? 0,
            window?.BusyWindowFrames ?? 0, window?.UnavailableWindowImages ?? 0,
            Stopwatch.GetElapsedTime(_started).TotalMilliseconds, _callbackWait, _callback, _snap, _insert, _submit,
            submitted ? window?.LastFrameTiming ?? default : default,
            GC.GetAllocatedBytesForCurrentThread() - _allocated,
            GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2));
    }

    internal static void Export(Android.Content.Context context)
    {
        if (!Enabled) return;
        var trace = _trace is not null && _trace.TryGetTarget(out var value) ? value.Snapshot().ToArray() : [];
        var report = new Report(false, _frameCount, _inputCount, _pulseCount,
            Copy(Frames, _frameCount), Copy(Inputs, _inputCount), Copy(Pulses, _pulseCount),
            trace, FrameworkWorkProfile.CaptureEntries(), GC.GetTotalAllocatedBytes(), GC.GetTotalMemory(false),
            new(_magnifiersCreated, _magnifierUpdates, _magnifiersDismissed));
        var path = System.IO.Path.Combine(context.GetExternalFilesDir(null)!.AbsolutePath, "doroti-scroll-profile.json");
        File.WriteAllText(path, JsonSerializer.Serialize(report, AndroidScrollJsonContext.Default.Report));
        Android.Util.Log.Info("DorotiScrollProfile", $"export={path} frames={_frameCount} inputs={_inputCount} pulses={_pulseCount}");
    }

    private static long Now => DorotiFrameClock.Now.Ticks / 10;
    private static T[] Copy<T>(T[] ring, long count)
    {
        var result = new T[(int)Math.Min(count, Capacity)];
        for (var i = 0; i < result.Length; i++) result[i] = ring[(Math.Max(0, count - Capacity) + i) % Capacity];
        return result;
    }

    internal readonly record struct Frame(long AtUs, long Gesture, long VsyncNanos, bool Submitted, string Reason,
        long FrameworkFrame, long Scene, long Submission, int InFlight, long Busy, long Unavailable,
        double OwnerMs, double CallbackWaitMs, double CallbackMs, double SnapMs, double InsertMs, double SubmitMs, VulkanWindowFrameTiming Vulkan,
        long Allocated, int Gen0, int Gen1, int Gen2);
    internal readonly record struct Input(long AtUs, long Gesture, int Action, int Pointer, long EventMs,
        long ArrivalMs, int History, float X, float Y, double DispatchMs);
    internal readonly record struct Pulse(long AtUs, long NativeNanos, long AgeUs,
        bool Pending, bool Painting, bool Invalidated, bool Touch);
    internal sealed record Report(bool PresentObserved, long FrameCount, long InputCount, long PulseCount,
        Frame[] Frames, Input[] Inputs, Pulse[] Pulses, DorotiFrameTraceEntry[] Trace,
        FrameworkWorkProfile.Entry[] Work, long TotalAllocated, long ManagedHeap, MagnifierEvents Magnifier);
    internal readonly record struct MagnifierEvents(long Created, long Updates, long Dismissed);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(AndroidScrollProfile.Report))]
internal sealed partial class AndroidScrollJsonContext : JsonSerializerContext;
#endif
