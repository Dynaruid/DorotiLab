using System.Threading.Channels;

namespace Doroti.Ui;

/// <summary>Bounded typed application queue. Inputs and terminal work wait for admission and are never dropped.</summary>
public sealed class DorotiApplicationDispatcher : IDorotiApplicationDispatcher, IAsyncDisposable
{
    private readonly Channel<Action> _queue;
    private readonly Thread _owner;
    private readonly TaskCompletionSource _exited = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int _disposed;
    public DorotiApplicationDispatcher(int capacity = 256, string name = "Doroti application")
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        _queue = Channel.CreateBounded<Action>(new BoundedChannelOptions(capacity) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait, AllowSynchronousContinuations = false });
        _owner = new Thread(() =>
        {
            try { while (_queue.Reader.WaitToReadAsync().AsTask().GetAwaiter().GetResult()) while (_queue.Reader.TryRead(out var work)) work(); _exited.TrySetResult(); }
            catch (Exception error) { _exited.TrySetException(error); }
        }) { IsBackground = true, Name = name };
        if (ExecutionContext.IsFlowSuppressed()) _owner.Start();
        else { using (ExecutionContext.SuppressFlow()) _owner.Start(); }
    }
    public bool HasThreadAccess => Environment.CurrentManagedThreadId == _owner.ManagedThreadId;
    public ValueTask InvokeAsync(Action callback, CancellationToken token = default) => new(InvokeAsync(() => { callback(); return true; }, token).AsTask());
    public async ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        token.ThrowIfCancellationRequested();
        if (HasThreadAccess) return callback();
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        await _queue.Writer.WriteAsync(() =>
        {
            try { token.ThrowIfCancellationRequested(); completion.TrySetResult(callback()); }
            catch (OperationCanceledException error) { completion.TrySetCanceled(error.CancellationToken); }
            catch (Exception error) { completion.TrySetException(error); }
        }, token);
        // Once admitted, cleanup waits for actual callback completion. Cancellation
        // never reports a still-running invocation as drained.
        return await completion.Task;
    }
    public async ValueTask DisposeAsync()
    {
        if (HasThreadAccess) throw new InvalidOperationException("Dispose the application dispatcher after leaving its owner callback.");
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _queue.Writer.TryComplete();
        await _exited.Task;
    }
}
