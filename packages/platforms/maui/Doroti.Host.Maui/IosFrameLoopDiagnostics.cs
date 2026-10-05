using System.Diagnostics;

namespace Doroti.Host.Maui;

public sealed record IosFrameLoopEvent(string Phase, long FrameId, long SceneId, long Generation,
    double TimestampMicroseconds, double DurationMilliseconds, int Pending, string? Reason,
    long InputSequence, long FrameworkFrameNumber);

public sealed record IosFrameLoopSnapshot(string Policy, string Runtime, long OmittedEvents,
    int MaximumPending,
    IReadOnlyList<IosFrameLoopEvent> Events);

internal sealed class IosFrameLoopDiagnostics(string policy, double mediaOrigin)
{
    private const int Capacity = 65536;
    private readonly object _gate = new();
    private readonly List<IosFrameLoopEvent> _events = new();
    private readonly long _origin = Stopwatch.GetTimestamp();
    private long _omitted;
    private int _maximumPending;
    internal void Record(string phase, long frame, long generation, int pending = 0,
        MauiPaintCompletion? completion = null, string? reason = null, double duration = 0,
        double presentedTime = 0)
    {
        var timestamp = presentedTime > 0 ? (presentedTime - mediaOrigin) * 1_000_000
            : Stopwatch.GetElapsedTime(_origin).TotalMicroseconds;
        lock (_gate)
        {
            _maximumPending = Math.Max(_maximumPending, pending);
            if (_events.Count == Capacity) { _omitted++; return; }
            _events.Add(new(phase, frame, completion?.SceneSequence ?? 0, generation,
                timestamp, duration, pending, reason, completion?.InputSequence ?? 0,
                completion?.Descriptor.FrameworkFrameNumber ?? 0));
        }
    }

    internal IosFrameLoopSnapshot Snapshot()
    {
        lock (_gate) return new(policy, System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            _omitted, _maximumPending, _events.ToArray());
    }
}
