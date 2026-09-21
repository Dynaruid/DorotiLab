// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/routes.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public abstract class OverlayRoute<T> : Route<T>
{
    internal virtual List<OverlayEntry> _overlayEntries { get; private set; } =
        new List<OverlayEntry>();

    protected OverlayRoute(RouteSettings? settings = null, bool? requestFocus = null)
        : base(settings: settings, requestFocus: requestFocus) { }

    public abstract IEnumerable<OverlayEntry> createOverlayEntries();
    public override List<OverlayEntry> overlayEntries => _overlayEntries;

    public override void install()
    {
        DartRuntimePrimitives.Assert(() => !Enumerable.Any(_overlayEntries));
        _overlayEntries.AddRange(createOverlayEntries());
        base.install();
    }

    public virtual bool finishedWhenPopped => true;

    public override bool didPop(T? result)
    {
        bool returnValue = base.didPop(result);
        DartRuntimePrimitives.Assert(() => returnValue);
        if (finishedWhenPopped)
        {
            navigator!.finalizeRoute(this);
        }
        return returnValue;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        foreach (OverlayEntry entry in _overlayEntries)
        {
            entry.dispose();
        }
        _overlayEntries.Clear();
        base.dispose();
    }
}

public interface ITransitionRoute
{
    Animation<double>? animation { get; }
    AnimationController? controller { get; }
    Future completed { get; }
    string debugLabel { get; }
    bool canTransitionTo(RouteBase nextRoute);
    bool canTransitionFrom(RouteBase previousRoute);
}

public abstract class TransitionRoute<T> : OverlayRoute<T>, PredictiveBackRoute, ITransitionRoute
{
    internal virtual Completer<T?> _transitionCompleter { get; private set; } = new Completer<T?>();
    internal virtual Scheduler.PerformanceModeRequestHandle? _performanceModeRequestHandle { get; set; } =
        default;
    internal virtual bool _popFinalized { get; set; } = false;
    internal virtual Animation<double>? _animation { get; set; } = default;
    internal virtual AnimationController? _controller { get; set; } = default;
    internal virtual ProxyAnimation _secondaryAnimation { get; private set; } =
        new ProxyAnimation(AnimationsLibrary.kAlwaysDismissedAnimation);
    public virtual bool willDisposeAnimationController { get; set; } = true;
    internal virtual Physics.Simulation? _simulation { get; set; } = default;
    internal virtual T? _result { get; set; } = default;
    internal virtual Action? _trainHoppingListenerRemover { get; set; } = default;

    protected TransitionRoute(RouteSettings? settings = null, bool? requestFocus = null)
        : base(settings: settings, requestFocus: requestFocus) { }

    public override bool isCurrent => base.isCurrent;
    public virtual bool popGestureEnabled => throw new NotSupportedException();
    public virtual Future<T?> completed => _transitionCompleter.future;
    Future ITransitionRoute.completed => completed;
    public abstract Duration transitionDuration { get; }
    public virtual Duration reverseTransitionDuration => transitionDuration;
    public abstract bool opaque { get; }
    public virtual bool allowSnapshotting => true;
    public override bool finishedWhenPopped =>
        DartRuntimePrimitives.ConvertValue<bool>(_controller!.isDismissed && !_popFinalized);
    public virtual Animation<double>? animation => _animation;
    public virtual AnimationController? controller => _controller;
    public virtual Animation<double>? secondaryAnimation => _secondaryAnimation;

    bool ITransitionRoute.canTransitionTo(RouteBase nextRoute) => canTransitionTo(nextRoute);

    bool ITransitionRoute.canTransitionFrom(RouteBase previousRoute) =>
        canTransitionFrom(previousRoute);

