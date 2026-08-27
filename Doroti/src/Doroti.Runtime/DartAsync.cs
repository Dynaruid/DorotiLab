using System.Runtime.CompilerServices;

namespace Doroti.Runtime;

public sealed class Timer : IDisposable
{
    private readonly System.Threading.Timer _timer;
    private int _isActive = 1;

    public Timer(Duration duration, Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var scheduler = DartAsyncRuntime.captureMicrotaskScheduler();
        _timer = new System.Threading.Timer(
            _ => DartAsyncRuntime.dispatchCaptured(scheduler, () => InvokeOnce(callback)),
            null,
            (TimeSpan)duration,
            Timeout.InfiniteTimeSpan);
    }

    public Timer(Duration duration, Action<Timer> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var scheduler = DartAsyncRuntime.captureMicrotaskScheduler();
        _timer = new System.Threading.Timer(
            _ => DartAsyncRuntime.dispatchCaptured(scheduler, () => InvokeOnce(() => callback(this))),
            null,
            (TimeSpan)duration,
            Timeout.InfiniteTimeSpan);
    }

    private Timer(Duration duration, Action<Timer> callback, bool periodic)
    {
        var scheduler = DartAsyncRuntime.captureMicrotaskScheduler();
        _timer = new System.Threading.Timer(
            _ => DartAsyncRuntime.dispatchCaptured(scheduler, () => InvokePeriodic(callback)),
            null,
            (TimeSpan)duration,
            periodic ? (TimeSpan)duration : Timeout.InfiniteTimeSpan);
    }

    public bool isActive => Volatile.Read(ref _isActive) == 1;

    public static Timer periodic(Duration duration, Action<Timer> callback) => new(duration, callback, periodic: true);

    public static void run(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        DartRuntimePrimitives.ObserveTask(Task.Run(callback), "Timer.run");
    }

    public void cancel()
    {
        Interlocked.Exchange(ref _isActive, 0);
        _timer.Dispose();
    }

    public void Dispose() => cancel();

    private void InvokeOnce(Action callback)
    {
        // A native timer can expire and enqueue its callback just before a
        // gesture cancels it. Dart cancellation still suppresses that queued
        // event, so claim the callback only when the host event loop executes.
        if (Interlocked.Exchange(ref _isActive, 0) != 1) return;
        callback();
    }

    private void InvokePeriodic(Action<Timer> callback)
    {
        if (Volatile.Read(ref _isActive) != 1) return;
        callback(this);
    }
}

public static class DartAsyncRuntime
{
    private static readonly AsyncLocal<Action<Action>?> MicrotaskScheduler = new();

    public static void unawaited(object? future)
    {
        if (future is Future dartFuture)
        {
            DartRuntimePrimitives.Observe(dartFuture, "Dart unawaited");
        }
    }

    public static Future wait(IEnumerable<Future> futures) =>
        Future.fromTask(Task.WhenAll(futures.Select(future => future.asTask())));

    public static Future<List<T>> wait<T>(IEnumerable<Future> futures) =>
        Future<List<T>>.fromTask(WaitValues<T>(futures));

    private static async Task<List<T>> WaitValues<T>(IEnumerable<Future> futures)
    {
        var values = new List<T>();
        foreach (var future in futures)
        {
            var value = await future.asObjectTask().ConfigureAwait(false);
            values.Add(value is T typed ? typed : default!);
        }
        return values;
    }

    public static void scheduleMicrotask(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        var scheduler = MicrotaskScheduler.Value;
        if (scheduler is null)
        {
            DartRuntimePrimitives.ObserveTask(Task.Run(callback), "scheduleMicrotask");
        }
        else
        {
            scheduler(callback);
        }
    }

    internal static Action<Action>? captureMicrotaskScheduler() => MicrotaskScheduler.Value;

    internal static void dispatchCaptured(Action<Action>? scheduler, Action callback)
    {
        if (scheduler is null)
        {
            DartRuntimePrimitives.ObserveTask(Task.Run(callback), "captured async callback");
            return;
        }
        scheduler(callback);
    }

