using System.Runtime.CompilerServices;
namespace Doroti.Runtime;
internal sealed class FutureContinuation(DorotiCallbackDispatcher? owner)
{
    private Exception? _rejected;
    internal void Check() { if (_rejected is { } error) throw error; owner?.Lifetime.ThrowIfCancellationRequested(); }
    internal void Register(Task task, Action continuation, bool unsafeFlow)
    {
        void Complete()
        {
            if (owner is null) { continuation(); return; }
            _ = DispatchAsync();
            async Task DispatchAsync()
            {
                var executed = false;
                try { await owner.PostAsync(() => { executed = true; continuation(); }); }
                catch (Exception error)
                {
                    if (executed) return;
                    _rejected = error;
                    // Resume only to let GetResult throw. A rejected owner never executes
                    // the successful continuation or silently selects another view.
                    continuation();
                }
            }
        }
        if (unsafeFlow) task.GetAwaiter().UnsafeOnCompleted(Complete);
        else task.GetAwaiter().OnCompleted(Complete);
    }
}
public readonly struct FutureAwaiter<T> : ICriticalNotifyCompletion
{
    private readonly Task<T> _task;
    private readonly FutureContinuation _continuation;
    internal FutureAwaiter(Task<T> task) { _task = task; _continuation = new(DorotiExecutionContext.CaptureDispatcher()); }
    public bool IsCompleted => _task.IsCompleted;
    public T GetResult() { _continuation.Check(); return _task.GetAwaiter().GetResult(); }
    public void OnCompleted(Action continuation) => _continuation.Register(_task, continuation, false);
    public void UnsafeOnCompleted(Action continuation) => _continuation.Register(_task, continuation, true);
}
public readonly struct FutureAwaiter : ICriticalNotifyCompletion
{
    private readonly Task _task;
    private readonly FutureContinuation _continuation;
    internal FutureAwaiter(Task task) { _task = task; _continuation = new(DorotiExecutionContext.CaptureDispatcher()); }
    public bool IsCompleted => _task.IsCompleted;
    public void GetResult() { _continuation.Check(); _task.GetAwaiter().GetResult(); }
    public void OnCompleted(Action continuation) => _continuation.Register(_task, continuation, false);
    public void UnsafeOnCompleted(Action continuation) => _continuation.Register(_task, continuation, true);
}
