#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/scheduler/ticker.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Scheduler;

public delegate void TickerCallback(Duration elapsed);

public interface TickerProvider
{
    public Ticker createTicker(Action<Duration> onTick);
}

public class Ticker
{
    internal virtual TickerFuture? _future { get; set; } = default;
    public virtual bool forceFrames { get; set; } = false;
    internal virtual bool _muted { get; set; } = false;
    internal virtual Duration? _startTime { get; set; } = default;
    internal virtual Action<Duration> _onTick { get; private set; } = default!;
    internal virtual long? _animationId { get; set; } = default;
    public virtual string? debugLabel { get; private set; }
    internal virtual StackTrace _debugCreationStack { get; set; } = default!;

    public Ticker(Action<Duration> _onTick, string? debugLabel = null)
    {
        this._onTick = _onTick;
        this.debugLabel = debugLabel;
    }

    public virtual bool muted
    {
        get => _muted;
        set
        {
            var __value = value;
            if ((__value == muted))
            {
                return;
            }
            _muted = __value;
            if (__value)
            {
                unscheduleTick();
            }
            else
            {
                if (shouldScheduleTick)
                {
                    scheduleTick();
                }
            }
        }
    }
    public virtual bool isTicking
    {
        get
        {
            if ((_future is null))
            {
                return false;
            }
            if (muted)
            {
                return false;
            }
            if (SchedulerBinding.instance.framesEnabled)
            {
                return true;
            }
            if ((!object.Equals(SchedulerBinding.instance.schedulerPhase, SchedulerPhase.idle)))
            {
                return true;
            }
            return false;
        }
    }
    public virtual bool isActive => (_future is not null);
    public virtual TickerFuture start()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (isActive)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A ticker was started twice."), new ErrorDescription("A ticker that is already active cannot be started again without first stopping it."), describeForError("The affected ticker was") });
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (_startTime is null));
        _future = new TickerFuture();
        RecordAnimationPhase(DorotiFramePhase.animationStart);
        if (shouldScheduleTick)
        {
            scheduleTick();
        }
        if (((FoundationRuntimePorts.EnumIndex(SchedulerBinding.instance.schedulerPhase) > FoundationRuntimePorts.EnumIndex(SchedulerPhase.idle)) && (FoundationRuntimePorts.EnumIndex(SchedulerBinding.instance.schedulerPhase) < FoundationRuntimePorts.EnumIndex(SchedulerPhase.postFrameCallbacks))))
        {
            _startTime = SchedulerBinding.instance.currentFrameTimeStamp;
        }
        return _future!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode describeForError(string name)
    {
        return new DiagnosticsProperty<Ticker>(name, this, description: ToString(debugIncludeStack: true));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void stop(bool canceled = false)
    {
        if (!isActive)
        {
            return;
        }
        TickerFuture localFuture = _future!;
        RecordAnimationPhase(DorotiFramePhase.animationEnd);
        _future = null;
        _startTime = null;
        DartRuntimePrimitives.Assert(() => !isActive);
        unscheduleTick();
        if (canceled)
        {
            localFuture._cancel(this);
        }
        else
        {
            localFuture._complete();
        }
    }

    public virtual bool scheduled => (_animationId is not null);
    public virtual bool shouldScheduleTick => ((!muted && isActive) && !scheduled);
    internal virtual void _tick(Duration timeStamp)
    {
        DartRuntimePrimitives.Assert(() => isTicking);
        DartRuntimePrimitives.Assert(() => scheduled);
        _animationId = null;
        _startTime ??= timeStamp;
        _onTick((timeStamp - DartRuntimePrimitives.RequireValue(_startTime)));
        if (shouldScheduleTick)
        {
            scheduleTick(rescheduling: true);
        }
    }

    public virtual void scheduleTick(bool rescheduling = false)
    {
        DartRuntimePrimitives.Assert(() => !scheduled);
        DartRuntimePrimitives.Assert(() => shouldScheduleTick);
        if (forceFrames)
        {
            SchedulerBinding.instance.scheduleForcedFrame();
        }
        else
        {
            SchedulerBinding.instance.scheduleFrame();
        }
        _animationId = SchedulerBinding.instance.scheduleFrameCallback(_tick, rescheduling: rescheduling, scheduleNewFrame: false);
    }

    public virtual void unscheduleTick()
    {
        if (scheduled)
        {
            SchedulerBinding.instance.cancelFrameCallbackWithId(DartRuntimePrimitives.RequireValue(_animationId));
            _animationId = null;
        }
        DartRuntimePrimitives.Assert(() => !shouldScheduleTick);
    }

    public virtual void absorbTicker(Ticker originalTicker)
    {
        DartRuntimePrimitives.Assert(() => !isActive);
        DartRuntimePrimitives.Assert(() => (_future is null));
        DartRuntimePrimitives.Assert(() => (_startTime is null));
        DartRuntimePrimitives.Assert(() => (_animationId is null));
        DartRuntimePrimitives.Assert(() => (((originalTicker._future is not null)) || ((originalTicker._startTime is null))));
        if ((originalTicker._future is not null))
        {
            _future = originalTicker._future;
            _startTime = originalTicker._startTime;
            if (shouldScheduleTick)
            {
                scheduleTick();
            }
            originalTicker._future = null;
            originalTicker.unscheduleTick();
        }
        originalTicker.dispose();
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        if ((_future is not null))
        {
            TickerFuture localFuture__13113 = _future!;
            RecordAnimationPhase(DorotiFramePhase.animationEnd);
            _future = null;
            DartRuntimePrimitives.Assert(() => !isActive);
            unscheduleTick();
            localFuture__13113._cancel(this);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _startTime = Duration.zero;
                return true;
        });
    }

    private void RecordAnimationPhase(DorotiFramePhase phase)
    {
        SchedulerBinding.instance.platformDispatcher.frameTrace.RecordTicker(
            phase,
            global::System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this),
            debugLabel ?? _onTick.Target?.GetType().Name ?? "Ticker");
    }

    public virtual string ToString(bool debugIncludeStack = false)
    {
        var buffer = new StringBuffer();
        buffer.write($"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Ticker"))}(");
        DartRuntimePrimitives.Assert(() =>
            {
                buffer.write((debugLabel ?? ""));
                return true;
            });
        buffer.write(")");
        DartRuntimePrimitives.Assert(() =>
            {
                if (debugIncludeStack)
                {
                    buffer.writeln();
                    buffer.writeln($"The stack trace when the {this.GetType()} was actually created was:");
                    FlutterError.defaultStackFilter(_debugCreationStack.ToString().trimRight().split("\n")).forEach(buffer.writeln);
                }
                return true;
            });
        return buffer.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TickerFuture : Future
{
    internal virtual Completer<object?> _primaryCompleter { get; private set; } = new Completer<object?>();
    internal virtual Completer<object?>? _secondaryCompleter { get; set; } = default;
    internal virtual bool? _completed { get; set; } = default;

    public TickerFuture()
    {
    }

    public static TickerFuture CreateComplete()
    {
        var __instance = new TickerFuture();
        __instance._complete();
        return __instance;
    }

    internal virtual void _complete()
    {
        DartRuntimePrimitives.Assert(() => (_completed is null));
        _completed = true;
        _primaryCompleter.complete();
        _secondaryCompleter?.complete();
    }

    internal virtual void _cancel(Ticker ticker)
    {
        DartRuntimePrimitives.Assert(() => (_completed is null));
        _completed = false;
        _secondaryCompleter?.completeError(new TickerCanceled(ticker));
    }

    public virtual void whenCompleteOrCancel(Action callback)
    {
        void thunk(object value)
        {
            callback();
        }
        _ = orCancel.then(thunk, onError: thunk);
    }

    public virtual Future orCancel
    {
        get
        {
            if ((_secondaryCompleter is null))
            {
                _secondaryCompleter = new Completer<object?>();
                if (_completed is bool _completed__value17796)
                {
                    if (DartRuntimePrimitives.RequireValue(_completed__value17796))
                    {
                        _secondaryCompleter!.complete();
                    }
                    else
                    {
                        _secondaryCompleter!.completeError(new TickerCanceled());
                    }
                }
            }
            return _secondaryCompleter!.future;
        }
    }
    public virtual Stream<object?> asStream()
    {
        return _primaryCompleter.future.asStream();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future catchError(Delegate onError, Func<object, bool>? test = null)
    {
        return _primaryCompleter.future.catchError(onError, test: test);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<R> then<R>(Func<object?, object> onValue, Delegate? onError = null)
    {
        return _primaryCompleter.future.then<R>(onValue, onError: onError);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future timeout(Duration timeLimit, Func<object>? onTimeout = null)
    {
        return _primaryCompleter.future.timeout(timeLimit, onTimeout: onTimeout);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future whenComplete(Func<object> action)
    {
        return _primaryCompleter.future.whenComplete(action);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({((_completed is null) ? "active" : (DartRuntimePrimitives.RequireValue(_completed) ? "complete" : "canceled"))})";
}

public class TickerCanceled : Exception
{
    public virtual Ticker? ticker { get; private set; }

    public TickerCanceled(Ticker? ticker = null)
    {
        this.ticker = ticker;
    }

    public override string ToString()
    {
        if ((ticker is not null))
        {
            return $"This ticker was canceled: {ticker}";
        }
        return "The ticker was canceled before the \"orCancel\" property was first used.";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
