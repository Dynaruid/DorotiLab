// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/animation/animations.dart
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

internal class _AlwaysCompleteAnimation__animations : Animation<double>
{
    internal _AlwaysCompleteAnimation__animations()
    {
    }

    public override void addListener(Action listener)
    {
    }

    public override void removeListener(Action listener)
    {
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
    }

    public override AnimationStatus status => AnimationStatus.completed;
    public override double value => 1.0;
    public override string ToString() => "kAlwaysCompleteAnimation";
}

public static partial class AnimationsLibrary
{
    public static Animation<double> kAlwaysCompleteAnimation = new _AlwaysCompleteAnimation__animations();
}

internal class _AlwaysDismissedAnimation__animations : Animation<double>
{
    internal _AlwaysDismissedAnimation__animations()
    {
    }

    public override void addListener(Action listener)
    {
    }

    public override void removeListener(Action listener)
    {
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
    }

    public override AnimationStatus status => AnimationStatus.dismissed;
    public override double value => 0.0;
    public override string ToString() => "kAlwaysDismissedAnimation";
}

public static partial class AnimationsLibrary
{
    public static Animation<double> kAlwaysDismissedAnimation = new _AlwaysDismissedAnimation__animations();
}

public class AlwaysStoppedAnimation<T> : Animation<T>
{
    private T __field_value = default!;
    public override T value { get => __field_value; }

    public AlwaysStoppedAnimation(T value)
    {
        this.__field_value = value;
    }

    public override void addListener(Action listener)
    {
    }

    public override void removeListener(Action listener)
    {
    }

    public override void addStatusListener(AnimationStatusListener listener)
    {
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
    }

    public override AnimationStatus status => AnimationStatus.forward;
    public override string toStringDetails()
    {
        return $"{base.toStringDetails()} {this.value}; paused";
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

public class ProxyAnimation : Animation<double>, AnimationLazyListenerMixin, AnimationLocalListenersMixin, AnimationLocalStatusListenersMixin
{
    internal virtual AnimationStatus? _status { get; set; } = default;
    internal virtual double? _value { get; set; } = default;
    internal virtual Animation<double>? _parent { get; set; } = default;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual HashedObserverList<Action> _listeners { get; set; } = new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } = new ObserverList<AnimationStatusListener>();

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
        get => this._parent;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._parent)))
            {
                return;
            }
            if ((this._parent is not null))
            {
                _status = this._parent!.status;
                _value = this._parent!.value;
                if (isListening)
                {
                    didStopListening();
                }
            }
            _parent = __value;
            if ((this._parent is not null))
            {
                if (isListening)
                {
                    didStartListening();
                }
                if ((this._value != this._parent!.value))
                {
                    notifyListeners();
                }
                if ((!object.Equals(this._status, this._parent!.status)))
                {
                    notifyStatusListeners(this._parent!.status);
                }
                _status = null;
                _value = null;
            }
        }
    }
    public virtual void didStartListening()
    {
        if ((this._parent is not null))
        {
            this._parent!.addListener((Action)notifyListeners);
            this._parent!.addStatusListener((AnimationStatusListener)notifyStatusListeners);
        }
    }

    public virtual void didStopListening()
    {
        if ((this._parent is not null))
        {
            this._parent!.removeListener((Action)notifyListeners);
            this._parent!.removeStatusListener((AnimationStatusListener)notifyStatusListeners);
        }
    }

    public override AnimationStatus status => ((this._parent is not null) ? this._parent!.status : DartRuntimePrimitives.RequireValue(this._status));
    public override double value => ((this._parent is not null) ? this._parent!.value : DartRuntimePrimitives.RequireValue(this._value));
    public override string ToString()
    {
        if ((this.parent is null))
        {
            return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ProxyAnimation"))}(null; {base.toStringDetails()} {this.value.toStringAsFixed(3L)})";
        }
        return $"{this.parent}➩{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ProxyAnimation"))}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 0L));
        if ((this._listenerCounter == 0L))
        {
            didStartListening();
        }
        this._listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 1L));
        this._listenerCounter -= 1L;
        if ((this._listenerCounter == 0L))
        {
            didStopListening();
        }
    }

    public virtual bool isListening => (this._listenerCounter > 0L);
    public override void addListener(Action listener)
    {
        didRegisterListener();
        this._listeners.add(listener);
    }

    public override void removeListener(Action listener)
    {
        bool removed__4206 = this._listeners.remove(listener);
        if (removed__4206)
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
        List<Action> localListeners__4969 = this._listeners.ToList();
        foreach (var listener__5037 in localListeners__4969)
        {
            InformationCollector? collector__5095 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__5095 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalListenersMixin>($"The {this.GetType()} notifying listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    return true;
                });
            try
            {
                if (this._listeners.contains(listener__5037))
                {
                    listener__5037();
                }
            }
            catch (Exception exception__5520)
            {
                var stack__5531 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__5520, stack: stack__5531, library: "animation library", context: new ErrorDescription($"while notifying listeners for {this.GetType()}"), informationCollector: collector__5095));
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
        bool removed__7458 = this._statusListeners.remove(listener);
        if (removed__7458)
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
        List<AnimationStatusListener> localListeners__8291 = this._statusListeners.ToList();
        foreach (var listener__8365 in localListeners__8291)
        {
            try
            {
                if (this._statusListeners.contains(listener__8365))
                {
                    listener__8365(status);
                }
            }
            catch (Exception exception__8511)
            {
                var stack__8522 = new System.Diagnostics.StackTrace();
                InformationCollector? collector__8561 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector__8561 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {this.GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__8511, stack: stack__8522, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {this.GetType()}"), informationCollector: collector__8561));
            }
        }
    }

}

