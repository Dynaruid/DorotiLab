using Doroti.Runtime;
using Timer = Doroti.Runtime.Timer;

var canceledQueue = new DartMicrotaskQueue();
var canceledCalls = 0;
using (DartAsyncRuntime.enterMicrotaskScheduler(canceledQueue.enqueue))
{
    using var timer = new Timer(Duration.Create(milliseconds: 1), () => canceledCalls++);
    WaitForQueuedCallback(canceledQueue, "canceled timer callback");
    timer.cancel();
    canceledQueue.drain();
}

Require(canceledCalls == 0, "Cancel must suppress a callback already queued for the host event loop.");

var completedQueue = new DartMicrotaskQueue();
var completedCalls = 0;
using (DartAsyncRuntime.enterMicrotaskScheduler(completedQueue.enqueue))
{
    using var timer = new Timer(Duration.Create(milliseconds: 1), () => completedCalls++);
    WaitForQueuedCallback(completedQueue, "active timer callback");
    Require(timer.isActive, "A one-shot timer must remain active until its queued callback begins.");
    completedQueue.drain();
    Require(!timer.isActive, "A one-shot timer must become inactive when its callback begins.");
}

Require(completedCalls == 1, "An active one-shot timer callback must run exactly once.");

var clock = new ManualTimeProvider();
var ownerQueue = new DartMicrotaskQueue();
using (DartAsyncRuntime.enterTimeProvider(clock))
using (DartAsyncRuntime.enterMicrotaskScheduler(ownerQueue.enqueue))
{
    var calls = 0;
    using var timer = new Timer(Duration.Create(milliseconds: 10), () => calls++);
    clock.Advance(TimeSpan.FromMilliseconds(9));
    Require(ownerQueue.count == 0 && calls == 0, "A host timer cannot run before its deadline.");
    clock.Advance(TimeSpan.FromMilliseconds(1));
    Require(ownerQueue.count == 1 && calls == 0, "The timer uses the captured owner queue.");
    ownerQueue.drain();
    Require(calls == 1 && !timer.isActive, "The host timer fires once and releases its handle.");

    var canceled = new Timer(Duration.zero, () => calls += 100);
    clock.Advance(TimeSpan.Zero);
    canceled.cancel();
    ownerQueue.drain();
    Require(calls == 1, "Cancel suppresses an already queued host timer callback.");

    Timer.run(() => calls++);
    Require(calls == 1, "Timer.run is an event, not an inline/ThreadPool callback.");
    clock.Advance(TimeSpan.Zero);
    ownerQueue.drain();
    Require(calls == 2, "Timer.run uses the host timer provider.");

    using var periodic = Timer.periodic(Duration.Create(milliseconds: 5), t => { calls++; t.cancel(); });
    clock.Advance(TimeSpan.FromMilliseconds(5));
    ownerQueue.drain();
    clock.Advance(TimeSpan.FromMilliseconds(50));
    ownerQueue.drain();
    Require(calls == 3, "A periodic timer can cancel itself without another callback.");

    var delayed = new Future(Duration.Create(milliseconds: 10));
    var generic = new Future<int>(Duration.Create(milliseconds: 10));
    Require(!delayed.asTask().IsCompleted && !generic.asTask().IsCompleted, "Timed futures wait for host time.");
    clock.Advance(TimeSpan.FromMilliseconds(10));
    Require(SpinWait.SpinUntil(() => delayed.asTask().IsCompleted && generic.asTask().IsCompleted,
        TimeSpan.FromSeconds(5)), "Both Future delays use host time.");
    var completer = new Completer<int>();
    var timed = completer.future.timeout(Duration.Create(milliseconds: 10), (Func<object>)(() => 77));
    clock.Advance(TimeSpan.FromMilliseconds(10));
    WaitForQueuedCallback(ownerQueue, "timeout recovery callback");
    ownerQueue.drain();
    Require(SpinWait.SpinUntil(() => timed.asTask().IsCompleted, TimeSpan.FromSeconds(5)), "Future timeout uses host time.");
    Require(timed.asTask().GetAwaiter().GetResult() == 77, "Future timeout returns the recovery value.");
    completer.complete(42);
}
Require(ReferenceEquals(DartAsyncRuntime.timeProvider, TimeProvider.System), "Time provider scope restores the native default.");
Require(clock.LiveTimers == 0, "Completed, canceled and timed-out operations release timer handles.");
var nestedQueue = new DartMicrotaskQueue();
var wakes = 0;
var nestedCalls = 0;
using (DartAsyncRuntime.enterTimeProvider(clock))
using (DartAsyncRuntime.enterMicrotaskScheduler(action => { wakes++; nestedQueue.enqueue(action); }))
{
    nestedQueue.enqueue(() => { _ = new Timer(Duration.Create(milliseconds: 5), () => nestedCalls++); });
    nestedQueue.drain();
    clock.Advance(TimeSpan.FromMilliseconds(5));
    Require(wakes == 1 && nestedCalls == 0, "A timer created by a microtask retains the host wakeup scheduler.");
    nestedQueue.drain();
    Require(nestedCalls == 1, "The nested timer runs on its host pump.");
}
Console.WriteLine("Runtime async contract: PASS");

static void WaitForQueuedCallback(DartMicrotaskQueue queue, string name)
{
    Require(SpinWait.SpinUntil(() => queue.count > 0, TimeSpan.FromSeconds(5)), $"Timed out waiting for {name}.");
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

sealed class ManualTimeProvider : TimeProvider
{
    private readonly List<ManualTimer> _timers = [];
    private long _ticks;
    public int LiveTimers => _timers.Count;
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;
    public override long GetTimestamp() => _ticks;
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = new ManualTimer(this, callback, state);
        _timers.Add(timer);
        timer.Change(dueTime, period);
        return timer;
    }
    public void Advance(TimeSpan amount)
    {
        _ticks += amount.Ticks;
        foreach (var timer in _timers.ToArray()) timer.Fire();
    }
    private sealed class ManualTimer(ManualTimeProvider owner, TimerCallback callback, object? state) : ITimer
    {
        private long? _due;
        private TimeSpan _period;
        private bool _disposed;
        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_disposed) return false;
            _due = dueTime == Timeout.InfiniteTimeSpan ? null : owner._ticks + dueTime.Ticks;
            _period = period;
            return true;
        }
        public void Fire()
        {
            if (_disposed || _due is null || _due > owner._ticks) return;
            _due = _period > TimeSpan.Zero ? owner._ticks + _period.Ticks : null;
            callback(state);
        }
        public void Dispose() { _disposed = true; owner._timers.Remove(this); }
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }
}
