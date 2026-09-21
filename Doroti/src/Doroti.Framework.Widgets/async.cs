// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/async.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class StreamBuilderBase<T, S> : StatefulWidget
{
    public virtual Stream<T>? stream { get; private set; }

    protected StreamBuilderBase(Key? key = null, Stream<T>? stream = default!)
        : base(key: key)
    {
        this.stream = stream;
    }

    public abstract S initial();

    public virtual S afterConnected(S current) => current;

    public abstract S afterData(S current, T data);

    public virtual S afterError(
        S current,
        object error,
        System.Diagnostics.StackTrace? stackTrace
    ) => current;

    public virtual S afterDone(S current) => current;

    public virtual S afterDisconnected(S current) => current;

    public abstract Widget build(BuildContext context, S currentSummary);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _StreamBuilderBaseState__async<T, S>());
}

internal class _StreamBuilderBaseState__async<T, S> : State<StreamBuilderBase<T, S>>
{
    internal virtual StreamSubscription<T>? _subscription { get; set; } = default;
    internal virtual S _summary { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _summary = widget.initial();
        _subscribe();
    }

    public override void didUpdateWidget(StreamBuilderBase<T, S> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.stream, widget.stream))
        {
            if (_subscription is not null)
            {
                _unsubscribe();
                _summary = widget.afterDisconnected(_summary);
            }
            _subscribe();
        }
    }

    public override Widget build(BuildContext context) => widget.build(context, _summary);

    public override void dispose()
    {
        _unsubscribe();
        base.dispose();
    }

    internal virtual void _subscribe()
    {
        if (widget.stream is not null)
        {
            _subscription = widget.stream!.listen(
                (data) =>
                {
                    setState(() =>
                    {
                        _summary = widget.afterData(_summary, data);
                    });
                },
                onError: (error, stackTrace) =>
                {
                    setState(() =>
                    {
                        _summary = widget.afterError(_summary, error, stackTrace);
                    });
                },
                onDone: () =>
                {
                    setState(() =>
                    {
                        _summary = widget.afterDone(_summary);
                    });
                }
            );
            _summary = widget.afterConnected(_summary);
        }
    }

    internal virtual void _unsubscribe()
    {
        if (_subscription is not null)
        {
            DartRuntimePrimitives.Ignore(_subscription!.cancel());
            _subscription = null;
        }
    }
}

public enum ConnectionState
{
    none,
    waiting,
    active,
    done,
}

public class AsyncSnapshot<T>
{
    public virtual ConnectionState connectionState { get; private set; } = default!;
    public virtual T? data { get; private set; }
    public virtual object? error { get; private set; }
    public virtual System.Diagnostics.StackTrace? stackTrace { get; private set; }

    public AsyncSnapshot(
        ConnectionState connectionState,
        T? data,
        object? error,
        System.Diagnostics.StackTrace? stackTrace
    )
    {
        this.connectionState = connectionState;
        this.data = data;
        this.error = error;
        this.stackTrace = stackTrace;
        System.Diagnostics.Debug.Assert((data is null) || (error is null));
        System.Diagnostics.Debug.Assert((stackTrace is null) || (error is not null));
    }

    public static AsyncSnapshot<T> CreateNothing()
    {
        return new AsyncSnapshot<T>(ConnectionState.none, default, default, default);
    }

    public static AsyncSnapshot<T> CreateWaiting()
    {
        return new AsyncSnapshot<T>(ConnectionState.waiting, default, default, default);
    }

    public static AsyncSnapshot<T> CreateWithData(ConnectionState state, T data)
    {
        return new AsyncSnapshot<T>(state, data, default, default);
    }

    public static AsyncSnapshot<T> CreateWithError(
        ConnectionState state,
        object error,
        System.Diagnostics.StackTrace? stackTrace = default!
    )
    {
        return new AsyncSnapshot<T>(state, default, error, stackTrace);
    }

    public virtual T requireData
    {
        get
        {
            if (hasData)
            {
                return data!;
            }
            if (hasError)
            {
                Dart_coreLibrary.throwWithStackTrace(error!, stackTrace!);
            }
            throw new InvalidOperationException("Snapshot has neither data nor error");
        }
    }

    public virtual AsyncSnapshot<T> inState(ConnectionState state) =>
        new AsyncSnapshot<T>(state, data, error, stackTrace);

    public virtual bool hasData => DartRuntimePrimitives.ConvertValue<bool>(data is not null);
    public virtual bool hasError => DartRuntimePrimitives.ConvertValue<bool>(error is not null);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "AsyncSnapshot")}({connectionState}, {data}, {error}, {stackTrace})";

    public override bool Equals(object? other)
    {
        var __other = other as AsyncSnapshot<T>;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is AsyncSnapshot<T>)
            && Equals(__other.connectionState, connectionState)
            && EqualityComparer<T>.Default.Equals(__other.data, data)
            && Equals(__other.error, error)
            && Equals(__other.stackTrace, stackTrace);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(connectionState, data, error)
        );
}

