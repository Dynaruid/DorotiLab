// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animations.dart
using Doroti.Runtime;

namespace Doroti.Framework.Animation;

internal class _AlwaysCompleteAnimation__animations : Animation<double>
{
    internal _AlwaysCompleteAnimation__animations() { }

    public override void addListener(Action listener) { }

    public override void removeListener(Action listener) { }

    public override void addStatusListener(AnimationStatusListener listener) { }

    public override void removeStatusListener(AnimationStatusListener listener) { }

    public override AnimationStatus status => AnimationStatus.completed;
    public override double value => 1.0;

    public override string ToString() => "kAlwaysCompleteAnimation";
}

public static partial class AnimationsLibrary
{
    public static Animation<double> kAlwaysCompleteAnimation =
        new _AlwaysCompleteAnimation__animations();
}

internal class _AlwaysDismissedAnimation__animations : Animation<double>
{
    internal _AlwaysDismissedAnimation__animations() { }

    public override void addListener(Action listener) { }

    public override void removeListener(Action listener) { }

    public override void addStatusListener(AnimationStatusListener listener) { }

    public override void removeStatusListener(AnimationStatusListener listener) { }

    public override AnimationStatus status => AnimationStatus.dismissed;
    public override double value => 0.0;

    public override string ToString() => "kAlwaysDismissedAnimation";
}

public static partial class AnimationsLibrary
{
    public static Animation<double> kAlwaysDismissedAnimation =
        new _AlwaysDismissedAnimation__animations();
}

public class AlwaysStoppedAnimation<T> : Animation<T>
{
    private T __field_value = default!;
    public override T value
    {
        get => __field_value;
    }

    public AlwaysStoppedAnimation(T value)
    {
        __field_value = value;
    }

    public override void addListener(Action listener) { }

    public override void removeListener(Action listener) { }

    public override void addStatusListener(AnimationStatusListener listener) { }

    public override void removeStatusListener(AnimationStatusListener listener) { }

    public override AnimationStatus status => AnimationStatus.forward;

    public override string toStringDetails()
    {
        return $"{base.toStringDetails()} {value}; paused";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface AnimationWithParentMixin<T>
{
    public Animation<T> parent { get; }
    public void addListener(Action listener);
    public void removeListener(Action listener);
    public void addStatusListener(AnimationStatusListener listener);
    public void removeStatusListener(AnimationStatusListener listener);
    public AnimationStatus status { get; }
}

public class ProxyAnimation
    : Animation<double>,
        AnimationLazyListenerMixin,
        AnimationLocalListenersMixin,
        AnimationLocalStatusListenersMixin
{
    internal virtual AnimationStatus? _status { get; set; } = default;
    internal virtual double? _value { get; set; } = default;
    internal virtual Animation<double>? _parent { get; set; } = default;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual HashedObserverList<Action> _listeners { get; set; } =
        new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } =
        new ObserverList<AnimationStatusListener>();

    public ProxyAnimation(Animation<double>? animation = null)
    {
        _parent = animation;
        if (_parent is null)
        {
            _status = AnimationStatus.dismissed;
            _value = 0.0;
        }
    }

    public virtual Animation<double>? parent
    {
        get => _parent;
        set
        {
            var __value = value;
            if (Equals(__value, _parent))
            {
                return;
            }
            if (_parent is not null)
            {
                _status = _parent!.status;
                _value = _parent!.value;
                if (isListening)
                {
                    didStopListening();
                }
            }
            _parent = __value;
            if (_parent is not null)
            {
                if (isListening)
                {
                    didStartListening();
                }
                if (_value != _parent!.value)
                {
                    notifyListeners();
                }
                if (!Equals(_status, _parent!.status))
                {
                    notifyStatusListeners(_parent!.status);
                }
                _status = null;
                _value = null;
            }
        }
    }

    public virtual void didStartListening()
    {
        if (_parent is not null)
        {
            _parent!.addListener(notifyListeners);
            _parent!.addStatusListener(notifyStatusListeners);
        }
    }

    public virtual void didStopListening()
    {
        if (_parent is not null)
        {
            _parent!.removeListener(notifyListeners);
            _parent!.removeStatusListener(notifyStatusListeners);
        }
    }

    public override AnimationStatus status =>
        (_parent is not null) ? _parent!.status : DartRuntimePrimitives.RequireValue(_status);
    public override double value =>
        (_parent is not null) ? _parent!.value : DartRuntimePrimitives.RequireValue(_value);

    public override string ToString()
    {
        if (parent is null)
        {
            return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ProxyAnimation")}(null; {base.toStringDetails()} {value.toStringAsFixed(3L)})";
        }
        return $"{parent}➩{objectRuntimeTypeFunctions.objectRuntimeType(this, "ProxyAnimation")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 0L);
        if (_listenerCounter == 0L)
        {
            didStartListening();
        }
        _listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 1L);
        _listenerCounter -= 1L;
        if (_listenerCounter == 0L)
        {
            didStopListening();
        }
    }

