// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/async.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public abstract class StreamBuilderBase<T, S> : StatefulWidget
{
    public virtual Stream<T>? stream { get; private set; }

    protected StreamBuilderBase(global::Doroti.Framework.Foundation.Key? key = null, Stream<T>? stream = default!) : base(key: key)
    {
        this.stream = stream;
    }

    public abstract S initial();
    public virtual S afterConnected(S current) => current;
    public abstract S afterData(S current, T data);
    public virtual S afterError(S current, object error, global::System.Diagnostics.StackTrace stackTrace) => current;
    public virtual S afterDone(S current) => current;
    public virtual S afterDisconnected(S current) => current;
    public abstract Widget build(BuildContext context, S currentSummary);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StreamBuilderBaseState__async<T, S>());
}

internal class _StreamBuilderBaseState__async<T, S> : State<StreamBuilderBase<T, S>>
{
    internal virtual StreamSubscription<T>? _subscription { get; set; } = default;
    internal virtual S _summary { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _summary = this.widget.initial();
        _subscribe();
    }

    public override void didUpdateWidget(StreamBuilderBase<T, S> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((StreamBuilderBase<T, S>)oldWidget).stream, ((StreamBuilderBase<T, S>)(object)this.widget).stream)))
        {
            if ((this._subscription is not null))
            {
                _unsubscribe();
                _summary = this.widget.afterDisconnected(this._summary);
            }
            _subscribe();
        }
    }

    public override Widget build(BuildContext context) => this.widget.build(context, this._summary);
    public override void dispose()
    {
        _unsubscribe();
        base.dispose();
    }

    internal virtual void _subscribe()
    {
        if ((((StreamBuilderBase<T, S>)(object)this.widget).stream is not null))
        {
            _subscription = ((StreamBuilderBase<T, S>)(object)this.widget).stream!.listen(((global::System.Action<T>)((data) =>
            {
                setState(((global::System.Action)(() =>
                {
                    _summary = this.widget.afterData(this._summary, data);
                })));
            })), onError: ((error, stackTrace) =>
            {
                setState(((global::System.Action)(() =>
                {
                    _summary = this.widget.afterError(this._summary, error, stackTrace);
                })));
            }), onDone: ((global::System.Action)(() =>
            {
                setState(((global::System.Action)(() =>
                {
                    _summary = this.widget.afterDone(this._summary);
                })));
            })));
            _summary = this.widget.afterConnected(this._summary);
        }
    }

    internal virtual void _unsubscribe()
    {
        if ((this._subscription is not null))
        {
            DartRuntimePrimitives.Ignore(this._subscription!.cancel());
            _subscription = null;
        }
    }

}

public enum ConnectionState
{
    none,
    waiting,
    active,
    done
}

public class AsyncSnapshot<T>
{
    public virtual ConnectionState connectionState { get; private set; } = default!;
    public virtual T? data { get; private set; }
    public virtual object? error { get; private set; }
    public virtual global::System.Diagnostics.StackTrace? stackTrace { get; private set; }

    public AsyncSnapshot(ConnectionState connectionState, T? data, object? error, global::System.Diagnostics.StackTrace? stackTrace)
    {
        this.connectionState = connectionState;
        this.data = data;
        this.error = error;
        this.stackTrace = stackTrace;
        System.Diagnostics.Debug.Assert(((data is null) || (error is null)));
        System.Diagnostics.Debug.Assert(((stackTrace is null) || (error is not null)));
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

    public static AsyncSnapshot<T> CreateWithError(ConnectionState state, object error, global::System.Diagnostics.StackTrace stackTrace = default!)
    {
        return new AsyncSnapshot<T>(state, default, error, stackTrace);
    }

    public virtual T requireData
    {
        get
        {
            if (this.hasData)
            {
                return ((T)(object?)this.data!);
            }
            if (this.hasError)
            {
                Dart_coreLibrary.throwWithStackTrace(this.error!, this.stackTrace!);
            }
            throw new InvalidOperationException("Snapshot has neither data nor error");
            return default!;
        }
    }
    public virtual AsyncSnapshot<T> inState(ConnectionState state) => new AsyncSnapshot<T>(state, this.data, this.error, this.stackTrace);
    public virtual bool hasData => DartRuntimePrimitives.ConvertValue<bool>((this.data is not null));
    public virtual bool hasError => DartRuntimePrimitives.ConvertValue<bool>((this.error is not null));
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "AsyncSnapshot"))}({this.connectionState}, {this.data}, {this.error}, {this.stackTrace})";
    public override bool Equals(object? other)
    {
        var __other = other as AsyncSnapshot<T>;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((((__other is AsyncSnapshot<T>) && (object.Equals(((AsyncSnapshot<T>)((AsyncSnapshot<T>)__other)).connectionState, this.connectionState))) && EqualityComparer<T>.Default.Equals(((AsyncSnapshot<T>)((AsyncSnapshot<T>)__other)).data, this.data)) && (object.Equals(((AsyncSnapshot<T>)((AsyncSnapshot<T>)__other)).error, this.error))) && (object.Equals(((AsyncSnapshot<T>)((AsyncSnapshot<T>)__other)).stackTrace, this.stackTrace)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.connectionState, this.data, this.error));
}

