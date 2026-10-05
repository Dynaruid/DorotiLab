namespace Doroti.Ui;
/// <summary>Waits for a provider's actual presentation terminal, rather than a framework submission.</summary>
public interface IFramePresentationHostCapability
{
    ValueTask WaitForPresentationAsync(long minimumFrameworkFrame, CancellationToken cancellationToken = default);
}
/// <summary>One view's bounded presentation receipts. Providers acknowledge only committed frames.</summary>
public sealed class DorotiFramePresentation : IFramePresentationHostCapability, IDisposable
{
    private readonly object _gate = new();
    private readonly Dictionary<long, (long Minimum, TaskCompletionSource Completion)> _waiters = [];
    private long _presented, _next;
    private bool _disposed;
    public void Presented(long frameworkFrame)
    {
        TaskCompletionSource[] ready;
        lock (_gate)
        {
            if (_disposed) return;
            _presented = Math.Max(_presented, frameworkFrame);
            var ids = _waiters.Where(value => value.Value.Minimum <= _presented).Select(value => value.Key).ToArray();
            ready = ids.Select(id => _waiters[id].Completion).ToArray();
            foreach (var id in ids) _waiters.Remove(id);
        }
        foreach (var completion in ready) completion.TrySetResult();
    }
    public async ValueTask WaitForPresentationAsync(long minimumFrameworkFrame, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (minimumFrameworkFrame <= 0) throw new ArgumentOutOfRangeException(nameof(minimumFrameworkFrame));
        TaskCompletionSource completion; long id;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_presented >= minimumFrameworkFrame) return;
            if (_waiters.Count >= 64) throw new InvalidOperationException("Presentation wait admission is full.");
            completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
            id = ++_next; _waiters.Add(id, (minimumFrameworkFrame, completion));
        }
        try { await completion.Task.WaitAsync(cancellationToken); }
        finally { lock (_gate) _waiters.Remove(id); }
    }
    public void Dispose()
    {
        TaskCompletionSource[] remaining;
        lock (_gate) { if (_disposed) return; _disposed = true; remaining = _waiters.Values.Select(value => value.Completion).ToArray(); _waiters.Clear(); }
        foreach (var completion in remaining) completion.TrySetException(new ObjectDisposedException(nameof(DorotiFramePresentation)));
    }
}