    public virtual bool isListening => _listenerCounter > 0L;

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
                collector = () =>
                    new List<DiagnosticsNode>
                    {
                        new DiagnosticsProperty<AnimationLocalListenersMixin>(
                            $"The {GetType()} notifying listeners was",
                            this,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    };
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
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription($"while notifying listeners for {GetType()}"),
                        informationCollector: collector
                    )
                );
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
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<AnimationLocalStatusListenersMixin>(
                                $"The {GetType()} notifying status listeners was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        };
                    return true;
                });
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription(
                            $"while notifying status listeners for {GetType()}"
                        ),
                        informationCollector: collector
                    )
                );
            }
        }
    }
}

public class ReverseAnimation
    : Animation<double>,
        AnimationLazyListenerMixin,
        AnimationLocalStatusListenersMixin
{
    public virtual Animation<double> parent { get; private set; } = default!;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } =
        new ObserverList<AnimationStatusListener>();

    public ReverseAnimation(Animation<double> parent)
    {
        this.parent = parent;
    }

    public override void addListener(Action listener)
    {
        didRegisterListener();
        parent.addListener(listener);
    }

    public override void removeListener(Action listener)
    {
        parent.removeListener(listener);
        didUnregisterListener();
    }

    public virtual void didStartListening()
    {
        parent.addStatusListener(_statusChangeHandler);
    }

    public virtual void didStopListening()
    {
        parent.removeStatusListener(_statusChangeHandler);
    }

    internal virtual void _statusChangeHandler(AnimationStatus status)
    {
        notifyStatusListeners(_reverseStatus(status));
    }

    public override AnimationStatus status => _reverseStatus(parent.status);
    public override double value => 1.0 - parent.value;

    internal virtual AnimationStatus _reverseStatus(AnimationStatus status)
    {
        return status switch
        {
            AnimationStatus.forward => AnimationStatus.reverse,
            AnimationStatus.reverse => AnimationStatus.forward,
            AnimationStatus.completed => AnimationStatus.dismissed,
            AnimationStatus.dismissed => AnimationStatus.completed,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{parent}➪{objectRuntimeTypeFunctions.objectRuntimeType(this, "ReverseAnimation")}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 0L);
        if (_listenerCounter == 0L)
        {
            didStartListening();
        }
        _listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 1L);
        _listenerCounter -= 1L;
        if (_listenerCounter == 0L)
        {
            didStopListening();
        }
    }

    public virtual bool isListening => _listenerCounter > 0L;

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
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<AnimationLocalStatusListenersMixin>(
                                $"The {GetType()} notifying status listeners was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        };
                    return true;
                });
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription(
                            $"while notifying status listeners for {GetType()}"
                        ),
                        informationCollector: collector
                    )
                );
            }
        }
    }
}

public class CurvedAnimation : Animation<double>, AnimationWithParentMixin<double>
{
    public virtual Animation<double> parent { get; private set; } = default!;
    public virtual Curve curve { get; set; } = default!;
    public virtual Curve? reverseCurve { get; set; } = default;
    internal virtual AnimationStatus? _curveDirection { get; set; } = default;
    public virtual bool isDisposed { get; set; } = false;

    public CurvedAnimation(Animation<double> parent, Curve curve, Curve? reverseCurve = null)
    {
        this.parent = parent;
        this.curve = curve;
        this.reverseCurve = reverseCurve;
    }

    internal virtual void _updateCurveDirection(AnimationStatus status)
    {
        _curveDirection = AnimationStatusMembers.isAnimating(status)
            ? (_curveDirection ?? status)
            : null;
    }