public delegate Widget AsyncWidgetBuilder<T>(BuildContext context, AsyncSnapshot<T> snapshot);

public class StreamBuilder<T> : StreamBuilderBase<T, AsyncSnapshot<T>>
{
    public virtual global::System.Func<BuildContext, AsyncSnapshot<T>, Widget> builder { get; private set; } = default!;
    public virtual T? initialData { get; private set; }

    public StreamBuilder(global::Doroti.Framework.Foundation.Key? key = null, T? initialData = default, Stream<T>? stream = default!, global::System.Func<BuildContext, AsyncSnapshot<T>, Widget> builder = default!) : base(key: key, stream: stream)
    {
        this.initialData = initialData;
        this.builder = builder;
    }

    public override AsyncSnapshot<T> initial() => ((this.initialData is null) ? AsyncSnapshot<T>.CreateNothing() : AsyncSnapshot<T>.CreateWithData(ConnectionState.none, ((T?)(object?)this.initialData)!));
    public override AsyncSnapshot<T> afterConnected(AsyncSnapshot<T> current) => current.inState(ConnectionState.waiting);
    public override AsyncSnapshot<T> afterData(AsyncSnapshot<T> current, T data)
    {
        return AsyncSnapshot<T>.CreateWithData(ConnectionState.active, data);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AsyncSnapshot<T> afterError(AsyncSnapshot<T> current, object error, global::System.Diagnostics.StackTrace stackTrace)
    {
        return AsyncSnapshot<T>.CreateWithError(ConnectionState.active, error, stackTrace);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override AsyncSnapshot<T> afterDone(AsyncSnapshot<T> current) => current.inState(ConnectionState.done);
    public override AsyncSnapshot<T> afterDisconnected(AsyncSnapshot<T> current) => current.inState(ConnectionState.none);
    public override Widget build(BuildContext context, AsyncSnapshot<T> currentSummary) => this.builder(context, currentSummary);
}

public class FutureBuilder<T> : StatefulWidget
{
    public virtual Future<T>? future { get; private set; }
    public virtual global::System.Func<BuildContext, AsyncSnapshot<T>, Widget> builder { get; private set; } = default!;
    public virtual T? initialData { get; private set; }
    public static bool debugRethrowError = false;

    public FutureBuilder(global::Doroti.Framework.Foundation.Key? key = null, Future<T>? future = default!, T? initialData = default, global::System.Func<BuildContext, AsyncSnapshot<T>, Widget> builder = default!) : base(key: key)
    {
        this.future = future;
        this.initialData = initialData;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _FutureBuilderState__async<T>());
}

internal class _FutureBuilderState__async<T> : State<FutureBuilder<T>>
{
    internal virtual object? _activeCallbackIdentity { get; set; } = default;
    internal virtual AsyncSnapshot<T> _snapshot { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _snapshot = ((((FutureBuilder<T>)(object)this.widget).initialData is null) ? AsyncSnapshot<T>.CreateNothing() : AsyncSnapshot<T>.CreateWithData(ConnectionState.none, ((T?)(object?)((FutureBuilder<T>)(object)this.widget).initialData)!));
        _subscribe();
    }

    public override void didUpdateWidget(FutureBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((object.Equals(((FutureBuilder<T>)oldWidget).future, ((FutureBuilder<T>)(object)this.widget).future)))
        {
            return;
        }
        if ((this._activeCallbackIdentity is not null))
        {
            _unsubscribe();
            _snapshot = this._snapshot.inState(ConnectionState.none);
        }
        _subscribe();
    }

    public override Widget build(BuildContext context) => this.widget.builder(context, this._snapshot);
    public override void dispose()
    {
        _unsubscribe();
        base.dispose();
    }

    internal virtual void _subscribe()
    {
        if ((((FutureBuilder<T>)(object)this.widget).future is null))
        {
            return;
        }
        var callbackIdentity = new object();
        _activeCallbackIdentity = callbackIdentity;
        DartRuntimePrimitives.Ignore(((FutureBuilder<T>)(object)this.widget).future!.then(((data) =>
        {
            if ((object.Equals(this._activeCallbackIdentity, callbackIdentity)))
            {
                setState(((global::System.Action)(() =>
                {
                    _snapshot = AsyncSnapshot<T>.CreateWithData(ConnectionState.done, data);
                })));
            }
        }), onError: ((error, stackTrace) =>
        {
            if ((object.Equals(this._activeCallbackIdentity, callbackIdentity)))
            {
                setState(((global::System.Action)(() =>
                {
                    _snapshot = AsyncSnapshot<T>.CreateWithError(ConnectionState.done, error, stackTrace);
                })));
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if (FutureBuilder<object>.debugRethrowError)
                    {
                        DartRuntimePrimitives.Ignore(new Future<object>(error, stackTrace));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
        })));
        if ((!object.Equals(((AsyncSnapshot<T>)this._snapshot).connectionState, ConnectionState.done)))
        {
            _snapshot = this._snapshot.inState(ConnectionState.waiting);
        }
    }

    internal virtual void _unsubscribe()
    {
        _activeCallbackIdentity = null;
    }

}

