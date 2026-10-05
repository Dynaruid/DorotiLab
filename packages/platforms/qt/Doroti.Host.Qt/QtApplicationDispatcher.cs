using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Doroti.Host.Qt;

internal static class QtApplicationDispatcher
{
    internal sealed class Owner : Doroti.Ui.IDorotiApplicationDispatcher
    {
        private readonly int _thread = Environment.CurrentManagedThreadId;
        public bool HasThreadAccess => Environment.CurrentManagedThreadId == _thread;
        public ValueTask InvokeAsync(Action callback, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!HasThreadAccess) return new(Post(callback, cancellationToken));
            callback(); return ValueTask.CompletedTask;
        }
        public async ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
        {
            T result = default!;
            await InvokeAsync(() => { result = callback(); }, cancellationToken);
            return result;
        }
    }
    private sealed record Work(Action Action, CancellationToken Token, TaskCompletionSource Completion);
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_post_v2", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int PostNative(delegate* unmanaged[Cdecl]<nint, int, void> callback, nint context);

    internal static unsafe Task Post(Action action, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var handle = GCHandle.Alloc(new Work(action, token, completion));
        int status;
        try { status = PostNative(&Run, GCHandle.ToIntPtr(handle)); }
        catch { handle.Free(); throw; }
        if (status != 0)
        {
            handle.Free();
            completion.TrySetException(new ObjectDisposedException("Qt application event loop"));
        }
        return completion.Task;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void Run(nint context, int status)
    {
        var handle = GCHandle.FromIntPtr(context);
        var work = (Work)handle.Target!;
        handle.Free();
        try
        {
            if (status != 0) throw new ObjectDisposedException("Qt application event loop");
            work.Token.ThrowIfCancellationRequested();
            work.Action();
            work.Completion.TrySetResult();
        }
        catch (OperationCanceledException) { work.Completion.TrySetCanceled(work.Token); }
        catch (Exception error) { work.Completion.TrySetException(error); }
    }
}
