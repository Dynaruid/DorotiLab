// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation_controller.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Animation;

internal enum _AnimationDirection__animation_controller
{
    forward,
    reverse
}

public static partial class Animation_controllerLibrary
{
    internal static global::Doroti.Framework.Physics.SpringDescription _kFlingSpringDescription = global::Doroti.Framework.Physics.SpringDescription.CreateWithDampingRatio(mass: 1.0, stiffness: 500.0);
}

public static partial class Animation_controllerLibrary
{
    internal static global::Doroti.Framework.Physics.Tolerance _kFlingTolerance = new global::Doroti.Framework.Physics.Tolerance(velocity: double.PositiveInfinity, distance: 0.01);
}

public enum AnimationBehavior
{
    normal,
    preserve
}

public static class AnimationBehaviorMembers
{
    internal static bool _enableAnimations(this AnimationBehavior value) => (value switch { AnimationBehavior.normal => !PlatformDispatcher.instance.accessibilityFeatures.disableAnimations, AnimationBehavior.preserve => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}

public class AnimationController : Animation<double>, AnimationEagerListenerMixin, AnimationLocalListenersMixin, AnimationLocalStatusListenersMixin
{
    public virtual double lowerBound { get; private set; } = default!;
    public virtual double upperBound { get; private set; } = default!;
    public virtual string? debugLabel { get; private set; }
    public virtual AnimationBehavior animationBehavior { get; private set; } = default!;
    public virtual Duration? duration { get; set; } = default;
    public virtual Duration? reverseDuration { get; set; } = default;
    internal virtual Ticker? _ticker { get; set; } = default;
    internal virtual global::Doroti.Framework.Physics.Simulation? _simulation { get; set; } = default;
    internal virtual double _value { get; set; } = default!;
    internal virtual Duration? _lastElapsedDuration { get; set; } = default;
    internal virtual _AnimationDirection__animation_controller _direction { get; set; } = _AnimationDirection__animation_controller.forward;
    internal virtual AnimationStatus _status { get; set; } = default!;
    internal virtual AnimationStatus _lastReportedStatus { get; set; } = AnimationStatus.dismissed;
    public virtual HashedObserverList<Action> _listeners { get; set; } = new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } = new ObserverList<AnimationStatusListener>();

    public AnimationController(double? value = null, Duration? duration = null, Duration? reverseDuration = null, string? debugLabel = null, double lowerBound = 0.0, double upperBound = 1.0, AnimationBehavior animationBehavior = AnimationBehavior.normal, TickerProvider vsync = default!)
    {
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.debugLabel = debugLabel;
        this.lowerBound = lowerBound;
        this.upperBound = upperBound;
        this.animationBehavior = animationBehavior;
        System.Diagnostics.Debug.Assert((upperBound >= lowerBound));
        this._ticker = vsync.createTicker(this._tick);
        this._internalSetValue(value ?? lowerBound);
    }

    public static AnimationController CreateUnbounded(double value = 0.0, Duration? duration = null, Duration? reverseDuration = null, string? debugLabel = null, TickerProvider vsync = default!, AnimationBehavior animationBehavior = AnimationBehavior.preserve)
    {
        var __instance = new AnimationController(
            value: value,
            duration: duration,
            reverseDuration: reverseDuration,
            debugLabel: debugLabel,
            lowerBound: double.NegativeInfinity,
            upperBound: double.PositiveInfinity,
            animationBehavior: animationBehavior,
            vsync: vsync);
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchCreated("animation", "AnimationController", __instance));
        return __instance;
    }

    public virtual Animation<double> view => this;
    public virtual void resync(TickerProvider vsync)
    {
        Ticker oldTicker = this._ticker!;
        _ticker = vsync.createTicker(this._tick);
        this._ticker!.absorbTicker(oldTicker);
    }

    public override double value
    {
        get => this._value;
        set
        {
            var newValue = value;
            stop();
            _internalSetValue(newValue);
            notifyListeners();
            _checkStatusChanged();
        }
    }
    public virtual void reset()
    {
        value = this.lowerBound;
    }

