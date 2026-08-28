// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/routes.dart
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
        bool returnValue = base.didPop(result);
        DartRuntimePrimitives.Assert(() => returnValue);
        if (this.finishedWhenPopped)
        {
            this.navigator!.finalizeRoute(this);
        }
        return returnValue;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        foreach (OverlayEntry entry in this._overlayEntries)
        {
            entry.dispose();
        }
        this._overlayEntries.Clear();
        base.dispose();
    }

}

public interface ITransitionRoute
{
    global::Doroti.Framework.Animation.Animation<double>? animation { get; }
    global::Doroti.Framework.Animation.AnimationController? controller { get; }
    Future completed { get; }
    string debugLabel { get; }
    bool canTransitionTo(RouteBase nextRoute);
    bool canTransitionFrom(RouteBase previousRoute);
}

public abstract class TransitionRoute<T> : OverlayRoute<T>, PredictiveBackRoute, ITransitionRoute
{
    internal virtual Completer<T?> _transitionCompleter { get; private set; } = new Completer<T?>();
    internal virtual global::Doroti.Framework.Scheduler.PerformanceModeRequestHandle? _performanceModeRequestHandle { get; set; } = default;
    internal virtual bool _popFinalized { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _animation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _secondaryAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation(global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
    public virtual bool willDisposeAnimationController { get; set; } = true;
    internal virtual global::Doroti.Framework.Physics.Simulation? _simulation { get; set; } = default;
    internal virtual T? _result { get; set; } = default;
    internal virtual global::System.Action? _trainHoppingListenerRemover { get; set; } = default;

    protected TransitionRoute(RouteSettings? settings = null, bool? requestFocus = null) : base(settings: settings, requestFocus: requestFocus)
    {
    }

    public override bool isCurrent => base.isCurrent;
    public virtual bool popGestureEnabled => throw new NotSupportedException();
    public virtual Future<T?> completed => this._transitionCompleter.future;
    Future ITransitionRoute.completed => completed;
    public abstract Duration transitionDuration { get; }
    public virtual Duration reverseTransitionDuration => this.transitionDuration;
    public abstract bool opaque { get; }
    public virtual bool allowSnapshotting => true;
    public override bool finishedWhenPopped => DartRuntimePrimitives.ConvertValue<bool>((this._controller!.isDismissed && !this._popFinalized));
    public virtual global::Doroti.Framework.Animation.Animation<double>? animation => this._animation;
    public virtual global::Doroti.Framework.Animation.AnimationController? controller => this._controller;
    public virtual global::Doroti.Framework.Animation.Animation<double>? secondaryAnimation => this._secondaryAnimation;
    bool ITransitionRoute.canTransitionTo(RouteBase nextRoute) => canTransitionTo(nextRoute);
    bool ITransitionRoute.canTransitionFrom(RouteBase previousRoute) => canTransitionFrom(previousRoute);
    public virtual bool debugTransitionCompleted()
    {
        var disposed = false;
        DartRuntimePrimitives.Assert(() =>
            {
                disposed = this._transitionCompleter.isCompleted;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return DartRuntimePrimitives.RequireValue(disposed);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        Duration durationLocal = this.transitionDuration;
        Duration reverseDurationLocal = this.reverseTransitionDuration;
        return new global::Doroti.Framework.Animation.AnimationController(duration: durationLocal, reverseDuration: reverseDurationLocal, debugLabel: this.debugLabel, vsync: this.navigator!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> createAnimation()
    {
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        DartRuntimePrimitives.Assert(() => (this._controller is not null));
        return this._controller!.view;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(() => (this.transitionDuration >= Duration.zero), () => (object?)$"The `duration` must be positive for a non-simulation animation. Received {this.transitionDuration}.");
        return ((global::Doroti.Framework.Physics.Simulation)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Physics.Simulation? _createSimulationAndVerify(bool forward)
    {
        global::Doroti.Framework.Physics.Simulation? simulation = ((global::Doroti.Framework.Physics.Simulation?)(object?)createSimulation(forward: forward));
        DartRuntimePrimitives.Assert(() => (this.transitionDuration >= Duration.zero), () => (object?)"The `duration` must be positive for an animation that doesn't use simulation. " + "Either set `transitionDuration` or set `createSimulation`. " + $"Received {this.transitionDuration}.");
        return simulation;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        switch (status)
        {
            case global::Doroti.Framework.Animation.AnimationStatus.completed:
                {
                    if (System.Linq.Enumerable.Any(this.overlayEntries))
                    {
                        this.overlayEntries.First().opaque = this.opaque;
                    }
                    this._performanceModeRequestHandle?.dispose();
                    _performanceModeRequestHandle = null;
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.forward:
            case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                {
                    if (System.Linq.Enumerable.Any(this.overlayEntries))
                    {
                        this.overlayEntries.First().opaque = false;
                    }
                    _performanceModeRequestHandle ??= global::Doroti.Framework.Scheduler.SchedulerBinding.instance.requestPerformanceMode(DartPerformanceMode.latency);
                    break;
                }
            case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
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
        _animation = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = createAnimation();
    __cascade.addStatusListener((AnimationStatusListener)this._handleStatusChanged);
    return __cascade;
}))();
        DartRuntimePrimitives.Assert(() => (this._animation is not null), () => (object?)$"{this.GetType()}.createAnimation() returned null.");
        base.install();
        if ((this._animation!.isCompleted && System.Linq.Enumerable.Any(this.overlayEntries)))
        {
            this.overlayEntries.First().opaque = this.opaque;
        }
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didPush called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        base.didPush();
        _simulation = _createSimulationAndVerify(forward: true);
        if ((this._simulation is null))
        {
            return ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)this._controller!.forward());
        }
        else
        {
            return ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)this._controller!.animateWith(this._simulation!));
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
        if (((object?)oldRoute is ITransitionRoute oldTransitionRoute))
        {
            this._controller!.value = oldTransitionRoute.controller!.value;
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
        _updateSecondaryAnimation((object?)nextRoute as RouteBase);
        base.didPopNext((object?)nextRoute);
    }

    public override void didChangeNext(dynamic nextRoute)
    {
        DartRuntimePrimitives.Assert(() => (this._controller is not null), () => (object?)$"{this.GetType()}.didChangeNext called before calling install() or after calling dispose().");
        DartRuntimePrimitives.Assert(() => !debugTransitionCompleted(), () => (object?)$"Cannot reuse a {this.GetType()} after disposing it.");
        _updateSecondaryAnimation((object?)nextRoute as RouteBase);
        base.didChangeNext((object?)nextRoute);
    }

    internal virtual void _updateSecondaryAnimation(RouteBase? nextRoute)
    {
        global::System.Action? previousTrainHoppingListenerRemover = this._trainHoppingListenerRemover;
        _trainHoppingListenerRemover = null;
        if ((((nextRoute is ITransitionRoute nextTransitionRoute) && canTransitionTo(nextRoute)) && nextTransitionRoute.canTransitionFrom(this)))
        {
            global::Doroti.Framework.Animation.Animation<double>? current = ((global::Doroti.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent;
            if ((current is not null))
            {
                global::Doroti.Framework.Animation.Animation<double> currentTrainLocal = (((current is global::Doroti.Framework.Animation.TrainHoppingAnimation) ? ((global::Doroti.Framework.Animation.TrainHoppingAnimation)((global::Doroti.Framework.Animation.TrainHoppingAnimation)current)).currentTrain : current))!;
                global::Doroti.Framework.Animation.Animation<double> nextTrain = nextTransitionRoute.animation!;
                if (((((global::Doroti.Framework.Animation.Animation<double>)currentTrainLocal).value == ((global::Doroti.Framework.Animation.Animation<double>)nextTrain).value) || !((global::Doroti.Framework.Animation.Animation<double>)nextTrain).isAnimating))
                {
                    _setSecondaryAnimation(nextTrain, nextTransitionRoute.completed);
                }
                else
                {
                    global::Doroti.Framework.Animation.TrainHoppingAnimation? newAnimation = default!;
                    void jumpOnAnimationEnd(global::Doroti.Framework.Animation.AnimationStatus status)
                    {
                        if (!global::Doroti.Framework.Animation.AnimationStatusMembers.isAnimating(status))
                        {
                            _setSecondaryAnimation(nextTrain, nextTransitionRoute.completed);
                            if ((this._trainHoppingListenerRemover is not null))
                            {
                                this._trainHoppingListenerRemover!();
                                _trainHoppingListenerRemover = null;
                            }
                        }
                    }
                    _trainHoppingListenerRemover = (global::System.Action)(() =>
                    {
                        nextTrain.removeStatusListener((AnimationStatusListener)jumpOnAnimationEnd);
                        newAnimation?.dispose();
                    });
                    nextTrain.addStatusListener((AnimationStatusListener)jumpOnAnimationEnd);
                    newAnimation = new global::Doroti.Framework.Animation.TrainHoppingAnimation(currentTrainLocal, nextTrain, onSwitchedTrain: ((global::System.Action)(() =>
                    {
                        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent, newAnimation)));
                        DartRuntimePrimitives.Assert(() => (object.Equals(newAnimation!.currentTrain, nextTransitionRoute.animation)));
                        _setSecondaryAnimation(newAnimation!.currentTrain, nextTransitionRoute.completed);
                        if ((this._trainHoppingListenerRemover is not null))
                        {
                            this._trainHoppingListenerRemover!();
                            _trainHoppingListenerRemover = null;
                        }
                    })));
                    _setSecondaryAnimation(newAnimation, nextTransitionRoute.completed);
                }
            }
            else
            {
                _setSecondaryAnimation(nextTransitionRoute.animation, nextTransitionRoute.completed);
            }
        }
        else
        {
            _setSecondaryAnimation(global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation);
        }
        previousTrainHoppingListenerRemover?.Invoke();
    }

    internal virtual void _setSecondaryAnimation(global::Doroti.Framework.Animation.Animation<double>? animation, Future? disposed = null)
    {
        this._secondaryAnimation.parent = animation;
        if (disposed is not null)
        {
            DartRuntimePrimitives.Ignore(disposed.then((global::System.Action<object>)((_) =>
            {
                if ((object.Equals(((global::Doroti.Framework.Animation.ProxyAnimation)this._secondaryAnimation).parent, animation)))
                {
                    this._secondaryAnimation.parent = global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation;
                    if ((animation is global::Doroti.Framework.Animation.TrainHoppingAnimation))
                    {
                        global::Doroti.Framework.Animation.TrainHoppingAnimation animation__as19490 = (global::Doroti.Framework.Animation.TrainHoppingAnimation)animation;
                        ((global::Doroti.Framework.Animation.TrainHoppingAnimation)animation__as19490).dispose();
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
            AnimationStatusListener animationStatusCallback = default!;
            animationStatusCallback = ((status) =>
            {
                this.navigator?.didStopUserGesture();
                this._controller!.removeStatusListener((AnimationStatusListener)animationStatusCallback);
            });
            this._controller!.addStatusListener((AnimationStatusListener)animationStatusCallback);
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

    public virtual string debugLabel => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute");
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute"))}(animation: {this._controller})";
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
        ModalRoute<object> route = ModalRoute<object>.of<object>(this.context)!;
        return route.barrierDismissible;
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
    public virtual RouteBase route { get; private set; } = default!;

    internal _ModalScopeStatus__routes(bool isCurrent, bool canPop, bool impliesAppBarDismissal, RouteBase route, bool opaque, Widget child) : base(child: child)
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

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Framework.Foundation.FlagProperty("isCurrent", value: this.isCurrent, ifTrue: "active", ifFalse: "inactive"));
        description.add(new global::Doroti.Framework.Foundation.FlagProperty("canPop", value: this.canPop, ifTrue: "can pop"));
        description.add(new global::Doroti.Framework.Foundation.FlagProperty("impliesAppBarDismissal", value: this.impliesAppBarDismissal, ifTrue: "implies app bar dismissal"));
    }

    public override bool updateShouldNotifyDependent(InheritedModel<_ModalRouteAspect__routes> oldWidget, HashSet<_ModalRouteAspect__routes> dependencies)
    {
        var __oldWidget = (_ModalScopeStatus__routes)(object)oldWidget;
        return dependencies.any(((dependency) => (dependency switch { _ModalRouteAspect__routes.isCurrent => (this.isCurrent != ((_ModalScopeStatus__routes)__oldWidget).isCurrent), _ModalRouteAspect__routes.canPop => (this.canPop != ((_ModalScopeStatus__routes)__oldWidget).canPop), _ModalRouteAspect__routes.settings => (!object.Equals(this.route.settings, ((_ModalScopeStatus__routes)__oldWidget).route.settings)), _ModalRouteAspect__routes.isActive => (this.route.isActive != ((_ModalScopeStatus__routes)__oldWidget).route.isActive), _ModalRouteAspect__routes.isFirst => (this.route.isFirst != ((_ModalScopeStatus__routes)__oldWidget).route.isFirst), _ModalRouteAspect__routes.opaque => (this.opaque != ((_ModalScopeStatus__routes)__oldWidget).opaque), _ModalRouteAspect__routes.popDisposition => (!object.Equals(this.route.popDisposition, ((_ModalScopeStatus__routes)__oldWidget).route.popDisposition)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _ModalScope__routes<T> : StatefulWidget
{
    public virtual ModalRoute<T> route { get; private set; } = default!;

    internal _ModalScope__routes(global::Doroti.Framework.Foundation.Key? key = null, ModalRoute<T> route = default!) : base(key: key)
    {
        this.route = route;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ModalScopeState__routes<T>());
}

public class _ModalScopeState__routes<T> : State<_ModalScope__routes<T>>
{
    internal virtual Widget? _page { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.Listenable _listenable { get; set; } = default!;
    public virtual FocusScopeNode focusScopeNode { get; private set; } = new FocusScopeNode(debugLabel: $"{typeof(_ModalScopeState__routes<T>)} Focus Scope");
    public virtual ScrollController primaryScrollController { get; private set; } = new ScrollController();

    public override void initState()
    {
        base.initState();
        var animations = new List<global::Doroti.Framework.Foundation.Listenable>();
        _listenable = global::Doroti.Framework.Foundation.Listenable.CreateMerge(animations.Cast<global::Doroti.Framework.Foundation.Listenable?>());
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
        TraversalEdgeBehavior traversalEdgeBehaviorLocal = default!;
        TraversalEdgeBehavior directionalTraversalEdgeBehaviorLocal = default!;
        ModalRoute<T> routeLocal = ((_ModalScope__routes<T>)(object)this.widget).route;
        if ((((ModalRoute<T>)routeLocal).traversalEdgeBehavior is not null))
        {
            traversalEdgeBehaviorLocal = DartRuntimePrimitives.RequireValue(((ModalRoute<T>)routeLocal).traversalEdgeBehavior);
        }
        else
        {
            traversalEdgeBehaviorLocal = routeLocal.navigator!.widget.routeTraversalEdgeBehavior;
        }
        if ((((ModalRoute<T>)routeLocal).directionalTraversalEdgeBehavior is not null))
        {
            directionalTraversalEdgeBehaviorLocal = DartRuntimePrimitives.RequireValue(((ModalRoute<T>)routeLocal).directionalTraversalEdgeBehavior);
        }
        else
        {
            directionalTraversalEdgeBehaviorLocal = routeLocal.navigator!.widget.routeDirectionalTraversalEdgeBehavior;
        }
        this.focusScopeNode.traversalEdgeBehavior = traversalEdgeBehaviorLocal;
        this.focusScopeNode.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehaviorLocal;
        if ((routeLocal.isCurrent && this._shouldRequestFocus))
        {
            routeLocal.navigator!.focusNode.enclosingScope?.setFirstFocus(this.focusScopeNode);
        }
    }

    internal virtual void _forceRebuildPage()
    {
        setState(((global::System.Action)(() =>
        {
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
            return ((object.Equals(((_ModalScope__routes<T>)(object)this.widget).route.animation?.status, global::Doroti.Framework.Animation.AnimationStatus.reverse)) || ((((_ModalScope__routes<T>)(object)this.widget).route.navigator?.userGestureInProgress ?? false)));
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
        return ((Widget)(object?)new AnimatedBuilder(animation: ((_ModalScope__routes<T>)(object)this.widget).route.restorationScopeId, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            DartRuntimePrimitives.Assert(() => (child is not null));
            return ((Widget)(object?)new RestorationScope(restorationId: ((_ModalScope__routes<T>)(object)this.widget).route.restorationScopeId.value, child: child!));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: new _ModalScopeStatus__routes(route: ((_ModalScope__routes<T>)(object)this.widget).route, isCurrent: ((_ModalScope__routes<T>)(object)this.widget).route.isCurrent, canPop: ((_ModalScope__routes<T>)(object)this.widget).route.canPop, opaque: ((_ModalScope__routes<T>)(object)this.widget).route.opaque, impliesAppBarDismissal: ((_ModalScope__routes<T>)(object)this.widget).route.impliesAppBarDismissal, child: new Offstage(offstage: ((_ModalScope__routes<T>)(object)this.widget).route.offstage, child: new PageStorage(bucket: ((_ModalScope__routes<T>)(object)this.widget).route._storageBucket, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
        {
            return ((Widget)(object?)new Actions(actions: new DartMap<Type, dynamic> { [typeof(DismissIntent)] = new _DismissModalAction__routes(context) }, child: new PrimaryScrollController(controller: this.primaryScrollController, child: FocusScope.CreateWithExternalFocusNode(focusScopeNode: this.focusScopeNode, child: new RepaintBoundary(child: new ListenableBuilder(listenable: this._listenable, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
            {
                return ((Widget)(object?)((_ModalScope__routes<T>)(object)this.widget).route._buildFlexibleTransitions(context, ((_ModalScope__routes<T>)(object)this.widget).route.animation!, ((_ModalScope__routes<T>)(object)this.widget).route.secondaryAnimation!, new ListenableBuilder(listenable: (((_ModalScope__routes<T>)(object)this.widget).route.navigator?.userGestureInProgressNotifier ?? new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false)), builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
                {
                    bool ignoreEvents = this._shouldIgnoreFocusRequest;
                    this.focusScopeNode.canRequestFocus = !ignoreEvents;
                    return ((Widget)(object?)new IgnorePointer(ignoring: ignoreEvents, child: child));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })), child: child)));
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), child: _page ??= new RepaintBoundary(key: ((_ModalScope__routes<T>)(object)this.widget).route._subtreeKey, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) =>
            {
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
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>? receivedTransition { get; set; } = default;
    internal virtual bool _offstage { get; set; } = false;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation? _animationProxy { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation? _secondaryAnimationProxy { get; set; } = default;
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

    public static bool? isCurrentOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isCurrent)?.isCurrent;
    public static bool? canPopOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.canPop)?.canPop;
    public static RouteSettings? settingsOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.settings)?.settings;
    public static bool? isActiveOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isActive)?.isActive;
    public static bool? isFirstOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.isFirst)?.isFirst;
    public static bool? opaqueOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.opaque)?.opaque;
    public static RoutePopDisposition? popDispositionOf(BuildContext context) => ModalRoute<T>._of<T>(context, _ModalRouteAspect__routes.popDisposition)?.popDisposition;
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
        return ((global::System.Func<dynamic, bool>)((route) =>
        {
            return (((object?)route is ModalRoute<T> typedRoute) && !typedRoute.willHandlePopInternally && (typedRoute.settings.ToString() == name));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Widget buildPage(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation);
    public virtual Widget buildTransitions(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>? delegatedTransition => DartRuntimePrimitives.ConvertValue<global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>>(null);
    internal virtual Widget _buildFlexibleTransitions(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
    {
        if (((this.receivedTransition is null) || ((global::Doroti.Framework.Animation.Animation<double>)secondaryAnimation).isDismissed))
        {
            return ((Widget)(object?)buildTransitions(context, animation, secondaryAnimation, child));
        }
        var proxyAnimation = new global::Doroti.Framework.Animation.ProxyAnimation();
        Widget proxiedOriginalTransitions = ((Widget)(object?)buildTransitions(context, animation, proxyAnimation, child));
        return (this.receivedTransition!(context, animation, secondaryAnimation, this.allowSnapshotting, proxiedOriginalTransitions) ?? proxiedOriginalTransitions);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void install()
    {
        base.install();
        _animationProxy = new global::Doroti.Framework.Animation.ProxyAnimation(base.animation);
        _secondaryAnimationProxy = new global::Doroti.Framework.Animation.ProxyAnimation(base.secondaryAnimation);
    }

    public override global::Doroti.Framework.Scheduler.TickerFuture didPush()
    {
        if (((((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState is not null) && this.navigator!.widget.requestFocus))
        {
            this.navigator!.focusNode.enclosingScope?.setFirstFocus(((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState!.focusScopeNode);
        }
        return ((global::Doroti.Framework.Scheduler.TickerFuture)(object?)base.didPush());
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
    public virtual global::Doroti.Framework.Animation.Curve barrierCurve => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Curve>(global::Doroti.Framework.Animation.Curves.ease);
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
            setState(((global::System.Action)(() =>
            {
                _offstage = __value;
            })));
            this._animationProxy!.parent = (this._offstage ? global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation : base.animation);
            this._secondaryAnimationProxy!.parent = (this._offstage ? global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysDismissedAnimation : base.secondaryAnimation);
            changedInternalState();
        }
    }
    public virtual BuildContext? subtreeContext => ((GlobalKey<IState>)this._subtreeKey).currentContext;
    public override global::Doroti.Framework.Animation.Animation<double>? animation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(this._animationProxy);
    public override global::Doroti.Framework.Animation.Animation<double>? secondaryAnimation => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(this._secondaryAnimationProxy);
    public async override Future<RoutePopDisposition> willPop()
    {
        _ModalScopeState__routes<T>? scope = ((_ModalScopeState__routes<T>?)(object?)((GlobalKey<_ModalScopeState__routes<T>>)this._scopeKey).currentState);
        DartRuntimePrimitives.Assert(() => (scope is not null));
        foreach (var callback in new List<global::System.Func<Future<bool>>>(DartRuntimePrimitives.ConvertEnumerable<global::System.Func<Future<bool>>>(this._willPopCallbacks)))
        {
            if (!await callback())
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
            foreach (dynamic popEntry in this._popEntries)
            {
                if (!((global::Doroti.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry).canPopNotifier).value)
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
        foreach (dynamic popEntry in this._popEntries)
        {
            ((dynamic)popEntry).onPopInvokedWithResult(didPop, result);
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
        ((global::Doroti.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry).canPopNotifier).addListener(this._maybeDispatchNavigationNotification);
        _maybeDispatchNavigationNotification();
    }

    public virtual void unregisterPopEntry(dynamic popEntry)
    {
        this._popEntries.Remove(popEntry);
        ((global::Doroti.Framework.Foundation.ValueListenable<bool>)((dynamic)popEntry).canPopNotifier).removeListener(this._maybeDispatchNavigationNotification);
        _maybeDispatchNavigationNotification();
    }

    internal virtual void _maybeDispatchNavigationNotification()
    {
        if (!this.isCurrent)
        {
            return;
        }
        var notification = new NavigationNotification(canHandlePop: ((object.Equals(this.popDisposition, RoutePopDisposition.doNotPop)) || System.Linq.Enumerable.Any(this._willPopCallbacks)));
        switch (global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase)
        {
            case global::Doroti.Framework.Scheduler.SchedulerPhase.postFrameCallbacks:
                {
                    notification.dispatch(this.subtreeContext);
                    break;
                }
            case global::Doroti.Framework.Scheduler.SchedulerPhase.idle:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.midFrameMicrotasks:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks:
            case global::Doroti.Framework.Scheduler.SchedulerPhase.transientCallbacks:
                {
                    global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
                    {
                        if (!((this.subtreeContext?.mounted ?? false)))
                        {
                            return;
                        }
                        notification.dispatch(this.subtreeContext);
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
        if ((((nextRoute is ModalRoute<T>) && canTransitionTo(nextRoute)) && (!object.Equals((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)((ModalRoute<T>)nextRoute).delegatedTransition, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)this.delegatedTransition))))
        {
            ModalRoute<T> nextRoute__as84419 = (ModalRoute<T>)nextRoute;
            receivedTransition = (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>)((ModalRoute<T>)nextRoute__as84419).delegatedTransition;
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
        if ((((nextRoute is ModalRoute<T>) && canTransitionTo(nextRoute)) && (!object.Equals((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)((ModalRoute<T>)nextRoute).delegatedTransition, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>?)this.delegatedTransition))))
        {
            ModalRoute<T> nextRoute__as84796 = (ModalRoute<T>)nextRoute;
            receivedTransition = (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, bool, Widget?, Widget?>)((ModalRoute<T>)nextRoute__as84796).delegatedTransition;
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
        if ((!object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
        {
            setState(((global::System.Action)(() =>
            {
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
        Widget barrier = ((Widget)(object?)buildModalBarrier());
        if ((this.filter is not null))
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(new BackdropFilter(filter: this.filter, child: barrier));
        }
        barrier = DartRuntimePrimitives.ConvertValue<Widget>(new IgnorePointer(ignoring: !this.animation!.isForwardOrCompleted, child: barrier));
        if ((this.semanticsDismissible && this.barrierDismissible))
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(new Semantics(sortKey: new global::Doroti.Framework.Semantics.OrdinalSortKey(1.0), child: barrier));
        }
        return barrier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildModalBarrier()
    {
        Widget barrier = default!;
        if ((((this.barrierColor is not null) && (this.barrierColor!.alpha != 0L)) && !this.offstage))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(this.barrierColor, this.barrierColor!.withOpacity(0.0))));
            global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Color?> colorLocal = ((global::Doroti.Framework.Animation.Animation<global::Doroti.Ui.Color?>)(object?)this.animation!.drive(new global::Doroti.Framework.Animation.ColorTween(begin: this.barrierColor!.withOpacity(0.0), end: this.barrierColor).chain(new global::Doroti.Framework.Animation.CurveTween(curve: this.barrierCurve))));
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(new AnimatedModalBarrier(color: colorLocal, dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible));
        }
        else
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(new ModalBarrier(dismissible: this.barrierDismissible, semanticsLabel: this.barrierLabel, barrierSemanticsDismissible: this.semanticsDismissible));
        }
        return barrier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildModalScope(BuildContext context)
    {
        return _modalScopeCache ??= new Semantics(sortKey: new global::Doroti.Framework.Semantics.OrdinalSortKey(0.0), child: new _ModalScope__routes<T>(key: this._scopeKey, route: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IEnumerable<OverlayEntry> createOverlayEntries()
    {
        return ((IEnumerable<OverlayEntry>)(object?)new List<OverlayEntry> { (_modalBarrier = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildModalBarrier)), (_modalScope = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._buildModalScope, maintainState: this.maintainState, canSizeOverlay: this.opaque)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ModalRoute"))}({this.settings}, animation: {this._animation})";
    public virtual void addLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => (((LocalHistoryEntry)entry)._owner is null));
        entry._owner = DartRuntimePrimitives.ConvertValue<LocalHistoryRoute<object>>(this);
        this._localHistory ??= new List<LocalHistoryEntry>();
        bool wasEmpty = !System.Linq.Enumerable.Any(this._localHistory!);
        this._localHistory!.Add(entry);
        var internalStateChanged = false;
        if (((LocalHistoryEntry)entry).impliesAppBarDismissal)
        {
            internalStateChanged = (this._entriesImpliesAppBarDismissal == 0L);
            this._entriesImpliesAppBarDismissal += 1L;
        }
        if ((wasEmpty || internalStateChanged))
        {
            changedInternalState();
        }
    }

    public virtual void removeLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((LocalHistoryEntry)entry)._owner, this)));
        DartRuntimePrimitives.Assert(() => this._localHistory!.Contains(entry));
        var internalStateChanged = false;
        if ((this._localHistory!.Remove(entry) && ((LocalHistoryEntry)entry).impliesAppBarDismissal))
        {
            this._entriesImpliesAppBarDismissal -= 1L;
            internalStateChanged = (this._entriesImpliesAppBarDismissal == 0L);
        }
        entry._owner = null;
        entry._notifyRemoved();
        if ((!System.Linq.Enumerable.Any(this._localHistory!) || internalStateChanged))
        {
            DartRuntimePrimitives.Assert(() => (this._entriesImpliesAppBarDismissal == 0L));
            if ((object.Equals(global::Doroti.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Framework.Scheduler.SchedulerPhase.persistentCallbacks)))
            {
                global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((duration) =>
                {
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
            LocalHistoryEntry entry = this._localHistory!.removeLast<LocalHistoryEntry>();
            DartRuntimePrimitives.Assert(() => (object.Equals(((LocalHistoryEntry)entry)._owner, this)));
            entry._owner = null;
            entry._notifyRemoved();
            var internalStateChanged = false;
            if (((LocalHistoryEntry)entry).impliesAppBarDismissal)
            {
                this._entriesImpliesAppBarDismissal -= 1L;
                internalStateChanged = (this._entriesImpliesAppBarDismissal == 0L);
            }
            if ((!System.Linq.Enumerable.Any(this._localHistory!) || internalStateChanged))
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
        bool contained = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                contained = this._listeners.ContainsKey(route);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return contained;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void subscribe(RouteAware routeAware, R route)
    {
        HashSet<RouteAware> subscribers = this._listeners.putIfAbsent(route, (() => new HashSet<RouteAware>()));
        if (subscribers.Add(routeAware))
        {
            routeAware.didPush();
        }
    }

    public virtual void unsubscribe(RouteAware routeAware)
    {
        List<R> routes = this._listeners.Keys.ToList().ToList();
        foreach (var route in routes)
        {
            HashSet<RouteAware>? subscribers = this._listeners.GetValueOrDefault(route);
            if ((subscribers is not null))
            {
                subscribers.Remove(routeAware);
                if (!System.Linq.Enumerable.Any(subscribers))
                {
                    this._listeners.remove(route);
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
            List<RouteAware>? previousSubscribers = this._listeners.GetValueOrDefault(previousRoute__as94558)?.ToList().ToList();
            if ((previousSubscribers is not null))
            {
                foreach (RouteAware routeAware in previousSubscribers)
                {
                    routeAware.didPopNext();
                }
            }
            List<RouteAware>? subscribers = this._listeners.GetValueOrDefault(route__as94544)?.ToList().ToList();
            if ((subscribers is not null))
            {
                foreach (RouteAware routeAwareLocal in subscribers)
                {
                    routeAwareLocal.didPop();
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
            HashSet<RouteAware>? previousSubscribers = this._listeners.GetValueOrDefault(previousRoute__as95162);
            if ((previousSubscribers is not null))
            {
                foreach (RouteAware routeAware in previousSubscribers)
                {
                    routeAware.didPushNext();
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
    internal virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget> _pageBuilder { get; private set; } = default!;
    internal virtual bool _barrierDismissible { get; private set; } = default!;
    internal virtual string? _barrierLabel { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual Duration _transitionDuration { get; private set; } = default!;
    internal virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget>? _transitionBuilder { get; private set; }
    public virtual global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder { get; private set; }
    public virtual Offset? anchorPoint { get; private set; }
    private bool __field_fullscreenDialog = default!;
    public override bool fullscreenDialog { get => __field_fullscreenDialog; }

    public RawDialogRoute(global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget> pageBuilder, bool barrierDismissible = true, Color? barrierColor = default!, string? barrierLabel = null, Duration? transitionDuration = null, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget>? transitionBuilder = null, global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null, RouteSettings? settings = null, bool? requestFocus = null, Offset? anchorPoint = null, TraversalEdgeBehavior? traversalEdgeBehavior = null, TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null, bool fullscreenDialog = false) : base(settings: settings, requestFocus: requestFocus, traversalEdgeBehavior: traversalEdgeBehavior, directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior)
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
    public override Widget buildPage(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation)
    {
        return ((Widget)(object?)new Semantics(scopesRoute: true, explicitChildNodes: true, child: new DisplayFeatureSubScreen(anchorPoint: this.anchorPoint, child: this._pageBuilder(context, animation, secondaryAnimation))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildTransitions(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child)
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
        Widget barrier = ((Widget)(object?)base.buildModalBarrier());
        if ((this.barrierBuilder is not null))
        {
            return ((Widget)(object?)new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => this.barrierBuilder!(context, new RouteBarrierDetails(animation: this.animation!, barrierColor: this.barrierColor, barrierLabel: this.barrierLabel, barrierDismissible: this.barrierDismissible), barrier)))));
        }
        return barrier;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class RoutesLibrary
{
    public static Future<T?> showGeneralDialog<T>(BuildContext context, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget> pageBuilder, bool barrierDismissible = false, string? barrierLabel = null, Color barrierColor = default!, Duration? transitionDuration = null, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget>? transitionBuilder = null, global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null, bool useRootNavigator = true, bool fullscreenDialog = false, RouteSettings? routeSettings = null, Offset? anchorPoint = null, bool? requestFocus = null)
    {
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 200);
        DartRuntimePrimitives.Assert(() => (!barrierDismissible || (barrierLabel is not null)));
        return ((Future<T?>)(object?)Navigator.of(context, rootNavigator: useRootNavigator).push<T>(new RawDialogRoute<T>(pageBuilder: (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget>)pageBuilder, barrierDismissible: barrierDismissible, barrierLabel: barrierLabel, barrierColor: barrierColor, transitionDuration: __transitionDuration, transitionBuilder: (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Animation.Animation<double>, Widget, Widget>?)transitionBuilder, barrierBuilder: (global::System.Func<BuildContext, RouteBarrierDetails, Widget, Widget>?)barrierBuilder, settings: routeSettings, anchorPoint: anchorPoint, requestFocus: requestFocus, fullscreenDialog: fullscreenDialog)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate Widget RoutePageBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation);

public delegate Widget RouteTransitionsBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, Widget child);

public class RouteBarrierDetails
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual Color? barrierColor { get; private set; }
    public virtual string? barrierLabel { get; private set; }
    public virtual bool barrierDismissible { get; private set; } = default!;

    public RouteBarrierDetails(global::Doroti.Framework.Animation.Animation<double> animation, Color? barrierColor = null, string? barrierLabel = null, bool barrierDismissible = default!)
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
    public abstract global::Doroti.Framework.Foundation.ValueListenable<bool> canPopNotifier { get; }
    public override string ToString()
    {
        return $"PopEntry canPop: {(((global::Doroti.Framework.Foundation.ValueListenable<bool>)this.canPopNotifier).value)}, onPopInvoked: {this.onPopInvokedWithResult}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