    internal static Task dispatchCapturedAsync(Action<Action>? scheduler, Action callback)
    {
        if (scheduler is null) return Task.Run(callback);

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            scheduler(() =>
            {
                try
                {
                    callback();
                    completion.TrySetResult();
                }
                catch (Exception exception)
                {
                    completion.TrySetException(exception);
                }
            });
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }
        return completion.Task;
    }

    internal static Task<T> dispatchCapturedAsync<T>(Action<Action>? scheduler, Func<T> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (scheduler is null) return Task.Run(callback);

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            scheduler(() =>
            {
                try
                {
                    completion.TrySetResult(callback());
                }
                catch (Exception exception)
                {
                    completion.TrySetException(exception);
                }
            });
        }
        catch (Exception exception)
        {
            completion.TrySetException(exception);
        }
        return completion.Task;
    }

    public static void scheduleMicrotask(Action<Duration?> callback) =>
        scheduleMicrotask(() => callback(null));

    /// <summary>Overrides microtask dispatch for a scoped deterministic event-loop pump.</summary>
    public static IDisposable enterMicrotaskScheduler(Action<Action> scheduler)
    {
        ArgumentNullException.ThrowIfNull(scheduler);
        var previous = MicrotaskScheduler.Value;
        MicrotaskScheduler.Value = scheduler;
        return new MicrotaskSchedulerScope(previous);
    }

    public static Task<T> AwaitFutureOr<T>(Future<T> future) => future.asTask();

    public static Task<T> AwaitFutureOr<T>(T value) => Task.FromResult(value);

    public static Task AwaitFutureOr(Future future) => future.asTask();

    public static async Task<T> AwaitFutureOrValue<T>(object? value)
    {
        if (value is Future<T> future) return await future;
        if (value is T typed) return typed;
        return default!;
    }

    public static Task<object?> AwaitObject(Future future) => future.asObjectTask();

    internal static async Task InvokeErrorHandlerAsync(Delegate handler, Exception error)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var result = handler.Method.GetParameters().Length > 1
            ? handler.DynamicInvoke(error, new System.Diagnostics.StackTrace())
            : handler.DynamicInvoke(error);
        switch (result)
        {
            case Future future:
                await future.asTask().ConfigureAwait(false);
                break;
            case Task task:
                await task.ConfigureAwait(false);
                break;
        }
    }

    private sealed class MicrotaskSchedulerScope(Action<Action>? previous) : IDisposable
    {
        private Action<Action>? _previous = previous;

        public void Dispose()
        {
            MicrotaskScheduler.Value = Interlocked.Exchange(ref _previous, null);
        }
    }
}

/// <summary>A deterministic FIFO queue for Flutter/Dart microtask behavior validation and host pumps.</summary>
public sealed class DartMicrotaskQueue
{
    private readonly object _gate = new();
    private readonly Queue<Action> _callbacks = [];

    public int count { get { lock (_gate) return _callbacks.Count; } }

    public void enqueue(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate) _callbacks.Enqueue(callback);
    }

    public void drain()
    {
        using var scope = DartAsyncRuntime.enterMicrotaskScheduler(enqueue);
        while (true)
        {
            Action? callback;
            lock (_gate)
            {
                if (!_callbacks.TryDequeue(out callback)) return;
            }
            callback();
        }
    }
}

/// <summary>A Dart Future-shaped runtime value backed by a .NET task.</summary>
[AsyncMethodBuilder(typeof(FutureMethodBuilder<>))]
public class Future<T> : Future
{
    private readonly Task<T> _typedTask;

    protected Future(Task<T> task) : base(task) => _typedTask = task ?? throw new ArgumentNullException(nameof(task));

    public Future(Duration delay) : this(DelayAsync(delay)) { }

    public Future(object error, System.Diagnostics.StackTrace? stackTrace)
        : this(Task.FromException<T>(error as Exception ??
            new Exception(error?.ToString() ?? "Dart Future.error", new Exception(stackTrace?.ToString()))))
    { }

    private static async Task<T> DelayAsync(Duration delay)
    {
        await Task.Delay((TimeSpan)delay).ConfigureAwait(false);
        return default!;
    }

    public static Future<T> value(T value) => new(Task.FromResult(value));

    public new static Future<T> error(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(Task.FromException<T>(error));
    }

    public static Future<T> fromTask(Task<T> task) => new(task);

    public new Task<T> asTask() => _typedTask;

    internal override async Task<object?> asObjectTask() => await _typedTask.ConfigureAwait(false);

    public new TaskAwaiter<T> GetAwaiter() => _typedTask.GetAwaiter();