public class ReverseAnimation : Animation<double>, AnimationLazyListenerMixin, AnimationLocalStatusListenersMixin
{
    public virtual Animation<double> parent { get; private set; } = default!;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } = new ObserverList<AnimationStatusListener>();

    public ReverseAnimation(Animation<double> parent)
    {
        this.parent = parent;
    }

    public override void addListener(Action listener)
    {
        didRegisterListener();
        this.parent.addListener((Action)listener);
    }

    public override void removeListener(Action listener)
    {
        this.parent.removeListener((Action)listener);
        didUnregisterListener();
    }

    public virtual void didStartListening()
    {
        this.parent.addStatusListener((AnimationStatusListener)this._statusChangeHandler);
    }

    public virtual void didStopListening()
    {
        this.parent.removeStatusListener((AnimationStatusListener)this._statusChangeHandler);
    }

    internal virtual void _statusChangeHandler(AnimationStatus status)
    {
        notifyStatusListeners(_reverseStatus(status));
    }

    public override AnimationStatus status => _reverseStatus(((Animation<double>)this.parent).status);
    public override double value => (1.0 - ((Animation<double>)this.parent).value);
    internal virtual AnimationStatus _reverseStatus(AnimationStatus status)
    {
        return (status switch { AnimationStatus.forward => AnimationStatus.reverse, AnimationStatus.reverse => AnimationStatus.forward, AnimationStatus.completed => AnimationStatus.dismissed, AnimationStatus.dismissed => AnimationStatus.completed, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{this.parent}➪{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ReverseAnimation"))}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 0L));
        if ((this._listenerCounter == 0L))
        {
            didStartListening();
        }
        this._listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 1L));
        this._listenerCounter -= 1L;
        if ((this._listenerCounter == 0L))
        {
            didStopListening();
        }
    }

    public virtual bool isListening => (this._listenerCounter > 0L);
    public override void addStatusListener(AnimationStatusListener listener)
    {
        didRegisterListener();
        this._statusListeners.add(listener);
    }

    public override void removeStatusListener(AnimationStatusListener listener)
    {
        bool removed__7458 = this._statusListeners.remove(listener);
        if (removed__7458)
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
        List<AnimationStatusListener> localListeners__8291 = this._statusListeners.ToList();
        foreach (var listener__8365 in localListeners__8291)
        {
            try
            {
                if (this._statusListeners.contains(listener__8365))
                {
                    listener__8365(status);
                }
            }
            catch (Exception exception__8511)
            {
                var stack__8522 = new System.Diagnostics.StackTrace();
                InformationCollector? collector__8561 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector__8561 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {this.GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__8511, stack: stack__8522, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {this.GetType()}"), informationCollector: collector__8561));
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
        _curveDirection = (global::Doroti.Framework.Animation.AnimationStatusMembers.isAnimating(status) ? (this._curveDirection ?? status) : null);
    }

    internal virtual bool _useForwardCurve
    {
        get
        {
            return ((this.reverseCurve is null) || (!object.Equals(((this._curveDirection ?? ((Animation<double>)this.parent).status)), AnimationStatus.reverse)));
            return default!;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        isDisposed = true;
        this.parent.removeStatusListener((AnimationStatusListener)this._updateCurveDirection);
    }

    public override double value
    {
        get
        {
            Curve? activeCurve__14481 = (this._useForwardCurve ? this.curve : this.reverseCurve);
            double t__14554 = ((Animation<double>)this.parent).value;
            if ((activeCurve__14481 is null))
            {
                return t__14554;
            }
            if (((t__14554 == 0.0) || (t__14554 == 1.0)))
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        double transformedValue__14696 = activeCurve__14481.transform(t__14554);
                        double roundedTransformedValue__14762 = transformedValue__14696.round().toDouble();
                        if ((roundedTransformedValue__14762 != t__14554))
                        {
                            throw new FlutterError($"Invalid curve endpoint at {t__14554}.\n" + "Curves must map 0.0 to near zero and 1.0 to near one but " + $"{DartRuntimePrimitives.RuntimeType(activeCurve__14481)} mapped {t__14554} to {transformedValue__14696}, which " + $"is near {roundedTransformedValue__14762}.");
                        }
                        return true;
                    });
                return t__14554;
            }
            return activeCurve__14481.transform(t__14554);
            return default!;
        }
    }
    public override string ToString()
    {
        if ((this.reverseCurve is null))
        {
            return $"{this.parent}➩{this.curve}";
        }
        if (this._useForwardCurve)
        {
            return $"{this.parent}➩{this.curve}ₒₙ/{this.reverseCurve}";
        }
        return $"{this.parent}➩{this.curve}/{this.reverseCurve}ₒₙ";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addListener(Action listener) => this.parent.addListener((Action)listener);
    public override void removeListener(Action listener) => this.parent.removeListener((Action)listener);
    public override void addStatusListener(AnimationStatusListener listener) => this.parent.addStatusListener((AnimationStatusListener)listener);
    public override void removeStatusListener(AnimationStatusListener listener) => this.parent.removeStatusListener((AnimationStatusListener)listener);
    public override AnimationStatus status => ((Animation<double>)this.parent).status;
}

internal enum _TrainHoppingMode__animations
{
    minimize,
    maximize
}

public class TrainHoppingAnimation : Animation<double>, AnimationEagerListenerMixin, AnimationLocalListenersMixin, AnimationLocalStatusListenersMixin
{
    internal virtual Animation<double>? _currentTrain { get; set; } = default;
    internal virtual Animation<double>? _nextTrain { get; set; } = default;
    internal virtual _TrainHoppingMode__animations? _mode { get; set; } = default;
    public virtual Action? onSwitchedTrain { get; set; } = default;
    internal virtual AnimationStatus? _lastStatus { get; set; } = default;
    internal virtual double? _lastValue { get; set; } = default;
    public virtual HashedObserverList<Action> _listeners { get; set; } = new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } = new ObserverList<AnimationStatusListener>();

    public TrainHoppingAnimation(Animation<double> _currentTrain, Animation<double>? _nextTrain, Action? onSwitchedTrain = null)
    {
        this._currentTrain = _currentTrain;
        this._nextTrain = _nextTrain;
        this.onSwitchedTrain = onSwitchedTrain;
    }

    public virtual Animation<double>? currentTrain => this._currentTrain;
    internal virtual void _statusChangeHandler(AnimationStatus status)
    {
        DartRuntimePrimitives.Assert(() => (this._currentTrain is not null));
        if ((!object.Equals(status, this._lastStatus)))
        {
            notifyStatusListeners(status);
            _lastStatus = status;
        }
        DartRuntimePrimitives.Assert(() => (this._lastStatus is not null));
    }

    public override AnimationStatus status => this._currentTrain!.status;
    internal virtual void _valueChangeHandler()
    {
        DartRuntimePrimitives.Assert(() => (this._currentTrain is not null));
        var hop__18975 = false;
        if ((this._nextTrain is not null))
        {
            DartRuntimePrimitives.Assert(() => (this._mode is not null));
            hop__18975 = (DartRuntimePrimitives.RequireValue(this._mode) switch { _TrainHoppingMode__animations.minimize => (this._nextTrain!.value <= this._currentTrain!.value), _TrainHoppingMode__animations.maximize => (this._nextTrain!.value >= this._currentTrain!.value), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            if (hop__18975)
            {
                ((Func<Animation<double>>)(() =>
{
    var __cascade = this._currentTrain!;
    __cascade.removeStatusListener(this._statusChangeHandler);
    __cascade.removeListener(this._valueChangeHandler);
    return __cascade;
}))();
                _currentTrain = this._nextTrain;
                _nextTrain = null;
                this._currentTrain!.addStatusListener((AnimationStatusListener)this._statusChangeHandler);
                _statusChangeHandler(this._currentTrain!.status);
            }
        }
        double newValue__19603 = this.value;
        if ((newValue__19603 != this._lastValue))
        {
            notifyListeners();
            _lastValue = newValue__19603;
        }
        DartRuntimePrimitives.Assert(() => (this._lastValue is not null));
        if ((hop__18975 && (this.onSwitchedTrain is not null)))
        {
            this.onSwitchedTrain!();
        }
    }

    public override double value => this._currentTrain!.value;
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        DartRuntimePrimitives.Assert(() => (this._currentTrain is not null));
        this._currentTrain!.removeStatusListener((AnimationStatusListener)this._statusChangeHandler);
        this._currentTrain!.removeListener((Action)this._valueChangeHandler);
        _currentTrain = null;
        this._nextTrain?.removeListener(this._valueChangeHandler);
        _nextTrain = null;
        clearListeners();
        clearStatusListeners();
    }

    public override string ToString()
    {
        if ((this._nextTrain is not null))
        {
            return $"{this.currentTrain}➩{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TrainHoppingAnimation"))}(next: {this._nextTrain})";
        }
        return $"{this.currentTrain}➩{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TrainHoppingAnimation"))}(no next)";
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
        bool removed__4206 = this._listeners.remove(listener);
        if (removed__4206)
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
        List<Action> localListeners__4969 = this._listeners.ToList();
        foreach (var listener__5037 in localListeners__4969)
        {
            InformationCollector? collector__5095 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__5095 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalListenersMixin>($"The {this.GetType()} notifying listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    return true;
                });
            try
            {
                if (this._listeners.contains(listener__5037))
                {
                    listener__5037();
                }
            }
            catch (Exception exception__5520)
            {
                var stack__5531 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__5520, stack: stack__5531, library: "animation library", context: new ErrorDescription($"while notifying listeners for {this.GetType()}"), informationCollector: collector__5095));
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
        bool removed__7458 = this._statusListeners.remove(listener);
        if (removed__7458)
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
        List<AnimationStatusListener> localListeners__8291 = this._statusListeners.ToList();
        foreach (var listener__8365 in localListeners__8291)
        {
            try
            {
                if (this._statusListeners.contains(listener__8365))
                {
                    listener__8365(status);
                }
            }
            catch (Exception exception__8511)
            {
                var stack__8522 = new System.Diagnostics.StackTrace();
                InformationCollector? collector__8561 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector__8561 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {this.GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__8511, stack: stack__8522, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {this.GetType()}"), informationCollector: collector__8561));
            }
        }
    }

}

