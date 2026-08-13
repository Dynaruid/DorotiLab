namespace Doroti.Core;

/// <summary>Monotonic time used by frame scheduling and testable runtime services.</summary>
public interface IClock
{
    TimeSpan Now { get; }
}

/// <summary>Schedules a callback for the next engine frame without prescribing a platform loop.</summary>
public interface IFrameDispatcher
{
    void ScheduleFrame(Action<TimeSpan> callback);
}

public interface IUiDispatcher
{
    bool CheckAccess();

    void Post(Action callback);
}

public sealed class QueuedUiDispatcher : IUiDispatcher
{
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly System.Collections.Concurrent.ConcurrentQueue<Action> _work = new();

    public event Action? WorkScheduled;

    public int PendingCount => _work.Count;

    public bool CheckAccess() => Environment.CurrentManagedThreadId == _threadId;

    public void Post(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        _work.Enqueue(callback);
        WorkScheduled?.Invoke();
    }

    public int Drain()
    {
        if (!CheckAccess())
        {
            throw new InvalidOperationException("UI work must be drained on the owning thread.");
        }
        var count = 0;
        while (_work.TryDequeue(out var callback))
        {
            callback();
            count++;
        }
        return count;
    }
}
