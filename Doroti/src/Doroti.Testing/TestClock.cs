namespace Doroti.Testing;

/// <summary>Virtual time shared by Dart timers and frame timestamps. No wall-clock sleeps.</summary>
public sealed class TestClock : TimeProvider, IDisposable
{
    private readonly List<TestTimer> _timers = [];
    public TimeSpan Elapsed { get; private set; }
    public int PendingTimers => _timers.Count(t => t.Due is not null);
    public override long TimestampFrequency => TimeSpan.TicksPerSecond;
    public override long GetTimestamp() => Elapsed.Ticks;
    public override DateTimeOffset GetUtcNow() => DateTimeOffset.UnixEpoch + Elapsed;
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = new TestTimer(this, callback, state);
        _timers.Add(timer);
        timer.Change(dueTime, period);
        return timer;
    }
    public void Advance(TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(duration, TimeSpan.Zero);
        var end = Elapsed + duration;
        var callbacks = 0;
        while (_timers.Where(t => t.Due <= end).OrderBy(t => t.Due).FirstOrDefault() is { } timer)
        {
            if (++callbacks > 10000) throw new TimeoutException("Virtual timer queue did not quiesce.");
            Elapsed = timer.Due!.Value;
            timer.Fire();
        }
        Elapsed = end;
    }
    public void Dispose()
    {
        foreach (var timer in _timers.ToArray()) timer.Dispose();
    }
    private sealed class TestTimer(TestClock owner, TimerCallback callback, object? state) : ITimer
    {
        private TimeSpan _period;
        private bool _disposed;
        public TimeSpan? Due { get; private set; }
        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_disposed) return false;
            if (dueTime < TimeSpan.Zero && dueTime != Timeout.InfiniteTimeSpan) throw new ArgumentOutOfRangeException(nameof(dueTime));
            if (period < TimeSpan.Zero && period != Timeout.InfiniteTimeSpan) throw new ArgumentOutOfRangeException(nameof(period));
            _period = period;
            Due = dueTime == Timeout.InfiniteTimeSpan ? null : owner.Elapsed + dueTime;
            return true;
        }
        public void Fire()
        {
            Due = _period > TimeSpan.Zero ? owner.Elapsed + _period : null;
            callback(state);
        }
        public void Dispose() { _disposed = true; Due = null; owner._timers.Remove(this); }
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }
}