public delegate Widget AsyncWidgetBuilder<T>(BuildContext context, AsyncSnapshot<T> snapshot);

public class StreamBuilder<T> : StreamBuilderBase<T, AsyncSnapshot<T>>
{
    public virtual Func<BuildContext, AsyncSnapshot<T>, Widget> builder { get; private set; } =
        default!;
    public virtual T? initialData { get; private set; }

    public StreamBuilder(
        Key? key = null,
        T? initialData = default,
        Stream<T>? stream = default!,
        Func<BuildContext, AsyncSnapshot<T>, Widget> builder = default!
    )
        : base(key: key, stream: stream)
    {
        this.initialData = initialData;
        this.builder = builder;
    }

    public override AsyncSnapshot<T> initial() =>
        (initialData is null)
            ? AsyncSnapshot<T>.CreateNothing()
            : AsyncSnapshot<T>.CreateWithData(ConnectionState.none, initialData!);

    public override AsyncSnapshot<T> afterConnected(AsyncSnapshot<T> current) =>
        current.inState(ConnectionState.waiting);

    public override AsyncSnapshot<T> afterData(AsyncSnapshot<T> current, T data)
    {
        return AsyncSnapshot<T>.CreateWithData(ConnectionState.active, data);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override AsyncSnapshot<T> afterError(
        AsyncSnapshot<T> current,
        object error,
        System.Diagnostics.StackTrace? stackTrace
    )
    {
        return AsyncSnapshot<T>.CreateWithError(ConnectionState.active, error, stackTrace);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override AsyncSnapshot<T> afterDone(AsyncSnapshot<T> current) =>
        current.inState(ConnectionState.done);

    public override AsyncSnapshot<T> afterDisconnected(AsyncSnapshot<T> current) =>
        current.inState(ConnectionState.none);

    public override Widget build(BuildContext context, AsyncSnapshot<T> currentSummary) =>
        builder(context, currentSummary);
}

public class FutureBuilder<T> : StatefulWidget
{
    public virtual Future<T>? future { get; private set; }
    public virtual Func<BuildContext, AsyncSnapshot<T>, Widget> builder { get; private set; } =
        default!;
    public virtual T? initialData { get; private set; }
    public static bool debugRethrowError = false;

    public FutureBuilder(
        Key? key = null,
        Future<T>? future = default!,
        T? initialData = default,
        Func<BuildContext, AsyncSnapshot<T>, Widget> builder = default!
    )
        : base(key: key)
    {
        this.future = future;
        this.initialData = initialData;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _FutureBuilderState__async<T>());
}

internal class _FutureBuilderState__async<T> : State<FutureBuilder<T>>
{
    internal virtual object? _activeCallbackIdentity { get; set; } = default;
    internal virtual AsyncSnapshot<T> _snapshot { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _snapshot =
            (widget.initialData is null)
                ? AsyncSnapshot<T>.CreateNothing()
                : AsyncSnapshot<T>.CreateWithData(ConnectionState.none, widget.initialData!);
        _subscribe();
    }

    public override void didUpdateWidget(FutureBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (Equals(oldWidget.future, widget.future))
        {
            return;
        }
        if (_activeCallbackIdentity is not null)
        {
            _unsubscribe();
            _snapshot = _snapshot.inState(ConnectionState.none);
        }
        _subscribe();
    }

    public override Widget build(BuildContext context) => widget.builder(context, _snapshot);

    public override void dispose()
    {
        _unsubscribe();
        base.dispose();
    }

    internal virtual void _subscribe()
    {
        if (widget.future is null)
        {
            return;
        }
        var callbackIdentity = new object();
        _activeCallbackIdentity = callbackIdentity;
        DartRuntimePrimitives.Ignore(
            widget.future!.then(
                (data) =>
                {
                    if (Equals(_activeCallbackIdentity, callbackIdentity))
                    {
                        setState(() =>
                        {
                            _snapshot = AsyncSnapshot<T>.CreateWithData(ConnectionState.done, data);
                        });
                    }
                },
                onError: (error, stackTrace) =>
                {
                    if (Equals(_activeCallbackIdentity, callbackIdentity))
                    {
                        setState(() =>
                        {
                            _snapshot = AsyncSnapshot<T>.CreateWithError(
                                ConnectionState.done,
                                error,
                                stackTrace
                            );
                        });
                    }
                    DartRuntimePrimitives.Assert(() =>
                    {
                        if (FutureBuilder<object>.debugRethrowError)
                        {
                            DartRuntimePrimitives.Ignore(new Future<object>(error, stackTrace));
                        }
                        return true;
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    });
                }
            )
        );
        if (!Equals(_snapshot.connectionState, ConnectionState.done))
        {
            _snapshot = _snapshot.inState(ConnectionState.waiting);
        }
    }

    internal virtual void _unsubscribe()
    {
        _activeCallbackIdentity = null;
    }
}
