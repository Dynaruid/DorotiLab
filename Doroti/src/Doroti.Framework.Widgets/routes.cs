// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/routes.dart
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

namespace Doroti.Generated.Framework.Widgets;

public abstract class OverlayRoute<T> : Route<T>
{
    internal virtual List<OverlayEntry> _overlayEntries { get; private set; } = new List<OverlayEntry>();

    protected OverlayRoute(RouteSettings? settings = null, bool? requestFocus = null) : base(settings: settings, requestFocus: requestFocus)
    {
    }

    public abstract IEnumerable<OverlayEntry> createOverlayEntries();
    public override List<OverlayEntry> overlayEntries => this._overlayEntries;
    public override void install()
    {
        DartRuntimePrimitives.Assert(() => !System.Linq.Enumerable.Any(this._overlayEntries));
        this._overlayEntries.AddRange(createOverlayEntries());
        base.install();
    }

    public virtual bool finishedWhenPopped => true;
    public override bool didPop(T? result)
    {
        bool returnValue__2797 = base.didPop(result);
        DartRuntimePrimitives.Assert(() => returnValue__2797);
        if (this.finishedWhenPopped)
        {
            this.navigator!.finalizeRoute(this);
        }
        return returnValue__2797;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        foreach (OverlayEntry entry__3020 in this._overlayEntries)
        {
            entry__3020.dispose();
        }
        this._overlayEntries.Clear();
        base.dispose();
    }

}

