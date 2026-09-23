namespace Doroti.Runtime;

/// <summary>
/// Host-owned scheduling and time for managed callbacks. Capture a dispatcher
/// before starting asynchronous work, then post UI work through that capture.
/// </summary>
public static class DorotiExecutionContext
{
    private static readonly AsyncLocal<DorotiCallbackDispatcher?> ActiveDispatcher = new();
    private static readonly AsyncLocal<TimeProvider?> ActiveTimeProvider = new();

    public static TimeProvider TimeProvider =>
        ActiveTimeProvider.Value
        ?? (
            OperatingSystem.IsBrowser()
                ? throw new InvalidOperationException(
                    "Doroti browser timers require an active host TimeProvider scope."
                )
                : TimeProvider.System
        );

    public static DorotiCallbackDispatcher? CaptureDispatcher() => ActiveDispatcher.Value;

    public static IDisposable EnterDispatcher(
        Func<Action, bool> tryPost,
        CancellationToken lifetime = default
    )
    {
        ArgumentNullException.ThrowIfNull(tryPost);
        var previous = ActiveDispatcher.Value;
        ActiveDispatcher.Value = new DorotiCallbackDispatcher(tryPost, lifetime);
        return new DispatcherScope(previous);
    }

    public static IDisposable EnterTimeProvider(TimeProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        var previous = ActiveTimeProvider.Value;
        ActiveTimeProvider.Value = provider;
        return new TimeProviderScope(previous);
    }

    private sealed class DispatcherScope(DorotiCallbackDispatcher? previous) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                ActiveDispatcher.Value = previous;
            }
        }
    }

    private sealed class TimeProviderScope(TimeProvider? previous) : IDisposable
    {
        private int _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                ActiveTimeProvider.Value = previous;
            }
        }
    }
}

/// <summary>A captured host callback queue. Rejected posts cancel the returned task.</summary>
public sealed class DorotiCallbackDispatcher
{
    private readonly Func<Action, bool> _tryPost;
    private readonly CancellationToken _lifetime;

    internal DorotiCallbackDispatcher(Func<Action, bool> tryPost, CancellationToken lifetime)
    {
        _tryPost = tryPost;
        _lifetime = lifetime;
    }

    public bool TryPost(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return !_lifetime.IsCancellationRequested
            && _tryPost(() =>
            {
                if (!_lifetime.IsCancellationRequested)
                {
                    callback();
                }
            });
    }

    public async Task PostAsync(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var completion = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        using var registration = _lifetime.Register(() => completion.TrySetCanceled(_lifetime));
        try
        {
            if (!TryPost(() =>
                {
                    try
                    {
                        callback();
                        completion.TrySetResult();
                    }
                    catch (Exception error)
                    {
                        completion.TrySetException(error);
                    }
                }))
            {
                completion.TrySetCanceled();
            }
        }
        catch (Exception error)
        {
            completion.TrySetException(error);
        }
        await completion.Task.ConfigureAwait(false);
    }
}