    public virtual double velocity
    {
        get
        {
            if (!this.isAnimating)
            {
                return 0.0;
            }
            return this._simulation!.dx((DartRuntimePrimitives.RequireValue(this.lastElapsedDuration).inMicroseconds.toDouble() / Duration.microsecondsPerSecond));
            return default!;
        }
    }
    internal virtual void _internalSetValue(double newValue)
    {
        _value = Dart_uiLibrary.clampDouble(newValue, this.lowerBound, this.upperBound);
        if ((this._value == this.lowerBound))
        {
            _status = AnimationStatus.dismissed;
        }
        else
        {
            if ((this._value == this.upperBound))
            {
                _status = AnimationStatus.completed;
            }
            else
            {
                _status = (this._direction switch { _AnimationDirection__animation_controller.forward => AnimationStatus.forward, _AnimationDirection__animation_controller.reverse => AnimationStatus.reverse, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            }
        }
    }

    public virtual Duration? lastElapsedDuration => this._lastElapsedDuration;
    public override bool isAnimating => ((this._ticker is not null) && this._ticker!.isActive);
    public override AnimationStatus status => this._status;
    public virtual TickerFuture forward(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.duration is null))
                {
                    throw new FlutterError("AnimationController.forward() called with no default duration.\n" + "The \"duration\" property should be set, either in the constructor or later, before " + "calling the forward() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _direction = _AnimationDirection__animation_controller.forward;
        if ((from is not null))
        {
            double from__value18454 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value18454);
        }
        return _animateToInternal(this.upperBound);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture reverse(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this.duration is null) && (this.reverseDuration is null)))
                {
                    throw new FlutterError("AnimationController.reverse() called with no default duration or reverseDuration.\n" + "The \"duration\" or \"reverseDuration\" property should be set, either in the constructor or later, before " + "calling the reverse() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _direction = _AnimationDirection__animation_controller.reverse;
        if ((from is not null))
        {
            double from__value19811 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value19811);
        }
        return _animateToInternal(this.lowerBound);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture toggle(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Duration? durationLocal = this.duration;
                if (isForwardOrCompleted)
                {
                    durationLocal ??= this.reverseDuration;
                }
                if ((durationLocal is null))
                {
                    throw new FlutterError("AnimationController.toggle() called with no default duration.\n" + "The \"duration\" property should be set, either in the constructor or later, before " + "calling the toggle() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _direction = (isForwardOrCompleted ? _AnimationDirection__animation_controller.reverse : _AnimationDirection__animation_controller.forward);
        if ((from is not null))
        {
            double from__value21256 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value21256);
        }
        return _animateToInternal((this._direction switch { _AnimationDirection__animation_controller.forward => this.upperBound, _AnimationDirection__animation_controller.reverse => this.lowerBound, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateTo(double target, Duration? duration = null, Curve curve = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this.duration is null) && (duration is null)))
                {
                    throw new FlutterError("AnimationController.animateTo() called with no explicit duration and no default duration.\n" + "Either the \"duration\" argument to the animateTo() method should be provided, or the " + "\"duration\" property should be set, either in the constructor or later, before " + "calling the animateTo() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _direction = _AnimationDirection__animation_controller.forward;
        return _animateToInternal(target, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateBack(double target, Duration? duration = null, Curve curve = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((this.duration is null) && (this.reverseDuration is null)) && (duration is null)))
                {
                    throw new FlutterError("AnimationController.animateBack() called with no explicit duration and no default duration or reverseDuration.\n" + "Either the \"duration\" argument to the animateBack() method should be provided, or the " + "\"duration\" or \"reverseDuration\" property should be set, either in the constructor or later, before " + "calling the animateBack() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _direction = _AnimationDirection__animation_controller.reverse;
        return _animateToInternal(target, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TickerFuture _animateToInternal(double target, Duration? duration = null, Curve curve = default!)
    {
        curve ??= Curves.linear;
        var scale = (AnimationBehaviorMembers._enableAnimations(this.animationBehavior) ? 1.0 : 0.05);
        var simulationDuration = duration;
        if ((simulationDuration is null))
        {
            DartRuntimePrimitives.Assert(() => !(((this.duration is null) && (object.Equals(this._direction, _AnimationDirection__animation_controller.forward)))));
            DartRuntimePrimitives.Assert(() => !((((this.duration is null) && (object.Equals(this._direction, _AnimationDirection__animation_controller.reverse))) && (this.reverseDuration is null))));
            double range = (this.upperBound - this.lowerBound);
            double remainingFraction = (double.IsFinite(range) ? (((target - this._value)).abs() / range) : 1.0);
            Duration directionDuration = ((((object.Equals(this._direction, _AnimationDirection__animation_controller.reverse)) && (this.reverseDuration is not null))) ? DartRuntimePrimitives.RequireValue(this.reverseDuration) : DartRuntimePrimitives.RequireValue(this.duration));
            simulationDuration = (directionDuration * remainingFraction);
        }
        else
        {
            if ((target == this.value))
            {
                simulationDuration = Duration.zero;
            }
        }
        stop();
        if ((object.Equals(DartRuntimePrimitives.RequireValue(simulationDuration), Duration.zero)))
        {
            if ((this.value != target))
            {
                _value = Dart_uiLibrary.clampDouble(target, this.lowerBound, this.upperBound);
                notifyListeners();
            }
            _status = (((object.Equals(this._direction, _AnimationDirection__animation_controller.forward))) ? AnimationStatus.completed : AnimationStatus.dismissed);
            _checkStatusChanged();
            return new TickerFuture();
        }
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(simulationDuration) > Duration.zero));
        DartRuntimePrimitives.Assert(() => !this.isAnimating);
        return _startSimulation(new _InterpolationSimulation__animation_controller(this._value, target, DartRuntimePrimitives.RequireValue(simulationDuration), curve, scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture repeat(double? min = null, double? max = null, bool reverse = false, Duration? period = null, long? count = null)
    {
        min ??= this.lowerBound;
        max ??= this.upperBound;
        period ??= this.duration;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((period is null))
                {
                    throw new FlutterError("AnimationController.repeat() called without an explicit period and with no default Duration.\n" + "Either the \"period\" argument to the repeat() method should be provided, or the " + "\"duration\" property should be set, either in the constructor or later, before " + "calling the repeat() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => (max >= DartRuntimePrimitives.RequireValue(min)));
        DartRuntimePrimitives.Assert(() => ((max <= this.upperBound) && (min >= this.lowerBound)));
        DartRuntimePrimitives.Assert(() => ((count is null) || (DartRuntimePrimitives.RequireValue(count) > 0L)));
        stop();
        return _startSimulation(new _RepeatingSimulation__animation_controller(this._value, DartRuntimePrimitives.RequireValue(min), DartRuntimePrimitives.RequireValue(max), reverse, DartRuntimePrimitives.RequireValue(period), this._directionSetter, count));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _directionSetter(_AnimationDirection__animation_controller direction)
    {
        _direction = direction;
        _status = (((object.Equals(this._direction, _AnimationDirection__animation_controller.forward))) ? AnimationStatus.forward : AnimationStatus.reverse);
        _checkStatusChanged();
    }

    public virtual TickerFuture fling(double velocity = 1.0, global::Doroti.Framework.Physics.SpringDescription? springDescription = null, AnimationBehavior? animationBehavior = null)
    {
        springDescription ??= Animation_controllerLibrary._kFlingSpringDescription;
        _direction = ((velocity < 0.0) ? _AnimationDirection__animation_controller.reverse : _AnimationDirection__animation_controller.forward);
        double target = ((velocity < 0.0) ? (this.lowerBound - ((global::Doroti.Framework.Physics.Tolerance)Animation_controllerLibrary._kFlingTolerance).distance) : (this.upperBound + ((global::Doroti.Framework.Physics.Tolerance)Animation_controllerLibrary._kFlingTolerance).distance));
        AnimationBehavior behavior = (animationBehavior ?? this.animationBehavior);
        var scale = (AnimationBehaviorMembers._enableAnimations(behavior) ? 1.0 : 200.0);
        var simulation = ((Func<global::Doroti.Framework.Physics.SpringSimulation>)(() =>
{
    var __cascade = new global::Doroti.Framework.Physics.SpringSimulation(springDescription, this.value, target, (velocity * scale));
    __cascade.tolerance = Animation_controllerLibrary._kFlingTolerance;
    return __cascade;
}))();
        DartRuntimePrimitives.Assert(() => (!object.Equals(((global::Doroti.Framework.Physics.SpringSimulation)simulation).type, global::Doroti.Framework.Physics.SpringType.underDamped)));
        stop();
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateWith(global::Doroti.Framework.Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        stop();
        _direction = _AnimationDirection__animation_controller.forward;
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateBackWith(global::Doroti.Framework.Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        stop();
        _direction = _AnimationDirection__animation_controller.reverse;
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TickerFuture _startSimulation(global::Doroti.Framework.Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => !this.isAnimating);
        _simulation = simulation;
        _lastElapsedDuration = Duration.zero;
        _value = Dart_uiLibrary.clampDouble(simulation.x(0.0), this.lowerBound, this.upperBound);
        TickerFuture result = this._ticker!.start();
        _status = (((object.Equals(this._direction, _AnimationDirection__animation_controller.forward))) ? AnimationStatus.forward : AnimationStatus.reverse);
        _checkStatusChanged();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void stop(bool canceled = true)
    {
        DartRuntimePrimitives.Assert(() => (this._ticker is not null));
        _simulation = null;
        _lastElapsedDuration = null;
        this._ticker!.stop(canceled: canceled);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._ticker is null))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("AnimationController.dispose() called more than once."), new ErrorDescription($"A given {this.GetType()} cannot be disposed more than once.\n"), new DiagnosticsProperty<AnimationController>($"The following {this.GetType()} object was disposed multiple times", this, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._ticker!.dispose();
        _ticker = null;
        clearStatusListeners();
        clearListeners();
    }

    internal virtual void _checkStatusChanged()
    {
        AnimationStatus newStatus = this.status;
        if ((!object.Equals(this._lastReportedStatus, newStatus)))
        {
            _lastReportedStatus = newStatus;
            notifyStatusListeners(newStatus);
        }
    }

    internal virtual void _tick(Duration elapsed)
    {
        _lastElapsedDuration = elapsed;
        double elapsedInSeconds = (elapsed.inMicroseconds.toDouble() / Duration.microsecondsPerSecond);
        DartRuntimePrimitives.Assert(() => (elapsedInSeconds >= 0.0));
        _value = Dart_uiLibrary.clampDouble(this._simulation!.x(elapsedInSeconds), this.lowerBound, this.upperBound);
        if (this._simulation!.isDone(elapsedInSeconds))
        {
            _status = (((object.Equals(this._direction, _AnimationDirection__animation_controller.forward))) ? AnimationStatus.completed : AnimationStatus.dismissed);
            stop(canceled: false);
        }
        notifyListeners();
        _checkStatusChanged();
    }

    public override string toStringDetails()
    {
        var paused = (this.isAnimating ? "" : "; paused");
        var ticker = ((this._ticker is null) ? "; DISPOSED" : ((this._ticker!.muted ? "; silenced" : "")));
        var label = "";
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.debugLabel is not null))
                {
                    label = $"; for {this.debugLabel}";
                }
                return true;
            });
        var more = $"{base.toStringDetails()} {this.value.toStringAsFixed(3L)}";
        return $"{more}{paused}{ticker}{label}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener()
    {
    }

    public virtual void didUnregisterListener()
    {
    }

    public override void addListener(Action listener)
    {
        didRegisterListener();
        this._listeners.add(listener);
    }

    public override void removeListener(Action listener)
    {
        bool removed = this._listeners.remove(listener);
        if (removed)
        {
            didUnregisterListener();
        }
    }

    public virtual void clearListeners()
    {
        this._listeners.clear();
    }

    public virtual void notifyListeners()
    {
        List<Action> localListeners = this._listeners.ToList();
        foreach (var listener in localListeners)
        {
            InformationCollector? collector = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalListenersMixin>($"The {this.GetType()} notifying listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    return true;
                });
            try
            {
                if (this._listeners.contains(listener))
                {
                    listener();
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "animation library", context: new ErrorDescription($"while notifying listeners for {this.GetType()}"), informationCollector: collector));
            }
        }
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
        didRegisterListener();
        this._statusListeners.add(listener);
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
        bool removed = this._statusListeners.remove(listener);
        if (removed)
        {
            didUnregisterListener();
        }
    }

    public virtual void clearStatusListeners()
    {
        this._statusListeners.clear();
    }

    public virtual void notifyStatusListeners(AnimationStatus status)
    {
        List<AnimationStatusListener> localListeners = this._statusListeners.ToList();
        foreach (var listener in localListeners)
        {
            try
            {
                if (this._statusListeners.contains(listener))
                {
                    listener(status);
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                InformationCollector? collector = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {this.GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {this.GetType()}"), informationCollector: collector));
            }
        }
    }

}

internal class _InterpolationSimulation__animation_controller : global::Doroti.Framework.Physics.Simulation
{
    internal virtual double _durationInSeconds { get; private set; } = default!;
    internal virtual double _begin { get; private set; } = default!;
    internal virtual double _end { get; private set; } = default!;
    internal virtual Curve _curve { get; private set; } = default!;

    internal _InterpolationSimulation__animation_controller(double _begin, double _end, Duration duration, Curve _curve, double scale)
    {
        this._begin = _begin;
        this._end = _end;
        this._curve = _curve;
        this._durationInSeconds = (((duration.inMicroseconds * scale)) / Duration.microsecondsPerSecond);
        System.Diagnostics.Debug.Assert((duration.inMicroseconds > 0L));
    }

    public override double x(double time)
    {
        double t = Dart_uiLibrary.clampDouble((time / this._durationInSeconds), 0.0, 1.0);
        return (t switch { 0.0 => this._begin, 1.0 => this._end, _ => (this._begin + (((this._end - this._begin)) * this._curve.transform(t))) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        double epsilon = ((global::Doroti.Framework.Physics.Tolerance)tolerance).time;
        return (((x((time + epsilon)) - x((time - epsilon)))) / ((2L * epsilon)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time) => (time > this._durationInSeconds);
}

internal delegate void _DirectionSetter__animation_controller(_AnimationDirection__animation_controller direction);

internal class _RepeatingSimulation__animation_controller : global::Doroti.Framework.Physics.Simulation
{
    public virtual double min { get; private set; } = default!;
    public virtual double max { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual long? count { get; private set; }
    public virtual Action<_AnimationDirection__animation_controller> directionSetter { get; private set; } = default!;
    internal virtual double _periodInSeconds { get; private set; } = default!;
    internal virtual double _initialT { get; private set; } = default!;
    private bool __late__exitTimeInSeconds_initialized;
    private double __late__exitTimeInSeconds = default!;
    internal virtual double _exitTimeInSeconds
    {
        get
        {
            if (!__late__exitTimeInSeconds_initialized)
            {
                __late__exitTimeInSeconds = (((DartRuntimePrimitives.RequireValue(this.count) * this._periodInSeconds)) - this._initialT);
                __late__exitTimeInSeconds_initialized = true;
            }
            return __late__exitTimeInSeconds;
        }
    }

    internal _RepeatingSimulation__animation_controller(double initialValue, double min, double max, bool reverse, Duration period, Action<_AnimationDirection__animation_controller> directionSetter, long? count)
    {
        this.min = min;
        this.max = max;
        this.reverse = reverse;
        this.directionSetter = directionSetter;
        this.count = count;
        this._periodInSeconds = (period.inMicroseconds / Duration.microsecondsPerSecond);
        this._initialT = (((max == min)) ? 0.0 : (((((Dart_uiLibrary.clampDouble(initialValue, min, max) - min)) / ((max - min)))) * ((period.inMicroseconds / Duration.microsecondsPerSecond))));
        System.Diagnostics.Debug.Assert(((count is null) || (DartRuntimePrimitives.RequireValue(count) > 0L)));
    }

    public override double x(double time)
    {
        DartRuntimePrimitives.Assert(() => (time >= 0.0));
        double totalTimeInSeconds = (time + this._initialT);
        double t = (((totalTimeInSeconds / this._periodInSeconds)) % 1.0);
        bool isPlayingReverse = ((checked((long)(((checked((long)(totalTimeInSeconds / this._periodInSeconds)))))) & 1L) != 0L);
        if ((this.reverse && isPlayingReverse))
        {
            this.directionSetter(_AnimationDirection__animation_controller.reverse);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.max, this.min, t));
        }
        else
        {
            this.directionSetter(_AnimationDirection__animation_controller.forward);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(this.min, this.max, t));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time) => (((this.max - this.min)) / this._periodInSeconds);
    public override bool isDone(double time)
    {
        return ((this.count is not null) && ((time >= this._exitTimeInSeconds)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