public abstract class TransitionRoute<T> : OverlayRoute<T>, PredictiveBackRoute
{
    internal virtual Completer<T?> _transitionCompleter { get; private set; } = new Completer<T?>();
    internal virtual global::Doroti.Generated.Framework.Scheduler.PerformanceModeRequestHandle? _performanceModeRequestHandle { get; set; } = default;
    internal virtual bool _popFinalized { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double>? _animation { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController? _controller { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation _secondaryAnimation { get; private set; } = new global::Doroti.Generated.Framework.Animation.ProxyAnimation(global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
    public virtual bool willDisposeAnimationController { get; set; } = true;
    internal virtual global::Doroti.Generated.Framework.Physics.Simulation? _simulation { get; set; } = default;
    internal virtual T? _result { get; set; } = default;
    internal virtual global::System.Action? _trainHoppingListenerRemover { get; set; } = default;

    protected TransitionRoute(RouteSettings? settings = null, bool? requestFocus = null) : base(settings: settings, requestFocus: requestFocus)
    {
    }

    public override bool isCurrent => base.isCurrent;
    public virtual bool popGestureEnabled => throw new NotSupportedException();
    public virtual Future<T?> completed => this._transitionCompleter.future;
    public abstract Duration transitionDuration { get; }
    public virtual Duration reverseTransitionDuration => this.transitionDuration;
    public abstract bool opaque { get; }
    public virtual bool allowSnapshotting => true;
    public override bool finishedWhenPopped => DartRuntimePrimitives.ConvertValue<bool>((this._controller!.isDismissed && !this._popFinalized));
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? animation => this._animation;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController? controller => this._controller;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double>? secondaryAnimation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._secondaryAnimation);
    public virtual bool debugTransitionCompleted()
    {
        var disposed__7941 = false;
        DartRuntimePrimitives.Assert(() =>
            {
                disposed__7941 = this._transitionCompleter.isCompleted;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(disposed__7941);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Animation.AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        Duration duration__8562 = this.transitionDuration;
        Duration reverseDuration__8612 = this.reverseTransitionDuration;
        return new global::Doroti.Generated.Framework.Animation.AnimationController(duration: duration__8562, reverseDuration: reverseDuration__8612, debugLabel: this.debugLabel, vsync: this.navigator!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> createAnimation()
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        DartRuntimePrimitives.Assert(() => (this._controller is not null));
        return this._controller!.view;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => (this.transitionDuration >= Duration.zero), () => (object?)$"The `duration` must be positive for a non-simulation animation. Received {this.transitionDuration}.");
        return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Physics.Simulation? _createSimulationAndVerify(bool forward)
    {
        global::Doroti.Generated.Framework.Physics.Simulation? simulation__10578 = ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)createSimulation(forward: forward));
        DartRuntimePrimitives.Assert(() => (this.transitionDuration >= Duration.zero), () => (object?)"The `duration` must be positive for an animation that doesn't use simulation. " + "Either set `transitionDuration` or set `createSimulation`. " + $"Received {this.transitionDuration}.");
        return simulation__10578;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
                {
                    if (System.Linq.Enumerable.Any(this.overlayEntries))
                    {
                        this.overlayEntries.First().opaque = this.opaque;
                    }
                    this._performanceModeRequestHandle?.dispose();
                    _performanceModeRequestHandle = null;
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
                {
                    if (System.Linq.Enumerable.Any(this.overlayEntries))
                    {
                        this.overlayEntries.First().opaque = false;
                    }
                    _performanceModeRequestHandle ??= global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.requestPerformanceMode(DartPerformanceMode.latency);
                    break;
                }
            case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
                {
                    if (!this.isActive)
                    {
                        this.navigator!.finalizeRoute(this);
                        _popFinalized = true;
                        this._performanceModeRequestHandle?.dispose();
                        _performanceModeRequestHandle = null;
                    }
                    break;
                }
        }
    }

    public override void install()
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot install a {this.GetType()} after disposing it.");
        _controller = createAnimationController();
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.createAnimationController() returned null.");
        _animation = ((Func<global::Doroti.Generated.Framework.Animation.Animation<double>>)(() =>
{            var __cascade = createAnimation();
            __cascade.addStatusListener((AnimationStatusListener)this._handleStatusChanged);
            return __cascade;        }))();
        DartRuntimePrimitives.Assert(() => (this._animation is not null), () => (object?)$"{this.GetType()}.createAnimation() returned null.");
        base.install();
        if ((this._animation!.isCompleted && System.Linq.Enumerable.Any(this.overlayEntries)))
        {
            this.overlayEntries.First().opaque = this.opaque;
        }
    }

    public override global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didPush called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        base.didPush();
        _simulation = _createSimulationAndVerify(forward: true);
        if ((this._simulation is null))
        {
            return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)this._controller!.forward());
        }
        else
        {
            return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)this._controller!.animateWith(this._simulation!));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didAdd()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didPush called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        base.didAdd();
        this._controller!.value = this._controller!.upperBound;
    }

    public override void didReplace(dynamic oldRoute)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didReplace called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        if ((oldRoute is TransitionRoute<T>))
        {
            TransitionRoute<T> oldRoute__as13771 = (TransitionRoute<T>)oldRoute;
            this._controller!.value = ((global::Doroti.Generated.Framework.Animation.AnimationController?)((dynamic)oldRoute__as13771)._controller)!.value;
        }
        base.didReplace((object?)oldRoute);
    }

    public override bool didPop(T? result)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didPop called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !this._transitionCompleter.isCompleted, () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        _result = result;
        _simulation = _createSimulationAndVerify(forward: false);
        if ((this._simulation is null))
        {
            this._controller!.reverse();
        }
        else
        {
            this._controller!.animateBackWith(this._simulation!);
        }
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didPopNext(dynamic nextRoute)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didPopNext called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        _updateSecondaryAnimation(nextRoute);
        base.didPopNext((object?)nextRoute);
    }

    public override void didChangeNext(dynamic nextRoute)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didChangeNext called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        _updateSecondaryAnimation(nextRoute);
        base.didChangeNext((object?)nextRoute);
    }

    internal virtual void _updateSecondaryAnimation(dynamic nextRoute)
    {
        global::System.Action? previousTrainHoppingListenerRemover__15733 = this._trainHoppingListenerRemover;
        _trainHoppingListenerRemover = null;
        if ((((nextRoute is TransitionRoute<dynamic>) && canTransitionTo(nextRoute)) && ((bool)((dynamic)nextRoute).canTransitionFrom(this))))
        {
            dynamic nextRoute__as15851 = (dynamic)nextRoute;
            global::Doroti.Generated.Framework.Animation.Animation<double>? current__16006 = ((global::Doroti.Generated.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent;
            if ((current__16006 is not null))
            {
                global::Doroti.Generated.Framework.Animation.Animation<double> currentTrain__16105 = (((current__16006 is global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation) ? ((global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation)((global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation)current__16006)).currentTrain : current__16006))!;
                global::Doroti.Generated.Framework.Animation.Animation<double> nextTrain__16246 = ((global::Doroti.Generated.Framework.Animation.Animation<double>?)((dynamic)nextRoute__as15851)._animation)!;
                if (((((global::Doroti.Generated.Framework.Animation.Animation<double>)currentTrain__16105).value == ((global::Doroti.Generated.Framework.Animation.Animation<double>)nextTrain__16246).value) || !((global::Doroti.Generated.Framework.Animation.Animation<double>)nextTrain__16246).isAnimating))
                {
                    _setSecondaryAnimation(nextTrain__16246, ((Future<object>)((dynamic)nextRoute__as15851).completed));
                }
                else
                {
                    global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation? newAnimation__17180 = default!;
                    void jumpOnAnimationEnd(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
                    {
                        if (!global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isAnimating(status))
                        {
                            _setSecondaryAnimation(nextTrain__16246, ((Future<object>)((dynamic)nextRoute__as15851).completed));
                            if ((this._trainHoppingListenerRemover is not null))
                            {
                                this._trainHoppingListenerRemover!();
                                _trainHoppingListenerRemover = null;
                            }
                        }
                    }
                    _trainHoppingListenerRemover = (global::System.Action)(() => {
nextTrain__16246.removeStatusListener((AnimationStatusListener)jumpOnAnimationEnd);
newAnimation__17180?.dispose();
});
                    nextTrain__16246.addStatusListener((AnimationStatusListener)jumpOnAnimationEnd);
                    newAnimation__17180 = new global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation(currentTrain__16105, nextTrain__16246, onSwitchedTrain: ((global::System.Action)(() => {
DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent, newAnimation__17180)));
DartRuntimePrimitives.Assert(() => (object.Equals(newAnimation__17180!.currentTrain, ((global::Doroti.Generated.Framework.Animation.Animation<double>?)((dynamic)nextRoute__as15851)._animation))));
_setSecondaryAnimation(newAnimation__17180!.currentTrain, ((Future<object>)((dynamic)nextRoute__as15851).completed));
if ((this._trainHoppingListenerRemover is not null))
{
    this._trainHoppingListenerRemover!();
    _trainHoppingListenerRemover = null;
}
})));
                    _setSecondaryAnimation(newAnimation__17180, ((Future<object>)((dynamic)nextRoute__as15851).completed));
                }
            }
            else
            {
                _setSecondaryAnimation(((global::Doroti.Generated.Framework.Animation.Animation<double>?)((dynamic)nextRoute__as15851)._animation), ((Future<object>)((dynamic)nextRoute__as15851).completed));
            }
        }
        else
        {
            _setSecondaryAnimation(global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
        }
        previousTrainHoppingListenerRemover__15733?.Invoke();
    }

    internal virtual void _setSecondaryAnimation(global::Doroti.Generated.Framework.Animation.Animation<double>? animation, Future<object>? disposed = null)
    {
        this._secondaryAnimation.parent = animation;
        if (disposed is not null)
        {
            DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) => {
if ((object.Equals(((global::Doroti.Generated.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent, animation)))
{
    this._secondaryAnimation.parent = global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation;
    if ((animation is global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation))
    {
        global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation animation__as19490 = (global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation)animation;
        ((global::Doroti.Generated.Framework.Animation.TrainHoppingAnimation)animation__as19490).dispose();
    }
}
})));
        }
    }

    public virtual bool canTransitionTo(dynamic nextRoute) => true;
    public virtual bool canTransitionFrom(dynamic previousRoute) => true;
    public virtual void handleStartBackGesture(double progress = 0.0)
    {
        DartRuntimePrimitives.Assert(() => this.isCurrent);
        this._controller?.value = progress;
        this.navigator?.didStartUserGesture();
    }

    public virtual void handleUpdateBackGestureProgress(double progress)
    {
        if (!this.isCurrent)
        {
            return;
        }
        this._controller?.value = progress;
    }

    public virtual void handleCancelBackGesture()
    {
        _handleDragEnd(animateForward: true);
    }

    public virtual void handleCommitBackGesture()
    {
        _handleDragEnd(animateForward: false);
    }

    internal virtual void _handleDragEnd(bool animateForward)
    {
        if (this.isCurrent)
        {
            if (animateForward)
            {
                if (!this._controller!.isCompleted)
                {
                    this._controller!.forward();
                }
            }
            else
            {
                this.navigator?.pop<object>();
                if ((this._controller?.isAnimating ?? false))
                {
                    this._controller!.reverse(from: this._controller!.upperBound);
                }
            }
        }
        if ((this._controller?.isAnimating ?? false))
        {
            AnimationStatusListener animationStatusCallback__23538 = default!;
            animationStatusCallback__23538 = ((status) => {
this.navigator?.didStopUserGesture();
this._controller!.removeStatusListener((AnimationStatusListener)animationStatusCallback__23538);
});
            this._controller!.addStatusListener((AnimationStatusListener)animationStatusCallback__23538);
        }
        else
        {
            this.navigator?.didStopUserGesture();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._transitionCompleter.isCompleted, () => (object?)$"Cannot dispose a {this.GetType()} twice.");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot dispose a {this.GetType()} twice.");
        this._animation?.removeStatusListener((AnimationStatusListener)this._handleStatusChanged);
        this._performanceModeRequestHandle?.dispose();
        _performanceModeRequestHandle = null;
        if (this.willDisposeAnimationController)
        {
            this._controller?.dispose();
        }
        this._transitionCompleter.complete(this._result);
        base.dispose();
    }

    public virtual string debugLabel => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute");
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute"))}(animation: {this._controller})";
}