public abstract class CompoundAnimation<T> : Animation<T>, AnimationLazyListenerMixin, AnimationLocalListenersMixin, AnimationLocalStatusListenersMixin
{
    public virtual Animation<T> first { get; private set; } = default!;
    public virtual Animation<T> next { get; private set; } = default!;
    internal virtual AnimationStatus? _lastStatus { get; set; } = default;
    internal virtual T? _lastValue { get; set; } = default;
    public virtual long _listenerCounter { get; set; } = 0L;
    public virtual HashedObserverList<Action> _listeners { get; set; } = new HashedObserverList<Action>();
    public virtual ObserverList<AnimationStatusListener> _statusListeners { get; set; } = new ObserverList<AnimationStatusListener>();

    protected CompoundAnimation(Animation<T> first, Animation<T> next)
    {
        this.first = first;
        this.next = next;
    }

    public virtual void didStartListening()
    {
        this.first.addListener((Action)this._maybeNotifyListeners);
        this.first.addStatusListener((AnimationStatusListener)this._maybeNotifyStatusListeners);
        this.next.addListener((Action)this._maybeNotifyListeners);
        this.next.addStatusListener((AnimationStatusListener)this._maybeNotifyStatusListeners);
    }

    public virtual void didStopListening()
    {
        this.first.removeListener((Action)this._maybeNotifyListeners);
        this.first.removeStatusListener((AnimationStatusListener)this._maybeNotifyStatusListeners);
        this.next.removeListener((Action)this._maybeNotifyListeners);
        this.next.removeStatusListener((AnimationStatusListener)this._maybeNotifyStatusListeners);
    }