    public virtual bool debugTransitionCompleted()
    {
        var disposed = false;
        DartRuntimePrimitives.Assert(() =>
        {
            disposed = _transitionCompleter.isCompleted;
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return (disposed);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual AnimationController createAnimationController()
    {
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        Duration durationLocal = transitionDuration;
        Duration reverseDurationLocal = reverseTransitionDuration;
        return new AnimationController(
            duration: durationLocal,
            reverseDuration: reverseDurationLocal,
            debugLabel: debugLabel,
            vsync: navigator!
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Animation<double> createAnimation()
    {
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        DartRuntimePrimitives.Assert(() => _controller is not null);
        return _controller!.view;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Physics.Simulation? createSimulation(bool forward)
    {
        DartRuntimePrimitives.Assert(
            () => transitionDuration >= Duration.zero,
            () =>
                (object?)
                    $"The `duration` must be positive for a non-simulation animation. Received {transitionDuration}."
        );
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Physics.Simulation? _createSimulationAndVerify(bool forward)
    {
        Physics.Simulation? simulation = createSimulation(forward: forward);
        DartRuntimePrimitives.Assert(
            () => transitionDuration >= Duration.zero,
            () =>
                (object?)
                    "The `duration` must be positive for an animation that doesn't use simulation. "
                + "Either set `transitionDuration` or set `createSimulation`. "
                + $"Received {transitionDuration}."
        );
        return simulation;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleStatusChanged(AnimationStatus status)
    {
        switch (status)
        {
            case AnimationStatus.completed:
            {
                if (Enumerable.Any(overlayEntries))
                {
                    overlayEntries.First().opaque = opaque;
                }
                _performanceModeRequestHandle?.dispose();
                _performanceModeRequestHandle = null;
                break;
            }
            case AnimationStatus.forward:
            case AnimationStatus.reverse:
            {
                if (Enumerable.Any(overlayEntries))
                {
                    overlayEntries.First().opaque = false;
                }
                _performanceModeRequestHandle ??=
                    Scheduler.SchedulerBinding.instance.requestPerformanceMode(
                        DartPerformanceMode.latency
                    );
                break;
            }
            case AnimationStatus.dismissed:
            {
                if (!isActive)
                {
                    navigator!.finalizeRoute(this);
                    _popFinalized = true;
                    _performanceModeRequestHandle?.dispose();
                    _performanceModeRequestHandle = null;
                }
                break;
            }
        }
    }

    public override void install()
    {
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot install a {GetType()} after disposing it."
        );
        _controller = createAnimationController();
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () => (object?)$"{GetType()}.createAnimationController() returned null."
        );
        _animation = (
            (Func<Animation<double>>)(
                () =>
                {
                    var __cascade = createAnimation();
                    __cascade.addStatusListener(_handleStatusChanged);
                    return __cascade;
                }
            )
        )();
        DartRuntimePrimitives.Assert(
            () => _animation is not null,
            () => (object?)$"{GetType()}.createAnimation() returned null."
        );
        base.install();
        if (_animation!.isCompleted && Enumerable.Any(overlayEntries))
        {
            overlayEntries.First().opaque = opaque;
        }
    }

    public override Scheduler.TickerFuture didPush()
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didPush called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        base.didPush();
        _simulation = _createSimulationAndVerify(forward: true);
        if (_simulation is null)
        {
            return _controller!.forward();
        }
        else
        {
            return _controller!.animateWith(_simulation!);
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didAdd()
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didPush called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        base.didAdd();
        _controller!.value = _controller!.upperBound;
    }

    public override void didReplace(dynamic? oldRoute)
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didReplace called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        if ((object?)oldRoute is ITransitionRoute oldTransitionRoute)
        {
            _controller!.value = oldTransitionRoute.controller!.value;
        }
        base.didReplace((object?)oldRoute);
    }

    public override bool didPop(T? result)
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didPop called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !_transitionCompleter.isCompleted,
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        _result = result;
        _simulation = _createSimulationAndVerify(forward: false);
        if (_simulation is null)
        {
            _controller!.reverse();
        }
        else
        {
            _controller!.animateBackWith(_simulation!);
        }
        return base.didPop(result);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didPopNext(dynamic nextRoute)
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didPopNext called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        _updateSecondaryAnimation((object?)nextRoute as RouteBase);
        base.didPopNext((object)nextRoute);
    }

    public override void didChangeNext(dynamic? nextRoute)
    {
        DartRuntimePrimitives.Assert(
            () => _controller is not null,
            () =>
                (object?)
                    $"{GetType()}.didChangeNext called before calling install() or after calling dispose()."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot reuse a {GetType()} after disposing it."
        );
        _updateSecondaryAnimation((object?)nextRoute as RouteBase);
        base.didChangeNext((object?)nextRoute);
    }

    internal virtual void _updateSecondaryAnimation(RouteBase? nextRoute)
    {
        Action? previousTrainHoppingListenerRemover = _trainHoppingListenerRemover;
        _trainHoppingListenerRemover = null;
        if (
            (nextRoute is ITransitionRoute nextTransitionRoute)
            && canTransitionTo((object)nextRoute)
            && nextTransitionRoute.canTransitionFrom(this)
        )
        {
            Animation<double>? current = _secondaryAnimation.parent;
            if (current is not null)
            {
                Animation<double> currentTrainLocal = (
                    (current is TrainHoppingAnimation)
                        ? ((TrainHoppingAnimation)current).currentTrain
                        : current
                )!;
                Animation<double> nextTrain = nextTransitionRoute.animation!;
                if ((currentTrainLocal.value == nextTrain.value) || !nextTrain.isAnimating)
                {
                    _setSecondaryAnimation(nextTrain, nextTransitionRoute.completed);
                }
                else
                {
                    TrainHoppingAnimation? newAnimation = default!;
                    void jumpOnAnimationEnd(AnimationStatus status)
                    {
                        if (!AnimationStatusMembers.isAnimating(status))
                        {
                            _setSecondaryAnimation(nextTrain, nextTransitionRoute.completed);
                            if (_trainHoppingListenerRemover is not null)
                            {
                                _trainHoppingListenerRemover!();
                                _trainHoppingListenerRemover = null;
                            }
                        }
                    }
                    _trainHoppingListenerRemover = () =>
                    {
                        nextTrain.removeStatusListener(jumpOnAnimationEnd);
                        newAnimation?.dispose();
                    };
                    nextTrain.addStatusListener(jumpOnAnimationEnd);
                    newAnimation = new TrainHoppingAnimation(
                        currentTrainLocal,
                        nextTrain,
                        onSwitchedTrain: () =>
                        {
                            DartRuntimePrimitives.Assert(() =>
                                Equals(_secondaryAnimation.parent, newAnimation)
                            );
                            DartRuntimePrimitives.Assert(() =>
                                Equals(newAnimation!.currentTrain, nextTransitionRoute.animation)
                            );
                            _setSecondaryAnimation(
                                newAnimation!.currentTrain,
                                nextTransitionRoute.completed
                            );
                            if (_trainHoppingListenerRemover is not null)
                            {
                                _trainHoppingListenerRemover!();
                                _trainHoppingListenerRemover = null;
                            }
                        }
                    );
                    _setSecondaryAnimation(newAnimation, nextTransitionRoute.completed);
                }
            }
            else
            {
                _setSecondaryAnimation(
                    nextTransitionRoute.animation,
                    nextTransitionRoute.completed
                );
            }
        }
        else
        {
            _setSecondaryAnimation(AnimationsLibrary.kAlwaysDismissedAnimation);
        }
        previousTrainHoppingListenerRemover?.Invoke();
    }

    internal virtual void _setSecondaryAnimation(
        Animation<double>? animation,
        Future? disposed = null
    )
    {
        _secondaryAnimation.parent = animation;
        if (disposed is not null)
        {
            DartRuntimePrimitives.Ignore(
                disposed.then(
                    (_) =>
                    {
                        if (Equals(_secondaryAnimation.parent, animation))
                        {
                            _secondaryAnimation.parent =
                                AnimationsLibrary.kAlwaysDismissedAnimation;
                            if (animation is TrainHoppingAnimation)
                            {
                                TrainHoppingAnimation animation__as19490 =
                                    (TrainHoppingAnimation)animation;
                                animation__as19490.dispose();
                            }
                        }
                    }
                )
            );
        }
    }

    public virtual bool canTransitionTo(dynamic nextRoute) => true;

    public virtual bool canTransitionFrom(dynamic previousRoute) => true;

    public virtual void handleStartBackGesture(double progress = 0.0)
    {
        DartRuntimePrimitives.Assert(() => isCurrent);
        _controller?.value = progress;
        navigator?.didStartUserGesture();
    }

    public virtual void handleUpdateBackGestureProgress(double progress)
    {
        if (!isCurrent)
        {
            return;
        }
        _controller?.value = progress;
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
        if (isCurrent)
        {
            if (animateForward)
            {
                if (!_controller!.isCompleted)
                {
                    _controller!.forward();
                }
            }
            else
            {
                navigator?.pop<object>();
                if (_controller?.isAnimating ?? false)
                {
                    _controller!.reverse(from: _controller!.upperBound);
                }
            }
        }
        if (_controller?.isAnimating ?? false)
        {
            AnimationStatusListener animationStatusCallback = default!;
            animationStatusCallback = (status) =>
            {
                navigator?.didStopUserGesture();
                _controller!.removeStatusListener(animationStatusCallback);
            };
            _controller!.addStatusListener(animationStatusCallback);
        }
        else
        {
            navigator?.didStopUserGesture();
        }
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(
            () => !_transitionCompleter.isCompleted,
            () => (object?)$"Cannot dispose a {GetType()} twice."
        );
        DartRuntimePrimitives.Assert(
            () => !debugTransitionCompleted(),
            () => (object?)$"Cannot dispose a {GetType()} twice."
        );
        _animation?.removeStatusListener(_handleStatusChanged);
        _performanceModeRequestHandle?.dispose();
        _performanceModeRequestHandle = null;
        if (willDisposeAnimationController)
        {
            _controller?.dispose();
        }
        _transitionCompleter.complete(_result);
        base.dispose();
    }

    public virtual string debugLabel =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute");

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "TransitionRoute")}(animation: {_controller})";
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
    public virtual Action? onRemove { get; private set; }
    internal virtual ILocalHistoryRoute? _owner { get; set; } = default;
    public virtual bool impliesAppBarDismissal { get; private set; } = default!;

    public LocalHistoryEntry(Action? onRemove = null, bool impliesAppBarDismissal = true)
    {
        this.onRemove = onRemove;
        this.impliesAppBarDismissal = impliesAppBarDismissal;
    }

    public virtual void remove()
    {
        _owner?.removeLocalHistoryEntry(this);
        DartRuntimePrimitives.Assert(() => _owner is null);
    }

    internal virtual void _notifyRemoved()
    {
        onRemove?.Invoke();
    }
}

public interface ILocalHistoryRoute
{
    void addLocalHistoryEntry(LocalHistoryEntry entry);
    void removeLocalHistoryEntry(LocalHistoryEntry entry);
}

public interface LocalHistoryRoute<T> : ILocalHistoryRoute
{
    List<LocalHistoryEntry>? _localHistory { get; set; }
    long _entriesImpliesAppBarDismissal { get; set; }

    public new void addLocalHistoryEntry(LocalHistoryEntry entry);
    public new void removeLocalHistoryEntry(LocalHistoryEntry entry);
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
        IModalRoute route = ModalRoute<object>.untypedOf(this.context)!;
        return route.barrierDismissible;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override object? invoke(DismissIntent intent, BuildContext? context = null)
    {
        return Navigator.of(this.context).maybePop<object>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
    popDisposition,
}

internal class _ModalScopeStatus__routes : InheritedModel<_ModalRouteAspect__routes>
{
    public virtual bool isCurrent { get; private set; } = default!;
    public virtual bool canPop { get; private set; } = default!;
    public virtual bool impliesAppBarDismissal { get; private set; } = default!;
    public virtual bool opaque { get; private set; } = default!;
    public virtual RouteBase route { get; private set; } = default!;

    internal _ModalScopeStatus__routes(
        bool isCurrent,
        bool canPop,
        bool impliesAppBarDismissal,
        RouteBase route,
        bool opaque,
        Widget child
    )
        : base(child: child)
    {
        this.isCurrent = isCurrent;
        this.canPop = canPop;
        this.impliesAppBarDismissal = impliesAppBarDismissal;
        this.route = route;
        this.opaque = opaque;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_ModalScopeStatus__routes)oldWidget;
        return (isCurrent != __old.isCurrent)
            || (canPop != __old.canPop)
            || (impliesAppBarDismissal != __old.impliesAppBarDismissal)
            || (!Equals(route, __old.route))
            || (opaque != __old.opaque);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(
            new FlagProperty("isCurrent", value: isCurrent, ifTrue: "active", ifFalse: "inactive")
        );
        description.add(new FlagProperty("canPop", value: canPop, ifTrue: "can pop"));
        description.add(
            new FlagProperty(
                "impliesAppBarDismissal",
                value: impliesAppBarDismissal,
                ifTrue: "implies app bar dismissal"
            )
        );
    }

    public override bool updateShouldNotifyDependent(
        InheritedModel<_ModalRouteAspect__routes> oldWidget,
        HashSet<_ModalRouteAspect__routes> dependencies
    )
    {
        var __oldWidget = (_ModalScopeStatus__routes)oldWidget;
        return dependencies.any(
            (dependency) =>
                dependency switch
                {
                    _ModalRouteAspect__routes.isCurrent => isCurrent != __oldWidget.isCurrent,
                    _ModalRouteAspect__routes.canPop => canPop != __oldWidget.canPop,
                    _ModalRouteAspect__routes.settings => !Equals(
                        route.settings,
                        __oldWidget.route.settings
                    ),
                    _ModalRouteAspect__routes.isActive => route.isActive
                        != __oldWidget.route.isActive,
                    _ModalRouteAspect__routes.isFirst => route.isFirst != __oldWidget.route.isFirst,
                    _ModalRouteAspect__routes.opaque => opaque != __oldWidget.opaque,
                    _ModalRouteAspect__routes.popDisposition => !Equals(
                        route.popDisposition,
                        __oldWidget.route.popDisposition
                    ),
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _ModalScope__routes<T> : StatefulWidget
{
    public virtual ModalRoute<T> route { get; private set; } = default!;

    internal _ModalScope__routes(Key? key = null, ModalRoute<T> route = default!)
        : base(key: key)
    {
        this.route = route;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ModalScopeState__routes<T>());
}

public class _ModalScopeState__routes<T> : State<_ModalScope__routes<T>>
{
    internal virtual Widget? _page { get; set; } = default;
    internal virtual Listenable _listenable { get; set; } = default!;
    public virtual FocusScopeNode focusScopeNode { get; private set; } =
        new FocusScopeNode(debugLabel: $"{typeof(_ModalScopeState__routes<T>)} Focus Scope");
    public virtual ScrollController primaryScrollController { get; private set; } =
        new ScrollController();

    public override void initState()
    {
        base.initState();
        var animations = new List<Listenable>();
        _listenable = Listenable.CreateMerge(animations.Cast<Listenable?>());
    }

    public override void didUpdateWidget(_ModalScope__routes<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(widget.route, oldWidget.route));
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
        ModalRoute<T> routeLocal = widget.route;
        if (routeLocal.traversalEdgeBehavior is not null)
        {
            traversalEdgeBehaviorLocal = (
                routeLocal.traversalEdgeBehavior
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        else
        {
            traversalEdgeBehaviorLocal = routeLocal.navigator!.widget.routeTraversalEdgeBehavior;
        }
        if (routeLocal.directionalTraversalEdgeBehavior is not null)
        {
            directionalTraversalEdgeBehaviorLocal = (
                routeLocal.directionalTraversalEdgeBehavior
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        else
        {
            directionalTraversalEdgeBehaviorLocal = routeLocal
                .navigator!
                .widget
                .routeDirectionalTraversalEdgeBehavior;
        }
        focusScopeNode.traversalEdgeBehavior = traversalEdgeBehaviorLocal;
        focusScopeNode.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehaviorLocal;
        if (routeLocal.isCurrent && _shouldRequestFocus)
        {
            routeLocal.navigator!.focusNode.enclosingScope?.setFirstFocus(focusScopeNode);
        }
    }

    internal virtual void _forceRebuildPage()
    {
        setState(() =>
        {
            _page = null;
        });
    }

    public override void dispose()
    {
        focusScopeNode.dispose();
        primaryScrollController.dispose();
        base.dispose();
    }

    internal virtual bool _shouldIgnoreFocusRequest
    {
        get
        {
            return Equals(widget.route.animation?.status, AnimationStatus.reverse)
                || (widget.route.navigator?.userGestureInProgress ?? false);
        }
    }
    internal virtual bool _shouldRequestFocus
    {
        get { return widget.route.requestFocus; }
    }

    internal virtual void _routeSetState(Action fn)
    {
        if (widget.route.isCurrent && !_shouldIgnoreFocusRequest && _shouldRequestFocus)
        {
            widget.route.navigator!.focusNode.enclosingScope?.setFirstFocus(focusScopeNode);
        }
        setState(() => fn());
    }

    public override Widget build(BuildContext context)
    {
        focusScopeNode.skipTraversal = !widget.route.isCurrent;
        return new AnimatedBuilder(
            animation: widget.route.restorationScopeId,
            builder: (context, child) =>
            {
                DartRuntimePrimitives.Assert(() => child is not null);
                return new RestorationScope(
                    restorationId: widget.route.restorationScopeId.value,
                    child: child!
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: new _ModalScopeStatus__routes(
                route: widget.route,
                isCurrent: widget.route.isCurrent,
                canPop: widget.route.canPop,
                opaque: widget.route.opaque,
                impliesAppBarDismissal: widget.route.impliesAppBarDismissal,
                child: new Offstage(
                    offstage: widget.route.offstage,
                    child: new PageStorage(
                        bucket: widget.route._storageBucket,
                        child: new Builder(
                            builder: (context) =>
                            {
                                return new Actions(
                                    actions: new DartMap<Type, dynamic>
                                    {
                                        [typeof(DismissIntent)] = new _DismissModalAction__routes(
                                            context
                                        ),
                                    },
                                    child: new PrimaryScrollController(
                                        controller: primaryScrollController,
                                        child: FocusScope.CreateWithExternalFocusNode(
                                            focusScopeNode: focusScopeNode,
                                            child: new RepaintBoundary(
                                                child: new ListenableBuilder(
                                                    listenable: _listenable,
                                                    builder: (context, child) =>
                                                    {
                                                        return widget.route._buildFlexibleTransitions(
                                                            context,
                                                            widget.route.animation!,
                                                            widget.route.secondaryAnimation!,
                                                            new ListenableBuilder(
                                                                listenable: widget
                                                                    .route
                                                                    .navigator
                                                                    ?.userGestureInProgressNotifier
                                                                    ?? new ValueNotifier<bool>(
                                                                        false
                                                                    ),
                                                                builder: (context, child) =>
                                                                {
                                                                    bool ignoreEvents =
                                                                        _shouldIgnoreFocusRequest;
                                                                    focusScopeNode.canRequestFocus =
                                                                        !ignoreEvents;
                                                                    return new IgnorePointer(
                                                                        ignoring: ignoreEvents,
                                                                        child: child
                                                                    );
                                                                    throw new InvalidOperationException(
                                                                        "Callback completed without returning a value."
                                                                    );
                                                                },
                                                                child: child
                                                            )
                                                        );
                                                        throw new InvalidOperationException(
                                                            "Callback completed without returning a value."
                                                        );
                                                    },
                                                    child: _page ??= new RepaintBoundary(
                                                        key: widget.route._subtreeKey,
                                                        child: new Builder(
                                                            builder: (context) =>
                                                            {
                                                                return widget.route.buildPage(
                                                                    context,
                                                                    widget.route.animation!,
                                                                    widget.route.secondaryAnimation!
                                                                );
                                                                throw new InvalidOperationException(
                                                                    "Callback completed without returning a value."
                                                                );
                                                            }
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                );
                                throw new InvalidOperationException(
                                    "Callback completed without returning a value."
                                );
                            }
                        )
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public interface IModalRoute : ITransitionRoute, PredictiveBackRoute, ILocalHistoryRoute
{
    RouteBase routeBase { get; }
    NavigatorState? navigator { get; }
    RouteSettings settings { get; }
    new bool isCurrent { get; }
    bool isFirst { get; }
    bool isActive { get; }
    bool canPop { get; }
    bool impliesAppBarDismissal { get; }
    bool fullscreenDialog { get; }
    bool maintainState { get; }
    bool barrierDismissible { get; }
    bool popGestureInProgress { get; }
    Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        bool,
        Widget?,
        Widget?
    >? delegatedTransition { get; }
    bool offstage { get; set; }
    BuildContext? subtreeContext { get; }
    void registerPopEntry(IPopEntry entry);
    void unregisterPopEntry(IPopEntry entry);
    void addScopedWillPopCallback(Func<Future<bool>> callback);
    void removeScopedWillPopCallback(Func<Future<bool>> callback);
}

public interface IPageRoute : IModalRoute { }

public abstract class ModalRoute<T> : TransitionRoute<T>, LocalHistoryRoute<T>, IModalRoute
{
    RouteBase IModalRoute.routeBase => this;
    public virtual ImageFilter? filter { get; private set; }
    public virtual TraversalEdgeBehavior? traversalEdgeBehavior { get; private set; }
    public virtual TraversalEdgeBehavior? directionalTraversalEdgeBehavior { get; private set; }
    public virtual Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        bool,
        Widget?,
        Widget?
    >? receivedTransition { get; set; } = default;
    internal virtual bool _offstage { get; set; } = false;
    internal virtual ProxyAnimation? _animationProxy { get; set; } = default;
    internal virtual ProxyAnimation? _secondaryAnimationProxy { get; set; } = default;
    internal virtual List<Func<Future<bool>>> _willPopCallbacks { get; private set; } =
        new List<Func<Future<bool>>>();
    internal virtual HashSet<IPopEntry> _popEntries { get; private set; } =
        new HashSet<IPopEntry>();
    internal virtual GlobalKey<_ModalScopeState__routes<T>> _scopeKey { get; private set; } =
        GlobalKey<_ModalScopeState__routes<T>>.Create();
    internal virtual GlobalKey<IState> _subtreeKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual PageStorageBucket _storageBucket { get; private set; } =
        new PageStorageBucket();
    internal virtual OverlayEntry _modalBarrier { get; set; } = default!;
    internal virtual Widget? _modalScopeCache { get; set; } = default;
    internal virtual OverlayEntry _modalScope { get; set; } = default!;
    public virtual List<LocalHistoryEntry>? _localHistory { get; set; } = default;
    public virtual long _entriesImpliesAppBarDismissal { get; set; } = 0L;

    protected ModalRoute(
        RouteSettings? settings = null,
        bool? requestFocus = null,
        ImageFilter? filter = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null
    )
        : base(settings: settings, requestFocus: requestFocus)
    {
        this.filter = filter;
        this.traversalEdgeBehavior = traversalEdgeBehavior;
        this.directionalTraversalEdgeBehavior = directionalTraversalEdgeBehavior;
    }

    public static ModalRoute<TRouteResult>? of<TRouteResult>(BuildContext context)
    {
        return ModalRoute<TRouteResult>._of<TRouteResult>(context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static ModalRoute<TRouteResult>? _of<TRouteResult>(
        BuildContext context,
        _ModalRouteAspect__routes? aspect = null
    )
    {
        return (
            (ModalRoute<TRouteResult>?)
                InheritedModel<object>
                    .inheritFrom<_ModalScopeStatus__routes>(context, aspect: aspect)
                    ?.route
        )!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static IModalRoute? untypedOf(BuildContext context) =>
        context.dependOnInheritedWidgetOfExactType<_ModalScopeStatus__routes>()?.route
        as IModalRoute;

    private static _ModalScopeStatus__routes? ScopeOf(
        BuildContext context,
        _ModalRouteAspect__routes aspect
    ) => InheritedModel<object>.inheritFrom<_ModalScopeStatus__routes>(context, aspect: aspect);

    // These queries do not depend on the route's result type. Search uses a string
    // route inside object-valued navigators; invariant generic casts are invalid here.
    public static bool? isCurrentOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.isCurrent)?.isCurrent;

    public static bool? canPopOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.canPop)?.canPop;

    public static RouteSettings? settingsOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.settings)?.route.settings;

    public static bool? isActiveOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.isActive)?.route.isActive;

    public static bool? isFirstOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.isFirst)?.route.isFirst;

    public static bool? opaqueOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.opaque)?.opaque;

    public static RoutePopDisposition? popDispositionOf(BuildContext context) =>
        ScopeOf(context, _ModalRouteAspect__routes.popDisposition)?.route.popDisposition;

    public virtual void setState(Action fn)
    {
        if (_scopeKey.currentState is not null)
        {
            _scopeKey.currentState!._routeSetState(() => fn());
        }
        else
        {
            fn();
        }
    }

    public static Func<dynamic, bool> withName(string name)
    {
        return (route) =>
        {
            return ((object?)route is ModalRoute<T> typedRoute)
                && !typedRoute.willHandlePopInternally
                && (typedRoute.settings.ToString() == name);
            throw new InvalidOperationException("Callback completed without returning a value.");
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    );

    public virtual Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return child;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        bool,
        Widget?,
        Widget?
    >? delegatedTransition =>
        DartRuntimePrimitives.ConvertValue<
            Func<BuildContext, Animation<double>, Animation<double>, bool, Widget?, Widget?>
        >(null);

    internal virtual Widget _buildFlexibleTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        if ((receivedTransition is null) || secondaryAnimation.isDismissed)
        {
            return buildTransitions(context, animation, secondaryAnimation, child);
        }
        var proxyAnimation = new ProxyAnimation();
        Widget proxiedOriginalTransitions = buildTransitions(
            context,
            animation,
            proxyAnimation,
            child
        );
        return receivedTransition!(
                context,
                animation,
                secondaryAnimation,
                allowSnapshotting,
                proxiedOriginalTransitions
            ) ?? proxiedOriginalTransitions;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void install()
    {
        base.install();
        _animationProxy = new ProxyAnimation(base.animation);
        _secondaryAnimationProxy = new ProxyAnimation(base.secondaryAnimation);
    }

    public override Scheduler.TickerFuture didPush()
    {
        if ((_scopeKey.currentState is not null) && navigator!.widget.requestFocus)
        {
            navigator!.focusNode.enclosingScope?.setFirstFocus(
                _scopeKey.currentState!.focusScopeNode
            );
        }
        return base.didPush();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void didAdd()
    {
        if ((_scopeKey.currentState is not null) && navigator!.widget.requestFocus)
        {
            navigator!.focusNode.enclosingScope?.setFirstFocus(
                _scopeKey.currentState!.focusScopeNode
            );
        }
        base.didAdd();
    }

    public abstract bool barrierDismissible { get; }
    public virtual bool semanticsDismissible => true;
    public abstract Color? barrierColor { get; }
    public abstract string? barrierLabel { get; }
    public virtual Curve barrierCurve => DartRuntimePrimitives.ConvertValue<Curve>(Curves.ease);
    public abstract bool maintainState { get; }
    public virtual bool popGestureInProgress => navigator!.userGestureInProgress;
    public override bool popGestureEnabled
    {
        get
        {
            if (isFirst)
            {
                return false;
            }
            if (willHandlePopInternally)
            {
                return false;
            }
            if (hasScopedWillPopCallback || Equals(popDisposition, RoutePopDisposition.doNotPop))
            {
                return false;
            }
            if (!animation!.isCompleted)
            {
                return false;
            }
            return true;
        }
    }
    public virtual bool offstage
    {
        get => _offstage;
        set
        {
            var __value = value;
            if (_offstage == __value)
            {
                return;
            }
            setState(() =>
            {
                _offstage = __value;
            });
            _animationProxy!.parent = _offstage
                ? AnimationsLibrary.kAlwaysCompleteAnimation
                : base.animation;
            _secondaryAnimationProxy!.parent = _offstage
                ? AnimationsLibrary.kAlwaysDismissedAnimation
                : base.secondaryAnimation;
            changedInternalState();
        }
    }
    public virtual BuildContext? subtreeContext => _subtreeKey.currentContext;
    public override Animation<double>? animation =>
        DartRuntimePrimitives.ConvertValue<Animation<double>>(_animationProxy);
    public override Animation<double>? secondaryAnimation =>
        DartRuntimePrimitives.ConvertValue<Animation<double>>(_secondaryAnimationProxy);

    public override async Future<RoutePopDisposition> willPop()
    {
        _ModalScopeState__routes<T>? scope = _scopeKey.currentState;
        DartRuntimePrimitives.Assert(() => scope is not null);
        foreach (
            var callback in new List<Func<Future<bool>>>(
                DartRuntimePrimitives.ConvertEnumerable<Func<Future<bool>>>(_willPopCallbacks)
            )
        )
        {
            if (!await callback())
            {
                return RoutePopDisposition.doNotPop;
            }
        }
        return await base.willPop();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override RoutePopDisposition popDisposition
    {
        get
        {
            foreach (IPopEntry popEntry in _popEntries)
            {
                if (!popEntry.canPopNotifier.value)
                {
                    return RoutePopDisposition.doNotPop;
                }
            }
            return base.popDisposition;
        }
    }

    public override void onPopInvokedWithResult(bool didPop, T? result)
    {
        foreach (IPopEntry popEntry in _popEntries)
        {
            popEntry.onPopInvokedWithResultObject(didPop, result);
        }
        base.onPopInvokedWithResult(didPop, result);
    }

    public virtual void addScopedWillPopCallback(Func<Future<bool>> callback)
    {
        DartRuntimePrimitives.Assert(
            () => _scopeKey.currentState is not null,
            () =>
                (object?)
                    "Tried to add a willPop callback to a route that is not currently in the tree."
        );
        _willPopCallbacks.Add(callback);
        if (checked(_willPopCallbacks.Count) == 1L)
        {
            _maybeDispatchNavigationNotification();
        }
    }

    public virtual void removeScopedWillPopCallback(Func<Future<bool>> callback)
    {
        DartRuntimePrimitives.Assert(
            () => _scopeKey.currentState is not null,
            () =>
                (object?)
                    "Tried to remove a willPop callback from a route that is not currently in the tree."
        );
        _willPopCallbacks.Remove(callback);
        if (!Enumerable.Any(_willPopCallbacks))
        {
            _maybeDispatchNavigationNotification();
        }
    }

    public virtual void registerPopEntry(IPopEntry popEntry)
    {
        _popEntries.Add(popEntry);
        popEntry.canPopNotifier.addListener(_maybeDispatchNavigationNotification);
        _maybeDispatchNavigationNotification();
    }

    public virtual void unregisterPopEntry(IPopEntry popEntry)
    {
        _popEntries.Remove(popEntry);
        popEntry.canPopNotifier.removeListener(_maybeDispatchNavigationNotification);
        _maybeDispatchNavigationNotification();
    }

    internal virtual void _maybeDispatchNavigationNotification()
    {
        if (!isCurrent)
        {
            return;
        }
        var notification = new NavigationNotification(
            canHandlePop: Equals(popDisposition, RoutePopDisposition.doNotPop)
                || Enumerable.Any(_willPopCallbacks)
        );
        switch (Scheduler.SchedulerBinding.instance.schedulerPhase)
        {
            case Scheduler.SchedulerPhase.postFrameCallbacks:
            {
                notification.dispatch(subtreeContext);
                break;
            }
            case Scheduler.SchedulerPhase.idle:
            case Scheduler.SchedulerPhase.midFrameMicrotasks:
            case Scheduler.SchedulerPhase.persistentCallbacks:
            case Scheduler.SchedulerPhase.transientCallbacks:
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (timeStamp) =>
                    {
                        if (!(subtreeContext?.mounted ?? false))
                        {
                            return;
                        }
                        notification.dispatch(subtreeContext);
                    },
                    debugLabel: "ModalRoute.dispatchNotification"
                );
                break;
            }
        }
    }

    public virtual bool hasScopedWillPopCallback
    {
        get { return Enumerable.Any(_willPopCallbacks); }
    }

    public override void didChangePrevious(dynamic? previousRoute)
    {
        base.didChangePrevious((object?)previousRoute);
        changedInternalState();
    }

    public override void didChangeNext(dynamic? nextRoute)
    {
        if (
            (nextRoute is IModalRoute)
            && canTransitionTo((object)nextRoute)
            && (!Equals(((IModalRoute)(object)nextRoute).delegatedTransition, delegatedTransition))
        )
        {
            IModalRoute nextRoute__as84419 = (IModalRoute)(object)nextRoute;
            receivedTransition = nextRoute__as84419.delegatedTransition;
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
        if (
            (nextRoute is IModalRoute)
            && canTransitionTo((object)nextRoute)
            && (!Equals(((IModalRoute)(object)nextRoute).delegatedTransition, delegatedTransition))
        )
        {
            IModalRoute nextRoute__as84796 = (IModalRoute)(object)nextRoute;
            receivedTransition = nextRoute__as84796.delegatedTransition;
        }
        else
        {
            receivedTransition = null;
        }
        base.didPopNext((object)nextRoute);
        changedInternalState();
        _maybeDispatchNavigationNotification();
    }

    public override void changedInternalState()
    {
        base.changedInternalState();
        if (
            !Equals(
                Scheduler.SchedulerBinding.instance.schedulerPhase,
                Scheduler.SchedulerPhase.persistentCallbacks
            )
        )
        {
            setState(() => { });
            _modalBarrier.markNeedsBuild();
        }
        _modalScope.maintainState = maintainState;
    }

    public override void changedExternalState()
    {
        base.changedExternalState();
        _modalBarrier.markNeedsBuild();
        if (_scopeKey.currentState is not null)
        {
            _scopeKey.currentState!._forceRebuildPage();
        }
    }

    public virtual bool canPop =>
        DartRuntimePrimitives.ConvertValue<bool>(hasActiveRouteBelow || willHandlePopInternally);
    public virtual bool impliesAppBarDismissal =>
        DartRuntimePrimitives.ConvertValue<bool>(
            hasActiveRouteBelow || (_entriesImpliesAppBarDismissal > 0L)
        );
    public virtual bool fullscreenDialog => false;

    internal virtual Widget _buildModalBarrier(BuildContext context)
    {
        Widget barrier = buildModalBarrier();
        if (filter is not null)
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(
                new BackdropFilter(filter: filter, child: barrier)
            );
        }
        barrier = DartRuntimePrimitives.ConvertValue<Widget>(
            new IgnorePointer(ignoring: !animation!.isForwardOrCompleted, child: barrier)
        );
        if (semanticsDismissible && barrierDismissible)
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(
                new Semantics(sortKey: new OrdinalSortKey(1.0), child: barrier)
            );
        }
        return barrier;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Widget buildModalBarrier()
    {
        Widget barrier = default!;
        if ((barrierColor is not null) && (barrierColor!.alpha != 0L) && !offstage)
        {
            DartRuntimePrimitives.Assert(() =>
                !Equals(barrierColor, barrierColor!.withOpacity(0.0))
            );
            Animation<Color?> colorLocal = animation!.drive(
                new ColorTween(begin: barrierColor!.withOpacity(0.0), end: barrierColor).chain(
                    new CurveTween(curve: barrierCurve)
                )
            );
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedModalBarrier(
                    color: colorLocal,
                    dismissible: barrierDismissible,
                    semanticsLabel: barrierLabel,
                    barrierSemanticsDismissible: semanticsDismissible
                )
            );
        }
        else
        {
            barrier = DartRuntimePrimitives.ConvertValue<Widget>(
                new ModalBarrier(
                    dismissible: barrierDismissible,
                    semanticsLabel: barrierLabel,
                    barrierSemanticsDismissible: semanticsDismissible
                )
            );
        }
        return barrier;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _buildModalScope(BuildContext context)
    {
        return _modalScopeCache ??= new Semantics(
            sortKey: new OrdinalSortKey(0.0),
            child: new _ModalScope__routes<T>(key: _scopeKey, route: this)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IEnumerable<OverlayEntry> createOverlayEntries()
    {
        return new List<OverlayEntry>
        {
            (_modalBarrier = new OverlayEntry(builder: _buildModalBarrier)),
            (
                _modalScope = new OverlayEntry(
                    builder: _buildModalScope,
                    maintainState: maintainState,
                    canSizeOverlay: opaque
                )
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ModalRoute")}({settings}, animation: {_animation})";

    public virtual void addLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => entry._owner is null);
        entry._owner = this;
        _localHistory ??= new List<LocalHistoryEntry>();
        bool wasEmpty = !Enumerable.Any(_localHistory!);
        _localHistory!.Add(entry);
        var internalStateChanged = false;
        if (entry.impliesAppBarDismissal)
        {
            internalStateChanged = _entriesImpliesAppBarDismissal == 0L;
            _entriesImpliesAppBarDismissal += 1L;
        }
        if (wasEmpty || internalStateChanged)
        {
            changedInternalState();
        }
    }

    public virtual void removeLocalHistoryEntry(LocalHistoryEntry entry)
    {
        DartRuntimePrimitives.Assert(() => Equals(entry._owner, this));
        DartRuntimePrimitives.Assert(() => _localHistory!.Contains(entry));
        var internalStateChanged = false;
        if (_localHistory!.Remove(entry) && entry.impliesAppBarDismissal)
        {
            _entriesImpliesAppBarDismissal -= 1L;
            internalStateChanged = _entriesImpliesAppBarDismissal == 0L;
        }
        entry._owner = null;
        entry._notifyRemoved();
        if (!Enumerable.Any(_localHistory!) || internalStateChanged)
        {
            DartRuntimePrimitives.Assert(() => _entriesImpliesAppBarDismissal == 0L);
            if (
                Equals(
                    Scheduler.SchedulerBinding.instance.schedulerPhase,
                    Scheduler.SchedulerPhase.persistentCallbacks
                )
            )
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (duration) =>
                    {
                        if (isActive)
                        {
                            changedInternalState();
                        }
                    },
                    debugLabel: "LocalHistoryRoute.changedInternalState"
                );
            }
            else
            {
                changedInternalState();
            }
        }
    }

    public override bool didPop(T? result)
    {
        if ((_localHistory is not null) && Enumerable.Any(_localHistory!))
        {
            LocalHistoryEntry entry = _localHistory!.removeLast();
            DartRuntimePrimitives.Assert(() => Equals(entry._owner, this));
            entry._owner = null;
            entry._notifyRemoved();
            var internalStateChanged = false;
            if (entry.impliesAppBarDismissal)
            {
                _entriesImpliesAppBarDismissal -= 1L;
                internalStateChanged = _entriesImpliesAppBarDismissal == 0L;
            }
            if (!Enumerable.Any(_localHistory!) || internalStateChanged)
            {
                changedInternalState();
            }
            return false;
        }
        return base.didPop(result);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool willHandlePopInternally
    {
        get { return (_localHistory is not null) && Enumerable.Any(_localHistory!); }
    }
}

public abstract class PopupRoute<T> : ModalRoute<T>
{
    protected PopupRoute(
        RouteSettings? settings = null,
        bool? requestFocus = null,
        ImageFilter? filter = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null
    )
        : base(
            settings: settings,
            requestFocus: requestFocus,
            filter: filter,
            traversalEdgeBehavior: traversalEdgeBehavior,
            directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior
        ) { }

    public override bool opaque => false;
    public override bool maintainState => true;
    public override bool allowSnapshotting => false;
}

public class RouteObserver<R> : NavigatorObserver
    where R : notnull
{
    internal virtual DartMap<R, HashSet<RouteAware>> _listeners { get; private set; } =
        new DartMap<R, HashSet<RouteAware>>();

    public virtual bool debugObservingRoute(R route)
    {
        bool contained = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            contained = _listeners.ContainsKey(route);
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return contained;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void subscribe(RouteAware routeAware, R route)
    {
        HashSet<RouteAware> subscribers = _listeners.putIfAbsent(
            route,
            () => new HashSet<RouteAware>()
        );
        if (subscribers.Add(routeAware))
        {
            routeAware.didPush();
        }
    }

    public virtual void unsubscribe(RouteAware routeAware)
    {
        List<R> routes = _listeners.Keys.ToList().ToList();
        foreach (var route in routes)
        {
            HashSet<RouteAware>? subscribers = _listeners.GetValueOrDefault(route);
            if (subscribers is not null)
            {
                subscribers.Remove(routeAware);
                if (!Enumerable.Any(subscribers))
                {
                    _listeners.remove(route);
                }
            }
        }
    }

    public override void didPop(dynamic route, dynamic? previousRoute)
    {
        if ((route is R) && (previousRoute is R))
        {
            R route__as94544 = (R)(object)route;
            R previousRoute__as94558 = (R)(object)previousRoute;
            List<RouteAware>? previousSubscribers = _listeners
                .GetValueOrDefault(previousRoute__as94558)
                ?.ToList()
                .ToList();
            if (previousSubscribers is not null)
            {
                foreach (RouteAware routeAware in previousSubscribers)
                {
                    routeAware.didPopNext();
                }
            }
            List<RouteAware>? subscribers = _listeners
                .GetValueOrDefault(route__as94544)
                ?.ToList()
                .ToList();
            if (subscribers is not null)
            {
                foreach (RouteAware routeAwareLocal in subscribers)
                {
                    routeAwareLocal.didPop();
                }
            }
        }
    }

    public override void didPush(dynamic route, dynamic? previousRoute)
    {
        if ((route is R) && (previousRoute is R))
        {
            R route__as95148 = (R)(object)route;
            R previousRoute__as95162 = (R)(object)previousRoute;
            HashSet<RouteAware>? previousSubscribers = _listeners.GetValueOrDefault(
                previousRoute__as95162
            );
            if (previousSubscribers is not null)
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
    public virtual void didPopNext() { }

    public virtual void didPush() { }

    public virtual void didPop() { }

    public virtual void didPushNext() { }
}

public class RawDialogRoute<T> : PopupRoute<T>
{
    internal virtual Func<BuildContext, Animation<double>, Animation<double>, Widget> _pageBuilder
    {
        get;
        private set;
    } = default!;
    internal virtual bool _barrierDismissible { get; private set; } = default!;
    internal virtual string? _barrierLabel { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual Duration _transitionDuration { get; private set; } = default!;
    internal virtual Func<
        BuildContext,
        Animation<double>,
        Animation<double>,
        Widget,
        Widget
    >? _transitionBuilder { get; private set; }
    public virtual Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder
    {
        get;
        private set;
    }
    public virtual Offset? anchorPoint { get; private set; }
    private bool __field_fullscreenDialog = default!;
    public override bool fullscreenDialog
    {
        get => __field_fullscreenDialog;
    }

    public RawDialogRoute(
        Func<BuildContext, Animation<double>, Animation<double>, Widget> pageBuilder,
        bool barrierDismissible = true,
        Color? barrierColor = default!,
        string? barrierLabel = null,
        Duration? transitionDuration = null,
        Func<
            BuildContext,
            Animation<double>,
            Animation<double>,
            Widget,
            Widget
        >? transitionBuilder = null,
        Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null,
        RouteSettings? settings = null,
        bool? requestFocus = null,
        Offset? anchorPoint = null,
        TraversalEdgeBehavior? traversalEdgeBehavior = null,
        TraversalEdgeBehavior? directionalTraversalEdgeBehavior = null,
        bool fullscreenDialog = false
    )
        : base(
            settings: settings,
            requestFocus: requestFocus,
            traversalEdgeBehavior: traversalEdgeBehavior,
            directionalTraversalEdgeBehavior: directionalTraversalEdgeBehavior
        )
    {
        Color? __barrierColor = barrierColor ?? new Color(0x80000000);
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 200);
        this.barrierBuilder = barrierBuilder;
        this.anchorPoint = anchorPoint;
        __field_fullscreenDialog = fullscreenDialog;
        _pageBuilder = pageBuilder;
        _barrierDismissible = barrierDismissible;
        _barrierLabel = barrierLabel;
        _barrierColor = __barrierColor;
        _transitionDuration = __transitionDuration;
        _transitionBuilder = transitionBuilder;
    }

    public override bool barrierDismissible => _barrierDismissible;
    public override string? barrierLabel => _barrierLabel;
    public override Color? barrierColor => _barrierColor;
    public override Duration transitionDuration => _transitionDuration;

    public override Widget buildPage(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation
    )
    {
        return new Semantics(
            scopesRoute: true,
            explicitChildNodes: true,
            child: new DisplayFeatureSubScreen(
                anchorPoint: anchorPoint,
                child: _pageBuilder(context, animation, secondaryAnimation)
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildTransitions(
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        if (_transitionBuilder is null)
        {
            return new FadeTransition(opacity: animation, child: child);
        }
        return _transitionBuilder(context, animation, secondaryAnimation, child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget buildModalBarrier()
    {
        Widget barrier = base.buildModalBarrier();
        if (barrierBuilder is not null)
        {
            return new Builder(
                builder: (context) =>
                    barrierBuilder!(
                        context,
                        new RouteBarrierDetails(
                            animation: animation!,
                            barrierColor: barrierColor,
                            barrierLabel: barrierLabel,
                            barrierDismissible: barrierDismissible
                        ),
                        barrier
                    )
            );
        }
        return barrier;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class RoutesLibrary
{
    public static Future<T?> showGeneralDialog<T>(
        BuildContext context,
        Func<BuildContext, Animation<double>, Animation<double>, Widget> pageBuilder,
        bool barrierDismissible = false,
        string? barrierLabel = null,
        Color barrierColor = default!,
        Duration? transitionDuration = null,
        Func<
            BuildContext,
            Animation<double>,
            Animation<double>,
            Widget,
            Widget
        >? transitionBuilder = null,
        Func<BuildContext, RouteBarrierDetails, Widget, Widget>? barrierBuilder = null,
        bool useRootNavigator = true,
        bool fullscreenDialog = false,
        RouteSettings? routeSettings = null,
        Offset? anchorPoint = null,
        bool? requestFocus = null
    )
    {
        Duration __transitionDuration = transitionDuration ?? Duration.Create(milliseconds: 200);
        DartRuntimePrimitives.Assert(() => !barrierDismissible || (barrierLabel is not null));
        return Navigator
            .of(context, rootNavigator: useRootNavigator)
            .push(
                new RawDialogRoute<T>(
                    pageBuilder: pageBuilder,
                    barrierDismissible: barrierDismissible,
                    barrierLabel: barrierLabel,
                    barrierColor: barrierColor,
                    transitionDuration: __transitionDuration,
                    transitionBuilder: transitionBuilder,
                    barrierBuilder: barrierBuilder,
                    settings: routeSettings,
                    anchorPoint: anchorPoint,
                    requestFocus: requestFocus,
                    fullscreenDialog: fullscreenDialog
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public delegate Widget RoutePageBuilder(
    BuildContext context,
    Animation<double> animation,
    Animation<double> secondaryAnimation
);

public delegate Widget RouteTransitionsBuilder(
    BuildContext context,
    Animation<double> animation,
    Animation<double> secondaryAnimation,
    Widget child
);

public class RouteBarrierDetails
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Color? barrierColor { get; private set; }
    public virtual string? barrierLabel { get; private set; }
    public virtual bool barrierDismissible { get; private set; } = default!;

    public RouteBarrierDetails(
        Animation<double> animation,
        Color? barrierColor = null,
        string? barrierLabel = null,
        bool barrierDismissible = default!
    )
    {
        this.animation = animation;
        this.barrierColor = barrierColor;
        this.barrierLabel = barrierLabel;
        this.barrierDismissible = barrierDismissible;
    }
}

public delegate Widget RouteBarrierBuilder(
    BuildContext context,
    RouteBarrierDetails details,
    Widget barrier
);

public delegate void PopInvokedWithResultCallback<T>(bool didPop, T? result);

public interface IPopEntry
{
    ValueListenable<bool> canPopNotifier { get; }
    void onPopInvokedWithResultObject(bool didPop, object? result);
}

public abstract class PopEntry<T> : IPopEntry
{
    void IPopEntry.onPopInvokedWithResultObject(bool didPop, object? result) =>
        onPopInvokedWithResult(didPop, result is null ? default : (T)result);

    public PopEntry() { }

    public virtual void onPopInvoked(bool didPop) { }

    public virtual void onPopInvokedWithResult(bool didPop, T? result) => onPopInvoked(didPop);

    public abstract ValueListenable<bool> canPopNotifier { get; }

    public override string ToString()
    {
        return $"PopEntry canPop: {canPopNotifier.value}, onPopInvoked: {onPopInvokedWithResult}";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