    internal virtual bool _useForwardCurve
    {
        get
        {
            return (reverseCurve is null)
                || (!Equals(_curveDirection ?? parent.status, AnimationStatus.reverse));
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        isDisposed = true;
        parent.removeStatusListener(_updateCurveDirection);
    }

    public override double value
    {
        get
        {
            Curve? activeCurve = _useForwardCurve ? curve : reverseCurve;
            double t = parent.value;
            if (activeCurve is null)
            {
                return t;
            }
            if ((t == 0.0) || (t == 1.0))
            {
                DartRuntimePrimitives.Assert(() =>
                {
                    double transformedValue = activeCurve.transform(t);
                    double roundedTransformedValue = transformedValue.round().toDouble();
                    if (roundedTransformedValue != t)
                    {
                        throw new FlutterError(
                            $"Invalid curve endpoint at {t}.\n"
                                + "Curves must map 0.0 to near zero and 1.0 to near one but "
                                + $"{DartRuntimePrimitives.RuntimeType(activeCurve)} mapped {t} to {transformedValue}, which "
                                + $"is near {roundedTransformedValue}."
                        );
                    }
                    return true;
                });
                return t;
            }
            return activeCurve.transform(t);
        }
    }

    public override string ToString()
    {
        if (reverseCurve is null)
        {
            return $"{parent}➩{curve}";
        }
        if (_useForwardCurve)
        {
            return $"{parent}➩{curve}ₒₙ/{reverseCurve}";
        }
        return $"{parent}➩{curve}/{reverseCurve}ₒₙ";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addListener(Action listener) => parent.addListener(listener);

    public override void removeListener(Action listener) => parent.removeListener(listener);

    public override void addStatusListener(AnimationStatusListener listener) =>
        parent.addStatusListener(listener);

    public override void removeStatusListener(AnimationStatusListener listener) =>
        parent.removeStatusListener(listener);

    public override AnimationStatus status => parent.status;
}

internal enum _TrainHoppingMode__animations
{
    minimize,
    maximize,
}

public class TrainHoppingAnimation
    : Animation<double>,
        AnimationEagerListenerMixin,
        AnimationLocalListenersMixin,
        AnimationLocalStatusListenersMixin
{
    internal virtual Animation<double>? _currentTrain { get; set; } = default;
    internal virtual Animation<double>? _nextTrain { get; set; } = default;
    internal virtual _TrainHoppingMode__animations? _mode { get; set; } = default;
    public virtual Action? onSwitchedTrain { get; set; } = default;
    internal virtual AnimationStatus? _lastStatus { get; set; } = default;
    internal virtual double? _lastValue { get; set; } = default;
    public virtual HashedObserverList<Action> _listeners { get; set; } =
        new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } =
        new ObserverList<AnimationStatusListener>();

    public TrainHoppingAnimation(
        Animation<double> _currentTrain,
        Animation<double>? _nextTrain,
        Action? onSwitchedTrain = null
    )
    {
        this._currentTrain = _currentTrain;
        this._nextTrain = _nextTrain;
        this.onSwitchedTrain = onSwitchedTrain;
    }

    public virtual Animation<double>? currentTrain => _currentTrain;

    internal virtual void _statusChangeHandler(AnimationStatus status)
    {
        DartRuntimePrimitives.Assert(() => _currentTrain is not null);
        if (!Equals(status, _lastStatus))
        {
            notifyStatusListeners(status);
            _lastStatus = status;
        }
        DartRuntimePrimitives.Assert(() => _lastStatus is not null);
    }

    public override AnimationStatus status => _currentTrain!.status;

    internal virtual void _valueChangeHandler()
    {
        DartRuntimePrimitives.Assert(() => _currentTrain is not null);
        var hop = false;
        if (_nextTrain is not null)
        {
            DartRuntimePrimitives.Assert(() => _mode is not null);
            hop = DartRuntimePrimitives.RequireValue(_mode) switch
            {
                _TrainHoppingMode__animations.minimize => _nextTrain!.value <= _currentTrain!.value,
                _TrainHoppingMode__animations.maximize => _nextTrain!.value >= _currentTrain!.value,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            if (hop)
            {
                (
                    (Func<Animation<double>>)(
                        () =>
                        {
                            var __cascade = _currentTrain!;
                            __cascade.removeStatusListener(_statusChangeHandler);
                            __cascade.removeListener(_valueChangeHandler);
                            return __cascade;
                        }
                    )
                )();
                _currentTrain = _nextTrain;
                _nextTrain = null;
                _currentTrain!.addStatusListener(_statusChangeHandler);
                _statusChangeHandler(_currentTrain!.status);
            }
        }
        double newValue = value;
        if (newValue != _lastValue)
        {
            notifyListeners();
            _lastValue = newValue;
        }
        DartRuntimePrimitives.Assert(() => _lastValue is not null);
        if (hop && (onSwitchedTrain is not null))
        {
            onSwitchedTrain!();
        }
    }

    public override double value => _currentTrain!.value;

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        DartRuntimePrimitives.Assert(() => _currentTrain is not null);
        _currentTrain!.removeStatusListener(_statusChangeHandler);
        _currentTrain!.removeListener(_valueChangeHandler);
        _currentTrain = null;
        _nextTrain?.removeListener(_valueChangeHandler);
        _nextTrain = null;
        clearListeners();
        clearStatusListeners();
    }

    public override string ToString()
    {
        if (_nextTrain is not null)
        {
            return $"{currentTrain}➩{objectRuntimeTypeFunctions.objectRuntimeType(this, "TrainHoppingAnimation")}(next: {_nextTrain})";
        }
        return $"{currentTrain}➩{objectRuntimeTypeFunctions.objectRuntimeType(this, "TrainHoppingAnimation")}(no next)";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener() { }

    public virtual void didUnregisterListener() { }

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
                collector = () =>
                    new List<DiagnosticsNode>
                    {
                        new DiagnosticsProperty<AnimationLocalListenersMixin>(
                            $"The {GetType()} notifying listeners was",
                            this,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    };
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
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription($"while notifying listeners for {GetType()}"),
                        informationCollector: collector
                    )
                );
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
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<AnimationLocalStatusListenersMixin>(
                                $"The {GetType()} notifying status listeners was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        };
                    return true;
                });
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription(
                            $"while notifying status listeners for {GetType()}"
                        ),
                        informationCollector: collector
                    )
                );
            }
        }
    }
}

