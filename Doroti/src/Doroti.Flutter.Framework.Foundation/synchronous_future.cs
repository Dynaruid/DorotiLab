// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/synchronous_future.dart
namespace Doroti.Generated.Framework.Foundation;

/// <summary>A completed value whose continuation is invoked synchronously, matching Flutter's SynchronousFuture.</summary>
public sealed class SynchronousFuture<T> : Doroti.Flutter.Runtime.Future<T>
{
    private readonly T _value;

    public SynchronousFuture(T value) : base(Task.FromResult(value)) => _value = value;

    public new Task<TResult> then<TResult>(Func<T, TResult> onValue) => Task.FromResult(onValue(_value));

    public Task<TResult> thenAsync<TResult>(Func<T, Task<TResult>> onValue) => onValue(_value);

    public new async IAsyncEnumerable<T> asStream()
    {
        yield return _value;
        await Task.CompletedTask;
    }

    public Task<T> catchError(Func<Exception, T> onError, Func<Exception, bool>? test = null)
    {
        _ = onError;
        _ = test;
        return new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously).Task;
    }

    public async Task<T> timeout(TimeSpan timeLimit, Func<T>? onTimeout = null)
    {
        _ = timeLimit;
        _ = onTimeout;
        await Task.CompletedTask;
        return _value;
    }

    public async Task<T> whenComplete(Func<Task?> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var result = action();
        if (result is not null)
        {
            await result.ConfigureAwait(false);
        }
        return _value;
    }

    public new Task<T> asTask() => Task.FromResult(_value);
}
