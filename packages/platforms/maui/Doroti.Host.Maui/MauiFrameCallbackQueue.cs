namespace Doroti.Host.Maui;

internal sealed class MauiFrameCallbackQueue
{
    private Action<TimeSpan>? _pending;
    private readonly object _gate = new();
    internal bool HasPending { get { lock (_gate) return _pending is not null; } }
    internal bool TrySchedule(Action<TimeSpan> callback)
    {
        lock (_gate)
        {
            if (_pending is not null) return false;
            _pending = callback;
            return true;
        }
    }
    internal Action<TimeSpan>? Take()
    {
        lock (_gate)
        {
            var callback = _pending;
            _pending = null;
            return callback;
        }
    }
    internal void Clear() { lock (_gate) _pending = null; }
}
