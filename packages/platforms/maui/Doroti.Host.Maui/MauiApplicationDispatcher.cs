using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;

namespace Doroti.Host.Maui;

/// <summary>The native main loop owns the framework; workers only enqueue typed work.</summary>
internal sealed class MauiApplicationDispatcher : IDorotiApplicationDispatcher
{
    private int _pending;
    public bool HasThreadAccess =>
#if MACOS
        Foundation.NSThread.IsMain;
#else
        MainThread.IsMainThread;
#endif
    public async ValueTask InvokeAsync(Action callback, CancellationToken cancellationToken = default) =>
        await InvokeAsync(() => { callback(); return true; }, cancellationToken);
    public ValueTask<T> InvokeAsync<T>(Func<T> callback, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(callback);
        cancellationToken.ThrowIfCancellationRequested();
        if (HasThreadAccess) return ValueTask.FromResult(callback());
        if (Interlocked.Increment(ref _pending) > 256)
        {
            Interlocked.Decrement(ref _pending);
            throw new InvalidOperationException("The MAUI application dispatcher queue is full.");
        }
        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
#if MACOS
            AppKit.NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
#else
            MainThread.BeginInvokeOnMainThread(() =>
#endif
            {
                Interlocked.Decrement(ref _pending);
                try { cancellationToken.ThrowIfCancellationRequested(); completion.TrySetResult(callback()); }
                catch (OperationCanceledException error) { completion.TrySetCanceled(error.CancellationToken); }
                catch (Exception error) { completion.TrySetException(error); }
            });
        }
        catch { Interlocked.Decrement(ref _pending); throw; }
        return new(completion.Task);
    }
}
