// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/synchronous_future.dart
namespace Doroti.Framework.Foundation;

/// <summary>A completed value whose continuation is invoked synchronously, matching Flutter's SynchronousFuture.</summary>
public sealed class SynchronousFuture<T> : Doroti.Runtime.Future<T>
{
    private readonly T _value;

    public SynchronousFuture(T value) : base(Task.FromResult(value)) => _value = value;

    public override Doroti.Runtime.Future<TResult> then<TResult>(Func<T, TResult> onValue) =>
        new SynchronousFuture<TResult>(onValue(_value));

    // Localizations keeps heterogeneous delegate results as Future. Preserve
    // synchronous delivery through that base reference as well as Future<T>.
    public override Doroti.Runtime.Future<TResult> then<TResult>(Func<object?, object?> onValue, Delegate? onError = null)
    {
        var result = onValue(_value);
        return result is Doroti.Runtime.Future<TResult> future
            ? future
            : new SynchronousFuture<TResult>((TResult)result!);
    }

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
