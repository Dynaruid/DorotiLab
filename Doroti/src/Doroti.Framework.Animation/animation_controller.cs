// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animation_controller.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Animation;

internal enum _AnimationDirection__animation_controller
{
    forward,
    reverse
}

public static partial class Animation_controllerLibrary
{
    internal static Physics.SpringDescription _kFlingSpringDescription = Physics.SpringDescription.CreateWithDampingRatio(mass: 1.0, stiffness: 500.0);
}

public static partial class Animation_controllerLibrary
{
    internal static Physics.Tolerance _kFlingTolerance = new Physics.Tolerance(velocity: double.PositiveInfinity, distance: 0.01);
}

public enum AnimationBehavior
{
    normal,
    preserve
}

public static class AnimationBehaviorMembers
{
    internal static bool _enableAnimations(this AnimationBehavior value) => value switch { AnimationBehavior.normal => !PlatformDispatcher.instance.accessibilityFeatures.disableAnimations, AnimationBehavior.preserve => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
    internal virtual Physics.Simulation? _simulation { get; set; } = default;
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
        System.Diagnostics.Debug.Assert(upperBound >= lowerBound);
        _ticker = vsync.createTicker(_tick);
        _internalSetValue(value ?? lowerBound);
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
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchCreated("animation", "AnimationController", __instance));
        return __instance;
    }

    public virtual Animation<double> view => this;
    public virtual void resync(TickerProvider vsync)
    {
        Ticker oldTicker = _ticker!;
        _ticker = vsync.createTicker(_tick);
        _ticker!.absorbTicker(oldTicker);
    }

    public override double value
    {
        get => _value;
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
        value = lowerBound;
    }

    public virtual double velocity
    {
        get
        {
            if (!isAnimating)
            {
                return 0.0;
            }
            return _simulation!.dx(DartRuntimePrimitives.RequireValue(lastElapsedDuration).inMicroseconds.toDouble() / Duration.microsecondsPerSecond);
        }
    }
    internal virtual void _internalSetValue(double newValue)
    {
        _value = Dart_uiLibrary.clampDouble(newValue, lowerBound, upperBound);
        if (_value == lowerBound)
        {
            _status = AnimationStatus.dismissed;
        }
        else
        {
            if (_value == upperBound)
            {
                _status = AnimationStatus.completed;
            }
            else
            {
                _status = _direction switch { _AnimationDirection__animation_controller.forward => AnimationStatus.forward, _AnimationDirection__animation_controller.reverse => AnimationStatus.reverse, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            }
        }
    }

    public virtual Duration? lastElapsedDuration => _lastElapsedDuration;
    public override bool isAnimating => (_ticker is not null) && _ticker!.isActive;
    public override AnimationStatus status => _status;
    public virtual TickerFuture forward(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (duration is null)
                {
                    throw new FlutterError("AnimationController.forward() called with no default duration.\n" + "The \"duration\" property should be set, either in the constructor or later, before " + "calling the forward() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _direction = _AnimationDirection__animation_controller.forward;
        if (from is not null)
        {
            double from__value18454 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value18454);
        }
        return _animateToInternal(upperBound);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture reverse(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((duration is null) && (reverseDuration is null))
                {
                    throw new FlutterError("AnimationController.reverse() called with no default duration or reverseDuration.\n" + "The \"duration\" or \"reverseDuration\" property should be set, either in the constructor or later, before " + "calling the reverse() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _direction = _AnimationDirection__animation_controller.reverse;
        if (from is not null)
        {
            double from__value19811 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value19811);
        }
        return _animateToInternal(lowerBound);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture toggle(double? from = null)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                Duration? durationLocal = duration;
                if (isForwardOrCompleted)
                {
                    durationLocal ??= reverseDuration;
                }
                if (durationLocal is null)
                {
                    throw new FlutterError("AnimationController.toggle() called with no default duration.\n" + "The \"duration\" property should be set, either in the constructor or later, before " + "calling the toggle() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _direction = isForwardOrCompleted ? _AnimationDirection__animation_controller.reverse : _AnimationDirection__animation_controller.forward;
        if (from is not null)
        {
            double from__value21256 = DartRuntimePrimitives.RequireValue(from);
            value = DartRuntimePrimitives.RequireValue(from__value21256);
        }
        return _animateToInternal(_direction switch { _AnimationDirection__animation_controller.forward => upperBound, _AnimationDirection__animation_controller.reverse => lowerBound, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateTo(double target, Duration? duration = null, Curve curve = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.duration is null) && (duration is null))
                {
                    throw new FlutterError("AnimationController.animateTo() called with no explicit duration and no default duration.\n" + "Either the \"duration\" argument to the animateTo() method should be provided, or the " + "\"duration\" property should be set, either in the constructor or later, before " + "calling the animateTo() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _direction = _AnimationDirection__animation_controller.forward;
        return _animateToInternal(target, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateBack(double target, Duration? duration = null, Curve curve = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.duration is null) && (reverseDuration is null) && (duration is null))
                {
                    throw new FlutterError("AnimationController.animateBack() called with no explicit duration and no default duration or reverseDuration.\n" + "Either the \"duration\" argument to the animateBack() method should be provided, or the " + "\"duration\" or \"reverseDuration\" property should be set, either in the constructor or later, before " + "calling the animateBack() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _direction = _AnimationDirection__animation_controller.reverse;
        return _animateToInternal(target, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TickerFuture _animateToInternal(double target, Duration? duration = null, Curve curve = default!)
    {
        curve ??= Curves.linear;
        var scale = AnimationBehaviorMembers._enableAnimations(animationBehavior) ? 1.0 : 0.05;
        var simulationDuration = duration;
        if (simulationDuration is null)
        {
            DartRuntimePrimitives.Assert(() => !((this.duration is null) && Equals(_direction, _AnimationDirection__animation_controller.forward)));
            DartRuntimePrimitives.Assert(() => !((this.duration is null) && Equals(_direction, _AnimationDirection__animation_controller.reverse) && (reverseDuration is null)));
            double range = upperBound - lowerBound;
            double remainingFraction = double.IsFinite(range) ? ((target - _value).abs() / range) : 1.0;
            Duration directionDuration = (Equals(_direction, _AnimationDirection__animation_controller.reverse) && (reverseDuration is not null)) ? DartRuntimePrimitives.RequireValue(reverseDuration) : DartRuntimePrimitives.RequireValue(this.duration);
            simulationDuration = directionDuration * remainingFraction;
        }
        else
        {
            if (target == value)
            {
                simulationDuration = Duration.zero;
            }
        }
        stop();
        if (Equals(DartRuntimePrimitives.RequireValue(simulationDuration), Duration.zero))
        {
            if (value != target)
            {
                _value = Dart_uiLibrary.clampDouble(target, lowerBound, upperBound);
                notifyListeners();
            }
            _status = Equals(_direction, _AnimationDirection__animation_controller.forward) ? AnimationStatus.completed : AnimationStatus.dismissed;
            _checkStatusChanged();
            return TickerFuture.CreateComplete();
        }
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(simulationDuration) > Duration.zero);
        DartRuntimePrimitives.Assert(() => !isAnimating);
        return _startSimulation(new _InterpolationSimulation__animation_controller(_value, target, DartRuntimePrimitives.RequireValue(simulationDuration), curve, scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture repeat(double? min = null, double? max = null, bool reverse = false, Duration? period = null, long? count = null)
    {
        min ??= lowerBound;
        max ??= upperBound;
        period ??= duration;
        DartRuntimePrimitives.Assert(() =>
            {
                if (period is null)
                {
                    throw new FlutterError("AnimationController.repeat() called without an explicit period and with no default Duration.\n" + "Either the \"period\" argument to the repeat() method should be provided, or the " + "\"duration\" property should be set, either in the constructor or later, before " + "calling the repeat() function.");
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => max >= DartRuntimePrimitives.RequireValue(min));
        DartRuntimePrimitives.Assert(() => (max <= upperBound) && (min >= lowerBound));
        DartRuntimePrimitives.Assert(() => (count is null) || (DartRuntimePrimitives.RequireValue(count) > 0L));
        stop();
        return _startSimulation(new _RepeatingSimulation__animation_controller(_value, DartRuntimePrimitives.RequireValue(min), DartRuntimePrimitives.RequireValue(max), reverse, DartRuntimePrimitives.RequireValue(period), _directionSetter, count));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _directionSetter(_AnimationDirection__animation_controller direction)
    {
        _direction = direction;
        _status = Equals(_direction, _AnimationDirection__animation_controller.forward) ? AnimationStatus.forward : AnimationStatus.reverse;
        _checkStatusChanged();
    }

    public virtual TickerFuture fling(double velocity = 1.0, Physics.SpringDescription? springDescription = null, AnimationBehavior? animationBehavior = null)
    {
        springDescription ??= Animation_controllerLibrary._kFlingSpringDescription;
        _direction = (velocity < 0.0) ? _AnimationDirection__animation_controller.reverse : _AnimationDirection__animation_controller.forward;
        double target = (velocity < 0.0) ? (lowerBound - Animation_controllerLibrary._kFlingTolerance.distance) : (upperBound + Animation_controllerLibrary._kFlingTolerance.distance);
        AnimationBehavior behavior = animationBehavior ?? this.animationBehavior;
        var scale = AnimationBehaviorMembers._enableAnimations(behavior) ? 1.0 : 200.0;
        var simulation = ((Func<Physics.SpringSimulation>)(() =>
{
    var __cascade = new Physics.SpringSimulation(springDescription, value, target, velocity * scale);
    __cascade.tolerance = Animation_controllerLibrary._kFlingTolerance;
    return __cascade;
}))();
        DartRuntimePrimitives.Assert(() => !Equals(simulation.type, Physics.SpringType.underDamped));
        stop();
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateWith(Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        stop();
        _direction = _AnimationDirection__animation_controller.forward;
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TickerFuture animateBackWith(Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        stop();
        _direction = _AnimationDirection__animation_controller.reverse;
        return _startSimulation(simulation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual TickerFuture _startSimulation(Physics.Simulation simulation)
    {
        DartRuntimePrimitives.Assert(() => !isAnimating);
        _simulation = simulation;
        _lastElapsedDuration = Duration.zero;
        _value = Dart_uiLibrary.clampDouble(simulation.x(0.0), lowerBound, upperBound);
        TickerFuture result = _ticker!.start();
        _status = Equals(_direction, _AnimationDirection__animation_controller.forward) ? AnimationStatus.forward : AnimationStatus.reverse;
        _checkStatusChanged();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void stop(bool canceled = true)
    {
        DartRuntimePrimitives.Assert(() => _ticker is not null);
        _simulation = null;
        _lastElapsedDuration = null;
        _ticker!.stop(canceled: canceled);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("AnimationController.dispose() called more than once."), new ErrorDescription($"A given {GetType()} cannot be disposed more than once.\n"), new DiagnosticsProperty<AnimationController>($"The following {GetType()} object was disposed multiple times", this, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _ticker!.dispose();
        _ticker = null;
        clearStatusListeners();
        clearListeners();
    }

    internal virtual void _checkStatusChanged()
    {
        AnimationStatus newStatus = status;
        if (!Equals(_lastReportedStatus, newStatus))
        {
            _lastReportedStatus = newStatus;
            notifyStatusListeners(newStatus);
        }
    }

    internal virtual void _tick(Duration elapsed)
    {
        _lastElapsedDuration = elapsed;
        double elapsedInSeconds = elapsed.inMicroseconds.toDouble() / Duration.microsecondsPerSecond;
        DartRuntimePrimitives.Assert(() => elapsedInSeconds >= 0.0);
        _value = Dart_uiLibrary.clampDouble(_simulation!.x(elapsedInSeconds), lowerBound, upperBound);
        if (_simulation!.isDone(elapsedInSeconds))
        {
            _status = Equals(_direction, _AnimationDirection__animation_controller.forward) ? AnimationStatus.completed : AnimationStatus.dismissed;
            stop(canceled: false);
        }
        notifyListeners();
        _checkStatusChanged();
    }

    public override string toStringDetails()
    {
        var paused = isAnimating ? "" : "; paused";
        var ticker = (_ticker is null) ? "; DISPOSED" : (_ticker!.muted ? "; silenced" : "");
        var label = "";
        DartRuntimePrimitives.Assert(() =>
            {
                if (debugLabel is not null)
                {
                    label = $"; for {debugLabel}";
                }
                return true;
            });
        var more = $"{base.toStringDetails()} {value.toStringAsFixed(3L)}";
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
        _listeners.add(listener);
    }

    public override void removeListener(Action listener)
    {
        bool removed = _listeners.remove(listener);
        if (removed)
        {
            didUnregisterListener();
        }
    }

    public virtual void clearListeners()
    {
        _listeners.clear();
    }

    public virtual void notifyListeners()
    {
        List<Action> localListeners = _listeners.ToList();
        foreach (var listener in localListeners)
        {
            InformationCollector? collector = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector = () => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalListenersMixin>($"The {GetType()} notifying listeners was", this, style: DiagnosticsTreeStyle.errorProperty) };
                    return true;
                });
            try
            {
                if (_listeners.contains(listener))
                {
                    listener();
                }
            }
            catch (Exception exceptionLocal)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "animation library", context: new ErrorDescription($"while notifying listeners for {GetType()}"), informationCollector: collector));
            }
        }
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
        didRegisterListener();
        _statusListeners.add(listener);
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
        bool removed = _statusListeners.remove(listener);
        if (removed)
        {
            didUnregisterListener();
        }
    }

    public virtual void clearStatusListeners()
    {
        _statusListeners.clear();
    }

    public virtual void notifyStatusListeners(AnimationStatus status)
    {
        List<AnimationStatusListener> localListeners = _statusListeners.ToList();
        foreach (var listener in localListeners)
        {
            try
            {
                if (_statusListeners.contains(listener))
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
                        collector = () => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) };
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {GetType()}"), informationCollector: collector));
            }
        }
    }

}

internal class _InterpolationSimulation__animation_controller : Physics.Simulation
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
        _durationInSeconds = duration.inMicroseconds * scale / Duration.microsecondsPerSecond;
        System.Diagnostics.Debug.Assert(duration.inMicroseconds > 0L);
    }

    public override double x(double time)
    {
        double t = Dart_uiLibrary.clampDouble(time / _durationInSeconds, 0.0, 1.0);
        return t switch { 0.0 => _begin, 1.0 => _end, _ => _begin + ((_end - _begin) * _curve.transform(t)) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        double epsilon = tolerance.time;
        return (x(time + epsilon) - x(time - epsilon)) / (2L * epsilon);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time) => time > _durationInSeconds;
}

internal delegate void _DirectionSetter__animation_controller(_AnimationDirection__animation_controller direction);

internal class _RepeatingSimulation__animation_controller : Physics.Simulation
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
                __late__exitTimeInSeconds = DartRuntimePrimitives.RequireValue(count) * _periodInSeconds - _initialT;
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
        _periodInSeconds = period.inMicroseconds / Duration.microsecondsPerSecond;
        _initialT = (max == min) ? 0.0 : ((Dart_uiLibrary.clampDouble(initialValue, min, max) - min) / (max - min) * (period.inMicroseconds / Duration.microsecondsPerSecond));
        System.Diagnostics.Debug.Assert((count is null) || (DartRuntimePrimitives.RequireValue(count) > 0L));
    }

    public override double x(double time)
    {
        DartRuntimePrimitives.Assert(() => time >= 0.0);
        double totalTimeInSeconds = time + _initialT;
        double t = totalTimeInSeconds / _periodInSeconds % 1.0;
        bool isPlayingReverse = (checked(checked((long)(totalTimeInSeconds / _periodInSeconds))) & 1L) != 0L;
        if (reverse && isPlayingReverse)
        {
            directionSetter(_AnimationDirection__animation_controller.reverse);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(max, min, t));
        }
        else
        {
            directionSetter(_AnimationDirection__animation_controller.forward);
            return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(min, max, t));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time) => (max - min) / _periodInSeconds;
    public override bool isDone(double time)
    {
        return (count is not null) && time >= _exitTimeInSeconds;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