public interface PredictiveBackRoute
{
    public bool isCurrent { get; }
    public bool popGestureEnabled { get; }
    public void handleStartBackGesture(double progress = 0.0);
    public void handleUpdateBackGestureProgress(double progress);
    public void handleCommitBackGesture();
    public void handleCancelBackGesture();
}

public class LocalHistoryEntry
{
    public virtual global::System.Action? onRemove { get; private set; }
    internal virtual LocalHistoryRoute<object>? _owner { get; set; } = default;
    public virtual bool impliesAppBarDismissal { get; private set; } = default!;

    public LocalHistoryEntry(global::System.Action? onRemove = null, bool impliesAppBarDismissal = true)
    {
        this.onRemove = onRemove;
        this.impliesAppBarDismissal = impliesAppBarDismissal;
    }

    public virtual void remove()
    {
        this._owner?.removeLocalHistoryEntry(this);
        DartRuntimePrimitives.Assert(() => (this._owner is null));
    }

    internal virtual void _notifyRemoved()
    {
        this.onRemove?.Invoke();
    }

}

public interface LocalHistoryRoute<T>
{
    List<LocalHistoryEntry>? _localHistory { get; set; }
    long _entriesImpliesAppBarDismissal { get; set; }

    public void addLocalHistoryEntry(LocalHistoryEntry entry);
    public void removeLocalHistoryEntry(LocalHistoryEntry entry);
    public Future<RoutePopDisposition> willPop();
    public RoutePopDisposition popDisposition { get; }
    public bool didPop(T? result);
    public bool willHandlePopInternally { get; }
}

internal class _DismissModalAction__routes : DismissAction
{
    public virtual BuildContext context { get; private set; } = default!;

    internal _DismissModalAction__routes(BuildContext context)
    {
        this.context = context;
    }

