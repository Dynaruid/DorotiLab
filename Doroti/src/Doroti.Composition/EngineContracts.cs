using Doroti.Core;

namespace Doroti.Composition;

public readonly record struct FrameId(ulong Value);

public readonly record struct ResourceId(ulong Value);

public enum FramePhase
{
    Idle,
    Scheduled,
    Build,
    Commit,
}

public interface ICommittedScene
{
    FrameId FrameId { get; }

    SurfaceGeneration SurfaceGeneration { get; }
}

public interface IFrameScheduler
{
    bool HasScheduledFrame { get; }

    FramePhase Phase { get; }

    void ScheduleFrame();

    void OnVsync(TimeSpan timestamp);
}

public interface ICompositor
{
    ValueTask SubmitAsync(ICommittedScene scene, CancellationToken cancellationToken = default);
}

public interface IResourceRegistry
{
    IResourceLease Retain(ResourceId resource);
}

public interface IResourceLease : IDisposable
{
    ResourceId Resource { get; }

    IResourceSnapshot Snapshot { get; }
}

public sealed class FrameSchedulerPort(IClock clock, IFrameDispatcher dispatcher) : IFrameScheduler
{
    private bool _rescheduleRequested;

    public bool HasScheduledFrame => Phase is FramePhase.Scheduled;

    public FramePhase Phase { get; private set; }

    public event Action<TimeSpan>? BeginFrame;

    public void ScheduleFrame()
    {
        if (Phase is FramePhase.Build or FramePhase.Commit)
        {
            _rescheduleRequested = true;
            return;
        }
        if (Phase is not FramePhase.Idle)
        {
            return;
        }

        Phase = FramePhase.Scheduled;
        dispatcher.ScheduleFrame(OnVsync);
    }

    public void OnVsync(TimeSpan timestamp)
    {
        if (Phase is not FramePhase.Scheduled)
        {
            return;
        }

        Phase = FramePhase.Build;
        try
        {
            BeginFrame?.Invoke(timestamp == default ? clock.Now : timestamp);
            Phase = FramePhase.Commit;
        }
        finally
        {
            Phase = FramePhase.Idle;
            if (_rescheduleRequested)
            {
                _rescheduleRequested = false;
                ScheduleFrame();
            }
        }
    }
}
