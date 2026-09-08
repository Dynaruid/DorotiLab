using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Doroti.Host.Web;

/// <summary>
/// Timers owned by the browser event loop. No System.Threading.Timer/TimerQueue
/// is used, including for TimeProvider-based Task delays and cancellation.
/// </summary>
[SupportedOSPlatform("browser")]
public sealed partial class BrowserTimeProvider : TimeProvider, IDisposable
{
    private static readonly ConcurrentDictionary<int, BrowserTimer> Timers = new();
    private static int _nextId;
    private readonly object _gate = new();
    private readonly int _id = Interlocked.Increment(ref _nextId);
    private readonly int _ownerThreadId = Environment.CurrentManagedThreadId;
    private readonly SynchronizationContext _context = SynchronizationContext.Current
        ?? throw new InvalidOperationException("Browser timers require a JS owner synchronization context.");
    private volatile bool _disposed;
    private static long _created;
    private static long _fired;
    private static long _crossThreadOperations;

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ValidateTimeout(dueTime);
        ValidateTimeout(period);
        BrowserTimer timer;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            timer = new(this, callback, state);
            Timers[timer.Id] = timer;
            Interlocked.Increment(ref _created);
        }
        timer.Change(dueTime, period);
        return timer;
    }

    public void Dispose()
    {
        BrowserTimer[] owned;
        lock (_gate)
        {
            if (_disposed) return;
            _disposed = true;
            owned = Timers.Values.Where(timer => ReferenceEquals(timer.Owner, this)).ToArray();
        }
        foreach (var timer in owned) timer.Dispose();
        // Also clears cancellation work already posted from another thread.
        OnOwner(() => CancelBrowserTimerOwner(_id));
    }

    private void OnOwner(Action action)
    {
        if (Environment.CurrentManagedThreadId == _ownerThreadId) action();
        else
        {
            Interlocked.Increment(ref _crossThreadOperations);
            _context.Post(_ => action(), null);
        }
    }

    private static void ValidateTimeout(TimeSpan value)
    {
        if (value != Timeout.InfiniteTimeSpan && (value < TimeSpan.Zero || value.TotalMilliseconds > uint.MaxValue - 1))
            throw new ArgumentOutOfRangeException(nameof(value));
    }

    [JSExport]
    public static void DispatchTimer(int id, int generation)
    {
        if (Timers.TryGetValue(id, out var timer)) timer.Fire(generation);
    }

    [JSExport]
    public static string CaptureDiagnostics() => JsonSerializer.Serialize(new {
        backend = "browser-owner-timeout",
        ownerThreadId = Environment.CurrentManagedThreadId,
        liveTimers = Timers.Values.Count(timer => timer.Owner._ownerThreadId == Environment.CurrentManagedThreadId),
        created = Interlocked.Read(ref _created),
        fired = Interlocked.Read(ref _fired),
        crossThreadOperations = Interlocked.Read(ref _crossThreadOperations),
        systemTimerCount = global::System.Threading.Timer.ActiveCount,
    });

    [JSImport("scheduleBrowserTimer", "doroti.web")]
    private static partial void ScheduleBrowserTimer(int id, int owner, int generation, double delayMilliseconds);
    [JSImport("cancelBrowserTimer", "doroti.web")]
    private static partial void CancelBrowserTimer(int id);
    [JSImport("cancelBrowserTimerOwner", "doroti.web")]
    private static partial void CancelBrowserTimerOwner(int owner);

    private sealed class BrowserTimer : ITimer
    {
        private readonly object _gate = new();
        private readonly TimerCallback _callback;
        private readonly object? _state;
        private readonly ExecutionContext? _executionContext = ExecutionContext.Capture();
        private bool _disposed;
        private int _generation;
        private long _dueAt;
        private TimeSpan _period;
        private bool _armed;
        private bool _callbackRunning;
        private TaskCompletionSource? _callbackCompleted;

        internal BrowserTimer(BrowserTimeProvider owner, TimerCallback callback, object? state)
        {
            Owner = owner;
            _callback = callback;
            _state = state;
        }

        internal int Id { get; } = Interlocked.Increment(ref _nextId);
        internal BrowserTimeProvider Owner { get; }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            ValidateTimeout(dueTime);
            ValidateTimeout(period);
            int generation;
            lock (_gate)
            {
                if (_disposed || Owner._disposed) return false;
                generation = ++_generation;
                _period = period;
                _armed = dueTime != Timeout.InfiniteTimeSpan;
                _dueAt = _armed ? Owner.GetTimestamp() + (long)(dueTime.TotalSeconds * Owner.TimestampFrequency) : 0;
            }
            Owner.OnOwner(() => Arm(generation));
            return true;
        }

        private void Arm(int generation)
        {
            lock (_gate)
            {
                if (_disposed || Owner._disposed || _generation != generation) return;
                CancelBrowserTimer(Id);
                if (_armed)
                    ScheduleBrowserTimer(Id, Owner._id, generation, Math.Max(0,
                        (_dueAt - Owner.GetTimestamp()) * 1000.0 / Owner.TimestampFrequency));
            }
        }

        internal void Fire(int generation)
        {
            if (Environment.CurrentManagedThreadId != Owner._ownerThreadId)
                throw new InvalidOperationException("Browser timer left its JS owner.");
            lock (_gate)
            {
                if (_disposed || Owner._disposed || !_armed || _generation != generation) return;
                var now = Owner.GetTimestamp();
                if (now < _dueAt)
                {
                    // setTimeout is clamped to an int32 interval. Long timers
                    // are rearmed without firing early.
                    Arm(generation);
                    return;
                }
                _armed = _period > TimeSpan.Zero;
                if (_armed) _dueAt = now + (long)(_period.TotalSeconds * Owner.TimestampFrequency);
                _callbackRunning = true;
            }
            try
            {
                Arm(generation);
                Interlocked.Increment(ref _fired);
                if (_executionContext is { } context)
                    ExecutionContext.Run(context.CreateCopy(), _ => _callback(_state), null);
                else _callback(_state);
            }
            finally
            {
                lock (_gate)
                {
                    _callbackRunning = false;
                    _callbackCompleted?.TrySetResult();
                }
            }
        }

        public void Dispose()
        {
            lock (_gate)
            {
                if (_disposed) return;
                _disposed = true;
                _armed = false;
                _generation++;
            }
            Timers.TryRemove(Id, out _);
            Owner.OnOwner(() => CancelBrowserTimer(Id));
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            lock (_gate)
            {
                if (!_callbackRunning) return ValueTask.CompletedTask;
                _callbackCompleted ??= new(TaskCreationOptions.RunContinuationsAsynchronously);
                return new ValueTask(_callbackCompleted.Task);
            }
        }
    }
}
