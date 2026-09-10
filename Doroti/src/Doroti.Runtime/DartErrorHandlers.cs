using System.Diagnostics;

namespace Doroti.Runtime;

/// <summary>
/// Explicit adapters for custom exception and return types at a Future boundary.
/// Action/Func handlers accepting object or Exception can be passed directly when
/// their result matches Future&lt;T&gt; (or is a reference type for untyped Future).
/// Adapt value results on untyped futures, narrower exception types and custom delegates.
/// </summary>
public static class DartErrorHandlers
{
    public static Func<Exception, StackTrace, object?> Adapt<TError, TResult>(Func<TError, TResult> callback)
        where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(callback);
        return (error, _) => callback((TError)error);
    }

    public static Func<Exception, StackTrace, object?> Adapt<TError, TResult>(Func<TError, StackTrace, TResult> callback)
        where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(callback);
        return (error, stack) => callback((TError)error, stack);
    }

    public static Func<Exception, StackTrace, Task<object?>> AdaptTask<TError, TResult>(Func<TError, Task<TResult>> callback)
        where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(callback);
        return async (error, _) => await callback((TError)error).ConfigureAwait(false);
    }

    public static Action<Exception, StackTrace> Adapt<TError>(Action<TError, StackTrace> callback)
        where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(callback);
        return (error, stack) => callback((TError)error, stack);
    }

    private sealed class VoidResult;
    private static readonly VoidResult Void = new();

    internal static object? Invoke<T>(Delegate handler, Exception error)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var stack = new StackTrace(error, true);
        // Func argument contravariance includes handlers taking object. Match
        // the closed result before object covariance so value types need no DLR.
        return handler switch
        {
            Func<Exception, StackTrace, T> callback => callback(error, stack),
            Func<Exception, T> callback => callback(error),
            Func<Exception, StackTrace, Future<T>> callback => callback(error, stack),
            Func<Exception, Future<T>> callback => callback(error),
            Func<Exception, StackTrace, Task<T>> callback => callback(error, stack),
            Func<Exception, Task<T>> callback => callback(error),
            Func<Exception, StackTrace, object?> callback => callback(error, stack),
            Func<Exception, object?> callback => callback(error),
            Action<Exception, StackTrace> callback => InvokeAction(callback, error, stack),
            Action<Exception> callback => InvokeAction(callback, error),
            _ => throw new ArgumentException(
                "Unsupported Future error delegate. Use Action/Func accepting object or Exception, " +
                "or DartErrorHandlers.Adapt/AdaptTask for a custom exception or delegate type.", nameof(handler)),
        };
    }

    private static object InvokeAction(Action<Exception, StackTrace> callback, Exception error, StackTrace stack)
    {
        callback(error, stack);
        return Void;
    }

    private static object InvokeAction(Action<Exception> callback, Exception error)
    {
        callback(error);
        return Void;
    }

    internal static async Task<T> Recover<T>(Delegate handler, Exception error)
    {
        var result = Invoke<T>(handler, error);
        if (ReferenceEquals(result, Void)) return default!; // Explicit legacy Action recovery contract.
        if (result is Future future) result = await future.asObjectTask().ConfigureAwait(false);
        else if (result is Task<T> task) return await task.ConfigureAwait(false);
        else if (result is Task<object?> objectTask) result = await objectTask.ConfigureAwait(false);
        if (result is T value) return value;
        if (result is null && default(T) is null) return default!;
        throw new InvalidCastException($"The Future error handler did not return {typeof(T)} or an awaitable of that type.");
    }

    internal static async Task Observe(Delegate handler, Exception error)
    {
        var result = Invoke<object?>(handler, error);
        if (result is Future future) await future.asTask().ConfigureAwait(false);
        else if (result is Task task) await task.ConfigureAwait(false);
    }
}