    public override bool isEnabled(DismissIntent intent, BuildContext? context = null)
    {
        dynamic route__36290 = ModalRoute<object>.of<object>(this.context)!;
        return ((bool)((dynamic)route__36290).barrierDismissible);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? invoke(DismissIntent intent, BuildContext? context = null)
    {
        return Navigator.of(this.context).maybePop<object>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _ModalRouteAspect__routes
{
    isCurrent,
    canPop,
    settings,
    isActive,
    isFirst,
    opaque,
    popDisposition
}

internal class _ModalScopeStatus__routes : InheritedModel<_ModalRouteAspect__routes>
{
    public virtual bool isCurrent { get; private set; } = default!;
    public virtual bool canPop { get; private set; } = default!;
    public virtual bool impliesAppBarDismissal { get; private set; } = default!;
    public virtual bool opaque { get; private set; } = default!;
    public virtual dynamic route { get; private set; } = default!;

    internal _ModalScopeStatus__routes(bool isCurrent, bool canPop, bool impliesAppBarDismissal, dynamic route, bool opaque, Widget child) : base(child: child)
    {
        this.isCurrent = isCurrent;
        this.canPop = canPop;
        this.impliesAppBarDismissal = impliesAppBarDismissal;
        this.route = route;
        this.opaque = opaque;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_ModalScopeStatus__routes)(object)oldWidget;
        return (((((this.isCurrent != ((_ModalScopeStatus__routes)__old).isCurrent) || (this.canPop != ((_ModalScopeStatus__routes)__old).canPop)) || (this.impliesAppBarDismissal != ((_ModalScopeStatus__routes)__old).impliesAppBarDismissal)) || (!object.Equals(this.route, ((_ModalScopeStatus__routes)__old).route))) || (this.opaque != ((_ModalScopeStatus__routes)__old).opaque));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("isCurrent", value: this.isCurrent, ifTrue: "active", ifFalse: "inactive"));
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("canPop", value: this.canPop, ifTrue: "can pop"));
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("impliesAppBarDismissal", value: this.impliesAppBarDismissal, ifTrue: "implies app bar dismissal"));
    }

    public override bool updateShouldNotifyDependent(InheritedModel<_ModalRouteAspect__routes> oldWidget, HashSet<_ModalRouteAspect__routes> dependencies)
    {
        var __oldWidget = (_ModalScopeStatus__routes)(object)oldWidget;
        return dependencies.any(((dependency) => (dependency switch { _ModalRouteAspect__routes.isCurrent => (this.isCurrent != ((_ModalScopeStatus__routes)__oldWidget).isCurrent), _ModalRouteAspect__routes.canPop => (this.canPop != ((_ModalScopeStatus__routes)__oldWidget).canPop), _ModalRouteAspect__routes.settings => (!object.Equals(((RouteSettings)((dynamic)this.route).settings), ((RouteSettings)((dynamic)((_ModalScopeStatus__routes)__oldWidget).route).settings))), _ModalRouteAspect__routes.isActive => (((bool)((dynamic)this.route).isActive) != ((bool)((dynamic)((_ModalScopeStatus__routes)__oldWidget).route).isActive)), _ModalRouteAspect__routes.isFirst => (((bool)((dynamic)this.route).isFirst) != ((bool)((dynamic)((_ModalScopeStatus__routes)__oldWidget).route).isFirst)), _ModalRouteAspect__routes.opaque => (this.opaque != ((_ModalScopeStatus__routes)__oldWidget).opaque), _ModalRouteAspect__routes.popDisposition => (!object.Equals(((RoutePopDisposition)((dynamic)this.route).popDisposition), ((RoutePopDisposition)((dynamic)((_ModalScopeStatus__routes)__oldWidget).route).popDisposition))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ModalScope__routes<T> : StatefulWidget
{
    public virtual ModalRoute<T> route { get; private set; } = default!;

    internal _ModalScope__routes(global::Doroti.Generated.Framework.Foundation.Key? key = null, ModalRoute<T> route = default!) : base(key: key)
    {
        this.route = route;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ModalScopeState__routes<T>());
}

public class _ModalScopeState__routes<T> : State<_ModalScope__routes<T>>
{
    internal virtual Widget? _page { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Foundation.Listenable _listenable { get; set; } = default!;
    public virtual FocusScopeNode focusScopeNode { get; private set; } = new FocusScopeNode(debugLabel: $"{typeof(_ModalScopeState__routes<T>)} Focus Scope");
    public virtual ScrollController primaryScrollController { get; private set; } = new ScrollController();

    public override void initState()
    {
        base.initState();
        var animations__40042 = new List<global::Doroti.Generated.Framework.Foundation.Listenable>();
        _listenable = global::Doroti.Generated.Framework.Foundation.Listenable.CreateMerge(animations__40042.Cast<global::Doroti.Generated.Framework.Foundation.Listenable?>());
    }

    public override void didUpdateWidget(_ModalScope__routes<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => (object.Equals(((_ModalScope__routes<T>)(object)this.widget).route, ((_ModalScope__routes<T>)oldWidget).route)));
        _updateFocusScopeNode();
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _page = null;
        _updateFocusScopeNode();
    }

    internal virtual void _updateFocusScopeNode()
    {
        TraversalEdgeBehavior traversalEdgeBehavior__40558 = default!;
        TraversalEdgeBehavior directionalTraversalEdgeBehavior__40613 = default!;
        ModalRoute<T> route__40671 = ((_ModalScope__routes<T>)(object)this.widget).route;
        if ((((ModalRoute<T>)route__40671).traversalEdgeBehavior is not null))
        {
            traversalEdgeBehavior__40558 = DartRuntimePrimitives.RequireValue(((ModalRoute<T>)route__40671).traversalEdgeBehavior);
        }
        else
        {
            traversalEdgeBehavior__40558 = route__40671.navigator!.widget.routeTraversalEdgeBehavior;
        }
        if ((((ModalRoute<T>)route__40671).directionalTraversalEdgeBehavior is not null))
        {
            directionalTraversalEdgeBehavior__40613 = DartRuntimePrimitives.RequireValue(((ModalRoute<T>)route__40671).directionalTraversalEdgeBehavior);
        }
        else
        {
            directionalTraversalEdgeBehavior__40613 = route__40671.navigator!.widget.routeDirectionalTraversalEdgeBehavior;
        }
        this.focusScopeNode.traversalEdgeBehavior = traversalEdgeBehavior__40558;
        this.focusScopeNode.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehavior__40613;
        if ((route__40671.isCurrent && this._shouldRequestFocus))
        {
            route__40671.navigator!.focusNode.enclosingScope?.setFirstFocus(this.focusScopeNode);
        }
    }

    internal virtual void _forceRebuildPage()
    {
        setState(((global::System.Action)(() => {
_page = null;
})));
    }

    public override void dispose()
    {
        this.focusScopeNode.dispose();
        this.primaryScrollController.dispose();
        base.dispose();
    }

    internal virtual bool _shouldIgnoreFocusRequest
    {
        get
        {
            return ((object.Equals(((_ModalScope__routes<T>)(object)this.widget).route.animation?.status, global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse)) || ((((_ModalScope__routes<T>)(object)this.widget).route.navigator?.userGestureInProgress ?? false)));
            return default!;
        }
    }
    internal virtual bool _shouldRequestFocus
    {
        get
        {
            return ((_ModalScope__routes<T>)(object)this.widget).route.requestFocus;
            return default!;
        }
    }
    internal virtual void _routeSetState(global::System.Action fn)
    {
        if (((((_ModalScope__routes<T>)(object)this.widget).route.isCurrent && !this._shouldIgnoreFocusRequest) && this._shouldRequestFocus))
        {
            ((_ModalScope__routes<T>)(object)this.widget).route.navigator!.focusNode.enclosingScope?.setFirstFocus(this.focusScopeNode);
        }
        setState(() => fn());
    }

    public override Widget build(BuildContext context)
    {
        this.focusScopeNode.skipTraversal = !((_ModalScope__routes<T>)(object)this.widget).route.isCurrent;
        return ((Widget)(object?)new AnimatedBuilder(animation: ((_ModalScope__routes<T>)(object)this.widget).route.restorationScopeId, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
DartRuntimePrimitives.Assert(() => (child is not null));
return ((Widget)(object?)new RestorationScope(restorationId: ((_ModalScope__routes<T>)(object)this.widget).route.restorationScopeId.value, child: child!));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new _ModalScopeStatus__routes(route: ((_ModalScope__routes<T>)(object)this.widget).route, isCurrent: ((_ModalScope__routes<T>)(object)this.widget).route.isCurrent, canPop: ((_ModalScope__routes<T>)(object)this.widget).route.canPop, opaque: ((_ModalScope__routes<T>)(object)this.widget).route.opaque, impliesAppBarDismissal: ((_ModalScope__routes<T>)(object)this.widget).route.impliesAppBarDismissal, child: new Offstage(offstage: ((_ModalScope__routes<T>)(object)this.widget).route.offstage, child: new PageStorage(bucket: ((_ModalScope__routes<T>)(object)this.widget).route._storageBucket, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return ((Widget)(object?)new Actions(actions: new DartMap<Type, dynamic> { [typeof(DismissIntent)] = new _DismissModalAction__routes(context) }, child: new PrimaryScrollController(controller: this.primaryScrollController, child: FocusScope.CreateWithExternalFocusNode(focusScopeNode: this.focusScopeNode, child: new RepaintBoundary(child: new ListenableBuilder(listenable: this._listenable, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
return ((Widget)(object?)((_ModalScope__routes<T>)(object)this.widget).route._buildFlexibleTransitions(context, ((_ModalScope__routes<T>)(object)this.widget).route.animation!, ((_ModalScope__routes<T>)(object)this.widget).route.secondaryAnimation!, new ListenableBuilder(listenable: (((_ModalScope__routes<T>)(object)this.widget).route.navigator?.userGestureInProgressNotifier ?? new global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>(false)), builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
bool ignoreEvents__44972 = this._shouldIgnoreFocusRequest;
this.focusScopeNode.canRequestFocus = !ignoreEvents__44972;
return ((Widget)(object?)new IgnorePointer(ignoring: ignoreEvents__44972, child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: child)));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: _page ??= new RepaintBoundary(key: ((_ModalScope__routes<T>)(object)this.widget).route._subtreeKey, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
return ((Widget)(object?)((_ModalScope__routes<T>)(object)this.widget).route.buildPage(context, ((_ModalScope__routes<T>)(object)this.widget).route.animation!, ((_ModalScope__routes<T>)(object)this.widget).route.secondaryAnimation!));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))))))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class ModalRoute<T> : TransitionRoute<T>, LocalHistoryRoute<T>
{
    public virtual ImageFilter? filter { get; private set; }
    public virtual TraversalEdgeBehavior? traversalEdgeBehavior { get; private set; }
    public virtual TraversalEdgeBehavior? directionalTraversalEdgeBehavior { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>? receivedTransition { get; set; } = default;
    internal virtual bool _offstage { get; set; } = false;
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation? _animationProxy { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.ProxyAnimation? _secondaryAnimationProxy { get; set; } = default;
    internal virtual List<global::System.Func<Future<bool>>> _willPopCallbacks { get; private set; } = new List<global::System.Func<Future<bool>>>();
    internal virtual HashSet<dynamic> _popEntries { get; private set; } = new HashSet<dynamic>();
    internal virtual GlobalKey<_ModalScopeState__routes<T>> _scopeKey { get; private set; } = GlobalKey<_ModalScopeState__routes<T>>.Create();
    internal virtual GlobalKey<IState> _subtreeKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual PageStorageBucket _storageBucket { get; private set; } = new PageStorageBucket();
    internal virtual OverlayEntry _modalBarrier { get; set; } = default!;
    internal virtual Widget? _modalScopeCache { get; set; } = default;
    internal virtual OverlayEntry _modalScope { get; set; } = default!;
    public virtual List<LocalHistoryEntry>? _localHistory { get; set; } = default;
    public virtual long _entriesImpliesAppBarDismissal { get; set; } = 0L;

    protected ModalRoute(RouteSettings? settings = null, bool? requestFocus = null, ImageFilter? filter = null, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null) : base(settings: settings, requestFocus: requestFocus)
    {
        this.filter = filter;
        this.traversalEdgeBehavior = traversalEdgeBehavior;
        this.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehavior;
    }

    public static ModalRoute<T>? of<T>(BuildContext context)
    {
        return ((ModalRoute<T>?)(object?)ModalRoute<T>._of<T>(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static ModalRoute<T>? _of<T>(BuildContext context, _ModalRouteAspect__routes? aspect = null)
    {
        return ((ModalRoute<T>?)(object?)InheritedModel<object>.inheritFrom<_ModalScopeStatus__routes>(context, aspect: aspect)?.route)!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool? isCurrentOf(BuildContext context) => ((bool?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isCurrent))?.isCurrent);
    public static bool? canPopOf(BuildContext context) => ((bool?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.canPop))?.canPop);
    public static RouteSettings? settingsOf(BuildContext context) => ((RouteSettings?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.settings))?.settings);
    public static bool? isActiveOf(BuildContext context) => ((bool?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isActive))?.isActive);
    public static bool? isFirstOf(BuildContext context) => ((bool?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isFirst))?.isFirst);
    public static bool? opaqueOf(BuildContext context) => ((bool?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.opaque))?.opaque);
    public static RoutePopDisposition? popDispositionOf(BuildContext context) => ((RoutePopDisposition?)((dynamic)ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.popDisposition))?.popDisposition);
    public virtual void setState(global::System.Action fn)
    {
        if ((((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null))
        {
            ((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState!._routeSetState(() => fn());
        }
        else
        {
            fn();
        }
    }

    public static global::System.Func<dynamic, bool> withName(string name)
    {
        return ((global::System.Func<dynamic, bool>)((route) => {
return ((!((bool)((dynamic)route).willHandlePopInternally) && (route is ModalRoute<T>)) && (((RouteSettings)((dynamic)route).settings).ToString() == name));
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Widget buildPage(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation);
    public virtual Widget buildTransitions(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>? delegatedTransition => DartRuntimePrimitives.ConvertValue<global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>>(null);
    internal virtual Widget _buildFlexibleTransitions(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        if (((this.receivedTransition is null) || ((global::Doroti.Generated.Framework.Animation.Animation<double>)secondaryAnimation).isDismissed))
        {
            return ((Widget)(object?)buildTransitions(context, animation, secondaryAnimation, child));
        }
        var proxyAnimation__63641 = new global::Doroti.Generated.Framework.Animation.ProxyAnimation();
        Widget proxiedOriginalTransitions__63694 = ((Widget)(object?)buildTransitions(context, animation, proxyAnimation__63641, child));
        return (this.receivedTransition!(context, animation, secondaryAnimation, this.allowSnapshotting, proxiedOriginalTransitions__63694) ?? proxiedOriginalTransitions__63694);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void install()
    {
        base.install();
        _animationProxy = new global::Doroti.Generated.Framework.Animation.ProxyAnimation(base.animation);
        _secondaryAnimationProxy = new global::Doroti.Generated.Framework.Animation.ProxyAnimation(base.secondaryAnimation);
    }

    public override global::Doroti.Generated.Framework.Scheduler.TickerFuture didPush()
    {
        if (((((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null) && this.navigator!.widget.requestFocus))
        {
            this.navigator!.focusNode.enclosingScope?.setFirstFocus(((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState!.focusScopeNode);
        }
        return ((global::Doroti.Generated.Framework.Scheduler.TickerFuture)(object?)base.didPush());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didAdd()
    {
        if (((((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null) && this.navigator!.widget.requestFocus))
        {
            this.navigator!.focusNode.enclosingScope?.setFirstFocus(((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState!.focusScopeNode);
        }
        base.didAdd();
    }

    public abstract bool barrierDismissible { get; }
    public virtual bool semanticsDismissible => true;
    public abstract global::Doroti.Ui.Color? barrierColor { get; }
    public abstract string? barrierLabel { get; }
    public virtual global::Doroti.Generated.Framework.Animation.Curve barrierCurve => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Curve>(global::Doroti.Generated.Framework.Animation.Curves.ease);
    public abstract bool maintainState { get; }
    public virtual bool popGestureInProgress => this.navigator!.userGestureInProgress;
    public virtual bool popGestureEnabled
    {
        get
        {
            if (this.isFirst)
            {
                return false;
            }
            if (this.willHandlePopInternally)
            {
                return false;
            }
            if ((this.hasScopedWillPopCallback || (object.Equals(this.popDisposition, RoutePopDisposition.doNotPop))))
            {
                return false;
            }
            if (!this.animation!.isCompleted)
            {
                return false;
            }
            return true;
            return default!;
        }
    }
    public virtual bool offstage
    {
        get => this._offstage;
        set
        {
            var __value = value;
            if ((this._offstage == __value))
            {
                return;
            }
            setState(((global::System.Action)(() => {
_offstage = __value;
})));
            this._animationProxy!.parent = (this._offstage ? global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation : base.animation);
            this._secondaryAnimationProxy!.parent = (this._offstage ? global::Doroti.Generated.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation : base.secondaryAnimation);
            changedInternalState();
        }
    }
    public virtual BuildContext? subtreeContext => ((GlobalKey<IState>)this._subtreeKey).currentContext;
    public override global::Doroti.Generated.Framework.Animation.Animation<double>? animation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._animationProxy);
    public override global::Doroti.Generated.Framework.Animation.Animation<double>? secondaryAnimation => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<double>>(this._secondaryAnimationProxy);
    public async override Future<RoutePopDisposition> willPop()
    {
        _ModalScopeState__routes<T>? scope__77408 = ((_ModalScopeState__routes<T>?)(object?)((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState);
        DartRuntimePrimitives.Assert(() => (scope__77408 is not null));
        foreach (var callback__77482 in new List<global::System.Func<Future<bool>>>(DartRuntimePrimitives.ConvertEnumerable<global::System.Func<Future<bool>>>(this._willPopCallbacks)))
        {
            if (!await callback__77482())
            {
                return RoutePopDisposition.doNotPop;
            }
        }
        return await base.willPop();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override RoutePopDisposition popDisposition
    {
        get
        {
            foreach (dynamic popEntry__78552 in this._popEntries)
            {
                if (!((global::Doroti.Generated.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry__78552).canPopNotifier).value)
                {
                    return RoutePopDisposition.doNotPop;
                }
            }
            return base.popDisposition;
            return default!;
        }
    }
    public override void onPopInvokedWithResult(bool didPop, T? result)
    {
        foreach (dynamic popEntry__78822 in this._popEntries)
        {
            ((dynamic)popEntry__78822).onPopInvokedWithResult(didPop, result);
        }
        base.onPopInvokedWithResult(didPop, result);
    }

    public virtual void addScopedWillPopCallback(global::System.Func<Future<bool>> callback)
    {
        DartRuntimePrimitives.Assert(() => (((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null), () => (object?)"Tried to add a willPop callback to a route that is not currently in the tree.");
        this._willPopCallbacks.Add(callback);
        if ((checked((long)(this._willPopCallbacks.Count)) == 1L))
        {
            _maybeDispatchNavigationNotification();
        }
    }

    public virtual void removeScopedWillPopCallback(global::System.Func<Future<bool>> callback)
    {
        DartRuntimePrimitives.Assert(() => (((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null), () => (object?)"Tried to remove a willPop callback from a route that is not currently in the tree.");
        this._willPopCallbacks.Remove(callback);
        if (!System.Linq.Enumerable.Any(this._willPopCallbacks))
        {
            _maybeDispatchNavigationNotification();
        }
    }

    public virtual void registerPopEntry(dynamic popEntry)
    {
        this._popEntries.Add(popEntry);
        ((global::Doroti.Generated.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry).canPopNotifier).addListener(() => this._maybeDispatchNavigationNotification());
        _maybeDispatchNavigationNotification();
    }

    public virtual void unregisterPopEntry(dynamic popEntry)
    {
        this._popEntries.Remove(popEntry);
        ((global::Doroti.Generated.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry).canPopNotifier).removeListener(() => this._maybeDispatchNavigationNotification());
        _maybeDispatchNavigationNotification();
    }

    internal virtual void _maybeDispatchNavigationNotification()
    {
        if (!this.isCurrent)
        {
            return;
        }
        var notification__82227 = new NavigationNotification(canHandlePop: ((object.Equals(this.popDisposition, RoutePopDisposition.doNotPop)) || System.Linq.Enumerable.Any(this._willPopCallbacks)));
        switch (global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)
        {
            case global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.postFrameCallbacks:
                {
                    notification__82227.dispatch(this.subtreeContext);
                    break;
                }
            case global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.idle:
            case global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.midFrameMicrotasks:
            case global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks:
            case global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.transientCallbacks:
                {
                    global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
if (!((this.subtreeContext?.mounted ?? false)))
{
    return;
}
notification__82227.dispatch(this.subtreeContext);
})), debugLabel: "ModalRoute.dispatchNotification");
                    break;
                }
        }
    }

    public virtual bool hasScopedWillPopCallback
    {
        get
        {
            return System.Linq.Enumerable.Any(this._willPopCallbacks);
            return default!;
        }
    }
    public override void didChangePrevious(dynamic previousRoute)
    {
        base.didChangePrevious((object?)previousRoute);
        changedInternalState();
    }

    public override void didChangeNext(dynamic nextRoute)
    {
        if ((((nextRoute is ModalRoute<T>) && canTransitionTo(nextRoute)) && (!object.Equals((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)((ModalRoute<T>)nextRoute).delegatedTransition, (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)this.delegatedTransition))))
        {
            ModalRoute<T> nextRoute__as84419 = (ModalRoute<T>)nextRoute;
            receivedTransition = (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>)((ModalRoute<T>)nextRoute__as84419).delegatedTransition;
        }
        else
        {
            receivedTransition = null;
        }
        base.didChangeNext((object?)nextRoute);
        changedInternalState();
    }

    public override void didPopNext(dynamic nextRoute)
    {
        if ((((nextRoute is ModalRoute<T>) && canTransitionTo(nextRoute)) && (!object.Equals((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)((ModalRoute<T>)nextRoute).delegatedTransition, (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)this.delegatedTransition))))
        {
            ModalRoute<T> nextRoute__as84796 = (ModalRoute<T>)nextRoute;
            receivedTransition = (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, bool, Widget?, Widget?>)((ModalRoute<T>)nextRoute__as84796).delegatedTransition;
        }
        else
        {
            receivedTransition = null;
        }
        base.didPopNext((object?)nextRoute);
        changedInternalState();
        _maybeDispatchNavigationNotification();
    }

    public override void changedInternalState()
    {
        base.changedInternalState();
        if ((!object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() => {
})));
            this._modalBarrier.markNeedsBuild();
        }
        this._modalScope.maintainState = this.maintainState;
    }

    public override void changedExternalState()
    {
        base.changedExternalState();
        this._modalBarrier.markNeedsBuild();
        if ((((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null))
        {
            ((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState!._forceRebuildPage();
        }
    }

    public virtual bool canPop => DartRuntimePrimitives.ConvertValue<bool>((this.hasActiveRouteBelow || this.willHandlePopInternally));
    public virtual bool impliesAppBarDismissal => DartRuntimePrimitives.ConvertValue<bool>((this.hasActiveRouteBelow || (this._entriesImpliesAppBarDismissal > 0L)));
    public virtual bool fullscreenDialog => false;
    internal virtual Widget _buildModalBarrier(BuildContext context)
    {
        Widget barrier__87151 = ((Widget)(object?)buildModalBarrier());
        if ((this.filter is not null))
        {
            barrier__87151 = DartRuntimePrimitives.ConvertValue<Widget>(new BackdropFilter(filter: this.filter, child: barrier__87151));
        }
        barrier__87151 = DartRuntimePrimitives.ConvertValue<Widget>(new IgnorePointer(ignoring: !this.animation!.isForwardOrCompleted, child: barrier__87151));
        if ((this.semanticsDismissible && this.barrierDismissible))
        {
            barrier__87151 = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(sortKey: new global::Doroti.Generated.Framework.Semantics.OrdinalSortKey(1.0), child: barrier__87151));
        }
        return barrier__87151;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildModalBarrier()
    {
        Widget barrier__88150 = default!;
        if ((((this.barrierColor is not null) && (this.barrierColor!.alpha != 0L)) && !this.offstage))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(this.barrierColor, this.barrierColor!.withOpacity(0.0))));
            global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Ui.Color?> color__88400 = ((global::Doroti.Generated.Framework.Animation.Animation<global::Doroti.Ui.Color?>)(object?)this.animation!.drive(new global::Doroti.Generated.Framework.Animation.ColorTween(begin: this.barrierColor!.withOpacity(0.0), end: this.barrierColor).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: this.barrierCurve))));
            barrier__88150 = DartRuntimePrimitives.ConvertValue<Widget>(new AnimatedModalBarrier(color: color__88400, dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible));
        }
        else
        {
            barrier__88150 = DartRuntimePrimitives.ConvertValue<Widget>(new ModalBarrier(dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible));
        }
        return barrier__88150;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildModalScope(BuildContext context)
    {
        return _modalScopeCache ??= new Semantics(sortKey: new global::Doroti.Generated.Framework.Semantics.OrdinalSortKey(0.0), child: new _ModalScope__routes<T>(key: this._scopeKey, route: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IEnumerable<OverlayEntry> createOverlayEntries()
    {
        return ((IEnumerable<OverlayEntry>)(object?)new List<OverlayEntry> { (_modalBarrier = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildModalBarrier)), (_modalScope = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildModalScope, maintainState: this.maintainState, canSizeOverlay: this.opaque)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ModalRoute"))}({this.settings}, animation: {this._animation})";
    public virtual void addLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => (((LocalHistoryEntry)entry)._owner is null));
        entry._owner = DartRuntimePrimitives.ConvertValue<LocalHistoryRoute<object>>(this);
        this._localHistory ??= new List<LocalHistoryEntry>();
        bool wasEmpty__33256 = !System.Linq.Enumerable.Any(this._localHistory!);
        this._localHistory!.Add(entry);
        var internalStateChanged__33330 = false;
        if (((LocalHistoryEntry)entry).impliesAppBarDismissal)
        {
            internalStateChanged__33330 = (this._entriesImpliesAppBarDismissal == 0L);
            this._entriesImpliesAppBarDismissal += 1L;
        }
        if ((wasEmpty__33256 || internalStateChanged__33330))
        {
            changedInternalState();
        }
    }

    public virtual void removeLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((LocalHistoryEntry)entry)._owner, this)));
        DartRuntimePrimitives.Assert(() => this._localHistory!.Contains(entry));
        var internalStateChanged__33903 = false;
        if ((this._localHistory!.Remove(entry) && ((LocalHistoryEntry)entry).impliesAppBarDismissal))
        {
            this._entriesImpliesAppBarDismissal -= 1L;
            internalStateChanged__33903 = (this._entriesImpliesAppBarDismissal == 0L);
        }
        entry._owner = null;
        entry._notifyRemoved();
        if ((!System.Linq.Enumerable.Any(this._localHistory!) || internalStateChanged__33903))
        {
            DartRuntimePrimitives.Assert(() => (this._entriesImpliesAppBarDismissal == 0L));
            if ((object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
            {
                global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) => {
if (this.isActive)
{
    changedInternalState();
}
})), debugLabel: "LocalHistoryRoute.changedInternalState");
            }
            else
            {
                changedInternalState();
            }
        }
    }

    public override bool didPop(T? result)
    {
        if (((this._localHistory is not null) && System.Linq.Enumerable.Any(this._localHistory!)))
        {
            LocalHistoryEntry entry__35472 = this._localHistory!.removeLast<LocalHistoryEntry>();
            DartRuntimePrimitives.Assert(() => (object.Equals(((LocalHistoryEntry)entry__35472)._owner, this)));
            entry__35472._owner = null;
            entry__35472._notifyRemoved();
            var internalStateChanged__35612 = false;
            if (((LocalHistoryEntry)entry__35472).impliesAppBarDismissal)
            {
                this._entriesImpliesAppBarDismissal -= 1L;
                internalStateChanged__35612 = (this._entriesImpliesAppBarDismissal == 0L);
            }
            if ((!System.Linq.Enumerable.Any(this._localHistory!) || internalStateChanged__35612))
            {
                changedInternalState();
            }
            return false;
        }
        return base.didPop(result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool willHandlePopInternally
    {
        get
        {
            return ((this._localHistory is not null) && System.Linq.Enumerable.Any(this._localHistory!));
            return default!;
        }
    }
}

public abstract class PopupRoute<T> : ModalRoute<T>
{
    protected PopupRoute(RouteSettings? settings = null, bool? requestFocus = null, ImageFilter? filter = null, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null) : base(settings: settings, requestFocus: requestFocus, filter: filter, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
    }

    public override bool opaque => false;
    public override bool maintainState => true;
    public override bool allowSnapshotting => false;
}

public class RouteObserver<R> : NavigatorObserver where R : notnull
{
    internal virtual DartMap<R, HashSet<RouteAware>> _listeners { get; private set; } = new DartMap<R, HashSet<RouteAware>>();

    public virtual bool debugObservingRoute(R route)
    {
        bool contained__93244 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                contained__93244 = this._listeners.ContainsKey(route);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return contained__93244;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void subscribe(RouteAware routeAware, R route)
    {
        HashSet<RouteAware> subscribers__93728 = this._listeners.putIfAbsent(route, (() => new HashSet<RouteAware>()));
        if (subscribers__93728.Add(routeAware))
        {
            routeAware.didPush();
        }
    }

    public virtual void unsubscribe(RouteAware routeAware)
    {
        List<R> routes__94151 = this._listeners.Keys.ToList().ToList();
        foreach (var route__94201 in routes__94151)
        {
            HashSet<RouteAware>? subscribers__94249 = this._listeners.GetValueOrDefault(route__94201);
            if ((subscribers__94249 is not null))
            {
                subscribers__94249.Remove(routeAware);
                if (!System.Linq.Enumerable.Any(subscribers__94249))
                {
                    this._listeners.remove(route__94201);
                }
            }
        }
    }

    public override void didPop(dynamic route, dynamic previousRoute)
    {
        if (((route is R) && (previousRoute is R)))
        {
            R route__as94544 = (R)(object)route;
            R previousRoute__as94558 = (R)(object)previousRoute;
            List<RouteAware>? previousSubscribers__94610 = this._listeners.GetValueOrDefault(previousRoute__as94558)?.ToList().ToList();
            if ((previousSubscribers__94610 is not null))
            {
                foreach (RouteAware routeAware__94741 in previousSubscribers__94610)
                {
                    routeAware__94741.didPopNext();
                }
            }
            List<RouteAware>? subscribers__94862 = this._listeners.GetValueOrDefault(route__as94544)?.ToList().ToList();
            if ((subscribers__94862 is not null))
            {
                foreach (RouteAware routeAware__94969 in subscribers__94862)
                {
                    routeAware__94969.didPop();
                }
            }
        }
    }

    public override void didPush(dynamic route, dynamic previousRoute)
    {
        if (((route is R) && (previousRoute is R)))
        {
            R route__as95148 = (R)(object)route;
            R previousRoute__as95162 = (R)(object)previousRoute;
            HashSet<RouteAware>? previousSubscribers__95213 = this._listeners.GetValueOrDefault(previousRoute__as95162);
            if ((previousSubscribers__95213 is not null))
            {
                foreach (RouteAware routeAware__95334 in previousSubscribers__95213)
                {
                    routeAware__95334.didPushNext();
                }
            }
        }
    }

}

public abstract class RouteAware
{
    public virtual void didPopNext()
    {
    }

    public virtual void didPush()
    {
    }

    public virtual void didPop()
    {
    }

    public virtual void didPushNext()
    {
    }

}

public class RawDialogRoute<T> : PopupRoute<T>
{
    internal virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> _pageBuilder { get; private set; } = default!;
    internal virtual bool _barrierDismissible { get; private set; } = default!;
    internal virtual string? _barrierLabel { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual Duration _transitionDuration { get; private set; } = default!;
    internal virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget, Widget>? _transitionBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder { get; private set; }
    public virtual Offset? anchorPoint { get; private set; }
    private bool __field_fullscreenDialog = default!;
    public override bool fullscreenDialog { get => __field_fullscreenDialog; }

    public RawDialogRoute(global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> pageBuilder, bool barrierDismissible = true, Color? barrierColor = default!, string? barrierLabel = null, Duration? transitionDuration = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget, Widget>? transitionBuilder = null, global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null, RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null, bool fullscreenDialog = false) : base(settings: settings, requestFocus: requestFocus, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
    {
        Color? __barrierColor = barrierColor ?? new Color(0x80000000);
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 200);
        this.barrierBuilder = barrierBuilder;
        this.anchorPoint = anchorPoint;
        this.__field_fullscreenDialog = fullscreenDialog;
        this._pageBuilder = pageBuilder;
        this._barrierDismissible = barrierDismissible;
        this._barrierLabel = barrierLabel;
        this._barrierColor = __barrierColor;
        this._transitionDuration = __transitionDuration;
        this._transitionBuilder = transitionBuilder;
    }

    public override bool barrierDismissible => this._barrierDismissible;
    public override string? barrierLabel => this._barrierLabel;
    public override Color? barrierColor => this._barrierColor;
    public override Duration transitionDuration => this._transitionDuration;
    public override Widget buildPage(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((Widget)(object?)new Semantics(scopesRoute: true, explicitChildNodes: true, child: new DisplayFeatureSubScreen(anchorPoint: this.anchorPoint, child: this._pageBuilder(context, animation, secondaryAnimation))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        if ((this._transitionBuilder is null))
        {
            return ((Widget)(object?)new FadeTransition(opacity: animation, child: child));
        }
        return this._transitionBuilder(context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildModalBarrier()
    {
        Widget barrier__101884 = ((Widget)(object?)base.buildModalBarrier());
        if ((this.barrierBuilder is not null))
        {
            return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => this.barrierBuilder!(context, new RouteBarrierDetails(animation: this.animation!, barrierColor: this.barrierColor, barrierLabel: this.barrierLabel, barrierDismissible: this.barrierDismissible), barrier__101884)))));
        }
        return barrier__101884;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class RoutesLibrary
{
    public static Future<T?> showGeneralDialog<T>(BuildContext context, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget> pageBuilder, bool barrierDismissible = false, string? barrierLabel = null, Color barrierColor = default!, Duration? transitionDuration = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget, Widget>? transitionBuilder = null, global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null, bool useRootNavigator = true, bool fullscreenDialog = false, RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 200);
        DartRuntimePrimitives.Assert(() => (!barrierDismissible || (barrierLabel is not null)));
        return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: useRootNavigator).push<T>(new RawDialogRoute<T>(pageBuilder: (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>)pageBuilder, barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, barrierColor: barrierColor, transitionDuration: __transitionDuration, transitionBuilder: (global::System.Func<BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget, Widget>?)transitionBuilder, barrierBuilder: (global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>?)barrierBuilder, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate Widget RoutePageBuilder(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation);

public delegate Widget RouteTransitionsBuilder(BuildContext context, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Animation.Animation<double> secondaryAnimation, Widget child);

public class RouteBarrierDetails
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual Color? barrierColor { get; private set; }
    public virtual string? barrierLabel { get; private set; }
    public virtual bool barrierDismissible { get; private set; } = default!;

    public RouteBarrierDetails(global::Doroti.Generated.Framework.Animation.Animation<double> animation, Color? barrierColor = null, string? barrierLabel = null, bool barrierDismissible = default!)
    {
        this.animation = animation;
        this.barrierColor = barrierColor;
        this.barrierLabel = barrierLabel;
        this.barrierDismissible = barrierDismissible;
    }

}

public delegate Widget RouteBarrierBuilder(BuildContext context, RouteBarrierDetails details, Widget barrier);

public delegate void PopInvokedWithResultCallback<T>(bool didPop, T? result);

public abstract class PopEntry<T>
{
    public PopEntry() { }

    public virtual void onPopInvoked(bool didPop)
    {
    }

    public virtual void onPopInvokedWithResult(bool didPop, T? result) => onPopInvoked(didPop);
    public abstract global::Doroti.Generated.Framework.Foundation.ValueListenable<bool> canPopNotifier { get; }
    public override string ToString()
    {
        return $"PopEntry canPop: {(((global::Doroti.Generated.Framework.Foundation.ValueListenable<bool>)this.canPopNotifier).value)}, onPopInvoked: {this.onPopInvokedWithResult}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
