using Doroti.Composition;
using Doroti.Rendering;

namespace Doroti.Engine;

public sealed class FrameLifecyclePort : IFrameLifecyclePort
{
    private readonly object _gate = new();
    private readonly IFrameScheduler _scheduler;
    private readonly Dictionary<int, Action<TimeSpan>> _frameCallbacks = [];
    private readonly List<Action<TimeSpan>> _persistentFrameCallbacks = [];
    private readonly Queue<Action<TimeSpan>> _postFrameCallbacks = [];
    private readonly Queue<Action> _microtasks = [];
    private readonly Queue<Action> _buildDirty = [];
    private readonly Queue<Action> _layoutDirty = [];
    private readonly Queue<Action> _paintDirty = [];
    private readonly PriorityQueue<ScheduledTask, (int Priority, long Sequence)> _tasks = new();
    private int _nextCallbackId;
    private long _nextTaskSequence;

    public FrameLifecyclePort(FrameSchedulerPort scheduler)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        scheduler.BeginFrame += BeginFrame;
    }

    public SchedulerPhase Phase { get; private set; }

    public int ScheduleFrameCallback(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        int id;
        lock (_gate)
        {
            id = ++_nextCallbackId;
            _frameCallbacks.Add(id, callback);
        }
        _scheduler.ScheduleFrame();
        return id;
    }

    public void CancelFrameCallback(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        lock (_gate)
        {
            _frameCallbacks.Remove(id);
        }
    }

    public void AddPersistentFrameCallback(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            _persistentFrameCallbacks.Add(callback);
        }
    }

    public void AddPostFrameCallback(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            _postFrameCallbacks.Enqueue(callback);
        }
    }

    public void ScheduleMicrotask(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            _microtasks.Enqueue(callback);
        }
    }

    public ValueTask<T> ScheduleTask<T>(Func<T> callback, int priority)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var task = new ScheduledTask<T>(callback);
        lock (_gate)
        {
            _tasks.Enqueue(task, (-priority, _nextTaskSequence++));
        }
        _scheduler.ScheduleFrame();
        return new(task.Completion.Task);
    }

    public void MarkBuildDirty(Action callback) => EnqueueDirty(_buildDirty, callback);

    public void MarkLayoutDirty(Action callback) => EnqueueDirty(_layoutDirty, callback);

    public void MarkPaintDirty(Action callback) => EnqueueDirty(_paintDirty, callback);

    private void BeginFrame(TimeSpan timestamp)
    {
        Action<TimeSpan>[] callbacks;
        Action<TimeSpan>[] persistentCallbacks;
        Action<TimeSpan>[] postFrameCallbacks;
        lock (_gate)
        {
            callbacks = _frameCallbacks.OrderBy(item => item.Key).Select(item => item.Value).ToArray();
            _frameCallbacks.Clear();
            persistentCallbacks = _persistentFrameCallbacks.ToArray();
            postFrameCallbacks = _postFrameCallbacks.ToArray();
            _postFrameCallbacks.Clear();
        }
        try
        {
            DrainTasks();
            Phase = SchedulerPhase.TransientCallbacks;
            foreach (var callback in callbacks)
            {
                callback(timestamp);
            }
            Phase = SchedulerPhase.MidFrameMicrotasks;
            Drain(_microtasks);
            Phase = SchedulerPhase.PersistentCallbacks;
            Drain(_buildDirty);
            Drain(_layoutDirty);
            Drain(_paintDirty);
            foreach (var callback in persistentCallbacks)
            {
                callback(timestamp);
            }
            Phase = SchedulerPhase.PostFrameCallbacks;
            foreach (var callback in postFrameCallbacks)
            {
                callback(timestamp);
            }
        }
        finally
        {
            Phase = SchedulerPhase.Idle;
        }
    }

    private void EnqueueDirty(Queue<Action> queue, Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            queue.Enqueue(callback);
        }
        _scheduler.ScheduleFrame();
    }

    private void Drain(Queue<Action> queue)
    {
        while (true)
        {
            Action? callback;
            lock (_gate)
            {
                callback = queue.Count == 0 ? null : queue.Dequeue();
            }
            if (callback is null)
            {
                return;
            }
            callback();
        }
    }

    private void DrainTasks()
    {
        while (true)
        {
            ScheduledTask? task;
            lock (_gate)
            {
                task = _tasks.Count == 0 ? null : _tasks.Dequeue();
            }
            if (task is null)
            {
                return;
            }
            task.Invoke();
        }
    }

    private abstract class ScheduledTask
    {
        public abstract void Invoke();
    }

    private sealed class ScheduledTask<T>(Func<T> callback) : ScheduledTask
    {
        public TaskCompletionSource<T> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override void Invoke()
        {
            try
            {
                Completion.TrySetResult(callback());
            }
            catch (Exception exception)
            {
                Completion.TrySetException(exception);
            }
        }
    }
}
