namespace Doroti.Composition;

public enum FrameAckStatus
{
    Presented,
    Stale,
    Superseded,
    Failed,
    Cancelled,
}

public enum FrameFaultKind
{
    Programming,
    Stale,
    Superseded,
    Cancelled,
    RecoverableSurfaceLoss,
    FatalBackend,
}

public sealed record FrameAckResult(
    FrameId FrameId,
    FrameAckStatus Status,
    string? Diagnostic = null,
    FrameFaultKind? FaultKind = null);

public sealed class FrameAck
{
    private readonly TaskCompletionSource<FrameAckResult> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public FrameAck(FrameId frameId) => FrameId = frameId;

    public FrameId FrameId { get; }

    public bool IsCompleted => _completion.Task.IsCompleted;

    public Task<FrameAckResult> Completion => _completion.Task;

    public bool TryComplete(FrameAckStatus status, string? diagnostic = null, FrameFaultKind? faultKind = null) =>
        _completion.TrySetResult(new(FrameId, status, diagnostic, faultKind));
}

/// <summary>A deterministic one-in-flight plus one-pending latest-frame mailbox.</summary>
public sealed class FrameMailbox<T> : IDisposable
    where T : class
{
    private readonly object _gate = new();
    private readonly Func<T, FrameAckStatus, string?, bool> _complete;
    private readonly SemaphoreSlim _available = new(0, 1);
    private T? _pending;
    private bool _inFlight;
    private bool _shutdown;

    public FrameMailbox(Func<T, FrameAckStatus, string?, bool> complete) =>
        _complete = complete ?? throw new ArgumentNullException(nameof(complete));

    public int Depth
    {
        get
        {
            lock (_gate)
            {
                return (_inFlight ? 1 : 0) + (_pending is null ? 0 : 1);
            }
        }
    }

    public int HighWatermark { get; private set; }

    public int SupersededCount { get; private set; }

    public bool TrySubmit(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        T? superseded = null;
        lock (_gate)
        {
            if (_shutdown)
            {
                _complete(item, FrameAckStatus.Cancelled, "Mailbox is shutting down.");
                return false;
            }

            if (_pending is not null)
            {
                superseded = _pending;
                SupersededCount++;
            }
            _pending = item;
            HighWatermark = Math.Max(HighWatermark, DepthUnsafe());
            if (!_inFlight && _available.CurrentCount == 0)
            {
                _available.Release();
            }
        }

        if (superseded is not null)
        {
            _complete(superseded, FrameAckStatus.Superseded, "A newer pending frame replaced this frame.");
        }
        return true;
    }

    public async ValueTask<T?> TakeAsync(CancellationToken cancellationToken = default)
    {
        await _available.WaitAsync(cancellationToken).ConfigureAwait(false);
        lock (_gate)
        {
            if (_pending is null)
            {
                return null;
            }
            var item = _pending;
            _pending = null;
            _inFlight = true;
            return item;
        }
    }

    public T? Take(CancellationToken cancellationToken = default)
    {
        _available.Wait(cancellationToken);
        lock (_gate)
        {
            if (_pending is null)
            {
                return null;
            }
            var item = _pending;
            _pending = null;
            _inFlight = true;
            return item;
        }
    }

    public void CompleteInFlight()
    {
        lock (_gate)
        {
            if (!_inFlight)
            {
                throw new InvalidOperationException("The mailbox has no in-flight frame.");
            }
            _inFlight = false;
            if (_pending is not null && _available.CurrentCount == 0)
            {
                _available.Release();
            }
            else if (_shutdown && _available.CurrentCount == 0)
            {
                _available.Release();
            }
        }
    }

    public void Shutdown()
    {
        T? pending;
        lock (_gate)
        {
            if (_shutdown)
            {
                return;
            }
            _shutdown = true;
            pending = _pending;
            _pending = null;
            if (!_inFlight && _available.CurrentCount == 0)
            {
                _available.Release();
            }
        }
        if (pending is not null)
        {
            _complete(pending, FrameAckStatus.Cancelled, "Pending frame was cancelled during shutdown.");
        }
    }

    public void Dispose()
    {
        Shutdown();
        _available.Dispose();
    }

    private int DepthUnsafe() => (_inFlight ? 1 : 0) + (_pending is null ? 0 : 1);
}