    public override AnimationStatus status => (global::Doroti.Framework.Animation.AnimationStatusMembers.isAnimating(((Animation<T>)this.next).status) ? ((Animation<T>)this.next).status : ((Animation<T>)this.first).status);
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "CompoundAnimation"))}({this.first}, {this.next})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _maybeNotifyStatusListeners(AnimationStatus __unused0)
    {
        if ((!object.Equals(this.status, this._lastStatus)))
        {
            _lastStatus = this.status;
            notifyStatusListeners(this.status);
        }
    }

    internal virtual void _maybeNotifyListeners()
    {
        if (!EqualityComparer<T>.Default.Equals(value, this._lastValue))
        {
            _lastValue = value;
            notifyListeners();
        }
    }

    public virtual void didRegisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 0L));
        if ((this._listenerCounter == 0L))
        {
            didStartListening();
        }
        this._listenerCounter += 1L;
    }

    public virtual void didUnregisterListener()
    {
        DartRuntimePrimitives.Assert(() => (this._listenerCounter >= 1L));
        this._listenerCounter -= 1L;
        if ((this._listenerCounter == 0L))
        {
            didStopListening();
        }
    }

    public virtual bool isListening => (this._listenerCounter > 0L);
    public override void addListener(Action listener)
    {
        didRegisterListener();
        this._listeners.add(listener);
    }

    public override void removeListener(Action listener)
    {
        bool removed__4206 = this._listeners.remove(listener);
        if (removed__4206)
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
        List<Action> localListeners__4969 = this._listeners.ToList();
        foreach (var listener__5037 in localListeners__4969)
        {
            InformationCollector? collector__5095 = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    collector__5095 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalListenersMixin>($"The {this.GetType()} notifying listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    return true;
                });
            try
            {
                if (this._listeners.contains(listener__5037))
                {
                    listener__5037();
                }
            }
            catch (Exception exception__5520)
            {
                var stack__5531 = new System.Diagnostics.StackTrace();
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__5520, stack: stack__5531, library: "animation library", context: new ErrorDescription($"while notifying listeners for {this.GetType()}"), informationCollector: collector__5095));
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
        bool removed__7458 = this._statusListeners.remove(listener);
        if (removed__7458)
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
        List<AnimationStatusListener> localListeners__8291 = this._statusListeners.ToList();
        foreach (var listener__8365 in localListeners__8291)
        {
            try
            {
                if (this._statusListeners.contains(listener__8365))
                {
                    listener__8365(status);
                }
            }
            catch (Exception exception__8511)
            {
                var stack__8522 = new System.Diagnostics.StackTrace();
                InformationCollector? collector__8561 = default!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        collector__8561 = (() => new List<DiagnosticsNode> { new DiagnosticsProperty<AnimationLocalStatusListenersMixin>($"The {this.GetType()} notifying status listeners was", this, style: DiagnosticsTreeStyle.errorProperty) });
                        return true;
                    });
                FlutterError.reportError(new FlutterErrorDetails(exception: exception__8511, stack: stack__8522, library: "animation library", context: new ErrorDescription($"while notifying status listeners for {this.GetType()}"), informationCollector: collector__8561));
            }
        }
    }

}

public class AnimationMean : CompoundAnimation<double>
{
    public AnimationMean(Animation<double> left, Animation<double> right) : base(first: left, next: right)
    {
    }

    public override double value => (((((Animation<double>)first).value + ((Animation<double>)next).value)) / 2.0);
}

public class AnimationMax<T> : CompoundAnimation<T> where T : struct
{
    public AnimationMax(Animation<T> first, Animation<T> next) : base(first: first, next: next)
    {
    }

    public override T value => DartRuntimePrimitives.Max(((Animation<T>)first).value, ((Animation<T>)next).value);
}

public class AnimationMin<T> : CompoundAnimation<T> where T : struct
{
    public AnimationMin(Animation<T> first, Animation<T> next) : base(first: first, next: next)
    {
    }

    public override T value => DartRuntimePrimitives.Min(((Animation<T>)first).value, ((Animation<T>)next).value);
}