public abstract class CompoundAnimation<T>
    : Animation<T>,
        AnimationLazyListenerMixin,
        AnimationLocalListenersMixin,
        AnimationLocalStatusListenersMixin
{
    public virtual Animation<T> first { get; private set; } = default!;
    public virtual Animation<T> next { get; private set; } = default!;
    internal virtual AnimationStatus? _lastStatus { get; set; } = default;
    internal virtual T? _lastValue { get; set; } = default;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual HashedObserverList<Action> _listeners { get; set; } =
        new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } =
        new ObserverList<AnimationStatusListener>();

    protected CompoundAnimation(Animation<T> first, Animation<T> next)
    {
        this.first = first;
        this.next = next;
    }

    public virtual void didStartListening()
    {
        first.addListener(_maybeNotifyListeners);
        first.addStatusListener(_maybeNotifyStatusListeners);
        next.addListener(_maybeNotifyListeners);
        next.addStatusListener(_maybeNotifyStatusListeners);
    }

    public virtual void didStopListening()
    {
        first.removeListener(_maybeNotifyListeners);
        first.removeStatusListener(_maybeNotifyStatusListeners);
        next.removeListener(_maybeNotifyListeners);
        next.removeStatusListener(_maybeNotifyStatusListeners);
    }

    public override AnimationStatus status =>
        AnimationStatusMembers.isAnimating(next.status) ? next.status : first.status;

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "CompoundAnimation")}({first}, {next})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _maybeNotifyStatusListeners(AnimationStatus __unused0)
    {
        if (!Equals(status, _lastStatus))
        {
            _lastStatus = status;
            notifyStatusListeners(status);
        }
    }

    internal virtual void _maybeNotifyListeners()
    {
        if (!EqualityComparer<T>.Default.Equals(value, _lastValue))
        {
            _lastValue = value;
            notifyListeners();
        }
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 0L);
        if (_listenerCounter == 0L)
        {
            didStartListening();
        }
        _listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => _listenerCounter >= 1L);
        _listenerCounter -= 1L;
        if (_listenerCounter == 0L)
        {
            didStopListening();
        }
    }

    public virtual bool isListening => _listenerCounter > 0L;

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
                collector = () =>
                    new List<DiagnosticsNode>
                    {
                        new DiagnosticsProperty<AnimationLocalListenersMixin>(
                            $"The {GetType()} notifying listeners was",
                            this,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    };
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
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription($"while notifying listeners for {GetType()}"),
                        informationCollector: collector
                    )
                );
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
                    collector = () =>
                        new List<DiagnosticsNode>
                        {
                            new DiagnosticsProperty<AnimationLocalStatusListenersMixin>(
                                $"The {GetType()} notifying status listeners was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        };
                    return true;
                });
                FlutterError.reportError(
                    new FlutterErrorDetails(
                        exception: exceptionLocal,
                        stack: stackLocal,
                        library: "animation library",
                        context: new ErrorDescription(
                            $"while notifying status listeners for {GetType()}"
                        ),
                        informationCollector: collector
                    )
                );
            }
        }
    }
}

public class AnimationMean : CompoundAnimation<double>
{
    public AnimationMean(Animation<double> left, Animation<double> right)
        : base(first: left, next: right) { }

    public override double value => (first.value + next.value) / 2.0;
}

public class AnimationMax<T> : CompoundAnimation<T>
    where T : struct
{
    public AnimationMax(Animation<T> first, Animation<T> next)
        : base(first: first, next: next) { }

    public override T value => DartRuntimePrimitives.Max(first.value, next.value);
}

public class AnimationMin<T> : CompoundAnimation<T>
    where T : struct
{
    public AnimationMin(Animation<T> first, Animation<T> next)
        : base(first: first, next: next) { }

    public override T value => DartRuntimePrimitives.Min(first.value, next.value);
}
