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
        // The owner synchronously waits on the channel readiness task. Publish
        // that readiness directly; a ThreadPool hop can otherwise prevent an
        // idle UI owner from waking while the pool is busy. User callbacks still
        // execute only in the dedicated owner's read loop.
        _queue = Channel.CreateBounded<Action>(new BoundedChannelOptions(capacity) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait, AllowSynchronousContinuations = true });
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
    public ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken token = default)
    {
        if (callback is null) return ValueTask.FromException<T>(new ArgumentNullException(nameof(callback)));
        if (Volatile.Read(ref _disposed) != 0)
            return ValueTask.FromException<T>(new ObjectDisposedException(nameof(DorotiApplicationDispatcher)));
        if (token.IsCancellationRequested) return ValueTask.FromCanceled<T>(token);
        if (HasThreadAccess)
        {
            try { return ValueTask.FromResult(callback()); }
            catch (OperationCanceledException error)
            {
                var canceled = new TaskCompletionSource<T>();
                canceled.SetCanceled(error.CancellationToken);
                return new(canceled.Task);
            }
            catch (Exception error) { return ValueTask.FromException<T>(error); }
        }
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Action work = () =>
        {
            try { token.ThrowIfCancellationRequested(); completion.TrySetResult(callback()); }
            catch (OperationCanceledException error) { completion.TrySetCanceled(error.CancellationToken); }
            catch (Exception error) { completion.TrySetException(error); }
        };
        if (!_queue.Writer.TryWrite(work))
        {
            var admission = _queue.Writer.WriteAsync(work, token);
            if (!admission.IsCompletedSuccessfully) _ = ObserveAdmissionAsync(admission, completion);
        }
        // Once admitted, cleanup waits for actual callback completion. Cancellation
        // never reports a still-running invocation as drained.
        // Return the owner's completion itself. An async wrapper would require
        // a pool continuation before a synchronous native waiter could resume.
        return new(completion.Task);
    }
    private static async Task ObserveAdmissionAsync<T>(ValueTask admission, TaskCompletionSource<T> completion)
    {
        try { await admission.ConfigureAwait(false); }
        catch (OperationCanceledException error) { completion.TrySetCanceled(error.CancellationToken); }
        catch (Exception error) { completion.TrySetException(error); }
    }
    public async ValueTask DisposeAsync()
    {
        if (HasThreadAccess) throw new InvalidOperationException("Dispose the application dispatcher after leaving its owner callback.");
        if (Interlocked.Exchange(ref _disposed, 1) == 0) _queue.Writer.TryComplete();
        await _exited.Task;
    }
}