    public Future<TResult> then<TResult>(Func<T, TResult> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return Future<TResult>.fromTask(ThenAsync(
            _typedTask, callback, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    public Future<TResult> then<TResult>(Func<T, Future<TResult>> callback, Delegate? onError = null)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return Future<TResult>.fromTask(ThenFutureAsync(
            _typedTask, callback, onError, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    public Future<TResult> then<TResult>(Func<T, Future<TResult>> callback, Action<Exception, System.Diagnostics.StackTrace> onError) =>
        then(callback, (Delegate)onError);

    public Future<TResult> then<TResult>(Func<T, object?> callback, Delegate? onError = null)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return Future<TResult>.fromTask(ThenFutureOrAsync<TResult>(
            _typedTask, callback, onError, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    public Future then(Action<T> callback, Delegate? onError = null)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return Future.fromTask(ThenActionAsync(
            _typedTask, callback, onError, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    public Future then(Action<T> callback, Action<Exception, System.Diagnostics.StackTrace> onError) => then(callback, (Delegate)onError);

    public Stream<T> asStream()
    {
        var controller = new StreamController<T>();
        DartRuntimePrimitives.ObserveTask(PublishAsync(), "Future.asStream");
        return controller.stream;

        async Task PublishAsync()
        {
            try
            {
                controller.add(await _typedTask.ConfigureAwait(false));
            }
            catch (Exception error)
            {
                controller.addError(error);
            }
            finally
            {
                await controller.close();
            }
        }
    }

    public new Future<T> catchError(Delegate onError, Func<object, bool>? test = null) =>
        fromTask(CatchAsync(_typedTask, onError, test));

    public new Future<T> catchError(Func<object, System.Diagnostics.StackTrace?, Future> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    public new Future<T> catchError(Action<object, System.Diagnostics.StackTrace?> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    public new Future<T> onError(Delegate onError, Func<object, bool>? test = null) => catchError(onError, test);

    public new Future<T> onError(Action<object, System.Diagnostics.StackTrace?> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    public Future<T> timeout(Duration timeLimit, Func<Future>? onTimeout = null) =>
        fromTask(TimeoutAsync(_typedTask, timeLimit, onTimeout));

    public Future<T> timeout(Duration timeLimit, Func<object> onTimeout) =>
        fromTask(TimeoutFutureOrAsync(_typedTask, timeLimit, onTimeout));

    public Future<T> whenComplete(Func<object> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return fromTask(WhenCompleteAsync(_typedTask, action, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    public new Future<T> whenComplete(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return fromTask(WhenCompleteAsync(
            _typedTask,
            () => { action(); return null!; },
            DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    private static async Task<TResult> ThenAsync<TResult>(
        Task<T> task,
        Func<T, TResult> callback,
        Action<Action>? scheduler)
    {
        var value = await task.ConfigureAwait(false);
        return await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => callback(value)).ConfigureAwait(false);
    }

    private static async Task<TResult> ThenFutureAsync<TResult>(
        Task<T> task,
        Func<T, Future<TResult>> callback,
        Delegate? onError,
        Action<Action>? scheduler)
    {
        try
        {
            var value = await task.ConfigureAwait(false);
            var next = await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => callback(value)).ConfigureAwait(false);
            return await next.asTask().ConfigureAwait(false);
        }
        catch (Exception error) when (onError is not null)
        {
            await DartAsyncRuntime.InvokeErrorHandlerAsync(onError, error);
            return default!;
        }
    }

    private static async Task<TResult> ThenFutureOrAsync<TResult>(
        Task<T> task,
        Func<T, object?> callback,
        Delegate? onError,
        Action<Action>? scheduler)
    {
        try
        {
            var value = await task.ConfigureAwait(false);
            var result = await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => callback(value)).ConfigureAwait(false);
            return result switch
            {
                Future<TResult> future => await future,
                TResult typedResult => typedResult,
                _ => default!,
            };
        }
        catch (Exception error) when (onError is not null)
        {
            await DartAsyncRuntime.InvokeErrorHandlerAsync(onError, error);
            return default!;
        }
    }

    private static async Task ThenActionAsync(
        Task<T> task,
        Action<T> callback,
        Delegate? onError,
        Action<Action>? scheduler)
    {
        try
        {
            var value = await task.ConfigureAwait(false);
            await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => callback(value)).ConfigureAwait(false);
        }
        catch (Exception error) when (onError is not null) { await DartAsyncRuntime.InvokeErrorHandlerAsync(onError!, error); }
    }

    private static async Task<T> CatchAsync(Task<T> task, Delegate onError, Func<object, bool>? test)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        catch (Exception error) when (test?.Invoke(error) ?? true)
        {
            var recovered = onError.DynamicInvoke(error);
            return recovered switch
            {
                Future<T> future => await future,
                Task<T> recoveredTask => await recoveredTask.ConfigureAwait(false),
                T value => value,
                _ => default!,
            };
        }
    }

    private static async Task<T> TimeoutAsync(Task<T> task, Duration limit, Func<Future>? onTimeout)
    {
        try
        {
            return await task.WaitAsync((TimeSpan)limit).ConfigureAwait(false);
        }
        catch (TimeoutException) when (onTimeout is not null)
        {
            await onTimeout();
            return default!;
        }
    }

    private static async Task<T> TimeoutFutureOrAsync(Task<T> task, Duration limit, Func<object> onTimeout)
    {
        try
        {
            return await task.WaitAsync((TimeSpan)limit).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            var result = onTimeout();
            return result switch
            {
                Future<T> future => await future,
                T value => value,
                _ => default!,
            };
        }
    }

    private static async Task<T> WhenCompleteAsync(
        Task<T> task,
        Func<object> action,
        Action<Action>? scheduler)
    {
        try
        {
            return await task.ConfigureAwait(false);
        }
        finally
        {
            // Dart Future callbacks remain on their originating isolate event
            // loop. A .NET Task continuation otherwise runs on the ThreadPool,
            // outside PlatformDispatcher scope, which stops repeating tickers
            // such as EditableText's iOS caret animation after one cycle.
            await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => _ = action()).ConfigureAwait(false);
        }
    }
}

/// <summary>The non-generic Future used for Dart Future&lt;void&gt; lowering.</summary>
[AsyncMethodBuilder(typeof(FutureMethodBuilder))]
public class Future
{
    private readonly Task _task;

    protected Future() : this(Task.CompletedTask) { }

    protected Future(Task task) => _task = task ?? throw new ArgumentNullException(nameof(task));

    public Future(Duration delay) : this(Task.Delay((TimeSpan)delay)) { }

    public Future(Func<Future> computation)
        : this(RunComputation(computation)) { }

    private static async Task RunComputation(Func<Future> computation)
    {
        ArgumentNullException.ThrowIfNull(computation);
        await computation();
    }

    public static Future value() => new(Task.CompletedTask);

    public static Future error(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(Task.FromException(error));
    }

    public static Future fromTask(Task task) => new(task);

    public Task asTask() => _task;

    public Future whenComplete(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return fromTask(WhenCompleteAsync(_task, action, DartAsyncRuntime.captureMicrotaskScheduler()));
    }

    private static async Task WhenCompleteAsync(
        Task task,
        Action action,
        Action<Action>? scheduler)
    {
        try { await task.ConfigureAwait(false); }
        finally { await DartAsyncRuntime.dispatchCapturedAsync(scheduler, action).ConfigureAwait(false); }
    }

    internal virtual async Task<object?> asObjectTask()
    {
        await _task.ConfigureAwait(false);
        return null;
    }

    public TaskAwaiter GetAwaiter() => _task.GetAwaiter();

    public Future then(Action<object> onValue, Delegate? onError = null) =>
        fromTask(ThenAsync(
            asObjectTask(), onValue, onError, DartAsyncRuntime.captureMicrotaskScheduler()));

    public Future<TResult> then<TResult>(Func<object?, object?> onValue, Delegate? onError = null) =>
        Future<TResult>.fromTask(ThenFutureOrAsync<TResult>(
            asObjectTask(), onValue, onError, DartAsyncRuntime.captureMicrotaskScheduler()));

    public Future<object?> then(Func<object?, object?> onValue, Delegate? onError = null) =>
        Future<object?>.fromTask(ThenFutureOrAsync<object?>(
            asObjectTask(), onValue, onError, DartAsyncRuntime.captureMicrotaskScheduler()));

    public Future then(Action<object> onValue, Action<Exception, System.Diagnostics.StackTrace> onError) => then(onValue, (Delegate)onError);

    public Future catchError(Delegate onError, Func<object, bool>? test = null) =>
        fromTask(CatchAsync(_task, onError, test));

    public Future catchError(Func<object, System.Diagnostics.StackTrace?, Future> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    public Future catchError(Action<object, System.Diagnostics.StackTrace?> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    public Future onError(Delegate onError, Func<object, bool>? test = null) => catchError(onError, test);

    public Future onError(Action<object, System.Diagnostics.StackTrace?> onError, Func<object, bool>? test = null) =>
        catchError((Delegate)onError, test);

    private static async Task ThenAsync(
        Task<object?> task,
        Action<object> onValue,
        Delegate? onError,
        Action<Action>? scheduler)
    {
        try
        {
            var value = await task.ConfigureAwait(false);
            await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => onValue(value!)).ConfigureAwait(false);
        }
        catch (Exception error) when (onError is not null)
        {
            await DartAsyncRuntime.InvokeErrorHandlerAsync(onError, error);
        }
    }

    private static async Task<TResult> ThenFutureOrAsync<TResult>(
        Task<object?> task,
        Func<object?, object?> onValue,
        Delegate? onError,
        Action<Action>? scheduler)
    {
        try
        {
            var value = await task.ConfigureAwait(false);
            var result = await DartAsyncRuntime.dispatchCapturedAsync(scheduler, () => onValue(value)).ConfigureAwait(false);
            return result switch
            {
                Future<TResult> future => await future,
                TResult typedResult => typedResult,
                _ => default!,
            };
        }
        catch (Exception error) when (onError is not null)
        {
            await DartAsyncRuntime.InvokeErrorHandlerAsync(onError, error);
            return default!;
        }
    }

    private static async Task CatchAsync(Task task, Delegate onError, Func<object, bool>? test)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception error) when (test?.Invoke(error) ?? true)
        {
            var recovered = onError.Method.GetParameters().Length > 1
                ? onError.DynamicInvoke(error, new System.Diagnostics.StackTrace())
                : onError.DynamicInvoke(error);
            if (recovered is Future future)
            {
                await future.asTask().ConfigureAwait(false);
            }
            else if (recovered is Task recoveredTask)
            {
                await recoveredTask.ConfigureAwait(false);
            }
        }
    }
}

public sealed class Completer<T>
{
    private readonly TaskCompletionSource<T> _source = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Future<T> future => Future<T>.fromTask(_source.Task);
    public bool isCompleted => _source.Task.IsCompleted;

    public void complete(T value) => _source.TrySetResult(value);

    public void complete(object? value)
    {
        if (value is Future<T> future)
        {
            complete(future);
        }
        else if (value is T typed)
        {
            _source.TrySetResult(typed);
        }
        else
        {
            _source.TrySetResult(default!);
        }
    }

    public void complete() => _source.TrySetResult(default!);

    public void complete(Future<T> value) => CompleteAsync(value.asTask());

    public void complete(Task<T> value) => CompleteAsync(value);

    private void CompleteAsync(Task<T> value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _ = CompleteCoreAsync(value);
    }

    private async Task CompleteCoreAsync(Task<T> value)
    {
        try
        {
            _source.TrySetResult(await value.ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            _source.TrySetCanceled();
        }
        catch (Exception error)
        {
            _source.TrySetException(error);
        }
    }

    public void completeError(object error) => _source.TrySetException(error as Exception ?? new Exception(error?.ToString() ?? "null"));
    public void completeError(object error, object? stackTrace) => completeError(error);
}

public struct FutureMethodBuilder<T>
{
    private AsyncTaskMethodBuilder<T> _builder;

    public static FutureMethodBuilder<T> Create() => new() { _builder = AsyncTaskMethodBuilder<T>.Create() };

    public Future<T> Task => Future<T>.fromTask(_builder.Task);

    public void SetResult(T result) => _builder.SetResult(result);

    public void SetException(Exception exception) => _builder.SetException(exception);

    public void SetStateMachine(IAsyncStateMachine stateMachine) => _builder.SetStateMachine(stateMachine);

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine => _builder.Start(ref stateMachine);

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}

public struct FutureMethodBuilder
{
    private AsyncTaskMethodBuilder _builder;

    public static FutureMethodBuilder Create() => new() { _builder = AsyncTaskMethodBuilder.Create() };

    public Future Task => Future.fromTask(_builder.Task);

    public void SetResult() => _builder.SetResult();

    public void SetException(Exception exception) => _builder.SetException(exception);

    public void SetStateMachine(IAsyncStateMachine stateMachine) => _builder.SetStateMachine(stateMachine);

    public void Start<TStateMachine>(ref TStateMachine stateMachine)
        where TStateMachine : IAsyncStateMachine => _builder.Start(ref stateMachine);

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitOnCompleted(ref awaiter, ref stateMachine);

    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine => _builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
}

public sealed class StreamSubscription<T>
{
    private readonly Action<StreamSubscription<T>> _cancel;

    internal StreamSubscription(Action<T> onData, Action<Exception>? onError, Action? onDone, Action<StreamSubscription<T>> cancel)
    {
        OnData = onData;
        OnError = onError;
        OnDone = onDone;
        _cancel = cancel;
    }

    internal Action<T>? OnData { get; private set; }

    public void onData(Action<T>? callback) => OnData = callback;

    internal Action<Exception>? OnError { get; }

    internal Action? OnDone { get; }

    public bool isCanceled { get; private set; }

    public Future cancel()
    {
        if (!isCanceled)
        {
            isCanceled = true;
            _cancel(this);
        }
        return Future.value();
    }
}

public sealed class Stream<T> : IAsyncEnumerable<T>
{
    private readonly StreamController<T> _controller;

    internal Stream(StreamController<T> controller) => _controller = controller;

    public bool isBroadcast => _controller.IsBroadcast;

    public StreamSubscription<T> listen(Action<T> onData, Action<Exception>? onError = null, Action? onDone = null) =>
        _controller.Listen(onData, onError, onDone);

    public StreamSubscription<T> listen(Action<T> onData, Action<object, System.Diagnostics.StackTrace?> onError, Action? onDone = null) =>
        _controller.Listen(onData, error => onError(error, new System.Diagnostics.StackTrace(error, true)), onDone);

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
        var subscription = listen(
            value => channel.Writer.TryWrite(value),
            error => channel.Writer.TryComplete(error),
            () => channel.Writer.TryComplete());
        try
        {
            await foreach (var value in channel.Reader.ReadAllAsync(cancellationToken)) yield return value;
        }
        finally
        {
            await subscription.cancel();
        }
    }
}

public sealed class StreamController<T>
{
    private readonly List<StreamSubscription<T>> _listeners = [];
    private readonly bool _isBroadcast;
    private bool _closed;
    private bool _wasListened;
    private readonly Func<Task>? _onListen;
    private readonly Func<Task>? _onCancel;

    public StreamController(bool broadcast = false, Func<Task>? onListen = null, Func<Task>? onCancel = null)
    {
        _isBroadcast = broadcast;
        _onListen = onListen;
        _onCancel = onCancel;
        stream = new(this);
    }

    public StreamController(Func<Task> onListen)
    {
        _onListen = onListen ?? throw new ArgumentNullException(nameof(onListen));
        stream = new(this);
    }

    public Stream<T> stream { get; }

    internal bool IsBroadcast => _isBroadcast;

    public void add(T value)
    {
        EnsureOpen();
        foreach (var listener in _listeners.ToArray())
        {
            if (!listener.isCanceled)
            {
                listener.OnData?.Invoke(value);
            }
        }
    }

    public void Add(T value) => add(value);

    public void addError(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        EnsureOpen();
        foreach (var listener in _listeners.ToArray())
        {
            if (listener.isCanceled)
            {
                continue;
            }
            if (listener.OnError is null)
            {
                throw error;
            }
            listener.OnError(error);
        }
    }

    public Future close()
    {
        if (_closed)
        {
            return Future.value();
        }
        _closed = true;
        foreach (var listener in _listeners.ToArray())
        {
            if (!listener.isCanceled)
            {
                listener.OnDone?.Invoke();
            }
        }
        _listeners.Clear();
        return Future.value();
    }

    internal StreamSubscription<T> Listen(Action<T> onData, Action<Exception>? onError, Action? onDone)
    {
        ArgumentNullException.ThrowIfNull(onData);
        if (_closed)
        {
            throw new InvalidOperationException("Cannot listen to a closed Stream.");
        }
        if (!_isBroadcast && _wasListened)
        {
            throw new InvalidOperationException("A single-subscription Stream can only be listened to once.");
        }
        _wasListened = true;
        if (_onListen is not null) DartRuntimePrimitives.ObserveTask(_onListen(), "Stream.onListen");
        var subscription = new StreamSubscription<T>(onData, onError, onDone, item =>
        {
            _listeners.Remove(item);
            if (_listeners.Count == 0 && _onCancel is not null) DartRuntimePrimitives.ObserveTask(_onCancel(), "Stream.onCancel");
        });
        _listeners.Add(subscription);
        return subscription;
    }

    private void EnsureOpen()
    {
        if (_closed)
        {
            throw new InvalidOperationException("Cannot add events after closing a Stream.");
        }
    }
}
